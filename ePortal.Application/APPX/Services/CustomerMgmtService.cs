using ePortal.Application.APPX.Contracts;
using ePortal.Persistence;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.CustomerMgmt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Spire.Additions.Xps.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Application.APPX.Services
{
    public class CustomerMgmtService : ICustomerMgmtService
    {
        #region "Variable"
        public string filePath = ""; // = serverpath.getFileUploadPath() + "CustomerMaster";
        protected string errMsg;
        DataSet objDs = new DataSet();
       
        int _ApprovalCode = 0;
        #endregion
        private ICustomerMgmtRepo oMasterQueries;
        private readonly IAppConfigurationService _env;
        private ISessionService _session;
        ILogger<CustomerMgmtService> _logger;
        private readonly IEportalESS objess;
        public CustomerMgmtService(ICustomerMgmtRepo customerMgmt, IAppConfigurationService appConfiguration, ISessionService sessionService, IEportalESS eportalESS, ILogger<CustomerMgmtService> logger)
        {
            oMasterQueries = customerMgmt;
            _env = appConfiguration;
            _session = sessionService;
            _logger = logger;
            objess = eportalESS;
           
        }
             
        public CustomerMgmtResponse AddCustomerMasterData(CMMASTER_DATA data, string Actiontype) {
            CustomerMgmtResponse res= new CustomerMgmtResponse();
            string createdDate= DateTime.Now.ToString("dd/MM/yyyy");
            string oDs = "";
            if(Actiontype=="Add")
            oDs = oMasterQueries.AddCustomerMasterData(data.Code,data.CodeDesc, data.GroupName, data.PgroupName, data.CreatedBy, data.IsActive);
            else
                oDs = oMasterQueries.UpdateCustomerMasterData((data.CmdId?? 0).ToString(), data.Code, data.CodeDesc, data.GroupName, data.PgroupName, data.CreatedBy, data.IsActive);
            if (oDs == "1")
            {
                res.code = "1";
                res.message =(Actiontype == "Add")? "Successfully created!": "Successfully updated!";
            }
            else
            {
                res.code = "-1";
                res.message = oDs;
            }
            return res;
        }
        public DataTable FetchCodesByGroupName(string groupName)
        {
            DataTable res = new DataTable();
            try { 
            
            DataSet oDs = oMasterQueries.FetchCodesByGroupName(groupName);

            if (oDs != null && oDs.Tables.Count > 0 )
            {
                //MasterDataGridView.DataSource = oDs.Tables[0];
                //MasterDataGridView.DataBind();
                 var dataList = oDs.Tables[0]?.AsEnumerable()
                 .Select(r => new CMMASTER_DATA
                 {
                     Code = r.Field<string>("CODE"),
                     CodeDesc = r.Field<string>("CODE_DESC"),
                     PgroupName = r.Field<string>("PGROUP_NAME"),
                     IsActive = r.Field<string>("ISACTIVE"),
                     CmdId = Convert.ToInt32(r["CMD_ID"])
                 })
                 .ToList() ?? new List<CMMASTER_DATA>();

                return oDs.Tables[0];
            }
        } catch (Exception ex) {
                _logger.LogError("Error in group list binding", ex.Message);
            }
           
            return res;
        }
        public List<CustomerReqApproverDetails> GetCustomerApprovalDetails(string customerId) {
            try
            {
                DataSet ds = oMasterQueries.GetCustomerApprovalDetails(customerId.Trim().ToUpper());
                DataTableConverter dc = new DataTableConverter();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var data = dc.TableToList<CustomerReqApproverDetails>(ds.Tables[0]);
                    return data ?? (new List<CustomerReqApproverDetails>());
                }
                else
                {
                    return new List<CustomerReqApproverDetails>();
                }
            }
            catch (Exception ex) {
                _logger.LogError("GetCustomerApprovalDetails", ex.Message);
                return new List<CustomerReqApproverDetails>();
            }
           
        }
        public List<CMApprovalAuthority> GetFinanceApproverData()
        {
            try
            {
                DataSet ds = oMasterQueries.GetApprovalMatrixList("FIN");
                DataTableConverter dc = new DataTableConverter();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var data = dc.TableToList<CMApprovalAuthority>(ds.Tables[0]);
                    return data ?? (new List<CMApprovalAuthority>());
                }
                else
                {
                    return new List<CMApprovalAuthority>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetFinanceApproverData", ex.Message);
                return new List<CMApprovalAuthority>();
            }

        }
        public List<SwitchApprovalAuthority> GeSwitchApproverData(int userid)
        {
            try
            {
                DataSet ds = oMasterQueries.GetSwitchApprovalData(userid);
                DataTableConverter dc = new DataTableConverter();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var data = dc.TableToList<SwitchApprovalAuthority>(ds.Tables[0]);
                    return data ?? (new List<SwitchApprovalAuthority>());
                }
                else
                {
                    return new List<SwitchApprovalAuthority>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GeSwitchApproverData", ex.Message);
                return new List<SwitchApprovalAuthority>();
            }

        }
        public string GeApproverName(string userid)
        {
            try
            {
                DataSet ds = oMasterQueries.GetEmpNameMatrixList(userid);
                DataTableConverter dc = new DataTableConverter();
                if (ds.Tables[0].Rows.Count > 0)
                {
                                                           
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GeSwitchApproverData", ex.Message);
                return "error";
            }

        }
        public string SaveFinanceApproverData(CMApproverMatrx data, ref int res)
        {
            res = 0;
            try { 
            int _masterTypeId = Convert.ToInt32(data.MasterType);
            string _plantId = data.RequestType;
            string _authType_Id = "FINANCE";
            int _authEmp_Id = Convert.ToInt32(data.AuthorizationAuth);
            int _userId = Convert.ToInt32(data.CreatedBy);
            int _activeAuthority = Convert.ToInt32(data.AuthActive);
            if (_authType_Id != null && _authEmp_Id != 0 && _plantId != null && _masterTypeId != 0)
            {
                if (data.ActionType == "Submit")
                {
                   var output = oMasterQueries.CustomerApprovalAuthority_Add(_authType_Id, _authEmp_Id, _plantId, _masterTypeId, _userId, _activeAuthority);
                    if (output != "1")
                    {
                        return output;
                    }
                    else
                    {
                            res = 1;
                        return  "Approval Matrix request is added successfully.";
                    }
                  
                }
                else
                {
                    var output = oMasterQueries.CustomerApprovalAuthority_Update(Convert.ToInt32(data.REF_ID), _plantId, 1, _userId, _activeAuthority);
                        
                        if (output != "1")
                        {
                             return output;
                        }
                        else
                        {
                            res = 1;
                             return "Approval Matrix request is Updated successfully.";
                        }
                       
                    }
                }
                return "";
            }
            catch (Exception ex) {
                _logger.LogError("SaveFinanceApproverData", ex.Message);
                return "error";
            }
        }
        public List<CustomerMasterDataMng> ShowFieldMaster()
        {
            try
            {
                DataSet ds = oMasterQueries.GetCutomerFieldDataList();
                DataTableConverter dc = new DataTableConverter();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var data = dc.TableToList<CustomerMasterDataMng>(ds.Tables[0]);
                    return data ?? (new List<CustomerMasterDataMng>());
                }
                else
                {
                    return (new List<CustomerMasterDataMng>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GeSwitchApproverData", ex.Message);
                return (new List<CustomerMasterDataMng>());
            }
        }
        public List<CustomerMasterDataMng> SaveMandatoryData(CustomerMasterDataMng data, ref string res)
        {
            try
            {
                string FIELDNAME_IN = "", SERVICETYPE_IN = "", AG_5AG1_IN = "", AG_5AG3_IN = "", AG_5AGV_IN = "", AG_5AGG_IN = "", AG_5AG6_IN = "", AG_5AGW_IN = "", AG_5AG2_IN = "", AG_5AG5_IN = "", GROUP_NAME_IN = "", CLIENTIDLABEL_IN = "", ClIENTIDTEXTLABEL_IN = "", IS_ACTION_IN = "A", CREATEDBY_IN = "";
                
                AG_5AG1_IN = data.AG_5AG1??"0";
                AG_5AG3_IN = data.AG_5AG3 ?? "0";
                AG_5AGV_IN = data.AG_5AGV ?? "0";
                AG_5AGG_IN = data.AG_5AGG ?? "0";
                AG_5AG6_IN = data.AG_5AG6 ?? "0";
                AG_5AGW_IN = data.AG_5AGW ?? "0";
                AG_5AG2_IN = data.AG_5AG2 ?? "0";
                AG_5AG5_IN = data.AG_5AG5 ?? "0";
                CLIENTIDLABEL_IN = data.CLIENTIDLABEL ?? "";
                ClIENTIDTEXTLABEL_IN = data.CLIENTIDTEXTLABEL ?? "";
                GROUP_NAME_IN = data.GROUP_NAME ?? "";
                CREATEDBY_IN = data.CreatedBy ?? "";
                FIELDNAME_IN = data.FIELDNAME??"";
                string rs = oMasterQueries.UpdateMandentaryData(data.ID, FIELDNAME_IN, SERVICETYPE_IN, AG_5AG1_IN, AG_5AG3_IN, AG_5AGV_IN, AG_5AGG_IN, AG_5AG6_IN, AG_5AGW_IN, AG_5AG2_IN, AG_5AG5_IN, GROUP_NAME_IN, CLIENTIDLABEL_IN, ClIENTIDTEXTLABEL_IN, IS_ACTION_IN, CREATEDBY_IN);
                res = rs;
                DataSet ds = oMasterQueries.GetCutomerFieldDataList();
                DataTableConverter dc = new DataTableConverter();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var datalist = dc.TableToList<CustomerMasterDataMng>(ds.Tables[0]);
                    return datalist ?? (new List<CustomerMasterDataMng>());
                }
                else
                {
                    return (new List<CustomerMasterDataMng>());
                }
            }
            catch (Exception ex)
            {
                res = "Error in dat save!";
                _logger.LogError("GeSwitchApproverData", ex.Message);
                return (new List<CustomerMasterDataMng>());
            }
        }
        public List<CustomerMasterDataMng> AddMandatoryData(CustomerMasterDataMng data, ref string res)
        {
            try
            {
             string FIELDNAME_IN = "", SERVICETYPE_IN = "", AG_5AG1_IN = "", AG_5AG3_IN = "", AG_5AGV_IN = "", AG_5AGG_IN = "", AG_5AG6_IN = "", AG_5AGW_IN = "", AG_5AG2_IN = "", AG_5AG5_IN = "", GROUP_NAME_IN = "", CLIENTIDLABEL_IN = "", ClIENTIDTEXTLABEL_IN = "", IS_ACTION_IN = "A", CREATEDBY_IN = "";
            FIELDNAME_IN = data.FIELDNAME ?? "";
            AG_5AG1_IN = data.AG_5AG1 ?? "0";
            AG_5AG3_IN = data.AG_5AG3 ?? "0";
            AG_5AGV_IN = data.AG_5AGV ?? "0";
            AG_5AGG_IN = data.AG_5AGG ?? "0";
            AG_5AG6_IN = data.AG_5AG6 ?? "0";
            AG_5AGW_IN = data.AG_5AGW ?? "0";
            AG_5AG2_IN = data.AG_5AG2 ?? "0";
            AG_5AG5_IN = data.AG_5AG5 ?? "0";
            CLIENTIDLABEL_IN = data.CLIENTIDLABEL ?? "";
            ClIENTIDTEXTLABEL_IN = data.CLIENTIDTEXTLABEL ?? "";
            GROUP_NAME_IN = data.GROUP_NAME ?? "";
            CREATEDBY_IN = data.CreatedBy ?? "";   

                string err_msg = "";
                if (CLIENTIDLABEL_IN.Trim().Length < 1) { errMsg += "Client Label ID/"; }
                if (ClIENTIDTEXTLABEL_IN.Trim().Length < 1) { errMsg += "Client ID Textbox/"; }
                if (FIELDNAME_IN.Trim().Length < 1) { errMsg += "Field Name/"; }
                if (GROUP_NAME_IN.Trim().Length < 1) { errMsg += "Group Name/"; }
                if (err_msg != "")
                {
                    res = "Please " + err_msg.Substring(0, err_msg.Length - 1) + " cannot be blank.";
                   
                }
                string rs = oMasterQueries.AddMandentaryData(FIELDNAME_IN, SERVICETYPE_IN, AG_5AG1_IN, AG_5AG3_IN, AG_5AGV_IN, AG_5AGG_IN, AG_5AG6_IN, AG_5AGW_IN, AG_5AG2_IN, AG_5AG5_IN, GROUP_NAME_IN, CLIENTIDLABEL_IN, ClIENTIDTEXTLABEL_IN, IS_ACTION_IN, CREATEDBY_IN);
                res = rs;
                DataSet ds = oMasterQueries.GetCutomerFieldDataList();
                DataTableConverter dc = new DataTableConverter();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var datalist = dc.TableToList<CustomerMasterDataMng>(ds.Tables[0]);
                    return datalist ?? (new List<CustomerMasterDataMng>());
                }
                else
                {
                    return (new List<CustomerMasterDataMng>());
                }
            }
            catch (Exception ex)
            {
                res = "Error in dat save!";
                _logger.LogError("AddMandatoryData", ex.Message);
                return (new List<CustomerMasterDataMng>());
            }
        }
        public List<CustomerRequestRpt> ShowCustomerRequestData(CustomerRequestInput _data, ref string res)
        {
            try
            {
                string REQUESTTYPE = _data.REQUESTTYPE ?? "";
                string CUSTACCGROUP = _data.CUSTACCGROUP ?? "";
                string REQDATEFROM = _data.REQDATEFROM??"";
                string REQDATETO = _data.REQDATETO??"";
                string CUSTCODE = _data.CUSTCODE??"";
                string STATUS = _data.STATUS??"";
                string CUSTOMERNAME = _data.CUSTOMERNAME ?? "";
                string REQUEST_NO = _data.REQUEST_NO??"";
                string UserID = _data.USERID ?? "";
                string UserType = _data.USERTYPE ?? "A";
                DataSet ds = oMasterQueries.GETCUSTOMERREQUEST(UserID, STATUS, "", REQUESTTYPE, CUSTACCGROUP, CUSTCODE, REQDATEFROM, REQDATETO, CUSTOMERNAME, REQUEST_NO, UserType);
                DataTableConverter dc = new DataTableConverter();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //var data = dc.TableToList<CustomerRequestRpt>(ds.Tables[0]);
                   var dataList = ds.Tables[0]?.AsEnumerable()
                  .Select(r => new CustomerRequestRpt
                  {
                      REQUESTEDID = r.Field<long>("REQUESTEDID"),
                      REQUESTERID = r.Field<long>("REQUESTERID"),
                      REQUEST_TYPE = r.Field<string>("REQUEST_TYPE"),
                      REQUESTDATE = r.Field<string>("REQUESTDATE"),
                      GEN_REQUEST_NO = r.Field<string>("GEN_REQUEST_NO"),
                      CUSTOMER_CODE = r.Field<string>("CUSTOMER_CODE"),
                      CUSTOMER_TYPE = r.Field<string>("CUSTOMER_TYPE"),
                      CUST_ACC_TYPE = r.Field<string>("CUST_ACC_TYPE"),
                      NAME1 = r.Field<string>("NAME1"),
                      Status = r.Field<string>("Status"),
                      DETAIL_STATUS = r.Field<string>("DETAIL_STATUS"),
                      CMHEADERID=r.Field<long>("CMHEADERID"),
                      USERTYPE = UserType

                  })
                  .ToList() ?? new List<CustomerRequestRpt>();
                    return dataList ?? (new List<CustomerRequestRpt>());
                }
                else
                {
                    return (new List<CustomerRequestRpt>());
                }
            }
            catch (Exception ex)
            {
                res = "Error in data binding";
                _logger.LogError("ShowCustomerRequestData", ex.Message);
                return (new List<CustomerRequestRpt>());
            }
        }
        public DataSet BindMasterData(string groupname)
        {
            try
            {
                DataSet ds = oMasterQueries.GetCustomerMasterData(groupname);
                return ds;               
            }
            catch (Exception ex)
            {
                _logger.LogError("BindMasterData", ex.Message);
                return new DataSet();
            }
        }
        public DataSet GetSectionHead(int _empCode)
        {
            DataSet ds = new DataSet();
            try
            {
                DataSet oDs = new DataSet();
                oDs = oMasterQueries.GetSectionHead(_empCode);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = oDs.Tables[0].Copy();
                    dt.TableName = "Table1";
                    ds.Tables.Add(dt);
                }

                oDs = oMasterQueries.GetDepartmentHead(_empCode);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = oDs.Tables[0].Copy();
                    dt.TableName = "Table2";
                    ds.Tables.Add(dt);

                }
                return ds;
            }
            catch(Exception ex) {
                _logger.LogError("GetSectionHead", ex.Message);
                return new DataSet();
            }

        }
        public DataSet BindTransportGRP(string RequestType, string Country)
        {
            DataSet objDs = new DataSet();
            try { 
            if (RequestType == "EX" && Country != "IN")
            {
                objDs = oMasterQueries.GetCustomerMasterDataWithPGRP("TRANSPORTATION CODE EXP", Country);                
            }
            else
            {
                objDs = oMasterQueries.GetCustomerMasterData("TRANSPORTATION CODE");
              
            }
                return objDs;
            }
            catch (Exception ex) {
                _logger.LogError("BindTransportGRP", ex.Message);
                return new DataSet();
            }
            
        }
         public DataTable SetMandatoryField(CustAccountFlags data)
            {
                try
                {
                    string ag5AG1 = "", ag5AG3 = "", ag5AGV = "", ag5AGG = "", ag5AG6 = "", ag5AGW = "", ag5AG2 = "", ag5AG5 = "";
                    if (data.Ag5AG1=="1")
                    {
                        ag5AG1 = "1";
                    }
                    if (data.Ag5AG2 == "1" )
                    {
                        ag5AG2 = "1";
                    }
                    if (data.Ag5AGV == "1" )
                    {
                        ag5AGV = "1";
                    }
                    if (data.Ag5AG3 == "1")
                    {
                        ag5AG3 = "1";
                    }
                    if (data.Ag5AGG == "1")
                    {
                        ag5AGG = "1";
                    }
                    if (data.Ag5AG6 == "1")
                    {
                        ag5AG6 = "1";
                    }
                    if (data.Ag5AGW == "1" )
                    {
                        ag5AGW = "1";
                    }

                    if (data.Ag5AG5 == "1")
                    {
                        ag5AG5 = "1";
                    }
                    objDs = oMasterQueries.GetCustomerMandatoryData(ag5AG1, ag5AG3, ag5AGV, ag5AGG, ag5AG6, ag5AGW, ag5AG2, ag5AG5, "");
                    if (objDs.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = objDs.Tables[0].Copy();
                        return dt;

                    }
                    else
                    {
                        return new DataTable();
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError("SetMandatoryField", ex.Message);
                    return new DataTable();
                }
            }
         public DataTable GetDetailDraft(string UserId, string RequestNo, ref string err)
            {
                try
                {               
                    long userid = Convert.ToInt32(UserId);
                    objDs = oMasterQueries.GetCMDetailDraft(userid, RequestNo, ref err);                
                    if (objDs.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = objDs.Tables[0].Copy();
                        return dt;
                    }
                    else
                    {
                        return new DataTable();
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError("GetDetailDraft", ex.Message);
                    return new DataTable();
                }
            }
         public DataTable GetCustomerMasterDataWithPGRP(string Group_Name, string PGroup_Name)
            {
                try
                {               
                    objDs = oMasterQueries.GetCustomerMasterDataWithPGRP(Group_Name, PGroup_Name);
                    if (objDs.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = objDs.Tables[0].Copy();
                        return dt;
                    }
                    else
                    {
                        return new DataTable();
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError("GetCustomerMasterDataWithPGRP", ex.Message);
                    return new DataTable();
                }
            }
         public CustomerResponseData SaveGeneral(CustomerMasterDetail data, string CIN_GST_NO_FILE_IN , string email_id)
         {
            CustomerResponseData res=new CustomerResponseData();
            try
            {
               DataSet objDs = oMasterQueries.GetCustomerMandatoryData("", "", "", "", "", "", "", "", "");
                
                DataTable  dv;
                //var str = _session.Get<string>("vds");                
                //dt = JsonConvert.DeserializeObject<DataTable>(str);

                string headerid = "", detailid = "";
                long cmhid=0, cmdid1=0, cmdid2=0, USERID=0;
                string CUSTOMER_CODE_IN = "", TITLE_IN = "", NAME1_IN = "",
                 NAME2_IN = "", NAME3_IN = "", NAME4_IN = "", SEARCHTERM_IN = "", STREETHOUSENUMBER_IN = "", STREET2_IN = "", STREET3_IN = "",
                 STREET4_IN = "", STREET5_IN = "", POSTALCODE_IN = "", CITY_IN = "", COUNTRY_IN = "", CREGION_IN = "", CTIMEZONE_IN = "", TRANSPORTATIONCODE_IN = "",
                 MOBILEPHONE_IN = "", FAX_IN = "", EMAIL_IN = "", INDUSTRY_IN = "", TAXNUMBER3_IN = "", CITYCODE_IN = "", CTRY_IN = "", CUSTOMERCLASS_IN = "", VENDOR_CODE_IN = ""; // Added Vendor Code Declaration                            
                cmhid = data.CMHEADERID ?? 0;
                cmdid1 = data.CMDETAILID ?? 0;
                cmdid2 = data.CMDETAILID1 ?? 0;
                USERID = data.REQUESTEDID ?? 0;
                 dv = objDs.Tables[1].Select(" group_name='GeneralData' And " + "ag_" + data.CUST_ACC_TYPE + "=1").CopyToDataTable();
                
                foreach (DataRow row in dv.Rows)
                {
                    if ("txtDealerCode" == row["clientidtextlabel"].ToString()) { CUSTOMER_CODE_IN = data.CUSTOMER_CODE ?? ""; }
                    if ("txtName1" == row["clientidtextlabel"].ToString()) { NAME1_IN = data.NAME1 ?? "".Trim(); }
                    if ("txtName2" == row["clientidtextlabel"].ToString()) { NAME2_IN = data.NAME2 ?? "".Trim(); }
                    if ("txtName3" == row["clientidtextlabel"].ToString()) { NAME3_IN = data.NAME3 ?? "".Trim(); }
                    if ("txtName4" == row["clientidtextlabel"].ToString()) { NAME4_IN = data.NAME4 ?? "".Trim(); }
                    if ("txtSearchTerms" == row["clientidtextlabel"].ToString()) { SEARCHTERM_IN = data.SEARCHTERM ?? "".Trim(); }
                    if ("txtStreetHouse" == row["clientidtextlabel"].ToString()) { STREETHOUSENUMBER_IN = data.STREETHOUSENUMBER ?? "".Trim(); }
                    if ("txtStreet2" == row["clientidtextlabel"].ToString()) { STREET2_IN = data.STREET2 ?? "".Trim(); }
                    if ("txtStreet3" == row["clientidtextlabel"].ToString()) { STREET3_IN = data.STREET3 ?? "".Trim(); }
                    if ("txtStreet4" == row["clientidtextlabel"].ToString()) { STREET4_IN = data.STREET4 ?? "".Trim(); }
                    if ("txtStreet5" == row["clientidtextlabel"].ToString()) { STREET5_IN = data.STREET5 ?? "".Trim(); }
                    if ("txtPostalCode" == row["clientidtextlabel"].ToString()) { POSTALCODE_IN = data.POSTALCODE ?? "".Trim(); }
                    if ("txtCity" == row["clientidtextlabel"].ToString()) { CITY_IN = data.CITY ?? "".Trim(); }
                    if ("txtMobilePhone" == row["clientidtextlabel"].ToString()) { MOBILEPHONE_IN = data.MOBILEPHONE ?? "".Trim(); }
                    if ("txtFax" == row["clientidtextlabel"].ToString()) { FAX_IN = data.FAX??"".Trim(); }
                    if ("txtEmail" == row["clientidtextlabel"].ToString()) { EMAIL_IN = data.EMAIL??"".Trim(); }
                    if ("txtTaxNumberGst" == row["clientidtextlabel"].ToString()) { TAXNUMBER3_IN = data.TAXNUMBER3 ?? "".Trim(); }
                    if ("txtVendorCode" == row["clientidtextlabel"].ToString()) { VENDOR_CODE_IN = data.VENDORNO ?? "".Trim(); }
                    if ("ddlTitle" == row["clientidtextlabel"].ToString()) { TITLE_IN = data.TITLE ?? ""; }
                    if ("ddlCountry" == row["clientidtextlabel"].ToString()) { COUNTRY_IN = data.COUNTRY ?? ""; }
                    if ("ddlRegion" == row["clientidtextlabel"].ToString()) { CREGION_IN = data.CREGION ?? ""; }
                    if ("ddlTimeZone" == row["clientidtextlabel"].ToString()) { CTIMEZONE_IN = data.CTIMEZONE ?? ""; }
                    if ("ddlTransportationCode" == row["clientidtextlabel"].ToString()) { TRANSPORTATIONCODE_IN = data.TRANSPORTATIONCODE ?? ""; }
                    if ("ddlIndustry" == row["clientidtextlabel"].ToString()) { INDUSTRY_IN = data.INDUSTRY ?? ""; }
                    if ("ddlCityCode" == row["clientidtextlabel"].ToString()) { CITYCODE_IN = data.CITYCODE ?? ""; }
                    if ("ddCustomerClass" == row["clientidtextlabel"].ToString()) { CUSTOMERCLASS_IN = data.CUSTOMERCLASS ?? ""; }
                }

                var result = oMasterQueries.SaveGENERALData(USERID, cmdid1, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE,data.SALES_ORG,data.DIVISION_GRP,data.DISTRIBTUION_CHH, CUSTOMER_CODE_IN, TITLE_IN, NAME1_IN, NAME2_IN, NAME3_IN, NAME4_IN, SEARCHTERM_IN, STREETHOUSENUMBER_IN, STREET2_IN, STREET3_IN, STREET4_IN, STREET5_IN, POSTALCODE_IN, CITY_IN, COUNTRY_IN, CREGION_IN, CTIMEZONE_IN, TRANSPORTATIONCODE_IN, MOBILEPHONE_IN, FAX_IN, EMAIL_IN, INDUSTRY_IN, TAXNUMBER3_IN, CITYCODE_IN, CTRY_IN, CUSTOMERCLASS_IN, VENDOR_CODE_IN, CIN_GST_NO_FILE_IN, email_id, ref headerid, ref detailid);

                if (result == "1")
                {
                    res.CMHEADERID = Convert.ToInt32(headerid);
                    res.CMDEAILID1 = Convert.ToInt32(detailid);
                    res.RS = Convert.ToInt32(result);                                  

                }
                else {
                    res.MESSAGE = "Error in process of data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveGeneral");
                }
                if (data.CUST_ACC_TYPE1 == "5AGW" && result == "1")
                {

                    CUSTOMER_CODE_IN = ""; TITLE_IN = ""; NAME1_IN = ""; NAME2_IN = ""; NAME3_IN = ""; NAME4_IN = ""; SEARCHTERM_IN = ""; STREETHOUSENUMBER_IN = ""; STREET2_IN = ""; STREET3_IN = ""; STREET4_IN = ""; STREET5_IN = ""; POSTALCODE_IN = ""; CITY_IN = ""; COUNTRY_IN = ""; CREGION_IN = ""; CTIMEZONE_IN = ""; TRANSPORTATIONCODE_IN = "";
                    MOBILEPHONE_IN = ""; FAX_IN = ""; EMAIL_IN = ""; INDUSTRY_IN = ""; TAXNUMBER3_IN = ""; CITYCODE_IN = ""; CTRY_IN = ""; CUSTOMERCLASS_IN = ""; VENDOR_CODE_IN = "";

                    try {
                        dv = objDs.Tables[1].Select(" group_name='GeneralData' And " + "ag_" + data.CUST_ACC_TYPE1 + "=1").CopyToDataTable();
                    } catch { dv = new DataTable(); }
                    
                    foreach (DataRow row in dv.Rows)
                    {
                        if ("txtDealerCode" == row["clientidtextlabel"].ToString()) { CUSTOMER_CODE_IN = data.CUSTOMER_CODE ?? ""; }
                        if ("txtName1" == row["clientidtextlabel"].ToString()) { NAME1_IN = data.NAME1 ?? "".Trim(); }
                        if ("txtName2" == row["clientidtextlabel"].ToString()) { NAME2_IN = data.NAME2 ?? "".Trim(); }
                        if ("txtName3" == row["clientidtextlabel"].ToString()) { NAME3_IN = data.NAME3 ?? "".Trim(); }
                        if ("txtName4" == row["clientidtextlabel"].ToString()) { NAME4_IN = data.NAME4 ?? "".Trim(); }
                        if ("txtSearchTerms" == row["clientidtextlabel"].ToString()) { SEARCHTERM_IN = data.SEARCHTERM ?? "".Trim(); }
                        if ("txtStreetHouse" == row["clientidtextlabel"].ToString()) { STREETHOUSENUMBER_IN = data.STREETHOUSENUMBER ?? "".Trim(); }
                        if ("txtStreet2" == row["clientidtextlabel"].ToString()) { STREET2_IN = data.STREET2 ?? "".Trim(); }
                        if ("txtStreet3" == row["clientidtextlabel"].ToString()) { STREET3_IN = data.STREET3 ?? "".Trim(); }
                        if ("txtStreet4" == row["clientidtextlabel"].ToString()) { STREET4_IN = data.STREET4 ?? "".Trim(); }
                        if ("txtStreet5" == row["clientidtextlabel"].ToString()) { STREET5_IN = data.STREET5 ?? "".Trim(); }
                        if ("txtPostalCode" == row["clientidtextlabel"].ToString()) { POSTALCODE_IN = data.POSTALCODE ?? "".Trim(); }
                        if ("txtCity" == row["clientidtextlabel"].ToString()) { CITY_IN = data.CITY ?? "".Trim(); }
                        if ("txtMobilePhone" == row["clientidtextlabel"].ToString()) { MOBILEPHONE_IN = data.MOBILEPHONE ?? "".Trim(); }
                        if ("txtFax" == row["clientidtextlabel"].ToString()) { FAX_IN = data.FAX ?? "".Trim(); }
                        if ("txtEmail" == row["clientidtextlabel"].ToString()) { EMAIL_IN = data.EMAIL ?? "".Trim(); }
                        if ("txtTaxNumberGst" == row["clientidtextlabel"].ToString()) { TAXNUMBER3_IN = data.TAXNUMBER3 ?? "".Trim(); }
                        if ("txtVendorCode" == row["clientidtextlabel"].ToString()) { VENDOR_CODE_IN = data.VENDORNO ?? "".Trim(); }
                        if ("ddlTitle" == row["clientidtextlabel"].ToString()) { TITLE_IN = data.TITLE ?? ""; }
                        if ("ddlCountry" == row["clientidtextlabel"].ToString()) { COUNTRY_IN = data.COUNTRY ?? ""; }
                        if ("ddlRegion" == row["clientidtextlabel"].ToString()) { CREGION_IN = data.CREGION ?? ""; }
                        if ("ddlTimeZone" == row["clientidtextlabel"].ToString()) { CTIMEZONE_IN = data.CTIMEZONE ?? ""; }
                        if ("ddlTransportationCode" == row["clientidtextlabel"].ToString()) { TRANSPORTATIONCODE_IN = data.TRANSPORTATIONCODE ?? ""; }
                        if ("ddlIndustry" == row["clientidtextlabel"].ToString()) { INDUSTRY_IN = data.INDUSTRY ?? ""; }
                        if ("ddlCityCode" == row["clientidtextlabel"].ToString()) { CITYCODE_IN = data.CITYCODE ?? ""; }
                        if ("ddCustomerClass" == row["clientidtextlabel"].ToString()) { CUSTOMERCLASS_IN = data.CUSTOMERCLASS ?? ""; }
                    }

                        cmhid = data.CMHEADERID??0;
                    // result = oMasterQueries.SaveGENERALData(USERID, cmdid2, cmhid, rdlRequest.SelectedValue, rdlRequestType.SelectedValue, txtCompanyCode.Text, ac_55AGW, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, CUSTOMER_CODE_IN, TITLE_IN, NAME1_IN, NAME2_IN, NAME3_IN, NAME4_IN, SEARCHTERM_IN, STREETHOUSENUMBER_IN, STREET2_IN, STREET3_IN, STREET4_IN, STREET5_IN, POSTALCODE_IN, CITY_IN, COUNTRY_IN, CREGION_IN, CTIMEZONE_IN, TRANSPORTATIONCODE_IN, MOBILEPHONE_IN, FAX_IN, EMAIL_IN, INDUSTRY_IN, TAXNUMBER3_IN, CITYCODE_IN, CTRY_IN, CUSTOMERCLASS_IN, VENDOR_CODE_IN, CIN_GST_NO_FILE_IN, email_id, ref headerid, ref detailid);
                    result = oMasterQueries.SaveGENERALData(USERID, cmdid2, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE1, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, CUSTOMER_CODE_IN, TITLE_IN, NAME1_IN, NAME2_IN, NAME3_IN, NAME4_IN, SEARCHTERM_IN, STREETHOUSENUMBER_IN, STREET2_IN, STREET3_IN, STREET4_IN, STREET5_IN, POSTALCODE_IN, CITY_IN, COUNTRY_IN, CREGION_IN, CTIMEZONE_IN, TRANSPORTATIONCODE_IN, MOBILEPHONE_IN, FAX_IN, EMAIL_IN, INDUSTRY_IN, TAXNUMBER3_IN, CITYCODE_IN, CTRY_IN, CUSTOMERCLASS_IN, VENDOR_CODE_IN, CIN_GST_NO_FILE_IN, email_id, ref headerid, ref detailid);

                }
                if (result == "1")
                {
                    res.CMDEAILID2 = Convert.ToInt32(detailid);
                    res.MESSAGE = "Successfully saved!";
                   // hidDetaildID1.Value = detailid;
                    //ShowInfo("Successfully saved!");
                    res.VIEWSTATE = "Y";
                    
                    if (CIN_GST_NO_FILE_IN != "")
                    {
                        res.FILE1 = CIN_GST_NO_FILE_IN;
                        //hyperGSTNo.Text = CIN_GST_NO_FILE_IN;
                        //hyperGSTNo.Visible = true;
                        //hyperGSTNo.OnClientClick = "javascript:openDialog('../../Uploads/CustomerMaster/" + hyperGSTNo.Text + "',980,680);";
                    }

                }
                else
                {
                    //ShowError("Error in process of data save!");
                    //btnSaveGenData.ToolTip = result;
                    res.MESSAGE = "Error in process of data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveGeneral");
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SaveGeneral");
                res.MESSAGE = "Error in process of data save!";
                res.RS = 0;
                return res;
            }
            
        }
        public CustomerResponseData SaveBankData(CustomerMasterDetail data, string BANK_MANDATE_FILE_IN, string BANK_CANCELED_FILE_IN, string email_id)
        {
            CustomerResponseData res = new CustomerResponseData();
            try
            {
                DataSet objDs = oMasterQueries.GetCustomerMandatoryData("", "", "", "", "", "", "", "", "");

                DataTable dv=new DataTable();
                //var str = _session.Get<string>("vds");                
                //dt = JsonConvert.DeserializeObject<DataTable>(str);              
                string headerid = "", detailid = "";
                long cmhid = 0, cmdid1 = 0, cmdid2 = 0, USERID = 0;
                string BANKKEY_IN = "",BANKACCOUNT_IN = "", ACCOUNTHOLDER_IN = "", BANKCONTROLKEY_IN = "", BANKNAME_IN = "", BANKREGION_IN = "", BANKSTREET_IN = "", BANKCITY_IN = "",
BANKBRANCH_IN = "", BANKCURRENCY_IN = "", CTRY_IN = "";
                cmhid = data.CMHEADERID ?? 0;
                cmdid1 = data.CMDETAILID ?? 0;
                cmdid2 = data.CMDETAILID1 ?? 0;
                USERID = data.REQUESTEDID ?? 0;
                //dv1 = dv.Select(" group_name='BankDetails' And " + "ag_" + ac_type + "=1").CopyToDataTable();
                try {
                    dv = objDs.Tables[1].Select(" group_name='BankDetails' And " + "ag_" + data.CUST_ACC_TYPE + "=1").CopyToDataTable();
                } catch { }
                

                foreach (DataRow row in dv.Rows)
                {
                    if ("txtBankKey" == row["clientidtextlabel"].ToString()) { BANKKEY_IN = data.BANKKEY ?? ""; }
                    if ("txtBankAccount" == row["clientidtextlabel"].ToString()) { BANKACCOUNT_IN = data.BANKACCOUNT ?? "".Trim(); }
                    if ("ddlAccountHolder" == row["clientidtextlabel"].ToString()) { ACCOUNTHOLDER_IN = data.ACCOUNTHOLDER ?? "".Trim(); }
                    if ("ddlBankControlKey" == row["clientidtextlabel"].ToString()) { BANKCONTROLKEY_IN = data.BANKCONTROLKEY ?? ""; }
                    if ("txtBankName" == row["clientidtextlabel"].ToString()) { BANKNAME_IN = data.BANKNAME ?? "".Trim(); }
                    if ("txtBankStreet" == row["clientidtextlabel"].ToString()) { BANKSTREET_IN = data.BANKSTREET ?? "".Trim(); }
                    if ("txtBankCity" == row["clientidtextlabel"].ToString()) { BANKCITY_IN = data.BANKCITY ?? "".Trim(); }
                    if ("txtBankBranch" == row["clientidtextlabel"].ToString()) { BANKBRANCH_IN = data.BANKBRANCH ?? "".Trim(); }
                    if ("ddlBankControlKey" == row["clientidtextlabel"].ToString()) { BANKCONTROLKEY_IN = data.BANKCONTROLKEY ?? "".Trim(); }
                    if ("ddlCtry" == row["clientidtextlabel"].ToString()) { CTRY_IN = data.CTRY ?? ""; }
                    if ("ddlBankRegion" == row["clientidtextlabel"].ToString()) { BANKREGION_IN = data.BANKREGION ?? "".Trim(); }
                    if ("ddlBankCurrency" == row["clientidtextlabel"].ToString()) { BANKCURRENCY_IN = data.BANKCURRENCY ?? "".Trim(); }
                }
                
                // result = oMasterQueries.SaveBANKData(USERID, cmdid1, cmhid, rdlRequest.SelectedValue, rdlRequestType.SelectedValue, txtCompanyCode.Text, ac_type, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, "", "", "", "", "", "", "", "", "", "", "", "", email_id, ref headerid, ref detailid);
                string result = oMasterQueries.SaveBANKData(USERID, cmdid1, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, BANKKEY_IN, BANKACCOUNT_IN, ACCOUNTHOLDER_IN, BANKCONTROLKEY_IN, BANKNAME_IN, BANKREGION_IN, BANKSTREET_IN, BANKCITY_IN, BANKBRANCH_IN, BANKCURRENCY_IN, BANK_MANDATE_FILE_IN, BANK_CANCELED_FILE_IN, CTRY_IN, email_id, ref headerid, ref detailid);

                if (result == "1")
                {
                    res.CMHEADERID = Convert.ToInt32(headerid);
                    res.CMDEAILID1 = Convert.ToInt32(detailid);
                    res.RS = Convert.ToInt32(result);
                    data.CMHEADERID = Convert.ToInt64(headerid);
                }
                else
                {
                    res.MESSAGE = "Error in process of data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveGeneral");
                    return res;
                }
                if (data.CUST_ACC_TYPE1 == "5AGW" && result == "1")
                {

                        BANKKEY_IN = ""; BANKACCOUNT_IN = ""; ACCOUNTHOLDER_IN = ""; BANKCONTROLKEY_IN = ""; BANKNAME_IN = ""; BANKREGION_IN = ""; BANKSTREET_IN = ""; BANKCITY_IN = "";
                        BANKBRANCH_IN = ""; BANKCURRENCY_IN = ""; CTRY_IN = "";

                    // dv1 = dv.Select(" group_name='BankDetails' And " + "ag_" + ac_55AGW + "=1").CopyToDataTable();
                    try {
                        dv = objDs.Tables[1].Select(" group_name='BankDetails' And " + "ag_" + data.CUST_ACC_TYPE1 + "=1").CopyToDataTable();
                    } catch {
                        dv = new DataTable();
                    }
                        
                    foreach (DataRow row1 in dv.Rows)
                    {
                            if ("txtBankKey" == row1["clientidtextlabel"].ToString()) { BANKKEY_IN = data.BANKKEY??""; }
                            if ("txtBankAccount" == row1["clientidtextlabel"].ToString()) { BANKACCOUNT_IN = data.BANKACCOUNT ?? "".Trim(); }
                            if ("ddlAccountHolder" == row1["clientidtextlabel"].ToString()) { ACCOUNTHOLDER_IN = data.ACCOUNTHOLDER ?? "".Trim(); }
                            if ("ddlBankControlKey" == row1["clientidtextlabel"].ToString()) { BANKCONTROLKEY_IN = data.BANKCONTROLKEY ?? ""; }
                            if ("txtBankName" == row1["clientidtextlabel"].ToString()) { BANKNAME_IN = data.BANKNAME ?? "".Trim(); }
                            if ("txtBankStreet" == row1["clientidtextlabel"].ToString()) { BANKSTREET_IN = data.BANKSTREET ?? "".Trim(); }
                            if ("txtBankCity" == row1["clientidtextlabel"].ToString()) { BANKCITY_IN = data.BANKCITY ?? "".Trim(); }
                            if ("txtBankBranch" == row1["clientidtextlabel"].ToString()) { BANKBRANCH_IN = data.BANKBRANCH ?? "".Trim(); }
                            if ("ddlBankControlKey" == row1["clientidtextlabel"].ToString()) { BANKCONTROLKEY_IN = data.BANKCONTROLKEY ?? "".Trim(); }
                            if ("ddlCtry" == row1["clientidtextlabel"].ToString()) { CTRY_IN = data.CTRY ?? ""; }
                            if ("ddlBankRegion" == row1["clientidtextlabel"].ToString()) { BANKREGION_IN = data.BANKREGION ?? "".Trim(); }
                            if ("ddlBankCurrency" == row1["clientidtextlabel"].ToString()) { BANKCURRENCY_IN = data.BANKCURRENCY ?? "".Trim(); }
                        }

                    cmhid = data.CMHEADERID ?? 0;
                        // result = oMasterQueries.SaveBANKData(USERID, cmdid2, cmhid, rdlRequest.SelectedValue, rdlRequestType.SelectedValue, txtCompanyCode.Text, ac_55AGW, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, txtBankKey.Text.ToUpper(), txtBankAccount.Text.Trim(), txtAccountHolder.Text.ToUpper(), txtBankControlKey.Text.ToUpper(), txtBankName.Text.ToUpper(), txtBankRegion.Text.ToUpper(), txtBankStreet.Text.ToUpper(), txtBankCity.Text.ToUpper(), txtBankBranch.Text.ToUpper(), ddlBankCurrency.Text.ToUpper(), BANK_MANDATE_FILE_IN, BANK_CANCELED_FILE_IN, email_id, ref headerid, ref detailid);
                        result = oMasterQueries.SaveBANKData(USERID, cmdid2, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE1, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, BANKKEY_IN, BANKACCOUNT_IN, ACCOUNTHOLDER_IN, BANKCONTROLKEY_IN, BANKNAME_IN, BANKREGION_IN, BANKSTREET_IN, BANKCITY_IN, BANKBRANCH_IN, BANKCURRENCY_IN, BANK_MANDATE_FILE_IN, BANK_CANCELED_FILE_IN, CTRY_IN, email_id, ref headerid, ref detailid);

                    }
                if (result == "1")
                {
                    res.CMDEAILID2 = Convert.ToInt32(detailid);
                    res.MESSAGE = "Successfully saved!";                    
                    res.VIEWSTATE = "Y";
                  
                    if (BANK_MANDATE_FILE_IN != "")
                    {
                        res.FILE1 = BANK_MANDATE_FILE_IN;
                        //hyperBankMandate.Visible = true;
                        //hyperBankMandate.OnClientClick = "javascript:openDialog('../../Uploads/CustomerMaster/" + hyperBankMandate.Text + "',980,680);";
                    }
                    if (BANK_CANCELED_FILE_IN != "")
                    {
                        res.FILE2 = BANK_CANCELED_FILE_IN;
                        //hyperCancelCheque.Visible = true;
                        //hyperCancelCheque.OnClientClick = "javascript:openDialog('../../Uploads/CustomerMaster/" + hyperCancelCheque.Text + "',980,680);";
                    }

                }
                else
                {
                    //ShowError("Error in process of data save!");
                    //btnSaveGenData.ToolTip = result;
                    res.MESSAGE = "Error in process of data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveGeneral");
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SaveBankData");
                res.MESSAGE = "Error in process of bank data save!";
                res.RS = 0;
                return res;
            }

        }
        public CustomerResponseData SaveIndustryData(CustomerMasterDetail data, string email_id)
        {
            CustomerResponseData res = new CustomerResponseData();
            try
            {
                DataSet objDs = oMasterQueries.GetCustomerMandatoryData("", "", "", "", "", "", "", "", "");

                DataTable dv=new DataTable();
                //var str = _session.Get<string>("vds");                
                //dt = JsonConvert.DeserializeObject<DataTable>(str);              
                string headerid = "", detailid = "";
                long cmhid = 0, cmdid1 = 0, cmdid2 = 0, USERID = 0;
                string INDUSTRY1_IN = "", INDUSTRYCODE1_IN = "", INDUSTRYNAME_IN = "", FIRSTNAME_IN = "";

                cmhid = data.CMHEADERID ?? 0;
                cmdid1 = data.CMDETAILID ?? 0;
                cmdid2 = data.CMDETAILID1 ?? 0;
                USERID = data.REQUESTEDID ?? 0;
                // dv1 = dv.Select(" group_name='IndustryDetails' And " + "ag_" + ac_type + "=1").CopyToDataTable();
                try {
                    dv = objDs.Tables[1].Select(" group_name='IndustryDetails' And " + "ag_" + data.CUST_ACC_TYPE + "=1").CopyToDataTable();
                } catch { }
                

                foreach (DataRow row in dv.Rows)
                {
                    if ("txtIndustry1" == row["clientidtextlabel"].ToString()) { INDUSTRY1_IN = data.INDUSTRY1??""; }
                    if ("txtIndustryCode1" == row["clientidtextlabel"].ToString()) { INDUSTRYCODE1_IN = data.INDUSTRYCODE1??"".Trim(); }
                    if ("txtIndustryName" == row["clientidtextlabel"].ToString()) { INDUSTRYNAME_IN =data.INDUSTRYNAME??"".Trim(); }
                    if ("txtFirstName" == row["clientidtextlabel"].ToString()) { FIRSTNAME_IN = data.FIRSTNAME??"".Trim(); }
                }

                
                // result = oMasterQueries.SaveIndustryData(USERID, cmdid1, cmhid, ac_type, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, "", "", "", "", "", "", "", "", "", "", "", "", email_id, ref headerid, ref detailid);
                string result = oMasterQueries.SaveIndustryData(USERID, cmdid1, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, INDUSTRY1_IN, INDUSTRYCODE1_IN, INDUSTRYNAME_IN, FIRSTNAME_IN, email_id, ref headerid, ref detailid);

                if (result == "1")
                {
                    res.CMHEADERID = Convert.ToInt32(headerid);
                    res.CMDEAILID1 = Convert.ToInt32(detailid);
                    res.RS = Convert.ToInt32(result);
                    data.CMHEADERID = Convert.ToInt64(headerid);
                }
                else
                {
                    res.MESSAGE = "Error in process of data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveIndustryData");
                }
                if (data.CUST_ACC_TYPE1 == "5AGW" && result == "1")
                {

                    INDUSTRY1_IN = ""; INDUSTRYCODE1_IN = ""; INDUSTRYNAME_IN = ""; FIRSTNAME_IN = "";

                    // dv1 = dv.Select(" group_name='IndustryDetails' And " + "ag_" + ac_55AGW + "=1").CopyToDataTable();
                    try {
                        dv = objDs.Tables[1].Select(" group_name='IndustryDetails' And " + "ag_" + data.CUST_ACC_TYPE1 + "=1").CopyToDataTable();
                    } catch {
                        dv = new DataTable();
                    }
                    
                    foreach (DataRow row1 in dv.Rows)
                    {
                        if ("txtIndustry1" == row1["clientidtextlabel"].ToString()) { INDUSTRY1_IN = data.INDUSTRY1 ?? ""; }
                        if ("txtIndustryCode1" == row1["clientidtextlabel"].ToString()) { INDUSTRYCODE1_IN = data.INDUSTRYCODE1 ?? "".Trim(); }
                        if ("txtIndustryName" == row1["clientidtextlabel"].ToString()) { INDUSTRYNAME_IN = data.INDUSTRYNAME ?? "".Trim(); }
                        if ("txtFirstName" == row1["clientidtextlabel"].ToString()) { FIRSTNAME_IN = data.FIRSTNAME ?? "".Trim(); }
                    }

                    cmhid = data.CMHEADERID ?? 0;                    
                    result = oMasterQueries.SaveIndustryData(USERID, cmdid2, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE1, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, INDUSTRY1_IN, INDUSTRYCODE1_IN, INDUSTRYNAME_IN, FIRSTNAME_IN, email_id, ref headerid, ref detailid);
                }
                if (result == "1")
                {
                    res.CMDEAILID2 = Convert.ToInt32(detailid);
                    res.MESSAGE = "Successfully saved!";
                    res.VIEWSTATE = "Y";              

                }
                else
                {
                    //ShowError("Error in process of data save!");
                    //btnSaveGenData.ToolTip = result;
                    res.MESSAGE = "Error in process of data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveIndustryData");
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SaveIndustryData");
                res.MESSAGE = "Error in process of Industry data save!";
                res.RS = 0;
                return res;
            }

        }
        public CustomerResponseData SaveSaleData(CustomerMasterDetail data, string email_id)
        {
            CustomerResponseData res = new CustomerResponseData();
            try
            {
                DataSet objDs = oMasterQueries.GetCustomerMandatoryData("", "", "", "", "", "", "", "", "");
                DataTable dv;                      
                string headerid = "", detailid = "";
                long cmhid = 0, cmdid1 = 0, cmdid2 = 0, USERID = 0;
                string SALESCURRENCY_IN = "", EXCHANGERATETYPE_IN = "", SALESDISTRICT_IN = "", SALESOFFICE_IN = "", SALESGROUP_IN = "", CUSTOMERGROUP_IN = "", PRICEGROUP_IN = "", CUSTPRICPROC1_IN = "",
DELIVERYPRIORITYGROUP_IN = "", SHIPPINGCONDITION_IN = "";
                cmhid = data.CMHEADERID ?? 0;
                cmdid1 = data.CMDETAILID ?? 0;
                cmdid2 = data.CMDETAILID1 ?? 0;
                USERID = data.REQUESTEDID ?? 0;
                //dv1 = dv.Select(" group_name='SalesAreaData' And " + "ag_" + ac_type + "=1").CopyToDataTable();
                try {
                    dv = objDs.Tables[1].Select(" group_name='SalesAreaData' And " + "ag_" + data.CUST_ACC_TYPE + "=1").CopyToDataTable();
                } catch
                {
                    dv = new DataTable();
                }
                

                foreach (DataRow row in dv.Rows)
                {
                    if ("ddlExchangeRateType" == row["clientidtextlabel"].ToString()) { EXCHANGERATETYPE_IN = data.EXCHANGERATETYPE??""; }
                    if ("ddlPriceGroup" == row["clientidtextlabel"].ToString()) { PRICEGROUP_IN = data.PRICEGROUP??""; }
                    if ("ddlSalesDistrict" == row["clientidtextlabel"].ToString()) { SALESDISTRICT_IN = data.SALESDISTRICT??""; }
                    if ("ddlSalesOffice" == row["clientidtextlabel"].ToString()) { SALESOFFICE_IN = data.SALESOFFICE??""; }
                    if ("ddlSalesGroup" == row["clientidtextlabel"].ToString()) { SALESGROUP_IN = data.SALESGROUP??""; }
                    if ("ddlCustomerGroup" == row["clientidtextlabel"].ToString()) { CUSTOMERGROUP_IN = data.CUSTOMERGROUP??""; }
                    if ("ddlSalesCurrency" == row["clientidtextlabel"].ToString()) { SALESCURRENCY_IN = data.SALESCURRENCY??""; }
                    if ("ddlCustomerPrice" == row["clientidtextlabel"].ToString()) { CUSTPRICPROC1_IN = data.CUSTPRICPROC1??""; }
                    if ("ddlDeliveryPriority" == row["clientidtextlabel"].ToString()) { DELIVERYPRIORITYGROUP_IN = data.DELIVERYPRIORITYGROUP??""; }
                    if ("ddlShippingConditions" == row["clientidtextlabel"].ToString()) { SHIPPINGCONDITION_IN = data.SHIPPINGCONDITION??""; }
                }

                //result = oMasterQueries.SaveSalesData(USERID, cmdid1, cmhid, rdlRequest.SelectedValue, rdlRequestType.SelectedValue, txtCompanyCode.Text, ac_type, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, ddlSalesCurrency.Text.Trim(), ddlExchangeRateType.Text, ddlSalesDistrict.SelectedValue, ddlSalesOffice.SelectedValue, ddlSalesGroup.SelectedValue, ddlCustomerGroup.SelectedValue, ddlPriceGroup.Text, ddlCustomerPrice.SelectedValue, ddlDeliveryPriority.SelectedValue, ddlShippingConditions.SelectedValue, email_id, ref headerid, ref detailid);
               string result = oMasterQueries.SaveSalesData(USERID, cmdid1, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, SALESCURRENCY_IN, EXCHANGERATETYPE_IN, SALESDISTRICT_IN, SALESOFFICE_IN, SALESGROUP_IN, CUSTOMERGROUP_IN, PRICEGROUP_IN, CUSTPRICPROC1_IN, DELIVERYPRIORITYGROUP_IN, SHIPPINGCONDITION_IN, email_id, ref headerid, ref detailid);
                if (result == "1")
                {
                    res.CMHEADERID = Convert.ToInt32(headerid);
                    res.CMDEAILID1 = Convert.ToInt32(detailid);
                    res.RS = Convert.ToInt32(result);
                    data.CMHEADERID = Convert.ToInt64(headerid);
                }
                else
                {
                    res.MESSAGE = "Error in process of data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveSaleData");
                }
                if (data.CUST_ACC_TYPE1 == "5AGW" && result == "1")
                {

                    SALESCURRENCY_IN = ""; EXCHANGERATETYPE_IN = ""; SALESDISTRICT_IN = ""; SALESOFFICE_IN = ""; SALESGROUP_IN = ""; CUSTOMERGROUP_IN = ""; PRICEGROUP_IN = ""; CUSTPRICPROC1_IN = ""; DELIVERYPRIORITYGROUP_IN = ""; SHIPPINGCONDITION_IN = "";

                    // dv1 = dv.Select(" group_name='SalesAreaData' And " + "ag_" + ac_55AGW + "=1").CopyToDataTable();
                    try {
                        dv = objDs.Tables[1].Select(" group_name='SalesAreaData' And " + "ag_" + data.CUST_ACC_TYPE1 + "=1").CopyToDataTable();
                    } catch
                    {
                        dv=new DataTable();
                    }
                  
                    foreach (DataRow row1 in dv.Rows)
                    {
                        if ("ddlExchangeRateType" == row1["clientidtextlabel"].ToString()) { EXCHANGERATETYPE_IN = data.EXCHANGERATETYPE ?? ""; }
                        if ("ddlPriceGroup" == row1["clientidtextlabel"].ToString()) { PRICEGROUP_IN = data.PRICEGROUP ?? ""; }
                        if ("ddlSalesDistrict" == row1["clientidtextlabel"].ToString()) { SALESDISTRICT_IN = data.SALESDISTRICT ?? ""; }
                        if ("ddlSalesOffice" == row1["clientidtextlabel"].ToString()) { SALESOFFICE_IN = data.SALESOFFICE ?? ""; }
                        if ("ddlSalesGroup" == row1["clientidtextlabel"].ToString()) { SALESGROUP_IN = data.SALESGROUP ?? ""; }
                        if ("ddlCustomerGroup" == row1["clientidtextlabel"].ToString()) { CUSTOMERGROUP_IN = data.CUSTOMERGROUP ?? ""; }
                        if ("ddlSalesCurrency" == row1["clientidtextlabel"].ToString()) { SALESCURRENCY_IN = data.SALESCURRENCY ?? ""; }
                        if ("ddlCustomerPrice" == row1["clientidtextlabel"].ToString()) { CUSTPRICPROC1_IN = data.CUSTPRICPROC1 ?? ""; }
                        if ("ddlDeliveryPriority" == row1["clientidtextlabel"].ToString()) { DELIVERYPRIORITYGROUP_IN = data.DELIVERYPRIORITYGROUP ?? ""; }
                        if ("ddlShippingConditions" == row1["clientidtextlabel"].ToString()) { SHIPPINGCONDITION_IN = data.SHIPPINGCONDITION ?? ""; }
                    }

                    cmhid = data.CMHEADERID ?? 0;                   
                    //  result = oMasterQueries.SaveSalesData(USERID, cmdid2, cmhid, rdlRequest.SelectedValue, rdlRequestType.SelectedValue, txtCompanyCode.Text, ac_55AGW, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, ddlSalesCurrency.Text.Trim(), ddlExchangeRateType.Text, ddlSalesDistrict.SelectedValue, ddlSalesOffice.SelectedValue, ddlSalesGroup.SelectedValue, ddlCustomerGroup.SelectedValue, ddlPriceGroup.Text, ddlCustomerPrice.SelectedValue, ddlDeliveryPriority.SelectedValue, ddlShippingConditions.SelectedValue, email_id, ref headerid, ref detailid);
                    result = oMasterQueries.SaveSalesData(USERID, cmdid2, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE1, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, SALESCURRENCY_IN, EXCHANGERATETYPE_IN, SALESDISTRICT_IN, SALESOFFICE_IN, SALESGROUP_IN, CUSTOMERGROUP_IN, PRICEGROUP_IN, CUSTPRICPROC1_IN, DELIVERYPRIORITYGROUP_IN, SHIPPINGCONDITION_IN, email_id, ref headerid, ref detailid);
                }
                if (result == "1")
                {
                    res.CMDEAILID2 = Convert.ToInt32(detailid);
                    res.MESSAGE = "Successfully saved!";
                    res.VIEWSTATE = "Y";

                }
                else
                {
                    //ShowError("Error in process of data save!");
                    //btnSaveGenData.ToolTip = result;
                    res.MESSAGE = "Error in process of data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveSaleData");
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SaveSaleData");
                res.MESSAGE = "Error in process of Sales data save!";
                res.RS = 0;
                return res;
            }

        }
        public CustomerResponseData SaveCINData(CustomerMasterDetail data, string CIN_PAN_NO_FILE_IN, string CIN_GST_NO_FILE_IN,string CIN_CIN_DOC_FILE_IN, string email_id)
        {
            CustomerResponseData res = new CustomerResponseData();
            try
            {
                DataSet objDs = oMasterQueries.GetCustomerMandatoryData("", "", "", "", "", "", "", "", "");
                DataTable dv;              
                //var str = _session.Get<string>("vds");                
                //dt = JsonConvert.DeserializeObject<DataTable>(str);              
                string headerid = "", detailid = "";
                long cmhid = 0, cmdid1 = 0, cmdid2 = 0, USERID = 0;
                string CSTNO_IN = "", LSTNO_IN = "", INVOICINGDATES_IN = "", INVOICINGLISTDATES_IN = "", SERREGNO_IN = "", PANNUMBER_IN = "", PAYMENTGUARANTEEPROC_IN = "",
                    E_INVOICE_APPLICABLE="";
                cmhid = data.CMHEADERID ?? 0;
                cmdid1 = data.CMDETAILID ?? 0;
                cmdid2 = data.CMDETAILID1 ?? 0;
                USERID = data.REQUESTEDID ?? 0;
                // dv1 = dv.Select(" group_name='CINDetails' And " + "ag_" + ac_type + "=1").CopyToDataTable();
                dv = objDs.Tables[1].Select(" group_name='CINDetails' And " + "ag_" + data.CUST_ACC_TYPE + "=1").CopyToDataTable();

                foreach (DataRow row in dv.Rows)
                {
                    if ("txtCstno" == row["clientidtextlabel"].ToString()) { CSTNO_IN = data.CSTNO??"".Trim(); }
                    if ("txtLstNo" == row["clientidtextlabel"].ToString()) { LSTNO_IN = data.LSTNO??"".Trim(); }
                    if ("txtInvoicingDates" == row["clientidtextlabel"].ToString()) { INVOICINGDATES_IN = data.INVOICINGDATES??"".Trim(); }
                    if ("txtInvoicingListDates" == row["clientidtextlabel"].ToString()) { INVOICINGLISTDATES_IN = data.INVOICINGLISTDATES??"".Trim(); }
                    if ("txtServiceRegNo" == row["clientidtextlabel"].ToString()) { SERREGNO_IN = data.SERREGNO??"".Trim(); }
                    if ("txtPanNumber" == row["clientidtextlabel"].ToString()) { PANNUMBER_IN = data.PANNUMBER??"".Trim(); }
                    if ("txtPaymentGuarProc" == row["clientidtextlabel"].ToString()) { PAYMENTGUARANTEEPROC_IN = data.PAYMENTGUARANTEEPROC??"".Trim(); }
                    E_INVOICE_APPLICABLE = data.E_INVOICE_APPLICABLE??"";
                }
                                
                //result = oMasterQueries.SaveCINData(USERID, cmdid1, cmhid, rdlRequest.SelectedValue, rdlRequestType.SelectedValue, txtCompanyCode.Text, ac_type, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, CSTNO_IN, LSTNO_IN, INVOICINGDATES_IN, INVOICINGLISTDATES_IN, SERREGNO_IN, PANNUMBER_IN, PAYMENTGUARANTEEPROC_IN, CIN_PAN_NO_FILE_IN, CIN_GST_NO_FILE_IN, email_id, ref headerid, ref detailid);
                string result = oMasterQueries.SaveCINData(USERID, cmdid1, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, CSTNO_IN, LSTNO_IN, INVOICINGDATES_IN, INVOICINGLISTDATES_IN, SERREGNO_IN, PANNUMBER_IN, PAYMENTGUARANTEEPROC_IN, E_INVOICE_APPLICABLE, CIN_PAN_NO_FILE_IN, CIN_GST_NO_FILE_IN, CIN_CIN_DOC_FILE_IN, email_id, ref headerid, ref detailid);
                if (result == "1")
                {
                    res.CMHEADERID = Convert.ToInt32(headerid);
                    res.CMDEAILID1 = Convert.ToInt32(detailid);
                    res.RS = Convert.ToInt32(result);
                    data.CMHEADERID = Convert.ToInt64(headerid);
                }
                else
                {
                    res.MESSAGE = "Error in process of CIN data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveCINData");
                }
                if (data.CUST_ACC_TYPE1 == "5AGW" && result == "1")
                {
                    CSTNO_IN = ""; LSTNO_IN = ""; INVOICINGDATES_IN = ""; INVOICINGLISTDATES_IN = ""; SERREGNO_IN = ""; PANNUMBER_IN = ""; PAYMENTGUARANTEEPROC_IN = "";
                    E_INVOICE_APPLICABLE = "";

                    // dv1 = dv.Select(" group_name='CINDetails' And " + "ag_" + ac_55AGW + "=1").CopyToDataTable();
                    dv = objDs.Tables[1].Select(" group_name='CINDetails' And " + "ag_" + data.CUST_ACC_TYPE1 + "=1").CopyToDataTable();
                    foreach (DataRow row1 in dv.Rows)
                    {
                        if ("txtCstno" == row1["clientidtextlabel"].ToString()) { CSTNO_IN = data.CSTNO ?? "".Trim(); }
                        if ("txtLstNo" == row1["clientidtextlabel"].ToString()) { LSTNO_IN = data.LSTNO ?? "".Trim(); }
                        if ("txtInvoicingDates" == row1["clientidtextlabel"].ToString()) { INVOICINGDATES_IN = data.INVOICINGDATES ?? "".Trim(); }
                        if ("txtInvoicingListDates" == row1["clientidtextlabel"].ToString()) { INVOICINGLISTDATES_IN = data.INVOICINGLISTDATES ?? "".Trim(); }
                        if ("txtServiceRegNo" == row1["clientidtextlabel"].ToString()) { SERREGNO_IN = data.SERREGNO ?? "".Trim(); }
                        if ("txtPanNumber" == row1["clientidtextlabel"].ToString()) { PANNUMBER_IN = data.PANNUMBER ?? "".Trim(); }
                        if ("txtPaymentGuarProc" == row1["clientidtextlabel"].ToString()) { PAYMENTGUARANTEEPROC_IN = data.PAYMENTGUARANTEEPROC ?? "".Trim(); }
                        E_INVOICE_APPLICABLE = data.E_INVOICE_APPLICABLE ?? "";
                    }

                    cmhid = data.CMHEADERID ?? 0;                    
                    //result = oMasterQueries.SaveCINData(USERID, cmdid2, cmhid, rdlRequest.SelectedValue, rdlRequestType.SelectedValue, txtCompanyCode.Text, ac_55AGW, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, CSTNO_IN, LSTNO_IN, INVOICINGDATES_IN, INVOICINGLISTDATES_IN, SERREGNO_IN, PANNUMBER_IN, PAYMENTGUARANTEEPROC_IN, CIN_PAN_NO_FILE_IN, CIN_GST_NO_FILE_IN, email_id, ref headerid, ref detailid);
                    result = oMasterQueries.SaveCINData(USERID, cmdid2, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE1, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, CSTNO_IN, LSTNO_IN, INVOICINGDATES_IN, INVOICINGLISTDATES_IN, SERREGNO_IN, PANNUMBER_IN, PAYMENTGUARANTEEPROC_IN, E_INVOICE_APPLICABLE, CIN_PAN_NO_FILE_IN, CIN_GST_NO_FILE_IN, CIN_CIN_DOC_FILE_IN, email_id, ref headerid, ref detailid);

                }
                if (result == "1")
                {
                    res.CMDEAILID2 = Convert.ToInt32(detailid);
                    res.MESSAGE = "Successfully saved!";
                    res.VIEWSTATE = "Y";

                    if (CIN_PAN_NO_FILE_IN != "")
                    {
                        res.FILE1 = CIN_PAN_NO_FILE_IN;
                        //hyperBankMandate.Visible = true;
                        //hyperBankMandate.OnClientClick = "javascript:openDialog('../../Uploads/CustomerMaster/" + hyperBankMandate.Text + "',980,680);";
                    }
                    if (CIN_CIN_DOC_FILE_IN != "")
                    {
                        res.FILE2 = CIN_CIN_DOC_FILE_IN;
                        //hyperCancelCheque.Visible = true;
                        //hyperCancelCheque.OnClientClick = "javascript:openDialog('../../Uploads/CustomerMaster/" + hyperCancelCheque.Text + "',980,680);";
                    }

                }
                else
                {
                    //ShowError("Error in process of data save!");
                    //btnSaveGenData.ToolTip = result;
                    res.MESSAGE = "Error in process of data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveGeneral");
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SaveBankData");
                res.MESSAGE = "Error in process of bank data save!";
                res.RS = 0;
                return res;
            }

        }
        public CustomerResponseData SaveCompanyData(CustomerMasterDetail data, string COMP_RTO_FILE_IN, string COMP_LOI_FILE_IN,string OTHER_DOC_FILE_IN, string email_id)
        {
            CustomerResponseData res = new CustomerResponseData();
            try
            {
                DataSet objDs = oMasterQueries.GetCustomerMandatoryData("", "", "", "", "", "", "", "", "");
                DataTable dv;
               
                //var str = _session.Get<string>("vds");                
                //dt = JsonConvert.DeserializeObject<DataTable>(str);              
                string headerid = "", detailid = "";
                long cmhid = 0, cmdid1 = 0, cmdid2 = 0, USERID = 0;
                string RECONSACCOUNT_IN = "", TERMOFPAYMENT_IN = "", PAYMENTMETHODS_IN = "", HOUSEBANK_IN = "", PMTMETHSUPL_IN = "", WITHHOLDINGTAXTYPE_IN = "", WITHHOLDINGTAXCODE_IN = "", VALIDFROM_IN = "",
VALIDTO_IN = "", WITHHOLDINGTAXNUMBER_IN = "", INCOTERM1_IN = "", INCOTERM2_IN = "", CREDITCONTROLAREA_IN = "", ACCASSIGMENTGROUP_IN = "", TAXCLASSIFICATION_IN = "";

                cmhid = data.CMHEADERID ?? 0;
                cmdid1 = data.CMDETAILID ?? 0;
                cmdid2 = data.CMDETAILID1 ?? 0;
                USERID = data.REQUESTEDID ?? 0;
                //  dv1 = dv.Select(" group_name='CompanyCodeData' And " + "ag_" + ac_type + "=1").CopyToDataTable();
                dv = objDs.Tables[1].Select(" group_name='CompanyCodeData' And " + "ag_" + data.CUST_ACC_TYPE + "=1").CopyToDataTable();

                foreach (DataRow row in dv.Rows)
                {
                    if ("txtPaymentMethods" == row["clientidtextlabel"].ToString()) { PAYMENTMETHODS_IN = data.PAYMENTMETHODS??"".Trim(); }
                    if ("txtHouseBank" == row["clientidtextlabel"].ToString()) { HOUSEBANK_IN = data.HOUSEBANK??"".Trim(); }
                    if ("txtPaymentMethodSupplier" == row["clientidtextlabel"].ToString()) { PMTMETHSUPL_IN = data.PMTMETHSUPL??"".Trim(); }
                    if ("txtWithHoldingTaxType" == row["clientidtextlabel"].ToString()) { WITHHOLDINGTAXTYPE_IN = data.WITHHOLDINGTAXTYPE??"".Trim(); }
                    if ("txtWithHoldingTaxCode" == row["clientidtextlabel"].ToString()) { WITHHOLDINGTAXCODE_IN = data.WITHHOLDINGTAXCODE??"".Trim(); }
                    if ("txtValidFrom" == row["clientidtextlabel"].ToString()) { VALIDFROM_IN = data.VALIDFROM??"".Trim(); }
                    if ("txtValidTo" == row["clientidtextlabel"].ToString()) { VALIDTO_IN = data.VALIDTO??"".Trim(); }
                    if ("txtWithHoldingTaxNumber" == row["clientidtextlabel"].ToString()) { WITHHOLDINGTAXNUMBER_IN = data.WITHHOLDINGTAXNUMBER??"".Trim(); }
                    if ("ddlReconAccount" == row["clientidtextlabel"].ToString()) { RECONSACCOUNT_IN = data.RECONSACCOUNT??""; }
                    if ("ddlTermsOfPayment" == row["clientidtextlabel"].ToString()) { TERMOFPAYMENT_IN = data.TERMOFPAYMENT??""; }
                    if ("ddlIncoterms" == row["clientidtextlabel"].ToString()) { INCOTERM1_IN = data.INCOTERM1??""; }
                    if ("ddlIncoterms" == row["clientidtextlabel"].ToString()) { INCOTERM2_IN = data.INCOTERM2??""; }
                    if ("ddlTaxClassification" == row["clientidtextlabel"].ToString()) { TAXCLASSIFICATION_IN = data.TAXCLASSIFICATION?? ""; }
                    if ("ddlAccountAssignmentGroup" == row["clientidtextlabel"].ToString()) { ACCASSIGMENTGROUP_IN = data.ACCASSIGMENTGROUP?? ""; }
                    if ("ddlCreditControlArea" == row["clientidtextlabel"].ToString()) { CREDITCONTROLAREA_IN = data.CREDITCONTROLAREA?? ""; }
                }

               //result = oMasterQueries.SaveCompanyData(USERID, cmdid1, cmhid, rdlRequest.SelectedValue, rdlRequestType.SelectedValue, txtCompanyCode.Text, ac_type, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, ddlReconAccount.Text.Trim(), ddlTermsOfPayment.SelectedValue, txtPaymentMethods.Text.Trim(), txtHouseBank.Text, txtPaymentMethodSupplier.Text, txtWithHoldingTaxType.Text, txtWithHoldingTaxCode.Text, txtValidFrom.Text, txtValidTo.Text, txtWithHoldingTaxNumber.Text, ddlIncoterms.SelectedValue, ddlIncoterms.SelectedItem.Text, ddlCreditControlArea.SelectedValue, ddlAccountAssignmentGroup.SelectedValue, ddlTaxClassification.Text, "", "", email_id, ref headerid, ref detailid);
               string result = oMasterQueries.SaveCompanyData(USERID, cmdid1, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, RECONSACCOUNT_IN,
TERMOFPAYMENT_IN, PAYMENTMETHODS_IN, HOUSEBANK_IN, PMTMETHSUPL_IN, WITHHOLDINGTAXTYPE_IN, WITHHOLDINGTAXCODE_IN, VALIDFROM_IN,
VALIDTO_IN, WITHHOLDINGTAXNUMBER_IN, INCOTERM1_IN, INCOTERM2_IN, CREDITCONTROLAREA_IN, ACCASSIGMENTGROUP_IN, TAXCLASSIFICATION_IN, COMP_RTO_FILE_IN, COMP_LOI_FILE_IN, email_id, OTHER_DOC_FILE_IN, ref headerid, ref detailid);
                if (result == "1")
                {
                    res.CMHEADERID = Convert.ToInt32(headerid);
                    res.CMDEAILID1 = Convert.ToInt32(detailid);
                    res.RS = Convert.ToInt32(result);
                    data.CMHEADERID = Convert.ToInt64(headerid);
                }
                else
                {
                    res.MESSAGE = "Error in process of Company data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveCompanyData");
                }
                if (data.CUST_ACC_TYPE1 == "5AGW" && result == "1")
                {
                    RECONSACCOUNT_IN = ""; TERMOFPAYMENT_IN = ""; PAYMENTMETHODS_IN = ""; HOUSEBANK_IN = ""; PMTMETHSUPL_IN = ""; WITHHOLDINGTAXTYPE_IN = ""; WITHHOLDINGTAXCODE_IN = ""; VALIDFROM_IN = ""; VALIDTO_IN = ""; WITHHOLDINGTAXNUMBER_IN = ""; INCOTERM1_IN = ""; INCOTERM2_IN = ""; CREDITCONTROLAREA_IN = ""; ACCASSIGMENTGROUP_IN = ""; TAXCLASSIFICATION_IN = "";

                    // dv1 = dv.Select(" group_name='CompanyCodeData' And " + "ag_" + ac_55AGW + "=1").CopyToDataTable();
                    dv = objDs.Tables[1].Select(" group_name='CompanyCodeData' And " + "ag_" + data.CUST_ACC_TYPE1 + "=1").CopyToDataTable();
                    foreach (DataRow row1 in dv.Rows)
                    {
                        if ("txtPaymentMethods" == row1["clientidtextlabel"].ToString()) { PAYMENTMETHODS_IN = data.PAYMENTMETHODS ?? "".Trim(); }
                        if ("txtHouseBank" == row1["clientidtextlabel"].ToString()) { HOUSEBANK_IN = data.HOUSEBANK ?? "".Trim(); }
                        if ("txtPaymentMethodSupplier" == row1["clientidtextlabel"].ToString()) { PMTMETHSUPL_IN = data.PMTMETHSUPL ?? "".Trim(); }
                        if ("txtWithHoldingTaxType" == row1["clientidtextlabel"].ToString()) { WITHHOLDINGTAXTYPE_IN = data.WITHHOLDINGTAXTYPE ?? "".Trim(); }
                        if ("txtWithHoldingTaxCode" == row1["clientidtextlabel"].ToString()) { WITHHOLDINGTAXCODE_IN = data.WITHHOLDINGTAXCODE ?? "".Trim(); }
                        if ("txtValidFrom" == row1["clientidtextlabel"].ToString()) { VALIDFROM_IN = data.VALIDFROM ?? "".Trim(); }
                        if ("txtValidTo" == row1["clientidtextlabel"].ToString()) { VALIDTO_IN = data.VALIDTO ?? "".Trim(); }
                        if ("txtWithHoldingTaxNumber" == row1["clientidtextlabel"].ToString()) { WITHHOLDINGTAXNUMBER_IN = data.WITHHOLDINGTAXNUMBER ?? "".Trim(); }
                        if ("ddlReconAccount" == row1["clientidtextlabel"].ToString()) { RECONSACCOUNT_IN = data.RECONSACCOUNT ?? ""; }
                        if ("ddlTermsOfPayment" == row1["clientidtextlabel"].ToString()) { TERMOFPAYMENT_IN = data.TERMOFPAYMENT ?? ""; }
                        if ("ddlIncoterms" == row1["clientidtextlabel"].ToString()) { INCOTERM1_IN = data.INCOTERM1 ?? ""; }
                        if ("ddlIncoterms" == row1["clientidtextlabel"].ToString()) { INCOTERM2_IN = data.INCOTERM2 ?? ""; }
                        if ("ddlTaxClassification" == row1["clientidtextlabel"].ToString()) { TAXCLASSIFICATION_IN = data.TAXCLASSIFICATION ?? ""; }
                        if ("ddlAccountAssignmentGroup" == row1["clientidtextlabel"].ToString()) { ACCASSIGMENTGROUP_IN = data.ACCASSIGMENTGROUP ?? ""; }
                        if ("ddlCreditControlArea" == row1["clientidtextlabel"].ToString()) { CREDITCONTROLAREA_IN = data.CREDITCONTROLAREA ?? ""; }
                    }

                    cmhid = data.CMHEADERID ?? 0;
                   
                    // result = oMasterQueries.SaveCompanyData(USERID, cmdid2, cmhid, rdlRequest.SelectedValue, rdlRequestType.SelectedValue, txtCompanyCode.Text, ac_55AGW, txtSalesOrganization.Text, division, ddlDistribution.SelectedValue, ddlReconAccount.Text.Trim(), ddlTermsOfPayment.SelectedValue, txtPaymentMethods.Text.Trim(), txtHouseBank.Text, txtPaymentMethodSupplier.Text, txtWithHoldingTaxType.Text, txtWithHoldingTaxCode.Text, txtValidFrom.Text, txtValidTo.Text, txtWithHoldingTaxNumber.Text, ddlIncoterms.SelectedValue, ddlIncoterms.SelectedItem.Text, ddlCreditControlArea.SelectedValue, ddlAccountAssignmentGroup.SelectedValue, ddlTaxClassification.Text, COMP_RTO_FILE_IN, COMP_LOI_FILE_IN, email_id, ref headerid, ref detailid);
                    result = oMasterQueries.SaveCompanyData(USERID, cmdid2, cmhid, data.CM_REQ_TYPE, data.REQUEST_TYPE, data.COMPANY_CODE, data.CUST_ACC_TYPE1, data.SALES_ORG, data.DIVISION_GRP, data.DISTRIBTUION_CHH, RECONSACCOUNT_IN,
 TERMOFPAYMENT_IN, PAYMENTMETHODS_IN, HOUSEBANK_IN, PMTMETHSUPL_IN, WITHHOLDINGTAXTYPE_IN, WITHHOLDINGTAXCODE_IN, VALIDFROM_IN,
 VALIDTO_IN, WITHHOLDINGTAXNUMBER_IN, INCOTERM1_IN, INCOTERM2_IN, CREDITCONTROLAREA_IN, ACCASSIGMENTGROUP_IN, TAXCLASSIFICATION_IN, COMP_RTO_FILE_IN, COMP_LOI_FILE_IN, email_id, OTHER_DOC_FILE_IN, ref headerid, ref detailid);
                }
                if (result == "1")
                {
                    res.CMDEAILID2 = Convert.ToInt32(detailid);
                    res.MESSAGE = "Successfully saved!";
                    res.VIEWSTATE = "Y";

                    if (COMP_RTO_FILE_IN != "")
                    {
                        res.FILE1 = COMP_RTO_FILE_IN;
                        //hyperBankMandate.Visible = true;
                        //hyperBankMandate.OnClientClick = "javascript:openDialog('../../Uploads/CustomerMaster/" + hyperBankMandate.Text + "',980,680);";
                    }
                    if (COMP_LOI_FILE_IN != "")
                    {
                        res.FILE2 = COMP_LOI_FILE_IN;
                        //hyperCancelCheque.Visible = true;
                        //hyperCancelCheque.OnClientClick = "javascript:openDialog('../../Uploads/CustomerMaster/" + hyperCancelCheque.Text + "',980,680);";
                    }
                    if (OTHER_DOC_FILE_IN != "")
                    {
                        res.FILE3 = OTHER_DOC_FILE_IN;
                        //hyperCancelCheque.Visible = true;
                        //hyperCancelCheque.OnClientClick = "javascript:openDialog('../../Uploads/CustomerMaster/" + hyperCancelCheque.Text + "',980,680);";
                    }

                }
                else
                {
                    
                    res.MESSAGE = "Error in process of Company data save!";
                    res.FILE1 = result;
                    res.RS = 0;
                    _logger.LogError(result, "SaveCompanyData");
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "SaveCompanyData");
                res.MESSAGE = "Error in process of Company data save!";
                res.RS = 0;
                return res;
            }

        }
        public CustomerResponseData SubmitCutomerRequest(FinalSubmit data)
        {
            CustomerResponseData res = new CustomerResponseData();
            try
            {
                string request_no = "";
               string result = oMasterQueries.SubmitCMRequest(data.USERID??0, data.cmhid??0, data.SEC_HEAD_ID ?? 0, data.DEPT_DIV_ID??0, data.REMARKS??"", ref request_no);
                if (result == "1")
                {
                    res.RS = 1;
                    res.FILE1 = request_no;
                    string message = "";

                    if (data.hidGenDetailID == "")
                    {
                        res.MESSAGE = "Customer request " + request_no + " successfully forwarded!";

                    }
                    else
                    {
                        res.MESSAGE = "Customer request " + request_no + " successfully resubmitted and forwarded!";
                    }
                    string strSubject = "Pending for the Customer Master Approval";                             
                    oMasterQueries.SENDMAILTODEPTHEAD(data.USERID??0, request_no, strSubject, "S", data.DealerCode.Trim().ToUpper(), data.Name1.Trim());
                   
                }
                else
                {
                    res.RS = 0;
                   res.MESSAGE= "Error message :" + result;
                }

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("SubmitCutomerRequest", ex.Message);
                res.RS= 0;
                res.MESSAGE = ex.Message;
                return res;
            }
        }
        public CustomerResponseData ResetCMRequest(long USERID, string RequestType)
        {
            CustomerResponseData res = new CustomerResponseData();
            try
            {
                string request_no = "";
                string result = oMasterQueries.ResetCMRequest(USERID, RequestType,ref request_no);
                if (result == "1")
                {
                    res.RS = 1;
                    res.FILE1 = request_no;
                    string message = "Data has been reset!";           
            
                }
                else
                {
                    res.RS = 0;
                    res.MESSAGE = "Error message :" + result;
                }

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("ResetCMRequest", ex.Message);
                res.RS = 0;
                res.MESSAGE = ex.Message;
                return res;
            }
        }
        protected string BindMasterData(string code, string group_name)
        {
            string result = "";
            try
            {
                if (objDs == null) { objDs = oMasterQueries.GetCustomerMasterData("REGION"); }

                if (objDs.Tables[1].Rows.Count > 0)
                {
                    DataTable dv1 = objDs.Tables[1].Select(" group_name='" + group_name + "' And " + "CODE='" + code + "'").CopyToDataTable();
                    if (dv1 != null && dv1.Rows.Count > 0) { result = dv1.Rows[0]["CODE_DESC"].ToString(); }
                }

            }
            catch (Exception ex) { }
            return result;
        }
        private string GetTitleDesc(string value)
        {
            string titleDesc = "";
            if (value == "0001") { titleDesc = "Ms."; }
            if (value == "0002") { titleDesc = "Mr."; }
            if (value == "0003") { titleDesc = "Company"; }
            if (value == "0004") { titleDesc = "Mr.and Mrs."; }
            return titleDesc;

        }
        public CustomerMasterDetail GetPreviewData(string request, long userid)
        {
            CustomerMasterDetail res = new CustomerMasterDetail();
            try
            {
                string request_no = "", err_msg="";
                DataSet ds = oMasterQueries.GetCMDetailDraft(userid, "", ref err_msg);
                
                int s = 0;
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    res.CM_REQ_TYPE = ds.Tables[0].Rows[0]["CM_REQ_TYPE"].ToString() == "C" ? "Create" : "Update";
                    if (ds.Tables[0].Rows.Count > 1) {
                        s = 1;
                        res.CUST_ACC_TYPE = ds.Tables[0].Rows[0]["cust_acc_type"].ToString() + "|" + ds.Tables[0].Rows[1]["cust_acc_type"].ToString(); 
                    }
                    else { res.CUST_ACC_TYPE = ds.Tables[0].Rows[0]["cust_acc_type"].ToString(); }
                    //lblaccountgroup.Text = ds.Tables[0].Rows[0]["cust_acc_type"].ToString();
                    res.DISTRIBTUION_CHH = ds.Tables[0].Rows[0]["distribtuion_chh"].ToString();
                    res.DIVISION_GRP = ds.Tables[0].Rows[0]["division_grp"].ToString();
                    res.SALES_ORG = ds.Tables[0].Rows[0]["sales_org"].ToString();
                    res.COMPANY_CODE = "50AG"; // ds.Tables[0].Rows[0]["COMPANY_CODE"].ToString();

                    //**********************General Data************************************************//

                    res.CUSTOMER_CODE = ds.Tables[0].Rows[0]["customer_code"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["customer_code"].ToString() : ds.Tables[0].Rows[0]["customer_code"].ToString();
                    res.TITLE = ds.Tables[0].Rows[0]["title"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["title"].ToString() : ds.Tables[0].Rows[0]["title"].ToString();
                    res.NAME1 = ds.Tables[0].Rows[0]["name1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["name1"].ToString() : ds.Tables[0].Rows[0]["name1"].ToString();
                    res.NAME2 = ds.Tables[0].Rows[0]["name2"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["name2"].ToString() : ds.Tables[0].Rows[0]["name2"].ToString();
                    res.NAME3 = ds.Tables[0].Rows[0]["name3"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["name3"].ToString() : ds.Tables[0].Rows[0]["name3"].ToString();
                    res.NAME4 = ds.Tables[0].Rows[0]["name4"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["name4"].ToString() : ds.Tables[0].Rows[0]["name4"].ToString();
                    res.SEARCHTERM = ds.Tables[0].Rows[0]["searchterm"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["searchterm"].ToString() : ds.Tables[0].Rows[0]["searchterm"].ToString();
                    res.STREETHOUSENUMBER= ds.Tables[0].Rows[0]["streethousenumber"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["streethousenumber"].ToString() : ds.Tables[0].Rows[0]["streethousenumber"].ToString();
                    res.STREET2 = ds.Tables[0].Rows[0]["street2"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["street2"].ToString() : ds.Tables[0].Rows[0]["street2"].ToString();
                    res.STREET3 = ds.Tables[0].Rows[0]["street3"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["street3"].ToString() : ds.Tables[0].Rows[0]["street3"].ToString();
                    res.STREET4 = ds.Tables[0].Rows[0]["street4"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["street4"].ToString() : ds.Tables[0].Rows[0]["street4"].ToString();
                    res.STREET5 = ds.Tables[0].Rows[0]["street5"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["street5"].ToString() : ds.Tables[0].Rows[0]["street5"].ToString();
                    res.POSTALCODE = ds.Tables[0].Rows[0]["postalcode"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["postalcode"].ToString() : ds.Tables[0].Rows[0]["postalcode"].ToString();
                    res.CITY = ds.Tables[0].Rows[0]["city"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["city"].ToString() : ds.Tables[0].Rows[0]["city"].ToString();
                    res.COUNTRY = ds.Tables[0].Rows[0]["country"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["country"].ToString() : ds.Tables[0].Rows[0]["country"].ToString();
                    res.CREGION = ds.Tables[0].Rows[0]["cregion"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["cregion"].ToString() : ds.Tables[0].Rows[0]["cregion"].ToString();
                    res.CTIMEZONE = ds.Tables[0].Rows[0]["ctimezone"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["ctimezone"].ToString() : ds.Tables[0].Rows[0]["ctimezone"].ToString();
                    res.TRANSPORTATIONCODE = ds.Tables[0].Rows[0]["transportationcode"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["transportationcode"].ToString() : ds.Tables[0].Rows[0]["transportationcode"].ToString();
                    res.MOBILEPHONE = ds.Tables[0].Rows[0]["mobilephone"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["mobilephone"].ToString() : ds.Tables[0].Rows[0]["mobilephone"].ToString();
                    res.FAX = ds.Tables[0].Rows[0]["fax"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["fax"].ToString() : ds.Tables[0].Rows[0]["fax"].ToString();
                    res.EMAIL = ds.Tables[0].Rows[0]["email"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["email"].ToString() : ds.Tables[0].Rows[0]["email"].ToString();
                    res.INDUSTRY = ds.Tables[0].Rows[0]["industry"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["industry"].ToString() : ds.Tables[0].Rows[0]["industry"].ToString();
                    res.TAXNUMBER3 = ds.Tables[0].Rows[0]["taxnumber3"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["taxnumber3"].ToString() : ds.Tables[0].Rows[0]["taxnumber3"].ToString();
                    res.CITYCODE = ds.Tables[0].Rows[0]["citycode"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["citycode"].ToString() : ds.Tables[0].Rows[0]["citycode"].ToString();
                    res.CUSTOMERCLASS = ds.Tables[0].Rows[0]["customerclass"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["customerclass"].ToString() : ds.Tables[0].Rows[0]["customerclass"].ToString();
                    res.VENDORNO = ds.Tables[0].Rows[0]["VENDORNO"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["VENDORNO"].ToString() : ds.Tables[0].Rows[0]["VENDORNO"].ToString();
                    //***************Bank Details**********************//
                    res.CTRY = ds.Tables[0].Rows[0]["ctry"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["ctry"].ToString() : ds.Tables[0].Rows[0]["ctry"].ToString();
                    res.BANKKEY = ds.Tables[0].Rows[0]["bankkey"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankkey"].ToString() : ds.Tables[0].Rows[0]["bankkey"].ToString();
                    res.BANKACCOUNT = ds.Tables[0].Rows[0]["bankaccount"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankaccount"].ToString() : ds.Tables[0].Rows[0]["bankaccount"].ToString();
                    res.ACCOUNTHOLDER = ds.Tables[0].Rows[0]["accountholder"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["accountholder"].ToString() : ds.Tables[0].Rows[0]["accountholder"].ToString();
                    res.BANKCONTROLKEY = ds.Tables[0].Rows[0]["bankcontrolkey"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankcontrolkey"].ToString() : ds.Tables[0].Rows[0]["bankcontrolkey"].ToString();
                    res.BANKNAME = ds.Tables[0].Rows[0]["bankname"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankname"].ToString() : ds.Tables[0].Rows[0]["bankname"].ToString();
                    res.BANKREGION = ds.Tables[0].Rows[0]["bankregion"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankregion"].ToString() : ds.Tables[0].Rows[0]["bankregion"].ToString();
                    res.BANKSTREET = ds.Tables[0].Rows[0]["bankstreet"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankstreet"].ToString() : ds.Tables[0].Rows[0]["bankstreet"].ToString();
                    res.BANKCITY = ds.Tables[0].Rows[0]["bankcity"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankcity"].ToString() : ds.Tables[0].Rows[0]["bankcity"].ToString();
                    res.BANKBRANCH = ds.Tables[0].Rows[0]["bankbranch"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankbranch"].ToString() : ds.Tables[0].Rows[0]["bankbranch"].ToString();
                    res.BANKCURRENCY = ds.Tables[0].Rows[0]["bankcurrency"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankcurrency"].ToString() : ds.Tables[0].Rows[0]["bankcurrency"].ToString();
                    //***************Industry Details *********************************//
                    res.INDUSTRY1 = ds.Tables[0].Rows[0]["industry1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["industry1"].ToString() : ds.Tables[0].Rows[0]["industry1"].ToString();
                    res.INDUSTRYCODE1 = ds.Tables[0].Rows[0]["industrycode1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["industrycode1"].ToString() : ds.Tables[0].Rows[0]["industrycode1"].ToString();
                    res.INDUSTRYNAME = ds.Tables[0].Rows[0]["industryname"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["industryname"].ToString() : ds.Tables[0].Rows[0]["industryname"].ToString();
                    res.FIRSTNAME = ds.Tables[0].Rows[0]["firstname"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["firstname"].ToString() : ds.Tables[0].Rows[0]["firstname"].ToString();
                    //************************Sales Area Data***************************************//
                    res.EXCHANGERATETYPE = ds.Tables[0].Rows[0]["exchangeratetype"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["exchangeratetype"].ToString() : ds.Tables[0].Rows[0]["exchangeratetype"].ToString();
                    res.PRICEGROUP = ds.Tables[0].Rows[0]["pricegroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["pricegroup"].ToString() : ds.Tables[0].Rows[0]["pricegroup"].ToString();
                    res.SALESDISTRICT = ds.Tables[0].Rows[0]["salesdistrict"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["salesdistrict"].ToString() : ds.Tables[0].Rows[0]["salesdistrict"].ToString();
                    res.SALESOFFICE = ds.Tables[0].Rows[0]["salesoffice"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["salesoffice"].ToString() : ds.Tables[0].Rows[0]["salesoffice"].ToString();
                    res.SALESGROUP = ds.Tables[0].Rows[0]["salesgroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["salesgroup"].ToString() : ds.Tables[0].Rows[0]["salesgroup"].ToString();
                    res.CUSTOMERGROUP = ds.Tables[0].Rows[0]["customergroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["customergroup"].ToString() : ds.Tables[0].Rows[0]["customergroup"].ToString();
                    res.SALESCURRENCY = ds.Tables[0].Rows[0]["salescurrency"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["salescurrency"].ToString() : ds.Tables[0].Rows[0]["salescurrency"].ToString();
                    res.CUSTPRICPROC1 = ds.Tables[0].Rows[0]["custpricproc1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["custpricproc1"].ToString() : ds.Tables[0].Rows[0]["custpricproc1"].ToString();
                    res.DELIVERYPRIORITYGROUP = ds.Tables[0].Rows[0]["deliveryprioritygroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["deliveryprioritygroup"].ToString() : ds.Tables[0].Rows[0]["deliveryprioritygroup"].ToString();
                    res.SHIPPINGCONDITION = ds.Tables[0].Rows[0]["shippingcondition"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["shippingcondition"].ToString() : ds.Tables[0].Rows[0]["shippingcondition"].ToString();
                    //******************************CIN Details********************************************//
                    res.CSTNO = ds.Tables[0].Rows[0]["cstno"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["cstno"].ToString() : ds.Tables[0].Rows[0]["cstno"].ToString();
                    res.LSTNO = ds.Tables[0].Rows[0]["lstno"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["lstno"].ToString() : ds.Tables[0].Rows[0]["lstno"].ToString();
                    res.INVOICINGDATES = ds.Tables[0].Rows[0]["invoicingdates"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["invoicingdates"].ToString() : ds.Tables[0].Rows[0]["invoicingdates"].ToString();
                    res.INVOICINGLISTDATES = ds.Tables[0].Rows[0]["invoicinglistdates"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["invoicinglistdates"].ToString() : ds.Tables[0].Rows[0]["invoicinglistdates"].ToString();
                    res.SERREGNO = ds.Tables[0].Rows[0]["serregno"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["serregno"].ToString() : ds.Tables[0].Rows[0]["serregno"].ToString();
                    res.PANNUMBER = ds.Tables[0].Rows[0]["pannumber"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["pannumber"].ToString() : ds.Tables[0].Rows[0]["pannumber"].ToString();
                    res.PAYMENTGUARANTEEPROC = ds.Tables[0].Rows[0]["paymentguaranteeproc"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["paymentguaranteeproc"].ToString() : ds.Tables[0].Rows[0]["paymentguaranteeproc"].ToString();
                    res.E_INVOICE_APPLICABLE = ds.Tables[0].Rows[0]["E_INVOICE_APPLICABLE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["E_INVOICE_APPLICABLE"].ToString() : ds.Tables[0].Rows[0]["E_INVOICE_APPLICABLE"].ToString();
                    //***********************Company Code Data**********************************//
                    res.RECONSACCOUNT = ds.Tables[0].Rows[0]["reconsaccount"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["reconsaccount"].ToString() : ds.Tables[0].Rows[0]["reconsaccount"].ToString();
                    res.INCOTERM1 = ds.Tables[0].Rows[0]["incoterm1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["incoterm1"].ToString() : ds.Tables[0].Rows[0]["incoterm1"].ToString();
                    res.TERMOFPAYMENT = ds.Tables[0].Rows[0]["termofpayment"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["termofpayment"].ToString() : ds.Tables[0].Rows[0]["termofpayment"].ToString();
                    res.CREDITCONTROLAREA = ds.Tables[0].Rows[0]["creditcontrolarea"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["creditcontrolarea"].ToString() : ds.Tables[0].Rows[0]["creditcontrolarea"].ToString();
                    res.ACCASSIGMENTGROUP = ds.Tables[0].Rows[0]["accassigmentgroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["accassigmentgroup"].ToString() : ds.Tables[0].Rows[0]["accassigmentgroup"].ToString();
                    res.TAXCLASSIFICATION = ds.Tables[0].Rows[0]["taxclassification"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["taxclassification"].ToString() : ds.Tables[0].Rows[0]["taxclassification"].ToString();
                    res.WITHHOLDINGTAXTYPE = ds.Tables[0].Rows[0]["withholdingtaxtype"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["withholdingtaxtype"].ToString() : ds.Tables[0].Rows[0]["withholdingtaxtype"].ToString();
                    res.WITHHOLDINGTAXCODE = ds.Tables[0].Rows[0]["withholdingtaxcode"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["withholdingtaxcode"].ToString() : ds.Tables[0].Rows[0]["withholdingtaxcode"].ToString();
                    res.VALIDFROM = ds.Tables[0].Rows[0]["validfrom"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["validfrom"].ToString() : ds.Tables[0].Rows[0]["validfrom"].ToString();
                    res.VALIDTO = ds.Tables[0].Rows[0]["validto"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["validto"].ToString() : ds.Tables[0].Rows[0]["validto"].ToString();
                    res.WITHHOLDINGTAXNUMBER = ds.Tables[0].Rows[0]["withholdingtaxnumber"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["withholdingtaxnumber"].ToString() : ds.Tables[0].Rows[0]["withholdingtaxnumber"].ToString();
                    res.PMTMETHSUPL = ds.Tables[0].Rows[0]["pmtmethsupl"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["pmtmethsupl"].ToString() : ds.Tables[0].Rows[0]["pmtmethsupl"].ToString();
                    res.PAYMENTMETHODS = ds.Tables[0].Rows[0]["paymentmethods"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["paymentmethods"].ToString() : ds.Tables[0].Rows[0]["paymentmethods"].ToString();
                    res.HOUSEBANK = ds.Tables[0].Rows[0]["housebank"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["housebank"].ToString() : ds.Tables[0].Rows[0]["housebank"].ToString();

                    res.BANK_CANCELED_FILE = ds.Tables[0].Rows[0]["BANK_MANDATE_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["BANK_MANDATE_FILE"].ToString() : ds.Tables[0].Rows[0]["BANK_MANDATE_FILE"].ToString();
                    res.BANK_CANCELED_FILE = ds.Tables[0].Rows[0]["BANK_CANCELED_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["BANK_CANCELED_FILE"].ToString() : ds.Tables[0].Rows[0]["BANK_CANCELED_FILE"].ToString();
                    res.CIN_PAN_NO_FILE = ds.Tables[0].Rows[0]["CIN_PAN_NO_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["CIN_PAN_NO_FILE"].ToString() : ds.Tables[0].Rows[0]["CIN_PAN_NO_FILE"].ToString();
                    res.CIN_GST_NO_FILE = ds.Tables[0].Rows[0]["CIN_GST_NO_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["CIN_GST_NO_FILE"].ToString() : ds.Tables[0].Rows[0]["CIN_GST_NO_FILE"].ToString();
                    res.COMP_RTO_FILE = ds.Tables[0].Rows[0]["COMP_RTO_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["COMP_RTO_FILE"].ToString() : ds.Tables[0].Rows[0]["COMP_RTO_FILE"].ToString();
                    res.COMP_LOI_FILE = ds.Tables[0].Rows[0]["COMP_LOI_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["COMP_LOI_FILE"].ToString() : ds.Tables[0].Rows[0]["COMP_LOI_FILE"].ToString();
                    res.CIN_EINVOICE_DOC_FILE = ds.Tables[0].Rows[0]["CIN_EINVOICE_DOC_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["CIN_EINVOICE_DOC_FILE"].ToString() : ds.Tables[0].Rows[0]["CIN_EINVOICE_DOC_FILE"].ToString();
                    res.OTHER_DOC_FILE = ds.Tables[0].Rows[0]["OTHER_DOC_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["OTHER_DOC_FILE"].ToString() : ds.Tables[0].Rows[0]["OTHER_DOC_FILE"].ToString();
                    res.REMARKS = ds.Tables[0].Rows[0]["REMARKS"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["REMARKS"].ToString() : ds.Tables[0].Rows[0]["REMARKS"].ToString();

                   


                    ////***********************Fill with description******************************** //////
                    if (res.CREGION != "")
                    {
                        string t = BindMasterData(res.CREGION, "REGION");
                        if (t.Length > 0) res.CREGION = t;
                    }
                    if (res.COUNTRY != "")
                    {
                        string t = BindMasterData(res.COUNTRY, "COUNTRY");
                        if (t.Length > 0) res.COUNTRY = t;
                    }

                    if (res.TRANSPORTATIONCODE != "")
                    {
                        string t = BindMasterData(res.TRANSPORTATIONCODE, "TRANSPORTATION CODE");
                        if (t.Length > 0) res.TRANSPORTATIONCODE = t;
                    }

                    if (res.CITYCODE != "")
                    {
                        string t = BindMasterData(res.CITYCODE, "CITY CODE");
                        if (t.Length > 0) res.CITYCODE = t;
                    }
                    if (res.CUSTOMERCLASS != "")
                    {
                        string t = BindMasterData(res.CUSTOMERCLASS, "CUSTOMER CLASS");
                        if (t.Length > 0) res.CUSTOMERCLASS = t;
                    }
                    if (res.CTRY != "")
                    {
                        string t = BindMasterData(res.CTRY, "COUNTRY");
                        if (t.Length > 0) res.CTRY = t;
                    }

                    if (res.INDUSTRY != "")
                    {
                        string t = BindMasterData(res.INDUSTRY, "INDUSTRY");
                        if (t.Length > 0) res.INDUSTRY = t;
                    }
                    if (res.CTIMEZONE  != "")
                    {
                        string t = BindMasterData(res.CTIMEZONE, "TIMEZONE");
                        if (t.Length > 0) res.CTIMEZONE = t;
                    }
                    if (res.EXCHANGERATETYPE != "")
                    {
                        string t = BindMasterData(res.EXCHANGERATETYPE, "EXCHANGE RATE TYPE");
                        if (t.Length > 0) res.EXCHANGERATETYPE = t;
                    }
                    if (res.SALESDISTRICT != "")
                    {
                        string t = BindMasterData(res.SALESDISTRICT, "SALES DISTRICT");
                        if (t.Length > 0) res.SALESDISTRICT = t;
                    }
                    if (res.SALESOFFICE != "")
                    {
                        string t = BindMasterData(res.SALESOFFICE, "SALES OFFICE");
                        if (t.Length > 0) res.SALESOFFICE = t;
                    }
                    if (res.SALESGROUP != "")
                    {
                        string t = BindMasterData(res.SALESGROUP, "SALES GROUP");
                        if (t.Length > 0) res.SALESGROUP = t;
                    }
                    if (res.CUSTOMERGROUP != "")
                    {
                        string t = BindMasterData(res.CUSTOMERGROUP, "CUSTOMER GROUP");
                        if (t.Length > 0) res.CUSTOMERGROUP = t;
                    }
                    if (res.SALESCURRENCY != "")
                    {
                        string t = BindMasterData(res.SALESCURRENCY, "CURRENCY");
                        if (t.Length > 0) res.SALESCURRENCY = t;
                    }
                    if (res.CUSTPRICPROC1 != "")
                    {
                        string t = BindMasterData(res.CUSTPRICPROC1, "CUST. PRICE");
                        if (t.Length > 0) res.CUSTPRICPROC1 = t;
                    }
                    if (res.DELIVERYPRIORITYGROUP != "")
                    {
                        string t = BindMasterData(res.DELIVERYPRIORITYGROUP, "DELIVERY PRIORITY");
                        if (t.Length > 0) res.DELIVERYPRIORITYGROUP = t;
                    }
                    if (res.SHIPPINGCONDITION != "")
                    {
                        string t = BindMasterData(res.SHIPPINGCONDITION, "SHIPPING CONDITIONS");
                        if (t.Length > 0) res.SHIPPINGCONDITION = t;
                    }
                    if (res.INCOTERM1 != "")
                    {
                        string t = BindMasterData(res.INCOTERM1, "INCOTERMS");
                        if (t.Length > 0) res.INCOTERM1 = t;
                    }
                    if (res.TERMOFPAYMENT != "")
                    {
                        string t = BindMasterData(res.TERMOFPAYMENT, "TERMS OF PAYMENT");
                        if (t.Length > 0) res.TERMOFPAYMENT = t;
                    }
                    if (res.CREDITCONTROLAREA != "")
                    {
                        string t = BindMasterData(res.CREDITCONTROLAREA, "CREDIT CONTROL AREA");
                        if (t.Length > 0) res.CREDITCONTROLAREA = t;
                    }
                    if (res.ACCASSIGMENTGROUP != "")
                    {
                        string t = BindMasterData(res.ACCASSIGMENTGROUP, "ACCT ASSIGNMENT GROUP");
                        if (t.Length > 0) res.ACCASSIGMENTGROUP = t;
                    }
                    if (res.TAXCLASSIFICATION != "")
                    {
                        string t = BindMasterData(res.TAXCLASSIFICATION, "TAX CLASSIFICATION");
                        if (t.Length > 0) res.TAXCLASSIFICATION = t;
                    }

                    if (res.DISTRIBTUION_CHH != "")
                    {
                        string t = BindMasterData(res.DISTRIBTUION_CHH, "DISTRIBUTION");
                        if (t.Length > 0) res.DISTRIBTUION_CHH = t;
                    }

                    if (res.BANKREGION != "")
                    {
                        string t = BindMasterData(res.BANKREGION, "REGION");
                        if (t.Length > 0) res.BANKREGION = t;
                    }
                    if (res.BANKCURRENCY != "")
                    {
                        string t = BindMasterData(res.BANKCURRENCY, "CURRENCY");
                        if (t.Length > 0) res.BANKCURRENCY = t;
                    }

                    res.TITLE = GetTitleDesc(res.TITLE);
                }

           
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("ResetCMRequest", ex.Message);               
                return res;
            }
        }
        public CustomerMasterDetail GetCustomerRequestDetails(string RequestNo, string userid)
        {
            CustomerMasterDetail res = new CustomerMasterDetail();
            try
            {
                string request_no = "", err_msg = "";
                DataSet ds = oMasterQueries.GetCustomerDetails(userid, RequestNo, 0);

                int s = 0;
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    res.CMHEADERID = Convert.ToInt64(ds.Tables[0].Rows[0]["cmheaderid"].ToString());
                    res.GEN_REQUEST_NO = ds.Tables[0].Rows[0]["gen_request_no"].ToString();

                    res.CM_REQ_TYPE = ds.Tables[0].Rows[0]["REQUEST_TYPE1"].ToString() == "C" ? "Create" : "Update";
                    if (ds.Tables[0].Rows.Count > 1)
                    {
                        s = 1;
                        res.CUST_ACC_TYPE = ds.Tables[0].Rows[0]["cust_acc_type"].ToString() + "|" + ds.Tables[0].Rows[1]["cust_acc_type"].ToString();
                    }
                    else { res.CUST_ACC_TYPE = ds.Tables[0].Rows[0]["cust_acc_type"].ToString(); }
                    //lblaccountgroup.Text = ds.Tables[0].Rows[0]["cust_acc_type"].ToString();
                    res.DISTRIBTUION_CHH = ds.Tables[0].Rows[0]["distribtuion_chh"].ToString();
                    res.DIVISION_GRP = ds.Tables[0].Rows[0]["division_grp"].ToString();
                    res.SALES_ORG = ds.Tables[0].Rows[0]["sales_org"].ToString();
                    res.COMPANY_CODE = "50AG"; // ds.Tables[0].Rows[0]["COMPANY_CODE"].ToString();

                    //**********************General Data************************************************//

                    res.CUSTOMER_CODE = ds.Tables[0].Rows[0]["customer_code"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["customer_code"].ToString() : ds.Tables[0].Rows[0]["customer_code"].ToString();
                    res.TITLE = ds.Tables[0].Rows[0]["title"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["title"].ToString() : ds.Tables[0].Rows[0]["title"].ToString();
                    res.NAME1 = ds.Tables[0].Rows[0]["name1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["name1"].ToString() : ds.Tables[0].Rows[0]["name1"].ToString();
                    res.NAME2 = ds.Tables[0].Rows[0]["name2"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["name2"].ToString() : ds.Tables[0].Rows[0]["name2"].ToString();
                    res.NAME3 = ds.Tables[0].Rows[0]["name3"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["name3"].ToString() : ds.Tables[0].Rows[0]["name3"].ToString();
                    res.NAME4 = ds.Tables[0].Rows[0]["name4"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["name4"].ToString() : ds.Tables[0].Rows[0]["name4"].ToString();
                    res.SEARCHTERM = ds.Tables[0].Rows[0]["searchterm"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["searchterm"].ToString() : ds.Tables[0].Rows[0]["searchterm"].ToString();
                    res.STREETHOUSENUMBER = ds.Tables[0].Rows[0]["streethousenumber"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["streethousenumber"].ToString() : ds.Tables[0].Rows[0]["streethousenumber"].ToString();
                    res.STREET2 = ds.Tables[0].Rows[0]["street2"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["street2"].ToString() : ds.Tables[0].Rows[0]["street2"].ToString();
                    res.STREET3 = ds.Tables[0].Rows[0]["street3"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["street3"].ToString() : ds.Tables[0].Rows[0]["street3"].ToString();
                    res.STREET4 = ds.Tables[0].Rows[0]["street4"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["street4"].ToString() : ds.Tables[0].Rows[0]["street4"].ToString();
                    res.STREET5 = ds.Tables[0].Rows[0]["street5"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["street5"].ToString() : ds.Tables[0].Rows[0]["street5"].ToString();
                    res.POSTALCODE = ds.Tables[0].Rows[0]["postalcode"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["postalcode"].ToString() : ds.Tables[0].Rows[0]["postalcode"].ToString();
                    res.CITY = ds.Tables[0].Rows[0]["city"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["city"].ToString() : ds.Tables[0].Rows[0]["city"].ToString();
                    res.COUNTRY = ds.Tables[0].Rows[0]["country"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["country"].ToString() : ds.Tables[0].Rows[0]["country"].ToString();
                    res.CREGION = ds.Tables[0].Rows[0]["cregion"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["cregion"].ToString() : ds.Tables[0].Rows[0]["cregion"].ToString();
                    res.CTIMEZONE = ds.Tables[0].Rows[0]["ctimezone"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["ctimezone"].ToString() : ds.Tables[0].Rows[0]["ctimezone"].ToString();
                    res.TRANSPORTATIONCODE = ds.Tables[0].Rows[0]["transportationcode"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["transportationcode"].ToString() : ds.Tables[0].Rows[0]["transportationcode"].ToString();
                    res.MOBILEPHONE = ds.Tables[0].Rows[0]["mobilephone"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["mobilephone"].ToString() : ds.Tables[0].Rows[0]["mobilephone"].ToString();
                    res.FAX = ds.Tables[0].Rows[0]["fax"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["fax"].ToString() : ds.Tables[0].Rows[0]["fax"].ToString();
                    res.EMAIL = ds.Tables[0].Rows[0]["email"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["email"].ToString() : ds.Tables[0].Rows[0]["email"].ToString();
                    res.INDUSTRY = ds.Tables[0].Rows[0]["industry"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["industry"].ToString() : ds.Tables[0].Rows[0]["industry"].ToString();
                    res.TAXNUMBER3 = ds.Tables[0].Rows[0]["taxnumber3"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["taxnumber3"].ToString() : ds.Tables[0].Rows[0]["taxnumber3"].ToString();
                    res.CITYCODE = ds.Tables[0].Rows[0]["citycode"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["citycode"].ToString() : ds.Tables[0].Rows[0]["citycode"].ToString();
                    res.CUSTOMERCLASS = ds.Tables[0].Rows[0]["customerclass"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["customerclass"].ToString() : ds.Tables[0].Rows[0]["customerclass"].ToString();
                    res.VENDORNO = ds.Tables[0].Rows[0]["VENDORNO"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["VENDORNO"].ToString() : ds.Tables[0].Rows[0]["VENDORNO"].ToString();
                    //***************Bank Details**********************//
                    res.CTRY = ds.Tables[0].Rows[0]["ctry"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["ctry"].ToString() : ds.Tables[0].Rows[0]["ctry"].ToString();
                    res.BANKKEY = ds.Tables[0].Rows[0]["bankkey"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankkey"].ToString() : ds.Tables[0].Rows[0]["bankkey"].ToString();
                    res.BANKACCOUNT = ds.Tables[0].Rows[0]["bankaccount"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankaccount"].ToString() : ds.Tables[0].Rows[0]["bankaccount"].ToString();
                    res.ACCOUNTHOLDER = ds.Tables[0].Rows[0]["accountholder"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["accountholder"].ToString() : ds.Tables[0].Rows[0]["accountholder"].ToString();
                    res.BANKCONTROLKEY = ds.Tables[0].Rows[0]["bankcontrolkey"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankcontrolkey"].ToString() : ds.Tables[0].Rows[0]["bankcontrolkey"].ToString();
                    res.BANKNAME = ds.Tables[0].Rows[0]["bankname"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankname"].ToString() : ds.Tables[0].Rows[0]["bankname"].ToString();
                    res.BANKREGION = ds.Tables[0].Rows[0]["bankregion"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankregion"].ToString() : ds.Tables[0].Rows[0]["bankregion"].ToString();
                    res.BANKSTREET = ds.Tables[0].Rows[0]["bankstreet"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankstreet"].ToString() : ds.Tables[0].Rows[0]["bankstreet"].ToString();
                    res.BANKCITY = ds.Tables[0].Rows[0]["bankcity"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankcity"].ToString() : ds.Tables[0].Rows[0]["bankcity"].ToString();
                    res.BANKBRANCH = ds.Tables[0].Rows[0]["bankbranch"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankbranch"].ToString() : ds.Tables[0].Rows[0]["bankbranch"].ToString();
                    res.BANKCURRENCY = ds.Tables[0].Rows[0]["bankcurrency"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["bankcurrency"].ToString() : ds.Tables[0].Rows[0]["bankcurrency"].ToString();
                    //***************Industry Details *********************************//
                    res.INDUSTRY1 = ds.Tables[0].Rows[0]["industry1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["industry1"].ToString() : ds.Tables[0].Rows[0]["industry1"].ToString();
                    res.INDUSTRYCODE1 = ds.Tables[0].Rows[0]["industrycode1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["industrycode1"].ToString() : ds.Tables[0].Rows[0]["industrycode1"].ToString();
                    res.INDUSTRYNAME = ds.Tables[0].Rows[0]["industryname"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["industryname"].ToString() : ds.Tables[0].Rows[0]["industryname"].ToString();
                    res.FIRSTNAME = ds.Tables[0].Rows[0]["firstname"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["firstname"].ToString() : ds.Tables[0].Rows[0]["firstname"].ToString();
                    //************************Sales Area Data***************************************//
                    res.EXCHANGERATETYPE = ds.Tables[0].Rows[0]["exchangeratetype"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["exchangeratetype"].ToString() : ds.Tables[0].Rows[0]["exchangeratetype"].ToString();
                    res.PRICEGROUP = ds.Tables[0].Rows[0]["pricegroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["pricegroup"].ToString() : ds.Tables[0].Rows[0]["pricegroup"].ToString();
                    res.SALESDISTRICT = ds.Tables[0].Rows[0]["salesdistrict"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["salesdistrict"].ToString() : ds.Tables[0].Rows[0]["salesdistrict"].ToString();
                    res.SALESOFFICE = ds.Tables[0].Rows[0]["salesoffice"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["salesoffice"].ToString() : ds.Tables[0].Rows[0]["salesoffice"].ToString();
                    res.SALESGROUP = ds.Tables[0].Rows[0]["salesgroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["salesgroup"].ToString() : ds.Tables[0].Rows[0]["salesgroup"].ToString();
                    res.CUSTOMERGROUP = ds.Tables[0].Rows[0]["customergroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["customergroup"].ToString() : ds.Tables[0].Rows[0]["customergroup"].ToString();
                    res.SALESCURRENCY = ds.Tables[0].Rows[0]["salescurrency"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["salescurrency"].ToString() : ds.Tables[0].Rows[0]["salescurrency"].ToString();
                    res.CUSTPRICPROC1 = ds.Tables[0].Rows[0]["custpricproc1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["custpricproc1"].ToString() : ds.Tables[0].Rows[0]["custpricproc1"].ToString();
                    res.DELIVERYPRIORITYGROUP = ds.Tables[0].Rows[0]["deliveryprioritygroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["deliveryprioritygroup"].ToString() : ds.Tables[0].Rows[0]["deliveryprioritygroup"].ToString();
                    res.SHIPPINGCONDITION = ds.Tables[0].Rows[0]["shippingcondition"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["shippingcondition"].ToString() : ds.Tables[0].Rows[0]["shippingcondition"].ToString();
                    //******************************CIN Details********************************************//
                    res.CSTNO = ds.Tables[0].Rows[0]["cstno"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["cstno"].ToString() : ds.Tables[0].Rows[0]["cstno"].ToString();
                    res.LSTNO = ds.Tables[0].Rows[0]["lstno"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["lstno"].ToString() : ds.Tables[0].Rows[0]["lstno"].ToString();
                    res.INVOICINGDATES = ds.Tables[0].Rows[0]["invoicingdates"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["invoicingdates"].ToString() : ds.Tables[0].Rows[0]["invoicingdates"].ToString();
                    res.INVOICINGLISTDATES = ds.Tables[0].Rows[0]["invoicinglistdates"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["invoicinglistdates"].ToString() : ds.Tables[0].Rows[0]["invoicinglistdates"].ToString();
                    res.SERREGNO = ds.Tables[0].Rows[0]["serregno"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["serregno"].ToString() : ds.Tables[0].Rows[0]["serregno"].ToString();
                    res.PANNUMBER = ds.Tables[0].Rows[0]["pannumber"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["pannumber"].ToString() : ds.Tables[0].Rows[0]["pannumber"].ToString();
                    res.PAYMENTGUARANTEEPROC = ds.Tables[0].Rows[0]["paymentguaranteeproc"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["paymentguaranteeproc"].ToString() : ds.Tables[0].Rows[0]["paymentguaranteeproc"].ToString();
                    res.E_INVOICE_APPLICABLE = ds.Tables[0].Rows[0]["E_INVOICE_APPLICABLE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["E_INVOICE_APPLICABLE"].ToString() : ds.Tables[0].Rows[0]["E_INVOICE_APPLICABLE"].ToString();
                    //***********************Company Code Data**********************************//
                    res.RECONSACCOUNT = ds.Tables[0].Rows[0]["reconsaccount"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["reconsaccount"].ToString() : ds.Tables[0].Rows[0]["reconsaccount"].ToString();
                    res.INCOTERM1 = ds.Tables[0].Rows[0]["incoterm1"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["incoterm1"].ToString() : ds.Tables[0].Rows[0]["incoterm1"].ToString();
                    res.TERMOFPAYMENT = ds.Tables[0].Rows[0]["termofpayment"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["termofpayment"].ToString() : ds.Tables[0].Rows[0]["termofpayment"].ToString();
                    res.CREDITCONTROLAREA = ds.Tables[0].Rows[0]["creditcontrolarea"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["creditcontrolarea"].ToString() : ds.Tables[0].Rows[0]["creditcontrolarea"].ToString();
                    res.ACCASSIGMENTGROUP = ds.Tables[0].Rows[0]["accassigmentgroup"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["accassigmentgroup"].ToString() : ds.Tables[0].Rows[0]["accassigmentgroup"].ToString();
                    res.TAXCLASSIFICATION = ds.Tables[0].Rows[0]["taxclassification"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["taxclassification"].ToString() : ds.Tables[0].Rows[0]["taxclassification"].ToString();
                    res.WITHHOLDINGTAXTYPE = ds.Tables[0].Rows[0]["withholdingtaxtype"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["withholdingtaxtype"].ToString() : ds.Tables[0].Rows[0]["withholdingtaxtype"].ToString();
                    res.WITHHOLDINGTAXCODE = ds.Tables[0].Rows[0]["withholdingtaxcode"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["withholdingtaxcode"].ToString() : ds.Tables[0].Rows[0]["withholdingtaxcode"].ToString();
                    res.VALIDFROM = ds.Tables[0].Rows[0]["validfrom"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["validfrom"].ToString() : ds.Tables[0].Rows[0]["validfrom"].ToString();
                    res.VALIDTO = ds.Tables[0].Rows[0]["validto"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["validto"].ToString() : ds.Tables[0].Rows[0]["validto"].ToString();
                    res.WITHHOLDINGTAXNUMBER = ds.Tables[0].Rows[0]["withholdingtaxnumber"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["withholdingtaxnumber"].ToString() : ds.Tables[0].Rows[0]["withholdingtaxnumber"].ToString();
                    res.PMTMETHSUPL = ds.Tables[0].Rows[0]["pmtmethsupl"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["pmtmethsupl"].ToString() : ds.Tables[0].Rows[0]["pmtmethsupl"].ToString();
                    res.PAYMENTMETHODS = ds.Tables[0].Rows[0]["paymentmethods"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["paymentmethods"].ToString() : ds.Tables[0].Rows[0]["paymentmethods"].ToString();
                    res.HOUSEBANK = ds.Tables[0].Rows[0]["housebank"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["housebank"].ToString() : ds.Tables[0].Rows[0]["housebank"].ToString();

                    res.BANK_MANDATE_FILE = ds.Tables[0].Rows[0]["BANK_MANDATE_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["BANK_MANDATE_FILE"].ToString() : ds.Tables[0].Rows[0]["BANK_MANDATE_FILE"].ToString();
                    res.BANK_CANCELED_FILE = ds.Tables[0].Rows[0]["BANK_CANCELED_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["BANK_CANCELED_FILE"].ToString() : ds.Tables[0].Rows[0]["BANK_CANCELED_FILE"].ToString();
                    res.CIN_PAN_NO_FILE = ds.Tables[0].Rows[0]["CIN_PAN_NO_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["CIN_PAN_NO_FILE"].ToString() : ds.Tables[0].Rows[0]["CIN_PAN_NO_FILE"].ToString();
                    res.CIN_GST_NO_FILE = ds.Tables[0].Rows[0]["CIN_GST_NO_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["CIN_GST_NO_FILE"].ToString() : ds.Tables[0].Rows[0]["CIN_GST_NO_FILE"].ToString();
                    res.COMP_RTO_FILE = ds.Tables[0].Rows[0]["COMP_RTO_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["COMP_RTO_FILE"].ToString() : ds.Tables[0].Rows[0]["COMP_RTO_FILE"].ToString();
                    res.COMP_LOI_FILE = ds.Tables[0].Rows[0]["COMP_LOI_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["COMP_LOI_FILE"].ToString() : ds.Tables[0].Rows[0]["COMP_LOI_FILE"].ToString();
                    res.CIN_EINVOICE_DOC_FILE = ds.Tables[0].Rows[0]["CIN_EINVOICE_DOC_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["CIN_EINVOICE_DOC_FILE"].ToString() : ds.Tables[0].Rows[0]["CIN_EINVOICE_DOC_FILE"].ToString();
                    res.OTHER_DOC_FILE = ds.Tables[0].Rows[0]["OTHER_DOC_FILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["OTHER_DOC_FILE"].ToString() : ds.Tables[0].Rows[0]["OTHER_DOC_FILE"].ToString();
                    res.REMARKS = ds.Tables[0].Rows[0]["REMARKS"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["REMARKS"].ToString() : ds.Tables[0].Rows[0]["REMARKS"].ToString();
                    var req_det = new CustomerRequesterDetail();
                    req_det.requesterid = ds.Tables[0].Rows[0]["requesterid"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["requesterid"].ToString() : ds.Tables[0].Rows[0]["requesterid"].ToString();
                    req_det.ENAME = ds.Tables[0].Rows[0]["ENAME"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["ENAME"].ToString() : ds.Tables[0].Rows[0]["ENAME"].ToString();
                    req_det.EMAILID = ds.Tables[0].Rows[0]["EMAILID"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["EMAILID"].ToString() : ds.Tables[0].Rows[0]["EMAILID"].ToString();
                    req_det.TMOBILE = ds.Tables[0].Rows[0]["TMOBILE"].ToString() == "" && s == 1 ? ds.Tables[0].Rows[1]["TMOBILE"].ToString() : ds.Tables[0].Rows[0]["TMOBILE"].ToString();
                    res.REQUESTERDETAILS=req_det;


                    ////***********************Fill with description******************************** //////
                    if (res.CREGION != "")
                    {
                        string t = BindMasterData(res.CREGION, "REGION");
                        if (t.Length > 0) res.CREGION = t;
                    }
                    if (res.COUNTRY != "")
                    {
                        string t = BindMasterData(res.COUNTRY, "COUNTRY");
                        if (t.Length > 0) res.COUNTRY = t;
                    }

                    if (res.TRANSPORTATIONCODE != "")
                    {
                        string t = BindMasterData(res.TRANSPORTATIONCODE, "TRANSPORTATION CODE");
                        if (t.Length > 0) res.TRANSPORTATIONCODE = t;
                    }

                    if (res.CITYCODE != "")
                    {
                        string t = BindMasterData(res.CITYCODE, "CITY CODE");
                        if (t.Length > 0) res.CITYCODE = t;
                    }
                    if (res.CUSTOMERCLASS != "")
                    {
                        string t = BindMasterData(res.CUSTOMERCLASS, "CUSTOMER CLASS");
                        if (t.Length > 0) res.CUSTOMERCLASS = t;
                    }
                    if (res.CTRY != "")
                    {
                        string t = BindMasterData(res.CTRY, "COUNTRY");
                        if (t.Length > 0) res.CTRY = t;
                    }

                    if (res.INDUSTRY != "")
                    {
                        string t = BindMasterData(res.INDUSTRY, "INDUSTRY");
                        if (t.Length > 0) res.INDUSTRY = t;
                    }
                    if (res.CTIMEZONE != "")
                    {
                        string t = BindMasterData(res.CTIMEZONE, "TIMEZONE");
                        if (t.Length > 0) res.CTIMEZONE = t;
                    }
                    if (res.EXCHANGERATETYPE != "")
                    {
                        string t = BindMasterData(res.EXCHANGERATETYPE, "EXCHANGE RATE TYPE");
                        if (t.Length > 0) res.EXCHANGERATETYPE = t;
                    }
                    if (res.SALESDISTRICT != "")
                    {
                        string t = BindMasterData(res.SALESDISTRICT, "SALES DISTRICT");
                        if (t.Length > 0) res.SALESDISTRICT = t;
                    }
                    if (res.SALESOFFICE != "")
                    {
                        string t = BindMasterData(res.SALESOFFICE, "SALES OFFICE");
                        if (t.Length > 0) res.SALESOFFICE = t;
                    }
                    if (res.SALESGROUP != "")
                    {
                        string t = BindMasterData(res.SALESGROUP, "SALES GROUP");
                        if (t.Length > 0) res.SALESGROUP = t;
                    }
                    if (res.CUSTOMERGROUP != "")
                    {
                        string t = BindMasterData(res.CUSTOMERGROUP, "CUSTOMER GROUP");
                        if (t.Length > 0) res.CUSTOMERGROUP = t;
                    }
                    if (res.SALESCURRENCY != "")
                    {
                        string t = BindMasterData(res.SALESCURRENCY, "CURRENCY");
                        if (t.Length > 0) res.SALESCURRENCY = t;
                    }
                    if (res.CUSTPRICPROC1 != "")
                    {
                        string t = BindMasterData(res.CUSTPRICPROC1, "CUST. PRICE");
                        if (t.Length > 0) res.CUSTPRICPROC1 = t;
                    }
                    if (res.DELIVERYPRIORITYGROUP != "")
                    {
                        string t = BindMasterData(res.DELIVERYPRIORITYGROUP, "DELIVERY PRIORITY");
                        if (t.Length > 0) res.DELIVERYPRIORITYGROUP = t;
                    }
                    if (res.SHIPPINGCONDITION != "")
                    {
                        string t = BindMasterData(res.SHIPPINGCONDITION, "SHIPPING CONDITIONS");
                        if (t.Length > 0) res.SHIPPINGCONDITION = t;
                    }
                    if (res.INCOTERM1 != "")
                    {
                        string t = BindMasterData(res.INCOTERM1, "INCOTERMS");
                        if (t.Length > 0) res.INCOTERM1 = t;
                    }
                    if (res.TERMOFPAYMENT != "")
                    {
                        string t = BindMasterData(res.TERMOFPAYMENT, "TERMS OF PAYMENT");
                        if (t.Length > 0) res.TERMOFPAYMENT = t;
                    }
                    if (res.CREDITCONTROLAREA != "")
                    {
                        string t = BindMasterData(res.CREDITCONTROLAREA, "CREDIT CONTROL AREA");
                        if (t.Length > 0) res.CREDITCONTROLAREA = t;
                    }
                    if (res.ACCASSIGMENTGROUP != "")
                    {
                        string t = BindMasterData(res.ACCASSIGMENTGROUP, "ACCT ASSIGNMENT GROUP");
                        if (t.Length > 0) res.ACCASSIGMENTGROUP = t;
                    }
                    if (res.TAXCLASSIFICATION != "")
                    {
                        string t = BindMasterData(res.TAXCLASSIFICATION, "TAX CLASSIFICATION");
                        if (t.Length > 0) res.TAXCLASSIFICATION = t;
                    }

                    if (res.DISTRIBTUION_CHH != "")
                    {
                        string t = BindMasterData(res.DISTRIBTUION_CHH, "DISTRIBUTION");
                        if (t.Length > 0) res.DISTRIBTUION_CHH = t;
                    }

                    if (res.BANKREGION != "")
                    {
                        string t = BindMasterData(res.BANKREGION, "REGION");
                        if (t.Length > 0) res.BANKREGION = t;
                    }
                    if (res.BANKCURRENCY != "")
                    {
                        string t = BindMasterData(res.BANKCURRENCY, "CURRENCY");
                        if (t.Length > 0) res.BANKCURRENCY = t;
                    }

                    res.TITLE = GetTitleDesc(res.TITLE);
                }


                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("ResetCMRequest", ex.Message);
                return res;
            }
        }
        public CMRequestHistoryDetail GetApprovalHistory(string RequestDetailID, string userId)
        {
            CMRequestHistoryDetail cmhist = new CMRequestHistoryDetail();
            try
            {
                DataSet ds = new DataSet();
                ds = oMasterQueries.GetApprovalDetails(userId, RequestDetailID);
                List<CMRequestHistoryModel> datalist = new List<CMRequestHistoryModel>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var tmp = new CMRequestHistoryModel();
                        tmp.Role = row["Role"].ToString();
                        tmp.UserId = row["UserId"].ToString();
                        tmp.EName = row["EName"].ToString();
                        tmp.Action = row["Action"].ToString();
                        tmp.ActionDate = row["ACTION_DATE"].ToString();
                        tmp.ActionRemarks = row["ACTION_REMARKS"].ToString();
                        datalist.Add(tmp);
                    }

                }
                cmhist.Historymodle = datalist;

                if (ds.Tables[1].Rows.Count > 0)
                {
                    cmhist.RequesterDetail = ds.Tables[1].Rows[0]["REQUESTER_DETAIL"].ToString();
                    cmhist.RequestDate = ds.Tables[1].Rows[0]["REQUESTDATE"].ToString();
                    //lblRecomendationBy.Text = ds.Tables[0].Rows[0]["SUPERVISOR"].ToString();
                    cmhist.SectionDetail = ds.Tables[1].Rows[0]["SECTION_DETAIL"].ToString();
                    cmhist.DeptDetail = ds.Tables[1].Rows[0]["DEPT_DETAIL"].ToString();
                    cmhist.FinDetail = ds.Tables[1].Rows[0]["FIN_DETAIL"].ToString();
                    cmhist.SectionStatus = ds.Tables[1].Rows[0]["SECTION_STATUS"].ToString();
                    cmhist.DeptStatus = ds.Tables[1].Rows[0]["DEPT_STATUS"].ToString();
                    cmhist.FinStatus = ds.Tables[1].Rows[0]["FIN_STATUS"].ToString();
                    cmhist.GenRequestNo = ds.Tables[1].Rows[0]["GEN_REQUEST_NO"].ToString();
                    if (ds.Tables[1].Rows[0]["FIN_DETAIL2"].ToString() != "")
                    {
                        cmhist.FinStatus2 = ds.Tables[1].Rows[0]["FIN_STATUS2"].ToString();
                        cmhist.FinDetail2 = ds.Tables[1].Rows[0]["FIN_DETAIL2"].ToString();
                    }

                }
                return cmhist;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetApprovalHistory", ex.Message);
                return cmhist;
            }
          
           
        }
        public string btnExcelExport(List<CustomerRequestRpt> dt)
        {
            string res = "";
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (dt.Count > 0)
                {
                    stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                    stringBuilder.Append("<tr style='background-color: lightgray;'>");
                    stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                    stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Request No</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Type</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Customer Type</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Customer Acc Group</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Division Group</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Distribution Channel</th>");
                    stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Customer Code</th>");
                    stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Name1</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Status</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Approval Status</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Requester(Initiate)</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Last Action By</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Last Action Date</th>");
                    stringBuilder.Append("</tr>");
                    int srNo = 1;
                    foreach (var row in dt)
                    {
                        stringBuilder.Append("<tr>");
                        stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + row.GEN_REQUEST_NO + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + row.REQUEST_TYPE + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + row.CUST_ACC_TYPE + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + row.DIVISION_GRP + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + row.DISTRIBTUION_CHH + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + row.CUSTOMER_CODE + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + row.NAME1 + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + row.Status + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + row.DETAIL_STATUS + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + row.REQUESTERID + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + row.LASTMODIFIEDBY + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + row.LASTMODIFIEDDATE + "</td>");
                        stringBuilder.Append("</tr>");
                    }
                    stringBuilder.Append("</table>");
                    res = stringBuilder.ToString();
                }
                   
                    return res;
                }
            catch (Exception ex)
            {
                _logger.LogError("btnExport", ex.Message);
                return "";
            }
        }
        public List<CMRequestModel> BindMMCreationRequestDetails(long userid)
        {
            DataSet ds = new DataSet();
            try
            {
                ds = oMasterQueries.GetApprovalRequestDetails(userid);
                if (ds.Tables[0].Rows.Count > 0)
                {      // CMHEADERID ENAME   GEN_REQUEST_NO CUSTOMER_CODE   NAME CUST_ACC_TYPE   REQUEST_TYPE Status  REQUESTDATE REQUEST_TYPE1             
                    var dataList = ds.Tables[0]?.AsEnumerable()
                   .Select(r => new CMRequestModel
                   {
                       CMHEADERID = r.Field<long>("CMHEADERID"),
                       ENAME = r.Field<string>("ENAME"),
                       GEN_REQUEST_NO = r.Field<string>("GEN_REQUEST_NO"),
                       CUSTOMER_CODE = r.Field<string>("CUSTOMER_CODE"),
                       NAME = r.Field<string>("NAME"),
                       CUST_ACC_TYPE = r.Field<string>("CUST_ACC_TYPE"),
                       REQUEST_TYPE = r.Field<string>("REQUEST_TYPE"),
                       Status = r.Field<string>("Status"),
                       REQUESTDATE = r.Field<DateTime>("REQUESTDATE"),
                       REQUEST_TYPE1 = r.Field<string>("REQUEST_TYPE1")                    

                   })
                   .ToList() ?? new List<CMRequestModel>();
                    return dataList ?? (new List<CMRequestModel>());
                }
                else
                {
                    return (new List<CMRequestModel>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("BindMMCreationRequestDetails", ex.Message);
                return new List<CMRequestModel>();
            }
        }
        public ApprovalResponse ApprovalRequestSubmit(ApprovalReqest data)
        {
            var res = new ApprovalResponse();

            DataSet ds = new DataSet();
            try
            {
                long UserId = data.UserId??0;
                long cmheaderid = data.cmheadid??0;
                string request_no = data.request_no??"";
                int action = data.action??0;

                string rs = oMasterQueries.ApprovalRequest(UserId, cmheaderid, request_no, action, data.Remarks);
                if (rs == "1")
                {
                    res.RS = 1;
                    string strSubject = "";
                    switch (data.action)
                    {
                        case 1:
                            strSubject = "Pending for the Customer Master Approval";
                            oMasterQueries.SENDMAILTODEPTHEAD(UserId, request_no, strSubject, "A", data.Customer_Code, data.Name1);
                            res.message = "Successfully Approved.";
                            res.redirecturl= "CMRequestApproval";
                            break;
                        case 0:
                            strSubject = "Pending for the Customer Master Approval";
                            oMasterQueries.SENDMAILTODEPTHEAD(UserId, request_no, strSubject, "R", data.Customer_Code, data.Name1);
                            res.message = "Successfully Rejected.";
                            res.redirecturl = "CMRequestApproval";
                            break;
                        case 2:
                            strSubject = "Pending for the Customer Master resubmit";
                            oMasterQueries.SENDMAILTODEPTHEAD(UserId, request_no, strSubject, "B", data.Customer_Code, data.Name1);
                            res.message = "Successfully send Backed.";
                            res.redirecturl = "CMRequestApproval";
                            break;
                    }
                    return res;
                }
                else
                {
                    return new ApprovalResponse { RS = 0, message = rs };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ApprovalRequestSubmit", ex.Message);
                return new ApprovalResponse { RS=0, message=ex.Message};
            }
        }
        public List<CustomerRequestRpt> ShowCustomerSyncData(CustomerSyncInput _data)
        {
            try
            {
                string REQUESTTYPE = _data.REQUESTTYPE ?? "";
                string CUSTACCGROUP = _data.CUSTACCGROUP ?? "";
                string REQDATEFROM = _data.REQDATEFROM ?? "";
                string REQDATETO = _data.REQDATETO ?? "";
                string CUSTCODE = _data.CUSTCODE ?? "";
                string STATUS = _data.STATUS ?? "";
                string UserID = _data.UserId ?? "";
                DataSet ds;
                ds = oMasterQueries.GETCUSTOMERREQUESTDataSync(UserID, STATUS, "", REQUESTTYPE, CUSTACCGROUP, CUSTCODE, REQDATEFROM, REQDATETO);
                
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //var data = dc.TableToList<CustomerRequestRpt>(ds.Tables[0]);
                    var dataList = ds.Tables[0]?.AsEnumerable()
                   .Select(r => new CustomerRequestRpt
                   {                       
                       REQUESTERID = r.Field<long>("REQUESTERID"),
                       REQUEST_TYPE = r.Field<string>("REQUEST_TYPE"),                       
                       REQUESTDATE = r.Field<string>("REQUESTDATE"),
                       GEN_REQUEST_NO = r.Field<string>("GEN_REQUEST_NO"),
                       CUSTOMER_CODE = r.Field<string>("CUSTOMER_CODE"),
                       //CUSTOMER_TYPE = r.Field<string>("CUSTOMER_TYPE"),
                       CUST_ACC_TYPE = r.Field<string>("CUST_ACC_TYPE"),
                       NAME1 = r.Field<string>("NAME1"),
                       Status = r.Field<string>("Status"),
                       DETAIL_STATUS = r.Field<string>("DETAIL_STATUS"),
                       CMHEADERID = r.Field<long>("CMHEADERID"),
                       LASTMODIFIEDDATE = r.Field<string>("updateddate"),
                       Sap_Remarks = r.Field<string>("Sap_Remarks")
                       })
                   .ToList() ?? new List<CustomerRequestRpt>();
                    return dataList ?? (new List<CustomerRequestRpt>());
                }
                else
                {
                    return (new List<CustomerRequestRpt>());
                }
            }
            catch (Exception ex)
            {
               
                _logger.LogError("ShowCustomerSyncData", ex.Message);
                return (new List<CustomerRequestRpt>());
            }
        }
        public async Task<CustomerResponseData> SyncCustomerRequest(string cmheaderid, string custAccType, string UserId)
        {
            CustomerResponseData res=new CustomerResponseData();
            try
            {
               

                if (cmheaderid != null && custAccType != null)
                {
                    //string cmheaderid = HDVENDORHEADERID.Value;
                    //string custAccType = HDPROCESSSTATUS.Value;
                    //Datatable saved
                    //Step 1 Get
                    DataSet ds = new DataSet();
                    ds = oMasterQueries.GetDataForSAPSYNC(UserId, cmheaderid, custAccType);
                    DataTable _sapCust = new DataTable();
                    DataTable _sapWT = new DataTable();
                   
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataTable _sapCustBeData = new DataTable();
                        _sapCustBeData = ds.Tables[0];
                        // Add columns to _sapCust
                        foreach (DataColumn col in _sapCustBeData.Columns)
                        {

                            _sapCust.Columns.Add(col.ColumnName, col.DataType);
                            // not copy DateTime data type due to serial
                            //var t = col.DataType;
                            //if (t != typeof(DateTime) && t != typeof(DateTimeOffset))
                            //{
                            //    _sapCust.Columns.Add(col.ColumnName, t);
                            //}


                        }
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string[] DivDataGroupMain = ds.Tables[0].Rows[i]["division_grp"].ToString().Split('|');
                            if (ds.Tables[0].Rows[i]["cust_acc_type"].ToString() == "5AGW")
                            {
                                _sapWT.Columns.Add("CUSTOMER_CODE", typeof(string));
                                _sapWT.Columns.Add("SPART", typeof(string));
                                _sapWT.Columns.Add("WITHHOLDINGTAXTYPE", typeof(string));
                                _sapWT.Columns.Add("WITHHOLDINGTAXCODE", typeof(string));
                                _sapWT.Columns.Add("VALIDFROM", typeof(string));
                                _sapWT.Columns.Add("VALIDTO", typeof(string));
                                _sapWT.Columns.Add("WITHHOLDINGTAXNUMBER", typeof(string));

                                string[] WITHHOLDINGTAXTYPE = ds.Tables[0].Rows[i]["WITHHOLDINGTAXTYPE"].ToString().Split('|');
                                string[] WITHHOLDINGTAXCODE = ds.Tables[0].Rows[i]["WITHHOLDINGTAXCODE"].ToString().Split('|');
                                string[] VALIDFROM = ds.Tables[0].Rows[i]["VALIDFROM"].ToString().Split('|');
                                string[] VALIDTO = ds.Tables[0].Rows[i]["VALIDTO"].ToString().Split('|');
                                string[] WITHHOLDINGTAXNUMBER = ds.Tables[0].Rows[i]["WITHHOLDINGTAXNUMBER"].ToString().Split('|');
                                for (int j = 0; j < WITHHOLDINGTAXTYPE.Length; j++)
                                {
                                    DataRow newRow = _sapWT.NewRow();
                                    newRow["CUSTOMER_CODE"] = ds.Tables[0].Rows[i]["CUSTOMER_CODE"].ToString() + "V";
                                    newRow["SPART"] = "";
                                    newRow["WITHHOLDINGTAXTYPE"] = WITHHOLDINGTAXTYPE[j];
                                    newRow["WITHHOLDINGTAXCODE"] = WITHHOLDINGTAXCODE[j];
                                    newRow["VALIDFROM"] = ConvertDateFormat(VALIDFROM[j]);
                                    newRow["VALIDTO"] = ConvertDateFormat(VALIDTO[j]);
                                    newRow["WITHHOLDINGTAXNUMBER"] = WITHHOLDINGTAXNUMBER[j];
                                    _sapWT.Rows.Add(newRow);
                                }

                                // Iterate through DivDataGroupMain and create new rows in _sapCust
                                foreach (string dataMain in DivDataGroupMain)
                                {
                                    DataRow newRow = _sapCust.NewRow();
                                    foreach (DataColumn col in _sapCustBeData.Columns)
                                    {
                                       
                                            if (col.ColumnName.ToLower() == "division_grp")
                                            {
                                                newRow[col.ColumnName] = dataMain;
                                            }
                                            else if (col.ColumnName.ToUpper() == "CUSTOMER_CODE")
                                            {
                                                //string CUSTOMER_CODE = ds.Tables[0].Rows[i]["CUSTOMER_CODE"].ToString();
                                                newRow[col.ColumnName] = ds.Tables[0].Rows[i]["CUSTOMER_CODE"].ToString() + "V";
                                            }
                                            else//CUSTOMER_CODE
                                            {
                                                newRow[col.ColumnName] = _sapCustBeData.Rows[i][col.ColumnName]; // Assuming you want to copy data from the first row of _sapCustBeData
                                            }
                                                                               
                                    }
                                    //newRow["division_grp"] = dataMain;
                                    //newRow["customer_code"] = newRow["customer_code"].ToString() + ds.Tables[0].Rows[i]["cust_acc_type"].ToString() == "5AGW"?"V":"";
                                    _sapCust.Rows.Add(newRow); // Add new row to _sapCust
                                }
                            }
                            else
                            {
                                // Iterate through DivDataGroupMain and create new rows in _sapCust
                                foreach (string dataMain in DivDataGroupMain)
                                {
                                    DataRow newRow = _sapCust.NewRow();
                                    foreach (DataColumn col in _sapCustBeData.Columns)
                                    {
                                       
                                            if (col.ColumnName.ToLower() == "division_grp")
                                            {
                                                newRow[col.ColumnName] = dataMain;
                                            }
                                            else if (col.ColumnName.ToUpper() == "CUSTOMER_CODE")
                                            {
                                                //string vadd = ds.Tables[0].Rows[i]["cust_acc_type"].ToString() == "5AGW" ? "V" : "";
                                                // string CUSTOMER_CODE = ds.Tables[0].Rows[i]["CUSTOMER_CODE"].ToString();
                                                newRow[col.ColumnName] = ds.Tables[0].Rows[i]["CUSTOMER_CODE"].ToString();
                                            }
                                            else//CUSTOMER_CODE
                                            {
                                                newRow[col.ColumnName] = _sapCustBeData.Rows[i][col.ColumnName]; // Assuming you want to copy data from the first row of _sapCustBeData
                                            }
                                      

                                      
                                    }
                                    //newRow["division_grp"] = dataMain;
                                    //newRow["customer_code"] = newRow["customer_code"].ToString() + ds.Tables[0].Rows[i]["cust_acc_type"].ToString() == "5AGW"?"V":"";
                                    _sapCust.Rows.Add(newRow); // Add new row to _sapCust
                                }
                            }

                        }
                        ////////////////////////////////////////////////
                        ///DataTable to class
                        ///
                        DataTableConverter dc = new DataTableConverter();
                        var dataList = new List<sapCust>();
                        if (_sapCust.Rows.Count > 0)
                        {
                             dataList = dc.TableToList<sapCust>(_sapCust);
                            
                        }
                        var dataList1 = new List<sapWt>();
                        if (_sapWT.Rows.Count > 0)
                        {
                            dataList1 = dc.TableToList<sapWt>(_sapWT);

                        }

                        // Creating an empty DataTable
                        // EportalESS EPOBJ = new EportalESS();
                        // DataTable resultTable = EPOBJ.POST_CUSTOMER_MASTER(_sapCust, _sapWT);
                        DataTable resultTable = await objess.POST_CUSTOMER_MASTER(dataList, dataList1);
                        if (resultTable != null && resultTable.Rows.Count > 0)
                        {
                            string valueMsg = "";
                            DataTable fsapCust = resultTable;                            
                            foreach (DataRow row in fsapCust.Rows)
                            {
                                string uid = row["UID"].ToString();
                                string kunnr = row["KUNNR"].ToString();
                                string spart = row["SPART"].ToString();
                                string zStatus = row["ZStatus"].ToString();
                                string value = row["VALUE"].ToString();
                                valueMsg = " " + value;
                                //CMOBJ.InsertIntoSAPRFCLOG(Session["userID"].ToString(),uid, kunnr, spart, zStatus, value);
                                string syncStatus = oMasterQueries.InsertIntoSAPRFCLOG(UserId, uid, kunnr, spart, zStatus, value);
                                //UpdateSyncStatus(syncStatus);
                                //binddata();
                                if (syncStatus == "1")
                                {
                                    res.MESSAGE = "Sync Successful: User Created";                                  
                                }
                                else if (syncStatus == "2")
                                {
                                    res.MESSAGE = "Sync Successful: User Updated";
                                }
                                else
                                {
                                    res.MESSAGE = "Error Unsuccessful";                                  
                                }
                                res.RS = 1;                              
                                res.FILE1 = valueMsg;
                            }
                        }

                    }
                    else
                    {
                        res.RS = 0;
                        res.MESSAGE = "RFC not return proper value!";
                    }
                
                }
                else
                {
                    res.RS = 0;
                    res.MESSAGE = "Invalid parameters values";
                    _logger.LogError("SyncCustomerRequest", "Parameters values not found within the RepeaterItem container.");
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError("SyncCustomerRequest", ex.Message);
                res.RS = 1;
                res.MESSAGE=ex.Message;
                return res;
            }
        }
        protected string ConvertDateFormat(string df)
        {
            string dt = "";
            if (df.Length == 10)
            {
                dt = df.Substring(6, 4) + df.Substring(3, 2) + df.Substring(0, 2);
            }
            return dt;
        }
    }
}
