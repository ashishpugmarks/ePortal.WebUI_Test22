using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Services
{
    public class Stationary: IStationary
    {
        #region "Instance Variables & Constructor"
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objConn;
        private readonly ICommonFunctions objCommon;

        OracleCommand oCmd;
        DataSet ds = new DataSet();
        DataTable Dt;
        DataRow[] _datarow;
        string strConn;
        string errMsg = string.Empty;
        string strQry = string.Empty;

        public Stationary(IDataManagement _oDataMgmt, IConnectionString _objConn, ICommonFunctions _objCommon)
        {
            oDataMgmt = _oDataMgmt;
            objConn = _objConn;
            objCommon = _objCommon;
        }
        #endregion

        #region"INSERT AND UPDATE"
        public int InsertUpdate_StationaryMaster(String strStationaryID, String strStationary_typeCode, String strStationaryCode, String strStationaryDesc, int strActive,
        String strUOM, String strBy, string strplant, string strreorder, string strmaxqty)
        {
            String strErrMsg = String.Empty;
            string strCn = objConn.getConnectingString();
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_ADSTATIONARY.SPROC_UPDATE_STATIONARYMASTER";
                    objCmd.Parameters.Add("STATIONRY_MASTERID_IN", OracleDbType.Int32).Value = strStationaryID;
                    objCmd.Parameters.Add("STATIONRY_TYPECODE_IN", OracleDbType.Varchar2).Value = strStationary_typeCode;
                    objCmd.Parameters.Add("STATIONRY_CODE_IN", OracleDbType.Varchar2).Value = strStationaryCode;
                    objCmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strStationaryDesc;
                    objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Int32).Value = strActive;
                    objCmd.Parameters.Add("UOM_IN", OracleDbType.Varchar2).Value = strUOM;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strBy;
                    objCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strplant;
                    objCmd.Parameters.Add("REORDER", OracleDbType.Varchar2).Value = strreorder;
                    objCmd.Parameters.Add("MAXQTY_IN", OracleDbType.Varchar2).Value = strmaxqty;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    objCmd.Dispose();
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
        public int InsertUpdate_StationaryIN(String strSStockInID, String strStationaryID, String strQty, String strRecivingdate, String strBillno,
        String strPOno, String strRemark, int strActive, String straddby)
        {
            String strErrMsg = String.Empty;
            string strCn = objConn.getConnectingString();
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_ADSTATIONARY.SPROC_UPDATE_STATIONARYIN";
                    objCmd.Parameters.Add("STATIONRY_STOCKID_IN", OracleDbType.Int32).Value = strSStockInID;
                    objCmd.Parameters.Add("STATIONRYID_IN", OracleDbType.Varchar2).Value = strStationaryID;
                    objCmd.Parameters.Add("QTY_IN", OracleDbType.Varchar2).Value = strQty;
                    objCmd.Parameters.Add("RECIVINGDATE_IN", OracleDbType.Varchar2).Value = strRecivingdate;
                    objCmd.Parameters.Add("BILLNO_IN", OracleDbType.Varchar2).Value = strBillno;
                    objCmd.Parameters.Add("PONO_IN", OracleDbType.Varchar2).Value = strPOno;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strRemark;
                    objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Int32).Value = strActive;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = straddby;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    objCmd.Dispose();
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
        public string InsertStationaryReq(string strecode, string strmobileno, string strextno, string strXml, string strapprovalauth)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ADSTATIONARY.SPROC_STATIONARYREQ_SET";
            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            objCmd.Parameters.Add("MOBILENO_IN", OracleDbType.Varchar2).Value = strmobileno;
            objCmd.Parameters.Add("EXTNNO_IN", OracleDbType.Varchar2).Value = strextno;
            objCmd.Parameters.Add("XMLSERVICE_TYPE", OracleDbType.Varchar2).Value = strXml;
            objCmd.Parameters.Add("APPAUTH_IN", OracleDbType.Varchar2).Value = strapprovalauth;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);

            //IF ERROR OCCURED
            if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
            {
                errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);

            }
            return (errMsg);
        }
        public string UpdateStationaryReq(string strreqid, string strecode, string strmobileno, string strextno, string strXml)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ADSTATIONARY.SPROC_STATIONARYREQ_UPDATE";
            objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = strreqid;
            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            objCmd.Parameters.Add("MOBILENO_IN", OracleDbType.Varchar2).Value = strmobileno;
            objCmd.Parameters.Add("EXTNNO_IN", OracleDbType.Varchar2).Value = strextno;
            objCmd.Parameters.Add("XMLSERVICE_TYPE", OracleDbType.Varchar2).Value = strXml;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);

            //IF ERROR OCCURED
            if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
            {
                errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);

            }
            return (errMsg);
        }
        public string CancelStationaryRequest(string RequestID, string Remarks, string empcode)
        {
            string strCn = objConn.getConnectingString();
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
                    objCmd.CommandText = "PKG_ADSTATIONARY.SPROC_CANECLLREQUEST_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(errMsg);
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
        public string UpdateApprovalStatus(string RequestID, string status, string Remarks, string UserID)
        {
            string strCn = objConn.getConnectingString();
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
                    objCmd.CommandText = "PKG_ADSTATIONARY.SPROC_UPDATEAPPROVAL_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        throw new Exception(errMsg);
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
        public string UpdateAdminStatusReq(string strreqid, string strecode, string strststus, string strremark, string strreccode, string strrecname, string strXml)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ADSTATIONARY.SPROC_STYADMINREQ_UPDATE";
            objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = strreqid;
            objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strecode;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strststus;
            objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strremark;
            objCmd.Parameters.Add("RECCODE_IN", OracleDbType.Varchar2).Value = strreccode;
            objCmd.Parameters.Add("RECNAME_IN", OracleDbType.Varchar2).Value = strrecname;
            objCmd.Parameters.Add("XMLSERVICE_TYPE", OracleDbType.Varchar2).Value = strXml;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);

            //IF ERROR OCCURED
            if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
            {
                errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);

            }
            return (errMsg);
        }
        public string AdminInsertStationaryReq(string strecode, string strXml)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ADSTATIONARY.SPROC_STATIONARYADMINREQ_SET";
            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            objCmd.Parameters.Add("XMLSERVICE_TYPE", OracleDbType.Varchar2).Value = strXml;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);

            //IF ERROR OCCURED
            if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
            {
                errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);

            }
            return (errMsg);
        }
        #endregion

        #region"GET DATA"
        public DataTable Get_StationaryIN(string strstockid, String strplantid, string strfmdate, string strtodate, string stritemcode, string strbillno, string strpono, string strstatus)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_STATIONARYIN_GET";
            oCmd.Parameters.Add("STATIONRY_STOCKID_IN", OracleDbType.Varchar2).Value = strstockid;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplantid;
            oCmd.Parameters.Add("FROM_DATE_IN", OracleDbType.Varchar2).Value = strfmdate;
            oCmd.Parameters.Add("TO_DATE_IN", OracleDbType.Varchar2).Value = strtodate;
            oCmd.Parameters.Add("ITEM_ID_IN", OracleDbType.Varchar2).Value = stritemcode;
            oCmd.Parameters.Add("BILL_NO_IN", OracleDbType.Varchar2).Value = strbillno;
            oCmd.Parameters.Add("PO_NO_IN", OracleDbType.Varchar2).Value = strpono;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("CUR_STY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public DataTable Get_Stationarymaster(String strSid, string strstatus, string strplant)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_STATIONARYMASTER_GET";
            oCmd.Parameters.Add("STATIONRY_MASTERID_IN", OracleDbType.Int32).Value = strSid;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplant;
            oCmd.Parameters.Add("CUR_STY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public DataTable GetPendingApprovalList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_APPPENDINGLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetHistoryApprovalList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_APPHISTORYLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPHISTORYLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetPendingRequestList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_PENDINGREQLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_PENREQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetHistoryRequestList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_HISTORYREQLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_HISREQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetRequestDetail(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_REQDETAIL_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_REQDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetRequestDetailPart(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_REQDETAILPART_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_REQDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }
        public DataTable StationaryreqList(string strEmpCode, string strEmpName, string ReqeustStatus, string strDateFrom, string strDateTo, string strKI, string strPlantID)
        {
            OracleCommand objCmd = new OracleCommand();
            DataTable dt = new DataTable();
            objCmd.CommandText = "PKG_ADSTATIONARY.SPROC_STATIONARYREQLIST_GET";
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.Parameters.Add("CUR_REQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            objCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
            objCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = strKI;
            objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strDateFrom;
            objCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = strDateTo;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = ReqeustStatus;
            objCmd.Parameters.Add("PLANT_IN", OracleDbType.Int32).Value = strPlantID;

            //GET DATA FOR DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(objCmd);
            return (dt);
        }
        public DataTable StationaryreqExcel(string strEmpCode, string strEmpName, string ReqeustStatus, string strDateFrom, string strDateTo, string strKI, string strPlantID)
        {
            OracleCommand objCmd = new OracleCommand();
            DataTable dt = new DataTable();
            objCmd.CommandText = "PKG_ADSTATIONARY.SPROC_STATIONARYREQEXCEL_GET";
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.Parameters.Add("CUR_REQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            objCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
            objCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = strKI;
            objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strDateFrom;
            objCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = strDateTo;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = ReqeustStatus;
            objCmd.Parameters.Add("PLANT_IN", OracleDbType.Int32).Value = strPlantID;

            //GET DATA FOR DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(objCmd);
            return (dt);
        }
        public DataTable Get_AvailableStock(String strSid, string strplant)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_AVAILABLE_STOCK";
            oCmd.Parameters.Add("STATIONARYID_IN", OracleDbType.Varchar2).Value = strSid;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplant;
            oCmd.Parameters.Add("CUR_STY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public DataTable Get_IssueStock(String strSid, string strki, string strmonth, string strplant)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_ISSUE_STOCK";
            oCmd.Parameters.Add("STATIONARYID_IN", OracleDbType.Varchar2).Value = strSid;
            oCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strki;
            oCmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplant;
            oCmd.Parameters.Add("CUR_STY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public DataTable Get_MonthlyReport(string strki, string strmonth, string strplant)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_MONTHLY_REPORT";
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = strki;
            oCmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            oCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strplant;
            oCmd.Parameters.Add("CUR_STOCKR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public DataTable GetApprovalList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_ADSTATIONARY.SPROC_APPAUTHORITYLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPAUTHLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        #endregion
    }
}
