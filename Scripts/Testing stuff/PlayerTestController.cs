using Godot;
using System;

public partial class PlayerTestController : CharacterBody3D
{	
	[Export] private float _speed { get; set; } = 10;

    public override void _PhysicsProcess(double delta)  
	{
		Vector3 dir = Vector3.Zero;

		if(Input.IsActionPressed("move_right"))
		{
			dir.X += 10;
		}
		if(Input.IsActionPressed("move_left"))
		{
			dir.X -= 10;
		}
		if(Input.IsActionPressed("move_forward"))
		{
			dir.Z += 10;
		}
		if(Input.IsActionPressed("move_backward"))
		{
			dir.Z -= 10;
		}

		dir = dir.Normalized();

		Velocity = dir * _speed;
	}
}
