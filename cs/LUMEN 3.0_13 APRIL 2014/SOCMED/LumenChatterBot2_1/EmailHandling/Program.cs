/*
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.ComponentModel;
namespace EmailHandling
{
    public class SimpleAsynchronousExample
    {
        static bool mailSent = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }
        public static void Main(string[] args)
        {
            Console.WriteLine("Send email...");

        }

        public void sendEmail()
        {

            #region SIMPLE SENDING EMAIL USING SMTP-CLIENT

            //*
            Console.WriteLine("Send email...");

            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("lumensocialrobot@gmail.com", "srrg-lskk");

            MailMessage mm = new MailMessage("presidenIndonesia@presiden.com", "lumensocialrobot@gmail.com", "test", "test");
            //MailMessage mm = new MailMessage("lumensocialrobot@gmail.com", "doubt376yeah@m.facebook.com", "test", "test");

            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);

            Console.WriteLine("Goodbye.");

            //*

            #endregion


            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("lumensocialrobot@gmail.com", "srrg-lskk");

            // Specify the e-mail sender. 
            // Create a mailing address that includes a UTF8 character 
            // in the display name.
            MailAddress from = new MailAddress("jane@contoso.com",
               "Jane " + (char)0xD8 + " Clayton",
            System.Text.Encoding.UTF8);

            // Set destinations for the e-mail message.
            MailAddress to = new MailAddress("lumensocialrobot@gmail.com");
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Body = "This is a test e-mail message sent by an application. ";

            // Include some non-ASCII characters in body and subject. 
            string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
            message.Body += Environment.NewLine + someArrows;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = "test message 1" + someArrows;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new
            SendCompletedEventHandler(SendCompletedCallback);
            // The userState can be any object that allows your callback  
            // method to identify this send operation. 
            // For this example, the userToken is a string constant. 
            string userState = "test message1";
            client.SendAsync(message, userState);
            Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
            string answer = Console.ReadLine();
            // If the user canceled the send, and mail hasn't been sent yet, 
            // then cancel the pending operation. 
            if (answer.StartsWith("c") && mailSent == false)
            {
                client.SendAsyncCancel();
            }
            // Clean up.
            message.Dispose();
            Console.WriteLine("Goodbye.");
        }

        public void getEmail()
        {

        }
    }
}

*/

// POP3 Email Client Test Harness (Main)
// =====================================
//
// copyright by Peter Huber, Singapore, 2006
// this code is provided as is, bugs are probable, free for any use at own risk, no 
// responsibility accepted. All rights, title and interest in and to the accompanying content retained.  :-)
//
// based on POP3 Client as a C# Class, by Bill Dean, http://www.codeproject.com/csharp/Pop3client.asp 
// based on Retrieve Mail From a POP3 Server Using C#, by Agus Kurniawan, http://www.codeproject.com/csharp/popapp.asp 
// based on Post Office Protocol - Version 3, http://www.ietf.org/rfc/rfc1939.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Timers;

namespace Pop3
{
    class Program
    {
        public static Pop3.Pop3MimeClient DemoClient = new Pop3.Pop3MimeClient("pop.gmail.com", 995, true, "lumensocialrobot@gmail.com", "srrg-lskk");
        public static System.Timers.Timer aTimer;
        public static int Count = 0;

        public static void OnTimeEventCheckAndReplyEmail(object source, ElapsedEventArgs e)
        {
            DemoClient.Connect();
            //get mailbox stats
            int numberOfMailsInMailbox, mailboxSize;
            DemoClient.GetMailboxStats(out numberOfMailsInMailbox, out mailboxSize);

            RxMailMessage mm;

            if (numberOfMailsInMailbox == 0)
            {
                DemoClient.Disconnect();
                //no email : donothing
            }
            else
            {
                var emailId = 1;
                DemoClient.GetEmail(emailId, out mm);
                if (mm == null)
                {
                    Console.WriteLine("Email " + emailId.ToString() + " cannot be displayed.");
                }
                else
                {
                    Console.WriteLine(mm.MailStructure());
                    MailAddress from = new MailAddress("lumensocialrobot@gmail.com");
                    MailAddress to = new MailAddress(mm.ReplyTo.Address);
                    //string body = mm.Body;
                    //MailAddress from = new MailAddress("lumensocialrobot@gmail.com");
                    //MailAddress to = new MailAddress("doubt376yeah@m.facebook.com");
                    //MailAddress to = new MailAddress("yuana2004@yahoo.co.uk");
                    string body = "OGNAK GNOUK. ";
                    string subject ="Re: " + mm.Subject.Trim();
                    //SendTestmail(from, to, subject, body);
                    Count++;
                    Console.WriteLine("Email reply from : " + from);
                    Console.WriteLine("Email reply to : " + to);
                    Console.WriteLine("Email Subject : " + subject);
                    Console.WriteLine("Email body : " + body);
                    Console.WriteLine();
                    DemoClient.Disconnect();
                    Console.WriteLine();

                }
            }
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            Console.WriteLine();

        }

