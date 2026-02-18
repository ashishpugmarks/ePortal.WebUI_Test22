using System.Collections;
using System.Text;
using DocumentFormat.OpenXml.VariantTypes;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter(noOfNonces: 15)]
    [SessionTimeout]
    public class SeatMgmtController : Controller
    {
        private readonly ISeatMgmtService _smService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        public SeatMgmtController(ISeatMgmtService smService, ISessionService sessionService, ILogger<HomeController> logger)
        {
            _smService = smService;
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult ManageSeat()
        {
            try
            {
                //var watch = System.Diagnostics.Stopwatch.StartNew();
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                        //IS_READONLY = date_obj > DateTime.Now ? false :true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = DateTime.Now.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.GetEmpList(Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), CalHeaderList, employeeDetails);
                
                //SR102091 START
                ViewBag.MandatoryDays = _smService.WFO_Mandatory_Days();
                int parmValue = 0;
                var counts = _smService.GetNewJoineeAndInactiveCount(SVM.EMP_LIST, SVM.CAL_HEADER_LIST, parmValue);
                SVM.NewJoineeCount = counts.Item1;
                SVM.InactiveCount = counts.Item2;
                Tuple<int, int, int ,int> _tuple = _smService.GetSeatCountByDivID(Convert.ToInt64(employeeDetails._DivId), Convert.ToInt64(employeeDetails._SiteId), SVM.NewJoineeCount, SVM.InactiveCount);
                //Tuple<int, int, int, int> _tuple = _smService.GetSeatCountByDivID(Convert.ToInt64(employeeDetails._DivId), Convert.ToInt64(employeeDetails._SiteId));//Added by aumento : SR100656 - add one tuple
                //SR102091 END

                SVM.ALLOW_SEAT = _tuple.Item1;
                SVM.ALLOW_PERCENT = _tuple.Item2;
                SVM.PHYSICALSEAT = _tuple.Item3;
                SVM.EXTRA_SEAT = _tuple.Item4;//Added by aumento : SR100656
                //watch.Stop();
                //var elapsedTime = watch.ElapsedMilliseconds;
                return View(SVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult ManageSeat(int parmValue, string parmDate)
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                DateTime crntDate = DateTime.ParseExact(parmDate, "dd-MMM-yyyy", null);
                DateTime _Date = new DateTime(crntDate.Year, crntDate.Month, 1).AddMonths(parmValue);
                int days = DateTime.DaysInMonth(_Date.Year, _Date.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(_Date.Year, _Date.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                        //IS_READONLY = date_obj > DateTime.Now ? false : true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = _Date.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.GetEmpList(Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), CalHeaderList, employeeDetails);

                //SR102091 START
                ViewBag.MandatoryDays = _smService.WFO_Mandatory_Days();
                var counts = _smService.GetNewJoineeAndInactiveCount(SVM.EMP_LIST, SVM.CAL_HEADER_LIST, parmValue);
                SVM.NewJoineeCount = counts.Item1;
                SVM.InactiveCount = counts.Item2;
                Tuple<int, int, int, int> _tuple = _smService.GetSeatCountByDivID(Convert.ToInt64(employeeDetails._DivId), Convert.ToInt64(employeeDetails._SiteId), SVM.NewJoineeCount, SVM.InactiveCount);
                //Tuple<int, int, int,int> _tuple = _smService.GetSeatCountByDivID(Convert.ToInt64(employeeDetails._DivId), Convert.ToInt64(employeeDetails._SiteId));//Added by aumento : SR100656
                //SR102091 END

                SVM.ALLOW_SEAT = _tuple.Item1;
                SVM.ALLOW_PERCENT = _tuple.Item2;
                SVM.PHYSICALSEAT = _tuple.Item3;
                SVM.EXTRA_SEAT = _tuple.Item4;//Added by aumento : SR100656
                return View(SVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPut]
        public ActionResult ManageSeat([FromBody] List<SeatRosterViewModel> ModelList)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Tuple<short, string> _tuple = _smService.SaveRoster(ModelList, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))));
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult ManageSeatOpHead()
        {
            try
            {

                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",

                        //IS_READONLY = date_obj > DateTime.Now ? false :true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = DateTime.Now.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.GetEmpListByOpHead(Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), CalHeaderList, employeeDetails);

                //SR102091 START
                ViewBag.MandatoryDays = _smService.WFO_Mandatory_Days();
                int parmValue = 0;
                var counts = _smService.GetNewJoineeAndInactiveCount(SVM.EMP_LIST, SVM.CAL_HEADER_LIST, parmValue);
                SVM.NewJoineeCount = counts.Item1;
                SVM.InactiveCount = counts.Item2;
                //Tuple<int, int, int> _tuple = _smService.GetSeatCountByOpID(Convert.ToInt64(employeeDetails._OpId), Convert.ToInt64(employeeDetails._SiteId));
                Tuple<int, int, int> _tuple = _smService.GetSeatCountByOpID(Convert.ToInt64(employeeDetails._OpId), Convert.ToInt64(employeeDetails._SiteId), SVM.NewJoineeCount, SVM.InactiveCount);
                //S102091 END

                SVM.ALLOW_SEAT = _tuple.Item1;
                SVM.ALLOW_PERCENT = _tuple.Item2;
                SVM.PHYSICALSEAT = _tuple.Item3;
                return View(SVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult ManageSeatOpHead(int parmValue, string parmDate)
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                DateTime crntDate = DateTime.ParseExact(parmDate, "dd-MMM-yyyy", null);
                DateTime _Date = new DateTime(crntDate.Year, crntDate.Month, 1).AddMonths(parmValue);
                int days = DateTime.DaysInMonth(_Date.Year, _Date.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(_Date.Year, _Date.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                        //IS_READONLY = date_obj > DateTime.Now ? false : true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = _Date.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.GetEmpListByOpHead(Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), CalHeaderList, employeeDetails);

                //SR102091 START
                ViewBag.MandatoryDays = _smService.WFO_Mandatory_Days();
                var counts = _smService.GetNewJoineeAndInactiveCount(SVM.EMP_LIST, SVM.CAL_HEADER_LIST, parmValue);
                SVM.NewJoineeCount = counts.Item1;
                SVM.InactiveCount = counts.Item2;
                //Tuple<int, int, int> _tuple = _smService.GetSeatCountByOpID(Convert.ToInt64(employeeDetails._OpId), Convert.ToInt64(employeeDetails._SiteId));
                Tuple<int, int, int> _tuple = _smService.GetSeatCountByOpID(Convert.ToInt64(employeeDetails._OpId), Convert.ToInt64(employeeDetails._SiteId), SVM.NewJoineeCount, SVM.InactiveCount);
                //SR102091 END

                SVM.ALLOW_SEAT = _tuple.Item1;
                SVM.ALLOW_PERCENT = _tuple.Item2;
                SVM.PHYSICALSEAT = _tuple.Item3;
                return View(SVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPut]
        public ActionResult ManageSeatOpHead([FromBody] List<SeatRosterViewModel> ModelList)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Tuple<short, string> _tuple = _smService.SaveRoster(ModelList, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))));
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public IActionResult ViewRoster()
         {
            try
            {
                SeatMgmtViewModel SVM = new SeatMgmtViewModel();

                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                        //IS_READONLY = date_obj > DateTime.Now ? false : true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = DateTime.Now.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.ViewRosterEmpList(Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), CalHeaderList, employeeDetails);
                if (SVM.EMP_LIST.Count > 0)
                {
                    long floorID = SVM.EMP_LIST.Select(s => s.FLOORID).FirstOrDefault();
                    SVM.MAP_FILE_NAME = _smService.GetMapFilename(floorID);
                }
                return View(SVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ViewRoster(int parmValue, string parmDate)
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                List<SeatEmpCalViewModel> EmpList = new List<SeatEmpCalViewModel>();
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                DateTime crntDate = DateTime.ParseExact(parmDate, "dd-MMM-yyyy", null);
                DateTime _Date = new DateTime(crntDate.Year, crntDate.Month, 1).AddMonths(parmValue);
                int days = DateTime.DaysInMonth(_Date.Year, _Date.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(_Date.Year, _Date.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                        //IS_READONLY = date_obj > DateTime.Now ? false : true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = _Date.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.ViewRosterEmpList(Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), CalHeaderList, employeeDetails);
                if (SVM.EMP_LIST.Count > 0)
                {
                    long floorID = SVM.EMP_LIST.Select(s => s.FLOORID).FirstOrDefault();
                    SVM.MAP_FILE_NAME = _smService.GetMapFilename(floorID);
                }
                return View(SVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public IActionResult FixSeatMaster()
        {
            List<FixSeatViewModel> model = new List<FixSeatViewModel>();
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                model = _smService.GetFixSeatList();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult FixSeatMaster([FromBody] FixSeatVM fsvm)
        {
            short retVal = 0;
            string msg = "";


            FixSeatViewModel model = new FixSeatViewModel();
            model.FIXSEATMAPID = fsvm.FIXSEATMAPID;
            model.ADEMPCODE = fsvm.ADEMPCODE;
            model.SEATNO = fsvm.SEATNO;
            model.STATUS = fsvm.STATUS;

            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Tuple<short, string> _tuple = _smService.SaveFixMapping(model.FIXSEATMAPID, model.ADEMPCODE, model.SEATNO, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), model.STATUS);
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPut]
        public IActionResult FixSeatMaster(long id, short status)
        {
            short retVal = 0;
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                retVal = _smService.UpdateStatus(id, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), status);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        //public string AutocompleteEmployee(string term)
        //{
        //    List<Employee_Details> portaluser = _smService.AutocompleteEmployee(term);
        //    List<string> list = new List<string>();
        //    foreach (var dataitem in portaluser)
        //    {
        //        list.Add(dataitem._ECode.ToString() + "-" + (dataitem._EFirstName.ToString() + " " + dataitem._ELastName.ToString()).Replace(".", string.Empty) + "");
        //    }
        //    System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    string sJSON = oSerializer.Serialize(list);
        //    return sJSON;
        //}

        [HttpGet]
        public async Task<IActionResult> AutocompleteEmployee(string term)
        {
            // Retrieve the list of guests based on the search term
            List<Employee_Details> portaluser = _smService.AutocompleteEmployee(term);

            // Format each guest's information into a string
            var result = portaluser.Select(dataItem =>
                $"{dataItem._ECode} - {dataItem._EFirstName.Replace(".", string.Empty)} - {dataItem._ELastName.Replace(".", string.Empty)}"
            ).ToList();

            // Return the result as a JSON response
            return Json(result);
        }

        //public string AutocompleteSeat(string term, long eCode)
        //{
        //    List<SeatAllocationViewModel> seat = _smService.AutocompleteSeat(term, eCode);
        //    List<string> list = new List<string>();
        //    foreach (var dataitem in seat)
        //    {
        //        list.Add(dataitem.SEAT_NAME.ToString() + "-" + dataitem.SEAT_NO.ToString() + "");
        //    }
        //    System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    string sJSON = oSerializer.Serialize(list);
        //    return sJSON;
        //}

        [HttpGet]
        public async Task<IActionResult> AutocompleteSeat(string term, long eCode)
        {
            // Retrieve the list of guests based on the search term
            List<SeatAllocationViewModel> seat = _smService.AutocompleteSeat(term, eCode);

            // Format each guest's information into a string
            var result = seat.Select(dataItem =>
                $"{dataItem.SEAT_NAME} - {dataItem.SEAT_NO}"
            ).ToList();

            // Return the result as a JSON response
            return Json(result);
        }

        [HttpGet]
        public ActionResult SeatingReport()
        {
            SearchSeatViewModel SRVM = new SearchSeatViewModel();
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");




                List<SelectListViewModel> _opList = _smService.GetOrgLevelList((long)1);
                ViewBag.OpList = new SelectList(_opList, "Value", "Text");
                List<SelectListViewModel> _divList = _smService.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "Value", "Text");

                //SRVM.OpId = employeeDetails._OpId;
                //SRVM.DivId = employeeDetails._DivId;
                return View(SRVM);
            }
            catch
            {
                return View(SRVM);
            }
        }

        [HttpPost]
        public ActionResult SeatingReport([FromBody] SearchSeatViewModel SRVM)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            List<SeatingReportViewModel> _headerList = _smService.GetSeatingReport(SRVM);
            return PartialView("_SeatingReport", _headerList);
        }

        [HttpPost]
        public ActionResult ExportToExcel([FromBody] SearchSeatViewModel SRVM)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<SeatingReportViewModel> _headerList = _smService.GetSeatingReport(SRVM);
                string str = this.excelHtml(_headerList);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        // Added by aumento for the SR56983 ===============
        //public ActionResult DownloadExcel()
        //{
        //    try
        //    {
        //        if (TempData["EXCELFILE"] == null)
        //        {
        //            return View();
        //        }
        //        string str = (string)TempData["EXCELFILE"];
        //        HttpContext.Response.AddHeader("content-disposition", "attachment; filename=SeatingDetailReport.xls");
        //        Response.ContentType = "application/vnd.ms-excel";
        //        return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("ErrorPage");
        //    }
        //}

        [HttpGet]
        public IActionResult DownloadExcel()
        {
            try
            {
                if (!TempData.ContainsKey("EXCELFILE") || TempData["EXCELFILE"] == null)
                {
                    return View();
                }

                string content = TempData["EXCELFILE"] as string;

                var fileBytes = Encoding.UTF8.GetBytes(content);
                var fileName = "SeatingDetailReport.xls";

                return File(fileBytes, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DownloadExcel: " + ex.Message.ToString());
                return RedirectToAction("ErrorPage");
            }
        }


        // Added by aumento for the SR56983 ===============
        //public ActionResult DownloadSummaryExcel()
        //{
        //    try
        //    {
        //        if (TempData["EXCELFILE"] == null)
        //        {
        //            return View();
        //        }
        //        string str = (string)TempData["EXCELFILE"];
        //        HttpContext.Response.AddHeader("content-disposition", "attachment; filename=SeatingSummaryReport.xls");
        //        Response.ContentType = "application/vnd.ms-excel";
        //        return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("ErrorPage");
        //    }
        //}

        [HttpGet]
        public IActionResult DownloadSummaryExcel()
        {
            try
            {
                if (!TempData.ContainsKey("EXCELFILE") || TempData["EXCELFILE"] == null)
                {
                    return View();
                }

                string content = TempData["EXCELFILE"] as string;

                var fileBytes = Encoding.UTF8.GetBytes(content);
                var fileName = "SeatingSummaryReport.xls";

                return File(fileBytes, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in DownloadSummaryExcel: " + ex.Message.ToString());
                return RedirectToAction("ErrorPage");
            }
        }

        public string excelHtml(List<SeatingReportViewModel> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Seat No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Building/Floor</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Associate Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (SeatingReportViewModel AHVM in _headerList)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.DATE.ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.SEATNAME + "-" + AHVM.SEATNO + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.BUILDING + "/" + AHVM.FLOOR + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.ADEMPNAME + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.ADEMPCODE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.OPERATION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.DIVISION + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        [HttpGet]
        public ActionResult DivisionWiseSeatMaster()
        {
            List<DivWiseSeatViewModel> model = new List<DivWiseSeatViewModel>();
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                model = _smService.GetDivisionWiseSeatList();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult AddDivisionSeat()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            List<SelectListViewModel> _opList = _smService.GetFloorOperationList();
            ViewBag.OpList = new SelectList(_opList, "Value", "Text");
            ViewBag.OpSeating = 0;
            //List<SelectListViewModel> _divList = _smService.GetOrgLevelList((long)2);
            //ViewBag.DivList = new SelectList(_divList, "Value", "Text");
            DivWiseSeatViewModel model = new DivWiseSeatViewModel();
            return PartialView("_DivisionSeatCapacity", model);
        }

        [HttpGet]
        public ActionResult EditDivisionSeat(long id)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            List<SelectListViewModel> _opList = _smService.GetFloorOperationList();
            ViewBag.OpList = new SelectList(_opList, "Value", "Text");
            ViewBag.OpSeating = _smService.GetFloorOperationSeatCount(id);
            //List<SelectListViewModel> _divList = _smService.GetOrgLevelList((long)2);
            //ViewBag.DivList = new SelectList(_divList, "Value", "Text");
            return PartialView("_DivisionSeatCapacity", _smService.GetDivisionWiseSeatById(id));
        }

        [HttpPost]
        public ActionResult DivisionWiseSeatMaster([FromBody] DivWiseSeatVM dwsvm)
        {

            DivWiseSeatViewModel model = new DivWiseSeatViewModel();
            short retVal = 0;
            string msg = "";
            try
            {
                model.OPERATIONID = dwsvm.OPERATIONID;
                model.DIV_LIST = dwsvm.DIV_LIST;

                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                Tuple<short, string> _tuple = _smService.SaveDivisionWiseSeat(model, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))));
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        public ActionResult BindDivisionByOperationId(long id)
        {
            int OpSeating = _smService.GetFloorOperationSeatCount(id);
            List<SelectListViewModel> divList = _smService.BindDivision(id);
            return Json(new { divList, OpSeating });
        }

        public ActionResult DivisionWiseCountReport()
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                DivWiseCountReportViewModel DCRVM = new DivWiseCountReportViewModel();
                List<SeatCalenderViewModel> CalList = new List<SeatCalenderViewModel>();

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    bool isOffDay = false; //_smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                    });
                }
                DCRVM.CAL_HEADER_LIST = CalList;
                DCRVM.DIV_LIST = _smService.GetDivisionWiseCountReport(CalList);
                DCRVM.CAL_MONTH_YEAR = DateTime.Now.ToString("MMM-yyyy");
                return View(DCRVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult DivisionWiseCountReport(int parmValue, string parmDate)
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                DivWiseCountReportViewModel DCRVM = new DivWiseCountReportViewModel();
                List<SeatCalenderViewModel> CalList = new List<SeatCalenderViewModel>();

                DateTime crntDate = DateTime.ParseExact(parmDate, "dd-MMM-yyyy", null);
                DateTime _Date = new DateTime(crntDate.Year, crntDate.Month, 1).AddMonths(parmValue);
                int days = DateTime.DaysInMonth(_Date.Year, _Date.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(_Date.Year, _Date.Month, day);
                    bool isOffDay = false; //_smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                    });
                }
                DCRVM.CAL_HEADER_LIST = CalList;
                DCRVM.DIV_LIST = _smService.GetDivisionWiseCountReport(CalList);
                DCRVM.CAL_MONTH_YEAR = _Date.ToString("MMM-yyyy");
                return View(DCRVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public IActionResult DivisionWiseCountExcel(string parmDate)
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                DivWiseCountReportViewModel DCRVM = new DivWiseCountReportViewModel();
                List<SeatCalenderViewModel> CalList = new List<SeatCalenderViewModel>();
                StringBuilder sb_calDay_rows = new StringBuilder();
                StringBuilder sb_calWeekDay_rows = new StringBuilder();

                DateTime crntDate = DateTime.ParseExact(parmDate, "dd-MMM-yyyy", null);
                DateTime _Date = new DateTime(crntDate.Year, crntDate.Month, 1);
                int days = DateTime.DaysInMonth(_Date.Year, _Date.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(_Date.Year, _Date.Month, day);
                    bool isOffDay = false; // _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    var _bgColor = isOffDay ? "#ccc" : "#b3d3ef";
                    sb_calDay_rows.Append("<th style='text-align:center;width:2.7%;background-color:'" + _bgColor + "'>" + day + "</th>");

                    var _bgColor2 = isOffDay ? "#ccc" : "#e9e9e9";
                    sb_calWeekDay_rows.Append("<th style='text-align:center;width:2.7%;background-color:'" + _bgColor2 + "'>" + date_obj.ToString("dddd").Substring(0, 3) + "</th>");

                    CalList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                    });
                }
                DCRVM.CAL_HEADER_LIST = CalList;
                DCRVM.DIV_LIST = _smService.GetDivisionWiseCountReport(CalList);
                DCRVM.CAL_MONTH_YEAR = _Date.ToString("MMM-yyyy");

                StringBuilder sb = new StringBuilder();
                sb.Append("<table cellpadding='3' cellspacing='0' border='1'");
                sb.Append("<tr><th colspan='" + (CalList.Count + 2) + "' style='text-align:center;background-color:'blue'>" + DCRVM.CAL_MONTH_YEAR + "</th></tr>");

                StringBuilder sb_div_rows = new StringBuilder();
                foreach (SeatEmpCalViewModel divObj in DCRVM.DIV_LIST)
                {
                    sb_div_rows.Append("<tr>");
                    sb_div_rows.Append("<th>" + divObj.OPERATION + "</th>");
                    sb_div_rows.Append("<th>" + divObj.DIVISION + "</th>");
                    foreach (var divCal in divObj.CAL_LIST)
                    {
                        var tdBGColor = divCal.IS_OFFDAY ? divCal.OFFDAY_COLOR : "";
                        sb_div_rows.Append("<td align='center' width='2.7%' style='background-color:'" + tdBGColor + "'>" + divCal.DATE_WISE_COUNT + "</td>");
                    }
                    sb_div_rows.Append("</tr>");
                }
                sb.Append("<tr>");
                sb.Append("<th rowspan='2'>Operation</th>");
                sb.Append("<th rowspan='2'>Division</th>");
                sb.Append(sb_calDay_rows.ToString());
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append(sb_calWeekDay_rows.ToString());
                sb.Append("</tr>");

                sb.Append(sb_div_rows.ToString());
                sb.Append("</table>");

                string str = sb.ToString();
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=DivisionWiseReport_" + DateTime.Now.ToString("ddMMyyHHmm") + ".xls");
                string filename = "DivisionWiseReport_" + DateTime.Now.ToString("ddMMyyHHmm") + ".xls";
                //Response.ContentType = "application/vnd.ms-excel";
                //return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");

                byte[] fileBytes = Encoding.UTF8.GetBytes(str);

                return File(fileBytes, "application/vnd.ms-excel", filename);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public ActionResult FloorOperationMapping()
        {
            List<FloorOperationMapViewModel> model = new List<FloorOperationMapViewModel>();
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                List<SelectListViewModel> _opList = _smService.GetOrgLevelList((long)1);
                ViewBag.OpList = new SelectList(_opList, "Value", "Text");
                List<SelectListViewModel> _floorList = _smService.GetFloorList();
                ViewBag.FloorList = new SelectList(_floorList, "Value", "Text");

                model = _smService.GetFloorOperationMapList();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult FloorOperationMapping([FromBody] FloorOperationMappingVM fomvm)
        {
            FloorOperationMapViewModel model = new FloorOperationMapViewModel();
            model.FLOOR_OPMAPPID = fomvm.FLOOR_OPMAPPID;
            model.FLOORID = fomvm.FLOORID;
            model.OPERATIONID = fomvm.OPERATIONID;
            model.SEATCAPACITY = fomvm.SEATCAPACITY;
            model.STATUS = fomvm.STATUS;

            short retVal = 0;
            string msg = "";
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                Tuple<short, string> _tuple = _smService.SaveFloorOpMapping(model, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))));
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPut]
        // public ActionResult FloorOperationMapping(long id, short status) //Added by Aumento :: SR100223
        public ActionResult FloorOperationMapping(long id, short status, string IsSeatAutoUpdated) //Added by Aumento :: SR100223
        {
            short retVal = 0;
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}


                //retVal = _smService.UpdateOpMappingStatus(id, Convert.ToInt64(Session["UserId"]), status); //Added by Aumento :: SR100223
                retVal = _smService.UpdateOpMappingStatus(id, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), status, IsSeatAutoUpdated); //Added by Aumento :: SR100223                
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult FloorSeatMaster()
        {
            List<FloorSeatMstViewModel> model = new List<FloorSeatMstViewModel>();
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                List<SelectListViewModel> _floorList = _smService.GetFloorList();
                ViewBag.FloorList = new SelectList(_floorList, "Value", "Text");

                model = _smService.GetFloorSeatMstList();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult FloorSeatMaster([FromBody] FloorSeatMstVM fsvm)
        {

            FloorSeatMstViewModel model = new FloorSeatMstViewModel();

            model.SEATMSTID = fsvm.SEATMSTID;
            model.FLOORID = fsvm.FLOORID;
            model.SEATNAME = fsvm.SEATNAME;
            model.SEATNO = fsvm.SEATNO;
            model.STATUS = fsvm.STATUS;

            short retVal = 0;
            string msg = "";
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                Tuple<short, string> _tuple = _smService.SaveFloorSeat(model, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))));
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPut]
        public ActionResult FloorSeatMaster(long id, short status)
        {
            short retVal = 0;
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                retVal = _smService.UpdateSeatStatus(id, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), status);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet] // Added by aumento for the SR56983 ===============
        public ActionResult SeatingAllocationDetailReport()
        {
            SearchSeatingDetailViewModel SRVM = new SearchSeatingDetailViewModel();
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<SelectListViewModel> _opList = _smService.GetOpeLevelList((long)1);
                ViewBag.OpList = new SelectList(_opList, "Value", "Text");
                List<SelectListViewModel> _divList = _smService.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "Value", "Text");
                List<SelectListViewModel> _building = _smService.BindBuilding();
                ViewBag.BulList = new SelectList(_building, "Value", "Text");

                ////SRVM.OpId = employeeDetails._OpId;
                ////SRVM.DivId = employeeDetails._DivId;


            }
            catch (Exception ex)
            {
                return View(SRVM);
            }
            //return View("SeatingReport_New");
            return View(SRVM);
        }

        [HttpPost] // Added by aumento for the SR56983 ===============
        public ActionResult SeatingAllocationDetailReport([FromBody] SearchSeatingDetailVM ssdvm)
        {
            SearchSeatingDetailViewModel SRVM = new SearchSeatingDetailViewModel();

            SRVM.OpId = ssdvm.OpId;
            SRVM.DivId = ssdvm.DivId;
            SRVM.Employee = ssdvm.Employee;
            SRVM.FromDate = ssdvm.FromDate;
            SRVM.ToDate = ssdvm.ToDate;
            SRVM.FLOOR_ID = ssdvm.FLOOR_ID;

            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            List<SeatingAllocationDetailReportViewModel> _headerList = _smService.GetSeatingAllocationDetail(SRVM);
            return PartialView("_SeatingAllocationDetailReport", _headerList);
        }

        [HttpPost] // Added by aumento for the SR56983 ===============
        public ActionResult ExportToExcelSeatAllocation([FromBody] SearchSeatingDetailVM ssdvm)
        {
            short retVal = 0;
            try
            {
                SearchSeatingDetailViewModel SRVM = new SearchSeatingDetailViewModel();

                SRVM.OpId = ssdvm.OpId;
                SRVM.DivId = ssdvm.DivId;
                SRVM.Employee = ssdvm.Employee;
                SRVM.FromDate = ssdvm.FromDate;
                SRVM.ToDate = ssdvm.ToDate;
                SRVM.FLOOR_ID = ssdvm.FLOOR_ID;

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<SeatingAllocationDetailReportViewModel> _headerList = _smService.GetSeatingAllocationDetail(SRVM);
                string str = this.excelHtmlSeatAllocation(_headerList);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public string excelHtmlSeatAllocation(List<SeatingAllocationDetailReportViewModel> _headerList) // Added by aumento for the SR56983 =============== 
        {
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                string str = "";
                if (_headerList.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                    stringBuilder.Append("<tr style='background-color: lightgray;'>");
                    stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Associate Name</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Building/Floor</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Date</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Seat No.</th>");
                    //SR102091 START
                    //stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Remarks</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                    //SR102091 END
                    stringBuilder.Append("</tr>");
                    int srNo = 1;
                    foreach (SeatingAllocationDetailReportViewModel AHVM in _headerList)
                    {
                        string dateFormatted = AHVM.DATE.HasValue ? AHVM.DATE.Value.ToString("dd-MMM-yyyy") : string.Empty;
                        stringBuilder.Append("<tr>");
                        stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.ADEMPCODE + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.ADEMPNAME + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.OPERATION + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.DIVISION + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.BUILDING + "/" + AHVM.FLOOR + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + dateFormatted + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.SEATNAME + "-" + AHVM.SEATNO + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.REMARKS + "</td>");
                        stringBuilder.Append("</tr>");
                    }
                    stringBuilder.Append("</table>");
                    str = stringBuilder.ToString();
                }
                return str;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpGet]  // Added by aumento for the SR56980 ===============
        public ActionResult OpDivWiseViewRoster()
        {
            try
            {
                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                ViewBag.Oplist = _smService.GetOpList((long)1);
                ViewBag.Divlist = _smService.GetOpList((long)2);

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                        //IS_READONLY = date_obj > DateTime.Now ? false : true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = DateTime.Now.ToString("MMM-yyyy");
                //SVM.EMP_LIST = _smService.ViewRosterEmpList(Convert.ToInt64(Session["UserId"].ToString()), CalHeaderList, employeeDetails);
                SVM.EMP_LIST = _smService.GetViewRosterList(Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), CalHeaderList, employeeDetails, 0, 0);
                if (SVM.EMP_LIST.Count > 0)
                {
                    long floorID = SVM.EMP_LIST.Select(s => s.FLOORID).FirstOrDefault();
                    SVM.MAP_FILE_NAME = _smService.GetMapFilename(floorID);
                }
                return View(SVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost] // Added by aumento for the SR56980 ===============
        public JsonResult OpDivWiseViewRoster(long? OpId, long? DivId)
        {
            try
            {
                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                //if (Session["UserId"] == null)
                //{
                //    return Json(new { success = false, message = "User not logged in" });
                //}
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                ViewBag.Oplist = _smService.GetOpList((long)1);
                ViewBag.Divlist = _smService.GetOpList((long)2);

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                        //IS_READONLY = date_obj > DateTime.Now ? false : true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = DateTime.Now.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.GetViewRosterList(Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), CalHeaderList, employeeDetails, OpId, DivId);
                if (SVM.EMP_LIST.Count > 0)
                {
                    long floorID = SVM.EMP_LIST.Select(s => s.FLOORID).FirstOrDefault();
                    SVM.MAP_FILE_NAME = _smService.GetMapFilename(floorID);
                }
                //return View(SVM);
                return Json(new { success = true, data = SVM });
            }
            catch (Exception ex)
            {
                //return RedirectToAction("ErrorPage");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost] // Added by aumento for the SR56980 ===============
        public JsonResult OpDivWiseViewRosterprev(long? OpId, long? DivId, int parmValue, string parmDate)
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    //return RedirectToAction("Index", "Login");
                //    return Json(new { success = false, message = "User not logged in" });
                //}

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                List<SeatEmpCalViewModel> EmpList = new List<SeatEmpCalViewModel>();
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                DateTime crntDate = DateTime.ParseExact(parmDate, "dd-MMM-yyyy", null);
                DateTime _Date = new DateTime(crntDate.Year, crntDate.Month, 1).AddMonths(parmValue);
                int days = DateTime.DaysInMonth(_Date.Year, _Date.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(_Date.Year, _Date.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                        DATE_WISE_COUNT = 0,
                        //IS_READONLY = date_obj > DateTime.Now ? false : true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = _Date.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.GetViewRosterList(Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), CalHeaderList, employeeDetails, OpId, DivId);
                if (SVM.EMP_LIST.Count > 0)
                {
                    long floorID = SVM.EMP_LIST.Select(s => s.FLOORID).FirstOrDefault();
                    SVM.MAP_FILE_NAME = _smService.GetMapFilename(floorID);
                }
                //return View(SVM);
                return Json(new { success = true, data = SVM });
            }
            catch (Exception ex)
            {
                // return RedirectToAction("ErrorPage");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet] // Added by aumento for the SR56983 ===============
        public ActionResult SeatingAllocationSummaryReport()
        {
            SearchSeatViewModel SRVM = new SearchSeatViewModel();
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<SelectListViewModel> _opList = _smService.GetOrgLevelList((long)1);
                ViewBag.OpList = new SelectList(_opList, "Value", "Text");
                List<SelectListViewModel> _divList = _smService.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "Value", "Text");

                ////SRVM.OpId = employeeDetails._OpId;
                ////SRVM.DivId = employeeDetails._DivId;


            }
            catch (Exception ex)
            {
                return View(SRVM);
            }
            //return View("SeatingReport_New");
            return View(SRVM);
        }

        [HttpPost] // Added by aumento for the SR56983 ===============
        public ActionResult SeatingAllocationSummaryReport([FromBody] SearchSeatViewModel SRVM)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            List<SeatingAllocationSummaryReportViewModel> _headerList = _smService.GetSeatingAllocationSummary(SRVM);
            return PartialView("_SeatingAllocationSummaryReport", _headerList);
        }

        [HttpPost] // Added by aumento for the SR56983 ===============
        public ActionResult ExportToExcelSummary([FromBody] SearchSeatViewModel SRVM)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<SeatingAllocationSummaryReportViewModel> _headerList = _smService.GetSeatingAllocationSummary(SRVM);
                string str = this.excelHtmlSummary(_headerList);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public string excelHtmlSummary(List<SeatingAllocationSummaryReportViewModel> _headerList) // Added by aumento for the SR56983 ===============
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Building/Floor</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Total ManPower</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Physical Seat Available</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Eligibility As Per (75%)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Allocated Seat</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Vacant Seat</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (SeatingAllocationSummaryReportViewModel AHVM in _headerList)
                {
                    string dateFormatted = AHVM.DATE.HasValue ? AHVM.DATE.Value.ToString("dd-MMM-yyyy") : string.Empty;
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + dateFormatted + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.BUILDING + "/" + AHVM.FLOOR + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.TOTAL_MANEPOWER + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.SEATCAPACITY + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.TOTAL_PHISICAL_SEAT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.ALLOCATED_SEAT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.VACANT_SEAT + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        [HttpGet] // Added by aumento for the SR57671 ===============
        public ActionResult ManageSeatOpHeadForOffDay()
        {
            try
            {

                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",

                        //IS_READONLY = date_obj > DateTime.Now ? false :true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = DateTime.Now.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.GetEmpListByOpHead(Convert.ToInt64(_sessionService.Get<string>("userID")), CalHeaderList, employeeDetails);
                //SR102091 START
                int parmValue = 0;
                var counts = _smService.GetNewJoineeAndInactiveCount(SVM.EMP_LIST, SVM.CAL_HEADER_LIST, parmValue);
                //SR102091 END
                if (employeeDetails.Functional_Designation == "Division Head")
                {
                    //SR102091 START
                    Tuple<int, int, int, int> _tupleDiv = _smService.GetSeatCountByDivID(Convert.ToInt64(employeeDetails._DivId), Convert.ToInt64(employeeDetails._SiteId), counts.Item1, counts.Item2);
                    //Tuple<int, int, int,int> _tupleDiv = _smService.GetSeatCountByDivID(Convert.ToInt64(employeeDetails._DivId), Convert.ToInt64(employeeDetails._SiteId));//Added by aumento : SR100656
                    //SR102091 END
                    SVM.ALLOW_SEAT = _tupleDiv.Item1;
                    SVM.ALLOW_PERCENT = _tupleDiv.Item2;
                    SVM.PHYSICALSEAT = _tupleDiv.Item3;
                    SVM.EXTRA_SEAT = _tupleDiv.Item4;//Added by aumento : SR100656
                }
                else
                {
                    //SR102091 START
                    Tuple<int, int, int> _tuple = _smService.GetSeatCountByOpID(Convert.ToInt64(employeeDetails._OpId), Convert.ToInt64(employeeDetails._SiteId), counts.Item1, counts.Item2);
                    //Tuple<int, int, int> _tuple = _smService.GetSeatCountByOpID(Convert.ToInt64(employeeDetails._OpId), Convert.ToInt64(employeeDetails._SiteId));
                    //SR102091 END
                    SVM.ALLOW_SEAT = _tuple.Item1;
                    SVM.ALLOW_PERCENT = _tuple.Item2;
                    SVM.PHYSICALSEAT = _tuple.Item3;
                }


                return View(SVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost] // Added by aumento for the SR57671 ===============
        public ActionResult ManageSeatOpHeadForOffDay(int parmValue, string parmDate)
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

                SeatMgmtViewModel SVM = new SeatMgmtViewModel();
                List<SeatCalenderViewModel> CalHeaderList = new List<SeatCalenderViewModel>();

                DateTime crntDate = DateTime.ParseExact(parmDate, "dd-MMM-yyyy", null);
                DateTime _Date = new DateTime(crntDate.Year, crntDate.Month, 1).AddMonths(parmValue);
                int days = DateTime.DaysInMonth(_Date.Year, _Date.Month);
                for (int day = 1; day <= days; day++)
                {
                    DateTime date_obj = new DateTime(_Date.Year, _Date.Month, day);
                    bool isOffDay = _smService.IsOffDay(date_obj.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    CalHeaderList.Add(new SeatCalenderViewModel
                    {
                        CALID = day,
                        CAL_DATE = date_obj.ToString("dd-MMM-yyyy"),
                        CAL_DATE_FORMAT = date_obj.Date,
                        WEEK_DAY = date_obj.ToString("dddd").Substring(0, 3),
                        IS_OFFDAY = isOffDay,
                        OFFDAY_COLOR = "#ccc",
                        WORKINGDAY_COLOR = "#fff",
                        //IS_READONLY = date_obj > DateTime.Now ? false : true,
                    });
                }
                SVM.CAL_HEADER_LIST = CalHeaderList;

                SVM.CAL_MONTH_YEAR = _Date.ToString("MMM-yyyy");
                SVM.EMP_LIST = _smService.GetEmpListByOpHead(Convert.ToInt64(_sessionService.Get<string>("userID")), CalHeaderList, employeeDetails);
                //Tuple<int, int, int> _tuple = _smService.GetSeatCountByOpID(Convert.ToInt64(employeeDetails._OpId), Convert.ToInt64(employeeDetails._SiteId));
                var counts = _smService.GetNewJoineeAndInactiveCount(SVM.EMP_LIST, SVM.CAL_HEADER_LIST, parmValue); //SR102091
                if (employeeDetails.Functional_Designation == "Division Head")
                {
                    //SR102091 START
                    Tuple<int, int, int, int> _tupleDiv = _smService.GetSeatCountByDivID(Convert.ToInt64(employeeDetails._DivId), Convert.ToInt64(employeeDetails._SiteId), counts.Item1, counts.Item2);
                    //Tuple<int, int, int,int> _tupleDiv = _smService.GetSeatCountByDivID(Convert.ToInt64(employeeDetails._DivId), Convert.ToInt64(employeeDetails._SiteId));//Added by aumento : SR100656
                    //SR102091 END
                    SVM.ALLOW_SEAT = _tupleDiv.Item1;
                    SVM.ALLOW_PERCENT = _tupleDiv.Item2;
                    SVM.PHYSICALSEAT = _tupleDiv.Item3;
                    SVM.PHYSICALSEAT = _tupleDiv.Item4;//Added by aumento : SR100656
                }
                else
                {
                    //SR102091 START
                    Tuple<int, int, int> _tuple = _smService.GetSeatCountByOpID(Convert.ToInt64(employeeDetails._OpId), Convert.ToInt64(employeeDetails._SiteId), counts.Item1, counts.Item2);
                    //Tuple<int, int, int> _tuple = _smService.GetSeatCountByOpID(Convert.ToInt64(employeeDetails._OpId), Convert.ToInt64(employeeDetails._SiteId));
                    //SR102091 END
                    SVM.ALLOW_SEAT = _tuple.Item1;
                    SVM.ALLOW_PERCENT = _tuple.Item2;
                    SVM.PHYSICALSEAT = _tuple.Item3;
                }
                return View(SVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPut] // Added by aumento for the SR57671 ===============
        public ActionResult ManageSeatOpHeadForOffDay([FromBody] List<SeatRosterViewModel> ModelList)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                Tuple<short, string> _tuple = _smService.SaveRoster(ModelList, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))));
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        // Start :: Added by aumento : SR100656
        public ActionResult ExtraSeatForDivMasterList()
        {
            List<ExtrSeatMstViewModel> model = new List<ExtrSeatMstViewModel>();
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                List<SelectListViewModel> _Division = _smService.GetDivisionList();
                ViewBag.Division = new SelectList(_Division, "Value", "Text");

                model = _smService.GetExtrSeatMstList();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ExtraSeatForDiv([FromBody] ExtrSeatMstVM esmvm)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                ExtrSeatMstViewModel model = new ExtrSeatMstViewModel();
                model.SRNO = esmvm.SRNO;
                model.DIVISIONID = esmvm.DIVISIONID;
                model.EXTRASEATCOUNT = esmvm.EXTRASEATCOUNT;
                model.STATUS = esmvm.STATUS;


                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                Tuple<short, string> _tuple = _smService.SaveExtraSeatForDiv(model, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))));
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }


        [HttpPut]
        public ActionResult ExtraSeatForDiv(long id, short status)
        {
            short retVal = 0;
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                retVal = _smService.UpdateExtraSeatForDivStatus(id, Convert.ToInt64(Convert.ToString(_sessionService.Get<string>("userID"))), status);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        // End :: Added by aumento : SR100656

        //SR102091 START
        [HttpGet]
        public ActionResult ManageWFOMandatoryDays()
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                var days = _smService.GetWFO_Mandatory_Days();
                return View(days);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }
        [HttpPost]
        public JsonResult UpdateWFODays(string pname, string days)
        {
            long ModifiedBy = Convert.ToInt64(_sessionService.Get<string>("userID"));
            bool result = _smService.Update_WFO_Days(pname, days, ModifiedBy);

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update WFO Days." });
            }
        }
        [HttpGet]
        public ActionResult ManageAssociateActiveInactiveRoster()
        {
            SearchSeatViewModel SRVM = new SearchSeatViewModel();
            try
            {
                List<DivWiseSeatViewModel> model = new List<DivWiseSeatViewModel>();
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                model = _smService.GetDivisionWiseSeatList();
                List<SelectListViewModel> _opList = model
                    .GroupBy(s => new { s.OPERATIONID, s.OPERATION })
                    .Select(g => new SelectListViewModel
                    {
                        Value = g.Key.OPERATIONID,
                        Text = g.Key.OPERATION
                    }).ToList();

                ViewBag.OpList = new SelectList(_opList, "Value", "Text");

                List<SelectListViewModel> _divList = model
                    .GroupBy(s => new { s.DIVISIONID, s.DIVISION })
                    .Select(g => new SelectListViewModel
                    {
                        Value = g.Key.DIVISIONID,
                        Text = g.Key.DIVISION
                    }).ToList();

                ViewBag.DivList = new SelectList(_divList, "Value", "Text");
                SRVM.Status = -1; // to select all status on form load
                return View(SRVM);
            }
            catch
            {
                return View(SRVM);
            }
        }
        [HttpPost]
        public ActionResult ManageAssociateActiveInactiveRoster([FromBody] SearchSeatViewModel SRVM)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            List<EmpActiveInactiveforRosterViewModel> _headerList = _smService.GetAssociatedata(SRVM);
            return PartialView("_AssociateActiveInactiveList", _headerList);
        }
        [HttpPost]
        public ActionResult UpdateEmployeeStatus(int empCode)
        {
            try
            {
                int userid = Convert.ToInt32(_sessionService.Get<string>("userID"));

                bool result = _smService.SaveEmployeeRoster(empCode, userid);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult ExportToExcelActiveInactiveDetail([FromBody] SearchSeatViewModel SRVM)
        {
            int retVal = 0;
            try
            {
                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

                List<EmpActiveInactiveforRosterViewModel> _headerList = _smService.GetAssociatedata(SRVM);
                string str = this.excelHtml_(_headerList);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (int)-1;
            }
            return Json(retVal);
        }
        public string excelHtml_(List<EmpActiveInactiveforRosterViewModel> _headerList)
        {
            //Employee_Details employeeDetails = (Employee_Details)this.Session["Employee"];
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            string str = "";
            if (_headerList.Count > 0)
            {
                var sortedList = _headerList
                                .OrderBy(x => x.OPERATION)
                                .ThenBy(x => x.DIVISION)
                                .ThenBy(x => x.DEPARTMENT)
                                .ThenBy(x => x.EMPLOYEENAME)
                                .ToList();
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Opration</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Department</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Adempcode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Added Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Updated Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (EmpActiveInactiveforRosterViewModel RD in sortedList)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + RD.OPERATION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + RD.DIVISION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + RD.DEPARTMENT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + RD.ADEMPCODE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + RD.EMPLOYEENAME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (RD.CREATED_DATE.HasValue ? RD.CREATED_DATE.Value.ToString("dd-MM-yyyy") : "") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (RD.UPDATED_DATE.HasValue ? RD.UPDATED_DATE.Value.ToString("dd-MM-yyyy") : "") + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + (RD.ACTIVITY_STATUS == 1 ? "Active" : "Deactive") + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }
        public ActionResult ManageAssociateActiveInactiveRosterHistory(long empId)
        {
            var list = new List<EmpActiveInactiveforRosterLogViewModel>();
            list = _smService.GetAssociateActiveInactiveRosterHistory(empId);
            return View(list);
        }
        public ActionResult ManageWFOMandatoryDaysHistory(string Statement, string Description)
        {
            var list = new List<MandatoryWFODaysViewModel>();
            list = _smService.GetWFOMandatoryDaysHistory(Statement, Description);
            return View(list);
        }
        //SR102091 END

    }
}
