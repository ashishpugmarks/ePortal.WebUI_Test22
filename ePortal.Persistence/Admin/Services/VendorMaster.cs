using System;
using System.Data;
using System.Globalization;
using System.Runtime.Remoting;
//using System.Web.UI.WebControls;
using System.Xml.Linq;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.ViewModels.APPX.VendorMaster;


//using Microsoft.Office.Interop.Excel;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Client;
//using SAP.Middleware.Connector;

namespace ePortal.Persistence.Admin.Services
{
    public class VendorMaster : IVendorMaster
    {
        //DataManagement odmgt = new DataManagement();
        private readonly IDataManagement odmgt;
        private readonly IConnectionString objCnStr;

        public VendorMaster(IDataManagement _odmgt, IConnectionString _objCnStr)
        {
            odmgt = _odmgt;
            objCnStr = _objCnStr;
        }
        public DataSet GET_DEPARTMENTHEAD(string ECODE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_DEPARTMENTHEAD_GET";
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("CUR_APPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_OPHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string GetCurrentPage(Uri uri)
        {
            string[] segment = uri.Segments;
            string page = string.Empty;
            if (0 < segment.Length)
            {
                page = segment[segment.Length - 1];
            }
            return page;
        }
        //added parameter
        //ADDED BY AJIT TTL
        public string VENDORMASTER_INFORMATION_INSERT(VendorMasterDetails vmd) //CR6782
        {
            try
            {
                OracleCommand ocmd = new OracleCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.BindByName = true;
                ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORINFORMATION_INSERT";
                ocmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = vmd.REQECODE;
                ocmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = vmd.REQUESTTYPE;
                ocmd.Parameters.Add("VENDORACCGRP_IN", OracleDbType.Varchar2).Value = vmd.VENDORACCGRP;
                ocmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = vmd.VENDORCODE;
                ocmd.Parameters.Add("CSVATTACHMENT_IN", OracleDbType.Varchar2).Value = vmd.CSVATTACHMENT;
                ocmd.Parameters.Add("DEPTHEAD_IN", OracleDbType.Varchar2).Value = vmd.DEPTHEAD;
                ocmd.Parameters.Add("VENDORNAME1_IN", OracleDbType.Varchar2).Value = vmd.VENDORNAME1;
                ocmd.Parameters.Add("VENDORNAME2_IN", OracleDbType.Varchar2).Value = vmd.VENDORNAME2;
                ocmd.Parameters.Add("VENDORNAME3_IN", OracleDbType.Varchar2).Value = vmd.VENDORNAME3;
                ocmd.Parameters.Add("STREET1_IN", OracleDbType.Varchar2).Value = vmd.STREET1;
                ocmd.Parameters.Add("STREET2_IN", OracleDbType.Varchar2).Value = vmd.STREET2;
                ocmd.Parameters.Add("STREET3_IN", OracleDbType.Varchar2).Value = vmd.STREET3;
                ocmd.Parameters.Add("STREET4_IN", OracleDbType.Varchar2).Value = vmd.STREET4;
                ocmd.Parameters.Add("CITY_IN", OracleDbType.Varchar2).Value = vmd.CITY;
                ocmd.Parameters.Add("REGION_IN", OracleDbType.Varchar2).Value = vmd.REGION;
                ocmd.Parameters.Add("POSTALCODE_IN", OracleDbType.Varchar2).Value = vmd.POSTALCODE;
                ocmd.Parameters.Add("COUNTRY_IN", OracleDbType.Varchar2).Value = vmd.COUNTRY;
                ocmd.Parameters.Add("MOBILENO_IN", OracleDbType.Varchar2).Value = vmd.MOBILENO;
                ocmd.Parameters.Add("TELEPHONENO_IN", OracleDbType.Varchar2).Value = vmd.TELEPHONENO;
                ocmd.Parameters.Add("EMAIL1_IN", OracleDbType.Varchar2).Value = vmd.EMAIL1;
                ocmd.Parameters.Add("EMAIL2_IN", OracleDbType.Varchar2).Value = vmd.EMAIL2;
                ocmd.Parameters.Add("EMAIL3_IN", OracleDbType.Varchar2).Value = vmd.EMAIL3;
                ocmd.Parameters.Add("MSMEINFOSTATUS_IN", OracleDbType.Varchar2).Value = vmd.MSMEINFOSTATUS;
                ocmd.Parameters.Add("MSMECATEGORY_IN", OracleDbType.Varchar2).Value = vmd.MSMECATEGORY;
                ocmd.Parameters.Add("MSMECERTIFICATION_IN", OracleDbType.Varchar2).Value = vmd.MSMECERTIFICATION;
                ocmd.Parameters.Add("SERVICEAGENTGRP_IN", OracleDbType.Varchar2).Value = vmd.SERVICEAGENTGRP;
                ocmd.Parameters.Add("BANKCOUNTRY_IN", OracleDbType.Varchar2).Value = vmd.BANKCOUNTRY;
                ocmd.Parameters.Add("BANKNAME_IN", OracleDbType.Varchar2).Value = vmd.BANKNAME;
                ocmd.Parameters.Add("BRANCHNAME_IN", OracleDbType.Varchar2).Value = vmd.BRANCHNAME;
                ocmd.Parameters.Add("BANKADDRESS_IN", OracleDbType.Varchar2).Value = vmd.BANKADDRESS;
                ocmd.Parameters.Add("BANKCITY_IN", OracleDbType.Varchar2).Value = vmd.BANKCITY;
                ocmd.Parameters.Add("BANKSTATE_IN", OracleDbType.Varchar2).Value = vmd.BANKSTATE;
                ocmd.Parameters.Add("TYPEOFACCOUNT_IN", OracleDbType.Varchar2).Value = vmd.TYPEOFACCOUNT;
                ocmd.Parameters.Add("BANKACCNO_IN", OracleDbType.Varchar2).Value = vmd.BANKACCNO;
                ocmd.Parameters.Add("IFSCCODE_IN", OracleDbType.Varchar2).Value = vmd.IFSCCODE;
                ocmd.Parameters.Add("BANKCATEGORY_IN", OracleDbType.Varchar2).Value = vmd.BANKCATEGORY;
                ocmd.Parameters.Add("SCHEMAGROUP_IN", OracleDbType.Varchar2).Value = vmd.SCHEMAGROUP;
                ocmd.Parameters.Add("ORDERCURRENCY_IN", OracleDbType.Varchar2).Value = vmd.ORDERCURRENCY;
                ocmd.Parameters.Add("PANNUMBER_IN", OracleDbType.Varchar2).Value = vmd.PANNUMBER;
                ocmd.Parameters.Add("CSTREGNUMBER_IN", OracleDbType.Varchar2).Value = vmd.CSTREGNUMBER;
                ocmd.Parameters.Add("LSTNUMBER_IN", OracleDbType.Varchar2).Value = vmd.LSTNUMBER;
                ocmd.Parameters.Add("SERVICEREGNUMBER_IN", OracleDbType.Varchar2).Value = vmd.SERVICEREGNUMBER;
                ocmd.Parameters.Add("ECCNUMBER_IN", OracleDbType.Varchar2).Value = vmd.ECCNUMBER;
                ocmd.Parameters.Add("EXCISEREGNO_IN", OracleDbType.Varchar2).Value = vmd.EXCISEREGNO;
                ocmd.Parameters.Add("EXCISERANGE_IN", OracleDbType.Varchar2).Value = vmd.EXCISERANGE;
                ocmd.Parameters.Add("EXCISEDIVISION_IN", OracleDbType.Varchar2).Value = vmd.EXCISEDIVISION;
                ocmd.Parameters.Add("COMMISTIONERATE_IN", OracleDbType.Varchar2).Value = vmd.COMMISTIONERATE;
                ocmd.Parameters.Add("FILE_REGCERTIFICATENO_IN", OracleDbType.Varchar2).Value = vmd.FILE_REGCERTIFICATENO;
                ocmd.Parameters.Add("FILE_MANDATEFORM_IN", OracleDbType.Varchar2).Value = vmd.FILE_MANDATEFORM;
                ocmd.Parameters.Add("FILE_CANCELCHEQUE_IN", OracleDbType.Varchar2).Value = vmd.FILE_CANCELCHEQUE;
                ocmd.Parameters.Add("FILE_PANCARD_IN", OracleDbType.Varchar2).Value = vmd.FILE_PANCARD;
                ocmd.Parameters.Add("FILE_SERVICEREGCERTIFICATE_IN", OracleDbType.Varchar2).Value = vmd.FILE_SERVICEREGCERTIFICATE;
                ocmd.Parameters.Add("FILE_CSTCERTIFICATE_IN", OracleDbType.Varchar2).Value = vmd.FILE_CSTCERTIFICATE;
                ocmd.Parameters.Add("FILE_EXCISEREGCERTIFICATE_IN", OracleDbType.Varchar2).Value = vmd.FILE_EXCISEREGCERTIFICATE;
                ocmd.Parameters.Add("FILE_MSMECERTIFICATE_IN", OracleDbType.Varchar2).Value = vmd.FILE_MSMECERTIFICATE;
                ocmd.Parameters.Add("GSTIN_IN", OracleDbType.Varchar2).Value = vmd.GSTIN;
                ocmd.Parameters.Add("GSTCLASSIFICATION_IN", OracleDbType.Varchar2).Value = vmd.GSTCLASSIFICATION;
                ocmd.Parameters.Add("FILE_GSTCERTIFICATE_IN", OracleDbType.Varchar2).Value = vmd.FILE_GSTCERTIFICATE;
                ocmd.Parameters.Add("EINVOICE_APPLICABLE_IN", OracleDbType.Int16).Value = vmd.E_INVOICEApplicable;
                ocmd.Parameters.Add("FILE_EINVOICE_IN", OracleDbType.Varchar2).Value = vmd.FILE_EINVOICE;
                ocmd.Parameters.Add("MSMEFROM_IN", OracleDbType.Varchar2).Value = vmd.MSMEFROM;
                ocmd.Parameters.Add("MSMETO_IN", OracleDbType.Varchar2).Value = vmd.MSMETO;
                ocmd.Parameters.Add("MSMECITY_IN", OracleDbType.Varchar2).Value = vmd.MSMECITY;
                ocmd.Parameters.Add("LEIAPPLICABLE_IN", OracleDbType.Varchar2).Value = vmd.LEIAPPLICABLE;
                ocmd.Parameters.Add("LEINO_IN", OracleDbType.Varchar2).Value = vmd.LEINO;
                ocmd.Parameters.Add("FILE_LEI_IN", OracleDbType.Varchar2).Value = vmd.FILE_LEICERTIFICATE;
                ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = vmd.REMARKS;
                ocmd.Parameters.Add("SPLAPPROVAL_IN", OracleDbType.Varchar2).Value = vmd.SPLAPPROVAL;
                ocmd.Parameters.Add("FILE_CONFLICTCER_IN", OracleDbType.Varchar2).Value = vmd.FILE_CONFLICTCERTIFICATE;
                ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                // Added by TTL Ajit 04-DEC-24
                ocmd.Parameters.Add("VENDORHEADERID_OUT", OracleDbType.Long, 10).Direction = ParameterDirection.Output;

                ocmd.Parameters.Add("SWIFTCODE_IN", OracleDbType.Varchar2).Value = vmd.SWIFTCODE;
                ocmd.Parameters.Add("IBAN_NO_IN", OracleDbType.Varchar2).Value = vmd.IBANNO;
                ocmd.Parameters.Add("REFERENCE_DETAILS_IN", OracleDbType.Varchar2).Value = vmd.REFERENCE_DETAIL;
                ocmd.Parameters.Add("INTER_BANKACCNO_IN", OracleDbType.Varchar2).Value = vmd.I_BANKACCNO;
                ocmd.Parameters.Add("INTER_BANKNAME_IN", OracleDbType.Varchar2).Value = vmd.I_BANKNAME;
                ocmd.Parameters.Add("INTER_BANKADDRESS_IN", OracleDbType.Varchar2).Value = vmd.I_BANKADDRESS;
                ocmd.Parameters.Add("INTER_BANKCITY_IN", OracleDbType.Varchar2).Value = vmd.I_BANKCITY;
                ocmd.Parameters.Add("INTER_BANKSTATE_IN", OracleDbType.Varchar2).Value = vmd.I_BANKSTATE;
                ocmd.Parameters.Add("INTER_BANKCOUNTRY_IN", OracleDbType.Varchar2).Value = vmd.I_BANKCOUNTRY;
                ocmd.Parameters.Add("INTER_BANKBRANCH_IN", OracleDbType.Varchar2).Value = vmd.I_BRANCHNAME;
                ocmd.Parameters.Add("INTER_TYPEOFACC_IN", OracleDbType.Varchar2).Value = vmd.I_TYPEOFACCOUNT;
                ocmd.Parameters.Add("INTER_SWIFTCOCE_IN", OracleDbType.Varchar2).Value = vmd.I_SWIFTCODE;
                ocmd.Parameters.Add("INTER_IBANNO_IN", OracleDbType.Varchar2).Value = vmd.I_IBANNO;
                ocmd.Parameters.Add("INTER_BANKCATEGORY_IN", OracleDbType.Varchar2).Value = vmd.I_BANKCATEGORY;
                ocmd.Parameters.Add("INTER_BANK_KEY_IN", OracleDbType.Varchar2).Value = vmd.I_BANK_KEY;
                ocmd.Parameters.Add("INTER_REFERENCE_DETAILS_IN", OracleDbType.Varchar2).Value = vmd.INTER_REFERENCE_DETAIL;
                //CR6782       
                ocmd.Parameters.Add("TYPE_OF_INDUSTRY_IN", OracleDbType.Varchar2).Value = vmd.TYPE_OF_INDUSTRY;
                ocmd.Parameters.Add("CLASSIFICATION_OF_YEAR_IN", OracleDbType.Varchar2).Value = vmd.CLASSIFICATION_OF_YEAR;
                ocmd.Parameters.Add("DATE_OF_CLASSIFICATION_IN", OracleDbType.Varchar2).Value = vmd.DATE_OF_CLASSIFICATION;
                odmgt.ExecuteQuery(ocmd);
                //string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
                // Added by TTL Ajit 04-DEC-24
                string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString() + "#" + ocmd.Parameters["VENDORHEADERID_OUT"].Value.ToString();
                return strErr;
                ocmd.Dispose();
            }
            catch (Exception ex)
            {
                return "0 #Error msg " + ex.Message + "#0";
            }
        }

        public DataSet GETVENDORPENDINGREQUEST(string ecode, string status, string VENDORHEADERID, string REQUESTTYPE, string VENDORACCGROUP, string VENDORNAME, string REQDATEFROM, string REQDATETO)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORREQUEST_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("VMHEADERID_IN", OracleDbType.Varchar2).Value = VENDORHEADERID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
            oCmd.Parameters.Add("VENDORACCGROUP_IN", OracleDbType.Varchar2).Value = VENDORACCGROUP;
            oCmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            oCmd.Parameters.Add("REQDATEFROM_IN", OracleDbType.Varchar2).Value = REQDATEFROM;
            oCmd.Parameters.Add("REQDATETO_IN", OracleDbType.Varchar2).Value = REQDATETO;
            oCmd.Parameters.Add("CUR_PENDREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output; //CUR_WTHTAX
            oCmd.Parameters.Add("CUR_WTHTAX", OracleDbType.RefCursor).Direction = ParameterDirection.Output; // Added by TTL Ajit 04-DEC-24
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public DataSet VENDORREQUESTBYID(string VenderHeaderID)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataTable dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORREQUESTBYID_GET";
                oCmd.Parameters.Add("VMHEADERID_IN", OracleDbType.Varchar2).Value = VenderHeaderID;
                oCmd.Parameters.Add("CUR_VMREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("CURPROCHIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("CUROLDREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("CURINTERBANK", OracleDbType.RefCursor).Direction = ParameterDirection.Output; // Added by TTL Ajit 04-DEC-24
                oCmd.Parameters.Add("CURHOLDTAX", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
                DataSet ds = odmgt.GetDataSet(oCmd);
                oCmd.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public DataSet GETVENDORAPPROVALGREQUEST(string ecode, string status, string REQUESTTYPE, string VENDORACCGROUP, string VENDORNAME)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORAPPROVALREQ_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
            oCmd.Parameters.Add("VENDORACCGROUP_IN", OracleDbType.Varchar2).Value = VENDORACCGROUP;
            oCmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            oCmd.Parameters.Add("CUR_APPREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public string VENDORREQUESTAPPROVAL(string VENDORHEADERID, string STRREMARKS, string STATUS, string APPECODE, string APPLEVEL, string STRATTACHMENT, string SPLAPPCODE, string SPLAPPSTATUS)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORMASTER_APPROVAL";
            ocmd.Parameters.Add("VENDORHEADERID_IN", OracleDbType.Varchar2).Value = VENDORHEADERID;
            ocmd.Parameters.Add("STRREMARKS_IN", OracleDbType.Varchar2).Value = STRREMARKS;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("APPECODE_IN", OracleDbType.Varchar2).Value = APPECODE;
            ocmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = "";
            ocmd.Parameters.Add("APPLEVEL_IN", OracleDbType.Varchar2).Value = APPLEVEL;
            ocmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = STRATTACHMENT;
            ocmd.Parameters.Add("SPLAPPECODE_IN", OracleDbType.Varchar2).Value = SPLAPPCODE;
            ocmd.Parameters.Add("SPLAPPSTATUS_IN", OracleDbType.Varchar2).Value = SPLAPPSTATUS;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public string SISVENDORREQUESTAPPROVAL(string VENDORHEADERID, string STRREMARKS, string STATUS, string APPECODE, string REQTYPE, VENDORLIST objvmlist, INTERMEDIARYBANK interbank, WTHTAX[] withtaxs)
        {
            // Added by TTL Ajit 04-DEC-24
            CultureInfo provider = CultureInfo.InvariantCulture;
            string rtnval = string.Empty;
            string subResult = string.Empty;
            string strErr = string.Empty;
            string strErrmsg = string.Empty;
            string VENDORCODE_IN = "";
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction tran;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                tran = objCn.BeginTransaction();
                try
                {
                    //VENDOR MASTER DETAIL UPDATE IN SAP
                    if (objvmlist != null)
                    {
                        //EportalESS objess = new EportalESS();
                        //RfcConfigParameters parms = new RfcConfigParameters();
                        //parms = objess.sapconnection();
                        //RfcDestination rfcDest1 = RfcDestinationManager.GetDestination(parms);
                        //RfcRepository rfcRep1 = rfcDest1.Repository;
                        //IRfcFunction function1 = rfcRep1.CreateFunction("ZRFC_FI_VENDOR_REG_PROCESS");
                        //function1.SetValue("I_CHNG_IND", REQTYPE);
                        //IRfcStructure objst = function1.GetStructure("I_VNDR_DET");
                        //objst.SetValue("LIFNR", objvmlist.Lifnr);
                        //objst.SetValue("KTOKK", objvmlist.Ktokk);
                        //objst.SetValue("SORT1", objvmlist.Sort1);
                        //objst.SetValue("COUNTRY", objvmlist.Country);
                        //objst.SetValue("LANGU", objvmlist.Langu);
                        //objst.SetValue("NAME1", objvmlist.Name1);
                        //objst.SetValue("NAME2", objvmlist.Name2);
                        //objst.SetValue("NAME3", objvmlist.Name3);
                        //objst.SetValue("ADDRESS1", objvmlist.Address1);
                        //objst.SetValue("ADDRESS2", objvmlist.Address2);
                        //objst.SetValue("ADDRESS3", objvmlist.Address3);
                        //objst.SetValue("ADDRESS4", objvmlist.Address4);
                        //objst.SetValue("CITY", objvmlist.City);
                        //objst.SetValue("DISTRICT", objvmlist.District);
                        //objst.SetValue("REGION", objvmlist.Region);
                        //objst.SetValue("PO_BOX", objvmlist.Post_Code1);
                        //objst.SetValue("POST_CODE1", objvmlist.Post_Code1);
                        //objst.SetValue("POST_CODE2", objvmlist.Post_Code1);
                        //objst.SetValue("MOBILE_NO", objvmlist.Mobile_No);
                        //objst.SetValue("TELEPHONE_NO", objvmlist.Telephone_No);
                        //objst.SetValue("FAX_NO", objvmlist.Fax_No);
                        //objst.SetValue("EMAIL1", objvmlist.Email1);
                        //objst.SetValue("EMAIL2", objvmlist.Email2);
                        //objst.SetValue("EMAIL3", objvmlist.Email3);
                        //objst.SetValue("MSME_STAT_INFO", objvmlist.Msme_Stat_Info);
                        //objst.SetValue("MSME_CATEGORY", objvmlist.Msme_Category);
                        //objst.SetValue("MSME_CERTIFICATE", objvmlist.Msme_Certificate);
                        //objst.SetValue("BANKA", objvmlist.Banka);
                        //objst.SetValue("BNK_STRAS", objvmlist.Bnk_Stras);
                        //objst.SetValue("BNK_ORT01", objvmlist.Bnk_Ort01);
                        //objst.SetValue("PROVZ", objvmlist.Provz);
                        //objst.SetValue("BRNCH", objvmlist.Brnch);
                        //objst.SetValue("SWIFT", objvmlist.Swift);
                        //objst.SetValue("BANKS", objvmlist.Banks);
                        //objst.SetValue("BANKL", objvmlist.Bankl);
                        //objst.SetValue("BANKN", objvmlist.Bankn);
                        //objst.SetValue("BKONT", objvmlist.Bkont);
                        //objst.SetValue("BVTYP", objvmlist.Bvtyp);
                        //objst.SetValue("XEZER", objvmlist.Xezer);
                        //objst.SetValue("BKREF", objvmlist.Bkref);
                        //objst.SetValue("KOINH", objvmlist.Koinh);
                        //objst.SetValue("BUKRS", objvmlist.Bukrs);
                        //objst.SetValue("AKONT", objvmlist.Akont);
                        //objst.SetValue("ZUAWA", "01");
                        //objst.SetValue("ZTERM", "0001");
                        //objst.SetValue("ZWELS", objvmlist.Zwels);
                        //objst.SetValue("HBKID", objvmlist.Hbkid);
                        //objst.SetValue("EKORG", objvmlist.Ekorg);
                        //objst.SetValue("WAERS", objvmlist.Waers);
                        //objst.SetValue("WEBRE", objvmlist.Webre);
                        //objst.SetValue("MINBW", "0");
                        //objst.SetValue("KALSK", objvmlist.Kalsk);
                        //objst.SetValue("KZAUT", objvmlist.Kzaut);
                        //objst.SetValue("LEBRE", objvmlist.Lebre);
                        //objst.SetValue("XNBWY", objvmlist.Xnbwy);
                        //objst.SetValue("PLIFZ", "0");
                        //objst.SetValue("BSTAE", "0004");
                        //objst.SetValue("VENSL", "0");
                        //objst.SetValue("J_1IPANNO", objvmlist.J_1ipanno);
                        //objst.SetValue("J_1IEXCD", objvmlist.J_1iexcd);
                        //objst.SetValue("J_1isern", objvmlist.J_1isern);
                        //objst.SetValue("J_1iexrn", objvmlist.J_1iexrn);
                        //objst.SetValue("J_1iexrg", objvmlist.J_1iexrg);
                        //objst.SetValue("J_1iexdi", objvmlist.J_1iexdi);
                        //objst.SetValue("J_1iexco", objvmlist.J_1iexco);
                        //objst.SetValue("J_1ICSTNO", objvmlist.J_1icstno);
                        //objst.SetValue("J_1ILSTNO", objvmlist.J_1ilstno);
                        //objst.SetValue("J_1ISERN", objvmlist.LEI_NO);
                        //objst.SetValue("DLGRP", objvmlist.Dlgrp);
                        //objst.SetValue("STCD3", objvmlist.STCD3);
                        //objst.SetValue("VEN_CLASS", objvmlist.VEN_CLASS);
                        //objst.SetValue("EINV_VALID", objvmlist.EINV_VALID);
                        //objst.SetValue("LEI_STATUS_INFO", objvmlist.LEI_STATUS);
                        //objst.SetValue("MSME_VALID_FROM", objvmlist.MSME_FROM);
                        //objst.SetValue("MSME_VALID_TO", objvmlist.MSME_TO);
                        //objst.SetValue("MSME_REG_CITY", objvmlist.MSME_CITY);
                        ////----------Added by TTL CR6782-------------------
                        //objst.SetValue("BRSCH", objvmlist.TypeOfIndustry);
                        //objst.SetValue("ZCLS_YR", objvmlist.ClassificationYear);
                        //DateTime validFrom;
                        //if (objvmlist.DateOfClassification != "")
                        //{
                        //    validFrom = DateTime.ParseExact(objvmlist.DateOfClassification, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        //    objst.SetValue("ZDAT_CLS", validFrom);
                        //}

                        ////objst.SetValue("VALID_FROM", objvmlist.DateOfClassification);
                        ////--------------------------------------------------------------------------------
                        ////*********aDDED BY TTL AJIT
                        //if (interbank != null)
                        //{

                        //    IRfcStructure objinb = function1.GetStructure("I_VBDR_INTMENDTRY");
                        //    objinb.SetValue("BANKA", interbank.BANKA);
                        //    objinb.SetValue("BANKL", interbank.BANKL);
                        //    objinb.SetValue("BANKN", interbank.BANKN);
                        //    objinb.SetValue("BANKS", interbank.BANKS);
                        //    objinb.SetValue("BKONT", interbank.BKONT);
                        //    objinb.SetValue("BKREF", interbank.BKREF);
                        //    objinb.SetValue("BNK_ORT01", interbank.BNK_ORT01);
                        //    objinb.SetValue("BNK_STRAS", interbank.BNK_STRAS);
                        //    objinb.SetValue("BRNCH", interbank.BRNCH);
                        //    objinb.SetValue("KOINH", interbank.KOINH);
                        //    objinb.SetValue("PROVZ", interbank.PROVZ);
                        //    objinb.SetValue("SWIFT", interbank.SWIFT);
                        //    objinb.SetValue("IBAN", interbank.IBAN);
                        //}


                        ////------------------------------------------
                        //RfcSessionManager.BeginContext(rfcDest1);
                        //function1.Invoke(rfcDest1);
                        //IRfcStructure strreturn = function1.GetStructure("E_VNDR_DET");
                        //IRfcTable tblReturn = function1.GetTable("E_RETURN");
                        //DataTable sapTable1 = new DataTable();

                        ////DataTable rowTable = new DataTable();
                        ////for (int i = 0; i <= strreturn.ElementCount - 1; i++)
                        ////{
                        ////    rowTable.Columns.Add(strreturn.GetElementMetadata(i).Name);
                        ////}

                        ////DataRow row1 = rowTable.NewRow();
                        ////for (int j = 0; j <= strreturn.ElementCount - 1; j++)
                        ////{
                        ////    row1[j] = strreturn.GetValue(j);
                        ////}

                        ////rowTable.Rows.Add(row1);

                        //DataTable sapTable = new DataTable();

                        //for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
                        //{
                        //    RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
                        //    sapTable.Columns.Add(metadata.Name);
                        //}

                        //foreach (IRfcStructure row in tblReturn)
                        //{
                        //    DataRow sapdr = sapTable.NewRow();
                        //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
                        //    {
                        //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
                        //        sapdr[metadata.Name] = row.GetString(metadata.Name);
                        //    }
                        //    sapTable.Rows.Add(sapdr);
                        //}
                        //if (sapTable.Rows.Count > 0)
                        //{
                        //    subResult = sapTable.Rows[0]["Log_No"].ToString().Trim();
                        //    VENDORCODE_IN = sapTable.Rows[0]["Message_V1"].ToString();
                        //    strErr = "<ul>";
                        //    for (int i = 0; i < sapTable.Rows.Count; i++)
                        //    {
                        //        strErr = strErr + "<li>SAP - " + sapTable.Rows[i]["Message"].ToString() + "</li>";
                        //    }
                        //    strErr = strErr + "</ul>";
                        //    if (subResult == "1")
                        //    {
                        //        if (REQTYPE == "x")
                        //        {
                        //            strErrmsg = "Vendor " + VENDORCODE_IN + " has been updated Successfully";

                        //        }
                        //        else
                        //        {
                        //            strErrmsg = "Vendor " + VENDORCODE_IN + " has been created Successfully";
                        //        }
                        //    }
                        //}
                        ////--------------------------------------------------------------------------------
                        ////*********aDDED BY TTL AJIT
                        ////************************HOLDING TAX SAP POST********************************************************//
                        //if (subResult == "1" && withtaxs != null)
                        //{
                        //    IRfcFunction function2 = rfcRep1.CreateFunction("ZMM_RFC_VENDOR_WITHTAX");
                        //    function2.SetValue("VENCODE", VENDORCODE_IN);
                        //    IRfcTable rfcTable = function2.GetTable("WITHTAX");
                        //    foreach (WTHTAX withtax in withtaxs)
                        //    {
                        //        DateTime from_Date = DateTime.Now;
                        //        DateTime to_Date = DateTime.Now;
                        //        from_Date = DateTime.ParseExact(withtax.WT_EXDF, "dd.MM.yyyy", provider);
                        //        to_Date = DateTime.ParseExact(withtax.WT_EXDT, "dd.MM.yyyy", provider);
                        //        rfcTable.Append();
                        //        rfcTable.SetValue("WITHT", withtax.WITHT);
                        //        rfcTable.SetValue("WT_WITHCD", withtax.WT_WTSTCD);
                        //        rfcTable.SetValue("WT_SUBJCT", withtax.WT_SUBJCT);
                        //        rfcTable.SetValue("WT_QSREC", withtax.WT_QSREC);
                        //        rfcTable.SetValue("WT_WTSTCD", withtax.WT_WTSTCD);
                        //        rfcTable.SetValue("WT_EXNR", withtax.WT_EXNR);
                        //        rfcTable.SetValue("WT_EXRT", withtax.WT_EXRT);
                        //        rfcTable.SetValue("WT_EXDF", from_Date);
                        //        rfcTable.SetValue("WT_EXDT", to_Date);
                        //        rfcTable.SetValue("WT_WTEXRS", withtax.WT_WTEXRS);
                        //    }



                        //    RfcSessionManager.BeginContext(rfcDest1);
                        //    function2.Invoke(rfcDest1);

                        //    // Retrieve and process output
                        //    IRfcTable itResult = function2.GetTable("E_RETURN");
                        //    DataTable resultTable = new DataTable();

                        //    for (int liElement = 0; liElement < itResult.ElementCount; liElement++)
                        //    {
                        //        RfcElementMetadata metadata = itResult.GetElementMetadata(liElement);
                        //        resultTable.Columns.Add(metadata.Name);
                        //    }

                        //    foreach (IRfcStructure row in itResult)
                        //    {
                        //        DataRow resultRow = resultTable.NewRow();
                        //        for (int liElement = 0; liElement < itResult.ElementCount; liElement++)
                        //        {
                        //            RfcElementMetadata metadata = itResult.GetElementMetadata(liElement);
                        //            resultRow[metadata.Name] = row.GetString(metadata.Name);
                        //        }
                        //        resultTable.Rows.Add(resultRow);
                        //    }
                        //}


                        //// End RFC session
                        //RfcSessionManager.EndContext(rfcDest1);


                    }

                    if (subResult == "1")
                    {

                        //Update Status and insert on master table
                        OracleCommand oCmd = new OracleCommand();
                        oCmd.Connection = objCn;
                        oCmd.Transaction = tran;
                        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        oCmd.BindByName = true;
                        oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORMASTER_APPROVAL";
                        oCmd.Parameters.Add("VENDORHEADERID_IN", OracleDbType.Varchar2).Value = VENDORHEADERID;
                        oCmd.Parameters.Add("STRREMARKS_IN", OracleDbType.Varchar2).Value = STRREMARKS;
                        oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
                        oCmd.Parameters.Add("APPECODE_IN", OracleDbType.Varchar2).Value = APPECODE;
                        oCmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE_IN;
                        oCmd.Parameters.Add("APPLEVEL_IN", OracleDbType.Varchar2).Value = "4";
                        oCmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = "";
                        oCmd.Parameters.Add("SPLAPPECODE_IN", OracleDbType.Varchar2).Value = "";
                        oCmd.Parameters.Add("SPLAPPSTATUS_IN", OracleDbType.Varchar2).Value = "0";
                        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                        oCmd.ExecuteNonQuery();
                        subResult = oCmd.Parameters["RESULT_OUT"].Value.ToString();
                        strErr = "<ul><li>" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString() + "</li></ul>";
                        if (subResult != "1")
                        {
                            tran.Rollback();
                        }
                        else
                        {
                            oCmd.Parameters.Clear();
                            oCmd.Transaction = tran;
                            tran.Commit();
                        }
                    }
                    rtnval = subResult + "#" + strErr + "#" + strErrmsg + "#" + VENDORCODE_IN;
                }
                catch (Exception ex)
                {
                    if (tran.Connection != null)
                    {
                        tran.Rollback();
                        rtnval = "0#<ul><li>" + ex.Message + "</li></ul>#0#";
                    }
                    else
                    {
                        rtnval = "0#<ul><li>" + ex.Message + "</li></ul>#0#";
                    }
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return rtnval;
        }

        public string INSERTAPPROVALAUTHORITY(string approvalauthorityid, string site, string conapprovalauthority, string addedby, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_INSERTAPPROVALAUTHORITY";
            ocmd.Parameters.Add("APPROVALAUTHORITYID_IN", OracleDbType.Varchar2).Value = approvalauthorityid;
            ocmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = site;
            ocmd.Parameters.Add("CONAPPROVALAUTHORITY_IN", OracleDbType.Varchar2).Value = conapprovalauthority;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
        }

        public DataSet GETAPPROVALAUTHORITY(string approvalauthorityid, string site, string conapprovalauthority, string status)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_APPROVALAUTHORITY_GET";
            ocmd.Parameters.Add("APPROVALAUTHORITYID_IN", OracleDbType.Varchar2).Value = approvalauthorityid;
            ocmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = site;
            ocmd.Parameters.Add("CONAPPROVALAUTHORITY_IN", OracleDbType.Varchar2).Value = conapprovalauthority;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            ocmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataSet GETSITE()
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_SITE_GET";
            ocmd.Parameters.Add("CUR_SITELIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataSet GET_VENDORFINANCEDEPTAPPROVALLIST(string SITE, string ECODE, string ENAME, string STATUS, string FINECODE, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION)
        {

            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_FINDEPTREQUEST_GET";
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            ocmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            ocmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            ocmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = SECTION;
            ocmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = SITE;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("FINECODE_IN", OracleDbType.Varchar2).Value = FINECODE;
            ocmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataSet GET_VENDORSISDEPTREQUESTEDLIST(string SITE, string ECODE, string ENAME, string STATUS, string SISECODE, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_SISDEPTREQUEST_GET";
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            ocmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            ocmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            ocmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = SECTION;
            ocmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = SITE;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("SISECODE_IN", OracleDbType.Varchar2).Value = SISECODE;
            ocmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string VENDORDETAILVALIDATE(string REQUESTTYPE, string VENDORACCGRP, string VENDORCODE, string VENDORNAME1, 
            string VENDORNAME2, string VENDORNAME3, string STREET1, string STREET2, string STREET3, string STREET4, string CITY, 
            string REGION, string POSTALCODE, string COUNTRY, string MOBILENO, string TELEPHONENO, string EMAIL1, string EMAIL2, 
            string EMAIL3, string MSMEINFOSTATUS, string MSMECATEGORY, string MSMECERTIFICATION, string SERVICEAGENTGRP, string BANKCOUNTRY, 
            string BANKNAME, string BRANCHNAME, string BANKADDRESS, string TYPEOFACCOUNT, string BANKCITY, string BANKSTATE, 
            string BANKACCNO, string IFSCCODE, string BANKCATEGORY, string SCHEMAGROUP, string ORDERCURRENCY, string PANNUMBER, 
            string CSTREGNUMBER, string LSTNUMBER, string SERVICEREGNUMBER, string ECCNUMBER, string EXCISEREGNO, string EXCISERANGE, 
            string EXCISEDIVISION, string COMMISTIONERATE, string GSTIN, string GSTCLASSIFICATION, string E_INVOICEApplicable, 
            string MSMEFROM, string MSMETO, string MSMECITY, string LEIAPPLICABLE, string LEINO, string SWIFTCODE_IN, string IBANNO_IN, 
            string IBANKACCNO_IN, string IBRANCHNAME_IN, string IBANKADDRESS_IN, string IBANKCITY_IN, string IBANKSTATE_IN, 
            string BANKCOUNTRY_IN, string ITYPEOFACCOUNT_IN, string IBANKCATEGORY_IN, string ISWIFTCODE_IN, string IIBANNO_IN, 
            string IBANK_KEY, string TYPE_OF_INDUSTRY, string CLASSIFICATION_OF_YEAR, string DATE_OF_CLASSIFICATION) //CR6782
        { 
            //Added by Ajit TTL
            if (SERVICEAGENTGRP.Length != 0) { SERVICEAGENTGRP = SERVICEAGENTGRP.PadLeft(4, '0'); }

            using (OracleCommand ocmd = new OracleCommand())
            {
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.BindByName = true;
                ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORDETAILVALIDATE";
                ocmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
                ocmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
                ocmd.Parameters.Add("VENDORACCGRP_IN", OracleDbType.Varchar2).Value = VENDORACCGRP;
                ocmd.Parameters.Add("VENDORNAME1_IN", OracleDbType.Varchar2).Value = VENDORNAME1;
                ocmd.Parameters.Add("VENDORNAME2_IN", OracleDbType.Varchar2).Value = VENDORNAME2;
                ocmd.Parameters.Add("VENDORNAME3_IN", OracleDbType.Varchar2).Value = VENDORNAME3;
                ocmd.Parameters.Add("STREET1_IN", OracleDbType.Varchar2).Value = STREET1;
                ocmd.Parameters.Add("STREET2_IN", OracleDbType.Varchar2).Value = STREET2;
                ocmd.Parameters.Add("STREET3_IN", OracleDbType.Varchar2).Value = STREET3;
                ocmd.Parameters.Add("STREET4_IN", OracleDbType.Varchar2).Value = STREET4;
                ocmd.Parameters.Add("CITY_IN", OracleDbType.Varchar2).Value = CITY;
                ocmd.Parameters.Add("REGION_IN", OracleDbType.Varchar2).Value = REGION;
                ocmd.Parameters.Add("POSTALCODE_IN", OracleDbType.Varchar2).Value = POSTALCODE;
                ocmd.Parameters.Add("COUNTRY_IN", OracleDbType.Varchar2).Value = COUNTRY;
                ocmd.Parameters.Add("MOBILENO_IN", OracleDbType.Varchar2).Value = MOBILENO;
                ocmd.Parameters.Add("TELEPHONENO_IN", OracleDbType.Varchar2).Value = TELEPHONENO;
                ocmd.Parameters.Add("EMAIL1_IN", OracleDbType.Varchar2).Value = EMAIL1;
                ocmd.Parameters.Add("EMAIL2_IN", OracleDbType.Varchar2).Value = EMAIL2;
                ocmd.Parameters.Add("EMAIL3_IN", OracleDbType.Varchar2).Value = EMAIL3;
                ocmd.Parameters.Add("MSMEINFOSTATUS_IN", OracleDbType.Varchar2).Value = MSMEINFOSTATUS;
                ocmd.Parameters.Add("MSMECATEGORY_IN", OracleDbType.Varchar2).Value = MSMECATEGORY;
                ocmd.Parameters.Add("MSMECERTIFICATION_IN", OracleDbType.Varchar2).Value = MSMECERTIFICATION;
                ocmd.Parameters.Add("SERVICEAGENTGRP_IN", OracleDbType.Varchar2).Value = SERVICEAGENTGRP;
                ocmd.Parameters.Add("BANKCOUNTRY_IN", OracleDbType.Varchar2).Value = BANKCOUNTRY;
                ocmd.Parameters.Add("BANKNAME_IN", OracleDbType.Varchar2).Value = BANKNAME;
                ocmd.Parameters.Add("BRANCHNAME_IN", OracleDbType.Varchar2).Value = BRANCHNAME;
                ocmd.Parameters.Add("BANKADDRESS_IN", OracleDbType.Varchar2).Value = BANKADDRESS;
                ocmd.Parameters.Add("BANKCITY_IN", OracleDbType.Varchar2).Value = BANKCITY;
                ocmd.Parameters.Add("BANKSTATE_IN", OracleDbType.Varchar2).Value = BANKSTATE;
                ocmd.Parameters.Add("TYPEOFACCOUNT_IN", OracleDbType.Varchar2).Value = TYPEOFACCOUNT;
                ocmd.Parameters.Add("BANKACCNO_IN", OracleDbType.Varchar2).Value = BANKACCNO;
                ocmd.Parameters.Add("IFSCCODE_IN", OracleDbType.Varchar2).Value = IFSCCODE;
                ocmd.Parameters.Add("BANKCATEGORY_IN", OracleDbType.Varchar2).Value = BANKCATEGORY;
                ocmd.Parameters.Add("SCHEMAGROUP_IN", OracleDbType.Varchar2).Value = SCHEMAGROUP;
                ocmd.Parameters.Add("ORDERCURRENCY_IN", OracleDbType.Varchar2).Value = ORDERCURRENCY;
                ocmd.Parameters.Add("PANNUMBER_IN", OracleDbType.Varchar2).Value = PANNUMBER;
                ocmd.Parameters.Add("CSTREGNUMBER_IN", OracleDbType.Varchar2).Value = CSTREGNUMBER;
                ocmd.Parameters.Add("LSTNUMBER_IN", OracleDbType.Varchar2).Value = LSTNUMBER;
                ocmd.Parameters.Add("SERVICEREGNUMBER_IN", OracleDbType.Varchar2).Value = SERVICEREGNUMBER;
                ocmd.Parameters.Add("ECCNUMBER_IN", OracleDbType.Varchar2).Value = ECCNUMBER;
                ocmd.Parameters.Add("EXCISEREGNO_IN", OracleDbType.Varchar2).Value = EXCISEREGNO;
                ocmd.Parameters.Add("EXCISERANGE_IN", OracleDbType.Varchar2).Value = EXCISERANGE;
                ocmd.Parameters.Add("EXCISEDIVISION_IN", OracleDbType.Varchar2).Value = EXCISEDIVISION;
                ocmd.Parameters.Add("COMMISTIONERATE_IN", OracleDbType.Varchar2).Value = COMMISTIONERATE;
                ocmd.Parameters.Add("GSTIN_IN", OracleDbType.Varchar2).Value = GSTIN;
                ocmd.Parameters.Add("GSTCLASSIFICATION_IN", OracleDbType.Varchar2).Value = GSTCLASSIFICATION;
                ocmd.Parameters.Add("EINVOICE_IN", OracleDbType.Varchar2).Value = E_INVOICEApplicable;
                ocmd.Parameters.Add("MSMEFROM_IN", OracleDbType.Varchar2).Value = MSMEFROM;
                ocmd.Parameters.Add("MSMETO_IN", OracleDbType.Varchar2).Value = MSMETO;
                ocmd.Parameters.Add("MSMECITY_IN", OracleDbType.Varchar2).Value = MSMECITY;
                ocmd.Parameters.Add("LEIAPPLICABLE_IN", OracleDbType.Varchar2).Value = LEIAPPLICABLE;
                ocmd.Parameters.Add("LEINO_IN", OracleDbType.Varchar2).Value = LEINO;
                ocmd.Parameters.Add("SWIFTCODE_IN", OracleDbType.Varchar2).Value = SWIFTCODE_IN; // Added by TTL Ajit 04-DEC-24
                ocmd.Parameters.Add("IBANNO_IN", OracleDbType.Varchar2).Value = IBANNO_IN;
                ocmd.Parameters.Add("IBANKACCNO_IN", OracleDbType.Varchar2).Value = IBANKACCNO_IN;
                ocmd.Parameters.Add("IBRANCHNAME_IN", OracleDbType.Varchar2).Value = IBRANCHNAME_IN;
                ocmd.Parameters.Add("IBANKADDRESS_IN", OracleDbType.Varchar2).Value = IBANKADDRESS_IN;
                ocmd.Parameters.Add("IBANKCITY_IN", OracleDbType.Varchar2).Value = IBANKCITY_IN;
                ocmd.Parameters.Add("IBANKSTATE_IN", OracleDbType.Varchar2).Value = IBANKSTATE_IN;
                ocmd.Parameters.Add("IBANKCOUNTRY_IN", OracleDbType.Varchar2).Value = BANKCOUNTRY_IN;
                ocmd.Parameters.Add("ITYPEOFACCOUNT_IN", OracleDbType.Varchar2).Value = ITYPEOFACCOUNT_IN;
                ocmd.Parameters.Add("IBANKCATEGORY_IN", OracleDbType.Varchar2).Value = IBANKCATEGORY_IN;
                ocmd.Parameters.Add("ISWIFTCODE_IN", OracleDbType.Varchar2).Value = ISWIFTCODE_IN;
                ocmd.Parameters.Add("IIBANNO_IN", OracleDbType.Varchar2).Value = IIBANNO_IN;
                ocmd.Parameters.Add("IBANK_KEY_IN", OracleDbType.Varchar2).Value = IBANK_KEY;
                //CR6782       
                ocmd.Parameters.Add("TYPE_OF_INDUSTRY_IN", OracleDbType.Varchar2).Value = TYPE_OF_INDUSTRY;
                ocmd.Parameters.Add("CLASSIFICATION_OF_YEAR_IN", OracleDbType.Varchar2).Value = CLASSIFICATION_OF_YEAR;
                ocmd.Parameters.Add("DATE_OF_CLASSIFICATION_IN", OracleDbType.Varchar2).Value = DATE_OF_CLASSIFICATION;

                ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                odmgt.ExecuteQuery(ocmd);
                string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
                return strErr;
                ocmd.Dispose();
            }
        }

        public DataSet VENDORREQUESTBYVEDNORCODE(string VENDORCODE)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_REQUESTBYVEDNORCODE_GET";
            oCmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            oCmd.Parameters.Add("CUROLDREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }
        // SWIFTCODE, IBANNO, I_BANKACCNO, I_BANKNAME, I_BANKADDRESS, I_BANKCITY,
        //I_BANKSTATE, I_BANKCOUNTRY, I_BRANCHNAME, I_TYPEOFACCOUNT, I_SWIFTCODE, I_IBANNO, I_BANKCATEGORY, I_BANK_KEY, REFERENCE_DETAIL, I_REFERENCE_DETAIL,
        //                                                                    TypeofIndustry, ClassificationOfYear, DateOfClassification
        public string VENDORMASTER_INFORMATION_UPDATE(VendorMasterDetails vmd) //CR6782
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORINFORMATION_UPDATE";
            ocmd.Parameters.Add("VENDORHEADERID_IN", OracleDbType.Varchar2).Value= vmd.REQUESTID;
            ocmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value= vmd.REQECODE;
            ocmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value= vmd.REQUESTTYPE;
            ocmd.Parameters.Add("VENDORACCGRP_IN", OracleDbType.Varchar2).Value= vmd.VENDORACCGRP;
            ocmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value= vmd.VENDORCODE;
            ocmd.Parameters.Add("CSVATTACHMENT_IN", OracleDbType.Varchar2).Value= vmd.CSVATTACHMENT;
            ocmd.Parameters.Add("DEPTHEAD_IN", OracleDbType.Varchar2).Value= vmd.DEPTHEAD;
            ocmd.Parameters.Add("VENDORNAME1_IN", OracleDbType.Varchar2).Value= vmd.VENDORNAME1;
            ocmd.Parameters.Add("VENDORNAME2_IN", OracleDbType.Varchar2).Value= vmd.VENDORNAME2;
            ocmd.Parameters.Add("VENDORNAME3_IN", OracleDbType.Varchar2).Value= vmd.VENDORNAME3;
            ocmd.Parameters.Add("STREET1_IN", OracleDbType.Varchar2).Value= vmd.STREET1;
            ocmd.Parameters.Add("STREET2_IN", OracleDbType.Varchar2).Value= vmd.STREET2;
            ocmd.Parameters.Add("STREET3_IN", OracleDbType.Varchar2).Value= vmd.STREET3;
            ocmd.Parameters.Add("STREET4_IN", OracleDbType.Varchar2).Value= vmd.STREET4;
            ocmd.Parameters.Add("CITY_IN", OracleDbType.Varchar2).Value= vmd.CITY;
            ocmd.Parameters.Add("REGION_IN", OracleDbType.Varchar2).Value= vmd.REGION;
            ocmd.Parameters.Add("POSTALCODE_IN", OracleDbType.Varchar2).Value= vmd.POSTALCODE;
            ocmd.Parameters.Add("COUNTRY_IN", OracleDbType.Varchar2).Value= vmd.COUNTRY;
            ocmd.Parameters.Add("MOBILENO_IN", OracleDbType.Varchar2).Value= vmd.MOBILENO;
            ocmd.Parameters.Add("TELEPHONENO_IN", OracleDbType.Varchar2).Value= vmd.TELEPHONENO;
            ocmd.Parameters.Add("EMAIL1_IN", OracleDbType.Varchar2).Value= vmd.EMAIL1;
            ocmd.Parameters.Add("EMAIL2_IN", OracleDbType.Varchar2).Value= vmd.EMAIL2;
            ocmd.Parameters.Add("EMAIL3_IN", OracleDbType.Varchar2).Value= vmd.EMAIL3;
            ocmd.Parameters.Add("MSMEINFOSTATUS_IN", OracleDbType.Varchar2).Value= vmd.MSMEINFOSTATUS;
            ocmd.Parameters.Add("MSMECATEGORY_IN", OracleDbType.Varchar2).Value= vmd.MSMECATEGORY;
            ocmd.Parameters.Add("MSMECERTIFICATION_IN", OracleDbType.Varchar2).Value= vmd.MSMECERTIFICATION;
            ocmd.Parameters.Add("SERVICEAGENTGRP_IN", OracleDbType.Varchar2).Value= vmd.SERVICEAGENTGRP;
            ocmd.Parameters.Add("BANKCOUNTRY_IN", OracleDbType.Varchar2).Value= vmd.BANKCOUNTRY;
            ocmd.Parameters.Add("BANKNAME_IN", OracleDbType.Varchar2).Value= vmd.BANKNAME;
            ocmd.Parameters.Add("BRANCHNAME_IN", OracleDbType.Varchar2).Value= vmd.BRANCHNAME;
            ocmd.Parameters.Add("BANKADDRESS_IN", OracleDbType.Varchar2).Value= vmd.BANKADDRESS;
            ocmd.Parameters.Add("BANKCITY_IN", OracleDbType.Varchar2).Value= vmd.BANKCITY;
            ocmd.Parameters.Add("BANKSTATE_IN", OracleDbType.Varchar2).Value= vmd.BANKSTATE;
            ocmd.Parameters.Add("TYPEOFACCOUNT_IN", OracleDbType.Varchar2).Value= vmd.TYPEOFACCOUNT;
            ocmd.Parameters.Add("BANKACCNO_IN", OracleDbType.Varchar2).Value= vmd.BANKACCNO;
            ocmd.Parameters.Add("IFSCCODE_IN", OracleDbType.Varchar2).Value= vmd.IFSCCODE;
            ocmd.Parameters.Add("BANKCATEGORY_IN", OracleDbType.Varchar2).Value= vmd.BANKCATEGORY;
            ocmd.Parameters.Add("SCHEMAGROUP_IN", OracleDbType.Varchar2).Value= vmd.SCHEMAGROUP;
            ocmd.Parameters.Add("ORDERCURRENCY_IN", OracleDbType.Varchar2).Value= vmd.ORDERCURRENCY;
            ocmd.Parameters.Add("PANNUMBER_IN", OracleDbType.Varchar2).Value= vmd.PANNUMBER;
            ocmd.Parameters.Add("CSTREGNUMBER_IN", OracleDbType.Varchar2).Value= vmd.CSTREGNUMBER;
            ocmd.Parameters.Add("LSTNUMBER_IN", OracleDbType.Varchar2).Value= vmd.LSTNUMBER;
            ocmd.Parameters.Add("SERVICEREGNUMBER_IN", OracleDbType.Varchar2).Value= vmd.SERVICEREGNUMBER;
            ocmd.Parameters.Add("ECCNUMBER_IN", OracleDbType.Varchar2).Value= vmd.ECCNUMBER;
            ocmd.Parameters.Add("EXCISEREGNO_IN", OracleDbType.Varchar2).Value= vmd.EXCISEREGNO;
            ocmd.Parameters.Add("EXCISERANGE_IN", OracleDbType.Varchar2).Value= vmd.EXCISERANGE;
            ocmd.Parameters.Add("EXCISEDIVISION_IN", OracleDbType.Varchar2).Value= vmd.EXCISEDIVISION;
            ocmd.Parameters.Add("COMMISTIONERATE_IN", OracleDbType.Varchar2).Value= vmd.COMMISTIONERATE;
            ocmd.Parameters.Add("FILE_REGCERTIFICATENO_IN", OracleDbType.Varchar2).Value= vmd.FILE_REGCERTIFICATENO;
            ocmd.Parameters.Add("FILE_MANDATEFORM_IN", OracleDbType.Varchar2).Value= vmd.FILE_MANDATEFORM;
            ocmd.Parameters.Add("FILE_CANCELCHEQUE_IN", OracleDbType.Varchar2).Value= vmd.FILE_CANCELCHEQUE;
            ocmd.Parameters.Add("FILE_PANCARD_IN", OracleDbType.Varchar2).Value= vmd.FILE_PANCARD;
            ocmd.Parameters.Add("FILE_SERVICEREGCERTIFICATE_IN", OracleDbType.Varchar2).Value= vmd.FILE_SERVICEREGCERTIFICATE;
            ocmd.Parameters.Add("FILE_CSTCERTIFICATE_IN", OracleDbType.Varchar2).Value= vmd.FILE_CSTCERTIFICATE;
            ocmd.Parameters.Add("FILE_EXCISEREGCERTIFICATE_IN", OracleDbType.Varchar2).Value= vmd.FILE_EXCISEREGCERTIFICATE;
            ocmd.Parameters.Add("FILE_MSMECERTIFICATE_IN", OracleDbType.Varchar2).Value= vmd.FILE_MSMECERTIFICATE;
            ocmd.Parameters.Add("GSTIN_IN", OracleDbType.Varchar2).Value= vmd.GSTIN;
            ocmd.Parameters.Add("GSTCLASSIFICATION_IN", OracleDbType.Varchar2).Value= vmd.GSTCLASSIFICATION;
            ocmd.Parameters.Add("FILE_GSTCERTIFICATE_IN", OracleDbType.Varchar2).Value= vmd.FILE_GSTCERTIFICATE;
            ocmd.Parameters.Add("EINVOICE_APPLICABLE_IN", OracleDbType.Int16).Value= vmd.E_INVOICEApplicable;
            ocmd.Parameters.Add("FILE_EINVOICE_IN", OracleDbType.Varchar2).Value= vmd.FILE_EINVOICE;
            ocmd.Parameters.Add("MSMEFROM_IN", OracleDbType.Varchar2).Value= vmd.MSMEFROM;
            ocmd.Parameters.Add("MSMETO_IN", OracleDbType.Varchar2).Value= vmd.MSMETO;
            ocmd.Parameters.Add("MSMECITY_IN", OracleDbType.Varchar2).Value= vmd.MSMECITY;
            ocmd.Parameters.Add("LEIAPPLICABLE_IN", OracleDbType.Varchar2).Value= vmd.LEI_APPLICABLE;
            ocmd.Parameters.Add("LEINO_IN", OracleDbType.Varchar2).Value= vmd.LEINO;
            ocmd.Parameters.Add("FILE_LEI_IN", OracleDbType.Varchar2).Value= vmd.FILE_LEICERTIFICATE;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value= vmd.REMARKS;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("FILE_CONFCERTIFICATENO_IN", OracleDbType.Varchar2).Value= vmd.FILE_CONFLICTCERTIFICATE;
            //CR6782
            ocmd.Parameters.Add("SWIFTCODE_IN", OracleDbType.Varchar2).Value= vmd.SWIFTCODE;
            ocmd.Parameters.Add("IBAN_NO_IN", OracleDbType.Varchar2).Value= vmd.IBANNO;
            ocmd.Parameters.Add("REFERENCE_DETAILS_IN", OracleDbType.Varchar2).Value= vmd.REFERENCE_DETAIL;
            ocmd.Parameters.Add("INTER_BANKACCNO_IN", OracleDbType.Varchar2).Value= vmd.I_BANKACCNO;
            ocmd.Parameters.Add("INTER_BANKNAME_IN", OracleDbType.Varchar2).Value= vmd.I_BANKNAME;
            ocmd.Parameters.Add("INTER_BANKADDRESS_IN", OracleDbType.Varchar2).Value= vmd.I_BANKADDRESS;
            ocmd.Parameters.Add("INTER_BANKCITY_IN", OracleDbType.Varchar2).Value= vmd.I_BANKCITY;
            ocmd.Parameters.Add("INTER_BANKSTATE_IN", OracleDbType.Varchar2).Value= vmd.I_BANKSTATE;
            ocmd.Parameters.Add("INTER_BANKCOUNTRY_IN", OracleDbType.Varchar2).Value= vmd.I_BANKCOUNTRY;
            ocmd.Parameters.Add("INTER_BANKBRANCH_IN", OracleDbType.Varchar2).Value= vmd.I_BRANCHNAME;
            ocmd.Parameters.Add("INTER_TYPEOFACC_IN", OracleDbType.Varchar2).Value= vmd.I_TYPEOFACCOUNT;
            ocmd.Parameters.Add("INTER_SWIFTCOCE_IN", OracleDbType.Varchar2).Value= vmd.I_SWIFTCODE;
            ocmd.Parameters.Add("INTER_IBANNO_IN", OracleDbType.Varchar2).Value= vmd.I_IBANNO;
            ocmd.Parameters.Add("INTER_BANKCATEGORY_IN", OracleDbType.Varchar2).Value= vmd.I_BANKCATEGORY;
            ocmd.Parameters.Add("INTER_BANK_KEY_IN", OracleDbType.Varchar2).Value= vmd.I_BANK_KEY;
            ocmd.Parameters.Add("INTER_REFERENCE_DETAILS_IN", OracleDbType.Varchar2).Value= vmd.INTER_REFERENCE_DETAIL;
            ocmd.Parameters.Add("TYPE_OF_INDUSTRY_IN", OracleDbType.Varchar2).Value= vmd.TYPE_OF_INDUSTRY;
            ocmd.Parameters.Add("CLASSIFICATION_OF_YEAR_IN", OracleDbType.Varchar2).Value= vmd.CLASSIFICATION_OF_YEAR;
            ocmd.Parameters.Add("DATE_OF_CLASSIFICATION_IN", OracleDbType.Varchar2).Value= vmd.DATE_OF_CLASSIFICATION;
            //VENDORHEADERID_OUT
            ocmd.Parameters.Add("VENDORHEADERID_OUT", OracleDbType.Int32, 6).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString() + "#" + ocmd.Parameters["VENDORHEADERID_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataSet GET_VENDORMASTER_DETAILS(string VENDORHEADERID)
        {
            OracleCommand oCmd = new OracleCommand();
            // cHANGED by TTL Ajit 04-DEC-24
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORMASTERDETAIL_GET";
            oCmd.Parameters.Add("VENDORHEADERID_IN", OracleDbType.Varchar2).Value = VENDORHEADERID;
            oCmd.Parameters.Add("CUR_MSTREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_INTERBANK", OracleDbType.RefCursor).Direction = ParameterDirection.Output; //ADDED BY TTL AJIT
            oCmd.Parameters.Add("CUR_WTHTAX", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public DataSet GETVENDORLISTIFEXISTS(string VENDORNAME1, string VENDORADDRESS, string VENDORCITY, string VENDORACNO, string VENDORPANNO)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORLISTIFEXISTS_GET";
            oCmd.Parameters.Add("VENDORNAME1_IN", OracleDbType.Varchar2).Value = VENDORNAME1;
            oCmd.Parameters.Add("VENDORADDRESS_IN", OracleDbType.Varchar2).Value = VENDORADDRESS;
            oCmd.Parameters.Add("VENDORCITY_IN", OracleDbType.Varchar2).Value = VENDORCITY;
            oCmd.Parameters.Add("VENDORACNO_IN", OracleDbType.Varchar2).Value = VENDORACNO;
            oCmd.Parameters.Add("VENDORPANNO_IN", OracleDbType.Varchar2).Value = VENDORPANNO;
            oCmd.Parameters.Add("CUR_VENLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public DataSet GETKIOPERDIVDEPTSEC(string OPERATION, string DIVISION, string DEPARTMENT)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.GETOPER_DIV_DEPT_SEC";
            oCmd.Parameters.Add("OPER_IN", OracleDbType.Varchar2).Value = OPERATION;
            oCmd.Parameters.Add("DIVI_IN", OracleDbType.Varchar2).Value = DIVISION;
            oCmd.Parameters.Add("DEPT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            oCmd.Parameters.Add("CUR_KI", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_OPER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_DIV", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_DEPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_SEC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public DataSet VENDORMASTERREPORT(string ECODE, string ENAME, string OPERATION, string REQTYPE, string VENDORNAME, string VENDORCODE, string ACCGRP, string SITE, string REQDATEFROM, string REQDATETO, string STATUS, string FLAG_IN)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.VENDORMASTERREPORT_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            oCmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            oCmd.Parameters.Add("REQTYPE_IN", OracleDbType.Varchar2).Value = REQTYPE;
            oCmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            oCmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            oCmd.Parameters.Add("ACCGRP_IN", OracleDbType.Varchar2).Value = ACCGRP;
            oCmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = SITE;
            oCmd.Parameters.Add("REQDATEFROM_IN", OracleDbType.Varchar2).Value = REQDATEFROM;
            oCmd.Parameters.Add("REQDATETO_IN", OracleDbType.Varchar2).Value = REQDATETO;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            oCmd.Parameters.Add("FLAG_IN", OracleDbType.Varchar2).Value = FLAG_IN;
            oCmd.Parameters.Add("CURRPT_IN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public string CONFIRMDUPLICATE_AUTHORITY(string approvalauthorityid, string site)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_CONFIRMDUPLICATE_AUTH";
            ocmd.Parameters.Add("APPAUTHID_IN", OracleDbType.Varchar2).Value = approvalauthorityid;
            ocmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = site;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataSet GET_MANAGEVENDORMASTERLIST(string VENDORCODE, string VENDORNAME, string BANKACCNO, string PANNUMBER, string STATUS)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORMASTERMST_GET";
            ocmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            ocmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            ocmd.Parameters.Add("BANKACCNO_IN", OracleDbType.Varchar2).Value = BANKACCNO;
            ocmd.Parameters.Add("PANNUMBER_IN", OracleDbType.Varchar2).Value = PANNUMBER;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("CUR_VENMST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CURHOLDTAX", OracleDbType.RefCursor).Direction = ParameterDirection.Output; // Added by TTL Ajit 04-DEC-24
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public string MANAGEVENDORMASTERINSERT(string VENDORACCGRP, string VENDORCODE, string VENDORNAME1, string VENDORNAME2,
            string VENDORNAME3, string STREET1, string STREET2, string STREET3, string STREET4, string CITY, string REGION,
            string POSTALCODE, string COUNTRY, string MOBILENO, string TELEPHONENO, string EMAIL1, string EMAIL2, string EMAIL3,
            string MSMEINFOSTATUS, string MSMECATEGORY, string MSMECERTIFICATION, string SERVICEAGENTGRP, string BANKCOUNTRY,
            string BANKNAME, string BRANCHNAME, string BANKADDRESS, string TYPEOFACCOUNT, string BANKCITY, string BANKSTATE,
            string BANKACCNO, string IFSCCODE, string BANKCATEGORY, string SCHEMAGROUP, string ORDERCURRENCY, string PANNUMBER,
            string CSTREGNUMBER, string LSTNUMBER, string SERVICEREGNUMBER, string ECCNUMBER, string EXCISEREGNO,
            string EXCISERANGE, string EXCISEDIVISION, string COMMISTIONERATE, string ADDEDBY, string GSTIN, string GSTCLASSIFICATION)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORMASTERMST_INSERT";
            ocmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            ocmd.Parameters.Add("VENDORACCGRP_IN", OracleDbType.Varchar2).Value = VENDORACCGRP;
            ocmd.Parameters.Add("VENDORNAME1_IN", OracleDbType.Varchar2).Value = VENDORNAME1;
            ocmd.Parameters.Add("VENDORNAME2_IN", OracleDbType.Varchar2).Value = VENDORNAME2;
            ocmd.Parameters.Add("VENDORNAME3_IN", OracleDbType.Varchar2).Value = VENDORNAME3;
            ocmd.Parameters.Add("STREET1_IN", OracleDbType.Varchar2).Value = STREET1;
            ocmd.Parameters.Add("STREET2_IN", OracleDbType.Varchar2).Value = STREET2;
            ocmd.Parameters.Add("STREET3_IN", OracleDbType.Varchar2).Value = STREET3;
            ocmd.Parameters.Add("STREET4_IN", OracleDbType.Varchar2).Value = STREET4;
            ocmd.Parameters.Add("CITY_IN", OracleDbType.Varchar2).Value = CITY;
            ocmd.Parameters.Add("REGION_IN", OracleDbType.Varchar2).Value = REGION;
            ocmd.Parameters.Add("POSTALCODE_IN", OracleDbType.Varchar2).Value = POSTALCODE;
            ocmd.Parameters.Add("COUNTRY_IN", OracleDbType.Varchar2).Value = COUNTRY;
            ocmd.Parameters.Add("MOBILENO_IN", OracleDbType.Varchar2).Value = MOBILENO;
            ocmd.Parameters.Add("TELEPHONENO_IN", OracleDbType.Varchar2).Value = TELEPHONENO;
            ocmd.Parameters.Add("EMAIL1_IN", OracleDbType.Varchar2).Value = EMAIL1;
            ocmd.Parameters.Add("EMAIL2_IN", OracleDbType.Varchar2).Value = EMAIL2;
            ocmd.Parameters.Add("EMAIL3_IN", OracleDbType.Varchar2).Value = EMAIL3;
            ocmd.Parameters.Add("MSMEINFOSTATUS_IN", OracleDbType.Varchar2).Value = MSMEINFOSTATUS;
            ocmd.Parameters.Add("MSMECATEGORY_IN", OracleDbType.Varchar2).Value = MSMECATEGORY;
            ocmd.Parameters.Add("MSMECERTIFICATION_IN", OracleDbType.Varchar2).Value = MSMECERTIFICATION;
            ocmd.Parameters.Add("SERVICEAGENTGRP_IN", OracleDbType.Varchar2).Value = SERVICEAGENTGRP;
            ocmd.Parameters.Add("BANKCOUNTRY_IN", OracleDbType.Varchar2).Value = BANKCOUNTRY;
            ocmd.Parameters.Add("BANKNAME_IN", OracleDbType.Varchar2).Value = BANKNAME;
            ocmd.Parameters.Add("BRANCHNAME_IN", OracleDbType.Varchar2).Value = BRANCHNAME;
            ocmd.Parameters.Add("BANKADDRESS_IN", OracleDbType.Varchar2).Value = BANKADDRESS;
            ocmd.Parameters.Add("BANKCITY_IN", OracleDbType.Varchar2).Value = BANKCITY;
            ocmd.Parameters.Add("BANKSTATE_IN", OracleDbType.Varchar2).Value = BANKSTATE;
            ocmd.Parameters.Add("TYPEOFACCOUNT_IN", OracleDbType.Varchar2).Value = TYPEOFACCOUNT;
            ocmd.Parameters.Add("BANKACCNO_IN", OracleDbType.Varchar2).Value = BANKACCNO;
            ocmd.Parameters.Add("IFSCCODE_IN", OracleDbType.Varchar2).Value = IFSCCODE;
            ocmd.Parameters.Add("BANKCATEGORY_IN", OracleDbType.Varchar2).Value = BANKCATEGORY;
            ocmd.Parameters.Add("SCHEMAGROUP_IN", OracleDbType.Varchar2).Value = SCHEMAGROUP;
            ocmd.Parameters.Add("ORDERCURRENCY_IN", OracleDbType.Varchar2).Value = ORDERCURRENCY;
            ocmd.Parameters.Add("PANNUMBER_IN", OracleDbType.Varchar2).Value = PANNUMBER;
            ocmd.Parameters.Add("CSTREGNUMBER_IN", OracleDbType.Varchar2).Value = CSTREGNUMBER;
            ocmd.Parameters.Add("LSTNUMBER_IN", OracleDbType.Varchar2).Value = LSTNUMBER;
            ocmd.Parameters.Add("SERVICEREGNUMBER_IN", OracleDbType.Varchar2).Value = SERVICEREGNUMBER;
            ocmd.Parameters.Add("ECCNUMBER_IN", OracleDbType.Varchar2).Value = ECCNUMBER;
            ocmd.Parameters.Add("EXCISEREGNO_IN", OracleDbType.Varchar2).Value = EXCISEREGNO;
            ocmd.Parameters.Add("EXCISERANGE_IN", OracleDbType.Varchar2).Value = EXCISERANGE;
            ocmd.Parameters.Add("EXCISEDIVISION_IN", OracleDbType.Varchar2).Value = EXCISEDIVISION;
            ocmd.Parameters.Add("COMMISTIONERATE_IN", OracleDbType.Varchar2).Value = COMMISTIONERATE;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ADDEDBY;

            ocmd.Parameters.Add("GSTIN_IN", OracleDbType.Varchar2).Value = GSTIN;
            ocmd.Parameters.Add("GSTCLASSIFICATION_IN", OracleDbType.Varchar2).Value = GSTCLASSIFICATION;

            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }

        public DataSet GETVENDORLISTMATCHDTL(string VENDORNAME1, string VENDORADDRESS, string VENDORCITY, string VENDORACNO, string VENDORPANNO, string STRADDRESS1, string STRADDRESS2, string STRADDRESS3)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORLISTMATCHDTL_GET";
            oCmd.Parameters.Add("VENDORNAME1_IN", OracleDbType.Varchar2).Value = VENDORNAME1;
            oCmd.Parameters.Add("VENDORADDRESS_IN", OracleDbType.Varchar2).Value = VENDORADDRESS;
            oCmd.Parameters.Add("VENDORCITY_IN", OracleDbType.Varchar2).Value = VENDORCITY;
            oCmd.Parameters.Add("VENDORACNO_IN", OracleDbType.Varchar2).Value = VENDORACNO;
            oCmd.Parameters.Add("VENDORPANNO_IN", OracleDbType.Varchar2).Value = VENDORPANNO;
            oCmd.Parameters.Add("ADDRESS1_IN", OracleDbType.Varchar2).Value = STRADDRESS1;
            oCmd.Parameters.Add("ADDRESS2_IN", OracleDbType.Varchar2).Value = STRADDRESS2;
            oCmd.Parameters.Add("ADDRESS3_IN", OracleDbType.Varchar2).Value = STRADDRESS3;
            oCmd.Parameters.Add("CUR_VENLISTNAME", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_VENLISTADRESS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_VENLISTACCOUNT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_VENLISTPAN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_EMPLISTADRESS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public DataSet GETVENDORLISTMATCHDTLA(string VENDORNAME1, string VENDORADDRESS, string VENDORCITY, string VENDORACNO, string VENDORPANNO, string STRADDRESS1, string STRADDRESS2, string STRADDRESS3)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORLISTMATCHDTLA_GET";
            oCmd.Parameters.Add("VENDORNAME1_IN", OracleDbType.Varchar2).Value = VENDORNAME1;
            oCmd.Parameters.Add("VENDORADDRESS_IN", OracleDbType.Varchar2).Value = VENDORADDRESS;
            oCmd.Parameters.Add("VENDORCITY_IN", OracleDbType.Varchar2).Value = VENDORCITY;
            oCmd.Parameters.Add("VENDORACNO_IN", OracleDbType.Varchar2).Value = VENDORACNO;
            oCmd.Parameters.Add("VENDORPANNO_IN", OracleDbType.Varchar2).Value = VENDORPANNO;
            oCmd.Parameters.Add("ADDRESS1_IN", OracleDbType.Varchar2).Value = STRADDRESS1;
            oCmd.Parameters.Add("ADDRESS2_IN", OracleDbType.Varchar2).Value = STRADDRESS2;
            oCmd.Parameters.Add("ADDRESS3_IN", OracleDbType.Varchar2).Value = STRADDRESS3;
            oCmd.Parameters.Add("CUR_VENLISTNAME", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_VENLISTADRESS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_VENLISTACCOUNT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_VENLISTPAN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_EMPLISTADRESS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }
        // Added by TTL Ajit 04-DEC-24
        public DataSet GET_WHOLDINGTAXMASTER(string ECODE, string ERR_MSG)
        {
            try
            {
                OracleCommand ocmd = new OracleCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.BindByName = true;
                ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_WITHHOLDINGTAXTYPE_GET";
                ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
                ocmd.Parameters.Add("CUR_APPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
                return odmgt.GetDataSet(ocmd);
                ocmd.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                ERR_MSG = ex.Message;
                return null;
            }
        }
        // Added by TTL Ajit 04-DEC-24
        public string GET_WHOLDINGTAXSAVE(int ECODE_IN, string HEADERID_IN, string VWithholding_Tax_Type, string VWithholding_Tax_Code, string VLiable,
            string VRecipient_type, string VWithholding_tax_id_no, string VExemption_certi_no, string VExemption_rate,
            string VDate_On_Which_Exemption_Begins, string VDate_On_Which_Exemption_Ends, string VReason_For_Exemption, int status)
        {
            try
            {
                OracleCommand ocmd = new OracleCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.BindByName = true;
                ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_WITHHOLDINGTAXT_INSERT";
                ocmd.Parameters.Add("ECODE_IN", OracleDbType.Int32).Value = ECODE_IN;
                ocmd.Parameters.Add("HEADERID_IN", OracleDbType.Int64).Value = Convert.ToInt64(HEADERID_IN);
                ocmd.Parameters.Add("VWithholding_Tax_Type", OracleDbType.Varchar2, 2).Value = VWithholding_Tax_Type;
                ocmd.Parameters.Add("VWithholding_Tax_Code", OracleDbType.Varchar2, 2).Value = VWithholding_Tax_Code;
                ocmd.Parameters.Add("VLiable", OracleDbType.Varchar2, 1).Value = VLiable;
                ocmd.Parameters.Add("VRecipient_type", OracleDbType.Varchar2, 2).Value = VRecipient_type;
                ocmd.Parameters.Add("VWithholding_tax_id_no", OracleDbType.Varchar2, 16).Value = VWithholding_tax_id_no;
                ocmd.Parameters.Add("VExemption_certi_no", OracleDbType.Varchar2, 25).Value = VExemption_certi_no;
                ocmd.Parameters.Add("VExemption_rate", OracleDbType.Double).Value = VExemption_rate != "" ? Convert.ToDouble(VExemption_rate) : 0;
                ocmd.Parameters.Add("VDate_On_Which_Exemption_Begins", OracleDbType.Varchar2, 10).Value = VDate_On_Which_Exemption_Begins;
                ocmd.Parameters.Add("VDate_On_Which_Exemption_Ends", OracleDbType.Varchar2, 10).Value = VDate_On_Which_Exemption_Ends;
                ocmd.Parameters.Add("VReason_For_Exemption", OracleDbType.Varchar2, 2).Value = VReason_For_Exemption;
                ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = status;
                ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("ERR_MSG", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
                odmgt.ExecuteQuery(ocmd);
                string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERR_MSG"].Value.ToString();
                return strErr;
                ocmd.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return null;
            }
        }


        //------------------------------Block vendor------------------------------
        public DataSet GETVENDORLISTBLOCKDTL(string VENDORCODE)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORBLOCKLISTDTL_GET";
            oCmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            oCmd.Parameters.Add("CUR_VENLISTDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public string VENDORBLOCK_INFORMATION_INSERT(string REQECODE, string REQUESTTYPE, string REQUESTCAT, string BLOCKPURCHASING, string BLOCKPOSTING, string VENDORCODE, string CSVATTACHMENT,
    string DEPTHEAD, string remarks)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORBLOCKINSERT";
            ocmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = REQECODE;
            ocmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
            ocmd.Parameters.Add("VENDORCATEGORY_IN", OracleDbType.Varchar2).Value = REQUESTCAT;
            ocmd.Parameters.Add("BLOCKPURCHASING_IN", OracleDbType.Varchar2).Value = BLOCKPURCHASING;
            ocmd.Parameters.Add("BLOCKPOSTING_IN", OracleDbType.Varchar2).Value = BLOCKPOSTING;
            ocmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            ocmd.Parameters.Add("CSVATTACHMENT_IN", OracleDbType.Varchar2).Value = CSVATTACHMENT;
            ocmd.Parameters.Add("DEPTHEAD_IN", OracleDbType.Varchar2).Value = DEPTHEAD;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = remarks;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        public DataSet GETVENDORBLOCKPENDINGREQUEST(string ecode, string status, string VENDORHEADERID, string REQUESTTYPE, string REQUESTCATE, string VENDORCODE, string REQDATEFROM, string REQDATETO)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORBLOCKREQUEST_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("VMHEADERID_IN", OracleDbType.Varchar2).Value = VENDORHEADERID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
            oCmd.Parameters.Add("REQUESTCATE_IN", OracleDbType.Varchar2).Value = REQUESTCATE;
            oCmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            oCmd.Parameters.Add("REQDATEFROM_IN", OracleDbType.Varchar2).Value = REQDATEFROM;
            oCmd.Parameters.Add("REQDATETO_IN", OracleDbType.Varchar2).Value = REQDATETO;
            oCmd.Parameters.Add("CUR_PENDREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public DataSet GETVENDORBLOCAPPROVALGREQUEST(string ecode, string status, string REQUESTTYPE, string REQUESTCATE, string VENDORCODE)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORBLOCKAPPREQ_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
            oCmd.Parameters.Add("REQUESTCATE_IN", OracleDbType.Varchar2).Value = REQUESTCATE;
            oCmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            oCmd.Parameters.Add("CUR_APPREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }
        public DataSet VENDORBLOCKREQUESTBYID(string VenderHeaderID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORBLOCKREQBYID_GET";
            oCmd.Parameters.Add("VBHEADERID_IN", OracleDbType.Varchar2).Value = VenderHeaderID;
            oCmd.Parameters.Add("CUR_VBHREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CURPROCHIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_VBDTLREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }
        public string VENDORBLOCKREQAPPROVAL(string VENDORHEADERID, string STRREMARKS, string STATUS, string APPECODE, string APPLEVEL)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORBLOCK_APPROVAL";
            ocmd.Parameters.Add("VENDORHEADERID_IN", OracleDbType.Varchar2).Value = VENDORHEADERID;
            ocmd.Parameters.Add("STRREMARKS_IN", OracleDbType.Varchar2).Value = STRREMARKS;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("APPECODE_IN", OracleDbType.Varchar2).Value = APPECODE;
            ocmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = "";
            ocmd.Parameters.Add("APPLEVEL_IN", OracleDbType.Varchar2).Value = APPLEVEL;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        public DataSet GET_VENDORFINANCEBLOCKAPPROVALLIST(string SITE, string ECODE, string ENAME, string STATUS, string FINECODE, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_FINBLOCKREQUEST_GET";
            ocmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            ocmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
            ocmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            ocmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = SECTION;
            ocmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = SITE;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("FINECODE_IN", OracleDbType.Varchar2).Value = FINECODE;
            ocmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(ocmd);
            ocmd.Dispose();
        }
        public DataSet VENDORBLOCKREQUESTSAP(string VenderHeaderID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORBLOCKREQSAP_GET";
            oCmd.Parameters.Add("VBHEADERID_IN", OracleDbType.Varchar2).Value = VenderHeaderID;
            oCmd.Parameters.Add("CUR_VBDTLREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public string UpdateVendorblockSAP(string vendorcode, string strpurch, string strposting)
        {
            //EportalESS objess = new EportalESS();
            //RfcConfigParameters parms = new RfcConfigParameters();
            //parms = objess.sapconnection();
            //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //RfcRepository rfcRep = rfcDest.Repository;
            //IRfcFunction function = rfcRep.CreateFunction("ZRFC_FI_VENDOR_BLOCK");
            //IRfcStructure objst = function.GetStructure("I_VNDR");
            //objst.SetValue("VENDOR", vendorcode);
            //objst.SetValue("PUR_BLOCK", strpurch);
            //objst.SetValue("POST_BLOCK", strposting);
            //RfcSessionManager.BeginContext(rfcDest);
            //function.Invoke(rfcDest);
            //string msg = function.GetString("E_RETURN");

            //return msg;
            return null;
        }

        public string VENDORBLOCK_INFORMATION_UPDATE(string HDVENDORBLOCKID, string REQECODE, string REQUESTTYPE, string REQUESTCAT, string BLOCKPURCHASING, string BLOCKPOSTING, string VENDORCODE, string CSVATTACHMENT,
    string DEPTHEAD, string remarks)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORBLOCKUPDATE";
            ocmd.Parameters.Add("VENDORBLOCKID_IN", OracleDbType.Varchar2).Value = HDVENDORBLOCKID;
            ocmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = REQECODE;
            ocmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
            ocmd.Parameters.Add("VENDORCATEGORY_IN", OracleDbType.Varchar2).Value = REQUESTCAT;
            ocmd.Parameters.Add("BLOCKPURCHASING_IN", OracleDbType.Varchar2).Value = BLOCKPURCHASING;
            ocmd.Parameters.Add("BLOCKPOSTING_IN", OracleDbType.Varchar2).Value = BLOCKPOSTING;
            ocmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            ocmd.Parameters.Add("CSVATTACHMENT_IN", OracleDbType.Varchar2).Value = CSVATTACHMENT;
            ocmd.Parameters.Add("DEPTHEAD_IN", OracleDbType.Varchar2).Value = DEPTHEAD;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = remarks;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            odmgt.ExecuteQuery(ocmd);
            string strErr = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return strErr;
            ocmd.Dispose();
        }
        public DataSet VENDORBLOCKREPORT(string ECODE, string ENAME, string OPERATION, string REQTYPE, string VENDORNAME, string VENDORCODE, string ACCGRP, string SITE, string REQDATEFROM, string REQDATETO, string STATUS, string FLAG_IN)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.VENDORBLOCKREPORT_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            oCmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
            oCmd.Parameters.Add("REQTYPE_IN", OracleDbType.Varchar2).Value = REQTYPE;
            oCmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            oCmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            oCmd.Parameters.Add("REQCATE_IN", OracleDbType.Varchar2).Value = ACCGRP;
            oCmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = SITE;
            oCmd.Parameters.Add("REQDATEFROM_IN", OracleDbType.Varchar2).Value = REQDATEFROM;
            oCmd.Parameters.Add("REQDATETO_IN", OracleDbType.Varchar2).Value = REQDATETO;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            oCmd.Parameters.Add("FLAG_IN", OracleDbType.Varchar2).Value = FLAG_IN;
            oCmd.Parameters.Add("CURRPT_IN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public DataSet VENDORVERIFYEPORT(string VENDORNAME, string VENDORCODE)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.VENDORVERIFYREPORT_GET";
            oCmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            oCmd.Parameters.Add("VENDORCODE_IN", OracleDbType.Varchar2).Value = VENDORCODE;
            oCmd.Parameters.Add("CURRPT_IN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }
        public DataTable GetSpecialApprovalAuthority(string strEcode)
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
                    objCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_AD_SPECIAL_APPCODE";
                    objCmd.Parameters.Add("CUR_APPROVARCODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = Convert.ToInt64(strEcode);
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

        public DataTable GetSpecialAppAuthorityDIV(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_SPAPPAUTHORITYDIV_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPAUTHLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = odmgt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetSpecialAppAuthorityOP(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_SPAPPAUTHORITYOP_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPAUTHLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = odmgt.GetDataTable(oCmd);
            return (dt);
        }

        public DataSet GETVENDORAPPROVALGREQFIN(string ecode, string status, string REQUESTTYPE, string VENDORACCGROUP, string VENDORNAME)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_FINVENDORMASTER.SPROC_VENDORAPPROVALFIN_GET";
            oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = REQUESTTYPE;
            oCmd.Parameters.Add("VENDORACCGROUP_IN", OracleDbType.Varchar2).Value = VENDORACCGROUP;
            oCmd.Parameters.Add("VENDORNAME_IN", OracleDbType.Varchar2).Value = VENDORNAME;
            oCmd.Parameters.Add("CUR_APPREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return odmgt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        public DataTable GetSpecialAppAuthorityList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURREQUEST.SPROC_SPAPPAUTHORITYLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("CUR_APPAUTHLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = odmgt.GetDataTable(oCmd);
            return (dt);
        }
    }
    #region Classes
    [Serializable]
    public class VENDORLIST
    {
        string _Ktokk;
        string _Lifnr;
        string _Name1;
        string _Name2;
        string _Name3;
        string _Address1;
        string _Address2;
        string _Address3;
        string _Address4;
        string _City;
        string _Region;
        string _Post_Code1;
        string _Country;
        string _Mobile_No;
        string _Telephone_No;
        string _Email1;
        string _Email2;
        string _Email3;
        string _Msme_Stat_Info;
        string _Msme_Category;
        string _Msme_Certificate;
        string _Dlgrp;
        string _District;
        string _Po_Box;
        string _Post_Code2;
        string _Fax_No;
        string _Bankn;
        string _Banka;
        string _Bnk_Stras;
        string _Bnk_Ort01;
        string _Provz;
        string _Brnch;
        string _Banks;
        string _Koinh;
        string _Bkont;
        string _Bankl;
        string _Bvtyp;
        string _Kalsk;
        string _Waers;
        string _Akont;
        string _Swift;
        string _Bkref;
        string _Xezer;
        string _J_1ipanno;
        string _J_1icstno;
        string _J_1ilstno;
        string _J_1isern;
        string _J_1iexcd;
        string _J_1iexrn;
        string _J_1iexrg;
        string _J_1iexdi;
        string _J_1iexco;
        string _Bukrs;
        string _Zuawa;
        string _Zterm;
        string _Zwels;
        string _Hbkid;
        string _Ekorg;
        string _Webre;
        string _Minbw;
        string _Kzaut;
        string _Lebre;
        string _Xnbwy;
        string _Plifz;
        string _Bstae;
        string _Vensl;
        string _Sort1;
        string _Langu;
        string _STCD3;
        string _VEN_CLASS;
        string _EINV_VALID;
        string _MSME_FROM;
        string _MSME_TO;
        string _MSME_CITY;
        string _LEI_STATUS;
        string _LEI_NO;



        public VENDORLIST()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public string Ktokk
        {
            get { return _Ktokk; }
            set { _Ktokk = value; }
        }
        public string Lifnr
        {
            get { return _Lifnr; }
            set { _Lifnr = value; }
        }
        public string Name1
        {
            get { return _Name1; }
            set { _Name1 = value; }
        }
        public string Name2
        {
            get { return _Name2; }
            set { _Name2 = value; }
        }
        public string Name3
        {
            get { return _Name3; }
            set { _Name3 = value; }
        }
        public string Address1
        {
            get { return _Address1; }
            set { _Address1 = value; }
        }
        public string Address2
        {
            get { return _Address2; }
            set { _Address2 = value; }
        }
        public string Address3
        {
            get { return _Address3; }
            set { _Address3 = value; }
        }
        public string Address4
        {
            get { return _Address4; }
            set { _Address4 = value; }
        }
        public string City
        {
            get { return _City; }
            set { _City = value; }
        }
        public string Region
        {
            get { return _Region; }
            set { _Region = value; }
        }
        public string Post_Code1
        {
            get { return _Post_Code1; }
            set { _Post_Code1 = value; }
        }
        public string Country
        {
            get { return _Country; }
            set { _Country = value; }
        }
        public string Mobile_No
        {
            get { return _Mobile_No; }
            set { _Mobile_No = value; }
        }
        public string Telephone_No
        {
            get { return _Telephone_No; }
            set { _Telephone_No = value; }
        }
        public string Email1
        {
            get { return _Email1; }
            set { _Email1 = value; }
        }
        public string Email2
        {
            get { return _Email2; }
            set { _Email2 = value; }
        }
        public string Email3
        {
            get { return _Email3; }
            set { _Email3 = value; }
        }
        public string Msme_Stat_Info
        {
            get { return _Msme_Stat_Info; }
            set { _Msme_Stat_Info = value; }
        }
        public string Msme_Category
        {
            get { return _Msme_Category; }
            set { _Msme_Category = value; }
        }
        public string Msme_Certificate
        {
            get { return _Msme_Certificate; }
            set { _Msme_Certificate = value; }
        }
        public string Dlgrp
        {
            get { return _Dlgrp; }
            set { _Dlgrp = value; }
        }
        public string District
        {
            get { return _District; }
            set { _District = value; }
        }
        public string Po_Box
        {
            get { return _Po_Box; }
            set { _Po_Box = value; }
        }
        public string Post_Code2
        {
            get { return _Post_Code2; }
            set { _Post_Code2 = value; }
        }
        public string Fax_No
        {
            get { return _Fax_No; }
            set { _Fax_No = value; }
        }
        public string Bankn
        {
            get { return _Bankn; }
            set { _Bankn = value; }
        }
        public string Banka
        {
            get { return _Banka; }
            set { _Banka = value; }
        }
        public string Bnk_Stras
        {
            get { return _Bnk_Stras; }
            set { _Bnk_Stras = value; }
        }
        public string Bnk_Ort01
        {
            get { return _Bnk_Ort01; }
            set { _Bnk_Ort01 = value; }
        }
        public string Provz
        {
            get { return _Provz; }
            set { _Provz = value; }
        }
        public string Brnch
        {
            get { return _Brnch; }
            set { _Brnch = value; }
        }
        public string Banks
        {
            get { return _Banks; }
            set { _Banks = value; }
        }
        public string Koinh
        {
            get { return _Koinh; }
            set { _Koinh = value; }
        }
        public string Bkont
        {
            get { return _Bkont; }
            set { _Bkont = value; }
        }
        public string Bankl
        {
            get { return _Bankl; }
            set { _Bankl = value; }
        }
        public string Bvtyp
        {
            get { return _Bvtyp; }
            set { _Bvtyp = value; }
        }
        public string Kalsk
        {
            get { return _Kalsk; }
            set { _Kalsk = value; }
        }
        public string Waers
        {
            get { return _Waers; }
            set { _Waers = value; }
        }
        public string Akont
        {
            get { return _Akont; }
            set { _Akont = value; }
        }
        public string Swift
        {
            get { return _Swift; }
            set { _Swift = value; }
        }
        public string Bkref
        {
            get { return _Bkref; }
            set { _Bkref = value; }
        }
        public string Xezer
        {
            get { return _Xezer; }
            set { _Xezer = value; }
        }
        public string J_1ipanno
        {
            get { return _J_1ipanno; }
            set { _J_1ipanno = value; }
        }
        public string J_1icstno
        {
            get { return _J_1icstno; }
            set { _J_1icstno = value; }
        }
        public string J_1ilstno
        {
            get { return _J_1ilstno; }
            set { _J_1ilstno = value; }
        }
        public string J_1isern
        {
            get { return _J_1isern; }
            set { _J_1isern = value; }
        }
        public string J_1iexcd
        {
            get { return _J_1iexcd; }
            set { _J_1iexcd = value; }
        }
        public string J_1iexrn
        {
            get { return _J_1iexrn; }
            set { _J_1iexrn = value; }
        }
        public string J_1iexrg
        {
            get { return _J_1iexrg; }
            set { _J_1iexrg = value; }
        }
        public string J_1iexdi
        {
            get { return _J_1iexdi; }
            set { _J_1iexdi = value; }
        }
        public string J_1iexco
        {
            get { return _J_1iexco; }
            set { _J_1iexco = value; }
        }
        public string Bukrs
        {
            get { return _Bukrs; }
            set { _Bukrs = value; }
        }
        public string Zuawa
        {
            get { return _Zuawa; }
            set { _Zuawa = value; }
        }
        public string Zterm
        {
            get { return _Zterm; }
            set { _Zterm = value; }
        }
        public string Zwels
        {
            get { return _Zwels; }
            set { _Zwels = value; }
        }
        public string Hbkid
        {
            get { return _Hbkid; }
            set { _Hbkid = value; }
        }
        public string Ekorg
        {
            get { return _Ekorg; }
            set { _Ekorg = value; }
        }
        public string Webre
        {
            get { return _Webre; }
            set { _Webre = value; }
        }
        public string Minbw
        {
            get { return _Minbw; }
            set { _Minbw = value; }
        }
        public string Kzaut
        {
            get { return _Kzaut; }
            set { _Kzaut = value; }
        }
        public string Lebre
        {
            get { return _Lebre; }
            set { _Lebre = value; }
        }
        public string Xnbwy
        {
            get { return _Xnbwy; }
            set { _Xnbwy = value; }
        }
        public string Plifz
        {
            get { return _Plifz; }
            set { _Plifz = value; }
        }
        public string Bstae
        {
            get { return _Bstae; }
            set { _Bstae = value; }
        }
        public string Vensl
        {
            get { return _Vensl; }
            set { _Vensl = value; }
        }
        public string Sort1
        {
            get { return _Sort1; }
            set { _Sort1 = value; }
        }
        public string Langu
        {
            get { return _Langu; }
            set { _Langu = value; }
        }
        public string STCD3
        {
            get { return _STCD3; }
            set { _STCD3 = value; }
        }
        public string VEN_CLASS
        {
            get { return _VEN_CLASS; }
            set { _VEN_CLASS = value; }
        }
        public string EINV_VALID
        {
            get { return _EINV_VALID; }
            set { _EINV_VALID = value; }
        }

        public string MSME_FROM
        {
            get { return _MSME_FROM; }
            set { _MSME_FROM = value; }
        }
        public string MSME_TO
        {
            get { return _MSME_TO; }
            set { _MSME_TO = value; }
        }
        public string MSME_CITY
        {
            get { return _MSME_CITY; }
            set { _MSME_CITY = value; }
        }
        public string LEI_STATUS
        {
            get { return _LEI_STATUS; }
            set { _LEI_STATUS = value; }
        }
        public string LEI_NO
        {
            get { return _LEI_NO; }
            set { _LEI_NO = value; }
        }

        //CR6782
        string _TypeOfIndustry = string.Empty;
        string _ClassificationYear = string.Empty;
        string _DateOfClassification = string.Empty;

        public string TypeOfIndustry
        {
            get { return _TypeOfIndustry; }
            set { _TypeOfIndustry = value; }
        }
        public string ClassificationYear
        {
            get { return _ClassificationYear; }
            set { _ClassificationYear = value; }
        }
        public string DateOfClassification
        {
            get { return _DateOfClassification; }
            set { _DateOfClassification = value; }

        }
    }
    // Added by TTL Ajit 04-DEC-24
    public class INTERMEDIARYBANK
    {
        public string BANKL { get; set; }

        public string BANKN { get; set; }
        public string BANKA { get; set; }
        public string BNK_STRAS { get; set; }

        public string KOINH { get; set; }
        public string PROVZ { get; set; }
        public string BNK_ORT01 { get; set; }

        public string BANKS { get; set; }
        public string BRNCH { get; set; }

        public string BKONT { get; set; }
        public string SWIFT { get; set; }
        public string IBAN { get; set; }

        public string BKREF { get; set; }

    }
    // Added by TTL Ajit 04-DEC-24
    public class WTHTAX
    {
        public string WITHT { get; set; }
        public string WT_WITHCD { get; set; }
        public string WT_SUBJCT { get; set; }
        public string WT_QSREC { get; set; }
        public string WT_WTSTCD { get; set; }
        public string WT_EXNR { get; set; }
        public string WT_EXRT { get; set; }
        public string WT_EXDF { get; set; }
        public string WT_EXDT { get; set; }
        public string WT_WTEXRS { get; set; }



    }

    #endregion
}