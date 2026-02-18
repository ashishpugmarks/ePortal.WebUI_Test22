using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Admin.Interface;

namespace ePortal.Persistence.Admin.Services
{
    public class HumanResource:IHumanResource
    {
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;

        OracleCommand oCmd;
        DataSet ds;
        DataTable Dt;
        OracleConnection objConn;
        string strConn;
        string strErrMsg = string.Empty;

        public HumanResource(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }
        public DataSet getNewJoinee()
        {
           // ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_NEWJOINEE_EMPLOYEE_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objCmd.Parameters.Add("NEWJOINEE_RCSR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    DataSet objDs = new DataSet();
                    objCmd.BindByName = true;
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
        public DataSet GETNEWJOINING_EMPLOYEE(string ecode, string ename, string plantid)
        {
           // ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_USERDETAIL.SPROC_NEWJOININGEMP_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objCmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ename.ToString();
                    objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode.ToString();
                    objCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = plantid.ToString();
                    objCmd.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    DataSet objDs = new DataSet();
                    objCmd.BindByName = true;
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

        public string passwordUpdate_Admin(string strEmpCode, string strEType, string empmodBy)
        {
           // ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_UPDATEENCPASSWORD";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("ADUSERTYPE_IN", OracleDbType.Varchar2).Value = strEType;
                    objCmd.Parameters.Add("ADMODBY_IN", OracleDbType.Varchar2).Value = empmodBy;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 5000).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    string strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
                    return strResult;
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

        //// Get My Voice List
        public DataTable GetMyVoiceList(string status)
        {
           // ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_MYVOICELIST_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objCmd.Parameters.Add("SEARCH_STRING_", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("MYVOICELIST_RCSR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    DataTable objDt = new DataTable();
                    objCmd.BindByName = true;
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
        public string UpdateMyVoiceStatus(string EmpCode, string ReqId, string EmailStatus, string FileName)
        {
           // ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_MYVOICE_UPDATE";
                    objCmd.Parameters.Add("REQ_ID", OracleDbType.Varchar2).Value = ReqId;
                    objCmd.Parameters.Add("FILE_NAME", OracleDbType.Varchar2).Value = FileName;
                    objCmd.Parameters.Add("EMAIL_STATUS", OracleDbType.Varchar2).Value = EmailStatus;
                    objCmd.Parameters.Add("USER_ID", OracleDbType.Varchar2).Value = EmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 5000).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    //string strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
                    string strResult = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    return strResult;
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
