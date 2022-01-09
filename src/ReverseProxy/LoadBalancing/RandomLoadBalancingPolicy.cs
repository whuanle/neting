// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Model;
using Yarp.ReverseProxy.Utilities;

namespace Yarp.ReverseProxy.LoadBalancing;

internal sealed class RandomLoadBalancingPolicy : ILoadBalancingPolicy
{
    private readonly IRandomFactory _randomFactory;

    public RandomLoadBalancingPolicy(IRandomFactory randomFactory)
    {
        _randomFactory = randomFactory;
    }

    public string Name => LoadBalancingPolicies.Random;

    public DestinationState? PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations)
    {
        if (availableDestinations.Count == 0)
        {
            return null;
        }

        var random = _randomFactory.CreateRandomInstance();
        return availableDestinations[random.Next(availableDestinations.Count)];
    }
}
