using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Admin.Interface;

namespace ePortal.Persistence.Admin.Services
{
    public class Safety : ISafety
    {
        private readonly IConnectionString objCnStr;
        private readonly IDataManagement oDataMgmt;
        public Safety(IConnectionString conn, IDataManagement _oDataMgmt)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = conn;
        }
        #region"Instance variables"
        DataSet ds = new DataSet();
        DataRow[] _datarow;
        String strQry = String.Empty;
        #endregion

        #region"GET DATA"
        public DataTable Get_SFDepartment(string strplantid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_SAFETY.SPROC_SFDEPARTMENT_GET";
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplantid;
            oCmd.Parameters.Add("CUR_SFDEPARTMENTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public DataTable Get_Category(string strcategoryid, string strcategory, string strstatus, string strplantid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_SAFETY.SPROC_CATEGORY_GET";
            oCmd.Parameters.Add("CATEGORYID_IN", OracleDbType.Varchar2).Value = strcategoryid;
            oCmd.Parameters.Add("CATEGORY_IN", OracleDbType.Varchar2).Value = strcategory;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplantid;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("CUR_SFCATEGORY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public DataTable Get_Act(string stractid, string stract, string strstatus, string strplantid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_SAFETY.SPROC_ACT_GET";
            oCmd.Parameters.Add("ACTID_IN", OracleDbType.Varchar2).Value = stractid;
            oCmd.Parameters.Add("ACT_IN", OracleDbType.Varchar2).Value = stract;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplantid;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("CUR_SFACT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public DataTable GetFrequency(string strfrq)
        {
            DataTable dt = new DataTable();
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    DataSet objDS = new DataSet();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = "PKG_SAFETY.SPROC_FREQUENCY_GET";
                    cmd.Parameters.Add("FREQUENCY_IN", OracleDbType.Varchar2).Value = strfrq;
                    cmd.Parameters.Add("CUR_SFFRQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    oDa.Fill(dt);
                    return dt;
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
        public DataTable GetContract(string strcontractid, string strcategoryid, string stractid, string strstatus, string strsecid,
         string strdptid, string strdivid, string strvpid, string strfraomdate, string strtodate, string strsfsection, string strplantid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_SAFETY.SPROC_CONTRACTS_GET";
            oCmd.Parameters.Add("CONTRACTID_IN", OracleDbType.Varchar2).Value = strcontractid;
            oCmd.Parameters.Add("CATEGORYID_IN", OracleDbType.Varchar2).Value = strcategoryid;
            oCmd.Parameters.Add("ACTID_IN", OracleDbType.Varchar2).Value = stractid;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("SECID_IN", OracleDbType.Varchar2).Value = strsecid;
            oCmd.Parameters.Add("DPTID_IN", OracleDbType.Varchar2).Value = strdptid;
            oCmd.Parameters.Add("DIVID_IN", OracleDbType.Varchar2).Value = strdivid;
            oCmd.Parameters.Add("VPID_IN", OracleDbType.Varchar2).Value = strvpid;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strfraomdate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strtodate;
            oCmd.Parameters.Add("SFSECTION_IN", OracleDbType.Varchar2).Value = strsfsection;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplantid;
            oCmd.Parameters.Add("CUR_CONTRACT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public DataTable GetReminderPart(string strcontractid, string strstatus, string strflag, string strecode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_Safety.SPROC_REMINDER_GET";
            oCmd.Parameters.Add("CONTRACTID_IN", OracleDbType.Varchar2).Value = strcontractid;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("FLAG_IN", OracleDbType.Varchar2).Value = strflag;
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            oCmd.Parameters.Add("CUR_REMINDER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }
        public DataTable GetPendingApproval(string strcontractid, string strstatus, string strflag, string strecode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_Safety.SPROC_PENDAPPREMINDER_GET";
            oCmd.Parameters.Add("CONTRACTID_IN", OracleDbType.Varchar2).Value = strcontractid;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("FLAG_IN", OracleDbType.Varchar2).Value = strflag;
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            oCmd.Parameters.Add("CUR_REMINDER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }
        public DataTable GetReminderdDetail(string strcontractid)
        {
            DataTable dt = new DataTable();
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    DataSet objDS = new DataSet();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = "PKG_SAFETY.SPROC_REMINDERDETAIL_GET";
                    cmd.Parameters.Add("CONTRACTID_IN", OracleDbType.Varchar2).Value = strcontractid;
                    cmd.Parameters.Add("CUR_REMINDERDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    oDa.Fill(dt);
                    return dt;
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
        public DataTable GetReminderdMailID(string strreminderid)
        {
            DataTable dt = new DataTable();
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    DataSet objDS = new DataSet();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = "PKG_SAFETY.SPROC_REMINDERSTATUSMAIL_GET";
                    cmd.Parameters.Add("REMINDERID_IN", OracleDbType.Varchar2).Value = strreminderid;
                    cmd.Parameters.Add("CUR_REMINDERMAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    oDa.Fill(dt);
                    return dt;
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
        public DataTable GetDashBoardCount(string strsfsection, string strplantid, string strsecid, string strdptid, string strdivid, string strvpid)
        {
            DataTable dt = new DataTable();
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    DataSet objDS = new DataSet();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = "PKG_SAFETY.SPROC_DASHBOARDCNT_GET";
                    cmd.Parameters.Add("SFSECTION_IN", OracleDbType.Varchar2).Value = strsfsection;
                    cmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplantid;
                    cmd.Parameters.Add("SECID_IN", OracleDbType.Varchar2).Value = strsecid;
                    cmd.Parameters.Add("DPTID_IN", OracleDbType.Varchar2).Value = strdptid;
                    cmd.Parameters.Add("DIVID_IN", OracleDbType.Varchar2).Value = strdivid;
                    cmd.Parameters.Add("VPID_IN", OracleDbType.Varchar2).Value = strvpid;
                    cmd.Parameters.Add("CUR_REMINDER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    oDa.Fill(dt);
                    return dt;
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
        public DataTable GetDashBoardDetail(string strsfsection, string strplantid, string strsecid, string strdptid, string strdivid, string strvpid, string strstatus)
        {
            DataTable dt = new DataTable();
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    DataSet objDS = new DataSet();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = "PKG_SAFETY.SPROC_DASHBOARDDETAILS_GET";
                    cmd.Parameters.Add("SFSECTION_IN", OracleDbType.Varchar2).Value = strsfsection;
                    cmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplantid;
                    cmd.Parameters.Add("SECID_IN", OracleDbType.Varchar2).Value = strsecid;
                    cmd.Parameters.Add("DPTID_IN", OracleDbType.Varchar2).Value = strdptid;
                    cmd.Parameters.Add("DIVID_IN", OracleDbType.Varchar2).Value = strdivid;
                    cmd.Parameters.Add("VPID_IN", OracleDbType.Varchar2).Value = strvpid;
                    cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
                    cmd.Parameters.Add("CUR_REMINDER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    oDa.Fill(dt);
                    return dt;
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

        #region"INSERT AND UPDATE"
        public string InsertCategory(string strcategoryid, string strdes, string strinitial, string straddby, string stractive, string strplantid, string strAppAuth)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_SAFETY.SPROC_UPDATE_CATEGORY";
            objCmd.Parameters.Add("CATEGORYID_IN", OracleDbType.Varchar2).Value = strcategoryid;
            objCmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = strdes;
            objCmd.Parameters.Add("INITIALDESCRIP_IN", OracleDbType.Varchar2).Value = strinitial;
            objCmd.Parameters.Add("ADDBY_IN", OracleDbType.Varchar2).Value = straddby;
            objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = stractive;
            objCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplantid;
            objCmd.Parameters.Add("APPROVALTO_IN", OracleDbType.Varchar2).Value = strAppAuth;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);

            //IF ERROR OCCURED
            if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
            {
                errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);

            }
            return (errMsg);
        }
        public string InsertAct(string stractid, string strdes, string strinitial, string straddby, string stractive, string strplantid)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_Safety.SPROC_UPDATE_ACT";
            objCmd.Parameters.Add("SFACT_IN", OracleDbType.Varchar2).Value = stractid;
            objCmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = strdes;
            objCmd.Parameters.Add("INITIALDESCRIP_IN", OracleDbType.Varchar2).Value = strinitial;
            objCmd.Parameters.Add("ADDBY_IN", OracleDbType.Varchar2).Value = straddby;
            objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = stractive;
            objCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strplantid;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);

            //IF ERROR OCCURED
            if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
            {
                errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);

            }
            return (errMsg);
        }
        public int AddFrequency(string strfrqid, string strfrq, string status, int AddedBy, string strmonth)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    string strSql = "PKG_Safety.SPROC_FREQUENCY_UPDATE";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = strSql;
                    cmd.Parameters.Add("FREQUENCYID_IN", OracleDbType.Varchar2).Value = strfrqid;
                    cmd.Parameters.Add("FREQUENCY_IN", OracleDbType.Varchar2).Value = strfrq;
                    cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    cmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = AddedBy;
                    cmd.Parameters.Add("MONTH_IN", OracleDbType.Int32).Value = strmonth;
                    cmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
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
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
            return 0;
        }
        public string Insertcontracts(string strcontractid, string strcateegory, string stract, string strreq, string strfrq, string strexpdate, string strduedate, string strcomp, string strremark, string straddby, string stractive,
        string strattach, string strsectionid, string strrescode, string str1st, string str2nd, string str3rd, string str4th)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_Safety.SPROC_UPDATE_CONTRACTS";
            objCmd.Parameters.Add("SFCONTRACTID_IN", OracleDbType.Varchar2).Value = strcontractid;
            objCmd.Parameters.Add("CATEGORYID_IN", OracleDbType.Varchar2).Value = strcateegory;
            objCmd.Parameters.Add("ACTID_IN", OracleDbType.Varchar2).Value = stract;
            objCmd.Parameters.Add("REQUIREMENT_IN", OracleDbType.Varchar2).Value = strreq;
            objCmd.Parameters.Add("FREQUENCY_IN", OracleDbType.Varchar2).Value = strfrq;
            objCmd.Parameters.Add("EXPDATE_IN", OracleDbType.Varchar2).Value = strexpdate;
            objCmd.Parameters.Add("DUEDATE_IN", OracleDbType.Varchar2).Value = strduedate;
            objCmd.Parameters.Add("COMPSTATUS", OracleDbType.Varchar2).Value = strcomp;
            objCmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = strremark;
            objCmd.Parameters.Add("ADDBY_IN", OracleDbType.Varchar2).Value = straddby;
            objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = stractive;
            objCmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = strattach;
            //------------REMINDER---------------
            //objCmd.Parameters.Add("XMLRESPONSIBLE_IN", OracleDbType.Varchar2).Value = strxml;
            objCmd.Parameters.Add("SFSECTIONID_IN", OracleDbType.Varchar2).Value = strsectionid;
            objCmd.Parameters.Add("RESECODE_IN", OracleDbType.Varchar2).Value = strrescode;
            objCmd.Parameters.Add("REM1ST_IN", OracleDbType.Varchar2).Value = str1st;
            objCmd.Parameters.Add("REM2ND_IN", OracleDbType.Varchar2).Value = str2nd;
            objCmd.Parameters.Add("REM3RD_IN", OracleDbType.Varchar2).Value = str3rd;
            objCmd.Parameters.Add("REM4TH_IN", OracleDbType.Varchar2).Value = str4th;

            //---------------------------------------
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

            //EXECUTE QUERY
            oDataMgmt.ExecuteQuery(objCmd);

            //IF ERROR OCCURED
            if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
            {
                errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);

            }
            return (errMsg);
        }
        public int AddReminderdetail(string strremid, string strremark, string status, string AddedBy, string strattach)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    string strSql = "PKG_Safety.SPROC_REMINDER_DETAIL_SET";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = strSql;
                    cmd.Parameters.Add("REMINDERID_IN", OracleDbType.Varchar2).Value = strremid;
                    cmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
                    cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    cmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = AddedBy;
                    cmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = strattach;
                    cmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
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
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
            return 0;
        }
        public int UpdateReminderApproval(string strremid, string strremark, string status, string AddedBy)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    string strSql = "PKG_Safety.SPROC_REMINDER_APPSTATUS_SET";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.CommandText = strSql;
                    cmd.Parameters.Add("REMINDERID_IN", OracleDbType.Varchar2).Value = strremid;
                    cmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
                    cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    cmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = AddedBy;
                    cmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
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
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
            return 0;
        }
        #endregion
        public DataTable GETSAFETYSECTIONMST(string sfsection, string sfsectiondesc, string plant, string status, string SFSECTIONID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_SAFETY.SPROC_SFSECTION_GET";
            oCmd.Parameters.Add("SFSECTION_IN", OracleDbType.Varchar2).Value = sfsection;
            oCmd.Parameters.Add("SFSECDESC_IN", OracleDbType.Varchar2).Value = sfsectiondesc;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = plant;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("SFSECTIONIDID_IN", OracleDbType.Varchar2).Value = SFSECTIONID;
            oCmd.Parameters.Add("CUR_SFSECTIONLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }

        public string INSERTSECTIONMASTER(string section, string sfsectiondesc, string appauthecode, string status, string sectiontnsid, string addedby, string plant)
        {
            OracleCommand objCmd = new OracleCommand();
            string errMsg = string.Empty;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_SAFETY.SPROC_INSERTSECTIONMASTER";
            objCmd.Parameters.Add("ADSECTIONID_IN", OracleDbType.Varchar2).Value = section;
            objCmd.Parameters.Add("SFSECTIONDESC_IN", OracleDbType.Varchar2).Value = sfsectiondesc;
            objCmd.Parameters.Add("APPAUTHECODE_IN", OracleDbType.Varchar2).Value = appauthecode;
            objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            objCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = plant;
            objCmd.Parameters.Add("SFSECTIONID_IN", OracleDbType.Varchar2).Value = sectiontnsid;
            objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(objCmd);
            errMsg = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return errMsg;
        }
    }
}
