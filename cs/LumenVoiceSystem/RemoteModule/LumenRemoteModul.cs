using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Aldebaran.Proxies;

namespace RemoteModule
{
    public class LumenRemoteModul
    {
        private static string ip;
        private static string eventName;
        private static string speechKey;

        private static int port;
        private static int eventValue;

        private static bool simMode;

        private static TextToSpeechProxy tts;
        private static MemoryProxy memory;


        public LumenRemoteModul()
        {
            var mode = ConfigurationManager.AppSettings["SimMode"];
            if(mode=="true")
            {
                simMode = true;
                ip = ConfigurationManager.AppSettings["Local"];
            }
            else
            {
                simMode = false;
                ip = ConfigurationManager.AppSettings["IP"];
            }

            port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            eventName = ConfigurationManager.AppSettings["EventName"];
            speechKey = ConfigurationManager.AppSettings["SpeechKey"];
            LumenReady();
        }

        void LumenReady()
        {
            try
            {
                RobotPostureProxy rb = new RobotPostureProxy(ip,port);
                float speed = 1.0f;
                var postureInit = ConfigurationManager.AppSettings["PostureInit"];
                var success = rb.goToPosture(postureInit, speed);
                if(simMode)
                {
                    tts=new TextToSpeechProxy(ip,port);
                }
                {
                    memory = new MemoryProxy(ip, port);
                }

                if(success)
                {
                    Console.WriteLine("======= Lumen Ready ========");
                }
                else
                {
                    Console.WriteLine("======= <ERROR> Lumen Initilization Fail ======== ");
                }
                Console.WriteLine();

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void TTS(string text,int val)
        {
            var message = text;
            eventValue = val;
            try
            {
                if(simMode)
                {
                    tts.say(text);
                }
                else
                {
                    memory.declareEvent(eventName);
                    memory.raiseEvent(eventName,eventValue);
                    memory.insertData(speechKey,text);
                }
            }
            catch (Exception)
            {

                message = "<Error>";
            }
            Console.WriteLine(">> Lumen : {0}", message);
        }

    }
}
