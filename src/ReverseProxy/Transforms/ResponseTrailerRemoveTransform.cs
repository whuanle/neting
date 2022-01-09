// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace Yarp.ReverseProxy.Transforms;

/// <summary>
/// Removes a response trailer.
/// </summary>
public class ResponseTrailerRemoveTransform : ResponseTrailersTransform
{
    public ResponseTrailerRemoveTransform(string headerName, ResponseCondition condition)
    {
        if (string.IsNullOrEmpty(headerName))
        {
            throw new ArgumentException($"'{nameof(headerName)}' cannot be null or empty.", nameof(headerName));
        }

        HeaderName = headerName;
        Condition = condition;
    }

    internal string HeaderName { get; }

    internal ResponseCondition Condition { get; }

    // Assumes the response status code has been set on the HttpContext already.
    /// <inheritdoc/>
    public override ValueTask ApplyAsync(ResponseTrailersTransformContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        Debug.Assert(context.ProxyResponse != null);

        if (Condition == ResponseCondition.Always
            || Success(context) == (Condition == ResponseCondition.Success))
        {
            var responseTrailersFeature = context.HttpContext.Features.Get<IHttpResponseTrailersFeature>();
            var responseTrailers = responseTrailersFeature?.Trailers;
            // Support should have already been checked by the caller.
            Debug.Assert(responseTrailers != null);
            Debug.Assert(!responseTrailers.IsReadOnly);

            responseTrailers.Remove(HeaderName);
        }

        return default;
    }
}
