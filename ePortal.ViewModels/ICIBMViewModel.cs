using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace ePortal.ViewModels
{
    class ICIBMViewModel
    {
    }
    public class ICREQHEADER
    {
        [DisplayName("Request ID")]
        public long ICREQID { get; set; }

        [DisplayName("Title")]
        public string ICTITLE { get; set; }

        [DisplayName("Department-Plant")]
        public string DEPARTMENT { get; set; }

        [DisplayName("Background")]
        public string ICBACKGROUND { get; set; }

        [DisplayName("A00")]
        public string ICA00 { get; set; }

        [DisplayName("Purpose")]
        public string ICPURPOSE { get; set; }

        [DisplayName("Target")]
        public string TARGET { get; set; }

        [DisplayName("Requirement")]
        public string REQUIREMENT { get; set; }

        [DisplayName("Content of Execution")]
        public string CONTENTOFEXE { get; set; }

        [DisplayName("Payback Period (in Months)")]
        public Nullable<long> PAYBACK_PERIOD { get; set; }

        [DisplayName("Cost Saving (In Mill. Rs./Year)")]
        public Nullable<decimal> COSTSAVING { get; set; }

        [DisplayName("Budget")]
        public Nullable<decimal> BASIC_BUDGET { get; set; }

        [DisplayName("Proposed")]
        public Nullable<decimal> BASIC_PROPOSED { get; set; }

        [DisplayName("Budget")]
        public Nullable<decimal> TAX_BUDGET { get; set; }

        [DisplayName("Proposed")]
        public Nullable<decimal> TAX_PROPOSED { get; set; }

        [DisplayName("Import/Local")]
        public Nullable<short> IMPORT_LOCAL { get; set; }

        [DisplayName("Tax Credit Available")]
        public Nullable<short> TAX_CREDIT_AVAIL { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public System.DateTime? MODIFIEDDATE { get; set; }
        public short PROCESSSTATUS { get; set; }
        public short STATUS { get; set; }
        public string REMARK { get; set; }
        public  List<VM_DGIT_ICAPPAUTHSEQ> ICAPPAUTHSEQ { get; set; }
        public  List<VM_DGIT_ICAPPHISTORY> ICAPPHISTORY { get; set; }
        public  List<VM_DGIT_ICDOCDETAIL> ICDOCDETAIL { get; set; }
        public List<VM_SELECTITEMLIST> ICINVEST_EFFECT { get; set; }
        public  List<VM_DGIT_ICREQ_SCHEDULE> ICREQ_SCHEDULE { get; set; }
        public List<string> ICINVESTSelected { get; set; }
        public short IsFinalSubmit { get; set; }
        public string INVEST_EFFECT { get; set; }
        public string ISENABLE { get; set; }
        public Employee_Details Emp_Detail { get; set; }
        public string ADDEDBYNAME { get; set; }
        public string ISICIBMREQ { get; set; }
        public DateTime? iccycle { get; set; }

        [DisplayName("Gross Asset Value (Before Acc.Dep.) (In Mill.)")]
        public Nullable<decimal> GAVAMT { get; set; }

        [DisplayName("Net Asset Value (In Mill.)")]
        public Nullable<decimal> NAVAMT { get; set; }

        [DisplayName("Scrap Realization Value (In Mill.)")]
        public Nullable<decimal> SRVAMT { get; set; }

        [DisplayName("Net P&L Impact (In Mill.)")]
        public Nullable<decimal> NPLAMT { get; set; }
        [DisplayName("AUC Code")]
        public string AUCCODE { get; set; }
        [DisplayName("Updated By")]
        public Nullable<long> AUCCODEUPDBY { get; set; }
        [DisplayName("Updated Date")]
        public System.DateTime? AUCCODEUPDDATE { get; set; }
        [DisplayName("Remark")]
        public string AUCCODEREMARK { get; set; }

    }
    public class VM_DGIT_ICAPPAUTHSEQ
    {
        [DisplayName("SNo")]
        public long ICAPPAUTH_ID { get; set; }
        public long ICREQID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }
        [DisplayName("Employee Name")]
        public string ADEMPNAME { get; set; }

        [DisplayName("Designation")]
        public string ADDESIGNATION { get; set; }
        [DisplayName("Seq")]
        public short APP_SEQ { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }
        public short APPTYPE { get; set; }
        public short ISPARRALLEL { get; set; }
        [DisplayName("Level")]
        public string APP_HEADER { get; set; }
    }
    public partial class VM_DGIT_ICAPPHISTORY
    {
        [DisplayName("SNo")]
        public long ICAPPHISTORY_ID { get; set; }
        public long ICREQID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }
        [DisplayName("Employee")]
        public string APPEMP_NAME { get; set; }
        [DisplayName("Status")]
        public short APPROVAL_STATUS { get; set; }

        [DisplayName("Remark")]
        public string APPROVAL_REMARK { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }

        [DisplayName("Approval Date")]
        public System.DateTime? APP_DATE { get; set; }

        public string APP_EMAIL { get; set; }
        public string APPEMP_CODE { get; set; }
        [DisplayName("IS IC/IBM Required")]
        public short ISICIBMREQ { get; set; }

        [DisplayName("IC Cycle")]
        public short ICCYCLEID { get; set; }
        public Nullable<short> APP_TYPE { get; set; }
        [DisplayName("IC Title")]
        public string ICTitle { get; set; }
        [DisplayName("Level")]
        public string APP_HEADER { get; set; }
        public Nullable<short> APP_SEQ { get; set; }
        public Nullable<short> TAX_CREDIT_AVAIL { get; set; }

    }
    public class VM_DGIT_ICDOCDETAIL
    {
        [DisplayName("SNo")]
        public long ICDOC_ID { get; set; }
        public long ICREQID { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public System.DateTime? UPDATEDATE { get; set; }
        [DisplayName("Document Type")]
        public string DOC_TYPE { get; set; }
        [DisplayName("Attachment")]
        public string FILENAME { get; set; }
        [DisplayName("Additional Info")]
        public string ADDITIONAL_INFO { get; set; }
        public IFormFile FILE { get; set; }
        public byte[] FILE_BYTE { get; set; }
        public string FILE_CONTENTTYPE { get; set; }
        public Int16 IsDeleted { get; set; }
        public Int16 ICTYPE { get; set; }
    }
    public class VM_DGIT_ICINVEST_EFFECT
    {
        public long ICINVESTID { get; set; }
        public long ICREQID { get; set; }
        public long INVEST_EFFECT { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public System.DateTime? MODIFIEDDATE { get; set; }
    }
    public class VM_DGIT_ICREQ_SCHEDULE
    {
        public long ICREQSCHID { get; set; }
        public long ICREQID { get; set; }
        public string TARGET { get; set; }

        public System.DateTime SCHEDULE { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public System.DateTime? MODIFIEDDATE { get; set; }
        public string SCHEDULESTR { get; set; }
    }
    public class VM_DGIT_ICINVEST_MST
    {
        public long ICINVESTMSTID { get; set; }
        public string INVESTEFFECTNAME { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public System.DateTime? MODIFIEDDATE { get; set; }
    }
    public class VM_SELECTITEMLIST
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class VM_ICManageRquest
    {
        public string REQUESTTYPE { get; set; }
        public string ICTYPE { get; set; }
        public string ICREQID { get; set; }
        public string TransactionId { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Reqname { get; set; }
        public string Status { get; set; }
        public string ProcessStatus { get; set; }
        public string AppliedDate { get; set; }
        public string IsEdit { get; set; }
        public string IsCancel { get; set; }
        
    }
    public class VM_ICManageApproval
    {
        public string REQUESTTYPE { get; set; }
        public string ICTYPE { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string TransactionId { get; set; }
        public string APPTransactionId { get; set; }
        public string Title { get; set; }
        public string Reqname { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public string ProcessStatus { get; set; }
        public string AppliedDate { get; set; }
        public string IsEdit { get; set; }
        public string IsCancel { get; set; }
        public string BASIC_BUDGET { get; set; }
        public string BASIC_PROPOSED { get; set; }
        public string PAYBACK_PERIOD { get; set; }
        public string COSTSAVING { get; set; }
    }

    public class VM_ICApprovalInitiation
    {
        [DisplayName("Request ID")]
        public List<VM_ICASSETREQLIST> ICREQID { get; set; }
        public List<VM_DGIT_ICAPPAUTHSEQ> ICAPPAUTHSEQ { get; set; }
        public List<VM_DGIT_ICAPPAUTHSEQ> ICCHKAUTH { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }

    }
    public class VM_ICIBM_FIDASHBOARD_Search
    {
        [DisplayName("Request No.")]
        public long ICREQID { get; set; }
        [DisplayName("Title")]
        public string ICTITLE { get; set; }
        [DisplayName("Department-Plant")]
        public string DEPARTMENT { get; set; }
        [DisplayName("ECode")]
        public long ADEMPCODE { get; set; }
        [DisplayName("Employee Name")]
        public string ENAME { get; set; }
        [DisplayName("User Operation")]
        public string OPERATION { get; set; }
        public DateTime? LASTAPPROVAL_FROM { get; set; }
        public DateTime? LASTAPPROVAL_TO { get; set; }
        [DisplayName("Status")]
        public long status { get; set; }
        public List<VM_ICIBM_FINANCEDASHBOARD> Result { get; set; }
        
    }
    public class VM_ICIBM_FINANCEDASHBOARD
    {
        public long ICREQID { get; set; }
        public string ICTITLE { get; set; }
        public string DEPARTMENT { get; set; }
        public long ADEMPCODE { get; set; }
        public string ENAME { get; set; }
        public string OPERATION { get; set; }
        public DateTime? LASTAPPROVAL_FROM { get; set; }
        public DateTime? LASTAPPROVAL_TO { get; set; }
        public long status { get; set; }
        public string REQUESTTYPE { get; set; }
        public string ICTYPE { get; set; }
        public Nullable<decimal> BASIC_BUDGET { get; set; }
        public Nullable<decimal> BASIC_PROPOSED { get; set; }
        public Nullable<decimal> GAVAMT { get; set; }
        public Nullable<decimal> NAVAMT { get; set; }
        public Nullable<decimal> SRVAMT { get; set; }
        public Nullable<decimal> NPLAMT { get; set; }
    }

    public class VM_ICIBM_FITAXDASHBOARD_Search
    {
        [DisplayName("Request No.")]
        public long ICREQID { get; set; }
        [DisplayName("Title")]
        public string ICTITLE { get; set; }
        [DisplayName("Department-Plant")]
        public string DEPARTMENT { get; set; }
        [DisplayName("ECode")]
        public long ADEMPCODE { get; set; }
        [DisplayName("Employee Name")]
        public string ENAME { get; set; }
        [DisplayName("User Operation")]
        public string OPERATION { get; set; }
        public DateTime? LASTAPPROVAL_FROM { get; set; }
        public DateTime? LASTAPPROVAL_TO { get; set; }
        [DisplayName("Status")]
        public long status { get; set; }
        public long PlantID { get; set; }
        public List<VM_ICIBM_FINANCETAXDASHBOARD> Result { get; set; }

    }
    public class VM_ICIBM_FINANCETAXDASHBOARD
    {
        public long ICREQID { get; set; }
        public string ICTITLE { get; set; }
        public string DEPARTMENT { get; set; }
        public long ADEMPCODE { get; set; }
        public string ENAME { get; set; }
        public string OPERATION { get; set; }
        public System.DateTime LASTAPPROVAL { get; set; }
        public short PROCESSSTATUS { get; set; }
        public string REQUESTTYPE { get; set; }
        public string ICTYPE { get; set; }
        public Nullable<decimal> STATUS { get; set; }
    }


    public class VM_ICIBM_FINAUCDASHBOARD
    {
        public long ICREQID { get; set; }
        public string ICTITLE { get; set; }
        public string DEPARTMENT { get; set; }
        public long ADEMPCODE { get; set; }
        public string ENAME { get; set; }
        public string OPERATION { get; set; }
        public System.DateTime? LASTAPPROVAL { get; set; }
        public short PROCESSSTATUS { get; set; }
        public string REQUESTTYPE { get; set; }
        public string ICTYPE { get; set; }
        public Nullable<decimal> STATUS { get; set; }
        public Nullable<long> SYPLANTID { get; set; }
    }
    public class VM_ICIBM_FIAUCDASHBOARD_Search
    {
        [DisplayName("Request No.")]
        public long ICREQID { get; set; }
        [DisplayName("Title")]
        public string ICTITLE { get; set; }
        [DisplayName("Department-Plant")]
        public string DEPARTMENT { get; set; }
        [DisplayName("ECode")]
        public long ADEMPCODE { get; set; }
        [DisplayName("Employee Name")]
        public string ENAME { get; set; }
        [DisplayName("User Operation")]
        public string OPERATION { get; set; }
        public DateTime? LASTAPPROVAL_FROM { get; set; }
        public DateTime? LASTAPPROVAL_TO { get; set; }
        [DisplayName("Status")]
        public long status { get; set; }
        public long PlantID { get; set; }
        public List<VM_ICIBM_FINAUCDASHBOARD> Result { get; set; }

    }
    public class VM_ICIBM_PPCDASHBOARD_Search
    {
        [DisplayName("Request No.")]
        public long ICREQID { get; set; }
        [DisplayName("Title")]
        public string ICTITLE { get; set; }
        [DisplayName("Department-Plant")]
        public string DEPARTMENT { get; set; }
        [DisplayName("ECode")]
        public long ADEMPCODE { get; set; }
        [DisplayName("Employee Name")]
        public string ENAME { get; set; }
        [DisplayName("User Operation")]
        public string OPERATION { get; set; }
        public DateTime? LASTAPPROVAL_FROM { get; set; }
        public DateTime? LASTAPPROVAL_TO { get; set; }
        [DisplayName("Status")]
        public short status { get; set; }
        public List<VM_ICIBM_PPCDASHBOARD> Result { get; set; }
        public string REQUESTTYPE { get; set; }
        public string ICTYPE { get; set; }



        public long SearchBy { get; set; }


    }
    public class VM_ICIBM_PPCDASHBOARD
    {

        public long ICREQID { get; set; }

        public string ICTITLE { get; set; }

        public string DEPARTMENT { get; set; }

        public long ADEMPCODE { get; set; }

        public string ENAME { get; set; }

        public string OPERATION { get; set; }
        public DateTime? LASTAPPROVAL_FROM { get; set; }
        public DateTime? LASTAPPROVAL_TO { get; set; }
        public Nullable<decimal> status { get; set; }
        public string REQUESTTYPE { get; set; }
        public string ICTYPE { get; set; }
        [DisplayName("Budget Amt.")]
        public Nullable<decimal> BASIC_BUDGET { get; set; }
        public Nullable<decimal> COSTSAVING { get; set; }
        [DisplayName("Gross Asset Value (Before Acc.Dep.) (In Mill.)")]
        public Nullable<decimal> GAVAMT { get; set; }
        [DisplayName("Net Asset Value (In Mill.)")]
        public Nullable<decimal> NAVAMT { get; set; }
        [DisplayName("Scrap Realization Value (In Mill.)")]
        public Nullable<decimal> SRVAMT { get; set; }
        [DisplayName("Net P&L Impact (In Mill.)")]
        public Nullable<decimal> NPLAMT { get; set; }
        public List<VM_SELECTITEMLIST> ICINVEST_EFFECT { get; set; }
        public string SCHEDULE { get; set; }
        public string INVEST_EFFECT { get; set; }
        public Nullable<long> PAYBACK_PERIOD { get; set; }
        public Nullable<long> OPERATIONID { get; set; }
        public Nullable<long> DIVISIONID { get; set; }
        public Nullable<long> DEPARTMENTID { get; set; }
        public Nullable<long> SECTIONID { get; set; }

    }

    #region "Asset Disposal"
    public class ICASSETDISREQHEADER
    {
        [DisplayName("Request ID")]
        public long ICREQID { get; set; }

        [DisplayName("Title")]
        public string ICTITLE { get; set; }

        [DisplayName("Department-Plant")]
        public string DEPARTMENT { get; set; }

        [DisplayName("Background")]
        public string ICBACKGROUND { get; set; }

        [DisplayName("A00")]
        public string ICA00 { get; set; }

        [DisplayName("Purpose")]
        public string ICPURPOSE { get; set; }

        [DisplayName("Target")]
        public string TARGET { get; set; }

        [DisplayName("Requirement")]
        public string REQUIREMENT { get; set; }

        [DisplayName("Asset Disposal Details")]
        public string CONTENTOFEXE { get; set; }
        
        [DisplayName("Gross Asset Value (Before Acc.Dep.)")]
        public Nullable<decimal> BASIC_BUDGET { get; set; }

        [DisplayName("Net Asset Value")]
        public Nullable<decimal> BASIC_PROPOSED { get; set; }

        [DisplayName("Scrap Realization Value")]
        public Nullable<decimal> TAX_BUDGET { get; set; }

        [DisplayName("Net P&L Impact")]
        public Nullable<decimal> TAX_PROPOSED { get; set; }

        public long ADDEDBY { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public System.DateTime? MODIFIEDDATE { get; set; }
        public short PROCESSSTATUS { get; set; }
        public short STATUS { get; set; }
        public string REMARK { get; set; }
        public List<VM_DGIT_ICAPPAUTHSEQ> ICAPPAUTHSEQ { get; set; }
        public List<VM_DGIT_ICAPPHISTORY> ICAPPHISTORY { get; set; }
        public List<VM_DGIT_ICDOCDETAIL> ICDOCDETAIL { get; set; }
        public List<VM_DGIT_ICREQ_SCHEDULE> ICREQ_SCHEDULE { get; set; }
        public short IsFinalSubmit { get; set; }
        public string ISENABLE { get; set; }
        public Employee_Details Emp_Detail { get; set; }
        public string ADDEDBYNAME { get; set; }
        public string ISICIBMREQ { get; set; }
        public DateTime? iccycle { get; set; }

    }

    public class VM_ICASSETREQLIST
    {
        public string ICREQID { get; set; }
        public string ICTYPE { get; set; }
    }
    public class VM_DGIT_ICCONFIG_MST
    {
        public long ICCONFIGID { get; set; }
        public string ICENCCONFIGID { get; set; }

        [DisplayName("IC Month")]
        public System.DateTime ICMONTH { get; set; }

        [DisplayName("IC Date")]
        public System.DateTime ICDATE { get; set; }

        [DisplayName("IC Last Submission Date")]
        public System.DateTime LASTSUBDT { get; set; }
        public int MAILTO { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long UPDATEDBY { get; set; }
        public System.DateTime UPDATEDDATE { get; set; }
        [DisplayName("Email ID")]
        public string MAILIDS { get; set; }
        public int MAILSENTTOUSER { get; set; }
 	    public Nullable<short> STATUS { get; set; }
        [DisplayName("Meeting No")]
        public short ICMEETINGNO { get; set; }
        [DisplayName("Meeting Location")]
        public string MEETINGLOC { get; set; }
        [DisplayName("Meeting Time")]
        public string MEETINGTIME { get; set; }
        [DisplayName("Contact Email")]
        public string CONTACTEMAIL { get; set; }
    }

    #endregion

}
