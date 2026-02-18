using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public enum VehicleDLType
    {
        Learner = 1,
        Permanent = 2
    }

    public static class VehicleDLTypeExtensions
    {
        public static string GetText(this VehicleDLType type)
        {
            return type switch
            {
                VehicleDLType.Learner => "Learner",
                VehicleDLType.Permanent => "Permanent",
                _ => "Unknown"
            };
        }
    }
}

