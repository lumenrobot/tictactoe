using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Aldebaran.Proxies;

namespace naoTest
{
    class Program
    {
        static void Main(string[] args)
        {
            bool guessed = false;
            // used to determine if the user has guessed the right number 

            int wordAsInt = -1;
            // used to translate recongized word in to an integer value 

            List<string> words = new List<string>();
            // create list of words to sent to Nao bot 

            // Create a connection to the text to speech engine on the Nao bot 
            TextToSpeechProxy tts = new TextToSpeechProxy("192.168.0.101", 9559);

            // Create a connection to the speech recognition on the Nao bot 
            SpeechRecognitionProxy speechR = new SpeechRecognitionProxy("192.168.0.101",9559);

            // create connection to robot memory 
            MemoryProxy m = new MemoryProxy("192.168.0.101", 9559);

            // random number generator 
            Random rnd = new Random();

            // generates number between 1‐5 
            int rndNum = rnd.Next(6);


            // check for rndNum being 0 
            if (rndNum == 0)
            {
                wordAsInt++;
            }

            // add words we want recognized to word list 
            words.Add("one");
            words.Add("two");
            words.Add("three");
            words.Add("four");
            words.Add("five");

            speechR.setVocabulary(words, false); // send the word list to robot 


            Console.WriteLine("Guessing game running on NAO");

            // loop until number is guessed 
            while (!guessed)
            {
                // user instructions 
                tts.say("I have picked a number between one and five, try to guess it");

                System.Threading.Thread.Sleep(1500); // wait 1.5 seconds 

                speechR.subscribe("Main", 50, 50); // Start speech recognition engine 

                System.Threading.Thread.Sleep(5000); // wait 5 seconds 

                speechR.unsubscribe("Main"); // stop speech recognition engine 

                // get lastwordrecognized from memory 
                object wordObj = m.getData("LastWordRecognized");
                string word = (string)((ArrayList)wordObj)[0];

                // convert word to a integer 
                switch (word)
                {
                    case "one":
                        wordAsInt = 1;
                        break;
                    case "two":
                        wordAsInt = 2;
                        break;
                    case "three":
                        wordAsInt = 3;
                        break;
                    case "four":
                        wordAsInt = 4;
                        break;
                    case "five":
                        wordAsInt = 5;
                        break;
                    default:
                        wordAsInt = -1;
                        break;
                }

                // if else block to determine if user guessed too high, too low, or                    correctly 
                if (wordAsInt > rndNum)
                {
                    tts.say("You guessed too high");
                }
                else if (wordAsInt == rndNum)
                {
                    tts.say("You guessed correctly!");
                    guessed = true;
                }
                else if (wordAsInt < rndNum)
                {
                    tts.say("You guessed too low");
                }

                // debug output 
                Console.WriteLine("/nNumber guessed was ");
                Console.Write(word);
                Console.WriteLine("/n Actual number is ");
                Console.Write(rndNum);
            }
        }
    }
}