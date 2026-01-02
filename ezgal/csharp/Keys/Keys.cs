using Godot;
using System;

public partial class Keys : Control
{
	[Export]
	public Control TechnicalScene;
	[Export]
	private Game _gameScene;
	[Export]
	private Button _historyNode;
	[Export]
	private Button _settingsNode;
	[Export]
	private BoxContainer _boxContainerNode;
	
	private static ColorRect _backgroundNode;
	private static History _historyScene;
	
	public override void _Ready()
	{
		_backgroundNode = GetNode<ColorRect>("Background");
		_historyScene = GetNode<History>("Background/History");
		_historyNode.Pressed += OnHistoryPressed;

		_boxContainerNode.Position = new Vector2(
					Global.window_width - 30 - _boxContainerNode.Size.X,
					30
				);
	}

	public void OnHistoryPressed()
	{
		BackgroundPressed("History");
	}

	public void BackgroundPressed(string scene)
	{	
		foreach (Control child in _backgroundNode.GetChildren())
		{
			child.Hide();
		}
		if ( Global.KeysState == scene )
		{
			_backgroundNode.Hide();
			Global.KeysState = null;
		}
		else
		{
			_backgroundNode.Show();
			switch (scene)
			{
				case "History":
					_historyScene.Show();
					_historyScene.Text = _historyScene.loadFlowText(_gameScene.Datas);
					break;
				case "Technical":
					TechnicalScene.Show();
					break;
			}
			Global.KeysState = scene;
		}
	}
}
