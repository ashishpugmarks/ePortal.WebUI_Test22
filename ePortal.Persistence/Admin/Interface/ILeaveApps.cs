using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ePortal.Persistence.Admin.Interface
{
    public interface ILeaveApps
    {

        
DataTable GetEmployeeAuthCodes(string empCode); // PKG_LEAVEAPPAUTH.SPROC_AUTHCODES_GET
        IDataReader GetEmployeeData(string empCode);    // PKG_LEAVEAPPAUTH.SPROC_GETEMPDATA
        DataSet GetEligibleAuthorities(string divId, string empCode, string deptId, string vpId); // PKG_LEAVEAPPAUTH.SPROC_EMPRECOMMAUTH_GET
        IDataReader GetCurrentAuthorities(string empCode); // PKG_LEAVEAPPAUTH.SPROC_GETAPPAUTHORITY

        // Commands
        void SetAuthorities(string empCode, string recomCode, string approverCode, string modifiedBy); // PKG_LEAVEAPPAUTH.SPROC_SETAPPAUTHORITY
        void LogAuthorityChange(string empCode, string recomCode, string approverCode, string updatedBy); // SPROC_LEAVEAUTHORITY_LOG
    

        DataSet SearchLeaveDetail(string strEmpCode, string strLeavePlan, string strFromYear);
        int getdesignation(string strstrdesignation);
        DataSet EmpLeaveRecord(string strEmpCode, string LeaveYear, string ReqType);
        DataSet GetApprovarCode(string strEmpCode, string strApprovarType);
        string getLeaveBalance(string strUserID, string strLeaveType, string strLeaveApplied);
        string getEmployeeGender(string strecode);
        string leaveEntry(string strEmpCode, string strLeaveYear, string strDateFrom, string strDateTo,
                          string strHalfDay, string strLeaveType, string strPurpose,
                          string strContactNo, string strBackupEmpcode, string strSupervisorEmpCode,
                          string strHalfdaydate, string strShift, string strHalfdayShift,
                          string strinLieuFrmdate, string strinLieuTodate, string strfromtime,
                          string strtotime, string strShiftStartTime);
        DataSet ManageLeaveRequest(string strEmpCode);
        DataSet ManageLeaveRequest1(string strEmpCode, string strYear);
        DataSet ManageLeaveApproval(string strSupEmpCode);
        DataSet EditLeaveApproval(string strTransID, string strEcode, string strSecode);
        string UpdateApproval(string strSupervisorEmpCode, string strID, string strApprovalStatus,
                              string strRemarks, string strleavePlan, string strFlag,
                              string strSpecialAppStatus, string strSpecialAppEcode);
        DataSet SapLeaveData();
        DataSet ManageRecord();
        DataSet GetLeaveRecOnCond(string strFromDt, string strToDt, string empcode,
                                  string strsitid, string struserid, string strDesg, string strStatus);
        DataSet GetLeaveRecordForSAP(string strFromDt, string strToDt, string empcode,
                                     string strsitid, string struserid, string strDesg, string strStatus);
        DataSet get_AllEmp(string empcode);
        DataSet LeaveType();
        DataTable EmployeeDataGrid(string strEmpCode);
        DataTable STAFF_EmployeeDataGrid(string strEmpCode);
        DataTable EmployeeLeaveDetail(string strEmpCode);
        string UpdateleaveEntry(string strID, string strEmpcode, string strDateFrom, string strDateTo,
                                string strHalfDay, string strLeaveType, string strPurpose,
                                string strContactNo, string strBackupEmpcode, string strSupervisorEmpCode,
                                string strHalfdaydate, string strShift, string strHalfdayShift,
                                string strinLieuFrmdate, string strinLieuTodate, string strCboKiCode,
                                string strfromtime, string strtotime, string strShiftStartTime);
        DataSet CancelledLeaveAfteApproval(string empcode, string strsiteid, string struserid,
                                           string FromDate, string ToDate, string strLeaveType, string strRemarks);
        DataSet CancelledLeave(string UserID, string FromDate, string TillDate, string siteid, string userid);
        DataSet LeaveApprovalHistory(string strEmpCode);
        DataSet EditASRLeaveApproval(string strTransID);
        string leaveASRApproval(string strTrID, string strAppRemarks, string strAppStatus, string strASREcode);
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
        void Leavebulkapproval(string reqid, string empcode, string appecode, string appname);
        DataTable GetEMPLeaveDTL(long reqId);
        DataSet CancelledLeaveForReport(string UserID, string FromDate, string TillDate, string siteid, string userid);
        DataSet ManageSpecialRecord_ForReport(string strFromDt, string strToDt, string empcode, string strsiteid, string struserid);
        DataSet GETISUPDATEDMAILID(string IsUpdated, string strCode);


        // Operations / Org
        DataTable GetOpHeadOperations(string strEcode);
        DataTable OrgDetail_Get(string strEcode, string strDesig);
        DataTable GetOrgUnits(string strEcode, string strOrgtype);

        // Division(s)
        DataTable GetDivisions(string str_ecode, string fn_desig, string str_operations);
        DataTable GetDivision(string str_ecode, string fn_desig, string str_operations);

        // Department(s)
        DataTable GetDepartment(string str_Operations, string str_divisions, string strecode, string fndesig);
        DataTable Get_Department(string str_Operations, string str_divisions, string strecode, string fndesig);

        // Section(s)
        DataTable GetSection(string str_operations, string str_divisions, string str_depid, string strecode, string fndesig);
        DataTable Get_Section(string str_operations, string str_divisions, string str_depid, string strecode, string fndesig);

        // Employees
        DataTable GetEmployee(string str_operations, string str_divisions, string str_depid, string str_secid, string strecode, string fndesig);
        DataTable GetEmployees(string str_operations, string str_divisions, string str_depid, string str_secid, string strecode, string fndesig);

        // Leave balances
        DataTable GetLeaveAvailBal(
            string str_operations,
            string str_divisions,
            string str_depid,
            string str_secid,
            string strecode,
            string fndesig,
            string str_employees,
            string str_Year);

        // Designation
        DataTable GetFn_desig(string strecode);
    }

}
