using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
    public interface IConnectionString
    {
        string getConnectingString();
        string getConnectingStringVQMS(String Factory);
        string strConnectionString();
    }
}
