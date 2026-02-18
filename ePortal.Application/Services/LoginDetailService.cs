
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Repositories;
using ePortal.ViewModels;
using System.Web;
using System.Data;
using ePortal.Application.Contracts;

namespace ePortal.Application.Services
{
    public class LoginDetailService : ILoginDetailService
    {

        LoginDetailRepository _lgRepo;
        EmployeeLoginRepository _CommonRepo;
        CommonFunctions cf;
        Tuple<short, long> _retVal_tuple;

        public LoginDetailService()
        {
            _lgRepo = new LoginDetailRepository();
            _CommonRepo = new EmployeeLoginRepository();
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
            cf = new CommonFunctions();
        }

        public List<VM_LoginDetail_SYApplication> BindApplication()
        {
            return _lgRepo.BindApplication();
        }
        public SearchLoginDetail LoginDetailList(SearchLoginDetail VM)
        {
            return VM;
        }
        public List<Employee_Details> PortalAutocompleteSuggestions(string term)
        {
            return _lgRepo.PortalAutocompleteSuggestions(term);
        }
        public AddLoginIDHeader SearchEmployee(AddLoginIDHeader VM)
        {
            return _lgRepo.LoginDetail(VM);        
        }
        public Tuple<short, long> SaveIDDetail(VM_EMP_LOGINDETAIL model)
        { 
            return _lgRepo.SaveIDDetail(model);
        }
        // Start Added by Aumento :: SR78338

        public DataTable GetEmployeeLoginMapIDDetails(string MapFlag, string EmpFlag)
        {
            return cf.GetEmployeeLoginMapIDDetails(MapFlag, EmpFlag);
        }
        // End Added by Aumento :: SR78338
        //-------------------------------------------------------------------------------------------------------------------------------------------

    }
}

