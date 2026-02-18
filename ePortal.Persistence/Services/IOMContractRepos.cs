using ePortal.Persistence.Interface;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.RegularExpressions;

namespace ePortal.Persistence.Services
{
    public class IOMContractRepos : IIOMContractRepos
    {
        private readonly ILogger<IOMContractRepos> _logger;
        private readonly IDataManagement oDataMgmt;
        public IOMContractRepos(IDataManagement _IDataManagement, ILogger<IOMContractRepos> logger)
        {
            _logger = logger;
            oDataMgmt = _IDataManagement;
        }


        public string INSERTCONTRACTMASTER(string contractname, string initialdesc, string status, string contractid, string addedby)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_INSERTCONTRACTMASTER";
            ocmd.Parameters.Add("CONTRACTNAME_IN", OracleDbType.Varchar2).Value = contractname;
            ocmd.Parameters.Add("INITIALDESC_IN", OracleDbType.Varchar2).Value = initialdesc;
            ocmd.Parameters.Add("CONTRACTID_IN", OracleDbType.Varchar2).Value = contractid;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
        }

        public DataSet GETCONTRACTMASTER(string contratdesc, string initialdesc, string status, string CONTRACTID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_GETCONTRACTMASTER_GET";
            ocmd.Parameters.Add("CONTRACTDESC_IN", OracleDbType.Varchar2).Value = contratdesc;
            ocmd.Parameters.Add("INITIALDESC_IN", OracleDbType.Varchar2).Value = initialdesc;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("CONTRACTID_IN", OracleDbType.Varchar2).Value = CONTRACTID;
            ocmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataSet GET_AGREEMENTMASTER(string AGREEMENTNAME, string INITIALDESC, string AGREEMENTID, string STATUS, string SLA)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_AGREEMENTMASTER_GET";
            ocmd.Parameters.Add("AGREEMENTNAME_IN", OracleDbType.Varchar2).Value = AGREEMENTNAME;
            ocmd.Parameters.Add("INITIALDESC_IN", OracleDbType.Varchar2).Value = INITIALDESC;
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = AGREEMENTID;
            ocmd.Parameters.Add("SLA_IN", OracleDbType.Varchar2).Value = SLA;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string INSERTAGREEMENTMASTER(string agreementname, string initialdesc, string status, string agreementid, string addedby, string sla)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_INSERTAGREEMENTMASTER";
            ocmd.Parameters.Add("AGREEMENTNAME_IN", OracleDbType.Varchar2).Value = agreementname;
            ocmd.Parameters.Add("INITIALDESC_IN", OracleDbType.Varchar2).Value = initialdesc;
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = agreementid;
            ocmd.Parameters.Add("SLA_IN", OracleDbType.Varchar2).Value = sla;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
        }

