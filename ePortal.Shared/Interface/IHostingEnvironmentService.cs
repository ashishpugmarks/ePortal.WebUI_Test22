using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Shared.Interface
{
    public interface IHostingEnvironmentService
    {
        string GetWebRootPath();
    }
}
