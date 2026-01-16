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
          	SetJson();
	}

	private void SetJson()
	{
		string musicPath = ToolsInit.FindInitJsonType("end", "music", "stream");
		float musicVolumeDb = float.Parse(ToolsInit.FindInitJsonType("end", "music", "stream"));
		_musicNode.Stream = Tools.LoadAudio($"./sounds/{musicPath}");
		_musicNode.VolumeDb = musicVolumeDb;
		_musicNode.Play();
	}

	public override void _Process(double delta)
	{
	}
}
