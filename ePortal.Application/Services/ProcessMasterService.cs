using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Services
{
    public class ProcessMasterService : IProcessMasterService
    {
        private ProcessMasterRepository _ProcessMasterRepo;
        public ProcessMasterService(ProcessMasterRepository objProcessMasterRepo)
        {
            _ProcessMasterRepo = objProcessMasterRepo;
        }

        public List<ProcessMasterViewModel> GetMenuList()
        {
            return _ProcessMasterRepo.GetMenuList();

        }

        public short AddProcess(string ProcessName, string status, string UserID)
        {

            return _ProcessMasterRepo.AddProcess(ProcessName, status, UserID);

        }

        public short EditProcess(string ProcessID, string ProcessName, string status, string UserID)
        {

            return _ProcessMasterRepo.EditProcess(ProcessID, ProcessName, status, UserID);

        }


    }
}
