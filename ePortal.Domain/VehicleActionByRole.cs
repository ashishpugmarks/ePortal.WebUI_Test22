using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public enum VehicleActionByRole
    {
        Admin = 1,
        Security = 2,
        User = 3,
    }

    public static class VehicleActionByRoleExtensions
    {
        public static string GetText(this VehicleActionByRole role)
        {
            return role switch
            {
                VehicleActionByRole.Admin => "Admin",
                VehicleActionByRole.Security => "Security",
                VehicleActionByRole.User => "User",
                _ => "-"
            };
        }
    }
}
