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
    public class Password:IPassword
    {
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        public Password(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }
        
        public string updatepassword(string struserid, string strpassword, string strUserType)
        {
            OracleCommand objCmd;            
            OracleConnection objConn = new OracleConnection();
            string Result;
            
            string strCn = objCnStr.getConnectingString();
            using (objConn)
            {
                objConn.ConnectionString = strCn;
                try
                {
                    objConn.Open();
                    objCmd = new OracleCommand();
                    string strSql = "SPROC_CHANGEPASSWORD";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = struserid;
                    objCmd.Parameters.Add("ADUSERTYPE_IN", OracleDbType.Varchar2).Value = strUserType;
                    objCmd.Parameters.Add("PASSWORD_IN", OracleDbType.Varchar2).Value = strpassword;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 5000).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    Result = Convert.ToString((objCmd.Parameters["RESULT_OUT"].Value.ToString()).ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
                    return Result;
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
                
        public DataSet GetEmail(string strEmpCode, string strUserType)
        {
            OracleConnection objCn;
            
            OracleCommand objCmd;
            OracleDataAdapter objAdr;
            DataSet objDs;
            string strCn;
            
            strCn = objCnStr.getConnectingString();

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SELECT A.EMAILID, B.PASSWORD FROM ADLOGINUSER A , ADEMPLOGIN B  WHERE A.ADEMPCODE = " + strEmpCode + " AND B.ADEMPCODE = " + strEmpCode + " and B.SYUSERTYPEID =  " + strUserType;

                    objCmd.CommandType = System.Data.CommandType.Text;
                    objAdr = new OracleDataAdapter(objCmd);
                    objDs = new DataSet();
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
                
        public static string Randompassword(string DESIGNATION)
        {
            //if (DESIGNATION == "16" || DESIGNATION == "19")
            //{
            //    string _allowedChars = "0123456789";
            //    Random randNum = new Random();
            //    char[] chars = new char[4];
            //    int allowedCharCount = _allowedChars.Length;
            //    for (int i = 0; i < 4; i++)
            //    {
            //        chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            //    }
            //    return new string(chars);
            //}
            //else
            //{
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[8];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < 8; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
            // }
        }
                
        public DataTable GetContractualDetailsNew(string intEmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_CONTRACTUAL.SPROC_CONTRACTUALEMP_GET";
            oCmd.Parameters.Add("EMPLOYEECODE_IN", OracleDbType.NVarchar2).Value = intEmpCode;
            oCmd.Parameters.Add("CUR_EMPDET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }       

        public string PASSWORDRESETMASTER(string strempcode, string strpassword)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_CONTRACTUAL.SPROC_CONTRESETPWD";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.NVarchar2).Value = strempcode;
            ocmd.Parameters.Add("PASSWORD_IN", OracleDbType.NVarchar2).Value = strpassword;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.NVarchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.NVarchar2, 1000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(ocmd);
            string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
        }
    }
}
