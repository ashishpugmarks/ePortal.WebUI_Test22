using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
    public interface IEssServices
    {

        DataTable GET_EMPACCSTATEMENT_DATATABLE(string userid, string subtype, string monthfrom, string monthto, string finyear);

    }
}
