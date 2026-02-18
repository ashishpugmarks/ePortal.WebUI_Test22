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
    public class RoleUserMappingMasterRepository
    {
        private EPortalDBContext _EPortalDBContext;
        public RoleUserMappingMasterRepository(EPortalDBContext objEPortalDBContext)
        {
            _EPortalDBContext = objEPortalDBContext;
            //_Syki = _DB.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
        }

        #region  ROLE MASTER
        public List<RoleUserMappingViewModel> GetRoleUserMappingList()
        {
            List<RoleUserMappingViewModel> ilist = new List<RoleUserMappingViewModel>();


            var data = (from t in _EPortalDBContext.ADROLEUSER_MAPPING_TRN
                        join e in _EPortalDBContext.ADROLE_MST on t.ROLE_ID equals e.ROLE_ID into table1
                        from d in table1
                        join a in _EPortalDBContext.ADEMPLOYEE on t.USER_ID equals a.ADEMPCODE into table2
                        from u in table2
                        where t.ROLE_ID == d.ROLE_ID
                        select new
                        {
                            t.MAPPING_ID,
                            t.ROLE_ID,
                            d.ROLE_NAME,
                            t.USER_ID,
                            u.SALUTATION,
                            u.FIRSTNAME,
                            u.LASTNAME,
                            t.STATUS
                        }
            ).ToList();

            foreach (var itm in data)
            {

                ilist.Add(new RoleUserMappingViewModel
                {
                    MappingId = long.Parse(itm.MAPPING_ID.ToString()),
                    RoleId = long.Parse(itm.ROLE_ID.ToString()),
                    RoleName = itm.ROLE_NAME,
                    UserId = itm.USER_ID,
                    UserName = itm.SALUTATION + " " + itm.FIRSTNAME + " " + itm.LASTNAME,
                    Status = (itm.STATUS == 1) ? "Active" : "Inactive",
                    //ProcessId = Convert.ToInt64(itm.PROCESS_ID)
                });
            }

            return ilist;

        }

        public short EditRoleUserMappingData(string mappingid, RoleUserMappingViewModel savedata, string userId)
        {
            short retVal = 0;
            int FlagAdd = 0;

            using (var transaction = _EPortalDBContext.Database.BeginTransaction())
            {
                try
                {
                    Decimal Mapping_Id = decimal.Parse(mappingid.ToString());
                    var MST = _EPortalDBContext.ADROLEUSER_MAPPING_TRN.Where(x => x.MAPPING_ID == Mapping_Id).FirstOrDefault();

                    MST.MAPPING_ID = Mapping_Id;
                    MST.USER_ID = savedata.UserId;
                    MST.ROLE_ID = savedata.RoleId;
                    MST.STATUS = Convert.ToDecimal(savedata.Status);
                    MST.CREATED_DATE = DateTime.Now;
                    MST.CREATED_BY = Int32.Parse(userId.ToString());
                    MST.MODIFIED_DATE = DateTime.Now;
                    MST.MODIFIED_BY = Int32.Parse(userId.ToString());

                    _EPortalDBContext.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _EPortalDBContext.SaveChanges();

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

        public short AddRoleUserMappingData(RoleUserMappingViewModel savedata, string userId)
        {
            short retVal = 0;
            int FlagAdd = 0;
            int max = 0;

            using (var transaction = _EPortalDBContext.Database.BeginTransaction())
            {
                try
                {
                    var ctg = _EPortalDBContext.ADROLEUSER_MAPPING_TRN.Where(x => x.USER_ID == savedata.UserId && x.ROLE_ID == savedata.RoleId).ToList();
                    if (ctg.Count() == 0)
                    {

                        var count = _EPortalDBContext.ADROLEUSER_MAPPING_TRN.Count().ToString();
                        if (count == "0")
                        {
                            max = 1;
                        }
                        else
                        {
                            max = Int32.Parse(_EPortalDBContext.ADROLEUSER_MAPPING_TRN.Max(i => i.MAPPING_ID).ToString()) + 1;
                        }
                        FlagAdd = 1;


                        if (savedata.UserName != "")
                        {

                            ADROLEUSER_MAPPING_TRN MST = new ADROLEUSER_MAPPING_TRN();

                            MST.MAPPING_ID = max;
                            MST.ROLE_ID = savedata.RoleId;
                            MST.USER_ID = savedata.UserId;
                            MST.STATUS = Convert.ToDecimal(savedata.Status);
                            MST.CREATED_DATE = DateTime.Now;
                            MST.CREATED_BY = Int32.Parse(userId.ToString());
                            MST.MODIFIED_DATE = savedata.Modified_Date;
                            MST.MODIFIED_BY = Int32.Parse(userId.ToString());

                            _EPortalDBContext.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _EPortalDBContext.SaveChanges();

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

        public List<RoleUserMappingViewModel> GetRoleNameList()
        {
            List<RoleUserMappingViewModel> ilist = new List<RoleUserMappingViewModel>();

            var data = (from t in _EPortalDBContext.ADROLE_MST
                        select new
                        {
                            t.ROLE_ID,
                            t.ROLE_NAME
                        }
            ).ToList();

            foreach (var itm in data)
            {
                ilist.Add(new RoleUserMappingViewModel
                {
                    RoleId = long.Parse(itm.ROLE_ID.ToString()),
                    RoleName = itm.ROLE_NAME
                });
            }
            return ilist;

        }

        #endregion
    }
}
