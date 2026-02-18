using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Services
{
    public class BusRoute:IBusRoute
    {
       
        #region "Local Variables"
        //
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

        public BusRoute(IDataManagement _oDataMgmt, IConnectionString _objConn, ICommonFunctions objCommon)
        {
            oDataMgmt = _oDataMgmt;
            objConn = _objConn;
            _objCommon = objCommon;
        }
        public DataSet GetBusRoute(string strSiteID)
        {
            ds = new DataSet();
           
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
                    strSql = "PKG_ADBUSROUTE.SPROC_AD_BUSROUTE_GET";
                    objCmd.Parameters.Add("SYSITEID_IN", OracleDbType.Int32).Value = strSiteID;
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

        public DataSet GetBusRouteFromStop(string strStopID)
        {
            ds = new DataSet();
           
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
                    strSql = "PKG_ADBUSROUTE.SPROC_AD_BUSROUTEFROMSTOP_GET";
                    objCmd.Parameters.Add("BusStopID_IN", OracleDbType.Int32).Value = strStopID;
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

        public DataSet GetBusStop(string strBusRouteId)
        {
            ds = new DataSet();
            
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
                    strSql = "PKG_ADBUSROUTE.SPROC_AD_BUSSTOP_GET";
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("BUSROUTEID_IN", OracleDbType.Int32).Value = strBusRouteId;
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

        public string GetMonthlyPay(string strStopId)
        {
            
            string strConn = objConn.getConnectingString();
            string strmonthlypay;
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    string strSql = "PKG_ADBUSROUTE.SPROC_MONTHLYPAY_GET";
                    objCmd.Parameters.Add("BUSSTOPID_IN", OracleDbType.Int32).Value = strStopId;
                    objCmd.Parameters.Add("PAY_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strmonthlypay = Convert.ToString(objCmd.Parameters["PAY_OUT"].Value);
                    return strmonthlypay;
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

        public string SubmitRouteRequest(string strEmpCode, string strrequest, string strcurrentroute, string strcurrentstop, string strnewroute, string strnewstop, string straddress, string strremark, string strdate, string strpay)
        {
            
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                String strErrMsg;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_ADBUSROUTE.SPROC_ROUTECHANGE_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("REQUEST_IN", OracleDbType.Int32).Value = strrequest;
                    objCmd.Parameters.Add("CURROUTE_IN", OracleDbType.Varchar2).Value = strcurrentroute;
                    objCmd.Parameters.Add("CURSTOP_IN", OracleDbType.Varchar2).Value = strcurrentstop;
                    objCmd.Parameters.Add("NEWROUTE_IN", OracleDbType.Int32).Value = strnewroute;
                    objCmd.Parameters.Add("NEWSTOP_IN", OracleDbType.Int32).Value = strnewstop;
                    objCmd.Parameters.Add("ADDRESS_IN", OracleDbType.Varchar2).Value = straddress;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
                    objCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = strdate;
                    objCmd.Parameters.Add("PAY_IN", OracleDbType.Int32).Value = strpay;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public DataSet EditRouteChange(string strRequestId)
        {
            ds = new DataSet();
            
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
                    strSql = "PKG_ADBUSROUTE.SPROC_AD_ROUTEREQUESTBYID_GET";
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestId;
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

        public string UpdateRouteRequest(string strRequestId, string strrequest, string strcurrentroute, string strcurrentstop, string strnewroute, string strnewstop, string straddress, string strremark, string strdate, string strpay)
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

                    string strSql = "PKG_ADBUSROUTE.SPROC_UPDATEROUTECHANGE_SET";

                    objCmd.Parameters.Add("ROUTEREQUESTID_IN", OracleDbType.Int32).Value = strRequestId;
                    objCmd.Parameters.Add("REQUEST_IN", OracleDbType.Int32).Value = strrequest;
                    objCmd.Parameters.Add("CURROUTE_IN", OracleDbType.Varchar2).Value = strcurrentroute;
                    objCmd.Parameters.Add("CURSTOP_IN", OracleDbType.Varchar2).Value = strcurrentstop;
                    objCmd.Parameters.Add("NEWROUTE_IN", OracleDbType.Int32).Value = strnewroute;
                    objCmd.Parameters.Add("NEWSTOP_IN", OracleDbType.Int32).Value = strnewstop;
                    objCmd.Parameters.Add("ADDRESS_IN", OracleDbType.Varchar2).Value = straddress;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
                    objCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = strdate;
                    objCmd.Parameters.Add("PAY_IN", OracleDbType.Varchar2).Value = strpay;
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

        public DataSet ManageRouteChange(string strEmpCode)
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
                    strSql = "PKG_ADBUSROUTE.SPROC_ADMANAGEROUTEREQ_GET";
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
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

        public DataSet ManageRouteChange1(string strEmpCode)
        {
            ds = new DataSet();
            
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
                    strSql = "PKG_ADBUSROUTE.SPROC_ADMANAGEROUTEREQ_GET1";
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
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

        public DataSet GetRequestForCancel(string strRequestId)
        {
            ds = new DataSet();
            
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
                    strSql = "PKG_ADBUSROUTE.SPROC_ROUTEREQCANCEL_GET";
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestId;
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

        public string CancelRouteChangeRequest(string strRequestId, string strCancelRemark)
        {
            
            string strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                String strErrMsg;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_ADBUSROUTE.SPROC_CANCELREQUEST_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestId;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strCancelRemark;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public DataSet ADGetBusStop()
        {
            ds = new DataSet();
            
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
                    strSql = "PKG_ADBUSROUTE.SPROC_BUSSTOPADMIN_GET";
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

        public string UpdateRequestByAdmin(string strRequestId, string stradempcode, string strstatus, string stradminremark)
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

                    string strSql = "PKG_ADBUSROUTE.SPROC_UPDATEREQBYADMIN_SET";

                    objCmd.Parameters.Add("ROUTEREQUESTID_IN", OracleDbType.Int32).Value = strRequestId;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = stradempcode;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strstatus;
                    objCmd.Parameters.Add("ADREMARK_IN", OracleDbType.Varchar2).Value = stradminremark;
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

        public DataSet GetAminEmailId()
        {
            ds = new DataSet();
            
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
                    strSql = "PKG_ADBUSROUTE.SPROC_ADMINEMAILID_GET";
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

        public DataSet GetRouteChngReq(string strEmpCode, string strRequest, string strDateFrom, string strDateTo, string strRoute,
            string strStop, string strStatus, string strKiId, string strSiteID)
        {
            ds = new DataSet();
            
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
                    strSql = "PKG_ADBUSROUTE.SPROC_BUSROUTEREQADMIN_GET";
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Int32).Value = strKiId;
                    if (strEmpCode != "")
                    {
                        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    }
                    if (strRequest != "0")
                    {
                        objCmd.Parameters.Add("REQUEST_IN", OracleDbType.Varchar2).Value = strRequest;
                    }
                    if (strDateFrom != "")
                    {
                        objCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = strDateFrom;
                    }
                    if (strDateTo != "")
                    {
                        objCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = strDateTo;
                    }
                    if (strRoute != "0")
                    {
                        objCmd.Parameters.Add("ROUTE_IN", OracleDbType.Varchar2).Value = strRoute;
                    }
                    if (strStop != "0")
                    {
                        objCmd.Parameters.Add("STOP_IN", OracleDbType.Varchar2).Value = strStop;
                    }
                    if (strStatus != "")
                    {
                        objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                    }

                    objCmd.Parameters.Add("SYSITEID_IN", OracleDbType.Varchar2).Value = strSiteID;
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

        public DataTable EditEvalParam(string evalid)
        {
            OracleCommand cmd = new OracleCommand();
            DataTable dt = new DataTable();
            
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_EDIT_BUSROUTE";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("BUSROUTE_ID", OracleDbType.Int32).Value = evalid;
            cmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(cmd);
            return dt;
        }

        public DataTable EditBusCharges(string evalid)
        {
            OracleCommand cmd = new OracleCommand();
            DataTable dt = new DataTable();
            
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_EDIT_BUSCHARGES";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("BUSCHARGE_ID", OracleDbType.Int32).Value = evalid;
            cmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(cmd);
            return dt;
        }

        public DataTable EditStopDetails(string evalid)
        {
            OracleCommand cmd = new OracleCommand();
            DataTable dt = new DataTable();
            
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_EDIT_BUSSTOP";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("BUSSTOP_ID", OracleDbType.Varchar2).Value = evalid;
            cmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(cmd);
            return dt;
        }

        public DataTable EditVendorDetails(string evalid)
        {
            OracleCommand cmd = new OracleCommand();
            DataTable dt = new DataTable();
            
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_EDIT_VENDOR";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("VENDORID_IN", OracleDbType.Varchar2).Value = evalid;
            cmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(cmd);
            return dt;
        }

        public DataTable GetChargeDetails()
        {
            OracleCommand cmd = new OracleCommand();
            DataTable dt = new DataTable();
            
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_CHARGEDETAILS";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(cmd);
            return dt;
        }

        public DataTable GetCharges(string chargeid)
        {
            OracleCommand cmd = new OracleCommand();
            DataTable dt = new DataTable();
            
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_CHARGES";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("CHARGECODE_IN", OracleDbType.Varchar2).Value = chargeid;
            cmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(cmd);
            return dt;
        }

        public string AddEditBusRoute(string userID, string id, string routecode, string desc, string status, string strSiteID)
        {
            OracleCommand cmd = new OracleCommand();
            string strErrMsg = string.Empty;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_ADDEDITBUSROUTE";
            cmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userID;
            cmd.Parameters.Add("ROUTEID_IN", OracleDbType.Varchar2).Value = id;
            cmd.Parameters.Add("ROUTECODE_IN", OracleDbType.Varchar2).Value = routecode;
            cmd.Parameters.Add("DESC_IN", OracleDbType.Varchar2).Value = desc;
            cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            cmd.Parameters.Add("SYSITEID_IN", OracleDbType.Int32).Value = strSiteID;
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            
            cmd.BindByName = true;
            oDataMgmt.ExecuteQuery(cmd);
            strErrMsg = Convert.ToString(cmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(cmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public string AddEditVendorDetails(string userID, string id, string VendorName, string emailid, string contactpersname, string contactno, string status, string strsiteid, string strrkm)
        {
            OracleCommand cmd = new OracleCommand();
            string strErrMsg = string.Empty;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_ADDEDITVENDORDETAILS";
            cmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userID;
            cmd.Parameters.Add("VEHICLEVENDORID_IN", OracleDbType.Varchar2).Value = id;
            cmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VendorName;
            cmd.Parameters.Add("MAILID_IN", OracleDbType.Varchar2).Value = emailid;
            cmd.Parameters.Add("CNTPERSNAME", OracleDbType.Varchar2).Value = contactpersname;
            cmd.Parameters.Add("CONTACTNO_IN", OracleDbType.Varchar2).Value = contactno;
            cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            cmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsiteid;
            cmd.Parameters.Add("RKM_IN", OracleDbType.Varchar2).Value = strrkm;
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            
            cmd.BindByName = true;
            oDataMgmt.ExecuteQuery(cmd);
            strErrMsg = Convert.ToString(cmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(cmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public string AddEditBusRouteCharges(string userID, string id, string codedesc, string mindist, string maxdist, string cost)
        {
            OracleCommand cmd = new OracleCommand();
            string strErrMsg = string.Empty;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_ADDEDITBUSROUTECHARGE";
            cmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userID;
            cmd.Parameters.Add("CHARGEID_IN", OracleDbType.Varchar2).Value = id;
            cmd.Parameters.Add("CHARGECODEDESC_IN", OracleDbType.Varchar2).Value = codedesc;
            cmd.Parameters.Add("MINDIST_IN", OracleDbType.Varchar2).Value = mindist;
            cmd.Parameters.Add("MAXDIST_IN", OracleDbType.Varchar2).Value = maxdist;
            cmd.Parameters.Add("COST_IN", OracleDbType.Varchar2).Value = cost;
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            
            cmd.BindByName = true;
            oDataMgmt.ExecuteQuery(cmd);
            strErrMsg = Convert.ToString(cmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(cmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public string AddEditBusStop(string userID, string routeID, string id, string routecode, string descrip, string charges, string status, string costcode, string strgarage)
        {
            OracleCommand cmd = new OracleCommand();
            string strErrMsg = string.Empty;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_ADBUSROUTE.SPROC_ADDEDITBUSSTOP";
            cmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userID;
            cmd.Parameters.Add("ROUTEID_IN", OracleDbType.Varchar2).Value = routeID;
            cmd.Parameters.Add("STOPID_IN", OracleDbType.Varchar2).Value = id;
            cmd.Parameters.Add("STOPCODE_IN", OracleDbType.Varchar2).Value = routecode;
            cmd.Parameters.Add("DESC_IN", OracleDbType.Varchar2).Value = descrip;
            cmd.Parameters.Add("CHARGES_IN", OracleDbType.Int32).Value = charges;
            cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            cmd.Parameters.Add("COSTCODE_IN", OracleDbType.Varchar2).Value = costcode;
            cmd.Parameters.Add("GARAGE_IN", OracleDbType.Varchar2).Value = strgarage;
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            
            cmd.BindByName = true;
            oDataMgmt.ExecuteQuery(cmd);
            strErrMsg = Convert.ToString(cmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(cmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }
        public DataSet GetBusStopList(string strSiteId)
        {
            ds = new DataSet();
            
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
                    strSql = "PKG_ADBUSROUTE.SPROC_AD_BUSSTOPLIST_GET";
                    objCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SiteID_IN", OracleDbType.Int32).Value = strSiteId;
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

    }
}
