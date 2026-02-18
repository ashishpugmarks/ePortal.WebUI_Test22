using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class RiskAssessmentViewModel
    {
       
    }
    public class RiskApproval
    {
        public long EmpCode { get; set; }
        public string ISSCMember { get; set; }
        public string Process { get; set; }
        public string DivisionName { get; set; }
        public int Count { get; set; }
        public string Operation { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? DHApprovedDate { get; set; }
        public string OPHeadStatus { get; set; }
        public DateTime? OPHApprovedDate { get; set; }
        public DateTime? ISSCMSubmittedDate { get; set; }
        public string DHStatus { get; set; }
        public string Url { get; set; }
    }


    public class RiskApprovalOP
    {
        public long EmpCode { get; set; }
        public string ISSCMember { get; set; }
        public string Process { get; set; }
        public string DivisionName { get; set; }
        public int Count { get; set; }
        public string Operation { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? DHApprovedDate { get; set; }
        public decimal? OPHeadStatus { get; set; }
        public DateTime? OPHApprovedDate { get; set; }
        public DateTime? ISSCMSubmittedDate { get; set; }
        public decimal? DHStatus { get; set; }
    }

    public class RiskMailerReminderVM
    {
        public int SNo { get; set; }
        public string SYKI { get; set; }
        public decimal SYKIID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReminerDate { get; set; }
        public long? ReminderId { get; set; }
        public long? ADDEDBY { get; set; }

        public decimal ReminderFor { get; set; }

    }





    public class RiskAssessmentSYKIList
    {
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public decimal ACTIVE { get; set; }
        public string DIVISIONNAMEN { get; set; }
        public decimal ADEMPCODE { get; set; }
        public string OperationName { get; set; }
    }

    public class RiskSelfPending
    {
        public string Process { get; set; }
        public int Count { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? ISSCMSubmittedDate { get; set; }
        public decimal? DHStatus { get; set; }
        public DateTime? DHApprovedDate { get; set; }
        public decimal? OPHeadStatus { get; set; }
        public string DivisionName { get; set; }
        public DateTime? OPHApprovedDate { get; set; }


    }

    public class RiskAssessmentSearchModel
    {
        public long? Id { get; set; }
        public long? ISSCMemberNominationID { get; set; }
        public long ADEMPCODE { get; set; }

        public string SYKI { get; set; }
        public decimal? SYKIID { get; set; }
        public string OPERATIONID { get; set; }
        public string OPERATIONNAME { get; set; }
        public decimal DIVHDHDID { get; set; }
        public string DIVISIONNAMEN { get; set; }
        public long ADDEDBY { get; set; }
        public string EmpName { get; set; }

        public DateTime NomiationDate { get; set; }

        public string ISSC_Name { get; set; }
        public string Div_Head { get; set; }
        public string Opr_Head { get; set; }
        public decimal? ISSC_EMPCODE { get; set; }
        public decimal? Div_EMPCODE { get; set; }
        public decimal? Op_EMPCODE { get; set; }
    }
    public class RiskAssessmentSYKIViewModel
    {
        public List<RiskAssessmentSYKIList> _SYKIList { get; set; }
        public RiskAssessmentSYKIViewModel()
        {
            _SYKIList = new List<RiskAssessmentSYKIList>();
        }
    }

    public class RiskAssessmentPeriodForITGRCMMemberNominationListVM
    {
        public decimal? ID { get; set; }
        public int? SNo { get; set; }

        public string SYKI { get; set; }
        public decimal? SYKIID { get; set; }
        public long? OPERATIONID { get; set; }
        public long? DIVISIONID { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? Active { get; set; }

        public string EmpName { get; set; }

        public string OperationName { get; set; }

        public string DivisionName { get; set; }

        public int? ActionType { get; set; }
        public string SelectedIds { get; set; }

    }

    public class RiskAssessmentForITGRCMemberNominationVM
    {
        public decimal ID { get; set; }
        public int SNo { get; set; }
        public decimal SYKIID { get; set; }
        public long OPERATIONID { get; set; }
        public long DIVISIONID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdationDate { get; set; }
        public int Active { get; set; }

        public int? Status { get; set; }
        public string Msg { get; set; }


    }

    public class BulkUpdateRiskPeriodForITGRCMMemberNominationVM
    {
        public RiskAssessmentPeriodForITGRCMMemberNominationListVM PeriodForITGRC { get; set; }
        public List<RiskAssessmentPeriodForITGRCMMemberNominationListVM> lstPeriodForITGRC { get; set; }
    }

    public class CommonRiskAssessmentVM
    {
        public decimal ID { get; set; }
        public long SKYID { get; set; }
        public string SYKIName { get; set; }
        public string PrimaryRisk { get; set; }
        public string RiskCategory { get; set; }
        public string PotentialRisk { get; set; }
        public string RiskLeadToBreach { get; set; }
        public string RiskOwner { get; set; }
        public string Iden_Frequency { get; set; }
        public string Iden_Impact { get; set; }
        public string Impact_Type { get; set; }
        public string Iden_Current_Risk_Level { get; set; }
        public string CM_To_Pre_Potential_Risk { get; set; }
        public string Current_Measures_Level { get; set; }
        public string Frequency_RA { get; set; }
        public string Impact_RA { get; set; }
        public string Current_Risk_Level_RA { get; set; }
        public string Treatement_For_Risk_Level { get; set; }
        public string ActionItem { get; set; }
        public DateTime? TargetDate { get; set; }
        public string Responsebiity { get; set; }
        public int? Status { get; set; }
        public int? RiskStatus { get; set; }
        public string Frequency_RRA { get; set; }
        public string Impact_RRA { get; set; }
        public string Current_Risk_Level_RRA { get; set; }
        public int? ISSC_MEM_Status { get; set; }
        public long CreatedBy { get; set; }
        public long? ISSC_MEM_EmpCode { get; set; }
        public long? Division_Head_EmpCode { get; set; }
        public long? Operating_Head_EmpCode { get; set; }
        public int? OP_Head_Status { get; set; }
        public int? Div_Head_Status { get; set; }
        public string OP_Head_Remarks { get; set; }
        public string Div_Head_Remark { get; set; }
        public string Other_Potential_Risk { get; set; }
        public int? ApproveStatus { get; set; }
        public string OperatingHeadName { get; set; }
        public string DivisionHeadName { get; set; }
        public string DivisionName { get; set; }
    }

    public class DownloadReportVM
    {
        public string PrimaryRisk { get; set; }
        public string RiskCategory { get; set; }
        public string PotentialRisk { get; set; }
        public string RiskLeadToBreach { get; set; }
        public string RiskOwner { get; set; }
        public string CM_To_Pre_Potential_Risk { get; set; }
        public string Current_Measures_Level { get; set; }
        public string Iden_Frequency { get; set; }
        public string Impact_Type { get; set; }
        public string Iden_Impact { get; set; }
        public string Iden_Current_Risk_Level { get; set; }
        public string Frequency_RA { get; set; }
        public string Impact_RA { get; set; }
        public string Current_Risk_Level_RA { get; set; }
        public string Treatement_For_Risk_Level { get; set; }
        public string ActionItem { get; set; }
        public string TargetDate { get; set; }
        public string Responsebiity { get; set; }
        public string Status { get; set; }
        public string Frequency_RRA { get; set; }
        public string Impact_RRA { get; set; }
        public string Current_Risk_Level_RRA { get; set; }
    }

    public class Primary_RiskDeatils_VM
    {
        public string Id { get; set; }
        public string PrimaryAsset { get; set; }
    }

    public class Risk_Approver_Remarks_ISSCM
    {
        public string Divison_Remarks { get; set; }
        public DateTime? DivisionDate { get; set; }
        public string Operating_Remarks { get; set; }
        public DateTime? OperatingDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class Risk_Deficency_Remarks
    {
        public string PrimaryAsset { get; set; }
        public string RiskCategory { get; set; }
        public string Remarks { get; set; }
        public DateTime? cdate { get; set; }
    }

    public class RiskFrequencyVM
    {
        public int FrequencyLevelId { get; set; }
        public string DegreeOfFrequency { get; set; }
    }

    public class DivisionWise_Risk_Count
    {
        public string DivisionName { get; set; }
        public decimal TSCount { get; set; }
        public decimal TACount { get; set; }
        public decimal TBCount { get; set; }
        public decimal TCCount { get; set; }
        public decimal TDCount { get; set; }
        public string Current_Degree_RA { get; set; }
        public decimal Id { get; set; }
    }

    public class RiskFrequencyImapectMapVM
    {
        public int Id { get; set; }
        public string Result { get; set; }
    }

    public class RiskTreatmentLevelVM
    {
        public int TreatmentId { get; set; }
        public string Treatment { get; set; }
    }

    public class RiskCategoryVM
    {
        public int RiskId { get; set; }
        public string Category { get; set; }
    }

    public class RiskCategoryCreVM
    {
        public int RiskId { get; set; }
        [Required(ErrorMessage ="Please enter risk category.")]
        public string Category { get; set; }
        public string? Status { get; set; }
        //public int IsActive { get; set; }
    }

    public class RiskStatementCreVM
    {
        [Required(ErrorMessage = "Please select risk category.")]
        public int RiskId { get; set; }
        public int? StatementId { get; set; }
        [Required(ErrorMessage = "Please enter potential risk.")]
        public string Statement { get; set; }
        
    }

    public class RiskStatementLstVM
    {
        public string RiskCategory { get; set; }
        public int StatementId { get; set; }
        public string Statement { get; set; }
        public string Status { get; set; }
    }


    public class RiskImpactVM
    {
        public int RiskLevelid { get; set; }
        public string DegreeOfImpact { get; set; }
    }

    public class RiskStatementVM
    {
        public int StatementId { get; set; }
        public string RiskStatement { get; set; }
    }

    public class Risk_Current_Meaasure_LevelVM
    {
        public int CurrentMeasureLevelId { get; set; }
        public string RiskLevel { get; set; }
    }

    public class Degree_LevelVM
    {
        public int DegreeId { get; set; }
        public string Degree { get; set; }
        public int LevelId { get; set; }
    }

    public class PotentialRiskVM
    {
        public int PotentialId { get; set; }
        public string PotentialRisk { get; set; }
    }

    public class RiskLeadVM
    {
        public int RiskLeadId { get; set; }
        public string RiskLead { get; set; }
    }

    public class PrimaryAssestsVM
    {
        public int AssestId { get; set; }
        public string Assets { get; set; }
    }

    public class ImpactTypeVM
    {
        public int ImpactTypeId { get; set; }
        public string ImpactType { get; set; }
    }

    public class RiskAssessmentVM
    {
        public decimal? Id { get; set; }
        public decimal SkyId { get; set; }
        [Required(ErrorMessage = "Please select primary risk.")]
        public int PrimaryAssetId { get; set; }
        [Required(ErrorMessage = "Please select risk category.")]
        public int Risk_CategoryId { get; set; }
        [Required(ErrorMessage = "Please select impact type.")]
        public int RiskImactTypeId { get; set; }
        [Required(ErrorMessage = "Please select potential risk.")]
        public int? Potential_RiskId { get; set; }
        [Required(ErrorMessage = "Please select Risk lead to breach of (C/I/A).")]
        public int Risk_LeadId { get; set; }
        public string Risk_Owner { get; set; }
        [Required(ErrorMessage = "Please select frequency.")]
        public int Ident_Frequency { get; set; }
        [Required(ErrorMessage = "Please select impact.")]
        public int Ident_Imapct { get; set; }
        [Required(ErrorMessage = "Please select degree risk level.")]
        public int Ident_Current_Degree_Risk_Lebel { get; set; }
        [Required(ErrorMessage = "Please enter current measure to prevent the potential risk.")]
        public string Current_Measures_To_Prevent_Potential_Risk { get; set; }
        [Required(ErrorMessage = "Please select current measure level.")]
        public int Current_Measures_Level { get; set; }
        [Required(ErrorMessage = "Please select frequency.")]
        public int Frequency { get; set; }
        [Required(ErrorMessage = "Please select impact.")]
        public int Imapct { get; set; }
        [Required(ErrorMessage = "Please select degree risk level.")]
        public int Current_Degree_Risk_Lebel { get; set; }
        [Required(ErrorMessage = "Please select risk treatment level.")]
        public int Risk_Treatment_for_Risk_Level { get; set; }
        [Required(ErrorMessage = "Please enter action item.")]
        public string Action_Item { get; set; }
        [Required(ErrorMessage = "Please enter responsbility.")]
        public string Responsbility { get; set; }
        [Required(ErrorMessage = "Please select target date.")]
        public string Target_Date { get; set; }
        public int? Risk_Status { get; set; }
        public int? Status { get; set; }
        [Required(ErrorMessage = "Please select frequency.")]
        public int Frequency_RRA { get; set; }
        [Required(ErrorMessage = "Please select impact.")]
        public int Imapct_RRA { get; set; }
        [Required(ErrorMessage = "Please select degree risk level.")]
        public int Risk_Degree_Level_RRA { get; set; }
        public int? ACTIVE { get; set; }
        public int? Issc_Member_Status { get; set; }
        public long? Division_Head_Id { get; set; }
        public int? Division_Status { get; set; }
        public long? Operating_Head_Id { get; set; }
        public int? Operating_Status { get; set; }
        public DateTime? Final_Submit_Date_ISSCMEMBER { get; set; }
        public DateTime? Final_Submit_Date_DIVISIONHEAD { get; set; }
        public DateTime? Final_Submit_Date_OPERATINGHEAD { get; set; }
        public string Review_Remark_DIVISIONHEAD { get; set; }
        public string Review_Remark_OPERATINGHEAD { get; set; }
        public int? Aprrove_Status { get; set; }
        public DateTime CreationDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdationDate { get; set; }
        public long UpdatedBy { get; set; }
        [Required(ErrorMessage = "Please enter  potential risk.")]
        public string OtherPotential { get; set; }
    }

    public class Risk_PrimaryAsset
    {
        public decimal PrimaryAssetId { get; set; }
        public string PrimaryAssetText { get; set; }
    }

    public class Risk_Approver_Remarks
    {
        public string Divison_Remarks { get; set; }
        public DateTime? DivisionDate { get; set; }
        public string Operating_Remarks { get; set; }
        public DateTime? OperatingDate { get; set; }
    }
    
    public class Risk_DifiencyReport
    {
        [Required(ErrorMessage = "Please select Operation")]
        public long operationId { get; set; }
        [Required(ErrorMessage = "Please select Division")]
        public long DivisionId { get; set; }
        [Required(ErrorMessage = "Please select Primary Risk")]
        public decimal PrimaryAssetId { get; set; }
        [Required(ErrorMessage = "Please select Risk Category")]
        public decimal RiskCategoeyId { get; set; }
        [Required(ErrorMessage = "Please enter Feedback")]

        public string FeedBack { get; set; }
        public long? createdby { get; set; }

    }

    public class RiskAssessment_Start_EndDate_ISSC_Member
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class BulkUpdateEmailVM
    {
        public string Emailid { get; set; }
        public string Empname { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }

    public class Get_RiskDivision_ISSC_Member
    {
        public decimal DivId { get; set; }
        public string DivisionName { get; set; }
        public decimal ISSCMember_EmpCode { get; set; }
        public string ISSCMember { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? ISSC_Member_Submit_Status { get; set; }
        public DateTime? ISSC_Member_Submit_Date { get; set; }
        public decimal? DivisionHead_Submit_Status { get; set; }
        public DateTime? DivisionHead_Submit_Date { get; set; }
        public decimal? OperatingHead_Submit_Status { get; set; }
        public DateTime? OperatingHead__Submit_Date { get; set; }
        public string ApplicationEnd_Date { get; set; }
        public string SYKI { get; set; }
        public string ProjectTitle { get; set; }
        public string Url { get; set; }

        public string DivisionHeadSubmitStatus { get; set; }
        public string OperatingHeadSubmitStatus { get; set; }

        public string DivisionHead_Name { get; set; }
        public string OperatingHead_Name { get; set; }
        public string OperationName { get; set; }
    }

    public class Risk_DivisionHead_Details
    {
        public string Emailid { get; set; }
        public string Empname { get; set; }
        public string DivisionName { get; set; }
        public long DivisionEmpCode { get; set; }
        public string ISSC_Member_Name { get; set; }
        public string ISSC_MemberEmail { get; set; }
        public int? ISSC_Member_Submit_Status { get; set; }
        public DateTime? ISSC_Member_Submit_Date { get; set; }
        public int? DivisionHead_Submit_Status { get; set; }
        public DateTime? DivisionHead_Submit_Date { get; set; }
        public int? OperatingHead_Submit_Status { get; set; }
        public DateTime? OperatingHead__Submit_Date { get; set; }
    }

    public class RiskAssessmentSYKIListForOH
    {
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public decimal ACTIVE { get; set; }
        public string DIVISIONNAMEN { get; set; }
        public decimal ADEMPCODE { get; set; }
        public string OperationName { get; set; }
        public long? OPID { get; set; }

    }

    public class RiskRegistrationSYKIList
    {
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public decimal ACTIVE { get; set; }
        public string DIVISIONNAMEN { get; set; }
        public decimal ADEMPCODE { get; set; }
        public string OperationName { get; set; }

    }
    public class RiskAssessmentSYKIViewModelOH
    {
        public List<RiskAssessmentSYKIListForOH> _SYKIList { get; set; }
        public RiskAssessmentSYKIViewModelOH()
        {
            _SYKIList = new List<RiskAssessmentSYKIListForOH>();
        }
    }





    public class Risk_ISSCMember_details
    {
        public string EmpName { get; set; }

        public string EmpEmail { get; set; }

        public long EmpCode { get; set; }
    }
}
