using ClosedXML.Excel;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.Web.Models;
using ePortal.WebUI.Filters;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Text;



namespace HMSI.ePortal.Web.Controllers
{
    [CSPFilter] 
    public class AssetTransferController : Controller
    {
        //private System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        private string sJSON = String.Empty;
        LibResult res = new LibResult();
        private readonly IAT_ApprovalService _IATService;
        private readonly IAssetDisposalService _AssetService;
        private readonly IEportalESS RFC;

        private readonly ILogger<AssetTransferController> _logger;
        private readonly IConfiguration _settings;
        private readonly ISessionService _sessionService;
        public AssetTransferController(IAT_ApprovalService IATService, IAssetDisposalService AssetService, IEportalESS _RFC, ILogger<AssetTransferController> logger, IConfiguration settings, ISessionService sessionService)
        {
            _IATService = IATService;
            _AssetService = AssetService;
            RFC = _RFC;
            _logger = logger;
            _settings = settings;
            _sessionService = sessionService;
        }



        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        [ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "none" })]
        public async Task<List<AssetDetailViewModel>> GetAssetDetail()
        {
            ///------ SAP Connection Object -------////
            //EportalESS objasset = new EportalESS();
            List<AssetDetailViewModel> iList = new List<AssetDetailViewModel>();
            DataTable dt = new DataTable();
            //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            int _profitCenter = (int)_AssetService.GetSiteDetailBySiteId((long)_Employee_Details._SiteId).PROFIT_CENTER;
            long? operationMappId = _IATService.GetOperationMappId(_Employee_Details);
            string _fnCode = _AssetService.GetFnCodeByOperationId(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_Employee_Details._OpId);
            int _opCount = _AssetService.GetOperationCount(operationMappId != null && operationMappId > 0 ? (long)operationMappId : (long)_Employee_Details._OpId);
            dt = await RFC.GetAssetsDetails(_fnCode, (_opCount == 0 ? _profitCenter.ToString("D10") : null));
            if (dt.Rows.Count > 0)
            {
                List<SYSITE> _SiteList = _AssetService.GetSiteList();
                iList = (from DataRow row in dt.AsEnumerable()
                         select new AssetDetailViewModel
                         {
                             ASSETCODE = row["ANLN12"].ToString(),
                             //Change by aumento as on 02082023 for SR51823============================
                             ASSETCLASSNAME = row["TXK50"].ToString(),
                             //========================================================================
                             ASSETDESCRIPTION = row["TXT50"].ToString(),
                             ASSETDETAIL = row["ANLHTXT"].ToString(),
                             VENDORCODE = Convert.ToString(row["LIFNR"]),
                             VENDORNAME = Convert.ToString(row["LIEFE"]),
                             INVOICENO = Convert.ToString(row["XBLNR"]),
                             INVOICEDATE = Convert.ToString(row["BLDAT"]) == "0000-00-00" ? "" : Convert.ToDateTime(Convert.ToString(row["BLDAT"])).ToString("yyyy-MM-dd"),
                             ORIGINALCOST = Convert.ToDecimal(Convert.ToString(row["KANSW"]) == "" ? 0 : row["KANSW"]),
                             DEPRECIATIONCOST = Convert.ToDecimal(Convert.ToString(row["KNAFA"]) == "" ? 0 : row["KNAFA"]),
                             WDV = Convert.ToDecimal(Convert.ToString(row["KANSW"]) == "" ? 0 : row["KANSW"]) - Convert.ToDecimal(Convert.ToString(row["KNAFA"]) == "" ? 0 : row["KNAFA"]),
                             SERIALNUMBER = row["SERNR"].ToString(),
                             PONUMBER = row["TYPBZ"].ToString(),
                             PROFITCENTER = row["PRCTR"].ToString(),
                             //Change by aumento as on 02082023 for SR51823============================
                             COSTCENTER = row["KOSTL"].ToString(),
                             //========================================================================
                             PLANTID = Convert.ToInt64(_SiteList.Where(w => w.PROFIT_CENTER == Convert.ToInt16(row["PRCTR"].ToString() == "0000001100" ? "0000001000" : row["PRCTR"].ToString())).Select(s => s.SYSITEID).FirstOrDefault()),
                             PLANTNAME = _SiteList.Where(w => w.PROFIT_CENTER == Convert.ToInt16(row["PRCTR"].ToString() == "0000001100" ? "0000001000" : row["PRCTR"].ToString())).Select(s => s.DESCRIP).FirstOrDefault(),
                             CAPTALIZED_DATE = Convert.ToDateTime(row["AKTIV"].ToString()),
                         }).OrderBy(o => o.INVOICEDATE).ToList();
            }
            return iList.Where(x => x.PLANTID > 0).ToList();
        }

        public async Task<ActionResult> ViewCapitalizedAssetReport()
        {
            try
            {
                AssetDisposalViewModel ADVM_Model = new AssetDisposalViewModel();
                ADVM_Model.SapAssetList = await GetAssetDetail();
                return View("ViewCapitalizedAssetReport", ADVM_Model.SapAssetList);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        [HttpPost]
        //[ValidateInput(false)]
        public FileResult ExportAssetItems(string GridHtml)
        {
            try
            {
                return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "AssetItems.xls");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        #region Capitalize asset



        #region Approval Type Operation Mapping 

        public ActionResult ATOMList()
        {
            return View("ATOMList");
        }

        public ActionResult ATOMAdd()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ATOMSave(string ApproveType, int Operation, int SYKIID, int Division, int EvpAuth)
        {
            try
            {
                //int LoginCode = int.Parse(Session["UserId"].ToString());
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                res = _IATService.ATOMAdd(ApproveType, Operation, SYKIID, Division, EvpAuth, LoginCode);
                sJSON = JsonConvert.SerializeObject(res);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public JsonResult GetSykilist()
        {
            res.resultObject = _IATService.GetSykilist();
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }

        public JsonResult GetSykilistforatom()
        {
            res.resultObject = _IATService.GetSykilistforatom();
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }

        public JsonResult GetOperationlist()
        {
            res.resultObject = _IATService.GetOperationlist();
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }


        public JsonResult GetDivisionList(long Operation)
        {
            res.resultObject = _IATService.GetDivisionList(Operation);
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }

        public JsonResult GetEVPUser(long div_id)
        {
            res.resultObject = _IATService.GetEVPUser(div_id);
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }

        [HttpGet]
        public JsonResult ATOMGetDataList()
        {
            List<AT_APPROVAL_OPERATION_MAPPING> ilist = new List<AT_APPROVAL_OPERATION_MAPPING>();
            ilist = _IATService.ATOMGetDataList();
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        [HttpPost]
        public JsonResult ATOMDelete(int id)
        {
            bool rest = _IATService.ATOMDelete(id);
            return Json(rest);
        }

        [HttpPost]
        public JsonResult ATOMSearch(string ApprovalType, int Operation, int Syki)
        {
            var ilist = _IATService.ATOMSearch(ApprovalType, Operation, Syki);
            sJSON = JsonConvert.SerializeObject(ilist.resultObject);
            return Json(sJSON);
        }

        public ActionResult ATOMEdit(int id)
        {
            try
            {
                AT_APPROVAL_OPERATION_MAPPING PHVM = _IATService.ATOMEdit(id);

                return View(PHVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }
        #endregion

        #region Capitalized Asset Transfer

        public string Department(string term)
        {
            TempData["Department"] = term;

            return term;
        }

        [HttpGet]
        public JsonResult Requestor_Detail(string AssetType, string Transferor)
        {

            try
            {
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                res = _IATService.Requestor_Detail(AssetType, Transferor, LoginCode);
                sJSON = JsonConvert.SerializeObject(res.resultObject);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetTransferor(string AssetType)
        {

            try
            {
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                res = _IATService.GetTransferor(AssetType, LoginCode);
                sJSON = JsonConvert.SerializeObject(res.resultObject);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadFile(IFormFile file)
        {
            // Handle the uploaded file
            if (file != null && file.Length > 0)
            {
                // Process the file
                // For example, save it to disk, store it in a database, etc.
                var fileName = Path.GetFileName(file.FileName);
                //var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                var path = Path.Combine(serverpath.getFileUploadPath(), "App_Data", "uploads", fileName);
                //file.SaveAs(path);
                string directoryPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Json(new { success = true, fileName = fileName });
            }
            else
            {
                return Json(new { success = false, error = "No file uploaded" });
            }
        }


        public string AutocompleteSuggestions(string term)
        {
            string department = "";

            try
            {
                TempData.Keep();
                //department = TempData["Department"].ToString();
                if (department == "-Select-")
                    department = "";
            }
            catch (Exception ex) {
                _logger.LogError(ex.ToString());
            }

            List<Employee_Details> portaluser = _IATService.AutocompleteSuggestions(term, department);
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
        public ActionResult CapitalizeList()
        {
            return View("CapitalizeList");
        }

        [HttpGet]
        public JsonResult GeCapitalizeList(string flag)
        {
            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            ilist = _IATService.GetCapitalizeList(LoginCode, flag);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        [HttpPost]
        public JsonResult CapitalizedEditDelete(int id)
        {
            bool rest = _IATService.CapitalizedEditDelete(id);
            return Json(rest);
        }
        [HttpPost]
        public JsonResult CapitalizeListSearch(string TranjectionType, /*string AssetType,*/ int TranjectionNo, string TransfereeUser, string Status, string flag)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();
            ilist = _IATService.CapitalizeListSearch(TranjectionType,/* AssetType,*/ TranjectionNo, TransfereeUser, Status, LoginCode, flag);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);

        }

        [HttpPost]
        public JsonResult CapitalizeStatusSearch(string Status, string flag)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();
            ilist = _IATService.CapitalizeStatusSearch(Status, LoginCode, flag);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);

        }

        [HttpPost]
        public JsonResult CapitalizeListDelete(int id, string flag)
        {
            var data = _IATService.CapitalizeListDelete(id, flag);
            sJSON = JsonConvert.SerializeObject(data.resultObject);
            return Json(sJSON);
        }


        public ActionResult CapitalizeAdd()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<AT_AssetTransferViewModel> iList = new List<AT_AssetTransferViewModel>();
            if (TempData["APPROVAL_AUTH_LIST"] == null)
            {
                iList = new List<AT_AssetTransferViewModel>();
            }
            TempData["APPROVAL_AUTH_LIST"] = iList.ToList();
            TempData.Clear();
            return View();
        }

        public JsonResult AutocomplitTranNo()
        {
            var data = _IATService.AutocomplitTranNo();
            sJSON = JsonConvert.SerializeObject(data.resultObject);
            return Json(sJSON);
        }

        public JsonResult BindApprovalType()
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            var data = _IATService.BindApprovalType(LoginCode);
            sJSON = JsonConvert.SerializeObject(data.resultObject);
            return Json(sJSON);
        }

        public JsonResult GetDepartment()
        {
            res.resultObject = _IATService.GetDepartment();
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }

        [HttpPost]
        public async Task<JsonResult> SearchData(string AssetCode, int TranNo, string NewLocation, string SiteAddress)
        {
            List<AT_ASSET_TRANSFER_DETAIL> ilist = new List<AT_ASSET_TRANSFER_DETAIL>();
            LibResult res = new LibResult();

            Tuple<int, long, int, string> Tdata = _IATService.GetAssetCode(AssetCode);

            int count = Tdata.Item1; // count
            string requestCode = Tdata.Item2.ToString(); // request code
            string User = Tdata.Item3.ToString(); // request code
            string errorMsg = Tdata.Item4.ToString();



            //if (count > 0)
            //{
            //    return Json("NO");
            //}

            DataTable dtT = await RFC.GetAssetCodeData(AssetCode);

            res = _IATService.Maxsrno();
            var r = res.resultObject;
            var e = 0;
            e = int.Parse(r.ToString());
            e = e + 1;

            foreach (DataRow item in dtT.Rows)
            {
                try
                {
                    AT_ASSET_TRANSFER_DETAIL dt = new AT_ASSET_TRANSFER_DETAIL();
                    var NewNo = int.Parse(DateTime.Now.Second.ToString());
                    var NewNo2 = int.Parse(DateTime.Now.Minute.ToString());

                    dt.SRNO = e;
                    dt.TRAN_NO = TranNo;
                    dt.ASSET_CODE = item["ASSET_NO"].ToString();
                    dt.PO_NO = item["PO_NO"].ToString();
                    dt.PO_DATE = item["PO_DATE"].ToString() == "0000-00-00" ? DateTime.MinValue : DateTime.Parse(item["PO_DATE"].ToString());
                    dt.FULLY_PARTIAL = "--Select--";
                    dt.QTY = Decimal.Parse(item["QUANTITY"].ToString());
                    dt.CURRENT_LOCATION = item["CURRENT_LOCATION"].ToString();
                    if (SiteAddress == "")
                    {
                        dt.NEW_LOCATION = NewLocation;
                    }
                    else
                    {
                        dt.NEW_LOCATION = NewLocation + " - " + SiteAddress;
                    }
                    dt.ORIGINAL_COST = Decimal.Parse(item["ORIGINAL_COST"].ToString());
                    dt.DEPRECIATION = Decimal.Parse(item["ACC_DEPR"].ToString());
                    dt.NET_BLOCK = Decimal.Parse(item["NET_BLOCK"].ToString());
                    dt.INV_NO = item["INVOICE_NO"].ToString() == null ? "-" : item["INVOICE_NO"].ToString();
                    dt.INV_DATE = item["INVOICE_DATE"].ToString() == "0000-00-00" ? DateTime.MinValue : DateTime.Parse(item["INVOICE_DATE"].ToString());
                    dt.ASSET_SERIAL_NO = item["SR_NO"].ToString();
                    dt.ASSET_CLASS = Int32.Parse(item["ASSET_CLASS"].ToString());
                    dt.CAPITALIZED_DATE = item["CAP_DATE"].ToString() == "0000-00-00" ? DateTime.MinValue : DateTime.Parse(item["CAP_DATE"].ToString());
                    dt.VENDER_NAME = item["VENDOR_NAME"].ToString() == null ? "-" : item["VENDOR_NAME"].ToString();
                    dt.VENDOR_CODE = item["VENDOR_CODE"].ToString() == null ? "" : item["VENDOR_CODE"].ToString();
                    dt.ASSETMAIN_NO_TEXT = item["MAIN_NO_TEXT"].ToString();
                    dt.LICENSE_NO = "";
                    dt.LICENSE_DATE = DateTime.Now;
                    dt.HSN_CODE = 0;
                    dt.DESCRIPTION = item["ASSET_DESC"].ToString();
                    dt.ASSET_CLS_DESC = item["ASSET_CLS_DESC"].ToString();


                    ilist.Add(dt);

                    var data = new { ilist = ilist, count = count, requestCode = requestCode, User = User };

                    sJSON = JsonConvert.SerializeObject(data);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw ex;
                }
            }
            //TempData["AUTHORITY_LIST"] = ilist.ToList();
            TempData["AUTHORITY_LIST"] = JsonConvert.SerializeObject(ilist.ToList());
            return Json(sJSON);

        }

        [HttpPost]
        public JsonResult Summary(List<AT_ASSET_TRANSFER_DETAIL> Detail)
        {
            AT_ASSET_TRANSFER_DETAIL dt = new AT_ASSET_TRANSFER_DETAIL();
            try
            {
                //dt.ORIGINAL_COST = (from s in Detail select s.ORIGINAL_COST).Sum();
                //dt.DEPRECIATION = (from s in Detail select s.DEPRECIATION).Sum();
                //dt.NET_BLOCK = (from s in Detail select s.NET_BLOCK).Sum();


                var summedValues = new
                {
                    ORIGINAL_COST = Detail.Sum(s => s.ORIGINAL_COST),
                    DEPRECIATION = Detail.Sum(s => s.DEPRECIATION),
                    NET_BLOCK = Detail.Sum(s => s.NET_BLOCK),
                    Basic_price = Detail.Sum(s => s.BASIC_PRICE),
                    Invoice_Value = Detail.Sum(s => s.INVOICE_VALUE)
                };

                sJSON = JsonConvert.SerializeObject(summedValues);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                // Handle exceptions
                return Json(new { error = ex.Message });
            }
            return Json(sJSON);
        }

        [HttpPost]
        public JsonResult SaveTransaction()
        {
            try
            {
                string headerJson = Request.Form["Header"];
                string detailsJson = Request.Form["Details"];
                string authorityJson = Request.Form["Authority"];
                string flag = Request.Form["flag"];
                string assetType = Request.Form["AssetType"];

                AT_ASSSET_TRANSFER_HEADER headerData = JsonConvert.DeserializeObject<AT_ASSSET_TRANSFER_HEADER>(headerJson);
                List<AT_ASSET_TRANSFER_DETAIL> detailsData = JsonConvert.DeserializeObject<List<AT_ASSET_TRANSFER_DETAIL>>(detailsJson);
                List<AT_APPROVAL_AUTHORITY> authorityData = JsonConvert.DeserializeObject<List<AT_APPROVAL_AUTHORITY>>(authorityJson);


                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                res = _IATService.SaveTransaction(headerData, detailsData, authorityData, flag, assetType, LoginCode);

                sJSON = JsonConvert.SerializeObject(res);
                foreach (var detail in detailsData)
                {
                    //UploadFilesDetailCap("ASSET_UPLOAD", detail.TRAN_NO, detail.ASSET_CODE);
                    string Type = "ASSET_UPLOAD";
                    int TranNo = detail.TRAN_NO;
                    string AssetCode = detail.ASSET_CODE;
                    string SubAucCode = detail.SUBAUCCODE;
                    string retFname = string.Empty;
                    //bool isUpload = false;
                    try
                    {
                        if (Request.Form.Files.Count > 0)
                        {
                            long EmpCode = Int32.Parse(_sessionService.Get<string>("userID").ToString());
                            var files = Request.Form.Files;
                            for (int i = 0; i < files.Count; i++)
                            {
                                IFormFile file = files[i];
                                string fname;

                                //if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                if (Request.Headers["User-Agent"].Contains("MSIE") || Request.Headers["User-Agent"].Contains("Trident"))
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else
                                {
                                    fname = file.FileName;
                                }

                                String FileNM;
                                if (assetType == "Capitalized Asset")
                                {
                                    FileNM = Type + '_' + AssetCode + "_" + TranNo + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + fname;
                                }
                                else
                                {
                                    FileNM = Type + '_' + SubAucCode + "_" + TranNo + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + fname;
                                }

                                //fname = Path.Combine(Server.MapPath("~/Uploads/AT/DetailUpload/"), FileNM);
                                //file.SaveAs(fname);
                                var path = Path.Combine(serverpath.getFileUploadPath(), "AT", "DetailUpload", FileNM);

                                string directoryPath = Path.GetDirectoryName(path);
                                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }
                                using (var stream = new FileStream(path, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                        throw ex;
                    }
                }
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                // Handle exceptions
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult GetAuthEmpById(int EmpCode, int TranNo, string Department)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                Employee_Details _Login_Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                List<AT_AssetTransferAuthorityViewModel> AuthList = new List<AT_AssetTransferAuthorityViewModel>();

                Employee_Details employee_dtl = _IATService.GetAuthEmpById(EmpCode, _Login_Employee_Details, TranNo, Department);

                if (employee_dtl == null) { employee_dtl = new Employee_Details(); }
                //var AUTHORITY_LISTOTHER = _sessionService.Get<List<AT_AssetTransferAuthorityViewModel>>("AUTHORITY_LISTOTHER");
                if (TempData["AUTHORITY_LISTOTHER"] != null)
                {
                    //AuthList = (List<AT_AssetTransferAuthorityViewModel>)TempData["AUTHORITY_LISTOTHER"];
                    AuthList = JsonConvert.DeserializeObject<List<AT_AssetTransferAuthorityViewModel>>(TempData["AUTHORITY_LISTOTHER"]?.ToString());
                    //AuthList = AUTHORITY_LISTOTHER;
                }
                if (employee_dtl._ECode > 0 && !string.IsNullOrEmpty(employee_dtl._EName))
                {
                    if (AuthList.Count > 0)
                    {

                        AuthList.Add(new AT_AssetTransferAuthorityViewModel
                        {
                            TRAN_NO = TranNo,
                            ADEMPCODE = employee_dtl._ECode,
                            APPROVAL_STATUS = "-",
                            ADEMPNAME = employee_dtl._EName,
                            DEPARTMENT = Department,
                            ADDESIGNATION = employee_dtl._Desig
                        });


                    }
                    else
                    {
                        foreach (var i in AuthList)
                        {
                            if (i.DEPARTMENT == "Security")
                            {
                                i.APPROVAL_STATUS = "-";
                            }
                        }
                        AuthList.Add(new AT_AssetTransferAuthorityViewModel
                        {
                            TRAN_NO = TranNo,
                            ADEMPCODE = employee_dtl._ECode,
                            DEPARTMENT = Department,
                            APPROVAL_STATUS = "-",
                            ADEMPNAME = employee_dtl._EName,
                            ADDESIGNATION = employee_dtl._Desig
                        });
                    }
                }
                //TempData["AUTHORITY_LISTOTHER"] = AuthList.ToList();
                TempData["AUTHORITY_LISTOTHER"] = JsonConvert.SerializeObject(AuthList.ToList());
                //_sessionService.Set("AUTHORITY_LISTOTHER", AuthList.ToList());

                return Json(new
                {
                    ECODE = employee_dtl._ECode,
                    ENAME = employee_dtl._EName,
                    LISTOT = AuthList.ToList()

                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new
                {
                    ECODE = 0,
                    ENAME = "",
                    LISTOT = new List<AT_AssetTransferViewModel>()
                });
            }
        }

        public ActionResult DeleteAppAuthority(long EmpCode, string ISALL, string Department)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<AT_AssetTransferAuthorityViewModel> AuthList = new List<AT_AssetTransferAuthorityViewModel>();

                if (ISALL == "0")
                {
                    if (TempData["AUTHORITY_LISTOTHER"] != null)
                    {
                        //AuthList = _sessionService.Get<List<AT_AssetTransferAuthorityViewModel>>("AUTHORITY_LISTOTHER");
                        //AuthList = (List<AT_AssetTransferAuthorityViewModel>)TempData["AUTHORITY_LISTOTHER"];
                        AuthList = JsonConvert.DeserializeObject<List<AT_AssetTransferAuthorityViewModel>>(TempData["AUTHORITY_LISTOTHER"]?.ToString());
                    }
                    //var AUTHORITY_LISTOTHER = _sessionService.Get<List<AT_AssetTransferAuthorityViewModel>>("AUTHORITY_LISTOTHER");
                    //if (AUTHORITY_LISTOTHER != null)
                    //{
                    //    AuthList = AUTHORITY_LISTOTHER;
                    //}
                    if (AuthList.Count > 0)
                    {
                        AuthList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(EmpCode) && r.DEPARTMENT == Department);
                        //TempData["AUTHORITY_LISTOTHER"] = AuthList.ToList();
                        TempData["AUTHORITY_LISTOTHER"] = JsonConvert.SerializeObject(AuthList.ToList());
                        //_sessionService.Set("AUTHORITY_LISTOTHER", AuthList.ToList());
                        retval = 1;
                    }
                }
                else
                {
                    //TempData["AUTHORITY_LISTOTHER"] = AuthList;
                    TempData["AUTHORITY_LISTOTHER"] = JsonConvert.SerializeObject(AuthList.ToList());
                    TempData["Designation"] = "";
                    TempData.Keep();
                    //_sessionService.Set("AUTHORITY_LISTOTHER", AuthList.ToList());
                    retval = 1;
                }
                return Json(new { RESULT = retval, LISTOT = AuthList.ToList() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new { RESULT = -1 });
            }
        }




        #endregion

        #region Approval List
        public ActionResult Approval()
        {
            return View("List");
        }
        public ActionResult SecurityApproval()
        {
            return View("SecurityApprovalList");
        }

        [HttpGet]
        public JsonResult CapitalizedDetail(int id, int APPAUTHSRNO)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            var rest = _IATService.CapitalizedDetail(id, LoginCode, APPAUTHSRNO);
            sJSON = JsonConvert.SerializeObject(rest.resultObject);
            return Json(sJSON);
        }

        public ActionResult ApprovalListEdit(int id, int APPAUTHSRNO)
        {
            ViewBag.APPAUTHSRNO = APPAUTHSRNO;
            AT_ASSSET_TRANSFER_HEADER at = _IATService.Get_AT_ASSSET_TRANSFER_HEADER_ModelData(id);
            return View("CapitalizedAssetTransferEdit", at);
        }

        public ActionResult SecurityApprovalListEdit(int id, int APPAUTHSRNO)
        {
            ViewBag.APPAUTHSRNO = APPAUTHSRNO;
            AT_ASSSET_TRANSFER_HEADER at = _IATService.Get_AT_ASSSET_TRANSFER_HEADER_ModelData(id);
            return View("SecurityApprovalEdit", at);
        }

        [HttpPost]
        public JsonResult ApprovalListSearchDataStatus( string Status, string flag)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();
            ilist = _IATService.ApprovalListSearchDataStatus(Status, LoginCode, flag);
            ilist = ilist.Distinct().ToList();

            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        [HttpPost]
        public JsonResult ApprovalListSearchData(string TranjectionType, string AssetType, int TranjectionNo, string Status, string flag)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();
            ilist = _IATService.ApprovalListSearchData(TranjectionType, AssetType, TranjectionNo, Status, LoginCode, flag);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        [HttpGet]
        public JsonResult GetApprovalList()
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();
            ilist = _IATService.GetApprovalList(LoginCode);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        [HttpGet]
        public JsonResult SecurityGetApprovalList()
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();
            ilist = _IATService.SecurityGetApprovalList(LoginCode);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }


        #endregion

        #region Edit Capitalize

        public ActionResult CapitalizeEdit()
        {
            return View();
        }

        public ActionResult CapitalizedEdit(int id, int APPAUTHSRNO)
        {

            ViewBag.APPAUTHSRNO = APPAUTHSRNO;
            AT_ASSSET_TRANSFER_HEADER PHVM = _IATService.CapitalizedEdit(id);

            return View(PHVM);
        }

        [HttpPost]
        public JsonResult EditTransaction()
        {

            string headerJson = Request.Form["Header"];
            string detailsJson = Request.Form["Details"];
            string authorityJson = Request.Form["Authority"];
            string flag = Request.Form["flag"];
            string assetType = Request.Form["AssetType"];

            AT_ASSSET_TRANSFER_HEADER headerData = JsonConvert.DeserializeObject<AT_ASSSET_TRANSFER_HEADER>(headerJson);
            List<AT_ASSET_TRANSFER_DETAIL> detailsData = JsonConvert.DeserializeObject<List<AT_ASSET_TRANSFER_DETAIL>>(detailsJson);
            List<AT_APPROVAL_AUTHORITY> authorityData = JsonConvert.DeserializeObject<List<AT_APPROVAL_AUTHORITY>>(authorityJson);

            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            res = _IATService.EditTransaction(headerData, detailsData, authorityData, flag, LoginCode, assetType);
            sJSON = JsonConvert.SerializeObject(res);
            foreach (var detail in detailsData)
            {
                //UploadFilesDetailCap("ASSET_UPLOAD", detail.TRAN_NO, detail.ASSET_CODE);
                string Type = "ASSET_UPLOAD";
                int TranNo = detail.TRAN_NO;
                string AssetCode = detail.ASSET_CODE;
                string SubAucCode = detail.SUBAUCCODE;
                string retFname = string.Empty;
                //bool isUpload = false;
                try
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        long EmpCode = Int32.Parse(_sessionService.Get<string>("userID").ToString());
                        var files = Request.Form.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            IFormFile file = files[i];
                            string fname;

                            //if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            if (Request.Headers["User-Agent"].Contains("MSIE") || Request.Headers["User-Agent"].Contains("Trident"))
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else
                            {
                                fname = file.FileName;
                            }
                            String FileNM;
                            if (assetType == "Capitalized Asset")
                            {
                                FileNM = Type + '_' + AssetCode + "_" + TranNo + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + fname;
                            }
                            else
                            {
                                FileNM = Type + '_' + SubAucCode + "_" + TranNo + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + fname;
                            }


                            //fname = Path.Combine(Server.MapPath("~/Uploads/AT/DetailUpload/"), FileNM);
                            //file.SaveAs(fname);
                            var path = Path.Combine(serverpath.getFileUploadPath(), "AT", "DetailUpload", FileNM);

                            string directoryPath = Path.GetDirectoryName(path);
                            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return Json(new { error = ex.Message });
                }
            }
            return Json(sJSON);

        }

        [HttpPost]
        public ActionResult UpdateApproval(string Remarks, int TranNo, string Status, AssetTransferTrasactionData at)
        {
            short retVal = 0;
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            retVal = _IATService.UpdateApproval(Remarks, LoginCode, TranNo, Status, at.Details);
            return Json(retVal);
        }

        public ActionResult CapitalizedAssetTransferEdit(int id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AT_AssetTransferHeaderViewModel at = _IATService.CapitalizedAssetTransferEdit(id);

            return View(at);
        }

        [HttpGet]
        public JsonResult CapitalizedAuthority(int id)
        {

            try
            {
                List<AT_AssetTransferAuthorityViewModel> getauthority = _IATService.CapitalizedAuthority(id);
                //TempData["AUTHORITY_LISTOTHER"] = getauthority.ToList();
                TempData["AUTHORITY_LISTOTHER"] = JsonConvert.SerializeObject(getauthority.ToList());
                //_sessionService.Set("AUTHORITY_LISTOTHER", getauthority.ToList());

                return Json(new
                {
                    LISTOT = getauthority.ToList()

                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new
                {

                    LISTOT = new List<AT_AssetTransferAuthorityViewModel>()
                });
            }
        }


        #endregion

        #region
        [HttpGet]
        public ActionResult Gethead(int TranNo, string TranType, int Transfree, string ApprovalType)
        {
            try
            {
                short LoginCode = short.Parse(_sessionService.Get<string>("userID").ToString());

                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                long? operationMappId = _IATService.GetOperationMappId(_Employee_Details);

                List<AT_AssetTransferAuthorityViewModel> getauthority = _IATService.Gethead(LoginCode, TranNo, TranType, Transfree, ApprovalType, operationMappId);
                TempData["AUTHORITY_LIST"] = JsonConvert.SerializeObject(getauthority.ToList());

                //TempData["AUTHORITY_LIST"] = getauthority.ToList();

                return Ok(new
                {
                    LIST = getauthority.ToList()

                });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Ok(new
                {
                    LIST = new List<AT_AssetTransferViewModel>()
                });
            }
        }

        [HttpPost]
        public ActionResult OpenPdf(string pdfPath)
        {
            try
            {
                //string _Path = Server.MapPath("~/Uploads/AT/AT PDF/");
                string _Path = Path.Combine(serverpath.getFileUploadPath(), "Uploads", "AT", "AT PDF");

                //string file_path = "../../../Uploads/AT/AT PDF/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, _Path + pdfPath);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new
                {
                    FILEPATH = "",
                });
            }

        }

        [HttpPost]
        public ActionResult OpenPdfNew(string pdfPath, int TranNo, string AssetType)
        {
            try
            {
                //string _Path = Server.MapPath("~/Uploads/AT/AT PDF/");
                string _Path = Path.Combine(serverpath.getFileUploadPath(), "AT", "AT PDF");

                if (!System.IO.File.Exists(_Path + pdfPath))
                {
                    GeneratePdfReport(Convert.ToInt32(TranNo), AssetType);
                }



                //string file_path = "../../../Uploads/AT/AT PDF/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, _Path + pdfPath);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new
                {
                    FILEPATH = "",
                });
            }

        }

        [HttpPost]
        public ActionResult OpenPdfGETOUT(string pdfPath)
        {
            try
            {
                string file_path = "../../../Uploads/AT/UploadFile/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, file_path + pdfPath);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new
                {
                    FILEPATH = "",
                });
            }

        }

        #endregion

        #region  Taxation list

        [HttpGet]
        public JsonResult GetTaxationApprovalList()
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();
            ilist = _IATService.GetTaxationApprovalList(LoginCode);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }
        public ActionResult Taxationapprove()
        {
            return View("Taxationapprove");
        }

        public ActionResult TaxationApprovalEdit(int id, int APPAUTHSRNO)
        {
            ViewBag.APPAUTHSRNO = APPAUTHSRNO;
            AT_ASSSET_TRANSFER_HEADER at = _IATService.Get_AT_ASSSET_TRANSFER_HEADER_ModelData(id);

            return View("TaxationApproveEdit", at);
        }

        #endregion


        #region  Finance list

        [HttpGet]
        public JsonResult GetFinanceApprovalList()
        {
            try
            {
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();
                ilist = _IATService.GetFinanceApprovalList(LoginCode);
                sJSON = JsonConvert.SerializeObject(ilist);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetFinanceApproveList()
        {
            try
            {
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();
                ilist = _IATService.GetFinanceApproveList(LoginCode);
                sJSON = JsonConvert.SerializeObject(ilist);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetFinalFinanceApprovalList(string flag)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();
            ilist = _IATService.GetFinalFinanceApprovalList(LoginCode, flag);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }
        public ActionResult Financeapprove()
        {
            return View("Financeapprove");
        }

        public ActionResult FinanceApproval()
        {
            return View("FinanceApprovalList");
        }

        public ActionResult FinanceFinalList()
        {
            return View("FinanceFinalList");
        }

        public ActionResult GateInStampList()
        {
            return View("GateInStampList");
        }

        [HttpPost]
        public ActionResult UpdateFinanceApproval(string Remarks, int TranNo, string Status, string AssetType)
        {
            try
            {
                short retVal = 0;
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                retVal = _IATService.UpdateFinanceApproval(Remarks, LoginCode, TranNo, Status);
                if (retVal == 2)
                {
                    GeneratePdfReport(TranNo, AssetType);
                }
                return Json(retVal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;

            }
        }

        public ActionResult FinancenApprovalEdit(int id, int APPAUTHSRNO)
        {
            ViewBag.APPAUTHSRNO = APPAUTHSRNO;
            AT_ASSSET_TRANSFER_HEADER at = _IATService.Get_AT_ASSSET_TRANSFER_HEADER_ModelData(id);

            return View("FinanceApproveEdit", at);
        }

        public ActionResult FinancenApproveEdit(int id, int APPAUTHSRNO)
        {
            ViewBag.APPAUTHSRNO = APPAUTHSRNO;
            AT_ASSSET_TRANSFER_HEADER at = _IATService.Get_AT_ASSSET_TRANSFER_HEADER_ModelData(id);

            return View("FinanceApprovalEdit", at);
        }

        public ActionResult FinancenFinalApprovalEdit(int id, int APPAUTHSRNO)
        {
            ViewBag.APPAUTHSRNO = APPAUTHSRNO;
            AT_ASSSET_TRANSFER_HEADER at = _IATService.Get_AT_ASSSET_TRANSFER_HEADER_ModelData(id);

            return View("FinanceFinalEdit", at);
        }

        #endregion

        #region Upload
        public ActionResult UploadFiles(string Type, int TranNo)
        {
            string retFname = string.Empty;
            if (Request.Form.Files.Count > 0)
            {
                long EmpCode = Int32.Parse(_sessionService.Get<string>("userID").ToString());

                try
                {
                    var files = Request.Form.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        IFormFile file = files[i];
                        string fname;

                        //if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        if (Request.Headers["User-Agent"].Contains("MSIE") || Request.Headers["User-Agent"].Contains("Trident"))
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        String FileNM = Type + '_' + EmpCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fname);

                        //fname = Path.Combine(Server.MapPath("~/Uploads/AT/UploadFile/"), FileNM);
                        res = _IATService.UploadFile(TranNo, FileNM, EmpCode);
                        //file.SaveAs(fname);
                        var path = Path.Combine(serverpath.getFileUploadPath(),  "AT", "UploadFile", FileNM);

                        string directoryPath = Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        retFname = FileNM;
                    }
                    return Json(retFname);
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        public ActionResult GetAppAuthLogHistory(int TranNo)
        {
            List<AT_APPROVAL_AUTHORITY_LOG> ilist = new List<AT_APPROVAL_AUTHORITY_LOG>();
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            ilist = _IATService.History(LoginCode, TranNo);
            return View(ilist);

        }

        [HttpGet]
        public JsonResult History(int TranNo)
        {
            List<AT_APPROVAL_AUTHORITY_LOG> ilist = new List<AT_APPROVAL_AUTHORITY_LOG>();
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            ilist = _IATService.History(LoginCode, TranNo);
            ilist.ForEach(x => x.REMARKS = string.IsNullOrEmpty(x.REMARKS) ? " " : x.REMARKS);
             sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }
        public ActionResult GetinstampUpload(string Type, int TranNo)
        {
            string retFname = string.Empty;
            if (Request.Form.Files.Count > 0)
            {
                long EmpCode = Int32.Parse(_sessionService.Get<string>("userID").ToString());

                try
                {
                    var files = Request.Form.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        IFormFile file = files[i];
                        string fname;

                        //if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        if (Request.Headers["User-Agent"].Contains("MSIE") || Request.Headers["User-Agent"].Contains("Trident"))
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        String FileNM = Type + '_' + EmpCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fname);

                        //fname = Path.Combine(Server.MapPath("~/Uploads/AT/GetinStamp/"), FileNM);
                        res = _IATService.GetinstampUpload(TranNo, FileNM, EmpCode);
                        var path = Path.Combine(serverpath.getFileUploadPath(), "AT", "GetinStamp", FileNM);

                        string directoryPath = Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        //file.SaveAs(fname);
                        retFname = FileNM;
                    }
                    return Json(retFname);
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        #endregion

        #region NewLocation

        [HttpGet]
        public JsonResult GetNewLocation(string term, string TransfreeType)
        {
            string sJSON = string.Empty;
            LibResult res = new LibResult();
            try
            {
                res = _IATService.GetNewLocation(term, TransfreeType);
                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                sJSON = JsonConvert.SerializeObject(res.resultObject);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                _logger.LogError(ex.ToString());
                Console.WriteLine("An error occurred: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
            }
            return Json(sJSON);

        }


        #endregion

        #region Comman Approval Master

        public JsonResult AutocomplitName(int Ecode)
        {
            var data = _IATService.AutocomplitName(Ecode);
            sJSON = JsonConvert.SerializeObject(data.resultObject);
            return Json(sJSON);
        }

        public ActionResult CommanApprovalList()
        {
            try
            {
                return View("AT_CommanApprovalList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }


        public ActionResult AT_CommanApprovalAdd()
        {
            return View();
        }

        public ActionResult AT_CommanApprovalEdit(int id)
        {
            try
            {
                AT_COMMON_APPROVAL_MASTER PHVM = _IATService.AT_CommanApprovalEdit(id);

                return View(PHVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        public JsonResult GetCommanApprovalEdit(int srno, decimal Sysiteid, string Department, int ECode, decimal Sequenceno)
        {
            try
            {
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                res = _IATService.GetCommanApprovalEdit(srno, Sysiteid, Department, ECode, Sequenceno, LoginCode);
                sJSON = JsonConvert.SerializeObject(res);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public JsonResult ATOMUpdate(int srno, string ApproveType, int Operation, int SYKIID, int Division, int EvpAuth)
        {
            try
            {
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                res = _IATService.ATOMUpdate(srno, ApproveType, Operation, SYKIID, Division, EvpAuth, LoginCode);
                sJSON = JsonConvert.SerializeObject(res);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetCommanApprovalSave(int srno, int Sysiteid, string Department, int ECode, decimal Sequenceno, string Mode)
        {
            try
            {
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                res = _IATService.GetCommanApprovalSave(srno, Sysiteid, Department, ECode, Sequenceno, LoginCode, Mode);
                sJSON = JsonConvert.SerializeObject(res);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetCommanApprovalSearch([FromBody]string Department)
        {
            try
            {
                List<AT_COMMON_APPROVAL_MASTER> ilist = new List<AT_COMMON_APPROVAL_MASTER>();
                ilist = _IATService.GetCommanApprovalSearch(Department);
                sJSON = JsonConvert.SerializeObject(ilist);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

        }


        [HttpGet]
        public JsonResult GetCommanApprovalList()
        {
            try
            {
                List<AT_COMMON_APPROVAL_MASTER> ilist = new List<AT_COMMON_APPROVAL_MASTER>();
                ilist = _IATService.GetCommanApprovalList();
                sJSON = JsonConvert.SerializeObject(ilist);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }


        public JsonResult BindPlantName()
        {
            res.resultObject = _IATService.BindPlantName();
            sJSON = JsonConvert.SerializeObject(res.resultObject);
            return Json(sJSON);
        }

        [HttpPost]
        public JsonResult CommanApprovalDelete(int id)
        {
            try
            {
                bool rest = _IATService.CommanApprovalDelete(id);
                return Json(rest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        #endregion

        #region Asset Attachment

        public JsonResult UploadFilesDetail(string Type, int TranNo, string AssetCode)
        {
            string retFname = string.Empty;
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    long EmpCode = Int32.Parse(_sessionService.Get<string>("userID").ToString());


                    var files = Request.Form.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        IFormFile file = files[i];
                        string fname;

                        //if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        if (Request.Headers["User-Agent"].Contains("MSIE") || Request.Headers["User-Agent"].Contains("Trident"))
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        String FileNM = Type + '_' + AssetCode + "_" + TranNo + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fname);

                        //fname = Path.Combine(Server.MapPath("~/Uploads/AT/DetailUpload/"), FileNM);
                        //res = _IATService.UploadFile(TranNo, fname, EmpCode);
                        //file.SaveAs(fname);
                        var path = Path.Combine(serverpath.getFileUploadPath(), "AT", "DetailUpload", FileNM);

                        string directoryPath = Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        retFname = FileNM;

                        bool isUpload = _IATService.UploadFileAndSave(TranNo, AssetCode, retFname);

                    }
                    return Json(retFname);

                }
                else
                {
                    return Json("No files selected.");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json("Error occurred. Error details: " + ex.Message);
            }

        }

        public void UploadFilesDetailCap(string Type, int TranNo, string AssetCode)
        {
            string retFname = string.Empty;
            //bool isUpload = false;
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    long EmpCode = Int32.Parse(_sessionService.Get<string>("userID").ToString());


                    var files = Request.Form.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        IFormFile file = files[i];
                        string fname;

                        //if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        if (Request.Headers["User-Agent"].Contains("MSIE") || Request.Headers["User-Agent"].Contains("Trident"))
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        String FileNM = Type + '_' + AssetCode + "_" + TranNo + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fname);

                        //fname = Path.Combine(Server.MapPath("~/Uploads/AT/DetailUpload/"), FileNM);
                        //res = _IATService.UploadFile(TranNo, fname, EmpCode);
                        //file.SaveAs(fname);
                        var path = Path.Combine(serverpath.getFileUploadPath(), "AT", "DetailUpload", FileNM);

                        string directoryPath = Path.GetDirectoryName(path);
                        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        //retFname = FileNM;

                        //isUpload = _IATService.UploadFileAndSaveCap(TranNo, AssetCode, retFname);
                    }


                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult OpenPdfdt(string pdfPath)
        {
            try
            {
                string file_path = "../../../Uploads/AT/DetailUpload/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, file_path + pdfPath);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new
                {
                    FILEPATH = "",
                });
            }

        }

        [HttpPost]
        public ActionResult OpenOther(string pdfPath)
        {
            try
            {
                string file_path = "../../../Uploads/AT/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, file_path + pdfPath);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new
                {
                    FILEPATH = "",
                });
            }

        }

        [HttpPost]
        public ActionResult GetinDoc(string pdfPath)
        {
            try
            {
                string file_path = "../../../Uploads/AT/GetinDoc/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, file_path + pdfPath);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new
                {
                    FILEPATH = "",
                });
            }

        }

        #endregion
        #endregion

        #region Non capitalize Asset

        #region RFC Data

        [HttpPost]
        public async Task<JsonResult> SearchDataNC(string AucCode, int TranNo, string NewLocation, string SiteAddress, string AssetType)
        {
            List<AT_ASSET_TRANSFER_DETAIL> ilist = new List<AT_ASSET_TRANSFER_DETAIL>();
            LibResult res = new LibResult();
            //res = _IATService.GetAssetCode(AucCode);

            Tuple<int, long, int, string> Tdata = _IATService.GetAssetCode(AucCode);

            int count = Tdata.Item1; // count
            string requestCode = Tdata.Item2.ToString(); // request code
            string User = Tdata.Item3.ToString(); // request code
            string errorMsg = Tdata.Item4.ToString();

            //if (count > 0)
            //{
            //    return Json("NO");
            //}

            DataTable dtT = await RFC.GetAucCodeData(AucCode);
            res = _IATService.Maxsrno();
            var r = res.resultObject;
            var e = 0;
            //e = int.Parse(r.ToString());
            e = e + 1;
            foreach (DataRow item in dtT.Rows)
            {

                try
                {

                    AT_ASSET_TRANSFER_DETAIL dt = new AT_ASSET_TRANSFER_DETAIL();
                    var NewNo = int.Parse(DateTime.Now.Second.ToString());
                    var NewNo2 = int.Parse(DateTime.Now.Minute.ToString());

                    //dt.SRNO = e;

                    dt.TRAN_NO = TranNo;
                    dt.ASSET_CODE = item["SAP_ASSET_NO"].ToString();
                    dt.SUBAUCCODE = item["SAP_ASSET_NO"].ToString();
                    dt.PO_NO = item["PURCHASE_ORDER_NO"].ToString() == "" ? " " : item["PURCHASE_ORDER_NO"].ToString();
                    dt.PO_DATE = item["PURCHASE_ORDER_DATE"].ToString() == "0000-00-00" ? DateTime.MinValue : DateTime.Parse(item["PURCHASE_ORDER_DATE"].ToString());
                    dt.FULLY_PARTIAL = "--Select--";
                    dt.QTY = item["QUANTITY"].ToString() == "" ? 0 : Decimal.Parse(item["QUANTITY"].ToString());
                    dt.CURRENT_LOCATION = item["CURRENT_LOCATION"].ToString() == "" ? " " : item["CURRENT_LOCATION"].ToString();
                    if (SiteAddress == "")
                    {
                        dt.NEW_LOCATION = NewLocation;
                    }
                    else
                    {
                        dt.NEW_LOCATION = NewLocation + " - " + SiteAddress;
                    }

                    dt.ORIGINAL_COST = 0;
                    dt.DEPRECIATION = 0;//item["ACC_DEPR"].ToString()=="" ? 0 : Decimal.Parse(item["ACC_DEPR"].ToString());
                    dt.NET_BLOCK = 0; //item["NET_BLOCK"].ToString()=="" ? 0 : Decimal.Parse(item["NET_BLOCK"].ToString());
                    dt.INV_NO = item["INVOICE_NO"].ToString() == null || item["INVOICE_NO"].ToString() == "" ? " " : item["INVOICE_NO"].ToString();
                    dt.INV_DATE = item["INVOICE_DATE"].ToString() == "" ? DateTime.MinValue : DateTime.Parse(item["INVOICE_DATE"].ToString());
                    dt.ASSET_SERIAL_NO = " ";//item["SR_NO"].ToString()=="" ? "" : item["SR_NO"].ToString();
                    dt.ASSET_CLASS = item["ASSET_CLASS"].ToString() == "" ? 0 : Int32.Parse(item["ASSET_CLASS"].ToString());
                    dt.CAPITALIZED_DATE = DateTime.MinValue; //item["CAP_DATE"].ToString() == "" ? DateTime.MinValue : DateTime.Parse(item["CAP_DATE"].ToString());
                    dt.VENDER_NAME = item["VENDOR_NAME"].ToString() == null ? " " : item["VENDOR_NAME"].ToString();
                    dt.VENDOR_CODE = item["VENDOR_CODE"].ToString() == null ? "" : item["VENDOR_CODE"].ToString();
                    dt.ASSETMAIN_NO_TEXT = " ";//item["MAIN_NO_TEXT"].ToString()=="" ? "" : item["MAIN_NO_TEXT"].ToString();
                    dt.LICENSE_NO = " ";
                    dt.LICENSE_DATE = DateTime.Now;
                    dt.HSN_CODE = 0;
                    dt.DESCRIPTION = item["DESCRIPTION"].ToString() == "" ? " " : item["DESCRIPTION"].ToString();
                    dt.BASIC_PRICE = item["BASIC_PRICE"].ToString() == "" ? 0 : Decimal.Parse(item["BASIC_PRICE"].ToString());
                    dt.INVOICE_VALUE = item["INVOICE_VALUE"].ToString() == "" ? 0 : Decimal.Parse(item["INVOICE_VALUE"].ToString());
                    dt.ASSET_CLS_DESC = item["ASSET_CLASS_DESCRIPTION"].ToString();
                    e++;

                    ilist.Add(dt);

                    var data = new { ilist = ilist, count = count, requestCode = requestCode, User = User };

                    sJSON = JsonConvert.SerializeObject(data);
                    //sJSON = JsonConvert.SerializeObject(ilist);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw ex;
                }
            }
            //TempData["AUTHORITY_LIST"] = ilist.ToList();
            TempData["AUTHORITY_LIST"] = JsonConvert.SerializeObject(ilist.ToList());

            return Json(sJSON);

        }

        #endregion

        #region NonCapitalizelist

        public ActionResult NonCapitalizeList()
        {
            return View("NonCapitalizeList");
        }


        public ActionResult NonCapitalizeAdd()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<AT_AssetTransferViewModel> iList = new List<AT_AssetTransferViewModel>();
            if (TempData["APPROVAL_AUTH_LIST"] == null)
            {
                iList = new List<AT_AssetTransferViewModel>();
            }
            TempData["APPROVAL_AUTH_LIST"] = iList.ToList();
            TempData.Clear();
            return View();
        }

        public ActionResult NonCapitalizeEdit(int id, int APPAUTHSRNO)
        {

            ViewBag.APPAUTHSRNO = APPAUTHSRNO;
            AT_ASSSET_TRANSFER_HEADER PHVM = _IATService.NonCapitalizedEdit(id);

            return View(PHVM);
        }
        #endregion

        #endregion


        public ActionResult GeneratePdfReport(int Tran, string AssetType)
        {
            //string logFilePath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/AT/errorLog.txt");
            string logFilePath = Path.Combine(serverpath.getFileUploadPath(), "AT", "errorLog.txt");
            //StreamWriter swMailLog = null;
            try
            {
                //if (System.IO.File.Exists(logFilePath))
                //{
                //    swMailLog = new StreamWriter(logFilePath, true);
                //}
                //else
                //{
                //    swMailLog = System.IO.File.CreateText(logFilePath);
                //}


                //var Tran = 1;
                //var AssetType = "Capitalized Asset";
                AssetTransferReport assetregisterReport = new AssetTransferReport();
                AssetTransferReportNon assetregisterReportNon = new AssetTransferReportNon();
                string filename = Tran + "_" + "AssetTransferForm" + ".pdf";
                byte[] abytes;
                if (AssetType == "Capitalized Asset")
                {
                    abytes = assetregisterReport.PrepareReport(GetAssetRegisterDetail(Tran, filename));
                }
                else
                {
                    abytes = assetregisterReportNon.PrepareReport(GetAssetRegisterDetailNon(Tran, filename));
                }

                //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //string _Path = Server.MapPath("~/Uploads/AT/AT PDF/");
                string _Path = Path.Combine(serverpath.getFileUploadPath(), "AT", "AT PDF");
                //string relativePath = "Uploads/AT/AT PDF/";
                //string fullPath = Path.Combine(baseDirectory, relativePath);
                //string pdfPath = Path.Combine(fullPath, filename);
                string pdfPath = Path.Combine(_Path, filename);
                System.IO.File.WriteAllBytes(pdfPath, abytes);
                //OpenPDF(pdfPath);
                //return File(abytes, "application/pdf", filename);
                using (var reader = new PdfReader(pdfPath))

                using (var outputStream = new FileStream(pdfPath.Replace(".pdf", "_File.pdf"), FileMode.Create))
                using (var pdfStamper = new PdfStamper(reader, outputStream))
                {
                    var pageSize = reader.GetPageSize(1);
                    string relativePathimg = "logoex.png";
                    //string relativePathimg = "assets/images/logoex.png";
                    //string fullPathimg = Path.Combine(baseDirectory, relativePathimg);
                    string fullPathimg = Path.Combine(_Path, relativePathimg);
                    var image = Image.GetInstance(fullPathimg);
                    image.ScaleToFit(50, 50);

                    var page = pdfStamper.GetOverContent(1);
                    image.SetAbsolutePosition(pageSize.Width - image.ScaledWidth - 10, pageSize.Height - image.ScaledHeight - 10);
                    page.AddImage(image);
                }

                abytes = System.IO.File.ReadAllBytes(pdfPath.Replace(".pdf", "_File.pdf"));
                var filereplace = filename.Replace(".pdf", "_File.pdf");
                bool file = _IATService.filesave(filereplace, Tran);

                return File(abytes, "application/pdf", filereplace);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                //swMailLog.WriteLine("Errorlog date : " + DateTime.Now.ToString());
                //swMailLog.WriteLine("Error message : " + ex.Message.ToString());
                //swMailLog.Close();
                //swMailLog.Dispose();
                throw ex;
            }

        }

        //public ActionResult GeneratePdfReport(int Tran, string AssetType)
        //{
        //    try
        //    {
        //        AssetTransferReport assetregisterReport = new AssetTransferReport();
        //        AssetTransferReportNon assetregisterReportNon = new AssetTransferReportNon();

        //        string filename = $"{Tran}_AssetTransferForm.pdf";
        //        byte[] abytes;

        //        if (AssetType == "Capitalized Asset")
        //        {
        //            abytes = assetregisterReport.PrepareReport(GetAssetRegisterDetail(Tran, filename));
        //        }
        //        else
        //        {
        //            abytes = assetregisterReportNon.PrepareReport(GetAssetRegisterDetailNon(Tran, filename));
        //        }

        //        string _Path = Path.Combine(serverpath.getFileUploadPath(), "AT", "AT PDF");
        //        string pdfPath = Path.Combine(_Path, filename);
        //        System.IO.File.WriteAllBytes(pdfPath, abytes);

        //        // Prepare new file with logo
        //        string updatedPdfPath = pdfPath.Replace(".pdf", "_File.pdf");

        //        using (var reader = new iText.Kernel.Pdf.PdfReader(pdfPath))
        //        using (var writer = new iText.Kernel.Pdf.PdfWriter(updatedPdfPath))
        //        using (var pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader, writer))
        //        using (var document = new iText.Layout.Document(pdfDoc))
        //        {
        //            var page = pdfDoc.GetFirstPage();
        //            var pageSize = page.GetPageSize();

        //            string relativePathimg = "logoex.png";
        //            string fullPathimg = Path.Combine(_Path, relativePathimg);
        //            var imageData = iText.IO.Image.ImageDataFactory.Create(fullPathimg);
        //            var img = new iText.Layout.Element.Image(imageData)
        //                        .ScaleToFit(50, 50)
        //                        .SetFixedPosition(1, pageSize.GetWidth() - 60, pageSize.GetHeight() - 60);

        //            document.Add(img);
        //        }

        //        abytes = System.IO.File.ReadAllBytes(updatedPdfPath);
        //        var filereplace = filename.Replace(".pdf", "_File.pdf");
        //        bool file = _IATService.filesave(filereplace, Tran);

        //        return File(abytes, "application/pdf", filereplace);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #region Capitalized

        public List<AssetTransferDetail> GetAssetRegisterDetail(int tran, string filename)
        {
            List<AssetTransferDetail> AssetRegisterList = new List<AssetTransferDetail>();
            AssetTransferDetail AssetList = new AssetTransferDetail();
            List<AT_ASSET_TRANSFER_DETAIL> detail = new List<AT_ASSET_TRANSFER_DETAIL>();
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            var HeaderDetail = _IATService.Get_AssetRegister_Header_Detail(userId, tran, filename);
            var Detail = _IATService.DetailRecord(userId, tran);
            var Auth = _IATService.GetAuthorityDetail(userId, tran);


            int index = 1;
            foreach (var item in Detail)
            {
                AssetList = new AssetTransferDetail();
                AssetList.SrNo = index;
                AssetList.AssetCode = item.ASSET_CODE;
                AssetList.AssetDescription = item.DESCRIPTION;
                AssetList.VendorName = item.VENDER_NAME == null ? "" : item.VENDER_NAME;
                AssetList.PoNo = item.PO_NO == null ? "" : item.PO_NO;
                AssetList.PODate = Convert.ToDateTime(item.PO_DATE);
                AssetList.InvoiceNo = item.INV_NO == null ? "" : item.INV_NO;
                AssetList.InvoiceDate = Convert.ToDateTime(item.INV_DATE);
                AssetList.FullPartial = item.FULLY_PARTIAL;
                AssetList.Qty = item.QTY;
                AssetList.CurrentLocation = item.CURRENT_LOCATION == null ? "" : item.CURRENT_LOCATION;
                AssetList.NewLocation = item.NEW_LOCATION;
                AssetList.ORIGINAL_COST = item.ORIGINAL_COST == 0 ? 0 : item.ORIGINAL_COST;
                AssetList.DEPRECIATION = item.DEPRECIATION == 0 ? 0 : item.DEPRECIATION;
                AssetList.NET_BLOCK = item.NET_BLOCK == 0 ? 0 : item.NET_BLOCK;
                AssetList.BASIC_PRICE = item.BASIC_PRICE == 0 ? 0 : item.BASIC_PRICE;
                AssetList.INVOICE_VALUE = item.INVOICE_VALUE == 0 ? 0 : item.INVOICE_VALUE;
                AssetList.Transaction_Type = HeaderDetail.Tran_Type;
                AssetList.Plant = HeaderDetail.Plant;
                AssetList.Designation = HeaderDetail.Designation;
                AssetList.TransferorName = HeaderDetail.Transferor;
                AssetList.TransfereeName = HeaderDetail.Transferee;
                AssetList.DepartmentHead = Auth.DepartmentHead == null ? "" : Auth.DepartmentHead;
                AssetList.DivisionHead = Auth.DivisionHead == null ? "" : Auth.DivisionHead;
                AssetList.OperatingHead = Auth.OperatingHead == null ? "" : Auth.OperatingHead;
                AssetList.Director = Auth.Director == null ? "" : Auth.Director;
                AssetList.Directortwo = Auth.Directortwo == null ? "" : Auth.Directortwo;
                AssetList.ExecutiveCoordinator = Auth.ExecutiveCoordinator == null ? "" : Auth.ExecutiveCoordinator;
                AssetList.ExecutiveVicePresident = Auth.ExecutiveVicePresident == null ? "" : Auth.ExecutiveVicePresident;
                AssetList.CPO = Auth.CPO == null ? "" : Auth.CPO;
                AssetList.Coordinator = Auth.Coordinator == null ? "" : Auth.Coordinator;
                AssetList.DepartmentHeaddate = Auth.DepartmentHeaddate;
                AssetList.Coordinatordate = Auth.Coordinatordate;
                AssetList.DivisionHeaddate = Auth.DivisionHeaddate;
                AssetList.ExecutiveCoordinatordate = Auth.ExecutiveCoordinatordate;
                AssetList.OperatingHeaddate = Auth.OperatingHeaddate;
                AssetList.ExecutiveVicePresidentdate = Auth.ExecutiveVicePresidentdate;
                AssetList.Directordate = Auth.Directordate;
                AssetList.Directortwodate = Auth.Directortwodate;
                AssetList.CPOdate = Auth.CPOdate;
                AssetList.Preparedbydate = Auth.Preparedbydate;
                AssetList.TeamMemberdate = Auth.TeamMemberdate;
                AssetList.FSectionHeaddate = Auth.FSectionHeaddate;
                AssetList.FDepartmentHeaddate = Auth.FDepartmentHeaddate;
                AssetList.FDivisionHeaddate = Auth.FDivisionHeaddate;
                AssetList.TeamMember = Auth.TeamMember == null ? "" : Auth.TeamMember;
                AssetList.FSectionHead = Auth.FSectionHead == null ? "" : Auth.FSectionHead;
                AssetList.FDepartmentHead = Auth.FDepartmentHead == null ? "" : Auth.FDepartmentHead;
                AssetList.FDivisionHead = Auth.FDivisionHead == null ? "" : Auth.FDivisionHead;
                AssetList.ReasonTransfer = HeaderDetail.Remarks == null ? "" : HeaderDetail.Remarks;
                AssetList.Remarks = Auth.Remarks == null ? "" : Auth.Remarks;
                AssetList.Duration = HeaderDetail.Duration;

                AssetRegisterList.Add(AssetList);
                index++;
            }

            return AssetRegisterList;
        }
        public class AssetTransferDetail
        {
            public int SrNo { get; set; }
            public string AssetCode { get; set; }
            public string TransfereeName { get; set; }
            public string TransferorName { get; set; }
            public string AssetDescription { get; set; }
            public string Duration { get; set; }
            public string VendorName { get; set; }
            public string PoNo { get; set; }
            public DateTime PODate { get; set; }
            public string InvoiceNo { get; set; }
            public DateTime InvoiceDate { get; set; }
            public string FullPartial { get; set; }
            public Nullable<decimal> Qty { get; set; }
            public string CurrentLocation { get; set; }
            public string NewLocation { get; set; }
            public decimal ORIGINAL_COST { get; set; }
            public decimal DEPRECIATION { get; set; }
            public decimal NET_BLOCK { get; set; }
            public decimal BASIC_PRICE { get; set; }
            public decimal INVOICE_VALUE { get; set; }
            public decimal Total { get; set; }
            public string ReasonTransfer { get; set; }
            public long Tran_No { get; set; }
            public string Plant { get; set; }
            public string Designation { get; set; }
            public string Transaction_Type { get; set; }
            public string Preparedby { get; set; }
            public string DepartmentHead { get; set; }
            public string Coordinator { get; set; }
            public string DivisionHead { get; set; }
            public string ExecutiveCoordinator { get; set; }
            public string OperatingHead { get; set; }
            public string ExecutiveVicePresident { get; set; }
            public string Director { get; set; }
            public string Directortwo { get; set; }
            public string CPO { get; set; }
            public string TeamMember { get; set; }
            public string FSectionHead { get; set; }
            public string FDepartmentHead { get; set; }
            public string FDivisionHead { get; set; }

            public string Remarks { get; set; }

            public DateTime Preparedbydate { get; set; }
            public DateTime DepartmentHeaddate { get; set; }
            public DateTime Coordinatordate { get; set; }
            public DateTime DivisionHeaddate { get; set; }
            public DateTime ExecutiveCoordinatordate { get; set; }
            public DateTime OperatingHeaddate { get; set; }
            public DateTime ExecutiveVicePresidentdate { get; set; }
            public DateTime Directordate { get; set; }
            public DateTime Directortwodate { get; set; }
            public DateTime CPOdate { get; set; }
            public DateTime TeamMemberdate { get; set; }
            public DateTime FSectionHeaddate { get; set; }
            public DateTime FDepartmentHeaddate { get; set; }
            public DateTime FDivisionHeaddate { get; set; }

        }

        public class AssetTransferReport
        {
            int _totalColumn = 17;
            Document _document;
            Font _fontStyle;
            PdfPTable _pdfTable = new PdfPTable(17);
            PdfPCell _pdfPCell;
            MemoryStream _memoryStream = new MemoryStream();
            List<AssetTransferDetail> _assetregister = new List<AssetTransferDetail>();


            public byte[] PrepareReport(List<AssetTransferDetail> assetregister)
            {
                _assetregister = assetregister;

                //_document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
                _document = new Document(new Rectangle(PageSize.A4.Height, PageSize.A4.Width), 0f, 0f, 0f, 0f);
                //_document.SetPageSize(PageSize.A4);
                _document.SetMargins(10f, 10f, 10f, 10f);
                _pdfTable.WidthPercentage = 100;
                _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
                _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);



                PdfWriter pdfWriter = PdfWriter.GetInstance(_document, _memoryStream);

                _document.Open();
                _pdfTable.SetWidths(new float[] { 100f, 200f, 200f, 200f, 200f, 200f, 200f, 200f, 200f, 100f, 400f, 400f, 200f, 200f, 200f, 200f, 200f });
                String Tran_Type = assetregister.Max(i => i.Transaction_Type).ToString();
                String Plant = assetregister.Max(i => i.Plant).ToString();
                String Desig = assetregister.Max(i => i.Designation).ToString();
                String Remarks = assetregister.Max(i => i.ReasonTransfer).ToString();
                String TRemarks = assetregister.Max(i => i.Remarks).ToString();
                String duration = assetregister.Max(i => i.Duration).ToString();



                decimal ori = (from s in assetregister select s.ORIGINAL_COST).Sum();
                decimal Dep = (from s in assetregister select s.DEPRECIATION).Sum();
                decimal Net = (from s in assetregister select s.NET_BLOCK).Sum();
                decimal Basic = (from s in assetregister select s.BASIC_PRICE).Sum();
                decimal inv = (from s in assetregister select s.INVOICE_VALUE).Sum();

                #region date
                string deptdate = assetregister.Max(i => i.DepartmentHeaddate).ToString("dd-MMM-yy");
                if (deptdate == "01-Jan-01")
                {
                    deptdate = "";
                }
                string cordate = assetregister.Max(i => i.Coordinatordate).ToString("dd-MMM-yy");
                if (cordate == "01-Jan-01")
                {
                    cordate = "";
                }
                string divdate = assetregister.Max(i => i.DivisionHeaddate).ToString("dd-MMM-yy");
                if (divdate == "01-Jan-01")
                {
                    divdate = "";
                }
                string exdate = assetregister.Max(i => i.ExecutiveCoordinatordate).ToString("dd-MMM-yy");
                if (exdate == "01-Jan-01")
                {
                    exdate = "";
                }
                string opdate = assetregister.Max(i => i.OperatingHeaddate).ToString("dd-MMM-yy");
                if (opdate == "01-Jan-01")
                {
                    opdate = "";
                }
                string evpdate = assetregister.Max(i => i.ExecutiveVicePresidentdate).ToString("dd-MMM-yy");
                if (evpdate == "01-Jan-01")
                {
                    evpdate = "";
                }
                string dir = assetregister.Max(i => i.Directordate).ToString("dd-MMM-yy");
                if (dir == "01-Jan-01")
                {
                    dir = "";
                }
                string dir2 = assetregister.Max(i => i.Directortwodate).ToString("dd-MMM-yy");
                if (dir2 == "01-Jan-01")
                {
                    dir2 = "";
                }
                string cpo = assetregister.Max(i => i.CPOdate).ToString("dd-MMM-yy");
                if (cpo == "01-Jan-01")
                {
                    cpo = "";
                }
                string predate = assetregister.Max(i => i.Preparedbydate).ToString("dd-MMM-yy");
                if (predate == "01-Jan-01")
                {
                    predate = "";
                }

                string Fteamdate = assetregister.Max(i => i.TeamMemberdate).ToString("dd-MMM-yy");
                if (Fteamdate == "01-Jan-01")
                {
                    Fteamdate = "";
                }
                string Fsedate = assetregister.Max(i => i.FSectionHeaddate).ToString("dd-MMM-yy");
                if (Fsedate == "01-Jan-01")
                {
                    Fsedate = "";
                }
                string Fdedate = assetregister.Max(i => i.FDepartmentHeaddate).ToString("dd-MMM-yy");
                if (Fdedate == "01-Jan-01")
                {
                    Fdedate = "";
                }
                string Fdivdata = assetregister.Max(i => i.FDivisionHeaddate).ToString("dd-MMM-yy");
                if (Fdivdata == "01-Jan-01")
                {
                    Fdivdata = "";
                }
                #endregion

                this.ReportHeader(Tran_Type, Plant, Desig, duration);
                this.ReportBody();
                this.TotalBody(ori, Dep, Net, Tran_Type, Remarks, Basic, inv);
                this.UserApproval();
                this.UserApprovalDate(deptdate, cordate, divdate, exdate, opdate, evpdate, dir, dir2, cpo, predate);
                this.BlankRows();
                this.FinanceApproval(TRemarks);
                this.FinanceApprovalDate(Fteamdate, Fsedate, Fdedate, Fdivdata);
                this.BlankRows();
                this.Note();

                _document.Add(_pdfTable);
                _document.Close();
                return _memoryStream.ToArray();
            }
            private void ReportHeader(String Tran_Type, String Plant, String Desig, String duration)
            {

                //Main Title
                String ReportTitle = "Asset Transfer Approval";
                _fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
                _pdfPCell = new PdfPCell(new Phrase(ReportTitle, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();


                //Blank Row 
                _fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                //Sub table Header
                _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Plant/HO/RO/ZO : " + Plant, _fontStyle));
                _pdfPCell.Colspan = 7;
                _pdfPCell.Border = 0;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);


                _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                //_pdfPCell.Colspan = (_totalColumn - 15);
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.Colspan = 15;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                _pdfPCell = new PdfPCell(new Phrase("Section/Department/Division/Operation : " + Desig, _fontStyle));
                _pdfPCell.Colspan = 17;
                _pdfPCell.Rowspan = 2;
                _pdfPCell.Border = 0;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);


                _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();


                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = 7;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Transaction Type : " + Tran_Type, _fontStyle));
                _pdfPCell.Colspan = 5;
                _pdfPCell.Border = 0;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);



                if (Tran_Type == "Returnable Transfer")
                {
                    _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
                    _pdfPCell = new PdfPCell(new Phrase("Duration : " + duration, _fontStyle));
                    _pdfPCell.Colspan = 3;
                    _pdfPCell.Border = 0;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.ExtraParagraphSpace = 0;
                    _pdfTable.AddCell(_pdfPCell);


                }
                else
                {
                    _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.Border = 0;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                }

                var date = DateTime.Now;
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Date : " + date.ToString("dd-MMM-yy"), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.Colspan = 3;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();


                ////Blank Row 
                //_fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = 15;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);


                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("(Amt in INR)", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.Border = 0;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();



            }
            private void ReportBody()
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Sr.No", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("SAP Asset No./Code", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Asset Description", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Vendor Name", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Po.No.", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Po.Date", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Invoice No", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Invoice Date", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Full Partial", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Qty", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Current Location(Detailed Address with State)", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("New Location(Detailed Address with State)", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Original Cost(P)", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Accumulated Depreciation (Q)", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Net Block (R = P - Q)", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Basic Price", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Invoice Value", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("If already Capitalized in FAR", _fontStyle));
                _pdfPCell.Colspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("If Not Capitalized in FAR", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

                //Body
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                int sno = 1;
                foreach (AssetTransferDetail A in _assetregister)
                {
                    _pdfPCell = new PdfPCell(new Phrase(sno++.ToString(), _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.AssetCode, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.AssetDescription, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase(A.VendorName, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.PoNo, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string podate = A.PODate.ToString("dd-MMM-yy");
                    if (podate == "01-Jan-01")
                    {
                        podate = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(podate, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.InvoiceNo, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    string indate = A.InvoiceDate.ToString("dd-MMM-yy");
                    if (indate == "01-Jan-01")
                    {
                        indate = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(indate, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.FullPartial, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.Qty.ToString(), _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.CurrentLocation, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.NewLocation, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.ORIGINAL_COST.ToString("#,##0"), _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.DEPRECIATION.ToString("#,##0"), _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.NET_BLOCK.ToString("#,##0"), _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string basic = A.BASIC_PRICE.ToString("#,##0");
                    if (basic == "0")
                    {
                        basic = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(basic, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    string inv = A.INVOICE_VALUE.ToString("#,##0");
                    if (inv == "0")
                    {
                        inv = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(inv, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfTable.CompleteRow();
                }
            }

            private void TotalBody(decimal ori, decimal Dep, decimal Net, String Tran_Type, String Remarks, decimal Basic, decimal Inv)
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Total", _fontStyle));
                _pdfPCell.Colspan = 12;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase(ori.ToString("#,##0"), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Dep.ToString("#,##0"), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Net.ToString("#,##0"), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                string basic = Basic.ToString("#,##0");
                if (basic == "0")
                {
                    basic = "";
                }
                _pdfPCell = new PdfPCell(new Phrase(basic, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                string inv = Inv.ToString("#,##0");
                if (inv == "0")
                {
                    inv = "";
                }
                _pdfPCell = new PdfPCell(new Phrase(inv, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

                _pdfPCell = new PdfPCell(new Phrase("Reason of Transfer : " + Remarks, _fontStyle));
                _pdfPCell.Colspan = 12;
                _pdfPCell.BorderColorBottom = BaseColor.WHITE;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Transferor/Custodian Name & E. Code ", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Transferee/Custodian Name & E. Code  " + "(" + Tran_Type + ")", _fontStyle));
                _pdfPCell.Colspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfTable.CompleteRow();




                //Body
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);

                foreach (AssetTransferDetail A in _assetregister)
                {

                    _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _pdfPCell.Colspan = 12;

                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase(A.TransferorName, _fontStyle));
                    _pdfPCell.Colspan = 2;

                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.TransfereeName, _fontStyle));
                    _pdfPCell.Colspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfTable.CompleteRow();

                    _pdfPCell = new PdfPCell(new Phrase("User Approval ", _fontStyle));
                    _pdfPCell.Colspan = _totalColumn;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfTable.CompleteRow();

                    break;
                }
            }

            private void UserApproval()
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Prepared by", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Department Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Coordinator", _fontStyle));
                _pdfPCell.Colspan = 2;
                //_pdfPCell.Rowspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Division Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Executive Coordinator", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Operating Head", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Executive Vice President", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Opt Head PPC /Sr. Dir. Purchase /Director", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Opt Head PPC /Sr. Dir. Purchase /Director", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("CPO & Director", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

                //Body
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);

                foreach (AssetTransferDetail A in _assetregister)
                {

                    _pdfPCell = new PdfPCell(new Phrase(A.TransferorName, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.DepartmentHead, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.Coordinator, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.DivisionHead, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.ExecutiveCoordinator, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.OperatingHead, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.ExecutiveVicePresident, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.Director, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.Directortwo, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.CPO, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfTable.CompleteRow();
                    break;
                }
            }

            private void UserApprovalDate(string deptdate, string cordate, string divdate, string exdate, string opdate, string evpdate, string dir, string dir2, string cpo, string predate)
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Date : " + predate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);



                _pdfPCell = new PdfPCell(new Phrase(deptdate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(cordate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(divdate, _fontStyle));
                _pdfPCell.Colspan = 2;
                //_pdfPCell.Rowspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(exdate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(opdate, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(evpdate, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(dir, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(dir2, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(cpo, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

            }

            private void BlankRows()
            {

                //Blank Row 
                _fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

            }

            private void FinanceApproval(String TRemarks)
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Finance & Accounts Approval", _fontStyle));
                _pdfPCell.Colspan = 8;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Tax Compliance / Remarks if any", _fontStyle));
                _pdfPCell.Colspan = (_totalColumn - 6);
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Team Member", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Section Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Department Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Division Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Delivery Challan / Tax Invoice No. :  " + TRemarks, _fontStyle));
                _pdfPCell.Colspan = (_totalColumn - 6);
                _pdfPCell.Rowspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

                //Body
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);

                foreach (AssetTransferDetail A in _assetregister)
                {

                    _pdfPCell = new PdfPCell(new Phrase(A.TeamMember, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.FSectionHead, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.FDepartmentHead, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.FDivisionHead, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);



                    _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _pdfPCell.Colspan = (_totalColumn - 8);
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.Border = 0;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfTable.CompleteRow();
                    break;
                }
            }

            private void FinanceApprovalDate(string Fteamdate, string Fsedate, string Fdedate, string Fdivdata)
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Date : " + Fteamdate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Fsedate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Fdedate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Fdivdata, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = (_totalColumn - 8);
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();
            }

            private void Note()
            {

                //Blank Row 
                _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Note : <EBQ> User Approval Up to CPO is required) ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                _pdfPCell = new PdfPCell(new Phrase("(Other than <EBQ> User Approval Up to Operating Head is required)", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

            }
        }
        #endregion

        #region NonCapitalized
        public List<AssetTransferDetailNon> GetAssetRegisterDetailNon(int tran, string filename)
        {
            List<AssetTransferDetailNon> AssetRegisterList = new List<AssetTransferDetailNon>();
            AssetTransferDetailNon AssetList = new AssetTransferDetailNon();
            List<AT_ASSET_TRANSFER_DETAIL> detail = new List<AT_ASSET_TRANSFER_DETAIL>();
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            var HeaderDetail = _IATService.Get_AssetRegister_Header_Detail(userId, tran, filename);
            var Detail = _IATService.DetailRecord(userId, tran);
            var Auth = _IATService.GetAuthorityDetail(userId, tran);


            int index = 1;
            foreach (var item in Detail)
            {
                AssetList = new AssetTransferDetailNon();
                AssetList.SrNo = index;
                AssetList.AssetCode = item.ASSET_CODE;
                AssetList.AssetDescription = item.DESCRIPTION;
                AssetList.VendorName = item.VENDER_NAME == null ? "" : item.VENDER_NAME;
                AssetList.PoNo = item.PO_NO == null ? "" : item.PO_NO;
                AssetList.PODate = Convert.ToDateTime(item.PO_DATE);
                AssetList.InvoiceNo = item.INV_NO == null ? "" : item.INV_NO;
                AssetList.InvoiceDate = Convert.ToDateTime(item.INV_DATE);
                AssetList.FullPartial = item.FULLY_PARTIAL;
                AssetList.CurrentLocation = item.CURRENT_LOCATION == null ? "" : item.CURRENT_LOCATION;
                AssetList.NewLocation = item.NEW_LOCATION;
                AssetList.ORIGINAL_COST = item.ORIGINAL_COST == 0 ? 0 : item.ORIGINAL_COST;
                AssetList.DEPRECIATION = item.DEPRECIATION == 0 ? 0 : item.DEPRECIATION;
                AssetList.NET_BLOCK = item.NET_BLOCK == 0 ? 0 : item.NET_BLOCK;
                AssetList.BASIC_PRICE = item.BASIC_PRICE == 0 ? 0 : item.BASIC_PRICE;
                AssetList.INVOICE_VALUE = item.INVOICE_VALUE == 0 ? 0 : item.INVOICE_VALUE;
                AssetList.Transaction_Type = HeaderDetail.Tran_Type;
                AssetList.Plant = HeaderDetail.Plant;
                AssetList.Designation = HeaderDetail.Designation;
                AssetList.TransferorName = HeaderDetail.Transferor;
                AssetList.TransfereeName = HeaderDetail.Transferee;
                AssetList.DepartmentHead = Auth.DepartmentHead == null ? "" : Auth.DepartmentHead;
                AssetList.DivisionHead = Auth.DivisionHead == null ? "" : Auth.DivisionHead;
                AssetList.OperatingHead = Auth.OperatingHead == null ? "" : Auth.OperatingHead;
                AssetList.Director = Auth.Director == null ? "" : Auth.Director;
                AssetList.Directortwo = Auth.Directortwo == null ? "" : Auth.Directortwo;
                AssetList.ExecutiveCoordinator = Auth.ExecutiveCoordinator == null ? "" : Auth.ExecutiveCoordinator;
                AssetList.ExecutiveVicePresident = Auth.ExecutiveVicePresident == null ? "" : Auth.ExecutiveVicePresident;
                AssetList.CPO = Auth.CPO == null ? "" : Auth.CPO;
                AssetList.Coordinator = Auth.Coordinator == null ? "" : Auth.Coordinator;
                AssetList.DepartmentHeaddate = Auth.DepartmentHeaddate;
                AssetList.Coordinatordate = Auth.Coordinatordate;
                AssetList.DivisionHeaddate = Auth.DivisionHeaddate;
                AssetList.ExecutiveCoordinatordate = Auth.ExecutiveCoordinatordate;
                AssetList.OperatingHeaddate = Auth.OperatingHeaddate;
                AssetList.ExecutiveVicePresidentdate = Auth.ExecutiveVicePresidentdate;
                AssetList.Directordate = Auth.Directordate;
                AssetList.Directortwodate = Auth.Directortwodate;
                AssetList.CPOdate = Auth.CPOdate;
                AssetList.Preparedbydate = Auth.Preparedbydate;
                AssetList.TeamMemberdate = Auth.TeamMemberdate;
                AssetList.FSectionHeaddate = Auth.FSectionHeaddate;
                AssetList.FDepartmentHeaddate = Auth.FDepartmentHeaddate;
                AssetList.FDivisionHeaddate = Auth.FDivisionHeaddate;
                AssetList.TeamMember = Auth.TeamMember == null ? "" : Auth.TeamMember;
                AssetList.FSectionHead = Auth.FSectionHead == null ? "" : Auth.FSectionHead;
                AssetList.FDepartmentHead = Auth.FDepartmentHead == null ? "" : Auth.FDepartmentHead;
                AssetList.FDivisionHead = Auth.FDivisionHead == null ? "" : Auth.FDivisionHead;
                AssetList.ReasonTransfer = HeaderDetail.Remarks == null ? "" : HeaderDetail.Remarks;
                AssetList.Remarks = Auth.Remarks == null ? "" : Auth.Remarks;
                AssetList.Duration = HeaderDetail.Duration;

                AssetRegisterList.Add(AssetList);
                index++;
            }

            return AssetRegisterList;
        }

        public class AssetTransferDetailNon
        {
            public int SrNo { get; set; }
            public string AssetCode { get; set; }
            public string TransfereeName { get; set; }
            public string TransferorName { get; set; }
            public string AssetDescription { get; set; }
            public string VendorName { get; set; }
            public string PoNo { get; set; }
            public DateTime PODate { get; set; }
            public string InvoiceNo { get; set; }
            public DateTime InvoiceDate { get; set; }
            public string FullPartial { get; set; }
            public string CurrentLocation { get; set; }
            public string NewLocation { get; set; }
            public decimal ORIGINAL_COST { get; set; }
            public decimal DEPRECIATION { get; set; }
            public decimal NET_BLOCK { get; set; }
            public decimal BASIC_PRICE { get; set; }
            public decimal INVOICE_VALUE { get; set; }
            public decimal Total { get; set; }
            public string ReasonTransfer { get; set; }
            public long Tran_No { get; set; }
            public string Plant { get; set; }
            public string Designation { get; set; }
            public string Transaction_Type { get; set; }
            public string Preparedby { get; set; }
            public string DepartmentHead { get; set; }
            public string Coordinator { get; set; }
            public string DivisionHead { get; set; }
            public string ExecutiveCoordinator { get; set; }
            public string OperatingHead { get; set; }
            public string ExecutiveVicePresident { get; set; }
            public string Director { get; set; }
            public string Directortwo { get; set; }
            public string CPO { get; set; }
            public string TeamMember { get; set; }
            public string FSectionHead { get; set; }
            public string FDepartmentHead { get; set; }
            public string FDivisionHead { get; set; }

            public string Remarks { get; set; }
            public string Duration { get; set; }

            public DateTime Preparedbydate { get; set; }
            public DateTime DepartmentHeaddate { get; set; }
            public DateTime Coordinatordate { get; set; }
            public DateTime DivisionHeaddate { get; set; }
            public DateTime ExecutiveCoordinatordate { get; set; }
            public DateTime OperatingHeaddate { get; set; }
            public DateTime ExecutiveVicePresidentdate { get; set; }
            public DateTime Directordate { get; set; }
            public DateTime Directortwodate { get; set; }
            public DateTime CPOdate { get; set; }
            public DateTime TeamMemberdate { get; set; }
            public DateTime FSectionHeaddate { get; set; }
            public DateTime FDepartmentHeaddate { get; set; }
            public DateTime FDivisionHeaddate { get; set; }

        }
        public class AssetTransferReportNon
        {
            int _totalColumn = 16;
            Document _document;
            Font _fontStyle;
            PdfPTable _pdfTable = new PdfPTable(16);
            PdfPCell _pdfPCell;
            MemoryStream _memoryStream = new MemoryStream();
            List<AssetTransferDetailNon> _assetregister = new List<AssetTransferDetailNon>();

            public byte[] PrepareReport(List<AssetTransferDetailNon> assetregister)
            {
                _assetregister = assetregister;

                //_document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
                _document = new Document(new Rectangle(PageSize.A4.Height, PageSize.A4.Width), 0f, 0f, 0f, 0f);
                //_document.SetPageSize(PageSize.A4);
                _document.SetMargins(10f, 10f, 10f, 10f);
                _pdfTable.WidthPercentage = 100;
                _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
                _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);

                PdfWriter pdfWriter = PdfWriter.GetInstance(_document, _memoryStream);

                _document.Open();
                _pdfTable.SetWidths(new float[] { 100f, 200f, 200f, 200f, 200f, 150f, 250f, 150f, 100f, 400f, 400f, 200f, 200f, 200f, 200f, 200f });
                String Tran_Type = assetregister.Max(i => i.Transaction_Type).ToString();
                String Plant = assetregister.Max(i => i.Plant).ToString();
                String Desig = assetregister.Max(i => i.Designation).ToString();
                String Remarks = assetregister.Max(i => i.ReasonTransfer).ToString();
                String TRemarks = assetregister.Max(i => i.Remarks).ToString();

                String duration = assetregister.Max(i => i.Duration).ToString();

                decimal ori = (from s in assetregister select s.ORIGINAL_COST).Sum();
                decimal Dep = (from s in assetregister select s.DEPRECIATION).Sum();
                decimal Net = (from s in assetregister select s.NET_BLOCK).Sum();
                decimal Basic = (from s in assetregister select s.BASIC_PRICE).Sum();
                decimal inv = (from s in assetregister select s.INVOICE_VALUE).Sum();

                #region date
                string deptdate = assetregister.Max(i => i.DepartmentHeaddate).ToString("dd-MMM-yy");
                if (deptdate == "01-Jan-01")
                {
                    deptdate = "";
                }
                string cordate = assetregister.Max(i => i.Coordinatordate).ToString("dd-MMM-yy");
                if (cordate == "01-Jan-01")
                {
                    cordate = "";
                }
                string divdate = assetregister.Max(i => i.DivisionHeaddate).ToString("dd-MMM-yy");
                if (divdate == "01-Jan-01")
                {
                    divdate = "";
                }
                string exdate = assetregister.Max(i => i.ExecutiveCoordinatordate).ToString("dd-MMM-yy");
                if (exdate == "01-Jan-01")
                {
                    exdate = "";
                }
                string opdate = assetregister.Max(i => i.OperatingHeaddate).ToString("dd-MMM-yy");
                if (opdate == "01-Jan-01")
                {
                    opdate = "";
                }
                string evpdate = assetregister.Max(i => i.ExecutiveVicePresidentdate).ToString("dd-MMM-yy");
                if (evpdate == "01-Jan-01")
                {
                    evpdate = "";
                }
                string dir = assetregister.Max(i => i.Directordate).ToString("dd-MMM-yy");
                if (dir == "01-Jan-01")
                {
                    dir = "";
                }
                string dir2 = assetregister.Max(i => i.Directortwodate).ToString("dd-MMM-yy");
                if (dir2 == "01-Jan-01")
                {
                    dir2 = "";
                }
                string cpo = assetregister.Max(i => i.CPOdate).ToString("dd-MMM-yy");
                if (cpo == "01-Jan-01")
                {
                    cpo = "";
                }
                string predate = assetregister.Max(i => i.Preparedbydate).ToString("dd-MMM-yy");
                if (predate == "01-Jan-01")
                {
                    predate = "";
                }

                string Fteamdate = assetregister.Max(i => i.TeamMemberdate).ToString("dd-MMM-yy");
                if (Fteamdate == "01-Jan-01")
                {
                    Fteamdate = "";
                }
                string Fsedate = assetregister.Max(i => i.FSectionHeaddate).ToString("dd-MMM-yy");
                if (Fsedate == "01-Jan-01")
                {
                    Fsedate = "";
                }
                string Fdedate = assetregister.Max(i => i.FDepartmentHeaddate).ToString("dd-MMM-yy");
                if (Fdedate == "01-Jan-01")
                {
                    Fdedate = "";
                }
                string Fdivdata = assetregister.Max(i => i.FDivisionHeaddate).ToString("dd-MMM-yy");
                if (Fdivdata == "01-Jan-01")
                {
                    Fdivdata = "";
                }
                #endregion

                this.ReportHeader(Tran_Type, Plant, Desig, duration);
                this.ReportBody();
                this.TotalBody(ori, Dep, Net, Tran_Type, Remarks, Basic, inv);
                this.UserApproval();
                this.UserApprovalDate(deptdate, cordate, divdate, exdate, opdate, evpdate, dir, dir2, cpo, predate);
                this.BlankRows();
                this.FinanceApproval(TRemarks);
                this.FinanceApprovalDate(Fteamdate, Fsedate, Fdedate, Fdivdata);
                this.BlankRows();
                this.Note();
                //_pdfTable.HeaderRows = 3;
                _document.Add(_pdfTable);
                _document.Close();
                return _memoryStream.ToArray();
            }
            private void ReportHeader(String Tran_Type, String Plant, String Desig, String duration)
            {
                //Main Title
                String ReportTitle = "Asset Transfer Approval";
                _fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
                _pdfPCell = new PdfPCell(new Phrase(ReportTitle, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                //Blank Row 
                _fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();



                //Sub table Header
                _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Plant/HO/RO/ZO : " + Plant, _fontStyle));
                _pdfPCell.Colspan = 7;
                _pdfPCell.Border = 0;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);


                _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                //_pdfPCell.Colspan = (_totalColumn - 15);
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.Colspan = 15;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                _pdfPCell = new PdfPCell(new Phrase("Section/Department/Division/Operation : " + Desig, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.Rowspan = 2;
                _pdfPCell.Border = 0;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);


                _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = 6;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Transaction Type : " + Tran_Type, _fontStyle));
                _pdfPCell.Colspan = 5;
                _pdfPCell.Border = 0;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);


                if (Tran_Type == "Returnable Transfer")
                {
                    _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
                    _pdfPCell = new PdfPCell(new Phrase("Duration :" + duration, _fontStyle));
                    _pdfPCell.Colspan = 3;
                    _pdfPCell.Border = 0;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.ExtraParagraphSpace = 0;
                    _pdfTable.AddCell(_pdfPCell);


                }
                else
                {
                    _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.Border = 0;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                }

                var date = DateTime.Now;
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Date of Transfer : " + date.ToString("dd-MMM-yy"), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.Colspan = 3;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();



                ////Blank Row 
                //_fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = 15;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);


                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("(Amt in INR)", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.Border = 0;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();
            }
            private void ReportBody()
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Sr.No", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("SAP Asset No./Code", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Asset Description", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Vendor Name", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Po.No.", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Po.Date", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Invoice No", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Invoice Date", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Full Partial", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Current Location(Detailed Address with State)", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("New Location(Detailed Address with State)", _fontStyle));
                _pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Original Cost(P)", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Accumulated Depreciation (Q)", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Net Block (R = P - Q)", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Basic Price", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Invoice Value", _fontStyle));
                //_pdfPCell.Rowspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("If already Capitalized in FAR", _fontStyle));
                _pdfPCell.Colspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("If Not Capitalized in FAR", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

                //Body
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                int sno = 1;
                foreach (AssetTransferDetailNon A in _assetregister)
                {
                    _pdfPCell = new PdfPCell(new Phrase(sno++.ToString(), _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.AssetCode, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.AssetDescription, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase(A.VendorName, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.PoNo, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string podate = A.PODate.ToString("dd-MMM-yy");
                    if (podate == "01-Jan-01")
                    {
                        podate = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(podate, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.InvoiceNo, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    string indate = A.InvoiceDate.ToString("dd-MMM-yy");
                    if (indate == "01-Jan-01")
                    {
                        indate = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(indate, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.FullPartial, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase(A.CurrentLocation, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.NewLocation, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string ori = A.ORIGINAL_COST.ToString("#,##0");
                    if (ori == "0")
                    {
                        ori = "";
                    }

                    _pdfPCell = new PdfPCell(new Phrase(ori, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    string Dep = A.DEPRECIATION.ToString("#,##0");
                    if (Dep == "0")
                    {
                        Dep = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(Dep, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    string Net = A.NET_BLOCK.ToString("#,##0");
                    if (Net == "0")
                    {
                        Net = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(Net, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    string basic = A.BASIC_PRICE.ToString("#,##0");
                    if (basic == "0")
                    {
                        basic = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(basic, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    string inv = A.INVOICE_VALUE.ToString("#,##0");
                    if (inv == "0")
                    {
                        inv = "";
                    }
                    _pdfPCell = new PdfPCell(new Phrase(inv, _fontStyle));
                    _pdfPCell.Rowspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfTable.CompleteRow();
                }
            }

            private void TotalBody(decimal ori, decimal Dep, decimal Net, String Tran_Type, String Remarks, decimal Basic, decimal Inv)
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Total", _fontStyle));
                _pdfPCell.Colspan = 11;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                string orig = ori.ToString("#,##0");
                if (orig == "0")
                {
                    orig = "";
                }
                _pdfPCell = new PdfPCell(new Phrase(orig, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                string Depp = Dep.ToString("#,##0");
                if (Depp == "0")
                {
                    Depp = "";
                }
                _pdfPCell = new PdfPCell(new Phrase(Depp, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                string Netb = Net.ToString("#,##0");
                if (Netb == "0")
                {
                    Netb = "";
                }
                _pdfPCell = new PdfPCell(new Phrase(Netb, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                string basic = Basic.ToString("#,##0");
                if (basic == "0")
                {
                    basic = "";
                }
                _pdfPCell = new PdfPCell(new Phrase(basic, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                string inv = Inv.ToString("#,##0");
                if (inv == "0")
                {
                    inv = "";
                }
                _pdfPCell = new PdfPCell(new Phrase(inv, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

                _pdfPCell = new PdfPCell(new Phrase("Reason of Transfer : " + Remarks, _fontStyle));
                _pdfPCell.Colspan = 11;
                _pdfPCell.BorderColorBottom = BaseColor.WHITE;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Transferor/Custodian Name & E. Code ", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Transferee/Custodian Name & E. Code  " + "(" + Tran_Type + ")", _fontStyle));
                _pdfPCell.Colspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfTable.CompleteRow();




                //Body
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);

                foreach (AssetTransferDetailNon A in _assetregister)
                {

                    _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                    _pdfPCell.Colspan = 11;

                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);


                    _pdfPCell = new PdfPCell(new Phrase(A.TransferorName, _fontStyle));
                    _pdfPCell.Colspan = 2;

                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.TransfereeName, _fontStyle));
                    _pdfPCell.Colspan = 3;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfTable.CompleteRow();

                    _pdfPCell = new PdfPCell(new Phrase("User Approval ", _fontStyle));
                    _pdfPCell.Colspan = _totalColumn;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfTable.CompleteRow();

                    break;
                }
            }

            private void UserApproval()
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Prepared by", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Department Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Coordinator", _fontStyle));
                _pdfPCell.Colspan = 2;
                //_pdfPCell.Rowspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Division Head", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Executive Coordinator", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Operating Head", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Executive Vice President", _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Opt Head PPC /Sr. Dir. Purchase /Director", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Opt Head PPC /Sr. Dir. Purchase /Director", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("CPO & Director", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

                //Body
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);

                foreach (AssetTransferDetailNon A in _assetregister)
                {

                    _pdfPCell = new PdfPCell(new Phrase(A.TransferorName, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.DepartmentHead, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.Coordinator, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.DivisionHead, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.ExecutiveCoordinator, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.OperatingHead, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.ExecutiveVicePresident, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.Director, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.Directortwo, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.CPO, _fontStyle));
                    _pdfPCell.Rowspan = 5;
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfTable.CompleteRow();
                    break;
                }
            }

            private void UserApprovalDate(string deptdate, string cordate, string divdate, string exdate, string opdate, string evpdate, string dir, string dir2, string cpo, string predate)
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Date : " + predate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);



                _pdfPCell = new PdfPCell(new Phrase(deptdate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(cordate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(divdate, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(exdate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(opdate, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(evpdate, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(dir, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(dir2, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(cpo, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

            }

            private void BlankRows()
            {

                //Blank Row 
                _fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

            }

            private void FinanceApproval(String TRemarks)
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Finance & Accounts Approval", _fontStyle));
                _pdfPCell.Colspan = 8;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Tax Compliance / Remarks if any", _fontStyle));
                _pdfPCell.Colspan = (_totalColumn - 6);
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Team Member", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Section Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Department Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase("Division Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase("Delivery Challan / Tax Invoice No. :  " + TRemarks, _fontStyle));
                _pdfPCell.Colspan = (_totalColumn - 6);
                _pdfPCell.Rowspan = 3;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();

                //Body
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);

                foreach (AssetTransferDetailNon A in _assetregister)
                {

                    _pdfPCell = new PdfPCell(new Phrase(A.TeamMember, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.FSectionHead, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.FDepartmentHead, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfPCell = new PdfPCell(new Phrase(A.FDivisionHead, _fontStyle));
                    _pdfPCell.Colspan = 2;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);



                    _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                    _pdfPCell.Colspan = (_totalColumn - 8);
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _pdfPCell.Border = 0;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfTable.AddCell(_pdfPCell);

                    _pdfTable.CompleteRow();
                    break;
                }
            }

            private void FinanceApprovalDate(string Fteamdate, string Fsedate, string Fdedate, string Fdivdata)
            {
                //Header
                _fontStyle = FontFactory.GetFont("Tahoma", 6f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Date : " + Fteamdate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Fsedate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Fdedate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(Fdivdata, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
                _pdfPCell.Colspan = (_totalColumn - 8);
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.Border = 0;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();
            }

            private void Note()
            {

                //Blank Row 
                _fontStyle = FontFactory.GetFont("Tahoma", 7f, 1);
                _pdfPCell = new PdfPCell(new Phrase("Note : <EBQ> User Approval Up to CPO is required) ", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

                _pdfPCell = new PdfPCell(new Phrase("(Other than <EBQ> User Approval Up to Operating Head is required)", _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfTable.AddCell(_pdfPCell);
                _pdfTable.CompleteRow();

            }
        }
        #endregion

        #region MIS Report

        public ActionResult MISReport()
        {
            return View("MISReport");
        }

        [HttpGet]
        public JsonResult MISReportSearch(string TransectionType, string AssetType, DateTime FromDate, DateTime ToDate, string Plant, int page)
        {
            try
            {
                //int page = 1;
                int pageSize = 150;

                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                List<AT_AssetTransferMISReportViewModel> ilist = new List<AT_AssetTransferMISReportViewModel>();
                ilist = _IATService.MISReportSearch(TransectionType, AssetType, FromDate, ToDate, LoginCode, Plant);

                if (ilist != null && ilist.Count > 0)
                {
                    List<List<AT_AssetTransferMISReportViewModel>> paginatedData = new List<List<AT_AssetTransferMISReportViewModel>>();

                    int totalPages = (int)Math.Ceiling((double)ilist.Count / pageSize);

                    int skip = (page - 1) * pageSize;
                    var pageData = ilist.Skip(skip).Take(pageSize).ToList();

                    paginatedData.Add(pageData);

                    var Data = new
                    {
                        Pagedata = pageData,
                        TotalPages = totalPages
                    };

                    //string serializedData = JsonConvert.SerializeObject(Data);
                    sJSON = JsonConvert.SerializeObject(Data);
                    return Json(sJSON);
                }
                else
                {
                    // Handle the case where ilist is null or empty
                    return Json(new { error = "No data found" });
                }



                //return Json(ilist);

                //sJSON = JsonConvert.SerializeObject(ilist);
                //return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }

        }

        public JsonResult ReportMISExcel(string TransectionType, string AssetType, DateTime FromDate, DateTime ToDate, string Plant)
        {
            try
            {
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                var ilist = _IATService.MISReportSearch(TransectionType, AssetType, FromDate, ToDate, LoginCode, Plant);

                var filename = "MISReport.xlsx";
                //string _Path = Server.MapPath("~/Uploads/AT/MISReport/");
                string _Path = Path.Combine(serverpath.getFileUploadPath(), "AT", "MISReport");

                if (!Directory.Exists(_Path))
                {
                    Directory.CreateDirectory(_Path);
                }

                //var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //var relativePath = "Uploads/AT/MISReport/";
                //var fullPath = Path.Combine(baseDirectory, relativePath);
                var ExcelPath = Path.Combine(_Path, filename);



                DataTable dataTable = ConvertToDataTable(ilist);

                MapHeaderNames(dataTable);
                if (System.IO.File.Exists(ExcelPath))
                {

                    System.IO.File.Delete(ExcelPath);
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("MIS_Report");

                    // Write data to worksheet
                    int rowCount = 1;

                    // Write column headers
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cell(rowCount, i + 1).Value = dataTable.Columns[i].ColumnName;

                        string columnName = dataTable.Columns[i].ColumnName;

                        if (columnName == "Sr.No." || columnName == "Request No." || columnName == "Fully/Partial" || columnName == "Qty")
                        {
                            worksheet.Column(i + 1).Width = 10;
                            //worksheet.Cell(1, i + 1).Value = columnName;
                            worksheet.Column(i + 1).Style.NumberFormat.Format = "0";
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue; // Set background color
                        }
                        else if (columnName == "Plant/HO/RO/ZO" || columnName == "Request Date" || columnName == "Transaction Type" ||
                                 columnName == "Asset Type" || columnName == "Asset Code" || columnName == "Vendor Code" ||
                                 columnName == "Po. No." || columnName == "Po. Date" || columnName == "Invoice No." ||
                                 columnName == "Invoice Date" || columnName == "Original Cost" || columnName == "Depreciation" ||
                                 columnName == "Net Block" || columnName == "Basic Value" || columnName == "Invoice Value" ||
                                 columnName == "New Asset Code" || columnName == "EODC Clearance" || columnName == "Transferee Type")
                        {
                            worksheet.Column(i + 1).Width = 20;
                            //worksheet.Cell(1, i + 1).Value = columnName;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        }
                        else if (columnName == "Requested by" || columnName == "Operation" || columnName == "Division" ||
                                 columnName == "Department" || columnName == "Transferee User" || columnName == "Vendor Name" || columnName == "Status")
                        {
                            worksheet.Column(i + 1).Width = 30;
                            //worksheet.Cell(1, i + 1).Value = columnName;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        }
                        else
                        {
                            worksheet.Column(i + 1).Width = 80;
                            //worksheet.Cell(1, i + 1).Value = columnName;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        }

                    }

                    // Increment row count for data rows
                    rowCount++;

                    // Write data rows
                    foreach (DataRow row in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            //worksheet.Cell(rowCount, i + 1).Value = (XLCellValue)row[i]; // Write cell value from DataRow
                            var cellValue = row[i] != DBNull.Value ? row[i].ToString() : string.Empty;
                            worksheet.Cell(rowCount, i + 1).Value = cellValue;
                        }
                        rowCount++;
                    }

                    // Format date columns
                    //var dateColumns = new[] { "Request Date", "Po. Date", "Invoice Date" }; // Adjust property names accordingly
                    //foreach (var columnName in dateColumns)
                    //{
                    //    var columnIndex = Array.IndexOf(dateColumns, columnName) + 1;
                    //    if (columnIndex > 0)
                    //    {
                    //        worksheet.Column(columnIndex).Style.NumberFormat.Format = "dd-MMM-yyyy";
                    //    }
                    //}

                    workbook.SaveAs(ExcelPath);
                }




                return Json(filename);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new { Error = ex.Message });
            }
        }

        public static void MapHeaderNames(DataTable dataTable)
        {
            // Define a dictionary for mapping header names
            Dictionary<string, string> headerMappings = new Dictionary<string, string>()
    {
         { "SRNO", "Sr.No.: Serial number" },
        { "Plant", "Plant/HO/RO/ZO: Location identifiers (Plant, Head Office, Regional Office, Zonal Office)" },
        { "TRAN_NO", "Request No.: Request number" },
        { "TRAN_DATE", "Request Date: Date of the request" },
        { "TRAN_TYPE", "Transaction Type: Type of transaction" },
        { "ASSET_TYPE", "Asset Type: Type of asset" },
        { "TRANSFREE_TYPE", "Transferee Type: Type of transferee" },
        { "TRANSFEROR", "Requested by: Person who made the request" },
        { "OPERATION", "Operation: Operation related to the request" },
        { "DIVISION", "Division: Division associated with the asset" },
        { "DEPARTMENT", "Department: Department associated with the asset" },
        { "TRANSFREE", "Transferee User: User to whom the asset is being transferred" },
        { "ASSET_CODE", "Asset Code: Code of the asset" },
        { "ASSET_CLS_DESC", "Asset Class Description: Description of the asset class" },
        { "DESCRIPTION", "Asset Description: Description of the asset" },
        { "ASSETMAIN_NO_TEXT", "Asset Main Text: Main text associated with the asset" },
        { "VENDOR_CODE", "Vendor Code: Code of the vendor" },
        { "VENDER_NAME", "Vendor Name: Name of the vendor" },
        { "PO_NO", "Po. No.: Purchase order number" },
        { "PO_DATE", "Po. Date: Date of the purchase order" },
        { "INV_NO", "Invoice No.: Invoice number" },
        { "INV_DATE", "Invoice Date: Date of the invoice" },
        { "FULLY_PARTIAL", "Fully/Partial: Indicates whether the transaction is fully or partially completed" },
        { "QTY", "Qty: Quantity of the asset" },
        { "CURRENT_LOCATION", "Current Location: Current location of the asset" },
        { "NEW_LOCATION", "New Location: New location of the asset (if applicable)" },
        { "ORIGINAL_COST", "Original Cost: Original cost of the asset" },
        { "DEPRECIATION", "Depreciation: Accumulated depreciation of the asset" },
        { "NET_BLOCK", "Net Block: Net block value of the asset" },
        { "BASIC_PRICE", "Basic Value: Basic value in Rupees" },
        { "INVOICE_VALUE", "Invoice Value: Invoice value in Rupees" },
        { "NEW_ASSET_CODE", "New Asset Code: New code assigned to the asset (if applicable)" },
        { "STATUS", "Status: Status of the transaction or asset" },
        { "EODC_CLEAREANSE", "EODC Clearance: End of depreciable cost clearance status" }

    };

            // Iterate through columns and update their names based on the mappings
            foreach (DataColumn column in dataTable.Columns)
            {
                if (headerMappings.ContainsKey(column.ColumnName))
                {
                    // Remove text after the colon (":")
                    int colonIndex = headerMappings[column.ColumnName].IndexOf(':');
                    if (colonIndex != -1)
                    {
                        column.ColumnName = headerMappings[column.ColumnName].Substring(0, colonIndex);
                    }
                }
            }
        }

        static DataTable ConvertToDataTable<T>(List<T> models)
        {

            DataTable dataTable = new DataTable(typeof(T).Name);

            System.Reflection.PropertyInfo[] Props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (System.Reflection.PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in models)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }



        [HttpGet]
        public JsonResult GetPlanForMISReport()
        {
            LibResult res = new LibResult();
            try
            {
                res = _IATService.GetPlanForMISReport();
                sJSON = JsonConvert.SerializeObject(res);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }


        }


        #endregion

        #region Manage Asset Transfer By Admin

        public ActionResult ManageAssetTransferByAdmin()
        {
            return View("ManageAssetTransferbyAdmin");
        }

        [HttpGet]
        public JsonResult ManageAssetTransfer(string flag)
        {
            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            ilist = _IATService.ManageAssetTransfer(LoginCode, flag);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }

        public ActionResult ManageAssetTransferbyAdminEdit(int id, int APPAUTHSRNO)
        {

            ViewBag.APPAUTHSRNO = APPAUTHSRNO;
            AT_ASSSET_TRANSFER_HEADER PHVM = _IATService.CapitalizedEdit(id);

            return View(PHVM);
        }

        public ActionResult AuthorityUpdate(int id)
        {
            try
            {
                AT_APPROVAL_AUTHORITY PHVM = _IATService.AuthEdit(id);

                return View(PHVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult AuthoritySave(int Empcode, string EmpName, int srno, string Department)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            res = _IATService.AuthoritySave(Empcode, EmpName, srno, Department, LoginCode);
            sJSON = JsonConvert.SerializeObject(res);
            return Json(sJSON);
        }

        [HttpGet]
        public JsonResult ManageRequestor_Detail(string AssetType, string Transferor, int AddedBy)
        {

            try
            {
                int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
                res = _IATService.ManageRequestor_Detail(AssetType, Transferor, AddedBy, LoginCode);
                sJSON = JsonConvert.SerializeObject(res.resultObject);
                return Json(sJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw ex;
            }
        }

        #endregion

        //SR109866-CR7276

        #region Capitalized Asset transfer Report
        public ActionResult CapitalizeAssetTransferReport()
        {
            return View("CapitalizeAssetTransferReport");
        }

        [HttpGet]
        public JsonResult GetCapitalizeAssetTransferReportList(string flag)
        {
            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            ilist = _IATService.GetCapitalizeAssetTransferReportList(LoginCode, flag);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);
        }
        [HttpPost]
        public JsonResult CapitalizeAssetTransferReportListSearch(string TranjectionType, /*string AssetType,*/ int TranjectionNo, string TransfereeUser, string Status, string flag)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();
            ilist = _IATService.CapitalizeAssetTransferReportListSearch(TranjectionType,/* AssetType,*/ TranjectionNo, TransfereeUser, Status, LoginCode, flag);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);

        }

        [HttpPost]
        public JsonResult CapitalizeAssetTransferReportStatusSearch(string Status, string flag)
        {
            int LoginCode = int.Parse(_sessionService.Get<string>("userID").ToString());
            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();
            ilist = _IATService.CapitalizeAssetTransferReportStatusSearch(Status, LoginCode, flag);
            sJSON = JsonConvert.SerializeObject(ilist);
            return Json(sJSON);

        }
        #endregion
        //SR109866-CR7276

    }
}