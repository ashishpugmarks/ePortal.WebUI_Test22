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
    public class DataManagement: IDataManagement
    {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        DataRow[] _datarow;

        private readonly IConnectionString objCnStr;
        public DataManagement(IConnectionString _objCnStr)
        {
            objCnStr = _objCnStr;
        }

        /// <summary>
        /// GET DATASET AGAINST ORACLE QUERY
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string strSql)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    ds = new DataSet();
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
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
        /// GET DATASET AGAINST ORACLE COMMAND
        /// </summary>
        /// <param name="objCmd"></param>
        /// <returns></returns>
        public DataSet GetDataSet(OracleCommand objCmd)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    ds = new DataSet();
                    objCn.Open();
                    objCmd.Connection = objCn;
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
        /// GET DATA TABLE AGAINST ORACLE QUERY
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string strSql)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    dt = new DataTable();
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(dt);
                    return dt;
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
        /// GET DATA TABLE AGAINST ORACLE COMMAND
        /// </summary>
        /// <param name="objCmd"></param>
        /// <returns></returns>
        public DataTable GetDataTable(OracleCommand objCmd)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    dt = new DataTable();
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(dt);
                    return dt;
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
        /// GET DATA ROW AGAINST ORACLE QUERY
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public DataRow[] GetDataRow(string strSql)
        {
            // ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    dt = new DataTable();
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(dt);
                    _datarow = dt.Select();
                    return _datarow;
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
        /// GET DATAROW AGAISNT ORACLE COMMAND
        /// </summary>
        /// <param name="objCmd"></param>
        /// <returns></returns>
        public DataRow[] GetDataRow(OracleCommand objCmd)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    dt = new DataTable();
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(dt);
                    _datarow = dt.Select();
                    return _datarow;
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
        /// EXECUTE ORACLE QUERY
        /// </summary>
        /// <param name="strSql"></param>
        public void ExecuteQuery(string strSql)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
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
        /// EXECUTE ORACLE COMMAND
        /// </summary>
        /// <param name="objCmd"></param>
        public void ExecuteQuery(OracleCommand objCmd)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
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
