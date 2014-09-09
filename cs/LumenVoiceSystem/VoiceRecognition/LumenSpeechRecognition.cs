using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.Threading;

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

        private void Beep()
        {
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
            var user = ">> User :";
            if (e.Result.Text == "hello")
            {
                Console.WriteLine("{0} {1}",user,e.Result.Text);
                Program.VoiceText(e.Result.Text);
            }
            
            else if (e.Result.Text == "yes")
            {
                Console.WriteLine("{0} {1}", user, e.Result.Text);
                Program.VoiceText(e.Result.Text);
            }
            else if (e.Result.Text == "no")
            {
                Console.WriteLine("{0} {1}", user, e.Result.Text);
                Program.VoiceText(e.Result.Text);
            }
            else if (e.Result.Text == "intro")
            {
                Console.WriteLine("{0} {1}", user, e.Result.Text);
                Program.VoiceText(e.Result.Text);
            }

            Beep();

        }
        static void _SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech rejected. Did you mean:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
            Program.VoiceText("<rejected>");

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
