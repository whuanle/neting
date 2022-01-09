using k8s;
using Microsoft.Extensions.DependencyInjection;

namespace Neting.NetingKubernetes
{
    public static class KubernetesExtensions
    {
        public static void AddKubernetesClient(this IServiceCollection services)
        {
            KubernetesClientConfiguration config;
            if (NetingConfig.IsDevelopment)
            {
                config = KubernetesClientConfiguration.BuildConfigFromConfigFile(NetingConfig.Instance.Kubernetes);
            }
            else
            {
                config = KubernetesClientConfiguration.BuildConfigFromConfigFile("/root/.kube/config");
            }
            services.AddSingleton(s =>
            {
                return new Kubernetes(config);
            });
        }
    }
}
