using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;

namespace ePortal.Persistence.TravelBilling.Interface
{
    public interface IcCostCenterMaster
    {
        bool DeleteCostCenterBySrNo(string SrNo, string UserID);
        DataTable GetCostCenterMasterBySrNo(int id);
        DataTable GetCostCenterMasterList(string strSearchString);
        DataTable GetCostCenterMasterLogList(string ADCOSTCENTERID);
        bool ImportCostCenterMaster(ADCOSTCENTERMASTER inputJson);
        bool InsertUpdateadCostCenterMaster(ADCOSTCENTERMASTER inputJson);
    }
}
