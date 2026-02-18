using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public enum VehicleSendBackType
    {
        User = 1,
        Admin = 2
    }

    public static class VehicleSendBackTypeExtensions
    {
        public static string GetText(this VehicleSendBackType type)
        {
            return type switch
            {
                VehicleSendBackType.User => "User",
                VehicleSendBackType.Admin => "Admin",
                _ => "-"
            };
        }
    }
}
