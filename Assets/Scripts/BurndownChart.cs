using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Gui;
using Assets.Scripts.Vsts;
using Assets.Scripts.Vsts.Models;

namespace Assets.Scripts
{
	public class BurndownChart : MonoBehaviour
	{
		public static ChartStateEnum ChartState = ChartStateEnum.Startup;
		public static bool LoadedObjects = false;
		

		private static string _loadingText = "";
		private static string _errorText = "";

		private Loader _loader = new Loader();

		private static Dictionary<string, DaySummary> _savedDaySummaries;
		private static Dictionary<string, string> _barSummaryText;

		SettingsScreen settingsScreen = new SettingsScreen();
		public static ChartStateEnum StateBeforeSettings = ChartStateEnum.Startup;

		public static string SelectedDayKey = "";

		public static int LinesSprintCount = 0;
		public static int LinesDayCount = 0;
		public static int LinesMax = 0;
		public static List<string> LinesSprintText = new List<string>();
		public static float HeightScale = 0.25f;
		public static int ZOrigOffset = 0;
		public static float XOrigOffset = 0;



		// Update is called once per frame
		void Update()
		{

		}

		void Start()
		{
			Settings.LoadSettings();
			_savedDaySummaries = new Dictionary<string, DaySummary>();

			ChartState = ChartStateEnum.Startup;
		}

		private void DeleteGameObjects()
		{
			foreach (GameObject bar in GameObject.FindGameObjectsWithTag("Bar"))
				GameObject.DestroyImmediate(bar);
		}

		private Color[] BarColors = new Color[]
		{
			new Color(1, 0, 0, 0.5f),
			new Color(0, 1, 0, 0.5f),
			new Color(0, 0, 1, 0.5f),
			new Color(1, 1, 0, 0.5f),
			new Color(0, 1, 1, 0.5f),
			new Color(1, 0, 1, 0.5f)
		};

		private void CreateText(string text, Vector3 position, Quaternion rotation, float scale = 1f)
		{
			GameObject textObj = (GameObject) Instantiate(Resources.Load("TextPanel"), position, rotation);

			textObj.tag = "Text";
			textObj.GetComponent<TextMesh>().text = text;
			textObj.transform.localScale = new Vector3(0.5f * scale, 0.5f * scale);
		}

		private void DeleteTexts()
		{
			foreach (GameObject text in GameObject.FindGameObjectsWithTag("Text"))
				GameObject.DestroyImmediate(text);
		}

		private void CreateTexts()
		{
			float xOffset = BurndownChart.XOrigOffset - 0.5f;
			float yOffset = 0f;
			float zOffset = BurndownChart.ZOrigOffset + 0.5f;
			int yMax = BurndownChart.LinesMax;
			if (BurndownChart.LinesMax % 5 != 0)
				yMax += (5 - (BurndownChart.LinesMax % 5));

			DeleteTexts();
			int i = 0;
			for (i = 1; i <= BurndownChart.LinesDayCount; i++)
			{
				CreateText(i.ToString(), new Vector3(xOffset + i - 0.8f, yOffset, zOffset - BurndownChart.LinesSprintCount - 0.1f),
					Quaternion.Euler(90, 0, 0));
			}

			for (i = 5; i <= yMax; i = i + 5)
			{
				CreateText(i.ToString(),
					new Vector3(xOffset - 0.7f, yOffset + (i * BurndownChart.HeightScale) + 0.4f,
						zOffset - BurndownChart.LinesSprintCount - 0.1f), Quaternion.Euler(0, 0, 0));
			}

			i = 0;
			foreach (var name in BurndownChart.LinesSprintText)
			{
				CreateText(name, new Vector3(xOffset + BurndownChart.LinesDayCount + 0.2f, yOffset, zOffset - i - 0.1f),
					Quaternion.Euler(90, 0, 0));
				i++;
			}

			CreateText(Settings.ChartTypeText[(int) Settings.ChartType],
				new Vector3(xOffset + 3f, yOffset, zOffset - BurndownChart.LinesSprintCount - 1.0f), Quaternion.Euler(90, 0, 0));
			CreateText(CommonData.Team.Name, new Vector3(xOffset + 0f, yOffset, zOffset + 10.0f), Quaternion.Euler(90, 0, 0),
				3);

		}

		private void DeleteLines()
		{
			foreach (GameObject line in GameObject.FindGameObjectsWithTag("Line"))
				GameObject.DestroyImmediate(line);
		}

