using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.ViewModels.APPX.Finance;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
namespace ePortal.Persistence.Admin.Services
{
    public class TaxDeclaration: ITaxDeclaration
    {
        private readonly IDataManagement odmgt;
        private readonly IConnectionString objConn;
        private readonly ICommonFunctions _objCommon;

        OracleCommand oCmd;
        DataSet ds;
        DataTable Dt;
        //OracleConnection objConn;
        string strConn;
        string errMsg = string.Empty;
        String strMsg = string.Empty;

        public TaxDeclaration(IDataManagement _oDataMgmt, IConnectionString _objConn, ICommonFunctions objCommon)
        {
            odmgt = _oDataMgmt;
            objConn = _objConn;
            _objCommon = objCommon;
        }

        public DataTable GetEmpUsrPrdDtl(string strEmpcode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_EXPORTEMPPRDDETAIL_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);

        }
        public DataSet GETEXPORT_EMPINVDETAIL(string strStartdate, string strEnddate, string strKi)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETEXPORT_EMPINVDETAIL";
            ocmd.Parameters.Add("V_STARTDATE", OracleDbType.Varchar2).Value = strStartdate;
            ocmd.Parameters.Add("V_ENDDATE", OracleDbType.Varchar2).Value = strEnddate;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strKi;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_SAPHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_EMPCODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_EMPRENTDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);

        }
        public String UpdateEmpTaxDecOpendate(string strEmpcode, string strStartdate, string strEnddate, string strAddedby, string strKi, string strIseditable, string strIseditableactual, string str_ltacount)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_SETEMPTAXDCLOPENDATE";
            ocmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKi;
            ocmd.Parameters.Add("V_STARTDATE", OracleDbType.Varchar2).Value = strStartdate;
            ocmd.Parameters.Add("V_ENDDATE", OracleDbType.Varchar2).Value = strEnddate;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("V_ISEDITABLE", OracleDbType.Int32).Value = strIseditable;
            ocmd.Parameters.Add("V_ISEDITABLEACTUAL", OracleDbType.Int32).Value = strIseditableactual;
            ocmd.Parameters.Add("V_LTACOUNT", OracleDbType.Int32).Value = str_ltacount;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            strMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strMsg;
        }
        public String UpdateTaxhead(string strTaxheadid, string strTaxhead, string strTaxabbr, string strDesc, string strMaxlimit, string strIsmandatory, string strXmlopt, string strActive, string strAddedby, string strPropsapdesc, string strActsapdesc, string strSapinfotype, string strSeqno, string strSapSubinfotype)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPDATETAXHAED";
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = strTaxheadid;
            ocmd.Parameters.Add("V_TAXHEADNAME", OracleDbType.Varchar2).Value = strTaxhead;
            ocmd.Parameters.Add("V_TAXHEADABBR", OracleDbType.Varchar2).Value = strTaxabbr;
            ocmd.Parameters.Add("V_SAPINFOTYPE", OracleDbType.Int32).Value = strSapinfotype;
            ocmd.Parameters.Add("V_SAPSUBINFOTYPE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strSapSubinfotype) ? (object)DBNull.Value : strSapSubinfotype; ;
            ocmd.Parameters.Add("V_PROPSAPFLDNAME", OracleDbType.Varchar2).Value = strPropsapdesc;
            ocmd.Parameters.Add("V_ACTUALSAPFLDNAME", OracleDbType.Varchar2).Value = strActsapdesc;
            ocmd.Parameters.Add("V_DESCRIPTION", OracleDbType.Varchar2).Value = strDesc;
            ocmd.Parameters.Add("V_MAXLIMIT", OracleDbType.Int32).Value = string.IsNullOrEmpty(strMaxlimit) ? (object)DBNull.Value : strMaxlimit;
            ocmd.Parameters.Add("V_ISMANDATORY", OracleDbType.Int32).Value = strIsmandatory;
            ocmd.Parameters.Add("V_ACTIVE", OracleDbType.Int32).Value = strActive;
            ocmd.Parameters.Add("V_OPTIONXML", OracleDbType.Varchar2).Value = strXmlopt;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("V_SEQNO", OracleDbType.Int32).Value = (string.IsNullOrEmpty(strSeqno) ? (object)DBNull.Value : strSeqno);
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("NEW_TAXHEADID_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("TRANSACTION_TYPE_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strTaxHeadID = ocmd.Parameters["NEW_TAXHEADID_OUT"].Value.ToString();
            string strTransactionType = ocmd.Parameters["TRANSACTION_TYPE_OUT"].Value.ToString();
            UpdateTaxheadLog(strTaxHeadID, strTaxhead, strTaxabbr, strDesc, strMaxlimit, strIsmandatory, strXmlopt, strActive, strAddedby, strPropsapdesc, strActsapdesc, strSapinfotype, strSeqno, strSapSubinfotype
                , strTransactionType);
            strMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);

            return strMsg;
        }
        public void UpdateTaxheadLog(string strTaxheadid, string strTaxhead, string strTaxabbr, string strDesc, string strMaxlimit, string strIsmandatory, string strXmlopt, string strActive, string strAddedby, string strPropsapdesc, string strActsapdesc, string strSapinfotype, string strSeqno, string strSapSubinfotype, string strTransactionType)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "FINTAXHEAD_LOG";
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = strTaxheadid;
            ocmd.Parameters.Add("V_TAXHEADNAME", OracleDbType.Varchar2).Value = strTaxhead;
            ocmd.Parameters.Add("V_TAXHEADABBR", OracleDbType.Varchar2).Value = strTaxabbr;
            ocmd.Parameters.Add("V_SAPINFOTYPE", OracleDbType.Int32).Value = strSapinfotype;
            ocmd.Parameters.Add("V_SAPSUBINFOTYPE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strSapSubinfotype) ? (object)DBNull.Value : strSapSubinfotype; ;
            ocmd.Parameters.Add("V_PROPSAPFLDNAME", OracleDbType.Varchar2).Value = strPropsapdesc;
            ocmd.Parameters.Add("V_ACTUALSAPFLDNAME", OracleDbType.Varchar2).Value = strActsapdesc;
            ocmd.Parameters.Add("V_DESCRIPTION", OracleDbType.Varchar2).Value = strDesc;
            ocmd.Parameters.Add("V_MAXLIMIT", OracleDbType.Int32).Value = string.IsNullOrEmpty(strMaxlimit) ? (object)DBNull.Value : strMaxlimit;
            ocmd.Parameters.Add("V_ISMANDATORY", OracleDbType.Int32).Value = strIsmandatory;
            ocmd.Parameters.Add("V_ACTIVE", OracleDbType.Int32).Value = strActive;
            ocmd.Parameters.Add("V_OPTIONXML", OracleDbType.Varchar2).Value = strXmlopt;
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("V_TRANSACTIONTYPE", OracleDbType.Varchar2, 500).Value = strTransactionType;
            odmgt.ExecuteQuery(ocmd);
        }

        public DataTable GetTaxheaddetail(string strTaxheadid, string strTaxhead, string strActive, string strSapinfotype, string strKI)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETTAXHEADDETAIL";
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strTaxheadid) ? (object)DBNull.Value : strTaxheadid;
            ocmd.Parameters.Add("V_TAXHEAD", OracleDbType.Varchar2).Value = strTaxhead;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = string.IsNullOrEmpty(strActive) ? (object)DBNull.Value : strActive;
            ocmd.Parameters.Add("V_SAPINFOTYPE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(strSapinfotype) ? (object)DBNull.Value : strSapinfotype;
            ocmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strKI) ? (object)DBNull.Value : strKI;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);

        }
        public DataTable GetTaxhead_Optdetail(string strTaxheadoptid, string strTaxheadid, string strActive)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETTAXHEADOPT_DETAIL";
            ocmd.Parameters.Add("V_TAXHEADOPTID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strTaxheadoptid) ? (object)DBNull.Value : strTaxheadoptid;
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strTaxheadid) ? (object)DBNull.Value : strTaxheadid;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = string.IsNullOrEmpty(strActive) ? (object)DBNull.Value : strActive;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);

        }
        public DataTable GetEmpTax_Invdetail(string strTaxheadid, string strEmpcode, string strActive, string strKi, string strSapinfotype)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETEMPTAXINVDETAIL";
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strTaxheadid) ? (object)DBNull.Value : strTaxheadid;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = string.IsNullOrEmpty(strActive) ? (object)DBNull.Value : strActive;
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = string.IsNullOrEmpty(strKi) ? (object)DBNull.Value : strKi;
            ocmd.Parameters.Add("V_SAPINFOTYPE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(strSapinfotype) ? (object)DBNull.Value : strSapinfotype;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetEmpRent_Invdetail(string strEmpcode, string strKi)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETRENT_DETAIL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = string.IsNullOrEmpty(strKi) ? (object)DBNull.Value : strKi;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetEmp584_Invdetail(string strTaxheadid, string strEmpcode, string strActive, string strKi, string strSapinfotype)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETEMP584INVDETAIL";
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strTaxheadid) ? (object)DBNull.Value : strTaxheadid;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = string.IsNullOrEmpty(strActive) ? (object)DBNull.Value : strActive;
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = string.IsNullOrEmpty(strKi) ? (object)DBNull.Value : strKi;
            ocmd.Parameters.Add("V_SAPINFOTYPE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(strSapinfotype ) ? (object)DBNull.Value : strSapinfotype;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetEmpUserPeriodDetail(string strEmpcode, string strActive, string strKi)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETEMPUSERPERIODDETAIL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = string.IsNullOrEmpty(strKi) ? (object)DBNull.Value : strKi;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        //public void UpdateEmpInvDetail(string strEmpcode, string strKi, string strRentfrom, string strRentto, string strAcctype, string strCitycat, string strIsexemp, string strhraclaim, string strRentamt, string strTaddress, string strTmobile, List<Rent_Detail> objRent, string[,] strInv584detail, string[,] strInvdetail, string strActive, string strIsdraft, ArrayList arr_586, string grd586, ArrayList arr_585, string grd585, ReceiptDtl_LTAMain objlta, ReceiptDtl_12BMain obj12BMain, LoanDetail objloan, Emp80EE_Detail objEmp80EE, ArrayList HLLIST, PLoanDetail objploan, ArrayList objltcList, ArrayList objltcmList, string UserId)
        //{
        //    String strErrMsg = String.Empty;

        //    string strCn = objConn.getConnectingString(); ;
        //    OracleCommand objCmd;
        //    OracleTransaction tran;
        //    using (OracleConnection objCn = new OracleConnection())
        //    {
        //        objCn.ConnectionString = strCn;
        //        objCn.Open();
        //        tran = objCn.BeginTransaction();

        //        try
        //        {
        //            objCmd = new OracleCommand();
        //            objCmd.Connection = objCn;
        //            objCmd.Transaction = tran;
        //            int loopcount = 0;
        //            foreach (var rent in objRent)
        //            {
        //                objCmd.Parameters.Clear();
        //                //string strSql = "PKG_FINTAXDECLARATION.SPROC_UPDATEEMPRENTDEATAIL";
        //                string strSql = "PKG_FINTAXDECLARATION.SPROC_UPDATEEMPRENTDEATAIL_N";
        //                objCmd.Parameters.Add("V_FINEMPRENTID", OracleDbType.Int32).Value = rent._strFINEMPRENTDTLID;// == "" ? 0 : rent._strFINEMPRENTDTLID;
        //                objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int64).Value = strEmpcode;
        //                objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int64).Value = strKi;

        //                objCmd.Parameters.Add("V_RENTFROM", OracleDbType.Varchar2).Value = rent._Periodfromdate;
        //                objCmd.Parameters.Add("V_RENTTO", OracleDbType.Varchar2).Value = rent._PeriodTodate;
        //                objCmd.Parameters.Add("V_ACCTYPE", OracleDbType.Int32).Value = strAcctype;
        //                objCmd.Parameters.Add("V_CITYCAT", OracleDbType.Int32).Value = rent._CityCat;//
        //                objCmd.Parameters.Add("V_ISTAXEXEMPTED", OracleDbType.Varchar2).Value = strIsexemp;
        //                objCmd.Parameters.Add("V_HRACLAIM", OracleDbType.Varchar2).Value = strhraclaim;
        //                objCmd.Parameters.Add("V_ADDRESS", OracleDbType.Varchar2).Value = rent._ResAddress;//
        //                objCmd.Parameters.Add("V_MOBILE", OracleDbType.Varchar2).Value = strTmobile;
        //                objCmd.Parameters.Add("V_RENTAMT", OracleDbType.Decimal).Value = rent._MonthlyRentAmt;//
        //                objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;

        //                objCmd.Parameters.Add("V_EMPSO", OracleDbType.Varchar2).Value = rent._EmpSO;
        //                objCmd.Parameters.Add("V_LANDLORDNAME", OracleDbType.Varchar2).Value = rent._LandlordName;
        //                objCmd.Parameters.Add("V_LANDLORDSO", OracleDbType.Varchar2).Value = rent._LandlordSO;
        //                objCmd.Parameters.Add("V_LANDLORDRESHNO", OracleDbType.Varchar2).Value = rent._LandlordResHno;
        //                objCmd.Parameters.Add("V_LANDLORDRESGNO", OracleDbType.Varchar2).Value = rent._LandlordResGno;
        //                objCmd.Parameters.Add("V_LANDLORDRESVILLAGE", OracleDbType.Varchar2).Value = rent._LandlordResVillage;
        //                objCmd.Parameters.Add("V_LANDLORDRESCITY", OracleDbType.Varchar2).Value = rent._LandlordResCity;
        //                objCmd.Parameters.Add("V_LANDLORDRESPINCODE", OracleDbType.Varchar2).Value = rent._LandlordResPincode;
        //                objCmd.Parameters.Add("V_LANDLORDPAN", OracleDbType.Varchar2).Value = rent._LandlordPAN;
        //                objCmd.Parameters.Add("V_ISLANPAN", OracleDbType.Varchar2).Value = rent._ISPANCARD == "" ? (object)DBNull.Value : rent._ISPANCARD;
        //                objCmd.Parameters.Add("V_LANDLORDMNO", OracleDbType.Varchar2).Value = rent._Landlordmno;

        //                objCmd.Parameters.Add("V_LANDLORDNAME2", OracleDbType.Varchar2).Value = rent._LandlordName2;
        //                objCmd.Parameters.Add("V_LANDLORDSO2", OracleDbType.Varchar2).Value = rent._LandlordSO2;
        //                objCmd.Parameters.Add("V_LANDLORDRESHNO2", OracleDbType.Varchar2).Value = rent._LandlordResHno2;
        //                objCmd.Parameters.Add("V_LANDLORDRESGNO2", OracleDbType.Varchar2).Value = rent._LandlordResGno2;
        //                objCmd.Parameters.Add("V_LANDLORDRESVILLAGE2", OracleDbType.Varchar2).Value = rent._LandlordResVillage2;
        //                objCmd.Parameters.Add("V_LANDLORDRESCITY2", OracleDbType.Varchar2).Value = rent._LandlordResCity2;
        //                objCmd.Parameters.Add("V_LANDLORDRESPINCODE2", OracleDbType.Varchar2).Value = rent._LandlordResPincode2;
        //                objCmd.Parameters.Add("V_LANDLORDPAN2", OracleDbType.Varchar2).Value = rent._LandlordPAN2;
        //                objCmd.Parameters.Add("V_LANDLORDMNO2", OracleDbType.Varchar2).Value = rent._Landlordmno2;

        //                objCmd.Parameters.Add("V_LANDLORDNAME3", OracleDbType.Varchar2).Value = rent._LandlordName3;
        //                objCmd.Parameters.Add("V_LANDLORDSO3", OracleDbType.Varchar2).Value = rent._LandlordSO3;
        //                objCmd.Parameters.Add("V_LANDLORDRESHNO3", OracleDbType.Varchar2).Value = rent._LandlordResHno3;
        //                objCmd.Parameters.Add("V_LANDLORDRESGNO3", OracleDbType.Varchar2).Value = rent._LandlordResGno3;
        //                objCmd.Parameters.Add("V_LANDLORDRESVILLAGE3", OracleDbType.Varchar2).Value = rent._LandlordResVillage3;
        //                objCmd.Parameters.Add("V_LANDLORDRESCITY3", OracleDbType.Varchar2).Value = rent._LandlordResCity3;
        //                objCmd.Parameters.Add("V_LANDLORDRESPINCODE3", OracleDbType.Varchar2).Value = rent._LandlordResPincode3;
        //                objCmd.Parameters.Add("V_LANDLORDPAN3", OracleDbType.Varchar2).Value = rent._LandlordPAN3;
        //                objCmd.Parameters.Add("V_LANDLORDMNO3", OracleDbType.Varchar2).Value = rent._Landlordmno3;

        //                objCmd.Parameters.Add("V_LANDLORDNAME4", OracleDbType.Varchar2).Value = rent._LandlordName4;
        //                objCmd.Parameters.Add("V_LANDLORDSO4", OracleDbType.Varchar2).Value = rent._LandlordSO4;
        //                objCmd.Parameters.Add("V_LANDLORDRESHNO4", OracleDbType.Varchar2).Value = rent._LandlordResHno4;
        //                objCmd.Parameters.Add("V_LANDLORDRESGNO4", OracleDbType.Varchar2).Value = rent._LandlordResGno4;
        //                objCmd.Parameters.Add("V_LANDLORDRESVILLAGE4", OracleDbType.Varchar2).Value = rent._LandlordResVillage4;
        //                objCmd.Parameters.Add("V_LANDLORDRESCITY4", OracleDbType.Varchar2).Value = rent._LandlordResCity4;
        //                objCmd.Parameters.Add("V_LANDLORDRESPINCODE4", OracleDbType.Varchar2).Value = rent._LandlordResPincode4;
        //                objCmd.Parameters.Add("V_LANDLORDPAN4", OracleDbType.Varchar2).Value = rent._LandlordPAN4;
        //                objCmd.Parameters.Add("V_LANDLORDMNO4", OracleDbType.Varchar2).Value = rent._Landlordmno4;

        //                objCmd.Parameters.Add("V_LOOPCOUNT", OracleDbType.Int32).Value = loopcount;
        //                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        //                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
        //                objCmd.CommandText = strSql;
        //                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                objCmd.BindByName = true;
        //                objCmd.ExecuteNonQuery();
        //                int subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
        //                string strResult = objCmd.Parameters["ERROR_MSG"].Value.ToString();
        //                if (subResult != 1)
        //                {
        //                    tran.Rollback();
        //                    throw new Exception("Failed to update HRA details. Transaction will rollback now.");
        //                }
        //                loopcount++;
        //            }


        //            for (int i = 0; i < strInv584detail.GetLength(0); i++)
        //            {
        //                objCmd.Parameters.Clear();
        //                objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPDATEEMP584_INVDEC";
        //                objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
        //                objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKi;
        //                objCmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = strInv584detail[i, 0];
        //                objCmd.Parameters.Add("V_TAXHEADOPTID", OracleDbType.Int32).Value = strInv584detail[i, 1] == "" ? (object)DBNull.Value : strInv584detail[i, 1];
        //                objCmd.Parameters.Add("V_ACTIVE", OracleDbType.Int32).Value = strActive;
        //                objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;
        //                objCmd.Parameters.Add("V_PROJ_AMOUNT", OracleDbType.Int32).Value = strInv584detail[i, 2] == "" ? (object)DBNull.Value : strInv584detail[i, 2];
        //                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        //                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
        //                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                objCmd.BindByName = true;
        //                objCmd.ExecuteNonQuery();
        //                int subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
        //                if (subResult != 1)
        //                {
        //                    tran.Rollback();
        //                    throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
        //                }
        //            }
        //            for (int i = 0; i < strInvdetail.GetLength(0); i++)
        //            {
        //                objCmd.Parameters.Clear();
        //                objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPDATEEMPTAX_INVDEC";
        //                objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
        //                objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKi;
        //                objCmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = strInvdetail[i, 0];
        //                objCmd.Parameters.Add("V_TAXHEADOPTID", OracleDbType.Int32).Value = strInvdetail[i, 1] == "" ? (object)DBNull.Value : strInvdetail[i, 1];
        //                objCmd.Parameters.Add("V_ACTIVE", OracleDbType.Int32).Value = strActive;
        //                objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;
        //                objCmd.Parameters.Add("V_PROJ_AMOUNT", OracleDbType.Int32).Value = strInvdetail[i, 2] == "" ? (object)DBNull.Value : strInvdetail[i, 2];
        //                objCmd.Parameters.Add("V_ACT_AMOUNT", OracleDbType.Int32).Value = strInvdetail[i, 3] == "" ? (object)DBNull.Value : strInvdetail[i, 3];
        //                objCmd.Parameters.Add("V_SAVEASDRAFT", OracleDbType.Int32).Value = strIsdraft == "" ? (object)DBNull.Value : strIsdraft;
        //                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        //                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
        //                //objCmd.CommandText = strSql;
        //                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                objCmd.BindByName = true;
        //                objCmd.ExecuteNonQuery();

        //                int subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
        //                if (subResult != 1)
        //                {
        //                    tran.Rollback();
        //                    throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
        //                }
        //            }

        //            if (arr_586 != null)
        //                Update586Receiptdtl(arr_586, strKi, strEmpcode, objCmd, grd586, UserId);
        //            if (arr_585 != null)
        //                Update586Receiptdtl(arr_585, strKi, strEmpcode, objCmd, grd585, UserId);
        //            if (objlta != null)
        //                UpdateReceipt_LTADetail(objlta, strKi, strEmpcode, objCmd, UserId);
        //            if (obj12BMain != null)
        //                Update_12BDetail(obj12BMain, strKi, strEmpcode, objCmd, UserId);
        //            if (objloan != null)
        //                Update_LoanDetail(objloan, strKi, strEmpcode, objCmd);
        //            if (HLLIST != null)
        //                UpdateHLListDetail(HLLIST, objCmd, strEmpcode);
        //            if (objEmp80EE != null)
        //                Update_80EEDetail(objEmp80EE, strKi, strEmpcode, objCmd);
        //            if (objploan != null)
        //                Update_LoanDetail_PROJ(objploan, strKi, strEmpcode, objCmd);
        //            if (objltcList != null)
        //                Update_LTCDetail(objltcList, strKi, strEmpcode, objCmd);
        //            if (objltcmList != null)
        //                Update_LTCFMDetail(objltcmList, strKi, strEmpcode, objCmd);
        //            tran.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            try
        //            {
        //                tran.Rollback();
        //            }
        //            catch (OracleException x)
        //            {
        //                if (tran.Connection != null)
        //                {
        //                    throw new Exception("An exception of type " + x.GetType() + " was encountered while attempting to roll back the transaction.");
        //                }
        //            }
        //            throw new Exception(ex.ToString());
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
        ////Update 586 Receipt Detail
        public void Update586Receiptdtl(List<ReceiptIndexViewModel> arr586, string strKIID, string strEmpcode, OracleCommand objCmd, string hdn_taxheadid, string UserId)
        {
            for (int i = 0; i < arr586.Count; i++)
            {
                 var obj = arr586[i].Receipts;
                    objCmd.Parameters.Clear();
                    string strTaxhead = arr586[i].TaxId;
                    //if (obj.Length > 0)
                    //{
                    objCmd.CommandText = "UPDATE FIN_TAX588_586RPDTL T SET T.ACTIVE=0,T.DATELSTMOD=SYSDATE,T.MODIFIEDBY= " + UserId + " WHERE T.ADEMPCODE=" + strEmpcode + " AND T.SYKIID=" + strKIID + " AND TAXHEADID=" + strTaxhead;
                    objCmd.CommandType = CommandType.Text;
                    objCmd.ExecuteNonQuery();
                    objCmd.CommandText = "UPDATE FIN_TAX588_586RPDTL_V1 T SET T.ACTIVE=0,T.DATELSTMOD=SYSDATE,T.MODIFIEDBY= " + UserId + " WHERE T.ADEMPCODE=" + strEmpcode + " AND T.SYKIID=" + strKIID + " AND TAXHEADID=" + strTaxhead;
                    objCmd.CommandType = CommandType.Text;
                    objCmd.ExecuteNonQuery();
                    //}

                    foreach (ReceiptDtl_586 o in obj)
                    {
                        if (!string.IsNullOrEmpty(o._PolicyNo))
                        {
                            objCmd.Parameters.Clear();
                            objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT585_586_RECDTL_V1";
                            objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(o._ID)  ? "0" : o._ID;
                            objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
                            objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                            objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
                            objCmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = o._TaxHeadID;
                            objCmd.Parameters.Add("V_POLICYNO", OracleDbType.Varchar2).Value = o._PolicyNo;
                            objCmd.Parameters.Add("V_SUMASSURED", OracleDbType.Varchar2).Value = o._SumAssured;
                            objCmd.Parameters.Add("V_RECEIPT_NO", OracleDbType.Varchar2).Value = o._ReceiptNo;
                            objCmd.Parameters.Add("V_POLICY_DATE", OracleDbType.Varchar2).Value = o._Date;
                            objCmd.Parameters.Add("V_RECEIPT_AMOUNT", OracleDbType.Int32).Value = o.Amount;
                            objCmd.Parameters.Add("V_PREMIUM_DATE", OracleDbType.Varchar2).Value = o._PrmDate;
                            objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;
                            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                            objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                            //objCmd.CommandText = strSql;
                            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                            objCmd.BindByName = true;
                            objCmd.ExecuteNonQuery();
                            Int32 subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                            if (subResult != 1)
                            {
                                throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
                            }
                        }
                        else
                        {
                            objCmd.Parameters.Clear();
                            objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT585_586_RECDTL";
                            objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(o._ID) ? "0" : o._ID;
                            objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
                            objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                            objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
                            objCmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = o._TaxHeadID;
                            objCmd.Parameters.Add("V_RECEIPT_NO", OracleDbType.Varchar2).Value = o._ReceiptNo;
                            objCmd.Parameters.Add("V_RECEIPT_AMOUNT", OracleDbType.Int32).Value = o.Amount;
                            objCmd.Parameters.Add("V_RECEIPT_DATE", OracleDbType.Varchar2).Value = o._PrmDate;
                            objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;
                            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                            objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                            //objCmd.CommandText = strSql;
                            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                            objCmd.BindByName = true;
                            objCmd.ExecuteNonQuery();
                            Int32 subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                            if (subResult != 1)
                            {
                                throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
                            }
                        }
                    }
                }
            }
        

        //Update LTA Exemption Detail
        public void UpdateReceipt_LTADetail(ReceiptDtl_LTAMain obj, string strKIID, string strEmpcode, OracleCommand objCmd, string UserId)
        {

            objCmd.Parameters.Clear();
            objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT_LTARECDTL";
            objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty( obj.ID)? "0" : obj.ID;
            objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
            objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
            objCmd.Parameters.Add("V_LTATAKEN", OracleDbType.Int32).Value = obj.LTAAmount;
            objCmd.Parameters.Add("V_PLACEOFVISIT", OracleDbType.Varchar2).Value = obj.Placeofvisit;
            objCmd.Parameters.Add("V_JOURNEYFROM_DT", OracleDbType.Varchar2).Value = obj.JFrom;
            objCmd.Parameters.Add("V_JOURNEYTO_DT", OracleDbType.Varchar2).Value = obj.JTo;
            objCmd.Parameters.Add("V_TRIPNO", OracleDbType.Varchar2).Value = obj.TripNo;
            objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;
            objCmd.Parameters.Add("V_NORMALTRAINFARE", OracleDbType.Int32).Value = string.IsNullOrEmpty(obj.NormalTrainFare) ? "0" : obj.NormalTrainFare;// Added on 16 dec 2017
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
            //objCmd.CommandText = strSql;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.ExecuteNonQuery();
            Int32 subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            string strFin_taxltadtlid = objCmd.Parameters["V_ID"].Value.ToString();
            if (subResult != 1)
            {
                throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
            }


            if (obj.objLTAdtlcolls != null || obj.objLTAdtlcolls.Count > 0)
            {
                foreach (ReceiptDtl_LTACollection o in obj.objLTAdtlcolls)
                {
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT_LTARECJOURNEYDTL";
                    objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(o.ID) ? "0" : o.ID;
                    objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
                    objCmd.Parameters.Add("V_FIN_TAXLTADTLID", OracleDbType.Int32).Value = strFin_taxltadtlid;
                    objCmd.Parameters.Add("V_NAMEOFPERSON", OracleDbType.Varchar2).Value = o.Name;
                    objCmd.Parameters.Add("V_RELATION", OracleDbType.Varchar2).Value = o.Relation;
                    objCmd.Parameters.Add("V_AGE", OracleDbType.Varchar2).Value = o.Age;
                    objCmd.Parameters.Add("V_MOT", OracleDbType.Varchar2).Value = obj.MOTHeader;
                    objCmd.Parameters.Add("V_BILLNO", OracleDbType.Varchar2).Value = o.TktBillNo;
                    objCmd.Parameters.Add("V_AMOUNT", OracleDbType.Int32).Value = string.IsNullOrEmpty(o.Amount) ? (object)DBNull.Value : o.Amount;
                    objCmd.Parameters.Add("V_TRAINFARE", OracleDbType.Int32).Value = string.IsNullOrEmpty(o.TrainFare) ? (object)DBNull.Value : o.TrainFare;
                    objCmd.Parameters.Add("V_ISTRAVEL", OracleDbType.Int32).Value = string.IsNullOrEmpty(o.IsTravel) ? (object)DBNull.Value : o.IsTravel;
                    objCmd.Parameters.Add("V_ELEGIBLE", OracleDbType.Int32).Value = string.IsNullOrEmpty(o.ElilableAmt) ? (object)DBNull.Value : o.ElilableAmt;
                    objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strEmpcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                    //objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (subResult != 1)
                    {
                        throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
                    }
                }
            }

        }
        //Update 12 B Exemption Detail
        public void Update_12BDetail(EmployerB12ViewModel obj, string strKIID, string strEmpcode, OracleCommand objCmd, string UserId)
        {

            if (obj.B12List != null && obj.B12List?.Count > 0)
            {
                objCmd.Parameters.Clear();
                objCmd.CommandText = "UPDATE FIN_TAX12BDTL T SET T.ACTIVE=0,T.DATELSTMOD=SYSDATE,T.MODIFIEDBY= " + strEmpcode + " WHERE T.ADEMPCODE=" + strEmpcode + " AND T.SYKIID=" + strKIID;
                objCmd.CommandType = CommandType.Text;
                objCmd.ExecuteNonQuery();
                foreach (EmployerB12ListDetail o in obj.B12List)
                {
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT_12BRECDTL";
                    objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(o.ID)? "0" : o.ID;
                    objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
                    objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                    objCmd.Parameters.Add("V_EMPADDRESS", OracleDbType.Varchar2).Value = o.EmployerAddress;
                    objCmd.Parameters.Add("V_RES_STATUS", OracleDbType.Varchar2).Value = o.ResStatus;
                    objCmd.Parameters.Add("V_EMPLOYER_NAME", OracleDbType.Varchar2).Value = o.EmployerName;
                    objCmd.Parameters.Add("V_EMPLOYER_ADD", OracleDbType.Varchar2).Value = o.EmployerAddress;
                    objCmd.Parameters.Add("V_TAN", OracleDbType.Varchar2).Value = o.EmployerTan;
                    objCmd.Parameters.Add("V_PAN", OracleDbType.Varchar2).Value = o.EmployerPan;
                    objCmd.Parameters.Add("V_PERIOD_FROM", OracleDbType.Varchar2).Value = o.FromDate;
                    objCmd.Parameters.Add("V_PERIOD_TO", OracleDbType.Varchar2).Value = o.ToDate;
                    objCmd.Parameters.Add("V_TOTAL_GROSS", OracleDbType.Int32).Value = o.TotalGross;
                    objCmd.Parameters.Add("V_TOTAL_PERK", OracleDbType.Int32).Value = o.Perk;
                    objCmd.Parameters.Add("V_TOTAL_EXEMP", OracleDbType.Int32).Value = o.TotalExempt;
                    objCmd.Parameters.Add("V_TOTAL_DED", OracleDbType.Int32).Value = o.Deductions;
                    objCmd.Parameters.Add("V_TOTALTAXAMT", OracleDbType.Int32).Value = o.TotalAmtTax;
                    objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    Int32 subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (subResult != 1)
                    {
                        throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
                    }
                }
            }
        }
        //Update Loan  Detail
        public void Update_LoanDetail(LoanApplicationViewModel obj, string strKIID, string strEmpcode, OracleCommand objCmd)
        {

            objCmd.Parameters.Clear();
            objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT_LOANDTL";
            objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(obj.ID) ? "0" : obj.ID;
            objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
            objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
            objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            objCmd.Parameters.Add("V_ISUC", OracleDbType.Varchar2).Value = obj.IsUnderCons;
            objCmd.Parameters.Add("V_ISOWNER", OracleDbType.Varchar2).Value = obj.PropertyOwner;
            objCmd.Parameters.Add("V_DATEOFCOMPLETION", OracleDbType.Varchar2).Value = obj.ConsDate;
            objCmd.Parameters.Add("V_DATELOANTKN", OracleDbType.Varchar2).Value = obj.LoanTaken;
            objCmd.Parameters.Add("V_CURRYEAR", OracleDbType.Varchar2).Value = obj.CurrentYear;
            objCmd.Parameters.Add("V_CURRINTREST", OracleDbType.Varchar2).Value = obj.CurrInterest;
            objCmd.Parameters.Add("V_YEAR1", OracleDbType.Varchar2).Value = obj.Year1;
            objCmd.Parameters.Add("V_YEAR1INTEREST", OracleDbType.Varchar2).Value = obj.PreYear1;
            objCmd.Parameters.Add("V_YEAR2", OracleDbType.Varchar2).Value = obj.Year2;
            objCmd.Parameters.Add("V_YEAR2INTEREST", OracleDbType.Varchar2).Value = obj.PreYear2;
            objCmd.Parameters.Add("V_YEAR3", OracleDbType.Varchar2).Value = obj.Year3;
            objCmd.Parameters.Add("V_YEAR3INTEREST", OracleDbType.Varchar2).Value = obj.PreYear3;
            objCmd.Parameters.Add("V_YEAR4", OracleDbType.Varchar2).Value = obj.Year4;
            objCmd.Parameters.Add("V_YEAR4INTEREST", OracleDbType.Varchar2).Value = obj.PreYear4;
            objCmd.Parameters.Add("V_YEAR5", OracleDbType.Varchar2).Value = obj.Year5;
            objCmd.Parameters.Add("V_YEAR5INTEREST", OracleDbType.Varchar2).Value = obj.PreYear5;
            objCmd.Parameters.Add("V_ISJOINTLOAN", OracleDbType.Varchar2).Value = obj.IsJointLoan;
            objCmd.Parameters.Add("V_RELATION", OracleDbType.Varchar2).Value = obj.RelationshipId;
            objCmd.Parameters.Add("V_SHAREPER", OracleDbType.Varchar2).Value = obj.ShareInProperty;
            objCmd.Parameters.Add("V_TAXEXEMPSHAREPER", OracleDbType.Varchar2).Value = obj.ShareInTaxExemption;
            objCmd.Parameters.Add("V_SELFOCCUPFROM", OracleDbType.Varchar2).Value = obj.SelfOccupancyFrom;
            objCmd.Parameters.Add("V_SELFOCCUPTO", OracleDbType.Varchar2).Value = obj.SelfOccupancyTo;
            objCmd.Parameters.Add("V_ADDRESSOFHLT", OracleDbType.Varchar2).Value = obj.HouseAddress;
            //NEW pARAM aDDED
            objCmd.Parameters.Add("V_LOANAMOUNT", OracleDbType.Varchar2).Value = DBNull.Value;
            objCmd.Parameters.Add("V_LOADPROVIDER", OracleDbType.Varchar2).Value = DBNull.Value;
            objCmd.Parameters.Add("V_ANNUALRENT", OracleDbType.Varchar2).Value = obj.AnnualRent;
            objCmd.Parameters.Add("V_MUNICIPALTAX", OracleDbType.Varchar2).Value = obj.MunicipalTax;
            objCmd.Parameters.Add("V_LOADPROVIDERADRESS", OracleDbType.Varchar2).Value = DBNull.Value;
            objCmd.Parameters.Add("V_LOADPROVIDERPAN", OracleDbType.Varchar2).Value = DBNull.Value;
            //---------------
            objCmd.Parameters.Add("V_OTHHOUSEOWNER", OracleDbType.Varchar2).Value = obj.PropertyOwner;
            objCmd.Parameters.Add("V_VALUEOFHOUSE", OracleDbType.Varchar2).Value = obj.ValueOfHouse;

            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.ExecuteNonQuery();
            Int32 subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            if (subResult != 1)
            {
                throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
            }
            objCmd.Parameters.Clear();
            objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT_HOUSELOANDTL";
            objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
            objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.ExecuteNonQuery();
            Int32 subResultNEW = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            if (subResultNEW != 1)
            {
                throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
            }

        }
        //Update Employee 80EE  Detail
        public void Update_80EEDetail(EEDetailsModels obj, string strKIID, string strEmpcode, OracleCommand objCmd)
        {

            objCmd.Parameters.Clear();
            objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT_80EEDTL";
            objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(obj.ID) ? "0" : obj.ID;
            objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
            objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
            objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            objCmd.Parameters.Add("V_TAXHEADID", OracleDbType.Varchar2).Value = obj.TaxId;
            objCmd.Parameters.Add("V_LSDATTE", OracleDbType.Varchar2).Value = obj.Loansanctiondate;
            objCmd.Parameters.Add("V_LSAMOUNT", OracleDbType.Varchar2).Value = obj.Loansanctionamount;
            objCmd.Parameters.Add("V_TVRHP", OracleDbType.Varchar2).Value = obj.Vofhouse;
            objCmd.Parameters.Add("V_ISEMPOOTHHOUSE", OracleDbType.Varchar2).Value = obj.Ownerofoterhhouse;
            objCmd.Parameters.Add("V_OWNEROFPROPERTY", OracleDbType.Varchar2).Value = obj.Ownerofproperty;
            objCmd.Parameters.Add("V_ISPUC", OracleDbType.Varchar2).Value = obj.Popunderconstr;
            objCmd.Parameters.Add("V_DATEOFCOMP", OracleDbType.Varchar2).Value = obj.Compofconstdate;
            objCmd.Parameters.Add("V_IAC24B", OracleDbType.Varchar2).Value = obj.Amtclaimed24b;
            objCmd.Parameters.Add("V_PSP", OracleDbType.Varchar2).Value = obj.Perofshareproperty;
            objCmd.Parameters.Add("V_PSTE", OracleDbType.Varchar2).Value = obj.Perofshareintax;
            objCmd.Parameters.Add("V_DECLAREAMOUNT", OracleDbType.Varchar2).Value = obj.Declamount;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.ExecuteNonQuery();
            Int32 subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            if (subResult != 1)
            {
                throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
            }


        }

        //Update House Loan Detail
        public void UpdateHLListDetail(LoanApplicationViewModel HLLIST, OracleCommand objCmd, string strEmpcode)
        {
            for (int i = 0; i < HLLIST.LoanProviders.Count; i++)
            {
                LoanProviderModel obj = (LoanProviderModel)HLLIST.LoanProviders[i];
                if (obj.LoanAmount != "")
                {
                    string val = obj.LoanAmount;

                    //foreach (HouseRent_Detail o in obj)
                    //{
                    //if (!string.IsNullOrEmpty(o._LoanAmount))
                    //{
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_HOUSE_LOANDTL";
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Varchar2).Value = strEmpcode;
                    objCmd.Parameters.Add("V_LOANAMOUNT", OracleDbType.Varchar2).Value = obj.LoanAmount;
                    objCmd.Parameters.Add("V_LOADPROVIDER", OracleDbType.Varchar2).Value = obj.LoanProvider;
                    objCmd.Parameters.Add("V_LOADPROVIDERADRESS", OracleDbType.Varchar2).Value = obj.AddressLoanProvider;
                    objCmd.Parameters.Add("V_LOADPROVIDERPAN", OracleDbType.Varchar2).Value = obj.PanLoanProvider;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    Int32 subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (subResult != 1)
                    {
                        throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
                    }

                    //  }

                    //}
                }
            }
            //for (int i = 0; i < HLLIST.Count; i++)
            //{
            //    HouseRent_Detail obj = (HouseRent_Detail)HLLIST[i];
            //    if (obj.LoanAmount != "")
            //    {
            //        string val = obj.LoanAmount;

            //        //foreach (HouseRent_Detail o in obj)
            //        //{
            //        //if (!string.IsNullOrEmpty(o._LoanAmount))
            //        //{
            //        objCmd.Parameters.Clear();
            //        objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_HOUSE_LOANDTL";
            //        objCmd.Parameters.Add("V_LOANAMOUNT", OracleDbType.Varchar2).Value = obj.LoanAmount;
            //        objCmd.Parameters.Add("V_LOADPROVIDER", OracleDbType.Varchar2).Value = obj.LoanProvider;
            //        objCmd.Parameters.Add("V_LOADPROVIDERADRESS", OracleDbType.Varchar2).Value = obj.addressLoanProvider;
            //        objCmd.Parameters.Add("V_LOADPROVIDERPAN", OracleDbType.Varchar2).Value = obj.panLoanProvider;
            //        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            //        objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
            //        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            //        objCmd.BindByName = true;
            //        objCmd.ExecuteNonQuery();
            //        Int32 subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            //        if (subResult != 1)
            //        {
            //            throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
            //        }

            //        //  }

            //        //}
            //    }
            //}
        }


        //-----------Alok-----------------
        public DataTable Gettaxstatus(string strEmpcode, string strstatus, string strActstatus, string strki)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_TAXSTATUS_GET";
            ocmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = string.IsNullOrEmpty(strstatus) ? (object)DBNull.Value : strstatus;
            ocmd.Parameters.Add("ACTSTATUS_IN", OracleDbType.Int32).Value = string.IsNullOrEmpty(strActstatus) ? (object)DBNull.Value : strActstatus;
            ocmd.Parameters.Add("KI_IN", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);

        }
        public DataTable Get586_585ReceiptDetail(string strEmpcode, string strTaxheadid, string strki, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GET585_586RECEIPTDTL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strTaxheadid) ? (object)DBNull.Value : strTaxheadid;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);

        }
        public DataTable Get586_585ReceiptDetail_V1(string strEmpcode, string strTaxheadid, string strki, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GET585_586RECEIPTDTL_V1";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strTaxheadid) ? (object)DBNull.Value : strTaxheadid;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);

        }
        public DataSet GetLTADtl(string strEmpcode, string strki, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETLTADTL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_LTADTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_LTAJDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
        }
        public DataTable Get12BDtl(string strEmpcode, string strki, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GET12BDTL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_12BDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetFINANCIALYEAR()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETFINANCIALYEAR";
            ocmd.Parameters.Add("CUR_FINANCIALYEAR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetLOANDTL(string strEmpcode, string strki)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETLOANDTL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("CUR_LOANDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable Get80EEDTL(string strEmpcode, string strki)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GET80EEDTL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("CUR_80EEDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable Get80EEADTL(string strEmpcode, string strki)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GET80EEADTL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("CUR_80EEADTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetPreviousYearDtl(string strEmpcode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETPREVIOUSHISTORY";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        #region  "Print Report"
        public DataTable Get585_586RecDtl(string strEmpcode, string strki, string strStatus, string strSapinfotype, string strTaxheadid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_RPT585_586RECEIPTDTL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strTaxheadid) ? (object)DBNull.Value : strTaxheadid;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = string.IsNullOrEmpty(strStatus) ? (object)DBNull.Value : strStatus;
            ocmd.Parameters.Add("V_SAPINFOTYPE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(strSapinfotype) ? (object)DBNull.Value : strSapinfotype;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        // New Print Report
        public DataTable Get585_586RecDtl_V1(string strEmpcode, string strki, string strStatus, string strSapinfotype, string strTaxheadid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_RPT585_586RECEIPTDTL_V1";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strTaxheadid) ? (object)DBNull.Value : strTaxheadid;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = string.IsNullOrEmpty(strStatus) ? (object)DBNull.Value : strStatus;
            ocmd.Parameters.Add("V_SAPINFOTYPE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(strSapinfotype) ? (object)DBNull.Value : strSapinfotype;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        #endregion

        public String CopyTaxHead(string strAddedby)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_COPYPRETAXHEADDETAIL";
            ocmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            strMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strMsg;
        }
        public DataTable GetCurrKIStatus(string strEmpcode)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETCURRKISTATUS";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = strEmpcode;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetTrainFareEligPerAge(string gender, string age, string normaltrainfare)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_TRAIN_FARE_ELIG_AMOUNT";
            ocmd.Parameters.Add("GENDER_IN", OracleDbType.Varchar2).Value = gender;
            ocmd.Parameters.Add("AGE_IN", OracleDbType.Int32).Value = age;
            ocmd.Parameters.Add("FARE_IN", OracleDbType.Int64).Value = normaltrainfare;
            ocmd.Parameters.Add("CUR_TRAINFARE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GetEmpRent_Invdetail_N(string strEmpcode, string strKi)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETRENT_DETAIL_N";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = string.IsNullOrEmpty(strKi) ? (object)DBNull.Value : strKi;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetHOUSELOANDTL(string FIN_TAXLOANDTLID)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETHOUSELOANDTL";
            ocmd.Parameters.Add("FIN_TAXLOANDTLID_IN", OracleDbType.Int32).Value = string.IsNullOrEmpty(FIN_TAXLOANDTLID) ? (object)DBNull.Value : FIN_TAXLOANDTLID;
            ocmd.Parameters.Add("CUR_LOANDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataSet GET_HOUSE_LOAN_DETAIL(string strStartdate, string strEnddate, string strKi)
        {

            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GET_HOUSE_LOAN_ETAIL";
            ocmd.Parameters.Add("V_STARTDATE", OracleDbType.Varchar2).Value = strStartdate;
            ocmd.Parameters.Add("V_ENDDATE", OracleDbType.Varchar2).Value = strEnddate;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strKi;
            ocmd.Parameters.Add("CUR_EMPRENTDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
        }
        // Add below EportalDal/Admin/TaxDeclaration.cs 

        

        public DataTable GETHRA_DETAIL(string strstartdate, string strenddate, string strEmpCodeFrom, string strEmpcodeTo, string strKi)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETHRA_DETAIL";
            //ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_STARTDATE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(strstartdate) ? (object)DBNull.Value : strstartdate;
            ocmd.Parameters.Add("V_ENDDATE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(strenddate) ? (object)DBNull.Value : strenddate;
            ocmd.Parameters.Add("V_EMPCODEFROM", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpCodeFrom) ? (object)DBNull.Value : strEmpCodeFrom;
            ocmd.Parameters.Add("V_EMPCODETO", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcodeTo) ? (object)DBNull.Value : strEmpcodeTo;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = string.IsNullOrEmpty(strKi) ? (object)DBNull.Value : strKi;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }
        public DataTable GETHRA_DETAILTXT(string strstartdate, string strenddate, string strEmpCodeFrom, string strEmpcodeTo, string strKi)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETHRA_DTLEXPTOTXT";
            //ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_STARTDATE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(strstartdate) ? (object)DBNull.Value : strstartdate;
            ocmd.Parameters.Add("V_ENDDATE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(strenddate) ? (object)DBNull.Value : strenddate;
            ocmd.Parameters.Add("V_EMPCODEFROM", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpCodeFrom) ? (object)DBNull.Value : strEmpCodeFrom;
            ocmd.Parameters.Add("V_EMPCODETO", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcodeTo) ? (object)DBNull.Value : strEmpcodeTo;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = string.IsNullOrEmpty(strKi) ? (object)DBNull.Value : strKi;
            ocmd.Parameters.Add("CUR_TAXHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        public DataTable GetLOANDTL_proj(string strEmpcode, string strki)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETLOANDTL_PROJ";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("CUR_LOANDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataTable(ocmd);
        }

        //Update Loan  Detail
        public void Update_LoanDetail_PROJ(LoanApplicationViewModel obj, string strKIID, string strEmpcode, OracleCommand objCmd)
        {

            objCmd.Parameters.Clear();
            objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT_LOANDTL_PROJ";
            objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = "0";
            objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
            objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
            objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            objCmd.Parameters.Add("V_LOANAMOUNT", OracleDbType.Varchar2).Value = obj.LoanAmount;
            objCmd.Parameters.Add("V_LOADPROVIDER", OracleDbType.Varchar2).Value = obj.LoanProviderName;
            objCmd.Parameters.Add("V_LOADPROVIDERADRESS", OracleDbType.Varchar2).Value = obj.AddressLoanProvider;
            objCmd.Parameters.Add("V_LOADPROVIDERPAN", OracleDbType.Varchar2).Value = obj.PanLoanProvider;
            objCmd.Parameters.Add("V_DATEOFLOANTKN", OracleDbType.Varchar2).Value = obj.ProjLoanTaken;
            objCmd.Parameters.Add("V_VALUEOFHOUSE", OracleDbType.Varchar2).Value = obj.ProjValueofHouse;
            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
            objCmd.CommandType = System.Data.CommandType.StoredProcedure;
            objCmd.BindByName = true;
            objCmd.ExecuteNonQuery();
            Int32 subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
            if (subResult != 1)
            {
                throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
            }

        }

        public DataSet GetLTCDtl(string strEmpcode, string strki, string strStatus)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_GETLTCDTL";
            ocmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = string.IsNullOrEmpty(strEmpcode) ? (object)DBNull.Value : strEmpcode;
            ocmd.Parameters.Add("V_KI", OracleDbType.Int32).Value = strki;
            ocmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            ocmd.Parameters.Add("CUR_LTCDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_LTCMDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
        }
        //Update LTA Exemption Detail
        public void Update_LTCDetail(ArrayList objltcList, string strKIID, string strEmpcode, OracleCommand objCmd)
        {

            if (objltcList.Count > 0)
            {
                foreach (LTAModel o in objltcList)
                {
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT_LTCRECDTL";
                    objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(o.SID) ? "0" : o.SID;
                    objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                    objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
                    objCmd.Parameters.Add("V_LTATAKEN", OracleDbType.Int32).Value = 0;
                    objCmd.Parameters.Add("V_SELLERNAME", OracleDbType.Varchar2).Value = o.SellerName;
                    objCmd.Parameters.Add("V_GSTNUMBER", OracleDbType.Varchar2).Value = o.GSTNo;
                    objCmd.Parameters.Add("V_INVOICEDATE", OracleDbType.Varchar2).Value = o.InvoiceDate;
                    objCmd.Parameters.Add("V_INVOICENO", OracleDbType.Varchar2).Value = o.InvoiceNo;
                    objCmd.Parameters.Add("V_PARTICULARSGOODS", OracleDbType.Varchar2).Value = o.Goods_Service;
                    objCmd.Parameters.Add("V_GOODSAMOUNT", OracleDbType.Int32).Value = Convert.ToInt32(o.InvoiceAMT);
                    objCmd.Parameters.Add("V_GSTRATE", OracleDbType.Int32).Value = o.GSTRate;
                    objCmd.Parameters.Add("V_GSTAMOUNT", OracleDbType.Int32).Value = Convert.ToInt32(o.GSTAmount);
                    objCmd.Parameters.Add("V_TOTALINVOICEAMOUNT", OracleDbType.Int32).Value = Convert.ToInt32(o.TotalAmount);
                    objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strEmpcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                    //objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    var subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (subResult != 1)
                    {
                        throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
                    }
                }
            }

        }
        public void Update_LTCFMDetail(ArrayList objltcFmList, string strKIID, string strEmpcode, OracleCommand objCmd)
        {

            if (objltcFmList.Count > 0)
            {
                foreach (LTAFamilyModel o in objltcFmList)
                {
                    objCmd.Parameters.Clear();
                    objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPT_LTCMDTL";
                    objCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(o.FID) ? "0" : o.FID;
                    objCmd.Parameters["V_ID"].Direction = ParameterDirection.InputOutput;
                    objCmd.Parameters.Add("V_FIN_TAXLTCMEMBERDTLID", OracleDbType.Int32).Value = 0;
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                    objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKIID;
                    objCmd.Parameters.Add("V_NAMEOFPERSON", OracleDbType.Varchar2).Value = o.Personname;
                    objCmd.Parameters.Add("V_RELATION", OracleDbType.Varchar2).Value = o.Relationship;
                    objCmd.Parameters.Add("V_AGE", OracleDbType.Varchar2).Value = o.Age;
                    objCmd.Parameters.Add("V_ISELIGIBLEAMT", OracleDbType.Int32).Value = Convert.ToInt32(Convert.ToDecimal(o.Max_Eligible));
                    objCmd.Parameters.Add("V_ISCLAIM", OracleDbType.Int32).Value = Convert.ToInt32(Convert.ToDecimal(o.ClaimExp));
                    objCmd.Parameters.Add("V_EXEM_AMOUNT", OracleDbType.Int32).Value = Convert.ToInt32(Convert.ToDecimal(o.ExemptionAmount));
                    objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strEmpcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                    //objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    var subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (subResult != 1)
                    {
                        throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
                    }
                }
            }

        }
        public void UpdateTaxEmpInvDetail(string strEmpcode, string strKi, string strRentfrom, string strRentto, string strAcctype, string strCitycat, string strIsexemp, string strhraclaim, string strRentamt, string strTaddress, string strTmobile, List<HRADetails> objRent, string[,] strInv584detail, string[,] strInvdetail, string strActive, string strIsdraft, List<ReceiptIndexViewModel> arr_586, ReceiptDtl_LTAMain objlta, EmployerB12ViewModel obj12BMain, LoanApplicationViewModel objloan, EEDetailsModels objEmp80EE, LoanApplicationViewModel HLLIST, LoanApplicationViewModel objploan,string UserId,string Type)
        {
            String strErrMsg = String.Empty;

            string strCn = objConn.getConnectingString(); ;
            OracleCommand objCmd;
            OracleTransaction tran;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                tran = objCn.BeginTransaction();

                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.Transaction = tran;
                    int loopcount = 0;
                    foreach (var rent in objRent)
                    {
                        objCmd.Parameters.Clear();
                        //string strSql = "PKG_FINTAXDECLARATION.SPROC_UPDATEEMPRENTDEATAIL";
                        string strSql = "PKG_FINTAXDECLARATION.SPROC_UPDATEEMPRENTDEATAIL_N";
                        objCmd.Parameters.Add("V_FINEMPRENTID", OracleDbType.Int32).Value = (!rent._strFINEMPRENTDTLID.HasValue) ? 0 : rent._strFINEMPRENTDTLID;
                        objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int64).Value = strEmpcode;
                        objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int64).Value = strKi;

                        objCmd.Parameters.Add("V_RENTFROM", OracleDbType.Varchar2).Value = rent.FinPeriodStartDate;
                        objCmd.Parameters.Add("V_RENTTO", OracleDbType.Varchar2).Value = rent.FinPeriodEndDate;
                        objCmd.Parameters.Add("V_ACCTYPE", OracleDbType.Int32).Value = 0;
                        objCmd.Parameters.Add("V_CITYCAT", OracleDbType.Int32).Value = rent.CityCategory;//
                        objCmd.Parameters.Add("V_ISTAXEXEMPTED", OracleDbType.Varchar2).Value = "X";
                        objCmd.Parameters.Add("V_HRACLAIM", OracleDbType.Varchar2).Value = strhraclaim;
                        objCmd.Parameters.Add("V_ADDRESS", OracleDbType.Varchar2).Value = rent.PresentResidentialAddress;//
                        objCmd.Parameters.Add("V_MOBILE", OracleDbType.Varchar2).Value = strTmobile;
                        objCmd.Parameters.Add("V_RENTAMT", OracleDbType.Decimal).Value = rent.MonthlyRentAmount;//
                        objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;

                        objCmd.Parameters.Add("V_EMPSO", OracleDbType.Varchar2).Value = rent.EmployeeSO;
                        objCmd.Parameters.Add("V_LANDLORDNAME", OracleDbType.Varchar2).Value = rent.LandLord1.LandlordName;
                        objCmd.Parameters.Add("V_LANDLORDSO", OracleDbType.Varchar2).Value = rent.LandLord1.LandlordSO;
                        objCmd.Parameters.Add("V_LANDLORDRESHNO", OracleDbType.Varchar2).Value = rent.LandLord1.LandlordResHNo;
                        objCmd.Parameters.Add("V_LANDLORDRESGNO", OracleDbType.Varchar2).Value = rent.LandLord1.LandlordResGNo;
                        objCmd.Parameters.Add("V_LANDLORDRESVILLAGE", OracleDbType.Varchar2).Value = rent.LandLord1.LandlordResVillage;
                        objCmd.Parameters.Add("V_LANDLORDRESCITY", OracleDbType.Varchar2).Value = rent.LandLord1.LandlordResCity;
                        objCmd.Parameters.Add("V_LANDLORDRESPINCODE", OracleDbType.Varchar2).Value = rent.LandLord1.LandlordResPincode;
                        objCmd.Parameters.Add("V_LANDLORDPAN", OracleDbType.Varchar2).Value = rent.LandLord1.LandlordPAN;
                        objCmd.Parameters.Add("V_ISLANPAN", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(rent.LandLord1.LandlordPAN) ? (object)DBNull.Value : "1";
                        objCmd.Parameters.Add("V_LANDLORDMNO", OracleDbType.Varchar2).Value = rent.LandLord1.LandlordMobileNo;

                        objCmd.Parameters.Add("V_LANDLORDNAME2", OracleDbType.Varchar2).Value = rent.LandLord2.LandlordName;
                        objCmd.Parameters.Add("V_LANDLORDSO2", OracleDbType.Varchar2).Value = rent.LandLord2.LandlordSO;
                        objCmd.Parameters.Add("V_LANDLORDRESHNO2", OracleDbType.Varchar2).Value = rent.LandLord2.LandlordResHNo;
                        objCmd.Parameters.Add("V_LANDLORDRESGNO2", OracleDbType.Varchar2).Value = rent.LandLord2.LandlordResGNo;
                        objCmd.Parameters.Add("V_LANDLORDRESVILLAGE2", OracleDbType.Varchar2).Value = rent.LandLord2.LandlordResVillage;
                        objCmd.Parameters.Add("V_LANDLORDRESCITY2", OracleDbType.Varchar2).Value = rent.LandLord2.LandlordResCity;
                        objCmd.Parameters.Add("V_LANDLORDRESPINCODE2", OracleDbType.Varchar2).Value = rent.LandLord2.LandlordResPincode;
                        objCmd.Parameters.Add("V_LANDLORDPAN2", OracleDbType.Varchar2).Value = rent.LandLord2.LandlordPAN;
                        objCmd.Parameters.Add("V_LANDLORDMNO2", OracleDbType.Varchar2).Value = rent.LandLord2.LandlordMobileNo;

                        objCmd.Parameters.Add("V_LANDLORDNAME3", OracleDbType.Varchar2).Value = rent.LandLord3.LandlordName;
                        objCmd.Parameters.Add("V_LANDLORDSO3", OracleDbType.Varchar2).Value = rent.LandLord3.LandlordSO;
                        objCmd.Parameters.Add("V_LANDLORDRESHNO3", OracleDbType.Varchar2).Value = rent.LandLord3.LandlordResHNo;
                        objCmd.Parameters.Add("V_LANDLORDRESGNO3", OracleDbType.Varchar2).Value = rent.LandLord3.LandlordResGNo;
                        objCmd.Parameters.Add("V_LANDLORDRESVILLAGE3", OracleDbType.Varchar2).Value = rent.LandLord3.LandlordResVillage;
                        objCmd.Parameters.Add("V_LANDLORDRESCITY3", OracleDbType.Varchar2).Value = rent.LandLord3.LandlordResCity;
                        objCmd.Parameters.Add("V_LANDLORDRESPINCODE3", OracleDbType.Varchar2).Value = rent.LandLord3.LandlordResPincode;
                        objCmd.Parameters.Add("V_LANDLORDPAN3", OracleDbType.Varchar2).Value = rent.LandLord3.LandlordPAN;
                        objCmd.Parameters.Add("V_LANDLORDMNO3", OracleDbType.Varchar2).Value = rent.LandLord3.LandlordMobileNo;

                        objCmd.Parameters.Add("V_LANDLORDNAME4", OracleDbType.Varchar2).Value = rent.LandLord4.LandlordName;
                        objCmd.Parameters.Add("V_LANDLORDSO4", OracleDbType.Varchar2).Value = rent.LandLord4.LandlordSO;
                        objCmd.Parameters.Add("V_LANDLORDRESHNO4", OracleDbType.Varchar2).Value = rent.LandLord4.LandlordResHNo;
                        objCmd.Parameters.Add("V_LANDLORDRESGNO4", OracleDbType.Varchar2).Value = rent.LandLord4.LandlordResGNo;
                        objCmd.Parameters.Add("V_LANDLORDRESVILLAGE4", OracleDbType.Varchar2).Value = rent.LandLord4.LandlordResVillage;
                        objCmd.Parameters.Add("V_LANDLORDRESCITY4", OracleDbType.Varchar2).Value = rent.LandLord4.LandlordResCity;
                        objCmd.Parameters.Add("V_LANDLORDRESPINCODE4", OracleDbType.Varchar2).Value = rent.LandLord4.LandlordResPincode;
                        objCmd.Parameters.Add("V_LANDLORDPAN4", OracleDbType.Varchar2).Value = rent.LandLord4.LandlordPAN;
                        objCmd.Parameters.Add("V_LANDLORDMNO4", OracleDbType.Varchar2).Value = rent.LandLord4.LandlordMobileNo;

                        objCmd.Parameters.Add("V_LOOPCOUNT", OracleDbType.Int32).Value = loopcount;
                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                        objCmd.CommandText = strSql;
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.ExecuteNonQuery();
                        int subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                        string strResult = objCmd.Parameters["ERROR_MSG"].Value.ToString();
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update HRA details. Transaction will rollback now.");
                        }
                        loopcount++;
                    }


                    for (int i = 0; i < strInv584detail.GetLength(0); i++)
                    {
                        objCmd.Parameters.Clear();
                        objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPDATEEMP584_INVDEC";
                        objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                        objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKi;
                        objCmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = decimal.Parse(strInv584detail[i, 0]).ToString("0") ;
                        objCmd.Parameters.Add("V_TAXHEADOPTID", OracleDbType.Int32).Value = string.IsNullOrEmpty(strInv584detail[i, 1]) ? (object)DBNull.Value : decimal.Parse(strInv584detail[i, 1]).ToString("0");
                        objCmd.Parameters.Add("V_ACTIVE", OracleDbType.Int32).Value = strActive;
                        objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;
                        objCmd.Parameters.Add("V_PROJ_AMOUNT", OracleDbType.Int32).Value =string.IsNullOrEmpty(strInv584detail[i, 2]) ? (object)DBNull.Value : decimal.Parse(strInv584detail[i, 2]).ToString("0");
                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.ExecuteNonQuery();
                        int subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
                        }
                    }
                    for (int i = 0; i < strInvdetail.GetLength(0); i++)
                    {
                        objCmd.Parameters.Clear();
                        objCmd.CommandText = "PKG_FINTAXDECLARATION.SPROC_UPDATEEMPTAX_INVDEC";
                        objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                        objCmd.Parameters.Add("V_SYKIID", OracleDbType.Int32).Value = strKi;
                        objCmd.Parameters.Add("V_TAXHEADID", OracleDbType.Int32).Value = decimal.Parse(strInvdetail[i, 0]).ToString("0") ;
                        objCmd.Parameters.Add("V_TAXHEADOPTID", OracleDbType.Int32).Value = (object)DBNull.Value;// string.IsNullOrEmpty(strInvdetail[i, 1]) ? (object)DBNull.Value : strInvdetail[i, 1];
                        objCmd.Parameters.Add("V_ACTIVE", OracleDbType.Int32).Value = strActive;
                        objCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = UserId;
                        objCmd.Parameters.Add("V_PROJ_AMOUNT", OracleDbType.Int32).Value = string.IsNullOrEmpty(strInvdetail[i, 2]) ? (object)DBNull.Value : decimal.Parse(strInvdetail[i, 2]).ToString("0");
                        objCmd.Parameters.Add("V_ACT_AMOUNT", OracleDbType.Int32).Value = string.IsNullOrEmpty(strInvdetail[i, 3])? (object)DBNull.Value : decimal.Parse(strInvdetail[i, 3]).ToString("0");
                        objCmd.Parameters.Add("V_SAVEASDRAFT", OracleDbType.Int32).Value = string.IsNullOrEmpty(strIsdraft) ? (object)DBNull.Value : strIsdraft;
                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                        //objCmd.CommandText = strSql;
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.ExecuteNonQuery();

                        int subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update investment declaration. Transaction will rollback now.");
                        }
                    }

                    if (arr_586 != null)
                        Update586Receiptdtl(arr_586, strKi, strEmpcode, objCmd, "",UserId);
                    
                    if (!string.IsNullOrEmpty(objlta?.TotEligableAmt) && Type == "2")
                        UpdateReceipt_LTADetail(objlta, strKi, strEmpcode, objCmd, UserId);
                    if (obj12BMain != null && Type == "1")
                        Update_12BDetail(obj12BMain, strKi, strEmpcode, objCmd, UserId);
                    if (!string.IsNullOrEmpty(objloan?.FinalTotalAmount) && Type == "2")
                        Update_LoanDetail(objloan, strKi, strEmpcode, objCmd);
                    if (HLLIST?.LoanProviders != null && Type == "2")
                        UpdateHLListDetail(HLLIST, objCmd, strEmpcode);
                    if (!string.IsNullOrEmpty(objEmp80EE?.EEamount) && Type == "2")
                        Update_80EEDetail(objEmp80EE, strKi, strEmpcode, objCmd);
                    if (objploan != null && Type == "1")
                        Update_LoanDetail_PROJ(objploan, strKi, strEmpcode, objCmd);
                    //if (objltcList != null)
                    //    Update_LTCDetail(objltcList, strKi, strEmpcode, objCmd);
                    //if (objltcmList != null)
                    //    Update_LTCFMDetail(objltcmList, strKi, strEmpcode, objCmd);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch (OracleException x)
                    {
                        if (tran.Connection != null)
                        {
                            throw new Exception("An exception of type " + x.GetType() + " was encountered while attempting to roll back the transaction.");
                        }
                    }
                    throw new Exception(ex.ToString());
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

        public void Update_TaxPeriod(string strtaxregime, string strEmpcode, string KII)
        {
            string strCn = objConn.getConnectingString(); using (OracleConnection objCn = new OracleConnection(strCn)) using (OracleCommand ocmd = objCn.CreateCommand())
            {
                objCn.Open(); ocmd.CommandText = @" UPDATE FIN_TAXUSERPERIOD SET ISACTUALFILLED = 1, TXDCLTYPE = :taxregime WHERE ADEMPCODE = :empcode AND SYKIID = :kii"; ocmd.CommandType = CommandType.Text; // Add parameters safely
                                                                                                                                                                                                                ocmd.Parameters.Add(new OracleParameter("taxregime", strtaxregime)); ocmd.Parameters.Add(new OracleParameter("empcode", strEmpcode)); ocmd.Parameters.Add(new OracleParameter("kii", KII)); odmgt.ExecuteQuery(ocmd); 
            }
        }
        public void Update_TaxPeriodProjected(string strtaxregime, string strEmpcode, string KII)
        {
            string strCn = objConn.getConnectingString(); using (OracleConnection objCn = new OracleConnection(strCn)) using (OracleCommand ocmd = objCn.CreateCommand())
            {
                objCn.Open(); ocmd.CommandText = @" UPDATE FIN_TAXUSERPERIOD SET ISPROJFILLED= 1, TXDCLTYPE = :taxregime WHERE ADEMPCODE = :empcode AND SYKIID = :kii"; ocmd.CommandType = CommandType.Text; // Add parameters safely
                ocmd.Parameters.Add(new OracleParameter("taxregime", strtaxregime)); ocmd.Parameters.Add(new OracleParameter("empcode", strEmpcode)); ocmd.Parameters.Add(new OracleParameter("kii", KII)); odmgt.ExecuteQuery(ocmd);
            }
        }
        public void Update_TaxFinalPeriodProjected(string strtaxregime, string strEmpcode, string KII)
        {
            string strCn = objConn.getConnectingString(); using (OracleConnection objCn = new OracleConnection(strCn)) using (OracleCommand ocmd = objCn.CreateCommand())
            {
                objCn.Open(); ocmd.CommandText = @"update FIN_TAXUSERPERIOD set ISEDITABLE=0, ISPROJFILLED=1,TXDCLTYPE = :taxregime WHERE ADEMPCODE = :empcode AND SYKIID = :kii"; ocmd.CommandType = CommandType.Text; // Add parameters safely
                ocmd.Parameters.Add(new OracleParameter("taxregime", strtaxregime)); ocmd.Parameters.Add(new OracleParameter("empcode", strEmpcode)); ocmd.Parameters.Add(new OracleParameter("kii", KII)); odmgt.ExecuteQuery(ocmd);
            }
        }
        public void Update_TaxFinalPeriod(string strtaxregime, string strEmpcode, string KII)
        {
            string strCn = objConn.getConnectingString(); using (OracleConnection objCn = new OracleConnection(strCn)) using (OracleCommand ocmd = objCn.CreateCommand())
            {
                objCn.Open(); ocmd.CommandText = @"update FIN_TAXUSERPERIOD set ISEDITABLE_ACTUAL=0, ISACTUALFILLED=1,TXDCLTYPE = :taxregime WHERE ADEMPCODE = :empcode AND SYKIID = :kii"; ocmd.CommandType = CommandType.Text; // Add parameters safely
                ocmd.Parameters.Add(new OracleParameter("taxregime", strtaxregime)); ocmd.Parameters.Add(new OracleParameter("empcode", strEmpcode)); ocmd.Parameters.Add(new OracleParameter("kii", KII)); odmgt.ExecuteQuery(ocmd);
            }
        }
      

    }


}

