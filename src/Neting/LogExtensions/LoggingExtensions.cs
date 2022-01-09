using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace Neting.LogExtensions
{
    public static class LoggingExtensions
    {
        public static void AddSerilogLogging(this IServiceCollection services)
        {
            using var scoped = services.BuildServiceProvider();
            if (scoped == null)
            {
                throw new InvalidOperationException("无法构建依赖注入容器");
            }
            IConfiguration? configuration = scoped.GetService<IConfiguration>();
            if (configuration == null)
            {
                throw new InvalidOperationException("未找到程序启动配置文件");
            }

            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(configuration)
#if DEBUG
                            .MinimumLevel.Debug()
#else
                            .MinimumLevel.Information()
#endif
                            .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}
