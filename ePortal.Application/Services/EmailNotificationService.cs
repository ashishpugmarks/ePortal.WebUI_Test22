using ePortal.Application.Contracts;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ePortal.Application.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(IConfiguration configuration, ILogger<EmailNotificationService> logger)
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

        public bool SendItemSentBackNotification(LostAndFoundViewModel item, string userEmail, string userName)
        {
            string subject = $"Lost & Found Request Sent Back - {item.ProductName}";
            string body = $@"
                  <html><body>
                  <p>Dear {item.ReportedBy} San,</p>
                  <p>Your request ID <b>{item.Id}</b> raised for {item.ItemType} item has been sent back. 
                     You can check the details on e-Portal in 
                     <b>Employee Self-Services -> Security Services -> Lost & Found -> View request</b>.</p>
                  <br/>
                  <p>Thanks & Regards,<br/>Security Team</p>
                  </body></html>";
                
            return SendEmail(userEmail, subject, body);
        }

        public bool SendItemRejectedNotification(LostAndFoundViewModel item, string userEmail, string userName)
        {
            string subject = $"Lost & Found Request Rejected - {item.ProductName}";
            string body = $@"
                   <html><body>
                   <p>Dear {item.ReportedBy} San,</p>
                   <p>Your request ID <b>{item.Id}</b> raised for {item.ItemType} item has been auto cancelled as no such item found or reported to Security Desk.</p>
                   <br/>
                   <p>Thanks & Regards,<br/>Security Team</p>
                   </body></html>";

            return SendEmail(userEmail, subject, body);
        }

        public bool SendItemApprovedNotification(LostAndFoundViewModel item, string userEmail, string userName)
        {
            string subject = $"Lost & Found Request Approved - {item.ProductName}";
            string body = "";
            switch (item.ItemType)
            {
                case DomainClasses.Enums.LostAndFoundItemType.Found:
                    body = $@"
                    <html><body>
                    <p>Dear {item.ReportedBy} San,</p>
                    <p>Your request ID <b>{item.Id}</b> raised for {item.ItemType} item has been approved. 
                       You can check the details on e-Portal in 
                       <b>Employee Self-Services -> Security Services -> Lost & Found -> View request</b>.</p>
                    <br/>
                    <p>Thanks & Regards,<br/>Security Team</p>
                    </body></html>";
                    break;
                case DomainClasses.Enums.LostAndFoundItemType.Lost:
                    body = $@"
                    <html><body>
                    <p>Dear {item.CreatedBy} San,</p>
                    <p>Your request ID <b>{item.Id}</b> raised for {item.ItemType} item has been approved. 
                       You can collect the item from the Security Desk.</p>
                    <br/>
                    <p>Thanks & Regards,<br/>Security Team</p>
                    </body></html>";
                    break;
                default:
                    body = $@"
                    <html><body>
                    <p>Dear {item.ReportedBy} San,</p>
                    <p>Your request ID <b>{item.Id}</b> raised for {item.ItemType} item has been approved. 
                       You can check the details on e-Portal in 
                       <b>Employee Self-Services -> Security Services -> Lost & Found -> View request</b>.</p>
                    <br/>
                    <p>Thanks & Regards,<br/>Security Team</p>
                    </body></html>";
                    break;
            }

            return SendEmail(userEmail, subject, body);
        }

        public bool SendItemClaimedNotification(LostAndFoundViewModel item, string userEmail, string userName)
        {
            string subject = $"Lost & Found Request Closed - {item.ProductName}";
            string body = $@"
                <html><body>
                <p>Dear {item.ReportedBy} San,</p>
                <p>Your request ID <b>{item.Id}</b> raised for {item.ItemType} item has been claimed and the request is now closed.. 
                   You can check the details on e-Portal in 
                   <b>Employee Self-Services -> Security Services -> Lost & Found -> View request</b>.</p>
                <br/>
                <p>Thanks & Regards,<br/>Security Team</p>
                </body></html>";

            return SendEmail(userEmail, subject, body);
        }

    }
}
