using DocumentFormat.OpenXml.InkML;
using ePortal.Application.APPX.Contracts;
using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.VQMS_DPR;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.WebUI.Controllers
{
    [SessionTimeout]
    [CSPFilter]
    public class VQMS_DPRController : Controller
    {
        #region Private Variables

        private readonly IVQMS_DPRService _service;
        private readonly ILogger<VQMS_DPRController> _logger;
        private readonly string _userId;
        private readonly string _userName;
        private readonly Employee_Details Emp;

        #endregion Private Variables

        #region Constructor
        public VQMS_DPRController(IVQMS_DPRService service, ICommonFunctions _commn, ISessionService sessionService, ILogger<VQMS_DPRController> logger
            )
        {
            _service = service;
            _logger = logger;
            _userId = sessionService.Get<string>("userID").ToString();
            _userName = sessionService.Get<string>("userName").ToString();
            Emp = sessionService.Get<Employee_Details>("Employee");
        }
        #endregion Constructor

        #region DailyPassRatioRpt
        public IActionResult DailyPassRatioRpt()
        {
            try
            {
                //txtDate.Attributes.Add("onmouseover", "Readonly()");
                //txtDate.Attributes.Add("onmouseout", "Readonly()");
                //tr_showpendingacknowledgement.Style.Add(HtmlTextWriterStyle.Display, "none");
                //SearchDailyPassRatioRpt(ddlFactory, Date,"");
                var ddlFactory = Emp.Site_Id;
                var Date = DateTime.Now.ToString("dd-MMM-yyyy");
                ViewBag.Factory = ddlFactory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SearchDailyPassRatioRpt(string ddlFactory, DateTime txtDate, string ddlShift = "")
        {
            DailyPassRatioRptViewModel data = new();
            try
            {
                var Date = txtDate.ToString("dd-MMM-yyyy");

                data = await _service.SearchDailyPassRatioRpt(ddlFactory, Date, ddlShift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }

        #endregion DailyPassRatioRpt

        #region Shop

        public async Task<IActionResult> PRD_DefectReport()
        {
            try
            {
                ViewBag.Site = Emp._SiteId.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SerchDefectReport(string txtdatefrom, string ddlSite)
        {
            var data = new List<object>();
            try
            {
                txtdatefrom = string.IsNullOrEmpty(txtdatefrom) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtdatefrom;
                data = await _service.FillChart(txtdatefrom, ddlSite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> GetModelWiseDefectReport(string strShop, string txtdatefrom, string ddlSite)
        {
            var data = new List<object>();
            try
            {
                txtdatefrom = string.IsNullOrEmpty(txtdatefrom) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtdatefrom;
                data = await _service.getModelwiseDefect_Report(strShop, txtdatefrom, txtdatefrom, ddlSite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> GetDefectWiseReport(string strShop, string txtdatefrom, string strModel, string ddlSite)
        {
            var data = new List<object>();
            try
            {
                txtdatefrom = string.IsNullOrEmpty(txtdatefrom) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtdatefrom;
                data = await _service.getdefectwise_Report(strShop, txtdatefrom, strModel, ddlSite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }
        #endregion Shop

        #region FinalInspectionReport_DPR

        public async Task<IActionResult> FinalInspectionReport_DPR()
        {
            return View();
        }
        public async Task<IActionResult> SearchFinalInspectionReport_DPR(string txtDate, string ddlSite, string ddline, string ddlModel)
        {
            var data = new FinalInspectionDefectViewModel();
            try
            {
                txtDate = string.IsNullOrEmpty(txtDate) ? DateTime.Now.ToString("dd-MMM-yyyy") : txtDate;

                data.ReportHeader = await _service.LoadReportHeader(txtDate, ddlModel, ddline, ddlSite);

                data.SectionWiseGraph = await _service.LoadGraphSectionWise(txtDate, ddlModel, ddline, ddlSite);
                data.CategoryWiseGraph = await _service.LoadGraphCategorywise(txtDate, ddlModel, ddline, ddlSite);

                data.DefectWiseReport = await _service.LoadDefectDataRows(txtDate, ddlSite, ddline, ddlModel);
                data.SectionWiseReport = await _service.LoadSectionWiseData(txtDate, ddlModel, ddlSite, ddline);
                data.CategoryWiseReport = await _service.LoadCategorywiseData(txtDate, ddlModel, ddline, ddlSite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }
        public async Task<JsonResult> GetLines(string SiteId)
        {
            var data = new List<SelectListItem>();
            try
            {
                var result = await _service.LoadLineDropdownlist(SiteId);
                data = result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }
        public async Task<JsonResult> GetModels(string SiteId)
        {
            var data = new List<SelectListItem>();
            try
            {
                var result = await _service.LoadModelDropdownlist(SiteId);
                data = result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }
        public async Task<JsonResult> GetDocType(string SiteId)
        {
            var data = "";
            try
            {
                data = await _service.GetDocType(SiteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }

        public async Task<IActionResult> Export(string txtDate, string ddlModel, string ddlSite, string ddline)
        {
            try
            {
                var data = await _service.GenerateHTMLReport(txtDate, ddlModel, ddlSite, ddline);

                var bytes = Encoding.UTF8.GetBytes(data);
                var fileName = $"{DateTime.Now:yyyyMMdd}.xls";
                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return BadRequest("Internal Server Error");
            }

        }
        #endregion FinalInspectionReport_DPR

        #region DefectTrendRpt

        public async Task<IActionResult> DefectTrendRpt()
        {
            return View();
        }
        public async Task<IActionResult> ExportDefectTrendRpt(string txtDate, string ddlModel, string ddlSite)
        {
            try
            {
                var data = await _service.ExportDefectTrendRpt(txtDate, ddlModel, ddlSite);

                var bytes = Encoding.UTF8.GetBytes(data);
                var fileName = $"{DateTime.Now:yyyyMMdd}.xls";
                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return BadRequest("Internal Server Error");
            }

        }
        #endregion DefectTrendRpt
    }
}
