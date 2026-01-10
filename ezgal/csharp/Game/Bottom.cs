using Godot;
using System;

public partial class Bottom : Control
{
	Tween tween;
	[Export]
	ColorRect dialog { get; set; }
	[Export]
	RichTextLabel text { get; set; }
	[Export]
	Label name { get; set; }
	[Export]
	private AudioStreamPlayer _soundsNode;
	[Export]
	private Keys _keysScene;

	[Signal]
	public delegate void StartGameEventHandler();

	public override void _Ready()
	{
		Hide();
	}

	// 重写隐藏函数
	public new void Hide()
	{
		dialog.Size = new Vector2(Global.window_width, 0);
		base.Hide();
	}

	// 重写显示函数
	public new void Show()
	{
		if (dialog.Size == new Vector2(Global.window_width, 0))
		{
			tween = GetTree().CreateTween();
			tween.TweenProperty(dialog, "size", new Vector2(Global.window_width, 292), 0.3f);
		}
		base.Show();
	}

	// 添加语言文本
	public void SetText(string text_data)
	{ 
		text.Text = $"{text_data} »";
		text.VisibleRatio = 0.0f;
		tween = GetTree().CreateTween();
		tween.Finished += OnTweenFinished;
		_soundsNode.Play();
		tween.TweenProperty(text, "visible_ratio", 1.0f, Tools.RemoveBBCode(text_data).Length * Global.text_speed);
	}

	// 添加角色名
	public new void SetName(string name_data)
	{
		name.Text = name_data;
	}

	// 鼠标点击事件(左键)
	public void _on_dialog_gui_input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && Global.KeysState == null)
		{
			EmitSignal(nameof(StartGame));
		}
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

	public void OnTweenFinished()
	{
		_soundsNode.Stop();
	}
}
