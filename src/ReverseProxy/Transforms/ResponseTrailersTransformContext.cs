// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Yarp.ReverseProxy.Transforms;

/// <summary>
/// Transform state for use with <see cref="ResponseTrailersTransform"/>
/// </summary>
public class ResponseTrailersTransformContext
{
    /// <summary>
    /// The current request context.
    /// </summary>
    public HttpContext HttpContext { get; init; } = default!;

    /// <summary>
    /// The incoming proxy response.
    /// </summary>
    public HttpResponseMessage ProxyResponse { get; init; } = default!;

    /// <summary>
    /// Gets or sets if the response trailers have been copied from the HttpResponseMessage
    /// to the HttpResponse. Transforms use this when searching for the current value of a header they
    /// should operate on.
    /// </summary>
    public bool HeadersCopied { get; set; }
}
