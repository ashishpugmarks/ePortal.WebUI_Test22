using ePortal.Application.Contracts;
using ePortal.ViewModels;
using ePortal.Web.Models;
using System.Collections.Specialized;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ePortal.Shared.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.Shared;
using ePortal.WebUI.Infra;
using ePortal.WebUI.Configuration;
using Microsoft.Extensions.Options;
using ePortal.Shared.Configuration;
using ePortal.WebUI.Filters;
using ePortal.WebUI.Models;
using System.IO;

namespace ePortal.WebUI.Controllers
{
    //[CSPFilter(formActionEndpoints:"")]
    //[CSPFilter(noOfNonces: 10)]
    [CSPFilter]
    public class LoginController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly GeneralSettings _settings;
        private readonly IEmpLoginService _loginService;
        private readonly ISessionService _sessionService;

        public LoginController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, IOptions<GeneralSettings> settings, IEmpLoginService loginService, ISessionService sessionService, IConfiguration configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _settings = settings.Value;
            _loginService = loginService;
            _sessionService = sessionService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _sessionService.Clear();
            _loginService.DeleteCookie(_configuration["GeneralSettings:CookieName"].ToString()); //Added By TTL against CR6695 as on 29 - 07 - 2025

            _logger.LogInformation("Home page visited at time {Time}", DateTime.UtcNow);

            List<ContentViewModel> newsList = new List<ContentViewModel>();

            LoginViewModel LoginModel = new LoginViewModel();
            List<SelectListItem> usertype = new List<SelectListItem>();
            usertype.Add(new SelectListItem { Text = "User", Value = "1" });
            usertype.Add(new SelectListItem { Text = "Admin", Value = "2" });
            LoginModel.usertype = usertype;
            LoginModel.ContentList = _loginService.GetContent(new ContentViewModel() { PROCESSID = 1, STATUS = 1 }).OrderBy(m => m.ATTACHMENTID).ToList();

            LoginModel.NewsListUpperSec = new List<ContentViewModel>();
            LoginModel.NewsListBottomSec = new List<ContentViewModel>();

            List<string> processids = _loginService.GetParameterValue("CORPORATENEWS_PRCID").Split(',').ToList();
            List<string> uppernewsid = new List<string>();
            foreach (var o in processids)
            {
                string str = o.Split('~')[0];
                if (str == "U")
                    LoginModel.NewsListUpperSec.AddRange(_loginService.GetContent(new ContentViewModel() { PROCESSID = Convert.ToInt32(o.Split('~')[1]), STATUS = 1 }));
                if (str == "L")
                    LoginModel.NewsListBottomSec.AddRange(_loginService.GetContent(new ContentViewModel() { PROCESSID = Convert.ToInt32(o.Split('~')[1]), STATUS = 1 }));

                ////comment below line
                //break;
            }

            //string Client_Ip = "";//System.Web.HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? System.Web.HttpContext.Current.Request.UserHostAddress;
            //if (System.Web.HttpContext.Current.Request.Url.DnsSafeHost == "localhost")
            //    Client_Ip = "10.249.16.17";
            //else
            //    Client_Ip = System.Web.HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? System.Web.HttpContext.Current.Request.UserHostAddress;

            string Client_Ip = GetClientIp();

            //    var httpContext = _httpContextAccessor.HttpContext;
            //if (httpContext == null) return null;

            //if (httpContext.Request.Host.Host == "localhost")
            //    return "10.249.16.17";

            //var ipFromHeader = httpContext.Request.Headers["HTTP_CLIENT_IP"].FirstOrDefault();
            //return ipFromHeader ?? httpContext.Connection.RemoteIpAddress?.ToString();


            String[] arrecode = _loginService.GetParameterValue("INTERNETVALIDATION").Split(',');
            ArrayList collecode = new ArrayList(arrecode);
            string striprange = Client_Ip.Substring(0, 6);
            if (!collecode.Contains(striprange))
            {
                LoginModel.APIaddress = "http://portal.honda2wheelersindia.com//api/media/play?f=";//Internet
            }
            else
            {
                LoginModel.APIaddress = _settings.APIAddress + " /api/media/play?f=";//Intranet
            }

