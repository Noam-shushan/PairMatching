using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Configuration;
using System.IO;
using MimeKit;
using MailKit.Net.Smtp;


namespace LogicLayer.Email
{
    /// <summary>
    /// Send email from the system.<br/>
    /// Can send html/css template and open email with attchment.
    /// </summary>
    public class SendEmail
    {
        private readonly MailSettings _mailSettings;

        public string SystemMail { get; private set; }

        private const int CHUNK_SIZE = 80;

        /// <summary>
        /// A Singleton referenc of the SendEmail class
        /// </summary>
        public static SendEmail Instance { get; } = new SendEmail();

        private SendEmail(bool isTest = false)
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

            SystemMail = mailSettings.Smtp.From;

            _mailSettings = !isTest ? new MailSettings
            {
                Host = mailSettings.Smtp.Network.Host,
                Password = mailSettings.Smtp.Network.Password,
                Port = mailSettings.Smtp.Network.Port,
                From = mailSettings.Smtp.From,
                UserName = mailSettings.Smtp.Network.UserName,
                EnableSsl = mailSettings.Smtp.Network.EnableSsl
            } :
            new MailSettings 
            {
                Host = "localhost",
                EnableSsl = false,
                From = mailSettings.Smtp.From,
                Port = 25,
                UserName = mailSettings.Smtp.Network.UserName
            };
        }


        /// <summary>
        /// The destination address of the email
        /// </summary>
        private List<string> _to = new List<string>();

        /// <summary>
        /// The email subject
        /// </summary>
        private string _subject = "";

        /// <summary>
        /// The template of the email body
        /// </summary>
        private StringBuilder _template = new StringBuilder();

        /// <summary>
        /// Set the destination address of the email
        /// </summary>
        /// <param name="to">email address</param>
        /// <returns>this email sender</returns>
        public SendEmail To(params string[] to)
        {
            var validAdrress = new List<string>();
            foreach (var addr in to)
            {
                if (EmailAddressChecker.CheckEmailAddress(addr) == EmailAddressStatus.Valid)
                {
                    validAdrress.Add(addr.Trim());
                }
            }
            _to = validAdrress;
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
        /// Send an open email with the option to add a file attachment
        /// </summary>
        /// <param name="fileAttachment">file name to attach to the email</param>
        /// <returns></returns>
        public async Task SendOpenMailAsync(IEnumerable<string> fileAttachments = null)
        {
            if (_to?.Count == 0)
            {
                return;
            }

            try
            {
                SmtpClient client = await GetSmtpClienet();

                IEnumerable<MailboxAddress> listOfAddress = GetAddresses();

                var message = GetMailMessage(fileAttachments);

                while (listOfAddress.Any())
                {
                    message.To.Clear();
                    var temp = listOfAddress.Take(CHUNK_SIZE);
                    message.To.AddRange(temp);
                    listOfAddress = listOfAddress.Skip(CHUNK_SIZE);
                    await client.SendAsync(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private MimeMessage GetMailMessage(IEnumerable<string> fileAttachments)
        {
            var message = new MimeMessage();

            var from = new MailboxAddress(_mailSettings.UserName, _mailSettings.From);
            message.From.Add(from);

            var bodyBuilder = new BodyBuilder
            {
                TextBody = _template.ToString(),
            };
            SetAttachments(bodyBuilder, fileAttachments);

            message.Body = bodyBuilder.ToMessageBody();
            message.Subject = _subject;
            return message;
        }


        private IEnumerable<MailboxAddress> GetAddresses()
        {
            var listOfAddress = new List<MailboxAddress>();
            foreach (var addr in _to)
            {
                try
                {
                    listOfAddress.Add(new MailboxAddress("User", addr));
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return listOfAddress;
        }

        private void SetAttachments(BodyBuilder bodyBuilder, IEnumerable<string> fileAttachments)
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
                    bodyBuilder.Attachments.Add(f);
                }
            }
        }

        /// <summary>
        /// Send email template
        /// </summary>
        /// <returns></returns>
        public async Task SendAutoEmailAsync()
        {
            if (_to?.Count == 0)
            {
                return;
            }
            try
            {
                SmtpClient client = await GetSmtpClienet();

                using (var message = new MimeMessage())
                {
                    var from = new MailboxAddress(_mailSettings.UserName, _mailSettings.From);
                    message.From.Add(from);

                    IEnumerable<MailboxAddress> listOfAddress = GetAddresses();

                    message.To.AddRange(listOfAddress);

                    var bodyBuilder = new BodyBuilder
                    {
                        HtmlBody = _template.ToString(),
                    };

                    message.Body = bodyBuilder.ToMessageBody();
                    message.Subject = _subject;

                    await client.SendAsync(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        SmtpClient _smtpClient;
        private async Task<SmtpClient> GetSmtpClienet(bool isTest = false)
        {
            if (_smtpClient != null)
            {
                return _smtpClient;
            }
            _smtpClient = new SmtpClient();

            await _smtpClient.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.EnableSsl);

            if (!isTest)
            {
                await _smtpClient.AuthenticateAsync(_mailSettings.From, _mailSettings.Password);
            }

            return _smtpClient;
        }

        /// <summary>
        /// Send email template
        /// </summary>
        /// <returns></returns>
        public async Task SendAutoEmailAsync<T>(T model, MailTemplate template)
        {
            string result = new CreateEmailTemplate()
                .Compile(model, template.Template.ToString());


            var temp = new StringBuilder()
                .Append(result);

            await Subject(template.Subject)
                .Template(temp)
                .SendAutoEmailAsync();
        }

        /// <summary>
        /// Send bug result to the devloper
        /// </summary>
        /// <param name="exception">The exception that catch</param>
        /// <returns></returns>
        internal async Task Error(Exception exception)
        {
            await To("noam8shu@gmail.com")
                .Subject("Bug in PairMatching")
                .Template(new StringBuilder()
                        .AppendLine(exception.Message)
                        .AppendLine(exception.StackTrace))
                .SendAutoEmailAsync();
        }
    }
}
