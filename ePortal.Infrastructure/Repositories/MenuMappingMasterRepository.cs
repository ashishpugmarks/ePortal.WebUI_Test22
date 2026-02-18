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
    public class MenuMappingMasterRepository
    {
        private EPortalDBContext _MenuMappingMasterDBContext;
        private decimal _Syki;


        public MenuMappingMasterRepository(EPortalDBContext objMenuMappingMasterDBContext)
        {
            _MenuMappingMasterDBContext = objMenuMappingMasterDBContext;
            _Syki = _MenuMappingMasterDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

        }

        public List<MenuMappingViewModel> GetMappingList()
        {
            List<MenuMappingViewModel> Ilist = new List<MenuMappingViewModel>();

            try
            {

                var Data = (from r in _MenuMappingMasterDBContext.ADMENUPARAMMAPPING_TRN
                            join k in _MenuMappingMasterDBContext.ADMENU_MST on r.MENUID equals k.MENU_ID
                            select new
                            {
                                r.MAPPINGID,
                                r.MENUID,
                                r.MENUPARAMID,
                                r.OPERATOR,
                                k.MENU_TEXT,
                                r.PARAM_VALUE,
                                k.STATUS
                            }).ToList();

                foreach (var item in Data)
                {
                    Ilist.Add(
                        new MenuMappingViewModel
                        {
                            MenuId = int.Parse(item.MENUID.ToString()),
                            MenuName = item.MENU_TEXT,
                            ParamVal = item.PARAM_VALUE.ToString(),
                            Operator = item.OPERATOR == "1" ? "Equal" : "Not Equal",
                            Status = item.STATUS == 1 ? "Active" : "DeActive",
                            MappingId = item.MAPPINGID,
                            MenuparamId = Convert.ToInt16(item.MENUPARAMID)
                        });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Ilist;
        }

        public List<MenuParamdata> GetMappingDataforGrid(Int64 MappingId)
        {
            List<MenuParamdata> mdata = new List<MenuParamdata>();
            List<MenuMappingViewModel> Ilist = new List<MenuMappingViewModel>();
            try
            {
                var Data = (from r in _MenuMappingMasterDBContext.ADMENUPARAMMAPPING_TRN
                            where r.MAPPINGID == MappingId
                            select new
                            {
                                r.MAPPINGID,
                                r.MENUPARAMID,
                                r.PARAM_VALUE
                            }).ToList();
                string[] arrPARAM_VALUE = Data[0].PARAM_VALUE.Split(',');
                var MenuParamID = Int64.Parse(Data[0].MENUPARAMID.ToString());
                var menuParam = _MenuMappingMasterDBContext.ADMENUPARAM_MST.FirstOrDefault(x => x.MENUPARAMID == MenuParamID);
                dynamic Res = null;

                foreach (string id in arrPARAM_VALUE)
                {
                    var mid = Int64.Parse(id);
                    if (menuParam != null)
                    {
                        switch (menuParam.TABLE_NAME)
                        {
                            case "ADORGLEVEL":
                                var dt = (from i in _MenuMappingMasterDBContext.ADORGLEVEL
                                          where i.ACTIVE == 1 && i.ADORGLEVELID == mid
                                          select new
                                          {
                                              value = i.ADORGLEVELID,
                                              text = i.LEVELDESCRIP,

                                          }).ToList();
                                if (dt.Count() != 0)
                                {
                                    mdata.Add(
                                        new MenuParamdata
                                        {
                                            value = dt[0].value,
                                            text = dt[0].text
                                        });
                                }
                                //var mpdt = _DB.ADORGLEVEL.Where(x=>x.ACTIVE == 1 && A)
                                break;

                            case "ADFUNCTIONALDESIGNATION":
                                var dt1 = ((from i in _MenuMappingMasterDBContext.ADFUNCTIONALDESIGNATION
                                            where i.ACTIVE == 1 && i.ADFUNCTIONALDESIGNATIONID == mid
                                            select new
                                            {
                                                value = i.ADFUNCTIONALDESIGNATIONID,
                                                text = i.DESCRIP
                                            }).ToList());

                                if (dt1.Count() != 0)
                                {
                                    mdata.Add(
                                        new MenuParamdata
                                        {
                                            value = dt1[0].value,
                                            text = dt1[0].text
                                        });
                                }

                                break;

                            case "ADDESIGNATION":
                                var dt2 = ((from i in _MenuMappingMasterDBContext.ADDESIGNATION
                                            where i.ACTIVE == 1 && i.ADDESIGNATIONID == mid
                                            select new
                                            {
                                                value = i.ADDESIGNATIONID,
                                                text = i.DESCRIP
                                            }).ToList());

                                if (dt2.Count() != 0)
                                {
                                    mdata.Add(
                                        new MenuParamdata
                                        {
                                            value = dt2[0].value,
                                            text = dt2[0].text
                                        });
                                }

                                break;

                            case "SYSITE":
                                var dt3 = ((from i in _MenuMappingMasterDBContext.SYSITE
                                            where i.ACTIVE == 1 && i.SYSITEID == mid
                                            select new
                                            {
                                                value = i.SYSITEID,
                                                text = i.DESCRIP
                                            }).ToList());

                                if (dt3.Count() != 0)
                                {
                                    mdata.Add(
                                        new MenuParamdata
                                        {
                                            value = dt3[0].value,
                                            text = dt3[0].text
                                        });
                                }

                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return Ilist;
            return mdata;
        }

        public short AddMenuMappingData(MenuMappingViewModel model_, string userID)
        {
            short retVal = 0;
            int FlagAdd = 0;
            int max = 0;

            using (var transaction = _MenuMappingMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    var ctg = _MenuMappingMasterDBContext.ADMENUPARAMMAPPING_TRN.Where(x => x.MAPPINGID == model_.MappingId).ToList();
                    if (ctg.Count() == 0)
                    {

                        var count = _MenuMappingMasterDBContext.ADMENUPARAMMAPPING_TRN.Count().ToString();
                        if (count == "0")
                        {
                            max = 1;
                        }
                        else
                        {
                            max = Int32.Parse(_MenuMappingMasterDBContext.ADMENUPARAMMAPPING_TRN.Max(i => i.MAPPINGID).ToString()) + 1;
                        }
                        FlagAdd = 1;


                        if (model_.MenuId != 0)
                        {

                            ADMENUPARAMMAPPING_TRN MST = new ADMENUPARAMMAPPING_TRN();

                            MST.MAPPINGID = max;
                            MST.MENUID = model_.MenuId;
                            MST.PARAM_VALUE = model_.ParamVal;
                            MST.OPERATOR = model_.Operator;
                            MST.STATUS = short.Parse(model_.Status);
                            MST.CREATED_DATE = DateTime.Now;
                            MST.CREATED_BY = Int32.Parse(userID.ToString());
                            MST.MODIFIED_DATE = model_.Modified_Date;
                            MST.MODIFIED_BY = Int32.Parse(userID.ToString());
                            MST.MENUPARAMID = model_.MenuparamId;

                            _MenuMappingMasterDBContext.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _MenuMappingMasterDBContext.SaveChanges();

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

                var asta = (from _ITDIncomeTaxDetail in _MenuMappingMasterDBContext.ADMENUPARAMMAPPING_TRN select _ITDIncomeTaxDetail).ToList();
                List<MenuMappingViewModel> view = new List<MenuMappingViewModel>();

                return retVal;
            }
        }

        public short EditMenuMappingData(string mapid, MenuMappingViewModel _model, string UserID)
        {

            short retVal = 0;
            int FlagAdd = 0;

            using (var transaction = _MenuMappingMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    var Mappingid = Int64.Parse(mapid.ToString());
                    var MST = _MenuMappingMasterDBContext.ADMENUPARAMMAPPING_TRN.Where(x => x.MAPPINGID == Mappingid).FirstOrDefault();

                    FlagAdd = 0;

                    MST.MAPPINGID = Mappingid;
                    //MST.MENUID = _model.MenuId;
                    MST.PARAM_VALUE = _model.ParamVal;
                    MST.OPERATOR = _model.Operator;
                    MST.STATUS = short.Parse(_model.Status);
                    MST.CREATED_DATE = DateTime.Now;
                    MST.CREATED_BY = Int32.Parse(UserID.ToString());
                    MST.MODIFIED_DATE = _model.Modified_Date;
                    MST.MODIFIED_BY = Int32.Parse(UserID.ToString());
                    MST.MENUPARAMID = _model.MenuparamId;

                    _MenuMappingMasterDBContext.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _MenuMappingMasterDBContext.SaveChanges();

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

        public dynamic GetADMENUPARAM_MSTLIst()
        {
            dynamic Res;

            try
            {
                var elist = (from data in _MenuMappingMasterDBContext.ADMENUPARAM_MST
                             select new
                             { value = data.MENUPARAMID, text = data.PARAMETER_NAME }
                        ).ToList();

                Res = elist;

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Res;
        }

        public dynamic GetSuggesionValue(string Key, int ParamType)
        {
            dynamic Res = null;

            try
            {
                var menuParam = _MenuMappingMasterDBContext.ADMENUPARAM_MST.FirstOrDefault(x => x.MENUPARAMID == ParamType);
                if (menuParam != null)
                {
                    switch (menuParam.TABLE_NAME)
                    {
                        case "ADORGLEVEL":
                            Res = (from i in _MenuMappingMasterDBContext.ADORGLEVEL
                                   where i.ACTIVE == 1 && i.LEVELDESCRIP.ToString().ToUpper().Contains(Key.ToUpper())
                                   select new
                                   {
                                       value = i.ADORGLEVELID,
                                       text = i.LEVELDESCRIP
                                   }).ToList();
                            break;

                        case "ADFUNCTIONALDESIGNATION":
                            Res = (from i in _MenuMappingMasterDBContext.ADFUNCTIONALDESIGNATION
                                   where i.ACTIVE == 1 && i.DESCRIP.ToString().ToUpper().Contains(Key.ToUpper())
                                   select new
                                   {
                                       value = i.ADFUNCTIONALDESIGNATIONID,
                                       text = i.DESCRIP
                                   }).ToList();
                            break;

                        case "ADDESIGNATION":
                            Res = (from i in _MenuMappingMasterDBContext.ADDESIGNATION
                                   where i.ACTIVE == 1 && i.DESCRIP.ToString().ToUpper().Contains(Key.ToUpper())
                                   select new
                                   {
                                       value = i.ADDESIGNATIONID,
                                       text = i.DESCRIP
                                   }).ToList();
                            break;

                        case "SYSITE":
                            Res = (from i in _MenuMappingMasterDBContext.SYSITE
                                   where i.ACTIVE == 1 && i.DESCRIP.ToString().ToUpper().Contains(Key.ToUpper())
                                   select new
                                   {
                                       value = i.SYSITEID,
                                       text = i.DESCRIP
                                   }).ToList();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Res;
        }


        public dynamic GetMenuName(string Key)
        {
            dynamic Res;

            try
            {
                var elist = (from data in _MenuMappingMasterDBContext.ADMENU_MST
                             where data.MENU_TEXT.ToString().ToUpper().Contains(Key.ToUpper())
                             select new
                             { MenuName = data.MENU_TEXT, MenuID = data.MENU_ID }
                        ).ToList();

                Res = elist;



            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Res;
        }

        //public List<MenuMappingViewModel> GetParameterName()
        //{
        //    List<MenuMappingViewModel> ilist = new List<MenuMappingViewModel>();

        //    var data = (from t in _DB.ADMENUPARAM_MST
        //                select new
        //                {
        //                    t.MENUPARAMID,
        //                    t.PARAMETER_NAME
        //                }
        //    ).ToList();

        //    foreach (var itm in data)
        //    {
        //        ilist.Add(new MenuMappingViewModel
        //        {
        //            MenuparamId = Convert.ToInt16(itm.MENUPARAMID),
        //            MenuParamName = itm.PARAMETER_NAME
        //        });
        //    }
        //    return ilist;
        //}

    }
}
