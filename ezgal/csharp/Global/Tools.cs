using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Tools : Node
{
	/// <summary>
	/// Why avoid `expand_mode` for image scaling:
	/// Godot lacks native support for dynamically selecting the stretching axis. Using built-in modes may cause 
	/// Aspect ratio distortion
	/// or
	/// Imbalanced scaling
	/// </summary> 
	public static void SetTexture(Sprite2D sprite, string path)
	{
		// path = "background/start_texture", sprite = start_texture
		var texture = ResourceLoader.Load($"./image/{path}.png") as Texture2D
			?? ResourceLoader.Load($"./image/{path}.jpg") as Texture2D;
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
		var texture = ResourceLoader.Load($"./image/{path}.png") as Texture2D
			?? ResourceLoader.Load($"./image/{path}.jpg") as Texture2D;

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
