using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neting.ApiService;
using Neting.ApiService.Models;
using System.Threading.Tasks;

namespace Neting.Controller
{
    /// <summary>
    /// 管理 Kubernetes 中的 Service
    /// </summary>
    [ApiController]
    [Route("/common")]
    [Authorize]
    public class CommonController
    {
        private readonly CommonService _service;
        public CommonController(CommonService service)
        {
            _service = service;
        }

        public class LoginInput
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResult<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginInput input)
        {
            var result = await _service.LoginAsyc(input.UserName, input.Password);
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取所有命名空间
        /// </summary>
        /// <returns></returns>
        [HttpGet("namespaces")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DataResults<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServices()
        {
            var result = await _service.GetNamespacesAsync();
            return new JsonResult(result);
        }
    }
}
