using System.Security.Cryptography;
using ePortal.Shared.Interface;
using ePortal.WebUI.Models;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ePortal.WebUI.Filters
{
    public class CSPAttribute:ActionFilterAttribute
    {
        private readonly IConfiguration _configuration;
        private readonly ISessionService _sessionService;
        private readonly CSPOptions _options;

        //Configurable properties
        //public string FormActionEndpoints { get; set; }
        public int NoOfNonces = 11;

        public CSPAttribute(IConfiguration configuration, ISessionService sessionService, CSPOptions options)
        {
            _configuration = configuration;
            _sessionService = sessionService;
            _options= options ?? new CSPOptions();

            //Fallback default ifnot set externally
            //FormActionEndpoints= _configuration["GeneralSettings:BMSPortal"] ?? "";
        }
        
        private List<RequestInfo> RequestInfoList
        {
            get
            {
                return _sessionService.Get<List<RequestInfo>>("RequestInfo") ?? null;
            }
            set
            {
                _sessionService.Set("RequestInfo", value);
            }
        }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //check for [SkipCSP] on action
            var skipCSP = filterContext.ActionDescriptor.EndpointMetadata.OfType<SkipCSPAttribute>().Any();

            if (skipCSP)
            {
                return;
            }


            var httpContext = filterContext.HttpContext;
            var request = httpContext.Request;

            //CSP Endpoints
            string Endpoints = _configuration["GeneralSettings:CSPEndpoints"] ?? "";
            //string FormActionEndpoints = _configuration["GeneralSettings:BMSPortal"] ?? "";

            // Use FormActionEndpoints from attribute or fallback to config
            //FormActionEndpoints = string.IsNullOrEmpty(FormActionEndpoints)
            //   ? _configuration["FormActionEndpoints"]
            //   : FormActionEndpoints;


            string FormActionEndpoints = string.IsNullOrEmpty(_options.FormActionEndpoints)
            ? _configuration["GeneralSettings:BMSPortal"]
            : _options.FormActionEndpoints;

            //if (_options.FormActionEndpoints=="")
            //{
            //    FormActionEndpoints = "";
            //}
            //else if(string.IsNullOrEmpty(_options.FormActionEndpoints))
            //{
            //    FormActionEndpoints = _configuration["GeneralSettings:BMSPortal"] ?? "";
            //}
            //else
            //{
            //    FormActionEndpoints = _options.FormActionEndpoints;
            //}

            //FormActionEndpoints = string.IsNullOrEmpty(_options.FormActionEndpoints)
            //   ? _options.FormActionEndpoints :
            //   _configuration["GeneralSettings:BMSPortal"];

            NoOfNonces = Convert.ToInt32(_options.NoOfNonces);

            // Generate a secure nonce
            string ScriptNonce = "";
            string StyleNonce = "";
            string StyleHash = "";
            string ScriptHash = "";

            //Generating 10 Random Nonces
            List<string> ScriptNonces = new();
            List<string> StyleNonces =  new();
            List<string> ScriptHashes = new();
            List<string> StyleHashes =  new();

            //var isAjaxReuest = filterContext.HttpContext.Request.IsAjaxRequest();
            var isAjaxRequest = request.Headers["X-Requested-With"] == "XMLHttpRequest";

            //if (filterContext.HttpContext.Request.HttpMethod.ToLower() == "get" && !isAjaxReuest) //on every get request which is not generated from Ajax
            if(request.Method == HttpMethods.Get && !isAjaxRequest)
            {
                for (int i = 0; i < NoOfNonces; i++)
                {
                    string scriptNonce = GenerateNonce();
                    ScriptNonces.Add(scriptNonce);
                    filterContext.HttpContext.Items[$"ScriptNonce_{i}"] = scriptNonce;

                    string styleNonce = GenerateNonce();
                    StyleNonces.Add(styleNonce);
                    filterContext.HttpContext.Items[$"StyleNonce_{i}"] = styleNonce;
                }
                //store nonces in session
                UpsertRequestInfo(new RequestInfo()
                {
                    //PageName = filterContext.HttpContext.Request.Url.AbsoluteUri,
                    PageName= $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}",
                    ScriptNonces = ScriptNonces,
                    StyleNonces = StyleNonces
                });
            }
            //else if (filterContext.HttpContext.Request.HttpMethod.ToLower() == "get" && isAjaxReuest)  //on every get request which is generated from Ajax
            else if (request.Method == HttpMethods.Get && isAjaxRequest)
            {
                RequestInfo requestInfo = GetRequestInfo(new RequestInfo()
                {
                    //PageName = filterContext.HttpContext.Request.UrlReferrer?.AbsoluteUri ?? filterContext.HttpContext.Request.Url.AbsoluteUri

                    PageName = filterContext.HttpContext.Request.GetTypedHeaders().Referer?.AbsoluteUri
           ?? $"{filterContext.HttpContext.Request.Scheme}://{filterContext.HttpContext.Request.Host}{filterContext.HttpContext.Request.Path}{filterContext.HttpContext.Request.QueryString}"

                });

                if (requestInfo != null)
                {
                    ScriptNonces = requestInfo.ScriptNonces;
                    StyleNonces = requestInfo.StyleNonces;

                    for (int i = 0; i < ScriptNonces.Count(); i++)
                    {
                        filterContext.HttpContext.Items[$"ScriptNonce_{i}"] = ScriptNonces[i];
                    }
                    for (int i = 0; i < StyleNonces.Count(); i++)
                    {
                        filterContext.HttpContext.Items[$"StyleNonce_{i}"] = StyleNonces[i];
                    }
                }
            }
            else //on post methods or other than get methods
            {
                RequestInfo requestInfo = GetRequestInfo(new RequestInfo()
                {
                    //PageName = filterContext.HttpContext.Request.UrlReferrer?.AbsoluteUri ?? filterContext.HttpContext.Request.Url.AbsoluteUri
                    PageName = filterContext.HttpContext.Request.GetTypedHeaders().Referer?.AbsoluteUri
           ?? $"{filterContext.HttpContext.Request.Scheme}://{filterContext.HttpContext.Request.Host}{filterContext.HttpContext.Request.Path}{filterContext.HttpContext.Request.QueryString}"
                });
                if (requestInfo != null)
                {
                    ScriptNonces = requestInfo.ScriptNonces;
                    StyleNonces = requestInfo.StyleNonces;

                    for (int i = 0; i < ScriptNonces.Count(); i++)
                    {
                        filterContext.HttpContext.Items[$"ScriptNonce_{i}"] = ScriptNonces[i];
                    }
                    for (int i = 0; i < StyleNonces.Count(); i++)
                    {
                        filterContext.HttpContext.Items[$"StyleNonce_{i}"] = StyleNonces[i];
                    }
                }
            }
            //Adding sha256 hashes
            StyleHashes = GetStyleHashes();
            ScriptHashes = GetScriptHashes();

            ScriptNonce = string.Join(" ", ScriptNonces.Select(x => $"'nonce-{x}'"));
            StyleNonce = string.Join(" ", StyleNonces.Select(x => $"'nonce-{x}'"));
            StyleHash = string.Join(" ", StyleHashes.Select(x => $"'{x}'"));
            ScriptHash = string.Join(" ", ScriptHashes.Select(x => $"'{x}'"));

            // Define your CSP policy, incorporating the nonce for inline scripts
            string csp = $"default-src 'self'; " +
                         $"script-src {Endpoints} {ScriptNonce} {ScriptHash}; " +
                         $"style-src 'self' 'unsafe-inline' https://www.gstatic.com; " +
                         $"img-src 'self' data: blob: https://img.icons8.com https://portal.honda2wheelersindia.com; " +
                         $"font-src 'self' data:; " +
                         $"frame-ancestors 'self'; " +
                         $"object-src {Endpoints}; " +
                         $"base-uri 'self'; " +
                         $"form-action 'self' {FormActionEndpoints}; " +
                         $"worker-src {Endpoints} blob:; " +
                         $"media-src https://10.117.14.70:8088;";

            try
            {
                // Ensure the CSP header is not already set
                if (!filterContext.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    filterContext.HttpContext.Response.Headers.Append("Content-Security-Policy", csp);
                }
            }
            catch { }

            //filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            filterContext.HttpContext.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
            filterContext.HttpContext.Response.Headers.Pragma = "no-cache";





            base.OnResultExecuting(filterContext);
        }
        private static string GenerateNonce()
        {
            // Use RNGCryptoServiceProvider for a cryptographically secure nonce
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] nonceBytes = new byte[16]; // 128-bit nonce
                rng.GetBytes(nonceBytes);
                return Convert.ToBase64String(nonceBytes);
            }
        }
        private static List<string> GetStyleHashes()
        {
            List<string> hashes = new List<string>()
            {

            };
            return hashes;
        }
        private static List<string> GetScriptHashes()
        {
            List<string> hashes = new List<string>()
            {

            };
            return hashes;
        }

        private void UpsertRequestInfo(RequestInfo obj)
        {
            try
            {



                var list = RequestInfoList ?? new List<RequestInfo>();

                var itemToUpdate = list.FirstOrDefault(x => x.PageName == obj.PageName);
                if (itemToUpdate != null)
                {
                    itemToUpdate.ScriptNonces = obj.ScriptNonces;
                    itemToUpdate.StyleNonces = obj.StyleNonces;

                    // Save the updated list back to session
                    RequestInfoList = list;
                }
                else
                {
                    list.Add(obj);
                }

                RequestInfoList = list;


                //RequestInfo requestInfo = RequestInfoList?.Where(x => x.PageName == obj.PageName).FirstOrDefault();
                //if (requestInfo != null)
                //{
                //    RequestInfoList.Where(x => x.PageName == obj.PageName).FirstOrDefault().ScriptNonces = obj.ScriptNonces;
                //    RequestInfoList.Where(x => x.PageName == obj.PageName).FirstOrDefault().StyleNonces = obj.StyleNonces;
                //}
                //else
                //{
                //    RequestInfoList = RequestInfoList ?? new List<RequestInfo>();
                //    RequestInfoList.Add(obj);


                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private RequestInfo GetRequestInfo(RequestInfo obj)
        {
            try
            {
                RequestInfo requestInfo = RequestInfoList?.Where(x => x.PageName == obj.PageName).FirstOrDefault();
                if (requestInfo == null)
                {
                    List<string> ScriptNonces = new List<string>();
                    List<string> StyleNonces = new List<string>();

                    for (int i = 0; i < NoOfNonces; i++)
                    {
                        string scriptNonce = GenerateNonce();
                        ScriptNonces.Add(scriptNonce);

                        string styleNonce = GenerateNonce();
                        StyleNonces.Add(styleNonce);
                    }
                    obj.ScriptNonces = ScriptNonces;
                    obj.StyleNonces = StyleNonces;
                    return obj;
                }
                else
                {
                    return requestInfo;
                }
            }
            catch (Exception ex)
            {
                return new RequestInfo();
            }
        }
    }
    public class RequestInfo
    {
        public string PageName { get; set; }
        public List<string> StyleNonces { get; set; }
        public List<string> ScriptNonces { get; set; }
    }
}















































