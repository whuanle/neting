// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Model;

namespace Yarp.ReverseProxy.Health;

/// <summary>
/// Passive health check evaluation policy.
/// </summary>
public interface IPassiveHealthCheckPolicy
{
    /// <summary>
    /// Policy's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Registers a successful or failed request and evaluates a new <see cref="DestinationHealthState.Passive"/> value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="cluster">Request's cluster.</param>
    /// <param name="destination">Request's destination.</param>
    void RequestProxied(HttpContext context, ClusterState cluster, DestinationState destination);
}
