using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aldebaran.Proxies;

namespace LumenWebControl.Models
{
    public class TextToSpeechRepository
    {

        public static void TextToSpeech (string ip, int port, string text)
        {
            TextToSpeechProxy tts = new TextToSpeechProxy(ip, port);
            tts.say(text);
        }

    }
}