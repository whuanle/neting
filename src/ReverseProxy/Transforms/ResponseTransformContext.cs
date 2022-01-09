// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Yarp.ReverseProxy.Transforms;

/// <summary>
/// Transform state for use with <see cref="ResponseTransform"/>
/// </summary>
public class ResponseTransformContext
{
    /// <summary>
    /// The current request context.
    /// </summary>
    public HttpContext HttpContext { get; init; } = default!;

    /// <summary>
    /// The proxy response. This can be null if the destination did not respond.
    /// </summary>
    public HttpResponseMessage? ProxyResponse { get; init; }

    /// <summary>
    /// Gets or sets if the response headers have been copied from the HttpResponseMessage and HttpContent
    /// to the HttpResponse. Transforms use this when searching for the current value of a header they
    /// should operate on.
    /// </summary>
    public bool HeadersCopied { get; set; }

    /// <summary>
    /// Set to true if the proxy should exclude the body and trailing headers when proxying this response.
    /// Defaults to false.
    /// </summary>
    public bool SuppressResponseBody { get; set; }
}
