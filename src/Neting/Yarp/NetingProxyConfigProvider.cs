using System.Collections.Generic;
using Yarp.ReverseProxy.Configuration;

namespace Neting.Yarp
{
    /// <summary>
    /// Neting 路由映射提供器
    /// </summary>
    public class NetingProxyConfigProvider : IProxyConfigProvider
    {
        private volatile static NetingProxyConfig _config;

        static NetingProxyConfigProvider()
        {
            // 启动后应当马上从 etcd 中拉取数据
            _config = new NetingProxyConfig();
        }

        public IProxyConfig GetConfig()
        {
            return _config;
        }

        public void Refresh(IEnumerable<RouteConfig> routeConfigs, IEnumerable<ClusterConfig> clusterConfigs)
        {
            _config.Refresh(routeConfigs);
            _config.Refresh(clusterConfigs);
        }

        public void Refresh(IEnumerable<RouteConfig> routeConfigs)
        {
            _config.Refresh(routeConfigs);
        }

        public void Refresh(IEnumerable<ClusterConfig> clusterConfigs)
        {
            _config.Refresh(clusterConfigs);
        }
    }
}
