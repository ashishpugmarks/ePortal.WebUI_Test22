using DocumentFormat.OpenXml.Bibliography;
using ePortal.Application.Contracts;
using ePortal.Application.Services;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.Intrinsics.X86;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class ContineousWorkingController : Controller
    {
        private readonly IContineousWorkingService _ContineousWorkingService;
        private readonly ISessionService _sessionService;
        public ContineousWorkingController(IContineousWorkingService ContineousWorkingService, ISessionService objSession)
        {
            _ContineousWorkingService = ContineousWorkingService;
            _sessionService = objSession;
        }

        [HttpGet]
        public ActionResult ContineousWorkingDashboard()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

            }
            catch (Exception ex)
            {
                return View();
            }
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> ContineousWorkingDashboard([FromBody] SearchContineousWorkingViewModel SWM)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            var dataList = await _ContineousWorkingService.ContineousWorkingDashboard(SWM, employeeDetails.Employee_Code);

            return PartialView("_ContineousWorkingDashbordList", dataList);

        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] SearchContineousWorkingViewModel SWM)
        {
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");

            Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");

            var list = await _ContineousWorkingService.ContineousWorkingDashboard(SWM, emp.Employee_Code);

            if (list == null || !list.Any())
                return Json(new { fileName = "" });

            string fileName = $"ContinuousWorkingReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Report");
                var table = ws.Cell(1, 1).InsertTable(list);
                table.Field("SITE_DESCRIP").Name = "LOCATION";
                ws.Columns().AdjustToContents();

                table.Theme = ClosedXML.Excel.XLTableTheme.TableStyleMedium9;
                workbook.SaveAs(filePath);
            }

            return Json(new { fileName });
        }

        public IActionResult DownloadExcel(string file)
        {
            string filePath = Path.Combine(Path.GetTempPath(), file);

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                file);
        }

    }
}
