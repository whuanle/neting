using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Neting
{
    public static class NetingAuthorizationExtesions
    {
        public static void AddNetingAuthorization(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();              // 注入 HttpContextAccessor
            services.AddScoped<HttpContext>(x =>            // 注入 HttpContext
            {
                var httpAccessor = x.GetService<IHttpContextAccessor>();
                return httpAccessor!.HttpContext!;
            });

            // 注册权限处理
            services.AddAuthorization();

            // 注册和配置权限验证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(NetingConfig.Instance.Token)),    // 加密解密Token的密钥，应当从环境变量中获取

                            ValidateIssuer = false,
                            ValidateAudience = false,

                            // 是否验证令牌有效期
                            ValidateLifetime = true,
                            // 每次颁发令牌，令牌有效时间
                            ClockSkew = TimeSpan.FromDays(7)
                        };
                    });
        }
    }
}
