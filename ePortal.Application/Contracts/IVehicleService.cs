using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.ViewModels;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.Application.Contracts
{
    public interface IVehicleService
    {
        VehicleResponseDto SaveVehicleRequest(VehicleUpsertDto dto, Employee_Details emp, string userId);
        EmployeeDetailDto GetEmployeeDetailsByEmpCode(long empCode);
        VehicleUpsertDto GetEditVehicleDetailById(int vehicleId);
        VehicleMasterDto GetVehicleDetailByVehicleNo(string vehicleNo);
        VehicleResponseDto EditVehicleWithDocsAsync(VehicleUpsertDto dto ,string userId);
        List<VehicleMasterDto> GetVehicleDetailsBySiteId(long userSiteId);
        VehicleResponseDto UpdateVehicleStatus(int vehicleId, int status, string remarks,string userId);
        List<VehicleExportResponseDto> GetAdminVehiclesForExportExcel(VehicleExportDto model, int empCode);
        List<VehicleExportResponseDto> GetSecurityVehiclesForExportExcel(VehicleExportDto model, int empCode);
        LocationDto GetUserLocationName(int userSiteId);
        string VehicleExcelHtml(List<VehicleExportResponseDto> vehicleList);
        VehicleResponseDto MyVehicleRequests(long empCode);
        VehicleMasterDto GetVehicleDetailById(int vehicleId);
        List<ParkingZoneDto> GetParkingZones();
        VehicleResponseDto UpsertZone(ParkingZoneDto zone,string userId);
        ParkingZoneDto? GetParkingZoneById(long id);
        VehicleResponseDto DeleteZone(int id);
        List<ZoneStatisticsDto> ZoneStatistics(int vehicleId);
        VehicleResponseDto ApproveRequest(ApproveRequestDto request,string userId);
        VehicleResponseDto DeleteVehicle(int vehicleId);
        int AddMapping(AdminLocationMapViewModel model);
        List<EmployeeDetailDto> GetAdminsBySite();
        List<LocationDto> GetSiteList();
        List<AdminLocationMapViewModel> GetAdminMappingList();
        AdminLocationMapViewModel GetAdminMappingById(int id);
        int UpdateAdminMapping(AdminLocationMapViewModel model);
        int AddSecurityMapping(SecurityLocationMapViewModelVP model);
        int UpdateSecurityMapping(SecurityLocationMapViewModelVP model);
        List<SecurityLocationMapViewModelVP> GetSecurityMappingList();
        SecurityLocationMapViewModelVP GetSecurityMappingById(int id);
        List<EmployeeDetailDto> GetSecurityPersonnelBySite();
        List<int> GetAdminMappedLocations(long empCode);
        VehicleAdminDto GetVehicleDataForAdmin(int empCode);
        List<int> GetSecurityMappedLocations(long empCode);
        VehicleSecurityDto GetVehicleDataForSecurity(int empCode);
        VehicleResponseDto VehicleSecurityAction(VehicleSecurityActionDto request, string userId);
        List<VehicleMasterDto> GetExpiredVehicles(long empcode);
        VehicleResponseDto DeactivateVehicle(DeactivateVehicle request, string userId);
        bool CanEdit(VehicleUpsertDto upsertDto);
    }
}
