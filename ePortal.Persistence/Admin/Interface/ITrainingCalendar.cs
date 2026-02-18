using System.Data;

namespace ePortal.Persistence.Admin.Interface
{
    public interface ITrainingCalendar
    {
        string CancelTraninigBatch(string strbatchid, string straddedby);
        string EditAssociateDetail(string strdetailid, string strstatus, string strRescheduledate, string strtrainingid, string straddedby);
        string EditTraninigBatch(string strbatchid, string strfrmdate, string strtodate, string strstarttime, string strendtime, string strtrainer1, string strtrainer2, string strvenue, string strpendingasscosiates, string straddedby, string strki, string strcomment);
        DataTable GetAllSelectedBatches(string strTrainigId, string strDate, string strKi);
        DataTable GetAllTraining();
        DataTable GetAssoDetails(string RequestDetailID);
        DataTable GetAssoDetailsForEmail(string strbatchid);
        DataTable GetAssoEmailDetails(string strComdtlId);
        DataTable GetAssoEvalEmail(string strEmpcode, string strTransactionId);
        DataTable GetAssoOfBatch(string strbatchid);
        DataTable GetAvailableDataList(string strtrainingid, string strdate);
        DataTable GetBatchDetail(string strbatchid);
        DataTable GetEmailDetails(string strbatchid);
        DataSet GetEvaluationdetail(string strTrngevalid);
        DataTable GetEvaluationList(string strLoginEmpCode);
        DataTable GetEvaluationMail();
        DataSet GetEvaluationquestionwithanswerlist(string strevalID);
        DataTable GetEvaluationRpt(string strTraining, string strKi, string strgrade, string strbatch);
        DataTable GetEvalutionDetail(string strEmp, string strTransactionID);
        DataTable GetFeedbackDetail(string strEmp, string strTransactionID);
        DataTable GetFeedbackDetails(string strBatchid, string strempcode);
        DataTable GetFeedbackRpt(string strOperation, string strDivision, string strDepartment, string strSection, string strTraining, string strKi);
        DataTable GetFeedbackStatus(string strTraining, string strKi, string strbatchid, string strstatus);
        string GetIsHolidayOrNot(string strdate, string strSySiteID);
        DataTable GetPendingAssoList(string strTrainingId);
        DataTable GetPendingEvalutionList(string strTraining, string strKi, string strStatus, string strbatch, string strEmpCode);
        DataTable GetScheduledAssoList(string strdate);
        DataTable GetScheduledTrainingId(string strdate);
        DataTable GetTrainerEmailID(string strbatchid);
        string GetTrainingDuration(string strtrainingid);
        DataTable GetTrngDataInCalndr(string strdate);
        DataSet GET_EVALANSWERINUSERHAND(string TRANSID);
        DataTable Get_EvalutionTrainerEmailID(string strEmp);
        DataTable Get_FeedbackEmpEmailID(string strEmp);
        DataTable GET_SeniorHeadMail(string strEmpcode);
        string InsertEvaluationfrm(string strBatchID, string strEmpCode, string strlogincode, string strXml, string strGrade);
        string InsertFeedbackfrm(string strBatchID, string strEmpCode, string strrdolst1, string strrdolst2, string strrdolst3, string strrdolst4, string strrdolst5, string strrdolst6, string strrdolst7_1, string strrdolst7_2, string strrdolst8_1, string strrdolst8_2, string strrdolst9_1, string strrdolst9_2, string strrdolst10_1, string strrdolst10_2, string strrdolst11_1, string strrdolst11_2, string strrdolst12, string strrdolst13, string strrdolst14, string strtxtlike, string strtxttopics, string strtxtfeedback, string strtxtImprovementFeedback);
        string RescheduleCount(string strecode, string strtrainingid);
        DataTable SetMailCounterGetMailId(string strbatchid, string straddedby);
        string SetTraninigBatch(string strinternaltraining, string strfrmdate, string strtodate, string strstarttime, string strendtime, string strtrainer1, string strtrainer2, string strvenue, string strpendingasscosiates, string straddedby, string strki, string strcomment);
        string UpdateRetrainingstatus(string strTrngevalid, int strretrngstatus);
    }
}
