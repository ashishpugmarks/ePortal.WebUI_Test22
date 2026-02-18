using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
    public interface IIOMContractRepos
    {
        string INSERTCONTRACTMASTER(string contractname, string initialdesc, string status, string contractid, string addedby);
        DataSet GETCONTRACTMASTER(string contratdesc, string initialdesc, string status, string CONTRACTID);
        DataSet GET_AGREEMENTMASTER(string AGREEMENTNAME, string INITIALDESC, string AGREEMENTID, string STATUS, string SLA);
        string INSERTAGREEMENTMASTER(string agreementname, string initialdesc, string status, string agreementid, string addedby, string sla);
        DataSet GetIOMFullDetail(string IOMID, string Ecode);
        DataTable GETIOMPENDINGREQUEST(string ecode, string status);
        DataTable GETIOMAPPROVALHISTORY(string ecode);
        DataTable GETPENDINGADMINREQUEST(string REQECODE, string REQENAME, string ECODE, string STATUS, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string IOMID, string VENDNAME);
        DataSet GetNextApprovalAuthority(string Ecode);
        string ADMINIOMAPPROVAL(string HDIOMID, string Empcode, string strRemarks, string status, string agreement, string agreementtype, string UniqueId);

        string UPDATEUSERACKNOWLEDGEMENT(string HDIOMID, string Empcode, string strRemarks);
        string UPDATEFINALDOCUMENT(string HDIOMID, string Empcode, string strRemarks, string finaldoc);
        DataTable GETIOMREPORTDATA(string REQECODE, string REQENAME, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string UNIQUEID, string VENDORNAME, string IOMID);
        DataSet GETCONTRACTLIST(string contracttype, string vendorname, string expdatefrom, string expdateto, string ecode, string agreementid);
        string closecontract(string agreementid, string closeremarks, string ecode);

        string CONTRACTRENEWAL(string fromiom, string agreementtype, string Contracttype, string EffectiveDate, string Termmonth, string Termyear, string Dateofexpiry, string Mannerofpayment, string Purpose, string Remarks, string addedby, string finaldoc, string VendorName, string agreementid);
        DataTable GET_MANAGEIOMDETAILS(string ecode);
        // DataTable GetDashBoardCount(string strvpid, string strdivid, string strdptid, string strsecid);
        DataSet GetContractount_contracttype(string strctype, string stroperation, string strdivision, string strdeprtment);
        DataSet GetContrac_Detail(string strctype, string strvpid, string vendorname);
        DataSet GETIOMBYAGREEMENTID(string AGREEMENTID);
        DataSet GETLIVECONTRACTREPORT(string OPERATIONID);
        string INSERTSTATUSMASTER(string statusName, string status, string statusid, string addedby);
        DataSet GETSTATUSMASTER(string STATUSID, string STATUSNAME, string STATUS);
        DataTable GETCONTRACTREPORTDATA(string CONTRACT, string OPERATION, string STATUS, string VENDORNAME, string DIVISION, string DEPARTMENT, string UNIQUEID, string IOMID);
        DataTable GETBACKDATEREPORTDATA(string REQECODE, string REQENAME, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string UNIQUEID, string VENDORNAME, string IOMID);
        DataSet ViewAgreementDetail(string AGREEMENTID);
        string INSERT_IOMDEATIL_ADMIN(string From, string AgrementType, string ContractType, string EffectiveDate, string Termmonth, string Termyear, string Dateofexpiry, string Mannerofpayment, string Purpose, string Remarks, string ADDEDBY, string VendorName, string finaldocument, string divisionid, string departmentid);
        string Insert_IOMDetail(string From, string AgrementType, string ContractType, string EffectiveDate, string Termmonth,
                string Termyear, string Dateofexpiry, string Mannerofpayment, string Purpose, string Remarks, string ADDEDBY,
                string agreementfile, string referencefile, string VendorName, string agreementid, string antibribery, string considerable,
                string authlevel, string mstagr, string depid, string divid, string OPHEADECODE, string CONTRACTECODE, string SURETYAMOUNT,
                string NAMEOFSURETY, string RETURNFROMDATE, string RETURNTODATE, int IT_DeclareValue); // IT_DeclareValue  Added by Aumento :: SR79956

        string Update_IOMDetail(string HDIOMID, string Empcode, string strRemarks, string status, string backdateremark, string authlevel, string OPHEADECODE);
        string UPDATEIOMDETAIL(string fromiom, string agreementtype, string Contracttype, string EffectiveDate,
            string Termmonth, string Termyear, string Dateofexpiry, string Mannerofpayment, string Purpose, string Remarks,
            string addedby, string agreementfile, string approvalnotefile, string VendorName, string IOMID, string antibribery,
            string ndadocument, string authlevel, string mstagr, string depid, string divid, string OPHEADECODE, int IT_DeclareValue); // IT_DeclareValue  Added by Aumento :: SR79956

        DataSet GetUserWithLegalApprovalRights();
        DataTable GETADMINCONTRACTSTATUS(string REQECODE, string REQENAME, string ECODE, string STATUS, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string UNIQUEID, string IOMID, string VENDNAME/* ,STATUSID*/);
        string SUBMITCOMMUNICATION(string HDIOMID, string Empcode, string strRemarks, string refDoc, string UserType, int COMMUNICATIONTYPE);
        DataTable GETAPPROVEDADMINREQUEST(string REQECODE, string REQENAME, string ECODE, string STATUS, string CONTRACT, string OPERATION, string DIVISION, string DEPARTMENT, string IOMID, string VENDNAME, string UNIQUEID);
        string SUBMITUPDATEOPERATION(string HDIOMID, string Empcode, string operationType);
        DataSet GETIOMLIST_FOROPERATION(string contracttype, string vendorname, string expdatefrom, string expdateto, string OPERATION, string agreementid);

    }
}
