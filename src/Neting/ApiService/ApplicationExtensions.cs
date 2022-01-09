using Microsoft.Extensions.DependencyInjection;
using Neting.Database;

namespace Neting.ApiService
{
    public static class ApplicationExtensions
    {

        public static void AddApplication(this IServiceCollection services)
        {
            services.AddTransient<CommonService>();
            services.AddTransient<KubernetesSVCService>();
            services.AddTransient<NetingRouteService>();
            services.AddTransient<NetingClusterService>();
            services.AddTransient<NetingDateBase>();
        }
    }
}
