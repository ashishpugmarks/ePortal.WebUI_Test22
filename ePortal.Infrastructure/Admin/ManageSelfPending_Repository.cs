using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Infrastructure.DbContexts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Infrastructure.Admin
{
    public class ManageSelfPending_Repository
    {
        private EPortalDBContext _empLoginDBContext;

        public ManageSelfPending_Repository(EPortalDBContext empLoginDBContext)
        {
            _empLoginDBContext = empLoginDBContext;
        }       

        public long GetTotalSelfPending(Int32 strEmpCode)
        {            
            int resultout2 = 0;

            try
            {
                var sql = "BEGIN SPROC_PENDINGSELFCOUNT(:ECODE_IN,:SYKI_IN,:NOOFCOUNT); END;";
                OracleParameter lOGINEMPCODE_OUT = new OracleParameter("NOOFCOUNT", OracleDbType.Int32) { Direction = ParameterDirection.Output };
                var parameters = new OracleParameter[]
                {

                    new OracleParameter("ECODE_IN", OracleDbType.Int32) { Value = strEmpCode },
                    new OracleParameter("SYKI_IN", OracleDbType.Varchar2) { Value = null },
                    lOGINEMPCODE_OUT
                };
                //_empLoginDBContext.Database.ExecuteSqlRawAsync(sql, parameters);
                _empLoginDBContext.Database.ExecuteSqlRaw(sql, parameters);
                resultout2 = Convert.ToInt16(lOGINEMPCODE_OUT.Value);
            }
            catch (Exception ex)
            {

                //throw;
            }

            return resultout2;
        }
    }
}



