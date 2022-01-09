using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neting.ApiService;
using Neting.LogExtensions;
using Neting.NetingEtcd;
using Neting.NetingKubernetes;
using Neting.Yarp;

namespace Neting
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<NetingConfig>(sv => NetingConfig.Instance);

            // 日志
            services.AddSerilogLogging();

            // 身份认证
            services.AddNetingAuthorization();

            // Kubernetes
            services.AddKubernetesClient();

            // Etcd
            services.AddEtcdClient();

            // yarp
            services.AddReverseProxy()
                .LoadFromEtcd();

            // 注入应用层服务
            services.AddApplication();
#if DEBUG
            services.AddCors(options =>
            {
                options.AddPolicy("dev",
                    builder => builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin());
            });
#endif
            services.AddControllers();

            // 最后注入 etcd 监控
            services.AddNetingWatch();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

#if DEBUG
            app.UseCors("dev");
#endif

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
