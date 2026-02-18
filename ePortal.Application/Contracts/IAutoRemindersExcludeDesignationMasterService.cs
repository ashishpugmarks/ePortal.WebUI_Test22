using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IAutoRemindersExcludeDesignationMasterService
    {
        List<AutoRemindersExcludeDesignationMaster> GetAutoRemindersExcludeDesignationMasterData(string UserId);

        bool Update_AutoRemindersExcludeDesignationMaster(int Id, string UserId);
    }
}
