using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using Aldebaran.Proxies;

namespace LumenWebControl.Models
{
    public class MotionRepository
    {
        private static string ip;
        private static int port;
        private static string text;

        public MotionRepository(string ipParam, int portParam, string textParam)
        {
            ip = ipParam;
            port = portParam;
            text = textParam;
        }

        public void Speak()
        {
            TextToSpeechRepository.TextToSpeech(ip,port,text);
        }

        public void Stand()
        {
            try
            {
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                MotionProxy motion = new MotionProxy(ip,port);
                motion.stiffnessInterpolation("Body",1.0f,1.0f);

                float speed = 1.0f;
                var success = rb.goToPosture("Stand", speed);

                if (!success)
                {
                    TextToSpeechRepository.TextToSpeech(ip, port, "Sorry, I am unable to stand.");
                }
                else
                {
                    motion.stiffnessInterpolation("Body",0.6f,1.0f);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void StandZero()
        {
            try
            {
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                MotionProxy motion = new MotionProxy(ip, port);
                motion.stiffnessInterpolation("Body", 1.0f, 1.0f);

                float speed = 1.0f;
                var success = rb.goToPosture("StandZero", speed);

                if (!success)
                {
                    TextToSpeechRepository.TextToSpeech(ip, port, "Sorry, I am unable to Stand Zero.");
                }
                else
                {
                    motion.stiffnessInterpolation("Body", 0.6f, 1.0f);
                    
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void StandInit()
        {
            try
            {
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);

                float speed = 1.0f;
                var success = rb.goToPosture("StandInit", speed);

                if (!success)
                {
                    TextToSpeechRepository.TextToSpeech(ip, port, "Sorry, I am unable to Stand init.");
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            

        }

        public void Sit()
        {
            try
            {
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                MotionProxy motion = new MotionProxy(ip, port);
                motion.stiffnessInterpolation("Body", 1.0f, 1.0f);
                float speed = 1.0f;
                var success = rb.goToPosture("Sit", speed);

                if (!success)
                {
                    TextToSpeechRepository.TextToSpeech(ip, port, "Sorry, I am unable to sit.");
                }
                else
                {
                    motion.stiffnessInterpolation("Body",0.0f,1.0f);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void SitRelax()
        {
            try
            {
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                MotionProxy motion = new MotionProxy(ip, port);
                motion.stiffnessInterpolation("Body", 1.0f, 1.0f);
                float speed = 1.0f;
                var success = rb.goToPosture("SitRelax", speed);

                if (!success)
                {
                    TextToSpeechRepository.TextToSpeech(ip, port, "Sorry, I am unable to sit relax.");
                }
                else
                {
                    motion.stiffnessInterpolation("Body",0.0f,1.0f);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void Crouch()
        {
            try
            {
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                MotionProxy motion = new MotionProxy(ip, port);
                motion.stiffnessInterpolation("Body", 1.0f, 1.0f);
                float speed = 1.0f;
                var success = rb.goToPosture("Crouch", speed);

                if (!success)
                {
                    TextToSpeechRepository.TextToSpeech(ip, port, "Sorry, I am unable to crouch.");
                }
                else
                {
                    motion.stiffnessInterpolation("Body",0.0f,1.0f);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public void LyingBelly()
        {
            try
            {
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);

                float speed = 1.0f;
                var success = rb.goToPosture("LyingBelly", speed);

                if (!success)
                {
                    TextToSpeechRepository.TextToSpeech(ip, port, "Sorry, I am unable to LyingBelly.");
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public void LyingBack()
        {
            try
            {
                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                MotionProxy motion = new MotionProxy(ip, port);
                motion.stiffnessInterpolation("Body", 1.0f, 1.0f);
                float speed = 1.0f;
                var success = rb.goToPosture("LyingBack", speed);

                if (!success)
                {
                    TextToSpeechRepository.TextToSpeech(ip, port, "Sorry, I am unable to LyingBack.");
                }
                else
                {
                    motion.stiffnessInterpolation("Body",0.0,1.0);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error Report");
            }
        }

        

        public void SafeStiffnessOff()
        {
            try
            {
                MotionProxy motion = new MotionProxy(ip, port);
                motion.stiffnessInterpolation("Body", 1.0f, 1.0f);

                var isWakeUp = motion.robotIsWakeUp();

                RobotPostureProxy rb = new RobotPostureProxy(ip, port);
                float speed = 1.0f;

                if (isWakeUp)
                {

                    var success = rb.goToPosture("Crouch", speed);
                    if (success)
                    {
                        motion.stiffnessInterpolation("Body",0.0f,1.0f);
                        motion.rest();
                        TextToSpeechRepository.TextToSpeech(ip, port, "Stiffness off.");
                    }
                }
                else
                {
                    var success = rb.goToPosture("Crouch", speed);
                    motion.stiffnessInterpolation("Body", 0.0f,1.0f);
                    motion.rest();
                    TextToSpeechRepository.TextToSpeech(ip, port, "Body stiffness off.");
                }

                //var summary = motion.getSummary();
                //MessageBox.Show(summary, "Summary");
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

    }
}