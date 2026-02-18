using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IFormAService
    {
        Tuple<short, long> SaveFormARequest(FormAHeaderViewModel model);

        short SaveFormADetails(long AddedBy, long FORMAHEADERID, FormADetailViewModel _formADetail);

        List<FormAHeaderViewModel> GetFormAList();
        FormAHeaderViewModel GetFormARequestById(long id);
        short UpdateByFormId(long FormAId, long updatedBy);
        List<Employee_Details> PortalAutocompleteSuggestions(string Key);
    }
}
