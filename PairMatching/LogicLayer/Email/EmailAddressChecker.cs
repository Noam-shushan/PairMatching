using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public enum EmailAddressStatus { Valid, NotValid, Empty }

    public class EmailAddressChecker
    {
        private const string testFromAddress = "test@gmail.com";

        public static EmailAddressStatus CheckEmailAddress(string address)
        {
            if(address == string.Empty)
            {
                return EmailAddressStatus.Empty;
            }
            try
            {
               new MailMessage(testFromAddress, address);
            }
            catch (FormatException)
            {
                return EmailAddressStatus.NotValid;
            }
            return EmailAddressStatus.Valid;
        }

        public static void NotifyOnNotValidAddrress(List<BO.Student> students)
        {
            foreach(var s in students)
            {
                var emailStatus = CheckEmailAddress(s.Email);
                if(emailStatus != EmailAddressStatus.Valid)
                {
                    Notifications.Instance.NotValidEmailAdrress.Add(new NotValidEmailNotification
                    {
                        AddressStatus = emailStatus,
                        EmailAddress = s.Email,
                        StudentId = s.Id,
                        StudentName = s.Name
                    });
                }
            }
        }
    }
}
