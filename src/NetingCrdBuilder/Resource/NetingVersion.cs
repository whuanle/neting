using k8s.Models;
using System.Collections.Generic;

namespace Neting.Kubernetes.Resource
{
    public class NetingVersion
    {
        /// <summary>
        /// 创建 Neting CRD 资源
        /// </summary>
        /// <returns></returns>
        public static NetingCustomResourceDefinition MakeCRD()
        {
            var myCRD = new NetingCustomResourceDefinition()
            {
                Kind = "Neting",
                Group = "stable.neting",
                Version = "v1alpha1",
                PluralName = "netings",
            };

            return myCRD;
        }

        /// <summary>
        /// 创建 Neting 控制器
        /// </summary>
        /// <returns></returns>
        public static NetingCRDesource MakeCResource()
        {
            var myCResource = new NetingCRDesource()
            {
                Kind = "CResource",
                ApiVersion = "csharp.com/v1alpha1",
                Metadata = new V1ObjectMeta
                {
                    Name = "cr-instance-london",
                    NamespaceProperty = "default",
                    Labels = new Dictionary<string, string>
                    {
                        {
                            "identifier", "city"
                        },
                    },
                },
                // spec
                Spec = new CResourceSpec
                {
                    CityName = "London",
                },
            };
            return myCResource;
        }
    }
}
