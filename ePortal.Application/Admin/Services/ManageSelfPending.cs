using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Application.Admin.Contracts;
using ePortal.Application.Services;
using ePortal.Infrastructure.Admin;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Admin.Services
{
    public class ManageSelfPending : IManageSelfPending
    {
        private readonly ManageSelfPending_Repository _objloginRepositry;
        public ManageSelfPending(ManageSelfPending_Repository objloginRepositry)
        {
            _objloginRepositry = objloginRepositry;
        }

        public long GetTotalSelfPending(int strEmpCode)
        {
            return _objloginRepositry.GetTotalSelfPending(strEmpCode);
        }
    }
}

