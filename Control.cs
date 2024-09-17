using Godot;
using System;

public partial class Control : CanvasLayer
{

	[Export] HyperCube hyperCube;
	public static Vector6 ProjectionNormal = new Vector6(1, 1, 1, 1, 1, 1).Normalized();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	private void UpdateHyperCube()
	{
		hyperCube.RotationXY = (float)GetChild(0).GetChild(0).GetChild<Slider>(0).Value;
		hyperCube.RotationXZ = (float)GetChild(0).GetChild(1).GetChild<Slider>(0).Value;
		hyperCube.RotationXW = (float)GetChild(0).GetChild(2).GetChild<Slider>(0).Value;
		hyperCube.RotationYZ = (float)GetChild(0).GetChild(3).GetChild<Slider>(0).Value;
		hyperCube.RotationYW = (float)GetChild(0).GetChild(4).GetChild<Slider>(0).Value;
		hyperCube.RotationZW = (float)GetChild(0).GetChild(5).GetChild<Slider>(0).Value;

		ProjectionNormal.XY = (float)GetChild(0).GetChild(6).GetChild<Slider>(0).Value;
		ProjectionNormal.XZ = (float)GetChild(0).GetChild(7).GetChild<Slider>(0).Value;
		ProjectionNormal.XW = (float)GetChild(0).GetChild(8).GetChild<Slider>(0).Value;
		ProjectionNormal.YZ = (float)GetChild(0).GetChild(9).GetChild<Slider>(0).Value;
		ProjectionNormal.YW = (float)GetChild(0).GetChild(10).GetChild<Slider>(0).Value;
		ProjectionNormal.ZW = (float)GetChild(0).GetChild(11).GetChild<Slider>(0).Value;

		//ProjectionNormal = ProjectionNormal.Normalized();

		hyperCube.UpdateVoxelPositions();
	}

	private void drag_ended(bool value_changed)
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bool XY = hyperCube.RotationXY == (float)GetChild(0).GetChild(0).GetChild<Slider>(0).Value;
		bool XZ = hyperCube.RotationXZ == (float)GetChild(0).GetChild(1).GetChild<Slider>(0).Value;
		bool XW = hyperCube.RotationXW == (float)GetChild(0).GetChild(2).GetChild<Slider>(0).Value;
		bool YZ = hyperCube.RotationYZ == (float)GetChild(0).GetChild(3).GetChild<Slider>(0).Value;
		bool YW = hyperCube.RotationYW == (float)GetChild(0).GetChild(4).GetChild<Slider>(0).Value;
		bool ZW = hyperCube.RotationZW == (float)GetChild(0).GetChild(5).GetChild<Slider>(0).Value;

		bool XYp = ProjectionNormal.XY == (float)GetChild(0).GetChild(6).GetChild<Slider>(0).Value;
		bool XZp = ProjectionNormal.XZ == (float)GetChild(0).GetChild(7).GetChild<Slider>(0).Value;
		bool XWp = ProjectionNormal.XW == (float)GetChild(0).GetChild(8).GetChild<Slider>(0).Value;
		bool YZp = ProjectionNormal.YZ == (float)GetChild(0).GetChild(9).GetChild<Slider>(0).Value;
		bool YWp = ProjectionNormal.YW == (float)GetChild(0).GetChild(10).GetChild<Slider>(0).Value;
		bool ZWp = ProjectionNormal.ZW == (float)GetChild(0).GetChild(11).GetChild<Slider>(0).Value;

		if (!(XY && XZ && XW && YZ && YW && ZW && XYp && XZp && XWp && YZp && YWp && ZWp))
			UpdateHyperCube();
	}
}
