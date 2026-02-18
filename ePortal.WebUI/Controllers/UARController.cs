using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace ePortal.WebUI.Controllers
{
    public class UARController : Controller
    {
        private readonly IUARService _UARService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        //private readonly commanEmail sendMail;
        public UARController(IUARService UARService, ISessionService objSessionService, ILogger<HomeController> logger)
        {
            _UARService = UARService;
            _sessionService = objSessionService;
            _logger = logger;
            //sendMail = _sendMail;
        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult GetUARList()
        {
            ViewBag.KICodeList = new SelectList(_UARService.GetKICodeList(Convert.ToInt64(_sessionService.Get<string>("userID"))), "SYKIID", "KICODE");  //Added For SR78412 
            return View();
        }

        [HttpGet]
        public JsonResult GetUARListData(string Status, string Ename, string Ecode)
        {

            List<UAR_AccessReviewViewModel> UARList = new List<UAR_AccessReviewViewModel>();
            int logincode = int.Parse(_sessionService.Get<string>("userID"));
            UARList = _UARService.GetUARList(logincode, Status, Ename, Ecode);
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(UARList);
            string sJSON = JsonSerializer.Serialize(UARList);
            return Json(sJSON);
        }
        public ActionResult UARRequestList(string id, string Status)
        {

            List<UAR_AccessReviewViewModel> UARList = new List<UAR_AccessReviewViewModel>();
            var data = string.Empty;
            UARList = _UARService.GetEmpNameList(long.Parse(id));
            ViewBag.Emp = id;
            ViewBag.Name = UARList;
            ViewBag.Status = Status;
            ViewBag.SYKICODE = _UARService.GetSYKIID();
            foreach (var item in UARList)
            {
                ViewBag.Name = item.EMPLOYEE.ToString();
            }
            return View("UARRequestList");
        }

        [HttpGet]
        public JsonResult UARRequestListData(String EmpCode)
        {
            List<UAR_ViewModel> UARList = new List<UAR_ViewModel>();
            UARList = _UARService.UARRequestList(EmpCode, _sessionService.Get<string>("userID"));
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(UARList);
            string sJSON = JsonSerializer.Serialize(UARList);
            return Json(sJSON);
        }

        public JsonResult ReviewData(string EmpCode, List<UAR_ViewModel> Data)
        {
            bool success = false;

            try
            {
                #region Remove rights one by one as per selection
                foreach (var item in Data)
                {
                    if (item.Checked == "false")
                    {
                        string res = _UARService.ReviewListData(EmpCode, item.SYUSERRIGHTSID, _sessionService.Get<string>("userID"), item.ACCESSTYPE); //Remove rights - syuserrights
                        // ACCESSTYPE added for SR78412
                    }
                }
                #endregion
                success = true;
                _UARService.UpdateADUARRequest(long.Parse(EmpCode)); //Update UAR approval status
                int logincode = int.Parse(_sessionService.Get<string>("userID"));
                SendTokenMail(logincode, EmpCode, Data);
            }
            catch (Exception ex)
            {
                success = false;
            }

            return Json(success);
        }

        //public JsonResult DeActiveUser(String EmpCode, string strDate)
        public JsonResult DeActiveUser(string EmpCode, List<UAR_ViewModel> Data)
        {
            bool success = false;

            foreach (var item in Data)
            {
                if (item.Checked == "false")
                {
                    string res1 = _UARService.ReviewListData(EmpCode, item.SYUSERRIGHTSID, _sessionService.Get<string>("userID"), item.ACCESSTYPE); //Remove rights - syuserrights
                    //item.ACCESSTYPE added for SR78412
                }
            }

            string res = _UARService.DeActiveUser(EmpCode, _sessionService.Get<string>("userID")); //Remove rights and HRID/Third party/Contract employee deactivate
            if (res == "true")
            {
                success = true;
                _UARService.UpdateADUARRequest(long.Parse(EmpCode)); //Update UAR approval status
            }

            return Json(success);
        }

        //-----------------------------------------------------------------------------------------------
        //-----SR39197 Change Start for Mail Format (Aumento)-------------------------
        //-----------------------------------------------------------------------------------------------
        public string SendTokenMail(int logincode, string Ecode, List<UAR_ViewModel> res)
        {
            short retVal = 0;
            try
            {
                //UARService _UARService = new UARService();
                string EmailID_Reviewer = string.Empty;
                string EmployeeName = string.Empty;
                string ReviewerName = string.Empty;
                long ReviewerEcode_ = logincode;
                string pathname = string.Empty;
                foreach (var item in res)
                {
                    if (item.Checked == "false")
                    {
                        pathname += item.PARENTPATH + "<br/>";
                    }
                }
                _UARService.GetEmailandName(ReviewerEcode_, out EmailID_Reviewer, out EmployeeName);
                _UARService.GetEmailandName(long.Parse(Ecode), out EmailID_Reviewer, out ReviewerName);
                if (!string.IsNullOrEmpty(EmailID_Reviewer))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                        sendMail.MailTo = EmailID_Reviewer;

                    //sendMail.MailTo = "Chetna.Upmanyue@honda.hmsi.in";                    
                    string strSubject = "UAR Request";

                    string strBody = "<html><body><table cellpadding=\"10\" cellspacing=\"0\" width=\"600\"><tr><td>Dear " + EmployeeName.ToString() + "<br/><br/></td></tr><tr><td>As part of SOX Mandatory requirement of User Access Review, your E-Portal ID having Admin Access has been reviewed by your " + ReviewerName.ToString() + " & below access has been removed from your ID.<br/><br/></td></tr><tr><td> Access Right " + "-><br/>" + pathname.ToString() + " has been removed<br/><br/></td></tr><tr><td></td></tr><tr><td>Thanking You.<br/><br/></td></tr><tr><td>With regards,<br/>GRC Team <br/> <b>Note: This is a system-generated email. Please do not reply.<b/></td></tr></table></body></html>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    finally
                    {
                        retVal = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return "true";
        }
        //-----------------------------------------------------------------------------------------------
        //-----SR39197 Change Start for Mail Format (Aumento)-------------------------
        //-----------------------------------------------------------------------------------------------

        //SR92683 Changes Start
        public string SendMailToReviewer(int logincode, string Ecode, List<UAR_Regular_ViewModel> res)
        {
            short retVal = 0;
            try
            {
                //UARService _UARService = new UARService();
                string EmailID_Reviewer = string.Empty;
                string EmployeeName = string.Empty;
                string ReviewerName = string.Empty;
                //long ReviewerEcode_ = logincode;
                //string pathname = string.Empty;
                //foreach (var item in res)
                //{
                //    if (item.Checked == "false")
                //    {
                //        pathname += item.PARENTPATH + "<br/>";
                //    }
                //}
                //_UARService.GetEmailandName(ReviewerEcode_, out EmailID_Reviewer, out EmployeeName);
                _UARService.GetEmailandName(long.Parse(Ecode), out EmailID_Reviewer, out ReviewerName);
                if (!string.IsNullOrEmpty(EmailID_Reviewer))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                        sendMail.MailTo = EmailID_Reviewer;

                    //sendMail.MailTo = "Chetna.Upmanyue@honda.hmsi.in";                    
                    string strSubject = "UAR Request";

                    string strBody = "<html><body><table cellpadding=\"10\" cellspacing=\"0\" width=\"600\"><tr><td>Dear Approver,<br/><br/></td></tr><tr><td>GRC Team has initiated Eportal User Access Review (UAR) of associates belongs to your Operation/ Division/ Department/ Section, who have admin access in Eportal as this is a mandatory requirement of SOX Compliance.​<br/><br/></td></tr><tr><td>Kindly perform appropriate action for the access assigned to respective Associates by ensuring below points:​<ul><li>Associate Job profile/responsibility change</li><li>Associate transfer to a new area</li><li>Remove Resigned User ID</li></ul></td></tr><tr><td>Please click on the <a href=\"https://portal.honda2wheelersindia.com\">Employee Portal</a> link to approve the request.</td></tr><tr><td>Thanking You.<br/><br/></td></tr><tr><td>With regards,<br/>GRC Team <br/> <b>Note: This is a system-generated email. Please do not reply.<b/></td></tr></table></body></html>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    finally
                    {
                        retVal = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return "true";
        }
        //SR92683 Changes End

        //-----------------------------------------------------------------------------------------------
        //-----SR78412 Change Start -------------------------
        //-----------------------------------------------------------------------------------------------
        public ActionResult UARRegularRequestList(string id, string Status, string REVIEWER)
        {
            List<UAR_AccessReviewViewModel> UARList = new List<UAR_AccessReviewViewModel>();
            var data = string.Empty;
            UARList = _UARService.GetEmpNameList(long.Parse(id));
            ViewBag.Emp = id;
            ViewBag.Name = UARList;
            ViewBag.Status = Status;
            ViewBag.SYKICODE = _UARService.GetSYKIID();
            ViewBag.REVIEWER = REVIEWER;
            foreach (var item in UARList)
            {
                ViewBag.Name = item.EMPLOYEE.ToString();
            }
            //SR92683 Changes Start
            String SelfReviewRemarks = _UARService.GetSelfReviewRemarks(long.Parse(id), long.Parse(REVIEWER));
            ViewBag.SelfReviewRemarks = SelfReviewRemarks;
            //SR92683 Changes End
            return View("UARRegularRequestList");
        }

        [HttpGet]
        public JsonResult UARRegularRequestListData(String EmpCode, String REVIEWER)
        {
            List<UAR_Regular_ViewModel> UARList = new List<UAR_Regular_ViewModel>();
            UARList = _UARService.UARRegularRequestList(EmpCode, REVIEWER);
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(UARList);
            string sJSON = JsonSerializer.Serialize(UARList);

            return Json(sJSON);
        }

        public JsonResult DeActiveRegularUser(string EmpCode, List<UAR_Regular_ViewModel> Data)
        {
            bool success = false;

            foreach (var item in Data)
            {
                int UserChecked = 0; //SR102715
                if (item.USERCHECKED == "true")
                {
                    //string res1 = _UARService.ReviewRegularListData(EmpCode, item.SYUSERRIGHTSID, _sessionService.Get<string>("userID"),item.ACCESSTYPE); //Remove rights - syuserrights //SR102715
                    // ACCESSTYPE added for SR78412
                    UserChecked = 1; //SR102715
                }
                string res1 = _UARService.ReviewRegularListData(EmpCode, item.SYUSERRIGHTSID, _sessionService.Get<string>("userID"), item.ACCESSTYPE, UserChecked); //Remove rights - syuserrights //SR102715
            }
            //string res = _UARService.DeActiveUser(EmpCode, _sessionService.Get<string>("userID")); //Remove rights and HRID/Third party/Contract employee deactivate
            //if (res == "true")
            //{
            success = true;
            _UARService.UpdateADUARRegularRequest(long.Parse(EmpCode)); //Update UAR approval status
                                                                        // }

            return Json(success);
        }
        [HttpPost]
        public JsonResult ReviewRegularData([FromBody]ReviewRequestParameter parameter) //SR92683
        {
            bool success = false;

            try
            {
                #region Remove rights one by one as per selection
                foreach (var item in parameter.Data)
                {
                    int UserChecked = 0; //SR102715
                    if (item.USERCHECKED == "true")
                    {
                        //string res = _UARService.ReviewRegularListData(EmpCode, item.SYUSERRIGHTSID, _sessionService.Get<string>("userID"),item.ACCESSTYPE); //Remove rights - syuserrights //SR102715
                        // ACCESSTYPE added for SR78412
                        UserChecked = 1; //SR102715
                    }
                    string res = _UARService.ReviewRegularListData(parameter.EmpCode, item.SYUSERRIGHTSID, _sessionService.Get<string>("userID"), item.ACCESSTYPE, UserChecked); //Remove rights - syuserrights //SR102715
                }
                #endregion
                success = true;
                _UARService.UpdateSelfReviewRemarks(long.Parse(parameter.EmpCode), parameter.SelfRemarks); //SR92683
                _UARService.UpdateADUARRegularRequest(long.Parse(parameter.EmpCode)); //Update UAR approval status
                int logincode = int.Parse(_sessionService.Get<string>("userID"));
                SendMailToReviewer(logincode, parameter.REVIEWER, parameter.Data);

            }
            catch (Exception ex)
            {
                success = false;
            }

            return Json(success);
        }
        public ActionResult UARRegularReviewerRequestList(string id, string Status, string REVIEWER)
        {
            List<UAR_AccessReviewViewModel> UARList = new List<UAR_AccessReviewViewModel>();
            var data = string.Empty;
            UARList = _UARService.GetEmpNameList(long.Parse(id));
            ViewBag.Emp = id;
            ViewBag.Name = UARList;
            ViewBag.Status = Status;
            ViewBag.SYKICODE = _UARService.GetSYKIID();
            ViewBag.REVIEWER = REVIEWER;
            foreach (var item in UARList)
            {
                ViewBag.Name = item.EMPLOYEE.ToString();
            }
            //SR92683 Changes Start
            String ReviewerRemarks = _UARService.GetReviewerRemarks(long.Parse(id), long.Parse(REVIEWER));
            ViewBag.ReviewerRemarks = ReviewerRemarks;
            //SR92683 Changes End
            return View("UARRegularReviewerRequestList");
        }
        [HttpGet]
        public JsonResult UARRegularReviewerRequestListData(String EmpCode, String REVIEWER)
        {
            List<UAR_Regular_ViewModel> UARList = new List<UAR_Regular_ViewModel>();
            UARList = _UARService.UARRegularReviewerRequestList(EmpCode, REVIEWER);
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(UARList);
            string sJSON = JsonSerializer.Serialize(UARList);
            return Json(sJSON);
        }
        //SR92683 Changes Start
        public ActionResult UARRegularRequestHistoryList(string UARID, string id, string Status, string REVIEWER)
        {
            List<UAR_AccessReviewViewModel> UARList = new List<UAR_AccessReviewViewModel>();
            var data = string.Empty;
            UARList = _UARService.GetEmpNameList(long.Parse(id));
            ViewBag.Emp = id;
            ViewBag.Name = UARList;
            ViewBag.Status = Status;
            ViewBag.SYKICODE = _UARService.GetSYKIID();
            ViewBag.REVIEWER = REVIEWER;
            ViewBag.UARID = UARID;
            foreach (var item in UARList)
            {
                ViewBag.Name = item.EMPLOYEE.ToString();
            }
            String ReviewerRemarks = _UARService.GetReviewerRemarks(long.Parse(id), long.Parse(REVIEWER));
            ViewBag.ReviewerRemarks = ReviewerRemarks;
            return View("UARRegularRequestHistoryList");
        }
        [HttpGet]
        public JsonResult UARRegularRequestHistoryListData(String UARID)
        {
            List<UAR_Regular_ViewModel> UARList = new List<UAR_Regular_ViewModel>();
            UARList = _UARService.UARRegularRequestHistoryList(UARID);
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(UARList);
            string sJSON = JsonSerializer.Serialize(UARList);
            return Json(sJSON);
        }
        //SR92683 Changes End
        public JsonResult ReviewRegularReviewerData(string EmpCode, List<UAR_Regular_ViewModel> Data, string Remarks) //SR92683
        {
            bool success = false;
            try
            {
                #region Remove rights one by one as per selection
                foreach (var item in Data)
                {
                    if (item.USERCHECKED == "false")
                    {
                        string res = _UARService.ReviewRegularReviewerListData(EmpCode, item.SYUSERRIGHTSID, _sessionService.Get<string>("userID"), item.ACCESSTYPE); //Remove rights - syuserrights
                        // ACCESSTYPE added for SR78412
                    }
                }
                #endregion
                success = true;
                _UARService.UpdateReviewerRemarks(long.Parse(EmpCode), Remarks); //SR92683
                _UARService.UpdateADUARRequest(long.Parse(EmpCode)); //Update UAR approval status
                int logincode = int.Parse(_sessionService.Get<string>("userID"));
                SendTokenMailReviewer(logincode, EmpCode, Data);
            }
            catch (Exception ex)
            {
                success = false;
            }

            return Json(success);
        }
        public string SendTokenMailReviewer(int logincode, string Ecode, List<UAR_Regular_ViewModel> res)
        {
            short retVal = 0;
            try
            {
                //UARService _UARService = new UARService();
                string EmailID_Reviewer = string.Empty;
                string EmailID_ = string.Empty; //SR92683
                string EmployeeName = string.Empty;
                string ReviewerName = string.Empty;
                long ReviewerEcode_ = logincode;
                string pathname = string.Empty;
                foreach (var item in res)
                {
                    if (item.USERCHECKED == "false")
                    {
                        pathname += item.PARENTPATH + "<br/>";
                    }
                }

                //SR92683 Start
                if (pathname.Length == 0)
                {
                    pathname = "<b> No access Removed </b>";
                }
                //SR92683 End

                _UARService.GetEmailandName(ReviewerEcode_, out EmailID_Reviewer, out ReviewerName); //SR92683
                _UARService.GetEmailandName(long.Parse(Ecode), out EmailID_, out EmployeeName); //SR92683

                if (!string.IsNullOrEmpty(EmailID_)) //SR92683
                {
                    commanEmail sendMail = new commanEmail();
                    //sendMail.MailFrom = "portal.admin@honda2wheelersindia.com";
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in"; //SR92683
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                        sendMail.MailTo = EmailID_; //SR92683

                    //sendMail.MailTo = "Chetna.Upmanyue@honda.hmsi.in";                   
                    string strSubject = "UAR Request";

                    string strBody = "<html><body><table cellpadding=\"10\" cellspacing=\"0\" width=\"600\"><tr><td>Dear " + EmployeeName.ToString() + " San,<br/><br/></td></tr><tr><td>As part of SOX Mandatory requirement of User Access Review, your E-Portal ID having Admin Access has been reviewed by your reviewer  (" + ReviewerName.ToString() + " San) & below access has been removed from your ID.<br/><br/></td></tr><tr><td>Below Access Right has been removed. <br/><br/> Access Right " + "-><br/>" + pathname.ToString() + " <br/><br/></td></tr><tr><td></td></tr><tr><td>Thanking You.<br/><br/></td></tr><tr><td>With regards,<br/>GRC Team <br/> <b>Note: This is a system-generated email. Please do not reply.<b/></td></tr></table></body></html>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    finally
                    {
                        retVal = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return "true";
        }
        public JsonResult DeActiveUserReviewer(string EmpCode, List<UAR_Regular_ViewModel> Data)
        {
            bool success = false;

            foreach (var item in Data)
            {
                if (item.USERCHECKED == "false")
                {
                    string res1 = _UARService.ReviewListData(EmpCode, item.SYUSERRIGHTSID, _sessionService.Get<string>("userID"), item.ACCESSTYPE); //Remove rights - syuserrights
                    // ACCESSTYPE added for SR78412
                }
            }

            string res = _UARService.DeActiveUser(EmpCode, _sessionService.Get<string>("userID")); //Remove rights and HRID/Third party/Contract employee deactivate
            if (res == "true")
            {
                success = true;
                _UARService.UpdateADUARRequest(long.Parse(EmpCode)); //Update UAR approval status
            }

            return Json(success);
        }
        public JsonResult SendBackRequest([FromBody] string EmpCode, string REVIEWER)
        {
            bool success = false;
            success = true;
            _UARService.SendBackADUARRequest(long.Parse(EmpCode), long.Parse(REVIEWER));
            return Json(success);
        }
        [HttpGet]
        public JsonResult GetUARStatusListData(string Status, string Ename, string Ecode, string SYKIID) //SYKIID Added for SR78412
        {

            List<UAR_AccessReviewViewModel> UARList = new List<UAR_AccessReviewViewModel>();
            int logincode = int.Parse(_sessionService.Get<string>("userID"));
            UARList = _UARService.GetUARStatusList(logincode, Status, Ename, Ecode, SYKIID); //SYKIID Added for SR78412
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(UARList);
            string sJSON = JsonSerializer.Serialize(UARList);
            return Json(sJSON);
        }
        //-----------------------------------------------------------------------------------------------
        //-----SR78412 Change End -------------------------
        //-----------------------------------------------------------------------------------------------
    }
}
