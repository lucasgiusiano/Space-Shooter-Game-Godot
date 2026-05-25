using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Export] public Texture2D FullHeartTexture { get; set; }
	[Export] public Texture2D EmptyHeartTexture { get; set; }
	private int maxHealth = -1;
	private Vector2 screenSize;

	public void UpdateScore(int score)
	{
		Label scoreLabel = GetNode<Label>("ScoreLabel");
		scoreLabel.Text = $"Score: {score}";
	}


	public void UpdateHealth(int health)
	{
		for (int i = 0; i < maxHealth; i++)
		{
			Sprite2D heart = GetNode<Sprite2D>($"Heart{i + 1}");
			heart.Texture = i < health ? FullHeartTexture : EmptyHeartTexture;
		}
	}

	public void InitHearts(int health)
	{
		maxHealth = health;
		float spacing = 32;
		float totalWidth = (maxHealth - 1) * spacing;
		float startX = (screenSize.X / 2) - (totalWidth / 2);

		for (int i = 0; i < maxHealth; i++)
		{
			Sprite2D heart = new Sprite2D();
			heart.Name = $"Heart{i + 1}";
			heart.Texture = FullHeartTexture;
			heart.Scale = new Vector2(0.5f, 0.5f);
			heart.Position = new Vector2(startX + (i * spacing), 30);
			AddChild(heart);
		}
	}

	private void initiateHUDValues()
	{
		UpdateScore(0);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		screenSize = GetViewport().GetVisibleRect().Size;
		initiateHUDValues();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
