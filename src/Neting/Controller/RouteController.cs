using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neting.ApiService;
using Neting.ApiService.Models;
using Neting.Database;
using System.Threading.Tasks;

namespace Neting.Controller
{
    /// <summary>
    /// 创建路由服务
    /// </summary>
    [ApiController]
    [Route("/route")]
    [Authorize]
    public class RouteController
    {
        private readonly NetingRouteService _service;
        public RouteController(NetingRouteService service)
        {
            _service = service;
        }



        [HttpDelete("delete")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResults<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string name)
        {
            var result = await _service.DeleteRouteAsync(name);
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取 Route 列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResults<NetingRoute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClustersAsync(int? pageNo, int? pageSize)
        {
            int skipCount = pageNo.GetValueOrDefault();
            int takeCount = pageSize.GetValueOrDefault();
            skipCount = (skipCount - 1) * takeCount;
            var result = await _service.GetRoutesAsync(skipCount, takeCount);
            return new JsonResult(result);
        }

        /// <summary>
        /// 创建 Route
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateClusterAsync([FromBody] NetingRoute route)
        {
            var result = await _service.CreateRouteAsync(route);
            return new JsonResult(result);
        }
    }
}
