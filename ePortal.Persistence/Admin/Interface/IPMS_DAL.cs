using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IPMS_DAL
    {
        string GetKIId();
        DataTable GetKiList();
    }
}
