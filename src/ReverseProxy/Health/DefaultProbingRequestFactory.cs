// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using Yarp.ReverseProxy.Model;

namespace Yarp.ReverseProxy.Health;

internal sealed class DefaultProbingRequestFactory : IProbingRequestFactory
{
    private static readonly string? _version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    private static readonly string _defaultUserAgent = $"YARP{(string.IsNullOrEmpty(_version) ? "" : $"/{_version.Split('+')[0]}")} (Active Health Check Monitor)";

    public HttpRequestMessage CreateRequest(ClusterModel cluster, DestinationModel destination)
    {
        var probeAddress = !string.IsNullOrEmpty(destination.Config.Health) ? destination.Config.Health : destination.Config.Address;
        var probePath = cluster.Config.HealthCheck?.Active?.Path;
        UriHelper.FromAbsolute(probeAddress, out var destinationScheme, out var destinationHost, out var destinationPathBase, out _, out _);
        var probeUri = UriHelper.BuildAbsolute(destinationScheme, destinationHost, destinationPathBase, probePath, default);

        var request = new HttpRequestMessage(HttpMethod.Get, probeUri)
        {
            Version = cluster.Config.HttpRequest?.Version ?? HttpVersion.Version20,
#if NET
            VersionPolicy = cluster.Config.HttpRequest?.VersionPolicy ?? HttpVersionPolicy.RequestVersionOrLower
#endif
        };

        request.Headers.Add(HeaderNames.UserAgent, _defaultUserAgent);

        return request;
    }
}
