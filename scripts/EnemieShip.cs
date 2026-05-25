using Godot;
using System;

public partial class EnemieShip : CharacterBody2D
{
	[Signal]
	public delegate void EnemyDiedEventHandler();
	[Export]
	public PackedScene BulletScene { get; set; }
	[Export]
	public PackedScene ExplosionScene { get; set; }
	[Export]
	public float Speed = 0;
	[Export]
	public float FireRate = 0;
	[Export]
	public int Health = 0;
	private float _shootTimer = 0;



	public void trackPlayer(PlayerShip player)
	{

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
			EmitSignal(SignalName.EnemyDied);
			Explotion explotion = ExplosionScene.Instantiate<Explotion>();
			explotion.Position = Position;
			GetParent().AddChild(explotion);
			QueueFree();
		}
	}

	private void autoShoot(PlayerShip player, double delta)
	{
		_shootTimer += (float)delta;
		if (_shootTimer >= 1.0f / FireRate)
		{
			_shootTimer = 0;
			Bullet bullet = BulletScene.Instantiate<Bullet>();
			bullet.Rotation = Rotation + Mathf.Pi / 2;
			bullet.Position = Position;
			bullet.targetDirection = (player.Position - Position).Normalized();
			GetParent().AddChild(bullet);
		}
	}

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		PlayerShip player = GetTree().GetFirstNodeInGroup("Player") as PlayerShip;

		trackPlayer(player);
		autoShoot(player, delta);
	}
}
