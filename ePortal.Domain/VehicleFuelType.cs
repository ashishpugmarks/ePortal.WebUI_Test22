using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public enum VehicleFuelType
    {
        Diesel = 1,
        Petrol = 2,
        Electrical = 3
    }

    public static class VehicleFuelTypeExtensions
    {
        public static string GetText(this VehicleFuelType type)
        {
            return type switch
            {
                VehicleFuelType.Diesel => "Diesel",
                VehicleFuelType.Petrol => "Petrol",
                VehicleFuelType.Electrical => "Electrical",
                _ => "Unknown"
            };
        }
    }
}

