using ePortal.Application.Contracts;
using ePortal.Persistence.Admin.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Text;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class ICIBMController : Controller
    {
        // GET: ICIBM
        IICIBMService _ICService;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public object ICENCRYT { get; private set; }
        private readonly ISessionService _sessionService;
        private readonly IPORequest _objPO;
        private readonly IAppConfigurationService _env;
        public ICIBMController(IICIBMService ICService, ISessionService sessionService, IPORequest objPO, IWebHostEnvironment webHostEnvironment, IAppConfigurationService env)
        {
            _ICService = ICService;
            _sessionService = sessionService;
            _objPO = objPO;
            _webHostEnvironment = webHostEnvironment;
            _env = env;
        }

        public ActionResult ICRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ICREQHEADER _View = new ICREQHEADER();
            List<VM_DGIT_ICAPPAUTHSEQ> iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
            if (TempData["APPROVAL_AUTH_LIST"] == null)
            {
                iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
            }
            if (iList != null && iList.Any())
            {
                TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(iList.OrderBy(o => o.APP_SEQ).ToList());
            }
            TempData.Keep();
            #region "Investment Effect Master"
            List<VM_SELECTITEMLIST> names = new List<VM_SELECTITEMLIST>();
            var objInvstmstlst = _ICService.GetInvestEffectList();
            foreach (var objval in objInvstmstlst)
            {
                names.Add(new VM_SELECTITEMLIST { Text = objval.INVESTEFFECTNAME, Value = objval.ICINVESTMSTID.ToString() });
            }
            #endregion

            _View.ICINVEST_EFFECT = names;
            return View(_View);
        }
        [HttpPost]
        public ActionResult ICRequest([FromBody] ICREQHEADER model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.ICDOCDETAIL = new List<VM_DGIT_ICDOCDETAIL>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                Tuple<short, long> retVal_tuple = _ICService.SaveICRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult SaveICDetail([FromBody] ICREQHEADER PHVM)
        {
            short retVal = 0; long hearderId = 0; string encrvalue = "";
            string errmsg = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                PHVM.ICDOCDETAIL = new List<VM_DGIT_ICDOCDETAIL>();
                PHVM.ICAPPAUTHSEQ = new List<VM_DGIT_ICAPPAUTHSEQ>();
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                foreach (var objs in PHVM.ICREQ_SCHEDULE)
                {
                    objs.SCHEDULE = DateTime.ParseExact("01-" + objs.SCHEDULESTR, "dd-MMM-yyyy", null);
                }

                Tuple<short, long> retVal_tuple = _ICService.SaveICRequest(PHVM);
                retVal = retVal_tuple.Item1;
                hearderId = retVal_tuple.Item2;
                //encrvalue = Server.UrlEncode(Encryption.Encrypt(hearderId.ToString())).ToString();
                encrvalue = WebUtility.UrlEncode(Encryption.Encrypt(hearderId.ToString())).ToString();
                
            }
            catch (Exception ex)
            {
                retVal = -1;
                errmsg = ex.Message.ToString();
            }
            return Json(new { res = retVal, ICID = hearderId, err = errmsg, ICENCRYT = encrvalue });
        }

        [HttpPost]
        public ActionResult UploadAttachment(VM_DGIT_ICDOCDETAIL formData)
        {
            short retVal = 0;
            List<VM_DGIT_ICDOCDETAIL> poDtlList = new List<VM_DGIT_ICDOCDETAIL>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                //if (formData.FILE.ContentLength > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.ICREQID > 0)

                if (formData.FILE != null && formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.ICREQID > 0)
                {
                    FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.ICDOC_ID.ToString());
                    poDtlList.Add(new VM_DGIT_ICDOCDETAIL
                    {
                        ICREQID = formData.ICREQID,
                        FILENAME = _file.FileName,
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = formData.DOC_TYPE,
                        ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString()),
                    });
                    Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _ret_tuple = _ICService.SaveAttachment(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), formData.ICREQID, poDtlList);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        //string path = Server.MapPath("~/Uploads/ICIBM/");
                        string path = _env.GetGeneralSettings().Get_FileUpload_Path + "ICIBM\\";
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (poDtlList.Count > 0)
                        {
                            foreach (VM_DGIT_ICDOCDETAIL obj in poDtlList)
                            {
                                if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                {
                                    string fullPath = Path.Combine(path, obj.FILENAME);
                                    System.IO.File.WriteAllBytes(fullPath, obj.FILE_BYTE.ToArray());
                                    //System.IO.File.WriteAllBytes(path + obj.FILENAME, obj.FILE_BYTE.ToArray());
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
            return Json(new { res = retVal, attachmentList = poDtlList.ToList() });
        }

        [HttpDelete]
        public ActionResult DeleteAttachment([FromBody] VM_DGIT_ICDOCDETAIL formData)
        {
            short retVal = 0;
            List<VM_DGIT_ICDOCDETAIL> poDtlList = new List<VM_DGIT_ICDOCDETAIL>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string fileName = formData.FILENAME;
                string docType = formData.DOC_TYPE;
                long ICHeaderId = formData.ICDOC_ID;

                Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _ret_tuple = _ICService.DeleteAttachment(fileName, docType, ICHeaderId);
                retVal = _ret_tuple.Item1;
                poDtlList = _ret_tuple.Item2;
                if (retVal == 1)
                {
                    //string path = Server.MapPath("~/Uploads/ICIBM/");
                    string path = _env.GetGeneralSettings().Get_FileUpload_Path + "ICIBM\\";
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
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.IsDeleted == 0).ToList() });
        }

        //private FileViewModel GetUploadFile(HttpPostedFileBase file, string DocType, string ICDOCID)
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
        //            string _FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
        //            FVM.FileContentType = MimeMapping.GetMimeMapping(_FileName);
        //            string strExtensionName = System.IO.Path.GetExtension(file.FileName);
        //            //FVM.FileName = DocType + "_" + PONo + ".pdf";
        //            FVM.FileName = DocType + "_" + ICDOCID + DateTime.Now.ToString("ddMMyyHHmmss") + strExtensionName;
        //            FVM.File = bytes;
        //        }
        //        return FVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        private FileViewModel GetUploadFile(IFormFile file, string docType, string icdocId)
        {
            try
            {
                FileViewModel fvm = new FileViewModel();

                if (file != null && file.Length > 0)
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    string fileExtension = Path.GetExtension(fileName);

                    // Use built-in MIME type detection
                    string contentType = file.ContentType;

                    fvm.FileContentType = contentType;
                    fvm.FileName = $"{docType}_{icdocId}_{DateTime.Now:ddMMyyHHmmss}{fileExtension}";
                    fvm.File = bytes;
                }

                return fvm;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public ActionResult GetPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/ICIBM/"; //// Server.MapPath("~/Uploads/PO/");
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

        [HttpGet]
        public ActionResult ICUserApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            //long _ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64((id));
            ICREQHEADER obj = _ICService.GetICRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            obj.ISENABLE = obj.ICAPPHISTORY.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0 && a.ICAPPHISTORY_ID != 0).Count().ToString();
            //ViewBag.FnDesigId = _Employee_Details._FnDesigId;
            return View("ICUserApproval", obj);

        }
        [HttpPost]
        public ActionResult ICUserApproval([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                retVal = _ICService.ICApproval(PHVM, _Employee_Details);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpGet]
        public ActionResult ICFinTaxApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            //long _ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64((id));
            ICREQHEADER obj = _ICService.GetICRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            obj.ISENABLE = obj.ICAPPHISTORY.Where(a => a.APP_TYPE == 4 && a.APPROVAL_STATUS == 0 && a.ICAPPHISTORY_ID != 0).Count().ToString();
            //ViewBag.FnDesigId = _Employee_Details._FnDesigId;
            return View("ICFinTaxApproval", obj);
        }
        [HttpPost]
        public ActionResult ICFinTaxApproval([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.APPROVAL_STATUS = 1;
                retVal = _ICService.ICFinTaxApproval(PHVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult GetAppAuthority(string eCode, int headerId, string header)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Login_Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                List<VM_DGIT_ICAPPAUTHSEQ> AuthSeqList = new List<VM_DGIT_ICAPPAUTHSEQ>();

                string[] splitString = eCode.Split('-');
                long empCode = Convert.ToInt64(splitString[0].Trim());

                Employee_Details employee_dtl = _ICService.GetAuthEmpById(empCode, _Login_Employee_Details);
                if (employee_dtl == null) { employee_dtl = new Employee_Details(); }

                if (TempData["APPROVAL_AUTH_LIST"] != null)
                {
                    //AuthSeqList = (List<VM_DGIT_ICAPPAUTHSEQ>)TempData["APPROVAL_AUTH_LIST"];
                    AuthSeqList = JsonConvert.DeserializeObject<List<VM_DGIT_ICAPPAUTHSEQ>>(TempData["APPROVAL_AUTH_LIST"].ToString());
                }

                if (employee_dtl._ECode > 0 && !string.IsNullOrEmpty(employee_dtl._EName))
                {
                    AuthSeqList.Add(new VM_DGIT_ICAPPAUTHSEQ
                    {
                        ADEMPCODE = employee_dtl._ECode,
                        ADEMPNAME = employee_dtl._EName,
                        ADDESIGNATION = employee_dtl._Desig,
                        APP_SEQ = Convert.ToInt16(headerId),
                        APP_HEADER = header,
                        APPTYPE = 1,
                    });
                }
                //TempData["APPROVAL_AUTH_LIST"] = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();
                TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());

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
                    SEQ_LIST = new List<IOMAppAuthSeqViewModel>()
                });
            }
        }
        public ActionResult DeleteAppAuthority(string eCode, string header, string ISALL)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<VM_DGIT_ICAPPAUTHSEQ> AuthSeqList = new List<VM_DGIT_ICAPPAUTHSEQ>();

                if (ISALL == "0")
                {
                    if (TempData["APPROVAL_AUTH_LIST"] != null)
                    {
                        //AuthSeqList = (List<VM_DGIT_ICAPPAUTHSEQ>)TempData["APPROVAL_AUTH_LIST"];
                        AuthSeqList = JsonConvert.DeserializeObject<List<VM_DGIT_ICAPPAUTHSEQ>>(TempData["APPROVAL_AUTH_LIST"].ToString());
                    }

                    if (AuthSeqList.Count > 0)
                    {
                        AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode));
                        short rowno = 0;
                        foreach (var obj in AuthSeqList.OrderBy(a => a.APP_SEQ))
                        {
                            obj.APP_SEQ = rowno;
                            rowno += 1;
                        }
                        //TempData["APPROVAL_AUTH_LIST"] = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();
                        TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());
                        retval = 1;
                    }
                }
                else
                {
                    //TempData["APPROVAL_AUTH_LIST"] = AuthSeqList;
                    TempData["Designation"] = "";
                    TempData.Keep();
                    retval = 1;
                }
                return Json(new { RESULT = retval, SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { RESULT = -1 });
            }
        }

        [HttpGet]
        public ActionResult ICFinDashboard()
        {

            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            VM_ICIBM_FIDASHBOARD_Search PHVM = new VM_ICIBM_FIDASHBOARD_Search();

            return View(PHVM);
        }

        [HttpPost]
        public ActionResult ICFinDashboard([FromBody] VM_ICIBM_FIDASHBOARD_Search PHVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PHVM = _ICService.GetFinanceDashboard(PHVM);

            }
            catch (Exception ex)
            {
            }
            return PartialView("_GetFinDashboard", PHVM.Result);
        }
        [HttpGet]
        public ActionResult ICFinTAXDashboard()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            VM_ICIBM_FITAXDASHBOARD_Search PHVM = new VM_ICIBM_FITAXDASHBOARD_Search();
            return View(PHVM);
        }

        [HttpPost]
        public ActionResult ICFinTAXDashboard([FromBody] VM_ICIBM_FITAXDASHBOARD_Search PHVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee"); 
                PHVM.PlantID = Convert.ToInt64(_Employee_Details.Plant_Id);
                PHVM = _ICService.GetFinanceTAXDashboard(PHVM);
            }
            catch (Exception ex)
            {
            }
            return PartialView("_GetFinTaxDashboard", PHVM.Result);
        }
        [HttpGet]
        public ActionResult ICFinAUCDashboard()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            VM_ICIBM_FIAUCDASHBOARD_Search PHVM = new VM_ICIBM_FIAUCDASHBOARD_Search();
            return View(PHVM);
        }

        [HttpPost]
        public ActionResult ICFinAUCDashboard([FromBody] VM_ICIBM_FIAUCDASHBOARD_Search PHVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.PlantID = Convert.ToInt64(_Employee_Details.Plant_Id);
                PHVM = _ICService.GetFinanceAUCDashboard(PHVM);
            }
            catch (Exception ex)
            {
            }
            return PartialView("_GetFinAUCDashboard", PHVM.Result);
        }
        [HttpGet]
        public ActionResult ICFinApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            //long _ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64((id));
            ICREQHEADER obj = _ICService.GetICRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            #region "Investment Effect Master"
            VM_SELECTITEMLIST objsearch = new VM_SELECTITEMLIST();
            var objCyclelst = _ICService.GetCycle(objsearch);
            ViewBag.CycleList = new SelectList(objCyclelst, "Value", "Text");
            #endregion
            obj.ISENABLE = obj.ICAPPHISTORY.Where(a => a.APP_TYPE == 3 && a.APPROVAL_STATUS == 0 && a.ICAPPHISTORY_ID != 0).Count().ToString();
            return View("ICFinApproval", obj);

        }
        [HttpPost]
        public ActionResult ICFinApproval([FromBody]  VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                retVal = _ICService.ICFinanceApproval(PHVM, _Employee_Details);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult ICViewDetail(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            //long _ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            //long _ReqId = Convert.ToInt64((id));
            ICREQHEADER obj = _ICService.GetICRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            return View("ICViewDetail", obj);

        }

        [HttpPost]
        public ActionResult RejectICRequest([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                retVal = _ICService.RejectICRequest(PHVM, _Employee_Details);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpPost]
        public ActionResult ICRequestRejection([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
            }
            return PartialView("_UpdateStatus", PHVM);
        }
        [HttpPost]
        public ActionResult InitiateICApproval([FromBody] VM_ICApprovalInitiation PHVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<VM_DGIT_ICAPPAUTHSEQ> iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
                iList = _ICService.GetICAuthority();
                iList = iList.OrderBy(o => o.APP_SEQ).ToList();
                //TempData["APPROVAL_AUTH_LIST"] = iList;
                PHVM.ICAPPAUTHSEQ = iList;
            }
            catch (Exception ex)
            {
            }
            return PartialView("_InitiateICApproval", PHVM);
        }
        [HttpPost]
        public ActionResult InitiateICApprovalSubmit([FromBody] VM_ICApprovalInitiation PHVM)
        {
            int retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<VM_DGIT_ICAPPAUTHSEQ> iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
                if (PHVM.ICCHKAUTH.Count > 0)
                {
                    short[] seqcre = PHVM.ICCHKAUTH.OrderBy(m => m.APP_SEQ).Select(m => m.APP_SEQ).Distinct().ToArray();
                    short seqno = 1;
                    foreach (short seq in seqcre)
                    {

                        var lst = PHVM.ICCHKAUTH.Where(m => m.APP_SEQ == seq).ToList();
                        foreach (var data in lst)
                        {

                            data.APP_SEQ = seqno;
                            iList.Add(data);
                        }
                        seqno += 1;
                    }
                }
                PHVM.ICAPPAUTHSEQ = iList;
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                //List<VM_DGIT_ICAPPAUTHSEQ> iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
                //iList=_ICService.GetICAuthority();
                //TempData["APPROVAL_AUTH_LIST"] = iList.OrderBy(o => o.APP_SEQ).ToList();
                PHVM.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retval = _ICService.ICApprovalInitiation(PHVM, _Employee_Details);
                retval = 1;
            }
            catch (Exception ex)
            {
                retval = 0;
            }
            return Json(retval);
        }
        [HttpPost]
        public ActionResult ICBulkApproval(VM_ICApprovalInitiation PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                #region "IC Request"
                string[] reqid = PHVM.ICREQID.Where(m => m.ICTYPE == "1").Select(m => m.ICREQID).ToArray();
                for (int i = 0; i < reqid.Length; i++)
                {
                    if (reqid[i] == "") continue;
                    VM_DGIT_ICAPPHISTORY appobj = new VM_DGIT_ICAPPHISTORY();
                    appobj.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    appobj.APPROVAL_STATUS = 1;
                    appobj.APP_DATE = DateTime.Today;
                    appobj.ICREQID = Convert.ToInt64(reqid[i]);
                    _ICService.ICApproval(appobj, _Employee_Details);
                }
                #endregion
                #region "Asset Disposal"
                reqid = PHVM.ICREQID.Where(m => m.ICTYPE == "2").Select(m => m.ICREQID).ToArray();
                for (int i = 0; i < reqid.Length; i++)
                {
                    if (reqid[i] == "") continue;
                    VM_DGIT_ICAPPHISTORY appobj = new VM_DGIT_ICAPPHISTORY();
                    appobj.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    appobj.APPROVAL_STATUS = 1;
                    appobj.APP_DATE = DateTime.Today;
                    appobj.ICREQID = Convert.ToInt64(reqid[i]);
                    _ICService.ICAssetApproval(appobj, _Employee_Details);
                }
                #endregion
                retVal = 1;

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpGet]
        public ActionResult EditICRequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //long _ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            ICREQHEADER PHVM = _ICService.GetICRequestById(_ReqId);
            PHVM.ICAPPAUTHSEQ = PHVM.ICAPPAUTHSEQ.Where(m => m.APPTYPE == 1).ToList();
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");;
            ViewBag.strId = id;
            return View(PHVM);
        }

        [HttpPost]
        public ActionResult EditICRequest([FromBody] ICREQHEADER model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.ICDOCDETAIL = new List<VM_DGIT_ICDOCDETAIL>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                model.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                Tuple<short, long> retVal_tuple = _ICService.SaveICRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        #region "Manage Request,Approval History"
        public ActionResult RetrieveICRequest()
        {
            return Json(FetchICRequest().ToList());
        }
        public List<VM_ICManageRquest> FetchICRequest()
        {
            List<VM_ICManageRquest> ICReqlist = new List<VM_ICManageRquest>();
            //PORequest obj = new PORequest();
            DataTable dt = _objPO.ManageICRequest(_sessionService.Get<string>("userID").ToString()).Tables[0];

            if (dt.Rows.Count != 0)
            {
                //div_leave.Visible = true;
                VM_ICManageRquest IC = null;
                foreach (DataRow dr in dt.Rows)
                {
                    string strDisable = string.Empty;
                    IC = new VM_ICManageRquest();
                    //IC.TransactionId = Server.UrlEncode(Encryption.Encrypt(dr["ICREQID"].ToString())).ToString();
                    IC.TransactionId = WebUtility.UrlEncode(Encryption.Encrypt(dr["ICREQID"].ToString())).ToString();
                    IC.Title = dr["ICTITLE"].ToString();
                    IC.Department = dr["DEPARTMENT"].ToString();
                    IC.AppliedDate = dr["REQDATE"].ToString();
                    IC.Status = dr["REQSTATUS"].ToString();
                    IC.Reqname = dr["REQECODE"].ToString();
                    IC.ProcessStatus = dr["PROCESSSTATUS"].ToString();
                    IC.ICREQID = dr["ICREQID"].ToString();
                    IC.REQUESTTYPE = dr["REQUESTTYPE"].ToString();
                    IC.ICTYPE = dr["ICTYPE"].ToString();
                    if (IC.ProcessStatus == "0" || IC.ProcessStatus == "5")
                    {
                        IC.IsEdit = "1";
                    }
                    else
                    {
                        IC.IsEdit = "0";
                    }

                    if (IC.ProcessStatus == "0" || IC.ProcessStatus == "1")
                    {
                        IC.IsCancel = "1";
                    }
                    else
                    {
                        IC.IsCancel = "0";
                    }

                    ICReqlist.Add(IC);
                }
            }
            return ICReqlist;
        }

        public ActionResult RetrieveICManageApproval()
        {
            return Json(FetchICManageApproval().ToList());
        }
        public List<VM_ICManageApproval> FetchICManageApproval()
        {

            List<VM_ICManageApproval> ICApplist = new List<VM_ICManageApproval>();
            //PORequest objPO = new PORequest();
            DataTable dtPO = _objPO.ManageICApproval(_sessionService.Get<string>("userID").ToString()).Tables[0];

            if (dtPO.Rows.Count == 0)
            {

            }
            else
            {

                VM_ICManageApproval ICM = null;
                foreach (DataRow dr in dtPO.Rows)
                {
                    ICM = new VM_ICManageApproval();
                    string strDisable = string.Empty;
                    ICM.APPTransactionId = (dr["ICREQID"].ToString()).ToString();
                    ICM.TransactionId = WebUtility.UrlEncode(Encryption.Encrypt(dr["ICREQID"].ToString())).ToString();
                    ICM.EmpCode = dr["ADEMPCODE"].ToString();
                    ICM.EmpName = dr["EMPNAME"].ToString();
                    ICM.Title = dr["ICTITLE"].ToString();
                    ICM.Department = dr["DEPARTMENT"].ToString();
                    ICM.AppliedDate = dr["REQDATE"].ToString();
                    ICM.Status = dr["REQSTATUS"].ToString();
                    ICM.ProcessStatus = dr["PROCESSSTATUS"].ToString();

                    ICM.BASIC_BUDGET = dr["BASIC_BUDGET"].ToString();
                    ICM.BASIC_PROPOSED = dr["BASIC_PROPOSED"].ToString();
                    ICM.COSTSAVING = dr["NAV"].ToString();
                    ICM.PAYBACK_PERIOD = dr["SRV"].ToString();
                    ICM.REQUESTTYPE = dr["REQUESTTYPE"].ToString();
                    ICM.ICTYPE = dr["ICTYPE"].ToString();
                    ICApplist.Add(ICM);
                }
            }
            return ICApplist;
        }
        #endregion

        //---------------------Asset Disposal
        public ActionResult AssetICRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ICASSETDISREQHEADER _View = new ICASSETDISREQHEADER();
            List<VM_DGIT_ICAPPAUTHSEQ> iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
            if (TempData["APPROVAL_AUTH_LIST"] == null)
            {
                iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
            }
            TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(iList.OrderBy(o => o.APP_SEQ).ToList());
            TempData.Keep();
            return View(_View);
        }
        [HttpPost]
        public ActionResult SaveAssetICDetail([FromBody] ICASSETDISREQHEADER PHVM)
        {
            short retVal = 0; long hearderId = 0; string encrvalue = "";
            string errmsg = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                PHVM.ICDOCDETAIL = new List<VM_DGIT_ICDOCDETAIL>();
                PHVM.ICAPPAUTHSEQ = new List<VM_DGIT_ICAPPAUTHSEQ>();
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                foreach (var objs in PHVM.ICREQ_SCHEDULE)
                {
                    objs.SCHEDULE = DateTime.ParseExact("01-" + objs.SCHEDULESTR, "dd-MMM-yyyy", null);
                }

                Tuple<short, long> retVal_tuple = _ICService.SaveAssetICRequest(PHVM);
                retVal = retVal_tuple.Item1;
                hearderId = retVal_tuple.Item2;
                encrvalue = WebUtility.UrlEncode(Encryption.Encrypt(hearderId.ToString())).ToString();
            }
            catch (Exception ex)
            {
                retVal = -1;
                errmsg = ex.InnerException.ToString();
            }
            return Json(new { res = retVal, ICID = hearderId, err = errmsg, ICENCRYT = encrvalue });
        }

        [HttpPost]
        public ActionResult UploadAssetAttachment(VM_DGIT_ICDOCDETAIL formData)
        {
            short retVal = 0;
            List<VM_DGIT_ICDOCDETAIL> poDtlList = new List<VM_DGIT_ICDOCDETAIL>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                //if (formData.FILE.ContentLength > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.ICREQID > 0)
                if (formData.FILE != null && formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.ICREQID > 0)
                    {
                    FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.ICDOC_ID.ToString());
                    poDtlList.Add(new VM_DGIT_ICDOCDETAIL
                    {
                        ICREQID = formData.ICREQID,
                        FILENAME = _file.FileName,
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = formData.DOC_TYPE,
                        ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString()),
                    });
                    Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _ret_tuple = _ICService.SaveAssetAttachment(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), formData.ICREQID, poDtlList);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        //string path = Server.MapPath("~/Uploads/ICIBM/");
                        string path = _env.GetGeneralSettings().Get_FileUpload_Path + "ICIBM\\";
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (poDtlList.Count > 0)
                        {
                            foreach (VM_DGIT_ICDOCDETAIL obj in poDtlList)
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
            return Json(new { res = retVal, attachmentList = poDtlList.ToList() });
        }

        [HttpDelete]
        //public ActionResult DeleteAssetAttachment(string fileName, string docType, long ICHeaderId)
        public ActionResult DeleteAssetAttachment([FromBody] VM_DGIT_ICDOCDETAIL formData)
        {
            short retVal = 0;
            List<VM_DGIT_ICDOCDETAIL> poDtlList = new List<VM_DGIT_ICDOCDETAIL>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string fileName = formData.FILENAME;
                string docType = formData.DOC_TYPE;
                long ICHeaderId = formData.ICDOC_ID;

                Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _ret_tuple = _ICService.DeleteAssetAttachment(fileName, docType, ICHeaderId);
                retVal = _ret_tuple.Item1;
                poDtlList = _ret_tuple.Item2;
                if (retVal == 1)
                {
                    //string path = Server.MapPath("~/Uploads/ICIBM/");
                    string path = _env.GetGeneralSettings().Get_FileUpload_Path + "ICIBM\\";
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
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.IsDeleted == 0).ToList() });
        }

        [HttpPost]
        public ActionResult ICAssetRequest([FromBody] ICASSETDISREQHEADER model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.ICDOCDETAIL = new List<VM_DGIT_ICDOCDETAIL>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                Tuple<short, long> retVal_tuple = _ICService.SaveAssetICRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpGet]
        public ActionResult ICAssetViewDetail(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            //long _ReqId = Convert.ToInt64((id));
            ICASSETDISREQHEADER obj = _ICService.GetICAssetRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            return View("ICAssetViewDetail", obj);

        }

        [HttpGet]
        public ActionResult ICAssetUserApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            long _ReqId = Convert.ToInt64((id));
            ICASSETDISREQHEADER obj = _ICService.GetICAssetRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            obj.ISENABLE = obj.ICAPPHISTORY.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0 && a.ICAPPHISTORY_ID != 0).Count().ToString();
            return View("ICAssetUserApproval", obj);

        }
        [HttpPost]
        public ActionResult ICAssetUserApproval([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                retVal = _ICService.ICAssetApproval(PHVM, _Employee_Details);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult RejectICAssetRequest([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                retVal = _ICService.RejectICAssetRequest(PHVM, _Employee_Details);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpPost]
        public ActionResult ICAssetRequestRejection([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
            }
            return PartialView("_UpdateAssetStatus", PHVM);
        }

        [HttpGet]
        public ActionResult ICAssetFinApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            //long _ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64((id));
            ICASSETDISREQHEADER obj = _ICService.GetICAssetRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            #region "Investment Effect Master"
            VM_SELECTITEMLIST objsearch = new VM_SELECTITEMLIST();
            var objCyclelst = _ICService.GetCycle(objsearch);
            ViewBag.CycleList = new SelectList(objCyclelst, "Value", "Text");
            #endregion

            return View("ICAssetFinApproval", obj);

        }
        [HttpPost]
        public ActionResult ICAssetFinApproval([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                retVal = _ICService.ICAssetFinanceApproval(PHVM, _Employee_Details);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        public ActionResult DeleteICAppAuthority(string eCode, string header, string ISALL)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<VM_DGIT_ICAPPAUTHSEQ> AuthSeqList = new List<VM_DGIT_ICAPPAUTHSEQ>();

                if (ISALL == "0")
                {
                    if (TempData["ICAPPROVAL_AUTH_LIST"] != null)
                    {
                        //AuthSeqList = (List<VM_DGIT_ICAPPAUTHSEQ>)TempData["ICAPPROVAL_AUTH_LIST"];
                        AuthSeqList = JsonConvert.DeserializeObject<List<VM_DGIT_ICAPPAUTHSEQ>>(TempData["ICAPPROVAL_AUTH_LIST"].ToString());

                    }

                    if (AuthSeqList.Count > 0)
                    {
                        AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode));
                        //short rowno = 0;
                        //foreach (var obj in AuthSeqList.OrderBy(a => a.APP_SEQ))
                        //{
                        //    obj.APP_SEQ = rowno;
                        //    rowno += 1;
                        //}
                        TempData["ICAPPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());
                        retval = 1;
                    }
                }
                else
                {
                    TempData["ICAPPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqList);
                    TempData["Designation"] = "";
                    TempData.Keep();
                    retval = 1;
                }
                return Json(new { RESULT = retval, SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { RESULT = -1 });
            }
        }

        [HttpGet]
        public ActionResult UpdateICMemberMaster()
        {
            List<VM_DGIT_ICAPPAUTHSEQ> iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                iList = _ICService.GetICAuthority();
                iList = iList.OrderBy(o => o.APP_SEQ).ToList();
                TempData["ICAPPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(iList);
            }
            catch (Exception ex)
            {
            }
            return PartialView("_UpdateICMember", iList);
        }

        [HttpPost]
        public ActionResult UpdateICMemberMaster([FromBody] List<VM_DGIT_ICAPPAUTHSEQ> PSVM)
        {
            List<VM_DGIT_ICAPPAUTHSEQ> iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
            try
            {
                if (PSVM.Count > 0)
                {
                    short[] seqcre = PSVM.Select(m => m.APP_SEQ).Distinct().ToArray();
                    short seqno = 1;
                    foreach (short seq in seqcre)
                    {

                        var lst = PSVM.Where(m => m.APP_SEQ == seq).ToList();
                        foreach (var data in lst)
                        {
                            data.APP_SEQ = seqno;
                            iList.Add(data);
                        }
                        seqno += 1;
                    }
                }
                long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                TempData["ICAPPROVAL_AUTH_LIST"] = null;
                _ICService.SaveICMemberMaster(userid, iList);
                return Json(new { RESULT = 1 });


            }
            catch (Exception ex)
            {
                return Json(new { RESULT = -1, errmsg = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateAUCDetail([FromBody] ICREQHEADER PSVM)
        {
            List<VM_DGIT_ICAPPAUTHSEQ> iList = new List<VM_DGIT_ICAPPAUTHSEQ>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
            }
            return PartialView("_UpdateAUC", PSVM);
        }

        [HttpPost]
        public ActionResult UpdateAUC([FromBody] ICREQHEADER PSVM)
        {
            try
            {
                if (PSVM.ICREQID > 0)
                {
                    long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    PSVM.AUCCODEUPDBY = userid;
                    Tuple<short, long> retVal_tuple = _ICService.UpdateAUCCode(PSVM);
                    return Json(new { RESULT = retVal_tuple.Item1 });
                }
                else
                {
                    return Json(new { RESULT = -1 });
                }

            }
            catch (Exception ex)
            {
                return Json(new { RESULT = -1, errmsg = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult GetICAppAuthority(string eCode, int headerId, string header)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Login_Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                List<VM_DGIT_ICAPPAUTHSEQ> AuthSeqList = new List<VM_DGIT_ICAPPAUTHSEQ>();

                string[] splitString = eCode.Split('-');
                long empCode = Convert.ToInt64(splitString[0].Trim());

                Employee_Details employee_dtl = _ICService.GetAuthEmpById(empCode, _Login_Employee_Details);
                if (employee_dtl == null) { employee_dtl = new Employee_Details(); }

                if (TempData["ICAPPROVAL_AUTH_LIST"] != null)
                {
                   AuthSeqList = JsonConvert.DeserializeObject<List<VM_DGIT_ICAPPAUTHSEQ>>(TempData["ICAPPROVAL_AUTH_LIST"].ToString());
                }

                if (employee_dtl._ECode > 0 && !string.IsNullOrEmpty(employee_dtl._EName))
                {
                    AuthSeqList.Add(new VM_DGIT_ICAPPAUTHSEQ
                    {
                        ADEMPCODE = employee_dtl._ECode,
                        ADEMPNAME = employee_dtl._EName,
                        ADDESIGNATION = employee_dtl._Desig,
                        APP_SEQ = Convert.ToInt16(headerId),
                        APP_HEADER = header,
                        APPTYPE = 2,
                    });
                }
                TempData["ICAPPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());

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
                    SEQ_LIST = new List<IOMAppAuthSeqViewModel>()
                });
            }
        }

        [HttpGet]
        public ActionResult EditICAssetRequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            ICASSETDISREQHEADER PHVM = _ICService.GetICAssetRequestById(_ReqId);
            PHVM.ICAPPAUTHSEQ = PHVM.ICAPPAUTHSEQ.Where(m => m.APPTYPE == 1).ToList();
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            ViewBag.strId = id;
            return View(PHVM);
        }

        [HttpPost]
        public ActionResult EditICAssetRequest([FromBody] ICASSETDISREQHEADER model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.ICDOCDETAIL = new List<VM_DGIT_ICDOCDETAIL>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                model.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                Tuple<short, long> retVal_tuple = _ICService.SaveAssetICRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult ICApprovalConfig()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");

            }
            List<VM_DGIT_ICCONFIG_MST> obj = _ICService.GetICConfig();
            return View(obj);
        }
        [HttpGet]
        public ActionResult AddeditICConfig(VM_DGIT_ICCONFIG_MST data)
        {
            VM_DGIT_ICCONFIG_MST result;
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");

            }
            if (data.ICCONFIGID == 0)
            {
                return View(data);
            }
            try
            {
                //data.ICCONFIGID = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(data.ICCONFIGID)));
                result = _ICService.GetICConfig(data);
                return View(result);
            }
            catch { Exception ex; }
            result = _ICService.GetICConfig(data);
            result.ICDATE = DateTime.Today;
            result.LASTSUBDT = DateTime.Today;
            return View(result);
        }

        [HttpPost]
        public ActionResult saveIC([FromBody] VM_DGIT_ICCONFIG_MST data)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                data.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                List<VM_DGIT_ICCONFIG_MST> obj = _ICService.GetICConfig();
                foreach (var id in obj)
                {
                    if (id.ICCONFIGID == data.ICCONFIGID && data.ICCONFIGID != 0)
                    {
                        retval = _ICService.updateIC(data);
                        return Json(retval);
                    }
                    if (id.ICDATE == data.ICDATE)
                    {
                        retval = 2;
                        return Json(retval);
                    }
                }
                retval = _ICService.saveIC(data);
            }
            catch (Exception ex)
            {
                retval = 0;
            }
            return Json(retval);
        }

        [HttpDelete]
        public ActionResult DeleteIC([FromBody] VM_DGIT_ICCONFIG_MST data)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                retval = _ICService.deleteIC(data);
            }
            catch (Exception ex)
            {
                retval = 0;
            }
            return Json(retval);
        }

        [HttpPost]
        public ActionResult UploadFinalDocument([FromBody] VM_DGIT_ICAPPHISTORY iList)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

            }
            catch (Exception ex)
            {
            }
            return PartialView("_UploadICFinalDocument", iList);
        }
        [HttpPost]
        public ActionResult UploadAttachmentFinal(VM_DGIT_ICDOCDETAIL formData)
        {
            short retVal = 0;


            List<VM_DGIT_ICDOCDETAIL> poDtlList = new List<VM_DGIT_ICDOCDETAIL>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                #region "Save IC Request Final Document"
                if (formData.ICTYPE == 1)
                {
                    //if (formData.FILE.ContentLength > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.ICREQID > 0)
                    if (formData.FILE != null && formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.ICREQID > 0)
                    {
                        FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.ICREQID.ToString());
                        poDtlList.Add(new VM_DGIT_ICDOCDETAIL
                        {
                            ICREQID = formData.ICREQID,
                            FILENAME = _file.FileName,
                            FILE_CONTENTTYPE = _file.FileContentType,
                            FILE_BYTE = _file.File,
                            DOC_TYPE = formData.DOC_TYPE,
                            ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                            ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString()),
                        });
                        Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _ret_tuple = _ICService.SaveAttachmentFinal(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), formData.ICREQID, poDtlList);
                        retVal = _ret_tuple.Item1;
                        if (retVal == 1)
                        {
                            //string path = Server.MapPath("~/Uploads/ICIBM/");
                            string path = _env.GetGeneralSettings().Get_FileUpload_Path + "ICIBM\\";
                            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                            if (poDtlList.Count > 0)
                            {
                                foreach (VM_DGIT_ICDOCDETAIL obj in poDtlList)
                                {
                                    if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                    {
                                        System.IO.File.WriteAllBytes(path + obj.FILENAME, obj.FILE_BYTE.ToArray());
                                    }
                                }
                            }
                            VM_DGIT_ICDOCDETAIL deletefile = _ret_tuple.Item2.FirstOrDefault();
                            //DeleteAttachment(deletefile.FILENAME, deletefile.DOC_TYPE, deletefile.ICREQID);
                            DeleteAttachment(deletefile);
                        }

                    }
                }
                #endregion

                #region "Save Asset IC Final Document"
                if (formData.ICTYPE == 2)
                {
                    //if (formData.FILE.ContentLength > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.ICREQID > 0)
                    if (formData.FILE != null && formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.ICREQID > 0)
                    {
                        FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.ICREQID.ToString());
                        poDtlList.Add(new VM_DGIT_ICDOCDETAIL
                        {
                            ICREQID = formData.ICREQID,
                            FILENAME = _file.FileName,
                            FILE_CONTENTTYPE = _file.FileContentType,
                            FILE_BYTE = _file.File,
                            DOC_TYPE = formData.DOC_TYPE,
                            ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                            ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString()),
                        });
                        Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _ret_tuple = _ICService.SaveAssetAttachmentFinal(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), formData.ICREQID, poDtlList);
                        retVal = _ret_tuple.Item1;
                        if (retVal == 1)
                        {
                            //string path = Server.MapPath("~/Uploads/ICIBM/");
                            string path = _env.GetGeneralSettings().Get_FileUpload_Path + "ICIBM\\";
                            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                            if (poDtlList.Count > 0)
                            {
                                foreach (VM_DGIT_ICDOCDETAIL obj in poDtlList)
                                {
                                    if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                    {
                                        System.IO.File.WriteAllBytes(path + obj.FILENAME, obj.FILE_BYTE.ToArray());
                                    }
                                }
                            }
                            VM_DGIT_ICDOCDETAIL deletefile = _ret_tuple.Item2.FirstOrDefault();
                            //DeleteAssetAttachment(deletefile.FILENAME, deletefile.DOC_TYPE, deletefile.ICREQID);
                            DeleteAssetAttachment(deletefile);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal });
        }


        [HttpGet]
        public ActionResult ICPrintView(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            //long _ReqId = Convert.ToInt64((id));
            ICREQHEADER obj = _ICService.GetICRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            return View("ICPrintView", obj);

        }
        [HttpGet]
        public ActionResult ICAssetPrintView(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            //long _ReqId = Convert.ToInt64((id));
            ICASSETDISREQHEADER obj = _ICService.GetICAssetRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            return View("ICAssetPrintView", obj);

        }

        [HttpGet]
        public ActionResult ICCancelRequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            ICREQHEADER obj = _ICService.GetICRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            return View("ICCancelRequest", obj);

        }
        [HttpPost]
        public ActionResult ICCancelRequest([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.APPROVAL_STATUS = 4;

                retVal = _ICService.ICReqCancel(PHVM);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult ICAssetCancelRequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            ICASSETDISREQHEADER obj = _ICService.GetICAssetRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            return View("ICAssetCancelRequest", obj);

        }
        [HttpPost]
        public ActionResult ICAssetCancelRequest([FromBody] VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");;
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.APPROVAL_STATUS = 4;
                retVal = _ICService.ICAssetReqCancel(PHVM);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpGet]
        public ActionResult ICPPCDashboard()
        {

            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            VM_ICIBM_PPCDASHBOARD_Search PHVM = new VM_ICIBM_PPCDASHBOARD_Search();

            return View(PHVM);
        }

        [HttpPost]
        public ActionResult ICPPCDashboard([FromBody] VM_ICIBM_PPCDASHBOARD_Search PHVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PHVM.SearchBy = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM = _ICService.GetPPCDashboard(PHVM);

            }
            catch (Exception ex)
            {
            }
            return PartialView("_GetPPCDashboard", PHVM.Result);
        }

        [HttpPost]
        public ActionResult ICDetailExportToExcel([FromBody] VM_ICIBM_PPCDASHBOARD_Search SI)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                SI.SearchBy = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SI = _ICService.GetPPCDashboard(SI);
                string str = this.ICDetailExcelHtml(SI.Result);
                TempData.Remove("ALLOCATIONEXCELFILE");
                TempData["ALLOCATIONEXCELFILE"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        public ActionResult ICDetailDownloadExcel()
        {
            try
            {
                if (TempData["ALLOCATIONEXCELFILE"] == null)
                {
                    return View();
                }
                string str = (string)TempData["ALLOCATIONEXCELFILE"];
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=ICDetailReport.xls");
                Response.Headers.Add("Content-Disposition", "attachment; filename=ICDetailReport.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string ICDetailExcelHtml(List<VM_ICIBM_PPCDASHBOARD> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>Sr.No.</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Request Type</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Operation</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Department</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Title</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Budget Amt.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Investment Effect Forecast</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Cost Saving</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>ROI</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Implementation Month</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Gross Asset Value</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Net Asset Value</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Scrap Realization</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Net P&L</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Submission Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                List<VM_SELECTITEMLIST> investdtls = _ICService.GetICInvestdtls();
                foreach (VM_ICIBM_PPCDASHBOARD AHVM in _headerList)
                {
                    string statusname = "";
                    string investname = "";
                    string subdate = "";
                    if (AHVM.status == 0) { statusname = "User Management approval-WIP"; }
                    else if (AHVM.status == 1) { statusname = "Pending for Approval"; }
                    else if (AHVM.status == 2) { statusname = "Accepted By Finance"; }
                    else if (AHVM.status == 3) { statusname = "Rejected"; }
                    else if (AHVM.status == 4) { statusname = "Cancelled"; }
                    else if (AHVM.status == 6) { statusname = "Pending for IC Approval"; }
                    else if (AHVM.status == 7) { statusname = "Completed"; }
                    else if (AHVM.status == 9) { statusname = "IC Rejection"; }
                    foreach (var item in investdtls)
                    {
                        if (AHVM.INVEST_EFFECT.Contains(item.Value))
                        {
                            investname += item.Text + ", ";
                        }
                    }
                    if (AHVM.LASTAPPROVAL_FROM != null)
                    {
                        subdate = AHVM.LASTAPPROVAL_FROM.Value.ToString("dd-MMM-yyyy");
                    }
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.REQUESTTYPE + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.OPERATION + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.DEPARTMENT + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.ENAME + "-" + AHVM.ADEMPCODE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.ICTITLE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.BASIC_BUDGET + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + investname + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.COSTSAVING + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.PAYBACK_PERIOD + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.SCHEDULE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.GAVAMT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.NAVAMT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.SRVAMT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.NPLAMT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + subdate + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + statusname + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        [HttpGet]
        public ActionResult ICInvestdtls()
        {
            List<VM_SELECTITEMLIST> investnames = null;

            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return null;
                }


                investnames = _ICService.GetICInvestdtls();
            }
            catch (Exception ex)
            {
            }
            return Json(investnames);
        }




    }
}
