using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class AutoRemindersExcludeDesignationMasterService : IAutoRemindersExcludeDesignationMasterService
    {
        private readonly AutoRemindersExcludeDesignationMasterRepository _objAutoRemindersExcludeDesignationMasterRepository;
        public AutoRemindersExcludeDesignationMasterService(AutoRemindersExcludeDesignationMasterRepository objAutoRemindersExcludeDesignationMasterRepository)
        {
            _objAutoRemindersExcludeDesignationMasterRepository = objAutoRemindersExcludeDesignationMasterRepository;
        }
        public List<AutoRemindersExcludeDesignationMaster> GetAutoRemindersExcludeDesignationMasterData(string UserId)
        {
            var objresult = _objAutoRemindersExcludeDesignationMasterRepository.GetAutoRemindersExcludeDesignationMasterList(UserId);
            return objresult;
        }

        public bool Update_AutoRemindersExcludeDesignationMaster(int Id, string UserId)
        {
            var objresult = _objAutoRemindersExcludeDesignationMasterRepository.UpdateData(Id, UserId);
            return objresult;
        }
    }
}
