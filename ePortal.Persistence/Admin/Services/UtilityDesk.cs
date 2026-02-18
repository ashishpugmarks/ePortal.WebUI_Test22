using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Services
{
    public class UtilityDesk : IUtilityDesk
    {
        #region "Local Variables"
        //DataManagement oDataMgmt = new DataManagement();
        //ConnectionString

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objConn;
        private readonly ICommonFunctions _objCommon;

        OracleCommand oCmd;
        DataSet ds;
        DataTable Dt;
        //OracleConnection objConn;
        string strConn;
        string errMsg = string.Empty;
        #endregion
        public UtilityDesk(IDataManagement _oDataMgmt, IConnectionString _objConn, ICommonFunctions objCommon)
        {
            oDataMgmt = _oDataMgmt;
            objConn = _objConn;
            _objCommon = objCommon;
        }
        public DataSet get_ServiceCatalog(string a)
        {
            string strCn = objConn.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    if (a == "1") // a=1 is for utility repair call
                    {
                        strSql = "select t.utservicecatalogid as servicecatalogid, t.descrip as servicecatalog from utservicecatalog t Where Active=1 and t.servicetype=1 Order By T.DESCRIP";
                    }
                    else if (a == "2")       // a=2 is for admin call mgmt system
                    {
                        strSql = "select t.utservicecatalogid as servicecatalogid, t.descrip as servicecatalog from utservicecatalog t Where Active=1 and t.servicetype=2 Order By T.UTSERVICECATALOGID";
                    }
                    else
                    {
                        strSql = "select t.utservicecatalogid as servicecatalogid, t.descrip as servicecatalog from utservicecatalog t Where Active=1 Order By T.servicetype";
                    }

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
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

        public DataSet get_ServiceCatalog()
        {
            string strCn = objConn.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    strSql = "select t.utservicecatalogid as servicecatalogid, t.descrip as servicecatalog from utservicecatalog t Where Active=1 and t.servicetype=1 Order By T.DESCRIP";
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
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

        public DataSet get_ServiceCatalogAdmin()
        {
            string strCn = objConn.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    strSql = "SELECT t.utservicecatalogid as servicecatalogid, t.descrip as servicecatalog FROM utservicecatalog t left join adcallrequest ad on ad.adcallrequestid = t.utservicecatalogid Where Active=1 and t.servicetype=2 Order By T.UTSERVICECATALOGID";
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
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

        public DataSet get_Classification(string servicecatalogid)
        {
            string strConn;

            OracleConnection objCn;
            OracleCommand objCmd;

            strConn = objConn.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_UTILITY.SPROC_GET_CLASSIFICATION";
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("CATALOGID_IN", OracleDbType.Int32).Value = servicecatalogid;

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

        public string SubmitTicket(int strCode, int strExt, long strContactNo, string strDirect, string strLoc, string strLine, string strStation, string strServiceCatalog, string strClassification, string strDetail, string strActDetail, string strSite)
        {
            string strConn = objConn.getConnectingString();
            int currentSeqNo;
            string result = string.Empty;

            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                string strTicketType = strActDetail;
                //MSharma 21/07/2010 Added If condition to check the call type (When the strTicketType is 1 then it is Utility Call)
                if (strTicketType == "1")
                {
                    try
                    {
                        objConn.Open();
                        OracleCommand objCmd = new OracleCommand();

                        string strSql = "PKG_UTILITY.SPROC_ADD_TICKET";
                        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strCode;
                        objCmd.Parameters.Add("EXTNO_IN", OracleDbType.Int32).Value = strExt;
                        objCmd.Parameters.Add("CONTACTNO_IN", OracleDbType.Int64).Value = strContactNo;
                        objCmd.Parameters.Add("DIRECTNO_IN", OracleDbType.Varchar2).Value = strDirect;
                        objCmd.Parameters.Add("LOCATION_IN", OracleDbType.Varchar2).Value = strLoc;
                        objCmd.Parameters.Add("LINE_IN", OracleDbType.Varchar2).Value = strLine.ToString();
                        objCmd.Parameters.Add("STATION_IN", OracleDbType.Varchar2).Value = strStation;
                        objCmd.Parameters.Add("SERVICECATALOG_IN", OracleDbType.Varchar2).Value = strServiceCatalog;
                        objCmd.Parameters.Add("CLASSIFICATION_IN", OracleDbType.Varchar2).Value = strClassification;
                        objCmd.Parameters.Add("DETAIL_IN", OracleDbType.Varchar2).Value = strDetail;
                        objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strSite;
                        //Vbansal 21/07/2010 Added the output parameter to return the Ticket ID
                        objCmd.Parameters.Add("TICKETID", OracleDbType.Int64).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        objCmd.CommandText = strSql;
                        objCmd.Connection = objConn;
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.ExecuteNonQuery();
                        currentSeqNo = Convert.ToInt32(objCmd.Parameters["TICKETID"].Value.ToString());
                        //return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                        result = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                        return result + "#" + currentSeqNo;

                    }
                    finally
                    {
                        if (objConn != null)
                        {
                            objConn.Close();
                        }
                    }
                }
                //MSharma 21/07/2010 Added the else block to update the Admin Call Status (strTicketType = 2 for Admin Call) <Start>
                else
                {
                    try
                    {
                        objConn.Open();
                        OracleCommand objCmd = new OracleCommand();

                        string strSql = "PKG_UTILITY.SPROC_ADD_TICKET_ADMIN";
                        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strCode;
                        objCmd.Parameters.Add("EXTNO_IN", OracleDbType.Int32).Value = strExt;
                        objCmd.Parameters.Add("CONTACTNO_IN", OracleDbType.Int64).Value = strContactNo;
                        objCmd.Parameters.Add("DIRECTNO_IN", OracleDbType.Varchar2).Value = strDirect;
                        objCmd.Parameters.Add("LOCATION_IN", OracleDbType.Varchar2).Value = strLoc;
                        objCmd.Parameters.Add("LINE_IN", OracleDbType.Varchar2).Value = strLine.ToString();
                        objCmd.Parameters.Add("STATION_IN", OracleDbType.Varchar2).Value = strStation;
                        objCmd.Parameters.Add("SERVICECATALOG_IN", OracleDbType.Varchar2).Value = strServiceCatalog;
                        objCmd.Parameters.Add("CLASSIFICATION_IN", OracleDbType.Varchar2).Value = strClassification;
                        objCmd.Parameters.Add("DETAIL_IN", OracleDbType.Varchar2).Value = strDetail;
                        objCmd.Parameters.Add("TICKETID", OracleDbType.Int64).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        objCmd.CommandText = strSql;
                        objCmd.Connection = objConn;
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.ExecuteNonQuery();
                        currentSeqNo = Convert.ToInt32(objCmd.Parameters["TICKETID"].Value.ToString());
                        result = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                        return result + "#" + currentSeqNo;
                    }
                    //MSharma 21/07/2010 <End>
                    finally
                    {
                        if (objConn != null)
                        {
                            objConn.Close();
                        }
                    }
                }
            }
        }

        public DataSet GetAllUtilityReqTickets(string Type)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;


                    strSql = "PKG_UTILITY.SPROC_UTILITY_REQ_HISTORY";
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Varchar2).Value = Type;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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

        public DataSet GetAllUtilityTickets(string Type)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;


                    strSql = "PKG_UTILITY.SPROC_GET_ALLTICKETS";
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = Type;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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

        public DataSet GetAllAdminTickets(string Type)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;
                    strSql = "PKG_UTILITY.SPROC_GET_ADMINTICKETS";
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = Type;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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

        public DataSet ViewTicketDetail(string TICKETID)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;


                    strSql = "PKG_UTILITY.SPROC_GET_TICKETDETAIL";
                    objCmd.Parameters.Add("TICKETID_IN", OracleDbType.Int32).Value = Convert.ToInt32(TICKETID);
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;


                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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

        public DataSet ViewAdminCallTicketDetail(string TICKETID)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;


                    strSql = "PKG_UTILITY.SPROC_GET_ADMINCALLTKTDETAIL";
                    objCmd.Parameters.Add("TICKETID_IN", OracleDbType.Int32).Value = Convert.ToInt32(TICKETID);
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;


                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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


        public int SubmitSolution(string strHandledBy, string strRemarks, string strStatus, string strTicketId, string strempcode)
        {
            string strConn = objConn.getConnectingString();

            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_UTILITY.SPROC_SOLVE_TICKET";

                    objCmd.Parameters.Add("HANDLEDBY_IN", OracleDbType.Varchar2).Value = strHandledBy;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
                    objCmd.Parameters.Add("TICKETID_IN", OracleDbType.Int32).Value = strTicketId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strempcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());

                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }

            }
        }

        public int SubmitSolutionAdminCall(string strHandledBy, string strRemarks, string strStatus, string strTicketId, string strempcode)
        {
            string strConn = objConn.getConnectingString();

            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_UTILITY.SPROC_SOLVE_ADMINCALLTICKET";

                    objCmd.Parameters.Add("HANDLEDBY_IN", OracleDbType.Varchar2).Value = strHandledBy;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
                    objCmd.Parameters.Add("TICKETID_IN", OracleDbType.Int32).Value = strTicketId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strempcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());

                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }

            }
        }

        public string SubmitUtilityReq(string strEmpCode, string strExtNo, string strContactNo,
            string strDirectNo, string strLocation, string strLine, string strStation,
            string strServiceCatalog, string strClassification, string strbudget, string strTitle,
            string strScope, string strAttachfile, string strappauth, string strSiteID)
        {
            string strConn = objConn.getConnectingString();

            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    String strErrMsg;
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_UTILITY.SPROC_UTILITY_NewRequest_SET";

                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("EXT_IN", OracleDbType.Int32).Value = strExtNo;
                    objCmd.Parameters.Add("CONTACT_IN", OracleDbType.Varchar2).Value = strContactNo;
                    objCmd.Parameters.Add("DIRECTNO_IN", OracleDbType.Varchar2).Value = strDirectNo;
                    objCmd.Parameters.Add("LOCATION_IN", OracleDbType.Int32).Value = strLocation;
                    objCmd.Parameters.Add("LINE_IN", OracleDbType.Varchar2).Value = strLine;
                    objCmd.Parameters.Add("STATION_IN", OracleDbType.Varchar2).Value = strStation;
                    objCmd.Parameters.Add("CATALOG_IN", OracleDbType.Int32).Value = strServiceCatalog;
                    objCmd.Parameters.Add("CLASSIFICATION_IN", OracleDbType.Int32).Value = strClassification;
                    objCmd.Parameters.Add("BUDGET_IN", OracleDbType.Varchar2).Value = strbudget;
                    objCmd.Parameters.Add("TITLE_IN", OracleDbType.Varchar2).Value = strTitle;
                    objCmd.Parameters.Add("SCOPE_IN", OracleDbType.Varchar2).Value = strScope;
                    objCmd.Parameters.Add("ATTACH_IN", OracleDbType.Varchar2).Value = strAttachfile;
                    objCmd.Parameters.Add("APPAUTH_IN", OracleDbType.Varchar2).Value = strappauth;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strSiteID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("REQUESTNO", OracleDbType.Int32, 11).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["REQUESTNO"].Value) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
        }

        public string EditUtReqBySctMgr(string strrequestid, string strStatus, string strRemark, string strDeptMgrCode, string strsctmgrcode)
        {
            string strConn = objConn.getConnectingString();

            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    String strErrMsg;
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_UTILITY.SPROC_EDITREQBYSECTMGR_SET";

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strrequestid;
                    objCmd.Parameters.Add("SCTMGRSTATUS_IN", OracleDbType.Int32).Value = strStatus;
                    objCmd.Parameters.Add("SCTMGRREMARK_IN", OracleDbType.Varchar2).Value = strRemark;
                    objCmd.Parameters.Add("DPTMGRCODE_IN", OracleDbType.Int32).Value = strDeptMgrCode;
                    objCmd.Parameters.Add("SCTMGRCODE_IN", OracleDbType.Int32).Value = strsctmgrcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
        }

        public string EditUtReqByDptMgr(string strrequestid, string strStatus, string strRemark)
        {
            string strConn = objConn.getConnectingString();

            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    String strErrMsg;
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_UTILITY.SPROC_EDITREQBYDEPTMGR_SET";

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strrequestid;
                    objCmd.Parameters.Add("DPTTMGRSTATUS_IN", OracleDbType.Int32).Value = strStatus;
                    objCmd.Parameters.Add("DPTMGRREMARK_IN", OracleDbType.Varchar2).Value = strRemark;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
        }

        public string EditUtPoReqByUt(string strrequestid, string strquotesubmitdate, string strnoofquotes, string strquoteprice, string strRemark, string strutstatus, string strpono, string strpodate, string strclosereamrk, string strutuserid)
        {
            string strConn = objConn.getConnectingString();

            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    String strErrMsg;
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_UTILITY.SPROC_EDITREQBYUTILITY_SET";

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = strrequestid;
                    objCmd.Parameters.Add("NOOFQUOTE_IN", OracleDbType.Varchar2).Value = strnoofquotes;
                    objCmd.Parameters.Add("QUOTEPRICE_IN", OracleDbType.Varchar2).Value = strquoteprice;
                    objCmd.Parameters.Add("QUOTESUBMITDATE_IN", OracleDbType.Varchar2).Value = strquotesubmitdate;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strRemark;
                    objCmd.Parameters.Add("UTSTATUS_IN", OracleDbType.Int32).Value = strutstatus;
                    objCmd.Parameters.Add("PONO_IN", OracleDbType.Varchar2).Value = strpono;
                    objCmd.Parameters.Add("PODATE_IN", OracleDbType.Varchar2).Value = strpodate;
                    objCmd.Parameters.Add("CLOSEREMARK_IN", OracleDbType.Varchar2).Value = strclosereamrk;
                    objCmd.Parameters.Add("UTUSERID_IN", OracleDbType.Varchar2).Value = strutuserid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
        }

        public DataSet ViewManageRequest(string strempcode)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;
                    strSql = "PKG_UTILITY.SPROC_UTALLREQUEST_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strempcode;
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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

        public DataSet ViewManageRequest1(string strempcode)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;
                    strSql = "PKG_UTILITY.SPROC_UTALLREQUEST_GET1";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strempcode;
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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

        public DataSet ApprovalOfUtReq(string strSupEmpCode)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;
                    strSql = "PKG_UTILITY.SPROC_MANAGEUTREQ_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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

        public DataSet GetApprovalAuthority(string strempcode)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;


                    strSql = "PKG_UTILITY.SPROC_APPROVALAUTHORITY_GET";
                    objCmd.Parameters.Add("CODE_IN", OracleDbType.Int32).Value = strempcode;
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;


                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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

        public DataSet GetAminEmailId()
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    DataSet ds = new DataSet();
                    strSql = "PKG_UTILITY.SPROC_ADMINEMAILID_GET";
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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

        public string EditUtilityReq(string strreqid, string strEmpCode, string strExtNo, string strContactNo, string strDirectNo, string strLocation, string strLine, string strStation, string strServiceCatalog, string strClassification, string strbudget, string strTitle, string strScope, string strName, string strappauth)
        {
            string strConn = objConn.getConnectingString();

            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    String strErrMsg;
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_UTILITY.SPROC_EDITNEWUTREQ_SET";

                    objCmd.Parameters.Add("REQID_IN", OracleDbType.Int32).Value = strreqid;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("EXT_IN", OracleDbType.Int32).Value = strExtNo;
                    objCmd.Parameters.Add("CONTACT_IN", OracleDbType.Varchar2).Value = strContactNo;
                    objCmd.Parameters.Add("DIRECTNO_IN", OracleDbType.Varchar2).Value = strDirectNo;
                    objCmd.Parameters.Add("LOCATION_IN", OracleDbType.Int32).Value = strLocation;
                    objCmd.Parameters.Add("LINE_IN", OracleDbType.Varchar2).Value = strLine;
                    objCmd.Parameters.Add("STATION_IN", OracleDbType.Varchar2).Value = strStation;
                    objCmd.Parameters.Add("CATALOG_IN", OracleDbType.Int32).Value = strServiceCatalog;
                    objCmd.Parameters.Add("CLASSIFICATION_IN", OracleDbType.Int32).Value = strClassification;
                    objCmd.Parameters.Add("BUDGET_IN", OracleDbType.Varchar2).Value = strbudget;
                    objCmd.Parameters.Add("TITLE_IN", OracleDbType.Varchar2).Value = strTitle;
                    objCmd.Parameters.Add("SCOPE_IN", OracleDbType.Varchar2).Value = strScope;
                    objCmd.Parameters.Add("ATTACH_IN", OracleDbType.Varchar2).Value = strName;
                    objCmd.Parameters.Add("APPAUTH_IN", OracleDbType.Varchar2).Value = strappauth;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
        }

        public string CancelUtReq(string strcancelremark, string strreqid)
        {
            string strConn = objConn.getConnectingString();

            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    String strErrMsg;
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_UTILITY.SPROC_CANCELNEWUTREQ_SET";

                    objCmd.Parameters.Add("REQID_IN", OracleDbType.Int32).Value = strreqid;
                    objCmd.Parameters.Add("CANCEL_IN", OracleDbType.Varchar2).Value = strcancelremark;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
        }

        public DataSet UtilityAppHistory(string strempcode)
        {
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;
                    strSql = "PKG_UTILITY.SPROC_UTILITYAPPHISTORY";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strempcode;
                    objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
                    return ds;
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
        /// GET RESPONSIBLE USER DETAIL TO RESOLVE UTILITY CALL 
        /// </summary>
        /// <param name="EmpDeptCode"></param>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        public DataTable GetResponsibleUserDetails(string EmpDeptCode, string LocationCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_UTILITY.SPROC_USERDETAIL_GET";
            oCmd.Parameters.Add("DEPTCODE_IN", OracleDbType.Varchar2).Value = EmpDeptCode;
            oCmd.Parameters.Add("LOCATIONCODE_IN", OracleDbType.Varchar2).Value = LocationCode;
            oCmd.Parameters.Add("CUR_USERDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET UTILITY REPAIR CALL DATA
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="TillDate"></param>
        /// <param name="TicketID"></param>
        /// <param name="Status"></param>
        /// <param name="Department"></param>
        /// <param name="Section"></param>
        /// <param name="Catalog"></param>
        /// <returns></returns>
        public DataTable GetUtilityCallList(string FromDate, string TillDate, string TicketID, string Status,
                                            string Department, string Section, string Catalog, string Site)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_UTILITY.SPROC_UTILITYTICKETS_GET";
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = TillDate;
            oCmd.Parameters.Add("CATALOGID_IN", OracleDbType.Varchar2).Value = Catalog;
            oCmd.Parameters.Add("TICKETID_IN", OracleDbType.Varchar2).Value = TicketID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = Status;
            oCmd.Parameters.Add("DEPTCODE_IN", OracleDbType.Varchar2).Value = Department;
            oCmd.Parameters.Add("SECTCODE_IN", OracleDbType.Varchar2).Value = Section;
            oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = Site;
            oCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public UtilityDocumentInfo GetUtilityDocumentInfo(long docId)
        {
            UtilityDocumentInfo documentInfo = new UtilityDocumentInfo();
            string strattachment = string.Empty;
            string strConn = objConn.getConnectingString();
            OracleConnection objCn = new OracleConnection();
            OracleCommand objCmd = new OracleCommand("PKG_UTILITY.SPROC_REQUESTDETAILS_GET", objCn);
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;

            objCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int64).Value = docId;

            using (objCn)
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleDataReader objReader = objCmd.ExecuteReader();
                    if (objReader.HasRows)
                    {
                        while (objReader.Read())
                        {
                            strattachment = objReader["ATTACHMENT"].ToString();
                        }
                    }
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            string filePath = serverpath.getFileUploadPath() + @"\Utility\" + strattachment;
            String sFullPath = String.Empty;
            sFullPath = filePath;
            if (sFullPath != null && sFullPath.Length > 4)
            {
                if (sFullPath.Contains("\\"))
                {
                    try
                    {
                        FileInfo _fileInfo = new FileInfo(sFullPath);
                        if (_fileInfo.Exists)
                        {
                            String sContentType = "";
                            // Determine the content type
                            switch (_fileInfo.Extension.ToLower())
                            {
                                case ".dwf":
                                    sContentType = "Application/x-dwf";
                                    break;
                                case ".pdf":
                                    sContentType = "Application/pdf";
                                    break;
                                case ".txt":
                                    sContentType = "Application/txt";
                                    break;
                                case ".doc":
                                    sContentType = "Application/vnd.ms-word";
                                    break;
                                case ".ppt":
                                case ".pps":
                                    sContentType = "Application/vnd.ms-powerpoint";
                                    break;
                                case ".xls":
                                    sContentType = "Application/vnd.ms-excel";
                                    break;
                                default:
                                    // Catch-all content type, let the browser figure it out
                                    sContentType = "Application/octet-stream";
                                    break;
                            }
                            documentInfo = new UtilityDocumentInfo()
                            {
                                PhysicalPath = sFullPath,
                                ContentType = sContentType,
                                FileDownloadName = Path.GetFileName(sFullPath)
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return documentInfo;
        }

        public UtilityRequestViewModel GetContactInfo(long empCode)
        {
            UtilityRequestViewModel model = new UtilityRequestViewModel();
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_SISHELPDESK.SPROC_CONTACT";
            oCmd.Parameters.Add("CODE_IN", OracleDbType.Int64).Value = empCode;
            oCmd.Parameters.Add("CUR_HD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            if (dt != null && dt.Rows.Count > 0)
            {
                model.ExtensionNo = dt.Rows[0].Field<string>("EXTN");
                model.PhoneNo = dt.Rows[0].Field<string>("MOBILE");
            }
            return model;
        }

        public UtilityRequestViewModel GetSelectedApprovalAutority(long empCode)
        {
            UtilityRequestViewModel model = new UtilityRequestViewModel();
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_UTILITY.SPROC_USERDESIGNATION_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = empCode;
            oCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            if (dt != null && dt.Rows.Count > 0)
            {
                model.ApprovalAuthorityCode = dt.Rows[0].Field<long>("ADEMPCODE").ToString();
            }
            return model;
        }

        public Employee_Details GetEmployeeDetails(long empCode)
        {
            Employee_Details model = new Employee_Details();
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "SPROC_USER_DETAILS";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = empCode;
            oCmd.Parameters.Add("CSR_USERDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            if (dt != null && dt.Rows.Count > 0)
            {
                model._EmailId = dt.Rows[0].Field<string>("emailid");
                model._EName = dt.Rows[0].Field<string>("name");
                model._ECode = dt.Rows[0].Field<long>("adempcode");
            }
            return model;
        }

        public void SendMail(string mailTo, string mailSubject, string mailBody)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = mailTo;
            sendMail.MailSubject = mailSubject;
            sendMail.MailBody = mailBody;
            try
            {
                sendMail.Send();
            }
            catch (Exception){}
        }

        public DataTable GetUtilityRequestDetail(long requestId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_UTILITY.SPROC_REQUESTDETAILS_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int64).Value = requestId;
            oCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }

        public DataTable GetMailIds(long empCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_UTILITY.SPROC_MAILID_GET";
            oCmd.Parameters.Add("CODE_IN", OracleDbType.Int64).Value = empCode;
            oCmd.Parameters.Add("CUR_UTILITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }
    }
}
