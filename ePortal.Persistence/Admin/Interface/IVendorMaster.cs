using System.Data;
using ePortal.Persistence.Admin.Services;
using ePortal.ViewModels.APPX.VendorMaster;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IVendorMaster
    {
        string CONFIRMDUPLICATE_AUTHORITY(string approvalauthorityid, string site);
        DataSet GETAPPROVALAUTHORITY(string approvalauthorityid, string site, string conapprovalauthority, string status);
        string GetCurrentPage(Uri uri);
        DataSet GETKIOPERDIVDEPTSEC(string OPERATION, string DIVISION, string DEPARTMENT);
        DataSet GETSITE();
        DataTable GetSpecialAppAuthorityDIV(string EmpCode);
        DataTable GetSpecialAppAuthorityOP(string EmpCode);
        DataTable GetSpecialApprovalAuthority(string strEcode);
        DataSet GETVENDORAPPROVALGREQFIN(string ecode, string status, string REQUESTTYPE, string VENDORACCGROUP, string VENDORNAME);
        DataSet GETVENDORAPPROVALGREQUEST(string ecode, string status, string REQUESTTYPE, string VENDORACCGROUP, string VENDORNAME);
        DataSet GETVENDORBLOCAPPROVALGREQUEST(string ecode, string status, string REQUESTTYPE, string REQUESTCATE, string VENDORCODE);
        DataSet GETVENDORBLOCKPENDINGREQUEST(string ecode, string status, string VENDORHEADERID, string REQUESTTYPE, string REQUESTCATE, string VENDORCODE, string REQDATEFROM, string REQDATETO);
        DataSet GETVENDORLISTBLOCKDTL(string VENDORCODE);
        DataSet GETVENDORLISTIFEXISTS(string VENDORNAME1, string VENDORADDRESS, string VENDORCITY, string VENDORACNO, string VENDORPANNO);
        DataSet GETVENDORLISTMATCHDTL(string VENDORNAME1, string VENDORADDRESS, string VENDORCITY, string VENDORACNO, string VENDORPANNO, string STRADDRESS1, string STRADDRESS2, string STRADDRESS3);
        DataSet GETVENDORLISTMATCHDTLA(string VENDORNAME1, string VENDORADDRESS, string VENDORCITY, string VENDORACNO, string VENDORPANNO, string STRADDRESS1, string STRADDRESS2, string STRADDRESS3);
        DataSet GETVENDORPENDINGREQUEST(string ecode, string status, string VENDORHEADERID, string REQUESTTYPE, string VENDORACCGROUP, string VENDORNAME, string REQDATEFROM, string REQDATETO);
        DataSet GET_DEPARTMENTHEAD(string ECODE);
        DataSet GET_MANAGEVENDORMASTERLIST(string VENDORCODE, string VENDORNAME, string BANKACCNO, string PANNUMBER, string STATUS);
        DataSet GET_VENDORFINANCEBLOCKAPPROVALLIST(string SITE, string ECODE, string ENAME, string STATUS, string FINECODE, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION);
        DataSet GET_VENDORFINANCEDEPTAPPROVALLIST(string SITE, string ECODE, string ENAME, string STATUS, string FINECODE, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION);
        DataSet GET_VENDORMASTER_DETAILS(string VENDORHEADERID);
        DataSet GET_VENDORSISDEPTREQUESTEDLIST(string SITE, string ECODE, string ENAME, string STATUS, string SISECODE, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION);
        DataSet GET_WHOLDINGTAXMASTER(string ECODE, string ERR_MSG);
        string GET_WHOLDINGTAXSAVE(int ECODE_IN, string HEADERID_IN, string VWithholding_Tax_Type, string VWithholding_Tax_Code, string VLiable, string VRecipient_type, string VWithholding_tax_id_no, string VExemption_certi_no, string VExemption_rate, string VDate_On_Which_Exemption_Begins, string VDate_On_Which_Exemption_Ends, string VReason_For_Exemption, int status);
        string INSERTAPPROVALAUTHORITY(string approvalauthorityid, string site, string conapprovalauthority, string addedby, string status);
        string MANAGEVENDORMASTERINSERT(string VENDORACCGRP, string VENDORCODE, string VENDORNAME1, string VENDORNAME2, string VENDORNAME3, string STREET1, string STREET2, string STREET3, string STREET4, string CITY, string REGION, string POSTALCODE, string COUNTRY, string MOBILENO, string TELEPHONENO, string EMAIL1, string EMAIL2, string EMAIL3, string MSMEINFOSTATUS, string MSMECATEGORY, string MSMECERTIFICATION, string SERVICEAGENTGRP, string BANKCOUNTRY, string BANKNAME, string BRANCHNAME, string BANKADDRESS, string TYPEOFACCOUNT, string BANKCITY, string BANKSTATE, string BANKACCNO, string IFSCCODE, string BANKCATEGORY, string SCHEMAGROUP, string ORDERCURRENCY, string PANNUMBER, string CSTREGNUMBER, string LSTNUMBER, string SERVICEREGNUMBER, string ECCNUMBER, string EXCISEREGNO, string EXCISERANGE, string EXCISEDIVISION, string COMMISTIONERATE, string ADDEDBY, string GSTIN, string GSTCLASSIFICATION);
        string SISVENDORREQUESTAPPROVAL(string VENDORHEADERID, string STRREMARKS, string STATUS, string APPECODE, string REQTYPE, VENDORLIST objvmlist, INTERMEDIARYBANK interbank, WTHTAX[] withtaxs);
        string UpdateVendorblockSAP(string vendorcode, string strpurch, string strposting);
        DataSet VENDORBLOCKREPORT(string ECODE, string ENAME, string OPERATION, string REQTYPE, string VENDORNAME, string VENDORCODE, string ACCGRP, string SITE, string REQDATEFROM, string REQDATETO, string STATUS, string FLAG_IN);
        string VENDORBLOCKREQAPPROVAL(string VENDORHEADERID, string STRREMARKS, string STATUS, string APPECODE, string APPLEVEL);
        DataSet VENDORBLOCKREQUESTBYID(string VenderHeaderID);
        DataSet VENDORBLOCKREQUESTSAP(string VenderHeaderID);
        string VENDORBLOCK_INFORMATION_INSERT(string REQECODE, string REQUESTTYPE, string REQUESTCAT, string BLOCKPURCHASING, string BLOCKPOSTING, string VENDORCODE, string CSVATTACHMENT, string DEPTHEAD, string remarks);
        string VENDORBLOCK_INFORMATION_UPDATE(string HDVENDORBLOCKID, string REQECODE, string REQUESTTYPE, string REQUESTCAT, string BLOCKPURCHASING, string BLOCKPOSTING, string VENDORCODE, string CSVATTACHMENT, string DEPTHEAD, string remarks);
        string VENDORDETAILVALIDATE(string REQUESTTYPE, string VENDORACCGRP, string VENDORCODE, string VENDORNAME1, string VENDORNAME2, string VENDORNAME3, string STREET1, string STREET2, string STREET3, string STREET4, string CITY, string REGION, string POSTALCODE, string COUNTRY, string MOBILENO, string TELEPHONENO, string EMAIL1, string EMAIL2, string EMAIL3, string MSMEINFOSTATUS, string MSMECATEGORY, string MSMECERTIFICATION, string SERVICEAGENTGRP, string BANKCOUNTRY, string BANKNAME, string BRANCHNAME, string BANKADDRESS, string TYPEOFACCOUNT, string BANKCITY, string BANKSTATE, string BANKACCNO, string IFSCCODE, string BANKCATEGORY, string SCHEMAGROUP, string ORDERCURRENCY, string PANNUMBER, string CSTREGNUMBER, string LSTNUMBER, string SERVICEREGNUMBER, string ECCNUMBER, string EXCISEREGNO, string EXCISERANGE, string EXCISEDIVISION, string COMMISTIONERATE, string GSTIN, string GSTCLASSIFICATION, string E_INVOICEApplicable, string MSMEFROM, string MSMETO, string MSMECITY, string LEIAPPLICABLE, string LEINO, string SWIFTCODE_IN, string IBANNO_IN, string IBANKACCNO_IN, string IBRANCHNAME_IN, string IBANKADDRESS_IN, string IBANKCITY_IN, string IBANKSTATE_IN, string BANKCOUNTRY_IN, string ITYPEOFACCOUNT_IN, string IBANKCATEGORY_IN, string ISWIFTCODE_IN, string IIBANNO_IN, string IBANK_KEY, string TYPE_OF_INDUSTRY, string CLASSIFICATION_OF_YEAR, string DATE_OF_CLASSIFICATION);
        DataSet VENDORMASTERREPORT(string ECODE, string ENAME, string OPERATION, string REQTYPE, string VENDORNAME, string VENDORCODE, string ACCGRP, string SITE, string REQDATEFROM, string REQDATETO, string STATUS, string FLAG_IN);
        string VENDORMASTER_INFORMATION_INSERT(VendorMasterDetails vdm);
        string VENDORMASTER_INFORMATION_UPDATE(VendorMasterDetails vmd);
        string VENDORREQUESTAPPROVAL(string VENDORHEADERID, string STRREMARKS, string STATUS, string APPECODE, string APPLEVEL, string STRATTACHMENT, string SPLAPPCODE, string SPLAPPSTATUS);
        DataSet VENDORREQUESTBYID(string VenderHeaderID);
        DataSet VENDORREQUESTBYVEDNORCODE(string VENDORCODE);
        DataSet VENDORVERIFYEPORT(string VENDORNAME, string VENDORCODE);
        DataTable GetSpecialAppAuthorityList(string EmpCode);
    }
}