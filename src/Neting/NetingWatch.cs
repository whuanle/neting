using dotnet_etcd;
using Microsoft.Extensions.DependencyInjection;
using Neting.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Configuration;

namespace Neting.Yarp
{

    /// <summary>
    /// 监控 Etcd 数据库变化，并刷新内存中的信息
    /// </summary>
    public static class NetingWatch
    {

        private static readonly Provider provider;

        static NetingWatch()
        {
            provider = new Provider();
        }

        // 监听事件 cluster
        // 监听事件 cluster

        public static void InitWatch(EtcdClient etcdClient)
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        etcdClient.WatchRange(Constants.NetingClusterPath, async response =>
                        {
                            await NetingWatch.WatchCluster(response);
                        });
                    }
                    catch { }
                }
            })
            {
                IsBackground = true
            }.Start();


            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        etcdClient.WatchRange(Constants.NetingRoutePath, async response =>
                        {
                            await NetingWatch.WatchRouter(response);
                        });
                    }
                    catch { }
                }
            })
            {
                IsBackground = true
            }.Start();

        }

        public static async Task WatchCluster(WatchEvent[] response) => await RefreshCluster();
        public static async Task RefreshCluster()
        {
            // 不管它是增加还是修改或者变化，都要一次性取出 Cluster 中的数据，刷新到内存中

            var database = Background.Service.GetRequiredService<NetingDateBase>();
            var netingClusters = await database.NetingCluster.GetsAsync();

            List<ClusterConfig> configs = new List<ClusterConfig>();


            foreach (var netingCluster in netingClusters.Values)
            {
                // 生成后端服务定义
                var destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase);
                if (netingCluster.Services != null)
                {
                    // 其中一个后端
                    foreach (var item in netingCluster.Services)
                    {
                        DestinationConfig destinationConfig = new DestinationConfig
                        {
                            Health = item.Health,
                            Metadata = item.Metadata,
                            Address = $"{item.Protocol}{item.HostName}:{item.Port}/{item.Path}"
                        };
                        destinations.Add($"{item.Name}", destinationConfig);
                    }

                    ClusterConfig config = new ClusterConfig
                    {
                        ClusterId = netingCluster.Name,
                        Destinations = destinations
                    };
                    configs.Add(config);
                }
            }
            // 刷新到内存中
            provider.Refresh(configs);
        }
        public static async Task WatchRouter(WatchEvent[] response) => await RefreshRouter();
        public static async Task RefreshRouter()
        {
            // 不管它是增加还是修改或者变化，都要一次性取出 Cluster 中的数据，刷新到内存中

            var database = Background.Service.GetRequiredService<NetingDateBase>();
            var netingRoutes = await database.NetingRoute.GetsAsync();

            List<RouteConfig> configs = new List<RouteConfig>();

            foreach (var netingRoute in netingRoutes.Values)
            {
                RouteConfig config = new RouteConfig
                {
                    RouteId = netingRoute.Name,
                    ClusterId = netingRoute.ClusterName,
                    Metadata = netingRoute.Metadata,
                    CorsPolicy = netingRoute.CorsPolicy,
                    AuthorizationPolicy = netingRoute.AuthorizationPolicy,
                    Match = new RouteMatch
                    {
                        Hosts = netingRoute.Match.Hosts?.ToList() ?? null,
                        Path = netingRoute.Match.Path,
                    }
                };
                configs.Add(config);
            }
            // 刷新到内存中
            provider.Refresh(configs);
        }

        private class Provider : NetingProxyConfigProvider
        {
        }

    }
}
