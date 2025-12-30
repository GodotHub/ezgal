using Godot;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public partial class Global : Node
{
	// 屏幕宽度
	public static int window_width;
	// 屏幕高度
	public static int window_height;
	// text文本速度
	public static float text_speed = 0.06f;
	// 指针指向进度
	public static int intptr;
	// 指针指向类型
	public static string typeptr;
	// 程序控制文本
	public static List<Flow> datas;
	// 当前文本名称
	public static string read_file_name;
	// 专业词汇查询
	public static bool is_query = false;

	// 读取文件的处理工具.
	static string line;
	static Match match;
	static Flow flow_line;
	static List<Flow> new_datas;
	static List<string> set_option;

	public struct Flow
	{
		// 节点类型
		public string type;
		// 文本信息/参数
		public string text;
		// 说话对象
		public string name;
		// 立绘
		public Anima anima;
		// 选项
		public List<string> option;
		// 跳转脚本
		public string script;
		// 跳转位置
		public string jump;
		// 跳转标记
		public string jptr;
	}

	public struct Anima
	{
		// 立绘类型
		public string type;
		// 立绘名字
		public string name;
		// 位置
		public Vector2 position;
		// 缩放
		public float scale;
		// 状态
		public string state;
	}

	public override void _Ready()
	{
		// window size是否会改变
		window_width = GetWindow().Size.X;
		window_height = GetWindow().Size.Y;
		datas = RunFile("start.txt");
	}

	/*
	 * 这是检查代码用的，别改了，敲你喵！
	 * This is for code inspection. Don't modify it, damn it!
	 * これはコードチェック用なんだから、変更しないでくれよ、クソが！
	 *
	 public static void print(Flow new_data)
	 {
	 GD.Print($"Round ptr:{intptr}");
	 GD.Print($"type		:{new_data.type}");
	 GD.Print($"text		:{new_data.text}");
	 GD.Print($"name		:{new_data.name}");
	 GD.Print($"script	:{new_data.script}");
	 GD.Print($"jump		:{new_data.jump}");
	 GD.Print($"jptr		:{new_data.jptr}");
	 GD.Print($"Anima:");
	 GD.Print($"type		:{new_data.anima.type}");
	 GD.Print($"name		:{new_data.anima.name}");
	 GD.Print($"position	:{new_data.anima.position}");
	 GD.Print($"scale	:{new_data.anima.scale}");
	 GD.Print($"state	:{new_data.anima.state}");
	 GD.Print("----------------------");
	 }
	 */

	public static bool IsEmpty<T>(T structure) where T : struct
	{
		return EqualityComparer<T>.Default.Equals(structure, default(T));
	}

	// 读取data并转义为json格式.
	public static List<Flow> RunFile(string path)
	{
		if (FlowData.flowdata.Count == 0)
		{
			try {
				return ReadFile($"./script/{path}");
			}
			catch
			{
				GD.PrintErr($"Not found Path: ./script/{path}");
				return new List<Flow>();
			}
		}
		return FlowData.flowdata.FirstOrDefault(x => x.file == path).data;
	}

	public static void LoadDictionary(Dictionary dictionaryScene, Variant fileName)
	{
		dictionaryScene.Show();
		dictionaryScene.SetSelf(dictionaryScene);
		if (FlowData.dicdata.Count == 0)
		{
			try
			{
				using (StreamReader reader = new StreamReader($"./dictionary/{fileName}.txt"))
				{
					string textdata = reader.ReadToEnd();
					dictionaryScene.TextNode.Text = textdata;
				}
			}
			catch
			{
				dictionaryScene.TextNode.Text = "无法加载字典内容";
			}
		}
		else
		{
			try
			{
				string textdata = FlowData.dicdata.FirstOrDefault(x => x.file == $"{fileName}.txt").data;
				dictionaryScene.TextNode.Text = textdata;
			}
			catch
			{
				dictionaryScene.TextNode.Text = "无法加载字典内容";
			}
		}
	}

	// 读取文本文档转义为json格式.
	static List<Flow> ReadFile(string path)
	{
		read_file_name = path;
		flow_line = new Flow{};
		using (StreamReader reader = new StreamReader(path))
		{
			new_datas = new List<Flow>();
			while ((line = reader.ReadLine()) != null)
			{
				line = line.Trim();
				if  (flow_line.type != FlowData.options)
				{
					flow_line = new Flow{};
				}
				// 处理空行与中括号部分
				line = AnalyzeSymbols(line, reader);
				if (EndLine())
				{
					return new_datas;
				}

				// 处理大括号部分
				if (line.StartsWith("{"))
				{
					while (!line.Contains("}")) {
						line += reader.ReadLine();
					}

					if (flow_line.type == FlowData.options)
					{
						set_option.Add(line);
					}
					else
					{
						flow_line = SetBracesFunc1(line, new_datas, flow_line);
					}
					line = line.Split("}")[1].Trim();
					if (line == "")
					{
						line = AnalyzeSymbols(line, reader);
						if (EndLine())
						{
							return new_datas;
						}
					}
				}
				else if (flow_line.type == FlowData.options) 
				{
					set_option.Add(line);
				}

				if (flow_line.type != FlowData.options) 
				{
					// 处理对话部分
					if (line.Contains(":"))
					{
						string[] parts = line.Split(':');
						flow_line.name = parts[0].Trim();
						flow_line.text = parts[1].Trim();
					}
					else
					{
						flow_line.text = line;
					}
					new_datas.Add(flow_line);
				}
			}
			EndLine();
		}
		return new_datas;
	}

	// 包含大括号的字段分析
	static Flow SetBracesFunc1(string line, List<Flow> new_datas, Flow flow_line)
	{
		// 去掉括号
		match = Regex.Match(line, @"\{([^}]+)\}");
		if (match.Success)
		{
			string set_line = match.Groups[1].Value;
			foreach (string set in set_line.Split(','))
			{
				string[] sets = set.Split(':');
				switch (sets[0].Trim())
				{
					// 背景设置
					case "bg":
						new_datas.Add(
								new Flow{
								type = FlowData.background,
								text = sets[1],
								}
							     );
						break;
						// 特效设置
					case "ef":
						break;
					case "script":
						Flow script_flow = new_datas[new_datas.Count - 1];
						script_flow.script = sets[1];
						new_datas[new_datas.Count - 1] = script_flow;
						break;
					case "jump":
						Flow jump_flow = new_datas[new_datas.Count - 1];
						jump_flow.jump = sets[1];
						new_datas[new_datas.Count - 1] = jump_flow;
						break;
					default:
						flow_line.anima = AnalyzeAnima(sets);
						break;
				}
			}
		}
		return flow_line;
	}

	// option中处理包含大括号的字段
	public static Flow SetBracesFunc2(string line)
	{
		// 去掉括号
		Flow option_flow_line = new Flow{};
		match = Regex.Match(line, @"\{([^}]+)\}");
		if (match.Success)
		{
			string set_line = match.Groups[1].Value;
			foreach (string set in set_line.Split(','))
			{
				string[] sets = set.Split(':');
				switch (sets[0].Trim())
				{
					// 背景设置
					case "bg":
						option_flow_line.type = FlowData.background;
						option_flow_line.text = sets[1];
						break;
						// 特效设置
					case "ef":
						break;
					case "script":
						option_flow_line.script = sets[1];
						break;
					case "jump":
						option_flow_line.jump = sets[1];
						break;
					default:
						option_flow_line.anima = AnalyzeAnima(sets);
						break;
				}
			}
			if (option_flow_line.type != FlowData.background)
			{
				option_flow_line.type = FlowData.option;
			}
		}
		return option_flow_line;
	}

	static bool EndLine()
	{
		bool flag = false;
		if (line == null)
		{
			if (flow_line.type == FlowData.option)
			{
				flow_line.option = set_option;
				new_datas.Add(flow_line);
				new_datas.Add(new Flow{});
			}
			flag = true;
		}
		return flag;
	}

	// 分析符号部分
	static string AnalyzeSymbols(string line, StreamReader reader)
	{
		while (line != null && 
				(line.Trim() == "" || line.StartsWith("'''") || line.StartsWith("#") || line.StartsWith("[") || line.StartsWith("@"))
		      )
		{
			// 处理中括号部分
			if (line.StartsWith("["))
			{
				if (flow_line.type == FlowData.options)
				{
					flow_line.option = set_option;
					new_datas.Add(flow_line);
					new_datas.Add(new Flow{});
				}
				match = Regex.Match(line,  @"\[([^\]]*)\]");
				flow_line.type = match.Groups[1].Value;
				if (flow_line.type == FlowData.options)
				{
					set_option = new List<string>();
				}
			}
			// 处理块注释
			if (line.StartsWith("'''"))
			{
				line = reader.ReadLine();
				while (!line.Contains("'''") && line != null)
				{
					line = reader.ReadLine();
				}
			}
			// 设置跳转标志
			if (line.StartsWith("@"))
			{
				flow_line.jptr = line.Substring(1).Trim();
				//print(flow_line);
			}
			// 获取新line
			line = reader.ReadLine();
		}
		return line;
	}

	// 识别立绘部分字段, 生成立绘参数
	static Anima AnalyzeAnima(string[] sets)
	{
		string[] animas = sets[1].Split("-");
		string[] anima_position = animas[1].Split("x");
		Anima anima = new Anima{
			type = sets[0],
			name = animas[0],
			position = new Vector2(int.Parse(anima_position[0]), int.Parse(anima_position[1])),
			scale = float.Parse(animas[2]),
		};
		if (animas.Length > 3)
		{
			anima.state = animas[3];
		}
		return anima;
	}

	
}
