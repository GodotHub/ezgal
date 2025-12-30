using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Control
{
	// 背景图片
	[Export]
	public TextureRect Background { get; set; }
	// 对话框文本
	[Export]
	public Bottom Bottom { get; set; }
	// 全屏对话文本
	[Export]
	public Font Font { get; set; }
	// 设置选项节点
	[Export]
	public Options Options { get; set; }
	[Export(PropertyHint.FilePath)]
	public string EndScenePath { get; set; }
	// 剧情数据
	public List<Global.Flow> Datas { get; set; }
	// 立绘状态控制字典
	private Dictionary<string, Sprite2D> dicNode = new Dictionary<string, Sprite2D>();


	public override void _Ready()
	{
		Datas = new List<Global.Flow>();
		Datas.AddRange(Global.datas);
		Run();
	}

	// 设置背景图片
	private void InitBackground(string path)
	{
		var texture = ResourceLoader.Load($"./image/background/{path}") as Texture2D;
		if (texture == null)
		{
			GD.PrintErr($"`./image/background/` Failed to load `{path}`.");
		}
		else
		{
			Background.Texture = texture;
		}
	}

	public void End()
	{
		GetTree().ChangeSceneToFile(EndScenePath);
	}

	// 获取字段进行统一管理
	public void Run()
	{
		bool is_first = true;
		if (Datas.Count <= Global.intptr)
		{
			End();
			return;
		}

		Global.Flow data = Datas[Global.intptr];

		// 空数据直接跳过
		while (Object.Equals(data, new Global.Flow()))
		{
			Global.intptr++;
			if (Datas.Count <= Global.intptr)
			{
				End();
				return;
			}
			data = Datas[Global.intptr];
		}

		if (data.type == null)
		{
			data.type = Global.typeptr;
			Datas[Global.intptr] = data;
			is_first = false;
		}

		// 类型判断与处理
		switch (data.type)
		{
			case FlowData.background:
				// 设置背景图片
				InitBackground(data.text);
				Global.intptr++;
				Run();
				break;
			case FlowData.dialogue:
				// 对话框更新状态
				Global.typeptr = data.type;
				if (is_first)
				{
					Font.Hide();
					Bottom.Show();
				}
				Bottom.SetText(data.text);
				if (data.name != null)
				{
					Bottom.SetName(data.name);
				}
				if (data.anima.type != null)
				{
					create_anima(data.anima);
				}
				Global.intptr++;
				break;
			case FlowData.fullscreen:
				// 全屏更新状态
				Global.typeptr = data.type;
				if (is_first)
				{
					Font.Show();
					Bottom.Hide();
					Font.SetText(data.text, false);
				}
				else
				{
					Font.SetText(data.text, true);
				}
				if (data.anima.type != null)
				{
					create_anima(data.anima);
				}
				Global.intptr++;
				break;
			case FlowData.options:
				Font.Hide();
				Bottom.Hide();
				Options.SetOption(data.option);
				break;
			case FlowData.option:
				Global.intptr++;
				break;
			default:
				GD.Print("非预期");
				Global.intptr++;
				break;
		}

		// 跳转
		if (data.script != null || data.jump != null)
		{
			new_script(data.script, data.jump, data.type);
		}
	}

	// 跳转到新的脚本
	public void new_script(string file_name, string jump_ptr, string type)
	{
		if (file_name == null)
		{
			file_name = Global.read_file_name;
		}

		Datas = Datas.GetRange(0, Global.intptr);
		List<Global.Flow> new_datas = Global.RunFile(file_name);

		if (jump_ptr == null)
		{
			Datas.AddRange(new_datas);
		}
		else
		{
			string data_type = null;
			List<string> type_list = new List<string> { FlowData.fullscreen, FlowData.dialogue };
			for (int i = 0; i < new_datas.Count; i++)
			{
				if (type_list.Contains(new_datas[i].type))
				{
					data_type = new_datas[i].type;
				}
				int new_datas_count = new_datas.Count;
				if (new_datas[i].jptr == jump_ptr)
				{
					new_datas = new_datas.GetRange(i, new_datas_count - i);
					Global.Flow new_data = new_datas[0];
					new_data.type = data_type;
					new_datas[0] = new_data;
					Datas.AddRange(new_datas);
					break;
				}
			}
		}
		if (type == FlowData.option)
		{
			Run();
		}
	}

	// 创建立绘节点
	public void create_anima(Global.Anima anima)
	{
		//DicNode.Add("Node1Key", node1);
		Sprite2D node;
		if (dicNode.TryGetValue("Node1Key", out node))
		{
			node.Texture = (Texture2D)ResourceLoader.Load($"res://image/{anima.type}/{anima.name}");
			node.Position = anima.position;
			node.Scale = new Vector2(anima.scale, anima.scale);
		}
		else
		{
			PackedScene scene = (PackedScene)ResourceLoader.Load("res://scene/anima.tscn");
			node = (Sprite2D)scene.Instantiate();
			AddChild(node);
			//node.Texture = (Texture2D)ResourceLoader.Load($"./image/{anima.type}/{anima.name}");
			string imagePath = $"res://image/{anima.type}/{anima.name}";
			Texture2D texture = (Texture2D)ResourceLoader.Load(imagePath);

			if (texture != null)
			{
				node.Texture = texture;
			}
			else
			{
				GD.PrintErr($"not found image: {imagePath}");
			}
			node.Position = anima.position;
			node.Scale = new Vector2(anima.scale, anima.scale);
			dicNode.Add(anima.type, node);
		}

	}

	// 对话框信号
	public void _on_bottom_start_game()
	{
		Run();
	}

	// 全屏信号
	public void _on_font_start_game()
	{
		Run();
	}
}
