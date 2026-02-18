using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IHrIntrnlTraining
    {
        DataSet DisplayTrainerDetails();
        DataTable EmployeeDesignation(string strEmpCode);
        DataTable EmployeesTrainings(string strLoginEmpCode, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection, string strStatus, string strKI);
        DataTable GetAssociateList(string strLoggedUser, string strOperation, string strDivision, string strDept, string strSection);
        DataTable GetAvailableTrainingDates(string strBatchID);
        DataTable GetExtrnlTrngsOfEmp(string strLoginEmpCode, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection);
        DataTable GetHistoryApprovalList(string EmpCode);
        DataTable GetKI();
        DataTable GetMyTrainings(string strLoginEmpCode, string strTraining, string strTrainingStatus);
        string GetOperation(string strecode);
        DataTable GetPendingRequestList(string strEmpCode);
        DataTable GetPendingReScheduleRequest(string strEmpCode);
        DataTable GetPendingTrainingReScheduleHR(string strLoginEmpCode, string strEmpCode, string strEmpName, string strOperation, string strDivision, string strDepartment, string strSection, string strTraining, string strStatus, string strKI, string strFromDt, string strToDate);
        string GetRecAuthAppAuth(string strecode);
        DataTable GetRecMailId(string strRecAuth);
        DataTable GetRequestHistory(string RequestID);
        DataTable GetReScheduleRequestDetails(string strEmpCode, string strBatchID);
        DataTable GetSchedAttdAsso(string strLoginEmpCode, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection, string strTraining, string strStatus, string strKI, string strTrainingId, string strType);
        DataTable GetTodayTrngsOfEmp(string strLoginEmpCode, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection);
        DataTable GetTrainerMasterByID(string strTrainingMasterId);
        DataTable GetTrainingDetail(string strEmpCode, string strBatchID);
        DataTable GetTrainingRequestHistory(string strEmpCode);
        DataTable GetTrangMasterDetails(string strTrainingid);
        DataTable GetTrngMasterDesgDetails(string strTrainingid);
        DataTable get_AllQuestionHrTraining(string strHrTrainingId);
        DataSet get_AllTrainers();
        DataSet get_AllTrainers(int empcode);
        DataTable get_AllTraining();
        DataTable get_AllTrainingName();
        DataSet get_DepartmentName();
        DataSet get_DesignationName();
        DataSet get_KI();
        DataTable get_PendingHrTranDetail(string strHrtrainingId, string strTrType, string strStatus, string strStartdate, string strEnddate);
        DataTable get_PlanActualInformation(string strEmpcode);
        DataTable get_QuestionName(string strQueID);
        DataSet get_SectionName(int deptid);
        DataSet get_TrainerName();
        DataSet get_TrainingName();
        DataTable get_TrainingQuestion(string strTrainingId);
        int InsertNewTrainer(int empcode, int trainingid, int status, int addedby);
        int InsertTrainingReSchedule(string strRecAuth, string strAppAuth, string strEmpCode, string strBatchID, string strReason, string strAvailDate);
        int InsertTrngMasterData(string trainingname, string trainingperiod, int ki, int status, int addedby);
        int SetTrainingMaster(string strTrainingName, string strTrainingPeriod, string strExperience, string strstatus, string strmodifiedby, string strTrainingid, string strdesignation, string strTrObjective, string evaluation);
        string set_HrTrainingQuestion(string strTrainingId, string strQuestion, string strAddedby, string strStatus, string strHrtrainingQueId, string strFlag);
        int UpdateExistingTrainer(int trainingid, int status, int modifiedby, int trainerempcode);
        int UpdateTrngMasterData(string trainingname, string trainingperiod, int status, int trainingid, int modifiedby);
        int UpdateTrngReSdlReqByApp(string strRemarks, string strRequestID, string strStatus);
        int UpdateTrngReSdlReqByRec(string strRemarks, string strAppAuth, string strRequestID, string strAvailableDate, string strStatus, string strRecAuthority, string strAppAuthority);
    }
}
