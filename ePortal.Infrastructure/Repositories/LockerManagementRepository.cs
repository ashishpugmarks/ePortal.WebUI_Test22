using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.Locker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.Infrastructure.Repositories
{
    public class LockerManagementRepository
    {
        private EPortalDBContext _ePortaDBContext;
        private SYKI _Syki;
        private readonly IConfiguration _configuration;
        //private readonly string _uploadPath;
        private readonly IAppConfigurationService _configurations;
        public LockerManagementRepository(EPortalDBContext objEPortalDBContext, IConfiguration configuration, IAppConfigurationService appConfigurations)
        {
            _ePortaDBContext = objEPortalDBContext;
            _Syki = _ePortaDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            _configuration = configuration;

            _configurations = appConfigurations;
            // Ensure upload directory exists

        }






        //Admin And Security Mapping
        public bool CheckExists(int siteId, int adminCode)
        {
            return _ePortaDBContext.LOCKER_ADMIN_LOCATION_MAPPING
        .Count(m => m.SITE_ID == siteId && m.ADMIN_CODE == adminCode) > 0;
        }
        public int AddAdmin(LockerAdminLocationMapViewModel adminmap)
        {
            try
            {
                // Map ViewModel → Entity
                var entity = new LOCKER_ADMIN_LOCATION_MAPPING
                {
                    SITE_ID = adminmap.SiteId,
                    ADMIN_CODE = adminmap.AdminCode,
                    IS_ACTIVE = adminmap.IsActive,
                    CREATED_DATE = DateTime.Now
                };

                _ePortaDBContext.LOCKER_ADMIN_LOCATION_MAPPING.Add(entity);
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


        public List<LockerAdminLocationMapViewModel> GetAdminMappingList()
        {
            try
            {
                var result =
                    (from map in _ePortaDBContext.LOCKER_ADMIN_LOCATION_MAPPING
                     join site in _ePortaDBContext.SYSITE
                         on map.SITE_ID equals site.SYSITEID
                     join emp in _ePortaDBContext.ADEMPLOYEE
                         on map.ADMIN_CODE equals emp.ADEMPCODE
                     orderby site.DESCRIP
                     select new LockerAdminLocationMapViewModel
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
        public LockerAdminLocationMapViewModel GetAdminMappingById(int id)
        {
            return (
                from map in _ePortaDBContext.LOCKER_ADMIN_LOCATION_MAPPING
                where map.ID == id
                select new LockerAdminLocationMapViewModel
                {
                    Id = map.ID,
                    SiteId = map.SITE_ID,
                    AdminCode = map.ADMIN_CODE,
                    IsActive = map.IS_ACTIVE,
                }
            ).FirstOrDefault();
        }
        public int UpdateAdminMapping(LockerAdminLocationMapViewModel model)
        {
            var entity = _ePortaDBContext.LOCKER_ADMIN_LOCATION_MAPPING.FirstOrDefault(m => m.ID == model.Id);

            if (entity == null)
                return 0;

            entity.ADMIN_CODE = model.AdminCode;
            entity.IS_ACTIVE = model.IsActive;
            entity.MODIFIED_DATE = DateTime.Now;

            _ePortaDBContext.SaveChanges();
            return 1;
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
        //public LockerAdminRequestDTO GetLockerMangmentDataForAdmin(int empCode)
        //{
        //    LockerAdminRequestDTO result= new LockerAdminRequestDTO();

        //    var data = (
        //        from map in _ePortaDBContext.LOCKER_ADMIN_LOCATION_MAPPING

        //        join emp in _ePortaDBContext.ADEMPLOYEE
        //            on map.ADMIN_CODE equals emp.ADEMPCODE

        //        join site in _ePortaDBContext.SYSITE
        //            on map.SITE_ID equals site.SYSITEID

        //        where map.ADMIN_CODE == empCode

        //        orderby map.CREATED_DATE

        //        select new LockerAdminLocationMapViewModel
        //        {
        //            Id = map.ID,

        //            EmployeeName = emp.FIRSTNAME + " " + emp.LASTNAME,
        //            EmployeeCode = emp.ADEMPCODE.ToString(),

        //            SiteId = map.SITE_ID,
        //            SiteName = site.DESCRIP,

        //            AdminCode = map.ADMIN_CODE,
        //            AdminName = emp.FIRSTNAME + " " + emp.LASTNAME,


        //            IsActive = map.IS_ACTIVE
        //        }
        //    ).ToList();


        //     result.AdminRequest = data;
        //    return result;
        //}
        public List<LockerAdminLocationMapViewModel> GetLockerMangmentDataForAdmin(int siteid)
        {
            var assignments = _ePortaDBContext.LOCKERASSIGNMENTMASTER
                .Where(map => map.SYSITEID == siteid)


                .GroupJoin(_ePortaDBContext.FLOORMASTER,
                    map => map.FLOOR_ID,
                    floor => floor.FLOOR_ID,
                    (map, floors) => new { map, floors })
                .SelectMany(x => x.floors.DefaultIfEmpty(),
                    (x, floor) => new { x.map, floor })


                .GroupJoin(_ePortaDBContext.LOCKERMASTER,
                    mf => mf.map.LOCKER_ID,
                    locker => locker.LOCKER_ID,
                    (mf, lockers) => new { mf.map, mf.floor, lockers })
                .SelectMany(x => x.lockers.DefaultIfEmpty(),
                    (x, locker) => new { x.map, x.floor, locker })

                .GroupJoin(_ePortaDBContext.LOCKERBOXMASTER,
                    mfl => mfl.map.BOX_ID,
                    box => box.BOX_ID,
                    (mfl, boxes) => new { mfl.map, mfl.floor, mfl.locker, boxes })
                .SelectMany(x => x.boxes.DefaultIfEmpty(),
                    (x, box) => new
                    {
                        x.map.ASSIGN_ID,
                        x.map.STATUS,
                        x.map.REQUEST_DATE,
                        x.map.EMP_ID,
                        x.map.SYSITEID,
                        x.map.FLOOR_ID,
                        x.map.LOCKER_ID,
                        x.map.BOX_ID,

                        FloorName = x.floor != null ? x.floor.FLOOR_NAME : null,
                        LockerName = x.locker != null ? x.locker.LOCKER_CODE : null,
                        BoxNumber = box != null ? box.BOX_NO : null
                    })
                .ToList();


            var result = new List<LockerAdminLocationMapViewModel>();

            foreach (var map in assignments)
            {
                var associateDetails = GetAssociateDetailsFromSP(map.EMP_ID.ToString());

                result.Add(new LockerAdminLocationMapViewModel
                {
                    Id = map.ASSIGN_ID,
                    Status = map.STATUS,
                    RequestDate = map.REQUEST_DATE,
                    EmployeeCode = map.EMP_ID.ToString(),
                    SiteId = Convert.ToInt32(map.SYSITEID),
                    FloorId = map.FLOOR_ID,
                    LockeId = map.LOCKER_ID,
                    LockerBoxId = map.BOX_ID,
                    FloorName = map.FloorName,
                    LockerName = map.LockerName,
                    BoxNumber = map.BoxNumber,



                    SiteName = associateDetails?.SiteName,

                    EmployeeName = associateDetails != null
                                    ? associateDetails.EmployeeName + "-" + associateDetails.EmployeeCode
                                    : null,
                    EmployeeDepartment = associateDetails?.EmployeeDepartment,
                    EmployeeDesignation = associateDetails?.EmployeeDesignation,
                    EmployeeOperation = associateDetails?.EmployeeOperation,
                    EmployeeSection = associateDetails?.EmployeeSection,
                    EmployeeDivision = associateDetails?.EmployeeDivision,
                });
            }

            return result;
        }


        public LockerAdminLocationMapViewModel GetAssociateDetailsFromSP(string empCode)
        {
            LockerAdminLocationMapViewModel employee = new LockerAdminLocationMapViewModel();

            var conn = (OracleConnection)_ePortaDBContext.Database.GetDbConnection();

            if (conn.State != ConnectionState.Open)
                conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PKG_COMMONMETHOD.SPROC_ASSOCIATESEARCH_GET";
                cmd.CommandType = CommandType.StoredProcedure;

                var oCmd = (OracleCommand)cmd;
                oCmd.BindByName = true;

                oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = empCode;
                oCmd.Parameters.Add("FIRSTNAME_IN", OracleDbType.Varchar2).Value = "";
                oCmd.Parameters.Add("LASTNAME_IN", OracleDbType.Varchar2).Value = "";
                oCmd.Parameters.Add("BLOODGROUP_IN", OracleDbType.Varchar2).Value = "";
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = "";
                oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = "";
                oCmd.Parameters.Add("FUNDESIGNATION_IN", OracleDbType.Varchar2).Value = "";
                oCmd.Parameters.Add("LVL", OracleDbType.Varchar2).Value = "";
                oCmd.Parameters.Add("KIID", OracleDbType.Varchar2).Value = 14;
                oCmd.Parameters.Add("CUR_ASSOCIATE", OracleDbType.RefCursor)
                    .Direction = ParameterDirection.Output;

                using (var reader = oCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employee.EmployeeDesignation = reader["DESIGNATION"]?.ToString();
                        employee.EmployeeOperation = reader["OPERATION"]?.ToString();
                        employee.EmployeeDivision = reader["DIVISION"]?.ToString();
                        employee.EmployeeDepartment = reader["DEPARTMENT"]?.ToString();
                        employee.EmployeeSection = reader["SECTION"]?.ToString();
                        employee.EmployeeCode = reader["EMPCODE"].ToString();
                        employee.EmployeeName = reader["NAME"].ToString();
                        employee.SiteName = reader["SYSITE"].ToString();
                    }
                }
            }

            // ❌ DO NOT CLOSE CONNECTION HERE
            // EF Core manages it automatically

            return employee;
        }



        public List<int> GetAdminMappedLocations(long empCode)
        {
            return _ePortaDBContext.LOCKER_ADMIN_LOCATION_MAPPING
                             .Where(x => x.ADMIN_CODE == empCode && x.IS_ACTIVE == 1)
                             .Select(x => x.SITE_ID)
                             .ToList();
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

        public FloorResponse GetFloorList(int siteId)
        {
            var allFloors = _ePortaDBContext.FLOORMASTER
                            .Where(x => x.SYSITEID == siteId)
                            .ToList();

            var totalFloorCount = allFloors.Count();

            var activeFloorCount = allFloors.Count(x => x.STATUS == 1);

            var floorList = allFloors
                .Where(x => x.STATUS == 1)
                .Select(data => new FloorList
                {
                    Id = data.FLOOR_ID,
                    Name = data.FLOOR_NAME
                })
                .OrderBy(o => o.Name)
                .ToList();

            return new FloorResponse
            {
                TotalFloor = totalFloorCount,
                ActiveFloor = activeFloorCount,
                Floors = floorList
            };
        }

        public LockerResponse GetLockerListByFloor(int floorId)
        {
            var lockers = _ePortaDBContext.LOCKERMASTER
                          .Where(x => x.FLOOR_ID == floorId)
                          .ToList();

            var totalLockerCount = lockers.Count();

            // Change condition according to your DB logic
            var assignedLockerCount = lockers.Count(x => x.STATUS == 1);

            var lockerList = lockers
                .Select(data => new LockerList
                {
                    Id = data.LOCKER_ID,
                    LockerName = data.LOCKER_CODE
                })
                .OrderBy(o => o.LockerName)
                .ToList();

            return new LockerResponse
            {
                TotalLocker = totalLockerCount,
                AssignedLocker = assignedLockerCount,
                Lockers = lockerList
            };
        }
        //public List<LockerList> GetLockerListByFloor(int floorId)
        //{
        //    var iList = (from data in _ePortaDBContext.LOCKERMASTER
        //                 where data.FLOOR_ID == floorId 
        //                 select new LockerList
        //                 {
        //                     Id = data.LOCKER_ID,
        //                     LockerName = data.LOCKER_CODE


        //                 })
        //                 .OrderBy(o => o.LockerName)
        //                 .ToList();


        //    return iList;

        //}
        //public List<LockerBoxList> GetLockerBoxesByLockerId(int lockerId)
        //{
        //    var iList = (from data in _ePortaDBContext.LOCKERBOXMASTER
        //                 where data.LOCKER_ID == lockerId && data.STATUS == 0
        //                 select new LockerBoxList
        //                 {
        //                     Id = data.BOX_ID,
        //                     BoxNumber = data.BOX_NO
        //                 })
        //                 .OrderBy(o => o.BoxNumber)
        //                 .ToList();

        //    return iList;
        //}
        //public LockerBoxResponse GetLockerBoxesByLockerId(int lockerId)
        //{
        //    var allBoxes = _ePortaDBContext.LOCKERBOXMASTER
        //                    .Where(x => x.LOCKER_ID == lockerId)
        //                    .ToList();

        //    var totalBoxCount = allBoxes.Count();

        //    // Assuming STATUS == 1 means assigned
        //    var assignedBoxCount = allBoxes.Count(x => x.STATUS == 1);

        //    // Available boxes only
        //    var availableBoxes = allBoxes
        //        .Where(x => x.STATUS == 0)
        //        .Select(data => new LockerBoxList
        //        {
        //            Id = data.BOX_ID,
        //            BoxNumber = data.BOX_NO
        //        })
        //        .OrderBy(o => o.BoxNumber)
        //        .ToList();

        //    return new LockerBoxResponse
        //    {
        //        TotalLockerBox = totalBoxCount,
        //        TotalAssignedBox = assignedBoxCount,
        //        Boxes = availableBoxes
        //    };
        //}

        public LockerBoxResponse GetLockerBoxesByLockerId(int lockerId)
        {
            var allBoxes = _ePortaDBContext.LOCKERBOXMASTER
                            .Where(x => x.LOCKER_ID == lockerId)
                            .ToList();

            var totalBoxCount = allBoxes.Count();

            var assignedBoxIds = _ePortaDBContext.LOCKERASSIGNMENTMASTER
                                    .Where(x => x.LOCKER_ID == lockerId
     && x.STATUS == 4
     && x.RELEASE_DATE == null)
                                    .Select(x => x.BOX_ID)
                                    .ToList();

            var assignedBoxCount = assignedBoxIds.Count();

            var availableBoxes = allBoxes
                .Where(x => !assignedBoxIds.Contains(x.BOX_ID))
                .Select(x => new LockerBoxList
                {
                    Id = x.BOX_ID,
                    BoxNumber = x.BOX_NO
                })
                .OrderBy(x => x.BoxNumber)
                .ToList();

            return new LockerBoxResponse
            {
                TotalLockerBox = totalBoxCount,
                TotalAssignedBox = assignedBoxCount,
                Boxes = availableBoxes
            };
        }

        public bool AssignLocker(int requestId, int floorId, int lockerId, int boxId, int assignBy)
        {
            using var transaction = _ePortaDBContext.Database.BeginTransaction();

            try
            {
                // Check if box already assigned
                var alreadyAssigned = _ePortaDBContext.LOCKERASSIGNMENTMASTER
     .Count(x => x.BOX_ID == boxId
     && x.STATUS == 4
     && x.RELEASE_DATE == null) > 0;

                if (alreadyAssigned)
                    return false;

                var request = _ePortaDBContext.LOCKERASSIGNMENTMASTER
                                .FirstOrDefault(x => x.ASSIGN_ID == requestId);

                if (request == null)
                    return false;

                request.FLOOR_ID = floorId;
                request.LOCKER_ID = lockerId;
                request.BOX_ID = boxId;
                request.STATUS = 4; // Assigned
                request.ASSIGNED_BY = assignBy;
                request.ASSIGNED_DATE = DateTime.Now;

                _ePortaDBContext.SaveChanges();

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }

}
