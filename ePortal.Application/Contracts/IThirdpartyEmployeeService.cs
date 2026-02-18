
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IThirdpartyEmployeeService
    {
        List<ThirdpartyEmployeeViewModel> GetThirdpartyEmployeeList(int LoginCode);

        #region  AutoSuggestions

        dynamic GetCompanyName(string term);
        dynamic GetprojectmanagerName(string companyname, string projectmanager);
        dynamic GetAccountmanagerName(string companyname, string Accountmanager);
        dynamic Getprojectmanagermailid(string projectManagerName, string term);
        dynamic GetAccountManagermailid(string AccountManagerName, string term);
        object GetEcode();
        LibResult GetOwnership(string term, int LoginCode);

        List<SelectOwnShipData> GetOwnershipList(int LoginCode);
        #endregion

        short AddThirdpartyEmployeeData(ThirdpartyEmployeeViewModel model_, string userID);

        // Added by TTL SR94104 - CR6022
        List<ThirdpartyEmpUploadModel> BulkUploadEmployeeDetails(List<ThirdpartyEmpUploadModel> employeeDetails, string UserId);
        long GetExistingEmpCode(string associateCompanyID, string companyName);
        string GetValidEmployee(string eCode);
        // End by TTL SR94104 - CR6022

        short EditThirdpartyempData(ThirdpartyEmployeeViewModel model_, string userID);
        short DeleteThirdpartyEmp(long ecode);
        short DeboardEmployee(long ecode, int UserID);
    }
}
