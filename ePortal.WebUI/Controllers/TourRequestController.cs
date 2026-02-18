using DocumentFormat.OpenXml.Bibliography;
//using DocumentFormat.OpenXml.Drawing.Charts;
using ePortal.DomainClasses;
using ePortal.Persistence;
using ePortal.Persistence.Interface;
using ePortal.Persistence.TourRequest.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.TourRequest;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Collections;
using System.Data;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;


namespace ePortal.WebUI.Controllers
{
    [SessionTimeout]
    [CSPFilter]
    public class TourRequestController : Controller
    {
        private readonly ITourQueries oTourQueries;
        private readonly ITourBudget oTourBudget;
        private readonly ISessionService _sessionService;
        private readonly ILogger<TourRequestController> _logger;
        private readonly IEportalESS objess;
        private readonly IAppConfigurationService _env;
        private readonly ICommonFunctions objCommon;

        DataTableConverter dtList = new DataTableConverter();
        TourRequestViewModel model = new TourRequestViewModel();
        ArrayList otourperiod = new ArrayList();
        ArrayList oTourList = new ArrayList();
        ArrayList oTourListprv = new ArrayList();
        ArrayList oTourListpre = new ArrayList();
        ArrayList oday = new ArrayList();
        ArrayList oTourListOrg = new ArrayList();

        int UserID = 0;
        string UserName = ""; 
        string EmpDesig = "";
        ArrayList TourPeriod = new ArrayList();


        public TourRequestController(ILogger<TourRequestController> logger, ITourQueries _oTourQueries, ITourBudget _oTourBudget, ISessionService sessionService, IAppConfigurationService appConfiguration, IEportalESS _objess, ICommonFunctions _objCommon)
        {
            _logger = logger;
            oTourQueries = _oTourQueries;
            oTourBudget = _oTourBudget;
            _sessionService = sessionService;
            objess = _objess;
            _env = appConfiguration;
            objCommon = _objCommon;
        }


        #region "TourRequest.Aspx"
        #region "Tour Request ASPX : Page Load Start"
        [HttpGet]
        public IActionResult TourRequest()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();
            var model = new TourRequestViewModel();
            
            if (UserID > 0)
            {
                try
                {
                    model.hdnUserId = UserID.ToString();
                    ViewBag.previous = oTourListprv;
                    ViewBag.SAVESTATUS = "NO";
                    ViewBag.TourList = oTourList;
                    //BY DEFAULT MODE IS ADD WHICH WILL CHANGE UPDATE WHEN USER UPDATE AN ENTRY (SINGLE BUTTON HANDLE BOTH ADD/UPDATE)
                    ViewBag.MODE = "ADD";

                    DataTable employeeDetails = oTourQueries.EmployeeDetail(UserID);
                    DataTable dt = oTourQueries.GetAppAuthorities(UserID.ToString());//SELECT APPROPRIATE AUTHORITY
                    DataTable dtAuthList = oTourQueries.GetAppAuthorityList(UserID.ToString(), UserID.ToString());//GET USER APPROVAL AUTORITIES

                    string empPOS = string.Empty;
                    string appAuthCode = string.Empty;

                    if (dt.Rows.Count > 0)
                    {
                        empPOS = dt.Rows[0]["POS"].ToString();

                        switch (empPOS)
                        {
                            case "OTH": // Other
                                appAuthCode = dt.Rows[0]["SECTIONHEADID"].ToString();
                                if (string.IsNullOrEmpty(appAuthCode))
                                    appAuthCode = dt.Rows[0]["DEPARTMENTHEADID"].ToString();
                                break;

                            case "SEC": // Section Manager
                                appAuthCode = dt.Rows[0]["DEPARTMENTHEADID"].ToString();
                                break;

                            case "DPT": // Department Manager
                                appAuthCode = dt.Rows[0]["DIVISIONHEADID"].ToString();
                                if (string.IsNullOrEmpty(appAuthCode))
                                    appAuthCode = dt.Rows[0]["VPHEADID"].ToString();
                                break;

                            case "DIV": // Divisional Manager
                                appAuthCode = dt.Rows[0]["VPHEADID"].ToString();
                                break;
                        }
                    }
                     
                        var objTaxiDs = new DataSet();
                        var dvData = new DataView(dtAuthList);  
                        dvData.RowFilter = "APPlvl IN ('S1')";  

                        var tblS1 = dvData.ToTable();  
                        tblS1.TableName = "S1";
                        objTaxiDs.Tables.Add(tblS1);

                    if (objTaxiDs.Tables[0].Rows.Count > 0)
                    {
                        appAuthCode = objTaxiDs.Tables[0].Rows[0]["ADEMPCODE"].ToString();
                    }
                    else
                    {
                        var objTaxiDs1 = new DataSet();
                        var dvData1 = new DataView(dtAuthList);
                        dvData1.RowFilter = "APPlvl IN ('S2')";
                        var tblS2 = dvData1.ToTable();
                        tblS2.TableName = "S2";
                        objTaxiDs1.Tables.Add(tblS2);

                        if (objTaxiDs1.Tables[0].Rows.Count > 0)
                        {
                            appAuthCode = objTaxiDs1.Tables[0].Rows[0]["ADEMPCODE"].ToString();
                        }
                    }

                    TempData["REPORTINGAUTH"] = appAuthCode;
                    TempData["AppAuthCode"] = appAuthCode ?? string.Empty; 
                    ViewBag.EmpDesig = employeeDetails.Rows[0]["ADDESIGNATIONID"]; 

                    List<gvAdvance> prevTourList = FillAdvanceDetails(UserID.ToString()); 

                    var tourPeriodList =
                        otourperiod.Cast<object>()
                                   .Select(o => new SelectListItem
                                   {
                                       Text = o.GetType().GetProperty("TourPeriod")?.GetValue(o)?.ToString() ?? "",
                                       Value = o.GetType().GetProperty("Amount")?.GetValue(o)?.ToString() ?? ""
                                   })
                                   .Prepend(new SelectListItem { Text = "-Select Period-", Value = "0" })
                                   .ToList(); 

                    model = new TourRequestViewModel
                    {
                        hdnUserId = UserID.ToString(),
                        grdPrevList = dtList.TableToList<grdPrevList>(oTourQueries.GetPreviousRequestList(UserID.ToString())),

                        Cities = oTourQueries.GetCityList().AsSelectList_DS("SYCITYID", "DESCRIP", true, "All City", "0"),
                        TravelModes = oTourQueries.GetTravelModeList().AsSelectList_DS("ADTRAVELMODEID", "DESCRIP", true, "All Travel Mode", "0"),

                        cmbFromList = oTourQueries.GetCityList().AsSelectList_DS("SYCITYID", "DESCRIP", true, "-- select --", "0", true, "Other", ""),
                        cmbToList = oTourQueries.GetCityList().AsSelectList_DS("SYCITYID", "DESCRIP", true, "-- select --", "0", true, "Other", ""),
                        cmbStayingList = oTourQueries.GetCityList().AsSelectList_DS("SYCITYID", "DESCRIP", true, "-- select --", "0", true, "Other", ""),

                        cmbModeList = oTourQueries.GetTravelModeList().AsSelectList_DS("ADTRAVELMODEID", "DESCRIP", true, "-- select --", "0"),
                        cmbClassList = oTourQueries.GetTravelModeClass(string.Empty).AsSelectList_DS("ADTRAVELMODECLASSID", "DESCRIP", true, "-- select --", "0"),

                        BankAccount = employeeDetails.Rows[0]["ACCOUNTNO"].ToString(),
                        txtMobile = employeeDetails.Rows[0]["TMOBILE"].ToString(),
                        txtExtension = employeeDetails.Rows[0]["EXTENSIONNO"].ToString(),

                        cboAppAuthorityList = dtAuthList.AsSelectList("ADEMPCODE", "EMPNAME", true, "-- select --", "0"),
                        cboAppAuthority = appAuthCode, // Default Approval Authority
                        lblTourCoordinator = oTourQueries.GetTourCoordinator(UserID.ToString()).Rows[0]["CORDINATOR"].ToString(),//SHOW TOUR COORDINATOR

                        gvAdvance = prevTourList,
                        //TourPeriodList = oTourQueries.GetTouradvancedata(userId.ToString()).AsSelectList("amount", "tourperiod", true, "-Select Period-", "0"),
                        HFADVREMARKS = prevTourList.Count > 0 ? "YES" : "NO",
                        TourPeriodList = tourPeriodList
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                        MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                }
            }

            return View(model);
        }
        #endregion "Tour Request ASPX : Page Load End"
        #region "Tour Request ASPX : Control Events Start"
        [HttpGet]
        public IActionResult cmbMode_SelectedIndexChanged(string cmbModeSelectedValue)
        {
            object? response = null;
            response = new
            {
                cmbClassList = oTourQueries.GetTravelModeClass(cmbModeSelectedValue)
                    .AsSelectList_DS("ADTRAVELMODECLASSID", "DESCRIP", true, "-- select --", "0"),
                //cmbMode.Focus();

                trTrainVisible = cmbModeSelectedValue == "3", // Train section visibility

                ticketingByList = cmbModeSelectedValue == "2" ?
                                    new List<SelectListItem>
                                    {
                                        new SelectListItem { Text = "Self", Value = "0" },
                                        new SelectListItem { Text = "Admin", Value = "1" }
                                    }
                                    : new List<SelectListItem>() // Empty if not mode 2

                //cmdAddMore.Focus(); // Added by Aumento :: SR78268
            };

            return Json(new { result = response });
        }

