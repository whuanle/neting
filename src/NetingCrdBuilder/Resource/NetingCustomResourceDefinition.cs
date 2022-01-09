using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neting.Kubernetes.Resource
{
    /// <summary>
    /// Neting 在 Kubernetes 自定义 CustomResourceDefinition 的核心信息，用于创建资源类型
    /// </summary>
    public class NetingCustomResourceDefinition
    {
        public string Version { get; set; } = null!;

        public string Group { get; set; } = null!;

        public string PluralName { get; set; } = null!;

        public string Kind { get; set; } = null!;

        public string Namespace { get; set; } = null!;
    }
}
