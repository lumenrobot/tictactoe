#include "kinematics.h"

using namespace std;
using namespace	AL;
using namespace Math;

float DtR1(float deg)
{
	return(deg*(pi/180));
}

Transform PosOrient2Matrix(Position6D pos6D) 
{
	vector<float> pr(6,0.0f);
	Transform Result;

	pr = pos6D.toVector();

	Result = (Transform::fromPosition(pr[0],pr[1],pr[2]))*(Transform::fromRotZ(pr[5]))*(Transform::fromRotY(pr[4]))*(Transform::fromRotX(pr[3]));

	return (Result);
}

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

Rotation3D Matrix2Orient(Rotation R)
{
	vector<float> r(3,0.0f);
	Rotation3D Result;

	r[0] = atan2f(R.r3_c2,R.r3_c3);
	r[1] = atan2f(-R.r3_c1,sqrt(pow(R.r3_c2,2)+pow(R.r3_c3,2)));
	r[2] = atan2f(R.r2_c1,R.r1_c1);

	Result = Rotation3D(r[0],r[1],r[2]);

	return(Result);
}

Rotation RotArbitrary(ALValue ro, float fi)
{
	float c,s,v;
	vector<float> r;
	vector<float> rr(9,0.0f);
	Rotation Result;

	ro.ToFloatArray(r,false);

	c = cosf(fi);
	s = sinf(fi);
	v = 1-c;

	rr[0] = pow(r[0],2)*v+c;
	rr[1] = r[0]*r[1]*v-r[2]*s;
	rr[2] = r[0]*r[2]*v+r[1]*s;
	rr[3] = r[0]*r[1]*v+r[2]*s;
	rr[4] = pow(r[1],2)*v+c;
	rr[5] = r[1]*r[2]*v-r[0]*s;
	rr[6] = r[0]*r[2]*v-r[1]*s;
	rr[7] = r[1]*r[2]*v+r[0]*s;
	rr[8] = pow(r[2],2)*v+c;

	Result = Rotation(rr);

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

	Result = hbTtb*AHead*THead1*THead2*RXHead*AHeadEndBottom*(Transform::fromRotY(DtR1(40))); //Added extra 40 degress due to bottom camera angle

	return (Result);
}