        [HttpGet]
        public IActionResult FillGSTINNOClass(string CityId)
        {
            try
            {
                if (!string.IsNullOrEmpty(CityId) && CityId != "0") // Updated by Aumento as on 28022024
                {
                    DataTable dt = oTourQueries.GSTNODetail(CityId);

                    var response = new
                    {
                        //cmbClass_DataSource = dt.AsEnumerable().Select(r => new SelectListItem
                        //{
                        //    Value = r["state"].ToString(),  
                        //    Text = r["gstinno"].ToString()  
                        //}).ToList(),
                        txt_GST_Text = dt.Rows.Count > 0 ? dt.Rows[0][1].ToString() : "-"
                    };

                    return Json(new { result = response });
                }

                // Default response when CityId is empty or "0"
                return Json(new { result = new { /*cmbClass_DataSource = new List<SelectListItem>(),*/ txt_GST_Text = "-" } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching GST details." });
            }
        }

        [HttpGet]
        public IActionResult FillTravelClass(string ModeID)
        {
            //BOUND FROM LOCATION
            try
            {
                var cmbClassList = oTourQueries.GetTravelModeClass(ModeID).AsSelectList_DS("ADTRAVELMODECLASSID", "DESCRIP", true, "-- select --", "");

                return Json(new { result = cmbClassList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching Travel Class details." });
            }
        }

        [HttpGet]
        public IActionResult GetCityCategory(string CityCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(CityCode) && CityCode != "0")
                {
                    DataRow[] _datarow;
                    _datarow = oTourQueries.GetCityCategory(CityCode);
                    if (_datarow.Length > 0)
                    {

                        var response = new
                        {
                            DESCRIP = _datarow[0]["DESCRIP"].ToString(),
                            SYCITYCATEGORYID = System.Convert.ToInt32(_datarow[0]["SYCITYCATEGORYID"])
                        };
                        return Json(new { result = response });
                    }
                }

                return Json(new { result = new { DESCRIP = "_", SYCITYCATEGORYID = 0 } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching GetCityCategory details." });
            }
        }

        [HttpGet]
        public IActionResult GetEmployeeAllowanceDetail(int CityCategoryCode, int EmpDesignationID, string CurrDate, string UserType)
        {
            try
            {
                if (CityCategoryCode > 0 && EmpDesignationID > 0 && !string.IsNullOrEmpty(CurrDate) && !string.IsNullOrEmpty(UserType) && UserType != "0")
                {
                    DataRow[] _datarow;
                    _datarow = _datarow = oTourQueries.GetEmployeeAllowanceDetail(CityCategoryCode, EmpDesignationID, CurrDate, UserType);
                    if (_datarow.Length > 0)
                    {

                        var response = new
                        {
                            LODGINGAMT = System.Convert.ToDouble(_datarow[0]["LODGINGAMT"]),
                            DAILYALLOWANCEAMT = System.Convert.ToDouble(_datarow[0]["DAILYALLOWANCEAMT"])
                        };
                        return Json(new { result = response });
                    }
                }

                return Json(new { result = new { DESCRIP = "_", SYCITYCATEGORYID = 0 } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching GetEmployeeAllowanceDetail details." });
            }
        }

        private List<gvAdvance> FillAdvanceDetails(string userId)
        {
            var dt = oTourQueries.GetTouradvancedata(userId);
            var prevTourList = new List<gvAdvance>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var period = row["TOURPERIOD"].ToString();
                    var amount = row["TOURAMOUNT"].ToString();

                    var tour = new gvAdvance
                    {
                        TourPeriod = period,
                        Amount = amount
                    };

                    if (period.Length == 17 && !amount.Contains("-"))
                    {
                        var tourDate = DateTime.ParseExact(period.Substring(9, 8), "dd.MM.yy", null).AddDays(7);
                        var dayDiff = (DateTime.Today - tourDate).TotalDays;

                        tour.Day = dayDiff > 0 ? dayDiff.ToString() : string.Empty;
                        otourperiod.Add(tour);
                    }

                    prevTourList.Add(tour);
                }
            }
            TourPeriod = otourperiod;
            ViewBag.TourPeriod = Newtonsoft.Json.JsonConvert.SerializeObject(otourperiod);


            return prevTourList;
        }

        [HttpGet]
        public IActionResult CheckDayDetails(string tourDate)
        {
            try
            {
                UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var dt = oTourQueries.TourDayDetails(UserID.ToString(), tourDate);
                if (dt.Rows.Count > 0)
                {
                    var result = dt.AsEnumerable().Select(row => new
                    {
                        TRAVELDATE = row["TRAVELDATE"].ToString(),
                        DAYOBJECTIVE = row["DAYOBJECTIVE"].ToString(),
                        FRMCITY = row["FRMCITY"].ToString(),
                        TOCITY = row["TOCITY"].ToString()
                    }).ToList();

                    return Ok(result);
                }
                return Ok(new List<object>()); // Empty list if no records
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                   "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                   MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching CheckDayDetails." });
            }
        }


        [HttpGet]
        public IActionResult txtMiscAmout_TextChanged()
        {
            /* double MiscAmount = 0;
             double StayCharge = 0;
             double Allowances = 0;
             if (txtMiscAmout.Trim() != "")
                 MiscAmount = System.Convert.ToDouble(txtMiscAmout);

             StayCharge = System.Convert.ToDouble(lblTotalCharges);
             Allowances = System.Convert.ToDouble(lblTotalAllowances);
             lblTotalAmount = string.Format("{0:F2}", (StayCharge + Allowances + MiscAmount));
             txtRequiredAmount = string.Format("{0:F2}", (StayCharge + Allowances + MiscAmount));*/
            object? response = null;
            return Json(new { result = response });
        }

        [HttpGet]
        public IActionResult txtRequiredAmount_TextChanged(object sender, EventArgs e)
        {
            object? response = null;
            return Json(new { result = response });
            /* double TotalAmount = Convert.ToDouble(lblTotalAmount);
             double RequiredAmount = Convert.ToDouble(txtRequiredAmount);
             if (RequiredAmount > TotalAmount)
             {
                 txtRequiredAmount = string.Format("{0:F2}", TotalAmount);
                 txtRequiredAmount.Focus();
                 ScriptManager.RegisterClientScriptBlock(Page, GetType(), "key", "alert('Required amount should be less than equal to System generated amount')", true);
             }*/
        }

        // Added By Aumento For SR73072 Start        


        [HttpGet]
        public IActionResult GetEmployeeOfficialDetails()
        {
            DataTable dt = new DataTable();
            try
            {
                UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
                if (!string.IsNullOrEmpty(UserID.ToString()) && UserID.ToString() != "0")
                {
                    dt = objCommon.GetEmployeeOfficialDetails(UserID.ToString(), ""); 
                    var result = new
                    {
                        SYSITEID = dt.Rows[0]["SYSITEID"]?.ToString() 
                    };

                    return Json(new { result });
                }

                // Default response when UserID is empty or "0"
                return Json(new { result = new { dt } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching GetEmployeeOfficialDetails details." });
            }
        }

        [HttpGet]
        public IActionResult GetParameterValue()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            try
            {

                string SitesDay = objCommon.GetParameterValue("TOURREQUEST_VALIDATION");
                return Json(new { result = new { _SitesDay = SitesDay } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching GetParameterValue details." });
            }
        }


        [HttpGet]
        public IActionResult TourDayDetailsForAdvance(string TourFromDate)
        {
            DataTable dt = new DataTable();
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            try
            {                
                if (!string.IsNullOrEmpty(UserID.ToString()) && UserID.ToString() != "0")
                {
                    dt = oTourQueries.TourDayDetailsForAdvance(UserID.ToString(), TourFromDate);

                    var response = new
                    {
                        result = dt
                    };

                    return Json(new { result = response });
                }

                // Default response when UserID is empty or "0"
                return Json(new { result = new { dt } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching TourDayDetailsForAdvance details." });
            }
        }

        //[HttpGet]
        //public IActionResult GetAppAuthorities(string EmpCode)
        //{
        //    UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
        //    if (string.IsNullOrEmpty(EmpCode) || EmpCode == "0")
        //        EmpCode = UserID.ToString();

        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(EmpCode) && EmpCode != "0")
        //        {
        //            dt = oTourQueries.GetAppAuthorities(EmpCode);


        //            var response = new
        //            {
        //                result = dt
        //            };

        //            return Json(new { result = response });
        //        }

        //        // Default response when EmpCode is empty or "0"
        //        return Json(new { result = new { dt } });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,
        //            "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
        //            MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

        //        return StatusCode(500, new { error = "An error occurred while fetching GetAppAuthorities details." });
        //    }
        //}
        [HttpGet]
        public IActionResult GetAppAuthorities(string EmpCode)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));

            if (string.IsNullOrEmpty(EmpCode) || EmpCode == "0")
                EmpCode = UserID.ToString();

            try
            {
                DataTable dt = oTourQueries.GetAppAuthorities(EmpCode);

                var result = dt.AsEnumerable()
                    .Select(r => new
                    {
                        POS = r["POS"]?.ToString(),
                        ADVPID = r["ADVPID"] == DBNull.Value ? 0 : Convert.ToInt32(r["ADVPID"]),
                        EMAILID = r["EMAILID"]?.ToString()
                    })
                    .ToList();

                return Json(new { result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching GetAppAuthorities details." });
            }
        }


        [HttpPost]
        public IActionResult InsertTourDetail([FromBody] InsertTourDetailRequest req)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            string EmpCode = UserID.ToString();
            string REPORTINGAUTH = TempData["REPORTINGAUTH"].ToString();
            TempData.Keep("REPORTINGAUTH");
            try
            {
                if (!string.IsNullOrEmpty(EmpCode) && EmpCode != "0")
                {
                    var oTourListArray = new ArrayList(req.oTourList ?? new List<Tour>());
                    var oTourListPrvArray = new ArrayList(req.oTourListprv ?? new List<TourAdvance>());

                    int TxnNumber = oTourQueries.InsertTourDetail(EmpCode, req.MobileNo, req.ExtNo, req.Email, req.Objective, req.AdvRemarks, req.AdvRequired, req.APlusNights, req.ANights, req.BNights, req.CNights, req.StayCharge, req.APlusDays, req.ADays, req.BDays, req.CDays, req.DailyAllow, req.MiscAllow, req.authType, req.AppAuthCode, oTourListArray, req.MiscRemarks, req.RequiredAmount, req.TourStartDate, req.TourEndDate, oTourListPrvArray, req.Initiator_Status);

                    if (TxnNumber > 1 && req.Initiator_Status == "1")
                    {

                        if (req.AppAuthEmailID != "")
                            SendMail_TR(req.AppAuthEmailID, req.Objective, req.MobileNo, req.ExtNo, req.Email, TxnNumber);
                        //IF SELECT OTHER AUTHORITY FOR APPROVAL
                        if (REPORTINGAUTH != req.AppAuthCode && !string.IsNullOrEmpty(REPORTINGAUTH))
                        {
                            //GET APP AUTH POS
                            DataTable dt = new DataTable();
                            dt = oTourQueries.GetAppAuthorities(REPORTINGAUTH);
                            req.AppAuthEmailID = dt.Rows[0]["EMAILID"].ToString();
                            if (req.AppAuthEmailID != "")
                                SendMail_TR(req.AppAuthEmailID, req.Objective, req.MobileNo, req.ExtNo, req.Email, TxnNumber);
                        }
                    }

                    var response = new
                    {
                        TxnNumber = TxnNumber
                    };

                    return Json(new { result = response });
                }

                // Default response when parameters null
                return Json(new { result = new { TxnNumber = 0 } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching InsertTourDetail details." });
            }
        }

        [HttpGet]
        public IActionResult TourPerioddropdown()
        {
            try
            {
                var response = new
                {
                    result = ViewBag.TourPeriod
                };

                return Json(new { result = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching TourPerioddropdown details." });
            }
        }
        #endregion "Tour Request ASPX : Control Events End"
        #region "Tour Request ASPX : Functions Start"
        private void SendMail_TR(string strApprovalAuthEmail, string Objective, string MobileNo, string ExtNo, string Email, int TxnNumber)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();

            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";

            if (serverpath.isTestServer())
                sendMail.MailTo = serverpath.getTestEMail();
            else
                sendMail.MailTo = strApprovalAuthEmail;

            string strSubject = "Tour Request from - " + UserName + " (Employee Code: " + UserID.ToString() + ")";
            string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                             "</td></tr>" +
                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                             "</td></tr><tr height=150><td colspan=2>" +
                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td valign=top colspan=2><b>" + UserName + " San has submitted a Tour Request. The details are as follows:</b></td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td valign=top width='20%'>Employee Code:</td><td valign=top >" + UserID.ToString() + "</td></tr>" +
                             "<tr><td valign=top >Employee Name:</td><td valign=top >" + UserName + "</td></tr>" +
                             "<tr><td valign=top >Tour Objective:</td><td valign=top >" + Objective + "</td></tr>" +
                             "<tr><td valign=top >Mobile No.:</td><td valign=top >" + MobileNo + "</td></tr>" +
                             "<tr><td valign=top >Ext. No:</td><td valign=top >" + ExtNo + "</td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "Aspxview/TourRequest/TourRequestApproval.aspx?id=" + TxnNumber.ToString() + "&ecode=" + UserID.ToString() + "> Employee Portal</a> for your approval.</td></tr>" +
                             //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" +
                             "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated Email, please do not reply.</strong></td> " +
                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                             "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                             "</tr></table> ";


            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
            }
            finally
            {

            }
        }
        #endregion "Tour Request ASPX : Functions End"
        #endregion "TourRequest.Aspx"
        /*--------------------------------------------------------------------------*/


        #region "TourRequestList.Aspx"
        #region "Tour Request List ASPX  Start : Page Load Start"
        [HttpGet]
        public IActionResult TourRequestList()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();

            if (string.IsNullOrEmpty(UserID.ToString()))
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user is Operation Coordinator
            bool isOpCoordinator = oTourQueries.isOperationCoordinator(UserID.ToString()) == "YES";

            var model = new TourRequestListViewModel
            {
                cboFilter = isOpCoordinator,
                TourRequestList = null
            };

            return View(model);
        }
        #endregion "Tour Request List ASPX : Page Load End" 
        #region "Tour Request List ASPX : Control Events"
        [HttpGet]
        public IActionResult Search(string RequestID, string EmpCode, string EmpName, string RequestStatus, string HardCopyStatus, string FromDate, string TillDate, string AdvanceRequired, string FilterType)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            DataTable dt = new DataTable();
            try
            {
                //GET TOUR LIST FOR SELECTED DEPARTMENT COORDINATOR
                dt = oTourQueries.GetDepartmentTourList(UserID.ToString(), RequestID, EmpCode, EmpName, RequestStatus, HardCopyStatus, FromDate, TillDate, AdvanceRequired, FilterType);
                var response = JsonConvert.SerializeObject(dt);
                return Json(new { result = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching GetAppAuthorities details." });
            }
        }
        #endregion "Tour Request List ASPX : Control Events" 
        #endregion "TourRequestList.Aspx"
        /*-----------------------------------------------------------------------------*/


        #region "CancelTourRequest.Aspx"
        #region "Cancel Tour Request ASPX : Page Load Start"
        [HttpGet]
        //public IActionResult CancelTourRequest()
        //{
        //    CancelTourRequestViewModel model = new CancelTourRequestViewModel();
        //    string RequestID = HttpContext.Request.Query["id"];
        //    UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
        //    UserName = (_sessionService.Get<string>("userName")).ToString();


        //    oTourList = oTourQueries.GetRequestDetailPart(RequestID);
        //    TempData["TourList"] = oTourList;
        //    DataTable dt = dtList.ArrayListToDataTable<Tour>(oTourList);

        //    model.gvList = dtList.TableToList<gvListCTR>(dt);

        //    ArrayList oPrevTourList = new ArrayList();
        //    DataTable dtprevTour = oTourQueries.GetTouradvancedata(UserID.ToString());


        //    string Ecode = string.Empty;
        //    string Period = string.Empty;
        //    string Amount = string.Empty;
        //    if (dtprevTour.Rows.Count > 0)
        //    {
        //        for (int i = 0; dtprevTour.Rows.Count > i; i++)
        //        {
        //            Ecode = dtprevTour.Rows[i]["ECODE"].ToString();
        //            Period = dtprevTour.Rows[i]["TOURPERIOD"].ToString();
        //            Amount = dtprevTour.Rows[i]["TOURAMOUNT"].ToString();
        //            //IF CURRENT RECORD IS USER RECORD THEN
        //            Tour oTour = new Tour();
        //            oTour.TourPeriod = Period;
        //            oTour.Amount = Amount;
        //            if (Period.Length == 17)
        //            {
        //                if (oTour.Amount.IndexOf("-") == -1)
        //                {
        //                    Int32 day = Convert.ToInt32(((DateTime.Parse(DateTime.Today.ToString())) - ((DateTime.ParseExact(Period.Substring(9, 8), "dd.MM.yy", null)).AddDays(7))).TotalDays);
        //                    if (day > 0)
        //                        oTour.Day = day.ToString();
        //                    else
        //                        oTour.Day = "";


        //                    otourperiod.Add(oTour);
        //                }
        //            }
        //            oPrevTourList.Add(oTour);
        //        }
        //    }
        //    dtprevTour = dtList.ArrayListToDataTable<Tour>(oPrevTourList);
        //    model.gvAdvance = dtList.TableToList<gvAdvance>(dtprevTour);

        //    return View(model);
        //    // Check Session 
        //    //if (string.IsNullOrEmpty(UserID.ToString()))
        //    //{
        //    //    return Unauthorized(new { redirectUrl = "/Login" });
        //    //}
        //    //return Ok(new
        //    //{
        //    //    isCancelled = true
        //    //});
        //}
        public IActionResult CancelTourRequest(string id)
        {
            var model = new CancelTourRequestViewModel();
            TempData["RequestID"] = id.ToString();//Tour Changes
            TempData.Keep();//Tour Changes
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            string userName = _sessionService.Get<string>("userName");

            // ================= TOUR REQUEST DETAILS =================

            var _tourList = oTourQueries.GetRequestDetailPartForAdminApproval(id);
            
            List<Tour> tourList = _tourList.Cast<Tour>().ToList();
            model.TourList = tourList;
            DataTable dtTour = dtList.ArrayListToDataTable<Tour>(
                new ArrayList(tourList)
            );

            model.gvList = dtList.TableToList<gvListCTR>(dtTour);

            // ================= ADVANCE DETAILS =================
            DataTable dtPrevTour = oTourQueries.GetTouradvancedata(userId.ToString());
            List<Tour> advanceList = new();

            foreach (DataRow row in dtPrevTour.Rows)
            {
                var tour = new Tour
                {
                    TourPeriod = row["TOURPERIOD"].ToString(),
                    Amount = row["TOURAMOUNT"].ToString()
                };

                if (!string.IsNullOrEmpty(tour.TourPeriod)
                    && tour.TourPeriod.Length == 17
                    && !tour.Amount.Contains("-"))
                {
                    int day = (DateTime.Today -
                              DateTime.ParseExact(
                                  tour.TourPeriod.Substring(9, 8),
                                  "dd.MM.yy",
                                  null
                              ).AddDays(7)).Days;

                    tour.Day = day > 0 ? day.ToString() : string.Empty;
                }

                advanceList.Add(tour);
            }

            DataTable dtAdvance = dtList.ArrayListToDataTable<Tour>(
                new ArrayList(advanceList)
            );

            model.gvAdvance = dtList.TableToList<gvAdvance>(dtAdvance);

            DataTable dtheaderReq = new DataTable();
            dtheaderReq = oTourQueries.GetRequestHeaderPart(id, UserID.ToString());
            if (dtheaderReq.Rows.Count > 0)
            {
            //    ViewState["EmpDesig"] = dtheaderReq.Rows[0]["ADDESIGNATIONID"].ToString();
            //ViewState["AppDate"] = dtheaderReq.Rows[0]["APPLICATIONDATE"].ToString();

           model.lblRequestID = id;
                model.ADEMPCODE = Convert.ToString(dtheaderReq.Rows[0]["ADEMPCODE"]);
                model.lblPeriod = dtheaderReq.Rows[0]["PERIOD"].ToString();
            model.lblDays = dtheaderReq.Rows[0]["DAYS"].ToString();
            model.lblObjective = dtheaderReq.Rows[0]["OBJOFJOURNEY"].ToString();
                model.lblAdvRemarks = dtheaderReq.Rows[0]["PREVIOUSTOURADVREMARKS"].ToString();

                //period = dtheaderReq.Rows["0"]["PERIOD"]?.ToString();
                //days = row["DAYS"]?.ToString(),
                //objective = row["OBJOFJOURNEY"]?.ToString(),
                //advRemarks = row["PREVIOUSTOURADVREMARKS"]?.ToString(),
                model.miscAmount = dtheaderReq.Rows[0]["MISCELLANEOUSAMT"]?.ToString();
                model.requiredAmount = dtheaderReq.Rows[0]["TOTALAMOUNTREQUIRED"]?.ToString();
                model.isAdvanceRequired = dtheaderReq.Rows[0]["ISADVANCEREQ"]?.ToString() == "1";
                model.empDesignationId = dtheaderReq.Rows[0]["ADDESIGNATIONID"]?.ToString();
                model.applicationDate = dtheaderReq.Rows[0]["APPLICATIONDATE"]?.ToString();
                //TempData["AppDate"] = dt.Rows[0]["APPLICATIONDATE"].ToString();
                CancelTour_CalculateDayNights_TRA(model);
            }


            return View(model);
        }

        private void CancelTour_CalculateDayNights_TRA(CancelTourRequestViewModel model)
        {
            string CityCode;
            string StayingCity = string.Empty;
            int rownum;
            int days;
            int nights;
            int chargeAP = 0;
            int chargeA = 0;
            int chargeB = 0;
            int chargeC = 0;
            int TotalNights;

            int allowAP = 0;
            int allowA = 0;
            int allowB = 0;
            int allowC = 0;
            int TotalDays;
            int EmpDesignationID;

            double NightCharge = 0;
            double TotalNightCharge = 0;
            double DailyAllowance = 0;
            double TotalDailyAllowance = 0;

            //CHECK STAY CHARGE/DAILY ALLOWANCE WHEN TOUR ADVANCE IS SELECTED
            //if (model.isAdvanceRequired == true && model.gvList.Any())
            if (model.gvList.Any())
            {
                //SHOW ADVANCE PANEL
                string TravelDate = string.Empty;
                string CityCategory = string.Empty;
                int CityCategoryCode = 0;
                Boolean isDayCalculated = false;
                Boolean isNightCalculated = false;
                Tour oTour = new Tour();
                var oTourList = model.TourList;// (ArrayList)TempData["TourList"];
                List<string> ProcessDates = new List<string>();
                List<string> ProcessNights = new List<string>();

                for (rownum = 0; rownum < oTourList.Count; rownum++)
                {
                    oTour = (Tour)oTourList[rownum];
                    days = 1;
                    nights = 1;
                    //GET TRAVEL DATE
                    TravelDate = oTour.TourFromDate;
                    //CHECK DATE ALREADY PROCESSED OR NOT
                    isDayCalculated = DailyAllowanceCaluculation(ProcessDates, TravelDate);
                    if (isDayCalculated == true)
                    {
                        days = 0;
                    }
                    else
                    {
                        ProcessDates.Add(TravelDate);
                    }

                    //GET STAYING CITY CODE
                    CityCode = oTour.StayingLocCode;
                    StayingCity = oTour.StayingLoc.Trim();

                    //CHECK NIGHT ALREADY PROCESSED OR NOT
                    isNightCalculated = StayChargeCaluculation(ProcessNights, TravelDate, StayingCity);
                    if (isNightCalculated == true)
                    {
                        nights = 0;
                    }
                    else
                    {
                        //CHECK IF STAYING CITY EXIST
                        if (StayingCity != "")
                            ProcessNights.Add(TravelDate);
                    }
                    //IF NO STAYING CITY IS THERE
                    if (StayingCity == "")
                    {
                        nights = 0;
                        //DAILY ALLOWANCES CALCULATED ON TO CITY CODE
                        CityCode = oTour.ToLocCode;
                    }
                    //IF CITYCODE IS 0 IE OTHER THEN SET IT TO BLANK BECAUSE IS DOESN'T STORE IN DATABASE
                    if (CityCode == "0")
                        CityCode = "";

                    //GET CATEGORY DESCRIPTION OF CITY (RETURN C IN CASE OF OTHER CITY(CITYCODE=0)
                    DataRow[] _datarow;
                    _datarow = oTourQueries.GetCityCategory(CityCode);
                    if (_datarow.Length > 0)
                    {
                        CityCategory = _datarow[0]["DESCRIP"].ToString();
                        CityCategoryCode = System.Convert.ToInt16(_datarow[0]["SYCITYCATEGORYID"]);
                    }

                    EmpDesignationID = System.Convert.ToInt16(model.empDesignationId);
                    // Added By Kishan Dodiya
                    string UserType = Convert.ToInt64(model.ADEMPCODE) > 70000000 ? "EXPAT" : "LOCAL";
                    //CALCULATE NIGHT CHARGE AND DAILY ALLOWANCE CITY WISE
                    _datarow = oTourQueries.GetEmployeeAllowanceDetail(CityCategoryCode, EmpDesignationID, model.applicationDate.ToString(), UserType);
                    // End Added
                    if (_datarow.Length > 0)
                    {
                        NightCharge = System.Convert.ToDouble(_datarow[0]["LODGINGAMT"]);
                        DailyAllowance = System.Convert.ToDouble(_datarow[0]["DAILYALLOWANCEAMT"]);
                    }
                    TotalDailyAllowance += (DailyAllowance * days);
                    TotalNightCharge += (NightCharge * nights);

                    switch (CityCategory)
                    {
                        case "A+":
                            chargeAP += nights;
                            allowAP += days;
                            break;
                        case "A":
                            chargeA += nights;
                            allowA += days;
                            break;
                        case "B":
                            chargeB += nights;
                            allowB += days;
                            break;
                        case "C":
                            chargeC += nights;
                            allowC += days;
                            break;
                    }
                }
                TotalNights = chargeAP + chargeA + chargeB + chargeC;
                TotalDays = allowAP + allowA + allowB + allowC;


                model.lblStayChargeAPLUS = chargeAP.ToString();
                model.lblAllownceAPLUS = allowAP.ToString();
                model.lblStayChargeA = chargeA.ToString();
                model.lblAllownceA = allowA.ToString();
                model.lblStayChargeB = chargeB.ToString();
                model.lblAllownceB = allowB.ToString();
                model.lblStayChargeC = chargeC.ToString();
                model.lblAllownceC = allowC.ToString();

                model.lblTotalNights = TotalNights.ToString();
                model.lblTotalDays = TotalDays.ToString();

                //CALCULATE TOTAL ALLOWANCES AND CHARGE
                model.lblTotalAllowances = string.Format("{0:F2}", TotalDailyAllowance);
                model.lblTotalCharges = string.Format("{0:F2}", TotalNightCharge);

                TotalNights = chargeAP + chargeA + chargeB + chargeC;
                TotalDays = allowAP + allowA + allowB + allowC;

            }
            else
            {
                //HIDE ADVANCE PANEL
                //model.pnlAdvance = false;
            }
            double MiscAmount = 0;
            MiscAmount = System.Convert.ToDouble(model.miscAmount);

            model.TotalAmount = string.Format("{0:F2}", (TotalDailyAllowance + TotalNightCharge + MiscAmount));
        }


        #endregion "Cancel Tour Request ASPX : Page Load End" 
        #region "Cancel Tour Request ASPX : Control Event Start"

        [HttpGet]
        public IActionResult GetRequestHeader(string requestId)
        {
            int userID = Convert.ToInt32(_sessionService.Get<string>("userID"));

            try
            {
                if (userID != 0){

                    DataTable dt = oTourQueries.GetRequestHeaderPart(requestId, userID.ToString());

                    if (dt.Rows.Count > 0)
                    {

                        DataRow row = dt.Rows[0];

                        var result = new
                        {
                            period = row["PERIOD"]?.ToString(),
                            days = row["DAYS"]?.ToString(),
                            objective = row["OBJOFJOURNEY"]?.ToString(),
                            advRemarks = row["PREVIOUSTOURADVREMARKS"]?.ToString(),
                            miscAmount = row["MISCELLANEOUSAMT"]?.ToString(),
                            requiredAmount = row["TOTALAMOUNTREQUIRED"]?.ToString(),
                            isAdvanceRequired = row["ISADVANCEREQ"]?.ToString() == "Y",
                            empDesignationId = row["ADDESIGNATIONID"]?.ToString(),
                            applicationDate = row["APPLICATIONDATE"]?.ToString()
                        };
                        return Ok(new { result });
                    }
                }

                return Ok();



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRequestHeader");
                return StatusCode(500, new { error = "Failed to load request header." });
            }
        }
        [HttpGet]
        public IActionResult GetAdvanceDetails()
        {
            int userID = Convert.ToInt32(_sessionService.Get<string>("userID"));

            try
            {
                if (userID != 0)
                {
                    DataTable dt = oTourQueries.GetTouradvancedata(userID.ToString());
                    var result = new List<object>();

                    foreach (DataRow row in dt.Rows)
                    {
                        string period = row["TOURPERIOD"]?.ToString();
                        string amount = row["TOURAMOUNT"]?.ToString();
                        string day = "";

                        if (!string.IsNullOrEmpty(period) && period.Length == 17 && !amount.Contains("-"))
                        {
                            int diff = (DateTime.Today -
                                DateTime.ParseExact(period.Substring(9, 8), "dd.MM.yy", null)
                                .AddDays(7)).Days;

                            if (diff > 0) day = diff.ToString();
                        }

                        result.Add(new
                        {
                            period,
                            amount,
                            day
                        });
                        return Ok(new { result });
                    }

                }


                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAdvanceDetails");
                return StatusCode(500, new { error = "Failed to load advance details." });
            }
        }

        [HttpGet]
        public IActionResult GetRequestDetails(string requestId)
        {
            int userID = Convert.ToInt32(_sessionService.Get<string>("userID"));

            try
            {
                if (userID != 0)
                {

                    ArrayList list = oTourQueries.GetRequestDetailPartForAdminApproval(requestId);

                    var result = new List<object>();

                    foreach (var item in list)
                    {
                        var tour = (Tour)item; // cast to your model

                        result.Add(new
                        {
                            fromDate = tour.TourFromDate,
                            toDate = tour.TourFromDate,
                            location = tour.ToLoc,
                            stayingLoc = tour.StayingLoc,
                            stayingLocCode = tour.StayingLocCode,
                            toLocCode = tour.ToLocCode
                        });
                    }

                    return Ok(new { result });
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRequestDetails");
                return StatusCode(500, new { error = "Failed to load request details." });
            }
        }

        //[HttpGet]
        //public IActionResult GetRequestHeader(string requestId)
        //{
        //    UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(UserID.ToString()) && UserID.ToString() != "0")
        //        {
        //            dt = oTourQueries.GetRequestHeaderPart(requestId, UserID.ToString());
        //            if (dt.Rows.Count > 0)
        //            {
        //                ViewBag.EmpDesig = dt.Rows[0]["ADDESIGNATIONID"].ToString();
        //                ViewBag.AppDate = dt.Rows[0]["APPLICATIONDATE"].ToString();
        //            }

        //            var response = new
        //            {
        //                result = dt
        //            };

        //            return Json(new { result = response });
        //        }
        //        return Json(new { result = new { dt } });// Default response when UserID is empty or "0"
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,
        //            "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
        //            MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

        //        return StatusCode(500, new { error = "An error occurred while fetching GetRequestHeader details." });
        //    }
        //}

        //[HttpGet]
        //public IActionResult GetAdvanceDetails()
        //{
        //    UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(UserID.ToString()) && UserID.ToString() != "0")
        //        {
        //            var dt = oTourQueries.GetTouradvancedata(UserID.ToString());
        //            var result = new List<object>();

        //            foreach (DataRow row in dt.Rows)
        //            {
        //                string period = row["TOURPERIOD"].ToString();
        //                string amount = row["TOURAMOUNT"].ToString();
        //                string day = "";

        //                if (period.Length == 17 && !amount.Contains("-"))
        //                {
        //                    int diff = (int)((DateTime.Today) -
        //                        (DateTime.ParseExact(period.Substring(9, 8), "dd.MM.yy", null).AddDays(7))).TotalDays;
        //                    day = diff > 0 ? diff.ToString() : "";
        //                }

        //                result.Add(new
        //                {
        //                    Period = period,
        //                    Amount = amount,
        //                    Day = day
        //                });
        //            }

        //            return Ok(new { result });
        //        }

        //        return Ok(new { result = new List<object>() }); // Empty list if userId is invalid
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,
        //            "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
        //            MethodBase.GetCurrentMethod()?.Name, UserID.ToString(), ex.Message);

        //        return StatusCode(500, new { error = "An error occurred while fetching GetAdvanceDetails." });
        //    }
        //}

        //[HttpGet]
        //public IActionResult GetRequestDetails()
        //{
        //    UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
        //    string RequestID = HttpContext.Request.Query["id"];
        //    ArrayList dt = new ArrayList();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(UserID.ToString()) && UserID.ToString() != "0")
        //        {
        //            dt = oTourQueries.GetRequestDetailPartForAdminApproval(RequestID);
        //            ViewBag.TourList = dt;
        //            var response = new
        //            {
        //                result = dt
        //            };

        //            return Json(new { result = response });
        //        }
        //        return Json(new { result = new { dt } });// Default response when UserID is empty or "0"
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex,
        //            "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
        //            MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

        //        return StatusCode(500, new { error = "An error occurred while fetching GetRequestDetails details." });
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult> CmdSubmit_Click_CR([FromBody] CancelButtonViewModel model)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            string RequestID = model.lblRequestID;
            string strcancelremark = model.txtCancellationRemarks?.Trim() ?? string.Empty;
            string requestId = RequestID?.Trim() ?? string.Empty;

            string strResult = string.Empty;
            strResult = oTourQueries.CancelTourRequest(requestId, strcancelremark);
            if (strResult.Trim() != "")
            {
                return Ok(new { success = false, message= strResult });// StatusCode(500, new { error = strResult });
            }
            else
            {
                String errmsg1 = oTourBudget.INSERTTRAVELBUDGETHIS("", "", "", requestId, "1", "", "0", UserID.ToString());
                Sendmail_CR(model.lblObjective, model.lblPeriod, model.lblDays, strcancelremark);


                int TxnNumber;
                TxnNumber = oTourQueries.CheckRequestEmailsStatus(requestId);
                if (TxnNumber == 1)
                {
                    SendmailTravelDesk(model.lblObjective, model.lblPeriod, model.lblDays, strcancelremark);
                    SendmailHotelDesk(model.lblObjective, model.lblPeriod, model.lblDays, strcancelremark);
                }

                //var success = true;
                return Ok(new { success = true });

            }
        }
        #endregion "Cancel Tour Request ASPX : Control Event End"
        #region "Cancel Tour Request ASPX : Function Start"
        private void Sendmail_CR(string lblObjective, string lblPeriod, string lblDays, string txtCancellationRemarks)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            string RequestID = HttpContext.Request.Query["id"];

            string strSubject = string.Empty;
            string strBody = string.Empty;
            string strAppAuthEmail = string.Empty;
            string strcc = string.Empty;
            commanEmail sendMail = new commanEmail();
            DataTable dt = new DataTable();

            //GET APPROVAL AUTHORITIES EMAILS
            dt = oTourQueries.GetRequestEmails(RequestID);
            string FirstAppAuthEmail = string.Empty;
            string SecondAppAuthEmail = string.Empty;
            string FinAppAuthEmail = string.Empty;
            string AdminAppAuthEmail = string.Empty;

            if (dt.Rows.Count > 0)
            {
                FirstAppAuthEmail = dt.Rows[0]["RECEMAIL"].ToString();
                SecondAppAuthEmail = dt.Rows[0]["APPEMAIL"].ToString();
                FinAppAuthEmail = dt.Rows[0]["FINEMAIL"].ToString();
                AdminAppAuthEmail = dt.Rows[0]["ADMINEMAIL"].ToString();
            }
            if (FirstAppAuthEmail.Trim() != "")
                strAppAuthEmail = FirstAppAuthEmail;

            if (SecondAppAuthEmail.Trim() != "")
            {
                if (strAppAuthEmail.Trim() != "")
                    strAppAuthEmail = strAppAuthEmail + "," + SecondAppAuthEmail;
                else
                    strAppAuthEmail = SecondAppAuthEmail;
            }
            if (AdminAppAuthEmail.Trim() != "")
                strcc = AdminAppAuthEmail;

            if (FinAppAuthEmail.Trim() != "")
            {
                if (AdminAppAuthEmail.Trim() != "")
                    strcc = strcc + "," + FinAppAuthEmail;
                else
                    strcc = FinAppAuthEmail;
            }


            if (strAppAuthEmail != "" || strcc != "")
            {
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    if (strAppAuthEmail != "")
                    {
                        sendMail.MailTo = strAppAuthEmail;
                        if (strcc != "")
                            sendMail.MailCc = strcc;
                    }
                    else
                        sendMail.MailTo = strcc;
                }


                strSubject = "Tour Cancellation Request from - " + UserName.ToString() + "[ " + UserID.ToString() + " ]";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "<b>Tour Request</b></td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear San,</b><br />" +
                                 "<br />" + UserName.ToString() +
                                 "has Cancel a Tour request, the details are as follows:<br />" +
                                 "<br />" +
                                 "<tr><td width=125 height=23 valign=top>Tour Objective:</td><td width=389 valign=top>" + lblObjective + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Tour Period:</td><td width=389 valign=top>" + lblPeriod + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Tour Days:</td><td width=389 valign=top>" + lblDays + "</td>" +
                                 "</tr><tr>" +
                                 "<td width=125 height=23 valign=top>Reason of Cancellation:</td><td width=389 valign=top>" + txtCancellationRemarks + "</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                 "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                 "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                 "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                 "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                 "</tr></table> ";


                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                try
                {
                    bool status1 = sendMail.Send();
                }
                catch (Exception ex)
                {
                    //Response.Write("Exception Occured:   " + ex);
                }
            }
        }

        protected void SendmailHotelDesk(string lblObjective, string lblPeriod, string lblDays, string txtCancellationRemarks)
        {
            string RequestID = HttpContext.Request.Query["id"];

            string strSubject = string.Empty;
            string strBody = string.Empty;
            string strHotelDeskEmail = string.Empty;
            string strcc = string.Empty;
            commanEmail sendMail = new commanEmail();
            DataTable dt = new DataTable();
            strHotelDeskEmail = objCommon.GetParameterValue("HOTEL_DESK_MAIL");

            if (strHotelDeskEmail != "")
            {
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    if (strHotelDeskEmail != "")
                    {
                        sendMail.MailTo = strHotelDeskEmail;
                    }
                }

                strSubject = "Hotel Cancellation Request from - " + UserName.ToString() + "[ " + UserID.ToString() + " ]";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "<b>Tour Request</b></td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear San,</b><br />" +
                                 "<br />" + UserName.ToString() +
                                 " has Cancel a Tour request, the details are as follows:<br />" +
                                 "<br />" +
                                 "<tr><td width=125 height=23 valign=top>Tour Objective:</td><td width=389 valign=top>" + lblObjective + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Tour Period:</td><td width=389 valign=top>" + lblPeriod + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Tour Days:</td><td width=389 valign=top>" + lblDays + "</td>" +
                                 "</tr><tr>" +
                                 "<td width=125 height=23 valign=top>Reason of Cancellation:</td><td width=389 valign=top>" + txtCancellationRemarks + "</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                 "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                 "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                 "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                 "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                 "</tr></table> ";


                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                try
                {
                    bool status1 = sendMail.Send();
                }
                catch (Exception ex)
                {
                    //Response.Write("Exception Occured:   " + ex);
                }
            }
        }

        protected void SendmailTravelDesk(string lblObjective, string lblPeriod, string lblDays, string txtCancellationRemarks)
        {
            string RequestID = HttpContext.Request.Query["id"];

            string strSubject = string.Empty;
            string strBody = string.Empty;
            string strTravelDeskEmail = string.Empty;
            string strcc = string.Empty;
            commanEmail sendMail = new commanEmail();
            DataTable dt = new DataTable();
            strTravelDeskEmail = objCommon.GetParameterValue("TRAVEL_DESK_MAIL");

            if (strTravelDeskEmail != "")
            {
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    if (strTravelDeskEmail != "")
                    {
                        sendMail.MailTo = strTravelDeskEmail;

                    }

                }
                strSubject = "Travel Cancellation Request from - " + UserName.ToString() + "[ " + UserID.ToString() + " ]";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "<b>Tour Request</b></td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear San,</b><br />" +
                                 "<br />" + UserName.ToString() +
                                 "  has Cancel a Tour request, the details are as follows:<br />" +
                                 "<br />" +
                                 "<tr><td width=125 height=23 valign=top>Tour Objective:</td><td width=389 valign=top>" + lblObjective + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Tour Period:</td><td width=389 valign=top>" + lblPeriod + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Tour Days:</td><td width=389 valign=top>" + lblDays + "</td>" +
                                 "</tr><tr>" +
                                 "<td width=125 height=23 valign=top>Reason of Cancellation:</td><td width=389 valign=top>" + txtCancellationRemarks + "</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                 "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                 "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                 "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                 "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                 "</tr></table> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                try
                {
                    bool status1 = sendMail.Send();
                }
                catch (Exception ex)
                {
                    //Response.Write("Exception Occured:   " + ex);
                }
            }
        }
        #endregion "Cancel Tour Request ASPX : Function End"
        #endregion "CancelTourRequest.Aspx"
        /*--------------------------------------------------------------------------*/


        #region "MovementDetails.Aspx"
        #region "Movement Details ASPX  Start : Page Load Start"
        [HttpGet]
        public IActionResult MovementDetails()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();

            if (string.IsNullOrEmpty(UserID.ToString()))
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new MovementDetailsViewModel
            {

            };

            return View(model);
        }
        #endregion "Movement Details ASPX : Page Load End" 
        #region "Movement Details ASPX : Control Events"
        [HttpGet]
        public IActionResult SearchMovementDetails(string RequestID, string EmpCode, string EmpName, string FromDate, string TillDate)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            DataTable dt = new DataTable();
            try
            {
                //GET TOUR LIST FOR SELECTED DEPARTMENT COORDINATOR
                dt = oTourQueries.GetAssociatesMovementDetails(UserID.ToString(), RequestID, EmpCode, EmpName, FromDate, TillDate);
                var response = JsonConvert.SerializeObject(dt);
                return Json(new { result = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching MovementDetailsSearch details." });
            }
        }
        #endregion "Movement Details ASPX : Control Events" 
        #endregion "MovementDetails.Aspx"
        /*-----------------------------------------------------------------------------*/


        #region "TourApplicationDetails.Aspx"
        #region "Tour Application Details ASPX  Start : Page Load Start"
        [HttpGet]
        public IActionResult TourApplicationDetail()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();

            string RequestID = Request.Query["id"];

            if (string.IsNullOrEmpty(UserID.ToString()))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new TourApplicationDetailViewModel();
            DataTable dt = oTourQueries.GetRequestHistory(RequestID);
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];

                // Helper to safely read values
                string Get(string col) =>
                    dt.Columns.Contains(col) && row[col] != DBNull.Value ? row[col]?.ToString() ?? string.Empty : string.Empty;

                // Top-level flags (used for conditional sections in view)
                var recommAuthEmpCode = Get("RECADEMPCODE");
                var spApproval = Get("SPAPPROVAL");
                var dir2Approval = Get("DIR2APPROVAL");   // Added by TTL on 04-Oct-2025 against SR110729 > CR7297
                var fnApproval = Get("FNAPPROVAL");
                var adminApproval = Get("ADMINAPPROVAL");

                model.pnlRecAuth = !string.IsNullOrWhiteSpace(recommAuthEmpCode);
                model.pnlSPAuth = string.Equals(spApproval, "YES", StringComparison.OrdinalIgnoreCase);
                model.pnlDir2Auth = string.Equals(dir2Approval, "YES", StringComparison.OrdinalIgnoreCase);
                model.pnlFinance = string.Equals(fnApproval, "YES", StringComparison.OrdinalIgnoreCase);
                model.pnlAdmin = string.Equals(adminApproval, "YES", StringComparison.OrdinalIgnoreCase);

                // APPLICANT DETAILS
                model.ltlEmpCode = Get("ADEMPCODE");
                model.ltlEmpName = Get("EMPNAME");
                model.ltlObjective = Get("OBJOFJOURNEY");
                model.ltlPeriod = Get("TOURPERIOD");
                model.ltlDays = Get("DAYS");
                model.ltlRequestDate = Get("REQUESTDATE");
                model.ltlRequestStatus = Get("REQSTATUS"); // Added For SR73072

                // RECOMMENDED AUTHORITY DETAILS
                if (model.pnlRecAuth)
                {
                    model.ltlRecAuth = Get("RECNAME");
                    model.ltlRecEmail = Get("RECEMAIL");
                    model.ltlRecStatus = Get("STRRECSTATUS");
                    model.ltlRecRem = Get("RECREMAKRS");
                    model.ltlRecDate = Get("RECDATE");
                }

                // SPECIAL APPROVAL AUTHORITY DETAILS
                if (model.pnlSPAuth)
                {
                    model.ltlSPAppName = Get("SPAPPNAME");
                    model.ltlSPAppEmail = Get("SPAPPEMAIL");
                    model.ltlSPAppStatus = Get("STRSPAPPSTATUS");
                    model.ltlSPAppRemarks = Get("SPAPPREMARKS");
                    model.ltlSPAppDate = Get("SPAPPDATE");
                }

                // DIR2 APPROVAL (Added by TTL on 04-Oct-2025 against SR110729 > CR7297)
                if (model.pnlDir2Auth)
                {
                    model.ltlDir2AppName = Get("DIR2APPNAME");
                    model.ltlDir2AppEmail = Get("DIR2APPEMAIL");
                    model.ltlDir2AppStatus = Get("STRDIR2APPSTATUS");
                    model.ltlDir2AppRemarks = Get("APPREMARKS_DIR2");
                    model.ltlDir2AppDate = Get("DIR2APPDATE");
                }

                // APPROVE AUTHORITY DETAILS
                model.ltlAppName = Get("APPNAME");
                model.ltlAppEmail = Get("APPEMAIL");
                model.ltlAppStatus = Get("STRAPPSTATUS");
                model.ltlAppRemarks = Get("APPREMARKS");
                model.ltlAppDate = Get("APPDATE");

                // FINANCE DEPT DETAILS
                if (model.pnlFinance)
                {
                    model.ltlFinName = Get("FINNAME");
                    model.ltlFinEmail = Get("FINEMAIL");
                    model.ltlFinRemarks = Get("FINANCEREMARKS");
                    model.ltlFinStatus = Get("STRFINSTATUS");
                    model.ltlFinAppDate = Get("FINAPPDATE");
                    model.ltlTransferAmount = Get("TRANSFERAMOUNT");
                }

                // ADMIN DEPT DETAILS
                if (model.pnlAdmin)
                {
                    model.ltlAdminName = Get("ADMINNAME");
                    model.ltlAdminEmail = Get("ADMINEMAIL");
                    model.ltlAdminRemarks = Get("ADMINREMARKS");
                    model.ltlAdminStatus = Get("STRADMINSTATUS");
                    model.ltlAdminAppDate = Get("ADMINAPPDATE");
                }
            }
            else
            {
                // Optional: show a message or return NotFound
                TempData["Info"] = "No records found for the requested ID.";
            }

