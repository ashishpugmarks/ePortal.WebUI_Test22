using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class PresidentDeskController : Controller
    {      

        private readonly IPresidentDesk _objPresidentDeskServ;
        private readonly ISessionService _sessionService;
        private readonly ILogger<PresidentDeskController> _logger;
        PresidentDeskSearchModel _objPresidentDeskSearchModel;

        public PresidentDeskController(IPresidentDesk objPresidentDeskServ, ISessionService sessionService, ILogger<PresidentDeskController> logger)
        {
            _objPresidentDeskServ = objPresidentDeskServ;
            _sessionService = sessionService;
            _logger = logger;
            _objPresidentDeskSearchModel = new PresidentDeskSearchModel();
        }

        // GET: PresidentDesk
        public ActionResult Index()
        {
            return View();
        }

        // GET: PresidentDesk/Message/AllDetails
        public ActionResult GetPresident()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            var PresidentDetail = _objPresidentDeskServ.GetPresidentDetail(_objPresidentDeskSearchModel);
            return View(PresidentDetail.PresidentDetail);
        }
        // GET: PresidentDesk/Create
        [HttpGet]
        public ActionResult CreatePresident()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            try
            {
                return PartialView("_CreatePresident");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


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



        // POST: PresidentDesk/Create
        [HttpPost]
        public IActionResult CreatePresident(PresidentDesk_MSTViewModel pDMVM)
        {            
            try
            {
                
                ModelState.Remove("PRESIDENT_ID");
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    if (pDMVM.PostedFile != null && pDMVM.PostedFile.Length > 0)
                    {
                        //byte[] bytes;
                        //using (BinaryReader br = new BinaryReader(pDMVM.PostedFile.InputStream))
                        //{
                        //    bytes = br.ReadBytes(pDMVM.PostedFile.ContentLength);
                        //}

                        using (var ms = new MemoryStream())
                        {
                            pDMVM.PostedFile.CopyTo(ms);
                            pDMVM.PRESIDENT_PHOTO = ms.ToArray();
                        }

                        pDMVM.PHOTO_NAME = pDMVM.PostedFile.FileName.Substring(pDMVM.PostedFile.FileName.LastIndexOf("\\") + 1);
                        pDMVM.PHOTO_CONTENTTYPE = pDMVM.PostedFile.ContentType;                        
                    }

                    //ViewBag.presidentList = presidentList;
                    pDMVM.CREATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    var result = _objPresidentDeskServ.InsertUpdatePresidentDetail(pDMVM);
                    if (result.ValidationMessage == "Already Exists")
                    {
                        TempData["AlertMessage"] = 2; // 2 for (President already exists for the same Title, dactive existing first then Create/Edit)
                    }
                    else
                    {
                        TempData["AlertMessage"] = 1; // 1 for (Your Data has been Saved Successfully)
                    }
                }
                //else
                //{
                //    return RedirectToAction("CreatePresident");
                    
                //    //return View(pDMVM);
                //}

                    return RedirectToAction("GetPresident");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        // GET: PresidentDesk/Edit/5
        [HttpGet]
        public ActionResult EditPresident(long PresidentId)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            try
            {
                if (PresidentId != 0)
                {
                    _objPresidentDeskSearchModel.PresidentId = PresidentId;

                    var PresidentDeskDetails = _objPresidentDeskServ.GetPresidentDetail(_objPresidentDeskSearchModel);
                    if (PresidentDeskDetails.PresidentDetail != null)
                    {
                        var presidentList = PresidentDeskDetails.PresidentDetail[0];
                        presidentList.ACTIVE_TO = presidentList.ACTIVE_TO == null ? "31-Dec-9999" : presidentList.ACTIVE_TO;
                        //var presidentMessageList = PresidentDeskDetails.President_TRNDTL[0];
                        return PartialView("_EditPresident", presidentList);
                    }
                }
                return RedirectToAction("GetPresident");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        // POST: PresidentDesk/Edit/5
        [HttpPost]
        public ActionResult EditPresident(PresidentDesk_MSTViewModel pDMVM, IFormFile file)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            try
            {
                ModelState.Remove("PostedFile");
                ModelState.Remove("file");
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    if (file != null && file.Length > 0)
                    {
                        //byte[] bytes;
                        //using (BinaryReader br = new BinaryReader(file.InputStream))
                        //{
                        //    bytes = br.ReadBytes(file.ContentLength);
                        //}

                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            pDMVM.PRESIDENT_PHOTO = ms.ToArray();
                        }
                                                
                        //string[] splitFile = file.FileName.Split('.');
                        pDMVM.PHOTO_NAME = Path.GetFileName(file.FileName);
                        pDMVM.PHOTO_CONTENTTYPE = file.ContentType;
                        //pDMVM.PRESIDENT_PHOTO = bytes;
                    }
                    pDMVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    var result = _objPresidentDeskServ.InsertUpdatePresidentDetail(pDMVM);
                    if (result.ValidationMessage == "Already Exists")
                    {
                        TempData["AlertMessage"] = 2;//"President already exists for the same Title, deactive existing first then Create/Edit";
                    }
                    else
                    {
                        TempData["AlertMessage"] = 1;// "Your Data has been Saved Successfully ";
                    }
                    //return RedirectToAction("_EditPresidentMessages", pDTVM);                 
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
                // TODO: Add update logic here

                return RedirectToAction("GetPresident");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        // GET: PresidentDesk/Message/AllDetails
        public ActionResult GetPresidentMessages()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            var PresidentMessages = _objPresidentDeskServ.GetPresidentDetail(_objPresidentDeskSearchModel);
            return View(PresidentMessages.President_TRNDTL);
        }

        // GET: PresidentDesk/Details/5
        public ActionResult GetPresidentDeskDetails(long id, string TransactionType)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            try
            {
                string RedirectAction = string.Empty;
                if (id != 0)
                {

                    if (TransactionType == "PresidentDeskMST")
                    {
                        _objPresidentDeskSearchModel.PresidentId = id;
                    }
                    else if (TransactionType == "PresidentDeskTRN")
                    {
                        _objPresidentDeskSearchModel.PresidentMsgId = id;
                    }
                    else if (TransactionType == "PresidentDeskTRNAPP")
                    {
                        _objPresidentDeskSearchModel.PresidentMsgAppId = id;
                    }
                    var PresidentDeskDetails = _objPresidentDeskServ.GetPresidentDetail(_objPresidentDeskSearchModel);
                    if (PresidentDeskDetails.PresidentDetail != null && TransactionType == "PresidentDeskMST")
                    {
                        PresidentDeskDetails.President_TRNDTL = null;
                        PresidentDeskDetails.PresidentMsgApp_TRNDTL = null;
                        return PartialView("_GetPresidentDeskDetails", PresidentDeskDetails);
                    }
                    else if (PresidentDeskDetails.President_TRNDTL != null && TransactionType == "PresidentDeskTRN")
                    {
                        PresidentDeskDetails.PresidentDetail = null;
                        PresidentDeskDetails.PresidentMsgApp_TRNDTL = null;
                        return PartialView("_GetPresidentDeskDetails", PresidentDeskDetails);
                    }
                    else if (PresidentDeskDetails.PresidentMsgApp_TRNDTL != null && TransactionType == "PresidentDeskTRNAPP")
                    {
                        PresidentDeskDetails.PresidentDetail = null;
                        PresidentDeskDetails.President_TRNDTL = null;
                        return PartialView("_GetPresidentDeskDetails", PresidentDeskDetails);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        // GET: PresidentDesk/Create
        public ActionResult CreatePresidentMessages()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            try
            {
                var PresidentMessages = _objPresidentDeskServ.GetPresidentDetail(_objPresidentDeskSearchModel);
                var presidentList = PresidentMessages.PresidentDetail.Where(p => p.STATUS == 1).ToList();
                if (presidentList != null)
                {
                    ViewBag.presidentList = presidentList;
                }
                return PartialView("_CreatePresidentMessages");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
                

        // POST: PresidentDesk/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePresidentMessages(PresidentDesk_TRNViewModel pDTVM, IFormFile file)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            try
            {                
               ModelState.Remove("PRESIDENTMSG_ID");
                ModelState.Remove("file");
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    if (file != null && file.Length > 0)
                    {
                        //byte[] bytes;
                        //using (BinaryReader br = new BinaryReader(file.InputStream))
                        //{
                        //    bytes = br.ReadBytes(file.ContentLength);
                        //}

                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            pDTVM.ATTACHMENT = ms.ToArray();
                        }
                       
                        string[] splitFile = file.FileName.Split('.');
                        pDTVM.ATTACHMENT_NAME = Path.GetFileName(file.FileName);//splitFile[0];
                        pDTVM.ATTACHMENT_CONTENTTYPE = file.ContentType;
                        //pDTVM.ATTACHMENT = bytes;
                    }
                    pDTVM.CREATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    int result = _objPresidentDeskServ.InsertUpdatePresidentDeskMessage(pDTVM);
                    if (result > 0)
                    {
                        TempData["AlertMessage"] = result;
                    }
                    else
                    {
                        TempData["AlertMessage"] = result;
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

                return RedirectToAction("GetPresidentMessages");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        // GET: PresidentDesk/Edit/5
        public ActionResult EditPresidentMessages(long PresidentMsgId)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            try
            {
                _objPresidentDeskSearchModel.PresidentMsgId = PresidentMsgId;
                var PresidentDeskDetails = _objPresidentDeskServ.GetPresidentDetail(_objPresidentDeskSearchModel);
                if (PresidentDeskDetails.President_TRNDTL != null && PresidentDeskDetails.PresidentDetail != null)
                {
                    var presidentList = PresidentDeskDetails.PresidentDetail.Where(p => p.STATUS == 1).ToList();
                    ViewBag.presidentList = presidentList;
                    var presidentMessageList = PresidentDeskDetails.President_TRNDTL[0];
                    return PartialView("_EditPresidentMessages", presidentMessageList);

                }
                return RedirectToAction("GetPresidentMessages");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        // POST: PresidentDesk/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditPresidentMessages(PresidentDesk_TRNViewModel pDTVM, IFormFile file)
        {
            try
            {                
                pDTVM.MODIFIED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));

                ModelState.Remove("uploadedFile");
                ModelState.Remove("file");
                if (ModelState.IsValid)
                {
                    //HttpFileCollectionBase file = Request.Files;
                    //// TODO: Add insert logic here
                    //if (file[0] != null && file[0].ContentLength > 0)
                    //{
                    //    byte[] bytes;
                    //    using (BinaryReader br = new BinaryReader(file[0].InputStream))
                    //    {
                    //        bytes = br.ReadBytes(file[0].ContentLength);
                    //    }
                    //    string[] splitFile = file[0].FileName.Split('.');
                    //    pDTVM.ATTACHMENT_NAME = file[0].FileName.Substring(file[0].FileName.LastIndexOf("\\") + 1);
                    //    pDTVM.ATTACHMENT_CONTENTTYPE = MimeMapping.GetMimeMapping(pDTVM.ATTACHMENT_NAME);
                    //    pDTVM.ATTACHMENT = bytes;
                    //}

                    if (file != null && file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            pDTVM.ATTACHMENT = memoryStream.ToArray();
                        }

                        pDTVM.ATTACHMENT_NAME = Path.GetFileName(file.FileName);
                        pDTVM.ATTACHMENT_CONTENTTYPE = file.ContentType;
                    }

                    Int16 result = _objPresidentDeskServ.InsertUpdatePresidentDeskMessage(pDTVM);
                    if (result > 0)
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
                // TODO: Add update logic here            
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // GET: PresidentDeskMessage/ApprovalList
        public ActionResult GetPresidentMsgApprovalList()
        {
            try
            {
                var PresidentMsgAppList = _objPresidentDeskServ.GetPresidentDetail(_objPresidentDeskSearchModel);
                return View(PresidentMsgAppList.PresidentMsgApp_TRNDTL);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // GET: PresidentDeskMessage/ApprovalList
        [HttpGet]
        public ActionResult EditPresidentMsgApprovalList(long PresidentMsgAPPId)
        {
            try
            {
                _objPresidentDeskSearchModel.PresidentMsgAppId = PresidentMsgAPPId;
                var PresidentMsgAppList = _objPresidentDeskServ.GetPresidentDetail(_objPresidentDeskSearchModel);
                if (PresidentMsgAppList != null)
                {
                    var presidentMsgApprovalList = PresidentMsgAppList.PresidentMsgApp_TRNDTL[0];
                    return PartialView("_EditPresidentMsgApprovalList", presidentMsgApprovalList);
                }
                return RedirectToAction("GetPresidentMsgApprovalList");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult EditPresidentMsgApprovalList(PresidentMsgAPP_TRNViewModel pMATVM, string submit)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            try
            {
                switch (submit)
                {
                    case "Approve":
                        pMATVM.STATUS = 2;
                        break;
                    case "Reject":
                        pMATVM.STATUS = 4;
                        break;
                }


                ModelState.Remove("CREATED_DATE");
                if (ModelState.IsValid)
                {
                    pMATVM.APPROVED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    var PresidentMsgAppList = _objPresidentDeskServ.InsertUpdatePresidentMsgApproval(pMATVM);
                    TempData["AlertMessage"] = 1; // 1 for success
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
                        TempData["AlertMessage"] = modelErrors;
                    }
                }
                return RedirectToAction("GetPresidentMsgApprovalList");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public FileResult Download(Int64 id)
        {
            FileViewModel file = _objPresidentDeskServ.GetFileForDownload(id);
            return File(file.File, file.FileContentType, file.FileName);
        }

        [HttpPost]
        public IActionResult DeleteDocument(int ATTACHMENTID)
        {
            try
            {
                PresidentDesk_TRNViewModel CNPV = new PresidentDesk_TRNViewModel();
                CNPV = _objPresidentDeskServ.DeleteDocument(ATTACHMENTID);
                return Json(CNPV);
            }
            catch (Exception ex)
            {
                var Message = "";
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    Message = "Please contact to administrator" + "</br>" + ex.Message == null ? "" : ex.Message;
                }
                else
                {
                    Message = "Something went wrong";
                }

                return Json(Message, "Json");
            }

        }
    }
}
