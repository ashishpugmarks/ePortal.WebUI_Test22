using ePortal.Persistence.Interface;
using ePortal.Persistence.TourRequest.Interface;
using ePortal.ViewModels.APPX.TourRequest;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections;
using System.Data;
using System.Globalization;

namespace ePortal.Persistence.TourRequest.Services
{
    public class TourQueries : ITourQueries
    {
        private readonly IConfiguration configuration;
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        public TourQueries(IConfiguration _configuration, IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            configuration = _configuration;
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }
        #region "Local Variables"
        private string _ErrorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        DataRow[] _datarow;
        //DataManagement oDataMgmt = new DataManagement();
        #endregion

        #region "Get Data Result"
        /// <summary>
        /// GET ACTIVE CITY LIST
        /// </summary>
        /// <returns></returns>
        
        public DataSet GetCityList()
        {
            string strConn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    OracleCommand oCmd = new OracleCommand();
                    ds = new DataSet();
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.CommandText = "PKG_TOURREQUEST.SPROC_CITYLIST_GET";
                    oCmd.Parameters.Add("CUR_CITYLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    //GET DATA FROM DATA ACCESS LAYER
                    ds = oDataMgmt.GetDataSet(oCmd);
                    return (ds);
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
        /*Added by TTL on 16-Oct-2025 against SR110729 > CR7297 Start*/
        public DataSet GetCityListWithInactive(int CityId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_CITYLIST_WITH_INACTIVE_GET";
            oCmd.Parameters.Add("CITYID_IN", OracleDbType.Int32).Value = CityId;
            oCmd.Parameters.Add("CUR_CITYLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        public DataSet GetCityListOnTourRequestId(int RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_CITYLIST_ON_TOURREQID_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_CITYLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        /*Added by TTL on 16-Oct-2025 against SR110729 > CR7297 End*/

        public DataSet GetStateList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_STATELIST_GET";
            oCmd.Parameters.Add("CUR_STATELIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        #endregion

        #region "Get Hotel Details List"
        /// <summary>
        /// GET ACTIVE CITY LIST
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataSet GetHotelDetailsListByCity(string strCityId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_HOTELDETAILS_BYCITY_GET";
            oCmd.Parameters.Add("CUR_HOTELLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CITYID_IN", OracleDbType.Int32).Value = Convert.ToInt32(strCityId);

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        #endregion

        #region "Get single Hotel Details List"
        /// <summary>
        /// GET ACTIVE CITY LIST
        /// </summary>
        /// <param name="strHotelId"></param>
        /// <returns></returns>
        public DataTable GetSingleHotelDetailsByHotel(string strHotelId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_SINGLEHOTELDETAILS_GET";
            oCmd.Parameters.Add("CUR_HOTELLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("HOTELID_IN", OracleDbType.Int32).Value = strHotelId == "" ? null : strHotelId;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        #endregion

        /// <summary>
        /// GET CITY CATEGORY LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetCityCategoryList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_CITYCATLIST_GET";
            oCmd.Parameters.Add("CUR_CITYCATLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// GET TRAVEL MODE LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetTravelModeList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TRAVELMODELIST_GET";
            oCmd.Parameters.Add("CUR_TRAVELMODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }

        /// <summary>
        /// GET TRAVEL MODE CLASS AGAINST TRAVEL MODE
        /// </summary>
        /// <param name="ModeID"></param>
        /// <returns></returns>
        public DataSet GetTravelModeClass(string ModeID)
        {
            if (ModeID == "")
                ModeID = "0";

            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TRAVELMODECLASS_GET";
            oCmd.Parameters.Add("CUR_MODECLASS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("MODE_IN", OracleDbType.Varchar2).Value = ModeID;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// GET EMPLOYEE BASIC DETAILS
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable EmployeeDetail(int EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_EMPDETAILS_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        // GET GSTINNO using City on 21-01-2023(Aumento)
        //------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// GET GSTINNO BASIC DETAILS
        /// </summary>
        /// <param name="CityId"></param>
        /// <returns></returns>
        public DataTable GSTNODetail(string CityId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_GSTINNOCITYCLASS_GET";
            oCmd.Parameters.Add("CITY_IN", OracleDbType.Varchar2).Value = CityId;
            oCmd.Parameters.Add("CUR_CITYCLASS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        //------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// FUNCTION CONVERT STRING TO DATE
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public DateTime ConvertToDate(string strDate)
        {
            DateTime _dtDate;
            int dd, mm, yy;
            string[] arr;
            arr = strDate.Split('-');
            dd = Convert.ToInt16(arr[0]);
            mm = Convert.ToInt16(arr[1]);
            yy = Convert.ToInt16(arr[2]);

            _dtDate = System.Convert.ToDateTime(mm + "-" + dd + "-" + yy);
            return (_dtDate);
        }

        /// <summary>
        /// GET CITY CATEGORY AGAINST SELECTED CITY
        /// </summary>
        /// <param name="CityCode"></param>
        /// <returns></returns>
        public DataRow[] GetCityCategory(string CityCode)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_CITYCATEGORY_GET";
            oCmd.Parameters.Add("CUR_CITYCAT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CITYID_IN", OracleDbType.Varchar2).Value = CityCode;

            //GET DATA FROM DATA ACCESS LAYER
            _datarow = oDataMgmt.GetDataRow(oCmd);
            return (_datarow);
        }

        /// <summary>
        /// GET TOUR ALLOWANCE DETAIL PERIO WISE
        /// </summary>
        /// <param name="CityCategoryID"></param>
        /// <param name="EmpDesignationID"></param>
        /// <param name="ApplicationDate"></param>
        /// <returns></returns>
        public DataRow[] GetEmployeeAllowanceDetail(int CityCategoryID, int EmpDesignationID, string ApplicationDate
        , string UserType // Added by Kishan Dodiya
        )
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_ALLOWANCEDETAIL_GET";
            oCmd.Parameters.Add("CITYCATEGORY_IN", OracleDbType.Int32).Value = CityCategoryID;
            oCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Int32).Value = EmpDesignationID;
            oCmd.Parameters.Add("APPLICATIONDATE_IN", OracleDbType.Varchar2).Value = ApplicationDate;
            oCmd.Parameters.Add("USERTYPE_IN", OracleDbType.Varchar2).Value = UserType; // Added by Kishan Dodiya
            oCmd.Parameters.Add("CUR_ALLOWANCE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            _datarow = oDataMgmt.GetDataRow(oCmd);
            return (_datarow);
        }

        /// <summary>
        /// GET TOUR APPROVE AUTHORITY OF EMPLOYEE
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetAppAuthorities(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_APPAUTHORITY_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public string getConnectingString()
        {
            string strCn = string.Empty;
            strCn = objCnStr.getConnectingString();//ConfigurationManager.ConnectionStrings["cnConn"].ConnectionString;
            return strCn;
        }

        //Below Function added by aumento for the SR50547============================================
        public string GetParameterValue(string ParamValue)
        {
            string strCn;
            OracleCommand objCmd;
            string strparavalue = string.Empty;

            strCn = getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_SYPARAMVALUE_GET";
                    objCmd.Parameters.Add("PARAM_IN", OracleDbType.Varchar2).Value = ParamValue;
                    objCmd.Parameters.Add("PARAM_VAL", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToString(objCmd.Parameters["PARAM_VAL"].Value);
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
        //================================================================================================

        ///// <summary>
        ///// GET TOUR APPROVE AUTHORITY OF EMPLOYEE
        ///// </summary>
        ///// <param name="ParamValue"></param>
        ///// <returns></returns>
        //public string GetParameterValue(string ParamValue)
        //{
        //    OracleCommand oCmd = new OracleCommand();
        //    DataTable dt = new DataTable();
        //    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //    oCmd.BindByName = true;
        //    oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_SYPARAMVALUE_GET";
        //    oCmd.Parameters.Add("PARAM_IN", OracleDbType.Varchar2).Value = ParamValue;
        //    oCmd.Parameters.Add("PARAM_VAL", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

        //    //GET DATA FROM DATA ACCESS LAYER
        //    dt = oDataMgmt.GetDataTable(oCmd);
        //    return "";
        //}












        //Chnge by Aumento on 06012023=======================================================
        public DataSet GetGSTINNoList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_GSTINNOLIST_GET";
            oCmd.Parameters.Add("CUR_GSTINNOLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        //====================================================================================

        //Get tour advance data
        public DataTable GetTouradvancedata(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TOURADVANCEDATA_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_REQTADATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET APPROVAL AUTHORITY LIST FOR TOUR APPROVAL
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetAppAuthorityList(string EmpCode, string ReqEmpcode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_APPAUTHORITYLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("REQEMPCODE_IN", OracleDbType.Varchar2).Value = ReqEmpcode;
            oCmd.Parameters.Add("CUR_APPAUTHLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET SPECIAL APPROVAL AUTHORITY LIST
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetSpecialAppAuthorityList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_SPAPPAUTHORITYLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPAUTHLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET TOUR REQUEST LIST(PENDING)
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetPendingRequestList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_PENDINGREQLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_PENREQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET REQUEST LIST FOR HISTORY(APPROVED OR REJECTED)
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetHistoryRequestList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_HISTORYREQLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_HISREQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET TOUR REQUEST HISTORY AGAINST REQUEST ID FOR SHOW REQUEST HISTORY
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public DataTable GetRequestHistory(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQHISTORY_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_REQHISTORY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET REQUEST DETAIL AGAINST REQUEST ID FOR REQUEST CANCELLATION
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public DataTable GetRequestDetail(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQDETAIL_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_REQDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET REQUEST HEADER FOR REQUEST MODIFICATION
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        public DataTable GetRequestHeaderPart(string RequestID, string UserCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQHDRPART_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserCode;
            oCmd.Parameters.Add("CUR_REQHDRPART", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET REQUEST DETAIL FOR REQUEST MODIFICATION 
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public ArrayList GetRequestDetailPart(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            ArrayList oTourList = new ArrayList();
            string TourObjective = string.Empty;
            string TourFromDate = string.Empty;
            string TourTime = string.Empty;
            string FromLoc = string.Empty;
            string ToLoc = string.Empty;
            string StayingLoc = string.Empty;
            string FromLocCode = string.Empty;
            string ToLocCode = string.Empty;
            string StayingLocCode = string.Empty;
            string Mode = string.Empty;
            string ModeID = string.Empty;
            string ModeDetail = string.Empty;
            string TicketClass = string.Empty;
            string TicketClasssID = string.Empty;
            string TickingBy = string.Empty;
            string HotelReserv = string.Empty;
            string PickDrop = string.Empty;
            string SpecialApp = string.Empty;
            string Remarks = string.Empty;
            string StrIdtype = string.Empty;
            string StrIdno = string.Empty;
            string strRequestDetailID = string.Empty;
            string TourTimeTo = string.Empty;// Added by Kishan Dodiya
            string PreferredLocation = string.Empty;// Added by Kishan Dodiya

            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQDETAILPART_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_REQDETAILPART", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            for (int rownum = 0; rownum < dt.Rows.Count; rownum++)
            {

                TourObjective = dt.Rows[rownum]["DAYOBJECTIVE"].ToString();
                TourFromDate = dt.Rows[rownum]["TRAVELFROMDATE"].ToString();
                TourTime = dt.Rows[rownum]["TRAVELTIME"].ToString();
                FromLocCode = dt.Rows[rownum]["FROMCITYID"].ToString();
                FromLoc = dt.Rows[rownum]["FRMCITY"].ToString();
                ToLocCode = dt.Rows[rownum]["TOCITYID"].ToString();
                ToLoc = dt.Rows[rownum]["TOCITY"].ToString();
                StayingLocCode = dt.Rows[rownum]["STAYINGCITYID"].ToString();
                StayingLoc = dt.Rows[rownum]["STCITY"].ToString();
                ModeID = dt.Rows[rownum]["ADTRAVELMODEID"].ToString();
                Mode = dt.Rows[rownum]["TRAVELMODE"].ToString();
                TicketClasssID = dt.Rows[rownum]["ADTRAVELMODECLASSID"].ToString();
                TicketClass = dt.Rows[rownum]["TICKETCLASS"].ToString();
                ModeDetail = dt.Rows[rownum]["FLIGHTTRAINNO"].ToString();
                TickingBy = dt.Rows[rownum]["TICKETINGBY"].ToString();
                HotelReserv = dt.Rows[rownum]["HOTELRESERVATION"].ToString();
                PickDrop = dt.Rows[rownum]["PICKUPDROP"].ToString();
                SpecialApp = dt.Rows[rownum]["SPECIALAPPROVAL"].ToString();
                Remarks = dt.Rows[rownum]["REMARKS"].ToString();
                StrIdtype = dt.Rows[rownum]["IDCARDTYPE"].ToString();
                StrIdno = dt.Rows[rownum]["IDCARDNO"].ToString();
                strRequestDetailID = Convert.ToString(dt.Rows[rownum]["ADTOURREQDETAILID"]);
                TourTimeTo = dt.Rows[rownum]["TRAVELTIMETO"].ToString();// Added by Kishan Dodiya
                PreferredLocation = dt.Rows[rownum]["PREFERREDLOCATION"].ToString(); // Added by Kishan Dodiya

                oTourList.Add(new Tour(TourObjective, TourFromDate, TourTime,
                                    FromLocCode, FromLoc, ToLocCode, ToLoc, StayingLocCode, StayingLoc,
                                    ModeID, Mode, TicketClasssID, TicketClass, ModeDetail, TickingBy,
                                    HotelReserv, PickDrop, SpecialApp, Remarks, StrIdtype, StrIdno, strRequestDetailID, "", PreferredLocation, TourTimeTo));
            }
            return (oTourList);
        }
        public ArrayList GetRequestpreDetail(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            ArrayList oTourListpre = new ArrayList();

            string Tourperiod = string.Empty;
            string Advance = string.Empty;
            string Exp = string.Empty;
            string Balance = string.Empty;
            string Subdate = string.Empty;
            string Refundby = string.Empty;
            string Chqno = string.Empty;
            string Chqdate = string.Empty;
            string Refamount = string.Empty;

            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQPREDETAIL_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_REQPREDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            for (int rownum = 0; rownum < dt.Rows.Count; rownum++)
            {

                Tourperiod = dt.Rows[rownum]["TOURPERIOD"].ToString();
                Advance = dt.Rows[rownum]["ADVANCE"].ToString();
                Exp = dt.Rows[rownum]["EXPENSES"].ToString();
                Balance = dt.Rows[rownum]["BALANCE"].ToString();
                Subdate = dt.Rows[rownum]["SUBDATE"].ToString();
                Refundby = dt.Rows[rownum]["REFUNDBY"].ToString();
                Chqno = dt.Rows[rownum]["CHQNO"].ToString();
                Chqdate = dt.Rows[rownum]["CHQDATE"].ToString();
                Refamount = dt.Rows[rownum]["REFAMOUNT"].ToString();


                oTourListpre.Add(new TourAdvance(Tourperiod, Advance, Exp,
                        Balance, Subdate, Refundby, Chqno, Chqdate, Refamount));
            }
            return (oTourListpre);
        }

        /// <summary>
        /// GET DETAIL PART FOR ADMIN APPROVAL
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public ArrayList GetRequestDetailPartForAdminApproval(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            ArrayList oTourList = new ArrayList();
            string TourObjective = string.Empty;
            string TourFromDate = string.Empty;
            string TourTime = string.Empty;
            string FromLoc = string.Empty;
            string ToLoc = string.Empty;
            string StayingLoc = string.Empty;
            string FromLocCode = string.Empty;
            string ToLocCode = string.Empty;
            string StayingLocCode = string.Empty;
            string Mode = string.Empty;
            string ModeID = string.Empty;
            string ModeDetail = string.Empty;
            string TicketClass = string.Empty;
            string TicketClasssID = string.Empty;
            string TickingBy = string.Empty;
            string HotelReserv = string.Empty;
            string PickDrop = string.Empty;
            string RequestDetailID = string.Empty;
            string RequestStatus = string.Empty;
            string RequeststrStatus = string.Empty;
            string UserTicketStatas = string.Empty;
            string StrIdtype = string.Empty;
            string StrIdno = string.Empty;
            string IsGuestHouseBooked = string.Empty;
            string GuestHouseCount = string.Empty;
            //==========Change by aumento on 13012023===================== 
            string GSINNO = string.Empty;
            string TourTimeTo = string.Empty;// Added by Kishan Dodiya
            string PreferredLocation = string.Empty;// Added by Kishan Dodiya
            string FlightDepatureTime = string.Empty;// Added by Kishan Dodiya
                                                     //============================================================
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQDETAILFORADMIN_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_REQDETAILFORADMIN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            for (int rownum = 0; rownum < dt.Rows.Count; rownum++)
            {

                TourObjective = dt.Rows[rownum]["DAYOBJECTIVE"].ToString();
                TourFromDate = dt.Rows[rownum]["TRAVELFROMDATE"].ToString();
                TourTime = dt.Rows[rownum]["TRAVELTIME"].ToString();
                FromLocCode = dt.Rows[rownum]["FROMCITYID"].ToString();
                FromLoc = dt.Rows[rownum]["FRMCITY"].ToString();
                ToLocCode = dt.Rows[rownum]["TOCITYID"].ToString();
                ToLoc = dt.Rows[rownum]["TOCITY"].ToString();
                StayingLocCode = dt.Rows[rownum]["STAYINGCITYID"].ToString();
                StayingLoc = dt.Rows[rownum]["STCITY"].ToString();
                ModeID = dt.Rows[rownum]["ADTRAVELMODEID"].ToString();
                Mode = dt.Rows[rownum]["TRAVELMODE"].ToString();
                TicketClasssID = dt.Rows[rownum]["ADTRAVELMODECLASSID"].ToString();
                TicketClass = dt.Rows[rownum]["TICKETCLASS"].ToString();
                ModeDetail = dt.Rows[rownum]["FLIGHTTRAINNO"].ToString();
                TickingBy = dt.Rows[rownum]["TICKETINGBY"].ToString();
                HotelReserv = dt.Rows[rownum]["HOTELRESERVATION"].ToString();
                PickDrop = dt.Rows[rownum]["PICKUPDROP"].ToString();
                RequestDetailID = dt.Rows[rownum]["ADTOURREQDETAILID"].ToString();
                RequestStatus = dt.Rows[rownum]["STATUS"].ToString();
                RequeststrStatus = dt.Rows[rownum]["BOOKINGSTATUS"].ToString();
                UserTicketStatas = dt.Rows[rownum]["USERTICKETSTATUS"].ToString();
                StrIdtype = dt.Rows[rownum]["IDCARDTYPE"].ToString();
                StrIdno = dt.Rows[rownum]["IDCARDNO"].ToString();
                IsGuestHouseBooked = dt.Rows[rownum]["ISGUESTHOUSEBOOKED"].ToString();
                GuestHouseCount = dt.Rows[rownum]["GuestHouseCount"].ToString();
                //==========Change by aumento on 13012023=====================
                GSINNO = dt.Rows[rownum]["GSTINNO"].ToString();
                TourTimeTo = dt.Rows[rownum]["TRAVELTIMETO"].ToString();// Added by Kishan Dodiya
                PreferredLocation = dt.Rows[rownum]["PREFERREDLOCATION"].ToString();// Added by Kishan Dodiya
                FlightDepatureTime = dt.Rows[rownum]["FLIGHTDEPATURETIME"].ToString();// Added by Kishan Dodiya
                                                                                      //============================================================
                oTourList.Add(new Tour(TourObjective, TourFromDate, TourTime,
                                    FromLocCode, FromLoc, ToLocCode, ToLoc, StayingLocCode, StayingLoc,
                                    ModeID, Mode, TicketClasssID, TicketClass, ModeDetail, TickingBy,
                                    HotelReserv, PickDrop, RequestDetailID, RequestStatus, RequeststrStatus, UserTicketStatas, StrIdtype, StrIdno, IsGuestHouseBooked, GuestHouseCount, GSINNO, PreferredLocation, TourTimeTo, FlightDepatureTime));
            }
            return (oTourList);
        }

        /// <summary>
        /// GET PENDING REQUEST LIST FOR APPROVAL
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public DataTable GetPendingApprovalList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_APPPENDINGLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET PENDING CANCELLATION REQUEST LIST FOR APPROVAL
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public DataTable GetPendingCancelApprovalList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_CANCELLATIONLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("CUR_CANTICKETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// ALL THE APPROVED OR REJECTED REQUEST BY THE EMPLOYEE
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetHistoryApprovalList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_APPHISTORYLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPHISTORYLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET PENDING CANCELLATION REQUEST LIST FOR APPROVAL
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public DataTable GetHistoryCancelApprovalList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_HISCANCELLATIONLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("CUR_HISCANTICKETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET TOUR REQUEST LIST FOR FINANCE APPROVAL
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="EmpCode"></param>
        /// <param name="Status"></param>
        /// <param name="TourStatus"></param>
        /// <returns></returns>
        public DataTable GetListForFinApproval(string strAppAuth, string RequestID, string EmpCode, string Status, string TourStatus, string siteId, string Fromdate, string ToDate
        , string UserType // Added by Kishan Dodiya
        )
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_FINREQUESTLIST_GET";
            oCmd.Parameters.Add("ADMINEMPCODE_IN", OracleDbType.Varchar2).Value = strAppAuth;
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = Status;
            oCmd.Parameters.Add("TOURSTATUS_IN", OracleDbType.Varchar2).Value = TourStatus;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = Fromdate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = ToDate;
            oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = siteId;
            oCmd.Parameters.Add("USERTYPE_IN", OracleDbType.Varchar2).Value = UserType; // Added by Kishan Dodiya
            oCmd.Parameters.Add("CUR_FINLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }


        /// <summary>
        /// GET TOUR REQUEST LIST FOR ADMIN APPROVAL
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="EmpCode"></param>
        /// <param name="Status"></param>
        /// <param name="TourStatus"></param>
        /// <param name="FromDate"></param>
        /// <param name="TillDate"></param>
        /// <returns></returns>
        public DataTable GetListForAdminApproval(string RequestID, string EmpCode, string Status, string TourStatus,
                                                 string FromDate, string TillDate, string plantid
                                                 , string UserType // Added by Kishan Dodiya
                                                 )
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_ADMINREQUESTLIST_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = Status;
            oCmd.Parameters.Add("TOURSTATUS_IN", OracleDbType.Varchar2).Value = TourStatus;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = plantid;
            oCmd.Parameters.Add("USERTYPE_IN", OracleDbType.Varchar2).Value = UserType; // Added by Kishan Dodiya
            oCmd.Parameters.Add("CUR_ADMINLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET TICKET LIST OF ADMIN DEPARTMENT
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="EmpCode"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public DataTable GetTicketList(string RequestID, string EmpCode, string Status)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_ADMINTICKETLIST_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = Status;
            oCmd.Parameters.Add("CUR_TICKETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        //Added by aumento on 30052023 for SR50547==================================================================================================
        /// <summary>
        /// GET TICKET LIST APPROVAL NUMBER WISE
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="EmpCode"></param>
        /// <param name="Status"></param>
        /// <param name="TourStatus"></param>
        /// <param name="FromDate"></param>
        /// <param name="TillDate"></param>
        /// <returns></returns>
        //public DataTable GetTicketListAppNumWise(string RequestID, string EmpCode, string Status, string TourStatus,
        //                                         string FromDate, string TillDate, string plantid)
        public DataTable GetTicketListAppNumWise(string RequestID, string EmpCode, string Status, string TourStatus,
                                                 string FromDate, string TillDate, string plantid, string Type)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TKTLIST_GET";
            ///Added by aumento for SR79308====================================================================
            //oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TKTLIST_GET_NEW";
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TKTLIST_GET_TEMP";
            //=================================================================================================
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = Status;
            oCmd.Parameters.Add("TOURSTATUS_IN", OracleDbType.Varchar2).Value = TourStatus;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = plantid;
            oCmd.Parameters.Add("TYPE_IN", OracleDbType.Varchar2).Value = Type;
            oCmd.Parameters.Add("CUR_TKTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        //=======================================================================================================

        /// <summary>
        /// GET PENDING BOOKING AGAINST REQUEST
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public int GetPendignBooking(string RequestID)
        {
            int PendingBooking;
            //qry="       SELECT  COUNT(DTL.ADTOURREQDETAILID) AS PENDINGREQUEST";
            //qry=qry + " FROM    ADTOURREQDETAIL DTL ";
            //qry=qry + " WHERE   DTL.ADTOURREQUESTID=" + RequestID.ToString();
            //qry=qry + " AND     DTL.ADMINTICKETSTATUS=0";

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_PENDINGBOOKING_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("CUR_PENDINGCOUNT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            _datarow = oDataMgmt.GetDataRow(oCmd);
            PendingBooking = System.Convert.ToInt32(_datarow[0]["PENDINGREQUEST"]);
            return (PendingBooking);
        }

        /// <summary>
        /// GET REQUEST BOOKING DETAILS AGAINST DETAIL ID
        /// </summary>
        ///<param name="RequestDetailID"></param>
        /// <returns></returns>
        public DataTable GetReqeustBookingDetails(string RequestDetailID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQUESTBOOKING_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestDetailID;
            oCmd.Parameters.Add("CUR_REQBOOKING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        //----------------Display GSTINNO and ADDRESS on 21-01-2023 (Aumento)----------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// GET REQUEST BOOKING DETAILS BOOKED BY ADMIN DEPARTMENT
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public DataTable GetReqeustBookingList(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQUESTBOOKINGLIST_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("CUR_REQBOOKINGLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// GET ALL AUTHORITIES EMAIL ID FOR A REQUEST
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public DataTable GetRequestEmails(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQUESTEMAILID_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("CUR_EMAILID", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET ASSOCIATE PREVIOUS ADVANCE STATUS
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetPreviousAdvance(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_PREVADVANCE_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("CUR_PREVADVANCE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);


        }

        /// <summary>
        /// GET TOUR LIST FOR DEPARTMENT HEAD
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="RequestID"></param>
        /// <param name="EmpCode"></param>
        /// <param name="EmpName"></param>
        /// <param name="RequestStatus"></param>
        /// <param name="HardCopyStatus"></param>
        /// <param name="FromDate"></param>
        /// <param name="TillDate"></param>
        /// <param name="AdvanceRequired"></param>
        /// <param name="FilterType"></param>
        /// <returns></returns>
        public DataTable GetDepartmentTourList(string UserID, string RequestID, string EmpCode, string EmpName,
                                                string RequestStatus, string HardCopyStatus, string FromDate,
                                                string TillDate, string AdvanceRequired, string FilterType)
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_DPTREQUESTLIST_GET";
            oCmd.Parameters.Add("CUR_REQDPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("DPTHEADID_IN", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = EmpName;
            oCmd.Parameters.Add("REQSTATUS_IN", OracleDbType.Varchar2).Value = RequestStatus;
            oCmd.Parameters.Add("HARDCOPYSTATUS_IN", OracleDbType.Varchar2).Value = HardCopyStatus;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("ADVANCEREQ_IN", OracleDbType.Varchar2).Value = AdvanceRequired;
            oCmd.Parameters.Add("FILTERTYPE_IN", OracleDbType.Varchar2).Value = FilterType;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET TOUR ALLOWANCE DETAIL (DAILY ALLOW+STAY CHARGES)
        /// </summary>
        /// <param name="DesignationID"></param>
        /// <param name="CityCategoryID"></param>
        /// <param name="AllowanceID"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public DataTable GetAllowanceDetails(string DesignationID, string CityCategoryID, string AllowanceID, string Status)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_ALLOWDETAILS_GET";
            oCmd.Parameters.Add("CUR_ALLOWANCE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = DesignationID;
            oCmd.Parameters.Add("CITYCAT_IN", OracleDbType.Varchar2).Value = CityCategoryID;
            oCmd.Parameters.Add("ALLOWANCEID_IN", OracleDbType.Varchar2).Value = AllowanceID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = Status;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET EMPLOYEE TOUR SCHEDULE MONTH WISE
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <param name="MonthNum"></param>
        /// <param name="YearNum"></param>
        /// <returns></returns>
        public DataTable GetTourSchedule(string EmpCode, string MonthNum, string YearNum)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TOURSCHEDULE_GET";
            oCmd.Parameters.Add("CUR_SCHEDULE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = MonthNum;
            oCmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = YearNum;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET ASSOCIATE UNDER LOGGED EMPLOYEE
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetAssociateList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_ASSOCIATELIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_ASSOCIATELIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET ASSCIATE TOUR MOVEMENT DETAILS
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="RequestID"></param>
        /// <param name="EmpCode"></param>
        /// <param name="EmpName"></param>
        /// <param name="FromDate"></param>
        /// <param name="TillDate"></param>
        /// <returns></returns>
        public DataTable GetAssociatesMovementDetails(string UserID, string RequestID, string EmpCode, string EmpName,
                                                        string FromDate, string TillDate)
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_MOVSCHEDULE_GET";
            oCmd.Parameters.Add("CUR_MOVSCHEDULE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = EmpName;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
            //LOGGED EMPLOYEE ID
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET REQUEST PRINT LIST FOR DEPT COORDINATOR
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public DataTable GetPrintGroupList(string UserID, string GroupID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_PRINTLIST_GET";
            oCmd.Parameters.Add("CUR_REQDPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("DPTHEADID_IN", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("GROUPID_IN", OracleDbType.Varchar2).Value = GroupID;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET DUPLICATE PRINT LIST FOR DEPARTMENT COORDINATOR
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public DataTable GetDuplicatePrintList(string UserID, string FromDate, string ToDate)
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_DUPPRINTLIST_GET";
            oCmd.Parameters.Add("CUR_DUPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = ToDate;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET DEPARTMENT NAME AGAINST DEPRATMENT COORDINATOR
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetDepartmentName(string userID)
        {
            string DepartmentName = string.Empty;
            //qry = "SELECT DPT.ADDEPARTMENTID,DPT.DESCRIP AS DPTNAME FROM ADDEPARTMENT DPT WHERE DPT.DEPARTMENTCOORDINATORID="+userID .ToString ();
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_EMPOFFICIALDETAIL_GET";
            oCmd.Parameters.Add("CUR_EMPOFFDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            DepartmentName = dt.Rows[0]["DEPARTMENT"].ToString();
            return (DepartmentName);
        }

        /// <summary>
        /// GET LAST 3 TOUR REQUEST
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataTable GetPreviousRequestList(string userID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("CUR_REQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// CHECK USER IS SMC MEMBER OR NOT(FOR ROOM BOOKING)
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetEMPType(string userID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            string strEmpType = string.Empty;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_EMPTYPE_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("CUR_EMPTYPE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            if (dt.Rows.Count > 0)
                strEmpType = "SMC";

            return (strEmpType);
        }

        /// <summary>
        /// CHECK USER ALREADY APPLIED FOR THE TOUR DATE OR NOT IF THEN RETURN THE DAY DETAILS
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="TourDate"></param>
        /// <returns></returns>
        public DataTable TourDayDetails(string userID, string TourDate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TOURDAY_DETAILS";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("TOURDATE_IN", OracleDbType.Varchar2).Value = TourDate;
            oCmd.Parameters.Add("CUR_DAYDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET EMPLOYEE NAME
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetEmpName(string userID)
        {

            string EmpName = string.Empty;
            //qry = "SELECT DPT.ADDEPARTMENTID,DPT.DESCRIP AS DPTNAME FROM ADDEPARTMENT DPT WHERE DPT.DEPARTMENTCOORDINATORID="+userID .ToString ();
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_EMPOFFICIALDETAIL_GET";
            oCmd.Parameters.Add("CUR_EMPOFFDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            EmpName = dt.Rows[0]["EMPNAME"].ToString();
            return (EmpName);
        }

        /// <summary>
        /// GET STAYING CITY LIST
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public string GetStayingCity(string RequestID)
        {
            string StayingCity = string.Empty;

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_STAYINGCITY_GET";
            oCmd.Parameters.Add("CUR_STCITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            if (dt.Rows.Count > 0)
                StayingCity = dt.Rows[0]["LOCATION"].ToString();
            return (StayingCity);
        }

        /// <summary>
        /// CHECK ASSOCIATE IS OPERATION COORDINATOR OR NOT
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string isOperationCoordinator(string userID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            string strStatus = string.Empty;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_OPCORDINATOR_VALID";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("CUR_CORDINATOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            if (dt.Rows.Count > 0)
                strStatus = "YES";
            else
                strStatus = "NO";
            return (strStatus);
        }

        /// <summary>
        /// GET ASSOCIATE APPROPRIATE AUTHORITY
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public DataTable GetChangeAppAuth(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_APPAUTHCHANGE_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_ADMINLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// RETURN ASSOCIATE EMAIL ID BASED ON HIS/HER EMPLOYEE CODE
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetAssoiateEmail(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_SEARCHEMPLOYEE.SPROC_AD_OFFICIALDETAIL";
            oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// CHECK ADVANCE TAKEN AGAINST TOUR DATE OR NOT
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="TourDate"></param>
        /// <returns></returns>
        public DataTable TourDayDetailsForAdvance(string userID, string TourDate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_ADVTAKEN_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("TOURDATE_IN", OracleDbType.Varchar2).Value = TourDate;
            oCmd.Parameters.Add("CUR_DAYDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET ASSOCIATE TOUR COORDINATOR
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetTourCoordinator(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_DPTCOORDINATOR_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("CUR_DPTCORD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET MONTHLY TOUR DATA OF ASSOCIATES
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <param name="MonthNum"></param>
        /// <param name="YearNum"></param>
        /// <returns></returns>
        public DataTable GetMonthlyTourReport(string EmpCode, string MonthNum, string YearNum)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_MONTHLYMOVMENT_GET";
            oCmd.Parameters.Add("CUR_TOURDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = MonthNum;
            oCmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = YearNum;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET ASSOCIATE TOUR COORDINATOR
        /// </summary>
        /// <param name="UserID"></param>
        ///  /// <param name="GroupID"></param>
        /// <returns></returns>
        public DataTable GetExcelExportList(string UserID, string GroupID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_EXPORTLIST_GET";
            oCmd.Parameters.Add("CUR_REQDPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("DPTHEADID_IN", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("GROUPID_IN", OracleDbType.Varchar2).Value = GroupID;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        #region GetFinanceTourList
        public DataTable GetFinanceTourList(string UserID, string RequestID, string EmpCode, string EmpName,
                                                   string FromDate, string TillDate, string SiteId, string Bankkey
                                                   , string EmpType  // Added by Kishan Dodiya
                                                   )
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_FINANCEREQUESTLIST_GET";
            oCmd.Parameters.Add("CUR_REQDPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = EmpName;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
            //oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = Planid;
            oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = SiteId;
            oCmd.Parameters.Add("BANKKEY_IN", OracleDbType.Varchar2).Value = Bankkey;
            oCmd.Parameters.Add("EMPTYPE_IN", OracleDbType.Varchar2).Value = EmpType;  // Added by Kishan Dodiya


            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        #endregion

        #region "INSERT/UPDATE QUERIES"
        /// <summary>
        /// INSERT TOUR HEADER AND DETAILS AND AUTHORIZATION
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <param name="MobileNo"></param>
        /// <param name="ExtNo"></param>
        /// <param name="Email"></param>
        /// <param name="Objective"></param>
        /// <param name="AdvRemarks"></param>
        /// <param name="AdvRequired"></param>
        /// <param name="AplusNight"></param>
        /// <param name="ANight"></param>
        /// <param name="BNight"></param>
        /// <param name="CNight"></param>
        /// <param name="StayCharge"></param>
        /// <param name="APlusDay"></param>
        /// <param name="ADay"></param>
        /// <param name="Bday"></param>
        /// <param name="CDay"></param>
        /// <param name="DailyAllow"></param>
        /// <param name="MiscAllow"></param>
        /// <param name="authType"></param>
        /// <param name="AppAuthCode"></param>
        /// <param name="oTourList"></param>
        /// <param name="MiscRemarks"></param>
        /// <param name="RequiredAmount"></param>
        /// <param name="TourStartDate"></param>
        /// <param name="TourEndDate"></param>
        /// <returns></returns>
        public int InsertTourDetail(string EmpCode, string MobileNo, string ExtNo, string Email, string Objective,
                                    string AdvRemarks, int AdvRequired, string AplusNight, string ANight, string BNight, string CNight, double StayCharge,
                                    string APlusDay, string ADay, string Bday, string CDay, double DailyAllow, double MiscAllow, string authType, string AppAuthCode,
                                    ArrayList oTourList, string MiscRemarks, double RequiredAmount, string TourStartDate, string TourEndDate, ArrayList oTourListprv, string Initiator_Status)// 'Initiator_Status' Added by Aumento For SR73072  Save as Draft
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            OracleTransaction objTxn;
            int TxnNumber = 0;
            int rownum;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                objTxn = objCn.BeginTransaction();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.Transaction = objTxn;

                    //INSERT HEADER PORTION
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_ADDREQUEST_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.Parameters.Add("STARTDATE_IN", OracleDbType.Varchar2).Value = TourStartDate;
                    objCmd.Parameters.Add("ENDDATE_IN", OracleDbType.Varchar2).Value = TourEndDate;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
                    objCmd.Parameters.Add("MOBILENO_IN", OracleDbType.Varchar2).Value = MobileNo;
                    objCmd.Parameters.Add("EXTNO_IN", OracleDbType.Varchar2).Value = ExtNo;
                    objCmd.Parameters.Add("EMAIL_IN", OracleDbType.Varchar2).Value = Email;
                    objCmd.Parameters.Add("OBJECTIVE_IN", OracleDbType.Varchar2).Value = Objective;
                    objCmd.Parameters.Add("ADVREMARKS_IN", OracleDbType.Varchar2).Value = AdvRemarks;
                    objCmd.Parameters.Add("ADVREQUIRED_IN", OracleDbType.Int32).Value = AdvRequired;
                    objCmd.Parameters.Add("APLUSNIGHTS_IN", OracleDbType.Varchar2).Value = AplusNight;
                    objCmd.Parameters.Add("ANIGHTS_IN", OracleDbType.Varchar2).Value = ANight;
                    objCmd.Parameters.Add("BNIGHTS_IN", OracleDbType.Varchar2).Value = BNight;
                    objCmd.Parameters.Add("CNIGHTS_IN", OracleDbType.Varchar2).Value = CNight;
                    objCmd.Parameters.Add("STAYCHARGE_IN", OracleDbType.Double).Value = StayCharge;
                    objCmd.Parameters.Add("APLUSDAYS_IN", OracleDbType.Varchar2).Value = APlusDay;
                    objCmd.Parameters.Add("ADAYS_IN", OracleDbType.Varchar2).Value = ADay;
                    objCmd.Parameters.Add("BDAYS_IN", OracleDbType.Varchar2).Value = Bday;
                    objCmd.Parameters.Add("CDAYS_IN", OracleDbType.Varchar2).Value = CDay;
                    objCmd.Parameters.Add("DAILYALLOW_IN", OracleDbType.Double).Value = DailyAllow;
                    objCmd.Parameters.Add("MISCAMOUNT_IN", OracleDbType.Double).Value = MiscAllow;
                    objCmd.Parameters.Add("AUTHTYPE_IN", OracleDbType.Varchar2).Value = authType;
                    objCmd.Parameters.Add("APPEMPCODE_IN", OracleDbType.Varchar2).Value = AppAuthCode;
                    objCmd.Parameters.Add("MISCREMARKS_IN", OracleDbType.Varchar2).Value = MiscRemarks;
                    objCmd.Parameters.Add("REQUIREDMOUNT_IN", OracleDbType.Double).Value = RequiredAmount;
                    objCmd.Parameters.Add("INITIATOR_STATUS_IN", OracleDbType.Varchar2).Value = Initiator_Status; //Added by Aumento For SR73072 Save as Draft
                                                                                                                  //-----------------
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    TxnNumber = System.Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (TxnNumber == 0)
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }

                    //INSERT DETAIL PORTION
                    Tour oTour = new Tour();
                    for (rownum = 0; rownum < oTourList.Count; rownum++)
                    {
                        oTour = (Tour)oTourList[rownum];
                        objCmd.Parameters.Clear();
                        objCmd.CommandText = "PKG_TOURREQUEST.SPROC_ADDREQUESTDETAIL_SET";
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = TxnNumber.ToString();
                        //objCmd.Parameters.Add("TRAVELFROMDATE_IN", OracleDbType.Date).Value = oTour.TourFromDate;
                        objCmd.Parameters.Add("TRAVELFROMDATE_IN", OracleDbType.Date).Value = string.IsNullOrWhiteSpace(oTour.TourFromDate)
                                                                                                ? (object)DBNull.Value
                                                                                                : DateTime.TryParseExact(
                                                                                                      oTour.TourFromDate,
                                                                                                      new[] { "d-MMM-yyyy", "dd-MMM-yyyy" },
                                                                                                      CultureInfo.InvariantCulture,
                                                                                                      DateTimeStyles.None,
                                                                                                      out DateTime parsedDate
                                                                                                  )
                                                                                                  ? parsedDate
                                                                                                  : (object)DBNull.Value;
                        objCmd.Parameters.Add("TRAVELTIME_IN", OracleDbType.Varchar2).Value = oTour.TourTime;
                        objCmd.Parameters.Add("OBJECTIVE_IN", OracleDbType.Varchar2).Value = oTour.TourObjective;
                        //FROM CITY NOT OTHER
                        if (oTour.FromLocCode != "")
                            objCmd.Parameters.Add("FROMCITYID_IN", OracleDbType.Varchar2).Value = oTour.FromLocCode;
                        else
                            objCmd.Parameters.Add("OTHERFROM_IN", OracleDbType.Varchar2).Value = oTour.FromLoc;
                        //TO CITY NOT OTHER
                        if (oTour.ToLocCode != "")
                            objCmd.Parameters.Add("TOCITYID_IN", OracleDbType.Varchar2).Value = oTour.ToLocCode;
                        else
                            objCmd.Parameters.Add("OTHERTO_IN", OracleDbType.Varchar2).Value = oTour.ToLoc;
                        //STAYING NOT OTHER
                        if (oTour.StayingLocCode != "")
                            objCmd.Parameters.Add("STAYCITYID_IN", OracleDbType.Varchar2).Value = oTour.StayingLocCode;
                        else
                            objCmd.Parameters.Add("OTHERSTAY_IN", OracleDbType.Varchar2).Value = oTour.StayingLoc;

                        objCmd.Parameters.Add("MODE_IN", OracleDbType.Varchar2).Value = oTour.ModeID;
                        objCmd.Parameters.Add("FLTIGHT_IN", OracleDbType.Varchar2).Value = oTour.ModeDetail;
                        objCmd.Parameters.Add("MODECLASS_IN", OracleDbType.Varchar2).Value = oTour.TicketClassID;

                        objCmd.Parameters.Add("TICKETINGBY_IN", OracleDbType.Varchar2).Value = oTour.TicketingBy;
                        objCmd.Parameters.Add("HOTELRESRV_IN", OracleDbType.Varchar2).Value = oTour.HotelReserv;
                        objCmd.Parameters.Add("PICKUP_IN", OracleDbType.Varchar2).Value = oTour.PickDrop;
                        objCmd.Parameters.Add("SPECIALAPP_IN", OracleDbType.Varchar2).Value = oTour.SpecialApp;
                        objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = oTour.Remarks;
                        objCmd.Parameters.Add("IDTYPE_IN", OracleDbType.Varchar2).Value = oTour.IDType;
                        objCmd.Parameters.Add("IDNO_IN", OracleDbType.Varchar2).Value = oTour.IDNo;
                        objCmd.Parameters.Add("TRAVELTIMETO_IN", OracleDbType.Varchar2).Value = oTour.TourTimeTo; // Added by Kishan Dodiya
                        objCmd.Parameters.Add("PREFERREDLOCATION_IN", OracleDbType.Varchar2).Value = oTour.PreferredLocation; // Added by Kishan Dodiya

                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                        objCmd.ExecuteNonQuery();

                        //IF ERROR OCCURED
                        if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                        {
                            ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                            throw new Exception(ErrorMessage);
                        }
                    }
                    //INSERT PREVIOUS ADVANCE DETAIL PORTION

                    TourAdvance oPreTour = new TourAdvance();
                    for (rownum = 0; rownum < oTourListprv.Count; rownum++)
                    {
                        oPreTour = (TourAdvance)oTourListprv[rownum];
                        objCmd.Parameters.Clear();
                        objCmd.CommandText = "PKG_TOURREQUEST.SPROC_ADDPRETOURDTL_SET";
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = TxnNumber.ToString();
                        objCmd.Parameters.Add("TOURPERIOD_IN", OracleDbType.Varchar2).Value = oPreTour.Tourperiod;
                        objCmd.Parameters.Add("ADVANCE_IN", OracleDbType.Varchar2).Value = oPreTour.Advance;
                        objCmd.Parameters.Add("EXPENSES_IN", OracleDbType.Varchar2).Value = oPreTour.Exp;
                        objCmd.Parameters.Add("BALANCE_IN", OracleDbType.Varchar2).Value = oPreTour.Balance;
                        objCmd.Parameters.Add("SUBMITDATE_IN", OracleDbType.Varchar2).Value = oPreTour.Subdate;
                        objCmd.Parameters.Add("REFUNDBY_IN", OracleDbType.Varchar2).Value = oPreTour.Refundby;
                        objCmd.Parameters.Add("CHQNO_IN", OracleDbType.Varchar2).Value = oPreTour.Chqno;
                        objCmd.Parameters.Add("CHQDATE_IN", OracleDbType.Varchar2).Value = oPreTour.Chqdate;
                        objCmd.Parameters.Add("REFUNDAMT_IN", OracleDbType.Varchar2).Value = oPreTour.Refamount;
                        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;

                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                        objCmd.ExecuteNonQuery();

                        //IF ERROR OCCURED
                        if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                        {
                            ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                            throw new Exception(ErrorMessage);
                        }
                    }
                    //COMMIT TRANSACTION
                    objTxn.Commit();
                }
                catch (Exception ex)
                {
                    //ROLLBACK TRANSACTION
                    objTxn.Rollback();
                    ErrorMessage = ex.Message;

                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return (TxnNumber);
            }
        }

        /// <summary>
        /// CANCEL TOUR REQUEST
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="Remarks"></param>
        /// <returns></returns>
        public string CancelTourRequest(string RequestID, string Remarks)
        {
            // ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_CANECLLREQUEST_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        /// <summary>
        /// MODIFY TOUR REQUEST BEFORE APPROVAL
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="EmpCode"></param>
        /// <param name="MobileNo"></param>
        /// <param name="ExtNo"></param>
        /// <param name="Email"></param>
        /// <param name="Objective"></param>
        /// <param name="AdvRemarks"></param>
        /// <param name="AdvRequired"></param>
        /// <param name="AplusNight"></param>
        /// <param name="ANight"></param>
        /// <param name="BNight"></param>
        /// <param name="CNight"></param>
        /// <param name="StayCharge"></param>
        /// <param name="APlusDay"></param>
        /// <param name="ADay"></param>
        /// <param name="Bday"></param>
        /// <param name="CDay"></param>
        /// <param name="DailyAllow"></param>
        /// <param name="MiscAllow"></param>
        /// <param name="oTourList"></param>
        /// <param name="MiscRemarks"></param>
        /// <param name="RequiredAmount"></param>
        /// <returns></returns>
        public void UpdateTourDetail(string RequestID, string EmpCode, string MobileNo, string ExtNo, string Email, string Objective,
                                    string AdvRemarks, int AdvRequired, string AplusNight, string ANight, string BNight, string CNight, double StayCharge,
                                    string APlusDay, string ADay, string Bday, string CDay, double DailyAllow, double MiscAllow, ArrayList oTourList,
                                    string MiscRemarks, double RequiredAmount, ArrayList oTourListprv, string Initiator_Status)// 'Initiator_Status' Added by Aumento For SR73072 for Save as Draft
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            OracleTransaction objTxn;
            int rownum;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                objTxn = objCn.BeginTransaction();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.Transaction = objTxn;

                    //UPDATE HEADER PORTION
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATEREQUEST_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
                    objCmd.Parameters.Add("MOBILENO_IN", OracleDbType.Varchar2).Value = MobileNo;
                    objCmd.Parameters.Add("EXTNO_IN", OracleDbType.Varchar2).Value = ExtNo;
                    objCmd.Parameters.Add("EMAIL_IN", OracleDbType.Varchar2).Value = Email;
                    objCmd.Parameters.Add("OBJECTIVE_IN", OracleDbType.Varchar2).Value = Objective;
                    objCmd.Parameters.Add("ADVREMARKS_IN", OracleDbType.Varchar2).Value = AdvRemarks;
                    objCmd.Parameters.Add("ADVREQUIRED_IN", OracleDbType.Int32).Value = AdvRequired;
                    objCmd.Parameters.Add("APLUSNIGHTS_IN", OracleDbType.Varchar2).Value = AplusNight;
                    objCmd.Parameters.Add("ANIGHTS_IN", OracleDbType.Varchar2).Value = ANight;
                    objCmd.Parameters.Add("BNIGHTS_IN", OracleDbType.Varchar2).Value = BNight;
                    objCmd.Parameters.Add("CNIGHTS_IN", OracleDbType.Varchar2).Value = CNight;
                    objCmd.Parameters.Add("STAYCHARGE_IN", OracleDbType.Double).Value = StayCharge;
                    objCmd.Parameters.Add("APLUSDAYS_IN", OracleDbType.Varchar2).Value = APlusDay;
                    objCmd.Parameters.Add("ADAYS_IN", OracleDbType.Varchar2).Value = ADay;
                    objCmd.Parameters.Add("BDAYS_IN", OracleDbType.Varchar2).Value = Bday;
                    objCmd.Parameters.Add("CDAYS_IN", OracleDbType.Varchar2).Value = CDay;
                    objCmd.Parameters.Add("DAILYALLOW_IN", OracleDbType.Double).Value = DailyAllow;
                    objCmd.Parameters.Add("MISCAMOUNT_IN", OracleDbType.Double).Value = MiscAllow;
                    objCmd.Parameters.Add("MISCREMARKS_IN", OracleDbType.Varchar2).Value = MiscRemarks;
                    objCmd.Parameters.Add("REQUIREDAMOUNT_IN", OracleDbType.Double).Value = RequiredAmount;
                    objCmd.Parameters.Add("INITIATOR_STATUS_IN", OracleDbType.Varchar2).Value = Initiator_Status; //Added by Aumento For SR73072 Save as Draft

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    int errNum;
                    errNum = System.Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (errNum == 0)
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }




                    //DELETE DETAIL PORTION
                    string qry = string.Empty;
                    objCmd.Parameters.Clear();
                    qry = " DELETE FROM ADTOURREQDETAIL  DTL WHERE DTL.ADTOURREQUESTID=" + RequestID.ToString();
                    objCmd.CommandText = qry;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.ExecuteNonQuery();

                    //DELETE PREVIOUS DETAIL PORTION
                    string preqry = string.Empty;
                    objCmd.Parameters.Clear();
                    preqry = " DELETE FROM ADPRETOURDTL  DTL WHERE DTL.ADTOURREQUESTID=" + RequestID.ToString();
                    objCmd.CommandText = preqry;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.ExecuteNonQuery();



                    //REINSERT DETAIL PORTION
                    Tour oTour = new Tour();
                    for (rownum = 0; rownum < oTourList.Count; rownum++)
                    {
                        oTour = (Tour)oTourList[rownum];
                        objCmd.Parameters.Clear();
                        objCmd.CommandText = "PKG_TOURREQUEST.SPROC_ADDREQUESTDETAIL_SET";
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                        //    objCmd.Parameters.Add("TRAVELFROMDATE_IN", OracleDbType.Date).Value = oTour.TourFromDate;
                        objCmd.Parameters.Add("TRAVELFROMDATE_IN", OracleDbType.Date).Value = 
                            DateTime.ParseExact(oTour.TourFromDate,"dd-MMM-yyyy",CultureInfo.InvariantCulture, DateTimeStyles.None);
                        objCmd.Parameters.Add("TRAVELTIME_IN", OracleDbType.Varchar2).Value = oTour.TourTime;
                        objCmd.Parameters.Add("OBJECTIVE_IN", OracleDbType.Varchar2).Value = oTour.TourObjective;
                        //FROM CITY NOT OTHER
                        if (oTour.FromLocCode != "")
                            objCmd.Parameters.Add("FROMCITYID_IN", OracleDbType.Varchar2).Value = oTour.FromLocCode;
                        else
                            objCmd.Parameters.Add("OTHERFROM_IN", OracleDbType.Varchar2).Value = oTour.FromLoc;
                        //TO CITY NOT OTHER
                        if (oTour.ToLocCode != "")
                            objCmd.Parameters.Add("TOCITYID_IN", OracleDbType.Varchar2).Value = oTour.ToLocCode;
                        else
                            objCmd.Parameters.Add("OTHERTO_IN", OracleDbType.Varchar2).Value = oTour.ToLoc;
                        //STAYING NOT OTHER
                        if (oTour.StayingLocCode != "")
                            objCmd.Parameters.Add("STAYCITYID_IN", OracleDbType.Varchar2).Value = oTour.StayingLocCode;
                        else
                            objCmd.Parameters.Add("OTHERSTAY_IN", OracleDbType.Varchar2).Value = oTour.StayingLoc;

                        objCmd.Parameters.Add("MODE_IN", OracleDbType.Varchar2).Value = oTour.ModeID;
                        objCmd.Parameters.Add("FLTIGHT_IN", OracleDbType.Varchar2).Value = oTour.ModeDetail;
                        objCmd.Parameters.Add("MODECLASS_IN", OracleDbType.Varchar2).Value = oTour.TicketClassID;

                        objCmd.Parameters.Add("TICKETINGBY_IN", OracleDbType.Varchar2).Value = oTour.TicketingBy;
                        objCmd.Parameters.Add("HOTELRESRV_IN", OracleDbType.Varchar2).Value = oTour.HotelReserv;
                        objCmd.Parameters.Add("PICKUP_IN", OracleDbType.Varchar2).Value = oTour.PickDrop;
                        objCmd.Parameters.Add("SPECIALAPP_IN", OracleDbType.Varchar2).Value = oTour.SpecialApp;
                        objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = oTour.Remarks;
                        objCmd.Parameters.Add("IDTYPE_IN", OracleDbType.Varchar2).Value = oTour.IDType;
                        objCmd.Parameters.Add("IDNO_IN", OracleDbType.Varchar2).Value = oTour.IDNo;
                        objCmd.Parameters.Add("TRAVELTIMETO_IN", OracleDbType.Varchar2).Value = oTour.TourTimeTo; // Added by Kishan Dodiya
                        objCmd.Parameters.Add("PREFERREDLOCATION_IN", OracleDbType.Varchar2).Value = oTour.PreferredLocation; // Added by Kishan Dodiya


                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                        objCmd.ExecuteNonQuery();

                        //IF ERROR OCCURED
                        if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                        {
                            ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                            throw new Exception(ErrorMessage);
                        }
                    }
                    //INSERT PREVIOUS ADVANCE DETAIL PORTION

                    TourAdvance oPreTour = new TourAdvance();
                    for (rownum = 0; rownum < oTourListprv.Count; rownum++)
                    {
                        oPreTour = (TourAdvance)oTourListprv[rownum];
                        objCmd.Parameters.Clear();
                        objCmd.CommandText = "PKG_TOURREQUEST.SPROC_ADDPRETOURDTL_SET";
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                        objCmd.Parameters.Add("TOURPERIOD_IN", OracleDbType.Varchar2).Value = oPreTour.Tourperiod;
                        objCmd.Parameters.Add("ADVANCE_IN", OracleDbType.Varchar2).Value = oPreTour.Advance;
                        objCmd.Parameters.Add("EXPENSES_IN", OracleDbType.Varchar2).Value = oPreTour.Exp;
                        objCmd.Parameters.Add("BALANCE_IN", OracleDbType.Varchar2).Value = oPreTour.Balance;
                        objCmd.Parameters.Add("SUBMITDATE_IN", OracleDbType.Varchar2).Value = oPreTour.Subdate;
                        objCmd.Parameters.Add("REFUNDBY_IN", OracleDbType.Varchar2).Value = oPreTour.Refundby;
                        objCmd.Parameters.Add("CHQNO_IN", OracleDbType.Varchar2).Value = oPreTour.Chqno;
                        objCmd.Parameters.Add("CHQDATE_IN", OracleDbType.Varchar2).Value = oPreTour.Chqdate;
                        objCmd.Parameters.Add("REFUNDAMT_IN", OracleDbType.Varchar2).Value = oPreTour.Refamount;
                        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;

                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                        objCmd.ExecuteNonQuery();

                        //IF ERROR OCCURED
                        if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                        {
                            ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                            throw new Exception(ErrorMessage);
                        }

                    }
                    //COMMIT TRANSACTION
                    objTxn.Commit();
                }
                catch (Exception ex)
                {
                    //ROLLBACK TRANSACTION
                    objTxn.Rollback();
                    ErrorMessage = ex.Message;

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


        /// <summary>
        /// UPDATE TOUR APPROVAL STATUS
        /// </summary>
        /// <param name="AppType"></param>
        /// <param name="RequestID"></param>
        /// <param name="status"></param>
        /// <param name="Remarks"></param>
        /// <param name="AppAuthCode"></param>
        /// <param name="spAppAuthCode"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        /// /*Added by TTL on 04-Oct-2025 against SR110729 > CR7297 | new parameter added*/
        public string UpdateApprovalStatus(string AppType, string RequestID, string status, string Remarks,
                                            string AppAuthCode, string spAppAuthCode, string UserID, string Director2 = "")
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATEAPPROVAL_SET";
                    objCmd.Parameters.Add("APPTYPE_IN", OracleDbType.Varchar2).Value = AppType;
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("APPAUTHCODE_IN", OracleDbType.Varchar2).Value = AppAuthCode;
                    objCmd.Parameters.Add("SPAPPAUTHCODE_IN", OracleDbType.Varchar2).Value = spAppAuthCode;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    /*Added by TTL on 04-Oct-2025 against SR110729 > CR7297 Start*/
                    objCmd.Parameters.Add("D2APPAUTHCODE_IN", OracleDbType.Varchar2).Value = Director2;
                    /*Added by TTL on 04-Oct-2025 against SR110729 > CR7297 End*/
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        /// <summary>
        /// UPDATE FINANCE DEPARTMENT APPROVAL STATUS
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="status"></param>
        /// <param name="Remarks"></param>
        /// <param name="HardCopyStatus"></param>
        /// <param name="TranferAmount"></param>
        /// <param name="AppAuthCode"></param>
        /// <returns></returns>
        public string UpdateFinanceApprovalStatus(string RequestID, string status, string Remarks, string HardCopyStatus,
                                                   string TranferAmount, string AppAuthCode)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATEFINAPPROVAL_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("HARDCOPYSTATUS_IN", OracleDbType.Varchar2).Value = HardCopyStatus;
                    objCmd.Parameters.Add("TRANSFERAMOUNT_IN", OracleDbType.Varchar2).Value = TranferAmount;
                    objCmd.Parameters.Add("APPAUTHCODE_IN", OracleDbType.Varchar2).Value = AppAuthCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        /// <summary>
        /// UPDATE EACH BOOKING STATUS
        /// </summary>
        /// <param name="RequestDetailID"></param>
        /// <param name="TicketNumber"></param>
        ///<param name="PNRNumber"></param>
        /// <param name="TicketAmount"></param>
        /// <param name="HotelName"></param>
        /// <param name="HotelAddress"></param>
        /// <param name="CheckinDate"></param>
        /// <param name="CheckinTime"></param>
        /// <param name="CheckoutDate"></param>
        /// <param name="CheckoutTime"></param>
        /// <param name="PickDropDetails"></param>
        /// <param name="TicketStatus"></param>
        /// <param name="TicketIssueDate"></param>
        /// <param name="TicketRemarks">></param>
        /// <param name="CancellationAuthCode"></param>
        /// <param name="TicketType"></param>
        /// <returns></returns>

        //Below change TicketfileName to INVOICEAMOUNT_IN addded by aumento for the SR50547 on 22052023==============================================================================================================================
        public string UpdateTicketBooking(string RequestDetailID, string TicketNumber, string PNRNumber, string TicketAmount, string TicketCancelNumber,
                                            string HotelName, string HotelAddress, string CheckinDate,
                                            string CheckinTime, string CheckoutDate, string CheckoutTime,
                                            string PickDropDetails, string TicketStatus, string TicketIssueDate,
                                            string TicketRemarks, string CancellationAuthCode, string TicketType, string comparisionSheetfileName, string tktcancellationdate, string TicketfileName, string TicketfileName2,
                                            string AirlineName, string PrefixCode, string InvoiceNo, string InvoiceDate, string INVOICEAMOUNT, string GSTAmount, string ImpInfo, string InvTotalAmt, string FlightDepartureTime, string FlightDepartureDate)
        //=========================================================================================================================================================================================================================
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATETKTBOOKING_SET";
                    objCmd.Parameters.Add("REQUESTDETAILID_IN", OracleDbType.Varchar2).Value = RequestDetailID;
                    objCmd.Parameters.Add("TICKETNUMBER_IN", OracleDbType.Varchar2).Value = TicketNumber;
                    objCmd.Parameters.Add("PNRNUMBER_IN", OracleDbType.Varchar2).Value = PNRNumber;
                    objCmd.Parameters.Add("TICKETAMOUNT_IN", OracleDbType.Varchar2).Value = TicketAmount;
                    objCmd.Parameters.Add("TICKETCANCELAMOUNT_IN", OracleDbType.Varchar2).Value = TicketCancelNumber;
                    objCmd.Parameters.Add("ISSUEDATE_IN", OracleDbType.Varchar2).Value = TicketIssueDate;
                    objCmd.Parameters.Add("HOTELNAME_IN", OracleDbType.Varchar2).Value = HotelName;
                    objCmd.Parameters.Add("HOTELADDRESS_IN", OracleDbType.Varchar2).Value = HotelAddress;
                    objCmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = comparisionSheetfileName;
                    //IF HOTEL NOT BOOKED THEN NO CHECKIN/CHECKOUT IS REQUIRED
                    if (HotelName.Trim() != "")
                    {
                        objCmd.Parameters.Add("CHECKINDATE_IN", OracleDbType.Varchar2).Value = CheckinDate;
                        objCmd.Parameters.Add("CHECKINTIME_IN", OracleDbType.Varchar2).Value = CheckinTime;
                        objCmd.Parameters.Add("CHECKOUTDATE_IN", OracleDbType.Varchar2).Value = CheckoutDate;
                        objCmd.Parameters.Add("CHECKOUTTIME_IN", OracleDbType.Varchar2).Value = CheckoutTime;
                    }
                    objCmd.Parameters.Add("PICKDROP_IN", OracleDbType.Varchar2).Value = PickDropDetails;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = TicketStatus;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = TicketRemarks;
                    objCmd.Parameters.Add("CANADEMPCODE_IN", OracleDbType.Varchar2).Value = CancellationAuthCode;
                    objCmd.Parameters.Add("TICKETTYPE_IN", OracleDbType.Varchar2).Value = TicketType;
                    objCmd.Parameters.Add("TKTCANCELDATE_IN", OracleDbType.Varchar2).Value = tktcancellationdate;
                    objCmd.Parameters.Add("TICKETFILENAME_IN", OracleDbType.Varchar2).Value = TicketfileName;
                    objCmd.Parameters.Add("TICKETFILENAME2_IN", OracleDbType.Varchar2).Value = TicketfileName2;

                    //Below change TicketfileName to ExcessBaggage addded by aumento for the SR50547 on 22052023===================================================================

                    objCmd.Parameters.Add("AIRLINENAME_IN", OracleDbType.Varchar2).Value = AirlineName;
                    objCmd.Parameters.Add("PREFIXCODE_IN", OracleDbType.Varchar2).Value = PrefixCode;
                    objCmd.Parameters.Add("INVOICENO_IN", OracleDbType.Varchar2).Value = InvoiceNo;
                    objCmd.Parameters.Add("INVOICEDATE_IN", OracleDbType.Varchar2).Value = InvoiceDate;
                    objCmd.Parameters.Add("INVOICEAMOUNT_IN", OracleDbType.Varchar2).Value = INVOICEAMOUNT;
                    objCmd.Parameters.Add("GSTAMOUNT_IN", OracleDbType.Varchar2).Value = GSTAmount;
                    objCmd.Parameters.Add("IMPINFO_IN", OracleDbType.Varchar2).Value = ImpInfo;
                    objCmd.Parameters.Add("INVTOTALAMT_IN", OracleDbType.Varchar2).Value = InvTotalAmt;
                    objCmd.Parameters.Add("FLIGHTDEPARTURETIME_IN", OracleDbType.Varchar2).Value = FlightDepartureTime;
                    objCmd.Parameters.Add("FLIGHTDEPARTUREDATE_IN", OracleDbType.Varchar2).Value = FlightDepartureDate;
                    //==============================================================================================================================================================

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        /// <summary>
        /// UPDATE ADMIN APPROVAL STATUS
        /// </summary>
        /// <param name="RequestID"></param>
        /// <param name="status"></param>
        /// <param name="Remarks"></param>
        /// <param name="AppAuthCode"></param>
        /// <returns></returns>
        public string UpdateAdminApprovalStatus(string RequestID, string status, string Remarks, string AppAuthCode)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATEADMINAPPROVAL_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("APPAUTHCODE_IN", OracleDbType.Varchar2).Value = AppAuthCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        /// <summary>
        /// CANCEL TICKET REQUEST
        /// </summary>
        /// <param name="RequestDetailID"></param>
        /// <param name="Remarks"></param>
        /// <returns></returns>
        public string CancelTicketRequest(string RequestDetailID, string Remarks)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_CANCELTOURREQUEST_SET";
                    objCmd.Parameters.Add("REQUESTDETAILID_IN", OracleDbType.Varchar2).Value = RequestDetailID;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        /// <summary>
        /// APPROVE TICKECT CANCELLATION REQUEST BY APPROVAL AUTHORITY
        /// </summary>
        /// <param name="RequestDetailID"></param>
        /// <param name="Remarks"></param>
        /// <returns></returns>
        public string ApproveCanellationRequest(string RequestDetailID, string Remarks)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string qry = string.Empty;
            string errMsg = string.Empty;

            //qry = "         UPDATE ADTOURREQDETAIL DTL";
            //qry = qry + "   SET    DTL.CANCELLATIONAUTHREMARKS='" + Remarks + "',";
            //qry = qry + "          DTL.CANCELLATIONAUTHSTATUS=1,";//APPROVE CANCELLATION REQUEST
            //qry = qry + "          DTL.CANCELLATIONAPPDATE=SYSDATE";//SYSTEM DATE
            //qry = qry + "   WHERE  DTL.ADTOURREQDETAILID=" + RequestDetailID.ToString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_APPROVECANCELREQUEST_SET";
                    objCmd.Parameters.Add("REQUESTDETAILID_IN", OracleDbType.Varchar2).Value = RequestDetailID;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        /// <summary>
        /// UPDATE ALLOWANCE DETAILS
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="AllowanceID"></param>
        /// <param name="DailyAllowance"></param>
        /// <param name="StayCharge"></param>
        /// <param name="FromDate"></param>
        /// <param name="TillDate"></param>
        /// <param name="ActiveStatus"></param>
        /// <param name="Designation"></param>
        /// <param name="CityCategory"></param>
        /// <returns></returns>
        public string UpdateAllowanceDetail(string UserID, string AllowanceID, string DailyAllowance, string StayCharge,
                                            string FromDate, string TillDate, string ActiveStatus, string Designation,
                                            string CityCategory, string strNTA, string strHA, string strLA
                                            , string Usertype // Added by Kishan Dodiya
                                            )
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_TOURREQUEST.SPROC_TOURALLOWANCE_SET";
            objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = UserID;
            objCmd.Parameters.Add("ALLOWANCEID_IN", OracleDbType.Varchar2).Value = AllowanceID;
            objCmd.Parameters.Add("ALLOWANCE_IN", OracleDbType.Varchar2).Value = DailyAllowance;
            objCmd.Parameters.Add("STAYCHARGE_IN", OracleDbType.Varchar2).Value = StayCharge;
            objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            objCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = ActiveStatus;
            objCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = Designation;
            objCmd.Parameters.Add("CITYCATEGORYID_IN", OracleDbType.Varchar2).Value = CityCategory;
            objCmd.Parameters.Add("NTA_IN", OracleDbType.Varchar2).Value = strNTA;
            objCmd.Parameters.Add("HA_IN", OracleDbType.Varchar2).Value = strHA;
            objCmd.Parameters.Add("LA_IN", OracleDbType.Varchar2).Value = strLA;
            objCmd.Parameters.Add("USERTYPE_IN", OracleDbType.Varchar2).Value = Usertype; // Added by Kishan Dodiya
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            //SR79472 Start
            objCmd.Parameters.Add("OUT_TOURID", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("OUT_TRANSACTIONTYPE", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            //SR79472 End
            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);
            string strTourAllowaceID = objCmd.Parameters["OUT_TOURID"].Value.ToString();
            string StrTransactionType = objCmd.Parameters["OUT_TRANSACTIONTYPE"].Value.ToString();
            UpdateAllowanceLog(UserID, strTourAllowaceID, DailyAllowance, StayCharge, FromDate, TillDate, ActiveStatus, Designation, CityCategory, strNTA, strHA, strLA, StrTransactionType); //Updated By Deloitte SR72269 

            //IF ERROR OCCURED
            if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
            {
                errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);

            }
            return (errMsg);
        }

        //Updated By Deloitte SR72269
        public void UpdateAllowanceLog(string UserID, string AllowanceID, string DailyAllowance, string StayCharge,
                                           string FromDate, string TillDate, string ActiveStatus, string Designation,
                                           string CityCategory, string strNTA, string strHA, string strLA, string strTransactionType)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "TOURALLOWANCE_LOG";
            objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = UserID;
            objCmd.Parameters.Add("ALLOWANCEID_IN", OracleDbType.Varchar2).Value = AllowanceID;
            objCmd.Parameters.Add("ALLOWANCE_IN", OracleDbType.Varchar2).Value = DailyAllowance;
            objCmd.Parameters.Add("STAYCHARGE_IN", OracleDbType.Varchar2).Value = StayCharge;
            objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            objCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = ActiveStatus;
            objCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = Designation;
            objCmd.Parameters.Add("CITYCATEGORYID_IN", OracleDbType.Varchar2).Value = CityCategory;
            objCmd.Parameters.Add("NTA_IN", OracleDbType.Varchar2).Value = strNTA;
            objCmd.Parameters.Add("HA_IN", OracleDbType.Varchar2).Value = strHA;
            objCmd.Parameters.Add("LA_IN", OracleDbType.Varchar2).Value = strLA;
            objCmd.Parameters.Add("USERTYPE_IN", OracleDbType.Varchar2).Value = UserID;
            objCmd.Parameters.Add("TRANSACTIONTYPE_IN", OracleDbType.Varchar2).Value = strTransactionType;
            oDataMgmt.ExecuteQuery(objCmd);
        }
        //--//Updated By Deloitte SR72269


        /// <summary>
        /// UPDATE PRINTED REQUESTED GROUP NUMBER AND PRINT DATE
        /// </summary>
        /// <param name="RequestList"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int UpdatePrintStatus(string RequestList, string UserID)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            //string qry = string.Empty;
            int GroupID; //= GetPrintGroupNumber();
                         //string[] arr=RequestList .Split (',');

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_REQUESTPRINTSTATUS_SET";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("REQUESTLIST_IN", OracleDbType.Varchar2).Value = RequestList;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    GroupID = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());

                    if (GroupID == 0)
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                    GroupID = 0;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (GroupID);
        }

        /// <summary>
        /// Update Approval Authority By Dept Coordinator
        /// </summary>
        /// <param name="OldRecAuth"></param>
        /// <param name="RecAuth"></param>
        /// <param name="RequestID"></param>
        /// <param name="OldAppAuth"></param>
        /// <param name="AppAuth"></param>
        /// <param name="OldSPAppAuth"></param>
        /// <param name="SpAppAuth"></param>
        /// <param name="strModBy"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        public string UpdateApprovalAuthority(string OldRecAuth, string RecAuth, string RequestID, string OldAppAuth,
                                                string AppAuth, string OldSPAppAuth, string SpAppAuth, string strModBy, string strRemarks)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATEAPPAUTHORITY_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("OLDRECAUTH_IN", OracleDbType.Varchar2).Value = OldRecAuth;
                    objCmd.Parameters.Add("MODRECAUTH_IN", OracleDbType.Varchar2).Value = RecAuth;
                    objCmd.Parameters.Add("OLDAPPAUTH_IN", OracleDbType.Varchar2).Value = OldAppAuth;
                    objCmd.Parameters.Add("MODAPPAUTH_IN", OracleDbType.Varchar2).Value = AppAuth;
                    objCmd.Parameters.Add("OLDSPAPPAUTH_IN", OracleDbType.Varchar2).Value = OldSPAppAuth;
                    objCmd.Parameters.Add("MODSPAPPAUTH_IN", OracleDbType.Varchar2).Value = SpAppAuth;
                    objCmd.Parameters.Add("MODBY_IN", OracleDbType.Varchar2).Value = strModBy;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        //ADD/EDIT TOUR APPORVAL AUTHORITY
        public string ApprovalAuthority_Set(string strEmpcode, string strAppEmpCode, string strActive, string strAdddedBy, string strAppId)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_APPROVALAUTHORITY_SET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                    objCmd.Parameters.Add("APPEMPCODE_IN", OracleDbType.Varchar2).Value = strAppEmpCode;
                    objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strActive;
                    objCmd.Parameters.Add("ADDEDDBY_IN", OracleDbType.Varchar2).Value = strAdddedBy;
                    objCmd.Parameters.Add("ADTOURAPPID_IN", OracleDbType.Varchar2).Value = strAppId;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    errMsg = objCmd.Parameters["RESULT_OUT"].Value.ToString();
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        //GET APPROVAL AUTHORITY AND RELATED EMPCODE
        public DataTable ApprovalAuth_Get(string strEmpCode, string strEmpName, string strOperation, string strDivision)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_APPROVALAUTHORITY_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperation;
            oCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivision;

            oCmd.Parameters.Add("CUR_REQDPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }


        public string UpdateGuestBooking(string TourRequestDetailID, string Location, string Address, string Occupency,
                                            string RoomNo, string PickDropDetails, string Remarks, string CheckinDate,
                                            string CheckoutDate, string ByUserID, string Availablity, string EmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_TOURGHBOOKINGTRN_INSERT";
                    objCmd.Parameters.Add("TOURREQDTLID_IN", OracleDbType.Varchar2).Value = TourRequestDetailID;
                    //objCmd.Parameters.Add("LOCATION_IN", OracleDbType.Varchar2).Value = Location;
                    //objCmd.Parameters.Add("ADDRESS_IN", OracleDbType.Varchar2).Value = Address;
                    objCmd.Parameters.Add("CHECKINDATE_IN", OracleDbType.Varchar2).Value = CheckinDate;
                    objCmd.Parameters.Add("CHECKOUTDATE_IN", OracleDbType.Varchar2).Value = CheckoutDate;
                    objCmd.Parameters.Add("NOPFPERSON_IN", OracleDbType.Varchar2).Value = Occupency;
                    objCmd.Parameters.Add("PROCESSTATUS_IN", OracleDbType.Varchar2).Value = "1";
                    objCmd.Parameters.Add("GHRMSTID_IN", OracleDbType.Varchar2).Value = RoomNo;
                    //objCmd.Parameters.Add("PICKDROP_IN", OracleDbType.Varchar2).Value = PickDropDetails;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ByUserID;
                    objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = EmpCode;
                    objCmd.Parameters.Add("AVAILABILITY_IN", OracleDbType.Varchar2).Value = Availablity;

                    //objCmd.Parameters.Add("CARETAKERNAME_IN", OracleDbType.Varchar2).Value = caretakername;
                    //objCmd.Parameters.Add("CARETAKERNO_IN", OracleDbType.Varchar2).Value = caretakerno;
                    //                                                   
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        #endregion


        public DataTable GetGuestHouseBookingDetails(string RequestDetailID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_GUESTHSSTAYLOCATION_GET";
            oCmd.Parameters.Add("REQUESTDTLID_IN", OracleDbType.Varchar2).Value = RequestDetailID;
            oCmd.Parameters.Add("CUR_REQBOOKING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }


        public string UpdateHotelBooking(string RequestDetailID,
                                            string HotelName, string HotelAddress, double fare, string CheckinDate,
                                            string CheckinTime, string CheckoutDate, string CheckoutTime,
                                            string PickDropDetails, string Remarks, string HotelName1, string HotelName2, string HotelAvailable,
                                            string GSTNumber, string StayingCityID, string HotelID, string EmpCode, string HotelID2, string HotelID3,
                                            string Hotel_Cost, string GSTPer, string TotalAmount, string HotelCategory, string HotelBookedBy, string Hotel1GST, string Hotel2GST)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATEHOTELBOOKING_SET";
                    objCmd.Parameters.Add("REQUESTDETAILID_IN", OracleDbType.Varchar2).Value = RequestDetailID;
                    objCmd.Parameters.Add("HOTELNAME_IN", OracleDbType.Varchar2).Value = HotelName;
                    objCmd.Parameters.Add("HOTELADDRESS_IN", OracleDbType.Varchar2).Value = HotelAddress;
                    objCmd.Parameters.Add("HOTELFARE_IN", OracleDbType.Double).Value = fare;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("HOTELNAME1_IN", OracleDbType.Varchar2).Value = HotelName1;
                    objCmd.Parameters.Add("HOTELNAME2_IN", OracleDbType.Varchar2).Value = HotelName2;
                    objCmd.Parameters.Add("HOTELAVAILABLE_IN", OracleDbType.Varchar2).Value = HotelAvailable;

                    //IF HOTEL NOT BOOKED THEN NO CHECKIN/CHECKOUT IS REQUIRED
                    if (HotelName.Trim() != "")
                    {
                        objCmd.Parameters.Add("CHECKINDATE_IN", OracleDbType.Varchar2).Value = CheckinDate;
                        objCmd.Parameters.Add("CHECKINTIME_IN", OracleDbType.Varchar2).Value = CheckinTime;
                        objCmd.Parameters.Add("CHECKOUTDATE_IN", OracleDbType.Varchar2).Value = CheckoutDate;
                        objCmd.Parameters.Add("CHECKOUTTIME_IN", OracleDbType.Varchar2).Value = CheckoutTime;
                    }
                    objCmd.Parameters.Add("PICKDROP_IN", OracleDbType.Varchar2).Value = PickDropDetails;

                    //Addding 4 Parameter 
                    objCmd.Parameters.Add("STAYINGCITYID_IN", OracleDbType.Int32).Value = StayingCityID;
                    objCmd.Parameters.Add("HOTEL_ID_IN", OracleDbType.Int32).Value = HotelID;
                    objCmd.Parameters.Add("MODIFIEDBY_IN", OracleDbType.Int32).Value = EmpCode;
                    objCmd.Parameters.Add("GSTNUMBER_IN", OracleDbType.Varchar2).Value = GSTNumber;

                    objCmd.Parameters.Add("HOTEL_ID2_IN", OracleDbType.Int32).Value = HotelID2;
                    objCmd.Parameters.Add("HOTEL_ID3_IN", OracleDbType.Int32).Value = HotelID3;

                    //Addding 5 Parameter by aumento on 06062023 for the SR50547===========================================
                    objCmd.Parameters.Add("HOTEL_COST_IN", OracleDbType.Varchar2).Value = Hotel_Cost;
                    objCmd.Parameters.Add("GSTPER_IN", OracleDbType.Varchar2).Value = GSTPer;
                    objCmd.Parameters.Add("TOTALAMOUNT_IN", OracleDbType.Int32).Value = TotalAmount;
                    objCmd.Parameters.Add("HOTELCATEGORY_IN", OracleDbType.Varchar2).Value = HotelCategory;
                    objCmd.Parameters.Add("HOTELBOOKEDBY_IN", OracleDbType.Varchar2).Value = HotelBookedBy;
                    objCmd.Parameters.Add("Hotel1GST_IN", OracleDbType.Varchar2).Value = Hotel1GST;
                    objCmd.Parameters.Add("Hotel2GST_IN", OracleDbType.Varchar2).Value = Hotel2GST;
                    //=====================================================================================================

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }




        public string UpdateTicketBookingStatus(string RequestID, string status)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATETKTBKNGSTATUS_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        public string UpdateGuestHouseBookingStatus(string RequestID, string status)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATEGSTBKNGSTATUS_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                    else if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "3")
                    {
                        throw new Exception("Cannot Refer To Hotel. Kindly mark Guest House as - Non Available in atleast one travel Day!.");
                    }
                    else if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "4")
                    {
                        throw new Exception("Kindly mark Guest House as - Non Available in atleast one travel Day!.");
                    }

                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        public string UpdateHotelBookingStatus(string RequestID, string status)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_UPDATEHTLBKNGSTATUS_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        public DataTable GetAllBookingStatus(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TOURBOOKINGSTATUS_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("CUR_REQTADATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public int CheckRequestEmailsStatus(string RequestID)
        {
            int TxnNumber = 0;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            OracleTransaction objTxn;
            int rownum;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                objTxn = objCn.BeginTransaction();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.Transaction = objTxn;
                    objCmd.CommandText = "PKG_TOURREQUEST.SPROC_EMAILID_GET_STATUS";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();
                    TxnNumber = System.Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());


                }
                catch (Exception ex)
                {
                    //ROLLBACK TRANSACTION
                    objTxn.Rollback();
                    ErrorMessage = ex.Message;

                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return (TxnNumber);
            }
        }


        //GET SERVICE TourAUTHORITY
        public DataSet GETTOURAUTHORITYLIST(string ADTOURAPPAUTHORITYID, string AdEmpcode, string status)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.Parameters.Add("ADTOURAPPAUTHORITYID_IN", OracleDbType.Varchar2).Value = ADTOURAPPAUTHORITYID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = AdEmpcode;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TOURAUTHORITY_GET";
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();
        }


        //INSERT SERVICE TOURAUTHRITY.
        public string INSERTADTOURAUTHORITY(string ADTOURAPPAUTHORITYID, string AdEmpcode, string RECADEMPCODE, string APPADEMPCODE, string status, string addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_TOURAUTHORITY_INSERT";
            oCmd.Parameters.Add("ADTOURAPPAUTHORITYID_IN", OracleDbType.Varchar2).Value = ADTOURAPPAUTHORITYID;
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = AdEmpcode;
            oCmd.Parameters.Add("RECADEMPCODE_IN", OracleDbType.Varchar2).Value = RECADEMPCODE;
            oCmd.Parameters.Add("APPADEMPCODE_IN", OracleDbType.Varchar2).Value = APPADEMPCODE;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        #region "Add Hotel Detail"
        public string AddHotelDetail(string HOTELID, string HotelName, string HotelAddress, string City, string State, string GSTNo, string sStatus,
                                      string userID, string OtherCityName)
        {
            OracleConnection objCn;
            OracleCommand objCmd;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();

                    string strSql = "PKG_TOURSETTLEMENTNEW.SPROC_MANAGEHOTEL_INSERT";
                    objCmd.Parameters.Add("HOTELID_IN", OracleDbType.Varchar2).Value = HOTELID;
                    objCmd.Parameters.Add("HOTELNAME_IN", OracleDbType.Varchar2).Value = HotelName;
                    objCmd.Parameters.Add("HOTELADDRESS_IN", OracleDbType.Varchar2).Value = HotelAddress;
                    objCmd.Parameters.Add("CITYID_IN", OracleDbType.Varchar2).Value = City;
                    objCmd.Parameters.Add("STATEID_IN", OracleDbType.Varchar2).Value = State;
                    objCmd.Parameters.Add("GSTNO_IN", OracleDbType.Varchar2).Value = GSTNo;
                    objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = sStatus;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = userID;
                    objCmd.Parameters.Add("OTHERCITY_IN", OracleDbType.Varchar2).Value = OtherCityName;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    oDataMgmt.ExecuteQuery(objCmd);
                    string strErr = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG_OUT"].Value.ToString();
                    return strErr;

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

        public DataSet GETMANAGEHOTEL(string hotelId, string hotelName, string status, string empcode, string syki)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_MANAGEHOTEL_GET";
            oCmd.Parameters.Add("HOTELID_IN", OracleDbType.Varchar2).Value = hotelId;
            oCmd.Parameters.Add("HOTELNAME_IN", OracleDbType.Varchar2).Value = hotelName;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
            //oCmd.Parameters.Add("COMPHEADMSTID_IN", OracleDbType.Varchar2).Value = comheaderid;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = syki;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public DataTable GetSingleHotelDetails(string HotelID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_MANAGE_SINGLE_HOTEL_GET";
            oCmd.Parameters.Add("HOTELID_IN", OracleDbType.Varchar2).Value = HotelID;
            oCmd.Parameters.Add("CUR_GETLIST ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }
        #endregion

        #region "Get Hotel Details By Admin"
        public DataTable GetHotelDetailsByAdmin(string strType, string strHotelName, string strHotelAddress, string strCity, string strState, string strGSTNo, string strStatus, string strAddedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_MANAGE_HOTEL_GET";
            //oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("strType", OracleDbType.Varchar2).Value = strType;
            oCmd.Parameters.Add("HOTELNAME_IN", OracleDbType.Varchar2).Value = strHotelName;
            oCmd.Parameters.Add("HOTELADDRESS_IN", OracleDbType.Varchar2).Value = strHotelAddress;
            oCmd.Parameters.Add("CITYID_IN", OracleDbType.Varchar2).Value = strCity;
            oCmd.Parameters.Add("STATEID_IN", OracleDbType.Varchar2).Value = strState;
            oCmd.Parameters.Add("GSTNO_IN", OracleDbType.Varchar2).Value = strGSTNo;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strStatus;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strAddedby;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        #endregion

        #region "Get Cost Center Details"
        public DataTable GetCostCenterDetails(string strCostCenterID, string strStateID, string strOperationID, string strCostCenter, string strProfitCenter, string strStatus)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_MANAGE_COST_CENTER_GET";
            //oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("COSTCENTERID_IN ", OracleDbType.Int32).Value = strCostCenterID == "" ? null : strCostCenterID;
            oCmd.Parameters.Add("STATEID_IN", OracleDbType.Int32).Value = strStateID == "" ? null : strStateID;
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Int32).Value = strOperationID == "" ? null : strOperationID;
            oCmd.Parameters.Add("COSTCENTER_IN", OracleDbType.Varchar2).Value = strCostCenter;
            oCmd.Parameters.Add("PROFITCENTER_IN", OracleDbType.Varchar2).Value = strProfitCenter;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strStatus;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        #endregion

        #region "Get GSTIN No Details"
        public DataTable GetGSTINNODetails(string strStateID, string strGSTINNo, string strStatus)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_MANAGE_GSTINNo_GET";
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_FINTBS_GSTINDETAILS_GET";
            //oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("STATE_IN", OracleDbType.Int32).Value = strStateID == "" ? null : strStateID;
            oCmd.Parameters.Add("GSTINNO_IN", OracleDbType.Int32).Value = strGSTINNo == "" ? null : strGSTINNo;
            //oCmd.Parameters.Add("COSTCENTER_IN", OracleDbType.Varchar2).Value = strCostCenter;
            //oCmd.Parameters.Add("PROFITCENTER_IN", OracleDbType.Varchar2).Value = strProfitCenter;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            oCmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        #endregion

        #region "Get GSTIN No Details for Edit"
        public DataTable GetGSTINNOEDITDetails(Int32 SRNo)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_FINTBS_GSTINDETAILS_Edit";
            oCmd.Parameters.Add("SRNO_", OracleDbType.Int32).Value = SRNo;
            oCmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        #endregion

        #region "Add Hotel Detail"
        public string AddCostCenterDetail(string strCostCenterID, string strStateID, string strOperationID, string strCostCenter, string strProfitCenter, string strEffectiveForm, string strEffectiveTo, string strStatus,
                                      string userID)
        {
            OracleConnection objCn;
            OracleCommand objCmd;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();

                    string strSql = "PKG_TOURSETTLEMENTNEW.SPROC_MANAGE_COSTCENTER_INSERT";
                    objCmd.Parameters.Add("COSTCENTERID_IN", OracleDbType.Int32).Value = strCostCenterID == "" ? null : strCostCenterID;
                    objCmd.Parameters.Add("STATEID_IN", OracleDbType.Int32).Value = strStateID == "" ? null : strStateID;
                    objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Int32).Value = strOperationID == "" ? null : strOperationID;
                    objCmd.Parameters.Add("COSTCENTER_IN", OracleDbType.Varchar2).Value = strCostCenter;
                    objCmd.Parameters.Add("PROFITCENTER_IN", OracleDbType.Varchar2).Value = strProfitCenter;
                    objCmd.Parameters.Add("EFFECTIVEFORM_IN", OracleDbType.Varchar2).Value = strEffectiveForm;
                    objCmd.Parameters.Add("EFFECTIVETO_IN", OracleDbType.Varchar2).Value = strEffectiveTo;
                    objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strStatus;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = userID;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    oDataMgmt.ExecuteQuery(objCmd);
                    string strErr = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG_OUT"].Value.ToString();
                    return strErr;

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

        public string AddGSTINNoDetail(string StrSRNo, string strState, string strGSTINNo, string strEffectiveForm, string strEffectiveTo, string strStatus, string strAddress,
                                      string userID)
        {


            OracleConnection objCn;
            OracleCommand objCmd;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();

                    string strSql = "PKG_TOURSETTLEMENTNEW.SPROC_MANAGE_GSTCENTER_INSERT";
                    //objCmd.Parameters.Add("SRNO_IN", OracleDbType.Int32).Value = StrSRNo;
                    objCmd.Parameters.Add("SRNO_IN", OracleDbType.Int32).Value = StrSRNo == "" ? null : StrSRNo;
                    objCmd.Parameters.Add("STATE_IN", OracleDbType.Int32).Value = strState == "" ? null : strState;
                    objCmd.Parameters.Add("GSTINNo_IN", OracleDbType.Varchar2).Value = strGSTINNo;
                    objCmd.Parameters.Add("EFFECTIVEFROMDATE_IN", OracleDbType.Varchar2).Value = strEffectiveForm;
                    objCmd.Parameters.Add("EFFECTIVETODATE_IN", OracleDbType.Varchar2).Value = strEffectiveTo;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                    objCmd.Parameters.Add("ADDRESS_IN", OracleDbType.Varchar2).Value = strAddress;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = userID;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    oDataMgmt.ExecuteQuery(objCmd);
                    string strErr = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG_OUT"].Value.ToString();
                    return strErr;

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

        public DataSet GetCityListnew(string stateid)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_CITYLIST_GET";
            oCmd.Parameters.Add("STATEID_IN", OracleDbType.Varchar2).Value = stateid;
            oCmd.Parameters.Add("CUR_CITYLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        #endregion

        // Added by Kishan Dodiya
        public DataTable GetExpatsUser()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_EXPATSUSERLIST_GET";
            oCmd.Parameters.Add("CUR_EXPATS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        // End Added by Kishan Dodiya

        //region Start Added by Aumento as on 19022024 
        #region "Expat Tour Approval Matrix Master"
        public DataTable GetExpatsUserList()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_EXPATSUSERLIST_GET";
            oCmd.Parameters.Add("CUR_EXPATS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public string AddExpatTourAppMatDetail(int adempcode, int tourapp1, int tourapp2, int settlapp1, int settlapp2, int addedby, DateTime addeddate, int modifiedby, DateTime modifieddate)
        {
            OracleConnection objCn;
            OracleCommand objCmd;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();

                    string strSql = "PKG_TOURSETTLEMENTNEW.SPROC_ADDEXPATTOURAPPMAT_INSERT";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32, 1).Value = adempcode;
                    objCmd.Parameters.Add("TOURAPP1_IN", OracleDbType.Int32, 1).Value = tourapp1;
                    objCmd.Parameters.Add("TOURAPP2_IN", OracleDbType.Int32, 1).Value = tourapp2;
                    objCmd.Parameters.Add("SETTLAPP1_IN", OracleDbType.Int32, 1).Value = settlapp1 == 0 ? 0 : settlapp1;
                    objCmd.Parameters.Add("SETTLAPP2_IN", OracleDbType.Int32, 1).Value = settlapp2;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32, 1).Value = addedby;
                    objCmd.Parameters.Add("ADDEDDATE_IN", OracleDbType.Date).Value = addeddate;
                    objCmd.Parameters.Add("MODIFIEDBY_IN", OracleDbType.Int32, 1).Value = modifiedby;
                    objCmd.Parameters.Add("MODIFIEDDATE_IN", OracleDbType.Date).Value = modifieddate;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    oDataMgmt.ExecuteQuery(objCmd);
                    string strErr = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG_OUT"].Value.ToString();
                    return strErr;

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

        public string UpdateExpatTourAppMatDetail(int adempcode, int tourapp1, int tourapp2, int settlapp1, int settlapp2, int addedby, DateTime addeddate, int modifiedby, DateTime modifieddate)
        {
            OracleConnection objCn;
            OracleCommand objCmd;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();

                    string strSql = "PKG_TOURSETTLEMENTNEW.SPROC_ADDEXPATTOURAPPMAT_UPDTAE";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32, 1).Value = adempcode;
                    objCmd.Parameters.Add("TOURAPP1_IN", OracleDbType.Int32, 1).Value = tourapp1;
                    objCmd.Parameters.Add("TOURAPP2_IN", OracleDbType.Int32, 1).Value = tourapp2;
                    objCmd.Parameters.Add("SETTLAPP1_IN", OracleDbType.Int32, 1).Value = settlapp1;
                    objCmd.Parameters.Add("SETTLAPP2_IN", OracleDbType.Int32, 1).Value = settlapp2;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32, 1).Value = addedby;
                    objCmd.Parameters.Add("ADDEDDATE_IN", OracleDbType.Date).Value = addeddate;
                    objCmd.Parameters.Add("MODIFIEDBY_IN", OracleDbType.Int32, 1).Value = modifiedby;
                    objCmd.Parameters.Add("MODIFIEDDATE_IN", OracleDbType.Date).Value = modifieddate;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    oDataMgmt.ExecuteQuery(objCmd);
                    string strErr = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG_OUT"].Value.ToString();
                    return strErr;

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

        public DataTable GetExpatTourAppMatMaster(string Ecode, string TourApp1, string SettlApp1, string SettlApp2)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_EXPAT_TOUR_APP_MAST_GET";
            //oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Ecode;
            oCmd.Parameters.Add("TOURAPP1_IN", OracleDbType.Varchar2).Value = TourApp1;
            oCmd.Parameters.Add("SETTLAPP1_IN", OracleDbType.Varchar2).Value = SettlApp1;
            oCmd.Parameters.Add("SETTLAPP2_IN", OracleDbType.Varchar2).Value = SettlApp2;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        #endregion
        //region End Added by Aumento as on 19022024 
    }
}
