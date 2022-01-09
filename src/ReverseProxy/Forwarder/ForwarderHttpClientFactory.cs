// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Yarp.ReverseProxy.Configuration;

namespace Yarp.ReverseProxy.Forwarder;

/// <summary>
/// Default implementation of <see cref="IForwarderHttpClientFactory"/>.
/// </summary>
public class ForwarderHttpClientFactory : IForwarderHttpClientFactory
{
    private readonly ILogger<ForwarderHttpClientFactory> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwarderHttpClientFactory"/> class.
    /// </summary>
    public ForwarderHttpClientFactory() : this(NullLogger<ForwarderHttpClientFactory>.Instance) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwarderHttpClientFactory"/> class.
    /// </summary>
    public ForwarderHttpClientFactory(ILogger<ForwarderHttpClientFactory> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
    {
        if (CanReuseOldClient(context))
        {
            Log.ClientReused(_logger, context.ClusterId);
            return context.OldClient!;
        }

        var handler = new SocketsHttpHandler
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false

            // NOTE: MaxResponseHeadersLength = 64, which means up to 64 KB of headers are allowed by default as of .NET Core 3.1.
        };

        ConfigureHandler(context, handler);

        var middleware = WrapHandler(context, handler);

        Log.ClientCreated(_logger, context.ClusterId);

        return new HttpMessageInvoker(middleware, disposeHandler: true);
    }

    /// <summary>
    /// Checks if the options have changed since the old client was created. If not then the
    /// old client will be re-used. Re-use can avoid the latency of creating new connections.
    /// </summary>
    protected virtual bool CanReuseOldClient(ForwarderHttpClientContext context)
    {
        return context.OldClient != null && context.NewConfig == context.OldConfig;
    }

    /// <summary>
    /// Allows configuring the <see cref="SocketsHttpHandler"/> instance. The base implementation
    /// applies settings from <see cref="ForwarderHttpClientContext.NewConfig"/>.
    /// <see cref="SocketsHttpHandler.UseProxy"/>, <see cref="SocketsHttpHandler.AllowAutoRedirect"/>,
    /// <see cref="SocketsHttpHandler.AutomaticDecompression"/>, and <see cref="SocketsHttpHandler.UseCookies"/>
    /// are disabled prior to this call.
    /// </summary>
    protected virtual void ConfigureHandler(ForwarderHttpClientContext context, SocketsHttpHandler handler)
    {
        var newConfig = context.NewConfig;
        if (newConfig.SslProtocols.HasValue)
        {
            handler.SslOptions.EnabledSslProtocols = newConfig.SslProtocols.Value;
        }
        if (newConfig.MaxConnectionsPerServer != null)
        {
            handler.MaxConnectionsPerServer = newConfig.MaxConnectionsPerServer.Value;
        }
        if (newConfig.DangerousAcceptAnyServerCertificate ?? false)
        {
            handler.SslOptions.RemoteCertificateValidationCallback = delegate { return true; };
        }
#if NET
        handler.EnableMultipleHttp2Connections = newConfig.EnableMultipleHttp2Connections.GetValueOrDefault(true);

        if (newConfig.RequestHeaderEncoding != null)
        {
            var encoding = Encoding.GetEncoding(newConfig.RequestHeaderEncoding);
            handler.RequestHeaderEncodingSelector = (_, _) => encoding;
        }
#endif
        var webProxy = TryCreateWebProxy(newConfig.WebProxy);
        if (webProxy != null)
        {
            handler.Proxy = webProxy;
            handler.UseProxy = true;
        }
    }

    private static IWebProxy? TryCreateWebProxy(WebProxyConfig? webProxyConfig)
    {
        if (webProxyConfig == null || webProxyConfig.Address == null)
        {
            return null;
        }

        var webProxy = new WebProxy(webProxyConfig.Address);

        webProxy.UseDefaultCredentials = webProxyConfig.UseDefaultCredentials.GetValueOrDefault(false);
        webProxy.BypassProxyOnLocal = webProxyConfig.BypassOnLocal.GetValueOrDefault(false);

        return webProxy;
    }

    /// <summary>
    /// Adds any wrapping middleware around the <see cref="HttpMessageHandler"/>.
    /// </summary>
    protected virtual HttpMessageHandler WrapHandler(ForwarderHttpClientContext context, HttpMessageHandler handler)
    {
        return handler;
    }

    private static class Log
    {
        private static readonly Action<ILogger, string, Exception?> _clientCreated = LoggerMessage.Define<string>(
              LogLevel.Debug,
              EventIds.ClientCreated,
              "New client created for cluster '{clusterId}'.");

        private static readonly Action<ILogger, string, Exception?> _clientReused = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.ClientReused,
            "Existing client reused for cluster '{clusterId}'.");

        public static void ClientCreated(ILogger logger, string clusterId)
        {
            _clientCreated(logger, clusterId, null);
        }

        public static void ClientReused(ILogger logger, string clusterId)
        {
            _clientReused(logger, clusterId, null);
        }
    }
}
