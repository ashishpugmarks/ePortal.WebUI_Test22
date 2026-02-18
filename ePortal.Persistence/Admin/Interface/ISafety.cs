using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface ISafety
    {
        #region"GET DATA"
        public DataTable Get_SFDepartment(string strplantid);
        public DataTable Get_Category(string strcategoryid, string strcategory, string strstatus, string strplantid);
        public DataTable Get_Act(string stractid, string stract, string strstatus, string strplantid);
        public DataTable GetFrequency(string strfrq);
        public DataTable GetContract(string strcontractid, string strcategoryid, string stractid, string strstatus, string strsecid,
         string strdptid, string strdivid, string strvpid, string strfraomdate, string strtodate, string strsfsection, string strplantid);
        public DataTable GetReminderPart(string strcontractid, string strstatus, string strflag, string strecode);
        public DataTable GetPendingApproval(string strcontractid, string strstatus, string strflag, string strecode);
        public DataTable GetReminderdDetail(string strcontractid);
        public DataTable GetReminderdMailID(string strreminderid);
        public DataTable GetDashBoardCount(string strsfsection, string strplantid, string strsecid, string strdptid, string strdivid, string strvpid);
        public DataTable GetDashBoardDetail(string strsfsection, string strplantid, string strsecid, string strdptid, string strdivid, string strvpid, string strstatus);
        #endregion

        #region"INSERT AND UPDATE"
        public string InsertCategory(string strcategoryid, string strdes, string strinitial, string straddby, string stractive, string strplantid, string strAppAuth);
        public string InsertAct(string stractid, string strdes, string strinitial, string straddby, string stractive, string strplantid);
        public int AddFrequency(string strfrqid, string strfrq, string status, int AddedBy, string strmonth);
        public string Insertcontracts(string strcontractid, string strcateegory, string stract, string strreq, string strfrq, string strexpdate, string strduedate, string strcomp, string strremark, string straddby, string stractive,
        string strattach, string strsectionid, string strrescode, string str1st, string str2nd, string str3rd, string str4th);
        public int AddReminderdetail(string strremid, string strremark, string status, string AddedBy, string strattach);
        public int UpdateReminderApproval(string strremid, string strremark, string status, string AddedBy);
        #endregion
        public DataTable GETSAFETYSECTIONMST(string sfsection, string sfsectiondesc, string plant, string status, string SFSECTIONID);
        public string INSERTSECTIONMASTER(string section, string sfsectiondesc, string appauthecode, string status, string sectiontnsid, string addedby, string plant);
    }
}
