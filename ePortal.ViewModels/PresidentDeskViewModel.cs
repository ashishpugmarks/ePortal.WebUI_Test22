using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace ePortal.ViewModels
{
    public class PresidentDeskViewModel
    {
        public long[] PRESIDENTMSG_ID { get; set; }
        public short STATUS { get; set; }
        public List<PresidentDesk_MSTViewModel> PresidentDetail { get; set; }
        public List<PresidentDesk_TRNViewModel> President_TRNDTL { get; set; }
        public List<PresidentMsgAPP_TRNViewModel> PresidentMsgApp_TRNDTL { get; set; }

        public PresidentDeskViewModel()
        {
            PresidentDetail = new List<PresidentDesk_MSTViewModel>();
            President_TRNDTL = new List<PresidentDesk_TRNViewModel>();
            PresidentMsgApp_TRNDTL = new List<PresidentMsgAPP_TRNViewModel>();

        }

    }
    public class PresidentDeskSearchModel
    {
        public short Status { get; set; }
        public DateTime ApprovalDate { get; set; }
        public long PresidentId { get; set; }
        public long PresidentMsgId { get; set; }
        public long PresidentMsgAppId { get; set; }
        public string TransactionType { get; set; }
    }
    public class PresidentDesk_TRNViewModel
    {
        [DisplayName("#")]
        public long PRESIDENTMSG_ID { get; set; }
        [Required(ErrorMessage = "Please select President Name")]
        public long PRESIDENT_ID { get; set; }
        [Required]
        [DisplayName("Brief")]
        [StringLength(70, MinimumLength = 20, ErrorMessage = "Brief must be between 20 and 70 char")]
        public string BRIEF { get; set; }
        [Required]
        [DisplayName("Message")]
        [StringLength(2000, MinimumLength = 120, ErrorMessage = "Message must be between 120 and 2000 char")]
        public string MESSAGE { get; set; }
        [DisplayName("Attachment")]
        public byte[]? ATTACHMENT { get; set; }
        [Required]
        [DisplayName("Valid From")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public string VALID_FROM { get; set; }
        [Required]
        [DisplayName("Valid To")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public string VALID_TO { get; set; }
        [Required]
        [DisplayName("Status")]
        public short STATUS { get; set; }
        [DisplayName("Created By")]
        public string? CREATED_BY_NAME { get; set; }
        public long? CREATED_BY { get; set; }
        [DisplayName("Created Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime? CREATED_DATE { get; set; }
        [DisplayName("Modified By")]
        public string? MODIFIED_BY_NAME { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        [DisplayName("Modified Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        [DisplayName("Attachment Type")]
        public string? ATTACHMENT_CONTENTTYPE { get; set; }
        [DisplayName("Attachment")]
        public string? ATTACHMENT_NAME { get; set; }
        [DisplayName("President Name")]
        public string? PRESIDENT_NAME { get; set; }
        [Required]
        [DisplayName("Initiator Remarks")]
        [StringLength(200,ErrorMessage = "Initiator Remarks must be less then 200 char")]
        public string INITIATOR_REMARKS { get; set; }
    }
    public class PresidentDesk_MSTViewModel
    {
        [DisplayName("#")]
        public long PRESIDENT_ID { get; set; }
        [Required]
        [DisplayName("President Name")]
        public string PRESIDENT_NAME { get; set; }
        [DisplayName("President Photo")]
        public byte[]? PRESIDENT_PHOTO { get; set; }
        [Required]
        [DisplayName("President Title")]
        public string PRESIDENT_TITLE { get; set; }
        [Required]
        [DisplayName("Active From")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public string? ACTIVE_FROM { get; set; }
        [DisplayName("Active To")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public string? ACTIVE_TO { get; set; }
        [Required]
        [DisplayName("Status")]
        public short STATUS { get; set; }
        [Required]
        [DisplayName("Remarks")]
        public string REMARKS { get; set; }
        [DisplayName("Created By")]
        public string? CREATED_BY_NAME { get; set; }
        public long? CREATED_BY { get; set; }
        [DisplayName("Created Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime? CREATED_DATE { get; set; }
        [DisplayName("Modified By")]
        public string? MODIFIED_BY_NAME { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        [DisplayName("Modified Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        [DisplayName("Photo ContentType")]
        public string? PHOTO_CONTENTTYPE { get; set; }
        [DisplayName("Photo Name")]
        public string? PHOTO_NAME { get; set; }
        [Required(ErrorMessage = "Please select file.")]
        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed.")]
        public IFormFile? PostedFile { get; set; }
        public string? ValidationMessage { get; set; }

    }
    public class PresidentMsgAPP_TRNViewModel
    {
        [DisplayName("#")]
        public long? MESSAGEAPP_ID { get; set; }
        [Required]
        [DisplayName("President Title")]
        public string PRESIDENT_TITLE { get; set; }
        [DisplayName("President Name")]
        public string? PRESIDENT_NAME { get; set; }
        public long? PRESIDENTMSG_ID { get; set; }
        [Required]
        [DisplayName("Status")]
        public long STATUS { get; set; }

        [DisplayName("Initiator Remarks")]
        public string? INITIATOR_REMARKS { get; set; }
        public long? CREATED_BY { get; set; }
        [DisplayName("Initiated By")]
        public string? INITIATOR_NAME { get; set; }
        [DisplayName("Initiated Date")]
        [DisplayFormat(DataFormatString = "{0: dd-MMM-yyyy}")]
        public System.DateTime CREATED_DATE { get; set; }
        public Nullable<long> APPROVED_BY { get; set; }
        [DisplayName("Action Taken By")]
        public string? APPROVED_BY_NAME { get; set; }
        [DisplayName("Action Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> APPROVED_DATE { get; set; }
        [DisplayName("Remarks")]
        public string? APPAUTH_REMARKS { get; set; }
        [Required]
        [DisplayName("Brief")]
        public string BRIEF { get; set; }
        [Required]
        [DisplayName("Message")]
        public string MESSAGE { get; set; }
        [DisplayName("Attachment Type")]
        public string? ATTACHMENT_CONTENTTYPE { get; set; }
        [DisplayName("Attachment")]
        public string? ATTACHMENT_NAME { get; set; }

        //[DisplayName("Attachment")]
        //public byte[] ATTACHMENT { get; set; }

    }

}
