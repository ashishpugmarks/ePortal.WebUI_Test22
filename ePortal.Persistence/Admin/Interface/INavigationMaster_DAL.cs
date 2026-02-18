using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface INavigationMaster_DAL
    {
        public DataTable GetNavigationList();
        public DataTable GetNavigationListByEmpCode(String strEmpCode);
        public DataTable GetActivityListForNavigation(String strEmpCode);
        public DataTable GetNavigationUsersListByActivity(String strActivity);
        public DataTable GetUserNavigationRights(String strEmpCode);
        public DataTable GetUserRightsListByEmpCode(String strEmpCode);
        public DataTable GetRoleMasterList(String strEmpCode);
        public int InsertUpdateUserRights(String strXML);
        public DataTable GetNavigationDetails(String strID);
        public DataTable GetUsers();
        public String GetUserName(String strEmpCode);
        public DataTable GetNavRightsDetails(String strID, String strEmpCode);
        public int CheckUserRight(String strActivityID);
        public uint CheckActivityParent(String strEmpCode, String strActivityID);
        public int InsertUpdateNavigation(String strID, String strDescription,
                                          String strAltValue, String strNodeUrl,
                                          String strIsParent, String strParentID,
                                          String strBy, String strActive,
                                          String strPageName, String strDisplayOrder);
        public int InsertUpdateNavRights(String strXML);
        public string DeleteNavRights(String strEmpCode, String strActivityID, String strBy, String ACCESSTYPE); // ACCESSTYPE added for SR78412
        public DataTable AssessReviewReport_Get(string strEmpcode, string strEmpName, string FromDate, string ToDate, string strStatus, string strUserType);
        public DataTable FINTBS_GSTIN_Get(string strGSTINNo, string strState, string strStatus);
        public DataTable UserDetailsReport_Get(string strEmpcode, string strEmpName, string strUserType, string FromDate, string ToDate);
        public DataTable ThirdPartyUserList_GET(string strEmpcode, string strEmpName, string strEmpType);
        public DataTable PermanentEmployeeReport_Get(string strEmpcode, string strEmpName);
        public DataTable PermanentEmployeeReport_Get(string strEmpcode, string strEmpName, string strMonth, string strYear);
        public int InsertUpdateGSTDetails(String strSRNo, String strState,
                                          String strGstInNo, String strEffectiveFromDate,
                                          String strEffectiveToDate, String strStatus
                                          );
        public string DeActiveUser(String strEmpCode, string modifiedBy);
        public DataTable EmployeeReviewReport_Get(string strEmpcode, string strEmpName, string strUserType, string operation);
        public DataSet get_AllOperation();
        public DataSet get_AllDivision(string OperationID);
        public DataSet get_AllDepartment(string OperationID, string DivisionID);
        public DataTable GetEmployeeEDITDetails(Int32 ECODE);
        public DataSet get_Review(long OperationID);
        public DataSet get_Review2(long OperationID);
        public string UpdateEmployee(string strEmpCode, string StrOperation, string StrDivision,
                                       string StrDepartment);
        public DataSet get_ReportingManager1(string OperationID);
        public string InsertEmployee(long EmpCode, string EmpName, long OperationID, long DivisionID, long DepartmentID, long Reviewer1, long Reviewer2,
                                        long UserType, long reviwer, long strLoginEmpCode);
        public DataTable UARRequestList_Get(string strEmpcode, string LoginCode);
        public DataTable GetUARList(int loginCode, string Status, string Ename, string Ecode);
        public DataTable ReviewReport_Get(string strEmpcode, string strEmpName, string strStatus, string strUserType, string RequestDate, string operation, string SYKIID, string RequestToDate);
        public DataTable UpdateReviwer(string strEmpcode, string strEName, string UserType, string operation);
        public DataTable GetUAREDITDetails(Int32 ECODE);
        public string UpdateReviewer(string ecode, string remark, string selectedReviewer, string operation, string division, string department);
        public DataSet get_ReportingManagerDivision(string DivisionID);
        public DataSet get_ReportingManagerDepartment(string DepartmentID);
        public DataTable GetUARRegularList(int loginCode, string Status, string Ename, string Ecode);
        public DataTable UARRegularRequestList_Get(string strEmpcode, string REVIEWER);
        public string DeleteRegularNavRights(String strEmpCode, String strActivityID, String strBy, String ACCESSTYPE, int UserChecked); // ACCESSTYPE added for SR78412 //SR102715
        public DataTable UARRegularReviewerRequestList_Get(string strEmpcode, string REVIEWER);
        public DataTable ReviewStatusReport_Get(string strEmpcode, string strEmpName, string strStatus, string strUserType, string RequestDate, string operation, string SYKIID, string RequestToDate);
        public DataTable GetUARStatusList(int loginCode, string Status, string Ename, string Ecode, string SYKIID); //SYKIID Added for SR78412
        public DataSet get_AllSYKI();
        public DataTable GetSelfReviewRemarks(long AdEmpCode, long REVIEWER);
        public string UpdateSelfReviewRemarks(long AdEmpCode, String SelfRemarks);
        public DataTable GetReviewerRemarks(long AdEmpCode, long REVIEWER);
        public string UpdateReviewerRemarks(long AdEmpCode, String Remarks);
        public DataTable UARRegularRequestHistoryList_Get(String UARID);

    }
}
