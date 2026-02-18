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
    public class PRHeaderViewModel
    {
        public long PRHEADERID { get; set; }

        [Required]
        [DisplayName("Indent No.")]
        public string IndentNo { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Indent Date")]
        public DateTime? IndentDate { get; set; }

        [DisplayName("Indent Amount")]
        public decimal IndentAmount { get; set; }

        [DisplayName("Remark")]
        public string ItemDetail { get; set; }

        [DisplayName("Remark")]
        public string? Remark { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        public short PROCESS_STATUS { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Request Date")]
        public System.DateTime DATEADDED { get; set; }

        [DisplayName("Request By")]
        public long ADDEDBY { get; set; }
        public string ADDEDBYNAME { get; set; }

        [DisplayName("Updated Date")]
        public Nullable<System.DateTime> UPDATEDATE { get; set; }

        [DisplayName("Updated By")]
        public Nullable<long> UPDATEDBY { get; set; }

        [DisplayName("PR Attachment")]
        public string PRATTACHMENT { get; set; }
        public IFormFile PRFile { get; set; }

        public List<PRDetailViewModel> prDetail { get; set; }
        public List<PRAppAuthSeqViewModel> prAuthSeq { get; set; }
        public List<PRAppHistoryViewModel> prAppHis { get; set; }
        public List<PRPURStatusViewModel> prBuyerHis { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
        public short IsFinalSubmit { get; set; }

        public string ISENABLE { get; set; }
        public List<PRAPPSKIPViewModel> skipAuthList { get; set; }

        [DisplayName("PR Status")]
        public short PRStatus { get; set; }

        [DisplayName("Budget Head")]
        public long PRTYPE { get; set; }

        public List<PRBuyerMapViewModel> BUYER_MODEL { get; set; }

        //Below added by aumento for the SR70991==============================================================
        public List<PRBuyerMapViewModel> BUYER_MODEL_NEW { get; set; }
        public List<PRPURStatusViewModel> prBuyerHis_HOLD { get; set; }
        public List<PRPURStatusViewModel> prBuyerHis_UNHOLD { get; set; }
        public List<PRPURStatusViewModel> prBuyerHis_SENDACK { get; set; }

        //=====================================================================================================
        [DisplayName("Category")]
        public long CATID { get; set; }
        public string CATEGORY { get; set; }

        [DisplayName("Buyer Name")]
        public string Buyer_FName { get; set; }
        public string Buyer_LName { get; set; }
        public long Buyer_ECODE { get; set; }

        //SIS PR Change
        public bool ITServiceMatSISAppStatus { get; set; }

        [DisplayName("Please select this checkbox even if any single line item in this PR is IT Material or IT Service(s) as per the attached PDF check list")]
        public string ITServiceMaterialText { get; set; }
        public Nullable<short> IS_SISPR { get; set; }
        public bool IS_SISPRAPPROVAL { get; set; }
        public bool NonITServiceMatDeclaration { get; set; }

        [DisplayName("I hereby declare that there is no IT material or IT service(s) in this PR")]
        public string NonITServiceMatDeclarationText { get; set; }
        public Nullable<short> IS_NONSISPR { get; set; }
        public string APP_TYPEINFO { get; set; }

        public bool IPServiceMatDeclaration { get; set; }

        [DisplayName("Please select this checkbox if any single item in this PR contains to use of VI Elements like Company logos, or play / perform live or recorded music at the event \n <span style='color:red;'>( If user willfully create PR without IP Check sheet(if applicable), User & DH/DVH shall be responsible for violation of HCG )</span>")]
        public string IPServiceMatDeclarationText { get; set; }
        //SIS PR Change
        [DisplayName("Ariba RFP ID")]
        public string? ARIBARFPID { get; set; }
        [DisplayName("Ariba Doc Id applicable")]
        public Nullable<short> ARIBASTATUS { get; set; }
        [DisplayName("Ariba Buyer (Purchase)")]
        public long? ARIBABuyer_ECODE { get; set; }

        [DisplayName("Ariba SR No")] //Added by aumento for the SR73841
        public string? ARIBASRNO { get; set; } //Added by aumento for the SR73841
        [DisplayName("PR Release Location")] //  Added by Aumento ::  SR68003
        public Nullable<long> PR_ReleaseLocation { get; set; }

        [DisplayName("Release Location")] //  Added by Aumento ::  SR68003
        public string PRReleaseLocation { get; set; }

        [DisplayName("SLA Category")] //  Added by ttl ::  SR68003
        public long? SLA_Category { get; set; }
    }

    public class PRDetailViewModel
    {
        [DisplayName("SNo")]
        public long PRDTL_ID { get; set; }
        public long PRHEADERID { get; set; }

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
        public string PRNo { get; set; }
    }

    public class PRAppAuthSeqViewModel
    {
        [DisplayName("SNo")]
        public long PRAPPAUTH_ID { get; set; }
        public long PRID { get; set; }

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
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public short FNDESID { get; set; }
        public short PRINTORDER { get; set; }
        public short ISPARALELLAPP { get; set; }
        public short ADDESIGNATIONID { get; set; }
        public short ADACTUAL_FUNCDESGID { get; set; }
        public string ACTIONFOR { get; set; }
        public Nullable<short> IS_SISPR { get; set; }         //SIS PR Change
        public string APP_TYPEINFO { get; set; }         //SIS PR Change
    }

    public class PRAppHistoryViewModel
    {
        [DisplayName("SNo")]
        public long PRAPPHISTORY_ID { get; set; }
        public long PRID { get; set; }

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
        public short APP_SEQ { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss}"), DisplayName("Approval Date")]
        public Nullable<System.DateTime> APPROVALDATE { get; set; }
        public string PR_ATTACHMENT_NAME { get; set; }
        public string ACTIONFOR { get; set; }

        //SIS PR Change
        public Nullable<System.DateTime> HOLD_DATE { get; set; }
        public string HOLD_REMARKS { get; set; }
        public Nullable<long> HOLD_BY { get; set; }
        public string APP_TYPEINFO { get; set; }
        //SIS PR Change
    }
    public class PRAPPSKIPViewModel
    {
        [DisplayName("SNo")]
        public long PRAPPSKIP_ID { get; set; }
        public long PRID { get; set; }

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

    public class VW_DGIT_PRADDAUTH_MAP
    {
        public long PRADDAUTH_ID { get; set; }
        public short PRTYPEID { get; set; }
        public short ORGLVLID { get; set; }
        public long RES_ORGLVLID { get; set; }
        public long ADORGLEVELID { get; set; }
        public long ADEMPCODE { get; set; }

        public long ADORGLEVELTYPEID { get; set; }
    }

    public class SearchIndent
    {
        [DisplayName("Operation")]
        public long OperationID { get; set; }

        public List<long> MapOperationIDs { get; set; }

        [DisplayName("Division")]
        public long DivisionID { get; set; }

        [DisplayName("Department")]
        public long DEPTID { get; set; }

        [DisplayName("Section")]
        public long SECID { get; set; }

        [DisplayName("Plant")]
        public long? PlantID { get; set; }

        [DisplayName("From Date")]
        public string? Startdate { get; set; }

        [DisplayName("End Date")]
        public string? ENDDATE { get; set; }

        [DisplayName("Indent No.")]
        public string? IndentNo { get; set; }

        [DisplayName("Ecode")]
        public long ecode { get; set; }

        [DisplayName("Allocation Status")]
        public short? PRStatus { get; set; }
        public List<PRHeaderViewModel> SearchResult { get; set; }

        [DisplayName("GP1ORGP2")]
        public long CATID { get; set; }

        [DisplayName("Buyer ID")]
        public long BuyerID { get; set; }

        [DisplayName("SLA Category")]
        public long SLA_Category { get; set; }
    }

    public class PR_Div_Dep_SecViewModel
    {
        public long Value { get; set; }
        public string Text { get; set; }
    }

    public class PRPURStatusViewModel
    {
        [DisplayName("SNo")]
        public long PRPURSTATUSID { get; set; }
        public long PRHEADERID { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Remark")]
        public string REMARK { get; set; }

        [DisplayName("Date")]
        public System.DateTime DATEADDED { get; set; }

        [DisplayName("Buyer")]
        public long ADDEDBY { get; set; }
        public string ADDBY_NAME { get; set; }
    }

    public class SearchIndentUser
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
        public string? Startdate { get; set; }

        [DisplayName("End Date")]
        public string? ENDDATE { get; set; }

        [DisplayName("PR No.")]
        public string? IndentNo { get; set; }

        [DisplayName("Ecode")]
        public long ecode { get; set; }

        [DisplayName("PR Status")]
        public short PRStatus { get; set; }
        [DisplayName("Remark")]
        public string? ITEM_DETAIL { get; set; }
        public List<PRHeaderViewModel> SearchResult { get; set; }
        public List<POHeaderViewModel> PODeatil { get; set; }
        public PRAppHistoryViewModel LastPRApproval { get; set; }
        public List<VM_VW_PRUSERDASHBOARD> PRUSERDASHBOARD { get; set; }
        public int ISPUR { get; set; }

        //----------SR52365---------------------
        [DisplayName("KI Code")]
        public string kicode { get; set; }
        public List<VM_HMSIHOLIDAYS> BYE_Cal { get; set; }
        public List<VM_VW_PRUSERDASHBOARD> BYE_History { get; set; }
        //-----------------------
        [DisplayName("Ariba RFP ID")]
        public string ARIBARFPID { get; set; }
        [DisplayName("Ariba Doc Id applicable")]
        public Nullable<short> ARIBASTATUS { get; set; }
        [DisplayName("Ariba Buyer (Purchase)")]
        public long? ARIBABuyer_ECODE { get; set; }


        //Added by aumento as on 19092024 for the SR71870=========
        public long IsTeamMember { get; set; }
        public long ISSpecialRight { get; set; }

        [DisplayName("KI")]
        public long KIID { get; set; }

        [DisplayName("Approval Type")]
        public long IOMCATMSTID { get; set; }

        [DisplayName("Status")]
        public short Status { get; set; }
        public List<VM_VW_PRUSERDASHBOARD> PRDASHBOARDEPORT { get; set; }
        //=======================================================
    }

    public class PRBuyerMapViewModel
    {
        public long PRBUYERID { get; set; }
        public long PRHEADERID { get; set; }

        [DisplayName("Buyer")]
        public long BUYER_ECODE { get; set; }
        [DisplayName("Buyer")]
        public string BUYER_NAME { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayName("Assign Date")]
        public System.DateTime DATEADDED { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDDATE { get; set; }

        public long BUYER_SITEID { get; set; }

    }

    public class PRBuyerMstViewModel
    {
        public long PRBUYERMSTID { get; set; }
        public long ADORGLEVELID { get; set; }
        public string LEVELDESCRIP { get; set; }
        public long PRCAT { get; set; }
        public string CATNAME { get; set; }
        public long ADEMPCODE { get; set; }
        public string ADEMPNAME { get; set; }
        public long SYKIID { get; set; }
        public short ACTIVE { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
    }

    public partial class PR_DGIT_PRADDAPP_MST
    {
        public long PRADDAPP_MST_ID { get; set; }
        public long ADORGLVLID { get; set; }
        public long APPROVER { get; set; }
        public Nullable<long> FUNCTIONDESID { get; set; }
        public Nullable<long> ACTUALDESGID { get; set; }
        public short ISPRINTREQUIRED { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDDATE { get; set; }
        public short STATUS { get; set; }
        public short ISPARALELLAPP { get; set; }
        public Nullable<long> AMOUNTRANGE_FROM { get; set; }
        public Nullable<long> AMOUNTRANGE_TO { get; set; }
        public Nullable<short> INDENTTYPE { get; set; }
        public Nullable<short> ISADDEDINLAST { get; set; }
        public Nullable<short> APPSEQ { get; set; }
        public string ACTIONFOR { get; set; }
        public Nullable<short> IS_SISPR { get; set; } //SIS PR Change
        public string APP_TYPEINFO { get; set; } //SIS PR Change
        public Nullable<long> PLANTID { get; set; } //Added by Aumento::SR68003 
    }
    public partial class VM_VW_PRUSERDASHBOARD
    {
        public string ADDEDBYNAME { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public decimal INDENT_AMOUNT { get; set; }
        public string INDENT_NO { get; set; }
        public long PRHEADERID { get; set; }
        public string ITEM_DETAIL { get; set; }
        public Nullable<long> OPERATIONID { get; set; }
        public Nullable<long> DIVISIONID { get; set; }
        public Nullable<long> DEPARTMENTID { get; set; }
        public Nullable<long> SECTIONID { get; set; }
        public long ADDEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public Nullable<System.DateTime> PRAPPDATE { get; set; }
        public string POAPPROVALSTATUS { get; set; }
        public string PONO { get; set; }
        public Nullable<short> POSTATUS { get; set; }
        public string BUYERNAME { get; set; }
        public string BUYERSTATUS { get; set; }
        public Nullable<System.DateTime> POAPPDATE { get; set; }
        public string DEPARTMENT { get; set; }
        public string DIVISION { get; set; }
        public string OPERATION { get; set; }
        public string SECTION { get; set; }
        public Nullable<long> BUYSITEID { get; set; }
        [DisplayName("Ariba RFP ID")]
        public string ARIBARFPID { get; set; }
        [DisplayName("Ariba Doc Id applicable")]
        public Nullable<short> ARIBASTATUS { get; set; }
        [DisplayName("Ariba Buyer (Purchase)")]
        public long? ARIBABuyer_ECODE { get; set; }

        //Added by aumento for the SR73841============
        [DisplayName("Ariba SR No")]
        public string ARIBASRNO { get; set; }
        //============================================


    }

    public class PRBuyerMstViewModelJson
    {
        public int PRBUYERMSTID { get; set; }
        public int ADORGLEVELID { get; set; }
        public int PRCAT { get; set; }
        public int ADEMPCODE { get; set; }
        public string ADEMPNAME { get; set; }
        public int SYKIID { get; set; }
        public short ACTIVE { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public int ADDEDBY { get; set; }
    }


    public class PR_dgit_properationmap
    {
        public long PROPMAPID { get; set; }
        public short PRCAT { get; set; }
        public string CAtDESC { get; set; }
        public long ADEMPCODE { get; set; }
        public string EMPNAME { get; set; }
        public long ADORGLVLID { get; set; }
        public string ORGDESC { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public Nullable<long> ADDEDBY { get; set; }
        public short STATUS { get; set; }
    }

    public class PR_dgit_properationmapJson
    {
        public long PROPMAPID { get; set; }
        public short PRCAT { get; set; }
        public long ADEMPCODE { get; set; }
        public long ADORGLVLID { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public Nullable<long> ADDEDBY { get; set; }
        public short STATUS { get; set; }
        public string LEVELDESCRIP { get; set; }
    }
    public partial class AD_orglevel_type
    {
        public long ADORGLEVELTYPEID { get; set; }
        public string LEVELTYPE { get; set; }
    }
    public partial class AD_orglevel
    {
        public long ADORGLEVELID { get; set; }
        public string LEVELDESCRIP { get; set; }
    }

    public partial class DGIT_PRADDAPP
    {
        public Nullable<long> PRADDAPP_MST_ID { get; set; }
        public Nullable<long> ADORGLVLID { get; set; }
        public Nullable<long> APPROVER { get; set; }
        public Nullable<long> FUNCTIONDESID { get; set; }
        public Nullable<long> ACTUALDESGID { get; set; }
        public Nullable<long> AMOUNTRANGE_FROM { get; set; }
        public Nullable<long> AMOUNTRANGE_TO { get; set; }
        public string INDENTTYPE { get; set; }
        public string ISADDEDINLAST { get; set; }
        public Nullable<short> APPSEQ { get; set; }
        public string ACTIONFOR { get; set; }
        public Nullable<short> ISPRINTREQUIRED { get; set; }
        public string STATUS { get; set; }

        public string IS_SISPR { get; set; }
        public string ISPARALELLAPP { get; set; }

        public string APP_TYPEINFO { get; set; }
        public Nullable<long> PLANTID { get; set; } //Added by Aumento::SR68003 
    }

    public class VM_HMSIHOLIDAYS
    {
        public long HMSIHOLIDAYSID { get; set; }
        public System.DateTime MONTHDATEYEAR { get; set; }
        public string HOLIDAYDESCRIPTION { get; set; }
        public long SYSITEID { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public Nullable<System.DateTime> DATELSTMOD { get; set; }
        public long ADDEDBY { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public short ACTIVE { get; set; }
        public Nullable<short> ISHALFDAY { get; set; }
    }
    public class CategoryChangeRequest
    {
        public long Id { get; set; }
        public short SelectedCategory { get; set; }
    }

    public class AllocatorMasterRequest
    {
        public string? JsonData { get; set; }
        public int PRCAT { get; set; }
        public int ADEMPCODE { get; set; }
    }

    public class BuyerMasterRequest
    {
        public string? JsonData { get; set; }
        public int PRCAT { get; set; }
        public int ADORGLEVELID { get; set; }
    }

    public class Add_PRAppRequest
    {
        public string? JsonData { get; set; }
        public int ADORGLVLID { get; set; }
    }
    public class SLACategoryChangeRequest
    {
        public long Id { get; set; }
        public short SelectedSLACategory { get; set; }
    }

}
