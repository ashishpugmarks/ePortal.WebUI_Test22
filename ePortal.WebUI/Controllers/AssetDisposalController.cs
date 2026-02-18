//using ePortal.BusinessLibraries;
using ePortal.DomainClasses;
using ePortal.Persistence.Services;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using System.Data;
using ePortal.WebUI.Filters;
using ePortal.Application.Contracts;
using ePortal.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Hosting;
using ePortal.Shared;
using System.Text;
using ePortal.WebUI.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ePortal.Persistence.Interface;
using DocumentFormat.OpenXml.Office2010.Excel;
using static ePortal.WebUI.Controllers.AssetDisposalController;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.tool.xml;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class AssetDisposalController : Controller
    {
        IAssetDisposalService _AssetService;
        private readonly ISessionService _sessionService;
        ///------ SAP Connection Object -------////
        //private readonly EportalESS _objasset;
        private readonly IEportalESS _objasset;
        public AssetDisposalController(IAssetDisposalService AssetService, ISessionService sessionService, IEportalESS objasset)
        {
            _AssetService = AssetService;
            _sessionService = sessionService;
            _objasset = objasset;
        }

        #region Asset request

        [HttpGet]
        //public List<AssetDetailViewModel> GetAssetDetail()
        public async Task<List<AssetDetailViewModel>> GetAssetDetail()
        {
            List<AssetDetailViewModel> iList = new List<AssetDetailViewModel>();
            DataTable dt = new DataTable();

            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            int profitCenter = (int)_AssetService.GetSiteDetailBySiteId((long)employeeDetails._SiteId).PROFIT_CENTER;
            long? operationMappId = _AssetService.GetOperationMappId(employeeDetails);
            string fnCode = _AssetService.GetFnCodeByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)employeeDetails._OpId);
            int opCount = _AssetService.GetOperationCount(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)employeeDetails._OpId);


            //dt = _objasset.GetAssetsDetails(fnCode, (opCount == 0 ? profitCenter.ToString("D10") : null));

            dt = await _objasset.GetAssetsDetails(fnCode, (opCount == 0 ? profitCenter.ToString("D10") : null));

            if (dt.Rows.Count > 0)
            {
                List<SYSITE> _SiteList = _AssetService.GetSiteList();
                iList = dt.AsEnumerable().Select(row => new AssetDetailViewModel
                {
                    ASSETCODE = row["ANLN12"].ToString(),
                    ASSETCLASSNAME = row["TXK50"].ToString(),
                    ASSETDESCRIPTION = row["TXT50"].ToString(),
                    ASSETDETAIL = row["ANLHTXT"].ToString(),
                    VENDORCODE = Convert.ToString(row["LIFNR"]),
                    VENDORNAME = Convert.ToString(row["LIEFE"]),
                    INVOICENO = Convert.ToString(row["XBLNR"]),
                    INVOICEDATE = Convert.ToString(row["BLDAT"]) == "0000-00-00" ? "" : Convert.ToDateTime(Convert.ToString(row["BLDAT"])).ToString("yyyy-MM-dd"),
                    ORIGINALCOST = Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(row["KANSW"])) ? 0 : row["KANSW"]),
                    DEPRECIATIONCOST = Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(row["KNAFA"])) ? 0 : row["KNAFA"]),
                    WDV = Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(row["KANSW"])) ? 0 : row["KANSW"]) - Convert.ToDecimal(string.IsNullOrEmpty(Convert.ToString(row["KNAFA"])) ? 0 : row["KNAFA"]),
                    SERIALNUMBER = row["SERNR"].ToString(),
                    PONUMBER = row["TYPBZ"].ToString(),
                    PROFITCENTER = row["PRCTR"].ToString(),
                    //Change by aumento as on 02082023 for SR51823============================
                    COSTCENTER = row["KOSTL"].ToString(),
                    //========================================================================
                    PLANTID = Convert.ToInt64(_SiteList.FirstOrDefault(w => w.PROFIT_CENTER == Convert.ToInt16(row["PRCTR"].ToString() == "0000001100" ? "0000001000" : row["PRCTR"].ToString()))?.SYSITEID ?? 0),
                    PLANTNAME = _SiteList.FirstOrDefault(w => w.PROFIT_CENTER == Convert.ToInt16(row["PRCTR"].ToString() == "0000001100" ? "0000001000" : row["PRCTR"].ToString()))?.DESCRIP,
                    CAPTALIZED_DATE = Convert.ToDateTime(row["AKTIV"].ToString()),
                }).OrderBy(o => o.INVOICEDATE).ToList();
            }

            return iList.Where(x => x.PLANTID > 0).ToList();
        }

        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        //public List<AssetDetailViewModel> GetAssetDetail()
        //{
        //    ///------ SAP Connection Object -------////
        //    EportalESS objasset = new EportalESS();
        //    List<AssetDetailViewModel> iList = new List<AssetDetailViewModel>();
        //    DataTable dt = new DataTable();
        //    Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
        //    int _profitCenter = (int)_AssetService.GetSiteDetailBySiteId((long)_Employee_Details._SiteId).PROFIT_CENTER;
        //    long? operationMappId = _AssetService.GetOperationMappId(_Employee_Details);
        //    string _fnCode = _AssetService.GetFnCodeByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_Employee_Details._OpId);
        //    int _opCount = _AssetService.GetOperationCount(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_Employee_Details._OpId);
        //    dt = objasset.GetAssetsDetails(_fnCode, (_opCount == 0 ? _profitCenter.ToString("D10") : null));
        //    if (dt.Rows.Count > 0)
        //    {
        //        List<SYSITE> _SiteList = _AssetService.GetSiteList();
        //        iList = (from DataRow row in dt.AsEnumerable()
        //                 select new AssetDetailViewModel
        //                 {
        //                     ASSETCODE = row["ANLN12"].ToString(),
        //                     //Change by aumento as on 02082023 for SR51823============================
        //                     ASSETCLASSNAME = row["TXK50"].ToString(),
        //                     //========================================================================
        //                     ASSETDESCRIPTION = row["TXT50"].ToString(),
        //                     ASSETDETAIL = row["ANLHTXT"].ToString(),
        //                     VENDORCODE = Convert.ToString(row["LIFNR"]),
        //                     VENDORNAME = Convert.ToString(row["LIEFE"]),
        //                     INVOICENO = Convert.ToString(row["XBLNR"]),
        //                     INVOICEDATE = Convert.ToString(row["BLDAT"]) == "0000-00-00" ? "" : Convert.ToDateTime(Convert.ToString(row["BLDAT"])).ToString("yyyy-MM-dd"),
        //                     ORIGINALCOST = Convert.ToDecimal(Convert.ToString(row["KANSW"]) == "" ? 0 : row["KANSW"]),
        //                     DEPRECIATIONCOST = Convert.ToDecimal(Convert.ToString(row["KNAFA"]) == "" ? 0 : row["KNAFA"]),
        //                     WDV = Convert.ToDecimal(Convert.ToString(row["KANSW"]) == "" ? 0 : row["KANSW"]) - Convert.ToDecimal(Convert.ToString(row["KNAFA"]) == "" ? 0 : row["KNAFA"]),
        //                     SERIALNUMBER = row["SERNR"].ToString(),
        //                     PONUMBER = row["TYPBZ"].ToString(),
        //                     PROFITCENTER = row["PRCTR"].ToString(),
        //                     //Change by aumento as on 02082023 for SR51823============================
        //                     COSTCENTER = row["KOSTL"].ToString(),
        //                     //========================================================================
        //                     PLANTID = Convert.ToInt64(_SiteList.Where(w => w.PROFIT_CENTER == Convert.ToInt16(row["PRCTR"].ToString() == "0000001100" ? "0000001000" : row["PRCTR"].ToString())).Select(s => s.SYSITEID).FirstOrDefault()),
        //                     PLANTNAME = _SiteList.Where(w => w.PROFIT_CENTER == Convert.ToInt16(row["PRCTR"].ToString() == "0000001100" ? "0000001000" : row["PRCTR"].ToString())).Select(s => s.DESCRIP).FirstOrDefault(),
        //                     CAPTALIZED_DATE = Convert.ToDateTime(row["AKTIV"].ToString()),
        //                 }).OrderBy(o => o.INVOICEDATE).ToList();
        //    }
        //    return iList.Where(x => x.PLANTID > 0).ToList();
        //}

        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        [HttpGet]
        //public List<CustomerDetailViewModel> GetCustomerDetail()
       public async Task<List<CustomerDetailViewModel>> GetCustomerDetail()
        {
            List<CustomerDetailViewModel> iList = new List<CustomerDetailViewModel>();
            DataTable dt = await _objasset.GetCustomerDetails() ?? new DataTable();
           

            if (dt.Rows.Count > 0)
            {
                iList = dt.AsEnumerable()
                          .Select(row => new CustomerDetailViewModel
                          {
                              CUSTOMERCODE = row["KUNNR"]?.ToString() ?? string.Empty,
                              CUSTOMERNAME = row["NAME1"]?.ToString() ?? string.Empty,
                              CUSTADDRESS = GetCustomerAddress(row["NAME_CO"]?.ToString() ?? string.Empty,
                                                               row["STREET"]?.ToString() ?? string.Empty,
                                                               row["CITY1"]?.ToString() ?? string.Empty,
                                                               row["POST_CODE1"]?.ToString() ?? string.Empty),
                              CUSTOMERNAME_CODE = $"{row["NAME1"]?.ToString() ?? string.Empty} - {row["KUNNR"]?.ToString() ?? string.Empty}",
                              CITY1 = row["CITY1"]?.ToString() ?? string.Empty,
                              POSTALCODE = row["POST_CODE1"]?.ToString() ?? string.Empty,
                              CUSTGSTIN = row["STCD3"]?.ToString() ?? string.Empty
                          }).OrderBy(o => o.CUSTOMERNAME, StringComparer.OrdinalIgnoreCase)
                          .ToList();
            }

            return iList;
        }


        public string GetCustomerAddress(string NAME_CO, string STREET, string CITY, string POSTCODE)
        {
            string retval = string.Empty;
            if (NAME_CO != "" && CITY != "" && POSTCODE != "" && STREET != "")
            {
                retval = NAME_CO + ", " + STREET + ", " + CITY + " - " + POSTCODE;
            }
            else if (CITY != "" && POSTCODE != "" && STREET != "")
            {
                retval = STREET + ", " + CITY + " - " + POSTCODE;
            }
            else if (NAME_CO != "" && STREET != "")
            {
                retval = NAME_CO + ", " + STREET;
            }
            else
            {
                retval = STREET;
            }
            return retval;
        }
        [HttpGet]
        public ActionResult ManageAssetRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View("ManageAssetRequest", _AssetService.GetAssetRequestList(Convert.ToInt64(_sessionService.Get<string>("userID").ToString())));
        }
        [HttpGet]
        public ActionResult AssetRequestDetail(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("DetailAssetRequest", _AssetService.GetDetailById(id, _Employee_Details));
        }

        public ActionResult ViewMgfApprovalHistory(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ViewMgfApprovalHistory", _AssetService.GetAssetRequestByMgf(id, _Employee_Details));
        }

        [HttpGet]
        public async Task<ActionResult> CreateAssetRequest()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                AssetDisposalViewModel ADVM_Model = new AssetDisposalViewModel();
                List<SIS_ASSETTYPE_MST> _AssetTypeList = _AssetService.GetAssetTypeList();
                ViewBag.AssetType = new SelectList(_AssetTypeList, "SIS_ASSETTYPE_MSTID", "DESCRIPTION");
                List<CustomerDetailViewModel> _iList = await GetCustomerDetail();
                ViewBag.Customers = new SelectList(_iList, "CUSTOMERCODE", "CUSTOMERNAME_CODE");

                TempData["CUSTOMER_LIST"] = JsonConvert.SerializeObject(_iList);
                //ADVM_Model.SapAssetList = GetAssetDetail();
                ADVM_Model.SapAssetList = await GetAssetDetail(); // Added

                ////////// Get authority ////////
                //long? operationMappId = _AssetService.GetOperationMappId(_Employee_Details);
                //string codeByOperationId = _AssetService.GetFnCodeByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_Employee_Details._OpId);
                //short Status_Level = string.IsNullOrEmpty(codeByOperationId)
                //        ? (short)1 : string.IsNullOrEmpty(_AssetService.CheckValidationByFnCode(codeByOperationId, "Maintenance Dept. Approval"))
                //        ? (short)1 : (short)22;
                //ApprovalAuthorityViewModel AAVM = _AssetService.GetApprovalAuthorityBySiteId(Convert.ToInt64(_Employee_Details._SiteId), Status_Level);
                //if (AAVM != null)
                //{
                //    string str = Status_Level == 1 ? "Pending at Taxation Dept." : "Pending at Maintenance Dept.";
                //    ADVM_Model.ForwardedTo = str + " - " + AAVM.EMPNAME;
                //}

                string validationParmValue = _AssetService.GetValidationParmValue("IBM Approval");
                ViewBag.VaildIbmAmt = string.IsNullOrEmpty(validationParmValue) ? 0 : Convert.ToInt64(validationParmValue);

                return View(ADVM_Model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult SaveAssetDetail([FromBody] AssetDisposalViewModel model)
        {
            short retVal = 0; long hearderId = 0;
            try
            {
                var userIdString = _sessionService.Get<string>("userID");
                if (userIdString == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                model.ADEMPCODE = Convert.ToInt32(userIdString);
                model.ADDEDBY = Convert.ToInt64(userIdString);
                model.MODIFIEDBY = Convert.ToInt64(userIdString);
                Tuple<short, long> retVal_tuple = _AssetService.SaveAssetItem(model);
                retVal = retVal_tuple.Item1;
                hearderId = retVal_tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(new { res = retVal, headerId = hearderId }, JsonRequestBehavior.AllowGet);
            return Json(new { res = retVal, headerId = hearderId });
        }

        [HttpPost]
        public ActionResult CreateAssetRequest([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];               
                //string path = Server.MapPath("~/Uploads/AssetDisposal/");
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                string basePath = serverpath.getFileUploadPath();
                string path = Path.Combine(basePath, "AssetDisposal/");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                long userID = Convert.ToInt64(_sessionService.Get<string>("userID"));

                ADVM.ADEMPCODE = userID;
                ADVM.ADDEDBY = userID;
                ADVM.MODIFIEDBY = userID;

                ADVM.Emp_Detail = new Employee_Details()
                {
                    _SiteId = employeeDetails._SiteId
                };
                if (ADVM.ASSETTYPEID == 1)
                {
                    AttachmentViewModel _BACKUP_ATTACHMENT = new AttachmentViewModel();
                    string _backupFileName = string.Empty;
                    if (TempData["BACKUP_ATTACHMENT"] != null)
                    {
                        _BACKUP_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["BACKUP_ATTACHMENT"].ToString());                        
                        _backupFileName = GetFileNameStr(_BACKUP_ATTACHMENT.FileName.Trim());
                        ADVM.BACKUP_ATTACHMENT_NAME = _backupFileName;
                    }
                    else
                    {
                        _BACKUP_ATTACHMENT = null;
                        retVal = 3; //// Please upload attachments.
                    }
                    AttachmentViewModel _TRANSFERSLIP_ATTACHMENT = (AttachmentViewModel)null;
                    string _transferFileName = string.Empty;
                    if (TempData["TRANSFERSLIP_ATTACHMENT"] != null)
                    {
                        _TRANSFERSLIP_ATTACHMENT = JsonConvert.DeserializeObject <AttachmentViewModel>(TempData["TRANSFERSLIP_ATTACHMENT"].ToString());
                        _transferFileName = GetFileNameStr(_TRANSFERSLIP_ATTACHMENT.FileName.Trim());
                        ADVM.TRANSFERSLIP_ATTACHMENT_NAME = _transferFileName;
                    }
                    retVal = _AssetService.SaveAssetRequest(ADVM, employeeDetails);
                    if (retVal == 1)
                    {
                        if (_BACKUP_ATTACHMENT != null)
                        {
                            using (var memStream = new MemoryStream(_BACKUP_ATTACHMENT.fileBytes))
                            {
                                System.IO.File.WriteAllBytes(path + _backupFileName, memStream.ToArray());
                            }
                        }
                        if (_TRANSFERSLIP_ATTACHMENT != null)
                        {
                            using (var memStream = new MemoryStream(_TRANSFERSLIP_ATTACHMENT.fileBytes))
                            {
                                System.IO.File.WriteAllBytes(path + _transferFileName, memStream.ToArray());
                            }
                        }
                    }
                }
                else
                {
                    AttachmentViewModel _BACKUP_ATTACHMENT = new AttachmentViewModel();
                    string _backupFileName = string.Empty;
                    if (TempData["BACKUP_ATTACHMENT"] != null)
                    {
                        _BACKUP_ATTACHMENT = JsonConvert.DeserializeObject <AttachmentViewModel>(TempData["BACKUP_ATTACHMENT"].ToString());                       
                        _backupFileName = GetFileNameStr(_BACKUP_ATTACHMENT.FileName.Trim());
                        ADVM.BACKUP_ATTACHMENT_NAME = _backupFileName;
                    }
                    else
                    {
                        _BACKUP_ATTACHMENT = null;
                        retVal = 3; //// Please upload attachments.
                    }
                    AttachmentViewModel _TRANSFERSLIP_ATTACHMENT = (AttachmentViewModel)null;
                    string _transferFileName = string.Empty;
                    if (TempData["TRANSFERSLIP_ATTACHMENT"] != null)
                    {
                        _TRANSFERSLIP_ATTACHMENT = JsonConvert.DeserializeObject <AttachmentViewModel>(TempData["TRANSFERSLIP_ATTACHMENT"].ToString());
                        _transferFileName = GetFileNameStr(_TRANSFERSLIP_ATTACHMENT.FileName.Trim());
                        ADVM.TRANSFERSLIP_ATTACHMENT_NAME = _transferFileName;
                    }

                    retVal = _AssetService.SaveAssetRequest(ADVM, employeeDetails);
                    if (retVal == 1)
                    {
                        if (_BACKUP_ATTACHMENT != null)
                        {
                            using (var memStream = new MemoryStream(_BACKUP_ATTACHMENT.fileBytes))
                            {
                                System.IO.File.WriteAllBytes(path + _backupFileName, memStream.ToArray());
                            }
                        }
                        if (_TRANSFERSLIP_ATTACHMENT != null)
                        {

                            using (var memStream = new MemoryStream(_TRANSFERSLIP_ATTACHMENT.fileBytes))
                            {
                                System.IO.File.WriteAllBytes(path + _transferFileName, memStream.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult SAPCustomerDetailByCode([FromBody] CustomerDetailViewModel CDVM)
        {
            try                
               
            {
                //if (!string.IsNullOrEmpty(HttpContext.Session.GetString("CUSTOMER_LIST")))
                //{
                //    var customerListJson = HttpContext.Session.GetString("CUSTOMER_LIST");
                //    var _CustList = JsonConvert.DeserializeObject<List<CustomerDetailViewModel>>(customerListJson) ?? new List<CustomerDetailViewModel>();

                //    // Ensure CDVM is not null before accessing properties
                //    if (CDVM != null && !string.IsNullOrEmpty(CDVM.CUSTOMERCODE))
                //    {
                //        CDVM = _CustList.FirstOrDefault(x => x.CUSTOMERCODE == CDVM.CUSTOMERCODE);
                //    }

                //    TempData.Keep("CUSTOMER_LIST"); 
                //}
                if (TempData["CUSTOMER_LIST"] != null)
                {
                    //List<CustomerDetailViewModel> _CustList = (List<CustomerDetailViewModel>)TempData["CUSTOMER_LIST"];

                    List<CustomerDetailViewModel> _CustList = JsonConvert.DeserializeObject<List<CustomerDetailViewModel>>(TempData["CUSTOMER_LIST"].ToString());

                    if (!string.IsNullOrEmpty(CDVM.CUSTOMERCODE))
                    {
                        CDVM = _CustList.Where(x => x.CUSTOMERCODE == CDVM.CUSTOMERCODE).FirstOrDefault();
                    }
                    TempData.Keep();
                }
            }
            catch (Exception ex)
            {
                CDVM = new CustomerDetailViewModel();
            }
            //return Json(CDVM, JsonRequestBehavior.AllowGet);
            return Json(CDVM);
        }

        //[HttpPost]
        //public ActionResult UploadAttachment(AssetDisposalViewModel formData)
        //{
        //    short retVal = 0;
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        if (formData.ASSETTYPEID == 2)
        //        {
        //            if (formData.BACKUP_ATTACHMENT.ContentLength > 0 /*&& formData.MUTILATION_ATTACHMENT.ContentLength > 0*/ )
        //            {
        //                AttachmentViewModel _backupAttachment = new AttachmentViewModel();
        //                _backupAttachment.FileName = Path.GetFileName(formData.BACKUP_ATTACHMENT.FileName);
        //                _backupAttachment.fileBytes = new byte[formData.BACKUP_ATTACHMENT.ContentLength];
        //                formData.BACKUP_ATTACHMENT.InputStream.Read(_backupAttachment.fileBytes, 0, formData.BACKUP_ATTACHMENT.ContentLength);
        //                TempData["BACKUP_ATTACHMENT"] = _backupAttachment;
        //                retVal = 1;
        //            }
        //            if (formData.TRANSFERSLIP_ATTACHMENT != null)
        //            {
        //                AttachmentViewModel _slipAttachment = new AttachmentViewModel();
        //                _slipAttachment.FileName = Path.GetFileName(formData.TRANSFERSLIP_ATTACHMENT.FileName);
        //                _slipAttachment.fileBytes = new byte[formData.TRANSFERSLIP_ATTACHMENT.ContentLength];
        //                formData.TRANSFERSLIP_ATTACHMENT.InputStream.Read(_slipAttachment.fileBytes, 0, formData.TRANSFERSLIP_ATTACHMENT.ContentLength);
        //                TempData["TRANSFERSLIP_ATTACHMENT"] = _slipAttachment;

        //                //AttachmentViewModel _multilationAttachment = new AttachmentViewModel();
        //                //_multilationAttachment.FileName = Path.GetFileName(formData.MUTILATION_ATTACHMENT.FileName);
        //                //_multilationAttachment.fileBytes = new byte[formData.MUTILATION_ATTACHMENT.ContentLength];
        //                //formData.MUTILATION_ATTACHMENT.InputStream.Read(_multilationAttachment.fileBytes, 0, formData.MUTILATION_ATTACHMENT.ContentLength);
        //                //TempData["MUTILATION_ATTACHMENT"] = _multilationAttachment;
        //                retVal = 1;
        //            }
        //        }
        //        else
        //        {
        //            if (formData.BACKUP_ATTACHMENT.ContentLength > 0)
        //            {
        //                AttachmentViewModel _backupAttachment = new AttachmentViewModel();
        //                _backupAttachment.FileName = Path.GetFileName(formData.BACKUP_ATTACHMENT.FileName);
        //                _backupAttachment.fileBytes = new byte[formData.BACKUP_ATTACHMENT.ContentLength];
        //                formData.BACKUP_ATTACHMENT.InputStream.Read(_backupAttachment.fileBytes, 0, formData.BACKUP_ATTACHMENT.ContentLength);
        //                TempData["BACKUP_ATTACHMENT"] = _backupAttachment;
        //                retVal = 1;
        //            }
        //            if (formData.TRANSFERSLIP_ATTACHMENT != null)
        //            {
        //                AttachmentViewModel _slipAttachment = new AttachmentViewModel();
        //                _slipAttachment.FileName = Path.GetFileName(formData.TRANSFERSLIP_ATTACHMENT.FileName);
        //                _slipAttachment.fileBytes = new byte[formData.TRANSFERSLIP_ATTACHMENT.ContentLength];
        //                formData.TRANSFERSLIP_ATTACHMENT.InputStream.Read(_slipAttachment.fileBytes, 0, formData.TRANSFERSLIP_ATTACHMENT.ContentLength);
        //                TempData["TRANSFERSLIP_ATTACHMENT"] = _slipAttachment;
        //                retVal = 1;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    //return Json(retVal, JsonRequestBehavior.AllowGet);
        //    return Json(retVal);
        //}

        [HttpPost]
        public IActionResult UploadAttachment(AssetDisposalViewModel formData)
        {
            short retVal = 0;

            try
            {
                if (_sessionService.Get<string>("userID") ==  null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (formData.ASSETTYPEID == 2)
                {
                    if (formData.BACKUP_ATTACHMENT?.Length > 0)
                    {
                        AttachmentViewModel _backupAttachment = new AttachmentViewModel
                        {
                            FileName = Path.GetFileName(formData.BACKUP_ATTACHMENT.FileName),
                            fileBytes = new byte[formData.BACKUP_ATTACHMENT.Length]
                        };
                        formData.BACKUP_ATTACHMENT.OpenReadStream().Read(_backupAttachment.fileBytes, 0, (int)formData.BACKUP_ATTACHMENT.Length);
                        TempData["BACKUP_ATTACHMENT"] = JsonConvert.SerializeObject(_backupAttachment);
                        retVal = 1;
                    }
                    if (formData.TRANSFERSLIP_ATTACHMENT != null)
                    {
                        AttachmentViewModel _slipAttachment = new AttachmentViewModel
                        {
                            FileName = Path.GetFileName(formData.TRANSFERSLIP_ATTACHMENT.FileName),
                            fileBytes = new byte[formData.TRANSFERSLIP_ATTACHMENT.Length]
                        };
                        formData.TRANSFERSLIP_ATTACHMENT.OpenReadStream().Read(_slipAttachment.fileBytes, 0, (int)formData.TRANSFERSLIP_ATTACHMENT.Length);
                        TempData["TRANSFERSLIP_ATTACHMENT"] = JsonConvert.SerializeObject(_slipAttachment);
                        
                        retVal = 1;
                    }
                }
                else
                {
                    if (formData.BACKUP_ATTACHMENT?.Length > 0)
                    {
                        AttachmentViewModel _backupAttachment = new AttachmentViewModel
                        {
                            FileName = Path.GetFileName(formData.BACKUP_ATTACHMENT.FileName),
                            fileBytes = new byte[formData.BACKUP_ATTACHMENT.Length]
                        };
                        formData.BACKUP_ATTACHMENT.OpenReadStream().Read(_backupAttachment.fileBytes, 0, (int)formData.BACKUP_ATTACHMENT.Length);
                        TempData["BACKUP_ATTACHMENT"] = JsonConvert.SerializeObject(_backupAttachment);
                        
                        retVal = 1;
                    }

                    if (formData.TRANSFERSLIP_ATTACHMENT != null)
                    {
                        AttachmentViewModel _slipAttachment = new AttachmentViewModel
                        {
                            FileName = Path.GetFileName(formData.TRANSFERSLIP_ATTACHMENT.FileName),
                            fileBytes = new byte[formData.TRANSFERSLIP_ATTACHMENT.Length]
                        };
                        formData.TRANSFERSLIP_ATTACHMENT.OpenReadStream().Read(_slipAttachment.fileBytes, 0, (int)formData.TRANSFERSLIP_ATTACHMENT.Length);
                        TempData["TRANSFERSLIP_ATTACHMENT"] = JsonConvert.SerializeObject(_slipAttachment);
                        
                        retVal = 1;
                    }
                }
            }
            catch (Exception)
            {
                retVal = -1;
            }
            return Json(retVal);
        }


        [HttpGet]
        public ActionResult EditAssetRequest()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageAssetRequest", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditAssetRequest(long id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                AssetDisposalViewModel ADVM_Model = new AssetDisposalViewModel();
                List<CustomerDetailViewModel> _iList = await GetCustomerDetail();
                ViewBag.Customers = new SelectList(_iList, "CUSTOMERCODE", "CUSTOMERNAME_CODE");
                TempData["CUSTOMER_LIST"] = JsonConvert.SerializeObject(_iList);
                

                ADVM_Model = _AssetService.GetAssetRequestById(id, _Employee_Details);
                //ADVM_Model.SapAssetList = GetAssetDetail();
                ADVM_Model.SapAssetList = await GetAssetDetail();

                ////////// Get authority ////////
                //long? operationMappId = _AssetService.GetOperationMappId(_Employee_Details);
                //string codeByOperationId = _AssetService.GetFnCodeByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_Employee_Details._OpId);
                //short Status_Level = string.IsNullOrEmpty(codeByOperationId)
                //        ? (short)1 : string.IsNullOrEmpty(_AssetService.CheckValidationByFnCode(codeByOperationId, "Maintenance Dept. Approval"))
                //        ? (short)1 : (short)22;
                //ApprovalAuthorityViewModel AAVM = _AssetService.GetApprovalAuthorityBySiteId(Convert.ToInt64(_Employee_Details._SiteId), Status_Level);
                //if (AAVM != null)
                //{
                //    string str = Status_Level == 1 ? "Pending at Taxation Dept." : "Pending at Maintenance Dept.";
                //    ADVM_Model.ForwardedTo = str + " - " + AAVM.EMPNAME;
                //}

                string validationParmValue = _AssetService.GetValidationParmValue("IBM Approval");
                ViewBag.VaildIbmAmt = string.IsNullOrEmpty(validationParmValue) ? 0 : Convert.ToInt64(validationParmValue);

                return View("EditAssetRequest", ADVM_Model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPut]
        public ActionResult EditAssetRequest([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                //string path = Server.MapPath("~/Uploads/AssetDisposal/");
                string basePath = serverpath.getFileUploadPath().Replace(">", "");
                string path = Path.Combine(basePath, "AssetDisposal/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                ADVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Emp_Detail = new Employee_Details()
                {
                    _SiteId = employeeDetails._SiteId
                };
                if (ADVM.ASSETTYPEID == 1)
                {
                    //AttachmentViewModel _BACKUP_ATTACHMENT = (AttachmentViewModel)null;
                    AttachmentViewModel _BACKUP_ATTACHMENT = null;
                    string _backupFileName = string.Empty;
                    if (TempData["BACKUP_ATTACHMENT"] != null)                    
                    {
                        _BACKUP_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["BACKUP_ATTACHMENT"].ToString());
                        _backupFileName = GetFileNameStr(_BACKUP_ATTACHMENT.FileName.Trim());
                        ADVM.BACKUP_ATTACHMENT_NAME = _backupFileName;
                    }
                    //AttachmentViewModel _TRANSFERSLIP_ATTACHMENT = (AttachmentViewModel)null;
                    AttachmentViewModel _TRANSFERSLIP_ATTACHMENT = new AttachmentViewModel();
                    string _transferFileName = string.Empty;
                    if (TempData["TRANSFERSLIP_ATTACHMENT"] != null)
                    {
                        _TRANSFERSLIP_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["TRANSFERSLIP_ATTACHMENT"].ToString());
                        _transferFileName = GetFileNameStr(_TRANSFERSLIP_ATTACHMENT.FileName.Trim());
                        ADVM.TRANSFERSLIP_ATTACHMENT_NAME = _transferFileName;
                    }
                    retVal = _AssetService.SaveAssetRequest(ADVM, employeeDetails);
                    if (retVal == 1)
                    {
                        if (_BACKUP_ATTACHMENT != null)
                        {
                            using (var memStream = new MemoryStream(_BACKUP_ATTACHMENT.fileBytes))
                            {
                                System.IO.File.WriteAllBytes(path + _backupFileName, memStream.ToArray());
                            }
                        }
                        if (_TRANSFERSLIP_ATTACHMENT?.fileBytes != null && _TRANSFERSLIP_ATTACHMENT.fileBytes.Length > 0)
                        {
                            using (var memStream = new MemoryStream(_TRANSFERSLIP_ATTACHMENT.fileBytes))
                            {
                                System.IO.File.WriteAllBytes(path + _transferFileName, memStream.ToArray());
                            }
                        }
                    }
                }
                else
                {
                    AttachmentViewModel _BACKUP_ATTACHMENT = new AttachmentViewModel();
                    string _backupFileName = string.Empty;
                    if (TempData["BACKUP_ATTACHMENT"] != null)
                    {
                        _BACKUP_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["BACKUP_ATTACHMENT"].ToString());                        
                        _backupFileName = GetFileNameStr(_BACKUP_ATTACHMENT.FileName.Trim());
                        ADVM.BACKUP_ATTACHMENT_NAME = _backupFileName;
                    }

                    AttachmentViewModel _TRANSFERSLIP_ATTACHMENT = new AttachmentViewModel();
                    string _transferFileName = string.Empty;
                    if (TempData["TRANSFERSLIP_ATTACHMENT"] != null)
                    {
                        _TRANSFERSLIP_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["TRANSFERSLIP_ATTACHMENT"].ToString());
                        _transferFileName = GetFileNameStr(_TRANSFERSLIP_ATTACHMENT.FileName.Trim());
                        ADVM.TRANSFERSLIP_ATTACHMENT_NAME = _transferFileName;
                    }

                    retVal = _AssetService.SaveAssetRequest(ADVM, employeeDetails);
                    if (retVal == 1)
                    {
                        if (!string.IsNullOrEmpty(ADVM.BACKUP_ATTACHMENT_NAME))
                        {
                            if (_BACKUP_ATTACHMENT?.fileBytes != null)
                            {
                                using (var memStream = new MemoryStream(_BACKUP_ATTACHMENT.fileBytes))
                                {
                                    System.IO.File.WriteAllBytes(path + _backupFileName, memStream.ToArray());
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(ADVM.TRANSFERSLIP_ATTACHMENT_NAME))
                        {
                            if (_TRANSFERSLIP_ATTACHMENT?.fileBytes != null && _TRANSFERSLIP_ATTACHMENT.fileBytes.Length > 0)
                            {
                                using (var memStream = new MemoryStream(_TRANSFERSLIP_ATTACHMENT.fileBytes))
                                {
                                    System.IO.File.WriteAllBytes(path + _transferFileName, memStream.ToArray());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult UploadInvoiceAttachment(IFormFile INVOICE_ATTACHMENT)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (INVOICE_ATTACHMENT?.Length > 0)
                {
                    AttachmentViewModel _InvAttachment = new AttachmentViewModel();
                    _InvAttachment.FileName = Path.GetFileName(INVOICE_ATTACHMENT.FileName);
                    _InvAttachment.fileBytes = new byte[INVOICE_ATTACHMENT.Length];
                    INVOICE_ATTACHMENT.OpenReadStream().Read(_InvAttachment.fileBytes, 0, (int)INVOICE_ATTACHMENT.Length);
                    TempData["INVOICE_ATTACHMENT"] = JsonConvert.SerializeObject(_InvAttachment);
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        //[HttpPost]
        //public ActionResult UploadApprovalAttachment(HttpPostedFileBase APPROVAL_ATTACHMENT)
        //{
        //    short retVal = 0;
        //    try
        //    {
        //        if (_sessionService.Get<string>("userID") == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        if (APPROVAL_ATTACHMENT.ContentLength > 0)
        //        {
        //            AttachmentViewModel _AppAttachment = new AttachmentViewModel();
        //            _AppAttachment.FileName = Path.GetFileName(APPROVAL_ATTACHMENT.FileName);
        //            _AppAttachment.fileBytes = new byte[APPROVAL_ATTACHMENT.ContentLength];
        //            APPROVAL_ATTACHMENT.InputStream.Read(_AppAttachment.fileBytes, 0, APPROVAL_ATTACHMENT.ContentLength);
        //            TempData["APPROVAL_ATTACHMENT"] = _AppAttachment;
        //            retVal = 1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    //return Json(retVal, JsonRequestBehavior.AllowGet);
        //    return Json(retVal);
        //}

        [HttpPost]
        public IActionResult UploadApprovalAttachment(IFormFile APPROVAL_ATTACHMENT)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (APPROVAL_ATTACHMENT?.Length > 0)
                {
                    AttachmentViewModel _AppAttachment = new AttachmentViewModel
                    {
                        FileName = Path.GetFileName(APPROVAL_ATTACHMENT.FileName),
                        fileBytes = new byte[APPROVAL_ATTACHMENT.Length]
                    };

                    APPROVAL_ATTACHMENT.OpenReadStream().Read(_AppAttachment.fileBytes, 0, (int)APPROVAL_ATTACHMENT.Length);
                    TempData["APPROVAL_ATTACHMENT"] = JsonConvert.SerializeObject(_AppAttachment);                    
                    retVal = 1;
                }
            }
            catch (Exception)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult EditPresidentApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageAssetRequest", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditPresidentApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditPresidentApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditPresidentApproval(AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (TempData["APPROVAL_ATTACHMENT"] != null)               
                {
                    Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                    AttachmentViewModel _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["APPROVAL_ATTACHMENT"].ToString());
                    string _AppFileName = GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                    ADVM.Approval_History.ATTACHMENT = _AppFileName;
                    ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    retVal = _AssetService.UploadAssetInvoice(ADVM, _Employee_Details);
                    if (retVal == 1)
                    {
                        SaveApprovalFile(_APPROVAL_ATTACHMENT, _AppFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);

        }

        [HttpPost]
        public ActionResult PrintRequest([FromBody] AssetDisposalViewModel ADVM)      
        {
            string retval = "";
            try
            {
                var finalPrint = ADVM.finalPrint;
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM = _AssetService.GetAssetRequestById(ADVM.DISPOSALHEADERID, _Employee_Details);
                retval = printHtml(ADVM, finalPrint);
            }
            catch (Exception ex)
            {
                retval = "";
            }
            //return Json(retval, JsonRequestBehavior.AllowGet);
            return Json(retval);
        }

        [HttpGet]
        public ActionResult CancelRequest()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageAssetRequest", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }
        [HttpPost]
        public ActionResult CancelRequest(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("CancelRequest", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }
        [HttpPut]
        //public ActionResult CancelRequest([FromBody] AssetDisposalViewModel ADVM, int? cancelledByAdmin) //// cancelledByAdmin - 1
        public ActionResult CancelRequest([FromBody] AssetDisposalViewModel ADVM)
        {
            short resVal = 0;
            try
            {
                var userIdString = _sessionService.Get<string>("userID");
                var cancelledByAdmin = 1;

                if (userIdString == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(userIdString);
                ADVM.Approval_History.APPCODE = Convert.ToInt64(userIdString);
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(userIdString);
                resVal = _AssetService.CancelRequest(ADVM, _Employee_Details, cancelledByAdmin);
            }
            catch (Exception ex)
            {
                resVal = -1;
            }
            //return Json(resVal, JsonRequestBehavior.AllowGet);
            return Json(resVal);
        }
        #endregion

        #region Maintenance Dept. Approval
        public ActionResult ManageMaintenanceDeptApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ManageMaintenanceDeptApproval", _AssetService.GetApprovalListByMaintenanceDept(_Employee_Details));
        }

        [HttpGet]
        public ActionResult EditMaintenanceDeptApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageMaintenanceDeptApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditMaintenanceDeptApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditMaintenanceDeptApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditMaintenanceDeptApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                AttachmentViewModel _APPROVAL_ATTACHMENT = new AttachmentViewModel();
                string _fileName = string.Empty;
                if (this.TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["APPROVAL_ATTACHMENT"].ToString());
                    _fileName = this.GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                    ADVM.Approval_History.ATTACHMENT = _fileName;
                }
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateByMaintenanceDept(ADVM, _Employee_Details);
                if (retVal == 1 && !string.IsNullOrEmpty(_fileName) && TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    SaveApprovalFile(_APPROVAL_ATTACHMENT, _fileName);
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        #endregion

        #region Taxtion Approval
        public ActionResult ManageTaxtionApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ManageTaxtionApproval", _AssetService.GetApprovalListByTaxtion(_Employee_Details));
        }

        [HttpGet]
        public ActionResult EditTaxtionApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageTaxtionApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditTaxtionApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditTaxtionApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditTaxtionApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                AttachmentViewModel _APPROVAL_ATTACHMENT = new AttachmentViewModel();
                string _fileName = string.Empty;
                
                if (this.TempData["APPROVAL_ATTACHMENT"] != null)                
                {
                    _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(this.TempData["APPROVAL_ATTACHMENT"].ToString());
                    _fileName = this.GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                    ADVM.Approval_History.ATTACHMENT = _fileName;
                }
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateByTaxtion(ADVM, _Employee_Details);
                if (retVal == 1 && !string.IsNullOrEmpty(_fileName) && TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    SaveApprovalFile(_APPROVAL_ATTACHMENT, _fileName);
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        public ActionResult GetRequestHistory()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return PartialView("AssetRequestHistory", _AssetService.GetApprovalHistory(Convert.ToInt64(_sessionService.Get<string>("userID"))));
        }

        public ActionResult GetMGFRequestHistory()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return PartialView("AssetRequestMGFHistory", _AssetService.GetApprovalHistory(Convert.ToInt64(_sessionService.Get<string>("userID"))));
        }
        #endregion

        #region User Approval
        public ActionResult ManageUserApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ManageUserApproval", _AssetService.GetApprovalListByUser(_Employee_Details));
        }

        [HttpGet]
        public ActionResult EditUserApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageUserApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditUserApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditUserApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditUserApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateByUserApproval(ADVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult updateUserApproval(string id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // long _ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
            long _ReqId = Convert.ToInt64(id);
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditUserApproval", _AssetService.GetAssetRequestById(_ReqId, _Employee_Details));
        }
        #endregion

        #region IC & IBM Approval
        public ActionResult ManageICApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            //var lst =  _AssetService.GetApprovalListByIC(_Employee_Details);
            //  var sum = lst.Select(itm => itm.DisposalDetailList.Sum(o => o.ORIGINALCOST));
            return View("ManageICApproval", _AssetService.GetApprovalListByIC(_Employee_Details));
        }

        [HttpPost]
        public ActionResult ManageICApproval([FromBody] List<AssetDisposalViewModel> ADVMList)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                if (ADVMList != null)
                {
                    if (ADVMList.Count > 0)
                    {
                        foreach (AssetDisposalViewModel ADVM in ADVMList)
                        {
                            AttachmentViewModel _APPROVAL_ATTACHMENT = new AttachmentViewModel();
                            string _fileName = string.Empty;
                            if (TempData["APPROVAL_ATTACHMENT"] != null)    
                            {
                                _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["APPROVAL_ATTACHMENT"].ToString());
                                _fileName = this.GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                                ADVM.Approval_History.ATTACHMENT = _fileName;
                            }
                            ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                            ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                            ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                            ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                            retVal = _AssetService.RequestUpdateByIC(ADVM, _Employee_Details);
                            if (retVal == 1 && !string.IsNullOrEmpty(_fileName) && TempData["APPROVAL_ATTACHMENT"] != null)
                            {
                                SaveApprovalFile(_APPROVAL_ATTACHMENT, _fileName);
                            }
                        }
                    }
                    else
                    {
                        retVal = -2; //// Please select atleast one request for approval.
                    }
                }
                else
                {
                    retVal = -2; //// Please select atleast one request for approval.
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult EditICApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageICApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditICApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditICApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditICApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                AttachmentViewModel _APPROVAL_ATTACHMENT = new AttachmentViewModel();
                string _fileName = string.Empty;
                if (TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["APPROVAL_ATTACHMENT"].ToString());
                    _fileName = this.GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                    ADVM.Approval_History.ATTACHMENT = _fileName;
                }
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateByIC(ADVM, _Employee_Details);
                if (retVal == 1 && !string.IsNullOrEmpty(_fileName) && TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    SaveApprovalFile(_APPROVAL_ATTACHMENT, _fileName);
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        public ActionResult ManageIBMApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            string validationParmValue = _AssetService.GetValidationParmValue("IBM Approval");
            ViewBag.VaildIbmAmt = string.IsNullOrEmpty(validationParmValue) ? 0 : Convert.ToInt64(validationParmValue);

            return View("ManageIBMApproval", _AssetService.GetApprovalListByIBM(_Employee_Details));
        }

        [HttpPost]
        public ActionResult ManageIBMApproval([FromBody] List<AssetDisposalViewModel> ADVMList)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                if (ADVMList != null)
                {
                    if (ADVMList.Count > 0)
                    {
                        foreach (AssetDisposalViewModel ADVM in ADVMList)
                        {
                            AttachmentViewModel _APPROVAL_ATTACHMENT = new AttachmentViewModel();
                            string _fileName = string.Empty;
                            if (TempData["APPROVAL_ATTACHMENT"] != null)
                            {
                                _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["APPROVAL_ATTACHMENT"].ToString());
                                _fileName = this.GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                                ADVM.Approval_History.ATTACHMENT = _fileName;
                            }
                            ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                            ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                            ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                            ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                            retVal = _AssetService.RequestUpdateByIBM(ADVM, _Employee_Details);
                            if (retVal == 1 && !string.IsNullOrEmpty(_fileName) && TempData["APPROVAL_ATTACHMENT"] != null)
                            {
                                SaveApprovalFile(_APPROVAL_ATTACHMENT, _fileName);
                            }
                        }
                    }
                    else
                    {
                        retVal = -2; //// Please select atleast one request for approval.
                    }
                }
                else
                {
                    retVal = -2; //// Please select atleast one request for approval.
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }


        [HttpGet]
        public ActionResult EditIBMApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageIBMApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditIBMApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            string validationParmValue = _AssetService.GetValidationParmValue("IBM Approval");
            ViewBag.VaildIbmAmt = string.IsNullOrEmpty(validationParmValue) ? 0 : Convert.ToInt64(validationParmValue);

            return View("EditIBMApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditIBMApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                AttachmentViewModel _APPROVAL_ATTACHMENT = new AttachmentViewModel();
                string _fileName = string.Empty;
                if (TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["APPROVAL_ATTACHMENT"].ToString());
                    _fileName = this.GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                    ADVM.Approval_History.ATTACHMENT = _fileName;
                }
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateByIBM(ADVM, _Employee_Details);
                if (retVal == 1 && !string.IsNullOrEmpty(_fileName) && TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    SaveApprovalFile(_APPROVAL_ATTACHMENT, _fileName);
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }
        #endregion

        //Added by TTL :: SR95154 | CR6236
        #region Plant PPC Approval
        [HttpGet]
        public ActionResult ManagePlantPPCApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View(_AssetService.GetListForPlantPPCApproval(_Employee_Details));
        }

        [ActionName("PlantPPCApproval")]
        [HttpGet]
        public ActionResult EditPlantPPCApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManagePlantPPCApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [ActionName("PlantPPCApproval")]
        [HttpPost]
        public ActionResult EditPlantPPCApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            //List<SIS_ASSETCONDITION> conditionList = _AssetService.GetConditionList();
            //ViewBag.AssetCondition = new SelectList(conditionList, "CONDITIONID", "DESCRIPTION");
            return View("EditPlantPPCApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [ActionName("PlantPPCApproval")]
        [HttpPut]
        public ActionResult EditPlantPPCApproval([FromBody]  AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                retVal = _AssetService.RequestUpdateByPlantPPC(ADVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        #endregion
        // End by TTL :: SR95154 | CR6236
        #region Plan Mgf. & Quality Approval

        [HttpGet]
        public ActionResult ManagePlantApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var employeeDetails = HttpContext.Session.GetObject<Employee_Details>("Employee");

            return View(_AssetService.GetListForPlantApproval(employeeDetails));
            //return View(_AssetService.GetListForPlantApproval((Employee_Details)this.Session["Employee"]));
        }

        [ActionName("PlanApproval")]
        [HttpGet]
        public ActionResult EditPlantApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManagePlantApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [ActionName("PlanApproval")]
        [HttpPost]
        public ActionResult EditPlantApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //Employee_Details _Employee_Details = (Employee_Details)this.Session["Employee"];
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            List<SIS_ASSETCONDITION> conditionList = _AssetService.GetConditionList();
            ViewBag.AssetCondition = new SelectList(conditionList, "CONDITIONID", "DESCRIPTION");
            return View("EditPlantApproval", _AssetService.GetAssetRequestById(id, employeeDetails));
        }

        [ActionName("PlanApproval")]
        [HttpPut]
        public ActionResult EditPlantApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //Employee_Details _Employee_Details = (Employee_Details)this.Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(this._sessionService.Get<string>("userID").ToString());
                AttachmentViewModel _APPROVAL_ATTACHMENT = new AttachmentViewModel();
                string _fileName = string.Empty;
                if (this.TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(this.TempData["APPROVAL_ATTACHMENT"].ToString());
                    _fileName = this.GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                    ADVM.INVOICE_ATTACHMENT_NAME = _fileName;
                }
                retVal = _AssetService.RequestUpdateByPlantApproval(ADVM, _Employee_Details);
                if (retVal == 1 && !string.IsNullOrEmpty(_fileName) && TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    SaveApprovalFile(_APPROVAL_ATTACHMENT, _fileName);
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult ManagePlantMgfApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ManagePlantMgfApproval", _AssetService.GetApprovalListByPlanMgf(_Employee_Details));
        }

        [ActionName("PlanMgfApproval")]
        [HttpGet]
        public ActionResult EditMgfApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManagePlantMgfApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [ActionName("PlanMgfApproval")]
        [HttpPost]
        public ActionResult EditMgfApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            List<SIS_ASSETCONDITION> conditionList = _AssetService.GetConditionList();
            ViewBag.AssetCondition = new SelectList(conditionList, "CONDITIONID", "DESCRIPTION");
            return View("EditMgfApproval", _AssetService.GetAssetRequestByMgf(id, _Employee_Details));
        }

        [ActionName("PlanMgfApproval")]
        [HttpPut]
        public ActionResult EditMgfApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                AttachmentViewModel _APPROVAL_ATTACHMENT = new AttachmentViewModel();
                string _fileName = string.Empty;
                if (this.TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(this.TempData["APPROVAL_ATTACHMENT"].ToString());
                    _fileName = this.GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                    ADVM.Approval_History.ATTACHMENT = _fileName;
                }
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateByMgf(ADVM, _Employee_Details);
                if (retVal == 1 && !string.IsNullOrEmpty(_fileName) && TempData["APPROVAL_ATTACHMENT"] != null)
                {
                    SaveApprovalFile(_APPROVAL_ATTACHMENT, _fileName);
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        [HttpPost]
        public ActionResult UpdateReqApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (this._sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //Employee_Details _Employee_Details = (Employee_Details)this.Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.DATELSTMOD = DateTime.Now;
                retVal = _AssetService.UpdatePlantApprovalRequiredByMgf(ADVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult ManagePlantQualityApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ManagePlantQualityApproval", _AssetService.GetApprovalListByPlanQty(_Employee_Details));
        }

        [ActionName("PlanQualityApproval")]
        [HttpGet]
        public ActionResult EditQualityApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManagePlantQualityApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [ActionName("PlanQualityApproval")]
        [HttpPost]
        public ActionResult EditQualityApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditQualityApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [ActionName("PlanQualityApproval")]
        [HttpPut]
        public ActionResult EditQualityApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateByQty(ADVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);

        }
        #endregion

        #region Environment / Admin Approval
        [HttpGet]
        public ActionResult ManageEnvironmentApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ManageEnvironmentApproval", _AssetService.GetApprovalListByEnvironment(_Employee_Details));
        }

        [HttpGet]
        public ActionResult EditEnvironmentApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageEnvironmentApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditEnvironmentApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditEnvironmentApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditEnvironmentApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (ADVM.Approval_History == null)
                {
                    ADVM.Approval_History = new ApprovalHisViewModel();
                }

                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateByEnvironment(ADVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult ManageAdminApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ManageAdminApproval", _AssetService.GetApprovalListByAdmin(_Employee_Details));
        }

        [HttpGet]
        public ActionResult EditAdminApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageAdminApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditAdminApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditAdminApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditAdminApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateByAdmin(ADVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }
        #endregion

        #region Security Approval
        public ActionResult ManageSecurityApproval()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ManageSecurityApproval", _AssetService.GetApprovalListBySecurity(_Employee_Details));
        }

        [HttpGet]
        public ActionResult EditSecurityApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageSecurityApproval", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditSecurityApproval(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditSecurityApproval", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditSecurityApproval([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateBySecurity(ADVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }
        #endregion

        #region Asset Retirement
        public ActionResult ManageAssetRetirement()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("ManageAssetRetirement", _AssetService.GetApprovalListByAssetRetirement(_Employee_Details));
        }

        [HttpGet]
        public ActionResult EditAssetRetirement()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageAssetRetirement", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult EditAssetRetirement(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("EditAssetRetirement", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult EditAssetRetirement([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.RequestUpdateBySecurity(ADVM, _Employee_Details);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }
        #endregion

        #region Upload Invoice
        public ActionResult ManageInvoiceByUser()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int Status_Level = 15;
            return View("UploadInvoice/ManageInvoiceByUser", _AssetService.GetApprovalListForUploadInvoice(Status_Level, Convert.ToInt64(_sessionService.Get<string>("userID").ToString())));
        }

        [HttpGet]
        public ActionResult UploadInvoiceByUser()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageInvoiceByUser", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult UploadInvoiceByUser(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("UploadInvoice/UploadInvoiceByUser", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult UploadInvoiceByUser([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (TempData["INVOICE_ATTACHMENT"] != null)
                {
                    Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                    AttachmentViewModel _INVOICE_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["INVOICE_ATTACHMENT"].ToString());                   

                    string _uInvoiceFileName = GetFileNameStr(_INVOICE_ATTACHMENT.FileName.Trim());
                    ADVM.INVOICE_ATTACHMENT = _uInvoiceFileName;
                    ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    retVal = _AssetService.UploadAssetInvoice(ADVM, _Employee_Details);
                    if (retVal == 1)
                    {
                        SaveInvoiceFile(_INVOICE_ATTACHMENT, _uInvoiceFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        public ActionResult ManageInvoiceByAdmin()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int Status_Level = 17;
            return View("UploadInvoice/ManageInvoiceByAdmin", _AssetService.GetApprovalListForUploadInvoice(Status_Level, Convert.ToInt64(_sessionService.Get<string>("userID").ToString())));
        }

        [HttpGet]
        public ActionResult UploadInvoiceByAdmin()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageInvoiceByAdmin", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public ActionResult UploadInvoiceByAdmin(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            return View("UploadInvoice/UploadInvoiceByAdmin", _AssetService.GetAssetRequestById(id, _Employee_Details));
        }

        [HttpPut]
        public ActionResult UploadInvoiceByAdmin([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (TempData["INVOICE_ATTACHMENT"] != null)                
                {
                    Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                    AttachmentViewModel _INVOICE_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["INVOICE_ATTACHMENT"].ToString());
                    string _aInvoiceFileName = GetFileNameStr(_INVOICE_ATTACHMENT.FileName.Trim());
                    ADVM.INVOICE_ATTACHMENT = _aInvoiceFileName;
                    ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    retVal = _AssetService.UploadAssetInvoice(ADVM, _Employee_Details);
                    if (retVal == 1)
                    {
                        SaveInvoiceFile(_INVOICE_ATTACHMENT, _aInvoiceFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        public ActionResult ManageInvoiceByEnvironment()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int Status_Level = 16;
            return View("UploadInvoice/ManageInvoiceByEnvironment", _AssetService.GetApprovalListForUploadInvoice(Status_Level, Convert.ToInt64(_sessionService.Get<string>("userID").ToString())));
        }

        [HttpGet]
        public ActionResult UploadInvoiceByEnvironment()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageInvoiceByEnvironment", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadInvoiceByEnvironment(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            List<CustomerDetailViewModel> _iList = await GetCustomerDetail();
            ViewBag.Customers = new SelectList(_iList, "CUSTOMERCODE", "CUSTOMERNAME_CODE");
            TempData["CUSTOMER_LIST"] = JsonConvert.SerializeObject(_iList);            
            AssetDisposalViewModel ADVM = _AssetService.GetAssetRequestById(id, _Employee_Details);
            if (ADVM.CustomerDetail == null)
            {
                ADVM.CustomerDetail = new DisposalCustomerViewModel();
            }
            return View("UploadInvoice/UploadInvoiceByEnvironment", ADVM);
        }
        [HttpPut]
        public ActionResult UploadInvoiceByEnvironment([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (TempData["INVOICE_ATTACHMENT"] != null)
                {
                    Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                    AttachmentViewModel _INVOICE_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["INVOICE_ATTACHMENT"].ToString());
                    string _eInvoiceFileName = GetFileNameStr(_INVOICE_ATTACHMENT.FileName.Trim());
                    ADVM.INVOICE_ATTACHMENT = _eInvoiceFileName;
                    ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());

                    //// --- Approval Attachment
                    AttachmentViewModel _APPROVAL_ATTACHMENT = new AttachmentViewModel();
                    string _fileName = string.Empty;
                    if (this.TempData["APPROVAL_ATTACHMENT"] != null)
                    {
                        _APPROVAL_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(this.TempData["APPROVAL_ATTACHMENT"].ToString());
                        _fileName = this.GetFileNameStr(_APPROVAL_ATTACHMENT.FileName.Trim());
                        ADVM.Approval_History.ATTACHMENT = _fileName;
                    }

                    retVal = _AssetService.UploadAssetInvoice(ADVM, _Employee_Details);
                    if (retVal == 1)
                    {
                        SaveInvoiceFile(_INVOICE_ATTACHMENT, _eInvoiceFileName);
                    }

                    //// --- Save Approval Attachment
                    if (retVal == 1 && !string.IsNullOrEmpty(_fileName) && TempData["APPROVAL_ATTACHMENT"] != null)
                    {
                        SaveApprovalFile(_APPROVAL_ATTACHMENT, _fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        public ActionResult ManageInvoiceByTaxation()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int Status_Level = 25;
            return View("UploadInvoice/ManageInvoiceByTaxation", _AssetService.GetApprovalListForUploadInvoice(Status_Level, Convert.ToInt64(_sessionService.Get<string>("userID").ToString())));
        }

        [HttpGet]
        public ActionResult UploadInvoiceByTaxation()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return RedirectToAction("ManageInvoiceByTaxation", "AssetDisposal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "AppError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadInvoiceByTaxation(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            List<CustomerDetailViewModel> _iList = await GetCustomerDetail();
            ViewBag.Customers = new SelectList(_iList, "CUSTOMERCODE", "CUSTOMERNAME_CODE");
            TempData["CUSTOMER_LIST"] = JsonConvert.SerializeObject(_iList);
            AssetDisposalViewModel ADVM = _AssetService.GetAssetRequestById(id, _Employee_Details);
            if (ADVM.CustomerDetail == null)
            {
                ADVM.CustomerDetail = new DisposalCustomerViewModel();
            }
            return View("UploadInvoice/UploadInvoiceByTaxation", ADVM);
        }

        [HttpPut]
        public ActionResult UploadInvoiceByTaxation([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (TempData["INVOICE_ATTACHMENT"] != null)
                {
                    Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                    AttachmentViewModel _INVOICE_ATTACHMENT = JsonConvert.DeserializeObject<AttachmentViewModel>(TempData["INVOICE_ATTACHMENT"].ToString());
                    
                    string _eInvoiceFileName = GetFileNameStr(_INVOICE_ATTACHMENT.FileName.Trim());
                    ADVM.INVOICE_ATTACHMENT = _eInvoiceFileName;
                    ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.APPCODE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    ADVM.Approval_History.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                    retVal = _AssetService.UploadAssetInvoice(ADVM, _Employee_Details);
                    if (retVal == 1)
                    {
                        SaveInvoiceFile(_INVOICE_ATTACHMENT, _eInvoiceFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }
        #endregion

        #region Master Page
        #region Approval Authority Master
        [HttpGet]
        public ActionResult ManageApprovalAuthority()
        {
            return View("Master/ManageApprovalAuthority", _AssetService.GetApprovalAuthorityList());
        }

        [HttpGet]
        public ActionResult ManageApprovalAuthorityByFinance()
        {
            return View("Master/ManageApprovalAuthorityByFinance", _AssetService.ManageApprovalAuthorityByFinance());
        }

        [HttpGet]
        public ActionResult ManageApprovalAuthorityByPPC()
        {
            return View("Master/ManageApprovalAuthorityByPPC", _AssetService.ManageApprovalAuthorityByPPC());
        }

        [HttpGet]
        public ActionResult SetApprovalAuthority(short category)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Category = category;
            List<SYSITE> _SiteList = _AssetService.GetSiteList().OrderBy(x => x.DESCRIP).ToList(); //Added by TTL :: CR6754 (Make it order by)
            ViewBag.SiteList = new SelectList(_SiteList, "SYSITEID", "DESCRIP");
            List<ADORGLEVEL> _OrgList = _AssetService.GetOrgLevelList().OrderBy(x => x.LEVELDESCRIP).ToList(); //Added by TTL :: CR6754 (Make it order by)
            ViewBag.OrgLevelList = new SelectList(_OrgList, "ADORGLEVELID", "LEVELDESCRIP");
            List<SIS_AST_PROCESSSTATUS_MST> _ProcessStatusList = _AssetService.GetProcessStatusList().OrderBy(x => x.STATUSDESCRIPTION).ToList(); //Added by TTL :: CR6754 (Make it order by)
            ViewBag.ProcessStatusList = new SelectList(_ProcessStatusList.Where(p => p.CATEGORY == category).ToList(), "SIS_AST_PROCESSSTATUS_MSTID", "STATUSDESCRIPTION");
            return PartialView("Master/_SetApprovalAuthority");
        }

        [HttpGet]
        public ActionResult EditApprovalAuthority(long id, short category)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Category = category;
            List<SYSITE> _SiteList = _AssetService.GetSiteList().OrderBy(x => x.DESCRIP).ToList(); //Added by TTL :: CR6754 (Make it order by)
            ViewBag.SiteList = new SelectList(_SiteList, "SYSITEID", "DESCRIP");
            List<ADORGLEVEL> _OrgList = _AssetService.GetOrgLevelList().OrderBy(x => x.LEVELDESCRIP).ToList(); //Added by TTL :: CR6754 (Make it order by)
            ViewBag.OrgLevelList = new SelectList(_OrgList, "ADORGLEVELID", "LEVELDESCRIP");
            List<SIS_AST_PROCESSSTATUS_MST> _ProcessStatusList = _AssetService.GetProcessStatusList().OrderBy(x => x.STATUSDESCRIPTION).ToList(); //Added by TTL :: CR6754 (Make it order by)
            ViewBag.ProcessStatusList = new SelectList(_ProcessStatusList.Where(p => p.CATEGORY == category).ToList(), "SIS_AST_PROCESSSTATUS_MSTID", "STATUSDESCRIPTION");
            return PartialView("Master/_SetApprovalAuthority", _AssetService.GetApprovalAuthorityById(id));
        }

        [HttpPost]
        public ActionResult SaveApprovalAuthority([FromBody] ApprovalAuthorityViewModel AAVM)
        {
            String transactionType = "OPERATION";
            if (AAVM.APPAUTHID == 0)
            {
                transactionType = "INSERT";
            }
            else if (AAVM.APPAUTHID > 0)
            {
                transactionType = "UPDATE";
            }
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AAVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                AAVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.SaveApprovalAuthority(AAVM, transactionType);
            }
            catch (Exception ex)
            {
                retVal = -1;
                throw (ex);
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }
        #endregion

        #region Operation Mapping Master
        [HttpGet]
        public ActionResult ManageOperationMapping()
        {
            return View("Master/ManageOperationMapping", _AssetService.GetOperationMappingList());
        }

        [HttpGet]
        public ActionResult CreateOperationMapping()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<ADORGLEVEL> _OrgList = _AssetService.GetOrgLevelList();
            ViewBag.OrgLevelList = new SelectList(_OrgList, "ADORGLEVELID", "LEVELDESCRIP");
            return PartialView("Master/_OperationMapping");
        }

        [HttpGet]
        public ActionResult EditOperationMapping(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<ADORGLEVEL> _OrgList = _AssetService.GetOrgLevelList();
            ViewBag.OrgLevelList = new SelectList(_OrgList, "ADORGLEVELID", "LEVELDESCRIP");
            return PartialView("Master/_OperationMapping", _AssetService.GetOperationMappingById(id));
        }

        [HttpPost]
        public ActionResult SaveOperationMapping([FromBody] OPMappingViewModel OMVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                OMVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                OMVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.SaveOperationMapping(OMVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
                throw (ex);
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }
        #endregion

        #region Operation Master
        [HttpGet]
        public ActionResult ManageOperation()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View("Master/ManageOperation", _AssetService.GetOperationList());
        }

        [HttpGet]
        public ActionResult CreateOperation()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<ADORGLEVEL> _OrgList = _AssetService.GetOrgLevelList();
            ViewBag.OrgLevelList = new SelectList(_OrgList, "ADORGLEVELID", "LEVELDESCRIP");
            return PartialView("Master/_Operation");
        }

        [HttpGet]
        public ActionResult EditOperation(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<ADORGLEVEL> _OrgList = _AssetService.GetOrgLevelList();
            ViewBag.OrgLevelList = new SelectList(_OrgList, "ADORGLEVELID", "LEVELDESCRIP");
            return PartialView("Master/_Operation", _AssetService.GetOperationDtlById(id));
        }

        [HttpPost]
        public ActionResult SaveOperation([FromBody] AssetOperationViewModel AOVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AOVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                AOVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.SaveOperation(AOVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
                throw (ex);
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        #endregion

        #region Validation Master
        [HttpGet]
        public ActionResult ManageValidation()
        {
            return View("Master/ManageValidation", _AssetService.GetValidationList());
        }

        [HttpGet]
        public ActionResult CreateValidation()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<SIS_AST_PARAM_MST> _ParamList = _AssetService.GetParamList();
            ViewBag.ParamList = new SelectList(_ParamList, "SIS_AST_PARAM_MSTID", "PARAMNAME");
            List<SYSITE> siteList = _AssetService.GetSiteList();
            ViewBag.SYSITE = new SelectList(siteList, "SYSITEID", "DESCRIP");
            return PartialView("Master/_ValidationMaster");
        }

        [HttpGet]
        public ActionResult EditValidation(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<SIS_AST_PARAM_MST> _ParamList = _AssetService.GetParamList();
            ViewBag.ParamList = new SelectList(_ParamList, "SIS_AST_PARAM_MSTID", "PARAMNAME");
            List<SYSITE> siteList = _AssetService.GetSiteList();
            ViewBag.SYSITE = new SelectList(siteList, "SYSITEID", "DESCRIP");
            return PartialView("Master/_ValidationMaster", _AssetService.GetValidationById(id));
        }

        [HttpPost]
        public ActionResult SaveValidation([FromBody] ValidationViewModel VVM)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                VVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                VVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _AssetService.SaveValidation(VVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
                throw (ex);
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult GetParmDataById(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SIS_AST_PARAM_MST sisAstParamMst = _AssetService.GetParamList().Where(x => x.SIS_AST_PARAM_MSTID == id).FirstOrDefault();
            return (ActionResult)this.Json((object)new
            {
                PARAMID = sisAstParamMst.SIS_AST_PARAM_MSTID,
                DESCRIPTION = sisAstParamMst.PARAMDESCRIPTION,
                TYPE = sisAstParamMst.PARAMTYPE,
                VALUETYPE = sisAstParamMst.PARAMVALUETYPE,
                DISPLAYTEXT = sisAstParamMst.DISPLAY_LABEL == null ? "" : sisAstParamMst.DISPLAY_LABEL
                // }, JsonRequestBehavior.AllowGet);
            });
        }
        #endregion

        #region Request Cancellation
        public ActionResult ManageRequestCancellation()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View("Master/ManageRequestCancellation", _AssetService.GetRequestCancellationListForAdmin());
        }

        [HttpPost]
        public ActionResult UpdateIBMDate([FromBody] AssetDisposalViewModel ADVM)
        {
            short resVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ADVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                resVal = _AssetService.UpdateIBMDate(ADVM);
            }
            catch (Exception ex)
            {
                resVal = -1;
            }
            //return Json(resVal, JsonRequestBehavior.AllowGet);
            return Json(resVal);
        }
        #endregion

        #endregion

        #region Common Method
        public string GetFileNameStr(string ATTACHMENT_NAME)
        {
            return Path.Combine(Path.GetDirectoryName(ATTACHMENT_NAME),
                          string.Concat(Path.GetFileNameWithoutExtension(ATTACHMENT_NAME),
                          DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss"),
                          Path.GetExtension(ATTACHMENT_NAME)));
        }

        public void SaveInvoiceFile(AttachmentViewModel _INVOICE_ATTACHMENT, string _fileName)
        {
            //string path = Server.MapPath("~/Uploads/AssetDisposal/Invoice/");

            string basePath = serverpath.getFileUploadPath();
            string path = Path.Combine(basePath, "AssetDisposal", "Invoice");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (_INVOICE_ATTACHMENT.fileBytes.Length > 0)
            {
                using (var memStream = new MemoryStream(_INVOICE_ATTACHMENT.fileBytes))
                {
                    System.IO.File.WriteAllBytes(path + _fileName, memStream.ToArray());
                }
            }
        }

        public void SaveApprovalFile(AttachmentViewModel _APPROVAL_ATTACHMENT, string _fileName)
        {
            //string path = Server.MapPath("~/Uploads/AssetDisposal/Approval/");
            string basePath = serverpath.getFileUploadPath();
            string path = Path.Combine(basePath, "AssetDisposal", "Approval");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (_APPROVAL_ATTACHMENT.fileBytes.Length > 0)
            {
                using (var memStream = new MemoryStream(_APPROVAL_ATTACHMENT.fileBytes))
                {
                    System.IO.File.WriteAllBytes(path + _fileName, memStream.ToArray());
                }
            }
        }

        public ActionResult DownloadApprovalAttachment(string file_name)
        {
            try
            {
                //string file_path = Server.MapPath("~/Uploads/AssetDisposal/Approval/");
                string basePath = serverpath.getFileUploadPath();
                string file_path = Path.Combine(basePath, "AssetDisposal", "Approval");
                byte[] fileBytes = System.IO.File.ReadAllBytes(file_path + file_name);
                if (fileBytes.Length > 0)
                {
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file_name);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public ActionResult DownloadAttachment(string file_name)
        {
            try
            {
                //string file_path = Server.MapPath("~/Uploads/AssetDisposal/");

                string basePath = serverpath.getFileUploadPath();
                string file_path = Path.Combine(basePath, "AssetDisposal/");

                byte[] fileBytes = System.IO.File.ReadAllBytes(file_path + file_name);
                if (fileBytes.Length > 0)
                {
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file_name);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public ActionResult DownloadInvoice(string file_name)
        {
            try
            {
                //string file_path = Server.MapPath("~/Uploads/AssetDisposal/Invoice/");
                string basePath = serverpath.getFileUploadPath();
                string file_path = Path.Combine(basePath, "AssetDisposal", "Invoice");
                byte[] fileBytes = System.IO.File.ReadAllBytes(file_path + file_name);
                if (fileBytes.Length > 0)
                {
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file_name);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string printHtml(AssetDisposalViewModel ADVM, int? finalPrint)
        {
            //Employee_Details employeeDetails = (Employee_Details)this.Session["Employee"];
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            string retval = "";
            if (ADVM != null)
            {
                string _SiteName = this._AssetService.GetSiteDetailBySiteId(ADVM.Emp_Detail._SiteId.Value).DESCRIP.ToString();
                string _customerDtl = "";
                if (ADVM.CustomerDetail != null)
                {
                    _customerDtl = ADVM.CustomerDetail.CUSTOMERNAME + ", " + ADVM.CustomerDetail.CUSTADDRESS + " - " + ADVM.CustomerDetail.CUSTGSTIN;
                }
                Decimal _basicValue = ADVM.CustomerDetail == null ? new Decimal(0, 0, 0, false, (byte)2) : ADVM.CustomerDetail.BASICVALUE;
                string _gstRate = ADVM.CustomerDetail == null ? "" : ADVM.CustomerDetail.GSTRATE;
                string _gstIN = ADVM.CustomerDetail == null ? "" : ADVM.CustomerDetail.CUSTGSTIN;
                string _paymentDt = ADVM.CustomerDetail == null ? "" : ADVM.CustomerDetail.PAYMENTDETAILS;
                StringBuilder stringBuilder1 = new StringBuilder();
                stringBuilder1.Append("<h3 style='text-align:center;'>Asset Sale & Disposal Approval</h3>");
                stringBuilder1.Append("<table style='width:100%;font-size: 7pt;'>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th style='width:10%;text-align:right;'>Operation : </th><td style='width:20%;text-align:left;'>" + (string.IsNullOrEmpty(ADVM.Emp_Detail._OpDesc) ? "N/A" : ADVM.Emp_Detail._OpDesc) + "</td>");
                stringBuilder1.Append("<th style='width:10%;text-align:right;'>Division : </th><td style='width:20%;text-align:left;'>" + (string.IsNullOrEmpty(ADVM.Emp_Detail._DivDesc) ? "N/A" : ADVM.Emp_Detail._DivDesc) + "</td>");
                stringBuilder1.Append("<th style='width:10%;text-align:right;'>Department : </th><td style='width:20%;text-align:left;'>" + (string.IsNullOrEmpty(ADVM.Emp_Detail._DepDesc) ? "N/A" : ADVM.Emp_Detail._DepDesc) + "</td>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th style='width:10%;text-align:right;'>Section : </th><td style='width:20%;'>" + (string.IsNullOrEmpty(ADVM.Emp_Detail._SecDescrip) ? "N/A" : ADVM.Emp_Detail._SecDescrip) + "</td>");
                stringBuilder1.Append("<th style='width:10%;text-align:right;'>Location : </th><td style='width:20%;'>" + _SiteName + "</td>");
                stringBuilder1.Append("<th style='width:10%;text-align:right;'>Asset Type : </th><td style='width:20%;'>" + ADVM.ASSETTYPE + "</td>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("</table>");
                stringBuilder1.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 5pt;font-family:Arial'>");
                stringBuilder1.Append("<tr><th colspan='11' style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>Asset Information (Table A) - To be filled by user</th></tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>SAP Asset Code</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>Asset Description</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>Vender Name</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>InVoice No. & Date</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>Bill Of Entry & License No./Date if purchased under EPCG</th>");
                stringBuilder1.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Original Cost(P)</th>");
                stringBuilder1.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Depreciation (Q)</th>");
                stringBuilder1.Append("<th style='width:6%;text-align:center;border: 1px solid;'>WDV (R=P-Q)</th>");
                stringBuilder1.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Sale Price ('S)</th>");
                stringBuilder1.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Profit/Loss (S-R)</th>");
                stringBuilder1.Append("</tr>");
                int num2 = 1;
                Decimal num3 = new Decimal(0, 0, 0, false, (byte)2);
                Decimal num4 = new Decimal(0, 0, 0, false, (byte)2);
                Decimal num5 = new Decimal(0, 0, 0, false, (byte)2);
                Decimal num6 = new Decimal(0, 0, 0, false, (byte)2);
                Decimal num7 = new Decimal(0, 0, 0, false, (byte)2);
                foreach (DisposalDetailViewModel disposalDetail in (IEnumerable<DisposalDetailViewModel>)ADVM.DisposalDetailList)
                {
                    Decimal num8 = num3;
                    Decimal? nullable = disposalDetail.ORIGINALCOST;
                    Decimal num9 = Convert.ToDecimal((object)(!nullable.HasValue ? new Decimal?(new Decimal(0, 0, 0, false, (byte)2)) : disposalDetail.ORIGINALCOST));
                    num3 = num8 + num9;
                    Decimal num10 = num4;
                    nullable = disposalDetail.DEPRECIATIONCOST;
                    Decimal num11 = Convert.ToDecimal((object)(!nullable.HasValue ? new Decimal?(new Decimal(0, 0, 0, false, (byte)2)) : disposalDetail.DEPRECIATIONCOST));
                    num4 = num10 + num11;
                    Decimal num12 = num5;
                    nullable = disposalDetail.WDV;
                    Decimal num13 = Convert.ToDecimal((object)(!nullable.HasValue ? new Decimal?(new Decimal(0, 0, 0, false, (byte)2)) : disposalDetail.WDV));
                    num5 = num12 + num13;
                    Decimal num14 = num6;
                    nullable = disposalDetail.SALESPRICE;
                    Decimal num15 = Convert.ToDecimal((object)(!nullable.HasValue ? new Decimal?(new Decimal(0, 0, 0, false, (byte)2)) : disposalDetail.SALESPRICE));
                    num6 = num14 + num15;
                    Decimal num16 = num7;
                    nullable = disposalDetail.PROFIT_LOSS;
                    Decimal num17 = Convert.ToDecimal((object)(!nullable.HasValue ? new Decimal?(new Decimal(0, 0, 0, false, (byte)2)) : disposalDetail.PROFIT_LOSS));
                    num7 = num16 + num17;
                    string str8 = disposalDetail.BILLENTRY == "N/A" ? "" : disposalDetail.BILLENTRY + " / ";
                    string str9 = disposalDetail.LICENSENO == "N/A" ? "" : disposalDetail.LICENSENO;
                    DateTime dateTime = Convert.ToDateTime((object)disposalDetail.PURCHASEDDATE);
                    string str10;
                    if (!(dateTime.ToString("dd-MMM-yyyy") == "01-Jan-0001"))
                    {
                        dateTime = Convert.ToDateTime((object)disposalDetail.PURCHASEDDATE);
                        str10 = " Dt: " + dateTime.ToString("dd-MMM-yyyy");
                    }
                    else
                        str10 = "";
                    string str11 = str10;
                    stringBuilder1.Append("<tr>");
                    stringBuilder1.Append("<td style='text-align:center;border: 1px solid;'>" + (object)num2++ + "</td>");
                    stringBuilder1.Append("<td style='text-align:center;border: 1px solid;'>" + disposalDetail.ASSETCODE + "</td>");
                    stringBuilder1.Append("<td style='border:1px solid;'>" + disposalDetail.ASSETDESCRIPTION + "</td>");
                    stringBuilder1.Append("<td style='border:1px solid;'>" + disposalDetail.VENDORNAME + "</td>");
                    StringBuilder stringBuilder2 = stringBuilder1;
                    string[] strArray = new string[5]
                    {
            "<td style='border:1px solid;'>",
            disposalDetail.INVOICENO,
            " Dt: ",
            null,
            null
                    };
                    dateTime = Convert.ToDateTime((object)disposalDetail.INVOICEDATE);
                    strArray[3] = dateTime.ToString("dd-MMM-yyyy");
                    strArray[4] = "</td>";
                    string str12 = string.Concat(strArray);
                    stringBuilder2.Append(str12);
                    stringBuilder1.Append("<td style='text-align:center;border:1px solid;'>" + str8 + str9 + str11 + "</td>");
                    stringBuilder1.Append("<td style='text-align:right;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)disposalDetail.ORIGINALCOST) + "</td>");
                    stringBuilder1.Append("<td style='text-align:right;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)disposalDetail.DEPRECIATIONCOST) + "</td>");
                    stringBuilder1.Append("<td style='text-align:right;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)disposalDetail.WDV) + "</td>");
                    stringBuilder1.Append("<td style='text-align:right;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)disposalDetail.SALESPRICE) + "</td>");
                    stringBuilder1.Append("<td style='text-align:right;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)disposalDetail.PROFIT_LOSS) + "</td>");
                    stringBuilder1.Append("</tr>");
                }
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<td colspan='6' style='text-align:right;background-color: #ccc;border: 1px solid;'>Total</td>");
                stringBuilder1.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)num3) + "</td>");
                stringBuilder1.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)num4) + "</td>");
                stringBuilder1.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)num5) + "</td>");
                stringBuilder1.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)num6) + "</td>");
                stringBuilder1.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + string.Format("{0:0,0.00}", (object)num7) + "</td>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th colspan='6' style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>In Case of Sale - Table B</th>");
                stringBuilder1.Append("<th colspan='5' style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>In Case of Insurance Lost - Table C</th>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th colspan='3' style='text-align:center;border: 1px solid;'>Customer Name, Address & GSTIN</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>Basic Sales Value</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>GST Rate With HSN</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>Payment Detail</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>Claim Lodged (Yes/No)</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>Date Of Claim Lodged</th>");
                stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>Claim Amount (Rs)</th>");
                stringBuilder1.Append("<th colspan='2' style='text-align:center;border: 1px solid;'>Reason For No Claim / Remarks</th>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<td colspan='3' style='text-align:left;border: 1px solid;'>" + _customerDtl + "</td>");
                stringBuilder1.Append("<td style='border:1px solid;text-align:right;'>" + string.Format("{0:0,0.00}", _basicValue) + "</td>");
                stringBuilder1.Append("<td style='border:1px solid;text-align:center;'>" + _gstRate + "</td>");
                stringBuilder1.Append("<td style='border:1px solid;'>" + _paymentDt + "</td>");
                stringBuilder1.Append("<td style='text-align:center;border:1px solid;'></td>");
                stringBuilder1.Append("<td style='text-align:right;border:1px solid;'></td>");
                stringBuilder1.Append("<td style='text-align:right;border:1px solid;'></td>");
                stringBuilder1.Append("<td colspan='2' style='text-align:right;border:1px solid;'></td>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr style='height:35px;'>");
                stringBuilder1.Append("<td colspan='11' style='text-align:left;border:1px solid;'><b>Reason for Disposal :-</b> " + ADVM.REMARKS + "</td>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("</table>");
                stringBuilder1.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:4px;border: 1px solid;border-collapse: collapse;font-size: 5pt;font-family:Arial'>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th colspan='8' style='text-align:left;background-color: #B8DBFD;border: 1px solid;'>User Approval</th>");
                //stringBuilder1.Append("<th style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>Amount > 5 Million</th>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'>Prepared By</th>");
                stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'>Department Head</th>");
                stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'>Coordinator</th>");
                stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'>Division Head</th>");
                stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'>Execuitive Coordinator</th>");
                stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'>Operating Head</th>");
                stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'>Director</th>");
                stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'>CPO</th>");
                //stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'>President</th>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr style='height: 50px;'>");
                stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'><p>" + ADVM.EMP_NAME + "<br/> " + ADVM.DATEADDED?.ToString("dd-MMM-yyyy") + "</p></th>");
                for (short L = 2; L <= (short)10; L++)
                {
                    if (L != (short)7 && L != (short)8)
                    {
                        //short _SL = Convert.ToInt16(L == (short)10 ? 25 : L == (short)11 ? 10 : (int)L);
                        ApprovalHisViewModel approvalHisViewModel = ADVM.AppHistoryList.Where(a => (int)a.STATUS_LEVEL == (int)L && a.APPROVALSTATUS != 0).FirstOrDefault();
                        if (approvalHisViewModel != null)
                        {
                            string appDate = approvalHisViewModel.DATEADDED.ToString("dd-MMM-yyyy");
                            string appempName = approvalHisViewModel.APPEMP_NAME;
                            switch (approvalHisViewModel.STATUS_LEVEL)
                            {
                                case 2:
                                    stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'><p>" + appempName + "<br/> " + appDate + "</p></th>");
                                    break;
                                case 3:
                                    stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'><p>" + appempName + "<br/> " + appDate + "</p></th>");
                                    break;
                                case 4:
                                    stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'><p>" + appempName + "<br/> " + appDate + "</p></th>");
                                    break;
                                case 5:
                                    stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'><p>" + appempName + "<br/> " + appDate + "</p></th>");
                                    break;
                                case 6:
                                    stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'><p>" + appempName + "<br/> " + appDate + "</p></th>");
                                    break;
                                case 9:
                                    stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'><p>" + appempName + "<br/> " + appDate + "</p></th>");
                                    break;
                                case 10:
                                    stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'><p>" + appempName + "<br/> " + appDate + "</p></th>");
                                    break;
                                    //case 10:
                                    //    stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'><p>" + appempName + "<br/> " + appDate + "</p></th>");
                                    //    break;
                            }
                        }
                        else
                            stringBuilder1.Append("<th style='width:10%;text-align:center;border: 1px solid;'></th>");
                    }
                }
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("</table>");
                Decimal num18 = new Decimal(0, 0, 0, false, (byte)2);
                Decimal num19 = new Decimal(0, 0, 0, false, (byte)2);
                Decimal num20 = new Decimal(0, 0, 0, false, (byte)2);
                Decimal num21 = new Decimal(0, 0, 0, false, (byte)2);
                Decimal num22 = new Decimal(0, 0, 0, false, (byte)2);
                num18 = ADVM.DisposalDetailList.Sum<DisposalDetailViewModel>((Func<DisposalDetailViewModel, Decimal?>)(o => o.ORIGINALCOST)).Value;
                Decimal num23 = ADVM.DisposalDetailList.Sum<DisposalDetailViewModel>((Func<DisposalDetailViewModel, Decimal?>)(d => d.DEPRECIATIONCOST)).Value;
                num20 = ADVM.DisposalDetailList.Sum<DisposalDetailViewModel>((Func<DisposalDetailViewModel, Decimal?>)(w => w.WDV)).Value;
                Decimal num24 = ADVM.DisposalDetailList.Sum<DisposalDetailViewModel>((Func<DisposalDetailViewModel, Decimal?>)(s => s.SALESPRICE)).Value;
                Decimal num25 = ADVM.DisposalDetailList.Sum<DisposalDetailViewModel>((Func<DisposalDetailViewModel, Decimal?>)(p => p.PROFIT_LOSS)).Value;
                int? nullable1 = finalPrint;
                int num26 = 1;
                if (nullable1.GetValueOrDefault() == num26 && nullable1.HasValue)
                {
                    stringBuilder1.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:4px;border: 1px solid;border-collapse: collapse;font-size: 5pt;font-family:Arial'>");
                    stringBuilder1.Append("<tr>");
                    stringBuilder1.Append("<th colspan='4' style='text-align:left;background-color: #B8DBFD;border: 1px solid;'>Finance & Accounts Approval</th>");
                    //stringBuilder1.Append("<th colspan='4' style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>Amount > INR 5 Million</th>");
                    stringBuilder1.Append("</tr>");
                }
                else
                    stringBuilder1.Append("<table cellpadding='3' cellspacing='0' style='width:30%;margin-top:4px;border: 1px solid;border-collapse: collapse;font-size: 5pt;font-family:Arial'>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th colspan='2' style='text-align:left;border: 1px solid;'>Asset Summary</th>");
                nullable1 = finalPrint;
                int num27 = 1;
                if (nullable1.GetValueOrDefault() == num27 && nullable1.HasValue)
                {
                    //stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>Entered By</th>");
                    stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>IC</th>");
                    stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>IBM</th>");
                    //stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>HO Division Head</th>");
                    //stringBuilder1.Append("<th style='text-align:center;border: 1px solid;'>HO Operating Head</th>");
                    //stringBuilder1.Append("<th colspan='2' style='text-align:center;border: 1px solid;'>Director & CFO</th>");
                }
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th style='text-align:left;border: 1px solid;'>Original Cost</th>");
                stringBuilder1.Append("<th style='text-align:right;border: 1px solid;'>" + string.Format("{0:0,0.00}", (object)num18) + "</th>");
                nullable1 = finalPrint;
                int num28 = 1;
                if (nullable1.GetValueOrDefault() == num28 && nullable1.HasValue)
                {
                    ApprovalHisViewModel approvalHisViewModel1 = ADVM.AppHistoryList.Where(a => a.STATUS_LEVEL == (short)11).FirstOrDefault<ApprovalHisViewModel>();
                    DateTime dateadded;
                    string str8;
                    if (approvalHisViewModel1 != null)
                    {
                        string appempName = approvalHisViewModel1.APPEMP_NAME;
                        dateadded = approvalHisViewModel1.DATEADDED;
                        string str9 = dateadded.ToString("dd-MMM-yyyy");
                        str8 = appempName + "<br/> " + str9;
                    }
                    else
                        str8 = "";
                    string str10 = str8;
                    stringBuilder1.Append("<th rowspan='5' style='text-align:center;border: 1px solid;'>" + str10 + "</th>");
                    ApprovalHisViewModel approvalHisViewModel2 = ADVM.AppHistoryList.Where(a => a.STATUS_LEVEL == (short)12).FirstOrDefault<ApprovalHisViewModel>();
                    string str11;
                    if (approvalHisViewModel2 != null)
                    {
                        string appempName = approvalHisViewModel2.APPEMP_NAME;
                        dateadded = approvalHisViewModel2.DATEADDED;
                        string str9 = dateadded.ToString("dd-MMM-yyyy");
                        str11 = appempName + "<br/> " + str9;
                    }
                    else
                        str11 = "";
                    string str12 = str11;
                    stringBuilder1.Append("<th rowspan='5' style='text-align:center;border: 1px solid;'>" + str12 + "</th>");
                    //ApprovalHisViewModel approvalHisViewModel3 = ADVM.AppHistoryList.Where(a => a.STATUS_LEVEL == (short)13).FirstOrDefault<ApprovalHisViewModel>();
                    //string str13;
                    //if (approvalHisViewModel3 != null)
                    //{
                    //    string appempName = approvalHisViewModel3.APPEMP_NAME;
                    //    dateadded = approvalHisViewModel3.DATEADDED;
                    //    string str9 = dateadded.ToString("dd-MMM-yyyy");
                    //    str13 = appempName + "<br/> " + str9;
                    //}
                    //else
                    //    str13 = "";
                    //string str14 = str13;
                    //stringBuilder1.Append("<th rowspan='5' style='text-align:center;border: 1px solid;'>" + str14 + "</th>");
                    //ApprovalHisViewModel approvalHisViewModel4 = ADVM.AppHistoryList.Where(a => a.STATUS_LEVEL == (short)14).FirstOrDefault<ApprovalHisViewModel>();
                    //string str15;
                    //if (approvalHisViewModel4 != null)
                    //{
                    //    string appempName = approvalHisViewModel4.APPEMP_NAME;
                    //    dateadded = approvalHisViewModel4.DATEADDED;
                    //    string str9 = dateadded.ToString("dd-MMM-yyyy");
                    //    str15 = appempName + "<br/> " + str9;
                    //}
                    //else
                    //    str15 = "";
                    //string str16 = str15;
                    //stringBuilder1.Append("<th rowspan='5' style='text-align:center;border: 1px solid;'>" + str16 + "</th>");
                    //ApprovalHisViewModel approvalHisViewModel5 = ADVM.AppHistoryList.Where(a => a.STATUS_LEVEL == (short)15).FirstOrDefault<ApprovalHisViewModel>();
                    //string str17;
                    //if (approvalHisViewModel5 != null)
                    //{
                    //    string appempName = approvalHisViewModel5.APPEMP_NAME;
                    //    dateadded = approvalHisViewModel5.DATEADDED;
                    //    string str9 = dateadded.ToString("dd-MMM-yyyy");
                    //    str17 = appempName + "<br/> " + str9;
                    //}
                    //else
                    //    str17 = "";
                    //string str18 = str17;
                    //stringBuilder1.Append("<th rowspan='5' style='text-align:center;border: 1px solid;'>" + str18 + "</th>");
                    //ApprovalHisViewModel approvalHisViewModel6 = ADVM.AppHistoryList.Where(a => a.STATUS_LEVEL == (short)26).FirstOrDefault<ApprovalHisViewModel>();
                    //string str19;
                    //if (approvalHisViewModel6 != null)
                    //{
                    //    string appempName = approvalHisViewModel6.APPEMP_NAME;
                    //    dateadded = approvalHisViewModel6.DATEADDED;
                    //    string str9 = dateadded.ToString("dd-MMM-yyyy");
                    //    str19 = appempName + "<br/> " + str9;
                    //}
                    //else
                    //    str19 = "";
                    //string str20 = str19;
                    //stringBuilder1.Append("<th colspan='2' rowspan='5' style='text-align:center;border: 1px solid;'>" + str20 + "</th>");
                }
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th style='text-align:left;border: 1px solid;'>Accumulated Depreciation</th>");
                stringBuilder1.Append("<th style='text-align:right;border: 1px solid;'>" + string.Format("{0:0,0.00}", (object)num23) + "</th>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th style='text-align:left;border: 1px solid;'>WDV</th>");
                stringBuilder1.Append("<th style='text-align:right;border: 1px solid;'>" + string.Format("{0:0,0.00}", (object)num20) + "</th>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th style='text-align:left;border: 1px solid;'>Sale Value</th>");
                stringBuilder1.Append("<th style='text-align:right;border: 1px solid;'>" + string.Format("{0:0,0.00}", (object)num24) + "</th>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("<tr>");
                stringBuilder1.Append("<th style='text-align:left;border: 1px solid;'> Profit/Loss</th>");
                stringBuilder1.Append("<th style='text-align:right;border: 1px solid;'>" + string.Format("{0:0,0.00}", (object)num25) + "</th>");
                stringBuilder1.Append("</tr>");
                stringBuilder1.Append("</table>");
                retval = stringBuilder1.ToString();
            }
            return retval;
        }

        [HttpPost]
        public ActionResult ExportToExcel([FromBody] AssetDisposalViewModel ADVM)
        {
            short retVal = 0;
            try
            {
                string str = this.excelHtml(ADVM);
                this.TempData.Remove("EXCELFILE");
                this.TempData["EXCELFILE"] = (object)str;
                retVal = (short)1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
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
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=AssetDisposalExcel.xls");
                Response.Headers.Add("Content-Disposition", "attachment; filename=AssetDisposalExcel.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }
        public async Task<ActionResult> ExportAllAssetItems()
        {
            try
            {
                List<AssetDetailViewModel> iList = await GetAssetDetail();
                string AllItemHtml = AllExcelHtml(iList);
                return File(Encoding.ASCII.GetBytes(AllItemHtml), "application/vnd.ms-excel", "AllAssetItems.xls");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string AllExcelHtml(List<AssetDetailViewModel> ADVMList)
        {
            string str = "";
            if (ADVMList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 12pt;font-family:Arial'>");
                //stringBuilder.Append("<tr><th colspan='14' style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>Asset Information</th></tr>");
                //Change by aumento as on 02082023 for SR51823==================================================
                //stringBuilder.Append("<tr><th colspan='14' style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>Asset Information</th></tr>");
                stringBuilder.Append("<tr><th colspan='16' style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>Asset Information</th></tr>");
                //==============================================================================================
                stringBuilder.Append("<tr>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>SAP Asset Code</th>");
                //Change by aumento as on 02082023 for SR51823==================================================
                stringBuilder.Append("<th style='border: 1px solid;'>Asset Class Name</th>");
                //==============================================================================================
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Asset Description</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Asset Details</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Vender Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Serial No</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>PO No</th>");
                //Change by aumento as on 02082023 for SR51823==================================================
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Cost Center</th>");
                //==============================================================================================
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Plant</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Captalized date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Original Cost(P)</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Depreciation (Q)</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>WDV (R=P-Q)</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                Decimal _orgCost = 0.00M;
                Decimal _depCost = 0.00M;
                Decimal _wdv = 0.00M;
                foreach (AssetDetailViewModel disposalDetail in ADVMList)
                {
                    _orgCost += disposalDetail.ORIGINALCOST;
                    _depCost += disposalDetail.DEPRECIATIONCOST;
                    _wdv += disposalDetail.WDV;
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + disposalDetail.ASSETCODE + "</td>");
                    //Change by aumento as on 02082023 for SR51823=======================================================================
                    stringBuilder.Append("<td style='border: 1px solid;'>" + disposalDetail.ASSETCLASSNAME + "</td>");
                    //===================================================================================================================
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.ASSETDESCRIPTION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.ASSETDETAIL + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.VENDORNAME + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.INVOICENO + "</td>");
                    if (string.IsNullOrEmpty(disposalDetail.INVOICEDATE))
                    {
                        stringBuilder.Append("<td style='border:1px solid;'> </td>");

                    }
                    else
                    {
                        stringBuilder.Append("<td style='border:1px solid;'>" + Convert.ToDateTime(disposalDetail.INVOICEDATE).ToString("dd-MMM-yyyy") + "</td>");

                    }
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.SERIALNUMBER + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.PONUMBER + "</td>");
                    //Change by aumento as on 02082023 for SR51823=======================================================================
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + disposalDetail.COSTCENTER + "</td>");
                    //===================================================================================================================
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.PLANTNAME + "</td>");
                    if (string.IsNullOrEmpty(disposalDetail.CAPTALIZED_DATE.ToString()))
                    {
                        stringBuilder.Append("<td style='border:1px solid;'> </td>");

                    }
                    else
                    {
                        stringBuilder.Append("<td style='border:1px solid;'>" + Convert.ToDateTime(disposalDetail.CAPTALIZED_DATE).ToString("dd-MMM-yyyy") + "</td>");

                    }
                    stringBuilder.Append("<td style='text-align:right;border:1px solid;'>" + disposalDetail.ORIGINALCOST + "</td>");
                    stringBuilder.Append("<td style='text-align:right;border:1px solid;'>" + disposalDetail.DEPRECIATIONCOST + "</td>");
                    stringBuilder.Append("<td style='text-align:right;border:1px solid;'>" + disposalDetail.WDV + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("<tr>");
                stringBuilder.Append("<td colspan='11' style='text-align:right;background-color: #ccc;border: 1px solid;'>Total</td>");
                stringBuilder.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + _orgCost + "</td>");
                stringBuilder.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + _depCost + "</td>");
                stringBuilder.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + _wdv + "</td>");
                stringBuilder.Append("</tr>");
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        public string excelHtml(AssetDisposalViewModel ADVM)
        {
            //Employee_Details employeeDetails = (Employee_Details)this.Session["Employee"];
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            string str = "";
            if (ADVM != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 12pt;font-family:Arial'>");
                stringBuilder.Append("<tr><th colspan='9' style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>Asset Information</th></tr>");
                stringBuilder.Append("<tr>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>SAP Asset Code</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Asset Description</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Asset Details</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Vender Name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Invoice Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Serial No</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>PO No</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Plant</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Captalized date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Original Cost(P)</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Depreciation (Q)</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>WDV (R=P-Q)</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                Decimal _orgCost = 0.00M;
                Decimal _depCost = 0.00M;
                Decimal _wdv = 0.00M;
                foreach (DisposalDetailViewModel disposalDetail in (IEnumerable<DisposalDetailViewModel>)ADVM.DisposalDetailList)
                {
                    _orgCost += disposalDetail.ORIGINALCOST == null ? 0.00M : (Decimal)disposalDetail.ORIGINALCOST;
                    _depCost += disposalDetail.DEPRECIATIONCOST == null ? 0.00M : (Decimal)disposalDetail.DEPRECIATIONCOST;
                    _wdv += disposalDetail.WDV == null ? 0.00M : (Decimal)disposalDetail.WDV;
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + disposalDetail.ASSETCODE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.ASSETDESCRIPTION + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.ASSETDETAIL + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.VENDORNAME + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.INVOICENO + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + Convert.ToDateTime(disposalDetail.INVOICEDATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.SERIALNUMBER + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.PONUMBER + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + disposalDetail.PLANTNAME + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + Convert.ToDateTime(disposalDetail.CAPTALIZED_DATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:right;border:1px solid;'>" + disposalDetail.ORIGINALCOST + "</td>");
                    stringBuilder.Append("<td style='text-align:right;border:1px solid;'>" + disposalDetail.DEPRECIATIONCOST + "</td>");
                    stringBuilder.Append("<td style='text-align:right;border:1px solid;'>" + disposalDetail.WDV + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("<tr>");
                stringBuilder.Append("<td colspan='6' style='text-align:right;background-color: #ccc;border: 1px solid;'>Total</td>");
                stringBuilder.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + _orgCost + "</td>");
                stringBuilder.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + _depCost + "</td>");
                stringBuilder.Append("<td style='text-align:right;background-color: #ccc;border:1px solid;'>" + _wdv + "</td>");
                stringBuilder.Append("</tr>");
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        public ActionResult RequestReport()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AssetDisposalViewModel disposalViewModel = new AssetDisposalViewModel();
            List<SIS_ASSETTYPE_MST> _AssetTypeList = _AssetService.GetAssetTypeList();
            ViewBag.AssetTypeList = new SelectList(_AssetTypeList, "SIS_ASSETTYPE_MSTID", "DESCRIPTION");
            return View(disposalViewModel);
        }

        [HttpPost]
        //public ActionResult RequestReport([FromBody]  long AssetType, long EmpCode, string FromDate, string ToDate)
        public ActionResult RequestReport([FromBody] RequestReport request)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return PartialView("_GetAssetReport", _AssetService.GetRequestReport(request.AssetType, request.EmpCode, request.FromDate, request.ToDate, Convert.ToInt64(this._sessionService.Get<string>("userID").ToString())));
            //return PartialView("_GetAssetReport", _AssetService.GetRequestReport(AssetType, EmpCode, FromDate, ToDate, Convert.ToInt64(this._sessionService.Get<string>("userID").ToString())));
        }       

        [HttpPost]
        //public ActionResult GetAssetItemsById([FromBody]  long Id, string PageType)
        public ActionResult GetAssetItemsById([FromBody] AssetRequest request)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            if (request.PageType == "IBM")
            {
                string validationParmValue = _AssetService.GetValidationParmValue("IBM Approval");
                long VaildIbmAmt = string.IsNullOrEmpty(validationParmValue) ? 0 : Convert.ToInt64(validationParmValue);
                AssetDisposalViewModel AssetObj = _AssetService.GetAssetRequestById(request.Id, _Employee_Details);
                List<DisposalDetailViewModel> disposalDtList = new List<DisposalDetailViewModel>();
                if (AssetObj.DisposalDetailList?.Count > 0)
                {
                    disposalDtList = AssetObj.DisposalDetailList.Where(d => d.ORIGINALCOST >= VaildIbmAmt).ToList();
                    AssetObj.DisposalDetailList = disposalDtList;
                }
                return PartialView("_GetAssetItemsById", AssetObj);
            }
            else
            {
                return PartialView("_GetAssetItemsById", _AssetService.GetAssetRequestById(request.Id, _Employee_Details));
            }
        }

        //Added by TTL :: CR6754
        [HttpGet]
        public ActionResult GetEmpDetails(long EmpCode)
        {
            Employee_Details _Employee_Details = new Employee_Details();

            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            _Employee_Details = _AssetService.GetEmpDetailById(EmpCode);
            if (_Employee_Details != null)
            {
                return Json(_Employee_Details);
            }
            else
            {
                return null;
            }
        }
        //End by TTL :: CR6754

        [HttpGet]
        public ActionResult GetPPCPlantAuthority(long siteId, short statusLevel)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            string _1F = ""; string _2F = ""; string _3F = ""; string _4F = "";
            List<ApprovalAuthorityViewModel> ppcAuthorityList = _AssetService.ManageApprovalAuthorityByPPC().Where(x => x.STATUS_LEVEL == 23 && x.ACTIVE == true).OrderBy(o => o.SYSITEID).ToList();
            return Json(new
            {
                res = ppcAuthorityList,
            });
        }

        [HttpPost]
        //[ValidateInput(false)]
        public FileResult ExportAssetItems(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "AssetItems.xls");
        }
        //------------------START-[Code Added by Aumento on 23-Dec-2023]------------------------------
        public ActionResult ExportToExcelICDisposalData()
        {
            short retVal = 0;
            try
            {
                List<AssetDisposalViewModel> lstData = _AssetService.GetRequestCancellationListForAdmin();

                string str = this.excelHtmlICDisposalData(lstData);
                this.TempData.Remove("EXCELFILE");
                this.TempData["EXCELFILE"] = (object)str;
                retVal = (short)1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }
        public string excelHtmlICDisposalData(List<AssetDisposalViewModel> lstData)
        {
            string str = "";
            if (lstData != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 12pt;font-family:Arial'>");
                stringBuilder.Append("<tr><th colspan='14' style='text-align:center;background-color: #B8DBFD;border: 1px solid;'>Asset Disposal Information</th></tr>");
                stringBuilder.Append("<tr>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Asset Type</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request By</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>No. of Asset</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>IBM Approval Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Process Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>IC Month</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Implementation Month</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Operation</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Original Cost Summary</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Net Value</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (AssetDisposalViewModel ad in lstData)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.ASSETTYPE + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.EMP_NAME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.DATEADDED + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.AssetCount + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (ad.ACTIVE == true ? "Active" : "Deactive") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.IBM_APPDATE + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.PROCESSSTATUS + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.ICMONTH + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.IMPLEMENTMONTH + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.Emp_Detail._OpDesc + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.DisposalDetailList.Sum(o => o.ORIGINALCOST ?? 0).ToString("#,##0.00") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ad.DisposalDetailList.Sum(o => o.WDV ?? 0).ToString("#,##0.00") + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }
        //------------------END-[Code Added by Aumento on 23-Dec-2023]------------------------------
        #endregion

        #region Admin Page
        [HttpPost]
        public ActionResult GetSecurityApprovalList()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            List<AssetDisposalViewModel> ADVMList = _AssetService.GetApprovalListBySecurity(_Employee_Details);
            //return Json(ADVMList, JsonRequestBehavior.AllowGet);
            return Json(ADVMList);
        }

        [HttpGet]
        public ActionResult DetailForSecurity(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            AssetDisposalViewModel ADVM = _AssetService.GetDetailById(id, _Employee_Details);
            //return Json(ADVM, JsonRequestBehavior.AllowGet);
            return Json(ADVM);
            #endregion
        }
    }
}
