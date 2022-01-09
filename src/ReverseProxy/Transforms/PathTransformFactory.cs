// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Template;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Yarp.ReverseProxy.Transforms;

internal sealed class PathTransformFactory : ITransformFactory
{
    internal static readonly string PathSetKey = "PathSet";
    internal static readonly string PathPrefixKey = "PathPrefix";
    internal static readonly string PathRemovePrefixKey = "PathRemovePrefix";
    internal static readonly string PathPatternKey = "PathPattern";

    private readonly TemplateBinderFactory _binderFactory;

    public PathTransformFactory(TemplateBinderFactory binderFactory)
    {
        _binderFactory = binderFactory ?? throw new ArgumentNullException(nameof(binderFactory));
    }

    public bool Validate(TransformRouteValidationContext context, IReadOnlyDictionary<string, string> transformValues)
    {
        if (transformValues.TryGetValue(PathSetKey, out var pathSet))
        {
            TransformHelpers.TryCheckTooManyParameters(context, transformValues, expected: 1);
            CheckPathNotNull(context, PathSetKey, pathSet);
        }
        else if (transformValues.TryGetValue(PathPrefixKey, out var pathPrefix))
        {
            TransformHelpers.TryCheckTooManyParameters(context, transformValues, expected: 1);
            CheckPathNotNull(context, PathPrefixKey, pathPrefix);
        }
        else if (transformValues.TryGetValue(PathRemovePrefixKey, out var pathRemovePrefix))
        {
            TransformHelpers.TryCheckTooManyParameters(context, transformValues, expected: 1);
            CheckPathNotNull(context, PathRemovePrefixKey, pathRemovePrefix);
        }
        else if (transformValues.TryGetValue(PathPatternKey, out var pathPattern))
        {
            TransformHelpers.TryCheckTooManyParameters(context, transformValues, expected: 1);
            CheckPathNotNull(context, PathPatternKey, pathPattern);
            // TODO: Validate the pattern format. Does it build?
        }
        else
        {
            return false;
        }

        return true;
    }

    private void CheckPathNotNull(TransformRouteValidationContext context, string fieldName, string? path)
    {
        if (path == null)
        {
            context.Errors.Add(new ArgumentNullException(fieldName));
        }
    }

    public bool Build(TransformBuilderContext context, IReadOnlyDictionary<string, string> transformValues)
    {
        if (transformValues.TryGetValue(PathSetKey, out var pathSet))
        {
            TransformHelpers.CheckTooManyParameters(transformValues, expected: 1);
            var path = MakePathString(pathSet);
            context.AddPathSet(path);
        }
        else if (transformValues.TryGetValue(PathPrefixKey, out var pathPrefix))
        {
            TransformHelpers.CheckTooManyParameters(transformValues, expected: 1);
            var path = MakePathString(pathPrefix);
            context.AddPathPrefix(path);
        }
        else if (transformValues.TryGetValue(PathRemovePrefixKey, out var pathRemovePrefix))
        {
            TransformHelpers.CheckTooManyParameters(transformValues, expected: 1);
            var path = MakePathString(pathRemovePrefix);
            context.AddPathRemovePrefix(path);
        }
        else if (transformValues.TryGetValue(PathPatternKey, out var pathPattern))
        {
            TransformHelpers.CheckTooManyParameters(transformValues, expected: 1);
            var path = MakePathString(pathPattern);
            // We don't use the extension here because we want to avoid doing a DI lookup for the binder every time.
            context.RequestTransforms.Add(new PathRouteValuesTransform(path.Value!, _binderFactory));
        }
        else
        {
            return false;
        }

        return true;
    }

    private static PathString MakePathString(string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }
        if (!path.StartsWith("/", StringComparison.Ordinal))
        {
            path = "/" + path;
        }
        return new PathString(path);
    }
}
