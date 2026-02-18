using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web;
using Microsoft.AspNetCore.Http;


namespace ePortal.ViewModels
{
    public partial class CreativeMasterViewModel
    {
        [DisplayName("ID")]
        public long ATTACHMENTID { get; set; }

        [Required]
        [DisplayName("Content Process")]
        public long PROCESSID { get; set; }

        [Required]
        [DisplayName("Subject")]
        [StringLength(120, MinimumLength = 0, ErrorMessage = "Subject must be between 0 and 120 char")]
        public string? SUBJECT { get; set; }
        [Required]
        [DisplayName("Brief")]
        [StringLength(200, MinimumLength = 0, ErrorMessage = "Brief must be between 0 and 200 char")]
        public string? BRIEF { get; set; }
        //[Required]
        [DisplayName("Description")]
        [StringLength(2000, MinimumLength = 0, ErrorMessage = "Description must be between 0 and 2000 char")]
        public string? DESCRIPTION { get; set; }

        [DisplayName("Banner")]
        public byte[]? BANNER { get; set; }
        [DisplayName("Banner Content")]
        public string? BANNER_CONTENTTYPE { get; set; }
        [DisplayName("Banner Name")]
        public string? BANNER_NAME { get; set; }
        [DisplayName("Attachment 1")]
        public byte[]? ATTACHMENT1 { get; set; }
        [DisplayName("Attachment 1 Content")]
        public string? ATTACHMENT1_CONTENTTYPE { get; set; }
        [DisplayName("Attachment 1 Name ")]
        public string? ATTACHMENT1_NAME { get; set; }
        [DisplayName("Attachment 2")]
        public byte[]? ATTACHMENT2 { get; set; }
        [DisplayName("Attachment 2 Content")]
        public string? ATTACHMENT2_CONTENTTYPE { get; set; }
        [DisplayName("Attachment 2 Name ")]
        public string? ATTACHMENT2_NAME { get; set; }
        [Required]
        [DisplayName("Active-From")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public string? START_DATE { get; set; }
        [Required]
        [DisplayName("Active-To")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public string?    END_DATE { get; set; }

        [DisplayName("Status")]
        public Int16? STATUS { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Created Date")]
        public System.DateTime?   CREATED_DATE { get; set; }        
        public string? CreateDate    { get; set; }

        [DisplayName("Created By")]
        public long CREATED_BY { get; set; }

        [DisplayName("Created By")]
        public string? User_Name { get; set; }
        public long? MODIFIED_BY { get; set; }
        public System.DateTime? MODIFIED_DATE { get; set; }
        public long? FUNCTIONAL_DESIGNATION { get; set; }
        public long? DESIGNATION { get; set; }
        public long? SITE { get; set; }
        public long INITIATED_BY { get; set; }
        [Required]
        [DisplayName("Remarks")]
        public string? INITIATOR_REMARKS { get; set; }

        public IFormFile? BannerFile { get; set; }

        public IFormFile? Attachment1File { get; set; }

        public IFormFile? Attachment2File { get; set; }
        public virtual ProcessMstViewModel? CM_PROCESS_MST { get; set; }

        public List<CreativeMasterViewModel>? CorporateNews { get; set; }

        //public virtual ADFUNCTIONALDESIGNATION ADFUNCTIONALDESIGNATION { get; set; }
        //public virtual ADDESIGNATION ADDESIGNATION { get; set; }
        //public virtual SYSITE SYSITE { get; set; }
        public List<CM_Processattachmentappmapping_TrnViewModel>? CM_Processattachmentappmapping_Trn { get; set; }

        
        public virtual SearchViewModel? SearchViewModel { get; set; }

        [Required]
        [DisplayName("Approval Authority 1")]
        public long? APPAUTH1_ECODE { get; set; }
        public String? APPAUTH1_User { get; set; }
        public string? AppAuth1_status { get; set; }


        [Required]
        [DisplayName("Approval Authority 2")]
        public long? APPAUTH2_ECODE { get; set; }
        public String? APPAUTH2_User { get; set; }
        public string? AppAuth2_status { get; set; }
    }

    public class CM_Processattachmentappmapping_TrnViewModel
    {
        [DisplayName("Attachment ID")]
        public long ATTACHMENTID { get; set; }

        [DisplayName("Status")]
        public short? STATUS { get; set; }

        [DisplayName("App Auth E-Code")]
        public long? APPAUTH_ECODE { get; set; }

        public string APPAUTH_NAME { get; set; }

        [DisplayName("App Auth Date")]
        public System.DateTime? APPAUTH_DATE { get; set; }

        [DisplayName("App Auth Remarks")]
        public string APPAUTH_REMARKS { get; set; }

        [DisplayName("Created By")]
        public long? CREATED_BY { get; set; }

        [DisplayName("Created Date")]
        public System.DateTime? CREATED_DATE { get; set; }

        [DisplayName("Modified By")]
        public long? MODIFIED_BY { get; set; }

        [DisplayName("Modified Date")]
        public System.DateTime? MODIFIED_DATE { get; set; }

        [DisplayName("Mapping Attachment ID")]
        public long MAPPINGATTACHMENTID { get; set; }

        [DisplayName("App Level")]

        public long? APP_LEVEL { get; set; }
    }

    //public partial class CorporateNewsApprovalViewModel
    //{
    //    [DisplayName("ID")]
    //    public long ATTACHMENTAPPID { get; set; }
    //    public long ATTACHMENTID { get; set; }

    //    [DisplayName("Status")]
    //    public short STATUS { get; set; }
    //    [DisplayName("Subject")]
    //    public string SUBJECT { get; set; }

    //    [DisplayName("Brief")]
    //    public string BRIEF { get; set; }
    //    [DisplayName("Description")]    
    //    public string DESCRIPTION { get; set; }
    //    public String INITIATED_User { get; set; }

    //    [DisplayName("Initiated By")]
    //    public long INITIATED_BY { get; set; }

    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
    //    [DisplayName("Initiated Date")]
    //    public DateTime INITIATED_DATE { get; set; }

    //    [DisplayName("Initiator Remarks")]
    //    public string INITIATOR_REMARKS { get; set; }

    //    [Required]
    //    [DisplayName("Approval Authority")]
    //    public long> APPAUTH1_ECODE { get; set; }
    //    public String APPAUTH1_User { get; set; }

    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
    //    [DisplayName("Approved Date")]
    //    public System.DateTime> APPAUTH1_DATE { get; set; }

    //    [Required]
    //    [DisplayName("Remarks")]
    //    public string APPAUTH1_REMARKS { get; set; }
    //    [DisplayName("Banner")]
    //    public byte[] BANNER { get; set; }
    //    [DisplayName("Banner Content")]
    //    public string BANNER_CONTENTTYPE { get; set; }
    //    [DisplayName("Banner Name")]
    //    public string BANNER_NAME { get; set; }
    //    [DisplayName("Attachment 1")]
    //    public byte[] ATTACHMENT1 { get; set; }
    //    [DisplayName("Attachment 1 Content")]
    //    public string ATTACHMENT1_CONTENTTYPE { get; set; }
    //    [DisplayName("Attachment 1 Name ")]
    //    public string ATTACHMENT1_NAME { get; set; }
    //    [DisplayName("Attachment 2")]
    //    public byte[] ATTACHMENT2 { get; set; }
    //    [DisplayName("Attachment 2 Content")]
    //    public string ATTACHMENT2_CONTENTTYPE { get; set; }
    //    [DisplayName("Attachment 2 Name ")]
    //    public string ATTACHMENT2_NAME { get; set; }
    //    public virtual CreativeMasterViewModel CM_PROCESSATTACHMENT_TRN { get; set; }
    //}

}
