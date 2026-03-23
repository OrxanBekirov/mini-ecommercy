using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Helpers.Settings
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
        public string ReceiverEmail { get; set; }
        public bool EnableSsl { get; set; }
    }
}
