using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
    public interface IInformationSecurity
    {

        DataTable GetInformationSecurityProc_Formats();
        DataTable GetInformationSecurityPolicy_Guidelines();
        DataTable GetInformationSecurityPolicy_Awereness();
        DataTable GetInformationSecurityQuiz_Question(int SurveyNumber);
        DataTable GetInformationSecurityQuiz_QuestionFixed(int SurveyNumber, long ecode);
        DataTable GetInformationSecurityQuiz_Answer(int QuesID);
        string SaveQuizData(string UserID, string QuestingID, string AnswerID, string strremark);
        string SaveSurveyremarks(string UserID, string SurveyID, string strremark);
        string Surveyskip(string UserID, string SurveyID, string strskipcount);
        int GetSurveyskipCount(int SurveyNumber, string UserID);
        DataTable CheckInformationSecurityQuiz_Answer(int EMPCODE, int SurveyID);
        DataTable GetInfoSecSurveyRpt(string strDesignationId, string strDivisionId, string strDepartmentId,
                                                     string strSectionId, string strOperationId, string strAssoEmpCode,
                                                     string strAssoEmpName, string SurveyNumber);

        DataTable GetEMPWISESurveyRpt(string strDesignationId, string strDivisionId, string strDepartmentId,
                                                   string strSectionId, string strOperationId, string strAssoEmpCode,
                                                   string strAssoEmpName, string SurveyNumber);

        Boolean IsQuizCompleted(string UserID, int SurveyNumber);
        Boolean IsQuizClearByAssociate(string UserID, int SurveyNumber);
        Boolean IsSelfdeclarationCompleted(string UserID);
        DataTable GetInfoSecurityOvelAllResult(string strDesignationId,
                                                    string strDivisionId, string strDepartmentId, string strSectionId, string strOperationId,
                                                    string strAssoEmpCode, string strAssoEmpName, string SurveyNumber);
        DataTable GetActiveSurvey();
        DataTable GetActiveSurveyWithTM(int EmpCode);

        int GetActiveSurvey_Reteststatus();
        int GetQuestionCount(int SurveyNumber);

        string GetSurveyDescription(int SurveyNumber);
        DataTable GetSurveyList(string UserID);
        DataTable GetALLSurveyList();
        DataTable GetPartSurveyList(string surveyid);
        DataTable GetQuesDtls(string quesid);

        DataTable GetAnsDtls(string ansid);
        DataTable GetSurveyResult(string SurveyID, string EmpCode);
        int GetSurveyResultInPercentage(string SurveyNumber, string EmpCode);
        string AddEditSurveyDtls(string userid, string surveyid, string title, string descrip, string status, string fromdate, string todate, string retest, string stremail, string strfcomment);
        string AddEditSurveyQues(string userID, string sid, string quesid, string desc, string status);

        string AddEditSurveyAns(string userID, string ansid, string quesid, string desc, string status, string IsCorrect);
        string UPDATEQuizData(string UserID, string QuestingID, string AnswerID);
        DataTable GetEmail();
        DataTable GetALLANSList(string strqus);
        DataTable GetALLQUSList(string strsurvey);
        DataTable GetFAILASSO_EMAIL(string strsurvey, string strcutoff);

        Boolean ISSAPPartSelectionQuizFilled(string strUserID);
        string SAPPartnerSelectionQuizData(string strQuestingID, string strUserID, string stracc_ans, string stribm_ans, string strhp_ans, string strcsc_ans, string strcap_ans);
        DataSet SAPPARTNERREPORT();
        DataTable Get_QUSReport(int QuesID);
        DataTable Get_UserWiseReport();
        DataTable Get_WorstFeedbackReport(int VpID);
        DataTable Get_Operation(int VpID);
        string SaveITSelfDeclaration(string UserID);
        string AddITSelftrequest(string strDateFrom, string UserID, string strDescription, string status, string Attachment);
        DataTable GetALLQUSList();
        DataTable GetSISSELFRECSList(String ID);
        string UpdateITSelftrequest(string strDateFrom, string UserID, string strDescription, string status, string Attachment, string ITSELFDECLCONDITIONID, string FilePath);


        int CheckStatus();
        int CheckStatusUpdationTime(String ID);
        DataTable GetResult_Question(int SurveyNumber);
        string GetSurveyErrorEmailId();
        DataTable GetQuesRpt(string strSurveyHeaderID);


    }
}
