using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Shared.Configuration;

namespace ePortal.Shared.Interface
{
    public interface IAppConfigurationService
    {
        string GetConnectionString(string name);
        GeneralSettings GetGeneralSettings();

        string MapPath(string relativePath);

        string GetServerName();
    }
}
