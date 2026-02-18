using ePortal.ViewModels;
using ePortal.ViewModels.APPX.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IUtilityDesk
    {

        DataSet get_ServiceCatalog(string a);
        DataSet get_ServiceCatalog();
        DataSet get_ServiceCatalogAdmin();
        DataSet get_Classification(string servicecatalogid);

        string SubmitTicket(
            int strCode,
            int strExt,
            long strContactNo,
            string strDirect,
            string strLoc,
            string strLine,
            string strStation,
            string strServiceCatalog,
            string strClassification,
            string strDetail,
            string strActDetail,
            string strSite);

        DataSet GetAllUtilityReqTickets(string Type);
        DataSet GetAllUtilityTickets(string Type);
        DataSet GetAllAdminTickets(string Type);

        DataSet ViewTicketDetail(string TICKETID);
        DataSet ViewAdminCallTicketDetail(string TICKETID);

        int SubmitSolution(
            string strHandledBy,
            string strRemarks,
            string strStatus,
            string strTicketId,
            string strempcode);

        int SubmitSolutionAdminCall(
            string strHandledBy,
            string strRemarks,
            string strStatus,
            string strTicketId,
            string strempcode);

        string SubmitUtilityReq(
            string strEmpCode,
            string strExtNo,
            string strContactNo,
            string strDirectNo,
            string strLocation,
            string strLine,
            string strStation,
            string strServiceCatalog,
            string strClassification,
            string strbudget,
            string strTitle,
            string strScope,
            string strAttachfile,
            string strappauth,
            string strSiteID);

        string EditUtReqBySctMgr(
            string strrequestid,
            string strStatus,
            string strRemark,
            string strDeptMgrCode,
            string strsctmgrcode);

        string EditUtReqByDptMgr(
            string strrequestid,
            string strStatus,
            string strRemark);

        string EditUtPoReqByUt(
            string strrequestid,
            string strquotesubmitdate,
            string strnoofquotes,
            string strquoteprice,
            string strRemark,
            string strutstatus,
            string strpono,
            string strpodate,
            string strclosereamrk,
            string strutuserid);

        DataSet ViewManageRequest(string strempcode);
        DataSet ViewManageRequest1(string strempcode);

        DataSet ApprovalOfUtReq(string strSupEmpCode);
        DataSet GetApprovalAuthority(string strempcode);

        DataSet GetAminEmailId();

        string EditUtilityReq(
            string strreqid,
            string strEmpCode,
            string strExtNo,
            string strContactNo,
            string strDirectNo,
            string strLocation,
            string strLine,
            string strStation,
            string strServiceCatalog,
            string strClassification,
            string strbudget,
            string strTitle,
            string strScope,
            string strName,
            string strappauth);

        string CancelUtReq(string strcancelremark, string strreqid);

        DataSet UtilityAppHistory(string strempcode);

        DataTable GetResponsibleUserDetails(string EmpDeptCode, string LocationCode);

        DataTable GetUtilityCallList(
            string FromDate,
            string TillDate,
            string TicketID,
            string Status,
            string Department,
            string Section,
            string Catalog,
            string Site);
        UtilityDocumentInfo GetUtilityDocumentInfo(long docId);
        UtilityRequestViewModel GetContactInfo(long empCode);
        UtilityRequestViewModel GetSelectedApprovalAutority(long empCode);
        Employee_Details GetEmployeeDetails(long empCode);
        void SendMail(string mailTo, string mailSubject, string mailBody);

        DataTable GetUtilityRequestDetail(long requestId);
        DataTable GetMailIds(long empCode);
    }
}
