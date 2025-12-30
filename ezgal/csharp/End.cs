using Godot;
using System;

public partial class End : Control
{
	[Export]
	private AudioStreamPlayer _musicNode;
	[Export]
	private Sprite2D _endTextureNode;

	public override void _Ready()
	{
		Tools.SetTexture(_endTextureNode, "end_texture");
	}

	public override void _Process(double delta)
	{
	}
}
