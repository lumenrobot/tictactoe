using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;


namespace VoiceSampleCustom
{
    class Program
    {
        #region Custom Voice Recog
        static SpeechRecognitionEngine _speechRecognitionEngine;

        public static void Main()
        {
            _speechRecognitionEngine = new SpeechRecognitionEngine();

            _speechRecognitionEngine.SetInputToDefaultAudioDevice();

            //use dictation grammar from windows
            //DictationGrammar _dictationGrammar = new DictationGrammar();
            //_speechRecognitionEngine.LoadGrammar(_dictationGrammar);

            //use our defined grammar
            _speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder("hello"))); // load a "test" grammar
            //_speechRecognitionEngine.RequestRecognizerUpdate(); // request for recognizer update
            _speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder("intro"))); // load a "test" grammar
            //_speechRecognitionEngine.RequestRecognizerUpdate(); // request for recognizer update
            _speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder("yes"))); // load a "test" grammar
            //_speechRecognitionEngine.RequestRecognizerUpdate(); // request for recognizer update
            _speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder("no"))); // load a "exit" grammar
            //_speechRecognitionEngine.RequestRecognizerUpdate(); // request for recognizer update

            //_speechRecognitionEngine.InitialSilenceTimeout = TimeSpan.FromSeconds(3);
            //_speechRecognitionEngine.BabbleTimeout = TimeSpan.FromSeconds(2);
            //_speechRecognitionEngine.EndSilenceTimeout = TimeSpan.FromSeconds(1);
            //_speechRecognitionEngine.EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(1.5);

            _speechRecognitionEngine.SpeechRecognized -= SpeechRecognized;
            _speechRecognitionEngine.SpeechHypothesized -= SpeechHypothesizing;
            _speechRecognitionEngine.SpeechRecognitionRejected -= SpeechRecognitionRejected;
            //_speechRecognitionEngine.RecognizeCompleted -= RecognizeCompleted;

            _speechRecognitionEngine.SpeechRecognized += SpeechRecognized;
            _speechRecognitionEngine.SpeechHypothesized += SpeechHypothesizing;
            _speechRecognitionEngine.SpeechRecognitionRejected += SpeechRecognitionRejected;
            //_speechRecognitionEngine.RecognizeCompleted += RecognizeCompleted;


            _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Single);

            Console.WriteLine("Press any key to exit...");
            //beginRecognition();
            var a = Console.ReadKey();

        }

        private static void beginRecognition()
        {
            Console.WriteLine();
            Console.WriteLine("Recognizing...");
            _speechRecognitionEngine.Recognize();

        }

        private static void initRecognition()
        {
            Console.WriteLine("initializing");
            _speechRecognitionEngine = new SpeechRecognitionEngine();

            _speechRecognitionEngine.SetInputToDefaultAudioDevice();

            //use dictation grammar from windows
            //DictationGrammar _dictationGrammar = new DictationGrammar();
            //_speechRecognitionEngine.LoadGrammar(_dictationGrammar);

            //use our defined grammar
            _speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder("hello"))); // load a "test" grammar
            //_speechRecognitionEngine.RequestRecognizerUpdate(); // request for recognizer update
            _speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder("intro"))); // load a "test" grammar
            //_speechRecognitionEngine.RequestRecognizerUpdate(); // request for recognizer update
            _speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder("yes"))); // load a "test" grammar
            //_speechRecognitionEngine.RequestRecognizerUpdate(); // request for recognizer update
            _speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder("no"))); // load a "exit" grammar
            //_speechRecognitionEngine.RequestRecognizerUpdate(); // request for recognizer update

            _speechRecognitionEngine.InitialSilenceTimeout = TimeSpan.FromSeconds(3);
            _speechRecognitionEngine.BabbleTimeout = TimeSpan.FromSeconds(2);
            _speechRecognitionEngine.EndSilenceTimeout = TimeSpan.FromSeconds(1);
            _speechRecognitionEngine.EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(1.5);

            _speechRecognitionEngine.SpeechRecognized -= SpeechRecognized;
            _speechRecognitionEngine.SpeechHypothesized -= SpeechHypothesizing;
            _speechRecognitionEngine.SpeechRecognitionRejected -= SpeechRecognitionRejected;
            _speechRecognitionEngine.RecognizeCompleted -= RecognizeCompleted;

            _speechRecognitionEngine.SpeechRecognized += SpeechRecognized;
            _speechRecognitionEngine.SpeechHypothesized += SpeechHypothesizing;
            _speechRecognitionEngine.SpeechRecognitionRejected += SpeechRecognitionRejected;
            _speechRecognitionEngine.RecognizeCompleted += RecognizeCompleted;
            beginRecognition();
        }

        private static void SpeechHypothesizing(object sender, SpeechHypothesizedEventArgs e)
        {
            ///real-time results from the engine
            string realTimeResults = e.Result.Text;
            Console.WriteLine("Hypothesizing : {0}", realTimeResults);

        }

        private static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            
            ///final answer from the engine
            string finalAnswer = e.Result.Text;
            Console.WriteLine("Recognized : {0}", finalAnswer);
            Beep();
            Wait(5);
            //initRecognition();
            Console.WriteLine();
            Console.WriteLine("Init...");
            _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Single);
        }

        private static void SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("SpeechRecognitionRejected : ");
            Beep();
            Wait(5);
            //initRecognition();
            Console.WriteLine();
            Console.WriteLine("Init...");
            _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Single);


        }

        private static void RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            Console.WriteLine("RecognizeCompleted : ");
            Console.WriteLine();
        }

        private static void Beep()
        {
            int freq = 3000;
            int duration = 200;
            Console.Beep(freq, duration);
        }

        private static void Wait(int second)
        {
            for (int i = 0; i < second; i++)
            {
                // Sixty seconds is one minute.
                Thread.Sleep(1000);
            }
        }
        //*/
        #endregion
    }
}
