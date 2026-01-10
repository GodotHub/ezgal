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
		_musicNode.Stream = Tools.LoadAudio("./sounds/思念,交织于世界彼端.mp3");
		_musicNode.Play();
	}

	public override void _Process(double delta)
	{
	}
}
