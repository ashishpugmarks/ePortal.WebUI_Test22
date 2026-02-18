using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IPOAPowerScopeService
    {

        Tuple<short, long> SavePOADetail(List<ADPOWERSCOPEMASTERViewModel> AdPowerScopeList, string Ecode);

        Tuple<short, long> SavePower(string PCODE, string PNAME);
        List<POAPowerScopeADPOWERMASTERViewModel> GetPower(long empCode);

        //============Change Done on 27082022 For Add Other Category by (Aumento)=====================================================================================================================
        List<Employee_Details> PortalAutocompleteSuggestionsForPOA(string term, string designation);
        //===============================================================================================================================================================================================


        List<POAPowerScopeADPOWERMASTERViewModel> PortalAutocompleteSuggestionsForPOAPower(string term, string designation);
        List<Root> GetPOADetailsByECode(string Ecode, string Power);
        List<Root> GetPowerScopeViewDetailAllData();
        List<POAPowerScopeADPOWERMASTERViewModel> GetPowerMasterData();

        List<Root> GetPOADetailsSelectedByECode(string Ecode);
        List<Root> GetScopeByEmpCode(string empCode, string PowerCode);
        Tuple<short, List<POAPowerScopeADPOWERMASTERViewModel>> DeleteAttachment(string POWERCODE, string POWERNAME, long SRNO);
    }
}
