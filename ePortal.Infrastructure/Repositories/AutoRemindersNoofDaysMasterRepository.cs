using ePortal.Infrastructure.DbContexts;
using ePortal.Persistence.Interface;
using ePortal.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ePortal.Infrastructure.Repositories
{
    public class AutoRemindersNoofDaysMasterRepository
    {
        private readonly EPortalDBContext _arDBContext;
        private readonly IDataManagement oDataMgmt;
        private readonly ICommonFunctions objcmn;

        public AutoRemindersNoofDaysMasterRepository(EPortalDBContext objEPortalDBContext, IDataManagement _oDataMgmt, ICommonFunctions _objcmn)
        {
            _arDBContext = objEPortalDBContext;
            oDataMgmt = _oDataMgmt;
            objcmn = _objcmn;
        }
        public List<AutoRemindersNoofDaysMaster> GetAutoRemindersNoofDaysMasterList()
        {
            try
            {
                DataTable dt = new DataTable();

                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_GET_AUTO_REMINDERS_NO_OF_DAYS_MASTER_LIST";
                oCmd.Parameters.Add("TBL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                dt = oDataMgmt.GetDataTable(oCmd);

                var list = new List<AutoRemindersNoofDaysMaster>();

                foreach (DataRow row in dt.Rows)
                {
                    var data = new AutoRemindersNoofDaysMaster
                    {
                        SRNO = Convert.ToInt32(row["SRNO"]),
                        REMINDER_TYPE = row["REMINDER_TYPE"].ToString(),
                        NO_OF_DAYS = Convert.ToInt32(row["NO_OF_DAYS"])
                    };

                    list.Add(data);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public AutoRemindersNoofDaysMaster GetDataBySrNo(int? Id)
        {
            try
            {
                DataTable dt = new DataTable();

                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_GET_AUTO_REMINDERS_NO_OF_DAYS_MASTER_BY_SRNO";
                oCmd.Parameters.Add("SRNO_", OracleDbType.Int32).Value = Id;
                oCmd.Parameters.Add("TBL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                dt = oDataMgmt.GetDataTable(oCmd);

                AutoRemindersNoofDaysMaster data = new AutoRemindersNoofDaysMaster();
                data.SRNO = Convert.ToInt32(dt.Rows[0]["SRNO"]);
                data.REMINDER_TYPE = dt.Rows[0]["REMINDER_TYPE"].ToString().Replace("_", " ");
                data.NO_OF_DAYS = Convert.ToInt32(dt.Rows[0]["NO_OF_DAYS"]);
                return data;
            }
            catch (Exception ex)
            {   
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateData(int SrNo,int NoofDays, string UserID)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATE_AUTO_REMINDERS_NO_OF_DAYS_MASTER";
                oCmd.Parameters.Add("SRNO_", OracleDbType.Int32).Value = Convert.ToInt32(SrNo);
                oCmd.Parameters.Add("NO_OF_DAYS_", OracleDbType.Int32).Value = Convert.ToInt32(NoofDays);
                oCmd.Parameters.Add("UPDATED_BY_", OracleDbType.Varchar2).Value = UserID;
                oCmd.Parameters.Add("UPDATED_DATE_", OracleDbType.Date).Value = DateTime.Now;
                oDataMgmt.ExecuteQuery(oCmd);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
