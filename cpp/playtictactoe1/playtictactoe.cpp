#include "playtictactoe.h"
#include "kinematics.h"
#include <fstream>
#include <sstream>

using namespace std;
using namespace	AL;
using namespace Math;

PlayTicTacToe::PlayTicTacToe(
  boost::shared_ptr<AL::ALBroker> broker,
  const std::string& name) : AL::ALModule(broker, name), 
     fMemoryProxy(getParentBroker()),
	 robotPosture(getParentBroker()),
	 motion(getParentBroker()),
	 tts(getParentBroker()),
	 fCallbackMutex(AL::ALMutex::createALMutex())
{
  setModuleDescription("This module makes NAO plays Tic Tac Toe.");
  
  functionName("startMove",
               getName(),
               "Executes movement.");
  BIND_METHOD(PlayTicTacToe::startMove);
}

PlayTicTacToe::~PlayTicTacToe() {
   fMemoryProxy.unsubscribeToEvent("MovePeonNumber", "PlayTicTacToe");
 }

void PlayTicTacToe::init() {
	try {
    motion.wakeUp();
	robotPosture.goToPosture("Crouch", 0.75f);
	qi::os::msleep(3000);
	motion.setStiffnesses("Body",0.0f);
	
	ALValue angle0 = ALValue::array(0.0f);
	Position6D POL00 = Position6D(0.0934113f,0.178f,0.0329965f,0.463601f,0.664928f,-0.0480702f);
	ALValue angleL00 = LArmInverseHipBase(PosOrient2Matrix(POL00),angle0);
	Position6D POL01 = Position6D(-0.00259649f,0.132217f,-0.0326618f,0.0906276f,1.45813f,1.67902f);
	ALValue angleL01 = LArmInverseHipBase(PosOrient2Matrix(POL01),angle0);
	Position6D POR00 = Position6D(0.0887284f,-0.172308f,0.0285112f,0.475629f,0.727068f,0.126569f);
	ALValue angleR00 = RArmInverseHipBase(PosOrient2Matrix(POR00),angle0);
	Position6D POR01 = Position6D(0.00658906f,-0.132896f,-0.032212f,0.289373f,1.46564f,-1.29847f);
	ALValue angleR01 = RArmInverseHipBase(PosOrient2Matrix(POR01),angle0);
	ALValue namesLR	   = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", 
										"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw");
	ALValue ArmsStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmsStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleInit, timeInit;
	int i;

	motion.setStiffnesses(namesLR,ArmsStiffOn);
	
	angleInit.clear();
	angleInit.arraySetSize(10);
	for (i=0; i<=9; i++)
	{
		if((i>=0)&&(i<=4))
		{
			angleInit[i] = ALValue::array(angleL00[i], angleL01[i]);
		} 
		else
		{
			angleInit[i] = ALValue::array(angleR00[i-5], angleR01[i-5]);
		}
	}

	timeInit.clear();
	timeInit.arraySetSize(10);
	for (i=0; i<=9; i++)
	{
		timeInit[i] = ALValue::array(2.0f, 4.0f);
	}

	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));

	motion.setStiffnesses(namesLR,ArmsStiffOn);
	qi::os::msleep(500);
	motion.angleInterpolation(namesLR, angleInit, timeInit, true);
	qi::os::msleep(500);
	motion.setStiffnesses(namesLR,ArmsStiffOff);

	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.1f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));
	
	motion.setStiffnesses(ALValue::array("HeadYaw","HeadPitch"),ALValue::array(1.0f,1.0f));
	qi::os::msleep(1000);
	motion.setAngles(ALValue::array("HeadYaw","HeadPitch"),ALValue::array(-0.0087222f, -0.0558222f),0.25f);
	qi::os::msleep(1000);
	motion.setStiffnesses(ALValue::array("HeadYaw","HeadPitch"),ALValue::array(0.0f,0.0f));

	fMemoryProxy.subscribeToEvent("MovePeonNumber", "PlayTicTacToe",
                                  "startMove");
	
	}
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
  }
}
 
