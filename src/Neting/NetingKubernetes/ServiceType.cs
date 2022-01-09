namespace Neting.NetingKubernetes;

public enum ServiceType
{
    ClusterIP,
    NodePort,
    LoadBalancer,

    ExternalName
}
