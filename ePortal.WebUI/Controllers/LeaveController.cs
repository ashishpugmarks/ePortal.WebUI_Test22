using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.Application.Contracts; // ILeaveApps
using ePortal.Shared.Interface;     // ISessionService
using ePortal.ViewModels.APPX.Leave;
using ePortal.Persistence.Admin.Interface;
using ePortal.WebUI.Filters;
using ePortal.Persistence;
using static ePortal.ViewModels.APPX.Leave.LeaveViewModel;
using ePortal.ViewModels;
using ePortal.Persistence.Services;
using ePortal.Persistence.Admin.Services;
using ePortal.Persistence.Interface;
using DocumentFormat.OpenXml.Spreadsheet;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class LeaveController : Controller
    {
        private readonly ILeaveApps _leaveService;
        private readonly ISessionService _session;
        private readonly ICommonFunctions _common;

        public LeaveController(ILeaveApps leaveService, ISessionService sessionService,ICommonFunctions commonFunctions)
        {
            _leaveService = leaveService;
            _session = sessionService;
            _common    = commonFunctions;
        }

        // === GET: Leave Card New (initial load) ===
        [HttpGet]
        public IActionResult Leave_Card_New()
        {
            var userId = _session.Get<string>("userID") ?? string.Empty;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account"); // adjust as per your routes

            var vm = new LeaveViewModel();

            // Year list (sorted desc & select current year like Web Forms)
            var dtYear = _leaveService.FromYearGet(); // FROMDATE column
            var dv = dtYear.DefaultView;
            dv.Sort = "FROMDATE desc";                // Web Forms sorts by FROMDATE desc 
            dtYear = dv.ToTable();
          

            vm.FromYears = dtYear.AsSelectList("FROMDATE", "FROMDATE", false);
            vm.Year = DateTime.Now.Year.ToString();

            // Get functional designation
            var fnDesig = GetFunctionalDesig(userId); // NORMAL fallback same as Web Forms 

            // Operation(s)
            var dtOps = _leaveService.GetOrgUnits(userId, "1"); // 1 - OPERATION 
            vm.Operations = dtOps.AsSelectList("ID", "ORGUNIT", true, defaultText: "-All Operation-", defaultValue: "0");

            // Per Web Forms: if NOT Operating Head, auto-select & disable Operation from Session["Employee"]
            if (!string.Equals(fnDesig, "OPERATING HEAD", StringComparison.OrdinalIgnoreCase))
            {
                var emp = _session.Get<Employee_Details>("Employee");
                if (emp != null)
                {
                    vm.OperationId = emp.Operation_Id?.ToString();
                    vm.OperationDisabled = true;       // Web Forms disables operation for non OH 
                }
            }
            else
            {
                vm.OperationId = dtOps.Rows.Count > 0 ? dtOps.Rows[0]["ID"]?.ToString() ?? "" : "";
            }

            // Division(s)
            var effectiveOps = vm.OperationId == "0" || string.IsNullOrEmpty(vm.OperationId)
                ? vm.Operations.ComposeAllIds()
                : vm.OperationId;

            var dtDivs = _leaveService.GetDivision(userId, fnDesig, effectiveOps); // 
            vm.Divisions = dtDivs.AsSelectList("DIVISIONID", "DIVISION", true, defaultText: "-All Division-", defaultValue: "0");

            // Auto-select/disable Division for specific designations (parity with Web Forms)
            if (string.Equals(fnDesig, "DIVISION HEAD", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fnDesig, "DEPARTMENT MANAGER", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fnDesig, "SECTION MANAGER", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fnDesig, "NORMAL", StringComparison.OrdinalIgnoreCase))
            {
                var emp = _session.Get<Employee_Details>("Employee");
                if (emp != null && !string.IsNullOrEmpty(emp.Division_Id?.ToString()))
                {
                    vm.DivisionId = emp.Division_Id?.ToString();
                    vm.DivisionDisabled = true;        // Web Forms disables division for those roles 
                }
            }

            // Department(s)
            var effectiveDivs = vm.DivisionId == "0" || string.IsNullOrEmpty(vm.DivisionId)
                ? vm.Divisions.ComposeAllIds()
                : vm.DivisionId;

            var dtDeps = _leaveService.GetDepartment(userId, fnDesig, effectiveOps, effectiveDivs); // 
            vm.Departments = dtDeps.AsSelectList("DEPARTMENTID", "DEPARTMENT", true, defaultText: "-All Department-", defaultValue: "0");

            if (string.Equals(fnDesig, "DEPARTMENT MANAGER", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fnDesig, "SECTION MANAGER", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fnDesig, "NORMAL", StringComparison.OrdinalIgnoreCase))
            {
                var emp = _session.Get<Employee_Details>("Employee");
                if (emp != null && !string.IsNullOrEmpty(emp.Department_Id?.ToString()))
                {
                    vm.DepartmentId = emp.Department_Id?.ToString();
                    vm.DepartmentDisabled = true;      // parity with Web Forms 
                }
            }

            // Section(s)
            var dtSecs = _leaveService.GetSection(
                effectiveOps,
                vm.DivisionId ?? "0",
                vm.DepartmentId ?? "0",
                userId,
                fnDesig); // method name mirrors Web Forms; add to ILeaveApps if missing 

            vm.Sections = dtSecs.AsSelectList("SECTIONID", "SECTION", true, defaultText: "-All Sections-", defaultValue: "0");

            if (string.Equals(fnDesig, "SECTION MANAGER", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fnDesig, "NORMAL", StringComparison.OrdinalIgnoreCase))
            {
                var emp = _session.Get<Employee_Details>("Employee");
                if (emp != null && !string.IsNullOrEmpty(emp.Section_Id?.ToString()))
                {
                    vm.SectionId = emp.Section_Id?.ToString();
                    vm.SectionDisabled = true;         // parity with Web Forms 
                }
            }

            // Employee(s)
            var dtEmps = _leaveService.GetEmployees(
                effectiveOps,
                vm.DivisionId ?? "0",
                vm.DepartmentId ?? "0",
                vm.SectionId ?? "0",
                userId,
                fnDesig); // 

            vm.Employees = dtEmps.AsSelectList("ECODE", "EMPLOYEE", true, defaultText: "-All Employees-", defaultValue: "0");

            if (string.Equals(fnDesig, "NORMAL", StringComparison.OrdinalIgnoreCase))
            {
                vm.EmployeeId = userId;
                vm.EmployeeDisabled = true;            // Web Forms auto-selects current user & disables 
            }

            if (dtEmps.Rows.Count > 1)
            {
                vm.gridall = false;
                vm.EmployeeDisabled = false;
                vm.SectionDisabled = false;
            }
            else
            {
                vm.gridall = true;
            }
            // Grid data
            vm.Grid = LoadGridRows(
                userId,
                fnDesig,
                effectiveOps,
                vm.DivisionId ?? "0",
                vm.DepartmentId ?? "0",
                vm.SectionId ?? "0",
                vm.EmployeeId ?? "0",
                vm.Employees,
                vm.Year ?? DateTime.Now.Year.ToString());

             vm.selfGrid = LoadGridRows(
                userId,
                fnDesig,
                "",
                "",
                "",
                "",
                userId,
                vm.Employees,
                vm.Year ?? DateTime.Now.Year.ToString());


            return View(vm);
        }

       
// GET: /Leave/LeaveCard?ed=2025&eid=<encrypted ECode>
        [HttpGet]
        public IActionResult LeaveCard(string ed, string eid)
        {
            // Decrypt employee code (WebForms did Server.UrlDecode + Encryption.Decrypt(Request["eid"])) 
                var ecode = ePortal.Shared.Encryption.Decrypt(eid); // replace with your real decryption

            var vm = new LeaveCardViewModel
            {
                Year           = string.IsNullOrWhiteSpace(ed) ? DateTime.Now.Year.ToString() : ed,
                ECode          = ecode,
                EncryptedECode = eid
            };

            // Load employee details as in Page_Load (Operation/Division/Department/Section/Name) 
            DataTable emp = _common.GetEmpDetails(ecode);
            if (emp.Rows.Count > 0)
            {
                vm.Operation  = emp.Rows[0]["OPERATION"]?.ToString() ?? "";
                vm.Division   = emp.Rows[0]["DIVISION"]?.ToString() ?? "";
                vm.Department = emp.Rows[0]["DEPARTMENT"]?.ToString() ?? "";
                vm.Section    = emp.Rows[0]["SECTION"]?.ToString() ?? "";
                vm.Employee   = emp.Rows[0]["ENAME"]?.ToString() ?? "";
            }

            // Leave balances (AUTH) – mirrors FillLeaveBalance in WebForms 
            var dsBal = _leaveService.EmpLeaveRecord(ecode, vm.Year, "AUTH");
            if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count >= 4)
            {
                // Indexes based on your code-behind: [balance]=col[2], [availed]=col[1] for SL/CL/EL/CO 
                vm.SL_Balance = dsBal.Tables[0].Rows[0][2]?.ToString() ?? "0";
                vm.SL_Availed = dsBal.Tables[0].Rows[0][1]?.ToString() ?? "0";

                vm.CL_Balance = dsBal.Tables[0].Rows[1][2]?.ToString() ?? "0";
                vm.CL_Availed = dsBal.Tables[0].Rows[1][1]?.ToString() ?? "0";

                vm.EL_Balance = dsBal.Tables[0].Rows[2][2]?.ToString() ?? "0";
                vm.EL_Availed = dsBal.Tables[0].Rows[2][1]?.ToString() ?? "0";

                vm.CO_Balance = dsBal.Tables[0].Rows[3][2]?.ToString() ?? "0";
                vm.CO_Availed = dsBal.Tables[0].Rows[3][1]?.ToString() ?? "0";
            }

            // Gender & maternity visibility (stored procedure in WebForms: PKG_LEAVEAPPLICATION.SPROC_GETGENDER) 
            vm.Gender        = _leaveService.getEmployeeGender(ecode);   // wrap your SP call
            vm.ShowMaternity = string.Equals(vm.Gender, "F", StringComparison.OrdinalIgnoreCase);

            // Initial render; record grids load via AJAX
            return View("LeaveCard", vm);
        }

        // AJAX: return partial grid for a given leave type (1..9) – based on SearchLeaveDetail loop in WebForms 
        [HttpGet]
        public PartialViewResult GetLeaveGridByType(string ecode, string year, int leaveType)
        {
            var ds = _leaveService.SearchLeaveDetail(ecode, leaveType.ToString(), year);

            var rows = new List<LeaveDetailRow>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    
                    rows.Add(new LeaveDetailRow
                    {
                        AppDate       = dr["DATEADDED"]?.ToString() ?? "",
                        LeaveFrom     = dr["DATEFROM"]?.ToString() ?? "",
                        LeaveTo       = dr["DATETO"]?.ToString() ?? "",
                        Days          = dr["NOOFDAYS"]?.ToString() ?? "",
                        Purpose       = dr["PURPOSE"]?.ToString() ?? "",
                        RecommendedBy = dr["SUPERVISORNAME"]?.ToString() ?? "",
                        SanctionedBy  = dr["APPROVALSUPERVISORNAME"]?.ToString() ?? "",
                        PlanPU        = dr["LEAVEPLAN"]?.ToString() ?? ""
                    });

                }
            }

            ViewBag.LeaveType = leaveType;
            return PartialView("_LeaveGridByType", rows);
        }

        [HttpGet]
        public JsonResult GetLeaveBalances(string ecode, string year)
        {
            var dsBal = _leaveService.EmpLeaveRecord(ecode, year, "AUTH");
            var result = new
            {
                SL_Availed = GetBalCell(dsBal, 0, 1), SL_Balance = GetBalCell(dsBal, 0, 2),
                CL_Availed = GetBalCell(dsBal, 1, 1), CL_Balance = GetBalCell(dsBal, 1, 2),
                EL_Availed = GetBalCell(dsBal, 2, 1), EL_Balance = GetBalCell(dsBal, 2, 2),
                CO_Availed = GetBalCell(dsBal, 3, 1), CO_Balance = GetBalCell(dsBal, 3, 2)
            };
            return Json(result);
        }

        // ===== Helpers =====
        private static string GetBalCell(DataSet ds, int row, int col)
        {
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count <= row) return "0";
            return ds.Tables[0].Rows[row][col]?.ToString() ?? "0";
        }


        // === AJAX: divisions for a given operation selection ===
        [HttpGet]
        public JsonResult GetDivisions(string operationIds)
        {
            var userId = _session.Get<string>("userID") ?? string.Empty;
            var desig = GetFunctionalDesig(userId);
            var dt = _leaveService.GetDivision(userId, desig, operationIds);
            var list = dt.AsSelectList("DIVISIONID", "DIVISION", true, defaultText: "-All Division-", defaultValue: "0");
            return Json(list);
        }

        // === AJAX: departments for given op + division selections ===
        [HttpGet]
        public JsonResult GetDepartments(string operationIds, string divisionIds)
        {
            var userId = _session.Get<string>("userID") ?? string.Empty;
            var desig = GetFunctionalDesig(userId);
            var dt = _leaveService.GetDepartment(userId, desig, operationIds, divisionIds);
            var list = dt.AsSelectList("DEPARTMENTID", "DEPARTMENT", true, defaultText: "-All Department-", defaultValue: "0");
            return Json(list);
        }

        // === AJAX: sections for op + division + department ===
        [HttpGet]
        public JsonResult GetSections(string operationIds, string divisionId, string departmentId)
        {
            var userId = _session.Get<string>("userID") ?? string.Empty;
            var desig = GetFunctionalDesig(userId);
            var dt = _leaveService.GetSection(operationIds, divisionId, departmentId, userId, desig);
            var list = dt.AsSelectList("SECTIONID", "SECTION", true, defaultText: "-All Sections-", defaultValue: "0");
            return Json(list);
        }

        // === AJAX: employees for current filter ===
        [HttpGet]
        public JsonResult GetEmployees(string operationIds, string divisionId, string departmentId, string sectionId)
        {
            var userId = _session.Get<string>("userID") ?? string.Empty;
            var desig = GetFunctionalDesig(userId);
            var dt = _leaveService.GetEmployees(operationIds, divisionId, departmentId, sectionId, userId, desig);
            var list = dt.AsSelectList("ECODE", "EMPLOYEE", true, defaultText: "-All Employees-", defaultValue: "0");
            return Json(list);
        }

        // === AJAX: grid data ===
        //[HttpGet]
        //public PartialViewResult GetLeaveGrid(string year, string operationIds, string divisionIds, string departmentIds, string sectionIds, string employeeIds)
        //{
        //    var userId = _session.Get<string>("userID") ?? string.Empty;
        //    var desig = GetFunctionalDesig(userId);
        //    ViewData["Year"] = year;
        //    // If "0" received, compute "all" from current lists (client can pass these already composed)
        //    var grid = LoadGridRows(userId, desig, operationIds, divisionIds, departmentIds, sectionIds, employeeIds, Enumerable.Empty<SelectListItem>(), year);
        //    return PartialView("_LeaveGrid", grid);
        //}
        
