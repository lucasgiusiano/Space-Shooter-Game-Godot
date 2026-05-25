using Godot;

public partial class PlayerShip : CharacterBody2D
{
	[Signal]
	public delegate void PlayerHealthChangedEventHandler(int health);
	[Signal]
	public delegate void PlayerDiedEventHandler();
	[Export]
	public PackedScene BulletScene { get; set; }
	[Export]
	public PackedScene ExplosionScene { get; set; }
	[Export]
	public int Speed { get; set; } = 400;
	[Export]
	public double inertia { get; set; } = 2;

	[Export]
	public int Health { get; set; } = 3;

	private Vector2 screenSize;

	private void getInput(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = Velocity.Lerp(direction * Speed, (float)(inertia * delta));
		MoveAndSlide();

		LookAt(GetGlobalMousePosition());

		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, screenSize.X - 20),
			y: Mathf.Clamp(Position.Y, 0, screenSize.Y - 20)
		);

	}

	private void shoot()
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			Bullet bullet = BulletScene.Instantiate<Bullet>();
			bullet.Rotation = Rotation + Mathf.Pi / 2;
			bullet.Position = Position;
			bullet.targetDirection = (GetGlobalMousePosition() - Position).Normalized();
			GetParent().AddChild(bullet);
		}
	}

	public void subtractHealth(int amount)
	{
		Health -= amount;

		EmitSignal(SignalName.PlayerHealthChanged, Health);

		if (Health <= 0)
		{
			EmitSignal(SignalName.PlayerDied);
			Explotion explotion = ExplosionScene.Instantiate<Explotion>();
			explotion.Position = Position;
			GetParent().AddChild(explotion);
			QueueFree();
		}
	}
	public override void _Ready()
	{
		EmitSignal(SignalName.PlayerHealthChanged, Health);
		screenSize = GetViewportRect().Size;
	}

	public override void _PhysicsProcess(double delta)
	{
		getInput(delta);
		shoot();
	}
}