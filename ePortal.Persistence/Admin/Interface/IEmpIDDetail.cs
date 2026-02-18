using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IEmpIDDetail
    {
        DataTable Empuanexport(string strecode, string strename);
        string UpdateEmpUAN(string strEmpcode, string strempuan, string stremppfno, string strAddedby);
        string UpdateEmpIDDetail(string strEmpcode, string strempuan, string straadhar, string strpassport, string strpassportexpdate, string strdl, string strdlexpdate, string strpfno, string strrationcard, string strelectioncard, string strnprno, string strAddedby, string stractive);
        string UpdateEmpAadhar(string strEmpcode, string strempaadhar, string strAddedby);
        string UpdateEmpDLno(string strEmpcode, string strempdlno, DateTime strempdlexpdate, string strAddedby);
        string UpdateEmpPassportno(string strEmpcode, string stremppassportno, DateTime stremppassportexpdate, string strAddedby);
        string UpdateEmpRation(string strEmpcode, string strempration, string strAddedby);
        string UpdateEmpelection(string strEmpcode, string strempelection, string strAddedby);
        string UpdateEmpnpr(string strEmpcode, string strempnpr, string strAddedby);
        string InsertEmpUnblockDetail(string strAdempcode, DateTime strAddedDate, string strAddedby, string strflag, string Reason);
        DataTable GetUNB_Emp_DTL(string strecode);
        string InsertEmpblockDetail(string strAdempcode, DateTime strAddedDate, string strAddedby, string strflag, string Reason);
        DataTable GetB_Emp_DTL(string strecode);
        DataTable GetEmpBlockUnblockReport(string strType, string strEmployeeCode, string FromDate, string ToDate, string strStatus, string strUserID);
        DataTable GetB_Emp_Name(string StrTerm);
        DataTable GetLogin_Emp_Name(string StrTerm);
    }
}
