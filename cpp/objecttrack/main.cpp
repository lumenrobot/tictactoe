#include "objecttrack.h"

#include <iostream>
#include <fstream>
#include <sstream>

using namespace std;
using namespace AL;
using namespace Math;

int main(int argc, char* argv[]) {

  if(argc != 2)
  {
    std::cerr << "Wrong number of arguments!" << std::endl;
    std::cerr << "Usage: movehead NAO_IP" << std::endl;
    exit(2);
  }

  try {
    AL::ALMotionProxy motion(argv[1], 9559);
	AL::ALTextToSpeechProxy tts(argv[1], 9559);
    AL::ALRobotPostureProxy robotPosture(argv[1], 9559);
	AL::ALRedBallTrackerProxy redBallTracker(argv[1], 9559);

	//Variables declraration
	Position6D PosOriL,PosOriR;
	Transform TL,TR;
	vector<float> LHpr(6,0.0f); //position and orientation vector
	vector<float> RHpr(6,0.0f); //position and orientation vector
	ALValue angleL	 = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue angleR	 = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue nameL    = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw");
	ALValue nameR    = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw");
	ALValue angles   = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	ALValue stiffOn  = ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	ALValue stiffOff = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	float fractionMaxSpeed  = 0.175f;
	ALValue angleLists = ALValue::array(0.0f);
    ALValue timeLists  = ALValue::array(0.0f);
	bool isAbsolute    = true;
	unsigned int i,j;
	bool exit;
	
	ifstream LAPR[6]; 
	ifstream RAPR[6]; 

	LAPR[0].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/LArmPx.csv");
	LAPR[1].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/LArmPy.csv");
	LAPR[2].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/LArmPz.csv");
	LAPR[3].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/LArmRx.csv");
	LAPR[4].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/LArmRy.csv");
	LAPR[5].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/LArmRz.csv");
	RAPR[0].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/RArmPx.csv");
	RAPR[1].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/RArmPy.csv");
	RAPR[2].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/RArmPz.csv");
	RAPR[3].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/RArmRx.csv");
	RAPR[4].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/RArmRy.csv");
	RAPR[5].open("D:/Final Project/Tesis/NAO/Workplace/handmeasuring/build-win32-vs2010/sdk/bin/RArmRz.csv");
	
	string sPRL[1000][6];
	string sPRR[1000][6];
	float fPRL[1000][6];
	float fPRR[1000][6];
	unsigned int LArmSize;	
	unsigned int RArmSize;
	
	i = 0; j = 0;
	exit = false;
	cout << "LHand position and orientation: " << endl;
	while (!exit)
	{
		for (j=0; j<=5; j++)
		{
			getline(LAPR[j],sPRL[i][j]);
			sPRL[i][j].erase(sPRL[i][j].begin()+0);
			if (sPRL[i][j].empty())
			{
				fPRL[i][j] = 0.0f;
				LArmSize = i;
				exit = true;
			} else
			{
				stringstream ssL(sPRL[i][j]);
				ssL >> fPRL[i][j];
				cout << j << "  " << sPRL[i][j] << "  " << fPRL[i][j] <<  "  " << i << endl;
			}
		}
		i++;
	}

	i = 0; j = 0; 
	exit = false;
	cout << "RHand position and orientation: " << endl;
	while (!exit)
	{
		for (j=0; j<=5; j++)
		{
			getline(RAPR[j],sPRR[i][j]);
			sPRR[i][j].erase(sPRR[i][j].begin()+0);
			if (sPRR[i][j].empty())
			{
				fPRR[i][j] = 0.0f;
				RArmSize = i;
				exit = true;
			} else
			{
				stringstream ssR(sPRR[i][j]);
				ssR >> fPRR[i][j];
				cout << j << "  " << sPRR[i][j] << "  " << fPRR[i][j] <<  "  " << i << endl;
			}
		}
		i++;
	}

	cout << "LArmSize: " << LArmSize << endl;
	cout << "RArmSize: " << RArmSize << endl;

	motion.wakeUp();
	robotPosture.goToPosture("Crouch", 0.5f);
	//motion.rest();

	//Start 
	//LArm
	i = 0; j = 0;
	motion.setStiffnesses(nameL, stiffOn);
	while (i<LArmSize)
	{
		for (j=0; j<=5; j++)
		{
			LHpr[j] = fPRL[i][j];
		}
		PosOriL = Position6D(LHpr);
		TL = PosOrient2Matrix(PosOriL);
		angleL = funcLArmInv5(TL);
		cout << "LArm Execution No.: " << i+1 << endl;
		cout << "LArm sensor angles: " << angleL[0] << " " << angleL[1] << " " << angleL[2] << " " << angleL[3] << " " << angleL[4] << endl;
		cout << "LHand position and orientation: " << PosOriL << endl;
		switch (i)
		{
		case 2:
			motion.openHand("LHand");
			motion.setAngles(nameL, angleL, fractionMaxSpeed);
			qi::os::msleep(1500);
			break;
		case 3:
			motion.setAngles(nameL, angleL, fractionMaxSpeed);
			qi::os::msleep(1000);
			motion.closeHand("LHand");
			qi::os::msleep(1000);
			break;
		case 5:
			motion.setAngles(nameL, angleL, fractionMaxSpeed);
			qi::os::msleep(1500);
			motion.openHand("LHand");
			qi::os::msleep(1000);
			break;
		case 6:
			motion.setAngles(nameL, angleL, fractionMaxSpeed);
			motion.closeHand("LHand");
			qi::os::msleep(1500);
			break;
		default:		
			motion.setAngles(nameL, angleL, fractionMaxSpeed);
		    qi::os::msleep(1500);
		}
		i++;
	}

	//RArm
	i = 0; j = 0;
	motion.setStiffnesses(nameR, stiffOn);
	while (i<RArmSize)
	{
		for (j=0; j<=5; j++)
		{
			RHpr[j] = fPRR[i][j];
		}
		PosOriR = Position6D(RHpr);
		TR = PosOrient2Matrix(PosOriR);
		angleR = funcRArmInv5(TR);
		cout << "RArm Execution No.: " << i+1 << endl;
		cout << "RArm sensor angles: " << angleR[0] << " " << angleR[1] << " " << angleR[2] << " " << angleR[3] << " " << angleR[4] << endl;
		cout << "RHand position and orientation: " << PosOriR << endl;
		switch (i)
		{
		case 2:
			motion.openHand("RHand");
			motion.setAngles(nameR, angleR, fractionMaxSpeed);
			qi::os::msleep(1500);
			break;
		case 3:
			motion.setAngles(nameR, angleR, fractionMaxSpeed);
			qi::os::msleep(1000);
			motion.closeHand("RHand");
			qi::os::msleep(1000);
			break;
		case 5:
			motion.setAngles(nameR, angleR, fractionMaxSpeed);
			qi::os::msleep(1000);
			motion.openHand("RHand");
			qi::os::msleep(1000);
			break;
		case 6:
			motion.setAngles(nameR, angleR, fractionMaxSpeed);
			motion.closeHand("RHand");
			qi::os::msleep(1500);
			break;
		default:		
			motion.setAngles(nameR, angleR, fractionMaxSpeed);
		    qi::os::msleep(1500);
		}
		motion.setAngles(nameR, angleR, fractionMaxSpeed);
		qi::os::msleep(1500);
		i++;
	}
	
	robotPosture.goToPosture("Crouch", 0.5f);
	motion.rest();

  }
  catch (const AL::ALError& e) {
    std::cerr << "Caught exception: " << e.what() << std::endl;
    exit(1);
  }
  exit(0);
}