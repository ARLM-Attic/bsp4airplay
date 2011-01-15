#pragma once

#include "b4aLevel.h"

namespace Bsp4Airplay
{
	void Bsp4AirpayInit();
	void Bsp4AirpayTerminate();

	//inline int32 b4aDot(const CIwSVec3& a,const CIwSVec3& b)
	//{
	//	return (int32)a.x*(int32)b.x+(int32)a.y*(int32)b.y+(int32)a.z*(int32)b.z;
	//}
	//inline int32 b4aPlaneDist(const CIwSVec3& a,const CIwPlane& b)
	//{
	//	return b4aDot(b.v,a)-b.k;
	//}
	//inline CIwSVec3 b4aLerp(const CIwSVec3& a,const CIwSVec3& b,int32 adist, int32 bdist)
	//{
	//	int32 total = adist-bdist;
	//	if (total == 0)
	//		return a;
	//	while (total < -65536 || total > 65536)
	//	{
	//		total >>= 1;
	//		adist >>= 1;
	//		bdist >>= 1;
	//	}
	//	return CIwSVec3(
	//		(int16)((adist*b.x - bdist*a.x)/total),
	//		(int16)((adist*b.y - bdist*a.y)/total),
	//		(int16)((adist*b.z - bdist*a.z)/total));
	//}
	//const int32 b4aCollisionEpsilon=4096;

	inline int32 b4aDot(const CIwVec3& a,const CIwVec3& v)
	{
		return (int32)((
        (int64)a.x * v.x +
        (int64)a.y * v.y +
        (int64)a.z * v.z +
        0)>>12);
	}
	inline int32 b4aPlaneDist(const CIwVec3& a,const CIwPlane& b)
	{
		//return (int32)(a.x>>12)*(int32)b.v.x+(int32)(a.y>>12)*(int32)b.v.y+(int32)(a.z>>12)*(int32)b.v.z - b.k;
		int32 d = b4aDot(CIwVec3(b.v),a);
		return d-b.k;
	}
	inline CIwVec3 b4aLerp(const CIwVec3& a,const CIwVec3& b,int32 adist, int32 bdist)
	{
		int32 total = adist-bdist;
		if (total == 0)
			return a;
		/*while (total < -(1<<7) || total > (1<<7))
		{
			total >>= 1;
			adist >>= 1;
			bdist >>= 1;
		}*/
		return CIwVec3(
			(int32)(((int64)adist*b.x - (int64)bdist*a.x)/total),
			(int32)(((int64)adist*b.y - (int64)bdist*a.y)/total),
			(int32)(((int64)adist*b.z - (int64)bdist*a.z)/total));
	}
	const int32 b4aCollisionEpsilon=4096;

}