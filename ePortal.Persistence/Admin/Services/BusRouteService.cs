using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Routing;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Persistence.Admin.Services
{
    public class BusRouteService : IBusRouteService
    {
        #region "Local Variables"
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        private readonly ICommonFunctions _common;
        //private readonly CommonFunctions _common = new CommonFunctions();
        OracleCommand oCmd;
        DataSet ds;
        DataTable Dt;
        string strConn;
        string strErrMsg = string.Empty;
        OracleConnection objConn;
        #endregion
        public BusRouteService(IDataManagement _oDataMgmt, IConnectionString _objCnStr, ICommonFunctions common)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
            _common = common;
        }

        public IEnumerable<SiteModel> SelectSysite()
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_SITE_GET";
            oCmd.Parameters.Add("CUR_SITE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds.Tables[0].AsEnumerable()
                .Select(r => new SiteModel
                {
                    SiteId = r["SYSITEID"].ToString(),
                    SiteName = r["SITE"].ToString()
                })
                .ToList();
        }


        public DataSet GetBusStopsListAsync(string siteId)
        {

            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_BUSSTOPLIST_GET";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SiteID_IN", OracleDbType.Varchar2).Value = siteId;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public IEnumerable<BusRouteModel> GetCurrentRouteAsync(string strStopID)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_BUSSTOPLIST_GET";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SiteID_IN", OracleDbType.Varchar2).Value = strStopID;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds.Tables[0].AsEnumerable()
                .Select(r => new BusRouteModel
                {
                    ADBUSROUTEID = r["ADBUSROUTEID"].ToString(),
                    DESCRIP = r["DESCRIP"].ToString()
                })
                .ToList();
        }

        public IEnumerable<BusStopModel> GetNewStopAsync(string siteId)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_BUSROUTEFROMSTOP_GET";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("BusStopID_IN", OracleDbType.Varchar2).Value = siteId;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds.Tables[0].AsEnumerable()
                .Select(r => new BusStopModel
                {
                    ADBUSSTOPID = r["ADBUSSTOPID"].ToString(),
                    DESCRIP = r["DESCRIP"].ToString()
                })
                .ToList();
        }

        public Task<decimal> GetMonthlyPay()
        {
            throw new NotImplementedException();
        }

        public string SubmitRouteRequest(string strEmpCode, string strrequest, string strcurrentroute, string strcurrentstop, string strnewroute, string strnewstop, string straddress, string strremark, string strdate, string strpay)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    string strSql = "PKG_ADBUSROUTE.SPROC_ROUTECHANGE_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("REQUEST_IN", OracleDbType.Int32).Value = strrequest;
                    objCmd.Parameters.Add("CURROUTE_IN", OracleDbType.Int32).Value = strcurrentroute;
                    objCmd.Parameters.Add("CURSTOP_IN", OracleDbType.Int32).Value = strcurrentstop;
                    objCmd.Parameters.Add("NEWROUTE_IN", OracleDbType.Int32).Value = strnewroute;
                    objCmd.Parameters.Add("NEWSTOP_IN", OracleDbType.Int32).Value = strnewstop;
                    objCmd.Parameters.Add("ADDRESS_IN", OracleDbType.Varchar2).Value = straddress;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
                    objCmd.Parameters.Add("DATE_IN", OracleDbType.Date).Value = DateTime.ParseExact(strdate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    objCmd.Parameters.Add("PAY_IN", OracleDbType.Int32).Value = strpay;
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
                catch (Exception ex)
                {
                    throw ex;
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

        public IEnumerable<BusRouteModel> GetBusRouteFromStop(string strStopID)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_BUSROUTEFROMSTOP_GET";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("BusStopID_IN", OracleDbType.Varchar2).Value = strStopID;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds.Tables[0].AsEnumerable()
                .Select(r => new BusRouteModel
                {
                    ADBUSROUTEID = r["ADBUSROUTEID"].ToString(),
                    DESCRIP = r["DESCRIP"].ToString()
                })
                .ToList();
        }

        public DataSet GetBusRouteFromStop1(string strStopID)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_BUSROUTEFROMSTOP_GET";
            oCmd.Parameters.Add("BusStopID_IN", OracleDbType.Int32).Value = strStopID;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public DataSet GetBusRoute(string strSiteID)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_BUSROUTE_GET";
            oCmd.Parameters.Add("SYSITEID_IN", OracleDbType.Int32).Value = strSiteID;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public DataSet GetBusStop(string strBusRouteId)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_BUSSTOP_GET";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.Int32).Value = strBusRouteId;
            oCmd.Parameters.Add("BUSROUTEID_IN", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public DataSet EditRouteChange(int strRequestId)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_ROUTEREQUESTBYID_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestId;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output; 
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public string UpdateRouteRequest(string strrequestId, string strrequest, string strcurrentroute, string strcurrentstop, string strnewroute, string strnewstop, string straddress, string strremark, string strdate, string strpay)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    string strSql = "PKG_ADBUSROUTE.SPROC_UPDATEROUTECHANGE_SET";
                    objCmd.Parameters.Add("ROUTEREQUESTID_IN", OracleDbType.Int32).Value = strrequestId;
                    objCmd.Parameters.Add("REQUEST_IN", OracleDbType.Int32).Value = strrequest;
                    objCmd.Parameters.Add("CURROUTE_IN", OracleDbType.Int32).Value = strcurrentroute;
                    objCmd.Parameters.Add("CURSTOP_IN", OracleDbType.Int32).Value = strcurrentstop;
                    objCmd.Parameters.Add("NEWROUTE_IN", OracleDbType.Int32).Value = strnewroute;
                    objCmd.Parameters.Add("NEWSTOP_IN", OracleDbType.Int32).Value = strnewstop;
                    objCmd.Parameters.Add("ADDRESS_IN", OracleDbType.Varchar2).Value = straddress;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
                    objCmd.Parameters.Add("DATE_IN", OracleDbType.Date).Value = DateTime.ParseExact(strdate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    objCmd.Parameters.Add("PAY_IN", OracleDbType.Int32).Value = strpay;
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
                catch (Exception ex)
                {
                    throw ex;
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
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_ADMANAGEROUTEREQ_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public DataSet GetRequestForCancel(string strRequestId)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_ROUTEREQCANCEL_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestId;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public string CancelRouteChangeRequest(string strRequestId, string strCancelRemark)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    string strSql = "PKG_ADBUSROUTE.SPROC_CANCELREQUEST_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestId;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strCancelRemark;
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
                catch (Exception ex)
                {
                    throw ex;
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

        public string UpdateRequestByAdmin(string strRequestId, string stradempcode, string strstatus, string stradminremark)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
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
                catch (Exception ex)
                {
                    throw ex;
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

        public DataSet GetAdminEmailId()
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_ADMINEMAILID_GET";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public DataSet GetRouteChngReq(string strEmpCode, string strRequest, string strDateFrom, string strDateTo, string strRoute, string strStop, string strStatus, string strKiId, string strSiteID)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_BUSROUTEREQADMIN_GET";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("KIID_IN", OracleDbType.Int32).Value = strKiId;
            if (strEmpCode != "")
            {
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            }
            if (strRequest != "0")
            {
                oCmd.Parameters.Add("REQUEST_IN", OracleDbType.Varchar2).Value = strRequest;
            }
            if (strDateFrom != "")
            {
                oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = strDateFrom;
            }
            if (strDateTo != "")
            {
                oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = strDateTo;
            }
            if (strRoute != "0")
            {
                oCmd.Parameters.Add("ROUTE_IN", OracleDbType.Varchar2).Value = strRoute;
            }
            if (strStop != "0")
            {
                oCmd.Parameters.Add("STOP_IN", OracleDbType.Varchar2).Value = strStop;
            }
            if (strStatus != "")
            {
                oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            }

            oCmd.Parameters.Add("SYSITEID_IN", OracleDbType.Varchar2).Value = strSiteID;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public DataTable EditEvalParam(string evalid)
        {
            oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_EDIT_BUSROUTE";
            oCmd.Parameters.Add("BUSROUTE_ID", OracleDbType.Int32).Value = evalid;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }

        public DataTable EditBusCharges(string evalid)
        {
            oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_EDIT_BUSCHARGES";
            oCmd.Parameters.Add("BUSCHARGE_ID", OracleDbType.Int32).Value = evalid;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }

        public DataTable EditStopDetails(string evalid)
        {
            oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_EDIT_BUSSTOP";
            oCmd.Parameters.Add("BUSSTOP_ID", OracleDbType.Int32).Value = evalid;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }

        public DataTable EditVendorDetails(string evalid)
        {
            oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_EDIT_VENDOR";
            oCmd.Parameters.Add("VENDORID_IN", OracleDbType.Int32).Value = evalid;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }

        public DataTable GetChargeDetails()
        {
            oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_CHARGEDETAILS";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }

        public DataTable GetCharges(string chargeid)
        {
            oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_CHARGES";
            oCmd.Parameters.Add("CHARGECODE_IN", OracleDbType.Varchar2).Value = chargeid;
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }

        public string AddEditBusRoute(string userID, string id, string routecode, string desc, string status, string strSiteID)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_ADBUSROUTE.SPROC_ADDEDITBUSROUTE";
                    objCmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userID;
                    objCmd.Parameters.Add("ROUTEID_IN", OracleDbType.Varchar2).Value = id;
                    objCmd.Parameters.Add("ROUTECODE_IN", OracleDbType.Varchar2).Value = routecode;
                    objCmd.Parameters.Add("DESC_IN", OracleDbType.Varchar2).Value = desc;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("SYSITEID_IN", OracleDbType.Int32).Value = strSiteID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
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

        public string AddEditVendorDetails(string userID, string id, string VendorName, string emailid, string contactpersname, string contactno, string status, string strsiteid, string strrkm)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_ADBUSROUTE.SPROC_ADDEDITVENDORDETAILS";
                    objCmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userID;
                    objCmd.Parameters.Add("VEHICLEVENDORID_IN", OracleDbType.Varchar2).Value = id;
                    objCmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VendorName;
                    objCmd.Parameters.Add("MAILID_IN", OracleDbType.Varchar2).Value = emailid;
                    objCmd.Parameters.Add("CNTPERSNAME", OracleDbType.Varchar2).Value = contactpersname;
                    objCmd.Parameters.Add("CONTACTNO_IN", OracleDbType.Varchar2).Value = contactno;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsiteid;
                    objCmd.Parameters.Add("RKM_IN", OracleDbType.Varchar2).Value = strrkm;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
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

        public string AddEditBusRouteCharges(string userID, string id, string codedesc, string mindist, string maxdist, string cost)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_ADBUSROUTE.SPROC_ADDEDITBUSROUTECHARGE";
                    objCmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userID;
                    objCmd.Parameters.Add("CHARGEID_IN", OracleDbType.Varchar2).Value = id;
                    objCmd.Parameters.Add("CHARGECODEDESC_IN", OracleDbType.Varchar2).Value = codedesc;
                    objCmd.Parameters.Add("MINDIST_IN", OracleDbType.Varchar2).Value = mindist;
                    objCmd.Parameters.Add("MAXDIST_IN", OracleDbType.Varchar2).Value = maxdist;
                    objCmd.Parameters.Add("COST_IN", OracleDbType.Varchar2).Value = cost;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
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

        public string AddEditBusStop(string userID, string routeID, string id, string routecode, string descrip, string charges, string status, string costcode, string strgarage)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_ADBUSROUTE.SPROC_ADDEDITBUSSTOP";
                    objCmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userID;
                    objCmd.Parameters.Add("ROUTEID_IN", OracleDbType.Varchar2).Value = routeID;
                    objCmd.Parameters.Add("STOPID_IN", OracleDbType.Varchar2).Value = id;
                    objCmd.Parameters.Add("STOPCODE_IN", OracleDbType.Varchar2).Value = routecode;
                    objCmd.Parameters.Add("DESC_IN", OracleDbType.Varchar2).Value = descrip;
                    objCmd.Parameters.Add("CHARGES_IN", OracleDbType.Int32).Value = charges;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("COSTCODE_IN", OracleDbType.Varchar2).Value = costcode;
                    objCmd.Parameters.Add("GARAGE_IN", OracleDbType.Varchar2).Value = strgarage;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
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

        public DataSet GetBusStopList(string strSiteId)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_BUSSTOPLIST_GET";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SiteID_IN", OracleDbType.Varchar2).Value = strSiteId;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        IEnumerable<BusStopModel> IBusRouteService.GetBusStopsListAsync(string siteId)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_AD_BUSSTOPLIST_GET";
            oCmd.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SiteID_IN", OracleDbType.Varchar2).Value = siteId;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds.Tables[0].AsEnumerable()
                .Select(r => new BusStopModel
                {
                    ADBUSSTOPID = r["ADBUSSTOPID"].ToString(),
                    DESCRIP = r["DESCRIP"].ToString()
                })
                .ToList();
        }

        public string GetMonthlyPayFromDatabase(int strStopId)
        {
            //string strmonthlypay;
            //oCmd = new OracleCommand();
            //ds = new DataSet();
            //oCmd.CommandText = "PKG_ADBUSROUTE.SPROC_MONTHLYPAY_GET";
            //oCmd.Parameters.Add("BUSSTOPID_IN", OracleDbType.Int32).Value = strStopId;
            //oCmd.Parameters.Add("PAY_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            //oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            //oCmd.BindByName = true;
            // oCmd.ExecuteNonQuery();
            //strmonthlypay = Convert.ToString(oCmd.Parameters["PAY_OUT"].Value);
            //return strmonthlypay;

            string strConn = objCnStr.getConnectingString();
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


        public RouteChangeRequest GetRouteChangeRequest(long requestId = 24365)
        {
            string strConn = objCnStr.getConnectingString(); // Assuming this is fetching your connection string.

            using (OracleConnection objConn = new OracleConnection(strConn)) // Open connection here.
            {
                try
                {
                    using (var command = new OracleCommand("PKG_ADBUSROUTE.SPROC_ROUTEREQCANCEL_GET", objConn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters to the command
                        command.Parameters.Add("CUR_BUSROUTE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        command.Parameters.Add("REQUESTID_IN", OracleDbType.Int64).Value = requestId;

                        // Open the connection
                        objConn.Open();

                        using (var reader = command.ExecuteReader()) // Execute and get the reader
                        {
                            if (reader.HasRows) // Check if there are rows returned
                            {
                                reader.Read(); // Read the first row

                                var routeChangeRequest = new RouteChangeRequest
                                {
                                    RequestId = requestId,
                                    EmployeeName = reader["EMPNAME"].ToString(),
                                    Request = reader["REQUEST"].ToString(),
                                    CurrentRoute = reader["CURRROUTE"].ToString(),
                                    CurrentStop = reader["CURRSTOP"].ToString(),
                                    NewRoute = reader["NEWROUTE"].ToString(),
                                    NewStop = reader["NEWSTOP"].ToString(),
                                    UsageStartDate = reader["USTARTDATE"].ToString(),
                                    Address = reader["ADDRESS"].ToString(),
                                    NewCharge = Convert.ToDecimal(reader["NEWCHARGE"]),
                                    Remarks = reader["REMARK"].ToString(),
                                    AdminStatus = reader["ADMINSTATUS"].ToString(),
                                    AdminRemarks = reader["ADMINREMARKS"].ToString(),
                                    NewStopId = reader["ADREQBUSSTOPID"].ToString()
                                };

                                // Fetch New Route description based on NewStopId
                                IEnumerable<BusRouteModel> busRoutes = GetBusRouteFromStop(routeChangeRequest.NewStopId);
                                if (busRoutes.Any())
                                {
                                    routeChangeRequest.NewRoute = busRoutes.First().DESCRIP;
                                }

                                return routeChangeRequest;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception if needed
                    Console.WriteLine(ex.Message); // This can be replaced with proper logging
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close(); // Ensure the connection is closed after the operation
                    }
                }
            }

            return null; // If no rows were found or there was an error
        }

        public bool SentMailforSubmit(string strEmpCode,string userName, string strRequest, string strDate, string strAddress)
        {
            string strAuthEmailTo = string.Empty;
            string strAuthEmailCc = string.Empty;
            var objds = GetAdminEmailId();

            if (objds != null && objds.Tables.Count > 0 && objds.Tables[0].Rows.Count > 0)
            {
                strAuthEmailTo = objds.Tables[0].Rows[0][0]?.ToString();
                strAuthEmailCc = objds.Tables[0].Rows[0][1]?.ToString();
            }

            if (!string.IsNullOrEmpty(strAuthEmailTo))
            {
                commanEmail sendMail = new commanEmail();
                {
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                };

                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                    sendMail.MailTo = strAuthEmailTo;

                sendMail.MailCc = !string.IsNullOrEmpty(strAuthEmailCc)
                ? strAuthEmailCc : strAuthEmailTo;

                string strType = strRequest == "1"
                                    ? "Bus Membership"
                                    : "Bus Route Change";

                string strSubject = $"{strType} Request from - {userName} [ {strEmpCode} ]";


                string strBody = $@"<table cellpadding = 0 cellspacing = 0 border = 0 width = 600 class=smalltext>" +
                                    "<tr><td  height=35><img src=" + serverpath.getServerPath() + "assets//images//HondaLogo5.gif border=0 /></td>" +
                                    "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                    "</td></tr>" +
                                    "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "assets//images//Table_layout_04.gif height=30>&nbsp;" +
                                    "<b>" + strType + " Request </b></td></tr><tr height=150><td colspan=2>" +
                                    "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                    "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                    "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                    "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear San,</b><br />" +
                                    "<br />" + strEmpCode + " [" + userName + "] " +
                                    "has given a " + strType + " request, the details are as follows:<br />" +
                                    "<br /><tr><td width=125 height=23 valign=top>Request:</td><td width=389 valign=top>" + strType + "</td></tr>" +
                                    "<tr><td width=125 height=23 valign=top>Start date:</td><td width=389 valign=top>" + strDate + "</td>" +
                                    "</tr><tr><td width=125 height=23 valign=top>Resdential Address:</td><td width=389 valign=top>" + strAddress + "</td>" +
                                    "</tr>" +
                                    "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                    "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                    "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                    "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                    "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "assets//images//Table_layout_06.gif border=0 /></td> " +
                                    "</tr></table> ";
               
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;

                try
                {
                    sendMail.Send();
                }
                catch (Exception ex)
                {
                    
                }
            }
            return true;
        }

        public bool SentMailforCancel(string strEmpCode, string userName, string strRequest,string strRequestId, string strDate, string strAddress,string Remarks)
        {
            string strAuthEmailTo = string.Empty;
            string strAuthEmailCc = string.Empty;
            var objds = GetAdminEmailId();

            if (objds != null && objds.Tables.Count > 0 && objds.Tables[0].Rows.Count > 0)
            {
                strAuthEmailTo = objds.Tables[0].Rows[0][0]?.ToString();
                strAuthEmailCc = objds.Tables[0].Rows[0][1]?.ToString();
            }

            if (!string.IsNullOrEmpty(strAuthEmailTo))
            {
                commanEmail sendMail = new commanEmail();
                {
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                };

                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                    sendMail.MailTo = strAuthEmailTo;

                sendMail.MailCc = !string.IsNullOrEmpty(strAuthEmailCc)
                ? strAuthEmailCc : strAuthEmailTo;

                string strType = strRequest == "1"
                                    ? "Bus Membership"
                                    : "Bus Route Change";

                string strSubject = $" "+ strRequest + " Request - Cancelled By Associate";

                string strBody = $@"<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                    "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                    "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                    "</td></tr>" +
                                    "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "assets//images//Table_layout_04.gif height=30>&nbsp;" +
                                    "<b>" + strRequest + " Request </b></td></tr><tr height=150><td colspan=2>" +
                                    "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                    "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                    "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                    "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear San,</b><br />" +
                                    "<br />" + userName + " [" + strEmpCode + "] " +
                                    "has cancelled the " + strRequest + " request, the details are as follows:<br />" +
                                    "<br /><tr><td width=125 height=23 valign=top>Request ID:</td><td width=389 valign=top>" + strRequestId + "</td></tr>" +
                                    "<tr><td width=125 height=23 valign=top>Reason:</td><td width=389 valign=top>" + Remarks + "</td>" +
                                    "</tr>" +
                                    "</p>" +
                                    "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                    "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                    "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                    "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "assets//images//Table_layout_06.gif border=0 /></td> " +
                                    "</tr></table> ";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;

                try
                {
                    sendMail.Send();
                }
                catch (Exception ex)
                {

                }
            }
            return true;
        }

        public bool SentMailforUpdate(string strEmpCode, string userName, string strRequest, string strRequestId, string strDate, string strAddress)
        {
            string strAuthEmailTo = string.Empty;
            string strAuthEmailCc = string.Empty;
            var objds = GetAdminEmailId();

            if (objds != null && objds.Tables.Count > 0 && objds.Tables[0].Rows.Count > 0)
            {
                strAuthEmailTo = objds.Tables[0].Rows[0][0]?.ToString();
                strAuthEmailCc = objds.Tables[0].Rows[0][1]?.ToString();
            }

            if (!string.IsNullOrEmpty(strAuthEmailTo))
            {
                commanEmail sendMail = new commanEmail();
                {
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                };

                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                    sendMail.MailTo = strAuthEmailTo;

                sendMail.MailCc = !string.IsNullOrEmpty(strAuthEmailCc)
                ? strAuthEmailCc : strAuthEmailTo;

                string strType = strRequest == "1"
                                    ? "Bus Membership"
                                    : "Bus Route Change";

                string strSubject = $"{strType} Request from - {userName} [ {strEmpCode} ]";


                string strBody = $@"<table cellpadding = 0 cellspacing = 0 border = 0 width = 600 class=smalltext>" +
                                   "<tr><td  height=35><img src=" + serverpath.getServerPath() + "assets//images//HondaLogo5.gif border=0 /></td>" +
                                   "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                   "</td></tr>" +
                                   "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "assets//images//Table_layout_04.gif height=30>&nbsp;" +
                                   "<b>" + strType + " Request </b></td></tr><tr height=150><td colspan=2>" +
                                   "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                   "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                   "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                   "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear San,</b><br />" +
                                   "<br />" + userName + " [" + strEmpCode + "] " +
                                   "has updated the " + strType + " request, the details are as follows:<br />" +
                                   "<br /><tr><td width=125 height=23 valign=top>Request:</td><td width=389 valign=top>" + strType + "</td></tr>" +
                                   "<tr><td width=125 height=23 valign=top>Start date:</td><td width=389 valign=top>" + strDate + "</td>" +
                                   "</tr><tr><td width=125 height=23 valign=top>Resdential Address:</td><td width=389 valign=top>" + strAddress + "</td>" +
                                   "</tr>" +
                                   "<tr><td valign=top colspan=2>Please login Employee Portal for furthor action.</td></tr></p>" +
                                   "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                   "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                   "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                   "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "assets//images//Table_layout_06.gif border=0 /></td> " +
                                   "</tr></table> ";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;

                try
                {
                    sendMail.Send();
                }
                catch (Exception ex)
                {

                }
            }
            return true;
        }
    }
}
