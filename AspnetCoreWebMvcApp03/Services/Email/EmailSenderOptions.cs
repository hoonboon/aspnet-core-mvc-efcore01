using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreWebMvcApp03.Services.Email
{
    public class EmailSenderOptions
    {
        public const string EmailSender = "EmailSender";

        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SendGridApiKey { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }

    }
}
