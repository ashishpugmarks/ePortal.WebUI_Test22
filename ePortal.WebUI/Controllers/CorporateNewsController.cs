using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class CorporateNewsController : Controller
    {
        private readonly ICorporateNews _objCorporateNewsService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IAppConfigurationService _configuration;

        public CorporateNewsController(ICorporateNews objCorporateNewsService, ISessionService sessionService, IWebHostEnvironment env, IAppConfigurationService appConfiguration)
        {
            _objCorporateNewsService = objCorporateNewsService;
            _sessionService = sessionService;
            _env = env;
            _configuration = appConfiguration;
        }


        public IActionResult Index()
        {
            return View();
        }

        #region CorporateNews

        public ActionResult CorporateNewsMaster()
        {
            try
            {
                SearchViewModel searchViewModel = new SearchViewModel();
                CorporateNewsMasterViewModel CNMVM = new CorporateNewsMasterViewModel();
                CNMVM.CorporateNews = _objCorporateNewsService.GetCorporateNewsMasterList(searchViewModel);
                return View(CNMVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                ModelState.AddModelError("Error", ex.ToString());
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult CorporateNewsMaster(CorporateNewsMasterViewModel CNMVM)
        {
            try
            {

                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                CNMVM.CorporateNews = _objCorporateNewsService.GetCorporateNewsMasterList(CNMVM.SearchViewModel);
                return View(CNMVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                ModelState.AddModelError("Error", ex.ToString());
                return RedirectToAction("ErrorPage");
            }
            return View(CNMVM);
        }

        [HttpGet]
        public ActionResult CorporateNewsDetails(int id)
        {
            CorporateNewsMasterViewModel PAM = new CorporateNewsMasterViewModel();
            try
            {
                PAM = _objCorporateNewsService.GetCorporateNewsDetails(id);
                return PartialView("_CorporateNewsDetails", _objCorporateNewsService.GetCorporateNewsDetails(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                ModelState.AddModelError("Error", ex.ToString());
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public ActionResult CreateCorporateNews()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                IEnumerable<ProcessMstViewModel> items = _objCorporateNewsService.BindContentProcess();
                ViewBag.ContentProcess = new SelectList(items, "PROCESSID", "PROCESS_NAME", "DISPLAY_FILE");
                return PartialView("_CreateCorporateNews");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                ModelState.AddModelError("Error", ex.ToString());
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult CreateCorporateNews(CorporateNewsMasterViewModel AVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (ModelState.IsValid)
                {
                    FileViewModel bannerFile = GetUploadFile(AVM.BannerFile);
                    AVM.BANNER_NAME = bannerFile.FileName;
                    AVM.BANNER_CONTENTTYPE = bannerFile.FileContentType;
                    AVM.BANNER = bannerFile.File;

                    FileViewModel attachment1File = GetUploadFile(AVM.Attachment1File);
                    AVM.ATTACHMENT1_NAME = attachment1File.FileName;
                    AVM.ATTACHMENT1_CONTENTTYPE = attachment1File.FileContentType;
                    AVM.ATTACHMENT1 = attachment1File.File;

                    FileViewModel attachment2File = GetUploadFile(AVM.Attachment2File);
                    AVM.ATTACHMENT2_NAME = attachment2File.FileName;
                    AVM.ATTACHMENT2_CONTENTTYPE = attachment2File.FileContentType;
                    AVM.ATTACHMENT2 = attachment2File.File;
                    AVM.CREATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    int obj = _objCorporateNewsService.SaveProcessAttachment_Trn(AVM);
                    if (obj > 0)
                    {
                        TempData["AlertMessage"] = obj;
                    }
                    else
                    {
                        TempData["AlertMessage"] = obj;
                    }
                }
                else
                {
                    var modelErrors = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var modelError in modelState.Errors)
                        {
                            modelErrors.Add(modelError.ErrorMessage);
                        }
                    }
                    TempData["AlertMessage"] = modelErrors;
                }
                return RedirectToAction("CorporateNewsMaster");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["AlertMessage"] = ex.Message;
                return RedirectToAction("ErrorPage");
            }

        }

        //[SkipCSP]
        [HttpGet]
        public ActionResult EditCorporateNews(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                IEnumerable<ProcessMstViewModel> items = _objCorporateNewsService.BindContentProcess();
                ViewBag.ContentProcess = new SelectList(items, "PROCESSID", "PROCESS_NAME");
                return PartialView("_EditCorporateNews", _objCorporateNewsService.GetCorporateNewsDetails(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                ModelState.AddModelError("Error", ex.ToString());
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult EditCorporateNews(CorporateNewsMasterViewModel AVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                //ModelState.Remove("START_DATE");
                //ModelState.Remove("END_DATE");
                //ModelState.Remove("BannerFile");
                //ModelState.Remove("Attachment1File");
                //ModelState.Remove("Attachment2File");

                if (ModelState.IsValid)
                {
                    //HttpFileCollectionBase file = Request.Files;
                    //FileViewModel bannerFile = GetUploadFile(file[0]);
                    FileViewModel bannerFile = GetUploadFile(AVM.BannerFile);
                    AVM.BANNER_NAME = bannerFile.FileName;
                    AVM.BANNER_CONTENTTYPE = bannerFile.FileContentType;
                    AVM.BANNER = bannerFile.File;

                    FileViewModel attachment1File = GetUploadFile(AVM.Attachment1File);
                    AVM.ATTACHMENT1_NAME = attachment1File.FileName;
                    AVM.ATTACHMENT1_CONTENTTYPE = attachment1File.FileContentType;
                    AVM.ATTACHMENT1 = attachment1File.File;

                    FileViewModel attachment2File = GetUploadFile(AVM.Attachment2File);
                    AVM.ATTACHMENT2_NAME = attachment2File.FileName;
                    AVM.ATTACHMENT2_CONTENTTYPE = attachment2File.FileContentType;
                    AVM.ATTACHMENT2 = attachment2File.File;

                    AVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));

                    Int16 obj = _objCorporateNewsService.UpdateProcessAttachment_Trn(AVM);

                    if (obj > 0)
                    {
                        return Json("success");
                    }
                    else
                    {
                        return Json("failed");
                    }

                }
                else
                {
                    var modelErrors = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var modelError in modelState.Errors)
                        {
                            modelErrors.Add(modelError.ErrorMessage);
                        }
                    }
                    return Json(modelErrors);

                    //var errors = ModelState
                    //    .Where(x => x.Value.Errors.Count > 0)
                    //    .Select(x => new {
                    //        Field = x.Key,
                    //        Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    //    }).ToList();

                    //return Json(errors); // or log it
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        #endregion

        #region CorporateNews Approval 

        [HttpGet]
        public ActionResult CorporateNewsApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return View(_objCorporateNewsService.GetCorporateNewsApproval_List(Convert.ToInt64(_sessionService.Get<string>("userID"))));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                ModelState.AddModelError("Error", ex.ToString());
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult UpdateCorporateNewsStatus([FromBody] CorporateNewsApprovalViewModel AAVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AAVM.APPAUTH1_ECODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                //AAVM.APPAUTH2_ECODE = Convert.ToInt64(Session["UserId"]);
                AAVM = _objCorporateNewsService.UpdateStatus(AAVM);
                return Json(AAVM);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult CorporateNewsApprovalDetails(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return PartialView("_CorporateNewsApprovalDetails", _objCorporateNewsService.GetCorporateNewsApprovalAuthorityById(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());

                ModelState.AddModelError("Error", ex.ToString());
                return RedirectToAction("ErrorPage");
            }
        }
        #endregion

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
            var fvm = new FileViewModel();
            try
            {


                if (file != null && file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fvm.File = ms.ToArray();
                    }

                    fvm.FileName = Path.GetFileName(file.FileName);
                    fvm.FileContentType = file.ContentType; // Automatically populated
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                //throw new Exception("Error while processing the file.", ex);
            }
            return fvm;
        }


        [HttpPost]
        public ActionResult DeactiveCorporateNews(long ATTACHMENTID)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                CorporateNewsApprovalViewModel AAVM = new CorporateNewsApprovalViewModel();
                if (ATTACHMENTID != 0)
                {

                    AAVM.APPAUTH1_ECODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    AAVM.STATUS = 0;
                    AAVM.ATTACHMENTID = ATTACHMENTID;
                    //AAVM.APPAUTH2_ECODE = Convert.ToInt64(Session["UserId"]);
                    AAVM = _objCorporateNewsService.UpdateStatus(AAVM);
                }
                return Json(AAVM);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public FileResult Download(Int64 id, string type)
        {
            FileViewModel file = _objCorporateNewsService.GetFileForDownload(id, type);
            return File(file.File, file.FileContentType, file.FileName);
        }

        //[HttpPut]
        //public ActionResult DeleteDocument(Int64 ATTACHMENTID, string type)
        //{
        //    try
        //    {
        //        if(ATTACHMENTID==0 || string.IsNullOrEmpty(type))
        //        {
        //            return Json("error");
        //        }


        //        CorporateNewsMasterViewModel CNPV = new CorporateNewsMasterViewModel();
        //        CNPV = _objCorporateNewsService.DeleteDocument(ATTACHMENTID, type);
        //        return Json(CNPV, "Json");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message);
        //    }
        //}

        [HttpPost]
        public IActionResult DeleteDocument([FromForm] Int64 ATTACHMENTID, [FromForm] string type)
        {
            try
            {
                if (ATTACHMENTID == 0 || string.IsNullOrEmpty(type))
                {
                    return Json("error");
                }


                CorporateNewsMasterViewModel CNPV = new CorporateNewsMasterViewModel();
                CNPV = _objCorporateNewsService.DeleteDocument(ATTACHMENTID, type);
                //return Json(CNPV, "Json");
                return Json(CNPV);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        #region CreateNewsLetter 
        //Added By Bhupesh - NTT for CR-4894

        [HttpGet]
        public ActionResult CreateNews()
        {
            var newsLetters = _objCorporateNewsService.GetAllNewsLetters();
            var model = new NewsLetterViewModel
            {
                NewsLetters = newsLetters
            };
            return View(model);
        }

        //[HttpPost]
        //public ActionResult CreateNews(NewsLetterViewModel model, HttpPostedFileBase document)
        //{
        //    if (_sessionService.Get<string>("userID") == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        string fileName = null;
        //        string path = null;
        //        if (document != null && document.ContentLength > 0)
        //        {
        //            const int maxFileSize = 15 * 1024 * 1024;
        //            if (document.ContentLength > maxFileSize)
        //            {
        //                return View(model);
        //            }
        //            fileName = System.IO.Path.GetFileName(document.FileName);
        //            path = System.IO.Path.Combine(Server.MapPath("~/Uploads/Documents/"), fileName);
        //            document.SaveAs(path);
        //        }
        //        if (model.Id == 0)
        //        {
        //            NEWSLETTERS newsletter = new NEWSLETTERS();
        //            newsletter.DESCRIPTION = model.Description;
        //            newsletter.DOCUMENT_NAME = fileName;
        //            newsletter.STATUS = model.Status;
        //            newsletter.ADDED_BY = _sessionService.Get<string>("userID");
        //            newsletter.ADDED_DATE = DateTime.Now;
        //            _objCorporateNewsService.AddNewsLetter(newsletter);
        //        }
        //        else
        //        {
        //            NEWSLETTERS newsletter = new NEWSLETTERS();
        //            newsletter.ID = model.Id;
        //            newsletter.DESCRIPTION = model.Description;
        //            newsletter.DOCUMENT_NAME = fileName;
        //            newsletter.STATUS = model.Status;
        //            newsletter.UPDATED_BY = _sessionService.Get<string>("userID");
        //            newsletter.UPDATED_DATE = DateTime.Now;
        //            _objCorporateNewsService.UpdateNewsLetter(newsletter);
        //        }
        //        return RedirectToAction("CreateNews");
        //    }
        //    var newsLetters = _objCorporateNewsService.GetAllNewsLetters();
        //    var dataTable = new NewsLetterViewModel
        //    {
        //        NewsLetters = newsLetters
        //    };
        //    return View(dataTable);
        //}



        [HttpPost]
        public async Task<ActionResult> CreateNews(NewsLetterViewModel model, IFormFile document)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                string fileName = null;
                string path = null;
                if (document != null && document.Length > 0)
                {
                    const long maxFileSize = 15 * 1024 * 1024;

                    if (document.Length > maxFileSize)
                    {
                        ModelState.AddModelError("document", "File size must be less than 15 MB.");
                        return View(model);
                    }

                    fileName = Path.GetFileName(document.FileName);
                    //path = Path.Combine(Server.MapPath("~/Uploads/Documents/"), fileName);
                    //document.SaveAs(path);

                    // Save to wwwroot/Uploads/Documents
                    // var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "Documents");
                    string uploadsFolder = _configuration.GetGeneralSettings().Get_FileUpload_Path + "\\Documents";
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                    path = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }
                }
                if (model.Id == 0 || model.Id == null)
                {
                    NEWSLETTERS newsletter = new NEWSLETTERS();
                    newsletter.DESCRIPTION = model.Description;
                    newsletter.DOCUMENT_NAME = fileName;
                    newsletter.STATUS = model.Status;
                    newsletter.ADDED_BY = _sessionService.Get<string>("userID");
                    newsletter.ADDED_DATE = DateTime.Now;
                    _objCorporateNewsService.AddNewsLetter(newsletter);
                }
                else
                {
                    NEWSLETTERS newsletter = new NEWSLETTERS();
                    newsletter.ID = Convert.ToInt32(model.Id);
                    newsletter.DESCRIPTION = model.Description;
                    newsletter.DOCUMENT_NAME = fileName;
                    newsletter.STATUS = model.Status;
                    newsletter.UPDATED_BY = _sessionService.Get<string>("userID");
                    newsletter.UPDATED_DATE = DateTime.Now;
                    _objCorporateNewsService.UpdateNewsLetter(newsletter);
                }
                return RedirectToAction("CreateNews");
            }
            var newsLetters = _objCorporateNewsService.GetAllNewsLetters();
            var dataTable = new NewsLetterViewModel
            {
                NewsLetters = newsLetters
            };
            return View(dataTable);
        }


        [HttpPost]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                var newsLetter = _objCorporateNewsService.GetNewsLetterById(id);
                if (newsLetter != null)
                {
                    newsLetter.STATUS = (newsLetter.STATUS == "Active") ? "Inactive" : "Active";
                    _objCorporateNewsService.UpdateNewsLetter(newsLetter);
                    return Json(new { status = newsLetter.STATUS });
                }
                return Json(new { status = "Error" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public ActionResult EditNewsletter(int id)
        {
            var newsLetters = _objCorporateNewsService.GetNewsLetterById(id);
            var newsLetter = _objCorporateNewsService.GetAllNewsLetters();
            var model = new NewsLetterViewModel
            {
                Id = (int)newsLetters.ID,
                Description = newsLetters.DESCRIPTION,
                Status = newsLetters.STATUS,
                NewsLetters = newsLetter
            };
            return View("CreateNews", model);
        }
        #endregion
    }
}

