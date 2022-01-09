using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Neting.Database
{
    public class NetingCluster
    {
        [Required(ErrorMessage = "Cluster 名称不能为空")]
        public virtual string Name { get; set; } = null!;
        public virtual string? Description { get; set; }

        private HashSet<NetingClusterDestination>? _services;
        public virtual HashSet<NetingClusterDestination>? Services
        {
            get => _services;
            set
            {
                if (value == null) return;
                _services = new HashSet<NetingClusterDestination>(value, new ClusterServiceEqualityComparer());
            }
        }

    }

    /// <summary>
    /// 一个映射的目的地址
    /// </summary>
    public class NetingClusterDestination
    {
        /// <summary>
        /// 此服务名称
        /// </summary>
        [Required(ErrorMessage = "服务名称不能为空")]
        public virtual string Name { get; set; } = null!;

        /// <summary>
        /// http 还是 https
        /// </summary>
        [Required(ErrorMessage = "请填写请求方式，http 或 https")]
        public string Protocol { get; set; } = null!;

        /// <summary>
        /// 域名或IP
        /// </summary>
        [Required(ErrorMessage = "请填写服务主机地址")]
        public string HostName { get; set; } = null!;

        /// <summary>
        /// 端口
        /// </summary>
        [Required(ErrorMessage = "请填写服务端口")]
        public int Port { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [Required(ErrorMessage = "请填写服务 url 地址")]
        public string Path { get; set; } = null!;

        /// <summary>
        /// 配置健康检查
        /// </summary>
        public string? Health { get; init; }

        /// <summary>
        /// 标签
        /// </summary>
        public Dictionary<string, string>? Metadata { get; init; }
    }


    public class ClusterServiceEqualityComparer : IEqualityComparer<NetingClusterDestination>
    {
        public bool Equals(NetingClusterDestination? x, NetingClusterDestination? y)
        {
            return x?.Name == y?.Name || x?.Path == y?.Path;
        }

        public int GetHashCode([DisallowNull] NetingClusterDestination obj)
        {
            return HashCode.Combine(obj.Name);
        }
    }
}
