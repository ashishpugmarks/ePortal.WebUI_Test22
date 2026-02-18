using System.Data;
using System.Dynamic;
using ePortal.DomainClasses;
using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface ILocalConveyanceServices
    {
        #region local Coveyance Master

        #region  LC_ZTABLE MASTER

        List<LC_ZTABLE> GetLC_ZTABLEList();

        LibResult InsertDataToLC_ZTABLE(decimal FourWheelerRate, decimal TwoWheelerRate, DateTime FromDate, DateTime ToDate, int LoginCode, long SiteID); //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026

        LC_ZTABLE GetEditDataforLC_ZTABLE(int id);
        bool GetDeleteDataforLC_ZTABLE(int id, int loginCodes);// {loginCodes added} SR107643 :: LC ENHANCEMENT ::  AUMENTO

        LibResult EditDataForLC_ZTABLE(int SrNo, decimal FourWheelerRate, decimal TwoWheelerRate, DateTime FromDate, DateTime ToDate, int LoginCode, long SiteID); //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026

        List<object> GetSatelist();

        #endregion

        #region LC_EXCEPTION MASTER C TABLE
        List<LC_EXCEPTION_C_TABLE> GetLC_EXCEPTIONList();

        LibResult InsertDataIntoLC_EXCEPTION(int EmpCode, decimal FuelReimbursementPerLtr, DateTime FromDate, DateTime ToDate, int LoginCode, string Remarks);

        LC_EXCEPTION_C_TABLE GetEditDataForLC_EXCEPTION(int id);
        bool GetDeleteDataForLC_EXCEPTION(int id, long LoginCode); // {LoginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO

        LibResult EditDataForLC_EXCEPTION(int SrNo, int EmpCode, decimal FuelReimbursementPerLtr, DateTime FromDate, DateTime ToDate, int LoginCode, string Remarks);

        LibResult GETEmployee(string Key);

        #endregion


        #region LC_ReimbursementMaster B TABLE
        List<LC_REIMBURSEMENT_B_TABLE> GetLC_ReimbursementMasterList();

        LibResult InsertDataIntoLC_ReimbursementMaster(int DesignationID, string VehicleEntitlement, decimal Fule, int Refresh, int Meal, DateTime FromDate, DateTime ToDate, int LoginCode);

        LC_REIMBURSEMENT_B_TABLE GetEditDataForLC_ReimbursementMaster(int id);
        bool GetDeleteDataForLC_ReimbursementMaster(int id, int LoginCode); // {LoginCode addded} SR107643 :: LC ENHANCEMENT ::  AUMENTO

        LibResult EditDataForLC_ReimbursementMaster(int SrNo, int DesignationID, string VehicleEntitlement, decimal Fule, int Refresh, int Meal, DateTime FromDate, DateTime ToDate, int LoginCode);

        LibResult GetDesignationlist();
        #endregion

        #endregion

        #region  Local Conveyance Request functions

        //DataTable GetODDATA(string strEmpCode, string FromDate, string ToDate);
        Task<List<LC_DETAIL_TEMP>> GetODDATA(string strEmpCode, string FromDate);  // Changes in this Function by Aumento as on 21-06-2024


        LC_DETAIL_TEMP GetODDATAFromLC_DETAIL(long? EmpCode, DateTime? DateOfExpenditure, long? ASOFFODID);


        //List<LC_DETAIL_TEMP> GetODListDATAFromLC_DETAIL(string FromDate, string ToDate, int? EmpCode);
        List<LC_DETAIL_TEMP> GetODListDATAFromLC_DETAIL(string FromDate, int? EmpCode, int Status);  // Remove Todate perameter by Aumento as on 21-06-2024 // {Status added} SR107643 :: LC ENHANCEMENT ::  AUMENTO

        LibResult GetEmployeeDATA(int EmpCode);


        List<object> CityList();


        LibResult SaveDetails(LC_DETAIL_TEMP i, int logincode, string Mode); // {Mode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO

        string getStatus(LC_DETAIL_TEMP i);

        LibResult Getallowcheck(int logincode, TimeSpan INTime, TimeSpan OUTTime, decimal NOOFHOURS, string MODEOFTRAVEL, int KMCOVERED);

        LocalConveyanceRequestViewModel GetReqData(DateTime ExDate, int LoginEmp);

        LibResult SubmitData(List<LC_DETAIL_TEMP> DtData, LC_HEAD HeaderData, int logincode, Employee_Details LoginEmpDT);


        bool UpdateLCDetailTemp(LC_DETAIL dt);


        Tuple<string, string> GetDateRange();

        int GetMonthRangeCount();   // New Function Added by Aumento as on 21-06-2024


        Tuple<TimeSpan, TimeSpan, TimeSpan, TimeSpan> GetLunchDinnerTime();




        #endregion

        #region Local Conveyance Recommandation Functions


        List<LC_HEAD> GetListDataForReco(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status);

        LibResult GetRequestDataForReco(int LCTRANNO, int EMPCODE);

        LibResult ApproveRejectReco(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode);

        #endregion

        #region Local Conveyance Approval Functions


        List<LC_HEAD> GetListDataForApproval(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status);

        LibResult GetRequestDataForApproval(int LCTRANNO, int EMPCODE);

        LibResult ApproveReject(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode);

        #endregion

        #region Local Conveyance Admin Functions
        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        List<LC_HEAD> GetListDataForAdmin(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status);
        LibResult GetRequestDataForAdmin(int LCTRANNO, int EMPCODE);
        LibResult AdminSubmit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode, string Fuel, string doc, string approvetype);
        LibResult AdminSaveDetail(decimal FUEL_Reimburse, int EMPCODE, int ASOFFODID, DateTime Date, int loginCode);
        #endregion

        #region Local Conveyance Admin DH Functions
        List<LC_HEAD> GetListDataForAdminDH(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status);
        LibResult GetRequestDataForAdminDH(int LCTRANNO, int EMPCODE);
        LibResult AdminDH_Submit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode);
        #endregion

        #region Local Conveyance Admin OH Functions
        List<LC_HEAD> GetListDataForAdminOH(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status);
        LibResult GetRequestDataForAdminOH(int LCTRANNO, int EMPCODE);
        LibResult AdminOH_Submit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode);
        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        #endregion

        #region  Local Conveyance Finance Functions



        List<LC_HEAD> GetListDataForFinance(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status);

        LibResult GetRequestDataForFinance(int LCTRANNO, int EMPCODE);


        Task<LibResult> FinanceSubmit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode, string RequiredAmount, DateTime PostingDate, string ConvAmount, string MealAmout, List<LC_DETAIL> DtData);

        LibResult ViewPDF(int LCTRANNO, int EMPCODE, int loginCode);


        #endregion

        List<LC_HEAD> GetListDataForRecoApproval(int LoginCode);

        List<LC_HEAD_ViewModel> GetDataForExcelFromDB(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status, string Flag); // {Flag added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        object GetSysiteList(); // Added by Aumento as on 01082024
        object GetPaymentPlants();
        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        object GetRequestorDesg();
        object GetApprovalDesg();

        #region Master Log View
        Task<List<ExpandoObject>> GetLC_Master_Log_Data(int Srno, string TblName);
        #endregion

        #region OH Directore maping Master
        List<LC_OH_DIRAPPROVALFLOW_MASTERViewModel> GetLC_OH_DirApprovalFlowList();
        LC_OH_DIRAPPROVALFLOW_MASTER GetEditDataforLC_OH_DIRAPPROVALFLOW_MASTERViewModel(int id);
        LibResult EditDataforLC_OH_DIRAPPROVALFLOW_MASTER(int SrNo, string RequestorDesg, string ApprovalDesg, int LoginCode);
        #endregion

        #region Approval History
        DataTable GetApprovalHistory(int lctranno);
        #endregion

        List<LC_HEAD> GetListDataForRejectedRequest(int LoginCode);
        LibResult OpenRejectedRequest(int LCTRANNO, int EMPCODE);
        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

    }
}
