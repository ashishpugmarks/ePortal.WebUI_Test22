using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.VQMS_DPR;
using ePortal.ViewModels.Locker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.Application.Services
{
    public class LockerManagementService : ILockerManagementService
    {
        private readonly CommonRepository _commonRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly string _uploadPath;
        private readonly IAppConfigurationService _env;
        private readonly LockerManagementRepository _lockerManagement;
        public LockerManagementService(
            CommonRepository commonRepository,
            IConfiguration configuration,
            IAppConfigurationService env, IEmailNotificationService emailNotificationService, LockerManagementRepository lockerManagement)
        {
            _commonRepository = commonRepository;
            _configuration = configuration;
            _emailNotificationService = emailNotificationService;
            _env = env;
            _lockerManagement = lockerManagement;
        }




        public List<LocationDto> GetSiteList()
        {
            return _lockerManagement.GetSiteList();
        }

        public int AddMapping(LockerAdminLocationMapViewModel model)
        {
            if (_lockerManagement.CheckExists(model.SiteId, model.AdminCode))
                return 0;

            return _lockerManagement.AddAdmin(model);
        }

        public List<EmployeeDetailDto> GetAdminsBySite()
        {
            return _lockerManagement.GetAdminsBySite();
        }
        public List<LockerAdminLocationMapViewModel> GetAdminMappingList()
        {
            return _lockerManagement.GetAdminMappingList();
        }

        public LockerAdminLocationMapViewModel GetAdminMappingById(int id)
        {
            return _lockerManagement.GetAdminMappingById(id);
        }

        public int UpdateAdminMapping(LockerAdminLocationMapViewModel model)
        {
            return _lockerManagement.UpdateAdminMapping(model);
        }


        public List<int> GetAdminMappedLocations(long empCode)
        {
            return _lockerManagement.GetAdminMappedLocations(empCode);
        }

        public LockerAdminRequestDTO GetLockerMangmentDataForAdmin(int empCode)
        {
            var siteIds = GetAdminMappedLocations(empCode);
            if (siteIds == null || siteIds.Count == 0)
                return new LockerAdminRequestDTO();

            LockerAdminRequestDTO  final = new LockerAdminRequestDTO
            {
                Location = _lockerManagement.GetSitesByIds(siteIds)
            };

            foreach (var siteId in siteIds)
            {
                
                var lockerlist = _lockerManagement.GetLockerMangmentDataForAdmin(siteId);
                final.AdminRequest.AddRange(lockerlist);
            }
            
            final.TotalRequest = final.AdminRequest.Count;
            final.PendingRequest = final.AdminRequest.Count(r => r.Status == 2);
            final.ApprovedRequest = final.AdminRequest.Count(r => r.Status == 3);


            return final;
            
        }

        public FloorResponse GetFloorList(int siteId)
        {
            return _lockerManagement.GetFloorList(siteId);
        }

        public LockerResponse GetLockerListByFloor(int floorId)
        {
            return _lockerManagement.GetLockerListByFloor(floorId);
        }
        public LockerBoxResponse GetLockerBoxesByLockerId(int lockerId)
        {
            return _lockerManagement.GetLockerBoxesByLockerId(lockerId);
        }
        public bool AssignLocker(int requestId, int floorId, int lockerId, int boxId, int assignBy)
        {
            return _lockerManagement.AssignLocker(requestId, floorId, lockerId, boxId, assignBy);
        }
    }
}
