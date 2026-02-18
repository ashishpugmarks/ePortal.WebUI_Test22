using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;

namespace ePortal.Application.Services
{
    public class POAPowerScopeService : IPOAPowerScopeService
    {
        private readonly POAPowerScopeRepository _POAPowerScopeRepo;              

        public POAPowerScopeService(POAPowerScopeRepository POAPowerScopeRepo)
        {
            _POAPowerScopeRepo = POAPowerScopeRepo;                       

        }

        public Tuple<short, long> SavePOADetail(List<ADPOWERSCOPEMASTERViewModel> AdPowerScopeList, string Ecode)
        {

            return _POAPowerScopeRepo.SavePOADetail(AdPowerScopeList, Ecode);

            //Tuple<short, long> _tuple = _POAPowerScopeRepo.SavePOADetail(AdPowerScopeList);
            ////if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && AdPowerScopeList.IsFinalSubmit == 1)
            ////{
            ////    SendMailByRequestor(_tuple.Item2);
            ////}
            //return _tuple;
        }
        public Tuple<short, long> SavePower(string PCODE, string PNAME)
        {

            return _POAPowerScopeRepo.SavePower(PCODE, PNAME);

        }

        public List<POAPowerScopeADPOWERMASTERViewModel> GetPower(long empCode)
        {
            return _POAPowerScopeRepo.GetPower(empCode);
        }

        //============Change Done on 27082022 by Aumento Team For Add Other Category==========================================================================================
        public List<Employee_Details> PortalAutocompleteSuggestionsForPOA(string term, string designation)
        {
            return _POAPowerScopeRepo.PortalAutocompleteSuggestionsForPOA(term, designation);
        }

        //=====================================================================================================================================================================

        public List<POAPowerScopeADPOWERMASTERViewModel> PortalAutocompleteSuggestionsForPOAPower(string term, string designation)
        {
            return _POAPowerScopeRepo.PortalAutocompleteSuggestionsForPOAPower(term, designation);
        }

        public List<Root> GetPOADetailsByECode(string Ecode, string Power)
        {
            return _POAPowerScopeRepo.GetPOADetailsByECode(Ecode, Power);
        }

        public List<Root> GetPowerScopeViewDetailAllData()
        {
            return _POAPowerScopeRepo.GetPowerScopeViewDetailAllData();
        }

        public List<POAPowerScopeADPOWERMASTERViewModel> GetPowerMasterData()
        {
            return _POAPowerScopeRepo.GetPowerMasterData();
        }

        public List<Root> GetPOADetailsSelectedByECode(string Ecode)
        {
            return _POAPowerScopeRepo.GetPOADetailsSelectedByECode(Ecode);
        }

        public List<Root> GetScopeByEmpCode(string ecode, string PowerCode)
        {
            return _POAPowerScopeRepo.GetScopeByEmpCode(ecode, PowerCode);
        }

        public Tuple<short, List<POAPowerScopeADPOWERMASTERViewModel>> DeleteAttachment(string POWERCODE, string POWERNAME, long SRNO)
        {
            List<POAPowerScopeADPOWERMASTERViewModel> iList = new List<POAPowerScopeADPOWERMASTERViewModel>();
            short retVal = _POAPowerScopeRepo.DeleteAttachment(POWERCODE, POWERNAME, SRNO);
            //if (retVal == 1)
            //{
            //    iList = _ITDIncomeTaxDeclarationRepo.GetAttachmentDetail(SRNO);
            //}
            Tuple<short, List<POAPowerScopeADPOWERMASTERViewModel>> _tuple = new Tuple<short, List<POAPowerScopeADPOWERMASTERViewModel>>(retVal, iList);
            return _tuple;
        }

    }
}

