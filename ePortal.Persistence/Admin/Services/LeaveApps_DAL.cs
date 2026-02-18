using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Services
{
    public class LeaveApps_DAL : ILeaveApps_DAL
    {
        private readonly IDataManagement _dataMgmt;
        private readonly IConnectionString objCnStr;
        public LeaveApps_DAL(IDataManagement dataMgmt, IConnectionString _objCnStr)
        {
            _dataMgmt = dataMgmt;
            objCnStr = _objCnStr;
        }

        public DataSet SearchLeaveDetail(string strEmpCode, string strLeavePlan, string strFromYear)
        {
            string strCn = objCnStr.getConnectingString();

            using var objCn = new OracleConnection(strCn);
            using var objCmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_LEAVEDEATIL", objCn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            objCmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = Convert.ToInt32(strEmpCode);
            objCmd.Parameters.Add("LEAVEPLAN_IN", OracleDbType.Int32).Value = Convert.ToInt32(strLeavePlan);
            objCmd.Parameters.Add("FROMYEAR_IN", OracleDbType.Varchar2).Value = strFromYear;

            var adapter = new OracleDataAdapter(objCmd);
            var ds = new DataSet();

            objCn.Open();
            adapter.Fill(ds);

            return ds;
        }

        public int GetDesignation(string strstrdesignation)
        {
            string strCn = objCnStr.getConnectingString();

            using var objCn = new OracleConnection(strCn);
            using var objCmd = new OracleCommand("select addesignationid from ademployee where adempcode = :empCode", objCn);

            objCmd.CommandType = CommandType.Text;
            objCmd.BindByName = true;

            // Use parameterized query to prevent SQL injection
            objCmd.Parameters.Add("empCode", OracleDbType.Int32).Value = Convert.ToInt32(strstrdesignation);

            objCn.Open();
            object objStatus = objCmd.ExecuteScalar();

            return objStatus != null ? Convert.ToInt32(objStatus) : 0;
        }
        /// <summary>
        /// GET LEAVE DETAIL YEAR WISE
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="LeaveYear"></param>
        ///<param name="ReqType">USER/AUTH</param>
        /// <returns></returns>
        /// 

        public DataSet EmpLeaveRecord(string empCode, int leaveYear, string reqType)
        {
            string strCn = objCnStr.getConnectingString();

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_LEAVERECORD", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            cmd.Parameters.Add("CUR_LEAVEAPPS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empCode;
            cmd.Parameters.Add("YEAR_IN", OracleDbType.Int32).Value = leaveYear;
            cmd.Parameters.Add("REQTYPE_IN", OracleDbType.Varchar2).Value = reqType;

            var adapter = new OracleDataAdapter(cmd);
            var ds = new DataSet();

            conn.Open();
            adapter.Fill(ds);

            return ds;
        }
       public DataSet GetApprovarCode(string empCode, string approvarType)
        {
            string strCn = objCnStr.getConnectingString();

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_GETAPPROVARCODE.SPROC_AD_APPROVARCODE", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            // Parameters
            cmd.Parameters.Add("CUR_APPROVARCODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            // If empCode and approvarType are numeric, convert them before assigning
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = Convert.ToInt32(empCode);
            cmd.Parameters.Add("APPROVALTYPE_IN", OracleDbType.Int32).Value = Convert.ToInt32(approvarType);

            var adapter = new OracleDataAdapter(cmd);
            var ds = new DataSet();

            conn.Open();
            adapter.Fill(ds);

            return ds;
        }
       public string GetLeaveBalance(int userId, int leaveType, double leaveApplied)
        {
           string strCn = objCnStr.getConnectingString();

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_LEAVEBALANCE", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            cmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = userId;
            cmd.Parameters.Add("leaveType", OracleDbType.Int32).Value = leaveType;
            cmd.Parameters.Add("leaveApplied", OracleDbType.Double).Value = leaveApplied;
            cmd.Parameters.Add("leaveBalance", OracleDbType.Double).Direction = ParameterDirection.Output;

            conn.Open();
            cmd.ExecuteNonQuery();

            var balance = cmd.Parameters["leaveBalance"].Value;
            return balance?.ToString() ?? "0";
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
        public DataSet ManageLeaveRequest(string strEmpCode)
        {
            string strCn = objCnStr.getConnectingString();

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_PENDINGLEAVEREQUEST", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            cmd.Parameters.Add("CUR_PENDINGLEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;

            var adapter = new OracleDataAdapter(cmd);
            var ds = new DataSet();

            conn.Open();
            adapter.Fill(ds);

            return ds;
        }
        public DataSet ManageLeaveRequest1(string strEmpCode, string strYear)
        {
            string strCn = objCnStr.getConnectingString();

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_PENDINGLEAVEREQUEST1", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            cmd.Parameters.Add("CUR_PENDINGLEAVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
            cmd.Parameters.Add("LEAVE_YEAR", OracleDbType.Int32).Value = strYear;

            var adapter = new OracleDataAdapter(cmd);
            var ds = new DataSet();

            conn.Open();
            adapter.Fill(ds);

            return ds;
        }    
        public DataSet ManageLeaveApproval(string strSupEmpCode)
        {
            string strCn = objCnStr.getConnectingString();

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_PENDINGLEAVEAPPROVAL", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            cmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;

            var adapter = new OracleDataAdapter(cmd);
            var ds = new DataSet();

            conn.Open();
            adapter.Fill(ds);

            return ds;
        }       
        public DataSet EditLeaveApproval(string strTransID, string strEcode, string strSecode)
        {
            string strCn = objCnStr.getConnectingString();

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_LEAVEAPPROVALBYID", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            cmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strTransID;
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEcode;
            cmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSecode;
            cmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            var adapter = new OracleDataAdapter(cmd);
            var ds = new DataSet();

            conn.Open();
            adapter.Fill(ds);

            return ds;
        }
        public string UpdateApproval(string strSupervisorEmpCode, string strID, string strApprovalStatus, string strRemarks, string strleavePlan, string strFlag, string strSpecialAppStatus, string strSpecialAppEcode)
        {
            string strCn = objCnStr.getConnectingString();

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_UPDATEAPPROVAL", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            cmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strID;
            cmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupervisorEmpCode;
            cmd.Parameters.Add("APPROVALSTATUS_IN", OracleDbType.Int32).Value = strApprovalStatus;
            cmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
            cmd.Parameters.Add("LEAVEPLAN_IN", OracleDbType.Varchar2).Value = strleavePlan;
            cmd.Parameters.Add("FLAG_IN", OracleDbType.Varchar2).Value = strFlag;
            cmd.Parameters.Add("SPECIALAPPSTATUS_IN", OracleDbType.Varchar2).Value = strSpecialAppStatus;
            cmd.Parameters.Add("SPECIALAPPECODE_IN", OracleDbType.Varchar2).Value = strSpecialAppEcode;

            // Output parameters
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            conn.Open();
            cmd.ExecuteNonQuery();

            string result = $"{cmd.Parameters["RESULT_OUT"].Value}#{cmd.Parameters["ERRMSG"].Value}";
            return result;
        }
        public DataSet SapLeaveData()
        {
            
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

        public DataSet ManageRecord()
        {
            
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

        public DataSet GetLeaveRecOnCond(string strFromDt, string strToDt, string empcode, string strsitid, string struserid, string strDesg, string strStatus)
        {
            
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
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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

        public DataSet LeaveType()
        {
            
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

        public DataTable EmployeeDataGrid(string strEmpCode)
        {
            
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

        public DataTable STAFF_EmployeeDataGrid(string strEmpCode)
        {
            
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


        public DataTable EmployeeLeaveDetail(string strEmpCode)
        {
            
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
            public string UpdateLeaveEntry(string strID, string strEmpcode, string strDateFrom, string strDateTo,
                                        string strHalfDay, string strLeaveType, string strPurpose, string strContactNo,
                                        string strBackupEmpcode, string strSupervisorEmpCode, string strHalfdaydate,
                                        string strShift, string strHalfdayShift, string strinLieuFrmdate, string strinLieuTodate,
                                        string strCboKiCode, string strfromtime, string strtotime, string strShiftStartTime)
        {
            string strCn = objCnStr.getConnectingString();
            string strValidHalfDay = string.Empty;
            if (strHalfDay == "2")
                strValidHalfDay = strHalfdaydate;
            else if (strHalfDay == "1")
                strValidHalfDay = "";

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_UPDATELEAVEENTRY", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            // Input parameters
            cmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strID;
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode;
            cmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = strDateFrom;
            cmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = strDateTo;
            cmd.Parameters.Add("ISHALFDAY_IN", OracleDbType.Int32).Value = strHalfDay;
            cmd.Parameters.Add("ADLEAVETYPEID_IN", OracleDbType.Int32).Value = strLeaveType;
            cmd.Parameters.Add("PURPOSE_IN", OracleDbType.Varchar2).Value = strPurpose;
            cmd.Parameters.Add("CONTACTNO_IN", OracleDbType.Varchar2).Value = strContactNo;
            cmd.Parameters.Add("BACKUPEMPCODE_IN", OracleDbType.Varchar2).Value = strBackupEmpcode;
            cmd.Parameters.Add("SUPERVISORADEMPCODE_IN", OracleDbType.Varchar2).Value = strSupervisorEmpCode;
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("HALFDAYDATE_IN", OracleDbType.Varchar2).Value = strValidHalfDay;
            cmd.Parameters.Add("SHIFT_IN", OracleDbType.Varchar2).Value = strShift;
            cmd.Parameters.Add("HALFDAYSHIFT_IN", OracleDbType.Varchar2).Value = strHalfdayShift;

            cmd.Parameters.Add("INLIEUDATEFROM_IN", OracleDbType.Varchar2).Value = strinLieuFrmdate;
            cmd.Parameters.Add("INLIEUDATETO_IN", OracleDbType.Varchar2).Value = strinLieuTodate;
            cmd.Parameters.Add("LEAVEYEAR_IN", OracleDbType.Varchar2).Value = strCboKiCode;
            //=====================================================================================================
            // //Below Changes done as on 22-07-2022 (below parameter "strfromtime,strtotime,strShiftStartTime" added)
            //=====================================================================================================
            cmd.Parameters.Add("HALFDAYSTIME_IN", OracleDbType.Varchar2).Value = strfromtime;
            cmd.Parameters.Add("HALFDAYETIME_IN", OracleDbType.Varchar2).Value = strtotime;
            cmd.Parameters.Add("shiftstarttime", OracleDbType.Varchar2).Value = strShiftStartTime;
            //=====================================================================================================
            // Output parameters
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("NOOFLEAVEDAYS_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

            conn.Open();
            cmd.ExecuteNonQuery();

            string result = $"{cmd.Parameters["RESULT_OUT"].Value}#" +
                            $"{cmd.Parameters["NOOFLEAVEDAYS_OUT"].Value}#" +
                            $"{cmd.Parameters["ERRMSG"].Value}";

            return result;
        }

        //--SR39596 - CR-2263 Change start
        public DataSet CancelledLeaveAfteApproval(string empcode, string strsiteid, string struserid, string FromDate, string ToDate, string strLeaveType, string strRemarks)
        {
            
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

        public DataSet LeaveApprovalHistory(string strEmpCode)
        {
            
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

        public DataSet EditASRLeaveApproval(string strTransID)
        {
            
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
        public string LeaveASRApproval(string strTrID, string strAppRemarks, string strAppStatus, string strASREcode)
        {
            string strCn = objCnStr.getConnectingString();

            using var conn = new OracleConnection(strCn);
            using var cmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_LEAVEASRAPPROVAL", conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };

            // Input parameters
            cmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strTrID;
            cmd.Parameters.Add("APPREMARKS_IN", OracleDbType.Varchar2).Value = strAppRemarks;
            cmd.Parameters.Add("APPSTATUS_IN", OracleDbType.Varchar2).Value = strAppStatus;
            cmd.Parameters.Add("ASRECODE_IN", OracleDbType.Varchar2).Value = strASREcode;

            // Output parameters
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            conn.Open();
            cmd.ExecuteNonQuery();

            string result = $"{cmd.Parameters["RESULT_OUT"].Value}#{cmd.Parameters["ERRMSG"].Value}";
            return result;
        }

        public DataTable FromYearGet()
        {
            
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

        /// <summary>
        /// GET VALID LEAVE APPLICATION YEARS
        /// </summary>
        /// <param name="EmpCode"></param>
        /// <returns></returns>       
        public DataTable GetLeaveAppliedYears(string EmpCode)
        {
            using var oCmd = new OracleCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "PKG_LEAVEAPPLICATION.SPROC_FROMYEAR_GET",
                BindByName = true
            };

            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_LEAVEYRS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            return _dataMgmt.GetDataTable(oCmd);
        }

        public DataTable ExcelExport(string strEmpCode, string strYear)
        {
            
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
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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

        public DataSet GetSpecialApprovalAuthority(string strEcode)
        {
            
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
        public DataTable GetRealTimeatt(string strEmpCode, string strpunchdate, string strassociateecode)
        {
            
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

        public DataTable GetAccessReport(string strEmpCode, string strpunchdate, string strassociateecode)
        {
            
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

        //public void Leavebulkapproval(string reqid, string empcode, string appecode, string appname)
        //{
        //    string strApprovalStatus = string.Empty;
        //    string strleavePlan = string.Empty;
        //    string strRemarks = string.Empty;

        //    string strRequestid = string.Empty;
        //    string strEcode = string.Empty;

        //    string strSpecialAppStatus = string.Empty;
        //    string strSpecialAppEcode = string.Empty;
        //    int IsSpecialApprovalReq = 0;

        //    strRequestid = reqid;
        //    strEcode = empcode;

            
        //    string strConn = objCnStr.getConnectingString();
        //    commanEmail sendMail = new commanEmail();

        //    LeaveApps objLeave = new LeaveApps();
        //    DataSet objLeaveDsBYID = objLeave.EditLeaveApproval(strRequestid, strEcode, appecode);
        //    string strFlag = objLeaveDsBYID.Tables[0].Rows[0][11].ToString();
        //    string strappcode = objLeaveDsBYID.Tables[0].Rows[0]["SUPERSUPERVISORADEMPCODE"].ToString();
        //    if (strFlag == "A")
        //    {
        //        strApprovalStatus = "1";
        //        strleavePlan = "P";
        //        strRemarks = "Ok";
        //        if (strappcode != appecode)
        //        {

        //        }

        //        if (objLeaveDsBYID.Tables[0].Rows[0][21].ToString() == "9")
        //        {
        //            //TimeSpan span = (DateTime.ParseExact(objLeaveDsBYID.Tables[0].Rows[0]["DATEADDED"].ToString(), "dd-MMM-yyyy", null) - DateTime.ParseExact(objLeaveDsBYID.Tables[0].Rows[0]["lieufromdate"].ToString(), "dd-MMM-yyyy", null));
        //            DateTime leavedate = (DateTime.ParseExact(objLeaveDsBYID.Tables[0].Rows[0]["DATEADDED"].ToString(), "dd-MMM-yyyy", null));
        //            DateTime luidate = DateTime.ParseExact(objLeaveDsBYID.Tables[0].Rows[0]["lieufromdate"].ToString(), "dd-MMM-yyyy", null).AddMonths(6);

        //            if (leavedate > luidate)
        //            {
        //                IsSpecialApprovalReq = 1;
        //            }
        //        }
        //        if (IsSpecialApprovalReq == 1)
        //        {
        //            DataSet objSpecialAuthority = objLeave.GetSpecialApprovalAuthority(strEcode);
        //            strSpecialAppStatus = "0";
        //            strSpecialAppEcode = objSpecialAuthority.Tables[0].Rows[0]["adempcode"].ToString();
        //        }
        //        /////// END /////////
        //    }
        //    else if (strFlag == "R")
        //    {
        //        strApprovalStatus = "1";
        //        strRemarks = "Ok";
        //    }
        //    else if (strFlag == "S")
        //    {
        //        strApprovalStatus = "1";
        //        strRemarks = "Ok";
        //    }

        //    string strSupEmpCode = "";
        //    strSupEmpCode = appecode;

        //    LeaveApps objUpdateLeave = new LeaveApps();
        //    string EntryResult = objUpdateLeave.UpdateApproval(strSupEmpCode, strRequestid, strApprovalStatus, strRemarks, strleavePlan, strFlag, strSpecialAppStatus, strSpecialAppEcode);

        //    string[] leaveStatus = EntryResult.Split(new Char[] { '#' });
        //    string errResult = Convert.ToString(leaveStatus[0]);
        //    string errMsg = Convert.ToString(leaveStatus[1]);

        //    if (errResult == "0")
        //    {

        //    }
        //    if (errResult != "0")
        //    {
        //        string LeaveStatus = "";
        //        if (errResult == "1")
        //        {
        //            if (strFlag == "A")
        //            {
        //                LeaveStatus = "approved";
        //            }
        //            if (strFlag == "R")
        //            {
        //                LeaveStatus = "recommended";
        //            }
        //            if (strFlag == "S")
        //            {
        //                LeaveStatus = "Special Approved";
        //            }
        //        }
        //        else if (errResult == "2")
        //            LeaveStatus = "rejected";

        //        OracleConnection objCn = new OracleConnection();
        //        OracleCommand objCmd = new OracleCommand("select a.emailid from ademployee a where a.adempcode = " + strEcode, objCn);
        //        objCmd.CommandType = System.Data.CommandType.Text;
        //        string strEmailid = string.Empty;

        //        using (objCn)
        //        {
        //            objCn.ConnectionString = strConn;
        //            try
        //            {
        //                objCn.Open();
        //                OracleDataReader objReader = objCmd.ExecuteReader();
        //                if (objReader.HasRows)
        //                {
        //                    while (objReader.Read())
        //                    {
        //                        strEmailid = objReader["emailid"].ToString().Trim();
        //                    }
        //                }
        //            }
        //            finally
        //            {
        //                if (objCn != null)
        //                {
        //                    objCn.Close();
        //                }
        //            }
        //        }

        //        OracleConnection objCnn = new OracleConnection();
        //        OracleCommand objCmmd = new OracleCommand("PKG_LEAVEAPPLICATION.SPROC_AD_APPROVALAUTHCODE", objCnn);
        //        objCmmd.CommandType = System.Data.CommandType.StoredProcedure;

        //        objCmmd.Parameters.Add("CUR_CODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //        objCmmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = strEcode;
        //        objCmmd.Parameters.Add("TRANSACTIONID_IN", OracleDbType.Int64).Value = strRequestid;


        //        string strEmailAppid = string.Empty;
        //        string strRequesterName = string.Empty;
        //        string strRequestEcode = string.Empty;
        //        string strFromDate = string.Empty;
        //        string strtoDate = string.Empty;
        //        string strNoOfDays = string.Empty;
        //        string strLeaveType = string.Empty;

        //        using (objCnn)
        //        {
        //            objCnn.ConnectionString = strConn;
        //            try
        //            {
        //                objCnn.Open();
        //                OracleDataReader objReader = objCmmd.ExecuteReader();
        //                if (objReader.HasRows)
        //                {
        //                    while (objReader.Read())
        //                    {
        //                        strEmailAppid = objReader["SUPSUPEMAIL"].ToString().Trim();
        //                        strRequesterName = objReader["EMPNAME"].ToString().Trim();
        //                        strRequestEcode = objReader["ADEMPCODE"].ToString().Trim();
        //                        strFromDate = objReader["FROMDATE"].ToString().Trim();
        //                        strtoDate = objReader["TODATE"].ToString().Trim();
        //                        strNoOfDays = objReader["NOOFDAYS"].ToString().Trim();
        //                        strLeaveType = objReader["LeaveType"].ToString().Trim();
        //                    }
        //                }
        //            }
        //            finally
        //            {
        //                if (objCnn != null)
        //                {
        //                    objCnn.Close();
        //                }
        //            }

        //        }

        //        if (strFlag == "R" && errResult == "1")
        //        {
        //            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
        //            sendMail.MailTo = strEmailAppid;

        //            string strSubject = "Leave Request from - " + strRequesterName + ", Employee Code - " + strRequestEcode;
        //            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
        //                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Leave Request from " + strRequesterName + " - Emp Code (" + strRequestEcode + ")</font></b></td>" +
        //                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strRequestEcode + "</td>" +
        //                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strRequesterName + "</td>" +
        //                             " </tr><tr> " +
        //                             "<td width=125 valign=top>From Date</td><td width=389 valign=top>" + strFromDate + "</td></tr><tr><td width=125 valign=top>To Date</td>" +
        //                             "<td width=389 valign=top>" + strtoDate + "</td></tr><tr><td width=125 valign=top>Total no of day(s)</td><td width=389 valign=top>" + strNoOfDays + "</td></tr>" +
        //                             "<tr><td width=125 valign=top>Leave Type</td><td width=389 valign=top>" + strLeaveType + "</td></tr>" +
        //                             "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> for approval process.</td></tr>" +
        //                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

        //            sendMail.MailSubject = strSubject;
        //            sendMail.MailBody = strBody;
        //            try
        //            {
        //                bool status = sendMail.Send();
        //            }
        //            catch (Exception ex)
        //            {
        //                //
        //            }
        //            finally
        //            {
        //                //
        //            }

        //        }

        //        if (!string.IsNullOrEmpty(strEmailid))
        //        {

        //            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
        //            sendMail.MailTo = strEmailid;

        //            string strSubject = "Leave Approval Status - " + LeaveStatus;
        //            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
        //                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Leave Approval Status " + LeaveStatus + " - Emp Code (" + strRequestEcode + ")</font></b></td></tr>" +

        //                             "<tr><td>" +
        //                             "<table cellpadding=4 cellspacing=0 border=0 width=600px>" +
        //                             "<tr><td valign=top colspan =2>Your leave request has been <b>" + LeaveStatus + "</b> by Approval Authotiy. The leave details are as follows:</td></tr>" +
        //                             "<tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strRequestEcode + "</td></tr>" +
        //                             "<tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strRequesterName + "</td> </tr>" +
        //                             "<tr><td width=125 valign=top>From Date</td><td width=389 valign=top>" + strFromDate + "</td></tr>" +
        //                             "<tr><td width=125 valign=top>To Date</td><td width=389 valign=top>" + strtoDate + "</td></tr>" +
        //                             "<tr><td width=125 valign=top>Total no of day(s)</td><td width=389 valign=top>" + strNoOfDays + "</td></tr>" +
        //                             "<tr><td width=125 valign=top>Leave Type</td><td width=389 valign=top>" + strLeaveType + "</td></tr>" +
        //                             "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> to view the approval history.</td></tr>" +
        //                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

        //            sendMail.MailSubject = strSubject;
        //            sendMail.MailBody = strBody;
        //            try
        //            {
        //                bool status = sendMail.Send();
        //            }
        //            catch (Exception ex)
        //            {
        //                //
        //            }
        //            finally
        //            {
        //                //
        //            }
        //        }

        //    }
        //}

        //13-July-2022 - Edit Leave Issue - Start
        public DataTable GetEMPLeaveDTL(Int64 reqId)
        {
            
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
        //13-July-2022 - Edit Leave Issue - End

        //Below added by aumento for report on 18-04-2023 (SR39612 and 39614)=========================================
        public DataSet CancelledLeaveForReport(string UserID, string FromDate, string TillDate, string siteid, string userid)
        {
            
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

        public DataSet ManageSpecialRecord_ForReport(string strFromDt, string strToDt, string empcode, string strsiteid, string struserid)
        {
            
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
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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
        //===========================================================================================
    }
}
