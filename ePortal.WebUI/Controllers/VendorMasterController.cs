using Microsoft.AspNetCore.Mvc;
using ePortal.Persistence.Admin.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using System.Data;
using ePortal.ViewModels.APPX.VendorMaster;
using ePortal.Persistence;
using System.Text;
using System.Text.Json;
using ePortal.Application.APPX.Contracts;
using System.Web;
using System.Reflection;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic.FileIO;
using System.Net;

namespace ePortal.Controllers
{
    [CSPFilter]
    public class VendorMasterController : Controller
    {
        private readonly IVendorMaster VMOBJ;
        private readonly IVendorMasterService VMOBJService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<VendorMasterController> _logger;
        //private readonly string filePath = serverpath.getFileUploadPath() + "VendorMaster";
        private readonly string filePath = Path.Combine(serverpath.getFileUploadPath(), "VendorMaster");
        private readonly int SPLAPPROVAL = 0;
        private const string WithHoldingTaxData = "withholdingtax_list";
        private readonly string _userId;

        public VendorMasterController(IVendorMaster _VMOBJ, IVendorMasterService _VMOBJService, ISessionService sessionService, ILogger<VendorMasterController> logger)
        {
            VMOBJ = _VMOBJ;
            _sessionService = sessionService;
            VMOBJService = _VMOBJService;
            _logger = logger;
            _userId = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
        }
        public ActionResult VendorMasterForm()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            VendorMasterViewModel model = VMOBJService.GetWithholdingData();
            TempData["WithHoldingTaxData"] = string.Empty;
            return View(model);
        }

        [HttpPost]
        public JsonResult VendorAccGroupChanged([FromBody] VendorFormViewModel model)
        {
            string requestType = model.CreateVendor ? "1" : "2";
            model.ShowAccGroup = model.CreateVendor;
            model.ShowAccGroupList = model.CreateVendor;
            model.AccGroupLabel = model.CreateVendor ? "Vendor Account Group" : "";

            if (model.GeneralPurchase)
                model.VendorAccountGroup = "5AG1";
            else if (model.GPImport)
                model.VendorAccountGroup = "5AG2";
            else if (model.GroupCompanies)
                model.VendorAccountGroup = "5AG3";
            else if (model.BOPVendor)
                model.VendorAccountGroup = "5AGB";
            else if (model.ContractualEmp)
                model.VendorAccountGroup = "5AGA";

            model.ShowVendorCode = requestType == "2" ||
                (requestType == "1" && (model.VendorAccountGroup == "5AGA" || model.VendorAccountGroup == "5AGB"));
            return Json(model);
        }

        [HttpPost]
        public IActionResult NextToContinue([FromBody] VendorViewModel model)
        {
            string VendorAccGrp = model.VendorAccGrp;
            string RequestType = model.RequestType;
            string VENDORCODE = model.VENDORCODE;
            string csvfile = string.Empty;

            string fullpath = model.CsvFileName;
            if (!string.IsNullOrEmpty(fullpath))
            {
                string[] strcsvfile = fullpath.Split('\\');
                csvfile = strcsvfile[^1];
            }


            //RequestType = Request.Form["UpdateVendor"] == "true" ? "1" : "2";
            RequestType = Request.Form["RequestType"];
            if (RequestType == "UpdateVendor")
                RequestType = "2"; // Update
            else
                RequestType = "1"; // Create

            if (Request.Form["rdogeneralpurchase"] == "true") VendorAccGrp = "5AG1";
            else if (Request.Form["rdogpimport"] == "true") VendorAccGrp = "5AG2";
            else if (Request.Form["rdogroupcompanies"] == "true") VendorAccGrp = "5AG3";
            else if (Request.Form["rdobopvendor"] == "true") VendorAccGrp = "5AGB";
            else if (Request.Form["rdocontractualemp"] == "true") VendorAccGrp = "5AGA";

            if (string.IsNullOrEmpty(RequestType))
                return Json(new { success = false, message = "Select Request Type" });

            if (string.IsNullOrEmpty(VendorAccGrp) && RequestType == "1")
                return Json(new { success = false, message = "Select Vendor Account Group" });

            if (RequestType == "2" && string.IsNullOrEmpty(VENDORCODE))
                return Json(new { success = false, message = "Vendor Code is mandatory field" });

            // Proceed with further logic
            return Json(new { success = true, smssage = "" });

        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { status = 0, message = "Invalid file content" });

            if (string.IsNullOrEmpty(filePath))
                return Json(new { status = 0, message = "Path not found" });

            string strFileName = Path.GetFileName(file.FileName);
            string fileExt = Path.GetExtension(strFileName).ToLower();

            if (fileExt != ".csv")
                return Json(new { status = 0, message = "Invalid file format. Please select a valid CSV file" });

            if (file.Length > 3145728) // 3MB limit
                return Json(new { status = 0, message = "File exceeds maximum size limit" });

