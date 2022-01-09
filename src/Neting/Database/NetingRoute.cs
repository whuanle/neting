using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Neting.Database
{
    public class NetingRoute
    {
        /// <summary>
        /// RouteId
        /// </summary>
        [Required(ErrorMessage = "Route 名称不能为空")]
        public virtual string Name { get; set; } = default!;

        /// <summary>
        /// 描述
        /// </summary>
        public virtual string? Description { get; set; }

        public IReadOnlyDictionary<string, string>? Metadata { get; init; }

        /// <summary>
        /// 路由规则
        /// </summary>
        public NetingRouteMatch Match { get; init; } = default!;

        /// <summary>
        /// 绑定的 Cluster
        /// </summary>
        [Required(ErrorMessage = "Cluster 对象不能为空")]
        public virtual string ClusterName { get; set; } = default!;


        public string? AuthorizationPolicy { get; set; }
        public string? CorsPolicy { get; set; }



        public class NetingRouteMatch
        {
            /// <summary>
            /// 只运行这些来源访问
            /// </summary>
            public HashSet<string>? Hosts { get; set; }

            /// <summary>
            ///区配访问路径
            /// </summary>
            public string? Path { get; set; }
        }
    }
}
