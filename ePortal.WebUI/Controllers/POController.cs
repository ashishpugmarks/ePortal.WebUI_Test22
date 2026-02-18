using ePortal.ViewModels;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.IO;
using ePortal.Application.Contracts;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using iText.Layout.Element;
using iTextSharp.text.pdf.parser;
using System.Text;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Data;
using System.Net;
using Microsoft.AspNetCore.StaticFiles;
using Path = System.IO.Path;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;



namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class POController : Controller
    {
        private readonly IPOService _POService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<POController> _logger;
        private readonly IEportalESS objess;
        private readonly IAppConfigurationService _env;
        //public POController(IPOService POService)
        //{
        //    _POService = POService;
        //}
        public POController(ILogger<POController> logger, IPOService poservice, ISessionService sessionService, IAppConfigurationService appConfiguration, IEportalESS _objess)
        {
            _logger = logger;
            _POService = poservice;
            _sessionService = sessionService;
            objess = _objess;
            _env = appConfiguration;
        }
        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        [OutputCache(Duration = 3600, VaryByQueryKeys = new[] { "*" })]
        public async Task<POHeaderViewModel> GetPODetailFromSap(string _poNo)
        {
            ///------ SAP Connection Object -------////
            //EportalESS objasset = new EportalESS();
            POHeaderViewModel poObj = new POHeaderViewModel();
            List<POMappingViewModel> mappingList = new List<POMappingViewModel>();
            DataSet ds = new DataSet();
            ds = await objess.GetPOGetails(_poNo); //
            if (ds.Tables[0].Rows.Count > 0)
            {
                poObj = (from DataRow row in ds.Tables[0].AsEnumerable()
                         select new POHeaderViewModel
                         {
                             PONO = row["EBELN"].ToString(),
                             PO_DESC = row["BSART"].ToString(),
                             //STATUS = row["STATU"].ToString(),
                             VENDORID = row["LIFNR"].ToString(),
                             VENDORNAME = row["NAME1"].ToString(),
                             VENDORMAILID = row["SMTP_ADDR"].ToString(),
                             VERSIONNO = (string.IsNullOrEmpty(row["REVNO"].ToString()) ? (short)0 : Convert.ToInt16(row["REVNO"].ToString())),
                         }).FirstOrDefault();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                mappingList = (from DataRow row in ds.Tables[1].AsEnumerable()
                               select new POMappingViewModel
                               {
                                   PONO = row["EBELN"].ToString(),
                                   PRNO = row["BANFN"].ToString(),
                                   //STATUS = row["BANFN"].ToString(),
                                   //VENDORID = row["BNFPO"].ToString(),
                                   //VENDORNAME = row["ZCONTRACT_NO"].ToString(),
                               }).ToList();
                poObj.poMapping = mappingList.ToList();
                poObj.Plant = ds.Tables[1].Rows[0]["WERKS"].ToString();

            }
            string _venderCode = !string.IsNullOrEmpty(poObj.VENDORID) ? (poObj.VENDORID[0] == '0' ? poObj.VENDORID.Remove(0, 1) : poObj.VENDORID) : "";
            poObj.VENDORID = !string.IsNullOrEmpty(_venderCode) ? (_venderCode[0] == '0' ? _venderCode.Remove(0, 1) : _venderCode) : "";

            return poObj;
        }

        public ActionResult POViewDetail(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            return View("POViewDetail", _POService.GetPORequestById(_ReqId));
        }

        [HttpGet]
        public ActionResult PORequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<POAppAuthSeqViewModel> iList = new List<POAppAuthSeqViewModel>();
            if (TempData["APPROVAL_AUTH_LIST"] == null)
            {
                iList = _POService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            else
            {
                //iList = (List<POAppAuthSeqViewModel>)TempData["APPROVAL_AUTH_LIST"];
                iList = JsonSerializer.Deserialize<List<POAppAuthSeqViewModel>>(TempData["APPROVAL_AUTH_LIST"].ToString());
            }
            TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(iList);
            return View();
        }

        [HttpPost]
        public ActionResult PORequest([FromBody] POHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.poDetail = new List<PODetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                Tuple<short, long> retVal_tuple = _POService.SavePORequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public async Task<IActionResult> GetPODetail(string poNo)
        {
            POHeaderViewModel poDetail = new POHeaderViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                poDetail = await GetPODetailFromSap(poNo); ////"6500002191"
            }
            catch (Exception ex)
            {
                poDetail = null;
            }
            return Json(poDetail);
        }

        [HttpPost]
        public ActionResult SavePODetail([FromBody] POHeaderViewModel PHVM)
        {
            short retVal = 0; long hearderId = 0;
            string errmsg = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (PHVM.poMapping != null)
                {
                    if (PHVM.poMapping.Select(m => m.PRNO).Count() > 0)
                    {
                        long objprstatus = _POService.GetPRStatusByPOId(PHVM.poMapping.Select(m => m.PRNO).ToArray());
                        if (objprstatus == 1)
                        {
                            return Json(new { res = 5, poId = 0 });
                        }
                    }
                }

                PHVM.poDetail = new List<PODetailViewModel>();
                PHVM.poAuthSeq = new List<POAppAuthSeqViewModel>();
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                PHVM.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                PODetailViewModel model = new PODetailViewModel();

                if (TempData["PO_ATTACHMENT"] != null)
                {
                    //model = (PODetailViewModel)TempData["PO_ATTACHMENT"];
                    var poJson = TempData["PO_ATTACHMENT"] as string;
                    model = JsonSerializer.Deserialize<PODetailViewModel>(poJson);
                    PHVM.poDetail.Add(model);
                }
                string pathtemp = _env.GetGeneralSettings().Get_FileUpload_Path+"PO/Temp/";                
                if (!Directory.Exists(pathtemp)) { Directory.CreateDirectory(pathtemp); }
                if (PHVM.poDetail.Count > 0)
                {
                    foreach (PODetailViewModel obj in PHVM.poDetail)
                    {
                        if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                        {
                            // Added by Aumento for SR84686 start
                            string filePath = obj.FILENAME;
                            string fileName = System.IO.Path.GetFileName(filePath);
                            // Added by Aumento for SR84686 end
                            //System.IO.File.WriteAllBytes(pathtemp + obj.FILENAME, obj.FILE_BYTE.ToArray());
                            System.IO.File.WriteAllBytes(pathtemp + fileName, obj.FILE_BYTE.ToArray());
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
                    GetSigLOCATION("Temp/" + srcfile, out x, out y, out pageno); // Added by Aumento for SR84686
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

                Tuple<short, long> retVal_tuple = _POService.SavePORequest(PHVM);
                retVal = retVal_tuple.Item1;
                hearderId = retVal_tuple.Item2;
                if (retVal == 1)
                {
                    //string path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/";
                    string path = serverpath.getFileUploadPath("PO/");
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                    if (PHVM.poDetail.Count > 0)
                    {
                        foreach (PODetailViewModel obj in PHVM.poDetail)
                        {
                            if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                            {
                                System.IO.File.WriteAllBytes(path + obj.FILENAME, obj.FILE_BYTE.ToArray());
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                retVal = -1;
                errmsg = ex.Message;


            }
            return Json(new { res = retVal, poId = hearderId, err = errmsg });
        }

        [HttpPost]
        //public ActionResult UploadPO(IFormFile FILE, string DOC_TYPE, string PONo)
        public IActionResult UploadPO(IFormFile FILE, string DOC_TYPE, string PONo)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PODetailViewModel POAtt = new PODetailViewModel();
                if (FILE.Length > 0 && !string.IsNullOrEmpty(DOC_TYPE))
                {
                    // Added by Aumento for SR84686 start
                    //string path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/";
                    string path = serverpath.getFileUploadPath("PO/");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    // Added by Aumento for SR84686 end
                    FileViewModel _file = GetUploadFile(FILE, DOC_TYPE, PONo);
                    //POAtt.FILENAME = _file.FileName;
                    POAtt.FILENAME = pathtosave + @"\" + _file.FileName; // Added by Aumento for SR84686
                    POAtt.FILE_CONTENTTYPE = _file.FileContentType;
                    POAtt.FILE_BYTE = _file.File;
                    POAtt.DOC_TYPE = DOC_TYPE;
                    //TempData["PO_ATTACHMENT"] = POAtt;
                    TempData["PO_ATTACHMENT"] = JsonSerializer.Serialize(POAtt);
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal });

            //return new JsonResult(retVal);

        }

        [HttpPost]
        public ActionResult UploadAttachment([FromForm] PODetailViewModel formData)
        {
            short retVal = 0;
            List<PODetailViewModel> poDtlList = new List<PODetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.POHEADERID > 0)
                {
                    // Added by Aumento for SR84686 start
                    //string path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/";
                    string path = serverpath.getFileUploadPath("PO/");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    // Added by Aumento for SR84686 end
                    FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.PONo);
                    poDtlList.Add(new PODetailViewModel
                    {
                        POHEADERID = formData.POHEADERID,
                        //FILENAME = _file.FileName,
                        FILENAME = pathtosave + @"\" + _file.FileName, // Added by Aumento for SR84686
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = formData.DOC_TYPE,
                        ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID")),
                    });
                    Tuple<short, List<PODetailViewModel>> _ret_tuple = _POService.SaveAttachment(Convert.ToInt64(_sessionService.Get<string>("userID")), formData.POHEADERID, poDtlList);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        //string path = Server.MapPath("~/Uploads/PO/"); // comment by Aumento for SR84686
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (poDtlList.Count > 0)
                        {
                            foreach (PODetailViewModel obj in poDtlList)
                            {
                                if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                {
                                    System.IO.File.WriteAllBytes(path + obj.FILENAME, obj.FILE_BYTE.ToArray());
                                }
                            }
                        }
                    }
                    poDtlList = _ret_tuple.Item2;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.DOC_TYPE != "PO").ToList() });
        }

        [HttpDelete]
        //public ActionResult DeleteAttachment(string fileName, string docType, long poHeaderId)
        public IActionResult DeleteAttachment([FromBody] PODetailViewModel request)
        {
            short retVal = 0;
            List<PODetailViewModel> poDtlList = new List<PODetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string fileName = request.FILENAME;
                string docType = request.DOC_TYPE;
                long poHeaderId = request.POHEADERID;

                Tuple<short, List<PODetailViewModel>> _ret_tuple = _POService.DeleteAttachment(fileName, docType, poHeaderId);
                retVal = _ret_tuple.Item1;
                poDtlList = _ret_tuple.Item2;
                if (retVal == 1)
                {
                    //string path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/";
                    string path = serverpath.getFileUploadPath("PO/");
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
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.IsDeleted == 0 && w.DOC_TYPE != "PO").ToList() });
        }

        [HttpGet]
        public ActionResult PreviewPODetail(long poHeaderId)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                POHeaderViewModel PHVM = _POService.GetPORequestById(poHeaderId);
                return PartialView("_PreviewPODetail", PHVM);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpGet]
        public ActionResult EditPORequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            POHeaderViewModel PHVM = _POService.GetPORequestById(_ReqId);
            // added by aumento for SR82542
            //if (PHVM.PROCESS_STATUS != 0)
            //{
            //if (PHVM.poAuthSeq.Count == 0)
            //{
            //PHVM.poAuthSeq = _POService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID")));

            List<POAppAuthSeqViewModel> poAuthSeq = _POService.GetPOAuthorityHis_ById(_ReqId, Convert.ToInt64(_sessionService.Get<string>("userID")));

            PHVM.poAuthSeq = poAuthSeq.Count > 0 ? poAuthSeq : _POService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID")));


            // }
            //}
            // added by aumento for SR82542
            ViewBag.strId = id;
            return View(PHVM);
        }

        [HttpPost]
        public ActionResult EditPORequest(POHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.poDetail = new List<PODetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));

                Tuple<short, long> retVal_tuple = _POService.SavePORequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult POApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            //long _ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64((id));
            POHeaderViewModel obj = _POService.GetPORequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID"));
            obj.ISENABLE = obj.poAppHis.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0 && a.POAPPHISTORY_ID != 0).Count().ToString();
            ViewBag.FnDesigId = _Employee_Details._FnDesigId;
            return View("POApproval", obj);

        }

        [HttpPost]
        public ActionResult POApproval([FromBody] POAppHistoryViewModel PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));

                retVal = _POService.POApproval(PHVM, _Employee_Details);
                #region "Document Generation"
                //if (retVal == 1)
                //{
                //    POHeaderViewModel PO_Dtl = _POService.GetPORequestById(PHVM.POID);
                //    if (PO_Dtl != null)
                //    {
                //        var OBJAPP = PO_Dtl.poAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                //        var APPTYPE = PO_Dtl.poAuthSeq.Where(x => x.ADEMPCODE == OBJAPP.ADEMPCODE).FirstOrDefault().APPTYPE;
                //        //if digital sign required than comment OBJAPP if condition & un-comment if condition APPTYPE=null
                //        //if (OBJAPP== null)
                //        //{
                //        if (APPTYPE == 2)
                //        {
                //            if (PHVM.APPROVAL_STATUS == 1)
                //            {
                //                // Added by Aumento for SR84686
                //                string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                //                string path = Server.MapPath("~/Uploads/PO/");
                //                if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                //                string STRFILENAME = System.IO.Path.Combine(pathtosave, "PO" + PO_Dtl.PONO + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf");
                //                // Added by Aumento for SR84686 aumento
                //                //string STRFILENAME = "PO" + PO_Dtl.PONO + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";

                //                GetDocumentWithAppendedContent(PO_Dtl.poAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), PHVM.PO_ATTACHMENT_NAME, STRFILENAME, 100, PO_Dtl.poAuthSeq);
                //                List<PODetailViewModel> obj = new List<PODetailViewModel>() { new PODetailViewModel() { DOC_TYPE = "POA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "Approved PO" } };
                //                _POService.SaveAttachment(PHVM.ADDEDBY, PHVM.POID, obj);
                //            }
                //            //  }
                //        }
                //    }
                //}
                #endregion

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult POCancel(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                long _ReqId = Convert.ToInt64(Encryption.Decrypt(WebUtility.UrlDecode(id)));//Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
                return View("POCancel", _POService.GetPORequestById(_ReqId));
            }
            catch
            {
                long _ReqId = Convert.ToInt64(Encryption.Decrypt(id));//Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
                return View("POCancel", _POService.GetPORequestById(_ReqId));
            }
        }

        [HttpPost]
        public ActionResult POCancel([FromBody] POHeaderViewModel PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                PHVM.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                retVal = _POService.POCancel(PHVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult GetAppAuthority(string eCode)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details employee_dtl = new Employee_Details();

            //Below added by aumento as on 02092024 for SR70820================================================================
            employee_dtl = _POService.GetAuthEmpById(Convert.ToInt64(eCode), Convert.ToInt64(_sessionService.Get<string>("userID")));
            //=================================================================================================================

            if (employee_dtl == null) { employee_dtl = new Employee_Details(); }
            return Json(new
            {
                ECODE = employee_dtl._ECode,
                ENAME = employee_dtl._EName,
                DESIGNATION = employee_dtl._Desig,
                APPTYPE = 1,
                FNDESID = employee_dtl._FnDesigId
            });
        }


        private FileViewModel GetUploadFile(IFormFile file, string DocType, string PONo)
        {
            try
            {
                FileViewModel FVM = new FileViewModel();

                if (file != null && file.Length > 0)
                {
                    byte[] bytes;
                    using (var stream = file.OpenReadStream())
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }

                    string _FileName = Path.GetFileName(file.FileName); // This is cross-platform safe
                    string contentType;

                    // Use FileExtensionContentTypeProvider to get MIME type
                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(_FileName, out contentType))
                    {
                        contentType = "application/octet-stream"; // fallback
                    }

                    FVM.FileContentType = contentType;
                    FVM.FileName = $"{DocType}_{PONo}_{DateTime.Now:ddMMyyHHmmss}.pdf";
                    FVM.File = bytes;
                }

                return FVM;
            }
            catch (Exception)
            {
                throw; // Preserves original stack trace
            }

            //try
            //{
            //    FileViewModel FVM = new FileViewModel();
            //    if (file != null && file.Length > 0)
            //    {
            //        byte[] bytes;
            //        using (BinaryReader br = new BinaryReader(file.InputStream))
            //        {
            //            bytes = br.ReadBytes(file.Length);
            //        }
            //        string _FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
            //        FVM.FileContentType = MimeMapping.GetMimeMapping(_FileName);
            //        //FVM.FileName = DocType + "_" + PONo + ".pdf";
            //        FVM.FileName = DocType + "_" + PONo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
            //        FVM.File = bytes;
            //    }
            //    return FVM;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        public ActionResult POMultiDocsView(long id)
        {
            POHeaderViewModel obj = _POService.GetPORequestById(id);
            PRHeaderViewModel poDetail = new PRHeaderViewModel();
            PODetailViewModel item = new PODetailViewModel();
            item.FILENAME = "Select";
            obj.poDetail.Insert(0, item);
            if (obj.POHEADERID != 0)
            {
                poDetail = _POService.GetPRDetailByPOId(obj.POHEADERID);
                if (poDetail != null)
                {
                    foreach (var data in poDetail.prDetail)
                    {
                        PODetailViewModel item1 = new PODetailViewModel();
                        item1.FILENAME = "SD_" + data.FILENAME;
                        obj.poDetail.Add(item1);
                    }
                }
            }

            return View("POMultiDocsView", obj.poDetail);
        }
        public ActionResult POTwoDocsView(long id)
        {
            POHeaderViewModel obj = _POService.GetPORequestById(id);
            PRHeaderViewModel poDetail = new PRHeaderViewModel();
            PODetailViewModel item = new PODetailViewModel();
            item.FILENAME = "Select";
            obj.poDetail.Insert(0, item);

            poDetail = _POService.GetPRDetailByPOId(obj.POHEADERID);
            if (poDetail != null)
            {
                foreach (var data in poDetail.prDetail)
                {
                    PODetailViewModel item1 = new PODetailViewModel();
                    item1.FILENAME = "SD_" + data.FILENAME;
                    obj.poDetail.Add(item1);
                }
            }

            return View("POTwoDocsView", obj.poDetail);
        }
        public ActionResult GetPDF(string fileName)
        {
            try
            {
                //string file_path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/"; //// Server.MapPath("~/Uploads/PO/");
                string file_path = "../../../Uploads/PO/";
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
        public ActionResult GetTwoPDF(string fileName)
        {
            string type = fileName.Substring(0, 2);
            if (type != "SD")
            {
                try
                {
                    //string file_path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/"; //// Server.MapPath("~/Uploads/PO/");
                    string file_path = "../../../Uploads/PO/";
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
                    // string file_path = "../../../Uploads/PR/"; //// Server.MapPath("~/Uploads/PO/");
                    //string file_path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PR/"; //// Server.MapPath("~/Uploads/PO/");
                    string file_path = "../../../Uploads/PR/";
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
                    //string file_path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/"; //// Server.MapPath("~/Uploads/PO/");
                    string file_path = "../../../Uploads/PO/";
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
                    //string file_path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PR/"; //// Server.MapPath("~/Uploads/PO/");
                    string file_path = "../../../Uploads/PR/";
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
        public void GetDocumentWithAppendedContent(List<POAppHistoryViewModel> HISList, string SRCfileName, string DSTfileName, int marginLeft, List<POAppAuthSeqViewModel> AuthSeq)
        {
            int X = 0, Y = 0, APageNo = 0;
            GetSigLOCATION(SRCfileName, out X, out Y, out APageNo);
            Y = Y + 90;
            //var stream = new MemoryStream();
            //string path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/" + SRCfileName;            
            //var writer = new PdfWriter(_env.GetGeneralSettings().Get_FileUpload_Path + "/PO/" + DSTfileName);
            string path = serverpath.getFileUploadPath() + "PO/" + SRCfileName;
            var writer = new PdfWriter(serverpath.getFileUploadPath() + "PO/" + DSTfileName);
            //writer.SetCloseStream(false); // so I can reuse stream to create a readonly document later

            var pdfResult = new PdfDocument(new PdfReader(path), writer);
            var document = new Document(pdfResult);

            //document.Add(div);
            int pagecount = pdfResult.GetNumberOfPages();
            for (int i = 1; i <= pagecount; i++)
            {
                var div = new Div();
                Canvas canvas;
                PdfPage page = pdfResult.GetPage(i);
                int position = 0;

                for (int j = 0; j < AuthSeq.Count - 1; j++)
                {
                    Paragraph pgr = new Paragraph();
                    Int32 authseqno = AuthSeq[j].APP_SEQ; //(authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().APP_SEQ);
                    string empname = AuthSeq[j].ADEMPNAME;//authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().ADEMPNAME;
                    string appecode = AuthSeq[j].ADEMPCODE.ToString();//authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().ADEMPNAME;


                    canvas = new Canvas(page, page.GetPageSize());
                    //Set fixed position to put the div at the left bottom corner of the canvas
                    //div.SetFixedPosition(50, 0, page.GetPageSize().GetWidth());
                    if (j == AuthSeq.Count - 2)
                    {
                        if (i == APageNo)
                        {
                            string appdate = HISList.Where(m => m.ADEMPCODE == AuthSeq[j].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("yyyy.MM.dd HH:mm:ss ");
                            //     string str = "<table><tr><td style='border:4px solid;' >"+ HISList[j].APPEMP_NAME + " </BR>"+ Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy") + " </td></tr></table>";
                            pgr.Add(empname.ToUpper() + "\n");
                            pgr.Add("Date: " + appdate + "IST");
                            //  pgr.Add(str);
                            pgr.SetFontSize(11);
                            //  iText.Layout.Borders.SolidBorder dg;
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(11).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);

                            //Create canvas fro the last page

                            div.SetFixedPosition(X - 25, Y, 170);
                        }
                    }
                    else
                    {
                        string appdate = HISList.Where(m => m.ADEMPCODE == AuthSeq[j].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");
                        //     string str = "<table><tr><td style='border:4px solid;' >"+ HISList[j].APPEMP_NAME + " </BR>"+ Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy") + " </td></tr></table>";
                        pgr.Add(empname + " - [" + appecode + "]" + "\n");
                        pgr.Add(appdate);
                        //  pgr.Add(str);
                        pgr.SetFontSize(6);
                        //  iText.Layout.Borders.SolidBorder dg;
                        div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        div.Add(pgr);

                        //Create canvas fro the last page
                        //canvas = new Canvas(page, page.GetPageSize());
                        position = 80 * (j + 1);
                        div.SetFixedPosition(position, 35, 80);
                    }

                    //iText.Layout.Borders.Border bd=new ;
                    //bd.SetWidth(1);

                    //div.SetBorder(.Side.LEFT);


                    canvas.Add(div);
                }

            }


            document.Close();
            pdfResult.Close();

        }

        public void GetSigLOCATION(string SRCfileName, out Int32 X, out Int32 Y, out Int32 Pageno)
        {
            string path = _env.GetGeneralSettings().Get_FileUpload_Path + "PO/" + SRCfileName;
            X = 0; Y = 0; Pageno = 0;
            using (var reader = new iTextSharp.text.pdf.PdfReader(path))
            {

                for (int pagen = 1; pagen <= reader.NumberOfPages; pagen++)
                {
                    var parser = new PdfReaderContentParser(reader);

                    var strategy = parser.ProcessContent(pagen, new LocationTextExtractionStrategyWithPosition());
                    var res = strategy.GetAbsoluteLocations();

                    var searchResult = res.Where(p => p.Text.Contains("Signatories")).ToList();
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


        [HttpGet]
        public ActionResult GetPRATTDetail(string poID)
        {
            PRHeaderViewModel prDetail = new PRHeaderViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                prDetail = _POService.GetPRDetailByPOId(Convert.ToInt64(poID));

            }
            catch (Exception ex)
            {
                prDetail = null;
            }
            return Json(prDetail);
        }

        [HttpGet]
        public ActionResult PONextApproval(string POID_PARAM)
        {
            string retVal = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long userid = Convert.ToInt64(_sessionService.Get<string>("userID"));
                long poidcurr = 0, POIDNext;
                if (!string.IsNullOrEmpty(POID_PARAM))
                {
                    poidcurr = Convert.ToInt64(POID_PARAM);
                    POIDNext = _POService.GetPONextApprovalId(poidcurr, userid);
                    retVal = POIDNext == 0 ? "" : POIDNext.ToString();
                }
            }
            catch (Exception ex)
            {
                retVal = "";
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult POUserReport()
        {
            Search_VW_PODASHBOARD_VWMODEL obj = new Search_VW_PODASHBOARD_VWMODEL();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                ModelState.Clear();
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult POUserReport([FromBody] Search_VW_PODASHBOARD_VWMODEL SI)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Search_VW_PODASHBOARD_VWMODEL _headerObj = _POService.GetPODASHBOARD(SI);
            ModelState.Clear();
            return PartialView("_GetPOUserReport", _headerObj);
        }

        [HttpPost]
        public ActionResult SENTMAIL(string id_str)
        {
            short retVal = 0;
            string Error = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                retVal = _POService.SentMailToVendor(Convert.ToInt64(id_str));

            }
            catch (Exception ex)
            {
                Error = ex.StackTrace.ToString();
                retVal = -1;
            }
            return Json(new { res = retVal, err_msg = Error });
        }

        [HttpPost]
        public ActionResult ReGeneratePODoc(string id_str)
        {
            short retVal = 0;
            string Error = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long addedby = Convert.ToInt64(_sessionService.Get<string>("userID"));
                long POID = Convert.ToInt64(id_str);
                POHeaderViewModel PO_Dtl = _POService.GetPORequestById(POID);
                if (PO_Dtl != null)
                {
                    var OBJAPP = PO_Dtl.poAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                    var APPTYPE = PO_Dtl.poAuthSeq.Where(x => x.ADEMPCODE == OBJAPP.ADEMPCODE).FirstOrDefault().APPTYPE;
                    //if digital sign required than comment OBJAPP if condition & un-comment if condition APPTYPE=null
                    //if (OBJAPP== null)
                    //{
                    if (APPTYPE == 2)
                    {
                        if (PO_Dtl.PROCESS_STATUS == 1)
                        {
                            // Added by Aumento for SR84686
                            //string path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/";
                            string path = serverpath.getFileUploadPath() + "PO/";
                            string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                            if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                            string strDSTFILENAME = System.IO.Path.Combine(pathtosave, "PO" + PO_Dtl.PONO + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf");
                            // Added by Aumento for SR84686
                            //string strDSTFILENAME = "PO" + PO_Dtl.PONO + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";

                            string strSRCFILENAME = PO_Dtl.poDetail.Where(m => m.DOC_TYPE == "PO").FirstOrDefault().FILENAME;
                            GetDocumentWithAppendedContent(PO_Dtl.poAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), strSRCFILENAME, strDSTFILENAME, 100, PO_Dtl.poAuthSeq);
                            List<PODetailViewModel> obj = new List<PODetailViewModel>() { new PODetailViewModel() { DOC_TYPE = "POA", FILENAME = strDSTFILENAME, ADDITIONAL_INFO = "Approved PO" } };
                            _POService.SaveAttachment(addedby, POID, obj);
                        }
                        //  }
                    }
                }
                retVal = 1;
            }
            catch (Exception ex)
            {
                Error = ex.StackTrace.ToString();
                retVal = -1;
            }
            return Json(new { res = retVal, err_msg = Error });
        }

        [HttpGet]
        public ActionResult POALLUserReport()
        {
            SearchPO obj = new SearchPO();
            obj.Status = -1;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<PR_Div_Dep_SecViewModel> _OPList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _DivList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _DptList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _SCList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _KIList = new List<PR_Div_Dep_SecViewModel>();

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                _KIList = _POService.GetKiLIST(Convert.ToInt64(employeeDetails.Employee_Code));


                if (TempData["KIID"] == null || Convert.ToString(TempData["KIID"]) == "")
                {
                    obj.KIID = _KIList.FirstOrDefault().Value;
                }
                else
                {
                    obj.KIID = Convert.ToInt64(TempData["KIID"]);
                }

                Employee_Details employeeDetailski = _POService.GetEmployeeDetail(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                obj.OperationID = employeeDetailski._OpId == null ? 0 : (long)employeeDetailski._OpId;
                obj.DivisionID = employeeDetailski._DivId == null ? 0 : (long)employeeDetailski._DivId;
                obj.DEPTID = employeeDetailski._DepId == null ? 0 : (long)employeeDetailski._DepId;
                obj.SECID = employeeDetailski._SecId == null ? 0 : (long)employeeDetailski._SecId;
                obj.IsTeamMember = (employeeDetailski.Functional_Designation_Id == "" ? 1 : 0);


                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _POService.BindOperation(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                obj.ISSpecialRight = OPtemp.Item1;

                if (obj.IsTeamMember == 1 && obj.ISSpecialRight == 0)
                {
                    obj.ecode = Convert.ToInt64(employeeDetails.Employee_Code);
                }


                if (obj.ISSpecialRight == 1)
                {
                    _OPList = OPtemp.Item2;
                    long opid = _OPList.FirstOrDefault().Value;
                    Tuple<long, List<PR_Div_Dep_SecViewModel>> divtemp = _POService.BindDivision(opid, Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                    if (divtemp.Item2.Count > 0)
                    {
                        _DivList = divtemp.Item2;
                    }

                    else
                    {
                        _DivList = new List<PR_Div_Dep_SecViewModel>();
                    }
                    obj.DivisionID = 0;
                }
                else
                {
                    if (obj.OperationID != 0)
                    {
                        _OPList.Add(new PR_Div_Dep_SecViewModel
                        {
                            Value = Convert.ToInt64(employeeDetailski._OpId),
                            Text = employeeDetailski._OpDesc.ToString()
                        });
                    }
                    if (obj.DivisionID != 0)
                    {
                        _DivList.Add(new PR_Div_Dep_SecViewModel
                        {
                            Value = Convert.ToInt64(employeeDetailski._DivId),
                            Text = employeeDetailski._DivDesc.ToString()
                        });
                    }
                    else
                    {
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> divtemp = _POService.BindDivision(obj.OperationID, Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                        if (divtemp.Item2.Count > 0)
                        {
                            _DivList = divtemp.Item2;
                        }
                    }

                    if (obj.DEPTID != 0)
                    {
                        _DptList.Add(new PR_Div_Dep_SecViewModel
                        {
                            Value = Convert.ToInt64(employeeDetailski._DepId),
                            Text = employeeDetailski._DepDesc.ToString()
                        });
                    }
                    else
                    {
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> dpttemp = _POService.BindDepartment(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID, obj.OperationID, obj.DivisionID);
                        if (dpttemp.Item2.Count > 0)
                        {
                            _DptList = dpttemp.Item2;
                        }
                    }
                    if (obj.SECID != 0)
                    {
                        _SCList.Add(new PR_Div_Dep_SecViewModel
                        {
                            Value = Convert.ToInt64(employeeDetailski._SecId),
                            Text = employeeDetailski._SecDescrip.ToString()
                        });
                    }
                    else
                    {
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> sectemp = _POService.BindSection(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID, obj.OperationID, obj.DivisionID, obj.DEPTID);
                        if (sectemp.Item2.Count > 0)
                        {
                            _SCList = sectemp.Item2;
                        }
                    }

                }

                ViewBag.OPList = new SelectList(_OPList, "Value", "Text");
                ViewBag.DivList = new SelectList(_DivList, "Value", "Text");
                ViewBag.DepList = new SelectList(_DptList, "Value", "Text");
                ViewBag.SecList = new SelectList(_SCList, "Value", "Text");
                ViewBag.KIList = new SelectList(_KIList, "Value", "Text");
                //ViewBag.CategoryList = new SelectList(_IomService.BindIOMCategory(), "IOMCATMSTID", "CATDESC");

                ModelState.Clear();
                return View("POALLUserReport", obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult POALLUserReport([FromBody] SearchPO SI)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SI.loginid = Convert.ToInt64(_sessionService.Get<string>("userID"));
            SearchPO _headerObj = _POService.POALLUserReport(SI);
            ModelState.Clear();
            return PartialView("_GetPOALLUserReport", _headerObj);
        }
        [HttpPost]
        //*************************CR7510 11-Nov-25 Excelsheet download issue *********************************
        public ActionResult POALLUserReportExcel([FromBody] SearchPO SI)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SI.loginid = Convert.ToInt64(_sessionService.Get<string>("userID"));
                SearchPO _headerObj = _POService.POALLUserReport(SI);

                string str = this.POALLUserReportExcelHtml(_headerObj.SearchResult);
                TempData.Remove("POREPORTEXCELFILE");
                TempData["POREPORTEXCELFILE"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public ActionResult DownloadPOALLUserReportExcel()
        {
            try
            {
                if (TempData["POREPORTEXCELFILE"] == null)
                {
                    return View();
                }
                string str = (string)TempData["POREPORTEXCELFILE"];
                // HttpContext.Response.AddHeader("content-disposition", "attachment; filename=POReport.xls");

                //Response.ContentType = "application/vnd.ms-excel";
                //return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
                Response.Headers.Add("Content-Disposition", "attachment; filename=POReport.xls");
                Response.ContentType = "application/vnd.ms-excel";

                // Return the file with UTF-8 encoded bytes
                return File(Encoding.UTF8.GetBytes(str), "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string POALLUserReportExcelHtml(List<VM_VW_DGIT_POREPORT> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>PO No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Department</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Section</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Vendor Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>PO Description</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Last Updated Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Remark</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Vendor Code</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Version No.</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (var AHVM in _headerList)
                {
                    string POStatus = "";
                    if (AHVM.PROCESS_STATUS == 0 || AHVM.PROCESS_STATUS == 1)
                        POStatus = "WIP";
                    if (AHVM.PROCESS_STATUS == 2)
                        POStatus = "Complete";
                    if (AHVM.PROCESS_STATUS == 3)
                        POStatus = "Rejected";
                    if (AHVM.PROCESS_STATUS == 4)
                        POStatus = "Cancelled";

                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.PONO + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.ADDEDBY + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.ADDEDBYNAME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.OPERATION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.DIVISION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.DEPARTMENT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.SECTION + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.VENDORNAME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.PO_DESC + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.DATEADDED).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.UPDATEDATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + POStatus + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.REMARK + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.VENDORID + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.VERSION_NO + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }
        //***********************13-Nov-25**********************
        //***********************CR7504*************************
        [HttpGet]
        public ActionResult KIChange(int KIID)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return Json("0");
            }
            else
            {
                TempData["KIID"] = KIID;
                return Json("1");
            }
        }

        public ActionResult BindDivisionByOperationId(long id, long kiid)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<PR_Div_Dep_SecViewModel> divList = new List<PR_Div_Dep_SecViewModel>();
            if (id != 0)
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _POService.BindDivision(id, Convert.ToInt64(employeeDetails.Employee_Code), kiid);
                if (OPtemp.Item2.Count > 0)
                {
                    divList = OPtemp.Item2;
                }

                else
                {
                    divList = new List<PR_Div_Dep_SecViewModel>();
                }
            }
            return Json(divList);
        }

        public ActionResult BindDeptByDivisionId(long KIID, long op_Id, long Div_id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<PR_Div_Dep_SecViewModel> DepList = new List<PR_Div_Dep_SecViewModel>();
            if (Div_id != 0)
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _POService.BindDepartment(employeeDetails._ECode, KIID, op_Id, Div_id);
                if (OPtemp.Item2.Count > 0)
                {
                    DepList = OPtemp.Item2;
                }

                else
                {
                    DepList = new List<PR_Div_Dep_SecViewModel>();
                }
            }
            return Json(DepList);
        }

        public ActionResult BindSecByDepartmentId(long KIID, long op_Id, long divid, long deptid)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<PR_Div_Dep_SecViewModel> SecList = new List<PR_Div_Dep_SecViewModel>();
            if (deptid != 0)
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _POService.BindSection(employeeDetails._ECode, KIID, op_Id, divid, deptid);
                if (OPtemp.Item2.Count > 0)
                {
                    SecList = OPtemp.Item2;
                }

                else
                {
                    SecList = new List<PR_Div_Dep_SecViewModel>();
                }
            }
            return Json(SecList);
        }

    }

}
