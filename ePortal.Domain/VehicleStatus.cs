using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public enum VehicleStatus
    {
        Pending = 0,
        Approved = 1,
        SendBack = 2,
        Reject = 3,
        Hold = 4,
        Deactivated = 5,
    }

    public static class VehicleStatusExtensions
    {
        public static string GetText(this VehicleStatus status)
        {
            return status switch
            {
                VehicleStatus.Pending => "Pending Review",
                VehicleStatus.Approved => "Approved",
                VehicleStatus.SendBack => "Send Back",
                VehicleStatus.Reject => "Reject",
                VehicleStatus.Hold => "Hold",
                VehicleStatus.Deactivated => "Deactivated",
                _ => "Unknown"
            };
        }
    }
}
