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
//using namespace std;

int main(int argc, char* argv[]) {

	int i;
	int n;
	float x;

  if(argc != 2)
  {
    std::cerr << "Wrong number of arguments!" << std::endl;
    std::cerr << "Usage: movehead NAO_IP" << std::endl;
    exit(2);
  }

	const std::string phraseToSay1 = "Starting walking square module with 1 turn";
	const std::string phraseToSay2 = "Starting walking square module with 2 turns";
	const std::string phraseToSay3 = "Starting walking square module with 3 turns";
	const std::string phraseToSay4 = "Starting walking square module with 4 turns";
	const std::string phraseToSay5 = "Starting walking square module with 5 turns";
	const std::string phraseToSay6 = "Starting walking square module with 6 turns";
	const std::string phraseToSay7 = "Starting walking square module with 7 turns";
	const std::string phraseToSay8 = "Starting walking square module with 8 turns";
	const std::string phraseToSay9 = "Starting walking square module with 9 turns";
	const std::string phraseToSay10 = "Starting walking square module with 10 turns";
	const std::string phraseToSay = "Walking square module is complete";

    try {
    std::cout << "Masukkan jumlah putaran (bilangan bulat 1-10): "; 
	std::cin >> n;
	std::cout << "Masukkan jarak (m): "; 
	std::cin >> x;

    AL::ALMotionProxy motion(argv[1], 9559);
    AL::ALRobotPostureProxy robotPosture(argv[1], 9559);
	AL::ALTextToSpeechProxy tts(argv[1], 9559);

	motion.wakeUp();
	robotPosture.goToPosture("Crouch", 1.0f);
	robotPosture.goToPosture("Stand", 1.0f);
	float fractionMaxSpeed  = 0.2f; 

	switch (n) {
	case 1:
	tts.say(phraseToSay1);
	break;
	case 2:
	tts.say(phraseToSay2);
	break;
	case 3:
	tts.say(phraseToSay3);
	break;
	case 4:
	tts.say(phraseToSay4);
	break;
	case 5:
	tts.say(phraseToSay5);
	break;
	case 6:
	tts.say(phraseToSay6);
	break;
	case 7:
	tts.say(phraseToSay7);
	break;
	case 8:
	tts.say(phraseToSay8);
	break;
	case 9:
	tts.say(phraseToSay9);
	break;
	case 10:
	tts.say(phraseToSay10);
	break;
	}	

	bool isEnabled = true;
	motion.wbEnable(isEnabled);
	std::cout << "Whole body enabled." << std::endl;

	i=1;
	while(i<=n*4) {
	
	float y  = 0.0f;
	float theta = 0.0f;
	motion.moveTo(x, y, theta);

	theta  =1.57f;
	motion.walkTo(0.0f, y, theta);

	i++;
	}

	isEnabled = false;
	motion.wbEnable(isEnabled);
	std::cout << "Whole body disabled." << std::endl;

	tts.say(phraseToSay);
	robotPosture.goToPosture("Crouch", 1.0f);
	motion.rest();

	}
  catch (const AL::ALError& e) {
    std::cerr << "Caught exception: " << e.what() << std::endl;
    exit(1);
  }
  exit(0);
}
