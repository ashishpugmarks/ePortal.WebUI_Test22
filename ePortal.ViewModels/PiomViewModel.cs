using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ePortal.ViewModels
{
    public class IOMPHeaderViewModel
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

        public List<IOMPDetailViewModel> iompDetail { get; set; }

        public List<IOMPAppHistoryViewModel> iompAppHis { get; set; }
        public virtual Employee_Details Emp_Detail { get; set; }
        public short IsFinalSubmit { get; set; }

        public string ISENABLE { get; set; }
        public Int16 APP_TYPE { get; set; }

        public Int16 ishighlighted { get; set; }

        // --- NEW: Hierarchy of Stages → Parallel Groups → Sequential Approvers ---
        public List<IOMStageViewModel> Stages { get; set; } = new();
        public List<IOMStageViewModel> StageHis { get; set; } = new();
        public short ISEDITABLE { get; set; }
    }

    public class IOMStageViewModel
    {
        public long? StageID { get; set; } // optional, if editing existing stage
        public long? ParallelGroupID { get; set; } // optional, if editing existing stage
        public string StageName { get; set; }
        public int StageOrder { get; set; } // sequential order of stage
        public short STATUS { get; set; } // active/inactive etc.

        public short AdditinalType { get; set; }
        // Each Stage contains multiple parallel groups
        public List<IOMParallelGroupViewModel> ParallelGroups { get; set; } = new();
        // Helper: Check if all parallel groups are fully approved
        public bool IsStageCompleted => ParallelGroups.All(pg => pg.IsGroupCompleted);

        // Optional: Track stage start/end timestamps
        public DateTime? StageStartDate { get; set; }
        public DateTime? StageEndDate { get; set; }
        public List<IOMPAppHistoryViewModel> StageHistory { get; set; } = new();
    }

    public class IOMParallelGroupViewModel
    {
        public long? ParallelGroupID { get; set; } // optional, if editing
        public string GroupName { get; set; }
        public int GroupOrder { get; set; } // sequential order inside the stage
        public short STATUS { get; set; }

        // Each Parallel Group contains sequential approvers
        public List<IOMPAppHeaderViewModel> Headers { get; set; } = new();
        public List<IOMPAppAuthSeqViewModel> Approvers { get; set; } = new();
        public bool IsGroupCompleted => Approvers.All(a => a.STATUS == 1);
    }



    public partial class IOMPAppHeaderViewModel
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
    public class IOMPAppHistoryViewModel
    {
        [DisplayName("SNo")]
        public long IOMAPPHISTORY_ID { get; set; }

        public long IOMID { get; set; }

        public long Seq_id { get; set; }

        public int SequenceNo { get; set; }

        // --- Approver Info ---
        [DisplayName("Ecode")]
        public long EmpCode { get; set; }
        [DisplayName("Employee Name")]
        public string EmpName { get; set; }
        public string EmpCodeString { get; set; }    // Optional: Employee Code string
        public string Email { get; set; }            // Optional: Employee email

        // --- Action Info ---
        [DisplayName("Status")]
        public short ApprovalStatus { get; set; }    // Approved / Rejected / Pending
        [DisplayName("Remarks")]
        public string? ApprovalRemark { get; set; }

        [DisplayName("Added By")]
        public long AddedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Added Date")]
        public DateTime AddedDate { get; set; }

        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        [DisplayName("Approval Date")]
        public DateTime? ApprovalDate { get; set; }

        public string AttachmentName { get; set; }   // Optional: Uploaded file
        public short AppType { get; set; }

        // --- Helper flags (optional for UI) ---
        public bool IsStageCompleted { get; set; }   // Stage level completion
        public bool IsGroupCompleted { get; set; }   // Parallel group completion

        public string Header { get; set; }
    }


    public class IOMPAppAuthSeqViewModel
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
    public class IOMPDetailViewModel
    {
        [DisplayName("SNo")]
        public long IOMDTL_ID { get; set; }
        public long IOMHEADERID { get; set; }

        [DisplayName("Document Type")]
        public string DOC_TYPE { get; set; }

        [DisplayName("Attachment")]
        public string FILENAME { get; set; }

        [DisplayName("Additional Info")]
        public string? ADDITIONAL_INFO { get; set; }

        public IFormFile? FILE { get; set; }
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


    public class IOMPSIGLIST
    {
        // Stage Info
        public long? StageID { get; set; }
        public string StageName { get; set; }
        public int StageOrder { get; set; }

        // Group Info
        public long? ParallelGroupID { get; set; }
        public string GroupName { get; set; }
        public int GroupOrder { get; set; }

        public string Designation { get; set; }

        // Approvers
        public List<IOMPAppHistoryViewModel> appList { get; set; } = new();
    }
}
