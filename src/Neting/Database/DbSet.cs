using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;
using Neting.NetingEtcd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Neting.Database
{
    public abstract class DbSet<T> where T : class
    {
        private readonly EtcdClient _etcdClient;

        public DbSet(EtcdClient etcdClient)
        {
            _etcdClient = etcdClient;
        }

        public abstract string KeyPrefix { get; }

        public virtual async Task<bool> ExistKeyAsync(string name)
        {
            return await _etcdClient.ExistKeyAsync(KeyPrefix + name);
        }

        public virtual async Task AddAsync(string name, T entity)
        {
            await _etcdClient.PutAsync(KeyPrefix + name, entity);
        }

        public virtual async Task AddAsync(T entity, Func<T, string> func)
        {
            var name = func(entity);
            await _etcdClient.PutAsync(KeyPrefix + name, entity);
        }


        public virtual async Task RemoveAsync(string name)
        {
            await _etcdClient.DeleteAsync(KeyPrefix + name);
        }

        public virtual async Task<T?> GetAsync(string name)
        {
            return await _etcdClient.GetValAsync<T>(KeyPrefix + name);
        }

        /// <summary>
        /// 获取所有 keys
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> GetKeysAsync()
        {
            var result = await _etcdClient.GetAsync(new RangeRequest
            {
                Key = ByteString.CopyFromUtf8(KeyPrefix),
                RangeEnd = ByteString.CopyFromUtf8("\0"),
                KeysOnly = true,
            });
            if (result != null) return result.Kvs.Select(x => x.Key.ToStringUtf8().Remove(0, KeyPrefix.Length)).ToArray();
            return Array.Empty<string>();
        }


        public async Task<Dictionary<string, T>> GetsAsync()
        {
            var values = await _etcdClient.GetRangeValAsync(KeyPrefix);
            Dictionary<string, T> result = new Dictionary<string, T>();
            foreach (var item in values)
            {
                var value = typeof(T) == typeof(string) ? (T)((object)item.Value) : JsonSerializer.Deserialize<T>(item.Value);
                if (value == null) continue;
                result.Add(item.Key, value);
            }
            return result;
        }

        public virtual async Task UpdateAsync(T entity, Func<T, string> func)
        {
            var name = func(entity);
            await _etcdClient.PutAsync(KeyPrefix + name, entity);
        }

        /// <summary>
        /// 获取第一个值
        /// </summary>
        /// <returns></returns>
        public virtual async Task<T?> FirstAsync()
        {
            var request = new RangeRequest()
            {
                Key = ByteString.CopyFromUtf8(KeyPrefix),
                Limit = 1
            };
            var result = await _etcdClient.GetAsync(request);
            var first = result.Kvs.First();
            if (first == null) return default;
            var value = first.Value.ToStringUtf8();
            return typeof(T) == typeof(string) ? (T)((object)value) : JsonSerializer.Deserialize<T>(value);
        }

        /// <summary>
        /// 分页获取值
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, T>> PagingAsync(int skip, int count)
        {
            Dictionary<string, T> list = new Dictionary<string, T>();

            RangeRequest request;
            if (skip == 0)
            {
                request = new RangeRequest()
                {
                    Key = ByteString.CopyFromUtf8(KeyPrefix),
                    Limit = count
                };
            }
            else
            {
                // range_end ="\0"
                request = new RangeRequest
                {
                    Key = ByteString.CopyFromUtf8(KeyPrefix),
                    RangeEnd = ByteString.CopyFromUtf8("\0"),
                    KeysOnly = true,
                    Limit = skip + count + 1
                };

                // 获取 keys
                var keysResult = await _etcdClient.GetAsync(request);

                if (keysResult.Kvs.Count <= skip) return new Dictionary<string, T>();

                // 开始索引
                var keys = keysResult.Kvs.Select(x => x.Key.ToStringUtf8()).ToArray();
                var startKey = keys[skip];

                // 结束索引
                var lastIndex = skip + count;

                string endKey;
                if (lastIndex >= keys.Length)
                {
                    endKey = EtcdClient.GetRangeEnd(keys[keys.Length - 1]);
                }
                else
                {
                    endKey = keys[lastIndex];
                }
                request = new RangeRequest
                {
                    Key = ByteString.CopyFromUtf8(startKey),
                    RangeEnd = ByteString.CopyFromUtf8(endKey),
                    Limit = count
                };
            }

            var result = await _etcdClient.GetAsync(request);
            foreach (var item in result.Kvs)
            {
                var key = item.Key.ToStringUtf8();
                var str = item.Value.ToStringUtf8();
                if (string.IsNullOrEmpty(str))
                {
                    list.Add(item.Key.ToStringUtf8(), Activator.CreateInstance<T>());
                    continue;
                }

                var value = typeof(T) == typeof(string) ? (T)((object)str) : JsonSerializer.Deserialize<T>(str);
                if (value == null) continue;
                list.Add(key, value);
            }
            return list;
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> CountAsync()
        {
            var request = new RangeRequest
            {
                Key = ByteString.CopyFromUtf8(KeyPrefix),
                CountOnly = true,
                Limit = 0
            };
            var result = await _etcdClient.GetAsync(request);
            return (int)result.Count;
        }
    }
}
