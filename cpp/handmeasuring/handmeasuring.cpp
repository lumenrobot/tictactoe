#include "handmeasuring.h"
#include "function.h"
#include <fstream>
#include <sstream>

using namespace std;
using namespace	AL;
using namespace Math;

HandMeasuring::HandMeasuring(
  boost::shared_ptr<AL::ALBroker> broker,
  const std::string& name) : AL::ALModule(broker, name), 
     fMemoryProxy(getParentBroker()),
	 robotPosture(getParentBroker()),
	 motion(getParentBroker()),
	 tts(getParentBroker()),
	 fCallbackMutex(AL::ALMutex::createALMutex())
{
  setModuleDescription("This module measures Lumen's end effectors position and orientation in particular time.");
  
  functionName("startLArmMeasure",
               getName(),
               "Called by ALMemory when Lumen's front head is touched.");
  BIND_METHOD(HandMeasuring::startLArmMeasure);

  functionName("startRArmMeasure",
               getName(),
               "Called by ALMemory when Lumen's middle head is touched.");
  BIND_METHOD(HandMeasuring::startRArmMeasure);

  functionName("startTorsoMeasure",
               getName(),
               "Called by ALMemory when Lumen's back head is touched.");
  BIND_METHOD(HandMeasuring::startTorsoMeasure);
}

HandMeasuring::~HandMeasuring() {
  fMemoryProxy.unsubscribeToEvent("Front", "HandMeasuring");
  fMemoryProxy.unsubscribeToEvent("Middle", "HandMeasuring");
  fMemoryProxy.unsubscribeToEvent("Rear", "HandMeasuring");
 }

ofstream LArmPR,RArmPR,body,head;

void HandMeasuring::init() {
	try {
    motion.setStiffnesses("Body",0.75f);
	robotPosture.goToPosture("Crouch", 0.5f);
	qi::os::msleep(1000);
	motion.setAngles(ALValue::array("LHand", "RHand"),ALValue::array(0.75f, 0.75f), 0.1f);
	motion.setStiffnesses(ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw"), ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	motion.setStiffnesses(ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"), ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	//qi::os::msleep(25000);
	motion.setAngles(ALValue::array("LHand", "RHand"),ALValue::array(0.3f, 0.3f), 0.1f);
	LArmPR.open("LArmPR.csv");RArmPR.open("RArmPR.csv");
	body.open("body.csv");head.open("head.csv");
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.6f,0.6f));
	//motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.1f);
	//motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.8373f), 0.1f);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.1f);
	motion.setStiffnesses(ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw"), ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	motion.setStiffnesses(ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"), ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	fMemoryProxy.subscribeToEvent("FrontTactilTouched", "HandMeasuring",
                                  "startLArmMeasure");
	fMemoryProxy.subscribeToEvent("MiddleTactilTouched", "HandMeasuring",
                                  "startRArmMeasure");
	fMemoryProxy.subscribeToEvent("RearTactilTouched", "HandMeasuring",
                                  "startTorsoMeasure");
  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
  }
}

  ALValue namesL = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw");
  ALValue namesR = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw");
  ALValue namesF = ALValue::array("LHipPitch", "RHipPitch");
  bool useSensors = true;
  unsigned int i = 0;
  unsigned int j = 0;
  unsigned int k = 0;
  unsigned int l = 0;
  vector<float> angleValL(5,0.0f);
  vector<float> angleValR(5,0.0f);
  ALValue senvalL = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
  ALValue senvalR = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
  ALValue senvalH = ALValue::array(0.0f, 0.0f);
  Transform TL,TR,TH;
  Position6D prsL,prsR,prsH;
  vector<float> PRLv(6,0.0f);
  vector<float> PRRv(6,0.0f);
  vector<float> PRHv(6,0.0f);
  float PRLL[1000][6];
  float PRRR[1000][6];
  float Body[1000][14];
  float Head[1000][6];
  //ALValue theta0 = ALValue::array(0.13955f);
  //ALValue theta0 = ALValue::array(0.0f);
  ALValue theta0 = ALValue::array(0.34888f);
 
