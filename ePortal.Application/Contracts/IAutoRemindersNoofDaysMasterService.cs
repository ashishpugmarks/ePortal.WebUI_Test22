using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IAutoRemindersNoofDaysMasterService
    {
        List<AutoRemindersNoofDaysMaster> GetAutoRemindersNoofDaysMasterData();

        AutoRemindersNoofDaysMaster GetAutoRemindersNoofDaysMasterBySrNo(int Id);

        bool Update_AutoRemindersNoofDaysMaster(int Id, int NoofDays, string UserId);
    }
}
