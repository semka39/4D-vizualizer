using Godot;
using System;

public partial class Camera : Camera3D
{
	[Export]
	public Node3D Target;

	[Export]
	public float MouseSensitivity = 0.3f;

	[Export]
	public float PanSensitivity = 0.3f;

	[Export]
	public float Distance = 10.0f;

	[Export]
	public float ZoomSpeed = 1.0f;

	private Vector2 _rotation = new Vector2();
	private Vector2 _panOffset = Vector2.Zero;
	private bool _isPanning = false;
	private int VRotdirection = 1;

	public override void _Ready()
	{
		//Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			if (Input.IsActionPressed("mouse_rotating"))
			{
				_rotation.X += mouseMotion.Relative.Y * MouseSensitivity * VRotdirection;
				_rotation.Y -= mouseMotion.Relative.X * MouseSensitivity;

				_rotation.X = Mathf.Clamp(_rotation.X, -89, 89);

				//_isPanning = false;
				//_panOffset = Vector2.Zero;
			}

			if (Input.IsActionPressed("mouse_pan"))
			{
				_isPanning = true;
				Vector2 panMovement = new Vector2(-mouseMotion.Relative.X * PanSensitivity, mouseMotion.Relative.Y * PanSensitivity);
				_panOffset += panMovement;
			}
		}

		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.WheelUp)
			{
				Distance -= ZoomSpeed;
			}
			else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
			{
				Distance += ZoomSpeed;
			}
		}

		Distance = Mathf.Clamp(Distance, 1.0f, 100.0f);
	}

	public override void _Process(double delta)
	{
		UpdateCameraPosition();
	}

	private void UpdateCameraPosition()
	{
		if (Target != null)
		{
			Vector3 targetPosition = Target.GlobalPosition;

			if (_isPanning)
			{
				Vector3 forward = -GlobalTransform.Basis.Z.Normalized();
				Vector3 right = GlobalTransform.Basis.X.Normalized();
				Vector3 up = GlobalTransform.Basis.Y.Normalized();
				Vector3 panMovement = (right * _panOffset.X) + (up * _panOffset.Y);
				targetPosition += panMovement;
			}

			float radX = Mathf.DegToRad(_rotation.X);
			float radY = Mathf.DegToRad(_rotation.Y);

			Vector3 direction = new Vector3(
				Mathf.Sin(radY) * Mathf.Cos(radX),
				Mathf.Sin(radX),
				Mathf.Cos(radY) * Mathf.Cos(radX)
			).Normalized() * Distance;

			Vector3 cameraPosition = targetPosition + direction;

			if (cameraPosition.IsEqualApprox(targetPosition))
			{
				cameraPosition += new Vector3(0, 0, Distance);
			}

			if (!_isPanning)
			{
				GlobalTransform = new Transform3D(Basis.Identity, cameraPosition);
				LookAt(targetPosition, Vector3.Up);
			}
			else
			{
				GlobalTransform = new Transform3D(GlobalTransform.Basis, cameraPosition);
				LookAt(targetPosition, Vector3.Up);
			}
		}
	}
}
