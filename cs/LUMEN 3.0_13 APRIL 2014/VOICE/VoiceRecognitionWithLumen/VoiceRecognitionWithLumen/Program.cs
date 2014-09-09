
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Aldebaran.Proxies;
using VoiceRecognitionWithLumen;

namespace StartingWithSpeechRecognition
{
    class Program
    {
        public const string ip = "167.205.66.109";
        //public const string ip = "127.0.0.1";
        static SpeechRecognitionEngine _recognizer = null;
        static ManualResetEvent manualResetEvent = null;
        private static LumenSpeechRecognition model;

        static void Main()
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
                    //LumenInitialPosition();
                    //RecognizeSpeechAndWriteToConsole();
                    model = new LumenSpeechRecognition(ip);
                    model.RecognizeSpeechAndWriteToConsole();
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
                    //SpeechRecognitionWithDictationGrammar();
                    model = new LumenSpeechRecognition(ip);
                    model.SpeechRecognitionWithDictationGrammar();
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
        /*
        public static void LumenInitialPosition()
        {
            Console.WriteLine("Lumen is Ready ...");

            RobotPostureProxy rb = new RobotPostureProxy(ip, port);
            float speed = 1.0f;
            var success = rb.goToPosture("Stand", speed);

        }

        static void RecognizeSpeechAndWriteToConsole()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("hello"))); // load a "test" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("introduce"))); // load a "test" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("yes"))); // load a "test" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("no"))); // load a "exit" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("music"))); // load a "exit" grammar
            //_recognizer.RequestRecognizerUpdate(); // request for recognizer update
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("rest now"))); // load a "exit" grammar
            //_recognizer.RequestRecognizerUpdate(); // request for recognizer update
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("stop music"))); // load a "exit" grammar
            //_recognizer.RequestRecognizerUpdate(); // request for recognizer update
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("what can you do"))); // load a "exit" grammar
            //_recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.SpeechRecognized += _recognizeSpeechAndWriteToConsole_SpeechRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _recognizeSpeechAndWriteToConsole_SpeechRecognitionRejected; // if recognized speech is rejected, call the specified method
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous

        }
        static void _recognizeSpeechAndWriteToConsole_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if(e.Result.Text=="hello")
            {
                MemoryProxy memory = new MemoryProxy(ip, port);
                //memVal = "hello";
                int eventVal = 1;
                //memory.insertData(memKey,memVal);
                memory.declareEvent(memKey);
                memory.raiseEvent(memKey, eventVal);
                //memory.raiseEvent(memKey, memVal);

                Console.WriteLine("hello");

                TextToSpeechProxy tts = new TextToSpeechProxy(ip,port);
                tts.say("Hello");

                //var getMemVal = memory.getData(memKey);
                //tts.say("memory key "+ memKey +", value " + getMemVal.ToString());

                //memVal = "stop";
                //memory.insertData(memKey,memVal);
                //tts.say("change, memory key " + memKey + ", value " + getMemVal.ToString());


            }
            else if(e.Result.Text=="introduce")
            {
                MemoryProxy memory = new MemoryProxy(ip, port);
                //memVal = "hello";
                int eventVal = 4;
                //memory.insertData(memKey,memVal);
                memory.declareEvent(memKey);
                memory.raiseEvent(memKey, eventVal);

                Console.WriteLine("introduce");

                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                tts.say("Hello, my name is Lumen. I am a robot. You can call me Lumen.");

                //tts.say("Wow, you just call me Lumen. I love it."); 
                
                //to do : upload happy laugh
                //var ap = new AudioPlayerProxy(ip, port);
                //ap.playFile("/home/nao/sound/mp3/Laugh_Delighted_Giggle_Child.mp3");
            }
            else if (e.Result.Text == "yes")
            {
                MemoryProxy memory = new MemoryProxy(ip, port);
                //memVal = "hello";
                int eventVal = 2;
                //memory.insertData(memKey,memVal);
                memory.declareEvent(memKey);
                memory.raiseEvent(memKey, eventVal);

                Console.WriteLine("yes");
                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                tts.say("Yes.");
            }
            else if (e.Result.Text == "no")
            {
                MemoryProxy memory = new MemoryProxy(ip, port);
                //memVal = "hello";
                int eventVal = 3;
                //memory.insertData(memKey,memVal);
                memory.declareEvent(memKey);
                memory.raiseEvent(memKey, eventVal);

                Console.WriteLine("No.");
                TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                tts.say("No");
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

        */
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