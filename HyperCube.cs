using Godot;

public partial class HyperCube : Node3D
{
	[Export] public float Width = 10.0f;
	[Export] public float Height = 10.0f;
	[Export] public float Depth = 10.0f;
	[Export] public float SizeW = 10.0f;
	[Export] public float VoxelSize = 1.0f;
	[Export] public PackedScene VoxelScene;
	[Export] public bool IsHollow = false;
	public float RotationXY = 0.0f;
	public float RotationXZ = 0.0f;
	public float RotationXW = 0.0f;
	public float RotationYZ = 0.0f;
	public float RotationYW = 0.0f;
	public float RotationZW = 0.0f;

	[Export] private Mesh edgeMesh;
	[Export] private Mesh otherMesh;

	private Vector4[,,,] initialCoords;

	public override void _Ready()
	{
		GenerateHyperCube();
	}

	public void GenerateHyperCube()
	{
		ClearVoxels();

		int voxelCountX = Mathf.CeilToInt(Width / VoxelSize);
		int voxelCountY = Mathf.CeilToInt(Height / VoxelSize);
		int voxelCountZ = Mathf.CeilToInt(Depth / VoxelSize);
		int voxelCountW = Mathf.CeilToInt(SizeW / VoxelSize);

		initialCoords = new Vector4[voxelCountX, voxelCountY, voxelCountZ, voxelCountW];

		Vector4 offset = new Vector4(
			-Width / 2.0f,
			-Height / 2.0f,
			-Depth / 2.0f,
			-SizeW / 2.0f
		);

		for (int x = 0; x < voxelCountX; x++)
		{
			for (int y = 0; y < voxelCountY; y++)
			{
				for (int z = 0; z < voxelCountZ; z++)
				{
					for (int w = 0; w < voxelCountW; w++)
					{
						bool edge = (
							(x == 0 || x == voxelCountX - 1) && (y == 0 || y == voxelCountY - 1) && (z == 0 || z == voxelCountZ - 1) ||
							(x == 0 || x == voxelCountX - 1) && (y == 0 || y == voxelCountY - 1) && (w == 0 || w == voxelCountW - 1) ||
							(x == 0 || x == voxelCountX - 1) && (z == 0 || z == voxelCountZ - 1) && (w == 0 || w == voxelCountW - 1) ||
							(y == 0 || y == voxelCountY - 1) && (z == 0 || z == voxelCountZ - 1) && (w == 0 || w == voxelCountW - 1)
						);

						if (!edge && IsHollow)
							continue;

						Vector4 ourCoords = new Vector4(x * VoxelSize, y * VoxelSize, z * VoxelSize, w * VoxelSize) + offset;
						initialCoords[x, y, z, w] = ourCoords;

						Vector4 rotatedCoords = RotateCoordinates(ourCoords);
						CreateVoxel(rotatedCoords, edge ? edgeMesh : otherMesh);
					}
				}
			}
		}
	}


	private void ClearVoxels()
	{
		foreach (Node child in GetChildren())
		{
			if (child is Voxel4D)
			{
				child.QueueFree();
			}
		}
	}

	private Vector4 RotateCoordinates(Vector4 coords)
	{
		// Rotate coordinates around all 6 axes in 4D space
		Vector4 rotated = coords;

		// Apply rotation around each of the 6 axes in 4D space
		rotated = RotateAroundXY(rotated, Mathf.DegToRad(RotationXY));
		rotated = RotateAroundXZ(rotated, Mathf.DegToRad(RotationXZ));
		rotated = RotateAroundXW(rotated, Mathf.DegToRad(RotationXW));
		rotated = RotateAroundYZ(rotated, Mathf.DegToRad(RotationYZ));
		rotated = RotateAroundYW(rotated, Mathf.DegToRad(RotationYW));
		rotated = RotateAroundZW(rotated, Mathf.DegToRad(RotationZW));

		return rotated;
	}

	private Vector4 RotateAroundXY(Vector4 coords, float angle)
	{
		// Rotate around the XY plane
		float cosTheta = Mathf.Cos(angle);
		float sinTheta = Mathf.Sin(angle);
		return new Vector4(
			cosTheta * coords.X - sinTheta * coords.Y,
			sinTheta * coords.X + cosTheta * coords.Y,
			coords.Z,
			coords.W
		);
	}

	private Vector4 RotateAroundXZ(Vector4 coords, float angle)
	{
		// Rotate around the XZ plane
		float cosTheta = Mathf.Cos(angle);
		float sinTheta = Mathf.Sin(angle);
		return new Vector4(
			cosTheta * coords.X - sinTheta * coords.Z,
			coords.Y,
			sinTheta * coords.X + cosTheta * coords.Z,
			coords.W
		);
	}

