using Godot;
using System;

public partial class Main : Node
{

	[Export] public PackedScene EnemyLightScene { get; set; }
	[Export] public PackedScene EnemyMediumScene { get; set; }
	[Export] public PackedScene EnemyHeavyScene { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
