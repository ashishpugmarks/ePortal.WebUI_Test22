using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Persistence.SIS.Interface;
using ePortal.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.SIS.Services
{
    public class ISMS : IISMS
    {
        private readonly IConnectionString objCnStr;
        private readonly IDataManagement odmgt;
        public ISMS(IConnectionString conn, IDataManagement _oDataMgmt)
        {
            odmgt = _oDataMgmt;
            objCnStr = conn;
        }

        #region "Get"
        public DataTable GetISMSDetail(string strismsId, string strismsdes, string strismsstatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMS_GET";
            ocmd.Parameters.Add("V_ISMSID", OracleDbType.Varchar2).Value = strismsId;
            ocmd.Parameters.Add("V_ISMS_DES", OracleDbType.Varchar2).Value = strismsdes;
            ocmd.Parameters.Add("V_ISMS_STATUS", OracleDbType.Varchar2).Value = strismsstatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable BindRepeater()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSANNUDET_USER";
            // ocmd.Parameters.Add("ANNOUCEID_IN", OracleDbType.Int32).Value = "0";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetISMSViewDetails(Int32 ID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSANNUCEMENTPOPDET";
            ocmd.Parameters.Add("ANNOUCEID_IN", OracleDbType.Int32).Value = ID;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetISMSDTL(string strismsdtlId, string strismsId, string strismsdtldes, string strismsdtlstatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSDTL_GET";
            ocmd.Parameters.Add("V_ISMSDTLID", OracleDbType.Varchar2).Value = strismsdtlId;
            ocmd.Parameters.Add("V_ISMSID", OracleDbType.Varchar2).Value = strismsId;
            ocmd.Parameters.Add("V_ISMSDTL_DES", OracleDbType.Varchar2).Value = strismsdtldes;
            ocmd.Parameters.Add("V_ISMSDTL_STATUS", OracleDbType.Varchar2).Value = strismsdtlstatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetCategory(String Strflag)
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_CATEGORY_GET";
            objCmd.Parameters.Add("FLAG_IN", OracleDbType.Varchar2).Value = Strflag;
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable GetMILLERDetail(string strmaillerid, string strcat, string stryear, string strmonth, string strdes, string strstatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSMAILLER_GET";
            ocmd.Parameters.Add("MAILLERID_IN", OracleDbType.Int32).Value = strmaillerid;
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("V_ISMS_DES", OracleDbType.Varchar2).Value = strdes;
            ocmd.Parameters.Add("V_ISMS_STATUS", OracleDbType.Int32).Value = strstatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetPolicyCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_BINDITPOLICYCATEGORY";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable GetITProcedureCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_BINDITPROCCATEGORY";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable GetUserPolicyCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_BINDUSERPOLICYCATEGORY";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable GetUserProcedureCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_BINDUSERPROCECATEGORY";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        #endregion

        #region "Update & Insert"
        //public String UpdateISMS(string strTransid, string strDescription, string strStatus, string strDisplay, string StrAttachment, string strAddedby)
        //{
        //    string strErrMsg;
        //    OracleCommand ocmd = new OracleCommand();
        //    ocmd.CommandType = CommandType.StoredProcedure;
        //    ocmd.BindByName = true;
        //    ocmd.CommandText = "PKG_ISMS.SPROC_ISMS_UPDATE";
        //    ocmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strTransid;
        //    ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;
        //    ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
        //    ocmd.Parameters.Add("DISPLAY_IN", OracleDbType.Varchar2).Value = strDisplay;
        //    ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = StrAttachment;
        //    ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
        //    ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        //    ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
        //    odmgt.ExecuteQuery(ocmd);
        //    strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
        //    return strErrMsg;
        //}

        public String UpdateISMS(string strTransid, string strDescription, string strStatus, string strDisplay, string StrAttachment, string strAddedby, string StrCat, string StrYear, string StrMonth)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMS_UPDATE";
            ocmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strTransid;
            ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("DISPLAY_IN", OracleDbType.Varchar2).Value = strDisplay;
            ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = StrAttachment;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;

            ocmd.Parameters.Add("CATE_IN", OracleDbType.Varchar2).Value = StrCat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = StrYear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = StrMonth;

            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public String ISMSDetail_Set(string strDetailid, string strId, string strDescription, string strStatus, string StrAttachment, string strAddedby, string Strcate, string Stryear, string Strmonth)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSDETAIL_SET";
            ocmd.Parameters.Add("DETAILID_IN", OracleDbType.Int32).Value = strDetailid;
            ocmd.Parameters.Add("ISMSID_IN", OracleDbType.Int32).Value = strId;
            ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = StrAttachment;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;

            ocmd.Parameters.Add("CATE_IN", OracleDbType.Varchar2).Value = Strcate;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = Stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = Strmonth;

            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        //public String ISMSDetail_Set(string strDetailid, string strId, string strDescription, string strStatus, string StrAttachment, string strAddedby)
        //{
        //    string strErrMsg;
        //    OracleCommand ocmd = new OracleCommand();
        //    ocmd.CommandType = CommandType.StoredProcedure;
        //    ocmd.BindByName = true;
        //    ocmd.CommandText = "PKG_ISMS.SPROC_ISMSDETAIL_SET";
        //    ocmd.Parameters.Add("DETAILID_IN", OracleDbType.Int32).Value = strDetailid;
        //    ocmd.Parameters.Add("ISMSID_IN", OracleDbType.Int32).Value = strId;
        //    ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;
        //    ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
        //    ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = StrAttachment;
        //    ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;
        //    ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        //    ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
        //    odmgt.ExecuteQuery(ocmd);
        //    strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
        //    return strErrMsg;
        //}
        public DataTable GetDescription_Get(string description)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMS_GETDESCRIPTIONMENU";
            ocmd.Parameters.Add("decription_IN", OracleDbType.Varchar2).Value = description;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetISMSAnnonacement()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSANNOUCEMENT_GET";
            ocmd.Parameters.Add("ANNOUCEID_IN", OracleDbType.Int32).Value = "0";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetDescriptionIDValue(int proc_id_IN)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMS_GETMENUID";
            ocmd.Parameters.Add("proc_id_IN", OracleDbType.Int32).Value = proc_id_IN;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        //public DataTable DocumentType_Get()
        //{
        //    OracleCommand ocmd = new OracleCommand();
        //    ocmd.CommandType = CommandType.StoredProcedure;
        //    ocmd.BindByName = true;
        //    ocmd.CommandText = "PKG_ISMS.SPROC_DOCUMENTTYPE_GET";
        //    ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //    return odmgt.GetDataTable(ocmd);
        //}

        //public DataTable PolicyProcedure_Get(string strId, string strType, string strStatus)
        //{
        //    OracleCommand ocmd = new OracleCommand();
        //    ocmd.CommandType = CommandType.StoredProcedure;
        //    ocmd.BindByName = true;
        //    ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_GET";
        //    ocmd.Parameters.Add("ID_IN", OracleDbType.Varchar2).Value = strId;
        //    ocmd.Parameters.Add("TYPE_IN", OracleDbType.Varchar2).Value = strType;
        //    ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
        //    ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //    return odmgt.GetDataTable(ocmd);
        //}

        //public String PolicyProcedure_Set(string strId, string strDescription, string strCode, string strType, string strStatus, string StrAttachment, string strAddedby)
        //{
        //    string strErrMsg;
        //    OracleCommand ocmd = new OracleCommand();
        //    ocmd.CommandType = CommandType.StoredProcedure;
        //    ocmd.BindByName = true;
        //    ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_SET";
        //    ocmd.Parameters.Add("ID_IN", OracleDbType.Int32).Value = strId;
        //    ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;
        //    ocmd.Parameters.Add("CODE_IN", OracleDbType.Varchar2).Value = strCode;
        //    ocmd.Parameters.Add("TYPE_IN", OracleDbType.Varchar2).Value = strType;
        //    ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
        //    ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = StrAttachment;
        //    ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;
        //    ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        //    ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
        //    odmgt.ExecuteQuery(ocmd);
        //    strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
        //    return strErrMsg;
        //}

        public DataTable DocumentType_Get()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_DOCUMENTTYPE_GET";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable PolicyProcedure_Get(string strId, string strType, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_GET";
            ocmd.Parameters.Add("ID_IN", OracleDbType.Varchar2).Value = strId;
            ocmd.Parameters.Add("TYPE_IN", OracleDbType.Varchar2).Value = strType;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public String PolicyProcedure_Set(string strId, string strDescription, string strCode, string strType, string strStatus, string StrAttachment, string strAddedby, string Strcategory, string Stryear, string Strmonth, string Strday)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_SET";
            ocmd.Parameters.Add("ID_IN", OracleDbType.Int32).Value = strId;
            ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;
            ocmd.Parameters.Add("CODE_IN", OracleDbType.Varchar2).Value = strCode;
            ocmd.Parameters.Add("TYPE_IN", OracleDbType.Varchar2).Value = strType;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = StrAttachment;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;

            ocmd.Parameters.Add("CATE_IN", OracleDbType.Varchar2).Value = Strcategory;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = Stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = Strmonth;
            ocmd.Parameters.Add("DAY_IN", OracleDbType.Varchar2).Value = Strday;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }
        public DataTable SearchCatYearMon_Get(string strId, string strddcategory, string stryear, string strmonth)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_SEARCH_ISMSDETAILS_GET";
            ocmd.Parameters.Add("ID_IN", OracleDbType.Varchar2).Value = strId;
            ocmd.Parameters.Add("CATEGORY_IN", OracleDbType.Varchar2).Value = strddcategory;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public String ISMSANNOUCEMENT_Set(string strAnnouceid, string strDescription, string strDetails, string Strstatus, string strAddedby, string StrISMSId, string StrYear, string StrMonth)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSANNOUCEMENT_SET";
            ocmd.Parameters.Add("ANNOUCEID_IN", OracleDbType.Int32).Value = strAnnouceid;
            ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;
            ocmd.Parameters.Add("DETAILS_IN", OracleDbType.Varchar2).Value = strDetails;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = Strstatus;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("ISMSID_IN", OracleDbType.Int32).Value = StrISMSId;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = StrYear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = StrMonth;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public DataTable GetISMSANNOUCEMENTDetail(string strannouceId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSANNOUCEMENT_GET";
            ocmd.Parameters.Add("ANNOUCEID_IN", OracleDbType.Int32).Value = strannouceId;
            //ocmd.Parameters.Add("ANNOUCE_DES", OracleDbType.Varchar2).Value = strannoucedes;
            //ocmd.Parameters.Add("ANNOUCE_STATUS", OracleDbType.Int32).Value = strannoucestatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetISMSANNOUCEMENTDetail_GET(string strannouceId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSANNOUCEMENTDETAIL";
            ocmd.Parameters.Add("ANNOUCEID_IN", OracleDbType.Int32).Value = strannouceId;
            //ocmd.Parameters.Add("ANNOUCE_DES", OracleDbType.Varchar2).Value = strannoucedes;
            //ocmd.Parameters.Add("ANNOUCE_STATUS", OracleDbType.Int32).Value = strannoucestatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public String ISMSFAQ_Set(string strDetailid, string strismsId, string strDescription, string strcat, string stryear, string strmonth, string strStatus, string strAddedby, string stranswers)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSFAQ_SET";
            ocmd.Parameters.Add("FAQAUTOID_IN", OracleDbType.Int32).Value = strDetailid;
            ocmd.Parameters.Add("ISMSID_IN", OracleDbType.Int32).Value = strismsId;
            ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;

            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;

            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;


            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("ANS_IN", OracleDbType.Varchar2).Value = stranswers;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public DataTable GetISMSFAQ_GET(string strfaqid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSFAQ_GET";
            ocmd.Parameters.Add("FAQAUTOID_IN", OracleDbType.Int32).Value = strfaqid;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetISMSFAQ_GETEDIT(string strfaqid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSFAQ_GETEdit";
            ocmd.Parameters.Add("FAQAUTOID_IN", OracleDbType.Int32).Value = strfaqid;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public string GetISMSMAILLER_SET(string strmaillerId, string strdescription, string strismsstatus, string StrAttachment, string strAddedby, string Strcate, string Stryear, string Strmonth, string Strday)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSMAILLER_SET";
            ocmd.Parameters.Add("MAILLERID_IN", OracleDbType.Int32).Value = strmaillerId;
            ocmd.Parameters.Add("MAILLER_DES_IN", OracleDbType.Varchar2).Value = strdescription;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strismsstatus;
            ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = StrAttachment;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("CATE_IN", OracleDbType.Varchar2).Value = Strcate;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = Stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = Strmonth;
            ocmd.Parameters.Add("DAY_IN", OracleDbType.Varchar2).Value = Strday;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public DataTable GetISMSMailler_GET(string strmaillerId, string strcat, string stryear, string strmonth, string strismsdes, string strismsstatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSMAILLER_GET";
            ocmd.Parameters.Add("MAILLERID_IN", OracleDbType.Int32).Value = strmaillerId;
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("V_ISMS_DES", OracleDbType.Varchar2).Value = strismsdes;
            ocmd.Parameters.Add("V_ISMS_STATUS", OracleDbType.Int32).Value = strismsstatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetISMSMailler_GETSearch(string strmaillerId, string strcat, string stryear, string strmonth, string strismsstatus, string strday)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSMAILLER_Search";
            ocmd.Parameters.Add("MAILLERID_IN", OracleDbType.Int32).Value = strmaillerId;
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("DAY_IN", OracleDbType.Varchar2).Value = strday;
            ocmd.Parameters.Add("V_ISMS_STATUS", OracleDbType.Int32).Value = strismsstatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetISMSMailler_GETID(string strmaillerId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSMAILLER_GETID";
            ocmd.Parameters.Add("MAILLERID_IN", OracleDbType.Int32).Value = strmaillerId;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetISMSANNOUCEMENT_GETSearch(string strmaillerId, string stryear, string strmonth, string strismsstatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSAnnoucement_Search";
            ocmd.Parameters.Add("MAILLERID_IN", OracleDbType.Int32).Value = strmaillerId;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("V_ISMS_STATUS", OracleDbType.Int32).Value = strismsstatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetISMSFAQ_GETSearch(string strmaillerId, string strcat, string stryear, string strmonth, string strismsstatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSFAQ_SEARCH";
            ocmd.Parameters.Add("FAQAUTOID_IN", OracleDbType.Int32).Value = strmaillerId;
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("V_ISMS_STATUS", OracleDbType.Int32).Value = strismsstatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public String CheckList_Set(string strcheckId, string strDescription, string strStatus, string StrAttachment, string strAddedby, string Strcategory, string Stryear, string Strmonth, string Strday)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSCHEKLIST_SET";
            ocmd.Parameters.Add("CHECKLID_IN", OracleDbType.Int32).Value = strcheckId;
            ocmd.Parameters.Add("DES_IN", OracleDbType.Varchar2).Value = strDescription;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = StrAttachment;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;

            ocmd.Parameters.Add("CATE_IN", OracleDbType.Varchar2).Value = Strcategory;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = Stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = Strmonth;
            ocmd.Parameters.Add("DAY_IN", OracleDbType.Varchar2).Value = Strday;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public DataTable CheckList_GETID(string strcheckId)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSCHECKLIST_GETID";
            ocmd.Parameters.Add("CHECKLID_IN", OracleDbType.Int32).Value = strcheckId;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable CheckList_GETDETAILS()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSCHECKLIST_GETDETAILS";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable CheckList_SEARCH(string strchkId, string strcat, string stryear, string strmonth, string strStatus, string Strday)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSCHEKLIST_SEARCH";
            ocmd.Parameters.Add("CHECKLID_IN", OracleDbType.Int32).Value = strchkId;
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("DAY_IN", OracleDbType.Varchar2).Value = Strday;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable PolicyProcedure_GetDETAILS()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_GETDET";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        //public DataTable PolicyProcedure__SEARCH(string strcat, string stryear, string strmonth, string strStatus)
        //{
        //    OracleCommand ocmd = new OracleCommand();
        //    ocmd.CommandType = CommandType.StoredProcedure;
        //    ocmd.BindByName = true;
        //    ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_SEARCH";
        //    ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
        //    ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
        //    ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
        //    ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
        //    ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //    return odmgt.GetDataTable(ocmd);
        //}
        public DataTable PolicyProcedure__SEARCH(string strdoc, string strcat, string stryear, string strmonth, string strStatus, string Strday)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_SEARCH";
            ocmd.Parameters.Add("DOC_IN", OracleDbType.Varchar2).Value = strdoc;
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("DAY_IN", OracleDbType.Varchar2).Value = Strday;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable PolicyProcedure_GetITPOLICYDETAILS()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_ITPOL";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable ITPolicyProcedure__SEARCH(string strcat, string stryear, string strmonth, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ITPOLICY_SEARCH";
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable PolicyProcedure_GetITPROCEDEL()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_ITPROCE";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable ITProcedure__SEARCH(string strcat, string stryear, string strmonth, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ITPROCDE_SEARCH";
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable PolicyProcedure_GetUserPolciyDEL()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_POLICYPROCEDURE_USERPOL";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable UserPolicy__SEARCH(string strcat, string stryear, string strmonth, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_USERPOL_SEARCH";
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable PolicyProcedure_GetUserProcedureDEL()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_USERPROCEDURE";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable UserProcedure__SEARCH(string strcat, string stryear, string strmonth, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_USERPROCEDURE_SEARCH";
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetMonthCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_MailerMonthSearch";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable GetMaileryearSearh()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_MailerYearSearch";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable GetISMSMailler_GETSearchUser(string strmaillerId, string strcat, string stryear, string strmonth, string strismsstatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSMAILLER_SearchUser";
            ocmd.Parameters.Add("MAILLERID_IN", OracleDbType.Int32).Value = strmaillerId;
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("V_ISMS_STATUS", OracleDbType.Int32).Value = strismsstatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable MailerofMonth()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_MAILEROFMONTH";
            // ocmd.Parameters.Add("ANNOUCEID_IN", OracleDbType.Int32).Value = "0";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetISMSFAQUSER()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSFAQ_USERQUESTION";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable CheckList_SEARCHUSER(string strchkId, string strcat, string stryear, string strmonth, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMSCHEKLIST_USERSEARCH";
            ocmd.Parameters.Add("CHECKLID_IN", OracleDbType.Int32).Value = strchkId;
            ocmd.Parameters.Add("CAT_IN", OracleDbType.Varchar2).Value = strcat;
            ocmd.Parameters.Add("YEAR_IN", OracleDbType.Varchar2).Value = stryear;
            ocmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable SearchMonthChecklistCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_ISMSCHEKLIST_MONTHSEARCH";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable UserPolicyMonthCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_USERPOLICYMonthSearch";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable UserProcedureMonthCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_USERPROCEDUREMonthSearch";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable ITPOLICYMonthCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_ITPOLICYMonthSearch";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        public DataTable ITPROCEDUREMonthCategory()
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.CommandText = "PKG_ISMS.SPROC_ITPROCEDUREMonthSearch";
            objCmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            //dt = oDataMgmt.GetDataTable(objCmd);
            //return (dt);
            return odmgt.GetDataTable(objCmd);
        }
        #endregion

        #region for menu and submenu
        public List<ISMSMENUVM> GetMenu(string strismsId, string strismsdes, string strismsstatus)
        {
            List<ISMSMENUVM> objmenu = new List<ISMSMENUVM>();
            DataTable _objdt = new DataTable();
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMS_GETMENULIST";
            //ocmd.Parameters.Add("V_ISMSID", OracleDbType.Varchar2).Value = strismsId;
            //ocmd.Parameters.Add("V_ISMS_DES", OracleDbType.Varchar2).Value = strismsdes;
            //ocmd.Parameters.Add("V_ISMS_STATUS", OracleDbType.Varchar2).Value = strismsstatus;
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            _objdt = odmgt.GetDataTable(ocmd);
            if (_objdt.Rows.Count > 0)
            {
                for (int i = 0; i < _objdt.Rows.Count; i++)
                {
                    objmenu.Add(new ISMSMENUVM
                    {
                        ISMSID = (int)_objdt.Rows[i]["ISMSID"],
                        ISMS_DES = _objdt.Rows[i]["ISMS_DES"].ToString(),
                        FILE_NAME = _objdt.Rows[i]["FILE_NAME"].ToString(),
                        // Active =Convert.ToInt32( _objdt.Rows[i]["ACTIVE"]),
                        ADDED_DATE = Convert.ToDateTime(_objdt.Rows[i]["ADDED_DATE"]),
                        DISPLAY = Convert.ToInt32(_objdt.Rows[i]["DISPLAY"]),
                        //status = _objdt.Rows[i]["status"].ToString(),
                        //DISPLAYstatus = _objdt.Rows[i]["DISPLAYstatus"].ToString(),
                        //LVL = Convert.ToInt32(_objdt.Rows[i]["LVL"]),
                    });
                }
            }
            return objmenu;
        }
        /// 
        /// Get data from SubMenu table
        /// 

        public List<MenuISMSViewModel> GetISMSMenu()
        {
            List<MenuISMSViewModel> objmenu = new List<MenuISMSViewModel>();
            DataTable _objdt = new DataTable();
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_GETISMSMENULIST";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            _objdt = odmgt.GetDataTable(ocmd);
            if (_objdt.Rows.Count > 0)
            {
                for (int i = 0; i < _objdt.Rows.Count; i++)
                {
                    objmenu.Add(new MenuISMSViewModel
                    {
                        MenuId = Convert.ToInt64(_objdt.Rows[i]["MENU_ID"]),
                        MenuParentId = Convert.ToInt64((_objdt.Rows[i]["MENU_PARENT_ID"] is DBNull) ? 0 : _objdt.Rows[i]["MENU_PARENT_ID"]),
                        MenuLevel = Convert.ToInt16(_objdt.Rows[i]["MENU_LEVEL"]),
                        Title = _objdt.Rows[i]["MENU_TEXT"].ToString(),
                        ToolTip = _objdt.Rows[i]["MENU_TOOLTIP"].ToString(),
                        URL = _objdt.Rows[i]["MENU_URL"].ToString(),
                        Action = _objdt.Rows[i]["STATUS"].ToString(),
                        Controller = _objdt.Rows[i]["MENU_CONTROLLER"].ToString(),
                        MenuTarget = _objdt.Rows[i]["MENU_TARGET"].ToString(),
                        MenuContentType = _objdt.Rows[i]["MENUICON_CONTENT_TYPE"].ToString(),
                        isMenuAccessibleToUser = (bool)_objdt.Rows[i]["isMenuAccessibleToUser"],
                    });
                }
            }
            return objmenu;
        }
        public List<SIS_ISMS> GetSisIsmsList()
        {
            List<SIS_ISMS> objmenu = new List<SIS_ISMS>();
            DataTable _objdt = new DataTable();
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMS_GETMENULIST";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            _objdt = odmgt.GetDataTable(ocmd);
            if (_objdt.Rows.Count > 0)
            {
                for (int i = 0; i < _objdt.Rows.Count; i++)
                {
                    objmenu.Add(new SIS_ISMS
                    {
                        ISMSID = Convert.ToInt64(_objdt.Rows[i]["ISMSID"]),
                        MENU_PARENT_ID = Convert.ToInt64((_objdt.Rows[i]["MENU_PARENT_ID"] is DBNull) ? 0 : _objdt.Rows[i]["MENU_PARENT_ID"]),
                        MENU_LEVEL = Convert.ToInt16((_objdt.Rows[i]["MENU_LEVEL"] is DBNull) ? 0 : _objdt.Rows[i]["MENU_LEVEL"]),
                        ISMS_DES = _objdt.Rows[i]["ISMS_DES"].ToString(),
                        MENU_TOOLTIP = _objdt.Rows[i]["MENU_TOOLTIP"].ToString(),
                        MENU_URL = _objdt.Rows[i]["MENU_URL"].ToString(),
                        Active = _objdt.Rows[i]["Active"].ToString(),
                        MENU_CONTROLLER = _objdt.Rows[i]["MENU_CONTROLLER"].ToString(),
                        MENU_TARGET = _objdt.Rows[i]["MENU_TARGET"].ToString(),
                        MENUICON_CONTENT_TYPE = _objdt.Rows[i]["MENUICON_CONTENT_TYPE"].ToString(),
                    });
                }
            }
            return objmenu;
        }
        public List<ISMS_Info_Security> GetInfoSecurityList()
        {
            List<ISMS_Info_Security> objmenu = new List<ISMS_Info_Security>();
            DataTable _objdt = new DataTable();
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_ISMS.SPROC_ISMS_GETSUBMENULIST";
            ocmd.Parameters.Add("CUR_ISMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            _objdt = odmgt.GetDataTable(ocmd);
            if (_objdt.Rows.Count > 0)
            {
                for (int i = 0; i < _objdt.Rows.Count; i++)
                {
                    objmenu.Add(new ISMS_Info_Security
                    {
                        Proc_id = Convert.ToInt64(_objdt.Rows[i]["Proc_id"]),
                        Parent_ID = Convert.ToInt64((_objdt.Rows[i]["Parent_ID"] is DBNull) ? 0 : _objdt.Rows[i]["Parent_ID"]),
                        MENU_LEVEL = Convert.ToInt16((_objdt.Rows[i]["MENU_LEVEL"] is DBNull) ? 0 : _objdt.Rows[i]["MENU_LEVEL"]),
                        decription = _objdt.Rows[i]["decription"].ToString(),
                        MENU_URL = _objdt.Rows[i]["MENU_URL"].ToString(),
                        Active = _objdt.Rows[i]["Active"].ToString(),
                        Filename = _objdt.Rows[i]["Filename"].ToString(),
                        MENU_TARGET = _objdt.Rows[i]["MENU_TARGET"].ToString(),
                        //Menu_Target = _objdt.Rows[i]["Menu_Target"].ToString(),
                        Document_type = _objdt.Rows[i]["DOCUMENT_TYPE"].ToString(),
                    });
                }
            }
            return objmenu;
        }
        #endregion
    }
}