void PlayTicTacToe::startMove() {
  qiLogInfo("module.example") << "Measuring LArm (LHand) position and orientation" << std::endl;
  AL::ALCriticalSection section(fCallbackMutex);
  
  state =  fMemoryProxy.getData("MovePeonNumber");
  pix = fMemoryProxy.getData("MovePeonIx");
  piy = fMemoryProxy.getData("MovePeonIy");
  piz = fMemoryProxy.getData("MovePeonIz");
  pfx = fMemoryProxy.getData("MovePeonFx");
  pfy = fMemoryProxy.getData("MovePeonFy");
  pfz = fMemoryProxy.getData("MovePeonFz");
  save = fMemoryProxy.getData("MovePeonSave");

  cout << "Initial position from camera (x,y,z): (" << pix/100 << "," << piy/100 << "," <<piz/100 << ")" << endl;
  cout << "Final position from camera (x,y,z): (" << pfx/100 << "," << pfy/100 << "," <<pfz/100 << ")" << endl;
  
  ALValue theta0 = ALValue::array(0.0f);
  vector<float> headj = motion.getAngles(ALValue::array("HeadYaw", "HeadPitch"),true);
  ALValue Headj = ALValue::array(headj[0],headj[1]);
  Transform head = HeadForwardHipBase(headj, theta0);
  Position3D PIhb = head*Position3D(pix/100,piy/100,(piz/100)+0.000f);
  Position3D PFhb = head*Position3D((pfx/100),pfy/100,(pfz/100)-0.000f);
  
  cout << "Transformed initial position from camera (x,y,z): (" << PIhb.x << "," << PIhb.y << "," << PIhb.z << ")" << endl;
  cout << "Transformed Final position from camera (x,y,z): (" << PFhb.x << "," << PFhb.y << "," << PFhb.z << ")" << endl;

  ALValue angle00 = ALValue::array(0.0f);
  vector<float> thetahip0(1,0.0f);
  angle00.ToFloatArray(thetahip0,false);
  Transform hbTtb = (Transform::fromRotY(thetahip0[0]))*Atheta0;

  Position6D TorsoBase = motion.getPosition("Torso",2,true);
  Position3D TorsoBaseXYZ = Position3D(TorsoBase.x,TorsoBase.y,TorsoBase.z);
  Position3D HipBaseXYZ = hbTtb*TorsoBaseXYZ;

  cout << "Torso Base (x,y,z): (" << TorsoBaseXYZ.x << "," << TorsoBaseXYZ.y << "," << TorsoBaseXYZ.z << ")" << endl;
  cout << "Hip Base (x,y,z): (" << HipBaseXYZ.x << "," << HipBaseXYZ.y << "," << HipBaseXYZ.z << ")" << endl;

  float TablePlusPeonHeight = 0.260f;
  float TorsoBaseHeight = TorsoBaseXYZ.z;
  float HipBaseHeight = 0.0f;
  float pickZ = 0.0f;

  HipBaseHeight = TorsoBaseHeight-l0;
  pickZ = TablePlusPeonHeight-HipBaseHeight;
  //cout << "pickZ : " << pickZ << endl;

  ALValue angle00 = ALValue::array(0.0f);
  angle00 = motion.getAngles(ALValue::array("LHipPitch"),true);
  vector<float> angle00v(1,0.0f);
  angle00.ToFloatArray(angle00v,false);
  ALValue angle0 = ALValue::array(-0.69778f-angle00v[0]);
  cout << "Angle0: " << -0.69778f-angle00v[0] << endl;

  //C1 to C3
  if (state == 7) {
   try {
	cout << "LArm C1 to C3" << endl;
	//Variables
	int i;
	//ALValue angle0 = ALValue::array(0.0f);
	float handspeed = 0.125f;
	bool isAbsolute = true;

	ALValue namesL      = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHipPitch", "RHipPitch");
	ALValue namesR	    = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "LHipPitch", "RHipPitch");
	ALValue ArmStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Position6D POL121,POL122,POL123,POPrG,POG,POPrP,POP,POoP,POL671,POL672,POL673,POL674;
	ALValue angleL121	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL122	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL123	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL671	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL672	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL673	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL674	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleLists12, angleLists23, angleLists34, angleLists45, angleLists56, angleLists67;
	ALValue timeLists12, timeLists23, timeLists34, timeLists45, timeLists56, timeLists67;
	vector<Position6D> POL23(9,Position6D());
	vector<ALValue> angleL23(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL23		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL34(9,Position6D());
	vector<ALValue> angleL34(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL34		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL45(9,Position6D());
	vector<ALValue> angleL45(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL45		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL56(9,Position6D());
	vector<ALValue> angleL56(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL56		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Transform T;
	Position6D P;
	ALValue joints = ALValue::array(0.0f,0.0f,0.0f,0.0f,0.0f);
	ALValue theta0 = ALValue::array(0.0f);
	vector<float> theta0v(1,0.0f);

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.9f,0.9f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.1f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));

	//initialization
	//1 to 2
	POL121 = Position6D(0.00143129f,0.313372f,0.0978128f,0.00195577f,0.386458f,1.56388f);
	angleL121 = LArmInverseHipBase(PosOrient2Matrix(POL121),angle0);
	POL122 = Position6D(0.136652f,0.229993f,0.28055f,-1.84499f,-0.572716f,0.427814f);
	angleL122 = LArmInverseHipBase(PosOrient2Matrix(POL122),angle0);
	POL123 = Position6D(0.157484f,0.128542f,0.29702f,0.0423857f,-0.259745f,0.120411f);
	angleL123 = LArmInverseHipBase(PosOrient2Matrix(POL123),angle0);

	//2 to 3
	POPrG = POL123;
	POG = Position6D(PIhb.x,PIhb.y,PIhb.z,0.088634f,0.738837f,0.118033f);

	POL23 = TrajectoryGenerator(POPrG,POG, 4.0f);
	timeL23 = TimeGenerator(POL23, 4.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL23[" << i << "]: " << POL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL23[" << i << "]: " << timeL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL23[i] = LArmInverseHipBase(PosOrient2Matrix(POL23[i]),angle0);
	}

	//3 to 4
	POPrP = Position6D(0.150622f,0.100506f,0.275484f,-0.0244068f,-0.0399511f,-0.0914423f);

	POL34 = TrajectoryGeneratorReverse(POG,POPrP,2.0f);
	timeL34 = TimeGeneratorReverse(POL34,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL34[" << i << "]: " << POL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL34[" << i << "]: " << timeL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL34[i] = LArmInverseHipBase(PosOrient2Matrix(POL34[i]),angle0);
	}

	//4 to 5
	POP = Position6D(PFhb.x,PFhb.y,PFhb.z,-0.31477f,0.687766f,-0.562191f);

	POL45 = TrajectoryGenerator(POPrP,POP,2.0f);
	timeL45 = TimeGenerator(POL45,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL45[" << i << "]: " << POL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL45[" << i << "]: " << timeL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL45[i] = LArmInverseHipBase(PosOrient2Matrix(POL45[i]),angle0);
	}

	//5 to 6
	POoP = Position6D(0.149791f,0.0877184f,0.283815f,0.0736116f,-0.116345f,-0.122399f);

	POL56 = TrajectoryGeneratorReverse(POP,POoP,3.0f);
	timeL56 = TimeGeneratorReverse(POL56,3.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL56[" << i << "]: " << POL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL56[" << i << "]: " << timeL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL56[i] = LArmInverseHipBase(PosOrient2Matrix(POL56[i]),angle0);
	}

	//6 to 7
	POL671 = Position6D(0.149791f,0.0877184f,0.283815f,0.0736116f,-0.116345f,-0.122399f);
	angleL671 = LArmInverseHipBase(PosOrient2Matrix(POL671),angle0);
	POL672 = Position6D(0.0939287f,0.269753f,0.29629f,-2.04261f,-0.603583f,0.92518f);
	angleL672 = LArmInverseHipBase(PosOrient2Matrix(POL672),angle0);
	POL673 = Position6D(0.00500832f,0.317509f,0.107616f,-0.0302126f,0.327381f,1.54641f);
	angleL673 = LArmInverseHipBase(PosOrient2Matrix(POL673),angle0);
	POL674 = Position6D(-0.00259649f,0.132217f,-0.0326618f,0.0906276f,1.45813f,1.67902f);
	angleL674 = LArmInverseHipBase(PosOrient2Matrix(POL674),angle0);

	//Angle and Time Listing
	//1 to 2
	angleLists12.clear();
	angleLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists12[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists12[i] = ALValue::array(angleL121[i], angleL122[i], angleL123[i]);
		}
	}

	timeLists12.clear();
	timeLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists12[i] = ALValue::array(2.0f, 3.0f, 5.0f);
	}

	//2 to 3
	angleLists23.clear();
	angleLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists23[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists23[i] = ALValue::array(angleL23[1][i],angleL23[2][i],angleL23[3][i],angleL23[4][i],angleL23[5][i],angleL23[6][i],angleL23[7][i],angleL23[8][i]);
		}
	}

	timeLists23.clear();
	timeLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists23[i] = ALValue::array(timeL23[1],timeL23[2],timeL23[3],timeL23[4],timeL23[5],timeL23[6],timeL23[7],timeL23[8]);
	}

	//3 to 4
	angleLists34.clear();
	angleLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists34[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists34[i] = ALValue::array(angleL34[1][i],angleL34[2][i],angleL34[3][i],angleL34[4][i],angleL34[5][i],angleL34[6][i],angleL34[7][i],angleL34[8][i]);
		}
	}

	timeLists34.clear();
	timeLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists34[i] = ALValue::array(timeL34[1],timeL34[2],timeL34[3],timeL34[4],timeL34[5],timeL34[6],timeL34[7],timeL34[8]);
	}

	//4 to 5
	angleLists45.clear();
	angleLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists45[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists45[i] = ALValue::array(angleL45[1][i],angleL45[2][i],angleL45[3][i],angleL45[4][i],angleL45[5][i],angleL45[6][i],angleL45[7][i],angleL45[8][i]);
		}
	}

	timeLists45.clear();
	timeLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists45[i] = ALValue::array(timeL45[1],timeL45[2],timeL45[3],timeL45[4],timeL45[5],timeL45[6],timeL45[7],timeL45[8]);
	}

	//5 to 6
	angleLists56.clear();
	angleLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists56[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists56[i] = ALValue::array(angleL56[1][i],angleL56[2][i],angleL56[3][i],angleL56[4][i],angleL56[5][i],angleL56[6][i],angleL56[7][i],angleL56[8][i]);
		}
	}

	timeLists56.clear();
	timeLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists56[i] = ALValue::array(timeL56[1],timeL56[2],timeL56[3],timeL56[4],timeL56[5],timeL56[6],timeL56[7],timeL56[8]);
	}

	//6 to 7
	angleLists67.clear();
	angleLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists67[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists67[i] = ALValue::array(angleL671[i], angleL672[i],angleL673[i],angleL674[i]);
		}
	}

	timeLists67.clear();
	timeLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists67[i] = ALValue::array(2.0f, 3.0f, 5.0f,7.0f);
	}

	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);

	//Execution
	motion.setStiffnesses(namesL,ArmStiffOn);
	motion.setStiffnesses("LHand", 0.8f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists12, timeLists12, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.69778f, -0.69778f, 0.65f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists23, timeLists23, isAbsolute);
		joints.clear();
		theta0.clear();
		joints = motion.getAngles(ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw"),true);
		theta0 = motion.getAngles(ALValue::array("LHipPitch"),true);
		theta0.ToFloatArray(theta0v,false);
		cout << "Theta0: " << -0.69778f-theta0v[0] << endl;
		T = LArmForwardHipBase(joints, ALValue::array(-0.69778f-theta0v[0]));
		P = Matrix2PosOrient(T);
		cout << "Grasp position and orientation: (" << P.x << "," << P.y << "," << P.z << P.wx << "," << P.wy << "," << P.wz << ")" << endl;
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.69778f, -0.69778f, 0.25f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists34, timeLists34, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists45, timeLists45, isAbsolute);
		joints.clear();
		theta0.clear();
		joints = motion.getAngles(ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw"),true);
		theta0 = motion.getAngles(ALValue::array("LHipPitch"),true);
		theta0.ToFloatArray(theta0v,false);
		cout << "Theta0: " << -0.69778f-theta0v[0] << endl;
		T = LArmForwardHipBase(joints, ALValue::array(-0.69778f-theta0v[0]));
		P = Matrix2PosOrient(T);
		cout << "Grasp position and orientation: (" << P.x << "," << P.y << "," << P.z << P.wx << "," << P.wy << "," << P.wz << ")" << endl;
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.69778f, -0.69778f, 0.65f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists56, timeLists56, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.69778f, -0.69778f, 0.25f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists67, timeLists67, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	motion.setStiffnesses(namesR,ArmStiffOff);
	motion.setStiffnesses("LHand", 0.0f);
  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }

    //C2 to C4
  if (state == 8) {
   try {
	cout << "LArm C2 to C4" << endl;
	//Variables
	int i;
	//ALValue angle0 = ALValue::array(0.0f);
	float handspeed = 0.125f;
	bool isAbsolute = true;
	ALValue namesL     = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHipPitch", "RHipPitch");
	ALValue namesR	   = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "LHipPitch", "RHipPitch");
	ALValue ArmStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Position6D POL121,POL122,POL123,POPrG,POG,POPrP,POP,POoP,POL671,POL672,POL673,POL674;
	ALValue angleL121	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL122	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL123	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL671	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL672	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL673	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL674	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleLists12, angleLists23, angleLists34, angleLists45, angleLists56, angleLists67;
	ALValue timeLists12, timeLists23, timeLists34, timeLists45, timeLists56, timeLists67;
	vector<Position6D> POL23(9,Position6D());
	vector<ALValue> angleL23(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL23		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL34(9,Position6D());
	vector<ALValue> angleL34(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL34		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL45(9,Position6D());
	vector<ALValue> angleL45(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL45		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL56(9,Position6D());
	vector<ALValue> angleL56(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL56		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.9f,0.9f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.1f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));

	//initialization
	//1 to 2
	POL121 = Position6D(0.00143129f,0.313372f,0.0978128f,0.00195577f,0.386458f,1.56388f);
	angleL121 = LArmInverseHipBase(PosOrient2Matrix(POL121),angle0);
	POL122 = Position6D(0.136652f,0.229993f,0.28055f,-1.84499f,-0.572716f,0.427814f);
	angleL122 = LArmInverseHipBase(PosOrient2Matrix(POL122),angle0);
	POL123 = Position6D(0.125734f,0.0868246f,0.331496f,0.0896631f,-0.506006f,-0.144803f);
	angleL123 = LArmInverseHipBase(PosOrient2Matrix(POL123),angle0);

	//2 to 3
	POPrG = POL123;
	POG = Position6D(PIhb.x,PIhb.y,PIhb.z,-0.0318979f,0.737462f,-0.153589f);

	POL23 = TrajectoryGenerator(POPrG,POG, 4.0f);
	timeL23 = TimeGenerator(POL23, 4.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL23[" << i << "]: " << POL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL23[" << i << "]: " << timeL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL23[i] = LArmInverseHipBase(PosOrient2Matrix(POL23[i]),angle0);
	}

	//3 to 4
	POPrP = Position6D(0.144556f,0.0576853f,0.263665f,-0.0471818f,0.0471284f,-0.377006f);

	POL34 = TrajectoryGeneratorReverse(POG,POPrP,2.0f);
	timeL34 = TimeGeneratorReverse(POL34,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL34[" << i << "]: " << POL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL34[" << i << "]: " << timeL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL34[i] = LArmInverseHipBase(PosOrient2Matrix(POL34[i]),angle0);
	}

	//4 to 5
	POP = Position6D(PFhb.x,PFhb.y,PFhb.z,-0.332808f,0.3278f,-1.01149f);
	//-0.358702f,0.346905f,-1.07784f
	//-0.281358f,0.286444f,-1.02029f
	POL45 = TrajectoryGenerator(POPrP,POP,2.0f);
	timeL45 = TimeGenerator(POL45,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL45[" << i << "]: " << POL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL45[" << i << "]: " << timeL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL45[i] = LArmInverseHipBase(PosOrient2Matrix(POL45[i]),angle0);
	}

	//5 to 6
	POoP = Position6D(0.138421f,0.0178041f,0.28927f,0.358314f,-0.257501f,-0.806611f);

	POL56 = TrajectoryGeneratorReverse(POP,POoP,3.0f);
	timeL56 = TimeGeneratorReverse(POL56,3.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL56[" << i << "]: " << POL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL56[" << i << "]: " << timeL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL56[i] = LArmInverseHipBase(PosOrient2Matrix(POL56[i]),angle0);
	}

	//6 to 7
	POL671 = Position6D(0.149791f,0.0877184f,0.283815f,0.0736116f,-0.116345f,-0.122399f);
	angleL671 = LArmInverseHipBase(PosOrient2Matrix(POL671),angle0);
	POL672 = Position6D(0.0939287f,0.269753f,0.29629f,-2.04261f,-0.603583f,0.92518f);
	angleL672 = LArmInverseHipBase(PosOrient2Matrix(POL672),angle0);
	POL673 = Position6D(0.00500832f,0.317509f,0.107616f,-0.0302126f,0.327381f,1.54641f);
	angleL673 = LArmInverseHipBase(PosOrient2Matrix(POL673),angle0);
	POL674 = Position6D(-0.00259649f,0.132217f,-0.0326618f,0.0906276f,1.45813f,1.67902f);
	angleL674 = LArmInverseHipBase(PosOrient2Matrix(POL674),angle0);

	//Angle and Time Listing
	//1 to 2
	angleLists12.clear();
	angleLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists12[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists12[i] = ALValue::array(angleL121[i], angleL122[i], angleL123[i]);
		}
	}

	timeLists12.clear();
	timeLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists12[i] = ALValue::array(2.0f, 3.0f, 5.0f);
	}

	//2 to 3
	angleLists23.clear();
	angleLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists23[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists23[i] = ALValue::array(angleL23[1][i],angleL23[2][i],angleL23[3][i],angleL23[4][i],angleL23[5][i],angleL23[6][i],angleL23[7][i],angleL23[8][i]);
		}
	}

	timeLists23.clear();
	timeLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists23[i] = ALValue::array(timeL23[1],timeL23[2],timeL23[3],timeL23[4],timeL23[5],timeL23[6],timeL23[7],timeL23[8]);
	}

	//3 to 4
	angleLists34.clear();
	angleLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists34[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists34[i] = ALValue::array(angleL34[1][i],angleL34[2][i],angleL34[3][i],angleL34[4][i],angleL34[5][i],angleL34[6][i],angleL34[7][i],angleL34[8][i]);
		}
	}

	timeLists34.clear();
	timeLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists34[i] = ALValue::array(timeL34[1],timeL34[2],timeL34[3],timeL34[4],timeL34[5],timeL34[6],timeL34[7],timeL34[8]);
	}

	//4 to 5
	angleLists45.clear();
	angleLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists45[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists45[i] = ALValue::array(angleL45[1][i],angleL45[2][i],angleL45[3][i],angleL45[4][i],angleL45[5][i],angleL45[6][i],angleL45[7][i],angleL45[8][i]);
		}
	}

	timeLists45.clear();
	timeLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists45[i] = ALValue::array(timeL45[1],timeL45[2],timeL45[3],timeL45[4],timeL45[5],timeL45[6],timeL45[7],timeL45[8]);
	}

	//5 to 6
	angleLists56.clear();
	angleLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists56[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists56[i] = ALValue::array(angleL56[1][i],angleL56[2][i],angleL56[3][i],angleL56[4][i],angleL56[5][i],angleL56[6][i],angleL56[7][i],angleL56[8][i]);
		}
	}

	timeLists56.clear();
	timeLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists56[i] = ALValue::array(timeL56[1],timeL56[2],timeL56[3],timeL56[4],timeL56[5],timeL56[6],timeL56[7],timeL56[8]);
	}

	//6 to 7
	angleLists67.clear();
	angleLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists67[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists67[i] = ALValue::array(angleL671[i], angleL672[i],angleL673[i],angleL674[i]);
		}
	}

	timeLists67.clear();
	timeLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists67[i] = ALValue::array(2.0f, 3.0f, 5.0f,7.0f);
	}

	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);

	//Execution
	motion.setStiffnesses(namesL,ArmStiffOn);
	motion.setStiffnesses("LHand", 0.8f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists12, timeLists12, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.69778f, -0.69778f, 0.65f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists23, timeLists23, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.69778f, -0.69778f, 0.25f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists34, timeLists34, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists45, timeLists45, isAbsolute);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.69778f, -0.69778f, 0.65f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists56, timeLists56, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.69778f, -0.69778f, 0.25f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists67, timeLists67, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	motion.setStiffnesses(namesR,ArmStiffOff);
	motion.setStiffnesses("LHand", 0.0f);
  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }

    //C7 to C5
  if (state == 9) {
   try {
	cout << "RArm C7 to C5" << endl;
	//Variables
	int i;
	//ALValue angle0 = ALValue::array(0.0f);
	float handspeed = 0.125f;
	bool isAbsolute = true;
	ALValue namesL     = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHipPitch", "RHipPitch");
	ALValue namesR	   = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "LHipPitch", "RHipPitch");
	ALValue ArmStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Position6D POR121,POR122,POR123,POPrG,POG,POPrP,POP,POoP,POR671,POR672,POR673,POR674;
	ALValue angleR121	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR122	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR123	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR671	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR672	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR673	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR674	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleLists12, angleLists23, angleLists34, angleLists45, angleLists56, angleLists67;
	ALValue timeLists12, timeLists23, timeLists34, timeLists45, timeLists56, timeLists67;
	vector<Position6D> POR23(9,Position6D());
	vector<ALValue> angleR23(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR23		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR34(9,Position6D());
	vector<ALValue> angleR34(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR34		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR45(9,Position6D());
	vector<ALValue> angleR45(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR45		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR56(9,Position6D());
	vector<ALValue> angleR56(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR56		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.9f,0.9f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.1f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));

	//initialization
	//1 to 2
	POR121 = Position6D(0.00474005f,-0.313378f,0.098708f,0.0449575f,0.396628f,-1.54895f);
	angleR121 = RArmInverseHipBase(PosOrient2Matrix(POR121),angle0);
	POR122 = Position6D(0.145036f,-0.242922f,0.260995f,1.92265f,-0.437766f,-0.539707f);
	angleR122 = RArmInverseHipBase(PosOrient2Matrix(POR122),angle0);
	POR123 = Position6D(0.140234f,-0.123545f,0.320269f,0.0116885f,-0.420936f,-0.0909166f);
	angleR123 = RArmInverseHipBase(PosOrient2Matrix(POR123),angle0);

	//2 to 3
	POPrG = POR123;
	POG = Position6D(PIhb.x,PIhb.y,PIhb.z,-0.0300307f,0.771494f,-0.164747f);

	POR23 = TrajectoryGenerator(POPrG,POG, 4.0f);
	timeR23 = TimeGenerator(POR23, 4.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR23[" << i << "]: " << POR23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR23[" << i << "]: " << timeR23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR23[i] = RArmInverseHipBase(PosOrient2Matrix(POR23[i]),angle0);
	}

	//3 to 4
	POPrP = Position6D(0.151514f,-0.0977895f,0.272353f,-0.0196901f,-0.0180567f,0.0636983f);

	POR34 = TrajectoryGeneratorReverse(POG,POPrP,2.0f);
	timeR34 = TimeGeneratorReverse(POR34,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR34[" << i << "]: " << POR34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR34[" << i << "]: " << timeR34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR34[i] = RArmInverseHipBase(PosOrient2Matrix(POR34[i]),angle0);
	}

	//4 to 5
	POP = Position6D(PFhb.x,PFhb.y,PFhb.z,0.382835f,0.680849f,0.589045f);

	POR45 = TrajectoryGenerator(POPrP,POP,2.0f);
	timeR45 = TimeGenerator(POR45,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR45[" << i << "]: " << POR45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR45[" << i << "]: " << timeR45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR45[i] = RArmInverseHipBase(PosOrient2Matrix(POR45[i]),angle0);
	}

	//5 to 6
	POoP = Position6D(0.147094f,-0.0573662f,0.292511f,-0.0133812f,-0.214035f,0.403936f);

	POR56 = TrajectoryGeneratorReverse(POP,POoP,3.0f);
	timeR56 = TimeGeneratorReverse(POR56,3.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR56[" << i << "]: " << POR56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR56[" << i << "]: " << timeR56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR56[i] = RArmInverseHipBase(PosOrient2Matrix(POR56[i]),angle0);
	}

	//6 to 7
	POR671 = Position6D(0.140234f,-0.123545f,0.320269f,0.0116885f,-0.420936f,-0.0909166f);
	angleR671 = RArmInverseHipBase(PosOrient2Matrix(POR671),angle0);
	POR672 = Position6D(0.132275f,-0.255334f,0.259675f,1.69482f,-0.366011f,-0.607543f);
	angleR672 = RArmInverseHipBase(PosOrient2Matrix(POR672),angle0);
	POR673 = Position6D(0.00286376f,-0.315768f,0.103424f,0.0436051f,0.355651f,-1.55808f);
	angleR673 = RArmInverseHipBase(PosOrient2Matrix(POR673),angle0);
	POR674 = Position6D(0.00658906f,-0.132896f,-0.032212f,0.289373f,1.46564f,-1.29847f);
	angleR674 = RArmInverseHipBase(PosOrient2Matrix(POR674),angle0);

	//Angle and Time Listing
	//1 to 2
	angleLists12.clear();
	angleLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists12[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists12[i] = ALValue::array(angleR121[i], angleR122[i], angleR123[i]);
		}
	}

	timeLists12.clear();
	timeLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists12[i] = ALValue::array(2.0f, 3.0f, 5.0f);
	}

	//2 to 3
	angleLists23.clear();
	angleLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists23[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists23[i] = ALValue::array(angleR23[1][i],angleR23[2][i],angleR23[3][i],angleR23[4][i],angleR23[5][i],angleR23[6][i],angleR23[7][i],angleR23[8][i]);
		}
	}

	timeLists23.clear();
	timeLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists23[i] = ALValue::array(timeR23[1],timeR23[2],timeR23[3],timeR23[4],timeR23[5],timeR23[6],timeR23[7],timeR23[8]);
	}

	//3 to 4
	angleLists34.clear();
	angleLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists34[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists34[i] = ALValue::array(angleR34[1][i],angleR34[2][i],angleR34[3][i],angleR34[4][i],angleR34[5][i],angleR34[6][i],angleR34[7][i],angleR34[8][i]);
		}
	}

	timeLists34.clear();
	timeLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists34[i] = ALValue::array(timeR34[1],timeR34[2],timeR34[3],timeR34[4],timeR34[5],timeR34[6],timeR34[7],timeR34[8]);
	}

	//4 to 5
	angleLists45.clear();
	angleLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists45[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists45[i] = ALValue::array(angleR45[1][i],angleR45[2][i],angleR45[3][i],angleR45[4][i],angleR45[5][i],angleR45[6][i],angleR45[7][i],angleR45[8][i]);
		}
	}

	timeLists45.clear();
	timeLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists45[i] = ALValue::array(timeR45[1],timeR45[2],timeR45[3],timeR45[4],timeR45[5],timeR45[6],timeR45[7],timeR45[8]);
	}

	//5 to 6
	angleLists56.clear();
	angleLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists56[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists56[i] = ALValue::array(angleR56[1][i],angleR56[2][i],angleR56[3][i],angleR56[4][i],angleR56[5][i],angleR56[6][i],angleR56[7][i],angleR56[8][i]);
		}
	}

	timeLists56.clear();
	timeLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists56[i] = ALValue::array(timeR56[1],timeR56[2],timeR56[3],timeR56[4],timeR56[5],timeR56[6],timeR56[7],timeR56[8]);
	}

	//6 to 7
	angleLists67.clear();
	angleLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists67[i] = ALValue::array(-0.69778f, -0.69778f, -0.69778f, -0.69778f);
		} 
		else
		{
			angleLists67[i] = ALValue::array(angleR671[i], angleR672[i],angleR673[i],angleR674[i]);
		}
	}

	timeLists67.clear();
	timeLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists67[i] = ALValue::array(2.0f, 3.0f, 5.0f,7.0f);
	}

	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);

	//Execution
	motion.setStiffnesses(namesR,ArmStiffOn);
	motion.setStiffnesses("RHand", 0.8f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists12, timeLists12, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.69778f, -0.69778f, 0.65f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists23, timeLists23, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.69778f, -0.69778f, 0.25f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesR, angleLists34, timeLists34, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists45, timeLists45, isAbsolute);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.69778f, -0.69778f, 0.65f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesR, angleLists56, timeLists56, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.69778f, -0.69778f, 0.25f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists67, timeLists67, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	motion.setStiffnesses(namesR,ArmStiffOff);
	motion.setStiffnesses("RHand", 0.0f);


  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }

  //B7 to B5
  if (state == 6) {
   try {
	cout << "RArm B7 to B5" << endl;
	//Variables
	int i;
	//ALValue angle0 = ALValue::array(0.13955f);
	float handspeed = 0.125f;
	bool isAbsolute = true;
	ALValue namesL     = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHipPitch", "RHipPitch");
	ALValue namesR	   = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "LHipPitch", "RHipPitch");
	ALValue ArmStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Position6D POR121,POR122,POR123,POPrG,POG,POPrP,POP,POoP,POR671,POR672,POR673,POR674;
	ALValue angleR121	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR122	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR123	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR671	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR672	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR673	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR674	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleLists12, angleLists23, angleLists34, angleLists45, angleLists56, angleLists67;
	ALValue timeLists12, timeLists23, timeLists34, timeLists45, timeLists56, timeLists67;
	vector<Position6D> POR23(9,Position6D());
	vector<ALValue> angleR23(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR23		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR34(9,Position6D());
	vector<ALValue> angleR34(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR34		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR45(9,Position6D());
	vector<ALValue> angleR45(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR45		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR56(9,Position6D());
	vector<ALValue> angleR56(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR56		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	//HipPitch at 0.13955 rad (act = -0.83733 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.9f,0.9f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.8373f), 0.1f);
	qi::os::msleep(500);
	//motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));

	//initialization
	//1 to 2
	POR121 = Position6D(0.0201602f,-0.316299f,0.103024f,-0.0558378f,0.347126f,-1.5982f);
	angleR121 = RArmInverseHipBase(PosOrient2Matrix(POR121),angle0);
	POR122 = Position6D(0.18126f,-0.2481f,0.224048f,1.75529f,-0.217556f,-0.540241f);
	angleR122 = RArmInverseHipBase(PosOrient2Matrix(POR122),angle0);
	POR123 = Position6D(0.175929f,-0.130308f,00.313943,0.0294211f,-0.415198f,0-0.131393f);
	angleR123 = RArmInverseHipBase(PosOrient2Matrix(POR123),angle0);

	//2 to 3
	POPrG = POR123;
	POG = Position6D(PIhb.x,PIhb.y,PIhb.z,0.0288319f,0.643926f,-0.207146f);

	POR23 = TrajectoryGenerator(POPrG,POG, 4.0f);
	timeR23 = TimeGenerator(POR23, 4.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR23[" << i << "]: " << POR23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR23[" << i << "]: " << timeR23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR23[i] = RArmInverseHipBase(PosOrient2Matrix(POR23[i]),angle0);
	}

	//3 to 4
	//POPrP = Position6D(0.199509f,-0.0999644f,0.247197f,0.0149151,0.0754931f,0.0571058f);
	POPrP = Position6D(0.175929f,-0.130308f,00.313943,0.0294211f,-0.415198f,0-0.131393f);
	POR34 = TrajectoryGeneratorReverse(POG,POPrP,2.0f);
	timeR34 = TimeGeneratorReverse(POR34,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR34[" << i << "]: " << POR34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR34[" << i << "]: " << timeR34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR34[i] = RArmInverseHipBase(PosOrient2Matrix(POR34[i]),angle0);
	}

	//4 to 5
	POP = Position6D(PFhb.x,PFhb.y,PFhb.z,0.297705f,00.60046f,0.459395f);

	POR45 = TrajectoryGenerator(POPrP,POP,2.0f);
	timeR45 = TimeGenerator(POR45,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR45[" << i << "]: " << POR45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR45[" << i << "]: " << timeR45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR45[i] = RArmInverseHipBase(PosOrient2Matrix(POR45[i]),angle0);
	}

	//5 to 6
	POoP = Position6D(0.1803f,-0.0545408f,0.273972f,-0.0283267f,-0.0986127f,0.37857f);

	POR56 = TrajectoryGeneratorReverse(POP,POoP,3.0f);
	timeR56 = TimeGeneratorReverse(POR56,3.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR56[" << i << "]: " << POR56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR56[" << i << "]: " << timeR56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR56[i] = RArmInverseHipBase(PosOrient2Matrix(POR56[i]),angle0);
	}

	//6 to 7
	POR671 = Position6D(0.175929f,-0.130308f,00.313943,0.0294211f,-0.415198f,0-0.131393f);
	angleR671 = RArmInverseHipBase(PosOrient2Matrix(POR671),angle0);
	POR672 = Position6D(0.190027f,-0.230384f,0.246914f,1.64536f,-0.331751f,-0.469542f);
	angleR672 = RArmInverseHipBase(PosOrient2Matrix(POR672),angle0);
	POR673 = Position6D(0.0219401f,-0.314626f,0.0988801f,-0.0258363f,0.369215f,-1.58817f);
	angleR673 = RArmInverseHipBase(PosOrient2Matrix(POR673),angle0);
	POR674 = Position6D(0.0142085f,0-0.129252f,-0.0341076f,-0.516501f,1.47137f,-2.12411f);
	angleR674 = RArmInverseHipBase(PosOrient2Matrix(POR674),angle0);

	//Angle and Time Listing
	//1 to 2
	angleLists12.clear();
	angleLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists12[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists12[i] = ALValue::array(angleR121[i], angleR122[i], angleR123[i]);
		}
	}

	timeLists12.clear();
	timeLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists12[i] = ALValue::array(2.0f, 3.0f, 5.0f);
	}

	//2 to 3
	angleLists23.clear();
	angleLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists23[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists23[i] = ALValue::array(angleR23[1][i],angleR23[2][i],angleR23[3][i],angleR23[4][i],angleR23[5][i],angleR23[6][i],angleR23[7][i],angleR23[8][i]);
		}
	}

	timeLists23.clear();
	timeLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists23[i] = ALValue::array(timeR23[1],timeR23[2],timeR23[3],timeR23[4],timeR23[5],timeR23[6],timeR23[7],timeR23[8]);
	}

	//3 to 4
	angleLists34.clear();
	angleLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists34[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists34[i] = ALValue::array(angleR34[1][i],angleR34[2][i],angleR34[3][i],angleR34[4][i],angleR34[5][i],angleR34[6][i],angleR34[7][i],angleR34[8][i]);
		}
	}

	timeLists34.clear();
	timeLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists34[i] = ALValue::array(timeR34[1],timeR34[2],timeR34[3],timeR34[4],timeR34[5],timeR34[6],timeR34[7],timeR34[8]);
	}

	//4 to 5
	angleLists45.clear();
	angleLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists45[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists45[i] = ALValue::array(angleR45[1][i],angleR45[2][i],angleR45[3][i],angleR45[4][i],angleR45[5][i],angleR45[6][i],angleR45[7][i],angleR45[8][i]);
		}
	}

	timeLists45.clear();
	timeLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists45[i] = ALValue::array(timeR45[1],timeR45[2],timeR45[3],timeR45[4],timeR45[5],timeR45[6],timeR45[7],timeR45[8]);
	}

	//5 to 6
	angleLists56.clear();
	angleLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists56[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists56[i] = ALValue::array(angleR56[1][i],angleR56[2][i],angleR56[3][i],angleR56[4][i],angleR56[5][i],angleR56[6][i],angleR56[7][i],angleR56[8][i]);
		}
	}

	timeLists56.clear();
	timeLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists56[i] = ALValue::array(timeR56[1],timeR56[2],timeR56[3],timeR56[4],timeR56[5],timeR56[6],timeR56[7],timeR56[8]);
	}

	//6 to 7
	angleLists67.clear();
	angleLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists67[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists67[i] = ALValue::array(angleR671[i], angleR672[i],angleR673[i],angleR674[i]);
		}
	}

	timeLists67.clear();
	timeLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists67[i] = ALValue::array(2.0f, 3.0f, 5.0f,7.0f);
	}

	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);

	//Execution
	motion.setStiffnesses(namesR,ArmStiffOn);
	motion.setStiffnesses("RHand", 0.8f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists12, timeLists12, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.83733f, -0.83733f, 0.65f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists23, timeLists23, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.83733f, -0.83733f, 0.25f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesR, angleLists34, timeLists34, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists45, timeLists45, isAbsolute);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.83733f, -0.83733f, 0.65f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesR, angleLists56, timeLists56, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.83733f, -0.83733f, 0.25f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists67, timeLists67, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	motion.setStiffnesses(namesR,ArmStiffOff);
	motion.setStiffnesses("RHand", 0.0f);

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.1f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));
  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }

  //B6 to B4
  if (state == 5) {
   try {
	cout << "RArm B6 to B4" << endl;
	//Variables
	int i;
	//ALValue angle0 = ALValue::array(0.13955f);
	float handspeed = 0.125f;
	bool isAbsolute = true;
	ALValue namesL     = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHipPitch", "RHipPitch");
	ALValue namesR	   = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "LHipPitch", "RHipPitch");
	ALValue ArmStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Position6D POR121,POR122,POR123,POPrG,POG,POPrP,POP,POoP,POR671,POR672,POR673,POR674;
	ALValue angleR121	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR122	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR123	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR671	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR672	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR673	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR674	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleLists12, angleLists23, angleLists34, angleLists45, angleLists56, angleLists67;
	ALValue timeLists12, timeLists23, timeLists34, timeLists45, timeLists56, timeLists67;
	vector<Position6D> POR23(9,Position6D());
	vector<ALValue> angleR23(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR23		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR34(9,Position6D());
	vector<ALValue> angleR34(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR34		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR45(9,Position6D());
	vector<ALValue> angleR45(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR45		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR56(9,Position6D());
	vector<ALValue> angleR56(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR56		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	//HipPitch at 0.13955 rad (act = -0.83733 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.9f,0.9f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.8373f), 0.1f);
	qi::os::msleep(500);
	//motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));

	//initialization
	//1 to 2
	POR121 = Position6D(0.0201602f,-0.316299f,0.103024f,-0.0558378f,0.347126f,-1.5982f);
	angleR121 = RArmInverseHipBase(PosOrient2Matrix(POR121),angle0);
	POR122 = Position6D(0.18126f,-0.2481f,0.224048f,1.75529f,-0.217556f,-0.540241f);
	angleR122 = RArmInverseHipBase(PosOrient2Matrix(POR122),angle0);
	POR123 = Position6D(0.182015f,-0.0938233f,0.31092f,0.0233817f,-0.414817f,0.124915f);
	angleR123 = RArmInverseHipBase(PosOrient2Matrix(POR123),angle0);

	//2 to 3
	POPrG = POR123;
	POG = Position6D(PIhb.x,PIhb.y,PIhb.z,0.178757f,0.635046f,0.123398f);

	POR23 = TrajectoryGenerator(POPrG,POG, 4.0f);
	timeR23 = TimeGenerator(POR23, 4.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR23[" << i << "]: " << POR23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR23[" << i << "]: " << timeR23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR23[i] = RArmInverseHipBase(PosOrient2Matrix(POR23[i]),angle0);
	}

	//3 to 4
	POPrP = Position6D(0.190998f,-0.0491824f,0.2445f,-0.0495836f,0.0494769f,0.551564f);
	//0.24498,0-0.0684678,00.187064,00.149953,00.336374,00.30526
	POR34 = TrajectoryGeneratorReverse(POG,POPrP,2.0f);
	timeR34 = TimeGeneratorReverse(POR34,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR34[" << i << "]: " << POR34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR34[" << i << "]: " << timeR34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR34[i] = RArmInverseHipBase(PosOrient2Matrix(POR34[i]),angle0);
	}

	//4 to 5
	//POP = Position6D(0.199147f,-0.0144103f,0.138222f,0.235805f,0.36416f,0.759429f);
	//POP = Position6D(0.199784f,-0.00627046f,0.142048f,0.118756f,0.138687f,0.846524f);
	POP = Position6D(PFhb.x,PFhb.y,PFhb.z,0.235805f,0.36416f,0.759429f);

	POR45 = TrajectoryGenerator(POPrP,POP,2.0f);
	timeR45 = TimeGenerator(POR45,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR45[" << i << "]: " << POR45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR45[" << i << "]: " << timeR45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR45[i] = RArmInverseHipBase(PosOrient2Matrix(POR45[i]),angle0);
	}

	//5 to 6
	POoP = Position6D(0.171576f,-0.00574453f,0.242595f,0.0945277f,0.0211236f,0.841678f);

	POR56 = TrajectoryGeneratorReverse(POP,POoP,3.0f);
	timeR56 = TimeGeneratorReverse(POR56,3.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR56[" << i << "]: " << POR56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR56[" << i << "]: " << timeR56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR56[i] = RArmInverseHipBase(PosOrient2Matrix(POR56[i]),angle0);
	}

	//6 to 7
	POR671 = Position6D(0.182015f,-0.0938233f,0.31092f,0.0233817f,-0.414817f,0.124915f);
	angleR671 = RArmInverseHipBase(PosOrient2Matrix(POR671),angle0);
	POR672 = Position6D(0.190027f,-0.230384f,0.246914f,1.64536f,-0.331751f,-0.469542f);
	angleR672 = RArmInverseHipBase(PosOrient2Matrix(POR672),angle0);
	POR673 = Position6D(0.0219401f,-0.314626f,0.0988801f,-0.0258363f,0.369215f,-1.58817f);
	angleR673 = RArmInverseHipBase(PosOrient2Matrix(POR673),angle0);
	POR674 = Position6D(0.0142085f,0-0.129252f,-0.0341076f,-0.516501f,1.47137f,-2.12411f);
	angleR674 = RArmInverseHipBase(PosOrient2Matrix(POR674),angle0);

	//Angle and Time Listing
	//1 to 2
	angleLists12.clear();
	angleLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists12[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists12[i] = ALValue::array(angleR121[i], angleR122[i], angleR123[i]);
		}
	}

	timeLists12.clear();
	timeLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists12[i] = ALValue::array(2.0f, 3.0f, 5.0f);
	}

	//2 to 3
	angleLists23.clear();
	angleLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists23[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists23[i] = ALValue::array(angleR23[1][i],angleR23[2][i],angleR23[3][i],angleR23[4][i],angleR23[5][i],angleR23[6][i],angleR23[7][i],angleR23[8][i]);
		}
	}

	timeLists23.clear();
	timeLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists23[i] = ALValue::array(timeR23[1],timeR23[2],timeR23[3],timeR23[4],timeR23[5],timeR23[6],timeR23[7],timeR23[8]);
	}

	//3 to 4
	angleLists34.clear();
	angleLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists34[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists34[i] = ALValue::array(angleR34[1][i],angleR34[2][i],angleR34[3][i],angleR34[4][i],angleR34[5][i],angleR34[6][i],angleR34[7][i],angleR34[8][i]);
		}
	}

	timeLists34.clear();
	timeLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists34[i] = ALValue::array(timeR34[1],timeR34[2],timeR34[3],timeR34[4],timeR34[5],timeR34[6],timeR34[7],timeR34[8]);
	}

	//4 to 5
	angleLists45.clear();
	angleLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists45[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists45[i] = ALValue::array(angleR45[1][i],angleR45[2][i],angleR45[3][i],angleR45[4][i],angleR45[5][i],angleR45[6][i],angleR45[7][i],angleR45[8][i]);
		}
	}

	timeLists45.clear();
	timeLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists45[i] = ALValue::array(timeR45[1],timeR45[2],timeR45[3],timeR45[4],timeR45[5],timeR45[6],timeR45[7],timeR45[8]);
	}

	//5 to 6
	angleLists56.clear();
	angleLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists56[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists56[i] = ALValue::array(angleR56[1][i],angleR56[2][i],angleR56[3][i],angleR56[4][i],angleR56[5][i],angleR56[6][i],angleR56[7][i],angleR56[8][i]);
		}
	}

	timeLists56.clear();
	timeLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists56[i] = ALValue::array(timeR56[1],timeR56[2],timeR56[3],timeR56[4],timeR56[5],timeR56[6],timeR56[7],timeR56[8]);
	}

	//6 to 7
	angleLists67.clear();
	angleLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists67[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists67[i] = ALValue::array(angleR671[i], angleR672[i],angleR673[i],angleR674[i]);
		}
	}

	timeLists67.clear();
	timeLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists67[i] = ALValue::array(2.0f, 3.0f, 5.0f,7.0f);
	}

	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);

	//Execution
	motion.setStiffnesses(namesR,ArmStiffOn);
	motion.setStiffnesses("RHand", 0.8f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists12, timeLists12, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.83733f, -0.83733f, 0.65f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists23, timeLists23, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.83733f, -0.83733f, 0.25f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesR, angleLists34, timeLists34, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists45, timeLists45, isAbsolute);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.83733f, -0.83733f, 0.65f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesR, angleLists56, timeLists56, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-0.83733f, -0.83733f, 0.25f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists67, timeLists67, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	motion.setStiffnesses(namesR,ArmStiffOff);
	motion.setStiffnesses("RHand", 0.0f);

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.1f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));
  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }

  //B1 to B3
  if (state == 4) {
   try {
	cout << "LArm B1 to B3" << endl;
	//Variables
	int i;
	//ALValue angle0	= ALValue::array(0.13955f);
	float handspeed = 0.125f;
	bool isAbsolute = true;
	ALValue namesL     = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHipPitch", "RHipPitch");
	ALValue namesR	   = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "LHipPitch", "RHipPitch");
	ALValue ArmStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Position6D POL121,POL122,POL123,POPrG,POG,POPrP,POP,POoP,POL671,POL672,POL673,POL674;
	ALValue angleL121	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL122	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL123	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL671	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL672	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL673	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL674	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleLists12, angleLists23, angleLists34, angleLists45, angleLists56, angleLists67;
	ALValue timeLists12, timeLists23, timeLists34, timeLists45, timeLists56, timeLists67;
	vector<Position6D> POL23(9,Position6D());
	vector<ALValue> angleL23(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL23		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL34(9,Position6D());
	vector<ALValue> angleL34(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL34		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL45(9,Position6D());
	vector<ALValue> angleL45(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL45		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL56(9,Position6D());
	vector<ALValue> angleL56(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL56		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.9f,0.9f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.1f);
	qi::os::msleep(500);
	//motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));

	//initialization
	//1 to 2
	POL121 = Position6D(0.024482f,0.318035f,0.106922f,0.0143637f,0.319004f,1.57602f);
	angleL121 = LArmInverseHipBase(PosOrient2Matrix(POL121),angle0);
	POL122 = Position6D(0.164806f,0.254173f,0.257f,2.04137f,0.430911f,0.652044f);
	angleL122 = LArmInverseHipBase(PosOrient2Matrix(POL122),angle0);
	POL123 = Position6D(0.185871f,0.130619f,0.286225f,0.0394488f,-0.188752f,0.130974f);
	angleL123 = LArmInverseHipBase(PosOrient2Matrix(POL123),angle0);

	//2 to 3
	POPrG = POL123;
	POG = Position6D(PIhb.x,PIhb.y,PIhb.z,0.246983f,0.66281f,0.109827f);

	POL23 = TrajectoryGenerator(POPrG,POG, 4.0f);
	timeL23 = TimeGenerator(POL23, 4.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL23[" << i << "]: " << POL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL23[" << i << "]: " << timeL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL23[i] = LArmInverseHipBase(PosOrient2Matrix(POL23[i]),angle0);
	}

	//3 to 4
	POPrP = Position6D(0.196927f,0.096762f,0.254552f,0.0915351f,0.0287834f,-0.0609783f);

	POL34 = TrajectoryGeneratorReverse(POG,POPrP,2.0f);
	timeL34 = TimeGeneratorReverse(POL34,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL34[" << i << "]: " << POL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL34[" << i << "]: " << timeL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL34[i] = LArmInverseHipBase(PosOrient2Matrix(POL34[i]),angle0);
	}

	//4 to 5
	POP = Position6D(PFhb.x,PFhb.y,PFhb.z,-0.257976f,0.555705f,-0.591817f);

	POL45 = TrajectoryGenerator(POPrP,POP,2.0f);
	timeL45 = TimeGenerator(POL45,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL45[" << i << "]: " << POL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL45[" << i << "]: " << timeL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL45[i] = LArmInverseHipBase(PosOrient2Matrix(POL45[i]),angle0);
	}

	//5 to 6
	POoP = Position6D(0.189243f,0.0349128f,0.255999f,0.0687366f,-0.0397619f,0-0.493962f);

	POL56 = TrajectoryGeneratorReverse(POP,POoP,3.0f);
	timeL56 = TimeGeneratorReverse(POL56,3.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL56[" << i << "]: " << POL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL56[" << i << "]: " << timeL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL56[i] = LArmInverseHipBase(PosOrient2Matrix(POL56[i]),angle0);
	}

	//6 to 7
	POL671 = Position6D(0.152038f,0.10917f,0.342962f,0.203887f,-0.651306f,0.0439805f);
	angleL671 = LArmInverseHipBase(PosOrient2Matrix(POL671),angle0);
	POL672 = Position6D(0.183568f,0.239311f,0.241518f,-1.69508f,-0.343652f,0.510554f);
	angleL672 = LArmInverseHipBase(PosOrient2Matrix(POL672),angle0);
	POL673 = Position6D(0.0335165f,0.318456f,0.108422f,-0.12204f,0.31122f,01.53778f);
	angleL673 = LArmInverseHipBase(PosOrient2Matrix(POL673),angle0);
	POL674 = Position6D(0.00818429f,0.133365f,-0.0341804f,1.91975f,1.49514f,1.87114f);
	angleL674 = LArmInverseHipBase(PosOrient2Matrix(POL674),angle0);

	//Angle and Time Listing
	//1 to 2
	angleLists12.clear();
	angleLists12.arraySetSize(10);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists12[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists12[i] = ALValue::array(angleL121[i], angleL122[i], angleL123[i]);
		}
	}

	timeLists12.clear();
	timeLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists12[i] = ALValue::array(2.0f, 3.0f, 5.0f);
	}

	//2 to 3
	angleLists23.clear();
	angleLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists23[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists23[i] = ALValue::array(angleL23[1][i],angleL23[2][i],angleL23[3][i],angleL23[4][i],angleL23[5][i],angleL23[6][i],angleL23[7][i],angleL23[8][i]);
		}
	}

	timeLists23.clear();
	timeLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists23[i] = ALValue::array(timeL23[1],timeL23[2],timeL23[3],timeL23[4],timeL23[5],timeL23[6],timeL23[7],timeL23[8]);
	}

	//3 to 4
	angleLists34.clear();
	angleLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists34[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists34[i] = ALValue::array(angleL34[1][i],angleL34[2][i],angleL34[3][i],angleL34[4][i],angleL34[5][i],angleL34[6][i],angleL34[7][i],angleL34[8][i]);
		}
	}

	timeLists34.clear();
	timeLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists34[i] = ALValue::array(timeL34[1],timeL34[2],timeL34[3],timeL34[4],timeL34[5],timeL34[6],timeL34[7],timeL34[8]);
	}

	//4 to 5
	angleLists45.clear();
	angleLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists45[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists45[i] = ALValue::array(angleL45[1][i],angleL45[2][i],angleL45[3][i],angleL45[4][i],angleL45[5][i],angleL45[6][i],angleL45[7][i],angleL45[8][i]);
		}
	}

	timeLists45.clear();
	timeLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists45[i] = ALValue::array(timeL45[1],timeL45[2],timeL45[3],timeL45[4],timeL45[5],timeL45[6],timeL45[7],timeL45[8]);
	}

	//5 to 6
	angleLists56.clear();
	angleLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists56[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists56[i] = ALValue::array(angleL56[1][i],angleL56[2][i],angleL56[3][i],angleL56[4][i],angleL56[5][i],angleL56[6][i],angleL56[7][i],angleL56[8][i]);
		}
	}

	timeLists56.clear();
	timeLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists56[i] = ALValue::array(timeL56[1],timeL56[2],timeL56[3],timeL56[4],timeL56[5],timeL56[6],timeL56[7],timeL56[8]);
	}

	//6 to 7
	angleLists67.clear();
	angleLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists67[i] = ALValue::array(-0.83733f, -0.83733f, -0.83733f, -0.83733f);
		} 
		else
		{
			angleLists67[i] = ALValue::array(angleL671[i], angleL672[i],angleL673[i],angleL674[i]);
		}
	}

	timeLists67.clear();
	timeLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists67[i] = ALValue::array(2.0f, 3.0f, 5.0f,7.0f);
	}

	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);

	//Execution
	motion.setStiffnesses(namesL,ArmStiffOn);
	motion.setStiffnesses("LHand", 0.8f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists12, timeLists12, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.83733f, -0.83733f, 0.65f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists23, timeLists23, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.83733f, -0.83733f, 0.25f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists34, timeLists34, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists45, timeLists45, isAbsolute);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.83733f, -0.83733f, 0.65f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists56, timeLists56, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-0.83733f, -0.83733f, 0.25f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists67, timeLists67, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.83733f, -0.83733f), 0.2f);
	motion.setStiffnesses(namesR,ArmStiffOff);
	motion.setStiffnesses("LHand", 0.0f);

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.1f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));
  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }

  //A1 to A3
  if (state == 1) {
   try {
	cout << "LArm A1 to A3" << endl;
	//Variables
	int i;
	//ALValue angle0	= ALValue::array(0.34888f);
	float handspeed = 0.125f;
	bool isAbsolute = true;
	ALValue namesL     = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHipPitch", "RHipPitch");
	ALValue namesR	   = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "LHipPitch", "RHipPitch");
	ALValue ArmStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Position6D POL121,POL122,POL123,POPrG,POG,POPrP,POP,POoP,POL671,POL672,POL673,POL674;
	ALValue angleL121	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL122	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL123	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL671	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL672	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL673	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL674	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleLists12, angleLists23, angleLists34, angleLists45, angleLists56, angleLists67;
	ALValue timeLists12, timeLists23, timeLists34, timeLists45, timeLists56, timeLists67;
	vector<Position6D> POL23(9,Position6D());
	vector<ALValue> angleL23(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL23		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL34(9,Position6D());
	vector<ALValue> angleL34(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL34		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL45(9,Position6D());
	vector<ALValue> angleL45(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL45		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL56(9,Position6D());
	vector<ALValue> angleL56(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL56		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(1.0f,1.0f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.15f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipYawPitch"),ALValue::array(1.0f));

	//LArm
	//initialization
	//1 to 2
	POL121 = Position6D(0.0622613f,0.318123f,0.0977748f,-0.0554658f,0.317343f,01.57806f);
	angleL121 = LArmInverseHipBase(PosOrient2Matrix(POL121),angle0);
	POL122 = Position6D(0.198931,0.260727,0.238157f,-1.97994f,-0.376678f,0.669752f);
	angleL122 = LArmInverseHipBase(PosOrient2Matrix(POL122),angle0);
	POL123 = Position6D(0.237517f,0.128918f,0.264665f,0.00482213f,-0.153142f,0.0981324f);
	angleL123 = LArmInverseHipBase(PosOrient2Matrix(POL123),angle0);

	//2 to 3
	POPrG = POL123;
	POG = Position6D(PIhb.x,PIhb.y,PIhb.z+0.01f,0.0520276f,0.617237f,0.0924543f);

	POL23 = TrajectoryGenerator(POPrG,POG, 4.0f);
	timeL23 = TimeGenerator(POL23, 4.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL23[" << i << "]: " << POL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL23[" << i << "]: " << timeL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL23[i] = LArmInverseHipBase(PosOrient2Matrix(POL23[i]),angle0);
	}

	//3 to 4
	POPrP = Position6D(0.229834f,0.0917726f,0.241954f,0.0721187f,0.0752254f,-0.0852331f);

	POL34 = TrajectoryGeneratorReverse(POG,POPrP,2.0f);
	timeL34 = TimeGeneratorReverse(POL34,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL34[" << i << "]: " << POL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL34[" << i << "]: " << timeL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL34[i] = LArmInverseHipBase(PosOrient2Matrix(POL34[i]),angle0);
	}

	//4 to 5
	POP = Position6D(PFhb.x-0.01f,PFhb.y,PFhb.z+0.0f,0.0155708f,0.498108f,-0.591502f);

	POL45 = TrajectoryGenerator(POPrP,POP,2.0f);
	timeL45 = TimeGenerator(POL45,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL45[" << i << "]: " << POL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL45[" << i << "]: " << timeL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL45[i] = LArmInverseHipBase(PosOrient2Matrix(POL45[i]),angle0);
	}

	//5 to 6
	POoP = Position6D(0.231399f,0.0524062f,0.244387f,0.326277f,-0.00101565f,-0.438472f);

	POL56 = TrajectoryGeneratorReverse(POP,POoP,3.0f);
	timeL56 = TimeGeneratorReverse(POL56,3.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL56[" << i << "]: " << POL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL56[" << i << "]: " << timeL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL56[i] = LArmInverseHipBase(PosOrient2Matrix(POL56[i]),angle0);
	}

	//6 to 7
	POL671 = Position6D(0.237517f,0.128918f,0.264665f,0.00482213f,-0.153142f,0.0981324f);
	angleL671 = LArmInverseHipBase(PosOrient2Matrix(POL671),angle0);
	POL672 = Position6D(0.220376,0.243522f,0.19584f,-2.01829f,-0.208182f,0.478643f);
	angleL672 = LArmInverseHipBase(PosOrient2Matrix(POL672),angle0);
	POL673 = Position6D(0.0574361f,0.314933f,0.0898657f,-0.00848031f,0.35612f,1.60025f);
	angleL673 = LArmInverseHipBase(PosOrient2Matrix(POL673),angle0);
	POL674 = Position6D(0.00123893f,0.119104f,-0.0356745f,1.29611f,1.27495f,2.95056f);
	angleL674 = LArmInverseHipBase(PosOrient2Matrix(POL674),angle0);

	//Angle and Time Listing
	//1 to 2
	angleLists12.clear();
	angleLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists12[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists12[i] = ALValue::array(angleL121[i], angleL122[i], angleL123[i]);
		}
	}

	timeLists12.clear();
	timeLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists12[i] = ALValue::array(2.0f, 3.0f, 5.0f);
	}

	//2 to 3
	angleLists23.clear();
	angleLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists23[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists23[i] = ALValue::array(angleL23[1][i],angleL23[2][i],angleL23[3][i],angleL23[4][i],angleL23[5][i],angleL23[6][i],angleL23[7][i],angleL23[8][i]);
		}
	}

	timeLists23.clear();
	timeLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists23[i] = ALValue::array(timeL23[1],timeL23[2],timeL23[3],timeL23[4],timeL23[5],timeL23[6],timeL23[7],timeL23[8]);
	}

	//3 to 4
	angleLists34.clear();
	angleLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists34[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists34[i] = ALValue::array(angleL34[1][i],angleL34[2][i],angleL34[3][i],angleL34[4][i],angleL34[5][i],angleL34[6][i],angleL34[7][i],angleL34[8][i]);
		}
	}

	timeLists34.clear();
	timeLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists34[i] = ALValue::array(timeL34[1],timeL34[2],timeL34[3],timeL34[4],timeL34[5],timeL34[6],timeL34[7],timeL34[8]);
	}

	//4 to 5
	angleLists45.clear();
	angleLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists45[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists45[i] = ALValue::array(angleL45[1][i],angleL45[2][i],angleL45[3][i],angleL45[4][i],angleL45[5][i],angleL45[6][i],angleL45[7][i],angleL45[8][i]);
		}
	}

	timeLists45.clear();
	timeLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists45[i] = ALValue::array(timeL45[1],timeL45[2],timeL45[3],timeL45[4],timeL45[5],timeL45[6],timeL45[7],timeL45[8]);
	}

	//5 to 6
	angleLists56.clear();
	angleLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists56[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists56[i] = ALValue::array(angleL56[1][i],angleL56[2][i],angleL56[3][i],angleL56[4][i],angleL56[5][i],angleL56[6][i],angleL56[7][i],angleL56[8][i]);
		}
	}

	timeLists56.clear();
	timeLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists56[i] = ALValue::array(timeL56[1],timeL56[2],timeL56[3],timeL56[4],timeL56[5],timeL56[6],timeL56[7],timeL56[8]);
	}

	//6 to 7
	angleLists67.clear();
	angleLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists67[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists67[i] = ALValue::array(angleL671[i], angleL672[i],angleL673[i],angleL674[i]);
		}
	}

	timeLists67.clear();
	timeLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists67[i] = ALValue::array(2.0f, 3.0f, 5.0f,7.0f);
	}

	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);

	//Execution
	motion.setStiffnesses(namesL,ArmStiffOn);
	motion.setStiffnesses("LHand", 0.8f);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(1.0f,1.0f));
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists12, timeLists12, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-1.04667f, -1.04667f, 0.65f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists23, timeLists23, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-1.04667f, -1.04667f, 0.25f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists34, timeLists34, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists45, timeLists45, isAbsolute);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-1.04667f, -1.04667f, 0.65f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists56, timeLists56, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-1.04667f, -1.04667f, 0.25f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists67, timeLists67, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	motion.setStiffnesses(namesR,ArmStiffOff);
	motion.setStiffnesses("LHand", 0.0f);
	motion.setStiffnesses(ALValue::array("LHipYawPitch"),ALValue::array(0.0f));

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.15f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));
  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }

   //A2 to A4
  if (state == 2) {
   try {
	cout << "LArm A2 to A4" << endl;
	//Variables
	int i;
	//ALValue angle0	= ALValue::array(0.34888f);
	float handspeed = 0.125f;
	bool isAbsolute = true;
	ALValue namesL     = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHipPitch", "RHipPitch");
	ALValue namesR	   = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "LHipPitch", "RHipPitch");
	ALValue ArmStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Position6D POL121,POL122,POL123,POPrG,POG,POPrP,POP,POoP,POL671,POL672,POL673,POL674;
	ALValue angleL121	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL122	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL123	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL671	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL672	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL673	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleL674	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleLists12, angleLists23, angleLists34, angleLists45, angleLists56, angleLists67;
	ALValue timeLists12, timeLists23, timeLists34, timeLists45, timeLists56, timeLists67;
	vector<Position6D> POL23(9,Position6D());
	vector<ALValue> angleL23(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL23		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL34(9,Position6D());
	vector<ALValue> angleL34(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL34		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL45(9,Position6D());
	vector<ALValue> angleL45(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL45		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POL56(9,Position6D());
	vector<ALValue> angleL56(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeL56		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(1.0f,1.0f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.15f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipYawPitch"),ALValue::array(1.0f));

	//LArm
	//initialization
	//1 to 2
	POL121 = Position6D(0.0622613f,0.318123f,0.0977748f,-0.0554658f,0.317343f,01.57806f);
	angleL121 = LArmInverseHipBase(PosOrient2Matrix(POL121),angle0);
	POL122 = Position6D(0.198931,0.260727,0.238157f,-1.97994f,-0.376678f,0.669752f);
	angleL122 = LArmInverseHipBase(PosOrient2Matrix(POL122),angle0);
	POL123 = Position6D(0.228199f,0.0822198f,0.276844f,0.0368945f,-0.226299f,-0.12421f);
	angleL123 = LArmInverseHipBase(PosOrient2Matrix(POL123),angle0);

	//2 to 3
	POPrG = POL123;
	POG = Position6D(PIhb.x,PIhb.y,PIhb.z+0.01f,-0.0818397f,0.623312f,-0.154035f);

	POL23 = TrajectoryGenerator(POPrG,POG, 4.0f);
	timeL23 = TimeGenerator(POL23, 4.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL23[" << i << "]: " << POL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL23[" << i << "]: " << timeL23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL23[i] = LArmInverseHipBase(PosOrient2Matrix(POL23[i]),angle0);
	}

	//3 to 4
	POPrP = Position6D(0.235849f,0.0683565f,0.218795f,-0.102793f,0.195478,-0.244422f);

	POL34 = TrajectoryGeneratorReverse(POG,POPrP,2.0f);
	timeL34 = TimeGeneratorReverse(POL34,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL34[" << i << "]: " << POL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL34[" << i << "]: " << timeL34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL34[i] = LArmInverseHipBase(PosOrient2Matrix(POL34[i]),angle0);
	}

	//4 to 5
	POP = Position6D(PFhb.x-0.01f,PFhb.y,PFhb.z+0.01f,-0.149083f,0.347143f,-0.892745f);

	POL45 = TrajectoryGenerator(POPrP,POP,2.0f);
	timeL45 = TimeGenerator(POL45,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL45[" << i << "]: " << POL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL45[" << i << "]: " << timeL45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL45[i] = LArmInverseHipBase(PosOrient2Matrix(POL45[i]),angle0);
	}

	//5 to 6
	POoP = Position6D(0.229316f,0.017102f,0.230211f,0.238848f,-0.0280971f,-0.733455f);

	POL56 = TrajectoryGeneratorReverse(POP,POoP,3.0f);
	timeL56 = TimeGeneratorReverse(POL56,3.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POL56[" << i << "]: " << POL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeL56[" << i << "]: " << timeL56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleL56[i] = LArmInverseHipBase(PosOrient2Matrix(POL56[i]),angle0);
	}

	//6 to 7
	POL671 = Position6D(0.228199f,0.0822198f,0.276844f,0.0368945f,-0.226299f,-0.12421f);
	angleL671 = LArmInverseHipBase(PosOrient2Matrix(POL671),angle0);
	POL672 = Position6D(0.220376,0.243522f,0.19584f,-2.01829f,-0.208182f,0.478643f);
	angleL672 = LArmInverseHipBase(PosOrient2Matrix(POL672),angle0);
	POL673 = Position6D(0.0574361f,0.314933f,0.0898657f,-0.00848031f,0.35612f,1.60025f);
	angleL673 = LArmInverseHipBase(PosOrient2Matrix(POL673),angle0);
	POL674 = Position6D(0.00123893f,0.119104f,-0.0356745f,1.29611f,1.27495f,2.95056f);
	angleL674 = LArmInverseHipBase(PosOrient2Matrix(POL674),angle0);

	//Angle and Time Listing
	//1 to 2
	angleLists12.clear();
	angleLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists12[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists12[i] = ALValue::array(angleL121[i], angleL122[i], angleL123[i]);
		}
	}

	timeLists12.clear();
	timeLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists12[i] = ALValue::array(2.0f, 3.0f, 5.0f);
	}

	//2 to 3
	angleLists23.clear();
	angleLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists23[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists23[i] = ALValue::array(angleL23[1][i],angleL23[2][i],angleL23[3][i],angleL23[4][i],angleL23[5][i],angleL23[6][i],angleL23[7][i],angleL23[8][i]);
		}
	}

	timeLists23.clear();
	timeLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists23[i] = ALValue::array(timeL23[1],timeL23[2],timeL23[3],timeL23[4],timeL23[5],timeL23[6],timeL23[7],timeL23[8]);
	}

	//3 to 4
	angleLists34.clear();
	angleLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists34[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists34[i] = ALValue::array(angleL34[1][i],angleL34[2][i],angleL34[3][i],angleL34[4][i],angleL34[5][i],angleL34[6][i],angleL34[7][i],angleL34[8][i]);
		}
	}

	timeLists34.clear();
	timeLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists34[i] = ALValue::array(timeL34[1],timeL34[2],timeL34[3],timeL34[4],timeL34[5],timeL34[6],timeL34[7],timeL34[8]);
	}

	//4 to 5
	angleLists45.clear();
	angleLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists45[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists45[i] = ALValue::array(angleL45[1][i],angleL45[2][i],angleL45[3][i],angleL45[4][i],angleL45[5][i],angleL45[6][i],angleL45[7][i],angleL45[8][i]);
		}
	}

	timeLists45.clear();
	timeLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists45[i] = ALValue::array(timeL45[1],timeL45[2],timeL45[3],timeL45[4],timeL45[5],timeL45[6],timeL45[7],timeL45[8]);
	}

	//5 to 6
	angleLists56.clear();
	angleLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists56[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists56[i] = ALValue::array(angleL56[1][i],angleL56[2][i],angleL56[3][i],angleL56[4][i],angleL56[5][i],angleL56[6][i],angleL56[7][i],angleL56[8][i]);
		}
	}

	timeLists56.clear();
	timeLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists56[i] = ALValue::array(timeL56[1],timeL56[2],timeL56[3],timeL56[4],timeL56[5],timeL56[6],timeL56[7],timeL56[8]);
	}

	//6 to 7
	angleLists67.clear();
	angleLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists67[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists67[i] = ALValue::array(angleL671[i], angleL672[i],angleL673[i],angleL674[i]);
		}
	}

	timeLists67.clear();
	timeLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists67[i] = ALValue::array(2.0f, 3.0f, 5.0f,7.0f);
	}

	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);

	//Execution
	motion.setStiffnesses(namesL,ArmStiffOn);
	motion.setStiffnesses("LHand", 0.8f);

	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(1.0f,1.0f));
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists12, timeLists12, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-1.04667f, -1.04667f, 0.65f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists23, timeLists23, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-1.04667f, -1.04667f, 0.25f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists34, timeLists34, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists45, timeLists45, isAbsolute);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-1.04667f, -1.04667f, 0.65f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesL, angleLists56, timeLists56, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "LHand"),ALValue::array(-1.04667f, -1.04667f, 0.25f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesL, angleLists67, timeLists67, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	motion.setStiffnesses(namesR,ArmStiffOff);
	motion.setStiffnesses("LHand", 0.0f);
	motion.setStiffnesses(ALValue::array("LHipYawPitch"),ALValue::array(0.0f));

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.15f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));
  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }

   //A7 to A5
  if (state == 3) {
   try {
	cout << "RArm A7 to A5" << endl;
	//Variables
	int i;
	//ALValue angle0 = ALValue::array(0.34888f);
	float handspeed = 0.125f;
	bool isAbsolute = true;
	ALValue namesL     = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHipPitch", "RHipPitch");
	ALValue namesR	   = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "LHipPitch", "RHipPitch");
	ALValue ArmStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	Position6D POR121,POR122,POR123,POPrG,POG,POPrP,POP,POoP,POR671,POR672,POR673,POR674;
	ALValue angleR121	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR122	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR123	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR671	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR672	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR673	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR674	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleLists12, angleLists23, angleLists34, angleLists45, angleLists56, angleLists67;
	ALValue timeLists12, timeLists23, timeLists34, timeLists45, timeLists56, timeLists67;
	vector<Position6D> POR23(9,Position6D());
	vector<ALValue> angleR23(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR23		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR34(9,Position6D());
	vector<ALValue> angleR34(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR34		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR45(9,Position6D());
	vector<ALValue> angleR45(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR45		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	vector<Position6D> POR56(9,Position6D());
	vector<ALValue> angleR56(9,ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f));
	ALValue timeR56		= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	//HipPitch at 0.13955 rad (act = -0.83733 rad)
    motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(1.0f,1.0f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipYawPitch"),ALValue::array(1.0f));

	//initialization
	//1 to 2
	POR121 = Position6D(0.0538579f,-0.306606f,0.0727373f,-0.0892349f,0.451915f,-1.61621f);
	angleR121 = RArmInverseHipBase(PosOrient2Matrix(POR121),angle0);
	POR122 = Position6D(0.209336f,-0.261748f,0.200288f,1.88349f,-0.180987f,-0.631216f);
	angleR122 = RArmInverseHipBase(PosOrient2Matrix(POR122),angle0);
	POR123 = Position6D(0.221766f,-0.121142f,0.288381f,-0.0250543f,-0.285574f,-0.0481139f);
	angleR123 = RArmInverseHipBase(PosOrient2Matrix(POR123),angle0);

	//2 to 3
	POPrG = POR123;
	POG = Position6D(PIhb.x,PIhb.y,PIhb.z+0.01f,0.0642417f,0.638399f,-0.133826f);

	POR23 = TrajectoryGenerator(POPrG,POG, 4.0f);
	timeR23 = TimeGenerator(POR23, 4.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR23[" << i << "]: " << POR23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR23[" << i << "]: " << timeR23[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR23[i] = RArmInverseHipBase(PosOrient2Matrix(POR23[i]),angle0);
	}

	//3 to 4
	POPrP = Position6D(0.230656f,-0.102753f,0.243963f,0.0224238f,0.0617517f,0.0413769f);

	POR34 = TrajectoryGeneratorReverse(POG,POPrP,2.0f);
	timeR34 = TimeGeneratorReverse(POR34,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR34[" << i << "]: " << POR34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR34[" << i << "]: " << timeR34[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR34[i] = RArmInverseHipBase(PosOrient2Matrix(POR34[i]),angle0);
	}

	//4 to 5
	POP = Position6D(PFhb.x-0.01f,PFhb.y,PFhb.z+0.01f,0.161984f,0.602558f,0.465133f);

	POR45 = TrajectoryGenerator(POPrP,POP,2.0f);
	timeR45 = TimeGenerator(POR45,2.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR45[" << i << "]: " << POR45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR45[" << i << "]: " << timeR45[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR45[i] = RArmInverseHipBase(PosOrient2Matrix(POR45[i]),angle0);
	}

	//5 to 6
	POoP = Position6D(0.235948f,-0.0514988f,0.210759f,-0.00557462f,0.215234f,0.396429f);

	POR56 = TrajectoryGeneratorReverse(POP,POoP,3.0f);
	timeR56 = TimeGeneratorReverse(POR56,3.0f);

	for (i=0; i<=8; i++)
	{
		cout << "POR56[" << i << "]: " << POR56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		cout << "timeR56[" << i << "]: " << timeR56[i] << endl;
	}
	
	for (i=0; i<=8; i++)
	{
		angleR56[i] = RArmInverseHipBase(PosOrient2Matrix(POR56[i]),angle0);
	}

	//6 to 7
	POR671 = Position6D(0.221766f,-0.121142f,0.288381f,-0.0250543f,-0.285574f,-0.0481139f);
	angleR671 = RArmInverseHipBase(PosOrient2Matrix(POR671),angle0);
	POR672 = Position6D(0.219642f,-0.246644f,0.223384f,1.67938f,-0.241708f,-0.555064f);
	angleR672 = RArmInverseHipBase(PosOrient2Matrix(POR672),angle0);
	POR673 = Position6D(0.0597197f,-0.316484f,0.0938806f,-0.0188598f,0.343181f,-1.58772f);
	angleR673 = RArmInverseHipBase(PosOrient2Matrix(POR673),angle0);
	POR674 = Position6D(0.0437814f,-0.129268f,-0.0432777f,-0.698874f,1.43406f,-2.28938f);
	angleR674 = RArmInverseHipBase(PosOrient2Matrix(POR674),angle0);

	//Angle and Time Listing
	//1 to 2
	angleLists12.clear();
	angleLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists12[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists12[i] = ALValue::array(angleR121[i], angleR122[i], angleR123[i]);
		}
	}

	timeLists12.clear();
	timeLists12.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists12[i] = ALValue::array(2.0f, 3.0f, 5.0f);
	}

	//2 to 3
	angleLists23.clear();
	angleLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists23[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists23[i] = ALValue::array(angleR23[1][i],angleR23[2][i],angleR23[3][i],angleR23[4][i],angleR23[5][i],angleR23[6][i],angleR23[7][i],angleR23[8][i]);
		}
	}

	timeLists23.clear();
	timeLists23.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists23[i] = ALValue::array(timeR23[1],timeR23[2],timeR23[3],timeR23[4],timeR23[5],timeR23[6],timeR23[7],timeR23[8]);
	}

	//3 to 4
	angleLists34.clear();
	angleLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists34[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists34[i] = ALValue::array(angleR34[1][i],angleR34[2][i],angleR34[3][i],angleR34[4][i],angleR34[5][i],angleR34[6][i],angleR34[7][i],angleR34[8][i]);
		}
	}

	timeLists34.clear();
	timeLists34.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists34[i] = ALValue::array(timeR34[1],timeR34[2],timeR34[3],timeR34[4],timeR34[5],timeR34[6],timeR34[7],timeR34[8]);
	}

	//4 to 5
	angleLists45.clear();
	angleLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists45[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists45[i] = ALValue::array(angleR45[1][i],angleR45[2][i],angleR45[3][i],angleR45[4][i],angleR45[5][i],angleR45[6][i],angleR45[7][i],angleR45[8][i]);
		}
	}

	timeLists45.clear();
	timeLists45.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists45[i] = ALValue::array(timeR45[1],timeR45[2],timeR45[3],timeR45[4],timeR45[5],timeR45[6],timeR45[7],timeR45[8]);
	}

	//5 to 6
	angleLists56.clear();
	angleLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists56[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f,-1.04667f, -1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists56[i] = ALValue::array(angleR56[1][i],angleR56[2][i],angleR56[3][i],angleR56[4][i],angleR56[5][i],angleR56[6][i],angleR56[7][i],angleR56[8][i]);
		}
	}

	timeLists56.clear();
	timeLists56.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists56[i] = ALValue::array(timeR56[1],timeR56[2],timeR56[3],timeR56[4],timeR56[5],timeR56[6],timeR56[7],timeR56[8]);
	}

	//6 to 7
	angleLists67.clear();
	angleLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		if (i>4)
		{
			angleLists67[i] = ALValue::array(-1.04667f, -1.04667f, -1.04667f, -1.04667f);
		} 
		else
		{
			angleLists67[i] = ALValue::array(angleR671[i], angleR672[i],angleR673[i],angleR674[i]);
		}
	}

	timeLists67.clear();
	timeLists67.arraySetSize(7);
	for (i=0; i<=6; i++)
	{
		timeLists67[i] = ALValue::array(2.0f, 3.0f, 5.0f,7.0f);
	}

	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);

	//Execution
	motion.setStiffnesses(namesR,ArmStiffOn);
	motion.setStiffnesses("RHand", 0.8f);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(1.0f,1.0f));
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists12, timeLists12, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-1.04667f, -1.04667f, 0.65f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists23, timeLists23, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-1.04667f, -1.04667f, 0.25f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesR, angleLists34, timeLists34, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists45, timeLists45, isAbsolute);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-1.04667f, -1.04667f, 0.65f), handspeed);
	qi::os::msleep(1500);
	motion.angleInterpolation(namesR, angleLists56, timeLists56, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch", "RHand"),ALValue::array(-1.04667f, -1.04667f, 0.25f), handspeed);
	qi::os::msleep(500);
	motion.angleInterpolation(namesR, angleLists67, timeLists67, isAbsolute);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-1.04667f, -1.04667f), 0.2f);
	motion.setStiffnesses(namesR,ArmStiffOff);
	motion.setStiffnesses("RHand", 0.0f);
	motion.setStiffnesses(ALValue::array("LHipYawPitch"),ALValue::array(0.0f));

	//HipPitch at 0 rad (act = -0.69778 rad)
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));
  }
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
	}
  }

	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));

  //Initial hand pose
	Position6D POL01l = Position6D(-0.00259649f,0.132217f,-0.0326618f,0.0906276f,1.45813f,1.67902f);
	ALValue angleL01l = LArmInverseHipBase(PosOrient2Matrix(POL01l),ALValue::array(0.0f));
	Position6D POR01l = Position6D(0.00658906f,-0.132896f,-0.032212f,0.289373f,1.46564f,-1.29847f);
	ALValue angleR01l = RArmInverseHipBase(PosOrient2Matrix(POR01l),ALValue::array(0.0f));
	ALValue angleL01r	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR01r	= ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	int i;
	ALValue angleListsl;
	angleListsl.clear();
	angleListsl.arraySetSize(10);
	for (i=0; i<=9; i++)
	{
		if((i>=0)&&(i<=4))
		{
			angleListsl[i] = ALValue::array(angleL01l[i]);
		} 
		else
		{
			angleListsl[i] = ALValue::array(angleR01l[i-5]);
		}
	}

	ALValue timeListsl;
	timeListsl.clear();
	timeListsl.arraySetSize(10);
	for (i=0; i<=9; i++)
	{
		timeListsl[i] = ALValue::array(2.0f);
	}
	
	ALValue ArmsStiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue ArmsStiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	motion.setStiffnesses(ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"),ArmsStiffOn);
	qi::os::msleep(500);
	motion.angleInterpolation(ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"), angleListsl, timeListsl, true);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"),ArmsStiffOff);
	
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.7f,0.7f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(-0.69778f, -0.69778f), 0.2f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("LHipPitch", "RHipPitch"),ALValue::array(0.0f,0.0f));

	motion.setStiffnesses(ALValue::array("HeadYaw", "HeadPitch"),ALValue::array(1.0f,1.0f));
	qi::os::msleep(500);
	motion.setAngles(ALValue::array("HeadYaw", "HeadPitch"),headj,0.25f);
	qi::os::msleep(500);
	motion.setStiffnesses(ALValue::array("HeadYaw", "HeadPitch"),ALValue::array(0.0f,0.0f));
}

