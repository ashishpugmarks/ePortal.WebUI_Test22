using System.Collections;
using System.Data;

namespace ePortal.Persistence.TourRequest.Interface
{
    public interface ITourQueries
    {
        public DataSet GetCityList();
        public DataSet GetCityListWithInactive(int CityId);
        public DataSet GetCityListOnTourRequestId(int RequestID);

        public DataSet GetStateList();
        public DataSet GetHotelDetailsListByCity(string strCityId);
        public DataTable GetSingleHotelDetailsByHotel(string strHotelId);
        public DataSet GetCityCategoryList();
        public DataSet GetTravelModeList();
        public DataSet GetTravelModeClass(string ModeID);
        public DataTable EmployeeDetail(int EmpCode);
        public DataTable GSTNODetail(string CityId);
        public DateTime ConvertToDate(string strDate);
        public DataRow[] GetCityCategory(string CityCode);
        public DataRow[] GetEmployeeAllowanceDetail(int CityCategoryID, int EmpDesignationID, string ApplicationDate, string UserType);
        public DataTable GetAppAuthorities(string EmpCode);
        public string getConnectingString();
        public string GetParameterValue(string ParamValue);
        public DataSet GetGSTINNoList();
        public DataTable GetTouradvancedata(string EmpCode);
        public DataTable GetAppAuthorityList(string EmpCode, string ReqEmpcode);
        public DataTable GetSpecialAppAuthorityList(string EmpCode);
        public DataTable GetPendingRequestList(string EmpCode);
        public DataTable GetHistoryRequestList(string EmpCode);
        public DataTable GetRequestHistory(string RequestID);
        public DataTable GetRequestDetail(string RequestID);
        public DataTable GetRequestHeaderPart(string RequestID, string UserCode);
        public ArrayList GetRequestDetailPart(string RequestID);
        public ArrayList GetRequestpreDetail(string RequestID);
        public ArrayList GetRequestDetailPartForAdminApproval(string RequestID);
        public DataTable GetPendingApprovalList(string EmpCode);
        public DataTable GetPendingCancelApprovalList(string EmpCode);
        public DataTable GetHistoryApprovalList(string EmpCode);
        public DataTable GetHistoryCancelApprovalList(string EmpCode);
        public DataTable GetListForFinApproval(string strAppAuth, string RequestID, string EmpCode, string Status, string TourStatus, string siteId, string Fromdate, string ToDate, string UserType);
        public DataTable GetListForAdminApproval(string RequestID, string EmpCode, string Status, string TourStatus, string FromDate, string TillDate, string plantid, string UserType);
        public DataTable GetTicketList(string RequestID, string EmpCode, string Status);
        public DataTable GetTicketListAppNumWise(string RequestID, string EmpCode, string Status, string TourStatus,
                                                 string FromDate, string TillDate, string plantid, string Type);
        public int GetPendignBooking(string RequestID);
        public DataTable GetReqeustBookingDetails(string RequestDetailID);
        public DataTable GetReqeustBookingList(string RequestID);
        public DataTable GetRequestEmails(string RequestID);
        public DataTable GetPreviousAdvance(string EmpCode);
        public DataTable GetDepartmentTourList(string UserID, string RequestID, string EmpCode, string EmpName,
                                                string RequestStatus, string HardCopyStatus, string FromDate,
                                                string TillDate, string AdvanceRequired, string FilterType);
        public DataTable GetAllowanceDetails(string DesignationID, string CityCategoryID, string AllowanceID, string Status);
        public DataTable GetTourSchedule(string EmpCode, string MonthNum, string YearNum);
        public DataTable GetAssociateList(string EmpCode);
        public DataTable GetAssociatesMovementDetails(string UserID, string RequestID, string EmpCode, string EmpName,
                                                        string FromDate, string TillDate);
        public DataTable GetPrintGroupList(string UserID, string GroupID);
        public DataTable GetDuplicatePrintList(string UserID, string FromDate, string ToDate);
        public string GetDepartmentName(string userID);
        public DataTable GetPreviousRequestList(string userID);
        public string GetEMPType(string userID);
        public DataTable TourDayDetails(string userID, string TourDate);
        public string GetEmpName(string userID);
        public string GetStayingCity(string RequestID);
        public string isOperationCoordinator(string userID);
        public DataTable GetChangeAppAuth(string RequestID);
        public DataTable GetAssoiateEmail(string EmpCode);
        public DataTable TourDayDetailsForAdvance(string userID, string TourDate);
        public DataTable GetTourCoordinator(string EmpCode);
        public DataTable GetMonthlyTourReport(string EmpCode, string MonthNum, string YearNum);
        public DataTable GetExcelExportList(string UserID, string GroupID);
        public DataTable GetFinanceTourList(string UserID, string RequestID, string EmpCode, string EmpName, string FromDate, string TillDate, string SiteId, string Bankkey, string EmpType);
        public int InsertTourDetail(string EmpCode, string MobileNo, string ExtNo, string Email, string Objective,
                                    string AdvRemarks, int AdvRequired, string AplusNight, string ANight, string BNight, string CNight, double StayCharge, string APlusDay, string ADay, string Bday, string CDay, double DailyAllow, double MiscAllow, string authType, string AppAuthCode, ArrayList oTourList, string MiscRemarks, double RequiredAmount, string TourStartDate, string TourEndDate, ArrayList oTourListprv, string Initiator_Status);
        public string CancelTourRequest(string RequestID, string Remarks);
        public void UpdateTourDetail(string RequestID, string EmpCode, string MobileNo, string ExtNo, string Email, string Objective,
                                    string AdvRemarks, int AdvRequired, string AplusNight, string ANight, string BNight, string CNight, double StayCharge,
                                    string APlusDay, string ADay, string Bday, string CDay, double DailyAllow, double MiscAllow, ArrayList oTourList,
                                    string MiscRemarks, double RequiredAmount, ArrayList oTourListprv, string Initiator_Status);
        public string UpdateApprovalStatus(string AppType, string RequestID, string status, string Remarks, string AppAuthCode, string spAppAuthCode, string UserID, string Director2 = "");
        public string UpdateFinanceApprovalStatus(string RequestID, string status, string Remarks, string HardCopyStatus, string TranferAmount, string AppAuthCode);
        public string UpdateTicketBooking(string RequestDetailID, string TicketNumber, string PNRNumber, string TicketAmount, string TicketCancelNumber,
                                            string HotelName, string HotelAddress, string CheckinDate,
                                            string CheckinTime, string CheckoutDate, string CheckoutTime,
                                            string PickDropDetails, string TicketStatus, string TicketIssueDate,
                                            string TicketRemarks, string CancellationAuthCode, string TicketType, string comparisionSheetfileName, string tktcancellationdate, string TicketfileName, string TicketfileName2,
                                            string AirlineName, string PrefixCode, string InvoiceNo, string InvoiceDate, string INVOICEAMOUNT, string GSTAmount, string ImpInfo, string InvTotalAmt, string FlightDepartureTime, string FlightDepartureDate);
        public string UpdateAdminApprovalStatus(string RequestID, string status, string Remarks, string AppAuthCode);
        public string CancelTicketRequest(string RequestDetailID, string Remarks);
        public string ApproveCanellationRequest(string RequestDetailID, string Remarks);
        public string UpdateAllowanceDetail(string UserID, string AllowanceID, string DailyAllowance, string StayCharge,
                                            string FromDate, string TillDate, string ActiveStatus, string Designation,
                                            string CityCategory, string strNTA, string strHA, string strLA, string Usertype);
        public void UpdateAllowanceLog(string UserID, string AllowanceID, string DailyAllowance, string StayCharge,
                                           string FromDate, string TillDate, string ActiveStatus, string Designation,
                                           string CityCategory, string strNTA, string strHA, string strLA, string strTransactionType);
        public int UpdatePrintStatus(string RequestList, string UserID);
        public string UpdateApprovalAuthority(string OldRecAuth, string RecAuth, string RequestID, string OldAppAuth,
                                                string AppAuth, string OldSPAppAuth, string SpAppAuth, string strModBy, string strRemarks);
        public string ApprovalAuthority_Set(string strEmpcode, string strAppEmpCode, string strActive, string strAdddedBy, string strAppId);
        public DataTable ApprovalAuth_Get(string strEmpCode, string strEmpName, string strOperation, string strDivision);
        public string UpdateGuestBooking(string TourRequestDetailID, string Location, string Address, string Occupency,
                                            string RoomNo, string PickDropDetails, string Remarks, string CheckinDate,
                                            string CheckoutDate, string ByUserID, string Availablity, string EmpCode);
        public DataTable GetGuestHouseBookingDetails(string RequestDetailID);
        public string UpdateHotelBooking(string RequestDetailID,
                                            string HotelName, string HotelAddress, double fare, string CheckinDate,
                                            string CheckinTime, string CheckoutDate, string CheckoutTime,
                                            string PickDropDetails, string Remarks, string HotelName1, string HotelName2, string HotelAvailable,
                                            string GSTNumber, string StayingCityID, string HotelID, string EmpCode, string HotelID2, string HotelID3,
                                            string Hotel_Cost, string GSTPer, string TotalAmount, string HotelCategory, string HotelBookedBy, string Hotel1GST, string Hotel2GST);
        public string UpdateTicketBookingStatus(string RequestID, string status);
        public string UpdateGuestHouseBookingStatus(string RequestID, string status);
        public string UpdateHotelBookingStatus(string RequestID, string status);
        public DataTable GetAllBookingStatus(string RequestID);
        public int CheckRequestEmailsStatus(string RequestID);
        public DataSet GETTOURAUTHORITYLIST(string ADTOURAPPAUTHORITYID, string AdEmpcode, string status);
        public string INSERTADTOURAUTHORITY(string ADTOURAPPAUTHORITYID, string AdEmpcode, string RECADEMPCODE, string APPADEMPCODE, string status, string addedby);
        public string AddHotelDetail(string HOTELID, string HotelName, string HotelAddress, string City, string State, string GSTNo, string sStatus,
                                      string userID, string OtherCityName);
        public DataSet GETMANAGEHOTEL(string hotelId, string hotelName, string status, string empcode, string syki);
        public DataTable GetSingleHotelDetails(string HotelID);
        public DataTable GetHotelDetailsByAdmin(string strType, string strHotelName, string strHotelAddress, string strCity, string strState, string strGSTNo, string strStatus, string strAddedby);
        public DataTable GetCostCenterDetails(string strCostCenterID, string strStateID, string strOperationID, string strCostCenter, string strProfitCenter, string strStatus);
        public DataTable GetGSTINNODetails(string strStateID, string strGSTINNo, string strStatus);
        public DataTable GetGSTINNOEDITDetails(Int32 SRNo);
        public string AddCostCenterDetail(string strCostCenterID, string strStateID, string strOperationID, string strCostCenter, string strProfitCenter, string strEffectiveForm, string strEffectiveTo, string strStatus,
                                      string userID);
        public string AddGSTINNoDetail(string StrSRNo, string strState, string strGSTINNo, string strEffectiveForm, string strEffectiveTo, string strStatus, string strAddress, string userID);
        public DataSet GetCityListnew(string stateid);
        public DataTable GetExpatsUser();
        public DataTable GetExpatsUserList();
        public string AddExpatTourAppMatDetail(int adempcode, int tourapp1, int tourapp2, int settlapp1, int settlapp2, int addedby, DateTime addeddate, int modifiedby, DateTime modifieddate);
        public string UpdateExpatTourAppMatDetail(int adempcode, int tourapp1, int tourapp2, int settlapp1, int settlapp2, int addedby, DateTime addeddate, int modifiedby, DateTime modifieddate);
        public DataTable GetExpatTourAppMatMaster(string Ecode, string TourApp1, string SettlApp1, string SettlApp2);
    }
}