using Neting.ApiService.Models;
using Neting.Database;
using System.Threading.Tasks;

namespace Neting.ApiService
{
    public class NetingRouteService
    {
        private readonly NetingDateBase _database;

        public NetingRouteService(NetingDateBase database)
        {
            _database = database;
        }

        /// <summary>
        /// 获取 Route 列表
        /// </summary>
        /// <param name="skipCount"></param>
        /// <param name="takeCount"></param>
        /// <returns></returns>
        public async Task<DataResults<NetingRoute>> GetRoutesAsync(int skipCount, int takeCount)
        {
            var result = await _database.NetingRoute.GetsAsync();
            return new DataResults<NetingRoute>
            {
                Message = "查询成功",
                Data = result.Values
            };
        }

        public async Task<DataResult> CreateRouteAsync(NetingRoute input)
        {
            // 判断是否存在
            if (await _database.NetingRoute.ExistKeyAsync(input.Name))
            {
                return new DataResult
                {
                    Code = -1,
                    Message = "Route 已存在"
                };
            }

            // 存到 etcd
            await _database.NetingRoute.AddAsync(input.Name, input);

            return new DataResult
            {
                Message = "已创建 Route"
            };
        }

        public async Task<DataResult> DeleteRouteAsync(string name)
        {
            await _database.NetingRoute.RemoveAsync(name);
            return new DataResult
            {
                Message = "已删除"
            };

        }
    }
}
