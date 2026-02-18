using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ePortal.Application.Contracts;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ePortal.WebUI.Filters;
using ePortal.Application.Services;
using static ePortal.ViewModels.VehicleDTO;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics.Contracts;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly ISessionService _sessionService;
        private readonly IConfiguration _configuration;
        private readonly IAppConfigurationService _configurations;

        public VehicleController(IVehicleService vehicleservice, ISessionService sessionService, IConfiguration configuration, IAppConfigurationService appConfigurations)
        {
            _vehicleService = vehicleservice;
            _sessionService = sessionService;
            _configurations = appConfigurations;
        }
        public IActionResult Create()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var model = new VehicleUpsertDto();

            model.Vehicles.Add(new VehicleDetailDto());

            return View(model);
        }

        [HttpPost]
        public IActionResult Create([FromForm] VehicleUpsertDto model)
        {
            var emp = _sessionService.Get<Employee_Details>("Employee");
            var userId = _sessionService.Get<string>("userID");

            var result = _vehicleService.SaveVehicleRequest(model, emp, userId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public IActionResult GetEmployeeByCode(string empCode)
        {
            if (!long.TryParse(empCode, out var empCodeLong))
            {
                return Json(new { success = false, Message = "Invalid employee code." });
            }

            var result = _vehicleService.GetEmployeeDetailsByEmpCode(empCodeLong);

            if (result.FullName == null)
            {
                return Json(new { success = false, Message = "Employee not found." });
            }

            return Json(new
            {
                result,
                success = true,
            });
        }

        public IActionResult Detail(int id)
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");

            var result = _vehicleService.GetVehicleDetailById(id);
            if (result == null)
                return NotFound();

            return View(result);
        }

        public IActionResult Edit(int id)
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");

            var result = _vehicleService.GetEditVehicleDetailById(id);

            if (result == null)
                return NotFound();

            if (!_vehicleService.CanEdit(result))
                return RedirectToAction("Home", "Home");

            return View(result);
        }

        [HttpPost]
        public IActionResult Edit([FromForm] VehicleUpsertDto model)
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");

            if (model == null)
                return BadRequest(new VehicleResponseDto(false, "Invalid request payload."));

            try
            {
                var result = _vehicleService.EditVehicleWithDocsAsync(model, userId);
                return Ok(result);
            }
            catch
            {
                return StatusCode(500,
                    new VehicleResponseDto(false, "Something went wrong while updating vehicle."));
            }
        }

        [HttpGet]
        public IActionResult MyRequest()
        {
            var userIdString = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Index", "Login");

            if (!long.TryParse(userIdString, out long userId))
                return BadRequest(new VehicleResponseDto(false, "Invalid user id in session."));

            try
            {
                var result = _vehicleService.MyVehicleRequests(userId);

                if (!result.Success)
                    return BadRequest(result);

                var list = result.Data as List<VehicleMasterDto>;
                if (list == null)
                    return BadRequest(new VehicleResponseDto(false, "Invalid data format received."));

                return View(list);
            }
            catch
            {
                return StatusCode(500,
                    new VehicleResponseDto(false, "Something went wrong."));
            }
        }

        [HttpPost]
        public IActionResult UpdateVehicleStatus(int id, int newStatus, string remarks = "")
        {
            if (id <= 0 || (newStatus != 1 && newStatus != 2))
            {
                return Json(new VehicleResponseDto(false, "Invalid request parameters!"));
            }
            var userId = _sessionService.Get<string>("userID");
            try
            {
                var result = _vehicleService.UpdateVehicleStatus(id, newStatus, remarks?.Trim() ?? "", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Json(new VehicleResponseDto(false, "An error occurred while updating status. Please try again."));
            }
        }

        [HttpPost]
        public IActionResult ExportAdminReportToExcel([FromBody] VehicleExportDto model)
        {
            return ExportVehicleExcelInternal(
                model,
                _vehicleService.GetAdminVehiclesForExportExcel,
                nameof(DownloadExcelFile));
        }

        [HttpPost]
        public IActionResult ExportSecurityReportToExcel([FromBody] VehicleExportDto model)
        {
            return ExportVehicleExcelInternal(
                model,
                _vehicleService.GetSecurityVehiclesForExportExcel,
                nameof(DownloadExcelFile));
        }

        public IActionResult DownloadExcelFile(string key)
        {
            if (string.IsNullOrEmpty(key))
                return Content("Invalid download request.");

            var excelHtml = _sessionService.Get<string>(key);
            if (string.IsNullOrEmpty(excelHtml))
                return Content("File expired or not available.");

            var bytes = System.Text.Encoding.UTF8.GetBytes(excelHtml);

            // Optional: cleanup
            _sessionService.Remove(key);

            return File(
                bytes,
                "application/vnd.ms-excel",
                $"VehiclesReport_{DateTime.Now:yyyyMMddHHmmss}.xls");
        }

        private IActionResult ExportVehicleExcelInternal(
            VehicleExportDto model,
            Func<VehicleExportDto, int, List<VehicleExportResponseDto>> fetchMethod,
            string downloadActionName)
        {
            try
            {
                var employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                if (employeeDetails == null)
                {
                    return Json(new { success = false, message = "Session expired. Please login again." });
                }

                var data = fetchMethod(model, Convert.ToInt32(employeeDetails._ECode));

                if (data == null || data.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No records found for the selected criteria"
                    });
                }

                string excelHtml = _vehicleService.VehicleExcelHtml(data);

                if (string.IsNullOrWhiteSpace(excelHtml))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to generate export file"
                    });
                }

                var sessionKey = $"VehicleEXCELFILE_{Guid.NewGuid()}";
                _sessionService.Set(sessionKey, excelHtml);

                return Json(new
                {
                    success = true,
                    downloadUrl = Url.Action(downloadActionName, new { key = sessionKey })
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    message = "Unexpected error occurred while exporting data"
                });
            }
        }


        public IActionResult ParkingZones()
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Index", "Login");
            }
            var employee = _sessionService.Get<Employee_Details>("Employee");
            if (employee == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                var parkingZones = _vehicleService.GetParkingZones()
                                   ?? new List<ParkingZoneDto>();

                var locations = _vehicleService.GetSiteList()
                                ?? new List<LocationDto>();

                ViewBag.Locations = locations;

                return View(parkingZones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new VehicleResponseDto(false, "Something went wrong."));
            }
        }


        [HttpPost]
        public IActionResult CreateZone([FromBody]ParkingZoneDto data)
        {
            if (data == null)
                return BadRequest("Invalid payload");

            var userId = _sessionService.Get<string>("userID");
            var emp = _sessionService.Get<Employee_Details>("Employee");
            if (emp == null)
            {
                return BadRequest("Invalid user session");
            }
            var result = _vehicleService.UpsertZone(data, userId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public IActionResult GetZoneById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            var zone = _vehicleService.GetParkingZoneById(id);

            if (zone == null)
                return NotFound();

            return Json(zone); 
        }

        [HttpPost]
        public IActionResult UpdateZone([FromBody] ParkingZoneDto data)
        {
            if (data == null)
                return BadRequest("Invalid payload");

            var userId = _sessionService.Get<string>("userID");
            var emp = _sessionService.Get<Employee_Details>("Employee");
            if (emp == null)
            {
                return BadRequest("Invalid user session");
            }

            var result = _vehicleService.UpsertZone(data, userId);

            //return result.Success ? Ok(result) : BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult DeleteZone(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            var result = _vehicleService.DeleteZone(id);

            //if (!result.Success)
            //    return BadRequest(result);

            return Ok(result);
        }

        public IActionResult VehicleAdmin()
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");

            var employee = _sessionService.Get<Employee_Details>("Employee");
            var empcode = Convert.ToInt32(employee._ECode);
            var model = _vehicleService.GetVehicleDataForAdmin(empcode);

            if (model == null || model.VehicleMaster.Count == 0)
                TempData["ErrorMessage"] = "No location mapping found for you.";

            return View(model);
        }

        [HttpGet]
        public IActionResult ZoneStatistics(int vehicleId)
        {
            var userId = _sessionService.Get<string>("userID");
            var emp = _sessionService.Get<Employee_Details>("Employee");
            if (emp == null)
            {
                return BadRequest("Invalid user session");
            }
            var result = _vehicleService.ZoneStatistics(vehicleId);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult ApproveAdminRequest([FromBody] ApproveRequestDto model)
        {
            if (model == null || model.VehicleId <= 0)
                return BadRequest(new { success = false, message = "Invalid request." });
            var emp = _sessionService.Get<Employee_Details>("Employee");
            if (emp == null)
            {
                return BadRequest("Invalid user session");
            }
            var userId = _sessionService.Get<string>("userID");

            var result = _vehicleService.ApproveRequest(model, userId);

            //if (!result.Success)
            //    return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult DeleteVehicle(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");
            var result = _vehicleService.DeleteVehicle(id);

            //if (!result.Success)
            //    return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult DeactivateVehicle([FromBody] DeactivateVehicle request)
        {
            if (request == null || request.VehicleId <= 0 || string.IsNullOrWhiteSpace(request.Remark))
                return BadRequest("Invalid payload");

            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = _vehicleService.DeactivateVehicle(request, userId);
            return Ok(result);
        }

        public IActionResult VehicleSecurity()
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");

            var employee = _sessionService.Get<Employee_Details>("Employee");
            var empcode = Convert.ToInt32(employee._ECode);
            var model = _vehicleService.GetVehicleDataForSecurity(empcode);

            if (model == null || model.VehicleMaster.Count == 0)
                TempData["ErrorMessage"] = "No location mapping found for you.";

            return View(model);
        }
        [HttpPost]
        public IActionResult VehicleSecurityAction([FromBody] VehicleSecurityActionDto model)
        {
            if (model == null || model.VehicleId <= 0)
                return BadRequest(new { success = false, message = "Invalid request." });
            var emp = _sessionService.Get<Employee_Details>("Employee");
            if (emp == null)
            {
                return BadRequest("Invalid user session");
            }
            var userId = _sessionService.Get<string>("userID");

            var result = _vehicleService.VehicleSecurityAction(model, userId);

            //if (!result.Success)
            //    return BadRequest(result);

            return Ok(result);
        }

        //Admin Peronal Mapping 
        public IActionResult AdminMappingListData()
        {
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");
            return View();
        }
        [HttpGet]
        public IActionResult AdminMappingList()
        {
            var data = _vehicleService.GetAdminMappingList();
            string sJSON = System.Text.Json.JsonSerializer.Serialize(data);
            return new JsonResult(sJSON);
        }

        public IActionResult AddAdminMapping()
        {
            var vm = new AdminLocationMapViewModel
            {
                SiteOptions = _vehicleService.GetSiteList(),
                AdminOptions = new List<EmployeeDetailDto>()
            };

            var result = vm.SiteOptions.Where(x => x.Value == 31).FirstOrDefault();

            return View(vm);
        }

        [HttpGet]
        public IActionResult GetAdminsBySite()
        {
            var data = _vehicleService.GetAdminsBySite();
            return Json(data);
        }

        [HttpPost]
        public IActionResult AddAdminMapping([FromBody] AdminLocationMapViewModel model)
        {
            var result = _vehicleService.AddMapping(model);

            if (result > 0)
                return Json(1);        // created

            if (result == 0)
                return Json(0);        // already exists

            return Json(new { error = "Unknown error" });
        }

        [HttpGet]
        public IActionResult EditAdminMapping(int id)
        {
            var model = _vehicleService.GetAdminMappingById(id);

            if (model == null)
                return Content("Invalid ID");

            // Load dropdowns
            model.SiteOptions = _vehicleService.GetSiteList();
            model.AdminOptions = _vehicleService.GetAdminsBySite();

            return View(model);
        }


        [HttpPost]
        public IActionResult UpdateAdminMapping([FromBody] AdminLocationMapUpdateDto models)
        {
            if (models == null)
                return Json(0);
            AdminLocationMapViewModel model = new AdminLocationMapViewModel
            {
                Id = (int)models.Id,
                SiteId = (int)models.SiteId,
                AdminCode = (int)models.EmpCode,
                IsActive = (int)models.IsActive
            };
            var result = _vehicleService.UpdateAdminMapping(model);

            return Json(result);
        }

        //Security Peronal Mapping 
        public IActionResult SecurityMappingList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SecurityMappingListData()
        {
            var data = _vehicleService.GetSecurityMappingList();
            return Json(System.Text.Json.JsonSerializer.Serialize(data));
        }

        public IActionResult AddSecurityMapping()
        {
            var vm = new SecurityLocationMapViewModelVP
            {
                SiteOptions = _vehicleService.GetSiteList(),
                EmployeeOptions = new List<EmployeeDetailDto>()
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddSecurityMapping([FromBody] SecurityLocationMapViewModelVP model)
        {
            var result = _vehicleService.AddSecurityMapping(model);
            return Json(result);
        }

        public IActionResult EditSecurityMapping(int id)
        {
            var vm = _vehicleService.GetSecurityMappingById(id);
            vm.SiteOptions = _vehicleService.GetSiteList();
            vm.EmployeeOptions = _vehicleService.GetSecurityPersonnelBySite();
            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateSecurityMapping([FromBody] SecurityLocationMapUpdateDto dto)
        {
            var vm = new SecurityLocationMapViewModelVP
            {
                Id = dto.Id,
                SiteId = dto.SiteId,
                EmpCode = dto.EmpCode,
                IsActive = dto.IsActive
            };

            return Json(_vehicleService.UpdateSecurityMapping(vm));
        }

        [HttpGet]
        public IActionResult GetSecurityPersonnelBySite()
        {
            return Json(_vehicleService.GetSecurityPersonnelBySite());
        }
        //Security Peronal Mapping 

        public IActionResult ExpireVehicles()
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");
            var employee = _sessionService.Get<Employee_Details>("Employee");
            var empcode = Convert.ToInt32(employee._ECode);
            var result = _vehicleService.GetExpiredVehicles(empcode);
            return View(result);
        }

        [HttpGet]
        public IActionResult GetImage([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return BadRequest("Invalid image path");

            path = path.Replace("/", Path.DirectorySeparatorChar.ToString());

            var basePath = _configurations.GetGeneralSettings().Get_FileUpload_Path;
            var fullPath = Path.Combine(basePath, path);

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var contentType = GetContentType(fullPath);

            return PhysicalFile(fullPath, contentType);
        }

		private static string GetContentType(string path)
		{
			var ext = Path.GetExtension(path).ToLowerInvariant();

			return ext switch
			{
				".jpg" or ".jpeg" => "image/jpeg",
				".png" => "image/png",
				".gif" => "image/gif",
				".webp" => "image/webp",
				".bmp" => "image/bmp",
				_ => "application/octet-stream"
			};
		}
	}
}