        public DataSet GetIOMFullDetail(string IOMID, string Ecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_IOMAPPROVALDETAIL";
            ocmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = IOMID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Ecode;
            ocmd.Parameters.Add("CUR_EMPDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_IOMDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_IOMHISTORY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_AUTHDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_COMMHISTORY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataTable GETIOMPENDINGREQUEST(string ecode, string status)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_IOM.SPROC_IOMPENDINGREQUEST_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("CUR_PENDREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }

        public DataTable GETIOMAPPROVALHISTORY(string ecode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_IOM.SPROC_IOMAPPROVALHISTORY_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("CUR_HISTREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }

        public DataTable GETPENDINGADMINREQUEST(string REQECODE, string REQENAME, string ECODE, string STATUS, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string IOMID, string VENDNAME)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_IOM.SPROC_IOMADMINPENDINGREQ_GET";
            oCmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = REQECODE;
            oCmd.Parameters.Add("REQENAME_IN", OracleDbType.Varchar2).Value = REQENAME;
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            oCmd.Parameters.Add("CONTRACT_IN", OracleDbType.Varchar2).Value = CONTRACT;
            oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            oCmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = IOMID;
            oCmd.Parameters.Add("VENDERNAME_IN", OracleDbType.Varchar2).Value = VENDNAME;
            oCmd.Parameters.Add("CUR_ADMINREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }


        public DataSet GetNextApprovalAuthority(string Ecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_GetNextAppAuhtority_Get";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Ecode;
            ocmd.Parameters.Add("CUR_APPDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string ADMINIOMAPPROVAL(string HDIOMID, string Empcode, string strRemarks, string status, string agreement, string agreementtype, string UniqueId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_SUBMITIOMAPPROVALDETAIL";
            ocmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = HDIOMID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Empcode;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("AGREEMENT_IN", OracleDbType.Varchar2).Value = agreement;
            ocmd.Parameters.Add("AGREEMENTTYPE_IN", OracleDbType.Varchar2).Value = agreementtype;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("UNIQUEID_IN", OracleDbType.Varchar2).Value = UniqueId;

            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string UPDATEUSERACKNOWLEDGEMENT(string HDIOMID, string Empcode, string strRemarks)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_UPDATEUSERACKNOWLEDGE";
            ocmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = HDIOMID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Empcode;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string UPDATEFINALDOCUMENT(string HDIOMID, string Empcode, string strRemarks, string finaldoc)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_UPDATEFINALDOCUMENT";
            ocmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = HDIOMID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Empcode;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
            ocmd.Parameters.Add("FINALDOC_IN", OracleDbType.Varchar2).Value = finaldoc;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable GETIOMREPORTDATA(string REQECODE, string REQENAME, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string UNIQUEID, string VENDORNAME, string IOMID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_IOM.SPROC_IOMREPORT_Get";
            oCmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = REQECODE;
            oCmd.Parameters.Add("REQENAME_IN", OracleDbType.Varchar2).Value = REQENAME;

            oCmd.Parameters.Add("CONTRACT_IN", OracleDbType.Varchar2).Value = CONTRACT;
            oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            oCmd.Parameters.Add("UNIQUEID_IN", OracleDbType.Varchar2).Value = UNIQUEID;
            oCmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            oCmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = IOMID;

            oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            oCmd.Parameters.Add("CUR_IOMREPORT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }

        public DataSet GETCONTRACTLIST(string contracttype, string vendorname, string expdatefrom, string expdateto, string ecode, string agreementid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_CONTRACTLIST_GET";
            ocmd.Parameters.Add("CONTRACTYPE_IN", OracleDbType.Varchar2).Value = contracttype;
            ocmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = vendorname;
            ocmd.Parameters.Add("EXPDATEFROM_IN", OracleDbType.Varchar2).Value = expdatefrom;
            ocmd.Parameters.Add("EXPDATETO_IN", OracleDbType.Varchar2).Value = expdateto;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = agreementid;
            ocmd.Parameters.Add("CUR_CONTRACTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string closecontract(string agreementid, string closeremarks, string ecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_CLOSECONTRACT";
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = agreementid;
            ocmd.Parameters.Add("CLOSEREMARK_IN", OracleDbType.Varchar2).Value = closeremarks;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string CONTRACTRENEWAL(string fromiom, string agreementtype, string Contracttype, string EffectiveDate, string Termmonth, string Termyear, string Dateofexpiry, string Mannerofpayment, string Purpose, string Remarks, string addedby, string finaldoc, string VendorName, string agreementid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_CONTRACTRENEWAL";
            ocmd.Parameters.Add("FROM_IN", OracleDbType.Varchar2).Value = fromiom;
            ocmd.Parameters.Add("AGREEMENTTYPE_IN", OracleDbType.Varchar2).Value = agreementtype;
            ocmd.Parameters.Add("CONTRACTTYPE_IN", OracleDbType.Varchar2).Value = Contracttype;
            ocmd.Parameters.Add("DATEOFAGREEMENT_IN", OracleDbType.Varchar2).Value = EffectiveDate;
            ocmd.Parameters.Add("TERMMONTH_IN", OracleDbType.Varchar2).Value = Termmonth;
            ocmd.Parameters.Add("TERMYEAR_IN", OracleDbType.Varchar2).Value = Termyear;
            ocmd.Parameters.Add("DATEOFEXPIRY_IN", OracleDbType.Varchar2).Value = Dateofexpiry;
            ocmd.Parameters.Add("MANNEROFPAYMENT_IN", OracleDbType.Varchar2).Value = Mannerofpayment;
            ocmd.Parameters.Add("PURPOSE_IN", OracleDbType.Varchar2).Value = Purpose;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
            ocmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VendorName;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("FINALDOC_IN", OracleDbType.Varchar2).Value = finaldoc;
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = agreementid;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable GET_MANAGEIOMDETAILS(string ecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_MANAGEIOMAPPROVAL_GET";
            ocmd.Parameters.Add("CUR_APPDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ecode;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GetDashBoardCount(string strvpid, string strdivid, string strdptid, string strsecid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_LIVECONTRACTREPORT_GET";
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = strvpid;
            ocmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = strdivid;
            ocmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strdptid;
            ocmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strsecid;
            ocmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataTable(ocmd);
            ocmd.Dispose();

        }

        public DataSet GetContractount_contracttype(string strctype, string stroperation, string strdivision, string strdeprtment)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_CONTRACTCOUNT_Get";
            ocmd.Parameters.Add("CONTRACTTYPE_IN", OracleDbType.Varchar2).Value = strctype;
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = stroperation;
            ocmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = strdivision;
            ocmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strdeprtment;
            ocmd.Parameters.Add("CUR_IOMREPORT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_RENVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_WIPIOM", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();

        }

        public DataSet GetContrac_Detail(string strctype, string strvpid, string vendorname)
        {


            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_CONTRACTDETAIL_Get";
            ocmd.Parameters.Add("CONTRACTTYPE_IN", OracleDbType.Varchar2).Value = strctype;
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = strvpid;
            ocmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = vendorname;
            ocmd.Parameters.Add("CUR_IOMREPORT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_RENVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();

           
        }

        public DataSet GETIOMBYAGREEMENTID(string AGREEMENTID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_ALLIOMBYAGRMNTID_GET";
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = AGREEMENTID;
            ocmd.Parameters.Add("CUR_IOMDET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataSet GETLIVECONTRACTREPORT(string OPERATIONID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_LIVECONTRACTREPORT_GET";
            ocmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = OPERATIONID;
            ocmd.Parameters.Add("CUR_REPOPEATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_REPCONTRACTS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string INSERTSTATUSMASTER(string statusName, string status, string statusid, string addedby)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_INSERTSTATUSMASTER";
            ocmd.Parameters.Add("STATUSNAME_IN", OracleDbType.Varchar2).Value = statusName;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("STATUSID_IN", OracleDbType.Varchar2).Value = statusid;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            ocmd.Dispose();
        }

        public DataSet GETSTATUSMASTER(string STATUSID, string STATUSNAME, string STATUS)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_STATUSMASTER_GET";
            ocmd.Parameters.Add("STATUSID", OracleDbType.Varchar2).Value = STATUSID;
            ocmd.Parameters.Add("STATUSNAME", OracleDbType.Varchar2).Value = STATUSNAME;
            ocmd.Parameters.Add("STATUS", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("CUR_STSLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataTable GETCONTRACTREPORTDATA(string CONTRACT, string OPERATION, string STATUS, string VENDORNAME, string DIVISION, string DEPARTMENT, string UNIQUEID, string IOMID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_CONTRACTREPORTDATA_GET";
            ocmd.Parameters.Add("CONTRACT_IN", OracleDbType.Varchar2).Value = CONTRACT;
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            ocmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = IOMID;
            ocmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            ocmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            ocmd.Parameters.Add("UNIQUEID_IN", OracleDbType.Varchar2).Value = UNIQUEID;
            ocmd.Parameters.Add("CUR_RPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GETBACKDATEREPORTDATA(string REQECODE, string REQENAME, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string UNIQUEID, string VENDORNAME, string IOMID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_IOM.SPROC_BACKDATEREPORT_GET";
            oCmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = REQECODE;
            oCmd.Parameters.Add("REQENAME_IN", OracleDbType.Varchar2).Value = REQENAME;
            oCmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = IOMID;
            oCmd.Parameters.Add("UNIQUEID_IN", OracleDbType.Varchar2).Value = UNIQUEID;
            oCmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            oCmd.Parameters.Add("CONTRACT_IN", OracleDbType.Varchar2).Value = CONTRACT;
            oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            oCmd.Parameters.Add("CUR_BACKDATRPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }

        public DataSet ViewAgreementDetail(string AGREEMENTID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_VIEWAGREEMENTDETAIL_GET";
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = AGREEMENTID;
            ocmd.Parameters.Add("CUR_VIEWCONTRACT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string INSERT_IOMDEATIL_ADMIN(string From, string AgrementType, string ContractType, string EffectiveDate, string Termmonth, string Termyear, string Dateofexpiry, string Mannerofpayment, string Purpose, string Remarks, string ADDEDBY, string VendorName, string finaldocument, string divisionid, string departmentid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.INSERT_IOMDEATIL_ADMIN";
            ocmd.Parameters.Add("FROM_IN", OracleDbType.Varchar2).Value = From;
            ocmd.Parameters.Add("AGREEMENTTYPE_IN", OracleDbType.Varchar2).Value = AgrementType;
            ocmd.Parameters.Add("CONTRACTTYPE_IN", OracleDbType.Varchar2).Value = ContractType;
            ocmd.Parameters.Add("DATEOFAGREEMENT_IN", OracleDbType.Varchar2).Value = EffectiveDate;
            ocmd.Parameters.Add("TERMMONTH_IN", OracleDbType.Varchar2).Value = Termmonth;
            ocmd.Parameters.Add("TERMYEAR_IN", OracleDbType.Varchar2).Value = Termyear;
            ocmd.Parameters.Add("DATEOFEXPIRY_IN", OracleDbType.Varchar2).Value = Dateofexpiry;
            ocmd.Parameters.Add("MANNEROFPAYMENT_IN", OracleDbType.Varchar2).Value = Mannerofpayment;
            ocmd.Parameters.Add("PURPOSE_IN", OracleDbType.Varchar2).Value = Purpose;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
            ocmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VendorName;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ADDEDBY;
            ocmd.Parameters.Add("FINALDOCUMENT_IN", OracleDbType.Varchar2).Value = finaldocument;
            ocmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = departmentid;
            ocmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = divisionid;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string Insert_IOMDetail(string From, string AgrementType, string ContractType, string EffectiveDate, string Termmonth,
        string Termyear, string Dateofexpiry, string Mannerofpayment, string Purpose, string Remarks, string ADDEDBY,
        string agreementfile, string referencefile, string VendorName, string agreementid, string antibribery, string considerable,
        string authlevel, string mstagr, string depid, string divid, string OPHEADECODE, string CONTRACTECODE, string SURETYAMOUNT,
        string NAMEOFSURETY, string RETURNFROMDATE, string RETURNTODATE, int IT_DeclareValue) // IT_DeclareValue  Added by Aumento :: SR79956
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_INSERTIOMDETAILS";
            ocmd.Parameters.Add("FROM_IN", OracleDbType.Varchar2).Value = From;
            ocmd.Parameters.Add("AGREEMENTTYPE_IN", OracleDbType.Varchar2).Value = AgrementType;
            ocmd.Parameters.Add("CONTRACTTYPE_IN", OracleDbType.Varchar2).Value = ContractType;
            ocmd.Parameters.Add("DATEOFAGREEMENT_IN", OracleDbType.Varchar2).Value = EffectiveDate;
            ocmd.Parameters.Add("TERMMONTH_IN", OracleDbType.Varchar2).Value = Termmonth;
            ocmd.Parameters.Add("TERMYEAR_IN", OracleDbType.Varchar2).Value = Termyear;
            ocmd.Parameters.Add("DATEOFEXPIRY_IN", OracleDbType.Varchar2).Value = Dateofexpiry;
            ocmd.Parameters.Add("MANNEROFPAYMENT_IN", OracleDbType.Varchar2).Value = Mannerofpayment;
            ocmd.Parameters.Add("PURPOSE_IN", OracleDbType.Varchar2).Value = Purpose;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
            ocmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VendorName;
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = agreementid;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ADDEDBY;
            ocmd.Parameters.Add("AGREEMENTFILE_IN", OracleDbType.Varchar2).Value = agreementfile;
            ocmd.Parameters.Add("REFERENCEFILE_IN", OracleDbType.Varchar2).Value = referencefile;
            ocmd.Parameters.Add("ANTIBRIBERY_IN", OracleDbType.Varchar2).Value = antibribery;
            ocmd.Parameters.Add("NDADOCUMENT_IN", OracleDbType.Varchar2).Value = considerable;
            ocmd.Parameters.Add("AUTHLEVEL_IN", OracleDbType.Varchar2).Value = authlevel;
            ocmd.Parameters.Add("MSTAGR_IN", OracleDbType.Varchar2).Value = mstagr;
            ocmd.Parameters.Add("DEPID_IN", OracleDbType.Varchar2).Value = depid;
            ocmd.Parameters.Add("DIVID_IN", OracleDbType.Varchar2).Value = divid;
            ocmd.Parameters.Add("OPHEADECODE_IN", OracleDbType.Varchar2).Value = OPHEADECODE;
            ocmd.Parameters.Add("CONTRACTECODE_IN", OracleDbType.Varchar2).Value = CONTRACTECODE;
            ocmd.Parameters.Add("SURETYAMOUNT_IN", OracleDbType.Varchar2).Value = SURETYAMOUNT;
            ocmd.Parameters.Add("NAMEOFSURETY_IN", OracleDbType.Varchar2).Value = NAMEOFSURETY;
            ocmd.Parameters.Add("RETURNFROMDATE_IN", OracleDbType.Varchar2).Value = RETURNFROMDATE;
            ocmd.Parameters.Add("RETURNTODATE_IN", OracleDbType.Varchar2).Value = RETURNTODATE;
            ocmd.Parameters.Add("IT_DECLARE_IN", OracleDbType.Int32).Value = IT_DeclareValue; // IT_DeclareValue  Added by Aumento :: SR79956
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string Update_IOMDetail(string HDIOMID, string Empcode, string strRemarks, string status, string backdateremark, string authlevel, string OPHEADECODE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_UPDATEIOMAPPROVAL";
            ocmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = HDIOMID;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = Empcode;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("BACKDATEREMARK_IN", OracleDbType.Varchar2).Value = backdateremark;
            ocmd.Parameters.Add("AUTHLEVEL_IN", OracleDbType.Varchar2).Value = authlevel;
            ocmd.Parameters.Add("OPHEADECODE_IN", OracleDbType.Varchar2).Value = OPHEADECODE;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string UPDATEIOMDETAIL(string fromiom, string agreementtype, string Contracttype, string EffectiveDate,
        string Termmonth, string Termyear, string Dateofexpiry, string Mannerofpayment, string Purpose, string Remarks,
        string addedby, string agreementfile, string approvalnotefile, string VendorName, string IOMID, string antibribery,
        string ndadocument, string authlevel, string mstagr, string depid, string divid, string OPHEADECODE, int IT_DeclareValue) // IT_DeclareValue  Added by Aumento :: SR79956
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_UPDATEIOMDETAILAFTERSB";
            ocmd.Parameters.Add("FROM_IN", OracleDbType.Varchar2).Value = fromiom;
            ocmd.Parameters.Add("AGREEMENTTYPE_IN", OracleDbType.Varchar2).Value = agreementtype;
            ocmd.Parameters.Add("CONTRACTTYPE_IN", OracleDbType.Varchar2).Value = Contracttype;
            ocmd.Parameters.Add("DATEOFAGREEMENT_IN", OracleDbType.Varchar2).Value = EffectiveDate;
            ocmd.Parameters.Add("TERMMONTH_IN", OracleDbType.Varchar2).Value = Termmonth;
            ocmd.Parameters.Add("TERMYEAR_IN", OracleDbType.Varchar2).Value = Termyear;
            ocmd.Parameters.Add("DATEOFEXPIRY_IN", OracleDbType.Varchar2).Value = Dateofexpiry;
            ocmd.Parameters.Add("MANNEROFPAYMENT_IN", OracleDbType.Varchar2).Value = Mannerofpayment;
            ocmd.Parameters.Add("PURPOSE_IN", OracleDbType.Varchar2).Value = Purpose;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
            ocmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VendorName;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("AGREEMENTFILE_IN", OracleDbType.Varchar2).Value = agreementfile;
            ocmd.Parameters.Add("REFERENCEFILE_IN", OracleDbType.Varchar2).Value = approvalnotefile;
            ocmd.Parameters.Add("ANTIBRIBERY_IN", OracleDbType.Varchar2).Value = antibribery;
            ocmd.Parameters.Add("NDADOCUMENT_IN", OracleDbType.Varchar2).Value = ndadocument;
            ocmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = IOMID;
            ocmd.Parameters.Add("AUTHLEVEL_IN", OracleDbType.Varchar2).Value = authlevel;
            ocmd.Parameters.Add("MSTAGR_IN", OracleDbType.Varchar2).Value = mstagr;
            ocmd.Parameters.Add("DEPID_IN", OracleDbType.Varchar2).Value = depid;
            ocmd.Parameters.Add("DIVID_IN", OracleDbType.Varchar2).Value = divid;
            ocmd.Parameters.Add("OPHEADECODE_IN", OracleDbType.Varchar2).Value = OPHEADECODE;
            ocmd.Parameters.Add("IT_DECLARE_IN", OracleDbType.Int32).Value = IT_DeclareValue; // IT_DeclareValue  Added by Aumento :: SR79956
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string RemoveSpecialChar(string str)
        {
            string newstr = Regex.Replace(str, @"[^0-9a-zA-Z ]+", "");
            return newstr;
        }

        public DataSet GetUserWithLegalApprovalRights()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_CONTRACTAPPRIGHTS_GET";
            ocmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }
        public DataTable GETADMINCONTRACTSTATUS(string REQECODE, string REQENAME, string ECODE, string STATUS, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string UNIQUEID, string IOMID, string VENDNAME/* ,STATUSID*/)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_IOM.SPROC_IOMCONTRSTATUSREPORT_GET";
            oCmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = REQECODE;
            oCmd.Parameters.Add("REQENAME_IN", OracleDbType.Varchar2).Value = REQENAME;
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            oCmd.Parameters.Add("CONTRACT_IN", OracleDbType.Varchar2).Value = CONTRACT;
            oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            oCmd.Parameters.Add("UNIQUEID_IN", OracleDbType.Varchar2).Value = UNIQUEID;
            oCmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = IOMID;
            oCmd.Parameters.Add("VENDERNAME_IN", OracleDbType.Varchar2).Value = VENDNAME;
            //oCmd.Parameters.Add("STATUSID_IN", OracleDbType.Varchar2).Value = STATUSID;
            oCmd.Parameters.Add("CUR_ADMINREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }
        public string SUBMITCOMMUNICATION(string HDIOMID, string Empcode, string strRemarks, string refDoc, string UserType, int COMMUNICATIONTYPE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_SUBMITIOMCOMMUNICATION";
            ocmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = HDIOMID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Empcode;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
            ocmd.Parameters.Add("AGREEMENT_IN", OracleDbType.Varchar2).Value = refDoc;
            ocmd.Parameters.Add("USERTYPE_IN", OracleDbType.Varchar2).Value = UserType;
            ocmd.Parameters.Add("COMMUNICATIONTYPE_IN", OracleDbType.Varchar2).Value = COMMUNICATIONTYPE;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        /////////// Added by
        public DataTable GETAPPROVEDADMINREQUEST(string REQECODE, string REQENAME, string ECODE, string STATUS, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string IOMID, string VENDNAME, string UNIQUEID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_IOM.SPROC_IOMREQ_FORPRINT_GET";
            oCmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = REQECODE;
            oCmd.Parameters.Add("REQENAME_IN", OracleDbType.Varchar2).Value = REQENAME;
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            oCmd.Parameters.Add("UNIQUEID_IN", OracleDbType.Varchar2).Value = UNIQUEID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            oCmd.Parameters.Add("CONTRACT_IN", OracleDbType.Varchar2).Value = CONTRACT;
            oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            oCmd.Parameters.Add("IOMID_IN", OracleDbType.Varchar2).Value = IOMID;
            oCmd.Parameters.Add("VENDERNAME_IN", OracleDbType.Varchar2).Value = VENDNAME;
            oCmd.Parameters.Add("CUR_ADMINREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            oCmd.Dispose();
            return (dt);
        }

        /////////// Added by 
        public string SUBMITUPDATEOPERATION(string HDIOMID, string Empcode, string operationType)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_SUBMITUPDATEOPERATION";
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = HDIOMID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = Empcode;
            ocmd.Parameters.Add("OPERATIONTYPE_IN", OracleDbType.Varchar2).Value = operationType;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataSet GETIOMLIST_FOROPERATION(string contracttype, string vendorname, string expdatefrom, string expdateto, string OPERATION, string agreementid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_IOM.SPROC_IOMLIST_FOROPERATION_GET";
            ocmd.Parameters.Add("CONTRACTYPE_IN", OracleDbType.Varchar2).Value = contracttype;
            ocmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = vendorname;
            ocmd.Parameters.Add("EXPDATEFROM_IN", OracleDbType.Varchar2).Value = expdatefrom;
            ocmd.Parameters.Add("EXPDATETO_IN", OracleDbType.Varchar2).Value = expdateto;
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            ocmd.Parameters.Add("AGREEMENTID_IN", OracleDbType.Varchar2).Value = agreementid;
            ocmd.Parameters.Add("CUR_CONTRACTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();



        }


    }
}
