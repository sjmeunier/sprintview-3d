using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
	public class Settings : MonoBehaviour
	{
		private static string _settingsFilename = "config.txt";

		public static string UserName = "";
		public static string Token = "";
		public static string Account = "";
		public static string Project = "";
		public static string Team = "";
		public static ChartType ChartType = ChartType.BurnupItems;

		public static string[] ChartTypeText =
		{
			"Burndown - Story Points",
			"Burndown - Items",
			"Burnup - Story Points",
			"Burnup - Items"
		};

		public static void SaveSettings()
		{
			if (File.Exists(_settingsFilename))
				File.Delete(_settingsFilename);

			using (StreamWriter writer = new StreamWriter(_settingsFilename))
			{
				writer.WriteLine(String.Format("UserName: {0}", UserName));
				writer.WriteLine(String.Format("Token: {0}", Token));
				writer.WriteLine(String.Format("Account: {0}", Account));
				writer.WriteLine(String.Format("Project: {0}", Project));
				writer.WriteLine(String.Format("Team: {0}", Team));
				writer.WriteLine(String.Format("ChartType: {0}", (int) ChartType));
			}
		}

		public static void LoadSettings()
		{

			if (File.Exists(_settingsFilename))
			{
				var separator = new char[] {':'};
				using (StreamReader reader = new StreamReader(_settingsFilename))
				{
					while (!reader.EndOfStream)
					{
						var line = reader.ReadLine();
						if (line.Contains(":"))
						{
							var arr = line.Split(separator);
							switch (arr[0].ToLower().Trim())
							{
								case "username":
									UserName = arr[1].Trim();
									break;
								case "token":
									Token = arr[1].Trim();
									break;
								case "account":
									Account = arr[1].Trim();
									break;
								case "project":
									Project = arr[1].Trim();
									break;
								case "team":
									Team = arr[1].Trim();
									break;
								case "charttype":
									ChartType = (ChartType) Int32.Parse(arr[1].Trim());
									break;
							}
						}
					}
				}
			}
		}
	}
}