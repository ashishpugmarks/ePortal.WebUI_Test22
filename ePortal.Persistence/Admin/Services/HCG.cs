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
    public class HCG:IHCG
    {
        private readonly IConnectionString objCnStr;
        public HCG(IConnectionString _objCnStr) {
            objCnStr = _objCnStr;
        }    
        public int AddHCG(string hcgid, string series, string attachment, string status, string strhcgtype, int AddedBy, string strreleasedate)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    string strSql = "PKG_HCG.SPROC_UPDATE_HCG";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = strSql;
                    cmd.Parameters.Add("HCGID_IN", OracleDbType.Varchar2).Value = hcgid;
                    cmd.Parameters.Add("SERIES_IN", OracleDbType.Varchar2).Value = series;
                    cmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = attachment;
                    cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    cmd.Parameters.Add("HCGTYPE_IN", OracleDbType.Varchar2).Value = strhcgtype;
                    cmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = AddedBy;
                    cmd.Parameters.Add("RELEASEDATE_IN", OracleDbType.Varchar2).Value = strreleasedate;
                    cmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                    cmd.BindByName = true;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    {
                        return 1;
                    }
                    else
                    {
                        if (cmd.Parameters["RESULT_OUT"].Value.ToString() == "2")
                        {
                            return 2;
                        }
                        else
                        {
                            return 0;
                        }
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
        public DataSet gethcgDetails()
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
                    cmd.CommandText = "PKG_HCG.SPROC_HCG_GET";
                    cmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.BindByName = true;
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
