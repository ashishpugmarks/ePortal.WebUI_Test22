
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using System.Data;

namespace ePortal.Application.Contracts
{
    public interface ILoginDetailService
    {
        List<VM_LoginDetail_SYApplication> BindApplication();
        SearchLoginDetail LoginDetailList(SearchLoginDetail VM);
        List<Employee_Details> PortalAutocompleteSuggestions(string term);
        AddLoginIDHeader SearchEmployee(AddLoginIDHeader vm);
        Tuple<short, long> SaveIDDetail(VM_EMP_LOGINDETAIL model);

        // Start Added by Aumento :: SR78338
        DataTable GetEmployeeLoginMapIDDetails(string MapFlag, string EmpFlag);
        // End Added by Aumento :: SR78338
    }
}
