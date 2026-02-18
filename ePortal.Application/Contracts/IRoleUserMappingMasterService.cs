using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IRoleUserMappingMasterService
    {
        List<RoleUserMappingViewModel> GetRoleUserMppingList();

        List<RoleUserMappingViewModel> GetRoleNameList();
        short AddRoleUserMappingData(RoleUserMappingViewModel savedata, string userId);
        short EditRoleUserMappingData(string mappingId, RoleUserMappingViewModel model_, string userID);
    }
}
