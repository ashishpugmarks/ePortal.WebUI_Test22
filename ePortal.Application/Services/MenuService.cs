using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using ePortal.DomainClasses;



namespace ePortal.Application.Services
{
    public class MenuService
    {
        //Repository object
       // public IGenericRepository<Menu> repo;

        //Source objects -- These objects need to be filled up with database configurations for building dynamic menu
        public List<MenuMaster> menus;
        public List<MenuParamsGeneral> menuParamsGeneral;
        public List<MenuParamsFnDesignations> menuParamsFnDesignations;
        public List<MenuParamLocations> menuParamLocations;
        public UserViewModel user;

        //target Menu Object -- These objects form the final Menu Output which needs to be sent to UI in json format
        public ParentMenuObject parentMenu;
        public MenuViewModel menuViewModel;
        public List<MenuViewModel> allMenus;




        public ParentMenuObject BuildMenu()
        {
            //1. GetUser information loaded in UserViewModel
            //2. Load all source objects - menus, menuParamsGeneral, menuParamsFnDesignations, menuParamLocations
            //3. Loop thru all menu items and populate MenuViewModel for the given user by applying rules set in configurations in source objects and determine accessibility flag


            MenuViewModel menuItem;
            foreach(MenuMaster menuMasterRow in menus)
            {
               menuItem = PopulateMenuViewModel(menuMasterRow.MenuId);
               if (menuItem != null)
                   allMenus.Add(menuItem);
            }

            //4. Populate final menu object for UI
            ParentMenuObject parentMenu = new ParentMenuObject();
            parentMenu.menuObject = allMenus;
            //before returnning parentmenu object, run an alogorithm on parentMenu object to ensure proper linkage 
            //and wrapping up the menu in proper heirarchy using SubMenuItems property of MenuViewModel 
            return parentMenu;
            //return JavaScriptSerializer().Serialize(parentMenu);
        }

        public MenuViewModel PopulateMenuViewModel(int menuId)
        {
            //Logic to compute if menu is accessible to the current user
            MenuViewModel menu = new MenuViewModel();
            menu.MenuId = menuId;
            //populate all other properties
            menu.isMenuAccessibleToUser = isMenuAccessible(menuId);
            if (menu.isMenuAccessibleToUser)
                return menu;
            else
                return null;
        }

        public bool isMenuAccessible(int menuId)
        {
            //Logic to compute if menu is accessible to the current user
            //This logic will also compute the rules based on operators on each parameter 
            return true; //or false
        }
    }
}
