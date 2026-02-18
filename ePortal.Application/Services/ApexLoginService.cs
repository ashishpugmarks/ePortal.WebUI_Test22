
using ePortal.Application.Contracts;
using ePortal.Repositories;

namespace ePortal.Application.Services
{
    public class ApexLoginService: IApexLoginService
    {
        ApexLoginRepository _ApexRepo;
        public ApexLoginService()
        {
            _ApexRepo = new ApexLoginRepository();
        }
        public string StoreToken(string userid, string token)
        {
            return _ApexRepo.StoreToken(userid, token);
        }
        }
}
