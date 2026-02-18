using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;

namespace ePortal.ViewModels
{
    public class CommunicationViewModel
    {
        [DisplayName("SNo")]
        public long COMMUNICATIONID { get; set; }

        [Required]
        [DisplayName("Category")]
        public long COMM_CATID { get; set; }
        public string? CATEGORY { get; set; }

        [Required]
        [DisplayName("Sender Email")]
        public long COMM_MAILTYPEID { get; set; }
        public string? MAILTYPE { get; set; }

        [DisplayName("Mail ID")]
        public string? MAILID { get; set; }

        [Required]
        [DisplayName("Subject")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Subject must be between 10 and 200 char")]
        public string? SUBJECT { get; set; }

        [Required]
        [DisplayName("Description")]
        [StringLength(2000, MinimumLength = 100, ErrorMessage = "Description must be between 100 and 2000 char")]
        public string? CONTENT { get; set; }

        [AllowHtml]
        [DisplayName("Email Content")]
        //[StringLength(10000, MinimumLength = 100, ErrorMessage = "Email content must be between 100 and 10000 char")]
        public string? EMAIL_CONTENT { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Active From")]
        public string? START_DATE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? _SDATE { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Active To")]
        public string? END_DATE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? _EDATE { get; set; }

        [DisplayName("Status")]
        public Int16 STATUS { get; set; }

        [DisplayName("Process Status")]
        public Int16 PROCESS_STATUS { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Created Date")]
        public System.DateTime CREATED_DATE { get; set; }

        [DisplayName("Created By")]
        public long CREATED_BY { get; set; }

        [DisplayName("Created By")]
        public string CREATED_BY_NAME { get; set; }
        public string PENDINGAT_NAME { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        [DisplayName("Functional Designation")]
        public Nullable<long>[] FUNCTIONAL_DESIGNATION { get; set; }
        [DisplayName("Designation")]
        public Nullable<long>[] DESIGNATION { get; set; }
        [DisplayName("Location")]
        public Nullable<long>[] SITE { get; set; }
        [DisplayName("Operation")]
        public Nullable<long>[] OPERATION { get; set; }

        [DisplayName("Division")]
        public Nullable<long>[] DIVISION { get; set; }

        [DisplayName("Department")]
        public Nullable<long>[] DEPARTMENT { get; set; }

        [DisplayName("Functional Designation")]
        public string FnDesignationDescrip { get; set; }
        [DisplayName("Designation")]
        public string DesignationDescrip { get; set; }
        [DisplayName("Location")]
        public string SiteDescrip { get; set; }

        [DisplayName("Operation")]
        public string OPDescrip { get; set; }

        [DisplayName("Division")]
        public string DIVDescrip { get; set; }

        [DisplayName("Department")]
        public string DEPDescrip { get; set; }

        [DisplayName("Request No")]
        public string REQUESTNO { get; set; }

        [DisplayName("Automate Email Sending")]
        public bool EMAILSTATUS { get; set; }

        [Required]
        [DisplayName("Request Type")]
        public short REQUEST_TYPE { get; set; }
        public List<CommunicationDtlViewModel> commDetail { get; set; }
        public List<CommunicationAppSeqViewModel> commAuthSeq { get; set; }
        public List<CommunicationAppHisViewModel> commAppHis { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
        public List<CommunicationAppSeqViewModel> additionalCommAuthSeq { get; set; }
        public virtual SearchViewModel SearchViewModel { get; set; }
        public short IsFinalSubmit { get; set; }
        public string FnDesignationIds { get; set; }
        public string DesignationIds { get; set; }
        public string SiteIds { get; set; }
        public string OPIds { get; set; }
        public string DIVIds { get; set; }
        public string DEPIds { get; set; }
        public string ISENABLE { get; set; }
        [AllowHtml]
        public string EmialContect { get; set; }

        [Required(ErrorMessage = "Please choose recipient option ")]
        public short Recipient { get; set; }
        public short DesignationOption { get; set; }
        [DisplayName("Mail Status")]
        public short MAIL_SENT_STATUS { get; set; }

    }

    public class CommunicationDtlViewModel
    {
        [DisplayName("SNo")]
        public long COMM_DTLID { get; set; }
        public long COMMUNICATIONID { get; set; }
        [DisplayName("Document Type")]
        public string DOC_TYPE { get; set; }
        [DisplayName("File Name")]
        public string FILENAME { get; set; }
        [DisplayName("Additional Info")]
        public string ADDITIONAL_INFO { get; set; }
        public IFormFile FILE { get; set; }
        public byte[] FILE_BYTE { get; set; }
        public string FILE_CONTENTTYPE { get; set; }
        public Int16 STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public Int16 IsDeleted { get; set; }

    }

    public class CommunicationAppHisViewModel
    {
        [DisplayName("SNo")]
        public long COMM_HISTORYID { get; set; }
        public long COMMUNICATIONID { get; set; }
        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }
        [DisplayName("Employee Name")]
        public string ADEMPNAME { get; set; }
        public string APP_EMAIL { get; set; }

        [DisplayName("Status")]
        public Int16 APPROVAL_STATUS { get; set; }
        [DisplayName("Remark")]
        public string APPROVAL_REMARK { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}"), DisplayName("Approval Date")]
        public Nullable<System.DateTime> APPROVALDATE { get; set; }
        public short APPTYPE { get; set; }

        [DisplayName("Subject")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Subject must be between 10 and 200 char")]
        public string SUBJECT { get; set; }

        [DisplayName("Description")]
        [StringLength(2000, MinimumLength = 100, ErrorMessage = "Description must be between 100 and 2000 char")]
        public string CONTENT { get; set; }

        [AllowHtml]
        [DisplayName("Email Content")]
        [StringLength(4000, MinimumLength = 100, ErrorMessage = "Email content must be between 100 and 4000 char")]
        public string EMAIL_CONTENT { get; set; }
        public Int16 CHANGES_REQUIRED { get; set; }

    }

    public class CommunicationAppSeqViewModel
    {
        [DisplayName("SNo")]
        public long COMMAPPAUTH_ID { get; set; }
        public long COMMUNICATIONID { get; set; }
        public long ADEMPCODE { get; set; }
        [DisplayName("Employee Name")]
        public string ADEMPNAME { get; set; }
        public string FNAME { get; set; }
        public string LNAME { get; set; }

        [DisplayName("Designation")]
        public string ADDESIGNATION { get; set; }
        public long? FNDESID { get; set; }
        public Int16 APP_SEQ { get; set; }
        public Int16 STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public Int16 APPTYPE { get; set; }
        public Int16 ISADDITIONAL_AUTH { get; set; }
    }

    public class CommCategoryViewModel
    {
        public long COMMCATEGORYID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public Int16 STATUS { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public long CREATED_BY { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public byte APP_REQUIRED_TILL { get; set; }
        public short ISREQ_TEMPLATE { get; set; }
        public short ISREQ_BANNER { get; set; }
        public short ISREQ_OTHER { get; set; }
    }

    public class CommMailTypeViewModel
    {
        public long COMMTYPEID { get; set; }
        [DisplayName("Communication Type")]
        public string COMM_TYPE { get; set; }
        [DisplayName("Status")]
        public bool STATUS { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Added Date")]
        public System.DateTime CREATED_DATE { get; set; }
        public long CREATED_BY { get; set; }
        [DisplayName("Added By")]
        public string ADDEDBY_NAME { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }

        [DisplayName("Mail ID")]
        public string COMM_EMAILID { get; set; }
    }

    public class COMMSIGLIST
    {
        public string Designation { get; set; }
        public List<CommunicationAppHisViewModel> appList { get; set; }
    }

    public class Comm_Op_Div_DepViewModel
    {
        public long Value { get; set; }
        public string Text { get; set; }
    }
    public class CommRequest
    {
        public long id { get; set; }
    }

    public class CommunicationPreviewModel
    {
        public long id { get; set; }
        public string IsChanges { get; set; }
        public string TEMP_PRV_FNAME { get; set; }
    }
    public class CommunicationApprovalRequest
    {
        public CommunicationAppHisViewModel ADVM { get; set; }
        public string CommCategory { get; set; }
        public string TEMP_PRV_FNAME { get; set; }
    }


}
