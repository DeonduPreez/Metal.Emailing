using System;
using System.Threading.Tasks;
using Metal.Emailing.Models;

namespace Metal.Emailing.Interfaces
{
    public interface IEmailProvider
    {
        /// <summary>
        /// Send and wait for the email to finish sending
        /// </summary>
        /// <param name="emailRequest">Specifies all details of the email to be sent</param>
        /// <returns>Result of the email that was sent</returns>
        EmailSendResult SendEmail(EmailRequest emailRequest);

        /// <summary>
        /// Send the email asynchronously and waits for it to finish sending
        /// </summary>
        /// <param name="emailRequest">Specifies all details of the email to be sent</param>
        /// <returns>Result of the email that was sent</returns>
        Task<EmailSendResult> SendEmailAsync(EmailRequest emailRequest);

        /// <summary>
        /// Adds the email into a queue to be sent when the thread gets to it
        /// </summary>
        /// <param name="emailRequest">Specifies all details of the email to be sent</param>
        /// <param name="callback">Method that runs after the email has been sent</param>
        void SendEmail(EmailRequest emailRequest, Action<EmailSendResult> callback, int retryCount = 0);
    }

    public class EmailSendResult
    {
        public bool Success { get; internal set; }

        public DateTime TimeSent { get; internal set; }

        public string ErrorMessage { get; internal set; }

        public Exception Exception { get; internal set; }

        public EmailRequest EmailRequest { get; internal set; }
    }
}