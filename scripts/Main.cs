using Godot;
using System;

public partial class Main : Node
{

	[Export] public PackedScene EnemyLightScene { get; set; }
	[Export] public PackedScene EnemyMediumScene { get; set; }
	[Export] public PackedScene EnemyHeavyScene { get; set; }
	[Export] public float EnemyLightSpawnRate { get; set; } = 2.0f;
	[Export] public float EnemyMediumSpawnRate { get; set; } = 3.0f;
	[Export] public float EnemyHeavySpawnRate { get; set; } = 4.0f;

	private Random random = new Random();
	private Vector2 screenSize;

	private void SpawnEnemy(int enemyType)
	{
		int border = random.Next(0, 4);
		PackedScene enemyScene = EnemyLightScene;
		screenSize = GetViewport().GetVisibleRect().Size;
		Vector2 spawnPosition = Vector2.Zero;

		SelectEnemyType(enemyType, out enemyScene);

		SelectBorder(border, out spawnPosition);

		Node2D enemyInstance = enemyScene.Instantiate<Node2D>();
		enemyInstance.Position = spawnPosition;
		AddChild(enemyInstance);
	}

	private void SelectEnemyType(int enemyType, out PackedScene enemyScene)
	{
		switch (enemyType)
		{
			case 0:
				enemyScene = EnemyLightScene;
				break;
			case 1:
				enemyScene = EnemyMediumScene;
				break;
			case 2:
				enemyScene = EnemyHeavyScene;
				break;
			default:
				enemyScene = EnemyLightScene;
				break;
		}
	}

	private void SelectBorder(int border, out Vector2 spawnPosition)
	{
		switch (border)
		{
			case 0:
				spawnPosition = new Vector2(random.Next(0, (int)screenSize.X), -50);
				break;
			case 1:
				spawnPosition = new Vector2(random.Next(0, (int)screenSize.X), (int)screenSize.Y + 50);
				break;
			case 2:
				spawnPosition = new Vector2(-50, random.Next(0, (int)screenSize.Y));
				break;
			case 3:
				spawnPosition = new Vector2((int)screenSize.X + 50, random.Next(0, (int)screenSize.Y));
				break;
			default:
				spawnPosition = new Vector2(random.Next(0, (int)screenSize.X), -50);
				break;
		}
	}

	private void SetTimers()
	{
		GetNode<Timer>("TimerLight").WaitTime = EnemyLightSpawnRate;
		GetNode<Timer>("TimerMedium").WaitTime = EnemyMediumSpawnRate;
		GetNode<Timer>("TimerHeavy").WaitTime = EnemyHeavySpawnRate;

		GetNode<Timer>("TimerLight").Timeout += () => SpawnEnemy(0);
		GetNode<Timer>("TimerMedium").Timeout += () => SpawnEnemy(1);
		GetNode<Timer>("TimerHeavy").Timeout += () => SpawnEnemy(2);

		GetNode<Timer>("TimerLight").Start();
		GetNode<Timer>("TimerMedium").Start();
		GetNode<Timer>("TimerHeavy").Start();
	}

	public void NewGame()
	{
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetTimers();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
