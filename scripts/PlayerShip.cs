using Godot;

public partial class PlayerShip : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 400;
	[Export]
	public double inertia { get; set; } = 2;
	[Export]
	public PackedScene BulletScene { get; set; }

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

	public override void _Ready()
	{
		screenSize = GetViewportRect().Size;
	}

	public override void _PhysicsProcess(double delta)
	{
		getInput(delta);
		shoot();
	}
}