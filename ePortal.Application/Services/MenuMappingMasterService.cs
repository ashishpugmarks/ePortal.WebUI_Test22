using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class MenuMappingMasterService : IMenuMappingMasterService
    {
        private MenuMappingMasterRepository _MenuMappingMasterRepo;
        public MenuMappingMasterService(MenuMappingMasterRepository objMenuMappingMasterRepo)
        {
            _MenuMappingMasterRepo = objMenuMappingMasterRepo;
        }


        #region  MENU MAPPING MASTER

        public short AddMenuMappingData(MenuMappingViewModel model_, string userId)
        {
            return _MenuMappingMasterRepo.AddMenuMappingData(model_, userId);
        }

        public short EditMenuMappingData(string MapId, MenuMappingViewModel model_, string userId)
        {
            return _MenuMappingMasterRepo.EditMenuMappingData(MapId, model_, userId);
        }

        public List<MenuMappingViewModel> GetMappingList()
        {
            return _MenuMappingMasterRepo.GetMappingList();
        }

        public dynamic GetADMENUPARAM_MSTLIst()
        {
            return _MenuMappingMasterRepo.GetADMENUPARAM_MSTLIst();
        }

        //public List<MenuMappingViewModel> GetMappingList()
        //{
        //    return Repo.GetMappingList();
        //}

        public dynamic GetSuggesionValue(string Key, int ParamType)
        {
            return _MenuMappingMasterRepo.GetSuggesionValue(Key, ParamType);
        }

        public dynamic GetMenuName(string Key)
        {
            return _MenuMappingMasterRepo.GetMenuName(Key);
        }

        public List<string> GetMenuList()
        {
            throw new NotImplementedException();
        }

        //public List<MenuMappingViewModel> GetMappingDataforGrid(long id)
        //{
        //    return Repo.GetMappingDataforGrid(id);
        //}
        public List<MenuParamdata> GetMappingDataforGrid(long id)
        {
            return _MenuMappingMasterRepo.GetMappingDataforGrid(id);
        }
        #endregion
    }
}
