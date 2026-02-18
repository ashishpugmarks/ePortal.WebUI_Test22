// Auto-generated interface for Stationary
using System.Data;

public interface IStationary
{
    // Insert and Update
    int InsertUpdate_StationaryMaster(string strStationaryID, string strStationary_typeCode, string strStationaryCode, string strStationaryDesc, int strActive,
        string strUOM, string strBy, string strplant, string strreorder, string strmaxqty);
    int InsertUpdate_StationaryIN(string strSStockInID, string strStationaryID, string strQty, string strRecivingdate, string strBillno,
        string strPOno, string strRemark, int strActive, string straddby);
    string InsertStationaryReq(string strecode, string strmobileno, string strextno, string strXml, string strapprovalauth);
    string UpdateStationaryReq(string strreqid, string strecode, string strmobileno, string strextno, string strXml);
    string CancelStationaryRequest(string RequestID, string Remarks, string empcode);
    string UpdateApprovalStatus(string RequestID, string status, string Remarks, string UserID);
    string UpdateAdminStatusReq(string strreqid, string strecode, string strststus, string strremark, string strreccode, string strrecname, string strXml);
    string AdminInsertStationaryReq(string strecode, string strXml);

    // Get Data
    DataTable Get_StationaryIN(string strstockid, string strplantid, string strfmdate, string strtodate, string stritemcode, string strbillno, string strpono, string strstatus);
    DataTable Get_Stationarymaster(string strSid, string strstatus, string strplant);
    DataTable GetPendingApprovalList(string EmpCode);
    DataTable GetHistoryApprovalList(string EmpCode);
    DataTable GetPendingRequestList(string EmpCode);
    DataTable GetHistoryRequestList(string EmpCode);
    DataTable GetRequestDetail(string RequestID);
    DataTable GetRequestDetailPart(string RequestID);
    DataTable StationaryreqList(string strEmpCode, string strEmpName, string ReqeustStatus, string strDateFrom, string strDateTo, string strKI, string strPlantID);
    DataTable StationaryreqExcel(string strEmpCode, string strEmpName, string ReqeustStatus, string strDateFrom, string strDateTo, string strKI, string strPlantID);
    DataTable Get_AvailableStock(string strSid, string strplant);
    DataTable Get_IssueStock(string strSid, string strki, string strmonth, string strplant);
    DataTable Get_MonthlyReport(string strki, string strmonth, string strplant);
    DataTable GetApprovalList(string EmpCode);
}
