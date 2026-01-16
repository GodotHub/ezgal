using System;
using System.IO;
using System.Collections.Generic;

public class Write
{
	// 鉴别模块
	public static bool isBuild { get; set; }
	// 读取初始化信息地址
	public static string read_init_path = "./ezgal/script/.init.json";
	// 读取文件地址
	public static string read_file_path = "./ezgal/script";
	// 读取字典地址
	public static string dict_file_path = "./ezgal/technical";
	// 代码存储地址
	public static string save_file_path = "./ezgal/csharp/Global/FlowData.cs";

	public static void Help()
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

	public static void BuildFile()
	{
		isBuild = true;
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

	public static void EditFile()
	{
		isBuild = false;
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

	private static bool IsEmpty<T>(T structure) where T : struct
	{
		return EqualityComparer<T>.Default.Equals(structure, default(T));
	}

	private static string print_init(string print_name)
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
				return_file_data += $"\tpublic const string direction = $34${FlowData.direction}$34$;\n";
				return_file_data += $"\tpublic const bool IsBuild = {isBuild.ToString().ToLower()};\n";
				return_file_data += $"\n";
				if (isBuild)
				{
					return_file_data += $"\tpublic const string jsonString = $34$$34$$34$\n";
					return_file_data += File.ReadAllText(read_init_path).Replace("\"", "$34$");
					return_file_data += "$34$$34$$34$;";
				}
				else
				{
					return_file_data += $"\tpublic const string jsonString = null;\n";
				}
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

	private static string print_file(string file)
	{
		string return_file_data = "";
		return_file_data += "\t\tnew FileData{\n";
		return_file_data += $"\t\t\tfile = $34${Path.GetFileName(file)}$34$,\n";
		return_file_data += "\t\t\tdata = new List<Global.Flow>{\n";
		foreach (Ezlang.Flow flow in Ezlang.ReadFile(file))
		{
			return_file_data += print_flow(flow);
		}
		return_file_data += "\t\t\t},\n";
		return_file_data += "\t\t},\n";
		return return_file_data;
	}

	private static string print_technical(string file)
	{
		string return_file_data = "";
		return_file_data += "\t\tnew TechData{\n";
		return_file_data += $"\t\t\tfile = $34${Path.GetFileName(file)}$34$,\n";
		return_file_data += $"\t\t\tdata = $34${NewLines(File.ReadAllText(file))}$34$,\n";
		return_file_data += "\t\t},\n";
		return return_file_data;

	}

	private static string NewLines(string input)
	{
		string step1 = input.Replace("\r\n", "\\n");
		string step2 = step1.Replace("\r", "\\n");
		string final = step2.Replace("\n", "\\n");
		return final;
	}

	private static string print_flow(Ezlang.Flow data)
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
			if (data.background != null) { return_file_data += ($"\t\t\t\t\tbackground   = $34${data.background}$34$,\n"); }
			;
			if (!IsEmpty(data.anima))
			{
				return_file_data += "\t\t\t\t\tanima = new Global.Anima{\n";
				if (data.anima.type != null) { return_file_data += ($"\t\t\t\t\t\ttype         = $34${data.anima.type}$34$,\n"); }
				;
				if (data.anima.name != null) { return_file_data += ($"\t\t\t\t\t\tname         = $34${data.anima.name}$34$,\n"); }
				;
				if (!IsEmpty(data.anima.position)) { return_file_data += ($"\t\t\t\t\t\tposition      = new Vector2({data.anima.position.X}, {data.anima.position.Y}),\n"); }
				;
				if (data.anima.scale != 0.0f) { return_file_data += ($"\t\t\t\t\t\tscale    = {data.anima.scale}f,\n"); }
				;
				if (data.anima.state != null) { return_file_data += ($"\t\t\t\t\t\tstate    = $34${data.anima.state}$34$,\n"); }
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
			if (data.wait != null) { return_file_data += ($"\t\t\t\t\twait         = {data.wait},\n"); }
			;
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
}
