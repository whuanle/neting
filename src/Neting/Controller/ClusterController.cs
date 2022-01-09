using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neting.ApiService;
using Neting.ApiService.Models;
using Neting.Database;
using System.Threading.Tasks;

namespace Neting.Controller
{
    [ApiController]
    [Route("/cluster")]
    [Authorize]
    public class ClusterController
    {
        private readonly NetingClusterService _service;
        public ClusterController(NetingClusterService service)
        {
            _service = service;
        }

        [HttpDelete("delete")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResults<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string name)
        {
            var result = await _service.DeleteClusterAsync(name);
            return new JsonResult(result);
        }

        [HttpGet("names")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResults<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNamesAsync()
        {
            var result = await _service.GetClusterNamesAsync();
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取 Cluster 列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResults<NetingCluster>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClustersAsync(int? pageNo = 1, int? pageSize = 10)
        {
            int skipCount = pageNo.GetValueOrDefault();
            int takeCount = pageSize.GetValueOrDefault();
            skipCount = (skipCount - 1) * takeCount;
            // 暂时不分页
            var result = await _service.GetClustersAsync();
            return new JsonResult(result);
        }

        /// <summary>
        /// 创建 Cluster
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateClusterAsync([FromBody] NetingCluster cluster)
        {
            var result = await _service.CreateClusterAsync(cluster);
            return new JsonResult(result);
        }
    }
}
