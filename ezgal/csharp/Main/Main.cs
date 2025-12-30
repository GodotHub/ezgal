using Godot;
using System;

public partial class Main : Node2D
{
	[Export]
	private Sprite2D _startTextureNode;
	[Export]
	private AudioStreamPlayer _sounds;
	[Export]
	private BoxContainer _boxContainer;

	public override void _Ready()
	{
		Tools.SetTexture(_startTextureNode,"start_texture");
		_boxContainer.MouseExited += OnBoxContainerMouseExited;
	}
	
	void OnBoxContainerMouseExited()
	{
		_sounds.Play();
	}
}
