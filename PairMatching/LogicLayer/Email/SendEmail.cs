using FluentEmail.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;
using FluentEmail.Core;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using FluentEmail.Razor;
using System.Configuration;
using System.Net.Configuration;

namespace LogicLayer
{
    public class SendEmail
    {
        private readonly MailAddress fromMail;
        private string _to = "";
        private string _subject = "";
        private StringBuilder _template = new StringBuilder();
        private readonly SmtpClient client;


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

            client = new SmtpClient
            {
                Host = mailSettings.Smtp.Network.Host,
                Port = mailSettings.Smtp.Network.Port,
                UseDefaultCredentials = mailSettings.Smtp.Network.DefaultCredentials,
                Credentials = new NetworkCredential(mailSettings.Smtp.From,
                    mailSettings.Smtp.Network.Password),
                EnableSsl = mailSettings.Smtp.Network.EnableSsl,
                DeliveryMethod = mailSettings.Smtp.DeliveryMethod
            };
            fromMail = new MailAddress(mailSettings.Smtp.From, 
                mailSettings.Smtp.Network.UserName);
        }

        public async Task SendAsync<T>(T model)
        {
            if (_to == string.Empty)
            {
                throw new Exception("Missing destination address to send email");
            }

            var sender = new SmtpSender(() => client);
            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();

            try
            {
                var email = await Email
                    .From(fromMail.Address, fromMail.DisplayName)
                    .To(_to)
                    .Subject(_subject)
                    .UsingTemplate(_template.ToString(), model)
                    .SendAsync();
            }
            catch (Exception)
            {
                throw;
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

    public class MailTemplate
    {
        public string Subject { get; set; }

        public StringBuilder Template { get; set; }
    }

    public enum EmailTypes { SuccessfullyRegistered }

    public static class Templates
    {
        public static MailTemplate SuccessfullyRegisteredHebrew
        {
            get => new MailTemplate
            {
                Subject = "נרשמת בהצלחה לתכנית שלהבת!",
                Template = new StringBuilder()
                    .Append("<p>@Model.Name</p>")
                    .Append("<p>היקר\\ה, השאלון שמילאת נקלט אצלנו, והנך חלק משלהבת! תודה רבה על שותפותך והאמון שלך בנו! תהליך התאמת החברותא לוקח זמן מה, אך בקרוב נודיע לך מי החברותא שהותאמה לך. </ p>")
                    .Append("<br>")
                    .Append("<p>לאחר ההתאמה, יישלח לך מייל ובו פרטי ההתקשרות של החברותא, הסבר על אופן יצירת הקשר. עוד נשלח הסבר על אופי המסלול שבחרתם, כולל מגוון הכלים שהוא מציע לתמיכה בלימוד שלכם.</ p>")
                    .Append("<br><br>")
                    .Append("<p>כל טוב,<br> צוות שלהבת</p>")
            };
        }
    }
}
