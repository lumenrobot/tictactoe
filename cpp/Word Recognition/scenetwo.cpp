#include <alvalue/alvalue.h>
#include <alcommon/alproxy.h>
#include <alcommon/albroker.h>
#include <iostream>
#include <alerror/alerror.h>
#include <althread/alcriticalsection.h>
#include <qi/log.hpp>
#include <qi/os.hpp>
#include <alproxies/alredballtrackerproxy.h>
#include <almath/tools/almath.h>
#include <almath/tools/almathio.h>
#include <almath/tools/altransformhelpers.h>
#include <almath/tools/aldubinscurve.h>
#include <almath/tools/altrigonometry.h>
#include <almath/tools/avoidfootcollision.h>
#include <almath/types/alaxismask.h>
#include <almath/types/alpose2d.h>
#include <almath/types/alposition2d.h>
#include <almath/types/alposition3d.h>
#include <almath/types/alposition6d.h>
#include <almath/types/alpositionandvelocity.h>
#include <almath/types/alrotation.h>
#include <almath/types/alrotation3d.h>
#include <almath/types/altransform.h>
#include <almath/types/altransformandvelocity6d.h>
#include <almath/types/alvelocity3d.h>
#include <almath/types/alvelocity6d.h>
#include <math.h>

#include "scenetwo.h"

using namespace std;

SceneTwo::SceneTwo(
  boost::shared_ptr<AL::ALBroker> broker,
  const std::string& name) : AL::ALModule(broker, name), 
     fMemoryProxy(getParentBroker()),
	 fTtsProxy(getParentBroker()),
	 fSpeechRecognition(getParentBroker()),
	 robotPosture(getParentBroker()),
	 motion(getParentBroker()),
	 ballTracker(getParentBroker()),
	 fCallbackMutex(AL::ALMutex::createALMutex())
{
  setModuleDescription("This module combines several events and create a particular scenario.");
  
  functionName("playingBall",
               getName(),
               "Called by ALMemory when Lumen is going to play the ball.");
  BIND_METHOD(SceneTwo::playingBall);

  functionName("Start",
               getName(),
               "Called by ALMemory when Lumen is detecting a word.");
  BIND_METHOD(SceneTwo::Start);
  
  functionName("End",
               getName(),
               "Called by ALMemory when a word has been detected.");
  BIND_METHOD(SceneTwo::End);

  functionName("onSpeechRecognized",
               getName(),
               "Called by ALMemory when a word is recognized.");
  BIND_METHOD(SceneTwo::onSpeechRecognized);

}

void SceneTwo::init() {
	try {
	fSpeechRecognition.setLanguage("English");
	std::vector<std::string> wordlist(11,"");
	wordlist[0] = "Crouch";
	wordlist[1] = "Sit";
	wordlist[2] = "Stand";
	wordlist[3] = "Lie down";
	wordlist[4] = "Play the ball";
	wordlist[5] = "Hello";
	wordlist[6] = "Your name";
	wordlist[7] = "Who are you";
	wordlist[8] = "Introduce yourself";
	wordlist[9] = "Stop";
	wordlist[10] = "Hi";

	fSpeechRecognition.setVocabulary(wordlist,true);
	fTtsProxy.say("Hello master.");
	qi::os::msleep(100);
	fTtsProxy.say("	You may command me to sit, to crouch, to stand, to lie down, or even to play the ball."); 
	qi::os::msleep(100);
	fTtsProxy.say("What would you like me to do?");
	Start();
	}
  catch (const AL::ALError& e) {
    qiLogError("module.example") << e.what() << std::endl;
  }
}

void SceneTwo::Start()
{
	fSpeechRecognition.setAudioExpression(true);
	fMemoryProxy.subscribeToEvent("WordRecognized","SceneTwo","onSpeechRecognized");
}

void SceneTwo::End()
{
	fMemoryProxy.unsubscribeToEvent("WordRecognized","SceneTwo");
}


SceneTwo::~SceneTwo()
{

}

