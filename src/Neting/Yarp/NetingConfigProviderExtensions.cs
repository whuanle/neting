using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;

namespace Neting.Yarp
{
    public static class NetingConfigProviderExtensions
    {
        public static IReverseProxyBuilder LoadFromEtcd(this IReverseProxyBuilder builder)
        {
            builder.Services.AddSingleton<IProxyConfigProvider, NetingProxyConfigProvider>();
            return builder;
        }
    }
}
