using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class Ezlang
{
	// 程序控制文本
	public static List<Flow> datas;
	// 当前文本名称
	public static string read_file_name;
	// 专业词汇查询
	public static bool is_query = false;

	static string line;
	static Match match;
	static Flow flow_line;
	static List<Flow> new_datas;
	static List<string> set_option;

	public struct Flow
        {
                // 节点类型
                public string type;

                // 文本功能集合：
                // 文本信息(接入History体现)
                public string text;
                // 说话对象
                public string name;

                // 图片功能集合：
                // 背景
                public string background;
                // 立绘
                public Anima anima;

                // 选项
                public List<string> option;
		// 维持
		public int? wait;

                // 跳转功能集合：
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

	public struct Vector2
	{
		public int X { get; set; }
		public int Y { get; set; }

		public Vector2(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	// 读取文本文档转义为json格式.
	public static List<Flow> ReadFile(string path)
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
						SetBracesFunc(line, new_datas);
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
	static void SetBracesFunc(string line, List<Flow> new_datas)
	{
		// 去掉括号
		match = Regex.Match(line, @"\{([^}]+)\}");
		if (match.Success)
		{
			string set_line = match.Groups[1].Value;
			bool solo_performance = line.Contains("wait");
			bool next_data = false;

			foreach (string set in set_line.Split(','))
			{
				string[] sets = set.Split(':');
				int new_datas_count = new_datas.Count - 1;
				Flow data_flow = new_datas[new_datas_count];
				switch (sets[0].Trim())
				{
					// 等待设置
					case "wait":
						if (next_data) {
							data_flow.wait = int.Parse(sets[1]);
							break;
						}
						new_datas.Add(
							new Flow{
								wait = int.Parse(sets[1])
							}
						);
						next_data = true;
						break;
					// 背景设置
					case "bg":
						if (next_data) {
							data_flow.type = FlowData.direction;
							data_flow.background = sets[1];
							break;
						}
						if (!solo_performance) {
							data_flow.background = sets[1];
							break;
						}
						new_datas.Add(
							new Flow{
								type = FlowData.direction,
								background = sets[1],
							}
						);
						next_data = true;
						break;
					case "ef":
						break;
					case "script":
						data_flow.script = sets[1];
						break;
					case "jump":
						data_flow.jump = sets[1];
						break;
					default:
						if (next_data) {
							data_flow.type = FlowData.direction;
							data_flow.anima = AnalyzeAnima(sets);
							break;
						}
						if (!solo_performance) {
							data_flow.anima = AnalyzeAnima(sets);
							break;
						}
						new_datas.Add(
							new Flow{
								type = FlowData.direction,
								anima = AnalyzeAnima(sets)
							}
						);
						next_data = true;
						break;
				}
			new_datas[new_datas_count] = data_flow;
			}
		}
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
