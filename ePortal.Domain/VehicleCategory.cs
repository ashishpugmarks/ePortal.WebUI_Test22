using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public enum VehicleCategory
    {
        SelfDriver = 1,
        PersonalDriver = 2,
        CompanyDriver = 3
    }

    public static class VehicleCategoryExtensions
    {
        public static string GetText(this VehicleCategory category)
        {
            return category switch
            {
                VehicleCategory.SelfDriver => "Self Driver",
                VehicleCategory.PersonalDriver => "Personal Driver",
                VehicleCategory.CompanyDriver => "Company Driver",
                _ => "Unknown"
            };
        }
    }
}

