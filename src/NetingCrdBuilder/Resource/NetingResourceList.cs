using k8s;
using k8s.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neting.Kubernetes.Resource
{
    /// <summary>
    /// Neting 对象列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetingResourceList<T> : KubernetesObject where T : NetingResource
    {
        /// <summary>
        /// 元数据
        /// </summary>
        public V1ListMeta Metadata { get; set; }

        /// <summary>
        /// Neting 资源 <see cref="NetingResource"/>
        /// </summary>
        public List<T> Items { get; set; }
    }
}
