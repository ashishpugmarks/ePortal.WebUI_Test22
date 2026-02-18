using System.Collections;
using System.Data;
using System.Drawing;
using DocumentFormat.OpenXml.InkML;
using ePortal.Application.Contracts;
using ePortal.Persistence.Admin.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using iTextSharp.text.log;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _settings;
        private readonly ISessionService _sessionService;
        private readonly IManageSelfPending _objManageSelfPending;

        //Required Services
        private readonly IEmpLoginService _loginService;
        private readonly IWebHostEnvironment _env;
        private readonly IHomePageService _objHomeServ;
        private readonly IBikerCafeService _bikerCafeService;
        private readonly ISeatMgmtService _smService; //Seat booking popup
        private readonly IKaizenService _KZService; //for Dept Committee Kaizen
        private readonly ICorporateNews _objCorporateNewsService;  //Added By Bhupesh - NTT for CR-4894
        private readonly IEmpUserDetails _objEmpUserDetails;
        private readonly ISearchEmp _objSearchEmp;

        public HomeController(ILogger<HomeController> logger, IConfiguration settings, IEmpLoginService loginService, ISessionService sessionService,
            IHomePageService objHomeServ,
            IBikerCafeService bikerCafeService,
            ISeatMgmtService smService,
            IKaizenService KZService,
            ICorporateNews objCorporateNewsService, IManageSelfPending objManageSelfPending, IEmpUserDetails objEmpUserDetails, IWebHostEnvironment env, ISearchEmp objSearchEmp)
        {
            _logger = logger;
            _settings = settings;
            _loginService = loginService;
            _sessionService = sessionService;

            _objHomeServ = objHomeServ;
            _bikerCafeService = bikerCafeService;
            _smService = smService;
            _KZService = KZService;
            _objCorporateNewsService = objCorporateNewsService;
            _objManageSelfPending = objManageSelfPending;
            _objEmpUserDetails = objEmpUserDetails;
            _env = env;
            _objSearchEmp = objSearchEmp;
        }

        public ActionResult Index()
        {
            return View();
        }


        public IActionResult Crash()
        {
            throw new Exception("This is a test exception from Home/Crash");
        }

        //[AuthorizeUserAttribute]
        public async Task<ActionResult> Home()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}


            //if (Session["userID"] == null)
            //{
            //    Response.Redirect("/Login");
            //    return View();
            //}
            HomePageViewModel oHome = new HomePageViewModel();
            //Employee_Details obj = (Employee_Details)Session["Employee"];
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            try
            {
                String[] parm_value1 = _loginService.GetParameterValue("DESG_STAFFHOMEPAGE_VALIDATION").Split(',');
                ArrayList _arrayList1 = new ArrayList(parm_value1);
                String[] parm_value2 = _loginService.GetParameterValue("DESG_HOMEPAGE_VALIDATION").Split(',');
                ArrayList _arrayList2 = new ArrayList(parm_value2);
                if (_arrayList1.Contains(obj.Designation_Id)) //// obj != null && (obj._DesigId == (long)16 || obj._DesigId == (long)19 || obj._DesigId == (long)33) ---- 16 for Staff, 19 for Trainee, 33 for Line Associate
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else if (_arrayList2.Contains(obj.Designation_Id)) { }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }


            //long userid = Convert.ToInt64(Session["userID"].ToString());
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID"));
            string url = await _objHomeServ.GetMandatorytoFilledPage(Convert.ToDecimal(userid), Convert.ToDecimal(_sessionService.Get<string>("usertype")), 2, Convert.ToDecimal(_sessionService.Get<string>("ISSSOLOGIN"))); //--90/30 days password policy
            if (url != "")
            {
                //**
                //Uncomment this line when the upgraded Home page replaces the legacy version in the future..

                //Response.Redirect(url);    
            }
            DataSet ds = _loginService.MailApprovalGet(userid);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string strcontroller = ds.Tables[0].Rows[0]["CTRL_NAME"].ToString();
                string straction = ds.Tables[0].Rows[0]["CTRL_ACTION"].ToString();
                string strTid = ds.Tables[0].Rows[0]["CTRL_VALUE"].ToString();
                Response.Redirect("~" + "/" + strcontroller + "/" + straction + "?id=" + strTid);
            }
            PresidentDeskViewModel iList = new PresidentDeskViewModel();
            List<ContentViewModel> CVMList = new List<ContentViewModel>();

            iList.STATUS = 1;
            iList = _objHomeServ.GetPresidentDetail(iList);
            oHome.PresidentDesk = iList;
            CVMList.AddRange(_objHomeServ.GetContent(new ContentViewModel() { PROCESSID = 6, STATUS = 1 }));
            CVMList.AddRange(_objHomeServ.GetContent(new ContentViewModel() { PROCESSID = 8, STATUS = 1 }));
            CVMList.AddRange(_objHomeServ.GetContent());
            CVMList.AddRange(_objHomeServ.GetCommunicationContent()); //// -- Get New Announcement
            oHome.Announcement = CVMList;
            oHome.PersonalityQuotes = _objHomeServ.GetFamousQuotes(new PersonalityQuotesViewModel { STATUS = 1 });
            oHome.ApprovalCount = _objHomeServ.GetApprovalCount(userid);
            oHome.RequestCount = _objHomeServ.GetRequestCount(userid);
            oHome.RequestSelfCount = GetRequestSelfCount(userid);

            //Added By TTL against CR6852 as on 31-07-2025 
            if (TempData["NetworkType"] == null)
            {
                if (_sessionService.Get<string>("NetworkType") != null)
                {
                    TempData["NetworkType"] = _sessionService.Get<string>("NetworkType");
                }
            }

            int nettype = Convert.ToInt16(TempData["NetworkType"]);
            TempData.Keep("NetworkType");
            if (nettype == 0)
                ViewBag.URL = _settings["GeneralSettings:APIAddress"].ToString(); //System.Configuration.ConfigurationManager.AppSettings["APIAddress"].ToString();
            else
                ViewBag.URL = "http://portal.honda2wheelersindia.com//api/media/play?f=";

            oHome.QuickLink = _objHomeServ.GetQuickLinkMenu(nettype);
            oHome.DreamerCafeLink = _bikerCafeService.GetBCMenu();
            oHome.DOB = obj.DOB;

            oHome.ISPOPUPENABLEFOR_SEATMGMT = _smService.isPopUpEnableForDivisionHead(obj._ECode, obj._FnDesigId, obj._DivId); //Seat booking popup
            oHome.ISPOPUPENABLEFOR_KAIZENDEPTCOMMITTEE = _KZService.isPopupEnableForDeptCommittee(obj._ECode);// Kaizen Dept Committe Popup
            oHome.ISPOPUPENABLEFOR_KAIZENDivCOMMITTEE = _KZService.isPopupEnableForDivCommittee(obj._ECode);// Kaizen Dept Committe Popup
            return View(oHome);
        }
        public async Task<ActionResult> Dashboard()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            HomePageViewModel oHome = new HomePageViewModel();
            //Employee_Details obj = (Employee_Details)Session["Employee"];
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            ViewBag.des = obj.Designation_Id;  //Added By Bhupesh - NTT for CR-4894
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID"));
            string _url = await _objHomeServ.GetMandatorytoFilledPage(Convert.ToDecimal(userid), Convert.ToDecimal(_sessionService.Get<string>("usertype")), 2, Convert.ToDecimal(_sessionService.Get<string>("ISSSOLOGIN"))); //--90/30 days password policy
            if (_url != "")
            {
                Response.Redirect("~" + _url);
            }
            oHome.Announcement = _objHomeServ.GetContent(new ContentViewModel() { PROCESSID = 6, STATUS = 1 });
            oHome.RequestCount = _objHomeServ.GetRequestCount(userid);

            //Added By TTL against CR6852 as on 31-07-2025 
            if (TempData["NetworkType"] == null)
            {
                if (_sessionService.Get<string>("NetworkType") != null)
                {
                    TempData["NetworkType"] = _sessionService.Get<string>("NetworkType");
                }
            }

            int nettype = Convert.ToInt16(TempData["NetworkType"]);
            TempData.Keep("NetworkType");
            if (nettype == 0)
            {
                ViewBag.URL = _settings["GeneralSettings:APIAddress"].ToString(); // System.Configuration.ConfigurationManager.AppSettings["APIAddress"].ToString();
            }
            else
            {
                ViewBag.URL = "http://portal.honda2wheelersindia.com//api/media/play?f=";
            }
            //oHome.QuickLink = _objHomeServ.GetQuickLinkMenu(nettype);
            oHome.DOB = obj.DOB;
            ViewBag.PLANTID = obj.Plant_Id;
            var newsLetterEntities = _objCorporateNewsService.GetAllNewsLetters(); //Added by Bhupesh - NTT for CR-4894
            oHome.NewsLetters = newsLetterEntities.Select(n => new NewsLetterHomeViewModel
            {
                DESCRIPTION = n.DESCRIPTION,
                DOCUMENT_NAME = n.DOCUMENT_NAME
            }).ToList();
            return View(oHome);
        }

        public long GetRequestSelfCount(long userid)
        {
            long countVal = 0;
            //ManageSelfPending objdb = new ManageSelfPending();
            long rowcount = _objManageSelfPending.GetTotalSelfPending(Convert.ToInt32(_sessionService.Get<string>("userID")));
            if (rowcount > 0)
            {
                countVal = rowcount;
            }
            return countVal;
        }

        [HttpGet]
        public JsonResult GetMenu()
        {
            MenuViewModel iList = new MenuViewModel();

            if (_sessionService.Get<string>("MENU") != null && _sessionService.Get<string>("MENU") != "")
            {
                ////MenuViewModel iListMenu = new MenuViewModel();
                ////iListMenu.SubMenuItems = (List<MenuViewModel>)Session["MENU"];
                //return Json(_sessionService.Get<string>("MENU"));

                MenuViewModel iListMenu = new MenuViewModel();
                //iListMenu.SubMenuItems = (List<MenuViewModel>)Session["MENU"];
                iListMenu.SubMenuItems = _sessionService.Get<List<MenuViewModel>>("MENU");
                // return Json(_sessionService.Get<string>("MENU"));
                return Json(iListMenu);
            }

            if (_sessionService.Get<string>("userID") != null)
            {
                //Added By TTL against CR6852 as on 31-07-2025 
                if (TempData["NetworkType"] == null)
                {
                    if (_sessionService.Get<string>("NetworkType") != null)
                    {
                        TempData["NetworkType"] = _sessionService.Get<string>("NetworkType");
                    }
                }

                int nettype = Convert.ToInt16(TempData["NetworkType"]);
                TempData.Keep("NetworkType");

                iList.SubMenuItems = _objHomeServ.GetMenu(nettype).OrderBy(x => x.MenuLevel).ThenBy(t => t.MenuOrder).ToList();
                //Session["MENU"] = iList;
                _sessionService.Set("MENU", iList.SubMenuItems);
            }

            return Json(iList);
        }

        [HttpGet]
        public JsonResult GetEmergencyDept()
        {
            List<EmergencyContViewModel> iList = new List<EmergencyContViewModel>();
            iList = _objHomeServ.GetContactList(new EmergencyContViewModel { STATUS = 1 }).OrderBy(x => x.OPERATION).ToList();
            return Json(iList);
        }
        [HttpGet]
        public JsonResult GetEmergencyDeptbyid(string operation, string siteid)
        {
            List<EmergencyContViewModel> iList = new List<EmergencyContViewModel>();
            iList = _objHomeServ.GetContactList(new EmergencyContViewModel { STATUS = 1 }).OrderBy(x => x.OPERATION).ToList();
            var obj = iList.Where(m => m.OPERATION == operation && m.SiteID == Convert.ToInt32(siteid)).FirstOrDefault();
            return Json(obj);
        }

        [HttpGet]
        public JsonResult logout()
        {
            //Session["userID"] = null;
            //Session["Employee"] = null;

            _sessionService.Clear();
            _loginService.DeleteCookie(_settings["GeneralSettings:CookieName"].ToString()); //Added By TTL against CR6695 as on 29 - 07 - 2025
            return Json("TokenBridge/OldLogin");
        }


        //Added By TTL against CR6695 as on 29 - 07 - 2025
        [HttpGet]
        public void RedirectToOldAppLogin()
        {

            _sessionService.Clear();
            _loginService.DeleteCookie(_settings["GeneralSettings:CookieName"].ToString());
            RedirectToAction("TokenBridge/OldLogin");
        }



        [HttpGet]
        public void KeepSessionAlive()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                _sessionService.Clear();
                _loginService.DeleteCookie(_settings["GeneralSettings:CookieName"].ToString()); //Added By TTL against CR6695 as on 29 - 07 - 2025
                RedirectToAction("Index", "Login");
            }
        }


        [HttpGet]
        public ActionResult GetAssociateDetails()
        {
            string test = _sessionService.Get<string>("userID");
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            string fname = "Vineet";
            return Json(_objHomeServ.GetAssociateDetails("", "", "", "", "", "", fname, "", "", "", ""));
        }
        [HttpGet]
        public JsonResult GetPresidentContent(long id)
        {
            List<PresidentDesk_TRNViewModel> iList = new List<PresidentDesk_TRNViewModel> { new PresidentDesk_TRNViewModel { PRESIDENTMSG_ID = id, STATUS = 1 } };
            iList = _objHomeServ.GetPresidentContent(iList.ToList()).ToList();
            return Json(iList);
        }
        public ActionResult GetPresAttach(int id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            PresidentDesk_TRNViewModel newsList = new PresidentDesk_TRNViewModel();
            List<PresidentDesk_TRNViewModel> iList = new List<PresidentDesk_TRNViewModel> { new PresidentDesk_TRNViewModel { PRESIDENTMSG_ID = id, STATUS = 1 } };
            newsList = _objHomeServ.GetPresidentContentDwn(iList.ToList()).ToList().FirstOrDefault();
            //var base64 = Convert.ToBase64String(newsList.ATTACHMENT1);
            //var imgSrc = String.Format("data:" + newsList.ATTACHMENT1_CONTENTTYPE + ";base64,{0}", base64);
            return File(newsList.ATTACHMENT, newsList.ATTACHMENT_CONTENTTYPE, newsList.ATTACHMENT_NAME);
        }

        [HttpGet]
        public ActionResult BMS_Application()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Value = "1";
            ViewBag.MenuType = "4";

            //Employee_Details model = (Employee_Details)Session["Employee"];
            Employee_Details model = _sessionService.Get<Employee_Details>("Employee");
            return View(model);
        }

        [HttpGet]
        public ActionResult Inventory_Application()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Value = "2";
            ViewBag.MenuType = "3";
            //Employee_Details model = (Employee_Details)Session["Employee"];
            Employee_Details model = _sessionService.Get<Employee_Details>("Employee");
            return View(model);
        }
        public ActionResult OtherPortals()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<ExtPortalViewModel> modelList = _objHomeServ.GetExtPortalList();
            return View(modelList);
        }
        [HttpDelete]
        public ActionResult DeleteFamily(string ecode, string HeaderId)
        {
            int retVal = 0;
            string strmsg = "";
            List<ADEMP_FAMILYDeclaration> oList = new List<ADEMP_FAMILYDeclaration>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                //EmpUserDetails objAddDetail = new EmpUserDetails();
                //retVal = objAddDetail.DeleteFamilyDeclaration(ecode, HeaderId);

                retVal = _objEmpUserDetails.DeleteFamilyDeclaration(ecode, HeaderId);

                DataTable dt = _objEmpUserDetails.GetEmployeeDeclaration(_sessionService.Get<string>("userID"));
                foreach (DataRow dr in dt.Rows)
                {
                    ADEMP_FAMILYDeclaration obj = new ADEMP_FAMILYDeclaration();
                    obj.FamilyName = dr["ASSOCIATENAME"].ToString();
                    obj.RelationShip = dr["ASSOCIATEREL"].ToString();
                    obj.addedby = dr["ADEMPCODE"].ToString();
                    obj.ISPastorCurrent = dr["ISPASTORCURR"].ToString();
                    obj.FAMILYASSOCIATE_ID = (string.IsNullOrEmpty(dr["FAMILYASSOCIATE_ID"].ToString()) ? 0 : Convert.ToInt64(dr["FAMILYASSOCIATE_ID"].ToString()));
                    oList.Add(obj);
                }
                retVal = 1;
                strmsg = "Record Deleted Successfully";

            }
            catch (Exception ex)
            {
                retVal = -1;
                strmsg = ex.InnerException.ToString();
            }
            return Json(new { res = retVal, msg = strmsg, oListRet = oList });
        }
        [HttpPost]
        public ActionResult SkipUserdetail()
        {
            int retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                //EmpUserDetails objAddDetail = new EmpUserDetails();
                //retVal = objAddDetail.SkipUserdetail(Session["UserId"].ToString());
                retVal = _objEmpUserDetails.SkipUserdetail(_sessionService.Get<string>("userID"));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal });
        }

        [HttpPost]
        public ActionResult ADDEMPFamilyDetail_Associated(ADEMP_FAMILYDeclaration temp)
        {
            int retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (string.IsNullOrEmpty(temp.FamilyName) || string.IsNullOrEmpty(temp.ISPastorCurrent) || string.IsNullOrEmpty(temp.RelationShip))
                {
                    retVal = -2;
                    return Json(new { res = retVal, msg = "Employee Name and Relationship is mandatory." });
                }

                //temp.addedby = Session["userID"].ToString();
                _sessionService.Get<string>("userID");

                //EmpUserDetails objAddDetail = new EmpUserDetails();
                Tuple<Int32, string> ret = _objEmpUserDetails.UpdateFamilyDeclaration(temp.addedby, temp.FamilyName.Trim(), temp.RelationShip.Trim(), temp.addedby, Convert.ToInt32(temp.ISPastorCurrent));
                List<ADEMP_FAMILYDeclaration> oList = new List<ADEMP_FAMILYDeclaration>();
                DataTable dt = _objEmpUserDetails.GetEmployeeDeclaration(temp.addedby);
                foreach (DataRow dr in dt.Rows)
                {
                    ADEMP_FAMILYDeclaration obj = new ADEMP_FAMILYDeclaration();
                    obj.FamilyName = dr["ASSOCIATENAME"].ToString();
                    obj.RelationShip = dr["ASSOCIATEREL"].ToString();
                    obj.addedby = dr["ADEMPCODE"].ToString();
                    obj.ISPastorCurrent = dr["ISPASTORCURR"].ToString();
                    obj.FAMILYASSOCIATE_ID = (string.IsNullOrEmpty(dr["FAMILYASSOCIATE_ID"].ToString()) ? 0 : Convert.ToInt64(dr["FAMILYASSOCIATE_ID"].ToString()));
                    oList.Add(obj);
                }
                retVal = 1;
                return Json(new { res = retVal, msg = "Record Added Successfully.", oListRet = oList });
            }
            catch (Exception ex)
            {

                retVal = -1;
                return Json(new { res = retVal, msg = "Error:" + ex.InnerException.ToString() });
            }

        }

        //public IActionResult Error()
        //{
        //    var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        //    ViewBag.ErrorMessage = exceptionFeature?.Error.Message;
        //    return View();
        //}

        //public IActionResult StatusCode(int code)
        //{
        //    ViewBag.StatusCode = code;
        //    return View();
        //}




        public IActionResult EmployeeDetails(string id)
        {
            string strTransid = string.Empty;
            string photoUrl = string.Empty;

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userID")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                ViewBag.ErrorMessage = "Wrong Search has been done. Kindly select the employee from search results to see the employee detail.";
                return View(new EmployeeDetailViewModel());
            }
            else
            {
                strTransid = id;
                if (strTransid.Length == 2)
                {
                    strTransid = "0" + "0" + strTransid;
                }
                else if (strTransid.Length == 3)
                {
                    strTransid = "0" + strTransid;
                }
            }


            string strPhotoPath = serverpath.getPhotoPath();

            string strImagePath = string.Empty;

            strImagePath = strPhotoPath + strTransid + "." + "jpg";

            if (!System.IO.File.Exists(strImagePath))
            {
                strImagePath = strPhotoPath + "0264" + "." + "jpg";
            }

            string strPhotoServer = serverpath.getServerPath();

            if (System.IO.File.Exists(strPhotoPath + strTransid + "." + "jpg"))
            {
                photoUrl = @"" + strPhotoServer + "Uploads/Photographs/" + strTransid + "." + "jpg";
            }
            else
            {
                photoUrl = @"" + strPhotoServer + "Uploads/Photographs/0264.jpg";
            }

            //string paddedId = id.PadLeft(5, '0');
            //string photoPath = Path.Combine(_env.WebRootPath, "Uploads", "Photographs");
            //string fileName = $"{paddedId}.jpg";
            //string filePath = Path.Combine(photoPath, fileName);

            //if (!System.IO.File.Exists(filePath))
            //{
            //    fileName = "0264.jpg";
            //}

            //string photoUrl = $"/Uploads/Photographs/{fileName}";

            var ds = _objSearchEmp.OfficialDetail(strTransid);
            var model = new EmployeeDetailViewModel();


            //ltlEmpCode.Text = objDs.Tables[0].Rows[intCounter]["ADEMPCODE"].ToString();
            //ltlEmpName.Text = objDs.Tables[0].Rows[intCounter]["ENAME"].ToString();
            //ltlDiv.Text = objDs.Tables[0].Rows[intCounter]["DIVDESCRIP"].ToString();
            //ltlDept.Text = objDs.Tables[0].Rows[intCounter]["DEPTDESCRIP"].ToString();
            //ltlSectiion.Text = objDs.Tables[0].Rows[intCounter]["SECDESCRIP"].ToString();
            //ltlDesg.Text = objDs.Tables[0].Rows[intCounter]["DESG"].ToString();
            //ltlJobTitle.Text = objDs.Tables[0].Rows[intCounter]["JOBTITLE"].ToString();
            //ltlMobile.Text = objDs.Tables[0].Rows[intCounter]["TMOBILE"].ToString();
            //ltlExt.Text = objDs.Tables[0].Rows[intCounter]["EXTENSIONNO"].ToString();
            //ltlEmail.Text = objDs.Tables[0].Rows[intCounter]["EMAILID"].ToString();
            //ltlLocation.Text = objDs.Tables[0].Rows[intCounter]["LOCATION"].ToString();
            //ltlLandline.Text = objDs.Tables[0].Rows[intCounter]["LANDLINE"].ToString();
            //ltlBloodGroup.Text = objDs.Tables[0].Rows[intCounter]["BLOODGROUPCONSENT"].ToString();
            //ltlDOB.Text = objDs.Tables[0].Rows[intCounter]["DOB_CON"].ToString();
            //ltlOperation.Text = objDs.Tables[0].Rows[intCounter]["OPERATION"].ToString();
            //ltlSite.Text = objDs.Tables[0].Rows[intCounter]["SITENM"].ToString();
            //ltlfundesg.Text = objDs.Tables[0].Rows[intCounter]["FUNCTIONALDESIGNATION"].ToString() + objDs.Tables[0].Rows[intCounter]["ADDITIONALDESG"].ToString();



            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    //model.EmpCode = row["ADEMPCODE"]?.ToString();
                    //model.EmpName = row["ENAME"]?.ToString();  //row["ENAME"]?.ToString();
                    //model.Division = row["DIVDESCRIP"]?.ToString();
                    //model.Department = row["DEPTDESCRIP"]?.ToString();
                    //model.Section = row["SECDESCRIP"]?.ToString();
                    //model.Designation = row["DESG"]?.ToString();
                    //model.JobTitle = row["JOBTITLE"]?.ToString();
                    //model.Mobile = row["TMOBILE"]?.ToString();
                    //model.Extension = row["EXTENSIONNO"]?.ToString();
                    //model.Email = row["EMAILID"]?.ToString();
                    //model.Location = row["LOCATION"]?.ToString();
                    //model.DirectLine = row["LANDLINE"]?.ToString();
                    //model.BloodGroup = row["BLOODGROUPCONSENT"]?.ToString();
                    //model.DOB = row["DOB_CON"]?.ToString();
                    //model.Operation = row["OPERATION"]?.ToString();
                    //model.Site = row["SITENM"]?.ToString();
                    //model.FunctionalDesignation = row["FUNCTIONALDESIGNATION"]?.ToString() + row["ADDITIONALDESG"]?.ToString();
                    //


                    model.EmpCode = row["ADEMPCODE"]?.ToString();
                    model.EmpName = row["ENAME"]?.ToString();  //row["ENAME"]?.ToString();
                    model.Division = row["DIVDESCRIP"]?.ToString();
                    model.Department = row["DEPTDESCRIP"]?.ToString();
                    model.Section = row["SECDESCRIP"]?.ToString();
                    model.Designation = row["DESG"]?.ToString();
                    model.JobTitle = row["JOBTITLE"]?.ToString();
                    model.Mobile = row["TMOBILE"]?.ToString();
                    model.Extension = row["EXTENSIONNO"]?.ToString();
                    model.Email = row["EMAILID"]?.ToString();
                    model.Location = row["LOCATION"]?.ToString();
                    model.DirectLine = row["LANDLINE"]?.ToString();
                    model.BloodGroup = row["BLOODGROUPCONSENT"]?.ToString();
                    model.DOB = row["DOB_CON"]?.ToString();
                    model.Operation = row["OPERATION"]?.ToString();
                    model.Site = row["SITENM"]?.ToString();
                    model.FunctionalDesignation = row["FUNCTIONALDESIGNATION"]?.ToString() + row["ADDITIONALDESG"]?.ToString();

                    //model.EmpCode = SafeToString(row["ADEMPCODE"]);
                    //model.EmpName = SafeToString(row["ENAME"]);  //row["ENAME"]?.ToString();
                    //model.Division = SafeToString(row["DIVDESCRIP"]);
                    //model.Department = SafeToString(row["DEPTDESCRIP"]);
                    //model.Section = SafeToString(row["SECDESCRIP"]);
                    //model.Designation = SafeToString(row["DESG"]);
                    //model.JobTitle = SafeToString(row["JOBTITLE"]);
                    //model.Mobile = SafeToString(row["TMOBILE"]);
                    //model.Extension = SafeToString(row["EXTENSIONNO"]);
                    //model.Email = SafeToString(row["EMAILID"]);
                    //model.Location = SafeToString(row["LOCATION"]);
                    //model.DirectLine = SafeToString(row["LANDLINE"]);
                    //model.BloodGroup = SafeToString(row["BLOODGROUPCONSENT"]);
                    //model.DOB = SafeToString(row["DOB_CON"]);
                    //model.Operation = SafeToString(row["OPERATION"]);
                    //model.Site = SafeToString(row["SITENM"]);
                    //model.FunctionalDesignation = SafeToString(row["FUNCTIONALDESIGNATION"]) + SafeToString(row["ADDITIONALDESG"]);
                }
            }




            //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //{
            //    var row = ds.Tables[0].Rows[0];
            //    model.EmpCode = row["ADEMPCODE"]?.ToString();
            //    model.EmpName = row["ENAME"]?.ToString();
            //    model.Division = row["DIVDESCRIP"]?.ToString();
            //    model.Department = row["DEPTDESCRIP"]?.ToString();
            //    model.Section = row["SECDESCRIP"]?.ToString();
            //    model.Designation = row["DESG"]?.ToString();
            //    model.JobTitle = row["JOBTITLE"]?.ToString();
            //    model.Mobile = row["TMOBILE"]?.ToString();
            //    model.Extension = row["EXTENSIONNO"]?.ToString();
            //    model.Email = row["EMAILID"]?.ToString();
            //    model.Location = row["LOCATION"]?.ToString();
            //    model.DirectLine = row["LANDLINE"]?.ToString();
            //    model.BloodGroup = row["BLOODGROUPCONSENT"]?.ToString();
            //    model.DOB = row["DOB_CON"]?.ToString();
            //    model.Operation = row["OPERATION"]?.ToString();
            //    model.Site = row["SITENM"]?.ToString();
            //    model.FunctionalDesignation = row["FUNCTIONALDESIGNATION"]?.ToString() + row["ADDITIONALDESG"]?.ToString();
            //}

            var seatDs = _objSearchEmp.GetSeatNo(strTransid);
            if (seatDs != null && seatDs.Tables.Count > 0 && seatDs.Tables[0].Rows.Count > 0)
            {
                DataTable dtSeat = seatDs.Tables[0];

                foreach (DataRow row in dtSeat.Rows)
                {
                    model.SeatNo = row["fSeatName"]?.ToString();
                }
            }

            model.ImageUrl = photoUrl;

            return View(model);
        }


        //public static string SafeToString(object value)
        //{
        //    try
        //    {
        //        return value != null && value != DBNull.Value ? value.ToString() : string.Empty;
        //    }
        //    catch (Exception)
        //    {

        //        return "";
        //    }

        //}


    }
}
