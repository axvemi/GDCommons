using Axvemi.Commons.Spatial;
using Godot;

namespace Axvemi.Commons.Meshes.Samples;

[Tool]
public partial class TileData : Resource
{
	[Export()] public string Id;
	[Export()] public string Name;
	[Export()] public BaseMaterial3D Material;
	public string GetId() => Id;

	public bool ShouldDraw() => true;

	public bool ShouldDrawFace(int face, Tile<TileData>[] adjacentTiles)
	{
		switch (face)
		{
			case VoxelData.FaceNegativeZ when adjacentTiles[Tile.AdjacentTileNegativeZ]?.TileObject != null:
			case VoxelData.FaceZ when adjacentTiles[Tile.AdjacentTileZ]?.TileObject != null:
			case VoxelData.FaceNegativeY when adjacentTiles[Tile.AdjacentTileNegativeY]?.TileObject != null:
			case VoxelData.FaceY when adjacentTiles[Tile.AdjacentTileY]?.TileObject != null:
			case VoxelData.FaceNegativeX when adjacentTiles[Tile.AdjacentTileNegativeX]?.TileObject != null:
			case VoxelData.FaceX when adjacentTiles[Tile.AdjacentTileX]?.TileObject != null:
				return false;
			default:
				return true;
		}
	}

	public override string ToString()
	{
		return $"Id: {Id}; Name: {Name}";
	}
}