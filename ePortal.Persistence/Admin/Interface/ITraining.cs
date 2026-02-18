using System.Data;

namespace ePortal.Persistence.Admin.Interface
{
    public interface ITraining
    {
        DataTable GetKI();
        DataTable GetAssociateList(String strLoggedUser, String strOperation, String strDivision, String strDept, String strSection);
        DataTable EmployeeDesignation(string strEmpCode);
        DataTable EmployeesTrainings(String strLoginEmpCode, string strEmpCode, string strEmpName, string strOperation, string strDivision,
                                                    string strDepartment, string strSection, string strTraining, string strStatus,
                                                    string strKI, string strFromDt, string strToDate);
        DataTable GetMyTrainings(String strLoginEmpCode);
        DataTable GetAvailableTrainingDates(string strBatchID);
        DataTable GetRecommendedAuthority(string strEmpCode);
        DataTable GetApprovalAuthority(string strEmpCode);
        DataTable GetPendingReScheduleRequest(string strEmpCode);
        DataTable GetReScheduleRequestDetails(string strEmpCode, string strRequestID);
        DataTable GetHistoryApprovalList(String EmpCode);
        DataTable GetRequestHistory(String RequestID);
        DataTable GetPendingRequestList(string strEmpCode);
        DataTable GetTrainingRequestHistory(String strEmpCode);
        void CheckRec_App_Authority(String strPOS, int strOP, int strDIV, int strDPT, int strSEC, out String IsRecExist, out String IsAppExist);
        DataTable GetOldAttendedTrainings(String strEmpCode, String strTrainingTransID);
        DataTable GetTrainingDetail(String strEmpCode, String strBatchID);
        DataTable GetEmployeeEmailID(String strEmpCode);
        DataTable GetPendingTrainingReScheduleHR(String strLoginEmpCode, string strEmpCode, string strEmpName, string strOperation, string strDivision,
                                                    string strDepartment, string strSection, string strTraining, string strStatus,
                                                    string strKI, string strFromDt, string strToDate);
        DataTable GetFeedbackDetailsForTraining(String strBatchID, String strFeedbackStatus);
        DataTable GetFeedbackCourseContentScores(String strHrTrainingDetailID);
        DataTable GetFeedbackTrainerScores(String strHrTrainingDetailID);
        DataTable GetFeedbackTrainingEnvScores(String strHrTrainingDetailID);
        DataTable GetTrainingDetails(String strBatchID, String strHRTrainingDetailID);
        DataTable GetTrainingBatch(String strTrainingID, String strKI);
        DataTable GetTrainers(String strTrainingID);
        DataTable GetTrainingByActiveKI();
        DataTable GetBatchesTillDate(String strTrainingID, String strKI);
        DataTable GetBatchDetails(String strTrainingID, String strBatchID, String strEmpCode);
        int InsertTrainingReSchedule(String strAuthType, String strEmpCode, String strHRTrainingID,
                                String strRecAdeEmpCode, String strRemarks, String strAddedBy, String strActive, String strNewRecScheduleTrnID);
        string UpdateApprovalStatus(string AppType, string ResheduleDt, string RequestID, string status, string Remarks,
                                                   string AppAuthCode, string IsRecApplicable);
        string UpdateHRApprovalStatus(string RequestID, string status, string Remarks,
                                                  string AppAuthCode, string strReqEmpCode, string strBatchID, string strAddedBy);
        string CancelTrainigReScheduleRequest(string RequestID, string Remarks, string strAddedBy);
        DataTable Gettrainingobjective(string DESC, string TRDUR, string TRAINID);


        DataTable SPROC_AUTHORITY_GET(string ECODE);
        string SendMailcount(string TRNGDETAILID, string SENDMAILCOUNTYPE);







    }
}
