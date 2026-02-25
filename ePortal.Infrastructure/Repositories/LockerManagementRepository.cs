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

        public LockerAdminRequestDTO GetLockerMangmentDataForAdmin(int empCode)
        {
            LockerAdminRequestDTO result = new LockerAdminRequestDTO();

            var associateDetails = GetAssociateDetailsFromSP(empCode.ToString());

            var data = (
                from map in _ePortaDBContext.LOCKERASSIGNMENTMASTER
                where map.EMP_ID == empCode
                orderby map.CREATED_AT
                select new LockerAdminLocationMapViewModel
                {
                    Id = map.ASSIGN_ID,
                    EmployeeName = associateDetails.EmployeeName + "-" + associateDetails.EmployeeCode.ToString(),
                    SiteName = associateDetails.SiteName,
                    Status = map.STATUS,
                    RequestDate = map.REQUEST_DATE,
                    //  From Stored Procedure
                    EmployeeDepartment = associateDetails.EmployeeDepartment,
                    EmployeeDesignation = associateDetails.EmployeeDesignation,
                    EmployeeOperation = associateDetails.EmployeeOperation,
                    EmployeeSection = associateDetails.EmployeeSection,
                    EmployeeDivision = associateDetails.EmployeeDivision,
                }
            ).ToList();

            result.AdminRequest = data;
            result.TotalRequest = data.Count;
            result.PendingRequest = data.Count(r => r.Status == 2);
            result.ApprovedRequest = data.Count(r => r.Status == 3);


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
    }

}
