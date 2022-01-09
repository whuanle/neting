// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using System;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Yarp.ReverseProxy.Transforms;

/// <summary>
/// Extension methods for <see cref="TransformBuilderContext"/>.
/// </summary>
public static class TransformBuilderContextFuncExtensions
{
    /// <summary>
    /// Adds a transform Func that runs on each request for the given route.
    /// </summary>
    public static TransformBuilderContext AddRequestTransform(this TransformBuilderContext context, Func<RequestTransformContext, ValueTask> func)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        context.RequestTransforms.Add(new RequestFuncTransform(func));
        return context;
    }

    /// <summary>
    /// Adds a transform Func that runs on each response for the given route.
    /// </summary>
    public static TransformBuilderContext AddResponseTransform(this TransformBuilderContext context, Func<ResponseTransformContext, ValueTask> func)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        context.ResponseTransforms.Add(new ResponseFuncTransform(func));
        return context;
    }

    /// <summary>
    /// Adds a transform Func that runs on each response for the given route.
    /// </summary>
    public static TransformBuilderContext AddResponseTrailersTransform(this TransformBuilderContext context, Func<ResponseTrailersTransformContext, ValueTask> func)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        context.ResponseTrailersTransforms.Add(new ResponseTrailersFuncTransform(func));
        return context;
    }
}
