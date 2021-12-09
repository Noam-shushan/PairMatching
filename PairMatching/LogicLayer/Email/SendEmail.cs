using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;
using System.Net.Configuration;
using System.IO;


namespace LogicLayer.Email
{
    /// <summary>
    /// Send email from the system.<br/>
    /// Can send html/css template and open email with attchment.
    /// </summary>
    public class SendEmail
    {
        /// <summary>
        /// The main mail addres of the system
        /// </summary>
        public readonly MailAddress FromMail;

        /// <summary>
        /// The destination address of the email
        /// </summary>
        private string _to = "";

        /// <summary>
        /// The email subject
        /// </summary>
        private string _subject = "";

        /// <summary>
        /// The template of the email body
        /// </summary>
        private StringBuilder _template = new StringBuilder();

        /// <summary>
        /// create template from html file
        /// </summary>
        private readonly CreateEmailTemplate emailTemplateCreator;

        /// <summary>
        /// smtp object that send the email.
        /// </summary>
        private readonly SmtpSection smtp;

        private const int CHUNK_SIZE = 20;

        /// <summary>
        /// A Singleton referenc of the SendEmail class
        /// </summary>
        public static SendEmail Instance { get; } = new SendEmail();

        private SendEmail()
        {
            // Get the the config of the app 
            Configuration oConfig = ConfigurationManager
                .OpenExeConfiguration(ConfigurationUserLevel.None);
            
            // Get the mail settings of the app from the config file
            var mailSettings = oConfig
                .GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            if (mailSettings == null)
            {
                return;
            }
            // Get the smtp from the config file
            smtp = mailSettings.Smtp;

            // Get the system email address
            FromMail = new MailAddress(mailSettings.Smtp.From, 
                mailSettings.Smtp.Network.UserName);

            emailTemplateCreator = new CreateEmailTemplate();
        }


        List<string> tempTo;
        /// <summary>
        /// Set the destination address of the email
        /// </summary>
        /// <param name="to">email addres</param>
        /// <returns>this email sender</returns>
        public SendEmail To(string to)
        {
            _to = to;
            var validAdrress = new List<string>();
            var adrreses = to.Split(',');
            foreach (var addr in adrreses)
            {
                var emailStatus = EmailAddressChecker.CheckEmailAddress(addr);
                if(emailStatus == EmailAddressStatus.Valid)
                {
                    validAdrress.Add($"<{addr}>");
                }
            }
            tempTo = new List<string>(validAdrress);
            _to = string.Join(", ", validAdrress)
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("GMAIL", "gmail");
            return this;
        }

        /// <summary>
        /// Set the email subject
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <returns>this sender</returns>
        public SendEmail Subject(string subject)
        {
            _subject = subject;
            return this;
        }

        /// <summary>
        /// Set the email template body
        /// </summary>
        /// <param name="template">The email template body</param>
        /// <returns>This sender</returns>
        public SendEmail Template(StringBuilder template)
        {
            _template = template;
            return this;
        }

        /// <summary>
        /// Get the smtp client that send the email
        /// </summary>
        /// <param name="isTest">For testing purposes</param>
        /// <returns>The smtp client</returns>
        private SmtpClient GetSmtpClient(bool isTest = false)
        {
            return isTest ?
                // Sender for testing the system
                new SmtpClient 
                { 
                    Host = "localhost", 
                    Port = 25, 
                    EnableSsl = false, 
                    DeliveryFormat = SmtpDeliveryFormat.International 
                } :
                // Sender for the app
                new SmtpClient
                {
                    Host = smtp.Network.Host,
                    Port = smtp.Network.Port,
                    UseDefaultCredentials = smtp.Network.DefaultCredentials,
                    Credentials = new NetworkCredential(smtp.From, smtp.Network.Password),
                    EnableSsl = smtp.Network.EnableSsl,
                    DeliveryMethod = smtp.DeliveryMethod,
                    DeliveryFormat = smtp.DeliveryFormat
                };
        }

        /// <summary>
        /// Send an open email with the option to add a file attachment
        /// </summary>
        /// <param name="fileAttachment">file name to attach to the email</param>
        /// <returns></returns>
        public async Task SendOpenMailAsync(IEnumerable<string> fileAttachments = null)
        {
            if (_to == string.Empty)
            {
                //throw new Exception("Missing destination address to send email");
                return;
            }

            SmtpClient client = GetSmtpClient();
            while (tempTo.Any())
            {
                var addressChunk = GetChunkAddress(tempTo, CHUNK_SIZE);
                await SendOpenEmailToOne(fileAttachments, client, addressChunk);
                tempTo = tempTo.Skip(CHUNK_SIZE).ToList();
            }
        }

        private string GetChunkAddress(List<string> address, int chunkSize)
        {
            return string.Join(", ", address.Take(chunkSize))
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("GMAIL", "gmail");
        }

        private async Task SendOpenEmailToOne(IEnumerable<string> fileAttachments, SmtpClient client, string to)
        {
            try
            {
                using (var messege = new MailMessage(FromMail.Address,
                    to, _subject, _template.ToString()))
                {
                    GetAttachments(messege, fileAttachments);
                    await client.SendMailAsync(messege);
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is SmtpException)
            {
                throw new FormatException($"The Email address {to} is not valid");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void GetAttachments(MailMessage message, IEnumerable<string> fileAttachments)
        {
            if (fileAttachments != null)
            {
                foreach (var f in fileAttachments)
                {
                    if (!File.Exists(f))
                    {
                        throw new FileNotFoundException($"{f} File not found");
                    }
                }
                foreach (var f in fileAttachments)
                {
                    message.Attachments.Add(new Attachment(f));
                }
            }
        }

        /// <summary>
        /// Send email template
        /// </summary>
        /// <returns></returns>
        public async Task SendAsync()
        {
            if (_to == string.Empty)
            {
                //throw new Exception("Missing destination address to send email");
                return;
            }

            SmtpClient client = GetSmtpClient();
            while (tempTo.Any())
            {
                var addressChunk = GetChunkAddress(tempTo, CHUNK_SIZE);
                await SendAutoEmailToOne(client, addressChunk);
                tempTo = tempTo.Skip(CHUNK_SIZE).ToList();
            }
        }

        private async Task SendAutoEmailToOne(SmtpClient client, string to)
        {
            try
            {
                using (var messege = new MailMessage(FromMail.Address,
                    to, _subject, _template.ToString()))
                {
                    messege.IsBodyHtml = true;
                    await client.SendMailAsync(messege);
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is SmtpException)
            {
                throw new FormatException($"The Email address {to} is not valid");
            }
            catch (Exception ex1)
            {
                throw new Exception(ex1.Message);
            }
        }

        /// <summary>
        /// Send email template
        /// </summary>
        /// <returns></returns>
        public async Task SendAsync<T>(T model, MailTemplate template)
        {
            string result = emailTemplateCreator
                .Compile(model, template.Template.ToString());


            var temp = new StringBuilder()
                .Append(result);

            await Subject(template.Subject)
                .Template(temp)
                .SendAsync();
        }

        /// <summary>
        /// Send bug result to the devloper
        /// </summary>
        /// <param name="exception">The exception that catch</param>
        /// <returns></returns>
        public async Task Error(Exception exception)
        {
            await To("noam8shu@gmail.com")
                .Subject("Bug in PairMatching")
                .Template(new StringBuilder()
                        .AppendLine(exception.Message)
                        .AppendLine(exception.StackTrace))
                .SendAsync();
        }
    }
}
