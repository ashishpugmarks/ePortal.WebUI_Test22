using ePortal.DomainClasses;

using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IMenuMasterServices
    {

        #region  MENU MASTER

        List<MenuViewModel> GetMenuList(string flag);

        List<MenuViewModel> GetParentName(decimal id);

        List<MenuViewModel> GetMenuData();

        short AddMenuData(MenuViewModel savedata, string userId);

        short EditMenuData(string MenuId, MenuViewModel savedata, string userId);

        int ParentMenu(decimal MenuType, decimal PerentMenu, decimal MenuLevel, int LoginCode);

        #endregion

    }
}
