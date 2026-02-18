using ePortal.Application.Contracts;

using ePortal.DomainClasses;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using iText.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class PRController : Controller
    {

        private readonly IPRService _PrService;
       // private readonly IPOService _POService;
        private readonly ICommonFunctions cm;
        private readonly ISessionService _sessionService;

        public PRController(IPRService PrService, ICommonFunctions objcm, ISessionService objSessionService)
        {
            _PrService = PrService;
           //_POService = POService;
            cm = objcm;
            _sessionService = objSessionService;
        }

        public ActionResult PRViewDetail(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));//Convert.ToInt64(id); //
            //long _ReqId = Convert.ToInt64(Encryption.Decrypt(WebUtility.UrlDecode(id)));//Convert.ToInt64(id); //
            return View("PRViewDetail", _PrService.GetPRRequestById(_ReqId));
        }

        [HttpGet]
        public ActionResult PRRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
           
            string strSISoperationID = cm.GetParameterValue("SIS_OPERATION");
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            if (strSISoperationID == obj.Operation_Id)
                TempData["ISPRTYPEENABLED"] = "1";
            else
                TempData["ISPRTYPEENABLED"] = "0";
            //List<PRAppAuthSeqViewModel> iList = new List<PRAppAuthSeqViewModel>();
            //if (TempData["APPROVAL_AUTH_LIST"] == null)
            //{
            //    iList = _PrService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()));
            //}
            //else
            //{
            //    iList = (List<PRAppAuthSeqViewModel>)TempData["APPROVAL_AUTH_LIST"];
            //}
            //TempData["APPROVAL_AUTH_LIST"] = iList.OrderBy(o => o.APP_SEQ).ToList();
            //TempData.Keep();
            ViewBag.CategoryList = new SelectList(_PrService.BindPRCategory(), "CATID", "CATDESC");
            long orgLvlID = Convert.ToInt64(obj.Operation_Id);
            List<PRBuyerMstViewModel> _BuyerList = _PrService.GetABuyerMstList(orgLvlID);
            ViewBag.BuyerList = new SelectList(_BuyerList, "ADEMPCODE", "ADEMPNAME");
            ViewBag.PlantList = _PrService.GetPRReleasePlants(); //  Added by Aumento ::  SR68003
            return View();
        }

        [HttpPost]
        public ActionResult PRRequest([FromBody] PRHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<PRAPPSKIPViewModel> AuthSkipList = new List<PRAPPSKIPViewModel>();
                if (TempData["DELETED_AUTH_LIST"] != null)
                {
                    //AuthSkipList = (List<PRAPPSKIPViewModel>)TempData["DELETED_AUTH_LIST"];
                    AuthSkipList = JsonSerializer.Deserialize<List<PRAPPSKIPViewModel>>(TempData["DELETED_AUTH_LIST"].ToString());
                }
                model.skipAuthList = AuthSkipList;
                model.prDetail = new List<PRDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                model.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                Tuple<short, long> retVal_tuple = _PrService.SavePRRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult SavePRDetail([FromBody] PRHeaderViewModel PHVM)
        {
            short retVal = 0; long hearderId = 0;
            List<PRAppAuthSeqViewModel> iList = new List<PRAppAuthSeqViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PHVM.prDetail = new List<PRDetailViewModel>();
                PHVM.prAuthSeq = new List<PRAppAuthSeqViewModel>();
                PHVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PHVM.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                PRDetailViewModel model = new PRDetailViewModel();
                if (TempData["PR_ATTACHMENT"] != null)
                {
                    //model = (PRDetailViewModel)TempData["PR_ATTACHMENT"];
                    model = JsonSerializer.Deserialize<PRDetailViewModel>(TempData["PR_ATTACHMENT"].ToString());
                    PHVM.prDetail.Add(model);
                }

                string pathtemp = serverpath.getFileUploadPath("PR/Temp/");
                if (!Directory.Exists(pathtemp)) { Directory.CreateDirectory(pathtemp); }
                if (PHVM.prDetail.Count > 0)
                {
                    foreach (PRDetailViewModel obj in PHVM.prDetail)
                    {
                        if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                        {
                            // Added by Aumento for SR84686
                            string filePath = obj.FILENAME;
                            string fileName = System.IO.Path.GetFileName(filePath);
                            // Added by Aumento for SR84686
                            //System.IO.File.WriteAllBytes(pathtemp + obj.FILENAME, obj.FILE_BYTE.ToArray());
                            System.IO.File.WriteAllBytes(pathtemp + fileName, obj.FILE_BYTE.ToArray()); // Added by Aumento for SR84686
                        }
                    }
                }
                //block added to check to find Signature Location is valid or not
                if (!string.IsNullOrEmpty(model.FILENAME))
                {
                    // Added by Aumento for SR84686
                    string filePath = model.FILENAME;
                    string fileName = System.IO.Path.GetFileName(filePath);
                    string srcfile = fileName;
                    // Added by Aumento for SR84686
                    //string srcfile = model.FILENAME;
                    int x = 0; int y = 0; int pageno = 0;
                    GetSigLOCATION_Upload("Temp/" + srcfile, out x, out y, out pageno);
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
                //Logic to check whether PR No. is service PR or not
                long indent_type = 0;
                if (PHVM.IndentNo.Trim().Substring(0, 2) == "85")
                {
                    indent_type = 1;
                }

                Employee_Details objempdetail = _sessionService.Get<Employee_Details>("Employee");
                //iList = _PrService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), PHVM.IndentAmount, PHVM.PRTYPE, objempdetail, indent_type);
                iList = _PrService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), PHVM.IndentAmount, PHVM.PRTYPE, objempdetail, indent_type, PHVM.ITServiceMatSISAppStatus, PHVM.IPServiceMatDeclaration, PHVM); //16-July-2022 - SIS PR Change // { PHVM } Added by Aumento ::  SR68003
                //TempData["APPROVAL_AUTH_LIST"] = iList.OrderBy(o => o.APP_SEQ).ToList();
                TempData["APPROVAL_AUTH_LIST"] = JsonSerializer.Serialize(iList.OrderBy(o => o.APP_SEQ).ToList());    
                TempData.Keep();

               Tuple<short, long> retVal_tuple = _PrService.SavePRRequest(PHVM);
                retVal = retVal_tuple.Item1;
                hearderId = retVal_tuple.Item2;
                if (retVal == 1)
                {
                    string path =  serverpath.getFileUploadPath("PR/");
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                    if (PHVM.prDetail.Count > 0)
                    {
                        foreach (PRDetailViewModel obj in PHVM.prDetail)
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
            }
            return Json(new { res = retVal, poId = hearderId, SEQ_LIST = iList.OrderBy(o => o.APP_SEQ).ToList() });
        }
        [HttpPost]
        public ActionResult UploadPR(IFormFile FILE, string DOC_TYPE, string PRNo)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PRDetailViewModel POAtt = new PRDetailViewModel();
                if (FILE.Length > 0 && !string.IsNullOrEmpty(DOC_TYPE))
                {
                    // Added by Aumento for SR84686 start
                    string path =  serverpath.getFileUploadPath("PR/");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    // Added by Aumento for SR84686 end
                    FileViewModel _file = GetUploadFile(FILE, DOC_TYPE, PRNo);
                    //POAtt.FILENAME = _file.FileName;
                    POAtt.FILENAME = pathtosave + @"\" + _file.FileName; // Added by Aumento for SR84686
                    POAtt.FILE_CONTENTTYPE = _file.FileContentType;
                    POAtt.FILE_BYTE = _file.File;
                    POAtt.DOC_TYPE = DOC_TYPE;
                    TempData["PR_ATTACHMENT"] = JsonSerializer.Serialize(POAtt);
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
        public ActionResult UploadAttachment([FromForm] PRDetailViewModel formData)
        {
            short retVal = 0;
            List<PRDetailViewModel> poDtlList = new List<PRDetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE) && formData.PRHEADERID > 0)
                {
                    // Added by Aumento for SR84686 start
                    string path =  serverpath.getFileUploadPath("PR/");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    // Added by Aumento for SR84686 end
                    FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.PRNo);
                    poDtlList.Add(new PRDetailViewModel
                    {
                        PRHEADERID = formData.PRHEADERID,
                        //FILENAME = _file.FileName,
                        FILENAME = pathtosave + @"\" + _file.FileName, // Added by Aumento for SR84686
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = formData.DOC_TYPE,
                        ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString()),
                    });
                    Tuple<short, List<PRDetailViewModel>> _ret_tuple = _PrService.SaveAttachment(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), formData.PRHEADERID, poDtlList);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        //string path =  serverpath.getFileUploadPath("PR/"); // comment by Aumento for SR84686
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (poDtlList.Count > 0)
                        {
                            foreach (PRDetailViewModel obj in poDtlList)
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
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.DOC_TYPE != "PR").ToList() });
        }

        [HttpDelete]

        //public ActionResult DeleteAttachment([FromBody] string fileName, string docType, long prHeaderId)
        public IActionResult DeleteAttachment([FromBody] PRDetailViewModel request)
        { 
            short retVal = 0;
            List<PRDetailViewModel> poDtlList = new List<PRDetailViewModel>();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string fileName = request.FILENAME;
                string docType = request.DOC_TYPE;
                long prHeaderId = request.PRHEADERID;

                Tuple<short, List<PRDetailViewModel>> _ret_tuple = _PrService.DeleteAttachment(fileName, docType, prHeaderId);
                retVal = _ret_tuple.Item1;
                poDtlList = _ret_tuple.Item2;
                if (retVal == 1)
                {
                    string path =  serverpath.getFileUploadPath("PR/");
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
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.IsDeleted == 0 && w.DOC_TYPE != "PR").ToList() });
        }

        [HttpGet]
        public ActionResult EditPRRequest(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            PRHeaderViewModel PHVM = _PrService.GetPRRequestById(_ReqId);
           
            string strSISoperationID = cm.GetParameterValue("SIS_OPERATION");
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            if (strSISoperationID == obj.Operation_Id)
                TempData["ISPRTYPEENABLED"] = "1";
            else
                TempData["ISPRTYPEENABLED"] = "0";
            //Employee_Details objemp = _sessionService.Get<Employee_Details>("Employee");
            //if (PHVM.prAuthSeq.Count == 0)
            //{
            //    PHVM.prAuthSeq = _PrService.GetDefaultAuthority(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), PHVM.IndentAmount, PHVM.PRTYPE, objemp);
            //}

            //TempData["APPROVAL_AUTH_LIST"] = PHVM.prAuthSeq.OrderBy(o => o.APP_SEQ).ToList();
            //TempData.Keep();

            //SIS PR Change
            if (PHVM.IS_SISPR != null)
            {
                PHVM.ITServiceMatSISAppStatus = true;
            }
            else
            {
                PHVM.ITServiceMatSISAppStatus = false;
            }

            if (PHVM.IS_NONSISPR != null)
            {
                PHVM.NonITServiceMatDeclaration = true;
            }
            else
            {
                PHVM.NonITServiceMatDeclaration = false;
            }
            //SIS PR Change

            if (PHVM.skipAuthList != null)
            {
                TempData["DELETED_AUTH_LIST"] = JsonSerializer.Serialize(PHVM.skipAuthList);
                TempData.Keep();
            }

            ViewBag.strId = id;

            ViewBag.CategoryList = new SelectList(_PrService.BindPRCategory(), "CATID", "CATDESC");
            long orgLvlID = Convert.ToInt64(obj.Operation_Id);
            List<PRBuyerMstViewModel> _BuyerList = _PrService.GetABuyerMstList(orgLvlID);
            ViewBag.BuyerList = new SelectList(_BuyerList, "ADEMPCODE", "ADEMPNAME");
            ViewBag.PlantList = _PrService.GetPRReleasePlants(); //  Added by Aumento ::  SR68003
            ViewBag.PRCategoryList = _PrService.GetPRReleasePlants(); //  Added by Aumento ::  SR68003
            return View(PHVM);
        }

        [HttpPost]
        public ActionResult EditPRRequest([FromBody] PRHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.prDetail = new List<PRDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                model.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                Tuple<short, long> retVal_tuple = _PrService.SavePRRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpGet]
        public ActionResult PRApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //long _ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id))); 
            long _ReqId = Convert.ToInt64((id));
            PRHeaderViewModel obj = _PrService.GetPRRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
            obj.ISENABLE = obj.PROCESS_STATUS == 4 ? "0" : obj.prAppHis.Where(a => a.ADEMPCODE == userid && (a.APPROVAL_STATUS == 0 || a.APPROVAL_STATUS == 4) && a.PRAPPHISTORY_ID != 0).Count().ToString(); // SIS PR Change
            return View("PRApproval", obj);
        }

        [HttpPost]
        public ActionResult PRApproval([FromBody] PRAppHistoryViewModel PHVM)
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

                retVal = _PrService.PRApproval(PHVM, _Employee_Details);
                PRHeaderViewModel PO_Dtl = _PrService.GetPRRequestById(PHVM.PRID);
                if (PO_Dtl != null)
                {
                    var OBJAPP = PO_Dtl.prAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                    if (OBJAPP == null)
                    {
                        if (PHVM.APPROVAL_STATUS == 1)
                        {

                            // Added by Aumento for SR84686
                            string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                            string path =  serverpath.getFileUploadPath("PR/");
                            if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                            string STRFILENAME = System.IO.Path.Combine(pathtosave, "PR" + PO_Dtl.IndentNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf");
                            // Added by Aumento for SR84686  
                            //string STRFILENAME = "PR" + PO_Dtl.IndentNo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";                         
                            GetDocumentWithAppendedContent(PO_Dtl.prAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), PHVM.PR_ATTACHMENT_NAME, STRFILENAME, 100, PO_Dtl.prAuthSeq);
                            List<PRDetailViewModel> obj = new List<PRDetailViewModel>() { new PRDetailViewModel() { DOC_TYPE = "PRA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "Approved PR" } };
                            _PrService.SaveAttachment(PHVM.ADDEDBY, PHVM.PRID, obj);
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
        public ActionResult PRCancel(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                long _ReqId = Convert.ToInt64(Encryption.Decrypt(WebUtility.UrlDecode(id)));//Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
                return View("PRCancel", _PrService.GetPRRequestById(_ReqId));
            }
            catch
            {
                long _ReqId = Convert.ToInt64(Encryption.Decrypt(id));//Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
                return View("PRCancel", _PrService.GetPRRequestById(_ReqId));

            }
        }

        [HttpPost]
        public ActionResult PRCancel([FromBody] PRHeaderViewModel PHVM)
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
                retVal = _PrService.PRCancel(PHVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult PRReport()
        {
            SearchIndent obj = new SearchIndent();
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
        public ActionResult PRReport([FromBody] SearchIndent SI)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SI.BuyerID = Convert.ToInt64(_sessionService.Get<string>("userID"));
            SearchIndent _headerObj = _PrService.PRBuyerDashboard(SI);
            return PartialView("_GetPRForBuyer", _headerObj.SearchResult);
        }

        [HttpGet]
        public ActionResult UpdateStatus(long id, string cycleEndDt = "", long siteId = 0) //Changed by TTL on 24-May-2025 against SR99130 > CR6361
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            PRPURStatusViewModel PPSVM = _PrService.GetPRPURStatusById(id);
            if (PPSVM == null)
            {
                PPSVM = new PRPURStatusViewModel();
                PPSVM.PRHEADERID = id;
            }
            //Changed by TTL on 24-May-2025 against SR99130 > CR6361 - Start
            if (!string.IsNullOrEmpty(cycleEndDt) && siteId > 0)
            {
                DateTime CycleTimeStart = DateTime.ParseExact(cycleEndDt, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                DateTime CycleTimeWithGrace = DateTime.ParseExact(cycleEndDt, "dd-MMM-yyyy", CultureInfo.InvariantCulture).AddDays(60);
                List<DateTime> Holidays = _PrService.GetHolidaysByPlant(CycleTimeStart, CycleTimeWithGrace, siteId);

                //Getting 6 days which will exclude Cycle Start date (Suppose in case of 20, It will go upto 25)
                //Increasing Cycle type to N+5+2 days by TTL against CR7036 on 08-Aug-2025, Earlier it was 6
                DateTime ThresholdDate = GetThresholdDate(CycleTimeStart, 8, Holidays);
                //Hide Send back button if current date is greater than Threshold date
                ViewBag.HideSendBack = DateTime.Now.Date > ThresholdDate.Date;
            }
            //Changed by TTL on 24-May-2025 against SR99130 > CR6361 - End
            return PartialView("_UpdateStatus", PPSVM);
        }

        [HttpPost]
        public ActionResult ChangeSLACategory([FromBody] SLACategoryChangeRequest request)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                long id = request.Id;
                short selectedSLACategory = request.SelectedSLACategory;

                retVal = _PrService.ChangeSLACategory(id, selectedSLACategory, Convert.ToInt64(_sessionService.Get<string>("userID").ToString()));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }


        //Changed by TTL on 24-May-2025 against SR99130 > CR6361 - Start
        public static DateTime GetThresholdDate(DateTime startDate, int workingDaysRequired, List<DateTime> holidays)
        {
            var holidaySet = new HashSet<DateTime>(holidays.Select(h => h.Date));
            int workingDaysCount = 0;
            DateTime currentDate = startDate;

            while (workingDaysCount < workingDaysRequired)
            {
                if (!holidaySet.Contains(currentDate.Date))
                {
                    workingDaysCount++;
                }
                if (workingDaysCount < workingDaysRequired)
                    currentDate = currentDate.AddDays(1);
            }

            return currentDate;
        }
        //Changed by TTL on 24-May-2025 against SR99130 > CR6361 - End
        [HttpPost]
        public ActionResult UpdateStatus([FromBody] PRPURStatusViewModel PPSVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                PPSVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _PrService.UpdatePRStatus(PPSVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult ExportToExcel(SearchIndent SI)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SearchIndent _headerObj = _PrService.PRDashboard(SI);
                //if (reportType == 1)
                //{
                //    _headerList = (employeeDetails._FnDesigId == null ? _headerList.Where(h => h.ADEMPCODE == employeeDetails._ECode).ToList() : _headerList);
                //}
                string str = this.excelHtml(_headerObj.SearchResult);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = str;
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
                string str = (string)TempData["EXCELFILE"];
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=PRReport.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel", "PRReport.xls");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string excelHtml(List<PRHeaderViewModel> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Department</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Section</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Request Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Indent Number</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Indent Amount</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>PR Status</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (PRHeaderViewModel AHVM in _headerList)
                {
                    string prStatus = (AHVM.PRStatus == 2 ? "Accepted" : (AHVM.PRStatus == 1 ? "Hold" : "Pending"));
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.Emp_Detail._ECode + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._EName + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._OpDesc + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._DivDesc + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._DepDesc + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.Emp_Detail._SecDescrip + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.DATEADDED).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.IndentNo + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.IndentAmount + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + prStatus + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        public ActionResult BindDivisionByOperationId(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<PR_Div_Dep_SecViewModel> divList = new List<PR_Div_Dep_SecViewModel>();
            if (id == 0)
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                Tuple<short, List<ADORGLEVEL>> _opMap = _PrService.BindOperationForOPMap(employeeDetails._ECode);
                if (_opMap.Item2.Count > 0)
                {
                    List<ADORGLEVEL> DivList = _PrService.GetOrgLevelList((long)2);
                    if (DivList.Count > 0)
                    {
                        divList = (from data in _opMap.Item2
                                   join div in DivList on data.ADORGLEVELID equals div.PARENTLEVELID
                                   select new PR_Div_Dep_SecViewModel
                                   {
                                       Text = div.LEVELDESCRIP,
                                       Value = div.ADORGLEVELID
                                   }).ToList();
                    }
                }
                else
                {
                    divList = new List<PR_Div_Dep_SecViewModel>();
                }
            }
            else
            {
                divList = _PrService.BindDivision(id);
            }
            return Json(divList);
        }

        public ActionResult BindDeptByDivisionId(long id)
        {
            List<PR_Div_Dep_SecViewModel> depList = _PrService.BindDepartment(id, 0);
            return Json(depList);
        }

        public ActionResult BindSecByDepartmentId(long id)
        {
            List<PR_Div_Dep_SecViewModel> SecList = _PrService.BindSection(id, 0, 0);
            return Json(SecList);
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
                Employee_Details _Login_Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                List<PRAppAuthSeqViewModel> AuthSeqList = new List<PRAppAuthSeqViewModel>();

                Employee_Details employee_dtl = _PrService.GetAuthEmpById(Convert.ToInt64(eCode), designationId, designation, _Login_Employee_Details);
                if (employee_dtl == null) { employee_dtl = new Employee_Details(); }

                if (TempData["APPROVAL_AUTH_LIST"] != null)
                {
                    AuthSeqList = (List<PRAppAuthSeqViewModel>)TempData["APPROVAL_AUTH_LIST"];
                }

                if (employee_dtl._ECode > 0 && !string.IsNullOrEmpty(employee_dtl._EName))
                {
                    AuthSeqList.Add(new PRAppAuthSeqViewModel
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
                    SEQ_LIST = new List<PRAppAuthSeqViewModel>()
                });
            }
        }

        public ActionResult DeleteAppAuthority(string eCode, string remark)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<PRAppAuthSeqViewModel> AuthSeqList = new List<PRAppAuthSeqViewModel>();
                List<PRAPPSKIPViewModel> AuthSkipList = new List<PRAPPSKIPViewModel>();

                if (TempData["APPROVAL_AUTH_LIST"] != null)
                {
                    AuthSeqList = (List<PRAppAuthSeqViewModel>)TempData["APPROVAL_AUTH_LIST"];
                }

                if (TempData["DELETED_AUTH_LIST"] != null)
                {
                    AuthSkipList = (List<PRAPPSKIPViewModel>)TempData["DELETED_AUTH_LIST"];
                }

                if (AuthSeqList.Count > 0)
                {
                    //// --- Save skip authority --- ////
                    PRAppAuthSeqViewModel skipAuth = AuthSeqList.Where(r => r.ADEMPCODE == Convert.ToInt64(eCode)).FirstOrDefault();
                    if (skipAuth != null)
                    {
                        if (!AuthSkipList.Any(a => a.ADEMPCODE == skipAuth.ADEMPCODE))
                        {
                            PRAPPSKIPViewModel newSkipAuth = new PRAPPSKIPViewModel();
                            newSkipAuth.ADEMPCODE = skipAuth.ADEMPCODE;
                            newSkipAuth.SKIPREMARK = remark;
                            AuthSkipList.Add(newSkipAuth);
                        }
                    }
                    TempData["DELETED_AUTH_LIST"] = AuthSkipList;
                    //// --- End --- ////

                    AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode));

                    TempData["APPROVAL_AUTH_LIST"] = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();

                    retval = 1;
                }

                return Json(new { RESULT = retval, SEQ_LIST = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { RESULT = -1 });
            }
        }

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
                    FVM.FileName = DocType + "_" + PONo + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
                    FVM.File = bytes;
                }
                return FVM;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/PR/"; ////  serverpath.getFileUploadPath("PO/");
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

        public ActionResult PRMultiDocsView(long id)
        {
            PRHeaderViewModel obj = _PrService.GetPRRequestById(id);
            PRDetailViewModel item = new PRDetailViewModel();
            item.FILENAME = "Select";
            obj.prDetail.Insert(0, item);

            return View("PRMultiDocsView", obj.prDetail);
        }
        public ActionResult PRTwoDocsView(long id)
        {
            PRHeaderViewModel obj = _PrService.GetPRRequestById(id);
            PRDetailViewModel item = new PRDetailViewModel();
            item.FILENAME = "Select";
            obj.prDetail.Insert(0, item);

            return View("PRTwoDocsView", obj.prDetail);
        }
        public ActionResult GetTwoPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/PR/"; //// Server.MapPath("~/Uploads/PO/");
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
        public ActionResult GetMultiPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/PR/"; //// Server.MapPath("~/Uploads/PO/");
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
        public void GetDocumentWithAppendedContent(List<PRAppHistoryViewModel> HISList, string SRCfileName, string DSTfileName, int marginLeft, List<PRAppAuthSeqViewModel> authseq)
        {
            int X = 0, Y = 0, Pageno = 0;
            GetSigLOCATION(SRCfileName, out X, out Y, out Pageno);
            string path =  serverpath.getFileUploadPath("PR/" + SRCfileName);
            var writer = new PdfWriter( serverpath.getFileUploadPath("PR/" + DSTfileName));

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
                string maxappdate = authseq.Max(m => m.ADDEDDATE).Date.ToString("dd-MMM-yyyy");
                int PrintYaxis = 0;
                if (authseq.Where(m => m.PRINTORDER != 0).Count() <= 0)
                {
                    for (int j = 0; j < authseq.Count; j++)
                    {

                        Paragraph pgr = new Paragraph();
                        pgr.SetFontSize(6);
                        Int32 authseqno = authseq[j].APP_SEQ; //(authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().APP_SEQ);
                        string empname = authseq[j].ADEMPNAME;//authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().ADEMPNAME;
                        string appdate = HISList.Where(m => m.ADEMPCODE == authseq[j].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");
                        #region "Annotation Logic before Additional Approval - Commneted on 07-OCT-2021 since this logic is no longer needed "

                        //if (authseqno > 0 && DateTime.ParseExact(maxappdate, "dd-MMM-yyyy", null) < DateTime.ParseExact("20-Jun-2021", "dd-MMM-yyyy", null))
                        //{
                        //    if (authseqno == 1)
                        //    {

                        //        pgr.Add(empname + "\n");
                        //        //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                        //        pgr.Add(appdate);
                        //        div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        //        div.Add(pgr);
                        //        div.SetFixedPosition((X - 10), Y - 30, 80);
                        //    }
                        //    else if (authseqno == 2)
                        //    {
                        //        pgr.Add(empname + "\n");
                        //        //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                        //        pgr.Add(appdate);
                        //        div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        //        div.Add(pgr);
                        //        div.SetFixedPosition((X - 10), Y - 58, 80);
                        //    }
                        //    else if (authseqno == 9)
                        //    {
                        //        pgr.Add(empname + "\n");
                        //        // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                        //        pgr.Add(appdate);
                        //        div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        //        div.Add(pgr);
                        //        div.SetFixedPosition((X - 10) + (7) * 80, Y - 34, 80);
                        //    }
                        //    else if (authseqno == 10)
                        //    {
                        //        pgr.Add(empname + "\n");
                        //        // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                        //        pgr.Add(appdate);
                        //        div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        //        div.Add(pgr);
                        //        div.SetFixedPosition((X - 10) + (7) * 80, Y - 58, 80);
                        //    }
                        //    else if (authseqno == 11)
                        //    {
                        //        pgr.Add(empname + "\n");
                        //        pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                        //        div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        //        div.Add(pgr);
                        //        div.SetFixedPosition((X - 10) + (8) * 80, Y - 50, 80);
                        //    }
                        //    else
                        //    {
                        //        pgr.Add(empname + "\n");
                        //        pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                        //        div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        //        div.Add(pgr);
                        //        div.SetFixedPosition((X - 10) + (authseqno - 2) * 80, Y - 50, 80);
                        //    }

                        //}
                        #endregion
                        #region "Annotation Logic After Additional Approval "
                        //Below line Commented on 07-OCT-2021  after 07-OCT if print order in 0 in authseq for all auth then below logic will work
                        //if (authseqno > 0 && DateTime.ParseExact(maxappdate, "dd-MMM-yyyy", null) < DateTime.ParseExact("20-Jun-2021", "dd-MMM-yyyy", null))
                        if (authseqno > 0)
                        {
                            if (authseqno == 1)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10), Y - 30, 80);
                            }
                            else if (authseqno == 3)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10), Y - 58, 80);
                            }
                            else if (authseqno == 5)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 1 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 6)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 2 * 80, Y - 34, 80);
                            }
                            else if (authseqno == 7)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 2 * 80, Y - 58, 80);
                            }
                            else if (authseqno == 8)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 3 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 9)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 4 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 10)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 5 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 11)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 6 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 12) //Director 1
                            {
                                pgr.Add(empname + "\n");
                                // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (7) * 80, Y - 34, 80);
                            }
                            else if (authseqno == 13) //Director 2
                            {
                                pgr.Add(empname + "\n");
                                // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (7) * 80, Y - 58, 80);
                            }
                            else if (authseqno == 14) //CPO
                            {
                                pgr.Add(empname + "\n");
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (8) * 80, Y - 50, 80);
                            }
                            else
                            {
                                //pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                //div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                //div.Add(pgr);
                                //div.SetFixedPosition((X - 10) + (authseqno - 2) * 80, Y - 50, 80);
                            }

                        }

                        #endregion

                        //Create canvas fro the last page
                        canvas = new Canvas(page, page.GetPageSize());

                        canvas.Add(div);
                    }

                }
                else if (authseq.Where(m => m.PRINTORDER != 0).Count() > 0)
                {
                    authseq = authseq.Where(m => m.PRINTORDER != 0).OrderBy(m => m.PRINTORDER).ToList();

                    for (int j = 0; j < authseq.Count; j++)
                    {

                        Paragraph pgr = new Paragraph();
                        pgr.SetFontSize(6);
                        short PrintOrderSeq = authseq[j].PRINTORDER;
                        Int32 authseqno = authseq[j].APP_SEQ;
                        string empname = authseq[j].ADEMPNAME;
                        string appdate = HISList.Where(m => m.ADEMPCODE == authseq[j].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");
                        #region "Print Annotation After Adding Printorder Logic "
                        if (PrintOrderSeq > 0 && authseq.Where(m => m.PRINTORDER != 0).Count() > 0)
                        {

                            if (j != 0 && authseq[j - 1].PRINTORDER == PrintOrderSeq)
                            {
                                PrintYaxis = PrintYaxis - 28;
                            }
                            else
                            {
                                PrintYaxis = Y - 30;
                            }

                            if (PrintOrderSeq == 1)
                            {
                                pgr.Add(empname + "\n");
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10), PrintYaxis, 80);
                            }
                            if (PrintOrderSeq == 2)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 1 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 3)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 2 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 4)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 3 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 5)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 4 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 6)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 5 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 7)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 6 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 8)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (7) * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 9)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (8) * 80, PrintYaxis, 80);
                            }
                        }

                        #endregion

                        //Create canvas fro the last page
                        canvas = new Canvas(page, page.GetPageSize());

                        canvas.Add(div);
                    }
                }
            }


            document.Close();
            pdfResult.Close();

        }

        public void GetDocumentWithAppendedContent_new(List<PRAppHistoryViewModel> HISList, string SRCfileName, string DSTfileName, int marginLeft, List<PRAppAuthSeqViewModel> authseq)
        {
            int X = 0, Y = 0, Pageno = 0;
            GetSigLOCATION(SRCfileName, out X, out Y, out Pageno);
            string path =  serverpath.getFileUploadPath("PR/" + SRCfileName);
            var writer = new PdfWriter( serverpath.getFileUploadPath("PR/" + DSTfileName));

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
                //Below Added by aumento as on 31082024============================================
                if (authseq.Count() > 0)
                {
                    //=================================================================================
                    string maxappdate = authseq.Max(m => m.ADDEDDATE).Date.ToString("dd-MMM-yyyy");
                    //Below Added by aumento as on 31082024============================================
                }
                //=================================================================================
                int PrintYaxis = 0;
                if (authseq.Where(m => m.PRINTORDER != 0).Count() <= 0)
                {
                    for (int j = 0; j < authseq.Count; j++)
                    {

                        Paragraph pgr = new Paragraph();
                        pgr.SetFontSize(6);
                        Int32 authseqno = authseq[j].APP_SEQ; //(authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().APP_SEQ);
                        string empname = authseq[j].ADEMPNAME;//authseq.Where(m => m.ADEMPCODE == HISList[j].ADEMPCODE).FirstOrDefault().ADEMPNAME;
                        string appdate = HISList.Where(m => m.ADEMPCODE == authseq[j].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");
                        #region "Annotation Logic before Additional Approval - Commneted on 07-OCT-2021 since this logic is no longer needed "



                        //}
                        #endregion
                        #region "Annotation Logic After Additional Approval "
                        //Below line Commented on 07-OCT-2021  after 07-OCT if print order in 0 in authseq for all auth then below logic will work
                        //if (authseqno > 0 && DateTime.ParseExact(maxappdate, "dd-MMM-yyyy", null) < DateTime.ParseExact("20-Jun-2021", "dd-MMM-yyyy", null))
                        if (authseqno > 0)
                        {
                            if (authseqno == 1)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10), Y - 30, 80);
                            }
                            else if (authseqno == 3)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10), Y - 58, 80);
                            }
                            else if (authseqno == 5)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 1 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 6)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 2 * 80, Y - 34, 80);
                            }
                            else if (authseqno == 7)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 2 * 80, Y - 58, 80);
                            }
                            else if (authseqno == 8)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 3 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 9)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 4 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 10)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 5 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 11)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 6 * 80, Y - 50, 80);
                            }
                            else if (authseqno == 12) //Director 1
                            {
                                pgr.Add(empname + "\n");
                                // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (7) * 80, Y - 34, 80);
                            }
                            else if (authseqno == 13) //Director 2
                            {
                                pgr.Add(empname + "\n");
                                // pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (7) * 80, Y - 58, 80);
                            }
                            else if (authseqno == 14) //CPO
                            {
                                pgr.Add(empname + "\n");
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (8) * 80, Y - 50, 80);
                            }
                            else
                            {
                                //pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                //div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                //div.Add(pgr);
                                //div.SetFixedPosition((X - 10) + (authseqno - 2) * 80, Y - 50, 80);
                            }

                        }

                        #endregion

                        //Create canvas fro the last page
                        canvas = new Canvas(page, page.GetPageSize());

                        canvas.Add(div);
                    }

                }
                else if (authseq.Where(m => m.PRINTORDER != 0).Count() > 0)
                {
                    authseq = authseq.Where(m => m.PRINTORDER != 0).OrderBy(m => m.PRINTORDER).ToList();

                    for (int j = 0; j < authseq.Count; j++)
                    {

                        Paragraph pgr = new Paragraph();
                        pgr.SetFontSize(6);
                        short PrintOrderSeq = authseq[j].PRINTORDER;
                        Int32 authseqno = authseq[j].APP_SEQ;
                        string empname = authseq[j].ADEMPNAME;
                        string appdate = HISList.Where(m => m.ADEMPCODE == authseq[j].ADEMPCODE).Max(m => m.APPROVALDATE).Value.ToString("dd-MMM-yyyy");
                        #region "Print Annotation After Adding Printorder Logic "
                        if (PrintOrderSeq > 0 && authseq.Where(m => m.PRINTORDER != 0).Count() > 0)
                        {

                            if (j != 0 && authseq[j - 1].PRINTORDER == PrintOrderSeq)
                            {
                                PrintYaxis = PrintYaxis - 28;
                            }
                            else
                            {
                                PrintYaxis = Y - 30;
                            }

                            if (PrintOrderSeq == 1)
                            {
                                pgr.Add(empname + "\n");
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10), PrintYaxis, 80);
                            }
                            if (PrintOrderSeq == 2)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 1 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 3)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 2 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 4)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 3 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 5)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 4 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 6)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 5 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 7)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + 6 * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 8)
                            {

                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (7) * 80, PrintYaxis, 80);

                            }
                            if (PrintOrderSeq == 9)
                            {
                                pgr.Add(empname + "\n");
                                //pgr.Add(Convert.ToDateTime(HISList[j].APPROVALDATE).ToString("dd-MMM-yyyy"));
                                pgr.Add(appdate);
                                div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(6).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                                div.Add(pgr);
                                div.SetFixedPosition((X - 10) + (8) * 80, PrintYaxis, 80);
                            }
                        }

                        #endregion

                        //Create canvas fro the last page
                        canvas = new Canvas(page, page.GetPageSize());

                        canvas.Add(div);
                    }
                }
            }


            document.Close();
            pdfResult.Close();

        }

        public void GetSigLOCATION(string SRCfileName, out Int32 X, out Int32 Y, out Int32 Pageno)
        {
            string path =  serverpath.getFileUploadPath("PR/" + SRCfileName);

            X = 0; Y = 0; Pageno = 0;
            using (var reader = new iTextSharp.text.pdf.PdfReader(path))
            {

                for (int pagen = 1; pagen <= reader.NumberOfPages; pagen++)
                {
                    var parser = new PdfReaderContentParser(reader);

                    var strategy = parser.ProcessContent(pagen, new LocationTextExtractionStrategyWithPosition());
                    var res = strategy.GetAbsoluteLocations();

                    var searchResult = res.Where(p => p.Text.Contains("Section Head")).ToList();
                    if (searchResult != null && searchResult.Count > 0)
                    {
                        foreach (var obj in searchResult)
                        {
                            X = Convert.ToInt32(obj.Location.StartLocation[0]);
                            Y = Convert.ToInt32(obj.Location.StartLocation[1]) - 10;
                            Pageno = pagen;
                            break;
                        }
                    }
                    else
                    {
                        var searchResult1 = res.Where(p => p.Text.Contains("Dept. Manager")).ToList();
                        if (searchResult1 != null && searchResult1.Count > 0)
                        {
                            foreach (var obj in searchResult1)
                            {
                                X = Convert.ToInt32(obj.Location.StartLocation[0]);
                                Y = Convert.ToInt32(obj.Location.StartLocation[1]);
                                Pageno = pagen;
                                break;
                            }
                        }
                    }
                }
                reader.Close();
            }

            // document.Close();
            // pdfResult.Close();

        }

        public void GetSigLOCATION_Upload(string SRCfileName, out Int32 X, out Int32 Y, out Int32 Pageno)
        {
            string path =  serverpath.getFileUploadPath("PR/" + SRCfileName);

            X = 0; Y = 0; Pageno = 0;
            using (var reader = new iTextSharp.text.pdf.PdfReader(path))
            {

                for (int pagen = 1; pagen <= reader.NumberOfPages; pagen++)
                {
                    var parser = new PdfReaderContentParser(reader);

                    var strategy = parser.ProcessContent(pagen, new LocationTextExtractionStrategyWithPosition());
                    var res = strategy.GetAbsoluteLocations();

                    var searchResult = res.Where(p => p.Text.Contains("Section Head")).ToList();
                    if (searchResult != null && searchResult.Count > 0)
                    {
                        foreach (var obj in searchResult)
                        {
                            X = Convert.ToInt32(obj.Location.StartLocation[0]);
                            Y = Convert.ToInt32(obj.Location.StartLocation[1]) - 10;
                            Pageno = pagen;
                            //break; //Changes done by TTL on 27-May-2025 against SR99803 > CR6361
                        }
                    }
                }
                reader.Close();
            }

            // document.Close();
            // pdfResult.Close();

        }

        [HttpGet]
        public ActionResult PRUserReport()
        {
            SearchIndentUser obj = new SearchIndentUser();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<ADORGLEVEL> _secList = new List<ADORGLEVEL>();
                List<ADORGLEVEL> _depList = new List<ADORGLEVEL>();

                obj.OperationID = employeeDetails._OpId == null ? 0 : (long)employeeDetails._OpId;
                obj.DivisionID = employeeDetails._DivId == null ? 0 : (long)employeeDetails._DivId;
                obj.DEPTID = employeeDetails._DepId == null ? 0 : (long)employeeDetails._DepId;
                obj.SECID = employeeDetails._SecId == null ? 0 : (long)employeeDetails._SecId;
                obj.ISPUR = employeeDetails._FnDesigId == null ? 0 : (int)employeeDetails._FnDesigId;

                //==SR52365=======
                ViewBag.KICodeList = new SelectList(_PrService.GetKICodeList(Convert.ToInt64(_sessionService.Get<string>("userID").ToString())), "SYKIID", "KICODE");
                Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> _tuple;
                //=================
                List<PR_Div_Dep_SecViewModel> _DivList = new List<PR_Div_Dep_SecViewModel>();
                if (obj.DivisionID == 0)
                {
                    //==SR52365=======
                    //_DivList = _PrService.BindDivision((long)employeeDetails._OpId);
                    _tuple = _PrService.BindDivision((long)employeeDetails._OpId, (long)ViewBag.KICodeList.Items[0].SYKIID, long.Parse(_sessionService.Get<string>("userID").ToString()));
                    _DivList = _tuple.Item5;
                    //=================
                }
                else
                {
                    _DivList.Add(new PR_Div_Dep_SecViewModel
                    {
                        Value = Convert.ToInt64(employeeDetails._DivId),
                        Text = employeeDetails._DivDesc.ToString()
                    });
                }

                _depList = _PrService.GetOrgLevelList((long)3);
                _secList = _PrService.GetOrgLevelList((long)4);
                List<PR_Div_Dep_SecViewModel> _DptList = new List<PR_Div_Dep_SecViewModel>();
                if (obj.DEPTID == 0)
                {
                    _DptList = (from data in _depList
                                join d in _DivList on data.PARENTLEVELID equals d.Value
                                select new PR_Div_Dep_SecViewModel
                                {
                                    Text = data.LEVELDESCRIP,
                                    Value = data.ADORGLEVELID
                                }
                               ).ToList();
                }
                else
                {
                    _DptList.Add(new PR_Div_Dep_SecViewModel
                    {
                        Value = Convert.ToInt64(employeeDetails._DepId),
                        Text = employeeDetails._DepDesc.ToString()
                    });
                }
                List<PR_Div_Dep_SecViewModel> _SCList = new List<PR_Div_Dep_SecViewModel>();
                if (obj.SECID == 0)
                {
                    _SCList = (from data in _secList
                               join d in _DptList on data.PARENTLEVELID equals d.Value
                               select new PR_Div_Dep_SecViewModel
                               {
                                   Text = data.LEVELDESCRIP,
                                   Value = data.ADORGLEVELID
                               }
                                ).ToList();
                }
                else
                {
                    _SCList.Add(new PR_Div_Dep_SecViewModel
                    {
                        Value = Convert.ToInt64(employeeDetails._SecId),
                        Text = employeeDetails._SecDescrip.ToString()
                    });
                }

                ViewBag.DivList = new SelectList(_DivList, "Value", "Text");

                ViewBag.DepList = new SelectList(_DptList, "Value", "Text");

                ViewBag.SecList = new SelectList(_SCList, "Value", "Text");

                if (obj.ISPUR == 0)
                {
                    obj.ecode = employeeDetails._ECode;
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
        public ActionResult PRUserReport([FromBody] SearchIndentUser SI)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SearchIndentUser _headerObj = _PrService.PRUserDashboard(SI);
            ModelState.Clear();
            return PartialView("_GetPRUserReport", _headerObj);
        }

        [HttpPost]
        public ActionResult PRAgeing([FromBody] SearchIndentUser SI)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SearchIndentUser _headerObj = _PrService.PRAgeingHis(SI);
            ModelState.Clear();
            return PartialView("_POAgeingHistory", _headerObj);
        }

        [HttpPost]
        public ActionResult ExportToExcelUser([FromBody] SearchIndentUser SI)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                SearchIndentUser _headerObj = _PrService.PRUserDashboard(SI);
                //if (reportType == 1)
                //{
                //    _headerList = (employeeDetails._FnDesigId == null ? _headerList.Where(h => h.ADEMPCODE == employeeDetails._ECode).ToList() : _headerList);
                //}
                string str = this.excelHtmlUser(_headerObj);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public ActionResult DownloadExcelUser()
        {
            try
            {
                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }
                string str = (string)TempData["EXCELFILE"];
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=PRUserReport.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel", "PRUserReport.xls");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string excelHtmlUser(SearchIndentUser _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.PRUSERDASHBOARD.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Department</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Section</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Request Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>PR Submission Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Indent Number</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Remark</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Indent Amount</th>");

                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ariba RFP ID</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ariba SR NO</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ariba Byuer</th>");

                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Buyer Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Buyer Approval Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>PO Approval Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>PO Approval Ageing</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (var indent in _headerList.PRUSERDASHBOARD.Select(M => M.INDENT_NO).Distinct())
                {
                    VM_VW_PRUSERDASHBOARD AHVM = _headerList.PRUSERDASHBOARD.Where(M => M.INDENT_NO == indent).FirstOrDefault();
                    bool ispocreated = false;
                    DateTime POdateadded = DateTime.Today;
                    string prStatus = (AHVM.POSTATUS == 2 ? "Accepted" : (AHVM.POSTATUS == 1 ? "Hold" : "Pending"));
                    string POdtl = "";

                    foreach (var item1 in _headerList.PRUSERDASHBOARD.Where(M => M.INDENT_NO == indent).OrderBy(p => p.POAPPDATE))
                    {

                        POdtl = POdtl + item1.POAPPROVALSTATUS + "\n";
                        POdateadded = item1.POAPPDATE.Value;
                    }



                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.ADDEDBY + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.ADDEDBYNAME + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.OPERATION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.DIVISION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.DEPARTMENT + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.SECTION + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.DATEADDED).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.PRAPPDATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.INDENT_NO + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ITEM_DETAIL + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.INDENT_AMOUNT + "</td>");

                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ARIBARFPID + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ARIBASRNO + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ARIBABuyer_ECODE + "</td>");

                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.BUYERNAME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.BUYERSTATUS + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + POdtl + "</td>");
                    int agday = (POdateadded.Date - AHVM.PRAPPDATE.Value.Date).Days;
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + agday + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        [HttpGet]
        public ActionResult PRNextApproval(string PRID_PARAM)
        {
            string retVal = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long userid = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                long pridcurr = 0, PRIDNext;
                if (!string.IsNullOrEmpty(PRID_PARAM))
                {
                    pridcurr = Convert.ToInt64(PRID_PARAM);
                    PRIDNext = _PrService.GetPRNextApprovalId(pridcurr, userid);
                    retVal = PRIDNext == 0 ? "" : PRIDNext.ToString();
                }
            }
            catch (Exception ex)
            {
                retVal = "";
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult PRAllocationDashboard()
        {
            SearchIndent obj = new SearchIndent();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<ADORGLEVEL> _secList = new List<ADORGLEVEL>();
                List<ADORGLEVEL> _depList = new List<ADORGLEVEL>();
                List<ADORGLEVEL> _divList = new List<ADORGLEVEL>();

                //obj.OperationID = employeeDetails._OpId == null ? 0 : (long)employeeDetails._OpId;
                //obj.DivisionID = employeeDetails._DivId == null ? 0 : (long)employeeDetails._DivId;
                //obj.DEPTID = employeeDetails._DepId == null ? 0 : (long)employeeDetails._DepId;
                //obj.SECID = employeeDetails._SecId == null ? 0 : (long)employeeDetails._SecId;

                #region BIND OPERATION
                int IsAuthorisedUser = 0;

                Tuple<short, List<ADORGLEVEL>> _opMap = _PrService.BindOperationForOPMap(employeeDetails._ECode);
                if (_opMap.Item2.Count == 0 || _opMap.Item1 == 0)
                {
                    IsAuthorisedUser = 0;
                }
                else
                {
                    IsAuthorisedUser = 1;

                }
                ViewBag.OpList = new SelectList(_opMap.Item2.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
                #endregion

                #region BIND DIVISION
                _divList = _PrService.GetOrgLevelList((long)2);
                List<ADORGLEVEL> _DivList = new List<ADORGLEVEL>();
                if (obj.DivisionID == 0)
                {
                    _DivList = (from data in _opMap.Item2
                                join div in _divList on data.ADORGLEVELID equals div.PARENTLEVELID
                                select new ADORGLEVEL
                                {
                                    LEVELDESCRIP = div.LEVELDESCRIP,
                                    ADORGLEVELID = div.ADORGLEVELID
                                }).ToList();
                }
                else
                {
                    _DivList.Add(new ADORGLEVEL
                    {
                        ADORGLEVELID = Convert.ToInt64(employeeDetails._DivId),
                        LEVELDESCRIP = employeeDetails._DivDesc.ToString()
                    });
                }
                ViewBag.DivList = new SelectList(_DivList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
                #endregion

                #region BIND DEPARTMENT
                _depList = _PrService.GetOrgLevelList((long)3);
                List<ADORGLEVEL> _DptList = new List<ADORGLEVEL>();
                if (obj.DEPTID == 0)
                {
                    _DptList = (from data in _depList
                                join d in _DivList on data.PARENTLEVELID equals d.ADORGLEVELID
                                select new ADORGLEVEL
                                {
                                    LEVELDESCRIP = data.LEVELDESCRIP,
                                    ADORGLEVELID = data.ADORGLEVELID
                                }).ToList();
                }
                else
                {
                    _DptList.Add(new ADORGLEVEL
                    {
                        ADORGLEVELID = Convert.ToInt64(employeeDetails._DepId),
                        LEVELDESCRIP = employeeDetails._DepDesc.ToString()
                    });
                }
                ViewBag.DepList = new SelectList(_DptList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
                #endregion

                #region BIND SECTION
                _secList = _PrService.GetOrgLevelList((long)4);
                List<ADORGLEVEL> _SCList = new List<ADORGLEVEL>();
                if (obj.SECID == 0)
                {
                    _SCList = (from data in _secList
                               join d in _DptList on data.PARENTLEVELID equals d.ADORGLEVELID
                               select new ADORGLEVEL
                               {
                                   LEVELDESCRIP = data.LEVELDESCRIP,
                                   ADORGLEVELID = data.ADORGLEVELID
                               }).ToList();
                }
                else
                {
                    _SCList.Add(new ADORGLEVEL
                    {
                        ADORGLEVELID = Convert.ToInt64(employeeDetails._SecId),
                        LEVELDESCRIP = employeeDetails._SecDescrip.ToString()
                    });
                }
                ViewBag.SecList = new SelectList(_SCList.OrderBy(o => o.LEVELDESCRIP).ToList(), "ADORGLEVELID", "LEVELDESCRIP");
                #endregion

                #region BIND PLANT
                List<PR_Div_Dep_SecViewModel> _PlantList = new List<PR_Div_Dep_SecViewModel>();
                _PlantList = _PrService.BindPlant(0);

                ViewBag.PlantList = _PlantList;
                #endregion

                #region BIND PR CATEGORY
                List<DGIT_PRCAT_MST> catList = _PrService.BindPRCategory();
                ViewBag.CategoryList = new SelectList(catList.Where(f => f.CATID != _opMap.Item1).ToList(), "CATID", "CATDESC");

                string gp_TypeName = "";
                if (catList.Count > 0)
                {
                    var catObj = catList.Where(w => w.CATID == _opMap.Item1).Select(s => s.CATDESC).FirstOrDefault();
                    if (catObj != null)
                    {
                        gp_TypeName = catObj.ToString();
                    }
                }
                #endregion

                ViewBag.ISAUTHORISED_USER = IsAuthorisedUser;
                ViewBag.GP_TYPE = _opMap.Item1;
                ViewBag.GP_TYPE_NAME = gp_TypeName;

                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult PRAllocationDashboard([FromBody] SearchIndent SI)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (SI.OperationID == 0)
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                Tuple<short, List<ADORGLEVEL>> _opMap = _PrService.BindOperationForOPMap(employeeDetails._ECode);
                if (_opMap.Item2.Count > 0)
                {
                    SI.MapOperationIDs = _opMap.Item2.Select(s => s.ADORGLEVELID).ToList();
                }
            }
            else
            {
                SI.MapOperationIDs = new List<long>();
            }
            SearchIndent _headerObj = _PrService.PRDashboard(SI);
            return PartialView("_GetPRReport", _headerObj.SearchResult);
        }

        [HttpGet]
        public ActionResult AssignBuyer(long id, string GPType, string OrgLvlId)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            PRBuyerMapViewModel PBVM = _PrService.GetPRBuyerByHeaderId(id);
            if (PBVM == null)
            {
                PBVM = new PRBuyerMapViewModel();
                PBVM.PRHEADERID = id;
            }
            long? gpType = string.IsNullOrEmpty(GPType) ? Convert.ToInt64(0) : Convert.ToInt64(GPType);
            long? orgLvlId = 0;
            if (id > 0)
            {
                PRHeaderViewModel objModel = _PrService.GetPRRequestById(id);
                if (objModel != null)
                {
                    orgLvlId = objModel.Emp_Detail._OpId;
                }
            }
            //long? orgLvlId = string.IsNullOrEmpty(OrgLvlId) ? Convert.ToInt64(0) : Convert.ToInt64(OrgLvlId);
            List<PRBuyerMstViewModel> _BuyerList = _PrService.GetBuyerMstList(gpType, orgLvlId);
            ViewBag.BuyerList = new SelectList(_BuyerList, "ADEMPCODE", "ADEMPNAME");

            return PartialView("_AssignBuyer", PBVM);
        }

        [HttpPost]
        public ActionResult AssignBuyer([FromBody] PRBuyerMapViewModel PBVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PBVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _PrService.AssignBuyer(PBVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult ChangeCategory([FromBody] CategoryChangeRequest request)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                long id = request.Id;
                short selectedCategory = request.SelectedCategory;

                retVal = _PrService.PRChangeCategory(id, selectedCategory, Convert.ToInt64(_sessionService.Get<string>("userID").ToString()));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult UpdateAllocationStatus([FromBody] SearchIndent SI)
        {

            var id = SI.OperationID;

            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                retVal = _PrService.UpdateAllocationStatus(id, Convert.ToInt64(_sessionService.Get<string>("userID").ToString()));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult PRAllocationExportToExcel([FromBody] SearchIndent SI)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (SI.OperationID == 0)
                {
                    Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                    Tuple<short, List<ADORGLEVEL>> _opMap = _PrService.BindOperationForOPMap(employeeDetails._ECode);
                    if (_opMap.Item2.Count > 0)
                    {
                        SI.MapOperationIDs = _opMap.Item2.Select(s => s.ADORGLEVELID).ToList();
                    }
                }
                else
                {
                    SI.MapOperationIDs = new List<long>();
                }

                SearchIndent _headerObj = _PrService.PRDashboard(SI);
                string str = this.PRAllocationExcelHtml(_headerObj.SearchResult);
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

        public ActionResult PRAllocationDownloadExcel()
        {
            try
            {
                if (TempData["ALLOCATIONEXCELFILE"] == null)
                {
                    return View();
                }
                string str = (string)TempData["ALLOCATIONEXCELFILE"];
               // HttpContext.Response.AddHeader("content-disposition", "attachment; filename=PRAllocationReport.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel", "PRAllocationReport.xls");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string PRAllocationExcelHtml(List<PRHeaderViewModel> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Indent Number</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Ariba RFP ID</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Ariba Buyer</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Plant</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Department</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Section</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>PR Submission Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Indent Amount</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Allocation Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Buyer</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Assign Date</th>");
                //Below added by aumento for the SR70991==============================================================
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Allocator Name (1st)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Assign Date & Time</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Allocator Name (2nd)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Assign Date & Time</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Allocator Name (3rd)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Assign Date & Time</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Allocator Name (4th)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Assign Date & Time</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Allocator Name (5th)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Assign Date & Time</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Allocator Name (6th)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Assign Date & Time</th>");

                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Hold date & time (1st)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Remark (1st)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Unhold date (1st)</th>");

                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Hold date & time (2nd)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Remark (2nd)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Unhold date (2nd)</th>");

                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Hold date & time (3rd)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Remark (3rd)</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Unhold date (3rd)</th>");

                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Sent back Date & Time</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Remark</th>");

                //=====================================================================================================

                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (PRHeaderViewModel AHVM in _headerList)
                {
                    string prStatus = (AHVM.PRStatus == 1 ? "Allocated" : "Not Allocated");
                    string buyer = ""; string assignDate = "";
                    if (AHVM.BUYER_MODEL.Count > 0)
                    {
                        buyer = AHVM.BUYER_MODEL.FirstOrDefault().BUYER_NAME + " - " + AHVM.BUYER_MODEL.FirstOrDefault().BUYER_ECODE;
                        assignDate = AHVM.BUYER_MODEL.FirstOrDefault().DATEADDED.ToString("dd-MMM-yyyy");
                    }

                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.IndentNo + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ARIBARFPID + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ARIBABuyer_ECODE + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.Emp_Detail._ECode + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._EName + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.Emp_Detail._Desig + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._OpDesc + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._DivDesc + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.Emp_Detail._DepDesc + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.Emp_Detail._SecDescrip + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.UPDATEDATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.IndentAmount + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + prStatus + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + buyer + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + assignDate + "</td>");

                    //Below added by aumento for the SR70991==============================================================

                    // --- For First Buyer-----------------------------
                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 0 && AHVM.BUYER_MODEL_NEW[0] != null)
                    {
                        string empName1 = $"{AHVM.BUYER_MODEL_NEW[0].BUYER_NAME} - {AHVM.BUYER_MODEL_NEW[0].BUYER_ECODE}";
                        string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + FirstAllocatorName + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 0 && AHVM.BUYER_MODEL_NEW[0] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.BUYER_MODEL_NEW[0].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //----First Buyer End Change------------------------

                    // --- For Second Buyer-----------------------------
                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 1 && AHVM.BUYER_MODEL_NEW[1] != null)
                    {
                        string empName1 = $"{AHVM.BUYER_MODEL_NEW[1].BUYER_NAME} - {AHVM.BUYER_MODEL_NEW[1].BUYER_ECODE}";
                        string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + FirstAllocatorName + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 1 && AHVM.BUYER_MODEL_NEW[1] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.BUYER_MODEL_NEW[1].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //----Second Buyer End Change------------------------

                    // --- For Third Buyer-----------------------------
                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 2 && AHVM.BUYER_MODEL_NEW[2] != null)
                    {
                        string empName1 = $"{AHVM.BUYER_MODEL_NEW[2].BUYER_NAME} - {AHVM.BUYER_MODEL_NEW[2].BUYER_ECODE}";
                        string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + FirstAllocatorName + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 2 && AHVM.BUYER_MODEL_NEW[2] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.BUYER_MODEL_NEW[2].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //----Third Buyer End Change------------------------

                    // --- For Fourth Buyer-----------------------------
                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 3 && AHVM.BUYER_MODEL_NEW[3] != null)
                    {
                        string empName1 = $"{AHVM.BUYER_MODEL_NEW[3].BUYER_NAME} - {AHVM.BUYER_MODEL_NEW[3].BUYER_ECODE}";
                        string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + FirstAllocatorName + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 3 && AHVM.BUYER_MODEL_NEW[3] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.BUYER_MODEL_NEW[3].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //----Fourth Buyer End Change------------------------

                    // --- For Fifth Buyer-----------------------------
                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 4 && AHVM.BUYER_MODEL_NEW[4] != null)
                    {
                        string empName1 = $"{AHVM.BUYER_MODEL_NEW[4].BUYER_NAME} - {AHVM.BUYER_MODEL_NEW[4].BUYER_ECODE}";
                        string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + FirstAllocatorName + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 4 && AHVM.BUYER_MODEL_NEW[4] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.BUYER_MODEL_NEW[4].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //----Fifth Buyer End Change------------------------

                    // --- For Sixth Buyer-----------------------------
                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 5 && AHVM.BUYER_MODEL_NEW[5] != null)
                    {
                        string empName1 = $"{AHVM.BUYER_MODEL_NEW[5].BUYER_NAME} - {AHVM.BUYER_MODEL_NEW[5].BUYER_ECODE}";
                        string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + FirstAllocatorName + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.BUYER_MODEL_NEW != null && AHVM.BUYER_MODEL_NEW.Count > 5 && AHVM.BUYER_MODEL_NEW[5] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.BUYER_MODEL_NEW[5].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //----Sixth Buyer End Change------------------------------------


                    //---- For Hold/Unhold (1st) Buyer -----------------------------
                    if (AHVM.prBuyerHis_HOLD != null && AHVM.prBuyerHis_HOLD.Count > 0 && AHVM.prBuyerHis_HOLD[0] != null)
                    {
                        //string empName1 = $"{AHVM.BUYER_MODEL_NEW[0].DATEADDED} - {AHVM.BUYER_MODEL_NEW[5].BUYER_ECODE}";
                        //string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_HOLD[0].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.prBuyerHis_HOLD != null && AHVM.prBuyerHis_HOLD.Count > 0 && AHVM.prBuyerHis_HOLD[0] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_HOLD[0].REMARK + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.prBuyerHis_UNHOLD != null && AHVM.prBuyerHis_UNHOLD.Count > 0 && AHVM.prBuyerHis_UNHOLD[0] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_UNHOLD[0].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //---For Hold/Unhold (1st) Buyer End -----------------------------------


                    //---- For Hold/Unhold (2nd) Buyer -----------------------------
                    if (AHVM.prBuyerHis_HOLD != null && AHVM.prBuyerHis_HOLD.Count > 1 && AHVM.prBuyerHis_HOLD[1] != null)
                    {
                        //string empName1 = $"{AHVM.BUYER_MODEL_NEW[0].DATEADDED} - {AHVM.BUYER_MODEL_NEW[5].BUYER_ECODE}";
                        //string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_HOLD[1].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.prBuyerHis_HOLD != null && AHVM.prBuyerHis_HOLD.Count > 1 && AHVM.prBuyerHis_HOLD[1] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_HOLD[1].REMARK + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.prBuyerHis_UNHOLD != null && AHVM.prBuyerHis_UNHOLD.Count > 1 && AHVM.prBuyerHis_UNHOLD[1] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_UNHOLD[1].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //---- For Hold/Unhold (2nd) Buyer -----------------------------

                    //---- For Hold/Unhold (3rd) Buyer -----------------------------
                    if (AHVM.prBuyerHis_HOLD != null && AHVM.prBuyerHis_HOLD.Count > 2 && AHVM.prBuyerHis_HOLD[2] != null)
                    {
                        //string empName1 = $"{AHVM.BUYER_MODEL_NEW[0].DATEADDED} - {AHVM.BUYER_MODEL_NEW[5].BUYER_ECODE}";
                        //string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_HOLD[2].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.prBuyerHis_HOLD != null && AHVM.prBuyerHis_HOLD.Count > 2 && AHVM.prBuyerHis_HOLD[2] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_HOLD[2].REMARK + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.prBuyerHis_UNHOLD != null && AHVM.prBuyerHis_UNHOLD.Count > 2 && AHVM.prBuyerHis_UNHOLD[2] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_UNHOLD[2].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //---- For Hold/Unhold (3rd) Buyer -----------------------------

                    //---- For Send Back Buyer -----------------------------
                    if (AHVM.prBuyerHis_SENDACK != null && AHVM.prBuyerHis_SENDACK.Count > 0 && AHVM.prBuyerHis_SENDACK[0] != null)
                    {
                        //string empName1 = $"{AHVM.BUYER_MODEL_NEW[0].DATEADDED} - {AHVM.BUYER_MODEL_NEW[5].BUYER_ECODE}";
                        //string FirstAllocatorName = HttpUtility.HtmlEncode(empName1);
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_SENDACK[0].DATEADDED + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }

                    if (AHVM.prBuyerHis_HOLD != null && AHVM.prBuyerHis_SENDACK.Count > 0 && AHVM.prBuyerHis_SENDACK[0] != null)
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.prBuyerHis_SENDACK[0].REMARK + "</td>");
                    }
                    else
                    {
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + "" + "</td>");
                    }
                    //---- For Send Back Buyer-----------------------------

                    //=====================================================================================================
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        [HttpGet]
        public ActionResult RePRGenerateDoc(string id_str)
        {
            short retVal = 0;
            string Error = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                PRHeaderViewModel IOM_Dtl = _PrService.GetPRRequestById(Convert.ToInt64(id_str));
                if (IOM_Dtl != null)
                {
                    if (IOM_Dtl.PROCESS_STATUS == 2 && IOM_Dtl.prDetail.Where(m => m.DOC_TYPE == "PRA").Count() == 0)
                    {
                        var OBJAPP = IOM_Dtl.prAppHis.Where(M => M.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (OBJAPP == null)
                        {
                            var objiomfile = IOM_Dtl.prDetail.Where(m => m.DOC_TYPE == "PR");
                            if (objiomfile == null || objiomfile.Count() == 0)
                            {
                                throw new Exception("PR File Not Found");
                            }
                            string srcFile = objiomfile.FirstOrDefault().FILENAME;
                            // Added by Aumento for SR84686
                            string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                            string path =  serverpath.getFileUploadPath("PR/");
                            if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                            string STRFILENAME = System.IO.Path.Combine(pathtosave, "PR" + IOM_Dtl.IndentNo.Trim() + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf");
                            // Added by Aumento for SR84686
                            //string STRFILENAME = "PR" + IOM_Dtl.IndentNo.Trim() + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";

                            string strDstFile =  serverpath.getFileUploadPath("PR/") + STRFILENAME;
                            GetDocumentWithAppendedContent_new(IOM_Dtl.prAppHis.Where(x => x.APPROVAL_STATUS == 1).ToList(), srcFile, STRFILENAME, 100, IOM_Dtl.prAuthSeq);
                            List<PRDetailViewModel> obj = new List<PRDetailViewModel>() { new PRDetailViewModel() { DOC_TYPE = "PRA", FILENAME = STRFILENAME, ADDITIONAL_INFO = "Approved PR" } };
                            _PrService.SaveAttachment(Convert.ToInt64(_sessionService.Get<string>("userID").ToString()), Convert.ToInt64(id_str), obj);
                            retVal = 1;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Error = ex.StackTrace.ToString();
                retVal = -1;
            }
            return Json(new { res = retVal, err_msg = Error });
        }

        public ActionResult Add_PRapp_MST()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<AD_orglevel_type> items = _PrService.getOrgLevelType();
            ViewBag.ContentProcess = new SelectList(items, "ADORGLEVELTYPEID", "LEVELTYPE");
            ViewBag.PlantList = _PrService.GetPRReleasePlants(); //  Added by Aumento ::  SR68003
            return View();
        }

        public ActionResult GetOrgUnitData(int id)
        {
            List<AD_orglevel> portaluser = _PrService.GetOrgUnitData(id);
            var data = (from a in portaluser
                        select new
                        {
                            ADORGLEVELID = a.ADORGLEVELID,
                            LEVELDESCRIP = a.LEVELDESCRIP
                        }).ToList();
            //string sJSON = oSerializer.Serialize(list);
            string sJSON = JsonSerializer.Serialize(data);
            return Json(sJSON);
        }

        public string AutocompleteSuggestionsFunDesig(string term)
        {

            List<Employee_Details> portaluser = _PrService.PortalAutocompleteSuggestionsFunDesig(term);
            var data = (from a in portaluser
                        select new
                        {
                            _DesigId = a._DesigId,
                            _Desig = a._Desig
                        }).ToList();
            //string sJSON = oSerializer.Serialize(list);
            string sJSON = JsonSerializer.Serialize(data);
            return sJSON;
        }

        public string AutocompleteSuggestionsActDesig(string term)
        {
            List<Employee_Details> portaluser = _PrService.PortalAutocompleteSuggestionsActDesig(term);
            var data = (from a in portaluser
                        select new
                        {
                            _DesigId = a._DesigId,
                            _Desig = a._Desig
                        }).ToList();
            //string sJSON = oSerializer.Serialize(list);
            string sJSON = JsonSerializer.Serialize(data);
            return sJSON;
        }
        public string PortalAutocompleteSuggestionsEmployee(string term, int Catid, int Orgid)
        {
            List<Employee_Details> portaluser = _PrService.PortalAutocompleteSuggestionsEmployee(term, Catid, Orgid);
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
        //public string AutocompleteSuggestionsORGUnit(int id, string term)
        //{
        //    List<Employee_Details> portaluser = _PrService.PortalAutocompleteSuggestionsORGUnit(term, id);
        //    int i = 0;
        //    List<string> list = new List<string>();
        //    foreach (var dataitem in portaluser)
        //    {

        //        list.Add(dataitem._ECode.ToString() + "-" + dataitem._EFirstName.ToString() + "");
        //    }
        //    System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    string sJSON = oSerializer.Serialize(list);
        //    return sJSON;
        //}

        public ActionResult GetDgitPraddapp_mst([FromBody] PR_DGIT_PRADDAPP_MST obj)
        {
            string Error = "";
            string sJSON = "";
            try
            {
                List<PR_DGIT_PRADDAPP_MST> mst = _PrService.Get_PRADDAPP_MST_List(obj);
                var data = (from a in mst
                            select new
                            {
                                PRADDAPP_MST_ID = a.PRADDAPP_MST_ID,
                                //ADORGLVLID = a.ADORGLVLID,
                                APPROVER = a.APPROVER,
                                FUNCTIONDESID = a.FUNCTIONDESID,
                                ACTUALDESGID = a.ACTUALDESGID,
                                AMOUNTRANGE_FROM = a.AMOUNTRANGE_FROM,
                                AMOUNTRANGE_TO = a.AMOUNTRANGE_TO,
                                //INDENTTYPE = a.INDENTTYPE,
                                INDENTTYPE = (
                                                a.INDENTTYPE == 1 ? "1-Service" : ""
                                             //a.INDENTTYPE == 2 ? "2-Material PR" : ""
                                             ),
                                //ISADDEDINLAST = a.ISADDEDINLAST,
                                ISADDEDINLAST = (
                                                    a.ISADDEDINLAST == 1 ? "1-Add in Last" :
                                                    a.ISADDEDINLAST == 2 ? "2-Add in First" : ""
                                                    ),
                                APPSEQ = a.APPSEQ,
                                ACTIONFOR = a.ACTIONFOR,

                                //ACTIONFOR = (
                                //                a.ACTIONFOR == "1" ? "1-Approve" :
                                //                a.ACTIONFOR == "1" ? "2-Send Back" : ""
                                //                ),
                                ISPRINTREQUIRED = a.ISPRINTREQUIRED,
                                STATUS = (a.STATUS == 1 ? false : true),
                                IS_SISPR = (a.IS_SISPR == null ? "" : a.IS_SISPR.ToString()),
                                APP_TYPEINFO = a.APP_TYPEINFO,
                                ISPARALELLAPP = (a.ISPARALELLAPP == 1 ? "1- Parallel Approval" : "0- Sequntial Approval"),
                               // PLANTID = a.PLANTID // Added by Aumento ::  SR68003
                            }).ToList();
              //  System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                sJSON = JsonSerializer.Serialize(data);
            }
            catch (Exception ex)
            {
                Error = "error";
            }
            //string js = JsonSerializer.Serialize(data);
            return Json(new { res = sJSON, err_msg = Error });
        }

        public ActionResult AddEditDgitPraddapp_mst([FromBody] Add_PRAppRequest request)
        {
            long id = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string JsonData = request.JsonData;
                int ADORGLVLID = request.ADORGLVLID;

                if (JsonData != "")
                {

                    List<DGIT_PRADDAPP> obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DGIT_PRADDAPP>>(JsonData);

                    List<PR_DGIT_PRADDAPP_MST> data = (from a in obj
                                                       select new PR_DGIT_PRADDAPP_MST
                                                       {
                                                           PRADDAPP_MST_ID = Convert.ToInt64(a.PRADDAPP_MST_ID),
                                                           ADORGLVLID = ADORGLVLID,
                                                           APPROVER = Convert.ToInt64(a.APPROVER),
                                                           FUNCTIONDESID = a.FUNCTIONDESID,
                                                           ACTUALDESGID = a.ACTUALDESGID,
                                                           AMOUNTRANGE_FROM = a.AMOUNTRANGE_FROM,
                                                           AMOUNTRANGE_TO = a.AMOUNTRANGE_TO,
                                                           //INDENTTYPE = Convert.ToInt16(a.INDENTTYPE),
                                                           INDENTTYPE = Convert.ToInt16(
                                                                            a.INDENTTYPE == "1-Service" ? 1 : 0
                                                                         //a.INDENTTYPE == "2-Material PR" ? 2 : 0
                                                                         ),
                                                           //ISADDEDINLAST = a.ISADDEDINLAST,
                                                           ISADDEDINLAST = Convert.ToInt16(
                                                                                a.ISADDEDINLAST == "1-Add in Last" ? 1 :
                                                                                a.ISADDEDINLAST == "2-Add in First" ? 2 : 0
                                                                            ),
                                                           APPSEQ = a.APPSEQ,
                                                           ACTIONFOR = a.ACTIONFOR,
                                                           //ACTIONFOR = (
                                                           //                 a.ACTIONFOR == "1-Approve" ? "1" :
                                                           //                 a.ACTIONFOR == "2-Send Back" ? "2" : ""
                                                           //                 ),
                                                           ISPRINTREQUIRED = Convert.ToInt16(a.ISPRINTREQUIRED),
                                                           ADDEDBY = Convert.ToInt16(_sessionService.Get<string>("userID")),
                                                           STATUS = Convert.ToInt16(a.STATUS == "true" ? 0 : 1),
                                                           IS_SISPR = (string.IsNullOrEmpty(a.IS_SISPR) ? ((short?)null) : Convert.ToInt16(a.IS_SISPR)), //Convert.ToInt16(string.IsNullOrEmpty(a.IS_SISPR) ? "0" : a.IS_SISPR),
                                                           //IS_SISPR = Convert.ToInt16(string.IsNullOrEmpty(a.IS_SISPR) ? "0" : a.IS_SISPR),
                                                           APP_TYPEINFO = a.APP_TYPEINFO,
                                                           ISPARALELLAPP = Convert.ToInt16(
                                                                                a.ISPARALELLAPP == "0- Sequntial Approval" ? 0 : 1
                                                                            ),
                                                           PLANTID = a.PLANTID // Added by Aumento ::  SR68003
                                                       }).ToList();

                    id = _PrService.AddEditDgitPraddapp_mst(data);
                }

            }
            catch (Exception ex)
            {
                id = 1;
            }
            return Json(id);
        }
        //================================================Start=================================================
        //                                 Allocator Data Management 29-08-2022 (Aumento)
        //======================================================================================================
        [HttpGet]
        public ActionResult AllocatorMaster()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<DGIT_PRCAT_MST> items = _PrService.getDgitPRCatMst();
            ViewBag.ContentProcess = new SelectList(items, "CATID", "CATDESC");
            return View();
        }

        public ActionResult Get_Properationmap_List(PR_dgit_properationmap obj)
        {
            string Error = "";
            string sJSON = "";
            try
            {
                List<PR_dgit_properationmap> mst = _PrService.Get_Properationmap_List(obj);
                //int Count = _PrService.Get_EmployeeMap_Count(obj);
                //if (Count > 0)
                //{
                //    Error = "0";
                //}
                //else
                //{
                List<AD_orglevel> OrgLevel = _PrService.GetOperationList();
                var data = (from a in OrgLevel
                            select new
                            {
                                PROPMAPID = (mst.Where(x => x.ADORGLVLID == a.ADORGLEVELID).Count() >= 1 ? mst.Where(x => x.ADORGLVLID == a.ADORGLEVELID).Select(x => x.PROPMAPID).FirstOrDefault() : 0),
                                //PRCAT = (mst.Where(x => x.ADORGLVLID == a.ADORGLEVELID).Count() >= 1 ? mst.Where(x => x.ADORGLVLID == a.ADORGLEVELID).Select(x => x.PRCAT).FirstOrDefault() : 0),
                                //ADEMPCODE = (mst.Where(x => x.ADORGLVLID == a.ADORGLEVELID).Count() >= 1 ? mst.Where(x => x.ADORGLVLID == a.ADORGLEVELID).Select(x => x.ADEMPCODE).FirstOrDefault() : 0),
                                ADORGLVLID = a.ADORGLEVELID,
                                LEVELDESCRIP = a.LEVELDESCRIP,
                                STATUS = (mst.Where(x => x.ADORGLVLID == a.ADORGLEVELID && x.STATUS == 1).Count() == 1 ? true : false),
                            }).ToList();
               // System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                sJSON = JsonSerializer.Serialize(data);

                //}
            }
            catch (Exception ex)
            {
                Error = "error";
            }
            //string js = JsonSerializer.Serialize(data);
            return Json(new { res = sJSON, err_msg = Error });
        }
        [HttpPost]
        public IActionResult SaveAllocatorMaster([FromBody] AllocatorMasterRequest request)
        {
            long id = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string JsonData = request.JsonData;
                int PRCAT = request.PRCAT;
                int ADEMPCODE = request.ADEMPCODE;


                if (JsonData != "")
                {

                    List<PR_dgit_properationmap> obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PR_dgit_properationmap>>(JsonData);

                    List<PR_dgit_properationmap> data = (from a in obj
                                                         select new PR_dgit_properationmap
                                                         {
                                                             PROPMAPID = a.PROPMAPID,
                                                             PRCAT = Convert.ToInt16(PRCAT),
                                                             ADEMPCODE = ADEMPCODE,
                                                             ADORGLVLID = a.ADORGLVLID,
                                                             ADDEDBY = Convert.ToInt16(_sessionService.Get<string>("userID")),
                                                             STATUS = a.STATUS
                                                         }).ToList();

                    id = _PrService.SaveAllocatorMaster(data);
                }

            }
            catch (Exception ex)
            {
                id = 1;
            }
            return Json(id);
        }
        public ActionResult DownloadExcelUserOperation()
        {
            try
            {
                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }
                string str = (string)TempData["EXCELFILE"];
               // HttpContext.Response.AddHeader("content-disposition", "attachment; filename=PROperationMap.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel", "PROperationMap.xls");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public ActionResult ExportToExcelUserOpertaion([FromBody] PR_dgit_properationmap SI)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<PR_dgit_properationmap> _headerObj = _PrService.getPR_properationmapReport(SI);
                string str = this.excelHtmlUser(_headerObj);
                if (str != "")
                {
                    TempData.Remove("EXCELFILE");
                    TempData["EXCELFILE"] = str;
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public string excelHtmlUser(List<PR_dgit_properationmap> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation Map Id </th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Category Id </th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Category Name </th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Code</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation Level Id</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Operation Name</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (var indent in _headerList)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.PROPMAPID + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.PRCAT + "</td>");
                    stringBuilder.Append("<td style='text-align:left;border: 1px solid;'>" + indent.CAtDESC + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.ADEMPCODE + "</td>");
                    stringBuilder.Append("<td style='text-align:left;border: 1px solid;'>" + indent.EMPNAME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.ADORGLVLID + "</td>");
                    stringBuilder.Append("<td style='text-align:left;border: 1px solid;'>" + indent.ORGDESC + "</td>");

                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }
        //=======================================End (Allocator Data Management )===============================

        //================================================Start=================================================
        //                                 Buyer Data Management 29-08-2022 (Aumento)
        //======================================================================================================
        [HttpGet]
        public ActionResult prBuyerMaster()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<DGIT_PRCAT_MST> items = _PrService.getDgitPRCatMst();
            ViewBag.ContentProcess = new SelectList(items, "CATID", "CATDESC");
            return View();
        }

        public string GetOrgLevelData(string term)
        {

            List<AD_orglevel> OrgLevel = _PrService.PortalAutocompleteOrgLevel(term);
            var data = (from a in OrgLevel
                        orderby a.ADORGLEVELID ascending
                        select new
                        {
                            ADORGLEVELID = a.ADORGLEVELID,
                            LEVELDESCRIP = a.LEVELDESCRIP
                        }).ToList();
            //string sJSON = oSerializer.Serialize(list);
            string sJSON = JsonSerializer.Serialize(data);
            return sJSON;
        }

        public ActionResult Get_prbuyermst_List([FromBody] PRBuyerMstViewModel obj)
        {
            string Error = "";
            string sJSON = "";
            try
            {
                List<PRBuyerMstViewModel> OrgLevel = _PrService.getPRBuyerMstList(obj);

                var data = (from a in OrgLevel
                            orderby a.ADEMPCODE ascending
                            select new
                            {
                                PRBUYERMSTID = a.PRBUYERMSTID,
                                ADEMPCODE = a.ADEMPCODE,
                                ADEMPNAME = a.ADEMPNAME

                            }).ToList();

               // System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                sJSON = JsonSerializer.Serialize(data);
            }
            catch (Exception ex)
            {
                Error = "error";
            }
            //string js = JsonSerializer.Serialize(data);
            return Json(new { res = sJSON, err_msg = Error });
        }
        [HttpPost]
        public IActionResult SaveBuyerMaster([FromBody] BuyerMasterRequest request)
        {
            long id = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string JsonData = request.JsonData;
                int PRCAT = request.PRCAT;
                int ADORGLEVELID = request.ADORGLEVELID;

                if (JsonData != "")
                {
                    List<PRBuyerMstViewModel> obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PRBuyerMstViewModel>>(JsonData);

                    List<PRBuyerMstViewModel> data = (from a in obj
                                                      select new PRBuyerMstViewModel
                                                      {
                                                          PRBUYERMSTID = a.PRBUYERMSTID,
                                                          ADORGLEVELID = ADORGLEVELID,
                                                          PRCAT = PRCAT,
                                                          ADEMPCODE = a.ADEMPCODE,
                                                          ACTIVE = a.ACTIVE,
                                                          ADDEDBY = Convert.ToInt16(_sessionService.Get<string>("userID"))
                                                      }).ToList();

                    id = _PrService.SaveBuyerMaster(data);
                }

            }
            catch (Exception ex)
            {
                id = 1;
            }
            return Json(id);
        }

        public ActionResult DownloadExcelBuyer()
        {
            try
            {
                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }
                string str = (string)TempData["EXCELFILE"];
               // HttpContext.Response.AddHeader("content-disposition", "attachment; filename=PRBuyerMap.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel", "PRBuyerMap.xls");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public ActionResult ExportToExcelBuyer([FromBody] PRBuyerMstViewModel obj)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<PRBuyerMstViewModel> _headerObj = _PrService.getPR_BuyermapReport(obj);
                string str = this.excelHtmlBuyer(_headerObj);
                if (str != "")
                {
                    TempData.Remove("EXCELFILE");
                    TempData["EXCELFILE"] = str;
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public string excelHtmlBuyer(List<PRBuyerMstViewModel> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Buyer Map Id </th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Category Id </th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Category Name </th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation Level Id</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Operation Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Code</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Name</th>");

                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (var indent in _headerList)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.PRBUYERMSTID + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.PRCAT + "</td>");
                    stringBuilder.Append("<td style='text-align:left;border: 1px solid;'>" + indent.CATNAME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.ADORGLEVELID + "</td>");
                    stringBuilder.Append("<td style='text-align:left;border: 1px solid;'>" + indent.LEVELDESCRIP + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.ADEMPCODE + "</td>");
                    stringBuilder.Append("<td style='text-align:left;border: 1px solid;'>" + indent.ADEMPNAME + "</td>");


                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }
        //=======================================End (Buyer Data Management )===============================

        //===SR52365=============================
        public ActionResult BindDivisionByKI(long id)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            long op_Id = employeeDetails._OpId == null ? 0 : (long)employeeDetails._OpId;
            long adempcode = long.Parse(_sessionService.Get<string>("userID"));

            List<PR_Div_Dep_SecViewModel> divList = new List<PR_Div_Dep_SecViewModel>();
            Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> _tuple = _PrService.BindDivision(op_Id, id, adempcode); //KI coming in id parameter
            divList = _tuple.Item5;
            return Json(new { iList = divList, opID = _tuple.Item1, divID = _tuple.Item2, deptID = _tuple.Item3, secID = _tuple.Item4 });
        }

        public ActionResult BindDepartmentByDivKI(long id, long ki)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            long adempcode = long.Parse(_sessionService.Get<string>("userID"));

            List<PR_Div_Dep_SecViewModel> depList = new List<PR_Div_Dep_SecViewModel>();
            Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> _tuple = _PrService.BindDepartment(id, ki, adempcode);
            depList = _tuple.Item5;
            return Json(new { iList = depList, opID = _tuple.Item1, divID = _tuple.Item2, deptID = _tuple.Item3, secID = _tuple.Item4 });
        }

        public ActionResult BindSectionByDeptKI(long id, long ki)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            long adempcode = long.Parse(_sessionService.Get<string>("userID"));

            List<PR_Div_Dep_SecViewModel> secList = new List<PR_Div_Dep_SecViewModel>();
            Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> _tuple = _PrService.BindSection(0, id, ki, adempcode);
            secList = _tuple.Item5;
            return Json(new { iList = secList, opID = _tuple.Item1, divID = _tuple.Item2, deptID = _tuple.Item3, secID = _tuple.Item4 });
        }
        //======================

        [HttpGet]
        public ActionResult PRPIUserReport()
        {
            SearchIndentUser obj = new SearchIndentUser();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                ViewBag.KICodeList = new SelectList(_PrService.GetKICodeList(Convert.ToInt64(_sessionService.Get<string>("userID").ToString())), "SYKIID", "KICODE");

                ModelState.Clear();
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult PRPIUserReport([FromBody] SearchIndentUser SI)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SearchIndentUser _headerObj = _PrService.PRPIUserDashboard(SI);
            ModelState.Clear();
            return PartialView("_GetPRPIUserReport", _headerObj);
        }

        // -> Added by Aumento as on 16072024 
        [HttpGet]
        public ActionResult EditPRRequestUploadBeforeHold(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            PRHeaderViewModel PHVM = _PrService.GetPRRequestById(_ReqId);
            //CommonFunctions cm = new CommonFunctions();
            string strSISoperationID = cm.GetParameterValue("SIS_OPERATION");
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
            if (strSISoperationID == obj.Operation_Id)
                TempData["ISPRTYPEENABLED"] = "1";
            else
                TempData["ISPRTYPEENABLED"] = "0";

            if (PHVM.IS_SISPR != null)
            {
                PHVM.ITServiceMatSISAppStatus = true;
            }
            else
            {
                PHVM.ITServiceMatSISAppStatus = false;
            }

            if (PHVM.IS_NONSISPR != null)
            {
                PHVM.NonITServiceMatDeclaration = true;
            }
            else
            {
                PHVM.NonITServiceMatDeclaration = false;
            }
            //SIS PR Change

            if (PHVM.skipAuthList != null)
            {
                //TempData["DELETED_AUTH_LIST"] = PHVM.skipAuthList;
                TempData["DELETED_AUTH_LIST"] = JsonSerializer.Serialize(PHVM.skipAuthList);
                TempData.Keep();
            }

            ViewBag.strId = id;

            ViewBag.CategoryList = new SelectList(_PrService.BindPRCategory(), "CATID", "CATDESC");
            long orgLvlID = Convert.ToInt64(obj.Operation_Id);
            List<PRBuyerMstViewModel> _BuyerList = _PrService.GetABuyerMstList(orgLvlID);
            ViewBag.BuyerList = new SelectList(_BuyerList, "ADEMPCODE", "ADEMPNAME");

            short HoldStatus = 4;
            DateTime HoldDateTime = PHVM.prAppHis.Where(x => x.APPROVAL_STATUS == HoldStatus).OrderByDescending(y => y.PRAPPHISTORY_ID).Select(x => x.ADDEDDATE).FirstOrDefault();
            List<PRDetailViewModel> PRDetail_ = new List<PRDetailViewModel>();
            if (HoldDateTime != null)
            {

                PRDetail_ = PHVM.prDetail.Where(x => x.ADDEDDATE > HoldDateTime).ToList();
            }
            PHVM.prDetail = PRDetail_;

            return View(PHVM);
        }

        [HttpPost]
        public ActionResult EditPRRequestUploadBeforeHold([FromBody] PRHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.prDetail = new List<PRDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                model.UPDATEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                //added by aumento for SR86919 
                // Add a release hold entry and update process status
                retVal = _PrService.AddReleaseHoldentry(model);
                if (retVal == 1)
                {
                    retVal = _PrService.SendMailByRequestorAfterHoldandUploadDoc(model);
                }
                //added by aumento for SR86919
                //retVal = _PrService.SendMailByRequestorAfterHoldandUploadDoc(model);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        //Added by aumento as on 19092024 for the SR71870============================================================
        [HttpGet]
        public ActionResult PRDashboard()
        {
            SearchIndentUser obj = new SearchIndentUser();
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
                ViewBag.KICodeList = new SelectList(_PrService.GetKICodeList_ForPRDBOperationwise(Convert.ToInt64(_sessionService.Get<string>("userID").ToString())), "SYKIID", "KICODE");
                //_KIList = _IomService.GetKiLIST(Convert.ToInt64(employeeDetails.Employee_Code));               

                if (ViewBag.KICodeList != null)
                {
                    var kiCodeList = ViewBag.KICodeList as SelectList;
                    if (kiCodeList != null && kiCodeList.Any())
                    {
                        var firstRecord = kiCodeList.Cast<SelectListItem>().FirstOrDefault();
                        if (firstRecord != null)
                        {
                            obj.KIID = Convert.ToInt64(firstRecord.Value);
                        }
                    }
                }

                if (TempData["KIID"] == null || Convert.ToString(TempData["KIID"]) == "")
                {
                    //obj.KIID = _KIList.FirstOrDefault().Value;
                    obj.KIID = obj.KIID;
                }
                else
                {
                    obj.KIID = Convert.ToInt64(TempData["KIID"]);
                }


                Employee_Details employeeDetailski = _PrService.GetEmployeeDetail(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                obj.OperationID = employeeDetailski._OpId == null ? 0 : (long)employeeDetailski._OpId;
                obj.DivisionID = employeeDetailski._DivId == null ? 0 : (long)employeeDetailski._DivId;
                obj.DEPTID = employeeDetailski._DepId == null ? 0 : (long)employeeDetailski._DepId;
                obj.SECID = employeeDetailski._SecId == null ? 0 : (long)employeeDetailski._SecId;
                obj.IsTeamMember = (employeeDetailski.Functional_Designation_Id == "" ? 1 : 0);


                Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = _PrService.BindOperation(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                obj.ISSpecialRight = OPtemp.Item1;

                if (obj.IsTeamMember == 1 && obj.ISSpecialRight == 0)
                {
                    obj.ecode = Convert.ToInt64(employeeDetails.Employee_Code);
                }


                if (obj.ISSpecialRight == 1)
                {
                    _OPList = OPtemp.Item2;
                    long opid = _OPList.FirstOrDefault().Value;
                    Tuple<long, List<PR_Div_Dep_SecViewModel>> divtemp = _PrService.BindDivision_PRDB(opid, Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
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

                    //Below chanaged by aumento as on 22092024===================================
                    //if (obj.DivisionID != 0)
                    //{                      

                    //    _DivList.Add(new PR_Div_Dep_SecViewModel
                    //    {
                    //        Value = Convert.ToInt64(employeeDetailski._DivId),
                    //        Text = employeeDetailski._DivDesc.ToString()
                    //    });
                    //}
                    //else
                    //{
                    //    Tuple<long, List<PR_Div_Dep_SecViewModel>> divtemp = _PrService.BindDivision_PRDB(obj.OperationID, Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                    //    if (divtemp.Item2.Count > 0)
                    //    {
                    //        _DivList = divtemp.Item2;
                    //    }
                    //}

                    if (obj.DivisionID != 0)
                    {
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> divtemp = _PrService.BindDivision_PRDB(obj.OperationID, Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID);
                        if (divtemp.Item2.Count > 0)
                        {
                            _DivList = divtemp.Item2;
                        }
                    }

                    //======================================================================


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
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> dpttemp = _PrService.BindDepartment(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID, obj.OperationID, obj.DivisionID);
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
                        Tuple<long, List<PR_Div_Dep_SecViewModel>> sectemp = _PrService.BindSection(Convert.ToInt64(employeeDetails.Employee_Code), obj.KIID, obj.OperationID, obj.DivisionID, obj.DEPTID);
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
                ViewBag.KIList = new SelectList(_KIList, "Value", "Text");

                //ModelState.Clear();
                return View("PRDashboard", obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //====================================================================================================
        }

        [HttpPost]
        public ActionResult PRDashboadReport([FromBody] SearchIndentUser SI)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SearchIndentUser _headerObj = _PrService.PRDashboadReport(SI);
            ModelState.Clear();
            return PartialView("_GetPRDashboardReport", _headerObj);
        }
        //===========================================================================================================

        // <- Added by Aumento as on 16072024 


    }
    public class LocationTextExtractionStrategyWithPosition : LocationTextExtractionStrategy
    {

        private readonly List<TextChunk> locationalResult = new List<TextChunk>();

        private readonly ITextChunkLocationStrategy tclStrat;

        public LocationTextExtractionStrategyWithPosition() : this(new TextChunkLocationStrategyDefaultImp())
        {
        }

        /**
         * Creates a new text extraction renderer, with a custom strategy for
         * creating new TextChunkLocation objects based on the input of the
         * TextRenderInfo.
         * @param strat the custom strategy
         */
        public LocationTextExtractionStrategyWithPosition(ITextChunkLocationStrategy strat)
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

    public class TextLocation
    {
        public float X { get; set; }
        public float Y { get; set; }

        public string Text { get; set; }
    }
}