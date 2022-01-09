using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;

namespace Neting
{
    /// <summary>
    /// 配置
    /// </summary>
    public class NetingConfig
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string User { get; set; } = null!;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = null!;

        public string Etcd { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; } = null!;

        public int Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Kubernetes { get; set; } = null!;

        /// <summary>
        /// 当前是否为开发环境
        /// </summary>
        public static bool IsDevelopment { get; private set; }


        public static void Init(bool value)
        {
            IsDevelopment = value;
            if (value)
            {
                var jsonName = "appsettings.Development.json";
                var jsonFile = Path.Combine(Directory.GetParent(typeof(NetingConfig).Assembly.Location)!.FullName, jsonName);
                if (!File.Exists(jsonFile))
                {
                    jsonName = "appsettings.json";
                }
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile(jsonName, true, false)
                    .Build();

                Instance = config.GetSection("Neting").Get<NetingConfig>() ?? throw new ArgumentNullException("未找到启动配置");
            }
            else
            {
                Instance = new NetingConfig
                {
                    User = Environment.GetEnvironmentVariable("NETING_USER") ?? throw new ArgumentNullException("未找到启动环境变量 NETING_USER"),
                    Password = Environment.GetEnvironmentVariable("NETING_PASSWORD") ?? throw new ArgumentNullException("未找到启动环境变量 NETING_PASSWORD"),
                    Etcd = "http://" + (Environment.GetEnvironmentVariable("NETING_ETCD_SERVICE_HOST") ?? throw new ArgumentNullException("未找到启动环境变量 NETING_ETCD_SERVICE_HOST"))
                    + ":" + Environment.GetEnvironmentVariable("NETING_ETCD_SERVICE_PORT") ?? throw new ArgumentNullException("未找到启动环境变量 NETING_ETCD_SERVICE_PORT"),
                    Token = Environment.GetEnvironmentVariable("NETING_TOKENKEY") ?? throw new ArgumentNullException("未找到启动环境变量 NETING_TOKENKEY"),
                    Port = 80
                };
                //Instance.User = Convert.ToBase64String(Encoding.ASCII.GetBytes(Instance.User));
                //Instance.Password = Convert.ToBase64String(Encoding.ASCII.GetBytes(Instance.Password));
                //Instance.Token = Convert.ToBase64String(Encoding.ASCII.GetBytes(Instance.Token));
            }
        }


        /// <summary>
        /// 单例
        /// </summary>
        public static NetingConfig Instance { get; private set; } = null!;
    }
}
