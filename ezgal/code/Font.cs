using Godot;
using System;

public partial class Font : Control
{
	Tween tween;
	RichTextLabel text;
	Dictionary dic_node;

	[Signal]
	public delegate void StartGameEventHandler();

	public override void _Ready()
	{
		dic_node = GetNode<Dictionary>("dictionary");
		text = GetNode<RichTextLabel>("text");
		tween = GetTree().CreateTween();
		Hide();
	}
	
	// 隐藏对话框
	public new void Hide()
	{
		base.Hide();
	}

	// 显示对话框
	public new void Show()
	{
		base.Show();
	}

	// 设置对话框文本
	public void SetText(string text_data, bool add_data)
	{
		if ( tween.IsRunning() )
		{
			GD.Print("kill");
			tween.Kill();
			text.VisibleCharacters = text.Text.Length;

		}
		if (add_data && text.Text.Split("\n").Length < 12)
		{
			if (text.Text.EndsWith(" »"))
			{
				text.Text = text.Text.Replace(" »", "\n");
			}
			text.Text += $"{text_data} »";
		}
		else
		{
			text.VisibleCharacters = 0;
			text.Text = $"{text_data} »";
		};
		tween = GetTree().CreateTween();
		tween.TweenProperty(text, "visible_characters", text.Text.Length, text_data.Length * Global.text_speed);
	}

	// 鼠标点击事件
	public void _on_text_gui_input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (!Global.is_query)
			{
				EmitSignal(nameof(StartGame));
			}
		}
	}

	// 专业词汇文本事件
	public void _on_text_meta_hover_ended(Variant meta)
	{
		Global.is_query = false;
	}

	public void _on_text_meta_hover_started(Variant meta)
	{
		Global.is_query = true;
	}

	// 跳转到专业词汇文本事件
	public void _on_text_meta_clicked(Variant meta)
	{
		Global.LoadDictionary(dic_node, meta);
	}

}
