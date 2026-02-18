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
    public class ACRHeaderViewModel
    {
        [DisplayName("SNo")]
        public long ACRHEADERID { get; set; }

        [Required]
        [DisplayName("ACR Number")]
        public string ACRNO { get; set; }

        [Required]
        [DisplayName("Amount")]
        public Decimal? AMOUNT { get; set; }

        [DisplayName("PO Number")]
        public string PONO { get; set; }
        
        //Addded by aumento for new Payment processing site----------

        [DisplayName("Payment Processing Site")]
        public string SYSITE { get; set; }
        public long? SYSITE1 { get; set; }

        //-------------------End Payment processing site-------------

        [DisplayName("Supplier Code")]
        public string SUPPLIER_CODE { get; set; }

        [DisplayName("Supplier Name")]
        public string SUPPLIER_NAME { get; set; }

        [DisplayName("Remarks")]
        public string REMARK { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        public short PROCESS_STATUS { get; set; }

        [DisplayName("ACR Description")]
        public string ACR_DESC { get; set; }

        [DisplayName("Request Date")]
        public System.DateTime DATEADDED { get; set; }

        [DisplayName("Request By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBYNAME { get; set; }

        [DisplayName("Updated Date")]
        public System.DateTime? UPDATEDATE { get; set; }

        [DisplayName("Updated By")]
        public long? UPDATEDBY { get; set; }

        [DisplayName("ACR Attachment")]
        public string ACRATTACHMENT { get; set; }

        public long? SYSITEID { get; set; }
        public string SYSITEDESC { get; set; }
        public IFormFile ACRFile { get; set; }
        public string? VENDORCODE { get; set; } // added by aumento :: SR111540
        public string? VENDORNAME { get; set; } // added by aumento :: SR111540
        public List<ACRMappingViewModel> acrMapping { get; set; }
        public List<ACRDetailViewModel> acrDetail { get; set; }
        public List<ACRAppAuthSeqViewModel> acrAuthSeq { get; set; }
        public List<ACRAppHistoryViewModel> acrAppHis { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
        public short IsFinalSubmit { get; set; }
        public string ISENABLE { get; set; }
        public List<ACRAuthSkipViewModel> skipAuthList { get; set; }

        [DisplayName("Document Status")]
        public short? Document_Status { get; set; }

        [DisplayName("Document Recevied By")]
        public long? Doc_Rev_By { get; set; }

        [DisplayName("Document Recevied Date")]
        public DateTime? Doc_Rev_Date { get; set; }
        public short? PA_GENERATED { get; set; }//Added By Aumento
    }

    public class ACRDetailViewModel
    {
        [DisplayName("SNo")]
        public long ACRDTL_ID { get; set; }
        public long ACRHEADERID { get; set; }

        [DisplayName("Document Type")]
        public string DOC_TYPE { get; set; }

        [DisplayName("Attachment")]
        public string FILENAME { get; set; }

        [DisplayName("Additinal Info")]
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
        public string ACRNo { get; set; }
        
    }

    public class ACRMappingViewModel
    {
        [DisplayName("SNo")]
        public long ACRPRLNK_ID { get; set; }
        public long ACRHEADERID { get; set; }
        public string ACRNO { get; set; }

        //[DisplayName("PR No")]
        //public string PRNO { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public long? UPDATEDBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }
    }

    public class ACRAppAuthSeqViewModel
    {
        [DisplayName("SNo")]
        public long ACRAPPAUTH_ID { get; set; }
        public long ACRID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee Name")]
        public string ADEMPNAME { get; set; }

       //============Change Done on 27082022 For Add Other Category by (Aumento)================================================================================
        [DisplayName("Header")]
        public string Header { get; set; }
        //==========================================================================================================================================================

        [DisplayName("Designation")]
        public string ADDESIGNATION { get; set; }

        public short FNDESID { get; set; }

        [DisplayName("Seq")]
        public long? APP_SEQ { get; set; }
        public short STATUS { get; set; }
        public short APPTYPE { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public long? UPDATEBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }
    }

   
    public class ACRAppHistoryViewModel
    {
        [DisplayName("SNo")]
        public long ACRAPPHISTORY_ID { get; set; }
        public long ACRID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee")]
        public string APPEMP_NAME { get; set; }
        public string APP_EMAIL { get; set; }

        [DisplayName("Status")]
        public short APPROVAL_STATUS { get; set; }

        [DisplayName("Remarks")]
        public string APPROVAL_REMARK { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        //Below added on 13092022 for login user name by (Aumento)=======================================
        public string ADDEDBYName { get; set; }
        //================================================================================================

        [DisplayName("Added Date")]
        public System.DateTime ADDEDDATE { get; set; }
        public long? UPDATEBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }

        [DisplayName("Approval Date")]
        public System.DateTime? APPROVALDATE { get; set; }
        public string ACR_ATTACHMENT_NAME { get; set; }
        public string APPEMP_CODE { get; set; }
        public short APPTYPE { get; set; } //Added By Aumento as on 20022024 
        //Added By Aumento as on 31072024 for SR76166====================================
        public long? APP_SEQ { get; set; } //Added By Aumento as on 31072024
        //===============================================================================

    }

    public class ACRAuthSkipViewModel
    {
        [DisplayName("SNo")]
        public long ACRAPPSKIP_ID { get; set; }
        public long ACRID { get; set; }

        [DisplayName("Emp Code")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee")]
        public string ADEMPNAME { get; set; }
        public short STATUS { get; set; }

        [DisplayName("Remark")]
        public string SKIPREMARK { get; set; }
        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public System.DateTime ADDEDDATE { get; set; }
    }
    //Added By Aumento Start
    public class SearchACRViewModel 
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

        [DisplayName("ACR No.")]
        public string ACRNo { get; set; }

        [DisplayName("Ecode")]
        public long ecode { get; set; }

        [DisplayName("Status")]
        public short ReqStatus { get; set; }

        [DisplayName("Document Status")]
        public short Doc_Status { get; set; }

        //[DisplayName("Vendor Code")]
        //public string VendorCode { get; set; }

        //[DisplayName("Vendor Name")]
        //public string VendorName { get; set; }

        //[DisplayName("Request Number")]
        //public long RequestNumber { get; set; }
        [DisplayName("KI")]
        public long KIID { get; set; }
        //[DisplayName("Invoice Number")]
        //public string INVOICENO { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Invoice Date")]
        //public string INVOICEDATE { get; set; }
        [DisplayName("Vcode")] // added by aumento :: SR111540
        public string? VENDORCODE { get; set; } // added by aumento :: SR111540
        public string? VENDORNAME { get; set; } // added by aumento :: SR111540
        [DisplayName("PO Number")] // added by aumento :: SR111540
        public string? PONO { get; set; } // added by aumento :: SR111540
        public List<ACRHeaderViewModel>? SearchResult { get; set; }
    }

    public class VM_ACR_PaymentAdvise_Master
    {
        [DisplayName("SNo")]
        public long PAHEADER_ID { get; set; }
        [DisplayName("Payment Advice No")]
        public string PAYMENTADVISE_NO { get; set; }
        [DisplayName("Request No")]
        public long ACRNO { get; set; }
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
        //public string VenderCode { get; set; }
        //public DateTime InvoiceDate { get; set; }
        //public string VENDORCODE { get; set; }
        //public string VENDORNAME { get; set; }
        [DisplayName("From Date")]
        public string Startdate { get; set; }

        [DisplayName("End Date")]
        public string ENDDATE { get; set; }
    }

    public class VM_ACR_PADetailViewModel
    {
        [DisplayName("SNo")]
        public long PA_DTL_ID { get; set; }
        public long ACRNO { get; set; }
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
    public class DeleteACRPA
    {
        public string ACR_NO { get; set; }
        [DisplayName("Remark")]
        public string REMARK { get; set; }
        public string DOCUMENT_PATH { get; set; }
        public string PAYMENTADVISE_NO { get; set; }
        public DateTime PA_DATE { get; set; }
    }
    //Added By Aumento End
}

