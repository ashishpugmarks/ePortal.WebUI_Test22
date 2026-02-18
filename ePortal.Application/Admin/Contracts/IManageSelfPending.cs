using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Admin.Contracts
{
    public interface IManageSelfPending
    {
        long GetTotalSelfPending(Int32 strEmpCode);
    }
}