//        public override void OnResultExecuting(ResultExecutingContext filterContext)
//        {

//            // Generate a secure nonce
//            string ScriptNonce = "";
//            string StyleNonce = "";
//            string StyleHash = "";
//            string ScriptHash = "";

//            //Generating 10 Random Nonces
//            List<string> ScriptNonces = new List<string>();
//            List<string> StyleNonces = new List<string>();
//            List<string> ScriptHashes = new List<string>();
//            List<string> StyleHashes = new List<string>();

//            for (int i = 0; i < 11; i++)
//            {
//                string scriptNonce = GenerateNonce();
//                ScriptNonces.Add(scriptNonce);
//                filterContext.HttpContext.Items[$"ScriptNonce_{i}"] = scriptNonce;

//                string styleNonce = GenerateNonce();
//                StyleNonces.Add(styleNonce);
//                filterContext.HttpContext.Items[$"StyleNonce_{i}"] = styleNonce;
//            }
//            //Adding sha256 hashes
//            StyleHashes = GetStyleHashes();
//            ScriptHashes = GetScriptHashes();

//            ScriptNonce = string.Join(" ", ScriptNonces.Select(x => $"'nonce-{x}'"));
//            StyleNonce = string.Join(" ", StyleNonces.Select(x => $"'nonce-{x}'"));
//            StyleHash = string.Join(" ", StyleHashes.Select(x => $"'{x}'"));
//            ScriptHash = string.Join(" ", ScriptHashes.Select(x => $"'{x}'"));

