using ePortal.Application.Contracts;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Text;

namespace HMSI.ePortal.Web.Controllers
{
    public class FoundationDayBookingController : Controller
    {
        IFoundationDayBookingService _bcService;
        IEportalESS objess;
        private readonly ISessionService _sessionService;
        public FoundationDayBookingController(IFoundationDayBookingService bcService, IEportalESS objEportalESS, ISessionService objsessionService)
        {
            _bcService = bcService;
            objess = objEportalESS;
            _sessionService = objsessionService;
        }

        // GET: BikerCafe
        public ActionResult Index()
        {
            return View();
        }
        //// Family Booking 
        public async Task<List<FoundationDayBookingDtlViewModel>> GetFamilyDtl()
        {
            List<FoundationDayBookingDtlViewModel> _finaFamilyDtl = new List<FoundationDayBookingDtlViewModel>();
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                _finaFamilyDtl.Add(new FoundationDayBookingDtlViewModel
                {
                    MEMBER_NAME = employeeDetails._EName,
                    RELATION_TYPE = "Self"
                });
             
                DataTable dt = new DataTable();
                dt =await objess.GetFamilylist(_sessionService.Get<string>("userID"), "");
                if (dt.Rows.Count > 0)
                {

                    List<FoundationDayBookingDtlViewModel> _familyDtl = (from DataRow row in dt.AsEnumerable()
                                                                         where row["VALIDEND"].ToString()=="9999-12-31"
                                                                         select new FoundationDayBookingDtlViewModel
                                                                     {
                                                                         MEMBER_NAME = row["FIRSTNAME"].ToString() + " " + row["LASTNAME"].ToString(),
                                                                         RELATION_TYPE = row.ItemArray[32].ToString(),
                                                                     }).ToList();
                    if (_familyDtl.Count > 0)
                    {
                        _finaFamilyDtl.AddRange(_familyDtl);
                    }
                }
            }
            catch (Exception ex)
            {
                _finaFamilyDtl = new List<FoundationDayBookingDtlViewModel>();
            }
            return _finaFamilyDtl;
        }
        

            [HttpGet]
        public async Task<ActionResult> FoundationDayBooking()
        {
            FoundationDayBookingTrnViewModel model = new FoundationDayBookingTrnViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                string end_date = "", start_date = "";
               
                FD_VALIDATION_ViewModel validModel = _bcService.GetValidationData(Convert.ToInt64(employeeDetails.Site_Id));
                if (validModel != null)
                {
                    ViewBag.Site_id=validModel.SITE_ID;
                    ViewBag.BookingID = validModel.BC_MST_VALIDATION_ID;
                    end_date = validModel.VISIT_END_DATE.ToString("dd-MM-yyyy");
                    start_date = validModel.VISIT_START_DATE.ToString("dd-MM-yyyy");
                    //ViewBag.END_DATE = end_date;
                    //ViewBag.START_DATE = start_date;
                    ViewBag.BOOKING_ALLOWED = "0";
                    if (DateTime.Today.Date >= DateTime.ParseExact(start_date, "dd-MM-yyyy", null) && DateTime.Today.Date <= DateTime.ParseExact(end_date, "dd-MM-yyyy", null))
                    {
                        ViewBag.BOOKING_ALLOWED = "1";
                    }
                    model.strBOOKED_DATE = validModel.VISIT_BOOKING_DATE.ToString("dd-MM-yyyy");
                    model.MEALBOOKING_DTL =await GetFamilyDtl();

                    #region "Bind Type"
                    List<SelectListItem> iTypeLst = new List<SelectListItem>();
                    if (model.MEALBOOKING_DTL.Where(m => m.RELATION_TYPE == "Father").Count() == 0)
                    {
                        iTypeLst.Add(new SelectListItem() { Text = "Father", Value = "Father" });
                    }
                    if (model.MEALBOOKING_DTL.Where(m => m.RELATION_TYPE == "Mother").Count() == 0)
                    {
                        iTypeLst.Add(new SelectListItem() { Text = "Mother", Value = "Mother" });
                    }
                    if (model.MEALBOOKING_DTL.Where(m => m.RELATION_TYPE == "Spouse").Count() == 0)
                    {
                        iTypeLst.Add(new SelectListItem() { Text = "Spouse", Value = "Spouse" });
                    }
                    iTypeLst.Add(new SelectListItem() { Text = "Child", Value = "Child" });
                    ViewBag.TypeList = new SelectList(iTypeLst, "Value", "Text");
                    #endregion
                }
                else
                {
                    List<SelectListItem> iTypeLst = new List<SelectListItem>();
                    ViewBag.Site_id = Convert.ToInt64(employeeDetails.Site_Id);
                    ViewBag.TypeList = new SelectList(iTypeLst, "Value", "Text");
                    ViewBag.BOOKING_ALLOWED = "2";
                }
            }
            catch (Exception ex)
            {
                model = new FoundationDayBookingTrnViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult FoundationDayBooking([FromBody]FoundationDayBookingTrnViewModel model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                Tuple<short, string> _tuple = _bcService.SaveFamilyMealBooking(model);
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { val = retVal, msg = msg });
        }

