using System;
using System.Data;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// Summary description for HrIntrnlTraining
/// </summary>
public class HrIntrnlTraining : IHrIntrnlTraining
{
    #region "Local Variables"
    private readonly IConnectionString objCnStr;
    private readonly IDataManagement oDataMgmt;

    DataSet ds = new DataSet();
    DataRow[] _datarow;
    //DataManagement oDataMgmt = new DataManagement();
    string qry;
    #endregion


    public HrIntrnlTraining(IConnectionString conn, IDataManagement _oDataMgmt)
    {
        oDataMgmt = _oDataMgmt;
        objCnStr = conn;
    }
    public DataTable GetMyTrainings(string strLoginEmpCode, string strTraining, string strTrainingStatus)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_MYTRAININGLIST_GET";
        oCmd.Parameters.Add("LOGGEDEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
        if (strTraining != "0")
        {
            oCmd.Parameters.Add("TRAINING_IN", OracleDbType.Varchar2).Value = strTraining;
        }
        if (strTrainingStatus != "All")
        {
            oCmd.Parameters.Add("TRAININGSTATUS_IN", OracleDbType.Varchar2).Value = strTrainingStatus;
        }
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    /// <summary>
    /// Get operation of user
    /// </summary>
    /// <param name="strecode"></param>
    /// <returns></returns>
    public string GetOperation(string strecode)
    {
        string strMsg;
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_OPERATION_GET";
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;

        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        oDataMgmt.ExecuteQuery(oCmd);

        strMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString());
        return strMsg;
    }

    public DataTable GetKI()
    {
        DataTable dt;
        qry = "SELECT ki.kicode,ki.sykiid,ki.active FROM SYKI ki ORDER BY KICODE";
        dt = oDataMgmt.GetDataTable(qry);
        return (dt);
    }

