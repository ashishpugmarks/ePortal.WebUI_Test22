using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.IO;
using iText.Layout.Element;
using ePortal.ViewModels;
using Microsoft.AspNetCore.StaticFiles;
using ePortal.Application.Contracts;
using System.Net;
using ePortal.Shared;
using ePortal.Shared.Interface;
using System.Text;
using Newtonsoft.Json;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using System.Data;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using ePortal.WebUI.Filters;
using ePortal.ViewModels.DataExchange.IOM;
using System.Reflection;
//Changed by TTL on 07-Aug-2025 against SR102253 > CR6697 - Start
using iText.Layout.Layout;
using iText.Layout.Renderer;
//Changed by TTL on 07-Aug-2025 against SR102253 > CR6697 - End



namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class IOMController : Controller
    {
        private readonly IIOMService _IomService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<IOMController> _logger;
        private readonly string _userId;
        private readonly string _userName;
        private readonly Employee_Details _EmpDetails;

        public IOMController(IIOMService iomService, ISessionService sessionService, ILogger<IOMController> logger)
        {
            _IomService = iomService;
            _sessionService = sessionService;
            _logger = logger;
            _userId = _sessionService.Get<string>("userID").ToString();
            _userName = _sessionService.Get<string>("userName").ToString();
            _EmpDetails = _sessionService.Get<Employee_Details>("Employee");

        }


        public ActionResult IOMViewDetail(string id)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId;
            try
            {
                //_ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
                _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));

                // _ReqId = Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(id))); // Server.UrlDecode(Encryption.Decrypt(id))
            }
            catch (Exception ex)
            {
                _ReqId = Convert.ToInt64(Encryption.Decrypt(id)); // Server.UrlDecode(Encryption.Decrypt(id))
            }

            return View("IOMViewDetail", _IomService.GetIOMRequestById(_ReqId));
        }

        [HttpPost]
        public ActionResult UploadIOMAttachment(IOMDetailViewModel formData)
        {
            short retVal = 0; long _headerId = 0;
            List<IOMDetailViewModel> poDtlList = new List<IOMDetailViewModel>();
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (formData.IOMHEADERID > 0)
                {
                    _headerId = formData.IOMHEADERID;
                }
                else
                {
                    IOMHeaderViewModel model = new IOMHeaderViewModel();
                    model.IsFinalSubmit = 0;
                    model.PROCESS_STATUS = 0;
                    model.IOMDesc = formData.IOMDESC;
                    model.ADDEDBY = Convert.ToInt64(_userId.ToString());
                    model.UPDATEDBY = Convert.ToInt64(_userId.ToString());
                    model.STATUS = 1;
                    Tuple<short, long> retVal_tuple = _IomService.SaveIOMRequest(model);
                    if (retVal_tuple.Item1 == 1 && retVal_tuple.Item2 > 0)
                    {
                        _headerId = retVal_tuple.Item2;
                    }
                    else
                    {
                        return Json(new { res = retVal_tuple.Item1, headerId = _headerId, iomAttachment = poDtlList.Where(w => w.DOC_TYPE == "IOM").FirstOrDefault() });
                    }
                }
                if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE))
                {
                    //string path = Server.MapPath("~/Uploads/DGIT_IOM/");
                    string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, _headerId.ToString());
                    poDtlList.Add(new IOMDetailViewModel
                    {
                        IOMHEADERID = formData.IOMHEADERID,
                        FILENAME = pathtosave + @"\" + _file.FileName,
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = formData.DOC_TYPE,
                        ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_userId.ToString()),
                    });
                    Tuple<short, List<IOMDetailViewModel>> _ret_tuple = _IomService.SaveAttachment(Convert.ToInt64(_userId.ToString()), _headerId, poDtlList);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        if (poDtlList.Count > 0)
                        {
                            foreach (IOMDetailViewModel obj in poDtlList)
                            {
                                if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                {
                                    System.IO.File.WriteAllBytes(Path.Combine(path, obj.FILENAME), obj.FILE_BYTE.ToArray());
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
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(new { res = retVal, headerId = _headerId, iomAttachment = poDtlList.Where(w => w.DOC_TYPE == "IOM").FirstOrDefault() });
        }

        [HttpGet]
        public ActionResult IOMRequest()
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<IOMAppAuthSeqViewModel> iList = new List<IOMAppAuthSeqViewModel>();
            if (TempData["APPROVAL_AUTH_LIST"] == null)
            {
                iList = new List<IOMAppAuthSeqViewModel>();
            }
            var orderbyIlist = iList.OrderBy(o => o.APP_SEQ).ToList();
            var jsonList = JsonConvert.SerializeObject(orderbyIlist);
            TempData["APPROVAL_AUTH_LIST"] = jsonList;
            TempData.Keep();
            //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - Start
            Employee_Details employeeDetails = _EmpDetails;
            ViewBag.CategoryList = new SelectList(_IomService.BindIOMCategory(employeeDetails.Division_Id), "IOMCATMSTID", "CATDESC");
            //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - End
            return View();
        }

        [HttpPost]
        public ActionResult IOMRequest([FromBody] IOMHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.iomDetail = new List<IOMDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_userId.ToString());
                model.UPDATEDBY = Convert.ToInt64(_userId.ToString());


                Tuple<short, long> retVal_tuple = _IomService.SaveIOMRequest(model);
                retVal = retVal_tuple.Item1;

            }
            catch (Exception ex)
            {
                retVal = -1;
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult UploadAttachment(IOMDetailViewModel formData)
        {
            short retVal = 0;
            List<IOMDetailViewModel> poDtlList = new List<IOMDetailViewModel>();
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (formData.IOMHEADERID > 0)
                {
                    if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE))
                    {
                        //string path = Server.MapPath("~/Uploads/DGIT_IOM/");
                        string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                        FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.IOMHEADERID.ToString());
                        poDtlList.Add(new IOMDetailViewModel
                        {
                            IOMHEADERID = formData.IOMHEADERID,
                            FILENAME = pathtosave + @"\" + _file.FileName,
                            FILE_CONTENTTYPE = _file.FileContentType,
                            FILE_BYTE = _file.File,
                            DOC_TYPE = formData.DOC_TYPE,
                            ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                            ADDEDBY = Convert.ToInt64(_userId.ToString()),
                        });
                        Tuple<short, List<IOMDetailViewModel>> _ret_tuple = _IomService.SaveAttachment(Convert.ToInt64(_userId.ToString()), formData.IOMHEADERID, poDtlList);
                        retVal = _ret_tuple.Item1;
                        if (retVal == 1)
                        {
                            //string path = Server.MapPath("~/Uploads/DGIT_IOM/");
                            //if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                            if (poDtlList.Count > 0)
                            {
                                foreach (IOMDetailViewModel obj in poDtlList)
                                {
                                    if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                    {
                                        System.IO.File.WriteAllBytes(Path.Combine(path, obj.FILENAME), obj.FILE_BYTE.ToArray());
                                    }
                                }
                            }
                        }
                        poDtlList = _ret_tuple.Item2;
                    }
                }
                //else
                //{
                //    if (TempData["IOM_ATTACHMENT_LIST"] != null)
                //    {
                //        poDtlList = (List<IOMDetailViewModel>)TempData["IOM_ATTACHMENT_LIST"];
                //    }
                //    if (formData.FILE.ContentLength > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE))
                //    {
                //        if (formData.DOC_TYPE == "IOM" && poDtlList.Any(a => a.DOC_TYPE == "IOM"))
                //        {
                //            poDtlList.RemoveAll(r => r.DOC_TYPE == formData.DOC_TYPE);
                //        }
                //        FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE);
                //        //string path = Server.MapPath("~/Uploads/IOM/");
                //        //if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                //        //if (!string.IsNullOrEmpty(_file.FileName) && _file.File != null)
                //        //{
                //        //            System.IO.File.WriteAllBytes(path + _file.FileName, _file.File.ToArray());
                //        //}
                //        poDtlList.Add(new IOMDetailViewModel
                //        {
                //            IOMHEADERID = formData.IOMHEADERID,
                //            FILENAME = _file.FileName,
                //            FILE_CONTENTTYPE = _file.FileContentType,
                //            FILE_BYTE = _file.File,
                //            DOC_TYPE = formData.DOC_TYPE,
                //            ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                //            ADDEDBY = Convert.ToInt64(_userId.ToString()),
                //        });

                //        TempData["IOM_ATTACHMENT_LIST"] = poDtlList;
                //        retVal = 1;
                //    }
                //}
            }
            catch (Exception ex)
            {
                retVal = -1;
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.DOC_TYPE != "IOM").ToList() });
        }

        [HttpPost]
        public ActionResult DeleteAttachment(string fileName, string docType, long iomHeaderId)
        {
            short retVal = 0;
            List<IOMDetailViewModel> poDtlList = new List<IOMDetailViewModel>();
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (iomHeaderId > 0)
                {
                    Tuple<short, List<IOMDetailViewModel>> _ret_tuple = _IomService.DeleteAttachment(fileName, docType, iomHeaderId);
                    retVal = _ret_tuple.Item1;
                    poDtlList = _ret_tuple.Item2;
                    if (retVal == 1)
                    {
                        //string path = Server.MapPath("~/Uploads/DGIT_IOM/");
                        string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (System.IO.File.Exists(System.IO.Path.Combine(path, fileName)))
                        {
                            System.IO.File.Delete(System.IO.Path.Combine(path, fileName));
                        }
                    }
                }
                //else
                //{
                //    if (TempData["IOM_ATTACHMENT_LIST"] != null)
                //    {
                //        poDtlList = (List<IOMDetailViewModel>)TempData["IOM_ATTACHMENT_LIST"];
                //    }
                //    List<IOMDetailViewModel> newPoDTLList = poDtlList.Where(a => a.FILENAME != fileName).ToList();
                //    poDtlList = new List<IOMDetailViewModel>(newPoDTLList);
                //    retVal = 1;
                //    TempData["IOM_ATTACHMENT_LIST"] = poDtlList;
                //}
            }
            catch (Exception ex)
            {
                retVal = -1;
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.DOC_TYPE != "IOM").ToList() });
        }

        [HttpGet]
        public ActionResult EditIOMRequest(string id)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id))); // Server.UrlDecode(Encryption.Decrypt(id))
                IOMHeaderViewModel PHVM = _IomService.GetIOMRequestById(_ReqId);
                if (PHVM.iomAppHeaderList != null)
                {
                    List<IOMAppHeaderViewModel> _appheaderList = new List<IOMAppHeaderViewModel>();
                    int _seqOrder = 0;
                    foreach (IOMAppHeaderViewModel _obj in PHVM.iomAppHeaderList)
                    {
                        _seqOrder = _seqOrder + 1;
                        _obj.Seq_Order = _seqOrder;
                        _appheaderList.Add(_obj);
                    }
                    PHVM.iomAppHeaderList = _appheaderList;
                }

                if (PHVM.iomAuthSeq.Count == 0)
                {
                    PHVM.iomAuthSeq = new List<IOMAppAuthSeqViewModel>(); // _IomService.GetDefaultAuthority(Convert.ToInt64(Session["UserId"].ToString()));
                }
                if (PHVM.iomAuthSeq.Count() > 0)
                {
                    List<IOMAppAuthSeqViewModel> headerlst = new List<IOMAppAuthSeqViewModel>();
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 1, Header = "Section Manager" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 2, Header = "Department Manager" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 3, Header = "Coordinator" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 4, Header = "Division Head" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 5, Header = "Executive Coordinator" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 6, Header = "Operating Head" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 7, Header = "Executive Internal Auditor" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 8, Header = "Executive Vice President" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 9, Header = "Director" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 10, Header = "Senior Director" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 11, Header = "Chief Production Officer" });
                    headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 12, Header = "President & CEO" });
                    foreach (var item in PHVM.iomAuthSeq)
                    {
                        var hdrlst = headerlst.Where(m => m.Header == item.Header);
                        Int16 app_seq = 0;
                        if (hdrlst.Count() > 0 && PHVM.APP_TYPE == 1)
                        {
                            app_seq = headerlst.Where(m => m.Header == item.Header).FirstOrDefault().APP_SEQ;
                            item.APP_SEQ = app_seq;
                        }
                        else
                        {
                            app_seq = Convert.ToInt16(item.APP_SEQ);
                            item.APP_SEQ = app_seq;
                        }

                    }
                }
                TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(PHVM.iomAuthSeq.OrderBy(o => o.APP_SEQ).ToList());
                TempData.Keep();
                //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - Start
                Employee_Details employeeDetails = _EmpDetails;
                ViewBag.CategoryList = new SelectList(_IomService.BindIOMCategory(employeeDetails.Division_Id), "IOMCATMSTID", "CATDESC");
                //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - End
                ViewBag.strId = id;
                return View(PHVM);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        [HttpPost]
        public ActionResult EditIOMRequest([FromBody] IOMHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.iomDetail = new List<IOMDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_userId.ToString());
                model.UPDATEDBY = Convert.ToInt64(_userId.ToString());

                Tuple<short, long> retVal_tuple = _IomService.SaveIOMRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch (Exception ex)
            {
                retVal = -1;
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult IOMApproval(string id)
        {
            try
            {


                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long _ReqId = Convert.ToInt64(id); // Server.UrlDecode(Encryption.Decrypt(id))
                IOMHeaderViewModel obj = _IomService.GetIOMRequestById(_ReqId);
                long userid = Convert.ToInt64(_userId.ToString());
                //Changed by TTL on 26-June-2025 against SR101846 > CR6625 - Start
                int isEnable = 0;
                var lastRecordInHistory = obj.iomAppHis.OrderByDescending(x => x.IOMAPPHISTORY_ID).FirstOrDefault();
                if (lastRecordInHistory != null)
                {
                    if (lastRecordInHistory.ADEMPCODE == userid && (lastRecordInHistory.APPROVAL_STATUS == 0 || lastRecordInHistory.APPROVAL_STATUS == 5) && lastRecordInHistory.IOMAPPHISTORY_ID != 0)
                    {
                        isEnable = 1;
                    }
                }
                //obj.ISENABLE = obj.iomAppHis.Where(a => a.ADEMPCODE == userid && (a.APPROVAL_STATUS == 0  || a.APPROVAL_STATUS ==5 ) && a.IOMAPPHISTORY_ID != 0).Count().ToString(); //Commented by TTL on 26-June-2025 against SR101846 > CR6625
                obj.ISENABLE = isEnable.ToString();
                //Added by TTL on 26-June-2025 against SR101846 > CR6625 - End
                return View("IOMApproval", obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> IOMApproval(IOMAppHistoryViewModel PHVM)
        {
            short retVal = 0;
            long IOMIDNext = 0;
            string nextIomId = string.Empty;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _EmpDetails;
                PHVM.UPDATEBY = Convert.ToInt64(_userId.ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_userId.ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_userId.ToString());

                //Changed by TTL on 16-June-2025 against SR99343 > CR6531 - Start
                //Validate if the approver is currently the correct approver

                long _ReqId = PHVM.IOMID;
                IOMHeaderViewModel iomObj = _IomService.GetIOMRequestById(_ReqId);
                long userid = Convert.ToInt64(_userId.ToString());
                var lastRecordInHistory = iomObj.iomAppHis.OrderByDescending(x => x.IOMAPPHISTORY_ID).FirstOrDefault();

                //Changed by TTL on 26-June-2025 against SR101846 > CR6625 - Start
                bool isEligibleForApproval = false;
                if (lastRecordInHistory != null)
                {
                    if (lastRecordInHistory.ADEMPCODE == userid && (lastRecordInHistory.APPROVAL_STATUS == 0 || lastRecordInHistory.APPROVAL_STATUS == 5) && lastRecordInHistory.IOMAPPHISTORY_ID != 0)
                    {
                        isEligibleForApproval = true;
                    }
                }
                //bool isEligibleForApproval  = iomObj.iomAppHis.OrderByDescending(x=> x.IOMAPPHISTORY_ID).Where(a => a.ADEMPCODE == userid && (a.APPROVAL_STATUS == 0  || a.APPROVAL_STATUS ==5 ) && a.IOMAPPHISTORY_ID != 0).Any(); //Commented by TTL on 26-June-2025 against SR101846 > CR6625
                if (!isEligibleForApproval)
                {
                    retVal = 2;
                    return Json(new {
                        status = retVal,
                        iomid = "0"
                    });
                }
                //Changed by TTL on 26-June-2025 against SR101846 > CR6625 - End

                //Get the next approval id before approval
                IOMIDNext = _IomService.GetIOMNextApprovalId(PHVM.IOMID, userid);
                nextIomId = IOMIDNext.ToString();

                //Changed by TTL on 16-June-2025 against SR99343 > CR6531 - End
                retVal = _IomService.IOMApproval(PHVM, _Employee_Details);
                if (retVal == 1)
                {
                    IOMHeaderViewModel IOM_Dtl = _IomService.GetIOMRequestById(PHVM.IOMID);
                    if (IOM_Dtl.PROCESS_STATUS == 2)
                    {
                        if (IOM_Dtl != null)
                        {
                            var OBJAPP = IOM_Dtl.iomAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                            if (OBJAPP == null)
                            {
                                var objiomfile = IOM_Dtl.iomDetail.Where(m => m.DOC_TYPE == "IOM");
                                if (objiomfile == null || objiomfile.Count() == 0)
                                {
                                    throw new Exception("IOM File Not Found");
                                }
                                string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                                //string srcFile = Server.MapPath("~/Uploads/DGIT_IOM/") + objiomfile.FirstOrDefault().FILENAME;
                                string srcFile = Path.Combine(path, objiomfile.FirstOrDefault().FILENAME);
                                string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                                string STRFILENAME = pathtosave + @"\" + "IOM_" + PHVM.IOMID + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                                //string strDstFile = Server.MapPath("~/Uploads/DGIT_IOM/") + STRFILENAME;
                                string strDstFile = Path.Combine(path, STRFILENAME);


                                List<IOMAppAuthSeqViewModel> iomAuthSeq = IOM_Dtl.iomAuthSeq;
                                List<IOMSIGLIST> DocSiglist = new List<IOMSIGLIST>();
                                if (iomAuthSeq.Count > 0)
                                {
                                    var DesgList = iomAuthSeq.Select(m => m.Header).Distinct();
                                    foreach (var objd in DesgList)
                                    {
                                        IOMSIGLIST objsig = new IOMSIGLIST();
                                        List<IOMAppHistoryViewModel> objapplist = new List<IOMAppHistoryViewModel>();
                                        objsig.Designation = objd.ToString();

                                        foreach (var authlist in iomAuthSeq.Where(m => m.Header == objsig.Designation).ToList())
                                        {
                                            DateTime appdate = IOM_Dtl.iomAppHis.Where(m => m.ADEMPCODE == authlist.ADEMPCODE).Max(m => m.APPROVALDATE).Value;
                                            objapplist.Add(new IOMAppHistoryViewModel { APPEMP_NAME = authlist.ADEMPNAME, APPROVALDATE = appdate });
                                        }
                                        objsig.appList = objapplist;
                                        DocSiglist.Add(objsig);
                                    }
                                }

                                string _date = IOM_Dtl.DATEADDED != null ? IOM_Dtl.DATEADDED.ToString("dd-MMM-yyyy") : "";
                                //IOMAnnotationPdf(srcFile, strDstFile, DocSiglist);

                                //Add Initiator Heading and person Name - Added by Bhupesh NTT for CR-5233.  
                                //Thread bgThread = new Thread(async () =>
                                //{
                                await IOMAnnotationPdf(PHVM, _Employee_Details, srcFile, strDstFile, DocSiglist, IOM_Dtl.ADDEDBYNAME, _date, false);
                                //});
                                //bgThread.Start();

                                string straddinfo = "Approved IOM";
                                var objiomafile = IOM_Dtl.iomDetail.Where(m => m.DOC_TYPE == "IOMA" && m.IOMHEADERID == PHVM.IOMID);
                                if (objiomafile.Count() > 0)
                                {
                                    straddinfo = "Extended Approval Document";
                                }
                                List<IOMDetailViewModel> obj = new List<IOMDetailViewModel>() { new IOMDetailViewModel() { DOC_TYPE = "IOMA", FILENAME = STRFILENAME, ADDITIONAL_INFO = straddinfo } };
                                _IomService.SaveAttachment(PHVM.ADDEDBY, PHVM.IOMID, obj);

                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                retVal = -1;
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(new {
                        status = retVal,
                        iomid = nextIomId
                    });
        }

        [HttpGet]
        public ActionResult IOMCancel(string id)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id))); //(Server.UrlDecode(Encryption.Decrypt(id)));
            return View("IOMCancel", _IomService.GetIOMRequestById(_ReqId));
        }

        [HttpPost]
        public ActionResult IOMCancel([FromBody] IOMHeaderViewModel PHVM)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _EmpDetails;
                PHVM.UPDATEDBY = Convert.ToInt64(_userId.ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_userId.ToString());
                retVal = _IomService.IOMCancel(PHVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult GetAppAuthority([FromQuery] string eCode, [FromQuery] int headerId, [FromQuery] string header)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Login_Employee_Details = _EmpDetails;
                List<IOMAppAuthSeqViewModel> AuthSeqList = new List<IOMAppAuthSeqViewModel>();

                string[] splitString = eCode.Split('-');
                long empCode = Convert.ToInt64(splitString[0].Trim());

                Employee_Details employee_dtl = _IomService.GetAuthEmpById(empCode, _Login_Employee_Details);
                if (employee_dtl == null) { employee_dtl = new Employee_Details(); }

                if (TempData["APPROVAL_AUTH_LIST"] != null)
                {
                    //AuthSeqList = (List<IOMAppAuthSeqViewModel>)TempData["APPROVAL_AUTH_LIST"];
                    AuthSeqList = JsonConvert.DeserializeObject<List<IOMAppAuthSeqViewModel>>(TempData["APPROVAL_AUTH_LIST"].ToString());
                }

                if (employee_dtl._ECode > 0 && !string.IsNullOrEmpty(employee_dtl._EName))
                {
                    AuthSeqList.Add(new IOMAppAuthSeqViewModel
                    {
                        ADEMPCODE = employee_dtl._ECode,
                        ADEMPNAME = employee_dtl._EName,
                        ADDESIGNATION = employee_dtl._Desig,
                        Header = header,
                        APP_SEQ = Convert.ToInt16(headerId),
                        APPTYPE = 1,
                        FNDESID = Convert.ToInt16(employee_dtl._FnDesigId == null ? 0 : employee_dtl._FnDesigId),
                    });
                }
                //TempData["APPROVAL_AUTH_LIST"] = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();
                var AuthSeqListOrdrBy = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();
                TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqListOrdrBy);

                return Json(new
                {
                    ECODE = employee_dtl._ECode,
                    ENAME = employee_dtl._EName,
                    SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new
                {
                    ECODE = 0,
                    ENAME = "",
                    SEQ_LIST = new List<IOMAppAuthSeqViewModel>()
                });
            }
        }

        public ActionResult DeleteAppAuthority([FromQuery] string eCode, [FromQuery] string header, [FromQuery] string ISALL)
        {
            short retval = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<IOMAppAuthSeqViewModel> AuthSeqList = new List<IOMAppAuthSeqViewModel>();

                if (ISALL == "0")
                {
                    if (TempData["APPROVAL_AUTH_LIST"] != null)
                    {
                        AuthSeqList = JsonConvert.DeserializeObject<List<IOMAppAuthSeqViewModel>>(TempData["APPROVAL_AUTH_LIST"].ToString());
                    }

                    if (AuthSeqList.Count > 0)
                    {
                        AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode) && r.Header == header);
                        var AuthSeqListOrdrBy = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();
                        TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqListOrdrBy);
                        retval = 1;
                    }
                }
                else
                {
                    TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqList);
                    TempData["Designation"] = JsonConvert.SerializeObject("");
                    TempData.Keep();
                    retval = 1;
                }
                return Json(new { RESULT = retval, SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new { RESULT = -1 });
            }
        }

        [HttpGet]
        public ActionResult AddApprovalHeader()
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return PartialView("_ApprovalHeader");
        }

        private FileViewModel GetUploadFile(IFormFile file, string DocType, string headerid)
        {
            try
            {
                FileViewModel FVM = new FileViewModel();
                if (file != null && file.Length > 0)
                {
                    byte[] bytes;
                    //using (BinaryReader br = new BinaryReader(file.InputStream))
                    //{
                    //    bytes = br.ReadBytes(file.ContentLength);
                    //}
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        bytes = ms.ToArray();
                    }
                    //string _FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
                    string _FileName = Path.GetFileName(file.FileName);
                    string strExtensionName = Path.GetExtension(file.FileName);
                    //FVM.FileContentType = MimeMapping.GetMimeMapping(_FileName);
                    FVM.FileContentType = file.ContentType;
                    FVM.FileName = DocType + headerid + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + strExtensionName;
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

        public ActionResult GetPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/DGIT_IOM/"; //// Server.MapPath("~/Uploads/PO/");
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"550px\"></object>";
                string _path = string.Format(embed, file_path + fileName);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }
        public ActionResult GetTwoPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/DGIT_IOM/"; //// Server.MapPath("~/Uploads/PO/");
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, file_path + fileName);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }
        public ActionResult GetMultiPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/DGIT_IOM/"; //// Server.MapPath("~/Uploads/PO/");
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"260px\"></object>";
                string _path = string.Format(embed, file_path + fileName);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }
        public ActionResult IOMMultiDocsView(long id)
        {
            IOMHeaderViewModel obj = _IomService.GetIOMRequestById(id);
            IOMDetailViewModel item = new IOMDetailViewModel();
            item.FILENAME = "Select";
            obj.iomDetail.Insert(0, item);

            return View("IOMMultiDocsView", obj.iomDetail);
        }
        public ActionResult IOMTwoDocsView(long id)
        {
            IOMHeaderViewModel obj = _IomService.GetIOMRequestById(id);
            IOMDetailViewModel item = new IOMDetailViewModel();
            item.FILENAME = "Select";
            obj.iomDetail.Insert(0, item);


            return View("IOMTwoDocsView", obj.iomDetail);
        }
        public ActionResult DownloadAttachment(string fileName)
        {
            try
            {
                //string file_path = Server.MapPath("~/Uploads/DGIT_IOM/");
                string file_path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                if (!Directory.Exists(file_path)) { Directory.CreateDirectory(file_path); }
                //byte[] fileBytes = System.IO.File.ReadAllBytes(file_path + fileName);
                byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(file_path, fileName));
                if (fileBytes.Length > 0)
                {
                    //return File(fileBytes, MimeMapping.GetMimeMapping(fileName), fileName);
                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(fileName, out var contentType))
                    {
                        contentType = "application/octet-stream"; // Default fallback
                    }
                    return File(fileBytes, contentType, fileName);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }
        public string AutocompleteSuggestions(string term)
        {
            string designation = "";

            try
            {
                if (TempData.ContainsKey("Designation"))
                {
                    if (TempData["Designation"] != null && TempData["Designation"].ToString() != "")
                    {
                        designation = JsonConvert.DeserializeObject<string>(TempData["Designation"].ToString());
                        if (designation == "-Select-")
                            designation = "";
                    }
                    TempData.Keep();
                }
            }
            catch (Exception ex) { }
            List<Employee_Details> portaluser = _IomService.PortalAutocompleteSuggestions(term, designation);
            //List<PortalUser> data = new List<PortalUser>();
            int i = 0;
            List<string> list = new List<string>();
            foreach (var dataitem in portaluser)
            {
                list.Add(dataitem._ECode.ToString() + "-" + dataitem._EFirstName.ToString() + " " + dataitem._ELastName.ToString() + "");
            }
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(list);
            string sJSON = JsonConvert.SerializeObject(list);
            return sJSON;
        }
        public string AutocompleteDesignation(string term)
        {
            List<Employee_Details> portaluser = _IomService.AutocompleteDesignation(term);
            List<string> list = new List<string>();
            foreach (var dataitem in portaluser)
            {
                list.Add(dataitem._Desig.ToString() + "");
            }
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(list);
            string sJSON = JsonConvert.SerializeObject(list);
            return sJSON;
        }
        [HttpPost]
        public ActionResult PreviewIOM([FromBody] IOMHeaderViewModel oHeader)
        {
            try
            {
                string fileName = oHeader.IOMATTACHMENT;
                IOMHeaderViewModel OHDR = _IomService.GetIOMRequestById(oHeader.IOMHEADERID);
                List<IOMAppAuthSeqViewModel> iomAuthSeq = new List<IOMAppAuthSeqViewModel>();//OHDR.iomAuthSeq;
                //foreach (var oh in oHeader.iomAuthSeq)
                //{
                //    oh.APP_SEQ=
                //}
                if (oHeader.iomAuthSeq.Count() > 0)
                { iomAuthSeq.AddRange(oHeader.iomAuthSeq); }
                else
                {
                    if (OHDR.iomAuthSeq.Count > 0)
                    {
                        iomAuthSeq.AddRange(OHDR.iomAuthSeq);
                    }
                }



                List<IOMSIGLIST> DocSiglist = new List<IOMSIGLIST>();
                if (iomAuthSeq.Count > 0)
                {
                    var DesgList = iomAuthSeq.Select(m => m.Header).Distinct();
                    foreach (var obj in DesgList)
                    {
                        IOMSIGLIST objsig = new IOMSIGLIST();
                        List<IOMAppHistoryViewModel> objapplist = new List<IOMAppHistoryViewModel>();
                        objsig.Designation = obj.ToString();

                        foreach (var authlist in iomAuthSeq.Where(m => m.Header == objsig.Designation).ToList())
                        {
                            objapplist.Add(new IOMAppHistoryViewModel { APPEMP_NAME = authlist.ADEMPNAME, APPROVALDATE = DateTime.Now });
                        }
                        objsig.appList = objapplist;
                        DocSiglist.Add(objsig);
                    }
                }

                //Temp
                string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                //string srcPath = Server.MapPath("~/Uploads/DGIT_IOM/") + fileName;
                string srcPath = Path.Combine(path, fileName);
                string destfilename = "IOM" + oHeader.IOMHEADERID + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                //string destPath = Server.MapPath("~/Uploads/DGIT_IOM/Temp/") + destfilename;
                var path2 = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM", "Temp");
                string destPath = Path.Combine(path2, destfilename);
                if (!Directory.Exists(path2)) { Directory.CreateDirectory(path2); }
                //Changed by TTL on 07-Aug-2025 against SR102253 > CR6697 - Start
                string _date = OHDR.DATEADDED != null ? OHDR.DATEADDED.ToString("dd-MMM-yyyy") : "";  //Changes by TTL - Initiator Name Not Bind
                //IOMAnnotationPdfPreview(srcPath, destPath, DocSiglist);
                IOMAnnotationPdfPreview(srcPath, destPath, DocSiglist, OHDR.ADDEDBYNAME, _date);
                //Changed by TTL on 07-Aug-2025 against SR102253 > CR6697 - End
                string file_path = "../../../Uploads/DGIT_IOM/Temp/";
                //string file_path = Path.Combine("DGIT_IOM", "Temp");
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"288px\"></object>";
                string _path = string.Format(embed, file_path + destfilename);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }

        //Changed by TTL on 07-Aug-2025 against SR102253 > CR6697 - Start
        //Changed by TTL on 10-Sep-2025 against CR6974
        private async Task IOMAnnotationPdf(IOMAppHistoryViewModel IHVM, Employee_Details emp_dtl, string srcPath, string dstPath, List<IOMSIGLIST> objlist, string initiatorName = "", string _date = "", bool forRegenerate = true)
        {
            var writer = new PdfWriter(dstPath);
            var pdf = new PdfDocument(new PdfReader(srcPath), writer);
            var doc = new Document(pdf);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            float xOffset = 10f;
            float bottomMargin = 40f;
            float spacingBetweenTables = 10f;

            var tables = new List<Table>();
            int index = 0;
            int batch = 0;

            // Loop through batches based on your rules
            while (index < objlist.Count)
            {
                // First row = 12 approvers, rest = up to 13
                int takeCount = (batch == 0)
                    ? Math.Min(12, objlist.Count) // first batch only
                    : Math.Min(13, objlist.Count - index);

                var batchList = objlist.Skip(index).Take(takeCount).ToList();
                index += takeCount;

                // Column count: first row gets initiator column extra
                int cols = (batch == 0 ? batchList.Count + 1 : batchList.Count);
                float[] widths = Enumerable.Repeat(5f, cols).ToArray();

                Table tbl = new Table(UnitValue.CreatePercentArray(widths))
                    .SetFont(font)
                    .SetFontSize(6);

                if (batch == 0)
                {
                    // Initiator column header
                    tbl.AddHeaderCell(new Cell()
                        .Add(new Paragraph("Initiator"))
                        .SetFontSize(7)
                        .SetBackgroundColor(new DeviceGray(0.75f))
                        .SetTextAlignment(TextAlignment.CENTER));

                    // Initiator name/date cell
                    tbl.AddCell(new Cell()
                        .Add(new Paragraph($"{initiatorName}\n{_date}"))
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                // Approver headers
                foreach (var sig in batchList)
                {
                    tbl.AddHeaderCell(new Cell()
                        .Add(new Paragraph(sig.Designation))
                        .SetFontSize(7)
                        .SetBackgroundColor(new DeviceGray(0.75f))
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                // Approver details
                foreach (var sigList in batchList)
                {
                    string content = string.Join("\n", sigList.appList
                        .Select(s => $"{s.APPEMP_NAME}\n{s.APPROVALDATE?.ToString("dd-MMM-yyyy")}"));
                    tbl.AddCell(new Cell()
                        .Add(new Paragraph(content))
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                tables.Add(tbl);
                batch++;
            }

            // Render tables to PDF pages bottom-up
            int pageCount = pdf.GetNumberOfPages();
            for (int p = 1; p <= pageCount; p++)
            {
                PdfPage page = pdf.GetPage(p);
                float pageWidth = page.GetPageSize().GetWidth();
                float pageHeight = page.GetPageSize().GetHeight();
                Canvas canvas = new Canvas(page, page.GetPageSize());

                float currentYOffset = bottomMargin;

                for (int b = tables.Count - 1; b >= 0; b--)
                {
                    Table tbl = tables[b];

                    // Measure table height
                    var dummyDoc = new Document(pdf);
                    var renderer = (TableRenderer)tbl.CreateRendererSubTree();
                    renderer.SetParent(dummyDoc.GetRenderer());
                    var layoutResult = renderer.Layout(
                        new LayoutContext(new LayoutArea(0, new iText.Kernel.Geom.Rectangle(0, 0, pageWidth, pageHeight)))
                    );
                    float estimatedHeight = layoutResult.GetOccupiedArea().GetBBox().GetHeight();

                    if (currentYOffset + estimatedHeight > pageHeight - 20f)
                        continue;

                    tbl.SetFixedPosition(xOffset, currentYOffset, pageWidth - 2 * xOffset);
                    canvas.Add(tbl);
                    currentYOffset += estimatedHeight + spacingBetweenTables;
                }
            }

            doc.Close();
            pdf.Close();
            await Task.FromResult(0);

            if (!forRegenerate)
            {
                _IomService.SendAnnotatedPdfOnFinalApproval(IHVM, emp_dtl, dstPath);
            }
        }
        //Changed by TTL on 10-Sep-2025 against CR6974
        //Changed by TTL on 07-Aug-2025 against SR102253 > CR6697 - End

        //Changed by TTL on 10-Sep-2025 against CR6974
        private void IOMAnnotationPdfPreview(string srcPath, string dstPath, List<IOMSIGLIST> objlist, string initiatorName = "", string _date = "")
        {
            var writer = new PdfWriter(dstPath);
            var pdf = new PdfDocument(new PdfReader(srcPath), writer);
            var doc = new Document(pdf);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            float xOffset = 10f;
            float bottomMargin = 40f;
            float spacingBetweenTables = 10f;

            var tables = new List<Table>();

            // Index to track our place in the approvers list
            int index = 0;
            int batch = 0;

            while (index < objlist.Count)
            {
                int takeCount = (batch == 0) ? Math.Min(12, objlist.Count) : Math.Min(13, objlist.Count - index);
                var batchList = objlist.Skip(index).Take(takeCount).ToList();
                index += takeCount;

                // Column count (extra column for initiator in first row)
                int cols = (batch == 0 ? batchList.Count + 1 : batchList.Count);
                float[] widths = Enumerable.Repeat(5f, cols).ToArray();

                Table tbl = new Table(UnitValue.CreatePercentArray(widths))
                    .SetFont(font)
                    .SetFontSize(6);

                // Add header & cell for initiator in first row
                if (batch == 0)
                {
                    tbl.AddHeaderCell(new Cell()
                        .Add(new Paragraph("Initiator"))
                        .SetFontSize(7)
                        .SetBackgroundColor(new DeviceGray(0.75f))
                        .SetTextAlignment(TextAlignment.CENTER));

                    tbl.AddCell(new Cell()
                        .Add(new Paragraph($"{initiatorName}\n{_date}"))
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                // Header cells for approvers
                foreach (var sig in batchList)
                {
                    tbl.AddHeaderCell(new Cell()
                        .Add(new Paragraph(sig.Designation))
                        .SetFontSize(7)
                        .SetBackgroundColor(new DeviceGray(0.75f))
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                // Data cells for approvers
                foreach (var sigList in batchList)
                {
                    string content = string.Join("\n", sigList.appList
                        .Select(s => $"{s.APPEMP_NAME}\n{s.APPROVALDATE?.ToString("dd-MMM-yyyy")}"));
                    tbl.AddCell(new Cell()
                        .Add(new Paragraph(content))
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                tables.Add(tbl);
                batch++;
            }

            int pageCount = pdf.GetNumberOfPages();
            for (int p = 1; p <= pageCount; p++)
            {
                PdfPage page = pdf.GetPage(p);
                float pageWidth = page.GetPageSize().GetWidth();
                float pageHeight = page.GetPageSize().GetHeight();

                /* Modified by TTL on 25-Nov-2025 against SR111158 > CR7505 | Start */
                try
                {
                    using (var watermarkCanvas = new Canvas(page, page.GetPageSize()))
                    {
                        PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                        var watermark = new Paragraph("PREVIEW")
                            .SetFont(boldFont)
                            .SetFontColor(ColorConstants.LIGHT_GRAY)
                            .SetFontSize(Math.Min(pageWidth, pageHeight) * 0.18f);
                        try
                        {
                            watermark.SetOpacity(0.15f);
                        }
                        catch { }
                        watermarkCanvas.ShowTextAligned(
                                    watermark,
                                    pageWidth / 2f,
                                    pageHeight / 2f,
                                    p,
                                    TextAlignment.CENTER,
                                    VerticalAlignment.MIDDLE,
                                    (float)(Math.PI / 4) // 45 degrees in radians
                                );
                    }
                }
                catch{}
                /* Modified by TTL on 25-Nov-2025 against SR111158 > CR7505 | End */

                Canvas canvas = new Canvas(page, page.GetPageSize());

                float currentYOffset = bottomMargin;

                for (int b = tables.Count - 1; b >= 0; b--)
                {
                    Table tbl = tables[b];

                    var dummyDoc = new Document(pdf);
                    var renderer = (TableRenderer)tbl.CreateRendererSubTree();
                    renderer.SetParent(dummyDoc.GetRenderer());
                    var layoutResult = renderer.Layout(
                        new LayoutContext(new LayoutArea(0, new iText.Kernel.Geom.Rectangle(0, 0, pageWidth, pageHeight)))
                    );
                    float estimatedHeight = layoutResult.GetOccupiedArea().GetBBox().GetHeight();

                    if (currentYOffset + estimatedHeight > pageHeight - 20f)
                        continue;

                    tbl.SetFixedPosition(xOffset, currentYOffset, pageWidth - 2 * xOffset);
                    canvas.Add(tbl);
                    currentYOffset += estimatedHeight + spacingBetweenTables;
                }
            }

            doc.Close();
            pdf.Close();
        }
        //Changed by TTL on 10-Sep-2025 against CR6974

        //Changed by TTL on 07-Aug-2025 against SR102253 > CR6697 - End
        public string Designation(string term)
        {
            TempData["Designation"] = JsonConvert.SerializeObject(term);

            return term;
        }
        [HttpGet]
        public ActionResult IOMNextApproval(string IOMID_PARAM)
        {
            string retVal = "";
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long userid = Convert.ToInt64(_userId.ToString());
                long IOMidcurr = 0, IOMIDNext;
                if (!string.IsNullOrEmpty(IOMID_PARAM))
                {
                    IOMidcurr = Convert.ToInt64(IOMID_PARAM);
                    IOMIDNext = _IomService.GetIOMNextApprovalId(IOMidcurr, userid);
                    retVal = IOMIDNext == 0 ? "" : IOMIDNext.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                retVal = "";
            }
            return Json(retVal);
        }

        [HttpPost]
        public async Task<ActionResult> ReGenerateDoc([FromBody] RegenerateDocModel model)
        {
            short retVal = 0;
            string Error = "";
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                IOMHeaderViewModel IOM_Dtl = _IomService.GetIOMRequestById(Convert.ToInt64(model.id));
                if (IOM_Dtl != null)
                {
                    if (IOM_Dtl.PROCESS_STATUS == 2 && IOM_Dtl.iomDetail.Where(m => m.DOC_TYPE == "IOMA").Count() != IOM_Dtl.iomAuthSeq.Select(m => m.APPTYPE).Distinct().Count())
                    {
                        var OBJAPP = IOM_Dtl.iomAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (OBJAPP == null)
                        {
                            var objiomfile = IOM_Dtl.iomDetail.Where(m => m.DOC_TYPE == "IOM");
                            if (objiomfile == null || objiomfile.Count() == 0)
                            {
                                throw new Exception("IOM File Not Found");
                            }
                            string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                            //string srcFile = Server.MapPath("~/Uploads/DGIT_IOM/") + objiomfile.FirstOrDefault().FILENAME;
                            string srcFile = Path.Combine(path, objiomfile.FirstOrDefault().FILENAME);
                            string STRFILENAME = "IOM_" + IOM_Dtl.IOMHEADERID + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                            //string strDstFile = Server.MapPath("~/Uploads/DGIT_IOM/") + STRFILENAME;
                            string strDstFile = Path.Combine(path, STRFILENAME);

                            List<IOMAppAuthSeqViewModel> iomAuthSeq = IOM_Dtl.iomAuthSeq;
                            List<IOMSIGLIST> DocSiglist = new List<IOMSIGLIST>();
                            if (iomAuthSeq.Count > 0)
                            {
                                var DesgList = iomAuthSeq.Select(m => m.Header).Distinct();
                                foreach (var objd in DesgList)
                                {
                                    IOMSIGLIST objsig = new IOMSIGLIST();
                                    List<IOMAppHistoryViewModel> objapplist = new List<IOMAppHistoryViewModel>();
                                    objsig.Designation = objd.ToString();

                                    foreach (var authlist in iomAuthSeq.Where(m => m.Header == objsig.Designation).ToList())
                                    {
                                        DateTime appdate = IOM_Dtl.iomAppHis.Where(m => m.ADEMPCODE == authlist.ADEMPCODE).Max(m => m.APPROVALDATE).Value;
                                        objapplist.Add(new IOMAppHistoryViewModel { APPEMP_NAME = authlist.ADEMPNAME, APPROVALDATE = appdate });
                                    }
                                    objsig.appList = objapplist;
                                    DocSiglist.Add(objsig);
                                }
                            }

                            //await IOMAnnotationPdf(srcFile, strDstFile, DocSiglist);

                            //Calling IOMAnnotationPdf in a separate thread // TTL 11.02.25
                            //Changed by TTL on 27-June-2025 against SR101846 > CR-6625 - Start
                            //Thread bgThread = new Thread(async () => await IOMAnnotationPdf(null, null, srcFile, strDstFile, DocSiglist, "", "", true));
                            string _date = IOM_Dtl.DATEADDED != null ? IOM_Dtl.DATEADDED.ToString("dd-MMM-yyyy") : "";
                            Thread bgThread = new Thread(async () => await IOMAnnotationPdf(null, null, srcFile, strDstFile, DocSiglist, IOM_Dtl.ADDEDBYNAME, _date, true));
                            //Changed by TTL on 27-June-2025 against SR101846 > CR-6625 - End
                            bgThread.Start();

                            List<IOMDetailViewModel> obj = new List<IOMDetailViewModel>() { new IOMDetailViewModel() { DOC_TYPE = "IOMA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "Approved IOM" } };
                            _IomService.SaveAttachment(Convert.ToInt64(_userId.ToString()), IOM_Dtl.IOMHEADERID, obj);
                            retVal = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                Error = ex.InnerException.ToString();
                retVal = -1;
            }
            return Json(new { res = retVal, err_msg = Error });
        }

        public ActionResult IOMAdditinalApproval(string id)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId;
            try
            {
                TempData["APPROVAL_AUTH_LIST"] = null;
                List<IOMAppAuthSeqViewModel> iList = new List<IOMAppAuthSeqViewModel>();
                if (TempData["APPROVAL_AUTH_LIST"] == null)
                {
                    iList = new List<IOMAppAuthSeqViewModel>();
                }
                TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(iList.OrderBy(o => o.APP_SEQ).ToList());
                TempData.Keep();
                _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
                // _ReqId = Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(id))); // Server.UrlDecode(Encryption.Decrypt(id))
            }
            catch (Exception ex)
            {
                _ReqId = Convert.ToInt64(Encryption.Decrypt(id)); // Server.UrlDecode(Encryption.Decrypt(id))
            }
            var data = _IomService.GetIOMRequestById(_ReqId);
            if (data.ADDEDBY == Convert.ToInt64(_userId))
            {
                return View("IOMAdditinalApproval", _IomService.GetIOMRequestById(_ReqId));
            }
            else
            {
                IOMHeaderViewModel ohdr = new IOMHeaderViewModel();
                return View("IOMAdditinalApproval", ohdr);
            }
        }

        [HttpPost]
        public ActionResult IOMAdditinalApproval([FromBody] IOMHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.iomDetail = new List<IOMDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_userId.ToString());
                model.UPDATEDBY = Convert.ToInt64(_userId.ToString());
                model.IsFinalSubmit = 1;
                model.PROCESS_STATUS = 1;
                Tuple<short, long> retVal_tuple = _IomService.SaveAdditionalIOMRequest(model);
                retVal = retVal_tuple.Item1;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                retVal = -1;
            }
            return Json(retVal);
        }

        //Below button added by aumento as 15062023 for SR50547--------------------------------------------------------------------------------------
        [HttpGet]
        public ActionResult GetPrevAuthority([FromQuery] string AppType)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                Employee_Details _Login_Employee_Details = _EmpDetails;
                List<IOMAppAuthSeqViewModel> AuthSeqList = new List<IOMAppAuthSeqViewModel>();

                List<IOMAppAuthSeqViewModel> portaluser = _IomService.PrevAuthority(AppType, Convert.ToInt64(_userId.ToString()));

                if (portaluser == null) { portaluser = new List<IOMAppAuthSeqViewModel>(); }

                //if (TempData["APPROVAL_AUTH_LIST"] != null)
                //{
                //    AuthSeqList = (List<IOMAppAuthSeqViewModel>)TempData["APPROVAL_AUTH_LIST"];
                //}
                List<IOMAppAuthSeqViewModel> headerlst = new List<IOMAppAuthSeqViewModel>();
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 1, Header = "Section Manager" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 2, Header = "Department Manager" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 3, Header = "Coordinator" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 4, Header = "Division Head" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 5, Header = "Executive Coordinator" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 6, Header = "Operating Head" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 7, Header = "Executive Internal Auditor" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 8, Header = "Executive Vice President" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 9, Header = "Director" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 10, Header = "Senior Director" }); //Previous authority senior director added at 13072024
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 10, Header = "Chief Production Officer" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 11, Header = "President & CEO" });
                if (portaluser.Count > 0)
                {
                    foreach (var data in portaluser)
                    {
                        var hdrlst = headerlst.Where(m => m.Header == data.Header);
                        Int16 app_seq = 0;
                        if (hdrlst.Count() > 0 && AppType == "1")
                        {
                            app_seq = headerlst.Where(m => m.Header == data.Header).FirstOrDefault().APP_SEQ;
                        }
                        else
                        {
                            app_seq = Convert.ToInt16(data.APP_SEQ);
                        }
                        AuthSeqList.Add(new IOMAppAuthSeqViewModel
                        {
                            ADEMPCODE = data.ADEMPCODE,
                            ADEMPNAME = data.ADEMPNAME,
                            ADDESIGNATION = data.ADDESIGNATION,
                            Header = data.Header,
                            APP_SEQ = app_seq,
                            APPTYPE = 1,
                        });
                    }
                }
                var _authListOrdrBy = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();
                TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(_authListOrdrBy);

                return Json(new
                {
                    //ECODE = portaluser.ADEMPCODE,
                    //ENAME = portaluser.ADEMPNAME,
                    ECODE = AuthSeqList.Select(x => x.ADEMPCODE).ToList(),
                    ENAME = AuthSeqList.Select(x => x.ADEMPNAME).ToList(),
                    SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new
                {
                    ECODE = 0,
                    ENAME = "",
                    SEQ_LIST = new List<IOMAppAuthSeqViewModel>()
                });
            }

        }

        [HttpGet]
        public ActionResult IOMUserReport()
        {
            SearchIOM obj = new SearchIOM();
            obj.Status = -1;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<PR_Div_Dep_SecViewModel> _OPList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _DivList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _DptList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _SCList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _KIList = new List<PR_Div_Dep_SecViewModel>();
                Employee_Details employeeDetails = _EmpDetails;
                _KIList = _IomService.GetKiLIST(Convert.ToInt64(employeeDetails.Employee_Code));

                if (TempData["KIID"] == null || Convert.ToString(TempData["KIID"]) == "")
                {
                    obj.KIID = _KIList.FirstOrDefault().Value;
                }
                else
                {
                    obj.KIID = Convert.ToInt64(TempData["KIID"]);
                }



                Employee_Details employeeDetailski = _IomService.GetEmployeeDetail(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                obj.OperationID = employeeDetailski._OpId == null ? 0 : (long)employeeDetailski._OpId;
                obj.DivisionID = employeeDetailski._DivId == null ? 0 : (long)employeeDetailski._DivId;
                obj.DEPTID = employeeDetailski._DepId == null ? 0 : (long)employeeDetailski._DepId;
                obj.SECID = employeeDetailski._SecId == null ? 0 : (long)employeeDetailski._SecId;
                obj.IsTeamMember = (employeeDetailski.Functional_Designation_Id == "" ? 1 : 0);


                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _IomService.BindOperation(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                obj.ISSpecialRight = OPtemp.Item1;

                if (obj.IsTeamMember == 1 && obj.ISSpecialRight == 0)
                {
                    obj.ecode = Convert.ToInt64(employeeDetails.Employee_Code);
                }


                if (obj.ISSpecialRight == 1)
                {
                    _OPList = OPtemp.Item2;
                    long opid = _OPList.FirstOrDefault().Value;
                    Tuple<long, List<PR_Div_Dep_SecViewModel>> divtemp = _IomService.BindDivision(opid, Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
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
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> divtemp = _IomService.BindDivision(obj.OperationID, Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
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
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> dpttemp = _IomService.BindDepartment(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID, obj.OperationID, obj.DivisionID);
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
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> sectemp = _IomService.BindSection(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID, obj.OperationID, obj.DivisionID, obj.DEPTID);
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
                ViewBag.CategoryList = new SelectList(_IomService.BindIOMCategory(), "IOMCATMSTID", "CATDESC");

                ModelState.Clear();
                return View("IOMUserReport", obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult IOMUserReport(SearchIOM SI)
        {
            try
            {

                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SI.loginid = Convert.ToInt64(_userId.ToString());
                SearchIOM _headerObj = _IomService.IOMUserReport(SI);
                ModelState.Clear();
                return PartialView("_GetIOMUserReport", _headerObj);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return View();
            }
        }
        [HttpPost]
        public ActionResult ExportToExcelUser(SearchIOM SI)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SI.loginid = Convert.ToInt64(_userId.ToString());
                SearchIOM _headerObj = _IomService.IOMUserReport(SI);

                string str = this.ExportToExcelUserHtml(_headerObj.SearchResult);
                TempData.Remove("IOMREPORTEXCELFILE");
                //TempData["IOMREPORTEXCELFILE"] = str;
                TempData["IOMREPORTEXCELFILE"] = JsonConvert.SerializeObject(str);
                retVal = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public ActionResult DownloadExcelUser()
        {
            try
            {
                if (TempData["IOMREPORTEXCELFILE"] == null)
                {
                    return View();
                }
                //string str = (string)TempData["IOMREPORTEXCELFILE"];
                string str = JsonConvert.DeserializeObject<string>(TempData["IOMREPORTEXCELFILE"].ToString());
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=IOMReport.xls");
                Response.Headers.Add("content-disposition", "attachment; filename=IOMReport.xls");

                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        public string ExportToExcelUserHtml(List<VM_VW_DGIT_IOMREPORT> _headerList)
        {
            //Employee_Details employeeDetails = (Employee_Details)this.Session["Employee"];
            Employee_Details employeeDetails = _EmpDetails;
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Request ID</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Department</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Section</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Category</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Description</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Last Updated Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
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
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.IOMHEADERID + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.ADDEDBY + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.ADDEDBYNAME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.OPERATION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.DIVISION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.DEPARTMENT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.SECTION + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.CATDESC + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.IOM_DESC + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.DATEADDED).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.UPDATEDATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + POStatus + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        public ActionResult BindDivisionByOperationId(long id, long kiid)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<PR_Div_Dep_SecViewModel> divList = new List<PR_Div_Dep_SecViewModel>();
            if (id != 0)
            {
                Employee_Details employeeDetails = _EmpDetails;
                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _IomService.BindDivision(id, Convert.ToInt64(employeeDetails.Employee_Code), kiid);
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
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<PR_Div_Dep_SecViewModel> DepList = new List<PR_Div_Dep_SecViewModel>();
            if (Div_id != 0)
            {
                Employee_Details employeeDetails = _EmpDetails;
                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _IomService.BindDepartment(employeeDetails._ECode, KIID, op_Id, Div_id);
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
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<PR_Div_Dep_SecViewModel> SecList = new List<PR_Div_Dep_SecViewModel>();
            if (deptid != 0)
            {
                Employee_Details employeeDetails = _EmpDetails;
                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _IomService.BindSection(employeeDetails._ECode, KIID, op_Id, divid, deptid);
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

        [HttpPost]
        public ActionResult KIChange([FromBody] string KIID)
        {
            if (_userId == null)
            {
                return Json("0");
            }
            else
            {
                TempData["KIID"] = KIID;
                return Json("1");
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------


        // -> Added By Aumento 12072024
        [HttpGet]
        public ActionResult EditIOMRequestUploadBeforeHold(string id)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id))); // Server.UrlDecode(Encryption.Decrypt(id))
            IOMHeaderViewModel PHVM = _IomService.GetIOMRequestById(_ReqId);

            short HoldStatus = 5;
            DateTime HoldDateTime = PHVM.iomAppHis.Where(x => x.APPROVAL_STATUS == HoldStatus).OrderByDescending(y => y.IOMAPPHISTORY_ID).Select(x => x.APPROVALDATE).FirstOrDefault() ?? DateTime.Now;
            List<IOMDetailViewModel> iomDetail_ = new List<IOMDetailViewModel>();
            if (HoldDateTime != null)
            {

                iomDetail_ = PHVM.iomDetail.Where(x => x.ADDEDDATE > HoldDateTime).ToList();
            }
            PHVM.iomDetail = iomDetail_;
            if (PHVM.iomAppHeaderList != null)
            {
                List<IOMAppHeaderViewModel> _appheaderList = new List<IOMAppHeaderViewModel>();
                int _seqOrder = 0;
                foreach (IOMAppHeaderViewModel _obj in PHVM.iomAppHeaderList)
                {
                    _seqOrder = _seqOrder + 1;
                    _obj.Seq_Order = _seqOrder;
                    _appheaderList.Add(_obj);
                }
                PHVM.iomAppHeaderList = _appheaderList;
            }

            if (PHVM.iomAuthSeq.Count == 0)
            {
                PHVM.iomAuthSeq = new List<IOMAppAuthSeqViewModel>(); // _IomService.GetDefaultAuthority(Convert.ToInt64(Session["UserId"].ToString()));
            }
            if (PHVM.iomAuthSeq.Count() > 0)
            {
                List<IOMAppAuthSeqViewModel> headerlst = new List<IOMAppAuthSeqViewModel>();
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 1, Header = "Section Manager" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 2, Header = "Department Manager" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 3, Header = "Coordinator" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 4, Header = "Division Head" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 5, Header = "Executive Coordinator" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 6, Header = "Operating Head" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 7, Header = "Executive Internal Auditor" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 8, Header = "Executive Vice President" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 9, Header = "Director" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 10, Header = "Senior Director" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 11, Header = "Chief Production Officer" });
                headerlst.Add(new IOMAppAuthSeqViewModel() { APP_SEQ = 12, Header = "President & CEO" });
                foreach (var item in PHVM.iomAuthSeq)
                {
                    var hdrlst = headerlst.Where(m => m.Header == item.Header);
                    Int16 app_seq = 0;
                    if (hdrlst.Count() > 0 && PHVM.APP_TYPE == 1)
                    {
                        app_seq = headerlst.Where(m => m.Header == item.Header).FirstOrDefault().APP_SEQ;
                        item.APP_SEQ = app_seq;
                    }
                    else
                    {
                        app_seq = Convert.ToInt16(item.APP_SEQ);
                        item.APP_SEQ = app_seq;
                    }

                }
            }
            TempData["APPROVAL_AUTH_LIST"] = JsonConvert.SerializeObject(PHVM.iomAuthSeq.OrderBy(o => o.APP_SEQ).ToList());
            TempData.Keep();
            //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - Start
            Employee_Details employeeDetails = _EmpDetails;
            ViewBag.CategoryList = new SelectList(_IomService.BindIOMCategory(employeeDetails.Division_Id), "IOMCATMSTID", "CATDESC");
            //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - End





            ViewBag.strId = id;
            return View(PHVM);
        }

        [HttpPost]
        public ActionResult EditIOMRequestUploadBeforeHold([FromBody] IOMHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //Changed by TTL on 18-June-2025 against SR99343 > CR6531 - Start
                //Validate if the request is actually on hold
                long _ReqId = model.IOMHEADERID;
                IOMHeaderViewModel iomObj = _IomService.GetIOMRequestById(_ReqId);
                if (iomObj != null)
                {
                    if (iomObj.PROCESS_STATUS != 5)
                    {
                        retVal = -2;
                        return Json(retVal);
                    }
                }
                //Changed by TTL on 18-June-2025 against SR99343 > CR6531 - End

                Employee_Details _Employee_Details = _EmpDetails;
                model.iomDetail = new List<IOMDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_userId.ToString());
                model.UPDATEDBY = Convert.ToInt64(_userId.ToString());
                retVal = _IomService.SendMailByApprovalAuthorityForHoldRequest(model, _Employee_Details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                retVal = -1;
            }
            return Json(retVal);
        }
        // <- Added By Aumento 12072024

        //Added by Aumento for SR99176
        public ActionResult IOMAuditReport()
        {
            SearchIOM obj = new SearchIOM();
            obj.Status = -1;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<PR_Div_Dep_SecViewModel> _KIList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _OPList = new List<PR_Div_Dep_SecViewModel>();

                Employee_Details employeeDetails = _EmpDetails;
                _KIList = _IomService.GetKICodeLIst();

                if (TempData["KIID"] == null || Convert.ToString(TempData["KIID"]) == "")
                {
                    obj.KIID = _KIList.FirstOrDefault().Value;
                }
                else
                {
                    obj.KIID = Convert.ToInt64(TempData["KIID"]);
                }

                Employee_Details empdata = _IomService.GetEmployeeDetail(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                ViewBag.selectedOpId = empdata?._OpId ?? 0;
                var employeeDetailski = _IomService.GetAllOperation(obj.KIID);

                _OPList = employeeDetailski
                       .GroupBy(e => new { e._OpId, e._OpDesc })
                       .Select(g => new PR_Div_Dep_SecViewModel
                       {
                           Value = g.Key._OpId ?? 0,
                           Text = g.Key._OpDesc
                       }).ToList();


                // Empty placeholders for cascading dropdowns
                ViewBag.DivList = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.DepList = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.SecList = new SelectList(Enumerable.Empty<SelectListItem>());

                ViewBag.OPList = new SelectList(_OPList, "Value", "Text");
                ViewBag.KIList = new SelectList(_KIList, "Value", "Text");
                ViewBag.CategoryList = new SelectList(_IomService.BindIOMCategory(), "IOMCATMSTID", "CATDESC");

                return View("IOMAuditReport", obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return View(obj);
            }
        }
        [HttpGet]
        public JsonResult GetOperation(long Ki)
        {
            List<PR_Div_Dep_SecViewModel> _OpList = new List<PR_Div_Dep_SecViewModel>();
            var Operation = _IomService.GetAllOperation(Ki);
            _OpList = Operation
                 .GroupBy(e => new { e._OpId, e._OpDesc })
                 .Select(g => new PR_Div_Dep_SecViewModel
                 {
                     Value = g.Key._OpId ?? 0,
                     Text = g.Key._OpDesc
                 }).ToList();

            return Json(_OpList);
        }

        [HttpGet]
        public JsonResult GetDivisionsByOperation(long operationId, long Ki)
        {
            List<PR_Div_Dep_SecViewModel> _DivList = new List<PR_Div_Dep_SecViewModel>();
            var divisions = _IomService.GetDivisionByOp(operationId, Ki);
            _DivList = divisions
                   .GroupBy(e => new { e._DivId, e._DivDesc })
                   .Select(g => new PR_Div_Dep_SecViewModel
                   {
                       Value = g.Key._DivId ?? 0,
                       Text = g.Key._DivDesc
                   }).ToList();

            return Json(_DivList);
        }

        public JsonResult GetDepartmentByDivision(long opId, long DivId, long Ki)
        {
            List<PR_Div_Dep_SecViewModel> _DepList = new List<PR_Div_Dep_SecViewModel>();
            var Department = _IomService.GetDepartmentByDiv(DivId, opId, Ki);

            _DepList = Department
                   .GroupBy(e => new { e._DepId, e._DepDesc })
                   .Select(g => new PR_Div_Dep_SecViewModel
                   {
                       Value = g.Key._DepId ?? 0,
                       Text = g.Key._DepDesc
                   }).ToList();

            return Json(_DepList);
        }
        public JsonResult GetSectionByDepartment(long opId, long DivId, long DeptCode, long Ki)
        {
            List<PR_Div_Dep_SecViewModel> _SecList = new List<PR_Div_Dep_SecViewModel>();
            var Section = _IomService.GetSectionByDept(DeptCode, DivId, opId, Ki);

            _SecList = Section
                   .GroupBy(e => new { e._SecId, e._SecDescrip })
                   .Select(g => new PR_Div_Dep_SecViewModel
                   {
                       Value = g.Key._SecId ?? 0,
                       Text = g.Key._SecDescrip
                   }).ToList();

            return Json(_SecList);
        }

        [HttpPost]
        public ActionResult IOMAuditReport(SearchIOM SI)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SI.loginid = Convert.ToInt64(_userId.ToString());
            SearchIOM _headerObj = _IomService.IOMAuditReport(SI);
            ModelState.Clear();
            return PartialView("_GetIOMUserReport", _headerObj);
        }
        public ActionResult ExportToExcelAudit(SearchIOM SI)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SI.loginid = Convert.ToInt64(_userId.ToString());
                SearchIOM _headerObj = _IomService.IOMAuditReport(SI);

                string str = this.ExportToExcelUserHtml(_headerObj.SearchResult);
                TempData.Remove("IOMREPORTEXCELFILE");
                //TempData["IOMREPORTEXCELFILE"] = str;
                TempData["IOMREPORTEXCELFILE"] = JsonConvert.SerializeObject(str);
                retVal = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        public ActionResult DownloadExcelAudit()
        {
            try
            {
                if (TempData["IOMREPORTEXCELFILE"] == null)
                {
                    return View();
                }
                //string str = (string)TempData["IOMREPORTEXCELFILE"];
                string str = JsonConvert.DeserializeObject<string>(TempData["IOMREPORTEXCELFILE"].ToString());
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=IOMReport.xls");
                Response.Headers.Add("content-disposition", "attachment; filename=IOMReport.xls");

                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }
        //Added by Aumento for SR99176

        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - Start
        #region IOMDashboardChart
        [HttpGet]
        public ActionResult IOMDashboard()
        {
            SearchIOM obj = new SearchIOM();
            obj.FilterStatus = new short[] { 1, 5 };
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<PR_Div_Dep_SecViewModel> _OPList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _DivList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _DptList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _SCList = new List<PR_Div_Dep_SecViewModel>();
                List<PR_Div_Dep_SecViewModel> _KIList = new List<PR_Div_Dep_SecViewModel>();
                List<DropdownList> dropdownLists = new List<DropdownList>(); //Added by TTL on 28-July-2025 against SR104160 > CR6821

                Employee_Details employeeDetails = _EmpDetails;
                _KIList = _IomService.GetKiLIST(Convert.ToInt64(employeeDetails.Employee_Code));
                if (string.IsNullOrWhiteSpace(Convert.ToString(TempData["KIID"] ?? string.Empty)))
                {
                    obj.KIID = _KIList.FirstOrDefault().Value;
                }
                else
                {
                    obj.KIID = Convert.ToInt64(TempData["KIID"]);
                }


                Employee_Details employeeDetailski = _IomService.GetEmployeeDetail(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                obj.OperationID = employeeDetailski._OpId == null ? 0 : (long)employeeDetailski._OpId;
                obj.DivisionID = employeeDetailski._DivId == null ? 0 : (long)employeeDetailski._DivId;
                obj.DEPTID = employeeDetailski._DepId == null ? 0 : (long)employeeDetailski._DepId;
                obj.SECID = employeeDetailski._SecId == null ? 0 : (long)employeeDetailski._SecId;
                obj.IsTeamMember = (employeeDetailski.Functional_Designation_Id == "" ? 1 : 0);

                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _IomService.BindIOMGraphOperation(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                obj.ISSpecialRight = OPtemp.Item1;

                if (obj.IsTeamMember == 1 && obj.ISSpecialRight == 0)
                {
                    obj.ecode = Convert.ToInt64(employeeDetails.Employee_Code);
                }

                if (obj.ISSpecialRight == 1)
                {
                    _OPList = OPtemp.Item2;
                    obj.OperationID = 0;
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
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> divtemp = _IomService.BindDivision(obj.OperationID, Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
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
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> dpttemp = _IomService.BindDepartment(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID, obj.OperationID, obj.DivisionID);
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
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> sectemp = _IomService.BindSection(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID, obj.OperationID, obj.DivisionID, obj.DEPTID);
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
                ViewBag.CategoryList = new SelectList(_IomService.BindIOMCategory(), "IOMCATMSTID", "CATDESC");

                //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
                if (!string.IsNullOrEmpty(employeeDetailski.Functional_Designation_Id))
                {
                    if (employeeDetailski.Functional_Designation_Id == "1") //Section Head
                    {
                        dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(_SCList.Select(x => x.Value).ToArray(), obj.KIID);
                    }
                    else if (employeeDetailski.Functional_Designation_Id == "2") //Department Head
                    {
                        dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(_DptList.Select(x => x.Value).ToArray(), obj.KIID);
                    }
                    else if (employeeDetailski.Functional_Designation_Id == "3") //Division Head
                    {
                        dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(_DivList.Select(x => x.Value).ToArray(), obj.KIID);
                    }
                    else if (employeeDetailski.Functional_Designation_Id == "4") //Operating Head
                    {
                        dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(_OPList.Select(x => x.Value).ToArray(), obj.KIID);
                    }
                }
                else
                {
                    if (_SCList.Any())
                    {
                        dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(_SCList.Select(x => x.Value).ToArray(), obj.KIID);
                    }
                    else if (_DptList.Any())
                    {
                        dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(_DptList.Select(x => x.Value).ToArray(), obj.KIID);
                    }
                    else if (_DivList.Any())
                    {
                        dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(_DivList.Select(x => x.Value).ToArray(), obj.KIID);
                    }
                    else if (_OPList.Any())
                    {
                        dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(_OPList.Select(x => x.Value).ToArray(), obj.KIID);
                    }
                }

                if (!dropdownLists.Any(x => x.Value == Convert.ToString(employeeDetails.Employee_Code)))
                {
                    dropdownLists.Add(new DropdownList()
                    {
                        Text = $"{employeeDetails._EFirstName?.Trim()} {employeeDetails._ELastName?.Trim()}-{employeeDetails.Employee_Code}",
                        Value = employeeDetails.Employee_Code
                    });
                }
                if (dropdownLists.Any())
                {
                    dropdownLists = dropdownLists.OrderBy(x => x.Text).ToList();
                }
                ViewBag.Initiators = dropdownLists;
                //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End

                ViewBag.IOMGraphDashboardRange = JsonConvert.SerializeObject(
                        (
                            IOMGraphDashboardRange.RangeMap
                            .Union(IOMGraphDashboardRange.SubRangeMap_0_5)
                            .Union(IOMGraphDashboardRange.SubRangeMap_6_10)
                            .Union(IOMGraphDashboardRange.SubRangeMap_11_15)
                            .Union(IOMGraphDashboardRange.SubRangeMap_GTE_16)
                        ).ToDictionary(kv => kv.Key, kv => kv.Value)
                    );

                ModelState.Clear();

            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
            }


            return View(obj);
        }

        [HttpPost]
        public ActionResult IOMDashboard(SearchIOM SI)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SI.loginid = Convert.ToInt64(_userId.ToString());
            SI.FilterStatus = SI.FilterStatus == null ? new short[] { -1 } : SI.FilterStatus;
            DateTime dtStart = DateTime.Now;
            var data = _IomService.IOMDashboardGraphData(SI);
            DateTime dtEnd = DateTime.Now;

            ModelState.Clear();
            var duration = dtEnd.Subtract(dtStart).Duration();
            string formattedDuration = string.Format("{0:D2}:{1:D2}:{2:D3}", duration.Minutes, duration.Seconds, duration.Milliseconds);
            string benchmark = $"{data.Item2} rows processed in {formattedDuration} (min:sec:ms)";
            return Json(new { result = data.Item1, benchmark });
        }

        //Added by TTL on 30-July-2025 against SR104160 > CR6821 - Start
        [HttpGet]
        public ActionResult IOMDashboadReportList()
        {
            return View();
        }
        //Added by TTL on 30-July-2025 against SR104160 > CR6821 - End

        //Changed by TTL on 29-July-2025 against SR104160 > CR6821 - Start
        [HttpPost]
        public ActionResult IOMDashboadReportList(SearchIOM VM)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SearchIOMDashboadReportList data = new SearchIOMDashboadReportList();
            try
            {
                var _status = new short[] { VM.Status };
                data = new SearchIOMDashboadReportList
                {
                    searchIOM = new SearchIOM
                    {
                        OperationID = VM.OperationID,
                        DivisionID = VM.DivisionID,
                        DEPTID = VM.DEPTID,
                        SECID = VM.SECID,
                        FilterStatus = _status,
                        IOMCATMSTID = VM.IOMCATMSTID,
                        KIID = VM.KIID,
                        HasDivisionListData = VM.HasDivisionListData,
                        HasDeptListData = VM.HasDeptListData,
                        HasSecListData = VM.HasSecListData,
                        PendingAtUsers = VM.PendingAtUsers,
                        PendingWithType = VM.PendingWithType,
                        loginid = Convert.ToInt64(_userId.ToString()),
                        rangeId = VM.rangeId,
                        deptLavelId = VM.deptLavelId
                    },
                    IOMGraphDashboardRangeId = VM.rangeId,
                    deptLavelId = VM.deptLavelId
                };
                data = _IomService.IOMDashboadReportList(data);
                if (data?.SearchResult?.Any() ?? false)
                {
                    data.SearchResult = data.SearchResult.FindAll(x => string.Equals(x.OrgLevel ?? string.Empty, VM.designation ?? string.Empty, StringComparison.OrdinalIgnoreCase));
                    if (data.SearchResult.Any())
                    {
                        data.SearchResult.ForEach(x =>
                        {
                            x.EncryptedViewDetailUrl = $"/IOM/IOMViewDetail?id={WebUtility.UrlEncode(Encryption.Encrypt(x.IOMHEADERID.ToString()))}";
                        });
                    }
                    if (data.searchIOM.SearchResult.Any())
                    {
                        data.searchIOM.SearchResult.ForEach(x =>
                        {
                            x.EncryptedViewDetailUrl = $"/IOM/IOMViewDetail?id={WebUtility.UrlEncode(Encryption.Encrypt(x.IOMHEADERID.ToString()))}";
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
            }

            return Json(data);
        }

        //Changed by TTL on 29-July-2025 against SR104160 > CR6821 - Start
        [HttpPost]
        public ActionResult IOMDashboadReportListByDesignationGroup(SearchIOM VM)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SearchIOMDashboadReportList data = new SearchIOMDashboadReportList();
            try
            {
                var _status = new short[] { VM.Status };
                data = new SearchIOMDashboadReportList
                {
                    searchIOM = new SearchIOM
                    {
                        OperationID = VM.OperationID,
                        DivisionID = VM.DivisionID,
                        DEPTID = VM.DEPTID,
                        SECID = VM.SECID,
                        FilterStatus = _status,
                        IOMCATMSTID = VM.IOMCATMSTID,
                        KIID = VM.KIID,
                        HasDivisionListData = VM.HasDivisionListData,
                        HasDeptListData = VM.HasDeptListData,
                        HasSecListData = VM.HasSecListData,
                        PendingAtUsers = VM.PendingAtUsers,
                        PendingWithType = VM.PendingWithType,
                        loginid = Convert.ToInt64(_userId.ToString()),
                        rangeId = VM.rangeId,
                        deptLavelId = VM.deptLavelId,
                    },
                    IOMGraphDashboardRangeId = VM.rangeId,
                    deptLavelId = VM.deptLavelId
                };
                data = _IomService.IOMDashboadReportListByDesignationGroup(data);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
            }

            return Json(data);
        }
        //Changed by TTL on 29-July-2025 against SR104160 > CR6821 - End

        //Added by TTL on 05-Aug-2025 against SR104160 > CR6821 - Start
        [HttpPost]
        public ActionResult GetApproversAsPerOrgLevel(SearchIOM VM)
        {
            List<DropdownList> dropdownLists = new List<DropdownList>();
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                Employee_Details employeeDetails = _EmpDetails;
                Employee_Details employeeDetailski = _IomService.GetEmployeeDetail(Convert.ToInt64(employeeDetails.Employee_Code), VM.KIID);

                //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
                if (VM.SECID > 0)
                {
                    dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(new long[] { VM.SECID }, VM.KIID);
                }
                else if (VM.DEPTID > 0)
                {
                    dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(new long[] { VM.DEPTID }, VM.KIID);
                }
                else if (VM.DivisionID > 0)
                {
                    dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(new long[] { VM.DivisionID }, VM.KIID);
                }
                else if (VM.OperationID > 0)
                {
                    dropdownLists = _IomService.GetEmployeeListAsPerOrgLevel(new long[] { VM.OperationID }, VM.KIID);
                }

                if (!dropdownLists.Any(x => x.Value == Convert.ToString(employeeDetails.Employee_Code)))
                {
                    dropdownLists.Add(new DropdownList()
                    {
                        Text = $"{employeeDetails._EFirstName?.Trim()} {employeeDetails._ELastName?.Trim()}-{employeeDetails.Employee_Code}",
                        Value = employeeDetails.Employee_Code
                    });
                }
                if (dropdownLists.Any())
                {
                    dropdownLists = dropdownLists.OrderBy(x => x.Text).ToList();
                }
                return Json(dropdownLists);

            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
            }
            return Json(dropdownLists);
        }
        //Added by TTL on 05-Aug-2025 against SR104160 > CR6821 - End
        #endregion
        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - End

        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - Start
        public JsonResult GetPRDocumentsByIndentNo(List<string> IndentNos)
        {
            try
            {
                var DocList = _IomService.GetPRDocumentsByIndentNo(IndentNos);
                return Json(DocList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
          
        }
        public ActionResult GetPRPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/PR/"; //// Server.MapPath("~/Uploads/PO/");
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"550px\"></object>";
                string _path = string.Format(embed, file_path + fileName);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }
        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - End
    }
}
