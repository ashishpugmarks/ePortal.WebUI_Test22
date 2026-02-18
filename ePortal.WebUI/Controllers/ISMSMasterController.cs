
using ePortal.Application.Contracts;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;

namespace HMSI.ePortal.Web.Controllers
{
    public class ISMSMasterController : Controller
    {
        IISMSMaster _objISMSMasterService;
        private readonly IEmpLoginService _loginService;
        private readonly ISessionService _sessionService;
        public ISMSMasterController(IISMSMaster ISMSMasterService, IEmpLoginService oblogin, ISessionService objISessionService)
        {
            _objISMSMasterService = ISMSMasterService;
            _loginService = oblogin;
            _sessionService = objISessionService;
        }

        public ActionResult ISMSMaster()
        {

            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ISMSMasterViewModel viewModel = new ISMSMasterViewModel();
            viewModel.ISMSMaster = _objISMSMasterService.GetISMSMasterList(viewModel.SearchViewModel);
            return View("ISMSMaster", viewModel);

        }

        [HttpGet]
        public ActionResult createISMSMaster()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("index", "login");
                }
                ISMSMasterViewModel viewmodel = new ISMSMasterViewModel();
                
                IEnumerable<Employee_Details> items1 = _objISMSMasterService.BindAppAuth1();
                ViewBag.appauth1 = new SelectList(items1, "_ecode", "_ename");

                IEnumerable<Employee_Details> items2 = _objISMSMasterService.BindAppAuth1();
                ViewBag.appauth2 = new SelectList(items2, "_ecode", "_ename");
                
