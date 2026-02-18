using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.Extensions.Logging;

namespace ePortal.Application.Services
{
    public class ThirdpartyEmployeeService : IThirdpartyEmployeeService
    {
        ThirdpartyEmployeeRepository Repo;
        private ISessionService _session;
        private ILogger<ThirdpartyEmployeeService> _logger;
        public ThirdpartyEmployeeService(ThirdpartyEmployeeRepository _repo, ISessionService sessionService, ILogger<ThirdpartyEmployeeService> logger)
        {
            Repo = _repo;
            _session = sessionService;
            _logger = logger;
        }

        public List<ThirdpartyEmployeeViewModel> GetThirdpartyEmployeeList(int LoginCode)
        {
            return Repo.GetThirdpartyEmployeeList(LoginCode);
        }

        #region  AutoSuggestions

        public dynamic GetCompanyName(string term)
        {
            return Repo.GetCompanyName(term);
        }

        public dynamic GetprojectmanagerName(string companyname, string projectmanager)
        {
            return Repo.GetprojectmanagerName(companyname, projectmanager);
        }

        public dynamic GetAccountmanagerName(string companyname, string Accountmanager)
        {
            return Repo.GetAccountmanagerName(companyname, Accountmanager);
        }

        public dynamic Getprojectmanagermailid(string projectManagerName, string term)
        {
            return Repo.Getprojectmanagermailid(projectManagerName, term);
        }

        public dynamic GetAccountManagermailid(string AccountManagerName, string term)
        {
            return Repo.GetAccountManagermailid(AccountManagerName, term);
        }

        public object GetEcode()
        {
            return Repo.GetEcode();
        }
        public LibResult GetOwnership(string term, int LoginCode)
        {
            return Repo.GetOwnership(term, LoginCode);
        }

        public List<SelectOwnShipData> GetOwnershipList(int LoginCode)
        {
            return Repo.GetOwnershipList(LoginCode);
        }

        #endregion

        public short AddThirdpartyEmployeeData(ThirdpartyEmployeeViewModel model_, string userID)
        {
            return Repo.AddThirdpartyEmployeeData(model_, userID);
        }

        // Added by TTL SR94104 - CR6022
        public List<ThirdpartyEmpUploadModel> BulkUploadEmployeeDetails(List<ThirdpartyEmpUploadModel> employeeDetails, string userId)
        {
            return Repo.BulkUploadEmployeeDetails(employeeDetails, userId);
        }
        public long GetExistingEmpCode(string associateCompanyID, string companyName)
        {
            return Repo.GetExistingEmpCode(associateCompanyID, companyName);
        }
        public string GetValidEmployee(string eCode)
        {
            return Repo.GetValidEmployee(eCode);
        }
        // End by TTL SR94104 - CR6022
        public short EditThirdpartyempData(ThirdpartyEmployeeViewModel model_, string userID)
        {
            return Repo.EditThirdpartyempData(model_, userID);
        }
        public short DeleteThirdpartyEmp(long ecode)
        {
            return Repo.DeleteThirdpartyEmp(ecode);
        }

        public short DeboardEmployee(long ecode, int LoginCode)
        {
            return Repo.DeboardEmployee(ecode, LoginCode);
        }

    }
}
