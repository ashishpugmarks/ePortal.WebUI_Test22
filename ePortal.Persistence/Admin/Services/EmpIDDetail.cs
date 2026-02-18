
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
    public class EmpIDDetail : IEmpIDDetail
    {
        #region "Local Variables"
        private readonly IDataManagement odmgt;
        private readonly IConnectionString objCnStr;
        DataTable dt;

        public EmpIDDetail(IDataManagement _odmgt, IConnectionString _objCnStr)
        {
            odmgt = _odmgt;
            objCnStr = _objCnStr;
        }
        #endregion

        #region "Get "
        public DataTable Empuanexport(string strecode, string strename)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_EXPORTEMPUANDETAIL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("V_EMPNAME", OracleDbType.Varchar2).Value = strename;
            ocmd.Parameters.Add("CUR_EMPID", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return odmgt.GetDataTable(ocmd);
        }


        #endregion

        #region "Update & Insert"

        public String UpdateEmpUAN(string strEmpcode, string strempuan, string stremppfno, string strAddedby)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_EMPUAN_SET";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("V_UAN", OracleDbType.Int32).Value = strempuan;
            ocmd.Parameters.Add("V_PFNO", OracleDbType.Varchar2).Value = stremppfno;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public String UpdateEmpIDDetail(string strEmpcode, string strempuan, string straadhar, string strpassport, string strpassportexpdate, string strdl, string strdlexpdate, string strpfno, string strrationcard, string strelectioncard, string strnprno, string strAddedby, string stractive)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_EMPIDDETAIL_SET";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("V_UAN", OracleDbType.Varchar2).Value = strempuan;
            ocmd.Parameters.Add("V_AADHARCARD_NO", OracleDbType.Varchar2).Value = straadhar;
            ocmd.Parameters.Add("V_PASSPORT_NO", OracleDbType.Varchar2).Value = strpassport;
            ocmd.Parameters.Add("V_PASSPORT_EXPDATE", OracleDbType.Varchar2).Value = strpassportexpdate;
            ocmd.Parameters.Add("V_DL_NO", OracleDbType.Varchar2).Value = strdl;
            ocmd.Parameters.Add("V_DL_EXPDATE", OracleDbType.Varchar2).Value = strdlexpdate;
            ocmd.Parameters.Add("V_PFNO", OracleDbType.Varchar2).Value = strpfno;
            ocmd.Parameters.Add("V_RATIONCARDNO", OracleDbType.Varchar2).Value = strrationcard;
            ocmd.Parameters.Add("V_ELECTIONCARDNO", OracleDbType.Varchar2).Value = strelectioncard;
            ocmd.Parameters.Add("V_NPRNO", OracleDbType.Varchar2).Value = strnprno;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("V_ACTIVE", OracleDbType.Int32).Value = stractive;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public String UpdateEmpAadhar(string strEmpcode, string strempaadhar, string strAddedby)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_EMPAADHAR_SET";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("V_AADHARCARD_NO", OracleDbType.Int64).Value = strempaadhar;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }
        public String UpdateEmpDLno(string strEmpcode, string strempdlno, DateTime strempdlexpdate, string strAddedby)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_EMPDLNO_SET";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("V_DL_NO", OracleDbType.Varchar2).Value = strempdlno;
            ocmd.Parameters.Add("V_DL_EXPDATE", OracleDbType.Date).Value = strempdlexpdate;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }
        public String UpdateEmpPassportno(string strEmpcode, string stremppassportno, DateTime stremppassportexpdate, string strAddedby)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_EMPPASSPORTNO_SET";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("V_PASSPORT_NO", OracleDbType.Varchar2).Value = stremppassportno;
            ocmd.Parameters.Add("V_PASSPORT_EXPDATE", OracleDbType.Date).Value = stremppassportexpdate;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }
        public String UpdateEmpRation(string strEmpcode, string strempration, string strAddedby)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_EMPRATION_SET";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("V_RATIONCARDNO", OracleDbType.Varchar2).Value = strempration;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public String UpdateEmpelection(string strEmpcode, string strempelection, string strAddedby)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_ELECTIONCARD_SET";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("V_ELECTIONCARDNO", OracleDbType.Varchar2).Value = strempelection;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }
        public String UpdateEmpnpr(string strEmpcode, string strempnpr, string strAddedby)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_NPR_SET";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("V_NPRNO", OracleDbType.Varchar2).Value = strempnpr;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        //Below added by aumento for the SR58568==================================
        // Added by aumento for SR82082 as on 15112024--------------------------------------------------------------
        public String InsertEmpUnblockDetail(string strAdempcode, DateTime strAddedDate, string strAddedby, string strflag, string Reason)
        // ended by aumento for SR82082 as on 15112024--------------------------------------------------------------
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_INSEMP_UNBLOCKDTL";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strAdempcode;
            ocmd.Parameters.Add("V_ADDEDDATE", OracleDbType.Date).Value = strAddedDate;
            //ocmd.Parameters.Add("V_DL_EXPDATE", OracleDbType.Date).Value = strempdlexpdate;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("V_Flag", OracleDbType.Varchar2, 200).Value = strflag;
            ocmd.Parameters.Add("V_Reason", OracleDbType.Varchar2, 200).Value = Reason;//Added by aumento for SR82082 as on 15112024---------------------------------
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }


        #endregion
        #region "Get "
        public DataTable GetUNB_Emp_DTL(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_UNBEMPDTL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_EMPID", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            dt = odmgt.GetDataTable(ocmd);
            return dt;
        }
        //==============================================================================

        //Below added by aumento for the SR62958==================================
        //Added by aumento for SR82082 as on 15112024---------------------------------
        //public String InsertEmpblockDetail(string strAdempcode, DateTime strAddedDate, string strAddedby, string strflag)
        public String InsertEmpblockDetail(string strAdempcode, DateTime strAddedDate, string strAddedby, string strflag, string Reason)
        //Endded by aumento for SR82082 as on 15112024--------------------------------
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_INSEMP_BLOCKDTL";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strAdempcode;
            ocmd.Parameters.Add("V_ADDEDDATE", OracleDbType.Date).Value = strAddedDate;
            //ocmd.Parameters.Add("V_DL_EXPDATE", OracleDbType.Date).Value = strempdlexpdate;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("V_Flag", OracleDbType.Varchar2, 200).Value = strflag;
            ocmd.Parameters.Add("V_Reason", OracleDbType.Varchar2, 200).Value = Reason;//Added by aumento for SR82082 as on 15112024---------------------------------
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }


        #endregion
        #region "Get "
        public DataTable GetB_Emp_DTL(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_BEMPDTL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_EMPID", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            dt = odmgt.GetDataTable(ocmd);
            return dt;
        }
        //added by aumento for SR82082 on 18112024======================================
        public DataTable GetStatusDetails()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SP_GET_STATUS_DETAILS"; // Your stored procedure name here
            ocmd.Parameters.Add("CUR_EMPID", OracleDbType.RefCursor).Direction = ParameterDirection.Output; // Output cursor
            ocmd.BindByName = true;

            return odmgt.GetDataTable(ocmd); // Call to your data access method
        }



        public DataTable GetEmpBlockUnblockReport(string strType, string strEmployeeCode, string FromDate, string ToDate, string strStatus, string strUserID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SP_GET_BlOCKUNBLOCK_REPORT"; // Your stored procedure name here
            ocmd.Parameters.Add("V_TYPE", OracleDbType.Varchar2).Value = strType;
            ocmd.Parameters.Add("V_EMPLOYEECODE", OracleDbType.Varchar2).Value = strEmployeeCode;
            ocmd.Parameters.Add("V_FROMDATE", OracleDbType.Varchar2).Value = FromDate;
            ocmd.Parameters.Add("V_TODATE", OracleDbType.Varchar2).Value = ToDate;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("V_USER", OracleDbType.Varchar2).Value = strUserID;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output; // Output cursor
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;

            return odmgt.GetDataTable(ocmd); // Call to your data access method
        }
        public DataTable GetB_Emp_Name(string StrTerm)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_EMPDTL_LIST_GET";

            // Adding parameters
            // Assuming strecode is a string that can be converted to a numeric type
            ocmd.Parameters.Add("V_Term", OracleDbType.NVarchar2).Value = StrTerm; // Input parameter (NUMBER)
            ocmd.Parameters.Add("CUR_EMPID", OracleDbType.RefCursor).Direction = ParameterDirection.Output; // Output parameter

            // Bind parameters by name
            ocmd.BindByName = true;

            // Execute and return the result as DataTable
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetLogin_Emp_Name(string StrTerm)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_ASREMPIDDETAIL.SPROC_EMPDTL_GET_LOGIN_USERID";

            // Adding parameters
            // Assuming strecode is a string that can be converted to a numeric type
            ocmd.Parameters.Add("V_Term", OracleDbType.NVarchar2).Value = StrTerm; // Input parameter (NUMBER)
            ocmd.Parameters.Add("CUR_EMPID", OracleDbType.RefCursor).Direction = ParameterDirection.Output; // Output parameter

            // Bind parameters by name
            ocmd.BindByName = true;

            // Execute and return the result as DataTable
            return odmgt.GetDataTable(ocmd);
        }


        //endded by aumento for SR82082 on 18112024======================================
        //==============================================================================
        #endregion

    }
}
