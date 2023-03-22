using System.Collections.Generic;
using System.Numerics;

namespace Axvemi.Commons.Meshes.Samples;

/// <summary>
/// Mesh generated for a chunk. Contains one entry in the list per id
/// TODO: Would be cool to make it generic so it doesn't force to use a Tile<TileData>
/// </summary>
public class ChunkMesh
{
	public List<MeshData> MeshDatas { get; } = new();

	public ChunkMesh(Chunk<Tile<TileData>> chunk)
	{
		Dictionary<string, List<Tile<TileData>>> idObjectsDictionary = new();
		//Add each tile to its corresponding dictionary
		for (int x = 0; x < Chunk.ObjectsPerSide.X; x++)
		{
			for (int y = 0; y < Chunk.ObjectsPerSide.Y; y++)
			{
				for (int z = 0; z < Chunk.ObjectsPerSide.Z; z++)
				{
					Tile<TileData> tile = chunk.GetObjectAtLocalCoordinates(x, y, z);
					if (tile.TileObject == null) continue;
					//Add to dictionary
					string id = tile.TileObject.GetId();
					if (!idObjectsDictionary.ContainsKey(id))
					{
						idObjectsDictionary.Add(id, new List<Tile<TileData>>());
					}

					idObjectsDictionary[id].Add(tile);
				}
			}
		}

		//Generate the mesh data for each dictionary
		foreach (KeyValuePair<string, List<Tile<TileData>>> pair in idObjectsDictionary)
		{
			MeshData chunkMeshById = new MeshData(pair.Key);
			MeshDatas.Add(chunkMeshById);
			foreach (Tile<TileData> tile in pair.Value)
			{
				AddVoxelData(chunkMeshById, tile);
			}
		}
	}

	/// <summary>
	/// Add the data for the voxel of that tile to the MeshData
	/// </summary>
	/// <param name="meshData">MeshData to add the info into</param>
	/// <param name="tile">Tile with the info</param>
	private void AddVoxelData(MeshData meshData, Tile<TileData> tile)
	{
		if (tile.TileObject == null || !tile.TileObject.ShouldDraw()) return;
		Tile<TileData>[] adjacentTiles = tile.GetAdjacentTiles();
		Vector3 offset = new Vector3(tile.X * Tile.Width, tile.Y * Tile.Height, tile.Z * Tile.Depth);

		//Foreach face
		for (int i = 0; i < VoxelData.FaceVertices.GetLength(0); i++)
		{
			if (!tile.TileObject.ShouldDrawFace(i, adjacentTiles)) continue;

			//Add the vertices of that face, and the uvs
			for (int j = 0; j < VoxelData.FaceVertices.GetLength(1); j++)
			{
				Vector3 vertice = VoxelData.Verts[VoxelData.FaceVertices[i, j]] + offset;
				meshData.Vertices.Add(new Vector3(vertice.X, vertice.Y, vertice.Z));
				Vector2 uv = VoxelData.UvsVFlip[j];
				meshData.Uvs.Add(new Vector2(uv.X, uv.Y));
				Vector3 normal = VoxelData.Normals[i];
				meshData.Normals.Add(new Vector3(normal.X, normal.Y, normal.Z));
			}

			//Add the indices
			for (int j = 0; j < VoxelData.IndiceVertexOrderCc.Length; j++)
			{
				meshData.Indices.Add(VoxelData.IndiceVertexOrderCc[j] + meshData.CurrentIndex);
			}

			meshData.CurrentIndex += VoxelData.FaceVertices.GetLength(1);
		}
	}
}