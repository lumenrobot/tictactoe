using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using OpenPop.Mime;
using OpenPop.Pop3;
using System.Net;
using WCFSocmedProcessing.Model;

namespace WCFSocmedProcessing.Repository
{
    public class EmailHandlingRepository
    {

        private const string className = "EmailHandlingRepository";
        private const string ok = "OK";
        private const string err = "ERROR";

        private static string _popHostName = ConfigurationManager.AppSettings["Pop3HostNameGmail"];
        private static int _portPop3 = int.Parse(ConfigurationManager.AppSettings["PortPop3Gmail"]);
        private static int _portSmtp = int.Parse(ConfigurationManager.AppSettings["PortSmtpGmail"]);
        private static bool _isTesting = bool.Parse(ConfigurationManager.AppSettings["IsTesting"]);
        private static bool _useSsl = bool.Parse(ConfigurationManager.AppSettings["IsUseSslGmail"]);
        private static bool _useSslSmtp = bool.Parse(ConfigurationManager.AppSettings["IsUseSslGmailSmtp"]);
        private static string _usr = ConfigurationManager.AppSettings["UserGmail"];
        private static string _pwd = ConfigurationManager.AppSettings["PwdGmail"];
        private static int _timeout = int.Parse(ConfigurationManager.AppSettings["Timeout"]);
        private static string _smtpHostName = ConfigurationManager.AppSettings["SmtpHostNameGmail"];
        private static string _mailSubject = "";
        private static string _mailBody = "";
        private static MailAddress _mailFrom = new MailAddress(ConfigurationManager.AppSettings["MailAddressGmail"], ConfigurationManager.AppSettings["MailOwnerNameGmail"]);
        private static MailAddress _mailToFb = new MailAddress(ConfigurationManager.AppSettings["MailAddressFacebook"], ConfigurationManager.AppSettings["MailOwnerNameFacebook"]);

        /// <summary>
        ///  FetchAllMessages:
        ///  - fetch all messages from a POP3 server
        /// </summary>
        /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
        /// <param name="port">Host _port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
        /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
        /// <param name="username">Username of the user on the server</param>
        /// <param name="password">Password of the user on the server</param>
        /// <returns>All Messages on the POP3 server</returns>
        static List<Message> FetchAllMessages(string hostname, int port, bool useSsl, string username, string password)
        {
            try
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
            catch (Exception ex)
            {
                var nullMessageList = new List<Message>();
                return nullMessageList;
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
        public static bool GetNewInbox(out string info, out List<Message> allMessage)
        {
            var isSuccess = false;
            info = className + ".GetNewInbox(out string info, out List<Message> allMessage) - ";
            allMessage = new List<Message>();

            try
            {
                allMessage = FetchAllMessages(_popHostName, _portPop3, _useSsl, _usr, _pwd);
                isSuccess = true;
                info += ok;

            }
            catch (Exception ex)
            {
                isSuccess = false;
                info += err;
                info += " : " + ex.Message;
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

            if (!_isTesting)
            {
                mailTo = _mailToFb;
            }

            var isSuccess = false;
            info = "";
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = _portSmtp;
                client.Host = _smtpHostName;
                client.EnableSsl = _useSslSmtp;
                client.Timeout = _timeout;
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
                isSuccess = false;
            }
            return isSuccess;
        }


        /// <summary>
        /// Send Mail text only
        /// <param name="info">message info.</param>
        /// <param name="draft">draft sent mail.</param>
        /// </summary>
        /// <returns>
        /// -bool success
        /// -string info
        /// </returns>
        public static bool SendMail(out string info,View_Sent_Facebook draft)
        {
            var mailFrom = _mailFrom;
            var mailTo = _mailFrom;

            if(!_isTesting)
            {
                mailTo = new MailAddress(draft.ToAddress,draft.ToName);
            }

            var isSuccess = false;
            info = "";
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = _portSmtp;
                client.Host = _smtpHostName;
                client.EnableSsl = _useSslSmtp;
                client.Timeout = _timeout;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_usr, _pwd);

                //pure text email
                MailMessage message;

                //just body text email
                message = new MailMessage(mailFrom, mailTo);
                message.Subject = draft.Subject;
                message.Body = draft.BodyTextPlain;

                client.Send(message);

                // Clean up.
                message.Dispose();

                isSuccess = true;
            }
            catch (Exception ex)
            {
                info = ex.Message;
                isSuccess = false;
            }
            return isSuccess;
        }

        
        /// <summary>
        /// Sent status update to facebook account of Lumen.
        /// </summary>
        public static bool UpdateFacebookStatus(out string messageInfo, string subject)
        {
            messageInfo = className + ".UpdateFacebookStatus(out string message, string subject) - ";
            var mailFrom = _mailFrom;
            var mailTo = _mailFrom;

            if(!_isTesting)
            {
                mailTo = _mailToFb;
            }

            var isSuccess = false;
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = _portSmtp;
                client.Host = _smtpHostName;
                client.EnableSsl = _useSslSmtp;
                client.Timeout = _timeout;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_usr, _pwd);

                //pure text email
                MailMessage message;

                //just body text email
                message = new MailMessage(mailFrom, mailTo);
                message.Subject = subject;

                client.Send(message);

                // Clean up.
                message.Dispose();

                isSuccess = true;
                messageInfo += ok;
            }
            catch (Exception ex)
            {
                messageInfo += err;
                messageInfo += " : " + ex.Message;
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// Sent status update with attachment to facebook account of Lumen.
        /// </summary>
        /// <param name="messageInfo"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static bool UpdateFacebookStatusWithAttachment(string subject,string fileNameFullDirectory, out string messageInfo)
        {
            messageInfo = className + ".UpdateFacebookStatusWithAttachment(out string message, string subject) - ";
            var mailFrom = _mailFrom;
            var mailTo = _mailFrom;

            if (!_isTesting)
            {
                mailTo = _mailToFb;
            }

            var isSuccess = false;
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = _portSmtp;
                client.Host = _smtpHostName;
                client.EnableSsl = _useSslSmtp;
                client.Timeout = _timeout;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_usr, _pwd);

                //pure text email
                MailMessage message;

                //just body text email
                message = new MailMessage(mailFrom, mailTo);
                message.Subject = subject;
                message.Body = _mailBody;

                //part to add attachment

                //string attPath = Environment.CurrentDirectory + @"\test.jpg";
                //string attPath = ConfigurationManager.AppSettings["ImageDirectory"] + "test.jpg";
                string attPath = fileNameFullDirectory;
                Attachment att = new Attachment(attPath);

                message.Attachments.Add(att);

                client.Send(message);

                // Clean up.
                message.Dispose();

                isSuccess = true;
                messageInfo += ok;
            }
            catch (Exception ex)
            {
                messageInfo += err;
                messageInfo += " : " + ex.Message;
                isSuccess = false;
            }
            return isSuccess;
        }
    }
}