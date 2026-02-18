using ePortal.DomainClasses;
using ePortal.ViewModels;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.WebUI.Filters;
using ePortal.Shared;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class LocalConveyanceController : Controller
    {
        //private System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        private string sJSON = String.Empty;
        LibResult res = new LibResult();

        private readonly ILocalConveyanceServices _Ilcs;
        private readonly ISessionService _sessionService;
        public LocalConveyanceController(ILocalConveyanceServices Ilcs, ISessionService sessionService)
        {

            _Ilcs = Ilcs;
            _sessionService = sessionService;
        }

        #region local Coveyance Master

        #region  LC_ZTABLE MASTER



        public ActionResult LC_ZTABLEList()
        {
            return View();
        }

        public ActionResult LC_ZTABLECreate()
        {
            // ViewBag.STATE = _Ilcs.GetSatelist(); // Added by Aumento as on 01082024
            ViewBag.SITE = _Ilcs.GetSysiteList(); // Added by Aumento as on 01082024
            return View();
        }

        //[HttpGet]
        //public JsonResult GetLC_ZTABLEDataList()
        //{
        //    List<LC_ZTABLE> ilist = new List<LC_ZTABLE>();
        //    ilist = _Ilcs.GetLC_ZTABLEList();
        //    sJSON = JsonConvert.SerializeObject(ilist);
        //    return Json(sJSON);
        //}


        [HttpGet]
        public IActionResult GetLC_ZTABLEDataList()
        {
            var ilist = _Ilcs.GetLC_ZTABLEList();
            return Json(ilist);
        }


        [HttpPost]
        public JsonResult LC_ZTABLECreate(decimal FourWheelerRate, decimal TwoWheelerRate, DateTime FromDate, DateTime ToDate, long SiteID) //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.InsertDataToLC_ZTABLE(FourWheelerRate, TwoWheelerRate, FromDate, ToDate, LoginCode, SiteID);
            sJSON = JsonConvert.SerializeObject(res);
            return Json(sJSON);
            
        }

        [HttpPost]
        public JsonResult LC_ZTABLEEdit(int SrNo, decimal FourWheelerRate, decimal TwoWheelerRate, DateTime FromDate, DateTime ToDate, long SiteID) //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.EditDataForLC_ZTABLE(SrNo, FourWheelerRate, TwoWheelerRate, FromDate, ToDate, LoginCode, SiteID);
            sJSON = JsonConvert.SerializeObject(res);
            return Json(sJSON);


        }

        [HttpPost]
        public JsonResult LC_ZTABLEDelete(int id)
        {           
            // bool rest = _Ilcs.GetDeleteDataforLC_ZTABLE(id);  // SR107643 :: LC ENHANCEMENT ::  AUMENTO
            int loginCode = int.Parse(_sessionService.Get<string>("userID")); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
            bool rest = _Ilcs.GetDeleteDataforLC_ZTABLE(id, loginCode); // {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
            return Json(rest);
        }

        public IActionResult LC_ZTABLEEdit(int id)
        {


            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return BadRequest();
            }


            LC_ZTABLE LC_ZTABLE = _Ilcs.GetEditDataforLC_ZTABLE(id);
            if (LC_ZTABLE == null)
            {
                return NotFound();
            }
            //ViewBag.STATE = _Ilcs.GetSatelist(); // Added by Aumento as on 01082024
            ViewBag.SITE = _Ilcs.GetSysiteList(); // Added by Aumento as on 01082024
            return View(LC_ZTABLE);
        }


        public JsonResult GetSatelist()
        {
            res.resultObject = _Ilcs.GetSatelist();
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }




        #endregion

        #region LC_EXCEPTION_C_MASTER
        public ActionResult LC_EXCEPTIONCTableList()
        {
            return View();
        }

        public ActionResult LC_EXCEPTIONCTableCreate()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetLC_EXCEPTIONCTableDataList()
        {
            List<LC_EXCEPTION_C_TABLE> ilist = new List<LC_EXCEPTION_C_TABLE>();
            ilist = _Ilcs.GetLC_EXCEPTIONList();
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        [HttpPost]
        public JsonResult LC_EXCEPTIONCTableCreate(int EmpCode, decimal FuelReimbursementPerLtr, DateTime FromDate, DateTime ToDate, string Remarks)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.InsertDataIntoLC_EXCEPTION(EmpCode, FuelReimbursementPerLtr, FromDate, ToDate, LoginCode, Remarks);
            sJSON = JsonConvert.SerializeObject(res);
            return Json(sJSON);

        }

        [HttpPost]
        public JsonResult LC_EXCEPTIONCTableEdit(int SrNo, int EmpCode, decimal FuelReimbursementPerLtr, DateTime FromDate, DateTime ToDate, string Remarks)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.EditDataForLC_EXCEPTION(SrNo, EmpCode, FuelReimbursementPerLtr, FromDate, ToDate, LoginCode, Remarks);
            sJSON = JsonConvert.SerializeObject(res);
            return Json(sJSON);


        }

        [HttpPost]
        public JsonResult LC_EXCEPTIONCTableDelete(int id)
        {           
            //bool rest = _Ilcs.GetDeleteDataForLC_EXCEPTION(id); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
            int LoginCode = int.Parse(_sessionService.Get<string>("userID")); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
            bool rest = _Ilcs.GetDeleteDataForLC_EXCEPTION(id, LoginCode); // {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
            return Json(rest);
        }

        public IActionResult LC_EXCEPTIONCTableEdit(int id)
        {


            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return BadRequest();
            }


            LC_EXCEPTION_C_TABLE LC_EXCEPTION_C_TABLE = _Ilcs.GetEditDataForLC_EXCEPTION(id);
            if (LC_EXCEPTION_C_TABLE == null)
            {
                //return HttpNotFound();
                return NotFound();
            }
            return View(LC_EXCEPTION_C_TABLE);
        }

        public ActionResult AutocompleteSuggestions(string term)
        {

            var suggestions = _Ilcs.GETEmployee(term);
            return Json(suggestions);
        }
        #endregion

        #region LC_Reimbursement b table Master 

        public ActionResult LC_Reimbursement_b_TableList()
        {
            return View();
        }

        public ActionResult LC_Reimbursement_b_TableCreate()
        {
            return View();
        }


        public JsonResult GetLC_Reimbursement_b_TableDataList()
        {
            List<LC_REIMBURSEMENT_B_TABLE> ilist = new List<LC_REIMBURSEMENT_B_TABLE>();
            ilist = _Ilcs.GetLC_ReimbursementMasterList();
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        [HttpPost]
        public JsonResult LC_Reimbursement_b_TableCreate(int Designation, string VehicleEntitlement, decimal Fule, int Refresh, int Meal, DateTime FromDate, DateTime ToDate)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.InsertDataIntoLC_ReimbursementMaster(Designation, VehicleEntitlement, Fule, Refresh, Meal, FromDate, ToDate, LoginCode);
            sJSON = JsonConvert.SerializeObject(res);
            return Json(sJSON);

        }

        [HttpPost]
        public JsonResult LC_Reimbursement_b_TableEdit(int SrNo, int Designation, string VehicleEntitlement, decimal Fule, int Refresh, int Meal, DateTime FromDate, DateTime ToDate)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.EditDataForLC_ReimbursementMaster(SrNo, Designation, VehicleEntitlement, Fule, Refresh, Meal, FromDate, ToDate, LoginCode);
            sJSON = JsonConvert.SerializeObject(res);
            return Json(sJSON);


        }

        [HttpPost]
        public JsonResult LC_Reimbursement_b_TableDelete(int id)
        {           
            //bool rest = _Ilcs.GetDeleteDataForLC_ReimbursementMaster(id); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
            int LoginCode = int.Parse(_sessionService.Get<string>("userID")); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
            bool rest = _Ilcs.GetDeleteDataForLC_ReimbursementMaster(id, LoginCode); // {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
            return Json(rest);
        }

        public IActionResult LC_Reimbursement_b_TableEdit(int id)
        {

            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return BadRequest();
            }

            LC_REIMBURSEMENT_B_TABLE LC_REIMBURSEMENT_B_TABLE = _Ilcs.GetEditDataForLC_ReimbursementMaster(id);
            if (LC_REIMBURSEMENT_B_TABLE == null)
            {
                //return HttpNotFound();
                return NotFound();
            }
            return View(LC_REIMBURSEMENT_B_TABLE);
        }

        public JsonResult GetDesignationlist()
        {
            res.resultObject = _Ilcs.GetDesignationlist();
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }

        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        #region LC_OH_DirectoreFlow_Master
        [HttpGet]
        public IActionResult LC_OH_DirApprovalFlow_MasterList()
        {
            List<LC_OH_DIRAPPROVALFLOW_MASTERViewModel> ilist = new List<LC_OH_DIRAPPROVALFLOW_MASTERViewModel>();
            ilist = _Ilcs.GetLC_OH_DirApprovalFlowList();
            return View(ilist);
        }
        [HttpPost]
        public JsonResult LC_OH_DirApprovalFlow_MasterEdit(int SrNo, string RequestorDesg, string ApprovalDesg) //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.EditDataforLC_OH_DIRAPPROVALFLOW_MASTER(SrNo, RequestorDesg, ApprovalDesg, LoginCode);
            //sJSON = oSerializer.Serialize(res);
            return Json(res);
        }

        public ActionResult LC_OH_DirApprovalFlow_MasterEdit(int id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return BadRequest();
            }

            LC_OH_DIRAPPROVALFLOW_MASTER LC_OH_DIRAPPROVALFLOW_MASTERViewModel = _Ilcs.GetEditDataforLC_OH_DIRAPPROVALFLOW_MASTERViewModel(id);
            if (LC_OH_DIRAPPROVALFLOW_MASTERViewModel == null)
            {
                //return HttpNotFound();
                return NotFound();
            }
            ViewBag.REQUESTOR_DESG = _Ilcs.GetRequestorDesg();
            ViewBag.APPROVAL_DESG = _Ilcs.GetApprovalDesg();

            return View(LC_OH_DIRAPPROVALFLOW_MASTERViewModel);
        }
        #endregion

        #region Master Log View
        public ActionResult ViewLog(int SrNO, string TableName)
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                var Data = _Ilcs.GetLC_Master_Log_Data(SrNO, TableName).Result;
                ViewBag.DATA = Data;
                return View("ViewLog", Data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        #endregion

        #region  Local Conveyance Request 
        public ActionResult LCRequestForm()
        {

            Tuple<string, string> dateRange = _Ilcs.GetDateRange();

            ViewBag.StartDate = dateRange.Item1;
            ViewBag.EndDate = dateRange.Item2;
            ViewBag.MonthCount = _Ilcs.GetMonthRangeCount();   // New Function Added by Aumento as on 21-06-2024
            ViewBag.PlantList = _Ilcs.GetPaymentPlants();

            return View();
        }

        //public async Task<JsonResult> GetODDATA(string FromDate, int Status)
        //{
        //    // Changes in this Function by Aumento as on 21-06-2024

        //    List<LC_DETAIL_TEMP> ListData = await _Ilcs.GetODDATA(_sessionService.Get<string>("userID"), FromDate);  // Changes in this Function by Aumento as on 21-06-2024
        //    List<LC_DETAIL_TEMP> LCiList = new List<LC_DETAIL_TEMP>();

        //    if (Status == 0)
        //    {
        //        foreach (var row in ListData)
        //        {
        //            LC_DETAIL_TEMP LCitem = new LC_DETAIL_TEMP();
        //            LCitem = row;
        //            LC_DETAIL_TEMP existObj = _Ilcs.GetODDATAFromLC_DETAIL(LCitem.EMPCODE, LCitem.DATEOFEXPENDITURE, LCitem.ASOFFODID);

        //            if (existObj != null)
        //            {
        //                existObj.STATUS = _Ilcs.getStatus(existObj);
        //                if (existObj.ISSUBMITTED == 0 || existObj.ISSUBMITTED == 3)
        //                {
        //                    LCiList.Add(existObj);
        //                }
        //                continue;
        //            }
        //            else
        //            {
        //                LCiList.Add(LCitem);
        //            }
        //        }

        //        // Changes in this Function by Aumento as on 21-06-2024
        //    }
        //    else
        //    {
        //        int loginCode = int.Parse(_sessionService.Get<string>("userID"));

        //        LCiList = _Ilcs.GetODListDATAFromLC_DETAIL(FromDate, loginCode);  // Remove Todate perameter by Aumento as on 21-06-2024
        //    }


        //    LibResult Empdata = _Ilcs.GetEmployeeDATA(int.Parse(_sessionService.Get<string>("userID")));

        //    var combinedData = new
        //    {
        //        ODData = LCiList,
        //        EmployeeData = Empdata.resultObject
        //    };

        //    JsonSerializerSettings settings = new JsonSerializerSettings
        //    {
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //    };

        //    sJSON = JsonConvert.SerializeObject(combinedData, settings);
        //    return Json(sJSON);
        //}


        public async Task<JsonResult> GetODDATA(string FromDate, int Status)
        {
            // Changes in this Function by Aumento as on 21-06-2024

            //List<LC_DETAIL_TEMP> ListData = _Ilcs.GetODDATA(Session["UserId"].ToString(), FromDate); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
            List<LC_DETAIL_TEMP> LCiList = new List<LC_DETAIL_TEMP>();
            int loginCode = int.Parse(_sessionService.Get<string>("userID")); // SR107643 :: LC ENHANCEMENT ::  AUMENTO
            if (Status == 0)
            {
                // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                List<LC_DETAIL_TEMP> ListData = new List<LC_DETAIL_TEMP>();
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

                ListData = await _Ilcs.GetODDATA(_sessionService.Get<string>("userID").ToString(), FromDate);

                List<LC_DETAIL_TEMP> ExistOD = _Ilcs.GetODListDATAFromLC_DETAIL(FromDate, loginCode, Status);

                var datesToKeep = ListData.Select(x => x.DATEOFEXPENDITURE).ToList();

                if (ExistOD != null && datesToKeep != null)
                {
                    ExistOD.RemoveAll(e => datesToKeep.Contains(e.DATEOFEXPENDITURE));
                    ListData = ListData.Concat(ExistOD).ToList();
                }
                // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
                foreach (var row in ListData)
                {
                    LC_DETAIL_TEMP LCitem = new LC_DETAIL_TEMP();
                    LCitem = row;
                    LC_DETAIL_TEMP existObj = _Ilcs.GetODDATAFromLC_DETAIL(LCitem.EMPCODE, LCitem.DATEOFEXPENDITURE, LCitem.ASOFFODID);

                    if (existObj != null)
                    {
                        existObj.STATUS = _Ilcs.getStatus(existObj);
                        if (existObj.ISSUBMITTED == 0 || existObj.ISSUBMITTED == 3)
                        {
                            LCiList.Add(existObj);
                        }
                        continue;
                    }
                    else
                    {
                        LCiList.Add(LCitem);
                    }
                }

                // Changes in this Function by Aumento as on 21-06-2024
            }
            else
            {
                //  int loginCode = int.Parse(Session["UserId"].ToString()); // SR107643 :: LC ENHANCEMENT ::  AUMENTO

                LCiList = _Ilcs.GetODListDATAFromLC_DETAIL(FromDate, loginCode, Status);  // Remove Todate perameter by Aumento as on 21-06-2024 // {Status added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
            }


            LibResult Empdata = _Ilcs.GetEmployeeDATA(int.Parse(_sessionService.Get<string>("userID")));

            var combinedData = new
            {
                ODData = LCiList,
                EmployeeData = Empdata.resultObject
            };

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(combinedData, settings);
            return Json(sJSON);
        }

        public ActionResult UpdateLCRequest()
        {

            // Access query parameters using Request.Query
            if (!int.TryParse(Request.Query["id"], out int id) ||
                !int.TryParse(Request.Query["StatusId"], out int statusId))
            {
                return BadRequest("Missing or invalid query parameters.");
            }


            //ViewBag.id = Convert.ToInt32(Request.Query["id"].ToString());
            //ViewBag.StatusId = Convert.ToInt32(Request.Query["StatusId"].ToString());

            ViewBag.id = id;
            ViewBag.StatusId = statusId;


            Tuple<TimeSpan, TimeSpan, TimeSpan, TimeSpan> LDTime = _Ilcs.GetLunchDinnerTime();

            ViewBag.LunchStartTime = LDTime.Item1;
            ViewBag.LunchEndTime = LDTime.Item2;
            ViewBag.DinnerStartTime = LDTime.Item3;
            ViewBag.DinnerEndTime = LDTime.Item4;


            ViewBag.City = _Ilcs.CityList();


            ViewBag.Status = "1";
            ViewBag.hasError = res.hasError;
            ViewBag.errorMessage = "";

            return View();
        }

        [HttpPost]
        public JsonResult UpdateLCRequestSave(string ModelData)
        {

            try
            {
                LocalConveyanceRequestViewModel model = JsonConvert.DeserializeObject<LocalConveyanceRequestViewModel>(ModelData);
                var loginEmp = int.Parse(_sessionService.Get<string>("userID"));

                var _DATEOFEXPENDITURE = DateTime.ParseExact(model.DATEOFEXPENDITURE, "dd-MMM-yyyy", null);

                LC_DETAIL_TEMP Data = new LC_DETAIL_TEMP
                {
                    SRNO = model.SRNO,
                    ASOFFODID = !string.IsNullOrEmpty(model.ASOFFODID) ? long.Parse(model.ASOFFODID) : (long?)null,
                    EMPCODE = !string.IsNullOrEmpty(model.EMPCODE) ? long.Parse(model.EMPCODE) : (long?)null,
                    EMPNAME = model.EMPNAME,
                    DATEOFEXPENDITURE = _DATEOFEXPENDITURE, //DateTime.Parse(model.DATEOFEXPENDITURE),
                    PURPOSE = model.PURPOSE,
                    FROMCITY = model.FROMCITY,
                    TOCITY = model.TOCITY,
                    FROMCITYID = !string.IsNullOrEmpty(model.FROMCITYID) ? long.Parse(model.FROMCITYID) : (long?)null,
                    TOCITYID = !string.IsNullOrEmpty(model.TOCITYID) ? long.Parse(model.TOCITYID) : (long?)null,
                    TIMEIN = model.TIMEIN,
                    TIMEOUT = model.TIMEOUT,
                    NOOFHOURS = decimal.Parse(model.NOOFHOURS),
                    MODEOFTRAVEL = model.MODEOFTRAVEL,
                    KMCOVERED = !string.IsNullOrEmpty(model.KMCOVERED) ? decimal.Parse(model.KMCOVERED) : (decimal?)null,
                    RATEPERKM = !string.IsNullOrEmpty(model.RATEPERKM) ? decimal.Parse(model.RATEPERKM) : (decimal?)null,
                    TRAVELREIMBURSEMENT = !string.IsNullOrEmpty(model.TRAVELREIMBURSEMENT) ? decimal.Parse(model.TRAVELREIMBURSEMENT) : (decimal?)null,
                    TOLLTAXMISCAMOUNT = model.TOLLTAXMISCAMOUNT,
                    TOTAL = model.TOTAL,
                    DT = model.DT,
                    FROMCITY_OTHER = model.FROMCITY_OTHER,
                    TOCITY_OTHER = model.TOCITY_OTHER,
                    MODEOFTRAVEL_OTHER = model.MODEOFTRAVEL_OTHER,
                    MEAL_REFRE_ALLW_CLAIM = model.MEAL_REFRE_ALLW_CLAIM,
                    FUEL_ALLW_CLAIM = model.FUEL_ALLW_CLAIM,
                    FUEL_REIMBURSMENT = !string.IsNullOrEmpty(model.FUEL_REIMBURSMENT) ? decimal.Parse(model.FUEL_REIMBURSMENT) : (decimal?)null,
                    DINNER_ALLW_CLAIM = model.DINNER_ALLW_CLAIM,
                    DINNER_ALLW_AMOUNT = !string.IsNullOrEmpty(model.DINNER_ALLW_AMOUNT) ? decimal.Parse(model.DINNER_ALLW_AMOUNT) : (decimal?)null,
                    LUNCH_ALLW_CLAIM = model.LUNCH_ALLW_CLAIM,
                    LUNCH_ALLW_AMOUNT = !string.IsNullOrEmpty(model.LUNCH_ALLW_AMOUNT) ? decimal.Parse(model.LUNCH_ALLW_AMOUNT) : (decimal?)null,
                    TOURSTARTFROMHOMEKM = !string.IsNullOrEmpty(model.TOURSTARTFROMHOMEKM) ? decimal.Parse(model.TOURSTARTFROMHOMEKM) : (decimal?)null,
                    REFRESHMENTMEALALLOWANCE = decimal.Parse(model.REFRESHMENTMEALALLOWANCE)
                };

                res = _Ilcs.SaveDetails(Data, loginEmp, "Edit"); // {"Edit" added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
                if (res.hasError == false)
                {
                    res.errorMessage = "Data Updated Successfully";
                }

            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.Message.ToString();
            }

            return Json(new { result = res });
        }

        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        [HttpPost]
        public async Task<JsonResult> CreateLCRequestSave(string ModelData)
        {
            try
            {
                LocalConveyanceRequestViewModel model = JsonConvert.DeserializeObject<LocalConveyanceRequestViewModel>(ModelData);
                var loginEmp = int.Parse(_sessionService.Get<string>("userID"));

                model.EMPCODE = loginEmp.ToString();
                var _DATEOFEXPENDITURE = DateTime.ParseExact(model.DATEOFEXPENDITURE, "yyyy-MM-dd", null);
                var Year = _DATEOFEXPENDITURE.Year.ToString().Substring(_DATEOFEXPENDITURE.Year.ToString().Length - 2);

                string codeString = model.EMPCODE.Length > 5 ? model.EMPCODE.Substring(model.EMPCODE.Length - 5) : model.EMPCODE;

                var MaxASOFFODID = long.Parse(codeString + Year + _DATEOFEXPENDITURE.Month.ToString() + _DATEOFEXPENDITURE.Day.ToString());

                LC_DETAIL_TEMP Data = new LC_DETAIL_TEMP
                {
                    SRNO = model.SRNO,
                    ASOFFODID = MaxASOFFODID,
                    EMPCODE = !string.IsNullOrEmpty(model.EMPCODE) ? long.Parse(model.EMPCODE) : (long?)null,
                    EMPNAME = model.EMPNAME,
                    DATEOFEXPENDITURE = _DATEOFEXPENDITURE,
                    PURPOSE = model.PURPOSE,
                    FROMCITY = model.FROMCITY,
                    TOCITY = model.TOCITY,
                    FROMCITYID = !string.IsNullOrEmpty(model.FROMCITYID) ? long.Parse(model.FROMCITYID) : (long?)null,
                    TOCITYID = !string.IsNullOrEmpty(model.TOCITYID) ? long.Parse(model.TOCITYID) : (long?)null,
                    TIMEIN = model.TIMEIN,
                    TIMEOUT = model.TIMEOUT,
                    NOOFHOURS = decimal.Parse(model.NOOFHOURS),
                    MODEOFTRAVEL = model.MODEOFTRAVEL,
                    KMCOVERED = !string.IsNullOrEmpty(model.KMCOVERED) ? decimal.Parse(model.KMCOVERED) : (decimal?)null,
                    RATEPERKM = !string.IsNullOrEmpty(model.RATEPERKM) ? decimal.Parse(model.RATEPERKM) : (decimal?)null,
                    TRAVELREIMBURSEMENT = !string.IsNullOrEmpty(model.TRAVELREIMBURSEMENT) ? decimal.Parse(model.TRAVELREIMBURSEMENT) : (decimal?)null,
                    TOLLTAXMISCAMOUNT = model.TOLLTAXMISCAMOUNT,
                    TOTAL = model.TOTAL,
                    DT = model.DT,
                    FROMCITY_OTHER = model.FROMCITY_OTHER,
                    TOCITY_OTHER = model.TOCITY_OTHER,
                    MODEOFTRAVEL_OTHER = model.MODEOFTRAVEL_OTHER,
                    MEAL_REFRE_ALLW_CLAIM = model.MEAL_REFRE_ALLW_CLAIM,
                    FUEL_ALLW_CLAIM = model.FUEL_ALLW_CLAIM,
                    FUEL_REIMBURSMENT = !string.IsNullOrEmpty(model.FUEL_REIMBURSMENT) ? decimal.Parse(model.FUEL_REIMBURSMENT) : (decimal?)null,
                    DINNER_ALLW_CLAIM = model.DINNER_ALLW_CLAIM,
                    DINNER_ALLW_AMOUNT = !string.IsNullOrEmpty(model.DINNER_ALLW_AMOUNT) ? decimal.Parse(model.DINNER_ALLW_AMOUNT) : (decimal?)null,
                    LUNCH_ALLW_CLAIM = model.LUNCH_ALLW_CLAIM,
                    LUNCH_ALLW_AMOUNT = !string.IsNullOrEmpty(model.LUNCH_ALLW_AMOUNT) ? decimal.Parse(model.LUNCH_ALLW_AMOUNT) : (decimal?)null,
                    TOURSTARTFROMHOMEKM = !string.IsNullOrEmpty(model.TOURSTARTFROMHOMEKM) ? decimal.Parse(model.TOURSTARTFROMHOMEKM) : (decimal?)null,
                    REFRESHMENTMEALALLOWANCE = decimal.Parse(model.REFRESHMENTMEALALLOWANCE)
                };

                var Date_ = DateTime.Parse(Data.DATEOFEXPENDITURE.ToString()).ToString("yyyy-MM");

                var ODDataFromSAP = await _Ilcs.GetODDATA(_sessionService.Get<string>("userID"), Date_);

                if ((ODDataFromSAP.Select(x => x.DATEOFEXPENDITURE).ToList()).Contains(Data.DATEOFEXPENDITURE))
                {
                    res.hasError = false;
                    res.errorMessage = "Record For this Cannot be added  because OD for this Date of expenditure available...";
                    return Json(new { result = res });
                }

                res = _Ilcs.SaveDetails(Data, loginEmp, "Add");
                if (res.hasError == false)
                {
                    res.errorMessage = "Data Added Successfully";
                }
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.Message.ToString();
            }

            return Json(new { result = res });
        }
        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

        [HttpPost]
        public async Task<IActionResult> UploadFiles(string Type)
        {
            string retFname = string.Empty;

            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                long EmpCode = Int32.Parse(_sessionService.Get<string>("userID"));

                try
                {
                    foreach (var file in files)
                    {
                        string fname = Path.GetFileName(file.FileName);

                        string _type = Type.Replace("/", "");
                        string FileNM = $"{_type}_{EmpCode}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fname)}";

                        //string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "LCODD");

                        //string uploadPath= Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "LCODD");
                        string uploadPath = Path.Combine(serverpath.getFileUploadPath(), "LCODD");

                        

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        string fullPath = Path.Combine(uploadPath, FileNM);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        retFname = FileNM;
                    }

                    return Json(retFname);
                }
                catch (Exception ex)
                {
                    return Json($"Error occurred. Error details: {ex.Message}");
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }


        //public ActionResult UploadFiles(string Type)
        //{
        //    string retFname = string.Empty;
        //    if (Request.Files.Count > 0)
        //    {
        //        long EmpCode = Int32.Parse(_sessionService.Get<string>("userID"));

        //        try
        //        {
        //            HttpFileCollectionBase files = Request.Files;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                HttpPostedFileBase file = files[i];
        //                string fname;

        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = file.FileName;
        //                }
        //                string _type = Type.Replace("/", "");
        //                String FileNM = _type + '_' + EmpCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fname);

        //                fname = Path.Combine(Server.MapPath("~/Uploads/LCODD"), FileNM);
        //                file.SaveAs(fname);
        //                retFname = FileNM;
        //            }
        //            return Json(retFname);
        //        }

        //        catch (Exception ex)
        //        {
        //            return Json("Error occurred. Error details: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return Json("No files selected.");
        //    }
        //}

        public JsonResult Getallowcheck(TimeSpan INTime, TimeSpan OUTTime, decimal NOOFHOURS, string MODEOFTRAVEL, int KMCOVERED)
        {
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.Getallowcheck(loginEmp, INTime, OUTTime, NOOFHOURS, MODEOFTRAVEL, KMCOVERED);
            return Json(new { result = res });
        }

        [HttpPost]
        public JsonResult SubmitData(string DtData, string HeaderData)
        {
            //string logFilePath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/LCODD/errorLog.txt");
            //StreamWriter swMailLog = null;
            //if (System.IO.File.Exists(logFilePath))
            //{
            //    swMailLog = new StreamWriter(logFilePath, true);
            //}
            //else
            //{
            //    swMailLog = System.IO.File.CreateText(logFilePath);
            //}
            //swMailLog.WriteLine("--============================================================");
            //swMailLog.WriteLine("Call  SubmitData : " + DateTime.Now.ToString());
            //swMailLog.WriteLine("For Emp :" + Session["UserId"].ToString());
            try
            {

                List<LC_DETAIL_TEMP> objDt = JsonConvert.DeserializeObject<List<LC_DETAIL_TEMP>>(DtData);
                LC_HEAD objHead = JsonConvert.DeserializeObject<LC_HEAD>(HeaderData);
                objHead.LCTRANDATE = DateTime.Now.Date;

                List<LC_DETAIL> dtdata = new List<LC_DETAIL>();

                //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

                var loginEmp = int.Parse(_sessionService.Get<string>("userID"));

                res = _Ilcs.SubmitData(objDt, objHead, loginEmp, _Employee_Details);

                dtdata = (List<LC_DETAIL>)res.resultObject;

                if (res.hasError == false)
                {
                    foreach (var item in dtdata)
                    {
                        var updated = _Ilcs.UpdateLCDetailTemp(item);

                    }
                }
                //swMailLog.WriteLine("Call : Repository Response : " + res.errorMessage.ToString());

            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.Message.ToString();
                //swMailLog.WriteLine("Call : Exception : " + ex.Message.ToString());

            }
            //swMailLog.Close();
            //swMailLog.Dispose();
            return Json(new { result = res });
        }

        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        public ActionResult CreateLCRequest()
        {
            ViewBag.City = _Ilcs.CityList();

            ViewBag.Status = "1";

            ViewBag.hasError = res.hasError;
            ViewBag.errorMessage = "";

            var model = new LocalConveyanceRequestViewModel
            {
                DATEOFEXPENDITURE = DateTime.Today.ToString("yyyy-MM-dd"),
                TIMEIN = DateTime.Now.ToString("HH:mm"),
                TIMEOUT = DateTime.Now.ToString("HH:mm"),
                KMCOVERED = "0",
                TOURSTARTFROMHOMEKM = "0"
            };

            return View(model);
        }
        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

        #endregion

        #region Local Conveyance Recommandation 

        public ActionResult LCRecoList()
        {
            return View();
        }

        public JsonResult GetLCRecoListDATA(int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {

            List<LC_HEAD> Ilist = new List<LC_HEAD>();
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            Ilist = _Ilcs.GetListDataForReco(loginEmp, Ecode, FromDate, ToDate, Status);


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(Ilist, settings);
            return Json(sJSON);
        }

        public ActionResult LCRecoEdit()
        {

            //ViewBag.id = Convert.ToInt32(Request.QueryString["id"].ToString());
            //ViewBag.EmpCode = Convert.ToInt32(Request.QueryString["EmpCode"].ToString());


            // Access query parameters using Request.Query
            if (!int.TryParse(Request.Query["id"], out int id) ||
                !int.TryParse(Request.Query["EmpCode"], out int empCode))
            {
                return BadRequest("Missing or invalid query parameters.");
            }

            ViewBag.id = id;
            ViewBag.EmpCode = empCode;

            return View();
        }

        public JsonResult GetLCRecoDATA(int ID, int EMPCODE)
        {
            res = _Ilcs.GetRequestDataForReco(ID, EMPCODE);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res.resultObject, settings);
            return Json(sJSON);
        }


        public ActionResult LCRecoODItemView()
        {

            //ViewBag.id = Convert.ToInt32(Request.QueryString["id"].ToString());

            // Access query parameters using Request.Query
            if (!int.TryParse(Request.Query["id"], out int id))
            {
                return BadRequest("Missing or invalid query parameters.");
            }

            ViewBag.id = id;

            return View();
        }

        public JsonResult ApproveRejectReco(int ID, int EMPCODE, string Response, string Remark)
        {
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.ApproveRejectReco(ID, EMPCODE, Response, Remark, loginEmp);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res, settings);
            return Json(sJSON);
        }


        #endregion

        #region Local Conveyance Approval 

        public ActionResult LCApprovalList()
        {
            return View();
        }

        public JsonResult GetLCApprovalListDATA(int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {

            List<LC_HEAD> Ilist = new List<LC_HEAD>();
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            Ilist = _Ilcs.GetListDataForApproval(loginEmp, Ecode, FromDate, ToDate, Status);


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(Ilist, settings);
            return Json(sJSON);
        }

        public ActionResult LCApprovalEdit()
        {

            //ViewBag.id = Convert.ToInt32(Request.QueryString["id"].ToString());
            //ViewBag.EmpCode = Convert.ToInt32(Request.QueryString["EmpCode"].ToString());

            // Access query parameters using Request.Query
            if (!int.TryParse(Request.Query["id"], out int id) ||
                !int.TryParse(Request.Query["EmpCode"], out int empCode))
            {
                return BadRequest("Missing or invalid query parameters.");
            }

            ViewBag.id = id;
            ViewBag.EmpCode = empCode;

            return View();
        }

        public JsonResult GetLCApprovalDATA(int ID, int EMPCODE)
        {
            res = _Ilcs.GetRequestDataForApproval(ID, EMPCODE);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res.resultObject, settings);
            return Json(sJSON);
        }


        public ActionResult LCApprovalODItemView()
        {

            //ViewBag.id = Convert.ToInt32(Request.QueryString["id"].ToString());

            // Access query parameters using Request.Query
            if (!int.TryParse(Request.Query["id"], out int id))
            {
                return BadRequest("Missing or invalid query parameters.");
            }

            ViewBag.id = id;

            return View();
        }

        public JsonResult ApproveReject(int ID, int EMPCODE, string Response, string Remark)
        {
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.ApproveReject(ID, EMPCODE, Response, Remark, loginEmp);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res, settings);
            return Json(sJSON);
        }


        #endregion

        #region Local Conveyance Admin 

        public ActionResult LCAdminList()
        {
            return View();
        }

        public JsonResult GetLCAdminListDATA(int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {

            List<LC_HEAD> Ilist = new List<LC_HEAD>();
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            Ilist = _Ilcs.GetListDataForAdmin(loginEmp, Ecode, FromDate, ToDate, Status);


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(Ilist, settings);
            return Json(sJSON);
        }

        public ActionResult LCAdminEdit()
        {

            //ViewBag.id = Convert.ToInt32(Request.QueryString["id"].ToString());
            //ViewBag.EmpCode = Convert.ToInt32(Request.QueryString["EmpCode"].ToString());

            // Access query parameters using Request.Query
            if (!int.TryParse(Request.Query["id"], out int id) ||
                !int.TryParse(Request.Query["EmpCode"], out int empCode))
            {
                return BadRequest("Missing or invalid query parameters.");
            }

            ViewBag.id = id;
            ViewBag.EmpCode = empCode;

            return View();
        }

        public JsonResult GetLCAdminDATA(int ID, int EMPCODE)
        {
            res = _Ilcs.GetRequestDataForAdmin(ID, EMPCODE);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res.resultObject, settings);
            return Json(sJSON);
        }



        public ActionResult LCODAdminView()
        {
            //ViewBag.id = Convert.ToInt32(Request.QueryString["id"].ToString());

            // Access query parameters using Request.Query
            if (!int.TryParse(Request.Query["id"], out int id))
            {
                return BadRequest("Missing or invalid query parameters.");
            }

            ViewBag.id = id;

            return View();
        }

        public JsonResult AdminSubmit(int ID, int EMPCODE, string Response, string Remark, string Fuel, string doc, string approvetype) // {doc,approvetype added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.AdminSubmit(ID, EMPCODE, Response, Remark, loginEmp, Fuel, doc, approvetype); // {doc,approvetype added} SR107643 :: LC ENHANCEMENT ::  AUMENTO

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res, settings);
            return Json(sJSON);
        }

        public JsonResult AdminSaveDetail(decimal fuelReimburse, int empCode, int asOfFodId, DateTime date)
        {


            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.AdminSaveDetail(fuelReimburse, empCode, asOfFodId, date, loginEmp);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res, settings);
            return Json(sJSON);
        }

        //// START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        //public ActionResult AdminUploadFiles()
        //{
        //    string retFname = string.Empty;
        //    if (Request.Files.Count > 0)
        //    {
        //        long EmpCode = Int32.Parse(_sessionService.Get<string>("userID"));

        //        try
        //        {
        //            HttpFileCollectionBase files = Request.Files;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                HttpPostedFileBase file = files[i];
        //                string fname;

        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = file.FileName;
        //                    //fname = Path.GetFileName(file.FileName);
        //                }
        //                String FileNM = EmpCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fname); // Added on 27-03-2025
                                                
        //                fname = Path.Combine(Server.MapPath("~/Uploads/LCODD/AdminFuelDoc"), FileNM);
        //                file.SaveAs(fname);
        //                retFname = FileNM;
        //            }
        //            return Json(retFname);
        //        }

        //        catch (Exception ex)
        //        {
        //            return Json("Error occurred. Error details: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return Json("No files selected.");
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> AdminUploadFiles()
        {
            if (Request.Form.Files.Count > 0)
            {
                long empCode = long.Parse(_sessionService.Get<string>("userID"));
                string retFname = string.Empty;

                try
                {
                    foreach (var file in Request.Form.Files)
                    {
                        if (file.Length > 0)
                        {
                            string extension = Path.GetExtension(file.FileName);
                            string fileName = $"{empCode}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                            // Use IWebHostEnvironment to get wwwroot path
                            //fname = Path.Combine(Server.MapPath("~/Uploads/LCODD/AdminFuelDoc"), FileNM);

                            string uploadPath = Path.Combine(serverpath.getFileUploadPath(), "LCODD", "AdminFuelDoc");
                            
                            //if (!Directory.Exists(uploadPath))
                            //    Directory.CreateDirectory(uploadPath);

                            string fullPath = Path.Combine(serverpath.getFileUploadPath(), "LCODD", "AdminFuelDoc", fileName);

                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            retFname = fileName;
                        }
                    }

                    return Json(new { fileName = retFname });
                }
                catch (Exception ex)
                {
                    return Json(new { error = $"Error occurred: {ex.Message}" });
                }
            }
            else
            {
                return Json(new { error = "No files selected." });
            }
        }

        #endregion

        #region Local Conveyance Admin Fuel DH 
        public ActionResult LCAdminDH_List()
        {
            return View();
        }

        public JsonResult GetLCAdminDH_ListDATA(int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {

            List<LC_HEAD> Ilist = new List<LC_HEAD>();
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            Ilist = _Ilcs.GetListDataForAdminDH(loginEmp, Ecode, FromDate, ToDate, Status);


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(Ilist, settings);
            return Json(sJSON);
        }

        public JsonResult GetLCAdminDH_DATA(int ID, int EMPCODE)
        {
            res = _Ilcs.GetRequestDataForAdminDH(ID, EMPCODE);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res.resultObject, settings);
            return Json(sJSON);
        }
        //public ActionResult LCAdminDH_Edit()
        //{

        //    ViewBag.id = Convert.ToInt32(Request.QueryString["id"].ToString());
        //    ViewBag.EmpCode = Convert.ToInt32(Request.QueryString["EmpCode"].ToString());
        //    ViewBag.filePath = Path.Combine(Server.MapPath("~/Uploads/LCODD/AdminFuelDoc"), "");

        //    return View();
        //}


        public IActionResult LCAdminDH_Edit()
        {
            // Safely read query parameters
            int id = Convert.ToInt32(Request.Query["id"]);
            int empCode = Convert.ToInt32(Request.Query["EmpCode"]);

            // Use IWebHostEnvironment for path
            string filePath = Path.Combine(serverpath.getFileUploadPath(), "LCODD", "AdminFuelDoc","");

            ViewBag.id = id;
            ViewBag.EmpCode = empCode;
            ViewBag.filePath = filePath;

            return View();
        }


        public JsonResult AdminDH_Submit(int ID, int EMPCODE, string Response, string Remark)
        {
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.AdminDH_Submit(ID, EMPCODE, Response, Remark, loginEmp);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res, settings);
            return Json(sJSON);
        }
        #endregion

        #region Local conveyance Admin OH Functions

        public ActionResult LCAdminOH_List()
        {
            return View();
        }

        public JsonResult GetLCAdminOH_ListDATA(int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {

            List<LC_HEAD> Ilist = new List<LC_HEAD>();
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            Ilist = _Ilcs.GetListDataForAdminOH(loginEmp, Ecode, FromDate, ToDate, Status);


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(Ilist, settings);
            return Json(sJSON);
        }

        public ActionResult LCAdminOH_Edit()
        {
            // Safely read query parameters
            int id = Convert.ToInt32(Request.Query["id"]);
            int empCode = Convert.ToInt32(Request.Query["EmpCode"]);
                       
            ViewBag.id = id;
            ViewBag.EmpCode = empCode;            
            return View();
        }

        public JsonResult GetLCAdminOH_DATA(int ID, int EMPCODE)
        {
            res = _Ilcs.GetRequestDataForAdminOH(ID, EMPCODE);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res.resultObject, settings);
            return Json(sJSON);
        }

        public JsonResult AdminOH_Submit(int ID, int EMPCODE, string Response, string Remark)
        {
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.AdminOH_Submit(ID, EMPCODE, Response, Remark, loginEmp);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res, settings);
            return Json(sJSON);
        }
        #endregion
        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

        #endregion

        #region  Local Conveyance Finance 

        public ActionResult LCFinanceList()
        {
            return View();


        }

        public JsonResult GetListDataForFinance(int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {

            List<LC_HEAD> Ilist = new List<LC_HEAD>();
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            Ilist = _Ilcs.GetListDataForFinance(loginEmp, Ecode, FromDate, ToDate, Status);


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(Ilist, settings);
            return Json(sJSON);
        }

        public ActionResult LCFinanceEdit()
        {

            //ViewBag.id = Convert.ToInt32(Request.QueryString["id"].ToString());
            //ViewBag.EmpCode = Convert.ToInt32(Request.QueryString["EmpCode"].ToString());

            // Access query parameters using Request.Query
            if (!int.TryParse(Request.Query["id"], out int id) ||
                !int.TryParse(Request.Query["EmpCode"], out int empCode))
            {
                return BadRequest("Missing or invalid query parameters.");
            }

            ViewBag.id = id;
            ViewBag.EmpCode = empCode;

            return View();
        }

        public JsonResult GetRequestDataForFinance(int ID, int EMPCODE)
        {
            res = _Ilcs.GetRequestDataForFinance(ID, EMPCODE);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res.resultObject, settings);
            return Json(sJSON);
        }




        public ActionResult LCODItemFinanceView()
        {
            //ViewBag.id = Convert.ToInt32(Request.QueryString["id"].ToString());

            // Access query parameters using Request.Query
            if (!int.TryParse(Request.Query["id"], out int id))
            {
                return BadRequest("Missing or invalid query parameters.");
            }

            ViewBag.id = id;

            return View("LCODItemFinanceView");
        }


        public async Task<JsonResult> FinanceSubmit(int ID, int EMPCODE, string Response, string Remark, string RequiredAmount, DateTime PostingDate, string ConvAmount, string MealAmout, string DtData)
        {
            List<LC_DETAIL> objDt = JsonConvert.DeserializeObject<List<LC_DETAIL>>(DtData);

            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            res = await _Ilcs.FinanceSubmit(ID, EMPCODE, Response, Remark, loginEmp, RequiredAmount, PostingDate, ConvAmount, MealAmout, objDt);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res, settings);
            return Json(sJSON);
        }

        //public JsonResult ViewPDF(int ID, int EMPCODE)
        //{
        //    var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
        //    res = _Ilcs.ViewPDF(ID, EMPCODE, loginEmp);

        //    JsonSerializerSettings settings = new JsonSerializerSettings
        //    {
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //    };

        //    sJSON = JsonConvert.SerializeObject(res, settings);
        //    return Json(sJSON);
        //}

        [HttpGet]
        public IActionResult ViewPDF(int ID, int EMPCODE)
        {
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            res = _Ilcs.ViewPDF(ID, EMPCODE, loginEmp);
            return Json(res);
        }


        #endregion

        public JsonResult GetListDataForRecoApproval()
        {

            List<LC_HEAD> Ilist = new List<LC_HEAD>();
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            Ilist = _Ilcs.GetListDataForRecoApproval(loginEmp);


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(Ilist, settings);
            return Json(sJSON);
        }

        public JsonResult DownloadExcelData(int Ecode, DateTime FromDate, DateTime ToDate, string Status, string Flag) // {Flag added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            List<LC_HEAD_ViewModel> result = _Ilcs.GetDataForExcelFromDB(loginEmp, Ecode, FromDate, ToDate, Status, Flag); // {Flag added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(result, settings);
            return Json(sJSON);
        }

        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        #region Approval History

        public ActionResult History(int id)
        {
            DataTable dt = new DataTable();

            try
            {
                dt = _Ilcs.GetApprovalHistory(id);
            }
            catch (Exception ex)
            {
                dt = new DataTable();
                dt.Columns.Add("ErrorMessage");
                DataRow row = dt.NewRow();
                row["ErrorMessage"] = ex.Message;
                dt.Rows.Add(row);
            }

            return View(dt);
        }

        #endregion

        public ActionResult LCRejectedData()
        {
            return View();
        }

        public JsonResult GetListDataForRejectedRequest()
        {

            List<LC_HEAD> Ilist = new List<LC_HEAD>();
            var loginEmp = int.Parse(_sessionService.Get<string>("userID"));
            Ilist = _Ilcs.GetListDataForRejectedRequest(loginEmp);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(Ilist, settings);
            return Json(sJSON);
        }

        public JsonResult OpenRejectedRequest(int ID, int EMPCODE)
        {
            res = _Ilcs.OpenRejectedRequest(ID, EMPCODE);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            sJSON = JsonConvert.SerializeObject(res.resultObject, settings);
            return Json(sJSON);
        }
        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
    }
}
