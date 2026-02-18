using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.MasterMgmt
{
    public class MaterialApprovalUserUpdateModel
    {
        public string MMHeaderIdApprove { get; set; } = "0";

        public string MMHeaderDetailId { get; set; } = string.Empty;

        // Status
        public string? Status { get; set; } 

        [Display(Name = "Status (raw)")]
        public string StatusRaw { get; set; } = string.Empty;

        // Requester info
        public string RequesterId { get; set; } = string.Empty;

        public string RequestorName { get; set; } = string.Empty;

        public string? RequestDateText { get; set; } // raw string from DB, if needed

        public string? RequestDate { get; set; }   // parsed as DateTime if possible

         public string? PPCApprovingAuthority { get; set; }

        public string? FinanceApprovingAuthority { get; set; }
        public List<UserOptionDto> PPCUsers { get; set; } = new();
        public List<UserOptionDto> FinanceUsers { get; set; } = new();        
        public bool EnablePPCUser { get; set; }
        public bool EnableFinanceUser { get; set; }        
        public bool HasData { get; set; }

    }
    public enum ApprovalStatus
    {
        Unknown = 0,
        PendingAtFIN,
        PendingAtPPC,
        Other // e.g., Approved/Rejected/Forwarded/etc.
    }
    public class UserOptionDto
    {        
        public string? Text { get; set; } = string.Empty;
               
        public string? Value { get; set; } = string.Empty;
              
        public bool? Selected { get; set; }
    }

}
