using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class TaxiRequestController : Controller
    {
        private readonly ITexiRequest _TaxiService;
        
        private readonly ICommonFunctions _CommonFunctions;
        private readonly IShiftchg _IShiftchg;
        private readonly IBusRoute _IBusRoute;
        private readonly ISessionService _sessionService;
        public TaxiRequestController(ITexiRequest TaxiService, ICommonFunctions CommonFunctions, ISessionService sessionService, IShiftchg IShiftchg, IBusRoute iBusRoute) //IPRService Added by Aumento
        {
            _TaxiService = TaxiService;
            _CommonFunctions = CommonFunctions;
            _sessionService = sessionService;
            _IShiftchg = IShiftchg;
            _IBusRoute = iBusRoute;
          
        }
        [HttpGet]
        public async Task<IActionResult> TaxiRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            TaxiRequestViewModel data = new TaxiRequestViewModel();
           
            data.SITEList = _CommonFunctions.GetSITE().AsSelectList("sysiteid", "SITE",false);
            
            data.ApproverList = _TaxiService.get_TaxiApprovalAuthList(_sessionService.Get<string>("userID")).AsSelectList("ADEMPCODE", "EMPNAME", true, "- Select Approval Authority -"); ;
            //data.SITEList = SiteData.AsEnumerable().Select(row => new SelectListItem { Value = row["sysiteid"].ToString(), Text = row["SITE"].ToString() }).ToList();

            var isValidRequest = _TaxiService.isValidForTaxiBooking(_sessionService.Get<string>("userID"));

            if (isValidRequest.ToUpper().Trim() == "NO")
            {
                data.IsEnabled = false;

            }
            else
            {
                data.IsEnabled = true;

            }
            string supervisorEmpCode = _CommonFunctions.GetSupervisorEmpCodeByUserID(_sessionService.Get<string>("userID"));

            // If Supervisor Code exists, set it as selected value
            if (!string.IsNullOrEmpty(supervisorEmpCode) && supervisorEmpCode != "0")
            {
                var selectedItem = data.ApproverList.Where(x => x.Value == supervisorEmpCode);
                if (selectedItem != null)
                {
                    data.SUPERVISORADEMPCODE = selectedItem.Count()>0? selectedItem.FirstOrDefault().Value:"";
                }
            }
            var dt = _CommonFunctions.GetEmployeeOfficialDetails(_sessionService.Get<string>("userID"), string.Empty);
            if (dt.Rows.Count > 0)
            {
                data.PHONENO =long.Parse(dt.Rows[0]["TMOBILE"].ToString());
                data.EXTENSIONO = dt.Rows[0]["EXTENSIONNO"].ToString();
                data.SITE = dt.Rows[0]["SYSITEID"].ToString();
            }
            var strFrmdate = DateTime.Now.ToString("dd-MMM-yyyy");
            data.ShiftOptions = _IShiftchg.GetShiftDetails_NEW(_sessionService.Get<string>("userID"), strFrmdate).AsSelectList_DS("code","code",false);
            
            //ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            return View("VehicleRequest", data);
        }
        [HttpPost]
        public async Task<IActionResult> TaxiRequest([FromBody] TaxiRequestRequest objModel)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
           
            var Msg = _TaxiService.SaveTaxiRequest(objModel, _sessionService.Get<string>("userName"), _sessionService.Get<string>("userID"));
            return Json(Msg);
            
        }
        [HttpGet]
        public async Task<IActionResult> TaxiRequestList()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            TaxiRequestViewModel data = new TaxiRequestViewModel();
            int intReportingSite = 0;
            var RequestList = _TaxiService.get_TaxiRequestGateList(_sessionService.Get<string>("userID"), intReportingSite,"");

            if (RequestList != null)
            {

                RequestList.DefaultView.RowFilter = "IsNull(RELEASEDBY, 0) <> 0";

                // Convert filtered view to DataTable
                var filteredTable = RequestList.DefaultView.ToTable();
           ;
                var json = JsonConvert.SerializeObject(filteredTable);
                data.taxiRequestList = JsonConvert.DeserializeObject<List<TaxiRequestList>>(json);
            }
            return View("TaxiRequestList",data);
        }
        [HttpGet]
        public async Task<IActionResult> TaxiUsesReport()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            
            TaxiRequestViewModel data = new TaxiRequestViewModel();
           
            return View("TaxiUsesReport", data);
        }

        [HttpPost]
        public async Task<IActionResult> TaxiUsesReport(string DATEOFTRAVELFROM,string DATEOFTRAVELTO)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            
            TaxiRequestViewModel data = new TaxiRequestViewModel();
            int intReportingSite = 0;
            var RequestList = _TaxiService.Get_TaxiusesReport(_sessionService.Get<string>("userID"), "0", DATEOFTRAVELFROM, DATEOFTRAVELTO, "0");

            if (RequestList != null)
            {


                var json = JsonConvert.SerializeObject(RequestList);
                data.TaxiUsesReportList = JsonConvert.DeserializeObject<List<TaxiUsesReport>>(json);
            }
            return PartialView("_TaxiRequestReportList", data);
        }

        [HttpGet]
        public async Task<IActionResult> VehicleReleased(string id,string Did)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            VehicleReleased data = new VehicleReleased();
            string strAdminApp = string.Empty;
            string strTaxiNo = string.Empty;
            string strVendName = string.Empty;
            string strAmmount = string.Empty;
            string strAdminRem = string.Empty;
            string stroperation = string.Empty;
            string strurl = string.Empty;
            string strcode = string.Empty;
            string strsiteid = string.Empty;
            int intCount;

            //CHECK PAGE RIGHTS FOR LOGGED ASSOCIATE ON PARENT PAGE
            strcode = _sessionService.Get<string>("userID");


            //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "none");

            

                    
                    data = _TaxiService.VehicleRequestById(id, Did);
            data.BUSROUTELIST = _IBusRoute.GetBusStop("0").AsSelectList_DS("ADBUSSTOPID", "DESCRIP", true, "--Select Bus Stop--");
            if (data.RELEASEMETERREADING != "")
            {
                data.IsEnabled = false;

            }


            return View("VehicleReleased", data);
        }
        [HttpPost]
        public async Task<IActionResult> VehicleReleased([FromBody]VehicleReleased model)
        {
            if (string.IsNullOrEmpty(model.ADVECHICLEDETAILID))
            {
                return Ok(new { data = "Missing request detail ID", status = "error" });
                
            }

            

            if (string.IsNullOrEmpty(model.RELEASEMETERREADING) || model.RELEASEDAT == "0")
            {
               return Ok(new { data = "Please enter release meter reading and released at.", status = "error" });

            }

            if (int.TryParse(model.RELEASEMETERREADING, out int kmUsed) && int.TryParse(model.METERREADING, out int startReading))
            {
                if (kmUsed < startReading)
                {
                   return Ok(new { data = "Release meter reading must be greater than starting meter reading.", status = "error" });

                }
            }

            if (string.IsNullOrEmpty(model.TAXICONDITIONSTATUS))
            {
                
                return Ok(new { data = "Taxi Condition is a required field.", status = "error" });

            }

            string statusResult = _TaxiService.UpdateVechileReleased(
                model.ADVECHICLEDETAILID,
                model.RELEASEMETERREADING,
                model.RELEASEDAT,
                model.VISITE_PLACE,
                model.TAXICONDITIONSTATUS,
                model.REMARKS
            );

            var statusParts = statusResult.Split('#');
            string errResult = statusParts[0];
            string errMsg = statusParts.Length > 1 ? statusParts[1] : "Unknown error";

            if (errResult == "0")
            {
                
                return Ok(new {data="success",status="success"});
            }
            else
            {
                return Ok(new { data = errMsg, status = "error" });

            }
        }
        [HttpGet]
        public async Task<IActionResult> VehicleRequestDetail(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

           
            
            VehicleRequestModel data = _TaxiService.VehicleRequestDetails(id);
            
            return View("VehicleRequestDetail", data);
        }
        [HttpGet]
        public async Task<IActionResult> CancelTaxiRequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }



            TaxiRequestViewModel data = _TaxiService.CancelRequestDetails(id);

            return View("CancelTaxiRequest", data);
        }
        [HttpPost]
        public async Task<IActionResult> CancelRequest([FromBody] TaxiRequestRequest objModel)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var Msg = _TaxiService.SubmitCancelTaxiRequest(objModel, _sessionService.Get<string>("userID"), _sessionService.Get<string>("userName"));
            return Json(Msg);

        }

        [HttpGet]
        public async Task<IActionResult> EditTaxiRequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            TaxiRequestViewModel data = new TaxiRequestViewModel();
            data = _TaxiService.EditRequestDetails(id);
            data.SITEList = _CommonFunctions.GetSITE().AsSelectList("sysiteid", "SITE", false);

            data.ApproverList = _TaxiService.get_TaxiApprovalAuthList(_sessionService.Get<string>("userID")).AsSelectList("ADEMPCODE", "EMPNAME", true, "- Select Approval Authority -"); ;
            //data.SITEList = SiteData.AsEnumerable().Select(row => new SelectListItem { Value = row["sysiteid"].ToString(), Text = row["SITE"].ToString() }).ToList();
            var strFrmdate = data.DATEOFTRAVELFROM;
            data.ShiftOptions = _IShiftchg.GetShiftDetails_NEW(_sessionService.Get<string>("userID"), strFrmdate).AsSelectList_DS("code", "code", false);
            data.IsEnabled = true;
            //ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            return View("EditTaxiRequest", data);
        }
        [HttpPost]
        public async Task<IActionResult> EditTaxiRequest([FromBody] TaxiRequestRequest objModel)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var Msg = _TaxiService.EditTaxiRequest(objModel, _sessionService.Get<string>("userName"), _sessionService.Get<string>("userID"));
            return Json(Msg);

        }

        [HttpGet]
        public async Task<IActionResult> TaxiApproval(string id,string ecode)
        {
            //CHECK QUERY STRING IS MISSING THEN REDIRECT PAGE TO PARENT PAGE
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

                    //HIDE THE ERROR PANEL
                    //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "none");

                    string strRequestid = "";
                strRequestid = id;
                    string strEcode = "";
                strEcode = ecode;
                    string strrequesterempcode = string.Empty;
                    string strdivheadempcode = string.Empty;

                    //GET REQUEDT DETAILS AGAINST REQUEST ID AND EMPLOYEE CODE
                   
                    DataSet objTaxiReqDS = _TaxiService.EditTaxiApproval(strRequestid, strEcode);

                var data = new TaxiRequestViewModel();
            data.REQUESTID = id;
            data.EMPCODE = ecode;
                data.EMPNAME = objTaxiReqDS.Tables[0].Rows[0][2].ToString();
                data.EMAIL = objTaxiReqDS.Tables[0].Rows[0][3].ToString();
                data.APPLICATIONDATE = objTaxiReqDS.Tables[0].Rows[0][7].ToString();
                data.DATEOFTRAVELFROM = objTaxiReqDS.Tables[0].Rows[0][4].ToString();
                data.DATEOFTRAVELTO = objTaxiReqDS.Tables[0].Rows[0][5].ToString();
                data.REPORTINGTIME = objTaxiReqDS.Tables[0].Rows[0][9].ToString();
                data.PURPOSEOFVISIT = objTaxiReqDS.Tables[0].Rows[0][10].ToString();
                data.PLACEOFVISIT = objTaxiReqDS.Tables[0].Rows[0][11].ToString();
                data.NOOFPERSON = int.TryParse(objTaxiReqDS.Tables[0].Rows[0][12].ToString(), out _) ? Convert.ToInt32(objTaxiReqDS.Tables[0].Rows[0][12].ToString()) : (int?)null ;
                data.REMARKS = objTaxiReqDS.Tables[0].Rows[0][13].ToString();
                data.ADDRESS = objTaxiReqDS.Tables[0].Rows[0][14].ToString();
                data.EXTENSIONO = objTaxiReqDS.Tables[0].Rows[0][15].ToString();
                data.PHONENO =long.Parse( objTaxiReqDS.Tables[0].Rows[0][16].ToString());
                data.DEPTMGRREMARKS = objTaxiReqDS.Tables[0].Rows[0][17].ToString();
                data.DEPTMGRSSTAUS = objTaxiReqDS.Tables[0].Rows[0][20].ToString();
                if (objTaxiReqDS.Tables[0].Rows[0][18].ToString() == "0")
                    {
                       
                    data.IsEnabled = true;
                }
                    else
                    {
                    data.IsEnabled = false;
                       
                    }

                    if (objTaxiReqDS.Tables[0].Rows[0][18].ToString() == "1")
                    {
                        data.SecondApproval = false;
                    }
                    else
                    {
                        data.SecondApproval = true;
                    }
                    data.SUPERVISORADEMPCODE = objTaxiReqDS.Tables[0].Rows[0][1].ToString();
                    data.HDN_MFG_OPID = objTaxiReqDS.Tables[0].Rows[0][21].ToString();
            data.HDN_TAPUKARA_ID = objTaxiReqDS.Tables[0].Rows[0][22].ToString();
                    //TWO LEVEL APPROVAL REQUIRED FOR MANAFACTURING OPERATION
                    string MFG_OPID = _CommonFunctions.GetParameterValue("HRKRA_ADVIPID_MFG");
                    string TAPUKARA_ID = _CommonFunctions.GetParameterValue("TAPUKARA_SITE_ID");
                    if (objTaxiReqDS.Tables[0].Rows[0][21].ToString() != MFG_OPID)
                    {
                    data.SecondApproval = false;
                    }
                    if (objTaxiReqDS.Tables[0].Rows[0][21].ToString() == MFG_OPID && data.HDN_TAPUKARA_ID == TAPUKARA_ID)
                    {
                        data.SecondApproval = false;
                    }
                    DataSet Ds = _TaxiService.getDivisionHead(data.SUPERVISORADEMPCODE);
                    strdivheadempcode = Ds.Tables[0].Rows[0][0].ToString();
            
            if (data.HDN_MFG_OPID == MFG_OPID && data.HDN_TAPUKARA_ID != TAPUKARA_ID)
            {data.SecondApproval = true;
            // FILL SECOND LEVEL APPROVING AUTHORITIES
            data.ApproverList = _TaxiService.get_AllDivHead(data.SUPERVISORADEMPCODE).AsSelectList_DS("ADEMPCODE", "EMPNAME", true, " - Select Approval Authority - ", strdivheadempcode.ToString());

            }
            return View("TaxiApproval", data);
                }
        [HttpPost]
        public async Task<IActionResult> TaxiFinalApproval([FromBody] TaxiRequestRequest objModel)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var Msg = _TaxiService.TaxiApproval(objModel, _sessionService.Get<string>("userName"), _sessionService.Get<string>("userID"));
            return Json(Msg);

        }

        [HttpGet]
        public async Task<IActionResult> VehicleReport()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            TaxiRequestViewModel data = new TaxiRequestViewModel();

            return View("VehicleReport", data);
        }
        [HttpPost]
        public async Task<IActionResult> VehicleReport(string DATEOFTRAVELFROM,string DATEOFTRAVELTO)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            TaxiRequestViewModel data = new TaxiRequestViewModel();
           var RequestList = _TaxiService.GetTaxiRecord(_sessionService.Get<string>("userID"),DateTime.Parse( DATEOFTRAVELFROM).ToString("MM/dd/yyyy"), DateTime.Parse(DATEOFTRAVELTO).ToString("MM/dd/yyyy"));
            
            if (RequestList != null)
            {


                var json = JsonConvert.SerializeObject(RequestList.Tables[0]);
                data.VehicleReport = JsonConvert.DeserializeObject<List<VehicleReport>>(json);
            }
            return View("_VehicleReportList", data);
        }

        [HttpGet]
        public async Task<IActionResult> PrintableList(string sfromdate, string stodate)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            TaxiRequestViewModel data = new TaxiRequestViewModel();
            int intReportingSite = 0;
            var RequestList = _TaxiService.Get_TaxiusesReport(_sessionService.Get<string>("userID"), "0", sfromdate, stodate, "0");

            if (RequestList != null)
            {


                var json = JsonConvert.SerializeObject(RequestList);
                data.TaxiUsesReportList = JsonConvert.DeserializeObject<List<TaxiUsesReport>>(json);
                data.EMPNAME = data.TaxiUsesReportList[0].EMPNAME;
            }
            
            data.DATEOFTRAVELFROM = sfromdate;
            data.DATEOFTRAVELTO = stodate;
            return View(data);
          
        }
        

    }
}
  
