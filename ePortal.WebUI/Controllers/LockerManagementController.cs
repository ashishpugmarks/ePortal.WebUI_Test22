using System.IO;
using ClosedXML.Excel;
using ePortal.Application.Contracts;
using ePortal.Application.Services;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.Locker;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.WebUI.Controllers;

[SessionTimeout]
[CSPFilter]
public class LockerManagementController : Controller
{
    private readonly ILogger<LockerManagementController> _logger;
    private readonly ISessionService _sessionService;
    private readonly IConfiguration _configuration;
    private readonly IAppConfigurationService _env;
    private readonly ILockerManagementService _lockerManagementService;
    public LockerManagementController(ILogger<LockerManagementController> logger,
            ISessionService sessionService,
            IAppConfigurationService env,
            IConfiguration configuration, ILockerManagementService lockerManagementService)
    {
        _logger = logger;
        _sessionService = sessionService;
        _configuration = configuration;
        _env = env;
        _lockerManagementService = lockerManagementService;
    }
    public IActionResult LockerDashboard()
    {
        var model = BuildBaseModel();
        model.ShowResults = false;
        return View(model);
    }

    /// <summary>
    /// ADMIN LOCKER REQUEST  - LIST VIEW
    /// </summary>
    /// <returns></returns>
    public IActionResult LockerRequestList()
    {
        if (_sessionService.Get<string>("userID") == null)
        {
            return RedirectToAction("Index", "Login");
        }

        try
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index", "Login");

            var employee = _sessionService.Get<Employee_Details>("Employee");
            var empcode = Convert.ToInt32(employee._ECode);
            var model = _lockerManagementService.GetLockerMangmentDataForAdmin(empcode);

            if (model == null || model.AdminRequest.Count == 0)
                TempData["ErrorMessage"] = "No location mapping found for you.";
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Locker items");
            return View(new LockerRequestAdminDto());
        }
    }

   



    /// <summary>
    /// ADMIN LOCKER REQUEST DASHBOARD 
    /// </summary>

    [HttpPost]
    public IActionResult LockerDashboard(LockerManagementViewModel request)
    {
        var model = BuildBaseModel();

        model.SelectedLocation = request.SelectedLocation;
        model.SelectedFloor = request.SelectedFloor;
        model.SelectedMainLocker = request.SelectedMainLocker;
        model.ShowResults = true;

        if (string.IsNullOrEmpty(request.SelectedLocation) ||
            string.IsNullOrEmpty(request.SelectedFloor) ||
            string.IsNullOrEmpty(request.SelectedMainLocker))
        {
            model.SubLockers = new List<SubLockerViewModel>();
            return View(model);
        }

        model.SubLockers = GenerateLockersByCombination(
            request.SelectedLocation,
            request.SelectedFloor,
            request.SelectedMainLocker
        );

        return View(model);
    }

    private LockerManagementViewModel BuildBaseModel()
    {
        return new LockerManagementViewModel
        {
            Location = new List<string>
            {
                "HMSI Tapukhera",
                "HMSI Manesar",
                "HMSI Narsapura"
            },
            Floor = new List<string>
            {
                "Sales",
                "HR",
                "Production",
                "IT"
            },
            MainLocker = new List<string>
            {
                "Main Locker 1",
                "Main Locker 2",
                "Main Locker 3",
            },
            SubLockers = new List<SubLockerViewModel>()
        };
    }

    private List<SubLockerViewModel> GenerateLockersByCombination(
        string location,
        string department,
        string mainLocker)
    {
        var list = new List<SubLockerViewModel>();

        // Deterministic numeric mapping
        int locationFactor = location switch
        {
            "HMSI Tapukhera" => 1,
            "HMSI Manesar" => 2,
            "HMSI Narsapura" => 3,
            _ => 1
        };

        int deptFactor = department switch
        {
            "Sales" => 2,
            "HR" => 3,
            "Production" => 4,
            "IT" => 5,
            _ => 2
        };

        int lockerFactor = int.Parse(mainLocker.Split(" ").Last());

        int finalSeed = locationFactor * deptFactor * lockerFactor;

        for (int i = 1; i <= 100; i++)
        {
            bool occupied = (i * finalSeed) % 5 == 0;

            list.Add(new SubLockerViewModel
            {
                LockerNumber = $"{locationFactor}/GF/{lockerFactor}/{i}",
                IsOccupied = occupied,
                EmployeeName = occupied ? $"{department} Emp {i}" : null,
                EmployeeCode = occupied ? (5000 + i + finalSeed).ToString() : null
            });
        }

        return list;
    }



    public IActionResult Export(string location, string department, string mainLocker)
    {
        var data = GenerateLockersByCombination(location, department, mainLocker);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Locker Report");

            // Header Row
            worksheet.Cell(1, 1).Value = "Sr. No";
            worksheet.Cell(1, 2).Value = "Location";
            worksheet.Cell(1, 3).Value = "Floor";
            worksheet.Cell(1, 4).Value = "Locker";
            worksheet.Cell(1, 5).Value = "Employee Name";
            worksheet.Cell(1, 6).Value = "Employee Code";
            worksheet.Cell(1, 7).Value = "Locker Box Number";
            worksheet.Cell(1, 8).Value = "Status";

            // Make header bold
            worksheet.Range(1, 1, 1, 8).Style.Font.Bold = true;

            int row = 2;
            int srNo = 1;

            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = srNo;
                worksheet.Cell(row, 2).Value = location;
                worksheet.Cell(row, 3).Value = department; // Assuming department = Floor
                worksheet.Cell(row, 4).Value = mainLocker;
                worksheet.Cell(row, 5).Value = item.EmployeeName;
                worksheet.Cell(row, 6).Value = item.EmployeeCode;
                worksheet.Cell(row, 7).Value = item.LockerNumber;
                worksheet.Cell(row, 8).Value = item.IsOccupied ? "Occupied" : "Vacant";

                row++;
                srNo++;
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "LockerReport.xlsx");
            }
        }
    }



    



    //Admin Peronal Mapping 
    public IActionResult AdminLocationMapping()
    {
        if (_sessionService.Get<string>("userID") == null)
            return RedirectToAction("Index", "Login");
        return View();
    }
    [HttpGet]
    public IActionResult AdminMappingList()
    {
        var data = _lockerManagementService.GetAdminMappingList();
        string sJSON = System.Text.Json.JsonSerializer.Serialize(data);
        return new JsonResult(sJSON);
    }

    public IActionResult AddAdminMapping()
    {
        var vm = new LockerAdminLocationMapViewModel
        {
            SiteOptions = _lockerManagementService.GetSiteList(),
            AdminOptions = new List<EmployeeDetailDto>()
        };

       

        return View(vm);
    }

    [HttpGet]
    public IActionResult GetAdminsBySite()
    {
        var data = _lockerManagementService.GetAdminsBySite();
        return Json(data);
    }

    [HttpPost]
    public IActionResult AddAdminMapping([FromBody] LockerAdminLocationMapViewModel model)
    {
        var result = _lockerManagementService.AddMapping(model);

        if (result > 0)
            return Json(1);        // created

        if (result == 0)
            return Json(0);        // already exists

        return Json(new { error = "Unknown error" });
    }

    [HttpGet]
    public IActionResult EditAdminMapping(int id)
    {
        var model = _lockerManagementService.GetAdminMappingById(id);

        if (model == null)
            return Content("Invalid ID");

        // Load dropdowns
        model.SiteOptions = _lockerManagementService.GetSiteList();
        model.AdminOptions = _lockerManagementService.GetAdminsBySite();

        return View(model);
    }


    [HttpPost]
    public IActionResult UpdateAdminMapping([FromBody] LockerAdminLocationMapUpdateDto models)
    {
        if (models == null)
            return Json(0);
        LockerAdminLocationMapViewModel model = new LockerAdminLocationMapViewModel
        {
            Id = (int)models.Id,
            SiteId = (int)models.SiteId,
            AdminCode = (int)models.EmpCode,
            IsActive = (int)models.IsActive
        };
        var result = _lockerManagementService.UpdateAdminMapping(model);

        return Json(result);
    }

    [HttpGet]
    public IActionResult GetFloorList(int siteId)
    {
        var userId = _sessionService.Get<string>("userID");
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Json(new { success = false, message = "Session expired" });
        }

        var employee = _sessionService.Get<Employee_Details>("Employee");
        if (employee == null)
        {
            return Json(new { success = false, message = "Session expired" });
        }

        try
        {
            var floors = _lockerManagementService
                            .GetFloorList(Convert.ToInt32(siteId))
                            ?? new FloorResponse();

            return Json(floors);
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "Something went wrong." });
        }
    }

    [HttpGet]
    public IActionResult GetLockerList(int floorId)
    {
        try
        {
            var lockers = _lockerManagementService
                            .GetLockerListByFloor(floorId)
                            ?? new LockerResponse();

            return Json(lockers);
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "Something went wrong." });
        }
    }

    [HttpGet]
    public IActionResult GetLockerBoxList(int lockerId)
    {
        try
        {
            var boxes = _lockerManagementService
                            .GetLockerBoxesByLockerId(lockerId)
                            ?? new LockerBoxResponse();

            return Json(boxes);
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "Something went wrong." });
        }
    }

    [HttpPost]
    public IActionResult AssignLocker(int requestId, int floorId, int lockerId, int boxId)
    {
        var userId = _sessionService.Get<string>("userID");
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Json(new { success = false, message = "Session expired" });
        }

        var employee = _sessionService.Get<Employee_Details>("Employee");
        if (employee == null)
        {
            return Json(new { success = false, message = "Session expired" });
        }
        try
        {
            var result = _lockerManagementService.AssignLocker(requestId, floorId, lockerId, boxId, Convert.ToInt32(employee.Employee_Code));

            if (result)
                return Json(new { success = true });

            return Json(new { success = false, message = "Unable to assign locker." });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "Something went wrong." });
        }
    }

}
