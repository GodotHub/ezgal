using Godot;
using System;

public partial class Option : Button
{
	private Game _gameScene;
	private Options _optionsScene;

	[Export]
	private Button _optionNode;
	public string set_option { get; set; }

	public override void _Ready()
	{
		_optionsScene = GetNode<Options>("../../Options");
		_gameScene = GetNode<Game>("../../../Game");
		_optionNode.Pressed += OnOptionPressed;
	}

	private void OnOptionPressed()
	{
		Global.Flow new_flow = Global.SetOptionBracesFunc(set_option);
		new_flow.text = _optionNode.Text;
		if (new_flow.type == FlowData.option) {
			_gameScene.Datas[Global.intptr + 1] = new_flow;
		}
		else {
			Global.intptr++;
		}
		Global.intptr++;
		_optionsScene.Hide();
		_gameScene.Run();
	}
}