ALValue HeadInverseHipBase(Transform T, ALValue theta0)
{
	vector<float> thetahip(1,0.0f);
	float theta1 = 0.0f;
	float theta2 = 0.0f;
	float s0 = 0.0f;
	float c0 = 0.0f;
	float a = 0.0f;
	float xi = 0.0f;
	float argt = 0.0f;
	ALValue Result = ALValue::array(0.0f, 0.0f);

	theta0.ToFloatArray(thetahip,false);

	s0 = sinf(thetahip[0]);
	c0 = cosf(thetahip[0]);
	a = sqrt(pow(lhb2,2)+pow(lhb3,2));
	xi = atan2f(lhb3,lhb2);
	argt = (s0*T.r1_c4+c0*T.r3_c4-l0-lh1)/a;

	theta1 = atan2f(T.r2_c4,c0*T.r1_c4-s0*T.r3_c4);
	theta2 = xi-atan2f(argt,sqrt(1-pow(argt,2)));

	Result = ALValue::array(theta1,theta2);

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

ALValue LArmInverseHipBase(Transform T, ALValue theta0)
{
	vector<float> thetahip(1,0.0f);
	vector<float> positiondlhh(3,0.0f);
	vector<float> positiondlh0(3,0.0f);
	Position3D Pdlhh;
	Position3D Pdlh0;
	float normPobj0 = 0.0f;
	float theta1 = 0.0f;
	float theta2 = 0.0f;
	float theta3 = 0.0f;
	float theta4 = 0.0f;
	float theta5 = 0.0f;
	float argloc = 0.0f;
	float argl2 = 0.0f, argr21 = 0.0f, argr22 = 0.0f, argl5 = 0.0f, argr5 = 0.0f;
	float px = 0.0f, py = 0.0f, pz = 0.0f;
	float rx = 0.0f, ry = 0.0f, rz = 0.0f;
	Transform RotYtheta0;
	Transform hbTtb;
	Transform T051,T132,T1,T4,T5;
	ALValue Result = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	theta0.ToFloatArray(thetahip,false);

	RotYtheta0 = Transform::fromRotY(thetahip[0]);
	hbTtb = RotYtheta0*Atheta0;

	positiondlhh[0] = T.r1_c4;
	positiondlhh[1] = T.r2_c4;
	positiondlhh[2] = T.r3_c4;
	Pdlhh = Position3D(positiondlhh[0],positiondlhh[1],positiondlhh[2]);
	Pdlh0 = transformInverse(ALArm)*transformInverse(hbTtb)*Pdlhh;
	positiondlh0 = Pdlh0.toVector();
	normPobj0 = sqrt(pow(positiondlh0[0],2)+pow(positiondlh0[1],2)+pow(positiondlh0[2],2));

	//theta4 calclulation
	//Boundary: theta4 always in negative value, that is -1.5446 < theta4 < -0.0349 
	argloc = (pow(d3,2)+pow((d5+l3),2)-pow(normPobj0,2))/(2*d3*(d5+l3));
	
	if (argloc<-1)
	{
		argloc = -1;
	} else if(argloc>1)
	{
		argloc = 1;
	}
	
	//theta4 = -(pi-atan2f(sqrt(1-pow((argloc),2)),argloc));
	theta4 = -(pi-acosf(argloc));

	//First known matrix
	T051 = transformInverse(ALArm)*transformInverse(hbTtb)*T*transformInverse(ALArmEnd5)*transformInverse(RZLArm)*transformInverse(RXLArm);
	
	//theta2 calculation
	//Boundary: -0.3142 < theta2 < 1.3265
	argl2 = (-d5*T051.r2_c3+T051.r2_c4);
	argr21 = (-d5*T051.r1_c3+T051.r1_c4);
	argr22 = -(-d5*T051.r3_c3+T051.r3_c4);
	
	theta2 = atan2f(argl2,sqrt(pow(argr21,2)+pow(argr22,2)));

	//theta1 calculation
	//Boundary: -2.0857 < theta1 < 2.0857
	theta1 = atan2f(d5*T051.r3_c3-T051.r3_c4,-d5*T051.r1_c3+T051.r1_c4);

	//theta5 calculation
	//Boundary: -1.8238 < theta5 < 1.8238
	//Singular case: when theta4 is zero. Since minimum angle of theta4 is -0.0349, the singular case never occurs.
	argl5 = cosf(theta1)*cosf(theta2)*T051.r1_c2+sinf(theta2)*T051.r2_c2-sinf(theta1)*cosf(theta2)*T051.r3_c2;
	argr5 = cosf(theta1)*cosf(theta2)*T051.r1_c1+sinf(theta2)*T051.r2_c1-sinf(theta1)*cosf(theta2)*T051.r3_c1;

	if (theta4>=-0.0359f)
	{
		theta5 = 0.0f; //Handler for calculation error for theta4
	}
	else
	{
		theta5 = atan2f(-argl5/sin(theta4),argr5/sin(theta4));
		//theta5 = atan2f(-argl5,argr5);
	}

	T1 = RotXhalfpiMinus*(Transform::fromRotZ(theta1));
	T4 = RotXhalfpiPlus*(Transform::fromRotZ(theta4));
	T5 = RotXhalfpiMinus*(Transform::fromRotZ(theta5))*(Transform::fromPosition(0.0f,0.0f,d5));

	//Second known matrix
	T132 = transformInverse(T1)*T051*transformInverse(T5)*transformInverse(T4);

	//theta3 calculation
	if (theta4>=-0.0359f)
	{
		theta3 = 0.0f; //Handler for calculation error for theta4
	}
	else
	{
		theta3 = atan2f(T132.r2_c1,T132.r2_c2);
	}

	Result = ALValue::array(theta1,theta2,theta3,theta4,theta5);

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

ALValue RArmInverseHipBase(Transform T, ALValue theta0)
{
	vector<float> thetahip(1,0.0f);
	vector<float> positiondrhh(3,0.0f);
	vector<float> positiondrh0(3,0.0f);
	Position3D Pdrhh;
	Position3D Pdrh0;
	float normPobj0 = 0.0f;
	float theta1 = 0.0f;
	float theta2 = 0.0f;
	float theta3 = 0.0f;
	float theta4 = 0.0f;
	float theta5 = 0.0f;
	float argloc = 0.0f;
	float argl2 = 0.0f, argr21 = 0.0f, argr22 = 0.0f, argl5 = 0.0f, argr5 = 0.0f;
	float px = 0.0f, py = 0.0f, pz = 0.0f;
	float rx = 0.0f, ry = 0.0f, rz = 0.0f;
	Transform RotYtheta0;
	Transform hbTtb;
	Transform T051,T132,T1,T4,T5;
	ALValue Result = ALValue::array(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

	theta0.ToFloatArray(thetahip,false);

	RotYtheta0 = Transform::fromRotY(thetahip[0]);
	hbTtb = RotYtheta0*Atheta0;

	positiondrhh[0] = T.r1_c4;
	positiondrhh[1] = T.r2_c4;
	positiondrhh[2] = T.r3_c4;
	Pdrhh = Position3D(positiondrhh[0],positiondrhh[1],positiondrhh[2]);
	Pdrh0 = transformInverse(ARArm)*transformInverse(hbTtb)*Pdrhh;
	positiondrh0 = Pdrh0.toVector();
	normPobj0 = sqrt(pow(positiondrh0[0],2)+pow(positiondrh0[1],2)+pow(positiondrh0[2],2));

	//theta4 calclulation
	//Boundary: theta4 always in positive value, that is 0.0349 < theta4 < 1.5446 
	argloc = (pow(d3,2)+pow((d5+l3),2)-pow(normPobj0,2))/(2*d3*(d5+l3));
	
	if (argloc<-1)
	{
		argloc = -1;
	} else if(argloc>1)
	{
		argloc = 1;
	}
	
	//theta4 = (pi-atan2f(sqrt(1-pow((argloc),2)),argloc));
	theta4 = (pi-acosf(argloc));

	//First known matrix
	T051 = transformInverse(ARArm)*transformInverse(hbTtb)*T*transformInverse(ARArmEnd5)*transformInverse(RZRArm)*transformInverse(RXRArm);
	
	//theta2 calclulation
	//Boundary: -1.3265 < theta2 < 0.3142
	argl2 = (-d5*T051.r2_c3+T051.r2_c4);
	argr21 = (-d5*T051.r1_c3+T051.r1_c4);
	argr22 = (-d5*T051.r3_c3+T051.r3_c4);
	
	theta2 = atan2f(argl2,sqrt(pow(argr21,2)+pow(argr22,2)));

	//theta1 calculation
	//Boundary: -2.0857 < theta1 < 2.0857
	theta1 = atan2f(-d5*T051.r3_c3+T051.r3_c4,-d5*T051.r1_c3+T051.r1_c4);

	//theta5 calculation
	//Boundary: -1.8238 < theta5 < 1.8238
	//Singular case: when theta4 is zero. Since minimum angle of theta4 is -0.0349, the singular case never occurs.
	argl5 = cosf(theta1)*cosf(theta2)*T051.r1_c2+sinf(theta2)*T051.r2_c2+sinf(theta1)*cosf(theta2)*T051.r3_c2;
	argr5 = -cosf(theta1)*cosf(theta2)*T051.r1_c1-sinf(theta2)*T051.r2_c1-sinf(theta1)*cosf(theta2)*T051.r3_c1;

	if (theta4<=0.0359f)
	{
		theta5 = 0.0f; //Handler for calculation error for theta4
	}
	else
	{
		theta5 = atan2f(argl5/sin(theta4),argr5/sin(theta4));
		//theta5 = atan2f(argl5,argr5);
	}

	T1 = RotXhalfpiPlus*(Transform::fromRotZ(theta1));
	T4 = RotXhalfpiMinus*(Transform::fromRotZ(theta4));
	T5 = RotXhalfpiPlus*(Transform::fromRotZ(theta5))*(Transform::fromPosition(0.0f,0.0f,d5));

	//Second known matrix
	T132 = transformInverse(T1)*T051*transformInverse(T5)*transformInverse(T4);

	//theta3 calculation
	if (theta4<=0.0359f)
	{
		theta3 = 0.0f; //Handler for calculation error for theta4
	}
	else
	{
		theta3 = atan2f(T132.r2_c1,T132.r2_c2);
	}

	Result = ALValue::array(-theta1,theta2,theta3,theta4,theta5);

	return (Result);
}

vector<Position6D> TrajectoryGenerator(Position6D POi, Position6D POf, float tmax)
{
	Transform Ti,Tf;
	Rotation Ri,Rf,Rif;
	ALValue ro;
	vector<float> POiv(6,0.0f); 
	vector<float> POfv(6,0.0f); 
	unsigned int frag = 5;
	unsigned int i;
	float dxy = 0.0f, dz = 0.0f;
	float sint, cost;
	float subxy, sumxy, subz, sumz;
	float offsetxy, offsetz;
	float totalarclength;
	float kappai, kappaf;
	float ti, tf;
	float dtif;
	float a2, a3;
	float sumt;
	vector<float> dpxy(6,0.0f);
	vector<float> pxy(6,0.0f);
	vector<float> dpz(6,0.0f);
	vector<float> pzz(6,0.0f);
	vector<float> xy(8,0.0f);
	vector<float> z(8,0.0f);
	vector<float> px(8,0.0f);
	vector<float> py(8,0.0f);
	vector<float> pz(8,0.0f);
	vector<float> pxn(9,0.0f);
	vector<float> pyn(9,0.0f);
	vector<float> pzn(9,0.0f);
	vector<float> arclength(8,0.0f);
	vector<float> ri(9,0.0f);
	vector<float> rf(9,0.0f);
	vector<float> r(3,0.0f);
	vector<float> dt(8,0.0f);
	vector<float> t(9,0.0f);
	vector<float> kappa(9,0.0f);
	vector<Rotation3D> O(9,Rotation3D());
	vector<Position6D> Result(9,Position6D());

	POiv = POi.toVector();
	POfv = POf.toVector();
	Ti = PosOrient2Matrix(POi);
	Tf = PosOrient2Matrix(POf);

	//X-Y plane calculation
	dxy = sqrt(pow(POf.x-POi.x,2)+pow(POf.y-POi.y,2));
	sint = (POf.x-POi.x)/dxy;
	cost = (POf.y-POi.y)/dxy;
	subxy = dxy;
	sumxy = 0.0f;

	for (i=0; i<=frag; i++)
	{
		if (i>0)
		{
			subxy -= dpxy[i-1];
			sumxy += dpxy[i-1];
		}
		dpxy[i] = subxy/3;
		pxy[i] = sumxy+dpxy[i];
	}

	offsetxy = (dxy-pxy[frag])/2;

	for (i=0; i<=frag; i++)
	{
		pxy[i] = offsetxy+pxy[i];
	}

	for (i=0; i<=frag+2; i++)
	{
		if (i==0)
		{
			xy[i] = 0.0f;
		} 
		else if (i==(frag+2))
		{
			xy[i] = dxy;
		}
		else
		{
			xy[i] = pxy[i-1];
		}
	}

	for (i=0; i<=frag+2; i++)
	{
		if (i==0)
		{
			px[i] = POi.x;
			py[i] = POi.y;
		} 
		else if (i==(frag+2))
		{
			px[i] = POf.x;
			py[i] = POf.y;
		}
		else
		{
			px[i] = xy[i]*sint+px[0];
			py[i] = xy[i]*cost+py[0];
		}
	}

	//Z line calculation
	dz = POi.z-POf.z;
	subz = dz;
	sumz = 0.0f;

	for (i=0; i<=frag; i++)
	{
		if (i>0)
		{
			subz -= dpz[i-1];
			sumz += dpz[i-1];
		}
		dpz[i] = subz/3;
		pzz[i] = sumz+dpz[i];
	}

	offsetz = (dz-pzz[frag])/2;

	for (i=0; i<=frag; i++)
	{
		pzz[i] = offsetz+pzz[i];
	}

	for (i=0; i<=frag+2; i++)
	{
		if (i==0)
		{
			z[i] = 0.0f;
		} 
		else if (i==(frag+2))
		{
			z[i] = dz;
		}
		else
		{
			z[i] = pzz[i-1];
		}
	}

	for (i=0; i<=frag+2; i++)
	{
		if (i==0)
		{
			pz[i] = POi.z;
		} 
		else if (i==(frag+2))
		{
			pz[i] = POf.z;
		}
		else
		{
			pz[i] = -z[i]+pz[0];
		}
	}

	for (i=0; i<=frag+3; i++)
	{
		if (i==(frag+2))
		{
			pxn[i] = px[frag+2];
			pyn[i] = py[frag+2];
			pzn[i] = pz[frag+1]-((pz[frag+1]-pz[frag+2])*2)/3;
		} 
		else if (i==(frag+3))
		{
			pxn[i] = px[frag+2];
			pyn[i] = py[frag+2];
			pzn[i] = pz[frag+2];
		}
		else
		{
			pxn[i] = px[i];
			pyn[i] = py[i];
			pzn[i] = pz[i];
		}
	}

	//Arc length calculation
	totalarclength = 0.0f;

	for (i=0; i<=frag+2; i++)
	{
		arclength[i] = sqrt(pow(pxn[i+1]-pxn[i],2)+pow(pyn[i+1]-pyn[i],2)+pow(pzn[i+1]-pzn[i],2));
		totalarclength += arclength[i];
	}

	//Orientation calculation
	ri[0] = Ti.r1_c1;
	ri[1] = Ti.r1_c2;
	ri[2] = Ti.r1_c3;
	ri[3] = Ti.r2_c1;
	ri[4] = Ti.r2_c2;
	ri[5] = Ti.r2_c3;
	ri[6] = Ti.r3_c1;
	ri[7] = Ti.r3_c2;
	ri[8] = Ti.r3_c3;
	Ri = Rotation(ri);
	rf[0] = Tf.r1_c1;
	rf[1] = Tf.r1_c2;
	rf[2] = Tf.r1_c3;
	rf[3] = Tf.r2_c1;
	rf[4] = Tf.r2_c2;
	rf[5] = Tf.r2_c3;
	rf[6] = Tf.r3_c1;
	rf[7] = Tf.r3_c2;
	rf[8] = Tf.r3_c3;
	Rf = Rotation(rf);
	Rif = Ri.transpose()*Rf;

	kappaf = acosf((Rif.r1_c1+Rif.r2_c2+Rif.r3_c3-1)/2);
	r[0] = (Rif.r3_c2-Rif.r2_c3)/(2*sinf(kappaf));
	r[1] = (Rif.r1_c3-Rif.r3_c1)/(2*sinf(kappaf));
	r[2] = (Rif.r2_c1-Rif.r1_c2)/(2*sinf(kappaf));
	ro = ALValue::array(r[0],r[1],r[2]);

	ti = 0.0f;
	tf = tmax;
	dtif = tf-ti;
	a2 = (3*kappaf)/pow(tf,2);
	a3 = (-2*kappaf)/pow(tf,3);
	sumt = 0.0f;

	for (i=0; i<=frag+3; i++)
	{
		if (i==0)
		{
			t[i] = ti;
		} 
		else if (i==(frag+3))
		{
			t[i] = tf;
		}
		else
		{
			sumt += dt[i-1];
			dt[i] = (arclength[i-1]/totalarclength)*dtif;
			t[i] = sumt+dt[i];
		}
	}

	kappai = 0.0f;

	for (i=0; i<=frag+3; i++)
	{
		if (i==0)
		{
			kappa[i] = kappai;
		} 
		else if (i==(frag+3))
		{
			kappa[i] = kappaf;
		}
		else
		{
			kappa[i] = a2*pow(t[i],2)+a3*pow(t[i],3);
		}
	}

	for (i=0; i<=frag+3; i++)
	{
		if (i==0)
		{
			O[i] = Matrix2Orient(Ri);
		} 
		else if (i==(frag+3))
		{
			O[i] = Matrix2Orient(Rf);
		}
		else
		{
			O[i] = Matrix2Orient(Ri*RotArbitrary(ro,kappa[i]));
		}
	}

	for (i=0; i<=frag+3; i++)
	{
		if (i==0)
		{
			Result[i] = POi;
		} 
		else if (i==(frag+3))
		{
			Result[i] = POf;
		}
		else
		{
			Result[i] = Position6D(pxn[i],pyn[i],pzn[i],O[i].wx,O[i].wy,O[i].wz);
		}
	}

	return(Result);
}

vector<Position6D> TrajectoryGeneratorReverse(Position6D POi, Position6D POf, float tmax)
{
	unsigned int i;
	vector<Position6D> temp(9,Position6D());
	vector<Position6D> Result(9,Position6D());

	temp = TrajectoryGenerator(POf, POi, tmax);

	for (i=0; i<=8; i++)
	{
			Result[i] = temp[8-i];
	}

	return(Result);
}

ALValue TimeGenerator(vector<Position6D> PO, float tmax)
{
	unsigned int i;
	unsigned int frag = 5;
	float ti, tf;
	float dtif;
	float sumt;
	float totalarclength;
	vector<float> dt(8,0.0f);
	vector<float> t(9,0.0f);
	vector<float> arclength(8,0.0f);
	ALValue Result = ALValue::array(0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f);



	totalarclength = 0.0f;

	for (i=0; i<=frag+2; i++)
	{
		arclength[i] = sqrt(pow(PO[i+1].x-PO[i].x,2)+pow(PO[i+1].y-PO[i].y,2)+pow(PO[i+1].z-PO[i].z,2));
		totalarclength += arclength[i];
	}

	ti = 0.0f;
	tf = tmax;
	dtif = tf-ti;
	sumt = 0.0f;

	for (i=0; i<=frag+3; i++)
	{
		if (i==0)
		{
			t[i] = ti;
		} 
		else if (i==(frag+3))
		{
			t[i] = tf;
		}
		else
		{
			sumt += dt[i-1];
			dt[i] = (arclength[i-1]/totalarclength)*dtif;
			t[i] = sumt+dt[i];
		}
	}

	Result = ALValue::array(t[0],t[1],t[2],t[3],t[4],t[5],t[6],t[7],t[8]);

	return(Result);
}

ALValue TimeGeneratorReverse(vector<Position6D> PO, float tmax)
{
	unsigned int i;
	float ti = 0.0f;
	float tf;
	vector<float> time(9,0.0f);
	vector<Position6D> temp(9,Position6D());
	ALValue tim = ALValue::array(0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f);
	ALValue Result = ALValue::array(0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f);

	tf = tmax;

	for (i=0; i<=8; i++)
	{
			temp[i] = PO[8-i];
	}

	tim = TimeGenerator(temp, tmax);
	tim.ToFloatArray(time,false);

	for (i=0; i<=8; i++)
	{
		if (i==0)
		{
			Result[i] = ti;
		} 
		else if (i==8)
		{
			Result[i] = tf;
		}
		else
		{
			Result[i] = tf-time[8-i];
		}
	}

	return(Result);
}