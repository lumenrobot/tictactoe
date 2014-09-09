using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenPop.Mime;
using OpenPop.Pop3;
using System.Net;
using System.Net.Mail;
using System.Timers;
using LumenChatterBot2.Repository;

namespace EmailHandlingOpenPop
{
    class Program
    {
        private static string popHostname = "pop.gmail.com";
        private static int port = 995;
        private static bool useSsl = true;
        private static string usr = "lumensocialrobot@gmail.com";
        private static string pwd = "srrg-lskk";

        private static string smtpHostname = "smtp.gmail.com";
        private static string mailSubject = "";
        private static string mailBody = "";
        private static MailAddress mailFrom = new MailAddress("lumensocialrobot@gmail.com", "Lumen");
        private static MailAddress mailToFb = new MailAddress("doubt376yeah@m.facebook.com", "Lumen Fb");

        private static int timerUnit = 60000;
        private static int count = 0;
        private static Timer timerSet = new Timer(2000);

        static void Main()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("FB autoreply service program using OpenPop.Net");
            Console.WriteLine("=================================================");
            mailSubject = SocmedBotRepository.StartOnline();
            //var datetime = DateTime.Now;
            //mailSubject = mailSubject + " " + datetime.ToShortTimeString() + ", I am online guys... :D";
            //SendMailAttachment(mailFrom, mailToFb, mailSubject, mailBody);
            //SendMailAttachment(mailFrom, mailFrom, mailSubject, mailBody);

            // Set timer initial value to run.
            timerSet = new Timer(2000);

            // Hook up the Elapsed event for the timer.
            timerSet.Elapsed += new ElapsedEventHandler(OnTimeEventCheckAndReplyEmail);

            // Set the Interval to 3 minute.
            timerSet.Interval = timerUnit * 1;
            timerSet.Enabled = true;

            Console.WriteLine();
            Console.WriteLine("Service Started");
            Console.WriteLine("Press the Enter key to exit the program.......");
            Console.WriteLine("=================================================");
            Console.WriteLine();
            Console.WriteLine();

            Console.ReadLine();
            Console.WriteLine("Service Stopped");
            Console.WriteLine("Exiting Program");
        }

        public static void OnTimeEventCheckAndReplyEmail(object source, ElapsedEventArgs e)
        {
            count = count + 1;
            Console.WriteLine("Checking Count : - " + count + " -");
            AutoCheckNewEmailAndReply();
        }


        //single reply mail
        /*
        static void AutoCheckNewEmailAndReply()
        {
            var allMessage = FetchAllMessages(popHostname, port, useSsl, usr, pwd);

            if (allMessage.Count > 0)
            {
                var totalMsg = allMessage.Count;

                var lastMessage = allMessage[0];

                Console.WriteLine("There are : " + totalMsg + " email.");
                Console.WriteLine();
                Console.WriteLine("Message Detail :");
                Console.WriteLine("> Subject : " + lastMessage.Headers.Subject);
                Console.WriteLine("> To : " + lastMessage.Headers.To[0].MailAddress);
                Console.WriteLine("> From : " + lastMessage.Headers.From);

                if (lastMessage.Headers.ReplyTo != null)
                {
                    Console.WriteLine("> Reply To : " + lastMessage.Headers.ReplyTo);
                    Console.WriteLine("> Reply To Display Name : " + lastMessage.Headers.ReplyTo.DisplayName);
                    Console.WriteLine("> Reply To Address : " + lastMessage.Headers.ReplyTo.Address);
                    Console.WriteLine("> Reply To HasValidMailAddress : " +
                                      lastMessage.Headers.ReplyTo.HasValidMailAddress);
                    Console.WriteLine("> Reply To MailAddress : " + lastMessage.Headers.ReplyTo.MailAddress);

                    MessagePart plainText = lastMessage.FindFirstPlainTextVersion();
                    var builder = new StringBuilder();
                    builder.Append(plainText.GetBodyAsText());
                    string mainContent = builder.ToString();
                    var mainContentSplit = mainContent.Split('"');
                    var postedComment = "";

                    if (mainContentSplit.Any())
                    {
                        for (var i = 0; i < mainContentSplit.Count(); i++)
                        {
                            Console.WriteLine();
                            Console.WriteLine("> Body Mail [" + i + "]  : " + mainContentSplit[i]);
                        }
                        postedComment = mainContentSplit[1];
                    }
                    else
                    {
                        Console.WriteLine("------ Body Message is empty --------");
                    }

                    string mailToAddress = lastMessage.Headers.ReplyTo.Address;
                    string mailToDisplayName = lastMessage.Headers.ReplyTo.DisplayName;
                    var newMailTo = new MailAddress(mailToAddress, mailToDisplayName);
                    mailSubject = "Re: " + lastMessage.Headers.Subject;
                    mailBody = SocmedBotRepository.ConversationInput(postedComment);

                    //do sent via smtp
                    SendMail(mailFrom, newMailTo, mailSubject, mailBody);
                }
            }
            else
            {
                Console.WriteLine("There are no message.");
            }
            Console.WriteLine();
        }

        //*/

