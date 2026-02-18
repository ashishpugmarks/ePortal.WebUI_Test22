using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Interface
{
    public interface IDataManagement
    {
        DataSet GetDataSet(string strSql);
        DataSet GetDataSet(OracleCommand objCmd);
        DataTable GetDataTable(string strSql);
        DataTable GetDataTable(OracleCommand objCmd);
        DataRow[] GetDataRow(string strSql);
        DataRow[] GetDataRow(OracleCommand objCmd);
        void ExecuteQuery(string strSql);
        void ExecuteQuery(OracleCommand objCmd);
    }
}
