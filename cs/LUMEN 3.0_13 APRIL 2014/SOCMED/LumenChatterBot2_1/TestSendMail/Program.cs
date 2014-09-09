using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSendMail
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Press 1 to send email or ESC to quit.");
            var key = Console.ReadLine();
            while (key == "1")
            {
                TestSendMail mail = new TestSendMail();
                mail.SendMail();

                Console.WriteLine("Press 1 to send email or ESC to quit.");
                key = Console.ReadLine();
            }
            //if(key=="1")
            //{
            //    TestSendMail mail = new TestSendMail();
            //    mail.SendMail();
            //}
        }
    }
}
