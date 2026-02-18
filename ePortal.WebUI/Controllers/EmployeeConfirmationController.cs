using DocumentFormat.OpenXml.Office2021.PowerPoint.Comment;
using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.EmployeeConfirmation;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using System;
using System.Data;

namespace ePortal.WebUI.Controllers
{
    [SessionTimeout]
    [CSPFilter]
    public class EmployeeConfirmationController : Controller
    {
        private readonly IHRConfirmationReview _confirmationReviewService;
        private readonly ISessionService _sessionService;
        private readonly ICommonFunctions _commonFunctionsService;
        private readonly ILogger<EmployeeConfirmationController> _logger;

        public EmployeeConfirmationController(IHRConfirmationReview confirmationReviewService, 
                                              ISessionService objsessionService,
                                              ICommonFunctions commonFunctionsService,
                                              ILogger<EmployeeConfirmationController> logger)
        {
            _confirmationReviewService = confirmationReviewService;
            _sessionService = objsessionService;
            _commonFunctionsService = commonFunctionsService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult PendingConfirmations()
        {
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            var empDetails = _sessionService.Get<Employee_Details>("Employee");

            var dataTable = _confirmationReviewService.GetConfirmationList("1", userCode,
                string.IsNullOrEmpty(empDetails?.Operation_Id) ? "0" : empDetails.Operation_Id,
                string.IsNullOrEmpty(empDetails?.Division_Id) ? "0" : empDetails.Division_Id,
                string.IsNullOrEmpty(empDetails?.Department_Id) ? "0" : empDetails.Department_Id);

            //var viewModel = _confirmationHelper.BuildConfirmationRows(dataTable, empDetails.Employee_Code);
            var viewModel = new List<ConfirmationRowViewModel>();

            foreach (DataRow row in dataTable.Rows)
            {
                var model = new ConfirmationRowViewModel
                {
                    HRConfirmationReviewId = row["HRCONFIRMATIONREVIEWID"].ToString(),
                    ADEmpCode = row["ADEMPCODE"].ToString(),
                    EName = row["ENAME"].ToString(),
                    Reviewer1Status = row["REVIEWER1STATUS"].ToString(),
                    Reviewer2Status = row["REVIEWER2STATUS"].ToString(),
                    Reviewer3Status = row["REVIEWER3STATUS"].ToString(),
                    HRStatus = row["HRAPPROVAL"].ToString(),
                    DeptStatus = row["DEPT_STATUS"].ToString(),
                    DivStatus = row["DIV_STATUS"].ToString(),
                    OpStatus = row["OP_STATUS"].ToString(),
                    HRApprovalStatus = row["HRApprovalStatus"].ToString(),
                    HRRemarks = row["HRREMARKS"].ToString(),
                    OpName = row["Op_Name"].ToString(),
                    DivName = row["Div_Name"].ToString(),
                    DeptName = row["Dept_Name"].ToString(),
                    SecName = row["Sec_Name"].ToString(),
                    ConfirmationDate = row["DOC"].ToString(),
                    ProcessStatus = row["Proc_Status"].ToString()
                };

                model.PhotoUrl = BuildPhotoUrl(model.ADEmpCode);

                // 🔹 2. HOLD / HR tooltip
                model.IsHoldByHR = (model.ProcessStatus == "1" || model.HRStatus == "3");
                if (model.IsHoldByHR)
                {
                    // old code: "->Hold By HR.<br>" + HRRemarks, then Replace("<br>", Environment.NewLine)
                    model.HoldTooltip = "->Hold By HR." + Environment.NewLine + model.HRRemarks;
                }

                // Normalize empty statuses
                model.Reviewer1Status = string.IsNullOrEmpty(model.Reviewer1Status) ? "4" : model.Reviewer1Status;
                model.Reviewer2Status = string.IsNullOrEmpty(model.Reviewer2Status) ? "4" : model.Reviewer2Status;
                model.Reviewer3Status = string.IsNullOrEmpty(model.Reviewer3Status) ? "4" : model.Reviewer3Status;

                // Reviewer 1
                if (model.Reviewer1Status == "4")
                {
                    model.Reviewer1ImageUrl = "~/images/notapplicable20.png";
                    model.Reviewer1Link = "";
                }
                else if (model.Reviewer1Status == "0") // Pending
                {
                    if (empDetails.Employee_Code == row["REVIEWER1"].ToString())
                    {
                        model.Reviewer1ImageUrl = "~/images/system-users.png";
                        model.Reviewer1Link = $"ReviewForm?id={model.HRConfirmationReviewId}&ecode={model.ADEmpCode}";
                    }
                    else
                    {
                        model.Reviewer1ImageUrl = "~/images/error_do_not.png";
                        model.Reviewer1Link = "";
                    }
                }
                else
                {
                    if (empDetails.Employee_Code == row["REVIEWER1"].ToString())
                    {
                        model.Reviewer1ImageUrl = "~/images/applications.png";
                        model.Reviewer1Link = $"ReviewForm?id={model.HRConfirmationReviewId}&ecode={model.ADEmpCode}";
                    }
                    else
                    {
                        model.Reviewer1ImageUrl = "~/images/checkmark-korganizer.png";
                        model.Reviewer1Link = "";
                    }
                }

                // Reviewer 2
                if (model.Reviewer2Status == "4")
                {
                    model.Reviewer2ImageUrl = "~/images/notapplicable20.png";
                    model.Reviewer2Link = "";
                }
                else if (model.Reviewer2Status == "0" && model.Reviewer1Status == "0")
                {
                    model.Reviewer2ImageUrl = "~/images/error_do_not.png";
                    model.Reviewer2Link = "";
                }
                else if (model.Reviewer1Status != "0" && model.Reviewer2Status == "0")
                {
                    if (empDetails.Employee_Code == row["REVIEWER2"].ToString())
                    {
                        model.Reviewer2ImageUrl = "~/images/system-users.png";
                        model.Reviewer2Link = $"ReviewForm?id={model.HRConfirmationReviewId}&ecode={model.ADEmpCode}";
                    }
                    else
                    {
                        model.Reviewer2ImageUrl = "~/images/error_do_not.png";
                        model.Reviewer2Link = "";
                    }
                }
                else if (model.Reviewer2Status != "0")
                {
                    if (empDetails.Employee_Code == row["REVIEWER2"].ToString())
                    {
                        model.Reviewer2ImageUrl = "~/images/applications.png";
                        model.Reviewer2Link = $"ReviewForm?id={model.HRConfirmationReviewId}&ecode={model.ADEmpCode}";
                    }
                    else
                    {
                        model.Reviewer2ImageUrl = "~/images/checkmark-korganizer.png";
                        model.Reviewer2Link = "";
                    }
                }

                // Reviewer 3
                if (model.Reviewer3Status == "4")
                {
                    model.Reviewer3ImageUrl = "~/images/notapplicable20.png";
                    model.Reviewer3Link = "";
                }
                else if (model.Reviewer3Status == "0" && model.Reviewer2Status == "0")
                {
                    model.Reviewer3ImageUrl = "~/images/error_do_not.png";
                    model.Reviewer3Link = "";
                }
                else if (model.Reviewer2Status != "0" && model.Reviewer3Status == "0")
                {
                    if (empDetails.Employee_Code == row["REVIEWER3"].ToString())
                    {
                        model.Reviewer3ImageUrl = "~/images/system-users.png";
                        model.Reviewer3Link = $"ReviewForm?id={model.HRConfirmationReviewId}&ecode={model.ADEmpCode}";
                    }
                    else
                    {
                        model.Reviewer3ImageUrl = "~/images/error_do_not.png";
                        model.Reviewer3Link = "";
                    }
                }
                else if (model.Reviewer3Status != "0")
                {
                    if (empDetails.Employee_Code == row["REVIEWER3"].ToString())
                    {
                        model.Reviewer3ImageUrl = "~/images/applications.png";
                        model.Reviewer3Link = $"ReviewForm?id={model.HRConfirmationReviewId}&ecode={model.ADEmpCode}";
                    }
                    else
                    {
                        model.Reviewer3ImageUrl = "~/images/checkmark-korganizer.png";
                        model.Reviewer3Link = "";
                    }
                }

                // HR
                int r1 = Convert.ToInt32(model.Reviewer1Status);
                int r2 = Convert.ToInt32(model.Reviewer2Status);
                int r3 = Convert.ToInt32(model.Reviewer3Status);

                if ((r1 * r2 * r3 == 0) && model.HRStatus == "0")
                {
                    model.HRImageUrl = "~/images/error_do_not.png";
                    model.HRLink = "";
                }
                else if ((r1 * r2 * r3 != 0) && model.HRStatus == "0")
                {
                    model.HRImageUrl = "~/images/system-users.png";
                    //model.HRLink = $"ReviewForm?id={model.HRConfirmationReviewId}&ecode={model.ADEmpCode}";
                    model.HRLink = "";
                }
                else if (model.HRStatus != "0")
                {
                    model.HRImageUrl = ""; // Hide image
                    model.HRLink = "";
                }

                viewModel.Add(model);
            }

            return View(viewModel);
        }



        [HttpGet]
        public IActionResult ConfirmationsHistory()
        {
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            var empDetails = _sessionService.Get<Employee_Details>("Employee");

            var dataTable = _confirmationReviewService.GetConfirmationHistory("1",
                string.IsNullOrEmpty(empDetails?.Operation_Id) ? "0" : empDetails.Operation_Id,
                string.IsNullOrEmpty(empDetails?.Division_Id) ? "0" : empDetails.Division_Id,
                string.IsNullOrEmpty(empDetails?.Department_Id) ? "0" : empDetails.Department_Id);

            var viewModel = new List<ConfirmationHistoryViewModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                var model = new ConfirmationHistoryViewModel
                //model.Add(new ConfirmationHistoryViewModel
                {
                    HRConfirmationReviewId = row["HRCONFIRMATIONREVIEWID"].ToString(),
                    EmpCode = row["ADEMPCODE"].ToString(),
                    Name = row["ENAME"].ToString(),
                    ConfirmationDate = row["DOC"].ToString(),
                    DeptStatus = row["DEPT_STATUS"].ToString(),
                    DivStatus = row["DIV_STATUS"].ToString(),
                    OpStatus = row["OP_STATUS"].ToString(),
                    HRApprovalStatus = row["HRApprovalStatus"].ToString(),
                    DeptName = row["Dept_Name"].ToString(),
                    DivName = row["Div_name"].ToString(),
                    SecName = row["Sec_Name"].ToString(),
                    OpName = row["Op_Name"].ToString()
                    //PhotoUrl = BuildPhotoUrl(row["ADEMPCODE"].ToString())
                    // Add more fields as needed
                };
                model.PhotoUrl = BuildPhotoUrl(model.EmpCode);

                viewModel.Add(model);
            }

            return View(viewModel);

        }


