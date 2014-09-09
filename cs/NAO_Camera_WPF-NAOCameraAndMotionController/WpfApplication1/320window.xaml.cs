/** 
 * This software was developed by Austin Hughes 
 * Last Modified: 2013‐06‐11 
 */ 
 
using System; 
using System.IO; 
using System.Windows; 
using System.Windows.Media; 
using System.Windows.Media.Imaging; 
using System.Windows.Threading; 
using System.Threading; 
using Aldebaran; 
 
namespace NAO_Camera_WPF 
{ 
    public partial class _320window : Window 
    { 
        // Classes 
        private Camera naoCam = null; 
        private Motion naoMotion = null; 
        private Audio naoAudio = null; 
        private DispatcherTimer dispatcherTimer = new DispatcherTimer(); 
        private DispatcherTimer recordingTimer = new DispatcherTimer(); 
        private DataStorage storage = new DataStorage(); 
        private GetFrame newFrames; 
        private Thread frameThread; 
 
        // Variables 
        private const int COLOR_SPACE = 13; 
        private const int FPS = 30; 
        private bool isCamInitialized; 
        private bool isPictureUpdating = false; 
        private NaoCamImageFormat currentFormat; 
        private NaoCamImageFormat HDFormat; 
        private int time = 0; 
        private bool areJointsSet = false; 
 
        /// <summary> 
        /// Class constructor, starts getting frames from the robot 
        /// and makes a connection to the motion and audio classes 
        /// </summary> 
        /// <param name="ip"> ip address of the NAO robot </param> 
        public _320window(string ip) 
        { 
            InitializeComponent(); 
 
            // display the IP in the ipBox so the user knows which NAO it is connected to 
            ipBox.Text = ip;  
             
            // call the Camera constuctor, and set the image format to 320x240 
            naoCam = new Camera(); 
            currentFormat = naoCam.NaoCamImageFormats[1]; 
            HDFormat = naoCam.NaoCamImageFormats[3]; 
 
            // call the motion constructor 
            naoMotion = new Motion(); 
 
            // call the Audio constructor 
            naoAudio = new Audio(); 
 
            // create timer to display recording time 
            recordingTimer.Interval = new TimeSpan(0, 0, 1); 
            recordingTimer.Tick += new EventHandler(recordingTimeIncrease); 
 
            try 
            { 
                // connect to the NAO Motion API 
                naoMotion.connect(ipBox.Text); 
 
                // create the newFrames instance of the getFrames class 
                newFrames = new GetFrame(ipBox.Text, currentFormat, COLOR_SPACE, FPS, naoCam, storage); 
 
                // create a new thread to allow frame acquisition to occur without interrupting UI smoothness 
                frameThread = new Thread(new ThreadStart(newFrames.grabFrame)); 
                 
                // start the thread 
                frameThread.Start(); 
 
                // Create a timer for event based frame acquisition.  
                // Program will get new frame from storage based on FPS 
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)Math.Ceiling(1000.0 / FPS)); 
                dispatcherTimer.Start(); 
 
                // whenever the timer ticks the bitmapReady event is called 
                dispatcherTimer.Tick += new EventHandler(bitmapReady); 
 
