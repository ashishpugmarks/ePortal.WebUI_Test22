using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;
namespace ePortal.Persistence.Services
{
    public class TaxationServices : ITaxationServices
    {
        #region "Local Variables"

        DataSet ds = new DataSet();
        DataRow[] _datarow;
        string qry;

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;

        #endregion

        #region"Ctor"
        public TaxationServices(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }
        #endregion

        public int AddTAwareness(string taxationid, string series, string attachment, string status, int AddedBy, string strreleasedate)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    string strSql = "PKG_TAXATIONSERIES.SPROC_UPDATE_TAXATIONAS";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = strSql;
                    cmd.Parameters.Add("TAXATIONAS_IN", OracleDbType.Varchar2).Value = taxationid;
                    cmd.Parameters.Add("SERIES_IN", OracleDbType.Varchar2).Value = series;
                    cmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = attachment;
                    cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    cmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = AddedBy;
                    cmd.Parameters.Add("RELEASEDATE_IN", OracleDbType.Varchar2).Value = strreleasedate;
                    cmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    string strError = ex.Message;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
            return 0;
        }
        public DataSet GetTAwarenessDetails()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    DataSet objDS = new DataSet();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = "PKG_TAXATIONSERIES.SPROC_TAXATIONAS_GET";
                    cmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    oDa.Fill(objDS);
                    return objDS;
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
        public int AddTCirculars(string taxationid, string series, string attachment, string status, int AddedBy, string strreleasedate)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    string strSql = "PKG_TAXATIONSERIES.SPROC_UPDATE_TAXATIONCS";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = strSql;
                    cmd.Parameters.Add("TAXATIONCS_IN", OracleDbType.Varchar2).Value = taxationid;
                    cmd.Parameters.Add("SERIES_IN", OracleDbType.Varchar2).Value = series;
                    cmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = attachment;
                    cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    cmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = AddedBy;
                    cmd.Parameters.Add("RELEASEDATE_IN", OracleDbType.Varchar2).Value = strreleasedate;
                    cmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    string strError = ex.Message;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
            return 0;
        }
        public DataSet GetTCircularsDetails()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    DataSet objDS = new DataSet();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = "PKG_TAXATIONSERIES.SPROC_TAXATIONCS_GET";
                    cmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    oDa.Fill(objDS);
                    return objDS;
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
        public int AddTForms(string taxationid, string series, string attachment, string status, int AddedBy, string strreleasedate)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    string strSql = "PKG_TAXATIONSERIES.SPROC_UPDATE_TAXATIONFS";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = strSql;
                    cmd.Parameters.Add("TAXATIONFS_IN", OracleDbType.Varchar2).Value = taxationid;
                    cmd.Parameters.Add("SERIES_IN", OracleDbType.Varchar2).Value = series;
                    cmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = attachment;
                    cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    cmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = AddedBy;
                    cmd.Parameters.Add("RELEASEDATE_IN", OracleDbType.Varchar2).Value = strreleasedate;
                    cmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    string strError = ex.Message;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
            return 0;
        }
        public DataSet GetTFormsDetails()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    DataSet objDS = new DataSet();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = "PKG_TAXATIONSERIES.SPROC_TAXATIONFS_GET";
                    cmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    oDa.Fill(objDS);
                    return objDS;
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

    }
}
