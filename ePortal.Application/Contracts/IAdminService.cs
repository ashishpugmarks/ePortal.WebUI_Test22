using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface IAdminService
    {
        List<SelectListViewModel> GetLocationOperationByTypeId(int typeId);
        List<EmailGroupViewModel> GetEmailGroupList();
        Tuple<short, long> SaveEmailGroup(EmailGroupViewModel model);
        EmailGroupViewModel GetDetailById(long id);
    }
}
