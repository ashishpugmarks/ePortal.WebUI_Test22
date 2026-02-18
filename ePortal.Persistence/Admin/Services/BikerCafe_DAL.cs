using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Admin.Services
{
    public class BikerCafe_DAL:IBikerCafe_DAL
    {
        #region "Local Variables"
        DataSet ds = new DataSet();

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;

        //OracleCommand oCmd;
        //DataSet ds;
        //DataTable Dt;
        //OracleConnection objConn;
        //string strConn;
        //string strErrMsg = string.Empty;
        #endregion

        public BikerCafe_DAL(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }

        public DataTable GetMealTypeList()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALTYPELIST_GET";
            oCmd.Parameters.Add("CUR_MEALTYPE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetMealMstList(string MealType, string strstatus)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALLIST_GET";
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetMealAllocationList(string MealType, string Status, string FromDate, string TillDate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEAL_ALLOCATIONLIST_GET";
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = Status;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("CUR_MEALALLOCATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public string InsertMealAllocation(long ID, long MealTypeId, string MappingDate, long MealId, short status, long addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALALLOCATION_INSERT"; //"PKG_TOURREQUEST.SPROC_TOURAUTHORITY_INSERT";
            oCmd.Parameters.Add("AVAILABILITY_ID_IN", OracleDbType.Int64).Value = ID;
            oCmd.Parameters.Add("MEALTYPEID_IN", OracleDbType.Int64).Value = MealTypeId;
            oCmd.Parameters.Add("MAPPINGDATE_IN", OracleDbType.Varchar2).Value = MappingDate;
            oCmd.Parameters.Add("MEALID_IN", OracleDbType.Int64).Value = MealId;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int64).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public DataTable GetMealdatewiseList(string ecode, string FromDate, string TillDate, string MealType, string reporttype)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALSREQLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("REPORTTYPE_IN", OracleDbType.Int16).Value = reporttype;
            oCmd.Parameters.Add("CUR_MEALS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //SET CATEGORY LIST IN HELPDESK
        public string MealType_Set(string strCategoryID, string strDescription, string strReader, string strStatus, string strEmpcode)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            string strResult = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALTYPE_SET";
            objCmd.Parameters.Add("MEALTYPEID_IN", OracleDbType.Varchar2).Value = strCategoryID;
            objCmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;
            objCmd.Parameters.Add("REDARCODE_IN", OracleDbType.Varchar2).Value = strReader;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strEmpcode;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            objCmd.BindByName = true;
            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);
            errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
            strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (strResult + "#" + errMsg);
            objCmd.Dispose();
        }
        public string UpdateAvailabilityStatus(long ID, short status, long updatedBy)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_AVAILSTATUS_UPDATE";
            oCmd.Parameters.Add("AVAILABILITY_ID_IN", OracleDbType.Int64).Value = ID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("UPDATEDBY_IN", OracleDbType.Int64).Value = updatedBy;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public string AddMeal_Set(string strMealID, string strMealTypeID, string strSMealName, string strSMealDesc, string mealPrice, string mealPhotoName, byte[] mealPhoto, string strStatus, string strUserID)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            string strResult = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ADDMEAL_SET";
            objCmd.Parameters.Add("MEALID_IN", OracleDbType.Varchar2).Value = strMealID;
            objCmd.Parameters.Add("MEALTYPEID_IN", OracleDbType.Varchar2).Value = strMealTypeID;
            objCmd.Parameters.Add("MEALNAME_IN", OracleDbType.Varchar2).Value = strSMealName;
            objCmd.Parameters.Add("MEALDESCRIPTION_IN", OracleDbType.Varchar2).Value = strSMealDesc;
            objCmd.Parameters.Add("MEALPRICE_IN", OracleDbType.Varchar2).Value = mealPrice;
            objCmd.Parameters.Add("MEALPHOTO_IN", OracleDbType.Blob).Value = mealPhoto;
            objCmd.Parameters.Add("PHOTONAME_IN", OracleDbType.Varchar2).Value = mealPhotoName;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strUserID;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            objCmd.BindByName = true;
            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);
            errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
            strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (strResult + "#" + errMsg);
            objCmd.Dispose();
        }
        public string SaveSubsidizedMealToken(long ID, string TOKEN_CODE, long EMPLOYEE_ID, string TOKEN_DATE, short STATUS, long ADDEDBY)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_SUBSIDIZED_INSERT";
            oCmd.Parameters.Add("SUBSIDIZED_MEALID_IN", OracleDbType.Int64).Value = ID;
            oCmd.Parameters.Add("TOKEN_CODE_IN", OracleDbType.Varchar2).Value = TOKEN_CODE;
            oCmd.Parameters.Add("EMPLOYEE_ID_IN", OracleDbType.Int64).Value = EMPLOYEE_ID;
            oCmd.Parameters.Add("TOKEN_DATE_IN", OracleDbType.Varchar2).Value = TOKEN_DATE;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = STATUS;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int64).Value = ADDEDBY;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public DataTable SubsidizedMealTokenList(string TOKEN_CODE, long EMPLOYEE_ID, string STATUS, string FromDate, string TillDate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_SUBSIDIZEDLIST_GET";
            oCmd.Parameters.Add("TOKEN_CODE_IN", OracleDbType.Varchar2).Value = TOKEN_CODE;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = EMPLOYEE_ID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("CUR_SUBSIDIZED", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable OffdayList(string FromDate, string TillDate, string strmealtype)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_OFFDATELIST_GET";
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("MEALS_ID_IN", OracleDbType.Varchar2).Value = strmealtype;
            oCmd.Parameters.Add("CUR_OFFDAYLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //HR ID meal booking & Subsidized food
        public string UpdateSubsidizedAmt(Int64 TRNID, Int64 SubsidizedTokenID, Int64 EmpCode, string strTotalAmt, string strSubsidizedAmt, string strPayableAmt, string strStatus, string strUserID)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            string strResult = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.CommandText = "PKG_BIKERSCAFE.SPROC_UPDATE_SUBSIDIZED_AMT";
            objCmd.Parameters.Add("SUBSIDIZEDTOKENID_IN", OracleDbType.Int64).Value = SubsidizedTokenID;
            objCmd.Parameters.Add("SUBSIDIZEDTRNID_IN", OracleDbType.Int64).Value = TRNID;
            objCmd.Parameters.Add("EMPLOYEE_IN", OracleDbType.Int64).Value = EmpCode;
            objCmd.Parameters.Add("TOTALAMT_IN", OracleDbType.Varchar2).Value = strTotalAmt;
            objCmd.Parameters.Add("SUBSIDIZEDAMT_IN", OracleDbType.Varchar2).Value = strSubsidizedAmt;
            objCmd.Parameters.Add("PAYABLEAMT_IN", OracleDbType.Varchar2).Value = strPayableAmt;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strUserID;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            objCmd.BindByName = true;
            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);
            errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
            strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (strResult + "#" + errMsg);
            objCmd.Dispose();
        }
        public DataTable GetSubsidizedDataByOtp(string otp, string tokenDate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_SUBSIDIZEDDATA_GET";
            oCmd.Parameters.Add("TOKEN_CODE_IN", OracleDbType.Varchar2).Value = otp;
            oCmd.Parameters.Add("TOKEN_DATE_IN", OracleDbType.Varchar2).Value = tokenDate;
            oCmd.Parameters.Add("CUR_SUBSIDIZED", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetSubsidizedReport(string ecode, string FromDate, string TillDate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_SUBSIDIZEDREPORT_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("CUR_SUBSIDIZED", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetMealBookingData(string MealType, string FromDate, string TillDate, long loginUser, int type, int mealStatus)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALBOOKING_GET";
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("LOGINUSER_IN", OracleDbType.Int64).Value = loginUser;
            oCmd.Parameters.Add("TYPE_IN", OracleDbType.Int16).Value = type;
            oCmd.Parameters.Add("MEAL_STATUS_IN", OracleDbType.Int16).Value = mealStatus;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetValidationData()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_VALIDATIONMST_GET";
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public string InsertMealBooking(long ID, long MealTypeId, string bookingDate, short status, long addedby, int validBookingCount, long slotID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALBOOKING_INSERT"; //"PKG_TOURREQUEST.SPROC_TOURAUTHORITY_INSERT";
            oCmd.Parameters.Add("AVAILABILITY_ID_IN", OracleDbType.Int64).Value = ID;
            oCmd.Parameters.Add("MEALTYPEID_IN", OracleDbType.Int64).Value = MealTypeId;
            oCmd.Parameters.Add("BOOKINGDATE_IN", OracleDbType.Varchar2).Value = bookingDate;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int64).Value = addedby;
            oCmd.Parameters.Add("VALIDBOOKINGCOUNT_IN", OracleDbType.Int32).Value = validBookingCount;
            oCmd.Parameters.Add("SLOT_IN", OracleDbType.Int64).Value = slotID;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }
        public string BookingCancellation(long ID, short status, long updatedBy)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_BOOKINGCANCELLATION";
            oCmd.Parameters.Add("BOOKING_ID_IN", OracleDbType.Int64).Value = ID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("UPDATEDBY_IN", OracleDbType.Int64).Value = updatedBy;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }
        public DataSet ManageGuestMealApproval(string strSupEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_BIKERSCAFE.SPROC_PENDING_GM_APPROVAL";
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataSet objDs = new DataSet();
                    objAdr.Fill(objDs);
                    return objDs;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
        }

        public string UpdateMealTakenStatus(Int64 TRNID, Int64 GuestDTLID, string strStatus, string strUserID)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            string strResult = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.CommandText = "PKG_BIKERSCAFE.SPROC_GUESTMEALSTATUS_UPDATE";
            objCmd.Parameters.Add("GUESTMEAL_TRNID_IN", OracleDbType.Int64).Value = TRNID;
            objCmd.Parameters.Add("GUESTMEAL_DTLID_IN", OracleDbType.Int64).Value = GuestDTLID;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strUserID;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            objCmd.BindByName = true;
            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);
            errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
            strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (strResult + "#" + errMsg);
            objCmd.Dispose();
        }

        public DataTable GetGuestMealBookingByOtp(string otp, string bookingDate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_GUESTMEALBYOTP_GET";
            oCmd.Parameters.Add("TOKEN_CODE_IN", OracleDbType.Varchar2).Value = otp;
            oCmd.Parameters.Add("BOOKING_DATE_IN", OracleDbType.Varchar2).Value = bookingDate;
            oCmd.Parameters.Add("CUR_GUESTMEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetGuestMealReqList(string ecode, string FromDate, string TillDate, string MealType)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_GUESTREQLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("CUR_MEALS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetMealCountList(string FromDate, string TillDate, string MealType)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALREQCOUNT_GET";
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("CUR_MEALS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetMealReqCostList(string ecode, string FromDate, string TillDate, string MealType)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALREQCOSTLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("CUR_MEALS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public string UpdateValidationMst(long BC_ValidationId, string strUserID, string MealUpdatePeriod, string MealChangeTime, string MealBookingDuration, string MealBookingTime,
              string MealCancellationTime, string MealBookingDay, string IndQty, string JapQty, string AlaCarteSubsidyPercentage,
              string AlaCarteMaxSubsidy, string AlaCarteBookingTime, string GuestBookingTime, string GuestCancellationTime,
              string GuestBookingDay, string SelfAppDesg, string JapBookingTime, string JapCancellationTime, string JapBookingDay, string FmyVisitEndDate, string FmyVisitQty,
              string AlaOTPAcceptTime) //CR-2678 Change
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            string strResult = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.CommandText = "PKG_BIKERSCAFE.SPROC_UPDATE_VALIDATION_MST";
            objCmd.Parameters.Add("BC_VALIDATIONID_IN", OracleDbType.Int64).Value = BC_ValidationId;
            objCmd.Parameters.Add("MEALUPDATEPERIOD_IN", OracleDbType.Varchar2).Value = MealUpdatePeriod;
            objCmd.Parameters.Add("MEALCHANGETIME_IN", OracleDbType.Varchar2).Value = MealChangeTime;
            objCmd.Parameters.Add("MEALBOOKINGDURATION_IN", OracleDbType.Varchar2).Value = MealBookingDuration;
            objCmd.Parameters.Add("MEALBOOKINGTIME_IN", OracleDbType.Varchar2).Value = MealBookingTime;
            objCmd.Parameters.Add("MEALCANCELLATIONTIME_IN", OracleDbType.Varchar2).Value = MealCancellationTime;
            objCmd.Parameters.Add("MEALBOOKINGDAY_IN", OracleDbType.Varchar2).Value = MealBookingDay;
            objCmd.Parameters.Add("INDQTY_IN", OracleDbType.Varchar2).Value = IndQty;
            objCmd.Parameters.Add("JAPQTY_IN", OracleDbType.Varchar2).Value = JapQty;
            objCmd.Parameters.Add("ALACARTESUBSIDY_IN", OracleDbType.Varchar2).Value = AlaCarteSubsidyPercentage;
            objCmd.Parameters.Add("ALACARTEMAXSUBSIDY_IN", OracleDbType.Varchar2).Value = AlaCarteMaxSubsidy;
            objCmd.Parameters.Add("ALACARTEBOOKINGTIME_IN", OracleDbType.Varchar2).Value = AlaCarteBookingTime;
            objCmd.Parameters.Add("GUESTBOOKINGTIME_IN", OracleDbType.Varchar2).Value = GuestBookingTime;
            objCmd.Parameters.Add("GUESTCANCELLATIONTIME_IN", OracleDbType.Varchar2).Value = GuestCancellationTime;
            objCmd.Parameters.Add("GUESTBOOKINGDAY_IN", OracleDbType.Varchar2).Value = GuestBookingDay;
            objCmd.Parameters.Add("SELFAPPDESG_IN", OracleDbType.Varchar2).Value = SelfAppDesg;
            objCmd.Parameters.Add("JAPBOOKINGTIME_IN", OracleDbType.Varchar2).Value = JapBookingTime;
            objCmd.Parameters.Add("JAPCANCELLATIONTIME_IN", OracleDbType.Varchar2).Value = JapCancellationTime;
            objCmd.Parameters.Add("JAPBOOKINGDAY_IN", OracleDbType.Varchar2).Value = JapBookingDay;
            objCmd.Parameters.Add("FMYVISIT_ENDDATE_IN", OracleDbType.Varchar2).Value = FmyVisitEndDate;
            objCmd.Parameters.Add("FMYVISITQTY_IN", OracleDbType.Varchar2).Value = FmyVisitQty;
            objCmd.Parameters.Add("ALAOTPACCEPTTIME_IN", OracleDbType.Varchar2).Value = AlaOTPAcceptTime;  //CR-2678 Change
            objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strUserID;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            objCmd.BindByName = true;
            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);
            errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
            strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (strResult + "#" + errMsg);
            objCmd.Dispose();
        }

        public DataTable BindDesignation()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_BINDDESIGNATION_GET";
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetHRIDMappingData(string id, string ecode, string status)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_HRIDCODEMAPPING_GET";
            oCmd.Parameters.Add("HRECODEMAPID_IN", OracleDbType.Varchar2).Value = id;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public string InsertHRCodeMapping(string ecode, string hridCode, string saviorCode, string status, string addedby, string mappingId)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_HRIDCODEMAPPING_INSERT";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("HRIDCODE_IN", OracleDbType.Varchar2).Value = hridCode;
            oCmd.Parameters.Add("SAVIORCODE_IN", OracleDbType.Varchar2).Value = saviorCode;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("HRECODEMAPID_IN", OracleDbType.Varchar2).Value = mappingId;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        //--- Meal Booking By Admin ---//
        public DataTable GetMealBookingByAdmin(string MealType, string FromDate, string TillDate, long loginUser, int type, int mealStatus)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ADMINMEALBOOKING_GET";
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("LOGINUSER_IN", OracleDbType.Int64).Value = loginUser;
            oCmd.Parameters.Add("TYPE_IN", OracleDbType.Int16).Value = type;
            oCmd.Parameters.Add("MEAL_STATUS_IN", OracleDbType.Int16).Value = mealStatus;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public string InsertMealBookingByAdmin(long ID, long empCode, long MealTypeId, string bookingDate, short status, long requestBy, int validBookingCount, Int64 slotID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ADMINMEALBOOKING_INSERT"; //"PKG_TOURREQUEST.SPROC_TOURAUTHORITY_INSERT";
            oCmd.Parameters.Add("AVAILABILITY_ID_IN", OracleDbType.Int64).Value = ID;
            oCmd.Parameters.Add("MEALTYPEID_IN", OracleDbType.Int64).Value = MealTypeId;
            oCmd.Parameters.Add("BOOKINGDATE_IN", OracleDbType.Varchar2).Value = bookingDate;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = empCode;
            oCmd.Parameters.Add("REQUESTBY_IN", OracleDbType.Int64).Value = requestBy;
            oCmd.Parameters.Add("VALIDBOOKINGCOUNT_IN", OracleDbType.Int32).Value = validBookingCount;
            oCmd.Parameters.Add("SLOT_IN", OracleDbType.Int64).Value = slotID;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public string BookingCancellationByAdmin(long ID, short status, long updatedBy)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ADMINBOOKINGCANCELLATION";
            oCmd.Parameters.Add("BOOKING_ID_IN", OracleDbType.Int64).Value = ID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("UPDATEDBY_IN", OracleDbType.Int64).Value = updatedBy;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }
        public DataTable GetMealSlot(string MealType)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEALSLOT_GET";
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        // -- Ala Carte Order --//
        public DataTable GetAlaCarteOrderByAdmin(string FromDate, string TillDate, string ecode, int status)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ALACARTEORDER_GET";
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataSet GetOrderDetailById(long id)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ALACARTEORDERBYID_GET";
            oCmd.Parameters.Add("HDRID_IN", OracleDbType.Int64).Value = id;
            oCmd.Parameters.Add("CUR_ALACARTE_HDR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_ALACARTE_TRN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        public string UpdateOrderStatus(long hdrId, long trnId, Int16 item_status, Int16 order_status, string addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ALACARTEORDER_UPDATE";
            oCmd.Parameters.Add("HDRID_IN", OracleDbType.Int64).Value = hdrId;
            oCmd.Parameters.Add("TRNID_IN", OracleDbType.Int64).Value = trnId;
            oCmd.Parameters.Add("ITEM_STATUS_IN", OracleDbType.Int16).Value = item_status;
            oCmd.Parameters.Add("ORDER_STATUS_IN", OracleDbType.Int16).Value = order_status;
            oCmd.Parameters.Add("UPDATEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public DataTable GetAlacarteTypeList()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ALACARTETYPELIST_GET";
            oCmd.Parameters.Add("CUR_MEALTYPE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetAlacarteMstList(string MealType, string strstatus)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ALACARTEMENULIST_GET";
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public string AddAlaCarte_Set(string strMealID, string strMealTypeID, string strSMealName, string strSMealPreTime, string mealPrice, string mealPhotoName, byte[] mealPhoto, string strTakeway, string strDinein, string strStatus, string strUserID)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            string strResult = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.CommandText = "PKG_BIKERSCAFE.SPROC_ADDALACARTEMEAL_SET";
            objCmd.Parameters.Add("MEALID_IN", OracleDbType.Varchar2).Value = strMealID;
            objCmd.Parameters.Add("MEALTYPEID_IN", OracleDbType.Varchar2).Value = strMealTypeID;
            objCmd.Parameters.Add("MEALNAME_IN", OracleDbType.Varchar2).Value = strSMealName;
            objCmd.Parameters.Add("MEALPRETIME_IN", OracleDbType.Varchar2).Value = strSMealPreTime;
            objCmd.Parameters.Add("MEALPRICE_IN", OracleDbType.Varchar2).Value = mealPrice;
            objCmd.Parameters.Add("MEALPHOTO_IN", OracleDbType.Blob).Value = mealPhoto;
            objCmd.Parameters.Add("PHOTONAME_IN", OracleDbType.Varchar2).Value = mealPhotoName;
            objCmd.Parameters.Add("TAKEWAY_IN", OracleDbType.Varchar2).Value = strTakeway;
            objCmd.Parameters.Add("DINEIN_IN", OracleDbType.Varchar2).Value = strDinein;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strUserID;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            objCmd.BindByName = true;
            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);
            errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
            strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (strResult + "#" + errMsg);
            objCmd.Dispose();
        }

        public DataTable GetFamilyMealCountList(string FromDate, string TillDate, string MealType)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_FAMILYMEALREQCOUNT_GET";
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("CUR_MEALS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetFamilyMealBookingByOtp(string otp, string bookingDate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_FAMILYMEALBYOTP_GET";
            oCmd.Parameters.Add("TOKEN_CODE_IN", OracleDbType.Varchar2).Value = otp;
            oCmd.Parameters.Add("BOOKING_DATE_IN", OracleDbType.Varchar2).Value = bookingDate;
            oCmd.Parameters.Add("CUR_FAMILYMEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetFamilyReqdatewiseList(string FromDate, string TillDate, string reporttype)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_FAMILYREQLIST_GET";
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("REPORTTYPE_IN", OracleDbType.Int16).Value = reporttype;
            oCmd.Parameters.Add("CUR_MEALS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //Days master-Start
        public DataTable SlotDateList(string FromDate, string TillDate, string strmealtype)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_SLOTDATELIST_GET";
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("SLOT_ID_IN", OracleDbType.Varchar2).Value = strmealtype;
            oCmd.Parameters.Add("CUR_SLOTDATELIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public string InsertUpdateSlotAllocation(long slotTypeID, string mappingDate, short status, long addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_SLOTDATE_INSERT_UPDATE"; //"PKG_TOURREQUEST.SPROC_TOURAUTHORITY_INSERT";
            oCmd.Parameters.Add("SLOT_ID_IN", OracleDbType.Int64).Value = slotTypeID;
            oCmd.Parameters.Add("MAPPING_DATE_IN", OracleDbType.Varchar2).Value = mappingDate;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int64).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }
        //Days master-End
        //Meeting food block

        public DataTable GetMeetingfoodMstList(string MealType, string strstatus)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEETINGMENULIST_GET";
            oCmd.Parameters.Add("MEALTYPE_IN", OracleDbType.Varchar2).Value = MealType;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public string AddMeetingfood_Set(string strMealID, string strMealTypeID, string strSMealName, string mealPrice, string mealPhotoName, byte[] mealPhoto, string strStatus, string strUserID)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            string strResult = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEETINGMEAL_SET";
            objCmd.Parameters.Add("MEALID_IN", OracleDbType.Varchar2).Value = strMealID;
            objCmd.Parameters.Add("MEALTYPEID_IN", OracleDbType.Varchar2).Value = strMealTypeID;
            objCmd.Parameters.Add("MEALNAME_IN", OracleDbType.Varchar2).Value = strSMealName;
            objCmd.Parameters.Add("MEALPRICE_IN", OracleDbType.Varchar2).Value = mealPrice;
            objCmd.Parameters.Add("MEALPHOTO_IN", OracleDbType.Blob).Value = mealPhoto;
            objCmd.Parameters.Add("PHOTONAME_IN", OracleDbType.Varchar2).Value = mealPhotoName;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strUserID;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            objCmd.BindByName = true;
            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);
            errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
            strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (strResult + "#" + errMsg);
            objCmd.Dispose();
        }

        public DataSet ManageMeetingFoodApproval(string strSupEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_BIKERSCAFE.SPROC_PENDING_MF_APPROVAL";
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataSet objDs = new DataSet();
                    objAdr.Fill(objDs);
                    return objDs;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
        }

        public DataTable GetMeetingFoodByAdmin(string FromDate, string TillDate, string ecode, int status)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_METINGFOODORDER_GET";
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataSet GetMeetingfoodOrderDetailById(long id)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEETINFOODGORDERBYID_GET";
            oCmd.Parameters.Add("HDRID_IN", OracleDbType.Int64).Value = id;
            oCmd.Parameters.Add("CUR_MEETINGFOOD_HDR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MEETINGFOOD_TRN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        public string SUBMITADMINMEETINGFOOD(string HDID, string strempid, string strRemarks, string strstatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_BIKERSCAFE.SPROC_SUBMITMEETINGADMIN";
            ocmd.Parameters.Add("HDID_IN", OracleDbType.Varchar2).Value = HDID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strempid;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        public DataTable GetMeetingFoodByCafe(string FromDate, string TillDate, string ecode, int status)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_METINGFOODORDERCAFE_GET";
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int16).Value = status;
            oCmd.Parameters.Add("CUR_MEAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataSet GetMeetingfoodOrdercafeById(long id)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MEETINFOODGORDECAFEBYID_GET";
            oCmd.Parameters.Add("HDRID_IN", OracleDbType.Int64).Value = id;
            oCmd.Parameters.Add("CUR_MEETINGFOOD_HDR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MEETINGFOOD_TRN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        public string SUBMITCAFEMEETINGFOOD(string HDID, string strempid, string strRemarks, string strstatus, string strXml, string STRATTACHMENT)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_BIKERSCAFE.SPROC_SUBMITMEETINGCAFE";
            ocmd.Parameters.Add("HDID_IN", OracleDbType.Varchar2).Value = HDID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strempid;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            ocmd.Parameters.Add("XMLSERVICE_TYPE", OracleDbType.Varchar2).Value = strXml;
            ocmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = STRATTACHMENT;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable GetMeetingFoodReqList(string ecode, string FromDate, string TillDate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_BIKERSCAFE.SPROC_MeetingFoodREQLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("CUR_MEALS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
    }
}
