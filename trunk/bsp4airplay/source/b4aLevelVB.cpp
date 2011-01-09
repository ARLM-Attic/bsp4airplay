#include <IwTextParserITX.h>
#include <algorithm>
#include <b4aLevelVB.h>
#include <b4aLevel.h>

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseLevelVertexBuffer g_parseLevelVertexBuffer;
#endif
	bool SortByMaterial(Cb4aLevelVBSubcluster*a,Cb4aLevelVBSubcluster*b)
	{
		return a->GetMaterial() < b->GetMaterial();
	}
}


Cb4aLevelVertexBuffer::Cb4aLevelVertexBuffer()
{
}

Cb4aLevelVertexBuffer::~Cb4aLevelVertexBuffer()
{
	if (positionsStream.IsSet())
	{
		/*positionsStream.Free();
		normalsStream.Free();
		uv0Stream.Free();
		uv1Stream.Free();
		coloursStream.Free();*/
	}
}
void Cb4aLevelVertexBuffer::Serialise()
{
	if (positionsStream.IsSet())
		IwAssertMsg(BSP,false,("Can't serialise uploaded stream"));

	positions.SerialiseHeader();
	for (uint32 i=0; i<positions.size(); ++i)
		positions[i].Serialise();
	normals.SerialiseHeader();
	for (uint32 i=0; i<normals.size(); ++i)
		normals[i].Serialise();
	uv0s.SerialiseHeader();
	for (uint32 i=0; i<uv0s.size(); ++i)
		uv0s[i].Serialise();
	uv1s.SerialiseHeader();
	for (uint32 i=0; i<uv1s.size(); ++i)
		uv1s[i].Serialise();
	colours.SerialiseHeader();
	for (uint32 i=0; i<colours.size(); ++i)
		colours[i].Serialise();
}
void Cb4aLevelVertexBuffer::SetCapacity(uint32 n)
{
	positions.set_capacity(n);
	normals.set_capacity(n);
	uv0s.set_capacity(n);
	uv1s.set_capacity(n);
	colours.set_capacity(n);
}
void Cb4aLevelVertexBuffer::ScheduleCluster(Cb4aLevelVBSubcluster* cluster)
{
	renderQueue.push_back(cluster);
}
void Cb4aLevelVertexBuffer::FlushQueueBlock(Cb4aLevel* l,uint32 from, uint32 end)
{
	uint32 totalIndices = 0;
	for (uint32 i=from; i<end; ++i)
	{
		totalIndices +=renderQueue[i]->GetIndices().size();
	}
	if (totalIndices == 0)
		return;
	l->BindMaterial(renderQueue[from]->GetMaterial());
	uint16* ptr = IW_GX_ALLOC(uint16,totalIndices);
	uint16* cur = ptr;
	for (uint32 i=from; i<end; ++i)
	{
		const CIwArray<uint16>& indices = renderQueue[i]->GetIndices();
		memcpy(cur,&indices[0],indices.size()*2);
		cur += indices.size();
	}
	IwGxDrawPrims(IW_GX_TRI_LIST,ptr,totalIndices);
}
void Cb4aLevelVertexBuffer::Flush(Cb4aLevel* l)
{
	if (renderQueue.empty())
		return;

	std::sort(renderQueue.begin(),renderQueue.end(), SortByMaterial);
	PreRender();

	uint32 totalItems = renderQueue.size();

	// Debug mode!!
	//if (totalItems > 100)		totalItems= 100;

	uint32 start = 0;
	while (start< totalItems)
	{
		uint32 end = start;
		while (end < totalItems && renderQueue[end]->GetMaterial() == renderQueue[start]->GetMaterial()) ++end;

		FlushQueueBlock(l,start,end);
		start = end;
	}


	PostRender();
	renderQueue.clear();
}

void Cb4aLevelVertexBuffer::PreRender()
{
	if (!positionsStream.IsSet())
	{
		if (positions.empty())
			return;
		positionsStream.Set(CIwGxStream::SVEC3, &positions.front(), positions.size(), 0);
		positionsStream.Upload(true, false);
		normalsStream.Set(CIwGxStream::SVEC3, &normals.front(), normals.size(), 0);
		normalsStream.Upload(true, false);
		uv0Stream.Set(CIwGxStream::SVEC2, &uv0s.front(), uv0s.size(), 0);
		uv0Stream.Upload(true, false);
		uv1Stream.Set(CIwGxStream::SVEC2, &uv1s.front(), uv1s.size(), 0);
		uv1Stream.Upload(true, false);
		coloursStream.Set(CIwGxStream::COLOUR, &colours.front(), colours.size(), 0);
		coloursStream.Upload(true,false);
		/*positions.clear_optimised();
		normals.clear_optimised();
		uv0s.clear_optimised();
		uv1s.clear_optimised();
		colours.clear_optimised();*/
	}
	IwGxSetVertStreamWorldSpace(positionsStream);
	IwGxSetNormStream(normalsStream);
	IwGxSetUVStream(uv0Stream, 0);
	IwGxSetUVStream(uv1Stream, 1);
	//IwGxSetColStream(coloursStream);

	//IwGxSetVertStream(&positions.front(), positions.size());
	//IwGxSetNormStream(&normals.front(), normals.size());
	//IwGxSetUVStream(&uv0s.front(), 0);
	//IwGxSetUVStream(&uv1s.front(), 1);
	//IwGxSetColStream(&colours.front(), colours.size());
}

void Cb4aLevelVertexBuffer::PostRender()
{
	static CIwSVec3 fakeStream[1];
	IwGxSetVertStream(&fakeStream[0],1);
	IwGxSetNormStream(0,0);
	IwGxSetUVStream(0,0);
	IwGxSetUVStream(0, 1);
	IwGxSetColStream(0,0);
}

#ifdef IW_BUILD_RESOURCES
void* Bsp4Airplay::Cb4aLevelVertexBufferFactory()
{
	return &g_parseLevelVertexBuffer;
}

// Parse from text file: start block.
void  ParseLevelVertexBuffer::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	Cb4aLevel* level = dynamic_cast<Cb4aLevel*>(pParser->GetObject(-1));
	_this = level->AllocateLevelVertexBuffer();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool ParseLevelVertexBuffer::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("num_vertices", pAttrName))
	{
		uint32 num_verts;
		pParser->ReadUInt32(&num_verts);
		_this->SetCapacity(num_verts);
		return true;
	}if (!strcmp("v", pAttrName))
	{
		CIwSVec3 v;
		pParser->ReadInt16Array(&v.x,3);
		_this->positions.push_back(v);
		return true;
	}
	if (!strcmp("vn", pAttrName))
	{
		CIwSVec3 vn;
		pParser->ReadInt16Array(&vn.x,3);
		_this->normals.push_back(vn);
		return true;
	}
	if (!strcmp("uv0", pAttrName))
	{
		CIwSVec2 uv0;
		pParser->ReadInt16Array(&uv0.x,2);
		_this->uv0s.push_back(uv0);
		return true;
	}
	if (!strcmp("uv1", pAttrName))
	{
		CIwSVec2 uv1;
		pParser->ReadInt16Array(&uv1.x,2);
		_this->uv1s.push_back(uv1);
		return true;
	}
	if (!strcmp("col", pAttrName))
	{
		uint8 col[4];
		pParser->ReadUInt8Array(&col[0],4);
		CIwColour c;
		c.Set(col[0],col[1],col[2],col[3]);
		_this->colours.push_back(c);
		return true;
	}
	return CIwManaged::ParseAttribute(pParser,pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void ParseLevelVertexBuffer::ParseClose(CIwTextParserITX* pParser)
{
}
#endif