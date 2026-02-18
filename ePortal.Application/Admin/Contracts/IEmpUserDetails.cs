using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;

namespace ePortal.Application.Admin.Contracts
{
    public interface IEmpUserDetails
    {
        ADEMP_FAMILYDeclaration GetEmployeeDeclaration(string strEmpcode);
        Int32 DeleteFamilyDeclaration(string strEmpcode, string headerid);
    }
}
