using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class RoleUserMappingMasterServices : IRoleUserMappingMasterService
    {
        private readonly RoleUserMappingMasterRepository _RoleUserMappingMasterRepositoryRepo;

        public RoleUserMappingMasterServices(RoleUserMappingMasterRepository objRoleUserMappingMasterRepositoryRepo)
        {
            _RoleUserMappingMasterRepositoryRepo = objRoleUserMappingMasterRepositoryRepo;
        }
        #region ROLE MASTER
        public List<RoleUserMappingViewModel> GetRoleUserMppingList()
        {
            return _RoleUserMappingMasterRepositoryRepo.GetRoleUserMappingList();
        }

        public List<RoleUserMappingViewModel> GetRoleNameList()
        {
            return _RoleUserMappingMasterRepositoryRepo.GetRoleNameList();
        }

        public short AddRoleUserMappingData(RoleUserMappingViewModel savedata, string userId)
        {
            return _RoleUserMappingMasterRepositoryRepo.AddRoleUserMappingData(savedata, userId);
        }

        public short EditRoleUserMappingData(string mappingid, RoleUserMappingViewModel savedata, string userId)
        {
            return _RoleUserMappingMasterRepositoryRepo.EditRoleUserMappingData(mappingid, savedata, userId);
        }
        #endregion
    }
}