        static void Main(string[] args)
        {
            // TODO: Remove the comment signs from the next line, if you want to create some sample emails.
            //SendTestmail();
            //If you get a run time error her: SmtpFailedRecipientException, 'Mailbox unavailable. The server response was: 5.7.1 Unable to relay for',
            //then you need to change the settings of the local IIS/SMTP server.

            Console.WriteLine("POP3 MIME Client Demo");
            Console.WriteLine("=====================");
            Console.WriteLine();

            //prepare pop client
            // TODO: Replace server address, username and password with your own credentials.
            //Pop3.Pop3MimeClient DemoClient = new Pop3.Pop3MimeClient("pop.gmail.com", 995, true, "lumensocialrobot@gmail.com", "srrg-lskk");

            DemoClient.Trace += new TraceHandler(Console.WriteLine);
            DemoClient.ReadTimeout = 60000; //give pop server 60 seconds to answer

            //establish connection
            //DemoClient.Connect();

            ////get mailbox stats
            //int numberOfMailsInMailbox, mailboxSize;
            //DemoClient.GetMailboxStats(out numberOfMailsInMailbox, out mailboxSize);
            //Console.WriteLine("---------- Number of email in Mail Inbox : " + numberOfMailsInMailbox);
            //Console.Write("Input command (\"1\" chekMail, \"2\" replyMail, \"3\" exit)  : ");


            ////get at most the xx first emails
            //RxMailMessage mm;
            //var input = "";

            //do
            //{
            //    if (input == "q")
            //    {

            //    }
            //    else if(input == "1")
            //    {
            //        DemoClient.Disconnect();
            //        DemoClient.Connect();
            //        DemoClient.GetMailboxStats(out numberOfMailsInMailbox, out mailboxSize);
            //        Console.WriteLine();
            //        Console.WriteLine("---------- Number of email in Mail Inbox : " + numberOfMailsInMailbox);
            //    }
            //    else if(input == "2")
            //    {
            //        var emailId = 1;
            //        DemoClient.GetEmail(emailId, out mm);
            //        if (mm == null)
            //        {
            //            Console.WriteLine("Email " + emailId.ToString() + " cannot be displayed.");
            //        }
            //        else
            //        {
            //            Console.WriteLine(mm.MailStructure());
            //            MailAddress from = new MailAddress("lumensocialrobot@gmail.com");
            //            MailAddress to = mm.From;
            //            string body = mm.Body;
            //            SendTestmail(from, to, "Test Reply",body);
            //            Console.WriteLine("email has been replied to " + mm.From);

            //        }
            //    }
            //    else if (input == "3")
            //    {
            //        var emailId = 1;
            //        DemoClient.GetEmail(emailId, out mm);
            //        if (mm == null)
            //        {
            //            Console.WriteLine("Email " + emailId.ToString() + " cannot be displayed.");
            //        }
            //        else
            //        {
            //            Console.WriteLine(mm.MailStructure());
            //            MailAddress from = new MailAddress("lumensocialrobot@gmail.com");
            //            MailAddress to = new MailAddress("doubt376yeah@m.facebook.com");
            //            SendTestmail(from, to, "Test Reply","");
            //            Console.WriteLine("email has been replied to " + mm.From);

            //        }
            //    }

            //    Console.WriteLine();
            //    Console.Write("Input command (\"1\" chekMail, \"2\" replyMail, \"3\" updateStatus, \"q\" quit)  : ");
            //    input = Console.ReadLine();

            //} while (input != "q");

            //close connection
            //DemoClient.Disconnect();


            Console.WriteLine("Start Auto Reply Program");
            Console.WriteLine("===============================");

            // Create a timer with a ten second interval.
            int aMinute = 60000;
            aTimer = new System.Timers.Timer(2000);

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += new ElapsedEventHandler(OnTimeEventCheckAndReplyEmail);

            // Set the Interval to 3 minute.
            aTimer.Interval = aMinute * 5;
            aTimer.Enabled = true;

            Console.WriteLine();
            Console.WriteLine("Press the Enter key to exit the program.");
            Console.ReadLine();
            Console.WriteLine("Closed Connection");

            Console.WriteLine("Exit Program");


        }

