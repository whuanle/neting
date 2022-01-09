using Neting.ApiService.Models;
using Neting.Database;
using System.Threading.Tasks;

namespace Neting.ApiService
{
    /// <summary>
    /// Cluster 服务
    /// </summary>
    public class NetingClusterService
    {
        private readonly NetingDateBase _database;

        public NetingClusterService(NetingDateBase database)
        {
            _database = database;
        }


        public async Task<DataResults<string>> GetClusterNamesAsync()
        {
            var result = await _database.NetingCluster.GetKeysAsync();
            return new DataResults<string>
            {
                Message = "获取成功",
                Data = result
            };
        }

        /// <summary>
        /// 获取 Cluster 列表
        /// </summary>
        /// <returns></returns>
        public async Task<DataResults<NetingCluster>> GetClustersAsync()
        {
            var result = await _database.NetingCluster.GetsAsync();
            return new DataResults<NetingCluster>
            {
                Message = "获取成功",
                Data = result.Values
            };
        }

        /// <summary>
        /// 创建 Cluster 服务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataResult> CreateClusterAsync(NetingCluster input)
        {
            // 判断是否存在
            if (await _database.NetingCluster.ExistKeyAsync(input.Name))
            {
                return new DataResult
                {
                    Code = -1,
                    Message = "Cluster 已存在"
                };
            }

            // 存到 etcd
            await _database.NetingCluster.AddAsync(input.Name, input);

            return new DataResult
            {
                Message = "创建完成"
            };
        }


        public async Task<DataResult> DeleteClusterAsync(string name)
        {
            await _database.NetingCluster.RemoveAsync(name);
            return new DataResult
            {
                Message = "已删除"
            };
        }

    }
}
