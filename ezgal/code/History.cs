using Godot;
using System;

public partial class History : Button
{
	[Export]
	public Game Game { get; set; }

	[Export]
	public RichTextLabel TextLabel { get; set; }

	[Export]
	public RichTextLabel HistoryTextLabel { get; set; }

	[Export]
	public Label NameLabel { get; set; }

	public override void _Toggled(bool toggledOn)
	{
		if (toggledOn)
		{
			TextLabel.Hide();
			HistoryTextLabel.Show();
			NameLabel.Hide();
			ClearHistory();
			LoadHistory();
		}
		else
		{
			HistoryTextLabel.Hide();
			TextLabel.Show();
			NameLabel.Show();
		}
	}
	
	private void LoadHistory()
	{
		var type = "";
		var datas = Game.Datas;
		for (int i = 0; i <= Global.intptr; i++)
		{
			var data = datas[i];
			type = data.type ?? type;
			if (type == FlowData.dialogue)
			{
				string text;
				if (data.name == null)
				{
					text = $"{data.text}";
				}
				else
				{
					text = $"{data.name}: {data.text}";
				}
				HistoryTextLabel.Text += text + "\n";
			}
		}
	}

	private void ClearHistory()
	{
		HistoryTextLabel.Text = "";
	}
}
