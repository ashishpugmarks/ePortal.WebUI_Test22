using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.TravelBilling.Interface
{
    public interface IcHelpDataProvider
    {
        DataTable GetADDesignationsList();
        DataTable GetADEmployeesList(string Term, string ADDesigId);
        DataTable GetADVendorList(string Term);
        DataTable GetCostCenterList(string Term, string SiteId);
        DataTable GetSYDivisionList();
        DataTable GetSYPlantsList();
        DataTable GetSYSitesList();
        DataTable GetTaxCodeList();
        DataTable GetUserWiseSYPlantsList(string UserID);
        DataTable GetUserWiseSYSitesList(string UserID);
    }
}
