using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface IEmpLoginService
    {
        Employee_Details GetEmpDetails(string Userid);
        string Check_MailLogin(string strUserID);
        string Check_AutoLogin(string strUserID, string strToken, string Usertype);
        int Check_EmpLogin(string strUserID, string strPWD,string ClientIP, string Usertype);
        int Check_Switch_EmpLogin(string strUserID, string ClientIP, string Usertype);
        String GetParameterValue(string strParmaName);
        List<ContentViewModel> GetContent(ContentViewModel objSearch);

        ContentViewModel GetVideo(ContentViewModel objsearch);
        ContentViewModel GetAttachement2(ContentViewModel objsearch);

        List<ContentViewModel> GetPopupContent(ContentViewModel objsearch);

        string UpdateEmployeePassword(long uid, string strPassword, long modby, string Usertype);
        string UpdateEmployeeMobileno(long uid, string strmobileno, long modby);
        string MailApprovalInsert(string strwid, string strcontroler, string straction, string strTid);
        DataSet MailApprovalGet(long uid);

        string GenerateJwtToken(string userName);


        // Start-  Added By TTL against CR6695 as on 29-07-2025 
        bool WriteCookie(string cookiesName, CookieDetails _userContext, int expireInDays);
        bool DeleteCookie(string cookiesName);
        // End-  Added By TTL against CR6695 as on 29-07-2025 
    }

}
