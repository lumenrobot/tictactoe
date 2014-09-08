/*
 * Copyright (c) 2012 Aldebaran Robotics. All rights reserved.
 * Use of this source code is governed by a BSD-style license that can be
 * found in the COPYING file.
 */
#include <iostream>

#include <alerror/alerror.h>
#include <alproxies/almotionproxy.h>


/*
int main()
{
  std::cout << "Hello, world" << std::endl;
  return 0;
}
//*/

int main(int argc, char* argv[]) {

  if(argc != 2)
  {
    std::cerr << "Wrong number of arguments!" << std::endl;
    std::cerr << "Usage: movehead NAO_IP" << std::endl;
    exit(2);
  }

  /** The name of the chain or joint to be moved. */
  //headChain consist of HeadYaw and HeadPitch
  const AL::ALValue headChain = "Head";
  //Yaw=menyimpang/menoleh
  const AL::ALValue headYaw = "HeadYaw";
  //Pitch=anggukan
  const AL::ALValue headPitch = "HeadPitch";


  try {
    /** Create a ALMotionProxy to call the methods to move NAO's head.
    * Arguments for the constructor are:
    * - IP adress of the robot
    * - port on which NAOqi is listening, by default 9559
    */
    AL::ALMotionProxy motion(argv[1], 9559);

    /** Make sure the head is stiff to be able to move it.
    * To do so, make the stiffness go to the maximum in one second.
    */
    /** Target stiffness. */
    AL::ALValue stiffnessOn = 1.0f;
    AL::ALValue stiffnessOff = 0.0f;
    /** Time (in seconds) to reach the target. */
    AL::ALValue time = 1.0f;
    /** Call the stiffness interpolation method. */
	motion.stiffnessInterpolation(headChain, stiffnessOn, time);

    /** Set the target angle list, in radians. */
    AL::ALValue targetAngles = AL::ALValue::array(-1.2f, 1.2f, -1.2f, 1.2f,0.0f);
    /** Set the corresponding time lists, in seconds. */
    AL::ALValue targetTimes = AL::ALValue::array(3.0f, 9.0f, 15.0f, 21.0f, 24.0f);
    /** Specify that the desired angles are absolute. */
    bool isAbsolute = true;

    /** Call the angle interpolation method. The joint will reach the
    * desired angles at the desired times.
    */
	std::cout<<"Moving HeadPitch"<<std::endl;
    motion.angleInterpolation("HeadPitch", 0.0f, 2.0f, isAbsolute);

    motion.post.angleInterpolation(headYaw, targetAngles, targetTimes, isAbsolute);
    motion.post.angleInterpolation("HeadPitch", -0.6f, 4.0f, isAbsolute);


    /** Remove the stiffness on the head. */
    time = 1.0f;
    motion.stiffnessInterpolation(headChain, stiffnessOff, time);

  }
  catch (const AL::ALError& e) {
    std::cerr << "Caught exception: " << e.what() << std::endl;
    exit(1);
  }
  exit(0);
}