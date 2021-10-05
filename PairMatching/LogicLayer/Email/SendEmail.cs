//using FluentEmail.Smtp;
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
using RazorEngine;
using RazorEngine.Templating;

namespace LogicLayer.Eamil
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
        /// smtp object that send the email.
        /// </summary>
        private readonly SmtpSection smtp;

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
        }

        /// <summary>
        /// Set the destination address of the email
        /// </summary>
        /// <param name="to">email addres</param>
        /// <returns>this email sender</returns>
        public SendEmail To(string to)
        {
            _to = to;
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
                new SmtpClient { Host = "localhost", Port = 25, EnableSsl = false } :
                // Sender for the app
                new SmtpClient
                {
                    Host = smtp.Network.Host,
                    Port = smtp.Network.Port,
                    UseDefaultCredentials = smtp.Network.DefaultCredentials,
                    Credentials = new NetworkCredential(smtp.From, smtp.Network.Password),
                    EnableSsl = smtp.Network.EnableSsl,
                    DeliveryMethod = smtp.DeliveryMethod
                };
        }

        /// <summary>
        /// Send an open email with the option to add a file attachment
        /// </summary>
        /// <param name="fileAttachment">file name to attach to the email</param>
        /// <returns></returns>
        public async Task SendOpenMailAsync(string fileAttachment = "")
        {
            if (_to == string.Empty)
            {
                throw new Exception("Missing destination address to send email");
            }

            SmtpClient client = GetSmtpClient();

            try
            {
                using (var messege = new MailMessage(FromMail.Address, 
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
                    await Task.Run(() => client.Send(messege));
                }
            }
            catch (FormatException) // Not valid email eddres
            {
                throw new FormatException($"The Email address {_to} is not valid");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                throw new Exception("Missing destination address to send email");
            }

            SmtpClient client = GetSmtpClient();

            try
            {
                using (var messege = new MailMessage(FromMail.Address,
                    _to, _subject, _template.ToString()))
                {
                    messege.IsBodyHtml = true;
                    await Task.Run(() => client.Send(messege));
                }
            }
            catch (FormatException)
            {
                throw new FormatException($"The Email address {_to} is not valid");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Send email template
        /// </summary>
        /// <returns></returns>
        public async Task SendAsync<T>(T model, MailTemplate template)
        {
            string result;
            try
            {   // Get the template string using Razor Engine
                result = Engine.Razor
                        .RunCompile(template.Template.ToString(),
                        template.Subject, null, model);

            }
            catch (TemplateCompilationException)
            {
                throw new Exception("בעיה בשליחת המייל רצוי לשלוח מייל פתוח");
            }
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
                .SendAsync(exception, new MailTemplate
                {
                    Subject = "Bug in PairMatching", 
                    Template = new StringBuilder()
                        .AppendLine("<p>@Model.Message</p>")
                        .AppendLine("<p>@Model.StackTrace</p>")
                });
        }
    }
}
