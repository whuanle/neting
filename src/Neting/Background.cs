using dotnet_etcd;
using Microsoft.Extensions.DependencyInjection;
using Neting.Yarp;
using System;

namespace Neting
{
    internal static class Background
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        private static ServiceProvider _serviceProvider;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        public static IServiceProvider Service => _serviceProvider.CreateScope().ServiceProvider;
        public static void AddNetingWatch(this IServiceCollection services)
        {
            _serviceProvider = services.BuildServiceProvider() ?? throw new ArgumentNullException();
            NetingWatch.RefreshCluster().Wait();
            NetingWatch.RefreshRouter().Wait();
            NetingWatch.InitWatch(Service.GetRequiredService<EtcdClient>());
        }
    }
}
