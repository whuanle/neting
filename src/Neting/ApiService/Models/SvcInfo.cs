using System;
using System.Collections.Generic;

namespace Neting.ApiService.Models
{

    public class SvcInfoList
    {
        /// <summary>
        /// 分页属性，具有临时有效期，具体由 Kubernetes 确定
        /// </summary>
        public string? ContinueProperty { get; set; }

        /// <summary>
        /// 预计剩余数量
        /// </summary>
        public int RemainingItemCount { get; set; }

        /// <summary>
        /// SVC 列表
        /// </summary>
        public List<SvcInfo> Items { get; set; } = new List<SvcInfo>();
    }

    public class SvcInfo
    {
        /// <summary>
        /// SVC 名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 三种类型之一 <see cref="ServiceType"/>
        /// </summary>
        public string? ServiceType { get; set; }

        /// <summary>
        /// 有些 Service 没有 IP，值为 None
        /// </summary>
        public string ClusterIP { get; set; } = null!;

        public DateTime? CreationTime { get; set; }

        public IDictionary<string, string>? Labels { get; set; }

        public IDictionary<string, string>? Selector { get; set; }

        /// <summary>
        /// name,port
        /// </summary>
        public List<string> Ports { get; set; }

        public string[]? Endpoints { get; set; }
    }


    public class ServiceInfo
    {
        /// <summary>
        /// SVC 名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 三种类型之一 <see cref="ServiceType"/>
        /// </summary>
        public string? ServiceType { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace { get; set; } = null!;

        /// <summary>
        /// 有些 Service 没有此选项
        /// </summary>
        public string ClusterIP { get; set; } = null!;

        /// <summary>
        /// 外网访问 IP
        /// </summary>
        public string[]? ExternalAddress { get; set; }

        public IDictionary<string, string>? Labels { get; set; }

        public IDictionary<string, string>? Selector { get; set; }

        /// <summary>
        /// name,port
        /// </summary>
        public List<string>? Ports { get; set; }

        public string[]? Endpoints { get; set; }

        public DateTime? CreationTime { get; set; }

        // 关联的 Pod 以及 pod 的 ip
    }
}
