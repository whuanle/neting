using System.Collections.Generic;

namespace Neting.ApiService.Models
{
    /// <summary>
    /// Kubernetes Service 和 IP
    /// </summary>
    public class SvcPort
    {

        // LoadBalancer -> NodePort -> Port -> Target-Port

        /// <summary>
        /// 127.0.0.1:8080/tcp、127.0.0.1:8080/http
        /// </summary>
        public string Address { get; set; } = null!;

        /// <summary>
        /// LoadBalancer、NodePort、Cluster
        /// </summary>
        public string Type { get; set; } = null!;

        public string IP { get; set; } = null!;
        public int Port { get; set; }
    }
    public class SvcIpPort
    {
        public List<SvcPort>? LoadBalancers { get; set; }
        public List<SvcPort>? NodePorts { get; set; }
        public List<SvcPort>? Clusters { get; set; }
        public string? ExternalName { get; set; }
    }
}
