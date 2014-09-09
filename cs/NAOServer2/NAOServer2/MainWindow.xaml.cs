/** 
 * This software was developed by Austin Hughes 
 * Last Modified: 2013‐07‐16 
 */ 
 
using System; 
using System.IO; 
using System.Windows; 
using System.Windows.Media; 
using System.Windows.Media.Imaging; 
using System.Windows.Threading; 
using System.Collections.Generic; 
 
using SuperWebSocket; 
using Aldebaran; 
using NAO_Camera_WPF; 
using SuperSocket.SocketBase; 
 
namespace NAOserver 
{ 
    /// <summary> 
    /// Interaction logic for MainWindow.xaml 
    /// </summary> 
    public partial class MainWindow : Window 
    { 
        // Classes 
        private Camera naoCam = null; 
        private static Motion naoMotion = null;
        private static Audio naoAudio = null;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer(); 
        private DispatcherTimer recordingTimer = new DispatcherTimer(); 
        private WebSocketServer appServer = null;
        
        // Variables 
        private string curDir = Directory.GetCurrentDirectory();
        private const int COLOR_SPACE = 13; 
        private const int FPS = 30; 
        private bool isCamInitialized; 
        private bool isPictureUpdating = false;
        private bool isNaoConnect = false;
        private bool isAppServerStart = false;
        private NaoCamImageFormat currentFormat; 
        private BitmapSource imageBitmap; 
        private BitmapFrame frame; 
        private static String imageString = ""; 
        private static List<WebSocketSession> sessionList = new List<WebSocketSession>(); 
        private static float yaw; 
        private static float pitch; 
 
        /// <summary> 
        /// constuctor for MainWindow 
        /// </summary> 
        public MainWindow() 
        { 
            InitializeComponent(); 
 
            // call the Camera constuctor, and set the image format to 640x480 
            naoCam = new Camera(); 
 
            // call the Motion constructor 
            naoMotion = new Motion();

            // call the Audio constructor 
            naoAudio = new Audio(); 
 
            currentFormat = naoCam.NaoCamImageFormats[2]; 
 
            // Make sure the standard output directory exists 
            if (!System.IO.Directory.Exists("C:\\NAOserver\\")) 
            { 
                System.IO.Directory.CreateDirectory("C:\\NAOserver\\"); 
            } 
        } 
 
        /// <summary> 
        /// called when the window closes 
        /// </summary> 
        /// <param name="sender"> object that created the event </param> 
        /// <param name="e"> any addtional arguments </param> 
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) 
        { 
            if(appServer!=null)
            appServer.Stop(); // stop the websocket 
            
            try
            {
                // disconnect from camera and stop the timer 
                naoCam.Disconnect();
                naoMotion.Disconnect();
                naoAudio.Disconnect();
                dispatcherTimer.Stop();
            }
            catch (Exception ex)
            {
                isCamInitialized = false;

                // display error message and write exceptions to a file 
                MessageBox.Show("Exception occurred in closing, error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", ex.ToString());
            }
        } 
 
        /// <summary> 
        /// connect to the NAO robot 
        /// </summary> 
        /// <param name="sender"> object that created event </param> 
        /// <param name="e"> any addional methods </param> 
        private void connectButton_Click(object sender, RoutedEventArgs e) 
        { 
            appServer = new WebSocketServer(); 
 
            //Setup the websocket 
            if (!appServer.Setup(2012)) //Setup with listening port 
            { 
                MessageBox.Show("Failed to setup!"); 
                this.Close(); 
            } else
                MessageBox.Show("Setup Success"); 

            //Try to start the websocket 
            if (!appServer.Start())
            {
                MessageBox.Show("Failed to start!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Start Success");
                isAppServerStart = true;
            }
            naoConnect();
 
            // event handlers for websocket events 
            appServer.NewMessageReceived += new SessionHandler<WebSocketSession,string>(appServer_NewMessageReceived); 
            appServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(appServer_sessionClosed); 
            try
            {
                // Create a timer for event based frame acquisition.  
                // Program will get new frame from storage based on FPS 
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)Math.Ceiling(1000.0 / 30)); 
                dispatcherTimer.Start(); 
 
                // whenever the timer ticks the bitmapReady event is called 
                dispatcherTimer.Tick += new EventHandler(bitmapReady); 
 
