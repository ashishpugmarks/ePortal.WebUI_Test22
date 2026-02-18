using DocumentFormat.OpenXml;
using ePortal.Application.Contracts;
using ePortal.Application.Services;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ePortal.WebUI.Middleware
{
    public class SessionRestoreMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IConfiguration _settings;


        public SessionRestoreMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IConfiguration settings)
        {
            _next = next;
            _settings = settings;
            _logger = logger;

        }

        public async Task InvokeAsync(HttpContext context)
        {

            //Commented By TTL against CR6852 as on 05-08-2025 
            //var currDate = DateTime.UtcNow.Date;
            //var expiryDate = new DateTime(2025, 8, 1);

            //if (currDate <= expiryDate)
            //{
            //    var serverName = Environment.MachineName;
            //    var path = context.Request.Path;
            //    var userId = context.Session.GetString("userID") ?? "Anonymous";

            //    _logger.LogError("Info: Request handled by server: {" + serverName + "}, Path:{" + path + "} for User:{" + userId + "}");
            //}


            var currentPath = context.Request.Path.Value;
            List<string> excludedPaths = new List<string> { "login", "/tokenbridge/handlefromnewapp", "/tokenbridge/oldlogin" , "/piomrequest/regeneratedoc", "/tokenbridge/callnewappregeneratedoc" };
            currentPath = string.IsNullOrEmpty(currentPath) ? "" : currentPath.ToLower();

            if (!string.IsNullOrEmpty(Path.GetExtension(currentPath)) || excludedPaths.Any(x => currentPath.Contains(x)) || currentPath == "/")
            {
                await _next(context);
                return;
            }

            //var currentPath1 = context.Request.Path.Value;
            ////List<string> excludedPaths1 = new List<string> { "/bikercafe/", "/canteen/", "/corporatenews/", "/a00submit/", "/assettransfer/","/announcement/","/acr/","/assetregistration/"};
            //List<string> excludedPaths1 = new List<string> { "/bikercafe/" };
            //currentPath1 = string.IsNullOrEmpty(currentPath1) ? "" : currentPath1.ToLower();
            //if (excludedPaths1.Any(x => currentPath1.Contains(x)))
            //{
            //    context.Session.Clear();
            //}

            if (context.Session.GetString("userID") == null)
            {
                if (context.Request.Cookies.TryGetValue(_settings["GeneralSettings:CookieName"].ToString(), out var dataFromCookies))
                {
                    try
                    {
                        dataFromCookies = Encryption.Decrypt(dataFromCookies);
                        CookieDetails? obj = JsonConvert.DeserializeObject<CookieDetails>(dataFromCookies);
                        if (obj != null)
                        {
                            //_logger.LogError("Read Cookie:  cookie=" + dataFromCookies);

                            context.Session.SetString("userID", Convert.ToString(obj.UserID));
                            context.Session.SetString("userName", obj.UserName ?? "");
                            context.Session.SetString("usertype", obj.UserType ?? "");
                            context.Session.SetString("EMPLtype", obj.EmpType ?? "");
                            context.Session.SetString("ISSSOLOGIN", Convert.ToString(obj.ISSSOLOGIN));  //--90/30 days password policy
                            context.Session.SetString("NetworkType", obj.NetworkType ?? ""); //Added By TTL against CR6852 as on 31-07-2025 

                            //Added By TTL against CR6852 as on 31-07-2025 
                            string empDetailsJson = JsonConvert.SerializeObject(obj.EmpDetails);
                            context.Session.SetString("Employee", empDetailsJson);


                            //_sessionService.Set("pass", strpasswordencode);
                            context.Session.SetString("IdleTimeoutMinutes", Convert.ToString(_settings["GeneralSettings:IdleTimeoutMinutes"]));
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Response.Redirect("/Login/Index");
                        return;
                    }
                }
                else
                {
                    context.Response.Redirect("/Login/Index");
                    return;
                }
            }
            await _next(context);
        }
    }
}
