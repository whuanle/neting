using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Yarp.ReverseProxy.Configuration;

namespace Neting.Yarp
{
    /// <summary>
    /// Yarp 配置器存储结构，这个结构将会长久驻扎在内存中
    /// </summary>
    public class NetingProxyConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public NetingProxyConfig()
        {
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        /// <summary>
        /// 路由规则
        /// </summary>
        public IReadOnlyList<RouteConfig> Routes => _routes;

        /// <summary>
        /// 路由映射
        /// </summary>
        public IReadOnlyList<ClusterConfig> Clusters => _cluster;


        private readonly List<RouteConfig> _routes = new List<RouteConfig>();

        private readonly List<ClusterConfig> _cluster = new List<ClusterConfig>();


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Refresh(IEnumerable<RouteConfig> configs)
        {
            _routes.Clear();
            if (configs != null && configs.Any())
            {
                _routes.AddRange(configs);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Refresh(IEnumerable<ClusterConfig> configs)
        {
            _cluster.Clear();
            if (configs != null && configs.Any())
            {
                _cluster.AddRange(configs);
            }
        }


        public IChangeToken ChangeToken { get; }

        internal void SignalChange()
        {
            _cts.Cancel();
        }
    }

}
