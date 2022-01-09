using Yarp.ReverseProxy.Configuration;

namespace Neting.Yarp
{
    /// <summary>
    /// 一个完整的路由映射
    /// </summary>
    public class RouteMapConfig
    {
        // https://kubernetes.io/zh/docs/concepts/services-networking/ingress/

        /// <summary>
        /// 路径
        /// </summary>
        public RouteConfig Path { get; set; } = null!;

        /// <summary>
        /// 后端服务
        /// </summary>
        public ClusterConfig BackendList { get; set; } = null!;
    }
}
