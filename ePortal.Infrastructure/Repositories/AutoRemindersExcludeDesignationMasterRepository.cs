using ePortal.Infrastructure.DbContexts;
using ePortal.Persistence.Interface;
using ePortal.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ePortal.Infrastructure.Repositories
{
    public class AutoRemindersExcludeDesignationMasterRepository
    {
        private readonly EPortalDBContext _arDBContext;
        private readonly IDataManagement oDataMgmt;
        private readonly ICommonFunctions objcmn;

        public AutoRemindersExcludeDesignationMasterRepository(EPortalDBContext objEPortalDBContext, IDataManagement _oDataMgmt, ICommonFunctions _objcmn)
        {
            _arDBContext = objEPortalDBContext;
            oDataMgmt = _oDataMgmt;
            objcmn = _objcmn;
        }

        public List<AutoRemindersExcludeDesignationMaster> GetAutoRemindersExcludeDesignationMasterList(string UserId)
        {
            try
            {
                DataTable dt = new DataTable();

                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_GET_AUTO_REMINDER_EXCLUDE_DESIGNATION_LIST";
                oCmd.Parameters.Add("CREATED_BY_", OracleDbType.Varchar2).Value = UserId;
                oCmd.Parameters.Add("TBL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                dt = oDataMgmt.GetDataTable(oCmd);

                var list = new List<AutoRemindersExcludeDesignationMaster>();

                foreach (DataRow row in dt.Rows)
                {
                    var data = new AutoRemindersExcludeDesignationMaster
                    {
                        SRNO = Convert.ToInt32(row["SRNO"]),
                        DESIGNATION_ID = Convert.ToInt32(row["DESIGNATION_ID"]),
                        DESIGNATION_DESCRIPTION = row["DESIGNATION_DESCRIPTION"].ToString(),
                        IS_EXCLUDE = row["IS_EXCLUDE"].ToString()
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

        public bool UpdateData(int Id,  string UserID)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATE_AUTO_REMINDER_EXCLUDE_DESIGNATION";
                oCmd.Parameters.Add("ID_", OracleDbType.Int32).Value = Convert.ToInt32(Id);
                oCmd.Parameters.Add("UPDATED_BY_", OracleDbType.Varchar2).Value = UserID;
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
