using ePortal.Application.Contracts;
using ePortal.Shared.Interface;

namespace ePortal.WebUI.Helpers
{
    public class HostingEnvironmentService : IHostingEnvironmentService
    {
        private readonly IWebHostEnvironment _env;

        public HostingEnvironmentService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public string GetWebRootPath()
        {
            return _env.WebRootPath;
        }
    }
}
