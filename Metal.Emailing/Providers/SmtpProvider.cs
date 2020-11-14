using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Metal.Emailing.Interfaces;
using Metal.Emailing.Models;

namespace Metal.Emailing.Providers
{
    public class SmtpProvider : IEmailProvider
    {
        private readonly SmtpClient _smtpClient;

        public SmtpProvider(MailSettings mailSettings, bool enableSSL = false)
        {
            _smtpClient = new SmtpClient(mailSettings.Host, mailSettings.Port);
            _smtpClient.Credentials = new NetworkCredential(mailSettings.Username, mailSettings.Password);
            _smtpClient.EnableSsl = enableSSL;
        }

        public EmailSendResult SendEmail(EmailRequest emailRequest)
        {
            var result = new EmailSendResult()
            {
                Success = true,
                EmailRequest = emailRequest
            };

            try
            {
                _smtpClient.Send(GetMailMessage(emailRequest));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.Success = false;
                result.ErrorMessage = e.Message;
                result.Exception = e;
            }
            finally
            {
                result.TimeSent = DateTime.UtcNow;
            }

            return result;
        }

        public async Task<EmailSendResult> SendEmailAsync(EmailRequest emailRequest)
        {
            var result = new EmailSendResult()
            {
                Success = true,
                EmailRequest = emailRequest
            };

            try
            {
                await _smtpClient.SendMailAsync(GetMailMessage(emailRequest));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.Success = false;
                result.ErrorMessage = e.Message;
                result.Exception = e;
            }
            finally
            {
                result.TimeSent = DateTime.UtcNow;
            }

            return result;
        }

        public void SendEmail(EmailRequest emailRequest, Action<EmailSendResult> callback, int retryCount = 0)
        {
            // TODO : This should run on a queue
            throw new NotImplementedException();
        }

        private MailMessage GetMailMessage(EmailRequest emailRequest)
        {
            var mail = new MailMessage()
            {
                From = new MailAddress(emailRequest.FromEmail, emailRequest.DisplayName),
                IsBodyHtml = emailRequest.IsHtml,
                Subject = emailRequest.Subject,
                Body = emailRequest.Body
            };

            if (emailRequest.ReplyEmail == null)
            {
                emailRequest.ReplyEmail = emailRequest.FromEmail;
            }

            mail.ReplyToList.Add(emailRequest.ReplyEmail);

            foreach (var recipient in emailRequest.Recipients)
            {
                mail.To.Add(recipient);
            }

            foreach (var ccRecipient in emailRequest.CCRecipients)
            {
                mail.CC.Add(ccRecipient);
            }

            foreach (var bccRecipient in emailRequest.BCCRecipients)
            {
                mail.Bcc.Add(bccRecipient);
            }

            if (emailRequest.Attachments?.Count > 0)
            {
                foreach (var attachment in emailRequest.Attachments)
                {
                    mail.Attachments.Add(attachment);
                }
            }

            return mail;
        }
    }
}