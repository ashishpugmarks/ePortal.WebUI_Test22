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
    public class MenuMasterRepository
    {
        private EPortalDBContext _DB;
        private decimal _Syki;


        public static class DefaultValues
        {
            public const int MaxSrnoHead = 1;
            public const int MaxLcTranId = 2001;
            public const int MaxSrnoDt = 1;

        }
        public MenuMasterRepository(EPortalDBContext objDB)
        {
            _DB = objDB;
            _Syki = _DB.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
        }



        #region  MENU MASTER

        public List<MenuViewModel> GetMenuList(string flag)
        {
            List<MenuViewModel> ilist = new List<MenuViewModel>();

            var data = (from t in _DB.ADMENU_MST
                        orderby t.MENU_TEXT
                        select new
                        {
                            t.MENU_ID,
                            t.MENU_TEXT,
                            t.MENU_TOOLTIP,
                            t.MENU_TYPE,
                            t.STATUS,
                            t.MENU_URL
                        }

            ).ToList();

            foreach (var itm in data)
            {
                ilist.Add(new MenuViewModel
                {
                    MenuId = long.Parse(itm.MENU_ID.ToString()),
                    Title = flag == "ddl" ? itm.MENU_ID.ToString() + " - " + itm.MENU_TEXT : itm.MENU_TEXT,
                    ToolTip = itm.MENU_TOOLTIP,
                    MenuType = (itm.MENU_TYPE == 1) ? "User Level" : "Admin Level",
                    Status = (itm.STATUS == 1) ? "Active" : "Inactive",
                    URL = (itm.MENU_URL == null) ? "" : itm.MENU_URL


                });
            }
            return ilist;

        }

        public List<MenuViewModel> GetParentName(decimal id)
        {
            List<MenuViewModel> ilist = new List<MenuViewModel>();

            var data = (from t in _DB.ADMENU_MST
                        where t.MENU_PARENT_ID == id
                        select new
                        {
                            t.MENU_TEXT,
                        }
            ).FirstOrDefault();

            if (id != 0)
            {
                ilist.Add(new MenuViewModel
                {
                    ParentMenu_text = data.MENU_TEXT.ToString(),

                });
            }
            else
            {
                ilist.Add(new MenuViewModel
                {
                    ParentMenu_text = ""
                });
            }


            return ilist;
        }

        public short AddMenuData(MenuViewModel _model, string UserID)
        {

            short retVal = 0;
            int FlagAdd = 0;
            int max = 0;

            using (var transaction = _DB.Database.BeginTransaction())
            {
                try
                {
                    var MenuType = Convert.ToDecimal(_model.MenuType);
                    var ctg = _DB.ADMENU_MST.Where(x => x.MENU_TEXT == _model.Title && x.MENU_TYPE == MenuType && x.MENU_PARENT_ID == x.MENU_PARENT_ID).ToList();
                    if (ctg.Count() == 0)
                    {

                        var count = _DB.ADMENU_MST.Count().ToString();
                        if (count == "0")
                        {
                            max = 1;
                        }
                        else
                        {

                            max = Int32.Parse(_DB.ADMENU_MST.Max(i => i.MENU_ID).ToString()) + 1;

                        }
                        FlagAdd = 1;


                        if (_model.Title != "" && _model.Status != "")
                        {

                            ADMENU_MST MST = new ADMENU_MST();

                            MST.MENU_ID = max;
                            MST.MENU_TEXT = _model.Title;
                            MST.MENU_TOOLTIP = _model.ToolTip;
                            MST.MENU_TYPE = Convert.ToDecimal(_model.MenuType);
                            MST.MENU_TARGET = Convert.ToInt16(_model.MenuTarget);
                            MST.MENU_PARENT_ID = _model.MenuParentId;
                            MST.MENU_DISPLAY_ORDER = _model.MenuOrder;
                            MST.MENU_LEVEL = _model.MenuLevel;
                            MST.MENUICON_FILENAME = _model.MenuIconFileName;
                            MST.MENUICON_CONTENT_TYPE = _model.MenuContentType;
                            MST.MENU_URL = _model.URL;
                            MST.MENU_DISPLAY = Convert.ToInt16(_model.MenuDisaplay);
                            MST.STATUS = Convert.ToDecimal(_model.Status);
                            MST.ISDEFAULTMENU = Convert.ToInt16(_model.IsDefaultMenu);
                            MST.MENUICON = _model.MenuIcon;
                            MST.INTERNET = Convert.ToInt16(_model.Internet);
                            MST.CREATED_BY = Int32.Parse(UserID.ToString());
                            MST.CREATED_DATE = DateTime.Now;
                            MST.MENU_CONTROLLER = _model.Controller;
                            MST.MENU_ACTION = _model.Action;
                            MST.MODIFIED_BY = Convert.ToInt64(UserID);
                            MST.MODIFIED_DATE = _model.Modified_Date;

                            _DB.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _DB.SaveChanges();

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

        public List<MenuViewModel> GetMenuData()
        {
            List<MenuViewModel> ilist = new List<MenuViewModel>();

            var data = (from t in _DB.ADMENU_MST
                            //where t.STATUS == 1
                        select new
                        {
                            t.MENU_ID,
                            t.MENU_TYPE,
                            t.MENU_TEXT,
                            t.MENU_TOOLTIP,
                            t.MENU_LEVEL,
                            t.MENU_DISPLAY_ORDER,
                            t.MENU_DISPLAY,
                            t.MENU_TARGET,
                            t.STATUS,
                            t.MENUICON,
                            t.MENUICON_CONTENT_TYPE,
                            t.MENUICON_FILENAME,
                            t.INTERNET,
                            t.MENU_URL,
                            t.MENU_PARENT_ID,
                            t.ISDEFAULTMENU

                        }
            ).ToList();

            foreach (var itm in data)
            {
                ilist.Add(new MenuViewModel
                {
                    MenuId = long.Parse(itm.MENU_ID.ToString()),
                    Title = itm.MENU_TEXT,
                    ToolTip = itm.MENU_TOOLTIP,
                    MenuType = (itm.MENU_TYPE == 1) ? "User Level" : "Admin Level",
                    Status = (itm.STATUS == 1) ? "Active" : "Inactive",
                    URL = (itm.MENU_URL == null) ? "" : itm.MENU_URL,
                    MenuLevel = int.Parse(itm.MENU_LEVEL.ToString()),
                    MenuOrder = Convert.ToInt16(itm.MENU_DISPLAY_ORDER),
                    MenuDisaplay = itm.MENU_DISPLAY.ToString(),
                    MenuTarget = itm.MENU_TARGET.ToString(),
                    MenuIcon = itm.MENUICON,
                    MenuContentType = itm.MENUICON_CONTENT_TYPE,
                    MenuIconFileName = itm.MENUICON_FILENAME,
                    Internet = Convert.ToInt16(itm.INTERNET),
                    MenuParentId = Convert.ToInt64(itm.MENU_PARENT_ID),
                    IsDefaultMenu = Convert.ToInt16(itm.ISDEFAULTMENU)

                });
            }

            return ilist;

        }

        public short EditMenuData(string MenuId, MenuViewModel _model, string UserID)
        {

            short retVal = 0;
            int FlagAdd = 0;

            using (var transaction = _DB.Database.BeginTransaction())
            {
                try
                {
                    Decimal menu_id = decimal.Parse(MenuId.ToString());
                    var MST = _DB.ADMENU_MST.Where(x => x.MENU_ID == menu_id).FirstOrDefault();

                    FlagAdd = 0;

                    MST.MENU_ID = menu_id;
                    MST.MENU_TEXT = _model.Title;
                    MST.MENU_TOOLTIP = _model.ToolTip;
                    MST.MENU_TYPE = Convert.ToDecimal(_model.MenuType);
                    MST.MENU_TARGET = Convert.ToInt16(_model.MenuTarget);
                    MST.MENU_PARENT_ID = _model.MenuParentId;
                    MST.MENU_DISPLAY_ORDER = _model.MenuOrder;
                    MST.MENU_LEVEL = _model.MenuLevel;
                    MST.MENUICON_FILENAME = _model.MenuIconFileName;
                    MST.MENUICON_CONTENT_TYPE = _model.MenuContentType;
                    MST.MENU_URL = _model.URL;
                    MST.MENU_DISPLAY = Convert.ToInt16(_model.MenuDisaplay);
                    MST.STATUS = Convert.ToDecimal(_model.Status);
                    MST.ISDEFAULTMENU = Convert.ToInt16(_model.IsDefaultMenu);
                    MST.MENUICON = _model.MenuIcon;
                    MST.INTERNET = Convert.ToInt16(_model.Internet);
                    MST.CREATED_BY = Int32.Parse(UserID.ToString());
                    MST.CREATED_DATE = DateTime.Now;
                    MST.MENU_CONTROLLER = _model.Controller;
                    MST.MENU_ACTION = _model.Action;
                    MST.MODIFIED_BY = Convert.ToInt64(UserID);
                    MST.MODIFIED_DATE = _model.Modified_Date;

                    _DB.Entry(MST).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _DB.SaveChanges();

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


        public int ParentMenu(decimal MenuType, decimal PerentMenu, decimal MenuLevel, int LoginCode)
        {
            try
            {
                //List<MenuViewModel> Data = new List<MenuViewModel>();

                var Data = _DB.ADMENU_MST.Where(r => r.MENU_TYPE == MenuType && r.MENU_PARENT_ID == PerentMenu
                && r.MENU_LEVEL == MenuLevel).Select(a => a.MENU_DISPLAY_ORDER)
                .Max() + 1;

                var max = int.Parse(Data.ToString());

                return max;

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }
    }
}
#endregion