void HandMeasuring::startLArmMeasure() {
  qiLogInfo("module.example") << "Measuring LArm (LHand) position and orientation" << std::endl;
  AL::ALCriticalSection section(fCallbackMutex);
  fState1 =  fMemoryProxy.getData("FrontTactilTouched");
  if (fState1 > 0.5f) {
   try {
	   motion.setStiffnesses("LHand", 0.8f);
	   switch (i)
		{
		case 2:
			cout << "Wait for 3 seconds" << endl;
			qi::os::msleep(3000);
			cout << "LArm Measurement No.: " << i+1 << endl;
			senvalL.clear();
			angleValL = motion.getAngles(namesL,useSensors);
			cout << "LArm sensor angles: " << angleValL[0] << " " << angleValL[1] << " " << angleValL[2] << " " << angleValL[3] << " " << angleValL[4] << endl;
			senvalL = ALValue::array(angleValL[0],angleValL[1],angleValL[2],angleValL[3],angleValL[4]);
			TL = LArmForwardHipBase(senvalL,theta0);
			prsL = Matrix2PosOrient(TL);
			cout << "LHand position and orientation: "<< "," << prsL << endl;
			PRLv.clear();
			PRLv = prsL.toVector();
			LArmPR << PRLL[i][0] << PRLv[0] << "," << PRLL[i][1] << PRLv[1] << "," << PRLL[i][2] << PRLv[2] << "," << PRLL[i][3] << PRLv[3] << "," <<
			PRLL[i][4] << PRLv[4] << "," << PRLL[i][5] << PRLv[5] << "," << endl;
			motion.setAngles("LHand",0.75f, 0.15f);
			break;
		case 3:
			cout << "Wait for 3 seconds" << endl;
			qi::os::msleep(3000);
			cout << "LArm Measurement No.: " << i+1 << endl;
			senvalL.clear();
			angleValL = motion.getAngles(namesL,useSensors);
			cout << "LArm sensor angles: " << angleValL[0] << " " << angleValL[1] << " " << angleValL[2] << " " << angleValL[3] << " " << angleValL[4] << endl;
			senvalL = ALValue::array(angleValL[0],angleValL[1],angleValL[2],angleValL[3],angleValL[4]);
			TL = LArmForwardHipBase(senvalL,theta0);
			prsL = Matrix2PosOrient(TL);
			cout << "LHand position and orientation: "<< "," << prsL << endl;
			PRLv.clear();
			PRLv = prsL.toVector();
			LArmPR << PRLL[i][0] << PRLv[0] << "," << PRLL[i][1] << PRLv[1] << "," << PRLL[i][2] << PRLv[2] << "," << PRLL[i][3] << PRLv[3] << "," <<
			PRLL[i][4] << PRLv[4] << "," << PRLL[i][5] << PRLv[5] << "," << endl;
			motion.setAngles("LHand",0.3f, 0.15f);
			break;
		case 5:
			cout << "Wait for 3 seconds" << endl;
			qi::os::msleep(3000);
			cout << "LArm Measurement No.: " << i+1 << endl;
			senvalL.clear();
			angleValL = motion.getAngles(namesL,useSensors);
			cout << "LArm sensor angles: " << angleValL[0] << " " << angleValL[1] << " " << angleValL[2] << " " << angleValL[3] << " " << angleValL[4] << endl;
			senvalL = ALValue::array(angleValL[0],angleValL[1],angleValL[2],angleValL[3],angleValL[4]);
			TL = LArmForwardHipBase(senvalL,theta0);
			prsL = Matrix2PosOrient(TL);
			cout << "LHand position and orientation: "<< "," << prsL << endl;
			PRLv.clear();
			PRLv = prsL.toVector();
			LArmPR << PRLL[i][0] << PRLv[0] << "," << PRLL[i][1] << PRLv[1] << "," << PRLL[i][2] << PRLv[2] << "," << PRLL[i][3] << PRLv[3] << "," <<
			PRLL[i][4] << PRLv[4] << "," << PRLL[i][5] << PRLv[5] << "," << endl;
			motion.setAngles("LHand",0.75f, 0.15f);
			break;
		case 6:
			cout << "Wait for 3 seconds" << endl;
			qi::os::msleep(3000);
			cout << "LArm Measurement No.: " << i+1 << endl;
			senvalL.clear();
			angleValL = motion.getAngles(namesL,useSensors);
			cout << "LArm sensor angles: " << angleValL[0] << " " << angleValL[1] << " " << angleValL[2] << " " << angleValL[3] << " " << angleValL[4] << endl;
			senvalL = ALValue::array(angleValL[0],angleValL[1],angleValL[2],angleValL[3],angleValL[4]);
			TL = LArmForwardHipBase(senvalL,theta0);
			prsL = Matrix2PosOrient(TL);
			cout << "LHand position and orientation: "<< "," << prsL << endl;
			PRLv.clear();
			PRLv = prsL.toVector();
			LArmPR << PRLL[i][0] << PRLv[0] << "," << PRLL[i][1] << PRLv[1] << "," << PRLL[i][2] << PRLv[2] << "," << PRLL[i][3] << PRLv[3] << "," <<
			PRLL[i][4] << PRLv[4] << "," << PRLL[i][5] << PRLv[5] << "," << endl;
			motion.setAngles("LHand",0.3f, 0.15f);
			break;
		default:	
			qi::os::msleep(100);
			cout << "LArm Measurement No.: " << i+1 << endl;
			senvalL.clear();
			angleValL = motion.getAngles(namesL,useSensors);
			cout << "LArm sensor angles: " << angleValL[0] << " " << angleValL[1] << " " << angleValL[2] << " " << angleValL[3] << " " << angleValL[4] << endl;
			senvalL = ALValue::array(angleValL[0],angleValL[1],angleValL[2],angleValL[3],angleValL[4]);
			TL = LArmForwardHipBase(senvalL,theta0);
			prsL = Matrix2PosOrient(TL);
			cout << "LHand position and orientation: "<< "," << prsL << endl;
			PRLv.clear();
			PRLv = prsL.toVector();
			LArmPR << PRLL[i][0] << PRLv[0] << "," << PRLL[i][1] << PRLv[1] << "," << PRLL[i][2] << PRLv[2] << "," << PRLL[i][3] << PRLv[3] << "," <<
			PRLL[i][4] << PRLv[4] << "," << PRLL[i][5] << PRLv[5] << "," << endl;
		}
		i++;
   }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }
}

