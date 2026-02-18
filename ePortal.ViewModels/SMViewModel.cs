using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
namespace ePortal.ViewModels
{
    public class SMHeaderViewModel
    {
        public long SMHEADERID { get; set; }

        [Required]
        [DisplayName("Type")]
        public short SM_TYPE { get; set; }

        //[Required]
        //[RegularExpression("([0-9]+)", ErrorMessage = "SES/MRN No. not valid")]
        //[StringLength(10, ErrorMessage = "SES/MRN No. maximum length is 10")]
        [DisplayName("SES/MRN No.")]
        public string? SM_NO { get; set; }

        //[Required]
        [RegularExpression("([0-9]+)", ErrorMessage = "PO Number not valid")]
        [StringLength(10, ErrorMessage = "PO Number maximum length is 10")]
        [DisplayName("Purchase Order")]
        public string PONUMBER { get; set; }

        [Required]
        [DisplayName("SES/MRN Amount")]
        public decimal AMOUNT { get; set; }

        [Required]
        [DisplayName("Invoice Number")]
        public string INVOICENO { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Invoice Date")]
        [Required]
        public System.DateTime? INVOICEDATE { get; set; }
        public string? STR_INVDATE { get; set; }

        [Required]
        [DisplayName("Invoice Amount")]
        public decimal? INVAMOUNT { get; set; }

        //[Required]
        [DisplayName("Invoice Type")]
        public short? INVSIGTYPE { get; set; }

        //[Required]
        [DisplayName("Vendor Code")]
        public string VENDORCODE { get; set; }

        [DisplayName("Vendor Name")]
        public string? VENDORNAME { get; set; }

        [DisplayName("Nature Of Expense")]
        public string REMARK { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        public short PROCESS_STATUS { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Request Date")]
        public System.DateTime DATEADDED { get; set; }

        [DisplayName("Request By")]
        public long ADDEDBY { get; set; }
        public string? ADDEDBYNAME { get; set; }

        [DisplayName("Updated Date")]
        public System.DateTime? UPDATEDATE { get; set; }

        [DisplayName("Updated By")]
        public long? UPDATEDBY { get; set; }

        [DisplayName("SES/MRN Attachment")]
        public string? SMATTACHMENT { get; set; }
        public IFormFile? SMFile { get; set; }

        public List<SMDetailViewModel>? smDetail { get; set; }
        public List<SMAppAuthSeqViewModel>? smAuthSeq { get; set; }
        public List<SMAppHistoryViewModel>? smAppHis { get; set; }
        public virtual Employee_Details? Emp_Detail { get; set; }
        public short IsFinalSubmit { get; set; }
        public string? ISENABLE { get; set; }

        [DisplayName("Document Status")]
        public short? Document_Status { get; set; }

        [DisplayName("Document Recevied By")]
        public long? Doc_Rev_By { get; set; }

        [DisplayName("Document Recevied Date")]
        public DateTime? Doc_Rev_Date { get; set; }
        public short TaxationAppStatus { get; set; }
        public short? PA_GENERATED { get; set; }
        public List<SMDetailViewModel>? SESNO { get; set; }

        //Below added by aumento for SESMRN==================
        [DisplayName("Payment Processing Site")]
        public string SYSITE { get; set; }
        public long? SYSITE1 { get; set; }

        public bool IPServiceMatDeclaration { get; set; }

        [DisplayName("Please select this checkbox if any single item in this contains to use of VI Elements like Company logos, or play / perform live or recorded music at the event (If user willfully create PR without IP Check sheet(if applicable), User & DH/DVH shall be responsible for violation of HCG )")]
        public string? IPServiceMatDeclarationText { get; set; }
        //===================================================
    }

    public class SMDetailViewModel
    {
        [DisplayName("SNo")]
        public long SMDTL_ID { get; set; }
        public long SMHEADERID { get; set; }

        [DisplayName("Document Type")]
        public string DOC_TYPE { get; set; }

        [DisplayName("Attachment")]
        public string FILENAME { get; set; }

        [DisplayName("Additional Info")]
        public string ADDITIONAL_INFO { get; set; }

        public IFormFile FILE { get; set; }
        public byte[] FILE_BYTE { get; set; }
        public string FILE_CONTENTTYPE { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public long? UPDATEDBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }
        public Int16 IsDeleted { get; set; }
        [DisplayName("SES/MRN No.")]
        public string SMNo { get; set; }
    }

    public class SMAppAuthSeqViewModel
    {
        [DisplayName("SNo")]
        public long SMAPPSEQID { get; set; }
        public long SMHEADERID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee Name")]
        public string ADEMPNAME { get; set; }

        [DisplayName("Designation")]
        public string ADDESIGNATION { get; set; }

        [DisplayName("Seq")]
        public short APP_SEQ { get; set; }
        public short STATUS { get; set; }
        public short APPTYPE { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public long? UPDATEBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }
        public short FNDESID { get; set; }
        [DisplayName("Functional Designation")]
        public string FUNDESG_NAME { get; set; }
    }

    public class SMAppHistoryViewModel
    {
        [DisplayName("SNo")]
        public long SMAPPHISTORYID { get; set; }
        public long SMHEADERID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee")]
        public string APPEMP_NAME { get; set; }
        public string APPEMP_CODE { get; set; }
        public string APP_EMAIL { get; set; }

        [DisplayName("Status")]
        public short APPROVAL_STATUS { get; set; }

        [DisplayName("Remarks")]
        public string APPROVAL_REMARK { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public System.DateTime ADDEDDATE { get; set; }
        public long? UPDATEBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}"), DisplayName("Approval Date")]
        public System.DateTime? APPROVALDATE { get; set; }
        public string SM_ATTACHMENT_NAME { get; set; }
        public short APPTYPE { get; set; }
        public string[] TAXATION_AUTHORITY { get; set; }
        public long SELECTED_ADEMPCODE { get; set; }
    }

    public class SmOpMapViewModel
    {
        public long SMMAP_MSTID { get; set; }
        public long ADORGLEVELID { get; set; }
        public long? APPROVER1 { get; set; }
        public string APPROVER1Name { get; set; }
        public string APPROVER1Desg { get; set; }
        public short ACTIVE { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime? DATELSTMOD { get; set; }
        public long? MODIFIEDBY { get; set; }
        public long STARTAMOUNT { get; set; }
        public long ENDAMOUNT { get; set; }
        public long? APPROVER2 { get; set; }
        public string APPROVER2Name { get; set; }
        public string APPROVER2Desg { get; set; }
        public long SYKIID { get; set; }
        public short SESORMRN { get; set; }
    }

    public class SearchSMViewModel
    {
        [DisplayName("Operation")]
        public long OperationID { get; set; }

        [DisplayName("Division")]
        public long DivisionID { get; set; }

        [DisplayName("Department")]
        public long DEPTID { get; set; }

        [DisplayName("Section")]
        public long SECID { get; set; }

        [DisplayName("From Date")]
        public string Startdate { get; set; }

        [DisplayName("End Date")]
        public string ENDDATE { get; set; }

        [DisplayName("SES/MRN No.")]
        public string SMNo { get; set; }

        [DisplayName("Ecode")]
        public long ecode { get; set; }

        [DisplayName("Status")]
        public short ReqStatus { get; set; }

        [DisplayName("Document Status")]
        public short Doc_Status { get; set; }

        [DisplayName("Vendor Code")]
        public string VendorCode { get; set; }

        [DisplayName("Vendor Name")]
        public string? VendorName { get; set; }

        [DisplayName("Request Number")]
        public long RequestNumber { get; set; }
        [DisplayName("KI")]
        public long KIID { get; set; }
        [DisplayName("Invoice Number")]
        public string INVOICENO { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Invoice Date")]
        public string INVOICEDATE { get; set; }
        public List<SMHeaderViewModel> SearchResult { get; set; }
    }

    public class SMTaxationAuthViewModel
    {
        public long SMTAXEMPID { get; set; }
        public long SYPLANTID { get; set; }
        public long ECODE { get; set; }
        public string ENAME { get; set; }
        public short EMPTYPE { get; set; }
        public short ACTIVE { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long? ADDEDBY { get; set; }
    }

    public class SearchIOCGMaster
    {
        [DisplayName("KI")]
        public long KI { get; set; }
        [DisplayName("Operation")]
        public long OperationID { get; set; }

        [DisplayName("Division")]
        public long DivisionID { get; set; }

        [DisplayName("Department")]
        public long DEPTID { get; set; }

        [DisplayName("Section")]
        public long SECID { get; set; }

        [DisplayName("Status")]
        public Int16 STATUS { get; set; }
        public long curr_KI { get; set; }
        public List<VM_VW_SMIOCGAPPROVER_LIST> SearchResult { get; set; }
    }

    public class VM_VW_SMIOCGAPPROVER_LIST
    {
        public long SMMAP_MSTID { get; set; }
        public long ADORGLEVELID { get; set; }
        public string ORGNAME { get; set; }
        [DisplayName("Approver 1")]
        public long APPROVER1 { get; set; }
        [DisplayName("Status")]
        public short ACTIVE { get; set; }
        public System.DateTime? UPDDATE { get; set; }
        public long? UPDBY { get; set; }

        [DisplayName("Amount From")]
        public long STARTAMOUNT { get; set; }

        [DisplayName("Amount To")]
        public long ENDAMOUNT { get; set; }
        public long SYKIID { get; set; }

        [DisplayName("SES/MRN")]
        public short SESORMRN { get; set; }
        [DisplayName("Approver 2")]
        public long? APPROVER2 { get; set; }
        public string ORGLEVEL_1 { get; set; }
        public string ORGLEVEL_2 { get; set; }
        public string APP1_NAME { get; set; }
        public string APP2_NAME { get; set; }
        public string ORGLEVEL_3 { get; set; }

        [DisplayName("Operation")]
        public long? OPERID { get; set; }

        [DisplayName("Division")]
        public long? DIVID { get; set; }

        [DisplayName("Department")]
        public long? DEPTID { get; set; }

        [DisplayName("Section")]
        public long? SECID { get; set; }
    }

    public class VM_PaymentAdvise_Master
    {
        [DisplayName("SNo")]
        public long PAHEADER_ID { get; set; }
        [DisplayName("Payment Advice No")]
        public string PAYMENTADVISE_NO { get; set; }
        [DisplayName("Request No")]
        public long SES_MRN_NO { get; set; }
        [DisplayName("Document Path")]
        public string DOCUMENT_PATH { get; set; }
        public short? STATUS { get; set; }
        public short ISDELETE { get; set; }
        [DisplayName("Remark")]
        public string REMARK { get; set; }
        public DateTime DATEADDED { get; set; }
        public DateTime UPDATEDDATE { get; set; }
        public long? ADDEDBY { get; set; }
        public long UPDATEBY { get; set; }
        [DisplayName("Payment Advice Date")]
        public DateTime PA_DATE { get; set; }
        public List<string> reqno_list { get; set; }
        public short ISSEND { get; set; }
        public short datafromcsv { get; set; }
        public string Invoiceno { get; set; }
        public string VenderCode { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string VENDORCODE { get; set; }
        public string VENDORNAME { get; set; }
        [DisplayName("From Date")]
        public string Startdate { get; set; }

        [DisplayName("End Date")]
        public string ENDDATE { get; set; }
    }

    public class VM_PADetailViewModel
    {
        [DisplayName("SNo")]
        public long PA_DTL_ID { get; set; }
        public long SES_MRN_NO { get; set; }
        public string PAYMENTADVISE_NO { get; set; }
        public DateTime PA_DATE { get; set; }

        [DisplayName("Document Type")]
        public string DOC_TYPE { get; set; }

        [DisplayName("Attachment")]
        public string FILENAME { get; set; }

        [DisplayName("Additional Info")]
        public string ADDITIONAL_INFO { get; set; }
        public IFormFile FILE { get; set; }
        public byte[] FILE_BYTE { get; set; }
        public string FILE_CONTENTTYPE { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public long? UPDATEDBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }
        public Int16 IsDeleted { get; set; }
    }
    public class VM_Combine_PDF
    {
        public long SES_NO { get; set; }

    }
    public class DeletePA
    {
        public string SES_NO { get; set; }
        [DisplayName("Remark")]
        public string REMARK { get; set; }
        public string DOCUMENT_PATH { get; set; }
        public string PAYMENTADVISE_NO { get; set; }
        public DateTime PA_DATE { get; set; }
    }
    //Below added by aumento for the SR50703 on 18052023==========================================
    public class SMDGIT_SMIOCGBLOCKViewModel
    {
        public long SRNO { get; set; }
        public long SMIOCGBLOCKID { get; set; }
        public string LevelType { get; set; }
        public long ADORGLEVELID { get; set; }
        public string LevelDesc { get; set; }
        public long STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long UPDATEDBY { get; set; }
        public System.DateTime UPDATEDDATE { get; set; }
        
    }
    //==================================================================================================

}
