using FluentEmail.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;
using FluentEmail.Core;
using Newtonsoft.Json;
using System.Net;
using FluentEmail.Razor;
using System.Configuration;
using System.Net.Configuration;
using FluentEmail.Core.Models;

namespace LogicLayer
{
    public class SendEmail
    {
        private readonly MailAddress fromMail;
        private string _to = "";
        private string _subject = "";
        private StringBuilder _template = new StringBuilder();
        private readonly SmtpSection smtp;


        public SendEmail()
        {
            Configuration oConfig = ConfigurationManager
                .OpenExeConfiguration(ConfigurationUserLevel.None);
            var mailSettings = oConfig
                .GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            if (mailSettings == null)
            {
                return;
            }
            smtp = mailSettings.Smtp;

            fromMail = new MailAddress(mailSettings.Smtp.From, 
                mailSettings.Smtp.Network.UserName);
        }

        public async Task SendAsync<T>(T model)
        {
            if (_to == string.Empty)
            {
                throw new Exception("Missing destination address to send email");
            }

            SmtpClient client = new SmtpClient
            {
                Host = smtp.Network.Host,
                Port = smtp.Network.Port,
                UseDefaultCredentials = smtp.Network.DefaultCredentials,
                Credentials = new NetworkCredential(smtp.From,
                    smtp.Network.Password),
                EnableSsl = smtp.Network.EnableSsl,
                DeliveryMethod = smtp.DeliveryMethod
            };

            var clientToTest = new SmtpClient
            {
                Host = "localhost",
                Port = 25,
                EnableSsl = false,
            };

            var sender = new SmtpSender(() => clientToTest);
            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();

            SendResponse email = new SendResponse();
            try
            {
                email = await Email
                    .From(fromMail.Address, fromMail.DisplayName)
                    .To(_to)
                    .Subject(_subject)
                    .UsingTemplate(_template.ToString(), model)
                    .SendAsync();
            }
            catch (Exception ex)
            {
                if (!email.Successful)
                {
                    throw new Exception(string.Join("\n", email.ErrorMessages));
                }
                throw ex;
            }
        }

        public async Task SendAsync<T>(T model, MailTemplate template)
        {
            await Subject(template.Subject)
                .Template(template.Template)
                .SendAsync(model);
        }

        public async Task Error(Exception exception)
        {
            await To("noam8shu@gmail.com")
                .Subject("Bug in PairMatching")
                .Template(new StringBuilder()
                    .AppendLine("@Model.Message")
                    .AppendLine("@Model.StackTrace"))
                .SendAsync(exception);
        }

        public SendEmail To(string to)
        {
            _to = to;
            return this;
        }

        public SendEmail Subject(string subject)
        {
            _subject = subject;
            return this;
        }

        public SendEmail Template(StringBuilder template)
        {
            _template = template;
            return this;
        }
    }
}
