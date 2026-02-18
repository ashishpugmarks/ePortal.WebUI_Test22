using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Persistence.Interface;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Services
{
    public class AdminService : IAdminService
    {
        AdminRepository _AdminRepo;
        CommonRepository _CommonRepo;
        Tuple<short, long> _retVal_tuple;

        public AdminService(AdminRepository adminRepository, CommonRepository commonRepository)
        {
            _AdminRepo = adminRepository;
            _CommonRepo = commonRepository;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        }


        List<SelectListViewModel> IAdminService.GetLocationOperationByTypeId(int typeId)
        {
            return _AdminRepo.GetLocation_OperationByTypeId(typeId);
        }

        public Tuple<short, long> SaveEmailGroup(EmailGroupViewModel model)
        {
            return _AdminRepo.SaveEmailGroup(model);
        }

        public EmailGroupViewModel GetDetailById(long id)
        {
            return _AdminRepo.GetDetailById(id);
        }

        public List<EmailGroupViewModel> GetEmailGroupList()
        {
            return _AdminRepo.GetEmailGroupList();
        }
    }
}

