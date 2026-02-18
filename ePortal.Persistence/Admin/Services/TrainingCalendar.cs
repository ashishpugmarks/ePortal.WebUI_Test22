using System;
using System.Data;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;

/// <summary>
/// Summary description for TrainingCalendar
/// </summary>
public class TrainingCalendar: ITrainingCalendar
{
    #region "Local Variables"
        //DataManagement oDataMgmt = new DataManagement();
    private readonly IConnectionString objCnStr;
    private readonly IDataManagement oDataMgmt;
    #endregion

    public TrainingCalendar(IConnectionString conn, IDataManagement _oDataMgmt)
    {
        oDataMgmt = _oDataMgmt;
        objCnStr = conn;
    }
    public string GetIsHolidayOrNot(string strdate,string strSySiteID)
    {
        //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strErrMsg,IsWorking;
        IsWorking = "0";

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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_IsHolidayOrNot_GET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = strdate;
                objCmd.Parameters.Add("SYSITEID_IN", OracleDbType.Varchar2).Value = strSySiteID;
                objCmd.Parameters.Add("ISWORKINGDAY", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
                
                objCmd.ExecuteNonQuery();

                IsWorking = Convert.ToString(objCmd.Parameters["ISWORKINGDAY"].Value);
                return IsWorking;

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


    public DataTable GetTrngDataInCalndr(string strdate)
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
                string strSql = "PKG_HRTRAINING.SPORC_TRAININGDATA_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("TRAININGDATE_IN", OracleDbType.Varchar2).Value = strdate;
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

    public DataTable GetBatchDetail(string strbatchid)
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
                string strSql = "PKG_HRTRAINING.SPORC_BATCHDETAIL_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strbatchid;
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

    public DataTable GetEvalutionDetail(string strEmp,string strTransactionID)
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
                string strSql = "PKG_HRTRAINING.SPORC_EVALUTIONDETAIL_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmp;
                objCmd.Parameters.Add("HTTRANSACTIONID_IN", OracleDbType.Varchar2).Value = strTransactionID;
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

    public DataTable GetFeedbackDetail(string strEmp, string strTransactionID)
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
                string strSql = "PKG_HRTRAINING.SPORC_FEEDBACKETAIL_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmp;
                objCmd.Parameters.Add("HTTRANSACTIONID_IN", OracleDbType.Varchar2).Value = strTransactionID;
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


    public DataTable GetAssoOfBatch(string strbatchid)
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
                string strSql = "PKG_HRTRAINING.SPORC_BATCHASSODETAIL_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strbatchid;
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

    public DataTable GetAllTraining()
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
                string strSql = "PKG_HRTRAINING.SPORC_TRAINING_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable GetPendingAssoList(string strTrainingId)
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
                string strSql = "PKG_HRTRAINING.SPORC_PENDINGASSOCIATES_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingId;
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

    public string SetTraninigBatch(string strinternaltraining, string strfrmdate, string strtodate,
        string strstarttime, string strendtime, string strtrainer1, string strtrainer2,
        string strvenue, string strpendingasscosiates, string straddedby,
        string strki, string strcomment)
    {
        //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strErrMsg;

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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGBATCH_SET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("INTERNALTRAINING_IN", OracleDbType.Varchar2).Value = strinternaltraining;
                objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strfrmdate;
                objCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strtodate;
                objCmd.Parameters.Add("STARTTIME_IN", OracleDbType.Varchar2).Value = strstarttime;
                objCmd.Parameters.Add("ENDTIME_IN", OracleDbType.Varchar2).Value = strendtime;
                objCmd.Parameters.Add("TRAINER1_IN", OracleDbType.Varchar2).Value = strtrainer1;
                objCmd.Parameters.Add("TRAINER2_IN", OracleDbType.Varchar2).Value = strtrainer2;
                objCmd.Parameters.Add("VENUE_IN", OracleDbType.Varchar2).Value = strvenue;
                objCmd.Parameters.Add("PENDINGASSOCIATES_IN", OracleDbType.Varchar2).Value = strpendingasscosiates;
                objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = straddedby;
                objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strki;
                objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strcomment;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                return strErrMsg;

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

    public DataTable GetScheduledTrainingId(string strdate)
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
                string strSql = "PKG_HRTRAINING.SPORC_NOOFBATCH_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = strdate;
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

    public string EditTraninigBatch(string strbatchid, string strfrmdate, string strtodate, string strstarttime,
        string strendtime, string strtrainer1, string strtrainer2, string strvenue, string strpendingasscosiates,
        string straddedby, string strki, string strcomment)
    {
        //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strErrMsg;

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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_EDITTRAININGBATCH_SET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Int32).Value = strbatchid;
                objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strfrmdate;
                objCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strtodate;
                objCmd.Parameters.Add("STARTTIME_IN", OracleDbType.Varchar2).Value = strstarttime;
                objCmd.Parameters.Add("ENDTIME_IN", OracleDbType.Varchar2).Value = strendtime;
                objCmd.Parameters.Add("TRAINER1_IN", OracleDbType.Varchar2).Value = strtrainer1;
                objCmd.Parameters.Add("TRAINER2_IN", OracleDbType.Varchar2).Value = strtrainer2;
                objCmd.Parameters.Add("VENUE_IN", OracleDbType.Varchar2).Value = strvenue;
                objCmd.Parameters.Add("PENDINGASSOCIATES_IN", OracleDbType.Varchar2).Value = strpendingasscosiates;
                objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = straddedby;
                objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strki;
                objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strcomment;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                return strErrMsg;

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

    public DataTable GetAssoDetails(string RequestDetailID)
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
                string strSql = "PKG_HRTRAINING.SPORC_ASSOCIATEDETAIL_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("DETAILID_IN", OracleDbType.Varchar2).Value = RequestDetailID;
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

    public string RescheduleCount(string strecode, string strtrainingid)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_RESCHEDULECOUNT_GET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strtrainingid;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

    public DataTable GetAvailableDataList(string strtrainingid, string strdate)
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
                string strSql = "PKG_HRTRAINING.SPORC_SCHEDULEDDATELIST_GET";
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strtrainingid;
                objCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = strdate;
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

    public string EditAssociateDetail(string strdetailid, string strstatus, string strRescheduledate, string strtrainingid, string straddedby)
    {
        //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strErrMsg;

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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_EDITASSOCITEDETAIL_SET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("DETAILID_IN", OracleDbType.Int32).Value = strdetailid;
                objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
                objCmd.Parameters.Add("RESCHEDULEDATE_IN", OracleDbType.Varchar2).Value = strRescheduledate;
                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strtrainingid;
                objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = straddedby;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                return strErrMsg;

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

    public string GetTrainingDuration(string strtrainingid)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRAININGDURATION_GET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strtrainingid;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

    public DataTable GetAllSelectedBatches(string strTrainigId, string strDate, string strKi)
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
                string strSql = "PKG_HRTRAINING.SPROC_SELECTEDBATCHES_GET";
                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainigId;
                if (strDate != "")
                {
                    objCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = strDate;
                }
                objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKi;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable SetMailCounterGetMailId(string strbatchid, string straddedby)
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
                string strSql = "PKG_HRTRAINING.SPROC_SETMAILDATA_GETMAILID";
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strbatchid;
                objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = straddedby;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable GET_SeniorHeadMail( string strEmpcode)
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
                string strSql = "PKG_HRTRAINING.SPROC_EVALUTION_GETMAILID";
                objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable GetEmailDetails(string strbatchid)
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
                string strSql = "PKG_HRTRAINING.SPROC_EMAILDETAILS_GET";
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strbatchid;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable GetAssoEmailDetails(string strComdtlId)
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
                string strSql = "PKG_HRTRAINING.SPROC_ASSOEMAILDETAILS_GET";
                objCmd.Parameters.Add("COMDTLID_IN", OracleDbType.Varchar2).Value = strComdtlId;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable GetAssoDetailsForEmail(string strbatchid)
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
                string strSql = "PKG_HRTRAINING.SPROC_ASSODETAILSFORMAIL_GET";
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strbatchid;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable GetAssoEvalEmail(string strEmpcode,string strTransactionId)
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
                string strSql = "PKG_HRTRAINING.SPROC_ASSEVALMAIL_GET";
                objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                objCmd.Parameters.Add("HRTRAININGTRANSACTIONID_IN", OracleDbType.Varchar2).Value = strTransactionId;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable GetScheduledAssoList(string strdate)
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
                string strSql = "PKG_HRTRAINING.SPROC_LISTOFSCHEDULEASSO_GET";
                objCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = strdate;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public string InsertFeedbackfrm(string strBatchID, string strEmpCode, string strrdolst1, string strrdolst2,
        string strrdolst3, string strrdolst4, string strrdolst5, string strrdolst6, string strrdolst7_1,
        string strrdolst7_2, string strrdolst8_1, string strrdolst8_2, string strrdolst9_1, string strrdolst9_2,
        string strrdolst10_1, string strrdolst10_2, string strrdolst11_1, string strrdolst11_2, string strrdolst12,
        string strrdolst13, string strrdolst14, string strtxtlike, string strtxttopics, string strtxtfeedback, string strtxtImprovementFeedback)
    {
        //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strErrMsg;

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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRNGFEEDBACKSCORE_SET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                objCmd.Parameters.Add("RDOLST1_IN", OracleDbType.Varchar2).Value = strrdolst1;
                objCmd.Parameters.Add("RDOLST2_IN", OracleDbType.Varchar2).Value = strrdolst2;
                objCmd.Parameters.Add("RDOLST3_IN", OracleDbType.Varchar2).Value = strrdolst3;
                objCmd.Parameters.Add("RDOLST4_IN", OracleDbType.Varchar2).Value = strrdolst4;
                objCmd.Parameters.Add("RDOLST5_IN", OracleDbType.Varchar2).Value = strrdolst5;
                objCmd.Parameters.Add("RDOLST6_IN", OracleDbType.Varchar2).Value = strrdolst6;
                objCmd.Parameters.Add("RDOLST7_1_IN", OracleDbType.Varchar2).Value = strrdolst7_1;
                objCmd.Parameters.Add("RDOLST7_2_IN", OracleDbType.Varchar2).Value = strrdolst7_2;
                objCmd.Parameters.Add("RDOLST8_1_IN", OracleDbType.Varchar2).Value = strrdolst8_1;
                objCmd.Parameters.Add("RDOLST8_2_IN", OracleDbType.Varchar2).Value = strrdolst8_2;
                objCmd.Parameters.Add("RDOLST9_1_IN", OracleDbType.Varchar2).Value = strrdolst9_1;
                objCmd.Parameters.Add("RDOLST9_2_IN", OracleDbType.Varchar2).Value = strrdolst9_2;
                objCmd.Parameters.Add("RDOLST10_1_IN", OracleDbType.Varchar2).Value = strrdolst10_1;
                objCmd.Parameters.Add("RDOLST10_2_IN", OracleDbType.Varchar2).Value = strrdolst10_2;
                objCmd.Parameters.Add("RDOLST11_1_IN", OracleDbType.Varchar2).Value = strrdolst11_1;
                objCmd.Parameters.Add("RDOLST11_2_IN", OracleDbType.Varchar2).Value = strrdolst11_2;
                objCmd.Parameters.Add("RDOLST12_IN", OracleDbType.Varchar2).Value = strrdolst12;
                objCmd.Parameters.Add("RDOLST13_IN", OracleDbType.Varchar2).Value = strrdolst13;
                objCmd.Parameters.Add("RDOLST14_IN", OracleDbType.Varchar2).Value = strrdolst14;
                objCmd.Parameters.Add("COMMENT1_IN", OracleDbType.Varchar2).Value = strtxtlike;
                objCmd.Parameters.Add("COMMENT2_IN", OracleDbType.Varchar2).Value = strtxttopics;
                objCmd.Parameters.Add("COMMENT3_IN", OracleDbType.Varchar2).Value = strtxtfeedback;
                objCmd.Parameters.Add("COMMENT4_IN", OracleDbType.Varchar2).Value = strtxtImprovementFeedback;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                var err = Convert.ToString(objCmd.Parameters["ERRMSG"].Value.ToString());
                return strErrMsg;

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

    public DataTable GetFeedbackDetails(string strBatchid, string strempcode)
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
                string strSql = "PKG_HRTRAINING.SPROC_TRNGFEEDBACKDETAIL_GET";
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchid;
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strempcode;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable GetFeedbackRpt(string strOperation, string strDivision, string strDepartment,
        string strSection, string strTraining, string strKi)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRNGFEEDBACKDETAIL_GET";
                if (strOperation != "0")
                {
                    objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperation;
                }
                if (strOperation != "0")
                {
                    objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperation;
                }
                if (strDivision != "0")
                {
                    objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivision;
                }
                if (strDepartment != "0")
                {
                    objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartment;
                }
                if (strSection != "0")
                {
                    objCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSection;
                }
                if (strTraining != "0")
                {
                    objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTraining;
                }
                objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKi;
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

    public string CancelTraninigBatch(string strbatchid, string straddedby)
    {
        //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strErrMsg;

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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_CANCELTRAININGBATCH_SET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Int32).Value = strbatchid;
                objCmd.Parameters.Add("CANCELLEDBY_IN", OracleDbType.Varchar2).Value = straddedby;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                return strErrMsg;

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

    public DataTable GetTrainerEmailID(string strbatchid)
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
                string strSql = "PKG_HRTRAINING.SPORC_TRAINEREMAILID_GET";
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strbatchid;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataTable Get_EvalutionTrainerEmailID(string strEmp)
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
                string strSql = "PKG_HRTRAINING.SPORC_EVALTRAINEREMAILID_GET";
                objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmp;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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
    public DataTable Get_FeedbackEmpEmailID(string strEmp)
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
                string strSql = "select t.emailid as MAILID from ademployee t where t.adempcode='" + strEmp + "'";
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr= new OracleDataAdapter(objCmd);
                objAdr.Fill(objDt);
                return objDt;
                //string abc= objCmd.ex
                //return abc;


                //string strSql = "PKG_HRTRAINING.SPORC_EVALTRAINEREMAILID_GET";
                //objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmp;
                //objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                //objCmd.CommandText = strSql;
                //objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                //OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                //objAdr.Fill(objDt);
                //return objDt;
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



    //--Evaluation  module related function----

    //public string InsertEvaluationfrm(string strBatchID, string strEmpCode, string strrdolst1, string strrdolst2,
    //        string strrdolst3, string strlogincode)
    //{
    //    ConnectionString objCnStr;
    //    string strCn;
    //    OracleCommand objCmd;
    //    string strErrMsg;

    //    objCnStr = new ConnectionString();
    //    strCn = objCnStr.getConnectingString();

    //    using (OracleConnection objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strCn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();
    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRNGEVALUATIONSCORE_SET";
    //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;

    //            objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
    //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
    //            objCmd.Parameters.Add("RDOLST1_IN", OracleDbType.Varchar2).Value = strrdolst1;
    //            objCmd.Parameters.Add("RDOLST2_IN", OracleDbType.Varchar2).Value = strrdolst2;
    //            objCmd.Parameters.Add("RDOLST3_IN", OracleDbType.Varchar2).Value = strrdolst3;
    //            objCmd.Parameters.Add("LOGINCODE_IN", OracleDbType.Varchar2).Value = strlogincode;
    //            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
    //            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

    //            objCmd.ExecuteNonQuery();
    //            strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
    //            return strErrMsg;

    //        }
    //        finally
    //        {
    //            if (objCn != null)
    //            {
    //                objCn.Close();
    //            }
    //        }
    //    }
    //}

    //INSERT EVALUTION SCORE VALUE.
    public string InsertEvaluationfrm(string strBatchID, string strEmpCode,string strlogincode,string strXml,string strGrade)
        //string strEmpName,string strTraining,string strVenue,string strTrainingstartDate,string strTrainingEndDate,
        //string strTrainer1,string strTrainer2,string strStartTime,string strEndTime,string strTrainingId)

    {
        //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strErrMsg;

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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRNGEVALUATIONSCORE_SET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("BATCHID_IN", OracleDbType.Varchar2).Value = strBatchID;
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                objCmd.Parameters.Add("RDOLST1_IN", OracleDbType.Varchar2).Value = "1";
                objCmd.Parameters.Add("RDOLST2_IN", OracleDbType.Varchar2).Value = "1";
                objCmd.Parameters.Add("GRADE", OracleDbType.Varchar2).Value = strGrade;
                objCmd.Parameters.Add("LOGINCODE_IN", OracleDbType.Varchar2).Value = strlogincode;
                objCmd.Parameters.Add("XML_IN", OracleDbType.Varchar2).Value = strXml;
                //objCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpName;
                //objCmd.Parameters.Add("TRAINING_IN", OracleDbType.Varchar2).Value = strTraining;
                //objCmd.Parameters.Add("VENUE_IN", OracleDbType.Varchar2).Value = strVenue;
                //objCmd.Parameters.Add("TRAININGSTARTDATE_IN", OracleDbType.Varchar2).Value = strTrainingstartDate;
                //objCmd.Parameters.Add("TRAININGENDDATE_IN", OracleDbType.Varchar2).Value = strTrainingEndDate;
                //objCmd.Parameters.Add("TRAINER1_IN", OracleDbType.Varchar2).Value = strTrainer1;
                //objCmd.Parameters.Add("TRAINER2_IN", OracleDbType.Varchar2).Value = strTrainer2;
                //objCmd.Parameters.Add("TRSTARTTIME_IN", OracleDbType.Varchar2).Value = strStartTime;
                //objCmd.Parameters.Add("TRENDTIME_IN", OracleDbType.Varchar2).Value = strEndTime;
                //objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingId;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                 string strRes = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                return strErrMsg;

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







    // This function is used to get the Evaluation list.
    public DataTable GetEvaluationList(string strLoginEmpCode)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_EVALUATIONLIST_GET";
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    //GET DEATIL OF EVALUTION EAMIL SENT
    //public DataTable GetEvaluationMail(string strLoginEmpCode, string strTrainingId,string strKiID,string strStartDate,string strEndDate)
    ////public DataTable GetEvaluationMail(string strLoginEmpCode)
    //{
    //    OracleCommand oCmd = new OracleCommand();
    //    DataTable dt = new DataTable();
    //    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //    oCmd.CommandText = "PKG_HRTRAINING.SPROC_EAVLUTIONMAIL_SEND";
    //    oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strLoginEmpCode;
    //    oCmd.Parameters.Add("HRTRAININGID_IN", OracleDbType.Varchar2).Value = strTrainingId;
    //    oCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = strKiID;
    //    oCmd.Parameters.Add("STARTDATE_IN", OracleDbType.Varchar2).Value = strStartDate;
    //    oCmd.Parameters.Add("ENDDATE_IN", OracleDbType.Varchar2).Value = strEndDate;
    //    oCmd.Parameters.Add("CUR_QUE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

    //    dt = oDataMgmt.GetDataTable(oCmd);
    //    return (dt);
    //}

    //GET DETAIL OF EMAIL ID TO WHOM EVALUTION MAIL WILL BE SENT.
    public DataTable GetEvaluationMail()
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HRTRAINING.SPROC_NEWEVALUATIONLIST_GET";
        oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetEvaluationRpt(string strTraining, string strKi, string strgrade, string strbatch)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRNGEVALDETAIL_GET";

                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTraining;
                objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKi;
                objCmd.Parameters.Add("GRADE_IN", OracleDbType.Varchar2).Value = strgrade;
                objCmd.Parameters.Add("BATCH_IN", OracleDbType.Varchar2).Value = strbatch;
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

    public DataTable GetPendingEvalutionList(string strTraining, string strKi, string strStatus, string strbatch, string strEmpCode)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_NEWEVALUATIONLIST_GET";
                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTraining;
                objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKi;
                objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                objCmd.Parameters.Add("BATCH_IN", OracleDbType.Varchar2).Value = strbatch;
                objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
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

    public DataSet GetEvaluationdetail(string strTrngevalid)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPORC_ETRAININGDETAIL_GET";

                objCmd.Parameters.Add("EVALUATIONID_IN", OracleDbType.Varchar2).Value = strTrngevalid;
                objCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("CUR_EVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public string UpdateRetrainingstatus(string strTrngevalid, int strretrngstatus)
    {
        //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strErrMsg;

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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_RETRNGSTATUS_SET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("EVALID_IN", OracleDbType.Varchar2).Value = strTrngevalid;
                objCmd.Parameters.Add("RETRNGSTATUS_IN", OracleDbType.Varchar2).Value = strretrngstatus;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                return strErrMsg;

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

    public DataTable GetFeedbackStatus(string strTraining, string strKi, string strbatchid, string strstatus)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPROC_TRNGFEEDBACKSTATUS_GET";

                objCmd.Parameters.Add("TRAININGID_IN", OracleDbType.Varchar2).Value = strTraining;
                objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKi;
                objCmd.Parameters.Add("BATCH_IN", OracleDbType.Varchar2).Value = strbatchid;
                objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
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





    public DataSet GetEvaluationquestionwithanswerlist(string strevalID)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPORC_EVALANSWER_GET";
                objCmd.Parameters.Add("EVALID_IN", OracleDbType.Varchar2).Value = strevalID;
                objCmd.Parameters.Add("CUR_EVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

    public DataSet GET_EVALANSWERINUSERHAND(string TRANSID)
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
                objCmd.CommandText = "PKG_HRTRAINING.SPORC_EVALANSWERFORUSER_GET";
                objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Varchar2).Value = TRANSID;
                objCmd.Parameters.Add("CUR_EVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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
}
