using System.Data;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Admin.Services
{
    public class Login:ILogin
    {
        private readonly IConnectionString objCnStr;
        public Login(IConnectionString _objCnStr)
        {
            objCnStr = _objCnStr;
        }

        public int getEmpLogin(string strUserID, string strPWD, string strUserType, string strclintip)
        {
            string strCn;
            OracleCommand objCmd;
            object objCount;

            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_VALIDATELOGIN_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("USER_PASSWORD_IN", OracleDbType.Varchar2).Value = strPWD;
                    objCmd.Parameters.Add("USER_TYPE_IN", OracleDbType.Int32).Value = strUserType;
                    objCmd.Parameters.Add("USER_IP", OracleDbType.Varchar2).Value = strclintip;
                    objCmd.Parameters.Add("LOGINRESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    objCount = objCmd.Parameters["LOGINRESULT_OUT"].Value.ToString();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return Convert.ToInt32(objCount.ToString());
            }
        }

        public string getEmpName(string strUserID, string strUserType)
        {
            string strCn;
            OracleCommand objCmd;
            object objCount;


            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_LOGIN_EMPNAME_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("USERTYPE_IN", OracleDbType.Int32).Value = strUserType;

                    objCmd.Parameters.Add("USERNAME_OUT", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    objCount = objCmd.Parameters["USERNAME_OUT"].Value.ToString();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return Convert.ToString(objCount.ToString());
            }
        }

        public int getServiceLogin(string strUserID)
        {
            string strCn;
            OracleCommand objCmd;
            object objCount;

            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_VALIDATESERVICELOGIN_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("LOGINRESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    objCount = objCmd.Parameters["LOGINRESULT_OUT"].Value.ToString();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return Convert.ToInt16(objCount.ToString());
            }
        }

        public int Check_EmpLogin(string strUserID, string strPWD, string strProject)
        {
            string strCn;
            OracleCommand objCmd;
            object objCount;

            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_CHECK_LOGIN";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("USER_PASSWORD_IN", OracleDbType.Varchar2).Value = strPWD;
                    objCmd.Parameters.Add("APPLICATIONID", OracleDbType.Int32).Value = strProject;
                    objCmd.Parameters.Add("LOGINRESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    objCount = objCmd.Parameters["LOGINRESULT_OUT"].Value.ToString();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return Convert.ToInt16(objCount.ToString());
            }
        }

        public int check_FirstLogin(string strUserID, string strPWD, string strUserType)
        {
            string strCn;
            OracleCommand objCmd;
            object objCount;

            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_FIRSTLOGIN_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("USER_TYPE_IN", OracleDbType.Int32).Value = strUserType;
                    objCmd.Parameters.Add("LOGINRESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    objCount = objCmd.Parameters["LOGINRESULT_OUT"].Value.ToString();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return Convert.ToInt16(objCount.ToString());
            }
        }

        public void update_FirstLogin(string strUserID, int strUserType)
        {
            string strCn;
            OracleCommand objCmd;
            object objCount;

            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_USERDETAIL.SPROC_FIRSTLOGIN_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("USERTYPE_IN", OracleDbType.Int32).Value = strUserType;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    objCount = objCmd.ExecuteNonQuery();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                //return Convert.ToInt16(objCount);
            }
        }

        public int check_LoginBefore90Days(string strUserID, string strUserType)
        {
            string strCn;
            OracleCommand objCmd;
            object objCount;

            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_USERDETAIL.SPROC_LOGINBEFORE90DAYS";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("USER_TYPE_IN", OracleDbType.Int32).Value = strUserType;
                    objCmd.Parameters.Add("LOGINRESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    objCount = objCmd.Parameters["LOGINRESULT_OUT"].Value.ToString();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return Convert.ToInt16(objCount.ToString());
            }
        }

        public int check_LoginExpiry(string strUserID, out string strExpiryDayLeft, out int IsEligibleForPolicy)
        {
            string strCn;
            OracleCommand objCmd;
            object objCount;

            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "SPROC_LOGINEXPALERTNEW_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("EXPIRY_DAYS_OUT", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("LOGINRESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("IS_ELIGIBLE_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    IsEligibleForPolicy = Convert.ToInt32(objCmd.Parameters["IS_ELIGIBLE_OUT"].Value.ToString());
                    strExpiryDayLeft = objCmd.Parameters["EXPIRY_DAYS_OUT"].Value.ToString();
                    objCount = objCmd.Parameters["LOGINRESULT_OUT"].Value;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return Convert.ToInt32(objCount.ToString());
            }
        }

        //--90/30 days password policy - Start
        public int getPassExpiryDays(string strUserID)
        {
            string strCn;
            OracleCommand objCmd;
            object objCount;

            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_USERDETAIL.SPROC_GETPASSEXPDAYS";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("LOGINRESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    objCount = objCmd.Parameters["LOGINRESULT_OUT"].Value.ToString();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return Convert.ToInt16(objCount.ToString());
            }
        }
    }
}
