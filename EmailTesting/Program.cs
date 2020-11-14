using System;
using Metal.Emailing.Interfaces;
using Metal.Emailing.Models;
using Metal.Emailing.Providers;

namespace EmailTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                StartEmailing();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Fatal error: {e}");
            }
        }

        private static void StartEmailing()
        {
            var useEnvVar = GetYesNoInput("Should I use environment variables? (y/n): ") ?? default;

            string host;
            string port;
            string username;
            string password;

            if (useEnvVar)
            {
                host = Environment.GetEnvironmentVariable(nameof(host));
                port = Environment.GetEnvironmentVariable(nameof(port));
                username = Environment.GetEnvironmentVariable(nameof(username));
                password = Environment.GetEnvironmentVariable(nameof(password));
            }
            else
            {
                Console.Write("Please enter your host name: ");
                host = Console.ReadLine();
                var portIndex = host?.IndexOf(":", StringComparison.Ordinal);
                if (portIndex > -1)
                {
                    port = host.Substring(portIndex.Value + 1);
                }
                else
                {
                    Console.Write("Please enter your port: ");
                    port = Console.ReadLine();
                }

                Console.Write("Please enter your username: ");
                username = Console.ReadLine();
                password = MaskPassword();
            }

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (string.IsNullOrWhiteSpace(port) || !int.TryParse(port, out var actualPort))
            {
                throw new ArgumentNullException(nameof(port));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            var smtpProvider = new SmtpProvider(new MailSettings()
            {
                Host = host,
                Port = actualPort,
                Username = username,
                Password = password
            });

            RepeatUntilStopped(smtpProvider);
        }

        private static void RepeatUntilStopped(IEmailProvider smtpProvider)
        {
            var fromEmail = string.Empty;
            var displayName = string.Empty;
            var isBodyHtml = false;
            var subject = string.Empty;
            var body = string.Empty;

            bool stop = false;
            string[] recipients = null;

            while (!stop)
            {
                var tempFromEmail = GetUserInputInline("Please enter the from email: ");
                fromEmail = string.IsNullOrWhiteSpace(tempFromEmail) ? fromEmail : tempFromEmail;

                var tempDisplayName = GetUserInputInline("Please enter your display name, or leave blank: ");
                displayName = string.IsNullOrWhiteSpace(tempDisplayName) ? displayName : tempDisplayName;
                
                var isBodyHtmlInput = GetYesNoInput("Is the body of the email html? (y/n): ");
                isBodyHtml = isBodyHtmlInput == null ? isBodyHtml : isBodyHtmlInput.Value;

                var tempSubject = GetUserInputInline("Please enter the subject of the email: ");
                subject = string.IsNullOrWhiteSpace(tempSubject) ? subject : tempSubject;

                Console.WriteLine("Please enter the body of the email below: ");
                var tempBody = Console.ReadLine();
                body = string.IsNullOrWhiteSpace(tempBody) ? body : tempBody;

                Console.WriteLine("Please enter the recipient list separated by a comma (,) : ");
                var recipientListString = Console.ReadLine();
                recipients = string.IsNullOrWhiteSpace(recipientListString) ? recipients : recipientListString?.Split(",");

                var email = new EmailRequest()
                {
                    FromEmail = fromEmail,
                    Body = body,
                    Subject = subject,
                    Recipients = recipients,
                    DisplayName = displayName,
                    IsHtml = isBodyHtml
                };
                var result = smtpProvider.SendEmail(email);
                Console.WriteLine($"Success: {result.Success}");
                if (!result.Success)
                {
                    Console.WriteLine($"Error Message: {result.ErrorMessage}");
                }

                stop = !GetYesNoInput("Would you like to send another email? (y/n): ") ?? true;
            }
        }

        private static string GetUserInputInline(string question)
        {
            Console.Write(question);
            return Console.ReadLine();
        }

        private static bool? GetYesNoInput(string question)
        {
            Console.Write(question);

            var userInput = Console.ReadLine()?.ToLower();

            if (userInput != null && userInput.Contains("y"))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(userInput))
            {
                return null;
            }

            return false;
        }

        private static string MaskPassword()
        {
            Console.Write("Please enter your password: ");
            var pass = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    Console.Write("\b");
                }
            }
            // Stops Receiving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);

            return pass;
        }
    }
}