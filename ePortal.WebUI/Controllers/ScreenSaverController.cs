using ePortal.Application.Contracts;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class ScreenSaverController : Controller
    {
        private readonly IScreenSaverService _objScreenSaverService;

        private readonly ILogger<ScreenSaverController> _logger;
        private readonly ISessionService _sessionService;
        private readonly string _userId;
        private readonly string _userName;
        public ScreenSaverController(IScreenSaverService ScreenSaverService, IEmpLoginService oblogin, ISessionService sessionService, ILogger<ScreenSaverController> logger)
        {
            _objScreenSaverService = ScreenSaverService;
            _sessionService = sessionService;
            _logger = logger;
            _userId = _sessionService.Get<string>("userID").ToString();
            _userName = _sessionService.Get<string>("userName").ToString();
        }
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public ActionResult ScreenSaverMaster()
        {
            try
            {
                ScreenViewModel searchViewModel = new ScreenViewModel();
                ScreenSaverViewModel CNMVM = new ScreenSaverViewModel();
                CNMVM.ScreenSaver = _objScreenSaverService.GetScreenSaverMasterList(searchViewModel);
                return View(CNMVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult ScreenSaverMaster(ScreenSaverViewModel CNMVM)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                CNMVM.ScreenSaver = _objScreenSaverService.GetScreenSaverMasterList(CNMVM.SearchViewModel);
                return View(CNMVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public ActionResult CreateScreenSaverNews()
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
               
                return PartialView("_CreateScreenSaver");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult CreateScreenSaverNews(ScreenSaverViewModel AVM)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (ModelState.IsValid)
                {
                    FileViewModel bannerFile = GetUploadFile(AVM.FILE_NAME);
                    AVM.PoliciesFileName = bannerFile.FileName;
                    AVM.PoliciesContentType = bannerFile.FileContentType;
                    AVM.Policies = bannerFile.File;
                    Int16 obj = _objScreenSaverService.SaveProcessScreenSaver_Trn(AVM);
                  
                    if (obj > 0)
                    {
                        //string path = Server.MapPath("~/Uploads/ScreenSaver/");
                        string path = Path.Combine(serverpath.getFileUploadPath(), "ScreenSaver");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        System.IO.File.WriteAllBytes(Path.Combine(path , AVM.FILE_NAME.FileName), AVM.Policies.ToArray());
                        TempData["AlertMessage"] = JsonConvert.SerializeObject(obj);
                    }
                    else
                    {
                        TempData["AlertMessage"] = JsonConvert.SerializeObject(obj);
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
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(modelErrors);
                }
                return RedirectToAction("ScreenSaverMaster");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                TempData["AlertMessage"] = JsonConvert.SerializeObject(ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }
        private FileViewModel GetUploadFile(IFormFile file)
        {
            try
            {
                FileViewModel FVM = new FileViewModel();
                if (file != null && file.Length > 0)
                {
                    byte[] bytes;
                    //using (BinaryReader br = new BinaryReader(file.InputStream))
                    //{
                    //    bytes = br.ReadBytes((int)file.Length);
                    //}

                    using (var ms = new MemoryStream())
                    {
                        file.OpenReadStream().CopyTo(ms);
                        bytes = ms.ToArray();
                    }

                    //string[] splitFile = file.FileName.Split('.');
                    FVM.FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
                    //FVM.FileContentType = MimeMapping.GetMimeMapping(FVM.FileName);
                    FVM.FileContentType = file.ContentType;

                    FVM.File = bytes;
                }
                return FVM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult ScreenSaverDetails(int id)
        {
            try
            {
                return PartialView("_ScreenSaverDetails", _objScreenSaverService.GetScreenSaverDetails(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }

        }

        public FileResult Download(Int64 id, string type)
        {
            FileViewModel file = _objScreenSaverService.GetFileForDownload(id, type);
            return File(file.File, file.FileContentType, file.FileName);
        }

        //[HttpGet]
        //public ActionResult EditScreenSaver()
        //{
        //    try
        //    {
        //        if (Session["UserId"] == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        //IEnumerable<ScreenViewModel> items = _objScreenSaverService.BindScreenSaver();
        //        //ViewBag.ScreenSaver = new SelectList(items, "SAVERID", "DEPARTMENT_NAME");
        //        return PartialView("_EditCorporateNews");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //        //ModelState.AddModelError("Error", ex);
        //        //return RedirectToAction("ErrorPage");
        //    }

        //}

        //[HttpPost]
        //public ActionResult EditScreenSaver(ScreenSaverViewModel AVM)
        //{
        //    try
        //    {
        //        if (Session["UserId"] == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        ModelState.Remove("Attachment1File");

        //        if (ModelState.IsValid)
        //        {
        //            HttpFileCollectionBase file = Request.Files;

        //            FileViewModel attachment1File = GetUploadFile(file[1]);
        //            AVM.PoliciesFileName = attachment1File.FileName;
        //            AVM.PoliciesContentType = attachment1File.FileContentType;
        //            AVM.Policies = attachment1File.File;

        //            Int16 obj = _objScreenSaverService.UpdateScreenSaver_Trn(AVM);
        //            if (obj > 0)
        //            {
        //                return Json("success");
        //            }
        //            else
        //            {
        //                return Json("failed");
        //            }

        //        }
        //        else
        //        {
        //            var modelErrors = new List<string>();
        //            foreach (var modelState in ModelState.Values)
        //            {
        //                foreach (var modelError in modelState.Errors)
        //                {
        //                    modelErrors.Add(modelError.ErrorMessage);
        //                }
        //            }
        //            return Json(modelErrors);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message);
        //    }
        //}
        [HttpGet]
        public ActionResult EditScreenSaver(long id)
        {
            try
            {
                ScreenSaverViewModel data = _objScreenSaverService.GetScreenSaverDetails(id);
                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
          
        }
        [HttpPost]
        public ActionResult EditScreenSaver(ScreenSaverViewModel mst)
        {
            mst.MODIFIED_BY = Convert.ToInt16(_userId.ToString());
            //mst.ADDEDBY = Convert.ToInt16(Session["UserId"].ToString());
            FileViewModel bannerFile = GetUploadFile(mst.FILE_NAME);
            mst.PoliciesFileName = bannerFile.FileName;
            mst.PoliciesContentType = bannerFile.FileContentType;
            mst.Policies = bannerFile.File;
            short sh = _objScreenSaverService.UpdateScreenSaver_Trn(mst);
            //Below Added by Aumento For SR66258--------------------------------
            //if (sh > 0 )
            if (sh > 0 && mst.FILE_NAME != null)
            //------------------------------------------------------------------
            {
                //string path = Server.MapPath("~/Uploads/ScreenSaver/");
                string path = Path.Combine(serverpath.getFileUploadPath(), "ScreenSaver");
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                System.IO.File.WriteAllBytes(Path.Combine(path, mst.FILE_NAME.FileName), mst.Policies.ToArray());
                //TempData["AlertMessage"] = sh;
            }
            //else
            //{
            //    TempData["AlertMessage"] = sh;
            //}
            return Json(sh);
        }
        [HttpGet]
        public ActionResult SSDashboard(ScreenViewModel SVM)
        {
            ScreenViewModel obj = new ScreenViewModel();
            try
            {
                //ScreenViewModel searchViewModel = new ScreenViewModel();
                ScreenSaverViewModel CNMVM = new ScreenSaverViewModel();
               
                CNMVM.ScreenSaver = _objScreenSaverService.GetScreenSaverMasterList(SVM);

                return PartialView("ScreenSaverMaster");
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult ScreenSaver()
        {
            try
            {
                ScreenViewModel searchViewModel = new ScreenViewModel();
                ScreenSaverViewModel CNMVM = new ScreenSaverViewModel();
                CNMVM.ScreenSaver = _objScreenSaverService.ScreenSaver(searchViewModel);
                return View(CNMVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }
    }
}