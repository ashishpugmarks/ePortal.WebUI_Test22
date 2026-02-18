using System.Collections;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using ePortal.WebUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ePortal.WebUI.Controllers
{
    //[Route("TokenBridge")]
    public class TokenBridgeController : Controller
    {
        private readonly ILogger _logger;
        private readonly ISessionService _sessionService;
        private readonly IConfiguration _settings;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmpLoginService _loginService;
        private readonly IAppConfigurationService _configuration;

        public TokenBridgeController(ILogger<HomeController> logger, IConfiguration settings, ISessionService sessionService, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IEmpLoginService loginService, IAppConfigurationService appConfiguration)
        {
            _logger = logger;
            _sessionService = sessionService;
            _settings = settings;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _loginService = loginService;
            _configuration = appConfiguration;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> RedirectToOldApp(string target)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                _logger.LogError("Info in RedirectToOldApp: userID is null so redirected to login page."); //Added By TTL against CR6695 as on 30-07- 2025
                return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString() + "/Home/Home"); //Changed By TTL against CR6695 as on 30-07- 2025
            }

            // Below logic will keep user in new application's home page.
            //Keep this logiv
            if (string.IsNullOrEmpty(target))
            {
                _logger.LogError("Info in RedirectToOldApp: target is null so redirected to login page."); //Added By TTL against CR6695 as on 30-07- 2025
                return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString() + "/Home/Home");
            }


            try
            {
                var userData = new
                {
                    userId = _sessionService.Get<string>("userID"),
                    userName = _sessionService.Get<string>("userName"),
                    userType = _sessionService.Get<string>("usertype")
                };

                var json = JsonConvert.SerializeObject(userData);
                var encData = SecurityHelper.AESEncrypt(json, _settings["Switch_New_Old_New:SecretKey"].ToString());

                var claims = new[] { new Claim("encData", encData) };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings["Switch_New_Old_New:SecretKey"].ToString()));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                string clientIP = "";
                clientIP = GetClientIp();

                var token = new JwtSecurityToken(
                    issuer: "NewApp",
                    audience: "OldApp",
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: creds);


                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var encodedTarget = WebUtility.UrlEncode(target);
                var encodedToken = WebUtility.UrlEncode(tokenString);

                //var redirectUrl = $"{_settings["Switch_New_Old_New:OLD_APP_URL"].ToString()}/TokenBridgeOld/HandleFromOldApp?target={encodedTarget}&token={encodedToken}";
                //var redirectUrl = $"{_settings["Switch_New_Old_New:OLD_APP_URL"].ToString()}/TokenBridge/HandleFromOldApp";

                ViewBag.Token = encodedToken;
                ViewBag.Target = encodedTarget;
                ViewBag.OldAppUrl = $"{_settings["Switch_New_Old_New:OLD_APP_URL"].ToString()}/TokenBridge/HandleFromOldApp"; //"http://localhost:44378/TokenBridge/HandleFromNewApp"; //

                //clear current app session.
                //_sessionService.Clear();
            }
            catch (Exception ex)
            {
                //Added By TTL against CR6695 as on 30-07- 2025
                _logger.LogError(ex, "Error in RedirectToOldApp: Unexpected error during token processing." + ex.Message.ToString());
                return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString() + "/Home/Home");
            }

            return View("PostRedirect");
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> HandleFromNewApp(string target, string token)
        {

            try
            {

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(target))
                {
                    _logger.LogError("Error in HandleFromNewApp: Missing token or target");
                    //return Unauthorized("Missing token or target");
                    return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString() + "/Home/Home");
                }

                //Handled url with multiple parameters
                target = target.Replace("_A_N_D_", "&");

                target = WebUtility.UrlDecode(target);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_settings["Switch_New_Old_New:SecretKey"].ToString());
                int lngResult = 0;



                //clear session
                _sessionService.Clear();
                //Added By TTL against CR6695 as on 29 - 07 - 2025
                _loginService.DeleteCookie(_settings["GeneralSettings:CookieName"].ToString()); //clear cookies

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "OldApp",
                    ValidateAudience = true,
                    ValidAudience = "NewApp",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    //ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                }, out SecurityToken validatedToken);

                var encData = principal.FindFirst("encData")?.Value;
                var json = SecurityHelper.AESDecrypt(encData, _settings["Switch_New_Old_New:SecretKey"].ToString());
                var user = JsonConvert.DeserializeObject<UserSessionModel>(json);

                var userId = user.userId;
                var userName = user.userName;
                var userType = user.userType;

                if (string.IsNullOrEmpty(userType)) userType = "1";
                                
                //_logger.LogError("Token-Validated userId=" + user.userType + " userId=" + userId + " userType" + userType + " userName" + userName);

                string _Client_Ip = GetClientIp();

                //Below piece of code is only need to support switching logic.
                if (_sessionService.Get<string>("IdleTimeoutMinutes") == null)
                {
                    _sessionService.Set<string>("IdleTimeoutMinutes", _settings["GeneralSettings:IdleTimeoutMinutes"].ToString());
                }

                //Added By TTL against CR6852 as on 31-07-2025 
                try
                {
                    _loginService.Check_Switch_EmpLogin(userId, _Client_Ip, userType);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Check_Switch_EmpLogin() throws error " + ex.Message.ToString());
                }


                //lngResult = 1;

                //if (lngResult == 1)
                //{
                    String[] arrecode = _loginService.GetParameterValue("INTERNETVALIDATION").Split(',');
                    ArrayList collecode = new ArrayList(arrecode);

                    try
                    {
                        if (_Client_Ip == "0.0.0.0")
                        {
                            _logger.LogError("_Client_Ip not found set default ip: " + Convert.ToString(_Client_Ip)+" for User: "+Convert.ToString(userId));
                            TempData["NetworkType"] = "0";//Intranet
                            _sessionService.Set("NetworkType", 0);
                            TempData.Keep("NetworkType");
                        }
                        else
                        {
                            string striprange = _Client_Ip.Substring(0, 6);
                            if (!collecode.Contains(striprange))
                            {
                                TempData["NetworkType"] = "1";//Internet
                                _sessionService.Set("NetworkType", 1);
                            }
                            else
                            {
                                TempData["NetworkType"] = "0";//Intranet
                                _sessionService.Set("NetworkType", 0);
                            }
                            TempData.Keep("NetworkType");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("_Client_Ip is null message:" + Convert.ToString(ex.Message)+" for User: "+ Convert.ToString(userId));
                        TempData["NetworkType"] = "0";//Intranet
                        _sessionService.Set("NetworkType", 0);
                        TempData.Keep("NetworkType");
                    }


                    Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                    _sessionService.Set("userID", obj._ECode);
                    _sessionService.Set("userName", obj._EName);
                    _sessionService.Set("usertype", userType);
                    _sessionService.Set("EMPLtype", obj._EmpLtype ?? "");
                    _sessionService.Set("ISSSOLOGIN", 1);  //--90/30 days password policy

                    //_logger.LogError("Session created");

                    ViewBag.UserName = obj._EName;

                    //Added By TTL against CR6695 as on 29 - 07 - 2025                    
                    CookieDetails cookieDetails = new CookieDetails();
                    cookieDetails.UserID = obj._ECode;
                    cookieDetails.UserName = obj._EName;
                    cookieDetails.UserType = userType;
                    cookieDetails.EmpType = obj._EmpLtype ?? "";
                    cookieDetails.ISSSOLOGIN = 1;
                    cookieDetails.EmpDetails = obj; //Added By TTL against CR6852 as on 31-07-2025 
                    cookieDetails.NetworkType = _sessionService.Get<string>("NetworkType"); //Added By TTL against CR6852 as on 31-07-2025 
                    cookieDetails.IdleTimeoutMinutes = Convert.ToInt32(_settings["GeneralSettings:IdleTimeoutMinutes"]);
                    //add user in cookies
                    _loginService.WriteCookie(_settings["GeneralSettings:CookieName"].ToString(), cookieDetails, Convert.ToInt32(_settings["GeneralSettings:CookieExpireInDays"]));

                    //_logger.LogError("Cookies created");

                    // Decode original target and redirect
                    string decodedTarget = Uri.UnescapeDataString(target);
                    //string decodedTarget = WebUtility.UrlDecode(target);
                    string finalTarget = SafeEncodeUrl(decodedTarget);

                    return Redirect(finalTarget);

                //}
                //else
                //{
                //    _logger.LogError("Error in HandleFromNewApp: Check_Switch_EmpLogin return 0. userId=" + userId + " userName " + userName + " userType " + userType);
                //}


                //return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString() + "/Home/Home");
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogError(ex, "Error in HandleFromNewApp: Unexpected error during token processing." + Convert.ToString(ex.Message));

                //redirect back to old app or show session expired message.
                return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString() + "/Home/Home");
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Error in HandleFromNewApp: Unexpected error during token processing." + Convert.ToString(ex.Message));

                return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString() + "/Home/Home");
            }
            catch (Exception ex)
            {

                //string fallbackPath = _configuration.GetGeneralSettings().Get_FileUpload_Path+ "\\GlobalError_Upgrade\\APIError.txt";

                //await System.IO.File.AppendAllTextAsync(fallbackPath, $"Error in HandleFromNewApp: TokenBridgeController catch block (Unexpected error during token processing.): {ex.Message}, Inner: {ex.InnerException}" + Environment.NewLine);

                _logger.LogError($"Error in HandleFromNewApp: TokenBridgeController catch block (Unexpected error during token processing.): {Convert.ToString(ex.Message)}, Inner: {ex.InnerException}");
                //return Unauthorized("Invalid or expired token");
                return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString() + "/Home/Home");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ClearSessionAfterRedirect()
        {
            _sessionService.Clear();
            return Ok();
        }


        //Added By TTL against CR6695 as on 29 - 07 - 2025
        [AllowAnonymous]
        public IActionResult OldLogin()
        {
            return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString());
        }



        private string SafeEncodeUrl(string rawTarget)
        {

            // Split path and query manually
            var questionMarkIndex = rawTarget.IndexOf('?');

            if (questionMarkIndex == -1)
            {
                // No query string, return path as-is
                return rawTarget;
            }

            var path = rawTarget.Substring(0, questionMarkIndex);
            var query = rawTarget.Substring(questionMarkIndex + 1);

            // Split query manually and encode keys/values
            var safeQuery = query
                .Split('&')
                .Select(pair =>
                {
                    var kv = pair.Split(new[] { '=' }, 2);
                    var key = WebUtility.UrlEncode(kv[0]);
                    var value = kv.Length > 1 ? WebUtility.UrlEncode(kv[1]) : "";
                    return $"{key}={value}";
                });

            return path + "?" + string.Join("&", safeQuery);
        }


        //_____________________________________________________________________________________________________
        //_____________________________________________________________________________________________________

        public string? GetClientIp()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null) return "0.0.0.0"; //default IP

                if (httpContext.Request.Host.Host == "localhost")
                    return "10.249.16.17";

                var ipFromHeader = httpContext.Request.Headers["HTTP_CLIENT_IP"].FirstOrDefault();
                return ipFromHeader ?? httpContext.Connection.RemoteIpAddress?.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetClientIp()" + Convert.ToString(ex.Message));
                return "0.0.0.0"; //default IP
            }
        }


        public static string Randomotken()
        {

            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[8];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < 8; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);

        }

        public class UserSessionModel
        {
            public string userId { get; set; }
            public string userName { get; set; }
            public string userType { get; set; }
        }

    }
}
