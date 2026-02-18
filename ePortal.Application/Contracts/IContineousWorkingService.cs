using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IContineousWorkingService
    {
        Task<List<ContineousWorkingRowViewModel>> ContineousWorkingDashboard(SearchContineousWorkingViewModel SWM, string empCode);
        
    }
}
