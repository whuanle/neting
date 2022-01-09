using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Neting.Modes.Backend
{
    public class NetingServiceEqualityComparer : IEqualityComparer<NetingBackendService>
    {
        public bool Equals(NetingBackendService? x, NetingBackendService? y)
        {
            return x?.Name == y?.Name && x?.TargetPort == y?.TargetPort;
        }

        public int GetHashCode([DisallowNull] NetingBackendService obj)
        {
            return HashCode.Combine(obj.Name, obj.TargetPort);
        }
    }
}
