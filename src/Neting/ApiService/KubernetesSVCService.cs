using k8s;
using k8s.Models;
using Neting.ApiService.Models;
using Neting.NetingKubernetes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neting.ApiService
{
    /// <summary>
    /// Kubernetes 中的 Service、Endpoint
    /// </summary>
    public class KubernetesSVCService
    {
        private readonly Kubernetes _k8sClient;
        public KubernetesSVCService(Kubernetes k8sClient)
        {
            _k8sClient = k8sClient;
        }

        // LoadBalancer -> NodePort -> Port -> Target-Port

        /// <summary>
        /// 查找一个 Service 的 IP 和端口列表
        /// </summary>
        /// <remarks>主要用于创建 NetCluster 时，供用户选择 IP 和端口；列出三种类型的IP和端口供用户选择；</remarks>
        /// <param name="namespaceName"></param>
        /// <param name="svcName"></param>
        /// <returns></returns>
        public async Task<DataResult<SvcIpPort>> GetSvcIpsAsync(string svcName, string namespaceName)
        {
            var service = await _k8sClient.ReadNamespacedServiceAsync(svcName, namespaceName);
            if (service == null)
            {
                return new DataResult<SvcIpPort>
                {
                    Code = -1,
                    Message = "未找到 Service"
                };
            }

            SvcIpPort svc = new SvcIpPort();

            // LoadBalancer
            if (service.Spec.Type == nameof(ServiceType.LoadBalancer))
            {
                svc.LoadBalancers = new List<SvcPort>();
                var ips = svc.LoadBalancers;

                // 负载均衡器 IP
                var lbIP = service.Spec.LoadBalancerIP;
                var ports = service.Spec.Ports.Where(x => x.NodePort != null).ToArray();
                foreach (var port in ports)
                {
                    ips.Add(new SvcPort
                    {
                        Address = $"{lbIP}:{port.NodePort}/{port.Protocol}",
                        IP = lbIP,
                        Port = (int)port.NodePort!,
                        Type = nameof(ServiceType.LoadBalancer)
                    });
                }
            }

            if (service.Spec.Type == nameof(ServiceType.LoadBalancer) || service.Spec.Type == nameof(ServiceType.NodePort))
            {
                svc.NodePorts = new List<SvcPort>();
                var ips = svc.NodePorts;

                // 负载均衡器 IP，有些情况可以设置 ClusterIP 为 None；也可以手动设置为 None，只要有公网 IP 就行
                var clusterIP = service.Spec.ClusterIP;
                var ports = service.Spec.Ports.Where(x => x.NodePort != null).ToArray();
                foreach (var port in ports)
                {
                    ips.Add(new SvcPort
                    {
                        Address = $"{clusterIP}:{port.NodePort}/{port.Protocol}",
                        IP = clusterIP,
                        Port = (int)port.NodePort!,
                        Type = nameof(ServiceType.NodePort)
                    });
                }
            }

            // if (service.Spec.Type == nameof(ServiceType.ClusterIP))
            //if(service.Spec.ClusterIP == "None")            // 如果 Service 没有 Cluster IP，可能使用了无头模式，也有可能不想出现 ClusterIP
            {
                svc.Clusters = new List<SvcPort>();
                var ips = svc.Clusters;
                var clusterIP = service.Spec.ClusterIP;

                var ports = service.Spec.Ports.ToArray();
                foreach (var port in ports)
                {
                    ips.Add(new SvcPort
                    {
                        Address = $"{clusterIP}:{port.Port}/{port.Protocol}",
                        IP = clusterIP,
                        Port = port.Port,
                        Type = nameof(ServiceType.ClusterIP)
                    });
                }
            }

            if (!string.IsNullOrEmpty(service.Spec.ExternalName))
            {
                /* NAME            TYPE           CLUSTER-IP       EXTERNAL-IP          PORT(S)     AGE
                   myapp-svcname   ExternalName   <none>           myapp.baidu.com      <none>      1m
                   myapp-svcname ->  myapp-svc 
                   访问 myapp-svc.default.svc.cluster.local，变成 myapp.baidu.com
                 */
                svc.ExternalName = service.Spec.ExternalName;
            }

            return new DataResult<SvcIpPort>
            {
                Message = "获取成功",
                Data = svc,
            };
        }

        public async Task<DataResults<string>> GetServicesNameAsync(string namespaceName)
        {
            V1ServiceList services = await _k8sClient.ListNamespacedServiceAsync(namespaceName);
            return new DataResults<string>
            {
                Message = "获取成功",
                Data = services.Items.Select(x => x.Metadata.Name).ToArray(),
            };
        }

        /// <summary>
        /// 获取 Service 列表
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        public async Task<DataResult<SvcInfoList>> GetServicesAsync(string namespaceName, int pageSize = 1, string? continueProperty = null)
        {
            V1ServiceList services;
            if (string.IsNullOrEmpty(continueProperty))
            {
                services = await _k8sClient.ListNamespacedServiceAsync(namespaceName, limit: pageSize);
            }
            else
            {
                try
                {
                    services = await _k8sClient.ListNamespacedServiceAsync(namespaceName, continueParameter: continueProperty, limit: pageSize);
                }
                catch (Microsoft.Rest.HttpOperationException ex)
                {
                    return new DataResult<SvcInfoList>
                    {
                        Code = -1,
                        Message = "查询过期，请重新查询"
                    };
                }
                catch
                {
                    throw;
                }
            }

            SvcInfoList svcList = new SvcInfoList
            {
                ContinueProperty = services.Metadata.ContinueProperty,
                RemainingItemCount = (int)services.Metadata.RemainingItemCount.GetValueOrDefault(),
                Items = new List<SvcInfo>()
            };

            List<SvcInfo> svcInfos = svcList.Items;
            foreach (var item in services.Items)
            {
                SvcInfo service = new SvcInfo
                {
                    Name = item.Metadata.Name,
                    ServiceType = item.Spec.Type,
                    ClusterIP = item.Spec.ClusterIP,
                    Labels = item.Metadata.Labels,
                    Selector = item.Spec.Selector,
                    CreationTime = item.Metadata.CreationTimestamp
                };
                // 处理端口
                if (item.Spec.Type == nameof(ServiceType.LoadBalancer) || item.Spec.Type == nameof(ServiceType.NodePort))
                {
                    service.Ports = new List<string>();
                    foreach (var port in item.Spec.Ports)
                    {
                        service.Ports.Add($"{port.Port}:{port.NodePort}/{port.Protocol}");
                    }
                }
                else if (item.Spec.Type == nameof(ServiceType.ClusterIP))
                {
                    service.Ports = new List<string>();
                    foreach (var port in item.Spec.Ports)
                    {
                        service.Ports.Add($"{port.Port}/{port.Protocol}");
                    }
                }

                var endpoint = await _k8sClient.ReadNamespacedEndpointsAsync(item.Metadata.Name, namespaceName);
                if (endpoint != null && endpoint.Subsets.Count != 0)
                {
                    List<string> address = new List<string>();
                    foreach (var sub in endpoint.Subsets)
                    {
                        if (sub.Addresses == null) continue;
                        foreach (var addr in sub.Addresses)
                        {
                            foreach (var port in sub.Ports)
                            {
                                address.Add($"{addr.Ip}:{port.Port}/{port.Protocol}");
                            }
                        }

                    }
                    service.Endpoints = address.ToArray();
                }



                svcInfos.Add(service);
            }
            return new DataResult<SvcInfoList>
            {
                Message = "查询成功",
                Data = svcList,
            };
        }

        /// <summary>
        /// 获取一个 Service 的详细信息
        /// </summary>
        /// <param name="svcName"></param>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        public async Task<DataResult<ServiceInfo>> GetServiceAsync(string svcName, string namespaceName)
        {
            var service = await _k8sClient.ReadNamespacedServiceAsync(svcName, namespaceName);
            if (service == null) return new DataResult<ServiceInfo> { };

            ServiceInfo info = new ServiceInfo
            {
                Name = service.Metadata.Name,
                Namespace = service.Metadata.NamespaceProperty,
                ServiceType = service.Spec.Type,
                Labels = service.Metadata.Labels,
                ClusterIP = service.Spec.ClusterIP,
                CreationTime = service.Metadata.CreationTimestamp,
                Selector = service.Spec.Selector.ToDictionary(x => x.Key, x => x.Value),
                ExternalAddress = service.Spec.ExternalIPs?.ToArray(),
            };
            var endpoint = await _k8sClient.ReadNamespacedEndpointsAsync(svcName, namespaceName);
            List<string> address = new List<string>();
            foreach (var sub in endpoint.Subsets)
            {
                foreach (var addr in sub.Addresses)
                {
                    foreach (var port in sub.Ports)
                    {
                        address.Add($"{addr.Ip}:{port.Port}/{port.Protocol}");
                    }
                }

            }
            info.Endpoints = address.ToArray();
            return new DataResult<ServiceInfo>
            {
                Code = 0,
                Message = "查询成功",
                Data = info
            };
        }
    }
}
