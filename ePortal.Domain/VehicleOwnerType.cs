using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public enum VehicleOwnerType
    {
        Self = 1,
        Spouse = 2,
        Parents = 3,
        Son = 4,
        Daughter = 5,
    }

    public static class VehicleOwnerTypeExtensions
    {
        public static string GetText(this VehicleOwnerType role)
        {
            return role switch
            {
                VehicleOwnerType.Self => "Self",
                VehicleOwnerType.Spouse => "Spouse",
                VehicleOwnerType.Parents => "Parents",
                VehicleOwnerType.Son => "Son",
                VehicleOwnerType.Daughter => "Daughter",
                _ => "-"
            };
        }
    }
}
