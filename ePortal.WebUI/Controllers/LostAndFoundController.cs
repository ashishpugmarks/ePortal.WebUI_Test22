using System;
using System.Collections.Generic;
using ePortal.Application.Contracts;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

using ePortal.Shared.Interface;
using System.Text.Json;

using iText.IO.Image;
using iText.Layout.Element;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Colors;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Spreadsheet;
using ePortal.DomainClasses;
using System.Text;
using iText.StyledXmlParser.Jsoup.Helper;



namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class LostAndFoundController : Controller
    {
        private readonly ILogger<LostAndFoundController> _logger;
        private readonly ILostAndFoundService _lostAndFoundService;
        private readonly ISessionService _sessionService;
        private readonly IConfiguration _configuration;
        private readonly IAppConfigurationService _env;

        private string sJSON = String.Empty;
        public LostAndFoundController(
            ILogger<LostAndFoundController> logger,
            ILostAndFoundService lostAndFoundService,
            ISessionService sessionService,
            IAppConfigurationService env,
            IConfiguration configuration)
        {
            _logger = logger;
            _lostAndFoundService = lostAndFoundService;
            _sessionService = sessionService;
            _configuration = configuration;
            _env = env;
        }


        public IActionResult Index()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // Get user's site/location from session
                var employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                long userSiteId = employeeDetails?._SiteId ?? 0;

                List<LostAndFoundViewModel> items;
                string userLocationName = "All Locations";

                var userId = _sessionService.Get<string>("userID");
                items = _lostAndFoundService.GetRequestByUserId(userId);
                userLocationName = _lostAndFoundService.GetUserLocationName(userSiteId);
                ViewBag.UserLocationName = userLocationName;
                ViewBag.showMyRequest = true;
                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lost and found items");
                return View(new List<LostAndFoundViewModel>());
            }
        }

        public IActionResult IndexPartial(bool showMyRequest = true)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                long userSiteId = employeeDetails?._SiteId ?? 0;
                List<LostAndFoundViewModel> items;
                string userLocationName = "All Locations";

                if (showMyRequest)
                {
                    var userId = _sessionService.Get<string>("userID");
                    items = _lostAndFoundService.GetRequestByUserId(userId);
                    userLocationName = _lostAndFoundService.GetUserLocationName(userSiteId);
                    ViewBag.UserLocationName = userLocationName;
                    return PartialView("_LostAndFoundListMyRequest", items);
                }
                else
                {
                    if (userSiteId > 0)
                    {
                        items = _lostAndFoundService.GetActiveByUserLocation(userSiteId);
                        userLocationName = _lostAndFoundService.GetUserLocationName(userSiteId);
                    }
                    else
                    {
                        items = _lostAndFoundService.GetAllActive();
                    }
                    ViewBag.UserLocationName = userLocationName;
                    return PartialView("_LostAndFoundList", items);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lost and found items");
                TempData["ErrorMessage"] = "An error occurred while retrieving the items.";

                // Return empty appropriate partial to avoid UI breaks
                if (showMyRequest)
                    return PartialView("_LostAndFoundListMyRequest", new List<LostAndFoundViewModel>());
                else
                    return PartialView("_LostAndFoundList", new List<LostAndFoundViewModel>());
            }
        }

        /// <summary>
        /// Security Personnel Dashboard - Display all items for review and management
        /// </summary>
        public IActionResult List()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            try
            {
               
                List<long> mappedSiteIds = _lostAndFoundService.GetSecurityMappedLocations(employeeDetails._ECode);

                if (mappedSiteIds == null || mappedSiteIds.Count == 0)
                {
                    TempData["ErrorMessage"] = "No location mapping found for you.";
                    
                }

                List<LostAndFoundViewModel> items = new List<LostAndFoundViewModel>();
                string userLocationName = "";

                foreach (var siteId in mappedSiteIds)
                {
               
                        items.AddRange(_lostAndFoundService.GetAllItemsForSecurity(siteId));
                        userLocationName += _lostAndFoundService.GetUserLocationName(siteId) + ", ";
                  
                }

                // Trim last comma
                userLocationName = userLocationName.Trim().TrimEnd(',');

                ViewBag.UserLocationName = userLocationName;
                ViewBag.IsSecurityPersonnel = true;

                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lost and found items for security personnel");
                TempData["ErrorMessage"] = "An error occurred while retrieving the items.";
                return View(new List<LostAndFoundViewModel>());
            }
        }


        public IActionResult ExportToExcel(LostAndFoundExportModel model)
        {
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                if (employeeDetails == null)
                    return Json(new { success = false, message = "Employee not found in session" });

                
                var mappedSiteIds = _lostAndFoundService.GetSecurityMappedLocations(employeeDetails._ECode);

                if (mappedSiteIds == null || mappedSiteIds.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No location mapping found for you."
                    });
                }

                List<LostAndFoundViewModel> result = new List<LostAndFoundViewModel>();
                string locationName = "";
                string str = "";


                foreach (var siteId in mappedSiteIds)
                {
                    
                    var siteItems = _lostAndFoundService.GetLostAndFoundForExprotExcel(model, siteId);
                    result.AddRange(siteItems);

                    
                    //locationName += _lostAndFoundService.GetUserLocationName(siteId) ;
                   
                }


                str = _lostAndFoundService.LostAndFoundExcelHtml(result);



                _sessionService.Set<string>("LOSTANDFOUNDEXCELFILE", str);

                if (string.IsNullOrEmpty(str))
                {
                    return Json(new
                    {
                        success = false,
                        message = "No records found for the selected criteria"
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "Export data ready",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error during export: {ex.Message}"
                });
            }
        }



        public IActionResult DownloadExcelFile()
        {
            var excelHtml = _sessionService.Get<string>("LOSTANDFOUNDEXCELFILE");
            if (string.IsNullOrEmpty(excelHtml))
            {
                return Content("Session expired or file not available.");
            }
            var bytes = System.Text.Encoding.UTF8.GetBytes(excelHtml);
            return File(bytes, "application/vnd.ms-excel", "LostAndFoundReport.xls");
        }


        public IActionResult AddItem()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.PreviousUrl = Request.Headers["Referer"].ToString();
            var model = new LostAndFoundViewModel
            {
                DateFound = DateTime.Today,
                LocationOptions = _lostAndFoundService.GetLocation(),

            };

            return View(model);
        }

        /// <summary>
        /// Add new lost/found item (Security only)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(LostAndFoundViewModel model)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // TODO: Add role-based authorization check for security users

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = _sessionService.Get<string>("userID");
                var userName = _sessionService.Get<string>("userName") ?? userId;


                var itemId = _lostAndFoundService.Create(model, userName, userId);

                if (itemId > 0)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "Failed to add item. Please try again." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new item");
                return Json(new { success = false, message = "An error occurred while adding the item." });
            }
        }


        public IActionResult EditItem(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var referer = Request.Headers["Referer"].ToString();
                var previousUrl = !string.IsNullOrEmpty(referer) ? referer : "/LostAndFound/Index";
                

                var item = _lostAndFoundService.GetById(id);
                if (item == null)
                {
                    return Redirect(previousUrl);
                }

                if (item.Status == ePortal.DomainClasses.LostAndFoundStatus.Approved ||
                    item.Status == ePortal.DomainClasses.LostAndFoundStatus.Closed)
                {
                    return Redirect(previousUrl);
                }
                ViewBag.PreviousUrl = Request.Headers["Referer"].ToString();
                item.LocationOptions = _lostAndFoundService.GetLocation();

                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lost and found item for editing");
                return Redirect("/LostAndFound/Index");
            }
        }


        /// <summary>
        /// Update lost and found item
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditItem(LostAndFoundViewModel model)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                model.LocationOptions = _lostAndFoundService.GetLocation();
                return View(model);
            }

            try
            {
                var userinfo = _lostAndFoundService.GetById(model.Id);
                if (userinfo.Status == ePortal.DomainClasses.LostAndFoundStatus.Approved || userinfo.Status == ePortal.DomainClasses.LostAndFoundStatus.Closed)
                {
                    model.LocationOptions = _lostAndFoundService.GetLocation();
                    return View(model);
                }

                var userId = _sessionService.Get<string>("userID");
                var userName = _sessionService.Get<string>("userName") ?? userId;

                var success = _lostAndFoundService.Update(model, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Item updated successfully.";
                    return Json(new { success = true, message = "Item updated successfully." });
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update item. Please try again.";
                    model.LocationOptions = _lostAndFoundService.GetLocation();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lost and found item");
                TempData["ErrorMessage"] = "An error occurred while updating the item.";
                model.LocationOptions = _lostAndFoundService.GetLocation();
                return View(model);
            }
        }


        /// <summary>
        /// View details of a specific lost and found item
        /// </summary>
        public IActionResult Details(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            LostAndFoundViewModel item = new LostAndFoundViewModel();
            try
            {
                ViewBag.PreviousUrl = Request.Headers["Referer"].ToString();
                item = _lostAndFoundService.GetDetailById(id);
                item.LostAndFoundHistory = _lostAndFoundService.GetHistoryByItemId(id);
                if (item == null)
                {

                    item = new LostAndFoundViewModel();
                    return View(item);

                }
                item.location = _lostAndFoundService.GetUserLocationName(item.LocationFound);
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lost and found item details");
                TempData["ErrorMessage"] = "An error occurred while retrieving the item details.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public IActionResult PreviewImagesAsPdf(string files)
        {
            if (string.IsNullOrEmpty(files))
            {
                TempData["ErrorMessage"] = "No files provided.";
                return RedirectToAction("Index");
            }

            List<string> fileList;
            try
            {
                fileList = JsonConvert.DeserializeObject<List<string>>(files);
            }
            catch
            {
                fileList = files.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(f => f.Trim('\"', '[', ']'))
                                .ToList();
            }

            // Generate PDF into a byte array (not a stream we keep alive)
            byte[] pdfBytes;
            using (var ms = new MemoryStream())
            {
                using (var writer = new PdfWriter(ms))
                using (var pdf = new PdfDocument(writer))
                using (var doc = new Document(pdf))
                {
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        var file = fileList[i];
                        var imagePath = Path.Combine(_env.GetGeneralSettings().Get_FileUpload_Path + "LostAndFound", file);
                        if (System.IO.File.Exists(imagePath))
                        {
                            var imageData = ImageDataFactory.Create(imagePath);
                            var img = new iText.Layout.Element.Image(imageData).SetAutoScale(true);
                            doc.Add(img);
                            if (i < fileList.Count - 1)
                            {
                                doc.Add(new AreaBreak());
                            }
                        }
                        else
                        {
                            doc.Add(new Paragraph($"Image not found: {file}").SetFontColor(ColorConstants.RED));

                            if (i < fileList.Count - 1)
                            {
                                doc.Add(new AreaBreak());
                            }
                        }
                    }
                }
                pdfBytes = ms.ToArray();
            }

            // Tell browser to render inline
            Response.Headers["Content-Disposition"] = "inline; filename=Preview.pdf";
            return File(pdfBytes, "application/pdf");
        }

        [HttpGet("{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var uploadPath = _env.GetGeneralSettings().Get_FileUpload_Path + "LostAndFound";
            var filePath = Path.Combine(uploadPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var mimeType = "image/" + Path.GetExtension(filePath).Trim('.').ToLower();
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, mimeType);
        }

        /// <summary>
        /// Edit lost and found item
        /// </summary>

        //Mapping 
        public IActionResult SpMapping()
        {
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");
            return View();
        }
        public IActionResult SpMappingList()
        {
            var items = _lostAndFoundService.GetAllMapping();
            string sJSON = System.Text.Json.JsonSerializer.Serialize(items);
            return new JsonResult(sJSON);
        }
        [HttpGet]
        public IActionResult AddSpMapping()
        {
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");
            var vm = new SecurityLocationMapViewModel
            {
                SiteOptions = _lostAndFoundService.GetSiteList(),
                EmployeeOptions = new System.Collections.Generic.List<SelectListItmesVm>()
            };
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult AddSpMapping([FromBody] SecurityLocationMapViewModel model)
        {
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");
            if (!ModelState.IsValid)
            {
                model.SiteOptions = _lostAndFoundService.GetSiteList();
                return View(model);
            }
            var user = _sessionService.Get<string>("userName") ?? _sessionService.Get<string>("userID");
            var result = _lostAndFoundService.CreateMapping(model, user);
            return new JsonResult(result);
        }

        public IActionResult EditMapping(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");

            var vm = _lostAndFoundService.GetByIdMapping(id);
            if (vm == null) return RedirectToAction("SpMapping");

            // ? Populate site options
            vm.SiteOptions = _lostAndFoundService.GetSiteList()
                .Select(x => new SelectListItmesVm
                {
                    Value = x.Value,
                    Text = x.Text
                })
                .ToList();

          
            vm.EmployeeOptions = _lostAndFoundService.GetAllEmployeeMapping()
                .Select(x => new SelectListItmesVm
                {
                    Value = x.Value,
                    Text = x.Text
                })
                .ToList();

            return View(vm);
        }


        [HttpPost]
        public IActionResult EditMapping(SecurityLocationMapViewModel model)
        {
            if (_sessionService.Get<string>("userID") == null)
                return RedirectToAction("Index", "Login");
            if (!ModelState.IsValid)
            {
                model.SiteOptions = _lostAndFoundService.GetSiteList();
                return View(model);
            }
            var user = _sessionService.Get<string>("userName") ?? _sessionService.Get<string>("userID");
            var result = _lostAndFoundService.UpdateMapping(model, user);

            return new JsonResult(result);
        }

        [HttpGet]
        public IActionResult SearchEmployeesMapping(string term)
        {
            var list = _lostAndFoundService.GetSecurityEmpList(term ?? string.Empty);
            return new JsonResult(list);
        }
        [HttpGet]
        public IActionResult GetEmployeesBySite()
        {
            var list = _lostAndFoundService.GetAllEmployeeMapping();
            return new JsonResult(list);
        }

        // Security Personnel Status Management Actions

        /// <summary>
        /// Approve an item (Security Personnel Only)
        /// </summary>
        [HttpPost]
        public IActionResult ApproveItem(long id, string securityRemarks)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            if (employeeDetails == null || !_lostAndFoundService.IsUserMappedAsSecurityPersonnel(employeeDetails._ECode))
            {
                return Json(new { success = false, message = "Access denied. Only security personnel can perform this action." });
            }

            try
            {
                var userName = _sessionService.Get<string>("userName") ?? _sessionService.Get<string>("userID");
                var empCode = employeeDetails._ECode.ToString();

                var success = _lostAndFoundService.ApproveItem(id, securityRemarks, userName, empCode);

                if (success)
                {
                    return Json(new { success = true, message = "Item approved successfully. It is now visible to all users." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to approve item. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving item with ID: {ItemId}", id);
                return Json(new { success = false, message = "An error occurred while approving the item." });
            }
        }

        /// <summary>
        /// Send back an item with remarks (Security Personnel Only)
        /// </summary>
        [HttpPost]
        public IActionResult SendBackItem(long id, string securityRemarks)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            if (employeeDetails == null || !_lostAndFoundService.IsUserMappedAsSecurityPersonnel(employeeDetails._ECode))
            {
                return Json(new { success = false, message = "Access denied. Only security personnel can perform this action." });
            }

            if (string.IsNullOrWhiteSpace(securityRemarks))
            {
                return Json(new { success = false, message = "Security remarks are required when sending back an item." });
            }

            try
            {
                var userName = _sessionService.Get<string>("userName") ?? _sessionService.Get<string>("userID");
                var empCode = employeeDetails._ECode.ToString();
                
                var success = _lostAndFoundService.SendBackItem(id, securityRemarks, userName, empCode);

                if (success)
                {
                    return Json(new { success = true, message = "Item sent back successfully. User has been notified via email." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to send back item. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending back item with ID: {ItemId}", id);
                return Json(new { success = false, message = "An error occurred while sending back the item." });
            }
        }



        [HttpPost]
        public IActionResult CloseItem(long id, string closeItemBy, string? closeByEmpCode, string verificationDetails, string? companyName, long? empType)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            if (employeeDetails == null || !_lostAndFoundService.IsUserMappedAsSecurityPersonnel(employeeDetails._ECode))
            {
                return Json(new { success = false, message = "Access denied. Only security personnel can perform this action." });
            }

            if (string.IsNullOrWhiteSpace(closeItemBy))
            {
                return Json(new { success = false, message = "Name is required." });
            }

            try
            {
                var userName = _sessionService.Get<string>("userName") ?? _sessionService.Get<string>("userID");
                var empCode = employeeDetails._ECode.ToString();

                var success = _lostAndFoundService.CloseItem(id, closeItemBy, closeByEmpCode ?? "", verificationDetails ?? "", userName, empCode, companyName, empType);

                if (success)
                {
                    return Json(new { success = true, message = "Item closed successfully. User has been notified via email." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to closed. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing with ID: {ItemId}", id);
                return Json(new { success = false, message = "An error occurred while closing." });
            }
        }


        public IActionResult PendingItems()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            if (employeeDetails == null || !_lostAndFoundService.IsUserMappedAsSecurityPersonnel(employeeDetails._ECode))
            {
                TempData["ErrorMessage"] = "Access denied. This page is only accessible to security personnel.";
                return RedirectToAction("Index");
            }

            try
            {
                long userSiteId = Convert.ToInt64(employeeDetails.Site_Id);
                var items = _lostAndFoundService.GetPendingItems(userSiteId);
                var userLocationName = _lostAndFoundService.GetUserLocationName(userSiteId);

                ViewBag.UserLocationName = userLocationName;
                ViewBag.IsSecurityPersonnel = true;
                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending items for security personnel");
                TempData["ErrorMessage"] = "An error occurred while retrieving pending items.";
                return View(new List<LostAndFoundViewModel>());
            }
        }




        [HttpPost]
        public ActionResult CancelRequest(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return Json(new { success = false, sessionExpired = true, message = "Your session has expired. Please log in again." });
            }
            try
            {
                var userinfo = _lostAndFoundService.GetById(id);
                if (userinfo.Status == ePortal.DomainClasses.LostAndFoundStatus.Approved || userinfo.Status == ePortal.DomainClasses.LostAndFoundStatus.Closed)
                {
                    return Json(new { success = false, message = "Action not allowed." });
                }

                bool requestCancel = _lostAndFoundService.CancelRequestById(id);
                return Json(new { success = requestCancel, message = requestCancel ? "Request cancelled successfully." : "Unable to cancel the request. Please try again." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred. Please try again later." });
            }

        }
        public IActionResult GetEmployeeByCode(string empCode)
        {
            if (!long.TryParse(empCode, out var empCodeLong))
            {
                return Json(new { success = false, Message = "Invalid employee code." });
            }

            var result = _lostAndFoundService.GetEmployeeDetailsByEmpCode(empCodeLong);

            if (result.FullName == null)
            {
                return Json(new { success = false, Message = "Employee not found." });
            }

            return Json(new
            {
                Name = result.FullName,
                Department = result.DepartmentName,
                Designation = result.Designation,
                success = true,
            });
        }

       

    }
}
