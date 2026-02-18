using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;

namespace ePortal.Infrastructure.Repositories
{
    public class RoleMasterRepository
    {
        private EPortalDBContext _RoleMasterRepositoryDBContext;
        public RoleMasterRepository(EPortalDBContext objRoleMasterRepositoryDBContext)
        {
            _RoleMasterRepositoryDBContext = objRoleMasterRepositoryDBContext;
            //_Syki = _DB.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
        }
        #region  ROLE MASTER
        public List<RoleViewModel> GetRoleList()
        {
            List<RoleViewModel> ilist = new List<RoleViewModel>();

            var data = (from t in _RoleMasterRepositoryDBContext.ADROLE_MST
                        join e in _RoleMasterRepositoryDBContext.ADPROCESS_MST on t.PROCESS_ID equals e.PROCESS_ID into table1
                        from d in table1
                        where t.PROCESS_ID == d.PROCESS_ID
                        select new
                        {
                            t.ROLE_ID,
                            t.ROLE_NAME,
                            d.PROCESS_NAME,
                            d.PROCESS_ID,
                            t.ROLE_BPO,
                            t.STATUS
                        }
            ).ToList();

            foreach (var itm in data)
            {
                ilist.Add(new RoleViewModel
                {
                    RoleId = long.Parse(itm.ROLE_ID.ToString()),
                    RoleName = itm.ROLE_NAME,
                    ProcessName = itm.PROCESS_NAME.ToString(),
                    RoleBPO = itm.ROLE_BPO,
                    Status = (itm.STATUS == 1) ? "Active" : "Inactive",
                    ProcessId = Convert.ToInt64(itm.PROCESS_ID)
                });
            }

            return ilist;

        }

        public short AddRoleData(RoleViewModel _model, string UserID)
        {

            short retVal = 0;
            int FlagAdd = 0;
            int max = 0;

            using (var transaction = _RoleMasterRepositoryDBContext.Database.BeginTransaction())
            {
                try
                {
                    var ctg = _RoleMasterRepositoryDBContext.ADROLE_MST.Where(x => x.ROLE_NAME == _model.RoleName).ToList();
                    if (ctg.Count() == 0)
                    {

                        var count = _RoleMasterRepositoryDBContext.ADROLE_MST.Count().ToString();
                        if (count == "0")
                        {
                            max = 1;
                        }
                        else
                        {
                            max = Int32.Parse(_RoleMasterRepositoryDBContext.ADROLE_MST.Max(i => i.ROLE_ID).ToString()) + 1;
                        }
                        FlagAdd = 1;


                        if (_model.RoleName != "")
                        {

                            ADROLE_MST MST = new ADROLE_MST();

                            MST.ROLE_ID = max;
                            MST.ROLE_NAME = _model.RoleName;
                            MST.PROCESS_ID = _model.ProcessId;
                            MST.ROLE_BPO = _model.RoleBPO;
                            MST.STATUS = Convert.ToDecimal(_model.Status);
                            MST.CREATED_DATE = DateTime.Now;
                            MST.CREATED_BY = Int32.Parse(UserID.ToString());
                            MST.MODIFIED_DATE = _model.Modified_Date;
                            MST.MODIFIED_BY = Int32.Parse(UserID.ToString());

                            _RoleMasterRepositoryDBContext.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _RoleMasterRepositoryDBContext.SaveChanges();

                            transaction.Commit();
                        }

                        retVal = 1;
                    }
                    else
                    {
                        retVal = 0;
                    }

                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                    throw ex;
                }

                return retVal;
            }
        }

        public List<RoleViewModel> GetProNameList()
        {
            List<RoleViewModel> ilist = new List<RoleViewModel>();

            var data = (from t in _RoleMasterRepositoryDBContext.ADPROCESS_MST
                        select new
                        {
                            t.PROCESS_ID,
                            t.PROCESS_NAME
                        }
            ).ToList();

            foreach (var itm in data)
            {
                ilist.Add(new RoleViewModel
                {
                    ProcessId = long.Parse(itm.PROCESS_ID.ToString()),
                    ProcessName = itm.PROCESS_NAME
                });
            }
            return ilist;

        }
        public dynamic GetEmpID(string Key)
        {
            dynamic Res;

            try
            {
                var elist = (from data in _RoleMasterRepositoryDBContext.ADEMPLOYEE
                             where (data.ADEMPCODE.ToString().ToUpper().Contains(Key.ToUpper())
                             || data.FIRSTNAME.ToUpper().Contains(Key.ToUpper())
                             || data.LASTNAME.ToUpper().Contains(Key.ToUpper()))
                             && data.ACTIVE == 1
                             select new
                             { name = data.SALUTATION + " " + data.FIRSTNAME + " " + data.LASTNAME, ecode = data.ADEMPCODE }
                        ).ToList();

                Res = elist;



            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Res;
        }

        public short EditRoleData(string RoleId, RoleViewModel _model, string UserID)
        {

            short retVal = 0;
            int FlagAdd = 0;
            int max = 0;

            using (var transaction = _RoleMasterRepositoryDBContext.Database.BeginTransaction())
            {
                try
                {
                    Decimal Role_Id = decimal.Parse(RoleId.ToString());
                    var MST = _RoleMasterRepositoryDBContext.ADROLE_MST.Where(x => x.ROLE_ID == Role_Id).FirstOrDefault();

                    MST.ROLE_ID = Role_Id;
                    MST.ROLE_NAME = _model.RoleName;
                    MST.PROCESS_ID = _model.ProcessId;
                    MST.ROLE_BPO = _model.RoleBPO;
                    MST.STATUS = Convert.ToDecimal(_model.Status);
                    MST.CREATED_DATE = DateTime.Now;
                    MST.CREATED_BY = Int32.Parse(UserID.ToString());
                    MST.MODIFIED_DATE = DateTime.Now;
                    MST.MODIFIED_BY = Int32.Parse(UserID.ToString());

                    _RoleMasterRepositoryDBContext.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _RoleMasterRepositoryDBContext.SaveChanges();

                    transaction.Commit();

                    retVal = 1;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                    throw ex;
                }

                return retVal;

            }
        }

        public dynamic AutocomplitName(long Ecode)
        {
            dynamic name;
            try
            {

                var N = (from data in _RoleMasterRepositoryDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == Ecode && v.ACTIVE == 1)
                         select
                            data.SALUTATION + " " + data.FIRSTNAME + " " + data.LASTNAME
                     ).FirstOrDefault();

                name = Ecode + " - " + N;


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return name;
        }
        #endregion
    }
}
