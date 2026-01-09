using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class Make
{
	// 读取文件地址
	public static string read_file_path = "./ezgal/script";
	// 读取字典地址
	public static string dict_file_path = "./ezgal/technical";
	// 代码存储地址
	public static string save_file_path = "./ezgal/csharp/Global/FlowData.cs";
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

	public static void Main(string[] args)
	{
		if (args.Length < 1)
		{
			help();
			return;
		}
		else if (args[0] == "build")
		{
			try
			{
				string result = FlowData.set_language(args[1]);
				if (result.StartsWith("Error"))
				{
					Console.WriteLine(result);
					return;
				}
				buildfile();
				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[False] builcfile() error: {ex.Message}");
			}

		}
		else if (args[0] == "edit")
		{
			try
			{
				string result = FlowData.set_language(args[1]);
				if (result.StartsWith("Error"))
				{
					Console.WriteLine(result);
					return;
				}
				editfile();
				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[False] editfile() error: {ex.Message}");
			}
		}
	}

	public static void help()
	{
		string helpText = """
			Usage: dotnet run --project make <COMMAND> [LANGUAGE]

			Commands:
			   build      Build script content into code for the specified language
			   edit       Revert code functionality to script mode for the specified language

			Languages:
			   zh/CN      Chinese
			   en/EN      English
			   jp/JP      Japanese

			Examples:
			   dotnet run --project make build en
			   dotnet run --project make edit ZH

			Notes:
			   This toolchain is currently in TESTING status, please refer to this version's README for accurate syntax.
			""";
		Console.WriteLine(helpText);
	}

	public static void buildfile()
	{
		string return_file_data = "";
		return_file_data += print_init("in");
		return_file_data += print_init("const");
		return_file_data += print_init("file");
		foreach (string file in Directory.GetFiles(read_file_path))
		{
			if (file.Length > 3 && file.EndsWith(".txt"))
			{
				return_file_data += print_file(file);
			}
		}
		return_file_data += "\t};\n";
		return_file_data += print_init("dict");
		foreach (string file in Directory.GetFiles(dict_file_path))
		{
			return_file_data += print_technical(file);
		}
		return_file_data += "\t};\n";
		return_file_data += print_init("out");
		return_file_data = return_file_data.Replace("\"", "\\\"").Replace("$34$", "\"");
		File.WriteAllText(save_file_path, return_file_data);
	}

	public static void editfile()
	{
		string return_file_data = "";
		return_file_data += print_init("in");
		return_file_data += print_init("const");
		return_file_data += print_init("file");
		return_file_data += "\t};\n";
		return_file_data += print_init("dict");
		return_file_data += "\t};\n";
		return_file_data += print_init("out");
		return_file_data = return_file_data.Replace("{\n\t};", "();").Replace("\"", "\\\"").Replace("$34$", "\"");
		File.WriteAllText(save_file_path, return_file_data);
	}

	public static bool IsEmpty<T>(T structure) where T : struct
	{
		return EqualityComparer<T>.Default.Equals(structure, default(T));
	}

	public static string print_init(string print_name)
	{
		string return_file_data = "";
		switch (print_name)
		{
			case "in":
				return_file_data += "using Godot;\n";
				return_file_data += "using System;\n";
				return_file_data += "using System.Collections.Generic;\n";
				return_file_data += "\n";
				return_file_data += "public partial class FlowData : Node\n";
				return_file_data += "{\n";
				break;
			case "const":
				return_file_data += $"\tpublic const string dialogue = $34${FlowData.dialogue}$34$;\n";
				return_file_data += $"\tpublic const string fullscreen = $34${FlowData.fullscreen}$34$;\n";
				return_file_data += $"\tpublic const string options = $34${FlowData.options}$34$;\n";
				return_file_data += $"\tpublic const string option = $34${FlowData.option}$34$;\n";
				return_file_data += $"\tpublic const string background = $34${FlowData.background}$34$;\n";
				return_file_data += "\n";
				break;
			case "file":
				return_file_data += "\n";
				return_file_data += "\tpublic struct FileData{\n";
				return_file_data += "\t\tpublic string file;\n";
				return_file_data += "\t\tpublic List<Global.Flow> data;\n";
				return_file_data += "\t}\n";
				return_file_data += "\n";
				return_file_data += "\tpublic static List<FileData> flowdata = new List<FileData>{\n";
				break;
			case "dict":
				return_file_data += "\n";
				return_file_data += "\tpublic struct TechData{\n";
				return_file_data += "\t\tpublic string file;\n";
				return_file_data += "\t\tpublic string data;\n";
				return_file_data += "\t}\n";
				return_file_data += "\tpublic static List<TechData> Techdata = new List<TechData>{\n";
				break;
			case "out":
				return_file_data += "}\n";
				break;
			default:
				break;
		}
		return return_file_data;
	}

	public static string print_file(string file)
	{
		string return_file_data = "";
		return_file_data += "\t\tnew FileData{\n";
		return_file_data += $"\t\t\tfile = $34${Path.GetFileName(file)}$34$,\n";
		return_file_data += "\t\t\tdata = new List<Global.Flow>{\n";
		foreach (Flow flow in ReadFile(file))
		{
			return_file_data += print_flow(flow);
		}
		return_file_data += "\t\t\t},\n";
		return_file_data += "\t\t},\n";
		return return_file_data;
	}

	public static string print_technical(string file)
	{
		string return_file_data = "";
		return_file_data += "\t\tnew TechData{\n";
		return_file_data += $"\t\t\tfile = $34${Path.GetFileName(file)}$34$,\n";
		return_file_data += $"\t\t\tdata = $34${NewLines(File.ReadAllText(file))}$34$,\n";
		return_file_data += "\t\t},\n";
		return return_file_data;

	}

	static string NewLines(string input)
	{
		string step1 = input.Replace("\r\n", "\\n");
		string step2 = step1.Replace("\r", "\\n");
		string final = step2.Replace("\n", "\\n");
		return final;
	}

	public static string print_flow(Flow data)
	{
		string return_file_data = "";
		if (IsEmpty(data))
		{
			return_file_data += "\t\t\t\tnew Global.Flow(),\n";
		}
		else
		{
			return_file_data += "\t\t\t\tnew Global.Flow{\n";
			if (data.type != null) { return_file_data += ($"\t\t\t\t\ttype         = $34${data.type}$34$,\n"); }
			;
			if (data.text != null) { return_file_data += ($"\t\t\t\t\ttext         = $34${data.text}$34$,\n"); }
			;
			if (data.name != null) { return_file_data += ($"\t\t\t\t\tname         = $34${data.name}$34$,\n"); }
			;
			if (!IsEmpty(data.anima))
			{
				return_file_data += "\t\t\t\t\tanima = new Global.Anima{\n";
				if (data.anima.type != null) { return_file_data += ($"\t\t\t\t\t\ttype         = $34${data.anima.type}$34$,\n"); }
				;
				if (data.anima.name != null) { return_file_data += ($"\t\t\t\t\t\tname         = $34${data.anima.name}$34$,\n"); }
				;
				if (!IsEmpty(data.anima.position)) { return_file_data += ($"\t\t\t\t\t\tposition	= new Vector2({data.anima.position.X}, {data.anima.position.Y}),\n"); }
				;
				if (data.anima.scale != 0.0f) { return_file_data += ($"\t\t\t\t\t\tscale	= {data.anima.scale}f,\n"); }
				;
				if (data.anima.state != null) { return_file_data += ($"\t\t\t\t\t\tstate	= $34${data.anima.state}$34$,\n"); }
				;
				return_file_data += "\t\t\t\t\t},\n";
			}
			;
			if (data.option != null)
			{
				return_file_data += "\t\t\t\t\toption = new List<string>{\n";
				foreach (string option in data.option)
				{
					return_file_data += $"\t\t\t\t\t\t$34${option}$34$,\n";
				}
				return_file_data += "\t\t\t\t\t},\n";
			}
			if (data.script != null) { return_file_data += ($"\t\t\t\t\tscript       = $34${data.script}$34$,\n"); }
			;
			if (data.jump != null) { return_file_data += ($"\t\t\t\t\tjump         = $34${data.jump}$34$,\n"); }
			;
			if (data.jptr != null) { return_file_data += ($"\t\t\t\t\tjptr         = $34${data.jptr}$34$,\n"); }
			;
			return_file_data += "\t\t\t\t},\n";
		}
		return return_file_data;
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
