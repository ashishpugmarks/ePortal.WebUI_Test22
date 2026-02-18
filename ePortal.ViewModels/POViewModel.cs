using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ePortal.ViewModels
{
    public class POHeaderViewModel
    {

        [DisplayName("SNo")]
        public long POHEADERID { get; set; }
        [Required]
        [DisplayName("PO No.")]
        public string PONO { get; set; }

        [Required]
        [DisplayName("Vendor Code")]
        public string VENDORID { get; set; }
        [DisplayName("Vendor")]
        public string? VENDORNAME { get; set; }

        [DisplayName("Remarks")]
        public string? REMARK { get; set; }

        [DisplayName("Status")]
        public short? STATUS { get; set; }
        public short? PROCESS_STATUS { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        [DisplayName("Vendor Email")]
        public string VENDORMAILID { get; set; }

        [DisplayName("Contract No.")]
        public string CONTRACT_NO { get; set; }

        [DisplayName("PO Description")]
        public string PO_DESC { get; set; }

        [DisplayName("Request Date")]
        public System.DateTime DATEADDED { get; set; }

        [DisplayName("Request By")]
        public long? ADDEDBY { get; set; }
        public string? ADDEDBYNAME { get; set; }

        [DisplayName("Updated Date")]
        public Nullable<System.DateTime> UPDATEDATE { get; set; }

        [DisplayName("Updated By")]
        public Nullable<long> UPDATEDBY { get; set; }
        public string? ISENABLE { get; set; }

        [DisplayName("PO Attachment")]
        public string? POATTACHMENT { get; set; }
        public IFormFile? POFile { get; set; }

        [DisplayName("PR No.")]
        public string[]? PRNO_ARRAY { get; set; }

        public List<POMappingViewModel>? poMapping { get; set; }
        public List<PODetailViewModel>? poDetail { get; set; }
        public List<POAppAuthSeqViewModel>? poAuthSeq { get; set; }
        public List<POAppHistoryViewModel>? poAppHis { get; set; }
        public Employee_Details? Emp_Detail { get; set; }
        public short? IsFinalSubmit { get; set; }
        public List<POAppSkipViewModel>? poAuthSkipList { get; set; }

        [DisplayName("Urgency")]
        public short? HIGH_URGENCY { get; set; }

        [DisplayName("Plant")]
        public string? Plant { get; set; }

        [DisplayName("Ammendment No.")]
        public short? VERSIONNO { get; set; }
    }

    public class PODetailViewModel
    {
        [DisplayName("SNo")]
        public long PODTL_ID { get; set; }
        public long POHEADERID { get; set; }

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
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public Int16 IsDeleted { get; set; }
        public string PONo { get; set; }
    }

    public class POMappingViewModel
    {
        [DisplayName("SNo")]
        public long? POPRLNK_ID { get; set; }
        public long? POHEADERID { get; set; }
        public string? PONO { get; set; }

        [DisplayName("PR No.")]
        public string? PRNO { get; set; }

        [DisplayName("Status")]
        public short? STATUS { get; set; }
        public long? ADDEDBY { get; set; }
        public System.DateTime? ADDEDDATE { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
    }

    public class POAppAuthSeqViewModel
    {
        [DisplayName("SNo")]
        public long POAPPAUTH_ID { get; set; }
        public long POID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee Name")]
        public string ADEMPNAME { get; set; }

        [DisplayName("Designation")]
        public string ADDESIGNATION { get; set; }
        public short FNDESID { get; set; }

        [DisplayName("Seq")]
        public short APP_SEQ { get; set; }
        public short STATUS { get; set; }
        public short APPTYPE { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
    }

    public class POAppHistoryViewModel
    {
        [DisplayName("SNo")]
        public long POAPPHISTORY_ID { get; set; }
        public long POID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee")]
        public string APPEMP_NAME { get; set; }
        public string APPEMP_CODE { get; set; }
        public string APP_EMAIL { get; set; }

        [DisplayName("Status")]
        public short APPROVAL_STATUS { get; set; }

        [DisplayName("Remarks")]
        public string? APPROVAL_REMARK { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayName("Added Date")]
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}"), DisplayName("Approval Date")]
        public Nullable<System.DateTime> APPROVALDATE { get; set; }
        public string? PO_ATTACHMENT_NAME { get; set; }
        public short? High_Urgency { get; set; }
    }

    public class VendorViewModel
    {
        public string VENDORCODE { get; set; }
        public string VENDORNAME { get; set; }
        public string VENDOREMAIL { get; set; }

    }

    public class POAppSkipViewModel
    {
        [DisplayName("SNo")]
        public long POAPPSKIP_ID { get; set; }
        public long POID { get; set; }
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

    public class VW_PODASHBOARD_VIEWMODEL
    {
        public long POHEADERID { get; set; }
        public string PONO { get; set; }
        public string VENDORNAME { get; set; }
        public string PRNO { get; set; }
        public long ADDEDBY { get; set; }
        public string ADDEDBYNAME { get; set; }
        public System.DateTime UPDATEDATE { get; set; }
        public string FILENAME { get; set; }
        public string PLANTID { get; set; }
        public Nullable<long> VERSION_NO { get; set; }
    }

    public class Search_VW_PODASHBOARD_VWMODEL
    {
        public long? POHEADERID { get; set; }

        [DisplayName("PO No.")]
        public string PONO { get; set; }

        [DisplayName("Vendor Name")]
        public string VENDORNAME { get; set; }

        [DisplayName("Indent No.")]
        public string PRNO { get; set; }

        [DisplayName("PO Created By")]
        public long ADDEDBY { get; set; }

        [DisplayName("PO Creator Name")]
        public string ADDEDBYNAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public System.DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public System.DateTime EndDate { get; set; }
        public List<VW_PODASHBOARD_VIEWMODEL>? searchResult { get; set; }
        [DisplayName("Plant")]
        public string PLANTID { get; set; }

        [DisplayName("Ammendment No.")]
        public long VERSION_NO { get; set; }
    }

    public class SearchPO
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

        [DisplayName("Ecode")]
        public long ecode { get; set; }

        [DisplayName("Status")]
        public short Status { get; set; }
        [DisplayName("Remark")]
        public string ITEM_DETAIL { get; set; }
        public List<VM_VW_DGIT_POREPORT> SearchResult { get; set; }
        [DisplayName("Approval Type")]
        public long ISSpecialRight { get; set; }

        public long loginid { get; set; }
        [DisplayName("KI")]
        public long KIID { get; set; }

        public long IsTeamMember { get; set; }
        public string ADDEDBYNAME { get; set; }

    }
    public class VM_VW_DGIT_POREPORT
    {
        public long POHEADERID { get; set; }
        public string PONO { get; set; }
        public string VENDORID { get; set; }
        public string REMARK { get; set; }
        public short STATUS { get; set; }
        public short PROCESS_STATUS { get; set; }
        public string VENDORMAILID { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public string CONTRACT_NO { get; set; }
        public string PO_DESC { get; set; }
        public Nullable<short> HIGH_URGENCY { get; set; }
        public short MAILCNT { get; set; }
        public string PLANTID { get; set; }
        public Nullable<short> VERSION_NO { get; set; }
        public Nullable<long> OPERATIONID { get; set; }
        public string OPERATION { get; set; }
        public Nullable<long> DIVISIONID { get; set; }
        public string DIVISION { get; set; }
        public Nullable<long> DEPARTMENTID { get; set; }
        public string DEPARTMENT { get; set; }
        public Nullable<long> SECTIONID { get; set; }
        public string SECTION { get; set; }
        public string ADDEDBYNAME { get; set; }
        public string VENDORNAME { get; set; }
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
    }
}
