using System;
using System.Collections.Generic;
using System.Text;

namespace BspFileFormat.HL2
{
	public class header_t
	{
		public uint magic;      // magic number ("VBSP")
		public uint version;

public dentry_t Entities; //Map entities
public dentry_t Planes; //Plane array
public dentry_t Texdata; //Index to texture names
public dentry_t Vertexes; //Vertex array
public dentry_t Visibility; //Compressed visibility bit arrays
public dentry_t Nodes; //BSP tree nodes  
public dentry_t Texinfo; //Face texture array
public dentry_t Faces; //Face array
public dentry_t Lighting; //Lightmap samples
public dentry_t Occlusion; //Occlusion data(?)
public dentry_t Leafs; //BSP tree leaf nodes
public dentry_t Unused11; //
public dentry_t Edges; //Edge array
public dentry_t Surfedges; //Index of edges
public dentry_t Models; //Brush models (geometry of brush entities)
public dentry_t Worldlights; //Light entities
public dentry_t LeafFaces; //Index to faces in each leaf
public dentry_t LeafBrushes; //Index to brushes in each leaf
public dentry_t Brushes; //Brush array
public dentry_t Brushsides; //Brushside array
public dentry_t Areas; //Area array
public dentry_t AreaPortals; //Portals between areas
public dentry_t Portals; //Polygons defining the boundary between adjacent leaves(?)
public dentry_t Clusters; //Leaves that are enterable by the player
public dentry_t PortalVerts; //Vertices of portal polygons
public dentry_t Clusterportals; //Polygons defining the boundary between adjacent clusters(?)
public dentry_t Dispinfo; //Displacement surface array
public dentry_t OriginalFaces; //Brush faces array before BSP splitting
public dentry_t Unused28; //
public dentry_t PhysCollide; //Physics collision data(?)
public dentry_t VertNormals; //Vertex normals(?)
public dentry_t VertNormalIndices; //Vertex normal index array(?)
public dentry_t DispLightmapAlphas; //Displacement lightmap data(?)
public dentry_t DispVerts; //Vertices of displacement surface meshes
public dentry_t DispLightmapSamplePos; //Displacement lightmap data(?)
public dentry_t GameLump; //Game-specific data lump
public dentry_t LeafWaterData; // (?)
public dentry_t Primitives; //Non-polygonal primatives(?)
public dentry_t PrimVerts; // (?)
public dentry_t PrimIndices; //(?)
public dentry_t Pakfile; //Embedded uncompressed-Zip format file
public dentry_t ClipPortalVerts; //(?)
public dentry_t Cubemaps; //Env_cubemap location array
public dentry_t TexdataStringData; //Texture name data
public dentry_t TexdataStringTable; //Index array into texdata string data
public dentry_t Overlays; //Info_overlay array       
public dentry_t LeafMinDistToWater; //(?)
public dentry_t FaceMacroTextureInfo; //(?)
public dentry_t DispTris; //Displacement surface triangles
public dentry_t PhysCollideSurface; //Physics collision surface data(?)
public dentry_t Unused50; //
public dentry_t Unused51; //
public dentry_t Unused52; //
public dentry_t LightingHDR; //HDR related lighting data(?)
public dentry_t WorldlightsHDR; //HDR related worldlight data(?)
public dentry_t LeaflightHDR1; //HDR related leaf lighting data(?)
public dentry_t LeaflightHDR2; //HDR related leaf lighting data(?)
public dentry_t Unused57;
public dentry_t Unused58;
public dentry_t Unused59;
public dentry_t Unused60;
public dentry_t Unused61;
public dentry_t Unused62;
public dentry_t Unused63;

		public uint revision;

		internal void Read(System.IO.BinaryReader source)
		{
			magic = source.ReadUInt32();
			version = source.ReadUInt32();

			Entities.Read(source); //Map entities
Planes.Read(source); //Plane array
Texdata.Read(source); //Index to texture names
Vertexes.Read(source); //Vertex array
Visibility.Read(source); //Compressed visibility bit arrays
Nodes.Read(source); //BSP tree nodes  
Texinfo.Read(source); //Face texture array
Faces.Read(source); //Face array
Lighting.Read(source); //Lightmap samples
Occlusion.Read(source); //Occlusion data(?)
Leafs.Read(source); //BSP tree leaf nodes
Unused11.Read(source); //
Edges.Read(source); //Edge array
Surfedges.Read(source); //Index of edges
Models.Read(source); //Brush models (geometry of brush entities)
Worldlights.Read(source); //Light entities
LeafFaces.Read(source); //Index to faces in each leaf
LeafBrushes.Read(source); //Index to brushes in each leaf
Brushes.Read(source); //Brush array
Brushsides.Read(source); //Brushside array
Areas.Read(source); //Area array
AreaPortals.Read(source); //Portals between areas
Portals.Read(source); //Polygons defining the boundary between adjacent leaves(?)
Clusters.Read(source); //Leaves that are enterable by the player
PortalVerts.Read(source); //Vertices of portal polygons
Clusterportals.Read(source); //Polygons defining the boundary between adjacent clusters(?)
Dispinfo.Read(source); //Displacement surface array
OriginalFaces.Read(source); //Brush faces array before BSP splitting
Unused28.Read(source); //
PhysCollide.Read(source); //Physics collision data(?)
VertNormals.Read(source); //Vertex normals(?)
VertNormalIndices.Read(source); //Vertex normal index array(?)
DispLightmapAlphas.Read(source); //Displacement lightmap data(?)
DispVerts.Read(source); //Vertices of displacement surface meshes
DispLightmapSamplePos.Read(source); //Displacement lightmap data(?)
GameLump.Read(source); //Game-specific data lump
LeafWaterData.Read(source); // (?)
Primitives.Read(source); //Non-polygonal primatives(?)
PrimVerts.Read(source); // (?)
PrimIndices.Read(source); //(?)
Pakfile.Read(source); //Embedded uncompressed-Zip format file
ClipPortalVerts.Read(source); //(?)
Cubemaps.Read(source); //Env_cubemap location array
TexdataStringData.Read(source); //Texture name data
TexdataStringTable.Read(source); //Index array into texdata string data
Overlays.Read(source); //Info_overlay array       
LeafMinDistToWater.Read(source); //(?)
FaceMacroTextureInfo.Read(source); //(?)
DispTris.Read(source); //Displacement surface triangles
PhysCollideSurface.Read(source); //Physics collision surface data(?)
Unused50.Read(source); //
Unused51.Read(source); //
Unused52.Read(source); //
LightingHDR.Read(source); //HDR related lighting data(?)
WorldlightsHDR.Read(source); //HDR related worldlight data(?)
LeaflightHDR1.Read(source); //HDR related leaf lighting data(?)
LeaflightHDR2.Read(source); //HDR related leaf lighting data(?)
Unused57.Read(source);
Unused58.Read(source);
Unused59.Read(source);
Unused60.Read(source);
Unused61.Read(source);
Unused62.Read(source);
Unused63.Read(source);

			revision = source.ReadUInt32();
		}
	}
}
