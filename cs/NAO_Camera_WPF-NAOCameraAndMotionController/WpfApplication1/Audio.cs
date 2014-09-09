/** 
 * This software was developed by Austin Hughes 
 * Last Modified: 2013‐06‐11 
 */ 
 
using System; 
using System.Collections; 
using System.IO; 
using System.Windows; 
using System.ComponentModel; 
using Aldebaran.Proxies; 
using WinSCP; 
 
namespace NAO_Camera_WPF 
{ 
    class Audio 
    { 
        // Needed proxies 
        private AudioDeviceProxy audio = null; 
        private TextToSpeechProxy tts = null; 
 
        // Worker to download files in the background 
        private BackgroundWorker bgWorker = new BackgroundWorker(); 
 
        // Variables 
        private string ipString = ""; 
 
        /// <summary> 
        /// Connects to the NAO robot 
        /// </summary> 
        /// <param name="ip"> ip address of the robot </param> 
        public void connect(string ip) 
        { 
            // if audio or tts is not null it then was not properly disconnected 
            if (audio != null || tts != null) 
            { 
                Disconnect(); 
            } 
 
            // attempt to connect 
            try 
            { 
                ipString = ip; 
                audio = new AudioDeviceProxy(ip, 9559); 
                tts = new TextToSpeechProxy(ip, 9559); 
            } 
            catch (Exception e) 
            { 
                // display error message and write exceptions to a file 
                MessageBox.Show("Exception occurred, error log in C:\\NAOcam\\exception.txt"); 
                System.IO.File.WriteAllText(@"C:\\NAOcam\\exception.txt", 
e.ToString()); 
            } 
        } 
 
        /// <summary> 
        /// Resets the audio connection 
        /// </summary> 
        public void Disconnect() 
        { 
            stopRecording(); 
            tts = null; 
            audio = null; 
        } 
 
        /// <summary> 
        /// Starts recording audio 
        /// </summary> 
        public void record() 
        { 
            // if connection was successful start recording audio 
            if (audio != null) 
            { 
                audio.startMicrophonesRecording("/home/nao/temp.ogg"); 
            } 
        } 
 
        /// <summary> 
        /// Stops recording audio and saves the file 
        /// to the local machine 
        /// </summary> 
        public void stopRecording() 
        { 
            // try to disconnect from audio and stop microphones from recording 
            try 
            { 
 
                // make sure audio proxy is not null 
                if (audio != null) 
                { 
                    // stop recording and set audio proxy to null 
                    audio.stopMicrophonesRecording(); 
                    audio = null; 
 
                    // Set up the Background Worker Events 
                    bgWorker.DoWork += bgWorker_DoWork; 
                    bgWorker.RunWorkerCompleted += bgWorker_WorkerCompleted; 
 
                    // Run the Background Worker 
                    bgWorker.RunWorkerAsync(); 
                } 
 
            } 
            catch (Exception e) 
            { 
                // display error message and write exceptions to a file 
                MessageBox.Show("Exception occurred, error log in C:\\NAOcam\\exception.txt"); 
                System.IO.File.WriteAllText(@"C:\\NAOcam\\exception.txt", 
e.ToString()); 
            } 
        } 
 
        /// <summary> 
        /// Sends a string to the text to speech engine on the robot 
        /// </summary> 
        /// <param name="sentence"> the phrase to be spoken </param> 
        public void talk(string sentence) 
        { 
            if (tts != null) 
            { 
                tts.say(sentence); 
                MessageBox.Show("Message sent successfully!"); 
            } 
        } 
 
        /// <summary> 
        /// Downloads audio from the robot  
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        void bgWorker_DoWork(object sender, DoWorkEventArgs e) 
        { 
            // Setup session options 
            SessionOptions sessionOptions = new SessionOptions 
            { 
                Protocol = Protocol.Sftp, 
                HostName = ipString, 
                UserName = "nao", 
                Password = "nao", 
                SshHostKeyFingerprint = "ssh‐rsa 20487c:48:34:e3:c0:7a:92:8e:2f:95:79:0e:12:69:79:e7", 
            }; 
 
            using (Session session = new Session()) 
            { 
                // tell library path to the winSCP executable 
                session.ExecutablePath = System.IO.Directory.GetCurrentDirectory() 
+ "\\winSCPexe.exe"; 
       
                // Connect 
                session.Open(sessionOptions); 
 
                //Set up transfer 
                TransferOptions transferOptions = new TransferOptions(); 
                transferOptions.TransferMode = TransferMode.Binary; 
 
                // generate a file based on date and time 
                string time = Convert.ToString(DateTime.UtcNow.ToFileTime()); 
 
                string destination = "C:\\NAOcam\\" + "NAO_Audio_" + time + ".ogg"; 
 
                // download files 
                TransferOperationResult transferResult; 
                transferResult = session.GetFiles("/home/nao/temp.ogg", 
@destination, true, transferOptions); 
 
                // Throw on any error 
                transferResult.Check(); 
            } 
        } 
 
        /// <summary> 
        /// Called when bgWorker finishes its work. 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        void bgWorker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs 
e) 
        { 
            if (e.Cancelled) 
            { 
                MessageBox.Show("Cancelled"); 
            } 
            else if (e.Error != null) 
            { 
                MessageBox.Show("Error"); 
            } 
            else 
            { 
                // feeback to user 
                MessageBox.Show("Audio saved in C:\\NAOcam"); 
            } 
        } 
    } 
} 
 