void HandMeasuring::startRArmMeasure() {
  qiLogInfo("module.example") << "Measuring RArm (RHand) position and orientation" << std::endl;
  AL::ALCriticalSection section(fCallbackMutex);
  fState2 =  fMemoryProxy.getData("MiddleTactilTouched");
  if (fState2 > 0.5f) {
   try {
	   motion.setStiffnesses("RHand", 0.8f);
	   switch (j)
		{
		case 2:
			cout << "Wait for 3 seconds" << endl;
			qi::os::msleep(3000);
			cout << "RArm Measurement No.: " << j+1 << endl;
			senvalR.clear();
			angleValR = motion.getAngles(namesR,useSensors);
			cout << "RArm sensor angles: " << angleValR[0] << " " << angleValR[1] << " " << angleValR[2] << " " << angleValR[3] << " " << angleValR[4] << endl;
			senvalR = ALValue::array(angleValR[0],angleValR[1],angleValR[2],angleValR[3],angleValR[4]);
			TR = RArmForwardHipBase(senvalR,theta0);
			prsR = Matrix2PosOrient(TR);
			cout << "RHand position and orientation: "<< "," << prsR << endl;
			PRRv.clear();
			PRRv = prsR.toVector();
			RArmPR << PRRR[j][0] << PRRv[0] << "," << PRRR[j][1] << PRRv[1] << "," << PRRR[j][2] << PRRv[2] << "," << PRRR[j][3] << PRRv[3] << "," <<
			PRRR[j][4] << PRRv[4] << "," << PRRR[j][5] << PRRv[5] << "," << endl;
			motion.setAngles("RHand",0.8f, 0.15f);
			break;
		case 3:
			cout << "Wait for 3 seconds" << endl;
			qi::os::msleep(3000);
			cout << "RArm Measurement No.: " << j+1 << endl;
			senvalR.clear();
			angleValR = motion.getAngles(namesR,useSensors);
			cout << "RArm sensor angles: " << angleValR[0] << " " << angleValR[1] << " " << angleValR[2] << " " << angleValR[3] << " " << angleValR[4] << endl;
			senvalR = ALValue::array(angleValR[0],angleValR[1],angleValR[2],angleValR[3],angleValR[4]);
			TR = RArmForwardHipBase(senvalR,theta0);
			prsR = Matrix2PosOrient(TR);
			cout << "RHand position and orientation: "<< "," << prsR << endl;
			PRRv.clear();
			PRRv = prsR.toVector();
			RArmPR << PRRR[j][0] << PRRv[0] << "," << PRRR[j][1] << PRRv[1] << "," << PRRR[j][2] << PRRv[2] << "," << PRRR[j][3] << PRRv[3] << "," <<
			PRRR[j][4] << PRRv[4] << "," << PRRR[j][5] << PRRv[5] << "," << endl;
			motion.setAngles("RHand",0.3f, 0.15f);
			break;
		case 5:
			cout << "Wait for 3 seconds" << endl;
			qi::os::msleep(3000);
			cout << "RArm Measurement No.: " << j+1 << endl;
			senvalR.clear();
			angleValR = motion.getAngles(namesR,useSensors);
			cout << "RArm sensor angles: " << angleValR[0] << " " << angleValR[1] << " " << angleValR[2] << " " << angleValR[3] << " " << angleValR[4] << endl;
			senvalR = ALValue::array(angleValR[0],angleValR[1],angleValR[2],angleValR[3],angleValR[4]);
			TR = RArmForwardHipBase(senvalR,theta0);
			prsR = Matrix2PosOrient(TR);
			cout << "RHand position and orientation: "<< "," << prsR << endl;
			PRRv.clear();
			PRRv = prsR.toVector();
			RArmPR << PRRR[j][0] << PRRv[0] << "," << PRRR[j][1] << PRRv[1] << "," << PRRR[j][2] << PRRv[2] << "," << PRRR[j][3] << PRRv[3] << "," <<
			PRRR[j][4] << PRRv[4] << "," << PRRR[j][5] << PRRv[5] << "," << endl;
			motion.setAngles("RHand",0.8f, 0.15f);
			break;
		case 6:
			cout << "Wait for 3 seconds" << endl;
			qi::os::msleep(3000);
			cout << "RArm Measurement No.: " << j+1 << endl;
			senvalR.clear();
			angleValR = motion.getAngles(namesR,useSensors);
			cout << "RArm sensor angles: " << angleValR[0] << " " << angleValR[1] << " " << angleValR[2] << " " << angleValR[3] << " " << angleValR[4] << endl;
			senvalR = ALValue::array(angleValR[0],angleValR[1],angleValR[2],angleValR[3],angleValR[4]);
			TR = RArmForwardHipBase(senvalR,theta0);
			prsR = Matrix2PosOrient(TR);
			cout << "RHand position and orientation: "<< "," << prsR << endl;
			PRRv.clear();
			PRRv = prsR.toVector();
			RArmPR << PRRR[j][0] << PRRv[0] << "," << PRRR[j][1] << PRRv[1] << "," << PRRR[j][2] << PRRv[2] << "," << PRRR[j][3] << PRRv[3] << "," <<
			PRRR[j][4] << PRRv[4] << "," << PRRR[j][5] << PRRv[5] << "," << endl;
			motion.setAngles("RHand",0.3f, 0.15f);
			break;
		default:	
			qi::os::msleep(100);
			cout << "RArm Measurement No.: " << j+1 << endl;
			senvalR.clear();
			angleValR = motion.getAngles(namesR,useSensors);
			cout << "RArm sensor angles: " << angleValR[0] << " " << angleValR[1] << " " << angleValR[2] << " " << angleValR[3] << " " << angleValR[4] << endl;
			senvalR = ALValue::array(angleValR[0],angleValR[1],angleValR[2],angleValR[3],angleValR[4]);
			TR = RArmForwardHipBase(senvalR,theta0);
			prsR = Matrix2PosOrient(TR);
			cout << "RHand position and orientation: "<< "," << prsR << endl;
			PRRv.clear();
			PRRv = prsR.toVector();
			RArmPR << PRRR[j][0] << PRRv[0] << "," << PRRR[j][1] << PRRv[1] << "," << PRRR[j][2] << PRRv[2] << "," << PRRR[j][3] << PRRv[3] << "," <<
			PRRR[j][4] << PRRv[4] << "," << PRRR[j][5] << PRRv[5] << "," << endl;
		}	
		j++;
}

  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }
}

