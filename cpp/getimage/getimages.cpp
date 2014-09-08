/**
 *
 * This example demonstrates how to get images from the robot remotely and how
 * to display them on your screen using opencv.
 *
 * Copyright Aldebaran Robotics
 */

// Aldebaran includes.
#include <alproxies/alvideodeviceproxy.h>
#include <alvision/alimage.h>
#include <alvision/alvisiondefinitions.h>
#include <alerror/alerror.h>

// Opencv includes.
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>


#include <iostream>
#include <string>

using namespace AL;


/**
* \brief Shows images retrieved from the robot.
*
* \param robotIp the IP adress of the robot
*/
void showImages(const std::string& robotIp,const std::string& subscriber,int quality )
{
  /** Create a proxy to ALVideoDevice on the robot.*/
  ALVideoDeviceProxy camProxy(robotIp, 9559);

  /** Subscribe a client image requiring 320*240 and BGR colorspace.*/
  const std::string clientName = camProxy.subscribe(subscriber, quality, kBGRColorSpace, 30);
  
  cv::Mat imgHeader;
  /** Create an cv::Mat header to wrap into an opencv image.*/
  if (quality==0){
	imgHeader = cv::Mat(cv::Size(160, 120), CV_8UC3);
  } else if(quality==1){
	  imgHeader = cv::Mat(cv::Size(320, 240), CV_8UC3);
  } else if(quality==2){
	  imgHeader = cv::Mat(cv::Size(640, 480), CV_8UC3);
  }else if(quality==3){
	  imgHeader = cv::Mat(cv::Size(1280, 960), CV_8UC3);
  }
		
  /** Create a OpenCV window to display the images. */
  cv::namedWindow("images");

  /** Main loop. Exit when pressing ESC.*/
  while ((char) cv::waitKey(30) != 27)
  {
    /** Retrieve an image from the camera.
    * The image is returned in the form of a container object, with the
    * following fields:
    * 0 = width
    * 1 = height
    * 2 = number of layers
    * 3 = colors space index (see alvisiondefinitions.h)
    * 4 = time stamp (seconds)
    * 5 = time stamp (micro seconds)
    * 6 = image buffer (size of width * height * number of layers)
    */
    ALValue img = camProxy.getImageRemote(clientName);

    /** Access the image buffer (6th field) and assign it to the opencv image
    * container. */
    imgHeader.data = (uchar*) img[6].GetBinary();

    /** Tells to ALVideoDevice that it can give back the image buffer to the
    * driver. Optional after a getImageRemote but MANDATORY after a getImageLocal.*/
    camProxy.releaseImage(clientName);

    /** Display the iplImage on screen.*/
    cv::imshow("images", imgHeader);
  }

  /** Cleanup.*/
  camProxy.unsubscribe(clientName);
}



int main(int argc, char* argv[])
{
  if (argc < 4)
  {
    std::cerr << "Usage 'getimages robotIp subscriber-user quality'" << std::endl;
	std::cerr << "put the number for chosen quality: k960p  = 3,k4VGA  = 3, kVGA   = 2,kQVGA  = 1,kQQVGA = 0" << std::endl;

    return 1;
  }

  const std::string robotIp(argv[1]);

  try
  {
    showImages(robotIp,argv[2],atoi(argv[3]));
  }
  catch (const AL::ALError& e)
  {
    std::cerr << "Caught exception " << e.what() << std::endl;
  }

  return 0;
}

