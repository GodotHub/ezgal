using Godot;
using System;

public partial class Dictionary : Control
{
	[Export]
	public RichTextLabel TextNode { get; set; }
	private Dictionary _self;

	public override void _Ready()
	{
		Hide();
	}

	// 隐藏对话框
	public new void Hide()
	{
		TextNode.Text = "";
		base.Hide();
	}

	/* 显示对话框
	public new void Show()
	{
		base.Show();
	}
	*/

	public void SetSelf(Dictionary dictionaryScene)
	{
		this._self = dictionaryScene;
	}

	public void _on_text_meta_clicked(Variant meta)
	{
		Global.LoadDictionary(_self, meta);
	}

	public void _on_button_pressed()
	{
		Hide();
	}
}
