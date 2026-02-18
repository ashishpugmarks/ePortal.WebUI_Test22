using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Admin.Services
{
    public class Shiftchg:IShiftchg
    {
        DataSet objDs = new DataSet();
        private readonly IConnectionString _conn;
        private readonly IDataManagement oDataMgmt;

        public Shiftchg(IConnectionString conn, IDataManagement _oDataMgmt)
        {
            oDataMgmt = _oDataMgmt;
            _conn = conn;
        }
        public DataSet GetShiftDetails(string struserCode)
        {
            //
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_GETSHIFTDETAILS";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = struserCode;
                    objCmd.Parameters.Add("CUR_SHIFTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
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
        public DataSet GetShiftDetails_NEW(string struserCode, string strreqdate)
        {
            
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_GETSHIFTDETAILS_NEW";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = struserCode;
                    objCmd.Parameters.Add("REQDATE_IN", OracleDbType.Varchar2).Value = strreqdate;
                    objCmd.Parameters.Add("CUR_SHIFTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
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

        public DataSet GetDetails(string strCode)
        {
            
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_GETEMPDETAILS";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strCode;
                    objCmd.Parameters.Add("CUR_SHIFTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
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

        public string AddShiftChangerequest(string strUser, string strCurrshft, string strNewshft, string strDateFrom, string strDateTo, string strReason, string strRemark, string strAppCode)
        {
            
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    //objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_SHIFTCHANGETRANS";
                    objCmd.Parameters.Add("EMPLOYEE_IN", OracleDbType.Int32).Value = strUser;
                    objCmd.Parameters.Add("CURRSHFIFT_IN", OracleDbType.Int32).Value = strCurrshft;
                    objCmd.Parameters.Add("NEWSHFIFT_IN", OracleDbType.Int32).Value = strNewshft;
                    objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strDateFrom;
                    objCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strDateTo;
                    objCmd.Parameters.Add("REASON_IN", OracleDbType.Varchar2).Value = strReason;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strRemark;
                    objCmd.Parameters.Add("APPAUTH_IN", OracleDbType.Int32).Value = strAppCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
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

        public int UpdateShiftChangerequest(string strUser, string strCurrshft, string strNewshft, string strDateFrom, string strDateTo, string strReason, string strRemark, string strAppCode, string strUserCode)
        {
            
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    //objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_UPDATESHIFTCHANGETRANS";
                    objCmd.Parameters.Add("USER_IN", OracleDbType.Int32).Value = strUserCode;
                    objCmd.Parameters.Add("SHIFTTRANSID_IN", OracleDbType.Int32).Value = strUser;
                    objCmd.Parameters.Add("CURRSHFIFT_IN", OracleDbType.Int32).Value = strCurrshft;
                    objCmd.Parameters.Add("NEWSHFIFT_IN", OracleDbType.Int32).Value = strNewshft;
                    objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strDateFrom;
                    objCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strDateTo;
                    objCmd.Parameters.Add("REASON_IN", OracleDbType.Varchar2).Value = strReason;
                    objCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strRemark;
                    objCmd.Parameters.Add("APPAUTH_IN", OracleDbType.Int32).Value = strAppCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
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

        public DataSet GetAppCode(string strAppCode, string struserCode)
        {
            
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_GETAPPCODE";
                    objCmd.Parameters.Add("APPCODE_IN", OracleDbType.Int32).Value = strAppCode;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = struserCode;
                    objCmd.Parameters.Add("CUR_SHIFTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
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

        public DataSet EditShiftChangeRequest(string strCode)
        {
            
            string strConn = _conn.getConnectingString();
            DataSet objDss = new DataSet();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_EDITSHIFT_GET";
                    objCmd.Parameters.Add("SHIFTTRANSID_IN", OracleDbType.Int32).Value = strCode;
                    objCmd.Parameters.Add("CUR_SHIFTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDss);
                    return objDss;
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

        public DataSet ManageShiftChange(string strEmpCode)
        {
            
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_ASR_MANAGESHIFT";
                    objCmd.Parameters.Add("CUR_SHIFTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
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
        public DataSet ManageShiftChange1(string strEmpCode)
        {
            
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_ASR_MANAGESHIFT1";
                    objCmd.Parameters.Add("CUR_SHIFTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
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

        public DataSet ShiftChange(string strCode, string strAppFromDate, string strAppToDate, string strKI)
        {
            DataSet objDS = new DataSet();

            string strsql = string.Empty;
            
            string strCn = _conn.getConnectingString();
            OracleConnection objCn;
            OracleCommand objCmd;

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    strsql = "PKG_SHIFTCHANGE.SPROC_SHIFTCHANGEREQ_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strCode;
                    objCmd.Parameters.Add("STARTDATE_IN", OracleDbType.Varchar2).Value = strAppFromDate;
                    objCmd.Parameters.Add("ENDDATE_IN", OracleDbType.Varchar2).Value = strAppToDate;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKI;
                    objCmd.Parameters.Add("CUR_REQDTLS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    //strsql = "select t.asrshiftchangetranid, t.adempcode as empcode,a.firstname || ' ' || a.lastname as empname,dep.descrip as dept,"
                    //         + " to_char(t.nsstartdate,'dd/mm/yyyy') as datefrom,to_char(t.nsenddate,'dd/mm/yyyy') as dateto,"
                    //         + " sys.code  as currentshift,syn.code as newshift,"
                    //         + " decode(t.issupervisorapproved,0,'Pending',1,'Approved',2,'Rejected',3,'Cancelled') as supervisorapproval"
                    //         + " from asrshiftchangetran t"
                    //         + " left outer join syshift sys on sys.syshiftid = t.currentshift"
                    //         + " left outer join syshift syn on syn.syshiftid = t.newshift"
                    //         + " inner join adempdivdeptsect ade on ade.adempcode = t.adempcode"
                    //         + " left outer join addepartment dep on dep.addepartmentid = ade.addepartmentid"
                    //         + " left outer join ademployee a on a.adempcode = t.adempcode where 1=1";

                    //    if (strCode.ToString() != "")
                    //    {
                    //        strsql = strsql + " and  t.adempcode = '" + strCode.ToString() + "' ";
                    //    }
                    //    if (strAppFromDate != "" && strAppToDate.ToString() != "")
                    //    {
                    //        strsql = strsql + " and t.nsstartdate between '" + strAppFromDate + "' and '" + strAppToDate + "' ";
                    //    }
                    objCmd.CommandText = strsql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDS);
                    return objDS;
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

        public DataTable GetKi()
        {
            DataTable dt = new DataTable();

            string strsql = string.Empty;
            
            string strCn = _conn.getConnectingString();
            OracleConnection objCn;
            OracleCommand objCmd;

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    strsql = "PKG_SHIFTCHANGE.SPROC_KILIST_GET";
                    objCmd.Parameters.Add("CUR_KILIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strsql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(dt);
                    return dt;
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

        public int UpdateShiftApproval(string strRequestid, string strApprovalStatus, string strRemarks)
        {
            ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;

            
            strCn = _conn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_SHIFTCHANGE.SPROC_AD_UPDATESHIFTAPPROVAL";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("APPROVALSTATUS_IN", OracleDbType.Int32).Value = strApprovalStatus;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
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

        public DataSet GetShiftReqDetails(string strSupEmpCode)
        {
            
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_GETSHIFTREQDETAILS";
                    objCmd.Parameters.Add("CUR_SHIFTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
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
        public DataSet ShiftChangeAppHistory(string strEmpCode)
        {
            
            string strConn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_SHIFTCHANGE.SPROC_ASR_SHIFTAPPHISTORY";
                    objCmd.Parameters.Add("CUR_SHIFTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
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

        //-----------------------------------------------------------------------------------
        //Get Manesar Head name if shift will be A to G, B to G, C to G  on 22-09-2022 (Aumneto) 
        //----------------------------------------------------------------------------------

        public DataTable GetData(string Shift, string CurrShift)
        {
            string js = string.Empty;
            //Employee_Details emp = (Employee_Details)HttpContext..Session["Employee"];
            Employee_Details emp = new Employee_Details();
            string strConn = _conn.getConnectingString();
            OracleConnection objCn;
            OracleCommand objCmd = new OracleCommand();
            objCn = new OracleConnection();
            DataTable ds = new DataTable();
            if ((emp.Designation_Id == "16" || emp.Designation_Id == "19" || emp.Designation_Id == "33") && emp.Site_Id == "3" && Shift == "114"
                && (CurrShift == "113" || CurrShift == "115" || CurrShift == "116") && CurrShift != "114")
            {

                objCmd = new OracleCommand("PKG_SHIFTCHANGE.SPROC_GETMANESARHEADFORGSHIFT", objCn);
            }
            else
            {
                objCmd = new OracleCommand("PKG_SHIFTCHANGE.SPROC_SHIFTCHANGEAPPLIST_GET", objCn);
            }
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.Parameters.Add("CUR_OVERSTAY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = "6001";// HttpContext.Current.Session["userID"];
            using (objCn)
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    // OracleDataReader objReader = objCmd.ExecuteReader();
                    OracleDataAdapter adp = new OracleDataAdapter(objCmd);
                    //DataTable dt = new DataTable();
                    adp.Fill(ds);
                    //if (dt.Rows.Count > 0)
                    //{
                    //    js = JsonConvert.SerializeObject(dt);
                    //}
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return ds;
        }

    }
}