            return View(LoginModel);
        }

        public ActionResult GetSearchResults(int id)
        {
            ContentViewModel newsList = new ContentViewModel();
            newsList = _loginService.GetVideo(new ContentViewModel() { ATTACHMENTID = id });
            //var base64 = Convert.ToBase64String(newsList.ATTACHMENT1);
            //var imgSrc = String.Format("data:" + newsList.ATTACHMENT1_CONTENTTYPE + ";base64,{0}", base64);
            return File(newsList.ATTACHMENT1, newsList.ATTACHMENT1_CONTENTTYPE, newsList.ATTACHMENT1_NAME);
        }

        public ActionResult GetAttachement2(int id)
        {
            ContentViewModel newsList = new ContentViewModel();
            newsList = _loginService.GetAttachement2(new ContentViewModel() { ATTACHMENTID = id });
            //var base64 = Convert.ToBase64String(newsList.ATTACHMENT1);
            //var imgSrc = String.Format("data:" + newsList.ATTACHMENT1_CONTENTTYPE + ";base64,{0}", base64);
            return File(newsList.ATTACHMENT2, newsList.ATTACHMENT2_CONTENTTYPE, newsList.ATTACHMENT2_NAME);
        }

        [HttpPost]
        public ActionResult checklogin(string uid, string pass, string Usertype)
        {
            string loginhomepage = "0";
            long userid = 0;
            try { userid = Convert.ToInt64(uid); }
            catch (Exception EX) { return Json("4"); }
            int lngResult = 0;
            string networkType = string.Empty;

            //string Client_Ip = "";//System.Web.HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? System.Web.HttpContext.Current.Request.UserHostAddress;            

            //if (System.Web.HttpContext.Current.Request.UrlReferrer.DnsSafeHost == "localhost")
            //    Client_Ip = "10.116.16.17";
            //else
            //    Client_Ip = System.Web.HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? System.Web.HttpContext.Current.Request.UserHostAddress;

            string Client_Ip = GetClientIp();

            //string 
            lngResult = _loginService.Check_EmpLogin(uid, pass, Client_Ip, Usertype);
            if (lngResult == 1)
            {
                String[] arrecode = _loginService.GetParameterValue("INTERNETVALIDATION").Split(',');
                ArrayList collecode = new ArrayList(arrecode);
                string striprange = Client_Ip.Substring(0, 6);
                if (!collecode.Contains(striprange))
                {
                    TempData["NetworkType"] = "1";//Internet
                    _sessionService.Set("NetworkType", 1);
                    networkType = "1";
                }
                else
                {
                    TempData["NetworkType"] = "0";//Intranet
                    _sessionService.Set("NetworkType", 0);
                    networkType = "0";
                }
                TempData.Keep("NetworkType");

                //Employee_Details obj = (Employee_Details)Session["Employee"];
                //Session["userID"] = obj._ECode;
                //Session["userName"] = obj._EName;
                //Session["usertype"] = Usertype;
                //Session["EMPLtype"] = obj._EmpLtype;
                //Session["ISSSOLOGIN"] = 0; //--90/30 days password policy
                //string strpasswordencode = Encryption.EncodePasswordToBase64(pass);
                //Session["pass"] = strpasswordencode;

                Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                _sessionService.Set("userID", obj._ECode);
                _sessionService.Set("userName", obj._EName);
                _sessionService.Set("usertype", Usertype);
                _sessionService.Set("EMPLtype", obj._EmpLtype ?? "");
                _sessionService.Set("ISSSOLOGIN", 0);  //--90/30 days password policy
                string strpasswordencode = Encryption.EncodePasswordToBase64(pass);
                _sessionService.Set("pass", strpasswordencode);

                //Generate Token
                //var token = _loginService.GenerateJwtToken(Convert.ToString(obj._ECode));
                //if (token != null)
                //{
                //    _sessionService.Set("JWToken", token);
                //}

                _sessionService.Set("IdleTimeoutMinutes", _settings.IdleTimeoutMinutes);


                //Added By TTL against CR6695 as on 29 - 07 - 2025

                CookieDetails cookieDetails = new CookieDetails();
                cookieDetails.UserID = obj._ECode;
                cookieDetails.UserName = obj._EName;
                cookieDetails.UserType = Usertype;
                cookieDetails.EmpType = obj._EmpLtype ?? "";
                cookieDetails.ISSSOLOGIN = 0;
                cookieDetails.EmpDetails = obj; //Added By TTL against CR6852 as on 31-07-2025 
                cookieDetails.NetworkType = networkType; //Added By TTL against CR6852 as on 31-07-2025 

                //add user in cookies
                _loginService.WriteCookie(_configuration["GeneralSettings:CookieName"].ToString(), cookieDetails, Convert.ToInt32(_configuration["GeneralSettings:CookieExpireInDays"]));


                ViewBag.UserName = obj._EName;
                String[] parm_value1 = _loginService.GetParameterValue("DESG_STAFFHOMEPAGE_VALIDATION").Split(',');
                ArrayList _arrayList1 = new ArrayList(parm_value1);
                String[] parm_value2 = _loginService.GetParameterValue("DESG_HOMEPAGE_VALIDATION").Split(',');
                ArrayList _arrayList2 = new ArrayList(parm_value2);

                //Changed by TTL on 09-Sep-2025 against SR107134 > CR7066
                String[] parm_value3 = _loginService.GetParameterValue("MFA_EXCEPTION").Split(',');
                ArrayList _arrayList3 = new ArrayList(parm_value3);

                if (_arrayList1.Contains(obj.Designation_Id)) //// obj != null && (obj._DesigId == (long)16 || obj._DesigId == (long)19 || obj._DesigId == (long)33) ---- 16 for Staff, 19 for Trainee, 33 for Line Associate
                {
                    if (networkType == "1")
                    {
                        if (!_arrayList3.Contains(obj.Designation_Id))
                        {
                            ViewBag.Message = "7";
                            loginhomepage = "2"; //Staff home page
                        }
                        else
                        {
                            ViewBag.Message = "6";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "6";
                    }
                    // ViewBag.Message = "6";
                }
                //Changed by TTL on 09-Sep-2025 against SR107134 > CR7066
                else if (_arrayList2.Contains(obj.Designation_Id))
                {
                    if (networkType == "1")
                    {
                        ViewBag.Message = "7";
                        loginhomepage = "1"; //JE& above home page
                    }
                    else
                    {
                        ViewBag.Message = "1";
                    }
                }
                else
                {
                    if (Usertype == "1")
                    {
                        ViewBag.Message = "0"; //"Invalid Login Id or Password.<br>Login Id and Password are case sensitive. Please try again.";
                    }
                    else
                    {
                        if (networkType == "1")
                        {
                            ViewBag.Message = "7";
                            loginhomepage = "1"; //JE& above home page
                        }
                        else
                        {
                            ViewBag.Message = "1";
                        }
                    }
                }
            }
            if (lngResult == 0)
            {
                ViewBag.Message = "0"; //"Invalid Login Id or Password.<br>Login Id and Password are case sensitive. Please try again.";
            }
            if (lngResult == 3)
            {
                ViewBag.Message = "3"; //"Your login Id has been blocked,due to 10 wrong password attempts.Kindly use 'Forgot password' link to change your password."
            }
            if (lngResult == 5)
            {
                ViewBag.Message = "5"; //"Your login Id has been blocked,due to 10 wrong password attempts.Kindly use 'Forgot password' link to change your password."
            }

            //Session["HOMEPAGETYPE"] = loginhomepage;
            _sessionService.Set("HOMEPAGETYPE", loginhomepage);
            return Json(ViewBag.Message);
        }

        [HttpPost]
        public ActionResult GetPopDetail(int id)
        {
            ContentViewModel newsList = new ContentViewModel();
            newsList = _loginService.GetPopupContent(new ContentViewModel() { ATTACHMENTID = id }).FirstOrDefault();
            return Json(newsList);
        }

        //public ActionResult ForgetPassword()
        //{
        //    Response.Redirect("~/aspxview/ForgotPassword.aspx",false);
        //    HttpContext.ApplicationInstance.CompleteRequest();
        //    return View();
        //}

        [HttpPost]
        public ActionResult ForgotPassword(string uid, string dob, string Usertype)
        {

            Employee_Details dtchk = _loginService.GetEmpDetails(uid);
            if (dtchk == null)
            {
                ViewBag.Message = "Please enter correct Employee Code and D.O.B.";
                return Json(ViewBag.Message);
            }
            else
            {
                //if (dtchk.Department_Id == _loginService.GetParameterValue("ASR_DEPTID") && dtchk.Designation_Id != _loginService.GetParameterValue("DESIGNATION_STAFF"))
                //{
                //    ViewBag.Message = "A&SR department can only reset the staff password";
                //    return Json(ViewBag.Message);
                //}
                //if (dtchk.Department_Id != _loginService.GetParameterValue("ASR_DEPTID") && dtchk.Designation_Id == _loginService.GetParameterValue("DESIGNATION_STAFF"))
                //{
                //    ViewBag.Message = "Please enter correct Employee Code and D.O.B.";
                //    return Json(ViewBag.Message);
                //}
                if (dtchk.Designation_Id == _loginService.GetParameterValue("DESIGNATION_STAFF"))
                {
                    ViewBag.Message = "Please enter correct Employee Code and D.O.B.";
                    return Json(ViewBag.Message);
                }
                if ((DateTime.ParseExact(dtchk.DOB, "dd-MMM-yyyy", null)) != (DateTime.ParseExact(dob, "dd-MMM-yyyy", null)))
                {
                    ViewBag.Message = "Please enter correct Employee Code and D.O.B.";
                    return Json(ViewBag.Message);
                }
                else
                {
                    EncryptionMVC objenc = new EncryptionMVC();
                    string DESIGNATIONID = dtchk.Designation_Id;
                    string strPassword = objenc.Randompassword(DESIGNATIONID);
                    string strencrypassword = objenc.EncodePasswordToBase64(strPassword);
                    string strStatus = _loginService.UpdateEmployeePassword(Convert.ToInt64(uid), strencrypassword, Convert.ToInt64(uid), Usertype);
                    string[] passwordStatus = strStatus.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(passwordStatus[0]);
                    string errMsg = Convert.ToString(passwordStatus[1]);
                    string strPhoneNo = Convert.ToString(passwordStatus[2]);
                    string strEmpname = Convert.ToString(passwordStatus[3]);

                    if (errResult == "1")
                    {
                        string strSubject, strBody;
                        string strEmailID = errMsg;
                        EmailCore sendMail = new EmailCore();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (!string.IsNullOrEmpty(strEmailID) || !string.IsNullOrEmpty(strPhoneNo))
                        {
                            sendMail.MailTo = strEmailID;
                            strSubject = "Password Reset Request for Employee Portal";
                            strBody = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Password Reset </b></div>"
                               + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                               + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + strEmpname + " San,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                               + "<tr><td colspan=2>&nbsp;&nbsp;You’ve successfully changed your Eportal Password.<br/><br/></td></tr><tr>"
                               + "<tr><td colspan=2>&nbsp;&nbsp;Your new password is   " + strPassword + "<br/><br/>&nbsp;&nbsp;Kindly change your password after next Login. </td></tr></table>";

                            strBody = strBody +
                            "<table cellpadding=0 cellspacing=0 border=0 >" +
                            "<tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Portal<br/><br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                            "</tr></table></td></tr><tr><td></td></tr><tr> " +
                            "</tr></table></div> ";
                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool statusUpdate = false;
                                short retStatus = 0;
                                if (!string.IsNullOrEmpty(strEmailID))
                                {
                                    statusUpdate = sendMail.Send();
                                }
                                //if (!string.IsNullOrEmpty(strPhoneNo))
                                //{
                                //    retStatus = SendSMS(strPassword, strPhoneNo, strEmpname);
                                //}

                                //Changed By TTL against CR6695 as on 29 - 07 - 2025
                                if (statusUpdate || retStatus == 1)
                                {
                                    ViewBag.Message = "Your password has been reset successfully. As a security precaution, changed password has been sent to your email id.";
                                    return Json(ViewBag.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error in Forgot password: user-" + uid + " Error:" + ex.Message.ToString());
                                ViewBag.Message = ex.ToString();
                                // return Json("Something went wrong.Kindly contact to administrator.");
                                return Json(ViewBag.Message);
                            }
                        }
                        else
                        {
                            _logger.LogError("Please contact to SIS department for new Password.user-" + uid);
                            ViewBag.Message = "Please contact to SIS department for new Password</b>";
                            return Json(ViewBag.Message);
                        }
                    }
                    else
                    {
                        _logger.LogError("Please contact to SIS department for new Password.user-" + uid);
                        return Json("Something went wrong.Kindly contact to administrator11.");
                    }
                }
            }

            return Json(ViewBag.Message);
        }

        /// <summary>
        /// API URL :- http://smspanel.quickstartechnology.com/API/pushsms.aspx?loginID=T1hondasms120&password=647467&mobile=8800443409&text=hi&senderid=HMSIIT&route_id=2&Unicode=0
        /// </summary>
        public short SendSMS(string strPassword, string strPhoneNo, string stremp)
        {
            short retVal = 0;
            try
            {
                if (!string.IsNullOrEmpty(strPhoneNo))
                {
                    string _loginId = string.Empty;
                    string _password = string.Empty;
                    string _mobile = string.Empty;
                    string _text = string.Empty;
                    string _senderid = string.Empty;
                    string _route_id = string.Empty;
                    string _Unicode = string.Empty;
                    //string s = ConfigurationManager.AppSettings["SAP_Connection_Path"].ToString();
                    string s = _settings.SAP_Connection_Path.ToString();
                    string File_Name = s + "/SmsApiParameters.txt";
                    StreamReader sr = System.IO.File.OpenText(File_Name);
                    string input = sr.ReadLine();
                    if (input != null)
                    {
                        string[] str = input.Split(';');
                        _loginId = str[0].Trim();
                        _password = str[1].Trim();
                        _mobile = strPhoneNo; //// str[2].Trim(); // set user phone no 
                        _text = str[3].Trim() + " " + strPassword;
                        _senderid = str[4].Trim();
                        _route_id = str[5].Trim();
                        _Unicode = str[6].Trim();
                    }
                    else
                    {
                        return retVal;
                    }
                    sr.Close();

                    string testmsg = "Dear " + stremp + " san,\n" + _text + ".\nKindly change your password after next Login.\n\nBest Regards,\nTeam Portal";
                    var url = "http://104.245.39.160/API/pushsms.aspx";
                    var client = new WebClient();
                    var method = "POST";
                    var parameters = new NameValueCollection();
                    parameters.Add("loginID", _loginId);
                    parameters.Add("password", _password);
                    parameters.Add("mobile", _mobile);
                    parameters.Add("text", testmsg);
                    parameters.Add("senderid", _senderid);
                    parameters.Add("route_id", _route_id);
                    parameters.Add("Unicode", _Unicode);
                    client.QueryString = parameters;
                    byte[] response_data = client.UploadValues(url, method, parameters);
                    string responsebody = Encoding.UTF8.GetString(response_data);
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public short OTPSendSMS(string strOTP, string strPhoneNo, string stremp)
        {
            short retVal = 0;
            try
            {
                if (!string.IsNullOrEmpty(strPhoneNo))
                {
                    string msg = strOTP + " is One-Time password for Eportal Login. Never share this password. Valid for 5 minutes. Honda 2W ";
                    string mobileNo = strPhoneNo;
                    //string url = "http://smscounter.com/api/url_api.php?api_key=8LkKvnVlmHg1rC95&pass=b2knFxco7D&senderid=HMSIIT&template_id=1507167032325326970&message=" + msg + "&dest_mobileno=" + mobileNo + "&mtype=TXT"; //SMS not coming with DMD
                    string url = "http://smscounter.com/api/url_api.php?api_key=8LkKvnVlmHg1rC95&pass=b2knFxco7D&senderid=HMSIIT&template_id=1707167541664337686&message=" + msg + "&dest_mobileno=" + mobileNo + "&mtype=TXT";  //SMS coming with DND

                    string StrSMSSentID = "";
                    WebClient ObjWebClient = new WebClient();

                    StrSMSSentID = ObjWebClient.DownloadString(url);
                    ObjWebClient.Dispose();

                    retVal = 1;
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }


        //[HttpPost]
        ////public ActionResult Contactus(string SContactusECode, string SContactusName, string SContactusEmailid, string SContactusmobile, string Scontactussugfor, string SContactusubject, string SContactusSugg)
        //public ActionResult Contactus()
        //{
        //    string SContactusECode = Request.Form["SContactusECode"];
        //    string SContactusName = Request.Form["SContactusName"];
        //    string SContactusEmailid = Request.Form["SContactusEmailid"];
        //    string SContactusmobile = Request.Form["SContactusmobile"];
        //    string Scontactussugfor = Request.Form["Scontactussugfor"];
        //    string SContactusubject = Request.Form["SContactusubject"];
        //    string SContactusSugg = Request.Form["SContactusSugg"];

        //    bool blnStatus;

        //    if (SContactusECode == "" || SContactusName == "" || SContactusEmailid == "" || SContactusmobile == "" || Scontactussugfor == "0" || SContactusubject == "" || SContactusSugg == "")
        //    {
        //        ViewBag.Message = "0#All fields are mandatory.";
        //        return Json(ViewBag.Message);
        //    }
        //    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        //    Match match = regex.Match(SContactusEmailid);
        //    if (!match.Success)
        //    {
        //        ViewBag.Message = "0#Invalid Emailid.Please check.";
        //        return Json(ViewBag.Message);
        //    }

        //    HttpPostedFileBase file; //Uploaded file
        //    int fileSize;
        //    string fileName = "";
        //    string mimeType = "";
        //    System.IO.Stream fileContent = null;
        //    if (HttpContext.Request.Files.Count > 0)
        //    {
        //        file = HttpContext.Request.Files["UploadedImage"];
        //        fileSize = file.ContentLength;
        //        fileName = file.FileName;
        //        mimeType = file.ContentType;
        //        fileContent = file.InputStream;
        //    }

        //    EmailCore objEmail = new EmailCore();
        //    string strBody, strToEmail;
        //    strBody = string.Empty;
        //    strBody = "Dear San,<BR>" + SContactusSugg + "<br><br> With regards,<Br>" + SContactusName + " [" + SContactusECode + "] <br>Department: " + Scontactussugfor;
        //    if (SContactusmobile != "")
        //    {
        //        strBody = strBody + "<br>Contact No: " + SContactusmobile + "<br>";
        //    }

        //    strToEmail = string.Empty;
        //    if (Scontactussugfor == "HR")
        //        strToEmail = _loginService.GetParameterValue("HR_CONTACTUS_MAILID");
        //    else if (Scontactussugfor == "A&SR")
        //        strToEmail = _loginService.GetParameterValue("ASR_CONTACTUS_MAILID");
        //    else if (Scontactussugfor == "Admin")
        //        strToEmail = _loginService.GetParameterValue("ADMIN_CONTACTUS_MAILID");
        //    else if (Scontactussugfor == "SIS")
        //        strToEmail = _loginService.GetParameterValue("SIS_CONTACTUS_MAILID");

        //    objEmail = new EmailCore();
        //    objEmail.MailFrom = "portal.admin@honda.hmsi.in";
        //    //if (SContactusEmailid != "")
        //    //{
        //    //    objEmail.MailFrom = SContactusEmailid;
        //    //}
        //    //else
        //    //{
        //    //    objEmail.MailFrom = "dummy@dummy.com";
        //    //}
        //    objEmail.MailTo = strToEmail;
        //    objEmail.MailCc = SContactusEmailid;
        //    objEmail.MailSubject = SContactusubject;

        //    if (!string.IsNullOrEmpty(fileName))
        //    {
        //        objEmail.attachementName = fileName;
        //        objEmail.attContentType = mimeType;
        //        objEmail.Mailattachement = fileContent;
        //    }
        //    objEmail.MailBody = strBody + "<br><br>";
        //    try
        //    {
        //        blnStatus = objEmail.Send();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = "1#Something went wrong.Kindly contact to administrator.";
        //    }
        //    finally
        //    {
        //        ViewBag.Message = "1#Your E-Mail has been sent sucessfully";
        //    }

        //    return Json(ViewBag.Message);
        //}


        [HttpPost]
        //public ActionResult Contactus(string SContactusECode, string SContactusName, string SContactusEmailid, string SContactusmobile, string Scontactussugfor, string SContactusubject, string SContactusSugg)
        public async Task<ActionResult> Contactus(IFormFile UploadedImage)
        {
            string SContactusECode = Request.Form["SContactusECode"];
            string SContactusName = Request.Form["SContactusName"];
            string SContactusEmailid = Request.Form["SContactusEmailid"];
            string SContactusmobile = Request.Form["SContactusmobile"];
            string Scontactussugfor = Request.Form["Scontactussugfor"];
            string SContactusubject = Request.Form["SContactusubject"];
            string SContactusSugg = Request.Form["SContactusSugg"];

            bool blnStatus;

            if (SContactusECode == "" || SContactusName == "" || SContactusEmailid == "" || SContactusmobile == "" || Scontactussugfor == "0" || SContactusubject == "" || SContactusSugg == "")
            {
                ViewBag.Message = "0#All fields are mandatory.";
                return Json(ViewBag.Message);
            }
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(SContactusEmailid);
            if (!match.Success)
            {
                ViewBag.Message = "0#Invalid Emailid.Please check.";
                return Json(ViewBag.Message);
            }

            IFormFile file; //Uploaded file
            //int fileSize;
            string fileName = "";
            string mimeType = "";
            System.IO.Stream fileContent = null;




            if (UploadedImage != null && UploadedImage.Length > 0)
            {
                fileName = Path.GetFileName(UploadedImage.FileName);
                mimeType = UploadedImage.ContentType;

                var memoryStream = new MemoryStream();
                await UploadedImage.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset position before reading
                fileContent = memoryStream;

            }


            //if (HttpContext.Request.Files.Count > 0)
            //{
            //    file = HttpContext.Request.Files["UploadedImage"];
            //    fileSize = file.ContentLength;
            //    fileName = file.FileName;
            //    mimeType = file.ContentType;
            //    fileContent = file.InputStream;
            //}

            EmailCore objEmail = new EmailCore();
            string strBody, strToEmail;
            strBody = string.Empty;
            strBody = "Dear San,<BR>" + SContactusSugg + "<br><br> With regards,<Br>" + SContactusName + " [" + SContactusECode + "] <br>Department: " + Scontactussugfor;
            if (SContactusmobile != "")
            {
                strBody = strBody + "<br>Contact No: " + SContactusmobile + "<br>";
            }

            strToEmail = string.Empty;
            if (Scontactussugfor == "HR")
                strToEmail = _loginService.GetParameterValue("HR_CONTACTUS_MAILID");
            else if (Scontactussugfor == "A&SR")
                strToEmail = _loginService.GetParameterValue("ASR_CONTACTUS_MAILID");
            else if (Scontactussugfor == "Admin")
                strToEmail = _loginService.GetParameterValue("ADMIN_CONTACTUS_MAILID");
            else if (Scontactussugfor == "SIS")
                strToEmail = _loginService.GetParameterValue("SIS_CONTACTUS_MAILID");

            objEmail = new EmailCore();
            objEmail.MailFrom = "portal.admin@honda.hmsi.in";
            //if (SContactusEmailid != "")
            //{
            //    objEmail.MailFrom = SContactusEmailid;
            //}
            //else
            //{
            //    objEmail.MailFrom = "dummy@dummy.com";
            //}
            objEmail.MailTo = strToEmail;
            objEmail.MailCc = SContactusEmailid;
            objEmail.MailSubject = SContactusubject;

            if (!string.IsNullOrEmpty(fileName))
            {
                objEmail.attachementName = fileName;
                objEmail.attContentType = mimeType;
                objEmail.Mailattachement = fileContent;
            }
            objEmail.MailBody = strBody + "<br><br>";
            try
            {
                blnStatus = objEmail.Send();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "1#Something went wrong.Kindly contact to administrator.";
            }
            finally
            {
                ViewBag.Message = "1#Your E-Mail has been sent sucessfully";
            }

            return Json(ViewBag.Message);
        }

        [HttpPost]
        public ActionResult AutoLogin()
        {
            var keys = Request.Form.Keys.ToList();

            string _wId = Request.Form[keys[0]];
            string _tId = Request.Form[keys[1]];

            if (!string.IsNullOrEmpty(_wId) && !string.IsNullOrEmpty(_tId))
            {
                string strResult = _loginService.Check_AutoLogin(_wId, _tId, "1");
                string[] strArray = strResult.Split('#');
                if (strArray[1] == "1")
                {
                    String[] _arrecode = _loginService.GetParameterValue("INTERNETVALIDATION").Split(',');
                    ArrayList _collecode = new ArrayList(_arrecode);

                    //string _Client_Ip = "";//System.Web.HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? System.Web.HttpContext.Current.Request.UserHostAddress;
                    //if (System.Web.HttpContext.Current.Request.UrlReferrer.DnsSafeHost == "localhost")
                    //    _Client_Ip = "10.249.16.17";
                    //else
                    //    _Client_Ip = System.Web.HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? System.Web.HttpContext.Current.Request.UserHostAddress;


                    string _Client_Ip = GetClientIp();



                    string _striprange = _Client_Ip.Substring(0, 6);
                    if (!_collecode.Contains(_striprange))
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
                    //TempData["NetworkType"] = "0";//Intranet

                    Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                    _sessionService.Set("userID", obj._ECode);
                    _sessionService.Set("userName", obj._EName);
                    _sessionService.Set("usertype", "1");
                    _sessionService.Set("EMPLtype", obj._EmpLtype);
                    _sessionService.Set("ISSSOLOGIN", 1);  //--90/30 days password policy
                    ViewBag.UserName = obj._EName;


                    //Added By TTL against CR6695 as on 29 - 07 - 2025
                    CookieDetails cookieDetails = new CookieDetails();
                    cookieDetails.UserID = obj._ECode;
                    cookieDetails.UserName = obj._EName;
                    cookieDetails.UserType = "1";
                    cookieDetails.EmpType = obj._EmpLtype;
                    cookieDetails.ISSSOLOGIN = 1;
                    cookieDetails.EmpDetails = obj; //Added By TTL against CR6852 as on 31-07-2025  
                    cookieDetails.NetworkType = _sessionService.Get<string>("NetworkType"); //Added By TTL against CR6852 as on 31-07-2025 

                    //add user in cookies
                    _loginService.WriteCookie(_configuration["GeneralSettings:CookieName"].ToString(), cookieDetails, Convert.ToInt32(_configuration["GeneralSettings:CookieExpireInDays"]));


                    //Employee_Details obj = (Employee_Details)Session["Employee"];
                    //Session["userID"] = obj._ECode;
                    //Session["userName"] = obj._EName;
                    //Session["usertype"] = "1";//TempData["NetworkType"].ToString();
                    //Session["EMPLtype"] = obj._EmpLtype;
                    //Session["ISSSOLOGIN"] = 1; //--90/30 days password policy
                    //ViewBag.UserName = obj._EName;

                    return RedirectToAction("Home", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult EmailApproval(string wid, string C, string A, string Tid)
        {
            //Guest meal booking change
            try
            {
                //Session["WID"] = wid.ToString();
                _sessionService.Set("WID", wid.ToString());
                string strval = _loginService.MailApprovalInsert(wid.ToString(), C, A, Tid.Trim().ToString());

                //if (Session["WID"] != null)
                //{
                //    if (Session["userID"] != null)
                //    {
                //        return RedirectToAction("Home", "Home");
                //    }

                //    // return RedirectToAction("Home", "Home");
                //    Response.Redirect("~/SSO");
                //    return View();
                //}
                //else
                //{
                //    return RedirectToAction("Index");
                //}

                if (_sessionService.Get<string>("WID") != null)
                {
                    if (_sessionService.Get<string>("userID") != null)
                    {
                        return RedirectToAction("Home", "Home");
                    }

                    // return RedirectToAction("Home", "Home");
                    Response.Redirect("~/SSO");
                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("~/SSO");
                return View();
            }
        }
        //// OTP Authentication
        [HttpGet]
        public ActionResult OTPAuthentication()
        {
            try
            {
                //if (Session["USERID"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //Session["OTPUSERID"] = Session["USERID"].ToString();
                //if (Session["OTPUSERID"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //Session["USERID"] = null;

                var userId = _sessionService.Get<string>("USERID");

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Index", "Login");
                }

                var otpUserId = userId;
                if (otpUserId != null)
                {
                    _sessionService.Set("OTPUSERID", otpUserId);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                _sessionService.Remove("USERID");

                string strTime = _loginService.GetParameterValue("OTPMINS");
                ViewBag.OTPMIN = strTime.ToString(); ;

                ViewBag.msg = "To verify you are human, please enter below captcha";
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult OTPAuthentication(string OTP)
        {
            short retVal = 0;
            //if (Session["TOKENOTP"].ToString() == OTP)
            //{
            //    Session["USERID"] = Session["OTPUSERID"].ToString();
            //    if (Session["HOMEPAGETYPE"].ToString() == "1")
            //    {
            //        if (Session["usertype"].ToString() == "1")
            //            retVal = 1;
            //        else
            //            retVal = 3;
            //        Session["HOMEPAGETYPE"] = null;

            //    }
            //    else if (Session["HOMEPAGETYPE"].ToString() == "2")
            //    {
            //        if (Session["usertype"].ToString() == "1")
            //            retVal = 2;
            //        else
            //            retVal = 3;
            //        Session["HOMEPAGETYPE"] = null;

            //    }
            //    else
            //    {
            //        retVal = -1;
            //    }

            //}
            //else
            //{
            //    retVal = -1; //// Entered OTP is incorrect, Please retry.
            //}

            var tokenOtp = _sessionService.Get<string>("TOKENOTP");

            if (tokenOtp == OTP)
            {
                var otpUserId = _sessionService.Get<string>("OTPUSERID");

                if (string.IsNullOrEmpty(otpUserId))
                {
                    _sessionService.Set("USERID", otpUserId);
                }

                if (_sessionService.Get<string>("HOMEPAGETYPE") == "1")
                {
                    if (_sessionService.Get<string>("usertype") == "1")
                        retVal = 1;
                    else
                        retVal = 3;

                    _sessionService.Remove("HOMEPAGETYPE");
                }
                else if (_sessionService.Get<string>("HOMEPAGETYPE") == "2")
                {
                    if (_sessionService.Get<string>("usertype") == "1")
                        retVal = 2;
                    else
                        retVal = 3;

                    _sessionService.Remove("HOMEPAGETYPE");
                }
                else
                {
                    retVal = -1;
                }
            }
            else
            {
                retVal = -1; //// Entered OTP is incorrect, Please retry.
            }

            string strTime = _loginService.GetParameterValue("OTPMINS");
            ViewBag.OTPMIN = strTime.ToString(); ;
            //  ViewBag.OTPMIN = strTime.ToString();"
            ViewBag.msg = "A verification code has been send to your Mobile/email.<br />code will be vaild for " + strTime + " minutes.";

            return Json(retVal);
        }

        [HttpPost]
        public ActionResult CaptchaAuthentication(string captcha)
        {
            short retVal = 0;
            //if (Session["CAPTCHA"].ToString() == captcha)
            //{
            //    Employee_Details dtchk = _loginService.GetEmpDetails(Session["OTPUSERID"].ToString());

            //    if (!string.IsNullOrEmpty(dtchk.MobileNo))
            //    {

            //        retVal = 1;
            //        OTPTOKEN(Session["OTPUSERID"].ToString(), Session["usertype"].ToString());
            //        string strTime = _loginService.GetParameterValue("OTPMINS");
            //        ViewBag.OTPMIN = strTime.ToString(); ;
            //        //  ViewBag.OTPMIN = strTime.ToString();"
            //        ViewBag.msg = "A verification code has been send to your Mobile/email.<br />code will be vaild for " + strTime + " minutes.";

            //    }
            //    else
            //    {
            //        retVal = 3;

            //    }
            //}
            //else
            //{
            //    retVal = 2; //// Entered OTP is incorrect, Please retry.
            //}


            if (_sessionService.Get<string>("CAPTCHA") == captcha)
            {
                Employee_Details dtchk = _loginService.GetEmpDetails(_sessionService.Get<string>("OTPUSERID"));

                if (!string.IsNullOrEmpty(dtchk.MobileNo))
                {

                    retVal = 1;
                    OTPTOKEN(_sessionService.Get<string>("OTPUSERID"), _sessionService.Get<string>("usertype"));
                    string strTime = _loginService.GetParameterValue("OTPMINS");
                    ViewBag.OTPMIN = strTime.ToString(); ;
                    //  ViewBag.OTPMIN = strTime.ToString();"
                    ViewBag.msg = "A verification code has been send to your Mobile/email.<br />code will be vaild for " + strTime + " minutes.";

                }
                else
                {
                    retVal = 3;

                }
            }
            else
            {
                retVal = 2; //// Entered OTP is incorrect, Please retry.
            }


            return Json(retVal);
        }

        [HttpPost]
        public ActionResult OTPTOKEN(string uid, string Usertype)
        {

            Employee_Details dtchk = _loginService.GetEmpDetails(uid);
            if (dtchk == null)
            {
                ViewBag.Message = "Please enter correct Employee Code";
                return Json(ViewBag.Message);
            }
            else
            {

                if (!string.IsNullOrEmpty(dtchk.MobileNo.ToString()))
                {
                    Random generator = new Random();
                    string TOKEN_CODE = generator.Next(0, 1000000).ToString("D6");
                    //Session["TOKENOTP"] = TOKEN_CODE;
                    _sessionService.Set("TOKENOTP", TOKEN_CODE);

                    TempData["TOKENOTP"] = "0";//Intranet
                    TempData.Keep("NetworkType");
                    string strPhoneNo = dtchk.MobileNo;
                    string strEmpname = dtchk.Employee_Name.ToString();
                    string strEmailID = dtchk.EMail_Id;

                    string strSubject, strBody;
                    EmailCore sendMail = new EmailCore();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    if (!string.IsNullOrEmpty(strEmailID) || !string.IsNullOrEmpty(strPhoneNo))
                    {
                        sendMail.MailTo = strEmailID;
                        strSubject = "OTP to Verify Eportal login";
                        strBody = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>OTP to Verify Eportal login </b></div>"
                           + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                           + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + strEmpname + " San,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                           + "<tr><td colspan=2>&nbsp;&nbsp;" + TOKEN_CODE + " is One-Time password for Eportal Login. Never share this password. Valid for 5 minutes.<br/><br/></td></tr><tr>"
                           + "<tr><td colspan=2>&nbsp;&nbsp;To safegaurd yourself from unauthorized access, kindly do not share this OTP with anyone.<br/><br/>&nbsp;&nbsp; </td></tr></table>";

                        strBody = strBody +
                        "<table cellpadding=0 cellspacing=0 border=0 >" +
                        "<tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Portal<br/><br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                        "</tr></table></td></tr><tr><td></td></tr><tr> " +
                        "</tr></table></div> ";
                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool statusUpdate = false;
                            short retStatus = 0;
                            if (!string.IsNullOrEmpty(strEmailID))
                            {
                                statusUpdate = sendMail.Send();
                            }
                            if (!string.IsNullOrEmpty(strPhoneNo))
                            {
                                retStatus = OTPSendSMS(TOKEN_CODE, strPhoneNo, strEmpname);
                            }
                            if (statusUpdate || retStatus == 1)
                            {
                                return Json(ViewBag.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = ex.ToString();
                            // return Json("Something went wrong.Kindly contact to administrator.");
                            return Json(ViewBag.Message);
                        }
                    }
                    else
                    {
                        return Json(ViewBag.Message);
                    }


                    //try
                    //       {
                    //    short retStatus = 0;
                    //     if (!string.IsNullOrEmpty(strPhoneNo))
                    //    {
                    //        retStatus = OTPSendSMS(TOKEN_CODE, strPhoneNo, strEmpname);
                    //    }
                    //    if ( retStatus == 1)
                    //    {
                    //        return Json(ViewBag.Message);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    ViewBag.Message = ex.ToString();
                    //    // return Json("Something went wrong.Kindly contact to administrator.");
                    //    return Json(ViewBag.Message);
                    //}
                }
                else
                {
                    ViewBag.Message = "Please update Mobile no for OTP login";
                    return Json(ViewBag.Message);
                }

            }

            return Json(ViewBag.Message);
        }

        private string GetRandomText()
        {
            StringBuilder randomText = new StringBuilder();
            string alphabets = "012345679ACEFGHKLMNPRSWXZabcdefghijkhlmnopqrstuvwxyz";
            Random r = new Random();
            for (int j = 0; j < 5; j++)
            {
                randomText.Append(alphabets[r.Next(alphabets.Length)]);
            }
            return randomText.ToString();
        }

        public ActionResult REOTPTOKEN(string uid)
        {

            Employee_Details dtchk = _loginService.GetEmpDetails(uid);
            if (dtchk == null)
            {
                ViewBag.Message = "Please enter correct Employee Code.";
                return Json(ViewBag.Message);
            }
            else
            {
                if (!string.IsNullOrEmpty(dtchk.MobileNo))
                {
                    Random generator = new Random();
                    string TOKEN_CODE = generator.Next(0, 1000000).ToString("D6");

                    //Session["TOKENOTP"] = TOKEN_CODE;
                    _sessionService.Set("TOKENOTP", TOKEN_CODE);
                    string strPhoneNo = dtchk.MobileNo;
                    string strEmpname = dtchk.Employee_Name.ToString();
                    string strEmailID = dtchk.EMail_Id;

                    string strSubject, strBody;
                    EmailCore sendMail = new EmailCore();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    if (!string.IsNullOrEmpty(strEmailID) || !string.IsNullOrEmpty(strPhoneNo))
                    {
                        sendMail.MailTo = strEmailID;
                        strSubject = "OTP to Verify Eportal login";
                        strBody = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>OTP to Verify Eportal login </b></div>"
                           + "<table cellpadding=0 cellspacing=0 border=0 width=700px>"
                           + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + strEmpname + " San,</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                           + "<tr><td colspan=2>&nbsp;&nbsp;" + TOKEN_CODE + " is One-Time password for Eportal Login. Never share this password. Valid for 5 minutes.<br/><br/></td></tr><tr>"
                           + "<tr><td colspan=2>&nbsp;&nbsp;To safegaurd yourself from unauthorized access, kindly do not share this OTP with anyone.<br/><br/>&nbsp;&nbsp; </td></tr></table>";

                        strBody = strBody +
                        "<table cellpadding=0 cellspacing=0 border=0 >" +
                        "<tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Portal<br/><br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                        "</tr></table></td></tr><tr><td></td></tr><tr> " +
                        "</tr></table></div> ";
                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool statusUpdate = false;
                            short retStatus = 0;
                            if (!string.IsNullOrEmpty(strEmailID))
                            {
                                statusUpdate = sendMail.Send();
                            }
                            if (!string.IsNullOrEmpty(strPhoneNo))
                            {
                                retStatus = OTPSendSMS(TOKEN_CODE, strPhoneNo, strEmpname);
                            }
                            if (statusUpdate || retStatus == 1)
                            {
                                return Json(ViewBag.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = ex.ToString();
                            // return Json("Something went wrong.Kindly contact to administrator.");
                            return Json(ViewBag.Message);
                        }
                    }
                    else
                    {
                        return Json(ViewBag.Message);
                    }
                    //try
                    //{
                    //    short retStatus = 0;
                    //    if (!string.IsNullOrEmpty(strPhoneNo))
                    //    {
                    //        retStatus = OTPSendSMS(TOKEN_CODE, strPhoneNo, strEmpname);
                    //    }
                    //    if (retStatus == 1)
                    //    {
                    //        return Json(ViewBag.Message);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    ViewBag.Message = ex.ToString();
                    //    // return Json("Something went wrong.Kindly contact to administrator.");
                    //    return Json(ViewBag.Message);
                    //}

                }
                return Json(ViewBag.Message);
            }
        }

        public ActionResult GetCaptchaImage()
        {
            try
            {
                //Session["CAPTCHA"] = GetRandomText();
                _sessionService.Set("CAPTCHA", GetRandomText());
                //if (Session["CAPTCHA"] == null)
                var captchaText = _sessionService.Get<string>("CAPTCHA");
                if (string.IsNullOrEmpty(captchaText))
                {
                    return RedirectToAction("Index", "Login");
                }

                //string text = Session["CAPTCHA"].ToString();
                string text = _sessionService.Get<string>("CAPTCHA");

                // Create a CAPTCHA image using the text stored in the Session object.
                RandomImage ci = new RandomImage(text, 300, 70);

                // Change the response headers to output a JPEG image.
                this.Response.Clear();
                this.Response.ContentType = "image/Jpeg";

                // Write the image to the response stream in JPEG format.
                MemoryStream ms = new MemoryStream();
                ci.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                // Dispose of the CAPTCHA image object.
                ci.Dispose();

                return File(ms.ToArray(), "image/Jpeg");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult UpdateMobile(string uid, string dob, string mobileno)
        {
            short retVal = 0;
            Employee_Details dtchk = _loginService.GetEmpDetails(uid);
            if (dtchk == null)
            {
                ViewBag.Message = "Please enter correct Employee Code and D.O.B.";
                //return Json(ViewBag.Message);
                retVal = 2;
            }
            else
            {

                if ((DateTime.ParseExact(dtchk.DOB, "dd-MMM-yyyy", null)) != (DateTime.ParseExact(dob, "dd-MMM-yyyy", null)))
                {
                    ViewBag.Message = "Please enter correct Employee Code and D.O.B.";
                    //return Json(ViewBag.Message);
                    retVal = 2;
                }
                else
                {
                    string strStatus = _loginService.UpdateEmployeeMobileno(Convert.ToInt64(uid), mobileno, Convert.ToInt64(uid));
                    string[] passwordStatus = strStatus.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(passwordStatus[0]);
                    string errMsg = Convert.ToString(passwordStatus[1]);
                    string strPhoneNo = Convert.ToString(passwordStatus[2]);
                    string strEmpname = Convert.ToString(passwordStatus[3]);

                    if (errResult == "1")
                    {
                        REOTPTOKEN(uid);
                        retVal = 1;
                    }
                    else
                    {
                        //return Json("Something went wrong.Kindly contact to administrator.");
                        retVal = 2;
                    }
                }
            }

            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        public string? GetClientIp()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            if (httpContext.Request.Host.Host == "localhost")
                return "10.249.16.17";

            var ipFromHeader = httpContext.Request.Headers["HTTP_CLIENT_IP"].FirstOrDefault();
            return ipFromHeader ?? httpContext.Connection.RemoteIpAddress?.ToString();
        }

    }
}
