using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;

namespace ePortal.Infrastructure.Repositories
{
    public class KaizenRepository
    {
        private EPortalDBContext _KaizenContext;        
        public KaizenRepository(EPortalDBContext KaizenContext)
        {
            _KaizenContext= KaizenContext;
        }

        public bool isPopupEnableForDeptCommittee(long ecode)
        {
            var a = DateTime.Now.Date;
            long months = Convert.ToInt64(a.Month > 10 ? a.Month.ToString() : '0' + a.Month.ToString());
            var result = (from data in _KaizenContext.KAIZEN_DEPT_COMMITTEE_USERS
                          where data.ADEMPCODE == ecode && data.ADDEDDATE.Value.Month == months && data.TOTALSCORE == 0 && data.STATUS != 0
                          select new
                          {

                          }).ToList();

            if (result.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool isPopupEnableForDivCommittee(long ecode)
        {
            var a = DateTime.Now.Date;
            long months = Convert.ToInt64(a.Month > 10 ? a.Month.ToString() : '0' + a.Month.ToString());
            var result = (from data in _KaizenContext.KAIZEN_DIV_COMMITTEE_USERS
                          where data.ADEMPCODE == ecode && data.ADDEDDATE.Value.Month == months && data.TOTALSCORE == 0 && data.STATUS != 0
                          select new
                          {

                          }).ToList();

            if (result.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
