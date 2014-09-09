using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Aldebaran.Proxies;

namespace TestNaoConsole
{
    class Program
    {
        private static string ip = "167.205.66.109";
        //private static string ip = "127.0.0.1";
        private static int port = 9559;

        private static MotionProxy motion = new MotionProxy(ip, port);
        private static MemoryProxy mem = new MemoryProxy(ip, port);
        private static RobotPostureProxy posture = new RobotPostureProxy(ip, port);

        private static List<string> headYawValSensor = new List<string>();
        private static List<string> headPitchValSensor = new List<string>();

        //for all body recording
        //26 array
        private static List<List<string>> bodySensorValue = new List<List<string>>();
        //26 joint of Nao robot
        private static Dictionary<string, string> bodySensorName = new Dictionary<string, string>
                                                                      {
                                                                          //Head
                                                                          {"HeadYaw","Device/SubDeviceList/HeadYaw/Position/Sensor/Value"},
                                                                          {"HeadPitch","Device/SubDeviceList/HeadPitch/Position/Sensor/Value"},
                                                                          //LeftArm
                                                                          {"LShoulderPitch","Device/SubDeviceList/LShoulderPitch/Position/Sensor/Value"},
                                                                          {"LShoulderRoll","Device/SubDeviceList/LShoulderRoll/Position/Sensor/Value"},
                                                                          {"LElbowYaw","Device/SubDeviceList/LElbowYaw/Position/Sensor/Value"},
                                                                          {"LElbowRoll","Device/SubDeviceList/LElbowRoll/Position/Sensor/Value"},
                                                                          {"LWristYaw","Device/SubDeviceList/LWristYaw/Position/Sensor/Value"},
                                                                          {"LHand","Device/SubDeviceList/LHand/Position/Sensor/Value"},
                                                                          //RightArm
                                                                          {"RShoulderPitch","Device/SubDeviceList/RShoulderPitch/Position/Sensor/Value"},
                                                                          {"RShoulderRoll","Device/SubDeviceList/RShoulderRoll/Position/Sensor/Value"},
                                                                          {"RElbowYaw","Device/SubDeviceList/RElbowYaw/Position/Sensor/Value"},
                                                                          {"RElbowRoll","Device/SubDeviceList/RElbowRoll/Position/Sensor/Value"},
                                                                          {"RWristYaw","Device/SubDeviceList/RWristYaw/Position/Sensor/Value"},
                                                                          {"RHand","Device/SubDeviceList/RHand/Position/Sensor/Value"},
                                                                          //LeftLeg
                                                                          {"LHipYawPitch","Device/SubDeviceList/LHipYawPitch/Position/Sensor/Value"},
                                                                          {"LHipRoll","Device/SubDeviceList/LHipRoll/Position/Sensor/Value"},
                                                                          {"LHipPitch","Device/SubDeviceList/LHipPitch/Position/Sensor/Value"},
                                                                          {"LKneePitch","Device/SubDeviceList/LKneePitch/Position/Sensor/Value"},
                                                                          {"LAnklePitch","Device/SubDeviceList/LAnklePitch/Position/Sensor/Value"},
                                                                          {"LAnkleRoll","Device/SubDeviceList/LAnkleRoll/Position/Sensor/Value"},
                                                                          //RightLeg
                                                                          {"RHipYawPitch","Device/SubDeviceList/RHipYawPitch/Position/Sensor/Value"},
                                                                          {"RHipRoll","Device/SubDeviceList/RHipRoll/Position/Sensor/Value"},
                                                                          {"RHipPitch","Device/SubDeviceList/RHipPitch/Position/Sensor/Value"},
                                                                          {"RKneePitch","Device/SubDeviceList/RKneePitch/Position/Sensor/Value"},
                                                                          {"RAnklePitch","Device/SubDeviceList/RAnklePitch/Position/Sensor/Value"},
                                                                          {"RAnkleRoll","Device/SubDeviceList/RAnkleRoll/Position/Sensor/Value"},
                                                                      };

        static void Main()
        {


            Console.WriteLine("------ WELCOME TO NAO CONSOLE REMOTE PROGRAM ------- \n");
            Console.WriteLine(">> Program starting...");
            ShowOptionCommand();

            Console.Write(">> Enter your command : ");
            var userkey = Console.ReadLine();
            Console.WriteLine();

            while (userkey != "0")
            {
                switch (userkey)
                {
                    case "1":
                        GetMotSummary();
                        break;
                    case "2":
                        GetMemDataListName();
                        break;
                    case "3":
                        GetMemDataHead();
                        break;
                    case "4":
                        RecordMemDataHead();
                        break;
                    case "5":
                        TestMotionHeadRecorded();
                        break;
                    case "6":
                        RecordMemDataBody();
                        break;
                    case "7":
                        TestMotionBodyRecorded();
                        break;
                    default:
                        Console.WriteLine(">> Your command is not valid. Please try again.");
                        break; ;
                }

                Console.WriteLine();
                ShowOptionCommand();
                Console.Write(">> Enter your command : ");
                userkey = Console.ReadLine();
                Console.WriteLine();
            }

            Console.WriteLine();

        }

