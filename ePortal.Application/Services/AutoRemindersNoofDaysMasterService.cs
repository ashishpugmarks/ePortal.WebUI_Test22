using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class AutoRemindersNoofDaysMasterService : IAutoRemindersNoofDaysMasterService
    {
        private readonly AutoRemindersNoofDaysMasterRepository _objAutoRemindersNoofDaysMasterRepository;
        public AutoRemindersNoofDaysMasterService(AutoRemindersNoofDaysMasterRepository objAutoRemindersNoofDaysMasterRepository)
        {
            _objAutoRemindersNoofDaysMasterRepository = objAutoRemindersNoofDaysMasterRepository;
        }

        public List<AutoRemindersNoofDaysMaster> GetAutoRemindersNoofDaysMasterData()
        {
            var objresult = _objAutoRemindersNoofDaysMasterRepository.GetAutoRemindersNoofDaysMasterList();
            return objresult;
        }

        public AutoRemindersNoofDaysMaster GetAutoRemindersNoofDaysMasterBySrNo(int Id)
        {
            var objresult = _objAutoRemindersNoofDaysMasterRepository.GetDataBySrNo(Id);
            return objresult;
        }

        public bool Update_AutoRemindersNoofDaysMaster(int Id, int NoofDays, string UserId)
        {
            var objresult = _objAutoRemindersNoofDaysMasterRepository.UpdateData(Id, NoofDays, UserId);
            return objresult;
        }
    }
}
