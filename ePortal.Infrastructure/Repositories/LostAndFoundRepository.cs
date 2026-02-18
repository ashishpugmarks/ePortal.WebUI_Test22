
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spire.Additions.Xps.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class LostAndFoundRepository
    {
        private readonly EPortalDBContext _dbContext;
        private readonly SYKI _syki;
        private readonly IConfiguration _configuration;

        public LostAndFoundRepository(EPortalDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _syki = _dbContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            _configuration = configuration;
        }

        public LostAndFound GetById(long id)
        {
            var result = _dbContext.LostAndFound.ToList();
            return _dbContext.LostAndFound
                .Where(x => x.Id == id && x.IsActive == 1 && x.Status != LostAndFoundStatus.Cancel)
                .FirstOrDefault();
        }
        public LostAndFound GetDetailById(long id)
        {
            return _dbContext.LostAndFound
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }
        public List<LostAndFound> GetAllActive()
        {
            var sevenDaysAgo = DateTime.Now.AddDays(-7);

            return _dbContext.LostAndFound
                .Where(x => x.IsActive == 1 && x.Status == 1 && x.Date >= sevenDaysAgo) // Only approved items
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        public List<LostAndFound> GetByUserLocation(long userSiteId)
        {
            return _dbContext.LostAndFound
                .Where(x => x.IsActive == 1 && x.LocationId == userSiteId)
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        public List<LostAndFound> GetActiveByUserLocation(long userSiteId)
        {
            var sevenDaysAgo = DateTime.Now.AddDays(-7);
            var data = _dbContext.LostAndFound
                .Where(x => x.IsActive == 1 && x.Status == 1 && x.LocationId == userSiteId && x.Date >= sevenDaysAgo)
                .OrderByDescending(x => x.Date)
                .ToList();
            return data;
        }

        public List<LostAndFound> GetAll()
        {
            return _dbContext.LostAndFound
                .Where(x => x.IsActive == 1 && x.Status != LostAndFoundStatus.Cancel)
                .OrderByDescending(x => x.Date)
                .ToList();
        }
        public async Task<string> GetEmployeeNameByCodeAsync(string employeeCode)
        {
            if (string.IsNullOrEmpty(employeeCode))
                return string.Empty;

            try
            {
                // Try to parse the employee code as a long (ADEMPCODE)
                if (long.TryParse(employeeCode, out long empCode))
                {
                    var employee = await _dbContext.ADEMPLOYEE
                        .Where(e => e.ADEMPCODE == empCode)
                        .Select(e => new { e.FIRSTNAME, e.LASTNAME })
                        .FirstOrDefaultAsync();

                    if (employee != null)
                    {
                        return $"{employee.FIRSTNAME} {employee.LASTNAME}".Trim();
                    }
                }

                return string.Empty; // Return empty string if not found or invalid format
            }
            catch (Exception ex)
            {
                // Log the error and return empty string
                // You might want to inject ILogger and log the actual error in production
                return string.Empty;
            }
        }

        public long Create(LostAndFound entity)
        {
            try
            {
                _dbContext.LostAndFound.Add(entity);
                _dbContext.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(LostAndFound entity)
        {
            try
            {
                var existingEntity = _dbContext.LostAndFound.Find(entity.Id);
                if (existingEntity == null)
                    return false;

                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool Delete(long id)
        {
            try
            {
                var entity = _dbContext.LostAndFound.Find(id);
                if (entity == null)
                    return false;

                entity.IsActive = 0;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<SelectListItmesVm> GetSiteList()
        {
            var iList = (from data in _dbContext.SYSITE
                         where data.ACTIVE == 1
                         select new SelectListItmesVm
                         {
                             Value = data.SYSITEID,
                             Text = data.DESCRIP,
                         }).OrderBy(o => o.Text).ToList();
            return iList;
        }




        //MAPPING SP

        public List<SelectListItmesVm> SearchEmployees(string term)
        {
            term = term?.Trim() ?? string.Empty;
            var query = _dbContext.ADEMPLOYEE.Where(e => e.ACTIVE == 1);
            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(e => e.FIRSTNAME.Contains(term) || e.LASTNAME.Contains(term) || e.ADEMPCODE.ToString().Contains(term));
            }

            return query
                .OrderBy(e => e.FIRSTNAME)
                .ThenBy(e => e.LASTNAME)
                .Take(50)
                .Select(e => new SelectListItmesVm
                {
                    Value = e.ADEMPCODE,
                    Text = e.FIRSTNAME + " " + e.LASTNAME + " [" + e.ADEMPCODE + "]"
                }).ToList();
        }


        public List<SecurityLocationMapViewModel> GetAllMappingSP()
        {
            var result = (from map in _dbContext.SECURITY_LOCATION_MAP
                          join emp in _dbContext.ADEMPLOYEE on map.EmpCode equals emp.ADEMPCODE
                          join site in _dbContext.SYSITE on map.SiteId equals site.SYSITEID
                          //where map.IsActive == 1
                          orderby map.SiteId
                          select new SecurityLocationMapViewModel
                          {
                              Id = map.Id,
                              SiteId = map.SiteId,
                              EmpCode = map.EmpCode,
                              IsActive = map.IsActive,
                              CreatedBy = map.CreatedBy,
                              CreatedDate = map.CreatedDate,
                              ModifiedBy = map.ModifiedBy,
                              ModifiedDate = map.ModifiedDate,

                              // extra display fields
                              SiteName = site.DESCRIP,
                              EmployeeName = emp.FIRSTNAME + " " + emp.LASTNAME
                          }).ToList();

            return result;
        }

        public SECURITY_LOCATION_MAP? GetByIdMappingSP(long id)
        {
            return _dbContext.SECURITY_LOCATION_MAP.FirstOrDefault(x => x.Id == id);
        }

        public bool ExistsForSite(long siteId)
        {
            return _dbContext.SECURITY_LOCATION_MAP.Any(x => x.SiteId == siteId && x.IsActive == 1);
        }

        public long CreateMappingSP(SECURITY_LOCATION_MAP entity)
        {
            var exists = _dbContext.SECURITY_LOCATION_MAP.Where(x => x.SiteId == entity.SiteId && x.EmpCode == entity.EmpCode && x.IsActive == 1).Count();

            if (exists > 0)
            {
                return 0;
            }
            _dbContext.SECURITY_LOCATION_MAP.Add(entity);
            _dbContext.SaveChanges();
            return 1;
        }

        public bool UpdateMappingSP(SECURITY_LOCATION_MAP entity)
        {
            try
            {
                var existing = _dbContext.SECURITY_LOCATION_MAP
                    .FirstOrDefault(x => x.Id == entity.Id);

                if (existing == null)
                    return false;

                // update properties manually (safer than SetValues)
                existing.SiteId = entity.SiteId;
                existing.EmpCode = entity.EmpCode;
                existing.IsActive = entity.IsActive;
                existing.ModifiedBy = entity.ModifiedBy;
                existing.ModifiedDate = DateTime.Now;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Don't use throw ex, just use throw to preserve stack trace
                throw;
            }
        }

        public List<SelectListItmesVm> GetAllEmployee()
        {
            var result = (from emp in _dbContext.ADEMPLOYEE
                          join assoc in _dbContext.VW_ASSOCIATELVLDETAILS
                              on emp.ADEMPCODE equals assoc.ADEMPCODE
                          where emp.ACTIVE == (short)1
                               
                          group emp by emp.ADEMPCODE into g
                          select new SelectListItmesVm
                          {
                              Value = g.Key,
                              Text = g.Key + " - " + g.FirstOrDefault().FIRSTNAME + " " + g.FirstOrDefault().LASTNAME
                          })
                        .ToList();

            return result;
        }

        public bool IsUserMappedAsSecurityPersonnel(long empCode)
        {
            return _dbContext.SECURITY_LOCATION_MAP
            .Count(x => x.EmpCode == empCode && x.IsActive == 1) > 0;
        }

        // New methods for security personnel workflow
        public List<LostAndFound> GetPendingItems(long userSiteId)
        {
            return _dbContext.LostAndFound
                .Where(x => x.IsActive == 1 && x.Status == 0 && x.LocationId == userSiteId) // Pending items only
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
        }

        public List<LostAndFound> GetAllItemsForSecurity(long userSiteId)
        {
            return _dbContext.LostAndFound.Include(x => x.Location)
                .Where(x => x.LocationId == userSiteId && x.Status != LostAndFoundStatus.Cancel)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
        }



        public string GetUserEmailByEmpCode(string empCode)
        {
            var employee = _dbContext.ADEMPLOYEE
                .Where(x => x.ADEMPCODE.ToString() == empCode && x.ACTIVE == 1)
                .FirstOrDefault();

            return employee?.EMAILID ?? string.Empty;
        }

        public List<LostAndFound> GetRequestByUserId(string userId)        
        {
            var data = (from lf in _dbContext.LostAndFound
                        join s in _dbContext.SYSITE
                            on lf.LocationId equals s.SYSITEID
                        where lf.IsActive == 1 && lf.CreatedBy == userId
                        orderby (lf.Status == 2 ? 0 : 1), lf.Id descending
                        select new LostAndFound
                        {
                            Id = lf.Id,
                            ItemType = lf.ItemType,
                            ItemName = lf.ItemName,
                            Location = s,
                            Date = lf.Date,
                            Status = lf.Status,
                            CreatedBy = lf.CreatedBy,
                        })
                        .ToList();

            return data;
        }

        public bool CancelRequestById(long id)
        {
            var entity = _dbContext.LostAndFound.Find(id);
            if (entity == null)
                return false;

            entity.IsActive = 0;
            entity.Status = 5;
            _dbContext.SaveChanges();
            return true;
        }

        public (string FullName, string DepartmentName, string Designation) GetEmployeeDetailsByEmpCode(long empCode)
        {
            //    var query =
            //(from empDivDept in _dbContext.ADEMPDIVDEPTSECT
            // join empLogin in _dbContext.ADEMPLOYEE
            //     on empDivDept.ADEMPCODE equals empLogin.ADEMPCODE
            // join department in _dbContext.ADORGLEVEL
            //     on empDivDept.ADORGLEVELID equals department.ADORGLEVELID
            // join designation2 in _dbContext.ADDESIGNATION
            //     on empDivDept.ADDESIGNATIONID equals designation2.ADDESIGNATIONID
            //     into designationGroup
            // from designation in designationGroup.DefaultIfEmpty()
            // where empDivDept.ADEMPCODE == empCode
            // select new
            // {
            //     FullName = empLogin.FIRSTNAME + " " + empLogin.LASTNAME,
            //     DepartmentName = department.LEVELDESCRIP,
            //     Designation = designation != null ? designation.DESCRIP : null
            // })
            //.FirstOrDefault();
            var SYkID =  _dbContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => _syki).FirstOrDefault();
            var query = _dbContext.VW_ASSOCIATELVLDETAILS
             .Join(_dbContext.ADEMPLOYEE,
                   vw => vw.ADEMPCODE,      // FK in VW_ASSOCIATELVLDETAILS
                   emp => emp.ADEMPCODE,    // PK in ADEMPLOYEE
                   (vw, emp) => new { vw, emp })
             .Join(_dbContext.ADDESIGNATION,
                   t => t.vw.ADDESIGNATIONID,  // designation id from VW_ASSOCIATELVLDETAILS
                   desig => desig.ADDESIGNATIONID,
                   (t, desig) => new { t.vw, t.emp, desig })
             .Where(x => x.vw.ADEMPCODE == empCode && x.vw.SYKI == _syki.SYKIID)
             .Select(x => new
             {
                 FullName = x.emp.FIRSTNAME + " " + x.emp.LASTNAME, 
                 DepartmentName = x.vw.DEPARTMENT,
                 Designation = x.desig.DESCRIP
             })
             .FirstOrDefault();


            if (query == null)
            {
                return ("", "", "");
            }

            return (query.FullName, query.DepartmentName, query.Designation);
        }

        public List<LostAndFound> GetPendingItemsOlderThanWorkingDays(int workingDays)
        {
            var items = _dbContext.LostAndFound
                .Where(x => x.IsActive == 1
                            && x.Status == LostAndFoundStatus.Pending)
                .OrderBy(x => x.CreatedDate)
                .ToList();

            var result = new List<LostAndFound>();

            foreach (var item in items)
            {
                // Calculate site-specific cutoff date
                var cutoffDate = GetWorkingDayThreshold(DateTime.Now, workingDays, item.LocationId);

                if (item.CreatedDate <= cutoffDate)
                {
                    result.Add(item);
                }
            }

            return result;
        }


        // Helper method to calculate working-day cutoff
        private DateTime GetWorkingDayThreshold(DateTime fromDate, int workingDays, long siteId)
        {
            int daysCounted = 0;
            DateTime tempDate = fromDate;

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // 🔹 Get all holidays (including weekly offs) for this site and current month
            var holidays = _dbContext.HMSIHOLIDAYS
                .Where(h => h.ACTIVE == 1
                         && h.SYSITEID == siteId
                         && h.MONTHDATEYEAR.Month == currentMonth
                         && h.MONTHDATEYEAR.Year == currentYear)
                .Select(h => h.MONTHDATEYEAR.Date)
                .ToList();

            // 🔹 Move backward and count only working days (not in HMSIHOLIDAYS)
            while (daysCounted < workingDays)
            {
                tempDate = tempDate.AddDays(-1);

                bool isHolidayOrWeeklyOff = holidays.Contains(tempDate.Date);

                if (!isHolidayOrWeeklyOff)
                {
                    daysCounted++;
                }
            }

            return tempDate;
        }


        public List<LostAndFound> GetLostAndFoundForExprotExcel(LostAndFoundExportModel model, long userSiteId)
        {
            var query = _dbContext.LostAndFound.AsQueryable();

            if (!string.IsNullOrEmpty(model.ItemType))
            {
                query = query.Where(x => x.ItemType == model.ItemType);
            }

            if (model.Status > 0)
            {
                query = query.Where(x => x.Status == model.Status);
            }

            // Date filter only if both DateFrom & DateTo are provided
            if (model.DateFrom.HasValue && model.DateTo.HasValue)
            {
                query = query.Where(x => x.Date >= model.DateFrom.Value && x.Date <= model.DateTo.Value);
            }

            query = query
                .Where(x =>
                    x.LocationId == userSiteId &&
                    x.Status != LostAndFoundStatus.Cancel).Include(x => x.Location)
                .OrderByDescending(x => x.CreatedDate);

            return query.ToList();
        }   
        


        public void AddLostAndFoundHistory(LostAndFoundHistory history)
        {
            try
            {
                if (history == null)
                    throw new ArgumentNullException(nameof(history));

                
                if (history.ADDEDDATE == default)
                    history.ADDEDDATE = DateTime.Now;

               
                if (history.CHANGEDDATE == default)
                    history.CHANGEDDATE = DateTime.Now;

                _dbContext.LostAndFoundHistory.Add(history);
                _dbContext.SaveChanges(); 

               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<LostAndFoundHistory> GetHitoryByItemid(long Id)
        {
            return _dbContext.LostAndFoundHistory
                .Where(x => x.LOST_AND_FOUND_ID == Id)
                .OrderByDescending(x => x.CHANGEDDATE)
                .ToList();
        }

        public Dictionary<string, string> GetEmployeeNamesByCodes(List<string> employeeCodes)
        {
            if (employeeCodes == null || employeeCodes.Count == 0)
                return new Dictionary<string, string>();

            // Parse all valid long codes
            var validCodes = employeeCodes
                .Where(code => long.TryParse(code, out _))
                .Select(code => Convert.ToInt64(code))
                .ToList();

            // Single DB query to fetch employees
            var employees = _dbContext.ADEMPLOYEE
                .Where(e => validCodes.Contains(e.ADEMPCODE))
                .Select(e => new
                {
                    e.ADEMPCODE,
                    e.FIRSTNAME,
                    e.LASTNAME
                })
                .ToList();

            // Convert to dictionary: Key = code string, Value = Full Name
            return employees.ToDictionary(
                e => e.ADEMPCODE.ToString(),
                e => $"{e.FIRSTNAME} {e.LASTNAME}".Trim()
            );
        }
        public List<long> GetSecurityMappedLocations(long empCode)
        {
            return _dbContext.SECURITY_LOCATION_MAP
                             .Where(x => x.EmpCode == empCode && x.IsActive == 1)
                             .Select(x => x.SiteId)
                             .ToList();
        }
    }
}