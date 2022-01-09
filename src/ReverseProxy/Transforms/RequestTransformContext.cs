// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Yarp.ReverseProxy.Transforms;

/// <summary>
/// Transform state for use with <see cref="RequestTransform"/>
/// </summary>
public class RequestTransformContext
{
    /// <summary>
    /// The current request context.
    /// </summary>
    public HttpContext HttpContext { get; init; } = default!;

    /// <summary>
    /// The outgoing proxy request. All field are initialized except for the 'RequestUri' and optionally headers.
    /// If no value is provided then the 'RequestUri' will be initialized using the updated 'DestinationPrefix',
    /// 'Path', and 'Query' properties after the transforms have run. The headers will be copied later when
    /// applying header transforms.
    /// </summary>
    public HttpRequestMessage ProxyRequest { get; init; } = default!;

    /// <summary>
    /// Gets or sets if the request headers have been copied from the HttpRequest to the HttpRequestMessage and HttpContent.
    /// Transforms use this when searching for the current value of a header they should operate on.
    /// </summary>
    public bool HeadersCopied { get; set; }

    /// <summary>
    /// The path to use for the proxy request.
    /// </summary>
    /// <remarks>
    /// This will be prefixed by any PathBase specified for the destination server.
    /// </remarks>
    public PathString Path { get; set; }

    /// <summary>
    /// The query used for the proxy request.
    /// </summary>
    public QueryTransformContext Query { get; set; } = default!;

    /// <summary>
    /// The URI prefix for the proxy request. This includes the scheme and host and can optionally include a
    /// port and path base. The 'Path' and 'Query' properties will be appended to this after the transforms have run.
    /// </summary>
    public string DestinationPrefix { get; init; } = default!;
}
