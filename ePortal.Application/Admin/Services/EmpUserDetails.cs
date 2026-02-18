using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Application.Admin.Contracts;
using ePortal.Infrastructure.Admin;
using ePortal.ViewModels;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Application.Admin.Services
{
    public class EmpUserDetails: IEmpUserDetails
    {
        private readonly EmpUserDetails_Repository _objloginRepositry;
        public EmpUserDetails(EmpUserDetails_Repository objloginRepositry)
        {
            _objloginRepositry = objloginRepositry;
        }
        ADEMP_FAMILYDeclaration IEmpUserDetails.GetEmployeeDeclaration(string strEmpcode)
        {
            //return GetEmployeeDeclaration(strEmpcode);
            return null;
        }

        int IEmpUserDetails.DeleteFamilyDeclaration(string strEmpcode, string headerid)
        {
            return _objloginRepositry.DeleteFamilyDeclaration(strEmpcode, headerid);
        }
    }
}
