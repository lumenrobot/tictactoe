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

const float pi = 3.1415926535f;
const float halfpi = pi/2;

const float shoulderOffsetY = 0.098f;
const float shoulderOffsetZ = 0.100f;
const float elbowOffsetY = 0.015f;
const float handOffsetX = 0.05775f;
const float upperArmLength = 0.105f;
const float lowerArmLength = 0.05595f;
const float HandOffsetZ = 0.01231f;
const float HipOffsetZ = 0.085f;
const float NeckOffsetZ = 0.1265f;
const float CameraTopX = 0.0539f;
const float CameraTopZ = 0.0679f;
const float CameraBottomX = 0.0488f;
const float CameraBottomZ = 0.02381f;

const float l0 = HipOffsetZ;
const float l1 = shoulderOffsetY+elbowOffsetY;
const float l2 = shoulderOffsetZ;
const float l3 = handOffsetX;
const float l4 = HandOffsetZ;
const float d3 = upperArmLength;
const float d5 = lowerArmLength;
const float lh1 = NeckOffsetZ;
const float lht2 = CameraTopX;
const float lht3 = CameraTopZ;
const float lhb2 = CameraBottomX;
const float lhb3 = CameraBottomZ;

const Transform RotXhalfpiPlus = Transform::fromRotX(halfpi);
const Transform RotXhalfpiMinus = Transform::fromRotX(-halfpi);
const Transform RotZhalfpiPlus = Transform::fromRotZ(halfpi);
const Transform RotZhalfpiMinus = Transform::fromRotZ(-halfpi);

const Transform Atheta0 = Transform::fromPosition(0.0f,0.0f,l0);

const Transform AHead = Transform::fromPosition(0.0f,0.0f,lh1);
const Transform RXHead = RotXhalfpiPlus; 
const Transform AHeadEndBottom = Transform::fromPosition(lhb2,0.0f,lhb3);

const Transform ALArm = Transform::fromPosition(0.0f,l1,l2);
const Transform RZLArm = RotZhalfpiPlus;
const Transform RXLArm = RotXhalfpiPlus;
const Transform ALArmEnd5 = Transform::fromPosition(l3,0.0f,-l4);

const Transform ARArm = Transform::fromPosition(0.0f,-l1,l2);
const Transform RZRArm = RotZhalfpiMinus;
const Transform RXRArm = RotXhalfpiMinus;
const Transform ARArmEnd5 = Transform::fromPosition(l3,0.0f,-l4);

Position6D Matrix2PosOrient(Transform T);

Transform HeadForwardHipBase(ALValue theta, ALValue theta0);

Transform LArmForwardHipBase(ALValue theta, ALValue theta0);

Transform RArmForwardHipBase(ALValue theta, ALValue theta0);