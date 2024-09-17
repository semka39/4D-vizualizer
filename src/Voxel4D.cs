using Godot;

public partial class Voxel4D : Node3D
{
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
	public float W { get; set; }
	public Mesh mesh { get; set; }

	public override void _Ready()
	{
		Transform3D transform = Transform;
		transform.Origin = ProjectTo3D(new Vector4(X, Y, Z, W), Control.ProjectionNormal);
		//.Normalized() == Vector6.Inf ? Vector6.Zero : Control.ProjectionNormal.Normalized()
		Transform = transform;

		MeshInstance3D meshInstance = GetChild<MeshInstance3D>(0);
		if (meshInstance.Mesh != mesh)
			meshInstance.Mesh = mesh;
	}

	private Vector3 ProjectTo3D(Vector4 point, Vector6 normal)
	{
		float x = point.X - normal.XY * point.Y - normal.XZ * point.Z - normal.XW * point.W;
		float y = point.Y - normal.YZ * point.Z - normal.YW * point.W;
		float z = point.Z - normal.ZW * point.W;

		return new Vector3(x, y, z);
	}
}

public struct Vector6
{
	public float XY, XZ, YZ, XW, YW, ZW;

	public Vector6(float xy, float xz, float yz, float xw, float yw, float zw)
	{
		XY = xy;
		XZ = xz;
		YZ = yz;
		XW = xw;
		YW = yw;
		ZW = zw;
	}

	public Vector6 Normalized()
	{
		float magnitude = Mathf.Sqrt(XY * XY + XZ * XZ + YZ * YZ + XW * XW + YW * YW + ZW * ZW);
		if (magnitude == 0)
			return new Vector6(0, 0, 0, 0, 0, 0);
		return new Vector6(XY / magnitude, XZ / magnitude, YZ / magnitude, XW / magnitude, YW / magnitude, ZW / magnitude);
	}

	public bool IsInf()
	{
		return float.IsInfinity(XY) || float.IsInfinity(XZ) || float.IsInfinity(YZ) ||
			   float.IsInfinity(XW) || float.IsInfinity(YW) || float.IsInfinity(ZW);
	}

	public bool IsZero()
	{
		return XY == 0 && XZ == 0 && YZ == 0 && XW == 0 && YW == 0 && ZW == 0;
	}

	public static Vector6 Zero => new Vector6(0, 0, 0, 0, 0, 0);
	public static Vector6 Inf => new Vector6(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity,
											 float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

	public static bool operator ==(Vector6 a, Vector6 b)
	{
		return a.XY == b.XY && a.XZ == b.XZ && a.YZ == b.YZ &&
			   a.XW == b.XW && a.YW == b.YW && a.ZW == b.ZW;
	}

	public static bool operator !=(Vector6 a, Vector6 b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Vector6))
			return false;

		var other = (Vector6)obj;
		return this == other;
	}

	public override int GetHashCode()
	{
		return XY.GetHashCode() ^ XZ.GetHashCode() ^ YZ.GetHashCode() ^
			   XW.GetHashCode() ^ YW.GetHashCode() ^ ZW.GetHashCode();
	}
}
