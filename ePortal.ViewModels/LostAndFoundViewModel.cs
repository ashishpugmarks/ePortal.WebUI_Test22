using System;
using System.ComponentModel.DataAnnotations;
using ePortal.DomainClasses.Enums;
using Microsoft.AspNetCore.Http;

namespace ePortal.ViewModels
{
    public class LostAndFoundViewModel
    {
        public long Id { get; set; }
        public LostAndFoundItemType ItemType { get; set; }

        public string? ProductName { get; set; }

        public string? MarkDescription { get; set; }


        public string? MaterialType { get; set; }

        public string? location { get; set; }
        public string? AdditionalDetails { get; set; }


        public long LocationFound { get; set; } = 0;

        public List<SelectListItmesVm>? LocationOptions { get; set; }


        public DateTime DateFound { get; set; } = DateTime.Now;

        public List<IFormFile>? ItemPhoto { get; set; }

        public string? ItemPhotoPath { get; set; }

        public int Status { get; set; } = 1;

        public string? ReportedBy { get; set; }

        public DateTime ReportedDate { get; set; }

        public string? ClaimedBy { get; set; }
        public string? ClaimedByEmpCode { get; set; }
        public string? ClaimedByName { get; set; }

        public DateTime? DateClaimed { get; set; }

        public string? VerificationDetails { get; set; }

        public bool IsActive { get; set; }


        public string? CreatedBy { get; set; }

        public string? CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }


        public string? ModifiedBy { get; set; }

        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public short IsResubmit { get; set; }
        public string StatusText => ePortal.DomainClasses.LostAndFoundStatus.GetStatusText(Status);
        public string StatusClass => ePortal.DomainClasses.LostAndFoundStatus.GetStatusClass(Status);

        public List<LostAndFoundHistoryViewModel>? LostAndFoundHistory { get; set; }

    }
    public class LostAndFoundHistoryViewModel
    {
        public long ID { get; set; }

        public long LOST_AND_FOUND_ID { get; set; }

        public string? LAF_STATUS { get; set; }  // Lost and Found Status

        public string? REMARKS { get; set; }

        public long? ADEMPCODE { get; set; }   // Long type

        public DateTime CHANGEDDATE { get; set; } = DateTime.Now;

        public long ADDEDBY { get; set; }

        public DateTime ADDEDDATE { get; set; }

        public string UPDATEBY { get; set; }

        public DateTime? UPDATEDATE { get; set; }
    }
    public class SelectListItmesVm
    {
        public string Text { get; set; }
        public long Value { get; set; }
    }
    public class LostAndFoundExportModel
    {
        public string ItemType { get; set; }
        public long Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

}
