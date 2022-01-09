// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Yarp.ReverseProxy.Transforms;

/// <summary>
/// Sets or appends the X-Forwarded-For header with the previous clients's IP address.
/// </summary>
public class RequestHeaderXForwardedForTransform : RequestTransform
{
    /// <summary>
    /// Creates a new transform.
    /// </summary>
    /// <param name="headerName">The header name.</param>
    /// <param name="action">Action to applied to the header.</param>
    public RequestHeaderXForwardedForTransform(string headerName, ForwardedTransformActions action)
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

        var remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        switch (TransformAction)
        {
            case ForwardedTransformActions.Set:
                RemoveHeader(context, HeaderName);
                if (remoteIp != null)
                {
                    AddHeader(context, HeaderName, remoteIp);
                }
                break;
            case ForwardedTransformActions.Append:
                Append(context, remoteIp);
                break;
            case ForwardedTransformActions.Remove:
                RemoveHeader(context, HeaderName);
                break;
            default:
                throw new NotImplementedException(TransformAction.ToString());
        }

        return default;
    }

    private void Append(RequestTransformContext context, string? remoteIp)
    {
        var existingValues = TakeHeader(context, HeaderName);
        if (remoteIp == null)
        {
            if (!string.IsNullOrEmpty(existingValues))
            {
                AddHeader(context, HeaderName, existingValues);
            }
        }
        else
        {
            var values = StringValues.Concat(existingValues, remoteIp);
            AddHeader(context, HeaderName, values);
        }
    }
}
