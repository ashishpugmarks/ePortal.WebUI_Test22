using DocumentFormat.OpenXml.Spreadsheet;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Persistence.Admin.Interface;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Office.Interop.Excel;
using Org.BouncyCastle.Asn1.Ocsp;
using ePortal.Persistence.Admin.Services;
using System.Web.WebPages;
using iText.StyledXmlParser.Jsoup.Helper;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;



namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class RouteChangeController : Controller
    {
        private readonly IBusRouteService _busService;
        private readonly ISessionService _sessionService;

        public RouteChangeController(IBusRouteService service, ISessionService objSessionService)
        {
            _busService = service;
            _sessionService = objSessionService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        { 
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            var currentStops = _busService.GetBusStopsListAsync(obj.Site_Id)
                   ?? Enumerable.Empty<BusStopModel>();

            var newStops = _busService.GetBusStopsListAsync(obj.Site_Id)
                            ?? Enumerable.Empty<BusStopModel>();
            var model = new RouteChangeViewModel
            {

                EmployeeCode = _sessionService.Get<string>("userID").ToString(),
                Sites = _busService.SelectSysite(),
                CurrentStops = currentStops,
                NewStops = newStops,
                CurrentRoutes = new List<BusRouteModel>(),
                SelectedSiteId = obj.Site_Id,
                StartDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd")
            };

            AssignDropDownValue(model.Sites, obj.Site_Id, out string selectedValue);
            model.SelectedSiteId = selectedValue;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutesForStop(string stopId)
        {
            if (string.IsNullOrEmpty(stopId))
                return BadRequest("Stop ID is required.");

            var routes = _busService.GetBusRouteFromStop(stopId)
                         ?? Enumerable.Empty<BusRouteModel>();
            var result = routes.Select(r => new
            {
                routeId = r.ADBUSROUTEID,
                routeName = r.DESCRIP
            });
            return Json(result);
        }


        private void AssignDropDownValue(IEnumerable<SiteModel> sites, string site_Id, out string selectedValue)
        {
            if (site_Id != null)
            {
                string val = site_Id.ToString();
                if (sites.Any(x => x.SiteId == val))
                    selectedValue = val;
                else
                    selectedValue = sites.First().SiteId; // default first item
            }
            else
            {
                selectedValue = sites.First().SiteId; // default
            }
        }

        [HttpGet]
        public IActionResult GetCurrentRoute(string strStopID)
        {
            var route = _busService.GetCurrentRouteAsync(strStopID);
            return Json(route);
        }

        [HttpGet]
        public IActionResult GetNewStopAsync(string siteId)
        {
            var route = _busService.GetNewStopAsync(siteId);
            return Json(route);
        }
        [HttpGet]
        public IActionResult GetBusStopsListAsync(string siteId)
        {
            var route = _busService.GetBusStopsListAsync(siteId);
            return Json(route);
        }

        [HttpGet]
        public JsonResult GetMonthlyPay(string strStopId)
        {
            if (string.IsNullOrEmpty(strStopId))
            {
                return Json(new { MonthlyPay = 0 });
            }

            var ds = _busService.GetMonthlyPayFromDatabase(Convert.ToInt32(strStopId));

            if (ds == null)
            {
                // Log or debug here
                return Json(new { MonthlyPay = 0 });
            }

            var monthlyPay = ds;
            return Json(new { MonthlyPay = monthlyPay });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(RouteChangeViewModel model)
        {

            //return RedirectToAction("ViewRouteChangeReq", new { id = 24385 });
            // return RedirectToAction("CancelRouteChange", new { id = 24385 });

            if (!model.AcceptTerms)
            {
                TempData["ErrorMessage"] = "Please accept the terms of services.";
                return View("Index", model);
            }
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            var currentStops = _busService.GetBusStopsListAsync(obj.Site_Id)
                   ?? Enumerable.Empty<BusStopModel>();

            var newStops = _busService.GetBusStopsListAsync(obj.Site_Id)
                            ?? Enumerable.Empty<BusStopModel>();
            var currentRoute = _busService.GetBusRouteFromStop(model.CurrentStopId)
                            ?? Enumerable.Empty<BusRouteModel>();
            model.Sites = _busService.SelectSysite();
            model.CurrentStops = currentStops;
            model.NewStops = newStops;
            model.CurrentRoutes = currentRoute;
            model.CurrentRouteId = model.CurrentRouteId?.ToString() ?? string.Empty;
            try
            {

                //Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                string strEmpName = obj.Employee_Name;
                string strEmpCode = _sessionService.Get<string>("userID");
                string strRequest = model.RequestType.ToString();
                string strCurrentRoute = model.CurrentRouteId?.ToString() ?? string.Empty;
                string strCurrentStop = model.CurrentStopId?.ToString() ?? string.Empty;
                string strNewRoute = model.NewStopId?.ToString() ?? string.Empty;
                string strNewStop = model.NewStopId?.ToString() ?? string.Empty;
                string strAddress = model.Address;
                string strRemark = model.Remarks;
                string strDate = model.StartDate;
                decimal monthlyPay = model.MonthlyPay;
                string strPay = monthlyPay.ToString();
                var result = _busService.SubmitRouteRequest(strEmpCode, strRequest, strCurrentRoute, strCurrentStop,
                                                         strNewRoute, strNewStop, strAddress, strRemark, strDate, strPay);
                var resultParts = result.Split('#');
                string status = resultParts[0];
                string errorMsg = resultParts[1];
                bool retVal = true;
                if (status == "1")
                {
                    retVal = _busService.SentMailforSubmit(strEmpCode, strEmpName, strRequest, strDate, strAddress);
                    // Response.Redirect("../ManageRequest.aspx", false);

                    return RedirectToAction(
                    actionName: "RedirectToOldApp",
                    controllerName: "TokenBridge",
                    routeValues: new { target = "/ASPXView/ManageRequest.aspx" }
                    );
                }
                else
                {
                    TempData["ErrorMessage"] = errorMsg;

                    return View("Index", model); //RedirectToAction("Index", "RouteChange");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while submitting the request.";
                return View("Index", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditRouteChange(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            if (obj == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get route change details
            var currentStops = _busService.GetBusStopsListAsync(obj.Site_Id)
                  ?? Enumerable.Empty<BusStopModel>();

            var newStops = _busService.GetBusStopsListAsync(obj.Site_Id)
                            ?? Enumerable.Empty<BusStopModel>();

            if (currentStops == null && newStops == null)
            {
                return NotFound();
            }
            DataSet ObjDs = _busService.EditRouteChange(id);
            var row = ObjDs.Tables[0].Rows[0];
            var model = new RouteChangeViewModel
            {
                SelectedRequestId = id,
                EmployeeCode = _sessionService.Get<string>("userID").ToString(),
                Sites = _busService.SelectSysite(),
                CurrentStops = currentStops,
                NewStops = newStops,
                CurrentRoutes = _busService.GetBusRouteFromStop(row["ADCURBUSSTOPID"].ToString()),
                StartDate = row["StartDate"].ToString(),
                RequestType = row["ISBUSMEMBERSHIPORROUTECHANGE"].ToString(),
                SelectedSiteId = obj.Site_Id,
                CurrentStopId = row["ADCURBUSSTOPID"].ToString(),
                CurrentRouteId = row["ADCURBUSROUTEID"].ToString(),
                NewStopId = row["ADREQBUSSTOPID"].ToString(),
                Address = row["RESIDENTIALADDRESS"].ToString(),
                Remarks = row["REMARKS"].ToString(),
                MonthlyPay = Convert.ToDecimal(row["MONTHLYCHARGES"])
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRouteChange(RouteChangeViewModel model)
        {

            if (!model.AcceptTerms)
            {

                TempData["ErrorMessage"] = "Please accept the terms of services.";
                return View("EditRouteChange", model);
            }
            string requestid = model.SelectedRequestId.ToString();
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            var currentStops = _busService.GetBusStopsListAsync(obj.Site_Id)
                   ?? Enumerable.Empty<BusStopModel>();

            var newStops = _busService.GetBusStopsListAsync(obj.Site_Id)
                            ?? Enumerable.Empty<BusStopModel>();
            var currentRoute = _busService.GetBusRouteFromStop(model.CurrentStopId)
                            ?? Enumerable.Empty<BusRouteModel>();
            model.Sites = _busService.SelectSysite();
            model.CurrentStops = currentStops;
            model.NewStops = newStops;
            model.CurrentRoutes = currentRoute;
            model.CurrentRouteId = model.CurrentRouteId?.ToString() ?? string.Empty;
            try
            {
                string strEmpCode = _sessionService.Get<string>("userID");
                string strEmpName = obj.Employee_Name;
                string strRequest = model.RequestType.ToString();
                string strCurrentRoute = model.CurrentRouteId?.ToString() ?? string.Empty;
                string strCurrentStop = model.CurrentStopId?.ToString() ?? string.Empty;
                string strNewRoute = model.NewStopId?.ToString() ?? string.Empty;
                string strNewStop = model.NewStopId?.ToString() ?? string.Empty;
                string strAddress = model.Address;
                string strRemark = model.Remarks;
                string strDate = model.StartDate;
                decimal monthlyPay = model.MonthlyPay;
                string strPay = monthlyPay.ToString();
                var result = _busService.UpdateRouteRequest(requestid, strRequest, strCurrentRoute, strCurrentStop,
                                                         strNewRoute, strNewStop, strAddress, strRemark, strDate, strPay);
                var resultParts = result.Split('#');
                string status = resultParts[0];
                string errorMsg = resultParts[1];
                bool retVal = true;
                if (status == "1")
                {
                    retVal = _busService.SentMailforUpdate(strEmpCode, strEmpName, strRequest, requestid, strDate, strAddress);
                    // Response.Redirect("../ManageRequest.aspx", false);

                    return RedirectToAction(
                 actionName: "RedirectToOldApp",
                 controllerName: "TokenBridge",
                 routeValues: new { target = "/ASPXView/ManageRequest.aspx" }
                 );

                }
                else
                {
                    TempData["ErrorMessage"] = errorMsg;
                    return View("EditRouteChange", model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while submitting the request.";
                return View("EditRouteChange", model);
            }
        }

        /// <summary>
        /// View Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ViewRouteChangeReq(long id)
        {
            var routeChangeRequest = _busService.GetRouteChangeRequest(id);

            if (routeChangeRequest == null)
            {
                return NotFound();
            }

            return View(routeChangeRequest);
        }

        [HttpGet]
        public IActionResult CancelRouteChange(long id)
        {
            var routeChangeRequest = _busService.GetRouteChangeRequest(id);

            if (routeChangeRequest == null)
            {
                return NotFound();
            }
            routeChangeRequest.SelectedRequestId = (int)id;
            return View(routeChangeRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelRouteChange(RouteChangeRequest model)
        {
            if (string.IsNullOrEmpty(model.Request) || string.IsNullOrEmpty(model.CancelRemarks))
            {
                return View("CancelRouteChange", model);
            }
            string requestid = model.SelectedRequestId.ToString();
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
           

            try
            {
                string strEmpCode = _sessionService.Get<string>("userID");
                string strEmpName = obj.Employee_Name;
                string strRequest = model.Request.ToString();
                string strAddress = model.Address;
                string cancelRemark = model.CancelRemarks;
                string strDate = model.UsageStartDate;
                var result = _busService.CancelRouteChangeRequest(requestid, cancelRemark);
                var resultParts = result.Split('#');
                string status = resultParts[0];
                string errorMsg = resultParts[1];
                bool retVal = true;

                if (status == "1")
                {
                    retVal = _busService.SentMailforCancel(strEmpCode, strEmpName, strRequest, requestid, strDate, strAddress, cancelRemark);
                    //return Response.Redirect("../ManageRequest.aspx", false);

                    return RedirectToAction(
                 actionName: "RedirectToOldApp",
                 controllerName: "TokenBridge",
                 routeValues: new { target = "/ASPXView/ManageRequest.aspx" }
                 );
                }
                else
                {
                    TempData["ErrorMessage"] = errorMsg;
                    return View("CancelRouteChange", model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while submitting the request.";
                return View("CancelRouteChange", model);
            }
        }

    }

}

