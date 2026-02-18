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
    public class MedicalInsuranceViewModel
    {
        [DisplayName("ID")]
        public long MedicalInsuranceID { get; set; }

        [DisplayName("Emp Code")]
        public long EmpCode { get; set; }

        [Required]
        [DisplayName("Employee Code")]
        public string SearchEmpCode { get; set; }

        //[Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Relation")]
        public string? Relation { get; set; }

        //[Required]
        [DisplayName("Gender")]
        public string Gender { get; set; }

        //[Required]
        //[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("D.O.B")]
        public string DOB { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("D.O.M")]
        public string? DOM { get; set; }

        [DisplayName("Photo")]
        public byte[]? Photo { get; set; }
        public string? SelfPhoto { get; set; }

        [DisplayName("Emp Status")]
        public short Emp_Status { get; set; }

        [DisplayName("Request Type")]
        public Int16 RequestType { get; set; }

        [DisplayName("Remarks")]
        public String? Remarks { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Created Date")]
        public string? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public short? Approval_Status { get; set; }
        public string? Approval_Date { get; set; }
        public string? Approval_By { get; set; }

        public string? PREVIOUS_EMP_NAME { get; set; }
        public string? PREVIOUS_EMPDOB { get; set; }
        //public Nullable<System.DateTime> DOM { get; set; }

        public string? lastChangeDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Modified Date")]
        public string? ModifiedDate { get; set; }

        public NomineeDetailViewModel? NomineeDetail { get; set; }
        public DependentDetailViewModel? DependentDetail { get; set; }
        public List<DependentDetailViewModel>? SAPDependentList { get; set; }
        public List<DependentDetailViewModel>? Dependents { get; set; }
        public List<DependentDetailViewModel>? DeleteDependentList { get; set; }
        public short? IsMedicalInsurance { get; set; }
        public short? IsEmpExist { get; set; }
        public short? IsJapaneseUser { get; set; }

    }

    public class NomineeDetailViewModel
    {
        [DisplayName("ID")]
        public long ID { get; set; }

        [DisplayName("Name")]
        public string NomineeName { get; set; }

        [DisplayName("Relation")]
        public string? NomineeRelation { get; set; }

        [DisplayName("Gender")]
        public string NomineeGender { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("D.O.B")]
        public string? NomineeDOB { get; set; }

        [DisplayName("Nominee Status")]
        public short? Nominee_Status { get; set; }

        public string? PREVIOUS_NOMINEE_GENDER { get; set; }
        public string? PREVIOUS_NOMINEE_DOB { get; set; }
        public string? PREVIOUS_NOMINEE_NAME { get; set; }
        public string? PREVIOUS_NOMINEE_RELATION { get; set; }
        public Int16? Status { get; set; }

        [DisplayName("Modified Date")]
        public string? ModifiedDate { get; set; }

        //[DisplayName("Policy Type")]
        //public Int16 NomineePolicyType { get; set; }
    }

    public class DependentDetailViewModel
    {
        [DisplayName("ID")]
        public long ID { get; set; }
        public long DepItemId { get; set; }
        public int Sno { get; set; }

        //[Required]
        [DisplayName("Name")]
        public string DepName { get; set; }

        [DisplayName("Relation")]
        public string? DepRelation { get; set; }

        [DisplayName("Gender")]
        public string DepGender { get; set; }

        //[Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("D.O.B")]
        public string? DepDOB { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("D.O.M")]
        public string? DepDOM { get; set; }

        //      [DisplayName("Photo")]
        //      public byte[] DepPhoto { get; set; }
        //      public string DepPhotoContype { get; set; }
        //      public IFormFile DepImgFile { get; set; }
        //      public string DepImgString { get; set; }

        public string? Doc_ContentType { get; set; }
        public string? Doc_fileString { get; set; }

        [DisplayName("Request Type")]
        public string? DepRequestType { get; set; }
        [DisplayName("Change Type")]
        public Int16? DepChangeType { get; set; }

        [DisplayName("Policy Type")]
        public string? DepPolicyType { get; set; }
        public string? DeleteDepRemarks { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Created Date")]
        public string? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }

        public string? PREVIOUS_DEP_GENDER { get; set; }
        public string? PREVIOUS_DEP_DOB { get; set; }
        public string? PREVIOUS_DEP_NAME { get; set; }
        public string? PREVIOUS_DEP_RELATION { get; set; }

        public short? Status { get; set; }

        [DisplayName("Modified Date")]
        public string? ModifiedDate { get; set; }

    }

    public class HealthCenterViewModel
    {
        [DisplayName("ID")]
        public long ID { get; set; }

        public long EmpCode { get; set; }

        [DisplayName("Employee")]
        public string EmpName { get; set; }

        [DisplayName("Operation")]
        public string Operation { get; set; }

        [DisplayName("Division")]
        public string Division { get; set; }

        [DisplayName("Department")]
        public string Department { get; set; }

        [DisplayName("Section")]
        public string Section { get; set; }

        [DisplayName("Location")]
        public string Location { get; set; }

        public Int16 Status { get; set; }

        [DisplayName("Designation")]
        public string Desg { get; set; }

        [DisplayName("Request For")]
        public Int32 RequestType { get; set; }

        [DisplayName("Application Date")]
        public string CreatedDate { get; set; }

        [DisplayName("Approval Date")]
        public string ApprovalDate { get; set; }

    }

    public class HealthCenterApprovalViewModel
    {
        [DisplayName("Plant")]
        public Int32 PlantID { get; set; }
        [DisplayName("From")]
        public string ReqDateFrom { get; set; }
        [DisplayName("To")]
        public string ReqDateTo { get; set; }
        [DisplayName("From")]
        public string ApprovalDateFrom { get; set; }
        [DisplayName("To")]
        public string ApprovalDateTo { get; set; }
        public List<HealthCenterViewModel> filterData { get; set; }
        public List<MedicalInsuranceViewModel> Emp_Detail_List { get; set; }

        [DisplayName("Status")]
        public short ApprovalStatus { get; set; }
        public string[] EmpIds { get; set; }


    }

    public class MasterReportViewModel
    {
        public Nullable<decimal> SNO { get; set; }
        public string REQUESTTYPE { get; set; }
        public Nullable<long> EMP_CODE { get; set; }
        public string ENAME { get; set; }
        public string TMOBILE { get; set; }
        public string PERNAME { get; set; }
        public string GENDER { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> DOB { get; set; }
        public string AGE { get; set; }
        public string RELATIONSHIP { get; set; }
        public string RECTYPE { get; set; }
        public string DESIGNATION { get; set; }
        public string CHANGETYPE { get; set; }
        public string PAIDTYPE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime DOJ { get; set; }

        [DisplayName("Plant")]
        public Int32 PlantID { get; set; }
        [DisplayName("From")]
        public string ReqDateFrom { get; set; }
        [DisplayName("To")]
        public string ReqDateTo { get; set; }
        [DisplayName("From")]
        public string ApprovalDateFrom { get; set; }
        [DisplayName("To")]
        public string ApprovalDateTo { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
        public List<MasterReportViewModel> MRVMList { get; set; }
    }

    public class RenewalPeriodViewModel
    {
        public long RenewalId { get; set; }

        public long KiId { get; set; }
        [DisplayName("Ki")]
        public string? Ki { get; set; }

        [DisplayName("Type")]
        public short UserType { get; set; }

        public long PlantId { get; set; }
        [DisplayName("Plant")]
        public string? Plant { get; set; }
        public long[]? Plant_Ids { get; set; }

        [DisplayName("From Date")]
        public string FromDate { get; set; }

        [DisplayName("To Date")]
        public string ToDate { get; set; }

        [DisplayName("Created By")]
        public string? CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Created Date")]
        public DateTime? CreatedDate { get; set; }

        [DisplayName("Modified By")]
        public string? ModifiedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        public Int16 Status { get; set; }
        public List<RenewalPeriodViewModel>? PeriodSettingList { get; set; }

    }

    public class EmployeePolicyLocationMapping
    {
        public long EmpPolicyLocId { get; set; }

        [DisplayName("Employee Code")]
        public long EmpId { get; set; }
        [DisplayName("Name")]
        public string Employee { get; set; }
        public string Emp_Email { get; set; }

        public long PlantId { get; set; }
        [DisplayName("Plant")]
        public string Plant { get; set; }

        //public byte[]Upload { get; set; }

        [DisplayName("Upload")]
        public IFormFile Upload { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [DisplayName("Created Date")]
        public DateTime? CreatedDate { get; set; }

        public Int16? Status { get; set; }
        public List<EmployeePolicyLocationMapping> EmployeePolicyList { get; set; }

    }
    
    public class PolicyAndPlantMappingViewModel
    {
        public long? MappingId { get; set; }

        public short PlantId { get; set; }
        [DisplayName("Plant")]
        public string? Plant { get; set; }

        public long PolicyTypeId { get; set; }
        [DisplayName("Policy Type")]
        public string? PolicyType { get; set; }
        public string? TypeDescription { get; set; }
        [DisplayName("Designation")]
        public long[] DesignationIds { get; set; }
        public short? TotalCompanyPaid { get; set; }
        public short? TotalAssociatedPaid { get; set; }
        public short CompanyPaid_Cnt { get; set; }
        public short AssociatedPaid_Cnt { get; set; }
        public String? Renewal_StartDate { get; set; }
        public String? Renewal_EndDate { get; set; }
        public short? Status { get; set; }
        public short? ApprovalStatus { get; set; }
        public long? CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime? CreatedDate { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public List<PolicyAndDesignationMappingViewModel>? PolicyPlantdesgMappingList { get; set; }
    }
    public class PolicyAndDesignationMappingViewModel
    {

        public Int16 PlantMappID { get; set; }

        [DisplayName("Plant")]
        public string PlantName { get; set; }
        [DisplayName("Policy Name")]
        public string PolicyName { get; set; }
        public long DesignationMappId { get; set; }
        public long MappingId { get; set; }

        public long DesignationId { get; set; }
        [DisplayName("Designation")]
        public string Designation { get; set; }
        public short Status { get; set; }
        public long CreatedBy { get; set; }

        public long PolicyTypeId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime? CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class PolicyMasterViewModel
    {
        public long PolicyTypeId { get; set; }
        public string TypeCode { get; set; }
        public string Description { get; set; }
        public short Status { get; set; }
        public long CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public System.DateTime CreatedDate { get; set; }
        public List<PolicyAndPlantMappingViewModel> PolicyPlantList { get; set; }
    }

    public class PolicyMappingViewModal
    {
        [DisplayName("Plant")]
        public Int32 PlantMappID { get; set; }
        public List<PolicyMasterViewModel> PolicyMaster { get; set; }
        public List<PolicyAndPlantMappingViewModel> PolicyPlantMap { get; set; }
        public List<PolicyAndDesignationMappingViewModel> PlantPolicyDesgList { get; set; }

    }

    public class HealtCenterApprovalInputParam
    {
        //{"APPROVALID":"4306","Remark":"ok","AppStatus":"1","ReqType":"2"}
        public int APPROVALID { get; set; }
        public string Remark { get; set; }
        public short AppStatus { get; set; }
        public short ReqType { get; set; }
    }
}
