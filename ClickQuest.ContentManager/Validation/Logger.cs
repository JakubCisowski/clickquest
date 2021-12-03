using System;
using System.IO;

namespace ClickQuest.ContentManager.Validation
{
	public static class Logger
	{
		public static DateTime SessionStartDate = DateTime.Now;

		public static void Log(string log)
		{
			string folderPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.ToString(), "Logs", "AssetsLogs");

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			// Log bugs in specified format.
			using StreamWriter writer = new StreamWriter(Path.Combine(folderPath, "Logs " + SessionStartDate.ToString("dd-MM-yyyy-HH-mm-ss") + ".txt"), true);

			writer.WriteLine(log);
		}
	}
}