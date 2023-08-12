using System.Linq;
using System.Threading.Tasks;
using Axvemi.Commons.Spatial;
using Godot;
using Godot.Collections;
using Vector2 = System.Numerics.Vector2;

namespace Axvemi.Commons.Meshes.Samples;

[Tool]
public partial class CreateWorldMeshController : Node
{
	[Export()] public bool Refresh;

	[ExportGroup("World")]
	[Export()] private int _gridWidth = 1;
	[Export()] private int _gridHeight = 1;

	//TODO: Export TileData directly when fixed on the engine
	[ExportGroup("Tile Data")]
	private const string GrassDataPath = "CommonsGodot/Visuals/Samples/Tiles/grass_block_data.tres";
	private const string DirtDataPath = "CommonsGodot/Visuals/Samples/Tiles/dirt_block_data.tres";
	private const string BedrockDataPath = "CommonsGodot/Visuals/Samples/Tiles/bedrock_block_data.tres";

	[ExportGroup("Noise")]
	[Export()] private int _seed;
	[Export()] private float _frequency = 0.1f;

	private Grid<Chunk<Tile<TileData>>> _grid;
	private TileData _grassTileData;
	private TileData _dirtTileData;
	private TileData _bedrockTileData;

	private static Chunk<Tile<TileData>> GenerateChunk(Grid<Chunk<Tile<TileData>>> grid, int x, int y)
	{
		return new Chunk<Tile<TileData>>(grid, x, y, GenerateTile);
	}

	private static Tile<TileData> GenerateTile(Chunk<Tile<TileData>> chunk, int x, int y, int z)
	{
		return new Tile<TileData>(chunk, x, y, z, (_) => new TileData());
	}

	public override void _Process(double delta)
	{
		if (Refresh)
		{
			_grassTileData = GD.Load<TileData>(GrassDataPath);
			_dirtTileData = GD.Load<TileData>(DirtDataPath);
			_bedrockTileData = GD.Load<TileData>(BedrockDataPath);

			Clean();
			GenerateWorld();
			Refresh = false;
		}
	}

	private void Clean()
	{
		_grid = null;
		foreach (Node child in GetChildren())
		{
			RemoveChild(child);
			child.QueueFree();
		}
	}

	private void GenerateWorld()
	{
		_grid = new Grid<Chunk<Tile<TileData>>>(_gridWidth, _gridHeight, Vector2.Zero, GenerateChunk);
		foreach (Chunk<Tile<TileData>> chunk in _grid.GridObjectsDimensionalArray)
		{
			foreach (Tile<TileData> tile in chunk.ChunkObjectsDimensionalArray)
			{
				WorldGenerator.SetTileData(tile, _seed, _frequency, _bedrockTileData, _dirtTileData, _grassTileData);
			}
		}

		GenerateChunkMeshes();
	}

	private async void GenerateChunkMeshes()
	{
		for (int x = 0; x < _grid.Width; x++)
		{
			for (int z = 0; z < _grid.Height; z++)
			{
				await GenerateChunkMesh(_grid.GetObjectAtCoordinates(x, z));
			}
		}
	}

	private async Task GenerateChunkMesh(Chunk<Tile<TileData>> chunk)
	{
		Vector2 chunkPosition = _grid.GetWorldPositionFromCoordinates(chunk.X * Chunk.ChunkSize, chunk.Y * Chunk.ChunkSize);

		//Create Root
		Node3D chunkRoot = new Node3D();
		chunkRoot.Position = new Vector3(chunkPosition.X, 0, chunkPosition.Y);
		AddChild(chunkRoot);
		chunkRoot.Owner = GetTree().EditedSceneRoot;

		//Create each mesh
		ChunkMesh chunkMesh = new ChunkMesh(chunk);
		foreach (var chunkMeshById in chunkMesh.MeshDatas)
		{
			Array arrays = new Array();
			arrays.Resize((int)Mesh.ArrayType.Max);
			arrays[(int)Mesh.ArrayType.Vertex] = chunkMeshById.Vertices.Select(vertex => new Vector3(vertex.X, vertex.Y, vertex.Z)).ToArray();
			arrays[(int)Mesh.ArrayType.Index] = chunkMeshById.Indices.Select(index => index).ToArray();
			arrays[(int)Mesh.ArrayType.TexUV] = chunkMeshById.Uvs.Select(uv => new Godot.Vector2(uv.X, uv.Y)).ToArray();
			arrays[(int)Mesh.ArrayType.Normal] = chunkMeshById.Normals.Select(normal => new Vector3(normal.X, normal.Y, normal.Z)).ToArray();

			// Create the Mesh.
			ArrayMesh arrayMesh = new ArrayMesh();
			arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

			MeshInstance3D meshInstance3D = new MeshInstance3D();
			meshInstance3D.Mesh = arrayMesh;
			BaseMaterial3D material = null;
			switch (chunkMeshById.Id)
			{
				case "grass":
					material = _grassTileData.Material;
					break;
				case "dirt":
					material = _dirtTileData.Material;
					break;
				case "bedrock":
					material = _bedrockTileData.Material;
					break;
			}

			meshInstance3D.MaterialOverride = material;
			chunkRoot.AddChild(meshInstance3D);
			meshInstance3D.Owner = GetTree().EditedSceneRoot;
		}

		await Task.Delay(1);
	}
}