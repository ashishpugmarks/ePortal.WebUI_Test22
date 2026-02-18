using System.Data;
using System.Dynamic;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class LocalConveyanceServices : ILocalConveyanceServices
    {
        private readonly LocalConveyanceRepository LcRepo;
        public LocalConveyanceServices(LocalConveyanceRepository _LcRepo)
        {
            LcRepo = _LcRepo;
        }

        #region local Coveyance Master

        #region  LC_ZTABLE MASTER

        public List<LC_ZTABLE> GetLC_ZTABLEList()
        {
            return LcRepo.GetLC_ZTABLEList();

        }

        public LibResult InsertDataToLC_ZTABLE(decimal FourWheelerRate, decimal TwoWheelerRate, DateTime FromDate, DateTime ToDate, int LoginCode, long SiteID) //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026
        {

            return LcRepo.InsertDataToLC_ZTABLE(FourWheelerRate, TwoWheelerRate, FromDate, ToDate, LoginCode, SiteID); //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026
        }

        public LC_ZTABLE GetEditDataforLC_ZTABLE(int id)
        {
            return LcRepo.GetEditDataforLC_ZTABLE(id);
        }
        public bool GetDeleteDataforLC_ZTABLE(int id, int loginCode) // {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {

            return LcRepo.GetDeleteDataforLC_ZTABLE(id, loginCode); // {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        }

        public LibResult EditDataForLC_ZTABLE(int SrNo, decimal FourWheelerRate, decimal TwoWheelerRate, DateTime FromDate, DateTime ToDate, int LoginCode, long SiteID) //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026
        {

            return LcRepo.EditDataForLC_ZTABLE(SrNo, FourWheelerRate, TwoWheelerRate, FromDate, ToDate, LoginCode, SiteID); //  {old :string StateDescript} replaced with long SiteID : Added by Aumento as on 01082026
        }

        public List<object> GetSatelist()
        {
            return LcRepo.GetSatelist();
        }

        #endregion

        #region LC_EXCEPTION MASTER C TABLE
        public List<LC_EXCEPTION_C_TABLE> GetLC_EXCEPTIONList()
        {

            return LcRepo.GetLC_EXCEPTIONList(); ;


        }

        public LibResult InsertDataIntoLC_EXCEPTION(int EmpCode, decimal FuelReimbursementPerLtr, DateTime FromDate, DateTime ToDate, int LoginCode, string Remarks)
        {

            return LcRepo.InsertDataIntoLC_EXCEPTION(EmpCode, FuelReimbursementPerLtr, FromDate, ToDate, LoginCode, Remarks);
        }

        public LC_EXCEPTION_C_TABLE GetEditDataForLC_EXCEPTION(int id)
        {
            return LcRepo.GetEditDataForLC_EXCEPTION(id);
        }
        public bool GetDeleteDataForLC_EXCEPTION(int id, long LoginCode) // {LoginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {

            return LcRepo.GetDeleteDataForLC_EXCEPTION(id, LoginCode); // {LoginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        }

        public LibResult EditDataForLC_EXCEPTION(int SrNo, int EmpCode, decimal FuelReimbursementPerLtr, DateTime FromDate, DateTime ToDate, int LoginCode, string Remarks)
        {

            return LcRepo.EditDataForLC_EXCEPTION(SrNo, EmpCode, FuelReimbursementPerLtr, FromDate, ToDate, LoginCode, Remarks);
        }

        public LibResult GETEmployee(string Key)
        {

            return LcRepo.GETEmployee(Key);
        }

        #endregion


        #region LC_ReimbursementMaster B TABLE
        public List<LC_REIMBURSEMENT_B_TABLE> GetLC_ReimbursementMasterList()
        {

            return LcRepo.GetLC_ReimbursementMasterList();


        }

        public LibResult InsertDataIntoLC_ReimbursementMaster(int DesignationID, string VehicleEntitlement, decimal Fule, int Refresh, int Meal, DateTime FromDate, DateTime ToDate, int LoginCode)
        {

            return LcRepo.InsertDataIntoLC_ReimbursementMaster(DesignationID, VehicleEntitlement, Fule, Refresh, Meal, FromDate, ToDate, LoginCode);
        }

        public LC_REIMBURSEMENT_B_TABLE GetEditDataForLC_ReimbursementMaster(int id)
        {
            return LcRepo.GetEditDataForLC_ReimbursementMaster(id);
        }
        public bool GetDeleteDataForLC_ReimbursementMaster(int id, int loginCode) // {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {

            return LcRepo.GetDeleteDataForLC_ReimbursementMaster(id, loginCode); // {loginCode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        }

        public LibResult EditDataForLC_ReimbursementMaster(int SrNo, int DesignationID, string VehicleEntitlement, decimal Fule, int Refresh, int Meal, DateTime FromDate, DateTime ToDate, int LoginCode)
        {

            return LcRepo.EditDataForLC_ReimbursementMaster(SrNo, DesignationID, VehicleEntitlement, Fule, Refresh, Meal, FromDate, ToDate, LoginCode);
        }

        public LibResult GetDesignationlist()
        {

            return LcRepo.GetDesignationlist();
        }
        #endregion

        #endregion

        #region  Local Conveyance Request functions

        //public DataTable GetODDATA(string strEmpCode, string FromDate, string ToDate)
        //{

        //        return LcRepo.GetODDATA(strEmpCode,FromDate,ToDate);

        //}
        public async Task<List<LC_DETAIL_TEMP>> GetODDATA(string strEmpCode, string FromDate)  // Changes in this Function by Aumento as on 21-06-2024
        {

            return await LcRepo.GetODDATA(strEmpCode, FromDate);  // Changes in this Function by Aumento as on 21-06-2024

        }

        public LC_DETAIL_TEMP GetODDATAFromLC_DETAIL(long? EmpCode, DateTime? DateOfExpenditure, long? ASOFFODID)
        {

            return LcRepo.GetODDATAFromLC_DETAIL(EmpCode, DateOfExpenditure, ASOFFODID);

        }


        //public List<LC_DETAIL_TEMP> GetODListDATAFromLC_DETAIL(string FromDate, string ToDate, int? EmpCode)
        //{

        //    return LcRepo.GetODListDATAFromLC_DETAIL(FromDate,ToDate, EmpCode);

        //}
        public List<LC_DETAIL_TEMP> GetODListDATAFromLC_DETAIL(string FromDate, int? EmpCode, int Status)  // Remove Todate perameter by Aumento as on 21-06-2024// {Status added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {

            return LcRepo.GetODListDATAFromLC_DETAIL(FromDate, EmpCode, Status);  // Remove Todate perameter by Aumento as on 21-06-2024// {Status added} SR107643 :: LC ENHANCEMENT ::  AUMENTO

        }

        public LibResult GetEmployeeDATA(int EmpCode)
        {

            return LcRepo.GetEmployeeDATA(EmpCode);
        }


        public List<object> CityList()
        {


            return LcRepo.CityList();
        }


        public LibResult SaveDetails(LC_DETAIL_TEMP i, int logincode, string Mode) // {Mode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {

            return LcRepo.SaveDetails(i, logincode, Mode); // {Mode added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        }

        public string getStatus(LC_DETAIL_TEMP i)
        {
            return LcRepo.getStatus(i);
        }


        public LibResult Getallowcheck(int logincode, TimeSpan INTime, TimeSpan OUTTime, decimal NOOFHOURS, string MODEOFTRAVEL, int KMCOVERED)
        {

            return LcRepo.Getallowcheck(logincode, INTime, OUTTime, NOOFHOURS, MODEOFTRAVEL, KMCOVERED);
        }


        public LocalConveyanceRequestViewModel GetReqData(DateTime ExDate, int LoginEmp)
        {
            return LcRepo.GetReqData(ExDate, LoginEmp);
        }
        public LibResult SubmitData(List<LC_DETAIL_TEMP> DtData, LC_HEAD HeaderData, int logincode, Employee_Details LoginEmpDT)
        {


            return LcRepo.SubmitData(DtData, HeaderData, logincode, LoginEmpDT);
        }

        public bool UpdateLCDetailTemp(LC_DETAIL dt)
        {
            return LcRepo.UpdateLCDetailTemp(dt);
        }

        public Tuple<string, string> GetDateRange()
        {
            return LcRepo.GetDateRange();
        }
        // New Function Added by Aumento as on 21-06-2024
        public int GetMonthRangeCount()
        {
            return LcRepo.GetMonthRangeCount();
        }
        // New Function Added by Aumento as on 21-06-2024

        public Tuple<TimeSpan, TimeSpan, TimeSpan, TimeSpan> GetLunchDinnerTime()
        {

            return LcRepo.GetLunchDinnerTime();

        }



        #endregion

        #region Local Conveyance Recommandation Functions


        public List<LC_HEAD> GetListDataForReco(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {

            return LcRepo.GetListDataForReco(LoginCode, Ecode, FromDate, ToDate, Status);
        }

        public LibResult GetRequestDataForReco(int LCTRANNO, int EMPCODE)
        {

            return LcRepo.GetRequestDataForReco(LCTRANNO, EMPCODE);

        }

        public LibResult ApproveRejectReco(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode)
        {


            return LcRepo.ApproveRejectReco(LCTRANNO, EMPCODE, Response, Remark, loginCode);

        }

        #endregion

        #region Local Conveyance Approval Functions


        public List<LC_HEAD> GetListDataForApproval(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {

            return LcRepo.GetListDataForApproval(LoginCode, Ecode, FromDate, ToDate, Status);
        }

        public LibResult GetRequestDataForApproval(int LCTRANNO, int EMPCODE)
        {

            return LcRepo.GetRequestDataForApproval(LCTRANNO, EMPCODE);

        }

        public LibResult ApproveReject(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode)
        {


            return LcRepo.ApproveReject(LCTRANNO, EMPCODE, Response, Remark, loginCode);

        }

        #endregion

        #region Local Conveyance Admin Functions

        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        public List<LC_HEAD> GetListDataForAdmin(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {
            return LcRepo.GetListDataForAdmin(LoginCode, Ecode, FromDate, ToDate, Status);
        }

        public LibResult GetRequestDataForAdmin(int LCTRANNO, int EMPCODE)
        {
            return LcRepo.GetRequestDataForAdmin(LCTRANNO, EMPCODE);
        }

        public LibResult AdminSubmit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode, string Fuel, string doc, string approvetype)
        {
            return LcRepo.AdminSubmit(LCTRANNO, EMPCODE, Response, Remark, loginCode, Fuel, doc, approvetype);
        }

        public LibResult AdminSaveDetail(decimal FUEL_Reimburse, int EMPCODE, int ASOFFODID, DateTime Date, int loginCode)
        {
            return LcRepo.AdminSaveDetail(FUEL_Reimburse, EMPCODE, ASOFFODID, Date, loginCode);
        }

        #endregion
        #region Local Conveyance Admin DH Functions
        public List<LC_HEAD> GetListDataForAdminDH(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {
            return LcRepo.GetListDataForAdminDH(LoginCode, Ecode, FromDate, ToDate, Status);
        }
        public LibResult GetRequestDataForAdminDH(int LCTRANNO, int EMPCODE)
        {
            return LcRepo.GetRequestDataForAdminDH(LCTRANNO, EMPCODE);
        }
        public LibResult AdminDH_Submit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode)
        {
            return LcRepo.AdminDH_Submit(LCTRANNO, EMPCODE, Response, Remark, loginCode);
        }
        #endregion

        #region LocalConveyance Admin OH Functions
        public List<LC_HEAD> GetListDataForAdminOH(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {
            return LcRepo.GetListDataForAdminOH(LoginCode, Ecode, FromDate, ToDate, Status);
        }

        public LibResult GetRequestDataForAdminOH(int LCTRANNO, int EMPCODE)
        {
            return LcRepo.GetRequestDataForAdminOH(LCTRANNO, EMPCODE);
        }

        public LibResult AdminOH_Submit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode)
        {
            return LcRepo.AdminOH_Submit(LCTRANNO, EMPCODE, Response, Remark, loginCode);
        }
        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        #endregion

        #region  Local Conveyance Finance Functions



        public List<LC_HEAD> GetListDataForFinance(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status)
        {
            return LcRepo.GetListDataForFinance(LoginCode, Ecode, FromDate, ToDate, Status);
        }

        public LibResult GetRequestDataForFinance(int LCTRANNO, int EMPCODE)
        {

            return LcRepo.GetRequestDataForFinance(LCTRANNO, EMPCODE);

        }


        public async Task<LibResult> FinanceSubmit(int LCTRANNO, int EMPCODE, string Response, string Remark, int loginCode, string RequiredAmount, DateTime PostingDate, string ConvAmount, string MealAmout, List<LC_DETAIL> DtData)
        {
            // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

            var Data = await LcRepo.FinanceSubmit(LCTRANNO, EMPCODE, Response, Remark, loginCode, RequiredAmount, PostingDate, ConvAmount, MealAmout, DtData);
            if (Data.hasError == false)
            {
                LcRepo.ViewPDF(LCTRANNO, EMPCODE, loginCode);
            }
            return Data;
            // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

        }

        public LibResult ViewPDF(int LCTRANNO, int EMPCODE, int loginCode)
        {
            return LcRepo.ViewPDF(LCTRANNO, EMPCODE, loginCode);
        }


        #endregion

        public List<LC_HEAD> GetListDataForRecoApproval(int LoginCode)
        {
            return LcRepo.GetListDataForRecoApproval(LoginCode);
        }
        public List<LC_HEAD_ViewModel> GetDataForExcelFromDB(int LoginCode, int Ecode, DateTime FromDate, DateTime ToDate, string Status, string Flag) // {Flag added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        {
            return LcRepo.GetDataForExcelFromDB(LoginCode, Ecode, FromDate, ToDate, Status, Flag); // {Flag added} SR107643 :: LC ENHANCEMENT ::  AUMENTO
        }

        public object GetSysiteList()   // Added by Aumento as on 01082024
        {
            return LcRepo.GetSysiteList();
        }
        public object GetPaymentPlants()
        {
            return LcRepo.GetPaymentPlants();
        }
        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        public object GetRequestorDesg()
        {
            return LcRepo.GetRequestorDesg();
        }

        public object GetApprovalDesg()
        {
            return LcRepo.GetApprovalDesg();
        }

        #region Master Log View

        public async Task<List<ExpandoObject>> GetLC_Master_Log_Data(int Srno, string TblName)
        {
            return await LcRepo.GetLC_Master_Log_Data(Srno, TblName);
        }
        #endregion

        #region OH Directore maping Master

        public List<LC_OH_DIRAPPROVALFLOW_MASTERViewModel> GetLC_OH_DirApprovalFlowList()
        {
            return LcRepo.GetLC_OH_DirApprovalFlowList();
        }
        public LC_OH_DIRAPPROVALFLOW_MASTER GetEditDataforLC_OH_DIRAPPROVALFLOW_MASTERViewModel(int id)
        {
            return LcRepo.GetEditDataforLC_OH_DIRAPPROVALFLOW_MASTERViewModel(id);
        }

        public LibResult EditDataforLC_OH_DIRAPPROVALFLOW_MASTER(int SrNo, string RequestorDesg, string ApprovalDesg, int LoginCode)
        {

            return LcRepo.EditDataforLC_OH_DIRAPPROVALFLOW_MASTER(SrNo, RequestorDesg, ApprovalDesg, LoginCode);
        }
        #endregion

        #region Approval History

        public DataTable GetApprovalHistory(int lctranno)
        {
            return LcRepo.GetApprovalHistory(lctranno);
        }
        #endregion

        public List<LC_HEAD> GetListDataForRejectedRequest(int LoginCode)
        {
            return LcRepo.GetListDataForRejectedRequest(LoginCode);
        }

        public LibResult OpenRejectedRequest(int LCTRANNO, int EMPCODE)
        {
            return LcRepo.OpenRejectedRequest(LCTRANNO, EMPCODE);
        }
        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO    
    }
}