            return View(model);
        }
        #endregion "Tour Application Details ASPX : Page Load End"  
        #endregion "TourApplicationDetails.Aspx"
        /*-----------------------------------------------------------------------------*/


        #region "TourBookingDetail.Aspx"
        #region "Tour Booking Details ASPX  Start : Page Load Start"
        [HttpGet]
        public IActionResult TourBookingDetail()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();

            string RequestID = Request.Query["id"];

            if (string.IsNullOrEmpty(UserID.ToString()))
            {
                return RedirectToAction("Login", "Account");
            }

            //GET DAYWISE DETAILS OF TOUR
            DataTable dt = oTourQueries.GetReqeustBookingList(RequestID);

            var model = new TourBookingDetailViewModel
            {
                gvDayWiseTourList = dtList.TableToList<gvDayWiseTourList>(oTourQueries.GetReqeustBookingList(RequestID))
            };


            return View(model);
        }
        #endregion "Tour Booking Details ASPX : Page Load End"  
        #endregion "TourBookingDetails.Aspx"
        /*-----------------------------------------------------------------------------*/


        #region "TourRequestApproval.Aspx"
        #region "Tour Request Approval ASPX  Start : Page Load Start"
        [HttpGet]
        public IActionResult TourRequestApproval()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();

            string RequestID = Request.Query["id"];
            string empcode = Request.Query["ecode"];

            if (string.IsNullOrEmpty(UserID.ToString()))
            {
                return RedirectToAction("Login", "Account");
            }
            //TempData["TourList"] = oTourList;
            //TempData["previous"] = oTourListpre;
            var model = new TourRequestApprovalViewModel{};
            model.pnlApproval = false;
            model.pnlRecomend = false;
            model.pnlAdvance = false;

            //FILL TOUR HEADER AND DETAILS
            FillRequestHeader_TRA(RequestID, model);
            FillRequestDetail_TRA(RequestID, model);
            FillPreTourDetail_TRA(RequestID, model);
            model.lblMsgVisible = model.pnlMsgVisible = false;

            //FILL PREVIOUS ADVANCE DETAILS
            FillAdvanceDetails_TRA(empcode, model);

            //GET NEW APPROVAL AUTHORITIES IN CASE OF RECOMMENDATION FOR SELECTED EMPLOYEE
            DataTable dt = new DataTable();
            DataTable dtAuthList = new DataTable();
            DataTable dtSAuthList = new DataTable();
            string ecode = UserID.ToString();
            string EmpPOS = string.Empty;
            string AppAuthCode = string.Empty;


            //GET USER APPROVAL AUTORITIES
            dtAuthList = oTourQueries.GetAppAuthorityList(ecode, empcode);

            //BIND APP AUTHORITY
            model.cboAppAuthorityList = dtAuthList.AsSelectList("ADEMPCODE", "EMPNAME", true, "-- select --", "0");

            //GET USER SPECAIL APPROVAL AUTORITIES
            dtSAuthList = oTourQueries.GetSpecialAppAuthorityList(ecode);
            model.cboSPAppAuthorityList = dtSAuthList.AsSelectList("ADEMPCODE", "EMPNAME", true, "-- select --", "0");


            //SELECT APPROPRIATE AUTHORITY
            dt = oTourQueries.GetAppAuthorities(ecode);

            if (dt.Rows.Count > 0)
            {
                EmpPOS = dt.Rows[0]["POS"].ToString().Trim();
                switch (EmpPOS)
                {
                    //IN CASE OF OTHER
                    case "OTH":
                        //FILL ALL SECTION HEAD OF USER DEPARTMENT
                        AppAuthCode = dt.Rows[0]["SECTIONHEADID"].ToString();
                        break;
                    //IN CASE OF SECTION MANAGER
                    case "SEC":
                        AppAuthCode = dt.Rows[0]["DEPARTMENTHEADID"].ToString();
                        break;
                    //IN CASE OF DEPARTMENT MANAGER
                    case "DPT":
                        AppAuthCode = dt.Rows[0]["DIVISIONHEADID"].ToString();
                        if (AppAuthCode == "")
                            AppAuthCode = dt.Rows[0]["VPHEADID"].ToString();
                        break;
                    //IN CASE OF DIVISIONAL MANAGER
                    case "DIV":
                        AppAuthCode = dt.Rows[0]["VPHEADID"].ToString();
                        break;
                }
            }
            DataSet objTaxiDs1 = new DataSet();
            DataView dvData1 = new DataView(dtAuthList);
            dvData1.RowFilter = "APPlvl in ('S2')";
            objTaxiDs1.Tables.Add(dvData1.ToTable());
            if (objTaxiDs1.Tables[0].Rows.Count > 0)
            {
                AppAuthCode = objTaxiDs1.Tables[0].Rows[0]["ADEMPCODE"].ToString();
            }

            //SELECT DEFAULT APPROVAL AUTHORITY
            model.cboAppAuthority = AppAuthCode;
            model.cboSPAppAuthority = AppAuthCode;
            TempData["REPORTINGAUTH"] = AppAuthCode; 

            //IN CASE OF RETURN BACK FOR MODIFICATION 
            if (Convert.ToString(TempData["RETURNSTATUS"]) == "3")
            {
                //APP AUTH CODE WHICH WAS MENTIONED IN THE REQEUST
                if (!string.IsNullOrEmpty(Convert.ToString(TempData["APPAUTH"])))
                {
                    model.cboAppAuthority = Convert.ToString(TempData["APPAUTH"]);
                }
                //SP APP AUTH CODE WHICH WAS MENTIONED IN THE REQEUST
                if (!string.IsNullOrEmpty(Convert.ToString(TempData["SPAPPAUTH"])))
                {
                    model.cboSPAppAuthority = Convert.ToString(TempData["SPAPPAUTH"]);
                    //cboSPAppAuthority = false;
                }
            }

            //IF NO SPECIAL APPROVAL IS REQUIRED OR ASSOCIATE IS ABOVE DIV HEAD
            //GET ASSOCIATE POSITION
            string AssociatePOS = string.Empty;
            dt = oTourQueries.GetAppAuthorities(empcode);
            AssociatePOS = dt.Rows[0]["POS"].ToString().Trim(); ;
            if (model.rblSPList == "0" || EmpPOS == "OP" || AssociatePOS == "OP")
                model.trSPVisible = false;
            else model.trSPVisible = true;

            /*Added by TTL on 18-Nov-2025 against SR111158 > CR7504 - Start*/
            model.pnlRecommendBtns = model.pnlRecomend;
            model.pnlApprovalBtns = model.pnlApproval;
            /*Added by TTL on 18-Nov-2025 against SR111158 > CR7504 - End*/

            return View(model);
        }
        #endregion "Tour Request Approval ASPX : Page Load End"  

        #region "Tour Request Approval ASPX  Start : Control Events Start"
        [HttpPost]
        public IActionResult GetNextRequestId(string id)
        {
            id = Request.Query["id"].ToString();
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var NextReqId = NextRequestIdbyEmpCode(id, UserID.ToString());
            return Json(new
            {
                nextReqId = NextReqId
            });

        }
        [HttpPost]
        public IActionResult CmdSubmit_Click_TRA([FromBody] SaveTourRequestApproval req)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();
            try
            {
                
                string errMsg = string.Empty;
                string errMsg1 = string.Empty;  

                //GET NEW APPROVAL AUTHORITY IN CASE OF RECOMMENDATION
                string AppAuthCode = string.Empty;
                string spAppAuthCode = string.Empty;

                //FOR APPROVAL
                if (req.authType.Trim() == "R" && req.appstatus == "1")
                    AppAuthCode = req.appAuthCode;
                //IF SPECIAL APPROVAL REQUIRED
                else if (req.authType.Trim() == "A" && req.rblSPList == "1" && req.appstatus == "1")
                {
                    spAppAuthCode = req.spAppAuthCode;
                    //IF CURRENT AUTHORITY IS SPECIAL APPROVAL AUTHORITY THEN DON'T GO FOR SPECIAL APPROVAL
                    DataTable dt = new DataTable();
                    string EmpPOS = string.Empty;
                    string AssociatePOS = string.Empty;
                    //GET LOGGED EMP POSITION
                    dt = oTourQueries.GetAppAuthorities(UserID.ToString());
                    EmpPOS = dt.Rows[0]["POS"].ToString().Trim();

                    //GET ASSOCIATE POSITION
                    dt = new DataTable();
                    dt = oTourQueries.GetAppAuthorities(req.EmpCode);
                    AssociatePOS = dt.Rows[0]["POS"].ToString().Trim();

                    if (EmpPOS == "OP" || AssociatePOS == "OP")
                        //spAppAuthCode = "";
                        spAppAuthCode = UserID.ToString();
                }
                if (req.appstatus == "1")
                {
                    DataSet ds = oTourBudget.GettourbudgetHis("", "", "", req.RequestID, "1", "1");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        string strrequiredamountvalue = Convert.ToString((Convert.ToDecimal(req.totalRequiredAmount) + Convert.ToDecimal(req.intthamount) + Convert.ToDecimal(req.approxTicketAmount)));
                        if (Convert.ToDecimal((string.IsNullOrEmpty(strrequiredamountvalue) ? "0" : strrequiredamountvalue)) > Convert.ToDecimal(string.IsNullOrEmpty(req.balanceBudget) ? "0" : req.balanceBudget.ToString()))
                        {
                            return Json(new
                            {
                                error = "",
                                budgetError = "Travel budget is not enough for this tour",
                                redirectToManage = false,
                                nextReqId = ""
                            });
                        }
                    }
                }
                 
                /*Added by TTL on 04-Oct-2025 against SR110729 > CR7297 Start*/
                errMsg = oTourQueries.UpdateApprovalStatus(req.authType, req.RequestID, req.appstatus, req.remarks, AppAuthCode, spAppAuthCode, UserID.ToString(), req.dir2EmpCode);
                /*Added by TTL on 04-Oct-2025 against SR110729 > CR7297 End*/
                if (errMsg.Trim() == "")
                {
                    if (req.appstatus == "2")
                    {
                        errMsg1 = oTourBudget.INSERTTRAVELBUDGETHIS("", "", req.reqOperationId.ToString(), req.RequestID, "1", req.totalRequiredAmount.ToString(), "2", UserID.ToString());
                    }
                    else
                    {
                        errMsg1 = oTourBudget.INSERTTRAVELBUDGETHIS("", "", req.reqOperationId.ToString(), req.RequestID, "1", req.totalRequiredAmount.ToString(), "1", UserID.ToString());
                    }
                    SendMail_TRA(req.RequestID, req.EmpCode, req.authType, req.strStatus, req.remarks, req.appstatus, req.rblSPList, req.trSPVisible, req.appAuthCode,req.spAppAuthCode);
                    //Start : Added By TTL SR101635 > CR6641 on 18/08/2025
                    var NextReqId = NextRequestIdbyEmpCode(req.RequestID, UserID.ToString());
                    if (string.IsNullOrEmpty(NextReqId))
                        return Json(new
                        {
                            error = "",
                            budgetError = "",
                            redirectToManage = true,
                            nextReqId = ""
                        });
                    else
                        return Json(new
                        {
                            error = "",
                            budgetError = "",
                            redirectToManage = false,
                            nextReqId = NextReqId
                        });
                    //End : Added By TTL SR101635 > CR6641 on 18/08/2025
                }
                else
                    return Json(new
                    {
                        error = errMsg,
                        budgetError = "",
                        redirectToManage = false,
                        nextReqId = "",
                        lblMsg = errMsg
                    }); 
            }
            catch {
                return Json(new
                {
                    error = "Unexpected error occurred.",
                    budgetError = "",
                    redirectToManage = false,
                    nextReqId = ""
                });
            }
        }
        #endregion "Tour Request Approval ASPX  Start : Control Events End"

        #region "Tour Request Approval ASPX  Start : Function Start"
        //CHECK THE DA CALCULATED OF SELECTED DATE  OR NOT
        private Boolean DailyAllowanceCaluculation(List<string> ProcessDates, string TravelDate)
        {
            foreach (string str in ProcessDates)
            {
                //PROCESSED
                if (str == TravelDate)
                    return true;
            }
            //NOT PROCESSED
            return false;
        }

        //CHECK THE STAY CHARGE CALCULATED OF SELECTED DATE  OR NOT
        private Boolean StayChargeCaluculation(List<string> ProcessNights, string TravelDate, string StayingCity)
        {
            foreach (string str in ProcessNights)
            {
                //PROCESSED AND STAYING CITY EXIST
                if (str == TravelDate && StayingCity.Trim() != "")
                    return true;
            }
            //NOT PROCESSED
            return false;
        }

        private void CalculateDayNights_TRA(TourRequestApprovalViewModel model)
        {
            string CityCode;
            string StayingCity = string.Empty;
            int rownum;
            int days;
            int nights;
            int chargeAP = 0;
            int chargeA = 0;
            int chargeB = 0;
            int chargeC = 0;
            int TotalNights;

            int allowAP = 0;
            int allowA = 0;
            int allowB = 0;
            int allowC = 0;
            int TotalDays;
            int EmpDesignationID;

            double NightCharge = 0;
            double TotalNightCharge = 0;
            double DailyAllowance = 0;
            double TotalDailyAllowance = 0;

            //CHECK STAY CHARGE/DAILY ALLOWANCE WHEN TOUR ADVANCE IS SELECTED
            if (model.chkAdvanceChecked == true && model.gvList.Any())
            {
                //SHOW ADVANCE PANEL
                model.pnlAdvance = true;
                string TravelDate = string.Empty;
                string CityCategory = string.Empty;
                int CityCategoryCode = 0;
                Boolean isDayCalculated = false;
                Boolean isNightCalculated = false;
                Tour oTour = new Tour();
                //  oTourList = (ArrayList)TempData["TourList"];
                var oTourList = model.TourList; //(ArrayList)TempData["TourList"];
                List<string> ProcessDates = new List<string>();
                List<string> ProcessNights = new List<string>();

                for (rownum = 0; rownum < oTourList.Count; rownum++)
                {
                    oTour = (Tour)oTourList[rownum];
                    days = 1;
                    nights = 1;
                    //GET TRAVEL DATE
                    TravelDate = oTour.TourFromDate;
                    //CHECK DATE ALREADY PROCESSED OR NOT
                    isDayCalculated = DailyAllowanceCaluculation(ProcessDates, TravelDate);
                    if (isDayCalculated == true)
                    {
                        days = 0;
                    }
                    else
                    {
                        ProcessDates.Add(TravelDate);
                    }

                    //GET STAYING CITY CODE
                    CityCode = oTour.StayingLocCode;
                    StayingCity = oTour.StayingLoc.Trim();

                    //CHECK NIGHT ALREADY PROCESSED OR NOT
                    isNightCalculated = StayChargeCaluculation(ProcessNights, TravelDate, StayingCity);
                    if (isNightCalculated == true)
                    {
                        nights = 0;
                    }
                    else
                    {
                        //CHECK IF STAYING CITY EXIST
                        if (StayingCity != "")
                            ProcessNights.Add(TravelDate);
                    }
                    //IF NO STAYING CITY IS THERE
                    if (StayingCity == "")
                    {
                        nights = 0;
                        //DAILY ALLOWANCES CALCULATED ON TO CITY CODE
                        CityCode = oTour.ToLocCode;
                    }
                    //IF CITYCODE IS 0 IE OTHER THEN SET IT TO BLANK BECAUSE IS DOESN'T STORE IN DATABASE
                    if (CityCode == "0")
                        CityCode = "";

                    //GET CATEGORY DESCRIPTION OF CITY (RETURN C IN CASE OF OTHER CITY(CITYCODE=0)
                    DataRow[] _datarow;
                    _datarow = oTourQueries.GetCityCategory(CityCode);
                    if (_datarow.Length > 0)
                    {
                        CityCategory = _datarow[0]["DESCRIP"].ToString();
                        CityCategoryCode = System.Convert.ToInt16(_datarow[0]["SYCITYCATEGORYID"]);
                    }

                    EmpDesignationID = System.Convert.ToInt16(TempData["EmpDesig"]);
                    // Added By Kishan Dodiya
                    string UserType = Convert.ToInt64(TempData["ADEMPCODE"]) > 70000000 ? "EXPAT" : "LOCAL";
                    //CALCULATE NIGHT CHARGE AND DAILY ALLOWANCE CITY WISE
                    _datarow = oTourQueries.GetEmployeeAllowanceDetail(CityCategoryCode, EmpDesignationID, TempData["AppDate"].ToString(), UserType);
                    // End Added
                    if (_datarow.Length > 0)
                    {
                        NightCharge = System.Convert.ToDouble(_datarow[0]["LODGINGAMT"]);
                        DailyAllowance = System.Convert.ToDouble(_datarow[0]["DAILYALLOWANCEAMT"]);
                    }
                    TotalDailyAllowance += (DailyAllowance * days);
                    TotalNightCharge += (NightCharge * nights);

                    switch (CityCategory)
                    {
                        case "A+":
                            chargeAP += nights;
                            allowAP += days;
                            break;
                        case "A":
                            chargeA += nights;
                            allowA += days;
                            break;
                        case "B":
                            chargeB += nights;
                            allowB += days;
                            break;
                        case "C":
                            chargeC += nights;
                            allowC += days;
                            break;
                    }
                }
                TotalNights = chargeAP + chargeA + chargeB + chargeC;
                TotalDays = allowAP + allowA + allowB + allowC;

                model.lblStayChargeAPLUS = chargeAP.ToString();
                model.lblAllownceAPLUS = allowAP.ToString();
                model.lblStayChargeA = chargeA.ToString();
                model.lblAllownceA = allowA.ToString();
                model.lblStayChargeB = chargeB.ToString();
                model.lblAllownceB = allowB.ToString();
                model.lblStayChargeC = chargeC.ToString();
                model.lblAllownceC = allowC.ToString();

                model.lblTotalNights = TotalNights.ToString();
                model.lblTotalDays = TotalDays.ToString();

                //CALCULATE TOTAL ALLOWANCES AND CHARGE
                model.lblTotalAllowances = string.Format("{0:F2}", TotalDailyAllowance);
                model.lblTotalCharges = string.Format("{0:F2}", TotalNightCharge);
            }
            else
            {
                //HIDE ADVANCE PANEL
                model.pnlAdvance = false;
            }
            double MiscAmount = 0;
            MiscAmount = System.Convert.ToDouble(model.lblMiscAmount);
            model.lblMiscAmount = string.Format("{0:F2}", MiscAmount);

            model.lblTotalAmount = string.Format("{0:F2}", (TotalDailyAllowance + TotalNightCharge + MiscAmount));
        }

        private void CalculateDayNightsForbudget_TRA(TourRequestApprovalViewModel model)
        {
            string CityCode;
            string StayingCity = string.Empty;
            int rownum;
            int days;
            int nights;
            int chargeAP = 0;
            int chargeA = 0;
            int chargeB = 0;
            int chargeC = 0;
            int TotalNights;

            int allowAP = 0;
            int allowA = 0;
            int allowB = 0;
            int allowC = 0;
            int TotalDays;
            int EmpDesignationID;

            double NightCharge = 0;
            double TotalNightCharge = 0;
            double DailyAllowance = 0;
            double TotalDailyAllowance = 0;

            //CHECK STAY CHARGE/DAILY ALLOWANCE WHEN TOUR ADVANCE IS SELECTED
            string TravelDate = string.Empty;
            string CityCategory = string.Empty;
            int CityCategoryCode = 0;
            Boolean isDayCalculated = false;
            Boolean isNightCalculated = false;
            Tour oTour = new Tour();
            //oTourList = (ArrayList)TempData["TourList"];
            var oTourList = model.TourList; //(ArrayList)TempData["TourList"];
            List<string> ProcessDates = new List<string>();
            List<string> ProcessNights = new List<string>();

            for (rownum = 0; rownum < oTourList.Count; rownum++)
            {
                oTour = (Tour)oTourList[rownum];
                days = 1;
                nights = 1;
                //GET TRAVEL DATE
                TravelDate = oTour.TourFromDate;
                //CHECK DATE ALREADY PROCESSED OR NOT
                isDayCalculated = DailyAllowanceCaluculation(ProcessDates, TravelDate);
                if (isDayCalculated == true)
                {
                    days = 0;
                }
                else
                {
                    ProcessDates.Add(TravelDate);
                }

                //GET STAYING CITY CODE
                CityCode = oTour.StayingLocCode;
                StayingCity = oTour.StayingLoc.Trim();

                //CHECK NIGHT ALREADY PROCESSED OR NOT
                isNightCalculated = StayChargeCaluculation(ProcessNights, TravelDate, StayingCity);
                if (isNightCalculated == true)
                {
                    nights = 0;
                }
                else
                {
                    //CHECK IF STAYING CITY EXIST
                    if (StayingCity != "")
                        ProcessNights.Add(TravelDate);
                }
                //IF NO STAYING CITY IS THERE
                if (StayingCity == "")
                {
                    nights = 0;
                    //DAILY ALLOWANCES CALCULATED ON TO CITY CODE
                    CityCode = oTour.ToLocCode;
                }
                //IF CITYCODE IS 0 IE OTHER THEN SET IT TO BLANK BECAUSE IS DOESN'T STORE IN DATABASE
                if (CityCode == "0")
                    CityCode = "";

                //GET CATEGORY DESCRIPTION OF CITY (RETURN C IN CASE OF OTHER CITY(CITYCODE=0)
                DataRow[] _datarow;
                _datarow = oTourQueries.GetCityCategory(CityCode);
                if (_datarow.Length > 0)
                {
                    CityCategory = _datarow[0]["DESCRIP"].ToString();
                    CityCategoryCode = System.Convert.ToInt16(_datarow[0]["SYCITYCATEGORYID"]);
                }

                EmpDesignationID = System.Convert.ToInt16(TempData["EmpDesig"]);
                // Added By Kishan Dodiya
                string UserType = Convert.ToInt64(TempData["ADEMPCODE"]) > 70000000 ? "EXPAT" : "LOCAL";
                //CALCULATE NIGHT CHARGE AND DAILY ALLOWANCE CITY WISE
                _datarow = oTourQueries.GetEmployeeAllowanceDetail(CityCategoryCode, EmpDesignationID, TempData["AppDate"].ToString(), UserType);
                // End Added
                if (_datarow.Length > 0)
                {
                    NightCharge = System.Convert.ToDouble(_datarow[0]["LODGINGAMT"]);
                    DailyAllowance = System.Convert.ToDouble(_datarow[0]["DAILYALLOWANCEAMT"]);
                }
                TotalDailyAllowance += (DailyAllowance * days);
                TotalNightCharge += (NightCharge * nights);

                switch (CityCategory)
                {
                    case "A+":
                        chargeAP += nights;
                        allowAP += days;
                        break;
                    case "A":
                        chargeA += nights;
                        allowA += days;
                        break;
                    case "B":
                        chargeB += nights;
                        allowB += days;
                        break;
                    case "C":
                        chargeC += nights;
                        allowC += days;
                        break;
                }
            }
            TotalNights = chargeAP + chargeA + chargeB + chargeC;
            TotalDays = allowAP + allowA + allowB + allowC;

            double MiscAmount = 0;
            MiscAmount = System.Convert.ToDouble(model.lblMiscAmount);
            model.hdntotalrequiredamount = string.Format("{0:F2}", (TotalDailyAllowance + TotalNightCharge + MiscAmount));
        }

        private void FillRequestHeader_TRA(string RequestID, TourRequestApprovalViewModel model)
        {
            DataTable dt = new DataTable();
            string AdvRequired = string.Empty;
            string UserCode = (_sessionService.Get<string>("userID")).ToString();
            string APPFlag = string.Empty;
            int ValidRequestDays;
            dt = oTourQueries.GetRequestHeaderPart(RequestID, UserCode);
            if (dt.Rows.Count > 0)
            {
                TempData["EmpDesig"] = dt.Rows[0]["ADDESIGNATIONID"].ToString();
                TempData["AppDate"] = dt.Rows[0]["APPLICATIONDATE"].ToString();
                TempData["ADEMPCODE"] = Convert.ToString(dt.Rows[0]["ADEMPCODE"]); // Added By Kishan Dodiya

                model.lblEmpName = dt.Rows[0]["EMPNAME"].ToString();
                model.txtMobile = dt.Rows[0]["MOBILENO"].ToString();
                model.txtExtension = dt.Rows[0]["EXTNNO"].ToString();
                model.txtEmail = dt.Rows[0]["ALTEMAILID"].ToString();
                model.txtObjective = dt.Rows[0]["OBJOFJOURNEY"].ToString();
                model.txtAdvRemarks = dt.Rows[0]["PREVIOUSTOURADVREMARKS"].ToString();
                AdvRequired = dt.Rows[0]["ISADVANCEREQ"].ToString();
                model.lblMiscAmount = dt.Rows[0]["MISCELLANEOUSAMT"].ToString();
                model.lblRequiredAmount = string.Format("{0:F2}", Convert.ToDouble(dt.Rows[0]["TOTALAMOUNTREQUIRED"]));
                model.lblMiscRemarks = dt.Rows[0]["MISCAMOUNTREMARKS"].ToString();
                APPFlag = dt.Rows[0]["APPFLAG"].ToString();
                model.lblRecEmpName = dt.Rows[0]["RECEMPNAME"].ToString();
                model.lblRecomRemarks = dt.Rows[0]["RECREMAKRS"].ToString();
                model.lblAppEmpName = dt.Rows[0]["APPEMPNAME"].ToString();
                model.lblAppRemarks = dt.Rows[0]["APPREMARKS"].ToString();
                model.rblSPList = dt.Rows[0]["SPECIALAPPROVAL"].ToString();
                /*Added by TTL on 06-Oct-2025 against SR110729 > CR7297 Start | Mapping corrected, It was REMARKS (TOURREQUEST DETAIL REMARKS)*/
                model.lblSPAppRemarks = dt.Rows[0]["SPAPPREMAKRS"].ToString();
                /*Added by TTL on 06-Oct-2025 against SR110729 > CR7297 End*/
                model.lblRequestDate = dt.Rows[0]["REQUESTDATE"].ToString();
                model.lblapproxticketamt = dt.Rows[0]["Flightamt"].ToString();



                //APPROVAL AUTHORITIES
                TempData["RECAUTH"] = dt.Rows[0]["RECADEMPCODE"].ToString();
                TempData["APPAUTH"] = dt.Rows[0]["APPADEMPCODE"].ToString();
                TempData["SPAPPAUTH"] = dt.Rows[0]["SPAPPADEMPCODE"].ToString();
                /*Added by TTL on 03-Oct-2025 against SR110729 > CR7297 Start*/
                TempData["DIR2_ADEMPCODE"] = UserCode == dt.Rows[0]["DIR2_ADEMPCODE"].ToString() ? "" : dt.Rows[0]["DIR2_ADEMPCODE"].ToString();
                /*Added by TTL on 03-Oct-2025 against SR110729 > CR7297 End*/
                TempData["RETURNSTATUS"] = dt.Rows[0]["RETURNSTATUS"].ToString();

                //SHOW RETURN BACK REMARKS
                model.lblReturnRemarks = dt.Rows[0]["RETURNREMAKRS"].ToString();
                if (dt.Rows[0]["RETURNREMAKRS"].ToString().Trim() == "")
                    model.trReturnVisible = false;
                else
                    model.trReturnVisible = true;

                ValidRequestDays = System.Convert.ToInt16(dt.Rows[0]["VALIDREQUESTDAYS"]);
                //SET APPROVAL TYPE
                model.hfAuthType = APPFlag;
                /*Added by TTL on 03-Oct-2025 against SR110729 > CR7297 Start*/
                model.hfDir2EmpCode = UserCode == dt.Rows[0]["DIR2_ADEMPCODE"].ToString() ? "" : dt.Rows[0]["DIR2_ADEMPCODE"].ToString();
                /*Added by TTL on 03-Oct-2025 against SR110729 > CR7297 End*/
                if (AdvRequired == "1")
                {
                    model.lblAdvAmt = "Yes";
                    model.chkAdvanceChecked = true;
                    model.pnlAdvance = true;
                }
                else
                {
                    model.lblAdvAmt = "No";
                    model.chkAdvanceChecked = false;
                    model.pnlAdvance = false;
                }

                if (APPFlag == "R")
                    model.pnlRecomend = true;
                else
                {
                    model.pnlRecomend = false;
                    model.pnlApproval = true;
                    if (APPFlag == "A")
                    {
                        model.trAppByVisible = false;
                        model.trAppRemarksVisible = false;
                    }
                    else
                    {
                        model.trAppByVisible = true;
                        model.trAppRemarksVisible = true;
                    }
                }


                //SPECIAL REQUEST IN CASE OF REQUEST GENERATED BEFORE 3 DAYS OF ACTUAL TRAVLEL.
                if (ValidRequestDays <= 3 && model.rblSPList == "1" && model.lblSPAppRemarks.Trim() == "")
                    model.lblSPAppRemarks = "Need special approval because request generated before 3 days of actual travel.";

                //Travel Budget details
                string stropid = dt.Rows[0]["OPERATIONID"].ToString();
                model.hdnReqoperationid = stropid;
            }
        }
        private void FillRequestDetail_TRA(string RequestID, TourRequestApprovalViewModel model)
        {

            //oTourList = oTourQueries.GetRequestDetailPart(RequestID);

            var _tourList = oTourQueries.GetRequestDetailPart(RequestID);

            List<Tour> tourList = _tourList.Cast<Tour>().ToList();
            model.TourList = tourList;
            DataTable dtTour = dtList.ArrayListToDataTable<Tour>(
                new ArrayList(tourList)
            );

            model.gvList = dtList.TableToList<gvList>(dtTour);


            //TempData["TourList"] = oTourList;
            //DataTable dt = dtList.ArrayListToDataTable<Tour>(oTourList);

            //model.gvList = dtList.TableToList<gvList>(dt);

            //CALCULATE STAY CHARGE AND DAILY ALLOWANCES
            CalculateDayNights_TRA(model);
            CalculateDayNightsForbudget_TRA(model);

            //Travel Budget details
            DataSet ds = oTourBudget.Gettourbudget("", "", model.hdnReqoperationid, "1", RequestID);
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.lbltravelplan = ds.Tables[0].Rows[0]["PLAN"].ToString();
                model.lbltravelactual = ds.Tables[0].Rows[0]["ACTUAL"].ToString();
                model.lblbalancebudget = ds.Tables[0].Rows[0]["TRAVELBALANCE"].ToString();
                model.lblbudgetconsumed = ds.Tables[0].Rows[0]["BUDGETCONSUMED"].ToString();
                DataSet ds1 = oTourBudget.GettourbudgetHis("", "", "", RequestID, "1", "1");
                if (ds1.Tables[0].Rows.Count == 0)
                {
                    model.lblbalancebudgetafttour = Convert.ToString((Convert.ToDecimal(model.lblbalancebudget) - (Convert.ToDecimal(model.hdntotalrequiredamount) + Convert.ToDecimal(model.lblapproxticketamt))));
                }
                else
                {
                    model.lblbalancebudgetafttour = model.lblbalancebudget.ToString();

                }
            }
            model.lbltotaltouramt = Convert.ToString((Convert.ToDecimal(model.hdntotalrequiredamount) + Convert.ToDecimal(model.lblapproxticketamt)));



        }
        private void FillPreTourDetail_TRA(string RequestID, TourRequestApprovalViewModel model)
        {
            //ArrayList dt = oTourQueries.GetRequestpreDetail(RequestID);
            //TempData["previous"] = dt;

            //DataTable dtPT = new DataTable();
            //dtPT = dtList.ArrayListToDataTable<TourAdvance>(dt);
            //model.grdprvadv = dtList.TableToList<grdprvadv>(dtPT);



            var _tourList = oTourQueries.GetRequestpreDetail(RequestID);

            List<TourAdvance> tourList = _tourList.Cast<TourAdvance>().ToList();
            model.TourAdvacneList = tourList;
            DataTable dtTour = dtList.ArrayListToDataTable<TourAdvance>(
                new ArrayList(tourList)
            );

            model.grdprvadv = dtList.TableToList<grdprvadv>(dtTour);
        }
        private void FillAdvanceDetails_TRA(string UserID, TourRequestApprovalViewModel model)
        {
            ArrayList oPrevTourList = new ArrayList();
            DataTable dt = oTourQueries.GetTouradvancedata(UserID);


            string Ecode = string.Empty;
            string Period = string.Empty;
            string Amount = string.Empty;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; dt.Rows.Count > i; i++)
                {
                    Ecode = dt.Rows[i]["ECODE"].ToString();
                    Period = dt.Rows[i]["TOURPERIOD"].ToString();
                    Amount = dt.Rows[i]["TOURAMOUNT"].ToString();
                    //IF CURRENT RECORD IS USER RECORD THEN
                    Tour oTour = new Tour();
                    oTour.TourPeriod = Period;
                    oTour.Amount = Amount;
                    if (Period.Length == 17)
                    {
                        if (oTour.Amount.IndexOf("-") == -1)
                        {
                            Int32 day = Convert.ToInt32(((DateTime.Parse(DateTime.Today.ToString())) - ((DateTime.ParseExact(Period.Substring(9, 8), "dd.MM.yy", null)).AddDays(7))).TotalDays);
                            if (day > 0)
                                oTour.Day = day.ToString();
                            else
                                oTour.Day = "";


                            otourperiod.Add(oTour);
                        }
                    }
                    oPrevTourList.Add(oTour);
                }
            }
            dt = dtList.ArrayListToDataTable<Tour>(oPrevTourList);
            model.gvAdvance = dtList.TableToList<gvAdvance>(dt);
        }
        protected string NextRequestIdbyEmpCode(string currentRequestId, string ecode)
        {
            var PendingReqests = oTourQueries.GetPendingApprovalList(ecode);

            var nextId = PendingReqests.AsEnumerable()
                .Select(row => row.Field<Int64>("ADTOURREQUESTID"))
                .OrderBy(id => id)
                .Where(id => id > Convert.ToInt64(currentRequestId))
                .FirstOrDefault();
            if(nextId == 0 && PendingReqests.Rows.Count > 0)
            {
                nextId = PendingReqests.AsEnumerable()
                .Select(row => row.Field<Int64>("ADTOURREQUESTID"))
                .OrderBy(id => id).FirstOrDefault();
            }
            var NextReqId = (nextId > 0) ? nextId.ToString() : "";
            return NextReqId;
        }

        private void SendMail_TRA(string RequestID, string EmpCode, string AuthType, string strStatus, string Remarks, string appstatus, string rblSPList, string trSPVisible, string appAuthCode,string spAppAuthCode)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            DataTable dtTour = new DataTable();
            DataTable dt = new DataTable();

            //GET APP AUTH LIST
            dt = oTourQueries.GetAppAuthorities(EmpCode);
            //GET TOUR DETAILS
            dtTour = oTourQueries.GetRequestDetail(RequestID);

            //SEND MAIL TO USER FOR CONFIRMATION
            if (serverpath.isTestServer())
                sendMail.MailTo = serverpath.getTestEMail();
            else
                sendMail.MailTo = dt.Rows[0]["EMAILID"].ToString();

            string strSubject = "Tour Approval Status - " + strStatus;
            string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                             "</td></tr>" +
                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                             "</td></tr><tr height=150><td colspan=2>" +
                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td valign=top colspan=2><b>Your Tour request has been <b>" + strStatus + "</b> by " + UserName + " San. The details are as follows:</td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td valign=top width='30%'>Tour Objective:</td><td valign=top >" + dtTour.Rows[0]["OBJOFJOURNEY"].ToString() + "</td></tr>" +
                             "<tr><td valign=top >Period:</td><td valign=top >" + dtTour.Rows[0]["PERIOD"].ToString() + "</td></tr>" +
                             "<tr><td valign=top >Days:</td><td valign=top >" + dtTour.Rows[0]["DAYS"].ToString() + "</td></tr>";
            if (AuthType == "R")
            {
                strBody = strBody + "<tr><td valign=top >Recommended By:</td><td valign=top >" + dtTour.Rows[0]["RECAUTH"].ToString() + "</td></tr>";
            }
            else if (AuthType == "A")
            {
                strBody = strBody + "<tr><td valign=top >Approved By:</td><td valign=top >" + dtTour.Rows[0]["APPAUTH"].ToString() + "</td></tr>";
            }
            else
            {
                strBody = strBody + "<tr><td valign=top >Recommended By:</td><td valign=top >" + dtTour.Rows[0]["RECAUTH"].ToString() + "</td></tr>" +
                                    "<tr><td valign=top >Approved By:</td><td valign=top >" + dtTour.Rows[0]["APPAUTH"].ToString() + "</td></tr>";
            }
            strBody = strBody + "<tr><td valign=top >Remarks:</td><td valign=top >" + Remarks.ToString() + "</td></tr>" +
                                "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                             "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                             "</tr></table> ";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
            }
            finally
            {

            }

            //SEND MAIL TO APPROVE AUTHORITY(AFTER RECOMMENDED BY AUTHORITY) IN CASE OF APPROVAL THE REQUEST
            if (AuthType == "R" && appstatus == "1")
            {
                string AppAuthCode = appAuthCode;
                //SEND MAIL TO SELECTED APPROVAL AUTHORITY
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    string Position = string.Empty;
                    string AppAuthEmail = string.Empty;

                    dt = new DataTable();
                    dt = oTourQueries.GetAppAuthorities(AppAuthCode);
                    if (dt.Rows.Count > 0)
                    {
                        AppAuthEmail = dt.Rows[0]["EMAILID"].ToString();
                    }
                    sendMail.MailTo = AppAuthEmail;
                }

                strSubject = "Tour Request from - " + dtTour.Rows[0]["EMPNAME"].ToString();
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "</td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td valign=top colspan=2><b>" + dtTour.Rows[0]["EMPNAME"].ToString() + " San has submitted a Tour Request. The details are as follows:</b></td></tr>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td valign=top width='30%'>Tour Objective:</td><td valign=top >" + dtTour.Rows[0]["OBJOFJOURNEY"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Period:</td><td valign=top >" + dtTour.Rows[0]["PERIOD"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Days:</td><td valign=top >" + dtTour.Rows[0]["DAYS"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Recommended By:</td><td valign=top >" + dtTour.Rows[0]["RECAUTH"].ToString() + "</td></tr>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "Aspxview/TourRequest/TourRequestApproval.aspx?id=" + RequestID + "&ecode=" + EmpCode + " > Employee Portal</a> for your approval.</td></tr>" +
                                 "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated Email, please do not reply.</strong></td> " +
                                 "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                 "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                 "</tr></table> ";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                try
                {
                    bool status = sendMail.Send();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                }
                finally
                {

                }

                //IF SEND APPROVAL FOR OTHER AUTHORITY INSTEAD OF DEFALULT ONE THEN SEND MAIL ALSO TO DEFAUTH AUTHORITY
                if (Convert.ToString(TempData["REPORTINGAUTH"]) != AppAuthCode && !string.IsNullOrEmpty(Convert.ToString(TempData["REPORTINGAUTH"])))
                {
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                    {
                        string Position = string.Empty;
                        string AppAuthEmail = string.Empty;

                        dt = new DataTable();
                        dt = oTourQueries.GetAppAuthorities(TempData["REPORTINGAUTH"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            AppAuthEmail = dt.Rows[0]["EMAILID"].ToString();
                        }
                        sendMail.MailTo = AppAuthEmail;
                    }

                    strSubject = "Tour Request from - " + dtTour.Rows[0]["EMPNAME"].ToString();
                    strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                     "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                     "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                     "</td></tr>" +
                                     "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                     "</td></tr><tr height=150><td colspan=2>" +
                                     "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                     "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                     "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                     "<tr><td colspan=2>&nbsp;</td></tr>" +
                                     "<tr><td valign=top colspan=2><b>" + dtTour.Rows[0]["EMPNAME"].ToString() + " San has submitted a Tour Request. The details are as follows:</b></td></tr>" +
                                     "<tr><td colspan=2>&nbsp;</td></tr>" +
                                     "<tr><td valign=top width='30%'>Tour Objective:</td><td valign=top >" + dtTour.Rows[0]["OBJOFJOURNEY"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Period:</td><td valign=top >" + dtTour.Rows[0]["PERIOD"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Days:</td><td valign=top >" + dtTour.Rows[0]["DAYS"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Recommended By:</td><td valign=top >" + dtTour.Rows[0]["RECAUTH"].ToString() + "</td></tr>" +
                                     "<tr><td colspan=2>&nbsp;</td></tr>" +
                                     "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "Aspxview/TourRequest/TourRequestApproval.aspx?id=" + RequestID + "&ecode=" + EmpCode + "> Employee Portal</a> for your approval.</td></tr>" +
                                     "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated Email, please do not reply.</strong></td> " +
                                     "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                     "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                     "</tr></table> ";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                    }
                    finally
                    {

                    }
                }
            }

            //SEND MAIL TO APPROVE AUTHORITY(IN CASE OF SPECIAL APPROVAL)
            if (AuthType == "A" && rblSPList == "1" && appstatus == "1" && trSPVisible == "1")
            {
                string AppAuthCode = spAppAuthCode;
                //SEND MAIL TO SELECTED SP APP AUTHORITY
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    string Position = string.Empty;

                    string AppAuthEmail = string.Empty;

                    dt = new DataTable();
                    dt = oTourQueries.GetAppAuthorities(AppAuthCode);
                    if (dt.Rows.Count > 0)
                    {
                        AppAuthEmail = dt.Rows[0]["EMAILID"].ToString();
                    }
                    sendMail.MailTo = AppAuthEmail;
                }

                strSubject = "Tour Request for Special Approval from - " + dtTour.Rows[0]["EMPNAME"].ToString();
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "</td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td valign=top colspan=2><b>Tour Request for Special Approval from " + dtTour.Rows[0]["EMPNAME"].ToString() + ")</b></td></tr>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td valign=top width='30%'>Tour Objective:</td><td valign=top >" + dtTour.Rows[0]["OBJOFJOURNEY"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Period:</td><td valign=top >" + dtTour.Rows[0]["PERIOD"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Days:</td><td valign=top >" + dtTour.Rows[0]["DAYS"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Recommended By:</td><td valign=top >" + dtTour.Rows[0]["RECAUTH"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Approved By:</td><td valign=top >" + dtTour.Rows[0]["APPAUTH"].ToString() + "</td></tr>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "Aspxview/TourRequest/TourRequestApproval.aspx?id=" + RequestID + "&ecode=" + EmpCode + "> Employee Portal</a> for your approval.</td></tr>" +
                                 "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                 "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                 "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                 "</tr></table> ";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                try
                {
                    bool status = sendMail.Send();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                }
                finally
                {

                }

                //IF SEND SP APPROVAL FOR OTHER AUTHORITY INSTEAD OF DEFALULT ONE THEN SEND MAIL ALSO TO DEFAUTH AUTHORITY
                if (Convert.ToString(TempData["REPORTINGAUTH"]) != AppAuthCode && !string.IsNullOrEmpty(Convert.ToString(TempData["REPORTINGAUTH"])))
                {
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                    {
                        string Position = string.Empty;
                        string AppAuthEmail = string.Empty;

                        dt = new DataTable();
                        dt = oTourQueries.GetAppAuthorities(TempData["REPORTINGAUTH"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            AppAuthEmail = dt.Rows[0]["EMAILID"].ToString();
                        }
                        sendMail.MailTo = AppAuthEmail;
                    }

                    strSubject = "Tour Request for Special Approval from - " + dtTour.Rows[0]["EMPNAME"].ToString();
                    strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                     "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                     "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                     "</td></tr>" +
                                     "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                     "</td></tr><tr height=150><td colspan=2>" +
                                     "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                     "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                     "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                     "<tr><td colspan=2>&nbsp;</td></tr>" +
                                     "<tr><td valign=top colspan=2><b>Tour Request for Special Approval from " + dtTour.Rows[0]["EMPNAME"].ToString() + ")</b></td></tr>" +
                                     "<tr><td colspan=2>&nbsp;</td></tr>" +
                                     "<tr><td valign=top width='30%'>Tour Objective:</td><td valign=top >" + dtTour.Rows[0]["OBJOFJOURNEY"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Period:</td><td valign=top >" + dtTour.Rows[0]["PERIOD"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Days:</td><td valign=top >" + dtTour.Rows[0]["DAYS"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Recommended By:</td><td valign=top >" + dtTour.Rows[0]["RECAUTH"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Approved By:</td><td valign=top >" + dtTour.Rows[0]["APPAUTH"].ToString() + "</td></tr>" +
                                     "<tr><td colspan=2>&nbsp;</td></tr>" +
                                     "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "Aspxview/TourRequest/TourRequestApproval.aspx?id=" + RequestID + "&ecode=" + EmpCode + "> Employee Portal</a> for your approval.</td></tr>" +
                                     "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                     "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                     "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                     "</tr></table> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                    }
                    finally
                    {

                    }
                }

            }

            //SEND MAIL TO RECOMMENDED AUTHORITY AND APPROVAL AUTHORITY(IN CASE OF RETURN BACK FROM APPROVAL AUTHORITY)
            if (AuthType == "SP" && appstatus == "3")
            {
                //APPROVAL AUTHORITY
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    string AppAuthCode = Convert.ToString(TempData["APPAUTH"]);
                    string AppAuthEmail = string.Empty;

                    dt = new DataTable();
                    dt = oTourQueries.GetAppAuthorities(AppAuthCode);
                    if (dt.Rows.Count > 0)
                        AppAuthEmail = dt.Rows[0]["EMAILID"].ToString();
                    sendMail.MailTo = AppAuthEmail;
                }

                strSubject = "Tour Request of " + dtTour.Rows[0]["EMPNAME"].ToString() + " return for modification from - " + UserName + "San";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "</td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td valign=top colspan=2><b>Tour Request retrun for modification from " + UserName + "San.</b></td></tr>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td valign=top width='30%'>Associate:</td><td valign=top >" + dtTour.Rows[0]["EMPNAME"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Tour Objective:</td><td valign=top >" + dtTour.Rows[0]["OBJOFJOURNEY"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Period:</td><td valign=top >" + dtTour.Rows[0]["PERIOD"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Days:</td><td valign=top >" + dtTour.Rows[0]["DAYS"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Recommended By:</td><td valign=top >" + dtTour.Rows[0]["RECAUTH"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Approved By:</td><td valign=top >" + dtTour.Rows[0]["APPAUTH"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Remarks:</td><td valign=top >" + Remarks.ToString() + "</td></tr>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "Aspxview/TourRequest/TourRequestApproval.aspx?id=" + RequestID + "&ecode=" + EmpCode + "> Employee Portal</a> for your approval.</td></tr>" +
                                 "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                 "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                 "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                 "</tr></table> ";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                try
                {
                    bool status = sendMail.Send();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                }
                finally
                {

                }


                //RECOMMENDED AUTHORITY
                if (!string.IsNullOrEmpty(Convert.ToString(TempData["RECAUTH"])))
                {
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                    {
                        string AppAuthCode = Convert.ToString(TempData["RECAUTH"]);
                        string AppAuthEmail = string.Empty;

                        dt = new DataTable();
                        dt = oTourQueries.GetAppAuthorities(AppAuthCode);
                        if (dt.Rows.Count > 0)
                            AppAuthEmail = dt.Rows[0]["EMAILID"].ToString();
                        sendMail.MailTo = AppAuthEmail;
                    }

                    strSubject = "Tour Request of " + dtTour.Rows[0]["EMPNAME"].ToString() + " return for modification from - " + UserName.ToString() + "San";
                    strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                     "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                     "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                     "</td></tr>" +
                                     "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                     "</td></tr><tr height=150><td colspan=2>" +
                                     "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                     "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                     "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                     "<tr><td colspan=2>&nbsp;</td></tr>" +
                                     "<tr><td valign=top colspan=2><b>Tour Request retrun for modification from " + UserName + "San.</b></td></tr>" +
                                     "<tr><td colspan=2>&nbsp;</td></tr>" +
                                     "<tr><td valign=top width='30%'>Associate:</td><td valign=top >" + dtTour.Rows[0]["EMPNAME"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Tour Objective:</td><td valign=top >" + dtTour.Rows[0]["OBJOFJOURNEY"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Period:</td><td valign=top >" + dtTour.Rows[0]["PERIOD"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Days:</td><td valign=top >" + dtTour.Rows[0]["DAYS"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Recommended By:</td><td valign=top >" + dtTour.Rows[0]["RECAUTH"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Approved By:</td><td valign=top >" + dtTour.Rows[0]["APPAUTH"].ToString() + "</td></tr>" +
                                     "<tr><td valign=top >Remarks:</td><td valign=top >" + Remarks.ToString() + "</td></tr>" +
                                     "<tr><td colspan=2>&nbsp;</td></tr>" +
                                     "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "Aspxview/TourRequest/TourRequestApproval.aspx?id=" + RequestID + "&ecode=" + EmpCode + "> Employee Portal</a> for your approval.</td></tr>" +
                                     "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                     "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                     "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                     "</tr></table> ";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                                            "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                                            MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                    }
                    finally
                    {
                    }
                }
            }


            //SEND MAIL TO RECOMMENDED AUTHORITY(IN CASE OF RETURN BACK FROM APPROVAL AUTHORITY)
            if (AuthType == "A" && appstatus == "3" && !string.IsNullOrEmpty(Convert.ToString(TempData["RECAUTH"])))
            {
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    string AppAuthCode = Convert.ToString(TempData["RECAUTH"]);
                    string AppAuthEmail = string.Empty;

                    dt = new DataTable();
                    dt = oTourQueries.GetAppAuthorities(AppAuthCode);
                    if (dt.Rows.Count > 0)
                        AppAuthEmail = dt.Rows[0]["EMAILID"].ToString();
                    sendMail.MailTo = AppAuthEmail;
                }

                strSubject = "Tour Request of " + dtTour.Rows[0]["EMPNAME"].ToString() + " return for modification from - " + UserName + "San";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "</td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td valign=top colspan=2><b>Tour Request retrun for modification from " + UserName + " San.</b></td></tr>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td valign=top width='30%'>Associate:</td><td valign=top >" + dtTour.Rows[0]["EMPNAME"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Tour Objective:</td><td valign=top >" + dtTour.Rows[0]["OBJOFJOURNEY"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Period:</td><td valign=top >" + dtTour.Rows[0]["PERIOD"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Days:</td><td valign=top >" + dtTour.Rows[0]["DAYS"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Recommended By:</td><td valign=top >" + dtTour.Rows[0]["RECAUTH"].ToString() + "</td></tr>" +
                                 "<tr><td valign=top >Remarks:</td><td valign=top >" + Remarks.ToString() + "</td></tr>" +
                                 "<tr><td colspan=2>&nbsp;</td></tr>" +
                                 "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "Aspxview/TourRequest/TourRequestApproval.aspx?id=" + RequestID + "&ecode=" + EmpCode + "> Employee Portal</a> for your approval.</td></tr>" +
                                 "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                 "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                 "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                 "</tr></table> ";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                try
                {
                    bool status = sendMail.Send();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                }
                finally
                {

                }
            }

        }
        #endregion "Tour Request Approval ASPX  End : Function End"
        #endregion "TourRequestApproval.Aspx"
        /*-----------------------------------------------------------------------------*/


        #region "TourAuthChange.Aspx"
        #region "Tour Auth Change ASPX  Start : Page Load Start"
        [HttpGet]
        public IActionResult TourAuthChange()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();
            var strId = HttpContext.Request.Query["tid"].ToString();
            //var decrypted = Encryption.Decrypt(tid);   
            //var strId = WebUtility.UrlDecode(decrypted);

            //IF QUERYSTRING IS NULL THEN 
            if (string.IsNullOrEmpty(strId))
            {
                return RedirectToAction("Home", "Home");
            }
            if (string.IsNullOrEmpty(UserID.ToString()))
            {
                return RedirectToAction("Index", "Login");
            }
            var model = new TourAuthChangeViewModel
            {
            };
            FillRequestHeader_TAC(strId, model);
            FillApprovalStatus_TAC(strId, model);
            FillRequestDetails_TAC(strId, model);
            //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "none"); 
            return View(model);
        }
        #endregion "Tour Auth Change ASPX : Page Load End"  

        #region "Tour Auth Change ASPX  Start : Control Events Start"
        [HttpPost]
        public IActionResult BtnSave_Click(string txtRemarks, string cboRecAuthority, string cboAppAuthority, string cboSpAppAuthority)
        {
            string strStatus = string.Empty;
            string strNewRecAuth = string.Empty;
            string strNewAppAuth = string.Empty;
            string strNewSpAppAuth = string.Empty;

            string strOldRecAuth = string.Empty;
            string strOldAppAuth = string.Empty;
            string strOldSpAppAuth = string.Empty;

            string strEmpCode = string.Empty;
            string strRemarks = string.Empty;

            var tid = HttpContext.Request.Query["tid"].ToString();
            var decrypted = Encryption.Decrypt(tid);
            var strId = WebUtility.UrlDecode(decrypted);

            DataTable dt = new DataTable();
            dt = oTourQueries.GetChangeAppAuth(strId);
            strRemarks = txtRemarks;
            if (string.IsNullOrEmpty(strRemarks))
                strRemarks = "Unavailability";

            if (dt.Rows.Count > 0)
            {
                strEmpCode = dt.Rows[0]["ADEMPCODE"].ToString().Trim();
                //CHECK RECOMMENDED STATUS IF RECOMMENDED AUTHORITY EXIST
                if (dt.Rows[0]["RECSTATUSDATA"].ToString().Trim() == "0" && dt.Rows[0]["Recadempcode"].ToString().Trim() != "")
                {
                    if (cboRecAuthority == "0")
                    {
                        //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline"); 
                        return StatusCode(500, new { error = "Please select recommendation authority from dropdown." });
                    }
                    strNewRecAuth = cboRecAuthority;
                    strOldRecAuth = dt.Rows[0]["Recadempcode"].ToString().Trim();
                }
                else if (dt.Rows[0]["RECSTATUSDATA"].ToString().Trim() == "1")
                {
                    strOldRecAuth = dt.Rows[0]["Recadempcode"].ToString();
                    strNewRecAuth = dt.Rows[0]["Recadempcode"].ToString();
                }
                //CHECK APPROVAL AUTHORITY WHEN REQUEST RECOMMENDED OR REQUEST WITHOUT RECOMMENDATION
                if ((dt.Rows[0]["RECSTATUSDATA"].ToString().Trim() == "1" || dt.Rows[0]["Recadempcode"].ToString().Trim() == "") && dt.Rows[0]["APPSTATUSDATA"].ToString().Trim() == "0")
                {
                    strNewAppAuth = cboAppAuthority;
                    strOldAppAuth = dt.Rows[0]["Appadempcode"].ToString().Trim();
                    if (cboAppAuthority == "0")
                    {
                        // errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline"); 
                        return StatusCode(500, new { error = "Please select approval authority from dropdown." });
                    }
                }
                else if (dt.Rows[0]["APPSTATUSDATA"].ToString().Trim() == "1")
                {
                    strOldAppAuth = dt.Rows[0]["Appadempcode"].ToString();
                    strNewAppAuth = dt.Rows[0]["Appadempcode"].ToString();
                }

                if ((dt.Rows[0]["RECSTATUSDATA"].ToString().Trim() == "1" || dt.Rows[0]["Recadempcode"].ToString().Trim() == "") && dt.Rows[0]["APPSTATUSDATA"].ToString().Trim() == "1" && dt.Rows[0]["SpappstatusDATA"].ToString().Trim() == "0")
                {
                    strNewSpAppAuth = cboSpAppAuthority;
                    strOldSpAppAuth = dt.Rows[0]["Spappadempcode"].ToString().Trim();
                    if (cboSpAppAuthority == "0")
                    {
                        //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline");
                        return StatusCode(500, new { error = "Please select special approval authority from dropdown." });
                    }
                }
                //UPDATE NEW APPROVAL AUTHORITY
                strStatus = oTourQueries.UpdateApprovalAuthority(strOldRecAuth, strNewRecAuth, strId, strOldAppAuth, strNewAppAuth, strOldSpAppAuth, strNewSpAppAuth, UserID.ToString(), strRemarks);

                if (strStatus != "")
                {
                    //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline");
                    return StatusCode(500, new { error = strStatus });
                }
                else
                {
                    //SEND MAIL TO APPROVAL AUTHORITY

                    //IF RECOMMENDATION IS PENDING AND RECOMMENDED AUTHORITY EXIST
                    if (dt.Rows[0]["RECSTATUSDATA"].ToString().Trim() == "0" && dt.Rows[0]["Recadempcode"].ToString().Trim() != "")
                    {
                        if (strOldRecAuth != strNewRecAuth)
                            EmailStatus_TAC(strOldRecAuth, strNewRecAuth, strEmpCode, strId, dt.Rows[0]["OBJOFJOURNEY"].ToString().Trim(), dt.Rows[0]["TOURSTARTDATE"].ToString().Trim(), dt.Rows[0]["TOURENDDATE"].ToString().Trim(), strRemarks);
                    }
                    //IF APPROVAL AUTHIRY EXIST
                    else if (dt.Rows[0]["APPSTATUSDATA"].ToString().Trim() == "0")
                    {
                        if (strOldAppAuth != strNewAppAuth)
                            EmailStatus_TAC(strOldAppAuth, strNewAppAuth, strEmpCode, strId, dt.Rows[0]["OBJOFJOURNEY"].ToString().Trim(), dt.Rows[0]["TOURSTARTDATE"].ToString().Trim(), dt.Rows[0]["TOURENDDATE"].ToString().Trim(), strRemarks);
                    }
                    //FOR SPECIAL APPROVAL
                    else if (dt.Rows[0]["SpappstatusDATA"].ToString().Trim() == "0")
                    {
                        if (strOldSpAppAuth != strNewSpAppAuth)
                            EmailStatus_TAC(strOldSpAppAuth, strNewSpAppAuth, strEmpCode, strId, dt.Rows[0]["OBJOFJOURNEY"].ToString().Trim(), dt.Rows[0]["TOURSTARTDATE"].ToString().Trim(), dt.Rows[0]["TOURENDDATE"].ToString().Trim(), strRemarks);
                    }

                    return StatusCode(200, new { success = "Ok" });
                }
            }

            return StatusCode(200, new { success = "Ok" });
        }

        #endregion "Tour Auth Change ASPX  Start : Control Events End"

        #region "Tour Auth Change ASPX  Start : Function Start"
        private void FillRequestHeader_TAC(string RequestID, TourAuthChangeViewModel model)
        {
            DataTable dt = new DataTable();
            string AdvRequired = string.Empty;
            string AdminStatus = string.Empty;
            string UserCode = (_sessionService.Get<string>("userID")).ToString();
            string APPFlag = string.Empty;
            //GET REQUEST HEADER PART
            dt = oTourQueries.GetRequestHeaderPart(RequestID, UserCode);

            model.lblPageRequestID = RequestID;
            //FILL REQUEST DETAILS
            if (dt.Rows.Count > 0)
            {

                TempData["EmpDesig"] = dt.Rows[0]["ADDESIGNATIONID"].ToString();
                TempData["AppDate"] = dt.Rows[0]["APPLICATIONDATE"].ToString();
                model.lblEmpName = dt.Rows[0]["EMPNAME"].ToString();
                model.txtMobile = dt.Rows[0]["MOBILENO"].ToString();
                model.txtExtension = dt.Rows[0]["EXTNNO"].ToString();
                model.txtObjective = dt.Rows[0]["OBJOFJOURNEY"].ToString();
                AdvRequired = dt.Rows[0]["ISADVANCEREQ"].ToString();

                APPFlag = dt.Rows[0]["APPFLAG"].ToString();
                AdminStatus = dt.Rows[0]["ADMINSTATUS"].ToString();

                model.lblDesignation = dt.Rows[0]["DESIGNATION"].ToString();
                model.lblOperation = dt.Rows[0]["OPERATION"].ToString();
                model.lblDivision = dt.Rows[0]["DIVISION"].ToString();
                model.lblDepartment = dt.Rows[0]["DEPARTMENT"].ToString();
                model.lblSection = dt.Rows[0]["SECTION"].ToString();

                //SET APPROVAL TYPE
                model.hfAuthType = APPFlag;
            }
        }

        private void FillApprovalStatus_TAC(string RequestID, TourAuthChangeViewModel model)
        {
            DataTable dt = new DataTable();
            dt = oTourQueries.GetChangeAppAuth(RequestID);
            if (dt.Rows.Count > 0)
            {
                //IF SPECIAL APPROVAL NOT REQUIRED
                if (dt.Rows[0]["SPLAPPROVAL"].ToString().Trim() == "0")
                {
                    if (dt.Rows[0]["RECSTATUSDATA"].ToString().Trim() == "1" && dt.Rows[0]["APPAUTH"].ToString().Trim() == "1")
                        model.btnSave = false;
                }
                //IF SPECIAL APPROVAL REQUIRED
                else if (dt.Rows[0]["SPLAPPROVAL"].ToString().Trim() == "1")
                {
                    if (dt.Rows[0]["RECSTATUSDATA"].ToString().Trim() == "1" && dt.Rows[0]["APPAUTH"].ToString().Trim() == "1" && dt.Rows[0]["SpappstatusDATA"].ToString().Trim() == "1")
                        model.btnSave = false;
                }

                //IF RECOMMENDATION IS NOT APPLICABLE THEN HIDE RECOMMENDATION PART
                if (dt.Rows[0]["RECAUTH"].ToString().Trim() == "")
                {
                    model.tr_rec = false;
                }
                else
                {
                    //SHOW RECOMMENDATION ROW
                    model.tr_rec = true;
                    FillRecomAuth_TAC(dt.Rows[0]["ADEMPCODE"].ToString(), model);

                    model.lblRecAuth = dt.Rows[0]["RECAUTH"].ToString();
                    model.lblRecDate = dt.Rows[0]["RECAPPDATE"].ToString();
                    model.lblRecStatus = dt.Rows[0]["RECSTATUS"].ToString();

                    //SELECT APPROPRIATE AUTHORITY
                    model.cboRecAuthority = AppropiateAuthority_TAC(dt.Rows[0]["ADEMPCODE"].ToString());

                    if (dt.Rows[0]["RECSTATUSDATA"].ToString().Trim() == "0")
                    {
                        model.tr_RecApp = true;
                        //model.tr_rec.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#FFC9C9");
                    }
                    else
                    {
                        model.tr_RecApp = false;
                        //model.tr_rec.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#D2FCD7");
                    }
                }
                //IF APPROVAL NOT REQUIRED THEN 
                if (dt.Rows[0]["APPAUTH"].ToString().Trim() == "")
                {
                    model.tr_App = false;
                }
                else
                {
                    //SHOW APPROVAL ROW
                    model.tr_App = true;
                    model.lblAppAuth = dt.Rows[0]["APPAUTH"].ToString();
                    model.lblAppDate = dt.Rows[0]["APPDATE"].ToString();
                    model.lblAppStatus = dt.Rows[0]["APPSTATUS"].ToString();
                    //IF REQUEST RECOMMENDED 
                    if (dt.Rows[0]["RECSTATUSDATA"].ToString().Trim() == "1")
                    {
                        FillApprovalAuth_TAC(dt.Rows[0]["Recadempcode"].ToString(), model);
                        model.cboAppAuthority = AppropiateAuthority_TAC(dt.Rows[0]["Recadempcode"].ToString());
                    }
                    //REQUEST WITHOUT RECOMMENDATION
                    else
                    {
                        FillApprovalAuth_TAC(dt.Rows[0]["ADEMPCODE"].ToString(), model);
                        model.cboAppAuthority = AppropiateAuthority_TAC(dt.Rows[0]["ADEMPCODE"].ToString());

                    }
                    if (dt.Rows[0]["APPSTATUSDATA"].ToString().Trim() == "0")
                    {
                        model.tr_AppAuth = true;
                        // model.tr_App.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#FFC9C9");
                    }
                    else
                    {
                        model.tr_AppAuth = false;
                        //model.tr_App.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#D2FCD7");
                    }
                }
                //IF SPECIAL APPROVAL NOT REQUIRED THEN 
                if (dt.Rows[0]["SPAPPAUTH"].ToString().Trim() == "")
                {
                    model.tr_SpApp = false;
                }
                else
                {
                    //SHOW SPECIAL APPROVAL ROW
                    model.tr_SpApp = true;

                    model.lblSplAppAuth = dt.Rows[0]["SPAPPAUTH"].ToString();
                    model.lblSplAppDate = dt.Rows[0]["SPAPPDATE"].ToString();
                    model.lblSpAppStatus = dt.Rows[0]["SPAPPSTATUS"].ToString();

                    if (dt.Rows[0]["APPSTATUSDATA"].ToString().Trim() == "1")
                    {
                        FillSpApprovalAuth_TAC(dt.Rows[0]["Appadempcode"].ToString(), model);
                        model.cboSpAppAuthority = AppropiateAuthority_TAC(dt.Rows[0]["Appadempcode"].ToString());
                    }
                    if (dt.Rows[0]["SpappstatusDATA"].ToString().Trim() == "0")
                    {
                        model.tr_SpAppAuth = true;
                        //model.tr_SpApp.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#FFC9C9");
                        if (dt.Rows[0]["Spappadempcode"].ToString() != "")
                            model.cboSpAppAuthority = dt.Rows[0]["Spappadempcode"].ToString();
                    }
                    else
                    {
                        model.tr_SpAppAuth = false;
                        //model.tr_SpApp.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#D2FCD7");
                    }
                }
            }
        }

        private void FillRecomAuth_TAC(string strEcode, TourAuthChangeViewModel model)
        {
            DataTable dtAuthList = new DataTable();
            //GET USER APPROVAL AUTORITIES
            dtAuthList = oTourQueries.GetAppAuthorityList(strEcode, strEcode);
            model.cboRecAuthorityList = dtAuthList.AsSelectList("ADEMPCODE", "EMPNAME", true, "-- select --", "0");
        }

        private void FillApprovalAuth_TAC(string strEcode, TourAuthChangeViewModel model)
        {
            DataTable dtAuthList = new DataTable();
            //GET USER APPROVAL AUTORITIES
            dtAuthList = oTourQueries.GetAppAuthorityList(strEcode, strEcode);
            model.cboAppAuthorityList = dtAuthList.AsSelectList("ADEMPCODE", "EMPNAME", true, "-- select --", "0");
        }

        private void FillSpApprovalAuth_TAC(string strEcode, TourAuthChangeViewModel model)
        {
            DataTable dtAuthList = new DataTable();
            //GET USER APPROVAL AUTORITIES
            dtAuthList = oTourQueries.GetAppAuthorityList(strEcode, strEcode);
            model.cboSpAppAuthorityList = dtAuthList.AsSelectList("ADEMPCODE", "EMPNAME", true, "-- select --", "0");
        }

        public string AppropiateAuthority_TAC(string strEcode)
        {
            DataTable dts = new DataTable();
            string EmpPOS = string.Empty;
            string AppAuthCode = string.Empty;

            //SELECT APPROPRIATE AUTHORITY
            dts = oTourQueries.GetAppAuthorities(strEcode);
            if (dts.Rows.Count > 0)
            {
                EmpPOS = dts.Rows[0]["POS"].ToString();
                switch (EmpPOS)
                {
                    //IN CASE OF OTHER
                    case "OTH":
                        //FILL ALL SECTION HEAD OF USER DEPARTMENT
                        AppAuthCode = dts.Rows[0]["SECTIONHEADID"].ToString();
                        if (AppAuthCode == "")
                            AppAuthCode = dts.Rows[0]["DEPARTMENTHEADID"].ToString();
                        break;
                    //IN CASE OF SECTION MANAGER
                    case "SEC":
                        AppAuthCode = dts.Rows[0]["DEPARTMENTHEADID"].ToString();
                        break;
                    //IN CASE OF DEPARTMENT MANAGER
                    case "DPT":
                        AppAuthCode = dts.Rows[0]["DIVISIONHEADID"].ToString();
                        if (AppAuthCode == "")
                            AppAuthCode = dts.Rows[0]["VPHEADID"].ToString();
                        break;
                    //IN CASE OF DIVISIONAL MANAGER
                    case "DIV":
                        AppAuthCode = dts.Rows[0]["VPHEADID"].ToString();
                        break;
                }
            }
            return AppAuthCode;
        }

        /// <summary>
        /// SEND MAIL TO BOTH OLD AND NEW APPROVAL AUTHORITY
        /// </summary>
        /// <param name="strOldEcode"></param>
        /// <param name="strNewEcode"></param>
        /// <param name="strEcode"></param>
        /// <param name="tourID"></param>
        /// <param name="tourObjective"></param>
        /// <param name="tourSdate"></param>
        /// <param name="tourEdate"></param>
        /// <param name="strRem"></param>
        private void EmailStatus_TAC(string strOldEcode, string strNewEcode, string strEcode, string tourID,
            string tourObjective, string tourSdate, string tourEdate, string strRem)
        {
            commanEmail oEmail = new commanEmail();
            oEmail.MailFrom = "portal.admin@honda.hmsi.in";

            if (serverpath.isTestServer())
            {
                oEmail.MailTo = serverpath.getTestEMail();
                oEmail.MailCc = serverpath.getTestEMail();
            }
            else
            {
                oEmail.MailTo = AssociateEmail_TAC(strNewEcode);
                oEmail.MailCc = AssociateEmail_TAC(strOldEcode);
            }

            string strSubject = "Tour Request System - Authority Change";
            string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                     "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                     "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                     "</td></tr>" +
                                     "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                     "<b>Tour Request System - Authority Change</b></td></tr><tr height=150><td colspan=2>" +
                                     "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                     "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                     "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                     "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + AssociateName_TAC(strNewEcode) + " San,</b><br />" +
                                     "<br /><br />Tour Request (Request ID-" + tourID + ") of  " + AssociateName_TAC(strEcode) + " San has been forwarded to you due to the following reason:<br><br><b>Reason:</b> " + strRem +
                                     "<br /><br />The tour detail is given below:<br><br><b>Recommendation/Approval Authority:</b> " + AssociateName_TAC(strOldEcode) + "<br><b>Tour Objective:</b> " + tourObjective + "<br><b>Tour Start Date:</b> " + tourSdate + " <br> <b>Tour End Date:</b> " + tourEdate + "<br><br></td></tr><tr><td valign=top colspan=2>Kindly take appropriate action by logging in <a href='" + serverpath.getServerPath() + "'>E-Portal</a>.</td></tr></p>" +
                                     "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                     "<td colspan=2><b>Thank You <br /> <br />Best Regards</b><br /> Team - Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                     "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                     "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                     "</tr></table> ";


            oEmail.MailSubject = strSubject;
            oEmail.MailBody = strBody;
            try
            {
                bool status = oEmail.Send();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                return;
            }
            finally
            {
                //Response.Write("Your E-mail has been sent sucessfully");
            }
        }

        public string AssociateEmail_TAC(string strEcode)
        {
            string emailid = string.Empty;
            DataTable dtAe = new DataTable();
            dtAe = oTourQueries.GetAssoiateEmail(strEcode);
            if (dtAe.Rows.Count > 0)
            {
                emailid = dtAe.Rows[0]["emailid"].ToString();
            }
            return emailid;
        }

        public string AssociateName_TAC(string strEcode)
        {
            string associateName = string.Empty;
            DataTable dtAn = new DataTable();
            dtAn = oTourQueries.GetAssoiateEmail(strEcode);
            if (dtAn.Rows.Count > 0)
            {
                associateName = dtAn.Rows[0][1].ToString();
            }
            return associateName;
        }

        private void FillRequestDetails_TAC(string RequestID, TourAuthChangeViewModel model)
        {
            DataTable dtD = new DataTable();
            dtD = oTourQueries.GetReqeustBookingList(RequestID);
            model.gvList = dtList.TableToList<gvTacList>(dtD);
        }

        #endregion "Tour Auth Change ASPX  Start : Function End"
        #endregion "TourAuthChange.Aspx"
        /*-----------------------------------------------------------------------------*/


        #region "TourSchedule.Aspx"
        #region "Tour Schedule ASPX  Start : Page Load Start"
        [HttpGet]
        public IActionResult TourSchedule()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            DataTable dtAssociateList = oTourQueries.GetAssociateList(UserID.ToString());


            var model = new TourScheduleViewModel
            {
                optMonthList = new List<SelectListItem>
                {
                    new SelectListItem{Text = "Jan", Value="1"},
                    new SelectListItem{Text = "Feb", Value="2"},
                    new SelectListItem{Text = "Mar", Value="3"},
                    new SelectListItem{Text = "Apr", Value="4"},
                    new SelectListItem{Text = "May", Value="5"},
                    new SelectListItem{Text = "Jun", Value="6"},
                    new SelectListItem{Text = "Jul", Value="7"},
                    new SelectListItem{Text = "Aug", Value="8"},
                    new SelectListItem{Text = "Sep", Value="9"},
                    new SelectListItem{Text = "Oct", Value="10"},
                    new SelectListItem{Text = "Nov", Value="11"},
                    new SelectListItem{Text = "Dec", Value="12"}
                },
                optYearList = Enumerable.Range(currentYear - 1, 21)
                .Select(y => new SelectListItem { Text = y.ToString(), Value = y.ToString() })
                .ToList(),
                cboAssociateList = dtAssociateList.AsSelectList("ADEMPCODE", "EMPNAME", true, "-- select --", "0"),
                cboAssociate = UserID.ToString(),
                optMonth = currentMonth.ToString(),
                optYear = currentYear.ToString(),
            };
            return View(model);
        }
        #endregion "Tour Schedule ASPX  Start : Page Load End"

        #region "Tour Schedule ASPX  Start : Control Event Start"
        [HttpGet]
        public IActionResult SearchTourSchedulle(string cboAssociate, string optMonth, string optYear)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            DataTable dt = new DataTable();
            try
            {
                dt = oTourQueries.GetTourSchedule(cboAssociate, optMonth, optYear);
                var response = JsonConvert.SerializeObject(dt);
                return Json(new { result = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching SearchTourSchedulle details." });
            }
        }

        [HttpGet]
        public IActionResult CmbMonthlyReport_Click(string UserID, string MonthNum, string YearNum)
        {
            DataTable dt = new DataTable();
            try
            {
                //GET TOUR SCHEDULE 
                DataTable dtUtPoReq = oTourQueries.GetMonthlyTourReport(UserID, MonthNum, YearNum);

                int[] ColumnList = { 0, 1, 2, 3,
                         4, 5, 6, 7,
                         8, 9, 10, 11,
                         12, 13, 14, 15,
                         16, 17, 18, 19,
                         20, 21, 22, 23,
                         24, 25, 26, 27,
                         28, 29, 30, 31,
                         32, 33, 34, 35,
                         36, 37, 38, 39,
                         40, 41, 42, 43,
                         44, 45, 46, 47,
                         48, 49, 50, 51,
                         52, 53, 54, 55,
                         56, 57, 58, 59,
                         60, 61, 62, 63,
                         64, 65
                       };
                string[] HeaderList = { "Emp Code", "Emp Name", "Department", "Section",
                            "1st Day Loc", "1st Day Activity","2nd Day Loc", "2nd Day Activity",
                            "3rd Day Loc", "3rd Day Activity","4th Day Loc", "4th Day Activity",
                            "5th Day Loc", "5th Day Activity","6th Day Loc", "6th Day Activity",
                            "7th Day Loc", "7th Day Activity","8th Day Loc", "8th Day Activity",
                            "9th Day Loc", "9th Day Activity","10th Day Loc", "10th Day Activity",
                            "11th Day Loc", "11th Day Activity","12th Day Loc", "12th Day Activity",
                            "13th Day Loc", "13th Day Activity","14th Day Loc", "14th Day Activity",
                            "15th Day Loc", "15th Day Activity","16th Day Loc", "16th Day Activity",
                            "17th Day Loc", "17th Day Activity","18th Day Loc", "18th Day Activity",
                            "19th Day Loc", "19th Day Activity","20th Day Loc", "21th Day Activity",
                            "21st Day Loc", "21st Day Activity","22nd Day Loc", "22nd Day Activity",
                            "23rd Day Loc", "23rd Day Activity","24th Day Loc", "24th Day Activity",
                            "25th Day Loc", "25th Day Activity","26th Day Loc", "26th Day Activity",
                            "27th Day Loc", "27th Day Activity","28th Day Loc", "28th Day Activity",
                            "29th Day Loc", "29th Day Activity","30th Day Loc", "30th Day Activity",
                            "31st Day Loc", "31st Day Activity"
                          };

                int[] NewColumnList;
                string[] NewHeaderList;
                int num;
                if (MonthNum == "01" || MonthNum == "03" || MonthNum == "05" || MonthNum == "07" || MonthNum == "08" || MonthNum == "10" || MonthNum == "12")
                {
                    NewColumnList = new int[ColumnList.Length];
                    NewHeaderList = new string[HeaderList.Length];
                    for (num = 0; num < ColumnList.Length; num++)
                    {
                        NewColumnList[num] = ColumnList[num];
                        NewHeaderList[num] = HeaderList[num];
                    }
                }
                else if (MonthNum == "04" || MonthNum == "06" || MonthNum == "09" || MonthNum == "11")
                {
                    NewColumnList = new int[ColumnList.Length - 2];
                    NewHeaderList = new string[HeaderList.Length - 2];
                    for (num = 0; num < ColumnList.Length - 2; num++)
                    {
                        NewColumnList[num] = ColumnList[num];
                        NewHeaderList[num] = HeaderList[num];
                    }
                }
                else
                {
                    //LEAP YEAR
                    if (Convert.ToInt16(YearNum) % 4 == 0)
                    {
                        NewColumnList = new int[ColumnList.Length - 4];
                        NewHeaderList = new string[HeaderList.Length - 4];
                        for (num = 0; num < ColumnList.Length - 4; num++)
                        {
                            NewColumnList[num] = ColumnList[num];
                            NewHeaderList[num] = HeaderList[num];
                        }
                    }
                    else
                    {
                        NewColumnList = new int[ColumnList.Length - 6];
                        NewHeaderList = new string[HeaderList.Length - 6];
                        for (num = 0; num < ColumnList.Length - 6; num++)
                        {
                            NewColumnList[num] = ColumnList[num];
                            NewHeaderList[num] = HeaderList[num];
                        }
                    }
                }

                if (dtUtPoReq.Rows.Count > 0)
                {
                    //ExcelExport obj_Export = new ExcelExport();
                    //obj_Export.ExportDetails(dtUtPoReq, NewColumnList, NewHeaderList, ExcelExport.ExportFormat.Excel, "Tour_Schedule_" + optMonth.SelectedItem + "_" + optYear.SelectedItem + ".xls");
                }
                var response = JsonConvert.SerializeObject(dt);
                return Json(new { result = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching CmbMonthlyReport_Click details." });
            }
        }
        //[HttpPost]
        //public ActionResult CmdExport_Click(string cboAssociate, string optMonth, string optYear)
        //{
        //    DataTable dt = oTourQueries.GetTourSchedule(cboAssociate, optMonth, optYear); 
        //    short retVal = 0;
        //    if (dt.Rows.Count > 0)
        //    {                
        //        try
        //        {
        //            List<grdTourScheduleRequestList> __headerObj = dtList.TableToList<grdTourScheduleRequestList>(dt);
        //            string str = this.TourScheduleExcelHtml(__headerObj);
        //            TempData.Remove("TOURSCHEDULEEXCELFILE");
        //            TempData["TOURSCHEDULEEXCELFILE"] = str;
        //            retVal = 1;
        //        }
        //        catch (Exception ex)
        //        {
        //            retVal = (short)-1;
        //        }
        //    }
        //    return Json(retVal);
        //}




        #endregion "Tour Schedule ASPX  Start : Control Event End"

        #region "Tour Schedule ASPX  Start : Function Start"

        public string TourScheduleExcelHtml(List<grdTourScheduleRequestList> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Day</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Time</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Day Objective</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>From City</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>To City</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Staying City</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Travel Mode</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Ticket Class</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>FLT / Train No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Special Approval</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Special Approval Remarks</th>");
                stringBuilder.Append("</tr>");
                foreach (var AHVM in _headerList)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.TRAVELDATE + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.DAYNAME + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.TRAVELTIME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.DAYOBJECTIVE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.FRMCITY + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.TOCITY + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.STCITY + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.TRAVELMODE + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.TICKETCLASS + "</td>");
                    //stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.DATEADDED).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.FLIGHTTRAINNO + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.TICKETINGBY + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.TICKETINGBY + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }
        #endregion "Tour Schedule ASPX  Start : Function End"
        #endregion "TourSchedule.Aspx"
        /*-----------------------------------------------------------------------------*/


        #region "TourRequestDuplicateList.Aspx"
        #region "Tour Request Duplicate List ASPX  Start : Page Load Start"
        [HttpGet]
        public IActionResult TourRequestDuplicateList()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));

            if (string.IsNullOrEmpty(UserID.ToString()))
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new TourRequestDuplicateListViewModel
            {

            };

            return View(model);
        }
        #endregion "Tour Request Duplicate List ASPX : Page Load End" 
        #region "TourRequestDuplicateList ASPX : Control Events"
        [HttpGet]
        public IActionResult SearchTourRequestDuplicateList(string FromDate, string TillDate)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            DataTable dt = new DataTable();
            try
            {
                //GET TOUR LIST FOR SELECTED DEPARTMENT COORDINATOR
                dt = oTourQueries.GetDuplicatePrintList(UserID.ToString(), FromDate, TillDate);
                var response = JsonConvert.SerializeObject(dt);
                return Json(new { result = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching SearchTourRequestDuplicateList details." });
            }
        }
        #endregion "Tour Request Duplicate List ASPX : Control Events" 
        #endregion "TourRequestDuplicateList.Aspx"

        /*-----------------------------------------------------------------------------*/



        #region "Edit`TourRequest.Aspx"
        #region "Edit Tour Request ASPX  Start : Page Load Start"
        [HttpGet]
        public IActionResult EditTourRequest()
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            ViewBag.hdnUserId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            string RequestID = Request.Query["id"].ToString();
            string ModeID = string.Empty;

            if (string.IsNullOrEmpty(UserID.ToString()))
            {
                return RedirectToAction("Login", "Account");
            }

            int currentMonth = DateTime.Now.Month;
            int currentDay = DateTime.Now.Day;
            int currentYear = DateTime.Now.Year; // Years (current year -1 to current year + 19)


            List<SelectListItem> optDayList = Enumerable.Range(1, 31)
                .Select(d => new SelectListItem { Text = d.ToString(), Value = d.ToString() }).ToList();// Days (1 to 31)

            List<SelectListItem> optYearList = Enumerable.Range(currentYear - 1, 21)
                    .Select(y => new SelectListItem { Text = y.ToString(), Value = y.ToString() }).ToList();

            List<SelectListItem> optHourList = Enumerable.Range(0, 24)
                .Select(h => new SelectListItem { Text = h.ToString(), Value = h.ToString() }).ToList(); // Hours (0 to 23)

            List<SelectListItem> optMinuteList = Enumerable.Range(0, 60)
                .Select(m => new SelectListItem { Text = m.ToString(), Value = m.ToString() }).ToList();// Minutes (0 to 59)


            DataTable dtPT = new DataTable();
            dtPT = dtList.ArrayListToDataTable<TourAdvance>(oTourListpre);

            int requestIdInt = 0;
            bool hasRequestId = !string.IsNullOrWhiteSpace(RequestID) && int.TryParse(RequestID.Trim(), out requestIdInt);

            var cityData = hasRequestId
                ? oTourQueries.GetCityListOnTourRequestId(requestIdInt)
                : oTourQueries.GetCityList();



            ViewBag.previous = oTourListpre;

            ViewBag.MODE = "ADD";
            //STORE DAY WISE ACTIVITIES IN VIEW STATE AND SAVE AT ONE SUBMIT IN DATABASE
            ViewBag.TourList = oTourList;
            ViewBag.previous = oTourListpre;
            DataTable employeeDetails = oTourQueries.EmployeeDetail(UserID);

            var model = new EditTourRequestViewModel
            {
                optDayList = optDayList,
                optDayStartList = optDayList,
                optDayEndList = optDayList,

                optYearList = optYearList,
                optYearStartList = optYearList,
                optYearEndList = optYearList,

                optHourList = optHourList,
                optMinuteList = optMinuteList,

                optHourToList = optHourList,
                optMinuteToList = optMinuteList,

                optMonthList = new List<SelectListItem>
                {
                    new SelectListItem{Text = "Jan", Value="1"},
                    new SelectListItem{Text = "Feb", Value="2"},
                    new SelectListItem{Text = "Mar", Value="3"},
                    new SelectListItem{Text = "Apr", Value="4"},
                    new SelectListItem{Text = "May", Value="5"},
                    new SelectListItem{Text = "Jun", Value="6"},
                    new SelectListItem{Text = "Jul", Value="7"},
                    new SelectListItem{Text = "Aug", Value="8"},
                    new SelectListItem{Text = "Sep", Value="9"},
                    new SelectListItem{Text = "Oct", Value="10"},
                    new SelectListItem{Text = "Nov", Value="11"},
                    new SelectListItem{Text = "Dec", Value="12"}
                },

                optMonthStartList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Jan", Value="1"},
                new SelectListItem{Text = "Feb", Value="2"},
                new SelectListItem{Text = "Mar", Value="3"},
                new SelectListItem{Text = "Apr", Value="4"},
                new SelectListItem{Text = "May", Value="5"},
                new SelectListItem{Text = "Jun", Value="6"},
                new SelectListItem{Text = "Jul", Value="7"},
                new SelectListItem{Text = "Aug", Value="8"},
                new SelectListItem{Text = "Sep", Value="9"},
                new SelectListItem{Text = "Oct", Value="10"},
                new SelectListItem{Text = "Nov", Value="11"},
                new SelectListItem{Text = "Dec", Value="12"}
            },

                optMonthEndList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Jan", Value="1"},
                new SelectListItem{Text = "Feb", Value="2"},
                new SelectListItem{Text = "Mar", Value="3"},
                new SelectListItem{Text = "Apr", Value="4"},
                new SelectListItem{Text = "May", Value="5"},
                new SelectListItem{Text = "Jun", Value="6"},
                new SelectListItem{Text = "Jul", Value="7"},
                new SelectListItem{Text = "Aug", Value="8"},
                new SelectListItem{Text = "Sep", Value="9"},
                new SelectListItem{Text = "Oct", Value="10"},
                new SelectListItem{Text = "Nov", Value="11"},
                new SelectListItem{Text = "Dec", Value="12"}
            },

                optDay = currentDay.ToString(),
                optMonth = currentMonth.ToString(),
                optYear = currentYear.ToString(),

                ddlrefundbyList = new List<SelectListItem>
                {
                    new SelectListItem{Text = "-Select-" , Value = "0"},
                    new SelectListItem{Text = "Cheque", Value="1"},
                    new SelectListItem{Text = "Cash", Value="2"}
                },

                cmbTicketingByList = new List<SelectListItem>
                {
                    new SelectListItem{Text = "Admin", Value="1"},
                    new SelectListItem{Text = "Self", Value="0"}
                },

                cmbHotelReservList = new List<SelectListItem>
                {
                    new SelectListItem{Text = "Admin", Value="1"},
                    //new SelectListItem{Text = "Self", Value="0"}
                },

                ddlidtypeList = new List<SelectListItem>
                {
                    new SelectListItem{Text = "--Select--", Value="0"},
                    new SelectListItem{Text = "Driving License", Value="1"},
                    new SelectListItem{Text = "Passport", Value="2"},
                    new SelectListItem{Text = "Pan Card", Value="3"},
                    new SelectListItem{Text = "Voter I-Card", Value="4"},
                    new SelectListItem{Text = "Unique I- Card", Value="5"},
                },

                cmbPickDropList = new List<SelectListItem>
                {
                    new SelectListItem{Text = "--select--", Value=""},
                    new SelectListItem{Text = "Yes", Value="1"},
                    new SelectListItem{Text = "No", Value="0"}
                },


                /*Added by TTL on 16-Oct-2025 against SR110729 > CR7297 Start*/
                cmbFromList = cityData.AsSelectList_DS("SYCITYID", "DESCRIP", true, "-- select --", "0", true, "Other", ""),

                cmbToList = cityData.AsSelectList_DS("SYCITYID", "DESCRIP", true, "-- select --", "0", true, "Other", ""),
                cmbStayingList = cityData.AsSelectList_DS("SYCITYID", "DESCRIP", true, "-- select --", "0", true, "Other", ""),
                /*Added by TTL on 16-Oct-2025 against SR110729 > CR7297 End*/

                cmbModeList = oTourQueries.GetTravelModeList().AsSelectList_DS("ADTRAVELMODEID", "DESCRIP", true, "-- select --", "0"),
                cmbClassList = oTourQueries.GetTravelModeClass(ModeID).AsSelectList_DS("ADTRAVELMODECLASSID", "DESCRIP", true, "-- select --", "0"),

                lblBankAccount = employeeDetails.Rows[0]["ACCOUNTNO"].ToString(),
                txtMobile = employeeDetails.Rows[0]["TMOBILE"].ToString(),
                txtExtension = employeeDetails.Rows[0]["EXTENSIONNO"].ToString(),
            };

            //FILL TOUR HEADER AND DETAILS      
            FillRequestHeader_ETR(RequestID, ref model);
            FillRequestDetail_ETR(RequestID, ref model);
            FillPreTourDetail_ETR(RequestID, ref model);
            //FILL PREVIOUS ADVANCE DETAILS 
            FillAdvanceDetails_ETR(UserID.ToString(), ref model);


            return View(model);
        }

        #endregion "Edit Tour Request ASPX : Page Load End" 

        #region "EditTourRequest ASPX : Control Events Start"
        [HttpPost]
        public IActionResult UpdateTourDetail([FromBody] UpdateTourDetailRequest req)
        { 
            string EmpCode = (_sessionService.Get<string>("userID")).ToString();
            int resStatus = 0;
            try
            {
                if (!string.IsNullOrEmpty(EmpCode) && EmpCode != "0")
                {
                    var oTourListArray = new ArrayList(req.oTourList ?? new List<Tour>());
                    var oTourListPrvArray = new ArrayList(req.oTourListprv ?? new List<TourAdvance>());
                    var _deletedArray = new ArrayList(req._deleted ?? new List<Tour> ());

                    oTourQueries.UpdateTourDetail(req.RequestID, EmpCode, req.MobileNo, req.ExtNo, req.Email, req.Objective, req.AdvRemarks, req.AdvRequired, req.APlusNights, req.ANights, req.BNights, req.CNights, req.StayCharge, req.APlusDays, req.ADays, req.BDays, req.CDays, req.DailyAllow, req.MiscAllow, oTourListArray, req.MiscRemarks, req.RequiredAmount, oTourListPrvArray, req.Initiator_Status);
                    resStatus = 1;

                    if(req.Initiator_Status == "1")
                    {
                        //SEND MAIL TO APP AUTH IF REQUEST UPDATED AFTER RETURN BACK FROM APP AUTH
                        if (TempData["RETURNSTATUS"].ToString() == "3")
                        {
                            //GET APP AUTH POS
                            DataTable dt = new DataTable();
                            dt = oTourQueries.GetAppAuthorities(TempData["RECAUTH"].ToString());
                            string AppAuthEmailID = dt.Rows[0]["EMAILID"].ToString();
                            if (AppAuthEmailID != "")
                                SendMail_ETR(AppAuthEmailID, req.Objective, req.MobileNo, req.ExtNo, req.Email);
                        }
                        if (req._deleted.Count > 0)
                        {
                            int TxnNumber;
                            TxnNumber = oTourQueries.CheckRequestEmailsStatus(req.RequestID);
                            if (TxnNumber == 1)
                            {
                                SendmailTravelDesk_ETR(req.RequestID, req._deleted);
                                SendmailHotelDesk_ETR(req.RequestID, req._deleted);
                            }

                        }
                    }
                    var response = new
                    {
                        resStatus = resStatus
                    };

                    return Json(new { result = response });
                }

                // Default response when parameters null
                return Json(new { result = new { resStatus = 0 } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching UpdateTourDetail details." });
            }
        }

        #endregion "Edit Tour Request ASPX : Control Events End" 


        #region "EditTourRequest ASPX : Functions Start"
        private void FillRequestHeader_ETR(string RequestID, ref EditTourRequestViewModel model)
        {
            DataTable dt = new DataTable();
            string UserCode = (_sessionService.Get<string>("userID").ToString());
            string AdvRequired = string.Empty;
            string strstartdate = string.Empty;
            double MiscAmt;
            dt = oTourQueries.GetRequestHeaderPart(RequestID, UserCode);
            if (dt.Rows.Count > 0)
            {
                ViewBag.EmpDesig = dt.Rows[0]["ADDESIGNATIONID"].ToString();
                ViewBag.AppDate = dt.Rows[0]["APPLICATIONDATE"].ToString();

                model.txtMobile = dt.Rows[0]["MOBILENO"].ToString();
                model.txtExtension = dt.Rows[0]["EXTNNO"].ToString();
                model.txtEmail = dt.Rows[0]["ALTEMAILID"].ToString();
                model.txtObjective = dt.Rows[0]["OBJOFJOURNEY"].ToString();
                AdvRequired = dt.Rows[0]["ISADVANCEREQ"].ToString();
                MiscAmt = Convert.ToDouble(dt.Rows[0]["MISCELLANEOUSAMT"]);
                if (MiscAmt > 0)
                    model.txtMiscAmout = MiscAmt.ToString();
                model.txtMiscRemarks = dt.Rows[0]["MISCAMOUNTREMARKS"].ToString();
                model.txtRequiredAmount = string.Format("{0:F2}", Convert.ToDouble(dt.Rows[0]["TOTALAMOUNTREQUIRED"]));
                // PREVIOUS TOUR ADVANCE DETAILS
                model.txtAdvRemarks = dt.Rows[0]["PREVIOUSTOURADVREMARKS"].ToString();

                //STORE RECCOMENDATION DETAILS
                TempData["RECAUTH"] = dt.Rows[0]["RECADEMPCODE"].ToString();
                TempData["RETURNSTATUS"] = dt.Rows[0]["RETURNSTATUS"].ToString();
                TempData["RETURNREMAKRS"] = dt.Rows[0]["RETURNREMAKRS"].ToString();


                strstartdate = dt.Rows[0]["STARTDATE"].ToString();
                if (DateTime.Today > DateTime.Parse(strstartdate))
                {
                    model.chkAdvanceChecked = false;
                    model.cmbTicketingByEnabled = false;
                }

                if (AdvRequired == "1")
                {
                    model.chkAdvance = true;
                    model.pnlAdvance = true;
                }
                else
                {
                    model.chkAdvance = false;
                    model.pnlAdvance = false;
                }
                //Added by Aumento For SR73072 Start
                String Initiator_Status = dt.Rows[0]["Initiator_Status"].ToString();
                String App_Status = dt.Rows[0]["APPSTATUS"].ToString();

                if (Initiator_Status == "0")
                {
                    model.btnSaveAsDraft = true;
                }
                else
                {
                    model.btnSaveAsDraft = false;
                }

                String StartDate = dt.Rows[0]["STARTDATE"].ToString();
                String EndDate = dt.Rows[0]["ENDDATE"].ToString();

                if (StartDate.Length > 0)
                {
                    DateTime date = DateTime.Parse(StartDate);
                    int intMonthStart = date.Month;
                    int intDayStart = date.Day;
                    int intYearStart = date.Year;

                    model.optDayStart = intDayStart.ToString();
                    model.optMonthStart = intMonthStart.ToString();
                    model.optYearStart = intYearStart.ToString();
                }

                if (EndDate.Length > 0)
                {
                    DateTime date = DateTime.Parse(EndDate);
                    int intMonthEnd = date.Month;
                    int intDayEnd = date.Day;
                    int intYearEnd = date.Year;

                    model.optDayEnd = intDayEnd.ToString();
                    model.optMonthEnd = intMonthEnd.ToString();
                    model.optYearEnd = intYearEnd.ToString();
                }
                //Added by Aumento For SR73072 End
                model.tr_train = false;
            }
        }
        private void FillRequestDetail_ETR(string RequestID, ref EditTourRequestViewModel model)
        {

            oTourList = oTourQueries.GetRequestDetailPart(RequestID);
            ViewBag.TourList = oTourList;
            oTourListOrg.AddRange(oTourList);
            ViewBag.TourListOrg = oTourListOrg;

            DataTable dt = dtList.ArrayListToDataTable<Tour>(oTourList);
            model.gvList = dtList.TableToList<gvListETR>(dt);

            //CALCULATE STAY CHARGE AND DAILY ALLOWANCES
            CalculateDayNights_ETR(model);
        }
        private void FillPreTourDetail_ETR(string RequestID, ref EditTourRequestViewModel model)
        {

            ArrayList dt = oTourQueries.GetRequestpreDetail(RequestID);
            ViewBag.previous = dt;

            DataTable dtPT = new DataTable();
            dtPT = dtList.ArrayListToDataTable<TourAdvance>(dt);
            model.grdprvadv = dtList.TableToList<grdprvadv>(dtPT);
        }
        private void FillAdvanceDetails_ETR(string UserID, ref EditTourRequestViewModel model)
        {
            ArrayList oPrevTourList = new ArrayList();
            DataTable dt = oTourQueries.GetTouradvancedata(UserID);


            string Ecode = string.Empty;
            string Period = string.Empty;
            string Amount = string.Empty;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; dt.Rows.Count > i; i++)
                {
                    Ecode = dt.Rows[i]["ECODE"].ToString();
                    Period = dt.Rows[i]["TOURPERIOD"].ToString();
                    Amount = dt.Rows[i]["TOURAMOUNT"].ToString();
                    //IF CURRENT RECORD IS USER RECORD THEN
                    Tour oTour = new Tour();
                    oTour.TourPeriod = Period;
                    oTour.Amount = Amount;
                    if (Period.Length == 17)
                    {
                        if (oTour.Amount.IndexOf("-") == -1)
                        {
                            Int32 day = Convert.ToInt32(((DateTime.Parse(DateTime.Today.ToString())) - ((DateTime.ParseExact(Period.Substring(9, 8), "dd.MM.yy", null)).AddDays(7))).TotalDays);
                            if (day > 0)
                                oTour.Day = day.ToString();
                            else
                                oTour.Day = "";


                            otourperiod.Add(oTour);
                        }
                    }
                    oPrevTourList.Add(oTour);
                }
            }
            dt = dtList.ArrayListToDataTable<Tour>(oPrevTourList);
            model.gvAdvance = dtList.TableToList<gvAdvance>(dt);

            ViewBag.TourPeriod = otourperiod;
            TourPerioddropdown_ETR(otourperiod, model); 

            
        }
        private void TourPerioddropdown_ETR(ArrayList otourperiod, EditTourRequestViewModel model)
        {
            var tourPeriodList = otourperiod.Cast<object>()
                                   .Select(o => new SelectListItem
                                   {
                                       Text = o.GetType().GetProperty("TourPeriod")?.GetValue(o)?.ToString() ?? "",
                                       Value = o.GetType().GetProperty("Amount")?.GetValue(o)?.ToString() ?? ""
                                   })
                                   .Prepend(new SelectListItem { Text = "-Select Period-", Value = "0" })
                                   .ToList();
            model.ddltourperiodList = tourPeriodList;
        }
        private void CalculateDayNights_ETR(EditTourRequestViewModel model)
        {
            string CityCode;
            string StayingCity = string.Empty;
            int rownum;
            int days;
            int nights;
            int chargeAP = 0;
            int chargeA = 0;
            int chargeB = 0;
            int chargeC = 0;
            int TotalNights;

            int allowAP = 0;
            int allowA = 0;
            int allowB = 0;
            int allowC = 0;
            int TotalDays;
            int EmpDesignationID;

            double NightCharge = 0;
            double TotalNightCharge = 0;
            double DailyAllowance = 0;
            double TotalDailyAllowance = 0;

            //CHECK STAY CHARGE/DAILY ALLOWANCE WHEN TOUR ADVANCE IS SELECTED
            if (model.chkAdvance == true && model.gvList.Any())
            {
                //SHOW ADVANCE PANEL
                model.pnlAdvance = true;
                string TravelDate = string.Empty;
                string CityCategory = string.Empty;
                int CityCategoryCode = 0;
                Boolean isDayCalculated = false;
                Boolean isNightCalculated = false;
                Tour oTour = new Tour();
                oTourList = (ArrayList)ViewBag.TourList;
                List<string> ProcessDates = new List<string>();
                List<string> ProcessNights = new List<string>();

                for (rownum = 0; rownum < oTourList.Count; rownum++)
                {
                    oTour = (Tour)oTourList[rownum];
                    days = 1;
                    nights = 1;
                    //GET TRAVEL DATE
                    TravelDate = oTour.TourFromDate;
                    //CHECK DATE ALREADY PROCESSED OR NOT
                    isDayCalculated = DailyAllowanceCaluculation(ProcessDates, TravelDate);
                    if (isDayCalculated == true)
                    {
                        days = 0;
                    }
                    else
                    {
                        ProcessDates.Add(TravelDate);
                    }

                    //GET STAYING CITY CODE
                    CityCode = oTour.StayingLocCode;
                    StayingCity = oTour.StayingLoc.Trim();

                    //CHECK NIGHT ALREADY PROCESSED OR NOT
                    isNightCalculated = StayChargeCaluculation(ProcessNights, TravelDate, StayingCity);
                    if (isNightCalculated == true)
                    {
                        nights = 0;
                    }
                    else
                    {
                        //CHECK IF STAYING CITY EXIST
                        if (StayingCity != "")
                            ProcessNights.Add(TravelDate);
                    }
                    //IF NO STAYING CITY IS THERE
                    if (StayingCity == "")
                    {
                        nights = 0;
                        //DAILY ALLOWANCES CALCULATED ON TO CITY CODE
                        CityCode = oTour.ToLocCode;
                    }
                    //IF CITYCODE IS 0 IE OTHER THEN SET IT TO BLANK BECAUSE IS DOESN'T STORE IN DATABASE
                    if (CityCode == "0")
                        CityCode = "";

                    //GET CATEGORY DESCRIPTION OF CITY (RETURN C IN CASE OF OTHER CITY(CITYCODE=0)
                    DataRow[] _datarow;
                    _datarow = oTourQueries.GetCityCategory(CityCode);
                    if (_datarow.Length > 0)
                    {
                        CityCategory = _datarow[0]["DESCRIP"].ToString();
                        CityCategoryCode = System.Convert.ToInt16(_datarow[0]["SYCITYCATEGORYID"]);
                    }

                    EmpDesignationID = System.Convert.ToInt16(ViewBag.EmpDesig);
                    // Added By Kishan Dodiya
                    string UserType = Convert.ToInt64(ViewBag.ADEMPCODE) > 70000000 ? "EXPAT" : "LOCAL";
                    //CALCULATE NIGHT CHARGE AND DAILY ALLOWANCE CITY WISE
                    _datarow = oTourQueries.GetEmployeeAllowanceDetail(CityCategoryCode, EmpDesignationID, ViewBag.AppDate.ToString(), UserType);
                    // End Added
                    if (_datarow.Length > 0)
                    {
                        NightCharge = System.Convert.ToDouble(_datarow[0]["LODGINGAMT"]);
                        DailyAllowance = System.Convert.ToDouble(_datarow[0]["DAILYALLOWANCEAMT"]);
                    }
                    TotalDailyAllowance += (DailyAllowance * days);
                    TotalNightCharge += (NightCharge * nights);

                    switch (CityCategory)
                    {
                        case "A+":
                            chargeAP += nights;
                            allowAP += days;
                            break;
                        case "A":
                            chargeA += nights;
                            allowA += days;
                            break;
                        case "B":
                            chargeB += nights;
                            allowB += days;
                            break;
                        case "C":
                            chargeC += nights;
                            allowC += days;
                            break;
                    }
                }
                TotalNights = chargeAP + chargeA + chargeB + chargeC;
                TotalDays = allowAP + allowA + allowB + allowC;

                model.lblStayChargeAPLUS = chargeAP.ToString();
                model.lblAllownceAPLUS = allowAP.ToString();
                model.lblStayChargeA = chargeA.ToString();
                model.lblAllownceA = allowA.ToString();
                model.lblStayChargeB = chargeB.ToString();
                model.lblAllownceB = allowB.ToString();
                model.lblStayChargeC = chargeC.ToString();
                model.lblAllownceC = allowC.ToString();

                model.lblTotalNights = TotalNights.ToString();
                model.lblTotalDays = TotalDays.ToString();

                //CALCULATE TOTAL ALLOWANCES AND CHARGE
                model.lblTotalAllowances = string.Format("{0:F2}", TotalDailyAllowance);
                model.lblTotalCharges = string.Format("{0:F2}", TotalNightCharge);
            }
            else
            {
                //HIDE ADVANCE PANEL
                model.pnlAdvance = false;
            }
            double MiscAmount = 0;
            MiscAmount = System.Convert.ToDouble(model.lblMiscAmount);
            model.lblMiscAmount = string.Format("{0:F2}", MiscAmount);

            model.lblTotalAmount = string.Format("{0:F2}", (TotalDailyAllowance + TotalNightCharge + MiscAmount));
        }
        protected void SendmailTravelDesk_ETR(string RequestID, List<Tour> _deleted)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();

            string strSubject = string.Empty;
            string strBody = string.Empty;
            string strTravelDeskEmail = string.Empty;
            string strcc = string.Empty;
            commanEmail sendMail = new commanEmail();
            DataTable dt = new DataTable();
            strTravelDeskEmail = objCommon.GetParameterValue("TRAVEL_DESK_MAIL");

            if (strTravelDeskEmail != "")
            {
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    if (strTravelDeskEmail != "")
                    {
                        sendMail.MailTo = strTravelDeskEmail;

                    }
                }

                strSubject = "Travel Desk:- Tour Cancellation Request from - " + UserName + "[ " + UserID.ToString() + " ]";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                             "</td></tr>" +
                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                             "</td></tr><tr height=150><td colspan=2>" +
                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                              "<tr><td valign=top width='20%'>Dear San,</td><td valign=top >" + " " + "</td></tr>" +
                             "<tr><td valign=top colspan=2> Following Days Of Travel has been cancelled By User. " + "</td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td valign=top >Employee Name:</td><td valign=top >" + UserName + " [" + UserID.ToString() + "]</td></tr>" +
                                "<tr><td valign=top > <b> Tour Dates:-</b></td><td valign=top > </td></tr>";
                int i = 1;
                foreach (var item in _deleted)
                {

                    strBody += "<tr><td colspan=2 style='padding-left:46px;'>&nbsp;&nbsp;" + i + ".&nbsp;&nbsp;" + item.TourFromDate + "</td></tr>";
                    i = i + 1;
                }


                strBody += "<tr><td colspan=2>&nbsp;</td></tr>" +
                       "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                       "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                       "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                       "</tr></table> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                try
                {
                    bool status1 = sendMail.Send();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception Occurred: {ex}");
                    _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);

                }
            }
        }
        protected void SendmailHotelDesk_ETR(string RequestID, List<Tour> _deleted)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();

            string strSubject = string.Empty;
            string strBody = string.Empty;
            string strHotelDeskEmail = string.Empty;
            string strcc = string.Empty;
            commanEmail sendMail = new commanEmail();
            DataTable dt = new DataTable();
            strHotelDeskEmail = objCommon.GetParameterValue("HOTEL_DESK_MAIL");

            if (strHotelDeskEmail != "")
            {
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    if (strHotelDeskEmail != "")
                    {
                        sendMail.MailTo = strHotelDeskEmail;
                    }
                }

                strSubject = "Hotel Desk:- Tour Cancellation Request from - " + UserName + "[ " + UserID.ToString() + " ]";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                             "</td></tr>" +
                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                             "</td></tr><tr height=150><td colspan=2>" +
                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                              "<tr><td valign=top width='20%'>Dear San,</td><td valign=top >" + " " + "</td></tr>" +
                             "<tr><td valign=top colspan=2>  Following Days Of Hotel has been cancelled By User.  " + "</td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td valign=top >Employee Name:</td><td valign=top >" + UserName + " [" + UserID.ToString() + "]</td></tr>" +
                               "<tr><td valign=top ><b>  Tour Dates:-</b></td><td valign=top > </td></tr>";
                int i = 1;
                foreach (var item in _deleted)
                {

                    strBody += "<tr><td colspan=2 style='padding-left:46px;'>&nbsp;&nbsp;" + i + ".&nbsp;&nbsp;" + item.TourFromDate + "</td></tr>";
                    i = i + 1;
                }

                strBody += "<tr><td colspan=2>&nbsp;</td></tr>" +
                       "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                       "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                       "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                       "</tr></table> ";


                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                try
                {
                    bool status1 = sendMail.Send();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception Occurred: {ex}");
                    _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                }
            }
        }
        private void SendMail_ETR(string strApprovalAuthEmail, string Objective, string MobileNo, string ExtNo, string Email)
        {
            UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));
            UserName = (_sessionService.Get<string>("userName")).ToString();

            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";

            if (serverpath.isTestServer())
                sendMail.MailTo = serverpath.getTestEMail();
            else
                sendMail.MailTo = strApprovalAuthEmail;

            string strSubject = "Updated Tour Request from - " + UserName + ", Employee Code - " + UserID.ToString();
            string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                             "</td></tr>" +
                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                             "</td></tr><tr height=150><td colspan=2>" +
                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td valign=top colspan=2><b>Updated Tour Request from " + UserName + " - Emp Code (" + UserID.ToString() + ")</b></td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td valign=top width='20%'>Employee Code:</td><td valign=top >" + UserID.ToString() + "</td></tr>" +
                             "<tr><td valign=top >Employee Name:</td><td valign=top >" + UserName + "</td></tr>" +
                             "<tr><td valign=top >Tour Objective:</td><td valign=top >" + Objective + "</td></tr>" +
                             "<tr><td valign=top >Mobile No.:</td><td valign=top >" + MobileNo + "</td></tr>" +
                             "<tr><td valign=top >Ext.:</td><td valign=top >" + ExtNo + "</td></tr>" +
                             "<tr><td valign=top >Email:</td><td valign=top >" + Email + "</td></tr>" +
                             "<tr><td valign=top >Return Back Remakrs:</td><td valign=top >" + TempData["RETURNREMAKRS"].ToString() + " </td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                             "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                             "</tr></table> ";


            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
            }
            finally
            {
                //Response.Write("Your E-mail has been sent sucessfully");
            }
        }
        #endregion "Edit Tour Request ASPX : Functions End" 
        #endregion "EditTourRequest.Aspx"

        /*-----------------------------------------------------------------------------*/
    }
}