            try
            {
                string userId = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
                string modifiedName = Path.GetFileNameWithoutExtension(strFileName) + "#" +
                                      DateTime.Now.ToString("ddMMyyyyHHmmss") + fileExt;

                string fullPath = Path.Combine(filePath, modifiedName);

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return Json(new { status = 1, filename = modifiedName, fullpath = fullPath });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Json(new { status = 0, message = ex.Message + " Permission to upload file denied" });
            }
        }

        public async Task<UploadResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new UploadResult { Status = false, Emsg = "Invalid file content" };
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return new UploadResult { Status = false, Emsg = "Path not found" };
            }

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string fileExt = Path.GetExtension(file.FileName);
            if (fileExt.ToLower() != ".pdf")
            {
                return new UploadResult { Status = false, Emsg = "Invalid file format. Only PDF allowed." };
            }

            if (file.Length > 1048576) // 1MB limit
            {
                return new UploadResult { Status = false, Emsg = "File exceeds maximum size limit (1MB)" };
            }

            try
            {
                string modifiedName = Path.GetFileNameWithoutExtension(file.FileName)
                                      + DateTime.Now.ToString("ddMMyyyyHHmmss")
                                      + fileExt;

                string fullPath = Path.Combine(filePath, modifiedName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new UploadResult { Status = true, Emsg = "File uploaded successfully", Filename = modifiedName };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new UploadResult { Status = false, Emsg = "Permission denied: " + ex.Message };
            }
            catch (Exception ex)
            {
                return new UploadResult { Status = false, Emsg = "Error: " + ex.Message };
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidateCSV([FromBody] ParseCsvRequest request)
        {
            var fullPath = Path.Combine(filePath, request.FileName);

            if (request == null) return Json(new { Status = 0, Message = "Request is null." });
            if (!System.IO.File.Exists(fullPath)) return Json(new { Status = 0, Message = "CSV file not found." });

            var lines = System.IO.File.ReadAllLines(fullPath, Encoding.UTF8);
            if (lines.Length < 66) return Json(new { Status = 0, Message = $"CSV expected at least 66 lines, found {lines.Length}" });

            try
            {
                Vendor vendor = new Vendor();
                vendor = await VMOBJService.GetCsvValidation(request, fullPath);

                string[] strstatus = vendor.Result.Split(new char[] { '#' }, 2);
                string resultmsg = strstatus.Length > 0 ? strstatus[0] : "0";
                string errmsg = strstatus.Length > 1 ? strstatus[1] : "Unexpected parse";

                return Json(new
                {
                    Status = resultmsg == "1" ? 1 : 0,
                    Message = errmsg,
                    Data = vendor
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new { Status = 0, message = "Parsing/validation failed: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetVendorListMatchDetails([FromBody] VendorMatchRequest request)
        {
            if (request == null) return BadRequest("Request body is required.");

            try
            {
                var result = await VMOBJService.GetVendorListMatchDetailsAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return StatusCode(500, new { error = "An error occurred while processing the request." });
            }
        }

        [HttpGet("getdetailbehalfofvendorcode")]
        public async Task<IActionResult> GetDetailBehalfOfVendorCode([FromQuery] string vendorCode)
        {
            if (string.IsNullOrEmpty(vendorCode))
                return BadRequest("vendorCode is required");

            try
            {
                var dto = await VMOBJService.GetDetailBehalfOfVendorCodeAsync(vendorCode);

                if (dto == null)
                    return NotFound();

                return Ok(dto);
            }
            catch (Exception ex)
            {
                // log error only
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return StatusCode(500, new { error = "An error occurred while processing the request." });
            }
        }

        private string CleanAndTrim(string input)
        {
            if (input == null) return string.Empty;
            return input.Replace('"', ' ').Trim();
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailsfromCSV([FromQuery] string csvfilename)
        {
            if (string.IsNullOrEmpty(csvfilename))
                return BadRequest("csvfilename is required");

            var fullPath = Path.Combine(filePath, csvfilename);
            if (!System.IO.File.Exists(fullPath))
                return NotFound("CSV file not found");
            try
            {
                var result = await VMOBJService.ParseFromFileAsync(fullPath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return StatusCode(500, new { error = "An error occurred while processing the request." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovalAuthority([FromQuery] string bankAccNo, [FromQuery] string panNumber)
        {
            try
            {
                var table = await VMOBJService.GetApprovalAuthorityTableAsync(bankAccNo, panNumber);

                var forwardList = table.AsSelectList("EMPLOYEE", "DEPARTMENTHEAD", true, "--select--", "");

                return Ok(forwardList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return StatusCode(500, new { error = "An error occurred while processing the request." });
            }
        }

        private List<Dictionary<string, string>> GetList()
        {
            var json = TempData["WithHoldingTaxData"]?.ToString() ?? string.Empty;// _sessionService.Get<string>(WithHoldingTaxData)?.ToString() ?? string.Empty;
            TempData.Keep("WithHoldingTaxData");
            if (string.IsNullOrEmpty(json)) return new List<Dictionary<string, string>>();
            return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json) ?? new List<Dictionary<string, string>>();
        }

        private void SaveList(List<Dictionary<string, string>> list)
        {
            TempData["WithHoldingTaxData"] = JsonSerializer.Serialize(list);
            //TempData.Keep("WithHoldingTaxData");
            //_sessionService.Set(WithHoldingTaxData, JsonSerializer.Serialize(list));
            //HttpContext.Session.SetString(WithHoldingTaxData, JsonSerializer.Serialize(list));
        }

        // fetch list (used by client to populate grid initially)
        [HttpGet]
        public IActionResult GetListAjax()
        {
            var list = GetList();
            return Json(new { items = list });
        }

        // add item (no view model)
        [HttpPost]
        public IActionResult AddAjax([FromBody] WithholdingTaxModel obj)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(obj.Withholding_Tax_Type))
                errors.Add("Select Holding Tax Type");

            if (string.IsNullOrWhiteSpace(obj.Recipient_Type))
                errors.Add("Select Recipient Type");

            if (!obj.Liable)
                errors.Add("Check Liable");

            if (obj.Date_On_Which_Exemption_Begins.HasValue && obj.Date_On_Which_Exemption_Ends.HasValue)
            {
                if (obj.Date_On_Which_Exemption_Begins > obj.Date_On_Which_Exemption_Ends)
                    errors.Add("Exemption ends date should be after exemption begin date");
            }

            if (errors.Any())
                return BadRequest(new { errors });

            var list = GetList();

            var item = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = (list.Count == 0 ? 1 :
                    (list.Max(x => int.TryParse(x.GetValueOrDefault("Id"), out var v) ? v : 0) + 1)).ToString(),
                ["Withholding_Tax_Type"] = obj.Withholding_Tax_Type?.Trim() ?? "",
                ["Withholding_Tax_Code"] = obj.Withholding_Tax_Code?.Trim() ?? "",
                ["Liable"] = obj.Liable ? "Y" : "N",
                ["Recipient_Type"] = obj.Recipient_Type?.Trim() ?? "",
                ["Withholding_Tax_Id_No"] = string.IsNullOrWhiteSpace(obj.Withholding_Tax_Id_No) ? "" : obj.Withholding_Tax_Id_No.Trim().ToUpper(),
                ["Exemption_Certi_No"] = obj.Exemption_Certi_No?.Trim() ?? "",
                ["Exemption_Rate"] = obj.Exemption_Rate?.ToString() ?? "",
                ["Date_On_Which_Exemption_Begins"] = obj.Date_On_Which_Exemption_Begins?.ToString("dd.MM.yyyy") ?? "",
                ["Date_On_Which_Exemption_Ends"] = obj.Date_On_Which_Exemption_Ends?.ToString("dd.MM.yyyy") ?? "",
                ["Reason_For_Exemption"] = obj.Reason_For_Exemption?.Trim() ?? ""
            };

            list.Add(item);
            SaveList(list);
            //TempData["WithHoldingTaxData"] = JsonSerializer.Serialize(list);
            //TempData.Keep();
            //_sessionService.Set(WithHoldingTaxData, JsonSerializer.Serialize(list));

            return Json(new { items = list });
        }

        [HttpPost]
        public IActionResult ClearAjax()
        {
            SaveList(new List<Dictionary<string, string>>());
            return Json(new { items = new List<Dictionary<string, string>>() });
        }

        [HttpGet]
        public IActionResult DeleteAjax([FromQuery] string id)
        {
            if (!int.TryParse(id, out var intId)) return BadRequest(new { errors = new[] { "Invalid id" } });

            var list = GetList();
            var item = list.FirstOrDefault(x => x.TryGetValue("Id", out var s) && int.TryParse(s, out var v) && v == intId);
            if (item == null) return NotFound(new { errors = new[] { "Item not found" } });

            list.Remove(item);
            SaveList(list);
            return Json(new { items = list });
        }

        [HttpGet]
        public IActionResult GetWithholdingTaxCodes([FromQuery] string parentKey)
        {
            try
            {
                string ECODE = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
                string Error_msg = string.Empty;
                DataTable dt = _sessionService.Get<DataTable>("WTHTax");
                if (dt == null)
                {
                    DataSet DS = VMOBJ.GET_WHOLDINGTAXMASTER(ECODE, Error_msg);
                    if (DS != null && DS.Tables[0].Rows.Count > 0) { dt = DS.Tables[0]; }
                }

                if (!string.IsNullOrEmpty(parentKey))
                {
                    dt = dt.Select(" COMPONENTTYPE=6 And PARENTKEY='" + parentKey + "' ").CopyToDataTable();
                }

                var items = dt.AsSelectList("COMPONENTKI", "COMPONENT", true, "Select Tax Code", "");

                return Json(new { model = items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new { items = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitVendorData([FromForm] VendorMasterDetails vmd)
        {
            try
            {
                var rows = GetList(); // returns List<Dictionary<string,string>>
                if (vmd.chkWithHoldingTax && rows.Count == 0)
                    return Json(new { success = false, message = "Please add holding tax data" });

                vmd.E_INVOICEApplicable = 2;

                if (string.IsNullOrEmpty(vmd.csvfilename))
                {
                    return Json(new { success = false, message = "CSV file not provided" });
                }
                var fullPath = Path.Combine(filePath, vmd.csvfilename);
                string[] lines = System.IO.File.ReadAllLines(fullPath);
                if (lines == null || lines.Length != 66)
                    return Json(new { success = false, message = "CSV template must contain 66 lines" });

                MapCsvToVendorDetails(lines, vmd);

                // cleanup strings (your original cleanup)
                vmd.REFERENCE_DETAIL = CleanAndTrim(vmd.REFERENCE_DETAIL);
                vmd.I_REFERENCE_DETAIL = CleanAndTrim(vmd.I_REFERENCE_DETAIL);
                vmd.VENDORNAME1 = CleanAndTrim(vmd.VENDORNAME1);
                vmd.VENDORNAME2 = CleanAndTrim(vmd.VENDORNAME2);
                vmd.VENDORNAME3 = CleanAndTrim(vmd.VENDORNAME3);
                vmd.STREET1 = CleanAndTrim(vmd.STREET1);
                vmd.STREET2 = CleanAndTrim(vmd.STREET2);
                vmd.STREET3 = CleanAndTrim(vmd.STREET3);
                vmd.STREET4 = CleanAndTrim(vmd.STREET4);
                vmd.BANKADDRESS = CleanAndTrim(vmd.BANKADDRESS);
                vmd.BRANCHNAME = CleanAndTrim(vmd.BRANCHNAME);

                // derive other values (REQUESTTYPE, VENDORACCGRP etc.) from vmd.
                vmd.VENDORCODE = vmd.VENDORCODE ?? string.Empty;
                vmd.CSVATTACHMENT = vmd.csvfilename ?? "";
                vmd.REQUESTTYPE = vmd.REQUESTTYPE;
                vmd.VENDORACCGRP = vmd.VENDORACCGRP ?? "";

                string[] FORDWORDTOARR = vmd.FORWARDTO?.Split(new Char[] { '#' });
                if (FORDWORDTOARR?.Length > 1)
                {
                    vmd.DEPTHEAD = Convert.ToString(FORDWORDTOARR[0]);
                    vmd.DEPTHEADEMAIL = Convert.ToString(FORDWORDTOARR[1]);
                    vmd.DEPTHEADNAME = Convert.ToString(FORDWORDTOARR[2]);
                }

                if ((vmd.MSMEINFOSTATUS ?? "").ToUpper() == "NO")
                    vmd.MSMECATEGORY = vmd.MSMECERTIFICATION = vmd.MSMEFROM = vmd.MSMETO = vmd.MSMECITY = "";

                if ((vmd.LEI_APPLICABLE ?? "").ToUpper() == "NO")
                    vmd.LEINO = "";

                if (vmd.REQUESTTYPE == "1" && !(vmd.VENDORACCGRP == "5AGB" || vmd.VENDORACCGRP == "5AGA"))
                    vmd.VENDORCODE = string.Empty;

                vmd.REMARKS = vmd.REMARKS ?? string.Empty;
                vmd.SPLAPPROVAL = vmd.SPLAPPROVAL;

                // validate using your existing VENDORSUBMIT_VALIDATE method
                //string validateResult = VENDORSUBMIT_VALIDATE(vmd.VENDORACCGRP, vmd.IFSCCODE, vmd.REQUESTTYPE, vmd.PANNUMBER, "", "", vmd.MSMECERTIFICATION, "", vmd.MSMEINFOSTATUS, vmd.BANKACCNO, vmd.GSTIN, vmd.REMARKS, vmd.E_INVOICEApplicable, vmd.LEI_APPLICABLE);
                string validateResult = VENDORSUBMIT_VALIDATE(vmd);
                string[] resultmsg = (validateResult ?? "").Split('#');
                if (resultmsg.Length == 0 || resultmsg[0] != "1")
                    return Json(new { success = false, message = resultmsg.Length > 1 ? resultmsg[1] : "Validation failed" });

                if (vmd.HDFILE_REGCERTIFICATENO != null)
                {
                    var regCert = await UploadDocument(vmd.HDFILE_REGCERTIFICATENO);
                    if (!regCert.Status)
                        return Json(new { success = false, message = "Registration Certificate Attachment - " + regCert.Emsg });
                    vmd.FILE_REGCERTIFICATENO = regCert.Filename;
                }

                if (vmd.HDFILE_MANDATEFORM != null)
                {
                    var mandateForm = await UploadDocument(vmd.HDFILE_MANDATEFORM);
                    if (!mandateForm.Status)
                        return Json(new { success = false, message = "Mandate Form Attachment - " + mandateForm.Emsg });
                    vmd.FILE_MANDATEFORM = mandateForm.Filename;
                }

                if (vmd.HDFILE_CANCELCHEQUE != null)
                {
                    var cancelCheque = await UploadDocument(vmd.HDFILE_CANCELCHEQUE);
                    if (!cancelCheque.Status)
                        return Json(new { success = false, message = "Cancel Cheque Attachment - " + cancelCheque.Emsg });
                    vmd.FILE_CANCELCHEQUE = cancelCheque.Filename;
                }

                if (vmd.HDFILE_PANCARD != null)
                {
                    var panCard = await UploadDocument(vmd.HDFILE_PANCARD);
                    if (!panCard.Status)
                        return Json(new { success = false, message = "Pan Card Attachment - " + panCard.Emsg });
                    vmd.FILE_PANCARD = panCard.Filename;
                }

                if (vmd.HDFILE_MSMECERTIFICATE != null)
                {
                    var msmeCert = await UploadDocument(vmd.HDFILE_MSMECERTIFICATE);
                    if (!msmeCert.Status)
                        return Json(new { success = false, message = "MSME Certificate Attachment - " + msmeCert.Emsg });
                    vmd.FILE_MSMECERTIFICATE = msmeCert.Filename;
                }

                if (vmd.HDNFILE_GSTCERTIFICATE != null)
                {
                    var gstCert = await UploadDocument(vmd.HDNFILE_GSTCERTIFICATE);
                    if (!gstCert.Status)
                        return Json(new { success = false, message = "GST Certificate Attachment - " + gstCert.Emsg });
                    vmd.FILE_GSTCERTIFICATE = gstCert.Filename;
                }

                if (vmd.HDNFILE_LEI != null)
                {
                    var leiCert = await UploadDocument(vmd.HDNFILE_LEI);
                    if (!leiCert.Status)
                        return Json(new { success = false, message = "LEI Certificate Attachment - " + leiCert.Emsg });
                    vmd.FILE_LEICERTIFICATE = leiCert.Filename;
                }

                if (vmd.HDNFILE_EINVOICE != null)
                {
                    var einvoice = await UploadDocument(vmd.HDNFILE_EINVOICE);
                    if (!einvoice.Status)
                        return Json(new { success = false, message = "E-Invoice Attachment - " + einvoice.Emsg });
                    vmd.FILE_EINVOICE = einvoice.Filename;
                }

                if (vmd.HDFILE_conflictCERTIFICATE != null)
                {
                    var conflictCert = await UploadDocument(vmd.HDFILE_conflictCERTIFICATE);
                    if (!conflictCert.Status)
                        return Json(new { success = false, message = "Conflict of Interest Attachment - " + conflictCert.Emsg });
                    vmd.FILE_CONFLICTCERTIFICATE = conflictCert.Filename;
                }


                vmd.REQECODE = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
                string strresult = VMOBJ.VENDORMASTER_INFORMATION_INSERT(vmd);

                var strstatus = (strresult ?? "").Split('#');
                var resultMsgs = strstatus.Length > 0 ? strstatus[0] : "0";
                var errMsg = strstatus.Length > 1 ? strstatus[1] : "";
                var headerid = strstatus.Length > 2 ? strstatus[2] : "0";

                if (resultMsgs == "1")
                {

                    if (vmd.chkWithHoldingTax && headerid != "0" && rows != null && rows.Count > 0)
                    {
                        int ECODE = Convert.ToInt32(_sessionService.Get<string>("userID")?.ToString() ?? "0");
                        int r = 0;

                        foreach (var dict in rows)
                        {
                            // helper to get value or empty string if key missing
                            string Val(string key) => dict != null && dict.ContainsKey(key) ? dict[key] ?? string.Empty : string.Empty;

                            string type = Val("Withholding_Tax_Type");
                            string code = Val("Withholding_Tax_Code");
                            string liable = Val("Liable");
                            string recipient = Val("Recipient_Type");
                            string taxId = Val("Withholding_Tax_Id_No");
                            string exemptNo = Val("Exemption_Certi_No");
                            string exemptRate = Val("Exemption_Rate");
                            string exemptStart = Val("Date_On_Which_Exemption_Begins");
                            string exemptEnd = Val("Date_On_Which_Exemption_Ends");
                            string reason = Val("Reason_For_Exemption");

                            int lastFlag = (r == 0) ? 0 : 1;

                            VMOBJ.GET_WHOLDINGTAXSAVE(ECODE, headerid, type, code, liable, recipient, taxId,
                                exemptNo, exemptRate, exemptStart, exemptEnd, reason, lastFlag);

                            r++;
                        }
                        TempData["WithHoldingTaxData"] = string.Empty;
                    }

                    // SEND MAIL and return success
                    SendMailToDeptHead(vmd);
                    //return Json(new { success = true, message = "Your Request Submitted Successfully", redirect = Url.Action("ManageRequest", "Vendor") });

                    return Json(new { success = true, message = "Your Request Submitted Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = errMsg });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json(new { success = false, message = "An error occurred while processing." });
            }
        }
        private void MapCsvToVendorDetails(string[] lines, VendorMasterDetails vmd)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                var str = lines[i] ?? string.Empty;
                var items = str.Split(',');
                string cellvalue;
                if (items.Length == 2) cellvalue = items[1];
                else { items = items.Skip(1).ToArray(); cellvalue = string.Join(",", items); }
                cellvalue = cellvalue.Trim();

                switch (i)
                {
                    case 2: vmd.VENDORNAME1 = cellvalue.ToUpper(); break;
                    case 3: vmd.VENDORNAME2 = cellvalue.ToUpper(); break;
                    case 4: vmd.VENDORNAME3 = cellvalue.ToUpper(); break;
                    case 5: vmd.STREET1 = cellvalue.ToUpper(); break;
                    case 6: vmd.STREET2 = cellvalue.ToUpper(); break;
                    case 7: vmd.STREET3 = cellvalue.ToUpper(); break;
                    case 8: vmd.STREET4 = cellvalue.ToUpper(); break;
                    case 9: vmd.CITY = cellvalue.ToUpper(); break;
                    case 10: vmd.REGION = cellvalue.ToUpper(); break;
                    case 11: vmd.POSTALCODE = cellvalue; break;
                    case 12: vmd.COUNTRY = cellvalue.ToUpper(); break;
                    case 13: vmd.MOBILENO = cellvalue; break;
                    case 14: vmd.TELEPHONENO = cellvalue; break;
                    case 15: vmd.EMAIL1 = cellvalue.ToUpper(); break;
                    case 16: vmd.EMAIL2 = cellvalue.ToUpper(); break;
                    case 17: vmd.EMAIL3 = cellvalue.ToUpper(); break;
                    case 18: vmd.MSMEINFOSTATUS = cellvalue.ToUpper(); break;
                    case 19: vmd.MSMECATEGORY = cellvalue.ToUpper(); break;
                    case 20: vmd.MSMECERTIFICATION = cellvalue.ToUpper(); break;
                    case 21: vmd.MSMEFROM = cellvalue.ToUpper(); break;
                    case 22: vmd.MSMETO = cellvalue.ToUpper(); break;
                    case 23: vmd.MSMECITY = cellvalue.ToUpper(); break;
                    case 24: vmd.TYPE_OF_INDUSTRY = cellvalue.ToUpper(); break;
                    case 25: vmd.CLASSIFICATION_OF_YEAR = cellvalue.ToUpper(); break;
                    case 26: vmd.DATE_OF_CLASSIFICATION = cellvalue.ToUpper(); break;
                    case 27:
                        vmd.SERVICEAGENTGRP = cellvalue.ToUpper();
                        vmd.SERVICEAGENTGRP = vmd.SERVICEAGENTGRP.Length != 0
                            ? vmd.SERVICEAGENTGRP.PadLeft(4, '0')
                            : vmd.SERVICEAGENTGRP;
                        break;
                    case 29: vmd.BANKACCNO = cellvalue; break;
                    case 30: vmd.BANKNAME = cellvalue.ToUpper(); break;
                    case 31: vmd.BANKADDRESS = cellvalue.ToUpper(); break;
                    case 32: vmd.BANKCITY = cellvalue.ToUpper(); break;
                    case 33: vmd.BANKSTATE = cellvalue.ToUpper(); break;
                    case 34: vmd.BANKCOUNTRY = cellvalue.ToUpper(); break;
                    case 35: vmd.BRANCHNAME = cellvalue.ToUpper(); break;
                    case 36: vmd.TYPEOFACCOUNT = cellvalue.ToUpper(); break;
                    case 37: vmd.IFSCCODE = cellvalue.ToUpper(); break;
                    case 38: vmd.SWIFTCODE = cellvalue.ToUpper(); break;
                    case 39: vmd.IBANNO = cellvalue.ToUpper(); break;
                    case 40: vmd.BANKCATEGORY = cellvalue.ToUpper(); break;
                    case 41: vmd.SCHEMAGROUP = cellvalue.ToUpper(); break;
                    case 42: vmd.ORDERCURRENCY = cellvalue.ToUpper(); break;
                    case 43: vmd.REFERENCE_DETAIL = cellvalue.ToUpper(); break;
                    case 45: vmd.I_BANK_KEY = cellvalue.ToUpper(); break;
                    case 46: vmd.I_BANKACCNO = cellvalue.ToUpper(); break;
                    case 47: vmd.I_BANKNAME = cellvalue; break;
                    case 48: vmd.I_BANKADDRESS = cellvalue; break;
                    case 49: vmd.I_BANKCITY = cellvalue; break;
                    case 50: vmd.I_BANKSTATE = cellvalue; break;
                    case 51: vmd.I_BANKCOUNTRY = cellvalue; break;
                    case 52: vmd.I_BRANCHNAME = cellvalue; break;
                    case 53: vmd.I_TYPEOFACCOUNT = cellvalue; break;
                    case 54: vmd.I_SWIFTCODE = cellvalue; break;
                    case 55: vmd.I_IBANNO = cellvalue; break;
                    case 56: vmd.I_BANKCATEGORY = cellvalue; break;
                    case 57: vmd.I_REFERENCE_DETAIL = cellvalue.ToUpper(); break;
                    case 59: vmd.PANNUMBER = cellvalue.ToUpper(); break;
                    case 61: vmd.GSTIN = cellvalue; break;
                    case 62: vmd.GSTCLASSIFICATION = cellvalue; break;
                    case 63:
                        if (cellvalue.ToUpper() == "YES") vmd.E_INVOICEApplicable = 1;
                        else if (cellvalue.ToUpper() == "NO") vmd.E_INVOICEApplicable = 0;
                        break;
                    case 64: vmd.LEI_APPLICABLE = cellvalue; break;
                    case 65: vmd.LEINO = cellvalue; break;
                }
            }
        }
        private string VENDORSUBMIT_VALIDATE(VendorMasterDetails vmd)
        {
            string ERRORLIST = "";
            string RESULT = "1";
            bool disclaimer = vmd.chkdeclaration;

            string FILEREGCERTIFICATENO = vmd.HDFILE_REGCERTIFICATENO?.FileName;
            string FILEMANDATEFORM = vmd.HDFILE_MANDATEFORM?.FileName;
            string FILECANCELCHEQUE = vmd.HDFILE_CANCELCHEQUE?.FileName;
            string FILEPANCARD = vmd.HDFILE_PANCARD?.FileName;
            string FILEMSMECERTIFICATE = vmd.HDFILE_MSMECERTIFICATE?.FileName;
            string FILEGSTCERTIFICATE = vmd.HDNFILE_GSTCERTIFICATE?.FileName;
            string FILEEINVOICE = vmd.HDNFILE_EINVOICE?.FileName;
            string FILELEI = vmd.HDNFILE_LEI?.FileName;
            string FILEconflictCERTIFICATE = vmd.HDFILE_conflictCERTIFICATE?.FileName;

            if (vmd.REQUESTTYPE == "1")
            {
                if (string.IsNullOrWhiteSpace(FILEREGCERTIFICATENO) &&
                    (vmd.VENDORACCGRP == "5AG1" || vmd.VENDORACCGRP == "5AG2" ||
                     vmd.VENDORACCGRP == "5AG3" || vmd.VENDORACCGRP == "5AGB"))
                {
                    ERRORLIST += "<li>Registration Certificate No. attachment is mandatory field</li>";
                    RESULT = "0";
                }
                if (vmd.VENDORACCGRP == "5AGB" && string.IsNullOrWhiteSpace(FILEconflictCERTIFICATE))
                {
                    ERRORLIST += "<li>Conflict of interest Certificate attachment is mandatory field</li>";
                    RESULT = "0";
                }
            }

            if (vmd.MSMEINFOSTATUS?.ToUpper() == "YES" && string.IsNullOrWhiteSpace(FILEMSMECERTIFICATE))
            {
                ERRORLIST += "<li>MSME certificate attachment is mandatory field</li>";
                RESULT = "0";
            }

            if (!string.IsNullOrWhiteSpace(vmd.IFSCCODE))
            {
                if (string.IsNullOrWhiteSpace(FILEMANDATEFORM) &&
                    (vmd.VENDORACCGRP == "5AG1" || vmd.VENDORACCGRP == "5AG2" ||
                     vmd.VENDORACCGRP == "5AG3" || vmd.VENDORACCGRP == "5AGB"))
                {
                    ERRORLIST += "<li>Mandate form attachment is mandatory field</li>";
                    RESULT = "0";
                }
                if (string.IsNullOrWhiteSpace(FILECANCELCHEQUE) &&
                    (vmd.VENDORACCGRP == "5AG1" || vmd.VENDORACCGRP == "5AGA" ||
                     vmd.VENDORACCGRP == "5AG3" || vmd.VENDORACCGRP == "5AGB"))
                {
                    ERRORLIST += "<li>Cancel cheque attachment is mandatory field</li>";
                    RESULT = "0";
                }
            }

            if (!string.IsNullOrWhiteSpace(vmd.PANNUMBER) && string.IsNullOrWhiteSpace(FILEPANCARD))
            {
                ERRORLIST += "<li>Pan Card attachment is mandatory field</li>";
                RESULT = "0";
            }

            if (!string.IsNullOrWhiteSpace(vmd.GSTIN) && string.IsNullOrWhiteSpace(FILEGSTCERTIFICATE))
            {
                ERRORLIST += "<li>GSTIN certificate attachment is mandatory field</li>";
                RESULT = "0";
            }

            if (vmd.E_INVOICEApplicable == 0 && string.IsNullOrWhiteSpace(FILEEINVOICE))
            {
                ERRORLIST += "<li>E-Invoice attachment is mandatory field</li>";
                RESULT = "0";
            }

            if (vmd.LEI_APPLICABLE?.ToUpper() == "YES" && string.IsNullOrWhiteSpace(FILELEI))
            {
                ERRORLIST += "<li>LEI attachment is mandatory field</li>";
                RESULT = "0";
            }

            // Check duplicate file names
            string[] arr = { FILEREGCERTIFICATENO, FILEMANDATEFORM, FILECANCELCHEQUE, FILEPANCARD, FILEMSMECERTIFICATE, FILEGSTCERTIFICATE };
            arr = arr.Where(val => !string.IsNullOrWhiteSpace(val)).ToArray();
            string[] arrdistinct = arr.Distinct().ToArray();
            if (arrdistinct.Length != arr.Length)
            {
                ERRORLIST += "<li>Attachment Document could not be Same</li>";
                RESULT = "0";
            }

            if (!disclaimer)
            {
                ERRORLIST += "<li>Declaration is mandatory</li>";
                RESULT = "0";
            }

            if (string.IsNullOrWhiteSpace(vmd.FORWARDTO))
            {
                ERRORLIST += "<li>Forward to is mandatory</li>";
                RESULT = "0";
            }

            if (string.IsNullOrWhiteSpace(vmd.REMARKS))
            {
                ERRORLIST += "<li>Remark is mandatory</li>";
                RESULT = "0";
            }

            ERRORLIST = "<ul>" + ERRORLIST + "</ul>";
            return RESULT + "#" + ERRORLIST;
        }
        private void SendMailToDeptHead(VendorMasterDetails vmd)
        {
            string UserId = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
            string UserName = _sessionService.Get<string>("userName")?.ToString() ?? string.Empty;
            string deptHeadName = vmd.DEPTHEADNAME ?? "";
            string deptHeadEmail = vmd.DEPTHEADEMAIL ?? "";
            string vendorName = vmd.VENDORNAME1 + " " + vmd.VENDORNAME2 + " " + vmd.VENDORNAME3;
            string requestType = vmd.REQUESTTYPE == "1" ? "Create Vendor" : "Update Vendor";

            if (!string.IsNullOrEmpty(deptHeadEmail))
            {
                commanEmail sendMail = new commanEmail
                {
                    MailFrom = "portal.admin@honda.hmsi.in",
                    MailTo = deptHeadEmail
                };

                string strSubject = "Pending for the Vendor Master Approval";

                string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;padding-top:0px;font-family:Times New Roman;font-size:14px;line-height:21px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Vendor Master Request</b></div>"
                                + "<table cellpadding=0 cellspacing=0 border=0 width=700px style='font-size:14px;' >"
                                + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + deptHeadName + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                + "<tr><td colspan=2>&nbsp;&nbsp;Vendor master request is pending at your end for approval. Following are the details:</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                strBody += "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                         + "<tr><td width='100' style='background-color:#CAE8EA;font-size:12px;font-weight:bold;'>Requestor</td>"
                         + "<td width='200' style='background-color:white;font-size:12px;'>&nbsp;" + UserName + "[" + UserId + "]" + "</td>"
                         + "<td width='100' style='background-color:#CAE8EA;font-size:12px;font-weight:bold;'>Request Date</td>"
                         + "<td width='300' style='background-color:white;font-size:12px;'>&nbsp;" + DateTime.Now.ToString("dd-MMM-yyyy") + "</td></tr>"
                         + "<tr><td width='100' style='background-color:#CAE8EA;font-size:12px;font-weight:bold;'>Request Type</td>"
                         + "<td width='200' style='background-color:white;font-size:12px;'>&nbsp;" + requestType + "</td>"
                         + "<td width='100' style='background-color:#CAE8EA;font-size:12px;font-weight:bold;'>Vendor Name</td>"
                         + "<td width='300' style='background-color:white;font-size:12px;'>&nbsp;" + vendorName + "</td></tr></table>";

                strBody += "<table cellpadding=0 cellspacing=0 border=0 style='font-size:14px;'>"
                         + "<tr><td><br/>&nbsp;&nbsp;Please login <a href=" + serverpath.getServerPath() + "index.aspx> E-Portal</a> for approval process.</td></tr>"
                         + "<tr><td><br/><b>&nbsp;&nbsp;Thank You</b><br/></td></tr>"
                         + "<tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                         + "<tr><td>&nbsp;&nbsp;Portal Admin<br/></td></tr>"
                         + "<tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>"
                         + "</table></div>";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
        }


        #region ManageRequest
        public IActionResult ManageRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetVendorRequest(VendorMasterFilter filter)
        {
            var result = await VMOBJService.GetVendorRequests(filter);

            var data = result.PendingRequests.ToList();

            return Json(data);
        }
        #endregion

        #region ViewVendorMaster
        [HttpGet]
        public async Task<IActionResult> ViewVendorMaster([FromQuery] string VMID)
        {
            VendorDetailsViewModel model = new VendorDetailsViewModel();
            string basePath = (Url.Content("~/Uploads/VendorMaster/")).ToString();
            model = await VMOBJService.GetVendorRequestDataById(VMID, basePath);

            return View(model);
        }
        #endregion

        #region ManageApprovalRequest
        public IActionResult ManageApprovalRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetApprovalRequest(VendorMasterFilter filter)
        {
            List<VendorApprovalRequest> result = new List<VendorApprovalRequest>();
            result = await VMOBJService.GetVendorApprovalRequests(filter);
            return Json(result);
        }
        #endregion

        #region ManageApprovalRequestFin
        public IActionResult ManageApprovalRequestFin()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetApprovalRequestFin(VendorMasterFilter filter)
        {
            List<VendorApprovalRequest> result = new List<VendorApprovalRequest>();
            result = await VMOBJService.GetVendorApprovalRequestsFin(filter);
            return Json(result);
        }
        #endregion

        #region EditVendorMasterForm
        [HttpGet]
        public IActionResult EditVendorMasterForm(string VMID, string returnUrl)
        {
            // fallback: if no returnUrl provided, use Referer header
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Request.Headers["Referer"].ToString();

            if (string.IsNullOrEmpty(VMID))
                return Redirect(returnUrl);

            string vendorId;
            try
            {
                vendorId = WebUtility.UrlDecode(Encryption.Decrypt(VMID));
            }
            catch
            {
                return Redirect(returnUrl);
            }

            if (string.IsNullOrEmpty(vendorId))
                return Redirect(returnUrl);

            DataSet ds = VMOBJ.VENDORREQUESTBYID(vendorId);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return Redirect(returnUrl);
            var ecode = _sessionService.Get<string>("userID");
            DataSet dsDept = null;
            if (!string.IsNullOrEmpty(ecode))
            {
                dsDept = VMOBJ.GET_DEPARTMENTHEAD(ecode);
            }

            var row = ds.Tables[0].Rows[0];

            var model = new EditVendorFormViewModel
            {
                VMID = VMID,
                VendorId = vendorId,
                RequestType = row["REQUESTTYPE"].ToString(),
                ProcessStatus = row["PROCESSSTATUS"].ToString(),
                VendorAccountGpId = row["VENDORACCOUNTGPID"].ToString(),
                VendorCode = row["VENDORCODE"].ToString(),
                RegistrationCert = row["REGISTRATIONCERTATTACH"].ToString(),
                ConflictCert = row["CONFLICTCER_ATTACH"].ToString(),
                MsmeCert = row["MSMECERTATTACHMENT"].ToString(),
                MandateForm = row["MANDATEATTACHMENT"].ToString(),
                CancelCheque = row["CCATTACHMENT"].ToString(),
                PanCard = row["PANATTECHMENT"].ToString(),
                GstCert = row["GSTCERTATTACHMENT"].ToString(),
                EInvoiceApplicable = row["EINVOICE_APPLICABLE"].ToString(),
                EInvoiceFile = row["EINVOICE_ATTACHMENT"].ToString(),
                LeiFile = row["LEI_ATTACHMENT"].ToString(),
                SendBackRemark = ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0
                    ? "<b>" + ds.Tables[1].Rows[^1]["APPDESC"] + ": </b> " + ds.Tables[1].Rows[^1]["REMARKS"]
                    : string.Empty,
                HasInterBank = (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0),
                WithHolding = ds.Tables.Count > 4 ? DataTableToList(ds, 4) : new(),
                DepartmentHeads = dsDept?.Tables[0].AsSelectList("EMPLOYEE", "DEPARTMENTHEAD", true, "--select--", ""),
                FilePath = Url.Content("~/Uploads/VendorMaster/")
            };

            ViewData["VendorPayload"] = JsonSerializer.Serialize(model);
            return View(); // pass strongly typed model to view
        }
        private static List<Dictionary<string, object>> DataTableToList(DataSet ds, int tableIndex)
        {
            var result = new List<Dictionary<string, object>>();

            if (ds == null || ds.Tables.Count <= tableIndex) return result;

            var table = ds.Tables[tableIndex];
            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                result.Add(dict);
            }

            return result;
        }
        public async Task<IActionResult> UpdateVendorData([FromForm] VendorMasterDetails vmd)
        {
            try
            {
                vmd.REQUESTID = vmd.VendorHeaderId;

                if (string.IsNullOrEmpty(vmd.csvfilename))
                {
                    return Json(new { success = false, message = "CSV file not provided" });
                }
                var fullPath = Path.Combine(filePath, vmd.csvfilename);
                string[] lines = System.IO.File.ReadAllLines(fullPath);
                if (lines == null || lines.Length != 66)
                    return Json(new { success = false, message = "CSV template must contain 66 lines" });

                MapCsvToVendorDetails(lines, vmd);

                vmd.VENDORNAME1 = CleanAndTrim(vmd.VENDORNAME1);
                vmd.VENDORNAME2 = CleanAndTrim(vmd.VENDORNAME2);
                vmd.VENDORNAME3 = CleanAndTrim(vmd.VENDORNAME3);
                vmd.STREET1 = CleanAndTrim(vmd.STREET1);
                vmd.STREET2 = CleanAndTrim(vmd.STREET2);
                vmd.STREET3 = CleanAndTrim(vmd.STREET3);
                vmd.STREET4 = CleanAndTrim(vmd.STREET4);
                vmd.BANKADDRESS = CleanAndTrim(vmd.BANKADDRESS);
                vmd.BRANCHNAME = CleanAndTrim(vmd.BRANCHNAME);

                vmd.VENDORCODE = vmd.VENDORCODE ?? string.Empty;
                vmd.CSVATTACHMENT = vmd.csvfilename ?? "";
                vmd.REQUESTTYPE = vmd.REQUESTTYPE;
                vmd.VENDORACCGRP = vmd.VENDORACCGRP ?? "";

                string[] FORDWORDTOARR = vmd.FORWARDTO?.Split(new Char[] { '#' });
                if (FORDWORDTOARR?.Length > 1)
                {
                    vmd.DEPTHEAD = Convert.ToString(FORDWORDTOARR[0]);
                    vmd.DEPTHEADEMAIL = Convert.ToString(FORDWORDTOARR[1]);
                    vmd.DEPTHEADNAME = Convert.ToString(FORDWORDTOARR[2]);
                }

                if ((vmd.MSMEINFOSTATUS ?? "").ToUpper() == "NO")
                    vmd.MSMECATEGORY = vmd.MSMECERTIFICATION = vmd.MSMEFROM = vmd.MSMETO = vmd.MSMECITY = "";

                if ((vmd.LEI_APPLICABLE ?? "").ToUpper() == "NO")
                    vmd.LEINO = "";

                if (vmd.REQUESTTYPE == "1" && !(vmd.VENDORACCGRP == "5AGB" || vmd.VENDORACCGRP == "5AGA"))
                    vmd.VENDORCODE = string.Empty;

                vmd.REMARKS = vmd.REMARKS ?? string.Empty;
                vmd.SPLAPPROVAL = vmd.SPLAPPROVAL;
                vmd.E_INVOICEApplicable = 2;

                string validateResult = VENDORUPDATE_VALIDATE(vmd);
                string[] resultmsg = (validateResult ?? "").Split('#');
                if (resultmsg.Length == 0 || resultmsg[0] != "1")
                    return Json(new { success = false, message = resultmsg.Length > 1 ? resultmsg[1] : "Validation failed" });

                if (string.IsNullOrEmpty(vmd.FILE_REGCERTIFICATENO) && vmd.HDFILE_REGCERTIFICATENO != null)
                {
                    var regCert = await UploadDocument(vmd.HDFILE_REGCERTIFICATENO);
                    if (!regCert.Status)
                        return Json(new { success = false, message = "Registration Certificate Attachment - " + regCert.Emsg });
                    vmd.FILE_REGCERTIFICATENO = regCert.Filename;
                }

                if (string.IsNullOrEmpty(vmd.FILE_MANDATEFORM) && vmd.HDFILE_MANDATEFORM != null)
                {
                    var mandateForm = await UploadDocument(vmd.HDFILE_MANDATEFORM);
                    if (!mandateForm.Status)
                        return Json(new { success = false, message = "Mandate Form Attachment - " + mandateForm.Emsg });
                    vmd.FILE_MANDATEFORM = mandateForm.Filename;
                }

                if (string.IsNullOrEmpty(vmd.FILE_CANCELCHEQUE) && vmd.HDFILE_CANCELCHEQUE != null)
                {
                    var cancelCheque = await UploadDocument(vmd.HDFILE_CANCELCHEQUE);
                    if (!cancelCheque.Status)
                        return Json(new { success = false, message = "Cancel Cheque Attachment - " + cancelCheque.Emsg });
                    vmd.FILE_CANCELCHEQUE = cancelCheque.Filename;
                }

                if (string.IsNullOrEmpty(vmd.FILE_PANCARD) && vmd.HDFILE_PANCARD != null)
                {
                    var panCard = await UploadDocument(vmd.HDFILE_PANCARD);
                    if (!panCard.Status)
                        return Json(new { success = false, message = "Pan Card Attachment - " + panCard.Emsg });
                    vmd.FILE_PANCARD = panCard.Filename;
                }

                if (string.IsNullOrEmpty(vmd.FILE_MSMECERTIFICATE) && vmd.HDFILE_MSMECERTIFICATE != null)
                {
                    var msmeCert = await UploadDocument(vmd.HDFILE_MSMECERTIFICATE);
                    if (!msmeCert.Status)
                        return Json(new { success = false, message = "MSME Certificate Attachment - " + msmeCert.Emsg });
                    vmd.FILE_MSMECERTIFICATE = msmeCert.Filename;
                }

                if (string.IsNullOrEmpty(vmd.FILE_GSTCERTIFICATE) && vmd.HDNFILE_GSTCERTIFICATE != null)
                {
                    var gstCert = await UploadDocument(vmd.HDNFILE_GSTCERTIFICATE);
                    if (!gstCert.Status)
                        return Json(new { success = false, message = "GST Certificate Attachment - " + gstCert.Emsg });
                    vmd.FILE_GSTCERTIFICATE = gstCert.Filename;
                }

                if (string.IsNullOrEmpty(vmd.FILE_LEICERTIFICATE) && vmd.HDNFILE_LEI != null)
                {
                    var leiCert = await UploadDocument(vmd.HDNFILE_LEI);
                    if (!leiCert.Status)
                        return Json(new { success = false, message = "LEI Certificate Attachment - " + leiCert.Emsg });
                    vmd.FILE_LEICERTIFICATE = leiCert.Filename;
                }

                if (string.IsNullOrEmpty(vmd.FILE_EINVOICE) && vmd.HDNFILE_EINVOICE != null)
                {
                    var einvoice = await UploadDocument(vmd.HDNFILE_EINVOICE);
                    if (!einvoice.Status)
                        return Json(new { success = false, message = "E-Invoice Attachment - " + einvoice.Emsg });
                    vmd.FILE_EINVOICE = einvoice.Filename;
                }

                if (string.IsNullOrEmpty(vmd.FILE_CONFLICTCERTIFICATE) && vmd.HDFILE_conflictCERTIFICATE != null)
                {
                    var conflictCert = await UploadDocument(vmd.HDFILE_conflictCERTIFICATE);
                    if (!conflictCert.Status)
                        return Json(new { success = false, message = "Conflict of Interest Attachment - " + conflictCert.Emsg });
                    vmd.FILE_CONFLICTCERTIFICATE = conflictCert.Filename;
                }


                vmd.REQECODE = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
                string strresult = VMOBJ.VENDORMASTER_INFORMATION_UPDATE(vmd);

                var strstatus = (strresult ?? "").Split('#');
                var resultMsgs = strstatus.Length > 0 ? strstatus[0] : "0";
                var errMsg = strstatus.Length > 1 ? strstatus[1] : "";
                var headerid = strstatus.Length > 2 ? strstatus[2] : "0";

                if (resultMsgs == "1")
                {
                    // SEND MAIL and return success
                    SendMailToDeptHead(vmd);
                    //return Json(new { success = true, message = "Your Request Submitted Successfully", redirect = Url.Action("ManageRequest", "Vendor") });

                    return Json(new { success = true, message = "Your Request has been Updated and Forward to Selected Authority for Approval" });
                }
                else
                {
                    return Json(new { success = false, message = errMsg });
                }
            }
            catch (Exception ex)
            {
                // log ex
                return Json(new { success = false, message = "An error occurred while processing." });
            }
        }
        private string VENDORUPDATE_VALIDATE(VendorMasterDetails vmd)
        {
            string ERRORLIST = "";
            string RESULT = "1";
            bool disclaimer = vmd.chkdeclaration;

            string FILEREGCERTIFICATENO = string.IsNullOrEmpty(vmd?.FILE_REGCERTIFICATENO)
                                ? vmd.HDFILE_REGCERTIFICATENO?.FileName
                                : vmd.FILE_REGCERTIFICATENO;

            string FILEMANDATEFORM = string.IsNullOrEmpty(vmd?.FILE_MANDATEFORM)
                                            ? vmd.HDFILE_MANDATEFORM?.FileName
                                            : vmd.FILE_MANDATEFORM;

            string FILECANCELCHEQUE = string.IsNullOrEmpty(vmd?.FILE_CANCELCHEQUE)
                                            ? vmd.HDFILE_CANCELCHEQUE?.FileName
                                            : vmd.FILE_CANCELCHEQUE;

            string FILEPANCARD = string.IsNullOrEmpty(vmd?.FILE_PANCARD)
                                            ? vmd.HDFILE_PANCARD?.FileName
                                            : vmd.FILE_PANCARD;

            string FILEMSMECERTIFICATE = string.IsNullOrEmpty(vmd?.FILE_MSMECERTIFICATE)
                                            ? vmd.HDFILE_MSMECERTIFICATE?.FileName
                                            : vmd.FILE_MSMECERTIFICATE;

            string FILEGSTCERTIFICATE = string.IsNullOrEmpty(vmd?.FILE_GSTCERTIFICATE)
                                            ? vmd.HDNFILE_GSTCERTIFICATE?.FileName
                                            : vmd.FILE_GSTCERTIFICATE;

            string FILEEINVOICE = string.IsNullOrEmpty(vmd?.FILE_EINVOICE)
                                            ? vmd.HDNFILE_EINVOICE?.FileName
                                            : vmd.FILE_EINVOICE;

            string FILELEI = string.IsNullOrEmpty(vmd?.FILE_LEICERTIFICATE)
                                            ? vmd.HDNFILE_LEI?.FileName
                                            : vmd.FILE_LEICERTIFICATE;

            string FILEconflictCERTIFICATE = string.IsNullOrEmpty(vmd?.FILE_CONFLICTCERTIFICATE)
                                            ? vmd.HDFILE_conflictCERTIFICATE?.FileName
                                            : vmd.FILE_CONFLICTCERTIFICATE;

            if (vmd.REQUESTTYPE == "1")
            {
                if (string.IsNullOrWhiteSpace(FILEREGCERTIFICATENO) &&
                    (vmd.VENDORACCGRP == "5AG1" || vmd.VENDORACCGRP == "5AG2" ||
                     vmd.VENDORACCGRP == "5AG3" || vmd.VENDORACCGRP == "5AGB"))
                {
                    ERRORLIST += "<li>Registration Certificate No. attachment is mandatory field</li>";
                    RESULT = "0";
                }
                if (vmd.VENDORACCGRP == "5AGB" && string.IsNullOrWhiteSpace(FILEconflictCERTIFICATE))
                {
                    ERRORLIST += "<li>Conflict of interest Certificate attachment is mandatory field</li>";
                    RESULT = "0";
                }
            }

            if (vmd.MSMEINFOSTATUS?.ToUpper() == "YES" && string.IsNullOrWhiteSpace(FILEMSMECERTIFICATE))
            {
                ERRORLIST += "<li>MSME certificate attachment is mandatory field</li>";
                RESULT = "0";
            }

            if (!string.IsNullOrWhiteSpace(vmd.IFSCCODE))
            {
                if (string.IsNullOrWhiteSpace(FILEMANDATEFORM) &&
                    (vmd.VENDORACCGRP == "5AG1" || vmd.VENDORACCGRP == "5AG2" ||
                     vmd.VENDORACCGRP == "5AG3" || vmd.VENDORACCGRP == "5AGB"))
                {
                    ERRORLIST += "<li>Mandate form attachment is mandatory field</li>";
                    RESULT = "0";
                }
                if (string.IsNullOrWhiteSpace(FILECANCELCHEQUE) &&
                    (vmd.VENDORACCGRP == "5AG1" || vmd.VENDORACCGRP == "5AGA" ||
                     vmd.VENDORACCGRP == "5AG3" || vmd.VENDORACCGRP == "5AGB"))
                {
                    ERRORLIST += "<li>Cancel cheque attachment is mandatory field</li>";
                    RESULT = "0";
                }
            }

            if (!string.IsNullOrWhiteSpace(vmd.PANNUMBER) && string.IsNullOrWhiteSpace(FILEPANCARD))
            {
                ERRORLIST += "<li>Pan Card attachment is mandatory field</li>";
                RESULT = "0";
            }

            if (!string.IsNullOrWhiteSpace(vmd.GSTIN) && string.IsNullOrWhiteSpace(FILEGSTCERTIFICATE))
            {
                ERRORLIST += "<li>GSTIN certificate attachment is mandatory field</li>";
                RESULT = "0";
            }

            if (vmd.E_INVOICEApplicable == 0 && string.IsNullOrWhiteSpace(FILEEINVOICE))
            {
                ERRORLIST += "<li>E-Invoice attachment is mandatory field</li>";
                RESULT = "0";
            }

            if (vmd.LEI_APPLICABLE?.ToUpper() == "YES" && string.IsNullOrWhiteSpace(FILELEI))
            {
                ERRORLIST += "<li>LEI attachment is mandatory field</li>";
                RESULT = "0";
            }

            // Check duplicate file names
            string[] arr = { FILEREGCERTIFICATENO, FILEMANDATEFORM, FILECANCELCHEQUE, FILEPANCARD, FILEMSMECERTIFICATE, FILEGSTCERTIFICATE };
            arr = arr.Where(val => !string.IsNullOrWhiteSpace(val)).ToArray();
            string[] arrdistinct = arr.Distinct().ToArray();
            if (arrdistinct.Length != arr.Length)
            {
                ERRORLIST += "<li>Attachment Document could not be Same</li>";
                RESULT = "0";
            }

            if (!disclaimer)
            {
                ERRORLIST += "<li>Declaration is mandatory</li>";
                RESULT = "0";
            }

            if (string.IsNullOrWhiteSpace(vmd.FORWARDTO))
            {
                ERRORLIST += "<li>Forward to is mandatory</li>";
                RESULT = "0";
            }

            if (string.IsNullOrWhiteSpace(vmd.REMARKS))
            {
                ERRORLIST += "<li>Remark is mandatory</li>";
                RESULT = "0";
            }

            ERRORLIST = "<ul>" + ERRORLIST + "</ul>";
            return RESULT + "#" + ERRORLIST;
        }
        #endregion

        #region VendorApprovalForm
        [HttpGet]
        public async Task<IActionResult> VendorApprovalForm(string VMID)
        {
            string ecode = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
            DataTable dtSAuthList = new DataTable();
            dtSAuthList = VMOBJ.GetSpecialAppAuthorityList(ecode);
            //string vid = Encryption.Decrypt(VMID);
            VendorDetailsViewModel model = new VendorDetailsViewModel();
            model.SPAppAuthorityList = dtSAuthList.AsSelectList("ADEMPCODE", "EMPNAME", true, "--select--", "");
            string basePath = (Url.Content("~/Uploads/VendorMaster/")).ToString();
            model = await VMOBJService.GetVendorRequestDataById(VMID, basePath);

            return View(model);
        }

        [HttpPost]
        public IActionResult ApproveVendor([FromBody] VendorApprovalModel model)
        {
            // basic null checks
            if (model == null)
                return Json(new { success = false, message = "Invalid request" });

            // decode VMID
            string VENDORHEADERID = string.Empty;
            try
            {
                VENDORHEADERID = model.Vmid ?? string.Empty;
            }
            catch
            {
                return Json(new { success = false, message = "Invalid VMID" });
            }

            // validations
            if (string.IsNullOrEmpty(model.Status))
                return Json(new { success = false, message = "Status is mandatory field" });

            if (string.IsNullOrWhiteSpace(model.Remarks))
                return Json(new { success = false, message = "Remarks is mandatory field" });

            if (model.SplAppId == "1" && !model.MatchChecked)
                return Json(new { success = false, message = "Kindly check Match Detail tab and acknowledge the match detail verification checkbox." });

            if (!model.DeclarationChecked)
                return Json(new { success = false, message = "Declaration is mandatory" });

            // when approving, ensure attachments are verified
            if (model.Status == "1")
            {
                var missing = new List<string>();
                if (!model.RegCertVerified) missing.Add("Registration Certificate Number");
                if (!model.ConflictCertVerified) missing.Add("Conflict of Interest Certificate");
                if (!model.MsmeVerified) missing.Add("MSME Certificate Number");
                if (!model.CancelChequeVerified) missing.Add("Cancel Cheque");
                if (!model.MandateFormVerified) missing.Add("Mandate Form");
                if (!model.PanCardVerified) missing.Add("Pan Card");
                if (!model.GstVerified) missing.Add("GST Registration");
                if (!model.EInvoiceVerified) missing.Add("E-Invoice");

                if (missing.Any())
                {
                    var list = string.Join(", ", missing);
                    return Json(new { success = false, message = $"Kindly verify the {list} attachment(s)." });
                }
            }

            // prepare parameters for business method
            string Ecode = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
            string strappcode = ""; // string.IsNullOrEmpty(model.AppAuthority) ? "" : model.AppAuthority;
            string strsplapplevel = "0"; // model.SplAppId == "1" ? "1" : "0";

            // call existing business method
            string strErrMsg = VMOBJ.VENDORREQUESTAPPROVAL(VENDORHEADERID, model.Remarks, model.Status, Ecode, "1", "", strappcode, strsplapplevel);
            string[] strmsg = strErrMsg.Split(new char[] { '#' });
            string errResult = Convert.ToString(strmsg[0]);
            string errMsg = strmsg.Length > 1 ? Convert.ToString(strmsg[1]) : strErrMsg;

            if (errResult == "1")
            {
                SendVendorStatusEmail(model);
                string userMessage = model.Status == "1" ? "Vendor Master Request has been Approved Successfully"
                    : model.Status == "2" ? "Vendor Master Request has been Rejected Successfully"
                    : "Vendor Master Request has been Send Back Successfully";

                return Json(new { success = true, message = userMessage, redirect = Url.Action("ManageApprovalRequest", "VendorMaster") });
            }

            return Json(new { success = false, message = errMsg.Replace("\n", "") });
        }
        #endregion

        #region VendorApprovalRequestFin
        [HttpGet]
        public async Task<IActionResult> VendorApprovalFormFin(string VMID)
        {
            string ecode = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
            DataTable dtSAuthList = new DataTable();
            dtSAuthList = VMOBJ.GetSpecialAppAuthorityOP(ecode);

            VendorDetailsViewModel model = new VendorDetailsViewModel();
            model.SPAppAuthorityList = dtSAuthList.AsSelectList("ADEMPCODE", "EMPNAME", true, "--select--", "");
            string basePath = Url.Content("~/Uploads/VendorMaster/").ToString();
            model = await VMOBJService.GetVendorRequestDataById(VMID, basePath);

            return View(model);
        }

        [HttpPost]
        public IActionResult ApproveVendorFin([FromBody] VendorApprovalModel model)
        {
            // basic null checks
            if (model == null)
                return Json(new { success = false, message = "Invalid request" });

            // decode VMID
            string VENDORHEADERID = string.Empty;
            try
            {
                VENDORHEADERID = model.Vmid ?? string.Empty;
            }
            catch
            {
                return Json(new { success = false, message = "Invalid VMID" });
            }

            // validations
            if (string.IsNullOrEmpty(model.Status))
                return Json(new { success = false, message = "Status is mandatory field" });

            if (string.IsNullOrWhiteSpace(model.Remarks))
                return Json(new { success = false, message = "Remarks is mandatory field" });
            //if (model.SplAppId == "1" && !model.MatchChecked)
            if (model.SplAppId != "1")
                return Json(new { success = false, message = "Kindly check Match Detail tab and acknowledge the match detail verification checkbox." });

            if (!model.DeclarationChecked)
                return Json(new { success = false, message = "Declaration is mandatory" });

            // when approving, ensure attachments are verified
            if (model.Status == "1")
            {
                var missing = new List<string>();
                if (!model.RegCertVerified) missing.Add("Registration Certificate Number");
                if (!model.ConflictCertVerified) missing.Add("Conflict of Interest Certificate");
                if (!model.MsmeVerified) missing.Add("MSME Certificate Number");
                if (!model.CancelChequeVerified) missing.Add("Cancel Cheque");
                if (!model.MandateFormVerified) missing.Add("Mandate Form");
                if (!model.PanCardVerified) missing.Add("Pan Card");
                if (!model.GstVerified) missing.Add("GST Registration");
                if (!model.EInvoiceVerified) missing.Add("E-Invoice");

                //if (missing.Any())
                //{
                //    var list = string.Join(", ", missing);
                //    return Json(new { success = false, message = $"Kindly verify the {list} attachment(s)." });
                //}
            }

            // prepare parameters for business method
            string Ecode = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
            string strappcode = ""; // string.IsNullOrEmpty(model.AppAuthority) ? "" : model.AppAuthority;
            string strsplapplevel = "0"; // model.SplAppId == "1" ? "1" : "0";

            // call existing business method
            string strErrMsg = VMOBJ.VENDORREQUESTAPPROVAL(VENDORHEADERID, model.Remarks, model.Status, Ecode, "6", "", strappcode, strsplapplevel);
            string[] strmsg = strErrMsg.Split(new char[] { '#' });
            string errResult = Convert.ToString(strmsg[0]);
            string errMsg = strmsg.Length > 1 ? Convert.ToString(strmsg[1]) : strErrMsg;

            if (errResult == "1")
            {
                SendVendorStatusEmail(model);
                string userMessage = model.Status == "1" ? "Vendor Master Request has been Approved Successfully"
                    : model.Status == "2" ? "Vendor Master Request has been Rejected Successfully"
                    : "Vendor Master Request has been Send Back Successfully";

                return Json(new { success = true, message = userMessage, redirect = Url.Action("ManageApprovalRequest", "VendorMaster") });
            }

            return Json(new { success = false, message = errMsg.Replace("\n", "") });
        }
        #endregion

        public void SendVendorStatusEmail(VendorApprovalModel model)
        {
            if (model == null) return;

            string requestorName = model.RequestorName ?? "";        // add these properties to your model or pass separately
            string requestorEmail = model.RequestorEmail ?? "";
            string requestTypeLabel = model.RequestType ?? "";
            string accGroup = model.AccGroup ?? "";
            string vendorName = model.VendorName ?? "";
            string requestDate = model.RequestDate ?? "";
            string remarks = model.Remarks ?? "";

            // Normalize request type text
            string reqType = (requestTypeLabel ?? "").ToUpper() == "CREATE VENDOR" ? "Creation" : "Updation";

            // If no recipient, nothing to do
            if (string.IsNullOrWhiteSpace(requestorEmail)) return;

            // Approver name from session (or pass as parameter)
            var username = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;

            var sb = new StringBuilder();

            if (model.Status == "2") // Rejected
            {
                sb.Append("<div style='width:700px;border:2px skyblue solid;padding-top:0px'>");
                sb.Append("<div style='width:700px;height:25px;background-color:skyblue;'><b>Vendor Master Request</b></div>");
                sb.Append("<table cellpadding='0' cellspacing='0' border='0' width='700px'>");
                sb.AppendFormat("<tr><td colspan='2'><b>&nbsp;&nbsp;Dear {0} San</b><br/></td></tr>", HttpUtility.HtmlEncode(requestorName));
                sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");
                sb.AppendFormat("<tr><td colspan='2'>&nbsp;&nbsp;Your request for vendor master {0} has been rejected by {1}. Following are the details:</td></tr>",
                                HttpUtility.HtmlEncode(reqType), HttpUtility.HtmlEncode(username));
                sb.Append("<tr><td colspan='2'>&nbsp;</td></tr></table><br/>");

                sb.Append("<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>");
                sb.Append("<tr>");
                sb.Append("<td width='130' style='background-color:#CAE8EA;...'> &nbsp;Request Type</td>");
                sb.AppendFormat("<td width='220' style='background-color:white;...'> &nbsp;{0}</td>", HttpUtility.HtmlEncode(requestTypeLabel));
                sb.Append("<td width='130' style='background-color:#CAE8EA;...'> &nbsp;Request Date</td>");
                sb.AppendFormat("<td width='220' style='background-color:white;...'> &nbsp;{0}</td>", HttpUtility.HtmlEncode(requestDate));
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td width='130' style='background-color:#CAE8EA;...'> &nbsp;Vendor Name</td>");
                sb.AppendFormat("<td width='570' colspan='3' style='background-color:white;...'> &nbsp;{0}</td>", HttpUtility.HtmlEncode(vendorName));
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td style='background-color:#CAE8EA;...'> &nbsp;Reason</td>");
                sb.AppendFormat("<td colspan='3' style='background-color:white;...'> &nbsp;{0}</td>", HttpUtility.HtmlEncode(remarks));
                sb.Append("</tr>");
                sb.Append("</table>");

                sb.Append("<table cellpadding='0' cellspacing='0' border='0'>");
                sb.Append("<tr><td><br/><b>&nbsp;&nbsp;Thank You</b><br/></td></tr>");
                sb.Append("<tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>");
                sb.Append("<tr><td>&nbsp;&nbsp;Portal Admin<br/></td></tr>");
                sb.Append("<tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>");
                sb.Append("</table></div>");

                SendEmail(requestorEmail, "Vendor Master Request Rejected", sb.ToString());
            }
            else if (model.Status == "3") // Send back
            {
                sb.Append("<div style='width:700px;border:2px skyblue solid;padding-top:0px'>");
                sb.Append("<div style='width:700px;height:25px;background-color:skyblue;'><b>Vendor Master Request</b></div>");
                sb.Append("<table cellpadding='0' cellspacing='0' border='0' width='700px'>");
                sb.AppendFormat("<tr><td colspan='2'><b>&nbsp;&nbsp;Dear {0} San</b><br/></td></tr>", HttpUtility.HtmlEncode(requestorName));
                sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");
                sb.AppendFormat("<tr><td colspan='2'>&nbsp;&nbsp;Your request for vendor master {0} has been send back by {1}. Following are the details:</td></tr>",
                                HttpUtility.HtmlEncode(reqType), HttpUtility.HtmlEncode(username));
                sb.Append("<tr><td colspan='2'>&nbsp;</td></tr></table><br/>");

                sb.Append("<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>");
                sb.Append("<tr>");
                sb.Append("<td width='130' style='background-color:#CAE8EA;...'> &nbsp;Request Type</td>");
                sb.AppendFormat("<td width='220' style='background-color:white;...'> &nbsp;{0}</td>", HttpUtility.HtmlEncode(requestTypeLabel));
                sb.Append("<td width='130' style='background-color:#CAE8EA;...'> &nbsp;Request Date</td>");
                sb.AppendFormat("<td width='220' style='background-color:white;...'> &nbsp;{0}</td>", HttpUtility.HtmlEncode(requestDate));
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td width='130' style='background-color:#CAE8EA;...'> &nbsp;Vendor Name</td>");
                sb.AppendFormat("<td width='570' colspan='3' style='background-color:white;...'> &nbsp;{0}</td>", HttpUtility.HtmlEncode(vendorName));
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td style='background-color:#CAE8EA;...'> &nbsp;Reason</td>");
                sb.AppendFormat("<td colspan='3' style='background-color:white;...'> &nbsp;{0}</td>", HttpUtility.HtmlEncode(remarks));
                sb.Append("</tr>");
                sb.Append("</table>");

                // Portal link - use absolute URL or configuration
                sb.Append("<table cellpadding='0' cellspacing='0' border='0'>");
                sb.Append("<tr><td><br/>&nbsp;&nbsp;<b>Login to <a href='https://portal.honda2wheelersindia.com'>Employee Portal</a> follow the following link:</b></td></tr>");
                sb.Append("<tr><td>&nbsp;&nbsp;Master Data Governance => Manage Request</td></tr>");
                sb.Append("<tr><td><br/><b>&nbsp;&nbsp;Thank You<br/></b></td></tr>");
                sb.Append("<tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>");
                sb.Append("<tr><td>&nbsp;&nbsp;Portal Admin<br/></td></tr>");
                sb.Append("<tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>");
                sb.Append("</table></div>");

                SendEmail(requestorEmail, "Vendor Master Request Send Back", sb.ToString());
            }
        }

        private void SendEmail(string to, string subject, string htmlBody)
        {
            var sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = to;
            sendMail.MailSubject = subject;
            sendMail.MailBody = htmlBody;
            sendMail.Send();
        }


        #region Vendor Block / Unblock
        #region VendorBlockForm
        [HttpGet]
        public IActionResult VendorBlockForm()
        {
            var vm = new VendorBlockViewModel
            {
                RequestCategory = RequestCategory.Block,
                RequestType = RequestType.SingleVendor,
                ForwardToList = GetForwardToOptions()
            };
            return View(vm);
        }

        private IEnumerable<SelectListItem> GetForwardToOptions()
        {
            DataSet DS = VMOBJ.GET_DEPARTMENTHEAD(_userId);
            var List = DS.Tables[0].AsSelectList("EMPLOYEE", "DEPARTMENTHEAD", true, "--select--", "");
            return List;
        }

        //[HttpPost]
        //public async Task<IActionResult> ContinueToNext(VendorBlockViewModel model)
        //{
        //    try
        //    {
        //        string requestTypeCode = model.RequestType == RequestType.SingleVendor ? "1" : "2";
        //        string requestCategoryCode = model.RequestCategory == RequestCategory.Block ? "1" : "2";

        //        // reset error state
        //        model.ErrorFlag = false;
        //        model.ErrorMessage = string.Empty;

        //        // validations from original code (use model error properties instead of ModelState)
        //        if (string.IsNullOrEmpty(requestTypeCode))
        //        {
        //            model.ErrorFlag = true;
        //            model.ErrorMessage = "Select Request Type";
        //        }
        //        else if (string.IsNullOrEmpty(requestCategoryCode))
        //        {
        //            model.ErrorFlag = true;
        //            model.ErrorMessage = "Select Request Category";
        //        }
        //        else if (!model.PurchasingData && !model.PostingData)
        //        {
        //            model.ErrorFlag = true;
        //            model.ErrorMessage = "Kindly select Purchasing Data or Posting Data";
        //        }

        //        if (model.ErrorFlag)
        //        {
        //            model.ShowDetails = false;
        //            model.ForwardToList = GetForwardToOptions();
        //            return View("VendorBlockForm", model);
        //        }

        //        if (requestTypeCode == "1") // Single Vendor
        //        {
        //            if (string.IsNullOrWhiteSpace(model.VendorCode))
        //            {
        //                model.ErrorFlag = true;
        //                model.ErrorMessage = "Vendor Code is mandatory field";
        //                model.ShowDetails = false;
        //                model.ForwardToList = GetForwardToOptions();
        //                return View("VendorBlockForm", model);
        //            }

        //            DataSet ods1 = VMOBJ.GETVENDORLISTBLOCKDTL(model.VendorCode.Trim());
        //            if (ods1 != null && ods1.Tables.Count > 0 && ods1.Tables[0].Rows.Count > 0)
        //            {
        //                model.VendorList = ConvertDataTableToVendorList(ods1.Tables[0]);
        //                model.VendorCodeList = model.VendorCode.Trim();
        //                model.ShowDetails = true;
        //                model.ErrorFlag = false;
        //                model.ErrorMessage = string.Empty;
        //                model.ForwardToList = GetForwardToOptions();
        //                return View("VendorBlockForm", model);
        //            }
        //            else
        //            {
        //                model.ErrorFlag = true;
        //                model.ErrorMessage = "Vendor detail not exist";
        //                model.ShowDetails = false;
        //                model.ForwardToList = GetForwardToOptions();
        //                return View("VendorBlockForm", model);
        //            }
        //        }
        //        else // Mass Vendor
        //        {
        //            if ((model.UploadFile == null || model.UploadFile.Length == 0) && string.IsNullOrWhiteSpace(model.UploadedFileName))
        //            {
        //                model.ErrorFlag = true;
        //                model.ErrorMessage = "Please Upload Vendor Master CSV file";
        //                model.ShowDetails = false;
        //                model.ForwardToList = GetForwardToOptions();
        //                return View("VendorBlockForm", model);
        //            }

        //            string savedFileName = null;
        //            if (model.UploadFile != null && model.UploadFile.Length > 0)
        //            {
        //                var uploadResult = await UploadFileAsync(model.UploadFile, filePath, HttpContext);
        //                var parts = uploadResult.Split('@');
        //                if (parts[0] != "1")
        //                {
        //                    model.ErrorFlag = true;
        //                    model.ErrorMessage = parts.Length > 1 ? parts[1] : "Unable to upload file";
        //                    model.ShowDetails = false;
        //                    model.ForwardToList = GetForwardToOptions();
        //                    return View("VendorBlockForm", model);
        //                }
        //                savedFileName = parts[1];
        //                model.UploadedFileName = savedFileName;
        //            }
        //            else
        //            {
        //                savedFileName = model.UploadedFileName;
        //            }

        //            var fullpath = Path.Combine(filePath, savedFileName);
        //            if (!System.IO.File.Exists(fullpath))
        //            {
        //                model.ErrorFlag = true;
        //                model.ErrorMessage = "Uploaded file not found on server";
        //                model.ShowDetails = false;
        //                model.ForwardToList = GetForwardToOptions();
        //                return View("VendorBlockForm", model);
        //            }

        //            var failedList = new List<string>();
        //            var validList = new List<string>();

        //            try
        //            {
        //                using (var csvReader = new TextFieldParser(fullpath))
        //                {
        //                    csvReader.SetDelimiters(new string[] { "," });
        //                    csvReader.HasFieldsEnclosedInQuotes = true;
        //                    if (!csvReader.EndOfData)
        //                        csvReader.ReadLine(); // skip header

        //                    while (!csvReader.EndOfData)
        //                    {
        //                        var fieldData = csvReader.ReadFields();
        //                        if (fieldData == null || fieldData.Length == 0) break;
        //                        var strVendorcode = fieldData[0]?.Trim();
        //                        if (!string.IsNullOrEmpty(strVendorcode))
        //                        {
        //                            DataSet ods1 = VMOBJ.GETVENDORLISTBLOCKDTL(strVendorcode);
        //                            if (ods1 == null || ods1.Tables.Count == 0 || ods1.Tables[0].Rows.Count == 0)
        //                                failedList.Add(strVendorcode);
        //                            else
        //                                validList.Add(strVendorcode);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                model.ErrorFlag = true;
        //                model.ErrorMessage = ex.Message;
        //                model.ShowDetails = false;
        //                model.ForwardToList = GetForwardToOptions();
        //                return View("VendorBlockForm", model);
        //            }

        //            if (failedList.Any())
        //            {
        //                var failedvendorcodelist = string.Join(",", failedList);
        //                model.ErrorFlag = true;
        //                model.ErrorMessage = $"Vendor detail not exist for vendor code {failedvendorcodelist}";
        //                model.ShowDetails = false;
        //                model.ForwardToList = GetForwardToOptions();
        //                return View("VendorBlockForm", model);
        //            }

        //            if (validList.Any())
        //            {
        //                var allvendorcodelist = string.Join(",", validList);
        //                DataSet ods1 = VMOBJ.GETVENDORLISTBLOCKDTL(allvendorcodelist);
        //                model.VendorCodeList = allvendorcodelist;
        //                model.VendorList = ConvertDataTableToVendorList(ods1.Tables[0]);
        //                model.ShowDetails = true;
        //                model.ErrorFlag = false;
        //                model.ErrorMessage = string.Empty;
        //                model.ForwardToList = GetForwardToOptions();
        //                return View("VendorBlockForm", model);
        //            }

        //            model.ErrorFlag = true;
        //            model.ErrorMessage = "No valid vendor codes found in uploaded file";
        //            model.ShowDetails = false;
        //            model.ForwardToList = GetForwardToOptions();
        //            return View("VendorBlockForm", model);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        model.ErrorFlag = true;
        //        model.ErrorMessage = ex.Message;
        //        model.ShowDetails = false;
        //        model.ForwardToList = GetForwardToOptions();
        //        return View("VendorBlockForm", model);
        //    }
        //}

        public async Task<IActionResult> ContinueToNext([FromForm] VendorBlockViewModel model)
        {
            // Initialize UI state to mirror original WebForms defaults
            model.ErrorFlag = false;
            model.Message = string.Empty;
            model.UploadedFileName = string.Empty;
            model.VendorCodeList = string.Empty;
            model.ShowDetails = false;
            model.ShowForm = true;
            model.VendorList = new List<VendorListItem>();

            try
            {
                // Set purchasing/posting labels based on category and checkboxes
                if (model.RequestCategory == RequestCategory.Block)
                {
                    model.LabelPurchasing = model.PurchasingData ? "Block" : string.Empty;
                    model.LabelPosting = model.PostingData ? "Block" : string.Empty;
                }
                else
                {
                    model.LabelPurchasing = model.PurchasingData ? "UnBlock" : string.Empty;
                    model.LabelPosting = model.PostingData ? "UnBlock" : string.Empty;
                }

                // Validations (mirror original alerts)
                if (!Enum.IsDefined(typeof(RequestType), model.RequestType))
                {
                    model.ErrorFlag = true;
                    model.Message = "Select Request Type";
                    return Json(model);
                }

                if (!Enum.IsDefined(typeof(RequestCategory), model.RequestCategory))
                {
                    model.ErrorFlag = true;
                    model.Message = "Select Request Category";
                    return Json(model);
                }

                if (!model.PurchasingData && !model.PostingData)
                {
                    model.ErrorFlag = true;
                    model.Message = "Kindly select Purchasing Data or Posting Data";
                    return Json(model);
                }

                // Single Vendor flow
                if (model.RequestType == RequestType.SingleVendor)
                {
                    if (string.IsNullOrWhiteSpace(model.VendorCode))
                    {
                        model.ErrorFlag = true;
                        model.Message = "Vendor Code is mandatory field";
                        return Json(model);
                    }

                    var vendorCodeTrim = model.VendorCode!.Trim();
                    DataSet ods1 = VMOBJ.GETVENDORLISTBLOCKDTL(vendorCodeTrim);
                    if (ods1 != null && ods1.Tables.Count > 0 && ods1.Tables[0].Rows.Count > 0)
                    {
                        model.VendorList = ConvertDataTableToVendorList(ods1.Tables[0]);

                        model.ShowDetails = true;
                        model.ShowForm = false;

                        // return approval authority payload if needed
                        model.ForwardToList = GetForwardToOptions();

                        return Json(model);
                    }
                    else
                    {
                        model.ErrorFlag = true;
                        model.Message = "Vendor detail not exist";
                        return Json(model);
                    }
                }

                // Mass Vendor flow
                if (model.RequestType == RequestType.MassVendor)
                {
                    // Ensure file is provided
                    if (model.UploadFile == null || model.UploadFile.Length == 0)
                    {
                        model.ErrorFlag = true;
                        model.Message = "Please Upload Vendor Master CSV file";
                        return Json(model);
                    }

                    // Upload file using provided helper
                    var uploadResult = await UploadFileAsync(model.UploadFile);
                    var parts = uploadResult.Split(new[] { '@' }, 2);
                    var flag = parts.Length > 0 ? parts[0] : "0";
                    var info = parts.Length > 1 ? parts[1] : string.Empty;

                    if (flag != "1")
                    {
                        model.ErrorFlag = true;
                        model.Message = info; // contains error message from UploadFileAsync
                        return Json(model);
                    }

                    // Build full path to saved file
                    var savedFileName = info;
                    var fullPath = Path.Combine(filePath, savedFileName);

                    // Parse CSV and validate vendor codes
                    var failedVendorCodes = new List<string>();
                    var allVendorCodes = new List<string>();

                    try
                    {
                        using (var csvReader = new TextFieldParser(fullPath))
                        {
                            csvReader.SetDelimiters(new string[] { "," });
                            csvReader.HasFieldsEnclosedInQuotes = true;

                            // original code read header first
                            if (!csvReader.EndOfData)
                                csvReader.ReadLine();

                            while (!csvReader.EndOfData)
                            {
                                var fields = csvReader.ReadFields();
                                if (fields == null || fields.Length == 0) break;

                                var strVendorCode = fields[0]?.Trim();
                                if (string.IsNullOrEmpty(strVendorCode)) continue;

                                DataSet ods1 = VMOBJ.GETVENDORLISTBLOCKDTL(strVendorCode);
                                if (ods1 == null || ods1.Tables.Count == 0 || ods1.Tables[0].Rows.Count == 0)
                                    failedVendorCodes.Add(strVendorCode);
                                else
                                    allVendorCodes.Add(strVendorCode);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        model.ErrorFlag = true;
                        model.Message = ex.Message;
                        return Json(model);
                    }

                    if (failedVendorCodes.Count == 0)
                    {
                        var allVendorCodesCsv = string.Join(",", allVendorCodes);
                        DataSet odsAll = VMOBJ.GETVENDORLISTBLOCKDTL(allVendorCodesCsv);

                        model.VendorCodeList = allVendorCodesCsv;
                        model.VendorList = odsAll != null && odsAll.Tables.Count > 0
                            ? ConvertDataTableToVendorList(odsAll.Tables[0])
                            : new List<VendorListItem>();

                        model.ShowDetails = true;
                        model.ShowForm = false;

                        model.ErrMsgDivVisible = false;
                        model.ErrMsgDivHtml = string.Empty;

                        model.ForwardToList = GetForwardToOptions();

                        return Json(model);
                    }
                    else
                    {
                        model.ErrMsgDivVisible = true;
                        model.ErrMsgDivHtml = $"<ul> Vendor detail not exist for vendor code {string.Join(",", failedVendorCodes)}</ul>";
                        model.ErrorFlag = true;
                        model.Message = "Some vendor codes not found";
                        return Json(model);
                    }
                }

                model.ErrorFlag = true;
                model.Message = "Invalid Request";
                return Json(model);
            }
            catch (Exception ex)
            {
                model.ErrorFlag = true;
                model.Message = ex.Message;
                return Json(model);
            }
        }

        // Provided helper: Convert DataTable to VendorListItem list
        private List<VendorListItem> ConvertDataTableToVendorList(DataTable table)
        {
            var list = new List<VendorListItem>();
            if (table == null) return list;
            foreach (DataRow r in table.Rows)
            {
                list.Add(new VendorListItem
                {
                    VendorCode = r["VENDORCODE"]?.ToString() ?? string.Empty,
                    VendorName = r["VENDORNAME"]?.ToString() ?? string.Empty,
                    City = r["CITY"]?.ToString() ?? string.Empty,
                    Region = r["REGION"]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        // Provided helper: Upload file asynchronously
        private async Task<string> UploadFileAsync(IFormFile postedFile)
        {
            if (postedFile == null || postedFile.Length == 0)
                return "0@Invalid file content";

            var strFileName = Path.GetFileName(postedFile.FileName);
            if (string.IsNullOrEmpty(strFileName))
                return "0@Invalid file name supplied";

            var fullpath = Path.Combine(filePath, strFileName);
            var fileext = Path.GetExtension(fullpath);
            var ModifiedName = Path.GetFileNameWithoutExtension(strFileName) + "#" + DateTime.Now.ToString("ddMMyyyyHHmmss") + fileext;
            var fullMpath = Path.Combine(filePath, ModifiedName);

            if (string.IsNullOrEmpty(filePath))
                return "0@Path not found";

            if (!string.Equals(fileext, ".csv", StringComparison.OrdinalIgnoreCase))
                return "0@Cannot upload file because the file format or extension is invalid please select a valid CSV file";

            try
            {
                if (postedFile.Length <= 2925714) // same limit as original
                {
                    using (var stream = new FileStream(fullMpath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    return "1@" + ModifiedName;
                }
                else
                {
                    return "0@Unable to upload,file exceeds maximum limit";
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return "0@" + ex.Message + " Permission to upload file denied";
            }
        }

        [HttpPost]
        public IActionResult SubmitVendorBlock([FromBody] VendorBlockViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorFlag = true;
                model.Message = "Validation failed. Please check your inputs.";
                return Json(model);
            }

            try
            {
                if (string.IsNullOrEmpty(model.Remarks))
                {
                    model.ErrorFlag = true;
                    model.Message = "Remarks is mandatory field";
                    return Json(model);
                }
                if (string.IsNullOrEmpty(model.ForwardTo))
                {
                    model.ErrorFlag = true;
                    model.Message = "Select Forward To";
                    return Json(model);
                }
                string reqCode = _userId;

                string requestType = model.RequestType == RequestType.SingleVendor ? "1" : "2";
                string requestCategory = model.RequestCategory == RequestCategory.Block ? "1" : "2";

                string blockPurchasing = model.PurchasingData ? "1" : "0";
                string blockPosting = model.PostingData ? "1" : "0";

                string vendorCode = model.RequestType == RequestType.SingleVendor ? model.VendorCode ?? string.Empty : model.VendorCodeList ?? string.Empty;
                if (model.RequestCategory == RequestCategory.Block)
                {
                    model.LabelPurchasing = model.PurchasingData ? "Block" : string.Empty;
                    model.LabelPosting = model.PostingData ? "Block" : string.Empty;
                }
                else
                {
                    model.LabelPurchasing = model.PurchasingData ? "UnBlock" : string.Empty;
                    model.LabelPosting = model.PostingData ? "UnBlock" : string.Empty;
                }

                if (!string.IsNullOrEmpty(model.ForwardTo))
                {
                    var arr = model.ForwardTo.Split('#');
                    if (arr.Length > 2)
                    {
                        model.DeptHead = arr[0];
                        model.DeptHeadEmail = arr[1];
                        model.DeptHeadName = arr[2];
                    }
                }

                string csvAttachment = string.Empty;

                // Call your business logic
                string strresult = VMOBJ.VENDORBLOCK_INFORMATION_INSERT(
                    reqCode, requestType, requestCategory, blockPurchasing, blockPosting,
                    vendorCode, csvAttachment, model.DeptHead, model.Remarks ?? string.Empty
                );

                string[] strstatus = strresult.Split('#');
                string resultmsgs = strstatus[0];
                string errmsg = strstatus.Length > 1 ? strstatus[1] : string.Empty;

                if (resultmsgs == "1")
                {
                    // Example: send mail
                    SENDMAILTODEPTHEAD(model);

                    model.ErrorFlag = false;
                    model.Message = "Your Request Submitted Successfully";
                }
                else
                {
                    model.ErrorFlag = true;
                    model.Message = errmsg.Replace("\n", "");
                }
            }
            catch (Exception ex)
            {
                model.ErrorFlag = true;
                model.Message = "Unexpected error: " + ex.Message;
            }

            return Json(model);
        }

        private void SENDMAILTODEPTHEAD(VendorBlockViewModel vmd)
        {
            string UserId = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
            string UserName = _sessionService.Get<string>("userName")?.ToString() ?? string.Empty;
            string deptHeadName = vmd.DeptHeadName ?? "";
            string deptHeadEmail = vmd.DeptHeadEmail ?? "";
            string requestType = vmd.RequestTypeText ?? "";
            string purchasing = vmd.LabelPurchasing ?? "";
            string posting = vmd.LabelPosting ?? "";

            if (!string.IsNullOrEmpty(deptHeadEmail))
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = deptHeadEmail.ToString();
                string strSubject = "Pending for the Vendor Approval";
                string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;padding-top:0px;font-family:Times New Roman;font-size:14px;line-height:21px;'><div style='width:700px;height:25px;background-color:skyblue;'><b>Vendor Block Request</b></div>"
                                + "<table cellpadding=0 cellspacing=0 border=0 width=700px style='font-size:14px;' >"
                                + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + deptHeadName + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                + "<tr><td colspan=2>&nbsp;&nbsp;Vendor block request is pending at your end for approval. Following are the details:</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                           + "<tr><td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Requestor</td>"
                + "<td width='200' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;" + UserName + "[" + UserId + "]" + "</td>"
                + "<td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Request Date</td>"
                + "<td width='300' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;'>&nbsp;" + DateTime.Now.ToString("dd-MMM-yyyy") + "</td></tr>"
                + "<tr><td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Request Type</td>"
                + "<td colspan='3'  width='200' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;" + requestType + "</td></tr>"
                + "<tr><td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Purchasing Data</td>"
                + "<td width='200' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;" + purchasing + "</td>"
                + "<td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Posting Data</td>"
                + "<td width='300' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;'>&nbsp;" + posting + "</td></tr></table>";

                strBody = strBody +
                "<table cellpadding=0 cellspacing=0 border=0 style='font-size:14px;'><tr><td><br/>&nbsp;&nbsp;Please login<a href=" + serverpath.getServerPath() + "index.aspx> E-Portal</a> for approval process." +
                "</td></tr><tr><td><br/><b>&nbsp;&nbsp;Thank You</b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Portal Admin<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                "</tr></table></td></tr><tr><td></td></tr><tr> " +
                "</tr></table></div> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
        }
        #endregion
        #region ManageBlockRequest
        public IActionResult ManageBlockRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetVendorBlockRequest(VendorBlockFilter filter)
        {
            var result = await VMOBJService.GetVendorBlockPendingRequests(filter);

            var data = result.ToList();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> ViewVendorBlock([FromQuery] string VMID)
        {
            string vendorId = WebUtility.UrlDecode(Encryption.Decrypt(VMID));
            DataSet ds = VMOBJ.VENDORBLOCKREQUESTBYID(vendorId);

            var model = new VendorBlockRequestViewModel();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                model.RequestorName = r["REQUESTORNAME"]?.ToString();
                model.RequestDate = r["REQUESTDATE"]?.ToString();
                model.RequestorEmail = r["EMAILID"]?.ToString();
                model.MobileNo = r["TMOBILE"]?.ToString();
                model.Site = r["SITE"]?.ToString();
                model.Operation = r["OPERATION"]?.ToString();
                model.Division = r["DIVISION"]?.ToString();
                model.Department = r["DEPARTMENT"]?.ToString();
                model.Section = r["SECTION"]?.ToString();
                model.RequestTypeDesc = r["REQUESTTYPEDESC"]?.ToString();
                model.PurchasingStatus = r["PURCHASINGSTATUS"]?.ToString();
                model.PostingStatus = r["POSTINGSTATUS"]?.ToString();
                model.Remarks = r["REMARKS"]?.ToString();
            }

            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    model.ProcessHistory.Add(new ProcessHistoryItem
                    {
                        AppDate = row["APPDATE"]?.ToString(),
                        Employee = row["EMPLOYEE"]?.ToString(),
                        StatusDescription = row["STATUSDESCRIPTION"]?.ToString(),
                        Remarks = row["Remarks"]?.ToString()
                    });
                }
            }

            if (ds != null && ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[2].Rows)
                {
                    model.Vendors.Add(new VendorListItem
                    {
                        VendorCode = row["VENDORCODE"]?.ToString(),
                        VendorName = row["VENDORNAME"]?.ToString(),
                        City = row["CITY"]?.ToString(),
                        Region = row["REGION"]?.ToString()
                    });
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditVendorBlockForm(string VMID, string returnUrl)
        {
            // fallback: if no returnUrl provided, use Referer header
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Request.Headers["Referer"].ToString();

            if (string.IsNullOrEmpty(VMID))
                return Redirect(returnUrl);

            string vendorId;
            try
            {
                vendorId = WebUtility.UrlDecode(Encryption.Decrypt(VMID));
            }
            catch
            {
                return Redirect(returnUrl);
            }

            if (string.IsNullOrEmpty(vendorId))
                return Redirect(returnUrl);

            DataSet ds = VMOBJ.VENDORBLOCKREQUESTBYID(vendorId);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return Redirect(returnUrl);
            DataSet dsDept = null;
            if (!string.IsNullOrEmpty(_userId))
            {
                dsDept = VMOBJ.GET_DEPARTMENTHEAD(_userId);
            }

            var row = ds.Tables[0].Rows[0];

            var model = new EditVendorBlockFormViewModel
            {
                VMID = VMID,
                VendorHeaderId = row["VENDORBLOCKHEADERID"].ToString(),
                RequestType = row["REQUESTTYPE"].ToString(),
                ProcessStatus = row["PROCESSSTATUS"].ToString(),
                RequestCategoryDesc = row["REQUESTCATEDESC"].ToString(),
                BlockPurchasing = row["BLOCKPURCHASING"].ToString(),
                BlockPosting = row["BLOCKPOSTING"].ToString(),
                SendBackRemark = ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0
                    ? "<b>" + ds.Tables[1].Rows[^1]["APPDESC"] + ": </b> " + ds.Tables[1].Rows[^1]["REMARKS"]
                    : string.Empty,
                DepartmentHeads = dsDept?.Tables[0].AsSelectList("EMPLOYEE", "DEPARTMENTHEAD", true, "--select--", "")
            };

            ViewData["VendorPayload"] = JsonSerializer.Serialize(model);
            return View(); // pass strongly typed model to view
        }
        [HttpPost]
        public IActionResult UpdateVendorBlock([FromBody] VendorBlockViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorFlag = true;
                model.Message = "Validation failed. Please check your inputs.";
                return Json(model);
            }

            try
            {
                if (string.IsNullOrEmpty(model.Remarks))
                {
                    model.ErrorFlag = true;
                    model.Message = "Remarks is mandatory field";
                    return Json(model);
                }
                if (string.IsNullOrEmpty(model.ForwardTo))
                {
                    model.ErrorFlag = true;
                    model.Message = "Select Forward To";
                    return Json(model);
                }
                string reqCode = _userId;

                string requestType = model.RequestType == RequestType.SingleVendor ? "1" : "2";
                string requestCategory = model.RequestCategory == RequestCategory.Block ? "1" : "2";

                string blockPurchasing = model.PurchasingData ? "1" : "0";
                string blockPosting = model.PostingData ? "1" : "0";

                string vendorCode = model.RequestType == RequestType.SingleVendor ? model.VendorCode ?? string.Empty : model.VendorCodeList ?? string.Empty;
                if (model.RequestCategory == RequestCategory.Block)
                {
                    model.LabelPurchasing = model.PurchasingData ? "Block" : string.Empty;
                    model.LabelPosting = model.PostingData ? "Block" : string.Empty;
                }
                else
                {
                    model.LabelPurchasing = model.PurchasingData ? "UnBlock" : string.Empty;
                    model.LabelPosting = model.PostingData ? "UnBlock" : string.Empty;
                }

                if (!string.IsNullOrEmpty(model.ForwardTo))
                {
                    var arr = model.ForwardTo.Split('#');
                    if (arr.Length > 2)
                    {
                        model.DeptHead = arr[0];
                        model.DeptHeadEmail = arr[1];
                        model.DeptHeadName = arr[2];
                    }
                }

                string csvAttachment = string.Empty;

                // Call your business logic
                string strresult = VMOBJ.VENDORBLOCK_INFORMATION_UPDATE(model.VendorHeaderId,
                    reqCode, requestType, requestCategory, blockPurchasing, blockPosting,
                    vendorCode, csvAttachment, model.DeptHead, model.Remarks ?? string.Empty
                );

                string[] strstatus = strresult.Split('#');
                string resultmsgs = strstatus[0];
                string errmsg = strstatus.Length > 1 ? strstatus[1] : string.Empty;

                if (resultmsgs == "1")
                {
                    model.ErrorFlag = false;
                    model.Message = "Your Request Submitted Successfully";
                }
                else
                {
                    model.ErrorFlag = true;
                    model.Message = errmsg.Replace("\n", "");
                }
            }
            catch (Exception ex)
            {
                model.ErrorFlag = true;
                model.Message = "Unexpected error: " + ex.Message;
            }

            return Json(model);
        }

        public IActionResult ManageBlockApprovalRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GetVendorBlockApprovalRequest(VendorBlockFilter filter)
        {
            var result = await VMOBJService.GetVendorBlockApprovalRequests(filter);

            var data = result.ToList();

            return Json(data);
        }
        public async Task<IActionResult> VendorBlockApprovalForm([FromQuery] string VMID)
        {
            string vendorId = WebUtility.UrlDecode(Encryption.Decrypt(VMID));
            DataSet ds = VMOBJ.VENDORBLOCKREQUESTBYID(vendorId);

            var model = new VendorBlockRequestViewModel();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var r = ds.Tables[0].Rows[0];
                model.VendorHeaderId = vendorId;
                model.RequestorName = r["REQUESTORNAME"]?.ToString();
                model.RequestDate = r["REQUESTDATE"]?.ToString();
                model.RequestorEmail = r["EMAILID"]?.ToString();
                model.MobileNo = r["TMOBILE"]?.ToString();
                model.Site = r["SITE"]?.ToString();
                model.Operation = r["OPERATION"]?.ToString();
                model.Division = r["DIVISION"]?.ToString();
                model.Department = r["DEPARTMENT"]?.ToString();
                model.Section = r["SECTION"]?.ToString();
                model.RequestTypeDesc = r["REQUESTTYPEDESC"]?.ToString();
                model.PurchasingStatus = r["PURCHASINGSTATUS"]?.ToString();
                model.PostingStatus = r["POSTINGSTATUS"]?.ToString();
                model.PurchasingId = r["BLOCKPURCHASING"]?.ToString();
                model.PostingId = r["BLOCKPOSTING"]?.ToString();
                model.Remarks = r["REMARKS"]?.ToString();
            }

            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    model.ProcessHistory.Add(new ProcessHistoryItem
                    {
                        AppDate = row["APPDATE"]?.ToString(),
                        Employee = row["EMPLOYEE"]?.ToString(),
                        StatusDescription = row["STATUSDESCRIPTION"]?.ToString(),
                        Remarks = row["Remarks"]?.ToString()
                    });
                }
            }

            if (ds != null && ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[2].Rows)
                {
                    model.Vendors.Add(new VendorListItem
                    {
                        VendorCode = row["VENDORCODE"]?.ToString(),
                        VendorName = row["VENDORNAME"]?.ToString(),
                        City = row["CITY"]?.ToString(),
                        Region = row["REGION"]?.ToString()
                    });
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveVendorBlock([FromBody] VendorBlockRequestViewModel model)
        { // ✅ Validation
          if (string.IsNullOrEmpty(model.Status)) 
                return Json(new { success = false, message = "Status is mandatory field" });
            if (string.IsNullOrEmpty(model.Remarks)) 
                return Json(new { success = false, message = "Remarks is mandatory field" }); 
            string ecode = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
            string msg = string.Empty;
            string strErr = string.Empty; 
            int errorcount = 0; // ✅ SAP update if posting = 0 and status = Approved

            if (model.PostingId == "0" && model.Status == "1")
            {
                DataSet ds = VMOBJ.VENDORBLOCKREQUESTSAP(model.VendorHeaderId);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string strvendorcode = ds.Tables[0].Rows[i]["VENDORCODE"].ToString();
                        string strpurchasing = ds.Tables[0].Rows[i]["BLOCKPURCHASING"].ToString();
                        string strposting = ds.Tables[0].Rows[i]["BLOCKPOSTING"].ToString();
                        msg = VMOBJ.UpdateVendorblockSAP(strvendorcode, strpurchasing, strposting);
                        strErr += "SAP - " + msg + " :" + strvendorcode + ",";
                        if (msg == "Invaild vendor code")
                        {
                            errorcount++;
                        }
                    }
                }
                if (errorcount != 0)
                {
                    return Json(new { success = false, message = strErr });
                }
                else
                {
                    //string strErrMsg = OBJVM.VENDORBLOCKREQAPPROVAL(VENDORHEADERID, strRemarks, status, Ecode, "1");
                    //string[] strmsg = strErrMsg.Split(new Char[] { '#' });
                    //string errResult = Convert.ToString(strmsg[0]);
                    //string errMsg = Convert.ToString(strmsg[1]);

                    string strErrMsg = VMOBJ.VENDORBLOCKREQAPPROVAL(model.VendorHeaderId, model.Remarks, model.Status, ecode, "1");
                    string[] strmsg = strErrMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(strmsg[0]);
                    string errMsg = Convert.ToString(strmsg[1]);
                    if (errResult == "1")
                    {
                        SendVendorBlockApprovalMail(model);
                        string successMessage = model.Status switch
                        {
                            "1" => "Vendor Request has been Approved Successfully",
                            "2" => "Vendor Request has been Rejected Successfully",
                            "3" => "Vendor Request has been Send Back Successfully",
                            _ => "Vendor Request processed successfully"
                        };
                        return Json(new { success = true, message = successMessage, redirect = Url.Action("ManageBlockApprovalRequest", "VendorMaster") });
                    }
                    else
                    {
                        //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('" + errMsg.Replace("\n", "") + "');", true);                       
                        return Json(new { success = false, message = strmsg });
                    }

                }
            }
            else {
                string strErrMsg = VMOBJ.VENDORBLOCKREQAPPROVAL(model.VendorHeaderId, model.Remarks, model.Status, ecode, "1");
                string[] strmsg = strErrMsg.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strmsg[0]);
                string errMsg = Convert.ToString(strmsg[1]);
                if (errResult == "1")
                {
                    // 🔗 Integrated sendmail logic
                    SendVendorBlockApprovalMail(model);
                    string successMessage = model.Status switch
                    {
                        "1" => "Vendor Request has been Approved Successfully",
                        "2" => "Vendor Request has been Rejected Successfully",
                        "3" => "Vendor Request has been Send Back Successfully",
                        _ => "Vendor Request processed successfully"
                    };
                    return Json(new { success = true, message = successMessage, redirect = Url.Action("ManageBlockApprovalRequest", "VendorMaster") });
                }
                else
                {
                    return Json(new { success = false, message = errMsg });
                }
            }
          
          
        }

        private void SendVendorBlockApprovalMail(VendorBlockRequestViewModel model)
        {
            string REQUESTORNAME = model.RequestorName;
            string REQUESTOREMAILID = model.RequestorEmail;
            string REQUESTTYPE = model.RequestTypeDesc;
            string PURCHASING = model.PurchasingStatus;
            string POSTING = model.PostingId;
            string REQUESTDATE = model.RequestDate;
            string UserId = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
            string UserName = _sessionService.Get<string>("userName")?.ToString() ?? string.Empty;
            string REMARKS = model.Remarks;

            //---------------------REQUEST REJECTED MAIL ON REQUESTOR----------------------------
            if (model.Status == "2")
            {
                if (REQUESTOREMAILID.Trim() != "")
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = REQUESTOREMAILID.ToString();
                    string strSubject = "Vendor Block Request Rejected";
                    string strBody = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Vendor Block Request</b></div>"
                                    + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                    + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + REQUESTORNAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                    + "<tr><td colspan=2>&nbsp;&nbsp;Your request for vendor block has been rejected by " + UserName + ". Following are the details:</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                    strBody += "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;...'>Request Type</td>"
                    + "<td width='220' style='background-color:white;...'>" + REQUESTTYPE + "</td>"
                    + "<td width='130' style='background-color:#CAE8EA;...'>Request Date</td>"
                    + "<td width='220' style='background-color:white;...'>" + REQUESTDATE + "</td></tr>"
                    + "<tr><td width='100' style='background-color:#CAE8EA;...'>Purchasing Data</td>"
                    + "<td width='200' style='background-color:white;...'>" + PURCHASING + "</td>"
                    + "<td width='100' style='background-color:#CAE8EA;...'>Posting Data</td>"
                    + "<td width='300' style='background-color:white;...'>" + POSTING + "</td></tr>"
                    + "<td style='background-color:#CAE8EA;...'>Reason</td>"
                    + "<td colspan='3' style='background-color:white;...'>" + REMARKS + "</td></tr></table>";

                    strBody += "<table cellpadding=0 cellspacing=0 border=0 >"
                    + "<tr><td><br/><b>&nbsp;&nbsp;Thank You</b><br/></td></tr>"
                    + "<tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                    + "<tr><td>&nbsp;&nbsp;Portal Admin<br/></td></tr>"
                    + "<tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>"
                    + "</tr></table></div>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
            }

            //---------------------REQUEST SEND BACK MAIL ON REQUESTOR----------------------------
            if (model.Status == "3")
            {
                if (REQUESTOREMAILID.Trim() != "")
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = REQUESTOREMAILID.ToString();
                    string strSubject = "Vendor Block Request Send Back";
                    string strBody = "<div style='width:700px;border:2px skyblue solid;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Vendor Master Request</b></div>"
                                    + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                    + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + REQUESTORNAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                    + "<tr><td colspan=2>&nbsp;&nbsp;Your request for vendor block has been send back by " + UserName + ". Following are the details:</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                    strBody += "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;...'>Request Type</td>"
                    + "<td width='220' style='background-color:white;...'>" + REQUESTTYPE + "</td>"
                    + "<td width='130' style='background-color:#CAE8EA;...'>Request Date</td>"
                    + "<td width='220' style='background-color:white;...'>" + REQUESTDATE + "</td></tr>"
                    + "<tr><td width='100' style='background-color:#CAE8EA;...'>Purchasing Data</td>"
                    + "<td width='200' style='background-color:white;...'>" + PURCHASING + "</td>"
                    + "<td width='100' style='background-color:#CAE8EA;...'>Posting Data</td>"
                    + "<td width='300' style='background-color:white;...'>" + POSTING + "</td></tr>"
                    + "<td style='background-color:#CAE8EA;...'>Reason</td>"
                    + "<td colspan='3' style='background-color:white;...'>" + REMARKS + "</td></tr></table>";

                    strBody += "<table cellpadding=0 cellspacing=0 border=0 >"
                    + "<tr><td><br/>&nbsp;&nbsp;<b>Login to <a href='https:\\portal.honda2wheelersindia.com'>Employee Portal</a> follow the following link:</b></td></tr>"
                    + "<tr><td>&nbsp;&nbsp;Master Data Governance => Manage Request</td></tr>"
                    + "<tr><td><br/><b>&nbsp;&nbsp;Thank You<br/></b></td></tr>"
                    + "<tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr>"
                    + "<tr><td>&nbsp;&nbsp;Portal Admin<br/></td></tr>"
                    + "<tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>"
                    + "</tr></table></div>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
            }
        }
        #endregion
        #endregion
    }
}
