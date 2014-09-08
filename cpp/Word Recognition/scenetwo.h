#ifndef SCENETWO_H
#define SCENETWO_H

#include <boost/shared_ptr.hpp>
#include <alcommon/almodule.h>
#include <string>
#include <alproxies/almemoryproxy.h>
#include <alproxies/almotionproxy.h>
#include <alproxies/alrobotpostureproxy.h>
#include <alproxies/altexttospeechproxy.h>
#include <alproxies/alspeechrecognitionproxy.h>
#include <alproxies/alredballtrackerproxy.h>
#include <althread/almutex.h>
#include <iostream>
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
using namespace std;

namespace AL
{
  class ALBroker;
}

class SceneTwo : public AL::ALModule
{

public:

  SceneTwo(boost::shared_ptr<AL::ALBroker> broker, const std::string& name);
  virtual ~SceneTwo();

    /** Overloading ALModule::init().
    * This is called right after the module has been loaded
    */
  virtual void init();

  void Start();

  void End();

  void playingBall();

  void onSpeechRecognized(const std::string& name,
                          const AL::ALValue& val,
                          const std::string& myName);


private:

      unsigned int i;
	  unsigned int n;

	  AL::Math::Position3D posballbase;
	  AL::Math::Transform bTo;
	  AL::Math::Transform oTb;
	  AL::Math::Transform bT2;
	  AL::Math::Position3D posballzero;
	  AL::Math::Position3D poshandballzero;
	  AL::Math::Position3D poshandballbase;
	  AL::Math::Position3D poshandbase;
	  AL::Math::Position3D poshandhand;
	  float pbonorm;
	  float theta1,theta2;
	  float c1,c2,s1,s2;
	  AL::ALValue phbb;
	  float xc,yc,zc;
	  AL::ALValue names;     
	  AL::ALValue stif;
      AL::ALValue angles;      
      float fractionMaxSpeed;
	  bool newpos;
	  string namea;
      int space;
      bool useSensorValues;
	  vector<float> result;
	  int axisMask;
  AL::ALMemoryProxy fMemoryProxy;
  AL::ALTextToSpeechProxy fTtsProxy;
  AL::ALSpeechRecognitionProxy fSpeechRecognition;
  boost::shared_ptr<AL::ALMutex> fCallbackMutex;
  AL::ALRobotPostureProxy robotPosture;
  AL::ALMotionProxy motion;
  AL::ALRedBallTrackerProxy ballTracker;
};

#endif  