        // ===================== GET =====================
        [HttpGet]
        public IActionResult ReviewFormDetails(string id)
        
        {
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            ReviewFormDetailsViewModel viewModel = new ReviewFormDetailsViewModel();

            // ---------- Main Details ----------
            DataSet ds = _confirmationReviewService.GetProcessDetails(id);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];

                viewModel.ConfirmationFormID = dr["ConfirmationFormID"]?.ToString();
                viewModel.EmpCode = dr["ADEMPCODE"]?.ToString();
                viewModel.Name = dr["ENAME"]?.ToString();
                viewModel.Designation = dr["DESIGNATION"]?.ToString();
                viewModel.Department = dr["DEPTNAME"]?.ToString();
                viewModel.Division = dr["DIVNAME"]?.ToString();
                viewModel.Section = dr["SECTION"]?.ToString();
                viewModel.JoiningDate = dr["REGDATE"]?.ToString();
                viewModel.InductionStatus = dr["INDUCTION_PROG_STATUS"]?.ToString();
                viewModel.InductionDate = dr["TRAININGSTARTDATE"]?.ToString();
                viewModel.PreviousExtensionPeriod = dr["ExPeriod"]?.ToString();
                viewModel.ReviewDate = dr["REVIEWDATE"]?.ToString();

                viewModel.DeptRemarks = dr["DEPTREMARKS"]?.ToString();
                viewModel.DivRemarks = dr["DIVREMARKS"]?.ToString();
                viewModel.OpRemarks = dr["OPHREMARKS"]?.ToString();

                viewModel.DeptApprovalStatus = dr["DEPTAPPROVAL"]?.ToString();
                viewModel.DivApprovalStatus = dr["DIVAPPROVAL"]?.ToString();
                viewModel.OpApprovalStatus = dr["OPHAPPROVAL"]?.ToString();
                viewModel.HRApprovalStatus = dr["HRAPPROVEDSTATUS"]?.ToString();

                viewModel.HRStatus = dr["HRAPPROVAL"]?.ToString();
                viewModel.HRRemarks = dr["HRREMARKS"]?.ToString();
                viewModel.FunctionalDesignationID = dr["ADFUNCTIONALDESIGNATIONID"]?.ToString();

