
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;


namespace VoiceRecognitionWithLumen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press 0 to start Lumen Voice Recognition Program");
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);
            char keychar = pressedKey.KeyChar;
            Console.WriteLine("You pressed '{0}'", keychar);
            switch (keychar)
            {
                case '0':
                    RecognizeSpeechAndWriteToConsole();
                    break;
                //case '1':
                //    RecognizeSpeechAndMakeSureTheComputerSpeaksToYou();
                //    break;
                //case '2':
                //    EmulateRecognize();
                //    break;
                //case '3':
                //    SpeechRecognitionWithChoices();
                //    break;
                //case '4':
                //    SpeechRecognitionWithDictationGrammar();
                //    break;
            }
            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey(true);

        }

        public static void RecognizeSpeechAndWriteToConsole()
        {
            // Create a new SpeechRecognizer instance.
            SpeechRecognizer sr = new SpeechRecognizer();
            Choices colors = new Choices();
            colors.Add(new string[] { "red", "green", "blue" });

            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(colors);

            // Create the Grammar instance.
            Grammar g = new Grammar(gb);

            sr.LoadGrammar(g);
            sr.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_SpeechRecognized);

        }

        public static void sr_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("message : " + e.Result.Text);
        }
    }
}

//*/


//*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Aldebaran.Proxies;

