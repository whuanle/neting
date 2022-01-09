// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Yarp.ReverseProxy.Model;

namespace Yarp.ReverseProxy.SessionAffinity;

/// <summary>
/// Affinity resolution result.
/// </summary>
public readonly struct AffinityResult
{
    public IReadOnlyList<DestinationState>? Destinations { get; }

    public AffinityStatus Status { get; }

    public AffinityResult(IReadOnlyList<DestinationState>? destinations, AffinityStatus status)
    {
        Destinations = destinations;
        Status = status;
    }
}
