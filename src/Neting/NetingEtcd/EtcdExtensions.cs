using dotnet_etcd;
using Microsoft.Extensions.DependencyInjection;

namespace Neting.NetingEtcd
{
    public static class EtcdExtensions
    {
        public static void AddEtcdClient(this IServiceCollection services)
        {
            services.AddTransient(s =>
            {
                var client = new EtcdClient(NetingConfig.Instance.Etcd);
                return client;
            });
        }
    }
}