namespace StartingWithSpeechRecognition
{
    class Program
    {
        private static string memKey = "SpeechState";
        private static string memVal = "";
        public const string ip = "167.205.56.179";
        //public const string ip = "167.205.66.142";
        //public const string ip = "127.0.0.1";
        public const int port = 9559;
        static SpeechRecognitionEngine _recognizer = null;
        static ManualResetEvent manualResetEvent = null;
        static void Main(string[] args)
        {
            manualResetEvent = new ManualResetEvent(false);
            Console.WriteLine("To recognize speech with Lumen, press 0");
            Console.WriteLine("To recognize speech and make sure the computer speaks to you, press 1");
            Console.WriteLine("To emulate speech recognition, press 2");
            Console.WriteLine("To recognize speech using Choices and GrammarBuilder.Append, press 3");
            Console.WriteLine("To recognize speech using a DictationGrammar, press 4");
            Console.WriteLine("To get a prompt building example, press 5");
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);
            char keychar = pressedKey.KeyChar;
            Console.WriteLine("You pressed '{0}'", keychar);
            switch (keychar)
            {
                case '0':
                    LumenInitialPosition();
                    RecognizeSpeechAndWriteToConsole();
                    break;
                case '1':
                    RecognizeSpeechAndMakeSureTheComputerSpeaksToYou();
                    break;
                case '2':
                    EmulateRecognize();
                    break;
                case '3':
                    SpeechRecognitionWithChoices();
                    break;
                case '4':
                    SpeechRecognitionWithDictationGrammar();
                    break;
                case '5':
                    PromptBuilding();
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
        #region Recognize speech and write to console

        public static void LumenInitialPosition()
        {
            Console.WriteLine("Lumen is Ready ...");

            RobotPostureProxy rb = new RobotPostureProxy(ip, port);
            float speed = 1.0f;
            var success = rb.goToPosture("Crouch", speed);

        }

        static void RecognizeSpeechAndWriteToConsole()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("hello"))); // load a "test" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("hi lumen"))); // load a "test" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("stand"))); // load a "test" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("sit"))); // load a "exit" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("music"))); // load a "exit" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("rest now"))); // load a "exit" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("stop music"))); // load a "exit" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("what can you do"))); // load a "exit" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.SpeechRecognized += _recognizeSpeechAndWriteToConsole_SpeechRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _recognizeSpeechAndWriteToConsole_SpeechRecognitionRejected; // if recognized speech is rejected, call the specified method
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous

        }
        static void _recognizeSpeechAndWriteToConsole_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if(e.Result.Text=="hello")
            {
                Console.WriteLine("hello");

                TextToSpeechProxy tts = new TextToSpeechProxy(ip,port);
                //tts.say("Hello, my name is Lumen. I am a robot. You can call me Lumen.");

                MemoryProxy memory = new MemoryProxy(ip,port);
                memVal = "Hello";
                memory.insertData(memKey,memVal);

                //var getMemVal = memory.getData(memKey);
                //tts.say("memory key "+ memKey +", value " + getMemVal.ToString());

                //memVal = "stop";
                //memory.insertData(memKey,memVal);
                //tts.say("change, memory key " + memKey + ", value " + getMemVal.ToString());


            }
            else if(e.Result.Text=="hi lumen")
            {
                Console.WriteLine("hi lumen");
                MemoryProxy memory = new MemoryProxy(ip, port);
                memVal = "Hi";
                memory.insertData(memKey, memVal);

                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                tts.say("Wow, you just call me Lumen. I love it."); 
                
                //to do : upload happy laugh
                var ap = new AudioPlayerProxy(ip, port);
                ap.playFile("/home/nao/sound/mp3/Laugh_Delighted_Giggle_Child.mp3");
            }
            else if (e.Result.Text == "what can you do")
            {
                MemoryProxy memory = new MemoryProxy(ip, port);
                memVal = "what";
                memory.insertData(memKey, memVal);

                Console.WriteLine("what can you do");
                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                tts.say("I can do many things. I can speak, stand, sit, or play music. Give me a command and I will do it.");
            }
            else if (e.Result.Text == "stand")
            {
                Console.WriteLine("stand");

                MemoryProxy memory = new MemoryProxy(ip, port);
                memVal = "stand";
                memory.insertData(memKey, memVal);
                
                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                var mp = new MotionProxy(ip, port);

                mp.setStiffnesses("Body",1.0f);
                float speed = 1.0f;

                tts.post.say("stand position");
                var success = rb.goToPosture("Stand", speed);

                if (!success)
                {
                    tts.say("Sorry, I am unable to stand.");
                }
            }
            else if (e.Result.Text == "sit")
            {
                MemoryProxy memory = new MemoryProxy(ip, port);
                memVal = "sit";
                memory.insertData(memKey, memVal);
                Console.WriteLine("sit");
                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                var mp = new MotionProxy(ip, port);

                float speed = 1.0f;

                tts.post.say("sit position");

                var success = rb.goToPosture("Sit", speed);
                mp.setStiffnesses("Body", 0.0f);
                if (!success)
                {
                    tts.say("Sorry, I am unable to sit.");
                }
            }
            else if (e.Result.Text == "rest now")
            {
                MemoryProxy memory = new MemoryProxy(ip, port);
                memVal = "rest";
                memory.insertData(memKey, memVal);
                Console.WriteLine("rest now");
                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                tts.post.say("Thank you.Have a nice day.");
                
                float speed = 1.0f;
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                var success = rb.goToPosture("Crouch", speed);
                var mp = new MotionProxy(ip, port);
                mp.setStiffnesses("Body", 0.0f);
                //manualResetEvent.Set();
            }
            else if (e.Result.Text=="music")
            {
                MemoryProxy memory = new MemoryProxy(ip, port);
                memVal = "music";
                memory.insertData(memKey, memVal);
                Console.WriteLine("music");

                var ap = new AudioPlayerProxy(ip, port);
                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);

                //ap.playFile("/home/nao/sound/wav/alrighty.wav");
                //tts.say("Ups, sorry for that.");
                tts.say("Here is your music.");
                //ap.playFile("/home/nao/sound/mp3/Magic_Marker.mp3");
                ap.playFile("/home/nao/sound/mp3/Cowcow.mp3");
            }
            else if (e.Result.Text=="Hello Lumen")
            {
                Console.WriteLine("Hello Lumen");
                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                tts.say("Hello master.");
                var mp = new MotionProxy(ip, port);
            }
            else if (e.Result.Text == "stop music")
            {
                MemoryProxy memory = new MemoryProxy(ip, port);
                memVal = "stop music";
                memory.insertData(memKey, memVal);
                Console.WriteLine("stop music");
                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                tts.say("Ok.Stop playing music now.");
                var ap = new AudioPlayerProxy(ip, port);
                ap.stopAll();
            }
                 
        }
        static void _recognizeSpeechAndWriteToConsole_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech rejected. Did you mean:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
        }
        #endregion

        #region Recognize speech and make sure the computer speaks to you (text to speech)
        static void RecognizeSpeechAndMakeSureTheComputerSpeaksToYou()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("hello computer"))); // load a "hello computer" grammar
            _recognizer.SpeechRecognized += _recognizeSpeechAndMakeSureTheComputerSpeaksToYou_SpeechRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _recognizeSpeechAndMakeSureTheComputerSpeaksToYou_SpeechRecognitionRejected;
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous
        }
        static void _recognizeSpeechAndMakeSureTheComputerSpeaksToYou_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "hello computer")
            {
                SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
                speechSynthesizer.Speak("hello user");
                speechSynthesizer.Dispose();
            }
            manualResetEvent.Set();
        }
        static void _recognizeSpeechAndMakeSureTheComputerSpeaksToYou_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (e.Result.Alternates.Count == 0)
            {
                Console.WriteLine("No candidate phrases found.");
                return;
            }
            Console.WriteLine("Speech rejected. Did you mean:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
        }
        #endregion

        #region Emulate speech recognition
        static void EmulateRecognize()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("emulate speech"))); // load "emulate speech" grammar
            _recognizer.SpeechRecognized += _emulateRecognize_SpeechRecognized;

            _recognizer.EmulateRecognize("emulate speech");

        }
        static void _emulateRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "emulate speech")
            {
                Console.WriteLine("Speech was emulated!");
            }
            manualResetEvent.Set();
        }
        #endregion

        #region Speech recognition with Choices and GrammarBuilder.Append
        static void SpeechRecognitionWithChoices()
        {
            _recognizer = new SpeechRecognitionEngine();
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append("I"); // add "I"
            grammarBuilder.Append(new Choices("like", "dislike")); // load "like" & "dislike"
            grammarBuilder.Append(new Choices("dogs", "cats", "birds", "snakes", "fishes", "tigers", "lions", "snails", "elephants")); // add animals
            _recognizer.RequestRecognizerUpdate();
            _recognizer.LoadGrammar(new Grammar(grammarBuilder)); // load grammar
            _recognizer.SpeechRecognized += speechRecognitionWithChoices_SpeechRecognized;
            _recognizer.SetInputToDefaultAudioDevice(); // set input to default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech
        }

        static void speechRecognitionWithChoices_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Do you really " + e.Result.Words[1].Text + " " + e.Result.Words[2].Text + "?");
            manualResetEvent.Set();
        }
        #endregion

        #region Speech recognition with DictationGrammar
        static void SpeechRecognitionWithDictationGrammar()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.RequestRecognizerUpdate();
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("exit")));
            _recognizer.RequestRecognizerUpdate();
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.SpeechRecognized += speechRecognitionWithDictationGrammar_SpeechRecognized;
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        static void speechRecognitionWithDictationGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "exit")
            {
                manualResetEvent.Set();
                return;
            }
            Console.WriteLine("You said: " + e.Result.Text);
        }
        #endregion

        #region Prompt building
        static void PromptBuilding()
        {
            PromptBuilder builder = new PromptBuilder();

            builder.StartSentence();
            builder.AppendText("This is a prompt building example.");
            builder.EndSentence();

            builder.StartSentence();
            builder.AppendText("Now, there will be a break of 2 seconds.");
            builder.EndSentence();

            builder.AppendBreak(new TimeSpan(0, 0, 2));

            builder.StartStyle(new PromptStyle(PromptVolume.ExtraSoft));
            builder.AppendText("This text is spoken extra soft.");
            builder.EndStyle();

            builder.StartStyle(new PromptStyle(PromptRate.Fast));
            builder.AppendText("This text is spoken fast.");
            builder.EndStyle();

            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            synthesizer.Speak(builder);
            synthesizer.Dispose();
        }
        #endregion

    }
}

//*/