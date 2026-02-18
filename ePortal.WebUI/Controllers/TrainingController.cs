using ePortal.Application.APPX.Contracts;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.Training;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Reflection;
using System.Text;

namespace ePortal.WebUI.Controllers
{
    [SessionTimeout]
    [CSPFilter]
    public class TrainingController : Controller
    {
        #region Private Variables

        private readonly ITrainingService _service;
        private readonly ILogger<TrainingController> _logger;
        private readonly string _userId;
        private readonly string _userName;
        private readonly Employee_Details LoginEmpDetails;

        #endregion Private Variables

        #region Constructor
        public TrainingController(
            ITrainingService service,
            ICommonFunctions _commn,
            ISessionService sessionService,
            ILogger<TrainingController> logger
            )
        {
            _service = service;
            _logger = logger;
            _userId = sessionService.Get<string>("userID").ToString();
            _userName = sessionService.Get<string>("userName").ToString();
            LoginEmpDetails = sessionService.Get<Employee_Details>("Employee");
        }
        #endregion Constructor

        #region MyTrainings
        public async Task<IActionResult> MyTrainings()
        {
            try
            {
                ViewBag.isSaved = TempData["isSaved"]?.ToString();
                ViewBag.AllTraining = _service.GetAllTraining();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Search(string trainning, string trainningStatus)
        {
            List<MyTrainingViewModel> data = new();
            try
            {
                data = _service.GetMyTraining(_userId, trainning, trainningStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return PartialView("_GetMyTrainings", data);
        }

        #endregion MyTrainings

        #region FeedbackForm
        public async Task<IActionResult> FeedbackForm(string batchId, string IsFilled)
        {
            var TrainingFeedBackDetails = new TrainingFeedbackViewModel();
            try
            {
                var BatchID = WebUtility.UrlDecode(Encryption.Decrypt(batchId.ToString()));
                var _isFilled = WebUtility.UrlDecode(Encryption.Decrypt(IsFilled.ToString()));
                ViewBag.IsFilled = _isFilled;
                ViewBag.BatchID = BatchID;

                TrainingFeedBackDetails.TrainingDetails = _service.GetTrainingDetail(_userId, BatchID);
                if (_isFilled == "1")
                {
                    TrainingFeedBackDetails.FeedbackDetails = _service.GetFeedbackDetails(BatchID, _userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View(TrainingFeedBackDetails);
        }
        [HttpPost]
        public async Task<IActionResult> SaveFeedbackForm(string batchId, TrainingFeedbackViewModel obj)
        {
            var result = "0";
            try
            {
                result = _service.SaveFeedbackForm(batchId, _userId, obj?.FeedbackDetails);
                TempData["isSaved"] = result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            if (result == "1")
                return RedirectToAction("MyTrainings");
            return RedirectToAction("FeedbackForm", new { batchId, IsFilled = "0" });
        }
        #endregion FeedbackForm

        #region RescheduleRequest

        public async Task<IActionResult> RescheduleRequest(string batchid, string cnt)
        {
            var TrainingDetails = new TrainningDetailsViewModel();
            try
            {
                var BatchID = WebUtility.UrlDecode(Encryption.Decrypt((Convert.ToString(batchid))));
                var count = WebUtility.UrlDecode(Encryption.Decrypt(Convert.ToString(cnt)));
                var AvailableDates = new List<SelectListItem>();
                TrainingDetails = _service.GetTrainingDetail(_userId, BatchID);

                var Result = _service.GetRecAuthAppAuth(_userId);
                var SplitResult = Result.Split(new Char[] { '#' });

                var RecAuth = SplitResult[0].ToString();
                var AppAuth = SplitResult[1].ToString();

                if (RecAuth == "")
                {
                    AvailableDates = _service.PopulateAvailableTrainingDates(BatchID).ToList();
                }
                else
                {
                    AvailableDates.Add(new SelectListItem() { Text = "0", Value = "0" });
                }
                ViewBag.BatchID = BatchID;
                ViewBag.RescheduleCount = count;
                TempData["RescheduleCount"] = count;
                ViewBag.RecAuth = RecAuth;
                ViewBag.AvailableDates = AvailableDates;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View(TrainingDetails);
        }
        [HttpPost]
        public async Task<IActionResult> SaveRescheduleRequest(string batchid, string AvailableDates, string Reason, string Training, string TrainingPeriod)
        {
            int intResult = 0;
            try
            {
                var Result = _service.GetRecAuthAppAuth(_userId);
                var SplitResult = Result.Split(new Char[] { '#' });

                var RecAuth = SplitResult[0].ToString();
                var AppAuth = SplitResult[1].ToString();

                intResult = _service.InsertTrainingReSchedule(RecAuth, AppAuth, batchid, Reason, AvailableDates, _userId, _userName, Training, TrainingPeriod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            if (intResult.ToString() == "1")
                return RedirectToAction("MyTrainings");
            var count = TempData["RescheduleCount"].ToString();
            return RedirectToAction("RescheduleRequest", new { batchid, cnt = count });
        }

        #endregion RescheduleRequest

        #region MyAssoTrainings
        public async Task<IActionResult> MyAssoTrainings()
        {
            MyAssoTrainingViewModel model = new();
            try
            {
                var emp = _service.EmployeeDesignation(_userId);
                var strOperation = _service.GetOperationId(_userId);

                var OpId = strOperation[0].ToString();

                //For Selection
                model.SecCode = Convert.ToString(emp.Rows[0]["ADSECTIONID"]);
                model.DeptCode = Convert.ToString(emp.Rows[0]["ADDEPARTMENTID"]);
                model.DivCode = Convert.ToString(emp.Rows[0]["ADDIVISIONID"]);
                model.OPCode = Convert.ToString(emp.Rows[0]["ADVPID"]);
                model.Desig = Convert.ToString(emp.Rows[0]["EMPPOS"]).Trim();
                //For Selection
                model.OperationId = OpId;
                model.OperationText = strOperation[1].ToString();
                model.Divisions = _service.PopulateDivision(OpId);
                model.Departments = _service.PopulateDepartment(OpId, model.DivCode);
                model.Sections = _service.PopulateSection(OpId, model.DivCode, model.DeptCode);
                model.Associates = _service.PopulateAssociates(_userId, OpId, model.DivCode, model.DeptCode, model.SecCode);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View(model);
        }

        public async Task<IActionResult> MyAssoTrainingsSearch(string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection)
        {
            MyAssoTrainingViewModel model = new();
            try
            {
                model.SummaryCount = _service.Search(_userId, strEmpCode ?? "0", strOperation ?? "0", strDivision ?? "0", strDepartment ?? "0", strSection ?? "0");
                //model.SummaryCount = count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            try
            {
                model.TodaysTraining = _service.SearchTodayTrng(_userId, strEmpCode ?? "0", strOperation ?? "0", strDivision ?? "0", strDepartment ?? "0", strSection ?? "0");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            try
            {
                model.ExternalTrng = _service.SearchextrnlTrng(_userId, strEmpCode, strOperation, strDivision ?? "0", strDepartment ?? "0", strSection ?? "0");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return PartialView("_MyAssoTrainingSearch", model);
        }

        public async Task<IActionResult> GetAttendedAssoPartial(string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection, string strTrainingId, string strType)
        {
            var intrnl = new List<InternalTrngViewModel>();
            try
            {
                intrnl = _service.GetAttendedAsso(_userId, strEmpCode, strOperation, strDivision, strDepartment, strSection, strTrainingId, strType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return PartialView("_AttendScheduleAsso", intrnl);
            //return Json(intrnl);
        }

        public async Task<JsonResult> GetDepartments(string OpId, string DivId)
        {
            var data = new List<SelectListItem>();
            try
            {
                data = _service.PopulateDepartment(OpId, DivId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }
        public async Task<JsonResult> GetSections(string OpId, string DivId, string DeptId)
        {
            var data = new List<SelectListItem>();
            try
            {
                data = _service.PopulateSection(OpId, DivId, DeptId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }
        public async Task<JsonResult> GetAssociates(string OpId, string DivId, string DeptId, string SecId)
        {
            var data = new List<SelectListItem>();
            try
            {
                data = _service.PopulateAssociates(_userId, OpId, DivId, DeptId, SecId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);

        }

        #endregion MyAssoTrainings

        #region TrainingEvaluationList
        public async Task<IActionResult> TrainingEvaluationList()
        {
            try
            {
                ViewBag.Pending = _service.FillPendingGridView(_userId);
                ViewBag.Complete = _service.FillCompletedGridView(_userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View();
        }

        #endregion TrainingEvaluationList

        #region TrainingEvaluation
        public async Task<IActionResult> TrainingEvaluation(string batchid, string empcode, string status, int red)
        {
            TrainingEvaluationViewModel obj = new TrainingEvaluationViewModel();
            try
            {
                obj.TrngDetails = _service.GetTrainingDetail(empcode, batchid);
                //obj.Trainer1 = data.TRAINER1??"";
                //obj.Trainer2 = data.TRAINER2?? "";
                if (status == "0")
                {
                    obj.IsBtnSubbmitVisible = true;
                    obj.IsbtnResetVisible = true;
                    obj.IsgridEvalutionVisible = true;
                    obj.IsANSWERGRIDVisible = false;
                    obj.gridEvalution = _service.GetTrainingQuestion(obj.TrngDetails.TRAININGID.ToString());
                }
                else if (status == "1")
                {
                    obj.IsBtnSubbmitVisible = false;
                    obj.IsbtnResetVisible = false;
                    obj.IsgridEvalutionVisible = false;
                    obj.IsANSWERGRIDVisible = true;
                    //string batchid = Request.QueryString["batchid"].ToString() + "#" + Request.QueryString["empcode"].ToString();
                    obj.ANSWERGRID = _service.GetEvalAnsUserHand($"{batchid}#{empcode}");
                }
                ViewBag.batchid = batchid;
                ViewBag.empcode = empcode;
                ViewBag.red = red;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View(obj);
        }


        [HttpPost]
        public async Task<IActionResult> SubmitEvaluation([FromBody] TrainingEvaluationViewModel model)
        {
            string rtnValue = "0";
            try
            {

                if (model.gridEvalution.Any(q => string.IsNullOrEmpty(q.SCORE1)))
                {
                    return Json(new { success = false, message = "Please select all answers." });
                }

                float avg = (float)model.gridEvalution.Average(q => Convert.ToInt32(q.SCORE1));

                var xml = new StringBuilder();
                xml.Append("<Evalution>");
                foreach (var q in model.gridEvalution)
                {
                    xml.Append("<Right>");
                    xml.Append($"<EvalutionQueID>{q.HRTRAININGQUEID}</EvalutionQueID>");
                    xml.Append($"<HrTrainingQueID>{System.Net.WebUtility.HtmlEncode(q.QUESTION)}</HrTrainingQueID>");
                    xml.Append($"<Score>{q.SCORE1}</Score>");
                    xml.Append("</Right>");
                }
                xml.Append("</Evalution>");

                rtnValue = _service.InsertEvaluationfrm(model.TrngDetails.BATCHID.ToString(), model.TrngDetails.ADEMPCODE, _userId, xml.ToString(), avg.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            if (rtnValue == "1")
            {
                string redirectUrl = model.Redirection == "2" ? "/Training/TrainingEvaluationList" : "/TokenBridge/RedirectToOldApp?target=/ASPXView/ManageApproval.aspx";
                return Json(new { success = true, message = "Information updated successfully", redirectUrl });
            }
            else
            {
                return Json(new { success = false, message = "Insertion failed." });
            }

        }
        #endregion TrainingEvaluation

        #region RequestApplicationDetail

        public async Task<IActionResult> RequestApplicationDetail(string id)
        {
            var data = new RequestHistoryDetailsViewModel();
            try
            {
                data = _service.FillRequestDetails(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }

            return View(data);
        }

        #endregion RequestApplicationDetail

        #region RescheduleReqApp

        public async Task<IActionResult> RescheduleReqApp(string reid)
        {
            var data = new ReScheduleRequestViewModel();
            try
            {
                var reqId = WebUtility.UrlDecode(Encryption.Decrypt((reid)));
                data = _service.GetReScheduleRequestDetails(_userId, reqId);
                ViewBag.Reqid = reqId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View(data);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateRescheduleReqByApp(string strRemarks, string strRequestID, string strStatus, string EmpCode)
        {
            var data = 0;
            try
            {

                //var Result = _service.GetRecAuthAppAuth(EmpCode);
                //var SplitResult = Result.Split(new Char[] { '#' });
                //var RecAuth = SplitResult[0].ToString();
                //var strAppAuth = SplitResult[1].ToString();

                data = _service.UpdateTrngReSdlReqByApp(strRemarks, strRequestID, strStatus);

                //SendEmail(strAppAuth, strStatus);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);

        }
        #endregion RescheduleReqApp

        #region CancelTrainingReScheduleRequest
        public async Task<IActionResult> CancelTrainingReScheduleRequest(string id, string batchid)
        {
            var batchDetails = new TrainningDetailsViewModel();
            var history = new RequestHistoryDetailsViewModel();
            try
            {
                //batchid = "2";
                batchDetails = _service.GetBatchDetail(batchid);
                history = _service.FillRequestDetails(id);
                ViewBag.TRN_ID = batchDetails.HRTRAININGID;
                ViewBag.RequestId = id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            var data = Tuple.Create(batchDetails, history);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitCancelReScheduleRequest(string strRequestID, string strCancelRemarks)
        {
            try
            {
                var data = _service.CancelTrainigReScheduleRequest(strRequestID, strCancelRemarks, _userId);
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return Json("0");
            }
        }

        #endregion CancelTrainingReScheduleRequest

        #region RescheduleReqRec
        public async Task<IActionResult> RescheduleReqRec(string reid)
        {
            var data = new ReScheduleRequestViewModel();
            try
            {
                var reqId = WebUtility.UrlDecode(Encryption.Decrypt((reid)));
                data = _service.GetReScheduleRequestDetails(_userId, reqId);
                ViewBag.AvailableDates = _service.PopulateAvailableTrainingDates(data.BATCHID).ToList();
                ViewBag.ReqId = reqId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRescheduleReqRec(string strRequestID, string strRemarks, string strAvailableDate, string strStatus, string strEmpCode)
        {
            var data = 0;
            try
            {
                var Result = _service.GetRecAuthAppAuth(strEmpCode);
                var SplitResult = Result.Split(new Char[] { '#' });

                var RecAuth = SplitResult[0].ToString();
                var AppAuth = SplitResult[1].ToString();

                data = _service.UpdateRescheduleReqRec(strRemarks, strRequestID, strAvailableDate, strStatus, RecAuth, AppAuth);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            return Json(data);
        }
        #endregion RescheduleReqRec

        #region TrainingDetail

        public async Task<IActionResult> TrainingDetail(string batchid, string transID, string eID, string ename)
        {
            TrainningDetailsViewModel model = new TrainningDetailsViewModel();
            try
            {
                if (batchid == "0" && transID != "0")
                {
                    model = _service.GetOldAttendedTrainings(eID, transID);
                }
                else if (batchid != "0" && transID == "0")
                {
                    model = _service.GetTrainingDetail(eID, batchid);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
            model.EMPNAME = ename;
            return View(model);
        }

        #endregion TrainingDetail
    }
}
