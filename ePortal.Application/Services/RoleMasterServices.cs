using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Services
{
    public class RoleMasterServices : IRoleMasterServices
    {
        private readonly RoleMasterRepository _RoleMasterrepo;
        public RoleMasterServices(RoleMasterRepository objRoleMasterrepo)
        {
            _RoleMasterrepo = objRoleMasterrepo;
        }
        #region ROLE MASTER
        public List<RoleViewModel> GetRoleList()
        {
            return _RoleMasterrepo.GetRoleList();
        }

        public dynamic GetEmpID(string Key)
        {
            return _RoleMasterrepo.GetEmpID(Key);
        }

        public short AddRoleData(RoleViewModel savedata, string userId)
        {
            return _RoleMasterrepo.AddRoleData(savedata, userId);
        }
        public List<RoleViewModel> GetProNameList()
        {
            return _RoleMasterrepo.GetProNameList();
        }
        public short EditRoleData(string RoleId, RoleViewModel savedata, string userId)
        {
            return _RoleMasterrepo.EditRoleData(RoleId, savedata, userId);
        }

        public dynamic AutocomplitName(long Ecode)
        {
            return _RoleMasterrepo.AutocomplitName(Ecode);
        }
        #endregion
    }
}
