using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace ePortal.Shared
{
    public class Mailer
    {
        public enum AlertColor
        {
            Info,
            Success,
            Danger
        }
        public static bool sendMail(EmailConfig obj)
        {
            try
            {
                using (var email = new MailMessage())
                {
                    email.From = new MailAddress("portal.admin@honda.hmsi.in", "Employee Portal Mailer");
                    foreach (var address in obj.MailTo.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        email.To.Add(address.Trim());

                    if (!string.IsNullOrWhiteSpace(obj.MailCc))
                    {
                        foreach (var cc in obj.MailCc.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            email.CC.Add(cc.Trim());
                    }

                    email.Subject = obj.MailSubject;
                    email.Body = obj.MailBody;
                    email.IsBodyHtml = true;

                    // Attach files by path
                    foreach (var file in obj.AttachmentFilePaths)
                    {
                        if (File.Exists(file))
                            email.Attachments.Add(new Attachment(file, MediaTypeNames.Application.Octet));
                    }

                    // Attach in-memory byte[] files
                    foreach (var att in obj.AttachmentBytes)
                    {
                        var stream = new MemoryStream(att.FileBytes);
                        var attachment = new Attachment(stream, att.FileName, MediaTypeNames.Application.Octet);
                        email.Attachments.Add(attachment);
                    }

                    using (var smtp = new SmtpClient("10.116.16.147", 25))
                    {
                        smtp.Send(email);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static string generateFormat(FormatConfig obj)
        {
            return $@"<!DOCTYPE html>
            <html lang=""en"" style=""margin:0;padding:0;"">
              <head>
                <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                <title>Email</title>
                <style>
                  /* Mobile responsiveness */
                  @@media only screen and (max-width: 720px) {{
                    .container {{
                      width: 100% !important;
                      border-radius: 0 !important;
                    }}
                    .table-content {{
                      font-size: 13px !important;
                    }}
                  }}
                </style>
              </head>
              <body style=""margin:0;padding:0;background:#f5f7fa;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;"">
                <!-- Full width wrapper -->
                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""background:#f5f7fa;"">
                  <tr>
                    <td align=""center"" style=""padding:24px 12px;"">
                      <!-- Responsive container -->
                      <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" class=""container""
                             style=""max-width:700px;background:#ffffff;border:1px solid #e2e8f0;border-radius:8px;overflow:hidden;"">
        
                        <!-- Panel title -->
                        <tr>
                          <td style=""background:{ToCssColor(obj.alertColor)};color:#ffffff;font-family:Segoe UI, Roboto, Helvetica, Arial, sans-serif;
                                     font-size:18px;line-height:24px;font-weight:600;padding:14px 18px;border-bottom:1px solid #0b1223;"">
                            {obj.title}
                          </td>
                        </tr>
        
                        <!-- Intro text -->
                        <tr>
                          <td style=""padding:18px;font-family:Segoe UI, Roboto, Helvetica, Arial, sans-serif;color:#0f172a;
                                     font-size:15px;line-height:22px;"">
                            {obj.openingLine}
                          </td>
                        </tr>
        
                        <!-- Table Wrapper -->
		                {obj.tableContent}
        
                        <!-- Closing line -->
                        <tr>
                          <td style=""padding:12px 18px 18px 18px;font-family:Segoe UI, Roboto, Helvetica, Arial, sans-serif;
                                     color:#334155;font-size:14px;line-height:20px;"">
                            {obj.closingLine}
                          </td>
                        </tr>
        
                        <!-- Footer -->
                        <tr>
                          <td style=""padding:12px 18px;background:#f8fafc;border-top:1px solid #e2e8f0;
                                     font-family:Segoe UI, Roboto, Helvetica, Arial, sans-serif;
                                     color:#64748b;font-size:12px;line-height:18px;
                                     border-bottom-left-radius:8px;border-bottom-right-radius:8px;"">
                            This is an automated notification. Please do not reply to this email.
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </body>
            </html>";
        }
        public static string GenerateDynamicTable(IEnumerable<object> data)
        {
            if (data == null || !data.Any())
                return "<p>No data available</p>";

            var props = data.First().GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var sbHeader = new StringBuilder();
            var sbRows = new StringBuilder();

            sbHeader.Append("<tr style=\"background:#1e293b;color:#ffffff;text-align:left;\">");
            foreach (var prop in props)
            {
                sbHeader.Append($@"<th align=""left"" style=""border-bottom:1px solid #cbd5e1;"">{prop.Name}</th>");
            }
            sbHeader.Append("</tr>");

            bool altRow = false;
            foreach (var item in data)
            {
                string bgColor = altRow ? "#f8fafc" : "#ffffff";
                sbRows.Append($@"<tr style=""background:{bgColor};"">");
                foreach (var prop in props)
                {
                    var value = prop.GetValue(item) ?? "";
                    sbRows.Append($@"<td style=""border-bottom:1px solid #cbd5e1;"">{value}</td>");
                }
                sbRows.Append("</tr>");
                altRow = !altRow;
            }

            return ($@"
            <tr>
                <td style=""padding:0 18px 18px 18px;"">
                    <table cellpadding=""0"" cellspacing=""0"" width=""100%"" 
                            style=""border:1px solid #cbd5e1; border-radius:6px; overflow:hidden;"">
                        <tr>
                            <td>
                                <table cellpadding=""6"" cellspacing=""0"" width=""100%""
                                        style=""border-collapse:collapse;
                                                font-family:Segoe UI, Roboto, Helvetica, Arial, sans-serif;
                                                font-size:14px;line-height:20px;color:#0f172a;"">
                                    <thead>
                                        {sbHeader}
                                    </thead>
                                    <tbody>
                                        {sbRows}
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>");
        }
        private static string ToCssColor(AlertColor color)
        {
            switch (color)
            {
                case AlertColor.Info:
                    return "#6c757d"; // Bootstrap gray
                case AlertColor.Success:
                    return "green";
                case AlertColor.Danger:
                    return "red";
                default:
                    return "black";
            }
        }

        public class FormatConfig
        {
            public string? title { get; set; }
            public string? openingLine { get; set; }
            public string? tableContent { get; set; }
            public string? closingLine { get; set; }
            public AlertColor alertColor { get; set; }
        }
        public class EmailConfig
        {
            public string? MailTo { get; set; }
            public string? MailCc { get; set; }
            public string? MailSubject { get; set; }
            public string? MailBody { get; set; }

            public List<string> AttachmentFilePaths { get; set; } = new List<string>();
            public List<EmailAttachment> AttachmentBytes { get; set; } = new List<EmailAttachment>();
        }
        public class EmailAttachment
        {
            public byte[]? FileBytes { get; set; }
            public string? FileName { get; set; }
        }
    }
}
