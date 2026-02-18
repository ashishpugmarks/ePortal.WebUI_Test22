using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Services
{
    public class BirthdayListService : IBirthdayListService
    {
        private readonly BirthdayListRepository _objBirthdayListRepositry;
        private readonly CommonRepository _objcommRespository;

        //BirthdayListRepository _BirthdayListRepo;
        //CommonRepository _CommonRepo;
        Tuple<short, long> _retVal_tuple;

        public BirthdayListService(BirthdayListRepository objBirthdayListRepositry, CommonRepository objcommRespository)
        {
            _objBirthdayListRepositry = objBirthdayListRepositry;
            _objcommRespository = objcommRespository;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);

        }

        public List<Root> GetBirthdayListData()
        {
            return _objBirthdayListRepositry.GetBirthdayListData();
        }


    }
}

