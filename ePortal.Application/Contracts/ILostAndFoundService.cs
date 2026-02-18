using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ePortal.DomainClasses;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;

namespace ePortal.Application.Contracts
{
    public interface ILostAndFoundService
    {
        LostAndFoundViewModel GetById(long id);
        long Create(LostAndFoundViewModel model, string uploadedBy, string uploadedByEmpCode);
        bool Update(LostAndFoundViewModel model, string modifiedBy);
        bool Delete(long id, string deletedBy);
        List<SelectListItmesVm> GetLocation();
        List<LostAndFoundViewModel> GetAll();
        List<LostAndFoundViewModel> GetAllActive();
        List<LostAndFoundViewModel> GetByUserLocation(long userSiteId);
        List<LostAndFoundViewModel> GetActiveByUserLocation(long userSiteId);
        string GetUserLocationName(long userSiteId);
        //MAPPING
        List<SecurityLocationMapViewModel> GetAllMapping();
        SecurityLocationMapViewModel? GetByIdMapping(long id);
        long CreateMapping(SecurityLocationMapViewModel model, string createdBy);
        bool UpdateMapping(SecurityLocationMapViewModel model, string modifiedBy);
        List<SelectListItmesVm> GetSiteList();
        List<SelectListItmesVm> GetSecurityEmpList(string term);

        List<SelectListItmesVm> GetAllEmployeeMapping();
        bool IsUserMappedAsSecurityPersonnel(long empCode);
     

        // Status management methods for security personnel
        bool ApproveItem(long id, string securityRemarks, string processedBy, string processedByEmpCode);
        bool SendBackItem(long id, string securityRemarks, string processedBy, string processedByEmpCode);
        bool RejectItem(long id);
        bool CloseItem(long id, string claimedBy, string claimedByEmpCode, string verificationDetails, string processedBy, string processedByEmpCode, string companyName, long? empType);

        // Get items for security dashboard with different status filters
        List<LostAndFoundViewModel> GetPendingItems(long userSiteId);
        List<LostAndFoundViewModel> GetAllItemsForSecurity(long userSiteId);
     
        // Get user email for notifications
        string GetUserEmailByEmpCode(string empCode);
        List<LostAndFoundViewModel> GetRequestByUserId(string userId);
        bool CancelRequestById(long id);

        (string FullName, string DepartmentName, string Designation) GetEmployeeDetailsByEmpCode(long empCode);
        public LostAndFoundViewModel GetDetailById(long id);
        List<LostAndFound> GetPendingItemsOlderThanWorkingDays(int workingDays);
        List<LostAndFoundViewModel> GetLostAndFoundForExprotExcel(LostAndFoundExportModel model, long userSiteId);
      
        string LostAndFoundExcelHtml(List<LostAndFoundViewModel> lostAndFoundList);
      
        void AddLostAndFoundHistory(LostAndFoundHistory history);
        List<LostAndFoundHistoryViewModel> GetHistoryByItemId(long Id);

        List<long> GetSecurityMappedLocations(long empCode);
    }
}
