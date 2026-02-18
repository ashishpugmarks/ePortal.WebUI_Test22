using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface ILogin
    {
        int getEmpLogin(string strUserID, string strPWD, string strUserType, string strclintip);
        string getEmpName(string strUserID, string strUserType);
        int getServiceLogin(string strUserID);
        int Check_EmpLogin(string strUserID, string strPWD, string strProject);
        int check_FirstLogin(string strUserID, string strPWD, string strUserType);
        void update_FirstLogin(string strUserID, int strUserType);
        int check_LoginBefore90Days(string strUserID, string strUserType);
        int check_LoginExpiry(string strUserID, out string strExpiryDayLeft, out int IsEligibleForPolicy);
        int getPassExpiryDays(string strUserID);
    }
}
