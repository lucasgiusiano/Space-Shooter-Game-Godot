using Godot;
using System;

public partial class Bullet : Area2D
{

	[Export]
	public int Speed { get; set; } = 500;

	public Vector2 targetDirection { get; set; }

	[Export] public bool IsPlayerBullet { get; set; } = false;

	public void onHit(Node body)
	{
		if (body is EnemieShip)
		{
			(body as EnemieShip).subtractHealth(1);
		}
		else if (body is PlayerShip)
		{
			(body as PlayerShip).subtractHealth(1);
		}
		QueueFree();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += targetDirection * Speed * (float)delta;
	}
}

