using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IVicePresident
    {
        int AddVP(string strVpDesc, string strVpIniDesc, string strEmpCode, string strStatus, string strCreatedBy, string strID);
        DataTable GetAllVP();
        string getConnectingString();
        DataTable getDetails(string strAdVPID);
    }
}
