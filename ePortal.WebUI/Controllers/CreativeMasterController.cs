using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.Json;

namespace ePortal.Web.Controllers
{
    [CSPFilter]
    public class CreativeMasterController : Controller
    {
        ICreativeMaster _objCreativeMasterService;
        ISessionService _sessionService;
        private readonly ICorporateNews _objCorporateNewsService;
        public CreativeMasterController(ICreativeMaster CorporateNewsService, IEmpLoginService oblogin, ISessionService objsessionService, ICorporateNews objCorporateNewsService)
        {
            _objCreativeMasterService = CorporateNewsService;
            _sessionService = objsessionService;
            _objCorporateNewsService = objCorporateNewsService;
            //_loginService = oblogin;
        }

        #region Creative Master
        public ActionResult CreativeMaster()
        {
            try
            {
                SearchViewModel searchViewModel = new SearchViewModel();
                CreativeMasterViewModel CMVM = new CreativeMasterViewModel();
                CMVM.CorporateNews = _objCreativeMasterService.GetCorporateNewsMasterList(searchViewModel);
                return View(CMVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        #endregion

        [HttpPost]
        public ActionResult CreativeMaster(CreativeMasterViewModel CMVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                CMVM.CorporateNews = _objCreativeMasterService.GetCorporateNewsMasterList(CMVM.SearchViewModel);
                return View(CMVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public ActionResult CreativeCorporateNewsDetails(int id)
        {
            try
            {
                return PartialView("_CreativeMasterViewDetails", _objCreativeMasterService.GetCorporateNewsDetails(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }

        }

        [HttpGet]
        public ActionResult CreateCreativeMaster()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                IEnumerable<ProcessMstViewModel> items = _objCreativeMasterService.BindContentProcess();
                ViewBag.ContentProcess = new SelectList(items, "PROCESSID", "PROCESS_NAME", "DISPLAY_FILE");
                IEnumerable<Employee_Details> items1 = _objCreativeMasterService.BindAppAuth1();
                //ViewBag.AppAuth1 = new SelectList(items1, "_ECode", "_EName");
                ViewBag.AppAuth1 = new SelectList(items1, "_ECode", "_EName");
                IEnumerable<Employee_Details> items2 = _objCreativeMasterService.BindAppAuth1();
                ViewBag.AppAuth2 = new SelectList(items2, "_ECode", "_EName");
                return PartialView("_CreateCreativeMaster");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult CreateCreativeMaster(CreativeMasterViewModel AVM)
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
                   
                    AVM.CREATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    int obj = _objCreativeMasterService.SaveProcessAttachment_Trn(AVM);
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
                return RedirectToAction("CreativeMaster");

            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = ex.Message;
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public ActionResult EditCreativeCorporateNews(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                IEnumerable<ProcessMstViewModel> items = _objCreativeMasterService.BindContentProcess();
                ViewBag.ContentProcess = new SelectList(items, "PROCESSID", "PROCESS_NAME");
                IEnumerable<Employee_Details> items1 = _objCreativeMasterService.BindAppAuth1();
                ViewBag.AppAuth1 = new SelectList(items1, "_ECode", "_EName");
                IEnumerable<Employee_Details> items2 = _objCreativeMasterService.BindAppAuth1();
                ViewBag.AppAuth2 = new SelectList(items2, "_ECode", "_EName");                
                return PartialView("_EditCreativeMaster", _objCreativeMasterService.GetCorporateNewsDetails(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }

        }

        [HttpPost]
        public ActionResult EditCorporateNews(CreativeMasterViewModel AVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Remove("BannerFile");
                ModelState.Remove("Attachment1File");
                ModelState.Remove("Attachment2File");

                if (ModelState.IsValid)
                {
                    var file = HttpContext.Request.Form.Files;
                    foreach (var f in file)
                    {
                        FileViewModel uploadedFile = GetUploadFile(f);

                        if (uploadedFile.FileContentType == "image/jpeg" || uploadedFile.FileContentType == "image/jpg")
                        {
                            AVM.BANNER_NAME = uploadedFile.FileName;
                            AVM.BANNER_CONTENTTYPE = uploadedFile.FileContentType;
                            AVM.BANNER = uploadedFile.File;
                        }
                        else if (uploadedFile.FileContentType == "video/mp4")
                        {
                            AVM.ATTACHMENT1_NAME = uploadedFile.FileName;
                            AVM.ATTACHMENT1_CONTENTTYPE = uploadedFile.FileContentType;
                            AVM.ATTACHMENT1 = uploadedFile.File;
                        }
                    }
                    AVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    Int16 obj = _objCreativeMasterService.UpdateProcessAttachment_Trn(AVM);
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
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
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
                    using (BinaryReader br = new BinaryReader(file.OpenReadStream()))
                    {
                        bytes = br.ReadBytes((int)file.Length);
                    }

                    string _FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
                    //FVM.FileContentType = MimeMapping.GetMimeMapping(_FileName);
                    string contentType = "";
                    new FileExtensionContentTypeProvider().TryGetContentType(_FileName, out contentType);
                    FVM.FileContentType = contentType;
                    //FVM.FileName = DocType + "_" + PONo + ".pdf";
                    FVM.FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
                    FVM.File = bytes;
                }
                return FVM;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        
public string AutocompleteSuggestions(string term)
    {
        List<Employee_Details> portaluser = _objCreativeMasterService.PortalAutocompleteSuggestions(term);

        var suggestions = portaluser.Select(dataitem =>
            $"{dataitem._ECode}-{dataitem._EFirstName} {dataitem._ELastName}"
        ).ToList();

        string sJSON = JsonSerializer.Serialize(suggestions);
        return sJSON;
    }


    [HttpGet]
        public ActionResult CreativeMasterApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(id); // Server.UrlDecode(Encryption.Decrypt(id))
            CreativeMasterViewModel obj = _objCreativeMasterService.GetCreativeMSTRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());            
            
            return View("CreativeMasterApproval", obj);
        }

        [HttpPost]
        public ActionResult CreativeMasterApproval([FromBody]CM_Processattachmentappmapping_TrnViewModel PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                
                PHVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());                        
                  
                    

                retVal = _objCreativeMasterService.CreativeMasterAppr(PHVM, _Employee_Details);
                
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        
        [HttpPost]
        public ActionResult DeleteDocument(Int64 ATTACHMENTID, string type)
        {
            try
            {
                CreativeMasterViewModel CNPV = new CreativeMasterViewModel();
                CNPV = _objCreativeMasterService.DeleteDocument(ATTACHMENTID, type);
                return Json(CNPV);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        [HttpGet]
        public ActionResult CreativeMasterApprovalHistory(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(id); // Server.UrlDecode(Encryption.Decrypt(id))
            CreativeMasterViewModel obj = _objCreativeMasterService.GetCreativeMSTRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

            return View("CreativeMasterApprovalHistory", obj);
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
                return RedirectToAction("CreativeMaster");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


    }
}
