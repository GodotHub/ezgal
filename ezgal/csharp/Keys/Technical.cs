using Godot;
using System;
using System.IO;

public partial class Technical : Control
{
	[Export]
	private Keys _keysScene;
	[Export]
	private VBoxContainer _vBoxContainerNode;
	[Export]
	public RichTextLabel TextNode { get; set; }

	private Keys _self;

	public override void _Ready()
	{
		SetTechDataName();
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

	public void SetSelf(Keys keysScene)
	{
		this._self = keysScene;
	}

	public void _on_text_meta_clicked(Variant meta)
	{
		Global.LoadTechnical(_self, meta);
	}

	public void OnButtonPressed(string text)
	{
		Global.KeysState = null;
		Global.LoadTechnical(_self, text);
	}

	private void SetTechDataName()
	{
		if (FlowData.Techdata.Count == 0)
		{
			string[] files = Directory.GetFiles("./technical/");
			foreach (string file in files)
			{
				SetLabel(file);
			}
			return;
		}

		foreach (FlowData.TechData data in FlowData.Techdata)
		{
			SetLabel(data.file);
		}
	}

	private void SetLabel(string name)
	{
		Button buttonNode = new Button();
		buttonNode.Text = Path.GetFileNameWithoutExtension(name);
		_vBoxContainerNode.AddChild(buttonNode);
		buttonNode.Pressed += () => OnButtonPressed(buttonNode.Text);
	}
}
