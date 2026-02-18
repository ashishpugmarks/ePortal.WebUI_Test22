using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IHRPMS
    {
        public string GetCurrentPage(Uri uri);
        public DataSet GETCOMPENTENCYHEADER(string compenheaderid, string compenheaderdesc, string status);
        public string INSERTCOMPENTENCYHEADER(string compenheaderdesc, string status, string compenheaderid, string addedby);
        public DataSet GETCOMPENLEVEL(string comlevelid, string comleveldesc, string status, string comheaderid, string empcode, string syki);
        public string INSERTCOMPENTENCYLEVEL(string compenleveldesc, string status, string compenheaderid, string addedby, string compenlevelid);
        public DataTable GEHEADDESIGMAP(string itemid);
        public DataTable GETDESIGNATION();
        public DataTable GEHEADDESIGMAPBYID(string desigid);
        public string INSERTHEADERDESIGNATIONMAPPING(string addedby, string desigid, string XMLSITEIN);
        public string UPDATEHEADERDESIGNATIONMAPPING(string addedby, string desigid, string XMLSITEIN);
        public DataTable GetKiList();
        public string GetKIId();
        public DataTable GetPMSAuthorities(string userID, string SYKI);
        public DataTable GetPMSAuthoritiesMatrix(string userID, string KIID);
        public DataTable GetHROverAllReport(string strKiId, string strDesignationId,
                                        string strDivisionId, string strDepartmentId, string strSectionId, string strOperationId,
                                        string strAssoEmpCode, string strAssoEmpName, string strEligibilityStatus);
        public DataSet GETAMANAGEPMS(string SYKI, string ECODE, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION);
        public DataSet GETGOALSETTINGFORMDATA(string ECODE, string SYKI);
        public string INSERTGOALSETTINGROLE(string role, string ecode, string HRPMSID, string SYKI);
        public string INSERTACTIVITYLIST(string HRPMSID, string GOALID, string ACTIVITYNAME, string ACTIVITYCONTROLITEM, string ACTIVITYTARGET, string ACTIVITYWEIGHTAGE, string ECODE);
        public string DELETEACTIVITY(string ACTIVITYID, string ECODE);
        public string OTHERACTIVITYINSERTED(string ECODE, string weightage, string otheractivityxml, string HRPMSID);
        public string GOALSETTINGUPDATE(string ECODE, string HRPMSID);
        public DataSet GETGOALSETTINGDETAIL(string HRPMSID);
        public string GOALSETTINGSTATUS_UPDATE(string HRPMSID, string BUTTONID, string COMMENT, string ECODE, string TWOWAYCONFIRMATION);
        public string UploadFile(string strEmpCode, string KiID, string AssociateID, string EvaluatorID, string ReviewerID,
                               string DivHeadID, string OPHeadID, string EligilbeStatus, string GSStartDate, string GSEndDate,
                               string EvalGSStartDate, string EvalGSEndDate, string RevGSStartDate, string RevGSEndDate,
                               string FHStartDate, string FHEndDate, string EvalFHStartDate, string EvalFHEndDate,
                               string RevFHStartDate, string RevFHEndDate, string SHStartDate, string SHEndDate,
                               string EvalSHStartDate, string EvalSHEndDate, string RevSHStartDate, string RevSHEndDate,
                               string GSStatus, string FHStatus, string SHStatus);
        public DataSet GETHRPMSREPORT(string SYKI, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION, string DESIGNATION, string ECODE, string ENAME, string SITE);
        public string INSERT_FIRSTHALF_ACTIVITYRESULT(string HRPMSID, string Ecode, string XMLACTIVITY);
        public string INSERT_FIRSTHALF_SENDTOEVALUATOR(string HRPMSID, string ASSOCIATECOMMENT, string ECODE);
        public string FIRSTHALFEVALSUBMIT(string HRPMSID, string BUTTONID, string PARTACOMMENT, string PARTBCOMMENT, string ECODE, string XMLPERFASSESSMENT,
        string XMLCOMPETENCIES, string FLAGAUTH, string FINALTOTALEVALSCORE, string FINALRATING, string FINALFHSCORE, string EVALOTHACTSCORE, string FEEDBACKCOMMENT, string FINALTOTALREVSCORE);
        public DataSet GETCOMPENLEVELWITHSCORE(string HRPMSID, string COMPHEADID);
        public string INSERT_SECONDHALF_ACTIVITYRESULT(string HRPMSID, string Ecode, string XMLACTIVITY);
        public string INSERT_SECONDHALF_SENDTOEVALUATOR(string HRPMSID, string ASSCOMMENT, string ECODE);
        public DataSet GETFHDIVHEADREQDETAILS(string SYKI, string ECODE, string NORMLEVEL);
        public DataSet GETDESIGNATIONWEIGTAGE(string DWMTRANSID, string DESIGNATION, string SYKI);
        public string INSERTDESIGNATIONWEIGHTAGE(string dwntransid, string desigid, string syki, string addedby, string PARTAWEIGHT, string PARTBWEIGHT);
        public DataSet GETPMSDESIGNATIONWEIGHT(string HRPMSID);
        public string FIRSTHALF_TWOWAYCOMM(string HRPMSID, string COMMENT, string ECODE, string twowayconfirmation);
        public string FIRSTHALFREVIEWER_SUBMIT(string HRPMSID, string BUTTONID, string ECODE, string XMLACTIVITY, string XMLCOMPETENCY, string FINALTOTALREVSCORE, string FINALRATING, string FINALFHSCORE, string FEEDBACKCOMMENT, string REVOTHACTSCORE);
        public string SECONDHALFEVALSUBMIT(string HRPMSID, string BUTTONID, string EVALOTHACTSCORE, string PARTAEVALCOMMENT, string XMLACTIVITY,
    string XMLCOMPETENCY, string PARTBEVALCOMMENT, string EVALFEEDBACKCOMMENT, string FLAGAUTH, string FINALTOTALEVALSCORE, string FINALRATING, string FINALSHSCORE, string ECODE, string FINALTOTALREVSCORE);
        public string SECONDHALFREVIEWER_SUBMIT(string HRPMSID, string BUTTONID, string ECODE, string XMLACTIVITY, string REVOTHACTSCORE,
        string XMLCOMPETENCY, string REVFEEDBACKCOMMENT, string FINALTOTALREVSCORE, string FINALRATING, string FINALSHSCORE);
        public string SECONDHALFS_SELF_TWOWAYCOMM(string HRPMSID, string COMMENT, string ECODE, string twowayconfirmation);
        public string NORMRATING_SUBMIT(string PMSID, string BUTTONID, string RATING, string ECODE);

        public string NORMALIZATION_FINALSUBMIT(string BUTTONID, string SYKI, string ECODE);
        public DataSet GETNORMRATINGDETAIL(string syki);
        public string INSERTRATINGNORM(string xmlratlist, string SYKI, string ADDEDBY);
        public DataSet GETKIOPERDIVDEPTSEC(string OPERATION, string DIVISION, string DEPARTMENT);
        public string FHNORM_REMARKSSUBMIT(string SUBMITBY, string PMSID, string REMARK, string ECODE);
        public DataSet GET_NORMALIZATION_DIVDEPTSEC(string SYKI, string ECODE, string NORMTYPE, string NORMLEVEL);
        public int CHECK_SUBMITBTNENABLED(DataSet ds, string status, string Startdate, string EndDate);
        public DataSet GETFHOPERATINGHEADREQDETAILS(string SYKI, string ECODE, string NORMLEVEL);
        public DataSet GETHRPMSNORMALIZATIONREPORT(string SYKI, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION, string DESIGNATION, string ECODE, string ENAME, string SITE, string RATING, string DIVHEADECODE, string OPHEADECODE);
        public DataSet GETDIVHEAD_OPHEAD(string SYKI);
        public string UploadHRRating(string ADEDDBY, string SYKI, string ECODE, string HRRATING);
        public string RemoveSpecialCharacters(string str);
        public DataTable NORMALIZATIONPERIOD_GET(string strki);
        public string NORMALIZATIONPERIOD_SET(string strki, string strdivfhs, string strdivfhe, string stropfhs, string stropfhe, string strdivshs, string strdivshe, string stropshs, string stropshe, string straddedby);
        public string NORMALIZATION_UPLOADEXCEL(string ASSOCIATEID, string DIVHEADRATING, string SUBMITTYPE, string ADDEDBY);
        public void GETEVALREVSCORE(string HRPMSID, out string strFH_EVAL_PARTASCORE, out string strFH_REV_PARTASCORE, out string strSH_EVAL_PARTASCORE, out string strSH_REV_PARTASCORE,
      out string strFH_EVAL_PARTBSCORE, out string strFH_REV_PARTBSCORE, out string strSH_EVAL_PARTBSCORE, out string strSH_REV_PARTABCORE);
        public DataSet GETANNUALRATINGMATRIX(string SYKI, string RATINGID, string STATUS);
        public string INSERTANNUALRATING(string RATINGID, string SYKI, string FIRSTHALF, string SECONDHALF, string ANNUAL, string STATUS, string ADDEDBY);
        public DataSet GETSHDIVHEADREQDETAILS(string SYKI, string ECODE, string NORMLEVEL);
        public DataSet GETSHOPERATINGHEADREQDETAILS(string SYKI, string ECODE, string NORMLEVEL);
        public int CHECK_SHSUBMITBTNENABLED(DataSet ds, string status, string Startdate, string EndDate);
        public string UploadSHHRRating(string ADEDDBY, string SYKI, string ECODE, string HRRATING, string ANNUALRATING, string PROMOTION);
        public DataSet GETHRPMSSHNORMALIZATIONREPORT(string SYKI, string OPERATION, string DIVISION, string DESIGNATION, string ECODE, string ENAME, string SITE, string RATING, string DIVHEADECODE, string OPHEADECODE);
        public string ANNUALRATING_SUBMIT(string PMSID, string BUTTONID, string RATING, string ECODE);
        public string PROMOTION_SUBMIT(string PMSID, string BUTTONID, string PROMOTION, string ECODE);
        public DataSet GETANNUALRATINGREPORT(string SYKI, string OPERATION, string DIVISION, string DESIGNATION, string ECODE, string ENAME);
        public DataSet GETTWOWAYCOMMUNICATIONRPT(string SYKI, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION, string DESIGNATION, string ECODE, string ENAME);
        public string NORMALIZATIONREMARKS_SUBMIT(string PMSID, string BUTTONID, string REMARKS, string ECODE);
        public DataSet GETELIGIBILITYRPT(string SYKI, string OPERATION, string DESIGNATION);
        public DataSet GET_PMSDIVISIONHEAD(string SYKI, string OPHEAD);
        public DataTable GET_Compantancy_Report(string Ki, string strdesg);
        public DataTable GET_Reward_Report(string Ki, string empid);
        public DataTable GETDESIGNATIONForReport();
        public DataSet GETSHDIVPLANDETAILS(string ECODE, string SYKI);

    }
}
