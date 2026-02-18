using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IPassword
    {
        string updatepassword(string struserid, string strpassword, string strUserType);
        DataSet GetEmail(string strEmpCode, string strUserType);        
        DataTable GetContractualDetailsNew(string intEmpCode);
        string PASSWORDRESETMASTER(string strempcode, string strpassword);
    }
}
