using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IUARService
    {

        List<UAR_AccessReviewViewModel> GetUARList(int loginCode, string Status, string Ename, string Ecode);
        //List<UAR_ViewModel> UARRequestList(string strEmpcode);
        List<UAR_ViewModel> UARRequestList(string strEmpcode, string LoginCode);
        List<UAR_AccessReviewViewModel> GetEmpNameList(long strEmpcode);
        string ReviewListData(string EmpCode, string strActivityID, string strBy, string ACCESSTYPE);// ACCESSTYPE added for SR78412
        string DeActiveUser(string EmpCode, string modifiedBy);
        string UpdateADUARRequest(long AdEmpCode);
        bool GetEmailandName(long Ecode, out string Email, out string Ename);
        string GetSYKIID();
        //SR78412 Changes Start
        List<UAR_Regular_ViewModel> UARRegularRequestList(string strEmpcode, string REVIEWER);
        string ReviewRegularListData(string EmpCode, string strActivityID, string strBy, string ACCESSTYPE, int UserChecked); //SR102715
        string UpdateADUARRegularRequest(long AdEmpCode);
        List<UAR_Regular_ViewModel> UARRegularReviewerRequestList(string strEmpcode, string REVIEWER);
        string ReviewRegularReviewerListData(string EmpCode, string strActivityID, string strBy, string ACCESSTYPE);// ACCESSTYPE added for SR78412
        string SendBackADUARRequest(long AdEmpCode, long REVIEWER);
        List<UAR_AccessReviewViewModel> GetUARStatusList(int loginCode, string Status, string Ename, string Ecode, string SYKIID);
        List<SYKI> GetKICodeList(long userId);
        //SR78412 Changes End
        //SR92683 Changes Start
        string GetSelfReviewRemarks(long AdEmpCode, long REVIEWER);
        string UpdateSelfReviewRemarks(long AdEmpCode, String SelfRemarks);
        string GetReviewerRemarks(long AdEmpCode, long REVIEWER);
        string UpdateReviewerRemarks(long AdEmpCode, String Remarks);
        List<UAR_Regular_ViewModel> UARRegularRequestHistoryList(String UARID);
        //SR92683 Changes End
    }
}
