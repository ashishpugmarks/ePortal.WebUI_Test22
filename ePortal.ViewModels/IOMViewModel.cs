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
    public class IOMHeaderViewModel
    {
        public long IOMHEADERID { get; set; }

        [Required]
        [DisplayName("Description")]
        public string IOMDesc { get; set; }

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

        [DisplayName("Approval Note")]
        public string IOMATTACHMENT { get; set; }
        public IFormFile IOMFile { get; set; }

        public List<IOMDetailViewModel> iomDetail { get; set; }
        public List<IOMAppAuthSeqViewModel> iomAuthSeq { get; set; }
        public List<IOMAppHistoryViewModel> iomAppHis { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
        public short IsFinalSubmit { get; set; }
        public List<IOMAppHeaderViewModel> iomAppHeaderList { get; set; }
        public string ISENABLE { get; set; }
        public Int16 APP_TYPE { get; set; }
        [DisplayName("Category")]
        public long? IOMCATMSTID { get; set; }
        public string IOMCATDESC { get; set; }
        public Int16 ishighlighted { get; set; }

        //Added by TTL on 18th July 2025 against SR101913 > CR6738 - Start
        [DisplayName("Ariba RFP Doc ID"), MaxLength(20, ErrorMessage = "Length must be within 20 chars.")]
        public string ARIBARFPID { get; set; }
        //Added by TTL on 18th July 2025 against SR101913 > CR6738 - End
        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - Start
        [DisplayName("Indent Nos.")]
        public List<string>? IndentNos { get; set; }
        public List<PRDetailViewModel>? PRDocDetails { get; set; }
        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - End
    }
    public class IOMDetailViewModel
    {
        [DisplayName("SNo")]
        public long IOMDTL_ID { get; set; }
        public long IOMHEADERID { get; set; }

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
        public string IOMNo { get; set; }
        public string IOMDESC { get; set; }
    }
    public class IOMAppAuthSeqViewModel
    {
        [DisplayName("SNo")]
        public long IOMAPPAUTH_ID { get; set; }
        public long IOMID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee Name")]
        public string ADEMPNAME { get; set; }

        [DisplayName("Designation")]
        public string ADDESIGNATION { get; set; }

        [DisplayName("Header")]
        public string Header { get; set; }

        [DisplayName("Seq")]
        public short APP_SEQ { get; set; }
        public short STATUS { get; set; }
        public short APPTYPE { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public short FNDESID { get; set; }
    }
    public class IOMAppHistoryViewModel
    {
        [DisplayName("SNo")]
        public long IOMAPPHISTORY_ID { get; set; }
        public long IOMID { get; set; }

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
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}"), DisplayName("Approval Date")]
        public Nullable<System.DateTime> APPROVALDATE { get; set; }
        public string IOM_ATTACHMENT_NAME { get; set; }
        public short APPTYPE { get; set; }
    }
    public partial class IOMAppHeaderViewModel
    {
        [DisplayName("SNo")]
        public long IOMAPPHEADERID { get; set; }
        public long IOMHEADERID { get; set; }

        [DisplayName("Header")]
        public string IOMAPPHEADER { get; set; }

        [DisplayName("Description")]
        public string APPHEADERDESC { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        public int Seq_Order { get; set; }
        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
    }
    public class IOMSIGLIST
    {
        public string Designation { get; set; }
        public List<IOMAppHistoryViewModel> appList { get; set; }
    }
    public class VM_DGIT_IOMCATMST
    {
        public long IOMCATMSTID { get; set; }
        public string CATDESC { get; set; }
        public short STATUS { get; set; }
        public short ISHIGHLIGHTED { get; set; }

    }
    public class SearchIOM
    {
        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - Start
        public SearchIOM()
        {
            HasDivisionListData = 0;
            HasDeptListData = 0;
            HasSecListData = 0;
        }
        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - End

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
        public List<VM_VW_DGIT_IOMREPORT> SearchResult { get; set; }
        [DisplayName("Approval Type")]
        public long IOMCATMSTID { get; set; }
        public long ISSpecialRight { get; set; }

        public long loginid { get; set; }
        [DisplayName("KI")]
        public long KIID { get; set; }

        public long IsTeamMember { get; set; }
        public string ADDEDBYNAME { get; set; }

        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - Start
        public string UPDATEDBYNAME { get; set; }
        public long HasSecListData { get; set; }
        public short[] FilterStatus { get; set; }
        public long HasDivisionListData { get; set; }
        public long HasDeptListData { get; set; }
        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - End

        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
        [DisplayName("Pending At")]
        public List<DropdownList> PendingAt { get; set; }
        public long[] PendingAtUsers { get; set; }
        public string PendingWithType { get; set; }
        public long rangeId { get; set; }
        public long deptLavelId { get; set; }
        public string designation { get; set; }
        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End
    }
    public class VM_ADORGLEVEL
    {
        public long ADORGLEVELID { get; set; }
        public string LEVELDESCRIP { get; set; }
        public Nullable<long> PARENTLEVELID { get; set; }
        public long ADORGLEVELTYPEID { get; set; }
        public Nullable<byte> SYKIID { get; set; }
        public Nullable<short> ACTIVE { get; set; }
        public Nullable<System.DateTime> DATEADDED { get; set; }
        public Nullable<long> ADDEDBY { get; set; }
        public Nullable<System.DateTime> LSTMODDATE { get; set; }
        public Nullable<long> LSTMODBY { get; set; }
        public Nullable<decimal> ADORGLEVELSAPID_old { get; set; }
        public string COSTCENTRE { get; set; }
        public Nullable<decimal> ADORGLEVELSAPID { get; set; }

    }
    public class VM_VW_DGIT_IOMREPORT
    {
        public long IOMHEADERID { get; set; }
        public string IOM_DESC { get; set; }
        public short STATUS { get; set; }
        public short PROCESS_STATUS { get; set; }
        public System.DateTime? DATEADDED { get; set; } // Changed by TTL on 10-July-2025 against SR93758 > CR5975
        public long ADDEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public short APP_TYPE { get; set; }
        public Nullable<long> IOMCATID { get; set; }
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public Nullable<long> OPERATIONID { get; set; }
        public string OPERATION { get; set; }
        public Nullable<long> DIVISIONID { get; set; }
        public string DIVISION { get; set; }
        public Nullable<long> DEPARTMENTID { get; set; }
        public string DEPARTMENT { get; set; }
        public Nullable<long> SECTIONID { get; set; }
        public string SECTION { get; set; }
        public string CATDESC { get; set; }
        public string ADDEDBYNAME { get; set; }

        // Added by TTL on 10-July-2025 against SR93758 > CR5975 - Start
        public System.DateTime? LEADSTARTDATE { get; set; }
        public System.DateTime? LEADENDDATE { get; set; }
        public long SYSITEID { get; set; }
        public decimal? PENDINGWITH_ECODE { get; set; }
        public decimal? LAST_MODIFIED_BY_ECODE { get; set; }
        public DateTime? LAST_MODIFIED_ON { get; set; }
        public DateTime? PENDING_SINCE { get; set; }
        public decimal HOLIDAYSINCELEADTIME { get; set; }
        public decimal HOLIDAYSINCEPENDING { get; set; }
        public decimal CYCLETIME { get; set; }
        public decimal PENDINGTIME { get; set; }
        public string PENDINGWITH_NAME { get; set; }
        public string LASTMODIFIED_NAME { get; set; }
        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
        public string EncryptedViewDetailUrl { get; set; }
        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End

        // Added by TTL on 10-July-2025 against SR93758 > CR5975 - End
    }
    // Added by TTL on 10-July-2025 against SR93758 > CR5975 - Start
    #region IOMDashboardChart
    public class IOMDashboardGraphViewModel
    {
        public IOMDashboardGraphViewModel()
        {
            Data = new List<IOMGraphDataModel>();
        }
        public IOMGraphGroupModel Group { get; set; }
        public List<IOMGraphDataModel> Data { get; set; }
    }
    public class IOMGraphDataModel
    {
        public IOMGraphDataModel()
        {
            RecordsByRange = new List<IOMDashboardGraphRecordCountByRange>();
        }
        public IOMGraphStatusModel Status { get; set; }
        public List<IOMDashboardGraphRecordCountByRange> RecordsByRange { get; set; }
    }

    public class IOMGraphGroupModel
    {
        public long? id { get; set; }
        public string DepartmentLabel { get; set; }
        public int count { get; set; }
    }
    public class IOMGraphStatusModel
    {
        public long? id { get; set; }
        public string desc { get; set; }
        public int count { get; set; }
    }

    public class IOMDashboardGraphRecordCountByRange
    {
        public string Range { get; set; }
        public int RecordCount { get; set; }
    }

    public class IOMDashboadReportList
    {
        public long IOMHEADERID { get; set; }
        public short STATUS { get; set; }
        public short PROCESS_STATUS { get; set; }
        public System.DateTime? DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public Nullable<long> IOMCATID { get; set; }
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public Nullable<long> OPERATIONID { get; set; }
        public string OPERATION { get; set; }
        public Nullable<long> DIVISIONID { get; set; }
        public string DIVISION { get; set; }
        public Nullable<long> DEPARTMENTID { get; set; }
        public string DEPARTMENT { get; set; }
        public Nullable<long> SECTIONID { get; set; }
        public string SECTION { get; set; }
        public string CATDESC { get; set; }
        public string ADDEDBYNAME { get; set; }

        public decimal CYCLETIME { get; set; }
        public string PendingAt { get; set; }
        public Nullable<System.DateTime> LASTACTIONDATE { get; set; }
        public decimal? LASTACTIONTAKENBY { get; set; }
        public string LASTACTIONTAKENBYNAME { get; set; }
        public decimal? PendingAtEmpCode { get; set; }
        public string OrgLevel { get; set; }
        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
        public string EncryptedViewDetailUrl { get; set; }
        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End
    }
    public class GroupedIOMByDesignation
    {
        public string Designation { get; set; }
        public int CountOfIOMs { get; set; }
        public List<IOMDashboardGraphRecordCountByRange> RecordsByRange { get; set; }
    }

    public class SearchIOMDashboadReportList
    {
        public SearchIOMDashboadReportList()
        {
            SearchResult = new List<IOMDashboadReportList>();
            AggregatedSearchResult = new List<GroupedIOMByDesignation>();
        }
        public SearchIOM searchIOM { get; set; }
        public long IOMGraphDashboardRangeId { get; set; }
        public long deptLavelId { get; set; }
        public List<IOMDashboadReportList> SearchResult { get; set; }
        public List<GroupedIOMByDesignation> AggregatedSearchResult { get; set; }

    }

    public static class IOMGraphDashboardRange
    {
        public static readonly Dictionary<int, string> RangeMap = new Dictionary<int, string>
        {
            { 1, "0-5 Days" },
            { 2, "6-10 Days" },
            { 3, "11-15 Days" },
            { 4, ">15 Days" }
        };
        public static readonly Dictionary<int, string> SubRangeMap_0_5 = new Dictionary<int, string>
        {
            { 5, "<=2 Days" },
            { 6, "3 Days" },
            { 7, ">=4 Days" }
        };
        public static readonly Dictionary<int, string> SubRangeMap_6_10 = new Dictionary<int, string>
        {
            { 8, "<=7 Days" },
            { 9, "8 Days" },
            { 10, ">=9 Days" }
        };
        public static readonly Dictionary<int, string> SubRangeMap_11_15 = new Dictionary<int, string>
        {
            { 11, "<=12 Days" },
            { 12, "13 Days" },
            { 13, ">=14 Days" }
        };
        public static readonly Dictionary<int, string> SubRangeMap_GTE_16 = new Dictionary<int, string>
        {
            { 14, ">15 Days" }
        };
        public static int? GetKeyByLabel(string label)
        {
            var match = RangeMap.FirstOrDefault(x => x.Value == label);
            return match.Equals(default(KeyValuePair<int, string>)) ? (int?)null : match.Key;
        }
    }

    public class SearchIOMsproc
    {
        public string StartDateIn { get; set; }
        public string EndDateIn { get; set; }
        public long OprnIdIn { get; set; }
        public long DivIdIn { get; set; }
        public long DeptIdIn { get; set; }
        public long SecIdIn { get; set; }
        public long AddedByIn { get; set; }
        public long CatmstIdIn { get; set; }
        public long KiidIn { get; set; }
        public string ProcStatusesIn { get; set; }
        public string ItemDetailIn { get; set; }
        public string AddedbyNameIn { get; set; }

        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
        public string PendingAtUsersIn { get; set; }
        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End
    }
    #endregion
    // Added by TTL on 10-July-2025 against SR93758 > CR5975 - End
}
