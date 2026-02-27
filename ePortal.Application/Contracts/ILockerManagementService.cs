using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using ePortal.ViewModels.Locker;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.Application.Contracts
{
    public interface ILockerManagementService
    {
        //Locker Management
        //List<LockerManagementViewModel> GetLockerRequestList(long userSiteId);

        int AddMapping(LockerAdminLocationMapViewModel model);
        List<LockerAdminLocationMapViewModel> GetAdminMappingList();
        List<EmployeeDetailDto> GetAdminsBySite();
        List<LocationDto> GetSiteList();

        LockerAdminLocationMapViewModel GetAdminMappingById(int id);
        int UpdateAdminMapping(LockerAdminLocationMapViewModel model);

        LockerAdminRequestDTO GetLockerMangmentDataForAdmin(int empCode);

        FloorResponse GetFloorList(int siteId);
        LockerResponse GetLockerListByFloor(int floorId);
        LockerBoxResponse GetLockerBoxesByLockerId(int floorId);
        bool AssignLocker(int requestId, int floorId, int lockerId, int boxId, int assignBy);
    }
}
