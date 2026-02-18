using ePortal.Persistence.Interface;
using ePortal.Persistence.TourRequest.Interface;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ePortal.Persistence.TourRequest.Services
{
    public class TourBudget : ITourBudget
    {
        private readonly IConfiguration configuration;
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        private readonly ICommonFunctions objcmn;

        DataSet ds = new DataSet();
        DataRow[] _datarow;

        public TourBudget(IConfiguration _configuration, IDataManagement _oDataMgmt, IConnectionString _objCnStr, ICommonFunctions _objcmn)
        {
            configuration = _configuration;
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
            objcmn = _objcmn;
        }

        /// <summary>
        /// GET ACTIVE tour budget LIST
        /// </summary>
        /// <returns></returns>
        public DataSet Gettourbudget(string stradtourbudget, string strtourbudgetki, string stroperationid, string strstatus, string strtravelid)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_TOURBUDGET_GET";
            oCmd.Parameters.Add("ADTOURBUDGET_IN", OracleDbType.Varchar2).Value = stradtourbudget;
            oCmd.Parameters.Add("TOURBUDGETKI_IN", OracleDbType.Varchar2).Value = strtourbudgetki;
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = stroperationid;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("TRAVELID_IN", OracleDbType.Varchar2).Value = strtravelid;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }

        public string INSERTTRAVELBUDGET(string strtravelbudgetid, string strki, string stroperation, string stramount, string status, string addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_TOURBUDGET_INSERT";
            oCmd.Parameters.Add("ADTOURBUDGET_IN", OracleDbType.Varchar2).Value = strtravelbudgetid;
            oCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strki;
            oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = stroperation;
            oCmd.Parameters.Add("AMOUNT_IN", OracleDbType.Varchar2).Value = stramount;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public DataSet GettourbudgetHis(string stradtourbudgethis, string strtourbudgetki, string stroperationid, string strtravelid, string strtraveltype, string strstatus)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_TOURBUDGETHIS_GET";
            oCmd.Parameters.Add("ADTOURBUDGETHISTORYID_IN", OracleDbType.Varchar2).Value = stradtourbudgethis;
            oCmd.Parameters.Add("TOURBUDGETKI_IN", OracleDbType.Varchar2).Value = strtourbudgetki;
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = stroperationid;
            oCmd.Parameters.Add("TRAVELID_IN", OracleDbType.Varchar2).Value = strtravelid;
            oCmd.Parameters.Add("TRAVELTYPE_IN", OracleDbType.Varchar2).Value = strtraveltype;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }


        public string INSERTTRAVELBUDGETHIS(string strtravelbudgetid, string strki, string stroperation, string strtravelid, string strtraveltype, string stramount, string status, string addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_TOURBUDGETHIS_INSERT";
            oCmd.Parameters.Add("ADTOURBUDGETHISTORYID_IN", OracleDbType.Varchar2).Value = strtravelbudgetid;
            oCmd.Parameters.Add("TOURHISKI_IN", OracleDbType.Varchar2).Value = strki;
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = stroperation;
            oCmd.Parameters.Add("TRAVELID_IN", OracleDbType.Varchar2).Value = strtravelid;
            oCmd.Parameters.Add("TRAVELTYPE_IN", OracleDbType.Varchar2).Value = strtraveltype;
            oCmd.Parameters.Add("AMOUNT_IN", OracleDbType.Varchar2).Value = stramount;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public string INSERTTICKETBUDGET(string strreqid, string stramount, string status, string addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_TICKETAMOUNT_INSERT";
            oCmd.Parameters.Add("REQUESTDETAILID_IN", OracleDbType.Varchar2).Value = strreqid;
            oCmd.Parameters.Add("AMOUNT_IN", OracleDbType.Varchar2).Value = stramount;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public string INSERTFINANCETBUDGET(string strreqid, string stramount, string status, string addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_FIANCETAMOUNT_INSERT";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = strreqid;
            oCmd.Parameters.Add("AMOUNT_IN", OracleDbType.Varchar2).Value = stramount;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public string INSERTSETTLMENTBUDGET(string strreqid, string stramount, string status, string addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_SETTLEMENTAMOUNT_INSERT";
            oCmd.Parameters.Add("SETTLMENTREQID_IN", OracleDbType.Varchar2).Value = strreqid;
            oCmd.Parameters.Add("AMOUNT_IN", OracleDbType.Varchar2).Value = stramount;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }
        public DataSet GETTOUREXPENSEREPORT(string strtourbudgetki, string stroperationid, string strstatus)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_TOUREXPENSEREPORT_GET";
            oCmd.Parameters.Add("TOURBUDGETKI_IN", OracleDbType.Varchar2).Value = strtourbudgetki;
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = stroperationid;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }
        #region ORG Mapping with Operation 
        //Get ORG Mapping with Operation
        public DataSet GETORGMAPWithOPR(string ORGOpMapId, string OperationId, string Ki, string menutypeid, string orgid, string status)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_ADTOUROPMAPPGET";
            oCmd.Parameters.Add("TOUROPMAPID_IN", OracleDbType.Varchar2).Value = ORGOpMapId;
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = OperationId;
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = Ki;
            oCmd.Parameters.Add("MENUTYPEIID_IN", OracleDbType.Varchar2).Value = menutypeid;
            oCmd.Parameters.Add("ORGID_IN", OracleDbType.Varchar2).Value = orgid;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();
        }
        //INSERT ORG Mapping with Operation Travel//
        public string INSERTORGMApOpER(string OperationiD, string Plantki, string OrgId, string menutypeId, string status, string addedby, string touropmappid)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_ADTOUROPMAPPBUD_INSERT";
            oCmd.Parameters.Add("Operation_IN", OracleDbType.Varchar2).Value = OperationiD;
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = Plantki;
            oCmd.Parameters.Add("MENUTYPEIID_IN", OracleDbType.Varchar2).Value = menutypeId;
            oCmd.Parameters.Add("ORGNIZATION_IN", OracleDbType.Varchar2).Value = OrgId;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("TOUROPMAPID_IN", OracleDbType.Varchar2).Value = touropmappid;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        //Get ORG Mapping with Operation asset
        public DataSet GETORGMAPWithOPRAsset(string ORGOpMapId, string OperationId, string Ki, string menutypeid, string orgid, string status)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_ADAssetOPMAPPGET";
            oCmd.Parameters.Add("TOUROPMAPID_IN", OracleDbType.Varchar2).Value = ORGOpMapId;
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = OperationId;
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = Ki;
            oCmd.Parameters.Add("MENUTYPEIID_IN", OracleDbType.Varchar2).Value = menutypeid;
            oCmd.Parameters.Add("ORGID_IN", OracleDbType.Varchar2).Value = orgid;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();
        }
        //INSERT ORG Mapping with Operation Asset//
        public string INSERTORGMApOpERAsset(string OperationiD, string Plantki, string OrgId, string menutypeId, string status, string addedby, string touropmappid)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_ADAssetOPMAPPBUD_INSERT";
            oCmd.Parameters.Add("Operation_IN", OracleDbType.Varchar2).Value = OperationiD;
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = Plantki;
            oCmd.Parameters.Add("MENUTYPEIID_IN", OracleDbType.Varchar2).Value = menutypeId;
            oCmd.Parameters.Add("ORGNIZATION_IN", OracleDbType.Varchar2).Value = OrgId;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("TOUROPMAPID_IN", OracleDbType.Varchar2).Value = touropmappid;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
        }

        public DataTable GetADORGLEVELTYPE()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_TOURBUDGET.SPROC_ADORGLEVELTYPE_GET";
                    objCmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        public DataTable GetOPBUDGET(string strecode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_TOURBUDGET.SPROC_ADBUDGETOPERATION_GET";
                    objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
                    objCmd.Parameters.Add("CUR_BUDGETOP", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataSet GETTOURBUDGETREPORT(string strtourbudgetki, string stroperationid, string strstatus)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_TOURBUDGETREPORT_GET";
            oCmd.Parameters.Add("TOURBUDGETKI_IN", OracleDbType.Varchar2).Value = strtourbudgetki;
            oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = stroperationid;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
    }
}