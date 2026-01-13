using Godot;
using System;
using System.Collections.Generic;

public partial class History : RichTextLabel
{
	[Export]
	private Keys _keysScene;
	private int left = 30;
	private int top = 60;
	private string type;

	public override void _Ready()
	{
		MetaClicked += OnMetaClicked;
		Position = new Vector2(
				left, top
				);
		Size = new Vector2(
				Global.window_width - (2 * left),
				Global.window_height - (2 * top)
				);
	}

	public void OnMetaClicked(Variant meta)
	{
		Global.LoadTechnical(_keysScene, meta);
	}

	// load Datas's text.
	/* todo: 
	/* 	1.应该优化一下剧本实现模式，以章节进行区分，不然实现多个剧本时容易撑爆历史记录.
	 *  	2.应该设置类似终端的输出的形式，保障每次读取只是GetRange(x,intptr)行Flow,有需要再增加.
	 *  	3.可能的首行flow.type==null会是最麻烦的地方.
	 *  	4.Flow中bg需要重新设置，避免选项被覆盖.
	 */
	public string loadFlowText(List<Global.Flow> datas)
	{
		List<Global.Flow> flows = datas.GetRange(0, Global.intptr);
		string result = "";
		string type = "";
		foreach (Global.Flow flow in flows)
		{
			type = flow.type ?? type;
			switch (type)
			{
				case FlowData.dialogue:
					result += $"{flow.name}    |  {flow.text}\n";
					break;
				case FlowData.fullscreen:
					result += $"{flow.text}\n";
					break;
				case FlowData.option:
					result += $"►{flow.text}◄\n";
					break;
				default:
					break;
			}
		}
		return result;
	}
}
