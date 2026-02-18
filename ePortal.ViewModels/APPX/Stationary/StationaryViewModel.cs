using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Stationary
{
    public class StationaryViewModel
    {

        [Display(Name = "Plant")]
        [Required(ErrorMessage = "Plant is required.")]
        public string PlantCode { get; set; }

        [Display(Name = "Mobile No.")]
        [Phone(ErrorMessage = "Enter a valid mobile number.")]
        [StringLength(20)]
        public string MobileNumber { get; set; }

        [Display(Name = "Ext No.")]
        [Required(ErrorMessage = "Ext No. is required.")]
        [StringLength(10)]
        public string ExtensionNumber { get; set; }

        [Display(Name = "Forwarded To (for Approval)")]
        [Required(ErrorMessage = "Approver is required.")]
        [StringLength(100)]
        public string ApproverId { get; set; }
        public string ApproverName { get; set; }

        // ----- Stationary Details -----
        [Display(Name = "Stationary Item")]
        [Required(ErrorMessage = "Stationary Item is required.")]
        public int StationeryItemId { get; set; }

        [Display(Name = "QTY")]
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10,000.")]
        public int Quantity { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(500)]
        public string Remarks { get; set; }

        // ----- Select list sources for dropdowns -----
        public IEnumerable<SelectListItem> Plants { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Approvers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> StationeryItems { get; set; } = new List<SelectListItem>();
        public int MaxQuantity { get; set; }
        public ExceptionInfo exceptionInfo { get; set; } = new ExceptionInfo();
        public List<StationaryItem> StationaryCart { get; set; } = new List<StationaryItem> { };
        public string? InitiatorName { get; set; }
        public string? InitiatedDate { get; set; }
        
        //Department Approver
        public string? ApproverEmail { get; set; }
        public string? ApproverRemarks { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? ApprovalStatusId { get; set; }
        public string? ApprovalDate { get; set; }

        //Admin Approver
        public string? AdminApprover { get; set; }
        public string? AdminApproverEmail { get; set; }
        public string? AdminApprovalStatus { get; set; }
        public string? AdminRemarks { get; set; }
        public string? AdminApprovalDate { get; set; }

        public string RequestId { get; set; }
    }
    public class ExceptionInfo
    {
        public string ErrorMessage { get; set; } = "";
        public bool ErrorPanelVisiblity { get; set; }
        public string ControlId { get; set; } = "";
    }
    public class StationaryItem
    {
        public string Item {  get; set; }
        public string ItemId { get; set; }
        public string ItemDescription { get; set; }
        public string Qty { get; set; }
        public string Remark { get; set; }
    }
}
