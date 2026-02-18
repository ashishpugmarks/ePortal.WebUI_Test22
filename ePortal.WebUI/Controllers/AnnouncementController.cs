using ClosedXML.Excel;
using ePortal.Application.Contracts;
using ePortal.Application.Services;
using ePortal.DomainClasses;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using ePortal.WebUI.Helpers;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using IOPath = System.IO.Path;
using ePortal.Shared.Services;
using ePortal.Persistence.TravelBilling.Services;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Persistence.TravelBilling.Interface;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class AnnouncementController : Controller
    {

        private readonly IAnnouncementService _AnnouncementService;
        private readonly IEmpLoginService _loginService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IAppConfigurationService _appConfig;
        //private readonly ICommonFunctions _commonFunctions;
        private readonly IcTravelBilling _cTravelBilling;

        public AnnouncementController(IAnnouncementService announcementService, IEmpLoginService loginService, ISessionService sessionService, IAppConfigurationService appConfig, ILogger<HomeController> logger, IcTravelBilling objTB, IWebHostEnvironment env)
        {
            _AnnouncementService = announcementService;
            _loginService = loginService;
            _sessionService = sessionService;
            _logger = logger;
            _appConfig = appConfig;
            _cTravelBilling = objTB;
            _env = env;
        }


        // Content Process Master

        #region Content Process Master
        [HttpGet]
        public IActionResult ContentProcessMaster()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // If the user is logged in, get the process master list
                var processList = _AnnouncementService.GetProcess_Mst_List();

                // Pass the list to the view
                return View(processList);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public IActionResult CreateContentProcess()
        {
            try
            {
                ProcessMstViewModel PVM = new ProcessMstViewModel();
                PVM.STATUS = 1; // Active=1, Deactive=0                
                return PartialView("_CreateContentProcess", PVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateContentProcess([FromForm] ProcessMstViewModel PVM, IFormFile? file)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                foreach (var property in typeof(ProcessMstViewModel).GetProperties())
                {
                    var value = property.GetValue(PVM);

                    // Remove from ModelState if the value is null or an empty string
                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        ModelState.Remove(property.Name);
                    }
                }

                if (ModelState.IsValid)
                {
                    FileViewModel bannerFile = await GetUploadFileAsync(file);
                    PVM.BANNER_NAME = bannerFile.FileName;
                    PVM.BANNER_CONTENTTYPE = bannerFile.FileContentType;
                    PVM.Upload_Banner = bannerFile.File;
                    PVM.CREATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    ProcessMstViewModel obj = _AnnouncementService.SaveProcess_Mst(PVM);
                }
                return RedirectToAction("ContentProcessMaster");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public IActionResult EditContentProcess(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //if (!string.IsNullOrEmpty(id.ToString()))
                if (id > 0)
                {
                    return PartialView("_EditContentProcess", _AnnouncementService.GetEditProcess_MstById(id));
                }
                return RedirectToAction("ContentProcessMaster");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }

        }

        [HttpPost]
        public async Task<IActionResult> EditContentProcess(ProcessMstViewModel PVM, IFormFile file)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (!ModelState.IsValid)
                {
                    FileViewModel bannerFile = await GetUploadFileAsync(file);
                    PVM.BANNER_NAME = bannerFile.FileName;
                    PVM.BANNER_CONTENTTYPE = bannerFile.FileContentType;
                    PVM.Upload_Banner = bannerFile.File;
                    PVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    ProcessMstViewModel obj = _AnnouncementService.UpdateProcess_Mst(PVM);
                }
                return RedirectToAction("ContentProcessMaster");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }
        #endregion
        ///// Announcement

        #region Announcement
        [HttpGet]
        public IActionResult AnnouncementMaster()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SearchViewModel searchViewModel = new SearchViewModel();
                AnnouncementMasterViewModel AMVM = new AnnouncementMasterViewModel();
                AMVM.Announcements = _AnnouncementService.GetAnnouncementMasterList(searchViewModel, Convert.ToInt64(_sessionService.Get<string>("userID")));
                return View(AMVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult AnnouncementMaster(AnnouncementMasterViewModel Announcement)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AnnouncementMasterViewModel AMVM = new AnnouncementMasterViewModel();
                AMVM.Announcements = _AnnouncementService.GetAnnouncementMasterList(Announcement.SearchViewModel, Convert.ToInt64(_sessionService.Get<string>("userID")));
                return View(AMVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }
        [HttpPost]
        public ActionResult AnnouncementExporttoExcel(AnnouncementMasterViewModel Announcement)
        {

            try
            {
                AnnouncementMasterViewModel AMVM = new AnnouncementMasterViewModel();
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                AMVM.Announcements = _AnnouncementService.GetAnnouncementMasterList(Announcement.SearchViewModel, Convert.ToInt64(_sessionService.Get<string>("userID")));


                System.Data.DataTable dt = new System.Data.DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[8] {
                                             new DataColumn("S. No."),
                                            new DataColumn("Process Name"),
                                            new DataColumn("Subject"),
                                            new DataColumn("Brief"),
                                            new DataColumn("start Date"),
                                            new DataColumn("End Date"),
                                            new DataColumn("Created Date"),
                                            new DataColumn("Status"),

            });
                int sno = 1;
                foreach (var item in AMVM.Announcements)
                {
                    dt.Rows.Add(sno, item.ProcessName, item.SUBJECT, item.BRIEF, item.START_DATE, item.END_DATE, item.CREATED_DATE, item.STATUS);
                    sno++;

                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Announcement_list.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
            // return View(AMVM);
        }


        [HttpGet]
        public ActionResult AnnouncementDetails(int id)
        {
            try
            {
                AnnouncementMasterViewModel AMVM = _AnnouncementService.GetAnnouncementDetails(id);
                if (AMVM.ATTACHMENTID > 0)
                {
                    return PartialView("_AnnouncementDetails", AMVM);
                }
                else
                {
                    return Json("error");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return Json("error");               
            }
        }

        [HttpGet]
        public ActionResult CreateAnnouncement()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                IEnumerable<ProcessMstViewModel> items = _AnnouncementService.BindContentProcess();
                IEnumerable<ADFUNCTIONALDESIGNATION> FnDesItems = _AnnouncementService.Bind_ADFunctionalDesignation();
                IEnumerable<ADDESIGNATION> DesItems = _AnnouncementService.Bind_ADDesignation();
                IEnumerable<SYSITE> SYSiteItems = _AnnouncementService.Bind_SYSite();
                ViewBag.ContentProcess = new SelectList(items, "PROCESSID", "PROCESS_NAME");
                ViewBag.FunctionalDesignation = new MultiSelectList(FnDesItems, "ADFUNCTIONALDESIGNATIONID", "DESCRIP");
                ViewBag.Designation = new MultiSelectList(DesItems, "ADDESIGNATIONID", "DESCRIP");
                ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
                return PartialView("_CreateAnnouncement");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return Json("error");
            }
        }

        [HttpPost]
        public ActionResult CreateAnnouncement(AnnouncementMasterViewModel AVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                foreach (var property in typeof(AnnouncementMasterViewModel).GetProperties())
                {
                    var value = property.GetValue(AVM);

                    // Remove from ModelState if the value is null or an empty string
                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        ModelState.Remove(property.Name);
                    }
                }

                if (ModelState.IsValid)
                {

                    //for unique circularid//
                    if (AVM.PROCESSID == 8)
                    {
                        DateTime dt = DateTime.Now;
                        AVM.UniqueCirculareID = dt.Year.ToString() + "/" + dt.Month.ToString("D2") + "-";
                    }

                    FileViewModel bannerFile = GetUploadFile(AVM.BannerFile);
                    AVM.BANNER_NAME = bannerFile.FileName;
                    AVM.BANNER_CONTENTTYPE = bannerFile.FileContentType;
                    AVM.BANNER = bannerFile.File;

                    FileViewModel attachment1File = GetUploadFile(AVM.AttachmentFile1);
                    AVM.ATTACHMENT1_NAME = attachment1File.FileName;
                    AVM.ATTACHMENT1_CONTENTTYPE = attachment1File.FileContentType;
                    AVM.ATTACHMENT1 = attachment1File.File;

                    FileViewModel attachment2File = GetUploadFile(AVM.AttachmentFile2);
                    AVM.ATTACHMENT2_NAME = attachment2File.FileName;
                    AVM.ATTACHMENT2_CONTENTTYPE = attachment2File.FileContentType;
                    AVM.ATTACHMENT2 = attachment2File.File;
                    AVM.CREATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    Int16 val = _AnnouncementService.SaveProcessAttachment_Trn(AVM);
                    if (val > 0)
                    {
                        TempData["resVal"] = Convert.ToString(val); //new { Value = val, Text = "Record saved successfully" };
                    }
                    else
                    {
                        TempData["resVal"] = Convert.ToString(val);  //new { Value = val, Text = "Record not saved" };
                    }
                }
                else
                {
                    TempData["resVal"] = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }
                return RedirectToAction("AnnouncementMaster");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public ActionResult EditAnnouncement(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                IEnumerable<ProcessMstViewModel> items = _AnnouncementService.BindContentProcess();
                IEnumerable<ADFUNCTIONALDESIGNATION> FnDesItems = _AnnouncementService.Bind_ADFunctionalDesignation();
                IEnumerable<ADDESIGNATION> DesItems = _AnnouncementService.Bind_ADDesignation();
                IEnumerable<SYSITE> SYSiteItems = _AnnouncementService.Bind_SYSite(); ViewBag.ContentProcess = new SelectList(items, "PROCESSID", "PROCESS_NAME");
                ViewBag.FunctionalDesignation = new MultiSelectList(FnDesItems, "ADFUNCTIONALDESIGNATIONID", "DESCRIP");
                ViewBag.Designation = new MultiSelectList(DesItems, "ADDESIGNATIONID", "DESCRIP");
                ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
                return PartialView("_EditAnnouncement", _AnnouncementService.GetEditProcessAttachmentById(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return Json("error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAnnouncement(AnnouncementMasterViewModel AVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                foreach (var property in typeof(AnnouncementMasterViewModel).GetProperties())
                {
                    var value = property.GetValue(AVM);

                    // Remove from ModelState if the value is null or an empty string
                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        ModelState.Remove(property.Name);
                    }
                }

                if (ModelState.IsValid)
                {
                    var files = HttpContext.Request.Form.Files;

                    foreach (var file in files)

                    {
                        if (file.Name == "file[0]")
                        {
                            var bannerFile = await GetUploadFileAsync(file);
                            AVM.BANNER_NAME = bannerFile.FileName;
                            AVM.BANNER_CONTENTTYPE = bannerFile.FileContentType;
                            AVM.BANNER = bannerFile.File;
                        }
                        else if (file.Name == "file[1]")
                        {
                            var attachment1File = await GetUploadFileAsync(file);
                            AVM.ATTACHMENT1_NAME = attachment1File.FileName;
                            AVM.ATTACHMENT1_CONTENTTYPE = attachment1File.FileContentType;
                            AVM.ATTACHMENT1 = attachment1File.File;
                        }
                        else if (file.Name == "file[2]")
                        {
                            var attachment2File = await GetUploadFileAsync(file);
                            AVM.ATTACHMENT2_NAME = attachment2File.FileName;
                            AVM.ATTACHMENT2_CONTENTTYPE = attachment2File.FileContentType;
                            AVM.ATTACHMENT2 = attachment2File.File;
                        }
                    }

                    AVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    Int16 val = _AnnouncementService.UpdateProcessAttachment_Trn(AVM);
                    if (val > 0)
                    {
                        TempData["resVal"] = Convert.ToString(val);
                    }
                    else
                    {
                        TempData["resVal"] = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    }
                }
                return RedirectToAction("AnnouncementMaster");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPut]
        public ActionResult DeactiveAnnouncement([FromBody] AnnouncementMasterViewModel AVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                //AnnouncementMasterViewModel obj = _AnnouncementService.DeactiveAnnouncement(AVM);
                //return View();
                retVal = _AnnouncementService.DeactiveAnnouncement(AVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");

            }
            return Json(retVal);
        }

        [HttpPut]
        public ActionResult CancelAnnouncement([FromBody] AnnouncementMasterViewModel AVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                //AnnouncementMasterViewModel obj = _AnnouncementService.CancelAnnouncement(AVM);
                retVal = _AnnouncementService.CancelAnnouncement(AVM);
                //return View();
            }
            catch (Exception ex)
            {
                retVal = -1;
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");

            }
            return Json(retVal);

        }

        [HttpPut]
        //public ActionResult DeleteFile(int id, string type)
        public ActionResult DeleteFile([FromBody] DeleteFileRequest fileRequest)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AnnouncementMasterViewModel AVM = new AnnouncementMasterViewModel();
                if (fileRequest.ATTACHMENTID > 0)
                {
                    AVM = _AnnouncementService.DeleteAnnouncementFile(fileRequest.ATTACHMENTID, fileRequest.ATTACHMENT_TYPE);
                }
                return Json(AVM);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        #endregion

        // Set Approval Authority

        #region Announcement Set Approval Authority
        public ActionResult AnnouncementApprovalAuthority()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return View(_AnnouncementService.GetAnnouncementApprovalAuthorityList());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }
        [HttpGet]
        public ActionResult EditAnnouncementApprovalAuthority(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                String[] parameterValue = _loginService.GetParameterValue("CONTENT_MGMT").Split(',');
                //IEnumerable<EmployeeViewModel> EmpList = _AnnouncementService.BindEmployeeBy_Designation(parameterValue);
                //ViewBag.Designation_Emp = new SelectList(EmpList, "EmpId", "Name");
                return PartialView("_EditAnnouncementApprovalAuthority", _AnnouncementService.GetAnnouncementApprovalAuthorityById(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }
        [HttpPost]
        public ActionResult EditAnnouncementApprovalAuthority([FromBody] AnnouncementApprovalViewModel AAVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AAVM.HRAPP_ECODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                AnnouncementApprovalViewModel obj = _AnnouncementService.SaveHRApproval(AAVM);
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.InnerException.Message);
            }
        }
        #endregion



        ////Announcement Approval

        #region Announcement Approval
        [HttpGet]
        public IActionResult AnnouncementApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return View(_AnnouncementService.GetAnnouncementApproval_List(Convert.ToInt64(_sessionService.Get<string>("userID"))));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public IActionResult UpdateAnnouncementStatus([FromBody] AnnouncementApprovalViewModel AAVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AAVM.APPAUTH1_ECODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                AAVM.APPAUTH2_ECODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                _AnnouncementService.UpdateStatus(AAVM);
                return Json(AAVM);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet]
        public IActionResult AnnouncementApprovalDetails(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return PartialView("_AnnouncementApprovalDetails", _AnnouncementService.GetAnnouncementApprovalAuthorityById(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        #endregion

        private async Task<FileViewModel> GetUploadFileAsync(IFormFile file)
        {
            FileViewModel FVM = new FileViewModel();
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    FVM.File = memoryStream.ToArray();
                }
                FVM.FileName = IOPath.GetFileName(file.FileName);
                FVM.FileContentType = file.ContentType; // Use file.ContentType
            }
            return FVM;
        }

        //private FileViewModel GetUploadFile(HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        FileViewModel FVM = new FileViewModel();
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            byte[] bytes;
        //            using (BinaryReader br = new BinaryReader(file.InputStream))
        //            {
        //                bytes = br.ReadBytes(file.ContentLength);
        //            }
        //            //string[] splitFile = file.FileName.Split('.');
        //            //FVM.FileName = splitFile[0];
        //            //FVM.FileContentType = splitFile[1];
        //            FVM.FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
        //            FVM.FileContentType = MimeMapping.GetMimeMapping(FVM.FileName);
        //            FVM.File = bytes;
        //        }
        //        return FVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private FileViewModel GetUploadFile(IFormFile file)
        {
            try
            {
                FileViewModel FVM = new FileViewModel();
                if (file != null && file.Length > 0)
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    FVM.FileName = System.IO.Path.GetFileName(file.FileName);
                    FVM.FileContentType = file.ContentType;
                    FVM.File = bytes;
                }
                return FVM;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the file.", ex);
            }
        }



        public async Task<IActionResult> Download(long id, string type)
        {
            try
            {
                FileViewModel file = _AnnouncementService.GetFileForDownload(id, type);
                if (file.File == null || string.IsNullOrEmpty(file.FileContentType))
                {
                    return NotFound(); // Use NotFound() for missing files
                }
                return File(file.File, file.FileContentType, file.FileName);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public JsonResult GetIsEmployeeActive(int id)
        {
            try
            {
                string res = _AnnouncementService.IsEmployeeActive(Convert.ToInt64(id));
                return Json(res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //added by gaurav//
        [HttpGet]
        public IActionResult AnnouncementArchive()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SearchViewModel searchViewModel = new SearchViewModel();
                AnnouncementMasterViewModel AMVM = new AnnouncementMasterViewModel();
                AMVM.Announcements = _AnnouncementService.GetAnnouncementArchiveList(searchViewModel, Convert.ToInt64(_sessionService.Get<string>("userID")));
                return View(AMVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public IActionResult AnnouncementArchiveDetails(int id)
        {
            try
            {
                AnnouncementMasterViewModel AMVM = _AnnouncementService.GetAnnouncementDetails(id);
                if (AMVM.ATTACHMENTID > 0)
                {
                    var base64ancbanner = AMVM.BANNER == null ? "" : Convert.ToBase64String(AMVM.BANNER);

                    string imgSrcancbanner = String.Format("data:" + AMVM.BANNER_CONTENTTYPE + ";base64,{0}", base64ancbanner);
                    string createDate = AMVM.CREATED_DATE.ToString("ddd MMM yyyy");
                    return Json(new { obj = AMVM, banner = imgSrcancbanner, date = createDate });
                }
                else
                {
                    return Json("error");
                }
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        #region Communication Proess
        [HttpGet]
        public IActionResult ManageCommunicationRequest()
        {
            try
            {
                return View("Communication/ManageCommunicationRequest", _AnnouncementService.CommunicationRequestList(Convert.ToInt64(_sessionService.Get<string>("userID"))));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        public IActionResult CommunicationMaster()
        {
            CommunicationViewModel obj = new CommunicationViewModel();
            IEnumerable<CommCategoryViewModel> categoryItems = _AnnouncementService.BindCommCategory();
            IEnumerable<CommMailTypeViewModel> MailTypeItems = _AnnouncementService.BindCommMailType();
            IEnumerable<ADFUNCTIONALDESIGNATION> FnDesItems = _AnnouncementService.Bind_ADFunctionalDesignation();
            IEnumerable<ADDESIGNATION> DesItems = _AnnouncementService.Bind_ADDesignation();
            IEnumerable<SYSITE> SYSiteItems = _AnnouncementService.Bind_SYSite();
            IList<Comm_Op_Div_DepViewModel> OpList = _AnnouncementService.BindOperation(1);
            IList<Comm_Op_Div_DepViewModel> DivList = _AnnouncementService.BindDivision(2, 0);
            IList<Comm_Op_Div_DepViewModel> DepList = _AnnouncementService.BindDepartment(3, 0);
            ViewBag.Categories = new SelectList(categoryItems, "COMMCATEGORYID", "CATEGORY_NAME");
            ViewBag.MailTypes = new SelectList(MailTypeItems, "COMMTYPEID", "COMM_TYPE");
            ViewBag.FunctionalDesignation = new MultiSelectList(FnDesItems, "ADFUNCTIONALDESIGNATIONID", "DESCRIP");
            ViewBag.Designation = new MultiSelectList(DesItems, "ADDESIGNATIONID", "DESCRIP");
            ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            ViewBag.OPList = new MultiSelectList(OpList, "Value", "Text");
            ViewBag.DivList = new MultiSelectList(DivList, "Value", "Text");
            ViewBag.DepList = new MultiSelectList(DepList, "Value", "Text");
            obj.EMAILSTATUS = true;
            return View("Communication/CommunicationMaster", obj);
        }

        [HttpPost]
        public IActionResult CommunicationMaster([FromForm] CommunicationViewModel CVM)
        {

            short retVal = 0; long _headerId = 0;
            CommCategoryViewModel commCateModel = new CommCategoryViewModel();
            List<CommunicationAppSeqViewModel> iList = new List<CommunicationAppSeqViewModel>();
            try
            {
                foreach (var property in typeof(CommunicationViewModel).GetProperties())
                {
                    var value = property.GetValue(CVM);

                    // Remove from ModelState if the value is null or an empty string
                    if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        ModelState.Remove(property.Name);
                    }
                }

                if (ModelState.IsValid)
                {
                    CVM.CREATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    CVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    CVM.PROCESS_STATUS = 0; // Pending at Initiator

                    string style25 = @"style=""width: 25%;""".Replace("\\", "");
                    string style50 = @"style=""width: 50%;""".Replace("\\", "");
                    string style75 = @"style=""width: 75%;""".Replace("\\", "");
                    string style100 = @"style=""width: 100%;""".Replace("\\", "");
                    string width25 = @"width=""25%""";
                    string width50 = @"width=""50%""";
                    string width75 = @"width=""75%""";
                    string width100 = @"width=""100%""";
                    CVM.EMAIL_CONTENT = CVM.EMAIL_CONTENT.Replace(style25, width25);
                    CVM.EMAIL_CONTENT = CVM.EMAIL_CONTENT.Replace(style50, width50);
                    CVM.EMAIL_CONTENT = CVM.EMAIL_CONTENT.Replace(style75, width75);
                    CVM.EMAIL_CONTENT = CVM.EMAIL_CONTENT.Replace(style100, width100);

                    DateTime dt = DateTime.Now;
                    CVM.REQUESTNO = dt.Year.ToString() + "/" + dt.Month.ToString("D2") + "-";

                    Tuple<short, long> retVal_tuple = _AnnouncementService.SaveCommunicationRequest(CVM);
                    retVal = retVal_tuple.Item1;
                    _headerId = retVal_tuple.Item2;

                    //Employee_Details _Employee_Details = (Employee_Details)HttpContext.Session.GetObject("Employee");
                    Employee_Details _Employee_Details = HttpContext.Session.GetObject<Employee_Details>("Employee");
                    iList = _AnnouncementService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID")), CVM.COMM_CATID, _Employee_Details);

                    commCateModel = _AnnouncementService.GetCommCategoryById(CVM.COMM_CATID);
                    commCateModel = commCateModel == null ? new CommCategoryViewModel() : commCateModel;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, headerId = _headerId, DefaultAuthList = iList, CategoryModel = commCateModel });
        }

        [HttpPut]
        public IActionResult CommunicationMasterFinalSave([FromBody] CommunicationViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.commDetail = new List<CommunicationDtlViewModel>();
                model.CREATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                Tuple<short, long> retVal_tuple = _AnnouncementService.FinalSubmitRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        //public ActionResult UploadAttachment(CommunicationDtlViewModel formData)
        public async Task<IActionResult> UploadAttachment(IFormFile FILE, string DOC_TYPE, string ADDITIONAL_INFO, long COMMUNICATIONID, string FILENAME)
        {
            short retVal = 0;
            List<CommunicationDtlViewModel> commDtlList = new List<CommunicationDtlViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (FILE != null && FILE.Length > 0 && !string.IsNullOrEmpty(DOC_TYPE) && COMMUNICATIONID > 0)
                {
                    FileViewModel _file = await GetUploadFileDtlAsync(FILE, DOC_TYPE, FILENAME, COMMUNICATIONID);
                    commDtlList.Add(new CommunicationDtlViewModel
                    {
                        COMMUNICATIONID = COMMUNICATIONID,
                        FILENAME = _file.FileName,
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = DOC_TYPE,
                        ADDITIONAL_INFO = ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID")),
                    });
                    Tuple<short, List<CommunicationDtlViewModel>> _ret_tuple = _AnnouncementService.SaveCommAttachment(Convert.ToInt64(_sessionService.Get<string>("userID")), COMMUNICATIONID, commDtlList);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        string basePath = serverpath.getFileUploadPath();
                        string path = System.IO.Path.Combine(basePath, "Communication");
                        //string path = Microsoft.AspNetCore.Server.MapPath("~/Uploads/Communication/");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (commDtlList.Count > 0)
                        {
                            foreach (CommunicationDtlViewModel obj in commDtlList)
                            {
                                if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                {
                                    await System.IO.File.WriteAllBytesAsync(IOPath.Combine(path, obj.FILENAME), obj.FILE_BYTE.ToArray());
                                }
                            }
                        }
                    }
                    commDtlList = _ret_tuple.Item2;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = commDtlList });
        }

        private async Task<FileViewModel> GetUploadFileDtlAsync(IFormFile file, string DocType, string FILENAME, long CommId)
        {
            FileViewModel FVM = new FileViewModel();
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    FVM.File = memoryStream.ToArray();
                }
                FileInfo fi = new FileInfo(file.FileName);
                FVM.FileContentType = file.ContentType; // Use file.ContentType
                FVM.FileName = FILENAME.Trim() + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + fi.Extension;
            }
            return FVM;
        }

        [HttpDelete]
        public IActionResult DeleteAttachment([FromBody] CommunicationDtlViewModel request)
        {
            short retVal = 0;
            List<CommunicationDtlViewModel> comDtlList = new List<CommunicationDtlViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string fileName = request.FILENAME;
                string docType = request.DOC_TYPE;
                long commHeaderId = request.COMM_DTLID;

                Tuple<short, List<CommunicationDtlViewModel>> _ret_tuple = _AnnouncementService.DeleteAttachment(fileName, docType, commHeaderId);
                retVal = _ret_tuple.Item1;
                comDtlList = _ret_tuple.Item2;
                if (retVal == 1)
                {
                    //string path = Server.MapPath("~/Uploads/Communication/");
                    string basePath = serverpath.getFileUploadPath();
                    string path = System.IO.Path.Combine(basePath, "Communication");
                    //string path = Server.MapPath("~/Uploads/Communication/");
                    if (System.IO.File.Exists(IOPath.Combine(path, fileName)))
                    {
                        System.IO.File.Delete(IOPath.Combine(path, fileName));
                    }
                }

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = comDtlList.Where(w => w.IsDeleted == 0).ToList() });
        }

        public IActionResult EditCommunicationMaster(string id)
        {
            long _ReqId;
            try
            {
                _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            }
            catch (Exception ex)
            {                
                _ReqId = Convert.ToInt64(Encryption.Decrypt(id));
            }
            CommunicationViewModel obj = new CommunicationViewModel();
            IEnumerable<CommCategoryViewModel> categoryItems = _AnnouncementService.BindCommCategory();
            IEnumerable<CommMailTypeViewModel> MailTypeItems = _AnnouncementService.BindCommMailType();
            IEnumerable<ADFUNCTIONALDESIGNATION> FnDesItems = _AnnouncementService.Bind_ADFunctionalDesignation();
            IEnumerable<ADDESIGNATION> DesItems = _AnnouncementService.Bind_ADDesignation();
            IEnumerable<SYSITE> SYSiteItems = _AnnouncementService.Bind_SYSite();
            IList<Comm_Op_Div_DepViewModel> OpList = _AnnouncementService.BindOperation(1);
            IList<Comm_Op_Div_DepViewModel> DivList = _AnnouncementService.BindDivision(2, 0);
            IList<Comm_Op_Div_DepViewModel> DepList = _AnnouncementService.BindDepartment(3, 0);
            ViewBag.Categories = new SelectList(categoryItems, "COMMCATEGORYID", "CATEGORY_NAME");
            ViewBag.MailTypes = new SelectList(MailTypeItems, "COMMTYPEID", "COMM_TYPE");
            ViewBag.FunctionalDesignation = new MultiSelectList(FnDesItems, "ADFUNCTIONALDESIGNATIONID", "DESCRIP");
            ViewBag.Designation = new MultiSelectList(DesItems, "ADDESIGNATIONID", "DESCRIP");
            ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            ViewBag.OPList = new MultiSelectList(OpList, "Value", "Text");
            ViewBag.DivList = new MultiSelectList(DivList, "Value", "Text");
            ViewBag.DepList = new MultiSelectList(DepList, "Value", "Text");
            //TempData["APPROVAL_AUTH_LIST"] = null;
            obj = _AnnouncementService.GetCommunicationDetails(_ReqId);
            return View("Communication/EditCommunicationMaster", obj);
        }

        public IActionResult GetPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/Communication/"; //// Server.MapPath("~/Uploads/PO/");
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"688px\"></object>";
                string _path = string.Format(embed, file_path + fileName);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }

        public IActionResult RenderPreview(long id)
        {
            return PartialView("Communication/_CommunicationPreview", _AnnouncementService.GetCommunicationDetails(id));
        }

        public IActionResult BrandCommunicationApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //Employee_Details _Employee_Details = (Employee_Details)HttpContext.Session.GetObject("Employee");
            Employee_Details _Employee_Details = HttpContext.Session.GetObject<Employee_Details>("Employee");
            return View("Communication/BrandCommunicationApproval", _AnnouncementService.CommunicationApprovalList(1));
        }

        [HttpGet]
        public IActionResult EditBrandCommunicationApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("BrandCommunicationApproval", "Announcement");
            }
            catch (Exception ex)
            {                
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public IActionResult EditBrandCommunicationApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            TempData.Remove("BC_ATTACHMENT");
            //Employee_Details _Employee_Details = (Employee_Details)HttpContext.Session.GetObject("Employee");
            Employee_Details _Employee_Details = HttpContext.Session.GetObject<Employee_Details>("Employee");
            CommunicationViewModel obj = _AnnouncementService.GetCommunicationDetails(id);
            return View("Communication/EditBrandCommunicationApproval", obj);
        }

        [HttpPut]
        //public async Task<IActionResult> EditBrandCommunicationApproval([FromBody] CommunicationAppHisViewModel ADVM, string CommCategory, string TEMP_PRV_FNAME)
        public async Task<IActionResult> EditBrandCommunicationApproval([FromBody] CommunicationApprovalRequest request)
        {
            short retVal = 0;

            var ADVM = request.ADVM;

            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string rootFilePath = _appConfig.GetGeneralSettings().Get_FileUpload_Path;
                string tempPath = IOPath.Combine(rootFilePath, "Communication", "Temp");
                if (System.IO.File.Exists(IOPath.Combine(tempPath, request.TEMP_PRV_FNAME)))
                {
                    System.IO.File.Delete(IOPath.Combine(tempPath, request.TEMP_PRV_FNAME));
                }
                //if (System.IO.File.Exists(System.IO.Path.Combine(Server.MapPath("~/Uploads/Communication/Temp/"), TEMP_PRV_FNAME)))
                //{
                //    System.IO.File.Delete(System.IO.Path.Combine(Server.MapPath("~/Uploads/Communication/Temp/"), TEMP_PRV_FNAME));
                //}
                //Employee_Details _Employee_Details = (Employee_Details)HttpContext.Session.GetObject("Employee");
                Employee_Details _Employee_Details = HttpContext.Session.GetObject<Employee_Details>("Employee");
                ADVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                ADVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                ADVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                List<CommunicationDtlViewModel> commDetailList = new List<CommunicationDtlViewModel>();

                if (TempData["BC_ATTACHMENT"] != null)
                {
                    //commDetailList.Add((CommunicationDtlViewModel)TempData["BC_ATTACHMENT"]);
                    commDetailList.Add(JsonConvert.DeserializeObject<CommunicationDtlViewModel>(TempData["BC_ATTACHMENT"].ToString()));

                }
                //string path = Server.MapPath("~/Uploads/Communication/");
                string path = IOPath.Combine(rootFilePath, "Communication");

                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                if (ADVM.APPROVAL_STATUS == 1)
                {
                    if (commDetailList.Count > 0)
                    {
                        foreach (CommunicationDtlViewModel obj in commDetailList)
                        {
                            if (obj.FILE_BYTE != null)
                            {
                                await System.IO.File.WriteAllBytesAsync(IOPath.Combine(path, obj.FILENAME), obj.FILE_BYTE.ToArray());
                            }
                        }
                    }
                    else
                    {
                        List<CommunicationDtlViewModel> CommAtachments = _AnnouncementService.GetCommunicationAttachments(ADVM.COMMUNICATIONID);
                        if (CommAtachments.Count > 0)
                        {
                            CommunicationDtlViewModel obj = CommAtachments.Where(w => w.DOC_TYPE == "Template").FirstOrDefault();
                            if (obj != null)
                            {
                                // Consider using a library like Aspose.Words or similar for Word to PDF conversion in .NET Core
                                // Microsoft.Office.Interop.Word is not available in .NET Core.
                                // For now, returning -2 to indicate conversion is not supported.
                                return Json(-2);
                            }
                        }
                    }
                }
                retVal = _AnnouncementService.CommunicationApproval(ADVM, _Employee_Details, commDetailList);
                if (retVal != 1)
                {
                    var atthDtl = commDetailList.FirstOrDefault();
                    if (atthDtl != null && System.IO.File.Exists(IOPath.Combine(path, atthDtl.FILENAME)))
                    {
                        System.IO.File.Delete(IOPath.Combine(path, atthDtl.FILENAME));
                    }
                }
            }
            catch (Exception ex)
            {
               retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public IActionResult UpdateFormDetail(CommunicationViewModel CVM)
        {
            short retVal = 0; long _headerId = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    CVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    Tuple<short, long> retVal_tuple = _AnnouncementService.UpdateCommunicationRequest(CVM);
                    retVal = retVal_tuple.Item1;
                    _headerId = retVal_tuple.Item2;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, headerId = _headerId });
        }

        [HttpPost]
        public async Task<IActionResult> UploadBC(IFormFile FILE, string DOC_TYPE, string ADDITIONAL_INFO, long COMMUNICATIONID, string FILENAME)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                TempData.Remove("BC_ATTACHMENT");
                CommunicationDtlViewModel BCAtt = new CommunicationDtlViewModel();
                if (FILE != null && FILE.Length > 0 && !string.IsNullOrEmpty(DOC_TYPE))
                {
                    FileViewModel _file = await GetUploadFileDtlAsync(FILE, DOC_TYPE, FILENAME, COMMUNICATIONID);
                    BCAtt.FILENAME = _file.FileName;
                    BCAtt.FILE_CONTENTTYPE = _file.FileContentType;
                    BCAtt.FILE_BYTE = _file.File;
                    BCAtt.DOC_TYPE = DOC_TYPE;
                    BCAtt.ADDITIONAL_INFO = ADDITIONAL_INFO;
                    TempData["BC_ATTACHMENT"] = JsonConvert.SerializeObject(BCAtt);
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public IActionResult CommunicationApprovalHistory()
        {
            try
            {
                return View("Communication/CommunicationApprovalHistory", _AnnouncementService.CommunicationApprovalHistory(Convert.ToInt64(_sessionService.Get<string>("userID"))));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        public IActionResult GetRequestHistory()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return PartialView("Communication/CommunicationApprovalHistory", _AnnouncementService.CommunicationApprovalHistory(Convert.ToInt64(_sessionService.Get<string>("userID"))));
        }

        [HttpGet]
        public IActionResult CommunicationApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId;
            try
            {
                _ReqId = Convert.ToInt64(Encryption.Decrypt(id));

            }
            catch (Exception ex)
            {
                _ReqId = Convert.ToInt64(Encryption.Decrypt(id));
            }
            //Employee_Details _Employee_Details = (Employee_Details)HttpContext.Session.GetObject("Employee");
            Employee_Details _Employee_Details = HttpContext.Session.GetObject<Employee_Details>("Employee");
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID"));

            CommunicationViewModel obj = _AnnouncementService.GetCommunicationDetails(_ReqId);
            obj.ISENABLE = obj.commAppHis.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0 && a.COMM_HISTORYID != 0).Count().ToString();
            return View("Communication/CommunicationApproval", obj);
        }

        [HttpGet]
        public IActionResult CommunicationRequestDetail(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId;
            try
            {
                //_ReqId = Convert.ToInt64(Encryption.Decrypt(id)); // Assuming Encryption helper is available
                string decodedTarget = Uri.UnescapeDataString(id).Replace(" ", "+");
                _ReqId = Convert.ToInt64(Encryption.Decrypt(decodedTarget));
            }
            catch (Exception ex)
            {
                _ReqId = Convert.ToInt64(Encryption.Decrypt(id));
            }
            CommunicationViewModel obj = _AnnouncementService.GetCommunicationDetails(_ReqId);
            return View("Communication/CommunicationRequestDetail", obj);
        }

        [HttpPost]
        public async Task<IActionResult> CommunicationApproval([FromBody] CommunicationAppHisViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //Employee_Details _Employee_Details = (Employee_Details)HttpContext.Session.GetObject("Employee");
                Employee_Details _Employee_Details = HttpContext.Session.GetObject<Employee_Details>("Employee");
                ADVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                ADVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                ADVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                List<CommunicationDtlViewModel> commDetailList = new List<CommunicationDtlViewModel>();
                retVal = _AnnouncementService.CommunicationApproval(ADVM, _Employee_Details, commDetailList);

                CommunicationViewModel IOM_Dtl = _AnnouncementService.GetCommunicationDetails(ADVM.COMMUNICATIONID);
                if (IOM_Dtl != null)
                {
                    if (IOM_Dtl.PROCESS_STATUS == 2 && IOM_Dtl.COMM_CATID == 1)
                    {
                        var OBJAPP = IOM_Dtl.commAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (OBJAPP == null)
                        {
                            var objiomfile = IOM_Dtl.commDetail.Where(m => m.DOC_TYPE == "VERIFIED_DOC");
                            if (objiomfile == null || !objiomfile.Any())
                            {
                                throw new Exception("Template File Not Found");
                            }
                            //string srcFile = Server.MapPath("~/Uploads/Communication/") + objiomfile.OrderByDescending(o => o.COMM_DTLID).FirstOrDefault().FILENAME;
                            //string STRFILENAME = IOM_Dtl.CATEGORY + "_" + DateTime.Now.Ticks + IOM_Dtl.COMMUNICATIONID + ".pdf";
                            //string strDstFile = Server.MapPath("~/Uploads/Communication/") + STRFILENAME;

                            string rootFilePath = _appConfig.GetGeneralSettings().Get_FileUpload_Path;

                            string srcFile = rootFilePath + "Communication\\" + objiomfile.OrderByDescending(o => o.COMM_DTLID).FirstOrDefault().FILENAME;
                            string STRFILENAME = IOM_Dtl.CATEGORY + "_" + DateTime.Now.Ticks + IOM_Dtl.COMMUNICATIONID + ".pdf";
                            string strDstFile = rootFilePath + "Communication\\" + STRFILENAME;

                            List<CommunicationAppSeqViewModel> iomAuthSeq = IOM_Dtl.commAuthSeq;
                            List<COMMSIGLIST> DocSiglist = new List<COMMSIGLIST>();
                            if (iomAuthSeq.Count > 0)
                            {
                                var DesgList = iomAuthSeq.Where(m => m.APPTYPE == 3).OrderBy(o => o.COMMAPPAUTH_ID).Select(s => s.ADDESIGNATION).Distinct();
                                foreach (var objd in DesgList)
                                {
                                    COMMSIGLIST objsig = new COMMSIGLIST();
                                    List<CommunicationAppHisViewModel> objapplist = new List<CommunicationAppHisViewModel>();
                                    objsig.Designation = objd.ToString();

                                    foreach (var authlist in iomAuthSeq.Where(m => m.ADDESIGNATION == objsig.Designation).ToList())
                                    {
                                        DateTime appdate = IOM_Dtl.commAppHis.Where(m => m.ADEMPCODE == authlist.ADEMPCODE).Max(m => m.APPROVALDATE).Value;
                                        objapplist.Add(new CommunicationAppHisViewModel { ADEMPNAME = authlist.ADEMPNAME, APPROVALDATE = appdate });
                                    }
                                    objsig.appList = objapplist;
                                    DocSiglist.Add(objsig);
                                }
                            }

                            CommunicationAnnotationPdf(srcFile, strDstFile, DocSiglist);
                            string straddinfo = "Approved Document";
                            List<CommunicationDtlViewModel> obj = new List<CommunicationDtlViewModel>() { new CommunicationDtlViewModel() { DOC_TYPE = "APPROVED_DOC", FILENAME = STRFILENAME, ADDITIONAL_INFO = straddinfo } };
                            _AnnouncementService.SaveCommAttachment(ADVM.ADDEDBY, ADVM.COMMUNICATIONID, obj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        //public ActionResult PreviewCommunication(long id, string IsChanges, string TEMP_PRV_FNAME)
        public ActionResult PreviewCommunication([FromBody] CommunicationPreviewModel model)
        {
            try
            {
                //if (System.IO.File.Exists(System.IO.Path.Combine(Server.MapPath("~/Uploads/Communication/Temp/"), TEMP_PRV_FNAME)))
                //{
                //    System.IO.File.Delete(System.IO.Path.Combine(Server.MapPath("~/Uploads/Communication/Temp/"), TEMP_PRV_FNAME));
                //}
                string rootFilePath = _appConfig.GetGeneralSettings().Get_FileUpload_Path;
                string fullPath = rootFilePath + "Communication\\" + "Temp\\" + model.TEMP_PRV_FNAME;

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                CommunicationViewModel OHDR = _AnnouncementService.GetCommunicationDetails(model.id);
                string fileName = string.Empty;
                //if (IsChanges != "1")
                //{
                //fileName = OHDR.commDetail.Where(m => m.DOC_TYPE == "Template").Select(s => s.FILENAME).FirstOrDefault();
                //    if (!string.IsNullOrEmpty(fileName))
                //    {
                //        Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                //        Microsoft.Office.Interop.Word.Document doc = word.Documents.Open(Server.MapPath("~/Uploads/Communication/") + fileName);
                //        doc.Activate();
                //        string newFileName = "Comm_Temp" + "_" + DateTime.Now.Ticks + id + ".pdf";
                //        doc.SaveAs2(Server.MapPath("~/Uploads/Communication/Temp/") + newFileName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                //        doc.Close();

                //        fileName = newFileName;
                //    }
                //}
                //else
                //{
                if (TempData["BC_ATTACHMENT"] != null)
                {
                    CommunicationDtlViewModel obj = JsonConvert.DeserializeObject<CommunicationDtlViewModel>(TempData["BC_ATTACHMENT"].ToString());
                    if (obj.FILE_BYTE != null)
                    {
                        string newFileName = "Comm_Temp" + "_" + DateTime.Now.Ticks + model.id + ".pdf";
                        //System.IO.File.WriteAllBytes(Server.MapPath("~/Uploads/Communication/Temp/") + newFileName, obj.FILE_BYTE.ToArray());
                        string folderPath = rootFilePath + "Communication\\" + "Temp\\";
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        System.IO.File.WriteAllBytes(folderPath + newFileName, obj.FILE_BYTE.ToArray());

                        fileName = newFileName;
                    }
                    TempData.Keep("BC_ATTACHMENT");
                }
                //}
                if (string.IsNullOrEmpty(fileName))
                {
                    return Json(new
                    {
                        FILEPATH = "",
                        TEMP_PREVIEW_FILE = "",
                    });
                }
                List<CommunicationAppSeqViewModel> iomAuthSeq = OHDR.commAuthSeq;
                List<COMMSIGLIST> DocSiglist = new List<COMMSIGLIST>();
                if (iomAuthSeq.Count > 0)
                {
                    var DesgList = iomAuthSeq.Where(m => m.APPTYPE == 3).OrderBy(o => o.COMMAPPAUTH_ID).Select(s => s.ADDESIGNATION).Distinct();
                    foreach (var obj in DesgList)
                    {
                        COMMSIGLIST objsig = new COMMSIGLIST();
                        List<CommunicationAppHisViewModel> objapplist = new List<CommunicationAppHisViewModel>();
                        objsig.Designation = obj.ToString();

                        foreach (var authlist in iomAuthSeq.Where(m => m.ADDESIGNATION == objsig.Designation).ToList())
                        {
                            objapplist.Add(new CommunicationAppHisViewModel { ADEMPNAME = authlist.ADEMPNAME, APPROVALDATE = DateTime.Now });
                        }
                        objsig.appList = objapplist;
                        DocSiglist.Add(objsig);
                    }
                }

                //Temp
                //string srcPath = Server.MapPath("~/Uploads/Communication/Temp/") + fileName;
                string srcPath = _appConfig.GetGeneralSettings().Get_FileUpload_Path + "Communication\\" + "Temp\\" + fileName;
                string destfilename = "Temp_Preview" + model.id + DateTime.Now.Ticks + ".pdf";
                //string destPath = Server.MapPath("~/Uploads/Communication/Temp/") + destfilename;
                string destPath = _appConfig.GetGeneralSettings().Get_FileUpload_Path + "Communication\\" + "Temp\\" + destfilename;

                CommunicationAnnotationPdfPreview(srcPath, destPath, DocSiglist);

                string file_path = "../../../Uploads/Communication/Temp/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"688px\"></object>";
                string _path = string.Format(embed, file_path + destfilename);
                return Json(new
                {
                    FILEPATH = _path,
                    TEMP_PREVIEW_FILE = destfilename,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }

        private void CommunicationAnnotationPdfPreview(string srcPath, string dstPath, List<COMMSIGLIST> objlist)
        {

            var writer = new PdfWriter(dstPath);
            var pdfResult = new PdfDocument(new PdfReader(srcPath), writer);
            var pdfDoc = new Document(pdfResult);

            float[] columnWidths = new float[objlist.Count];
            for (int i = 0; i <= objlist.Count - 1; i++)
            {
                columnWidths[i] = 5;
            }
            Table table = new Table(UnitValue.CreatePercentArray(columnWidths));

            PdfFont f = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            Cell[] headerFooter = new Cell[objlist.Count];
            for (int i = 0; i < objlist.Count; i++)
            {
                headerFooter[i] = new Cell().SetBackgroundColor(new DeviceGray(0.75f)).SetFont(f).SetFontSize(7).Add(new Paragraph(objlist[i].Designation)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                table.AddHeaderCell(headerFooter[i]);
            }

            foreach (var obj in objlist)
            {
                string content = "";
                foreach (var sig in obj.appList)
                {
                    string appdate = sig.APPROVALDATE != null ? sig.APPROVALDATE.Value.ToString("dd-MMM-yyyy") : "";
                    content += sig.ADEMPNAME + "\n" + "Test" + "\n";
                }
                table.AddCell(new Cell().SetTextAlignment(TextAlignment.CENTER).SetFontSize(6).Add(new Paragraph(content))).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }

            Paragraph paragraph = new Paragraph("Preview").SetFont(f).SetFontSize(40);
            PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.5f);

            for (int pageno = 1; pageno <= pdfResult.GetNumberOfPages(); pageno++)
            {

                PdfPage page = pdfResult.GetPage(pageno);
                table.SetFixedPosition(10, 40, (page.GetPageSize().GetWidth() - 30));
                Canvas canvas;
                canvas = new Canvas(page, page.GetPageSize());
                canvas.Add(table);
                Rectangle pageSize = page.GetPageSizeWithRotation();
                page.SetIgnorePageRotationForContent(true);
                float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
                float y = (pageSize.GetTop() + pageSize.GetBottom()) / 2;
                PageSize ps = new PageSize(page.GetPageSize().GetWidth(), page.GetPageSize().GetHeight());
                PdfCanvas over = new PdfCanvas(page);
                over.SaveState();
                over.SetExtGState(gs1);
                pdfDoc.ShowTextAligned(paragraph, x, y, pageno, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
                over.RestoreState();
            }
            pdfDoc.Close();
            pdfResult.Close();
        }

        private void CommunicationAnnotationPdf(string srcPath, string dstPath, List<COMMSIGLIST> objlist)
        {

            var writer = new PdfWriter(dstPath);
            var pdfResult = new PdfDocument(new PdfReader(srcPath), writer);
            var pdfDoc = new Document(pdfResult);

            float[] columnWidths = new float[objlist.Count];
            for (int i = 0; i <= objlist.Count - 1; i++)
            {
                columnWidths[i] = 5;
            }
            Table table = new Table(UnitValue.CreatePercentArray(columnWidths));

            PdfFont f = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            Cell[] headerFooter = new Cell[objlist.Count];
            for (int i = 0; i < objlist.Count; i++)
            {
                headerFooter[i] = new Cell().SetBackgroundColor(new DeviceGray(0.75f)).SetFont(f).SetFontSize(7).Add(new Paragraph(objlist[i].Designation)).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
                table.AddHeaderCell(headerFooter[i]);
            }

            foreach (var obj in objlist)
            {
                string content = "";
                foreach (var sig in obj.appList)
                {
                    string appdate = sig.APPROVALDATE != null ? sig.APPROVALDATE.Value.ToString("dd-MMM-yyyy") : "";
                    content += sig.ADEMPNAME + "\n" + appdate + "\n";
                }
                table.AddCell(new Cell().SetTextAlignment(TextAlignment.CENTER).SetFontSize(6).Add(new Paragraph(content))).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }

            for (int pageno = 1; pageno <= pdfResult.GetNumberOfPages(); pageno++)
            {
                PdfPage page = pdfResult.GetPage(pageno);
                Canvas canvas;
                canvas = new Canvas(page, page.GetPageSize());
                table.SetFixedPosition(10, 40, (page.GetPageSize().GetWidth() - 30));
                canvas.Add(table);
                //pdfDoc.Add(table);
            }
            pdfDoc.Close();
            pdfResult.Close();
        }

        [HttpGet]
        public ActionResult RetrieveCommunication()
        {
            return Json(FetchCommunicationRequest().ToList());
        }
        public List<CommunicationRquestApproval> FetchCommunicationRequest()
        {

            List<CommunicationRquestApproval> CommApplist = new List<CommunicationRquestApproval>();
            var dtPO = _AnnouncementService.CommunicationApprovalList(Convert.ToInt64(_sessionService.Get<string>("userID")));
            if (dtPO.Count > 0)
            {
                CommunicationRquestApproval CRA = null;
                foreach (var dr in dtPO)
                {
                    CRA = new CommunicationRquestApproval();
                    string strDisable = string.Empty;
                    CRA.APPTransactionId = dr.COMMUNICATIONID.ToString();
                    CRA.TransactionId = WebUtility.UrlEncode(Encryption.Encrypt(dr.COMMUNICATIONID.ToString())).ToString();
                    CRA.EmpCode = dr.CREATED_BY.ToString();
                    CRA.EmpName = dr.CREATED_BY_NAME.ToString();
                    CRA.Subject = dr.SUBJECT.ToString();
                    CRA.Category = dr.CATEGORY.ToString();
                    CRA.AppliedDate = dr.CREATED_DATE.ToString();
                    CRA.Status = dr.STATUS.ToString();
                    CRA.ProcessStatus = dr.PROCESS_STATUS.ToString();
                    CRA.RequestType = dr.REQUEST_TYPE == 2 ? "Emergency" : "Normal";
                    CommApplist.Add(CRA);
                }
            }
            return CommApplist;
        }

        [HttpPost]
        public IActionResult GetPopCommDetail(int id)
        {
            CommunicationViewModel newsList = new CommunicationViewModel();
            newsList = _AnnouncementService.GetCommunicationDetails(id);
            return Json(newsList);
        }

        [HttpGet]
        public IActionResult ArchiveCommunication()
        {
            try
            {
                ViewBag.Categories = new SelectList(_AnnouncementService.BindCommCategory(), "COMMCATEGORYID", "CATEGORY_NAME");
                return View("Communication/ArchiveCommunication", new CommunicationViewModel());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public IActionResult ArchiveCommunication([FromBody] CommunicationViewModel model)
        {
            List<CommunicationViewModel> iList = _AnnouncementService.ArchiveRequestList(1, model);
            return PartialView("Communication/_ArchiveList", iList);
        }

        [HttpGet]
        public IActionResult ArchiveCommunicationDetails(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(id);
            CommunicationViewModel obj = _AnnouncementService.GetCommunicationDetails(_ReqId);
            if (obj.STATUS != 1 && obj.PROCESS_STATUS != 2)
            {
                return RedirectToAction("ArchiveCommunication");
            }
            else
            {
                List<CommunicationViewModel> ilist = _AnnouncementService.ValidateArchiveRequestList(_ReqId);
                if (ilist.Count == 0)
                {
                    return RedirectToAction("ArchiveCommunication");
                }
            }
            return View("Communication/ArchiveCommunicationDetails", obj);
        }

        [HttpGet]
        public IActionResult ArchiveAnnouncementNotice()
        {
            try
            {
                return View("Communication/ArchiveAnnouncementNotice", _AnnouncementService.ArchiveRequestList(0, new CommunicationViewModel()));
            }
            catch (Exception ex)
            {               
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }
        [HttpGet]
        public ActionResult ArchiveAnnouncementNoticeDetails(int id)
        {
            //try
            //{
            //    CommunicationViewModel AMVM = _AnnouncementService.GetCommunicationDetails(id);
            //    if (AMVM != null)
            //    {
            //        string createDate = AMVM.CREATED_DATE.ToString("ddd MMM yyyy");
            //        string bannerName = AMVM.commDetail.Where(w => w.DOC_TYPE == "Banner").Select(s=>s.FILENAME).FirstOrDefault();
            //        return Json(new { obj = AMVM, date = createDate, banner = bannerName }, JsonRequestBehavior.AllowGet);
            //    }
            //    else
            //    {
            //        return Json("error", JsonRequestBehavior.AllowGet);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return Json("error", JsonRequestBehavior.AllowGet);
            //}
            return PartialView("Communication/_ReadMoreDetails", _AnnouncementService.GetCommunicationDetails(id));
        }

        [HttpPost]
        //public IActionResult CancelCommunicationRequest([FromBody] long id)
        public IActionResult CancelCommunicationRequest([FromBody] CommunicationDtlViewModel request)
        {
            long commHeaderId = request.COMM_DTLID;
            short retval = 0;
            try
            {
                retval = _AnnouncementService.CancelCommunicationRequest(commHeaderId, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retval = -1;
            }
            return Json(retval);
        }

        [HttpPost]
        public IActionResult DeactivateCommunicationRequest([FromBody] CommRequest model)
        {
            short retval = 0;
            try
            {
                retval = _AnnouncementService.DeactivateCommunicationRequest(model.id, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retval = -1;
            }
            return Json(retval);
        }

        [HttpGet]
        public ActionResult RetrieveSelfPendingCommunication()
        {
            return Json(FetchSelfPendingCommunicationRequest().ToList());
        }
        public List<CommunicationRquestApproval> FetchSelfPendingCommunicationRequest()
        {

            List<CommunicationRquestApproval> CommApplist = new List<CommunicationRquestApproval>();
            var dtPO = _AnnouncementService.SelfPendingCommunicationList(Convert.ToInt64(_sessionService.Get<string>("userID")));
            if (dtPO.Count > 0)
            {
                CommunicationRquestApproval CRA = null;
                foreach (var dr in dtPO)
                {
                    CRA = new CommunicationRquestApproval();
                    string strDisable = string.Empty;
                    CRA.APPTransactionId = dr.COMMUNICATIONID.ToString();
                    CRA.TransactionId = WebUtility.UrlEncode(Encryption.Encrypt(dr.COMMUNICATIONID.ToString())).ToString();
                    //CRA.EmpCode = dr.CREATED_BY.ToString();
                    //CRA.EmpName = dr.CREATED_BY_NAME.ToString();
                    CRA.Subject = dr.SUBJECT.ToString();
                    CRA.Category = dr.CATEGORY.ToString();
                    CRA.AppliedDate = dr.CREATED_DATE.ToString("dd-MMM-yyyy");
                    //CRA.Status = dr.STATUS.ToString();
                    //CRA.ProcessStatus = dr.PROCESS_STATUS.ToString();
                    CommApplist.Add(CRA);
                }
            }
            return CommApplist;
        }
        public IActionResult ViewPolicyCircular(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId;
            try
            {
                _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            }
            catch (Exception ex)
            {
                _ReqId = Convert.ToInt64(Encryption.Decrypt(id));
            }
            CommunicationViewModel obj = _AnnouncementService.GetCommunicationDetails(_ReqId);
            if (obj.STATUS != 1 && obj.PROCESS_STATUS != 2)
            {
                obj = new CommunicationViewModel();
            }
            return View("Communication/ViewPolicyCircular", obj);
        }

        [HttpPost]
        public IActionResult InserViewData([FromBody] CommunicationViewModel model)
        {
            short retval = 0;
            long COMMUNICATIONID = model.COMMUNICATIONID;
            try
            {
                retval = _AnnouncementService.InserViewData(COMMUNICATIONID, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
               retval = -1;
            }
            return Json(retval);
        }

        [HttpPost]
        public IActionResult BindDivisionByOperationId([FromBody] CommunicationViewModel model)
        {

            List<Comm_Op_Div_DepViewModel> FinalDivList = new List<Comm_Op_Div_DepViewModel>();
            //if (model.OPERATION != null)
            if (model != null)
            {
                foreach (long id in model.OPERATION)
                {
                    List<Comm_Op_Div_DepViewModel> divList = _AnnouncementService.BindDivision(2, Convert.ToInt64(id));
                    if (divList != null)
                    {
                        FinalDivList.AddRange(divList);
                    }
                }
            }
            return Json(FinalDivList);
        }

        [HttpPost]
        public IActionResult BindDeptmentByDivisionId([FromBody] CommunicationViewModel model)
        {
            List<Comm_Op_Div_DepViewModel> FinalDivList = new List<Comm_Op_Div_DepViewModel>();
            //if (model.DIVISION != null)
            if (model != null && model.DIVISION != null)
            {
                foreach (long id in model.DIVISION)
                {
                    List<Comm_Op_Div_DepViewModel> divList = _AnnouncementService.BindDepartment(3, Convert.ToInt64(id));
                    if (divList != null)
                    {
                        FinalDivList.AddRange(divList);
                    }
                }
            }
            return Json(FinalDivList);
        }

        [HttpGet]
        public IActionResult ManageCommunicationType()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View("Communication/ManageCommunicationType", _AnnouncementService.GetCommunicationTypeList());
        }

        //[HttpGet]
        public ActionResult CreateCommunicationType()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return PartialView("Communication/_CommunicationType");
        }

        //[HttpGet]
        public ActionResult EditCommunicationType(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return PartialView("Communication/_CommunicationType", _AnnouncementService.GetCommunicationTypeDtlById(id));
        }

        [HttpPost]
        public ActionResult SaveCommunicationType([FromBody] CommMailTypeViewModel AOVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AOVM.CREATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                AOVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                retVal = _AnnouncementService.SaveCommunicationType(AOVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
                throw (ex);
            }
            return Json(retVal);
        }

        [HttpPost]
        public JsonResult UploadContentImage(IFormFile aUploadedFile)
        {
            var vReturnImagePath = string.Empty; string imgSrc = string.Empty;
            //if (aUploadedFile.ContentLength > 0)
            if (aUploadedFile != null && aUploadedFile.Length > 0)
            {
                //var vFileName = System.IO.Path.GetFileNameWithoutExtension(aUploadedFile.FileName);
                //var vExtension = System.IO.Path.GetExtension(aUploadedFile.FileName);

                //string sImageName = vFileName + DateTime.Now.Ticks;

                //var vImageSavePath = Server.MapPath("~/Uploads/Communication/") + sImageName + vExtension;
                //vReturnImagePath = sImageName + vExtension;
                ////ViewBag.Msg = vImageSavePath;
                //var path = vImageSavePath;

                //// Saving Image in Original Mode  
                //aUploadedFile.SaveAs(path);
                //var vImageLength = new FileInfo(path).Length;
                ////here to add Image Path to You Database ,  
                ////TempData["message"] = string.Format("Image was Added Successfully");

                MemoryStream target = new MemoryStream();
                aUploadedFile.CopyTo(target);
                //byte[] data = target.ToArray();
                byte[] bytes = target.ToArray(); //System.IO.File.ReadAllBytes(path);
                string base64string = Convert.ToBase64String(bytes);
                imgSrc = String.Format("data:" + aUploadedFile.ContentType + ";base64,{0}", base64string);

            }
            return Json(imgSrc);
        }
        #endregion

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //Added by aumento for SR80255
        // Communication Report     
        // GET: CommunicationReport

        public ActionResult CommunicationReport()
        {
            var currentDate = DateTime.Now;
            var startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            var kiList = _AnnouncementService.GetKI();
            ViewBag.KIList = new SelectList(kiList, "SYKIID", "KICODE");

            return View("Communication/CommunicationReport");
        }
        [HttpPost]
        public ActionResult CommunicationReport(string startDate, string endDate, string KI)
        {
            try
            {
                var kiList = _AnnouncementService.GetKI();
                var data = _AnnouncementService._GetCommunicationReport(startDate, endDate, KI);
                ViewBag.dtlist = data;
                ViewBag.KIList = new SelectList(kiList, "SYKIID", "KICODE", KI);
                ViewBag.preStartDate = startDate;
                ViewBag.preEndDate = endDate;
                ViewBag.preki = KI;
                return View("Communication/CommunicationReport");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching data.";
                return View("Error");
            }
        }
        public ActionResult ExportToExcel(string KI, string sDate, string eDate)
        {
            try
            {
                string fromDateFormatted = sDate;
                string toDateFormatted = eDate;

                System.Data.DataTable dt1 = _AnnouncementService._GetCommunicationReport(fromDateFormatted, toDateFormatted, KI);

                if (dt1.Rows.Count > 0)
                {
                    return ExportToExcelFile(dt1, " Report " + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                }
                else
                {
                    return Content("No records found.");
                }
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        private FileResult ExportToExcelFile(System.Data.DataTable dt, string fileName)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                // Ensure the DataTable has a valid name
                if (string.IsNullOrWhiteSpace(dt.TableName))
                {
                    dt.TableName = "ReportSheet"; // Assign a default worksheet name
                }

                var ws = wb.Worksheets.Add(dt); // Now dt has a valid name
                ws.Columns().AdjustToContents(); // Auto adjust column width

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                                "application/vnd.ms-excel",
                                fileName);
                }
            }
        }
        //Added by aumento for SR80255

        //Travel Billing Process Start
        [HttpGet]
        public IActionResult RetrieveChequeRequest()
        {
            long UserId = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

            //cTravelBilling oTravelBilling = new cTravelBilling();

            DataTable odt = _cTravelBilling.ChequeRequestApprovalList(UserId);

            List<ChequeRequestApproval> ChequeReqlist = new List<ChequeRequestApproval>();

            if (odt.Rows.Count > 0)
            {
                ChequeRequestApproval obj = null;

                foreach (DataRow dr in odt.Rows)
                {
                    obj = new ChequeRequestApproval();

                    obj.CHEQUEREQNO = dr["CHEQUEREQNO"].ToString();
                    obj.REQUEST_DATE = dr["REQUEST_DATE"].ToString();
                    obj.ADDEDBY = dr["ADDEDBY"].ToString();
                    obj.EMPFULLNAME = dr["EMPFULLNAME"].ToString();
                    ChequeReqlist.Add(obj);
                }
            }

            return Json(ChequeReqlist.ToList());
        }
        //Travel Billing Process Start End
    }
}

//Travel Billing Process Start
public class ChequeRequestApproval
{
    public string CHEQUEREQNO { get; set; }

    public string REQUEST_DATE { get; set; }

    public string ADDEDBY { get; set; }

    public string EMPFULLNAME { get; set; }
}
//Travel Billing Process Start End