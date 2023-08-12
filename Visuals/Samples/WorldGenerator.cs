using Axvemi.Commons.Spatial;
using Godot;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace Axvemi.Commons.Meshes.Samples;

[Tool]
public static class WorldGenerator
{
	public static void SetTileData(Tile<TileData> tile, int seed, float frequency, TileData bedrockTileData, TileData dirtTileData, TileData grassTileData)
	{
		Vector3 coordinates = tile.GetWorldPosition();
		Vector2 positionWorld = new Vector2(coordinates.X, coordinates.Z);
		TileData tileData = null;

		//Bottom always a block
		if (coordinates.Y == 0)
		{
			tile.TileObject = bedrockTileData;
			return;
		}

		//Basic terrain pass
		int terrainHeight = Mathf.FloorToInt(Chunk.ObjectsPerSide.Y * Get2DPerlin(seed, positionWorld, frequency));
		//int terrainHeight = Chunk.CHUNK_OBJECTS_PER_SIDE.y / 2;
		if (tile.Y <= terrainHeight)
		{
			tileData = dirtTileData;
		}

		if (tile.Y == terrainHeight)
		{
			if (Get2DPerlin(seed, positionWorld, frequency) > 0.5f)
			{
				tileData = grassTileData;

				if (Get2DPerlin(seed, positionWorld, frequency) > 0.7f)
				{
					tileData = dirtTileData;
				}
			}
		}

		tile.TileObject = tileData;
	}

	private static float Get2DPerlin(int seed, Vector2 position, float frequency)
	{
		var noise = new FastNoiseLite();
		noise.Seed = seed;
		noise.Frequency = frequency;

		return noise.GetNoise2D(position.X, position.Y);
	}
}