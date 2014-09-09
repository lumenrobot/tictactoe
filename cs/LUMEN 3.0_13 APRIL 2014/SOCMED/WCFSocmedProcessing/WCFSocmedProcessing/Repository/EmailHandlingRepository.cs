using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Timers;
using System.Web;
using OpenPop.Mime;
using OpenPop.Pop3;
using System.Net;

namespace WCFSocmedProcessing.Repository
{
    public class EmailHandlingRepository
    {
        private static string _popHostname = "pop.gmail.com";
        private static int _port = 995;
        private static bool _useSsl = true;
        private static string _usr = "lumensocialrobot@gmail.com";
        private static string _pwd = "srrg-lskk";

        private static string _smtpHostname = "smtp.gmail.com";
        private static string _mailSubject = "";
        private static string _mailBody = "";
        private static MailAddress _mailFrom = new MailAddress("lumensocialrobot@gmail.com", "Lumen");
        private static MailAddress _mailToFb = new MailAddress("doubt376yeah@m.facebook.com", "Lumen Fb");

        private static int _timerUnit = 60000;
        private static int _count = 0;
        private static Timer _timerSet = new Timer(2000);


        /// <summary>
        ///  FetchAllMessages:
        ///  - fetch all messages from a POP3 server
        /// </summary>
        /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
        /// <param name="_port">Host _port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
        /// <param name="_useSsl">Whether or not to use SSL to connect to server</param>
        /// <param name="username">Username of the user on the server</param>
        /// <param name="password">Password of the user on the server</param>
        /// <returns>All Messages on the POP3 server</returns>
        static List<Message> FetchAllMessages(string hostname, int port, bool useSsl, string username, string password)
        {
            // The client disconnects from the server when being disposed
            using (Pop3Client client = new Pop3Client())
            {
                // Connect to the server
                client.Connect(hostname, port, useSsl);

                // Authenticate ourselves towards the server
                client.Authenticate(username, password);

                // Get the number of messages in the inbox
                int messageCount = client.GetMessageCount();

                // We want to download all messages
                List<Message> allMessages = new List<Message>(messageCount);

                // Messages are numbered in the interval: [1, messageCount]
                // Ergo: message numbers are 1-based.
                // Most servers give the latest message the highest number
                for (int i = messageCount; i > 0; i--)
                {
                    allMessages.Add(client.GetMessage(i));
                }

                // Now return the fetched messages
                return allMessages;
            }
        }

        /// <summary>
        /// GetNewInbox :
        /// -get all new inbox from pop3 server
        /// -save to Lumen db
        /// </summary>
        /// <remarks>
        /// 	<param name="info">info about total email,or error</param>
        /// 	<param name="allMessage">all email inbox</param>
        /// </remarks>
        /// <returns>
        /// -Info error/total
        /// -All Messages on the POP3 server
        /// </returns>
        public static bool GetNewInbox(out string info,out List<Message> allMessage )
        {
            var isSuccess = false;
            info = "";
            allMessage = new List<Message>();

            try
            {
                allMessage = FetchAllMessages(_popHostname, _port, _useSsl, _usr, _pwd);
                isSuccess = true;
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// Send Mail text only
        /// </summary>
        /// <remarks>
        /// 	<param name="info">info : subject or error</param>
        /// </remarks>
        /// <returns>
        /// -bool success
        /// -string info
        /// </returns>
        public static bool SendMail(out string info, string subject, string body)
        {
            var mailFrom = _mailFrom;
            var mailTo = _mailFrom;

            var isSuccess = false;
            info = "";
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = _smtpHostname;
                client.EnableSsl = true;
                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_usr, _pwd);

                //pure text email
                MailMessage message;

                //just body text email
                message = new MailMessage(mailFrom, mailTo);
                message.Subject = subject;
                message.Body = body;

                client.Send(message);
        
                // Clean up.
                message.Dispose();

                isSuccess = true;
            }
            catch (Exception ex)
            {
                info = ex.Message;
                isSuccess=false;
            }
            return isSuccess;
        }
    }
}