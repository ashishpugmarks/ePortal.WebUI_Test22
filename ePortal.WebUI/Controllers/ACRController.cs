using ePortal.Application.Contracts;
using ePortal.DomainClasses;// Added by Aumento 
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using iText.IO;
//using NWebsec.AspNetCore.Mvc.Csp;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;// Added by Aumento
using iText.Layout;
using iText.Layout.Element;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class ACRController : Controller
    {
        private readonly IACRService _ACRService;
        private readonly IPRService _PrService;// Added by Aumento
        private readonly ISessionService _sessionService;
        public ACRController(IACRService ACRService, IPRService PrService, ISessionService objSession) //IPRService Added by Aumento
        {
            _ACRService = ACRService;
            _PrService = PrService;// Added by Aumento
            _sessionService = objSession;
        }


        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        //public ACRHeaderViewModel GetPODetailFromSap(string _poNo)
        //{
        //    ///------ SAP Connection Object -------////
        //    EportalESS objasset = new EportalESS();
        //    ACRHeaderViewModel poObj = new ACRHeaderViewModel();
        //    List<POMappingViewModel> mappingList = new List<POMappingViewModel>();
        //    DataSet ds = new DataSet();
        //    ds = objasset.GetPOGetails(_poNo); //
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        poObj = (from DataRow row in ds.Tables[0].AsEnumerable()
        //                 select new ACRHeaderViewModel
        //                 {
        //                     PONO = row["EBELN"].ToString(),
        //                     PO_DESC = row["BSART"].ToString(),
        //                     //STATUS = row["STATU"].ToString(),
        //                     VENDORID = row["LIFNR"].ToString(),
        //                     VENDORNAME = row["NAME1"].ToString(),
        //                     VENDORMAILID = row["SMTP_ADDR"].ToString(),
        //                 }).FirstOrDefault();
        //    }
        //    if (ds.Tables[1].Rows.Count > 0)
        //    {
        //        mappingList = (from DataRow row in ds.Tables[1].AsEnumerable()
        //                       select new POMappingViewModel
        //                       {
        //                           PONO = row["EBELN"].ToString(),
        //                           PRNO = row["BANFN"].ToString(),
        //                           //STATUS = row["BANFN"].ToString(),
        //                           //VENDORID = row["BNFPO"].ToString(),
        //                           //VENDORNAME = row["ZCONTRACT_NO"].ToString(),
        //                       }).ToList();
        //        poObj.poMapping = mappingList.ToList();
        //    }
        //    string _venderCode = poObj.VENDORID[0] == '0' ? poObj.VENDORID.Remove(0, 1) : poObj.VENDORID;
        //    poObj.VENDORID = _venderCode[0] == '0' ? _venderCode.Remove(0, 1) : _venderCode;

        //    return poObj;
        //}

        public ActionResult ACRViewDetail(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            return View("ACRViewDetail", _ACRService.GetACRRequestById(_ReqId));
        }

        [HttpGet]
        public ActionResult ACRRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //Added By Aumento Start

            ACRHeaderViewModel vm = new ACRHeaderViewModel();
            IEnumerable<SYSITE> SYSiteItems = _ACRService.Bind_SYSite();
            ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            //Added By Aumento end
            List<ACRAppAuthSeqViewModel> iList = new List<ACRAppAuthSeqViewModel>();


            if (TempData["APPROVAL_AUTH_LIST"] == null)
            {
                iList = _ACRService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()));
            }
            else
            {

                iList = JsonSerializer.Deserialize<List<ACRAppAuthSeqViewModel>>(TempData["APPROVAL_AUTH_LIST"].ToString());
                if (iList.Count == 0)
                {
                    iList = _ACRService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()));
                }

            }
            //ViewBag.APPROVAL_AUTH_LIST = iList;
            vm.acrAuthSeq = iList;
            TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(iList);
            TempData.Keep();
            //_sessionService.Set<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST", iList);

            return View(vm);
        }

        [HttpPost]
        public ActionResult ACRRequest([FromBody] ACRHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<ACRAuthSkipViewModel>? AuthSkipList = new List<ACRAuthSkipViewModel>();
                if (TempData["DELETED_AUTH_LIST"] != null)
                {
                    //AuthSkipList = _sessionService.Get<List<ACRAuthSkipViewModel>>("DELETED_AUTH_LIST");
                    AuthSkipList = JsonSerializer.Deserialize<List<ACRAuthSkipViewModel>>(TempData["DELETED_AUTH_LIST"].ToString());
                }
                model.skipAuthList = AuthSkipList;
                model.acrDetail = new List<ACRDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                model.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                Tuple<short, long> retVal_tuple = _ACRService.SaveACRRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        //[HttpGet]
        //public ActionResult GetPODetail(string poNo)
        //{
        //    ACRHeaderViewModel poDetail = new ACRHeaderViewModel();
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        //poDetail = GetPODetailFromSap(poNo); ////"6500002191"
        //    }
        //    catch (Exception ex)
        //    {
        //        poDetail = null;
        //    }
        //    return Json(poDetail);
        //}

        [HttpPost]
        public ActionResult SaveACRDetail([FromBody] ACRHeaderViewModel PHVM)
        {
            short retVal = 0; long hearderId = 0; string errmsg = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (!string.IsNullOrEmpty(PHVM.PONO))
                {
                    long objpostatus = _ACRService.GetPRStatusByPOId(PHVM.PONO);
                    if (objpostatus == 1)
                    {
                        return Json(new { res = 5, poId = 0 });
                    }
                }

                PHVM.acrDetail = new List<ACRDetailViewModel>();
                PHVM.acrAuthSeq = new List<ACRAppAuthSeqViewModel>();
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ACRDetailViewModel? model = new ACRDetailViewModel();
                if (TempData["ACR_ATTACHMENT"] != null)
                {
                    //model = _sessionService.Get<ACRDetailViewModel>("ACR_ATTACHMENT");
                    model = JsonSerializer.Deserialize<ACRDetailViewModel>(TempData["ACR_ATTACHMENT"].ToString());
                    PHVM.acrDetail.Add(model);
                }
                string pathtemp = serverpath.getFileUploadPath("ACR/Temp/");
                string arcpath = serverpath.getFileUploadPath("ACR/");
                if (!Directory.Exists(pathtemp)) { Directory.CreateDirectory(pathtemp); }
                if (PHVM.acrDetail.Count > 0)
                {

                    foreach (ACRDetailViewModel obj in PHVM.acrDetail)
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(arcpath + obj.FILENAME);
                        //if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                        if (obj.IsDeleted == 0 && fileBytes != null)
                        {
                            // Added by Aumento for SR84686
                            string filePath = obj.FILENAME;
                            string fileName = System.IO.Path.GetFileName(filePath);
                            // Added by Aumento for SR84686
                            //System.IO.File.WriteAllBytes(pathtemp + obj.FILENAME, obj.FILE_BYTE.ToArray());
                            //System.IO.File.WriteAllBytes(pathtemp + fileName, obj.FILE_BYTE.ToArray()); // Added by Aumento for SR84686
                            System.IO.File.WriteAllBytes(pathtemp + fileName, fileBytes);
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
                        System.IO.File.Delete(arcpath + fileName);//Added by Nagendra
                        System.IO.File.Delete(pathtemp + fileName); // Added by Aumento for SR84686
                        retVal = 3;
                        return Json(new { res = retVal, acrId = hearderId });
                    }
                    else
                    {
                        //System.IO.File.Delete(pathtemp + model.FILENAME);
                        System.IO.File.Delete(arcpath + fileName);//Added by Nagendra
                        System.IO.File.Delete(pathtemp + fileName); // Added by Aumento for SR84686
                    }

                }

                // Signature Code end here

                Tuple<short, long> retVal_tuple = _ACRService.SaveACRRequest(PHVM);
                retVal = retVal_tuple.Item1;
                hearderId = retVal_tuple.Item2;
                if (retVal == 1)
                {
                    string path = serverpath.getFileUploadPath("ACR/");
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                    if (PHVM.acrDetail.Count > 0)
                    {
                        foreach (ACRDetailViewModel obj in PHVM.acrDetail)
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
                errmsg = ex.StackTrace.ToString();
            }
            return Json(new { res = retVal, acrId = hearderId, eMSG = errmsg });
        }

        [HttpPost]
        //public ActionResult UploadACR(IFormFile FILE, string DOC_TYPE, string ACRNo)
        public ActionResult UploadACR(IFormFile FILE, string DOC_TYPE, string ADDITIONAL_INFO, string ACRNo) //ADDITIONAL_INFO Added By Aumento as on 18042024
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ACRDetailViewModel POAtt = new ACRDetailViewModel();
                if (FILE.Length > 0 && !string.IsNullOrEmpty(DOC_TYPE))
                {
                    // Added by Aumento for SR84686 start
                    string path = serverpath.getFileUploadPath("ACR/");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    // Added by Aumento for SR84686 end
                    FileViewModel _file = GetUploadFile(FILE, DOC_TYPE, ACRNo);
                    //POAtt.FILENAME = _file.FileName;
                    POAtt.FILENAME = pathtosave + @"\" + _file.FileName; // Added by Aumento for SR84686
                    POAtt.FILE_CONTENTTYPE = _file.FileContentType;
                    //POAtt.FILE_BYTE = _file.File;
                    POAtt.DOC_TYPE = DOC_TYPE;
                    POAtt.ADDITIONAL_INFO = ADDITIONAL_INFO; //Added By Aumento as on 18042024
                    System.IO.File.Delete(path + POAtt.FILENAME);//Added by Nagendra
                    System.IO.File.WriteAllBytes(path + POAtt.FILENAME, _file.File.ToArray()); //Added By Nagendra
                    TempData["ACR_ATTACHMENT"] = JsonSerializer.Serialize(POAtt);
                    // TempData.Keep();


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
        public ActionResult UploadAttachment([FromForm] ACRDetailViewModel formData)
        {
            short retVal = 0;
            List<ACRDetailViewModel> poDtlList = new List<ACRDetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.ACRHEADERID > 0)
                {
                    // Added by Aumento for SR84686 start
                    string path = serverpath.getFileUploadPath("ACR/");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    // Added by Aumento for SR84686 end
                    FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.ACRNo);
                    poDtlList.Add(new ACRDetailViewModel
                    {
                        ACRHEADERID = formData.ACRHEADERID,
                        //FILENAME = _file.FileName,
                        FILENAME = pathtosave + @"\" + _file.FileName, // Added by Aumento for SR84686
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = formData.DOC_TYPE,
                        ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString()),
                    });
                    Tuple<short, List<ACRDetailViewModel>> _ret_tuple = _ACRService.SaveAttachment(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), formData.ACRHEADERID, poDtlList);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        //string path = serverpath.getFileUploadPath("ACR/"); // comment by Aumento for SR84686
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (poDtlList.Count > 0)
                        {
                            foreach (ACRDetailViewModel obj in poDtlList)
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
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.DOC_TYPE != "ACR").ToList() });
        }

        [HttpDelete]
        public ActionResult DeleteAttachment([FromBody] ACRFileDelete obj)
        {
            short retVal = 0;
            List<ACRDetailViewModel> poDtlList = new List<ACRDetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Tuple<short, List<ACRDetailViewModel>> _ret_tuple = _ACRService.DeleteAttachment(obj.fileName, obj.docType, obj.ParentID);
                retVal = _ret_tuple.Item1;
                poDtlList = _ret_tuple.Item2;
                if (retVal == 1)
                {
                    string path = serverpath.getFileUploadPath("ACR/");
                    if (System.IO.File.Exists(System.IO.Path.Combine(path, obj.fileName)))
                    {
                        System.IO.File.Delete(System.IO.Path.Combine(path, obj.fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.IsDeleted == 0 && w.DOC_TYPE != "ACR").ToList() });
        }

        //[HttpGet]
        //public ActionResult PreviewPODetail(long poHeaderId)
        //{
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        ACRHeaderViewModel PHVM = _ACRService.GetACRRequestById(poHeaderId);
        //        return PartialView("_PreviewPODetail", PHVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json("error");
        //    }
        //}

        [HttpGet]
        public ActionResult EditACRRequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }


            //Added By Aumento Start
            IEnumerable<SYSITE> SYSiteItems = _ACRService.Bind_SYSite();
            ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            //Added By Aumento End

            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            ACRHeaderViewModel PHVM = _ACRService.GetACRRequestById(_ReqId);

            if (PHVM.SYSITEID != null)
            {
                ViewBag.ESYSITEID = PHVM.SYSITEID;
            }
            if (PHVM.acrAuthSeq.Count == 0)
            {
                PHVM.acrAuthSeq = _ACRService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()));
            }

            ///_sessionService.Set<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST", PHVM.acrAuthSeq.OrderBy(o => o.APP_SEQ).ToList());
            TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(PHVM.acrAuthSeq.OrderBy(o => o.APP_SEQ).ToList());

            TempData.Keep();

            if (PHVM.skipAuthList != null)
            {
                // _sessionService.Set<List<ACRAuthSkipViewModel>>("DELETED_AUTH_LIST", PHVM.skipAuthList);
                TempData["DELETED_AUTH_LIST"] = JsonSerializer.Serialize(PHVM.skipAuthList);
                TempData.Keep();
            }

            ViewBag.strId = id;
            return View(PHVM);
        }

        [HttpPost]
        public ActionResult EditACRRequest(ACRHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.acrDetail = new List<ACRDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                model.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                Tuple<short, long> retVal_tuple = _ACRService.SaveACRRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult ACRApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }


            //long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64((id));
            ACRHeaderViewModel obj = _ACRService.GetACRRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            obj.ISENABLE = obj.acrAppHis.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0 && a.ACRAPPHISTORY_ID != 0).Count().ToString();
            return View("ACRApproval", obj);

        }

        [HttpPost]
        public ActionResult ACRApproval([FromBody] ACRAppHistoryViewModel PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                PHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                retVal = _ACRService.ACRApproval(PHVM, _Employee_Details);
                if (retVal == 1)
                {
                    ACRHeaderViewModel ACR_Dtl = _ACRService.GetACRRequestById(PHVM.ACRID);
                    if (ACR_Dtl != null)
                    {
                        var OBJAPP = ACR_Dtl.acrAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (OBJAPP == null)
                        {
                            // Added by Aumento for SR84686
                            string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                            string path = serverpath.getFileUploadPath("ACR/");
                            if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                            string STRFILENAME = System.IO.Path.Combine(pathtosave, "ACR" + ACR_Dtl.ACRNO + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf");
                            // Added by Aumento for SR84686
                            //string STRFILENAME = "ACR" + ACR_Dtl.ACRNO + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";

                            GetDocumentWithAppendedContent(ACR_Dtl.acrAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), PHVM.ACR_ATTACHMENT_NAME, STRFILENAME, 100, ACR_Dtl.acrAuthSeq);
                            List<ACRDetailViewModel> obj = new List<ACRDetailViewModel>() { new ACRDetailViewModel() { DOC_TYPE = "ACRA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "Approved ACR" } };
                            _ACRService.SaveAttachment(PHVM.ADDEDBY, PHVM.ACRID, obj);
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


        //Below Added by aumento as on 28092024 for SR80813------------------------------------------------------------------------------
        //[HttpGet]
        //public ActionResult GetNextApprAcrHeaderId()
        //{
        //    if (_sessionService.Get<string>("userID") == null)
        //    {
        //        return Json(null);
        //    }

        //    PORequest objPO = new PORequest();
        //    DataTable dtACR = objPO.ManageACRApproval(_sessionService.Get<string>("userID").ToString()).Tables[0];
        //    if (dtACR.Rows.Count > 0)
        //    {
        //        DataRow firstRow = dtACR.Rows[0];
        //        var nextAcrHeaderId = firstRow["acrheaderid"].ToString();
        //        return Json(nextAcrHeaderId);
        //    }

        //    return Json(null);
        //}
        //---------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        public ActionResult ACRCancel(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            //long _ReqId = Convert.ToInt64(Encryption.Decrypt(WebUtility.UrlDecode(id)));//Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));


            return View("ACRCancel", _ACRService.GetACRRequestById(_ReqId));
        }

        [HttpPost]
        public ActionResult ACRCancel([FromBody] ACRHeaderViewModel PHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                PHVM.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _ACRService.ACRCancel(PHVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult GetAppAuthority(string eCode, int designationId, string designation)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (eCode != null)
                {
                    eCode = eCode.Split("-")[0].Trim();
                }
                Employee_Details _Login_Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                List<ACRAppAuthSeqViewModel> AuthSeqList = new List<ACRAppAuthSeqViewModel>();

                Employee_Details employee_dtl = _ACRService.GetAuthEmpById(Convert.ToInt64(eCode), designationId, designation, _Login_Employee_Details);
                if (employee_dtl == null) { employee_dtl = new Employee_Details(); }

                if (TempData["APPROVAL_AUTH_LIST"] != null)
                {
                    // AuthSeqList = _sessionService.Get<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST");
                    AuthSeqList = JsonSerializer.Deserialize<List<ACRAppAuthSeqViewModel>>(TempData["APPROVAL_AUTH_LIST"].ToString());
                }

                if (employee_dtl._ECode > 0 && !string.IsNullOrEmpty(employee_dtl._EName))
                {
                    AuthSeqList.Add(new ACRAppAuthSeqViewModel
                    {
                        ADEMPCODE = employee_dtl._ECode,
                        ADEMPNAME = employee_dtl._EName,
                        ADDESIGNATION = employee_dtl._Desig,
                        APP_SEQ = Convert.ToInt16(designationId),
                        APPTYPE = 1,
                        FNDESID = Convert.ToInt16(employee_dtl._FnDesigId == null ? 0 : employee_dtl._FnDesigId),
                    });
                }
                //_sessionService.Set<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST", AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());
                TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());

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
                    SEQ_LIST = new List<ACRAppAuthSeqViewModel>()
                });
            }
        }

        //============Change Done on 27082022 For Add Other Category by (Aumento)==========================================================================================
        [HttpGet]
        public ActionResult GetAppOAuthority(string eCode, int designationId, string designation)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Login_Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                List<ACRAppAuthSeqViewModel> AuthSeqList = new List<ACRAppAuthSeqViewModel>();

                //Employee_Details employee_dtl = _ACRService.GetOAuthEmpById(Convert.ToInt64(eCode), designation, _Login_Employee_Details);
                Employee_Details employee_dtl = _ACRService.GetOAuthEmpById(Convert.ToInt64(eCode), designationId, designation, _Login_Employee_Details);
                if (employee_dtl == null) { employee_dtl = new Employee_Details(); }

                if (TempData["APPROVAL_AUTH_LIST"] != null)
                {
                    //AuthSeqList = _sessionService.Get<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST");
                    AuthSeqList = JsonSerializer.Deserialize<List<ACRAppAuthSeqViewModel>>(TempData["APPROVAL_AUTH_LIST"].ToString());
                }

                if (employee_dtl._ECode > 0 && !string.IsNullOrEmpty(employee_dtl._EName))
                {
                    AuthSeqList.Add(new ACRAppAuthSeqViewModel
                    {
                        ADEMPCODE = employee_dtl._ECode,
                        ADEMPNAME = employee_dtl._EName,
                        ADDESIGNATION = employee_dtl._Desig,
                        APP_SEQ = Convert.ToInt16(designationId),
                        APPTYPE = 1,
                        FNDESID = Convert.ToInt16(employee_dtl._FnDesigId == null ? 0 : employee_dtl._FnDesigId),
                    });
                }
                //Below Change done on 22092022 By (Aumento) ============================================================
                //ViewBag.APPROVAL_AUTH_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();
                //_sessionService.Set<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST", AuthSeqList.ToList());
                TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(AuthSeqList.ToList());
                //===========================================================================================================


                return Json(new
                {
                    ECODE = employee_dtl._ECode,
                    ENAME = employee_dtl._EName,
                    //Below Change done on 22092022 By (Aumento) ============================================================
                    SEQ_LIST = AuthSeqList.ToList()
                    // SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList()
                    //===========================================================================================================
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ECODE = 0,
                    ENAME = "",
                    SEQ_LIST = new List<ACRAppAuthSeqViewModel>()
                });
            }
        }

        //==============================================================================================================================================================================

        public ActionResult DeleteAppAuthority(string eCode, string remark)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<ACRAppAuthSeqViewModel> AuthSeqList = new List<ACRAppAuthSeqViewModel>();
                List<ACRAuthSkipViewModel> AuthSkipList = new List<ACRAuthSkipViewModel>();


                if (TempData["APPROVAL_AUTH_LIST"] != null)
                {
                    //AuthSeqList = _sessionService.Get<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST");
                    AuthSeqList = JsonSerializer.Deserialize<List<ACRAppAuthSeqViewModel>>(TempData["APPROVAL_AUTH_LIST"].ToString());
                }

                if (TempData["DELETED_AUTH_LIST"] != null)
                {
                    AuthSkipList = JsonSerializer.Deserialize<List<ACRAuthSkipViewModel>>(TempData["DELETED_AUTH_LIST"].ToString());
                }

                if (AuthSeqList.Count > 0)
                {
                    //// --- Save skip authority --- ////
                    ACRAppAuthSeqViewModel skipAuth = AuthSeqList.Where(r => r.ADEMPCODE == Convert.ToInt64(eCode)).FirstOrDefault();
                    if (skipAuth != null)
                    {
                        if (!AuthSkipList.Any(a => a.ADEMPCODE == skipAuth.ADEMPCODE))
                        {
                            ACRAuthSkipViewModel newSkipAuth = new ACRAuthSkipViewModel();
                            newSkipAuth.ADEMPCODE = skipAuth.ADEMPCODE;
                            newSkipAuth.SKIPREMARK = remark;
                            AuthSkipList.Add(newSkipAuth);
                        }
                    }
                    //_sessionService.Set<List<ACRAuthSkipViewModel>>("DELETED_AUTH_LIST", AuthSkipList);
                    TempData["DELETED_AUTH_LIST"] = JsonSerializer.Serialize(AuthSkipList);
                    //// --- End --- ////

                    AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode));

                    //_sessionService.Set<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST", AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());
                    TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());

                    retval = 1;
                }

                return Json(new { RESULT = retval, SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { RESULT = -1 });
            }
        }

        //============Change Done on 27082022 For Add Other Category by (Aumento)==========================================================================================
        public ActionResult DeleteAppAuthorityAll(string eCode, string header, string ISALL)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                List<ACRAppAuthSeqViewModel> AuthSeqList = new List<ACRAppAuthSeqViewModel>();

                if (ISALL == "0")
                {
                    if (TempData["APPROVAL_AUTH_LIST"] != null)
                    {
                        //AuthSeqList = _sessionService.Get<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST");
                        AuthSeqList = JsonSerializer.Deserialize<List<ACRAppAuthSeqViewModel>>(TempData["APPROVAL_AUTH_LIST"].ToString());
                    }

                    if (AuthSeqList.Count > 0)
                    {
                        AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode) && r.Header == header);
                        //_sessionService.Set<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST", AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());
                        TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(AuthSeqList.OrderBy(o => o.APP_SEQ).ToList());
                        retval = 1;
                    }
                }
                else
                {
                    //_sessionService.Set<List<ACRAppAuthSeqViewModel>>("APPROVAL_AUTH_LIST", AuthSeqList);
                    TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(AuthSeqList);
                    ViewBag.Designation = "";
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
        //===========================================================================================================================================================================

        //public ActionResult DeleteAppAuthority(string eCode)
        //{
        //    short retval = 0;
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        List<ACRAppAuthSeqViewModel> AuthSeqList = new List<ACRAppAuthSeqViewModel>();
        //        List<ACRAuthSkipViewModel> AuthSkipList = new List<ACRAuthSkipViewModel>();

        //        if (ViewBag.APPROVAL_AUTH_LIST != null)
        //        {
        //            AuthSeqList = (List<ACRAppAuthSeqViewModel>)ViewBag.APPROVAL_AUTH_LIST;
        //        }

        //        if (ViewBag.DELETED_AUTH_LIST != null)
        //        {
        //            AuthSkipList = (List<ACRAuthSkipViewModel>)ViewBag.DELETED_AUTH_LIST;
        //        }

        //        if (AuthSeqList.Count > 0)
        //        {
        //            //// --- Save skip authority --- ////
        //            ACRAppAuthSeqViewModel skipAuth = AuthSeqList.Where(r => r.ADEMPCODE == Convert.ToInt64(eCode)).FirstOrDefault();
        //            if (skipAuth != null)
        //            {
        //                if (!AuthSkipList.Any(a => a.ADEMPCODE == skipAuth.ADEMPCODE))
        //                {
        //                    ACRAuthSkipViewModel newSkipAuth = new ACRAuthSkipViewModel();
        //                    newSkipAuth.ADEMPCODE = skipAuth.ADEMPCODE;
        //                    //newSkipAuth.SKIPREMARK = remark;
        //                    AuthSkipList.Add(newSkipAuth);
        //                }
        //            }
        //            ViewBag.DELETED_AUTH_LIST = AuthSkipList;
        //            //// --- End --- ////

        //            AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode));

        //            ViewBag.APPROVAL_AUTH_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();

        //            retval = 1;
        //        }

        //        return Json(new { RESULT = retval, SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList() });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { RESULT = -1 });
        //    }
        //}


        //public ActionResult DeleteAllAppAuthority()
        //{
        //    short retval = 0;
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        List<ACRAppAuthSeqViewModel> AuthSeqList = new List<ACRAppAuthSeqViewModel>();
        //        List<ACRAuthSkipViewModel> AuthSkipList = new List<ACRAuthSkipViewModel>();

        //        if (ViewBag.APPROVAL_AUTH_LIST != null)
        //        {
        //            AuthSeqList = (List<ACRAppAuthSeqViewModel>)ViewBag.APPROVAL_AUTH_LIST;
        //        }

        //        if (ViewBag.DELETED_AUTH_LIST != null)
        //        {
        //            AuthSkipList = (List<ACRAuthSkipViewModel>)ViewBag.DELETED_AUTH_LIST;
        //        }
        //        if (AuthSeqList.Count > 0)
        //        {
        //            AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode) && r.Header == header);
        //            ViewBag.APPROVAL_AUTH_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();
        //            retval = 1;
        //        }

        //        if (AuthSeqList.Count > 0)
        //        {
        //            //// --- Save skip authority --- ////
        //            //AuthSeqList.Where(r => r.ADEMPCODE == Convert.ToInt64(eCode)).FirstOrDefault();
        //            ACRAppAuthSeqViewModel skipAuth = AuthSeqList.RemoveAll(x => x.ADEMPCODE).ToString();
        //            if (skipAuth != null)
        //            {
        //                if (!AuthSkipList.Any(a => a.ADEMPCODE == skipAuth.ADEMPCODE))
        //                {
        //                    ACRAuthSkipViewModel newSkipAuth = new ACRAuthSkipViewModel();
        //                    //newSkipAuth.ADEMPCODE = skipAuth.ADEMPCODE;
        //                    //newSkipAuth.SKIPREMARK = remark;
        //                    AuthSkipList.Add(newSkipAuth);
        //                }
        //            }
        //            ViewBag.DELETED_AUTH_LIST = AuthSkipList;
        //            //// --- End --- ////

        //            AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode));

        //            ViewBag.APPROVAL_AUTH_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();

        //            retval = 1;

        //        }

        //        return Json(new { RESULT = retval, SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList() });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { RESULT = -1 });
        //    }
        //}

        //[HttpGet]
        //public ActionResult GetAppAuthority(string eCode)
        //{
        //    if (_sessionService.Get<string>("userID") == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    Employee_Details employee_dtl = new Employee_Details();
        //    employee_dtl = _ACRService.GetAuthEmpById(Convert.ToInt64(eCode));
        //    if (employee_dtl == null) { employee_dtl = new Employee_Details(); }
        //    return Json(new
        //    {
        //        ECODE = employee_dtl._ECode,
        //        ENAME = employee_dtl._EName,
        //        DESIGNATION = employee_dtl._Desig,
        //        APPTYPE = 1,
        //        FNDESID = employee_dtl._FnDesigId
        //    });
        //}

        private FileViewModel GetUploadFile(IFormFile file, string DocType, string PONo)
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
                    FVM.FileName = DocType + "_" + PONo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
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

        public ActionResult GetPDF(string fileName, string FileTyp)
        {
            try
            {
                string file_path = "../../../Uploads/" + FileTyp + "/";// serverpath.getFileUploadPath(FileTyp+"/");
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

        //public void GetDocumentWithAppendedContent(List<ACRAppHistoryViewModel> HISList, string SRCfileName, string DSTfileName, int marginLeft, List<ACRAppAuthSeqViewModel> authseq)
        //{
        //    int X = 0, Y = 0, Pageno = 0;
        //    GetSigLOCATION(SRCfileName, out X, out Y, out Pageno);
        //    string path = serverpath.getFileUploadPath("ACR/" + SRCfileName);
        //    var writer = new PdfWriter(serverpath.getFileUploadPath("ACR/" + DSTfileName));

        //    var pdfResult = new PdfDocument(new PdfReader(path), writer);
        //    var document = new Document(pdfResult);

        //    //document.Add(div);

        //    //for (int i = 1; i <= pagecount; i++)
        //    if (X != 0 && Y != 0 && Pageno != 0)
        //    {

        //        var div = new Div();
        //        Canvas canvas;
        //        PdfPage page = pdfResult.GetPage(Pageno);

        //        int position = 0;
        //        for (int j = 0; j < authseq.Count; j++)
        //        {

        //            Paragraph pgr = new Paragraph();
        //            pgr.SetFontSize(6);

        //            Int32 authseqno = authseq[j].APP_SEQ; //(authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().APP_SEQ);
        //            string empname = authseq[j].ADEMPNAME;//authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().ADEMPNAME;
        //            string appdate = HISList.Where(m => m.ADEMPCODE == authseq[j].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");
        //            if (authseqno > 0)
        //            {
        //                if (authseqno == 1)
        //                {

        //                    pgr.Add(empname + "\n");
        //                    //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    pgr.Add(appdate);
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 15) , Y - 70, 60);
        //                }
        //                else if (authseqno == 2)
        //                {
        //                    pgr.Add(empname + "\n");
        //                    //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    pgr.Add(appdate);
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 15) + (1) * 50, Y - 70, 60);
        //                }
        //                else if (authseqno == 3)
        //                {

        //                    pgr.Add(empname + "\n");
        //                    //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    pgr.Add(appdate);
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    //div.SetFixedPosition((X - 15) + (2) * 50, Y - 80, 60);
        //                    div.SetFixedPosition((X - 15) + (1) * 50, Y - 70, 60);
        //                }
        //                else if (authseqno == 4)
        //                {
        //                    pgr.Add(empname + "\n");
        //                    //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    pgr.Add(appdate);
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 15) + (3) * 50, Y - 70, 60);
        //                    //div.SetFixedPosition((X - 15) + (1) * 50, Y - 80, 60);
        //                }
        //                else if (authseqno == 5)
        //                {

        //                    pgr.Add(empname + "\n");
        //                    //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    pgr.Add(appdate);
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 5) + (3) * 50, Y - 70, 60);
        //                }
        //                else if (authseqno == 6)
        //                {
        //                    pgr.Add(empname + "\n");
        //                    //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    pgr.Add(appdate);
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 1) + (4) * 50, Y - 70, 60);
        //                }
        //                else if (authseqno == 7)
        //                {
        //                    pgr.Add(empname + "\n");
        //                    // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    pgr.Add(appdate);
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 1) + (5) * 50, Y - 70, 60);
        //                }
        //                else if (authseqno == 9)
        //                {
        //                    pgr.Add(empname + "\n");
        //                    // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    pgr.Add(appdate);
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 1) + (6) * 50, Y - 60, 60);
        //                }
        //                else if (authseqno == 10)
        //                {
        //                    pgr.Add(empname + "\n");
        //                    // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    pgr.Add(appdate);
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 1) + (6) * 50, Y - 80, 60);
        //                }
        //                else if (authseqno == 11)
        //                {
        //                    pgr.Add(empname + "\n");
        //                    pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 1) + (7) * 50, Y - 70, 60);
        //                }
        //                else
        //                {
        //                    pgr.Add(empname + "\n");
        //                    pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
        //                    div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
        //                    div.Add(pgr);
        //                    div.SetFixedPosition((X - 15) + (authseqno-2) * 50, Y - 70, 60);
        //                }

        //            }

        //            //Create canvas fro the last page
        //            canvas = new Canvas(page, page.GetPageSize());

        //            canvas.Add(div);
        //        }

        //    }


        //    document.Close();
        //    pdfResult.Close();

        //}


        public void GetDocumentWithAppendedContent(List<ACRAppHistoryViewModel> HISList, string SRCfileName, string DSTfileName, int marginLeft, List<ACRAppAuthSeqViewModel> authseq)
        {
            int X = 0, Y = 0, Pageno = 0;
            GetSigLOCATION(SRCfileName, out X, out Y, out Pageno);
            string path = serverpath.getFileUploadPath("ACR/" + SRCfileName);
            var writer = new PdfWriter(serverpath.getFileUploadPath("ACR/" + DSTfileName));

            var pdfResult = new PdfDocument(new PdfReader(path), writer);
            var document = new Document(pdfResult);

            //document.Add(div);
            //Below added on 14092022 By (Aumento)========================================================================================
            int Temp1 = 0, Temp2 = 0, Temp3 = 0, Temp4 = 0, Temp5 = 0, Temp6 = 0, Temp7 = 0, Temp9 = 0, Temp10 = 0, Temp11 = 0, Temp12 = 0, tmp = 0;
            //=================================================================================================================================



            //for (int i = 1; i <= pagecount; i++)
            if (X != 0 && Y != 0 && Pageno != 0)
            {

                var div = new Div();
                //Below added on 14092022 By (Aumento)========================================================================================
                var div1 = new Div();
                //=================================================================================================================================
                Canvas canvas;
                PdfPage page = pdfResult.GetPage(Pageno);

                int position = 0;
                for (int j = 0; j < authseq.Count; j++)
                {

                    Paragraph pgr = new Paragraph();
                    pgr.SetFontSize(6);
                    //Below added on 14092022 By (Aumento)========================================================================================
                    string ReqCreatedFromEMP = HISList.OrderBy(a => a.ACRAPPHISTORY_ID).Select(a => a.ADDEDBYName).FirstOrDefault();
                    //=================================================================================================================================
                    //string ReqCreatedFromEMP = HISList[0].APPEMP_NAME;//authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().ADEMPNAME;                                     
                    long? authseqno = authseq[j].APP_SEQ; //(authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().APP_SEQ);
                    string empname = authseq[j].ADEMPNAME;//authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().ADEMPNAME;

                    string appdate = HISList.Where(m => m.ADEMPCODE == authseq[j].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");

                    //tmp = tmp + 1;
                    //pgr.Add(ReqCreatedFromEMP + "\n");
                    //div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    //div.Add(pgr);

                    if (authseqno >= 0)
                    {
                        //Below added on 14092022 By (Aumento)========================================================================================
                        if (tmp == 0)
                        {
                            Paragraph pgr1 = new Paragraph();
                            pgr1.SetFontSize(6);

                            tmp = tmp + 1;
                            pgr1.Add(ReqCreatedFromEMP + "\n");
                            div1 = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div1.Add(pgr1);
                            //div1.SetFixedPosition((X - 80), Y - 65, 60);
                            div1.SetFixedPosition((X - 70), Y - 65, 60);//added by aumento as on 12022024
                        }
                        //=================================================================================================================================
                        if (authseqno == 1)
                        {
                            pgr.Add(empname + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            //div.SetFixedPosition((X - 15), Y - 70, 60); 
                            //Below added on 14092022 By (Aumento)========================================================================================                           
                            if (Temp1 == 0)
                            {
                                Temp1 = Temp1 + 1;
                                div.SetFixedPosition((X - 15), Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 15), Y - 80, 60);
                            }
                            //=================================================================================================================================
                        }
                        else if (authseqno == 2)
                        {
                            pgr.Add(empname + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);

                            //div.SetFixedPosition((X - 15) + (1) * 50, Y - 70, 60);                            
                            //div.SetFixedPosition((X - 15) + (1) * 50, Y - 70, 60);
                            //Below added on 14092022 By (Aumento)========================================================================================                           
                            if (Temp2 == 0)
                            {
                                Temp2 = Temp2 + 1;
                                div.SetFixedPosition((X - 15) + (1) * 50, Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 15) + (1) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================
                        }
                        else if (authseqno == 3)
                        {
                            pgr.Add(empname + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            // div.SetFixedPosition((X - 15) + (2) * 50, Y - 60, 60);
                            //Below added on 14092022 By (Aumento)========================================================================================                           
                            if (Temp3 == 0)
                            {
                                Temp3 = Temp3 + 1;
                                div.SetFixedPosition((X - 15) + (2) * 50, Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 15) + (2) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================
                        }
                        else if (authseqno == 4)
                        {
                            pgr.Add(empname + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            //div.SetFixedPosition((X - 15) + (2) * 50, Y - 70, 60); 
                            //Below added on 14092022 By (Aumento)========================================================================================                                                     
                            if (Temp4 == 0)
                            {
                                Temp4 = Temp4 + 1;
                                div.SetFixedPosition((X - 15) + (2) * 50, Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 15) + (2) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================
                        }
                        else if (authseqno == 5)
                        {

                            pgr.Add(empname + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            //div.SetFixedPosition((X - 5) + (3) * 50, Y - 70, 60); 
                            //Below added on 14092022 By (Aumento)========================================================================================                                                      
                            if (Temp5 == 0)
                            {
                                Temp5 = Temp5 + 1;
                                div.SetFixedPosition((X - 5) + (3) * 50, Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 5) + (3) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================

                        }
                        else if (authseqno == 6)
                        {
                            pgr.Add(empname + "\n");
                            //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            // div.SetFixedPosition((X - 1) + (4) * 50, Y - 70, 60);
                            //Below added on 14092022 By (Aumento)========================================================================================                           
                            if (Temp6 == 0)
                            {
                                Temp6 = Temp6 + 1;
                                div.SetFixedPosition((X - 1) + (4) * 50, Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 1) + (4) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================
                        }
                        else if (authseqno == 7)
                        {
                            pgr.Add(empname + "\n");
                            // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            //div.SetFixedPosition((X - 1) + (5) * 50, Y - 70, 60);
                            //Below added on 14092022 By (Aumento)========================================================================================                           
                            if (Temp7 == 0)
                            {
                                Temp7 = Temp7 + 1;
                                div.SetFixedPosition((X - 1) + (5) * 50, Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 1) + (5) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================
                        }
                        else if (authseqno == 9)
                        {
                            pgr.Add(empname + "\n");
                            // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            //div.SetFixedPosition((X - 1) + (6) * 50, Y - 60, 60);
                            //Below added on 14092022 By (Aumento)========================================================================================                           
                            if (Temp9 == 0)
                            {
                                Temp9 = Temp9 + 1;
                                div.SetFixedPosition((X - 1) + (6) * 50, Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 1) + (6) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================

                        }
                        else if (authseqno == 10)
                        {
                            pgr.Add(empname + "\n");
                            // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            pgr.Add(appdate);
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            //div.SetFixedPosition((X - 1) + (6) * 50, Y - 80, 60);
                            //Below added on 14092022 By (Aumento)========================================================================================                           
                            if (Temp10 == 0)
                            {
                                Temp10 = Temp10 + 1;
                                div.SetFixedPosition((X - 1) + (6) * 50, Y - 70, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 1) + (6) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================
                        }
                        else if (authseqno == 11)
                        {
                            pgr.Add(empname + "\n");
                            pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            //div.SetFixedPosition((X - 1) + (7) * 50, Y - 70, 60);
                            //Below added on 14092022 By (Aumento)========================================================================================                           
                            if (Temp11 == 0)
                            {
                                Temp11 = Temp11 + 1;
                                div.SetFixedPosition((X - 1) + (7) * 50, Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 1) + (7) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================
                        }
                        else
                        {
                            pgr.Add(empname + "\n");
                            pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                            div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            div.Add(pgr);
                            //div.SetFixedPosition((X - 15) + (authseqno - 2) * 50, Y - 70, 60);
                            //Below added on 14092022 By (Aumento)========================================================================================                           
                            if (Temp12 == 0)
                            {
                                Temp12 = Temp12 + 1;
                                div.SetFixedPosition((X - 15) + ((float)authseqno - 2) * 50, Y - 60, 60);

                            }
                            else
                            {
                                div.SetFixedPosition((X - 15) + ((float)authseqno - 2) * 50, Y - 80, 60);
                            }
                            //=================================================================================================================================
                        }

                    }

                    //Create canvas fro the last page
                    canvas = new Canvas(page, page.GetPageSize());

                    canvas.Add(div);
                    canvas.Add(div1);
                }

            }


            document.Close();
            pdfResult.Close();

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
                poDetail = _ACRService.GetPODetailByPOId(poNo);
            }
            catch (Exception ex)
            {
                poDetail = null;
            }
            return Json(poDetail);
        }

        public void GetSigLOCATION(string SRCfileName, out Int32 X, out Int32 Y, out Int32 Pageno)
        {
            string path = serverpath.getFileUploadPath("ACR/" + SRCfileName);

            X = 0; Y = 0; Pageno = 0;
            using (var reader = new iTextSharp.text.pdf.PdfReader(path))
            {

                for (int pagen = 1; pagen <= reader.NumberOfPages; pagen++)
                {
                    var parser = new PdfReaderContentParser(reader);

                    var strategy = parser.ProcessContent(pagen, new LocationTextExtractionStrategyWithPosition());
                    var res = strategy.GetAbsoluteLocations();

                    var searchResult = res.Where(p => p.Text.Contains("Sect.Mgr")).ToList();
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

        public string AutocompleteSuggestions(string term, string designation)
        {
            //string designation = "";

            try
            {

                //TempData.Keep();
                //designation = ViewBag.Designation.ToString();
                if (designation == null)
                    designation = "";
                //designation = "2";
            }
            catch (Exception ex) { }
            List<Employee_Details> portaluser = _ACRService.PortalAutocompleteSuggestions(term, designation);
            //List<PortalUser> data = new List<PortalUser>();
            int i = 0;
            List<string> list = new List<string>();
            foreach (var dataitem in portaluser)
            {
                list.Add(dataitem._ECode.ToString() + "-" + dataitem._EFirstName.ToString() + " " + dataitem._ELastName.ToString() + "");
            }
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = JsonSerializer.Serialize(list);
            return sJSON;
        }

        public string AutocompleteOSuggestions(string term, string designation)
        {
            //string designation = "";
            try
            {

                //TempData.Keep();
                //designation = ViewBag.Designation.ToString();
                //if (designation == "-Select-")
                //    designation = "";
            }
            catch (Exception ex) { }
            List<Employee_Details> portaluser = _ACRService.PortalAutocompleteSuggestions(term, designation);
            //List<PortalUser> data = new List<PortalUser>();
            int i = 0;
            List<string> list = new List<string>();
            foreach (var dataitem in portaluser)
            {
                list.Add(dataitem._ECode.ToString() + "-" + dataitem._EFirstName.ToString() + " " + dataitem._ELastName.ToString() + "");
            }
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            string sJSON = JsonSerializer.Serialize(list);
            return sJSON;
        }

        // below region added by Aumento
        #region Dashboard 
        [HttpGet]
        public ActionResult ACRDashboard()
        {
            SearchACRViewModel obj = new SearchACRViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<ADORGLEVEL> _secList = new List<ADORGLEVEL>();
                List<ADORGLEVEL> _depList = new List<ADORGLEVEL>();
                int IsUserOP = 0;
                //List<ADORGLEVEL> _opList = _PrService.BindOperationForOPMap(employeeDetails._ECode);
                List<ADORGLEVEL> _opList = _PrService.GetOrgLevelList((long)1);

                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");

                List<ADORGLEVEL> _divList = _PrService.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "ADORGLEVELID", "LEVELDESCRIP");

                ViewBag.DepList = new SelectList(_depList, "ADORGLEVELID", "LEVELDESCRIP");

                ViewBag.SecList = new SelectList(_secList, "ADORGLEVELID", "LEVELDESCRIP");

                ViewBag.ISUSER_OPERATION = IsUserOP;

                obj.Doc_Status = -1; /// --- -All-
                obj.ReqStatus = 0; /// --- -Pending-

                var objki = _ACRService.BindKI();
                ViewBag.KIList = new SelectList(objki.OrderByDescending(m => m.Value), "Value", "Text");
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult ACRDashboard([FromBody] SearchACRViewModel SSM)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            SearchACRViewModel _headerObj = _ACRService.ACRDashboard(SSM, Convert.ToInt64(employeeDetails._PlantId));
            return PartialView("_DashboardList", _headerObj.SearchResult);
        }
        [HttpPost]
        public ActionResult PAUpload([FromBody] VM_ACR_PaymentAdvise_Master data)
        {
            //ViewBag.ACRList = data.reqno_list;
            return PartialView("_PaymentAdviseUpload", data);
        }
        [HttpPost]
        public ActionResult PARequest([FromBody] List<VM_ACR_PaymentAdvise_Master> info)
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
                    retval = _ACRService.PARequest(data);
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

            return Json(new { res = retval });

        }
        [HttpPost]
        public ActionResult UploadCSV()
        {
            return PartialView("_UploadCSV");
        }
        [HttpPost]
        public ActionResult ConvertCSVtoDataTable()
        {
            ActionResult retval = Json(new { res = 0 });
            var attachedFile = HttpContext.Request.Form.Files["CsvDoc"];
            if (attachedFile == null || attachedFile.Length <= 0) return Json(null);
            var csvReader = new StreamReader(attachedFile.OpenReadStream());
            string inputDataRead;
            var values = new List<string>();
            while ((inputDataRead = csvReader.ReadLine()) != null)
            {
                if (inputDataRead.Trim().Replace(" ", "").Replace(",", " ") != null && inputDataRead.Trim().Replace(" ", "").Replace(",", " ").Trim() != "")
                {
                    values.Add(inputDataRead.Trim().Replace(" ", "").Replace(",", " "));
                }

            }
            values.Remove(values[0]);
            List<VM_ACR_PaymentAdvise_Master> result = new List<VM_ACR_PaymentAdvise_Master>();
            foreach (var value in values)
            {
                string errmsg = "";
                var uploadModelRecord = new VM_ACR_PaymentAdvise_Master();
                var eachValue = value.Split(' ');
                if ((eachValue[0] != "" || eachValue[0] != null) && eachValue[0].Length <= 7)
                {
                    try
                    {
                        uploadModelRecord.PAYMENTADVISE_NO = eachValue[0] != "" ? (eachValue[0]) : "";
                    }
                    catch (Exception ex)
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
                    try
                    {
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
                //if (eachValue[2] != "" || eachValue[2] != null)
                //{
                //    try
                //    {
                //        uploadModelRecord.Invoiceno = eachValue[2] != "" ? (eachValue[2]) : "";
                //    }
                //    catch (Exception ex)
                //    {
                //        errmsg += "Invalid Invoice No,";
                //    }
                //}
                //else
                //{
                //    errmsg += "Invalid Invoice No,";
                //}
                //if (eachValue[3] != "" || eachValue[3] != null)
                //{
                //    try
                //    {
                //        uploadModelRecord.InvoiceDate = eachValue[3] != "" ? Convert.ToDateTime(eachValue[3]) : new DateTime();
                //    }
                //    catch (Exception ex)
                //    {
                //        errmsg += "Invalid Invoice Date,";
                //    }
                //}
                //else
                //{
                //    errmsg += "Invalid Invoice Date,";
                //}
                //if (eachValue[4] != "" || eachValue[4] != null)
                //{
                //    try
                //    {
                //        uploadModelRecord.VenderCode = eachValue[4] != "" ? eachValue[4] : "";
                //    }
                //    catch (Exception ex)
                //    {
                //        errmsg += "Invalid Vendor Code,";
                //    }
                //}
                //else
                //{
                //    errmsg += "Invalid Vendor Code,";
                //}

                uploadModelRecord.REMARK = "";
                if (errmsg != "")
                {
                    return Json(new { res = 3, errmsg = errmsg });
                }
                List<String> req_no = new List<string>();
                //long retVal = (_ACRService.GetSMNO(uploadModelRecord.Invoiceno, uploadModelRecord.InvoiceDate, uploadModelRecord.VenderCode));
                //if (retVal == -1)
                //{
                //    return Json(new { res = retVal });
                //}
                //req_no.Add(Convert.ToString(retVal));
                uploadModelRecord.reqno_list = req_no;

                result.Add(uploadModelRecord);
            }
            return PARequest(result);

        }
        [HttpPost]
        public ActionResult UpdateDocStatus([FromBody] ACRRegenerate objDetails)
        {
            short retVal = 0;
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee"); //Added By Aumento as on 28022024
                retVal = _ACRService.UpdateDocStatus(long.Parse(objDetails.id), Convert.ToInt64(_sessionService.Get<string>("userID")), _Employee_Details); //_Employee_Details Added By Aumento as on 28022024
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpPost]
        public ActionResult UploadPAAttachment(VM_ACR_PADetailViewModel formData)
        {
            short retVal = 0;
            List<VM_ACR_PADetailViewModel> smDtlList = new List<VM_ACR_PADetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE))
                {
                    FileViewModel _file = GetUploadPAFile(formData.FILE, formData.DOC_TYPE, formData.PAYMENTADVISE_NO.ToString());
                    smDtlList.Add(new VM_ACR_PADetailViewModel
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
                    Tuple<short, List<VM_ACR_PADetailViewModel>> _ret_tuple = _ACRService.SavePAAttachment(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), smDtlList, formData.PAYMENTADVISE_NO);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        string path = serverpath.getFileUploadPath("PA/");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (smDtlList.Count > 0)
                        {
                            foreach (VM_ACR_PADetailViewModel obj in smDtlList)
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
            VM_ACR_PaymentAdvise_Master obj = new VM_ACR_PaymentAdvise_Master();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                obj.PAYMENTADVISE_NO = "";
                List<VM_ACR_PaymentAdvise_Master> model1 = _ACRService.GetPADetails(obj);
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }
        [HttpPost]
        public ActionResult PADashboard([FromBody] VM_ACR_PaymentAdvise_Master model)
        {
            List<VM_ACR_PaymentAdvise_Master> model1 = _ACRService.GetPADetails(model);
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
        [HttpPost]
        public ActionResult DeletePAAttachment([FromBody] ACRFileDelete obj)
        {
            short retVal = 0;
            List<VM_ACR_PADetailViewModel> smDtlList = new List<VM_ACR_PADetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Tuple<short, List<VM_ACR_PADetailViewModel>> _ret_tuple = _ACRService.DeletePAAttachment(obj.fileName, obj.docType, obj.ParentID, obj.PA_ID);
                retVal = _ret_tuple.Item1;
                smDtlList = _ret_tuple.Item2;
                if (retVal == 1)
                {
                    string path = serverpath.getFileUploadPath("PA/");
                    if (System.IO.File.Exists(System.IO.Path.Combine(path, obj.fileName)))
                    {
                        System.IO.File.Delete(System.IO.Path.Combine(path, obj.fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = smDtlList.Where(w => w.PAYMENTADVISE_NO == obj.PA_ID).ToList() });

        }
        [HttpPost]
        public ActionResult DeletePA([FromBody] DeleteACRPA obj)
        {
            return PartialView("_DeletePA", obj);
        }
        [HttpPost]
        public ActionResult DeletePAReq([FromBody] DeleteACRPA obj)
        {
            short retval = 0;
            retval = _ACRService.deletePAReq(obj);
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
                string path = serverpath.getFileUploadPath("/../" + obj.DOCUMENT_PATH);
                string filename = year_month + "_" + obj.PAYMENTADVISE_NO + "_" + obj.ACR_NO;
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
        public ActionResult PAViewDocs([FromBody] ACRRegenerate obj)
        {
            //List<VM_ACR_PADetailViewModel> PADocs = _ACRService.PAdocs(PAYMENTADVISE_NO);
            List<VM_ACR_PADetailViewModel> PADocs = _ACRService.PAdocs(obj.id);
            return PartialView("_ViewPADoc", PADocs);
        }
        [HttpGet]
        public ActionResult ACRFinApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ACRDashboard", "ACR");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult ACRFinApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64((id));
            ACRHeaderViewModel obj = _ACRService.GetACRRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            obj.ISENABLE = "1";//  obj.acrAppHis.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0).Count().ToString();
            return View("ACRFinApproval", obj);
        }

        [HttpPost]
        public ActionResult ACRFinApprovalSubmit([FromBody] ACRAppHistoryViewModel SHVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                SHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                SHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                List<ACRDetailViewModel> DetailList = new List<ACRDetailViewModel>();
                /*
                if (TempData["SM_ATTACHMENT"] != null)
                {
                    DetailList.Add((ACRDetailViewModel)TempData["SM_ATTACHMENT"]);
                }
                */

                if (TempData["ACR_ATTACHMENT"] != null)//Change By Aumento as on 18042024
                {
                    DetailList.Add(JsonSerializer.Deserialize<ACRDetailViewModel>(TempData["ACR_ATTACHMENT"].ToString()));//Change By Aumento as on 18042024s
                }
                retVal = _ACRService.ACRFinApproval(SHVM, _Employee_Details, DetailList);
                if (retVal == 1 && SHVM.APPROVAL_STATUS == 1)
                {
                    string pathtemp = serverpath.getFileUploadPath("ACR/");
                    if (!Directory.Exists(pathtemp)) { Directory.CreateDirectory(pathtemp); }
                    if (DetailList.Count > 0)
                    {
                        foreach (ACRDetailViewModel obj in DetailList)
                        {
                            if (obj.FILE_BYTE != null)
                            {
                                System.IO.File.WriteAllBytes(pathtemp + obj.FILENAME, obj.FILE_BYTE.ToArray());
                            }
                        }
                    }
                }

                ACRHeaderViewModel SM_Dtl = _ACRService.GetACRRequestById(SHVM.ACRID);
                if (SM_Dtl != null)
                {
                    var OBJAPP = SM_Dtl.acrAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                    if (OBJAPP == null)
                    {
                        if (SHVM.APPROVAL_STATUS == 1 && SM_Dtl.PROCESS_STATUS == 9) //// 9-Approved by finance
                        {
                            foreach (var doc in SM_Dtl.acrDetail.Where(m => m.DOC_TYPE == "ACR"))
                            {
                                //string _type = SM_Dtl.ACR_DESC == 1 ? "Service Entry Sheet" : SM_Dtl.SM_TYPE == 2 ? "Material Receipt Note" : "";
                                string STRFILENAME = "ACRFA" + doc.ACRNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
                                GetDocumentWithAppendedContent(SM_Dtl.acrAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), doc.FILENAME, STRFILENAME, 100, SM_Dtl.acrAuthSeq);
                                List<ACRDetailViewModel> obj = new List<ACRDetailViewModel>() { new ACRDetailViewModel() { DOC_TYPE = "ACRFA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "Finance Approved " } };
                                _ACRService.SaveAttachment(SHVM.ADDEDBY, SHVM.ACRID, obj);
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
                //Employee_Details employeeDetails =_sessionService.Get<Employee_Details>("Employee");
                //_AuthList = _smService.GetTaxationAuthority(typeId, Convert.ToInt64(employeeDetails._PlantId));
                //return Json(_AuthList);
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
                //SMHeaderViewModel SMModel = _smService.GetSMRequestById(id);
                //if (SMModel != null)
                //{
                //    if (SMModel.smAppHis.Count > 0)
                //    {
                //        HisObjList = (from _SmAppHis in SMModel.smAppHis.Where(w => w.APPTYPE == 4)
                //                          //where _SmAppHis.APPROVAL_STATUS == 0
                //                      select _SmAppHis).ToList();

                //    }
                //}
            }
            catch (Exception ex)
            {
                HisObjList = new List<SMAppHistoryViewModel>();
            }
            return Json(HisObjList);
        }

        [HttpPost]
        public ActionResult UpdateTaxationAuth(List<SMAppHistoryViewModel> iList)
        {
            short retVal = 0;

            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                //retVal = _smService.UpdateTaxationAuth(iList, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        //[HttpGet]
        //public ActionResult SMTaxationDashboard()
        //{
        //    SearchSMViewModel obj = new SearchSMViewModel();
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        return View(obj);
        //    }
        //}

        //[HttpPost]
        //public ActionResult SMTaxationDashboard(SearchSMViewModel SSM)
        //{
        //    if (_sessionService.Get<string>("userID") == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    SearchSMViewModel _headerObj = _smService.SMTaxationDashboard(SSM, Convert.ToInt64(_sessionService.Get<string>("userID")));
        //    return PartialView("_TaxationDashboardList", _headerObj.SearchResult);
        //}

        //[HttpGet]
        //public ActionResult SMTaxationApproval()
        //{
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        return RedirectToAction("SMTaxationDashboard", "SM");
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("ErrorPage", "AppError");
        //    }
        //}

        //[HttpPost]
        //public ActionResult SMTaxationApproval(long id)
        //{
        //    if (_sessionService.Get<string>("userID") == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    long _ReqId = Convert.ToInt64((id));
        //    SMHeaderViewModel obj = _smService.GetSMRequestById(_ReqId);
        //    long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
        //    obj.ISENABLE = "1"; // obj.smAppHis.Where(a => a.ADEMPCODE == userid && a.APPROVAL_STATUS == 0).Count().ToString();
        //    return View("SMTaxationApproval", obj);
        //    //SMAppHistoryViewModel model = new SMAppHistoryViewModel();
        //    //model.SMHEADERID = id;
        //    //return PartialView("_UpdateTaxationRemark", model);
        //}

        //[HttpPut]
        //public ActionResult SMTaxationApproval(SMAppHistoryViewModel SHVM)
        //{
        //    short retVal = 0;
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        SHVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
        //        SHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
        //        SHVM.UPDATEBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
        //        retVal = _smService.SMTaxationApproval(SHVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return Json(retVal);
        //}

        [HttpPost]
        public ActionResult ExportToExcel([FromBody] SearchACRViewModel SSVM)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SearchACRViewModel _headerObj = _ACRService.ACRDashboard(SSVM, Convert.ToInt64(employeeDetails._PlantId));
                string str = this.excelHtml(_headerObj.SearchResult);
                //TempData.Remove("EXCELFILE");

                _sessionService.Set<string>("EXCELFILE", str);
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
                if (_sessionService.Get<string>("EXCELFILE") == null)
                {
                    return View();
                }
                string str = _sessionService.Get<string>("EXCELFILE");
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=ACRReport.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel", "ACRReport.xls");

            }
            catch (Exception ex)
            {

                return RedirectToAction("ErrorPage");
            }
        }

        public string excelHtml(List<ACRHeaderViewModel> _headerList)
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
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Vendor Code</th>"); // uncomment by aumento :: SR111540
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Vendor Name</th>"); // uncomment by aumento :: SR111540
                //stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Type</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>ACR Number</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Amount</th>"); // uncomment by aumento :: SR111540
                //stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice No</th>");
                //stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice Amount</th>");
                //stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice Date</th>");
                //stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Request Date</th>"); // comment by aumento :: SR111540
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>PO Number</th>"); // added by aumento :: SR111540
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
                foreach (ACRHeaderViewModel AHVM in _headerList)
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
                    //else if (AHVM.PROCESS_STATUS == 7)
                    //{
                    //    _processStatus = "Forwarded To Taxation";
                    //}

                    string _docStatus = "";
                    if (AHVM.Document_Status == 1)
                    {
                        _docStatus = "Received";
                    }
                    else
                    {
                        _docStatus = "Not Received";
                    }

                    //string reqType = "";
                    //if (AHVM.SM_TYPE == 1)
                    //{
                    //    reqType = "Service Entry Sheet";
                    //}
                    //else if (AHVM.SM_TYPE == 2)
                    //{
                    //    reqType = "Material Receipt Note";
                    //}

                    //string _amount = "";
                    //if (AHVM.INVOICENO == "0")
                    //{
                    //    if (AHVM.AMOUNT <= 200000)
                    //    {
                    //        _amount = "<= 2 Lakh";
                    //    }
                    //    else if (AHVM.AMOUNT > 200000)
                    //    {
                    //        _amount = "> 2 Lakh";
                    //    }
                    //}
                    List<string> sesno = new List<string>();
                    string ses = "";
                    if (string.IsNullOrEmpty(AHVM.ACRNO))
                    {
                        sesno = AHVM.acrDetail.Where(s => s.ACRNo != " " && s.ACRNo != null).Select(s => s.ACRNo).ToList();
                        foreach (var no in sesno)
                        {
                            ses += no + ", ";
                        }
                        AHVM.ACRNO = ses;
                    }
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.ACRHEADERID + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.Emp_Detail._ECode + "</td>"); 
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.Emp_Detail._EName + "</td>"); // text-align:center added by aumento :: SR111540
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.VENDORCODE + "</td>"); // uncomment and add text-align:center by aumento :: SR111540
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.VENDORNAME + "</td>"); // uncomment and add text-align:center by aumento :: SR111540
                    ///stringBuilder.Append("<td style='border:1px solid;'>" + reqType + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ACRNO + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.AMOUNT + "</td>"); // added by aumento :: SR111540
                    ////stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + _amount + "</td>");
                    ////stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.INVOICENO + "</td>");
                    ////stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.INVAMOUNT + "</td>");
                    ////stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.INVOICEDATE + "</td>");
                    //stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._OpDesc + "</td>");
                    //stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._DivDesc + "</td>");
                    //stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._DepDesc + "</td>");
                    //stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.Emp_Detail._SecDescrip + "</td>");
                    //stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.DATEADDED).ToString("dd-MMM-yyyy") + "</td>"); // comment by aumento :: SR111540
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.PONO + "</td>"); // added by aumento :: SR111540
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

        //Added by aumento as on 28092024 for the SR80813 ===============================================
        [HttpGet]
        public ActionResult ACRNextApproval(string ACRID_PARAM)
        {
            string retVal = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                long ACRidcurr = 0, ACRIDNext;
                if (!string.IsNullOrEmpty(ACRID_PARAM))
                {
                    ACRidcurr = Convert.ToInt64(ACRID_PARAM);
                    ACRIDNext = _ACRService.GetACRNextApprovalId(ACRidcurr, userid);
                    retVal = ACRIDNext == 0 ? "" : ACRIDNext.ToString();
                }
            }
            catch (Exception ex)
            {
                retVal = "";
            }
            return Json(retVal);
        }
        //Ended by aumento as on 28092024 for the SR80813 ===============================================

        #endregion

        // start added by aumento :: SR111540
        public JsonResult AutocompleteSuggestionsVendor(string term)
        {
            try
            {
                var list = _ACRService.AutocompleteSuggestionsVendor(term);
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
        public JsonResult AutocompleteSuggestionsPONumber(string Key)
        {
            try
            {
                var list = _ACRService.AutocompleteSuggestionsPONumber(Key);
                return Json(list);
            }
            catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        // end added by aumento :: SR111540
    }
}
