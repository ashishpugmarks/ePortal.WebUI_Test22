using ClosedXML.Excel;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Text.RegularExpressions;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class ThirdpartyEmployeeController : Controller
    {
        //private JavaScriptSerializer oSerializer = new JavaScriptSerializer();
        private string sJSON = String.Empty;
        private ISessionService _session;
        private ILogger<ThirdpartyEmployeeController> _logger;

        IThirdpartyEmployeeService _Is;
       public ThirdpartyEmployeeController(IThirdpartyEmployeeService thirdpartyEmployee, ISessionService session, ILogger<ThirdpartyEmployeeController> logger)
        {
            _session = session;
            _logger = logger;
            _Is = thirdpartyEmployee;
        }
        public IActionResult ThirdpartyEmployeeList()
        {
            //Added By TTL
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            bool DisabledDropdown = false;
            int UserID = int.Parse(_session.Get<string>("userID"));
            Employee_Details _Employee_Details = (Employee_Details)_session.Get<Employee_Details>("Employee");
            var Data = _Is.GetOwnershipList(UserID);
            List<SelectListItem> OwnershipList = new List<SelectListItem>();
            if (Data != null && Data.Count > 0)
            {
                foreach (var item in Data)
                {
                    OwnershipList.Add(new SelectListItem { Text = item.Value.ToString() + " - " + item.Text, Value = item.Value.ToString() });

                    if (Data.Count == 1 && item.Value.ToString() == UserID.ToString())
                    {
                        DisabledDropdown = true;
                    }
                }
            }
            ViewBag.OwnshipList = OwnershipList;
            ViewBag.DisabledDropdown = DisabledDropdown;
            //End by TTL
            return View();
        }
        [HttpGet]
        public IActionResult GetThirdpartyEmployeeList()
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int UserID = int.Parse(_session.Get<string>("userID"));
            var ilist = _Is.GetThirdpartyEmployeeList(UserID);
            sJSON =JsonConvert.SerializeObject(ilist);
            //return Json(sJSON, JsonRequestBehavior.AllowGet);
            return Json(sJSON);
        }


        #region  AutoSuggestions
        [HttpGet]
        public IActionResult AutocompleteCompanyName(string term)
        {
            var suggestions = _Is.GetCompanyName(term);
            return Json(suggestions);
        }
        [HttpGet]
        public IActionResult AutoCompleteProjectManager(string Companyname, string term)
        {
            var suggestions = _Is.GetprojectmanagerName(Companyname, term);
            //return Json(suggestions, JsonRequestBehavior.AllowGet);
            return Json(suggestions);
        }
        [HttpGet]
        public ActionResult AutoCompleteAccountManager(string Companyname, string term)
        {
            var suggestions = _Is.GetAccountmanagerName(Companyname, term);
            return Json(suggestions);
        }
        [HttpGet]
        public ActionResult AutoCompleteProjectManagerMailid(string ProjectManagerName, string term)
        {
            var suggestions = _Is.Getprojectmanagermailid(ProjectManagerName, term);
            return Json(suggestions);
        }
        [HttpGet]
        public ActionResult AutoCompleteAccountManagerMailid(string AccountManagerName, string term)
        {
            var suggestions = _Is.GetAccountManagermailid(AccountManagerName, term);
            return Json(suggestions);
        }

        public IActionResult GetEcode()
        {
            var ecode = _Is.GetEcode();
            sJSON =JsonConvert.SerializeObject(ecode);
            return Json(sJSON);
        }
        [HttpGet]
        public IActionResult AutocompleteOwner(string term)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            LibResult res = new LibResult();
            int UserID = int.Parse(_session.Get<string>("userID"));
            var suggestions = _Is.GetOwnership(term, UserID);
            var result=JsonConvert.SerializeObject(suggestions);
            return Json(result);
        }


        #endregion
        [HttpGet]
        public IActionResult ThirdpartyEmployeeAdd()
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            bool DisabledDropdown = false;
            int UserID = int.Parse(_session.Get<string>("userID"));
            Employee_Details _Employee_Details = (Employee_Details)_session.Get<Employee_Details>("Employee");
            var Data = _Is.GetOwnershipList(UserID);
            List<SelectListItem> OwnershipList = new List<SelectListItem>();
            if (Data != null && Data.Count > 0)
            {
                foreach (var item in Data)
                {
                    OwnershipList.Add(new SelectListItem { Text = item.Value.ToString() + " - " + item.Text, Value = item.Value.ToString() });

                    if (Data.Count == 1 && item.Value.ToString() == UserID.ToString())
                    {
                        DisabledDropdown = true;
                    }
                }
            }

            //var functionalDesignations = new[] { "2", "3", "4" };
            //if (_Employee_Details.Functional_Designation_Id != null &&
            //    functionalDesignations.Any(id => _Employee_Details.Functional_Designation_Id.Contains(id)))
            //{
            ViewBag.OwnshipList = OwnershipList;
            //}
            //else
            //{
            //    ViewBag.OwnshipList = null;
            //}

            ViewBag.DisabledDropdown = DisabledDropdown;
            return View();
        }

        [HttpPost]
        //public IActionResult ThirdpartyEmployeeAdd(string Data)
        public IActionResult ThirdpartyEmployeeAdd([FromBody] ThirdpartyEmployeeViewModel obj)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            short retVal = -1;

            try
            {
                //ThirdpartyEmployeeViewModel obj = JsonConvert.DeserializeObject<ThirdpartyEmployeeViewModel>(Data);
                string UserID = _session.Get<string>("userID");
                retVal = _Is.AddThirdpartyEmployeeData(obj, UserID);
                return Json(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError("ThirdpartyEmployeeAdd", ex.Message);
                return Json(new { res = retVal, error = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult ThirdpartyEmployeeEdit(long Ecode, string flag)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            bool DisabledDropdown = false;
            LibResult res = new LibResult();
            int UserID = int.Parse(_session.Get<string>("userID"));
            ThirdpartyEmployeeViewModel Thirdpartryemplist = _Is.GetThirdpartyEmployeeList(UserID).Where(x => x.Ecode == Ecode).FirstOrDefault();
            ViewBag.flag = flag;

            Employee_Details _Employee_Details = (Employee_Details)_session.Get<Employee_Details>("Employee");
            var Data = _Is.GetOwnershipList(UserID);
            List<SelectListItem> OwnershipList = new List<SelectListItem>();
            if (Data != null && Data.Count > 0)
            {
                foreach (var item in Data)
                {
                    OwnershipList.Add(new SelectListItem { Text = item.Value.ToString() + " - " + item.Text, Value = item.Value.ToString() });

                    if (Data.Count == 1 && item.Value.ToString() == UserID.ToString())
                    {
                        DisabledDropdown = true;
                    }
                }
            }

            //var functionalDesignations = new[] { "2", "3", "4" };
            //if (_Employee_Details.Functional_Designation_Id != null &&
            //    functionalDesignations.Any(id => _Employee_Details.Functional_Designation_Id.Contains(id)))
            //{
            ViewBag.OwnshipList = OwnershipList;
            //}
            //else
            //{
            //    ViewBag.OwnshipList = null;
            //}
            ViewBag.DisabledDropdown = DisabledDropdown;
            return View(Thirdpartryemplist);
        }

        [HttpPost]
        //public ActionResult ThirdpartyEmployeeEdit(string Data)
        public ActionResult ThirdpartyEmployeeEdit([FromBody]ThirdpartyEmployeeViewModel obj)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            short retVal = -1;
            try
            {
                //ThirdpartyEmployeeViewModel obj = JsonConvert.DeserializeObject<ThirdpartyEmployeeViewModel>(Data);
                string userID = _session.Get<string>("userID");

                retVal = _Is.EditThirdpartyempData(obj, userID);
                return Json(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError("ThirdpartyEmployeeEdit", ex.Message);
                return Json(new { res = retVal, error = ex.Message });
            }
        }
        [HttpGet]
        public ActionResult ThirdpartyEmployeeView(long Ecode)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int UserID = int.Parse(_session.Get<string>("userID"));
            ThirdpartyEmployeeViewModel Thirdpartryemplist = _Is.GetThirdpartyEmployeeList(UserID).Where(x => x.Ecode == Ecode).FirstOrDefault();
            return View(Thirdpartryemplist);
        }
        [HttpGet]
        public JsonResult ThirdpartyEmployeeDelete(long Ecode)
        {
            short retVal = -1;
            try
            {
                retVal = _Is.DeleteThirdpartyEmp(Ecode);
                return Json(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { res = retVal, error = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult DeboardEmployee(long Ecode)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            short retVal = -1;
            try
            {
                int UserID = int.Parse(_session.Get<string>("userID"));
                retVal = _Is.DeboardEmployee(Ecode, UserID);
                return Json(new { res = retVal, error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { res = retVal, error = ex.Message });
            }
        }


        // Added by TTL SR94104 - CR6022
        public ActionResult DownloadTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                // First sheet - for employee template
                var employeeSheet = workbook.Worksheets.Add("ThirdPartyEmployeeData");

                string[] headers = new string[]
                {
                    "Associate Name", "Designation", "Company Name",
                    "Associate Company ID", "Project Manager Name", "Project Manager Mail ID",
                    "Account Manager Name", "Account Manager Mail ID", "Ownership"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    employeeSheet.Cell(1, i + 1).Value = headers[i];
                }

                // Make header row bold
                employeeSheet.Range("A1:I1").Style.Font.Bold = true;

                // Adjust column width to fit content
                employeeSheet.Columns().AdjustToContents();


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ThirdPartyEmpUploadTemplate.xlsx"
                    );
                }
            }
        }

        [HttpPost]
        public IActionResult UploadMappings(List<IFormFile> fp)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var mappings = new List<ThirdpartyEmpUploadModel>();
            var seenCombinations = new HashSet<string>(); // Track duplicates
            var duplicateErrors = new List<string>(); // Collect duplicate messages
            var validationErrors = new List<string>(); // Collect validation errors
            var invalidEmails = new List<string>(); // Collect invalid email
            var existRecords = new List<string>(); // Collect already exist records in DB
            var invalidOwner = new List<string>(); // Collect invalid owner

            if (fp.Count > 0)
            {
                var file = fp[0];
                using (var stream = file.OpenReadStream())
                {
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheets.First();
                        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

                        foreach (var row in rows)
                        {
                            string associateName = row.Cell(1).Value.ToString().Trim();
                            string designation = row.Cell(2).Value.ToString().Trim();
                            string companyName = row.Cell(3).Value.ToString().Trim();
                            string associateCompanyID = row.Cell(4).Value.ToString().Trim();
                            string projectManagerName = row.Cell(5).Value.ToString().Trim();
                            string projectManagerEmail = row.Cell(6).Value.ToString().Trim();
                            string accountManagerName = row.Cell(7).Value.ToString().Trim();
                            string accountManagerEmail = row.Cell(8).Value.ToString().Trim();
                            string ownership = row.Cell(9).Value.ToString().Trim();

                            int errorFlag = 0;

                            // Validation: Ensure no empty cells in required columns
                            if (string.IsNullOrEmpty(associateName) || string.IsNullOrEmpty(designation) ||
                                string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(associateCompanyID) ||
                                string.IsNullOrEmpty(projectManagerName) || string.IsNullOrEmpty(projectManagerEmail) ||
                                string.IsNullOrEmpty(accountManagerName) || string.IsNullOrEmpty(accountManagerEmail) ||
                                string.IsNullOrEmpty(ownership))
                            {
                                validationErrors.Add($"Missing data in row {row.RowNumber() - 1}");
                                errorFlag = 1;
                            }

                            // Validate email format
                            if (!IsValidEmail(projectManagerEmail) || !IsValidEmail(accountManagerEmail))
                            {
                                invalidEmails.Add($"Invalid email format in row {row.RowNumber() - 1}");
                                errorFlag = 1;
                            }

                            // Check for duplicates in Excel file
                            string key = $"{associateCompanyID}-{projectManagerEmail}-{accountManagerEmail}";
                            if (seenCombinations.Contains(key))
                            {
                                duplicateErrors.Add($"Duplicate entry found in row {row.RowNumber() - 1}");
                                errorFlag = 2;
                            }
                            else
                            {
                                seenCombinations.Add(key);
                            }

                            // Check for existing records in the database
                            long existsEmpCode = _Is.GetExistingEmpCode(associateCompanyID, companyName);

                            if (existsEmpCode != 0)
                            {
                                existRecords.Add($"Entry already exists in DB for row {row.RowNumber() - 1}");
                                errorFlag = 3; // New error flag for existing records
                            }

                            // Check for Ownership validation
                            string ownerEmpName = ownership == "" ? "" : _Is.GetValidEmployee(ownership);
                            if (ownerEmpName == "")
                            {
                                invalidOwner.Add($"Invalid Ownership for row {row.RowNumber() - 1}");
                                errorFlag = 1;
                            }

                            // Add validated data to the mappings list
                            mappings.Add(new ThirdpartyEmpUploadModel
                            {
                                AssociateName = associateName,
                                Designation = designation,
                                CompanyName = companyName,
                                AssociateCompanyId = associateCompanyID,
                                ProjectManagerName = projectManagerName,
                                ProjectManagerMailID = projectManagerEmail,
                                AccoountManagerName = accountManagerName,
                                AccountManagerMailID = accountManagerEmail,
                                OwnerEmpCode = ownership,
                                OwnerEmpName = ownerEmpName.Trim(),
                                ErrorFlag = errorFlag
                            });
                        }
                    }
                }
            }

            return Json(new { mappings, duplicateErrors, validationErrors, invalidEmails, existRecords, invalidOwner });
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        [HttpPost]
        public ActionResult BulkIdUpload(List<ThirdpartyEmpUploadModel> employeeDetails)
        {
            if (_session.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            short retVal = 0;
            string errmsg = string.Empty;
            List<ThirdpartyEmpUploadModel> result = new List<ThirdpartyEmpUploadModel>();
            try
            {
                string UserID = _session.Get<string>("userID");
                result = _Is.BulkUploadEmployeeDetails(employeeDetails, UserID);
                if (result.Count > 0)
                {
                    retVal = 1;
                }
                else
                {
                    retVal = 0;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
                errmsg = ex.InnerException.ToString();
            }
            return Json(new
            {
                resFlag = retVal,
                err = errmsg,
                res = result,
            });
            // End by TTL
        }

        [HttpPost]
        public ActionResult ExportToExcel(TableDataModel dataModel)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Associates");

                worksheet.Cell(1, 1).Value = "Associate Unique ID";
                worksheet.Cell(1, 2).Value = "Associate Name";
                worksheet.Cell(1, 3).Value = "Designation";
                worksheet.Cell(1, 4).Value = "Company Name";
                worksheet.Cell(1, 5).Value = "Associate Company ID";
                worksheet.Cell(1, 6).Value = "Project Manager Name";
                worksheet.Cell(1, 7).Value = "Project Manager Mail ID";
                worksheet.Cell(1, 8).Value = "Account Manager Name";
                worksheet.Cell(1, 9).Value = "Account Manager Mail ID";
                worksheet.Cell(1, 10).Value = "Ownership Id";
                worksheet.Cell(1, 11).Value = "Ownership Name";

                for (int i = 0; i < dataModel.TableData.Count; i++)
                {
                    for (int j = 0; j < dataModel.TableData[i].Length; j++)
                    {
                        worksheet.Cell(i + 2, j + 1).Value = dataModel.TableData[i][j].ToString();
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin); // FIX: Ensure stream position is reset before returning

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Associates.xlsx");
                }
            }
        }
        // End by TTL SR94104 - CR6022

    }
}
