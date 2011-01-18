#include "Ib4aProjection.h"
#include "b4aLevelVBSubcluster.h"
#include "b4aLevelVB.h"
#include "b4aPlane.h"

using namespace Bsp4Airplay;
void Cb4aFlashlightProjectionVertex::Lerp(Cb4aFlashlightProjectionVertex*dst, const Cb4aFlashlightProjectionVertex& v0, int32 d0, const Cb4aFlashlightProjectionVertex& v1, int32 d1)
{
	int32 total = d0-d1;
	dst->pos.x = (int16)( ((int32)v1.pos.x*d0-(int32)v0.pos.x*d1)/total );
	dst->pos.y = (int16)( ((int32)v1.pos.y*d0-(int32)v0.pos.y*d1)/total );
	dst->pos.z = (int16)( ((int32)v1.pos.z*d0-(int32)v0.pos.z*d1)/total );
	dst->uv0.x = (int16)( ((int32)v1.uv0.x*d0-(int32)v0.uv0.x*d1)/total );
	dst->uv0.y = (int16)( ((int32)v1.uv0.y*d0-(int32)v0.uv0.y*d1)/total );
	dst->uv1.x = ( ((int32)v1.uv1.x*d0-(int32)v0.uv1.x*d1)/total );
	dst->uv1.y = ( ((int32)v1.uv1.y*d0-(int32)v0.uv1.y*d1)/total );
	dst->uv1.z = ( ((int32)v1.uv1.z*d0-(int32)v0.uv1.z*d1)/total );
}
Cb4aFlashlightProjection::Cb4aFlashlightProjection()
{
	near = 8;
}
void Cb4aFlashlightProjection::Add(Cb4aFlashlightProjectionFace& face,int plane)
{
	start:
	if (plane == 6)
	{
		CIwSVec2 proj;
		for (int i=0; i<3;++i)
		{
			proj.x = (int16)(IW_GEOM_ONE/2+((face.vertices[i].uv1.x*whz.z/(face.vertices[i].uv1.z))<<(IW_GEOM_POINT-0))/whz.x);
			proj.y = (int16)(IW_GEOM_ONE/2+((face.vertices[i].uv1.y*whz.z/(face.vertices[i].uv1.z))<<(IW_GEOM_POINT-0))/whz.y);
			positions.push_back(face.vertices[i].pos);
			uv0.push_back(proj);
			indices.push_back((uint16)indices.size());
		}
		return;
	}
	int32 dist[3];
	for (int i=0;i<3;++i)
		dist[i] = b4aPlaneDist(face.vertices[i].uv1, frustum[plane]);
	//cull
	if (dist[0] <= 0 && dist[1] <= 0 && dist[2] <= 0)
		return;
	//continue
	if (dist[0] >= 0 && dist[1] >= 0 && dist[2] >= 0)
	{
		++plane;
		goto start;
	}
	//slice
	int ptrs[3];
	if (dist[0] < 0 && dist[1] >= 0)
	{
		ptrs[0] = 0;
		ptrs[1] = 1;
		ptrs[2] = 2;
	} 
	else if (dist[1] < 0 && dist[2] >= 0)
	{
		ptrs[0] = 1;
		ptrs[1] = 2;
		ptrs[2] = 0;
	}
	else if (dist[2] < 0 && dist[0] >= 0)
	{
		ptrs[0] = 2;
		ptrs[1] = 0;
		ptrs[2] = 1;
	}
	else
	{
		IwAssertMsg(BSP,false,("It should not happen"));
	}
	if (dist[ptrs[1]] == 0)
	{
		Cb4aFlashlightProjectionVertex v;
		Cb4aFlashlightProjectionVertex::Lerp(&v,face.vertices[ptrs[2]], dist[ptrs[2]],face.vertices[ptrs[0]], dist[ptrs[0]]);
		face.vertices[ptrs[0]] = v;
		++plane;
		goto start;
	}
	Cb4aFlashlightProjectionVertex v01;
	Cb4aFlashlightProjectionVertex::Lerp(&v01,face.vertices[ptrs[0]], dist[ptrs[0]],face.vertices[ptrs[1]], dist[ptrs[1]]);
	if (dist[ptrs[2]] > 0)
	{
		Cb4aFlashlightProjectionVertex v20;
		Cb4aFlashlightProjectionVertex::Lerp(&v20,face.vertices[ptrs[2]], dist[ptrs[2]],face.vertices[ptrs[0]], dist[ptrs[0]]);
		
		Cb4aFlashlightProjectionFace face1;
		face1.vertices[0] = v20;
		face1.vertices[1] = v01;
		face1.vertices[2] = face.vertices[ptrs[1]];
		Add(face1,plane+1);
		face1.vertices[0] = v20;
		face1.vertices[1] = face.vertices[ptrs[1]];
		face1.vertices[2] = face.vertices[ptrs[2]];
		Add(face1,plane+1);
		return;
	}
	{
		Cb4aFlashlightProjectionVertex v12;
		Cb4aFlashlightProjectionVertex::Lerp(&v12,face.vertices[ptrs[1]], dist[ptrs[1]],face.vertices[ptrs[2]], dist[ptrs[2]]);
		
		Cb4aFlashlightProjectionFace face1;
		face1.vertices[0] = v01;
		face1.vertices[1] = face.vertices[ptrs[1]];
		face1.vertices[2] = v12;
		Add(face1,plane+1);
	}
}
void Cb4aFlashlightProjection::Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry)
{
	if (b4aIsBBoxIntersect(projectionBox, geometry->GetBBox()))
	{
		uint32 size = geometry->indices.size();
		Cb4aFlashlightProjectionFace face;
		for (uint i=0; i<size; i+=3)
		{
			face.vertices[0].pos = buffer->GetPosition(geometry->indices[i]);
			face.vertices[1].pos = buffer->GetPosition(geometry->indices[i+1]);
			face.vertices[2].pos = buffer->GetPosition(geometry->indices[i+2]);
			face.vertices[0].uv1 = matrix.TransposeTransformVec(CIwVec3(face.vertices[0].pos));
			face.vertices[1].uv1 = matrix.TransposeTransformVec(CIwVec3(face.vertices[1].pos));
			face.vertices[2].uv1 = matrix.TransposeTransformVec(CIwVec3(face.vertices[2].pos));

			if (face.vertices[0].uv1.z < near*1 && face.vertices[1].uv1.z < near*1 && face.vertices[2].uv1.z < near*1)
				continue;
			if (face.vertices[0].uv1.z > whz.z*1 && face.vertices[1].uv1.z > whz.z*1 && face.vertices[2].uv1.z > whz.z*1)
				continue;

			face.vertices[0].n = matrix.TransposeRotateVec(buffer->GetNormal(geometry->indices[i])).z;
			face.vertices[1].n = matrix.TransposeRotateVec(buffer->GetNormal(geometry->indices[i+1])).z;
			face.vertices[2].n = matrix.TransposeRotateVec(buffer->GetNormal(geometry->indices[i+2])).z;

			if (face.vertices[0].n > 0 && face.vertices[1].n > 0 && face.vertices[2].n > 0)
				continue;

			face.vertices[0].uv0 = buffer->GetUV0(geometry->indices[i]);
			face.vertices[1].uv0 = buffer->GetUV0(geometry->indices[i]);
			face.vertices[2].uv0 = buffer->GetUV0(geometry->indices[i]);
			Add(face,0);


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
void Cb4aFlashlightProjection::Clear()
{
	positions.clear();
	uv0.clear();
	indices.clear();
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

	frustum[0] = Cb4aPlane(CIwVec3(0,0,IW_GEOM_ONE),near);
	frustum[1] = Cb4aPlane(CIwVec3(0,0,-IW_GEOM_ONE),-whz.z);
	CIwVec3 n;
	n.x = -whz.z;
	n.y = 0;
	n.z = whz.x;
	n.Normalise();
	frustum[2] = Cb4aPlane(n,0);
	n.x = whz.z;
	n.y = 0;
	n.z = whz.x;
	n.Normalise();
	frustum[3] = Cb4aPlane(n,0);
	n.x = 0;
	n.y = -whz.z;
	n.z = whz.y;
	n.Normalise();
	frustum[4] = Cb4aPlane(n,0);
	n.x = 0;
	n.y = whz.z;
	n.z = whz.y;
	n.Normalise();
	frustum[5] = Cb4aPlane(n,0);

	//CIwVec3 d (whz.x/4,whz.y/4,(near+whz.x)/2);
	//for(int i=0; i<6; ++i)
	//{
	//	int32 dist = b4aPlaneDist(d,frustum[i]);
	//	IwAssert(BSP,(dist>0));
	//}

	Clear();
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

