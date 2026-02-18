using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Admin.Services;

using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Persistence.Admin.Services
{
    public class LeaveApps : ILeaveApps
    {
        #region "Local Variables"
        DataSet ds = new DataSet();
        DataTable objDt;
        OracleCommand cmd;
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;

        //OracleCommand oCmd;
        //DataSet ds;
        //DataTable Dt;
        //OracleConnection objConn;
        //string strConn;
        //string strErrMsg = string.Empty;
        #endregion

        public LeaveApps(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }

        public LeaveApps()
        {
        }
        
/// <summary>
    /// PKG_LEAVEAPPAUTH.SPROC_AUTHCODES_GET
    /// Returns the head IDs (DEPARTMENTHEADID, DIVISIONHEADID, VPHEADID) for the given empCode.
    /// </summary>
    public DataTable GetEmployeeAuthCodes(string empCode)
    {
        string strCn = objCnStr.getConnectingString();

        using (var objCn = new OracleConnection(strCn))
        {
            try
            {
                objCn.Open();
                using (var objCmd = new OracleCommand("PKG_LEAVEAPPAUTH.SPROC_AUTHCODES_GET", objCn))
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.BindByName = true;

                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = empCode;
                    objCmd.Parameters.Add("CUR_AUTHCODES", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (var objAdr = new OracleDataAdapter(objCmd))
                    {
                        var dt = new DataTable();
                        objAdr.Fill(dt);
                        return dt;
                    }
                }
            }
            finally
            {
                if (objCn.State != ConnectionState.Closed)
                    objCn.Close();
            }
        }
    }

    /// <summary>
    /// PKG_LEAVEAPPAUTH.SPROC_GETEMPDATA
    /// Returns a data reader with employee name, designation, division/department/vp IDs, and modifiedby.
    /// Caller is responsible for disposing the IDataReader.
    /// </summary>
    public IDataReader GetEmployeeData(string empCode)
    {
        string strCn = objCnStr.getConnectingString();
        var objCn = new OracleConnection(strCn);
        objCn.Open(); // Keep open until reader disposed

        var objCmd = new OracleCommand("PKG_LEAVEAPPAUTH.SPROC_GETEMPDATA", objCn)
        {
            CommandType = CommandType.StoredProcedure,
            BindByName = true
        };

        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = empCode;
        objCmd.Parameters.Add("CUR_LEAVEAPPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        // CommandBehavior.CloseConnection ensures connection closes when reader is disposed
        return objCmd.ExecuteReader(CommandBehavior.CloseConnection);
    }

    /// <summary>
    /// PKG_LEAVEAPPAUTH.SPROC_EMPRECOMMAUTH_GET
    /// Returns a DataSet of eligible recommendation/approval authorities for given div/emp/dept/vp IDs.
    /// </summary>
    public DataSet GetEligibleAuthorities(string divId, string empCode, string deptId, string vpId)
    {
        string strCn = objCnStr.getConnectingString();

        using (var objCn = new OracleConnection(strCn))
        {
            try
            {
                objCn.Open();
                using (var objCmd = new OracleCommand("PKG_LEAVEAPPAUTH.SPROC_EMPRECOMMAUTH_GET", objCn))
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.BindByName = true;

                    objCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = divId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empCode;
                    objCmd.Parameters.Add("Dept_IN", OracleDbType.Varchar2).Value = deptId;
                    objCmd.Parameters.Add("VP_IN", OracleDbType.Varchar2).Value = vpId;
                    objCmd.Parameters.Add("CUR_LEAVEAPPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (var objAdr = new OracleDataAdapter(objCmd))
                    {
                        var ds = new DataSet();
                        objAdr.Fill(ds);
                        return ds;
                    }
                }
            }
            finally
            {
                if (objCn.State != ConnectionState.Closed)
                    objCn.Close();
            }
        }
    }

    /// <summary>
    /// PKG_LEAVEAPPAUTH.SPROC_GETAPPAUTHORITY
    /// Returns a data reader for the current authorities (supervisorempcode, supsupervisorempcode) for an employee.
    /// Caller disposes the reader.
    /// </summary>
    public IDataReader GetCurrentAuthorities(string empCode)
    {
        string strCn = objCnStr.getConnectingString();
        var objCn = new OracleConnection(strCn);
        objCn.Open();

        var objCmd = new OracleCommand("PKG_LEAVEAPPAUTH.SPROC_GETAPPAUTHORITY", objCn)
        {
            CommandType = CommandType.StoredProcedure,
            BindByName = true
        };

        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = empCode;
        objCmd.Parameters.Add("CUR_LEAVEAPPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        return objCmd.ExecuteReader(CommandBehavior.CloseConnection);
    }

    /// <summary>
    /// PKG_LEAVEAPPAUTH.SPROC_SETAPPAUTHORITY
    /// Persists the recommendation and approval authority for an employee.
    /// </summary>
    public void SetAuthorities(string empCode, string recomCode, string approverCode, string modifiedBy)
    {
        string strCn = objCnStr.getConnectingString();

        using (var objCn = new OracleConnection(strCn))
        {
            try
            {
                objCn.Open();
                using (var objCmd = new OracleCommand("PKG_LEAVEAPPAUTH.SPROC_SETAPPAUTHORITY", objCn))
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.BindByName = true;

                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = empCode;
                    objCmd.Parameters.Add("RECOMMAUTH_IN", OracleDbType.Varchar2).Value = recomCode;
                    objCmd.Parameters.Add("APPAUTH_IN", OracleDbType.Varchar2).Value = approverCode;
                    objCmd.Parameters.Add("EMPMODCODE_IN", OracleDbType.Int64).Value = modifiedBy;

                    objCmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (objCn.State != ConnectionState.Closed)
                    objCn.Close();
            }
        }
    }

        /// <summary>
        /// SPROC_LEAVEAUTHORITY_LOG
        /// Logs the change in leave authorities.
        /// </summary>
        public void LogAuthorityChange(string empCode, string recomCode, string approverCode, string updatedBy)
        {
            string strCn = objCnStr.getConnectingString();

            using (var objCn = new OracleConnection(strCn))
            {
                try
                {
                    objCn.Open();
                    using (var objCmd = new OracleCommand("SPROC_LEAVEAUTHORITY_LOG", objCn))
                    {
                        objCmd.CommandType = CommandType.StoredProcedure;
                        objCmd.BindByName = true;

                        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = empCode;
                        objCmd.Parameters.Add("RECOMMAUTH_IN", OracleDbType.Varchar2).Value = recomCode;
                        objCmd.Parameters.Add("APPAUTH_IN", OracleDbType.Varchar2).Value = approverCode;
                        objCmd.Parameters.Add("EMPMODCODE_IN", OracleDbType.Int64).Value = updatedBy;
                        objCmd.Parameters.Add("UPDATEDBY", OracleDbType.Varchar2).Value = updatedBy;
                        objCmd.Parameters.Add("TRANSACTIONTYPE", OracleDbType.Varchar2).Value = "UPDATE";

                        objCmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    if (objCn.State != ConnectionState.Closed)
                        objCn.Close();
                }
            }
        }

        public DataSet SearchLeaveDetail(string strEmpCode, string strLeavePlan, string strFromYear)
        {
            //// ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_LEAVEDEATIL";
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("LEAVEPLAN_IN", OracleDbType.Int32).Value = strLeavePlan;
                    objCmd.Parameters.Add("FROMYEAR_IN", OracleDbType.Varchar2).Value = strFromYear;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public int getdesignation(string strstrdesignation)
        {
            string strSql;

            object objStatus;
            OracleConnection objCn;
            OracleCommand objCmd;
            string strsql;

            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    strsql = "select addesignationid from ademployee where adempcode = " + strstrdesignation;
                    objCmd.CommandText = strsql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = CommandType.Text;
                    objCmd.BindByName = true;
                    objStatus = objCmd.ExecuteScalar();
                    return Convert.ToInt32(objStatus);
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
        /// GET LEAVE DETAIL YEAR WISE
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="LeaveYear"></param>
        ///<param name="ReqType">USER/AUTH</param>
        /// <returns></returns>
        public DataSet EmpLeaveRecord(string strEmpCode, string LeaveYear, string ReqType)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_LEAVERECORD";
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("YEAR_IN", OracleDbType.Int32).Value = LeaveYear;
                    objCmd.Parameters.Add("REQTYPE_IN", OracleDbType.Varchar2).Value = ReqType;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet GetApprovarCode(string strEmpCode, string strApprovarType)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_GETAPPROVARCODE.SPROC_AD_APPROVARCODE";
                    objCmd.Parameters.Add("CUR_APPROVARCODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("APPROVALTYPE_IN", OracleDbType.Int32).Value = strApprovarType;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public string getLeaveBalance(string strUserID, string strLeaveType, string strLeaveApplied)
        {
            ////ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            object objCount;
            // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_LEAVEBALANCE";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("leaveType", OracleDbType.Int32).Value = strLeaveType;
                    objCmd.Parameters.Add("leaveApplied", OracleDbType.Double).Value = strLeaveApplied;
                    objCmd.Parameters.Add("leaveBalance", OracleDbType.Double).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    objCount = objCmd.Parameters["leaveBalance"].Value.ToString();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return Convert.ToString(objCount);
            }
        }

        /// <summary>
        /// CHECKING IN INSERTING LEAVE
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="strDateFrom"></param>
        /// <param name="strDateTo"></param>
        /// <param name="strHalfDay"></param>
        /// <param name="strLeaveType"></param>
        /// <param name="strPurpose"></param>
        /// <param name="strContactNo"></param>
        /// <param name="strBackupEmpcode"></param>
        /// <param name="strSupervisorEmpCode"></param>
        /// <param name="strHalfdaydate"></param>
        /// <param name="strShift"></param>
        /// <param name="strHalfdayShift"></param>
        /// <param name="strinLieuFrmdate"></param>
        /// <param name="strinLieuTodate"></param>
        /// <returns></returns>
        //=====================================================================================================
        //Below Changes done as on 22-07-2022 (In below procedure one last parameters "strShiftStartTime" added)
        //=====================================================================================================
        public string leaveEntry(string strEmpCode, string strLeaveYear, string strDateFrom, string strDateTo,
                                string strHalfDay, string strLeaveType, string strPurpose,
                                string strContactNo, string strBackupEmpcode, string strSupervisorEmpCode,
                                string strHalfdaydate, string strShift, string strHalfdayShift,
                                string strinLieuFrmdate, string strinLieuTodate, string strfromtime, string strtotime, string strShiftStartTime/*, int IsSpecialApprovalReq*/)
        //=====================================================================================================
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;
            string strValidHalfDay = string.Empty;

            if (strHalfDay == "2")
                strValidHalfDay = strHalfdaydate;
            else if (strHalfDay == "1")
                strValidHalfDay = "";

            // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_LEAVEENTRY";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("LEAVEYEAR_IN", OracleDbType.Varchar2).Value = strLeaveYear;
                    objCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = strDateFrom;
                    objCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = strDateTo;
                    objCmd.Parameters.Add("ISHALFDAY_IN", OracleDbType.Int32).Value = strHalfDay;
                    objCmd.Parameters.Add("ADLEAVETYPEID_IN", OracleDbType.Int32).Value = strLeaveType;
                    objCmd.Parameters.Add("PURPOSE_IN", OracleDbType.Varchar2).Value = strPurpose;
                    objCmd.Parameters.Add("CONTACTNO_IN", OracleDbType.Varchar2).Value = strContactNo;
                    objCmd.Parameters.Add("BACKUPEMPCODE_IN", OracleDbType.Varchar2).Value = strBackupEmpcode;
                    objCmd.Parameters.Add("SUPERVISORADEMPCODE_IN", OracleDbType.Varchar2).Value = strSupervisorEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("HALFDAYDATE_IN", OracleDbType.Varchar2).Value = strValidHalfDay;
                    objCmd.Parameters.Add("SHIFT_IN", OracleDbType.Varchar2).Value = strShift;
                    objCmd.Parameters.Add("HALFDAYSHIFT_IN", OracleDbType.Varchar2).Value = strHalfdayShift;

                    objCmd.Parameters.Add("INLIEUDATEFROM_IN", OracleDbType.Varchar2).Value = strinLieuFrmdate;
                    objCmd.Parameters.Add("INLIEUDATETO_IN", OracleDbType.Varchar2).Value = strinLieuTodate;
                    //objCmd.Parameters.Add("KICODE_IN", OracleDbType.Varchar2).Value = strCboKi;
                    //objCmd.Parameters.Add("ISSPECIALAPPROVALREQ_IN", OracleDbType.Int32).Value = IsSpecialApprovalReq;
                    objCmd.Parameters.Add("HALFDAYSTIME_IN", OracleDbType.Varchar2).Value = strfromtime;
                    objCmd.Parameters.Add("HALFDAYETIME_IN", OracleDbType.Varchar2).Value = strtotime;
                    //=====================================================================================================
                    //Below Changes done as on 22-07-2022 (below parameter "strShiftStartTime" added)
                    //=====================================================================================================
                    objCmd.Parameters.Add("SHIFTSTARTTIME", OracleDbType.Varchar2).Value = strShiftStartTime;
                    //=====================================================================================================

                    objCmd.Parameters.Add("NOOFLEAVEDAYS_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("RespEmpMailId_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("BACKUPEMPNAME_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["NOOFLEAVEDAYS_OUT"].Value) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value) + "#" + Convert.ToString(objCmd.Parameters["RespEmpMailId_OUT"].Value) + "#" + Convert.ToString(objCmd.Parameters["BACKUPEMPNAME_OUT"].Value);
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

        public DataSet ManageLeaveRequest(string strEmpCode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_PENDINGLEAVEREQUEST";
                    objCmd.Parameters.Add("CUR_PENDINGLEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objCmd.BindByName = true;
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

        public DataSet ManageLeaveRequest1(string strEmpCode, string strYear)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_PENDINGLEAVEREQUEST1";
                    objCmd.Parameters.Add("CUR_PENDINGLEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("LEAVE_YEAR", OracleDbType.Int32).Value = strYear;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet ManageLeaveApproval(string strSupEmpCode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_PENDINGLEAVEAPPROVAL";
                    objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet EditLeaveApproval(string strTransID, string strEcode, string strSecode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_LEAVEAPPROVALBYID";
                    objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strTransID;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEcode;
                    objCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSecode;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public string UpdateApproval(string strSupervisorEmpCode, string strID, string strApprovalStatus, string strRemarks, string strleavePlan, string strFlag, string strSpecialAppStatus, string strSpecialAppEcode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;
            // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_UPDATEAPPROVAL";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strID;
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupervisorEmpCode;
                    objCmd.Parameters.Add("APPROVALSTATUS_IN", OracleDbType.Int32).Value = strApprovalStatus;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("LEAVEPLAN_IN", OracleDbType.Varchar2).Value = strleavePlan;
                    objCmd.Parameters.Add("FLAG_IN", OracleDbType.Varchar2).Value = strFlag;
                    objCmd.Parameters.Add("SPECIALAPPSTATUS_IN", OracleDbType.Varchar2).Value = strSpecialAppStatus;
                    objCmd.Parameters.Add("SPECIALAPPECODE_IN", OracleDbType.Varchar2).Value = strSpecialAppEcode;
                    objCmd.BindByName = true;
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

        public DataSet SapLeaveData()
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_SAPDATA";
                    objCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet ManageRecord()
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_ADMIN_LEAVERECORD";
                    objCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet GetLeaveRecOnCond(string strFromDt, string strToDt, string empcode, string strsitid, string struserid, string strDesg, string strStatus)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    //strSql = " SELECT b.firstname || ' ' || b.lastname as ENAME, "+
                    //         " a.adempcode,decode(a.adleavetypeid, 1 ,decode(NVL(a.leaveplan,'P'),'P','HSLP','U','HSLU'),"+
                    //         " 2, decode(NVL(a.leaveplan,'P'),'P','HCLP','U','HCLU'), 3 , decode(NVL(a.leaveplan,'P'),'P','HELP','U','HELU'),5 "+
                    //         " , decode(NVL(a.leaveplan,'P'),'P','HCOP','U','HCOU'), 4, decode(NVL(a.leaveplan,'P'),'P','ALWP','U','UABS'), 6 " +
                    //         " ,decode(NVL(a.leaveplan,'P'),'P','HMTL','U','HMTL'), 7, decode(NVL(a.leaveplan,'P'),'P','HRLP','U','HRLU'), 8 " +
                    //         " , decode(NVL(a.leaveplan,'P'),'P','AELP','U','AELU'),9 , decode(NVL(a.leaveplan,'P'),'P','HSPL','U','HSPL')) " +
                    //         " as LEAVETYPEDETAIL," +
                    //         " to_char(a.datefrom,'dd-Mon-yyyy') as fromdate, to_char(a.dateto,'dd-Mon-yyyy') as todate,"+ 
                    //         " decode(NVL(a.ishalfday,1),1,'Full Day', 2, 'Half Day') as halfday,"+
                    //         " a.halfdaydate, decode(a.shift,1,'G',2,'A',3,'B',4,'C') As SHIFT , DECODE(a.halfdayshift,1,'1st', 2,'2nd') as hdayshift, " +
                    //         " to_char(a.datefrom,'ddmmyyyy') as fromdatechar, "+
                    //         " to_char(a.dateto,'ddmmyyyy') as todatechar, a.noofdays, to_char(a.halfdaydate,'dd-Mon-yyyy') as halfdate "+
                    //         " , A.AdTransactionID FROM adempleavetransaction a inner join ademployee b"+
                    //         " on a.adempcode = b.adempcode "+
                    //         " where a.issupervisorapproved = 1 and a.issupersupervisorapproved = 1";

                    //if (!string.IsNullOrEmpty(strFromDt) && !string.IsNullOrEmpty(strToDt))
                    //{
                    //    strSql = strSql + " and trunc(a.supersupervisorapproveddate) between '" + strFromDt + "' and '" + strToDt + "'";
                    //}
                    //if (!string.IsNullOrEmpty(empcode))
                    //{
                    //    strSql = strSql + " and a.adempcode = " + empcode;
                    //}
                    //strSql = strSql + " Order by  a.datefrom ";
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_LEAVEDATAFORIR_GET";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strFromDt;
                    objCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strToDt;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsitid;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = struserid;
                    objCmd.Parameters.Add("DESG_IN", OracleDbType.Varchar2).Value = strDesg;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                    objCmd.Parameters.Add("CUR_LVDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

        public DataSet GetLeaveRecordForSAP(string strFromDt, string strToDt, string empcode, string strsitid, string struserid, string strDesg, string strStatus)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_LEAVEDATAFORSAP_GET";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strFromDt;
                    objCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strToDt;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsitid;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = struserid;
                    objCmd.Parameters.Add("DESG_IN", OracleDbType.Varchar2).Value = strDesg;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                    objCmd.Parameters.Add("CUR_LVDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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


        public DataSet get_AllEmp(string empcode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    strSql = "PKG_LEAVEAPPLICATION.SPROC_AD_EMPLEAVECARD";
                    objCmd.CommandText = strSql;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = empcode;
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet LeaveType()
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.sproc_LeaveType_Get";
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataTable EmployeeDataGrid(string strEmpCode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
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
                    objCmd.CommandText = "PKG_LEAVEAPPAUTH.SPROC_GETEMPLOYEEDATA";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_LEAVEAPPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataTable STAFF_EmployeeDataGrid(string strEmpCode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
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
                    objCmd.CommandText = "PKG_LEAVEAPPAUTH.SPROC_GETSTAFF_EMPLOYEEDATA";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_LEAVEAPPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = CommandType.StoredProcedure;
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


        public DataTable EmployeeLeaveDetail(string strEmpCode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
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
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_EMPLEAVEDTL_GET";
                    objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("REQID_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_EMPLOYEE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        /// <summary>
        /// UPDATE LEAVE ENTRY
        /// </summary>
        /// <param name="strID"></param>
        /// <param name="strEmpcode"></param>
        /// <param name="strDateFrom"></param>
        /// <param name="strDateTo"></param>
        /// <param name="strHalfDay"></param>
        /// <param name="strLeaveType"></param>
        /// <param name="strPurpose"></param>
        /// <param name="strContactNo"></param>
        /// <param name="strBackupEmpcode"></param>
        /// <param name="strSupervisorEmpCode"></param>
        /// <param name="strHalfdaydate"></param>
        /// <param name="strShift"></param>
        /// <param name="strHalfdayShift"></param>
        /// <param name="strinLieuFrmdate"></param>
        /// <param name="strinLieuTodate"></param>
        /// <param name="strCboKiCode"></param>
        /// <returns></returns>
        //=====================================================================================================
        //Below Changes done as on 22-07-2022 (In below procedure three last parameters "strfromtime, strtotime, strShiftStartTime" added)
        //=====================================================================================================
        public string UpdateleaveEntry(string strID, string strEmpcode, string strDateFrom, string strDateTo,
                                        string strHalfDay, string strLeaveType, string strPurpose, string strContactNo,
                                        string strBackupEmpcode, string strSupervisorEmpCode, string strHalfdaydate,
                                        string strShift, string strHalfdayShift, string strinLieuFrmdate, string strinLieuTodate,
                                        string strCboKiCode, string strfromtime, string strtotime, string strShiftStartTime)
        //=====================================================================================================
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;
            string strValidHalfDay = string.Empty;

            if (strHalfDay == "2")
                strValidHalfDay = strHalfdaydate;
            else if (strHalfDay == "1")
                strValidHalfDay = "";

            // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_UPDATELEAVEENTRY";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strID;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode;
                    objCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = strDateFrom;
                    objCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = strDateTo;
                    objCmd.Parameters.Add("ISHALFDAY_IN", OracleDbType.Int32).Value = strHalfDay;
                    objCmd.Parameters.Add("ADLEAVETYPEID_IN", OracleDbType.Int32).Value = strLeaveType;
                    objCmd.Parameters.Add("PURPOSE_IN", OracleDbType.Varchar2).Value = strPurpose;
                    objCmd.Parameters.Add("CONTACTNO_IN", OracleDbType.Varchar2).Value = strContactNo;
                    objCmd.Parameters.Add("BACKUPEMPCODE_IN", OracleDbType.Varchar2).Value = strBackupEmpcode;
                    objCmd.Parameters.Add("SUPERVISORADEMPCODE_IN", OracleDbType.Varchar2).Value = strSupervisorEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("HALFDAYDATE_IN", OracleDbType.Varchar2).Value = strValidHalfDay;
                    objCmd.Parameters.Add("SHIFT_IN", OracleDbType.Varchar2).Value = strShift;
                    objCmd.Parameters.Add("HALFDAYSHIFT_IN", OracleDbType.Varchar2).Value = strHalfdayShift;

                    objCmd.Parameters.Add("INLIEUDATEFROM_IN", OracleDbType.Varchar2).Value = strinLieuFrmdate;
                    objCmd.Parameters.Add("INLIEUDATETO_IN", OracleDbType.Varchar2).Value = strinLieuTodate;
                    objCmd.Parameters.Add("LEAVEYEAR_IN", OracleDbType.Varchar2).Value = strCboKiCode;

                    //=====================================================================================================
                    // //Below Changes done as on 22-07-2022 (below parameter "strfromtime,strtotime,strShiftStartTime" added)
                    //=====================================================================================================
                    objCmd.Parameters.Add("HALFDAYSTIME_IN", OracleDbType.Varchar2).Value = strfromtime;
                    objCmd.Parameters.Add("HALFDAYETIME_IN", OracleDbType.Varchar2).Value = strtotime;
                    objCmd.Parameters.Add("shiftstarttime", OracleDbType.Varchar2).Value = strShiftStartTime;
                    //=====================================================================================================

                    objCmd.Parameters.Add("NOOFLEAVEDAYS_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["NOOFLEAVEDAYS_OUT"].Value) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
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

        //--SR39596 - CR-2263 Change start
        public DataSet CancelledLeaveAfteApproval(string empcode, string strsiteid, string struserid, string FromDate, string ToDate, string strLeaveType, string strRemarks)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_GETCANCELLEAVEAFTAPP";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsiteid;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = struserid;
                    objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
                    objCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = ToDate;
                    objCmd.Parameters.Add("LeaveTYPE_IN", OracleDbType.Varchar2).Value = strLeaveType;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = CommandType.StoredProcedure;
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
        //--SR39596 - CR-2263 Change end

        /// <summary>
        /// UPDATED ON 30 APR 2010 BY SANTOSH APPLY FILTER ON REPORT
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="FromDate"></param>
        /// <param name="TillDate"></param>
        /// <returns></returns>
        public DataSet CancelledLeave(string UserID, string FromDate, string TillDate, string siteid, string userid)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_GETCANCELLEAVE";
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
                    objCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = siteid;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = userid;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet LeaveApprovalHistory(string strEmpCode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_APPROVALHISTORYLEAVE";
                    objCmd.Parameters.Add("CUR_PENDINGLEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet EditASRLeaveApproval(string strTransID)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_ASRAPPROVALBYID";
                    objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strTransID;
                    objCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public string leaveASRApproval(string strTrID, string strAppRemarks, string strAppStatus, string strASREcode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_LEAVEASRAPPROVAL";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strTrID;
                    objCmd.Parameters.Add("APPREMARKS_IN", OracleDbType.Varchar2).Value = strAppRemarks;
                    objCmd.Parameters.Add("APPSTATUS_IN", OracleDbType.Varchar2).Value = strAppStatus;
                    objCmd.Parameters.Add("ASRECODE_IN", OracleDbType.Varchar2).Value = strASREcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
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

        public DataTable FromYearGet()
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_FROMYEAR_GET";
                    objCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        /// <summary>
        /// GET VALID LEAVE APPLICATION YEARS
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetLeaveAppliedYears(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_FROMYEAR_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_LEAVEYRS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }
        public DataTable ExcelExport(string strEmpCode, string strYear)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_AD_EXCELEXPORT";
                    objCmd.Parameters.Add("CUR_PENDINGLEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("LEAVE_YEAR", OracleDbType.Int32).Value = strYear;
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    DataTable objDt = new DataTable();
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

        public DataSet ManageSpecialRecord(string strFromDt, string strToDt, string empcode, string strsiteid, string struserid)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_MANAGESPECIALLEAVE";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
                    objCmd.Parameters.Add("APP_FROMDATE", OracleDbType.Varchar2).Value = strFromDt;
                    objCmd.Parameters.Add("APP_TODATE", OracleDbType.Varchar2).Value = strToDt;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsiteid;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = struserid;
                    objCmd.Parameters.Add("CUR_SLEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

        public DataSet GetPendingLeaveReport(string strFromDt, string strToDt, string empcode, string strsiteid, string struserid)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_PENDINGLEAVE_GET";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
                    objCmd.Parameters.Add("APP_FROMDATE", OracleDbType.Varchar2).Value = strFromDt;
                    objCmd.Parameters.Add("APP_TODATE", OracleDbType.Varchar2).Value = strToDt;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsiteid;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = struserid;
                    objCmd.Parameters.Add("CUR_PLEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

        public DataSet GetLeaveDTL(string empcode, string strreqid)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_EMPLEAVEDTL_GET";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = empcode;
                    objCmd.Parameters.Add("REQID_IN", OracleDbType.Varchar2).Value = strreqid;
                    objCmd.Parameters.Add("CUR_EMPLOYEE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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

        public DataSet CancelledLeaveAfteApprovalBYADTRANSACTIONID(string strRequestid)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_GETCANLEAAPPBYADTRANSID";
                    objCmd.Parameters.Add("ReqID", OracleDbType.Varchar2).Value = strRequestid;
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet GetSpecialApprovalAuthority(string strEcode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_GETAPPROVARCODE.SPROC_AD_SPECIAL_APPCODE";
                    objCmd.Parameters.Add("CUR_APPROVARCODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = Convert.ToInt64(strEcode);
                    objCmd.CommandType = CommandType.StoredProcedure;
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
        public DataTable GetRealTimeatt(string strEmpCode, string strpunchdate, string strassociateecode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
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
                    objCmd.CommandText = "PKG_LEAVEAPPAUTH.SPROC_GETREALTIMERATT";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("PUNCHDATE_IN", OracleDbType.Varchar2).Value = strpunchdate;
                    objCmd.Parameters.Add("ASSOCIATECODE_IN", OracleDbType.Varchar2).Value = strassociateecode;
                    objCmd.Parameters.Add("CUR_LEAVEAPPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataTable GetAccessReport(string strEmpCode, string strpunchdate, string strassociateecode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
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
                    objCmd.CommandText = "PKG_LEAVEAPPAUTH.SPROC_GETACCESSREPORT";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("PUNCHDATE_IN", OracleDbType.Varchar2).Value = strpunchdate;
                    objCmd.Parameters.Add("ASSOCIATECODE_IN", OracleDbType.Varchar2).Value = strassociateecode;
                    objCmd.Parameters.Add("CUR_LEAVEAPPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public void Leavebulkapproval(string reqid, string empcode, string appecode, string appname)
        {
            string strApprovalStatus = string.Empty;
            string strleavePlan = string.Empty;
            string strRemarks = string.Empty;

            string strRequestid = string.Empty;
            string strEcode = string.Empty;

            string strSpecialAppStatus = string.Empty;
            string strSpecialAppEcode = string.Empty;
            int IsSpecialApprovalReq = 0;

            strRequestid = reqid;
            strEcode = empcode;

            // ConnectionString// objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            //commanEmail sendMail = new commanEmail();

            LeaveApps objLeave = new LeaveApps();
            DataSet objLeaveDsBYID = objLeave.EditLeaveApproval(strRequestid, strEcode, appecode);
            string strFlag = objLeaveDsBYID.Tables[0].Rows[0][11].ToString();
            string strappcode = objLeaveDsBYID.Tables[0].Rows[0]["SUPERSUPERVISORADEMPCODE"].ToString();
            if (strFlag == "A")
            {
                strApprovalStatus = "1";
                strleavePlan = "P";
                strRemarks = "Ok";
                if (strappcode != appecode)
                {

                }

                if (objLeaveDsBYID.Tables[0].Rows[0][21].ToString() == "9")
                {
                    //TimeSpan span = (DateTime.ParseExact(objLeaveDsBYID.Tables[0].Rows[0]["DATEADDED"].ToString(), "dd-MMM-yyyy", null) - DateTime.ParseExact(objLeaveDsBYID.Tables[0].Rows[0]["lieufromdate"].ToString(), "dd-MMM-yyyy", null));
                    DateTime leavedate = DateTime.ParseExact(objLeaveDsBYID.Tables[0].Rows[0]["DATEADDED"].ToString(), "dd-MMM-yyyy", null);
                    DateTime luidate = DateTime.ParseExact(objLeaveDsBYID.Tables[0].Rows[0]["lieufromdate"].ToString(), "dd-MMM-yyyy", null).AddMonths(6);

                    if (leavedate > luidate)
                    {
                        IsSpecialApprovalReq = 1;
                    }
                }
                if (IsSpecialApprovalReq == 1)
                {
                    DataSet objSpecialAuthority = objLeave.GetSpecialApprovalAuthority(strEcode);
                    strSpecialAppStatus = "0";
                    strSpecialAppEcode = objSpecialAuthority.Tables[0].Rows[0]["adempcode"].ToString();
                }
                /////// END /////////
            }
            else if (strFlag == "R")
            {
                strApprovalStatus = "1";
                strRemarks = "Ok";
            }
            else if (strFlag == "S")
            {
                strApprovalStatus = "1";
                strRemarks = "Ok";
            }

            string strSupEmpCode = "";
            strSupEmpCode = appecode;

            LeaveApps objUpdateLeave = new LeaveApps();
            string EntryResult = objUpdateLeave.UpdateApproval(strSupEmpCode, strRequestid, strApprovalStatus, strRemarks, strleavePlan, strFlag, strSpecialAppStatus, strSpecialAppEcode);

            string[] leaveStatus = EntryResult.Split(new char[] { '#' });
            string errResult = Convert.ToString(leaveStatus[0]);
            string errMsg = Convert.ToString(leaveStatus[1]);

            if (errResult == "0")
            {

            }
            if (errResult != "0")
            {
                string LeaveStatus = "";
                if (errResult == "1")
                {
                    if (strFlag == "A")
                    {
                        LeaveStatus = "approved";
                    }
                    if (strFlag == "R")
                    {
                        LeaveStatus = "recommended";
                    }
                    if (strFlag == "S")
                    {
                        LeaveStatus = "Special Approved";
                    }
                }
                else if (errResult == "2")
                    LeaveStatus = "rejected";

                OracleConnection objCn = new OracleConnection();
                OracleCommand objCmd = new OracleCommand("select a.emailid from ademployee a where a.adempcode = " + strEcode, objCn);
                objCmd.CommandType = CommandType.Text;
                string strEmailid = string.Empty;

                using (objCn)
                {
                    objCn.ConnectionString = strConn;
                    try
                    {
                        objCn.Open();
                        OracleDataReader objReader = objCmd.ExecuteReader();
                        if (objReader.HasRows)
                        {
                            while (objReader.Read())
                            {
                                strEmailid = objReader["emailid"].ToString().Trim();
                            }
                        }
                    }
                    finally
                    {
                        if (objCn != null)
                        {
                            objCn.Close();
                        }
                    }
                }

                OracleConnection objCnn = new OracleConnection();
                OracleCommand objCmmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_APPROVALAUTHCODE", objCnn);
                objCmmd.CommandType = CommandType.StoredProcedure;

                objCmmd.Parameters.Add("CUR_CODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = strEcode;
                objCmmd.Parameters.Add("TRANSACTIONID_IN", OracleDbType.Int64).Value = strRequestid;


                string strEmailAppid = string.Empty;
                string strRequesterName = string.Empty;
                string strRequestEcode = string.Empty;
                string strFromDate = string.Empty;
                string strtoDate = string.Empty;
                string strNoOfDays = string.Empty;
                string strLeaveType = string.Empty;

                using (objCnn)
                {
                    objCnn.ConnectionString = strConn;
                    try
                    {
                        objCnn.Open();
                        OracleDataReader objReader = objCmmd.ExecuteReader();
                        if (objReader.HasRows)
                        {
                            while (objReader.Read())
                            {
                                strEmailAppid = objReader["SUPSUPEMAIL"].ToString().Trim();
                                strRequesterName = objReader["EMPNAME"].ToString().Trim();
                                strRequestEcode = objReader["ADEMPCODE"].ToString().Trim();
                                strFromDate = objReader["FROMDATE"].ToString().Trim();
                                strtoDate = objReader["TODATE"].ToString().Trim();
                                strNoOfDays = objReader["NOOFDAYS"].ToString().Trim();
                                strLeaveType = objReader["LeaveType"].ToString().Trim();
                            }
                        }
                    }
                    finally
                    {
                        if (objCnn != null)
                        {
                            objCnn.Close();
                        }
                    }

                }

                //if (strFlag == "R" && errResult == "1")
                //{
                //    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                //    sendMail.MailTo = strEmailAppid;

                //    string strSubject = "Leave Request from - " + strRequesterName + ", Employee Code - " + strRequestEcode;
                //    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                //                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Leave Request from " + strRequesterName + " - Emp Code (" + strRequestEcode + ")</font></b></td>" +
                //                     "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strRequestEcode + "</td>" +
                //                     "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strRequesterName + "</td>" +
                //                     " </tr><tr> " +
                //                     "<td width=125 valign=top>From Date</td><td width=389 valign=top>" + strFromDate + "</td></tr><tr><td width=125 valign=top>To Date</td>" +
                //                     "<td width=389 valign=top>" + strtoDate + "</td></tr><tr><td width=125 valign=top>Total no of day(s)</td><td width=389 valign=top>" + strNoOfDays + "</td></tr>" +
                //                     "<tr><td width=125 valign=top>Leave Type</td><td width=389 valign=top>" + strLeaveType + "</td></tr>" +
                //                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> for approval process.</td></tr>" +
                //                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                //    sendMail.MailSubject = strSubject;
                //    sendMail.MailBody = strBody;
                //    try
                //    {
                //        bool status = sendMail.Send();
                //    }
                //    catch (Exception ex)
                //    {
                //        //
                //    }
                //    finally
                //    {
                //        //
                //    }

                //}

                //if (!string.IsNullOrEmpty(strEmailid))
                //{

                //    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                //    sendMail.MailTo = strEmailid;

                //    string strSubject = "Leave Approval Status - " + LeaveStatus;
                //    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                //                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Leave Approval Status " + LeaveStatus + " - Emp Code (" + strRequestEcode + ")</font></b></td></tr>" +

                //                     "<tr><td>" +
                //                     "<table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                //                     "<tr><td valign=top colspan =2>Your leave request has been <b>" + LeaveStatus + "</b> by Approval Authotiy. The leave details are as follows:</td></tr>" +
                //                     "<tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strRequestEcode + "</td></tr>" +
                //                     "<tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strRequesterName + "</td> </tr>" +
                //                     "<tr><td width=125 valign=top>From Date</td><td width=389 valign=top>" + strFromDate + "</td></tr>" +
                //                     "<tr><td width=125 valign=top>To Date</td><td width=389 valign=top>" + strtoDate + "</td></tr>" +
                //                     "<tr><td width=125 valign=top>Total no of day(s)</td><td width=389 valign=top>" + strNoOfDays + "</td></tr>" +
                //                     "<tr><td width=125 valign=top>Leave Type</td><td width=389 valign=top>" + strLeaveType + "</td></tr>" +
                //                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> to view the approval history.</td></tr>" +
                //                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                //    sendMail.MailSubject = strSubject;
                //    sendMail.MailBody = strBody;
                //    try
                //    {
                //        bool status = sendMail.Send();
                //    }
                //    catch (Exception ex)
                //    {
                //        //
                //    }
                //    finally
                //    {
                //        //
                //    }
                //}

            }
        }

        //13-July-2022 - Edit Leave Issue - Start
        public DataTable GetEMPLeaveDTL(long reqId)
        {
            // ConnectionString// objCnStr = new ConnectionString();
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
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_EMPDETAILS_GET";
                    objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int64).Value = reqId;
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    objCmd.CommandType = CommandType.StoredProcedure;
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
        //13-July-2022 - Edit Leave Issue - End

        //Below added by aumento for report on 18-04-2023 (SR39612 and 39614)=========================================
        public DataSet CancelledLeaveForReport(string UserID, string FromDate, string TillDate, string siteid, string userid)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_GETCANCELLEAVE_FORREPORT";
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
                    objCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = siteid;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = userid;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet ManageSpecialRecord_ForReport(string strFromDt, string strToDt, string empcode, string strsiteid, string struserid)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            string strSql = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_MANAGESPECIALLEAVEREP";
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
                    objCmd.Parameters.Add("APP_FROMDATE", OracleDbType.Varchar2).Value = strFromDt;
                    objCmd.Parameters.Add("APP_TODATE", OracleDbType.Varchar2).Value = strToDt;
                    objCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsiteid;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = struserid;
                    objCmd.Parameters.Add("CUR_SLEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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
        //====================================================================================================

        // =====Below Added by aumento for IsUpdatedmailId on 18042023============================================
        public DataSet GETISUPDATEDMAILID(string IsUpdated, string strCode)
        {
            // ConnectionString// objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_GETISUPDATEDMAILID";
                    objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ISUPDATED_IN", OracleDbType.Varchar2).Value = IsUpdated;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strCode;
                    objCmd.CommandType = CommandType.StoredProcedure;
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

        public DataTable GetOpHeadOperations(string strEcode)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_OPHEADOPERATIONS_GET";
            cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEcode;
            cmd.Parameters.Add("CUR_OPERATIONS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        //---------------------------- nEW ADDEDY
        public DataTable OrgDetail_Get(string strEcode, string strDesig)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_ORGDETAILCOUNT_GET";
            cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEcode;
            cmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = strDesig;
            cmd.Parameters.Add("CUR_OPERATIONS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get division to fill division dropdown list in leave card.
        /// </summary>
        public DataTable GetDivisions(string str_ecode, string fn_desig, string str_operations)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_DIVISIONS_GET";
            cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = str_ecode;
            cmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = fn_desig;
            cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_operations;
            cmd.Parameters.Add("CUR_DIVISIONS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get operation.
        /// </summary>
        //public DataTable GetDivision(string str_Operations)
        //{
        //    objDt = new DataTable();
        //    cmd = new OracleCommand();
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //    cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_DIVISIONS_GET";
        //    cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_Operations;
        //    cmd.Parameters.Add("CUR_DIVISIONS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //    //oDataMgmt = new DataManagement();
        //    objDt = oDataMgmt.GetDataTable(cmd);
        //    return (objDt);
        //}

        /// <summary>
        /// This function is used to get all divisions of which employee is assigned.
        /// </summary>
        //public DataTable GetDivHeadDivisions(string strecode)
        //{
        //    objDt = new DataTable();
        //    cmd = new OracleCommand();
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //    cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_DIVHEADDIVISIONS_GET";
        //    cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
        //    cmd.Parameters.Add("CUR_DIVISIONS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //    //oDataMgmt = new DataManagement();
        //    objDt = oDataMgmt.GetDataTable(cmd);
        //    return (objDt);
        //}

        /// <summary>
        /// This function is used to get division of an employee to which it is associated.
        /// </summary>
        //public DataTable GetDivByEcode(string strecode)
        //{
        //    objDt = new DataTable();
        //    cmd = new OracleCommand();
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //    cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_DIVBYECODE_GET";
        //    cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
        //    cmd.Parameters.Add("CUR_DIVISIONS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //    //oDataMgmt = new DataManagement();
        //    objDt = oDataMgmt.GetDataTable(cmd);
        //    return (objDt);
        //}

        /// <summary>
        /// This function is used to get departments by operation.
        /// </summary>
        public DataTable GetDepartment(string strecode,string fndesig,string str_Operations, string str_divisions)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_DEPARTMENT_GET";
            cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_Operations;
            cmd.Parameters.Add("DIVISIONS_IN", OracleDbType.Varchar2).Value = str_divisions;
            cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            cmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = fndesig;
            cmd.Parameters.Add("CUR_DEPARTMENTS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get section.
        /// </summary>
        public DataTable GetSection(string str_operations, string str_divisions, string str_depid, string strecode, string fndesig)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_SECTION_GET";
            cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_operations;
            cmd.Parameters.Add("DIVISIONS_IN", OracleDbType.Varchar2).Value = str_divisions;
            cmd.Parameters.Add("DEPARTMENTS_IN", OracleDbType.Varchar2).Value = str_depid;
            cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            cmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = fndesig;
            cmd.Parameters.Add("CUR_SECTION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get employee.
        /// </summary>
        public DataTable GetEmployee(string str_operations, string str_divisions, string str_depid, string str_secid, string strecode, string fndesig)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_EMP_GET";
            cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_operations;
            cmd.Parameters.Add("DIVISIONS_IN", OracleDbType.Varchar2).Value = str_divisions;
            cmd.Parameters.Add("DEPARTMENTS_IN", OracleDbType.Varchar2).Value = str_depid;
            cmd.Parameters.Add("SECTIONS_IN", OracleDbType.Varchar2).Value = str_secid;
            cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            cmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = fndesig;
            cmd.Parameters.Add("CUR_EMPLOYEE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get employee number of availed leave and balanced leave
        /// </summary>
        public DataTable GetLeaveAvailBal(string str_operations, string str_divisions, string str_depid, string str_secid,
            string strecode, string fndesig, string str_employees, string str_Year)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_LEAVEAVAILBAL_GET";
            cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_operations;
            cmd.Parameters.Add("DIVISIONS_IN", OracleDbType.Varchar2).Value = str_divisions;
            cmd.Parameters.Add("DEPARTMENTS_IN", OracleDbType.Varchar2).Value = str_depid;
            cmd.Parameters.Add("SECTIONS_IN", OracleDbType.Varchar2).Value = str_secid;
            cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            cmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = fndesig;
            cmd.Parameters.Add("EMPLOYEES_IN", OracleDbType.Clob).Value = str_employees;
            cmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = str_Year;
            cmd.Parameters.Add("CUR_LEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public string getEmployeeGender(string strecode)
    {
             string strEmpGender = string.Empty;
           ////ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                     objDt = new DataTable();
                    cmd = new OracleCommand();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_GETGENDER";
                    cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int64).Value = strecode;
                    cmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.BindByName = true;
                    objDt = oDataMgmt.GetDataTable(cmd);
                    if (objDt != null)
                    {
                        strEmpGender = objDt.Rows[0]["gender"].ToString();
                    }
                }
                catch(Exception ex)
                {
                     if (objCn != null)
                    {
                        objCn.Close();
                    }
                    strEmpGender = ex.Message.ToString();
                     
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return strEmpGender;
            }
        }  
        
        // NEW 
        public DataTable GetFn_desig(string strecode)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_FNDESIG_GET";
            cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            cmd.Parameters.Add("CUR_FNDESIG", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get operation(s) of which employee is designates as a operating head.
        /// </summary>
        public DataTable GetOrgUnits(string strEcode, string strOrgtype)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_ORGLEVELS_GET";
            cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEcode;
            cmd.Parameters.Add("ORGTYPE_IN", OracleDbType.Varchar2).Value = strOrgtype;
            cmd.Parameters.Add("CUR_ORGLEVEL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get division to fill division dropdown list in leave card.
        /// </summary>
        public DataTable GetDivision(string str_ecode, string fn_desig, string str_operations)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_DIVISION_GET";
            cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = str_ecode;
            cmd.Parameters.Add("FNDESIG_IN", OracleDbType.Varchar2).Value = fn_desig;
            cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_operations;
            cmd.Parameters.Add("CUR_DIVISIONS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get departments by operation.
        /// </summary>
        public DataTable Get_Department(string str_Operations, string str_divisions, string strecode, string fndesig)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_DEPARTMENT_GET";
            cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_Operations;
            cmd.Parameters.Add("DIVISIONS_IN", OracleDbType.Varchar2).Value = str_divisions;
            cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            cmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = fndesig;
            cmd.Parameters.Add("CUR_DEPARTMENTS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get section.
        /// </summary>
        public DataTable Get_Section(string str_operations, string str_divisions, string str_depid, string strecode, string fndesig)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_SECTIONS_GET";
            cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_operations;
            cmd.Parameters.Add("DIVISIONS_IN", OracleDbType.Varchar2).Value = str_divisions;
            cmd.Parameters.Add("DEPARTMENTS_IN", OracleDbType.Varchar2).Value = str_depid;
            cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            cmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = fndesig;
            cmd.Parameters.Add("CUR_SECTION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// This function is used to get employee.
        /// </summary>
        public DataTable GetEmployees(string str_operations, string str_divisions, string str_depid, string str_secid, string strecode, string fndesig)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_LEAVEAPPLICATION.SPROC_EMPS_GET";
            cmd.Parameters.Add("OPERATIONS_IN", OracleDbType.Varchar2).Value = str_operations;
            cmd.Parameters.Add("DIVISIONS_IN", OracleDbType.Varchar2).Value = str_divisions;
            cmd.Parameters.Add("DEPARTMENTS_IN", OracleDbType.Varchar2).Value = str_depid;
            cmd.Parameters.Add("SECTIONS_IN", OracleDbType.Varchar2).Value = str_secid;
            cmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            cmd.Parameters.Add("DESIG_IN", OracleDbType.Varchar2).Value = fndesig;
            cmd.Parameters.Add("CUR_EMPLOYEE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.BindByName = true;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
    }
}