private static string DumpColumns(DataTable dt)
{
    if (dt == null) return "[null]";
    if (dt.Columns.Count == 0) return "[no columns]";
    return string.Join(",", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
}

[HttpGet]

[HttpGet]
public PartialViewResult GetLeaveGrid(
    string year,
    string operationIds,
    string divisionIds,
    string departmentIds,
    string sectionIds,
    string employeeIds)
{
    var userId = _session.Get<string>("userID") ?? string.Empty;
    var desig = GetFunctionalDesig(userId);
    ViewData["Year"] = year;

    // OPERATIONS
    if (string.IsNullOrEmpty(operationIds) || operationIds == "0")
    {
        var opsDt = _leaveService.GetOrgUnits(userId, "1"); // 1 - Operation
                                                            // Uncomment next line if you need to inspect columns in debug output:
                                                            // System.Diagnostics.Debug.WriteLine("GetOrgUnits columns: " + DumpColumns(opsDt));

                // Try common column names in order, first match wins
                if (((DataTable)opsDt).Rows.Count > 0)
                    operationIds =  opsDt.ComposeAllIds("ID") ?? opsDt.ComposeAllIds("OPERATIONID") ?? "0";
                else
                    operationIds = "0";

    }

    // DIVISIONS
    if (string.IsNullOrEmpty(divisionIds) || divisionIds == "0")
    {
        var divDt = _leaveService.GetDivision(userId, desig, operationIds);
        divisionIds =
              divDt.ComposeAllIds("DIVISIONID")
           ?? divDt.ComposeAllIds("DIV_ID")
           ?? "0";
    }

    // DEPARTMENTS
    if (string.IsNullOrEmpty(departmentIds) || departmentIds == "0")
    {
        var depDt = _leaveService.GetDepartment(userId, desig, operationIds, divisionIds);
        departmentIds =
              depDt.ComposeAllIds("DEPARTMENTID")
           ?? depDt.ComposeAllIds("DEPT_ID")
           ?? "0";
    }

    // SECTIONS
    if (string.IsNullOrEmpty(sectionIds) || sectionIds == "0")
    {
        var secDt = _leaveService.GetSection(operationIds, divisionIds, departmentIds, userId, desig);
        sectionIds =
              secDt.ComposeAllIds("SECTIONID")
           ?? secDt.ComposeAllIds("SEC_ID")
           ?? "0";
    }

   
    // EMPLOYEES (optional but symmetrical)
    if (string.IsNullOrEmpty(employeeIds) || employeeIds == "0")
    {
        var empDt = _leaveService.GetEmployees(operationIds, divisionIds, departmentIds, sectionIds, userId, desig);
        employeeIds =
              empDt.ComposeAllIds("ECODE")
           ?? string.Empty;
            }

       // dtEmps.AsSelectList("ECODE", "EMPLOYEE", true, defaultText: "-All Employees-", defaultValue: "0");
    var grid = LoadGridRows(
        userId,
        desig,
        operationIds,
        divisionIds,
        departmentIds,
        sectionIds,
        employeeIds,                    // currently not used internally, but harmless
        Enumerable.Empty<SelectListItem>(),
        year);

    return PartialView("_LeaveGrid", grid);
}
        
[HttpGet]
public PartialViewResult GetSelfGrid(string year)
{
    var userId = _session.Get<string>("userID") ?? string.Empty;
    var desig = GetFunctionalDesig(userId);

    // Load only the current user's grid (same logic you used when building vm.selfGrid on initial load)
    var rows = LoadGridRows(
        userId: userId,
        fnDesig: desig,
        operationIds: "",    // not needed for self
        divisionIds: "",     // not needed for self
        departmentIds: "",   // not needed for self
        sectionIds: "",      // not needed for self
        employeeId: userId,  // <-- key difference
        employeeList: Enumerable.Empty<SelectListItem>(),
        year: year
    );

    ViewData["Year"] = year;
    return PartialView("_LeaveGrid", rows);
}


        // ===== Helpers =====

        private string GetFunctionalDesig(string ecode)
        {
            var dt = _leaveService.GetFn_desig(ecode); // same as Web Forms source, NORMAL fallback 
            if (dt.Rows.Count > 0) return dt.Rows[0][1]?.ToString() ?? "NORMAL";
            return "NORMAL";
        }

        private List<EmployeeLeaveRow> LoadGridRows(
            string userId,
            string fnDesig,
            string operationIds,
            string divisionIds,
            string departmentIds,
            string sectionIds,
            string employeeId,
            IEnumerable<SelectListItem> employeeList,
            string year)
        {
            // Compose the employees list: if "0" or empty -> all in dropdown; otherwise the selected
            var employeesEffective = (string.IsNullOrEmpty(employeeId) || employeeId == "0")
                ? employeeList.ComposeAllIds()
                : employeeId;

            var dt = _leaveService.GetLeaveAvailBal(
                operationIds, divisionIds, departmentIds, sectionIds,
                userId, fnDesig, employeesEffective, year); // 

            var rows = new List<EmployeeLeaveRow>();
            foreach (DataRow r in dt.Rows)
            {
                rows.Add(new EmployeeLeaveRow
                {
                    ECode = r["ECODE"]?.ToString() ?? "",
                    EName = r["ENAME"]?.ToString() ?? "",
                    SL_Availed = TryDecimal(r["sl_av"]),
                    CL_Availed = TryDecimal(r["cl_av"]),
                    EL_Availed = TryDecimal(r["el_av"]),
                    COff_Availed = r["coff_av"]?.ToString(),
                    SL_Balance = TryDecimal(r["sl_b"]),
                    CL_Balance = TryDecimal(r["cl_b"]),
                    EL_Balance = TryDecimal(r["el_b"]),
                    COff_Balance = r["coff_b"]?.ToString(),
                });
            }
            return rows;

            static decimal? TryDecimal(object? o)
                => decimal.TryParse(o?.ToString(), out var d) ? d : (decimal?)null;
        }



        //Manage Leave Approval       


        [HttpGet]
        public IActionResult ManageLeaveAprAuth(string Ind, string id)
        {
            var userId = _session.Get<string>("userID") ?? string.Empty;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            // determine page index (Ind)
            int pageIndex = 0;
            if (!string.IsNullOrWhiteSpace(Ind) && int.TryParse(Ind, out var p))
                pageIndex = p;

            // Fetch both tables from your leave service (replace with your actual methods)
            DataTable dtJe = _leaveService.EmployeeDataGrid(userId);          // JE & above
            DataTable dtStaf = _leaveService.STAFF_EmployeeDataGrid(userId);    // Staff & Line Associate

            var vm = new ManageAprAuthViewModel
            {
                BackUrl = Url.Action("Home", "Home"),
                JeAndAbove = ToGridVm(dtJe, pageIndex, id),
                StaffAndLine = ToGridVm(dtStaf, pageIndex, id)
            };

            // Render the page that keeps your exact HTML design
            return View("ManageLeaveAprAuth", vm);
        }

        // Helper: convert DataTable to strongly-typed rows for our static headers
        private static ManageAprGridVm ToGridVm(DataTable dt, int pageIndex, string? highlightECode)
        {
            var grid = new ManageAprGridVm
            {
                PageIndex = pageIndex,
                HighlightECode = highlightECode
            };

            if (dt == null || dt.Rows.Count == 0) return grid;

            // We assume these column names exist; adjust to your actual schema if needed:
            // "ECODE", "ENAME", "DESIGNATION", "RECOMMEND_AUTH", "APPROVE_AUTH"
            foreach (DataRow r in dt.Rows)
            {
                grid.Rows.Add(new ManageAprRowVm
                {
                    ECode = r["ADEMPCODE"]?.ToString() ?? "",
                    EName = r["EMPNAME"]?.ToString() ?? "",
                    Designation = r["DESIGNATION"]?.ToString() ?? "",
                    RecommendAuth = r["SUPEMP"]?.ToString() ?? "",
                    ApproveAuth = r["SUPSUPEMP"]?.ToString() ?? "",
                });
            }

            return grid;
        }
        //Leave Apr Authority

        [HttpGet]
        public IActionResult LeaveAprAuth(string id, string Ind)
        {
            var userId = _session.Get<string>("userID") ?? string.Empty;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var vm = new LeaveAprAuthViewModel { Id = id, Ind = Ind };

            // Back link to list (parity with Web Forms hlBackT)
            vm.BackUrl = $@"/Leave/ManageLeaveAprAuth/?Ind={Ind}&Id={id}";

            // Load employee basic info via PKG_LEAVEAPPAUTH.SPROC_GETEMPDATA
            using (var rdr = _leaveService.GetEmployeeData(id))
            {
                string divId = "", deptId = "", vpId = "", modifiedBy = "";
                while (rdr.Read())
                {
                    vm.EmployeeCode = id;
                    vm.EmployeeName = rdr["empname"]?.ToString();
                    vm.Designation = rdr["DESIGNATION"]?.ToString();
                    divId = rdr["divid"]?.ToString() ?? "";
                    deptId = rdr["deptid"]?.ToString() ?? "";
                    vpId = rdr["vpid"]?.ToString() ?? "";
                    modifiedBy = rdr["modifiedby"]?.ToString() ?? "";
                }

                // Fill dropdowns using PKG_LEAVEAPPAUTH.SPROC_EMPRECOMMAUTH_GET
                var ds = _leaveService.GetEligibleAuthorities(divId, id, deptId, vpId);  //

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    vm.StatusMessage = "Employee Code does not exist";
                }
                else
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var name = dr["empname"]?.ToString() ?? "";
                        var code = dr["adempcode"]?.ToString() ?? "";

                        vm.RecommendationOptions.Add(new SelectListItem { Text = name, Value = code });
                        vm.ApprovalOptions.Add(new SelectListItem { Text = name, Value = code });
                    }

                    // Preselect current authorities via PKG_LEAVEAPPAUTH.SPROC_GETAPPAUTHORITY
                    using (var rdr2 = _leaveService.GetCurrentAuthorities(id))
                    {
                        string recom = "", approver = "";
                        while (rdr2.Read())
                        {
                            recom = rdr2["supervisorempcode"]?.ToString() ?? "";
                            approver = rdr2["supsupervisorempcode"]?.ToString() ?? "";
                        }
                        vm.SelectedRecommendationCode = recom;
                        vm.SelectedApprovalCode = approver;
                    }
                }

                // Disable logic (CheckAprAuth parity): compare modifiedBy with head IDs of current user
                var headsDt = _leaveService.GetEmployeeAuthCodes(userId);  // PKG_LEAVEAPPAUTH.SPROC_AUTHCODES_GET
                string deptHead = "", divHead = "", opHead = "";
                if (headsDt.Rows.Count > 0)
                {
                    deptHead = headsDt.Rows[0]["DEPARTMENTHEADID"]?.ToString() ?? "";
                    divHead = headsDt.Rows[0]["DIVISIONHEADID"]?.ToString() ?? "";
                    opHead = headsDt.Rows[0]["VPHEADID"]?.ToString() ?? "";
                }

                // If modifiedBy equals a head and user is not that head -> disable (same as Web Forms)
                bool shouldDisable =
                    (!string.IsNullOrEmpty(modifiedBy)) &&
                    ((modifiedBy == opHead && userId != opHead) ||
                     (modifiedBy == divHead && userId != divHead) ||
                     (modifiedBy == deptHead && userId != deptHead));

                vm.SubmitEnabled = !shouldDisable;
                vm.RecomEnabled = !shouldDisable;
                vm.ApproveEnabled = !shouldDisable;
            }

            return View("LeaveAprAuth", vm);
        }

        // === POST: Save authorities ===
        [HttpPost]
        public IActionResult LeaveAprAuth(LeaveAprAuthViewModel vm)
        {
            try
            {
                string UserID;
                string loggedinUserId = _session.Get<string>("userID");
                string strcode1 = vm.EmployeeCode;
                UserID = vm.EmployeeCode;
                string responseMessage = string.Empty;
                if (UserID == strcode1)
                {

                    DataSet ds = _common.GetGunctionDesID(UserID);
                    string Temp = ds.Tables[0].Rows[0][0].ToString();
                    if (Temp == "1" || Temp == "2" || Temp == "3" || Temp == "5" || ds.Tables[0].Rows[0][0].ToString() == "" || ds.Tables[0].Rows[0][0].ToString() == null)
                    {
                        string strRecomAuth = string.Empty;
                        string strApputh = string.Empty;

                        strApputh = vm.SelectedApprovalCode;
                        strRecomAuth = vm.SelectedRecommendationCode;

                        if (strRecomAuth == " -Select Recommendation Authority -")
                        {
                            responseMessage += "Select Recommendation Authority";
                        }
                        else if (strApputh == "-Select Approval Authority -")
                        {
                            responseMessage += "Select Approval Authority";
                        }
                        else if (Temp == "1" || Temp == "2" || Temp == "3" || Temp == "5" || ds.Tables[0].Rows[0][0].ToString() == "" || ds.Tables[0].Rows[0][0].ToString() == null)
                        {
                            String cboRecomAuthority1 = vm.SelectedRecommendationCode;
                            String cboAppAuthority1 = vm.SelectedApprovalCode;

                            if (cboRecomAuthority1.Contains(strcode1) || cboAppAuthority1.Contains(strcode1))
                            {
                                responseMessage += "Error!! Approval authority cannot be set to himself.";
                            }
                            else
                            {
                                strApputh = vm.SelectedApprovalCode;
                                strRecomAuth = vm.SelectedRecommendationCode;

                                _leaveService.SetAuthorities(UserID, strRecomAuth, strApputh, loggedinUserId);
                                _leaveService.LogAuthorityChange(UserID, strRecomAuth, strApputh, loggedinUserId);
                                responseMessage = string.Empty;
                            }
                        }
                        if (string.IsNullOrEmpty(responseMessage))
                        {
                            var result = new
                            {
                                status = 1,
                                message = "Request Updated Successfully",
                                exception = ""
                            };
                            return Ok(result);
                        }
                        else
                        {
                            var result = new
                            {
                                status = 0,
                                message = responseMessage,
                                exception = ""
                            };
                            return Ok(result);
                        }
                    }
                    else
                    {
                        string strRecomAuth = string.Empty;
                        string strApputh = string.Empty;

                        strApputh = vm.SelectedApprovalCode;
                        strRecomAuth = vm.SelectedRecommendationCode;

                        if (strRecomAuth == " -Select Recommendation Authority -")
                        {
                            responseMessage += "Select Recommendation Authority";
                        }
                        else if (strApputh == "-Select Approval Authority -")
                        {
                            responseMessage += "Select Approval Authority";
                        }
                        else
                        {
                            _leaveService.SetAuthorities(UserID, strRecomAuth, strApputh, loggedinUserId);
                            _leaveService.LogAuthorityChange(UserID, strRecomAuth, strApputh, loggedinUserId);
                            responseMessage = string.Empty;
                        }
                        if (string.IsNullOrEmpty(responseMessage))
                        {
                            var result = new
                            {
                                status = 1,
                                message = "Request Updated Successfully",
                                exception = ""
                            };
                            return Ok(result);
                        }
                        else
                        {
                            var result = new
                            {
                                status = 0,
                                message = responseMessage,
                                exception = ""
                            };
                            return Ok(result);
                        }
                    }
                }
                else
                {
                    var result = new
                    {
                        status = 0,
                        message = "Logged in user can not be approval authority can not be the same",
                        exception = ""
                    };
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                var result = new
                {
                    status = -1,
                    message = "Something went wrong",
                    exception = ex.Message
                };
                return BadRequest(result);
            }
        }

        // === Cancel: keep Ind & Id ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LeaveAprAuthCancel(string id, string Ind)
        {
            return RedirectToAction("ManageLeaveAprAuth", "Leave", new { Ind = Ind, Id = id });  //
        }



    }

}

