using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteModule
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Welcome to Lumen Remote Module Program ===");
            Console.WriteLine();
            Console.Write("Text : ");
            var text = Console.ReadLine();
            Console.Write("Value : ");
            var val = int.Parse(Console.ReadLine());
            Console.WriteLine();
            var model = new LumenRemoteModul();
            model.TTS(text,val);
            Console.WriteLine();
        }
    }
}
