#ifndef PLAYTICTACTOE_H
#define PLAYTICTACTOE_H

#include <boost/shared_ptr.hpp>
#include <alcommon/almodule.h>
#include <string>
#include <alproxies/almemoryproxy.h>
#include <alproxies/almotionproxy.h>
#include <alproxies/alrobotpostureproxy.h>
#include <alproxies/altexttospeechproxy.h>
#include <althread/almutex.h>
#include <iostream>
#include <fstream>
#include <math.h>
#include <qi/os.hpp>
#include <qi/log.hpp>
#include <althread/alcriticalsection.h>
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

using namespace std;
using namespace	AL;
using namespace Math;

namespace AL
{
  class ALBroker;
}

class PlayTicTacToe : public AL::ALModule
{

public:

  PlayTicTacToe(boost::shared_ptr<AL::ALBroker> broker, const std::string& name);

  virtual ~PlayTicTacToe();

  virtual void init();

  void startMove();


private:
	  
  AL::ALMemoryProxy fMemoryProxy;
  boost::shared_ptr<AL::ALMutex> fCallbackMutex;
  AL::ALRobotPostureProxy robotPosture;
  AL::ALMotionProxy motion;
  AL::ALTextToSpeechProxy tts;

  int state;
  float pix;
  float piy;
  float piz;
  float pfx;
  float pfy;
  float pfz;

};

#endif  
