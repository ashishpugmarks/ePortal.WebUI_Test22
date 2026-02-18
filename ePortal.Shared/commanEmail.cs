using System.Net.Mail;
using System.Net.Mime;
namespace ePortal.Shared
{
    /// <summary>
    /// Sending Email from help page.
    /// </summary>
    public class commanEmail
    {
        private string strMailFrom;
        private string strMailTo;
        private string strMailSubject;
        private string strMailBody;
        private string strAttachment;
        private string strMailCc;
        private MailMessage MailObject;
        private List<string> strAttachments;
        /// <summary>
        /// Properties for mail configuration
        /// </summary>
        public string MailFrom
        {
            set { strMailFrom = value; }
            get { return strMailFrom; }
        }
        public string MailTo
        {
            set { strMailTo = value; }
            get { return strMailTo; }
        }
        public string MailSubject
        {
            set { strMailSubject = value; }
            get { return strMailSubject; }
        }
        public string MailBody
        {
            set { strMailBody = value; }
            get { return strMailBody; }
        }
        public string MailCc
        {
            set { strMailCc = value; }
            get { return strMailCc; }
        }
        public List<string> AttachmentFilePath
        {
            set { strAttachments = value; }
            get { return strAttachments; }
        }

        /// <summary>
        /// Method to send email
        /// return boolan value is email sent or Not.
        /// </summary>
        /// <returns></returns>
        public bool Send()
        {
            bool bTemp = true;
            try
            {
                MailMessage Email = new MailMessage();
                MailAddress MailFrom = new MailAddress(strMailFrom, strMailFrom);
                Email.From = MailFrom;
                //Email.To.Add(strMailTo);

                foreach (var address in strMailTo.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Email.To.Add(address);
                }

                if (!String.IsNullOrEmpty(strMailCc))
                {
                    Email.CC.Add(strMailCc);
                }

                Email.IsBodyHtml = true;

                Email.Subject = strMailSubject;
                Email.Body = strMailBody;

                if (strAttachments != null)
                {
                    foreach (var item in strAttachments)
                    {
                        if (File.Exists(item))
                        {
                            // Create  the file attachment for this e-mail message.
                            Attachment data = new Attachment(item, MediaTypeNames.Application.Octet);

                            // Add time stamp information for the file.
                            ContentDisposition disposition = data.ContentDisposition;
                            disposition.CreationDate = File.GetCreationTime(item);
                            disposition.ModificationDate = System.IO.File.GetLastWriteTime(item);
                            disposition.ReadDate = System.IO.File.GetLastAccessTime(item);

                            // Add the file attachment to this e-mail message.
                            Email.Attachments.Add(data);
                        }
                    }
                }

                SmtpClient SmtpMail = new SmtpClient();
                SmtpMail.Host = "10.116.16.147";
                SmtpMail.Port = 25;
                SmtpMail.Send(Email); 
                return bTemp;
            }
            catch (InvalidOperationException ex)
            {
                bTemp = false;
                return bTemp;
            }
            catch (SmtpFailedRecipientException ex)
            {
                bTemp = false;
                return bTemp;
            }
            catch (SmtpException ex)
            {
                bTemp = false;
                return bTemp;
            }
        }
        /// <summary>
        /// Constructor to initialize email properties.
        /// </summary>
        public commanEmail()
        {
            MailObject = new MailMessage();
            strMailFrom = "";
            strMailTo = "";
            strMailSubject = "";
            strMailBody = "";
            strMailCc = "";
            strAttachments = new List<string>();
        }
    }
}








