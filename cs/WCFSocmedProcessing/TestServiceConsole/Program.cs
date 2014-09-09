using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestServiceConsole.ServiceReference1;

namespace TestServiceConsole
{
    class Program
    {
        static void Main()
        {
            TestUploadImage();
            
        }

        static void TestUploadImage()
        {
            Console.WriteLine("Upload file...");
            var errorMessage = "";
            var client = new Service1Client();

            var fileName = "D:\\KULIAHTEMPORER\\Tesis Group\\ONGOING TESIS 2014\\NAO DEVELOPMENT\\CSharp\\LUMEN 3.0_13 APRIL 2014\\SOCMED\\WCFSocmedProcessing\\WCFSocmedProcessing\\bin\\test.jpg";
            Stream stream = File.Open(fileName, FileMode.Open);

            var messageToPost = "With Social Robot Team";
            var isSuccessCreateDraft = client.CreateDraftMail(out errorMessage, messageToPost);
            if(isSuccessCreateDraft)
            {
                Console.WriteLine("Create Draft is Success.");
                Console.WriteLine("Upload Image Now....");
                client.UploadImageStatus(Path.GetFileName(fileName), 0, string.Empty, stream);
                client.PostImageStatusUpdate();
            }
            client.Close();
            Console.WriteLine("Upload file success.");
            Console.WriteLine("Status Updated.");
            Console.ReadLine();
        }
    }
}