        static void ShowOptionCommand()
        {
            Console.WriteLine(">> Press 1 to get motion-summary.");
            Console.WriteLine(">> Press 2 to get mem-dataList.");
            Console.WriteLine(">> Press 3 to get mem-dataHead.");
            Console.WriteLine(">> Press 4 to Record Head Sensor Data.");
            Console.WriteLine(">> Press 5 to Play Recorded Head Sensor Data.");
            Console.WriteLine(">> Press 6 to Record Body Sensor Data.");
            Console.WriteLine(">> Press 7 to Play Record Body Sensor Data.");
            Console.WriteLine(">> Press 0 to exit. \n");
        }

        static void GetMotSummary()
        {
            try
            {
                var summary = motion.getSummary();
                Console.WriteLine(summary);
            }
            catch (Exception ex)
            {
                ErrorReport(ex.Message);
            }
        }

        static void GetMemDataListName()
        {
            try
            {
                var memDataListName = mem.getDataListName();
                Console.WriteLine(memDataListName.Count);
                for (var i = 0; i < memDataListName.Count; i++)
                {
                    Console.WriteLine("{0}.{1}", i + 1, memDataListName[i]);
                }
            }
            catch (Exception ex)
            {
                ErrorReport(ex.Message);
            }


        }

        static void GetMemDataHead()
        {
            try
            {
                var headPitchSensor = "Device/SubDeviceList/HeadPitch/Position/Sensor/Value";
                var headYawSensor = "Device/SubDeviceList/HeadYaw/Position/Sensor/Value";
                var headYawSensorVal = mem.getData(headYawSensor);
                var headPitchSensorVal = mem.getData(headPitchSensor);
                Console.WriteLine("GetMemDataHead");
                Console.WriteLine("| headYawSensor             | headPitchSensorVal     | ");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("| {0}     | {1}    |", headYawSensorVal, headPitchSensorVal);
            }
            catch (Exception ex)
            {
                ErrorReport(ex.Message);
            }

        }

        static void ErrorReport(string errorMsg)
        {
            Console.WriteLine(">> Program Error !!!");
            Console.WriteLine(errorMsg);
        }



        static void SaveDataToCsv(List<List<string>> memVal)
        {
            try
            {
                CsvExport myExport = new CsvExport();

                for (var i = 0; i < memVal.Count; i++)
                {
                    myExport.AddRow();

                    //every single row has 26 value
                    var jointIndex = 0;
                    foreach (var joint in bodySensorName)
                    {
                        myExport[joint.Key] = memVal[i][jointIndex];
                        jointIndex++;
                    }
                }

                // Then you can do any of the following three output options:
                myExport.ExportToFile("LumenBodySensorData.csv");
                Console.WriteLine("Save to LumenBodySensorData.csv completed.");
            }
            catch (Exception)
            {

                throw;
            }

        }

        static void SaveDataToCsv(List<string> headYawValSensor, List<string> headPitchValSensor)
        {
            try
            {
                CsvExport myExport = new CsvExport();

                for (var i = 0; i < headYawValSensor.Count; i++)
                {
                    myExport.AddRow();
                    myExport["HeadYaw Sensor"] = headYawValSensor[i];
                    myExport["HeadPitch Sensor"] = headPitchValSensor[i];
                }

                // Then you can do any of the following three output options:
                //   string myCsv = myExport.Export();
                myExport.ExportToFile("LumenHeadData.csv");
                //   byte[] myCsvData = myExport.ExportToBytes();
                Console.WriteLine("Save to csv completed.");
            }
            catch (Exception)
            {

                throw;
            }

        }


