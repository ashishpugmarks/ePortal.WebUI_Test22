using Newtonsoft.Json;
using System.Data;
using ePortal.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ePortal.DomainClasses;
using Microsoft.AspNetCore.Mvc;
using ePortal.Shared.Interface;
using ePortal.Shared;
using System.Reflection;
using ePortal.WebUI.Filters;
using ePortal.Application.Contracts;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class NSPPaymentController : Controller
    {
        private string sJSON = String.Empty;
        private readonly INSPPaymentServices _Insp;
        private readonly ISessionService _sessionService;
        private readonly ILogger<NSPPaymentController> _logger;
        private readonly string _userId;
        private readonly string _userName;
        private readonly Employee_Details _EmpDetails;
        LibResult res;
        private const string name="";

        public NSPPaymentController(INSPPaymentServices Insp, ISessionService sessionService, ILogger<NSPPaymentController> logger)
        {
            _Insp = Insp;
            res = new LibResult();
            _logger = logger;
            _sessionService = sessionService;
            _userId = _sessionService.Get<string>("userID").ToString();
            _userName = _sessionService.Get<string>("userName").ToString();
            _EmpDetails = _sessionService.Get<Employee_Details>("Employee");
        }

        #region  Request Page 


        public ActionResult NSPPaymentList()
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            return View();
        }

        [HttpGet]
        public JsonResult GetNSPPaymentDataList(short? status, string FromDate, string ToDate)
        {
            List<NSP_PaymentHeaderViewModel> ilist = new List<NSP_PaymentHeaderViewModel>();
            string loginEmp = _userId.ToString();
            ilist = _Insp.GetNSPPaymentList(loginEmp, status, FromDate, ToDate);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        [HttpGet]
        public ActionResult NSPPaymentRequestCreate()
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            int loginEmp = int.Parse(_userId.ToString());
            ViewBag.Location = _Insp.GetLocationHelp();
            ViewBag.CostCenter = _Insp.GetCostCenterHelp();
            ViewBag.SelectedLocation = _Insp.GetLoginEmpLocation(loginEmp);
            return View();
        }

        public JsonResult SaveDetail(string modelData, string DetailSaveData, string AuthoritySaveData, string Flag)
        {
            try
            {
                Console.WriteLine("modelData:", modelData);
                Console.WriteLine("DetailSaveData:", DetailSaveData);
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    res.hasError = true;
                    res.errorMessage = "Your session has expired. Please log in again!";
                    return Json(new { result = res });
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                NSP_PaymentHeaderViewModel obj = JsonConvert.DeserializeObject<NSP_PaymentHeaderViewModel>(modelData);
                var loginEmps = int.Parse(_userId.ToString());
                List<NSP_PaymentDetailsViewModel> detailsList = JsonConvert.DeserializeObject<List<NSP_PaymentDetailsViewModel>>(DetailSaveData);
                List<NSP_ApprovalAuthority> authorityList = JsonConvert.DeserializeObject<List<NSP_ApprovalAuthority>>(AuthoritySaveData);

                //res = _Insp.SaveDetails(Convert.ToInt16(obj.RequestRefNo), Convert.ToDateTime(obj.RequestDate), obj.Category, obj.Location, obj.CostCentre, obj.Budgeted, obj.PurposeofExpenses, obj.VendorCode, obj.VendorName, obj.VendorPANNo, obj.VendorAddress, obj.BankName, obj.BankAccountNo, obj.BankIFSCCode, obj.TotalNetAmount, obj.TotalNetAmountInWord, detailsList, authorityList, loginEmps, obj.CurrencyType, obj.RequestType);
                res = _Insp.SaveDetails(loginEmps, obj, detailsList, authorityList, Flag);

                return Json(new { result = res });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new { result = false, errorMessage = ex.Message });
            }
        }

        public JsonResult AutocompleteSuggestions(string term, string catagory)
        {
            string sJSON = string.Empty;
            LibResult res = new LibResult();
            try
            {
                res = _Insp.VendorAutocompleteSuggestions(term, catagory);
                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                sJSON = JsonConvert.SerializeObject(res.resultObject);
                return Json(sJSON);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }

        }

        public ActionResult AutocompleteSuggestionsEmp(string term, string designation)
        {
            try
            {
                var suggestions = _Insp.GETEmployee(term, designation);
                return Json(suggestions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }

        }

        public ActionResult GetReferenceNumber(string term)
        {
            try
            {
                var suggestions = _Insp.GetReferenceNumber(term);
                return Json(suggestions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }
        public string Designation(string term)
        {
            TempData["Designation"] = term;
            return term;
        }
        [HttpGet]
        public JsonResult Gethead(int RequestNo, string ApprovalType)
        {
            try
            {
                long LoginCode = Convert.ToInt32(_userId);

                //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                Employee_Details _Employee_Details = _EmpDetails;
                //long? operationMappId = _Insp.GetOperationMappId(_Employee_Details);

                List<NSP_ApprovalAuthority> getauthority = _Insp.Gethead(LoginCode, RequestNo, ApprovalType);
                //TempData["AUTHORITY_LIST"] = getauthority.ToList();

                return Json(new
                {
                    LIST = getauthority
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new
                {
                    LIST = new List<NSP_PaymentHeaderViewModel>()
                });
            }
        }
        public ActionResult UploadFiles(string Type)
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            List<string> retFname = new List<string>();
            if (Request.Form.Files.Count > 0)
            {
                long EmpCode = Int32.Parse(_userId.ToString());

                try
                {
                    IFormFileCollection files = Request.Form.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //var newFileName = Convert.ToString(files.AllKeys[i]);
                        var newFileName = files[i].Name;
                        IFormFile file = files[i];
                        string fname;

                        //if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        //{
                        //    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        //    fname = testfiles[testfiles.Length - 1];
                        //}
                        //else
                        //{
                        //    fname = file.FileName;
                        //}
                        fname = Path.GetFileName(file.FileName);
                        String FileNM = newFileName.Substring(0, 1) + "_" + Type + '_' + EmpCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + i.ToString() + Path.GetExtension(fname);
                        string path = Path.Combine(serverpath.getFileUploadPath(), "NSPPayment");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        fname = Path.Combine(path, FileNM);
                        //file.SaveAs(fname);

                        using (var stream = new FileStream(fname, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        retFname.Add(FileNM);
                    }
                    return Json(retFname);
                }

                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        #region  Edit Request
        public ActionResult NSPPaymentRequestEdit(int id)
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            try
            {
                ViewBag.id = id;
                ViewBag.Location = _Insp.GetLocationHelp();
                ViewBag.CostCenter = _Insp.GetCostCenterHelp();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetNSPPaymentRequestEditData(int id)
        {
            long EmpCode = Int32.Parse(_userId.ToString());
            NSP_PaymentHeaderViewModel paymentheader = _Insp.GetNSPEditDataById(id, EmpCode);

            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            sJSON = JsonConvert.SerializeObject(paymentheader);
            return Json(sJSON);
        }

        public JsonResult UpdateDetail(string modelData, string DetailSaveData, string AuthoritySaveData, string Flag)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    res.hasError = true;
                    res.errorMessage = "Your session has expired. Please log in again!";
                    return Json(new { result = res });
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                NSP_PaymentHeaderViewModel obj = JsonConvert.DeserializeObject<NSP_PaymentHeaderViewModel>(modelData);
                var loginEmps = int.Parse(_userId.ToString());
                List<NSP_PaymentDetailsViewModel> detailsList = JsonConvert.DeserializeObject<List<NSP_PaymentDetailsViewModel>>(DetailSaveData);
                List<NSP_ApprovalAuthority> authorityList = JsonConvert.DeserializeObject<List<NSP_ApprovalAuthority>>(AuthoritySaveData);
                res = _Insp.UpdateDetails(loginEmps, obj, detailsList, authorityList, Flag);

                return Json(new { result = res });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new { result = false, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult NSPPaymentRequestDtRowDelete(int DetailID, int HeaderID)
        {

            bool rest = _Insp.NSPPaymentRequestDtRowDelete(DetailID, HeaderID);
            return Json(rest);
        }


        [HttpPost]
        public JsonResult NSPPaymentRequestDelete(int id)
        {
            bool rest = _Insp.DeleteWholeRequest(id);
            return Json(rest);
        }
        #endregion



        #endregion Request Page 

        #region Approval
        public ActionResult ApprovalNSPPaymentList()
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            return View();
        }

        [HttpGet]
        public JsonResult ApprovalGetNSPPaymentDataList(short? status, string FromDate, string ToDate)
        {
            try
            {
                List<NSP_PaymentHeaderViewModel> Ilist = new List<NSP_PaymentHeaderViewModel>();
                long loginEmp = (Convert.ToInt64(_userId.ToString()));
                Ilist = _Insp.GetApprovalNSPPaymentList(loginEmp, status, FromDate, ToDate);
                sJSON = JsonConvert.SerializeObject(Ilist);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        public ActionResult NSPPaymentRequestEditApproval(int id)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                NSP_PaymentHeaderViewModel paymentheader = _Insp.GetNSPById(id, Convert.ToInt64(_userId.ToString()), "APPROVAL");
                ViewBag.Location = _Insp.GetLocationHelp();
                ViewBag.CostCenter = _Insp.GetCostCenterHelp();
                return View(paymentheader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult ApproveReject(int HEADERID, string EMPCODE, string Response, string Remark)
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            //            long loginEmp = (Convert.ToInt64(_userId.ToString()));
            //            res = _Insp.ApproveReject(HEADERID, EMPCODE, Response, Remark, loginEmp);
            //            JsonSerializerSettings settings = new JsonSerializerSettings
            //            {
            //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //            };
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            if (_userId == null)
            {
                res.hasError = true;
                res.errorMessage = "Your session has expired. Please log in again!";
                sJSON = JsonConvert.SerializeObject(res, settings);
                return Json(sJSON);
            }

            long loginEmp = (Convert.ToInt64(_userId.ToString()));
            res = _Insp.ApproveReject(HEADERID, EMPCODE, Response, Remark, loginEmp);
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            sJSON = JsonConvert.SerializeObject(res, settings);
            return Json(sJSON);
        }

        #endregion Approval

        #region Finance 
        public ActionResult NSPFinanceList()
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            int loginEmp = (Convert.ToInt32(_userId.ToString()));
            ViewBag.Rights = _Insp.GetPostReverseRights(loginEmp).resultObject;
            return View();
        }

        [HttpGet]
        public JsonResult GetNSPFinanceList(short? status, string FromDate, string ToDate, string InvoiceSearchKey) // InvoiceSearchKey ADDED BY AUMENTO :: SR88023-CR5305
        {
            List<NSP_PaymentHeaderViewModel> ilist = new List<NSP_PaymentHeaderViewModel>();
            int? loginEmp = (Convert.ToInt32(_userId.ToString()));
            ilist = _Insp.GetNSPFinanceList(status, loginEmp, FromDate, ToDate, InvoiceSearchKey); // InvoiceSearchKey ADDED BY AUMENTO :: SR88023-CR5305
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        public ActionResult NSPFinanceEdit(int id)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                ViewBag.CostCenter = _Insp.GetCostCenterHelp();
                ViewBag.ID = id;
                ViewBag.TDSTypeList = _Insp.GetTDSTypeList();
                ViewBag.ESTaxData = _Insp.GetESTax();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetNSPPaymentFinanceTaxationEditData(int id, string DepHead)
        {
            long EmpCode = Int32.Parse(_userId.ToString());
            NSP_PaymentHeaderViewModel paymentheader = _Insp.GetNSPById(id, EmpCode, DepHead);

            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            sJSON = JsonConvert.SerializeObject(paymentheader);
            return Json(sJSON);
        }

        [HttpGet]
        public JsonResult GetGSTRateList(string Type, string TaxType, string COSTCENTER, int Status)
        {
            long EmpCode = Int32.Parse(_userId.ToString());
            var Data = _Insp.GetGSTRateList(Type, TaxType, COSTCENTER, Status);
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            sJSON = JsonConvert.SerializeObject(Data);
            return Json(sJSON);
        }


        [HttpPost]
        public JsonResult ApproveFinanceReject(string HeaderData, string DetailData)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                if (_userId == null)
                {
                    res.hasError = true;
                    res.errorMessage = "Your session has expired. Please log in again!";
                    sJSON = JsonConvert.SerializeObject(res, settings);
                    return Json(sJSON);
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                NSP_PaymentHeaderViewModel obj = JsonConvert.DeserializeObject<NSP_PaymentHeaderViewModel>(HeaderData);
                var loginEmps = int.Parse(_userId.ToString());
                List<NSP_PaymentDetailsViewModel> detailsList = JsonConvert.DeserializeObject<List<NSP_PaymentDetailsViewModel>>(DetailData);
                res = _Insp.ApproveFinanceReject(obj, detailsList, loginEmps);

                sJSON = JsonConvert.SerializeObject(res, settings);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        #endregion Finance

        #region Taxation
        public ActionResult NSPTaxation()
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            return View();
        }

        [HttpGet]
        public JsonResult GetNSPTaxation(short? status, string FromDate, string ToDate)
        {
            List<NSP_PaymentHeaderViewModel> ilist = new List<NSP_PaymentHeaderViewModel>();
            int loginEmp = (Convert.ToInt32(_userId.ToString()));
            ilist = _Insp.GetNSPTaxationList(status, loginEmp, FromDate, ToDate);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }
        public ActionResult NSPTaxationEdit(int id)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305

                ViewBag.ID = id;
                ViewBag.TDSTypeList = _Insp.GetTDSTypeList();
                ViewBag.ESTaxData = _Insp.GetESTax();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult ApproveTaxationReject(string HeaderData, string DetailData)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                if (_userId == null)
                {
                    res.hasError = true;
                    res.errorMessage = "Your session has expired. Please log in again!";
                    sJSON = JsonConvert.SerializeObject(res, settings);
                    return Json(sJSON);
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                NSP_PaymentHeaderViewModel obj = JsonConvert.DeserializeObject<NSP_PaymentHeaderViewModel>(HeaderData);
                var loginEmps = int.Parse(_userId.ToString());
                List<NSP_PaymentDetailsViewModel> detailsList = JsonConvert.DeserializeObject<List<NSP_PaymentDetailsViewModel>>(DetailData);
                res = _Insp.ApproveTaxationReject(obj, detailsList, loginEmps);
                sJSON = JsonConvert.SerializeObject(res, settings);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        #endregion Taxation

        #region Common Function

        [HttpGet]
        public ActionResult GetNSPHistoryData(int id)
        {
            try
            {
                List<NSP_ApprovalAuthority> paymentheader = _Insp.GetNSPHistoryById(id);

                return View(paymentheader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        #endregion

        #region GenerateNSPReport
        public JsonResult GeneratePdfReport(int HeaderID, string Mode)
        {
            try
            {

                string filename = _userId.ToString() + "_" + HeaderID + "_" + "NSPHeaderForm" + ".pdf";
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //string relativePath = "Uploads/NSPPayment/";
                //string fullPath = Path.Combine(baseDirectory, relativePath, filename);
                //string fullPath = Path.Combine(Server.MapPath("~/Uploads/NSPPayment"), filename);
                
                string path = Path.Combine(serverpath.getFileUploadPath(), "NSPPayment");
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                string fullPath = Path.Combine(path, filename);
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                //if (System.IO.File.Exists(fullPath))
                //{

                //    return Json(new { filePath = fullPath });
                //}
                //else
                //{

                NSPRequestReport NSPReport = new NSPRequestReport();
                byte[] abytes = NSPReport.PrepareReport(_Insp.GetNSPReportDetail(HeaderID), Mode);


                System.IO.File.WriteAllBytes(fullPath, abytes);


                return Json(new { filePath = filename });
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class NSPRequestReport
        {
            int _totalColumn = 10;
            Document _document;
            Font _fontStyle;
            PdfPTable _pdfTable = new PdfPTable(10);
            PdfPCell _pdfPCell;
            MemoryStream _memoryStream = new MemoryStream();
            NSPReportData ReportData = new NSPReportData();
            NSPReportData_Initiate_DT Initiate_DT = new NSPReportData_Initiate_DT();
            List<NSPReportData_Invoice_DT> Invoice_DT = new List<NSPReportData_Invoice_DT>();
            List<NSPReportData_Approval_DT> Approval_DT = new List<NSPReportData_Approval_DT>();
            public byte[] PrepareReport(NSPReportData Data, string Mode)
            {
                ReportData = Data;
                Initiate_DT = Data.Initiate_DT;
                Invoice_DT = Data.Invoice_DT;
                Approval_DT = Data.Approval_DT;


                //  _document = new Document(new Rectangle(PageSize.A4.Height, PageSize.A4.Width), 0f, 0f, 0f, 0f);
                _document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);

                _document.SetMargins(5f, 5f, 5f, 5f);
                _pdfTable.WidthPercentage = 100;
                _pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;
                _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);



                PdfWriter pdfWriter = PdfWriter.GetInstance(_document, _memoryStream);

                _document.Open();
                _pdfTable.SetWidths(new float[] { 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f });

                // decimal inv = (from s in Invoice_DT select Convert.ToDecimal(s.Amount)).Sum();



                this.ReportHeader(ReportData);
                this.ReportBody(Invoice_DT, Data, Mode);
                this.InitiateData(Initiate_DT);
                this.Approval(Approval_DT);
                _document.Add(_pdfTable);
                _document.Close();
                return _memoryStream.ToArray();
            }

            private void ReportHeader(NSPReportData Data)
            {
                if (Data != null)
                {

                    String ReportTitle = "Honda Motorcycle & Scooter India Pvt. Ltd.";
                    _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);


                    _pdfPCell = new PdfPCell(new Phrase(ReportTitle, _fontStyle));


                    _pdfPCell.Colspan = _totalColumn;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.Border = Rectangle.BOTTOM_BORDER;
                    _pdfPCell.PaddingBottom = 10f;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.ExtraParagraphSpace = 0;
                    _pdfTable.AddCell(_pdfPCell);
                    _pdfTable.CompleteRow();

                    _pdfPCell = new PdfPCell(new Phrase("Request Ref No : " + Data.RequestRefNo, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.BorderWidthBottom = 0;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase("Payment Request ( Non SAP)", _fontStyle));
                    _pdfPCell.Colspan = 8;
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    //if (Data.FY_RequestTranType == "NEW")
                    //{
                    //    _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                    //}
                    //else
                    //{
                    //    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    //}

                    _pdfTable.AddCell(_pdfPCell);

                    //_pdfPCell = new PdfPCell(new Phrase("Settlement Request ( Non SAP)", _fontStyle));
                    //_pdfPCell.Colspan = 3;
                    //_pdfPCell.Rowspan = 2;
                    //_pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //_pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //if (Data.FY_RequestTranType == "SETTLEMENT")
                    //{
                    //    _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                    //}
                    //else
                    //{
                    //    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    //}
                    //_pdfTable.AddCell(_pdfPCell);

                    string RequestDate_ = Data.RequestDate != null ? DateTime.Parse(Data.RequestDate).ToString("dd/MM/yyyy") : "";

                    _pdfPCell = new PdfPCell(new Phrase("Request Date : " + RequestDate_, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase("Category ", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase("Vendor", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    if (Data.Category.ToString() == "Vendor")
                    {
                        _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                    }
                    else
                    {
                        _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    }

                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase("Dealer", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    if (Data.Category.ToString() == "Dealer")
                    {
                        _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                    }
                    else
                    {
                        _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    }


                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase("Employees", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;

                    if (Data.Category.ToString() == "Employees")
                    {
                        _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                    }
                    else
                    {
                        _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    }
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase("Others", _fontStyle));
                    _pdfPCell.Colspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;

                    if (Data.Category.ToString() == "Others")
                    {
                        _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                    }
                    else
                    {
                        _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    }
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _pdfPCell.Colspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase("Location ", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(Data.Location, _fontStyle));
                    _pdfPCell.Colspan = 9;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string VendorCode = Data.VendorCode.ToString();
                    string VendorName = Data.VendorName.ToString();
                    string VendorPanNo = Data.VendorPANNo.ToString();
                    string VendorAddress = Data.VendorAddress != null ? Data.VendorAddress.ToString() : "";
                    string CostCenter = Data.CostCentre.ToString();
                    string Budgeted = Data.Budgeted.ToString() == "Yes" ? "Budgeted" : "Non-Budgeted";
                    string PurposeofExpenses = Data.PurposeofExpenses != null ? Data.PurposeofExpenses.ToString() : "";



                    _pdfPCell = new PdfPCell(new Phrase("Vendor Code/ Dealer Code/ Benifciery Name", _fontStyle));
                    _pdfPCell.Rowspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);




                    _pdfPCell = new PdfPCell(new Phrase(VendorCode, _fontStyle));
                    _pdfPCell.Rowspan = 3;
                    _pdfPCell.Colspan = 6;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase("Cost Centre ", _fontStyle));
                    _pdfPCell.Rowspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(CostCenter, _fontStyle));
                    _pdfPCell.Rowspan = 3;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase("Vendor PAN Numbe", _fontStyle));
                    _pdfPCell.Rowspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(VendorPanNo, _fontStyle));
                    _pdfPCell.Colspan = 6;
                    _pdfPCell.Rowspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase("Budgeted", _fontStyle));
                    _pdfPCell.Rowspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(Budgeted, _fontStyle));
                    _pdfPCell.Rowspan = 3;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase("Name of Vendor", _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(VendorName, _fontStyle));
                    _pdfPCell.Colspan = 9;
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase("Address of Vendor", _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(VendorAddress, _fontStyle));
                    _pdfPCell.Colspan = 9;
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _pdfPCell.Colspan = _totalColumn;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.Border = 0;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.ExtraParagraphSpace = 0;
                    _pdfTable.AddCell(_pdfPCell);
                    _pdfTable.CompleteRow();



                    _pdfPCell = new PdfPCell(new Phrase("Detailed Purpose of Expenses - Material / Services / Assets  < Used Where & Why  >Expenses - < Why Incurred >", _fontStyle));
                    _pdfPCell.Colspan = 4;
                    _pdfPCell.Rowspan = 4;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(PurposeofExpenses, _fontStyle));
                    _pdfPCell.Colspan = 6;
                    _pdfPCell.Rowspan = 4;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfTable.CompleteRow();
                }


            }
            private void ReportBody(List<NSPReportData_Invoice_DT> Invoice_DT, NSPReportData Data, string Mode)
            {
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = 7;
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("SBI Forex Rate  Date ", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);



                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);



                _pdfPCell = new PdfPCell(new Phrase("Invoice No.", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Invoice Date", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Description of Goods & Services", _fontStyle));
                _pdfPCell.Colspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Currency ", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Amount", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Ex rate", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Amount in INR", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);



                foreach (var item in Invoice_DT)
                {
                    _pdfPCell = new PdfPCell(new Phrase(item.InvoiceNo, _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string InvoiceDate_ = item.InvoiceDate != null ? DateTime.Parse(item.InvoiceDate).ToString("dd/MM/yyyy") : "";

                    _pdfPCell = new PdfPCell(new Phrase(InvoiceDate_, _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase(item.Description, _fontStyle));
                    _pdfPCell.Colspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(item.Currency, _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string Amount_ = item.Amount.ToString().ToString();

                    _pdfPCell = new PdfPCell(new Phrase(Amount_, _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string Exrate_ = item.Exrate.ToString().ToString();

                    _pdfPCell = new PdfPCell(new Phrase(Exrate_, _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string AmountINR_ = (item.Amount == null ? 0 : decimal.Parse(item.Amount) * (item.Exrate == null ? 1 : decimal.Parse(item.Exrate))).ToString();

                    _pdfPCell = new PdfPCell(new Phrase(AmountINR_, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);
                }

                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                // _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();




                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Total Amount =", _fontStyle));
                _pdfPCell.Colspan = 4;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                decimal TotalAmount_Deci = (from s in Invoice_DT select Convert.ToDecimal(s.Amount)).Sum();
                string TotalAmount_ = TotalAmount_Deci.ToString();
                _pdfPCell = new PdfPCell(new Phrase(TotalAmount_, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Less : TDS Deduction", _fontStyle));
                _pdfPCell.Colspan = 4;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                string TDSDeduction_ = Mode == "User" ? "0" : Data.TDSDeduction.ToString();
                _pdfPCell = new PdfPCell(new Phrase(TDSDeduction_, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);



                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Less : Advance Paid / Adjustments", _fontStyle));
                _pdfPCell.Colspan = 4;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                string AdvancePaid_Adjustments_ = Mode == "User" ? "0" : Data.AdvancePaid_Adjustments.ToString();
                _pdfPCell = new PdfPCell(new Phrase(AdvancePaid_Adjustments_, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);



                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Net Payable Amount =", _fontStyle));
                _pdfPCell.Colspan = 4;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);
                string NetPayableAmount_ = (TotalAmount_Deci - decimal.Parse(AdvancePaid_Adjustments_) - decimal.Parse(TDSDeduction_)).ToString();

                _pdfPCell = new PdfPCell(new Phrase(NetPayableAmount_, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

            }
            private void InitiateData(NSPReportData_Initiate_DT Initiate_DT)
            {
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Intiator Name", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Designation", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Department", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Division", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Operation", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Location", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Initiate_DT.IntiatorName, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Initiate_DT.Designation, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Initiate_DT.Department, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Initiate_DT.Division, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase(Initiate_DT.Operation, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase(Initiate_DT.Dt_Location, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

            }


            private void Approval(List<NSPReportData_Approval_DT> Approval_DT)
            {
                int totalRecords = Approval_DT.Count;
                int chunkSize = 5;
                int numberOfChunks = (int)Math.Ceiling((double)totalRecords / chunkSize);

                for (int i = 0; i < numberOfChunks; i++)
                {

                    int skipCount = i * chunkSize;
                    List<NSPReportData_Approval_DT> Approval_DTCurrent = Approval_DT.Skip(skipCount).Take(chunkSize).ToList();
                    bool isHeader = (i == 0);
                    BindApproval(Approval_DTCurrent, isHeader);
                }
            }


            private void BindApproval(List<NSPReportData_Approval_DT> Approval_DT, bool bindHeader)
            {

                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);

                if (bindHeader)
                {
                    _pdfPCell = new PdfPCell(new Phrase("Approval Authority", _fontStyle));
                    _pdfPCell.Colspan = _totalColumn;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.Border = 0;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.ExtraParagraphSpace = 0;
                    _pdfTable.AddCell(_pdfPCell);
                }
                int App_TitleColspan = 0;

                int remainingCol = (_totalColumn - Approval_DT.Count);

                int colspan_ = 0;

                if (remainingCol > 0 && remainingCol < _totalColumn)
                {
                    colspan_ = (remainingCol / Approval_DT.Count);
                }


                foreach (var item in Approval_DT)
                {
                    _pdfPCell = new PdfPCell(new Phrase(item.App_Title, _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.YELLOW;
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.Colspan = colspan_;
                    _pdfTable.AddCell(_pdfPCell);
                    App_TitleColspan++;
                }

                if (Approval_DT.Count < 10)
                {
                    _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.Colspan = (_totalColumn - App_TitleColspan);
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.Border = 0;
                    _pdfTable.AddCell(_pdfPCell);
                }
                _pdfTable.CompleteRow();

                int App_NameColspan = 0;
                foreach (var item in Approval_DT)
                {

                    _pdfPCell = new PdfPCell(new Phrase(item.App_Name, _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.Colspan = colspan_;
                    _pdfPCell.Rowspan = 2;
                    _pdfTable.AddCell(_pdfPCell);
                    App_NameColspan++;

                }

                if (Approval_DT.Count < 10)
                {
                    _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.Colspan = (_totalColumn - App_NameColspan);
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.Border = 0;
                    _pdfTable.AddCell(_pdfPCell);
                }
                _pdfTable.CompleteRow();

                int App_DateColspan = 0;
                foreach (var item in Approval_DT)
                {
                    string AppDate_ = item.App_Date != null ? DateTime.Parse(item.App_Date).ToString("dd/MM/yyyy") : "";
                    _pdfPCell = new PdfPCell(new Phrase("Approve Date : " + AppDate_, _fontStyle));
                    _pdfPCell.Colspan = colspan_;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);
                    App_DateColspan++;

                }

                if (Approval_DT.Count < 10)
                {
                    _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.Colspan = (_totalColumn - App_DateColspan);
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.Border = 0;
                    _pdfTable.AddCell(_pdfPCell);
                }
                _pdfTable.CompleteRow();

            }




        }

        #endregion

        #region MAster


        #region Role Master
        public ActionResult NSP_ROLE_MASTER_List()
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetNSP_ROLE_MASTER_List()
        {
            try
            {
                var Ilist = _Insp.GetNSP_Role_Master_List_Data();
                sJSON = JsonConvert.SerializeObject(Ilist);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        public ActionResult NSP_ROLE_MASTER_Add()
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        public ActionResult NSP_ROLE_MASTER_Edit(int SrNO)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                ViewBag.Record = _Insp.GetNSP_Role_Master_Edit_Data(SrNO).resultObject;
                ViewBag.SrNo = SrNO;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetNSP_ROLE_MASTER_Edit(int SrNO)
        {
            try
            {
                var Ilist = _Insp.GetNSP_Role_Master_Edit_Data(SrNO);
                sJSON = JsonConvert.SerializeObject(Ilist);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult NSP_Role_Master_SaveAndUpdate_Data(string Data, string Mode)
        {
            try
            {
                NSP_ROLE_MASTER obj = JsonConvert.DeserializeObject<NSP_ROLE_MASTER>(Data);
                var loginEmp = int.Parse(_userId.ToString());

                res = _Insp.NSP_Role_Master_SaveAndUpdate_Data(obj, Mode, loginEmp); JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                sJSON = JsonConvert.SerializeObject(res, settings);
                return Json(sJSON);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult DeActive_Role_Master_Record(int SrNo)
        {
            var loginEmp = int.Parse(_userId.ToString());
            var rest = _Insp.DeActive_Role_Master_Record(SrNo, loginEmp);
            return Json(rest);
        }

        public JsonResult BindPlantName()
        {
            res.resultObject = _Insp.BindPlantName();
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }




        #endregion

        #region TDS Master

        public ActionResult NSP_TDS_TAX_TYPE_MASTER_List()
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                var Data = _Insp.GetNSP_TDS_TAX_TYPE_MASTER_List_Data().resultObject;

                return View(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }


        public ActionResult NSP_NSP_TDS_TAX_TYPE_MASTER_Add()
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult NSP_NSP_TDS_TAX_TYPE_MASTER_SaveData(string Data, string Mode)
        {
            try
            {
                NSP_TDS_TAX_TYPE_MASTER obj = JsonConvert.DeserializeObject<NSP_TDS_TAX_TYPE_MASTER>(Data);
                var loginEmp = int.Parse(_userId.ToString());

                res = _Insp.NSP_NSP_TDS_TAX_TYPE_MASTER_SaveData(obj, loginEmp, Mode); JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                sJSON = JsonConvert.SerializeObject(res, settings);
                return Json(sJSON);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        public ActionResult NSP_TDS_TAX_TYPE_MASTER_Edit(int SrNO)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                var Data = _Insp.GetNSP_TDS_TAX_TYPE_MASTER_Edit_Data(SrNO).resultObject;
                ViewBag.SrNo = SrNO;

                return View(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult DeActive_NSP_TDS_TAX_TYPE_MASTER_Record(int SrNo)
        {
            var loginEmp = int.Parse(_userId.ToString());
            var rest = _Insp.DeActive_NSP_TDS_TAX_TYPE_MASTER_Record(SrNo, loginEmp);
            return Json(rest);
        }


        #endregion


        #region Bsiness Profit Master

        public ActionResult NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_List()
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                var Data = _Insp.GetNSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_List_Data().resultObject;

                return View("NSPBUPRO_MASTER_List", Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_SaveData(string Data, string Mode)
        {
            try
            {
                NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER obj = JsonConvert.DeserializeObject<NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER>(Data);
                var loginEmp = int.Parse(_userId.ToString());

                res = _Insp.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_SaveData(obj, loginEmp, Mode); JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                sJSON = JsonConvert.SerializeObject(res, settings);
                return Json(sJSON);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        public ActionResult NSPBUPRO_MASTER_Add()
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        public ActionResult NSPBUPRO_MASTER_Edit(int SrNO)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                var Data = _Insp.GetNSPBUPRO_MASTER_Edit_Data(SrNO).resultObject;
                ViewBag.SrNo = SrNO;

                return View(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult DeActive_NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_Record(int SrNo)
        {
            var loginEmp = int.Parse(_userId.ToString());
            var rest = _Insp.DeActive_NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_Record(SrNo, loginEmp);
            return Json(rest);
        }

        #endregion

        #region GST Master

        public ActionResult NSP_GST_TAX_MASTER_List()
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                var Data = _Insp.GetNSP_GST_TAX_MASTER_List_Data().resultObject;

                return View("NSP_GST_MASTER_List", Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        public ActionResult NSP_GST_TAX_MASTER_Add()
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult NSP_GST_TAX_MASTER_SaveData(string Data, string Mode)
        {
            try
            {
                NSP_GST_TAX_MASTER obj = JsonConvert.DeserializeObject<NSP_GST_TAX_MASTER>(Data);
                var loginEmp = int.Parse(_userId.ToString());

                res = _Insp.NSP_GST_TAX_MASTER_SaveData(obj, loginEmp, Mode); JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                sJSON = JsonConvert.SerializeObject(res, settings);
                return Json(sJSON);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        public ActionResult NSP_GST_TAX_MASTER_Edit(int SrNO)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                var Data = _Insp.GetNSP_GST_TAX_MASTER_Edit_Data(SrNO).resultObject;
                ViewBag.SrNo = SrNO;

                return View(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult DeActive_NSP_GST_TAX_MASTER_Record(int SrNo)
        {
            var loginEmp = int.Parse(_userId.ToString());
            var rest = _Insp.DeActive_NSP_GST_TAX_MASTER_Record(SrNo, loginEmp);
            return Json(rest);
        }


        #endregion


        #region Master Log View
        public ActionResult ViewLog(int SrNO, string TableName)
        {
            try
            {
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                // END :: ADDED BY AUMENTO :: SR88023-CR5305
                var Data = _Insp.GetNSP_Master_Log_Data(SrNO, TableName).Result;
                ViewBag.DATA = Data;
                return View("ViewLog", Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }
        #endregion



        #endregion

        #region POst and Reverse


        [HttpGet]  // List Data
        public JsonResult GetDataForPostInvoices(string Status, string Mode, string FromDate, string ToDate)
        {
            try
            {
                var loginEmp = int.Parse(_userId.ToString());
                var Ilist = _Insp.GetDataForPostInvoices(Status, loginEmp, Mode, FromDate, ToDate);
                sJSON = JsonConvert.SerializeObject(Ilist);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw ex;
            }
        }

        [HttpGet] // Open Posting Window
        public ActionResult PostInvoiceWindow(int HeaderID, int DtID, string InvoceNo, string CostCenterValue, string Status, string Mode, string InvoiceDate, string GSTIN, int ReqRefNo)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.CostCenter = _Insp.GetCostCenterHelp();
            ViewBag.BusinessCenter = _Insp.GetBusinessCenterHelp();
            ViewBag.HeaderID = HeaderID;
            ViewBag.DtID = DtID;
            ViewBag.InvoceNo = InvoceNo;
            ViewBag.CostCenterValue = CostCenterValue;
            ViewBag.Status = Status;
            ViewBag.Mode = Mode;
            ViewBag.InvoiceDate = InvoiceDate;
            ViewBag.GSTIN = GSTIN;
            ViewBag.ReqRefNo = ReqRefNo;

            return View();
        }


        [HttpGet] // Get posting Data
        public JsonResult GetPostInvoiceData(int RequestNo, int DtId, string InvoiceNo, string DocType, string CostCenter, string BusinessCenter, string Status)
        {
            List<NSP_PaymentDetails> ilist = new List<NSP_PaymentDetails>();
            int loginEmp = (Convert.ToInt32(_userId.ToString()));

            ilist = _Insp.GetPostInvoiceData(RequestNo, InvoiceNo, DtId, DocType, loginEmp, CostCenter, BusinessCenter, Status);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }



        [HttpGet] // Open Reverse Posting Window
        public ActionResult ReversePostInvoiceWindow(int HeaderID, string POSTEDDOCNO, string InvoiceNo, string InvoiceDate, string GSTIN, int ReqRefNo)
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            ViewBag.HeaderID = HeaderID;
            ViewBag.POSTEDDOCNO = POSTEDDOCNO;
            ViewBag.InvoceNo = InvoiceNo;
            ViewBag.InvoiceDate = InvoiceDate;
            ViewBag.GSTIN = GSTIN;
            ViewBag.ReqRefNo = ReqRefNo;
            return View();
        }

        [HttpGet] // Get posting Reverse Data
        public JsonResult GetPostInvoiceDataReverse(int HeaderID, string POSTEDDOCNO, string InvoiceNo)
        {
            List<NSP_PaymentDetails> ilist = new List<NSP_PaymentDetails>();
            ilist = _Insp.GetPostInvoiceDataReverse(HeaderID, POSTEDDOCNO, InvoiceNo);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }


        [HttpGet] // Get posting Data
        public JsonResult GetPostInvoiceHistory(int RequestNo, string InvoiceNo)
        {
            List<NSP_PaymentDetails> ilist = new List<NSP_PaymentDetails>();
            int loginEmp = (Convert.ToInt32(_userId.ToString()));

            ilist = _Insp.GEtPostedInvoiceHistory(RequestNo, InvoiceNo);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }


        [HttpPost]
        public async Task<JsonResult> POSTInvoice([FromBody]POSTReverseRequest request)
        {
            try
            {
                var loginEmp = int.Parse(_userId.ToString());

                var res = await _Insp.PostAndReversePostInSap(request, loginEmp);

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                string sJSON = JsonConvert.SerializeObject(res, settings);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet] // Open Posting Window
        public ActionResult PostInvoiceHistory(int HeaderID, int DtID, string InvoceNo)
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            ViewBag.CostCenter = _Insp.GetCostCenterHelp();
            ViewBag.BusinessCenter = _Insp.GetBusinessCenterHelp();
            ViewBag.HeaderID = HeaderID;
            ViewBag.DtID = DtID;
            ViewBag.InvoceNo = InvoceNo;



            return View();
        }



        #endregion


    }
}

