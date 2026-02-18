using ePortal.Shared.Configuration;
using ePortal.Shared.Interface;
using ePortal.WebUI.Controllers;
using Microsoft.Extensions.Options;
using Serilog;

namespace ePortal.WebUI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly GeneralSettings _settings;

        public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<GlobalExceptionMiddleware> logger, IOptions<GeneralSettings> options)
        {
            _next = next;
            _env = env;
            _logger = logger;
            _settings = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var routeData = context.GetRouteData();
                var controllerName = routeData?.Values["controller"]?.ToString();
                var action = routeData?.Values["action"]?.ToString();

                try
                {


                    if (!context.Response.HasStarted)
                    {

                        // --- 1. Get User ID from session ---
                        var userId = context.Session.GetString("userID") ?? "Anonymous";

                        // --- 2. Log with Serilog ---
                        _logger.LogError(ex, "Unhandled Exception for user {UserId}", userId);
                       

                        // --- 3. Write to file ---
                        //var logDir = Path.Combine(_env.WebRootPath, "Uploads", "GlobalError");
                        var logDir = _settings.Get_FileUpload_Path + "GlobalError_Upgrade";
                        Directory.CreateDirectory(logDir); // Ensure folder exists

                        var logPath = Path.Combine(logDir, $"errorLog_ErrorStage_{userId}.txt");

                        var content = $"Global (Exception in {controllerName}/{action}) Step-1: emp id - {userId}, DateTime: {DateTime.Now}, " +
                                      $"StackTrace: {ex.StackTrace}, Message: {ex.Message}{Environment.NewLine}";

                        await File.AppendAllTextAsync(logPath, content);
                         


                        // --- 4. Redirect to proper error action based on status code ---
                        var statusCode = context.Response.StatusCode;

                        if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                        {
                            context.Response.Redirect("/AppError/HttpError404");
                        }
                        else if (ex is InvalidOperationException)
                        {
                            context.Response.Redirect("/AppError/HttpError500");
                        }
                        else
                        {
                            context.Response.Redirect("/AppError/General");
                        }

                        // Optional: Execute any DI tasks (equivalent of IRunOnError)
                        // await _errorTaskService.RunAsync();  <-- You can inject via constructor
                    }
                }
                catch (Exception innerEx)
                {
                    // Final fallback in case error handling fails
                    // var fallbackPath = Path.Combine(_env.WebRootPath, "Uploads", "GlobalError", "fallback.txt");

                    _logger.LogError($"Error in middleware catch block: {innerEx.Message}, Inner: {innerEx.InnerException}");

                    var fallbackPath = Path.Combine(_settings.Get_FileUpload_Path, "GlobalError_Upgrade", "fallback.txt") ;
                    await File.AppendAllTextAsync(fallbackPath, $"Error in middleware catch block: {innerEx.Message}, Inner: {innerEx.InnerException}");
                }
            }
        }
    }
}
