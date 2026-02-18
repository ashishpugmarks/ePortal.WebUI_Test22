using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using ePortal.DomainClasses;
using Microsoft.AspNetCore.Http;

namespace ePortal.ViewModels
{
    public class VehicleDTO
    {
        public class VehicleUpsertDto
        {
            public int VehicleId { get; set; }
            public string EmpCode { get; set; } = string.Empty;

            public short VehicleType { get; set; }

            public short VehicleCategory { get; set; } 

            public string DLNumber { get; set; } = string.Empty;
            public string? DriverName { get; set; }

            public DateTime? DLValidTill { get; set; }
            public short DLType { get; set; }

            public long LocationId { get; set; }
            public string? LocationName { get; set; }
            public List<string> DLFilePaths { get; set; } = new();
            public List<IFormFile> DLFiles { get; set; } = new();

            public List<VehicleDetailDto> Vehicles { get; set; } = new();
            public List<VehicleHistoryDto>? VehicleHistory { get; set; } = new();

            public string EmpName { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string Designation { get; set; } = string.Empty;
            public string MobileNumber { get; set; } = string.Empty;
        }

        public class VehicleDetailDto
        {
            public int Id { get; set; }
            public string VehicleNumber { get; set; } = string.Empty;

            public short VehicleCategory { get; set; }

            public short FuelType { get; set; }

            public DateTime ModelYear { get; set; }

            public short OwnerType { get; set; }

            public string OwnerName { get; set; } = string.Empty;

            public string DLNumber { get; set; } = string.Empty;

            public DateTime? DLValidTill { get; set; }
            public short DLType { get; set; }

            public List<IFormFile> DLFiles { get; set; } = new();

            public List<string> DLFilePaths { get; set; } = new();

            public string? VehicleCustodian { get; set; }
            public string CustodianEmpName { get; set; } = string.Empty;
            public string CustodianDepartment { get; set; } = string.Empty;
            public string CustodianDesignation { get; set; } = string.Empty;

            // RC
            public string RCNumber { get; set; } = string.Empty;
            public DateTime? RCValidTill { get; set; }

            public List<IFormFile>? RCFile { get; set; }

            // Existing saved RC files (edit mode)
            public List<string> RCFilePaths { get; set; } = new();


            // Insurance
            public string InsuranceNumber { get; set; } = string.Empty;
            public DateTime? InsuranceValidTill { get; set; }

            // New uploads
            public List<IFormFile>? InsuranceFile { get; set; }

            // Existing saved files
            public List<string> InsuranceFilePaths { get; set; } = new();


            // PUC
            public string PUCNumber { get; set; } = string.Empty;
            public DateTime? PUCValidTill { get; set; }

            // New uploads
            public List<IFormFile>? PUCFile { get; set; }

            // Existing saved files
            public List<string> PUCFilePaths { get; set; } = new();
        }

        public class EmployeeDetailDto
        {
            public long? EmpCode { get; set; }
            public string FullName { get; set; }
            public string MobileNumber { get; set; }
            public string Designation { get; set; }
            public string Department { get; set; }
            public long? LocationId { get; set; }
            public string LocationName { get; set; }
        }
        public class LocationDto
        {
            public long Id { get; set; }
            public long Value { get; set; }
            public string Text { get; set; }
        }

        public class VehicleMasterDto
        {
            public int VehicleId { get; set; }
            public string EmpCode { get; set; } = string.Empty;
            public EmployeeDetailDto? Employee { get; set; }
            public string EmpName { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string Designation { get; set; } = string.Empty;
            public string MobileNumber { get; set; } = string.Empty;
            public bool IsExpired { get; set; }
            public int Status { get; set; }
            public string? DriverName { get; set; }

            // Admin Step
            public string? AdminActionBy { get; set; }
            public DateTime? AdminActionDate { get; set; }

            public long? ParkingZoneId { get; set; }
            public string? ParkingName { get; set; }
            public PARKING_ZONE? ParkingZone { get; set; }

            // Security Step
            public string? SecurityActionBy { get; set; }
            public DateTime? SecurityActionDate { get; set; }


            // Vehicle Info
            public short VehicleType { get; set; }
            public short VehicleCategory { get; set; }

            public string DlNumber { get; set; } = string.Empty;
            public DateTime? DlValidTill { get; set; }
            public short DlType { get; set; }
            public List<string>? DlPhotoPath { get; set; }

            // Audit
            public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
            public long LocationId { get; set; }
            public LocationDto Location { get; set; }
            public String? LocationName { get; set; }
            public List<EMPVehicleDetailDto>? VehicleDetail { get; set; }
            public List<VehicleHistoryDto>? VehicleHistory { get; set; }
        }

        public class EMPVehicleDetailDto
        {
            public int Id { get; set; }
            public string VehicleNo { get; set; } = string.Empty;
            public short FuelType { get; set; }
            public DateTime ModelYear { get; set; }
            public short OwnerType { get; set; }
            public string OwnerName { get; set; } = string.Empty;
            public int VehicleId { get; set; }

            public string? VehicleCustodian { get; set; }
            public string CustodianEmpName { get; set; } = string.Empty;
            public string CustodianDepartment { get; set; } = string.Empty;
            public string CustodianDesignation { get; set; } = string.Empty;

            public string DLNumber { get; set; } = string.Empty;
            public DateTime? DLValidTill { get; set; }
            public short DLType { get; set; }
            public List<string> DLFilePaths { get; set; } = new();

