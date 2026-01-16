using Godot;
using System;

public partial class Main : Node2D
{
	[Export]
	private Sprite2D _startTextureNode;
	[Export]
	private CpuParticles2D _cpuParticles2DNode;
	[Export]
	private AudioStreamPlayer _musicNode;
	[Export]
	private AudioStreamPlayer _soundsNode;
	[Export]
	private BoxContainer _boxContainer;

	[Export]
	private Label _titleNode;
	[Export]
	private Label _subTitleNode;

	public override void _Ready()
	{
		SetJson();
		Tools.SetTexture(_startTextureNode,"start_texture");
		_cpuParticles2DNode.Texture = Tools.LoadImage("./image/particle.png");
		_boxContainer.MouseExited += OnBoxContainerMouseExited;
	}

	private void SetJson()
	{
		string titleText = ToolsInit.FindInitJsonType("start", "title", "text");
		_titleNode.Text = titleText;
		string subTitleText = ToolsInit.FindInitJsonType("start", "subtitle", "text");
		_subTitleNode.Text = subTitleText;

		string musicPath = ToolsInit.FindInitJsonType("start", "music", "stream");
		float musicVolumeDb = float.Parse(ToolsInit.FindInitJsonType("start", "music", "volume_db"));
		_musicNode.Stream = Tools.LoadAudio($"./sounds/{musicPath}");
		_musicNode.VolumeDb = musicVolumeDb;
		_musicNode.Play();
	}
	
	void OnBoxContainerMouseExited()
	{
		_soundsNode.Play();
	}
}
