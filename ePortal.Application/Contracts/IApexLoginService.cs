

namespace ePortal.Application.Contracts
{
    public interface IApexLoginService
    {
        string StoreToken(string userid, string token);
        }
}
