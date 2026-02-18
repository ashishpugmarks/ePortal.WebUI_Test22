using ePortal.Shared.Configuration;
using ePortal.Shared.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

namespace ePortal.Shared.Services
{
    public class AppConfigurationService: IAppConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly GeneralSettings _settings;
        private readonly IHostingEnvironmentService _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppConfigurationService(IConfiguration configuration, IOptions<GeneralSettings> options, IHostingEnvironmentService env,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _settings = options.Value;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }

        public GeneralSettings GetGeneralSettings()
        {
            return _settings;
        }

        public string MapPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Relative path must not be null", nameof(relativePath));

            //// Remove ~/ if present
            //if (relativePath.StartsWith("~/"))
            //    relativePath = relativePath.Substring(2);

            //return Path.Combine(_env.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            //string testPath = _env.GetWebRootPath();

            //return Path.Combine(_env.GetWebRootPath(), relativePath);
            return relativePath;

        }

        public string GetServerName()
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Request?.Host.Host.ToLower() ?? string.Empty;
        }

    }  
}
