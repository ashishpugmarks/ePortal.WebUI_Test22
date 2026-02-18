using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly VehicleRepository _vehicleRepo;
        private readonly CommonRepository _CommonRepo;
        private readonly Tuple<short, long> _retVal_tuple;
        private readonly IVehicleEmailNotificationService _emailNotificationService;

        public VehicleService(VehicleRepository vehicleRepo, CommonRepository commonRepo, IVehicleEmailNotificationService emailNotificationService)
        {
            _vehicleRepo = vehicleRepo;
            _CommonRepo = commonRepo;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
            _emailNotificationService = emailNotificationService;
        }

        public EmployeeDetailDto GetEmployeeDetailsByEmpCode(long empCode)
        {
            return _vehicleRepo.GetEmployeeDetailsByEmpCode(empCode);
        }

        public VehicleUpsertDto GetEditVehicleDetailById(int vehicleId)
        {
            var master = _vehicleRepo.GetVehicleDetailById(vehicleId);
            return MapToUpsert(master);
        }

        public VehicleUpsertDto MapToUpsert(VehicleMasterDto src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            var dto = new VehicleUpsertDto
            {
                VehicleId = src.VehicleId,
                EmpCode = src.EmpCode,
                VehicleType = src.VehicleType,
                VehicleCategory = src.VehicleCategory,
                DLNumber = src.DlNumber,
                DLValidTill = src.DlValidTill,
                DLType = src.DlType,
                DriverName = src.DriverName,
                LocationId = src.LocationId,
                DLFilePaths = src.DlPhotoPath ?? new(),
                EmpName = src.EmpName,
                Department = src.Department,
                Designation = src.Designation,
                MobileNumber = src.MobileNumber,
                LocationName = src.LocationName,
            };

            dto.VehicleHistory = src.VehicleHistory;

            dto.Vehicles = src.VehicleDetail?
                .Select(v => new VehicleDetailDto
                {
                    Id = v.Id,
                    VehicleNumber = v.VehicleNo,
                    FuelType = v.FuelType,
                    ModelYear = v.ModelYear,
                    OwnerType = v.OwnerType,
                    OwnerName = v.OwnerName,

                    // DL top-level → child copy
                    DLNumber = src.DlNumber,
                    DLValidTill = src.DlValidTill,
                    DLType = src.DlType,
                    DLFilePaths = src.DlPhotoPath,


                    // RC
                    RCNumber = v.RcNo ?? string.Empty,
                    RCValidTill = v.RcValidTill,
                    RCFilePaths = v.RcPhotoPath ?? new(),

                    // Insurance
                    InsuranceNumber = v.InsuranceNo ?? string.Empty,
                    InsuranceValidTill = v.InsuranceValidTill,
                    InsuranceFilePaths = v.InsurancePhotoPath ?? new(),

                    // PUC
                    PUCNumber = v.PucNo ?? string.Empty,
                    PUCValidTill = v.PucValidTill,
                    PUCFilePaths = v.PucPhotoPath ?? new(),

                    VehicleCategory = src.VehicleCategory,
                    VehicleCustodian = v.VehicleCustodian,
                    CustodianDepartment = v.CustodianDepartment,
                    CustodianDesignation = v.CustodianDesignation,
                    CustodianEmpName = v.CustodianEmpName,
                })
                .ToList()
                ?? new List<VehicleDetailDto>();

            return dto;
        }

        public VehicleMasterDto GetVehicleDetailByVehicleNo(string vehicleNo)
        {
            //return _vehicleRepo.GetVehicleDetailByVehicleNo(vehicleNo);
            throw new NotImplementedException();
        }

        public List<VehicleMasterDto> GetVehicleDetailsBySiteId(long userSiteId)
        {
            return _vehicleRepo.GetVehicleDetailsBySiteId(userSiteId);
        }

        //upd
        public VehicleResponseDto UpdateVehicleStatus(int vehicleId, int status, string remarks, string userId)
        {
            //var success = _vehicleRepo.UpdateVehicleStatus(vehicleId, status, remarks);

            //if (success)
            //{
            //    VehicleHistoryDto vehicleHistoryDto = new VehicleHistoryDto
            //    {
            //        VEHICLEID = vehicleId,
            //        STATUS = status,
            //        REMARKS = remarks,
            //        CHANGEDBY = userId,
            //    };
            //    var saveVehicleHistory = _vehicleRepo.SaveVehicleHistory(vehicleHistoryDto);
            //    var get = _vehicleRepo.GetVehicleHistoryById(1);
            //    var responseData = new { VehicleId = vehicleId, NewStatus = status };
            //    return new VehicleResponseDto(true, "Vehicle status updated successfully!", responseData);
            //}
            //else
            //{
            //    return new VehicleResponseDto(false, "Failed to update vehicle status. Please check details.", null);
            //}
            throw new NotImplementedException();
        }



        public LocationDto GetUserLocationName(int userSiteId)
        {
            return _vehicleRepo.GetSiteById(userSiteId);
        }
        public string VehicleExcelHtml(List<VehicleExportResponseDto> vehicleList)
        {
            string str = "";
            if (vehicleList != null && vehicleList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>Sr No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>VehicleId</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>VehicleCategory</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Created Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Location</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Zone</th>");
                stringBuilder.Append("</tr>");

                int srNo = 1;
                foreach (var item in vehicleList)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (item.VehicleId) + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ((VehicleCategory)item.VehicleCategory).GetText() + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + item.CreatedDate.ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ((VehicleStatus)item.Status).GetText() + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (item.LocationName) + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (item.EmpName) + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + (item.ParkingZoneName) + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        public VehicleResponseDto SaveVehicleRequest(VehicleUpsertDto dto, Employee_Details emp, string userId)
        {
            if (emp == null || string.IsNullOrEmpty(userId))
                return new VehicleResponseDto(false, "Session expired. Login again.");

            if (!long.TryParse(dto.EmpCode, out _))
                return new VehicleResponseDto(false, "Employee code must be numeric.");

            foreach (var v in dto.Vehicles)
            {
                if (_vehicleRepo.VehicleNumberExists(v.VehicleNumber))
                    return new VehicleResponseDto(false, $"Vehicle number already exists: {v.VehicleNumber}");

                if (!string.IsNullOrEmpty(v.RCNumber) &&
                    _vehicleRepo.RCNumberExists(v.RCNumber))
                    return new VehicleResponseDto(false, $"RC number already exists: {v.RCNumber}");

                if (!string.IsNullOrEmpty(v.InsuranceNumber) &&
                    _vehicleRepo.InsuranceNumberExists(v.InsuranceNumber))
                    return new VehicleResponseDto(false, $"Insurance number already exists: {v.InsuranceNumber}");

                if (!string.IsNullOrEmpty(v.PUCNumber) &&
                    _vehicleRepo.PUCNumberExists(v.PUCNumber))
                    return new VehicleResponseDto(false, $"PUC number already exists: {v.PUCNumber}");
            }

            if (!string.IsNullOrEmpty(dto.DLNumber) &&
                _vehicleRepo.DLNumberExists(dto.DLNumber,dto.EmpCode))
            {
                return new VehicleResponseDto(false, $"DL number already exists: {dto.DLNumber}");
            }

            var employeeCode = Convert.ToInt32(dto.EmpCode);
            var empdata = GetEmployeeDetailsByEmpCode(employeeCode);

            dto.LocationId = empdata.LocationId ?? 0;
            var result = _vehicleRepo.SaveVehicleRequest(dto, userId); 
            var vehicle = _vehicleRepo.GetVehicleDetailById(Convert.ToInt32(result.Data));
			var AdminEmail = _vehicleRepo.GetAdminEmailsBySiteId(vehicle.LocationId);
			_ = Task.Run(() =>
			{
				try
				{
					
					_emailNotificationService.SendRequestEmailToAdminNotification(vehicle, AdminEmail);
				}
				catch (Exception ex)
				{
				}
			});
			return result;
        }

        public VehicleResponseDto EditVehicleWithDocsAsync(VehicleUpsertDto dto, string userId)
        {
            try
            {
                var vehicleId = dto.VehicleId;

                foreach (var v in dto.Vehicles)
                {
                    if (_vehicleRepo.VehicleNumberExists(v.VehicleNumber, vehicleId))
                        return new VehicleResponseDto(false, $"Vehicle number already exists: {v.VehicleNumber}");

                    if (!string.IsNullOrEmpty(v.RCNumber) &&
                        _vehicleRepo.RCNumberExists(v.RCNumber, vehicleId))
                        return new VehicleResponseDto(false, $"RC number already exists: {v.RCNumber}");

                    if (!string.IsNullOrEmpty(v.InsuranceNumber) &&
                        _vehicleRepo.InsuranceNumberExists(v.InsuranceNumber, vehicleId))
                        return new VehicleResponseDto(false, $"Insurance number already exists: {v.InsuranceNumber}");

                    if (!string.IsNullOrEmpty(v.PUCNumber) &&
                        _vehicleRepo.PUCNumberExists(v.PUCNumber, vehicleId))
                        return new VehicleResponseDto(false, $"PUC number already exists: {v.PUCNumber}");
                }

                if (!string.IsNullOrEmpty(dto.DLNumber) &&
                    _vehicleRepo.DLNumberExists(dto.DLNumber, dto.EmpCode, vehicleId))
                {
                    return new VehicleResponseDto(false, $"DL number already exists: {dto.DLNumber}");
                }

                var result = _vehicleRepo.EditVehicleWithDocs(dto, userId);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public VehicleResponseDto MyVehicleRequests(long empCode)
        {
            return _vehicleRepo.MyVehicleRequests(empCode);
        }

        public VehicleMasterDto GetVehicleDetailById(int vehicleId)
        {
            return _vehicleRepo.GetVehicleDetailById(vehicleId);
        }

        public List<ParkingZoneDto> GetParkingZones()
        {
            return _vehicleRepo.GetParkingZones();
        }

        public VehicleResponseDto UpsertZone(ParkingZoneDto zone, string userId)
        {
            if (userId == null || string.IsNullOrEmpty(userId))
                return new VehicleResponseDto(false, "Session expired. Login again.");

            return _vehicleRepo.UpsertZone(zone);
        }

        public ParkingZoneDto? GetParkingZoneById(long id)
        {
            return _vehicleRepo.GetParkingZoneById(id);
        }

        public VehicleResponseDto DeleteZone(int id)
        {
            return _vehicleRepo.DeleteZone(id);
        }

        public List<ZoneStatisticsDto> ZoneStatistics(int vehicleId)
        {
            return _vehicleRepo.ZoneStatistics(vehicleId);
        }

        public VehicleResponseDto ApproveRequest(ApproveRequestDto request, string userId)
        {
            var result = _vehicleRepo.ApproveRequest(request, userId);
            if (result.Success)
            {
                var vehicle = _vehicleRepo.GetVehicleDetailById(request.VehicleId);
                var userEmail = _vehicleRepo.GetUserEmailByEmpCode(vehicle.EmpCode);
                var SecuritypersonalEmail = _vehicleRepo.GetApproveByAdminEmailToSecurityNotification(vehicle.LocationId);
                var Admindetails = _vehicleRepo.GetApproveByAdminDetials(request.VehicleId);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            _emailNotificationService.SendApproveByAdminNotification(vehicle, userEmail);                            
                            _emailNotificationService.SendApproveByAdminEmailToSecurityNotification(vehicle, SecuritypersonalEmail, Admindetails);                            
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }
            }
            return result;
        }

        public VehicleResponseDto DeleteVehicle(int vehicleId)
        {
            var vehicle = GetVehicleDetailById(vehicleId);
            if (vehicle == null)
                throw new InvalidOperationException($"Vehicle not found. Id: {vehicleId}");

            var userEmail = _vehicleRepo.GetUserEmailByEmpCode(vehicle.EmpCode);

            string adminEmail = null;
            string securityEmail = null;

            if (vehicle.VehicleHistory != null && vehicle.VehicleHistory.Count > 0)
            {
                var orderedHistory = vehicle.VehicleHistory
                    .OrderByDescending(x => x.Id)
                    .ToList();

                var adminHistory = orderedHistory
                    .FirstOrDefault(x => x.ActionByRole == ((int)VehicleActionByRole.Admin).ToString());

                var securityHistory = orderedHistory
                    .FirstOrDefault(x => x.ActionByRole == ((int)VehicleActionByRole.Security).ToString());

                if (adminHistory != null)
                    adminEmail = _vehicleRepo.GetUserEmailByEmpCode(adminHistory.ActionBy);

                if (securityHistory != null)
                    securityEmail = _vehicleRepo.GetUserEmailByEmpCode(securityHistory.ActionBy);
            }

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                try
                {
                    _emailNotificationService.SendVehicleRemoveNotification(
                        vehicle,
                        userEmail,
                        adminEmail,
                        securityEmail);
                }
                catch (Exception ex)
                {
                }
            }

            return _vehicleRepo.DeleteVehicle(vehicleId);
        }


        public List<LocationDto> GetSiteList()
        {
            return _vehicleRepo.GetSiteList();
        }

        public int AddMapping(AdminLocationMapViewModel model)
        {
            if (_vehicleRepo.CheckExists(model.SiteId, model.AdminCode))
                return 0;

            return _vehicleRepo.AddAdmin(model);
        }

        public List<EmployeeDetailDto> GetAdminsBySite()
        {
            return _vehicleRepo.GetAdminsBySite();
        }

        public List<AdminLocationMapViewModel> GetAdminMappingList()
        {
            return _vehicleRepo.GetAdminMappingList();
        }
        public AdminLocationMapViewModel GetAdminMappingById(int id)
        {
            return _vehicleRepo.GetAdminMappingById(id);
        }

        public int UpdateAdminMapping(AdminLocationMapViewModel model)
        {
            return _vehicleRepo.UpdateAdminMapping(model);
        }

        public int AddSecurityMapping(SecurityLocationMapViewModelVP model)
        {
            if (_vehicleRepo.CheckSecurityExists(model.SiteId, model.EmpCode))
                return 0;

            return _vehicleRepo.AddSecurityMapping(model);
        }

        public int UpdateSecurityMapping(SecurityLocationMapViewModelVP model)
        {
            return _vehicleRepo.UpdateSecurityMapping(model);
        }

        public List<SecurityLocationMapViewModelVP> GetSecurityMappingList()
        {
            return _vehicleRepo.GetSecurityMappingList();
        }

        public SecurityLocationMapViewModelVP GetSecurityMappingById(int id)
        {
            return _vehicleRepo.GetSecurityMappingById(id);
        }

        public List<EmployeeDetailDto> GetSecurityPersonnelBySite()
        {
            return _vehicleRepo.GetSecurityPersonnelBySite();
        }

        public List<int> GetAdminMappedLocations(long empCode)
        {
            return _vehicleRepo.GetAdminMappedLocations(empCode);
        }

        public List<int> GetSecurityMappedLocations(long empCode)
        {
            return _vehicleRepo.GetSecurityMappedLocations(empCode);
        }

        public VehicleAdminDto GetVehicleDataForAdmin(int empCode)
        {
            var siteIds = GetAdminMappedLocations(empCode);
            if (siteIds == null || siteIds.Count == 0)
                return new VehicleAdminDto();

            var final = new VehicleAdminDto
            {
                Location = _vehicleRepo.GetSitesByIds(siteIds)
            };

            foreach (var siteId in siteIds) 
            {
                // vehicles
                var vehicles = _vehicleRepo.GetVehicleDetailsBySiteId(siteId);
                if (vehicles?.Any() == true)
                    final.VehicleMaster.AddRange(vehicles);

                var zones = _vehicleRepo.GetZonesBySiteId(siteId);

                final.TotalZones.AddRange(zones);
                final.ZonesOccupied.AddRange(
                    zones.Where(z => (z.AvailableCapacity ?? 0) == 0)
                );
                final.ActiveZones.AddRange(
                    zones.Where(z => z.IsActive)
                );
                final.InactiveZones.AddRange(
                    zones.Where(z => !z.IsActive)
                );
            }

            final.TotalVehicles = final.VehicleMaster.Count;

            final.PendingVehicles = final.VehicleMaster.Count(v =>
            {
                var latest = v.VehicleHistory?
                    .OrderByDescending(h => h.Id)
                    .FirstOrDefault();

                return latest == null || latest.AdminStatus == 0;
            });

            final.ApprovedVehicles = final.VehicleMaster.Count(v =>
            {
                var latest = v.VehicleHistory?
                    .OrderByDescending(h => h.Id)
                    .FirstOrDefault();

                return latest != null && latest.AdminStatus == 1;
            });

            return final;
        }


        public List<VehicleExportResponseDto> GetAdminVehiclesForExportExcel(VehicleExportDto model, int empCode)
        {
            var siteIds = GetAdminMappedLocations(empCode)
                .Select(id => (long)id)
                .ToList();

            if (siteIds == null || siteIds.Count == 0)
                return new List<VehicleExportResponseDto>();

            if (model.LocationId.HasValue &&
                !siteIds.Contains(model.LocationId.Value))
            {
                return new List<VehicleExportResponseDto>();
            }

            return _vehicleRepo.GetAdminVehiclesForExportExcel(model, siteIds);
        }

        public List<VehicleExportResponseDto> GetSecurityVehiclesForExportExcel(VehicleExportDto model, int empCode)
        {
            var siteIds = GetSecurityMappedLocations(empCode)
                .Select(id => (long)id)
                .ToList();

            if (siteIds == null || siteIds.Count == 0)
                return new List<VehicleExportResponseDto>();

            if (model.LocationId.HasValue &&
                !siteIds.Contains(model.LocationId.Value))
            {
                return new List<VehicleExportResponseDto>();
            }

            return _vehicleRepo.GetSecurityVehiclesForExportExcel(model, siteIds);
        }

        public VehicleSecurityDto GetVehicleDataForSecurity(int empCode)
        {
            var siteIds = GetSecurityMappedLocations(empCode);
            if (siteIds == null || siteIds.Count == 0)
                return new VehicleSecurityDto();
            var location = _vehicleRepo.GetSitesByIds(siteIds);

            var final = new VehicleSecurityDto();
            final.Location = location;
            foreach (var siteId in siteIds)
            {
                var vehicles = _vehicleRepo.GetSecurityVehicleDetailsBySiteId(siteId);
                if (vehicles != null && vehicles.Count > 0)
                    final.VehicleMaster.AddRange(vehicles);
            }
            final.TotalVehicles = final.VehicleMaster.Count;

            final.PendingVehicles = final.VehicleMaster.Count(v =>
            {
                var latest = v.VehicleHistory?
                    .OrderByDescending(h => h.Id)
                    .FirstOrDefault();

                return latest != null && latest.SecurityStatus == 0;
            });

            final.ApprovedVehicles = final.VehicleMaster.Count(v =>
            {
                var latest = v.VehicleHistory?
                    .OrderByDescending(h => h.Id)
                    .FirstOrDefault();

                return latest != null && latest.SecurityStatus == 1;
            });

            final.SendBackVehicles = final.VehicleMaster.Count(v =>
            {
                var latest = v.VehicleHistory?
                    .OrderByDescending(h => h.Id)
                    .FirstOrDefault();

                return latest != null && latest.SecurityStatus == 2;
            });

            final.RejectVehicles = final.VehicleMaster.Count(v =>
            {
                var latest = v.VehicleHistory?
                    .OrderByDescending(h => h.Id)
                    .FirstOrDefault();

                return latest != null && latest.SecurityStatus == 3;
            });

            final.ExpireVehicles = final.VehicleMaster.Count(v => v.IsExpired);

            return final;
        }

        public VehicleResponseDto VehicleSecurityAction(VehicleSecurityActionDto request, string userId)
        {
            var result = _vehicleRepo.VehicleSecurityAction(request, userId);
            if (result.Success)
            {
                var vehicle = _vehicleRepo.GetVehicleDetailById(request.VehicleId);
                var userEmail = _vehicleRepo.GetUserEmailByEmpCode(vehicle.EmpCode);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            if (request.Action == (int)VehicleStatus.Approved)
                            {
                                _emailNotificationService.SendApproveBySecurityNotification(vehicle, userEmail);
                            }
                            else if (request.Action == (int)VehicleStatus.SendBack)
                            {
                                _emailNotificationService.SendSendBackNotification(vehicle, userEmail);
                            }
                            else if (request.Action == (int)VehicleStatus.Reject)
                            {
                                _emailNotificationService.SendRejectNotification(vehicle, userEmail);
                            }
                            else
                            {
                                _emailNotificationService.SendHoldNotification(vehicle, userEmail);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }
            }
            return result;
        }

        public List<VehicleMasterDto> GetExpiredVehicles(long empCode)
        {
            var siteIds = GetSecurityMappedLocations(empCode);
            if (siteIds == null || siteIds.Count == 0)
                return new List<VehicleMasterDto>();
            var final = new List<VehicleMasterDto>();
            foreach (var siteId in siteIds)
            {
                var vehicles = _vehicleRepo.GetExpiredVehicles(siteId);
                if (vehicles != null && vehicles.Count > 0)
                    final.AddRange(vehicles);
            }

            return final;
        }

        public VehicleResponseDto DeactivateVehicle(DeactivateVehicle request, string userId)
        {
            var result = _vehicleRepo.DeactivateVehicle(request, userId);
            if (result.Success)
            {
                var vehicle = _vehicleRepo.GetVehicleDetailById(request.VehicleId);
                var userEmail = _vehicleRepo.GetUserEmailByEmpCode(vehicle.EmpCode);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    _ = Task.Run(() =>
                    {
                        try
                        {
                           _emailNotificationService.SendDeactivateNotification(vehicle, userEmail);
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }
            }
            return result;
        }

        public bool CanEdit(VehicleUpsertDto vehicle)
        {
            if (vehicle == null)
                return false;

            var latestHistory = vehicle.VehicleHistory?
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (latestHistory == null)
                return false;

            if (latestHistory.SecurityStatus == (int)VehicleStatus.SendBack &&
                latestHistory.SendBackFor == 1)
                return true;

            if (latestHistory.AdminStatus == null ||
                latestHistory.AdminStatus == (int)VehicleStatus.Pending)
                return true;

            if (vehicle.VehicleCategory == (int)VehicleCategory.CompanyDriver)
                return true;

            return false;
        }
    }
}
