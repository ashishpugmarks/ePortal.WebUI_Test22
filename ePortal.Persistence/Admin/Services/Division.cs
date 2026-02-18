using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Admin.Services
{
    public class Division:IDivision
    {
        #region"Instance variables"
        DataSet ds = new DataSet();
        DataRow[] _datarow;
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;

        public Division(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }

        #endregion



        #region"Get Functions"
        public DataTable GetAllDivision()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MASTERS.SPROC_DIVISION_GET";
            oCmd.Parameters.Add("CUR_DIVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataSet GetFilterDivision(int operationID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
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
                    strSql = " SELECT DISTINCT DIVID AS  ADDIVISIONID, DIV AS  DESCRIP  FROM VW_ORGLEVELDETAILS WHERE 1=1  ";
                    if (operationID != 0)
                        strSql = strSql + " and OPID=" + operationID.ToString() + " AND DIVID IS NOT NULL ";
                    strSql = strSql + "  ORDER BY DIV ";
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
        public DataSet GetFilterDivision(int operationID, String strKI)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
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
                    strSql = "SELECT DISTINCT DIVID AS ADDIVISIONID, DIV AS DESCRIP,ACTIVE FROM VW_ORGLEVELDETAILS_KI WHERE 1= 1 "; ;
                    if (operationID != 0)
                        strSql = strSql + " and OPID=" + operationID.ToString() + "  ";
                    strSql = strSql + " and SYKIID= " + strKI + " AND DIVID IS NOT NULL  ORDER BY DIV";
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
        public DataSet GetDivision()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
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
                    strSql = "SELECT O.ADORGLEVELID AS ADVPID,O.LEVELDESCRIP AS DESCRIP FROM VW_OPERATION O ";
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds, "ADVP");
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
        //public string getConnectingString()
        //{
        //    string strCn = string.Empty;
        //    strCn = ConfigurationManager.ConnectionStrings["cnConn"].ConnectionString;
        //    return strCn;
        //}
        public DataSet getDetails(string strDivisionID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MASTERS.SPROC_DIVISIONBYID_GET";
            oCmd.Parameters.Add("DIVISION_ID", OracleDbType.Varchar2).Value = strDivisionID;
            oCmd.Parameters.Add("CUR_DIVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }

        public DataSet GetFilterDivisionOJT(int operationID, string str_KIid)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
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
                    strSql = " SELECT DISTINCT ADORGLEVELID AS  ADDIVISIONID, LEVELDESCRIP AS  DESCRIP  FROM VW_DIVISION WHERE 1=1  ";
                    if (operationID != 0)
                        strSql = strSql + " and PARENTLEVELID = " + operationID.ToString();
                    strSql = strSql + " AND SYKIID = " + str_KIid;
                    strSql = strSql + "  ORDER BY LEVELDESCRIP ";
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
        #endregion

        #region"Insert/Update Functions"
        public int AddDivision(string strDivDesc, string strDivIniDesc, string strDivHeadID, string strADVPID, string strStatus, string strCreatedBy, string strID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                object Status1;
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_MASTERS.SPROC_UPDATE_DIVISION";
                    objCmd.Parameters.Add("ADDIVISIONID_IN", OracleDbType.Varchar2).Value = strID;
                    objCmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = strDivDesc;
                    objCmd.Parameters.Add("INITIALDESCRIP_IN", OracleDbType.Varchar2).Value = strDivIniDesc;
                    objCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = strADVPID;
                    objCmd.Parameters.Add("BY_IN", OracleDbType.Varchar2).Value = strCreatedBy;
                    objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strStatus;
                    objCmd.Parameters.Add("DIVISIONHEADID_IN", OracleDbType.Varchar2).Value = strDivHeadID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    Status1 = objCmd.ExecuteNonQuery();
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
        #endregion
    }
}
