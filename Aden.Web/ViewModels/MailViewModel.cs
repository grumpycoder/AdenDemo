using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Aden.Web.ViewModels
{
    public class MailViewModel
    {
        public string Id { get; set; }
        public IEnumerable<string> CC;
        public IEnumerable<string> To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public DateTime Sent { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}