using Godot;
using System;
using System.Collections.Generic;

public partial class Options : Control
{
	[Export]
	private PackedScene _optionScene;
	//PackedScene option_tscn = GD.Load<PackedScene>("res://scene/option.tscn");
	//static string[] init_options;

	public override void _Ready()
	{
		Hide();
	}

	// 隐藏选项
	public new void Hide()
	{
		foreach (Button child in GetChildren())
		{
			child.QueueFree();
		}
		base.Hide();
	}

	// 显示选项
	public new void Show()
	{
		base.Show();
	}

	public void SetOption(List<string> options)
	{
		Show();
		Option button = _optionScene.Instantiate<Option>();
		int space = 80;
		int button_height = options.Count * (int)button.Size[1] + (options.Count - 1) * space;
		int option_y = (Global.window_height - button_height) / 2;
		foreach (string option in options)
		{
			Option new_button = _optionScene.Instantiate<Option>();
			string new_option = option;
			new_button.set_option = option;
			if (option.Contains("}"))
			{
				new_option = option.Split("}")[1].Trim();
			}
			new_button.Text = new_option;
			new_button.Position = new Vector2((Global.window_width - button.Size[0]) / 2, option_y);
			AddChild(new_button);
			option_y += space + (int)button.Size[1];
		}
	}
}