                return PartialView("_createISMSMaster",viewmodel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.ToString());
                return RedirectToAction("errorpage");
            }
        }

        private FileViewModel GetUploadFile(IFormFile file, string DocType)
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
                    FVM.FileName = DocType + "_" +DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
                    FVM.File = bytes;
                }
                return FVM;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadISMS(IFormFile FILE, string DOC_TYPE)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ISMSMasterViewModel CMAtt = new ISMSMasterViewModel();
                if (FILE.Length > 0 && !string.IsNullOrEmpty(DOC_TYPE))
                {
                    FileViewModel _file = GetUploadFile(FILE, DOC_TYPE);
                    CMAtt.ISMS_UPLOAD = _file.FileName;
                    CMAtt.ISMS_CONTENTTYPE = _file.FileContentType;
                    CMAtt.FILE_BYTE = _file.File;
                    //POAtt.DOC_TYPE = DOC_TYPE;
                    //TempData["ISMS_ATTACHMENT"] = CMAtt;
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult createISMSMaster(ISMSMasterViewModel AVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (ModelState.IsValid)
                {  
                    string CalenderType = "ISMS";
                    FileViewModel _ISMSFile = GetUploadFile(AVM.ISMSFILE, CalenderType);
                    string baseFileName = _ISMSFile.FileName;

                    AVM.ISMS_UPLOAD = _ISMSFile.FileName;
                    AVM.ISMS_CONTENTTYPE = _ISMSFile.FileContentType;
                    AVM.ISMS_BLOB = _ISMSFile.File;
                    AVM.FILE_BYTE = _ISMSFile.File;
                    AVM.CREATED_BY = _sessionService.Get<string>("userID").ToString();
                    
                    int obj = _objISMSMasterService.SaveProcessAttachment_Trn(AVM);
                  
                    string FileName = serverpath.getFileUploadPath("ISMS/" + AVM.ISMS_UPLOAD);
                    string path = serverpath.getFileUploadPath("ISMS/");
                    if (obj == 1)
                    {
                        
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (obj > 0)
                        {                            
                            if ((AVM.IsDeleted==null ||AVM.IsDeleted == 0) && AVM.FILE_BYTE != null)
                            {
                                System.IO.File.WriteAllBytes(FileName, AVM.FILE_BYTE.ToArray());
                            }                          
                        }
                    }
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
                return RedirectToAction("ISMSMaster");

            }
            catch (Exception ex)
            {
                //TempData["AlertMessage"] = ex.Message;
                ViewBag.AlertMessage = ex.Message;
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public ActionResult EditISMSMaster(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                IEnumerable<Employee_Details> items1 = _objISMSMasterService.BindAppAuth1();
                ViewBag.AppAuth1 = new SelectList(items1, "_ECode", "_EName");

                IEnumerable<Employee_Details> items2 = _objISMSMasterService.BindAppAuth1();
                ViewBag.AppAuth2 = new SelectList(items2, "_ECode", "_EName");

                ISMSMasterViewModel model = _objISMSMasterService.GetISMSMasterDetails(id);

                return PartialView("_EditISMSMaster", model);
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }

        }
        public FileResult Download(Int64 id, string type)
        {
            FileViewModel file = _objISMSMasterService.GetFileForDownload(id, type);
            return File(file.File, file.FileContentType, file.FileName);
        }

        [HttpPost]
        public ActionResult EditISMSMaster(ISMSMasterViewModel AVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Remove("ISMSFILE");

                string CalenderType = "ISMS";
                FileViewModel _ISMSFile = GetUploadFile(AVM.ISMSFILE, CalenderType);
                string baseFileName = _ISMSFile.FileName;

                AVM.ISMS_UPLOAD = _ISMSFile.FileName;
                AVM.ISMS_CONTENTTYPE = _ISMSFile.FileContentType;
                AVM.ISMS_BLOB = _ISMSFile.File;
                AVM.FILE_BYTE = _ISMSFile.File;                
                AVM.MODIFIED_BY = _sessionService.Get<string>("userID").ToString();

                Int16 obj = _objISMSMasterService.UpdateProcessAttachment_Trn(AVM);

                string FileName = serverpath.getFileUploadPath("ISMS/" + AVM.ISMS_UPLOAD);
                string path = serverpath.getFileUploadPath("ISMS/");
                if (obj == 1)
                {
                    
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                    if (obj > 0)
                    {                        
                        if (AVM.IsDeleted == 0 && AVM.FILE_BYTE != null)
                        {
                            System.IO.File.WriteAllBytes(FileName, AVM.FILE_BYTE.ToArray());
                        }                        
                    }
                }

                if (obj > 0)
                {
                    return Json("success");
                }
                else
                {
                    return Json("failed");
                }

                

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetISMSMasterDetails(int id)
        {
            try
            {
                return PartialView("_ISMSMasterViewDetails", _objISMSMasterService.GetISMSMasterDetails(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return RedirectToAction("ErrorPage");
            }

        }

        [HttpGet]
        public ActionResult ISMSMasterApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(id); // Server.UrlDecode(Encryption.Decrypt(id))
            ISMSMasterViewModel obj = _objISMSMasterService.GetISMSMSTRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

            return View("ISMSMasterApproval", obj);
        }


        [HttpPost]
        public ActionResult ISMSMasterApproval([FromBody]ISMSHDRDTLViewModel PHVM)
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

                retVal = _objISMSMasterService.ISMSMasterAppr(PHVM, _Employee_Details);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }


        [HttpGet]
        public ActionResult ISMSMasterApprovalHistory(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(id); // Server.UrlDecode(Encryption.Decrypt(id))
            ISMSMasterViewModel obj = _objISMSMasterService.GetISMSMSTRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

            return View("ISMSMasterApprovalHistory", obj);
        }

        [HttpGet]
        public ActionResult CancelReq(string id)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long _ReqId = Convert.ToInt64(id); // Server.UrlDecode(Encryption.Decrypt(id))
                retVal = _objISMSMasterService.CancelReqById(_ReqId);
                
                long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);

        }
        //Added by aumento as on 08082024 for the SR72656 -----------------------------------------------------
        [HttpGet]

        public string GetActiveLink()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                //return RedirectToAction("Index", "Login");
            }            
            string ActiveLink = _objISMSMasterService.GetActiveLink();
            return ActiveLink.ToString();          
        }
        //-----------------------------------------------------------------------------------------------------
    }
}
