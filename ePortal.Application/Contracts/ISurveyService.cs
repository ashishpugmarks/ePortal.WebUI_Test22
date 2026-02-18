using ePortal.DomainClasses;
using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface ISurveyService
    {
        IEnumerable<SurveyViewModel> ServerList();
        SurveyViewModel GetEditById(int? id);
        long CreateSurvey(SurveyViewModel AVM);
        short EditSurvey(SurveyViewModel AVM, int? id);
        SurveyViewModel GetDetail(int id);
        IEnumerable<ADFUNCTIONALDESIGNATION> Bind_ADFunctionalDesignation();
        IEnumerable<SYSITE> Bind_SYSite();
        short AddPercentage(SurveyViewModel SV, int id);
        QuestionViewModel GetAllAnswer(int? id);
        IEnumerable<QuestionViewModel> UserDashboard();
        IEnumerable<ADDESIGNATION> Bind_ADDesignation(); // Added by Aumento on 29-05-2024 :: SR71845

        IEnumerable<SurveyAutofillViewModel> FetchHRSurveyAutoFillList();

        string UpdateHRSurveyAutoFillList(int autoFillId, int modifiedBy, int status);

        IEnumerable<HrSurveyEmployeeDetailViewModel> GetHrSurveyEmployeeDetails();

        string AddUserHrSurveyAutoFill(long employeeId, long createdBy, long modifiedBy, int status);

        bool GetHRSurveyAutoFillList(string userId);

        IEnumerable<OperationViewModel> BindOperation(int typeId);
        IEnumerable<DivisionViewModel> BindDivision(long typeId, long op_Id);

        QuestionViewModel AddQuestionDetails(QuestionViewModel QVM);

        long DuplicateSurvey(long oldSurveyId);

        //sa CR6927
        long RemoveQuestion(long questionID);
        List<PR_Div_Dep_SecViewModel> GetEmpListForSurvey();
        IEnumerable<SurveyViewModel> GetSurveyList(SearchSurveyViewModel o);
        //ea CR6927
    }
}
