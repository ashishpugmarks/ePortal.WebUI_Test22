using ePortal.Persistence.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Shared;

namespace ePortal.Persistence.Services
{
    public class CustomerMgmtRepo : ICustomerMgmtRepo
    {
        #region "Local Variables"
        private string _ErrorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

        DataSet ds = new DataSet();
        DataRow[] _datarow;
        DataTable dt;
        private readonly IDataManagement oDataMgmt ;
        private readonly ICommonFunctions objcmn;
        #endregion
        public CustomerMgmtRepo(IDataManagement _oDataMgmt, ICommonFunctions _objcmn)
        {
            oDataMgmt = _oDataMgmt;
            objcmn = _objcmn;
        }
        #region "Get Data Result"

        /// <summary>
        /// Get list of finance approval user list
        /// </summary>
        /// <returns></returns>
        public DataSet GetCutomerFieldDataList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_CUSTOMERMASTER.GETMASTERMGMTDATA";
            oCmd.Parameters.Add("CUR_MASTERDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        public DataSet GetCustomerMasterData(string Group_Name)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_CUSTOMERMASTER.GETMASTERDATA";
            oCmd.Parameters.Add("GRPUP_NAME_IN", OracleDbType.Varchar2).Value = Group_Name;
            oCmd.Parameters.Add("CUR_MASTERDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_ALLDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        public DataSet GetCustomerMasterDataWithPGRP(string Group_Name, string PGroup_Name)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_CUSTOMERMASTER.GETMASTERDATAWITHPGRP";
            oCmd.Parameters.Add("GRPUP_NAME_IN", OracleDbType.Varchar2).Value = Group_Name;
            oCmd.Parameters.Add("PGRPUP_NAME_IN", OracleDbType.Varchar2).Value = PGroup_Name;
            oCmd.Parameters.Add("CUR_MASTERDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_ALLDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        public DataSet GetCustomerMandatoryData(string ag5AG1, string ag5AG3, string ag5AGV, string ag5AGG, string ag5AG6, string ag5AGW, string ag5AG2, string ag5AG5, string group_name)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMASTERDATA_GET";
            oCmd.Parameters.Add("AG_5AG1_IN", OracleDbType.Varchar2).Value = ag5AG1;
            oCmd.Parameters.Add("AG_5AG3_IN", OracleDbType.Varchar2).Value = ag5AG3;
            oCmd.Parameters.Add("AG_5AGV_IN", OracleDbType.Varchar2).Value = ag5AGV;
            oCmd.Parameters.Add("AG_5AGG_IN", OracleDbType.Varchar2).Value = ag5AGG;
            oCmd.Parameters.Add("AG_5AG6_IN", OracleDbType.Varchar2).Value = ag5AG6;
            oCmd.Parameters.Add("AG_5AGW_IN", OracleDbType.Varchar2).Value = ag5AGW;
            oCmd.Parameters.Add("AG_5AG2_IN", OracleDbType.Varchar2).Value = ag5AG2;
            oCmd.Parameters.Add("AG_5AG5_IN", OracleDbType.Varchar2).Value = ag5AG5;
            oCmd.Parameters.Add("GROUP_NAME_IN", OracleDbType.Varchar2).Value = group_name;

            oCmd.Parameters.Add("CUR_MASTERDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MASTERDATA_GROUPWISE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        public DataSet GetSectionHead(int _userId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_CUSTOMERMASTER.SECTIONHEAD_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = _userId;
            oCmd.Parameters.Add("CUR_SECTMGNAMEGET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        public DataSet GetDepartmentHead(int _userId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_CUSTOMERMASTER.SPROC_DEPARTMENTHEAD_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Int64).Value = _userId;
            oCmd.Parameters.Add("CUR_APPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_OPHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        public string SaveData(string GROUP_NAME_IN, long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CUST_ACC_TYPE_IN,
    string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string CUSTOMER_CODE_IN, string NAME1_IN,
    string NAME2_IN, string NAME3_IN, string NAME4_IN, string SEARCHTERM_IN, string STREETHOUSENUMBER_IN, string STREET2_IN, string STREET3_IN,
    string STREET4_IN, string STREET5_IN, string POSTALCODE_IN, string CITY_IN, string COUNTRY_IN, string CREGION_IN, string CTIMEZONE_IN, string TRANSPORTATIONCODE_IN,
    string MOBILEPHONE_IN, string FAX_IN, string EMAIL_IN, string INDUSTRY_IN, string TAXNUMBER3_IN, string CITYCODE_IN, string CTRY_IN, string BANKKEY_IN, string BANKACCOUNT_IN,
    string ACCOUNTHOLDER_IN, string BANKCONTROLKEY_IN, string BANKNAME_IN, string BANKREGION_IN, string BANKSTREET_IN, string BANKCITY_IN, string BANKBRANCH_IN, string CUSTOMERCLASS_IN
    , string INDUSTRYCODE1_IN, string FIRSTNAME_IN, string INDUSTRY1_IN, string INDUSTRYNAME_IN, string RECONSACCOUNT_IN, string TERMOFPAYMENT_IN, string CURRENCY_IN, string PAYMENTMETHODS_IN, string HOUSEBANK_IN, string PMTMETHSUPL_IN
    , string WITHHOLDINGTAXTYPE_IN, string WITHHOLDINGTAXCODE_IN, string VALIDFROM_IN, string VALIDTO_IN, string WITHHOLDINGTAXNUMBER_IN, string INCOTERM_IN, string CREDITCONTROLAREA_IN,
    string ACCASSIGMENTGROUP_IN, string TAXCLASSIFICATION_IN, string EXCHANGERATETYPE_IN, string SALESDISTRICT_IN, string SALESOFFICE_IN, string SALESGROUP_IN, string CUSTOMERGROUP_IN
    , string PRICEGROUP_IN, string CUSTPRICPROC1_IN, string DELIVERYPRIORITYGROUP_IN, string SHIPPINGCONDITION_IN, string CSTNO_IN, string LSTNO_IN, string INVOICINGDATES_IN
    , string INVOICINGLISTDATES_IN, string SERREGNO_IN, string PANNUMBER_IN, string PAYMENTGUARANTEEPROC_IN, string EMAILID_IN)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAIL_ADD";
                oCmd.Parameters.Add("GROUP_NAME_IN", OracleDbType.Varchar2).Value = GROUP_NAME_IN;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = USERID_IN;
                oCmd.Parameters.Add("CMDETAILID_IN", OracleDbType.Int64).Value = CMDETAILID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Int64).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("CUST_ACC_TYPE_IN", OracleDbType.Varchar2).Value = CUST_ACC_TYPE_IN;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = SALES_ORG_IN;
                oCmd.Parameters.Add("DIVISION_GRP_IN", OracleDbType.Varchar2).Value = DIVISION_GRP_IN;
                oCmd.Parameters.Add("DISTRIBTUION_CHH_IN", OracleDbType.Varchar2).Value = DISTRIBTUION_CHH_IN;
                oCmd.Parameters.Add("CUSTOMER_CODE_IN", OracleDbType.Varchar2).Value = CUSTOMER_CODE_IN;
                oCmd.Parameters.Add("STREETHOUSENUMBER_IN", OracleDbType.Varchar2).Value = STREETHOUSENUMBER_IN;
                oCmd.Parameters.Add("NAME2_IN", OracleDbType.Varchar2).Value = NAME2_IN;
                oCmd.Parameters.Add("NAME3_IN", OracleDbType.Varchar2).Value = NAME3_IN;
                oCmd.Parameters.Add("NAME4_IN", OracleDbType.Varchar2).Value = NAME4_IN;
                oCmd.Parameters.Add("SEARCHTERM_IN", OracleDbType.Varchar2).Value = SEARCHTERM_IN;
                oCmd.Parameters.Add("STREET2_IN", OracleDbType.Varchar2).Value = STREET2_IN;
                oCmd.Parameters.Add("STREET3_IN", OracleDbType.Varchar2).Value = STREET3_IN;
                oCmd.Parameters.Add("STREET4_IN", OracleDbType.Varchar2).Value = STREET4_IN;
                oCmd.Parameters.Add("STREET5_IN", OracleDbType.Varchar2).Value = STREET5_IN;
                oCmd.Parameters.Add("POSTALCODE_IN", OracleDbType.Varchar2).Value = POSTALCODE_IN;
                oCmd.Parameters.Add("CITY_IN", OracleDbType.Varchar2).Value = CITY_IN;
                oCmd.Parameters.Add("COUNTRY_IN", OracleDbType.Varchar2).Value = COUNTRY_IN;
                oCmd.Parameters.Add("CREGION_IN", OracleDbType.Varchar2).Value = CREGION_IN;
                oCmd.Parameters.Add("CTIMEZONE_IN", OracleDbType.Varchar2).Value = CTIMEZONE_IN;
                oCmd.Parameters.Add("TRANSPORTATIONCODE_IN", OracleDbType.Varchar2).Value = TRANSPORTATIONCODE_IN;
                oCmd.Parameters.Add("MOBILEPHONE_IN", OracleDbType.Varchar2).Value = MOBILEPHONE_IN;
                oCmd.Parameters.Add("FAX_IN", OracleDbType.Varchar2).Value = FAX_IN;
                oCmd.Parameters.Add("EMAIL_IN", OracleDbType.Varchar2).Value = EMAIL_IN;
                oCmd.Parameters.Add("INDUSTRY_IN", OracleDbType.Varchar2).Value = INDUSTRY_IN;

                oCmd.Parameters.Add("TAXNUMBER3_IN", OracleDbType.Varchar2).Value = TAXNUMBER3_IN;
                oCmd.Parameters.Add("CITYCODE_IN", OracleDbType.Varchar2).Value = CITYCODE_IN;
                oCmd.Parameters.Add("CTRY_IN", OracleDbType.Varchar2).Value = CTRY_IN;
                oCmd.Parameters.Add("BANKKEY_IN", OracleDbType.Varchar2).Value = BANKKEY_IN;
                oCmd.Parameters.Add("BANKACCOUNT_IN", OracleDbType.Varchar2).Value = BANKACCOUNT_IN;
                oCmd.Parameters.Add("ACCOUNTHOLDER_IN", OracleDbType.Varchar2).Value = ACCOUNTHOLDER_IN;
                oCmd.Parameters.Add("BANKCONTROLKEY_IN", OracleDbType.Varchar2).Value = BANKCONTROLKEY_IN;
                oCmd.Parameters.Add("BANKNAME_IN", OracleDbType.Varchar2).Value = BANKNAME_IN;
                oCmd.Parameters.Add("BANKREGION_IN", OracleDbType.Varchar2).Value = BANKREGION_IN;
                oCmd.Parameters.Add("BANKSTREET_IN", OracleDbType.Varchar2).Value = BANKSTREET_IN;
                oCmd.Parameters.Add("BANKCITY_IN", OracleDbType.Varchar2).Value = BANKCITY_IN;
                oCmd.Parameters.Add("BANKBRANCH_IN", OracleDbType.Varchar2).Value = BANKBRANCH_IN;
                oCmd.Parameters.Add("CUSTOMERCLASS_IN", OracleDbType.Varchar2).Value = CUSTOMERCLASS_IN;
                oCmd.Parameters.Add("INDUSTRYCODE1_IN", OracleDbType.Varchar2).Value = INDUSTRYCODE1_IN;
                oCmd.Parameters.Add("FIRSTNAME_IN", OracleDbType.Varchar2).Value = FIRSTNAME_IN;
                oCmd.Parameters.Add("INDUSTRY1_IN", OracleDbType.Varchar2).Value = INDUSTRY1_IN;
                oCmd.Parameters.Add("INDUSTRYNAME_IN", OracleDbType.Varchar2).Value = INDUSTRYNAME_IN;
                oCmd.Parameters.Add("RECONSACCOUNT_IN", OracleDbType.Varchar2).Value = RECONSACCOUNT_IN;
                oCmd.Parameters.Add("TERMOFPAYMENT_IN", OracleDbType.Varchar2).Value = TERMOFPAYMENT_IN;
                oCmd.Parameters.Add("CURRENCY_IN", OracleDbType.Varchar2).Value = CURRENCY_IN;
                oCmd.Parameters.Add("PAYMENTMETHODS_IN", OracleDbType.Varchar2).Value = PAYMENTMETHODS_IN;
                oCmd.Parameters.Add("HOUSEBANK_IN", OracleDbType.Varchar2).Value = HOUSEBANK_IN;
                oCmd.Parameters.Add("PMTMETHSUPL_IN", OracleDbType.Varchar2).Value = PMTMETHSUPL_IN;

                oCmd.Parameters.Add("WITHHOLDINGTAXTYPE_IN", OracleDbType.Varchar2).Value = WITHHOLDINGTAXTYPE_IN;
                oCmd.Parameters.Add("WITHHOLDINGTAXCODE_IN", OracleDbType.Varchar2).Value = WITHHOLDINGTAXCODE_IN;
                oCmd.Parameters.Add("VALIDFROM_IN", OracleDbType.Varchar2).Value = VALIDFROM_IN;
                oCmd.Parameters.Add("VALIDTO_IN", OracleDbType.Varchar2).Value = VALIDTO_IN;
                oCmd.Parameters.Add("WITHHOLDINGTAXNUMBER_IN", OracleDbType.Varchar2).Value = WITHHOLDINGTAXNUMBER_IN;
                oCmd.Parameters.Add("INCOTERM_IN", OracleDbType.Varchar2).Value = INCOTERM_IN;
                oCmd.Parameters.Add("CREDITCONTROLAREA_IN", OracleDbType.Varchar2).Value = CREDITCONTROLAREA_IN;
                oCmd.Parameters.Add("ACCASSIGMENTGROUP_IN", OracleDbType.Varchar2).Value = ACCASSIGMENTGROUP_IN;
                oCmd.Parameters.Add("TAXCLASSIFICATION_IN", OracleDbType.Varchar2).Value = TAXCLASSIFICATION_IN;
                oCmd.Parameters.Add("EXCHANGERATETYPE_IN", OracleDbType.Varchar2).Value = EXCHANGERATETYPE_IN;
                oCmd.Parameters.Add("SALESDISTRICT_IN", OracleDbType.Varchar2).Value = SALESDISTRICT_IN;
                oCmd.Parameters.Add("SALESOFFICE_IN", OracleDbType.Varchar2).Value = SALESOFFICE_IN;
                oCmd.Parameters.Add("SALESGROUP_IN", OracleDbType.Varchar2).Value = SALESGROUP_IN;
                oCmd.Parameters.Add("CUSTOMERGROUP_IN", OracleDbType.Varchar2).Value = CUSTOMERGROUP_IN;
                oCmd.Parameters.Add("PRICEGROUP_IN", OracleDbType.Varchar2).Value = PRICEGROUP_IN;
                oCmd.Parameters.Add("CUSTPRICPROC1_IN", OracleDbType.Varchar2).Value = CUSTPRICPROC1_IN;
                oCmd.Parameters.Add("DELIVERYPRIORITYGROUP_IN", OracleDbType.Varchar2).Value = DELIVERYPRIORITYGROUP_IN;
                oCmd.Parameters.Add("SHIPPINGCONDITION_IN", OracleDbType.Varchar2).Value = SHIPPINGCONDITION_IN;
                oCmd.Parameters.Add("CSTNO_IN", OracleDbType.Varchar2).Value = CSTNO_IN;
                oCmd.Parameters.Add("LSTNO_IN", OracleDbType.Varchar2).Value = LSTNO_IN;
                oCmd.Parameters.Add("INVOICINGDATES_IN", OracleDbType.Varchar2).Value = INVOICINGDATES_IN;
                oCmd.Parameters.Add("INVOICINGLISTDATES_IN", OracleDbType.Varchar2).Value = INVOICINGLISTDATES_IN;
                oCmd.Parameters.Add("SERREGNO_IN", OracleDbType.Varchar2).Value = SERREGNO_IN;
                oCmd.Parameters.Add("PANNUMBER_IN", OracleDbType.Varchar2).Value = PANNUMBER_IN;
                oCmd.Parameters.Add("PAYMENTGUARANTEEPROC_IN", OracleDbType.Varchar2).Value = PAYMENTGUARANTEEPROC_IN;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID_IN;

                oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public string SaveGENERALData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN,
    string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string CUSTOMER_CODE_IN, string TITLE_IN, string NAME1_IN,
    string NAME2_IN, string NAME3_IN, string NAME4_IN, string SEARCHTERM_IN, string STREETHOUSENUMBER_IN, string STREET2_IN, string STREET3_IN,
    string STREET4_IN, string STREET5_IN, string POSTALCODE_IN, string CITY_IN, string COUNTRY_IN, string CREGION_IN, string CTIMEZONE_IN, string TRANSPORTATIONCODE_IN,
    string MOBILEPHONE_IN, string FAX_IN, string EMAIL_IN, string INDUSTRY_IN, string TAXNUMBER3_IN, string CITYCODE_IN, string CTRY_IN, string CUSTOMERCLASS_IN,
      string VENDOR_CODE_IN, string CIN_GST_NO_FILE_IN, string EMAILID_IN, ref string header_id, ref string detailid) // **Added VENDOR_CODE_IN**
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAILGENRAL_ADD";
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = USERID_IN;
                oCmd.Parameters.Add("CMDETAILID_IN", OracleDbType.Int64).Value = CMDETAILID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Int64).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("CMREQUEST_TYPE_IN", OracleDbType.Varchar2).Value = CMREQUEST_TYPE_IN;
                oCmd.Parameters.Add("REQUEST_TYPE_IN", OracleDbType.Varchar2).Value = REQUEST_TYPE_IN;
                oCmd.Parameters.Add("COMPANY_CODE_IN", OracleDbType.Varchar2).Value = COMPANY_CODE_IN;
                oCmd.Parameters.Add("CUST_ACC_TYPE_IN", OracleDbType.Varchar2).Value = CUST_ACC_TYPE_IN;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = SALES_ORG_IN;
                oCmd.Parameters.Add("DIVISION_GRP_IN", OracleDbType.Varchar2).Value = DIVISION_GRP_IN;
                oCmd.Parameters.Add("DISTRIBTUION_CHH_IN", OracleDbType.Varchar2).Value = DISTRIBTUION_CHH_IN;
                oCmd.Parameters.Add("CUSTOMER_CODE_IN", OracleDbType.Varchar2, 10).Value = CUSTOMER_CODE_IN;
                oCmd.Parameters.Add("STREETHOUSENUMBER_IN", OracleDbType.Varchar2).Value = STREETHOUSENUMBER_IN;
                oCmd.Parameters.Add("TITLE_IN", OracleDbType.Varchar2).Value = TITLE_IN;
                oCmd.Parameters.Add("NAME1_IN", OracleDbType.Varchar2).Value = NAME1_IN;
                oCmd.Parameters.Add("NAME2_IN", OracleDbType.Varchar2).Value = NAME2_IN;
                oCmd.Parameters.Add("NAME3_IN", OracleDbType.Varchar2).Value = NAME3_IN;
                oCmd.Parameters.Add("NAME4_IN", OracleDbType.Varchar2).Value = NAME4_IN;
                oCmd.Parameters.Add("SEARCHTERM_IN", OracleDbType.Varchar2).Value = SEARCHTERM_IN;
                oCmd.Parameters.Add("STREET2_IN", OracleDbType.Varchar2).Value = STREET2_IN;
                oCmd.Parameters.Add("STREET3_IN", OracleDbType.Varchar2).Value = STREET3_IN;
                oCmd.Parameters.Add("STREET4_IN", OracleDbType.Varchar2).Value = STREET4_IN;
                oCmd.Parameters.Add("STREET5_IN", OracleDbType.Varchar2).Value = STREET5_IN;
                oCmd.Parameters.Add("POSTALCODE_IN", OracleDbType.Varchar2).Value = POSTALCODE_IN;
                oCmd.Parameters.Add("CITY_IN", OracleDbType.Varchar2).Value = CITY_IN;
                oCmd.Parameters.Add("COUNTRY_IN", OracleDbType.Varchar2).Value = COUNTRY_IN;
                oCmd.Parameters.Add("CREGION_IN", OracleDbType.Varchar2).Value = CREGION_IN;
                oCmd.Parameters.Add("CTIMEZONE_IN", OracleDbType.Varchar2).Value = CTIMEZONE_IN;
                oCmd.Parameters.Add("TRANSPORTATIONCODE_IN", OracleDbType.Varchar2).Value = TRANSPORTATIONCODE_IN;
                oCmd.Parameters.Add("MOBILEPHONE_IN", OracleDbType.Varchar2).Value = MOBILEPHONE_IN;
                oCmd.Parameters.Add("FAX_IN", OracleDbType.Varchar2).Value = FAX_IN;
                oCmd.Parameters.Add("EMAIL_IN", OracleDbType.Varchar2).Value = EMAIL_IN;
                oCmd.Parameters.Add("INDUSTRY_IN", OracleDbType.Varchar2).Value = INDUSTRY_IN;
                oCmd.Parameters.Add("TAXNUMBER3_IN", OracleDbType.Varchar2).Value = TAXNUMBER3_IN;
                oCmd.Parameters.Add("CITYCODE_IN", OracleDbType.Varchar2).Value = CITYCODE_IN;
                oCmd.Parameters.Add("CTRY_IN", OracleDbType.Varchar2).Value = CTRY_IN;
                oCmd.Parameters.Add("CUSTOMERCLASS_IN", OracleDbType.Varchar2).Value = CUSTOMERCLASS_IN;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID_IN;
                oCmd.Parameters.Add("VENDOR_CODE_IN", OracleDbType.Varchar2, 10).Value = VENDOR_CODE_IN; // **Added VENDOR_CODE_IN**          
                oCmd.Parameters.Add("CIN_GST_NO_FILE_IN", OracleDbType.Varchar2).Value = CIN_GST_NO_FILE_IN;
                oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("HEADERID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("DETAILID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                {
                    detailid = oCmd.Parameters["DETAILID_OUT"].Value.ToString();
                    header_id = oCmd.Parameters["HEADERID_OUT"].Value.ToString();
                    return "1";
                }
                else
                    return oCmd.Parameters["ERROR_MSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public string SaveBANKData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string BANKKEY_IN,
    string BANKACCOUNT_IN, string ACCOUNTHOLDER_IN, string BANKCONTROLKEY_IN, string BANKNAME_IN, string BANKREGION_IN, string BANKSTREET_IN, string BANKCITY_IN,
    string BANKBRANCH_IN, string BANKCURRENCY_IN, string BANK_MANDATE_FILE_IN, string BANK_CANCELED_FILE_IN, string CTRY_IN, string EMAILID_IN, ref string header_id, ref string detailid)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAILBANK_ADD";
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = USERID_IN;
                oCmd.Parameters.Add("CMDETAILID_IN", OracleDbType.Int64).Value = CMDETAILID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Int64).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("CMREQUEST_TYPE_IN", OracleDbType.Varchar2).Value = CMREQUEST_TYPE_IN;
                oCmd.Parameters.Add("REQUEST_TYPE_IN", OracleDbType.Varchar2).Value = REQUEST_TYPE_IN;
                oCmd.Parameters.Add("COMPANY_CODE_IN", OracleDbType.Varchar2).Value = COMPANY_CODE_IN;
                oCmd.Parameters.Add("CUST_ACC_TYPE_IN", OracleDbType.Varchar2).Value = CUST_ACC_TYPE_IN;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = SALES_ORG_IN;
                oCmd.Parameters.Add("DIVISION_GRP_IN", OracleDbType.Varchar2).Value = DIVISION_GRP_IN;
                oCmd.Parameters.Add("DISTRIBTUION_CHH_IN", OracleDbType.Varchar2).Value = DISTRIBTUION_CHH_IN;
                oCmd.Parameters.Add("BANKKEY_IN", OracleDbType.Varchar2).Value = BANKKEY_IN;
                oCmd.Parameters.Add("BANKACCOUNT_IN", OracleDbType.Varchar2).Value = BANKACCOUNT_IN;
                oCmd.Parameters.Add("ACCOUNTHOLDER_IN", OracleDbType.Varchar2).Value = ACCOUNTHOLDER_IN;
                oCmd.Parameters.Add("BANKCONTROLKEY_IN", OracleDbType.Varchar2).Value = BANKCONTROLKEY_IN;
                oCmd.Parameters.Add("BANKNAME_IN", OracleDbType.Varchar2).Value = BANKNAME_IN;
                oCmd.Parameters.Add("BANKREGION_IN", OracleDbType.Varchar2).Value = BANKREGION_IN;
                oCmd.Parameters.Add("BANKSTREET_IN", OracleDbType.Varchar2).Value = BANKSTREET_IN;
                oCmd.Parameters.Add("BANKCITY_IN", OracleDbType.Varchar2).Value = BANKCITY_IN;
                oCmd.Parameters.Add("BANKBRANCH_IN", OracleDbType.Varchar2).Value = BANKBRANCH_IN;
                oCmd.Parameters.Add("BANKCURRENCY_IN", OracleDbType.Varchar2).Value = BANKCURRENCY_IN;
                oCmd.Parameters.Add("BANK_MANDATE_FILE_IN", OracleDbType.Varchar2).Value = BANK_MANDATE_FILE_IN;
                oCmd.Parameters.Add("BANK_CANCELED_FILE_IN", OracleDbType.Varchar2).Value = BANK_CANCELED_FILE_IN;
                oCmd.Parameters.Add("CTRY_IN", OracleDbType.Varchar2).Value = CTRY_IN;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID_IN;
                oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("HEADERID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("DETAILID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                {
                    detailid = oCmd.Parameters["DETAILID_OUT"].Value.ToString();
                    header_id = oCmd.Parameters["HEADERID_OUT"].Value.ToString();
                    return "1";
                }
                else
                    return oCmd.Parameters["ERROR_MSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public string SaveCINData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string CSTNO_IN,
            string LSTNO_IN, string INVOICINGDATES_IN, string INVOICINGLISTDATES_IN, string SERREGNO_IN, string PANNUMBER_IN, string PAYMENTGUARANTEEPROC_IN, string E_INVOICE_APPLICABLE_IN, string CIN_PAN_NO_FILE_IN, string CIN_GST_NO_FILE_IN, string CIN_CIN_DOC_FILE_IN, string EMAILID_IN, ref string header_id, ref string detailid)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAILCIN_ADD";
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = USERID_IN;
                oCmd.Parameters.Add("CMDETAILID_IN", OracleDbType.Int64).Value = CMDETAILID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Int64).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("CMREQUEST_TYPE_IN", OracleDbType.Varchar2).Value = CMREQUEST_TYPE_IN;
                oCmd.Parameters.Add("REQUEST_TYPE_IN", OracleDbType.Varchar2).Value = REQUEST_TYPE_IN;
                oCmd.Parameters.Add("COMPANY_CODE_IN", OracleDbType.Varchar2).Value = COMPANY_CODE_IN;
                oCmd.Parameters.Add("CUST_ACC_TYPE_IN", OracleDbType.Varchar2).Value = CUST_ACC_TYPE_IN;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = SALES_ORG_IN;
                oCmd.Parameters.Add("DIVISION_GRP_IN", OracleDbType.Varchar2).Value = DIVISION_GRP_IN;
                oCmd.Parameters.Add("DISTRIBTUION_CHH_IN", OracleDbType.Varchar2).Value = DISTRIBTUION_CHH_IN;
                oCmd.Parameters.Add("CSTNO_IN", OracleDbType.Varchar2).Value = CSTNO_IN;
                oCmd.Parameters.Add("LSTNO_IN", OracleDbType.Varchar2).Value = LSTNO_IN;
                oCmd.Parameters.Add("INVOICINGDATES_IN", OracleDbType.Varchar2).Value = INVOICINGDATES_IN;
                oCmd.Parameters.Add("INVOICINGLISTDATES_IN", OracleDbType.Varchar2).Value = INVOICINGLISTDATES_IN;
                oCmd.Parameters.Add("SERREGNO_IN", OracleDbType.Varchar2).Value = SERREGNO_IN;
                oCmd.Parameters.Add("PANNUMBER_IN", OracleDbType.Varchar2).Value = PANNUMBER_IN;
                oCmd.Parameters.Add("PAYMENTGUARANTEEPROC_IN", OracleDbType.Varchar2).Value = PAYMENTGUARANTEEPROC_IN;
                oCmd.Parameters.Add("E_INVOICE_APPLICABLE_IN", OracleDbType.Varchar2).Value = E_INVOICE_APPLICABLE_IN; // Added parameter
                oCmd.Parameters.Add("CIN_PAN_NO_FILE_IN", OracleDbType.Varchar2).Value = CIN_PAN_NO_FILE_IN;
                oCmd.Parameters.Add("CIN_GST_NO_FILE_IN", OracleDbType.Varchar2).Value = CIN_GST_NO_FILE_IN;
                oCmd.Parameters.Add("CIN_CIN_DOC_FILE_IN", OracleDbType.Varchar2).Value = CIN_CIN_DOC_FILE_IN; // Added parameter
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID_IN;
                oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("HEADERID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("DETAILID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                {
                    detailid = oCmd.Parameters["DETAILID_OUT"].Value.ToString();
                    header_id = oCmd.Parameters["HEADERID_OUT"].Value.ToString();
                    return "1";
                }
                else
                    return oCmd.Parameters["ERROR_MSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        public string SaveIndustryData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string INDUSTRY1_IN,
     string INDUSTRYCODE1_IN, string INDUSTRYNAME_IN, string FIRSTNAME_IN, string EMAILID_IN, ref string header_id, ref string detailid)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAILINDUSTRY_ADD";
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = USERID_IN;
                oCmd.Parameters.Add("CMDETAILID_IN", OracleDbType.Int64).Value = CMDETAILID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Int64).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("CMREQUEST_TYPE_IN", OracleDbType.Varchar2).Value = CMREQUEST_TYPE_IN;
                oCmd.Parameters.Add("REQUEST_TYPE_IN", OracleDbType.Varchar2).Value = REQUEST_TYPE_IN;
                oCmd.Parameters.Add("COMPANY_CODE_IN", OracleDbType.Varchar2).Value = COMPANY_CODE_IN;
                oCmd.Parameters.Add("CUST_ACC_TYPE_IN", OracleDbType.Varchar2).Value = CUST_ACC_TYPE_IN;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = SALES_ORG_IN;
                oCmd.Parameters.Add("DIVISION_GRP_IN", OracleDbType.Varchar2).Value = DIVISION_GRP_IN;
                oCmd.Parameters.Add("DISTRIBTUION_CHH_IN", OracleDbType.Varchar2).Value = DISTRIBTUION_CHH_IN;
                oCmd.Parameters.Add("INDUSTRY1_IN", OracleDbType.Varchar2, 10).Value = INDUSTRY1_IN;
                oCmd.Parameters.Add("INDUSTRYCODE1_IN", OracleDbType.Varchar2).Value = INDUSTRYCODE1_IN;
                oCmd.Parameters.Add("INDUSTRYNAME_IN", OracleDbType.Varchar2).Value = INDUSTRYNAME_IN;
                oCmd.Parameters.Add("FIRSTNAME_IN", OracleDbType.Varchar2).Value = FIRSTNAME_IN;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID_IN;
                oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("HEADERID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("DETAILID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                {
                    detailid = oCmd.Parameters["DETAILID_OUT"].Value.ToString();
                    header_id = oCmd.Parameters["HEADERID_OUT"].Value.ToString();
                    return "1";
                }
                else
                    return oCmd.Parameters["ERROR_MSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public string SaveSalesData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string SALESCURRENCY_IN,
     string EXCHANGERATETYPE_IN, string SALESDISTRICT_IN, string SALESOFFICE_IN, string SALESGROUP_IN, string CUSTOMERGROUP_IN, string PRICEGROUP_IN, string CUSTPRICPROC1_IN,
     string DELIVERYPRIORITYGROUP_IN, string SHIPPINGCONDITION_IN, string EMAILID_IN, ref string header_id, ref string detailid)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAILSALES_ADD";
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = USERID_IN;
                oCmd.Parameters.Add("CMDETAILID_IN", OracleDbType.Int64).Value = CMDETAILID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Int64).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("CMREQUEST_TYPE_IN", OracleDbType.Varchar2).Value = CMREQUEST_TYPE_IN;
                oCmd.Parameters.Add("REQUEST_TYPE_IN", OracleDbType.Varchar2).Value = REQUEST_TYPE_IN;
                oCmd.Parameters.Add("COMPANY_CODE_IN", OracleDbType.Varchar2).Value = COMPANY_CODE_IN;
                oCmd.Parameters.Add("CUST_ACC_TYPE_IN", OracleDbType.Varchar2).Value = CUST_ACC_TYPE_IN;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = SALES_ORG_IN;
                oCmd.Parameters.Add("DIVISION_GRP_IN", OracleDbType.Varchar2).Value = DIVISION_GRP_IN;
                oCmd.Parameters.Add("DISTRIBTUION_CHH_IN", OracleDbType.Varchar2).Value = DISTRIBTUION_CHH_IN;
                oCmd.Parameters.Add("SALESCURRENCY_IN", OracleDbType.Varchar2, 10).Value = SALESCURRENCY_IN;
                oCmd.Parameters.Add("EXCHANGERATETYPE_IN", OracleDbType.Varchar2).Value = EXCHANGERATETYPE_IN;
                oCmd.Parameters.Add("SALESDISTRICT_IN", OracleDbType.Varchar2).Value = SALESDISTRICT_IN;
                oCmd.Parameters.Add("SALESOFFICE_IN", OracleDbType.Varchar2).Value = SALESOFFICE_IN;
                oCmd.Parameters.Add("SALESGROUP_IN", OracleDbType.Varchar2).Value = SALESGROUP_IN;
                oCmd.Parameters.Add("CUSTOMERGROUP_IN", OracleDbType.Varchar2).Value = CUSTOMERGROUP_IN;
                oCmd.Parameters.Add("PRICEGROUP_IN", OracleDbType.Varchar2).Value = PRICEGROUP_IN;
                oCmd.Parameters.Add("CUSTPRICPROC1_IN", OracleDbType.Varchar2).Value = CUSTPRICPROC1_IN;
                oCmd.Parameters.Add("DELIVERYPRIORITYGROUP_IN", OracleDbType.Varchar2).Value = DELIVERYPRIORITYGROUP_IN;
                oCmd.Parameters.Add("SHIPPINGCONDITION_IN", OracleDbType.Varchar2).Value = SHIPPINGCONDITION_IN;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID_IN;
                oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("HEADERID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("DETAILID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                {
                    detailid = oCmd.Parameters["DETAILID_OUT"].Value.ToString();
                    header_id = oCmd.Parameters["HEADERID_OUT"].Value.ToString();
                    return "1";
                }
                else
                    return oCmd.Parameters["ERROR_MSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        public string SaveCompanyData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string RECONSACCOUNT_IN,
    string TERMOFPAYMENT_IN, string PAYMENTMETHODS_IN, string HOUSEBANK_IN, string PMTMETHSUPL_IN, string WITHHOLDINGTAXTYPE_IN, string WITHHOLDINGTAXCODE_IN, string VALIDFROM_IN,
    string VALIDTO_IN, string WITHHOLDINGTAXNUMBER_IN, string INCOTERM1_IN, string INCOTERM2_IN, string CREDITCONTROLAREA_IN, string ACCASSIGMENTGROUP_IN, string TAXCLASSIFICATION_IN, string COMP_RTO_FILE_IN, string COMP_LOI_FILE_IN, string EMAILID_IN, string OTHER_DOC_FILE_IN, ref string header_id, ref string detailid)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAILCOMP_ADD";
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = USERID_IN;
                oCmd.Parameters.Add("CMDETAILID_IN", OracleDbType.Int64).Value = CMDETAILID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Int64).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("CMREQUEST_TYPE_IN", OracleDbType.Varchar2).Value = CMREQUEST_TYPE_IN;
                oCmd.Parameters.Add("REQUEST_TYPE_IN", OracleDbType.Varchar2).Value = REQUEST_TYPE_IN;
                oCmd.Parameters.Add("COMPANY_CODE_IN", OracleDbType.Varchar2).Value = COMPANY_CODE_IN;
                oCmd.Parameters.Add("CUST_ACC_TYPE_IN", OracleDbType.Varchar2).Value = CUST_ACC_TYPE_IN;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = SALES_ORG_IN;
                oCmd.Parameters.Add("DIVISION_GRP_IN", OracleDbType.Varchar2).Value = DIVISION_GRP_IN;
                oCmd.Parameters.Add("DISTRIBTUION_CHH_IN", OracleDbType.Varchar2).Value = DISTRIBTUION_CHH_IN;
                oCmd.Parameters.Add("RECONSACCOUNT_IN", OracleDbType.Varchar2).Value = RECONSACCOUNT_IN;
                oCmd.Parameters.Add("TERMOFPAYMENT_IN", OracleDbType.Varchar2).Value = TERMOFPAYMENT_IN;
                oCmd.Parameters.Add("PAYMENTMETHODS_IN", OracleDbType.Varchar2).Value = PAYMENTMETHODS_IN;
                oCmd.Parameters.Add("HOUSEBANK_IN", OracleDbType.Varchar2).Value = HOUSEBANK_IN;
                oCmd.Parameters.Add("PMTMETHSUPL_IN", OracleDbType.Varchar2).Value = PMTMETHSUPL_IN;
                oCmd.Parameters.Add("WITHHOLDINGTAXTYPE_IN", OracleDbType.Varchar2).Value = WITHHOLDINGTAXTYPE_IN;
                oCmd.Parameters.Add("WITHHOLDINGTAXCODE_IN", OracleDbType.Varchar2).Value = WITHHOLDINGTAXCODE_IN;
                oCmd.Parameters.Add("VALIDFROM_IN", OracleDbType.Varchar2).Value = VALIDFROM_IN;
                oCmd.Parameters.Add("VALIDTO_IN", OracleDbType.Varchar2).Value = VALIDTO_IN;
                oCmd.Parameters.Add("WITHHOLDINGTAXNUMBER_IN", OracleDbType.Varchar2).Value = WITHHOLDINGTAXNUMBER_IN;
                oCmd.Parameters.Add("INCOTERM1_IN", OracleDbType.Varchar2).Value = INCOTERM1_IN;
                oCmd.Parameters.Add("INCOTERM2_IN", OracleDbType.Varchar2).Value = INCOTERM2_IN;
                oCmd.Parameters.Add("CREDITCONTROLAREA_IN", OracleDbType.Varchar2).Value = CREDITCONTROLAREA_IN;
                oCmd.Parameters.Add("ACCASSIGMENTGROUP_IN", OracleDbType.Varchar2).Value = ACCASSIGMENTGROUP_IN;
                oCmd.Parameters.Add("TAXCLASSIFICATION_IN", OracleDbType.Varchar2).Value = TAXCLASSIFICATION_IN;
                oCmd.Parameters.Add("COMP_RTO_FILE_IN", OracleDbType.Varchar2).Value = COMP_RTO_FILE_IN;
                oCmd.Parameters.Add("COMP_LOI_FILE_IN", OracleDbType.Varchar2).Value = COMP_LOI_FILE_IN;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID_IN;
                oCmd.Parameters.Add("OTHER_DOC_FILE_IN", OracleDbType.Varchar2).Value = OTHER_DOC_FILE_IN;
                oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("HEADERID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("DETAILID_OUT", OracleDbType.Int64, 10).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                {
                    detailid = oCmd.Parameters["DETAILID_OUT"].Value.ToString();
                    header_id = oCmd.Parameters["HEADERID_OUT"].Value.ToString();
                    return "1";
                }
                else
                    return oCmd.Parameters["ERROR_MSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public string SubmitCMRequest(long EMPCODE_IN, long CMHEADERID_IN, long APPROVAL_IN, long DEPT_DIV_IN, string REMARKS, ref string req_no)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAIL_SUBMIT";
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = EMPCODE_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Int64).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("APPROVAL_IN", OracleDbType.Int64).Value = APPROVAL_IN;
                oCmd.Parameters.Add("DEPT_DIV_IN", OracleDbType.Int64).Value = DEPT_DIV_IN;
                oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = REMARKS;
                oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() != "0")
                {
                    req_no = oCmd.Parameters["RESULT_OUT"].Value.ToString();
                    return "1";
                }
                else
                    return oCmd.Parameters["ERROR_MSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        public DataSet GetCMDetailDraft(long _userId, string GEN_REQUEST_NO_IN, ref string err_msg)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAILDRAFT_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = _userId;
            oCmd.Parameters.Add("GEN_REQUEST_NO_IN", OracleDbType.Varchar2).Value = GEN_REQUEST_NO_IN;
            oCmd.Parameters.Add("CUR_CMDETAILDRAFT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_CMCOMMON", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ER_MSG", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        public string ResetCMRequest(long EMPCODE_IN, string REQUEST_TYPE, ref string req_no)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCMDETAIL_RESET";
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = EMPCODE_IN;
                oCmd.Parameters.Add("REQUEST_TYPE", OracleDbType.Varchar2).Value = REQUEST_TYPE;
                oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() != "0")
                {
                    req_no = oCmd.Parameters["RESULT_OUT"].Value.ToString();
                    return "1";
                }
                else
                    return oCmd.Parameters["ERROR_MSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        //***************VarunCode******************************* //
        //public DataSet GetCutomerFieldDataList()
        //{
        //    OracleCommand oCmd = new OracleCommand();
        //    ds = new DataSet();
        //    oCmd.CommandType = CommandType.StoredProcedure;
        //    oCmd.CommandText = "PKG_CUSTOMERMASTER.GETMASTERMGMTDATA";
        //    oCmd.Parameters.Add("CUR_MASTERDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //    oCmd.Parameters.Add("ERROR_MSG_OUT", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
        //    //Get data from data access layer
        //    ds = oDataMgmt.GetDataSet(oCmd);
        //    return (ds);
        //}
        public string AddCustomerMasterData(string code, string codeDesc, string groupName, string pgroupName, string createdBy, string isActive)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();

                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CMMasterData_ADD";
                // Input parameters
                oCmd.Parameters.Add("CODE_IN", OracleDbType.Varchar2).Value = code;
                oCmd.Parameters.Add("CODE_DESC_IN", OracleDbType.Varchar2).Value = codeDesc;
                oCmd.Parameters.Add("GROUP_NAME_IN", OracleDbType.Varchar2).Value = groupName;
                oCmd.Parameters.Add("PGROUP_NAME_IN", OracleDbType.Varchar2).Value = pgroupName;
                oCmd.Parameters.Add("CREATED_BY_IN", OracleDbType.Varchar2).Value = createdBy;
                oCmd.Parameters.Add("IS_ACTIVE_IN", OracleDbType.Varchar2).Value = isActive;

                // Output parameters
                // Code Start CR6411 SR99445 code changed by TTL 20/05/2025
                // oCmd.Parameters.Add("CMD_ID_OUT", OracleDbType.Varchar2, ParameterDirection.Output);
                oCmd.Parameters.Add("CMD_ID_OUT", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                // Code End CR6411 SR99445 code changed by TTL 20/05/2025
                oCmd.Parameters.Add("ERR_MSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;

                // Execute the stored procedure
                oDataMgmt.ExecuteQuery(oCmd);

                // Retrieve output values
                string cmdId = oCmd.Parameters["CMD_ID_OUT"].Value.ToString();
                string errMsg = oCmd.Parameters["ERR_MSG_OUT"].Value.ToString();


                DataSet resultDataSet = new DataSet();


                return errMsg;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return null;
            }
        }

        // Method to fetch data based on selected group name
        public DataSet FetchCodesByGroupName(string groupName)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();

                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.GetCodesByGroupName";
                // Input parameter
                oCmd.Parameters.Add("GROUP_NAME_IN", OracleDbType.Varchar2).Value = groupName;
                // Output parameter
                oCmd.Parameters.Add("CODES_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERR_MSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;

                DataTable dataTable = oDataMgmt.GetDataTable(oCmd);

                DataSet resultDataSet = new DataSet();
                resultDataSet.Tables.Add(dataTable);
                return resultDataSet;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return null;
            }
        }
        public string UpdateCustomerMasterData(string cmdID, string newcode, string newCodeDesc, string GroupName, string PGroupName, string updatedBy, string newIsActive)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();

                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CMMasterData_Update";
                // Input parameters
                oCmd.Parameters.Add("CMD_ID_IN", OracleDbType.Varchar2).Value = cmdID;
                oCmd.Parameters.Add("CODE_IN", OracleDbType.Varchar2).Value = newcode;
                oCmd.Parameters.Add("CODE_DESC_IN", OracleDbType.Varchar2).Value = newCodeDesc;
                oCmd.Parameters.Add("GROUP_NAME_IN", OracleDbType.Varchar2).Value = GroupName;
                oCmd.Parameters.Add("PGROUP_NAME_IN", OracleDbType.Varchar2).Value = PGroupName;
                oCmd.Parameters.Add("UPDATED_BY_IN", OracleDbType.Varchar2).Value = updatedBy;
                oCmd.Parameters.Add("IS_ACTIVE_IN", OracleDbType.Varchar2).Value = newIsActive;
                // Output parameters
                oCmd.Parameters.Add("ERR_MSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                string errMsg = oCmd.Parameters["ERR_MSG_OUT"].Value.ToString();
                if (!string.IsNullOrEmpty(errMsg))
                {
                    return errMsg;
                }
                else
                {
                    return "DB error message!";
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("An error occurred: " + ex.Message);
                return "An error occurred: " + ex.Message;
            }
        }

        public string CustomerApprovalAuthority_Add(string _authType_Id, int _authEmp_Id, string _plantId, int _masterTypeId, int user_Id, int ActiveAuthority)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ods = new DataSet();
                
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CM_APPROVALMATRIX_ADD";
                oCmd.Parameters.Add("VMASTERTYPEID", OracleDbType.Int64).Value = _masterTypeId;
                oCmd.Parameters.Add("VREQUEST_TYPE", OracleDbType.Varchar2).Value = _plantId;
                oCmd.Parameters.Add("VEMPCODE", OracleDbType.Int64).Value = _authEmp_Id;
                oCmd.Parameters.Add("VAUTHORIZATIONTYPE", OracleDbType.Varchar2).Value = _authType_Id;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = user_Id;
                oCmd.Parameters.Add("VMMAUTHORITYID_IN", OracleDbType.Int64).Value = ActiveAuthority;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        public string CustomerApprovalAuthority_Update(int VMASTERDATAAPPROVALID, string Request_type, int Authorization_type, int user_Id, int ActiveAuthority)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ods = new DataSet();
                
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CM_APPROVALMATRIX_UPDATE";
                oCmd.Parameters.Add("VMASTERDATAAPPROVALID", OracleDbType.Int64).Value = VMASTERDATAAPPROVALID;
                oCmd.Parameters.Add("VREQUEST_TYPE", OracleDbType.Varchar2).Value = Request_type;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = user_Id;
                oCmd.Parameters.Add("VMMAUTHORITYID_IN", OracleDbType.Varchar2).Value = Authorization_type;
                oCmd.Parameters.Add("VACTIVE", OracleDbType.Int64).Value = ActiveAuthority;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        public DataSet GetApprovalMatrixList(string _AuthType)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_CUSTOMERMASTER.CM_APPROVALMATRIX_GET";
            oCmd.Parameters.Add("CUR_MMAPPROVALDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VAUTHORIZATIONTYPE", OracleDbType.Varchar2).Value = _AuthType;
            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        public DataSet GetEmpNameMatrixList(string emp_Code)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                ds = new DataSet();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.EMP_NAMESEARCH";
                oCmd.Parameters.Add("CUR_EMPNAMESEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = emp_Code;

                //GET DATA FROM DATA ACCESS LAYER
                ds = oDataMgmt.GetDataSet(oCmd);
                return (ds);
            }
            catch
            {
                throw;
            }
        }
        public string MaterialSISUserAuthority_Add(int _sisauthEmp_Id, string _sisEmp_Name, int user_Id)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ods = new DataSet();
                
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CM_CMSISUSERMATRIX_ADD";
                oCmd.Parameters.Add("VEMPCODE", OracleDbType.Int64).Value = _sisauthEmp_Id;
                oCmd.Parameters.Add("VEMPNAME", OracleDbType.Varchar2, 500).Value = _sisEmp_Name;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = user_Id;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        public DataSet GETCUSTOMERREQUEST(string UID, string STATUS, string CUSTOMERHEADERID, string REQUESTTYPE, string CUSTOMERACCGROUP, string CUSTOMERCODE, string REQDATEFROM, string REQDATETO, string CUSTOMERNAME, string REQUEST_NO, string USER_ACCESS_TYPE_IN)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CUSTOMER_REQUEST_GET";
                oCmd.Parameters.Add("UID_IN", OracleDbType.Varchar2).Value = UID;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Varchar2).Value = CUSTOMERHEADERID;
                oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
                oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
                oCmd.Parameters.Add("CUSTOMERACCGROUP_IN", OracleDbType.Varchar2).Value = CUSTOMERACCGROUP;
                oCmd.Parameters.Add("CUSTOMERCODE_IN", OracleDbType.Varchar2).Value = CUSTOMERCODE;
                oCmd.Parameters.Add("REQDATEFROM_IN", OracleDbType.Varchar2).Value = REQDATEFROM;
                oCmd.Parameters.Add("REQDATETO_IN", OracleDbType.Varchar2).Value = REQDATETO;
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("CUSTOMERNAME", OracleDbType.Varchar2).Value = CUSTOMERNAME;
                oCmd.Parameters.Add("REQUEST_NO", OracleDbType.Varchar2).Value = REQUEST_NO;
                oCmd.Parameters.Add("USER_ACCESS_TYPE_IN", OracleDbType.Varchar2).Value = USER_ACCESS_TYPE_IN;
                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);
                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }
        public DataSet GetCustomerDetails(string UID_IN, string GEN_REQUEST_NO, long CMDETAILID_IN)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CUSTOMER_VIEW_GET";
                oCmd.Parameters.Add("UID_IN", OracleDbType.Varchar2).Value = UID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Varchar2).Value = GEN_REQUEST_NO;
                oCmd.Parameters.Add("CMDETAILID_IN", OracleDbType.Int64).Value = CMDETAILID_IN;
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);

                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }
        public DataSet GetApprovalDetails(string UID_IN, string GEN_REQUEST_NO)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCAPPROVALDETAIL_GET";
                oCmd.Parameters.Add("UID_IN", OracleDbType.Varchar2).Value = UID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Varchar2).Value = GEN_REQUEST_NO;
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("CUR_REQ_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);

                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }
        public DataSet GetApprovalRequestDetails(long UID_IN)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CMRAPPROVALREQUEST_GET";
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = UID_IN;
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);

                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }
        public string ApprovalRequest(long USERID, long CMHEADERID_IN, string REQUEST_NO, int ACTION_IN, string REMARKS)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROCUSTOMERAPPROVAL_SUBMIT";
                oCmd.Parameters.Add("USERID", OracleDbType.Int64).Value = USERID;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Int64).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("REQUEST_NO", OracleDbType.Varchar2).Value = REQUEST_NO;
                oCmd.Parameters.Add("ACTION_IN", OracleDbType.Int32).Value = ACTION_IN;
                oCmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = REMARKS;
                oCmd.Parameters.Add("RESULT_IN", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;

                oDataMgmt.ExecuteQuery(oCmd);

                if (oCmd.Parameters["RESULT_IN"].Value.ToString() != "0")
                {
                    return "1";
                }
                else
                    return oCmd.Parameters["ERR_MSG"].Value.ToString();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }

        //*********************************Data Sync********************************************************//
        public DataSet GETCUSTOMERREQUESTDataSync(string UID, string STATUS, string CUSTOMERHEADERID, string REQUESTTYPE, string CUSTOMERACCGROUP, string CUSTOMERCODE, string REQDATEFROM, string REQDATETO)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CUSTOMER_DATADYNC";
                oCmd.Parameters.Add("UID_IN", OracleDbType.Varchar2).Value = UID;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Varchar2).Value = CUSTOMERHEADERID;
                oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
                oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
                oCmd.Parameters.Add("CUSTOMERACCGROUP_IN", OracleDbType.Varchar2).Value = CUSTOMERACCGROUP;
                oCmd.Parameters.Add("CUSTOMERCODE_IN", OracleDbType.Varchar2).Value = CUSTOMERCODE;
                oCmd.Parameters.Add("REQDATEFROM_IN", OracleDbType.Varchar2).Value = REQDATEFROM;
                oCmd.Parameters.Add("REQDATETO_IN", OracleDbType.Varchar2).Value = REQDATETO;
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);
                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }
        public DataSet GetDataForSAPSYNC(string UID_IN, string CMHEADERID_IN, string CMDETAILID_IN)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.DOWNLOAD_GET";

                // Input parameters
                oCmd.Parameters.Add("UID_IN", OracleDbType.Varchar2).Value = UID_IN;
                oCmd.Parameters.Add("CMHEADERID_IN", OracleDbType.Varchar2).Value = CMHEADERID_IN;
                oCmd.Parameters.Add("CMDETAILID_IN", OracleDbType.Varchar2).Value = CMDETAILID_IN;

                // Output parameters
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2, ParameterDirection.Output);


                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);

                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return null;
            }
        }
        public string InsertIntoSAPRFCLOG(string userid, string uid, string kunnr, string spart, string zStatus, string value)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CMRFCSYNCLOG";

                // Input parameters
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = userid;
                oCmd.Parameters.Add("UID_IN", OracleDbType.Varchar2).Value = uid;
                oCmd.Parameters.Add("KUNNR_IN", OracleDbType.Varchar2).Value = kunnr;
                oCmd.Parameters.Add("SPART_IN", OracleDbType.Varchar2).Value = spart;
                oCmd.Parameters.Add("ZSTATUS_IN", OracleDbType.Varchar2).Value = zStatus;
                oCmd.Parameters.Add("VALUE_IN", OracleDbType.Varchar2).Value = value;
                // Output parameters
                oCmd.Parameters.Add("ERR_MSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERR_MSG_OUT"].Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
        }

        public string AddMandentaryData(string FIELDNAME_IN, string SERVICETYPE_IN, string AG_5AG1_IN, string AG_5AG3_IN, string AG_5AGV_IN, string AG_5AGG_IN, string AG_5AG6_IN, string AG_5AGW_IN, string AG_5AG2_IN, string AG_5AG5_IN, string GROUP_NAME_IN, string CLIENTIDLABEL_IN, string ClIENTIDTEXTLABEL_IN, string IS_ACTION_IN, string CREATEDBY_IN)
        {
            string rs = "";
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROMASTERFIELD_ADD";

                // Input parameters
                oCmd.Parameters.Add("FIELDNAME_IN", OracleDbType.Varchar2).Value = FIELDNAME_IN;
                oCmd.Parameters.Add("SERVICETYPE_IN", OracleDbType.Varchar2).Value = SERVICETYPE_IN;
                oCmd.Parameters.Add("AG_5AG1_IN", OracleDbType.Varchar2).Value = AG_5AG1_IN;
                oCmd.Parameters.Add("AG_5AG3_IN", OracleDbType.Varchar2).Value = AG_5AG3_IN;
                oCmd.Parameters.Add("AG_5AGV_IN", OracleDbType.Varchar2).Value = AG_5AGV_IN;
                oCmd.Parameters.Add("AG_5AGG_IN", OracleDbType.Varchar2).Value = AG_5AGG_IN;
                oCmd.Parameters.Add("AG_5AG6_IN", OracleDbType.Varchar2).Value = AG_5AG6_IN;
                oCmd.Parameters.Add("AG_5AGW_IN", OracleDbType.Varchar2).Value = AG_5AGW_IN;
                oCmd.Parameters.Add("AG_5AG2_IN", OracleDbType.Varchar2).Value = AG_5AG2_IN;
                oCmd.Parameters.Add("AG_5AG5_IN", OracleDbType.Varchar2).Value = AG_5AG5_IN;
                oCmd.Parameters.Add("GROUP_NAME_IN", OracleDbType.Varchar2).Value = GROUP_NAME_IN;
                oCmd.Parameters.Add("CLIENTIDLABEL_IN", OracleDbType.Varchar2).Value = CLIENTIDLABEL_IN;
                oCmd.Parameters.Add("ClIENTIDTEXTLABEL_IN", OracleDbType.Varchar2).Value = ClIENTIDTEXTLABEL_IN;
                oCmd.Parameters.Add("IS_ACTION_IN", OracleDbType.Varchar2).Value = IS_ACTION_IN;
                oCmd.Parameters.Add("CREATEDBY_IN", OracleDbType.Varchar2).Value = CREATEDBY_IN;

                // Output parameters
                oCmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERR_MSG"].Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
            return rs;
        }
        public string UpdateMandentaryData(int FIELDID_IN, string FIELDNAME_IN, string SERVICETYPE_IN, string AG_5AG1_IN, string AG_5AG3_IN, string AG_5AGV_IN, string AG_5AGG_IN, string AG_5AG6_IN, string AG_5AGW_IN, string AG_5AG2_IN, string AG_5AG5_IN, string GROUP_NAME_IN, string CLIENTIDLABEL_IN, string ClIENTIDTEXTLABEL_IN, string IS_ACTION_IN, string CREATEDBY_IN)
        {
            string rs = "";
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.PROMASTERFIELD_UPDATE";

                // Input parameters
                oCmd.Parameters.Add("FIELDID_IN", OracleDbType.Int32).Value = FIELDID_IN;
                oCmd.Parameters.Add("FIELDNAME_IN", OracleDbType.Varchar2).Value = FIELDNAME_IN;
                oCmd.Parameters.Add("SERVICETYPE_IN", OracleDbType.Varchar2).Value = SERVICETYPE_IN;
                oCmd.Parameters.Add("AG_5AG1_IN", OracleDbType.Varchar2).Value = AG_5AG1_IN;
                oCmd.Parameters.Add("AG_5AG3_IN", OracleDbType.Varchar2).Value = AG_5AG3_IN;
                oCmd.Parameters.Add("AG_5AGV_IN", OracleDbType.Varchar2).Value = AG_5AGV_IN;
                oCmd.Parameters.Add("AG_5AGG_IN", OracleDbType.Varchar2).Value = AG_5AGG_IN;
                oCmd.Parameters.Add("AG_5AG6_IN", OracleDbType.Varchar2).Value = AG_5AG6_IN;
                oCmd.Parameters.Add("AG_5AGW_IN", OracleDbType.Varchar2).Value = AG_5AGW_IN;
                oCmd.Parameters.Add("AG_5AG2_IN", OracleDbType.Varchar2).Value = AG_5AG2_IN;
                oCmd.Parameters.Add("AG_5AG5_IN", OracleDbType.Varchar2).Value = AG_5AG5_IN;
                oCmd.Parameters.Add("GROUP_NAME_IN", OracleDbType.Varchar2).Value = GROUP_NAME_IN;
                oCmd.Parameters.Add("CLIENTIDLABEL_IN", OracleDbType.Varchar2).Value = CLIENTIDLABEL_IN;
                oCmd.Parameters.Add("ClIENTIDTEXTLABEL_IN", OracleDbType.Varchar2).Value = ClIENTIDTEXTLABEL_IN;
                oCmd.Parameters.Add("IS_ACTION_IN", OracleDbType.Varchar2).Value = IS_ACTION_IN;
                oCmd.Parameters.Add("CREATEDBY_IN", OracleDbType.Varchar2).Value = CREATEDBY_IN;

                // Output parameters
                oCmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERR_MSG"].Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
            return rs;
        }

        public DataSet GetCutomerRequest(long UID_IN, string REQ_TYPE)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CUSTOMERREQ_GET";
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = UID_IN;
                oCmd.Parameters.Add("REQ_TYPE", OracleDbType.Varchar2).Value = REQ_TYPE;
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);               
                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }

        public DataSet GetCustomerApprovalDetails(string GEN_REQUEST_NO)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CUST_APPROVAL_GET";
                oCmd.Parameters.Add("REQ_NO", OracleDbType.Varchar2,20).Value = GEN_REQUEST_NO;
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RES_ERROR", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);               
                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }
        public string UpdateApproval(string CUR_REQ_NO, int APROVER_ROLE, int PRE_APROVER, int NEW_APROVER, int USER_ID, string REMARKS_IN, string ATTACH_DOC)
        {
            string rs = "";
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.SWITCH_APPROVAL";

                // Input parameters
                oCmd.Parameters.Add("CUR_REQ_NO", OracleDbType.Varchar2).Value = CUR_REQ_NO;
                oCmd.Parameters.Add("APROVER_ROLE", OracleDbType.Int32).Value = APROVER_ROLE;
                oCmd.Parameters.Add("PRE_APROVER", OracleDbType.Int32).Value = PRE_APROVER;
                oCmd.Parameters.Add("NEW_APROVER", OracleDbType.Int32).Value = NEW_APROVER;
                oCmd.Parameters.Add("USER_ID", OracleDbType.Int32).Value = USER_ID;
                oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = REMARKS_IN;
                oCmd.Parameters.Add("ATTACH_DOC", OracleDbType.Varchar2).Value = ATTACH_DOC;
                // Output parameters
                oCmd.Parameters.Add("RES_ERROR", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                oDataMgmt.ExecuteQuery(oCmd);
                return oCmd.Parameters["RES_ERROR"].Value.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }
            return rs;

        }

        public DataSet GetSwitchApprovalData(int user_id)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CUST_APPROVAL_SWITCHED_GET";
                oCmd.Parameters.Add("USER_ID", OracleDbType.Int32).Value = user_id;
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RES_ERROR", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);
                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }

        public DataSet GetEMAILData(long user_id, string REQNO)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_CUSTOMERMASTER.CUST_REQEMAIL_GET";
                oCmd.Parameters.Add("USER_ID", OracleDbType.Int64).Value = user_id;
                oCmd.Parameters.Add("RREQNO", OracleDbType.Varchar2).Value = REQNO;
                oCmd.Parameters.Add("CUR_REQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RES_ERROR", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                DataSet dataSet = oDataMgmt.GetDataSet(oCmd);
                oCmd.Dispose();
                return dataSet;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("An error occurred: " + ex.Message);
                return null; // Or handle the error in an appropriate manner for your application
            }
        }

        public void SENDMAILTODEPTHEAD(long USERID, string req_no, string strSubject, string action, string customercode, string customername)
        {
            // long USERID = Convert.ToInt32(((Employee_Details)Session["Employee"]).Employee_Code);
            DataSet ds = GetEMAILData(USERID, req_no);
            if (ds.Tables[0] != null)
            {
                string toemail = ds.Tables[0].Rows[0]["TOMAIL"].ToString();
                string ccemail = ds.Tables[0].Rows[0]["CCMAIL"].ToString();
                string NAME = ds.Tables[0].Rows[0]["NAME"].ToString();
                string REQUEST_TYPE = ds.Tables[0].Rows[0]["REQUEST_TYPE"].ToString() == "U" ? "Extend/Update" : "Create";
                String REQUESTOR = ds.Tables[0].Rows[0]["REQUESTOR"].ToString();
                String REQ_LVEL = ds.Tables[0].Rows[0]["REQ_LVEL"].ToString();

                if (!string.IsNullOrEmpty(toemail))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda2wheelersindia.com";
                    sendMail.MailTo = toemail;
                    sendMail.MailCc = ccemail;
                    string strBody = "";
                    if (REQ_LVEL == "FINAL")
                    {
                        strSubject = "Pending for the Customer Master Data Sync";
                        strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;padding-top:0px;font-family:Times New Roman;font-size:14px;line-height:21px;'><div style='width:100%;height:25px;background-color:skyblue;'><b>Customer Master Request</b></div>"
                                   + "<table cellpadding=0 cellspacing=0 border=0  style='font-size:14px;width:100%; margin-top:10px' >"
                                   + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + NAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                   + "<tr><td colspan=2>&nbsp;&nbsp;Customer master request has been approved and pending at your end for daya sync. Following are the details:</td></tr><tr><td colspan=2>&nbsp;</td></tr><tr><td colspan=2>&nbsp;</td></tr></table>";
                    }
                    else if (REQ_LVEL == "BACK" && action == "B")
                    {
                        strSubject = "Sent back the Customer Master Request";

                        strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;padding-top:0px;font-family:Times New Roman;font-size:14px;line-height:21px;'><div style='width:100%;height:25px;background-color:skyblue;'><b>Customer Master Request</b></div>"
                                   + "<table cellpadding=0 cellspacing=0 border=0  style='font-size:14px;width:100%; margin-top:10px' >"
                                   + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + NAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                   + "<tr><td colspan=2>&nbsp;&nbsp;Customer master request has been backed and pending at your end for resubmit. Following are the details:</td></tr><tr><td colspan=2>&nbsp;</td></tr><tr><td colspan=2>&nbsp;</td></tr></table>";
                    }
                    else if (REQ_LVEL == "BACK" && action == "R")
                    {
                        strSubject = "Rejected the Customer Master Request";
                        strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;padding-top:0px;font-family:Times New Roman;font-size:14px;line-height:21px;'><div style='width:100%;height:25px;background-color:skyblue;'><b>Customer Master Request</b></div>"
                                   + "<table cellpadding=0 cellspacing=0 border=0  style='font-size:14px;width:100%; margin-top:10px' >"
                                   + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + NAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                   + "<tr><td colspan=2>&nbsp;&nbsp;Customer master request has been rejected. Following are the details:</td></tr><tr><td colspan=2>&nbsp;</td></tr><tr><td colspan=2>&nbsp;</td></tr></table>";
                    }
                    else
                    {
                        strSubject = "Pending for the Customer Master Approval";
                        strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;padding-top:0px;font-family:Times New Roman;font-size:14px;line-height:21px;'><div style='width:100%;height:25px;background-color:skyblue;'><b>Customer Master Request</b></div>"
                                   + "<table cellpadding=0 cellspacing=0 border=0  style='font-size:14px;width:100%; margin-top:10px' >"
                                   + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + NAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                   + "<tr><td colspan=2>&nbsp;&nbsp;Customer master request is pending at your end for approval. Following are the details:</td></tr><tr><td colspan=2>&nbsp;</td></tr><tr><td colspan=2>&nbsp;</td></tr></table>";
                    }
                    //string strSubject = "Pending for the Customer Master Approval";


                    strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                               + "<tr><td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Requestor</td>"
                    + "<td width='200' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;" + REQUESTOR + "</td>"
                    + "<td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Request Date</td>"
                    + "<td width='300' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;'>&nbsp;" + DateTime.Now.ToString("dd-MMM-yyyy") + "</td></tr>"
                    + "<tr><td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Request No</td>"
                    + "<td width='200' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;" + req_no + "</td>"
                    + "<td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Request Type</td>"
                    + "<td width='300' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;'>&nbsp;" + REQUEST_TYPE + "</td></tr>"

                    + "<tr><td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Dealer/Customer Code</td>"
                    + "<td width='200' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;" + customercode + "</td>"
                    + "<td width='100' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;' >&nbsp;Name</td>"
                    + "<td width='300' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:12px;font-weight:100;letter-spacing:1px;line-height:22px;text-align:left;font-family:Times New Roman;'>&nbsp;" + customername + "</td></tr>"
                    + "</table>";

                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 style='font-size:14px;width:100%'><tr><td><br/>&nbsp;&nbsp;Please login<a href=" + serverpath.getServerPath() + "home> E-Portal</a> for approval process." +
                    "</td></tr><tr><td><br/><b>&nbsp;&nbsp;Thank You</b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Portal Admin<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "<tr><td>&nbsp;</td></tr></table>" +
                    "</div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
            }


        }


    }
}
