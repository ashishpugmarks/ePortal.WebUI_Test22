using System.CodeDom;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Xml.Linq;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using HarfBuzzSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static ePortal.ViewModels.VehicleDTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ePortal.Infrastructure.Repositories
{
    public class VehicleRepository
    {
        private EPortalDBContext _ePortaDBContext;
        private SYKI _Syki;
        private readonly IConfiguration _configuration;
        //private readonly string _uploadPath;
        private readonly IAppConfigurationService _configurations;
        public VehicleRepository(EPortalDBContext objEPortalDBContext, IConfiguration configuration, IAppConfigurationService appConfigurations)
        {
            _ePortaDBContext = objEPortalDBContext;
            _Syki = _ePortaDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            _configuration = configuration;
            
            _configurations = appConfigurations;
            // Ensure upload directory exists

        }


        public VehicleResponseDto SaveVehicleRequest(VehicleUpsertDto dto, string userId)
        {
            using var transaction = _ePortaDBContext.Database.BeginTransaction();

            try
            {

                if (dto.VehicleCategory == 3)
                {
                    int? firstVehicleId = null;
                    foreach (var vehicleDetail in dto.Vehicles)
                    {
                        var dlPaths = new List<string>();

                        if (vehicleDetail.DLFiles != null)
                        {
                            foreach (var file in vehicleDetail.DLFiles)
                                dlPaths.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), "DL"));
                        }

                        var vehicle = new EMP_VEHICLE_MASTER
                        {
                            EMPCODE = dto.EmpCode,
                            CREATED_DATE = DateTime.Now,
                            LOCATION_ID = dto.LocationId,
                            VEHICLE_TYPE = dto.VehicleType,
                            VEHICLE_CATEGORY = dto.VehicleCategory,
                            DL_NUMBER = vehicleDetail.DLNumber,
                            DL_VALID_TILL = vehicleDetail.DLValidTill,
                            DL_TYPE = vehicleDetail.DLType,
                            STATUS = 1,
                            DL_PHOTOPATH = dlPaths.Count > 0 ? JsonSerializer.Serialize(dlPaths) : null
                        };

                        _ePortaDBContext.EMPVEHICLEMASTER.Add(vehicle);
                        _ePortaDBContext.SaveChanges();

                        if (firstVehicleId == null)
                            firstVehicleId = vehicle.VEHICLEID;

                        var rcPaths = new List<string>();
                        var insPaths = new List<string>();
                        var pucPaths = new List<string>();

                        if (vehicleDetail.RCFile != null)
                        {
                            foreach (var file in vehicleDetail.RCFile)
                                rcPaths.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), "RC"));
                        }

                        if (vehicleDetail.InsuranceFile != null)
                        {
                            foreach (var file in vehicleDetail.InsuranceFile)
                                insPaths.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), "Insurance"));
                        }

                        if (vehicleDetail.PUCFile != null)
                        {
                            foreach (var file in vehicleDetail.PUCFile)
                                pucPaths.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), "PUC"));
                        }

                        var doc = new EMP_VEHICLE_DETAIL
                        {

                            VEHICLEID = vehicle.VEHICLEID,
                            VEHICLENO = vehicleDetail.VehicleNumber,
                            FUELTYPE = vehicleDetail.FuelType,
                            MODELYEAR = vehicleDetail.ModelYear,
                            VEHICLECUSTODIAN = vehicleDetail.VehicleCustodian,
                            OWNERNAME = "-",

                            RCNO = vehicleDetail.RCNumber,
                            RCVALIDTILL = vehicleDetail.RCValidTill,

                            INSURANCENO = vehicleDetail.InsuranceNumber,
                            INSURANCEVALIDTILL = vehicleDetail.InsuranceValidTill,

                            PUCNO = vehicleDetail.PUCNumber,
                            PUCVALIDTILL = vehicleDetail.PUCValidTill,

                            RCPHOTOPATH = JsonSerializer.Serialize(rcPaths),
                            INSURANCEPHOTOPATH = JsonSerializer.Serialize(insPaths),
                            PUCPHOTOPATH = JsonSerializer.Serialize(pucPaths),

                            UPLOADEDBY = userId,
                            UPLOADEDON = DateTime.Now
                        };

                        _ePortaDBContext.EMP_VEHICLE_DETAIL.Add(doc);

                        var vehicleHistory = new EMP_VEHICLE_HISTORY()
                        {
                            VEHICLEID = vehicle.VEHICLEID,
                            ACTIONBY = userId,
                            ACTIONBYROLE = ((int)ePortal.DomainClasses.VehicleActionByRole.User).ToString(),
                            ADMIN_STATUS = (int)VehicleStatus.Pending,
                            ADMINREMARK = "Vehicle request created",
                            SECURITY_STATUS = (int)VehicleStatus.Pending,
                            SPREMARK = "Vehicle request created",
                            ACTIONDATE = DateTime.Now
                        };
                        _ePortaDBContext.EMP_VEHICLE_HISTORY.Add(vehicleHistory);


                        _ePortaDBContext.SaveChanges();
                    }
                    transaction.Commit();

                    return new VehicleResponseDto(true, "Vehicle created successfully.", firstVehicleId);
                }
                else
                {
                    var existingCount = _ePortaDBContext.EMPVEHICLEMASTER.Count(x => x.EMPCODE == dto.EmpCode &&
                                        x.STATUS == 1 &&
                                        x.VEHICLE_TYPE == dto.VehicleType &&
                                        x.VEHICLE_CATEGORY == dto.VehicleCategory);



                    if (existingCount >= 2)
                    {
                        var vehicleName = dto.VehicleType == 1 ? "Four Wheeler" : "Two Wheeler";

                        return new VehicleResponseDto(
                            false,
                            $"You already have an active {vehicleName} request. Please remove the existing request before submitting a new one."
                        );
                    }


                    var dlPaths = new List<string>();

                    if (dto.DLFiles != null)
                    {
                        foreach (var file in dto.DLFiles)
                            dlPaths.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), "DL"));
                    }

                    var vehicle = new EMP_VEHICLE_MASTER
                    {
                        EMPCODE = dto.EmpCode,
                        CREATED_DATE = DateTime.Now,
                        LOCATION_ID = dto.LocationId,
                        STATUS = 1,
                        DRIVER_NAME = dto.DriverName ?? string.Empty,
                        VEHICLE_TYPE = dto.VehicleType,
                        VEHICLE_CATEGORY = dto.VehicleCategory,
                        DL_NUMBER = dto.DLNumber,
                        DL_VALID_TILL = dto.DLValidTill,
                        DL_TYPE = dto.DLType,
                        DL_PHOTOPATH = dlPaths.Count > 0 ? JsonSerializer.Serialize(dlPaths) : null
                    };

                    _ePortaDBContext.EMPVEHICLEMASTER.Add(vehicle);
                    _ePortaDBContext.SaveChanges();

                    foreach (var item in dto.Vehicles)
                    {
                        var rcPaths = new List<string>();
                        var insPaths = new List<string>();
                        var pucPaths = new List<string>();

                        if (item.RCFile != null)
                        {
                            foreach (var file in item.RCFile)
                                rcPaths.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), "RC"));
                        }

                        if (item.InsuranceFile != null)
                        {
                            foreach (var file in item.InsuranceFile)
                                insPaths.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), "Insurance"));
                        }

                        if (item.PUCFile != null)
                        {
                            foreach (var file in item.PUCFile)
                                pucPaths.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), "PUC"));
                        }

                        var empdetail = GetEmployeeDetailsByEmpCode(Convert.ToInt64(vehicle.EMPCODE));

                        var doc = new EMP_VEHICLE_DETAIL
                        {
                            VEHICLEID = vehicle.VEHICLEID,
                            VEHICLENO = item.VehicleNumber,
                            FUELTYPE = item.FuelType,
                            MODELYEAR = item.ModelYear,
                            OWNERTYPE = item.OwnerType,
                            OWNERNAME = (item.OwnerType == 1 ? empdetail.FullName : item.OwnerName),
                            RCNO = item.RCNumber,
                            RCVALIDTILL = item.RCValidTill,

                            INSURANCENO = item.InsuranceNumber,
                            INSURANCEVALIDTILL = item.InsuranceValidTill,

                            PUCNO = item.PUCNumber,
                            PUCVALIDTILL = item.PUCValidTill,

                            RCPHOTOPATH = JsonSerializer.Serialize(rcPaths),
                            INSURANCEPHOTOPATH = JsonSerializer.Serialize(insPaths),
                            PUCPHOTOPATH = JsonSerializer.Serialize(pucPaths),

                            UPLOADEDBY = dto.EmpCode,
                            UPLOADEDON = DateTime.Now
                        };

                        _ePortaDBContext.EMP_VEHICLE_DETAIL.Add(doc);
                    }

                    var vehicleHistory = new EMP_VEHICLE_HISTORY()
                    {
                        VEHICLEID = vehicle.VEHICLEID,
                        ACTIONBY = userId,
                        ACTIONBYROLE = ((int)ePortal.DomainClasses.VehicleActionByRole.User).ToString(),
                        ADMIN_STATUS = (int)VehicleStatus.Pending,
                        ADMINREMARK = "Vehicle request created",
                        SECURITY_STATUS = (int)VehicleStatus.Pending,
                        SPREMARK = "Vehicle request created",
                        ACTIONDATE = DateTime.Now
                    };
                    _ePortaDBContext.EMP_VEHICLE_HISTORY.Add(vehicleHistory);
                    _ePortaDBContext.SaveChanges();
                    transaction.Commit();

                    return new VehicleResponseDto(true, "Vehicle created successfully.", vehicle.VEHICLEID);
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return new VehicleResponseDto(false,
                    "Internal server error. Please try again later.");
            }
        }

        private static string Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim().ToUpper().Replace(" ", "");
        }

        public bool VehicleNumberExists(string vehicleNo, int? excludeVehicleId = null)
        {
            if (string.IsNullOrWhiteSpace(vehicleNo))
                return false;

            var norm = Normalize(vehicleNo);

            return _ePortaDBContext.EMP_VEHICLE_DETAIL
                .Count(x =>
                    x.VEHICLENO == norm &&
                    (!excludeVehicleId.HasValue || x.VEHICLEID != excludeVehicleId.Value)
                ) > 0;
        }



        public bool DLNumberExists(string dlNo, string employeeCode, int? excludeVehicleId = null)
        {
            if (string.IsNullOrWhiteSpace(dlNo))
                return false;

            var norm = Normalize(dlNo);
       
            return _ePortaDBContext.EMPVEHICLEMASTER
                .Count(x =>
                    x.DL_NUMBER == norm &&
                    x.STATUS == 1 &&
                    x.EMPCODE != employeeCode &&
                    (!excludeVehicleId.HasValue || x.VEHICLEID != excludeVehicleId.Value)
                ) > 0;
        }


        public bool RCNumberExists(string rcNo, int? excludeVehicleId = null)
        {
            if (string.IsNullOrWhiteSpace(rcNo))
                return false;

            var norm = Normalize(rcNo);

            return _ePortaDBContext.EMP_VEHICLE_DETAIL
                .Count(x =>
                    x.RCNO == norm &&
                    (!excludeVehicleId.HasValue || x.VEHICLEID != excludeVehicleId.Value)
                ) > 0;
        }

        public bool InsuranceNumberExists(string insNo, int? excludeVehicleId = null)
        {
            if (string.IsNullOrWhiteSpace(insNo))
                return false;

            var norm = Normalize(insNo);

            return _ePortaDBContext.EMP_VEHICLE_DETAIL
                .Count(x =>
                    x.INSURANCENO == norm &&
                    (!excludeVehicleId.HasValue || x.VEHICLEID != excludeVehicleId.Value)
                ) > 0;
        }


        public bool PUCNumberExists(string pucNo, int? excludeVehicleId = null)
        {
            if (string.IsNullOrWhiteSpace(pucNo))
                return false;

            var norm = Normalize(pucNo);

            return _ePortaDBContext.EMP_VEHICLE_DETAIL
                .Count(x =>
                    x.PUCNO == norm &&
                    (!excludeVehicleId.HasValue || x.VEHICLEID != excludeVehicleId.Value)
                ) > 0;
        }



        public VehicleResponseDto DeleteVehicle(int vehicleId)
        {
            using var transaction = _ePortaDBContext.Database.BeginTransaction();

            try
            {
                var vehicle = _ePortaDBContext.EMPVEHICLEMASTER
                    .FirstOrDefault(x => x.VEHICLEID == vehicleId);

                if (vehicle == null)
                    return new VehicleResponseDto(false, "Vehicle not found.");

                var zone = _ePortaDBContext.PARKING_ZONE
                    .FirstOrDefault(z => z.ID == vehicle.PARKING_ZONE_ID);

                if (zone != null)
                    zone.AVAILABLE_CAPACITY += 1;

                var histories = _ePortaDBContext.EMP_VEHICLE_HISTORY
                    .Where(h => h.VEHICLEID == vehicleId)
                    .ToList();

                if (histories.Count > 0)
                    _ePortaDBContext.EMP_VEHICLE_HISTORY.RemoveRange(histories);

                var details = _ePortaDBContext.EMP_VEHICLE_DETAIL
                    .Where(d => d.VEHICLEID == vehicleId)
                    .ToList();

                if (details.Count > 0)
                    _ePortaDBContext.EMP_VEHICLE_DETAIL.RemoveRange(details);

                _ePortaDBContext.EMPVEHICLEMASTER.Remove(vehicle);

                _ePortaDBContext.SaveChanges();
                transaction.Commit();

                return new VehicleResponseDto(true, "Vehicle deleted successfully.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new VehicleResponseDto(false, "Error while deleting vehicle: " + ex.Message);
            }
        }
        public string SaveItemPhoto(IFormFile photo, string fileName, string folderName)
        {
            if (photo == null || photo.Length == 0)
                return string.Empty;

            var basePath = _configurations.GetGeneralSettings().Get_FileUpload_Path;

            var uploadsFolder = Path.Combine(basePath, "Vehicle_Portal", folderName);
            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(photo.FileName);
            var uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            photo.CopyTo(stream);

            return $"Vehicle_Portal/{folderName}/{uniqueFileName}";
        }

        public EmployeeDetailDto GetEmployeeDetailsByEmpCode(long empCode)
        {
            var query = _ePortaDBContext.VW_ASSOCIATELVLDETAILS
             .Join(_ePortaDBContext.ADEMPLOYEE,
                   vw => vw.ADEMPCODE,
                   emp => emp.ADEMPCODE,
                   (vw, emp) => new { vw, emp })
             .Join(_ePortaDBContext.ADDESIGNATION,
                   t => t.vw.ADDESIGNATIONID,
                   desig => desig.ADDESIGNATIONID,
                   (t, desig) => new { t.vw, t.emp, desig })
              .Join(_ePortaDBContext.SYSITE,
              t => t.vw.SYSITEID,
              site => site.SYSITEID,
              (t, site) => new { t.vw, t.emp, t.desig, site })
             .Where(x => x.vw.ADEMPCODE == empCode && x.vw.SYKI == _Syki.SYKIID)
             .Select(x => new
             {
                 EmpCode = x.emp.ADEMPCODE,
                 MobileNumber = x.emp.TMOBILE,
                 FullName = x.emp.FIRSTNAME + " " + x.emp.LASTNAME,
                 DepartmentName = x.vw.DEPARTMENT,
                 Designation = x.desig.DESCRIP,
                 LocationId = x.vw.SYSITEID,
                 LocationName = x.site.DESCRIP
             })
             .FirstOrDefault();


            if (query == null)
            {
                return new EmployeeDetailDto();
            }
            EmployeeDetailDto empdetail = new EmployeeDetailDto()
            {
                EmpCode = query.EmpCode,
                Designation = query.Designation,
                FullName = query.FullName,
                MobileNumber = query.MobileNumber ?? "",
                Department = query.DepartmentName ?? "",
                LocationId = query.LocationId,
                LocationName = query.LocationName,

            };
            return empdetail;
        }
        public List<LocationDto> GetSiteList()
        {
            var iList = (from data in _ePortaDBContext.SYSITE
                         where data.ACTIVE == 1
                         select new LocationDto
                         {
                             Id = data.SYSITEID,
                             Value = data.SYSITEID,
                             Text = data.DESCRIP,
                         }).OrderBy(o => o.Text).ToList();
            return iList;
        }


        public VehicleMasterDto GetVehicleDetailById(int vehicleId)
        {
            var vehicle = _ePortaDBContext.EMPVEHICLEMASTER
                .Where(v => v.VEHICLEID == vehicleId)
                .Select(v => new VehicleMasterDto
                {
                    VehicleId = v.VEHICLEID,
                    EmpCode = v.EMPCODE,
                    Status = v.STATUS,
                    AdminActionBy = v.ADMIN_ACTION_BY,
                    AdminActionDate = v.ADMIN_ACTION_DATE,
                    VehicleType = v.VEHICLE_TYPE,
                    VehicleCategory = v.VEHICLE_CATEGORY,
                    SecurityActionBy = v.SECURITY_ACTION_BY,
                    SecurityActionDate = v.SECURITY_ACTION_DATE,
                    DriverName = v.DRIVER_NAME,
                    CreatedDate = v.CREATED_DATE,
                    LocationId = v.LOCATION_ID,                
                    EmpName = string.Empty,
                    DlNumber = v.DL_NUMBER,
                    DlValidTill = v.DL_VALID_TILL,
                    DlPhotoPath = NormalizePathList(v.DL_PHOTOPATH),
                    DlType = v.DL_TYPE,
                })
                .FirstOrDefault();

            if (vehicle == null)
                return new VehicleMasterDto();

            var vehicleDetails = _ePortaDBContext.EMP_VEHICLE_DETAIL
                .Where(d => d.VEHICLEID == vehicleId)
                .Select(d => new EMPVehicleDetailDto
                {
                    Id = d.ID,
                    VehicleId = d.VEHICLEID,
                    VehicleNo = d.VEHICLENO,
                    FuelType = d.FUELTYPE,
                    ModelYear = d.MODELYEAR,
                    OwnerType = d.OWNERTYPE,
                    OwnerName = d.OWNERNAME,
                    RcNo = d.RCNO,
                    RcValidTill = d.RCVALIDTILL,
                    RcPhotoPath = NormalizePathList(d.RCPHOTOPATH),

                    InsuranceNo = d.INSURANCENO,
                    InsuranceValidTill = d.INSURANCEVALIDTILL,
                    InsurancePhotoPath = NormalizePathList(d.INSURANCEPHOTOPATH),

                    PucNo = d.PUCNO,
                    PucValidTill = d.PUCVALIDTILL,
                    PucPhotoPath = NormalizePathList(d.PUCPHOTOPATH),

                    UploadedBy = d.UPLOADEDBY,
                    UploadedOn = d.UPLOADEDON,
                    VehicleCustodian = d.VEHICLECUSTODIAN,
                    CustodianDepartment = "",
                    CustodianDesignation = "",
                    CustodianEmpName = "",
                })
                .ToList();

            vehicle.VehicleDetail = vehicleDetails;

            var data = _ePortaDBContext.EMP_VEHICLE_HISTORY.Where(x => x.VEHICLEID == vehicleId).OrderBy(x => x.ID).Select(x => new VehicleHistoryDto
            {
                Id = x.ID,
                VehicleId = x.VEHICLEID,
                ActionBy = x.ACTIONBY,
                ActionByRole = x.ACTIONBYROLE,
                ActionDate = x.ACTIONDATE,
                AdminStatus = x.ADMIN_STATUS,
                AdminRemark = x.ADMINREMARK,
                SendBackFor = x.SENDBACK_FOR,
                SecurityStatus = x.SECURITY_STATUS,
                SpRemark = x.SPREMARK,
                ParkingZoneId = x.PARKINGZONEID,
            }).ToList();

            foreach (var item in data)
            {
                if (long.TryParse(item.ActionBy, out long empCode))
                {
                    var emp = GetEmployeeDetailsByEmpCode(empCode);
                    item.EmployeeName = emp != null
                        ? $"{emp.EmpCode} - {emp.FullName}"
                        : item.ActionBy;
                }
                else
                {
                    item.EmployeeName = item.ActionBy;
                }
            }

            vehicle.VehicleHistory = data;

            foreach (var v in vehicleDetails)
            {
                if (long.TryParse(v.VehicleCustodian, out var empCode))
                {
                    var emp = GetEmployeeDetailsByEmpCode(empCode);

                    v.CustodianDepartment = emp.Department;
                    v.CustodianDesignation = emp.Designation;
                    v.CustodianEmpName = emp.FullName;
                   
                }
            }

            // EMP DETAILS
            if (long.TryParse(vehicle.EmpCode, out long empCodeLong))
            {
                var empDetails = GetEmployeeDetailsByEmpCode(empCodeLong);
                if (empDetails != null)
                {
                    vehicle.EmpName = empDetails.FullName;
                    vehicle.Department = empDetails.Department;
                    vehicle.Designation = empDetails.Designation;
                    vehicle.MobileNumber = empDetails.MobileNumber;
                    vehicle.LocationName = empDetails.LocationName;
                }
            }

            return vehicle;
        }
        private static List<string> NormalizePathList(string? rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
                return new List<string>();

            rawPath = rawPath.Trim();

            // Case 1: Already JSON array → ["a","b"]
            if (rawPath.StartsWith("[") && rawPath.EndsWith("]"))
            {
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<string>>(rawPath)
                           ?? new List<string>();
                }
                catch
                {
                    // JSON corrupt ho → treat as single path
                    return new List<string> { rawPath };
                }
            }

            // Case 2: single path string
            return new List<string> { rawPath };
        }
        public List<EmployeeDetailDto> GetEmployeeDetailsByEmpCodes(List<long> empCodes)
        {
            return _ePortaDBContext.ADEMPLOYEE
                .Where(e => empCodes.Contains(e.ADEMPCODE))
                .Select(e => new EmployeeDetailDto
                {
                    EmpCode = e.ADEMPCODE,
                    FullName = e.FIRSTNAME + " " + e.LASTNAME,
                    MobileNumber = e.TMOBILE
                })
                .ToList();
        }

        public VehicleResponseDto EditVehicleWithDocs(VehicleUpsertDto dto, string userId)
        {
            using var transaction = _ePortaDBContext.Database.BeginTransaction();
            try
            {
                if (dto.VehicleId == 0)
                    return new VehicleResponseDto(false, "Invalid Vehicle Id");

                var master = _ePortaDBContext.EMPVEHICLEMASTER
                    .FirstOrDefault(x => x.VEHICLEID == dto.VehicleId);

                if (master == null)
                    return new VehicleResponseDto(false, "Vehicle not found");

                if (dto.Vehicles == null || dto.Vehicles.Count == 0)
                    return new VehicleResponseDto(false, "Vehicle list missing");

                var v0 = dto.Vehicles[0]; // always first detail row used for master update

                /* --------------------------------------------------------
                     UPDATE MASTER (DL depends on category)
                -------------------------------------------------------- */
                if (master.VEHICLE_CATEGORY == 3)
                {
                    master.DL_NUMBER = v0.DLNumber;
                    master.DL_VALID_TILL = v0.DLValidTill;
                    master.DL_TYPE = v0.DLType;
                    master.DL_PHOTOPATH = BuildDLPhotoPath(
                        v0.DLFilePaths,
                        v0.DLFiles,
                        master.DL_PHOTOPATH
                    );
                }
                else
                {
                    master.DL_NUMBER = dto.DLNumber;
                    master.DL_VALID_TILL = dto.DLValidTill;
                    master.DL_TYPE = dto.DLType;
                    master.DRIVER_NAME = dto.DriverName ?? string.Empty;
                    master.DL_PHOTOPATH = BuildDLPhotoPath(
                        dto.DLFilePaths,
                        dto.DLFiles,
                        master.DL_PHOTOPATH
                    );
                }

                _ePortaDBContext.SaveChanges();

                /* --------------------------------------------------------
                    LOAD EXISTING DETAIL ROWS
                -------------------------------------------------------- */
                var oldDetails = _ePortaDBContext.EMP_VEHICLE_DETAIL
                    .Where(x => x.VEHICLEID == master.VEHICLEID)
                    .ToList();  

                /* --------------------------------------------------------
                     CATEGORY 3 
                -------------------------------------------------------- */
                if (master.VEHICLE_CATEGORY == 3)
                {
                    var existing = oldDetails.FirstOrDefault();
                    if (existing != null)
                        UpdateDetail(existing, v0);

                    _ePortaDBContext.SaveChanges();

                    // create new requests for remaining rows
                    for (int i = 1; i < dto.Vehicles.Count; i++)
                        CreateNewMasterAndDetail(dto, dto.Vehicles[i], userId);

                    /* --------------------------------------------------------
                        HISTORY ENTRY 
                    -------------------------------------------------------- */
                    var lasthistory = _ePortaDBContext.EMP_VEHICLE_HISTORY
                        .OrderByDescending(x => x.ID)
                        .FirstOrDefault(x => x.VEHICLEID == dto.VehicleId);

                    if (lasthistory != null && lasthistory.ADMIN_STATUS != (int)VehicleStatus.Pending && lasthistory.SECURITY_STATUS == (int)VehicleStatus.SendBack)
                    {
                        var vehicleHistory = new EMP_VEHICLE_HISTORY()
                        {
                            VEHICLEID = lasthistory.VEHICLEID,
                            ACTIONBY = lasthistory.ACTIONBY,
                            ACTIONBYROLE = ((int)ePortal.DomainClasses.VehicleActionByRole.User).ToString(),
                            ADMIN_STATUS = lasthistory.ADMIN_STATUS,
                            ADMINREMARK = "Request updated by user after send back",
                            SECURITY_STATUS = (int)VehicleStatus.Pending,
                            SPREMARK = "Request updated by user after send back",
                            SENDBACK_FOR = lasthistory.SENDBACK_FOR,
                            PARKINGZONEID = lasthistory.PARKINGZONEID,
                            ACTIONDATE = DateTime.Now
                        };
                        _ePortaDBContext.EMP_VEHICLE_HISTORY.Add(vehicleHistory);
                        _ePortaDBContext.SaveChanges();
                    }

                    transaction.Commit();
                    return new VehicleResponseDto(true, "Vehicle updated successfully", master.VEHICLEID);
                }

                /* --------------------------------------------------------
                    NON-CATEGORY-3
                -------------------------------------------------------- */
                var oldDetailDict = oldDetails.ToDictionary(x => x.ID, x => x);

                foreach (var v in dto.Vehicles)
                {
                    if (v.Id > 0 && oldDetailDict.TryGetValue(v.Id, out var existingDetail))
                    {
                        // Update existing row (Id matched from frontend)
                        UpdateDetail(existingDetail, v);
                        oldDetailDict.Remove(v.Id);  // Mark as processed
                    }
                    else
                    {
                        var newDet = BuildDetail(master.VEHICLEID, v, userId);
                        _ePortaDBContext.EMP_VEHICLE_DETAIL.Add(newDet);
                    }
                }


                foreach (var remaining in oldDetailDict.Values)
                {
                    _ePortaDBContext.EMP_VEHICLE_DETAIL.Remove(remaining);
                }

                /* --------------------------------------------------------
                    HISTORY ENTRY 
                -------------------------------------------------------- */
                var lastVehiclehistory = _ePortaDBContext.EMP_VEHICLE_HISTORY
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault(x => x.VEHICLEID == dto.VehicleId);

                if (lastVehiclehistory != null && lastVehiclehistory.ADMIN_STATUS != (int)VehicleStatus.Pending && lastVehiclehistory.SECURITY_STATUS == (int)VehicleStatus.SendBack)
                {
                    var vehicleHistory = new EMP_VEHICLE_HISTORY()
                    {
                        VEHICLEID = lastVehiclehistory.VEHICLEID,
                        ACTIONBY = lastVehiclehistory.ACTIONBY,
                        ACTIONBYROLE = ((int)ePortal.DomainClasses.VehicleActionByRole.User).ToString(),
                        ADMIN_STATUS = lastVehiclehistory.ADMIN_STATUS,
                        ADMINREMARK = "Request updated by user after send back",
                        SECURITY_STATUS = (int)VehicleStatus.Pending,
                        SPREMARK = "Request updated by user after send back",
                        SENDBACK_FOR = lastVehiclehistory.SENDBACK_FOR,
                        PARKINGZONEID = lastVehiclehistory.PARKINGZONEID,
                        ACTIONDATE = DateTime.Now
                    };
                    _ePortaDBContext.EMP_VEHICLE_HISTORY.Add(vehicleHistory);
                }

                _ePortaDBContext.SaveChanges();
                transaction.Commit();

                return new VehicleResponseDto(true, "Vehicle updated successfully", master.VEHICLEID);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new VehicleResponseDto(false, "Internal server error. Please try again later.");
            }
        }


        private string BuildDLPhotoPath(List<string> keptPaths, List<IFormFile> newFiles, string oldDbValue)
        {
            var final = new List<string>();

            // KEEP OLD FILES SELECTED BY USER
            if (keptPaths != null && keptPaths.Count > 0)
                final.AddRange(keptPaths);

            // ADD NEW FILES
            if (newFiles != null && newFiles.Count > 0)
            {
                foreach (var file in newFiles)
                    final.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), "DL"));
            }

            // If nothing updated → keep old DL_PHOTOPATH
            if (final.Count == 0 && !string.IsNullOrEmpty(oldDbValue))
                return oldDbValue;

            return final.Count > 0 ? JsonSerializer.Serialize(final) : null;
        }


        private void UpdateDetail(EMP_VEHICLE_DETAIL detail, VehicleDetailDto v)
        {
            detail.VEHICLENO = v.VehicleNumber;
            detail.FUELTYPE = v.FuelType;
            detail.MODELYEAR = v.ModelYear;
            detail.OWNERTYPE = v.OwnerType;
            detail.OWNERNAME = v.OwnerName;
            detail.VEHICLECUSTODIAN = v.VehicleCustodian;


            // RC
            var keptRC = GetKeptPaths(v.RCFilePaths);
            var rcFinal = AppendNewPaths(keptRC, v.RCFile, "RC");
            DeleteRemovedFiles(detail.RCPHOTOPATH, rcFinal);
            detail.RCPHOTOPATH = JsonSerializer.Serialize(rcFinal);
            detail.RCNO = v.RCNumber;
            detail.RCVALIDTILL = v.RCValidTill;

            // Insurance
            var keptIns = GetKeptPaths(v.InsuranceFilePaths);
            var insFinal = AppendNewPaths(keptIns, v.InsuranceFile, "Insurance");
            DeleteRemovedFiles(detail.INSURANCEPHOTOPATH, insFinal);
            detail.INSURANCEPHOTOPATH = JsonSerializer.Serialize(insFinal);
            detail.INSURANCENO = v.InsuranceNumber;
            detail.INSURANCEVALIDTILL = v.InsuranceValidTill;

            // PUC
            var keptPuc = GetKeptPaths(v.PUCFilePaths);
            var pucFinal = AppendNewPaths(keptPuc, v.PUCFile, "PUC");
            DeleteRemovedFiles(detail.PUCPHOTOPATH, pucFinal);
            detail.PUCPHOTOPATH = JsonSerializer.Serialize(pucFinal);
            detail.PUCNO = v.PUCNumber;
            detail.PUCVALIDTILL = v.PUCValidTill;
            detail.UPLOADEDON = DateTime.Now;
        }

        private EMP_VEHICLE_DETAIL BuildDetail(int vehicleId, VehicleDetailDto v, string userId)
        {
            var rc = BuildFileList(v.RCFile, "RC");
            var ins = BuildFileList(v.InsuranceFile, "Insurance");
            var puc = BuildFileList(v.PUCFile, "PUC");

            return new EMP_VEHICLE_DETAIL
            {
                VEHICLEID = vehicleId,
                VEHICLENO = v.VehicleNumber,
                FUELTYPE = v.FuelType,
                MODELYEAR = v.ModelYear,
                OWNERTYPE = v.OwnerType,
                OWNERNAME = v.OwnerName,
                VEHICLECUSTODIAN = v.VehicleCustodian,

                RCNO = v.RCNumber,
                RCVALIDTILL = v.RCValidTill,

                INSURANCENO = v.InsuranceNumber,
                INSURANCEVALIDTILL = v.InsuranceValidTill,

                PUCNO = v.PUCNumber,
                PUCVALIDTILL = v.PUCValidTill,

                RCPHOTOPATH = JsonSerializer.Serialize(rc),
                INSURANCEPHOTOPATH = JsonSerializer.Serialize(ins),
                PUCPHOTOPATH = JsonSerializer.Serialize(puc),

                UPLOADEDBY = userId,
                UPLOADEDON = DateTime.Now
            };
        }

        private List<string> BuildFileList(List<IFormFile>? files, string folder)
        {
            var list = new List<string>();

            if (files != null)
            {
                foreach (var file in files)
                    list.Add(SaveItemPhoto(file, Path.GetFileName(file.FileName), folder));
            }

            return list;
        }


        private void CreateNewMasterAndDetail(VehicleUpsertDto dto, VehicleDetailDto v, string userId)
        {
            var newMaster = new EMP_VEHICLE_MASTER
            {
                EMPCODE = dto.EmpCode,
                CREATED_DATE = DateTime.Now,
                LOCATION_ID = dto.LocationId,
                STATUS = 0,
                VEHICLE_TYPE = dto.VehicleType,
                VEHICLE_CATEGORY = dto.VehicleCategory,

                DL_NUMBER = v.DLNumber,
                DL_VALID_TILL = v.DLValidTill,
                DL_TYPE = v.DLType,

                DL_PHOTOPATH = BuildDLPhotoPath(
                    v.DLFilePaths,
                    v.DLFiles,
                    null
                ),

            };

            _ePortaDBContext.EMPVEHICLEMASTER.Add(newMaster);
            _ePortaDBContext.SaveChanges();

            var newDetail = BuildDetail(newMaster.VEHICLEID, v, userId);
            _ePortaDBContext.EMP_VEHICLE_DETAIL.Add(newDetail);
            _ePortaDBContext.SaveChanges();
        }

        private List<string> GetKeptPaths(List<string>? existingPaths)
        {
            if (existingPaths == null || existingPaths.Count == 0)
                return new List<string>();

            return existingPaths
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        private List<string> AppendNewPaths(List<string> keptPaths, List<IFormFile>? newFiles, string folderName)
        {
            if (newFiles != null)
            {
                foreach (var file in newFiles)
                {
                    var newPath = SaveItemPhoto(file, Path.GetFileName(file.FileName), folderName);
                    if (!string.IsNullOrEmpty(newPath))
                    {
                        if (!keptPaths.Contains(newPath, StringComparer.OrdinalIgnoreCase))
                            keptPaths.Add(newPath);
                    }
                }
            }

            return keptPaths;
        }

        private void DeleteRemovedFiles(string? jsonOldPaths, List<string> finalPaths)
        {
            // Old paths from DB (JSON)
            var oldList = new List<string>();

            if (!string.IsNullOrEmpty(jsonOldPaths))
            {
                try
                {
                    oldList = JsonSerializer.Deserialize<List<string>>(jsonOldPaths) ?? new List<string>();
                }
                catch
                {
                    oldList = new List<string>();
                }
            }

            // Find deleted files
            var removed = oldList
                .Where(x => !finalPaths.Contains(x, StringComparer.OrdinalIgnoreCase))
                .ToList();

            foreach (var p in removed)
            {
                var full = Path.Combine("wwwroot", p.Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(full))
                    File.Delete(full);
            }
        }

        public VehicleResponseDto MyVehicleRequests(long empCode)
        {
            try
            {
                var vehicleRequests = _ePortaDBContext.EMPVEHICLEMASTER
                    .Where(v => v.EMPCODE == empCode.ToString()).OrderByDescending(x => x.CREATED_DATE)
                    .Select(v => new VehicleMasterDto
                    {
                        VehicleId = v.VEHICLEID,
                        EmpCode = v.EMPCODE,
                        Status = v.STATUS,
                        CreatedDate = v.CREATED_DATE,
                        VehicleCategory = v.VEHICLE_CATEGORY,
                        ParkingZoneId = v.PARKING_ZONE_ID,
                        VehicleType = v.VEHICLE_TYPE,
                    })
                    .ToList();
                var history = _ePortaDBContext.EMP_VEHICLE_HISTORY
                    .Where(h => vehicleRequests.Select(v => v.VehicleId).Contains(h.VEHICLEID))
                    .ToList();

                if (history == null || history.Count == 0)
                {
                    vehicleRequests.ForEach(v =>
                    {
                        v.VehicleHistory = new List<VehicleHistoryDto>();
                    });
                }
                else
                {
                    vehicleRequests.ForEach(v =>
                    {
                        v.VehicleHistory = history
                            .Where(h => h.VEHICLEID == v.VehicleId)
                            .Select(h => new VehicleHistoryDto
                            {
                                Id = h.ID,
                                VehicleId = h.VEHICLEID,
                                ActionBy = h.ACTIONBY,
                                ActionByRole = h.ACTIONBYROLE,
                                ActionDate = h.ACTIONDATE,
                                ParkingZoneId = h.PARKINGZONEID,
                                AdminRemark = h.ADMINREMARK,
                                SendBackFor = h.SENDBACK_FOR,
                                AdminStatus = h.ADMIN_STATUS,
                                SecurityStatus = h.SECURITY_STATUS,
                                SpRemark = h.SPREMARK,
                            })
                            .ToList();
                    });
                }

                var zoneIds = vehicleRequests
                    .Where(x => x.ParkingZoneId.HasValue)
                    .Select(x => x.ParkingZoneId.Value)
                    .Distinct()
                    .ToList();

                var zoneMap = _ePortaDBContext.PARKING_ZONE
                    .Where(z => zoneIds.Contains(z.ID))
                    .ToDictionary(z => z.ID, z => z);

                foreach (var v in vehicleRequests)
                {
                    if (v.ParkingZoneId.HasValue && zoneMap.TryGetValue(v.ParkingZoneId.Value, out var zone))
                        v.ParkingZone = zone;

                    if (long.TryParse(v.EmpCode, out long empCodeLong))
                    {
                        var empDetails = GetEmployeeDetailsByEmpCode(empCodeLong);
                        if (empDetails != null)
                        {
                            v.EmpName = empDetails.FullName;
                            v.Department = empDetails.Department;
                            v.Designation = empDetails.Designation;
                            v.MobileNumber = empDetails.MobileNumber;
                        }
                    }
                }
                return new VehicleResponseDto(true, "Vehicle requests retrieved successfully", vehicleRequests);
            }
            catch (Exception ex)
            {
                return new VehicleResponseDto(false, "Error while retrieving vehicle requests: " + ex.Message);
            }
        }

        public List<VehicleMasterDto> GetVehicleDetailsBySiteId(long userSiteId)
        {
            var vehicles = _ePortaDBContext.EMPVEHICLEMASTER
                .Where(v => v.LOCATION_ID == userSiteId && v.STATUS == 1).OrderByDescending(x => x.CREATED_DATE)
                .Select(v => new VehicleMasterDto
                {
                    VehicleId = v.VEHICLEID,
                    EmpCode = v.EMPCODE,
                    Status = v.STATUS,
                    AdminActionBy = v.ADMIN_ACTION_BY,
                    AdminActionDate = v.ADMIN_ACTION_DATE,
                    VehicleType = v.VEHICLE_TYPE,
                    VehicleCategory = v.VEHICLE_CATEGORY,
                    SecurityActionBy = v.SECURITY_ACTION_BY,
                    SecurityActionDate = v.SECURITY_ACTION_DATE,
                    CreatedDate = v.CREATED_DATE,
                    LocationId = v.LOCATION_ID,
                    EmpName = string.Empty,
                    DlNumber = v.DL_NUMBER,
                    DlValidTill = v.DL_VALID_TILL,
                    DlPhotoPath = NormalizePathList(v.DL_PHOTOPATH),
                    DlType = v.DL_TYPE,
                    ParkingZoneId = v.PARKING_ZONE_ID,
                })
                .ToList();

            var siteMap = GetSiteList().ToDictionary(x => x.Id, x => x);
            foreach (var v in vehicles)
            {
                if (siteMap.TryGetValue(v.LocationId, out var loc))
                    v.Location = loc;
            }

            var zoneIds = vehicles
                .Where(x => x.ParkingZoneId.HasValue)
                .Select(x => x.ParkingZoneId.Value)
                .Distinct()
                .ToList();

            var zoneMap = _ePortaDBContext.PARKING_ZONE
                .Where(z => zoneIds.Contains(z.ID))
                .ToDictionary(z => z.ID, z => z);

            foreach (var v in vehicles)
            {
                if (v.ParkingZoneId.HasValue && zoneMap.TryGetValue(v.ParkingZoneId.Value, out var zone))
                    v.ParkingZone = zone;
            }



            if (!vehicles.Any())
                return new List<VehicleMasterDto>();

            var history = _ePortaDBContext.EMP_VEHICLE_HISTORY.Where(h => vehicles.Select(v => v.VehicleId).Contains(h.VEHICLEID)).ToList();

            if (history == null || history.Count == 0)
            {
                vehicles.ForEach(v =>
                {
                    v.VehicleHistory = new List<VehicleHistoryDto>();
                });
            }
            else
            {
                vehicles.ForEach(v =>
                {
                    v.VehicleHistory = history
                        .Where(h => h.VEHICLEID == v.VehicleId)
                        .Select(h => new VehicleHistoryDto
                        {
                            Id = h.ID,
                            VehicleId = h.VEHICLEID,
                            ActionBy = h.ACTIONBY,
                            ActionByRole = h.ACTIONBYROLE,
                            ActionDate = h.ACTIONDATE,
                            ParkingZoneId = h.PARKINGZONEID,
                            AdminRemark = h.ADMINREMARK,
                            SendBackFor = h.SENDBACK_FOR,
                            AdminStatus = h.ADMIN_STATUS,
                            SecurityStatus = h.SECURITY_STATUS,
                            SpRemark = h.SPREMARK,
                        })
                        .ToList();
                });
            }


            foreach (var vehicle in vehicles)
            {
                var vehicleDetails = _ePortaDBContext.EMP_VEHICLE_DETAIL
                    .Where(d => d.VEHICLEID == vehicle.VehicleId)
                    .Select(d => new EMPVehicleDetailDto
                    {
                        Id = d.ID,
                        VehicleId = d.VEHICLEID,
                        VehicleNo = d.VEHICLENO,
                        FuelType = d.FUELTYPE,
                        ModelYear = d.MODELYEAR,
                        OwnerType = d.OWNERTYPE,
                        OwnerName = d.OWNERNAME,
                        RcNo = d.RCNO,
                        RcValidTill = d.RCVALIDTILL,
                        RcPhotoPath = NormalizePathList(d.RCPHOTOPATH),

                        InsuranceNo = d.INSURANCENO,
                        InsuranceValidTill = d.INSURANCEVALIDTILL,
                        InsurancePhotoPath = NormalizePathList(d.INSURANCEPHOTOPATH),

                        PucNo = d.PUCNO,
                        PucValidTill = d.PUCVALIDTILL,
                        PucPhotoPath = NormalizePathList(d.PUCPHOTOPATH),

                        UploadedBy = d.UPLOADEDBY,
                        UploadedOn = d.UPLOADEDON,
                        VehicleCustodian = d.VEHICLECUSTODIAN,
                        CustodianDepartment = "",
                        CustodianDesignation = "",
                        CustodianEmpName = ""
                    })
                    .ToList();

                vehicle.VehicleDetail = vehicleDetails;



                foreach (var v in vehicleDetails)
                {
                    if (long.TryParse(v.VehicleCustodian, out var empCode))
                    {
                        var emp = GetEmployeeDetailsByEmpCode(empCode);
                        if (emp != null)
                        {
                            v.CustodianDepartment = emp.Department;
                            v.CustodianDesignation = emp.Designation;
                            v.CustodianEmpName = emp.FullName;
                        }
                    }
                }

                if (long.TryParse(vehicle.EmpCode, out long empCodeLong))
                {
                    var empDetails = GetEmployeeDetailsByEmpCode(empCodeLong);
                    if (empDetails != null)
                    {
                        vehicle.EmpName = empDetails.FullName;
                        vehicle.Department = empDetails.Department;
                        vehicle.Designation = empDetails.Designation;
                        vehicle.MobileNumber = empDetails.MobileNumber;
                    }
                }
            }
            return vehicles;
        }

        public ZoneSummaryDto GetZoneSummary(long siteId)
        {
            var zones = _ePortaDBContext.PARKING_ZONE
                .Where(z => z.LOCATION_ID == siteId)
                .ToList();

            int totalZones = zones.Count;
            int zonesOccupied = zones.Count(z => (z.AVAILABLE_CAPACITY ?? 0) == 0);
            int activeZones = zones.Count(z => z.IS_ACTIVE);
            int inactiveZones = totalZones - activeZones;

            return new ZoneSummaryDto
            {
                TotalZones = totalZones,
                ZonesOccupied = zonesOccupied,
                ActiveZones = activeZones,
                InactiveZones = inactiveZones
            };
        }


        public List<ParkingZoneDto> GetZonesBySiteId(long siteId)
        {
            var zones = _ePortaDBContext.PARKING_ZONE
                .Where(z => z.LOCATION_ID == siteId)
                .ToList();
            return zones.Select(z => new ParkingZoneDto
            {
                Id = z.ID,
                ZoneName = z.ZONE_NAME,
                ZoneCode = z.ZONE_CODE,
                Description = z.DESCRIPTION,
                TotalCapacity = z.TOTAL_CAPACITY ?? 0,
                AvailableCapacity = z.AVAILABLE_CAPACITY ?? 0,
                IsActive = z.IS_ACTIVE,
                CreatedAt = z.CREATED_AT,
                UpdatedAt = z.UPDATED_AT
            }).ToList();
        }




        public LocationDto GetSiteById(int userSiteId)
        {
            var iList = (from data in _ePortaDBContext.SYSITE
                         where data.ACTIVE == 1
                         select new LocationDto
                         {
                             Id = data.SYSITEID,
                             Text = data.DESCRIP,
                         }).OrderBy(o => o.Text).Where(x => x.Id == userSiteId).FirstOrDefault();
            return iList;
        }
        public VehicleResponseDto SaveVehicleHistory(VehicleHistoryDto dto)
        {
            var history = new EMP_VEHICLE_HISTORY
            {
                VEHICLEID = dto.VehicleId,
                ACTIONBY = dto.ActionBy,
                ACTIONBYROLE = dto.ActionByRole,
                PARKINGZONEID = dto.ParkingZoneId,
                ACTIONDATE = DateTime.Now
            };

            _ePortaDBContext.EMP_VEHICLE_HISTORY.Add(history);
            _ePortaDBContext.SaveChanges();

            return new VehicleResponseDto(
                true,
                "Vehicle history created successfully.",
                history.ID
            );
        }


        public List<VehicleHistoryDto> GetVehicleHistoryById(int vehicleId)
        {
            var historyList = _ePortaDBContext.EMP_VEHICLE_HISTORY
                .Where(vh => vh.VEHICLEID == vehicleId)
                .OrderByDescending(vh => vh.ID)
                .Select(vh => new VehicleHistoryDto
                {
                    Id = vh.ID,
                    VehicleId = vh.VEHICLEID,
                    ActionBy = vh.ACTIONBY,
                    ActionDate = vh.ACTIONDATE,
                    ActionByRole = vh.ACTIONBYROLE,
                    SendBackFor = vh.SENDBACK_FOR,
                    ParkingZoneId = vh.PARKINGZONEID,
                })
                .ToList();

            foreach (var item in historyList)
            {
                item.EmpVehicleMaster = GetVehicleDetailById(item.VehicleId);

                if (long.TryParse(item.ActionBy, out long empCode))
                {
                    var emp = GetEmployeeDetailsByEmpCode(empCode);
                    item.EmployeeName = emp != null
                        ? $"{emp.EmpCode} - {emp.FullName}"
                        : item.ActionBy;
                }
                else
                {
                    item.EmployeeName = item.ActionBy;
                }
            }

            return historyList;
        }
        public List<ParkingZoneDto> GetParkingZones()
        {
            var result =
                (from zone in _ePortaDBContext.PARKING_ZONE
                 join site in _ePortaDBContext.SYSITE
                     on zone.LOCATION_ID equals (int?)site.SYSITEID into siteJoin
                 from site in siteJoin.DefaultIfEmpty()
                 select new ParkingZoneDto
                 {
                     Id = zone.ID,
                     ZoneName = zone.ZONE_NAME,
                     ZoneCode = zone.ZONE_CODE,
                     Capacity = zone.TOTAL_CAPACITY ?? 0,
                     AvailableCapacity = zone.AVAILABLE_CAPACITY ?? 0,
                     IsActive = zone.IS_ACTIVE,
                     CreatedAt = zone.CREATED_AT,
                     UpdatedAt = zone.UPDATED_AT,
                     LocationId = zone.LOCATION_ID ?? 0,
                     LocationName = site != null ? site.DESCRIP : string.Empty
                 })
                .ToList();

            return result;
        }



        public ParkingZoneDto? GetParkingZoneById(long? id)
        {
            return _ePortaDBContext.PARKING_ZONE
                .Where(x => x.ID == id)
                .Select(x => new ParkingZoneDto
                {
                    Id = x.ID,
                    ZoneName = x.ZONE_NAME,
                    ZoneCode = x.ZONE_CODE,
                    Capacity = x.TOTAL_CAPACITY ?? 0,
                    LocationId = x.LOCATION_ID ?? 0,
                    Description = x.DESCRIPTION ?? string.Empty,
                    AvailableCapacity = x.AVAILABLE_CAPACITY ?? 0,
                    IsActive = x.IS_ACTIVE,
                    CreatedAt = x.CREATED_AT,                    
                    UpdatedAt = x.UPDATED_AT
                })
                .FirstOrDefault();
        }
        public VehicleResponseDto UpsertZone(ParkingZoneDto zone)
        {
            if (zone == null)
                return new VehicleResponseDto(false, "Invalid request payload");

            try
            {
                // UPDATE
                if (zone.Id > 0)
                {
                    var existing = _ePortaDBContext.PARKING_ZONE
                        .FirstOrDefault(z => z.ID == zone.Id);

                    if (existing == null)
                        return new VehicleResponseDto(false, "Zone not found");

                    // Calculate assigned slots
                    var assigned = existing.TOTAL_CAPACITY - existing.AVAILABLE_CAPACITY;
                    if (assigned < 0) assigned = 0;

                    if (zone.Capacity < assigned)
                    {
                        return new VehicleResponseDto(
                            false,
                            $"Cannot reduce capacity below assigned vehicles ({assigned})."
                        );
                    }

                    // Update core fields
                    existing.ZONE_NAME = zone.ZoneName;
                    existing.LOCATION_ID = zone.LocationId;
                    existing.DESCRIPTION = zone.Description;
                    existing.TOTAL_CAPACITY = zone.Capacity;
                    existing.IS_ACTIVE = zone.IsActive;
                    existing.UPDATED_AT = DateTime.UtcNow;

                    // Recalculate available capacity
                    existing.AVAILABLE_CAPACITY = existing.TOTAL_CAPACITY - assigned;

                    _ePortaDBContext.SaveChanges();

                    return new VehicleResponseDto(true, "Zone updated successfully");
                }

                // INSERT
                var parkingzone = new PARKING_ZONE
                {
                    ZONE_NAME = zone.ZoneName,
                    TOTAL_CAPACITY = zone.Capacity,                    
                    DESCRIPTION = zone.Description,
                    AVAILABLE_CAPACITY = zone.Capacity,
                    IS_ACTIVE = zone.IsActive,
                    CREATED_AT = DateTime.UtcNow,
                    LOCATION_ID = (int)zone.LocationId
                };

                _ePortaDBContext.PARKING_ZONE.Add(parkingzone);
                _ePortaDBContext.SaveChanges();

                return new VehicleResponseDto(true, "Zone created successfully");
            }
            catch (Exception ex)
            {
                return new VehicleResponseDto(false, ex.Message);
            }
        }

        public VehicleResponseDto DeleteZone(int id)
        {
            var parkingZone = _ePortaDBContext.PARKING_ZONE
                .FirstOrDefault(x => x.ID == id);

            if (parkingZone == null)
            {
                return new VehicleResponseDto(
                    success: false,
                    message: "Zone not found"
                );
            }

            // Check if zone is assigned to any vehicle
            var isAssigned = _ePortaDBContext.EMPVEHICLEMASTER
                .FirstOrDefault(v => v.PARKING_ZONE_ID == id);

            if (isAssigned != null)
            {
                return new VehicleResponseDto(
                    success: false,
                    message: "Zone cannot be deleted because vehicles are assigned to it"
                );
            }

            _ePortaDBContext.PARKING_ZONE.Remove(parkingZone);
            _ePortaDBContext.SaveChanges();

            return new VehicleResponseDto(
                success: true,
                message: "Zone deleted successfully",
                data: new { Id = id }
            );
        }


        public List<ZoneStatisticsDto> ZoneStatistics(int vehicleId)
        {
            var vehiclemaster = _ePortaDBContext.EMPVEHICLEMASTER.FirstOrDefault(x => x.VEHICLEID == vehicleId && x.STATUS == 1);
            if (vehiclemaster == null)
            {
                return new List<ZoneStatisticsDto>();
            }
            var zones = _ePortaDBContext.PARKING_ZONE
                .Where(x => x.LOCATION_ID == vehiclemaster.LOCATION_ID && x.IS_ACTIVE)
                .ToList();

            var statistics = new List<ZoneStatisticsDto>();

            foreach (var item in zones)
            {
                var total = item.TOTAL_CAPACITY ?? 0;
                var available = item.AVAILABLE_CAPACITY ?? 0;

                var assigned = total - available;
                if (assigned < 0) assigned = 0;

                statistics.Add(new ZoneStatisticsDto
                {
                    Id = item.ID,
                    ZoneName = item.ZONE_NAME,
                    TotalCount = total,
                    Assigned = assigned,
                    Remaining = available
                });
            }

            return statistics;
        }

        public VehicleResponseDto ApproveRequest(ApproveRequestDto request, string userId)
        {
            if (request == null)
                return new VehicleResponseDto(false, "Invalid request");

            // Fetch vehicle
            var vehicle = _ePortaDBContext.EMPVEHICLEMASTER
                .FirstOrDefault(x => x.VEHICLEID == request.VehicleId && x.STATUS == 1);

            if (vehicle == null)
                return new VehicleResponseDto(false, "Vehicle not found");

            // Fetch zone
            var zone = _ePortaDBContext.PARKING_ZONE
                .FirstOrDefault(x => x.ID == request.ZoneId);

            if (zone == null)
                return new VehicleResponseDto(false, "Zone not found");

            // Capacity check
            if (zone.AVAILABLE_CAPACITY <= 0)
                return new VehicleResponseDto(false, "Zone capacity full");

            // Last history
            var lastHistory = _ePortaDBContext.EMP_VEHICLE_HISTORY
                .Where(x => x.VEHICLEID == request.VehicleId)
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();

            // Handle case: vehicle was send-back previously AND already had zone assigned
            if (vehicle.PARKING_ZONE_ID.HasValue &&
                vehicle.PARKING_ZONE_ID > 0 &&
                lastHistory != null &&
                lastHistory.ADMIN_STATUS == (int)VehicleStatus.SendBack &&
                lastHistory.SECURITY_STATUS == (int)VehicleStatus.SendBack)
            {
                // return capacity to previous zone
                var prevZone = _ePortaDBContext.PARKING_ZONE
                    .FirstOrDefault(z => z.ID == vehicle.PARKING_ZONE_ID);

                if (prevZone != null)
                    prevZone.AVAILABLE_CAPACITY += 1;
            }

            // If vehicle is already properly assigned and not a send-back scenario
            if (vehicle.PARKING_ZONE_ID.HasValue &&
                vehicle.PARKING_ZONE_ID > 0
                &&
                !(lastHistory?.ADMIN_STATUS == (int)VehicleStatus.SendBack &&
                  lastHistory?.SECURITY_STATUS == (int)VehicleStatus.SendBack)
                  )
            {
                return new VehicleResponseDto(false, "Vehicle already assigned to a zone");
            }

            // Assign new zone
            vehicle.ADMIN_ACTION_BY = userId;
            vehicle.ADMIN_ACTION_DATE = DateTime.Now;
            vehicle.PARKING_ZONE_ID = request.ZoneId;

            zone.AVAILABLE_CAPACITY -= 1;

            // Add new history record
            var history = new EMP_VEHICLE_HISTORY
            {
                VEHICLEID = vehicle.VEHICLEID,
                ACTIONBY = userId,
                ADMINREMARK = request.Remark,
                ADMIN_STATUS = (int)VehicleStatus.Approved,
                ACTIONBYROLE = ((int)VehicleActionByRole.Admin).ToString(),
                PARKINGZONEID = request.ZoneId,
                ACTIONDATE = DateTime.Now,
                SECURITY_STATUS = 0,
            };

            _ePortaDBContext.EMP_VEHICLE_HISTORY.Add(history);
            _ePortaDBContext.SaveChanges();

            return new VehicleResponseDto(true, "Request approved successfully");
        }

        public VehicleResponseDto DeactivateVehicle(DeactivateVehicle request, string userId)
        {
            var vehicle = _ePortaDBContext.EMPVEHICLEMASTER
                .FirstOrDefault(v => v.VEHICLEID == request.VehicleId && v.STATUS == 1);            

            if (vehicle is null)
                return new VehicleResponseDto(false, "Vehicle not found");

            var empCode = Convert.ToInt32(vehicle.EMPCODE);
            var empInfo = GetEmployeeDetailsByEmpCode(empCode);

            if(vehicle.LOCATION_ID == empInfo.LocationId)
                return new VehicleResponseDto(false, "You do not have permission to deactivate this vehicle due to location restrictions.");

            var zone = _ePortaDBContext.PARKING_ZONE
                  .FirstOrDefault(z => z.ID == vehicle.PARKING_ZONE_ID);

            if (zone != null)
            { 
                zone.AVAILABLE_CAPACITY += 1;
            }
            vehicle.PARKING_ZONE_ID = null;
            vehicle.STATUS = 0;

            var newHistory = new EMP_VEHICLE_HISTORY
            {
                VEHICLEID = vehicle.VEHICLEID,
                ACTIONBY = userId,
                PARKINGZONEID = vehicle.PARKING_ZONE_ID,
                ADMINREMARK =  request.Remark,
                ADMIN_STATUS = ((int)VehicleStatus.Deactivated),
                SECURITY_STATUS = ((int)VehicleStatus.Deactivated),
                SPREMARK = request.Remark,
                //ACTIONBYROLE = request.ActionByRole == 1 ? ((int)VehicleActionByRole.User).ToString() : ((int)VehicleActionByRole.Security).ToString(),
                ACTIONBYROLE = ((int)VehicleActionByRole.Security).ToString(),
                ACTIONDATE = DateTime.Now
            };

            _ePortaDBContext.EMP_VEHICLE_HISTORY.Add(newHistory);
            _ePortaDBContext.SaveChanges();

            return new VehicleResponseDto(true, "Request submitted successfully");
        }

        public VehicleResponseDto VehicleSecurityAction(
            VehicleSecurityActionDto request,
            string userId)
        {
            if (request is null)
                return new VehicleResponseDto(false, "Invalid request");

            var vehicle = _ePortaDBContext.EMPVEHICLEMASTER
                .FirstOrDefault(v => v.VEHICLEID == request.VehicleId && v.STATUS == 1);

            if (vehicle is null)
                return new VehicleResponseDto(false, "Vehicle not found");

            var lastHistory = _ePortaDBContext.EMP_VEHICLE_HISTORY
                .Where(h => h.VEHICLEID == request.VehicleId)
                .OrderByDescending(h => h.ID)
                .FirstOrDefault();

            if (lastHistory is null)
                return new VehicleResponseDto(false, "Vehicle history not found");

            if (lastHistory.ADMIN_STATUS != 1)
                return new VehicleResponseDto(false, "Vehicle not approved by Admin yet");

            var now = DateTime.UtcNow;

            vehicle.SECURITY_ACTION_BY = userId;
            vehicle.SECURITY_ACTION_DATE = now;


            if (request.Action == 3)
            {
                var zone = _ePortaDBContext.PARKING_ZONE
                    .FirstOrDefault(z => z.ID == vehicle.PARKING_ZONE_ID);

                if (zone is null)
                    return new VehicleResponseDto(false, "Parking zone not found");

                vehicle.STATUS = 0;
                vehicle.PARKING_ZONE_ID = null;
                zone.AVAILABLE_CAPACITY += 1;
            }

            var newHistory = new EMP_VEHICLE_HISTORY
            {
                VEHICLEID = vehicle.VEHICLEID,
                ACTIONBY = userId,
                PARKINGZONEID = vehicle.PARKING_ZONE_ID,
                ADMINREMARK = lastHistory.ADMINREMARK,
                ADMIN_STATUS = request.Sendbackfor == 2
                    ? request.Action
                    : lastHistory.ADMIN_STATUS,
                SECURITY_STATUS = request.Action,
                SPREMARK = request.Remark,
                ACTIONBYROLE = ((int)VehicleActionByRole.Security).ToString(),
                ACTIONDATE = now
            };

            if (request.Action != 1)
                newHistory.SENDBACK_FOR = request.Sendbackfor;

            _ePortaDBContext.EMP_VEHICLE_HISTORY.Add(newHistory);
            _ePortaDBContext.SaveChanges();

            return new VehicleResponseDto(true, "Request submitted successfully");
        }


        //Admin And Security Mapping
        public bool CheckExists(int siteId, int adminCode)
        {
            return _ePortaDBContext.ADMIN_LOCATION_MAPPING
        .Count(m => m.SITE_ID == siteId && m.ADMIN_CODE == adminCode) > 0;
        }
        public int AddAdmin(AdminLocationMapViewModel adminmap)
        {
            try
            {
                // Map ViewModel → Entity
                var entity = new ADMIN_LOCATION_MAPPING
                {
                    SITE_ID = adminmap.SiteId,
                    ADMIN_CODE = adminmap.AdminCode,
                    IS_ACTIVE = adminmap.IsActive,
                    CREATED_DATE = DateTime.Now
                };

                _ePortaDBContext.ADMIN_LOCATION_MAPPING.Add(entity);
                _ePortaDBContext.SaveChanges();

                return entity.ID;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving Admin Mapping: " + ex.Message);
            }
        }
        public List<EmployeeDetailDto> GetAdminsBySite()
        {
            try
            {
                var result = (
                    from emp in _ePortaDBContext.ADEMPLOYEE
                    join assoc in _ePortaDBContext.VW_ASSOCIATELVLDETAILS
                        on emp.ADEMPCODE equals assoc.ADEMPCODE
                    where emp.ACTIVE == 1
                    group emp by new
                    {
                        emp.ADEMPCODE,
                        emp.FIRSTNAME,
                        emp.LASTNAME,
                        emp.TMOBILE
                    }
                    into g
                    select new EmployeeDetailDto
                    {
                        EmpCode = g.Key.ADEMPCODE,
                        FullName = g.Key.FIRSTNAME + " " + g.Key.LASTNAME,
                        MobileNumber = g.Key.TMOBILE
                    }
                )
                .OrderBy(a => a.FullName)
                .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching admin list: " + ex.Message);
            }
        }


        public List<AdminLocationMapViewModel> GetAdminMappingList()
        {
            try
            {
                var result =
                    (from map in _ePortaDBContext.ADMIN_LOCATION_MAPPING
                     join site in _ePortaDBContext.SYSITE
                         on map.SITE_ID equals site.SYSITEID
                     join emp in _ePortaDBContext.ADEMPLOYEE
                         on map.ADMIN_CODE equals emp.ADEMPCODE
                     orderby site.DESCRIP
                     select new AdminLocationMapViewModel
                     {
                         Id = map.ID,
                         SiteId = map.SITE_ID,
                         SiteName = site.DESCRIP,
                         AdminCode = map.ADMIN_CODE,
                         AdminName = emp.FIRSTNAME + emp.LASTNAME,
                         IsActive = map.IS_ACTIVE
                     }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading Admin Mapping List: " + ex.Message);
            }
        }
        public AdminLocationMapViewModel GetAdminMappingById(int id)
        {
            return (
                from map in _ePortaDBContext.ADMIN_LOCATION_MAPPING
                where map.ID == id
                select new AdminLocationMapViewModel
                {
                    Id = map.ID,
                    SiteId = map.SITE_ID,
                    AdminCode = map.ADMIN_CODE,
                    IsActive = map.IS_ACTIVE,
                }
            ).FirstOrDefault();
        }
        public int UpdateAdminMapping(AdminLocationMapViewModel model)
        {
            var entity = _ePortaDBContext.ADMIN_LOCATION_MAPPING.FirstOrDefault(m => m.ID == model.Id);

            if (entity == null)
                return 0;

            entity.ADMIN_CODE = model.AdminCode;
            entity.IS_ACTIVE = model.IsActive;
            entity.MODIFIED_DATE = DateTime.Now;

            _ePortaDBContext.SaveChanges();
            return 1;
        }


        public bool CheckSecurityExists(int siteId, int empCode)
        {
            return _ePortaDBContext.SECURITY_LOCATION_MAPPING
                .Count(m => m.SITE_ID == siteId && m.EMP_CODE == empCode) > 0;
        }

        public int AddSecurityMapping(SecurityLocationMapViewModelVP model)
        {
            var entity = new SECURITY_LOCATION_MAPPING
            {
                SITE_ID = model.SiteId,
                EMP_CODE = model.EmpCode,
                IS_ACTIVE = model.IsActive,
                CREATED_DATE = DateTime.Now
            };

            _ePortaDBContext.SECURITY_LOCATION_MAPPING.Add(entity);
            _ePortaDBContext.SaveChanges();

            return entity.ID;
        }

        public List<SecurityLocationMapViewModelVP> GetSecurityMappingList()
        {
            return (from map in _ePortaDBContext.SECURITY_LOCATION_MAPPING
                    join site in _ePortaDBContext.SYSITE on map.SITE_ID equals site.SYSITEID
                    join emp in _ePortaDBContext.ADEMPLOYEE on map.EMP_CODE equals emp.ADEMPCODE
                    select new SecurityLocationMapViewModelVP
                    {
                        Id = map.ID,
                        SiteId = map.SITE_ID,
                        SiteName = site.DESCRIP,
                        EmpCode = map.EMP_CODE,
                        EmpName = emp.FIRSTNAME + " " + emp.LASTNAME,
                        IsActive = map.IS_ACTIVE
                    }).ToList();
        }

        public SecurityLocationMapViewModelVP GetSecurityMappingById(int id)
        {
            return (from map in _ePortaDBContext.SECURITY_LOCATION_MAPPING
                    where map.ID == id
                    select new SecurityLocationMapViewModelVP
                    {
                        Id = map.ID,
                        SiteId = map.SITE_ID,
                        EmpCode = map.EMP_CODE,
                        IsActive = map.IS_ACTIVE
                    }).FirstOrDefault();
        }

        public int UpdateSecurityMapping(SecurityLocationMapViewModelVP model)
        {
            var entity = _ePortaDBContext.SECURITY_LOCATION_MAPPING
                .FirstOrDefault(m => m.ID == model.Id);

            if (entity == null)
                return 0;

            entity.EMP_CODE = model.EmpCode;
            entity.IS_ACTIVE = model.IsActive;
            entity.MODIFIED_DATE = DateTime.Now;

            _ePortaDBContext.SaveChanges();
            return 1;
        }
        public List<EmployeeDetailDto> GetSecurityPersonnelBySite()
        {
            try
            {
                var result = (
                    from emp in _ePortaDBContext.ADEMPLOYEE
                    join assoc in _ePortaDBContext.VW_ASSOCIATELVLDETAILS
                        on emp.ADEMPCODE equals assoc.ADEMPCODE
                    where emp.ACTIVE == 1
                    group emp by new
                    {
                        emp.ADEMPCODE,
                        emp.FIRSTNAME,
                        emp.LASTNAME,
                        emp.TMOBILE
                    }
                    into g
                    select new EmployeeDetailDto
                    {
                        EmpCode = g.Key.ADEMPCODE,
                        FullName = g.Key.FIRSTNAME + " " + g.Key.LASTNAME,
                        MobileNumber = g.Key.TMOBILE
                    }
                )
                .OrderBy(a => a.FullName)
                .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching admin list: " + ex.Message);
            }
        }
        public List<int> GetAdminMappedLocations(long empCode)
        {
            return _ePortaDBContext.ADMIN_LOCATION_MAPPING
                             .Where(x => x.ADMIN_CODE == empCode && x.IS_ACTIVE == 1)
                             .Select(x => x.SITE_ID)
                             .ToList();
        }
        public List<int> GetSecurityMappedLocations(long empCode)
        {
            return _ePortaDBContext.SECURITY_LOCATION_MAPPING
                             .Where(x => x.EMP_CODE == empCode && x.IS_ACTIVE == 1)
                             .Select(x => x.SITE_ID)
                             .ToList();
        }
        public List<VehicleMasterDto> GetSecurityVehicleDetailsBySiteId(long userSiteId)
        {

            var allHistory = _ePortaDBContext.EMP_VEHICLE_HISTORY
                            .OrderBy(h => h.VEHICLEID)
                            .ThenByDescending(h => h.ACTIONDATE)
                            .ToList();

            var latestHistoryPerVehicle = _ePortaDBContext.EMP_VEHICLE_HISTORY
                .OrderByDescending(h => h.ACTIONDATE)
                .ToList()  
                .GroupBy(h => h.VEHICLEID)
                .Select(g => g.First())
                .Where(h => h.ADMIN_STATUS != (short)VehicleStatus.Pending)
                .ToList();

            var approvedVehicleIds = latestHistoryPerVehicle
                .Where(h => h.ADMIN_STATUS != (short)VehicleStatus.Pending)
                .Select(h => h.VEHICLEID)
                .ToList();


            if (!approvedVehicleIds.Any())
                return new List<VehicleMasterDto>();

            // STEP 2: Main vehicle list filtered
            var vehicles = _ePortaDBContext.EMPVEHICLEMASTER
                .Where(v => v.LOCATION_ID == userSiteId && approvedVehicleIds.Contains(v.VEHICLEID)).OrderByDescending(x => x.CREATED_DATE)
                .Select(v => new VehicleMasterDto
                {
                    VehicleId = v.VEHICLEID,
                    EmpCode = v.EMPCODE,
                    Status = v.STATUS,
                    AdminActionBy = v.ADMIN_ACTION_BY,
                    AdminActionDate = v.ADMIN_ACTION_DATE,
                    VehicleType = v.VEHICLE_TYPE,
                    VehicleCategory = v.VEHICLE_CATEGORY,
                    SecurityActionBy = v.SECURITY_ACTION_BY,
                    SecurityActionDate = v.SECURITY_ACTION_DATE,
                    CreatedDate = v.CREATED_DATE,
                    LocationId = v.LOCATION_ID,
                    EmpName = string.Empty,
                    DlNumber = v.DL_NUMBER,
                    DlValidTill = v.DL_VALID_TILL,
                    DlPhotoPath = NormalizePathList(v.DL_PHOTOPATH),
                    DlType = v.DL_TYPE,
                    ParkingZoneId = v.PARKING_ZONE_ID,
                    IsExpired = v.IsExpired,
                })
                .ToList();

            // STEP 3: Location mapping
            var siteMap = GetSiteList().ToDictionary(x => x.Id, x => x);

            foreach (var v in vehicles)
            {
                if (siteMap.TryGetValue(v.LocationId, out var loc))
                    v.Location = loc;
            }

            // STEP 4: Parking Zone mapping
            var zoneIds = vehicles
                .Where(x => x.ParkingZoneId.HasValue)
                .Select(x => x.ParkingZoneId.Value)
                .Distinct()
                .ToList();

            var zoneMap = _ePortaDBContext.PARKING_ZONE
                .Where(z => zoneIds.Contains(z.ID))
                .ToDictionary(z => z.ID, z => z);

            foreach (var v in vehicles)
            {
                if (v.ParkingZoneId.HasValue && zoneMap.TryGetValue(v.ParkingZoneId.Value, out var zone))
                    v.ParkingZone = zone;
            }

            var history = _ePortaDBContext.EMP_VEHICLE_HISTORY.Where(h => vehicles.Select(v => v.VehicleId).Contains(h.VEHICLEID)).ToList();

            if (history == null || history.Count == 0)
            {
                vehicles.ForEach(v => v.VehicleHistory = new List<VehicleHistoryDto>());
            }
            else
            {
                vehicles.ForEach(v =>
                {
                    v.VehicleHistory = history
                        .Where(h => h.VEHICLEID == v.VehicleId)
                        .Select(h => new VehicleHistoryDto
                        {
                            Id = h.ID,
                            VehicleId = h.VEHICLEID,
                            ActionBy = h.ACTIONBY,
                            ActionByRole = h.ACTIONBYROLE,
                            ActionDate = h.ACTIONDATE,
                            ParkingZoneId = h.PARKINGZONEID,
                            SendBackFor = h.SENDBACK_FOR,
                            AdminRemark = h.ADMINREMARK,
                            AdminStatus = h.ADMIN_STATUS,
                            SecurityStatus = h.SECURITY_STATUS,
                            SpRemark = h.SPREMARK,
                        })
                        .ToList();
                });
            }

            // STEP 6: Vehicle Details
            foreach (var vehicle in vehicles)
            {
                var vehicleDetails = _ePortaDBContext.EMP_VEHICLE_DETAIL
                    .Where(d => d.VEHICLEID == vehicle.VehicleId)
                    .Select(d => new EMPVehicleDetailDto
                    {
                        Id = d.ID,
                        VehicleId = d.VEHICLEID,
                        VehicleNo = d.VEHICLENO,
                        FuelType = d.FUELTYPE,
                        ModelYear = d.MODELYEAR,
                        OwnerType = d.OWNERTYPE,
                        OwnerName = d.OWNERNAME,
                        RcNo = d.RCNO,
                        RcValidTill = d.RCVALIDTILL,
                        RcPhotoPath = NormalizePathList(d.RCPHOTOPATH),

                        InsuranceNo = d.INSURANCENO,
                        InsuranceValidTill = d.INSURANCEVALIDTILL,
                        InsurancePhotoPath = NormalizePathList(d.INSURANCEPHOTOPATH),

                        PucNo = d.PUCNO,
                        PucValidTill = d.PUCVALIDTILL,
                        PucPhotoPath = NormalizePathList(d.PUCPHOTOPATH),

                        UploadedBy = d.UPLOADEDBY,
                        UploadedOn = d.UPLOADEDON,
                        VehicleCustodian = d.VEHICLECUSTODIAN,
                        CustodianDepartment = "",
                        CustodianDesignation = "",
                        CustodianEmpName = ""
                    })
                    .ToList();

                vehicle.VehicleDetail = vehicleDetails;

                // Custodian mapping
                foreach (var v in vehicleDetails)
                {
                    if (long.TryParse(v.VehicleCustodian, out var empCode))
                    {
                        var emp = GetEmployeeDetailsByEmpCode(empCode);
                        if (emp != null)
                        {
                            v.CustodianDepartment = emp.Department;
                            v.CustodianDesignation = emp.Designation;
                            v.CustodianEmpName = emp.FullName;
                        }
                    }
                }

                // Employee mapping
                if (long.TryParse(vehicle.EmpCode, out long empCodeLong))
                {
                    var empDetails = GetEmployeeDetailsByEmpCode(empCodeLong);
                    if (empDetails != null)
                    {
                        vehicle.EmpName = empDetails.FullName;
                        vehicle.Department = empDetails.Department;
                        vehicle.Designation = empDetails.Designation;
                        vehicle.MobileNumber = empDetails.MobileNumber;
                        vehicle.Employee = empDetails;
                    }
                }
            }

            return vehicles;
        }
        public List<LocationDto> GetSitesByIds(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<LocationDto>();

            return _ePortaDBContext.SYSITE
                .Where(x => x.ACTIVE == 1 && ids.Contains((int)x.SYSITEID))
                .OrderBy(x => x.DESCRIP)
                .Select(x => new LocationDto
                {
                    Id = x.SYSITEID,
                    Value = x.SYSITEID,
                    Text = x.DESCRIP
                })
                .ToList();
        }
        public List<VehicleExportResponseDto> GetAdminVehiclesForExportExcel(
            VehicleExportDto model,
            List<long> siteIds)
        {
            var query =
                from v in _ePortaDBContext.EMPVEHICLEMASTER
                where v.STATUS == 1
                      && siteIds.Contains(v.LOCATION_ID)

                // ---- LOCATION ----
                join loc in _ePortaDBContext.SYSITE
                    on v.LOCATION_ID equals loc.SYSITEID

                // ---- ZONE (LEFT JOIN) ----
                join z in _ePortaDBContext.PARKING_ZONE
                    on v.PARKING_ZONE_ID equals z.ID into zJoin
                from zone in zJoin.DefaultIfEmpty()

                    // ---- EMPLOYEE ----
                join emp in _ePortaDBContext.ADEMPLOYEE
                 on Convert.ToInt64(v.EMPCODE) equals emp.ADEMPCODE

                let latestHistory =
                    _ePortaDBContext.EMP_VEHICLE_HISTORY
                        .Where(h => h.VEHICLEID == v.VEHICLEID)
                        .OrderByDescending(h => h.ID)
                        .Select(h => new
                        {
                            h.ACTIONBY,
                            h.ACTIONDATE,
                            h.ADMIN_STATUS
                        })
                        .FirstOrDefault()

                select new
                {
                    Vehicle = v,
                    LocationName = loc.DESCRIP,
                    ZoneName = zone != null ? zone.ZONE_NAME : null,
                    EmpName = emp.FIRSTNAME + " " + emp.LASTNAME,
                    History = latestHistory
                };

            // -------- STATUS FILTER --------
            if (model.Status.HasValue)
            {
                if (model.Status.Value == 0) // Pending
                {
                    query = query.Where(x =>
                        x.History == null ||
                        x.History.ADMIN_STATUS == 0);
                }
                else if (model.Status.Value == 1) // Approved
                {
                    query = query.Where(x =>
                        x.History != null &&
                        x.History.ADMIN_STATUS == 1);
                }
            }

            // -------- VEHICLE CATEGORY --------
            if (model.VehicleCategory.HasValue)
                query = query.Where(x =>
                    x.Vehicle.VEHICLE_CATEGORY == model.VehicleCategory.Value);

            // -------- LOCATION FILTER --------
            if (model.LocationId.HasValue)
                query = query.Where(x =>
                    x.Vehicle.LOCATION_ID == model.LocationId.Value);

            // -------- DATE RANGE --------
            if (model.DateFrom.HasValue)
            {
                var from = model.DateFrom.Value.Date;
                query = query.Where(x => x.Vehicle.CREATED_DATE >= from);
            }

            if (model.DateTo.HasValue)
            {
                var to = model.DateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.Vehicle.CREATED_DATE <= to);
            }

            return query
                .OrderByDescending(x => x.Vehicle.CREATED_DATE)
                .Select(x => new VehicleExportResponseDto
                {
                    VehicleId = x.Vehicle.VEHICLEID,
                    EmpCode = x.Vehicle.EMPCODE,
                    EmpName = x.EmpName,

                    VehicleCategory = x.Vehicle.VEHICLE_CATEGORY,
                    CreatedDate = x.Vehicle.CREATED_DATE,

                    LocationId = x.Vehicle.LOCATION_ID,
                    LocationName = x.LocationName,

                    ParkingZoneId = x.Vehicle.PARKING_ZONE_ID,
                    ParkingZoneName = x.ZoneName,

                    AdminActionBy = x.History != null ? x.History.ACTIONBY : null,
                    AdminActionDate = x.History != null ? x.History.ACTIONDATE : null,
                    Status = x.History != null ? x.History.ADMIN_STATUS ?? 0 : 0
                })
                .ToList();
        }

        public List<VehicleExportResponseDto> GetSecurityVehiclesForExportExcel(
            VehicleExportDto model,
            List<long> siteIds)
        {
            var query =
                from v in _ePortaDBContext.EMPVEHICLEMASTER
                where siteIds.Contains(v.LOCATION_ID)

                // ---- LOCATION ----
                join loc in _ePortaDBContext.SYSITE
                    on v.LOCATION_ID equals loc.SYSITEID

                // ---- ZONE (LEFT JOIN) ----
                join z in _ePortaDBContext.PARKING_ZONE
                    on v.PARKING_ZONE_ID equals z.ID into zJoin
                from zone in zJoin.DefaultIfEmpty()

                    // ---- EMPLOYEE ----
                join emp in _ePortaDBContext.ADEMPLOYEE
                    on Convert.ToInt64(v.EMPCODE) equals emp.ADEMPCODE

                let latestHistory =
                    _ePortaDBContext.EMP_VEHICLE_HISTORY
                        .Where(h => h.VEHICLEID == v.VEHICLEID)
                        .OrderByDescending(h => h.ID)
                        .Select(h => new
                        {
                            h.ACTIONBY,
                            h.ACTIONDATE,
                            h.ADMIN_STATUS,
                            h.SECURITY_STATUS,
                            h.SPREMARK
                        })
                        .FirstOrDefault()

                // 🔐 BASE SECURITY VISIBILITY RULE (MOST IMPORTANT)
                where latestHistory != null &&
                      (
                          latestHistory.ADMIN_STATUS == 1 ||
                          latestHistory.SECURITY_STATUS == 2 ||
                          latestHistory.SECURITY_STATUS == 3
                      )

                select new
                {
                    Vehicle = v,
                    LocationName = loc.DESCRIP,
                    ZoneName = zone != null ? zone.ZONE_NAME : null,
                    EmpName = emp.FIRSTNAME + " " + emp.LASTNAME,
                    History = latestHistory
                };

            // -------- SECURITY STATUS FILTER (UI) --------
            if (model.Status.HasValue)
            {
                if (model.Status.Value == 0) // Pending
                {
                    query = query.Where(x =>
                        x.History.SECURITY_STATUS == 0);
                }
                else if (model.Status.Value == 1) // Approved
                {
                    query = query.Where(x =>
                        x.History.SECURITY_STATUS == 1);
                }
                else if (model.Status.Value == 2) // Send Back
                {
                    query = query.Where(x =>
                        x.History.SECURITY_STATUS == 2);
                }
                else if (model.Status.Value == 3) // Reject
                {
                    query = query.Where(x =>
                        x.History.SECURITY_STATUS == 3);
                }
            }

            // -------- VEHICLE CATEGORY --------
            if (model.VehicleCategory.HasValue)
                query = query.Where(x =>
                    x.Vehicle.VEHICLE_CATEGORY == model.VehicleCategory.Value);

            // -------- LOCATION FILTER --------
            if (model.LocationId.HasValue)
                query = query.Where(x =>
                    x.Vehicle.LOCATION_ID == model.LocationId.Value);

            // -------- DATE RANGE --------
            if (model.DateFrom.HasValue)
            {
                var from = model.DateFrom.Value.Date;
                query = query.Where(x => x.Vehicle.CREATED_DATE >= from);
            }

            if (model.DateTo.HasValue)
            {
                var to = model.DateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.Vehicle.CREATED_DATE <= to);
            }

            return query
                .OrderByDescending(x => x.Vehicle.CREATED_DATE)
                .Select(x => new VehicleExportResponseDto
                {
                    VehicleId = x.Vehicle.VEHICLEID,
                    EmpCode = x.Vehicle.EMPCODE,
                    EmpName = x.EmpName,

                    VehicleCategory = x.Vehicle.VEHICLE_CATEGORY,
                    CreatedDate = x.Vehicle.CREATED_DATE,

                    LocationId = x.Vehicle.LOCATION_ID,
                    LocationName = x.LocationName,

                    ParkingZoneId = x.Vehicle.PARKING_ZONE_ID,
                    ParkingZoneName = x.ZoneName,

                    AdminActionBy = x.History.ACTIONBY,
                    AdminActionDate = x.History.ACTIONDATE,
                    Status = x.History.SECURITY_STATUS ?? 0
                })
                .ToList();
        }


        public string GetUserEmailByEmpCode(string empCode)
        {
            var employee = _ePortaDBContext.ADEMPLOYEE
                .Where(x => x.ADEMPCODE.ToString() == empCode && x.ACTIVE == 1)
                .FirstOrDefault();

            return employee?.EMAILID ?? string.Empty;
        }
        public List<string> GetAdminEmailsBySiteId(long sysiteid)
        {
            return _ePortaDBContext.ADMIN_LOCATION_MAPPING
                .Where(alm => alm.SITE_ID == sysiteid && alm.IS_ACTIVE == 1)
                .Join(
                    _ePortaDBContext.ADEMPLOYEE,
                    alm => alm.ADMIN_CODE,
                    emp => emp.ADEMPCODE,
                    (alm, emp) => emp.EMAILID
                )
                .ToList();
        }
        public List<string> GetApproveByAdminEmailToSecurityNotification(long sysiteid)
        {
            return _ePortaDBContext.SECURITY_LOCATION_MAPPING
                .Where(alm => alm.SITE_ID == sysiteid && alm.IS_ACTIVE == 1)
                .Join(
                    _ePortaDBContext.ADEMPLOYEE,
                    alm => alm.EMP_CODE,
                    emp => emp.ADEMPCODE,
                    (alm, emp) => emp.EMAILID
                )
                .ToList();
        }
		public EmployeeDetailDto GetApproveByAdminDetials(int vehicleId)
		{
            var actionby = _ePortaDBContext.EMP_VEHICLE_HISTORY.Where(h => h.VEHICLEID == vehicleId && h.ADMIN_STATUS == (int)VehicleStatus.Approved).FirstOrDefault();

            var adminDetials = _ePortaDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Convert.ToInt64(actionby.ACTIONBY)).FirstOrDefault();

			return new EmployeeDetailDto
			{
                EmpCode = adminDetials.ADEMPCODE,
				FullName = adminDetials.FIRSTNAME + " " +adminDetials.LASTNAME
			};
		}





		public List<VehicleMasterDto> GetExpiredVehicles(long? userSiteId)
        {
            var vehicles = _ePortaDBContext.EMPVEHICLEMASTER
                  .Where(v => v.STATUS == 1 && v.IsExpired == true && v.LOCATION_ID == userSiteId)
                   .Select(v => new VehicleMasterDto
                   {
                       VehicleId = v.VEHICLEID,
                       EmpCode = v.EMPCODE,
                       Status = v.STATUS,
                       AdminActionBy = v.ADMIN_ACTION_BY,
                       AdminActionDate = v.ADMIN_ACTION_DATE,
                       VehicleType = v.VEHICLE_TYPE,
                       VehicleCategory = v.VEHICLE_CATEGORY,
                       SecurityActionBy = v.SECURITY_ACTION_BY,
                       SecurityActionDate = v.SECURITY_ACTION_DATE,
                       DriverName = v.DRIVER_NAME,
                       CreatedDate = v.CREATED_DATE,
                       LocationId = v.LOCATION_ID,
                       EmpName = string.Empty,
                       DlNumber = v.DL_NUMBER,
                       DlValidTill = v.DL_VALID_TILL,
                       DlPhotoPath = NormalizePathList(v.DL_PHOTOPATH),
                       DlType = v.DL_TYPE,
                   })
                  .ToList();

            var zoneIds = vehicles
               .Where(x => x.ParkingZoneId.HasValue)
               .Select(x => x.ParkingZoneId.Value)
               .Distinct()
               .ToList();

            var zoneMap = _ePortaDBContext.PARKING_ZONE
                .Where(z => zoneIds.Contains(z.ID))
                .ToDictionary(z => z.ID, z => z);

            foreach (var v in vehicles)
            {
                if (v.ParkingZoneId.HasValue && zoneMap.TryGetValue(v.ParkingZoneId.Value, out var zone))
                    v.ParkingZone = zone;
            }

            var history = _ePortaDBContext.EMP_VEHICLE_HISTORY.Where(h => vehicles.Select(v => v.VehicleId).Contains(h.VEHICLEID)).ToList();

            if (history == null || history.Count == 0)
            {
                vehicles.ForEach(v => v.VehicleHistory = new List<VehicleHistoryDto>());
            }
            else
            {
                vehicles.ForEach(v =>
                {
                    v.VehicleHistory = history
                        .Where(h => h.VEHICLEID == v.VehicleId)
                        .Select(h => new VehicleHistoryDto
                        {
                            Id = h.ID,
                            VehicleId = h.VEHICLEID,
                            ActionBy = h.ACTIONBY,
                            ActionByRole = h.ACTIONBYROLE,
                            ActionDate = h.ACTIONDATE,
                            ParkingZoneId = h.PARKINGZONEID,
                            SendBackFor = h.SENDBACK_FOR,
                            AdminRemark = h.ADMINREMARK,
                            AdminStatus = h.ADMIN_STATUS,
                            SecurityStatus = h.SECURITY_STATUS,
                            SpRemark = h.SPREMARK,
                        })
                        .ToList();
                });
            }

            foreach (var vehicle in vehicles)
            {
                var vehicleDetails = _ePortaDBContext.EMP_VEHICLE_DETAIL
                    .Where(d => d.VEHICLEID == vehicle.VehicleId)
                    .Select(d => new EMPVehicleDetailDto
                    {
                        Id = d.ID,
                        VehicleId = d.VEHICLEID,
                        VehicleNo = d.VEHICLENO,
                        FuelType = d.FUELTYPE,
                        ModelYear = d.MODELYEAR,
                        OwnerType = d.OWNERTYPE,
                        OwnerName = d.OWNERNAME,
                        RcNo = d.RCNO,
                        RcValidTill = d.RCVALIDTILL,
                        RcPhotoPath = NormalizePathList(d.RCPHOTOPATH),

                        InsuranceNo = d.INSURANCENO,
                        InsuranceValidTill = d.INSURANCEVALIDTILL,
                        InsurancePhotoPath = NormalizePathList(d.INSURANCEPHOTOPATH),

                        PucNo = d.PUCNO,
                        PucValidTill = d.PUCVALIDTILL,
                        PucPhotoPath = NormalizePathList(d.PUCPHOTOPATH),

                        UploadedBy = d.UPLOADEDBY,
                        UploadedOn = d.UPLOADEDON,
                        VehicleCustodian = d.VEHICLECUSTODIAN,
                        CustodianDepartment = "",
                        CustodianDesignation = "",
                        CustodianEmpName = ""
                    })
                    .ToList();

                vehicle.VehicleDetail = vehicleDetails;

                foreach (var v in vehicleDetails)
                {
                    if (long.TryParse(v.VehicleCustodian, out var empCode))
                    {
                        var emp = GetEmployeeDetailsByEmpCode(empCode);
                        if (emp != null)
                        {
                            v.CustodianDepartment = emp.Department;
                            v.CustodianDesignation = emp.Designation;
                            v.CustodianEmpName = emp.FullName;
                        }
                    }
                }

                if (long.TryParse(vehicle.EmpCode, out long empCodeLong))
                {
                    var empDetails = GetEmployeeDetailsByEmpCode(empCodeLong);
                    if (empDetails != null)
                    {
                        vehicle.EmpName = empDetails.FullName;
                        vehicle.Department = empDetails.Department;
                        vehicle.Designation = empDetails.Designation;
                        vehicle.MobileNumber = empDetails.MobileNumber;
                    }
                }
            }
            return vehicles;
        }
    }
}
