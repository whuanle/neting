// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using Yarp.ReverseProxy.Configuration;

namespace Yarp.ReverseProxy.Model;

/// <summary>
/// Immutable representation of the portions of a cluster
/// that only change in reaction to configuration changes
/// (e.g. http client options).
/// </summary>
/// <remarks>
/// All members must remain immutable to avoid thread safety issues.
/// Instead, instances of <see cref="ClusterModel"/> are replaced
/// in their entirety when values need to change.
/// </remarks>
public sealed class ClusterModel
{
    /// <summary>
    /// Creates a new Instance.
    /// </summary>
    public ClusterModel(
        ClusterConfig config,
        HttpMessageInvoker httpClient)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// The config for this cluster.
    /// </summary>
    public ClusterConfig Config { get; }

    /// <summary>
    /// An <see cref="HttpMessageInvoker"/> that used for proxying requests to an upstream server.
    /// </summary>
    public HttpMessageInvoker HttpClient { get; }

    // We intentionally do not consider destination changes when updating the cluster Revision.
    // Revision is used to rebuild routing endpoints which should be unrelated to destinations,
    // and destinations are the most likely to change.
    internal bool HasConfigChanged(ClusterModel newModel)
    {
        return !Config.EqualsExcludingDestinations(newModel.Config) || newModel.HttpClient != HttpClient;
    }
}
