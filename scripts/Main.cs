using Godot;
using System;

public partial class Main : Node
{

	[ExportGroup("Enemy Scenes")]
	[Export] public PackedScene EnemyLightScene { get; set; }
	[Export] public PackedScene EnemyMediumScene { get; set; }
	[Export] public PackedScene EnemyHeavyScene { get; set; }

	[ExportGroup("Difficulty - Spawn Rates")]
	/// <summary>Segundos entre cada spawn de enemigo ligero. Menor = más frecuente.</summary>
	[Export] public float EnemyLightSpawnRate { get; set; } = 2.0f;
	/// <summary>Segundos entre cada spawn de enemigo medio. Menor = más frecuente.</summary>
	[Export] public float EnemyMediumSpawnRate { get; set; } = 3.0f;
	/// <summary>Segundos entre cada spawn de enemigo pesado. Menor = más frecuente.</summary>
	[Export] public float EnemyHeavySpawnRate { get; set; } = 4.0f;

	[ExportGroup("Difficulty - Level Thresholds")]
	/// <summary>Score necesario para pasar al nivel 2. Sube para hacerlo más difícil de alcanzar.</summary>
	[Export] public int Level2Threshold { get; set; } = 50;
	/// <summary>Score necesario para pasar al nivel 3. Sube para hacerlo más difícil de alcanzar.</summary>
	[Export] public int Level3Threshold { get; set; } = 150;

	[ExportGroup("Music")]
	/// <summary>Música del nivel 1 (score 0 a Level2Threshold).</summary>
	[Export] public AudioStream Music1 { get; set; }
	/// <summary>Música del nivel 2 (score Level2Threshold a Level3Threshold).</summary>
	[Export] public AudioStream Music2 { get; set; }
	/// <summary>Música del nivel 3 (score mayor a Level3Threshold).</summary>
	[Export] public AudioStream Music3 { get; set; }
	/// <summary>Sonido que se reproduce al pasar de nivel.</summary>
	[Export] public AudioStream LevelUpSound { get; set; }
	private int score = 0;
	private int currentMusicLevel = 0;
	private Random random = new Random();
	private Vector2 screenSize;
	private void SpawnLightEnemy() => SpawnEnemy(0);
	private void SpawnMediumEnemy() => SpawnEnemy(1);
	private void SpawnHeavyEnemy() => SpawnEnemy(2);
	private float baseMusicVolume = -30f;

	private void SpawnEnemy(int enemyType)
	{
		int border = random.Next(0, 4);
		PackedScene enemyScene = EnemyLightScene;
		screenSize = GetViewport().GetVisibleRect().Size;
		Vector2 spawnPosition = Vector2.Zero;

		SelectEnemyType(enemyType, out enemyScene);

		SelectBorder(border, out spawnPosition);

		Node2D enemyInstance = enemyScene.Instantiate<Node2D>();
		(enemyInstance as EnemieShip).EnemyDied += HandleScoreChange;
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

	private void InitializeTimers()
	{
		GetNode<Timer>("TimerLight").WaitTime = EnemyLightSpawnRate;
		GetNode<Timer>("TimerMedium").WaitTime = EnemyMediumSpawnRate;
		GetNode<Timer>("TimerHeavy").WaitTime = EnemyHeavySpawnRate;

		GetNode<Timer>("TimerLight").Timeout += SpawnLightEnemy;
		GetNode<Timer>("TimerMedium").Timeout += SpawnMediumEnemy;
		GetNode<Timer>("TimerHeavy").Timeout += SpawnHeavyEnemy;

		GetNode<Timer>("TimerLight").Start();
		GetNode<Timer>("TimerMedium").Start();
		GetNode<Timer>("TimerHeavy").Start();
	}

	private void StopTimers()
	{
		GetNode<Timer>("TimerLight").Stop();
		GetNode<Timer>("TimerMedium").Stop();
		GetNode<Timer>("TimerHeavy").Stop();

		GetNode<Timer>("TimerLight").Timeout -= SpawnLightEnemy;
		GetNode<Timer>("TimerMedium").Timeout -= SpawnMediumEnemy;
		GetNode<Timer>("TimerHeavy").Timeout -= SpawnHeavyEnemy;
	}

	private async void HandleMusic(AudioStream targetMusic)
	{
		AudioStreamPlayer2D gameMusic = GetNode<AudioStreamPlayer2D>("GameMusic");
		if (gameMusic.Stream == targetMusic) return;

		Tween tween = CreateTween();
		tween.TweenProperty(gameMusic, "volume_db", -80f, 0.5f);
		await ToSignal(tween, "finished");

		gameMusic.Stream = targetMusic;
		gameMusic.Play();

		tween = CreateTween();
		tween.TweenProperty(gameMusic, "volume_db", baseMusicVolume, 0.5f); // volvés al volumen original
	}

	private void HandleLevelUpSound()
	{
		AudioStreamPlayer2D levelUpSound = GetNode<AudioStreamPlayer2D>("LevelUpSound");
		levelUpSound.Stream = LevelUpSound;
		levelUpSound.Play();
	}

	public void HandleHeartsChange(int health)
	{
		GetNode<HUD>("HUD").UpdateHealth(health);
	}

	public void HandleScoreChange()
	{
		score += 1;
		GetNode<HUD>("HUD").UpdateScore(score);

		int newLevel = score < Level2Threshold ? 0 : score < Level3Threshold ? 1 : 2;
		AudioStream target = newLevel == 0 ? Music1 : newLevel == 1 ? Music2 : Music3;

		if (newLevel != currentMusicLevel)
		{
			currentMusicLevel = newLevel;
			HandleLevelUpSound();
		}

		HandleMusic(target);
	}

	public void NewGame()
	{
		GetNode<PlayerShip>("PlayerShip").Show();
		GetNode<CanvasLayer>("StartScreen").Hide();
		GetNode<CanvasLayer>("HUD").Show();
		PlayerShip player = GetNode<PlayerShip>("PlayerShip");
		GetNode<HUD>("HUD").InitHearts(player.Health);

		GetNode<CanvasLayer>("StartScreen").GetNode<AudioStreamPlayer2D>("AudioIntro").Stop();

		AudioStreamPlayer2D gameMusic = GetNode<AudioStreamPlayer2D>("GameMusic");
		gameMusic.Stream = Music1;
		gameMusic.Play();

		InitializeTimers();
	}

	public void GameOver()
	{
		GetNode<AudioStreamPlayer2D>("GameMusic").Stop();

		GetNode<CanvasLayer>("HUD").Hide();
		GetNode<PlayerShip>("PlayerShip").Hide();

		GetNode<GameOverScreen>("GameOverScreen").Show();
		GetNode<GameOverScreen>("GameOverScreen").SetMaxScoreReached(score);

		StopTimers();

		GetNode<GameOverScreen>("GameOverScreen").GetNode<AudioStreamPlayer2D>("GameOverAudio").Play();
		GetNode<GameOverScreen>("GameOverScreen").GetNode<AudioStreamPlayer2D>("GameOverMusic").Play();
	}

	public void OnRetryButtonPressed()
	{
		GetTree().ReloadCurrentScene();
	}

	public void OnMenuButtonPressed()
	{
		GetTree().ReloadCurrentScene();
	}

	public void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<CanvasLayer>("HUD").Hide();
		GetNode<GameOverScreen>("GameOverScreen").Hide();
		GetNode<CanvasLayer>("StartScreen").Show();
		GetNode<PlayerShip>("PlayerShip").Hide();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
