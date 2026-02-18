using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Services
{
    public class RoleMappingMasterServices : IRoleMappingMasterServices
    {
        private readonly RoleMappingMasterRepository _RoleMappingMasterRepository;
        public RoleMappingMasterServices(RoleMappingMasterRepository objRoleMappingMasterRepository)
        {
            _RoleMappingMasterRepository = objRoleMappingMasterRepository;
        }
        public List<RoleMappingViewModel> GetRoleMappingList()
        {
            return _RoleMappingMasterRepository.GetRoleMappingList();
        }
        public List<RoleMappingViewModel> GetRoleList()
        {
            return _RoleMappingMasterRepository.GetRoleList();
        }

        public dynamic GetMenuId(string Key)
        {
            return _RoleMappingMasterRepository.GetMenuId(Key);
        }
        public short AddRoleMappingData(RoleMappingViewModel model_, string userID)
        {
            return _RoleMappingMasterRepository.AddRoleMappingData(model_, userID);
        }

        public short EditRoleMappingData(string MappingId, RoleMappingViewModel model_, string userID)
        {
            return _RoleMappingMasterRepository.EditRoleMappingData(MappingId, model_, userID);
        }

        public dynamic AutocomplitName(long menuid)
        {
            return _RoleMappingMasterRepository.AutocomplitName(menuid);
        }
    }
}
