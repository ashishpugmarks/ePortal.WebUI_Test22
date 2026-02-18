using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;

namespace ePortal.Infrastructure.Repositories
{
    public class BirthdayListRepository
    {
        private EPortalDBContext _POADBContext;
        //private readonly ISessionService _sessionService;
        //private ePortalEntities2 _POADBContext;
        private SYKI _Syki;
        public BirthdayListRepository(EPortalDBContext POADBContext)
        {
            _POADBContext = POADBContext;
            //_sessionService = sessionService;
            _Syki = _POADBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public List<Root> GetBirthdayListData()
        {
            try
            {
                var today = DateTime.Now;
                var _Syki_ = _Syki.SYKIID;
                var query = (
                    from post in _POADBContext.ADEMPLOYEE
                    join meta in _POADBContext.VW_ASSOCIATELVLDETAILS on post.ADEMPCODE equals meta.ADEMPCODE
                    where post.DOB.HasValue && post.DOB.Value.Month == today.Month && post.DOB.Value.Day == today.Day && post.ACTIVE == 1 && meta.ACTIVE == 1 && meta.SYKI == _Syki_
                    select new Root
                    {
                        ADEMPCODE = post.FIRSTNAME + " " + post.LASTNAME,
                        ADEMPCODE_ = post.ADEMPCODE,
                        operationID = meta.OPERATIONID,
                        operation = meta.OPERATION,
                        DivisionID = meta.DIVISIONID,
                        Division = meta.DIVISION,
                        DepartmentID = meta.DEPARTMENTID,
                        Department = meta.DEPARTMENT,
                        SectionID = meta.SECTIONID,
                        //Section = meta.SECTION == null ? "" : meta.SECTION,
                        Section = meta.SECTION == null ? "" : meta.SECTION,

                    }
                ).ToList();


                return query;
            }
            catch (Exception ex)
            {

                throw;
            }



        }
    }
}
