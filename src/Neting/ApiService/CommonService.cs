using dotnet_etcd;
using k8s;
using Microsoft.IdentityModel.Tokens;
using Neting.ApiService.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Neting.ApiService
{
    public class CommonService
    {
        private readonly Kubernetes _k8sClient;
        private readonly EtcdClient _etcd;
        private readonly NetingConfig _netingConfig;

        public CommonService(Kubernetes k8sClient, EtcdClient etcd, NetingConfig netingConfig)
        {
            _k8sClient = k8sClient;
            _etcd = etcd;
            _netingConfig = netingConfig;
        }

        /// <summary>
        /// 获取所有命名空间的名称
        /// </summary>
        /// <returns></returns>
        public async Task<DataResults<string>> GetNamespacesAsync()
        {
            var namespaces = await _k8sClient.ListNamespaceAsync();
            return new DataResults<string>
            {
                Message = "获取成功",
                Data = namespaces.Items.Select(x => x.Metadata.Name).ToArray()
            };
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<DataResult<string>> LoginAsyc(string name, string password)
        {
            if (name != _netingConfig.User || password != _netingConfig.Password)
            {
                return new DataResult<string>
                {
                    Code = -1,
                    Message = "登录失败"
                };
            }

            return new DataResult<string>
            {
                Code = 0,
                Message = "登录成功",
                Data = ToToken(new List<Claim> { new Claim("name", "admin") })
            };

            string ToToken(IEnumerable<Claim> claims)
            {
                JwtSecurityToken tokenkey = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_netingConfig.Token)),
                        SecurityAlgorithms.HmacSha256));

                string? tokenstr = default;
                try
                {
                    tokenstr = new JwtSecurityTokenHandler().WriteToken(tokenkey);
                    return tokenstr;
                }
                catch
                {
                    return "";
                }
            }
        }

    }
}
