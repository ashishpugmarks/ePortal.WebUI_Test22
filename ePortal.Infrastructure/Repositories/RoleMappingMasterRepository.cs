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
    public class RoleMappingMasterRepository
    {
        private EPortalDBContext _RoleMappingMasterDBContext;
        public RoleMappingMasterRepository(EPortalDBContext objRoleMappingMasterDBContext)
        {
            _RoleMappingMasterDBContext = objRoleMappingMasterDBContext;
        }

        public List<RoleMappingViewModel> GetRoleMappingList()
        {
            List<RoleMappingViewModel> ilist = new List<RoleMappingViewModel>();

            var data = (from t in _RoleMappingMasterDBContext.ADMENUROLE_MAPPING_MST
                        join e in _RoleMappingMasterDBContext.ADMENU_MST on t.MENU_ID equals e.MENU_ID into table1
                        from d in table1
                        join r in _RoleMappingMasterDBContext.ADROLE_MST on t.ROLE_ID equals r.ROLE_ID into table2
                        from a in table2
                        where t.MENU_ID == d.MENU_ID
                        select new
                        {
                            t.MAPPING_ID,
                            t.MENU_ID,
                            t.ROLE_ID,
                            t.STATUS,
                            d.MENU_TEXT,
                            a.ROLE_NAME,
                        }
            ).ToList();

            foreach (var itm in data)
            {
                ilist.Add(new RoleMappingViewModel
                {
                    MappingId = Convert.ToInt64(itm.MAPPING_ID),
                    MenuName = itm.MENU_TEXT,
                    RoleName = itm.ROLE_NAME,
                    Status = (itm.STATUS == 1) ? "Active" : "Inactive",
                    MenuId = Convert.ToInt64(itm.MENU_ID),
                    RoleId = Convert.ToInt64(itm.ROLE_ID)
                });
            }

            return ilist;
        }

        public short AddRoleMappingData(RoleMappingViewModel model_, string userID)
        {
            short retVal = 0;
            int FlagAdd = 0;
            int max = 0;

            using (var transaction = _RoleMappingMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    var ctg = _RoleMappingMasterDBContext.ADMENUROLE_MAPPING_MST.Where(x => x.MENU_ID == model_.MenuId).ToList();
                    if (ctg.Count() == 0)
                    {
                        var count = _RoleMappingMasterDBContext.ADMENUROLE_MAPPING_MST.Count().ToString();
                        if (count == "0")
                        {
                            max = 1;
                        }
                        else
                        {
                            max = Int32.Parse(_RoleMappingMasterDBContext.ADMENUROLE_MAPPING_MST.Max(i => i.MAPPING_ID).ToString()) + 1;
                        }
                        FlagAdd = 1;


                        if (model_.MenuId != 0)
                        {

                            ADMENUROLE_MAPPING_MST MST = new ADMENUROLE_MAPPING_MST();

                            MST.MAPPING_ID = max;
                            MST.MENU_ID = model_.MenuId;
                            MST.ROLE_ID = model_.RoleId;
                            MST.STATUS = Convert.ToDecimal(model_.Status);
                            MST.CREATED_DATE = DateTime.Now;
                            MST.CREATED_BY = Int32.Parse(userID.ToString());
                            MST.MODIFIED_DATE = model_.Modified_Date;
                            MST.MODIFIED_BY = Int32.Parse(userID.ToString());

                            _RoleMappingMasterDBContext.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _RoleMappingMasterDBContext.SaveChanges();

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

        public dynamic GetMenuId(string key)
        {
            dynamic Res;

            try
            {
                var elist = (from data in _RoleMappingMasterDBContext.ADMENU_MST
                             where (data.MENU_ID.ToString().ToUpper().Contains(key.ToUpper())
                             || data.MENU_TEXT.ToUpper().Contains(key.ToUpper()))
                             select new
                             { name = data.MENU_TEXT + " " + data.MENU_TEXT, mid = data.MENU_ID }
                        ).ToList();

                Res = elist;
            }
            catch (Exception ex)
            {

                throw;
            }
            return Res;
        }
        //public List<object> GetMenuId(string key)
        //{
        //    try
        //    {
        //        int menuId;
        //        bool isNumeric = int.TryParse(key, out menuId);

        //        return (from data in _RoleMappingMasterDBContext.ADMENU_MST
        //                where (isNumeric && data.MENU_ID == menuId)
        //                   || data.MENU_TEXT.ToUpper().Contains(key.ToUpper())
        //                select new
        //                {
        //                    name = data.MENU_TEXT + " " + data.MENU_TEXT,
        //                    mid = data.MENU_ID
        //                }).ToList<object>();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        public List<RoleMappingViewModel> GetRoleList()
        {
            List<RoleMappingViewModel> ilist = new List<RoleMappingViewModel>();
            var data = (from t in _RoleMappingMasterDBContext.ADROLE_MST
                        select new
                        {
                            t.ROLE_ID,
                            t.ROLE_NAME
                        }
            ).ToList();

            foreach (var itm in data)
            {
                ilist.Add(new RoleMappingViewModel
                {
                    RoleId = Convert.ToInt64(itm.ROLE_ID),
                    RoleName = itm.ROLE_NAME
                });
            }

            return ilist;
        }

        public short EditRoleMappingData(string MappingId, RoleMappingViewModel model_, string userID)
        {
            short retVal = 0;
            int FlagAdd = 0;

            using (var transaction = _RoleMappingMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    Decimal Mapping_Id = decimal.Parse(MappingId.ToString());
                    var MST = _RoleMappingMasterDBContext.ADMENUROLE_MAPPING_MST.Where(x => x.MAPPING_ID == Mapping_Id).FirstOrDefault();


                    if (MST == null)
                    {
                        retVal = -2; // or any other code to indicate "not found"
                        transaction.Rollback();
                        return retVal;
                    }


                    MST.MAPPING_ID = Mapping_Id;
                    MST.MENU_ID = model_.MenuId;
                    MST.ROLE_ID = model_.RoleId;
                    MST.STATUS = Convert.ToDecimal(model_.Status);
                    MST.CREATED_DATE = DateTime.Now;
                    MST.CREATED_BY = Int32.Parse(userID.ToString());
                    MST.MODIFIED_DATE = DateTime.Now;
                    MST.MODIFIED_BY = Int32.Parse(userID.ToString());

                    _RoleMappingMasterDBContext.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _RoleMappingMasterDBContext.SaveChanges();

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

        public dynamic AutocomplitName(long menuid)
        {
            dynamic name;
            try
            {

                var N = (from data in _RoleMappingMasterDBContext.ADMENU_MST.Where(v => v.MENU_ID == menuid)
                         select
                            data.MENU_TEXT
                     ).FirstOrDefault();

                name = menuid + " - " + N;


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return name;
        }
    }
}
