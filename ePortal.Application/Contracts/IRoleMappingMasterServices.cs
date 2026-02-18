using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IRoleMappingMasterServices
    {
        List<RoleMappingViewModel> GetRoleMappingList();
        List<RoleMappingViewModel> GetRoleList();
        dynamic GetMenuId(string key);
        short AddRoleMappingData(RoleMappingViewModel model_, string userID);
        short EditRoleMappingData(string MappingId, RoleMappingViewModel model_, string userID);

        dynamic AutocomplitName(long menuid);
    }
}
