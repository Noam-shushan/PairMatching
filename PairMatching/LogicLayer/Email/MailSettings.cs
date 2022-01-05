namespace LogicLayer.Email
{
    public class MailSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Password { get; set; }

        public string From { get; set; }

        public string UserName { get; set; }

        public bool EnableSsl { get; set; }
    }
}