        //Record 26 joint of nao body
        static void RecordMemDataBody()
        {
            try
            {
                Console.WriteLine(">> To start recording position press 'R' ");
                Console.WriteLine(">> To stop recording position press 'S' ");
                var key = Console.ReadLine();

                if (key == "r" || key == "R")
                {
                    var loop = 1;
                    Wait(5);
                    do
                    {
                        while (!Console.KeyAvailable)
                        {
                            List<string> memValList = new List<string>();
                            foreach (var a in bodySensorName)
                            {
                                var memValStr = mem.getData(a.Value).ToString();
                                memValList.Add(memValStr);
                            }
                            bodySensorValue.Add(memValList);
                            Console.WriteLine(">> Record {0} done.", loop);
                            Console.WriteLine();
                            loop++;
                            Wait(5);
                        }
                    } while (Console.ReadKey(true).Key != ConsoleKey.S);

                    Beep();
                    Console.WriteLine(">> Stop Recording...");
                    Console.WriteLine(">> Saving data to .csv format.");

                    SaveDataToCsv(bodySensorValue);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Record motion of head : headpitch and headyaw
        static void RecordMemDataHead()
        {
            try
            {
                Console.WriteLine(">> To start recording position press 'r' ");
                Console.WriteLine(">> To stop recording position press 's' ");
                var key = Console.ReadLine();

                if (key == "r" || key == "R")
                {
                    //List<string> headYawValSensor = new List<string>();
                    //List<string> headPitchValSensor = new List<string>();
                    var headPitchSensorName = "Device/SubDeviceList/HeadPitch/Position/Sensor/Value";
                    //var headPitchActuator = "Device/SubDeviceList/HeadPitch/Position/Actuator/Value";
                    var headYawSensorName = "Device/SubDeviceList/HeadYaw/Position/Sensor/Value";
                    //var headYawActuator = "Device/SubDeviceList/HeadYaw/Position/Actuator/Value";

                    Wait(5);
                    do
                    {
                        var headPitchVal = mem.getData(headPitchSensorName);
                        var headYawVal = mem.getData(headYawSensorName);
                        var temHeadPicthVal = headPitchVal;
                        var temHeadYawVal = headYawVal;
                        var loop = 0;
                        while (!Console.KeyAvailable)
                        {

                            headPitchVal = mem.getData(headPitchSensorName);
                            headYawVal = mem.getData(headYawSensorName);

                            if (loop != 0)
                            {
                                if (headPitchVal == temHeadPicthVal && headYawVal == temHeadYawVal)
                                {
                                    //do nothing
                                    Console.WriteLine(">> --- no new data ---");
                                    Console.WriteLine();

                                }
                                else
                                {
                                    temHeadPicthVal = headPitchVal;
                                    temHeadYawVal = headYawVal;
                                    headYawValSensor.Add(headYawVal.ToString());
                                    headPitchValSensor.Add(headPitchVal.ToString());
                                    Console.WriteLine("| headYaw            | HeadPitch      |");
                                    Console.WriteLine("-------------------------------------------------------");
                                    Console.WriteLine("{0}     | {1}    |", headYawVal, headPitchVal);
                                    Console.WriteLine();
                                    loop++;
                                }
                            }
                            else
                            {
                                temHeadPicthVal = headPitchVal;
                                temHeadYawVal = headYawVal;
                                headYawValSensor.Add(headYawVal.ToString());
                                headPitchValSensor.Add(headPitchVal.ToString());
                                Console.WriteLine("| headYaw            | HeadPitch      |");
                                Console.WriteLine("-------------------------------------------------------");
                                Console.WriteLine("| {0}     | {1}    |", headYawVal, headPitchVal);
                                Console.WriteLine();
                                loop++;
                            }
                            Wait(5);
                        }
                    } while (Console.ReadKey(true).Key != ConsoleKey.S);

                    Beep();
                    Console.WriteLine(">> Stop Recording...");
                    Console.WriteLine(">> Saving data to .csv format.");

                    SaveDataToCsv(headYawValSensor, headPitchValSensor);
                }



            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void Beep()
        {
            int freq = 3000;
            int duration = 200;
            Console.Beep(freq, duration);
        }

        private static void Wait(int second)
        {
            for (int i = 0; i < second; i++)
            {
                //1s=1000ms
                Thread.Sleep(1000);
            }
            Beep();
        }

        private static void TestMotionHeadRecorded()
        {
            try
            {
                motion.setStiffnesses("Head", 1.0f);
                for (var i = 0; i < headPitchValSensor.Count; i++)
                {
                    var headpitch = float.Parse(headPitchValSensor[i]);
                    var headyaw = float.Parse(headYawValSensor[i]);
                    motion.setAngles("HeadPitch", headpitch, 0.3f);
                    motion.setAngles("HeadYaw", headyaw, 0.3f);
                    Console.WriteLine(">> press any key to continue...");
                    Console.ReadKey();
                }
                motion.setStiffnesses("Head", 0.0f);

                Console.WriteLine(">> Head Record Playing done.");
            }
            catch (Exception)
            {
                motion.setStiffnesses("Head", 0.0f);
                Console.WriteLine(">> Error server.");
            }
        }

        private static void TestMotionBodyRecorded()
        {
            try
            {
                List<string> jointNameList = new List<string>();
                foreach (var a in bodySensorName)
                {
                    jointNameList.Add(a.Key);
                }

                motion.setStiffnesses("Body", 1.0f);
                const float fractionMaxSpeed = 0.1f;
                for (var i = 0; i < bodySensorValue.Count; i++)
                {
                    var angleValList = new List<float>();
                    foreach (var a in bodySensorValue[i])
                    {
                        var valTemp = float.Parse(a);
                        angleValList.Add(valTemp);
                    }
                    //motion.setAngles(jointNameList, angleValList, fractionMaxSpeed);
                    motion.angleInterpolationWithSpeed(jointNameList,angleValList,fractionMaxSpeed);
                    Console.WriteLine(">> Press any key to continue motion...");
                    //Console.ReadKey();
                }
                motion.setStiffnesses("Body", 0.0f);
                motion.rest();

                Console.WriteLine(">> Body Record Playing Done.");
                Console.WriteLine(">> Lumen status : REST.");
            }
            catch (Exception)
            {
                motion.setStiffnesses("Body", 0.0f);
                Console.WriteLine(">> Error server.");
            }
        }

    }
}
