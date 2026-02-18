using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IShiftchg
    {
        DataSet GetShiftDetails(string struserCode);
        public DataSet GetShiftDetails_NEW(string struserCode, string strreqdate);
        public DataSet GetDetails(string strCode);
        public string AddShiftChangerequest(string strUser, string strCurrshft, string strNewshft, string strDateFrom, string strDateTo, string strReason, string strRemark, string strAppCode);
        public int UpdateShiftChangerequest(string strUser, string strCurrshft, string strNewshft, string strDateFrom, string strDateTo, string strReason, string strRemark, string strAppCode, string strUserCode);
        public DataSet GetAppCode(string strAppCode, string struserCode);
        public DataSet EditShiftChangeRequest(string strCode);
        public DataSet ManageShiftChange(string strEmpCode);
        public DataSet ManageShiftChange1(string strEmpCode);
        public DataSet ShiftChange(string strCode, string strAppFromDate, string strAppToDate, string strKI);
        public DataTable GetKi();
        public int UpdateShiftApproval(string strRequestid, string strApprovalStatus, string strRemarks);
        public DataSet GetShiftReqDetails(string strSupEmpCode);
        public DataSet ShiftChangeAppHistory(string strEmpCode);
        public DataTable GetData(string Shift, string CurrShift);
    }
}
