using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class HomeRepository
    {
        private EPortalDBContext _empLoginDBContext;
        private readonly ISessionService _sessionService;
        private readonly EmployeeLoginRepository _empRepo;
        private readonly IConfiguration _configuration;
        private string moveToOldAppURL;

        public HomeRepository(EPortalDBContext empLoginDBContext, ISessionService sessionService, EmployeeLoginRepository empRepo, IConfiguration configuration)
        {
            _empLoginDBContext = empLoginDBContext;
            _sessionService = sessionService;
            _empRepo = empRepo;
            _configuration = configuration;
            moveToOldAppURL = _configuration["Switch_New_Old_New:Call_Old_App_URL"].ToString();
        }


        public List<MenuViewModel> GetMenu(int internet)
        {
            //try
            //{
            //Employee_Details objemp = (Employee_Details)HttpContext.Current.Session["Employee"];
            Employee_Details objemp = _sessionService.Get<Employee_Details>("Employee");


            List<ADMENU_MST> MenuAuth = new List<ADMENU_MST>();
            List<ADMENU_MST> Menutodisplay = new List<ADMENU_MST>();
            List<ADMENU_MST> objList1 = new List<ADMENU_MST>();

            long userid = Convert.ToInt32(objemp.Employee_Code);
            #region "User End"
            objList1 = _empLoginDBContext.ADMENU_MST.Where(m => m.STATUS == 1 && m.MENU_DISPLAY == 1 && m.MENU_TYPE == 1).ToList();

            if (internet == 1)
            {
                objList1 = objList1.Where(m => m.INTERNET == 1).ToList();
            }
            //objList1 = (from data in objList1
            //            select new ADMENU_MST
            //            {
            //                //against MENUID
            //                //var testList = _empLoginDBContext.ADMENUPARAMMAPPING_TRN.Where(d=>d.MENUID==Convert.ToInt32(data.MENU_ID)).ToList()
            //                MENU_ID = Convert.ToInt32(data.MENU_ID),
            //                MENU_PARENT_ID = Convert.ToInt32(data.MENU_PARENT_ID),
            //                MENU_LEVEL = Convert.ToInt32(data.MENU_LEVEL),
            //                MENU_URL = data.MENU_URL,
            //                MENU_TOOLTIP = data.MENU_TOOLTIP,
            //                MENU_TEXT = data.MENU_TEXT,
            //                ADMENUPARAMMAPPING_TRN = data.ADMENUPARAMMAPPING_TRN.ToList(),
            //                
            //                MENU_TARGET = data.MENU_TARGET,
            //                MENUICON = data.MENUICON,
            //                MENUICON_CONTENT_TYPE = data.MENUICON_CONTENT_TYPE,
            //                MENU_DISPLAY_ORDER = data.MENU_DISPLAY_ORDER
            //            }).ToList();

            //objList1 = (from data in objList1
            //            select new ADMENU_MST
            //            {
            //                MENU_ID = data.MENU_ID,
            //                MENU_PARENT_ID = data.MENU_PARENT_ID,
            //                MENU_LEVEL = data.MENU_LEVEL,
            //                MENU_URL = data.MENU_URL,
            //                MENU_TOOLTIP = data.MENU_TOOLTIP,
            //                MENU_TEXT = data.MENU_TEXT,
            //                //ADMENUPARAMMAPPING_TRN = data.ADMENUPARAMMAPPING_TRN.ToList(),
            //                ADMENUPARAMMAPPING_TRN = _empLoginDBContext.ADMENUPARAMMAPPING_TRN.Where(x => x.MENUID == data.MENU_ID).ToList(),
            //                MENU_TARGET = data.MENU_TARGET,
            //                MENUICON = data.MENUICON,
            //                MENUICON_CONTENT_TYPE = data.MENUICON_CONTENT_TYPE,
            //                MENU_DISPLAY_ORDER = data.MENU_DISPLAY_ORDER
            //            }).ToList();

            //var aa = objList1.Where(a => a.MENU_URL == "/BikerCafe/GuestMealBooking").ToList();
            //if (aa != null) { var abc = aa.ISUPGRADED == 1 ? "http://10.117.8.88:8884" + aa.MENU_URL: aa.MENU_URL; }


            objList1 = (from data in objList1
                        select new ADMENU_MST
                        {
                            MENU_ID = data.MENU_ID,
                            MENU_PARENT_ID = data.MENU_PARENT_ID,
                            MENU_LEVEL = data.MENU_LEVEL,
                            //MENU_URL = data.ISUPGRADED==1? moveToOldAppURL + data.NEW_MENU_URL : data.MENU_URL,                             
                            MENU_URL = data.ISUPGRADED == 1 ? data.NEW_MENU_URL : string.IsNullOrEmpty(data.MENU_URL) ? data.MENU_URL : moveToOldAppURL + data.MENU_URL,
                            MENU_TOOLTIP = data.MENU_TOOLTIP,
                            MENU_TEXT = data.MENU_TEXT,
                            //ADMENUPARAMMAPPING_TRN = data.ADMENUPARAMMAPPING_TRN.ToList(),
                            ADMENUPARAMMAPPING_TRN = _empLoginDBContext.ADMENUPARAMMAPPING_TRN.Where(x => x.MENUID == data.MENU_ID).ToList(),
                            MENU_TARGET = data.MENU_TARGET,
                            MENUICON = data.MENUICON,
                            MENUICON_CONTENT_TYPE = data.MENUICON_CONTENT_TYPE,
                            MENU_DISPLAY_ORDER = data.MENU_DISPLAY_ORDER
                        }).ToList();

            //List<ADMENU_MST> objList2 = new List<ADMENU_MST>();
            //objList2=


            //Company Casual
            //String[] parameterValue = GetParameterValue("DESIGNATION_KIOSKLOGIN").Split(',');
            string[] parm_value1 = GetParameterValue("DESG_STAFFHOMEPAGE_VALIDATION").Split(',');
            ArrayList _arrayList1 = new ArrayList(parm_value1);
            //if ((objemp._DesigId == (long)16 || objemp._DesigId == (long)19 || objemp._DesigId == (long)33))
            if (_arrayList1.Contains(objemp._DesigId != null ? objemp._DesigId.ToString() : "0"))
            //Company Casual
            {
                #region Get Staff
                foreach (var oData in objList1)
                {
                    bool IsAuth = false;
                    foreach (var obj in oData.ADMENUPARAMMAPPING_TRN.Where(m => m.STATUS == 1).OrderBy(o => o.MENUPARAMID).ToList())
                    {
                        IsAuth = false;
                        if (obj.MENUPARAMID == 2 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Designation_Id))
                            IsAuth = true;
                        if (obj.MENUPARAMID == 3 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Site_Id))
                            IsAuth = true;
                    }
                    if (IsAuth)
                        MenuAuth.Add(oData);
                }
                #endregion
            }
            else
            {
                #region All Data
                foreach (var oData in objList1)
                {
                    bool IsAuth = true;
                    foreach (var obj in oData.ADMENUPARAMMAPPING_TRN.Where(m => m.STATUS == 1))
                    {
                        IsAuth = false;
                        if (obj.OPERATOR == "1")
                        {
                            if (objemp.Functional_Designation_Id != "" && obj.MENUPARAMID == 1 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Functional_Designation_Id))
                                IsAuth = true;
                            if (objemp.Designation_Id != "" && obj.MENUPARAMID == 2 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Designation_Id))
                                IsAuth = true;
                            if (objemp.Site_Id != "" && obj.MENUPARAMID == 3 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Site_Id))
                                IsAuth = true;
                            if (obj.MENUPARAMID == 4 && obj.PARAM_VALUE.Split(',').ToArray().Contains(Convert.ToString(objemp.ORGLVL)))
                                IsAuth = true;
                            if (IsAuth == false)
                                break;

                        }
                        if (obj.OPERATOR == "2")
                        {
                            if (objemp.Functional_Designation_Id != "" && obj.MENUPARAMID == 1 && !(obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Functional_Designation_Id)))
                                IsAuth = true;
                            if (obj.MENUPARAMID == 2 && !(obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Designation_Id)))
                                IsAuth = true;
                            if (obj.MENUPARAMID == 3 && !(obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Site_Id)))
                                IsAuth = true;
                            if (obj.MENUPARAMID == 4 && !(obj.PARAM_VALUE.Split(',').ToArray().Contains(Convert.ToString(objemp.ORGLVL))))
                                IsAuth = true;
                            if (IsAuth == false)
                                break;
                        }
                    }
                    if (IsAuth)
                        MenuAuth.Add(oData);
                }
                #endregion
            }

            var oMinlvl = MenuAuth.Min(m => m.MENU_LEVEL);

            foreach (var omenu in MenuAuth.Where(m => m.MENU_LEVEL == oMinlvl))
            {
                List<ADMENU_MST> objmenu = prepareUsermenu(MenuAuth, omenu);
                if (objmenu.Count > 0)
                {
                    Menutodisplay.AddRange(objmenu);
                    Menutodisplay.Add(omenu);
                }
                if (!string.IsNullOrEmpty(omenu.MENU_URL))                
                    Menutodisplay.Add(omenu);
                
            }
            #endregion

            #region "Admin Side Menu"
            var oadmenu = (from odt in _empLoginDBContext.ADROLEUSER_MAPPING_TRN.Where(m => m.USER_ID == userid && m.STATUS == 1)
                           join orole in _empLoginDBContext.ADMENUROLE_MAPPING_MST.Where(n => n.STATUS == 1) on odt.ROLE_ID equals orole.ROLE_ID
                           join omenu in _empLoginDBContext.ADMENU_MST on orole.MENU_ID equals omenu.MENU_ID
                           where omenu.STATUS == 1 && omenu.MENU_DISPLAY == 1 && omenu.MENU_TYPE == 2
                           select omenu
                         ).ToList().Distinct();

            if (internet == 1)
            {
                oadmenu = oadmenu.Where(m => m.INTERNET == 1).ToList();
            }
            var oadmenulist = _empLoginDBContext.ADMENU_MST.Where(m => m.STATUS == 1 && m.MENU_TYPE == 2 && m.MENU_URL == null).ToList();
            if (internet == 1)
            {
                oadmenulist = oadmenulist.Where(m => m.INTERNET == 1).ToList();
            }
            oadmenulist.AddRange(oadmenu);
            foreach (var omenu in oadmenu)
            {
                List<ADMENU_MST> objmenu = prepareAdminmenu(oadmenulist, omenu);
                if (objmenu.Count > 0)
                {
                    Menutodisplay.AddRange(objmenu);
                    Menutodisplay.Add(omenu);
                }
                if (!string.IsNullOrEmpty(omenu.MENU_URL))
                {
                    //Added by Dalbir
                    if(omenu.ISUPGRADED==1)
                    {                       
                        omenu.MENU_URL = omenu.NEW_MENU_URL;
                    }
                    else
                    {
                        omenu.MENU_URL = moveToOldAppURL+omenu.MENU_URL;
                    }

                        Menutodisplay.Add(omenu);
                }
            }


            #endregion

            List<MenuViewModel> objList = new List<MenuViewModel>();
            objList = (from data in Menutodisplay.Distinct().ToList()
                       select new MenuViewModel
                       {
                           MenuId = Convert.ToInt32(data.MENU_ID),
                           MenuParentId = Convert.ToInt32(data.MENU_PARENT_ID),
                           MenuLevel = Convert.ToInt32(data.MENU_LEVEL),
                           URL = data.MENU_URL,
                           ToolTip = data.MENU_TOOLTIP,
                           Title = data.MENU_TEXT,
                           MenuTarget = data.MENU_TARGET == 1 ? "_blank" : "",
                           MenuIcon = data.MENUICON,
                           MenuContentType = data.MENUICON_CONTENT_TYPE,
                           MenuOrder = Convert.ToInt32(data.MENU_DISPLAY_ORDER)
                       }).ToList();

            return objList;
            //}
            //catch
            //{

            //    throw;
            //}
        }

        //public List<MenuViewModel> GetMenu(int internet)
        //{
        //    //try
        //    //{
        //        //Employee_Details objemp = (Employee_Details)HttpContext.Current.Session["Employee"];
        //        Employee_Details objemp = _sessionService.Get<Employee_Details>("Employee");


        //        List<ADMENU_MST> MenuAuth = new List<ADMENU_MST>();
        //        List<ADMENU_MST> Menutodisplay = new List<ADMENU_MST>();
        //        List<ADMENU_MST> objList1 = new List<ADMENU_MST>();

        //        long userid = Convert.ToInt32(objemp.Employee_Code);
        //        #region "User End"
        //        objList1 = _empLoginDBContext.ADMENU_MST.Where(m => m.STATUS == 1 && m.MENU_DISPLAY == 1 && m.MENU_TYPE == 1).ToList();
        //        if (internet == 1)
        //        {
        //            objList1 = objList1.Where(m => m.INTERNET == 1).ToList();
        //        }
        //        objList1 = (from data in objList1
        //                    select new ADMENU_MST
        //                    {
        //                        //against MENUID
        //                        //var testList = _empLoginDBContext.ADMENUPARAMMAPPING_TRN.Where(d=>d.MENUID==Convert.ToInt32(data.MENU_ID)).ToList()


        //                        MENU_ID = Convert.ToInt32(data.MENU_ID),
        //                        MENU_PARENT_ID = Convert.ToInt32(data.MENU_PARENT_ID),
        //                        MENU_LEVEL = Convert.ToInt32(data.MENU_LEVEL),
        //                        MENU_URL = data.MENU_URL,
        //                        MENU_TOOLTIP = data.MENU_TOOLTIP,
        //                        MENU_TEXT = data.MENU_TEXT,
        //                        ADMENUPARAMMAPPING_TRN = data.ADMENUPARAMMAPPING_TRN.ToList(),
        //                        MENU_TARGET = data.MENU_TARGET,
        //                        MENUICON = data.MENUICON,
        //                        MENUICON_CONTENT_TYPE = data.MENUICON_CONTENT_TYPE,
        //                        MENU_DISPLAY_ORDER = data.MENU_DISPLAY_ORDER
        //                    }).ToList();

        //        //List<ADMENU_MST> objList2 = new List<ADMENU_MST>();
        //        //objList2=


        //        //Company Casual
        //        //String[] parameterValue = GetParameterValue("DESIGNATION_KIOSKLOGIN").Split(',');
        //        String[] parm_value1 = GetParameterValue("DESG_STAFFHOMEPAGE_VALIDATION").Split(',');
        //        ArrayList _arrayList1 = new ArrayList(parm_value1);
        //        //if ((objemp._DesigId == (long)16 || objemp._DesigId == (long)19 || objemp._DesigId == (long)33))
        //        if (_arrayList1.Contains(objemp._DesigId != null ? objemp._DesigId.ToString() : "0"))
        //        //Company Casual
        //        {
        //            #region Get Staff
        //            foreach (var oData in objList1)
        //            {
        //                bool IsAuth = false;
        //                foreach (var obj in oData.ADMENUPARAMMAPPING_TRN.Where(m => m.STATUS == 1).OrderBy(o => o.MENUPARAMID).ToList())
        //                {
        //                    IsAuth = false;
        //                    if (obj.ADMENUPARAM_MST.MENUPARAMID == 2 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Designation_Id))
        //                        IsAuth = true;
        //                    if (obj.ADMENUPARAM_MST.MENUPARAMID == 3 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Site_Id))
        //                        IsAuth = true;
        //                }
        //                if (IsAuth)
        //                    MenuAuth.Add(oData);
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            #region All Data
        //            foreach (var oData in objList1)
        //            {
        //                bool IsAuth = true;
        //                foreach (var obj in oData.ADMENUPARAMMAPPING_TRN.Where(m => m.STATUS == 1))
        //                {
        //                    IsAuth = false;
        //                    if (obj.OPERATOR == "1")
        //                    {
        //                        if (obj.ADMENUPARAM_MST.MENUPARAMID == 1 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Functional_Designation_Id))
        //                            IsAuth = true;
        //                        if (obj.ADMENUPARAM_MST.MENUPARAMID == 2 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Designation_Id))
        //                            IsAuth = true;
        //                        if (obj.ADMENUPARAM_MST.MENUPARAMID == 3 && obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Site_Id))
        //                            IsAuth = true;
        //                        if (obj.ADMENUPARAM_MST.MENUPARAMID == 4 && obj.PARAM_VALUE.Split(',').ToArray().Contains(Convert.ToString(objemp.ORGLVL)))
        //                            IsAuth = true;
        //                        if (IsAuth == false)
        //                            break;
        //                    }
        //                    if (obj.OPERATOR == "2")
        //                    {
        //                        if (obj.ADMENUPARAM_MST.MENUPARAMID == 1 && !(obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Functional_Designation_Id)))
        //                            IsAuth = true;
        //                        if (obj.ADMENUPARAM_MST.MENUPARAMID == 2 && !(obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Designation_Id)))
        //                            IsAuth = true;
        //                        if (obj.ADMENUPARAM_MST.MENUPARAMID == 3 && !(obj.PARAM_VALUE.Split(',').ToArray().Contains(objemp.Site_Id)))
        //                            IsAuth = true;
        //                        if (obj.ADMENUPARAM_MST.MENUPARAMID == 4 && !(obj.PARAM_VALUE.Split(',').ToArray().Contains(Convert.ToString(objemp.ORGLVL))))
        //                            IsAuth = true;
        //                        if (IsAuth == false)
        //                            break;
        //                    }
        //                }
        //                if (IsAuth)
        //                    MenuAuth.Add(oData);
        //            }
        //            #endregion
        //        }

        //        var oMinlvl = MenuAuth.Min(m => m.MENU_LEVEL);

        //        foreach (var omenu in MenuAuth.Where(m => m.MENU_LEVEL == oMinlvl))
        //        {
        //            List<ADMENU_MST> objmenu = prepareUsermenu(MenuAuth, omenu);
        //            if (objmenu.Count > 0)
        //            {
        //                Menutodisplay.AddRange(objmenu);
        //                Menutodisplay.Add(omenu);
        //            }
        //            if (!string.IsNullOrEmpty(omenu.MENU_URL))
        //                Menutodisplay.Add(omenu);
        //        }
        //        #endregion

        //        #region "Admin Side Menu"
        //        var oadmenu = (from odt in _empLoginDBContext.ADROLEUSER_MAPPING_TRN.Where(m => m.USER_ID == userid && m.STATUS == 1)
        //                       join orole in _empLoginDBContext.ADMENUROLE_MAPPING_MST.Where(n => n.STATUS == 1) on odt.ROLE_ID equals orole.ROLE_ID
        //                       join omenu in _empLoginDBContext.ADMENU_MST on orole.MENU_ID equals omenu.MENU_ID
        //                       where omenu.STATUS == 1 && omenu.MENU_DISPLAY == 1 && omenu.MENU_TYPE == 2
        //                       select omenu
        //                     ).ToList();

        //        if (internet == 1)
        //        {
        //            oadmenu = oadmenu.Where(m => m.INTERNET == 1).ToList();
        //        }
        //        var oadmenulist = _empLoginDBContext.ADMENU_MST.Where(m => m.STATUS == 1 && m.MENU_TYPE == 2 && m.MENU_URL == null).ToList();
        //        if (internet == 1)
        //        {
        //            oadmenulist = oadmenulist.Where(m => m.INTERNET == 1).ToList();
        //        }
        //        oadmenulist.AddRange(oadmenu);
        //        foreach (var omenu in oadmenu)
        //        {
        //            List<ADMENU_MST> objmenu = prepareAdminmenu(oadmenulist, omenu);
        //            if (objmenu.Count > 0)
        //            {
        //                Menutodisplay.AddRange(objmenu);
        //                Menutodisplay.Add(omenu);
        //            }
        //            if (!string.IsNullOrEmpty(omenu.MENU_URL))
        //                Menutodisplay.Add(omenu);
        //        }


        //        #endregion

        //        List<MenuViewModel> objList = new List<MenuViewModel>();
        //        objList = (from data in Menutodisplay.Distinct().ToList()
        //                   select new MenuViewModel
        //                   {
        //                       MenuId = Convert.ToInt32(data.MENU_ID),
        //                       MenuParentId = Convert.ToInt32(data.MENU_PARENT_ID),
        //                       MenuLevel = Convert.ToInt32(data.MENU_LEVEL),
        //                       URL = data.MENU_URL,
        //                       ToolTip = data.MENU_TOOLTIP,
        //                       Title = data.MENU_TEXT,
        //                       MenuTarget = data.MENU_TARGET == 1 ? "_blank" : "",
        //                       MenuIcon = data.MENUICON,
        //                       MenuContentType = data.MENUICON_CONTENT_TYPE,
        //                       MenuOrder = Convert.ToInt32(data.MENU_DISPLAY_ORDER)
        //                   }).ToList();

        //        return objList;
        //    //}
        //    //catch
        //    //{

        //    //    throw;
        //    //}

        //}

        public List<ADMENU_MST> prepareUsermenu(List<ADMENU_MST> MenuAuth, ADMENU_MST mnu)
        {
            List<ADMENU_MST> objmenutodisplay = new List<ADMENU_MST>();

            foreach (var omenu in MenuAuth.Where(m => m.MENU_PARENT_ID == mnu.MENU_ID))
            {
                List<ADMENU_MST> objcnt = new List<ADMENU_MST>();
                if (MenuAuth.Where(m => m.MENU_PARENT_ID == omenu.MENU_ID).Count() > 0)
                    objcnt = prepareUsermenu(MenuAuth, omenu);
                if (objcnt.Count() > 0)
                {
                    objmenutodisplay.AddRange(objcnt);
                    objmenutodisplay.Add(omenu);
                }
                else if (!string.IsNullOrEmpty(omenu.MENU_URL))
                {
                    objmenutodisplay.Add(omenu);
                }

            }
            return objmenutodisplay;
        }
        public List<ADMENU_MST> prepareAdminmenu(List<ADMENU_MST> MenuAuth, ADMENU_MST mnu)
        {
            List<ADMENU_MST> objmenutodisplay = new List<ADMENU_MST>();

            foreach (var omenu in MenuAuth.Where(m => m.MENU_ID == mnu.MENU_PARENT_ID))
            {
                List<ADMENU_MST> objcnt = new List<ADMENU_MST>();
                if (MenuAuth.Where(m => m.MENU_ID == omenu.MENU_PARENT_ID).Count() > 0)
                    objcnt = prepareAdminmenu(MenuAuth, omenu);
                if (objcnt.Count() > 0)
                {
                    objmenutodisplay.AddRange(objcnt);
                    objmenutodisplay.Add(omenu);
                }
                else
                {
                    objmenutodisplay.Add(omenu);
                }

            }

            return objmenutodisplay;
        }

        //                            from lu in _empLoginDBContext.ADLOGINUSER
        //                                        .Where(d => d.ADEMPCODE == userid)
        //                            join emp in _empLoginDBContext.ADEMPLOYEE on lu.ADEMPCODE equals emp.ADEMPCODE into tempemp
        //                            from emp in tempemp.DefaultIfEmpty()

        //                            join dds in _empLoginDBContext.VW_ASSOCIATELVLDETAILS
        //                                        .Where(g => g.SYKI == lngsykiid) on lu.ADEMPCODE equals dds.ADEMPCODE into tempdds
        //                            from dds in tempdds.DefaultIfEmpty()

        //                            join tdds in _empLoginDBContext.ADEMPDIVDEPTSECT
        //                                        .Where(g => g.SYKI == lngsykiid) on lu.ADEMPCODE equals tdds.ADEMPCODE into edds
        //from tdds in edds.DefaultIfEmpty()
        //join dsg in _empLoginDBContext.ADDESIGNATION on dds.ADDESIGNATIONID equals dsg.ADDESIGNATIONID into tempdsg
        //                            from dsg in tempdsg.DefaultIfEmpty()

        public PresidentDeskViewModel GetPresidentDetail(PresidentDeskViewModel objsearch)
        {
            var objdata = (from data in _empLoginDBContext.CM_PRESIDENT_MST
                           where data.STATUS == objsearch.STATUS
                           orderby data.PRESIDENT_TITLE
                           //&& (objsearch.PRESIDENTMSG_ID==null) ? true : objsearch.PRESIDENTMSG_ID.Contains(data.PRESIDENT_ID)
                           select new PresidentDesk_MSTViewModel
                           {
                               PHOTO_CONTENTTYPE = data.PHOTO_CONTENTTYPE,
                               PHOTO_NAME = data.PHOTO_NAME,
                               PRESIDENT_ID = data.PRESIDENT_ID,
                               PRESIDENT_NAME = data.PRESIDENT_NAME,
                               PRESIDENT_PHOTO = data.PRESIDENT_PHOTO,
                               PRESIDENT_TITLE = data.PRESIDENT_TITLE,
                               REMARKS = data.REMARKS,
                               STATUS = data.STATUS,
                           }).ToList();

            //var objTRN = (from data in _empLoginDBContext.CM_PRESIDENTMSG_TRN
            //              where data.STATUS == objsearch.STATUS
            //              //&& (objsearch.PRESIDENTMSG_ID == null) ? true : objsearch.PRESIDENTMSG_ID.Contains(data.PRESIDENTMSG_ID)
            //              select new PresidentDesk_TRNViewModel
            //              {
            //                  BRIEF = data.BRIEF,
            //                  MESSAGE = data.MESSAGE,
            //                  PRESIDENTMSG_ID = data.PRESIDENTMSG_ID,
            //                  ATTACHMENT_NAME = data.ATTACHMENT_NAME,
            //                  ATTACHMENT_CONTENTTYPE = data.ATTACHMENT_CONTENTTYPE,
            //                  PRESIDENT_ID = data.CM_PRESIDENT_MST.PRESIDENT_ID,
            //              }).ToList();


            var objTRN = (from data in _empLoginDBContext.CM_PRESIDENTMSG_TRN.Where(d => d.STATUS == objsearch.STATUS)
                          join data1 in _empLoginDBContext.CM_PRESIDENT_MST on data.PRESIDENT_ID equals data1.PRESIDENT_ID into data1TempData
                          from data1 in data1TempData.DefaultIfEmpty()
                          select new PresidentDesk_TRNViewModel
                          {
                              BRIEF = data.BRIEF,
                              MESSAGE = data.MESSAGE,
                              PRESIDENTMSG_ID = data.PRESIDENTMSG_ID,
                              ATTACHMENT_NAME = data.ATTACHMENT_NAME,
                              ATTACHMENT_CONTENTTYPE = data.ATTACHMENT_CONTENTTYPE,
                              PRESIDENT_ID = data1.PRESIDENT_ID,
                          }).ToList();

            objsearch.PresidentDetail = (List<PresidentDesk_MSTViewModel>)objdata;
            objsearch.President_TRNDTL = (List<PresidentDesk_TRNViewModel>)objTRN;

            return objsearch;
        }

        public List<PresidentDesk_TRNViewModel> GetPresidentContent(List<PresidentDesk_TRNViewModel> objsearch)
        {
            PresidentDeskViewModel PdVM = new PresidentDeskViewModel();
            short[] st = objsearch.Select(m => m.STATUS).Distinct().ToArray();
            long[] msgid = objsearch.Select(m => m.PRESIDENTMSG_ID).Distinct().ToArray();
            var objdata = (from data in _empLoginDBContext.CM_PRESIDENTMSG_TRN
                           where st.Contains(data.STATUS)
                           && msgid.Contains(data.PRESIDENTMSG_ID)
                           select new PresidentDesk_TRNViewModel
                           {
                               BRIEF = data.BRIEF,
                               MESSAGE = data.MESSAGE,
                               PRESIDENTMSG_ID = data.PRESIDENTMSG_ID,
                               ATTACHMENT_NAME = data.ATTACHMENT_NAME,
                               ATTACHMENT_CONTENTTYPE = data.ATTACHMENT_CONTENTTYPE
                           }).ToList();
            if (objdata.Count > 0)
            {
                foreach (var data in objdata)
                {
                    PdVM.President_TRNDTL.Add(new PresidentDesk_TRNViewModel
                    {
                        BRIEF = data.BRIEF,
                        MESSAGE = CommonRepository.HtmlToText(data.MESSAGE),
                        PRESIDENTMSG_ID = data.PRESIDENTMSG_ID,
                        ATTACHMENT_NAME = data.ATTACHMENT_NAME,
                        ATTACHMENT_CONTENTTYPE = data.ATTACHMENT_CONTENTTYPE,
                        PRESIDENT_ID = data.PRESIDENT_ID
                        //ATTACHMENT = data.ATTACHMENT,
                    });
                }
            }

            return PdVM.President_TRNDTL;
        }
        public List<PresidentDesk_TRNViewModel> GetPresidentContentDwn(List<PresidentDesk_TRNViewModel> objsearch)
        {
            PresidentDeskViewModel PdVM = new PresidentDeskViewModel();
            short[] st = objsearch.Select(m => m.STATUS).Distinct().ToArray();
            long[] msgid = objsearch.Select(m => m.PRESIDENTMSG_ID).Distinct().ToArray();
            var objdata = (from data in _empLoginDBContext.CM_PRESIDENTMSG_TRN
                           where st.Contains(data.STATUS)
                           && msgid.Contains(data.PRESIDENTMSG_ID)
                           select new PresidentDesk_TRNViewModel
                           {
                               ATTACHMENT_NAME = data.ATTACHMENT_NAME,
                               ATTACHMENT_CONTENTTYPE = data.ATTACHMENT_CONTENTTYPE,
                               ATTACHMENT = data.ATTACHMENT
                           }).ToList();
            return objdata;
        }

        public List<ContentViewModel> GetContent(ContentViewModel objsearch)
        {
            //EmployeeLoginRepository objloginRep = new EmployeeLoginRepository();
            return _empRepo.GetContent(objsearch);
        }

        public List<ContentViewModel> GetContent()
        {
            //EmployeeLoginRepository objloginRep = new EmployeeLoginRepository();
            return _empRepo.GetContent();
        }

        public List<EmergencyContViewModel> GetContactList(EmergencyContViewModel objsearch)
        {
            var objdata = (from data in _empLoginDBContext.AD_EMERGENCYNO_TRN.Where(d => d.STATUS == objsearch.STATUS)
                           join data1 in _empLoginDBContext.SYSITE on data.SITE equals data1.SYSITEID into datatemp1
                           from data1 in datatemp1.DefaultIfEmpty()
                           select new EmergencyContViewModel
                           {
                               EMERGENCY_ID = data.EMERGENCY_ID,
                               STATUS = data.STATUS,
                               MOBILE_NO = data.MOBILE_NO,
                               OPERATION = data.OPERATION,
                               STD_CODE = data.STD_CODE,
                               TELEPHONE_NO = data.TELEPHONE_NO,
                               Site = data1.DESCRIP,
                               SiteID = data1.SYSITEID
                           }).ToList();

            return (List<EmergencyContViewModel>)objdata;
        }



        public PersonalityQuotesViewModel GetFamousQuotes(PersonalityQuotesViewModel objsearch)
        {
            //var objdata = (from data in _empLoginDBContext.AD_PERSONALITYQUOTES_TRN
            //               where data.STATUS == objsearch.STATUS
            //               select new PersonalityQuotesViewModel
            //               {
            //                   QUOTES = data.QUOTES,
            //                   QUOTES_ID = data.QUOTES_ID,
            //                   BACKGROUND_IMAGE = data.AD_FAMOUSPERSONALITY_MST.BACKGROUND_IMAGE,
            //                   IMAGE_CONTENTTYPE = data.AD_FAMOUSPERSONALITY_MST.IMAGE_CONTENTTYPE,
            //                   IMAGE_NAME = data.AD_FAMOUSPERSONALITY_MST.IMAGE_NAME,
            //                   PERSONALITYNAME = data.AD_FAMOUSPERSONALITY_MST.NAME,
            //                   PERSONALITY_ID = data.AD_FAMOUSPERSONALITY_MST.PERSONALITY_ID
            //               }).ToList();


            var objdata = (from data in _empLoginDBContext.AD_PERSONALITYQUOTES_TRN.Where(d => d.STATUS == objsearch.STATUS)
                           join data1 in _empLoginDBContext.AD_FAMOUSPERSONALITY_MST on data.PERSONALITY_ID equals data1.PERSONALITY_ID into tempdata1
                           from data1 in tempdata1.DefaultIfEmpty()
                           select new PersonalityQuotesViewModel
                           {
                               QUOTES = data.QUOTES,
                               QUOTES_ID = data.QUOTES_ID,
                               BACKGROUND_IMAGE = data1.BACKGROUND_IMAGE,
                               IMAGE_CONTENTTYPE = data1.IMAGE_CONTENTTYPE,
                               IMAGE_NAME = data1.IMAGE_NAME,
                               PERSONALITYNAME = data1.NAME,
                               PERSONALITY_ID = data1.PERSONALITY_ID
                           }).ToList();


            return (PersonalityQuotesViewModel)objdata.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }


        public long GetApprovalCount(long userid)
        {
            //Int64 resultout = 0;
            //ObjectParameter objparameter = new ObjectParameter("NOOFCOUNT", typeof(Int64));
            //_empLoginDBContext.SPROC_PENDINGCOUNT(userid, objparameter);
            //resultout = Convert.ToInt16(objparameter.Value);
            //return resultout;

            long resultout = 0;

            var sql = "BEGIN SPROC_PENDINGCOUNT(:EMPCODE_IN,:NOOFCOUNT); END;";
            OracleParameter ERRMSG_OUT_OUT = new OracleParameter("NOOFCOUNT", OracleDbType.Int64) { Direction = ParameterDirection.Output };
            var parameters = new OracleParameter[]
            {

                        new OracleParameter("EMPCODE_IN", OracleDbType.Decimal) { Value = userid },
                        ERRMSG_OUT_OUT
            };
            //_empLoginDBContext.Database.ExecuteSqlRawAsync(sql, parameters);
            _empLoginDBContext.Database.ExecuteSqlRaw(sql, parameters);
            var outputValue = parameters[1].Value;
            if (outputValue != DBNull.Value && outputValue != null)
            {
                resultout = Convert.ToInt64(outputValue.ToString());
            }

            //resultout = Convert.ToInt16(ERRMSG_OUT_OUT?.Value.ToString());            

            return resultout;
        }

        public long GetRequestCount(long userid)
        {
            //Int64 resultout = 0;
            //ObjectParameter objparameter = new ObjectParameter("NOOFCOUNT", typeof(Int64));
            //_empLoginDBContext.SPROC_REQPENDINGCOUNT(userid, objparameter);
            //resultout = Convert.ToInt16(objparameter.Value);
            //return resultout;

            Int64 resultout = 0;

            var sql = "BEGIN SPROC_REQPENDINGCOUNT(:EMPCODE_IN,:NOOFCOUNT); END;";
            OracleParameter ERRMSG_OUT_OUT = new OracleParameter("NOOFCOUNT", OracleDbType.Int64) { Direction = ParameterDirection.Output };
            var parameters = new OracleParameter[]
            {

                        new OracleParameter("EMPCODE_IN", OracleDbType.Decimal) { Value = userid },
                        ERRMSG_OUT_OUT
            };
            //_empLoginDBContext.Database.ExecuteSqlRawAsync(sql, parameters);
            _empLoginDBContext.Database.ExecuteSqlRaw(sql, parameters);
            //resultout = Convert.ToInt16(ERRMSG_OUT_OUT.Value);
            var outputValue = parameters[1].Value;
            if (outputValue != DBNull.Value && outputValue != null)
            {
                resultout = Convert.ToInt64(outputValue.ToString());
            }
            return resultout;
        }
        public List<PeopleSerchViewModel> GetAssociateDetails(string KI, string OperationID, string DivisionID, string DepartmentID, string SectionID,
                                       string EmpCode, string FirstName, string LastName, string Designation, string EMailID,
                                       string BloodGroup)
        {

            string strAssociateLowestLevel = string.Empty;

            if (OperationID == "" && DivisionID == "" && DepartmentID == "" && SectionID == "")
            {
                //Get All Data irrespective of any any levels
                strAssociateLowestLevel = "";
            }
            else if (SectionID != "")
            {
                strAssociateLowestLevel = SectionID;
            }
            else if (DepartmentID != "")
            {
                strAssociateLowestLevel = DepartmentID;
            }
            else if (DivisionID != "")
            {
                strAssociateLowestLevel = DivisionID;
            }
            else if (OperationID != "")
            {
                strAssociateLowestLevel = OperationID;
            }

            string[] parameterValue = GetParameterValue("DESIGNATION_KIOSKLOGIN").Split(',');
            List<long?> desg = new List<long?>();
            foreach (var val in parameterValue)
            {
                desg.Add(Convert.ToInt64(val));
            }

            List<PeopleSerchViewModel> objlist = (from odata in _empLoginDBContext.ADEMPLOYEE.Where(e => e.ACTIVE == 1)
                                                  join aslvl in _empLoginDBContext.VW_ASSOCIATELVLDETAILS.Where(m => !desg.Contains(m.ADDESIGNATIONID) && m.ACTIVE == 1 && m.SYKI == _empLoginDBContext.SYKI.Where(n => n.ACTIVE == 1).FirstOrDefault().SYKIID) on odata.ADEMPCODE equals aslvl.ADEMPCODE
                                                  select new PeopleSerchViewModel
                                                  {
                                                      _ECode = odata.ADEMPCODE,
                                                      _EName = odata.FIRSTNAME + " " + odata.LASTNAME
                                                  }).ToList();
            return objlist;
        }

        public async Task CreateUserMenuLog(string Ecode, string URL_IN)
        {
            //_empLoginDBContext.SPROC_CREATEUSERMENULOG(Ecode, URL_IN);


            string sql = "BEGIN SPROC_CREATEUSERMENULOG(:ADEMPCODE_IN,:URL_IN); END;";
            var parameters = new OracleParameter[]
            {
                        new OracleParameter("ADEMPCODE_IN", OracleDbType.Varchar2) { Value = Ecode },
                        new OracleParameter("URL_IN", OracleDbType.Varchar2) { Value = URL_IN }
            };
            await _empLoginDBContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public List<MenuViewModel> GetQuickLinkMenu(int internet)
        {
            DateTime _date = DateTime.Now.AddMonths(-6);
            //Employee_Details objemp = (Employee_Details)HttpContext.Current.Session["Employee"];
            Employee_Details objemp = _sessionService.Get<Employee_Details>("Employee");

            var iMenu = (from oData in _empLoginDBContext.AD_USERMENU_LOG.Where(m => m.ECODE == objemp._ECode && m.CREATEDDATE > _date)
                         join omnu in _empLoginDBContext.ADMENU_MST on oData.MENUID equals omnu.MENU_ID
                         where internet == 1 ? (omnu.INTERNET == 1) : true

                         group oData by oData.MENUID into g
                         select new
                         {
                             menuid = g.Key,
                             cnt = g.Count(),
                             priority = 2
                         }
                       ).ToList().Distinct().OrderByDescending(m => m.cnt).Take(6);

            var defaultmenu = (from odata in _empLoginDBContext.ADMENU_MST.Where(m => m.ISDEFAULTMENU != 0 && m.ISDEFAULTMENU != 3).OrderByDescending(m => m.ISDEFAULTMENU)
                               select new
                               {
                                   menuid = odata.MENU_ID,
                                   cnt = 0,
                                   priority = odata.ISDEFAULTMENU == 2 ? 1 : 3
                               }
                               ).Distinct().Take(6);

            var defaultmenu2 = (from odata in _empLoginDBContext.ADMENU_MST.Where(m => m.ISDEFAULTMENU == 3).OrderByDescending(m => m.ISDEFAULTMENU)
                                select new
                                {
                                    menuid = odata.MENU_ID,
                                    cnt = 0,
                                    priority = 1
                                }
                            ).Distinct().Take(6);

            List<MenuViewModel> objfinalizing = new List<MenuViewModel>();
            foreach (var obj in defaultmenu2)
            {
                MenuViewModel omenu = new MenuViewModel();
                omenu.MenuId = Convert.ToInt64(obj.menuid);
                omenu.MenuLevel = obj.priority;
                objfinalizing.Add(omenu);
            }
            foreach (var obj in iMenu)
            {
                MenuViewModel omenu = new MenuViewModel();
                omenu.MenuId = Convert.ToInt64(obj.menuid);
                omenu.MenuLevel = obj.priority;//to store priority use menulevel field which is not used further
                objfinalizing.Add(omenu);
            }
            foreach (var obj in defaultmenu)
            {
                MenuViewModel omenu = new MenuViewModel();
                omenu.MenuId = Convert.ToInt64(obj.menuid);
                omenu.MenuLevel = obj.priority;//to store priority use menulevel field which is not used further
                objfinalizing.Add(omenu);
            }
            Int64[] doubleArraypriority1 = objfinalizing.Where(m => m.MenuLevel == 1).Take(6).Select(m => m.MenuId).ToArray();
            Int64[] doubleArraypriority2 = objfinalizing.Where(m => m.MenuLevel == 2 && !doubleArraypriority1.Contains(m.MenuId)).Take(6 - doubleArraypriority1.Length).Select(m => m.MenuId).ToArray();
            Int64[] newArray = new Int64[6];
            Array.Copy(doubleArraypriority1, newArray, doubleArraypriority1.Length);
            Array.Copy(doubleArraypriority2, 0, newArray, doubleArraypriority1.Length, doubleArraypriority2.Length);
            Int64[] doubleArraypriority3 = objfinalizing.Where(m => m.MenuLevel == 3 && !newArray.Contains(m.MenuId)).Take(6 - (doubleArraypriority1.Length + doubleArraypriority2.Length)).Select(m => m.MenuId).ToArray();
            Array.Copy(doubleArraypriority3, 0, newArray, (doubleArraypriority1.Length + doubleArraypriority2.Length), doubleArraypriority3.Length);
            decimal[] finalizedmenu = Array.ConvertAll(newArray, x => (decimal)x);

            List<MenuViewModel> objList = new List<MenuViewModel>();
            //var finalmenu = (from data in _empLoginDBContext.ADMENU_MST
            //                 where finalizedmenu.Contains(data.MENU_ID)
            //                 select new
            //                 {
            //                     MenuId = data.MENU_ID,
            //                     URL = data.MENU_URL,
            //                     ToolTip = data.MENU_TOOLTIP,
            //                     Title = data.MENU_TEXT,
            //                     MenuTarget = data.MENU_TARGET == 1 ? "_blank" : "",
            //                     MENUICON = data.MENUICON,
            //                     MENUICON_CONTENT_TYPE = data.MENUICON_CONTENT_TYPE,
            //                     ISDEFAULTMENU = data.ISDEFAULTMENU,
            //                 }).OrderByDescending(o => o.ISDEFAULTMENU).ToList();

            //foreach (var obj in finalmenu)
            //{
            //    MenuViewModel objmenu = new MenuViewModel();
            //    objmenu.MenuId = Convert.ToInt64(obj.MenuId);
            //    objmenu.URL = obj.URL;
            //    objmenu.MenuIcon = obj.MENUICON;
            //    objmenu.MenuContentType = obj.MENUICON_CONTENT_TYPE;
            //    objmenu.Title = obj.Title;
            //    objmenu.ToolTip = obj.ToolTip;
            //    objmenu.MenuTarget = obj.MenuTarget;
            //    objList.Add(objmenu);
            //}

            var finalmenu = (from data in _empLoginDBContext.ADMENU_MST
                             where finalizedmenu.Contains(data.MENU_ID)
                             select new
                             {
                                 MenuId = data.MENU_ID,
                                 URL = data.MENU_URL,
                                 NEW_URL = data.NEW_MENU_URL,
                                 ToolTip = data.MENU_TOOLTIP,
                                 Title = data.MENU_TEXT,
                                 MenuTarget = data.MENU_TARGET == 1 ? "_blank" : "",
                                 MENUICON = data.MENUICON,
                                 MENUICON_CONTENT_TYPE = data.MENUICON_CONTENT_TYPE,
                                 ISDEFAULTMENU = data.ISDEFAULTMENU,
                                 IsUpgraded = data.ISUPGRADED
                             }).OrderByDescending(o => o.ISDEFAULTMENU).ToList();


            foreach (var obj in finalmenu)
            {
                MenuViewModel objmenu = new MenuViewModel();
                objmenu.MenuId = Convert.ToInt64(obj.MenuId);
                //objmenu.URL = obj.IsUpgraded==1? moveToOldAppURL + obj.NEW_URL : obj.URL;               
                objmenu.URL = obj.IsUpgraded == 1 ? obj.NEW_URL : string.IsNullOrEmpty(obj.URL) ? obj.URL : moveToOldAppURL + obj.URL;
                objmenu.MenuIcon = obj.MENUICON;
                objmenu.MenuContentType = obj.MENUICON_CONTENT_TYPE;
                objmenu.Title = obj.Title;
                objmenu.ToolTip = obj.ToolTip;
                objmenu.MenuTarget = obj.MenuTarget;
                objList.Add(objmenu);
            }

            return objList;
        }

        public string GetParameterValue(string strParmaName)
        {
            var SelectedParameter = (from v in _empLoginDBContext.SYPARAMETERS
                                     where v.PARAMNAME == strParmaName
                                     select v
                                  ).ToList();
            return (SelectedParameter.FirstOrDefault().PARAMVALUE);
        }
        public List<ExtPortalViewModel> GetExtPortalList()
        {
            var iList = (from data in _empLoginDBContext.ADEXT_PORTAL
                         where data.STATUS == 1
                         select new ExtPortalViewModel
                         {
                             EXTPORTALID = data.EXTPORTALID,
                             PORTALNAME = data.PORTALNAME,
                             PORTAL_DESC = data.PORTAL_DESC,
                             PORTAL_URL = data.PORTAL_URL,
                             PORTAL_ICON = data.PORTAL_ICON,
                             PORTALICON_CONTENT_TYPE = data.PORTALICON_CONTENT_TYPE,
                             PORTALICON_FILENAME = data.PORTALICON_FILENAME,
                             DISPLAY_ORDER = data.DISPLAY_ORDER,
                             CREATED_BY = data.CREATED_BY,
                             CREATED_DATE = data.CREATED_DATE,
                             STATUS = data.STATUS
                         }).OrderBy(o => o.DISPLAY_ORDER).ToList();
            return iList;
        }

        public List<ContentViewModel> GetCommunicationContent()
        {
            //EmployeeLoginRepository objloginRep = new EmployeeLoginRepository();
            return _empRepo.GetCommunicationContent();
        }
    }
}
