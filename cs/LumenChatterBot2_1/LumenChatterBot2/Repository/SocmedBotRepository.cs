using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LumenChatterBot2.Repository
{
    public class SocmedBotRepository 
    {
        private static string userInput = "";
        private static string userPrevInput = "";
        private static string botResponse = "";
        private static bool isExit = false;
        private static bool isResponRepeated = false;
        private static int botResponseId = 0;
        private static List<int> responHistoryList = new List<int>();

        public static string StartOnline()
        {
            botResponse = "OGNAK GNOUK..., I am life. :D";
            return botResponse;
        }

        public static string ConversationInput(string textInput)
        {
            userInput = textInput;
            if (userInput == "bye")
            {
                isExit = true;
                Console.ForegroundColor = ConsoleColor.Red;
                botResponse = "It was nice talking to you, see you next time.";
            }
            else if (userInput == userPrevInput && userInput.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                botResponse = "You are repeating yourself.";
            }
            else
            {
                botResponse = ChatterBotRepository.FindMatchKeyWord(userInput, responHistoryList, out botResponseId, out isResponRepeated);
                if (isResponRepeated && botResponse != "")
                {
                    botResponse += "You have been ask about it.";
                }
                else if (isResponRepeated && botResponse == "")
                {
                    botResponse = "You know the answer.";
                }
                else
                {
                    if (botResponse == "")
                    {
                        botResponse = "I'm not sure if I understand what you are talking about.";

                    }
                }

                /* timer to wait 
                int second = 2;
                for (int i = 0; i < second; i++)
                {
                    // Sixty seconds is one minute.
                    Thread.Sleep(1000);
                }
                //*/

            }

            return botResponse;
        }
    }
}
