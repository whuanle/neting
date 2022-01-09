// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Model;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Utilities;

namespace Yarp.ReverseProxy.Health;

/// <summary>
/// Calculates the proxied requests failure rate for each destination and marks it as unhealthy if the specified limit is exceeded.
/// </summary>
/// <remarks>
/// Rate is calculated as a percentage of failured requests to the total number of request proxied to a destination in the given period of time. Failed and total counters are tracked
/// in a sliding time window which means that only the recent readings fitting in the window are taken into account. The window is implemented as a linked-list of timestamped records
/// where each record contains the difference from the previous one in the number of failed and total requests. Additionally, there are 2 destination-wide counters storing aggregated values
/// to enable a fast calculation of the current failure rate. When a new proxied request is reported, its status firstly affects those 2 aggregated counters and then also gets put
/// in the record history. Once some record moves out of the detection time window, the failed and total counter deltas stored on it get subtracted from the respective aggregated counters.
/// </remarks>
internal sealed class TransportFailureRateHealthPolicy : IPassiveHealthCheckPolicy
{
    private static readonly TimeSpan _defaultReactivationPeriod = TimeSpan.FromSeconds(60);
    private readonly IDestinationHealthUpdater _healthUpdater;
    private readonly TransportFailureRateHealthPolicyOptions _policyOptions;
    private readonly IClock _clock;
    private readonly ConditionalWeakTable<ClusterState, ParsedMetadataEntry<double>> _clusterFailureRateLimits = new ConditionalWeakTable<ClusterState, ParsedMetadataEntry<double>>();
    private readonly ConditionalWeakTable<DestinationState, ProxiedRequestHistory> _requestHistories = new ConditionalWeakTable<DestinationState, ProxiedRequestHistory>();

    public string Name => HealthCheckConstants.PassivePolicy.TransportFailureRate;

    public TransportFailureRateHealthPolicy(
        IOptions<TransportFailureRateHealthPolicyOptions> policyOptions,
        IClock clock,
        IDestinationHealthUpdater healthUpdater)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _policyOptions = policyOptions?.Value ?? throw new ArgumentNullException(nameof(policyOptions));
        _healthUpdater = healthUpdater ?? throw new ArgumentNullException(nameof(healthUpdater));
    }

    public void RequestProxied(HttpContext context, ClusterState cluster, DestinationState destination)
    {
        var error = context.Features.Get<IForwarderErrorFeature>();
        var newHealth = EvaluateProxiedRequest(cluster, destination, error != null);
        var clusterReactivationPeriod = cluster.Model.Config.HealthCheck?.Passive?.ReactivationPeriod ?? _defaultReactivationPeriod;
        // Avoid reactivating until the history has expired so that it does not affect future health assessments.
        var reactivationPeriod = clusterReactivationPeriod >= _policyOptions.DetectionWindowSize ? clusterReactivationPeriod : _policyOptions.DetectionWindowSize;
        _healthUpdater.SetPassive(cluster, destination, newHealth, reactivationPeriod);
    }

    private DestinationHealth EvaluateProxiedRequest(ClusterState cluster, DestinationState destination, bool failed)
    {
        var history = _requestHistories.GetOrCreateValue(destination);
        var rateLimitEntry = _clusterFailureRateLimits.GetValue(cluster, c => new ParsedMetadataEntry<double>(TryParse, c, TransportFailureRateHealthPolicyOptions.FailureRateLimitMetadataName));
        var rateLimit = rateLimitEntry.GetParsedOrDefault(_policyOptions.DefaultFailureRateLimit);
        lock (history)
        {
            var failureRate = history.AddNew(
                _clock.TickCount,
                (long)_policyOptions.DetectionWindowSize.TotalMilliseconds,
                _policyOptions.MinimalTotalCountThreshold,
                failed);
            return failureRate < rateLimit ? DestinationHealth.Healthy : DestinationHealth.Unhealthy;
        }
    }

    private static bool TryParse(string stringValue, out double parsedValue)
    {
        return double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedValue);
    }

    private class ProxiedRequestHistory
    {
        private const long RecordWindowSize = 1000;
        private long _nextRecordCreatedAt;
        private long _nextRecordTotalCount;
        private long _nextRecordFailedCount;
        private long _failedCount;
        private double _totalCount;
        private readonly Queue<HistoryRecord> _records = new Queue<HistoryRecord>();

        public double AddNew(long eventTime, long detectionWindowSize, int totalCountThreshold, bool failed)
        {
            if (_nextRecordCreatedAt == 0)
            {
                // Initialization.
                _nextRecordCreatedAt = eventTime + RecordWindowSize;
            }

            // Don't create a new record on each event because it can negatively affect performance.
            // Instead, accumulate failed and total request counts reported during some period
            // and then add only one record storing them.
            if (eventTime >= _nextRecordCreatedAt)
            {
                _records.Enqueue(new HistoryRecord(_nextRecordCreatedAt, _nextRecordTotalCount, _nextRecordFailedCount));
                _nextRecordCreatedAt = eventTime + RecordWindowSize;
                _nextRecordTotalCount = 0;
                _nextRecordFailedCount = 0;
            }

            _nextRecordTotalCount++;
            _totalCount++;
            if (failed)
            {
                _failedCount++;
                _nextRecordFailedCount++;
            }

            while (_records.Count > 0 && (eventTime - _records.Peek().RecordedAt > detectionWindowSize))
            {
                var removed = _records.Dequeue();
                _failedCount -= removed.FailedCount;
                _totalCount -= removed.TotalCount;
            }

            return _totalCount < totalCountThreshold || _totalCount == 0 ? 0.0 : _failedCount / _totalCount;
        }

        private readonly struct HistoryRecord
        {
            public HistoryRecord(long recordedAt, long totalCount, long failedCount)
            {
                RecordedAt = recordedAt;
                TotalCount = totalCount;
                FailedCount = failedCount;
            }

            public long RecordedAt { get; }

            public long TotalCount { get; }

            public long FailedCount { get; }
        }
    }
}
