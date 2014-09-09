using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageProcessing.ServiceLanguageProcessing;

namespace LanguageProcessing
{
    class Program
    {
        static void Main()
        {
            Service1Client svcClient = new Service1Client();
            
            Console.WriteLine("======= Welcome to Language Processing Program =========");
            Console.WriteLine();
            Console.Write(">> textIn : ");
            var textIn = Console.ReadLine();

            var textOut = svcClient.GetResponse(textIn);

            Console.WriteLine(">> textOut : {0} (Behave : {1})",textOut.Text,textOut.Behaviour );
            Console.WriteLine();
        }
    }
}
