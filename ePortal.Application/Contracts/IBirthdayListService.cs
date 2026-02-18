using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface IBirthdayListService
    {
        List<Root> GetBirthdayListData();
    }
}
