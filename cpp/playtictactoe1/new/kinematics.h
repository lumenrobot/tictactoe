#ifndef KINEMATICS_H
#define KINEMATICS_H

#include <iostream>
#include <alerror/alerror.h>
#include <alproxies/almotionproxy.h>
#include <alproxies/alrobotpostureproxy.h>
#include <qi/os.hpp>
#include <alproxies/altexttospeechproxy.h>
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

using namespace std;
using namespace	AL;
using namespace Math;

const float pi = 3.1415926535f;
const float halfpi = pi/2;

static float a;

const float ShoulderOffsetY = 0.098f;
const float ShoulderOffsetZ = 0.100f;
const float ElbowOffsetY = 0.015f;
const float HandOffsetX = 0.05775f;
const float UpperArmLength = 0.105f;
const float LowerArmLength = 0.05595f;
const float HandOffsetZ = 0.01231f;
const float HipOffsetZ = 0.085f;
const float NeckOffsetZ = 0.1265f;
const float CameraTopX = 0.0539f;
const float CameraTopZ = 0.0679f;
const float CameraBottomX = 0.0488f;
const float CameraBottomZ = 0.02381f;

const float l0 = HipOffsetZ;
const float l1 = ShoulderOffsetY+ElbowOffsetY;
const float l2 = ShoulderOffsetZ;
const float l3 = HandOffsetX;
const float l4 = HandOffsetZ;
const float d3 = UpperArmLength;
const float d5 = LowerArmLength;
const float lh1 = NeckOffsetZ;
const float lht2 = CameraTopX;
const float lht3 = CameraTopZ;
const float lhb2 = CameraBottomX;
const float lhb3 = CameraBottomZ;

//Basic rotation matrices
const Transform RotXhalfpiPlus = Transform::fromRotX(halfpi);
const Transform RotXhalfpiMinus = Transform::fromRotX(-halfpi);
const Transform RotZhalfpiPlus = Transform::fromRotZ(halfpi);
const Transform RotZhalfpiMinus = Transform::fromRotZ(-halfpi);

//Hip Base matrix
const Transform Atheta0 = Transform::fromPosition(0.0f,0.0f,l0);

//Head matrices
const Transform AHead = Transform::fromPosition(0.0f,0.0f,lh1);
const Transform RXHead = RotXhalfpiPlus; 
const Transform AHeadEndBottom = Transform::fromPosition(lhb2,0.0f,lhb3);

//Left Arm matrices
const Transform ALArm = Transform::fromPosition(0.0f,l1,l2);
const Transform RZLArm = RotZhalfpiPlus;
const Transform RXLArm = RotXhalfpiPlus;
const Transform ALArmEnd5 = Transform::fromPosition(l3,0.0f,-l4);

//Right Arm matrices
const Transform ARArm = Transform::fromPosition(0.0f,-l1,l2);
const Transform RZRArm = RotZhalfpiMinus;
const Transform RXRArm = RotXhalfpiMinus;
const Transform ARArmEnd5 = Transform::fromPosition(l3,0.0f,-l4);

//Degree to radian function
float DtR1(float deg);

//Position and orientation to transformation matrix
Transform PosOrient2Matrix(Position6D pos6D);

//Transformation matrix to Position and Orientation
Position6D Matrix2PosOrient(Transform T);

//Rotation Matrix to Rotation3D
Rotation3D Matrix2Orient(Rotation R);

//Rotation Matrix about an arbitrary fram r with fi angle
Rotation RotArbitrary(ALValue ro, float fi);

//Head kinematics
Transform HeadForwardHipBase(ALValue theta, ALValue theta0);

ALValue HeadInverseHipBase(Transform T, ALValue theta0);

//Left Arm kinematics
Transform LArmForwardHipBase(ALValue theta, ALValue theta0);

ALValue LArmInverseHipBase(Transform T, ALValue theta0);

//Right Arm kinematics
Transform RArmForwardHipBase(ALValue theta, ALValue theta0);

ALValue RArmInverseHipBase(Transform T, ALValue theta0);

//Trajectory generation
vector<Position6D> TrajectoryGenerator(Position6D POi, Position6D POf, float tmax); //initial is higher than final

vector<Position6D> TrajectoryGeneratorReverse(Position6D POi, Position6D POf, float tmax); //final is higher than initial

//Time generation
ALValue TimeGenerator(vector<Position6D> PO, float tmax);

ALValue TimeGeneratorReverse(vector<Position6D> PO, float tmax);

#endif