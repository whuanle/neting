// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Yarp.ReverseProxy.Health;

/// <summary>
/// Defines options for the active health check monitor.
/// </summary>
public class ActiveHealthCheckMonitorOptions
{
    /// <summary>
    /// Default probing interval.
    /// </summary>
    public TimeSpan DefaultInterval { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Default probes timeout.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(10);
}
