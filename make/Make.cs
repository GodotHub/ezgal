using System;

public class Make
{
	public static void Main(string[] args)
	{
		if (args.Length < 1)
		{
			Write.Help();
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
				Write.BuildFile();
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
				Write.EditFile();
				Console.WriteLine(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[False] editfile() error: {ex.Message}");
			}
		}
	}
}
