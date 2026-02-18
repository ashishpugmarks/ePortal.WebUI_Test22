using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.EmployeeConfirmation
{
    public class ConfirmationRowViewModel
    {
        public string HRConfirmationReviewId { get; set; }
        public string ADEmpCode { get; set; }
        public string EName { get; set; }
        public string ConfirmationDate { get; set; }

        public string Reviewer1Status { get; set; }
        public string Reviewer2Status { get; set; }
        public string Reviewer3Status { get; set; }
        public string HRStatus { get; set; }

        public string DeptStatus { get; set; }
        public string DivStatus { get; set; }
        public string OpStatus { get; set; }
        public string HRApprovalStatus { get; set; }
        public string HRRemarks { get; set; }

        public string Reviewer1Link { get; set; }
        public string Reviewer2Link { get; set; }
        public string Reviewer3Link { get; set; }
        public string HRLink { get; set; }

        public string Reviewer1ImageUrl { get; set; }
        public string Reviewer2ImageUrl { get; set; }
        public string Reviewer3ImageUrl { get; set; }
        public string HRImageUrl { get; set; }

        public string PhotoUrl { get; set; }
        public string OpName { get; set; }
        public string DivName { get; set; }
        public string DeptName { get; set; }
        public string SecName { get; set; }

        public string ProcessStatus { get; set; }

        public bool IsHoldByHR { get; set; }
        public string HoldTooltip { get; set; }

    }


    public class ConfirmationHistoryViewModel
    {
        public string HRConfirmationReviewId { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string ConfirmationDate { get; set; }
        public string DeptStatus { get; set; }
        public string DivStatus { get; set; }
        public string OpStatus { get; set; }
        public string HRApprovalStatus { get; set; }
        public string DeptName { get; set; }
        public string DivName { get; set; }
        public string SecName { get; set; }
        public string OpName { get; set; }


        public string PhotoUrl { get; set; } // For tooltip image
        public string TooltipText { get; set; } // For mouseover tooltip

        // Optional: Add more fields if needed from the DataTable
    }



    //ReviewFormDetails
    /*public class ReviewFormDetailsViewModel
    {
        public string ConfirmationFormIDa { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string JoiningDate { get; set; }
        public string InductionStatus { get; set; }
        public string InductionDate { get; set; }
        public string PreviousExtensionPeriod { get; set; }
        public string ReviewDate { get; set; }

        public string DeptRemarks { get; set; }
        public string DivRemarks { get; set; }
        public string OpRemarks { get; set; }

        public string DeptApprovalStatus { get; set; }
        public string DivApprovalStatus { get; set; }
        public string OpApprovalStatus { get; set; }

        public string HRApprovalStatus { get; set; }
        public string HRRemarks { get; set; }
        public string FunctionalDesignationID { get; set; }
        public string ActiveTab { get; set; }

        public bool IsDeptTabEnabled { get; set; }
        public bool IsDivTabEnabled { get; set; }
        public bool IsOpTabEnabled { get; set; }

        public List<TraitViewModel> Traits { get; set; } = new List<TraitViewModel>();
    }

    public class TraitViewModel
    {
        public string TraitID { get; set; }
        public string TraitName { get; set; }
        public List<TraitAttributeViewModel> Attributes { get; set; } = new List<TraitAttributeViewModel>();
    }

    public class TraitAttributeViewModel
    {
        public string TraitAttributeID { get; set; }
        public string RatingID { get; set; }
        public string Rating { get; set; }
        public string Comment { get; set; }
    }*/


    /*public class ReviewFormDetailsViewModel
    {
        // Core identity fields
        public string ConfirmationID { get; set; }   // maps from ConfirmationFormID
        public string FunctionDesignation { get; set; } // ADFUNCTIONALDESIGNATIONID
        public string EmpCode { get; set; }

        // Page display fields (labels)
        public string Name { get; set; }
        public string Designation { get; set; }
        public string ECode { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string JoiningDate { get; set; }
        public string InductionStatus { get; set; }
        public string InductionDate { get; set; }
        public string PreviousExtensionPeriod { get; set; }
        public string ReviewDate { get; set; }

        // Comments / radio
        public string DeptApprovalStatus { get; set; }
        public string DivApprovalStatus { get; set; }
        public string OpApprovalStatus { get; set; }
        public string HRApprovalStatus { get; set; }

        public string HRStatus { get; set; }
        public string HRRemarks { get; set; }

        // Tab states and which tab is active: "DEPT","DIV","OPH","NULL"
        public string ActiveTab { get; set; }

        // Traits and nested attributes (for rendering)
        public List<TraitViewModel> Traits { get; set; } = new List<TraitViewModel>();

        // Flattened list for model binding on post (one entry per attribute)
        public List<RatingViewModel> RatingList { get; set; } = new List<RatingViewModel>();

        // UI helper properties for which tab can be edited (set by controller)
        public bool CanEditDept { get; set; }
        public bool CanEditDiv { get; set; }
        public bool CanEditOp { get; set; }
    }


    public class TraitViewModel
    {
        public string TraitID { get; set; }
        public string TraitName { get; set; }
        public List<TraitAttributeViewModel> Attributes { get; set; } = new List<TraitAttributeViewModel>();
    }


    public class TraitAttributeViewModel
    {
        public string TraitAttributeID { get; set; }
        public string AttributeName { get; set; }
        public string Rating { get; set; }
        public string RatingID { get; set; }
        public string Remark { get; set; }
    }


    public class RatingViewModel
    {
        public string RatingID { get; set; }          // HRTRAITSRATINGID or DB id
        public string TraitID { get; set; }
        public string TraitAttributeID { get; set; }
        public string Rating { get; set; }            // selected rating value
        public string Remark { get; set; }
        public string AttributeName { get; set; }     // friendly name, optional
    }*/

    public class ReviewFormDetailsViewModel
    {
        public string ConfirmationFormID { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string JoiningDate { get; set; }
        public string InductionStatus { get; set; }
        public string InductionDate { get; set; }
        public string PreviousExtensionPeriod { get; set; }
        public string ReviewDate { get; set; }

        public string DeptRemarks { get; set; }
        public string DivRemarks { get; set; }
        public string OpRemarks { get; set; }

        public string DeptApprovalStatus { get; set; }
        public string DivApprovalStatus { get; set; }
        public string OpApprovalStatus { get; set; }
        public string HRApprovalStatus { get; set; }

        public string HRStatus { get; set; }
        public string HRRemarks { get; set; }
        public string FunctionalDesignationID { get; set; }

        public string ActiveTab { get; set; }

        public bool ShowDeptTab { get; set; } = true;
        public bool ShowDivTab { get; set; } = true;
        public bool ShowOpTab { get; set; } = true;

        public bool IsDeptReadOnly { get; set; } = true;
        public bool IsDivReadOnly { get; set; } = true;
        public bool IsOpReadOnly { get; set; } = true;

        //public bool IsDeptTabEnabled { get; set; }
        //public bool IsDivTabEnabled { get; set; }
        //public bool IsOpTabEnabled { get; set; }

        public string DVDeptStatus { get; set; }
        public string OPDivHeadStatus { get; set; }

        public List<TraitViewModel> Traits { get; set; } = new();
    }

    public class TraitViewModel
    {
        public string TraitID { get; set; }
        public string TraitName { get; set; }
        public List<TraitAttributeViewModel> Attributes { get; set; } = new();
    }

    public class TraitAttributeViewModel
    {
        public string AttributeName { get; set; }
        public string TraitAttributeID { get; set; }
        public string RatingID { get; set; }
        public string Rating { get; set; }
        public string Comment { get; set; }
    }



    //*****************//ReviewForm********************

    public class ReviewFormViewModel
    {
        // === Hidden / technical ===
        public string ConfirmationFormID { get; set; }         // vs_ConfirmationFormID
        public string FunctionalDesigId { get; set; }          // vs_FunctionalDesig
        public string ActiveTab { get; set; }                  // vs_ActiveTab / "1","2","3","10"
        public string Reviewer1Code { get; set; }              // vs_Reviewer1
        public string Reviewer2Code { get; set; }              // vs_Reviewer2
        public string Reviewer3Code { get; set; }              // vs_Reviewer3
        public string Reviewer1Status { get; set; }            // vs_Reviwer1Status (DEPTAPPROVAL)

        public string Reviewer1Name { get; set; }
        public string Reviewer2Name { get; set; }
        public string Reviewer3Name { get; set; }

        public string Reviewer1Email { get; set; }
        public string Reviewer2Email { get; set; }
        public string Reviewer3Email { get; set; }

        // For validation messages
        public string ErrorMessage { get; set; }

        // === Associate Details ===
        public string Name { get; set; }
        public string EmpCode { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string JoiningDate { get; set; }         // or DateTime?
        public string InductionStatus { get; set; }
        public string InductionDate { get; set; }
        public string PreviousExtensionPeriod { get; set; }
        public string ReviewDate { get; set; }

        // === Reviewer comments / statuses ===
        // Reviewer 1 (Department)
        public string DeptApprovalStatus { get; set; }  // rbtDepartmentStatus
        public string DeptRemarks { get; set; }         // txtDeptComment

        // Reviewer 2 (Division)
        public string DVDeptStatus { get; set; }        // lblDVDeptStatus
        public string DVDeptHeadComment { get; set; }   // lblDVDeptHeadComment
        public string DivApprovalStatus { get; set; }   // rbtDivisionStatus
        public string DivRemarks { get; set; }          // txtDivComment

        // Reviewer 3 (Operating Head)
        public string OPDeptHeadStatus { get; set; }    // lblOPDeptHeadStatus
        public string OPDeptHeadComment { get; set; }   // lblOPDeptHeadComment
        public string OPDivHeadStatus { get; set; }     // lblOPDivHeadStatus
        public string OPDivHeadComment { get; set; }    // lblOPDivHeadComment
        public string OpApprovalStatus { get; set; }    // rbtOperatingStatus
        public string OpRemarks { get; set; }           // txtOPComment

        // HR
        public string HRApprovalStatus { get; set; }    // rbtHRApprovedStatus
        public string HRStatus { get; set; }            // ddlHRStatus
        public string HRRemarks { get; set; }           // txtHRRemarks

        // === Trait ratings ===
        public List<TraitViewModel2> Traits { get; set; } = new();

        // === Tab visibility & read-only (SetTab / SetTabPanelControls) ===
        public bool ShowDeptTab { get; set; }
        public bool ShowDivTab { get; set; }
        public bool ShowOpTab { get; set; }

        public bool IsDeptEditable { get; set; }
        public bool IsDivEditable { get; set; }
        public bool IsOpEditable { get; set; }
        public bool IsTraitsEditable { get; set; }  // reviewer1 logic

        // === Totals (lblTotalScore, lblTotalPer, lblAverageRating) ===
        public int TotalScore { get; set; }
        public decimal TotalPercentage { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class TraitAttributeViewModel2
    {
        public string TraitAttributeID { get; set; }
        public string RatingID { get; set; }           // HRTRAITSRATINGID
        public string Rating { get; set; }             // "1".."5" or "-1"
        public string Comment { get; set; }
        public string AttributeName { get; set; }      // ATTRIBUTEDESC
    }

    public class TraitViewModel2
    {
        public string TraitID { get; set; }            // HRTRAITID
        public string TraitName { get; set; }          // TRAIT
        public List<TraitAttributeViewModel2> Attributes { get; set; } = new();
    }


}
