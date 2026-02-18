using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Application.Contracts;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.Application.Services
{
    public class VehicleEmailNotificationService : IVehicleEmailNotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VehicleEmailNotificationService> _logger;
        public VehicleEmailNotificationService(IConfiguration configuration, ILogger<VehicleEmailNotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = toEmail;
                sendMail.MailSubject = subject;
                sendMail.MailBody = body;
                bool status = sendMail.Send();
                if (status)
                    _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                else
                    _logger.LogWarning("Email sending failed to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                return false;
            }
        }

        public bool SendApproveByAdminNotification(VehicleMasterDto vehicle, string userEmail )
        {
            string subject = $"Vehicle Request Approved by Admin - {vehicle.VehicleId}";
            string body = $@"<html><body>
            <p>Dear {vehicle.EmpName} San,</p>
            
            <p>
            Your vehicle request ID <b>{vehicle.VehicleId}</b> has been <b>approved by Admin</b>.
            The request has been forwarded for Security verification.
            You can check the status on e-Portal in
            <b>Employee Self-Services -> Security Services -> View Vehicle Request</b>.
            </p>
            
            <br/>
            
            <p>
            Thanks & Regards,<br/>
            Admin Team
            </p>
            
            </body></html>";
            return SendEmail(userEmail, subject, body);
        }

        public bool SendApproveBySecurityNotification(VehicleMasterDto vehicle, string userEmail )
        {
            string subject = $"Vehicle Request Approved by Security - {vehicle.VehicleId}";
            string body = $@"<html><body>
            <p>Dear {vehicle.EmpName} San,</p>
            
            <p>
            Your vehicle request ID <b>{vehicle.VehicleId}</b> has been <b>approved by Security</b>.
            The vehicle request process is now completed.
            You can check the final details on e-Portal in
            <b>Employee Self-Services -> Security Services -> View Vehicle Request</b>.
            </p>
            
            <br/>
            
            <p>
            Thanks & Regards,<br/>
            Security Team
            </p>
            
            </body></html>";

            return SendEmail(userEmail, subject, body);
        }

        public bool SendHoldNotification(VehicleMasterDto vehicle, string userEmail )
        {
            string subject = $"Vehicle Request On Hold - {vehicle.VehicleId}";
            string body = $@"<html><body>
            <p>Dear {vehicle.EmpName} San,</p>
            
            <p>
            Your vehicle request ID <b>{vehicle.VehicleId}</b> is currently <b>on hold</b>
            for further review or verification.
            You can check the details on e-Portal in
            <b>Employee Self-Services -> Security Services -> View Vehicle Request</b>.
            </p>
            
            <br/>
            
            <p>
            Thanks & Regards,<br/>
            Security Team
            </p>
            
            </body></html>";
            return SendEmail(userEmail, subject, body);
        }

        public bool SendRejectNotification(VehicleMasterDto vehicle, string userEmail )
        {
            string subject = $"Vehicle Request Rejected - {vehicle.VehicleId}";
            string body = $@"<html><body>
            <p>Dear {vehicle.EmpName} San,</p>
            
            <p>
            Your vehicle request ID <b>{vehicle.VehicleId}</b> has been <b>rejected</b>.
            You can check the details on e-Portal in
            <b>Employee Self-Services -> Security Services -> View Vehicle Request</b>.
            </p>
            
            <br/>
            
            <p>
            Thanks & Regards,<br/>
            Security Team
            </p>
            
            </body></html>";
            return SendEmail(userEmail, subject, body);
        }

        public bool SendSendBackNotification(VehicleMasterDto vehicle, string userEmail )
        {
            var remark = vehicle.VehicleHistory?.OrderByDescending(x => x.Id).FirstOrDefault()?.SpRemark;
            string subject = $"Vehicle Request Sent Back - {vehicle.VehicleId}";
            string body = $@"<html><body>
            <p>Dear {vehicle.EmpName} San,</p>
            
            <p>
            Your vehicle request ID <b>{vehicle.VehicleId}</b> has been sent back for correction.
            </p>
            
            {(!string.IsNullOrWhiteSpace(remark)
                            ? $"<p><b>Remark:</b> {remark}</p>"
                            : string.Empty)}
            
            <p>
            You can check the details on e-Portal in
            <b>Employee Self-Services -> Security Services -> View Vehicle Request</b>.
            </p>
            
            <br/>
            
            <p>
            Thanks & Regards,<br/>
            Security Team
            </p>
            
            </body></html>";

            return SendEmail(userEmail, subject, body);
        }

        public bool SendDeactivateNotification(VehicleMasterDto vehicle, string userEmail )
        {
            var remark = vehicle.VehicleHistory?
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault()?
                                .SpRemark;

            string subject = $"Vehicle Deactivated - {vehicle.VehicleId}";

            string body = $@"<html><body>
                         <p>Dear {vehicle.EmpName} San,</p>
                         
                         <p>
                             Your vehicle associated with request ID 
                             <b>{vehicle.VehicleId}</b> has been <b>deactivated</b>.
                         </p>

                         {(!string.IsNullOrWhiteSpace(remark)
                                     ? $"<p><b>Reason:</b> {remark}</p>"
                                     : string.Empty)}

                         <p>
                             You can view the details on e-Portal in
                             <b>Employee Self-Services 🡪 Security Services 🡪 View Vehicle Request</b>.
                         </p>

                         <br/>

                         <p>
                             Thanks & Regards,<br/>
                             Security Team
                         </p>
                        </body></html>";

            return SendEmail(userEmail, subject, body);
        }

        public bool SendVehicleRemoveNotification(
            VehicleMasterDto vehicle,
            string userEmail,
            string adminEmail,
            string securityEmail)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            if (string.IsNullOrWhiteSpace(userEmail))
                return false;

            try
            {
                const string subject = "Vehicle Request Removed";

                var userBody = BuildUserVehicleRemovedEmail(vehicle);
                SendEmail(userEmail, subject, userBody);

                if (!string.IsNullOrWhiteSpace(adminEmail))
                {
                    var adminBody = BuildAdminVehicleRemovedEmail(vehicle);
                    SendEmail(adminEmail, subject, adminBody);
                }

                if (!string.IsNullOrWhiteSpace(securityEmail))
                {
                    var securityBody = BuildSecurityVehicleRemovedEmail(vehicle);
                    SendEmail(securityEmail, subject, securityBody);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private string BuildAdminVehicleRemovedEmail(VehicleMasterDto vehicle)
        {
            return $@"
<html>
<body>
    <p>Dear Admin Team,</p>

    <p>
        This is to notify you that the vehicle request with ID
        <b>{vehicle.VehicleId}</b> has been <b>removed by the user</b>
        after final approval.
    </p>

    <p>
        <b>Details:</b><br/>
        Employee Name: {vehicle.EmpName}<br/>
        Employee ID: {vehicle.EmpCode}<br/>
        Vehicle Id: {vehicle.VehicleId}
    </p>

    <p>
        No action is required from the Admin side.
        This is an informational notification only.
    </p>

    <br/>

    <p>
        Regards,<br/>
        System Notification
    </p>
</body>
</html>";
        }


        private string BuildSecurityVehicleRemovedEmail(VehicleMasterDto vehicle)
        {
            return $@"
<html>
<body>
    <p>Dear Security Team,</p>

    <p>
        This is to inform you that the vehicle request with ID
        <b>{vehicle.VehicleId}</b> has been <b>removed by the user</b>
        after approval.
    </p>

    <p>
        <b>Request Info:</b><br/>
        Employee Name: {vehicle.EmpName}<br/>        
        Vehicle Id: {vehicle.VehicleId}
    </p>

    <p>
        This message is for notification purpose only.
        No further action is required.
    </p>

    <br/>

    <p>
        Thanks & Regards,<br/>
        System Notification
    </p>
</body>
</html>";
        }



        private string BuildUserVehicleRemovedEmail(VehicleMasterDto vehicle)
        {
            return $@"
    <html>
    <body>
        <p>Dear San,</p>
    
        <p>
            Your vehicle request with ID
            <b>{vehicle.VehicleId}</b> has been successfully <b>removed</b>.
        </p>
    
        <p>
            <b>Details:</b><br/>
            Employee Name: {vehicle.EmpName}<br/>
            Employee ID: {vehicle.EmpCode}<br/>
            Vehicle Id: {vehicle.VehicleId}
        </p>
    
    
        <p>
            This email is a confirmation for your reference.
        </p>
    
        <br/>
    
        <p>
            Regards,<br/>
            System Notification
        </p>
    </body>
    </html>";
            }

		public bool SendRequestEmailToAdminNotification(
			VehicleMasterDto vehicle,
			List<string> userEmail)
		{
			if (userEmail == null || !userEmail.Any())
				return false;

			string subject = $"New Vehicle Request Raised - {vehicle.VehicleId} From {vehicle.EmpName} San";

			string body = $@"<html><body>
        <p>Dear San,</p>

        <p>
            A new <b>Vehicle Request</b> has been raised by an Hmsi User.
            The request details are as follows:
        </p>

        <table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;'>
            <tr>
                <td><b>Employee Name</b></td>
                <td>{vehicle.EmpName}</td>
            </tr>
            <tr>
                <td><b>Employee Code / User ID</b></td>
                <td>{vehicle.EmpCode}</td>
            </tr>
            <tr>
                <td><b>Location</b></td>
                <td>{vehicle.LocationName}</td>
            </tr>
            <tr>
                <td><b>Vehicle Request ID</b></td>
                <td>{vehicle.VehicleId}</td>
            </tr>
        </table>

        <br/>

        <p>
            Please review and take necessary action by logging into the e-Portal.
        </p>

        <p>
            Thanks & Regards,<br/>
            e-Portal System
        </p>

    </body></html>";

			bool isSent = true;

			foreach (var email in userEmail)
			{
		
					var result = SendEmail(email, subject, body);

			}

			return isSent;
		}

		public bool SendApproveByAdminEmailToSecurityNotification(
		VehicleMasterDto vehicle,
		List<string> securityPersonalEmail,
		EmployeeDetailDto adminDetails)
		{
			if (securityPersonalEmail == null || !securityPersonalEmail.Any())
				return false;

			string subject = $"Vehicle Request Approved - {vehicle.VehicleId}";

			string body = $@"<html><body>
                        <p>Dear San,</p>
                    
                        <p>
                            Hi <b>{adminDetails.FullName}</b> (<b>{adminDetails.EmpCode}</b>) has approved
                            the vehicle request for the following vehicle:
                        </p>
                    
                        <table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;'>
                            <tr>
                                <td><b>Vehicle Request ID</b></td>
                                <td>{vehicle.VehicleId}</td>
                            </tr>                           
                            <tr>
                                <td><b>Location</b></td>
                                <td>{vehicle.LocationName}</td>
                            </tr>
                        </table>
                    
                        <br/>
                    
                        <p>
                            Please check the request by navigating to:
                            <br/>
                            <b>Admin → Vehicle Portal → Vehicle Security Request</b>
                        </p>
                    
                        <p>
                            You can view the request from there and take the required action.
                        </p>
                    
                        <br/>
                    
                        <p>
                            Thanks & Regards,<br/>
                            Admin Team
                        </p>
                    
                    </body></html>";

			bool isSent = true;

			foreach (var email in securityPersonalEmail)
			{
				var result = SendEmail(email, subject, body);
				// isSent = isSent && result; // optional strict check
			}

			return isSent;
		}

	}
}
