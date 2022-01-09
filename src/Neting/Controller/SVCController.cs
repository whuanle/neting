using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neting.ApiService;
using Neting.ApiService.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Neting.Controller
{
    /// <summary>
    /// 管理 Kubernetes 中的 Service
    /// </summary>
    [ApiController]
    [Route("/svc")]
    [Authorize]
    public class SVCController : ControllerBase
    {
        private readonly KubernetesSVCService _service;
        public SVCController(KubernetesSVCService service)
        {
            _service = service;
        }


        /// <summary>
        ///  根据服务名称名称获取 service ip
        /// </summary>
        /// <returns></returns>
        [HttpGet("service_ips")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResult<SvcIpPort>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSvcIpsAsync([Required] string svcName, [Required] string @namespace)
        {
            var result = await _service.GetSvcIpsAsync(svcName, @namespace);
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取所有 service 名称
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        [HttpGet("service_names")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResults<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServicesNameAsync(string @namespace)
        {
            var result = await _service.GetServicesNameAsync(@namespace);
            return new JsonResult(result);
        }

        /// <summary>
        /// 分页获取 service 列表
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="pageSize"></param>
        /// <param name="continueProperty"></param>
        /// <returns></returns>
        [HttpGet("services")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResult<SvcInfoList>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServices(string? @namespace, int pageSize = 10, string? continueProperty = null)
        {
            if (@namespace == null) @namespace = "default";
            var result = await _service.GetServicesAsync(@namespace, pageSize, continueProperty);
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取一个 Service 的详细信息
        /// </summary>
        /// <param name="svcName"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        [HttpGet("serviceinfo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResult<ServiceInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetService(string svcName, string? @namespace)
        {
            if (@namespace == null) @namespace = "default";
            var result = await _service.GetServiceAsync(svcName, @namespace);
            return new JsonResult(result);
        }
    }
}
