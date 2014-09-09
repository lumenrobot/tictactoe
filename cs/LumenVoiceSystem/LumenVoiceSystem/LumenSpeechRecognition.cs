using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.Threading;
using LumenVoiceSystem;

namespace VoiceRecognition
{
    public class LumenSpeechRecognition
    {
        private SpeechRecognitionEngine _recognizer;
        private ManualResetEvent manualResetEvent = null;

        //Constructor
        public LumenSpeechRecognition()
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("Lumen Speech Recognition Started");
            Console.WriteLine("==============================================");

            //RunTimer();
        }

        #region Speech Recognition with Grammar Builder
        public void RecognizeSpeechAndWriteToConsole()
        {
            _recognizer = new SpeechRecognitionEngine();
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("hello"))); // load a "test" grammar
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("intro"))); // load a "test" grammar
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("yes"))); // load a "test" grammar
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("no"))); // load a "exit" grammar
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("how are you"))); // load a "exit" grammar
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("what is your name"))); // load a "exit" grammar
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("are you real"))); // load a "exit" grammar

            var grammar = Program.LoadGrammar();

            foreach (var s in grammar)
            {
                _recognizer.LoadGrammar(new Grammar(new GrammarBuilder(s))); // load a grammar
            }
            
            _recognizer.RequestRecognizerUpdate(); // request for recognizer update
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("music"))); // load a "exit" grammar
            //_recognizer.RequestRecognizerUpdate(); // request for recognizer update
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("rest now"))); // load a "exit" grammar
            //_recognizer.RequestRecognizerUpdate(); // request for recognizer update
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("stop music"))); // load a "exit" grammar
            //_recognizer.RequestRecognizerUpdate(); // request for recognizer update
            //_recognizer.LoadGrammar(new Grammar(new GrammarBuilder("what can you do"))); // load a "exit" grammar
            //_recognizer.RequestRecognizerUpdate(); // request for recognizer update

            //_recognizer.InitialSilenceTimeout = TimeSpan.FromSeconds(3);
            //_recognizer.BabbleTimeout = TimeSpan.FromSeconds(2);
            //_recognizer.EndSilenceTimeout = TimeSpan.FromSeconds(1);
            //_recognizer.EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(1.5);

            _recognizer.SpeechRecognized += _SpeechRecognitionRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _SpeechRecognitionRejected; // if recognized speech is rejected, call the specified method
            _recognizer.RecognizeCompleted += _RecognizeCompleted; // if recognized speech is rejected, call the specified method
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Single); // recognize speech asynchronous

        }

        private void StartRecognition()
        {
            Console.WriteLine();
            Console.WriteLine("Starting Recognition...");
            _recognizer.RecognizeAsync(RecognizeMode.Single); // recognize speech asynchronous
            
        }

        private void Beep()
        {
            int freq = 3000;
            int duration = 200;
            Console.Beep(freq, duration);
        }

        private static void Wait(int second)
        {
            //* timer to wait 
            for (int i = 0; i < second; i++)
            {
                // Sixty seconds is one minute.
                Thread.Sleep(1000);
            }
            //*/
        }

        private void _SpeechRecognitionRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var user = ">> User :";

            Console.WriteLine("{0} {1}", user, e.Result.Text);
            Program.VoiceText(e.Result.Text);

            Beep();
            Wait(3);
            StartRecognition();
        }
        private void _SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech rejected. Did you mean:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
            Program.VoiceText("I do not know what you mean.");
            Beep();
            Wait(3);
            StartRecognition();

        }

        static void _RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            Console.WriteLine("Speech Completed:");
            Program.VoiceText("<completed>");

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
            Console.WriteLine(">> User : " + e.Result.Text);
            Program.VoiceText(e.Result.Text);
            Beep();
        }
        
        #endregion

    }
}
