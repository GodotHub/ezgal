using Godot;
using System;
using System.Collections.Generic;

public partial class FlowData : Node
{
	public const string dialogue = "对话框";
	public const string fullscreen = "全屏";
	public const string options = "选项";
	public const string option = "选择";
	public const string background = "背景";


	public struct FileData{
		public string file;
		public List<Global.Flow> data;
	}

	public static List<FileData> flowdata = new List<FileData>{
		new FileData{
			file = "测试2.txt",
			data = new List<Global.Flow>{
				new Global.Flow{
					type         = "全屏",
					text         = "现在是\"测试2.txt\"文件的内容",
				},
				new Global.Flow{
					text         = "我们可以测试更多跳转功能",
				},
				new Global.Flow{
					text         = "我现在设置了一个jump的跳转节点",
					script       = "测试3.txt",
				},
				new Global.Flow{
					text         = "这里是\"测试2.txt\"的\"循环1\"jump节点",
					name         = "少女",
					jptr         = "循环1",
				},
				new Global.Flow{
					text         = "我们可以更进一步使用选项进行选择",
					name         = "少女",
				},
				new Global.Flow{
					type         = "选项",
					option = new List<string>{
						"{script:测试3.txt}跳转测试3",
						"{jump:循环1}跳转循环1",
						"继续",
					},
				},
				new Global.Flow(),
				new Global.Flow{
					type         = "对话框",
					text         = "差不多就是这些内容了",
					name         = "少女",
					option = new List<string>{
						"{script:测试3.txt}跳转测试3",
						"{jump:循环1}跳转循环1",
						"继续",
					},
				},
			},
		},
		new FileData{
			file = "测试3.txt",
			data = new List<Global.Flow>{
				new Global.Flow{
					type         = "对话框",
					text         = "现在是\"测试3.txt\"文件的内容",
					name         = "少女",
				},
				new Global.Flow{
					text         = "我们可以通过设置的跳转节点回到\"测试2.txt\"文件",
					name         = "少女",
					script       = "测试2.txt",
					jump         = "循环1",
				},
			},
		},
		new FileData{
			file = "start.txt",
			data = new List<Global.Flow>{
				new Global.Flow{
					type         = "全屏",
					text         = "我现在修改了外部内容",
				},
				new Global.Flow{
					text         = "这里是重新开发的ezgal",
				},
				new Global.Flow{
					text         = "现在支持直接编辑txt文件",
				},
				new Global.Flow{
					text         = "这是全屏状态",
				},
				new Global.Flow{
					text         = "全屏下可以输入[color=red][url=文字信息]文字信息[/url][/color]",
				},
				new Global.Flow{
					text         = "根据设置默认支持12行",
				},
				new Global.Flow{
					text         = "过程中我们可以编辑背景",
				},
				new Global.Flow{
					text         = "比如设置背景为新背景的背景",
				},
				new Global.Flow{
					type         = "背景",
					text         = "ezbg.png",
				},
				new Global.Flow{
					text         = "现在背景更新了",
				},
				new Global.Flow{
					text         = "除此之外我们可以进行注释",
				},
				new Global.Flow{
					text         = "比如使用#号",
				},
				new Global.Flow{
					text         = "或者我们可以使用'''对块进行批量注释",
				},
				new Global.Flow{
					text         = "比如下面的情况",
				},
				new Global.Flow{
					text         = "现在我们超过了12行",
				},
				new Global.Flow{
					text         = "文字会自动更新屏幕",
				},
				new Global.Flow{
					type         = "全屏",
					text         = "或者我们可以设置全屏主动清屏",
				},
				new Global.Flow{
					type         = "对话框",
					text         = "然后是对话框功能",
				},
				new Global.Flow{
					text         = "对话框中我们可以设置角色名字",
				},
				new Global.Flow{
					text         = "让女主角给大家打个招呼",
				},
				new Global.Flow{
					text         = "大家好",
					name         = "少女",
					anima = new Global.Anima{
						type         = "少女",
						name         = "normal.png",
						position	= new Vector2(1200, 650),
						scale	= 1.1f,
					},
				},
				new Global.Flow{
					text         = "新ezgal的解释器已经实现了大部分基本功能.",
					name         = "少女",
					script       = "测试2.txt",
				},
				new Global.Flow{
					text         = "这些是跳转后的内容, 跳转后会隐藏后面的内容",
				},
			},
		},
	};

	public struct DicData{
		public string file;
		public string data;
	}
	public static List<DicData> dicdata = new List<DicData>{
		new DicData{
			file = "bbcode编码.txt",
			data = ">[i]bbcode编码[/i]\n\nBBCode（Bulletin Board Code）是一种轻量级标记语言, 使其在格式显示上更加美观, 以下是一些常用格式:\n[b]加粗文本[/b]\n[i]斜体文本[/i]\n[u]下划线文本[/u]\n[color=red]颜色文本[/color]\n[url=bbcode]链接文本[/url]\n\n",
		},
		new DicData{
			file = "文字信息.txt",
			data = ">[i]文字信息[/i]\n\n文字信息统一采用富文本进行编辑, 支持[color=blue][url=bbcode编码]bbcode编码[/url][/color], 可以通过设置url指向dictionary文件(指向链接不需要包含.txt的后缀)达成查询字典信息.",
		},
	};
}