            // RC
            public string? RcNo { get; set; }
            public DateTime? RcValidTill { get; set; }
            public List<string>? RcPhotoPath { get; set; }

            // Insurance
            public string? InsuranceNo { get; set; }
            public DateTime? InsuranceValidTill { get; set; }
            public List<string>? InsurancePhotoPath { get; set; }

            // PUC
            public string? PucNo { get; set; }
            public DateTime? PucValidTill { get; set; }
            public List<string>? PucPhotoPath { get; set; }

            public string? UploadedBy { get; set; } = string.Empty;
            public DateTime UploadedOn { get; set; }
        }

        public class VehicleHistoryDto
        {
            public long Id { get; set; }

            public int VehicleId { get; set; }
            public VehicleMasterDto EmpVehicleMaster { get; set; } = default!;

            public string ActionBy { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;

            public int? AdminStatus { get; set; } = 0;
            public string? AdminRemark { get; set; } = string.Empty;
            public int? SecurityStatus { get; set; } = 0;
            public string? SpRemark { get; set; } = string.Empty;
            public int? SendBackFor { get; set; }

            public string ActionByRole { get; set; } = string.Empty; // ADMIN, SECURITY

            public long? ParkingZoneId { get; set; }
            public ParkingZoneDto? ParkingZone { get; set; }

            public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        }
        public class VehicleResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }

            public VehicleResponseDto(bool success, string message, object data = null)
            {
                Success = success;
                Message = message;
                Data = data;
            }
        }

        public class DeactivateVehicle
        {
            public int VehicleId { get; set; }
            public int ActionByRole { get; set; } //role
            public string Remark { get; set; }
        }

        public class ZoneStatisticsDto()
        {
            public long Id { get; set; }
            public string ZoneName { get; set; } = string.Empty;
            public int TotalCount { get; set; } 
            public int Assigned { get; set; }
            public int Remaining { get; set; } 

        }
        public class ApproveRequestDto
        {
            public int VehicleId { get; set; }
            public int ZoneId { get; set; }
            public string Remark { get; set; } = string.Empty;
        }

        public class VehicleSecurityActionDto
        {
            public int VehicleId { get; set; }
            public int? Action { get; set; }
            public int? Sendbackfor { get; set; }
            public string Remark { get; set; } = string.Empty;
        }

        public class VehicleAdminDto
        {
            public List<VehicleMasterDto> VehicleMaster { get; set; } = new();

            public List<ParkingZoneDto> TotalZones { get; set; } = new();
            public List<ParkingZoneDto> ZonesOccupied { get; set; } = new();
            public List<ParkingZoneDto> ActiveZones { get; set; } = new();
            public List<ParkingZoneDto> InactiveZones { get; set; } = new();

            public int TotalVehicles { get; set; }
            public int PendingVehicles { get; set; }
            public int ApprovedVehicles { get; set; }

            public List<LocationDto>? Location { get; set; }
        }


        public class VehicleSecurityDto()
        {
            public List<VehicleMasterDto> VehicleMaster { get; set; } = new();
            public int TotalVehicles { get; set; }
            public int PendingVehicles { get; set; }
            public int ApprovedVehicles { get; set; }
            public int SendBackVehicles { get; set; }
            public int RejectVehicles { get; set; }
            public int ExpireVehicles { get; set; }
            public List<LocationDto>? Location { get; set; }
        }

        public class ZoneSummaryDto
        {
            public int TotalZones { get; set; }
            public int ZonesOccupied { get; set; }
            public int ActiveZones { get; set; }
            public int InactiveZones { get; set; }
        }
        public class ParkingZoneDto
        {
            public long Id { get; set; }
            public string ZoneName { get; set; } = string.Empty;
            public string? ZoneCode { get; set; }
            public string? Description { get; set; }
            public int TotalCapacity { get; set; }
            public int Capacity { get; set; }
            public int? AvailableCapacity { get; set; }
            public int LocationId { get; set; }
            public string? LocationName { get; set; }
            public List<LocationDto>? Locations { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }
        }

        public class VehicleExportDto
        {
            public int? Status { get; set; }
            public int? VehicleCategory { get; set; }
            public int? LocationId { get; set; }
            public DateTime? DateFrom { get; set; }
            public DateTime? DateTo { get; set; }
        }

        public class VehicleExportResponseDto
        {
            public int VehicleId { get; set; }
            public string EmpCode { get; set; } = string.Empty;
            public string EmpName { get; set; } = string.Empty;
            public int VehicleCategory { get; set; }
            public DateTime CreatedDate { get; set; }

            public long LocationId { get; set; }
            public string LocationName { get; set; } = string.Empty;

            public long? ParkingZoneId { get; set; }
            public string? ParkingZoneName { get; set; }

            public string? AdminActionBy { get; set; }
            public DateTime? AdminActionDate { get; set; }

            // 0 = Pending, 1 = Approved
            public int Status { get; set; }
        }



        public enum VehicleFuelType
        {
            Diesel = 1,
            Petrol = 2,
            Electrical = 3,
        }

        public static string GetVehicleFuelTypeText(int type)
        {
            return type switch
            {
                (int)VehicleFuelType.Diesel => "Diesel",
                (int)VehicleFuelType.Petrol => "Petrol",
                (int)VehicleFuelType.Electrical => "Electrical",
                _ => "Unknown"
            };
        }


    }
}
