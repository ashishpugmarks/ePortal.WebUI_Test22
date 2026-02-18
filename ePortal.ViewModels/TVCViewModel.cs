using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web;
using Microsoft.AspNetCore.Http;


namespace ePortal.ViewModels
{
    public partial class TVCMasterViewModel
    {
        [DisplayName("ID")]
        public long ATTACHMENTID { get; set; }

        [Required]
        [DisplayName("Content Process")]
        public long PROCESSID { get; set; }

        [Required]
        [DisplayName("Subject")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Subject must be between 5 and 20 char")]
        public string SUBJECT { get; set; }

        [DisplayName("Brief")]
        [StringLength(50, MinimumLength = 15, ErrorMessage = "Brief must be between 15 and 50 char")]
        public string BRIEF { get; set; }

        //[Required]
        //[DisplayName("Description")]
        //[StringLength(2000, MinimumLength = 120, ErrorMessage = "Description must be between 120 and 2000 char")]
        public string DESCRIPTION { get; set; }

        [DisplayName("Banner")]
        public byte[] BANNER { get; set; }
        [DisplayName("Banner Content")]
        public string BANNER_CONTENTTYPE { get; set; }
        [DisplayName("Banner Name")]
        public string BANNER_NAME { get; set; }
        [DisplayName("Attachment")]
        public byte[] ATTACHMENT1 { get; set; }
        [DisplayName("Attachment Content")]
        public string ATTACHMENT1_CONTENTTYPE { get; set; }
        [DisplayName("Attachment Name ")]
        public string ATTACHMENT1_NAME { get; set; }
        [DisplayName("Attachment 2")]
        public byte[] ATTACHMENT2 { get; set; }
        [DisplayName("Attachment 2 Content")]
        public string ATTACHMENT2_CONTENTTYPE { get; set; }
        [DisplayName("Attachment 2 Name ")]
        public string ATTACHMENT2_NAME { get; set; }
        [Required]
        [DisplayName("Active-From")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public string START_DATE { get; set; }
        [Required]
        [DisplayName("Active-To")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public string END_DATE { get; set; }

        [DisplayName("Status")]
        public Int16 STATUS { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Created Date")]
        public System.DateTime CREATED_DATE { get; set; }
        public string CreateDate { get; set; }

        [DisplayName("Created By")]
        public long CREATED_BY { get; set; }

        [DisplayName("Created By")]
        public string User_Name { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<long> FUNCTIONAL_DESIGNATION { get; set; }
        public Nullable<long> DESIGNATION { get; set; }
        public Nullable<long> SITE { get; set; }
        public long INITIATED_BY { get; set; }
        [DisplayName("Remarks")]
        public string INITIATOR_REMARKS { get; set; }

        public IFormFile BannerFile { get; set; }

        public IFormFile Attachment1File { get; set; }

        public IFormFile Attachment2File { get; set; }
        public virtual ProcessMstViewModel CM_PROCESS_MST { get; set; }

        //public virtual ADFUNCTIONALDESIGNATION ADFUNCTIONALDESIGNATION { get; set; }
        //public virtual ADDESIGNATION ADDESIGNATION { get; set; }
        //public virtual SYSITE SYSITE { get; set; }
        public List<TVCMasterViewModel> TVC { get; set; }
        public virtual SearchViewModel SearchViewModel { get; set; }
    }

    public partial class TVCApprovalViewModel
    {
        [DisplayName("ID")]
        public long ATTACHMENTAPPID { get; set; }
        public long ATTACHMENTID { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        [DisplayName("Subject")]
        public string SUBJECT { get; set; }

        [DisplayName("Brief")]
        public string BRIEF { get; set; }
        [DisplayName("Description")]    
        public string DESCRIPTION { get; set; }
        public String INITIATED_User { get; set; }

        [DisplayName("Initiated By")]
        public long INITIATED_BY { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Initiated Date")]
        public DateTime INITIATED_DATE { get; set; }

        [DisplayName("Initiator Remarks")]
        public string INITIATOR_REMARKS { get; set; }

        [Required]
        [DisplayName("Approval Authority")]
        public Nullable<long> APPAUTH1_ECODE { get; set; }
        public String APPAUTH1_User { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Approved Date")]
        public Nullable<System.DateTime> APPAUTH1_DATE { get; set; }

        [Required]
        [DisplayName("Remarks")]
        public string APPAUTH1_REMARKS { get; set; }
        [DisplayName("Banner")]
        public byte[] BANNER { get; set; }
        [DisplayName("Banner Content")]
        public string BANNER_CONTENTTYPE { get; set; }
        [DisplayName("Banner Name")]
        public string BANNER_NAME { get; set; }
        [DisplayName("Attachment 1")]
        public byte[] ATTACHMENT1 { get; set; }
        [DisplayName("Attachment 1 Content")]
        public string ATTACHMENT1_CONTENTTYPE { get; set; }
        [DisplayName("Attachment 1 Name ")]
        public string ATTACHMENT1_NAME { get; set; }
        [DisplayName("Attachment 2")]
        public byte[] ATTACHMENT2 { get; set; }
        [DisplayName("Attachment 2 Content")]
        public string ATTACHMENT2_CONTENTTYPE { get; set; }
        [DisplayName("Attachment 2 Name ")]
        public string ATTACHMENT2_NAME { get; set; }
        public virtual TVCMasterViewModel CM_PROCESSATTACHMENT_TRN { get; set; }
    }

}
