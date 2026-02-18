using ePortal.ViewModels;
using System.Collections.Generic;

namespace ePortal.Application.Contracts
{
    public interface ISummerTraineeMasterService
    {
        List<SummerTraineeViewModel> oplist(long obj);

        short Delete(long opID);
        short Add(long obj, string empcode);
    }
}
