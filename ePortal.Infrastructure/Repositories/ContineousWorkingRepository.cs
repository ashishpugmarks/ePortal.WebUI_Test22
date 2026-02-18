using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ePortal.ViewModels.SearchParameterList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class ContineousWorkingRepository
    {
        private EPortalDBContext _AcrDBContext;
        public ContineousWorkingRepository(EPortalDBContext objEPortalDBContext)
        {
            _AcrDBContext = objEPortalDBContext;

        }
        public List<ContineousWorkingRowViewModel> ContineousWorkingDashboard(SearchContineousWorkingViewModel SWM)
        {
            try
            {
                DateTime? dateTo = null;
                if (!string.IsNullOrWhiteSpace(SWM.DATETO))
                    dateTo = DateTime.Parse(SWM.DATETO);                    
                var result = (
                    from c in _AcrDBContext.CONTINUOUSATTENDANCELOG
                    join e in _AcrDBContext.ADEMPLOYEE
                        on c.ADEMPCODE equals e.ADEMPCODE
                    join d in _AcrDBContext.VW_ASSOCIATELVLDETAILS
                        on c.ADEMPCODE equals d.ADEMPCODE
                    join g in _AcrDBContext.ADDESIGNATION
                        on d.ADDESIGNATIONID equals g.ADDESIGNATIONID into gJoin
                    from gData in gJoin.DefaultIfEmpty()
                    join s in _AcrDBContext.SYSITE
                         on d.SYPLANTID equals s.SYSITEID into sJoin
                    from sData in sJoin.DefaultIfEmpty()

                    where
                        (dateTo == null || c.TDATE.Date == dateTo.Value.Date)                       
                    select new ContineousWorkingRowViewModel
                    {
                        ADEMPCODE = c.ADEMPCODE,
                        ASSOCIATE_NAME = e.FIRSTNAME + " " + e.LASTNAME,

                        OPERATION = d.OPERATION,
                        DIVISION = d.DIVISION,
                        DEPARTMENT = d.DEPARTMENT,
                        SECTION = d.SECTION,

                        DESIGNATION = gData.DESCRIP,
                        SITE_DESCRIP = sData != null ? sData.DESCRIP : "",
                        //LOCATION = d.SYPLANTID == 1 ? "1F" :
                        //           d.SYPLANTID == 2 ? "2F" :
                        //           d.SYPLANTID == 3 ? "3F" :
                        //           d.SYPLANTID == 4 ? "4F" :
                        //           d.SYPLANTID == 5 ? "HO" :
                        //           d.SYPLANTID == 0 ? "HO" : "HO",

                        STATUS = d.ACTIVE == 1 ? "Active" : "Inactive",

                        PUNCHDATE = c.TDATE.AddDays(-(c.TDAYS ?? 1) + 1).ToString("dd-MMM-yyyy"),
                        DATETO = c.TDATE.ToString("dd-MMM-yyyy"),

                        CONTINUOUSDAYS = c.TDAYS ?? 0
                    }
                )
                .OrderBy(x => x.SITE_DESCRIP)
                .ThenBy(x => x.DIVISION)
                .ThenBy(x => x.DEPARTMENT)
                .ThenBy(x => x.SECTION)
                .ThenBy(x => x.OPERATION)
                .ThenBy(x => x.ADEMPCODE)
                .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
