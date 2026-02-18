using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    //[CSPFilter]
    public class DynamicTMPLController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _settings;
        private readonly ISessionService _sessionService;
        private readonly IDataManagement _odmt;
        private readonly IAppConfigurationService _configure;


        public DynamicTMPLController(ILogger<HomeController> logger, IWebHostEnvironment env, IConfiguration settings, ISessionService sessionService,
            IDataManagement odmt, IAppConfigurationService appConfiguration)
        {
            _logger = logger;
            _env = env;
            _settings = settings;
            _sessionService = sessionService;        
            _odmt = odmt;
            _configure = appConfiguration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ITAdmin()
        {
            return View();
        }

        //public ActionResult RedirectToTMPl(string TMPL)
        //{
        //    //Flag = TMPL_INDEX & UserId = 8607 & Password = RkWdZztM0bXjoxDcsPy33FV6mvk + ladKSBjMrwAvZ8I = &Tmpl_Code = DCN101 & ReturnURL =
        //    try
        //    {
        //        string UserName, Password, Templatecode, ReturnUrl;
        //        UserName = Convert.ToString(Session["userID"]);
        //        Password = Convert.ToString(Session["pass"]);
        //        Templatecode = TMPL;
        //        ReturnUrl = "";
        //        DataManagement odmt = new DataManagement();
        //        System.Data.DataTable dt = odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + Templatecode + "'");

        //        string tmpl_url = "";
        //        String token = "";
        //        if (dt.Rows.Count > 0)
        //        {
        //            tmpl_url = dt.Rows[0]["tmpl_url"].ToString();

        //            //23-01-2023
        //            //Token generate
        //            token = GetDynamicPlateformAPIToken();
        //            if (token == "")
        //                return RedirectToAction("Index", "Login");
        //        }
        //        //23-01-2023
        //        string q = String.Format("Flag={0}&UserId={1}&Token={2}&Tmpl_Code={3}&ReturnURL={4}", "TMPL_INDEX", UserName, token, Templatecode, ReturnUrl);

        //        string EncQ = HttpUtility.UrlEncode(Encrypt(q));

        //        //23-01-2023
        //        string url = String.Format(tmpl_url + "/demo/RedirectViaAPI" + "?q={0}", EncQ);

        //        ViewBag.url = url;
        //        return Content(@"<script language='javascript'>window.open('" + url + "');window.history.go(-1);</script>");
        //    }
        //    catch (Exception ex)
        //    {
        //        System.IO.File.AppendAllText(Server.MapPath("~/Uploads") + "/APIError.txt", ex.Message.ToString());
        //        return Content(@"<script language='javascript'>window.open('" + "');window.history.go(-1);</script>");

        //    }
        //}
        ////Redirect to IT Platform
        //public ActionResult RedirectToITTMPl()
        //{
        //    //UserId = 8607 & Token = RkWdZztM0bXjoxDcsPy33FV6mvk & ReturnURL =
        //    try
        //    {
        //        string UserName, Password, Templatecode, ReturnUrl;
        //        UserName = Convert.ToString(Session["userID"]);
        //        Password = Convert.ToString(Session["pass"]);
        //        //Templatecode = TMPL;
        //        ReturnUrl = "";
        //        DataManagement odmt = new DataManagement();
        //        System.Data.DataTable dt = odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "APITOKEN" + "'");

        //        string tmpl_url = "";
        //        String token = "";
        //        if (dt.Rows.Count > 0)
        //        {
        //            tmpl_url = dt.Rows[0]["tmpl_url"].ToString();

        //            //23-01-2023
        //            //Token generate
        //            token = GetDynamicPlateformAPIToken();
        //            if (token == "")
        //                return RedirectToAction("Index", "Login");
        //        }

        //        string q = String.Format("UserId={0}&Token={1}&ReturnURL={2}", UserName, token, ReturnUrl);
        //        string EncQ = HttpUtility.UrlEncode(Encrypt(q));

        //        string url = String.Format(tmpl_url + "/Template/RedirectViaAPI" + "?q={0}", EncQ);
        //        //string url = String.Format("http://localhost:50754" + "/Template/RedirectViaAPI" + "?q={0}", EncQ);

        //        ViewBag.url = url;
        //        return Content(@"<script language='javascript'>window.open('" + url + "');window.history.go(-1);</script>");
        //    }
        //    catch (Exception ex)
        //    {
        //        System.IO.File.AppendAllText(Server.MapPath("~/Uploads") + "/APIError.txt", ex.Message.ToString());
        //        return Content(@"<script language='javascript'>window.open('" + "');window.history.go(-1);</script>");
        //    }
        //}

        public string Encrypt(string stringToEncrypt)
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
            byte[] rgbIV = { 0x21, 0x43, 0x56, 0x87, 0x10, 0xfd, 0xea, 0x1c };
            byte[] key = { };
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes("A0D1nX0Q");
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> GetDynamicPlateformAPIToken()
        {
            try
            {
                //System.IO.File.AppendAllText(Server.MapPath("~/Uploads") + "/APIError.txt", "1");
                //DataManagement odmt = new DataManagement();
                System.Data.DataTable dt = _odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "APITOKEN" + "'");

                string tmpl_url = "";
                if (dt.Rows.Count > 0)
                {
                    tmpl_url = dt.Rows[0]["tmpl_url"].ToString();
                }

                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | (SecurityProtocolType)12288;
                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    client.BaseAddress = new Uri(tmpl_url);
                    //var responseTask = client.GetAsync(URLPATH + "//api/POApproval/GetAllPODETAIL?POID=" + POID);
                    //string APIUserName = Convert.ToString(Session["userID"]);
                    //string APIPassword = Convert.ToString(Session["pass"]);
                    string APIUserName = _sessionService.Get<string>("userID"); 
                    string APIPassword = _sessionService.Get<string>("pass");
                    string encryptionURL = "Flag=GetAuthToken&UserId=" + APIUserName + "&Password=" + APIPassword;
                    encryptionURL = HttpUtility.UrlEncode(Encrypt(encryptionURL));
                    string APIurl = "api/GetAuthToken?q=" + encryptionURL;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //System.IO.File.AppendAllText(Server.MapPath("~/Uploads") + "/APIError.txt", "2");
                    var responseTask = client.GetAsync(APIurl);
                    //System.IO.File.AppendAllText(Server.MapPath("~/Uploads") + "/APIError.txt", "3");
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync().Result;
                        var resultContent = Newtonsoft.Json.JsonConvert.DeserializeObject<DynamicToken>(readTask);
                        //return "";
                        return resultContent.Token.ToString();
                    }
                    else //web api sent error response 
                    {
                        //log response status here..
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {    
                //string fallbackPath = _configure.GetGeneralSettings().Get_FileUpload_Path+ "\\GlobalError_Upgrade\\APIError.txt";

                //await System.IO.File.AppendAllTextAsync(fallbackPath, $"Error in GetDynamicPlateformAPIToken catch block: {ex.Message}, Inner: {ex.InnerException}" + Environment.NewLine);

                _logger.LogError($"Error in GetDynamicPlateformAPIToken catch block: {ex.Message}, Inner: {ex.InnerException}");
                return "";
            }
        }


        [HttpGet]
        public async Task<string> GetDynamicPlateformApprovalCount()
        {
            try
            {
                string token = await GetDynamicPlateformAPIToken();
                if (token == "")
                    return "";

                //DataManagement odmt = new DataManagement();
                System.Data.DataTable dt = _odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "APITOKEN" + "'");
                System.Data.DataTable dt_ml = _odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "Test_ML101" + "'");

                string tmpl_url = "";
                if (dt.Rows.Count > 0)
                {
                    tmpl_url = dt.Rows[0]["tmpl_url"].ToString();
                }

                string tmpl_url_ML = "";
                if (dt.Rows.Count > 0)
                {
                    tmpl_url_ML = dt_ml.Rows[0]["tmpl_url"].ToString();
                }

                using (var client = new HttpClient())
                {
                    //https://localhost:9191/api/GetApprovalDetails?UserId=2004&Token=638043818524713946
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | (SecurityProtocolType)12288;
                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    //string APIUserName = Convert.ToString(Session["userID"]); //HttpUtility.UrlEncode(Encrypt(Convert.ToString(Session["userID"])));
                    string APIUserName = _sessionService.Get<string>("userID");

                    string encryptionURL = "Flag=GetApprovalDetails&UserId=" + APIUserName + "&Token=" + token;
                    encryptionURL = HttpUtility.UrlEncode(Encrypt(encryptionURL));

                    string APIurl = tmpl_url + "/api/GetApprovalDetails?q=" + encryptionURL;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask = client.GetAsync(APIurl);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    string APIurl_DCN = tmpl_url + "/DcnApi/GetApprovalDetails?q=" + encryptionURL;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask_DCN = client.GetAsync(APIurl_DCN);
                    responseTask_DCN.Wait();
                    var result_DCN = responseTask_DCN.Result;

                    string APIurl_FF = tmpl_url + "/FFApi/GetApprovalDetails?q=" + encryptionURL;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask_FF = client.GetAsync(APIurl_FF);
                    responseTask_FF.Wait();
                    var result_FF = responseTask_FF.Result;

                    string APIurl_ML = tmpl_url_ML + "/MLApi/GetApprovalDetails?q=" + encryptionURL;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask_ML = client.GetAsync(APIurl_ML);
                    responseTask_ML.Wait();
                    var result_ML = responseTask_ML.Result;

                    if (result.IsSuccessStatusCode && result_DCN.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync().Result;
                        var resultContent = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask);
                        var readTask_DCN = result_DCN.Content.ReadAsStringAsync().Result;
                        var resultContent_DCN = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_DCN);
                        var readTask_ML = result_ML.Content.ReadAsStringAsync().Result;
                        var resultContent_ML = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_ML);
                        var readTask_FF = result_FF.Content.ReadAsStringAsync().Result;
                        var resultContent_FF = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_FF);
                        int counts = 0;
                        if (resultContent.tmplpendingcount.Count > 0)
                        {
                            foreach (var item in resultContent.tmplpendingcount)
                            {
                                if (item.tmplcode == "EL101" || item.tmplcode == "HSO101" || item.tmplcode == "HSO102" || item.tmplcode == "HLI101" || item.tmplcode == "CES101")
                                {
                                    continue;
                                }
                                else
                                {
                                    counts += item.count;
                                }
                            }
                        }
                        int count = counts + resultContent_DCN.pendingcount + resultContent_ML.pendingcount + resultContent_FF.pendingcount;
                        return count.ToString();
                    }
                    else //web api sent error response 
                    {
                        //log response status here..
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {                               
               
                //string fallbackPath = _configure.GetGeneralSettings().Get_FileUpload_Path + "\\GlobalError_Upgrade\\APIError.txt";
                //await System.IO.File.AppendAllTextAsync(fallbackPath, $"Error in GetDynamicPlateformApprovalCount catch block: {ex.Message}, Inner: {ex.InnerException}" + Environment.NewLine);


                _logger.LogError($"Error in GetDynamicPlateformApprovalCount catch block: {ex.Message}, Inner: {ex.InnerException}");
                return "";
            }
            }

        [HttpPost]
        public async Task<string> GetDynamicPlateformRequestCount()
        {
            try
            {
                string token = await GetDynamicPlateformAPIToken();
                if (token == "")
                    return "";

                //DataManagement odmt = new DataManagement();
                System.Data.DataTable dt = _odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "APITOKEN" + "'");
                System.Data.DataTable dt_ml = _odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "Test_ML101" + "'");

                string tmpl_url = "";
                if (dt.Rows.Count > 0)
                {
                    tmpl_url = dt.Rows[0]["tmpl_url"].ToString();
                }

                string tmpl_url_ML = "";
                if (dt.Rows.Count > 0)
                {
                    tmpl_url_ML = dt_ml.Rows[0]["tmpl_url"].ToString();
                }

                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | (SecurityProtocolType)12288;
                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    string APIUserName = _sessionService.Get<string>("userID");

                    string encryptionURL = "Flag=GetEntryDetails&UserId=" + APIUserName + "&Token=" + token;
                    encryptionURL = HttpUtility.UrlEncode(Encrypt(encryptionURL));

                    string APIurl = tmpl_url + "/api/GetEntryDetails?q=" + encryptionURL;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask = client.GetAsync(APIurl);
                    responseTask.Wait();
                    var result = responseTask.Result;

                    string APIurl_DCN = tmpl_url + "/DcnApi/GetEntryDetails?q=" + encryptionURL;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask_DCN = client.GetAsync(APIurl_DCN);
                    responseTask_DCN.Wait();
                    var result_DCN = responseTask_DCN.Result;

                    string APIurl_ML = tmpl_url_ML + "/MLApi/GetEntryDetails?q=" + encryptionURL;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask_ML = client.GetAsync(APIurl_ML);
                    responseTask_ML.Wait();
                    var result_ML = responseTask_ML.Result;

                    string APIurl_FF = tmpl_url + "/FFApi/GetEntryDetails?q=" + encryptionURL;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask_FF = client.GetAsync(APIurl_FF);
                    responseTask_FF.Wait();
                    var result_FF = responseTask_FF.Result;

                    if (result.IsSuccessStatusCode && result_DCN.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync().Result;
                        var resultContent = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask);
                        var readTask_DCN = result_DCN.Content.ReadAsStringAsync().Result;
                        var resultContent_DCN = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_DCN);
                        var readTask_ML = result_ML.Content.ReadAsStringAsync().Result;
                        var resultContent_ML = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_ML);
                        var readTask_FF = result_FF.Content.ReadAsStringAsync().Result;
                        var resultContent_FF = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_FF);
                        int counts = 0;
                        if (resultContent.tmplpendingcount.Count > 0)
                        {
                            foreach (var item in resultContent.tmplpendingcount)
                            {
                                if (item.tmplcode == "EL101" || item.tmplcode == "HSO101" || item.tmplcode == "HSO102" || item.tmplcode == "HLI101" || item.tmplcode == "CES101")
                                {
                                    continue;
                                }
                                else
                                {
                                    counts += item.count;
                                }
                            }
                        }
                        int count = counts + resultContent_DCN.pendingcount + resultContent_ML.pendingcount + resultContent_FF.pendingcount;
                        return count.ToString();
                    }
                    else //web api sent error response 
                    {
                        //log response status here..
                        return "";
                    }

                }
            }
            catch (Exception ex)
            {
                //System.IO.File.AppendAllText(Server.MapPath("~/Uploads") + "/APIError.txt", ex.Message.ToString());

                
                //string fallbackPath = _configure.GetGeneralSettings().Get_FileUpload_Path + "\\GlobalError_Upgrade\\APIError.txt";
                //await System.IO.File.AppendAllTextAsync(fallbackPath, $"Error in GetDynamicPlateformRequestCount catch block: {ex.Message}, Inner: {ex.InnerException}"+Environment.NewLine);

                //System.IO.File.AppendAllText(Path.Combine(_env.WebRootPath, "Uploads", "APIError.txt"), ex.Message.ToString());

                _logger.LogError($"Error in GetDynamicPlateformRequestCount catch block: {ex.Message}, Inner: {ex.InnerException}");
                return "";
            }
        }


        //[HttpPost]
        //public ActionResult GetDynamicPlateformApprovalList()
        //{
        //    string token = GetDynamicPlateformAPIToken();
        //    if (token == "")
        //        return null;

        //    DataManagement odmt = new DataManagement();
        //    System.Data.DataTable dt = odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "APITOKEN" + "'");
        //    System.Data.DataTable dt_ml = odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "Test_ML101" + "'");

        //    string tmpl_url = "";
        //    if (dt.Rows.Count > 0)
        //    {
        //        tmpl_url = dt.Rows[0]["tmpl_url"].ToString();
        //    }

        //    string tmpl_url_ML = "";
        //    if (dt.Rows.Count > 0)
        //    {
        //        tmpl_url_ML = dt_ml.Rows[0]["tmpl_url"].ToString();
        //    }

        //    using (var client = new HttpClient())
        //    {
        //        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | (SecurityProtocolType)12288;
        //        //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        //        string APIUserName = Convert.ToString(Session["userID"]); //HttpUtility.UrlEncode(Encrypt(Convert.ToString(Session["userID"])));

        //        string encryptionURL = "Flag=GetApprovalDetails&UserId=" + APIUserName + "&Token=" + token;
        //        encryptionURL = HttpUtility.UrlEncode(Encrypt(encryptionURL));


        //        string APIurl = tmpl_url + "/api/GetApprovalDetails?q=" + encryptionURL;
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var responseTask = client.GetAsync(APIurl);
        //        responseTask.Wait();
        //        var result = responseTask.Result;

        //        string APIurl_DCN = tmpl_url + "/DcnApi/GetApprovalDetails?q=" + encryptionURL;
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var responseTask_DCN = client.GetAsync(APIurl_DCN);
        //        responseTask_DCN.Wait();
        //        var result_DCN = responseTask_DCN.Result;

        //        string APIurl_FF = tmpl_url + "/FFApi/GetApprovalDetails?q=" + encryptionURL;
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var responseTask_FF = client.GetAsync(APIurl_FF);
        //        responseTask_FF.Wait();
        //        var result_FF = responseTask_FF.Result;

        //        string APIurl_ML = tmpl_url_ML + "/MLApi/GetApprovalDetails?q=" + encryptionURL;
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var responseTask_ML = client.GetAsync(APIurl_ML);
        //        responseTask_ML.Wait();
        //        var result_ML = responseTask_ML.Result;

        //        if (result.IsSuccessStatusCode && result_DCN.IsSuccessStatusCode && result_ML.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsStringAsync().Result;
        //            var resultContent = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask);
        //            var readTask_DCN = result_DCN.Content.ReadAsStringAsync().Result;
        //            var resultContent_DCN = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_DCN);
        //            var readTask_ML = result_ML.Content.ReadAsStringAsync().Result;
        //            var resultContent_ML = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_ML);

        //            var readTask_FF = result_FF.Content.ReadAsStringAsync().Result;
        //            var resultContent_FF = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_FF);

        //            //foreach (var obj in resultContent.tmplpendingcount)
        //            //{
        //            //    string URLUserName = HttpUtility.UrlEncode(Encrypt(Convert.ToString(Session["userID"])));
        //            //    string URLPassword = HttpUtility.UrlEncode(Convert.ToString(Session["pass"]));
        //            //    obj.url = string.Format(obj.url, URLUserName, URLPassword);
        //            //}
        //            resultContent.tmplpendingcount.AddRange(resultContent_DCN.tmplpendingcount);
        //            resultContent.tmplpendingcount.AddRange(resultContent_ML.tmplpendingcount);
        //            resultContent.tmplpendingcount.AddRange(resultContent_FF.tmplpendingcount);
        //            return Json(resultContent.tmplpendingcount.ToList());
        //        }
        //        else //web api sent error response 
        //        {
        //            //log response status here..
        //            return null;
        //        }
        //    }

        //}

        //[HttpPost]
        //public ActionResult GetDynamicPlateformRequestList()
        //{
        //    string token = GetDynamicPlateformAPIToken();
        //    if (token == "")
        //        return null;

        //    DataManagement odmt = new DataManagement();
        //    System.Data.DataTable dt = odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "APITOKEN" + "'");
        //    System.Data.DataTable dt_ml = odmt.GetDataTable("select t.tmpl_url from d_tmp_url_mapping t where t.tmpl_code='" + "Test_ML101" + "'");

        //    string tmpl_url = "";
        //    if (dt.Rows.Count > 0)
        //    {
        //        tmpl_url = dt.Rows[0]["tmpl_url"].ToString();
        //    }

        //    string tmpl_url_ML = "";
        //    if (dt.Rows.Count > 0)
        //    {
        //        tmpl_url_ML = dt_ml.Rows[0]["tmpl_url"].ToString();
        //    }

        //    using (var client = new HttpClient())
        //    {
        //        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | (SecurityProtocolType)12288;
        //        //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        //        string APIUserName = Convert.ToString(Session["userID"]); //HttpUtility.UrlEncode(Encrypt(Convert.ToString(Session["userID"])));

        //        string encryptionURL = "Flag=GetEntryDetails&UserId=" + APIUserName + "&Token=" + token;
        //        encryptionURL = HttpUtility.UrlEncode(Encrypt(encryptionURL));

        //        string APIurl = tmpl_url + "/api/GetEntryDetails?q=" + encryptionURL;
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var responseTask = client.GetAsync(APIurl);
        //        responseTask.Wait();
        //        var result = responseTask.Result;

        //        string APIurl_DCN = tmpl_url + "/DcnApi/GetEntryDetails?q=" + encryptionURL;
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var responseTask_DCN = client.GetAsync(APIurl_DCN);
        //        responseTask_DCN.Wait();
        //        var result_DCN = responseTask_DCN.Result;

        //        string APIurl_FF = tmpl_url + "/FFApi/GetEntryDetails?q=" + encryptionURL;
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var responseTask_FF = client.GetAsync(APIurl_FF);
        //        responseTask_FF.Wait();
        //        var result_FF = responseTask_FF.Result;

        //        string APIurl_ML = tmpl_url_ML + "/MLApi/GetEntryDetails?q=" + encryptionURL;
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var responseTask_ML = client.GetAsync(APIurl_ML);
        //        responseTask_ML.Wait();
        //        var result_ML = responseTask_ML.Result;

        //        if (result.IsSuccessStatusCode && result_DCN.IsSuccessStatusCode && result_ML.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsStringAsync().Result;
        //            var resultContent = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask);
        //            var readTask_DCN = result_DCN.Content.ReadAsStringAsync().Result;
        //            var resultContent_DCN = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_DCN);
        //            var readTask_ML = result_ML.Content.ReadAsStringAsync().Result;
        //            var resultContent_ML = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_ML);

        //            var readTask_FF = result_FF.Content.ReadAsStringAsync().Result;
        //            var resultContent_FF = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(readTask_FF);

        //            //foreach (var obj in resultContent.tmplpendingcount)
        //            //{
        //            //    string URLUserName = HttpUtility.UrlEncode(Encrypt(Convert.ToString(Session["userID"])));
        //            //    string URLPassword = HttpUtility.UrlEncode(Convert.ToString(Session["pass"]));
        //            //    obj.url = string.Format(obj.url, URLUserName, URLPassword);
        //            //}
        //            resultContent.tmplpendingcount.AddRange(resultContent_DCN.tmplpendingcount);
        //            resultContent.tmplpendingcount.AddRange(resultContent_ML.tmplpendingcount);
        //            resultContent.tmplpendingcount.AddRange(resultContent_FF.tmplpendingcount);
        //            return Json(resultContent.tmplpendingcount.ToList());
        //        }
        //        else //web api sent error response 
        //        {
        //            //log response status here..
        //            return null;
        //        }
        //    }

        //}

        //[HttpGet]
        //public string GetApprovalCount()
        //{
        //    return GetDynamicPlateformAPIToken();
        //}

    }

    public class DynamicToken
    {
        public string userID { get; set; }
        public string Token { get; set; }
    }

    public class Rootobject
    {
        public string userid { get; set; }
        public int pendingcount { get; set; }
        public List<Tmplpendingcount> tmplpendingcount { get; set; }
        public List<Listdetail> listdetails { get; set; }
    }

    public class Tmplpendingcount
    {
        public string tmplcode { get; set; }
        public string tmplname { get; set; }
        public int count { get; set; }
        public string url { get; set; }
    }

    public class Listdetail
    {
        public string tmplcode { get; set; }
        public string tmpldesc { get; set; }
        public string url { get; set; }
    }
}

