using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.ViewModels.APPX.CustomerMgmt;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
     public interface ICustomerMgmtRepo
    {

         
         DataSet GetCutomerFieldDataList();
         DataSet GetCustomerMasterData(string Group_Name);
         DataSet GetCustomerMasterDataWithPGRP(string Group_Name, string PGroup_Name);
         DataSet GetCustomerMandatoryData(string ag5AG1, string ag5AG3, string ag5AGV, string ag5AGG, string ag5AG6, string ag5AGW, string ag5AG2, string ag5AG5, string group_name);

         DataSet GetSectionHead(int _userId);
         DataSet GetDepartmentHead(int _userId);
         string SaveData(string GROUP_NAME_IN, long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CUST_ACC_TYPE_IN,
    string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string CUSTOMER_CODE_IN, string NAME1_IN,
    string NAME2_IN, string NAME3_IN, string NAME4_IN, string SEARCHTERM_IN, string STREETHOUSENUMBER_IN, string STREET2_IN, string STREET3_IN,
    string STREET4_IN, string STREET5_IN, string POSTALCODE_IN, string CITY_IN, string COUNTRY_IN, string CREGION_IN, string CTIMEZONE_IN, string TRANSPORTATIONCODE_IN,
    string MOBILEPHONE_IN, string FAX_IN, string EMAIL_IN, string INDUSTRY_IN, string TAXNUMBER3_IN, string CITYCODE_IN, string CTRY_IN, string BANKKEY_IN, string BANKACCOUNT_IN,
    string ACCOUNTHOLDER_IN, string BANKCONTROLKEY_IN, string BANKNAME_IN, string BANKREGION_IN, string BANKSTREET_IN, string BANKCITY_IN, string BANKBRANCH_IN, string CUSTOMERCLASS_IN
    , string INDUSTRYCODE1_IN, string FIRSTNAME_IN, string INDUSTRY1_IN, string INDUSTRYNAME_IN, string RECONSACCOUNT_IN, string TERMOFPAYMENT_IN, string CURRENCY_IN, string PAYMENTMETHODS_IN, string HOUSEBANK_IN, string PMTMETHSUPL_IN
    , string WITHHOLDINGTAXTYPE_IN, string WITHHOLDINGTAXCODE_IN, string VALIDFROM_IN, string VALIDTO_IN, string WITHHOLDINGTAXNUMBER_IN, string INCOTERM_IN, string CREDITCONTROLAREA_IN,
    string ACCASSIGMENTGROUP_IN, string TAXCLASSIFICATION_IN, string EXCHANGERATETYPE_IN, string SALESDISTRICT_IN, string SALESOFFICE_IN, string SALESGROUP_IN, string CUSTOMERGROUP_IN
    , string PRICEGROUP_IN, string CUSTPRICPROC1_IN, string DELIVERYPRIORITYGROUP_IN, string SHIPPINGCONDITION_IN, string CSTNO_IN, string LSTNO_IN, string INVOICINGDATES_IN
    , string INVOICINGLISTDATES_IN, string SERREGNO_IN, string PANNUMBER_IN, string PAYMENTGUARANTEEPROC_IN, string EMAILID_IN);

         string SaveGENERALData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN,
    string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string CUSTOMER_CODE_IN, string TITLE_IN, string NAME1_IN,
    string NAME2_IN, string NAME3_IN, string NAME4_IN, string SEARCHTERM_IN, string STREETHOUSENUMBER_IN, string STREET2_IN, string STREET3_IN,
    string STREET4_IN, string STREET5_IN, string POSTALCODE_IN, string CITY_IN, string COUNTRY_IN, string CREGION_IN, string CTIMEZONE_IN, string TRANSPORTATIONCODE_IN,
    string MOBILEPHONE_IN, string FAX_IN, string EMAIL_IN, string INDUSTRY_IN, string TAXNUMBER3_IN, string CITYCODE_IN, string CTRY_IN, string CUSTOMERCLASS_IN,
      string VENDOR_CODE_IN, string CIN_GST_NO_FILE_IN, string EMAILID_IN, ref string header_id, ref string detailid);// **Added VENDOR_CODE_IN**

         string SaveBANKData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string BANKKEY_IN,
    string BANKACCOUNT_IN, string ACCOUNTHOLDER_IN, string BANKCONTROLKEY_IN, string BANKNAME_IN, string BANKREGION_IN, string BANKSTREET_IN, string BANKCITY_IN,
    string BANKBRANCH_IN, string BANKCURRENCY_IN, string BANK_MANDATE_FILE_IN, string BANK_CANCELED_FILE_IN, string CTRY_IN, string EMAILID_IN, ref string header_id, ref string detailid);

         string SaveCINData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string CSTNO_IN,
            string LSTNO_IN, string INVOICINGDATES_IN, string INVOICINGLISTDATES_IN, string SERREGNO_IN, string PANNUMBER_IN, string PAYMENTGUARANTEEPROC_IN, string E_INVOICE_APPLICABLE_IN, string CIN_PAN_NO_FILE_IN, string CIN_GST_NO_FILE_IN, string CIN_CIN_DOC_FILE_IN, string EMAILID_IN, ref string header_id, ref string detailid);
         string SaveIndustryData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string INDUSTRY1_IN,
     string INDUSTRYCODE1_IN, string INDUSTRYNAME_IN, string FIRSTNAME_IN, string EMAILID_IN, ref string header_id, ref string detailid);

         string SaveSalesData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string SALESCURRENCY_IN,
     string EXCHANGERATETYPE_IN, string SALESDISTRICT_IN, string SALESOFFICE_IN, string SALESGROUP_IN, string CUSTOMERGROUP_IN, string PRICEGROUP_IN, string CUSTPRICPROC1_IN,
     string DELIVERYPRIORITYGROUP_IN, string SHIPPINGCONDITION_IN, string EMAILID_IN, ref string header_id, ref string detailid);
         string SaveCompanyData(long USERID_IN, long CMDETAILID_IN, long CMHEADERID_IN, string CMREQUEST_TYPE_IN, string REQUEST_TYPE_IN, string COMPANY_CODE_IN, string CUST_ACC_TYPE_IN, string SALES_ORG_IN, string DIVISION_GRP_IN, string DISTRIBTUION_CHH_IN, string RECONSACCOUNT_IN,
    string TERMOFPAYMENT_IN, string PAYMENTMETHODS_IN, string HOUSEBANK_IN, string PMTMETHSUPL_IN, string WITHHOLDINGTAXTYPE_IN, string WITHHOLDINGTAXCODE_IN, string VALIDFROM_IN,
    string VALIDTO_IN, string WITHHOLDINGTAXNUMBER_IN, string INCOTERM1_IN, string INCOTERM2_IN, string CREDITCONTROLAREA_IN, string ACCASSIGMENTGROUP_IN, string TAXCLASSIFICATION_IN, string COMP_RTO_FILE_IN, string COMP_LOI_FILE_IN, string EMAILID_IN, string OTHER_DOC_FILE_IN, ref string header_id, ref string detailid);

         string SubmitCMRequest(long EMPCODE_IN, long CMHEADERID_IN, long APPROVAL_IN, long DEPT_DIV_IN, string REMARKS, ref string req_no);
         DataSet GetCMDetailDraft(long _userId, string GEN_REQUEST_NO_IN, ref string err_msg);
         string ResetCMRequest(long EMPCODE_IN, string REQUEST_TYPE, ref string req_no);

        //***************VarunCode******************************* //
        // DataSet GetCutomerFieldDataList()
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
         string AddCustomerMasterData(string code, string codeDesc, string groupName, string pgroupName, string createdBy, string isActive);

        // Method to fetch data based on selected group name
         DataSet FetchCodesByGroupName(string groupName);
         string UpdateCustomerMasterData(string cmdID, string newcode, string newCodeDesc, string GroupName, string PGroupName, string updatedBy, string newIsActive);
         string CustomerApprovalAuthority_Add(string _authType_Id, int _authEmp_Id, string _plantId, int _masterTypeId, int user_Id, int ActiveAuthority);
         string CustomerApprovalAuthority_Update(int VMASTERDATAAPPROVALID, string Request_type, int Authorization_type, int user_Id, int ActiveAuthority);
        DataSet GetApprovalMatrixList(string _AuthType);
        public DataSet GetEmpNameMatrixList(string emp_Code);
         string MaterialSISUserAuthority_Add(int _sisauthEmp_Id, string _sisEmp_Name, int user_Id);
         DataSet GETCUSTOMERREQUEST(string UID, string STATUS, string CUSTOMERHEADERID, string REQUESTTYPE, string CUSTOMERACCGROUP, string CUSTOMERCODE, string REQDATEFROM, string REQDATETO, string CUSTOMERNAME, string REQUEST_NO, string USER_ACCESS_TYPE_IN);
         DataSet GetCustomerDetails(string UID_IN, string GEN_REQUEST_NO, long CMDETAILID_IN);
         DataSet GetApprovalDetails(string UID_IN, string GEN_REQUEST_NO);
         DataSet GetApprovalRequestDetails(long UID_IN);
         string ApprovalRequest(long USERID, long CMHEADERID_IN, string REQUEST_NO, int ACTION_IN, string REMARKS);

        //*********************************Data Sync********************************************************//
         DataSet GETCUSTOMERREQUESTDataSync(string UID, string STATUS, string CUSTOMERHEADERID, string REQUESTTYPE, string CUSTOMERACCGROUP, string CUSTOMERCODE, string REQDATEFROM, string REQDATETO);
         DataSet GetDataForSAPSYNC(string UID_IN, string CMHEADERID_IN, string CMDETAILID_IN);
         string InsertIntoSAPRFCLOG(string userid, string uid, string kunnr, string spart, string zStatus, string value);

         string AddMandentaryData(string FIELDNAME_IN, string SERVICETYPE_IN, string AG_5AG1_IN, string AG_5AG3_IN, string AG_5AGV_IN, string AG_5AGG_IN, string AG_5AG6_IN, string AG_5AGW_IN, string AG_5AG2_IN, string AG_5AG5_IN, string GROUP_NAME_IN, string CLIENTIDLABEL_IN, string ClIENTIDTEXTLABEL_IN, string IS_ACTION_IN, string CREATEDBY_IN);
         string UpdateMandentaryData(int FIELDID_IN, string FIELDNAME_IN, string SERVICETYPE_IN, string AG_5AG1_IN, string AG_5AG3_IN, string AG_5AGV_IN, string AG_5AGG_IN, string AG_5AG6_IN, string AG_5AGW_IN, string AG_5AG2_IN, string AG_5AG5_IN, string GROUP_NAME_IN, string CLIENTIDLABEL_IN, string ClIENTIDTEXTLABEL_IN, string IS_ACTION_IN, string CREATEDBY_IN);

         DataSet GetCutomerRequest(long UID_IN, string REQ_TYPE);

         DataSet GetCustomerApprovalDetails(string GEN_REQUEST_NO);
         string UpdateApproval(string CUR_REQ_NO, int APROVER_ROLE, int PRE_APROVER, int NEW_APROVER, int USER_ID, string REMARKS_IN, string ATTACH_DOC);

         DataSet GetSwitchApprovalData(int user_id);

         DataSet GetEMAILData(long user_id, string REQNO);

        void SENDMAILTODEPTHEAD(long USERID, string req_no, string strSubject, string action, string customercode, string customername);
    }
}
