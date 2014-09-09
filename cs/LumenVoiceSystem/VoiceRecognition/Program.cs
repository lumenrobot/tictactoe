using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace VoiceRecognition
{
    class Program
    {
        static SpeechRecognitionEngine _recognizer = null;
        static ManualResetEvent manualResetEvent = null;
        private static LumenSpeechRecognition model;

        static void Main()
        {
            manualResetEvent = new ManualResetEvent(false);
            Console.WriteLine("****** Welcome To Lumen Voice Recognition Program (Standalone) *****");
            Console.WriteLine();
            Console.WriteLine("To Start recognize speech with Lumen, press 0");
            //Console.WriteLine("To recognize speech and make sure the computer speaks to you, press 1");
            //Console.WriteLine("To emulate speech recognition, press 2");
            //Console.WriteLine("To recognize speech using Choices and GrammarBuilder.Append, press 3");
            Console.WriteLine("To recognize speech using a DictationGrammar, press 4");
            //Console.WriteLine("To get a prompt building example, press 5");
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);
            char keychar = pressedKey.KeyChar;
            Console.WriteLine("You pressed '{0}'", keychar);
            switch (keychar)
            {
                case '0':
                    model = new LumenSpeechRecognition();
                    model.RecognizeSpeechAndWriteToConsole();
                    break;
                case '1':
                    //RecognizeSpeechAndMakeSureTheComputerSpeaksToYou();
                    break;
                case '2':
                    //EmulateRecognize();
                    break;
                case '3':
                    //SpeechRecognitionWithChoices();
                    break;
                case '4':
                    model = new LumenSpeechRecognition();
                    model.SpeechRecognitionWithDictationGrammar();
                    break;
                case '5':
                    //PromptBuilding();
                    break;
                default:
                    Console.WriteLine("You didn't press 0, 1, 2, 3, 4, or 5!");
                    Console.WriteLine("Press any key to continue . . .");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                    break;
            }
            if (keychar != '5')
            {
                manualResetEvent.WaitOne();
            }
            if (_recognizer != null)
            {
                _recognizer.Dispose();
            }

            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey(true);
        }

        public static void VoiceText (string text)
        {
            Console.WriteLine("Confirmed : {0}",text);
            Console.WriteLine();
        }
    }
}