void HandMeasuring::startTorsoMeasure() {
  qiLogInfo("module.example") << "Measuring torso position and orientation" << std::endl;
  AL::ALCriticalSection section(fCallbackMutex);
  fstate3 =  fMemoryProxy.getData("RearTactilTouched");
  if (fstate3 > 0.5f) {
   try {
	  std::vector<float> result = motion.getPosition("CameraBottom", 2, true);
	  std::cout << "CameraBottom" << ":" << std::endl;
	  std::cout << "Position (x, y, z): " << result.at(0) << ", " << result.at(1)
				<< ", " << result.at(2) << std::endl;
	  std::cout << "Rotation (x, y, z): " << result.at(3) << ", " << result.at(4)
				<< ", " << result.at(5) << std::endl;
	  result = motion.getPosition("Torso", 2, true);
	  std::cout << "Torso" << ":" << std::endl;
	  std::cout << "Position (x, y, z): " << result.at(0) << ", " << result.at(1)
				<< ", " << result.at(2) << std::endl;
	  std::cout << "Rotation (x, y, z): " << result.at(3) << ", " << result.at(4)
				<< ", " << result.at(5) << std::endl;
	  std::vector<float> sensorA = motion.getAngles(ALValue::array("LHipPitch", "RHipPitch"), true);
      std::cout << "LHipPitch angles: " << std::endl << sensorA[0] << "RHipPitch angles: " << std::endl << sensorA[1] << std::endl;
	  vector<float> sensorAngles = motion.getAngles(ALValue::array("HeadYaw","HeadPitch"), true);
	  senvalH[0] = sensorAngles[0]; 
	  senvalH[1] = sensorAngles[1];
	  TH = HeadForwardHipBase(senvalH,theta0);
	  prsH = Matrix2PosOrient(TH);
	  cout << "Bottom Camera position and orientation: "<< "," << prsH << endl;
	  PRHv.clear();
	  PRHv = prsH.toVector();
	  head << Head[l][0] << PRHv[0] << "," << Head[j][1] << PRHv[1] << "," << Head[j][2] << PRHv[2] << "," << Head[j][3] << PRHv[3] << "," <<
	  Head[j][4] << PRHv[4] << "," << Head[j][5] << PRHv[5] << "," << endl;
      vector<float> sensorAngles1 = motion.getAngles(ALValue::array("LHipYawPitch","LHipRoll","LHipPitch","LKneePitch","LAnklePitch","LAnkleRoll"), true);
	  vector<float> sensorAngles2 = motion.getAngles(ALValue::array("RHipYawPitch","RHipRoll","RHipPitch","RKneePitch","RAnklePitch","RAnkleRoll"), true);
	  for (k = 0; k<=13; k++)
	  {
		  if (k<=1)
		  {
 			body << Body[l][k] << sensorAngles[k] << "," ;
		  }
		  else if ((k>1)&&(k<=7))
	      {
 			body << Body[l][k] << sensorAngles1[k-2] << "," ;
		  }
		  else if ((k>7)&&(k<=12))
		  {
 			body << Body[l][k] << sensorAngles2[k-8] << "," ;
		  }
		  else if (k==13)
		  {
 			body << Body[l][k] << sensorAngles2[5] << "," << endl;
		  }
	  }
	  l++;
   
}

  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }
}

