using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Utility
{
    public class UtilityRequestViewModel
    {

        // ===== Required in UI =====
        [Required(ErrorMessage = "Site is required.")]
        [Display(Name = "Site")]
        public string SiteId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Extension No. is required.")]
        [Display(Name = "Extension No.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Extension No. must be numeric.")]
        public string ExtensionNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact No. is required.")]
        [Display(Name = "Contact No.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        public string ContactNo { get; set; } = string.Empty;

        [Display(Name = "Direct No. (if any)")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        public string? DirectNo { get; set; }

        [Display(Name = "Line")]
        public string? Line { get; set; }

        [Display(Name = "Station")]
        public string? Station { get; set; }

        [Required(ErrorMessage = "Service Catalog is required.")]
        [Display(Name = "Service Catalog")]
        public string ServiceCatalogId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Classification is required.")]
        [Display(Name = "Classification")]
        public string ClassificationId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Budget Head is required.")]
        [Display(Name = "Budget Head")]
        public string BudgetHead { get; set; } = string.Empty;

        [Required(ErrorMessage = "Work Title is required.")]
        [Display(Name = "Work Title")]
        [StringLength(200, ErrorMessage = "Work Title cannot exceed 200 characters.")]
        public string WorkTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Scope of Work is required.")]
        [Display(Name = "Scope of Work")]
        [DataType(DataType.MultilineText)]
        public string ScopeOfWork { get; set; } = string.Empty;

        [Display(Name = "Attach File")]
        public IFormFile? Attachment { get; set; }

        [Display(Name = "Approval Authority")]
        public string? ApprovalAuthorityCode { get; set; }

        [Display(Name = "SectionManager")]
        public string? SectionManager { get; set; }

        [Display(Name = "Responsible Authority")]
        public string? ResponsibleAuthorityCode { get; set; }

        [Required(ErrorMessage = "You must agree to the terms of services.")]
        [Display(Name = "I agree to the Terms & Services")]
        public bool TermsAccepted { get; set; }

        // ===== Hidden / Derived / backend-support fields =====

        /// <summary>
        /// EmpCode (required by backend SubmitUtilityReq). 
        /// Typically set server-side from the signed-in user.
        /// </summary>
        public string EmpCode { get; set; } = string.Empty;

        /// <summary>
        /// Used when editing an existing request.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Name that will be stored in DB for the uploaded file (after saving).
        /// </summary>
        public string? SavedAttachmentName { get; set; }

        [Display(Name = "Contact No.:")]
        public string? PhoneNo { get; set;} = string.Empty;
        [Display(Name = "Direct No.:")]
        public string? Landline { get; set;} = string.Empty;
        [Display(Name = "Name:")]
        public string EmpName { get; set; } = string.Empty;

        public string? ServiceCatalogDescription { get; set; }
        public string? ClassificationDescription { get; set; }

        public string? NavigateUrl { get; set; }
        
        [MaxLength(2000)]
        public string? CancellationRemarks { get; set; }

        // Dropdown data
        public IEnumerable<SelectListItem> Sites { get; set; }
        public IEnumerable<SelectListItem> Locations { get; set; }
        public IEnumerable<SelectListItem> ServiceCatalogs { get; set; }
        public IEnumerable<SelectListItem> Classifications { get; set; } = new List<SelectListItem>() { new SelectListItem("-Select Classification-", "0") };
        public IEnumerable<SelectListItem> ApprovalAuthorities { get; set; }
        public string? LocationDescription { get; set; }
        public string? SectionManagerStatus { get; set; }
        public string? SectionManagerRemarks { get; set; }
        public string? DeptManagerStatus { get; set; }
        public string? DeptManagerRemarks { get; set; }
        public string? SubmitDate { get; set; }
        public string? Price { get; set; }
        public string? NoOfQuotes { get; set; }
        public string? Remarks { get; set; }
        public string? PoNo { get; set; }
        public string? PoDate { get; set; }
        public string? UtStatus { get; set; }
        public string? CloseRemarks { get; set; }
        public string? LogDate { get; set; }
        public string? SiteDescription { get; set; }
        public string? IsDeptApproved { get; set; }
        public string? IsSectApproved { get; set; }
        
        public string? UserMailId { get; set; }
        public string? DeptMgrMailId { get; set; }
        public string? DeptMgrEmpCode { get; set; }
        public string? DeptMgrName { get; set; }

        public UpdateUtilityRequestControls UpdateUTRControls { get; set; } = new UpdateUtilityRequestControls();
    }
    public class UtilityDocumentInfo
    {
        public string PhysicalPath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string? FileDownloadName { get; set; }
    }
    public class UpdateUtilityRequestControls
    {
        public bool SectionManagerStatusVisible { get; set; } = true;
        public bool TxtSctMgrRemarkVisblity { get; set; } = true;
        public bool Panel1Visiblity { get; set; } = true;
        public bool AppPanelVisiblity { get; set; } = true;
        public bool LblAppAuthVisiblity { get; set; } = true;
        public string LblAppAuthText { get; set; }
        public string LblSctMgrNameText { get; set; }
    }
}
