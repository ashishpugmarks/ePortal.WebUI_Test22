using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IMenuMappingMasterService
    {
        List<MenuMappingViewModel> GetMappingList();

        //List<MenuMappingViewModel> GetMappingDataforGrid(long id);
        List<MenuParamdata> GetMappingDataforGrid(long id);
        List<string> GetMenuList();

        dynamic GetADMENUPARAM_MSTLIst();

        //List<MenuMappingViewModel> GetADMENUPARAM_MSTLIst();
        dynamic GetSuggesionValue(string Key, int ParamType);

        dynamic GetMenuName(string Key);
        short AddMenuMappingData(MenuMappingViewModel model_, string userID);

        short EditMenuMappingData(string MapId, MenuMappingViewModel savedata, string userId);

    }
}
