using Godot;
using System;
using System.Collections.Generic;

public partial class FlowData : Node
{
	public const string dialogue = "对话框";
	public const string fullscreen = "全屏";
	public const string options = "选项";
	public const string option = "选择";
	public const string direction = "演出";
	public const bool IsBuild = false;

	public const string jsonString = null;


	public struct FileData{
		public string file;
		public List<Global.Flow> data;
	}

	public static List<FileData> flowdata = new List<FileData>();

	public struct TechData{
		public string file;
		public string data;
	}
	public static List<TechData> Techdata = new List<TechData>();
}