                // let rest of program know that camera is ready 
                isCamInitialized = true;  
            } 
            catch (Exception ex) 
            { 
                isCamInitialized = false; 
 
                // display error message and write exceptions to a file 
                MessageBox.Show("Exception occurred, error log in C:\\NAOcam\\exception.txt"); 
                System.IO.File.WriteAllText(@"C:\\NAOcam\\exception.txt", ex.ToString());  
            } 
        } 
 
        /// <summary> 
        /// called when the window closes 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) 
        { 
            try 
            { 
                while (frameThread.IsAlive) 
                { 
                    // stop the frame acquisition thread 
                    frameThread.Abort(); 
                } 
            } 
            catch (Exception) 
            { } 
 
            // stop timer and disconnect camera 
            dispatcherTimer.Stop(); 
            naoCam.Disconnect();           
        } 
 
        /// <summary> 
        /// Updates picture box with a new bitmap 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void bitmapReady(object sender, EventArgs e) 
        { 
            if (isCamInitialized && !isPictureUpdating) 
            { 
                isPictureUpdating = true;  
 
                try 
                { 
                    // get image from storage class 
                    byte[] imageBytes = storage.getBytes(); 
 
                    // if the image isnt empty create a bitmap and update primaryImage 
                    if (imageBytes != null)  
                    { 
                        primaryImage.Source = BitmapSource.Create(currentFormat.width, 
currentFormat.height, 96, 96, PixelFormats.Bgr24, BitmapPalettes.WebPalette, imageBytes, 
currentFormat.width * 3); 
                    } 
                } 
                catch (Exception e1) 
                { 
                    // display error message and write exceptions to a file 
                    MessageBox.Show("Exception occurred, error log in C:\\NAOcam\\exception.txt"); 
                    System.IO.File.WriteAllText(@"C:\\NAOcam\\exception.txt", e1.ToString()); 
                } 
            } 
            isPictureUpdating = false; // picture is updated 
        } 
 
        /// <summary> 
        /// Called when the save HD image button is pressed 
        /// Saves an HD (1280x960) version of the next available frame 
        /// </summary> 
        /// <param name="sender"> object that called the event </param> 
        /// <param name="e"> any additional arguments </param> 
        private void saveHdButton_Click(object sender, RoutedEventArgs e) 
        { 
            // stop framethread from running 
            frameThread.Abort(); 
 
            // Alert the user of the length of the process 
            MessageBox.Show("Warning: This process will take several seconds during which the program will be unresponsive."); 
 
            try 
            { 
                // disconnect from the camera 
                naoCam.Disconnect(); 
 
                // sleep to allow disconnect to occur 
                Thread.Sleep(150); 
 
                // connect to camera using HD resolution 
                naoCam.connect(ipBox.Text, HDFormat, COLOR_SPACE, 5); 
 
                // get an HD image 
                byte[] image = naoCam.getImage(); 
 
                // create a bitmap from the image 
                BitmapSource bitmap = BitmapSource.Create(HDFormat.width, 
HDFormat.height, 96, 96, PixelFormats.Bgr24, BitmapPalettes.WebPalette, image, 
HDFormat.width * 3); 
 
                // create a bitmap frame for use in JpegBitmapEncoder 
                BitmapFrame bitFrame = BitmapFrame.Create(bitmap); 
 
                // jpeg encoder to save file 
                JpegBitmapEncoder encoder = new JpegBitmapEncoder(); 
 
                // add the bitmap frame to the encoder 
                encoder.Frames.Add(bitFrame); 
 
                // get the current time in terms of windows file time 
                string time = Convert.ToString(DateTime.UtcNow.ToFileTime()); 
 
                // generate a file name 
                string filename = "C:\\NAOcam\\HDcapture" + time + ".jpg"; 
 
                // save the file to disk 
                using (var stream = File.Create(filename)) 
                { 
                    encoder.Save(stream); 
                } 
 
                // disconnect from camera 
                naoCam.Disconnect(); 
            } 
            catch (Exception e1) 
            { 
                // display error message and write exceptions to a file 
                MessageBox.Show("Exception occurred, error log in C:\\NAOcam\\exception.txt"); 
                System.IO.File.WriteAllText(@"C:\\NAOcam\\exception.txt", e1.ToString()); 
            } 
 
            // sleep to ensure disconnect happened 
            Thread.Sleep(150); 
 
            // connect to camera using 320x240 resolution 
            naoCam.connect(ipBox.Text, currentFormat, COLOR_SPACE, FPS); 
 
            // restart frame thread 
            frameThread = new Thread(new ThreadStart(newFrames.grabFrame)); 
            frameThread.Start(); 
 
            // let user know process finished 
            MessageBox.Show("Image saved in C:\\NAOcam\\"); 
        } 
 
        /// <summary> 
        /// Called when the start button is clicked 
        /// Starts audio recording 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void startButton_Click(object sender, RoutedEventArgs e) 
        { 
            stopButton.IsEnabled = true; 
            startButton.IsEnabled = false; 
            time = 0; 
            recordingTimer.Start(); 
 
            naoAudio.connect(ipBox.Text); 
            naoAudio.record(); 
        } 
 
        /// <summary> 
        /// Called when the start button is clicked 
        /// Stops audio recording 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void stopButton_Click(object sender, RoutedEventArgs e) 
        { 
            startButton.IsEnabled = true; 
            stopButton.IsEnabled = false; 
            recordingTimer.Stop(); 
            timeBlock.Text = "Recording Time: "; 
 
            naoAudio.stopRecording(); 
        } 
 
        /// <summary> 
        /// Called by the recordingTime timer, updates the amount 
        /// of time that a audio file has been recording 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void recordingTimeIncrease(object sender, EventArgs e) 
        { 
            time++; 
            timeBlock.Text = "Recording Time: " + time + " seconds"; 
        } 
 
        /// <summary> 
        /// Called when the say button is clicked 
        /// Sends a string to the text to speech engine 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void sayButton_Click(object sender, RoutedEventArgs e) 
        { 
            naoAudio.talk(ttsBox.Text); 
        } 
 
        /// <summary> 
        /// Called when the open hand button is clicked 
        /// Opens the left hand 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void lHandOpenButton_Click(object sender, RoutedEventArgs e) 
        { 
            naoMotion.openHand("LHand"); 
        } 
 
        /// <summary> 
        /// Called when the close hand button is clicked 
        /// Closes the left hand 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void lHandCloseButton_Click(object sender, RoutedEventArgs e) 
        { 
            naoMotion.closeHand("LHand"); 
        } 
 
        /// <summary> 
        /// Called when the open hand button is clicked 
        /// Opens the right hand 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void rHandOpenButton_Click(object sender, RoutedEventArgs e) 
        { 
            naoMotion.openHand("RHand"); 
        } 
 
        /// <summary> 
        /// Called when the close hand button is clicked 
        /// Closes the right hand 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void rHandCloseButton_Click(object sender, RoutedEventArgs e) 
        { 
            naoMotion.closeHand("RHand"); 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the head yaw angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void yawSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)yawSlider.Value, "HeadYaw"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the head pitch angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void pitchSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)pitchSlider.Value, "HeadPitch"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the left shoulder pitch angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void lspSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)lspSlider.Value, "LShoulderPitch"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the right shoulder pitch angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void rspSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)rspSlider.Value, "RShoulderPitch"); 
                } 
                catch (Exception) 
                { } 
            } 
 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the left shoulder roll angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void lsrSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)lsrSlider.Value, "LShoulderRoll"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the right shoulder roll angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void rsrSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)rsrSlider.Value, "RShoulderRoll"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the left elbow yaw angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void leySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)leySlider.Value, "LElbowYaw"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the right elbow yaw angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void reySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)reySlider.Value, "RElbowYaw"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the left elbow roll angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void lerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)lerSlider.Value, "LElbowRoll"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the right elbow roll angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void rerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)rerSlider.Value, "RElbowRoll"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the left wrist yaw angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void lwySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)lwySlider.Value, "LWristYaw"); 
                } 
                catch (Exception) 
                { } 
            } 
            else 
            { 
                areJointsSet = true; 
            } 
        } 
 
        /// <summary> 
        /// called when the slider is moved 
        /// sets the right wrist yaw angle to the value 
        /// of the slider 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void rwySlider_ValueChanged(object sender,RoutedPropertyChangedEventArgs<double> e) 
        { 
            if (areJointsSet == true) 
            { 
                try 
                { 
                    naoMotion.moveJoint((float)rwySlider.Value, "RWristYaw"); 
                } 
                catch (Exception) 
                { } 
            } 
        } 
 
        /// <summary> 
        /// Called when the window is loaded 
        /// Loads the current angles of the joints 
        /// and sets the sliders to these values 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> any additional arguments </param> 
        private void Window_Loaded(object sender, RoutedEventArgs e) 
        { 
            try 
            { 
                pitchSlider.Value = naoMotion.getAngle("HeadPitch"); 
                yawSlider.Value = naoMotion.getAngle("HeadYaw"); 
                rspSlider.Value = naoMotion.getAngle("RShoulderPitch"); 
                lspSlider.Value = naoMotion.getAngle("LShoulderPitch"); 
                rsrSlider.Value = naoMotion.getAngle("RShoulderRoll"); 
                lsrSlider.Value = naoMotion.getAngle("LShoulderRoll"); 
                reySlider.Value = naoMotion.getAngle("RElbowYaw"); 
                leySlider.Value = naoMotion.getAngle("LElbowYaw"); 
                rerSlider.Value = naoMotion.getAngle("RElbowRoll"); 
                lerSlider.Value = naoMotion.getAngle("LElbowRoll"); 
                rwySlider.Value = naoMotion.getAngle("RWristYaw"); 
                lwySlider.Value = naoMotion.getAngle("LWristYaw"); 
            } 
            catch (Exception e1) 
            { 
                // display error message and write exceptions to a file 
                MessageBox.Show("Exception occurred, error log in C:\\NAOcam\\exception.txt"); 
                System.IO.File.WriteAllText(@"C:\\NAOcam\\exception.txt", e1.ToString()); 
            } 
        } 
    } 
}