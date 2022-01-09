namespace Neting.Modes.Backend
{
    public class NetingBackendService
    {
        // 服务名称
        public string Name { get; set; }
        // 目标端口
        public int TargetPort { get; set; }

        public string Method { get; set; } = "http://";

        /// <summary>
        /// 路由地址
        /// </summary>
        public string Path { get; set; }
    }
}