    public DataTable GetAssociateList(string strLoggedUser, string strOperation, string strDivision, string strDept, string strSection)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_ASSOCIATES_GET";
        oCmd.Parameters.Add("OP_IN", OracleDbType.Int32).Value = strOperation;
        oCmd.Parameters.Add("DIV_IN", OracleDbType.Int32).Value = strDivision;
        oCmd.Parameters.Add("DEPT_IN", OracleDbType.Int32).Value = strDept;
        oCmd.Parameters.Add("SEC_IN", OracleDbType.Int32).Value = strSection;
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strLoggedUser;
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable EmployeesTrainings(string strLoginEmpCode, string strEmpCode, string strOperation, string strDivision,
                                        string strDepartment, string strSection, string strStatus,
                                        string strKI)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_COUNTTRAININGLIST_GET";
        oCmd.Parameters.Add("LOGGEDEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
        //oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = strOperation;
        oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = strDivision;
        oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strDepartment;
        oCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
        //oCmd.Parameters.Add("TRAINING_IN", OracleDbType.Varchar2).Value = strTraining;
        //oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
        //oCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strKI;
        //oCmd.Parameters.Add("FROM_DT", OracleDbType.Varchar2).Value = strFromDt;
        //oCmd.Parameters.Add("TO_DT", OracleDbType.Varchar2).Value = strToDate;

        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetSchedAttdAsso(string strLoginEmpCode, string strEmpCode, string strOperation, string strDivision,
                                        string strDepartment, string strSection, string strTraining, string strStatus,
                                        string strKI, string strTrainingId, string strType)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_ATTNDASSOLIST_GET";
        oCmd.Parameters.Add("LOGGEDEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
        //oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = strOperation;
        oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = strDivision;
        oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strDepartment;
        oCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
        //oCmd.Parameters.Add("TRAINING_IN", OracleDbType.Varchar2).Value = strTraining;
        //oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
        //oCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strKI;
        oCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingId;
        oCmd.Parameters.Add("TYPE_IN", OracleDbType.Varchar2).Value = strType;
        //oCmd.Parameters.Add("TO_DT", OracleDbType.Varchar2).Value = strToDate;

        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetTrainingDetail(string strEmpCode, string strBatchID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPORC_TRAININGDETAIL_GET";

        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public string GetRecAuthAppAuth(string strecode)
    {
        //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strMsg;

        //objCnStr = new ConnectionString();
        strCn = objCnStr.getConnectingString();

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_RECAUTHAPPAUTH_GET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
                objCmd.Parameters.Add("REC_AUTH", OracleDbType.Varchar2, 11).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("APP_AUTH", OracleDbType.Varchar2, 11).Direction = ParameterDirection.Output;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();
                strMsg = objCmd.Parameters["REC_AUTH"].Value.ToString() + "#" + objCmd.Parameters["APP_AUTH"].Value.ToString();
                return strMsg;

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
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGDATES_GET";
        oCmd.Parameters.Add("BATCHID_IN", OracleDbType.Int32).Value = strBatchID;
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public int InsertTrainingReSchedule(string strRecAuth, string strAppAuth, string strEmpCode, string strBatchID,
        string strReason, string strAvailDate)
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
                string strSql = "PKG_HRTRAINING.SPROC_TRNGRESCHREQUEST_SET";
                objCmd.Parameters.Add("RECAUTH_IN", OracleDbType.Varchar2).Value = strRecAuth;
                objCmd.Parameters.Add("APPAUTH_IN", OracleDbType.Varchar2).Value = strAppAuth;
                objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
                objCmd.Parameters.Add("REASON_IN", OracleDbType.Varchar2).Value = strReason;
                objCmd.Parameters.Add("AVAILDATE_IN", OracleDbType.Varchar2).Value = strAvailDate;
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

    public DataTable GetRecMailId(string strRecAuth)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "SPROC_USER_DETAILS";
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strRecAuth;
        oCmd.Parameters.Add("CSR_USERDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetPendingReScheduleRequest(string strEmpCode)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRANRESCHPENDINGLIST_GET";
        oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
        oCmd.Parameters.Add("CUR_APPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetReScheduleRequestDetails(string strEmpCode, string strBatchID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRANINGDETAILS_GET";
        oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
        oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strBatchID;
        oCmd.Parameters.Add("CUR_APPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetPendingRequestList(string strEmpCode)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_PENDINGREQLIST_GET";
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
        oCmd.Parameters.Add("CUR_PENREQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public int UpdateTrngReSdlReqByRec(string strRemarks, string strAppAuth, string strRequestID,
        string strAvailableDate, string strStatus, string strRecAuthority, string strAppAuthority)
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
                string strSql = "PKG_HRTRAINING.SPROC_TRNGRESCHREQUESTREC_SET";
                objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                objCmd.Parameters.Add("RECAUTH_IN", OracleDbType.Varchar2).Value = strRecAuthority;
                objCmd.Parameters.Add("APPAUTH_IN", OracleDbType.Varchar2).Value = strAppAuthority;
                objCmd.Parameters.Add("APPEMPCODE_IN", OracleDbType.Varchar2).Value = strAppAuth;
                objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = strRequestID;
                objCmd.Parameters.Add("AVAILABLEDATE_IN", OracleDbType.Varchar2).Value = strAvailableDate;
                objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
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

    public DataTable GetTrainingRequestHistory(string strEmpCode)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_HRTRAINGREQHISTORY_GET";
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
        oCmd.Parameters.Add("CUR_REQHISTORY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetRequestHistory(string RequestID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_REQHISTORY_GET";
        oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
        oCmd.Parameters.Add("CUR_REQHISTORY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetHistoryApprovalList(String EmpCode)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_APPHISTORYLIST_GET";
        oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
        oCmd.Parameters.Add("CUR_APPHISTORYLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public int UpdateTrngReSdlReqByApp(string strRemarks, string strRequestID, string strStatus)
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
                string strSql = "PKG_HRTRAINING.SPROC_TRNGRESCHREQUESTAPP_SET";
                objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = strRequestID;
                objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
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

    public DataTable GetPendingTrainingReScheduleHR(String strLoginEmpCode, string strEmpCode, string strEmpName, string strOperation, string strDivision,
                                        string strDepartment, string strSection, string strTraining, string strStatus,
                                        string strKI, string strFromDt, string strToDate)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_PENDINGLISTFORHR_GET";
        oCmd.Parameters.Add("LOGGEDEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
        oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = strOperation;
        oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = strDivision;
        oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strDepartment;
        oCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
        oCmd.Parameters.Add("TRAINING_IN", OracleDbType.Varchar2).Value = strTraining;
        oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
        oCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strKI;
        oCmd.Parameters.Add("FROM_DT", OracleDbType.Varchar2).Value = strFromDt;
        oCmd.Parameters.Add("TO_DT", OracleDbType.Varchar2).Value = strToDate;

        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable EmployeeDesignation(string strEmpCode)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_EMPDESIG_GET";
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataSet get_DesignationName()
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds = new DataSet();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_DESIGNATIONNAME_GET";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);
    }

    public DataSet get_DepartmentName()
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds = new DataSet();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_DEPTNAME_GET";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);
    }

    public DataSet get_KI()
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds = new DataSet();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_KI_GET";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);
    }

    public DataSet get_SectionName(int deptid)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strConn;

        OracleConnection objCn;
        OracleCommand objCmd;


        strConn = objCnStr.getConnectingString();

        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_GET_SECTION";
                objCmd.Parameters.Add("DEPTID_IN", OracleDbType.Int32).Value = deptid;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                DataSet objDs = new DataSet();
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

    public DataSet get_TrainingName()
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds = new DataSet();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGNAME_GET";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);
    }

    public DataSet get_AllTrainers()
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds = new DataSet();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_ALLEMPLOYEES";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);
    }

    public DataSet get_AllTrainers(int empcode)
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds = new DataSet();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_SELECTEMPLOYEES";
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = empcode;
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);
    }

    public DataSet get_TrainerName()
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds = new DataSet();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAINERNAME_GET";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);
    }

    public DataSet DisplayTrainerDetails()
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds = new DataSet();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAINER_MASTER";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        //GET DATA FROM DATA ACCESS LAYER
        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);
    }

    public int UpdateTrngMasterData(string trainingname, string trainingperiod, int status, int trainingid, int modifiedby)
    {
        //ConnectionString objCnStr = new ConnectionString(); ;
        string strCn = objCnStr.getConnectingString(); ;
        OracleCommand objCmd;
        int errMsg = 0;
        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGMASTER";
                objCmd.Parameters.Add("TRAININGNAME_IN", OracleDbType.Varchar2).Value = trainingname;
                objCmd.Parameters.Add("TRAININGPERIOD_IN", OracleDbType.Varchar2).Value = trainingperiod;
                objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32, 4).Value = status;
                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Int32, 4).Value = trainingid;
                objCmd.Parameters.Add("MODIFIEDBY_IN", OracleDbType.Int32, 9).Value = modifiedby;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();

