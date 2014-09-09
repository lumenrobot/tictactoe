using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.Threading;
using Aldebaran.Proxies;

namespace VoiceRecognitionWithLumen
{
    //Initiale mode
    //Update voice input
    //Conver voice to text
    class LumenSpeechRecognition
    {
        private static string speechContent = "SpeechContent";
        private static string ip;
        private SpeechRecognitionEngine _recognizer;
        private static string eventName = "SpeechState";
        private static int eventValue = 0 ;
        private const int port = 9559;
        private MemoryProxy memory;
        private ManualResetEvent manualResetEvent = null;
        private string eventSubcribe = "MoveState";


        //public const string ip = "167.205.66.109";
        //public const string ip = "167.205.66.142";
        //public const string ip = "127.0.0.1";

        public LumenSpeechRecognition(string ipParam)
        {
            ip = ipParam;
            memory = new MemoryProxy(ip,port);
            Console.WriteLine("Lumen is Ready ...");

            RobotPostureProxy rb = new RobotPostureProxy(ip, port);
            float speed = 1.0f;
            var success = rb.goToPosture("Stand", speed);
            RunTimer();
            //memory.subscribeToEvent(eventSubcribe,"LumenSpeechRecognition","Wait");
        }

        #region Speech Recognition with Grammar Builder
        public void RecognizeSpeechAndWriteToConsole()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("hello"))); // load a "test" grammar
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("intro"))); // load a "test" grammar
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
            _recognizer.SpeechRecognized += _SpeechRecognitionRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _SpeechRecognitionRejected; // if recognized speech is rejected, call the specified method
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous

        }

        private void Wait()
        {
            do
            {
                int second = 3;
                for (int i = 0; i < second; i++)
                {
                    // Sixty seconds is one minute.
                    Thread.Sleep(1000);
                }
                Console.WriteLine("eventSubcribe : " + memory.getData(eventSubcribe).ToString());
            } while (int.Parse(memory.getData(eventSubcribe).ToString()) == 1);

            Console.WriteLine("exit wait, eventSubcribe : " + memory.getData(eventSubcribe).ToString());
            int freq = 3000;
            int duration = 200;
            Console.Beep(freq, duration);
        }

        private void RunTimer()
        {
            //* timer to wait 
            int second = 2;
            for (int i = 0; i < second; i++)
            {
                // Sixty seconds is one minute.
                Thread.Sleep(1000);
            }
            //*/
        }
        private void _SpeechRecognitionRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "hello")
            {
                eventValue = 1;
                memory.declareEvent(eventName);
                memory.raiseEvent(eventName, eventValue);

                Console.WriteLine("hello");
                Console.WriteLine("Event : ({0}, {1})",eventName,eventValue);
                
                memory.insertData(speechContent,"Hello");

                //TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                //RunTimer();
                //tts.say("Hello");
            }
            
            else if (e.Result.Text == "yes")
            {
                eventValue = 2;
                memory.declareEvent(eventName);
                memory.raiseEvent(eventName, eventValue);

                Console.WriteLine("yes");
                Console.WriteLine("Event : ({0}, {1})", eventName, eventValue);

                memory.insertData(speechContent, "Yes.");

                //TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                //RunTimer();

                //tts.say("Yes.");
            }
            else if (e.Result.Text == "no")
            {
                eventValue = 3;
                memory.declareEvent(eventName);
                memory.raiseEvent(eventName, eventValue);

                Console.WriteLine("no");
                Console.WriteLine("Event : ({0}, {1})", eventName, eventValue);

                Console.WriteLine("No.");
                memory.insertData(speechContent, "No.");

                //TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                //RunTimer();
                //tts.say("No");
            }
            else if (e.Result.Text == "intro")
            {
                eventValue = 4;
                memory.declareEvent(eventName);
                memory.raiseEvent(eventName, eventValue);

                Console.WriteLine("intro");
                Console.WriteLine("Event : ({0}, {1})", eventName, eventValue);

                memory.insertData(speechContent, "Hello, my name is Lumen. I am a robot. You can call me Lumen.");


                //TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
                //RunTimer();
                //tts.say("Hello, my name is Lumen. I am a robot. You can call me Lumen.");
                //tts.say("Wow, you just call me Lumen. I love it."); 

                //to do : upload happy laugh
                //var ap = new AudioPlayerProxy(ip, port);
                //ap.playFile("/home/nao/sound/mp3/Laugh_Delighted_Giggle_Child.mp3");
            }
            //else if (e.Result.Text == "stand")
            //{
            //    Console.WriteLine("stand");

            //    eventValue = 3;
            //    memory.declareEvent(eventName);
            //    memory.raiseEvent(eventName, eventValue);

            //    TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
            //    RobotPostureProxy rb = new RobotPostureProxy(ip, port);
            //    var mp = new MotionProxy(ip, port);

            //    mp.setStiffnesses("Body", 1.0f);
            //    float speed = 1.0f;

            //    tts.post.say("stand position");
            //    var success = rb.goToPosture("Stand", speed);

            //    if (!success)
            //    {
            //        tts.say("Sorry, I am unable to stand.");
            //    }
            //}
            //else if (e.Result.Text == "sit")
            //{
            //    MemoryProxy memory = new MemoryProxy(ip, port);
            //    memVal = "sit";
            //    memory.insertData(memKey, memVal);
            //    Console.WriteLine("sit");
            //    TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
            //    RobotPostureProxy rb = new RobotPostureProxy(ip, port);
            //    var mp = new MotionProxy(ip, port);

            //    float speed = 1.0f;

            //    tts.post.say("sit position");

            //    var success = rb.goToPosture("Sit", speed);
            //    mp.setStiffnesses("Body", 0.0f);
            //    if (!success)
            //    {
            //        tts.say("Sorry, I am unable to sit.");
            //    }
            //}
            //else if (e.Result.Text == "rest now")
            //{
            //    MemoryProxy memory = new MemoryProxy(ip, port);
            //    memVal = "rest";
            //    memory.insertData(memKey, memVal);
            //    Console.WriteLine("rest now");
            //    TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
            //    tts.post.say("Thank you.Have a nice day.");

            //    float speed = 1.0f;
            //    RobotPostureProxy rb = new RobotPostureProxy(ip, port);
            //    var success = rb.goToPosture("Crouch", speed);
            //    var mp = new MotionProxy(ip, port);
            //    mp.setStiffnesses("Body", 0.0f);
            //    //manualResetEvent.Set();
            //}
            //else if (e.Result.Text == "music")
            //{
            //    MemoryProxy memory = new MemoryProxy(ip, port);
            //    memVal = "music";
            //    memory.insertData(memKey, memVal);
            //    Console.WriteLine("music");

            //    var ap = new AudioPlayerProxy(ip, port);
            //    TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);

            //    //ap.playFile("/home/nao/sound/wav/alrighty.wav");
            //    //tts.say("Ups, sorry for that.");
            //    tts.say("Here is your music.");
            //    //ap.playFile("/home/nao/sound/mp3/Magic_Marker.mp3");
            //    ap.playFile("/home/nao/sound/mp3/Cowcow.mp3");
            //}
            //else if (e.Result.Text == "Hello Lumen")
            //{
            //    Console.WriteLine("Hello Lumen");
            //    TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
            //    tts.say("Hello master.");
            //    var mp = new MotionProxy(ip, port);
            //}
            //else if (e.Result.Text == "stop music")
            //{
            //    MemoryProxy memory = new MemoryProxy(ip, port);
            //    memVal = "stop music";
            //    memory.insertData(memKey, memVal);
            //    Console.WriteLine("stop music");
            //    TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
            //    tts.say("Ok.Stop playing music now.");
            //    var ap = new AudioPlayerProxy(ip, port);
            //    ap.stopAll();
            //}

            Wait();

        }
        static void _SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech rejected. Did you mean:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
        }

#endregion 

        #region Speech recognition with DictationGrammar
        public void SpeechRecognitionWithDictationGrammar()
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

        private void speechRecognitionWithDictationGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "exit")
            {
                LumenSpeechAfter(e.Result.Text);
                manualResetEvent.Set();
                return;
            }
            LumenSpeechAfter(e.Result.Text);
            Console.WriteLine("You said: " + e.Result.Text);
        }

        private void LumenSpeechAfter(string text)
        {
            TextToSpeechProxy tts = new TextToSpeechProxy(ip,port);
            tts.say(text);
        }
        #endregion

    }
}
