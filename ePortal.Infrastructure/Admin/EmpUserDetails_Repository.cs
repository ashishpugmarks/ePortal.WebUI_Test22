using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Infrastructure.Admin
{
    public class EmpUserDetails_Repository
    {
        private EPortalDBContext _empLoginDBContext;

        public EmpUserDetails_Repository(EPortalDBContext empLoginDBContext)
        {
            _empLoginDBContext = empLoginDBContext;
        }

        //ADEMP_FAMILYDeclaration
        //public DataTable GetEmployeeDeclaration(string strEmpcode)
        //{
        //    oCmd = new OracleCommand();
        //    DataTable ds = new DataTable();
        //    oCmd.CommandText = "PKG_USERDETAIL.SPROC_GET_EMPFAMILYDECLARATION";
        //    oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = strEmpcode;
        //    oCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //    oCmd.BindByName = true;
        //    ds = oDataMgmt.GetDataTable(oCmd);
        //    return (ds);
        //}


        public Int32 DeleteFamilyDeclaration(string strEmpcode, string headerid)
        {
            int resultout2 = 0;

            //try
            //{
            var sql = "BEGIN PKG_USERDETAIL.SPROC_EMPFAMILYDEC_DELETE(:EMPCODE_IN,:HDRID_IN,:ERRMSG_OUT, :RESULT_OUT); END;";
            OracleParameter ERRMSG_OUT_OUT = new OracleParameter("ERRMSG_OUT", OracleDbType.Varchar2) { Direction = ParameterDirection.Output };
            OracleParameter RESULT_OUT_OUT = new OracleParameter("RESULT_OUT", OracleDbType.Int32) { Direction = ParameterDirection.Output };
            var parameters = new OracleParameter[]
            {

                    new OracleParameter("EMPCODE_IN", OracleDbType.Int32) { Value = strEmpcode },
                    new OracleParameter("HDRID_IN", OracleDbType.Int32) { Value = headerid },
                    ERRMSG_OUT_OUT,
                    RESULT_OUT_OUT
            };
            _empLoginDBContext.Database.ExecuteSqlRawAsync(sql, parameters);
            resultout2 = Convert.ToInt16(RESULT_OUT_OUT.Value);

            return resultout2;
        }
    }
}

