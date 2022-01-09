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
    /// Neting YAML
    /// </summary>
    public class NetingResource : KubernetesObject
    {
        /// <summary>
        /// 元数据
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public V1ObjectMeta Metadata { get; set; } = null!;
    }
}
