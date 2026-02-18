using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IProcessMasterService
    {
        List<ProcessMasterViewModel> GetMenuList();
        short AddProcess(string ProcessName, string status, string UserID);
        short EditProcess(string ProcessID, string ProcessName, string status, string UserID);
    }
}