void SceneTwo::playingBall()
{     
	  ballTracker.startTracker();
	  i = 1;
	  n = 300;
	  std::vector<float> mtransparam1(12,0.0f);
	  mtransparam1[0] = 1.0f;
	  mtransparam1[1] = 0.0f;
	  mtransparam1[2] = 0.0f;
	  mtransparam1[3] = 0.0f;
      mtransparam1[4] = 0.0f;
	  mtransparam1[5] = 1.0f;
	  mtransparam1[6] = 0.0f;
	  mtransparam1[7] = -0.113f;
	  mtransparam1[8] = 0.0f;
	  mtransparam1[9] = 0.0f;
	  mtransparam1[10] = 1.0f;
	  mtransparam1[11] = 0.10f;
	  std::vector<float> mtransparam(12,0.0f);
	  phbb = AL::ALValue::array(3,0.0f);
	  names = AL::ALValue::array("RShoulderPitch", "RShoulderRoll", "RElbowRoll"); 
	  stif = AL::ALValue::array(0.8f, 0.7f, 0.5f); 
	  fractionMaxSpeed  = 0.3f;
	  namea = "RArm";
	  space = 0;
	  useSensorValues = true;
	  axisMask = 7;

	  bTo = AL::Math::Transform(mtransparam1);
	  oTb = AL::Math::transformInverse(bTo);

	  cout << "Matriks transformasi bTo" << endl;
	  cout << bTo << endl;
	  cout << "Matriks transformasi oTb" << endl;
	  cout << oTb << endl;

	  posballbase = ballTracker.getPosition();
	  motion.setStiffnesses(names,stif);
	
	  while(i<=n) {
    
	  posballbase = ballTracker.getPosition();
	  cout << "posballbase" << posballbase << endl;
	  posballzero = oTb*posballbase;
	  cout << "posballzero" << posballzero << endl;
	
	  pbonorm = AL::Math::norm(posballzero);
	  poshandballzero = ((0.2187f)/pbonorm)*posballzero;

	  poshandballbase = bTo*poshandballzero;
	  cout << "posballhandbase" << poshandballbase << endl;
	
      result = motion.getPosition(namea, space, useSensorValues);
      cout << namea << ":" << endl;
      cout << "Position (x, y, z): " << result.at(0) << ", " << result.at(1)
           << ", " << result.at(2) << endl;

	  phbb = poshandballbase.toVector();
	  xc = phbb[0];
	  yc = phbb[1];
	  zc = phbb[2];

	  theta1 = -atan2f(zc-0.095f,xc);
	  theta2 = atan2f(yc+0.113f,xc);

	  c1 = cos(theta1);
	  c2 = cos(theta2);
	  s1 = sin(theta1);
	  s2 = sin(theta2);

      angles = AL::ALValue::array(theta1, theta2, 0.0349f);
	  motion.setStiffnesses(names,stif);
	  motion.setAngles(names, angles, fractionMaxSpeed);

	  newpos = ballTracker.isNewData();

	  mtransparam[0] = c1*c2;
	  mtransparam[1] = -c1*s2;
	  mtransparam[2] = -s1; 
	  mtransparam[3] = 0.2187f*c1*c2; 
	  mtransparam[4] = s1*s2; 
	  mtransparam[5] = -s1*s2;
	  mtransparam[6] = c1;
	  mtransparam[7] = 0.2187f*s1*c2-0.113f; 
	  mtransparam[8] = -s2;
	  mtransparam[9] = -c2;
	  mtransparam[10] = 0.0f;
	  mtransparam[11] = -0.2187f*s2+0.1f;

	  bT2 = AL::Math::Transform(mtransparam);
	
	  poshandbase = bT2*poshandhand;
      cout << "poshandbase" << poshandbase << endl;
	  qi::os::msleep(100);
	  i = i+1;
	  }
	  ballTracker.stopTracker();  
}


void SceneTwo::onSpeechRecognized(const std::string& name,
                                             const AL::ALValue& val,
                                             const std::string& myName)
{
  
  for(unsigned int i = 0; i < val.getSize()/2 ; ++i)
  {
    std::cout << "word recognized: " << val[i*2].toString()
              << " with confidence: " << (float)val[i*2+1] << std::endl;

    if((std::string)val[i*2] == "Crouch" && (float)val[i*2+1] > 0.30)
    {
	  End();
      fTtsProxy.say("I will crouch.");
	  motion.wakeUp();
	  robotPosture.goToPosture("Crouch",1.0f);
	  motion.rest();
	  Start();
    }

	if((std::string)val[i*2] == "Sit" && (float)val[i*2+1] > 0.30)
    {
	  End();
      fTtsProxy.say("I will sit.");
	  motion.wakeUp();
	  robotPosture.goToPosture("SitRelax",1.0f);
	  motion.rest();
	  Start();
    }

	if((std::string)val[i*2] == "Stand" && (float)val[i*2+1] > 0.30)
    {
	  End();
      fTtsProxy.say("I will stand.");
	  motion.wakeUp();
	  robotPosture.goToPosture("Stand",1.0f);
	  Start();
    }

	if((std::string)val[i*2] == "Lie down" && (float)val[i*2+1] > 0.30)
    {
      End();
      fTtsProxy.say("I will lie down.");
	  motion.wakeUp();
	  robotPosture.goToPosture("LyingBack",1.0f);
	  motion.rest();
	  Start();
    }

	if((std::string)val[i*2] == "Play the ball" && (float)val[i*2+1] > 0.30)
    {
	  End();
      fTtsProxy.say("Playing the ball for 30 seconds.");
	  motion.wakeUp();
	  robotPosture.goToPosture("Crouch",1.0f);
	  playingBall();
	  robotPosture.goToPosture("Sit",1.0f);
	  motion.rest();
	  Start();
    }

	if(((std::string)val[i*2] == "Hello"|| "Hi") && (float)val[i*2+1] > 0.40)
    {
	End();
	AL::ALValue Names = AL::ALValue::array("RShoulderRoll", "RElbowYaw", "RElbowRoll", "RShoulderPitch");
    AL::ALValue Angles = AL::ALValue::array(0.3f, 0.91f, 3.1f, 0.0f);
    
	fractionMaxSpeed  = 0.4f;
 
    motion.setStiffnesses(Names, AL::ALValue::array(1.0f, 1.0f, 1.0f, 1.0f));
    motion.setAngles(Names, Angles, fractionMaxSpeed);

	motion.openHand("RHand");
    motion.stiffnessInterpolation("RHand", 0.0f, 1.0f);
	fTtsProxy.say("Well, hello!");
	motion.closeHand("RHand");
	motion.stiffnessInterpolation("RHand", 0.0f, 0.2f);
	qi::os::msleep(100);

	Angles = AL::ALValue::array(0.3f, 0.0f, 0.0349f, 1.57f);
	motion.setStiffnesses(Names, AL::ALValue::array(1.0f, 1.0f, 1.0f, 1.0f));
    motion.setAngles(Names, Angles, fractionMaxSpeed);
	Start();
    }

	if(((std::string)val[i*2] == "Your name" || "Who are you" || "Introduce yourself") && (float)val[i*2+1] > 0.40)
    {
	End();
    fTtsProxy.say("My name is Lumen");
	Start();
    }

	if((std::string)val[i*2] == "Stop" && (float)val[i*2+1] > 0.30)
    {
      End();
      fTtsProxy.say("Process will be stopped");
	  robotPosture.goToPosture("Crouch",1.0f);
	  motion.rest();
	  exit();
    }
  }
}
