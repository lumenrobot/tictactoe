/** 
 * This software was developed by Austin Hughes 
 * Last Modified: 2013‐06‐09 
 */

using System.Windows;


namespace NAO_Camera_WPF
{
    /// <summary> 
    /// Interaction logic for MainWindow.xaml 
    /// </summary> 
    public partial class MainWindow : Window
    {
        /// <summary> 
        /// Class constructor 
        /// </summary> 
        public MainWindow()
        {
            InitializeComponent();

            // Make sure the standard output directory exists 
            if (!System.IO.Directory.Exists("C:\\NAOcam\\"))
            {
                System.IO.Directory.CreateDirectory("C:\\NAOcam\\");
            }
        }

        /// <summary> 
        /// Called whenever the button is clicked 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> additional arguments sent </param> 
        private void _640Button_Click(object sender, RoutedEventArgs e)
        {
            // generates a new window 
            _640window window = new _640window(ipBox.Text);

            // displays it 
            window.Show();

            // closes this window 
            this.Close();
        }

        /// <summary> 
        /// Called whenever the button is clicked 
        /// </summary> 
        /// <param name="sender"> object that called the method </param> 
        /// <param name="e"> additional arguments sent </param> 
        private void _320Button_Click(object sender, RoutedEventArgs e)
        {
            // generates a new window 
            _320window window = new _320window(ipBox.Text);

            // displays it 
            window.Show();

            // closes this window 
            this.Close();
        }
    }
}