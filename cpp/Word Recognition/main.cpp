/*
 Created by Sebastian A. Nugroho
 16 February, 2014
 */

#include <signal.h>
#include <boost/shared_ptr.hpp>
#include <alcommon/albroker.h>
#include <alcommon/almodule.h>
#include <alcommon/albrokermanager.h>
#include <alcommon/altoolsmain.h>

#include "scenetwo.h"

#ifdef SCENETWO_IS_REMOTE
# define ALCALL
#else
# ifdef _WIN32
#  define ALCALL __declspec(dllexport)
# else
#   define ALCALL
# endif
#endif

extern "C"
{

  ALCALL int _createModule( boost::shared_ptr<AL::ALBroker> pBroker )
  {
    AL::ALBrokerManager::setInstance(pBroker->fBrokerManager.lock());
    AL::ALBrokerManager::getInstance()->addBroker(pBroker);
	AL::ALModule::createModule<SceneTwo>(pBroker, "SceneTwo");

    return 0;
  }

  ALCALL int _closeModule()
  {
    return 0;
  }

}

#ifdef SCENETWO_IS_REMOTE

int main(int argc, char *argv[])
{
  TMainType sig;
  sig = &_createModule;

    ALTools::mainFunction("scenetwo", argc, argv, sig);
  
}
#endif