	private Vector4 RotateAroundXW(Vector4 coords, float angle)
	{
		// Rotate around the XW plane
		float cosTheta = Mathf.Cos(angle);
		float sinTheta = Mathf.Sin(angle);
		return new Vector4(
			cosTheta * coords.X - sinTheta * coords.W,
			coords.Y,
			coords.Z,
			sinTheta * coords.X + cosTheta * coords.W
		);
	}

	private Vector4 RotateAroundYZ(Vector4 coords, float angle)
	{
		// Rotate around the YZ plane
		float cosTheta = Mathf.Cos(angle);
		float sinTheta = Mathf.Sin(angle);
		return new Vector4(
			coords.X,
			cosTheta * coords.Y - sinTheta * coords.Z,
			sinTheta * coords.Y + cosTheta * coords.Z,
			coords.W
		);
	}

	private Vector4 RotateAroundYW(Vector4 coords, float angle)
	{
		// Rotate around the YW plane
		float cosTheta = Mathf.Cos(angle);
		float sinTheta = Mathf.Sin(angle);
		return new Vector4(
			coords.X,
			cosTheta * coords.Y - sinTheta * coords.W,
			coords.Z,
			sinTheta * coords.Y + cosTheta * coords.W
		);
	}

	private Vector4 RotateAroundZW(Vector4 coords, float angle)
	{
		// Rotate around the ZW plane
		float cosTheta = Mathf.Cos(angle);
		float sinTheta = Mathf.Sin(angle);
		return new Vector4(
			coords.X,
			coords.Y,
			cosTheta * coords.Z - sinTheta * coords.W,
			sinTheta * coords.Z + cosTheta * coords.W
		);
	}

	private void CreateVoxel(Vector4 coords, Mesh mesh)
	{
		if (VoxelScene == null)
		{
			GD.PrintErr("VoxelScene is not set.");
			return;
		}

		Voxel4D voxelInstance = (Voxel4D)VoxelScene.Instantiate();
		voxelInstance.X = coords.X;
		voxelInstance.Y = coords.Y;
		voxelInstance.Z = coords.Z;
		voxelInstance.W = coords.W;
		voxelInstance.mesh = mesh;

		AddChild(voxelInstance);
	}

	public void UpdateVoxelPositions()
	{
		int voxelCountX = Mathf.CeilToInt(Width / VoxelSize);
		int voxelCountY = Mathf.CeilToInt(Height / VoxelSize);
		int voxelCountZ = Mathf.CeilToInt(Depth / VoxelSize);
		int voxelCountW = Mathf.CeilToInt(SizeW / VoxelSize);

		int voxelIndex = 0;

		for (int w = 0; w < voxelCountW; w++)
		{
			for (int z = 0; z < voxelCountZ; z++)
			{
				for (int y = 0; y < voxelCountY; y++)
				{
					for (int x = 0; x < voxelCountX; x++)
					{
						if (voxelIndex >= GetChildCount())
							return;

						bool edge = (
							(x == 0 || x == voxelCountX - 1) && (y == 0 || y == voxelCountY - 1) && (z == 0 || z == voxelCountZ - 1) ||
							(x == 0 || x == voxelCountX - 1) && (y == 0 || y == voxelCountY - 1) && (w == 0 || w == voxelCountW - 1) ||
							(x == 0 || x == voxelCountX - 1) && (z == 0 || z == voxelCountZ - 1) && (w == 0 || w == voxelCountW - 1) ||
							(y == 0 || y == voxelCountY - 1) && (z == 0 || z == voxelCountZ - 1) && (w == 0 || w == voxelCountW - 1)
						);

						if (!edge && IsHollow)
							continue;

						UpdateVoxel(x, y, z, w, edge);
						voxelIndex++;
					}
				}
			}
		}


		void UpdateVoxel(int x, int y, int z, int w, bool edge)
		{
			if (GetChild(voxelIndex) is Voxel4D voxel)
			{
				Vector4 initialCoord = initialCoords[x, y, z, w];
				Vector4 rotatedCoord = RotateCoordinates(initialCoord);

				voxel.X = rotatedCoord.X;
				voxel.Y = rotatedCoord.Y;
				voxel.Z = rotatedCoord.Z;
				voxel.W = rotatedCoord.W;
				voxel.mesh = edge ? edgeMesh : otherMesh;

				voxel._Ready();
			}
		}


	}
}
