using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class SMController : Controller
    {

        private readonly ISMService _smService;
        private readonly IPRService _PrService;
        private readonly ISessionService _sessionService;
        public SMController(ISMService SmService, IPRService PrService, ISessionService objSessionService)
        {
            _smService = SmService;
            _PrService = PrService;
            _sessionService = objSessionService;
        }

        public ActionResult SMViewDetail(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId;
            try { _ReqId = Convert.ToInt64(Encryption.Decrypt(WebUtility.UrlDecode(id))); } catch (Exception ex) { _ReqId = Convert.ToInt64(Encryption.Decrypt(id)); }


            // Convert.ToInt64(id); //
            return View("SMViewDetail", _smService.GetSMRequestById(_ReqId));
        }
        [HttpGet]
        public ActionResult SMMultiDocsView(long id)
        {
            SMHeaderViewModel obj = _smService.GetSMRequestById(id);
            POHeaderViewModel poDetail = new POHeaderViewModel();
            SMDetailViewModel item = new SMDetailViewModel();
            item.FILENAME = "Select";
            obj.smDetail.Insert(0, item);
            if (!string.IsNullOrEmpty(obj.PONUMBER))
            {
                poDetail = _smService.GetPODetailByPOId(obj.PONUMBER);
                foreach (var data in poDetail.poDetail)
                {
                    SMDetailViewModel item1 = new SMDetailViewModel();
                    item1.FILENAME = "SD_" + data.FILENAME;
                    obj.smDetail.Add(item1);
                }
            }

            return View("SMMultiDocsView", obj.smDetail);
        }
        public ActionResult SMTwoDocsView(long id)
        {
            SMHeaderViewModel obj = _smService.GetSMRequestById(id);
            POHeaderViewModel poDetail = new POHeaderViewModel();
            SMDetailViewModel item = new SMDetailViewModel();
            item.FILENAME = "Select";
            obj.smDetail.Insert(0, item);
            if (!string.IsNullOrEmpty(obj.PONUMBER))
            {
                poDetail = _smService.GetPODetailByPOId(obj.PONUMBER);
                foreach (var data in poDetail.poDetail)
                {
                    SMDetailViewModel item1 = new SMDetailViewModel();
                    item1.FILENAME = "SD_" + data.FILENAME;
                    obj.smDetail.Add(item1);
                }
            }

            return View("SMTwoDocsView", obj.smDetail);
        }

        public ActionResult GetVendorDetail(string vendorCode)
        {
            string vName = string.Empty;
            try
            {
                VendorViewModel VVM = _smService.GetVendorByCode(vendorCode);
                if (VVM != null)
                {
                    vName = VVM.VENDORNAME;
                }
            }
            catch
            {
                vName = string.Empty;
            }
            return Json(new { vendorName = vName });
        }

         [HttpGet]
        public ActionResult SMRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //Added by aumento for SESMRN as on 23112023===========================
            IEnumerable<SYSITE> SYSiteItems = _smService.Bind_SYSite();
            ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            //string UserId = _sessionService.Get<string>("userID").ToString();
            //ViewBag.SYSITEID = _smService.GETSYSITEUSERID(UserId);
            //======================================================================
            return View();
        }

        [HttpPost]
        public ActionResult SMRequest([FromBody]SMHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //List<SMAPPSKIPViewModel> AuthSkipList = new List<SMAPPSKIPViewModel>();
                //if (TempData["DELETED_AUTH_LIST"] != null)
                //{
                //    AuthSkipList = (List<SMAPPSKIPViewModel>)TempData["DELETED_AUTH_LIST"];
                //}
                //model.skipAuthList = AuthSkipList;
                model.smDetail = new List<SMDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                model.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                Tuple<short, long> retVal_tuple = _smService.SaveSMRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult SaveSMDetail([FromBody]SMHeaderViewModel SHVM)
        {
            short retVal = 0; long hearderId = 0; string errorMsg = string.Empty;
            List<SMAppAuthSeqViewModel> iList = new List<SMAppAuthSeqViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Remove("SMHEADERID");
                if (ModelState.IsValid)
                {
                    SHVM.smDetail = new List<SMDetailViewModel>();
                    SHVM.smAuthSeq = new List<SMAppAuthSeqViewModel>();
                    SHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    SHVM.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    SMDetailViewModel model = new SMDetailViewModel();
                    if (TempData["SM_ATTACHMENT"] != null)
                    {
                        model = JsonSerializer.Deserialize<SMDetailViewModel>(TempData["SM_ATTACHMENT"].ToString());
                        SHVM.smDetail.Add(model);
                    }

                    string pathtemp = serverpath.getFileUploadPath("SES_MRN/Temp/");
                    if (!Directory.Exists(pathtemp)) { Directory.CreateDirectory(pathtemp); }
                    if (SHVM.smDetail.Count > 0)
                    {
                        foreach (SMDetailViewModel obj in SHVM.smDetail)
                        {
                            if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                            {
                                // Added by Aumento for SR84686 start                                
                                string filePath = obj.FILENAME;
                                string fileName = System.IO.Path.GetFileName(filePath);
                                // System.IO.File.WriteAllBytes(pathtemp + fileName, obj.FILE_BYTE.ToArray());
                                System.IO.File.WriteAllBytes(pathtemp + fileName, obj.FILE_BYTE);
                                // Added by Aumento for SR84686 end
                                //System.IO.File.WriteAllBytes(pathtemp + obj.FILENAME, obj.FILE_BYTE.ToArray());
                            }
                        }
                    }
                    //block added to check to find Signature Location is valid or not
                    if (!string.IsNullOrEmpty(model.FILENAME))
                    {
                        // Added by Aumento for SR84686 start
                        string filePath = model.FILENAME;
                        string fileName = System.IO.Path.GetFileName(filePath);
                        string srcfile = fileName;
                        // Added by Aumento for SR84686 end
                        //string srcfile = model.FILENAME;
                        int x = 0; int y = 0; int pageno = 0;
                        GetSigLOCATION("Temp/" + srcfile, out x, out y, out pageno);
                        if (x == 0)
                        {
                            //System.IO.File.Delete(pathtemp + model.FILENAME);
                            System.IO.File.Delete(pathtemp + fileName); // Added by Aumento for SR84686
                            retVal = 3;
                            return Json(new { res = retVal, poId = hearderId });
                        }
                        else
                        {
                            //System.IO.File.Delete(pathtemp + model.FILENAME);
                            System.IO.File.Delete(pathtemp + fileName); // Added by Aumento for SR84686
                        }

                    }
                    // Signature Code end here
                    Employee_Details _Employee_Details =_sessionService.Get<Employee_Details>("Employee");
                    Tuple<short, List<SMAppAuthSeqViewModel>> _ret= _smService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), (decimal)SHVM.INVAMOUNT, SHVM.SM_TYPE, _Employee_Details);
                    iList = _ret.Item2;
                    TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(iList.OrderBy(o => o.APP_SEQ).ToList());
                    TempData.Keep();
                    TempData["ISIOCGBLOCK"] = JsonSerializer.Serialize(_ret.Item1);

                    Tuple<short, long> retVal_tuple = _smService.SaveSMRequest(SHVM);
                    retVal = retVal_tuple.Item1;
                    hearderId = retVal_tuple.Item2;
                    if (retVal == 1)
                    {
                        string path = serverpath.getFileUploadPath("SES_MRN/");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (SHVM.smDetail.Count > 0)
                        {
                            foreach (SMDetailViewModel obj in SHVM.smDetail)
                            {
                                if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                {
                                    System.IO.File.WriteAllBytes(path + obj.FILENAME, obj.FILE_BYTE.ToArray());
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorMsg = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    retVal = 4;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
                errorMsg = ex.Message.ToString();
            }

            
            return Json(new { res = retVal, headerId = hearderId, SEQ_LIST = iList.OrderBy(o => o.APP_SEQ).ToList(), ERROR_MSG = errorMsg, ISIOCGBlock = JsonSerializer.Deserialize<int>((string)TempData["ISIOCGBLOCK"]) });


        }

        [HttpGet]
        public ActionResult GetPOATTDetail(string poNo)
        {
            POHeaderViewModel poDetail = new POHeaderViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                poDetail = _smService.GetPODetailByPOId(poNo);
            }
            catch (Exception ex)
            {
                poDetail = null;
            }
            return Json(poDetail);
        }

        [HttpPost]
        public ActionResult UploadSM(IFormFile FILE, string DOC_TYPE, string ADDITIONAL_INFO, string SMNo)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SMDetailViewModel SMAtt = new SMDetailViewModel();
                if (FILE.Length > 0 && !string.IsNullOrEmpty(DOC_TYPE))
                {
                    // Added by Aumento for SR84686 start
                    string path = serverpath.getFileUploadPath("SES_MRN/");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    // Added by Aumento for SR84686 end
                    FileViewModel _file = GetUploadFile(FILE, DOC_TYPE, SMNo);
                    //SMAtt.FILENAME = _file.FileName;
                    SMAtt.FILENAME = pathtosave + @"\" + _file.FileName; // Added by Aumento for SR84686
                    SMAtt.FILE_CONTENTTYPE = _file.FileContentType;
                    SMAtt.FILE_BYTE = _file.File;
                    SMAtt.DOC_TYPE = DOC_TYPE;
                    SMAtt.ADDITIONAL_INFO = ADDITIONAL_INFO;
                    TempData["SM_ATTACHMENT"] = JsonSerializer.Serialize(SMAtt);
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
        public ActionResult UploadAttachment([FromForm] SMDetailViewModel formData)
        {
            short retVal = 0;
            List<SMDetailViewModel> smDtlList = new List<SMDetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.SMHEADERID > 0)
                {
                    // Added by Aumento for SR84686 start
                    string path = serverpath.getFileUploadPath("SES_MRN/");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    // Added by Aumento for SR84686 end
                    FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.SMNo);
                    smDtlList.Add(new SMDetailViewModel
                    {
                        SMHEADERID = formData.SMHEADERID,
                        //FILENAME = _file.FileName,
                        FILENAME = pathtosave + @"\" + _file.FileName, // Added by Aumento for SR84686
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = formData.DOC_TYPE,
                        ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString()),
                        SMNo = formData.SMNo,
                    });

                    #region "Check valid SES/MRN File"
                    if (formData.DOC_TYPE == "SM")
                    {
                        string pathtemp = serverpath.getFileUploadPath("SES_MRN/Temp/");
                        if (!Directory.Exists(pathtemp)) { Directory.CreateDirectory(pathtemp); }
                        if (smDtlList.Count > 0)
                        {
                            foreach (SMDetailViewModel obj in smDtlList)
                            {
                                if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                {
                                    // Added by Aumento for SR84686 start
                                    string filePath = obj.FILENAME;
                                    string fileName = System.IO.Path.GetFileName(filePath);                                    
                                    System.IO.File.WriteAllBytes(pathtemp + fileName, obj.FILE_BYTE.ToArray());
                                    // Added by Aumento for SR84686 end
                                    //System.IO.File.WriteAllBytes(pathtemp + obj.FILENAME, obj.FILE_BYTE.ToArray());

                                    //block added to check to find Signature Location is valid or not
                                    if (!string.IsNullOrEmpty(obj.FILENAME))
                                    {
                                        string srcfile = fileName; // Added by Aumento for SR84686
                                        //string srcfile = obj.FILENAME;
                                        int x = 0; int y = 0; int pageno = 0;
                                        GetSigLOCATION("Temp/" + srcfile, out x, out y, out pageno);
                                        if (x == 0)
                                        {
                                            //System.IO.File.Delete(pathtemp + obj.FILENAME);
                                            System.IO.File.Delete(pathtemp + fileName); // Added by Aumento for SR84686
                                            retVal = 3;
                                            return Json(new { res = retVal });
                                        }
                                        else
                                        {
                                            //System.IO.File.Delete(pathtemp + obj.FILENAME);
                                            System.IO.File.Delete(pathtemp + fileName); // Added by Aumento for SR84686
                                        }

                                    }
                                    // Signature Code end here
                                }
                            }
                        }
                    }

                    #endregion
                    Tuple<short, List<SMDetailViewModel>> _ret_tuple = _smService.SaveAttachment(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), formData.SMHEADERID, smDtlList);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        //string path = serverpath.getFileUploadPath("SES_MRN/");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (smDtlList.Count > 0)
                        {
                            foreach (SMDetailViewModel obj in smDtlList)
                            {
                                if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                {
                                    System.IO.File.WriteAllBytes(path + obj.FILENAME, obj.FILE_BYTE.ToArray());
                                }
                            }
                        }
                    }
                    smDtlList = _ret_tuple.Item2;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = smDtlList.ToList() });
        }

        [HttpPost]
        public ActionResult DeleteAttachment(string fileName, string docType, long smHeaderId)
        {
            short retVal = 0;
            List<SMDetailViewModel> smDtlList = new List<SMDetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Tuple<short, List<SMDetailViewModel>> _ret_tuple = _smService.DeleteAttachment(fileName, docType, smHeaderId);
                retVal = _ret_tuple.Item1;
                smDtlList = _ret_tuple.Item2;
                if (retVal == 1)
                {
                    string path = serverpath.getFileUploadPath("SES_MRN/");
                    if (System.IO.File.Exists(System.IO.Path.Combine(path, fileName)))
                    {
                        System.IO.File.Delete(System.IO.Path.Combine(path, fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = smDtlList.Where(w => w.IsDeleted == 0).ToList() });
        }

        [HttpGet]
        public ActionResult EditSMRequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //Added by aumento for SESMRN as on 23112023===========================
            IEnumerable<SYSITE> SYSiteItems = _smService.Bind_SYSite();
            ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");            
            //======================================================================
            long _ReqId = 0;
            //long _ReqId = Convert.ToInt64(id);//Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            try { _ReqId = Convert.ToInt64(Encryption.Decrypt(WebUtility.UrlDecode(id))); }
            catch (Exception ex) { _ReqId = Convert.ToInt64(Encryption.Decrypt(id)); }

            SMHeaderViewModel SHVM = _smService.GetSMRequestById(_ReqId);
            ViewBag.ESYSITEID = "0";
            if (SHVM.SYSITE1 != null)
            {
                ViewBag.ESYSITEID = SHVM.SYSITE1;
            }

            if (SHVM.smAuthSeq.Count == 0)
            {
                Employee_Details _Employee_Details =_sessionService.Get<Employee_Details>("Employee");
                //SHVM.smAuthSeq = _smService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), SHVM.AMOUNT, SHVM.SM_TYPE, _Employee_Details);
                Tuple < short, List<SMAppAuthSeqViewModel>> ret = _smService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), SHVM.AMOUNT, SHVM.SM_TYPE, _Employee_Details);
                SHVM.smAuthSeq = ret.Item2;
                TempData["ISIOCGBLOCK"] = JsonSerializer.Serialize(ret.Item2);
            }

            TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(SHVM.smAuthSeq.OrderBy(o => o.APP_SEQ).ToList());
            TempData.Keep();

            //if (SHVM.skipAuthList != null)
            //{
            //    TempData["DELETED_AUTH_LIST"] = SHVM.skipAuthList;
            //    TempData.Keep();
            //}
            VendorViewModel VVM = _smService.GetVendorByCode(SHVM.VENDORCODE);
            if (VVM != null)
            {
                SHVM.VENDORNAME = VVM.VENDORNAME;
            }
            ViewBag.strId = id;
            return View(SHVM);
        }

        [HttpPost]
        public ActionResult EditSMRequest([FromBody]SMHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.smDetail = new List<SMDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                model.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                Tuple<short, long> retVal_tuple = _smService.SaveSMRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult SMApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id))); 
            long _ReqId = Convert.ToInt64((id));
            SMHeaderViewModel obj = _smService.GetSMRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            obj.ISENABLE = obj.smAppHis.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0 && a.SMAPPHISTORYID != 0).Count().ToString();
            return View("SMApproval", obj);
        }

        [HttpPost]
        public ActionResult SMApproval([FromBody]SMAppHistoryViewModel SHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details =_sessionService.Get<Employee_Details>("Employee");
                SHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                List<SMDetailViewModel> SmDetailList = new List<SMDetailViewModel>();
                if (TempData["SM_ATTACHMENT"] != null)
                {
                   
                    SmDetailList.Add(JsonSerializer.Deserialize<SMDetailViewModel>(TempData["SM_ATTACHMENT"].ToString()));

                }

                retVal = _smService.SMApproval(SHVM, _Employee_Details, SmDetailList);
                if (retVal == 1 && SHVM.APPROVAL_STATUS == 1)
                {
                    string pathtemp = serverpath.getFileUploadPath("SES_MRN/");
                    if (!Directory.Exists(pathtemp)) { Directory.CreateDirectory(pathtemp); }
                    if (SmDetailList.Count > 0)
                    {
                        foreach (SMDetailViewModel obj in SmDetailList)
                        {
                            if (obj.FILE_BYTE != null)
                            {
                                System.IO.File.WriteAllBytes(pathtemp + obj.FILENAME, obj.FILE_BYTE.ToArray());
                            }
                        }
                    }
                }

                SMHeaderViewModel SM_Dtl = _smService.GetSMRequestById(SHVM.SMHEADERID);
                if (SM_Dtl != null)
                {
                    var OBJAPP = SM_Dtl.smAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                    if (OBJAPP == null)
                    {
                        if (SHVM.APPROVAL_STATUS == 1)
                        {
                            foreach (var doc in SM_Dtl.smDetail.Where(m => m.DOC_TYPE == "SM"))
                            {
                                string _type = SM_Dtl.SM_TYPE == 1 ? "Service Entry Sheet" : SM_Dtl.SM_TYPE == 2 ? "Material Receipt Note" : "";

                                // Added by Aumento for SR84686 
                                string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                                string path = serverpath.getFileUploadPath("SES_MRN/");
                                if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                                string STRFILENAME = System.IO.Path.Combine(pathtosave, "SM" + doc.SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf");
                                // Added by Aumento for SR84686 
                                //string STRFILENAME = "SM" + doc.SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";

                                GetDocumentWithAppendedContent(SM_Dtl.smAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), doc.FILENAME, STRFILENAME, 100, SM_Dtl.smAuthSeq, SM_Dtl.SM_TYPE, SM_Dtl.ADDEDBYNAME, SM_Dtl.DATEADDED);
                                List<SMDetailViewModel> obj = new List<SMDetailViewModel>() { new SMDetailViewModel() { DOC_TYPE = "SMA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "User Mgmt. Approved " + _type } };
                                _smService.SaveAttachment(SHVM.ADDEDBY, SHVM.SMHEADERID, obj);
                            }
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

        [HttpGet]
        public ActionResult SMCancel(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = 0;
            try { _ReqId = Convert.ToInt64(Encryption.Decrypt(WebUtility.UrlDecode(id))); }
            catch (Exception ex) { _ReqId = Convert.ToInt64(Encryption.Decrypt(id)); }
            //Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            return View("SMCancel", _smService.GetSMRequestById(_ReqId));
        }

        [HttpPost]
        public ActionResult SMCancel([FromBody]SMHeaderViewModel SHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details =_sessionService.Get<Employee_Details>("Employee");
                SHVM.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _smService.SMCancel(SHVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        #region Dashboard
        [HttpGet]
        public ActionResult SMDashboard()
        {
            SearchSMViewModel obj = new SearchSMViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails =_sessionService.Get<Employee_Details>("Employee");
                List<ADORGLEVEL> _secList = new List<ADORGLEVEL>();
                List<ADORGLEVEL> _depList = new List<ADORGLEVEL>();
                int IsUserOP = 0;
                //List<ADORGLEVEL> _opList = _PrService.BindOperationForOPMap(employeeDetails._ECode);
                List<ADORGLEVEL> _opList = _PrService.GetOrgLevelList((long)1);
                //if (_opList.Count == 0)
                //{
                //    _opList.Add(new ADORGLEVEL
                //    {
                //        ADORGLEVELID = Convert.ToInt64(employeeDetails._OpId),
                //        LEVELDESCRIP = employeeDetails._OpDesc.ToString()
                //    });
                //    obj.OperationID = employeeDetails._OpId == null ? 0 : (long)employeeDetails._OpId;
                //    obj.DivisionID = employeeDetails._DivId == null ? 0 : (long)employeeDetails._DivId;
                //    obj.DEPTID = employeeDetails._DepId == null ? 0 : (long)employeeDetails._DepId;
                //    obj.SECID = employeeDetails._SecId == null ? 0 : (long)employeeDetails._SecId;
                //    _depList = _PrService.GetOrgLevelList((long)3);
                //    _secList = _PrService.GetOrgLevelList((long)4);

                //    IsUserOP = 1;
                //}
                //else
                //{
                //    obj.OperationID = _opList.Select(s => s.ADORGLEVELID).FirstOrDefault();
                //}
                //if (_opList.Count > 0)
                //{
                //    obj.OperationID = Convert.ToInt64(employeeDetails._OpId);
                //}
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");

                List<ADORGLEVEL> _divList = _PrService.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "ADORGLEVELID", "LEVELDESCRIP");

                ViewBag.DepList = new SelectList(_depList, "ADORGLEVELID", "LEVELDESCRIP");

                ViewBag.SecList = new SelectList(_secList, "ADORGLEVELID", "LEVELDESCRIP");

                ViewBag.ISUSER_OPERATION = IsUserOP;

                obj.Doc_Status = -1; /// --- -All-
                obj.ReqStatus = 0; /// --- -Pending-

                var objki = _smService.BindKI();
                ViewBag.KIList = new SelectList(objki.OrderByDescending(m => m.Value), "Value", "Text");
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult SMDashboard([FromBody]SearchSMViewModel SSM)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details employeeDetails =_sessionService.Get<Employee_Details>("Employee");
            SearchSMViewModel _headerObj = _smService.SMDashboard(SSM, Convert.ToInt64(employeeDetails._PlantId));
            return PartialView("_DashboardList", _headerObj.SearchResult);
        }
        [HttpPost]
        public ActionResult PAUpload([FromBody] VM_PaymentAdvise_Master data)
        {
            return PartialView("_PaymentAdviseUpload", data);
        }
        [HttpPost]
        public ActionResult PARequest([FromBody]List<VM_PaymentAdvise_Master> info)
        {
            short retval = 0;
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                foreach (var data in info)
                {
                    data.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    data.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    var PA_year = data.PA_DATE.Year;
                    var PA_month = data.PA_DATE.Month;
                    if (PA_month < 4)
                    {
                        PA_month += 9;
                        PA_year -= 1;
                    }
                    else
                    {
                        PA_month -= 3;
                    }
                    string year_month = Convert.ToString(PA_year) + "_" + Convert.ToString(PA_month);
                    data.DOCUMENT_PATH = "Uploads/Finance/" + year_month;
                    retval = _smService.PARequest(data);
                    if (retval == -1)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                retval = -1;
            }
        
            return Json(new { res = retval});

        }
        [HttpPost]
        public ActionResult UploadCSV()
        {
            return PartialView("_UploadCSV");
        } 
        [HttpPost]
        public  ActionResult ConvertCSVtoDataTable()
        {
            ActionResult retval = Json(new { res = 0 });
            var attachedFile = HttpContext.Request.Form.Files["CsvDoc"];
            if (attachedFile == null || attachedFile.Length <= 0) return Json(null);
            var csvReader = new StreamReader(attachedFile.OpenReadStream());
            string inputDataRead;
            var values = new List<string>();
            while ((inputDataRead = csvReader.ReadLine()) != null)
            {
                if (inputDataRead.Trim().Replace(" ", "").Replace(",", " ") != null && inputDataRead.Trim().Replace(" ", "").Replace(",", " ").Trim()!="")
                {
                    values.Add(inputDataRead.Trim().Replace(" ", "").Replace(",", " "));
                }

            }
            values.Remove(values[0]);
            List<VM_PaymentAdvise_Master> result = new List<VM_PaymentAdvise_Master>();
                foreach (var value in values)
                {
                string errmsg = "";
                    var uploadModelRecord = new VM_PaymentAdvise_Master();
                    var eachValue = value.Split(' ');
                    if ((eachValue[0] != "" || eachValue[0] != null)&& eachValue[0].Length<=7)
                    {
                    try
                    {
                        uploadModelRecord.PAYMENTADVISE_NO = eachValue[0] != "" ? (eachValue[0]) : "";
                    }
                    catch(Exception ex)
                    {
                        errmsg += "Invalid Payment Advise No,";
                    }
                    }
                    else
                    {
                        errmsg += "Invalid Payment Advise No,";
                    }
                    if (eachValue[1] != null || eachValue[1] != "")
                    {
                    try { 
                        uploadModelRecord.PA_DATE = eachValue[1] != "" ? DateTime.Parse(eachValue[1]) : new DateTime();
                    }
                    catch (Exception ex)
                    {
                        errmsg += "Invalid  PA Date,";
                    }
                }
                    else
                    {
                        errmsg += "Invalid PA Date,";
                    }
                if (eachValue[2] != "" || eachValue[2] != null)
                    {
                    try
                    {
                        uploadModelRecord.Invoiceno = eachValue[2] != "" ? (eachValue[2]) : "";
                    }
                    catch(Exception ex)
                    {
                        errmsg += "Invalid Invoice No,";
                    }
                    }
                    else
                    {
                        errmsg += "Invalid Invoice No,";
                    }
                    if (eachValue[3] != "" || eachValue[3] != null)
                    {
                    try
                    {
                        uploadModelRecord.InvoiceDate = eachValue[3] != "" ? Convert.ToDateTime(eachValue[3]) : new DateTime();
                    }
                    catch(Exception ex)
                    {
                        errmsg += "Invalid Invoice Date,";
                    }
                    }
                    else
                    {
                        errmsg += "Invalid Invoice Date,";
                    }
                    if (eachValue[4] != "" || eachValue[4] != null)
                    {
                    try
                    {
                        uploadModelRecord.VenderCode = eachValue[4] != "" ? eachValue[4] : "";
                    }
                    catch(Exception ex)
                    {
                        errmsg += "Invalid Vendor Code,";
                    }
                    }
                    else
                    {
                        errmsg += "Invalid Vendor Code,";
                    }
                    
                    uploadModelRecord.REMARK = "";
                if (errmsg != "")
                {
                    return Json(new { res = 3, errmsg = errmsg });
                }
                List<String> req_no = new List<string>();
                    long retVal=(_smService.GetSMNO(uploadModelRecord.Invoiceno, uploadModelRecord.InvoiceDate, uploadModelRecord.VenderCode));
                if (retVal == -1)
                {
                    return  Json(new { res = retVal });
                }
                req_no.Add(Convert.ToString(retVal));
                    uploadModelRecord.reqno_list=req_no;
                
                    result.Add(uploadModelRecord);
                }
            return PARequest(result);
            //using (StreamReader sr = new StreamReader(strFilePath))
            //{
            //    string[] headers = sr.ReadLine().Split(',');
            //    foreach (string header in headers)
            //    {
            //        dt.Columns.Add(header);
            //    }
            //    while (!sr.EndOfStream)
            //    {
            //        string[] rows = sr.ReadLine().Split(',');
            //        DataRow dr = dt.NewRow();
            //        for (int i = 0; i < headers.Length; i++)
            //        {
            //            dr[i] = rows[i];
            //        }
            //        dt.Rows.Add(dr);
            //    }

            //}

            //return View("PADashboard");
        }
        [HttpPost]
        public ActionResult UpdateDocStatus(long headerId)
        {
            short retVal = 0;
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                retVal = _smService.UpdateDocStatus(headerId, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpPost]
        public ActionResult UploadPAAttachment(VM_PADetailViewModel formData)
        {
            short retVal = 0;
            List<VM_PADetailViewModel> smDtlList = new List<VM_PADetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE))
                {
                    FileViewModel _file = GetUploadPAFile(formData.FILE, formData.DOC_TYPE, formData.PAYMENTADVISE_NO.ToString());
                    smDtlList.Add(new VM_PADetailViewModel
                    {
                        PAYMENTADVISE_NO = formData.PAYMENTADVISE_NO,
                        PA_DATE = formData.PA_DATE,
                        FILENAME = _file.FileName,
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = formData.DOC_TYPE,
                        ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString()),
                    });
                    Tuple<short, List<VM_PADetailViewModel>> _ret_tuple = _smService.SavePAAttachment(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), smDtlList, formData.PAYMENTADVISE_NO);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        string path = serverpath.getFileUploadPath("PA/");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (smDtlList.Count > 0)
                        {
                            foreach (VM_PADetailViewModel obj in smDtlList)
                            {
                                if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                {
                                    System.IO.File.WriteAllBytes(path + obj.FILENAME, obj.FILE_BYTE.ToArray());
                                }
                            }
                        }
                    }
                    smDtlList = _ret_tuple.Item2;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = smDtlList.ToList() });
        }
        [HttpGet]
        public ActionResult PADashboard()
        {
            VM_PaymentAdvise_Master obj = new VM_PaymentAdvise_Master();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails =_sessionService.Get<Employee_Details>("Employee");
                obj.PAYMENTADVISE_NO = "";
                List<VM_PaymentAdvise_Master> model1 = _smService.GetPADetails(obj);
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }
        [HttpPost]
        public ActionResult PADashboard([FromBody]VM_PaymentAdvise_Master model)
        {
            List<VM_PaymentAdvise_Master> model1 = _smService.GetPADetails(model);
            return PartialView("_PADashboardList", model1);
        }
        private FileViewModel GetUploadPAFile(IFormFile file, string DocType, string SMNo)
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
                    FVM.FileName = DocType + "_" + SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
                    FVM.File = bytes;
                }
                return FVM;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpDelete]
        public ActionResult DeletePAAttachment(string fileName, string docType, long PA_HeaderID, string PA_ID)
        {
            short retVal = 0;
            List<VM_PADetailViewModel> smDtlList = new List<VM_PADetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Tuple<short, List<VM_PADetailViewModel>> _ret_tuple = _smService.DeletePAAttachment(fileName, docType, PA_HeaderID, PA_ID);
                retVal = _ret_tuple.Item1;
                smDtlList = _ret_tuple.Item2;
                if (retVal == 1)
                {
                    string path = serverpath.getFileUploadPath("PA/");
                    if (System.IO.File.Exists(System.IO.Path.Combine(path, fileName)))
                    {
                        System.IO.File.Delete(System.IO.Path.Combine(path, fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = smDtlList.Where(w => w.PAYMENTADVISE_NO == PA_ID).ToList() });

        }
        [HttpPost]
        public ActionResult DeletePA([FromBody] DeletePA obj)
        {
            return PartialView("_DeletePA", obj);
        }
        [HttpPost]
        public ActionResult DeletePAReq([FromBody] DeletePA obj)
        {
            short retval = 0;
            retval = _smService.deletePAReq(obj);
            if (retval == 1)
            {
                var PA_year = obj.PA_DATE.Year;
                var PA_month = obj.PA_DATE.Month;
                if (PA_month < 4)
                {
                    PA_month += 9;
                    PA_year -= 1;
                }
                else
                {
                    PA_month -= 3;
                }
                string year_month = Convert.ToString(PA_year) + "_" + Convert.ToString(PA_month);
                string path = serverpath.getFileUploadPath("../" + obj.DOCUMENT_PATH);
                string filename = year_month + "_" + obj.PAYMENTADVISE_NO + "_" + obj.SES_NO;
                if (System.IO.File.Exists(System.IO.Path.Combine(path, filename + "_Others.pdf")))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(path, filename + "_Others.pdf"));
                }
                if (System.IO.File.Exists(System.IO.Path.Combine(path, filename + "_PO.pdf")))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(path, filename + "_PO.pdf"));
                }
                if (System.IO.File.Exists(System.IO.Path.Combine(path, filename + "_INV.pdf")))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(path, filename + "_INV.pdf"));
                }
            }
            return Json(retval);
        }
      
        [HttpPost]
        public ActionResult PAViewDocs(string PAYMENTADVISE_NO)
        {
            List<VM_PADetailViewModel> PADocs = _smService.PAdocs(PAYMENTADVISE_NO);
            return PartialView("_ViewPADoc",PADocs);
        }
        [HttpGet]
        public ActionResult SMFinApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("SMDashboard", "SM");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult SMFinApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64((id));
            SMHeaderViewModel obj = _smService.GetSMRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            obj.ISENABLE = "1"; // obj.smAppHis.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0).Count().ToString();
            return View("SMFinApproval", obj);
        }

        [HttpPost]
        public ActionResult SMFinFinalApproval([FromBody]SMAppHistoryViewModel SHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details =_sessionService.Get<Employee_Details>("Employee");
                SHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                List<SMDetailViewModel> SmDetailList = new List<SMDetailViewModel>();
                if (TempData["SM_ATTACHMENT"] != null)
                {
                    //SmDetailList.Add((SMDetailViewModel)TempData["SM_ATTACHMENT"]);
                    SmDetailList.Add(JsonSerializer.Deserialize<SMDetailViewModel>(TempData["SM_ATTACHMENT"].ToString()));
                }

                retVal = _smService.SMFinApproval(SHVM, _Employee_Details, SmDetailList);
                if (retVal == 1 && SHVM.APPROVAL_STATUS == 1)
                {
                    string pathtemp = serverpath.getFileUploadPath("SES_MRN/");
                    if (!Directory.Exists(pathtemp)) { Directory.CreateDirectory(pathtemp); }
                    if (SmDetailList.Count > 0)
                    {
                        foreach (SMDetailViewModel obj in SmDetailList)
                        {
                            if (obj.FILE_BYTE != null)
                            {
                                System.IO.File.WriteAllBytes(pathtemp + obj.FILENAME, obj.FILE_BYTE.ToArray());
                            }
                        }
                    }
                }

                SMHeaderViewModel SM_Dtl = _smService.GetSMRequestById(SHVM.SMHEADERID);
                if (SM_Dtl != null)
                {
                    var OBJAPP = SM_Dtl.smAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                    if (OBJAPP == null)
                    {
                        if (SHVM.APPROVAL_STATUS == 1 && SM_Dtl.PROCESS_STATUS == 9) //// 9-Approved by finance
                        {
                            foreach (var doc in SM_Dtl.smDetail.Where(m => m.DOC_TYPE == "SM"))
                            {
                                string _type = SM_Dtl.SM_TYPE == 1 ? "Service Entry Sheet" : SM_Dtl.SM_TYPE == 2 ? "Material Receipt Note" : "";
                                // Added by Aumento for SR84686 
                                string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                                string path = serverpath.getFileUploadPath("SES_MRN/");
                                if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                                string STRFILENAME = System.IO.Path.Combine(pathtosave, "SM" + doc.SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf");
                                // Added by Aumento for SR84686
                                //string STRFILENAME = "SMFA" + doc.SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
                                
                                GetDocumentWithAppendedContent(SM_Dtl.smAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), doc.FILENAME, STRFILENAME, 100, SM_Dtl.smAuthSeq, SM_Dtl.SM_TYPE, SM_Dtl.ADDEDBYNAME, SM_Dtl.DATEADDED);
                                List<SMDetailViewModel> obj = new List<SMDetailViewModel>() { new SMDetailViewModel() { DOC_TYPE = "SMFA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "Finance Approved " + _type } };
                                _smService.SaveAttachment(SHVM.ADDEDBY, SHVM.SMHEADERID, obj);
                            }
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

        public ActionResult BindTaxationAuth(short typeId)
        {
            List<SMTaxationAuthViewModel> _AuthList = new List<SMTaxationAuthViewModel>();
            try
            {
                Employee_Details employeeDetails =_sessionService.Get<Employee_Details>("Employee");
                _AuthList = _smService.GetTaxationAuthority(typeId, Convert.ToInt64(employeeDetails._PlantId));
                return Json(_AuthList);
            }
            catch (Exception ex)
            {
                return Json(_AuthList);
            }
        }

        public ActionResult GetTaxationAuthById(long id)
        {
            List<SMAppHistoryViewModel> HisObjList = new List<SMAppHistoryViewModel>();
            try
            {
                SMHeaderViewModel SMModel = _smService.GetSMRequestById(id);
                if (SMModel != null)
                {
                    if (SMModel.smAppHis.Count > 0)
                    {
                        HisObjList = (from _SmAppHis in SMModel.smAppHis.Where(w => w.APPTYPE == 4)
                                          //where _SmAppHis.APPROVAL_STATUS == 0
                                      select _SmAppHis).ToList();

                    }
                }
            }
            catch (Exception ex)
            {
                HisObjList = new List<SMAppHistoryViewModel>();
            }
            return Json(HisObjList);
        }

        [HttpPost]
        public ActionResult UpdateTaxationAuth([FromBody]List<SMAppHistoryViewModel> iList)
        {
            short retVal = 0;

            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                retVal = _smService.UpdateTaxationAuth(iList, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult SMTaxationDashboard()
        {
            SearchSMViewModel obj = new SearchSMViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult SMTaxationDashboard([FromBody] SearchSMViewModel SSM)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SearchSMViewModel _headerObj = _smService.SMTaxationDashboard(SSM, Convert.ToInt64(_sessionService.Get<string>("userID")));
            return PartialView("_TaxationDashboardList", _headerObj.SearchResult);
        }

        [HttpGet]
        public ActionResult SMTaxationApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("SMTaxationDashboard", "SM");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult SMTaxationApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64((id));
            SMHeaderViewModel obj = _smService.GetSMRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            obj.ISENABLE = "1"; // obj.smAppHis.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0).Count().ToString();
            return View("SMTaxationApproval", obj);
            //SMAppHistoryViewModel model = new SMAppHistoryViewModel();
            //model.SMHEADERID = id;
            //return PartialView("_UpdateTaxationRemark", model);
        }

        [HttpPost]
        public ActionResult SMTaxationApprovalFinal([FromBody]SMAppHistoryViewModel SHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _smService.SMTaxationApproval(SHVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult ExportToExcel([FromBody]SearchSMViewModel SSVM)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails =_sessionService.Get<Employee_Details>("Employee");
                SearchSMViewModel _headerObj = _smService.SMDashboard(SSVM, Convert.ToInt64(employeeDetails._PlantId));
                string str = this.excelHtml(_headerObj.SearchResult);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = JsonSerializer.Serialize(str);
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        [HttpPost]
        public ActionResult SMIPExportToExcel([FromBody] SearchSMViewModel SSVM)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SearchSMViewModel _headerObj = _smService.SMIPDashboard(SSVM, Convert.ToInt64(employeeDetails._PlantId));
                string str = this.excelHtml(_headerObj.SearchResult);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = JsonSerializer.Serialize(str);
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public ActionResult DownloadExcel()
        {
            try
            {
                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }
                string str =JsonSerializer.Deserialize<string>(TempData["EXCELFILE"].ToString());
               // HttpContext.Response.AddHeader("content-disposition", "attachment; filename=SES_MRNReport.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel", "SES_MRNReport.xls");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string excelHtml(List<SMHeaderViewModel> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Number</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Vendor Code</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Vendor Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Type</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>SES/MRN Number</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Amount</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice No</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice Amount</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Request Date</th>");
                //stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation</th>");
                //stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division</th>");
                //stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Department</th>");
                //stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Section</th>");
                //stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Indent Number</th>");
                //stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Indent Amount</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Document Status</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (SMHeaderViewModel AHVM in _headerList)
                {
                    string _processStatus = "";
                    if (AHVM.PROCESS_STATUS == 2 || AHVM.PROCESS_STATUS == 5)
                    {
                        _processStatus = "Pending";
                    }
                    else if (AHVM.PROCESS_STATUS == 8)
                    {
                        _processStatus = "Taxation Reviewed";
                    }
                    else if (AHVM.PROCESS_STATUS == 9)
                    {
                        _processStatus = "Accepted";
                    }
                    else if (AHVM.PROCESS_STATUS == 6)
                    {
                        _processStatus = "Hold";
                    }
                    else if (AHVM.PROCESS_STATUS == 7)
                    {
                        _processStatus = "Forwarded To Taxation";
                    }

                    string _docStatus = "";
                    if (AHVM.Document_Status == 1)
                    {
                        _docStatus = "Received";
                    }
                    else
                    {
                        _docStatus = "Not Received";
                    }

                    string reqType = "";
                    if (AHVM.SM_TYPE == 1)
                    {
                        reqType = "Service Entry Sheet";
                    }
                    else if (AHVM.SM_TYPE == 2)
                    {
                        reqType = "Material Receipt Note";
                    }

                    string _amount = "";
                    if (AHVM.INVOICENO == "0")
                    {
                        if (AHVM.AMOUNT <= 200000)
                        {
                            _amount = "<= 2 Lakh";
                        }
                        else    if (AHVM.AMOUNT > 200000)
                        {
                            _amount = "> 2 Lakh";
                        }
                    }
                    List<string> sesno = new List<string>();
                    string ses = "";
                    if (string.IsNullOrEmpty(AHVM.SM_NO))
                    {
                        sesno=AHVM.smDetail.Where(s => s.SMNo != " " && s.SMNo != null).Select(s => s.SMNo).ToList();
                        foreach(var no in sesno)
                        {
                            ses += no + ", ";
                        }
                        AHVM.SM_NO = ses;
                    }
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.SMHEADERID + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.Emp_Detail._ECode + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._EName + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.VENDORCODE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.VENDORNAME + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + reqType + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.SM_NO + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + _amount + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.INVOICENO + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.INVAMOUNT + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.INVOICEDATE + "</td>");
                    //stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._OpDesc + "</td>");
                    //stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._DivDesc + "</td>");
                    //stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._DepDesc + "</td>");
                    //stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.Emp_Detail._SecDescrip + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.DATEADDED).ToString("dd-MMM-yyyy") + "</td>");
                    //stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.IndentNo + "</td>");
                    //stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.IndentAmount + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + _processStatus + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + _docStatus + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        #endregion

        [HttpGet]
        public ActionResult GetAppAuthority(string eCode, int designationId, string designation)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Login_Employee_Details =_sessionService.Get<Employee_Details>("Employee");
                List<SMAppAuthSeqViewModel> AuthSeqList = new List<SMAppAuthSeqViewModel>();

                Employee_Details employee_dtl = _smService.GetAuthEmpById(Convert.ToInt64(eCode), designationId, designation, _Login_Employee_Details);
                if (employee_dtl == null) { employee_dtl = new Employee_Details(); }

                if (TempData["APPROVAL_AUTH_LIST"] != null)
                {
                    AuthSeqList = (List<SMAppAuthSeqViewModel>)TempData["APPROVAL_AUTH_LIST"];
                }

                if (employee_dtl._ECode > 0 && !string.IsNullOrEmpty(employee_dtl._EName))
                {
                    AuthSeqList.Add(new SMAppAuthSeqViewModel
                    {
                        ADEMPCODE = employee_dtl._ECode,
                        ADEMPNAME = employee_dtl._EName,
                        ADDESIGNATION = employee_dtl._Desig,
                        APP_SEQ = Convert.ToInt16(designationId),
                        APPTYPE = 1,
                        FNDESID = Convert.ToInt16(employee_dtl._FnDesigId == null ? 0 : employee_dtl._FnDesigId),
                    });
                }
                TempData["APPROVAL_AUTH_LIST"] = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();

                return Json(new
                {
                    ECODE = employee_dtl._ECode,
                    ENAME = employee_dtl._EName,
                    SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ECODE = 0,
                    ENAME = "",
                    SEQ_LIST = new List<SMAppAuthSeqViewModel>()
                });
            }
        }

        //public ActionResult DeleteAppAuthority(string eCode, string remark)
        //{
        //    short retval = 0;
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        List<SMAppAuthSeqViewModel> AuthSeqList = new List<SMAppAuthSeqViewModel>();
        //        //List<SMAPPSKIPViewModel> AuthSkipList = new List<SMAPPSKIPViewModel>();

        //        if (TempData["APPROVAL_AUTH_LIST"] != null)
        //        {
        //            AuthSeqList = (List<SMAppAuthSeqViewModel>)TempData["APPROVAL_AUTH_LIST"];
        //        }

        //        //if (TempData["DELETED_AUTH_LIST"] != null)
        //        //{
        //        //    AuthSkipList = (List<SMAPPSKIPViewModel>)TempData["DELETED_AUTH_LIST"];
        //        //}

        //        if (AuthSeqList.Count > 0)
        //        {
        //            //// --- Save skip authority --- ////
        //            SMAppAuthSeqViewModel skipAuth = AuthSeqList.Where(r => r.ADEMPCODE == Convert.ToInt64(eCode)).FirstOrDefault();
        //            if (skipAuth != null)
        //            {
        //                if (!AuthSkipList.Any(a => a.ADEMPCODE == skipAuth.ADEMPCODE))
        //                {
        //                    SMAPPSKIPViewModel newSkipAuth = new SMAPPSKIPViewModel();
        //                    newSkipAuth.ADEMPCODE = skipAuth.ADEMPCODE;
        //                    newSkipAuth.SKIPREMARK = remark;
        //                    AuthSkipList.Add(newSkipAuth);
        //                }
        //            }
        //            TempData["DELETED_AUTH_LIST"] = AuthSkipList;
        //            //// --- End --- ////

        //            AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode));

        //            TempData["APPROVAL_AUTH_LIST"] = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();

        //            retval = 1;
        //        }

        //        return Json(new { RESULT = retval, SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList() });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { RESULT = -1 });
        //    }
        //}

        private FileViewModel GetUploadFile(IFormFile file, string DocType, string SMNo)
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
                    FVM.FileName = DocType + "_" + SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
                    FVM.File = bytes;
                }
                return FVM;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public ActionResult GetPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/SES_MRN/"; //// serverpath.getFileUploadPath("PO/");
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"888px\"></object>";
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
        public ActionResult GetTwoPDF(string fileName)
        {
            string type = fileName.Substring(0, 2);
            if (type != "SD")
            {
                try
                {
                    string file_path = "../../../Uploads/SES_MRN/"; //// serverpath.getFileUploadPath("PO/");
                    string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
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
            else
            {
                try
                {
                    fileName = fileName.Substring(3, fileName.Length - 3);
                    string file_path = "../../../Uploads/PO/"; //// serverpath.getFileUploadPath("PO/");
                    string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
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
        }
        public ActionResult GetMultiPDF(string fileName)
        {
            string type = fileName.Substring(0, 2);
            if (type != "SD")
            {
                try
                {
                    string file_path = "../../../Uploads/SES_MRN/"; //// serverpath.getFileUploadPath("PO/");
                    string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"260px\"></object>";
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
            else
            {
                try
                {
                    fileName = fileName.Substring(3, fileName.Length - 3);
                    string file_path = "../../../Uploads/PO/"; //// serverpath.getFileUploadPath("PO/");
                    string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"260px\"></object>";
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
        }
        public ActionResult GetPAPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/PA/"; //// serverpath.getFileUploadPath("PO/");
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
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
        public string AutocompleteSuggestions(string term)
        {
            List<VendorViewModel> vList = _smService.VendorAutocompleteSuggestions(term);
            List<string> list = new List<string>();
            foreach (var dataitem in vList)
            {
                list.Add(dataitem.VENDORCODE.ToString() + "~" + dataitem.VENDORNAME.ToString() + "");
            }
            //ystem.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = JsonSerializer.Serialize(list);
            return sJSON;
        }
        public void GetDocumentWithAppendedContent(List<SMAppHistoryViewModel> HISList, string SRCfileName, string DSTfileName, int marginLeft, List<SMAppAuthSeqViewModel> authseq, short SM_type, string ReqName, DateTime dateadded)
        {
            int X = 0, Y = 0, Pageno = 0;
            GetSigLOCATION(SRCfileName, out X, out Y, out Pageno);
            string path = serverpath.getFileUploadPath("SES_MRN/" + SRCfileName);
            var writer = new PdfWriter(serverpath.getFileUploadPath("SES_MRN/" + DSTfileName));

            var pdfResult = new PdfDocument(new PdfReader(path), writer);
            var document = new Document(pdfResult);

            //document.Add(div);

            //for (int i = 1; i <= pagecount; i++)
            if (X != 0 && Y != 0 && Pageno != 0)
            {

                var div = new Div();
                Canvas canvas;
                PdfPage page = pdfResult.GetPage(Pageno);
                int position = 0;

                var userseq = authseq.Where(m => m.APPTYPE == 1).ToList().OrderByDescending(m => m.APP_SEQ).ToList();

                for (int j = 0; j < userseq.Count; j++)
                {
                    Paragraph pgr = new Paragraph();
                    pgr.SetFontSize(6);
                    Int32 authseqno = userseq[j].APP_SEQ; //(authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().APP_SEQ);
                    string empname = userseq[j].ADEMPNAME;//authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().ADEMPNAME;
                    string fundesgname = userseq[j].FUNDESG_NAME;
                    if (authseqno > 0 && HISList.Any(a => a.ADEMPCODE == userseq[j].ADEMPCODE))
                    {
                        string appdate = HISList.Where(m => m.ADEMPCODE == userseq[j].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");
                        //Changed by TTL on 20-June-2025 against SR100690 > CR6533 - Start
                        if (SM_type == 1)
                        {

                            pgr.Add(empname + "\n");
                            pgr.Add(fundesgname + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);

                            if (fundesgname.Trim().ToUpper() == "DIVISION HEAD" || fundesgname.Trim().ToUpper() == "OPERATING HEAD") //Changed by TTL on 13-Aug-2025 against SR104869 > CR6926
                            {
                                //div.SetFixedPosition((X + 150), Y - 39, 100); //Changed by TTL on 17-June-2025 against SR100690 > CR6533
                                div.SetFixedPosition((X + 150), Y - 62, 170);
                            }
                            if (fundesgname.Trim().ToUpper() == "DEPARTMENT HEAD")
                            {
                                //div.SetFixedPosition((X - 20), Y - 39, 100); //Changed by TTL on 17-June-2025 against SR100690 > CR6533
                                div.SetFixedPosition((X - 20), Y - 62, 340);
                            }
                            if (fundesgname.Trim().ToUpper() == "SECTION HEAD")
                            {
                                //div.SetFixedPosition((X - 20), Y - 39, 100); //Changed by TTL on 17-June-2025 against SR100690 > CR6533
                                div.SetFixedPosition((X - 160), Y - 62, 340);
                            }
                        }
                        if (SM_type == 2)
                        {
                            pgr.Add(empname + "\n");
                            pgr.Add(fundesgname + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            if (fundesgname.Trim().ToUpper() == "DEPARTMENT HEAD" || fundesgname.Trim().ToUpper() == "OPERATING HEAD") //Changed by TTL on 04-Sep-2025 against SR104869 > CR6926
                            {
                                //div.SetFixedPosition((X + 150), Y - 39, 100); //Changed by TTL on 17-June-2025 against SR100690 > CR6533
                                div.SetFixedPosition((X + 140), Y - 65, 170);
                            }
                            if (fundesgname.Trim().ToUpper() == "SECTION HEAD")
                            {
                                //div.SetFixedPosition((X - 20), Y - 39, 100); //Changed by TTL on 17-June-2025 against SR100690 > CR6533
                                div.SetFixedPosition((X - 20), Y - 65, 340);
                            }
                        }
                        //Changed by TTL on 20-June-2025 against SR100690 > CR6533 - End
                        if (j == 2)
                            break;

                    }
                    //Create canvas fro the last page
                    canvas = new Canvas(page, page.GetPageSize());
                    canvas.Add(div);
                }

                //Adding Prepared by
                Paragraph pgr2 = new Paragraph();
                pgr2.SetFontSize(6);
                pgr2.Add(ReqName + "\n");
                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                pgr2.Add(dateadded.ToString("dd-MMM-yyyy"));
                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                div.Add(pgr2);
                //Changed by TTL on 20-June-2025 against SR100690 > CR6533 - Start
                if (SM_type == 1)
                {
                    div.SetFixedPosition((X - 180), Y - 60, 100);
                }
                else if (SM_type == 2)
                {
                    div.SetFixedPosition((X - 190), Y - 55, 100); //Changed by TTL on 04-Sep-2025 against SR104869 > CR6926
                }
                //Changed by TTL on 20-June-2025 against SR100690 > CR6533 - End
                canvas = new Canvas(page, page.GetPageSize());
                canvas.Add(div);
                //End Prepared by Annotation Code

                var IOCGseq = authseq.Where(m => m.APPTYPE == 2).ToList().ToList();
                var IOCGLIST = IOCGseq.Select(m => m.ADEMPCODE).Distinct().ToList();
                //for (int k = 0; k < IOCGseq.Count; k++)
                if (IOCGseq.Count() > 0)
                {
                    Paragraph pgr1 = new Paragraph();
                    pgr1.SetFontSize(6);
                    //Int32 IOCGseqno = IOCGseq[k].APP_SEQ; //(authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().APP_SEQ);
                    //string empnameiocg = IOCGseq[k].ADEMPNAME;//authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().ADEMPNAME;
                    var isiocgapp = HISList.Where(m => IOCGLIST.Contains(m.ADEMPCODE) && m.APPROVAL_STATUS == 1).OrderByDescending(m => m.SMAPPHISTORYID).FirstOrDefault();
                    string empnameiocg = IOCGseq.Where(m => m.ADEMPCODE == isiocgapp.ADEMPCODE).FirstOrDefault().ADEMPNAME;
                    if (isiocgapp != null)
                    {
                        string appdate = isiocgapp.APPROVALDATE.Value.ToString("dd-MMM-yyyy"); //HISList.Where(m => m.ADEMPCODE == IOCGseq[k].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");
                        if (SM_type == 1)
                        {

                            pgr1.Add(empnameiocg + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr1.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr1);
                            div.SetFixedPosition((X - 190), Y - 125, 100); //Changed by TTL on 20-June-2025 against SR100690 > CR6533
                            canvas = new Canvas(page, page.GetPageSize());
                            canvas.Add(div);
                        }
                        if (SM_type == 2)
                        {

                            pgr1.Add(empnameiocg + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr1.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr1);
                            div.SetFixedPosition((X - 190), Y - 150, 100); //Changed by TTL on 20-June-2025 against SR100690 > CR6533, by TTL on 04-Sep-2025 against SR104869 > CR6926
                            canvas = new Canvas(page, page.GetPageSize());
                            canvas.Add(div);
                        }
                        //break;
                    }
                }

                //// --- Finance Approval
                var FINseq = authseq.Where(m => m.APPTYPE == 3).OrderByDescending(o => o.APP_SEQ).FirstOrDefault();
                if (FINseq != null)
                {
                    Paragraph pgr1 = new Paragraph();
                    pgr1.SetFontSize(6);
                    Int32 FINseqno = FINseq.APP_SEQ;
                    string empnameFin = FINseq.ADEMPNAME;
                    var isFinapp = HISList.Where(m => m.ADEMPCODE == FINseq.ADEMPCODE && m.APPROVAL_STATUS == 1).FirstOrDefault();
                    if (isFinapp != null)
                    {
                        string finAppdate = HISList.Where(m => m.ADEMPCODE == FINseq.ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");

                        pgr1.Add(empnameFin + "\n");
                        pgr1.Add(finAppdate);
                        div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        div.Add(pgr1);
                        if (SM_type == 1)
                            div.SetFixedPosition((X - 30), Y - 125, 100); //Changed by TTL on 20-June-2025 against SR100690 > CR6533
                        else
                            div.SetFixedPosition((X - 30), Y - 150, 100); //Changed by TTL on 20-June-2025 against SR100690 > CR6533, by TTL on 04-Sep-2025 against SR104869 > CR6926
                        canvas = new Canvas(page, page.GetPageSize());
                        canvas.Add(div);
                    }
                }
                //// -- End
            }

            document.Close();
            pdfResult.Close();

        }

        public void GetSigLOCATION(string SRCfileName, out Int32 X, out Int32 Y, out Int32 Pageno)
        {
            string path = serverpath.getFileUploadPath("SES_MRN/" + SRCfileName);

            X = 0; Y = 0; Pageno = 0;
            using (var reader = new iTextSharp.text.pdf.PdfReader(path))
            {

                for (int pagen = 1; pagen <= reader.NumberOfPages; pagen++)
                {
                    var parser = new PdfReaderContentParser(reader);

                    var strategy = parser.ProcessContent(pagen, new LocationTextExtractionStrategyWithPosition());
                    var res = strategy.GetAbsoluteLocations();

                    var searchResult = res.Where(p => p.Text.Contains("Inspected By")).ToList();
                    if (searchResult != null && searchResult.Count > 0)
                    {
                        foreach (var obj in searchResult)
                        {
                            X = Convert.ToInt32(obj.Location.StartLocation[0]);
                            Y = Convert.ToInt32(obj.Location.StartLocation[1]);
                            Pageno = pagen;
                            break;
                        }
                    }
                }
                reader.Close();
            }

            // document.Close();
            // pdfResult.Close();

        }

        [HttpPost]
        public ActionResult ReGenerateDoc(string id)
        {
            short retVal = 0;
            string Error = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details =_sessionService.Get<Employee_Details>("Employee");
                string ISSMA = "";
                string ISSMFA = "";

                SMHeaderViewModel SM_Dtl = _smService.GetSMRequestById(Convert.ToInt64(id));
                if (SM_Dtl != null)
                {
                    //var OBJAPP = SM_Dtl.smAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                    //if (OBJAPP == null)
                    {
                         List<SMDetailViewModel> SM_DOCS = SM_Dtl.smDetail.Where(m => m.DOC_TYPE == "SM").ToList();
                        if (SM_Dtl.smDetail.Where(m => m.DOC_TYPE == "SMA").Count() != 0 && SM_DOCS.Count()!= SM_Dtl.smDetail.Where(m => m.DOC_TYPE == "SMA").Count())
                        {
                            List<SMDetailViewModel> SMA_DOCS = SM_Dtl.smDetail.Where(m => m.DOC_TYPE == "SMA").ToList();
                            foreach (var data in SMA_DOCS)
                            {
                                _smService.DeleteAttachment(data.FILENAME, data.DOC_TYPE, data.SMHEADERID);
                                string path = serverpath.getFileUploadPath("SES_MRN/");
                                if (System.IO.File.Exists(System.IO.Path.Combine(path, data.FILENAME)))
                                {
                                    System.IO.File.Delete(System.IO.Path.Combine(path, data.FILENAME));
                                }
                            }
                            ISSMA = "SMA";

                        }
                        if (SM_Dtl.smDetail.Where(m => m.DOC_TYPE == "SMFA").Count() != 0 && SM_DOCS.Count() != SM_Dtl.smDetail.Where(m => m.DOC_TYPE == "SMFA").Count())
                        {
                            List<SMDetailViewModel> SMA_DOCS = SM_Dtl.smDetail.Where(m => m.DOC_TYPE == "SMFA").ToList();
                            foreach (var data in SMA_DOCS)
                            {
                                _smService.DeleteAttachment(data.FILENAME, data.DOC_TYPE, data.SMHEADERID);
                                string path = serverpath.getFileUploadPath("SES_MRN/");
                                if (System.IO.File.Exists(System.IO.Path.Combine(path, data.FILENAME)))
                                {
                                    System.IO.File.Delete(System.IO.Path.Combine(path, data.FILENAME));
                                }
                            }
                            ISSMFA = "SMFA";
                        }
                        foreach (var data in SM_DOCS)
                        {
                            string SM_ATTACHMENT_NAME = data.FILENAME;
                            if (SM_Dtl.PROCESS_STATUS >= 2 && SM_Dtl.PROCESS_STATUS < 9 && ISSMA=="SMA")
                            {
                                string _type = SM_Dtl.SM_TYPE == 1 ? "Service Entry Sheet" : SM_Dtl.SM_TYPE == 2 ? "Material Receipt Note" : "";

                                // Added by Aumento for SR84686 
                                string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                                string path = serverpath.getFileUploadPath("SES_MRN/");
                                if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                                string STRFILENAME = System.IO.Path.Combine(pathtosave, "SM" + data.SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf");
                                // Added by Aumento for SR84686 
                                //string STRFILENAME = "SM" + data.SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";

                                GetDocumentWithAppendedContent(SM_Dtl.smAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), SM_ATTACHMENT_NAME, STRFILENAME, 100, SM_Dtl.smAuthSeq, SM_Dtl.SM_TYPE, SM_Dtl.ADDEDBYNAME, SM_Dtl.DATEADDED);
                                List<SMDetailViewModel> obj = new List<SMDetailViewModel>() { new SMDetailViewModel() { DOC_TYPE = "SMA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "User Mgmt. Approved " + _type } };
                                _smService.SaveAttachment(Convert.ToInt64(_Employee_Details.Employee_Code), SM_Dtl.SMHEADERID, obj);
                                retVal = 1;
                            }
                            if (SM_Dtl.PROCESS_STATUS == 9 && ISSMFA=="SMFA")
                            {
                                string _type = SM_Dtl.SM_TYPE == 1 ? "Service Entry Sheet" : SM_Dtl.SM_TYPE == 2 ? "Material Receipt Note" : "";
                                // Added by Aumento for SR84686 
                                string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                                string path = serverpath.getFileUploadPath("SES_MRN/");
                                if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                                string STRFILENAME = System.IO.Path.Combine(pathtosave, "SMFA" + data.SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf");
                                
                                //string STRFILENAME = "SMFA" + data.SMNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";

                                GetDocumentWithAppendedContent(SM_Dtl.smAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), SM_ATTACHMENT_NAME, STRFILENAME, 100, SM_Dtl.smAuthSeq, SM_Dtl.SM_TYPE, SM_Dtl.ADDEDBYNAME, SM_Dtl.DATEADDED);
                                List<SMDetailViewModel> obj = new List<SMDetailViewModel>() { new SMDetailViewModel() { DOC_TYPE = "SMFA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "Finance Approved Document" + _type } };
                                _smService.SaveAttachment(Convert.ToInt64(_Employee_Details.Employee_Code), SM_Dtl.SMHEADERID, obj);
                                retVal = 1;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Error = ex.InnerException.ToString();
                retVal = -1;
            }
            return Json(new { res = retVal, err_msg = Error });
        }

        [HttpGet]
        public ActionResult SMIOCGMASTER()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SearchIOCGMaster obj = new SearchIOCGMaster();
            List<ADORGLEVEL> _secList = new List<ADORGLEVEL>();
            List<ADORGLEVEL> _depList = new List<ADORGLEVEL>();
            List<ADORGLEVEL> _divList = new List<ADORGLEVEL>();
            List<ADORGLEVEL> _OPList = new List<ADORGLEVEL>();
            #region BIND OPERATION
            _OPList = _PrService.GetOrgLevelList((long)1);
            obj.curr_KI = (long)_OPList.FirstOrDefault().SYKIID;
            //if (obj.DivisionID == 0)
            {
                _OPList = (from data in _OPList
                           select new ADORGLEVEL
                           {
                               LEVELDESCRIP = data.LEVELDESCRIP,
                               ADORGLEVELID = data.ADORGLEVELID
                           }).ToList();
            }
            ViewBag.OPList = new SelectList(_OPList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region BIND DIVISION
            //_divList = _PrService.GetOrgLevelList((long)2);
            List<ADORGLEVEL> _DivList = new List<ADORGLEVEL>();
            //if (obj.DivisionID == 0)
            //{
            //    _DivList = (from data in _divList
            //                select new ADORGLEVEL
            //                {
            //                    LEVELDESCRIP = data.LEVELDESCRIP,
            //                    ADORGLEVELID = data.ADORGLEVELID
            //                }).ToList();
            //}
            ViewBag.DivList = new SelectList(_DivList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region BIND DEPARTMENT
            //_depList = _PrService.GetOrgLevelList((long)3);
            List<ADORGLEVEL> _DptList = new List<ADORGLEVEL>();
            //if (obj.DEPTID == 0)
            {
                //_DptList = (from data in _depList
                //            select new ADORGLEVEL
                //            {
                //                LEVELDESCRIP = data.LEVELDESCRIP,
                //                ADORGLEVELID = data.ADORGLEVELID
                //            }).ToList();
            }

            ViewBag.DepList = new SelectList(_DptList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region BIND SECTION
            //_secList = _PrService.GetOrgLevelList((long)4);
            List<ADORGLEVEL> _SCList = new List<ADORGLEVEL>();
            //if (obj.SECID == 0)
            {
                //_SCList = (from data in _secList
                //           select new ADORGLEVEL
                //           {
                //               LEVELDESCRIP = data.LEVELDESCRIP,
                //               ADORGLEVELID = data.ADORGLEVELID
                //           }).ToList();
            }
            ViewBag.SecList = new SelectList(_SCList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region "Bind KI"

            var objki = _smService.BindKI();
            ViewBag.KIList = new SelectList(objki, "Value", "Text");
            #endregion

            return View(obj);
        }
        public ActionResult BindDivisionByOperationId(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<ADORGLEVEL> divList = new List<ADORGLEVEL>();
            if (id == 0)
            {
                List<ADORGLEVEL> DivList = _PrService.GetOrgLevelList((long)2);
                if (DivList.Count > 0)
                {
                    divList = (from data in DivList
                               select new ADORGLEVEL
                               {
                                   LEVELDESCRIP = data.LEVELDESCRIP,
                                   ADORGLEVELID = data.ADORGLEVELID
                               }).ToList();
                }

            }
            else
            {
                var _ilist = _PrService.BindDivision(id);
                divList = (from data in _ilist
                           select new ADORGLEVEL
                           {
                               LEVELDESCRIP = data.Text,
                               ADORGLEVELID = data.Value
                           }).ToList();

            }
            return Json(divList);
        }

        public ActionResult BindDeptByDivisionId(long opid, long divid)
        {
            List<PR_Div_Dep_SecViewModel> depList = _smService.BindDepartment(divid, opid);
            var _depList = (from data in depList
                            select new ADORGLEVEL
                            {
                                LEVELDESCRIP = data.Text,
                                ADORGLEVELID = data.Value
                            }).ToList();
            return Json(_depList);
        }

        public ActionResult BindSecByDepartmentId(long opid, long divid, long deptid)
        {
            List<PR_Div_Dep_SecViewModel> SecList = _smService.BindSection(deptid, divid, opid);
            var _SecList = (from data in SecList
                            select new ADORGLEVEL
                            {
                                LEVELDESCRIP = data.Text,
                                ADORGLEVELID = data.Value
                            }).ToList();
            return Json(_SecList);
        }

        [HttpPost]
        public ActionResult SMIOCGMASTER([FromBody]SearchIOCGMaster _sm)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            _smService.BindIOCGMasterList(_sm);
            ViewBag.CURR_KI = _sm.curr_KI;
            return PartialView("_SMIOCGMasterList", _sm.SearchResult);
        }
        [HttpPost]
        public ActionResult EditIOCGMaster(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            VM_VW_SMIOCGAPPROVER_LIST obj = new VM_VW_SMIOCGAPPROVER_LIST();
            List<ADORGLEVEL> _secList = new List<ADORGLEVEL>();
            List<ADORGLEVEL> _depList = new List<ADORGLEVEL>();
            List<ADORGLEVEL> _divList = new List<ADORGLEVEL>();
            List<ADORGLEVEL> _OPList = new List<ADORGLEVEL>();
            #region BIND OPERATION
            _OPList = _PrService.GetOrgLevelList((long)1);
            //if (obj.DivisionID == 0)
            {
                _OPList = (from data in _OPList
                           select new ADORGLEVEL
                           {
                               LEVELDESCRIP = data.LEVELDESCRIP,
                               ADORGLEVELID = data.ADORGLEVELID
                           }).ToList();
            }
            ViewBag.OPList = new SelectList(_OPList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region BIND DIVISION
            _divList = _PrService.GetOrgLevelList((long)2);
            List<ADORGLEVEL> _DivList = new List<ADORGLEVEL>();
            //if (obj.DivisionID == 0)
            {
                _DivList = (from data in _divList
                            select new ADORGLEVEL
                            {
                                LEVELDESCRIP = data.LEVELDESCRIP,
                                ADORGLEVELID = data.ADORGLEVELID
                            }).ToList();
            }
            ViewBag.DivList = new SelectList(_DivList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region BIND DEPARTMENT
            _depList = _PrService.GetOrgLevelList((long)3);
            List<ADORGLEVEL> _DptList = new List<ADORGLEVEL>();
            //if (obj.DEPTID == 0)
            {
                _DptList = (from data in _depList
                            select new ADORGLEVEL
                            {
                                LEVELDESCRIP = data.LEVELDESCRIP,
                                ADORGLEVELID = data.ADORGLEVELID
                            }).ToList();
            }

            ViewBag.DepList = new SelectList(_DptList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region BIND SECTION
            _secList = _PrService.GetOrgLevelList((long)4);
            List<ADORGLEVEL> _SCList = new List<ADORGLEVEL>();
            //if (obj.SECID == 0)
            {
                _SCList = (from data in _secList
                           select new ADORGLEVEL
                           {
                               LEVELDESCRIP = data.LEVELDESCRIP,
                               ADORGLEVELID = data.ADORGLEVELID
                           }).ToList();
            }
            ViewBag.SecList = new SelectList(_SCList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region "Bind KI"
            var objki = _smService.BindKI();
            ViewBag.KIList = new SelectList(objki, "Value", "Text");
            #endregion

            #region "Bind Status"
            List<SelectListItem> iStatusLst = new List<SelectListItem>();
            iStatusLst.Add(new SelectListItem() { Text = "Active", Value = "1" });
            iStatusLst.Add(new SelectListItem() { Text = "Deactive", Value = "0" });
            ViewBag.StatusList = new SelectList(iStatusLst, "Value", "Text");
            #endregion
            #region "Bind Type"
            List<SelectListItem> iTypeLst = new List<SelectListItem>();
            iTypeLst.Add(new SelectListItem() { Text = "Service Entry Sheet", Value = "1" });
            iTypeLst.Add(new SelectListItem() { Text = "Material Receipt Note", Value = "2" });
            ViewBag.TypeList = new SelectList(iTypeLst, "Value", "Text");
            #endregion

            if (id > 0)
            {
                obj.SMMAP_MSTID = id;
                obj = _smService.GetIOCGMasterByID(obj).FirstOrDefault();
            }
            return PartialView("_EditIOCGMaster", obj);
        }

        [HttpPost]
        public ActionResult SaveIOCGMaster()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            VM_VW_SMIOCGAPPROVER_LIST obj = new VM_VW_SMIOCGAPPROVER_LIST();
            List<ADORGLEVEL> _secList = new List<ADORGLEVEL>();
            List<ADORGLEVEL> _depList = new List<ADORGLEVEL>();
            List<ADORGLEVEL> _divList = new List<ADORGLEVEL>();
            List<ADORGLEVEL> _OPList = new List<ADORGLEVEL>();
            #region BIND OPERATION
            _OPList = _PrService.GetOrgLevelList((long)1);
            //if (obj.DivisionID == 0)
            {
                _OPList = (from data in _OPList
                           select new ADORGLEVEL
                           {
                               LEVELDESCRIP = data.LEVELDESCRIP,
                               ADORGLEVELID = data.ADORGLEVELID
                           }).ToList();
            }
            ViewBag.OPList = new SelectList(_OPList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region BIND DIVISION
            _divList = _PrService.GetOrgLevelList((long)2);
            List<ADORGLEVEL> _DivList = new List<ADORGLEVEL>();
            ////if (obj.DivisionID == 0)
            //{
            //    _DivList = (from data in _divList
            //                select new ADORGLEVEL
            //                {
            //                    LEVELDESCRIP = data.LEVELDESCRIP,
            //                    ADORGLEVELID = data.ADORGLEVELID
            //                }).ToList();
            //}
            ViewBag.DivList = new SelectList(_DivList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region BIND DEPARTMENT
            _depList = _PrService.GetOrgLevelList((long)3);
            List<ADORGLEVEL> _DptList = new List<ADORGLEVEL>();
            //if (obj.DEPTID == 0)
            //{
            //    _DptList = (from data in _depList
            //                select new ADORGLEVEL
            //                {
            //                    LEVELDESCRIP = data.LEVELDESCRIP,
            //                    ADORGLEVELID = data.ADORGLEVELID
            //                }).ToList();
            //}

            ViewBag.DepList = new SelectList(_DptList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region BIND SECTION
            _secList = _PrService.GetOrgLevelList((long)4);
            List<ADORGLEVEL> _SCList = new List<ADORGLEVEL>();
            //if (obj.SECID == 0)
            //{
            //    _SCList = (from data in _secList
            //               select new ADORGLEVEL
            //               {
            //                   LEVELDESCRIP = data.LEVELDESCRIP,
            //                   ADORGLEVELID = data.ADORGLEVELID
            //               }).ToList();
            //}
            ViewBag.SecList = new SelectList(_SCList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
            #endregion

            #region "Bind Status"
            //List<SelectListItem> iStatusLst = new List<SelectListItem>();
            //iStatusLst.Add(new SelectListItem() { Text = "Active", Value = "1" });
            //iStatusLst.Add(new SelectListItem() { Text = "Deactive", Value = "0" });
            //ViewBag.StatusList = new SelectList(iStatusLst, "Value", "Text");
            #endregion

            #region "Bind Type"
            List<SelectListItem> iTypeLst = new List<SelectListItem>();
            iTypeLst.Add(new SelectListItem() { Text = "Service Entry Sheet", Value = "1" });
            iTypeLst.Add(new SelectListItem() { Text = "Material Receipt Note", Value = "2" });
            ViewBag.TypeList = new SelectList(iTypeLst, "Value", "Text");
            #endregion

            obj.APP1_NAME = "";
            obj.APP2_NAME = "";
            obj.STARTAMOUNT = 1;
            obj.ENDAMOUNT = 99999999999;
            return PartialView("_SaveIOCGMaster", obj);
        }

        [HttpPost]
        public ActionResult SaveIOCGMasterData([FromBody]VM_VW_SMIOCGAPPROVER_LIST obj)
        {
            short retVal = 0;
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            obj.UPDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            Tuple<short, string> retval = _smService.SaveIOCGMasterData(obj);
            return Json(new { res = retval.Item1, err_msg = retval.Item2 });
        }
        [HttpPost]
        public ActionResult EditIOCGMasterData([FromBody]VM_VW_SMIOCGAPPROVER_LIST obj)
        {
            short retVal = 0;
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            obj.UPDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            Tuple<short, string> retval = _smService.EditIOCGMasterData(obj);
            return Json(new { res = retval.Item1, err_msg = retval.Item2 });
        }

        //Below added by aumento for the SR50703 on 18052023==============================================================================

        [HttpPost]
        public JsonResult GetOrganization(long ddOLvType)
        {
            Tuple<short, List<A00ADORGLEVELList>> _ret_tuple = _smService.PortalAutocompleteSuggestionsForORG_New(ddOLvType);
            return Json(_ret_tuple);
        }

        public ActionResult SaveAdorglevel(string ddOLvType, string Organization)
        {
            short retVal = 0;
            try
            {
                Tuple<short, long> retVal_tuple = _smService.SaveAdorglevel(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), ddOLvType, Organization);
                retVal = retVal_tuple.Item1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal });
        }

        [HttpPost]
        public JsonResult GetSMIOCGBLOCKData()
        {
            int i = 0;
            List<SMDGIT_SMIOCGBLOCKViewModel> POAPowerList = new List<SMDGIT_SMIOCGBLOCKViewModel>();
            POAPowerList = _smService.GetSMIOCGBLOCKData();
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = JsonSerializer.Serialize(POAPowerList);
            return Json(sJSON);
            
        }

        [HttpPost]
        public ActionResult UpdateStatus(long SMIOCGBLOCKID, long ADORGLEVELID, long SRNO)
        {
            Tuple<short, List<SMDGIT_SMIOCGBLOCKViewModel>> _ret_tuple = _smService.UpdateStatus(SMIOCGBLOCKID, ADORGLEVELID, SRNO);
            return Json(new { res = 1, attachmentList = _ret_tuple });
            
        }
        public ActionResult SMIOCGBlockMaster()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View("SMIOCGBlockMaster");

        }

        //================================================================================================================================
        [HttpGet]
        public ActionResult SMIPDashboard()
        {
            SearchSMViewModel obj = new SearchSMViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails =_sessionService.Get<Employee_Details>("Employee");
                List<ADORGLEVEL> _secList = new List<ADORGLEVEL>();
                List<ADORGLEVEL> _depList = new List<ADORGLEVEL>();
                int IsUserOP = 0;
                //List<ADORGLEVEL> _opList = _PrService.BindOperationForOPMap(employeeDetails._ECode);
                List<ADORGLEVEL> _opList = _PrService.GetOrgLevelList((long)1);
                //if (_opList.Count == 0)
                //{
                //    _opList.Add(new ADORGLEVEL
                //    {
                //        ADORGLEVELID = Convert.ToInt64(employeeDetails._OpId),
                //        LEVELDESCRIP = employeeDetails._OpDesc.ToString()
                //    });
                //    obj.OperationID = employeeDetails._OpId == null ? 0 : (long)employeeDetails._OpId;
                //    obj.DivisionID = employeeDetails._DivId == null ? 0 : (long)employeeDetails._DivId;
                //    obj.DEPTID = employeeDetails._DepId == null ? 0 : (long)employeeDetails._DepId;
                //    obj.SECID = employeeDetails._SecId == null ? 0 : (long)employeeDetails._SecId;
                //    _depList = _PrService.GetOrgLevelList((long)3);
                //    _secList = _PrService.GetOrgLevelList((long)4);

                //    IsUserOP = 1;
                //}
                //else
                //{
                //    obj.OperationID = _opList.Select(s => s.ADORGLEVELID).FirstOrDefault();
                //}
                //if (_opList.Count > 0)
                //{
                //    obj.OperationID = Convert.ToInt64(employeeDetails._OpId);
                //}
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");

                List<ADORGLEVEL> _divList = _PrService.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "ADORGLEVELID", "LEVELDESCRIP");

                ViewBag.DepList = new SelectList(_depList, "ADORGLEVELID", "LEVELDESCRIP");

                ViewBag.SecList = new SelectList(_secList, "ADORGLEVELID", "LEVELDESCRIP");

                ViewBag.ISUSER_OPERATION = IsUserOP;

                obj.Doc_Status = -1; /// --- -All-
                obj.ReqStatus = 0; /// --- -Pending-

                var objki = _smService.BindKI();
                ViewBag.KIList = new SelectList(objki.OrderByDescending(m => m.Value), "Value", "Text");
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult SMIPDashboard([FromBody]SearchSMViewModel SSM)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details employeeDetails =_sessionService.Get<Employee_Details>("Employee");
            SearchSMViewModel _headerObj = _smService.SMIPDashboard(SSM, Convert.ToInt64(employeeDetails._PlantId));
            return PartialView("_IPDashboardList", _headerObj.SearchResult);
        }
    }
    public class SMLocationTextExtractionStrategyWithPosition : LocationTextExtractionStrategy
    {

        private readonly List<TextChunk> locationalResult = new List<TextChunk>();

        private readonly ITextChunkLocationStrategy tclStrat;

        public SMLocationTextExtractionStrategyWithPosition() : this(new TextChunkLocationStrategyDefaultImp())
        {
        }

        /**
         * Creates a new text extraction renderer, with a custom strategy for
         * creating new TextChunkLocation objects based on the input of the
         * TextRenderInfo.
         * @param strat the custom strategy
         */
        public SMLocationTextExtractionStrategyWithPosition(ITextChunkLocationStrategy strat)
        {
            tclStrat = strat;
        }


        private bool StartsWithSpace(string str)
        {
            if (str.Length == 0) return false;
            return str[0] == ' ';
        }


        private bool EndsWithSpace(string str)
        {
            if (str.Length == 0) return false;
            return str[str.Length - 1] == ' ';
        }

        /**
         * Filters the provided list with the provided filter
         * @param textChunks a list of all TextChunks that this strategy found during processing
         * @param filter the filter to apply.  If null, filtering will be skipped.
         * @return the filtered list
         * @since 5.3.3
         */

        private List<TextChunk> filterTextChunks(List<TextChunk> textChunks, ITextChunkFilter filter)
        {
            if (filter == null)
            {
                return textChunks;
            }

            var filtered = new List<TextChunk>();

            foreach (var textChunk in textChunks)
            {
                if (filter.Accept(textChunk))
                {
                    filtered.Add(textChunk);
                }
            }

            return filtered;
        }

        public override void RenderText(TextRenderInfo renderInfo)
        {
            LineSegment segment = renderInfo.GetBaseline();
            if (renderInfo.GetRise() != 0)
            { // remove the rise from the baseline - we do this because the text from a super/subscript render operations should probably be considered as part of the baseline of the text the super/sub is relative to 
                Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
                segment = segment.TransformBy(riseOffsetTransform);
            }
            TextChunk tc = new TextChunk(renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment));
            locationalResult.Add(tc);
        }


        public IList<TextLocation> GetLocations()
        {

            var filteredTextChunks = filterTextChunks(locationalResult, null);
            filteredTextChunks.Sort();

            TextChunk lastChunk = null;

            var textLocations = new List<TextLocation>();

            foreach (var chunk in filteredTextChunks)
            {

                if (lastChunk == null)
                {
                    //initial
                    textLocations.Add(new TextLocation
                    {
                        Text = chunk.Text,
                        X = iTextSharp.text.Utilities.PointsToMillimeters(chunk.Location.StartLocation[0]),
                        Y = iTextSharp.text.Utilities.PointsToMillimeters(chunk.Location.StartLocation[1])
                    });

                }
                else
                {
                    if (chunk.SameLine(lastChunk))
                    {
                        var text = "";
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if (IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.Text) && !EndsWithSpace(lastChunk.Text))
                            text += ' ';

                        text += chunk.Text;

                        textLocations[textLocations.Count - 1].Text += text;

                    }
                    else
                    {

                        textLocations.Add(new TextLocation
                        {
                            Text = chunk.Text,
                            X = iTextSharp.text.Utilities.PointsToMillimeters(chunk.Location.StartLocation[0]),
                            Y = iTextSharp.text.Utilities.PointsToMillimeters(chunk.Location.StartLocation[1])
                        });
                    }
                }
                lastChunk = chunk;
            }

            //now find the location(s) with the given texts
            return textLocations;

        }

        public IList<LocationTextExtractionStrategy.TextChunk> GetAbsoluteLocations()
        {

            var filteredTextChunks = filterTextChunks(locationalResult, null);
            filteredTextChunks.Sort();


            return filteredTextChunks;

        }
        

    }

    public class SMTextLocation
    {
        public float X { get; set; }
        public float Y { get; set; }

        public string Text { get; set; }
    }


}