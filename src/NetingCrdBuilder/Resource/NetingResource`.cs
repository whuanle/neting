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
    /// <typeparam name="TSpec">YAML.spec</typeparam>
    /// <typeparam name="TStatus"></typeparam>
    public class NetingResource<TSpec, TStatus> : NetingResource
    {
        /// <summary>
        /// Spec
        /// </summary>
        [JsonProperty(PropertyName = "spec")]
        public TSpec Spec { get; set; }

#warning 不知道有什么用
        [JsonProperty(PropertyName = "CStatus")]
        public TStatus CStatus { get; set; }
    }
}
