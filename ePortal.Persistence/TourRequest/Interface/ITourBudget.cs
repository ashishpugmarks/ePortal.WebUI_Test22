using System.Data;

namespace ePortal.Persistence.TourRequest.Interface
{
    public interface ITourBudget
    {
        public DataSet Gettourbudget(string stradtourbudget, string strtourbudgetki, string stroperationid, string strstatus, string strtravelid);
        public DataSet GettourbudgetHis(string stradtourbudgethis, string strtourbudgetki, string stroperationid, string strtravelid, string strtraveltype, string strstatus);

        public string INSERTTRAVELBUDGETHIS(string strtravelbudgetid, string strki, string stroperation, string strtravelid, string strtraveltype, string stramount, string status, string addedby);

        public string INSERTTICKETBUDGET(string strreqid, string stramount, string status, string addedby);

        public string INSERTFINANCETBUDGET(string strreqid, string stramount, string status, string addedby);

        public string INSERTSETTLMENTBUDGET(string strreqid, string stramount, string status, string addedby);

        public DataSet GETTOUREXPENSEREPORT(string strtourbudgetki, string stroperationid, string strstatus);

        public DataSet GETORGMAPWithOPR(string ORGOpMapId, string OperationId, string Ki, string menutypeid, string orgid, string status);

        public string INSERTORGMApOpER(string OperationiD, string Plantki, string OrgId, string menutypeId, string status, string addedby, string touropmappid);

        public DataSet GETORGMAPWithOPRAsset(string ORGOpMapId, string OperationId, string Ki, string menutypeid, string orgid, string status);

        public string INSERTORGMApOpERAsset(string OperationiD, string Plantki, string OrgId, string menutypeId, string status, string addedby, string touropmappid);

        public DataTable GetADORGLEVELTYPE();
        public DataTable GetOPBUDGET(string strecode);
        public DataSet GETTOURBUDGETREPORT(string strtourbudgetki, string stroperationid, string strstatus);
    }
}