        static void SendMessage(SmtpClient client, MailMessage message)
        {
            client.Send(message);
            Console.WriteLine("Message '" + message.Subject + "'sent");
            // Clean up.
            message.Dispose();
        }


        ///// <summary>
        ///// Produce some test emails.
        ///// </summary>
        //static void SendTestmail()
        //{
        //    SmtpClient client = new SmtpClient();
        //    client.Port = 587;
        //    client.Host = "smtp.gmail.com";
        //    client.EnableSsl = true;
        //    client.Timeout = 10000;
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.UseDefaultCredentials = false;
        //    client.Credentials = new NetworkCredential("lumensocialrobot@gmail.com", "srrg-lskk");

        //    //pure text email
        //    // TODO: Replace email addresses.
        //    MailAddress from = new MailAddress("lumensocialrobot@gmail.com", "Any name or remove this string");
        //    MailAddress to = new MailAddress("lumensocialrobot@gmail.com", "Any name or remove this string");
        //    MailMessage message;
        //    AlternateView thisAlternateView1;
        //    AlternateView thisAlternateView2;

        //    //just body text email
        //    message = new MailMessage(from, to);
        //    message.Subject = "Text only email";
        //    message.Body = "This is a test e-mail message with just a plain text body.";
        //    SendMessage(client, message);

        //    ////pure text email with body and 1 alternative view
        //    //message = new MailMessage(from, to);
        //    //message.Subject = "pure text email with body and 1 alternative view";
        //    //message.Body = "This is the body text";
        //    //thisAlternateView1 = AlternateView.CreateAlternateViewFromString("This is the alternative view text");
        //    //message.AlternateViews.Add(thisAlternateView1);
        //    //SendMessage(client, message);

        //    ////pure text email with no body but 1 alternative view
        //    //message = new MailMessage(from, to);
        //    //message.Subject = "pure text email with no body but 1 alternative view";
        //    //thisAlternateView1 = AlternateView.CreateAlternateViewFromString("This is the alternative view text");
        //    //message.AlternateViews.Add(thisAlternateView1);
        //    //SendMessage(client, message);

        //    ////pure text email with plain text body and 2 alternative views
        //    //message = new MailMessage(from, to);
        //    //message.Subject = "pure text email with plain text body and 2 alternative views";
        //    //message.Body = "This is the body text";
        //    //thisAlternateView1 = AlternateView.CreateAlternateViewFromString("This is the alternative view text");
        //    //message.AlternateViews.Add(thisAlternateView1);
        //    //thisAlternateView2 = AlternateView.CreateAlternateViewFromString("<html><body>This is a <b>HTML<b> text body</body></html>");
        //    //thisAlternateView2.ContentType.MediaType = "text/html";
        //    //message.AlternateViews.Add(thisAlternateView2);
        //    //SendMessage(client, message);

        //    ////email with attachment and UTF8 body encoding
        //    //message = new MailMessage(from, to);
        //    //message.Subject = "email with attachment and UTF8 body encoding";
        //    //message.SubjectEncoding = System.Text.Encoding.ASCII;
        //    //message.Body = "Body text";
        //    //// Include some non-ASCII characters in body and subject.
        //    //string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
        //    //message.Body += Environment.NewLine + someArrows;
        //    //message.BodyEncoding =  System.Text.Encoding.UTF8;
        //    //// TODO: Write path and name of an existing file into the following string
        //    //Attachment thisAttachment = new Attachment(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Pic.GIF");
        //    //message.Attachments.Add(thisAttachment);

        //    //message.BodyEncoding =  System.Text.Encoding.UTF8;
        //    //message.Subject = "test message 1" + someArrows;
        //    //message.SubjectEncoding = System.Text.Encoding.UTF7;

        //    //client.Send(message);

        //    Console.WriteLine("Sending emails completed. Give POP3 server a bit of time to receive the emails, then press 'Return' key.");
        //    Console.ReadLine();
        //}



        /// <summary>
        /// Produce some test emails.
        /// </summary>
        static void SendTestmail(MailAddress from, MailAddress to, string subject,string body)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("lumensocialrobot@gmail.com", "srrg-lskk");

            //pure text email
            // TODO: Replace email addresses.
            //MailAddress from = new MailAddress("lumensocialrobot@gmail.com", "Any name or remove this string");
            //MailAddress to = new MailAddress("lumensocialrobot@gmail.com", "Any name or remove this string");
            MailMessage message;

            //just body text email
            message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            SendMessage(client, message);

            Console.WriteLine("Sending completed.");
            Console.WriteLine();
            //Console.WriteLine("Sending emails completed. Give POP3 server a bit of time to receive the emails, then press 'Return' key.");
            //Console.ReadLine();
        }

    }
}
