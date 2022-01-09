// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Model;
using Yarp.ReverseProxy.Utilities;

namespace Yarp.ReverseProxy.Forwarder;

/// <summary>
/// Invokes the proxy at the end of the request processing pipeline.
/// </summary>
internal sealed class ForwarderMiddleware
{
    private readonly IRandomFactory _randomFactory;
    private readonly RequestDelegate _next; // Unused, this middleware is always terminal
    private readonly ILogger _logger;
    private readonly IHttpForwarder _forwarder;

    public ForwarderMiddleware(RequestDelegate next, ILogger<ForwarderMiddleware> logger, IHttpForwarder forwarder, IRandomFactory randomFactory)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _forwarder = forwarder ?? throw new ArgumentNullException(nameof(forwarder));
        _randomFactory = randomFactory ?? throw new ArgumentNullException(nameof(randomFactory));
    }

    /// <inheritdoc/>
    public async Task Invoke(HttpContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        var reverseProxyFeature = context.GetReverseProxyFeature();
        var destinations = reverseProxyFeature.AvailableDestinations
            ?? throw new InvalidOperationException($"The {nameof(IReverseProxyFeature)} Destinations collection was not set.");

        var route = context.GetRouteModel();
        var cluster = route.Cluster!;

        if (destinations.Count == 0)
        {
            Log.NoAvailableDestinations(_logger, cluster.ClusterId);
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Features.Set<IForwarderErrorFeature>(new ForwarderErrorFeature(ForwarderError.NoAvailableDestinations, ex: null));
            return;
        }

        var destination = destinations[0];
        if (destinations.Count > 1)
        {
            var random = _randomFactory.CreateRandomInstance();
            Log.MultipleDestinationsAvailable(_logger, cluster.ClusterId);
            destination = destinations[random.Next(destinations.Count)];
        }

        reverseProxyFeature.ProxiedDestination = destination;

        var destinationModel = destination.Model;
        if (destinationModel == null)
        {
            throw new InvalidOperationException($"Chosen destination has no model set: '{destination.DestinationId}'");
        }

        try
        {
            cluster.ConcurrencyCounter.Increment();
            destination.ConcurrencyCounter.Increment();

            ForwarderTelemetry.Log.ForwarderInvoke(cluster.ClusterId, route.Config.RouteId, destination.DestinationId);

            var clusterConfig = reverseProxyFeature.Cluster;
            await _forwarder.SendAsync(context, destinationModel.Config.Address, clusterConfig.HttpClient,
                clusterConfig.Config.HttpRequest ?? ForwarderRequestConfig.Empty, route.Transformer);
        }
        finally
        {
            destination.ConcurrencyCounter.Decrement();
            cluster.ConcurrencyCounter.Decrement();
        }
    }

    private static class Log
    {
        private static readonly Action<ILogger, string, Exception?> _noAvailableDestinations = LoggerMessage.Define<string>(
            LogLevel.Warning,
            EventIds.NoAvailableDestinations,
            "No available destinations after load balancing for cluster '{clusterId}'.");

        private static readonly Action<ILogger, string, Exception?> _multipleDestinationsAvailable = LoggerMessage.Define<string>(
            LogLevel.Warning,
            EventIds.MultipleDestinationsAvailable,
            "More than one destination available for cluster '{clusterId}', load balancing may not be configured correctly. Choosing randomly.");

        public static void NoAvailableDestinations(ILogger logger, string clusterId)
        {
            _noAvailableDestinations(logger, clusterId, null);
        }

        public static void MultipleDestinationsAvailable(ILogger logger, string clusterId)
        {
            _multipleDestinationsAvailable(logger, clusterId, null);
        }
    }
}
