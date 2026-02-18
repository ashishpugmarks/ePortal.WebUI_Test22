using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Admin.Services
{
    public class ManageSelfPending: IManageSelfPending
    {
        private readonly IDataManagement oDataMgmt;

        public ManageSelfPending(IDataManagement _oDataMgmt)
        {
            oDataMgmt = _oDataMgmt;
        }


        //DataTable objDt;
        //OracleCommand cmd;

        //private readonly ConnectionString _conn;
        //private readonly DataManagement oDataMgmt;


        //public CommonFunctions(ConnectionString conn, DataManagement _oDataMgmt)
        //{
        //    oDataMgmt = _oDataMgmt;
        //    _conn = conn;
        //}



        public long GetTotalSelfPending(Int32 strEmpCode)
        {
            int RowCount = 0;
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "SPROC_PENDINGSELFCOUNT";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Int32).Value = strEmpCode;
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = null;
            oCmd.Parameters.Add("NOOFCOUNT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oDataMgmt.ExecuteQuery(oCmd);
            RowCount = System.Convert.ToInt32(oCmd.Parameters["NOOFCOUNT"].Value.ToString());
            return RowCount;
        }
        //Trainging Feedback Pending List.
        public DataTable GetPendingTraning(string strEmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_SELFPENDING.SPROC_TRNGFEEDBACKPENDDING_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
            oCmd.Parameters.Add("CUR_TRAINING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET PENDING REQUEST LIST FOR Tour Settlement
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public DataTable GetTourSettPendinglist(string strEmpcode)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_SELFPENDING.SPROC_PENDINGTOURSETTLE_GET";
            oCmd.Parameters.Add("V_APPEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            return oDataMgmt.GetDataTable(oCmd);
        }

        /// GET PENDING REQUEST LIST FOR Resignation Clearance
        public DataTable ResigClearancePending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_RESIGCLRPENDING";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_CLRDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        /// GET PENDING REQUEST LIST FOR Resignation Exit
        public DataTable ResigExitProcessPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_RESIGNATIONEXITPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_RESIGPROCESS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        /// GET PENDING REQUEST LIST FOR Taxation Income Tax Declaration
        public DataTable IncomeTaxDecPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_GETPENDININCOMETAXDEC";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        /// GET PENDING REQUEST LIST FOR Taxation Income Tax Proof Submission
        public DataTable IncomeTaxProofSubPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_GETPENDINGTAXPROOFSUB";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }
        /// GET PENDING REQUEST LIST FOR Superior Evaluation
        public DataTable SuperiorEvalPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_SUPEVALDASHBOARDPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_FHSUPEVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        /// GET PENDING REQUEST LIST FOR Car Perk
        public DataTable CarPerkSystemPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_GETEMPCARPENDINGDETAIL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("V_DESIGNATIONID", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_EMPCARDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        ///// GET PENDING REQUEST LIST FOR Recruitment job description
        public DataTable RecruitmentJobDescPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSJOBDESCRIPTIONPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.dispose();
        }

        /// GET PENDING REQUEST LIST FOR PMS Goal Setting Self
        public DataTable GoalSettingSelfPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_GETGOALSETTINSELFPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        /// GET PENDING REQUEST LIST FOR PMS Goal Setting Evaluation
        public DataTable GetGoalSettEvalPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_GETGOALSETTINEVALPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        /// GET PENDING REQUEST LIST FOR PMS Goal Setting Reviewer
        public DataTable GetGoalSettReviewerPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_GETGOALSETREVIEWPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        /// GET PENDING REQUEST LIST FOR PMS First half self
        public DataTable FirstHalfSelfPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSFIRSTHALFSELFPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }
        /// GET PENDING REQUEST LIST FOR PMS First half Evaluation
        public DataTable GetFirsstHalfEvalPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSFIRSTHALFEVLULPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }
        /// GET PENDING REQUEST LIST FOR PMS First half Evaluation
        public DataTable GetFirsstHalfReviewPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSFIRSTHREVIEWPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }
        /// GET PENDING REQUEST LIST FOR PMS second half 
        public DataTable GetSecondHalfPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSSECONDHALFPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }
        // GET PENDING REQUEST LIST FOR PMS second half Evaluation
        public DataTable SecondHalfEvalPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSSECONDHALFEVALPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }
        // GET PENDING REQUEST LIST FOR PMS second half Reviewer
        public DataTable ReviewerPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSSECONDREVIEWPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        // GET PENDING REQUEST LIST FOR PMS Normalization FH DIV Head
        public DataTable NormalizationFHDivPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSNORMALFHPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        // GET PENDING REQUEST LIST FOR PMS Normalization FH OP Head
        public DataTable NormalizationFHOPPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSNORMALFHOPPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        // GET PENDING REQUEST LIST FOR PMS Normalization SH Div Head
        public DataTable NormalizationSHDivPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSNORMALSHDIVPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        // GET PENDING REQUEST LIST FOR PMS Normalization SH OP Head
        public DataTable NormalizationSHOPPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_PMSNORMALSHOPPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }

        //WFH Request pending start -- Need to make change in procedure and same changes also required in below method
        public DataTable FetchWFHReqPending(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_SELFPENDING.SPROC_FETCHWFHREQPENDING";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(ocmd);
            //ocmd.Dispose();
        }
        //WFH Request pending end

    }
}
