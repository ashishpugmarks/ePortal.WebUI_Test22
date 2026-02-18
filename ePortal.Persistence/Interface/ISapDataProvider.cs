using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
    public interface ISapDataProvider
    {
        Task<string> SendPostRequestAsync(string endpoint, object payload);
    }
}
