using ePortal.ViewModels;
using ePortal.ViewModels.APPX.Training;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.APPX.Contracts
{
    public interface ITrainingService
    {
        IEnumerable<SelectListItem> GetAllTraining();
        List<MyTrainingViewModel> GetMyTraining(string userid, string trainning, string trainningStatus);
        TrainningDetailsViewModel GetTrainingDetail(string userid, string batchId);
        FeedbackViewModel GetFeedbackDetails(string batchId, string userid);
        string SaveFeedbackForm(string batchId, string UserId, FeedbackViewModel feedback);
        string GetRecAuthAppAuth(string UserId);
        IEnumerable<SelectListItem> PopulateAvailableTrainingDates(string BatchID);
        int InsertTrainingReSchedule(string RecAuth, string AppAuth, string batchid, string Reason, string AvailableDates, string userId, string userName, string Training, string TrainingPeriod);
        string[] GetOperationId(string UserId);
        IEnumerable<SelectListItem> PopulateDivision(string OpId);
        IEnumerable<SelectListItem> PopulateDepartment(string OpId, string DivId = "0");
        IEnumerable<SelectListItem> PopulateSection(string OpId, string DivId = "0", string DeptId = "0");
        IEnumerable<SelectListItem> PopulateAssociates(string UserId, string OpId, string DivId = "0", string DeptId = "0", string SecId = "0");
        DataTable EmployeeDesignation(string UserID = "0");
        List<TrngSummaryCountViewModel> Search(string UserId, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection);
        List<InternalTrngViewModel> GetAttendedAsso(string UserId, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection, string strTrainingId, string strType);
        List<TodaysTrainingViewModel> SearchTodayTrng(string UserId, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection);
        List<ExternalTrngViewModel> SearchextrnlTrng(string UserId, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection);
        List<EvaluationListViewModel> FillPendingGridView(string UserId);
        List<EvaluationListViewModel> FillCompletedGridView(string UserId);
        List<TrainingQuetionAnsViewModel> GetTrainingQuestion(string trngId);
        List<TrainingQuetionAnsViewModel> GetEvalAnsUserHand(string batchid);
        string InsertEvaluationfrm(string strBatchID, string strEmpCode, string strlogincode, string strXml, string strGrade);
        RequestHistoryDetailsViewModel FillRequestDetails(string RequestID);
        ReScheduleRequestViewModel GetReScheduleRequestDetails(string strEmpCode, string strBatchID);
        int UpdateTrngReSdlReqByApp(string strRemarks, string strRequestID, string strStatus);
        TrainningDetailsViewModel GetBatchDetail(string strBatchID);
        string CancelTrainigReScheduleRequest(string strRequestID, string strCancelRemarks, string strBy);
        int UpdateRescheduleReqRec(string strRemarks, string strRequestID, string strAvailableDate, string strStatus, string strRecAuth, string strAppAuth);
        TrainningDetailsViewModel GetOldAttendedTrainings(string strEmpCode, string strTrgTransID);
    }
}