                //IF ERROR OCCURED
                if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
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
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
        return (errMsg);

    }

    public int InsertTrngMasterData(string trainingname, string trainingperiod, int ki, int status, int addedby)
    {
        //ConnectionString objCnStr = new ConnectionString(); ;
        string strCn = objCnStr.getConnectingString(); ;
        OracleCommand objCmd;
        int errMsg = 0;
        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_ADDTRAININGMASTER";
                objCmd.Parameters.Add("TRAININGNAME_IN", OracleDbType.Varchar2).Value = trainingname;
                objCmd.Parameters.Add("TRAININGPERIOD_IN", OracleDbType.Varchar2).Value = trainingperiod;
                objCmd.Parameters.Add("KI_IN", OracleDbType.Int32).Value = ki;
                objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = status;
                objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = addedby;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();

                //IF NO ERROR OCCURED
                if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
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
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
        return (errMsg);

    }

    public int InsertNewTrainer(int empcode, int trainingid, int status, int addedby)
    {
        //ConnectionString objCnStr = new ConnectionString(); ;
        string strCn = objCnStr.getConnectingString(); ;
        OracleCommand objCmd;
        int errMsg = 0;
        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_INSERTTRAINER";
                objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32, 4).Value = status;
                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Int32, 4).Value = trainingid;
                objCmd.Parameters.Add("TRAINERCODE_IN", OracleDbType.Int32, 9).Value = empcode;
                objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32, 9).Value = addedby;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();

                //IF ERROR OCCURED
                if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
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
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
        return (errMsg);

    }

    public int UpdateExistingTrainer(int trainingid, int status, int modifiedby, int trainerempcode)
    {
        //ConnectionString objCnStr = new ConnectionString(); ;
        string strCn = objCnStr.getConnectingString(); ;
        OracleCommand objCmd;
        int errMsg = 0;
        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_UPDATETRAINER";
                objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32, 4).Value = status;
                objCmd.Parameters.Add("MODIFIEDBY_IN", OracleDbType.Int32, 9).Value = modifiedby;
                objCmd.Parameters.Add("TRAINEREMPCODE_IN", OracleDbType.Int32, 9).Value = trainerempcode;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();

                //IF ERROR OCCURED
                if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
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
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
        return (errMsg);

    }

    public DataTable GetExtrnlTrngsOfEmp(string strLoginEmpCode, string strEmpCode, string strOperation, string strDivision,
                                        string strDepartment, string strSection)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_EXTTRNGLSTOFEMP_GET";
                objCmd.Parameters.Add("LOGGEDEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                //objCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
                objCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = strOperation;
                objCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = strDivision;
                objCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strDepartment;
                objCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
                //objCmd.Parameters.Add("TRAINING_IN", OracleDbType.Varchar2).Value = strTraining;
                //objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                //objCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strKI;
                //objCmd.Parameters.Add("FROM_DT", OracleDbType.Varchar2).Value = strFromDt;
                //objCmd.Parameters.Add("TO_DT", OracleDbType.Varchar2).Value = strToDate;

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

    public DataTable GetTodayTrngsOfEmp(string strLoginEmpCode, string strEmpCode, string strOperation, string strDivision,
                                        string strDepartment, string strSection)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_CURNTTTRNGLSTOFEMP_GET";
                objCmd.Parameters.Add("LOGGEDEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                objCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = strOperation;
                objCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = strDivision;
                objCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strDepartment;
                objCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
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


    /// This function is used to get the details of training master.
    public DataTable GetTrangMasterDetails(string strTrainingid)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGMASTERDETAIL_GET";
        oCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Int32).Value = strTrainingid;
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        //GET DATA FROM DATA ACCESS LAYER
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }


    /// This function is used to set the details of selected training.
    public int SetTrainingMaster(string strTrainingName, string strTrainingPeriod, string strExperience,
         string strstatus, string strmodifiedby, string strTrainingid, string strdesignation, string strTrObjective, string evaluation)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGMASTERDETAIL_SET";
        oCmd.Parameters.Add("TRAININGNAME_IN", OracleDbType.Varchar2).Value = strTrainingName;
        oCmd.Parameters.Add("TRAININGPERIOD_IN", OracleDbType.Varchar2).Value = strTrainingPeriod;
        oCmd.Parameters.Add("EXPERIENCE_IN", OracleDbType.Varchar2).Value = strExperience;
        oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
        oCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingid;
        oCmd.Parameters.Add("MODIFIEDBY_IN", OracleDbType.Varchar2).Value = strmodifiedby;
        oCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strdesignation;
        oCmd.Parameters.Add("TRAININGOBJECTIVE_IN", OracleDbType.Varchar2).Value = strTrObjective;
        oCmd.Parameters.Add("EVALUATION_IN", OracleDbType.Varchar2).Value = evaluation;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        oDataMgmt.ExecuteQuery(oCmd);

        if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    /// This function is used to get the details of training master.
    public DataTable GetTrngMasterDesgDetails(string strTrainingid)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRNGMASTERDESGDETAIL_GET";
        oCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Int32).Value = strTrainingid;
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }
    /// This function is used to get the details of training master.
    public DataTable GetTrainerMasterByID(string strTrainingMasterId)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAINERMASTERDETAILS_GET";
        oCmd.Parameters.Add("TRAINERMASTERID_IN", OracleDbType.Int32).Value = strTrainingMasterId;
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    //To get all Training name,id for adding question to them.
    public DataTable get_AllTrainingName()
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPORC_TRAINING_GET";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    //To get all question according to id in HrTrainingQuestionnaire
    public DataTable get_AllQuestionHrTraining(string strHrTrainingId)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_HRTRAINING_QUESLIST_GET";
        oCmd.Parameters.Add("TRAININGID_IN",OracleDbType.Varchar2).Value = strHrTrainingId;
        oCmd.Parameters.Add("CUR_HRQUE",OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return dt;
    }
    public string set_HrTrainingQuestion(string strTrainingId, string strQuestion, string strAddedby, string strStatus, string strHrtrainingQueId, string strFlag)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_HRTRAININGQUESTION_SET";
        oCmd.Parameters.Add("QUESTION_IN", OracleDbType.Varchar2).Value = strQuestion;
        oCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Int32, 9).Value = strTrainingId == "" ? (object)DBNull.Value : strTrainingId;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32,9).Value = strAddedby;
        oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strStatus;
        oCmd.Parameters.Add("HRTRAININGQUEID_IN", OracleDbType.Int32, 9).Value = strHrtrainingQueId == "" ? (object)DBNull.Value : strHrtrainingQueId;
        oCmd.Parameters.Add("FLAG_IN", OracleDbType.Varchar2).Value = strFlag; 
       // oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32,4).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        oDataMgmt.ExecuteQuery(oCmd);


        if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }

    //To get all active-deactive training
    public DataTable get_AllTraining()
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPORC_ALLTRAINING_GET";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }
    //To get question for updation process.
    public DataTable get_QuestionName(string strQueID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_GETQUESTION";
        oCmd.Parameters.Add("QUESTIONID_IN", OracleDbType.Varchar2).Value = strQueID;
        oCmd.Parameters.Add("CUR_QUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    //To search the pending hr Training detail
    public DataTable get_PendingHrTranDetail(String strHrtrainingId, string strTrType,string strStatus, string strStartdate, string strEnddate)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGRESHEDULE";
        oCmd.Parameters.Add("HRTRAININGID_IN", OracleDbType.Varchar2).Value = strHrtrainingId;
        oCmd.Parameters.Add("TRAININGSTATUS_IN", OracleDbType.Varchar2).Value = strTrType;
        oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strStatus;
        oCmd.Parameters.Add("STARTDATE_IN", OracleDbType.Varchar2).Value = strStartdate;
        oCmd.Parameters.Add("ENDDATE_IN", OracleDbType.Varchar2).Value = strEnddate;

        oCmd.Parameters.Add("CUR_RESHEDULE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    //To get plannedvsActual training information
    public DataTable get_PlanActualInformation(string strEmpcode)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_ACTUALPLANNED_TRAINING";
        oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
        oCmd.Parameters.Add("CUR_TR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    //IN USER PANEL SHOW ALL QUESTION BASED ON QUEID FOR EVALUTION
    public DataTable get_TrainingQuestion(string strTrainingId)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_GETTRAINING_QUESTION";
        oCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingId;
        oCmd.Parameters.Add("CUR_QUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }
}
