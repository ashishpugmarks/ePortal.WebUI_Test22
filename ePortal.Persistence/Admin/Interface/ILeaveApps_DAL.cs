using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface ILeaveApps_DAL
    {
        DataSet SearchLeaveDetail(string strEmpCode, string strLeavePlan, string strFromYear);
        int GetDesignation(string strstrdesignation);
        DataSet EmpLeaveRecord(string empCode, int leaveYear, string reqType);
        DataSet GetApprovarCode(string empCode, string approvarType);
        string GetLeaveBalance(int userId, int leaveType, double leaveApplied);
        DataSet ManageLeaveRequest(string strEmpCode);

        DataSet ManageLeaveRequest1(string strEmpCode, string strYear);
        DataSet ManageLeaveApproval(string strSupEmpCode);
        DataSet EditLeaveApproval(string strTransID, string strEcode, string strSecode);
        string UpdateApproval(string strSupervisorEmpCode, string strID, string strApprovalStatus, string strRemarks, string strleavePlan, string strFlag, string strSpecialAppStatus, string strSpecialAppEcode);
        DataSet SapLeaveData();
        DataSet ManageRecord();
        DataSet GetLeaveRecOnCond(string strFromDt, string strToDt, string empcode, string strsitid, string struserid, string strDesg, string strStatus);
        DataSet GetLeaveRecordForSAP(string strFromDt, string strToDt, string empcode, string strsitid, string struserid, string strDesg, string strStatus);
        DataSet get_AllEmp(string empcode);
        DataSet LeaveType();
        DataTable EmployeeDataGrid(string strEmpCode);
        DataTable STAFF_EmployeeDataGrid(string strEmpCode);
        DataTable EmployeeLeaveDetail(string strEmpCode);

        string UpdateLeaveEntry(string strID, string strEmpcode, string strDateFrom, string strDateTo,
                                        string strHalfDay, string strLeaveType, string strPurpose, string strContactNo,
                                        string strBackupEmpcode, string strSupervisorEmpCode, string strHalfdaydate,
                                        string strShift, string strHalfdayShift, string strinLieuFrmdate, string strinLieuTodate,
                                        string strCboKiCode, string strfromtime, string strtotime, string strShiftStartTime);
        DataSet CancelledLeaveAfteApproval(string empcode, string strsiteid, string struserid, string FromDate, string ToDate, string strLeaveType, string strRemarks);
        DataSet CancelledLeave(string UserID, string FromDate, string TillDate, string siteid, string userid);
        DataSet LeaveApprovalHistory(string strEmpCode);
        DataSet EditASRLeaveApproval(string strTransID);
        string LeaveASRApproval(string strTrID, string strAppRemarks, string strAppStatus, string strASREcode);
        DataTable FromYearGet();
        DataTable GetLeaveAppliedYears(string EmpCode);
        DataTable ExcelExport(string strEmpCode, string strYear);
        DataSet ManageSpecialRecord(string strFromDt, string strToDt, string empcode, string strsiteid, string struserid);
        DataSet GetPendingLeaveReport(string strFromDt, string strToDt, string empcode, string strsiteid, string struserid);
        DataSet GetLeaveDTL(string empcode, string strreqid);
        DataSet CancelledLeaveAfteApprovalBYADTRANSACTIONID(string strRequestid);
        DataSet GetSpecialApprovalAuthority(string strEcode);
        DataTable GetRealTimeatt(string strEmpCode, string strpunchdate, string strassociateecode);
        DataTable GetAccessReport(string strEmpCode, string strpunchdate, string strassociateecode);
        DataTable GetEMPLeaveDTL(Int64 reqId);
        DataSet CancelledLeaveForReport(string UserID, string FromDate, string TillDate, string siteid, string userid);
        DataSet ManageSpecialRecord_ForReport(string strFromDt, string strToDt, string empcode, string strsiteid, string struserid);
        DataSet GETISUPDATEDMAILID(string IsUpdated, string strCode);
    }
}
