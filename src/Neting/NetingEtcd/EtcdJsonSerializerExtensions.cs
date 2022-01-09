using dotnet_etcd;
using Etcdserverpb;
using Grpc.Core;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Neting.NetingEtcd
{
    /// <summary>
    /// Etcd 客户端序列化和反序列化扩展
    /// </summary>
    public static class EtcdJsonSerializerExtensions
    {
        /// <summary>
        /// 序列化值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="key"></param>
        /// <param name="headers"></param>
        /// <param name="deadline"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T?> GetValAsync<T>(this EtcdClient client, string key, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            var value = await client.GetValAsync(key, headers, deadline, cancellationToken);
            return typeof(T) == typeof(string) ? (T)(object)value : JsonSerializer.Deserialize<T>(value);
        }

        public static async Task<PutResponse> PutAsync<T>(this EtcdClient client, string key, T val, Metadata? headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            return await client.PutAsync(key, JsonSerializer.Serialize(val));
        }

        public static async Task<bool> ExistKeyAsync(this EtcdClient client, string key)
        {
            var response = await client.GetAsync(key);
            return response.Count > 0;
        }
    }
}
