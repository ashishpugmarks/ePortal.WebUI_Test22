using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Services
{
    public class NavigationMaster_DAL : INavigationMaster_DAL
    {
        #region"Ctor"
        public NavigationMaster_DAL()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion
        #region"Instance variables"

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        public NavigationMaster_DAL(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }
        //DataSet ds = new DataSet();
        //DataRow[] _datarow;
        //string qry;
        #endregion

        #region "Get Data"

        public DataTable GetNavigationList()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_NAVIGATIONLIST_GET";
            oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetNavigationListByEmpCode(String strEmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_NAVIGATIONTBYEMPCODE_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetActivityListForNavigation(String strEmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_NAVIGATION_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetNavigationUsersListByActivity(String strActivity)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_NAVUSERTBYAPPID_GET";
            oCmd.Parameters.Add("ACTIVITY_IN", OracleDbType.Varchar2).Value = strActivity;
            oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetUserNavigationRights(String strEmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_NAVLISTBYEMPCODE_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        // add by vishal

        public DataTable GetUserRightsListByEmpCode(String strEmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_USERRIGHTSBYEMPCODE_LIST";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetRoleMasterList(String strEmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_ROLEMASTERLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public int InsertUpdateUserRights(String strXML)
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;


            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_NAVIGATION.SPROC_ASSIGN_USERRIGHTS";
                    objCmd.Parameters.Add("XML_IN", OracleDbType.Varchar2).Value = strXML;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

        // end

        public DataTable GetNavigationDetails(String strID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_NAVIGATIONBYID_GET";
            oCmd.Parameters.Add("ACTIVITY_ID", OracleDbType.Varchar2).Value = strID;
            oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetUsers()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_SYUSER_GET";
            oCmd.Parameters.Add("CUR_USER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public String GetUserName(String strEmpCode)
        {

            String strName = String.Empty;
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_USERNAME_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            oCmd.Parameters.Add("CUR_USER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            if (dt.Rows.Count > 0)
            {
                strName = Convert.ToString(dt.Rows[0]["NAME"]);
            }
            else
                strName = "";


            return strName;
        }

        public DataTable GetNavRightsDetails(String strID, String strEmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_NAVRIGHTSBYID_GET";
            oCmd.Parameters.Add("RIGHT_ID", OracleDbType.Varchar2).Value = strID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            oCmd.Parameters.Add("CUR_NAVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public int CheckUserRight(String strActivityID)
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;


            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_NAVIGATION.SPROC_COUNT_USRACTIVITY";
                    objCmd.Parameters.Add("ACTIVITY_ID", OracleDbType.Varchar2).Value = strActivityID;
                    objCmd.Parameters.Add("COUNT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["COUNT_OUT"].Value.ToString());
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

        public uint CheckActivityParent(String strEmpCode, String strActivityID)
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_NAVIGATION.SPROC_CHECKACTIVITYPARENT_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("ACTIVITY_IN", OracleDbType.Varchar2).Value = strActivityID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToUInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

        #endregion

        #region"INSERT-UPDATE FUNCTION"
        public int InsertUpdateNavigation(String strID, String strDescription,
                                          String strAltValue, String strNodeUrl,
                                          String strIsParent, String strParentID,
                                          String strBy, String strActive,
                                          String strPageName, String strDisplayOrder)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;


            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_NAVIGATION.SPROC_UPDATE_NAVIGATION";
                    objCmd.Parameters.Add("SYACTIVITYTYPEID_IN", OracleDbType.Varchar2).Value = strID;
                    objCmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = strDescription;
                    objCmd.Parameters.Add("BY_IN", OracleDbType.Varchar2).Value = strBy;
                    objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strActive;
                    objCmd.Parameters.Add("PARENTSYACTIVITYTYPEID_IN", OracleDbType.Varchar2).Value = strParentID;
                    objCmd.Parameters.Add("NODEURL_IN", OracleDbType.Varchar2).Value = strNodeUrl;
                    objCmd.Parameters.Add("ALTVALUE_IN", OracleDbType.Varchar2).Value = strAltValue;
                    objCmd.Parameters.Add("ISPARENT_IN", OracleDbType.Varchar2).Value = strIsParent;
                    objCmd.Parameters.Add("PAGENAME_IN", OracleDbType.Varchar2).Value = strPageName;
                    objCmd.Parameters.Add("DISPLAYORDER_IN", OracleDbType.Varchar2).Value = strDisplayOrder;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;


                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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


        //public int InsertUpdateNavRights(String strID, String strEmpCode,
        //                                  String strActivityID, String strEnabled,
        //                                  String strBy)
        //{

        //    ConnectionString objCnStr = new ConnectionString(); ;
        //    string strCn = objCnStr.getConnectingString(); ;
        //    OracleCommand objCmd;


        //    using (OracleConnection objCn = new OracleConnection())
        //    {
        //        objCn.ConnectionString = strCn;
        //        objCn.Open();
        //        try
        //        {
        //            objCmd = new OracleCommand();
        //            objCmd.Connection = objCn;
        //            string strSql = "PKG_NAVIGATION.SPROC_UPDATE_NAVRIGHTS";
        //            objCmd.Parameters.Add("SYUSERRIGHTSID_IN", OracleDbType.Varchar2).Value = strID;
        //            objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
        //            objCmd.Parameters.Add("SYACTIVITYTYPEID_IN", OracleDbType.Varchar2).Value = strActivityID;
        //            objCmd.Parameters.Add("ENABLED_IN", OracleDbType.Varchar2).Value = strEnabled;
        //            objCmd.Parameters.Add("BY_IN", OracleDbType.Varchar2).Value = strBy;
        //            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

        //            objCmd.CommandText = strSql;
        //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            objCmd.ExecuteNonQuery();
        //            return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value);
        //        }
        //        finally
        //        {
        //            if (objCn != null)
        //            {
        //                objCn.Close();
        //            }
        //        }

        //    }
        //}

        public int InsertUpdateNavRights(String strXML)
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;


            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_NAVIGATION.SPROC_ASSIGNUSER_NAVRIGHTS";
                    objCmd.Parameters.Add("XML_IN", OracleDbType.Varchar2).Value = strXML;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

        public string DeleteNavRights(String strEmpCode, String strActivityID, String strBy, String ACCESSTYPE) // ACCESSTYPE added for SR78412
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;


            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_NAVIGATION.SPROC_REMOVE_NAVRIGHTS";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("NAVID_IN", OracleDbType.Varchar2).Value = strActivityID;
                    objCmd.Parameters.Add("BY_IN", OracleDbType.Varchar2).Value = strBy;
                    objCmd.Parameters.Add("ACCESSTYPE_IN", OracleDbType.Varchar2).Value = ACCESSTYPE; // ACCESSTYPE added for SR78412
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return objCmd.Parameters["RESULT_OUT"].Value.ToString();
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

        #endregion


        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //Added cloumn for  Login Expire Date,Description(User Type) and Added User Type Dropdown in search criteria on 13-12-2022(Aumento) 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        #region User Access Review
        //public DataTable AssessReviewReport_Get(string strEmpcode, string strEmpName, string strMonth, string strYear, string strStatus,string strUserType)
        public DataTable AssessReviewReport_Get(string strEmpcode, string strEmpName, string FromDate, string ToDate, string strStatus, string strUserType)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_USERACCESSREVIEW_GET";
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
            //ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strMonth;
            //ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = strYear;
            ocmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            ocmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = ToDate;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("USER_TYPE", OracleDbType.Varchar2).Value = strUserType;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //Added FINT_GSTNO_MST on 15-12-2022(Aumento)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //(strGSTINNO, strSTATE, strStatus);
        public DataTable FINTBS_GSTIN_Get(string strGSTINNo, string strState, string strStatus)
        {
            DataTable dt = new DataTable();
            try
            {
                OracleCommand ocmd = new OracleCommand();
                ocmd = new OracleCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.BindByName = true;
                ocmd.CommandText = "PKG_NAVIGATION.SPROC_FINTBS_GSTINDETAILS_GET";
                ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("GSTINNO_IN", OracleDbType.Varchar2).Value = strGSTINNo;
                ocmd.Parameters.Add("STATE_IN", OracleDbType.Varchar2).Value = strState;
                ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;


                dt = oDataMgmt.GetDataTable(ocmd);
            }
            catch (OracleException ex)
            {
                dt = null;
            }

            return dt;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //Added New UserDetails Procedure on 15-12-2022(Aumento)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public DataTable UserDetailsReport_Get(string strEmpcode, string strEmpName, string strUserType, string FromDate, string ToDate)
        {
            DataTable dt = new DataTable();
            try
            {
                OracleCommand ocmd = new OracleCommand();
                ocmd = new OracleCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.BindByName = true;
                ocmd.CommandText = "PKG_NAVIGATION.SPROC_USERDETAILS_GET";
                ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
                ocmd.Parameters.Add("USERTYPE_IN", OracleDbType.Varchar2).Value = strUserType;
                ocmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
                ocmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = ToDate;
                dt = oDataMgmt.GetDataTable(ocmd);
            }
            catch (OracleException ex)
            {
                dt = null;
            }

            return dt;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //Added New Third Party User List Procedure on 16-12-2022(Aumento)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public DataTable ThirdPartyUserList_GET(string strEmpcode, string strEmpName, string strEmpType)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_ThirdPartyUserList_GET";
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
            ocmd.Parameters.Add("EMPLOYEETYPE_IN", OracleDbType.Varchar2).Value = strEmpType;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public DataTable PermanentEmployeeReport_Get(string strEmpcode, string strEmpName)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_PERMANENTEMPREPORT_GET";
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
            //ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strMonth;
            //ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = strYear;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }

        #endregion
        public DataTable PermanentEmployeeReport_Get(string strEmpcode, string strEmpName, string strMonth, string strYear)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_PERMANENTEMPREPORT_GET";
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
            //ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strMonth;
            //ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = strYear;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        #region"INSERT-UPDATE FUNCTION"
        public int InsertUpdateGSTDetails(String strSRNo, String strState,
                                          String strGstInNo, String strEffectiveFromDate,
                                          String strEffectiveToDate, String strStatus
                                          )
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;


            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_NAVIGATION.SPROC_UPDATEGSTDetails";
                    objCmd.Parameters.Add("SRNO_IN", OracleDbType.Varchar2).Value = strSRNo;
                    objCmd.Parameters.Add("STATE_IN", OracleDbType.Varchar2).Value = strState;
                    objCmd.Parameters.Add("GSTINNO_IN", OracleDbType.Varchar2).Value = strGstInNo;
                    objCmd.Parameters.Add("EFFECTIVEFROMDATE_IN", OracleDbType.Varchar2).Value = strEffectiveFromDate;
                    objCmd.Parameters.Add("EFFECTIVETODATE_IN", OracleDbType.Varchar2).Value = strEffectiveToDate;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;


                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //-----SR39197------------ IT GRC Change Start(Aumento)--------------------------------------------------- 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Employee Access Review

        //public string DeActiveUser(String strEmpCode, string strDate, string modifiedBy)
        public string DeActiveUser(String strEmpCode, string modifiedBy)
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            //strDate = DateTime.Now.ToString("yyyyMMdd");

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_NAVIGATION.SPROC_DEACTIVEUSERLIST_GET";
                    objCmd.Parameters.Add("RESIGNEDCODES", OracleDbType.Varchar2).Value = strEmpCode;
                    //objCmd.Parameters.Add("RESIGNEDDATE", OracleDbType.Varchar2).Value = strDate;
                    objCmd.Parameters.Add("MODIFIED_BY", OracleDbType.Varchar2).Value = modifiedBy;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    return "true";
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

        public DataTable EmployeeReviewReport_Get(string strEmpcode, string strEmpName, string strUserType, string operation)
        {
            try
            {
                OracleCommand ocmd = new OracleCommand();
                DataTable dt = new DataTable();
                ocmd = new OracleCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.BindByName = true;
                ocmd.CommandText = "PKG_NAVIGATION.SPROC_EMPLOYEEACCESSREVIEW_GET";
                ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
                // SR75389 start
                ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = operation;
                // SR75389 end
                //ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                ocmd.Parameters.Add("USER_TYPE", OracleDbType.Varchar2).Value = strUserType;
                dt = oDataMgmt.GetDataTable(ocmd);
                int data = dt.Rows.Count;
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        public DataSet get_AllOperation()
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_OPERATION_GET";
            oCmd.Parameters.Add("CUR_OPERATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            objDs = oDataMgmt.GetDataSet(oCmd);
            return (objDs);
        }
        public DataSet get_AllDivision(string OperationID)
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            if (OperationID != "")
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_NAVIGATION.SPROC_DIVISION_GET";
                oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = Convert.ToInt32(OperationID);
                oCmd.Parameters.Add("CUR_DIVISION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.BindByName = true;
                objDs = oDataMgmt.GetDataSet(oCmd);
            }
            return (objDs);
        }

        public DataSet get_AllDepartment(string OperationID, string DivisionID)
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            if (OperationID != "" && DivisionID != "")
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_NAVIGATION.SPROC_DEPARTMENT_GET";
                oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = Convert.ToInt32(OperationID);
                oCmd.Parameters.Add("ADDIVISIONID_IN", OracleDbType.Varchar2).Value = Convert.ToInt32(DivisionID);
                oCmd.Parameters.Add("CUR_DEPARTMENT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.BindByName = true;
                objDs = oDataMgmt.GetDataSet(oCmd);
            }
            return (objDs);
        }

        public DataTable GetEmployeeEDITDetails(Int32 ECODE)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_OPERATION_Edit";
            oCmd.Parameters.Add("ECODE_", OracleDbType.Int32).Value = ECODE;
            oCmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataSet get_Review(long OperationID)
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_GETAPPAUTHORITY";
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Int32).Value = OperationID;
            oCmd.Parameters.Add("CUR_REVIEWAPPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            objDs = oDataMgmt.GetDataSet(oCmd);
            return (objDs);
        }

        public DataSet get_Review2(long OperationID)
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_GETREPORTINGMANAGER";
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Int32).Value = OperationID;
            oCmd.Parameters.Add("CUR_REVIEWAPPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            objDs = oDataMgmt.GetDataSet(oCmd);
            return (objDs);
        }

        public string UpdateEmployee(string strEmpCode, string StrOperation, string StrDivision,
                                       string StrDepartment)
        {
            try
            {
                //ConnectionString objCnStr;
                string strCn;
                OracleCommand objCmd;
                string strValidHalfDay = string.Empty;
                //objCnStr = new ConnectionString();
                strCn = objCnStr.getConnectingString();
                using (OracleConnection objCn = new OracleConnection())
                {
                    objCn.ConnectionString = strCn;
                    try
                    {
                        objCn.Open();
                        objCmd = new OracleCommand();
                        objCmd.Connection = objCn;
                        objCmd.CommandText = "PKG_NAVIGATION.SPROC_UPDATEEMPLOYEEDETAILS";
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                        objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = StrOperation;
                        objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = StrDivision;
                        objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Int32).Value = StrDepartment;
                        objCmd.BindByName = true;
                        objCmd.ExecuteNonQuery();
                        return "0";
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet get_ReportingManager1(string OperationID)
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            if (OperationID != "")
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_NAVIGATION.SPROC_REPORTINGMANAGER1_GET";
                oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = Convert.ToInt32(OperationID);
                oCmd.Parameters.Add("CUR_REPORTINGMANAGER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.BindByName = true;
                objDs = oDataMgmt.GetDataSet(oCmd);

            }
            return (objDs);
        }
        public string InsertEmployee(long EmpCode, string EmpName, long OperationID, long DivisionID, long DepartmentID, long Reviewer1, long Reviewer2,
            long UserType, long reviwer, long strLoginEmpCode)
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            bool flag;
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_NAVIGATION.SPROC_INSERTEMPLOYEE";
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Long).Value = EmpCode;
                oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = EmpName;
                oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Long).Value = OperationID;
                oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Long).Value = DivisionID;
                oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Long).Value = DepartmentID;
                oCmd.Parameters.Add("REVIEWER1_IN", OracleDbType.Long).Value = Reviewer1;
                oCmd.Parameters.Add("REVIEWER2_IN", OracleDbType.Long).Value = Reviewer2;
                oCmd.Parameters.Add("USERTYPE_IN", OracleDbType.Long).Value = UserType;
                oCmd.Parameters.Add("SELECTEDREVIEWER_IN", OracleDbType.Long).Value = reviwer;
                oCmd.Parameters.Add("LOGIN_EMPCODE", OracleDbType.Long).Value = strLoginEmpCode;
                oCmd.BindByName = true;
                objDs = oDataMgmt.GetDataSet(oCmd);
            }
            catch (OracleException ex)
            {

                throw ex;
            }
            flag = objDs.HasErrors == false ? true : false;
            return (flag.ToString());
        }
        public DataTable UARRequestList_Get(string strEmpcode, string LoginCode)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_UARREQUESTLIST_GET";
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("SELECTEDREVIEWER_", OracleDbType.Varchar2).Value = LoginCode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        public DataTable GetUARList(int loginCode, string Status, string Ename, string Ecode)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.GETUARLIST_GET";
            ocmd.Parameters.Add("LOGINCODE_IN", OracleDbType.Int64).Value = loginCode;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = Status;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = Ename;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Ecode;
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }

        public DataTable ReviewReport_Get(string strEmpcode, string strEmpName, string strStatus, string strUserType, string RequestDate, string operation, string SYKIID, string RequestToDate)
        {
            try
            {
                OracleCommand ocmd = new OracleCommand();
                DataTable dt = new DataTable();
                ocmd = new OracleCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.BindByName = true;
                ocmd.CommandText = "PKG_NAVIGATION.SPROC_REVIEWREPORT_GET";
                //ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                //ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                //ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
                //ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                //ocmd.Parameters.Add("REQUESTDATE_IN", OracleDbType.Varchar2).Value = RequestDate;
                //ocmd.Parameters.Add("USER_TYPE", OracleDbType.Varchar2).Value = strUserType;
                ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
                ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                ocmd.Parameters.Add("REQUESTDATE_IN", OracleDbType.Varchar2).Value = RequestDate;
                ocmd.Parameters.Add("USER_TYPE", OracleDbType.Varchar2).Value = strUserType;
                // SR75389 start
                ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = operation;
                // SR75389 end
                // SR78412 start
                ocmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = SYKIID;
                ocmd.Parameters.Add("REQUESTTODATE_IN", OracleDbType.Varchar2).Value = RequestToDate;
                // SR78412 end
                dt = oDataMgmt.GetDataTable(ocmd);
                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public DataTable UpdateReviwer(string strEmpcode, string strEName, string UserType, string operation)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_UPDATEREVIEWER_GET";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = strEName;
            ocmd.Parameters.Add("USER_TYPE", OracleDbType.Varchar2).Value = UserType;
            // SR75389 start
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = operation;
            // SR75389 end
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        public DataTable GetUAREDITDetails(Int32 ECODE)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_UARDETAILS_Edit";
            oCmd.Parameters.Add("ECODE_", OracleDbType.Int32).Value = ECODE;
            oCmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }

        public string UpdateReviewer(string ecode, string remark, string selectedReviewer, string operation, string division, string department)
        {
            try
            {
                //ConnectionString objCnStr;
                string strCn;
                OracleCommand objCmd;
                string strValidHalfDay = string.Empty;
                //objCnStr = new ConnectionString();
                strCn = objCnStr.getConnectingString();
                using (OracleConnection objCn = new OracleConnection())
                {
                    objCn.ConnectionString = strCn;
                    try
                    {
                        objCn.Open();
                        objCmd = new OracleCommand();
                        objCmd.Connection = objCn;
                        objCmd.CommandText = "PKG_NAVIGATION.SPROC_UPDATEDATA_GET";
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.Parameters.Add("V_ECODE", OracleDbType.Int32).Value = ecode;
                        objCmd.Parameters.Add("V_REMARK", OracleDbType.Varchar2).Value = remark;
                        objCmd.Parameters.Add("V_SELECTEDREVIEWER", OracleDbType.Int32).Value = selectedReviewer;
                        objCmd.Parameters.Add("V_SELECTEDOPERATION", OracleDbType.Varchar2).Value = operation;
                        objCmd.Parameters.Add("V_SELECTEDDIVISION", OracleDbType.Varchar2).Value = division;
                        objCmd.Parameters.Add("V_SELECTEDDEPARTMENT", OracleDbType.Varchar2).Value = department;
                        objCmd.BindByName = true;
                        objCmd.ExecuteNonQuery();
                        return "0";
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet get_ReportingManagerDivision(string DivisionID)
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            if (DivisionID != "")
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_NAVIGATION.SPROC_REVIEWER_GET";
                oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = Convert.ToInt32(DivisionID);
                oCmd.Parameters.Add("CUR_REPORTINGMANAGER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.BindByName = true;
                objDs = oDataMgmt.GetDataSet(oCmd);

            }
            return (objDs);
        }
        public DataSet get_ReportingManagerDepartment(string DepartmentID)
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            if (DepartmentID != "")
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_NAVIGATION.SPROC_DEPARTMENTREVIEWER_GET";
                oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = Convert.ToInt32(DepartmentID);
                oCmd.Parameters.Add("CUR_REPORTINGMANAGER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.BindByName = true;
                objDs = oDataMgmt.GetDataSet(oCmd);

            }
            return (objDs);
        }
        //SR78412 Changes Start
        public DataTable GetUARRegularList(int loginCode, string Status, string Ename, string Ecode)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_UARREGULARLIST_GET";
            ocmd.Parameters.Add("LOGINCODE_IN", OracleDbType.Int64).Value = loginCode;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = Status;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = Ename;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Ecode;
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        public DataTable UARRegularRequestList_Get(string strEmpcode, string REVIEWER)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_UARREGULARREQUESTLIST_GET";
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("SELECTEDREVIEWER_", OracleDbType.Varchar2).Value = REVIEWER;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        public string DeleteRegularNavRights(String strEmpCode, String strActivityID, String strBy, String ACCESSTYPE, int UserChecked) // ACCESSTYPE added for SR78412 //SR102715
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;


            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_NAVIGATION.SPROC_REMOVE_REGULARNAVRIGHTS";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("NAVID_IN", OracleDbType.Varchar2).Value = strActivityID;
                    objCmd.Parameters.Add("BY_IN", OracleDbType.Varchar2).Value = strBy;
                    objCmd.Parameters.Add("ACCESSTYPE_IN", OracleDbType.Varchar2).Value = ACCESSTYPE;
                    objCmd.Parameters.Add("USERCHECKED_IN", OracleDbType.Int32).Value = UserChecked; //SR102715
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return objCmd.Parameters["RESULT_OUT"].Value.ToString();
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
        public DataTable UARRegularReviewerRequestList_Get(string strEmpcode, string REVIEWER)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_UARREGULARREVIEWERREQUESTLIST_GET";
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("SELECTEDREVIEWER_", OracleDbType.Varchar2).Value = REVIEWER;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        public DataTable ReviewStatusReport_Get(string strEmpcode, string strEmpName, string strStatus, string strUserType, string RequestDate, string operation, string SYKIID, string RequestToDate)
        {
            try
            {
                OracleCommand ocmd = new OracleCommand();
                DataTable dt = new DataTable();
                ocmd = new OracleCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.BindByName = true;
                ocmd.CommandText = "PKG_NAVIGATION.SPROC_REVIEWSTATUSREPORT_GET";
                ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
                ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                ocmd.Parameters.Add("REQUESTDATE_IN", OracleDbType.Varchar2).Value = RequestDate;
                ocmd.Parameters.Add("USER_TYPE", OracleDbType.Varchar2).Value = strUserType;
                ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = operation;
                ocmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = SYKIID;
                ocmd.Parameters.Add("REQUESTTODATE_IN", OracleDbType.Varchar2).Value = RequestToDate;
                dt = oDataMgmt.GetDataTable(ocmd);
                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public DataTable GetUARStatusList(int loginCode, string Status, string Ename, string Ecode, string SYKIID) //SYKIID Added for SR78412
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_UARSTATUSLIST_GET";
            ocmd.Parameters.Add("LOGINCODE_IN", OracleDbType.Int64).Value = loginCode;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = Status;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = Ename;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Ecode;
            ocmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = SYKIID; //SYKIID Added for SR78412
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        public DataSet get_AllSYKI()
        {
            DataSet objDs = new DataSet();
            objDs = new DataSet();
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_NAVIGATION.SPROC_SYKI_GET";
            oCmd.Parameters.Add("CUR_SYKI", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            objDs = oDataMgmt.GetDataSet(oCmd);
            return (objDs);
        }
        //SR78412 Changes End
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //-----SR39197------------ IT GRC Change End(Aumento)--------------------------------------------------- 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //SR92683 Changes Start
        public DataTable GetSelfReviewRemarks(long AdEmpCode, long REVIEWER)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_GETSELFREVIEWREMARKS";
            ocmd.Parameters.Add("EMPCODE_", OracleDbType.Int64).Value = AdEmpCode;
            ocmd.Parameters.Add("SELECTEDREVIEWER_", OracleDbType.Int64).Value = REVIEWER;
            ocmd.Parameters.Add("CUR_TBL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        public string UpdateSelfReviewRemarks(long AdEmpCode, String SelfRemarks)
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_NAVIGATION.SPROC_UPDATESELFREVIEWREMARKS";
                    objCmd.Parameters.Add("EMPCODE_", OracleDbType.Int64).Value = AdEmpCode;
                    objCmd.Parameters.Add("SELFREMARKS_", OracleDbType.Varchar2).Value = SelfRemarks;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    return "true";
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
        public DataTable GetReviewerRemarks(long AdEmpCode, long REVIEWER)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_GETREVIEWERREMARKS";
            ocmd.Parameters.Add("EMPCODE_", OracleDbType.Int64).Value = AdEmpCode;
            ocmd.Parameters.Add("SELECTEDREVIEWER_", OracleDbType.Int64).Value = REVIEWER;
            ocmd.Parameters.Add("CUR_TBL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        public string UpdateReviewerRemarks(long AdEmpCode, String Remarks)
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_NAVIGATION.SPROC_UPDATEREVIEWERREMARKS";
                    objCmd.Parameters.Add("EMPCODE_", OracleDbType.Int64).Value = AdEmpCode;
                    objCmd.Parameters.Add("REMARKS_", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    return "true";
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
        public DataTable UARRegularRequestHistoryList_Get(String UARID)
        {
            OracleCommand ocmd = new OracleCommand();
            DataTable dt = new DataTable();
            ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_NAVIGATION.SPROC_UARREGULARREQUESTHISTORYLIST_GET";
            ocmd.Parameters.Add("REVIEW_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("UARID_", OracleDbType.Varchar2).Value = UARID;
            dt = oDataMgmt.GetDataTable(ocmd);
            return dt;
        }
        //SR92683 Changes End
    }
}
