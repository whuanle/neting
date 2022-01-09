using k8s;
using k8s.Models;
using System.Text.Json.Serialization;

namespace Neting.K8sClient
{

    [KubernetesEntity(Group = "stable.ingress-net", Kind = "YarpNet", ApiVersion = "v1", PluralName = "yarpnets")]
    public class V1Neting : IKubernetesObject<V1ObjectMeta>, IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1PodSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";

        public const string KubeKind = "Neting";

        public const string KubeGroup = "stable.neting";

        [JsonPropertyName("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("metadata")]
        public V1ObjectMeta Metadata { get; set; }

        [JsonPropertyName("spec")]
        public V1PodSpec Spec { get; set; }

        [JsonPropertyName("status")]
        public V1PodStatus Status { get; set; }

        public V1Neting()
        {
        }

        public V1Neting(string apiVersion = null, string kind = null, V1ObjectMeta metadata = null, V1PodSpec spec = null, V1PodStatus status = null)
        {
            ApiVersion = apiVersion;
            Kind = kind;
            Metadata = metadata;
            Spec = spec;
            Status = status;
        }

        public virtual void Validate()
        {
            Metadata?.Validate();
            Spec?.Validate();
            Status?.Validate();
        }
    }
}
