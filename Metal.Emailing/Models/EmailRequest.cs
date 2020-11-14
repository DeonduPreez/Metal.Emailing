using System.Collections.Generic;
using System.Net.Mail;

namespace Metal.Emailing.Models
{
    public class EmailRequest
    {
        public EmailRequest()
        {
            Recipients = new List<string>();
            CCRecipients = new List<string>();
            BCCRecipients = new List<string>();
        }
        
        public string FromEmail { get; set; }
        
        public string ReplyEmail { get; set; }
        
        public string DisplayName { get; set; }
        
        public IEnumerable<string> Recipients { get; set; }
        
        public IEnumerable<string> CCRecipients { get; set; }
        
        public IEnumerable<string> BCCRecipients { get; set; }
        
        public string Subject { get; set; }
        
        public string Body { get; set; }
        
        public bool IsHtml { get; set; }
        
        public AttachmentCollection Attachments { get; set; }
    }
}