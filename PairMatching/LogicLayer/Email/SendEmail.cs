//using FluentEmail.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Configuration;
using System.Net.Configuration;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

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

        public async Task SendOpenMailAsync(string fileAttachment = "")
        {
            if (_to == string.Empty)
            {
                throw new Exception("Missing destination address to send email");
            }
            
            var clientToTest = new SmtpClient
            {
                Host = "localhost",
                Port = 25,
                EnableSsl = false,
            };

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
            try
            {
                using (var messege = new MailMessage(fromMail.Address, 
                    _to, _subject, _template.ToString()))
                {
                    if (fileAttachment != "")
                    {
                        if (!File.Exists(fileAttachment))
                        {
                            throw new Exception("File not found");
                        }
                        Attachment attachment = new Attachment(fileAttachment);
                        messege.Attachments.Add(attachment);
                    }
                    await Task.Run(() => clientToTest.Send(messege));
                }
            }
            catch (FormatException)
            {
                throw new Exception("Email address not valid");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            
            try
            {
                using (var messege = new MailMessage(fromMail.Address, _to, _subject, _template.ToString()))
                {
                    messege.IsBodyHtml = true;
                    await Task.Run(() => clientToTest.Send(messege));
                }
            }
            catch (FormatException)
            {
                throw new Exception("Email address not valid");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendAsync<T>(T model, MailTemplate template)
        {
            var result = Engine.Razor
                            .RunCompile(template.Template.ToString(), 
                            template.Subject, null, model);
            var temp = new StringBuilder()
                .Append(result);
            await Subject(template.Subject)
                .Template(temp)
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
