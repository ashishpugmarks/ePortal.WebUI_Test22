using Microsoft.AspNetCore.Mvc.Rendering;
namespace ePortal.ViewModels.APPX.Leave
{
    public class LeaveViewModel
    {
        public bool gridall;

        // Selected values
        public string? Year { get; set; }
        public string? OperationId { get; set; }
        public string? DivisionId { get; set; }
        public string? DepartmentId { get; set; }
        public string? SectionId { get; set; }
        public string? EmployeeId { get; set; }

        // Lists
        public IEnumerable<SelectListItem> FromYears { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Operations { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Divisions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Departments { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Sections { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Employees { get; set; } = Enumerable.Empty<SelectListItem>();

        // Disable flags (to mimic the code-behind designations behavior)
        public bool OperationDisabled { get; set; }
        public bool DivisionDisabled { get; set; }
        public bool DepartmentDisabled { get; set; }
        public bool SectionDisabled { get; set; }
        public bool EmployeeDisabled { get; set; }

        // Grid rows
        public List<EmployeeLeaveRow> Grid { get; set; } = new();   
         public List<EmployeeLeaveRow> selfGrid { get; set; } = new();   
       
    }
     public class EmployeeLeaveRow
{
    public string ECode { get; set; } = "";
    public string EName { get; set; } = "";
    // Availed
    public decimal? SL_Availed { get; set; }
    public decimal? CL_Availed { get; set; }
    public decimal? EL_Availed { get; set; }
    public string? COff_Availed { get; set; } // days/hrs string per legacy
    // Balance
    public decimal? SL_Balance { get; set; }
    public decimal? CL_Balance { get; set; }
    public decimal? EL_Balance { get; set; }
    public string? COff_Balance { get; set; } // days/hrs string per legacy
}

}

