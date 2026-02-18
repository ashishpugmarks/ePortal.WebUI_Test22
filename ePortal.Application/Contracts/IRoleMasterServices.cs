using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IRoleMasterServices
    {
        List<RoleViewModel> GetRoleList();
        List<RoleViewModel> GetProNameList();
        dynamic GetEmpID(string Key);
        short AddRoleData(RoleViewModel savedata, string userId);
        short EditRoleData(string RoleId, RoleViewModel savedata, string userId);

        dynamic AutocomplitName(long Ecode);
    }
}