//            // Define your CSP policy, incorporating the nonce for inline scripts
//            //string csp = $"default-src 'self';
//            //script-src http://localhost:52700/ {ScriptNonce} {ScriptHash};
//            //style-src 'self' 'unsafe-inline';
//            //img-src 'self' data:;
//            //font-src 'self' data:;
//            //frame-ancestors 'none'; object-src 'none';
//            //base-uri 'self';
//            //form-action 'self';
//            //worker-src http://localhost:52700 blob:;
//            //media-src http://10.117.14.46:8080";

//            var cspConfig = _configuration.GetSection("CSP");
//            string csp = $"default-src {cspConfig["DefaultSrc"]}; " +
//                         $"script-src {cspConfig["ScriptSrc"]} {ScriptNonce} {ScriptHash}; " +
//                         $"style-src {cspConfig["StyleSrc"]}; " +
//                         $"img-src {cspConfig["ImgSrc"]}; " +
//                         $"font-src {cspConfig["FontSrc"]}; " +
//                         $"object-src {cspConfig["ObjectSrc"]}; " +
//                         $"frame-ancestors {cspConfig["frameAncestors"]}; " +
//                         $"base-uri {cspConfig["baseUri"]}; " +
//                         $"form-action {cspConfig["formAction"]}; " +
//                         $"worker-src {cspConfig["workerSrc"]}; " +
//                         $"media-src {cspConfig["mediaSrc"]}; " +
//                         $"connect-src {cspConfig["connectSrc"]};";

//            // Ensure the CSP header is not already set
//            if (!filterContext.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
//            {
//                filterContext.HttpContext.Response.Headers.Append("Content-Security-Policy", csp);
//            }

//            filterContext.HttpContext.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
//            filterContext.HttpContext.Response.Headers.Pragma = "no-cache";

//            base.OnResultExecuting(filterContext);
//        }

//        private static string GenerateNonce()
//        {
//            // Use RNGCryptoServiceProvider for a cryptographically secure nonce
//            using (var rng = new RNGCryptoServiceProvider())
//            {
//                byte[] nonceBytes = new byte[16]; // 128-bit nonce
//                rng.GetBytes(nonceBytes);
//                return Convert.ToBase64String(nonceBytes);
//            }
//        }
//        private static List<string> GetStyleHashes()
//        {
//            List<string> hashes = new List<string>()
//            {

//            };
//            return hashes;
//        }
//        private static List<string> GetScriptHashes()
//        {
//            List<string> hashes = new List<string>()
//            {

//            };
//            return hashes;
//        }
//    }
//}
