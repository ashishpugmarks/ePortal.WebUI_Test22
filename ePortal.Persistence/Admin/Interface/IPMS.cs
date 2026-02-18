using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IPMS
    {
        DataTable GetFilterDesignation();
        DataTable GetOperaton();
        DataTable GetSect(string strOperationId, string strDivisionId, string strDepartmentId);
        DataTable GetSect(string strOperationId, string strDivisionId, string strDepartmentId, string strKI);
        public DataTable GetKiList();
        public string GetKIId();
    }
}
