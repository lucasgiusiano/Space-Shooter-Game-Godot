using Godot;
using System;

public partial class EnemieShip : CharacterBody2D
{
	[Export]
	public PackedScene ExplosionScene { get; set; }
	[Export]
	public float Speed = 0;
	[Export]
	public int FireRate = 0;
	[Export]
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

	public void subtractHealth(int amount)
	{
		Health -= amount;
		if (Health <= 0)
		{
			Explotion explotion = ExplosionScene.Instantiate<Explotion>();
			explotion.Position = Position;
			GetParent().AddChild(explotion);
			QueueFree();
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
