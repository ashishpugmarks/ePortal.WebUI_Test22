using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;
using System.Collections.Generic;

namespace ePortal.Application.Services
{
    public class SummerTraineeMasterService : ISummerTraineeMasterService
    {
        private readonly SummerTraineeMasterRepository _STMRepo;
        private readonly CommonRepository _CommonRepo;
        public SummerTraineeMasterService(SummerTraineeMasterRepository objSTMRepo, CommonRepository objCommonRepo)
        {
            _STMRepo = objSTMRepo;
            _CommonRepo = objCommonRepo;
        }

        public List<SummerTraineeViewModel> oplist(long obj)
        {
            return _STMRepo.oplist(obj);
        }
        public short Delete(long opID)
        {
            return _STMRepo.Delete(opID);
        }
        public short Add(long obj, string empcode)
        {
            return _STMRepo.Add(obj, empcode);
        }
        //public string StoreToken(string userid, string token)
        //{
        //    return _ApexRepo.StoreToken(userid, token);
        //}
    }
}