        public ActionResult ManageFoundationBooking()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            ViewBag.mealType = "";

            List<FoundationDayBookingTrnViewModel> iList = _bcService.GetFamilyBookedMealList(Convert.ToInt64(employeeDetails.Site_Id), startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), Convert.ToInt64(_sessionService.Get<string>("userID")));

            return View(iList);
        }
        
        [HttpPost]
        public ActionResult CancelFoundationDayBooking(long id)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                retVal = _bcService.CancelBooking(id, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        
        public ActionResult FoundationBookingDetail(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View("FoundationBookingDetail", _bcService.GetRequestDtlById(id));
        }

        [HttpGet]
        public ActionResult FDDashboard()
        {
            SearchFDREPORT_ViewModel obj = new SearchFDREPORT_ViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                obj.Startdate = DateTime.Today.Date.ToString("dd-MMM-yyyy");
                obj.ENDDATE = DateTime.Today.Date.ToString("dd-MMM-yyyy");
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult FDDashboard([FromBody]SearchFDREPORT_ViewModel SSM)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SearchFDREPORT_ViewModel _headerObj = _bcService.FDReport(SSM);
            return PartialView("_DashboardList", _headerObj.SearchResult);
        }
        [HttpPost]
        public ActionResult ExportToExcelUser([FromBody]SearchFDREPORT_ViewModel SSM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SearchFDREPORT_ViewModel _headerObj = _bcService.FDReport(SSM);

                string str = this.ExportToExcelUserHtml(_headerObj.SearchResult.OrderByDescending(m=>m.RequestDate).ToList());
                TempData.Remove("FDREPORTEXCELFILE");
                TempData["FDREPORTEXCELFILE"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public ActionResult DownloadExcelUser()
        {
            try
            {
                if (TempData["FDREPORTEXCELFILE"] == null)
                {
                    return View();
                }
                string str = (string)TempData["FDREPORTEXCELFILE"];
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=FDReport.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel", "FDReport.xls");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string ExportToExcelUserHtml(List<FDREPORT_ViewModel> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Requestor ECode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Requestor Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Mobile No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Site</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Foundation Day Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Member Name</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Relation Type</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Transport Type</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Vehicle Type</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Whatsapp No.</th>");// #Upgrade Aumento added :: CR7504
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Personal Email</th>");// #Upgrade Aumento added :: CR7504
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (var AHVM in _headerList)
                {
                    string POStatus = "";
                    if (AHVM.STATUS == 1)
                        POStatus = "Booked";
                    if (AHVM.STATUS == 2)
                        POStatus = "Cancelled";

                    string TransportStatus ="";
                    if (AHVM.TRANSPORT_TYPE == 1)
                        TransportStatus = "Own Vehicle";
                    if (AHVM.TRANSPORT_TYPE == 2)
                        TransportStatus = "Transport";
                    string VehicleStatus="";
                    if (AHVM.VEHICLE_TYPE == 1)
                        VehicleStatus = "Two Wheeler";
                    if (AHVM.VEHICLE_TYPE == 2)
                        VehicleStatus = "Four Wheeler";
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ReqEcode + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.Req_NAME + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.MobileNo + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.SiteDesc + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.BookingDate.ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.RequestDate.ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.MEMBER_NAME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.RELATION_TYPE + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + POStatus + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + TransportStatus + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + VehicleStatus + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.WHATSAPPNUMBER + "</td>"); // #Upgrade Aumento added :: CR7504
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.PERSONALEMAIL + "</td>"); // #Upgrade Aumento added :: CR7504
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
                srNo += 1;
            }
            return str;
        }
    }
}