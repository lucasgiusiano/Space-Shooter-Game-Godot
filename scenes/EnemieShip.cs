using Godot;
using System;

public partial class EnemieShip : CharacterBody2D
{
	public float Speed = 0;
	public int FireRate = 0;
	public int Health = 0;

	public void trackPlayer()
	{
		PlayerShip player = GetTree().GetFirstNodeInGroup("Player") as PlayerShip;
		if (player != null)
		{
			LookAt(player.Position);
			Velocity = (player.Position - Position).Normalized() * Speed;
			MoveAndSlide();
		}
	}

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		trackPlayer();
	}
}
