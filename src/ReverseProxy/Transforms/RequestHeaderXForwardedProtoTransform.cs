// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Yarp.ReverseProxy.Transforms;

/// <summary>
/// Sets or appends the X-Forwarded-Proto header with the request's original url scheme.
/// </summary>
public class RequestHeaderXForwardedProtoTransform : RequestTransform
{
    /// <summary>
    /// Creates a new transform.
    /// </summary>
    /// <param name="headerName">The header name.</param>
    /// <param name="action">Action to applied to the header.</param>
    public RequestHeaderXForwardedProtoTransform(string headerName, ForwardedTransformActions action)
    {
        if (string.IsNullOrEmpty(headerName))
        {
            throw new ArgumentException($"'{nameof(headerName)}' cannot be null or empty.", nameof(headerName));
        }

        HeaderName = headerName;
        Debug.Assert(action != ForwardedTransformActions.Off);
        TransformAction = action;
    }

    internal string HeaderName { get; }
    internal ForwardedTransformActions TransformAction { get; }

    /// <inheritdoc/>
    public override ValueTask ApplyAsync(RequestTransformContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var scheme = context.HttpContext.Request.Scheme;

        switch (TransformAction)
        {
            case ForwardedTransformActions.Set:
                RemoveHeader(context, HeaderName);
                AddHeader(context, HeaderName, scheme);
                break;
            case ForwardedTransformActions.Append:
                var existingValues = TakeHeader(context, HeaderName);
                var values = StringValues.Concat(existingValues, scheme);
                AddHeader(context, HeaderName, values);
                break;
            case ForwardedTransformActions.Remove:
                RemoveHeader(context, HeaderName);
                break;
            default:
                throw new NotImplementedException(TransformAction.ToString());
        }

        return default;
    }
}
