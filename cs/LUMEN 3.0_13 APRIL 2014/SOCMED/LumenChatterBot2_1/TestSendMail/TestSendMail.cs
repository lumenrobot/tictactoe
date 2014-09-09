using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace TestSendMail
{
    public class TestSendMail
    {
        private static string usr = "lumensocialrobot@gmail.com";
        private static string pwd = "srrg-lskk";

        private static string smtpHostname = "smtp.gmail.com";
        private static string mailSubject = "Test";
        private static string mailBody = "Testing";
        private static MailAddress mailFrom = new MailAddress("lumensocialrobot@gmail.com", "Lumen");
        private static MailAddress mailToFb = new MailAddress("doubt376yeah@m.facebook.com", "Lumen Fb");

        /// <summary>
        /// Produce some test emails.
        /// </summary>
        public void SendMail()
        {
            var mailTo = mailToFb;
            var subject = mailSubject;
            var body = mailBody;
            Console.WriteLine("======== Sending Message ========== ");

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = smtpHostname;
            client.EnableSsl = true;
            client.Timeout = 60000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(usr, pwd);

            //pure text email
            MailMessage message;

            //just body text email
            message = new MailMessage(mailFrom, mailTo);
            message.Subject = subject;
            message.Body = body;

            string attPath = Environment.CurrentDirectory + @"\test.jpg";
            Attachment att = new Attachment(attPath);
            Console.WriteLine(attPath);
            message.Attachments.Add(att);

            SendMessageToServer(client, message);

            Console.WriteLine("========== Sending completed ===========");
            Console.WriteLine();
        }

        static void SendMessageToServer(SmtpClient client, MailMessage message)
        {
            client.Send(message);
            Console.WriteLine("=============== Report Sending Message ================");
            Console.WriteLine(">Subject : " + message.Subject);
            Console.WriteLine(">From : " + message.From);
            Console.WriteLine(">To : " + message.To);
            Console.WriteLine();
            // Clean up.
            message.Dispose();
        }
    }
}
