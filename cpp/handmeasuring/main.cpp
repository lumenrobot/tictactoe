#include <signal.h>
#include <boost/shared_ptr.hpp>
#include <alcommon/albroker.h>
#include <alcommon/almodule.h>
#include <alcommon/albrokermanager.h>
#include <alcommon/altoolsmain.h>
#include <fstream>

#include "handmeasuring.h"
#include "function.h"

#ifdef HANDMEASURING_IS_REMOTE
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
	AL::ALModule::createModule<HandMeasuring>(pBroker, "HandMeasuring");

    return 0;
  }

  ALCALL int _closeModule()
  {
    return 0;
  }

}

#ifdef HANDMEASURING_IS_REMOTE

int main(int argc, char *argv[])
{
  TMainType sig;
  sig = &_createModule;

    ALTools::mainFunction("handmeasuring", argc, argv, sig);

}
#endif