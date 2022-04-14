using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BridgeForSbox;

struct QuixelAsset
{
	[JsonPropertyName( "resolution" )] public string Resolution { get; set; }
	[JsonPropertyName( "resolutionValue" )] public int ResolutionValue { get; set; }
	[JsonPropertyName( "category" )] public string Category { get; set; }
	[JsonPropertyName( "type" )] public string Type { get; set; }
	[JsonPropertyName( "id" )] public string Id { get; set; }
	[JsonPropertyName( "name" )] public string Name { get; set; }
	[JsonPropertyName( "directoryName" )] public string DirectoryName { get; set; }
	[JsonPropertyName( "path" )] public string Path { get; set; }
	[JsonPropertyName( "textureMimeType" )] public string TextureMimeType { get; set; }
	[JsonPropertyName( "averageColor" )] public string AverageColor { get; set; }
	[JsonPropertyName( "activeLOD" )] public string ActiveLOD { get; set; }
	[JsonPropertyName( "tags" )] public string[] Tags { get; set; }
	[JsonPropertyName( "categories" )] public string[] Categories { get; set; }
	[JsonPropertyName( "components" )] public List<Texture> Textures { get; set; }
	[JsonPropertyName( "meshList" )] public List<Mesh> Meshes { get; set; }
	[JsonPropertyName( "lodList" )] public List<MeshLOD> LODs { get; set; }
	[JsonPropertyName( "packedTextures" )] public List<PackedTextures> PackedTextures { get; set; }
	[JsonPropertyName( "meta" )] public List<MetaElement> Meta { get; set; }
}

interface IBaseAsset
{
	public string Path { get; set; }
	public string Name { get; set; }
}

struct Texture : IBaseAsset
{
	[JsonPropertyName( "name" )] public string Name { get; set; }
	[JsonPropertyName( "path" )] public string Path { get; set; }
	[JsonPropertyName( "resolution" )] public string Resolution { get; set; }
	[JsonPropertyName( "format" )] public string Format { get; set; }
	[JsonPropertyName( "type" )] public string Type { get; set; }
}

struct Mesh : IBaseAsset
{
	[JsonPropertyName( "path" )] public string Path { get; set; }
	[JsonPropertyName( "name" )] public string Name { get; set; }
	[JsonPropertyName( "format" )] public string Format { get; set; }
	[JsonPropertyName( "type" )] public string Type { get; set; }
}

struct MeshLOD : IBaseAsset
{
	[JsonPropertyName( "lod" )] public string LOD { get; set; }
	[JsonPropertyName( "path" )] public string Path { get; set; }
	[JsonPropertyName( "name" )] public string Name { get; set; }
	[JsonPropertyName( "format" )] public string Format { get; set; }
	[JsonPropertyName( "type" )] public string Type { get; set; }
}

struct PackedTextures : IBaseAsset
{
	[JsonPropertyName( "name" )] public string Name { get; set; }
	[JsonPropertyName( "path" )] public string Path { get; set; }
	[JsonPropertyName( "resolution" )] public string Resolution { get; set; }
	[JsonPropertyName( "format" )] public string Format { get; set; }
	[JsonPropertyName( "type" )] public string Type { get; set; }
	[JsonPropertyName( "channelsData" )] public ChannelsData ChannelsData { get; set; }
}

struct ChannelsData
{
	[JsonPropertyName( "Red" )] public ChannelInfo Red { get; set; }
	[JsonPropertyName( "Green" )] public ChannelInfo Green { get; set; }
	[JsonPropertyName( "Blue" )] public ChannelInfo Blue { get; set; }
	[JsonPropertyName( "Alpha" )] public ChannelInfo Alpha { get; set; }
	[JsonPropertyName( "Grayscale" )] public ChannelInfo Grayscale { get; set; }
}

struct ChannelInfo
{
	[JsonPropertyName( "type" )] public string Type { get; set; }
	[JsonPropertyName( "channel" )] public string Channel { get; set; }
}

struct MetaElement
{
	[JsonPropertyName( "value" )] public object Value { get; set; }
	[JsonPropertyName( "name" )] public string Name { get; set; }
	[JsonPropertyName( "key" )] public string Key { get; set; }
}
