#include <iostream>
#include <fstream>
#include <alerror/alerror.h>
#include <alproxies/almotionproxy.h>
#include <alproxies/alrobotpostureproxy.h>
#include <qi/os.hpp>
#include <alproxies/almemoryproxy.h>

#include "function.h"

using namespace std;
using namespace AL;

int main(int argc, char* argv[]) {

  if(argc != 2)
  {
    std::cerr << "Wrong number of arguments!" << std::endl;
    std::cerr << "Usage: movehead NAO_IP" << std::endl;
    exit(2);
  }
	
  try {
    ALMotionProxy motion(argv[1], 9559);
    ALRobotPostureProxy robotPosture(argv[1], 9559);
	ALMemoryProxy memory(argv[1], 9559);

	ALValue nameshead   = ALValue::array("HeadYaw","HeadPitch");
	ALValue namesL     = ALValue::array("LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw");
	ALValue namesR	   = ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw");
	ALValue namesHip =  ALValue::array("LHipPitch", "RHipPitch");
	//ALValue theta0 = ALValue::array(0.0f);
	//ALValue theta0 = ALValue::array(0.13955f);
	ALValue theta0 = ALValue::array(0.34888f);
	ALValue HipJoints = ALValue::array(-0.69778f, -0.69778f);
	ALValue HeadJoints = ALValue::array(0.0f, 0.0f);
	ALValue LAJoints = ALValue::array(0.0f, 0.0f, 0.0f, -0.0349f, 0.0f);
	ALValue RAJoints = ALValue::array(0.0f, 0.0f, 0.0f, 0.0349f, 0.0f);
	Position6D H,LA,RA;
	vector<float> jointAngles(5,0.0f);
	ALValue jointAngle;
	Transform M;

	motion.wakeUp();
	robotPosture.goToPosture("Crouch",0.75f);
	motion.rest();

	motion.setStiffnesses(namesHip,ALValue::array(1.0f,1.0f));
	motion.setStiffnesses(ALValue::array("LHipYawPitch"),ALValue::array(1.0f));
	qi::os::msleep(500);
	//motion.setAngles(namesHip,ALValue::array(-0.69778f, -0.69778f), 0.1f);
	//motion.setAngles(namesHip,ALValue::array(-0.83733f, -0.83733f), 0.1f);
	motion.setAngles(namesHip,ALValue::array(-1.04667f, -1.04667f), 0.1f);
	/*
	//Head
	motion.setStiffnesses(nameshead,ALValue::array(1.0f,1.0f));
	qi::os::msleep(1000);
	motion.setAngles(nameshead,HeadJoints,0.25f);
	qi::os::msleep(1000);
	jointAngles = motion.getAngles(nameshead,true);
	jointAngle = ALValue::array(jointAngles[0], jointAngles[1]);
	M = HeadForwardHipBase(jointAngle,theta0);
	H = Matrix2PosOrient(M);
	cout << "End Effector (x,y,z,wx,wy,wz): (" << H.x << "," << H.y << "," << H.z << "," << H.wx << "," << H.wy << "," << H.wz << ")" << endl;
	qi::os::msleep(1000);
	motion.setStiffnesses(nameshead,ALValue::array(0.0f,0.0f));
	*/
	/*
	//LArm
	motion.setStiffnesses(namesL,ALValue::array(1.0f,1.0f,1.0f,1.0f,1.0f));
	qi::os::msleep(500);
	motion.setAngles(namesL,LAJoints,0.25f);
	qi::os::msleep(1000);
	jointAngles = motion.getAngles(namesL,true);
	jointAngle = ALValue::array(jointAngles[0], jointAngles[1], jointAngles[2], jointAngles[3], jointAngles[4]);
	M = LArmForwardHipBase(jointAngle,theta0);
	H = Matrix2PosOrient(M);
	cout << "End Effector (x,y,z,wx,wy,wz): (" << H.x << "," << H.y << "," << H.z << "," << H.wx << "," << H.wy << "," << H.wz << ")" << endl;
	qi::os::msleep(1000);
	motion.setStiffnesses(namesL,ALValue::array(0.0f,0.0f,0.0f,0.0f,0.0f));
	qi::os::msleep(1000);
	*/

	//RArm
	motion.setStiffnesses(namesR,ALValue::array(1.0f,1.0f,1.0f,1.0f,1.0f));
	qi::os::msleep(500);
	motion.setAngles(namesR,RAJoints,0.25f);
	qi::os::msleep(1000);
	jointAngles = motion.getAngles(namesR,true);
	jointAngle = ALValue::array(jointAngles[0], jointAngles[1], jointAngles[2], jointAngles[3], jointAngles[4]);
	M = RArmForwardHipBase(jointAngle,theta0);
	H = Matrix2PosOrient(M);
	cout << "End Effector (x,y,z,wx,wy,wz): (" << H.x << "," << H.y << "," << H.z << "," << H.wx << "," << H.wy << "," << H.wz << ")" << endl;
	qi::os::msleep(1000);
	motion.setStiffnesses(namesR,ALValue::array(0.0f,0.0f,0.0f,0.0f,0.0f));
	qi::os::msleep(1000);

	motion.setStiffnesses(ALValue::array("LHipYawPitch"),ALValue::array(0.0f));
	motion.setStiffnesses(namesHip,ALValue::array(1.0f,1.0f));
	qi::os::msleep(1000);
	motion.setAngles(namesHip,HipJoints,0.2f);
	qi::os::msleep(1000);
	motion.setStiffnesses(namesHip,ALValue::array(0.0f,0.0f));

	}
  catch (const AL::ALError& e) {
    std::cerr << "Caught exception: " << e.what() << std::endl;
    exit(1);
  }
  exit(0);
}