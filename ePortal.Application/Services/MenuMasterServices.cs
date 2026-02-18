using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Services
{
    public class MenuMasterServices : IMenuMasterServices
    {
        private MenuMasterRepository _MenuMasterRepo;
        public MenuMasterServices(MenuMasterRepository objMenuMasterRepo)
        {
            _MenuMasterRepo = objMenuMasterRepo;
        }


        #region  MENU MASTER

        public List<MenuViewModel> GetMenuList(string flag)
        {
            return _MenuMasterRepo.GetMenuList(flag);
        }


        public List<MenuViewModel> GetParentName(decimal id)
        {
            return _MenuMasterRepo.GetParentName(id);
        }

        public List<MenuViewModel> GetMenuData()
        {
            return _MenuMasterRepo.GetMenuData();
        }

        public short AddMenuData(MenuViewModel savedata, string userId)
        {
            return _MenuMasterRepo.AddMenuData(savedata, userId);
        }

        public short EditMenuData(string MenuId, MenuViewModel savedata, string userId)
        {
            return _MenuMasterRepo.EditMenuData(MenuId, savedata, userId);
        }

        public int ParentMenu(decimal MenuType, decimal PerentMenu, decimal MenuLevel, int LoginCode)
        {
            return _MenuMasterRepo.ParentMenu(MenuType, PerentMenu, MenuLevel, LoginCode);
        }

        #endregion

    }
}