                // let rest of program know that camera is ready 
                isCamInitialized = true;
                if (isNaoConnect == true)
                    MessageBox.Show("Nao Connected");
                else
                    MessageBox.Show("NAO not Connected, sending dummy image");
            }
            catch (Exception ex)
            {
                isCamInitialized = false;

                // display error message and write exceptions to a file 
                MessageBox.Show("Exception occurred in dispatcher, error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", ex.ToString());
            }

        } 
 
        /// <summary> 
        /// disconnects from the NAO robot 
        /// </summary> 
        /// <param name="sender"> object that created the event </param> 
        /// <param name="e"> any additional arguments </param> 
        private void disconnectButton_Click(object sender, RoutedEventArgs e) 
        { 
            // disconnect from camera and stop the timer 
            dispatcherTimer.Stop();
            try
            {
                naoAudio.Disconnect();
                naoCam.Disconnect();
            }
            catch (Exception ex)
            {
                // display error message and write exceptions to a file 
                MessageBox.Show("Exception occurred in disconnection, error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", ex.ToString());
            }
             
            if(appServer!=null)
            appServer.Stop(); 
            MessageBox.Show("Disconnected"); 
        } 
 
        /// <summary> 
        /// Sends a new jpg through the websocket 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void bitmapReady(object sender, EventArgs e) 
        {
            byte[] imageBytes=null;
            // check for websocket sessions if none exist nothing needs to be done 
            if (sessionList.Count > 0)  
            { 
                if (isCamInitialized && !isPictureUpdating) 
                { 
                    isPictureUpdating = true; // picture is being updated 
 
                    try // try to get a new image 
                    {
                        if (isNaoConnect)
                        {
                            try
                            {
                                imageBytes = naoCam.getImage(); // store an image in imageBytes 
                            }
                            catch (Exception ex)
                            {
                                // display error message and write exceptions to a file 
                                MessageBox.Show("Exception occurred getting image from Nao, error log in C:\\NAOserver\\exception.txt");
                                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", ex.ToString());
                            }
                        }
                        else
                        {
                            imageBytes = null;
                        }
                        if (imageBytes != null) // if the image isnt empty create a bitmap and send via websocket 
                            imageBitmap = BitmapSource.Create(currentFormat.width, currentFormat.height, 96, 96, PixelFormats.Bgr24, BitmapPalettes.WebPalette, imageBytes, currentFormat.width * 3); 
                        else
                            imageBitmap = new BitmapImage(new Uri(String.Format("{0}/Petrus.jpg", curDir)));
                        frame = BitmapFrame.Create(imageBitmap); 
 
                        // converts bitmap frames to jpg 
                        JpegBitmapEncoder converter = new JpegBitmapEncoder(); 
 
                        converter.Frames.Add(frame); 
 
                        // memory stream to save jpg to byte array 
                        MemoryStream ms = new MemoryStream(); 
 
                        converter.Save(ms); 
 
                        ms.Close(); 
 
                        byte[] bytes = ms.ToArray(); 
                             
                        // since html can convert base64strings to images, convert the image to a base64 string 
                        imageString = Convert.ToBase64String(bytes); 
 
                        // send it to all connected sessions 
                        for (int x = 0; x < sessionList.Count; x++) 
                        { 
                            sessionList[x].Send(imageString); 
                        } 
                    }
                    catch (Exception e1) 
                    { 
                        // display error message and write exceptions to a file 
                        MessageBox.Show("Exception occurred in bitmap ready, error log in C:\\NAOserver\\exception.txt. Error: " + e1.ToString()); 
                        System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", e1.ToString()); 
                    } 
                } 
            } 
            isPictureUpdating = false; // picture is updated 
        } 
 
        /// <summary> 
        /// Event handler for new messages recieved via websocket 
        /// </summary> 
        /// <param name="session"> the session that sent a message </param> 
        /// <param name="message"> the message sent </param> 
        static void appServer_NewMessageReceived(WebSocketSession session, string message) 
        { 
            // if start was sent, add the session to the session list 
            if (message == "start") 
            { 
                sessionList.Add(session); 
            } 
 
            // move the robots head in the desired direction 
            if (message == "left") 
            { 
                yaw = naoMotion.getAngle("HeadYaw"); 
                naoMotion.moveJoint(yaw + .25f, "HeadYaw"); 
            } 
            if (message == "right") 
            { 
                yaw = naoMotion.getAngle("HeadYaw"); 
                naoMotion.moveJoint(yaw - .25f, "HeadYaw"); 
            } 
            if (message == "up") 
            { 
                pitch = naoMotion.getAngle("HeadPitch"); 
                naoMotion.moveJoint(pitch - .1f, "HeadPitch"); 
            } 
            if (message == "down") 
            { 
                pitch = naoMotion.getAngle("HeadPitch"); 
                naoMotion.moveJoint(pitch + .1f, "HeadPitch"); 
            }
            if (message == "leftElbowYawUp")
            {
                pitch = naoMotion.getAngle("LElbowYaw");
                naoMotion.moveJoint(pitch + .1f, "LElbowYaw");
            }
            if (message == "leftElbowYawDown")
            {
                pitch = naoMotion.getAngle("LElbowYaw");
                naoMotion.moveJoint(pitch - .1f, "LElbowYaw");
            }
            if (message == "RShoulderPitchUp")
            {
                pitch = naoMotion.getAngle("RShoulderPitch");
                naoMotion.moveJoint(pitch + .1f, "RShoulderPitch");
            }
            if (message == "RShoulderPitchDown")
            {
                pitch = naoMotion.getAngle("RShoulderPitch");
                naoMotion.moveJoint(pitch - .1f, "RShoulderPitch");
            }
            if (message == "LShoulderPitchUp")
            {
                pitch = naoMotion.getAngle("LShoulderPitch");
                naoMotion.moveJoint(pitch + .1f, "LShoulderPitch");
            }
            if (message == "LShoulderPitchDown")
            {
                pitch = naoMotion.getAngle("LShoulderPitch");
                naoMotion.moveJoint(pitch - .1f, "LShoulderPitch");
            }
            if (message == "RShoulderRollUp")
            {
                pitch = naoMotion.getAngle("RShoulderRoll");
                naoMotion.moveJoint(pitch + .1f, "RShoulderRoll");
            }
            if (message == "RShoulderRollDown")
            {
                pitch = naoMotion.getAngle("RShoulderRoll");
                naoMotion.moveJoint(pitch - .1f, "RShoulderRoll");
            }
            if (message == "LShoulderRollUp")
            {
                pitch = naoMotion.getAngle("LShoulderRoll");
                naoMotion.moveJoint(pitch + .1f, "LShoulderRoll");
            }
            if (message == "LShoulderRollDown")
            {
                pitch = naoMotion.getAngle("LShoulderRoll");
                naoMotion.moveJoint(pitch - .1f, "LShoulderRoll");
            }
            if (message == "RElbowYawUp")
            {
                pitch = naoMotion.getAngle("RElbowYaw");
                naoMotion.moveJoint(pitch + .1f, "RElbowYaw");
            }
            if (message == "RElbowYawDown")
            {
                pitch = naoMotion.getAngle("RElbowYaw");
                naoMotion.moveJoint(pitch - .1f, "RElbowYaw");
            }
            if (message == "RElbowRollUp")
            {
                pitch = naoMotion.getAngle("RElbowRoll");
                naoMotion.moveJoint(pitch + .1f, "RElbowRoll");
            }
            if (message == "RElbowRollDown")
            {
                pitch = naoMotion.getAngle("RElbowRoll");
                naoMotion.moveJoint(pitch - .1f, "RElbowRoll");
            }
            if (message == "LElbowRollUp")
            {
                pitch = naoMotion.getAngle("LElbowRoll");
                naoMotion.moveJoint(pitch + .1f, "LElbowRoll");
            }
            if (message == "LElbowRollDown")
            {
                pitch = naoMotion.getAngle("LElbowRoll");
                naoMotion.moveJoint(pitch - .1f, "LElbowRoll");
            }
            if (message == "RWristYawUp")
            {
                pitch = naoMotion.getAngle("RWristYaw");
                naoMotion.moveJoint(pitch + .1f, "RWristYaw");
            }
            if (message == "RWristYawDown")
            {
                pitch = naoMotion.getAngle("RWristYaw");
                naoMotion.moveJoint(pitch - .1f, "RWristYaw");
            }
            if (message == "LWristYawUp")
            {
                pitch = naoMotion.getAngle("LWristYaw");
                naoMotion.moveJoint(pitch + .1f, "LWristYaw");
            }
            if (message == "LWristYawDown")
            {
                pitch = naoMotion.getAngle("LWristYaw");
                naoMotion.moveJoint(pitch - .1f, "LWristYaw");
            }
          
        }
        private void naoConnect()
        {
            try // attempt to connect to the camera and motion system 
            {
                // connect to the NAO Motion API 
                naoCam.connect(ipBox.Text, currentFormat, COLOR_SPACE, FPS);
                if (naoCam.naoCamera == null)
                    return;
                naoMotion.connect(ipBox.Text);
                naoAudio.connect(ipBox.Text);

                isNaoConnect = true;
            }
            catch (Exception ex)
            {
                isCamInitialized = false;

                // display error message and write exceptions to a file 
                MessageBox.Show("Nao Connection error, but application can be continued. error log in C:\\NAOserver\\exception.txt");
                System.IO.File.WriteAllText(@"C:\\NAOserver\\exception.txt", ex.ToString());
            }
        }
 
        /// <summary> 
        /// event handler for sessions disconnecting 
        /// </summary> 
        /// <param name="session"> the session that is disconnecting </param> 
        /// <param name="close"> the reason why the session was closed </param> 
        static void appServer_sessionClosed(WebSocketSession session, CloseReason close) 
        { 
            sessionList.Remove(session); // remove the session from the session list 
        } 
    } 
}