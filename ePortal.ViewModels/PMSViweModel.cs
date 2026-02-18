using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
namespace ePortal.ViewModels
{
    public class PMSViweModel
    {
        public string SYKI { get; set; } = "";
        public string Operation { get; set; } = "";
        public string Division { get; set; } = "";
        public string Department { get; set; } = "";
        public string Section { get; set; } = "";

        // Dropdown lists
        public List<SelectListItem> KiList { get; set; } = new();
        public List<SelectListItem> OperationList { get; set; } = new();
        public List<SelectListItem> DivisionList { get; set; } = new();
        public List<SelectListItem> DepartmentList { get; set; } = new();
        public List<SelectListItem> SectionList { get; set; } = new();
    }
    public class PmsResultModel
    {
        public DataTable SelfTable { get; set; }
        public DataTable EvaluatorTable { get; set; }
        public DataTable ReviewerTable { get; set; }

        public PagingModel EvaluatorPaging { get; set; } = new();
        public PagingModel ReviewerPaging { get; set; } = new();
        public string SYKI { get; set; } = "";
    }
    public class PagingModel
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 15;
        public int TotalItems { get; set; } = 0;
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);
    }
    public class GoalInputModel
    {
        public string SYKI { get; set; }
        public string PMSID { get; set; }
        public string TWOWAYCONFIRM { get; set; }
    }

    public class FirstHalfInputModel
    {
        public string PMSID { get; set; }
        public string FHSTATUS { get; set; }
        public string FHCOMMENT { get; set; }
    }

    public class SecondHalfInputModel
    {
        public string PMSID { get; set; }
        public string SHSTATUS { get; set; }
        public string SHCOMMENT { get; set; }
    }

    public class EvaluatorModel
    {
        public string PMSID { get; set; }
    }
    public class GoalSettingViewModel
        {
            // Header
            public string Employee { get; set; }
            public string DateOfJoining { get; set; }
            public string Operation { get; set; }
            public string Division { get; set; }
            public string Department { get; set; }
            public string Section { get; set; }
            public string Evaluator { get; set; }
            public string Reviewer { get; set; }

            public string Role { get; set; }

            // Activities (PART A)
            public List<ActivityViewModel> Activities { get; set; } = new();
            public int ActivitiesTotalWeightage { get; set; }

            public List<ActivityViewModel> OtherActivities { get; set; } = new();
            public string OtherActivitiesWeightageText { get; set; }

            // Competencies (PART B)
            public List<CompetencyHeaderViewModel> CompetencyHeaders { get; set; } = new();

            // Two-way comments
            public string EvalComment { get; set; }
            public string RevComment { get; set; }
            public string AssocComment { get; set; }
        }

        public class ActivityViewModel
        {
            public int SNo { get; set; }
            public string Activity { get; set; }
            public string ControlItem { get; set; }
            public string Target { get; set; }
            public int Weightage { get; set; }
    }
    public class ActivitySecondHalfViewModel
    {
        public int SNo { get; set; }
        public string Activity { get; set; }
        public string ControlItem { get; set; }
        public string Target { get; set; }
        public int Weightage { get; set; }
        public string FirstHalfResult { get; set; }
        public string SecondHalfResult { get; set; }
    }

    public class CompetencyHeaderViewModel
        {
            public string HeadDescription { get; set; }
            public string HeadId { get; set; }
            public List<CompetencyLevelViewModel> Levels { get; set; } = new();
        }

        public class CompetencyLevelViewModel
        {
            public int SNo { get; set; }
            public string LevelDescription { get; set; }
            public decimal Score { get; set; } // computed score shown in UI
    }
    
    public class ViewFirstHalfViewModel
    {
        // Employee Details
        public string EmployeeName { get; set; }
        public string DateOfJoining { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Evaluator { get; set; }
        public string Reviewer { get; set; }
        public string Role { get; set; }

        // Activities
        public DataTable Activities { get; set; }
        public DataTable OtherActivities { get; set; }
        public int ActivityTotalWeight { get; set; }
        public string OtherActivityWeight { get; set; }

        // Competencies
        public DataTable CompetencyHeaders { get; set; }
        public List<CompetencyLevelVM> CompetencyData { get; set; }

        // Comments
        public string PartBASComment { get; set; }
        public string EvalFeedback { get; set; }
        public string RevFeedback { get; set; }
        public string AssocFeedback { get; set; }
    }
    public class CompetencyLevelVM
    {
        public string HeaderName { get; set; }
        public DataTable Levels { get; set; }
    }
    public class CompetencyHeaderSndHalfViewModel
    {
        public string Description { get; set; }
        public string HeaderId { get; set; }
        public List<CompetencyLevelSndHalViewModel> Levels { get; set; } = new();
    }
    public class CompetencyLevelSndHalViewModel
    {
        public int SNo { get; set; }
        public string Description { get; set; }
        public decimal FinalScore { get; set; } // computed score shown in UI
    }

    public class ViewSecondHalfFormViewModel
    {
        public string EmployeeName { get; set; }
        public string DateOfJoining { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Evaluator { get; set; }
        public string Reviewer { get; set; }
        public string Role { get; set; }

        public List<ActivitySecondHalfViewModel> Activities { get; set; } = new();
        public List<ActivitySecondHalfViewModel> OtherActivities { get; set; } = new();
        public int ActivityTotalWeight { get; set; }

        public List<CompetencyHeaderSndHalfViewModel> CompetencyHeaders { get; set; } = new();

        public string FirstHalfAssociateComment { get; set; }
        public string SecondHalfAssociateComment { get; set; }
        public string EvaluatorComment { get; set; }
        public string ReviewerComment { get; set; }
        public string AssociateComment { get; set; }
    }
    public class ManageOHApprovalViewModel
    {
        // Filters
        public string SelectedKi { get; set; }
        public string SelectedOperation { get; set; }
        public string SelectedDivision { get; set; }
        public string SelectedDepartment { get; set; }
        public string SelectedSection { get; set; }

        public List<SelectListItem> KiList { get; set; } = new();
        public List<SelectListItem> OperationList { get; set; } = new();
        public List<SelectListItem> DivisionList { get; set; } = new();
        public List<SelectListItem> DepartmentList { get; set; } = new();
        public List<SelectListItem> SectionList { get; set; } = new();

        // Grids
        public List<OHApprovalRowVM> SelfApprovals { get; set; } = new();
        public List<OHApprovalRowVM> EvalApprovals { get; set; } = new();
        public List<OHApprovalRowVM> Eval2Approvals { get; set; } = new();
        public List<OHApprovalRowVM> RevApprovals { get; set; } = new();
        public List<OHApprovalRowVM> Rev2Approvals { get; set; } = new();
        public List<OHApprovalRowVM> OtherApprovals { get; set; } = new();
    }

    public class OHApprovalRowVM
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
    }
    public class GSEvaluatorViewModel
    {
        // Associate Details
        public string Associate { get; set; }
        public string DateOfJoining { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Evaluator { get; set; }
        public string Reviewer { get; set; }
        public string Role { get; set; }

        // Flags
        public bool ShowSendBack { get; set; } = true;
        public bool ShowButtons { get; set; } = true;

        // Comment
        public string EvalComment { get; set; }

        // Tables
        public DataTable Activities { get; set; }
        public DataTable OtherActivities { get; set; }
        public DataTable CompetencyHeaders { get; set; }

        // Computed
        public int TotalActivityWeight { get; set; }
        public string OtherActivityWeight { get; set; }

        // Hidden
        public string PMSID { get; set; }
        public string AssociateEmail { get; set; }
    }
    public class FHEvaluatorViewModel
    {
        public string EncPmsId { get; set; }

        public DataTable AssociateTable { get; set; }
        public DataTable RoleTable { get; set; }
        public DataTable ActivityTable { get; set; }
        public DataTable CompetencyTable { get; set; }
        public DataTable CommentTable { get; set; }
    }
    public class TwoWayInput
    {
        public string EncPmsId { get; set; }      // UNIQUE
        public string EvalFeedback { get; set; }  // UNIQUE
        public bool ConfirmFlag { get; set; }     // UNIQUE
    }
    public class ReviewerPmsRequest
    {
        public string SYKI { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }

        public int RevPage { get; set; } = 0;
        public int PageSize { get; set; } = 15;
    }
    public class PagingInfo
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
    public class SHEvaluatorViewModel
    {
        public string HRPMSID { get; set; }

        // Associate details
        public string Associate { get; set; }
        public string DOJ { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Evaluator { get; set; }
        public string Reviewer { get; set; }

        // Role
        public string RoleDescription { get; set; }

        // Activity – Part A
        public List<ActivityVM> Activities { get; set; } = new();
        public List<ActivityVM> OtherActivities { get; set; } = new();

        public int TotalActivityWeight { get; set; }
        public int OtherActivityWeight { get; set; }

        public string PartACommentFH { get; set; }
        public string PartACommentSH { get; set; }

        // Competency – Part B
        public List<CompetencyHeaderVM> Competencies { get; set; } = new();
        public string PartBCommentSH { get; set; }

        // Two Way
        public string FeedbackComment { get; set; }
        public bool ConfirmTwoWay { get; set; }

        // Scores
        public string FHFinalScore { get; set; }
        public string SHFinalScore { get; set; }
    }

    public class ActivityVM
    {
        public int GoalId { get; set; }
        public string Activity { get; set; }
        public string ControlItem { get; set; }
        public string Target { get; set; }
        public int Weightage { get; set; }
        public int? EvalScoreSH { get; set; }
    }

    public class CompetencyHeaderVM
    {
        public int HeaderId { get; set; }
        public string Header { get; set; }
        public List<CompetencyLevelVM_SH> Levels { get; set; } = new();
    }

    public class CompetencyLevelVM_SH
    {
        public int CompetencyId { get; set; }
        public string Description { get; set; }
        public int? EvalScoreSH { get; set; }
    }
    public class SavePartARequest
    {
        public string PID { get; set; }
        public string OtherActivityScore { get; set; }
        public string PartAEvalComment { get; set; }
        public List<ActivityScore> Activities { get; set; }
    }

    public class ActivityScore
    {
        public string GoalId { get; set; }
        public string Score { get; set; }
    }
    public class SavePartBRequest
    {
        public string PID { get; set; }
        public string PartBEvalComment { get; set; }
        public string FinalEvalScore { get; set; }
        public string FinalReviewerScore { get; set; }
        public string FinalRating { get; set; }
        public string FinalSHScore { get; set; }
        public List<CompetencyScore> Competencies { get; set; }
    }

    public class CompetencyScore
    {
        public string CompetencyId { get; set; }
        public string Score { get; set; }
    }
    public class SendToReviewerRequest
    {
        public string PID { get; set; }
        public string EvalFeedbackComment { get; set; }
        public bool IsEvaluatorReviewerSame { get; set; }
    }


    public class ViewFirstHalfEvaluatorVM
    {
        // Associate Details
        public string Associate { get; set; }
        public string DateOfJoining { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Evaluator { get; set; }
        public string Reviewer { get; set; }

        // Role
        public string Role { get; set; }

        // Part A
        public List<ActivityVM_View> Activities { get; set; } = [];
        public List<ActivityVM_View> OtherActivities { get; set; } = [];
        public int ActivityWeightage { get; set; }
        public int OtherActivityWeightage { get; set; }
        public int OtherActivityEvalScore { get; set; }

        // Competencies
        public List<CompetencyHeaderVM_View> Competencies { get; set; } = [];

        // Comments
        public string PartAEvalComment { get; set; }
        public string PartBAssComment { get; set; }
        public string PartBEvalComment { get; set; }
        public string TwoWayEvalComment { get; set; }
        public string TwoWayAssocComment { get; set; }

        // Score
        public string Formula { get; set; }
        public decimal TotalScore { get; set; }
    }

    public class ActivityVM_View
    {
        public string Activity { get; set; }
        public string ControlItem { get; set; }
        public string Target { get; set; }
        public int Weightage { get; set; }
        public string MyResult { get; set; }
        public int EvalScore { get; set; }
    }

    public class CompetencyHeaderVM_View
    {
        public string HeaderId { get; set; }
        public string HeaderDescription { get; set; }
        public List<CompetencyLevelVM_View> Levels { get; set; } = [];
    }

    public class CompetencyLevelVM_View
    {
        public string LevelDescription { get; set; }
        public decimal SkillLevel { get; set; }
        public int EvalScore { get; set; }
    }
    public class ViewSecondHalfEvaluatorVM
    {
        public string EmployeeName { get; set; }
        public string DateOfJoining { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Evaluator { get; set; }
        public string Reviewer { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }

        public string Role { get; set; }

        public string PartAWeightage { get; set; }
        public string PartBWeightage { get; set; }

        public string FirstHalfFormula { get; set; }
        public string SecondHalfFormula { get; set; }

        public string FirstHalfScore { get; set; }
        public string SecondHalfScore { get; set; }
        public string FirstHalfTotalScore { get; set; }
        public string SecondHalfTotalScore { get; set; }

        public DataRow[] Activities { get; set; }
        public DataRow[] OtherActivities { get; set; }

        public int PartAActivityWeightage { get; set; }
        public string OtherActivityWeightage { get; set; }

        public DataTable Headers { get; set; }

        public string FHPartAComment { get; set; }
        public string SHPartAComment { get; set; }
        public string FHPartBComment { get; set; }
        public string SHPartBComment { get; set; }
        public string SHPartBEvalComment { get; set; }
        public string FHFeedback { get; set; }
        public string SHFeedback { get; set; }
        public string AssociateComment { get; set; }
    }
    public class ViewFirstHalfReviewerVM
    {
        // Associate Details
        public string Associate { get; set; }
        public string DateOfJoining { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Evaluator { get; set; }
        public string Reviewer { get; set; }

        public string Role { get; set; }

        // PART A
        public List<ActivityVM_FHR> Activities { get; set; } = new();
        public List<ActivityVM_FHR> OtherActivities { get; set; } = new();
        public int ActivityWeightage { get; set; }
        public int OtherActivityWeightage { get; set; }
        public string EvalOtherActScore { get; set; }
        public string RevOtherActScore { get; set; }

        // PART B
        public List<CompetencyHeaderVM_FHR> Competencies { get; set; } = new();

        // Scores
        public string EvalFormula { get; set; }
        public string EvalTotalScore { get; set; }
        public string RevFormula { get; set; }
        public string RevTotalScore { get; set; }
        public string EvalScore { get; set; }
        public string RevScore { get; set; }
        public string FinalScore { get; set; }

        // Comments
        public string PartAEvalComment { get; set; }
        public string PartBAssComment { get; set; }
        public string PartBEvalComment { get; set; }
        public string EvalTwoWayComment { get; set; }
        public string RevTwoWayComment { get; set; }
        public string AssocTwoWayComment { get; set; }
    }
    public class ActivityVM_FHR
    {
        public string Activity { get; set; }
        public string ControlItem { get; set; }
        public string Target { get; set; }
        public int Weightage { get; set; }
        public string MyResult { get; set; }
        public string EvalScore { get; set; }
        public string RevScore { get; set; }
    }

    public class CompetencyHeaderVM_FHR
    {
        public string HeaderDescription { get; set; }
        public List<CompetencyLevelVM_FHR> Levels { get; set; } = new();
    }

    public class CompetencyLevelVM_FHR
    {
        public string LevelDescription { get; set; }
        public string SkillLevel { get; set; }
        public string EvalScore { get; set; }
        public string RevScore { get; set; }
    }

public class ViewSecondHalfReviewerVM
    {
        // Employee Info
        public string Associate { get; set; }
        public string DateOfJoining { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Evaluator { get; set; }
        public string Reviewer { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Role { get; set; }

        // Scores
        public string FHEvalScore { get; set; }
        public string FHRevScore { get; set; }
        public string SHEvalScore { get; set; }
        public string SHRevScore { get; set; }

        public string FHEvalTotal { get; set; }
        public string FHRevTotal { get; set; }
        public string SHTotal { get; set; }

        // Formula text
        public string FHEvalFormula { get; set; }
        public string FHRevFormula { get; set; }
        public string SHEvalFormula { get; set; }
        public string SHRevFormula { get; set; }

        // Weightage
        public int PartAWeightage { get; set; }
        public int PartBWeightage { get; set; }

        // Activity Lists
        public DataTable Activities { get; set; }
        public DataTable OtherActivities { get; set; }
        public DataTable CompetencyHeaders { get; set; }

        // Comments
        public string FHEvalComment { get; set; }
        public string FHRevComment { get; set; }
        public string SHEvalComment { get; set; }
        public string SHRevComment { get; set; }
        public string AssociateComment { get; set; }
    }
    public class GSReviewerViewModel
    {
        public string PMSID { get; set; }

        // Associate Details
        public string AssociateName { get; set; }
        public string DateOfJoining { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Evaluator { get; set; }
        public string Reviewer { get; set; }
        public string ReviewerECode { get; set; }
        public string Role { get; set; }

        // Status
        public string GSStatus { get; set; }
        public string ReviewerComment { get; set; }
        public string EvaluatorComment { get; set; }
        public string AssociateEmail { get; set; }

        // Activity
        public List<ActivityVM_GSRF> Activities { get; set; } = new();
        public List<ActivityVM_GSRF> OtherActivities { get; set; } = new();

        // Competency
        public List<CompetencyHeaderVM_GSRF> Competencies { get; set; } = new();

        public bool IsEditable { get; set; }
    }

    public class ActivityVM_GSRF
    {
        public string Activity { get; set; }
        public string ControlItem { get; set; }
        public string Target { get; set; }
        public int Weightage { get; set; }
    }

    public class CompetencyHeaderVM_GSRF
    {
        public string HeaderDescription { get; set; }
        public List<CompetencyLevelVM_GSRF> Levels { get; set; } = new();
    }

    public class CompetencyLevelVM_GSRF
    {
        public string LevelDescription { get; set; }
        public decimal SkillLevel { get; set; }
    }
}
