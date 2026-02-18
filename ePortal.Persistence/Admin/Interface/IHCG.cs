using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IHCG
    {
        int AddHCG(string hcgid, string series, string attachment, string status, string strhcgtype, int AddedBy, string strreleasedate);
        DataSet gethcgDetails();
    }
}