                //New
                viewModel.DVDeptStatus = dr["DeptStatus"]?.ToString();
                viewModel.OPDivHeadStatus = dr["DivStatus"]?.ToString();

            }


            // ====== SETTAB EQUIVALENT ======

            var employee = _sessionService.Get<Employee_Details>("Employee");
            var loginDesig = employee?.Functional_Designation_Id;
            var loginEmpCode = employee?.Employee_Code;

            // Heads come from ds.Tables[1] like old dtEmpSupDetails
            string deptHead = null, divHead = null, opHead = null;
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var supRow = ds.Tables[1].Rows[0];
                deptHead = supRow["DEP_HEAD"]?.ToString();
                divHead = supRow["DIV_HEAD"]?.ToString();
                opHead = supRow["OP_HEAD"]?.ToString();
            }

            // By default, everything is read-only and no active tab
            viewModel.ActiveTab = "NULL";
            viewModel.IsDeptReadOnly = true;
            viewModel.IsDivReadOnly = true;
            viewModel.IsOpReadOnly = true;

            switch (loginDesig)
            {
                case "2": // Dept Head
                    if (!string.IsNullOrEmpty(deptHead) && deptHead == loginEmpCode)
                    {
                        viewModel.ActiveTab = "DEPT";
                        viewModel.IsDeptReadOnly = false;
                    }
                    break;

                case "3": // Div Head
                    if (!string.IsNullOrEmpty(divHead) && divHead == loginEmpCode)
                    {
                        viewModel.ActiveTab = "DIV";
                        viewModel.IsDivReadOnly = false;
                    }
                    break;

                case "4": // Operating Head
                    var opHeadCode = opHead;
                    if (string.IsNullOrEmpty(opHeadCode))
                    {
                        // equivalent to objCommon.GetParameterValue("HR_OPERATINGHEAD")
                        opHeadCode = _commonFunctionsService.GetParameterValue("HR_OPERATINGHEAD");
                    }
                    if (opHeadCode == loginEmpCode)
                    {
                        viewModel.ActiveTab = "OPH";
                        viewModel.IsOpReadOnly = false;
                    }
                    break;

                default:
                    // everything stays read-only, ActiveTab = "NULL"
                    break;
            }

            // Visibility based on employee's functional designation (ADFUNCTIONALDESIGNATIONID)
            var userFunctionDesigId = viewModel.FunctionalDesignationID;

            viewModel.ShowDeptTab = true;
            viewModel.ShowDivTab = true;
            viewModel.ShowOpTab = true;

            if (userFunctionDesigId == "2")
            {
                viewModel.ShowDeptTab = false;
            }
            else if (userFunctionDesigId == "3" || userFunctionDesigId == "4")
            {
                viewModel.ShowDeptTab = false;
                viewModel.ShowDivTab = false;
            }

            // Lock tabs if status already given (like Page_Load + SetTabPanelControls)
            if (!string.IsNullOrEmpty(viewModel.DeptApprovalStatus))
                viewModel.IsDeptReadOnly = true;

            if (!string.IsNullOrEmpty(viewModel.DivApprovalStatus))
                viewModel.IsDivReadOnly = true;

            if (!string.IsNullOrEmpty(viewModel.OpApprovalStatus))
                viewModel.IsOpReadOnly = true;

            // ======== TRAITS / ATTRIBUTES / RATINGS =================

            var confirmationId = string.IsNullOrEmpty(viewModel.ConfirmationFormID)
                ? "0"
                : viewModel.ConfirmationFormID;
            
            // ---------- Traits ----------
            DataTable traitsTable = _confirmationReviewService.GetTraitsList();

            // ---------- Ratings ----------
            DataTable ratingsTable = _confirmationReviewService
                .GetConfirmationFormRatings(confirmationId);

            foreach (DataRow traitRow in traitsTable.Rows)
            {
                var traitVM = new TraitViewModel
                {
                    TraitID = traitRow["HRTRAITID"].ToString(),
                    TraitName = traitRow["TRAIT"].ToString()
                };

                DataTable attributesTable = _confirmationReviewService
                    .GetTraitsAttributes(traitVM.TraitID, viewModel.ConfirmationFormID);

                foreach (DataRow attrRow in attributesTable.Rows)
                {
                    var ratingRow = ratingsTable.AsEnumerable()
                        .FirstOrDefault(r =>
                            r["HRTRAITSATTRIBUTESID"].ToString() ==
                            attrRow["HRTRAITSATTRIBUTESID"].ToString());

                    traitVM.Attributes.Add(new TraitAttributeViewModel
                    {
                        AttributeName = attrRow["ATTRIBUTEDESC"].ToString(),
                        TraitAttributeID = attrRow["HRTRAITSATTRIBUTESID"].ToString(),
                        RatingID = ratingRow?["HRTRAITSRATINGID"]?.ToString(),
                        Rating = ratingRow?["RATING"]?.ToString(),
                        Comment = ratingRow?["REMARKS"]?.ToString()
                    });
                }

                viewModel.Traits.Add(traitVM);
            }

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReviewFormDetails(ReviewFormDetailsViewModel model, string action)
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            // Determine active tab based on button pressed (SaveDept / SaveDiv / SaveOp)
            // This mirrors ViewState[vs_ActiveTab] in WebForms.
            string activeTab = model.ActiveTab;

            if (!string.IsNullOrEmpty(action))
            {
                switch (action)
                {
                    case "SaveDept":
                        activeTab = "DEPT";
                        break;
                    case "SaveDiv":
                        activeTab = "DIV";
                        break;
                    case "SaveOp":
                        activeTab = "OPH";
                        break;
                }
            }

            var detail = new FormDetails
            {
                ConfirmationID = string.IsNullOrEmpty(model.ConfirmationFormID)
                                        ? "0"
                                        : model.ConfirmationFormID,
                FunctionDesignation = model.FunctionalDesignationID ?? string.Empty,
                EmpCode = model.EmpCode,
                Extended = "1",
                FormFilledBy = userId,
                ApproverCode = userId,
                Active = "1",
                ReviewDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                ActiveTab = activeTab
            };

            // Map approval status + remarks based on active tab (like switch in SaveForm)
            switch (detail.ActiveTab)
            {
                case "DEPT":
                    detail.ApprovalStatus = model.DeptApprovalStatus;
                    detail.ApprovalRemarks = model.DeptRemarks?.Trim();
                    break;
                case "DIV":
                    detail.ApprovalStatus = model.DivApprovalStatus;
                    detail.ApprovalRemarks = model.DivRemarks?.Trim();
                    break;
                case "OPH":
                    detail.ApprovalStatus = model.OpApprovalStatus;
                    detail.ApprovalRemarks = model.OpRemarks?.Trim();
                    break;
                default:
                    detail.ApprovalStatus = null;
                    detail.ApprovalRemarks = null;
                    break;
            }

            // Build Rating list from nested Traits/Attributes (equivalent to rptTraits + rptTraitAttributes)
            var ratingList = new List<Ratings>();

            if (model.Traits != null)
            {
                foreach (var trait in model.Traits)
                {
                    if (trait.Attributes == null) continue;

                    foreach (var attr in trait.Attributes)
                    {
                        var rating = new Ratings
                        {
                            RatingID = attr.RatingID,           // hdn_RatingID.Value
                            TraitID = trait.TraitID,          // hdTraitID.Value
                            TraitAttributeID = attr.TraitAttributeID,  // hdn_TraitAttrib.Value
                            Rating = attr.Rating,            // ddlRating.SelectedValue
                            Remark = attr.Comment            // txtComment.Text
                        };
                        ratingList.Add(rating);
                    }
                }
            }

            detail.Rating = ratingList;

            // Call service & redirect like old code
            try
            {
                _confirmationReviewService.InsertConfirmationReviewTransaction(detail);
                // You can use TempData instead of JS alert
                TempData["Message"] = "Information Updated!";
            }
            catch (Exception)
            {
                TempData["Message"] = "Information updation failed!";
            }

            return RedirectToAction("PendingConfirmations"); // same as PendingConfirmations.aspx
        }





        // ===================== POST =====================
        /*[HttpPost]
        public IActionResult ReviewFormDetails(ReviewFormDetailsViewModel model)
        {
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            var employee = _sessionService.Get<Employee_Details>("Employee");

            FormDetails formDetails = new FormDetails
            {
                ConfirmationID = model.ConfirmationFormID,
                FunctionDesignation = employee?.Functional_Designation_Id,
                EmpCode = model.EmpCode,
                ApproverCode = userCode,
                FormFilledBy = userCode,
                Active = "1",
                Extended = "1",
                ActiveTab = model.ActiveTab,
                ReviewDate = DateTime.Now.ToString("dd-MMM-yyyy")
            };

            switch (model.ActiveTab)
            {
                case "DEPT":
                    formDetails.ApprovalStatus = model.DeptApprovalStatus;
                    formDetails.ApprovalRemarks = model.DeptRemarks;
                    break;
                case "DIV":
                    formDetails.ApprovalStatus = model.DivApprovalStatus;
                    formDetails.ApprovalRemarks = model.DivRemarks;
                    break;
                case "OPH":
                    formDetails.ApprovalStatus = model.OpApprovalStatus;
                    formDetails.ApprovalRemarks = model.OpRemarks;
                    break;
            }

            formDetails.Rating = model.Traits
                .SelectMany(t => t.Attributes.Select(a => new Ratings
                {
                    TraitID = t.TraitID,
                    TraitAttributeID = a.TraitAttributeID,
                    Rating = a.Rating,
                    Remark = a.Comment,
                    RatingID = a.RatingID
                })).ToList();

            try
            {
                _confirmationReviewService.InsertConfirmationReviewTransaction(formDetails);
                TempData["SuccessMessage"] = "Information Updated!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Information updation failed!";
            }

            return RedirectToAction("PendingConfirmations");
        }*/

        /*[HttpPost]
        public IActionResult ReviewFormDetails(ReviewFormDetailsViewModel model)
        {
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            var employee = _sessionService.Get<Employee_Details>("Employee");

            var formDetails = new FormDetails
            {
                ConfirmationID = model.EmployeeDetails.ConfirmationFormID ?? "0",
                FunctionDesignation = employee?.Functional_Designation_Id ?? "",
                EmpCode = model.EmployeeDetails.EmpCode,
                Extended = "1",
                FormFilledBy = userCode,
                ApproverCode = userCode,
                Active = "1",
                ReviewDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                ActiveTab = model.TabState.ActiveTab
            };

            // ✅ Approval Status & Remarks based on ActiveTab
            switch (formDetails.ActiveTab)
            {
                case "DEPT":
                    formDetails.ApprovalStatus = model.EmployeeDetails.DeptApprovalStatus;
                    formDetails.ApprovalRemarks = model.EmployeeDetails.DeptRemarks;
                    break;
                case "DIV":
                    formDetails.ApprovalStatus = model.EmployeeDetails.DivApprovalStatus;
                    formDetails.ApprovalRemarks = model.EmployeeDetails.DivRemarks;
                    break;
                case "OPH":
                    formDetails.ApprovalStatus = model.EmployeeDetails.OpApprovalStatus;
                    formDetails.ApprovalRemarks = model.EmployeeDetails.OpRemarks;
                    break;
            }

            // ✅ Map Ratings
            formDetails.Rating = model.Traits
                .SelectMany(trait => trait.Attributes.Select(attr => new Ratings
                {
                    RatingID = attr.RatingID,
                    TraitID = trait.TraitID,
                    TraitAttributeID = attr.TraitAttributeID,
                    Rating = attr.Rating,
                    Remark = attr.Comment
                }))
                .ToList();

            try
            {
                _confirmationReviewService.InsertConfirmationReviewTransaction(formDetails);
                TempData["SuccessMessage"] = "Information Updated!";
                return RedirectToAction("PendingConfirmations");
            }
            catch
            {
                TempData["ErrorMessage"] = "Information updation failed!";
                return RedirectToAction("PendingConfirmations");
            }
        }


        [HttpPost]
        public IActionResult ReviewFormDetails(ReviewFormDetailsViewModel model)
        {
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");


            // Get FunctionalDesignation from session (Employee object)
            var employee = _sessionService.Get<Employee_Details>("Employee");


            // Prepare FormDetails object for saving
            var formDetails = new FormDetails
            {
                ConfirmationID = model.ConfirmationFormID ?? "0",
                FunctionDesignation = employee?.Functional_Designation_Id ?? "",
                EmpCode = model.EmpCode,
                Extended = "1",
                FormFilledBy = userCode,
                ApproverCode = userCode,
                Active = "1",
                ReviewDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                ActiveTab = model.ActiveTab // Pass this from the view
            };

            // Determine Approval Status & Remarks based on ActiveTab
            switch (formDetails.ActiveTab)
            {
                case "DEPT":
                    formDetails.ApprovalStatus = model.DeptApprovalStatus;
                    formDetails.ApprovalRemarks = model.DeptRemarks;
                    break;
                case "DIV":
                    formDetails.ApprovalStatus = model.DivApprovalStatus;
                    formDetails.ApprovalRemarks = model.DivRemarks;
                    break;
                case "OPH":
                    formDetails.ApprovalStatus = model.OpApprovalStatus;
                    formDetails.ApprovalRemarks = model.OpRemarks;
                    break;
            }

            // Map Ratings from Traits
            formDetails.Rating = model.Traits
                .SelectMany(trait => trait.Attributes.Select(attr => new Ratings
                {
                    RatingID = attr.RatingID,
                    TraitID = trait.TraitID,
                    TraitAttributeID = attr.TraitAttributeID,
                    Rating = attr.Rating,
                    Remark = attr.Comment
                }))
                .ToList();

            try
            {
                _confirmationReviewService.InsertConfirmationReviewTransaction(formDetails);
                TempData["SuccessMessage"] = "Information Updated!";
                return RedirectToAction("PendingConfirmations");
            }
            catch
            {
                TempData["ErrorMessage"] = "Information updation failed!";
                return RedirectToAction("PendingConfirmations");
            }
        }*/




        [HttpGet]
        public IActionResult ReviewForm(string id = "0", string ecode = null)
        {
            // === Session check ===
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(ecode))
                return RedirectToAction("Login", "Account"); // same as old

            var loginEmp = _sessionService.Get<Employee_Details>("Employee");
            if (loginEmp == null)
                return RedirectToAction("Login", "Account");

            // 1) Get main data (equivalent to objConReview.GetEmpDetails(EmpCode))
            DataSet ds = _confirmationReviewService.GetEmpDetails(ecode);
            if (ds.Tables.Count < 2 || ds.Tables[0].Rows.Count == 0)
                return NotFound();

            var dtDetails = ds.Tables[0];
            var dtEmpSupDetails = ds.Tables[1];
            var dr = dtDetails.Rows[0];

            var vm = new ReviewFormViewModel();

            // ===== Basic fields =====
            vm.EmpCode = dr["ADEMPCODE"]?.ToString();
            vm.Name = dr["ENAME"]?.ToString();
            vm.Designation = dr["DESIGNATION"]?.ToString();
            vm.Department = dr["DEPTNAME"]?.ToString();
            vm.Division = dr["DIVNAME"]?.ToString();
            vm.Section = dr["SECTION"]?.ToString();
            vm.JoiningDate = dr["REGDATE"]?.ToString();
            vm.InductionStatus = dr["INDUCTION_PROG_STATUS"]?.ToString();
            vm.InductionDate = dr["TRAININGSTARTDATE"]?.ToString();
            vm.PreviousExtensionPeriod = dr["ExPeriod"]?.ToString();
            vm.ReviewDate = DateTime.Now.ToString("dd-MMM-yyyy");

            // ===== Reviewers / ViewState equivalents =====
            vm.Reviewer1Code = dr["REVIEWER1"]?.ToString();
            vm.Reviewer2Code = dr["REVIEWER2"]?.ToString();
            vm.Reviewer3Code = dr["REVIEWER3"]?.ToString();

            vm.Reviewer1Name = dr["REVIEWER1_NAME"]?.ToString();
            vm.Reviewer2Name = dr["REVIEWER2_NAME"]?.ToString();
            vm.Reviewer3Name = dr["REVIEWER3_NAME"]?.ToString();

            vm.Reviewer1Email = dr["REVIEWER1_EMAIL"]?.ToString();
            vm.Reviewer2Email = dr["REVIEWER2_EMAIL"]?.ToString();
            vm.Reviewer3Email = dr["REVIEWER3_EMAIL"]?.ToString();

            vm.FunctionalDesigId = dr["ADFUNCTIONALDESIGNATIONID"]?.ToString();
            vm.ConfirmationFormID = dr["ConfirmationFormID"]?.ToString() ?? "0";
            vm.Reviewer1Status = dr["DEPTAPPROVAL"]?.ToString(); // vs_Reviwer1Status

            // ===== Reviewer comments / statuses =====
            string reviewer1Comment = Convert.ToString(dr["DEPTREMARKS"]);
            string loginEmpCode = loginEmp.Employee_Code;

            // Old logic: reviewer1 drafting
            if (vm.Reviewer1Status == "0")
            {
                if (vm.Reviewer1Code == loginEmpCode)
                {
                    // Reviewer1 sees draft in own tab, but others see blank
                    vm.DeptRemarks = reviewer1Comment;
                    reviewer1Comment = "";
                }
                else
                {
                    reviewer1Comment = "";
                }
            }
            else
            {
                vm.DeptRemarks = reviewer1Comment;
            }

            vm.DeptApprovalStatus = dr["DEPTAPPROVAL"]?.ToString();
            vm.DivApprovalStatus = dr["DIVAPPROVAL"]?.ToString();
            vm.OpApprovalStatus = dr["OPHAPPROVAL"]?.ToString();
            vm.HRApprovalStatus = dr["HRAPPROVEDSTATUS"]?.ToString();

            vm.DivRemarks = dr["DIVREMARKS"]?.ToString();
            vm.OpRemarks = dr["OPHREMARKS"]?.ToString();

            vm.HRStatus = dr["HRAPPROVAL"]?.ToString();
            vm.HRRemarks = Convert.ToString(dr["HRREMARKS"])?.Replace("<br>", Environment.NewLine);

            // Division & Operating tabs "Reviewer 1" statuses
            vm.DVDeptStatus = dr["DeptStatus"]?.ToString();
            vm.DVDeptHeadComment = reviewer1Comment;

            vm.OPDeptHeadStatus = dr["DeptStatus"]?.ToString();
            vm.OPDeptHeadComment = reviewer1Comment;

            vm.OPDivHeadStatus = dr["DivStatus"]?.ToString();
            vm.OPDivHeadComment = dr["DIVREMARKS"]?.ToString();

            // EmpSupDetails (Dept/Div/Op heads)
            if (dtEmpSupDetails.Rows.Count > 0)
            {
                var row = dtEmpSupDetails.Rows[0];
                // if you still need them in MVC, you can put them into vm as well
                // string depHead = row["DEP_HEAD"].ToString();
                // etc...
            }

            // ===== Show / hide tabs (SetTab logic) =====
            vm.ShowDeptTab = !string.IsNullOrEmpty(vm.Reviewer1Code);
            vm.ShowDivTab = !string.IsNullOrEmpty(vm.Reviewer2Code);
            vm.ShowOpTab = !string.IsNullOrEmpty(vm.Reviewer3Code);

            // Who is logged in?
            bool isReviewer1 = vm.Reviewer1Code == loginEmpCode;
            bool isReviewer2 = vm.Reviewer2Code == loginEmpCode;
            bool isReviewer3 = vm.Reviewer3Code == loginEmpCode;

            vm.IsDeptEditable = isReviewer1;
            vm.IsDivEditable = isReviewer2;
            vm.IsOpEditable = isReviewer3;
            vm.IsTraitsEditable = isReviewer1 && vm.Reviewer1Status == "0"; // only reviewer1 can edit ratings

            // Active tab index (CR-2986 logic)
            if (isReviewer1 && vm.ShowDeptTab) vm.ActiveTab = "DEPT";
            else if (isReviewer2 && vm.ShowDivTab) vm.ActiveTab = "DIV";
            else if (isReviewer3 && vm.ShowOpTab) vm.ActiveTab = "OPH";
            else vm.ActiveTab = "DEPT"; // fallback

            // ===== Traits & Ratings =====
            DataTable traitsTable = _confirmationReviewService.GetTraitsList();
            DataTable ratingsTable = _confirmationReviewService.GetConfirmationFormRatings(vm.ConfirmationFormID);

            // flag for draft visibility
            bool reviewer1Pending = vm.Reviewer1Status == "0";

            foreach (DataRow traitRow in traitsTable.Rows)
            {
                var traitVm = new TraitViewModel2
                {
                    TraitID = traitRow["HRTRAITID"].ToString(),
                    TraitName = traitRow["TRAIT"].ToString()
                };

                DataTable attribTable = _confirmationReviewService.GetTraitsAttributes(traitVm.TraitID, vm.ConfirmationFormID);

                foreach (DataRow attrRow in attribTable.Rows)
                {
                    var ratingRow = ratingsTable.AsEnumerable()
                        .FirstOrDefault(r => r["HRTRAITSATTRIBUTESID"].ToString() ==
                                             attrRow["HRTRAITSATTRIBUTESID"].ToString());

                    var ratingId = ratingRow?["HRTRAITSRATINGID"]?.ToString() ?? "0";
                    var ratingVal = ratingRow?["RATING"]?.ToString() ?? "-1";
                    var comment = ratingRow?["REMARKS"]?.ToString() ?? string.Empty;

                    // Hide draft ratings/comments from non-Reviewer1 
                    if (!isReviewer1 && reviewer1Pending)
                    {
                        ratingId = "0";
                        ratingVal = "-1";
                        comment = string.Empty;
                    }

                    traitVm.Attributes.Add(new TraitAttributeViewModel2
                    {
                        TraitAttributeID = attrRow["HRTRAITSATTRIBUTESID"].ToString(),
                        AttributeName = attrRow["ATTRIBUTEDESC"].ToString(),
                        RatingID = ratingId,
                        Rating = ratingVal,
                        Comment = comment
                    });
                }

                vm.Traits.Add(traitVm);
            }

            // Optionally precompute totals on server side instead of JS:
            ComputeTotals(vm);

            return View(vm);   // View: ReviewForm.cshtml
        }

        private void ComputeTotals(ReviewFormViewModel vm)
        {
            const decimal TotalAttrib = 11m;
            const decimal TotalRating = TotalAttrib * 5m;

            int totalScore = 0;
            int countWithRating = 0;

            foreach (var t in vm.Traits)
            {
                foreach (var a in t.Attributes)
                {
                    if (!string.IsNullOrEmpty(a.Rating) && a.Rating != "-1")
                    {
                        totalScore += int.Parse(a.Rating);
                        countWithRating++;
                    }
                }
            }

            vm.TotalScore = totalScore;
            if (TotalRating > 0)
                vm.TotalPercentage = Math.Round((totalScore / TotalRating) * 100m, 2);

            if (TotalAttrib > 0)
                vm.AverageRating = Math.Round(totalScore / TotalAttrib, 2);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReviewForm(ReviewFormViewModel model, string command)
        {
            var loginEmp = _sessionService.Get<Employee_Details>("Employee");
            if (loginEmp == null)
                return RedirectToAction("Login", "Account");

            string loginCode = loginEmp.Employee_Code;

            switch (command)
            {
                case "SaveDept":
                    return SaveDept(model, loginEmp);
                case "SaveDiv":
                    return SaveDiv(model, loginEmp);
                case "SaveOp":
                    return SaveOp(model, loginEmp);
                case "SaveDraft":
                    return SaveDraft(model, loginEmp);
                default:
                    // Just reload
                    return RedirectToAction("ReviewForm",
                        new { id = model.ConfirmationFormID, ecode = model.EmpCode });
            }
        }

        private IActionResult SaveDept(ReviewFormViewModel model, Employee_Details loginEmp)
        {
            // Reviewer1 == login user?
            if (model.Reviewer1Code != loginEmp.Employee_Code)
            {
                // Unauthorized to save this tab, just reload
                return RedirectToAction("ReviewForm",
                    new { id = model.ConfirmationFormID, ecode = model.EmpCode });
            }

            // Validate: status must be selected
            if (string.IsNullOrEmpty(model.DeptApprovalStatus))
            {
                ModelState.AddModelError("", "Please select confirmation Status.");
                // reload data (GET) – you can either reconstruct from DB or reuse posted model
                ComputeTotals(model);
                return View(model);
            }

            // Build FormDetails like in WebForms
            var detail = new FormDetails
            {
                ConfirmationID = model.ConfirmationFormID,
                EmpCode = model.EmpCode,
                FormFilledBy = loginEmp.Employee_Code,
                ApproverCode = loginEmp.Employee_Code,
                ReviewDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                ActiveTab = "1", // Dept Head
                Active = "1",
                Extended = "1",
                FunctionDesignation = "1",
                ApprovalStatus = model.DeptApprovalStatus,
                ApprovalRemarks = model.DeptRemarks?.Trim(),
                Rating = new List<Ratings>()
            };

            foreach (var trait in model.Traits)
            {
                foreach (var attr in trait.Attributes)
                {
                    if (attr.Rating == "-1" || string.IsNullOrWhiteSpace(attr.Comment))
                    {
                        // Same behaviour: return silently or add ModelState error
                        // For now mimic: just stop and redisplay.
                        ModelState.AddModelError("", "All ratings and comments are mandatory.");
                        ComputeTotals(model);
                        return View(model);
                    }

                    detail.Rating.Add(new Ratings
                    {
                        RatingID = attr.RatingID,
                        TraitID = trait.TraitID,
                        TraitAttributeID = attr.TraitAttributeID,
                        Rating = attr.Rating,
                        Remark = attr.Comment
                    });
                }
            }

            try
            {
                _confirmationReviewService.InsertConfirmationReviewTransaction(detail);

                // email logic: if no Reviewer2 → SendMailToHR(), else SendMail to Reviewer2
                if (string.IsNullOrEmpty(model.Reviewer2Code))
                {
                    // _reviewService.SendMailToHR(...);  // wrap old SendMailToHR
                }
                else
                {
                    var mailTo = model.Reviewer2Email;
                    var mailToName = model.Reviewer2Name;
                    var approvedBy = loginEmp.Employee_Name;

                    if (!string.IsNullOrEmpty(mailTo))
                    {
                        //_confirmationReviewService.SendMailToReviewer2(mailTo, mailToName, approvedBy, model.Name);
                        SendMail(mailTo, mailToName, approvedBy, model.Name);
                    }
                }

                TempData["SuccessMessage"] = "Information Updated!";
                return RedirectToAction("PendingConfirmations");
            }
            catch
            {
                TempData["ErrorMessage"] = "Information updation failed!";
                return RedirectToAction("PendingConfirmations");
            }
        }

        private IActionResult SaveDiv(ReviewFormViewModel model, Employee_Details loginEmp)
        {
            if (model.Reviewer2Code != loginEmp.Employee_Code)
                return RedirectToAction("ReviewForm",
                    new { id = model.ConfirmationFormID, ecode = model.EmpCode });

            if (string.IsNullOrEmpty(model.DivApprovalStatus))
            {
                ModelState.AddModelError("", "Please select confirmation Status.");
                ComputeTotals(model);
                return View(model);
            }

            var detail = new FormDetails
            {
                ConfirmationID = model.ConfirmationFormID,
                EmpCode = model.EmpCode,
                FormFilledBy = loginEmp.Employee_Code,
                ApproverCode = loginEmp.Employee_Code,
                ReviewDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                ActiveTab = "2",
                Active = "1",
                Extended = "1",
                FunctionDesignation = "1",
                Rating = new List<Ratings>(),
                ApprovalStatus = model.DivApprovalStatus,
                ApprovalRemarks = model.DivRemarks?.Trim()
            };

            try
            {
                _confirmationReviewService.InsertConfirmationReviewTransaction(detail);

                if (string.IsNullOrEmpty(model.Reviewer3Code))
                {
                    // _reviewService.SendMailToHR();
                }
                else
                {
                    var mailTo = model.Reviewer3Email;
                    var mailToName = model.Reviewer3Name;
                    var approvedBy = loginEmp.Employee_Name;
                    if (!string.IsNullOrEmpty(mailTo))
                    {
                        //_confirmationReviewService.SendMailToReviewer3(mailTo, mailToName, approvedBy, model.Name);
                        SendMail(mailTo, mailToName, approvedBy, model.Name);
                    }
                }

                TempData["SuccessMessage"] = "Information Updated!";
                return RedirectToAction("PendingConfirmations");
            }
            catch
            {
                TempData["ErrorMessage"] = "Information updation failed!";
                return RedirectToAction("PendingConfirmations");
            }
        }

        private IActionResult SaveOp([FromBody] ReviewFormViewModel model, Employee_Details loginEmp)
        {
            if (model.Reviewer3Code != loginEmp.Employee_Code)
                return RedirectToAction("ReviewForm",
                    new { id = model.ConfirmationFormID, ecode = model.EmpCode });

            if (string.IsNullOrEmpty(model.OpApprovalStatus))
            {
                ModelState.AddModelError("", "Please select confirmation Status.");
                ComputeTotals(model);
                return View(model);
            }

            var detail = new FormDetails
            {
                ConfirmationID = model.ConfirmationFormID,
                EmpCode = model.EmpCode,
                FormFilledBy = loginEmp.Employee_Code,
                ApproverCode = loginEmp.Employee_Code,
                ReviewDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                ActiveTab = "3",
                Active = "1",
                Extended = "1",
                FunctionDesignation = "1",
                Rating = new List<Ratings>(),
                ApprovalStatus = model.OpApprovalStatus,
                ApprovalRemarks = model.OpRemarks?.Trim()
            };

            try
            {
                _confirmationReviewService.InsertConfirmationReviewTransaction(detail);
                TempData["SuccessMessage"] = "Information Updated!";
                return RedirectToAction("PendingConfirmations");
            }
            catch
            {
                TempData["ErrorMessage"] = "Information updation failed!";
                return RedirectToAction("PendingConfirmations");
            }
        }

        private IActionResult SaveDraft(ReviewFormViewModel model, Employee_Details loginEmp)
        {
            // Only reviewer1 can draft
            if (model.Reviewer1Code != loginEmp.Employee_Code)
                return RedirectToAction("ReviewForm",
                    new { id = model.ConfirmationFormID, ecode = model.EmpCode });

            var detail = new FormDetails
            {
                ConfirmationID = model.ConfirmationFormID,
                EmpCode = model.EmpCode,
                FormFilledBy = loginEmp.Employee_Code,
                ApproverCode = loginEmp.Employee_Code,
                ReviewDate = DateTime.Now.ToString("dd-MMM-yyyy"),
                ActiveTab = "10", // draft
                Active = "1",
                Extended = "1",
                FunctionDesignation = "1",
                ApprovalStatus = model.DeptApprovalStatus,
                ApprovalRemarks = model.DeptRemarks?.Trim(),
                Rating = new List<Ratings>()
            };

            foreach (var trait in model.Traits)
            {
                foreach (var attr in trait.Attributes)
                {
                    detail.Rating.Add(new Ratings
                    {
                        RatingID = attr.RatingID,
                        TraitID = trait.TraitID,
                        TraitAttributeID = attr.TraitAttributeID,
                        Rating = attr.Rating,
                        Remark = attr.Comment
                    });
                }
            }

            try
            {
                _confirmationReviewService.InsertConfirmationReviewTransaction(detail);
                TempData["SuccessMessage"] = "Information Updated!";
                return RedirectToAction("PendingConfirmations");
            }
            catch
            {
                TempData["ErrorMessage"] = "Information updation failed!";
                return RedirectToAction("PendingConfirmations");
            }
        }



        private void SendMail(string EMailID, string TO, string ApprovedBy, string Employee)
        {

            var sendMail = new commanEmail();

            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = EMailID;


            string strSubject = "Employee(s) confirmation of  " + Employee;
            string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=800 class=smalltext>" +
                        "<tr>" +
                            "<td colspan=2 width=100% height=35><img src=" + serverpath.getServerPath() + "..//..//assets//images//HondaLogo5.gif border=0 /></td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td colspan=2 height=3></td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "..//..//assets//images//Table_layout_04.gif height=40>" +
                                "<b>&nbsp;Pending Employee(s) Confirmation</b>" +
                            "</td>" +
                        "</tr>" +
                        "<tr height=150>" +
                            "<td colspan=2>" +
                                "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6>" +
                                    "<tr>" +
                                        "<td bgcolor=#bcddf6 width=6px>&nbsp;</td>" +
                                        "<td width=788 height=250 bgcolor=#FFFFFF valign=top>" +
                                            "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                                "<tr>" +
                                                    "<td colspan=3>&nbsp;</td>" +
                                                "</tr>" +
                                                "<tr>" +

                                                    "<td valign=top colspan=2>" +
                                                        "<p>" +
                                                        "<b>Dear " + TO + ",</b><br />" +
                                                         "<br />Greetings from Team HR.<br /><br />" +
                                                        "<br />" +
                                                             ApprovedBy + " san has reviewed the confirmation form for associate  " + Employee + "." +
                                                             "<br/>You are requested to review the same." +
                                                            "</p>" +
                                                             "<p>" +
                                                            "Please go to following link in eportal.  " +
                                                            "<b>Human Resource -> Employee Confirmation</b> </p><br />" +

                                                    "</td>" +
                                                "</tr>" +
                                                "<tr valign=bottom> " +
                                                    "<td colspan=2>" +
                                                        "<b>Thank You <br /> <br />Best Regards</b><br /> Team - HR<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong>" +
                                                    "</td>" +
                                                "</tr>" +
                                            "</table>" +
                                        "</td>" +
                                        "<td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                    "</tr>" +
                                "</table>" +
                            "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td colspan=2 width=100% bgcolor=#bcddf6 height=30>" +
                                "&nbsp;" +
                                "</td>" +
                         "</tr>" +
                    "</table> ";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }


        private void SendMailToHR(string empCode, string empName)
        {
            

            var objSendEmail = new commanEmail();

            //DataTable dt = new DataTable();
            string strSubject = string.Empty;
            string strBody = string.Empty;

            string parmaVal = _confirmationReviewService.GetHRMailDetails();
            string MailTo = parmaVal.Split('~')[0];
            string MailCC = parmaVal.Split('~')[1];

            string ToMailID = _commonFunctionsService.GetEMailID(MailTo);
            string CCMailID = _commonFunctionsService.GetEMailID(MailCC);

            objSendEmail.MailFrom = "portal.admin@honda.hmsi.in";

            if (!string.IsNullOrEmpty(ToMailID))
                objSendEmail.MailTo = ToMailID;
            else
                return;

            if (!string.IsNullOrEmpty(CCMailID))
                objSendEmail.MailCc = CCMailID;

            strSubject = " Employee Confirmation - " + empName + "[" + empCode + "]";
            strBody = "<table cellpadding=0 cellspacing=0 border=0 width=800 class=smalltext>" +
                     "<tr>" +
                         "<td colspan=2 width=100% height=35><img src=" + serverpath.getServerPath() + "..//..//assets//images//HondaLogo5.gif border=0 /></td>" +
                     "</tr>" +
                     "<tr>" +
                         "<td colspan=2 height=3></td>" +
                     "</tr>" +
                     "<tr>" +
                         "<td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "..//..//assets//images//Table_layout_04.gif height=40>" +
                             "<b>&nbsp;Pending Employee(s) Confirmation</b>" +
                         "</td>" +
                     "</tr>" +
                     "<tr height=150>" +
                         "<td colspan=2>" +
                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6>" +
                                 "<tr>" +
                                     "<td bgcolor=#bcddf6 width=6px>&nbsp;</td>" +
                                     "<td width=788 height=250 bgcolor=#FFFFFF valign=top>" +
                                         "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                             "<tr>" +
                                                 "<td colspan=3>&nbsp;</td>" +
                                             "</tr>" +
                                             "<tr>" +

                                                 "<td valign=top colspan=2>" +
                                                     "<p>" +
                                                     "<b>Dear San</b><br />" +
                                                      "<br />Greetings from Team HR.<br /><br />" +
                                                     "<br />" +
                                                         "The following employee(s) confirmation reviewing process has been completed." +
                                                         "</p>" +
                                                         "<p>" +
                                                              "<table width=\"90%\" style=\"border-left:1px solid #C1DAD7;\"  cellspacing=\"0\" cellpadding=\"2\" border=\"1\">" +
                                                                    "<tr><td style=\"background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\">S.No</td>" +
                                                                               "<td width=35% style=\"background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\">E-Code</td>" +
                                                                               "<td width=120 style=\"background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\">Name</td>" +
                                                                    "</tr>" +
                                                                    "<tr>" +
                                                                         "<td>1</td>" +
                                                                         "<td>" + empCode + "</td>" +
                                                                         "<td>" + empName + "</td>" +
                                                                    "</tr>" +

                                                             "</table>" +
                                                        "</p>" +
                                                         "<p>" +
                                                        "Please complete the process in SAP and E-Portal.  " +
                                                        "</p><br />" +

                                                "</td>" +
                                            "</tr>" +
                                            "<tr valign=bottom> " +
                                                "<td colspan=2>" +
                                                    "<b>Thank You <br /> <br />Best Regards</b><br /> Team - HR<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong>" +
                                                "</td>" +
                                            "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                    "<td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                "</tr>" +
                            "</table>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td colspan=2 width=100% bgcolor=#bcddf6 height=30>" +
                            "&nbsp;" +
                            "</td>" +
                     "</tr>" +
                "</table> ";

            objSendEmail.MailSubject = strSubject;
            objSendEmail.MailBody = strBody;
            try
            {
                bool blnEmailstatus = objSendEmail.Send();
            }
            catch (Exception ex)
            {
                //Response.Write("Exception Occured:   " + ex);
                _logger.LogError(ex, "Error Sending HR email");
            }
            finally
            {

            }
        }


        public string BuildPhotoUrl(string empCode)
        {
            var basePath = serverpath.getPhotoPath();

            if (string.IsNullOrEmpty(empCode))
                return basePath + "0264.jpg";

            if (empCode.Length == 2)
                empCode = "00" + empCode;
            else if (empCode.Length == 3)
                empCode = "0" + empCode;

            var physicalPath = Path.Combine(basePath, empCode + ".jpg");

            if (!System.IO.File.Exists(physicalPath))
                return basePath + "0264.jpg";

            return basePath + empCode + ".jpg";
        }


    }

}

