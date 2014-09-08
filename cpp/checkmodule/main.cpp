/*	
	Sebastian A. Nugroho (23212344)
    13 November 2013
*/

#include <iostream>
#include <alerror/alerror.h>
#include <alproxies/almotionproxy.h>
#include <alproxies/alrobotpostureproxy.h>
#include <qi/os.hpp>
#include <alproxies/altexttospeechproxy.h>

int main(int argc, char* argv[]) {

  if(argc != 2)
  {
    std::cerr << "Wrong number of arguments!" << std::endl;
    std::cerr << "Usage: movehead NAO_IP" << std::endl;
    exit(2);
  }

	const std::string phraseToSay1 = "Checking all joints";
	const std::string phraseToSay2 = "Joints checking is complete";

    try {
    
    AL::ALMotionProxy motion(argv[1], 9559);
    AL::ALRobotPostureProxy robotPosture(argv[1], 9559);
	AL::ALTextToSpeechProxy tts(argv[1], 9559);

	motion.wakeUp();
	robotPosture.goToPosture("Crouch", 1.0f);
	robotPosture.goToPosture("Stand", 1.0f);
	float fractionMaxSpeed  = 0.2f; 
	tts.say(phraseToSay1);

	//Head
	AL::ALValue stiffness = 1.0f;
    AL::ALValue time = 0.1f;
    motion.stiffnessInterpolation("HeadYaw", stiffness, time);

    AL::ALValue targetAngles = AL::ALValue::array(-1.5f, 1.5f, 0.0f);
    AL::ALValue targetTimes = AL::ALValue::array(1.0f, 3.0f, 4.0f);
    bool isAbsolute = true;

    motion.angleInterpolation("HeadYaw", targetAngles, targetTimes, isAbsolute);

    stiffness = 1.0f;
    motion.stiffnessInterpolation("HeadYaw", stiffness, time);

	stiffness = 1.0f;
    motion.stiffnessInterpolation("HeadPitch", stiffness, time);

    targetAngles = AL::ALValue::array(-0.6f, 0.5f, 0.0f);
    targetTimes = AL::ALValue::array(1.0f, 2.0f, 3.0f);

    motion.angleInterpolation("HeadPitch", targetAngles, targetTimes, isAbsolute);
	
    stiffness = 1.0f;
    motion.stiffnessInterpolation("HeadPitch", stiffness, time);

	//Hand
	AL::ALValue Names       = AL::ALValue::array("RShoulderRoll", "LShoulderRoll");
    AL::ALValue Angles      = AL::ALValue::array(0.0f, 0.0f);
	AL::ALValue Times		= AL::ALValue::array(0.0f, 0.0f);
	AL::ALValue Stiffon		= AL::ALValue::array(1.0f, 1.0f);
	AL::ALValue Stiffoff	= AL::ALValue::array(0.0f, 0.0f);
	
	Angles.clear();
	Angles.arraySetSize(2);
	Angles[0] = AL::ALValue::array(-1.3265f);
	Angles[1] = AL::ALValue::array(1.3265f);

	Times.clear();
	Times.arraySetSize(2);
	Times[0] = AL::ALValue::array(2.0f);
	Times[1] = AL::ALValue::array(2.0f);

	isAbsolute = true;

	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);
	motion.setStiffnesses(Names, Stiffon);

	Names       = AL::ALValue::array("RElbowRoll", "LElbowRoll");
    	
	Angles.clear();
	Angles.arraySetSize(2);
	Angles[0] = AL::ALValue::array(0.0349f);
	Angles[1] = AL::ALValue::array( -0.0349f);

	Times.clear();
	Times.arraySetSize(2);
	Times[0] = AL::ALValue::array(1.0f);
	Times[1] = AL::ALValue::array(1.0f);

	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);
	motion.setStiffnesses(Names, Stiffoff);

	Names       = AL::ALValue::array("RElbowYaw", "LElbowYaw");
    
	Angles.clear();
	Angles.arraySetSize(2);
	Angles[0] = AL::ALValue::array(-2.0857f, 2.0857f, 0.8f);
	Angles[1] = AL::ALValue::array( 2.0857f, -2.0857f, -0.8f);

	Times.clear();
	Times.arraySetSize(2);
	Times[0] = AL::ALValue::array(1.0f, 2.0f, 3.0f);
	Times[1] = AL::ALValue::array(1.0f, 2.0f, 3.0f);

	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);
	motion.setStiffnesses(Names, Stiffoff);

	Names       = AL::ALValue::array("RWristYaw", "LWristYaw");
    
	Angles.clear();
	Angles.arraySetSize(2);
	Angles[0] = AL::ALValue::array(-1.8238f, 1.8238f, 1.4f);
	Angles[1] = AL::ALValue::array(1.8238f, -1.8238f, -1.4f);

	Times.clear();
	Times.arraySetSize(2);
	Times[0] = AL::ALValue::array(1.0f, 2.0f, 3.0f);
	Times[1] = AL::ALValue::array(1.0f, 2.0f, 3.0f);

	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);
	motion.setStiffnesses(Names, Stiffoff);

	motion.openHand("RHand");
	motion.openHand("LHand");
    motion.stiffnessInterpolation("RHand", 1.0f, 0.2f);
	motion.stiffnessInterpolation("LHand", 1.0f, 0.2f);

	Names       = AL::ALValue::array("RShoulderRoll", "LShoulderRoll");
	
	Angles.clear();
	Angles.arraySetSize(2);
	Angles[0] = AL::ALValue::array(0.0f);
	Angles[1] = AL::ALValue::array(0.0f);

	Times.clear();
	Times.arraySetSize(2);
	Times[0] = AL::ALValue::array(1.0f);
	Times[1] = AL::ALValue::array(1.0f);
	
	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);

	Names       = AL::ALValue::array("RShoulderPitch", "LShoulderPitch");
	
	Angles.clear();
	Angles.arraySetSize(2);
	Angles[0] = AL::ALValue::array(-1.457f, 1.8857f, 1.5857f);
	Angles[1] = AL::ALValue::array(1.8857f, -1.457f, 1.5857f);

	Times.clear();
	Times.arraySetSize(2);
	Times[0] = AL::ALValue::array(2.0f, 4.0f, 6.0f);
	Times[1] = AL::ALValue::array(2.0f, 4.0f, 6.0f);

	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);

	motion.closeHand("RHand");
	motion.closeHand("LHand");
	motion.stiffnessInterpolation("RHand", 0.0f, 0.2f);
	motion.stiffnessInterpolation("LHand", 0.0f, 0.2f);
	
	//Legs
	
	Names       = AL::ALValue::array("RHipPitch", "LHipPitch", "RKneePitch", "LKneePitch", "RAnklePitch", "LAnklePitch");
	
	Angles.clear();
	Angles.arraySetSize(6);
	Angles[0] = AL::ALValue::array(-0.72f);
	Angles[1] = AL::ALValue::array(-0.72f);
	Angles[2] = AL::ALValue::array(2.1f);
	Angles[3] = AL::ALValue::array(2.1f);
	Angles[4] = AL::ALValue::array(-1.1f);
	Angles[5] = AL::ALValue::array(-1.1f);
	
	Times.clear();
	Times.arraySetSize(6);
	Times[0] = AL::ALValue::array(3.0f);
	Times[1] = AL::ALValue::array(3.0f);
	Times[2] = AL::ALValue::array(3.0f);
	Times[3] = AL::ALValue::array(3.0f);
	Times[4] = AL::ALValue::array(3.0f);
	Times[5] = AL::ALValue::array(3.0f);
	isAbsolute = true;
	Stiffon		= AL::ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	Stiffoff	= AL::ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);
	motion.setStiffnesses(Names, Stiffon);

	Names       = AL::ALValue::array("RShoulderRoll", "LShoulderRoll", "RShoulderPitch", "LShoulderPitch");

	Stiffon		= AL::ALValue::array(1.0f, 1.0f, 1.0f, 1.0f);
	Stiffoff	= AL::ALValue::array(0.0f, 0.0f, 0.0f, 0.0f);
	
	Angles.clear();
	Angles.arraySetSize(4);
	Angles[0] = AL::ALValue::array(-1.3265f, -1.3265f, 0.0f, -1.3265f,-1.3265f, 0.0f);
	Angles[1] = AL::ALValue::array(1.3265f, 1.3265f, 0.0f, 1.3265f, 1.3265f, 0.0f);
	Angles[2] = AL::ALValue::array(1.5f, 0.0f, 0.0f, 0.0f, 1.5f, 1.5f);
	Angles[3] = AL::ALValue::array(1.5f, 0.0f, 0.0f, 0.0f, 1.5f, 1.5f);

	Times.clear();
	Times.arraySetSize(4);
	Times[0] = AL::ALValue::array(2.0f, 4.0f, 6.0f, 8.0f, 10.0f, 12.0f);
	Times[1] = AL::ALValue::array(2.0f, 4.0f, 6.0f, 8.0f, 10.0f, 12.0f);
	Times[2] = AL::ALValue::array(2.0f, 4.0f, 6.0f, 8.0f, 10.0f, 12.0f);
	Times[3] = AL::ALValue::array(2.0f, 4.0f, 6.0f, 8.0f, 10.0f, 12.0f);
	isAbsolute = true;

	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);
	motion.setStiffnesses(Names, Stiffon);

	
	Names       = AL::ALValue::array("RHipPitch", "LHipPitch", "RKneePitch", "LKneePitch", "RAnklePitch", "LAnklePitch");

	Angles.clear();
	Angles.arraySetSize(6);
	Angles[0] = AL::ALValue::array(-0.8f, 0.0f);
	Angles[1] = AL::ALValue::array(-0.8f, 0.0f);
	Angles[2] = AL::ALValue::array(2.1f, 0.0f);
	Angles[3] = AL::ALValue::array(2.1f, 0.0f);
	Angles[4] = AL::ALValue::array(-1.1f, 0.0f);
	Angles[5] = AL::ALValue::array(-1.1f, 0.0f);
	
	Times.clear();
	Times.arraySetSize(6);
	Times[0] = AL::ALValue::array(3.0f, 6.0f);
	Times[1] = AL::ALValue::array(3.0f, 6.0f);
	Times[2] = AL::ALValue::array(3.0f, 6.0f);
	Times[3] = AL::ALValue::array(3.0f, 6.0f);
	Times[4] = AL::ALValue::array(3.0f, 6.0f);
	Times[5] = AL::ALValue::array(3.0f, 6.0f);
	isAbsolute = true;
	
	Stiffon		= AL::ALValue::array(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
	Stiffoff	= AL::ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);

	robotPosture.goToPosture("Stand", 0.5f);

	Names       = AL::ALValue::array("RShoulderRoll", "RElbowYaw", "RElbowRoll", "RShoulderPitch");
    Angles      = AL::ALValue::array(-0.3f, 0.9f, 2.3f, .0f);
    
	fractionMaxSpeed  = 0.1f;
 
    motion.setStiffnesses(Names, AL::ALValue::array(1.0f, 1.0f, 1.0f, 1.0f));
    qi::os::sleep(1.0f);
    motion.setAngles(Names, Angles, fractionMaxSpeed);
    qi::os::sleep(1.0f);
    motion.setStiffnesses(Names, AL::ALValue::array(1.0f, 1.0f, 1.0f, 1.0f));

	motion.openHand("RHand");
    motion.stiffnessInterpolation("RHand", 0.0f, 1.0f);
	motion.closeHand("RHand");
	motion.stiffnessInterpolation("RHand", 0.0f, 0.2f);

	Names       = AL::ALValue::array("RShoulderRoll", "LShoulderRoll");
	
	Angles.clear();
	Angles.arraySetSize(2);
	Angles[0] = AL::ALValue::array(0.0f);
	Angles[1] = AL::ALValue::array(0.0f);

	Times.clear();
	Times.arraySetSize(2);
	Times[0] = AL::ALValue::array(0.5f);
	Times[1] = AL::ALValue::array(0.5f);

	Stiffon		= AL::ALValue::array(1.0f, 1.0f);
	Stiffoff	= AL::ALValue::array(0.0f, 0.0f);
	
	motion.setStiffnesses(Names, Stiffon);
    motion.angleInterpolation(Names, Angles, Times, isAbsolute);

	tts.say(phraseToSay2);
	robotPosture.goToPosture("Crouch", 1.0f);
	motion.rest();

	}
  catch (const AL::ALError& e) {
    std::cerr << "Caught exception: " << e.what() << std::endl;
    exit(1);
  }
  exit(0);
}
