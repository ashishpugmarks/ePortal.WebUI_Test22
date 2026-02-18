using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;

namespace ePortal.Persistence.Interface
{
    public interface ISurveyConfig
    {
        List<SurveyAutofillViewModel> FetchHRSurveyAutoFillList();
        string UpdateHRSurveyAutoFillList(int autoFillId, int modifiedBy, int status);
        List<HrSurveyEmployeeDetailViewModel> GetHrSurveyEmployeeDetails();
        string AddUserHrSurveyAutoFill(long employeeId, long createdBy, long modifiedBy, int status);
        bool GetHRSurveyAutoFillList(string userId);
        List<DivisionViewModel> GetAllDivision();
        List<DivisionViewModel> GetAllDivisionBYOperation(long OperationID);
        List<OperationViewModel> GetAllOperation();
    }
}
