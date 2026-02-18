using System.Data;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Admin.Services
{
    public class Training: ITraining
    {
        #region "Local Variables"

        DataSet ds = new DataSet();
        DataRow[] _datarow;
        string qry;

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        
        #endregion

        #region"Ctor"
        public Training(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }
        #endregion

        #region"Get Functions"

        public DataTable GetKI()
        {
            DataTable dt;
            qry = "SELECT ki.kicode,ki.sykiid,ki.active FROM SYKI ki ORDER BY KICODE";
            dt = oDataMgmt.GetDataTable(qry);
            return (dt);
        }

        public DataTable GetAssociateList(String strLoggedUser, String strOperation, String strDivision, String strDept, String strSection)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_ASSOCIATES_GET";
                    objCmd.Parameters.Add("OP_IN", OracleDbType.Int32).Value = strOperation;
                    objCmd.Parameters.Add("DIV_IN", OracleDbType.Int32).Value = strDivision;
                    objCmd.Parameters.Add("DEPT_IN", OracleDbType.Int32).Value = strDept;
                    objCmd.Parameters.Add("SEC_IN", OracleDbType.Int32).Value = strSection;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strLoggedUser;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable EmployeeDesignation(string strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_EMPDESIG_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable EmployeesTrainings(String strLoginEmpCode, string strEmpCode, string strEmpName, string strOperation, string strDivision,
                                            string strDepartment, string strSection, string strTraining, string strStatus,
                                            string strKI, string strFromDt, string strToDate)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGLIST_GET";
                    objCmd.Parameters.Add("LOGGEDEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
                    objCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = strOperation;
                    objCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = strDivision;
                    objCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strDepartment;
                    objCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
                    objCmd.Parameters.Add("TRAINING_IN", OracleDbType.Varchar2).Value = strTraining;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                    objCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strKI;
                    objCmd.Parameters.Add("FROM_DT", OracleDbType.Varchar2).Value = strFromDt;
                    objCmd.Parameters.Add("TO_DT", OracleDbType.Varchar2).Value = strToDate;

                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetMyTrainings(String strLoginEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_MYTRAININGLIST_GET";
                    objCmd.Parameters.Add("LOGGEDEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetAvailableTrainingDates(string strBatchID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGDATES_GET";
                    objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Int32).Value = strBatchID;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetRecommendedAuthority(string strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_RECOMMENDEDAUTHLIST_GET";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_RECAUTHLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetApprovalAuthority(string strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_APPAUTHORITYLIST_GET";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_APPAUTHLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetPendingReScheduleRequest(string strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRANRESCHPENDINGLIST_GET";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_APPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetReScheduleRequestDetails(string strEmpCode, string strRequestID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRANINGDETAILS_GET";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestID;
                    objCmd.Parameters.Add("CUR_APPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetHistoryApprovalList(String EmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_APPHISTORYLIST_GET";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
                    objCmd.Parameters.Add("CUR_APPHISTORYLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetRequestHistory(String RequestID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_REQHISTORY_GET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
                    objCmd.Parameters.Add("CUR_REQHISTORY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetPendingRequestList(string strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_PENDINGREQLIST_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_PENREQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetTrainingRequestHistory(String strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_HRTRAINGREQHISTORY_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_REQHISTORY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public void CheckRec_App_Authority(String strPOS, int strOP, int strDIV, int strDPT, int strSEC, out String IsRecExist, out String IsAppExist)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_CHECKRECAPP_AUTH_GET";
                    objCmd.Parameters.Add("EMPPOS", OracleDbType.Varchar2).Value = strPOS;
                    objCmd.Parameters.Add("EMPDIV", OracleDbType.Varchar2).Value = strDIV;
                    objCmd.Parameters.Add("EMPDPT", OracleDbType.Varchar2).Value = strDPT;
                    objCmd.Parameters.Add("EMPSEC", OracleDbType.Varchar2).Value = strSEC;
                    objCmd.Parameters.Add("EMPOP", OracleDbType.Varchar2).Value = strOP;
                    objCmd.Parameters.Add("REC_AUTH", OracleDbType.Varchar2, 20).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("APP_AUTH", OracleDbType.Varchar2, 20).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    IsRecExist = Convert.ToString(objCmd.Parameters["REC_AUTH"].Value);
                    IsAppExist = Convert.ToString(objCmd.Parameters["APP_AUTH"].Value);
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

        public DataTable GetOldAttendedTrainings(String strEmpCode, String strTrainingTransID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPORC_OLDATTENDEDTRAING_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("TRGTRANSID_IN", OracleDbType.Varchar2).Value = strTrainingTransID;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetTrainingDetail(String strEmpCode, String strBatchID)
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
                    string strSql = "PKG_HRTRAINING.SPORC_TRAININGDETAIL_GET";
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
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

        public DataTable GetEmployeeEmailID(String strEmpCode)
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
                    string strSql = "PKG_HRTRAINING.SPORC_EMPEMAILID_GET";
                    objCmd.Parameters.Add("CUR_EMAILID", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
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

        public DataTable GetPendingTrainingReScheduleHR(String strLoginEmpCode, string strEmpCode, string strEmpName, string strOperation, string strDivision,
                                            string strDepartment, string strSection, string strTraining, string strStatus,
                                            string strKI, string strFromDt, string strToDate)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_PENDINGLISTFORHR_GET";
                    objCmd.Parameters.Add("LOGGEDEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
                    objCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = strOperation;
                    objCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = strDivision;
                    objCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strDepartment;
                    objCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
                    objCmd.Parameters.Add("TRAINING_IN", OracleDbType.Varchar2).Value = strTraining;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                    objCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strKI;
                    objCmd.Parameters.Add("FROM_DT", OracleDbType.Varchar2).Value = strFromDt;
                    objCmd.Parameters.Add("TO_DT", OracleDbType.Varchar2).Value = strToDate;

                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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



        public DataTable GetFeedbackDetailsForTraining(String strBatchID, String strFeedbackStatus)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_FEEDBACKDETAIL_GET";
                    objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
                    objCmd.Parameters.Add("FEEDBACKSTATUS_IN", OracleDbType.Varchar2).Value = strFeedbackStatus;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetFeedbackCourseContentScores(String strHrTrainingDetailID)
        {
           // ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_FEEDBCKCOURSESCRS_GET";
                    objCmd.Parameters.Add("HRTRAINGDETAILID_IN", OracleDbType.Varchar2).Value = strHrTrainingDetailID;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetFeedbackTrainerScores(String strHrTrainingDetailID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_FEEDBCKTRAINERSCRS_GET";
                    objCmd.Parameters.Add("HRTRAINGDETAILID_IN", OracleDbType.Varchar2).Value = strHrTrainingDetailID;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetFeedbackTrainingEnvScores(String strHrTrainingDetailID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_FEEDBCKTRNENV_GET";
                    objCmd.Parameters.Add("HRTRAINGDETAILID_IN", OracleDbType.Varchar2).Value = strHrTrainingDetailID;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetTrainingDetails(String strBatchID, String strHRTrainingDetailID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGDETAILS_GET";
                    objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
                    objCmd.Parameters.Add("HRTRAININGDETAILID_IN", OracleDbType.Varchar2).Value = strHRTrainingDetailID;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetTrainingBatch(String strTrainingID, String strKI)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGBATCH_GET";
                    objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingID;
                    objCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strKI;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetTrainers(String strTrainingID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAINERS_GET";
                    objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingID;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetTrainingByActiveKI()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPORC_TRAININGFORKI_GET";
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetBatchesTillDate(String strTrainingID, String strKI)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPORC_BATCHESBYTRNGID";
                    objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingID;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKI;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable GetBatchDetails(String strTrainingID, String strBatchID, String strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPORC_BATCH_GET";
                    objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingID;
                    objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        #endregion

        #region"Insert/Update function"

        public int InsertTrainingReSchedule(String strAuthType, String strEmpCode, String strHRTrainingID,
                        String strRecAdeEmpCode, String strRemarks, String strAddedBy, String strActive, String strNewRecScheduleTrnID)
        {

            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;


            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRTRAINING.SPROC_TRAININGRESCHREQUEST_SET";

                    objCmd.Parameters.Add("AUTHTYPE_IN", OracleDbType.Varchar2).Value = strAuthType;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("HRTRAININGID_IN", OracleDbType.Varchar2).Value = strHRTrainingID;
                    objCmd.Parameters.Add("RECADEMPCODE_IN", OracleDbType.Varchar2).Value = strRecAdeEmpCode;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    objCmd.Parameters.Add("RECSCHTRNID_IN", OracleDbType.Varchar2).Value = strNewRecScheduleTrnID;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strAddedBy;
                    objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strActive;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 800).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    string strerr = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

        public string UpdateApprovalStatus(string AppType, string ResheduleDt, string RequestID, string status, string Remarks,
                                           string AppAuthCode, string IsRecApplicable)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
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
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_UPDATEAPPROVAL_SET";
                    objCmd.Parameters.Add("APPTYPE_IN", OracleDbType.Varchar2).Value = AppType;
                    objCmd.Parameters.Add("RESCHEDULEDATE_IN", OracleDbType.Varchar2).Value = ResheduleDt;
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("APPAUTHCODE_IN", OracleDbType.Varchar2).Value = AppAuthCode;
                    objCmd.Parameters.Add("REC_APPLICABLE_IN", OracleDbType.Varchar2).Value = IsRecApplicable;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        //ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        //throw new Exception(ErrorMessage);
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

        public string UpdateHRApprovalStatus(string RequestID, string status, string Remarks,
                                           string AppAuthCode, string strReqEmpCode, string strBatchID, string strAddedBy)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
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
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_UPDATEHRAPPROVAL_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("APPAUTHCODE_IN", OracleDbType.Varchar2).Value = AppAuthCode;
                    objCmd.Parameters.Add("REQEMPCODE_IN", OracleDbType.Varchar2).Value = strReqEmpCode;
                    objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strAddedBy;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        //ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        //throw new Exception(ErrorMessage);
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

        public string CancelTrainigReScheduleRequest(string RequestID, string Remarks, string strAddedBy)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
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
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_CANCELTRNRESCHREQ_SET";
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
                    objCmd.Parameters.Add("BY_IN", OracleDbType.Varchar2).Value = strAddedBy;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        //ErrorMessage = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                        //throw new Exception(ErrorMessage);
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

        #endregion

        public DataTable Gettrainingobjective(string DESC, string TRDUR, string TRAINID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPORC_TRAININGOBJ_GET";
                    objCmd.Parameters.Add("DESC_IN", OracleDbType.Varchar2).Value = DESC;
                    objCmd.Parameters.Add("TRDUR_IN", OracleDbType.Varchar2).Value = TRDUR;
                    objCmd.Parameters.Add("TRAINID_IN", OracleDbType.Varchar2).Value = TRAINID;
                    objCmd.Parameters.Add("CUR_TRAININGOBJ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public DataTable SPROC_AUTHORITY_GET(string ECODE)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn;
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand(); ;
            strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRTRAINING.SPROC_AUTHORITY_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
                    objCmd.Parameters.Add("CUR_AUTHDET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDs = new DataTable();
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

        public string SendMailcount(string TRNGDETAILID, string SENDMAILCOUNTYPE)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRTRAINING.SPROC_SENDMAILCOUNT_SET";
                    objCmd.Parameters.Add("TRNGDETAILID_IN", OracleDbType.Varchar2).Value = TRNGDETAILID;
                    objCmd.Parameters.Add("SENDMAILTYPE_IN", OracleDbType.Varchar2).Value = SENDMAILCOUNTYPE;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 800).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    string strerr = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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
