using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.Application.Contracts
{
    public interface IVehicleEmailNotificationService
    {
        bool SendApproveByAdminNotification(VehicleMasterDto vehicle, string userEmail);
        bool SendApproveBySecurityNotification(VehicleMasterDto vehicle, string userEmail);
        bool SendSendBackNotification(VehicleMasterDto vehicle, string userEmail);
        bool SendHoldNotification(VehicleMasterDto vehicle, string userEmail);
        bool SendRejectNotification(VehicleMasterDto vehicle, string userEmail);
        bool SendDeactivateNotification(VehicleMasterDto vehicle, string userEmail);
        bool SendVehicleRemoveNotification(VehicleMasterDto vehicle, string userEmail, string adminEmail, string securityEmail);

		bool SendRequestEmailToAdminNotification(VehicleMasterDto vehicle, List<string> userEmail);
		bool SendApproveByAdminEmailToSecurityNotification(VehicleMasterDto vehicle, List<string> SecuritypersonalEmail ,EmployeeDetailDto Admindetails);
	}
}
