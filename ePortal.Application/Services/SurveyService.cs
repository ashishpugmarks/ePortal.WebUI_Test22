using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.Persistence.Interface;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly SurveyRepository _SurveyRepository;
        private readonly ISurveyConfig _SurveyConfig;
        

        public SurveyService(SurveyRepository SurveyRepository, ISurveyConfig SurveyConfig)
        {
            _SurveyRepository = SurveyRepository;
            _SurveyConfig = SurveyConfig;
        }

        public IEnumerable<SurveyViewModel> ServerList()
        {
            return _SurveyRepository.SurveyList();
        }
        public SurveyViewModel GetEditById(int? id)
        {
            return _SurveyRepository.GetEditById(id);
        }
        public long CreateSurvey(SurveyViewModel SV)
        {
            return _SurveyRepository.CreateSurvey(SV);
        }
        public short EditSurvey(SurveyViewModel SV, int? id)
        {
            return _SurveyRepository.EditSurvey(SV, id);
        }
        public SurveyViewModel GetDetail(int id)
        {
            return _SurveyRepository.Detail(id);
        }
        public IEnumerable<ADFUNCTIONALDESIGNATION> Bind_ADFunctionalDesignation()
        {
            return _SurveyRepository.Bind_ADFunctionalDesignation();
        }
        public IEnumerable<SYSITE> Bind_SYSite()
        {
            return _SurveyRepository.Bind_SYSite();
        }

        public short AddPercentage(SurveyViewModel SV, int id)
        {
            return _SurveyRepository.AddPercentage(SV, id);
        }
        public QuestionViewModel GetAllAnswer(int? id)
        {
            return _SurveyRepository.GetAllAnswer(id);
        }
        public IEnumerable<QuestionViewModel> UserDashboard()
        {
            return _SurveyRepository.UserDashboard();
        }
        public IEnumerable<ADDESIGNATION> Bind_ADDesignation()// Added by Aumento on 29-05-2024 :: SR71845
        {
            return _SurveyRepository.Bind_ADDesignation();
        }

        public IEnumerable<SurveyAutofillViewModel> FetchHRSurveyAutoFillList()
        {
            try
            {
                //DataTable dt = _SurveyConfig.FetchHRSurveyAutoFillList();
                //var sampel = "abc";
                //return dt.AsEnumerable()
                // .Select(row => new SurveyAutofillViewModel
                // {
                //     AUTOFILLID = row.Field<int?>("AUTOFILLID") ?? 0,
                //     EMPLOYEENAME = row.Field<string>("EMPLOYEENAME"),
                //     EMPLOYEECODE = row.Field<int?>("EMPLOYEECODE") ?? 0,
                //     STATUS = row.Field<int?>("STATUS") ?? 0,
                // }).ToList();
                return _SurveyConfig.FetchHRSurveyAutoFillList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in fetching HR Survey AutoFill List: " + ex.Message);
            }
        }

        public string UpdateHRSurveyAutoFillList(int autoFillId, int modifiedBy, int status)
        {
            //int status = isStatusOn ? 1 : 0;

            return _SurveyConfig.UpdateHRSurveyAutoFillList(autoFillId, modifiedBy, status);
        }

        public IEnumerable<HrSurveyEmployeeDetailViewModel> GetHrSurveyEmployeeDetails()
        {
            try
            {
                return _SurveyConfig.GetHrSurveyEmployeeDetails();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in fetching HR Survey AutoFill List: " + ex.Message);
            }
        }

        public string AddUserHrSurveyAutoFill(long employeeId, long createdBy, long modifiedBy, int status)
        {

            return _SurveyConfig.AddUserHrSurveyAutoFill(employeeId, createdBy, modifiedBy, status);
        }

        public bool GetHRSurveyAutoFillList(string userId)
        {
            return _SurveyConfig.GetHRSurveyAutoFillList(userId);
        }

        public IEnumerable<OperationViewModel> BindOperation(int typeId)
        {
            return _SurveyRepository.BindOperation(typeId);
        }

        public IEnumerable<DivisionViewModel> BindDivision(long typeId, long op_Id)
        {
            return _SurveyRepository.BindDivision(typeId, op_Id);
        }

        public QuestionViewModel AddQuestionDetails(QuestionViewModel QVM)
        {
            return _SurveyRepository.AddQuestionDetails(QVM);
        }

        public long DuplicateSurvey(long oldSurveyId)
        {
            return _SurveyRepository.DuplicateSurvey(oldSurveyId);
        }

        //sa CR6927
        public long RemoveQuestion(long questionID)
        {
            return _SurveyRepository.RemoveQuestion(questionID);
        }
        public List<PR_Div_Dep_SecViewModel> GetEmpListForSurvey()
        {
            return _SurveyRepository.GetEmpListForSurvey();
        }
        public IEnumerable<SurveyViewModel> GetSurveyList(SearchSurveyViewModel o)
        {
            return _SurveyRepository.GetSurveyList(o);
        }
        //ea CR6927
    }
}
