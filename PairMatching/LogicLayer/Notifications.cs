using LogicLayer.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class NotValidEmailNotification
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string EmailAddress { get; set; }

        public EmailAddressStatus AddressStatus { get; set; }
    }

    public class Notifications
    {
        public List<NotValidEmailNotification> NotValidEmailAdrress { get; set; }

        public static Notifications Instance { get; set; } = new Notifications();

        private Notifications()
        {
            NotValidEmailAdrress = new List<NotValidEmailNotification>();
        }
    }
}
