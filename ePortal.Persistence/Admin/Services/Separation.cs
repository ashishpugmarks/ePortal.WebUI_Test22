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
    public class Separation : ISeparation   
    {
        private readonly IConnectionString objCnStr;
        private readonly IDataManagement odmgt;
        public Separation(IConnectionString conn, IDataManagement _oDataMgmt)
        {
            odmgt = _oDataMgmt;
            objCnStr = conn;
        }
        public DataTable HRDDetail(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_EMPDETAILS_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_HRDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }
        //====================================================================
        //Below the code of Excel To Excel DataTable Event on 02-08-2022
        //====================================================================
        public DataTable ExcelExport(string ECODE, string STATUS)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_EMPSEPARATION.SPROC_CLRFORMHISTORY_GET_Xls";
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
                    objCmd.Parameters.Add("CUR_CLRDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
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
        //====================================================================

        public DataTable HRSPAPPROVAL(string strecode, string strlevel)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_AD_APPROVARCODE";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strecode;
            ocmd.Parameters.Add("LEVEL_IN", OracleDbType.Int32).Value = strlevel;
            ocmd.Parameters.Add("CUR_APPROVARCODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable HRSPResigDetail(string strecode, string strresigid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_RESIGDETAILS_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("CUR_HRDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable HRSPAcceptanceDetail(string strecode, string strresigid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_ACCEPTANCELETTER_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("CUR_HRDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable HRSPApprovalDetail(string strecode, string strresigid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_APPROVALDETAIL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("CUR_APPDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable HRSPpendingReq(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_PENDINGREQUEST_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable HRSPpendingApproval(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_APPPENDINGREQUEST_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable HRpendingList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId, string strResigby)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_HRPENDINGREQUEST_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = plantId;
            ocmd.Parameters.Add("RESIGTYPE_IN", OracleDbType.Varchar2).Value = strResigby;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }
        public DataTable HRpendingCancelList(string strappecode, string strecode, string strEmpname, string strresigid, string plantId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_HRPENDINGREQCANCEL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = plantId;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }
        public DataTable AcceptancependingList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_ACCEPTANCEPENDINGREQ_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = plantId;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable IRAcceptancependingList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_IRACCEPTANCEREQ_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable ClearancependingList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_IRPENDINGCLEARENCE_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = plantId;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string EMPRESIGREQ_Set(string strempcode, string strsubject, string strbody, string strrelievingdate, string strappauth, string strappauthlevel, string straddedby, string latestaddress)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_AD_RESIGREQ_SET";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strempcode;
            ocmd.Parameters.Add("SUBJECT_IN", OracleDbType.Varchar2).Value = strsubject;
            ocmd.Parameters.Add("BODY_IN", OracleDbType.Varchar2).Value = strbody;
            ocmd.Parameters.Add("LATESTADDRESS_IN", OracleDbType.Varchar2).Value = latestaddress;
            ocmd.Parameters.Add("RELIEVING_DATE_IN", OracleDbType.Varchar2).Value = strrelievingdate;
            ocmd.Parameters.Add("APPAUTH_IN", OracleDbType.Int32).Value = strappauth;
            ocmd.Parameters.Add("APPAUTHLEVEL_IN", OracleDbType.Int32).Value = strappauthlevel;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = straddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string UpdateApproval_Set(string strresigid, string strrelievingdate, string strstatus, string strremark, string strappauth, string strappauthlevel, string straddedby)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_RESIGAPPROVAL_SET";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Int32).Value = strresigid;
            ocmd.Parameters.Add("RELIEVING_DATE_IN", OracleDbType.Varchar2).Value = strrelievingdate;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            ocmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
            ocmd.Parameters.Add("APPAUTH_IN", OracleDbType.Int32).Value = strappauth;
            ocmd.Parameters.Add("APPAUTHLEVEL_IN", OracleDbType.Int32).Value = strappauthlevel;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = straddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string UpdateHRApproval_Set(string strresigid, string strremark, string straddedby, string strrelievingdate)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_HRRESIGAPPROVAL_SET";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Int32).Value = strresigid;
            ocmd.Parameters.Add("RELIEVING_DATE_IN", OracleDbType.Varchar2).Value = strrelievingdate;
            ocmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = straddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        //SR89704 Start
        public string UpdateIRApproval_Set(string strresigid, string strremark, string straddedby, string strrelievingdate)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_IRRESIGAPPROVAL_SET";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Int32).Value = strresigid;
            ocmd.Parameters.Add("RELIEVING_DATE_IN", OracleDbType.Varchar2).Value = strrelievingdate;
            ocmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = straddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        //SR89704 End

        public string set_resign_authority(string authid, string resignid, string userid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_ACCEPTANCELETTER_SET";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Int32).Value = Convert.ToInt32(resignid);
            ocmd.Parameters.Add("AUTHECODE_IN", OracleDbType.Varchar2).Value = authid;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = Convert.ToInt32(userid);
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable Get_HeaderMasterDetails(string header, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_HeaderMasterAUTH_GET";
            ocmd.Parameters.Add("HEADERMASTERID_IN", OracleDbType.Varchar2).Value = header;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("CUR_HDMSTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string INSERT_HEADER_MASTER(int HEADERMASTERID_IN, string description, string addedby, StringBuilder xmlServiceList, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_HEADERMASTER_SET";
            ocmd.Parameters.Add("HEADERMASTERID_IN", OracleDbType.Int32).Value = HEADERMASTERID_IN;
            ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = description;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = status;
            ocmd.Parameters.Add("XMLAUTH_ECODE", OracleDbType.Varchar2).Value = Convert.ToString(xmlServiceList);
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string Insert_IRCLEARANCEACTIVATION(string RESIGID, string SUBMITBY, string AMOUNT, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_INSERTIRCLRACTIVATION";
            ocmd.Parameters.Add("RESIGID_IN", OracleDbType.Int32).Value = RESIGID;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = SUBMITBY;
            ocmd.Parameters.Add("AMOUNT_IN", OracleDbType.Varchar2).Value = AMOUNT;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = status;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable MANAGECLEARANCE(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_LISTFULLANDFINAL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = plantId;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_MANAGECLRFORMDETAILS(object ECODE, string STATUS)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_MANAGECLRFORMDETAIL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("CUR_CLRDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_MANAGEDPTCLRFORMDETAILS(object ECODE, string STATUS)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_MANAGEDPTCLRFORMDTL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("CUR_CLRDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_SUBHEADERCLRDETAILS(string ECODE, string RESIGID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_SUBHEADERCLRDETAIL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("RESIGID_IN", OracleDbType.Varchar2).Value = RESIGID;
            ocmd.Parameters.Add("CUR_CLRDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string INSERT_CLRANCESUBHEADERDETAILS(string ecode, string ResigID, string SUBHEADERLIST)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_INSERT_CLRANCESUBHDTAILS";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Int32).Value = ecode;
            ocmd.Parameters.Add("RESIGID_IN", OracleDbType.Varchar2).Value = ResigID;
            ocmd.Parameters.Add("SUBHEADERLIST_IN", OracleDbType.Varchar2).Value = SUBHEADERLIST;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string INSERT_ASSRESIGFORMFILLBYIR(string strecode, string subject, string body, string reldate, string deptreldate, string deptappauth,
        string deptappdate, string divappauth, string divappdate, string opappauth, string opappdate, string submitby, string latestaddress, string ResigType, string HRecode, string Document_IN, string strResignedby, string strISuserAppReq, decimal BasicSalaryAmt)//BasicSalaryAmt Added for SR89704
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_ASSRESFORMFILLBYIR";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strecode;
            ocmd.Parameters.Add("SUBJECT_IN", OracleDbType.Varchar2).Value = subject;
            ocmd.Parameters.Add("BODY_IN", OracleDbType.Varchar2).Value = body;
            ocmd.Parameters.Add("RELDATE_IN", OracleDbType.Varchar2).Value = reldate;
            ocmd.Parameters.Add("DEPTRELDATE_IN", OracleDbType.Varchar2).Value = deptreldate;
            ocmd.Parameters.Add("DEPTAPPAUTH_IN", OracleDbType.Varchar2).Value = deptappauth;
            ocmd.Parameters.Add("DEPTAPPDATE_IN", OracleDbType.Varchar2).Value = deptappdate;
            ocmd.Parameters.Add("DIVAPPAUTH_IN", OracleDbType.Varchar2).Value = divappauth;
            ocmd.Parameters.Add("DIVAPPDATE_IN", OracleDbType.Varchar2).Value = divappdate;
            ocmd.Parameters.Add("OPAPPAUTH_IN", OracleDbType.Varchar2).Value = opappauth;
            ocmd.Parameters.Add("OPAPPDATE_IN", OracleDbType.Varchar2).Value = opappdate;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = submitby;
            ocmd.Parameters.Add("LATESTADDRESS_IN", OracleDbType.Varchar2).Value = latestaddress;
            ocmd.Parameters.Add("RESIGBY_IN", OracleDbType.Varchar2).Value = ResigType;
            ocmd.Parameters.Add("HRAPPCODE_IN", OracleDbType.Varchar2).Value = HRecode;
            ocmd.Parameters.Add("RESIGTYPE_IN", OracleDbType.Varchar2).Value = strResignedby;
            ocmd.Parameters.Add("DOCUMENT_IN", OracleDbType.Varchar2).Value = Document_IN;
            ocmd.Parameters.Add("ISUSERAPPREQ_IN", OracleDbType.Varchar2).Value = strISuserAppReq;
            ocmd.Parameters.Add("BASICSALARYAMT_IN", OracleDbType.Decimal).Value = BasicSalaryAmt;  // Added for SR89704
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable HEADER_MASTER_DEATIL_GET(string header, string status, string headerdesc)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_HeaderMasterDetails_GET";
            ocmd.Parameters.Add("HEADER_IN", OracleDbType.Varchar2).Value = header;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("HEADERDESC_IN", OracleDbType.Varchar2).Value = headerdesc;
            ocmd.Parameters.Add("CUR_HDMSTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable SYPLANT_MASTER_DEATIL_GET()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_SyplantMasterDetails_GET";
            ocmd.Parameters.Add("CUR_HDMSTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_HEADER_DETAILS()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_HeaderDetails_GET";
            ocmd.Parameters.Add("CUR_HDMSTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable SUB_HEADER_MASTER_DEATIL_GET(string SUBHEADERID_IN, string status, string HEADERID_IN, string SUBHEADER_IN)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_SubHDMasterDetails_GET";
            ocmd.Parameters.Add("SUBHEADERID_IN", OracleDbType.Varchar2).Value = SUBHEADERID_IN;
            ocmd.Parameters.Add("SUBHEADER_IN", OracleDbType.Varchar2).Value = SUBHEADER_IN;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("HEADERID_IN", OracleDbType.Varchar2).Value = HEADERID_IN;
            ocmd.Parameters.Add("CUR_HDMSTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_SubHeaderauthDetails(string subheader, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_Sub_HeaderMasterAUTH_GET";
            ocmd.Parameters.Add("SUBHEADERMASTERID_IN", OracleDbType.Varchar2).Value = subheader;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("CUR_HDMSTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string INSERT_SUB_HEADER_MASTER(int SUBHEADERMASTERID_IN, string header, string description, string addedby, StringBuilder xmlServiceList, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_SUBHEADERMASTER_SET";
            ocmd.Parameters.Add("SUBHEADERMASTERID_IN", OracleDbType.Int32).Value = SUBHEADERMASTERID_IN;
            ocmd.Parameters.Add("HEADER_IN", OracleDbType.Int32).Value = header;
            ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = description;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = status;
            ocmd.Parameters.Add("XMLAUTH_ECODE", OracleDbType.Varchar2).Value = Convert.ToString(xmlServiceList);
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataSet GET_EXITINTERVIEW_QUESTIONNAIRE(string STATUSSEC_IN, string STATUSQUS_IN, string STATUSOPT_IN)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_INTERVIEWQUESTIONAIRE";
            ocmd.Parameters.Add("STATUSSEC_IN", OracleDbType.Int32).Value = STATUSSEC_IN;
            ocmd.Parameters.Add("STATUSQUS_IN", OracleDbType.Int32).Value = STATUSQUS_IN;
            ocmd.Parameters.Add("STATUSOPT_IN", OracleDbType.Int32).Value = STATUSOPT_IN;
            ocmd.Parameters.Add("CUR_INTQUESLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_INTOPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string INSERT_INTERVIEW_ANSWER(string RESIGNATIONID, string SUBMITBY, string INTERVIEWXML)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_INSERTINTERVIEWANSWER";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = RESIGNATIONID;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = SUBMITBY;
            ocmd.Parameters.Add("INTERVIEWXML_IN", OracleDbType.Varchar2).Value = INTERVIEWXML;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataSet Get_GET_INTERVIEWANSWER(string RESIGNATIONID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_INTERVIEWANSWER_GET";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Int32).Value = Convert.ToInt32(RESIGNATIONID);
            ocmd.Parameters.Add("CUR_INTQUESLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_INTOPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_Exitinterviewquestionaire(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_EXITINTERVIEWDETAIL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_INTERVIEWDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_HeaderauthDetails(string header, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_HeaderMasterAUTH_GET";
            ocmd.Parameters.Add("HEADERMASTERID_IN", OracleDbType.Varchar2).Value = header;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("CUR_HDMSTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string CANCELLATION_RESIGNATION(string RESIGNID, string remarks)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_CANCEL_RESIGNATION";
            ocmd.Parameters.Add("RESIGID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = remarks;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable LISTPENDINGINTERVIEWQUEST(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_LISTPENDINGEXITINT_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = plantId;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string Final_Hr_Submit_Resignation(string ResigID, string hrremarks, string SUBMITBY)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_FINALSUBMITHRRESIG";
            ocmd.Parameters.Add("RESIGID_IN", OracleDbType.Varchar2).Value = ResigID;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = hrremarks;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = SUBMITBY;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string INSERT_DEPT_CLEARANCE(string CONFIDENTIAL_DOC, string OTHER_DOC, string LIBRARY_BOOK, string ITASSETS, string CAMERA, string KEY,
        string TRAVEL_BILL, string SUBMITBY, string ECODE, string ResigID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_INSERTDEPTCLEAR";
            ocmd.Parameters.Add("CONFDOC_IN", OracleDbType.Varchar2).Value = CONFIDENTIAL_DOC;
            ocmd.Parameters.Add("OTHERDOC_IN", OracleDbType.Varchar2).Value = OTHER_DOC;
            ocmd.Parameters.Add("LIBBOOK_IN", OracleDbType.Varchar2).Value = LIBRARY_BOOK;
            ocmd.Parameters.Add("ITASSETS_IN", OracleDbType.Varchar2).Value = ITASSETS;
            ocmd.Parameters.Add("CAMERA_IN", OracleDbType.Varchar2).Value = CAMERA;
            ocmd.Parameters.Add("KEY_IN", OracleDbType.Varchar2).Value = KEY;
            ocmd.Parameters.Add("TRAVELBILL_IN", OracleDbType.Varchar2).Value = TRAVEL_BILL;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = SUBMITBY;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("ResigID_IN", OracleDbType.Varchar2).Value = ResigID;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataSet FULLRESIGNDETAIL_GET(string RESIGNID, string ECODE, string CLEARENCEHEADERID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_FULLRESIGNDETAIL_GET";
            ocmd.Parameters.Add("ResigID_IN", OracleDbType.Varchar2).Value = Convert.ToString(RESIGNID);
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("CLEARENCEHEADERID_IN", OracleDbType.Varchar2).Value = CLEARENCEHEADERID;
            ocmd.Parameters.Add("CUR_HEADERDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_SUBHEADERDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string UPDATESUBHEADERSTATUS(string clrdetailid, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_UPDATESUBHEADERSTATUS";
            ocmd.Parameters.Add("CLRDETAILID_IN", OracleDbType.Varchar2).Value = clrdetailid;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable GET_MANAGECLEARANCEHEADERDETAILS(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_CLRHEADERDETAILS_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = Convert.ToString(strecode);
            ocmd.Parameters.Add("CUR_HEADERDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string UPDATE_SUBHEADERREMARKS(string REMARKS, string detailid, string HEADERSTATUS)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_UPDATE_SUBHEADREM";
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = REMARKS;
            ocmd.Parameters.Add("DETAILID_IN", OracleDbType.Varchar2).Value = detailid;
            ocmd.Parameters.Add("HEADERSTATUS_IN", OracleDbType.Varchar2).Value = HEADERSTATUS;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string CompletedResignation(string RESIGNID, string remarks)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_COMPLETEPROCESS";
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = remarks;
            ocmd.Parameters.Add("RESIGNID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable ExitintEmpDetail(string strresigin)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_EXITINTEMPDETAILS_GET";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigin;
            ocmd.Parameters.Add("CUR_HRDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_Resignation_data(string RESIGNID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_EDITRESIGNATION_GET";
            ocmd.Parameters.Add("RESIGNID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("CUR_RESIGNDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string UPDATE_RESIGNATION_DETAILS(string RESIGNID, string SUBJECT, string BODY, string releDate, string latestaddress)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_UPDATERESIGN_DETAILS";
            ocmd.Parameters.Add("RESIGNID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("SUBJECT_IN", OracleDbType.Varchar2).Value = SUBJECT;
            ocmd.Parameters.Add("BODY_IN", OracleDbType.Varchar2).Value = BODY;
            ocmd.Parameters.Add("RELDATE_IN", OracleDbType.Varchar2).Value = releDate;
            ocmd.Parameters.Add("LATESTADDRESS_IN", OracleDbType.Varchar2).Value = latestaddress;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string INSERT_INTERVIEW_ANSWER(string RESIGNATIONID, string SUBMITBY, string INTERVIEWXML, string qualification)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_INSERTINTERVIEWANSWER";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = RESIGNATIONID;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = SUBMITBY;
            ocmd.Parameters.Add("INTERVIEWXML_IN", OracleDbType.Varchar2).Value = INTERVIEWXML;
            ocmd.Parameters.Add("QUALIFICATION_IN", OracleDbType.Varchar2).Value = qualification;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string HR_MODIFYDEPARTMENTDATE(string deptdate, string RESIGNID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_MODIFYDEPARTMENTDATE_HR";
            ocmd.Parameters.Add("RESIGNID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("DEPTDATE_IN", OracleDbType.Varchar2).Value = deptdate;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        public string HR_CancelRequest(string cancelremark, string RESIGNID, string userid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_UPDATECANCELREQ_HR";
            ocmd.Parameters.Add("RESIGNID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("CANCEL_IN", OracleDbType.Varchar2).Value = cancelremark;
            ocmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = userid;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        public DataSet GET_EXITINTERVIEW_QUESTIONNAIRE(string STATUSSEC_IN, string STATUSQUS_IN, string STATUSOPT_IN, string section)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_INTERVIEWQUESTIONAIRE";
            ocmd.Parameters.Add("STATUSSEC_IN", OracleDbType.Int32).Value = STATUSSEC_IN;
            ocmd.Parameters.Add("STATUSQUS_IN", OracleDbType.Int32).Value = STATUSQUS_IN;
            ocmd.Parameters.Add("STATUSOPT_IN", OracleDbType.Int32).Value = STATUSOPT_IN;
            ocmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = section;
            ocmd.Parameters.Add("CUR_INTQUESLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_INTOPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string ADDMAILBYHR(string empcode, string emailid, string emailtype, string status, string plant, string submitby, string PROGRAMTYPE, string MAILTRANSACTIONID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_ADDMAILBYHR";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
            ocmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = emailid;
            ocmd.Parameters.Add("EMAILTYPE_IN", OracleDbType.Varchar2).Value = emailtype;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = plant;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = submitby;
            ocmd.Parameters.Add("PROGRAMTYPE_IN", OracleDbType.Varchar2).Value = PROGRAMTYPE;
            ocmd.Parameters.Add("MAILTRANSACTIONID_IN", OracleDbType.Varchar2).Value = MAILTRANSACTIONID;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable GET_MANAGEMAILBYHR(string TRANSID, string ECODE, string ENAME, string EMAILID, string STATUS, string EMAILTYPE, string PLANT)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_MANAGEMAILBYHR_GET";
            ocmd.Parameters.Add("TRANSID_IN", OracleDbType.Varchar2).Value = TRANSID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            ocmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("EMAILTYPE_IN", OracleDbType.Varchar2).Value = EMAILTYPE;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = PLANT;
            ocmd.Parameters.Add("CUR_MANAGEMAILBYHR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_DEPRTMENTAPPEMAILIDS(string RESIGNID, string GETEMAILTYPE, string strcode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_DEPRTMENTAPPEMAILIDS_GET";
            ocmd.Parameters.Add("RESIGNID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("GETEMAILTYPE_IN", OracleDbType.Varchar2).Value = GETEMAILTYPE;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strcode;
            ocmd.Parameters.Add("CUR_GETEMAILIDS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_Resignationprocess(string strempcode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_RESIGNAPPPROCESS_GET";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strempcode;
            ocmd.Parameters.Add("CUR_RESIGPROCESS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_HEADERCLEARANCE(string strempcode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_CLEARANCEHEADER_GET";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strempcode;
            ocmd.Parameters.Add("CUR_CLRHEADER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_RESIGPROCESSFLOW(string strempcode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_RESIGPROCESSFLOW_GET";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strempcode;
            ocmd.Parameters.Add("CUR_CLRHEADER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string UPDATE_DATERANGEVALIDATION(string MONTHID, string DATERANGEFROM, string DATERANGETO, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_DATERANGE_UPDATE";
            ocmd.Parameters.Add("MONTHID_IN", OracleDbType.Varchar2).Value = MONTHID;
            ocmd.Parameters.Add("DATERANGEFROM_IN", OracleDbType.Varchar2).Value = DATERANGEFROM;
            ocmd.Parameters.Add("DATERANGETO_IN", OracleDbType.Varchar2).Value = DATERANGETO;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable Get_DateRangeValidation(string MONTHID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_DATERANGEVALIDATION_GET";
            ocmd.Parameters.Add("MONTHID_IN", OracleDbType.Varchar2).Value = MONTHID;
            ocmd.Parameters.Add("CUR_DATERANGEVALID", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Pendingsendmailreq(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_PENDINGSENDMAIL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = plantId;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string Acceptancelistsendmail(string ResignID, string addedby)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_CLEARANCEMAILSEND_SET";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = ResignID;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable Get_HRMailIdResignation(string REGID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.sproc_hrmailid_get";
            ocmd.Parameters.Add("resignationid_in", OracleDbType.Varchar2).Value = REGID;
            ocmd.Parameters.Add("cur_hrmail", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();

        }

        public DataTable FILLAPPROVALDETAILS(string ecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_APPROVALFORMDETAIL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ecode;
            ocmd.Parameters.Add("CUR_APPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_GETCLRHEADERCOMPLETED(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_COMPLETEDCLRHEADER_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = Convert.ToString(strecode);
            ocmd.Parameters.Add("CUR_HEADERDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable EXPORT_HEAD_DETAILS()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_EXPORTHEADDETAIL_GET";
            ocmd.Parameters.Add("CUR_HDMSTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable EXPORT_SUBHEAD_DETAILS()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_EXPORTSUBHEADDETAIL_GET";
            ocmd.Parameters.Add("CUR_HDMSTDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable IRPendingsendmailreq(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_PENDINGSENDMAILIR_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = plantId;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_RESIGNOPHEADHRSENDMAIL(string ECODE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_RESGREQHRMAILID_GET";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("CUR_HRMAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_ResgApprovalHistory(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_APPREQUESTHISTORY_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_ResgCLRFORMHistory(object ECODE, string STATUS)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_CLRFORMHISTORY_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("CUR_CLRDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_ResgCLRHEADEHistory(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_CLRHEADHISTORY_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = Convert.ToString(strecode);
            ocmd.Parameters.Add("CUR_HEADERDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_rESGDPTCLRFORMHistory(object ECODE, string STATUS)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_DPTCLRFORMHISTORY_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("CUR_CLRDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_DeptclrviewHistoryDetails(string ECODE, string RESIGNID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_DEPTCLRVIEWHISTORY_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("RESIGNID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("CUR_CLRDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string MANAGEWITHDRAWALBYHR(string RESIGNID, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_MNGWITHDRAWBYHR_UPDATE";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable GET_MANAGEWITHDRAWDATA(string USERID, string ECODE, string RESIGNID, string PLANTID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_MANAGEWITHDRAW_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = USERID;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = RESIGNID;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = PLANTID;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable GET_MANAGEMAILBYIR(string TRANSID, string ECODE, string ENAME, string EMAILID, string STATUS, string EMAILTYPE, string PLANT)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_MANAGEMAILBYIR_GET";
            ocmd.Parameters.Add("TRANSID_IN", OracleDbType.Varchar2).Value = TRANSID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            ocmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("EMAILTYPE_IN", OracleDbType.Varchar2).Value = EMAILTYPE;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = PLANT;
            ocmd.Parameters.Add("CUR_MANAGEMAILBYIR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string ADDMAILBYIR(string empcode, string emailid, string emailtype, string status, string plant, string submitby, string PROGRAMTYPE, string MAILTRANSACTIONID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_ADDMAILBYIR";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
            ocmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = emailid;
            ocmd.Parameters.Add("EMAILTYPE_IN", OracleDbType.Varchar2).Value = emailtype;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = plant;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = submitby;
            ocmd.Parameters.Add("PROGRAMTYPE_IN", OracleDbType.Varchar2).Value = PROGRAMTYPE;
            ocmd.Parameters.Add("MAILTRANSACTIONID_IN", OracleDbType.Varchar2).Value = MAILTRANSACTIONID;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable GET_REGCLRMAILID(string strRegId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_RESGREQCLRMAILID_GET";
            ocmd.Parameters.Add("RESIGNID_IN", OracleDbType.Int32).Value = strRegId;
            ocmd.Parameters.Add("CUR_MAILID", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_ResignationReport(string ECODE, string ENAME, string OPERATION, string DIVISION, string STATUS, string FROMRELDATE, string TORELDATE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_RESIGNATIONREPORT_GET";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            ocmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("FROMRELDATE_IN", OracleDbType.Varchar2).Value = FROMRELDATE;
            ocmd.Parameters.Add("TORELDATE_IN", OracleDbType.Varchar2).Value = TORELDATE;
            ocmd.Parameters.Add("CUR_REPORT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string Get_SHOWCLRHEADREQUEST(string ADEMPCODE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_SHOWCLRHEADREQUEST";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ADEMPCODE;
            ocmd.Parameters.Add("SHOWAUTH_OUT", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["SHOWAUTH_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string Get_SHOWCLRSUBHEADREQUEST(string ADEMPCODE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_SHOWCLRREQUEST";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = ADEMPCODE;
            ocmd.Parameters.Add("SHOWAUTH_OUT", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["SHOWAUTH_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataSet APPROVALAUTHDETAIL(string RESIGNID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_APPROVALAUTHDETAIL_GET";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Int32).Value = RESIGNID;
            ocmd.Parameters.Add("CUR_AUTHDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_RESIGNDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }
        public DataTable HRFornIDetail(string strecode, string strresigid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_FORMI_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("CUR_HRDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }
        public DataTable GetFormIDetail(string strresigid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_FORMIDETAILS_GET";
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("CUR_HRDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }
        public string INSERT_FORMI(string strformi, string strresigid, string strbasicamt, string strgratuityamt, string strstatus,
            string SUBMITBY, string stremailid, string strrelievingdate)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_INSERTFORMI";
            ocmd.Parameters.Add("ADEMP_RESIG_FORMLID_IN", OracleDbType.Varchar2).Value = strformi;
            ocmd.Parameters.Add("RESIGID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("BASICSALARYAMT_IN", OracleDbType.Varchar2).Value = strbasicamt;
            ocmd.Parameters.Add("GRATUITYAMT_IN", OracleDbType.Varchar2).Value = strgratuityamt;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = SUBMITBY;
            ocmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = stremailid;
            ocmd.Parameters.Add("RELIEVING_DATE_IN", OracleDbType.Varchar2).Value = strrelievingdate;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        public DataTable MANAGEGratuityList(string strappecode, string strecode, string strEmpname, string strStatus, string strresigid, string plantId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_LISTGRATUITY_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strappecode;
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESIGNATIONID_IN", OracleDbType.Varchar2).Value = strresigid;
            ocmd.Parameters.Add("PLANTID", OracleDbType.Varchar2).Value = plantId;
            ocmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public string UPDATE_FORML(string strformi, string SUBMITBY, string FILENAME)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_UPDATEFORML";
            ocmd.Parameters.Add("ADEMP_RESIG_FORMLID_IN", OracleDbType.Varchar2).Value = strformi;
            ocmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = SUBMITBY;
            ocmd.Parameters.Add("FORMLFILE_IN", OracleDbType.Varchar2).Value = FILENAME;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        public string UPDATE_FORMI(string RelievingDate, string BasicAmt, string GratuityAmt, string RegId, string UpdatedBy)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_UPDATEFORMI";
            ocmd.Parameters.Add("RESIGID_IN", OracleDbType.Varchar2).Value = RegId;
            ocmd.Parameters.Add("RELIEVINGDATE_IN", OracleDbType.Varchar2).Value = RelievingDate;
            ocmd.Parameters.Add("BASICSALARYAMT_IN", OracleDbType.Varchar2).Value = BasicAmt;
            ocmd.Parameters.Add("GRATUITYAMT_IN", OracleDbType.Varchar2).Value = GratuityAmt;
            ocmd.Parameters.Add("UPDATEDBY_IN", OracleDbType.Varchar2).Value = UpdatedBy;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataTable STAFFAPPROVAL(string strecode, string strlevel)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_AD_APPCODE_STAFFBELOW";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strecode;
            ocmd.Parameters.Add("LEVEL_IN", OracleDbType.Int32).Value = strlevel;
            ocmd.Parameters.Add("CUR_APPROVARCODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        //Separation change
        public DataTable GET_SYPARAMETERS_PARAMVALUE(string paramName)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.GET_SYPARAMETERS_PARAMVALUE";
            ocmd.Parameters.Add("PARAMNAME_IN", OracleDbType.Varchar2).Value = paramName;
            ocmd.Parameters.Add("CUR_RESIGN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }

        public DataTable Get_IRMailIdResignation(string REGID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.sproc_irmailid_get";
            ocmd.Parameters.Add("resignationid_in", OracleDbType.Varchar2).Value = REGID;
            ocmd.Parameters.Add("cur_irmail", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }
        //Separation change

        //Window ID manual deletion - start
        public DataTable GET_Windowid_Deletion(string code, string paramName, string Requester)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_Windowdeletion_entry";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = code;
            ocmd.Parameters.Add("PARAM_VALUE", OracleDbType.Varchar2).Value = paramName;
            ocmd.Parameters.Add("REQUESTER", OracleDbType.Varchar2).Value = Requester;
            ocmd.Parameters.Add("CUR_SAPIDDELETION_DATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Dispose();
            return odmgt.GetDataTable(ocmd);
        }
        //Window ID manual deletion - end
        //SR89704 Start
        public DataTable EmpDesgValidation(string strecode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_EMPDESGVALIDATION_IR";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strecode;
            ocmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }
        //SR89704 End
        //Start : SR107848
        public DataTable GET_EXPORTCLRFORMDETAIL(object ECODE, string STATUS)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_EMPSEPARATION.SPROC_EXPORTCLRFORMDETAIL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("CUR_CLRDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
            ocmd.Dispose();
        }
        //End : SR107848
    }
}