        //multiple reply
        static void AutoCheckNewEmailAndReply()
        {
            var allMessage = FetchAllMessages(popHostname, port, useSsl, usr, pwd);

            if (allMessage.Count > 0)
            {
                var totalMsg = allMessage.Count;


                Console.WriteLine("There are : " + totalMsg + " email.");
                Console.WriteLine();

                foreach (var message in allMessage)
                {
                    Console.WriteLine("Message Detail :");
                    Console.WriteLine("> Subject : " + message.Headers.Subject);
                    Console.WriteLine("> To : " + message.Headers.To[0].MailAddress);
                    Console.WriteLine("> From : " + message.Headers.From);

                    if (message.Headers.From.DisplayName.ToLower() == "facebook")
                    {
                        if (message.Headers.ReplyTo != null)
                        {
                            try
                            {


                                Console.WriteLine("> Reply To : " + message.Headers.ReplyTo);
                                Console.WriteLine("> Reply To Display Name : " + message.Headers.ReplyTo.DisplayName);
                                Console.WriteLine("> Reply To Address : " + message.Headers.ReplyTo.Address);
                                Console.WriteLine("> Reply To HasValidMailAddress : " +
                                                  message.Headers.ReplyTo.HasValidMailAddress);
                                Console.WriteLine("> Reply To MailAddress : " + message.Headers.ReplyTo.MailAddress);

                                MessagePart plainText = message.FindFirstPlainTextVersion();
                                var builder = new StringBuilder();
                                builder.Append(plainText.GetBodyAsText());
                                string mainContent = builder.ToString();
                                var mainContentSplit = mainContent.Split('"');
                                var postedComment = "";

                                if (mainContentSplit.Any())
                                {
                                    for (var i = 0; i < mainContentSplit.Count(); i++)
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine("> Body Mail [" + i + "]  : " + mainContentSplit[i]);
                                    }
                                    postedComment = mainContentSplit[1];
                                }
                                else
                                {
                                    Console.WriteLine("------ Body Message is empty --------");
                                }

                                string mailToAddress = message.Headers.ReplyTo.Address;
                                string mailToDisplayName = message.Headers.ReplyTo.DisplayName;
                                var newMailTo = new MailAddress(mailToAddress, mailToDisplayName);
                                mailSubject = "Re: " + message.Headers.Subject;
                                mailBody = SocmedBotRepository.ConversationInput(postedComment);

                                //do sent via smtp
                                SendMail(mailFrom, newMailTo, mailSubject, mailBody);
                            }
                            catch (Exception)
                            {

                                Console.WriteLine("--- check inbox error --- skipped");
                            }
                        }
                    }

                }


            }
            else
            {
                Console.WriteLine("There are no message.");
            }
            Console.WriteLine();
        }


        static void SendMessageToServer(SmtpClient client, MailMessage message)
        {
            try
            {
                client.Send(message);
                Console.WriteLine("=============== Mail has been send ================");
                Console.WriteLine(">Subject : " + message.Subject);
                Console.WriteLine(">From : " + message.From);
                Console.WriteLine(">To : " + message.To);
                Console.WriteLine();
                // Clean up.
                message.Dispose();
            }
            catch (Exception)
            {

                Console.WriteLine("--- Sending error --- Skipped");
            }
        }

        /// <summary>
        /// Produce some test emails.
        /// </summary>
        static void SendMail(MailAddress mailFrom, MailAddress mailTo, string subject, string body)
        {
            try
            {
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

                SendMessageToServer(client, message);

                Console.WriteLine("========== Sending completed ===========");
                Console.WriteLine();
            }
            catch (Exception)
            {

                throw;
            }
        }


        static void SendMailAttachment(MailAddress mailFrom, MailAddress mailTo, string subject, string body)
        {
            try
            {
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
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Produce email with image as attachment.
        /// </summary>
        static void SendMailWithImageAttachment(MailAddress mailFrom, MailAddress mailTo, string subject, string body)
        {
            Console.WriteLine("======== Sending Message ========== ");

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = smtpHostname;
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(usr, pwd);

            //pure text email
            MailMessage message;

            //just body text email
            message = new MailMessage(mailFrom, mailTo);
            message.Subject = subject;
            message.Body = body;

            SendMessageToServer(client, message);

            Console.WriteLine("========== Sending completed ===========");
            Console.WriteLine();
        }


        /// <summary>
        /// Example showing:
        ///  - how to fetch all messages from a POP3 server
        /// </summary>
        /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
        /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
        /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
        /// <param name="username">Username of the user on the server</param>
        /// <param name="password">Password of the user on the server</param>
        /// <returns>All Messages on the POP3 server</returns>
        public static List<Message> FetchAllMessages(string hostname, int port, bool useSsl, string username, string password)
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
    }
}
