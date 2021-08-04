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

namespace LogicLayer
{
    public class SendEmail
    {
        private MailAccount mail;
        private string _to;
        private string _subject;
        private StringBuilder _template = new StringBuilder();


        public SendEmail()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emailAddress.json");
            mail = LoadObjFromJsonFile<MailAccount>(path);
        }

        public async Task Send<T>(T model)
        {
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail.Address, mail.Password)
            };
            var sender = new SmtpSender(() => smtp);
            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();

            var email = await Email
                .From(mail.Address, mail.Name)
                .To(_to)
                .Subject(_subject)
                .UsingTemplate(_template.ToString(), model)
                .SendAsync();
        }

        public async Task Error(Exception exception)
        {
            await To("noam8shu@gmail.com")
                .Subject("Bug in PairMatching")
                .Template(new StringBuilder()
                    .AppendLine("@Model.Message")
                    .AppendLine("@Model.StackTrace"))
                .Send(exception);
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

        T LoadObjFromJsonFile<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var jsonString = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"can not load the file {filePath}" + ex.Message);
            }
        }
    }

    public class MailAccount
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
