using System;
using System.Collections.Generic;
using System.Linq;
using LumenChatterBot2.Repository;

namespace LumenChatterBot2
{
    class Program
    {
        static void Main(string[] args)
        {

            string userInput = "";
            string userPrevInput = "";
            string botResponse = "";
            bool isExit = false;
            bool isResponRepeated = false;
            int botResponseId = 0;
            List<int> responHistoryList = new List<int>();

            while (!isExit)
            {
                userPrevInput = userInput;

                if(botResponseId != 0)
                {
                    responHistoryList.Add(botResponseId);
                }

                Console.Write("You : ");

                userInput = Console.ReadLine();
                userInput = ChatterBotRepository.PrepareInput(userInput);

                if (userInput == "bye")
                {
                    isExit = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    botResponse = "It was nice talking to you, see you next time.";
                }
                else if(userInput==userPrevInput && userInput.Any())
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
                    else if(isResponRepeated && botResponse=="")
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
                    
                    Console.ForegroundColor = ConsoleColor.Red;

                    /* timer to wait 
                    int second = 2;
                    for (int i = 0; i < second; i++)
                    {
                        // Sixty seconds is one minute.
                        Thread.Sleep(1000);
                    }
                    //*/

                }


                Console.WriteLine("Lumen : " + botResponse);
                Console.Beep();
                Console.ResetColor();
                Console.WriteLine("");
                
            }

        }
    }
}
