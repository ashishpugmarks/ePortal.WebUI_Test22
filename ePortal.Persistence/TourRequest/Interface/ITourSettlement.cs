using ePortal.Persistence.TourRequest.Services;
using ePortal.ViewModels.APPX.TourRequest;
using System.Collections;
using System.Data;

namespace ePortal.Persistence.TourRequest.Interface
{
    public interface ITourSettlement
    {
        public void UpdateTBS(string strSettlementid, string strEmpcode, DateTime Planfrom_date, DateTime Planto_date, DateTime Actfrom_date, DateTime Actto_date, DateTime Actfrom_time, DateTime Actto_time, string Addedby, Tour_Day_Detail[] obj, Ticket_Detail[] objticket, string strAppEcode, out string strSettid);

        public void NEWUpdateTBS(string strSettlementid, string strEmpcode, DateTime Planfrom_date, DateTime Planto_date, DateTime Actfrom_date, DateTime Actto_date, DateTime Actfrom_time, DateTime Actto_time, string Addedby, Tour_Day_Detail[] obj, Ticket_Detail[] objticket, string strAppEcode, out string strSettid, List<Hotel_Detail> objHotelList, string OTHERATTACHMENT, string OtherTKTFile);


        public void UpdateTBSTktdtl_ByUser(/*GridViewRowCollection grcol,*/ string strAddedby, string strStlmntid);
        public string UpdateApproval(string strAPPempcode, string strSettlementID, string strStatus, string strRemark, string strAddedby);
        public string CancelRequest(string strSettlementID, string strRemark, string strAddedby);
        public string UpdateBillFlag(string strSettlementID, string strTicketFlag, string strHotelFlag, string strTaxiFlag, string strBoardBill_Flag);
        public void UpdateTktAppDtl(/*GridViewRowCollection grcol, */string strAddedby, string strStlmntid, string strStatus);
        public string AddTicketDetail(string strTourDtlid, string strEmpcode, string strTvlDate, string strTvlTime, string strFromcity, string strTocity, string strTvlmode,
                                      string strflightno, string strTvlclass, string strTktNo, string strTktAmt, string strRemark, string strIssuedate,
                                      string strPnrno, string strTkttype, string straddedby, string strStatus, string strTvlStatus,
                                      string strFlightName, string strPrefixCode, string strInvoiceNo, string strInvoiceDate, string strInoiceAmount, string strGSTAmount, string strInvTotalAmount,
                                      string strFlightDepTime, string strFlightDepDate, string strImpInfo, string TicketfileName, string TicketfileName2);
        public string UpdateBillStatus(string straddedby, ArrayList strStlId, string strStatus);
        public void UpdateTBS_ByPayable(string strSettlementid, string strEmpcode, string Addedby, Tour_Day_Detail[] obj, Ticket_Detail[] objticket);
        public string UpdateCityCategory(string strId, string strCity, string strCityCategory, string straddedby, string strStatus, string strEffectiveFrom, string strEffectiveTo);
        public string UpdateCostCenter(string strXMLCostCenter, string straddedby);
        public DataTable GetPendingTourReq(string strEmpcode);
        public DataTable GetUserTicketDetail(string strTourReqID);
        public DataTable GetUserTourReqDtl(string strTourReqID);
        public DataTable GetCityDtl(string strCityname, string strEmpcode, DateTime Tourdate);
        public string CheckIsHoliday(string strDate, string strSiteID, string strempcode);
        public DataTable GetPenSettlementList(string strEmpcode);
        public DataTable GetTBSSettledList(string strEmpcode);
        public DataTable GetSettlementTranDtl(string strStlmntID);
        public DataTable GetSettlementDayDtl(string strStlmntID);
        public DataTable GetHotelSettlementDayDtl(string strStlmntID);
        public DataTable GetSettlementConvDtl(string strStlmntID);
        public DataTable GetSettlementTktDtl(string strStlmntID);
        public DataTable GetSettlementCityDtl(string strStlmntID);
        public DataTable GetApplist(string strEmpcode, string strLVL);
        public DataTable GetPendingApplist(string strEmpcode);
        public DataTable GetStlAppHistory(string strStlmntID);
        public DataTable GetPenStlATAdmin(string strEmpcode, string strEmpname, string strStlId, string strStatus, string strAddedby);
        public DataTable GetBillReceiveList(string strEmpcode, string strEmpname, string strStlId, string strStatus);
        public DataTable GetStlDtl_ATPAYBLE(string strAppcode, string strEmpcode, string strStatus, string strEmpname, string strSiteID, string strSecID, string strDeptID, string strDivID, string strOpID, string strStlId, string empType);
        public DataTable NewGetStlDtl_ATPAYBLE(string strAppcode, string strEmpcode, string strStatus, string strEmpname, string strSiteID, string strSecID, string strDeptID, string strDivID, string strOpID, string strStlId, string FormDate, string ToDate, string empType);
        public DataTable GetTktDtlBookedByAdmin(string strid, string strEmpcode, string strEmpname, string strfromdate, string strtodate);
        public DataTable GetSettAppDtl(string strStlmntID);
        public string MailtoApprovalAuth(string strEcode, string strEname, string strSettlement);
        public string MailToAssociate(string strAppEcode, string strAppname, string strappemail, string strAppremark, string strStatus, string strSettlID);
        public DataTable GetManageAppHistory(string strEcode);
        public DataTable GetTourDtl_BySettlementId(string strStlmntID);
        public List<string> GetTBSPilotUser();
        public DataTable GetCityList(string strCityid, string strCityName, string strStatus, string strCategory);
        public DataTable GetOrglvlList(string strOPID, string strDIVID, string strDEPID, string strSECID, string strStatus);
        public DataTable GetAssociatesExpenseDetails(string UserID, string RequestID, string EmpCode, string EmpName,
                                                  string FromDate, string TillDate);
        public DataTable GetHotelamountDtl(string strStlmntID, string strfromdate, string strtodate, string strtodatec);
        public string UpdateHotelDetailByFIN(Hotel_Detail obj_dt, string addedby);
        public string UpdateHotelGST(Hotel_Detail obj_dt, int addedBy);
        public bool ValidateGST(string gstNo, int stateId);
        public DataTable GetHotelID(string RequestID);
        public string DeleteHSETLDETBkbyAssociate(int HSETLDETBOKBYCAID, int ADTOURREQUESTID, string addedby);
        public DataSet GetGSTRateList();
        public DataTable GetgrdHDetailsByAssociate(string strH_SETTLEMENTID, string strADTOURREQUESTID, string strSTATUS_IN);
        public DataTable GetgrdHSetDetailsByAssociate(long strH_SETTLEMENTID, long strADTOURREQUESTID, int strBOOKEDBY_IN);
        public DataTable GetSingleHSettlementDetails(string HSettlementID);
        public string UpdateTBS_ByFinance(string strSettlementid, string strEmpcode, string Addedby, Tour_Day_Detail[] obj, string ApprovalType);
        public DataSet GetSettlementhHotlDtl(string strStlmntID);
        public void UploadSettlementAttachment(int ID, string ADTOURDTLID, string FILTERID, string DOCtype, string PATH, byte[] TICKET_FILE_BYTE, string TICKET_FILE_NAME, string ADDEDBY);
        public int DeleteAttachmentBySettlID(Int64 SettlID, string TicketID, string HotelID, string FileName, string DocType, string UpdatedBy);
        public int UploadAttachmentBySettlID(Int64 SettlID, string TicketID, string HotelID, string FileName, string DocType, string UpdatedBy);
        public DataTable GetFinaceAttachmentBySettlId(string strStlmntID);
        public DataTable GetApprovalDtl_ATPAYBLE(string strAppcode, string strEmpcode, string strStatus, string strEmpname, string strSiteID, string strSecID, string strDeptID, string strDivID, string strOpID, string strStlId);
        public string InsertDATA(string strSettlementid, string SAPDOCNO, DateTime SAPPOSTINDDATE, int type);
    }
}
