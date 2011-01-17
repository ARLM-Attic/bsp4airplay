#include "Ib4aProjection.h"
#include "b4aLevelVBSubcluster.h"
#include "b4aLevelVB.h"
#include "b4aPlane.h"

using namespace Bsp4Airplay;

Cb4aFlashlightProjection::Cb4aFlashlightProjection()
{
}

void Cb4aFlashlightProjection::Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry)
{
	if (b4aIsBBoxIntersect(projectionBox, geometry->GetBBox()))
	{
		uint32 size = geometry->indices.size();
		for (uint i=0; i<size; i+=3)
		{
			const CIwSVec3 & pos0 = buffer->GetPosition(geometry->indices[i]);
			const CIwSVec3 & pos1 = buffer->GetPosition(geometry->indices[i+1]);
			const CIwSVec3 & pos2 = buffer->GetPosition(geometry->indices[i+2]);
			CIwVec3 uvz0 = matrix.TransposeTransformVec(CIwVec3(pos0));
			CIwVec3 uvz1 = matrix.TransposeTransformVec(CIwVec3(pos1));
			CIwVec3 uvz2 = matrix.TransposeTransformVec(CIwVec3(pos2));
			//TODO: it should cut clipped triangles!
			if (uvz0.z < 8*1 || uvz1.z < 8*1 || uvz2.z < 8*1)
				continue;
			if (uvz0.z > whz.z*1 && uvz1.z > whz.z*1 && uvz2.z > whz.z*1)
				continue;
			uvz0.x = IW_GEOM_ONE/2+((uvz0.x)<<IW_GEOM_POINT)/(uvz0.z);
			uvz0.y = IW_GEOM_ONE/2+((uvz0.y)<<IW_GEOM_POINT)/(uvz0.z);
			uvz1.x = IW_GEOM_ONE/2+((uvz1.x)<<IW_GEOM_POINT)/(uvz1.z);
			uvz1.y = IW_GEOM_ONE/2+((uvz1.y)<<IW_GEOM_POINT)/(uvz1.z);
			uvz2.x = IW_GEOM_ONE/2+((uvz2.x)<<IW_GEOM_POINT)/(uvz2.z);
			uvz2.y = IW_GEOM_ONE/2+((uvz2.y)<<IW_GEOM_POINT)/(uvz2.z);
			/*uvz0.x = IW_GEOM_ONE/2+((uvz0.x*whz.z/(uvz0.z))<<IW_GEOM_POINT)/whz.x;
			uvz0.y = IW_GEOM_ONE/2+((uvz0.y*whz.z/(uvz0.z))<<IW_GEOM_POINT)/whz.y;
			uvz1.x = IW_GEOM_ONE/2+((uvz1.x*whz.z/(uvz1.z))<<IW_GEOM_POINT)/whz.x;
			uvz1.y = IW_GEOM_ONE/2+((uvz1.y*whz.z/(uvz1.z))<<IW_GEOM_POINT)/whz.y;
			uvz2.x = IW_GEOM_ONE/2+((uvz2.x*whz.z/(uvz2.z))<<IW_GEOM_POINT)/whz.x;
			uvz2.y = IW_GEOM_ONE/2+((uvz2.y*whz.z/(uvz2.z))<<IW_GEOM_POINT)/whz.y;*/
			//uvz0.x = (int32)( ((float)IW_GEOM_ONE)*uvz0.x*whz.z/(whz.x*uvz0.z) + IW_GEOM_ONE/2);
			//uvz0.y = (int32)( ((float)IW_GEOM_ONE)*uvz0.y*whz.z/(whz.y*uvz0.z) + IW_GEOM_ONE/2);
			//uvz1.x = (int32)( ((float)IW_GEOM_ONE)*uvz1.x*whz.z/(whz.x*uvz1.z) + IW_GEOM_ONE/2);
			//uvz1.y = (int32)( ((float)IW_GEOM_ONE)*uvz1.y*whz.z/(whz.y*uvz1.z) + IW_GEOM_ONE/2);
			//uvz2.x = (int32)( ((float)IW_GEOM_ONE)*uvz2.x*whz.z/(whz.x*uvz2.z) + IW_GEOM_ONE/2);
			//uvz2.y = (int32)( ((float)IW_GEOM_ONE)*uvz2.y*whz.z/(whz.y*uvz2.z) + IW_GEOM_ONE/2);

			if (uvz0.x < 0 && uvz1.x < 0 && uvz2.x<0) continue;
			if (uvz0.y < 0 && uvz1.y < 0 && uvz2.y<0) continue;
			if (uvz0.x > IW_GEOM_ONE && uvz1.x > IW_GEOM_ONE && uvz2.x > IW_GEOM_ONE) continue;
			if (uvz0.y > IW_GEOM_ONE && uvz1.y > IW_GEOM_ONE && uvz2.y > IW_GEOM_ONE) continue;

			//TODO: clip triangles
			if (uvz0.x < -32767 || uvz1.x < -32767 || uvz2.x<-32767) continue;
			if (uvz0.y < -32767 || uvz1.y < -32767 || uvz2.y<-32767) continue;
			if (uvz0.x > 32767 || uvz1.x > 32767 || uvz2.x > 32767) continue;
			if (uvz0.y > 32767 || uvz1.y > 32767 || uvz2.y > 32767) continue;

			positions.push_back(pos0);
			positions.push_back(pos1);
			positions.push_back(pos2);
			//uv0.push_back(buffer->GetUV0(geometry->indices[i]));
			//uv0.push_back(buffer->GetUV0(geometry->indices[i+1]));
			//uv0.push_back(buffer->GetUV0(geometry->indices[i+2]));
			uv0.push_back(CIwSVec2((int16)uvz0.x,(int16)uvz0.y));
			uv0.push_back(CIwSVec2((int16)uvz1.x,(int16)uvz1.y));
			uv0.push_back(CIwSVec2((int16)uvz2.x,(int16)uvz2.y));
			indices.push_back((uint16)indices.size());
			indices.push_back((uint16)indices.size());
			indices.push_back((uint16)indices.size());
		}
	}
	else
	{
		bool ff = true;
	}
}
void Cb4aFlashlightProjection::Flush()
{
	if (indices.empty())
		return;
	CIwMaterial* mat = IW_GX_ALLOC_MATERIAL();
	mat->SetTexture(texure);
	mat->SetAlphaMode(CIwMaterial::ALPHA_ADD);
	//mat->SetBlendMode(CIwMaterial::BLEND_ADD);
	mat->SetDepthWriteMode(CIwMaterial::DEPTH_WRITE_DISABLED);
	mat->SetZDepthOfs(-4);
	mat->SetZDepthOfsHW(-4);
	IwGxSetMaterial(mat);

	IwGxSetVertStream(&positions[0], indices.size());
	IwGxSetUVStream(&uv0[0], 0);
	IwGxDrawPrims(IW_GX_TRI_LIST,&indices[0],indices.size());
}
void Cb4aFlashlightProjection::Prepare(CIwTexture* tex, const CIwMat& mat, const CIwVec3 & _whz)
{
	texure = tex;
	matrix = mat;
	whz = _whz;

	projectionBox.m_Min = matrix.t;
	projectionBox.m_Max = matrix.t;
	CIwVec3 v;
	v = matrix.TransformVec(whz);
	projectionBox.BoundVec(&v);
	v = matrix.TransformVec(CIwVec3(-whz.x,-whz.y,whz.z));
	projectionBox.BoundVec(&v);
	v = matrix.TransformVec(CIwVec3(-whz.x,whz.y,whz.z));
	projectionBox.BoundVec(&v);
	v = matrix.TransformVec(CIwVec3(whz.x,-whz.y,whz.z));
	projectionBox.BoundVec(&v);

	positions.clear();
	uv0.clear();
	indices.clear();
}

Cb4aSkyProjection::Cb4aSkyProjection()
{
}
void Cb4aSkyProjection::Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry)
{

}
void Cb4aSkyProjection::Flush()
{
}

