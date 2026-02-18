using ePortal.DomainClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class KaizenViewModel
    {
        public int counts { get; set; }
        public List<string> tmpid { get; set; }
        public long? OPERATIONID { get; set; }
        public string OPERATION_ID { get; set; }
        public long? ID { get; set; }
        public string AreaName { get; set; }
        public long? DIVISIONID { get; set; }
        public long? DEPARTMENTID { get; set; }
        public long? SECTIONID { get; set; }
        public string DEPARTMENT { get; set; }
        public string DIVISION { get; set; }
        public string OPERATION { get; set; }
        public string SECTION { get; set; }
        public long pendingCount { get; set; }
        public long totalcount { get; set; }
        public string MONTH_ID { get; set; }
        public string KaizenID { get; set; }
        public long pen_count { get; set; }
    }

    public class SearchKZViewModel
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
        public long Ecode { get; set; }
        public int EvalMonth { get; set; }
        public string Period { get; set; }
        public int syki { get; set; }
        public List<VM_ADORGLEVEL_KAiZEN> OperationName { get; set; }
        public List<VM_ADORGLEVEL_KAiZEN> divisionName { get; set; }
        public List<VM_ADORGLEVEL_KAiZEN> sectionName { get; set; }
        public List<VM_ADORGLEVEL_KAiZEN> departmentName { get; set; }
    }
    public partial class VM_ADORGLEVEL_KAiZEN
    {
        public int count { get; set; }
        public string EmpMail { get; set; }
        public string Ph_NO { get; set; }
        public string DEPARTMENTNAME { get; set; }
        public string DIVISIONNAME { get; set; }
        public string OPERATIONNAME { get; set; }
        public string SECTIONNAME { get; set; }
        public decimal SRNO { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public List<KaizenViewModel> result { get; set; }

    }

    public  class VM_D_TMPL_HEADER
    {
        public decimal SRNO { get; set; }
        public string TMPL_CODE { get; set; }
        public Nullable<decimal> USER_CODE { get; set; }
        public string REMARKS { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
    }

    public  class VM_JSON_DATA
    {
        public string ReqFor { get; set; }
        public string ReqType { get; set; }
        public string name { get; set; }
        public string ecode { get; set; }
        public string operation { get; set; }
        public string division { get; set; }
        public string department { get; set; }
        public string section { get; set; }
        public string name1 { get; set; }
        public string ecode1 { get; set; }
        public string operation1 { get; set; }
        public string division1 { get; set; }
        public string department1 { get; set; }
        public string section1 { get; set; }
        public string location { get; set; }
        public string Theme { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string benefit { get; set; }
        public string costsave { get; set; }
        public string costamount { get; set; }
        public string[] category { get; set; }
        public string requestno { get; set; }
        public string GRADE { get; set; }
        public string ImplementationDate { get; set; }
        public string KaizenImplement { get; set; }
        public string MONTH_ID { get; set; }
        public long? totalscore { get; set; }
        public long? usertotalscore { get; set; }
        public string[] Scoring { get; set; }
        public string status { get; set; }
        public string decl { get; set; }
        public long? REQStatus { get; set; }

    }
    public class VM_D_TRAN
    {
        public long STAGEID { get; set; }
        public string SEQID { get; set; }
        public Nullable<System.DateTime> RECEIVEDDATE { get; set; }
        public Nullable<System.DateTime> ACTUALFINISHDATE { get; set; }
        public Nullable<decimal> ID { get; set; }
        public string FILEDETAIL { get; set; }
        public string APPROVER_FILEDETAIL { get; set; }
        public string TMPLHEADERID { get; set; }
        public string DOCUMENTTYPE { get; set; }
        public string ecode { get; set; }
        public string OPERATIONNAME { get; set; }
        public string DEPARTMENTNAME { get; set; }
        public string DIVISIONNAME { get; set; }
        public string SECTIONNAME { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public VM_JSON_DATA jsondata { get; set; }
        public VM_Approver_Data jsonapproverdata { get; set; }
        public List<VM_File_Detail> FileDetail { get; set; }
        public List<string> Filenames { get; set; }
        public string GRADE { get; set; }
        public string MONTH_ID { get; set; }
        public List<VM_File_Detail> APPROVER_FILENAMES { get; set; }
        public string totalscore { get; set; }
        public string copyIdea { get; set; }
        public KaizenMarks EvaluatorMarks { get; set; }
        public List<KaizenMarks> AllEvaluatorMarks { get; set; }
        public List<KaizenMarks> AllDiv_EvaluatorMarks { get; set; }
        public long? Creativityscore { get; set; }
        public long? Applicabilityscore { get; set; }
        public long? Effortscore { get; set; }
        public long? Safetyscore { get; set; }
        public long? Environmentscore { get; set; }
        public long? Conveniencescore { get; set; }
        public long? Qualityscore { get; set; }
        public long? costscore { get; set; }
        public string ReqMail { get; set; }
        public string ReqPh_no { get; set; }
        public Kaizen_Evaluator ApproverMarks { get; set; }
        public int EvalCount { get; set; }
        public DeptAvgMarks DeptCommitteeMarks { get; set; }
        public long? FinalRank { get; set; }
    }
    public class VM_Second_stage_data
    {
        public string copyidea { get; set; }
    }
    public class VM_Approver_Data
    {
        public string Creativityweight { get; set; }
        public string Creativityscore { get; set; }
        public string Applicabilityweight { get; set; }
        public string Applicabilityscore { get; set; }
        public string Effortweight { get; set; }
        public string Effortscore { get; set; }
        public string Safetyweight { get; set; }
        public string Safetyscore { get; set; }
        public string Environmentweight { get; set; }
        public string Environmentscore { get; set; }
        public string Convenienceweight { get; set; }
        public string Conveniencescore { get; set; }
        public string Qualityweight { get; set; }
        public string Qualityscore { get; set; }
        public string costweight { get; set; }
        public string costscore { get; set; }
        public string totalscore { get; set; }
        public string award { get; set; }
        public string Idea { get; set; }

    }
    public class VM_File_Detail
    {
        public string doctype { get; set; }
        public string info { get; set; }
        public string filepath { get; set; }
    }

    public class KaizenMarks
    {
        public string ID { get; set; }
        public string KAIZENID { get; set; }
        public long ADEMPCODE { get; set; }
        public long? Creativityscore { get; set; }
        public long? Applicabilityscore { get; set; }
        public long? Effortscore { get; set; }
        public long? Safetyscore { get; set; }
        public long? Environmentscore { get; set; }
        public long? Conveniencescore { get; set; }
        public long? Qualityscore { get; set; }
        public long? costscore { get; set; }
        public long? totalscore { get; set; }
        public Nullable<long> ADDEDBY { get; set; }
        public Nullable<System.DateTime> ADDEDDATE { get; set; }
        public Nullable<System.DateTime> MODIFIEDDATE { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public string OPERATIONID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public short? costverify { get; set; }
        public short? DuplicateIdea { get; set; }
        public string Remark { get; set; }
        public short? Status { get; set; }
        public string Theme{ get; set; }
        public long? rank { get; set; }

    }
    public class KaizenGrade
    {
        public string GRADE { get; set; }
        public long? totalscore { get; set; }
        public long? usertotalscore { get; set; }
        public long? REQstatus { get; set; }
        public string MONTH_ID { get; set; }
        public long? CREATIVITY { get; set; }
        public long? Applicabilityscore { get; set; }
        public long? Effortscore { get; set; }
        public long? Safetyscore { get; set; }
        public long? Environmentscore { get; set; }
        public long? Conveniencescore { get; set; }
        public long? Qualityscore { get; set; }
        public long? costscore { get; set; }
        public string Evalscore { get; set; }
        public string Kaizenid { get; set; }
        public int totalEvalCount { get; set; }
        public short? UserStatus { get; set; }
        public long? FinalRank { get; set; }
    }
    public class VM_EXCEL_DATA
    {
        public string FORM { get; set; }
        public string REQUEST_NO { get; set; }
        public string GRADE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string MONTH_ID { get; set; }
        public string APPROVER_DATA { get; set; }
        public long? TOTALSCORE { get; set; }
        public string stage1 { get; set; }
        public string stage2 { get; set; }
        public string Approver_Ecode { get; set; }
        public DateTime? First_Level_Approver { get; set; }
        public DateTime? Dept_Committee_Approver { get; set; }
        public DateTime? AddedDate { get; set; }
    }
   public class VM_Operation_UserList
    {
        public string lastName { get; set; }
        public string operationID { get; set; }
        public string operationName { get; set; }
        public string FirstName { get; set; }
        public long? user1 { get; set; }
        public long? user2 { get; set; }
        public long? user3 { get; set; }
        public long? user4 { get; set; }
        public long CommitteeID { get; set; }
        public string CommitteeName { get; set; }
        public string Username1 { get; set; }
        public string Username2 { get; set; }
        public string Username3 { get; set; }
        public string Username4 { get; set; }
        public short? status { get; set; }
        public long? totalscore { get; set; }
        public long? creativity { get; set; }
        public long? applicability { get; set; }
        public long? effort { get; set; }
        public long? safety { get; set; }
        public long? environment { get; set; }
        public long? convenince { get; set; }
        public long? quality { get; set; }
        public short? costverify { get; set; }
        public long? cost { get; set; }
        public long? modifiedby { get; set; }
        public DateTime? AddedDate { get; set; }
        public short? Duplicate_Idea { get; set; }
        public long? Rank { get; set; }
    }
    public class SearchHRAdmin
    {
        [DisplayName("Plant ID")]
        public long? syplantID { get; set; }
        [DisplayName("Plant Name")]
        public string syplantname { get; set; }
        [DisplayName("Emp code")]
        public long empcode { get; set; }
        [DisplayName("Name")]
        public string firstname { get; set; }
        public string lastname { get; set; }
        public long ID { get; set; }
    }
    public class Kaizen_Evaluator
    {
        public string name_tmp { get; set; }
        public long ecode_tmp { get; set; }
        public string ph_No_tmp { get; set; }
        public string emailid_tmp { get; set; }
    }
    public class Approver_Details
    {
        public string ecode { get; set; }
        public string name { get; set; }
    }

    public class VM_DivCommitee_UserList
    {
        public string lastName { get; set; }
        public string operationID { get; set; }
        public string operationName { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public long? empcode { get; set; }
        public string FirstName { get; set; }
        public long? user1 { get; set; }
        public long? user2 { get; set; }
        public long? user3 { get; set; }
        public long? user4 { get; set; }
        public long CommitteeID { get; set; }
        public string CommitteeName { get; set; }
        public string Username1 { get; set; }
        public string Username2 { get; set; }
        public string Username3 { get; set; }
        public string Username4 { get; set; }
        public short? status { get; set; }
        public long? totalscore { get; set; }
        public long? creativity { get; set; }
        public long? applicability { get; set; }
        public long? effort { get; set; }
        public long? safety { get; set; }
        public long? environment { get; set; }
        public long? convenince { get; set; }
        public long? quality { get; set; }
        public short? costverify { get; set; }
        public long? cost { get; set; }
        public long? modifiedby { get; set; }
        public DateTime? AddedDate { get; set; }
        public string Kaizenid { get; set; }
        public string Form { get; set; }
        public long? UserRank { get; set; }
        public long? Rank { get; set; }
        public string MonthID { get; set; }
        public long? FinalRank { get; set; }
    }

    public class DeptAvgMarks
    {
        public Double? creativity { get; set; }
        public Double? applicability { get; set; }
        public Double? effort { get; set; }
        public Double? safety { get; set; }
        public Double? environment { get; set; }
        public Double? convenince { get; set; }
        public Double? quality { get; set; }
        public Double? cost { get; set; }
        public Double? Totalscore { get; set; }
        public List<string> EmpName { get; set; }
        public List<long?> Empcode { get; set; }
    }

    public class TopCoreSteeringKaizen
    {
        public string Form { get; set; }
        public string KaizenID { get; set; }
        public string Theme { get; set; }
        public long? EvalScore { get; set; }
        public int? status { get; set; }
        public long? KaizenOrder { get; set; }
    }

    public class DivRank
    {
        public Dictionary<string,string> rank { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class KAIZEN_KIViewModel
    {
        public long Value { get; set; }
        public string Text { get; set; }
    }
    public class KaizenLibrary_Model
    {
        public string Form { get; set; }
        public string approverData { get; set; }
        public string requestID { get; set; }
        public string userID { get; set; }
        public string status { get; set; }
 
    }

    public class Kaizen_Lirbary_VM
    {
        public VM_JSON_DATA AssociateData { get; set; }
        public Approver_Details Approver_data { get; set; }
        public string requestID { get; set; }
        public string userID { get; set; }
        public string status { get; set; }
        public string AssociateData_1 { get; set; }
        public string AssociateData_3 { get; set; }
        public string File1 { get; set; }
        public string File2 { get; set; }
        public string DuplicateIdea1 { get; set; }
        public string DuplicateIdea2 { get; set; }
        public string DuplicateIdea3 { get; set; }
        public short? DuplicateIdea4 { get; set; }
        public string DuplicateIdea5 { get; set; }

    }
    public class DuplicateKaizen
    {
        public string copyidea { get; set; }
        public string decl { get; set; }
        public string Idea { get; set; }

    }
}
