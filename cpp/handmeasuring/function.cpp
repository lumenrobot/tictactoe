#include "function.h"

Position6D Matrix2PosOrient(Transform T)
{
	vector<float> pr(6,0.0f);
	Position6D Result;
	
	pr[0] = T.r1_c4;
	pr[1] = T.r2_c4;
	pr[2] = T.r3_c4;
	pr[3] = atan2f(T.r3_c2,T.r3_c3);
	pr[4] = atan2f(-T.r3_c1,sqrt(pow(T.r3_c2,2)+pow(T.r3_c3,2)));
	pr[5] = atan2f(T.r2_c1,T.r1_c1);

	Result = Position6D(pr[0],pr[1],pr[2],pr[3],pr[4],pr[5]);

	return(Result);
}

Transform HeadForwardHipBase(ALValue theta, ALValue theta0)
{
	vector<float> angle;
	vector<float> thetahip(1,0.0f);
	Transform RotYtheta0;
	Transform RotZtheta1;
	Transform RotZtheta2;
	Transform THead1;
	Transform THead2;
	Transform hbTtb;
	Transform Result;

	theta.ToFloatArray(angle,false);
	theta0.ToFloatArray(thetahip,false);

	RotYtheta0 = Transform::fromRotY(thetahip[0]);
	RotZtheta1 = Transform::fromRotZ(angle[0]);
	RotZtheta2 = Transform::fromRotZ(angle[1]);

	THead1 = RotZtheta1;
	THead2 = RotXhalfpiMinus*RotZtheta2;
	hbTtb = RotYtheta0*Atheta0;

	Result = hbTtb*AHead*THead1*THead2*RXHead*AHeadEndBottom;

	return (Result);
}

Transform LArmForwardHipBase(ALValue theta, ALValue theta0)
{
	vector<float> angle;
	vector<float> thetahip(1,0.0f);
	Transform RotYtheta0;
	Transform RotZtheta1;
	Transform RotZtheta2;
	Transform RotZtheta3;
	Transform RotZtheta4;
	Transform RotZtheta5;
	Transform TransZd3;
	Transform TransZd5;
	Transform TLArm1;
	Transform TLArm2;
	Transform TLArm3;
	Transform TLArm4;
	Transform TLArm5;
	Transform hbTtb;
	Transform Result;

	theta.ToFloatArray(angle,false);
	theta0.ToFloatArray(thetahip,false);

	RotYtheta0 = Transform::fromRotY(thetahip[0]);
	RotZtheta1 = Transform::fromRotZ(angle[0]);
	RotZtheta2 = Transform::fromRotZ(angle[1]-halfpi);
	RotZtheta3 = Transform::fromRotZ(angle[2]);
	RotZtheta4 = Transform::fromRotZ(angle[3]);
	RotZtheta5 = Transform::fromRotZ(angle[4]);
	TransZd3 = Transform::fromPosition(0.0f,0.0f,d3);
	TransZd5 = Transform::fromPosition(0.0f,0.0f,d5);

	TLArm1 = RotXhalfpiMinus*RotZtheta1;
	TLArm2 = RotXhalfpiPlus*RotZtheta2;
	TLArm3 = RotXhalfpiMinus*RotZtheta3*TransZd3;
	TLArm4 = RotXhalfpiPlus*RotZtheta4;
	TLArm5 = RotXhalfpiMinus*RotZtheta5*TransZd5;
	hbTtb = RotYtheta0*Atheta0;

	Result = hbTtb*ALArm*TLArm1*TLArm2*TLArm3*TLArm4*TLArm5*RXLArm*RZLArm*ALArmEnd5;

	return (Result);
}

Transform RArmForwardHipBase(ALValue theta, ALValue theta0)
{
	vector<float> angle;
	vector<float> thetahip(1,0.0f);
	Transform RotYtheta0;
	Transform RotZtheta1;
	Transform RotZtheta2;
	Transform RotZtheta3;
	Transform RotZtheta4;
	Transform RotZtheta5;
	Transform TransZd3;
	Transform TransZd5;
	Transform TRArm1;
	Transform TRArm2;
	Transform TRArm3;
	Transform TRArm4;
	Transform TRArm5;
	Transform hbTtb;
	Transform Result;

	theta.ToFloatArray(angle,false);
	theta0.ToFloatArray(thetahip,false);

	RotYtheta0 = Transform::fromRotY(thetahip[0]);
	RotZtheta1 = Transform::fromRotZ(-angle[0]);
	RotZtheta2 = Transform::fromRotZ(angle[1]+halfpi);
	RotZtheta3 = Transform::fromRotZ(angle[2]);
	RotZtheta4 = Transform::fromRotZ(angle[3]);
	RotZtheta5 = Transform::fromRotZ(angle[4]);
	TransZd3 = Transform::fromPosition(0.0f,0.0f,d3);
	TransZd5 = Transform::fromPosition(0.0f,0.0f,d5);

	TRArm1 = RotXhalfpiPlus*RotZtheta1;
	TRArm2 = RotXhalfpiMinus*RotZtheta2;
	TRArm3 = RotXhalfpiPlus*RotZtheta3*TransZd3;
	TRArm4 = RotXhalfpiMinus*RotZtheta4;
	TRArm5 = RotXhalfpiPlus*RotZtheta5*TransZd5;
	hbTtb = RotYtheta0*Atheta0;

	Result = hbTtb*ARArm*TRArm1*TRArm2*TRArm3*TRArm4*TRArm5*RXRArm*RZRArm*ARArmEnd5;

	return (Result);
}