		private void CreateLineBetweenPoints(Vector3 start, Vector3 end, float width)
		{
			var offset = end - start;
			var scale = new Vector3(width, offset.magnitude / 2.0f, width);
			var position = start + (offset / 2.0f);


			GameObject line = (GameObject) Instantiate(Resources.Load("MainLine"), position, Quaternion.identity);
			line.transform.up = offset;
			line.transform.localScale = scale;
			line.tag = "Line";

		}

		private void CreateLines()
		{
			float xOffset = BurndownChart.XOrigOffset - 0.5f;
			float yOffset = 0f;
			float zOffset = BurndownChart.ZOrigOffset + 0.5f;
			int yMax = BurndownChart.LinesMax;
			if (BurndownChart.LinesMax % 5 != 0)
				yMax += (5 - (BurndownChart.LinesMax % 5));
			float lineWidth = 0.02f;

			DeleteLines();
			//Draw left bar
			CreateLineBetweenPoints(
				new Vector3(xOffset, yOffset, zOffset),
				new Vector3(xOffset, yOffset + (yMax * BurndownChart.HeightScale), zOffset),
				lineWidth);
			CreateLineBetweenPoints(
				new Vector3(xOffset, yOffset + (yMax * BurndownChart.HeightScale), zOffset),
				new Vector3(xOffset, yOffset + (yMax * BurndownChart.HeightScale), zOffset - BurndownChart.LinesSprintCount),
				lineWidth);
			CreateLineBetweenPoints(
				new Vector3(xOffset, yOffset + (yMax * BurndownChart.HeightScale), zOffset - BurndownChart.LinesSprintCount),
				new Vector3(xOffset, yOffset, zOffset - BurndownChart.LinesSprintCount),
				lineWidth);
			CreateLineBetweenPoints(
				new Vector3(xOffset, yOffset, zOffset - BurndownChart.LinesSprintCount),
				new Vector3(xOffset, yOffset, zOffset),
				lineWidth);

			//Draw grade lines
			for (int i = 5; i < yMax; i = i + 5)
			{
				CreateLineBetweenPoints(
					new Vector3(xOffset, yOffset + (i * BurndownChart.HeightScale), zOffset),
					new Vector3(xOffset, yOffset + (i * BurndownChart.HeightScale), zOffset - BurndownChart.LinesSprintCount),
					lineWidth);
			}

			//Draw right bar
			CreateLineBetweenPoints(
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset, zOffset),
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset + (yMax * BurndownChart.HeightScale), zOffset),
				lineWidth);
			CreateLineBetweenPoints(
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset + (yMax * BurndownChart.HeightScale), zOffset),
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset + (yMax * BurndownChart.HeightScale),
					zOffset - BurndownChart.LinesSprintCount),
				lineWidth);
			CreateLineBetweenPoints(
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset + (yMax * BurndownChart.HeightScale),
					zOffset - BurndownChart.LinesSprintCount),
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset, zOffset - BurndownChart.LinesSprintCount),
				lineWidth);
			CreateLineBetweenPoints(
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset, zOffset - BurndownChart.LinesSprintCount),
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset, zOffset),
				lineWidth);

			//Draw grade lines
			for (int i = 5; i < yMax; i = i + 5)
			{
				CreateLineBetweenPoints(
					new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset + (i * BurndownChart.HeightScale), zOffset),
					new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset + (i * BurndownChart.HeightScale),
						zOffset - BurndownChart.LinesSprintCount),
					lineWidth);
			}

			CreateLineBetweenPoints(
				new Vector3(xOffset, yOffset, zOffset),
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset, zOffset),
				lineWidth);
			CreateLineBetweenPoints(
				new Vector3(xOffset + BurndownChart.LinesDayCount, yOffset, zOffset - BurndownChart.LinesSprintCount),
				new Vector3(xOffset, yOffset, zOffset - BurndownChart.LinesSprintCount),
				lineWidth);

			//Draw base grade lines
			for (int i = 1; i < BurndownChart.LinesDayCount; i++)
			{
				CreateLineBetweenPoints(
					new Vector3(xOffset + i, yOffset, zOffset),
					new Vector3(xOffset + i, yOffset, zOffset - BurndownChart.LinesSprintCount - 0.5f),
					lineWidth);
			}

		}

		private void CreateGameObjects()
		{
			DeleteGameObjects();

			ZOrigOffset = 0;
			int zOffset = ZOrigOffset;
			int count = 0;
			XOrigOffset = CommonData.Iterations.Iterations.Count > 0 ? CommonData.Iterations.Iterations[0].DailySummary.Count / -2f : 0f;

			_barSummaryText = new Dictionary<string, string>();
			string summaryBaseText =
				"{0}\r\nIteration:\t\t{1}\r\nDate:\t\t{2}\r\nTotal Items:\t{3}\r\nItems Done:\t{8}\r\nItems Remaining:\t{4}\r\nTotal Story Points:\t{5}\r\nStory Points Done:\t{7}\r\nStory Points Rem.:\t{6}";

			LinesSprintCount = 0;
			LinesDayCount = 0;
			LinesMax = 0;
			LinesSprintText = new List<string>();

			foreach (var iteration in CommonData.Iterations.Iterations)
			{
				LinesDayCount = Math.Max(LinesDayCount, iteration.DailySummary.Count);
				if (iteration.DailySummary.Count > 0)
				{
					LinesSprintCount++;
					LinesSprintText.Add(iteration.Name);
				}
				float xOffset = XOrigOffset;
				foreach (var day in iteration.DailySummary)
				{
					string barKey = string.Format("barsummary-{0}", count);
					_barSummaryText.Add(string.Format("barsummary-{0}", count),
						string.Format(summaryBaseText, CommonData.Team.Name, iteration.Name, day.Date.ToString("yyyy-MM-dd"), day.GetTotalItems(),
							day.GetOpenItems(), day.GetTotalStoryPoints(), day.GetOpenStoryPoints(),
							day.GetTotalStoryPoints() - day.GetOpenStoryPoints(), day.GetTotalItems() - day.GetOpenItems()));

					float height = 0;
					if (Settings.ChartType == ChartType.BurnupStoryPoints)
					{
						height = day.GetTotalStoryPoints() - day.GetOpenStoryPoints();
					}
					else if (Settings.ChartType == ChartType.BurnupItems)
					{
						height = day.GetTotalItems() - day.GetOpenItems();
					}
					else if (Settings.ChartType == ChartType.BurndownStoryPoints)
					{
						height = day.GetOpenStoryPoints();
					}
					else if (Settings.ChartType == ChartType.BurndownItems)
					{
						height = day.GetOpenItems();
					}

					if (height == 0)
						height = 0.1f;

					LinesMax = Math.Max(LinesMax, (int) Math.Round(height));
					var barHeight = (height) * HeightScale;


					var bar = (GameObject) Instantiate(Resources.Load("Bar"), new Vector3(xOffset, barHeight / 2f, zOffset),
						Quaternion.identity);
					bar.transform.GetComponent<Renderer>().material.color = BarColors[(zOffset * -1) % BarColors.Length];
					bar.transform.localScale = new Vector3(0.9f, barHeight, 0.9f);
					bar.GetComponent<Bar>().SelectedBarKey = barKey;
					bar.tag = "Bar";
					xOffset += 1;
					count++;
				}
				zOffset -= 1;
			}
		}

		private IEnumerator CreateObjects()
		{
			//Do all the creating logic here
			yield return new WaitForSeconds(0.25f);
			CreateGameObjects();
			CreateLines();
			CreateTexts();
			BurndownChart.LoadedObjects = true;
			ChartState = ChartStateEnum.Main;
			StopCoroutine("CreateObjects");
		}

		private IEnumerator FetchSprintData()
		{
			LoadedObjects = false;
			CommonData.LoadedData = false;
			yield return new WaitForSeconds(0.25f);

			DeleteGameObjects();

			//Do fetching
			VstsApi vstsApi = new VstsApi(Settings.UserName, Settings.Token, Settings.Account);

			//Get projects
			WWW request = vstsApi.BuildGetProjectsRequest();
			yield return request;
			if (!string.IsNullOrEmpty(request.error))
			{
				_errorText = request.error;
				ChartState = ChartStateEnum.Error;
				StopCoroutine("FetchSprintData");
			}
			ProjectCollection projects = new ProjectCollection();
			try
			{
				projects = ProjectCollection.CreateFromJson(request.text);
			}
			catch (Exception e)
			{
				_errorText = e.Message;
				ChartState = ChartStateEnum.Error;
				StopCoroutine("FetchSprintData");
			}

			if (projects == null || !(projects.Projects.Any(x => x.Name.ToLower() == Settings.Project.ToLower())))
			{
				_errorText = "Project does not exist";
				ChartState = ChartStateEnum.Error;
				StopCoroutine("FetchSprintData");
			}
			CommonData.Project = projects.Projects.First(x => x.Name.ToLower() == Settings.Project.ToLower());

			//Get teams
			request = vstsApi.BuildGetTeamsRequest(CommonData.Project);

			yield return request;
			if (!string.IsNullOrEmpty(request.error))
			{
				_errorText = request.error;
				ChartState = ChartStateEnum.Error;
				StopCoroutine("FetchSprintData");
			}

			TeamCollection teams = new TeamCollection();
			try
			{
				teams = TeamCollection.CreateFromJson(request.text);
			}
			catch (Exception e)
			{
				_errorText = e.Message;
				ChartState = ChartStateEnum.Error;
				StopCoroutine("FetchSprintData");
			}

			if (teams == null || !(teams.Teams.Any(x => x.Name.ToLower() == Settings.Team.ToLower())))
			{
				_errorText = "Team does not exist";
				ChartState = ChartStateEnum.Error;
				StopCoroutine("FetchSprintData");
			}

			CommonData.Team = teams.Teams.First(x => x.Name.ToLower() == Settings.Team.ToLower());

			//Get iterations
			request = vstsApi.BuildGetIterationsRequest(CommonData.Project, CommonData.Team);
			yield return request;
			if (!string.IsNullOrEmpty(request.error))
			{
				_errorText = request.error;
				ChartState = ChartStateEnum.Error;
				StopCoroutine("FetchSprintData");
			}

			try
			{
				CommonData.Iterations = BurndownIterationCollection.CreateFromJson(request.text);
			}
			catch (Exception e)
			{
				_errorText = e.Message;
				ChartState = ChartStateEnum.Error;
				StopCoroutine("FetchSprintData");
			}

			if (CommonData.Iterations == null || CommonData.Iterations.Iterations.Count == 0)
			{
				_errorText = "No iterations found";
				ChartState = ChartStateEnum.Error;
				StopCoroutine("FetchSprintData");
			}
			ChartState = ChartStateEnum.FinishedFetchingSprintData;
			StopCoroutine("FetchSprintData");
		}

		private IEnumerator FetchWorkItemData()
		{
			LoadedObjects = false;
			CommonData.LoadedData = false;
			yield return new WaitForSeconds(0.25f);

			LoadSavedDailySummaries();

			//Do fetching
			VstsApi vstsApi = new VstsApi(Settings.UserName, Settings.Token, Settings.Account);

			DateTime now = DateTime.Now.Date;
			//Get daily summary data
			foreach (var iteration in CommonData.Iterations.Iterations)
			{
				DateTime currentDate = iteration.StartDate;
				while (currentDate.Date <= iteration.FinishDate && currentDate.Date <= now.Date)
				{
					if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
					{
						currentDate = currentDate.AddDays(1);
						continue;
					}
					if (_savedDaySummaries.ContainsKey(currentDate.ToString("yyyyMMdd")) &&
					    currentDate.ToString("yyyyMMdd") != now.ToString("yyyyMMdd") &&
					    currentDate.ToString("yyyyMMdd") != now.AddDays(-1).ToString("yyyyMMdd"))
					{
						iteration.DailySummary.Add(_savedDaySummaries[currentDate.ToString("yyyyMMdd")]);
						currentDate = currentDate.AddDays(1);
						continue;
					}

					//Get work item ids in the current iteration for the specified date
					var request = vstsApi.BuildGetWorkItemIdsAsPerDateRequest(CommonData.Project, CommonData.Team,
						iteration, currentDate, "[Id],[Effort],[State],[Work Item Type],[Assigned To]");
					WorkItemIdList workItemIdList = null;
					yield return request;
					if (!string.IsNullOrEmpty(request.error))
					{
						_errorText = request.error;
						ChartState = ChartStateEnum.Error;
						StopCoroutine("FetchWorkItemData");
					}
					try
					{
						workItemIdList = WorkItemIdList.CreateFromJson(request.text);
					}
					catch (Exception e)
					{
						_errorText = e.Message;
						ChartState = ChartStateEnum.Error;
						StopCoroutine("FetchWorkItemData");
					}

					//Get work item details for the specified date
					request = vstsApi.BuildGetWorkItemsAsPerDateRequest(workItemIdList);
					yield return request;
					if (!string.IsNullOrEmpty(request.error))
					{
						_errorText = request.error;
						ChartState = ChartStateEnum.Error;
						StopCoroutine("FetchWorkItemData");
					}
					try
					{
						iteration.DailySummary.Add(DaySummary.CreateFromJson(currentDate, request.text));
					}
					catch (Exception e)
					{
						_errorText = e.Message;
						ChartState = ChartStateEnum.Error;
						StopCoroutine("FetchWorkItemData");
					}
					currentDate = currentDate.AddDays(1);
				}
			}

			SaveDailySummaries();
			CommonData.LoadedData = true;
			ChartState = ChartStateEnum.FinishedFetchingWorkItemData;
			StopCoroutine("FetchWorkItemData");
		}


		private void SaveDailySummaries()
		{
			var recordCount = 0;
			foreach (var iteration in CommonData.Iterations.Iterations)
			{
				recordCount += iteration.DailySummary.Count();
			}

			var filename = string.Format("{0}-summary.dat", CommonData.Team.Name);
			if (File.Exists(filename))
				File.Delete(filename);
			using (BinaryWriter writer = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate)))
			{
				writer.Write(recordCount);

				foreach (var iteration in CommonData.Iterations.Iterations)
				{
					recordCount += iteration.DailySummary.Count;
					foreach (var summary in iteration.DailySummary)
					{
						writer.Write(summary.Date.Ticks);
						writer.Write(summary.WorkItems.Count);
						foreach (var workItem in summary.WorkItems)
						{
							writer.Write(workItem.Id);
							writer.Write((int) workItem.ItemType);
							writer.Write((int) workItem.State);
							writer.Write(workItem.Effort);
						}
					}
				}
			}
		}

		private void LoadSavedDailySummaries()
		{

			_savedDaySummaries = new Dictionary<string, DaySummary>();
			var filename = string.Format("{0}-summary.dat", CommonData.Team.Name);
			if (!File.Exists(filename))
				return;

			using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.OpenOrCreate)))
			{
				var recordCount = reader.ReadInt32();

				for (int i = 0; i < recordCount; i++)
				{
					DaySummary summary = new DaySummary();
					summary.Date = new DateTime(reader.ReadInt64());
					var workItemCount = reader.ReadInt32();
					for (int j = 0; j < workItemCount; j++)
					{
						summary.WorkItems.Add(new WorkItem()
						{
							Id = reader.ReadString(),
							ItemType = (ItemType) reader.ReadInt32(),
							State = (State) reader.ReadInt32(),
							Effort = reader.ReadInt32()
						});
					}
					if (!_savedDaySummaries.ContainsKey(summary.Date.ToString("yyyyMMdd")))
						_savedDaySummaries.Add(summary.Date.ToString("yyyyMMdd"), summary);
				}
			}
		}

		public static void ShowSettings()
		{
			if (ChartState != ChartStateEnum.Settings)
				StateBeforeSettings = ChartState;
			ChartState = ChartStateEnum.Settings;
		}

		void OnGUI()
		{
			if (ChartState == ChartStateEnum.Startup)
			{
				ChartState = ChartStateEnum.FetchingSprintData;
				_loadingText = "Fetching sprint data from Visual Studio Team Services...";
				_loader.Draw(_loadingText);
				StartCoroutine("FetchSprintData");
			}
			else if (ChartState == ChartStateEnum.Error)
			{
				if (string.IsNullOrEmpty(_errorText))
					_errorText = "Unknown error";
				_loader.Draw(_errorText);
			}
			else if (ChartState == ChartStateEnum.FetchingSprintData)
			{
				_loader.Draw(_loadingText);
			}
			else if (ChartState == ChartStateEnum.FinishedFetchingSprintData)
			{
				ChartState = ChartStateEnum.FetchingSprintData;
				_loadingText = "Fetching work item data from Visual Studio Team Services...";
				_loader.Draw(_loadingText);
				StartCoroutine("FetchWorkItemData");
			}
			else if (ChartState == ChartStateEnum.FetchingSprintData)
			{
				_loader.Draw(_loadingText);
			}
			else if (ChartState == ChartStateEnum.FinishedFetchingWorkItemData)
			{
				ChartState = ChartStateEnum.CreatingObjects;
				_loadingText = "Creating objects...";
				_loader.Draw(_loadingText);
				StartCoroutine("CreateObjects");
			}
			else if (ChartState == ChartStateEnum.FetchingWorkItemData)
			{
				_loader.Draw(_errorText);
			}
			else if (ChartState == ChartStateEnum.Main)
			{
				if (!string.IsNullOrEmpty(BurndownChart.SelectedDayKey))
				{
					GUILayout.BeginArea(new Rect(10f, 10f, Screen.width * 0.3f, Screen.height - 20f));
					GUILayout.BeginVertical("box");
					GUILayout.Label(_barSummaryText[BurndownChart.SelectedDayKey]);
					GUILayout.EndVertical();
					GUILayout.EndArea();
				}
			}
			else if (ChartState == ChartStateEnum.Settings)
			{
				settingsScreen.draw();
			}
		}
	}
}