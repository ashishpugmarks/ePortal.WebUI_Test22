
// File: ViewModels/APPX/Leave/ManageAprAuthViewModel.cs
using System.Collections.Generic;

namespace ePortal.ViewModels.APPX.Leave
{
    public class ManageAprAuthViewModel
    {
        public ManageAprGridVm JeAndAbove { get; set; } = new();
        public ManageAprGridVm StaffAndLine { get; set; } = new();
        public string? BackUrl { get; set; }  // for the back link
          public string? BackUrlBack { get; set; }  // for the back link
    }

    public class ManageAprGridVm
    {
        // Table headers (same as your static markup)
        public List<string> Headers { get; set; } = new()
        {
            "Emp Code","Name","Designation","Recommendation Auth.","Approval Auth.","Edit"
        };

        // Rows
        public List<ManageAprRowVm> Rows { get; set; } = new();

        // Paging index (Ind)
        public int PageIndex { get; set; } = 0;

        // If you want to highlight a row (LightYellow), set this ECode
        public string? HighlightECode { get; set; }
    }

    public class ManageAprRowVm
    {
        public string ECode { get; set; } = "";
        public string EName { get; set; } = "";
        public string Designation { get; set; } = "";
        public string RecommendAuth { get; set; } = "";
        public string ApproveAuth { get; set; } = "";
    }
}
