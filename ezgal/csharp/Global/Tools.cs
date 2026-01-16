using Godot;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public partial class Tools : Node
{
	/// <summary>
	/// Why avoid `expand_mode` for image scaling:
	/// Godot lacks native support for dynamically selecting the stretching axis. Using built-in modes may cause 
	/// Aspect ratio distortion
	/// or
	/// Imbalanced scaling
	/// </summary> 
	public static Texture2D LoadImage(string absolutePath)
	{
		if (FlowData.IsBuild) {
			return ResourceLoader.Load<Texture2D>(absolutePath);
		}
		Image image = new Image();
		Error err = image.Load(absolutePath);

		if (err == Error.Ok)
		{
			ImageTexture texture = new ImageTexture();
			texture.SetImage(image);
			return texture;
		}
		else
		{
			GD.PrintErr($"加载失败 ({err}): {absolutePath}");
			return null;
		}
	}


	/// <summary>
	/// Set Audio.
	/// </summary>
	public static AudioStream LoadAudio(string absolutePath)
	{
		if (FlowData.IsBuild) {
			return ResourceLoader.Load<AudioStream>(absolutePath);
		}
		string extension = System.IO.Path.GetExtension(absolutePath).ToLower();
		switch (extension)
		{
			case ".wav":
				return LoadWavFile(absolutePath);
			case ".ogg":
				return LoadOggFile(absolutePath);
			case ".mp3":
				return LoadMp3File(absolutePath);
			default:
				return null;
		}
	}

	private static AudioStreamWav LoadWavFile(string path)
	{
		var audioStream = new AudioStreamWav();

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			return null;
		}

		audioStream.Data = file.GetBuffer((long)file.GetLength());
		audioStream.Format = AudioStreamWav.FormatEnum.Format16Bits; // 根据实际格式调整
		audioStream.Stereo = true; // 根据实际调整
		audioStream.MixRate = 44100; // 根据实际调整

		return audioStream;
	}

	private static AudioStreamOggVorbis LoadOggFile(string path)
	{
		var audioStream = AudioStreamOggVorbis.LoadFromFile(path);
		return audioStream;
	}

	private static AudioStreamMP3 LoadMp3File(string path)
	{
		var audioStream = new AudioStreamMP3();

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			return null;
		}

		audioStream.Data = file.GetBuffer((long)file.GetLength());
		return audioStream;
	}

	public static void SetTexture(Sprite2D sprite, string path)
	{
		// path = "background/start_texture", sprite = start_texture
		var texture = Tools.LoadImage($"./image/{path}.png") as Texture2D
			?? Tools.LoadImage($"./image/{path}.jpg") as Texture2D;
		if (texture == null)
		{
			GD.PrintErr($"`./image/` Failed to load `{path}.png` or `{path}.jpg`.");
		}
		else
		{
			sprite.Texture = texture;
			float width = (float)Global.window_width / (float)texture.GetWidth();
			float height = (float)Global.window_height / (float)texture.GetHeight();
			if (height > width)
			{
				sprite.Scale = new Vector2(height, height);
			}
			else
			{
				sprite.Scale = new Vector2(width, width);
			}
		}
	}

	/*
	   public static void SetTexture(TextureRect textureRect, string path)
	   {
	   var texture = Tools.LoadImage($"./image/{path}.png") as Texture2D
	   ?? Tools.LoadImage($"./image/{path}.jpg") as Texture2D;

	   if (texture == null)
	   {
	   GD.PrintErr($"`./image/` Failed to load `{path}.png` or `{path}.jpg`.");
	   }
	   else
	   {
	   textureRect.Texture = texture;
	   }
	   }
	   */

	// todo: only remove like "[url]" & "[/url]", while [example] will not remove.
	public static string RemoveBBCode(string input)
	{
		return Regex.Replace(input, @"\[\/?[^\]]+\]", "");
	}
}

public partial class ToolsInit : Node
{
	public static string FindInitJsonType(string scene, string node, string key)
	{
		string jsonString = FlowData.jsonString;
		if (!FlowData.IsBuild)
		{
			jsonString = System.IO.File.ReadAllText("./script/.init.json");
		}
		using JsonDocument doc = JsonDocument.Parse(jsonString);
		JsonElement rootElement = doc.RootElement;
		if (!rootElement.TryGetProperty(scene, out JsonElement sceneElement))
			return "";
		if (!sceneElement.TryGetProperty(node, out JsonElement nodeElement))
			return "";
		if (!nodeElement.TryGetProperty(key, out JsonElement keyElement))
			return "";
		return $"{keyElement}";
	}
}

public partial class LoadGame : Node
{
	public static void SaveGame()
	{
		return;
	}
}

