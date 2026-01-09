using Godot;
using System;

public partial class Font : Control
{
	Tween tween;
	[Export]
	RichTextLabel TextNode { get; set; }
	[Export]
	private AudioStreamPlayer _soundsNode;
	[Export]
	private Keys _keysScene;

	[Signal]
	public delegate void StartGameEventHandler();

	public override void _Ready()
	{
		tween = GetTree().CreateTween();
		Hide();
	}
	
	/* 隐藏对话框
	public new void Hide()
	{
		base.Hide();
	}

	// 显示对话框
	public new void Show()
	{
		base.Show();
	}
	*/

	// 设置对话框文本
	public void SetText(string text_data, bool add_data)
	{
		if ( tween.IsRunning() )
		{
			tween.Kill();
			_soundsNode.Stop();
			TextNode.VisibleCharacters = TextNode.Text.Length;

		}
		if (add_data && TextNode.Text.Split("\n").Length < 12)
		{
			if (TextNode.Text.EndsWith(" »"))
			{
				TextNode.Text = TextNode.Text.Replace(" »", "\n");
			}
			TextNode.Text += $"{text_data} »";
		}
		else
		{
			TextNode.VisibleCharacters = 0;
			TextNode.Text = $"{text_data} »";
		};

		tween = GetTree().CreateTween();
		_soundsNode.Play();
		tween.Finished += OnTweenFinished;
		tween.TweenProperty(
				TextNode,
				"visible_characters", 
				Tools.RemoveBBCode(TextNode.Text).Length,
				Tools.RemoveBBCode(text_data).Length * Global.text_speed
				);
	}

	// 鼠标点击事件
	public void _on_text_gui_input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && Global.KeysState == null)
		{
			EmitSignal(nameof(StartGame));
		}
	}

	// 专业词汇文本事件
	public void _on_text_meta_hover_ended(Variant meta)
	{
		Global.KeysState = null;
	}

	public void _on_text_meta_hover_started(Variant meta)
	{
		Global.KeysState = "Touch";
	}

	// 跳转到专业词汇文本事件
	public void _on_text_meta_clicked(Variant meta)
	{
		Global.LoadTechnical(_keysScene, meta);
	}

	// audio: tween finish
	public void OnTweenFinished()
	{
		_soundsNode.Stop();
	}

}
