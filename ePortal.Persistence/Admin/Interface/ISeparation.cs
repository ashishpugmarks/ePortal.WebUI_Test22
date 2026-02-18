using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface ISeparation
    {
        DataTable HRDDetail(string strecode);
        DataTable ExcelExport(string ECODE, string STATUS);
        DataTable HRSPAPPROVAL(string strecode, string strlevel);
        DataTable HRSPResigDetail(string strecode, string strresigid);
        DataTable HRSPAcceptanceDetail(string strecode, string strresigid);
        DataTable HRSPApprovalDetail(string strecode, string strresigid);
        DataTable HRSPpendingReq(string strecode);
        DataTable HRSPpendingApproval(string strecode);
        DataTable HRpendingList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId, string strResigby);
        DataTable HRpendingCancelList(string strappecode, string strecode, string strEmpname, string strresigid, string plantId);
        DataTable AcceptancependingList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId);
        DataTable IRAcceptancependingList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid);
        DataTable ClearancependingList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId);
        string EMPRESIGREQ_Set(string strempcode, string strsubject, string strbody, string strrelievingdate, string strappauth, string strappauthlevel, string straddedby, string latestaddress);
        string UpdateApproval_Set(string strresigid, string strrelievingdate, string strstatus, string strremark, string strappauth, string strappauthlevel, string straddedby);
        string UpdateHRApproval_Set(string strresigid, string strremark, string straddedby, string strrelievingdate);
        string UpdateIRApproval_Set(string strresigid, string strremark, string straddedby, string strrelievingdate);
        string set_resign_authority(string authid, string resignid, string userid);
        DataTable Get_HeaderMasterDetails(string header, string status);
        string INSERT_HEADER_MASTER(int HEADERMASTERID_IN, string description, string addedby, StringBuilder xmlServiceList, string status);
        string Insert_IRCLEARANCEACTIVATION(string RESIGID, string SUBMITBY, string AMOUNT, string status);
        DataTable MANAGECLEARANCE(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId);
        DataTable GET_MANAGECLRFORMDETAILS(object ECODE, string STATUS);
        DataTable GET_MANAGEDPTCLRFORMDETAILS(object ECODE, string STATUS);
        DataTable GET_SUBHEADERCLRDETAILS(string ECODE, string RESIGID);
        string INSERT_CLRANCESUBHEADERDETAILS(string ecode, string ResigID, string SUBHEADERLIST);
        string INSERT_ASSRESIGFORMFILLBYIR(string strecode, string subject, string body, string reldate, string deptreldate, string deptappauth,
        string deptappdate, string divappauth, string divappdate, string opappauth, string opappdate, string submitby, string latestaddress, string ResigType, string HRecode, string Document_IN, string strResignedby, string strISuserAppReq, decimal BasicSalaryAmt);
        DataTable HEADER_MASTER_DEATIL_GET(string header, string status, string headerdesc);
        DataTable SYPLANT_MASTER_DEATIL_GET();
        DataTable GET_HEADER_DETAILS();
        DataTable SUB_HEADER_MASTER_DEATIL_GET(string SUBHEADERID_IN, string status, string HEADERID_IN, string SUBHEADER_IN);
        DataTable Get_SubHeaderauthDetails(string subheader, string status);
        string INSERT_SUB_HEADER_MASTER(int SUBHEADERMASTERID_IN, string header, string description, string addedby, StringBuilder xmlServiceList, string status);
        DataSet GET_EXITINTERVIEW_QUESTIONNAIRE(string STATUSSEC_IN, string STATUSQUS_IN, string STATUSOPT_IN);
        string INSERT_INTERVIEW_ANSWER(string RESIGNATIONID, string SUBMITBY, string INTERVIEWXML);
        DataSet Get_GET_INTERVIEWANSWER(string RESIGNATIONID);
        DataTable Get_Exitinterviewquestionaire(string strecode);
        DataTable Get_HeaderauthDetails(string header, string status);
        string CANCELLATION_RESIGNATION(string RESIGNID, string remarks);
        DataTable LISTPENDINGINTERVIEWQUEST(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId);
        string Final_Hr_Submit_Resignation(string ResigID, string hrremarks, string SUBMITBY);
        string INSERT_DEPT_CLEARANCE(string CONFIDENTIAL_DOC, string OTHER_DOC, string LIBRARY_BOOK, string ITASSETS, string CAMERA, string KEY,
        string TRAVEL_BILL, string SUBMITBY, string ECODE, string ResigID);
        DataSet FULLRESIGNDETAIL_GET(string RESIGNID, string ECODE, string CLEARENCEHEADERID);
        string UPDATESUBHEADERSTATUS(string clrdetailid, string status);
        DataTable GET_MANAGECLEARANCEHEADERDETAILS(string strecode);
        string UPDATE_SUBHEADERREMARKS(string REMARKS, string detailid, string HEADERSTATUS);
        string CompletedResignation(string RESIGNID, string remarks);
        DataTable ExitintEmpDetail(string strresigin);
        DataTable Get_Resignation_data(string RESIGNID);
         string UPDATE_RESIGNATION_DETAILS(string RESIGNID, string SUBJECT, string BODY, string releDate, string latestaddress);
         string INSERT_INTERVIEW_ANSWER(string RESIGNATIONID, string SUBMITBY, string INTERVIEWXML, string qualification);
         string HR_MODIFYDEPARTMENTDATE(string deptdate, string RESIGNID);
         string HR_CancelRequest(string cancelremark, string RESIGNID, string userid);
         DataSet GET_EXITINTERVIEW_QUESTIONNAIRE(string STATUSSEC_IN, string STATUSQUS_IN, string STATUSOPT_IN, string section);
         string ADDMAILBYHR(string empcode, string emailid, string emailtype, string status, string plant, string submitby, string PROGRAMTYPE, string MAILTRANSACTIONID);
         DataTable GET_MANAGEMAILBYHR(string TRANSID, string ECODE, string ENAME, string EMAILID, string STATUS, string EMAILTYPE, string PLANT);
         DataTable GET_DEPRTMENTAPPEMAILIDS(string RESIGNID, string GETEMAILTYPE, string strcode);
         DataTable Get_Resignationprocess(string strempcode);
         DataTable Get_HEADERCLEARANCE(string strempcode);
         DataTable Get_RESIGPROCESSFLOW(string strempcode);
         string UPDATE_DATERANGEVALIDATION(string MONTHID, string DATERANGEFROM, string DATERANGETO, string status);
         DataTable Get_DateRangeValidation(string MONTHID);
         DataTable Pendingsendmailreq(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId);
         string Acceptancelistsendmail(string ResignID, string addedby);
         DataTable Get_HRMailIdResignation(string REGID);
         DataTable FILLAPPROVALDETAILS(string ecode);
         DataTable GET_GETCLRHEADERCOMPLETED(string strecode);
         DataTable EXPORT_HEAD_DETAILS();
         DataTable EXPORT_SUBHEAD_DETAILS();
         DataTable IRPendingsendmailreq(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId);
         DataTable Get_RESIGNOPHEADHRSENDMAIL(string ECODE);
         DataTable Get_ResgApprovalHistory(string strecode);
         DataTable GET_ResgCLRFORMHistory(object ECODE, string STATUS);
         DataTable GET_ResgCLRHEADEHistory(string strecode);
         DataTable GET_rESGDPTCLRFORMHistory(object ECODE, string STATUS);
         DataTable Get_DeptclrviewHistoryDetails(string ECODE, string RESIGNID);
         string MANAGEWITHDRAWALBYHR(string RESIGNID, string status);
         DataTable GET_MANAGEWITHDRAWDATA(string USERID, string ECODE, string RESIGNID, string PLANTID);
         DataTable GET_MANAGEMAILBYIR(string TRANSID, string ECODE, string ENAME, string EMAILID, string STATUS, string EMAILTYPE, string PLANT);
         string ADDMAILBYIR(string empcode, string emailid, string emailtype, string status, string plant, string submitby, string PROGRAMTYPE, string MAILTRANSACTIONID);
         DataTable GET_REGCLRMAILID(string strRegId);
         DataTable Get_ResignationReport(string ECODE, string ENAME, string OPERATION, string DIVISION, string STATUS, string FROMRELDATE, string TORELDATE);
         string Get_SHOWCLRHEADREQUEST(string ADEMPCODE);
         string Get_SHOWCLRSUBHEADREQUEST(string ADEMPCODE);
         DataSet APPROVALAUTHDETAIL(string RESIGNID);
         DataTable HRFornIDetail(string strecode, string strresigid);
         DataTable GetFormIDetail(string strresigid);
         string INSERT_FORMI(string strformi, string strresigid, string strbasicamt, string strgratuityamt, string strstatus,
            string SUBMITBY, string stremailid, string strrelievingdate);
         DataTable MANAGEGratuityList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId);
         string UPDATE_FORML(string strformi, string SUBMITBY, string FILENAME);
         string UPDATE_FORMI(string RelievingDate, string BasicAmt, string GratuityAmt, string RegId, string UpdatedBy);
         DataTable STAFFAPPROVAL(string strecode, string strlevel);
         DataTable GET_SYPARAMETERS_PARAMVALUE(string paramName);
         DataTable Get_IRMailIdResignation(string REGID);
         DataTable GET_Windowid_Deletion(string code, string paramName, string Requester);
         DataTable EmpDesgValidation(string strecode);
        
        //Start : SR107848
        DataTable GET_EXPORTCLRFORMDETAIL(object ECODE, string STATUS);
         //End : SR107848
    }
}
