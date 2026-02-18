using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels
{
    public class SurveyViewModel
    {
        [Key]
        [Display(Name = "Sl")]
        public decimal HRSECURITYSURVEYID { get; set; }

        //[Display(Name = "Survey")] //Survey enhancement
        [Display(Name = "Name of Survey(English)")] //Survey enhancement
        public string HRSURVEYDESC { get; set; }

        [Display(Name = "Name of Survey(Hindi)")] //Survey enhancement
        public string HRSURVEYDESCHINDI { get; set; }

        [Display(Name = "Name of Survey(Kannada)")] //Survey enhancement
        public string HRSURVEYDESCKANNADA { get; set; }

        [Display(Name = "Name of Survey(Gujarati)")] //Survey enhancement
        public string HRSURVEYDESCGUJARATI { get; set; }

        [Display(Name = "Name of Survey(Japanese)")] //Survey enhancement
        public string HRSURVEYDESCJAPANESE { get; set; }


        public string HRSURVEYSTARTDATE_STRING { get; set; }
        public string HRSURVEYENDDATE_STRING { get; set; }


        //[Display(Name = "From Date")] //Survey enhancement
        [Display(Name = "Survey start date")] //Survey enhancement
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> HRSURVEYSTARTDATE { get; set; }
        //[Display(Name = "To Date")] //Survey enhancement
        [Display(Name = "Survey end date")] //Survey enhancement
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> HRSURVEYENDDATE { get; set; }
        public Nullable<System.DateTime> DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        //[Display(Name = "Tittle")] //Survey enhancement
        [Display(Name = "Title of survey")] //Survey enhancement
        public string HRSURVEYTITAL { get; set; }
        //[Display(Name = "Status")] //Survey enhancement
        [Display(Name = "Do you want to activate survey from your selected date")] //Survey enhancement
        //public bool ACTIVE { get; set; }
        public long? ACTIVE { get; set; }
        [Display(Name = "ReTest")]
        public Nullable<long> RETEST { get; set; }
        //[Display(Name = "Email Result")] //Survey enhancement
        [Display(Name = "Do you want to send email result to participants")] //Survey enhancement
        public bool SURVEYRESULTMAIL { get; set; }
        //[Display(Name = "Feedback Comment")] //Survey enhancement
        [Display(Name = "Do you want to take feedback of survey")] //Survey enhancement
        public bool FEEDBACKSTATUS { get; set; }
        [Display(Name = "Percentage (%)")]
        public Nullable<long> PASSINGPERCENTAGE { get; set; }
        public Nullable<long> SURVEYID { get; set; }
        [Display(Name = "Functional Designation")]
        public string[] FUNCTIONAL_DESIGNATION { get; set; }
        public string FUNCTIONAL_DESIGNATIONS { get; set; }
        [Display(Name = "Location")]
        public string[] LOCATION { get; set; }
        public string LOCATIONS { get; set; }

        [Display(Name = "Operation")]
        public string[] OPERATION { get; set; }
        public string OPERATION_VALUE { get; set; }

        [Display(Name = "Division")]
        public string[] DIVISION { get; set; }
        public string DIVISION_VALUE { get; set; }

        public List<QuestionViewModel> Questionlist { get; set; }
        //[Display(Name = "Survey Header")] //Survey enhancement
        [Display(Name = "Detail of Survey(English)")] //Survey enhancement
        public string HRSURVEY_HEADER { get; set; }

        [Display(Name = "Detail of Survey(Hindi)")] //Survey enhancement
        public string HRSURVEY_HEADER_HINDI { get; set; }

        [Display(Name = "Detail of Survey(Kannada)")] //Survey enhancement
        public string HRSURVEY_HEADER_KANNADA { get; set; }

        [Display(Name = "Detail of Survey(Gujarati)")] //Survey enhancement
        public string HRSURVEY_HEADER_GUJARATI { get; set; }

        [Display(Name = "Detail of Survey(Japanese)")] //Survey enhancement
        public string HRSURVEY_HEADER_JAPANESE { get; set; }
        //[Display(Name = "Is Remarks Mandatory")] //Survey enhancement
        [Display(Name = "Is Remarks Mandatory")] //Survey enhancement
        public bool ISREMARKSMANDATORY { get; set; }
        [Display(Name = "Survey Type")]
        public long SurveyType { get; set; }
        //[Display(Name = "Login Count")] //Survey enhancement
        [Display(Name = "Count")] //Survey enhancement
        public long LoginCNT { get; set; }
        [Display(Name = "IS HCG Survey")]
        public bool ISHCGSURVEY { get; set; }

        public string HRSURVEYMODULETYPE { get; set; } //Survey enhancement
        public bool ISHCGAUTHECODE { get; set; }

        [DisplayName("Select Employee")]
        public long EMP { get; set; }
    }

    public class QuestionViewModel_VM
    {        
        public long QusId { get; set; }        
        public string QUSDESCRIPTION { get; set; }
                
        public string QUSDESCRIPTIONHINDI { get; set; }
                
        public string QUSDESCRIPTIONKANNADA { get; set; }
                
        public string QUSDESCRIPTIONGUJARATI { get; set; }
                
        public string QUSDESCRIPTIONJAPANESE { get; set; }
                
        public int OPTIONTYPE { get; set; }

        public List<AnswerViewModel> MappingList { get; set; }

        public long SUBQUESTIONPARENTID { get; set; }

        public int ISSUBQUESTION { get; set; }

        public string TRIGGEROPTIONIDS { get; set; }

        public long HRSECURITYSURVEYID { get; set; }

    }

    public class QuestionViewModel
    {
        [Key]
        public long QusId { get; set; }
        [Display(Name = "Question(English)")]
        public string QUSDESCRIPTION { get; set; }

        [Display(Name = "Question(Hindi)")]
        public string QUSDESCRIPTIONHINDI { get; set; }

        [Display(Name = "Question(Kannada)")]
        public string QUSDESCRIPTIONKANNADA { get; set; }

        [Display(Name = "Question(Gujarati)")]
        public string QUSDESCRIPTIONGUJARATI { get; set; }

        [Display(Name = "Question(Japanese)")]
        public string QUSDESCRIPTIONJAPANESE { get; set; }


        [Display(Name = "Answer(English)")]
        public string ANSDESCRIPTION { get; set; }

        [Display(Name = "Answer(Hindi)")]
        public string ANSDESCRIPTIONHINDI { get; set; }

        [Display(Name = "Answer(Kannada)")]
        public string ANSDESCRIPTIONKANNADA { get; set; }

        [Display(Name = "Answer(Gujarati)")]
        public string ANSDESCRIPTIONGUJARATI { get; set; }

        [Display(Name = "Answer(Japanese)")]
        public string ANSDESCRIPTIONJAPANESE { get; set; }

        //[Display(Name = "IsCorrect")] //Survey enhancement
        [Display(Name = "IsCorrect")] //Survey enhancement
        public short? ISCORRECT { get; set; }

        [Display(Name = "Status")]
        public short? Status { get; set; }

        [Display(Name = "Survey Name")]
        public string SurveyName { get; set; }
        public List<AnswerViewModel> MappingList { get; set; }
        public Nullable<long> HRQASSURVEYID { get; set; }
        public Boolean IsRetestNewQues { get; set; }

        [Display(Name = "Is Family Declaration Required")]
        public bool ISHCGSURVEY { get; set; }

        [Display(Name = "Is Family Dec. Req.")]
        public bool ISFAMILYDECREQ { get; set; }

        [Display(Name = "Option Type")]
        public short? OPTIONTYPE { get; set; }

        public long HRSECURITYSURVEYID { get; set; }
        public string PARENTQUSDESCRIPTION { get; set; }
        public List<AnswerViewModel> ParentMappingList { get; set; }

        public long? SUBQUESTIONPARENTID { get; set; }
        public short? ISSUBQUESTION { get; set; }
        public string TRIGGEROPTIONIDS { get; set; }
        public long ADDEDBY { get; set; }

        public int isChildPresent { get; set; }
    }

    public class AnswerViewModel
    {
        [Key]
        public long AnsId { get; set; }
        [Display(Name = "Answer(English)")]
        public string ANSDESCRIPTION { get; set; }

        [Display(Name = "Answer(Hindi)")]
        public string ANSDESCRIPTIONHINDI { get; set; }

        [Display(Name = "Answer(Kannada)")]
        public string ANSDESCRIPTIONKANNADA { get; set; }

        [Display(Name = "Answer(Gujarati)")]
        public string ANSDESCRIPTIONGUJARATI { get; set; }

        [Display(Name = "Answer(Japanese)")]
        public string ANSDESCRIPTIONJAPANESE { get; set; }

        [Display(Name = "IsCorrect")]
        public short? ISCORRECT { get; set; }

        [Display(Name = "Status")]
        public short? Status { get; set; }
        public bool ISFAMILYDECREQ { get; set; }
    }

    
    public class SurveyAutofillViewModel
    {
        [Key]
        [Display(Name = "Sl No")]
        public long AUTOFILLID { get; set; }

        [Display(Name = "Employee Code")]
        public long EMPLOYEECODE { get; set; }

        [Display(Name = "Employee Name")]
        public string EMPLOYEENAME { get; set; }

        [Display(Name = "Autofill Status")]
        public int STATUS { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class HrSurveyEmployeeDetailViewModel
    {
        public long EMPLOYEEID { get; set; }
        public string EMPLOYEENAME { get; set; }
    }


    public class DivisionViewModel
    {
        public long ADDIVISIONID { get; set; }
        public string DESCRIP { get; set; }
        public string INITIALDESCRIP { get; set; }
        public long ADVPID { get; set; }
        public int ACTIVE { get; set; }
    }

    public class OperationViewModel
    {
        public long OPERATIONID { get; set; }
        public string OPERATION { get; set; }
    }

    public class SearchSurveyViewModel
    {
        public long EmpID { get; set; }
        public string Startdate { get; set; }
        public string ENDDATE { get; set; }
    }

    public class SubquestionOptionMapping
    {
        public int SubquestionId { get; set; }
        public int OptionId { get; set; }
    }



    public class SurveyQuestion_Result
    {
        public int ROWNUM { get; set; }
        public string QUSDESCRIPTION { get; set; }
        public string SCORE { get; set; }
    }

    public class InfoSecuReportViewModel
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public int OperationId { get; set; }
        public int DivisionId { get; set; }
        public int DepartmentId { get; set; }
        public int SectionId { get; set; }
        public int SurveyId { get; set; }
        public List<int> DesignationIds { get; set; } = new();
        public string Message { get; set; }

        // Dropdown data
        public IEnumerable<SelectListItem> Operations { get; set; }
        public IEnumerable<SelectListItem> Divisions { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> Sections { get; set; }
        public IEnumerable<SelectListItem> Surveys { get; set; }
        public IEnumerable<SelectListItem> Designations { get; set; }

        // Grid data
        //public DataTable SurveyReport { get; set; }
        //public DataTable TotalResult { get; set; }

        // For displaying tables
        public List<SummaryItem> SummaryList { get; set; }
        public List<DetailItem> DetailList { get; set; }

    }

    public class SummaryItem
    {
        public string Ecode { get; set; }
        public string Name { get; set; }
        public string Score { get; set; }
        public string AVGPER { get; set; }
    }

    public class DetailItem
    {
        public string Level { get; set; }
        public string TotalAssociate { get; set; }     
    }


    //public class MachineViewModel
    //{
    //    public int OperationId { get; set; }
    //    public int DivisionId { get; set; }
    //    public int DepartmentId { get; set; }
    //    public int SectionId { get; set; }

    //    public List<SelectListItem> OperationList { get; set; }
    //    public List<SelectListItem> DivisionList { get; set; }
    //    public List<SelectListItem> DepartmentList { get; set; }
    //    public List<SelectListItem> SectionList { get; set; }
    //}


}