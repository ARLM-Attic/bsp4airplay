#include <b4aLevelVB.h>

using namespace Bsp4Airplay;

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
void Cb4aLevelVertexBuffer::PreRender()
{
	if (!positionsStream.IsSet())
	{
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
	IwGxSetVertStreamModelSpace(positionsStream);
	IwGxSetNormStream(normalsStream);
	IwGxSetUVStream(uv0Stream, 0);
	IwGxSetUVStream(uv1Stream, 1);
	IwGxSetColStream(coloursStream);

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
