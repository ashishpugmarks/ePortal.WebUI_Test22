using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public enum VehicleType
    {
        FourWheeler = 1,
        TwoWheeler = 2
    }

    public static class VehicleTypeExtensions
    {
        public static string GetText(this VehicleType type)
        {
            return type switch
            {
                VehicleType.FourWheeler => "Four Wheeler",
                VehicleType.TwoWheeler => "Two Wheeler",
                _ => "Unknown"
            };
        }
    }
}

