// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Yarp.ReverseProxy.Configuration;

/// <summary>
/// Active health check config.
/// </summary>
public sealed record ActiveHealthCheckConfig
{
    /// <summary>
    /// Whether active health checks are enabled.
    /// </summary>
    public bool? Enabled { get; init; }

    /// <summary>
    /// Health probe interval.
    /// </summary>
    public TimeSpan? Interval { get; init; }

    /// <summary>
    /// Health probe timeout, after which a destination is considered unhealthy.
    /// </summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>
    /// Active health check policy.
    /// </summary>
    public string? Policy { get; init; }

    /// <summary>
    /// HTTP health check endpoint path.
    /// </summary>
    public string? Path { get; init; }

    public bool Equals(ActiveHealthCheckConfig? other)
    {
        if (other == null)
        {
            return false;
        }

        return Enabled == other.Enabled
            && Interval == other.Interval
            && Timeout == other.Timeout
            && string.Equals(Policy, other.Policy, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Path, other.Path, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Enabled,
            Interval,
            Timeout,
            Policy?.GetHashCode(StringComparison.OrdinalIgnoreCase),
            Path?.GetHashCode(StringComparison.Ordinal));
    }
}
