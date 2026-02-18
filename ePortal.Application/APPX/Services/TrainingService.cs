using ePortal.Application.APPX.Contracts;
using ePortal.DomainClasses;
using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Admin.Services;
using ePortal.Shared;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.Training;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spire.Additions.Xps.Schema;
using Spire.Pdf;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ePortal.Application.APPX.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly DataTableConverter _dtConvert = new();
        private readonly IHrIntrnlTraining objHrTraining;
        private readonly ITrainingCalendar objcal;
        private readonly ISection objSection;
        private readonly IDivision objDiv;
        private readonly IDepartment objDept;
        private readonly ISearchEmp objDetail;
        private readonly IVicePresident objVp;
        private readonly ITraining objTraining;
        public TrainingService(IHrIntrnlTraining _objHrTraining, ITrainingCalendar _objcal, ISection _objSection, IDivision _objDiv, IDepartment _objDept, ISearchEmp _objDetail, IVicePresident _objVp, ITraining _objTraining
            )
        {
            objHrTraining = _objHrTraining;
            objcal = _objcal;
            objSection = _objSection;
            objDiv = _objDiv;
            objDept = _objDept;
            objDetail = _objDetail;
            objVp = _objVp;
            objTraining = _objTraining;
        }
        public IEnumerable<SelectListItem> GetAllTraining()
        {
            var dt = objcal.GetAllTraining();
            if (dt == null || dt.Rows.Count == 0) return new List<SelectListItem>();
            return dt.AsSelectList("HRTRAININGID", "TRAININGDETAIL", true, "-All-", "0");
        }
        public List<MyTrainingViewModel> GetMyTraining(string userid, string trainning, string trainningStatus)
        {
            DataTable dt = objHrTraining.GetMyTrainings(userid, trainning, trainningStatus);
            if (dt == null || dt.Rows.Count == 0) return new List<MyTrainingViewModel>();
            var data = _dtConvert.TableToList<MyTrainingViewModel>(dt);
            return data;
        }
        public TrainningDetailsViewModel GetTrainingDetail(string userid, string batchId)
        {
            DataTable dt = objHrTraining.GetTrainingDetail(userid, batchId);
            if (dt == null || dt.Rows.Count == 0) return new TrainningDetailsViewModel();
            var data = _dtConvert.TableToList<TrainningDetailsViewModel>(dt).FirstOrDefault();
            return data ?? new TrainningDetailsViewModel();
        }
        public FeedbackViewModel GetFeedbackDetails(string batchId, string userid)
        {
            DataTable objdt = objcal.GetFeedbackDetails(batchId, userid);

            if (objdt == null || objdt.Rows.Count == 0) return new FeedbackViewModel();

            var data = new FeedbackViewModel
            {
                Rdolst1 = Convert.ToString(objdt.Rows[0][4]),
                Rdolst2 = Convert.ToString(objdt.Rows[1][4]),
                Rdolst3 = Convert.ToString(objdt.Rows[2][4]),
                Rdolst4 = Convert.ToString(objdt.Rows[3][4]),
                Rdolst5 = Convert.ToString(objdt.Rows[4][4]),
                Rdolst6 = Convert.ToString(objdt.Rows[5][4]),
                Rdolst7_1 = Convert.ToString(objdt.Rows[6][4]),
                Rdolst7_2 = Convert.ToString(objdt.Rows[7][5]),
                Rdolst8_1 = Convert.ToString(objdt.Rows[8][4]),
                Rdolst8_2 = Convert.ToString(objdt.Rows[9][5]),
                Rdolst9_1 = Convert.ToString(objdt.Rows[10][4]),
                Rdolst9_2 = Convert.ToString(objdt.Rows[11][5]),
                Rdolst10_1 = Convert.ToString(objdt.Rows[12][4]),
                Rdolst10_2 = Convert.ToString(objdt.Rows[13][5]),
                Rdolst11_1 = Convert.ToString(objdt.Rows[14][4]),
                Rdolst11_2 = Convert.ToString(objdt.Rows[15][5]),
                Rdolst12 = Convert.ToString(objdt.Rows[16][4]),
                Rdolst13 = Convert.ToString(objdt.Rows[17][4]),
                Rdolst14 = Convert.ToString(objdt.Rows[18][4]),
                TxtLike = Convert.ToString(objdt.Rows[0][0]),
                TxtTopics = Convert.ToString(objdt.Rows[0][1]),
                TxtFeedback = Convert.ToString(objdt.Rows[0][2]),
                TxtImprovementFeedback = Convert.ToString(objdt.Rows[0][6])

            };
            return data;
        }

        public string SaveFeedbackForm(string batchId, string UserId, FeedbackViewModel feedback)
        {
            var data = objcal.InsertFeedbackfrm(batchId, UserId, feedback.Rdolst1, feedback.Rdolst2, feedback.Rdolst3,
                feedback.Rdolst4, feedback.Rdolst5, feedback.Rdolst6, feedback.Rdolst7_1, feedback.Rdolst7_2,
                feedback.Rdolst8_1, feedback.Rdolst8_2, feedback.Rdolst9_1, feedback.Rdolst9_2, feedback.Rdolst10_1,
                feedback.Rdolst10_2, feedback.Rdolst11_1, feedback.Rdolst11_2, feedback.Rdolst12, feedback.Rdolst13,
                feedback.Rdolst14, feedback.TxtLike, feedback.TxtTopics, feedback.TxtFeedback,
                feedback.TxtImprovementFeedback);
            return data;
        }
        public string GetRecAuthAppAuth(string UserId)
        {
            var data = objHrTraining.GetRecAuthAppAuth(UserId);
            if (data == null) return "";
            return data;
        }
        public IEnumerable<SelectListItem> PopulateAvailableTrainingDates(string BatchID)
        {
            DataTable dt = new DataTable();
            dt = objHrTraining.GetAvailableTrainingDates(BatchID);
            if (dt == null || dt.Rows.Count <= 0)
                return dt.AsSelectList("", "", true, "No Dates Available", "0");
            return dt.AsSelectList("HRTRAININGTRANSACTIONID", "AVAIL_DATES", false, "", "0");
        }
        public int InsertTrainingReSchedule(string RecAuth, string AppAuth, string batchid, string Reason, string AvailableDates, string userId, string userName, string Training, string TrainingPeriod)
        {
            var data = objHrTraining.InsertTrainingReSchedule(RecAuth, AppAuth, userId, batchid, Reason, AvailableDates);
            if (data.ToString() == "1")
            {
                if (AvailableDates == "0")
                {
                    SendEmail(userId, userName, RecAuth, Training, TrainingPeriod, Reason);
                }
            }
            return data;
        }

        private void SendEmail(string UserId, string UserName, string strRecAuth, string strTrainingName, string strTrainingDate, string strReason)
        {
            string strEmailId = string.Empty;
            DataTable objdt = objHrTraining.GetRecMailId(strRecAuth);
            strEmailId = Convert.ToString(objdt.Rows[0]["emailid"]);

            commanEmail sendMail;
            string strSubject = string.Empty;
            string strBody = string.Empty;

            sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            if (serverpath.isTestServer())
                sendMail.MailTo = "avinash.singh@honda.hmsi.in"; //serverpath.getTestEMail();
            else
                sendMail.MailTo = strEmailId;

            strSubject = "Training Re-Schedule Request by - " + UserName + " [ " + UserId + " ] ";
            strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                             "</td></tr>" +
                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                             "<b>Training Re-Schedule Request</b></td></tr><tr height=150><td colspan=2>" +
                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                             "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear Sir/Madam,</b><br />" +
                             "<br />" +
                             UserName + " (Employee Code: " + UserId + ") has requested for Re-Schedule of his/her Training <b>" + strTrainingName + "</b>  Currently Scheduled on  <b>" + strTrainingDate + "</b><br />" +
                             "<br /><b>Reason:</b> " + strReason +
                             "<br /></td></tr><tr valign=bottom> " +
                             "<td colspan=2><b>Thank You <br /> <br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                             "</tr></table></td></tr><tr><td colspan=2><img src=" + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                             "</tr></table> ";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
                //ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ALERT1", "alertMe('1')", true);
            }
            catch (Exception ex)
            {
                // Response.Write("Exception Occured:   " + ex);
            }
            finally
            {

                // Response.Write("Your E-mail has been sent sucessfully");
            }
        }

        public string[] GetOperationId(string UserId)
        {
            var str = objHrTraining.GetOperation(UserId);
            if (string.IsNullOrWhiteSpace(str))
                return null;
            var result = str.Split(new Char[] { '#' });
            return result;
        }

        public IEnumerable<SelectListItem> PopulateDivision(string OpId)
        {
            //ddlDivision.Items.Clear();
            var objDS = objDetail.get_AllDiv(Convert.ToInt32(OpId));
            if (objDS == null || objDS.Tables[0].Rows.Count == 0) return new List<SelectListItem>();
            var ddl = objDS.AsSelectList_DS("divisionid", "division", true, "--All--", "0");
            return ddl;
        }

        public IEnumerable<SelectListItem> PopulateDepartment(string OpId, string DivId = "0")
        {
            //ddlDepartment.Items.Clear();
            var objDS = objDetail.get_AllDept(Convert.ToInt32(OpId), Convert.ToInt32(DivId));
            if (objDS == null || objDS.Tables[0].Rows.Count == 0) return new List<SelectListItem>();
            var ddl = objDS.AsSelectList_DS("departmentid", "department", true, "--All--", "0");
            return ddl;
        }

        public IEnumerable<SelectListItem> PopulateSection(string OpId, string DivId = "0", string DeptId = "0")
        {
            //ddlSection.Items.Clear();
            //strDivId = ddlDivision.SelectedItem.Value;
            //strDeptId = ddlDepartment.SelectedItem.Value;

            var objDS = objDetail.get_AllSection(Convert.ToInt32(OpId), Convert.ToInt32(DivId), Convert.ToInt32(DeptId));
            if (objDS == null || objDS.Tables[0].Rows.Count == 0) return new List<SelectListItem>();
            var ddl = objDS.AsSelectList_DS("sectionid", "section", true, "--All--", "0");
            return ddl;
        }

        public IEnumerable<SelectListItem> PopulateAssociates(string UserId, string OpId, string DivId = "0", string DeptId = "0", string SecId = "0")
        {
            var dt = objHrTraining.GetAssociateList(UserId, OpId, DivId, DeptId, SecId);
            if (dt == null || dt.Rows.Count == 0) return new List<SelectListItem>();
            var ddl = dt.AsSelectList("ADEMPCODE", "EMPNAME", true, "--All--", "0");
            return ddl;

        }
        public DataTable EmployeeDesignation(string UserID)
        {
            var odt = objTraining.EmployeeDesignation(UserID);
            if (odt.Rows.Count > 0)
                return odt;
            return new DataTable();
        }

        public List<TrngSummaryCountViewModel> Search(string UserId, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection)
        {
            try
            {
                var dt1 = objHrTraining.EmployeesTrainings(UserId, strEmpCode, strOperation, strDivision, strDepartment, strSection, "", "");
                //if (dt == null || dt.Rows.Count == 0) return new TrainningDetailsViewModel();
                var data1 = _dtConvert.TableToList<TrngSummaryCountViewModel>(dt1);

                //var dt2 = objHrTraining.GetExtrnlTrngsOfEmp(UserId, strEmpCode, strOperation, strDivision, strDepartment, strSection);
                //var data2 = _dtConvert.TableToList<ExternalTrngViewModel>(dt2);

                //var result = Tuple.Create(data1, data2);
                return data1;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                throw;
            }
        }

        public List<ExternalTrngViewModel> SearchextrnlTrng(string UserId, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection)
        {
            try
            {
                DataTable dt = new DataTable();
                // get external training of associates
                dt = objHrTraining.GetExtrnlTrngsOfEmp(UserId, strEmpCode, strOperation, strDivision, strDepartment, strSection);
                if (dt == null || dt.Rows.Count == 0) return new List<ExternalTrngViewModel>();

                var data = _dtConvert.TableToList<ExternalTrngViewModel>(dt);
                return data;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                throw;
            }
        }
        public List<TodaysTrainingViewModel> SearchTodayTrng(string UserId, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection)
        {
            try
            {
                DataTable dt = new DataTable();
                // get associate's today training list
                dt = objHrTraining.GetTodayTrngsOfEmp(UserId, strEmpCode, strOperation, strDivision, strDepartment, strSection);
                if (dt == null || dt.Rows.Count == 0) return new List<TodaysTrainingViewModel>();

                var data = _dtConvert.TableToList<TodaysTrainingViewModel>(dt);
                return data;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                throw;
            }
        }
        public List<InternalTrngViewModel> GetAttendedAsso(string UserId, string strEmpCode, string strOperation, string strDivision, string strDepartment, string strSection, string strTrainingId, string strType)
        {
            DataTable dt = new DataTable();

            try
            {
                dt = objHrTraining.GetSchedAttdAsso(UserId, strEmpCode, strOperation, strDivision, strDepartment, strSection, "", "", "", strTrainingId, strType);
                if (dt == null || dt.Rows.Count == 0) return new List<InternalTrngViewModel>();

                var data = _dtConvert.TableToList<InternalTrngViewModel>(dt);
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<EvaluationListViewModel> FillPendingGridView(string UserId)
        {
            DataTable dt = new DataTable();
            dt = objcal.GetEvaluationList(UserId);
            DataView dv = new DataView(dt);
            dv.RowFilter = "ISEVALUATIONFILLED='0'";
            var filerData = dv.ToTable();
            if (filerData == null || filerData.Rows.Count == 0) return new List<EvaluationListViewModel>();

            var data = _dtConvert.TableToList<EvaluationListViewModel>(filerData);
            return data;
        }
        public List<EvaluationListViewModel> FillCompletedGridView(string UserId)
        {
            DataTable dt = new DataTable();
            dt = objcal.GetEvaluationList(UserId);
            DataView dv = new DataView(dt);
            dv.RowFilter = "ISEVALUATIONFILLED='1'";
            var filerData = dv.ToTable();
            if (filerData == null || filerData.Rows.Count == 0) return new List<EvaluationListViewModel>();

            var data = _dtConvert.TableToList<EvaluationListViewModel>(filerData);
            return data;
        }

        public FeedbackViewModel TrainingEvaluation(string batchid, string empcode)
        {
            //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "none");
            //strBatchID = (Request.QueryString["batchid"].ToString());
            //strEmpCode = (Request.QueryString["empcode"].ToString());
            //objdt = obltrn.GetTrainingDetail(strEmpCode, strBatchID);
            //string TrainingId = string.Empty;

            //if (objdt.Rows.Count > 0)
            //{
            //    dr = objdt.Rows[0];

            //    ltltname.Text = Convert.ToString(dr["EMPNAME"]);
            //    ltltecode.Text = Convert.ToString(dr["ADEMPCODE"]);
            //    ltlTraining.Text = Convert.ToString(dr["TRAINING"]);
            //    ltlVenue.Text = Convert.ToString(dr["TRAININGVENUE"]);
            //    ltlDate.Text = Convert.ToString(dr["TRAININGPERIOD"]);
            //    ltlTrainer1.Text = dr["TRAINER1"].ToString();
            //    ltlTrainer2.Text = dr["TRAINER2"].ToString();
            //    ltlTrngTime.Text = dr["TRAININGTIME"].ToString();
            //    TrainingId = dr["TRAININGID"].ToString();
            //    ltlTrObjective.Text = dr["TRAININGOBJECTIVE"].ToString();
            //    //ViewState["TrainingID"] = TrainingId;
            //}
            var data = GetTrainingDetail(empcode, batchid);
            //strTrainer1 = ltlTrainer1.Text;
            //strTrainer2 = ltlTrainer2.Text;

            //if (Request.QueryString["status"].ToString() == "0")
            //{
            //    btnSubmit.Visible = true;
            //    btnReset.Visible = true;
            //    gridEvalution.Visible = true;
            //    ANSWERGRID.Visible = false;
            //    DataTable dt = obltrn.get_TrainingQuestion(TrainingId);
            //    gridEvalution.DataSource = dt;
            //    gridEvalution.DataBind();
            //}
            //else if (Request.QueryString["status"].ToString() == "1")
            //{
            //    btnSubmit.Visible = false;
            //    btnReset.Visible = false;
            //    gridEvalution.Visible = false;
            //    ANSWERGRID.Visible = true;
            //    string batchid = Request.QueryString["batchid"].ToString() + "#" + Request.QueryString["empcode"].ToString();
            //    DataSet ds = objcal.GET_EVALANSWERINUSERHAND(batchid);
            //    ANSWERGRID.DataSource = ds.Tables[0];
            //    ANSWERGRID.DataBind();
            //}
            return new FeedbackViewModel();
        }

        public List<TrainingQuetionAnsViewModel> GetTrainingQuestion(string trngId)
        {
            DataTable dt = objHrTraining.get_TrainingQuestion(trngId);
            if (dt == null || dt.Rows.Count == 0) return new List<TrainingQuetionAnsViewModel>();

            var data = _dtConvert.TableToList<TrainingQuetionAnsViewModel>(dt);
            return data;
        }
        public List<TrainingQuetionAnsViewModel> GetEvalAnsUserHand(string batchid)
        {
            DataSet ds = objcal.GET_EVALANSWERINUSERHAND(batchid);
            var dt = ds.Tables[0];
            if (dt == null || dt.Rows.Count == 0) return new List<TrainingQuetionAnsViewModel>();

            var data = _dtConvert.TableToList<TrainingQuetionAnsViewModel>(dt);
            return data;
        }
        public string InsertEvaluationfrm(string strBatchID, string strEmpCode, string strlogincode, string strXml, string strGrade)
        {
            var data = objcal.InsertEvaluationfrm(strBatchID, strEmpCode, strlogincode, strXml, strGrade);
            return data ?? "";
        }
        public RequestHistoryDetailsViewModel FillRequestDetails(string RequestID)
        {
            var dt = objHrTraining.GetRequestHistory(RequestID);
            if (dt == null || dt.Rows.Count == 0) return new RequestHistoryDetailsViewModel();

            var data = _dtConvert.TableToList<RequestHistoryDetailsViewModel>(dt);

            return data.FirstOrDefault();
        }

        public ReScheduleRequestViewModel GetReScheduleRequestDetails(string strEmpCode, string strBatchID)
        {
            var dt = objHrTraining.GetReScheduleRequestDetails(strEmpCode, strBatchID);

            if (dt == null || dt.Rows.Count == 0) return new ReScheduleRequestViewModel();

            var data = _dtConvert.TableToList<ReScheduleRequestViewModel>(dt);

            return data.FirstOrDefault();
        }
        public int UpdateTrngReSdlReqByApp(string strRemarks, string strRequestID, string strStatus)
        {
            var intResult = objHrTraining.UpdateTrngReSdlReqByApp(strRemarks, strRequestID, strStatus);
            return intResult;
        }

        public TrainningDetailsViewModel GetBatchDetail(string strBatchID)
        {
            var dt = objcal.GetBatchDetail(strBatchID);
            if (dt == null || dt.Rows.Count == 0) return new TrainningDetailsViewModel();
            var data = _dtConvert.TableToList<TrainningDetailsViewModel>(dt);
            return data.FirstOrDefault();
        }

        public string CancelTrainigReScheduleRequest(string strRequestID, string strCancelRemarks, string strBy)
        {
            var data = objTraining.CancelTrainigReScheduleRequest(strRequestID, strCancelRemarks, strBy);
            if (string.IsNullOrWhiteSpace(data)) return "";
            return data;
        }
        public int UpdateRescheduleReqRec(string strRemarks, string strRequestID, string strAvailableDate, string strStatus, string strRecAuth, string strAppAuth)
        {
            var data = objHrTraining.UpdateTrngReSdlReqByRec(strRemarks, strAppAuth, strRequestID, strAvailableDate, strStatus, strRecAuth, strAppAuth);
            return data;
        }

        public TrainningDetailsViewModel GetOldAttendedTrainings(string strEmpCode, string strTrgTransID)
        {
            var dt = objTraining.GetOldAttendedTrainings(strEmpCode, strTrgTransID);
            if (dt == null || dt.Rows.Count == 0) return new TrainningDetailsViewModel();

            var data = _dtConvert.TableToList<TrainningDetailsViewModel>(dt);

            return data.FirstOrDefault();
        }
    }
}
