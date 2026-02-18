using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace ePortal.ViewModels
{
    public partial class AnnouncementMasterViewModel
    {
        [DisplayName("ID")]
        public long ATTACHMENTID { get; set; }

        [Required]
        [DisplayName("Content Process")]
        public long PROCESSID { get; set; }
        public string PROCESS_NAME { get; set; }

        [Required]
        [DisplayName("Subject")]
        [StringLength(100, MinimumLength = 30, ErrorMessage = "Subject must be between 30 and 100 char")]
        public string SUBJECT { get; set; }

        [DisplayName("Brief")]
        [StringLength(200, MinimumLength = 100, ErrorMessage = "Brief must be between 100 and 200 char")]
        public string BRIEF { get; set; }

        [Required]
        [DisplayName("Description")]
        [StringLength(2000, MinimumLength = 100, ErrorMessage = "Description must be between 100 and 2000 char")]
        public string DESCRIPTION { get; set; }

        [Required]
        [DisplayName("Remarks")]
        [StringLength(2000, ErrorMessage = "Remarks max length 2000 char")]
        public string REMARKS { get; set; }

        [DisplayName("Banner")]
        public byte[] BANNER { get; set; }
        [DisplayName("Banner Type")]
        public string BANNER_CONTENTTYPE { get; set; }

        [DisplayName("Banner Name")]
        public string BANNER_NAME { get; set; }

        [DisplayName("Attachment 1")]
        public byte[] ATTACHMENT1 { get; set; }
        [DisplayName("Attachment 1")]
        public string ATTACHMENT1_CONTENTTYPE { get; set; }
        [DisplayName("Attachment 1 Name ")]
        public string ATTACHMENT1_NAME { get; set; }

        [DisplayName("Attachment 2")]
        public byte[] ATTACHMENT2 { get; set; }
        [DisplayName("Attachment 2 ")]
        public string ATTACHMENT2_CONTENTTYPE { get; set; }
        [DisplayName("Attachment 2 Name ")]
        public string ATTACHMENT2_NAME { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Active From")]
        public String START_DATE { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Active To")]
        public String END_DATE { get; set; }

        [DisplayName("Status")]
        public Int16 STATUS { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Created Date")]
        public System.DateTime CREATED_DATE { get; set; }

        [DisplayName("Created By")]
        public long CREATED_BY { get; set; }

        [DisplayName("Created By")]
        public string User_Name { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<long>[] FUNCTIONAL_DESIGNATION { get; set; }
        public Nullable<long>[] DESIGNATION { get; set; }
        public Nullable<long>[] SITE { get; set; }
        [DisplayName("Fn-Designation")]
        public string FnDesignationDescrip { get; set; }
        [DisplayName("Designation")]
        public string DesignationDescrip { get; set; }
        [DisplayName("Location")]
        public string SiteDescrip { get; set; }

        //[Required]
        public IFormFile BannerFile { get; set; }

        //[Required]
        public IFormFile AttachmentFile1 { get; set; }

        //[Required]
        public IFormFile AttachmentFile2 { get; set; }

        public virtual ProcessMstViewModel CM_PROCESS_MST { get; set; }
        //public virtual ADFUNCTIONALDESIGNATION ADFUNCTIONALDESIGNATION { get; set; }
        //public virtual ADDESIGNATION ADDESIGNATION { get; set; }
        //public virtual SYSITE SYSITE { get; set; }
        public List<AnnouncementMasterViewModel> Announcements { get; set; }
        public virtual AnnouncementApprovalViewModel AnnouncementApp { get; set; }
        public virtual SearchViewModel SearchViewModel { get; set; }
        public string UniqueCirculareID { get; set; }

        //public string EmailID{ get; set; }
        public bool SendEmail { get; set; }

        [DisplayName("Circular No")]
        public string CIRCULARENO { get; set; }

        [DisplayName("Process Name")]
        public string ProcessName { get; set; }
    }
    public partial class AnnouncementApprovalViewModel
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
        public String INITIATED_User { get; set; }

        [DisplayName("Initiated By")]
        public long INITIATED_BY { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Initiated Date")]
        public DateTime INITIATED_DATE { get; set; }

        [DisplayName("Initiator Remarks")]
        public String INITIATOR_REMARKS { get; set; }

        [Required]
        [DisplayName("Approval Authority 1")]
        public Nullable<long> APPAUTH1_ECODE { get; set; }
        public String APPAUTH1_User { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Approved Date")]
        public Nullable<System.DateTime> APPAUTH1_DATE { get; set; }

        [Required]
        [DisplayName("Remarks")]
        public String APPAUTH1_REMARKS { get; set; }

        [Required]
        [DisplayName("Approval Authority 2")]
        public Nullable<long> APPAUTH2_ECODE { get; set; }
        public String APPAUTH2_User { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Approved Date")]
        public Nullable<System.DateTime> APPAUTH2_DATE { get; set; }

        [Required]
        [DisplayName("Remarks")]
        public string APPAUTH2_REMARKS { get; set; }
        public String HRAPP_User { get; set; }

        [Required]
        [DisplayName("Approved By")]
        public Nullable<long> HRAPP_ECODE { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Approved Date")]
        public Nullable<System.DateTime> HRAPP_DATE { get; set; }

        [Required]
        [DisplayName("HR Remarks")]
        public string HRAPP_REMARKS { get; set; }

        public virtual AnnouncementMasterViewModel AnnouncementTrn { get; set; }

        //public virtual ADLOGINUSER ADLOGINUSER { get; set; }
        //public virtual ADLOGINUSER ADLOGINUSER1 { get; set; }
        //public virtual ADLOGINUSER ADLOGINUSER2 { get; set; }
        //public virtual ADLOGINUSER ADLOGINUSER3 { get; set; }
        //public virtual AnnouncementMasterViewModel CM_PROCESSATTACHMENT_TRN { get; set; }
    }


    public partial class DeleteFileRequest
    {
        public long ATTACHMENTID { get; set; }
        public string ATTACHMENT_TYPE { get; set; }
    }

}
