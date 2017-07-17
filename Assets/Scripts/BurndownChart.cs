using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Vsts;
using Assets.Scripts.Vsts.Models;

public class BurndownChart : MonoBehaviour
{
	public static ChartStateEnum ChartState = ChartStateEnum.Startup;
	public static bool LoadedObjects = false;
	public static bool LoadedData = false;

	private static string _loadingText = "";
	private static string _errorText = "";

	private Loader _loader = new Loader();

	public static Team Team;
	public static Project Project;
	public static BurndownIterationCollection Iterations;
	private static Dictionary<string, DaySummary> SavedDaySummaries;
	public static Dictionary<string, string> BarSummaryText;

	SettingsScreen settingsScreen = new SettingsScreen();
	public static ChartStateEnum StateBeforeSettings = ChartStateEnum.Startup;

	public static string SelectedDayKey = "";
	
	// Update is called once per frame
	void Update()
	{

	}

	void Start()
	{
		Settings.LoadSettings();
		SavedDaySummaries = new Dictionary<string, DaySummary>();

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

	private void CreateGameObjects()
	{
		DeleteGameObjects();

		int zOffset = Iterations.Iterations.Count * -1;
		int count = 0;
		float xOrigOffset = Iterations.Iterations.Count > 0 ? Iterations.Iterations[0].DailySummary.Count / -2f : 0f;

		BarSummaryText = new Dictionary<string, string>();
		string summaryBaseText = "{0}\r\nIteration:\t\t{1}\r\nDate:\t\t{2}\r\nTotal Items:\t{3}\r\nItems Done:\t{8}\r\nItems Remaining:\t{4}\r\nTotal Story Points:\t{5}\r\nStory Points Done:\t{7}\r\nStory Points Rem.:\t{6}";

		foreach (var iteration in Iterations.Iterations)
		{
			float xOffset = xOrigOffset;
			foreach (var day in iteration.DailySummary)
			{
				string barKey = string.Format("barsummary-{0}", count);
				BarSummaryText.Add(string.Format("barsummary-{0}", count),
					string.Format(summaryBaseText, Team.Name, iteration.Name, day.Date.ToString("yyyy-MM-dd"), day.GetTotalItems(), day.GetOpenItems(), day.GetTotalStoryPoint(), day.GetOpenStoryPoints(), day.GetTotalStoryPoint() - day.GetOpenStoryPoints(), day.GetTotalItems() - day.GetOpenItems()));

				float height = 0;
				if (Settings.ChartType == ChartType.BurnupItems)
				{
					height = day.GetTotalItems() - day.GetOpenItems();
				}
				else if (Settings.ChartType == ChartType.BurnupStoryPoints)
				{
					height = day.GetOpenStoryPoints();
				}
				else if (Settings.ChartType == ChartType.BurndownItems)
				{
					height = day.GetOpenItems();
				}
				else if (Settings.ChartType == ChartType.BurnupItems)
				{
					height = day.GetTotalItems() - day.GetOpenItems();
				}

				if (height == 0)
					height = 0.1f;

				var barHeight = (height) / 4f;


				var bar = (GameObject)Instantiate(Resources.Load("Bar"), new Vector3(xOffset, barHeight / 2f, zOffset), Quaternion.identity);
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
		BurndownChart.LoadedObjects = true;
		ChartState = ChartStateEnum.Main;
		StopCoroutine("CreateObjects");
	}

	private IEnumerator FetchSprintData()
	{
		LoadedObjects = false;
		LoadedData = false;
		yield return new WaitForSeconds(0.25f);

		DeleteGameObjects();

		//Do fetching
		VstsApi vstsApi = new VstsApi(Settings.UserName, Settings.Token, Settings.Account);

		//Get projects
		WWW request = vstsApi.BuildGetProjectsRequest();
		yield return request;
		if (request.error != null)
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
		BurndownChart.Project = projects.Projects.First(x => x.Name.ToLower() == Settings.Project.ToLower());
		
		//Get teams
		request = vstsApi.BuildGetTeamsRequest(BurndownChart.Project);
		yield return request;
		if (request.error != null)
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

		BurndownChart.Team = teams.Teams.First(x => x.Name.ToLower() == Settings.Team.ToLower());
		
		//Get iterations
		request = vstsApi.BuildGetIterationsRequest(BurndownChart.Project, BurndownChart.Team);
		yield return request;
		if (request.error != null)
		{
			_errorText = request.error;
			ChartState = ChartStateEnum.Error;
			StopCoroutine("FetchSprintData");
		}

		try
		{
			BurndownChart.Iterations = BurndownIterationCollection.CreateFromJson(request.text);
		}
		catch (Exception e)
		{
			_errorText = e.Message;
			ChartState = ChartStateEnum.Error;
			StopCoroutine("FetchSprintData");
		}

		if (BurndownChart.Iterations == null || BurndownChart.Iterations.Iterations.Count == 0)
		{
			_errorText = "No iterations found";
			ChartState = ChartStateEnum.Error;
			StopCoroutine("FetchData");
		}
		ChartState = ChartStateEnum.FinishedFetchingSprintData;
		StopCoroutine("FetchSprintData");
	}

	private IEnumerator FetchWorkItemData()
	{
		LoadedObjects = false;
		LoadedData = false;
		yield return new WaitForSeconds(0.25f);

		LoadSavedDailySummaries();

		//Do fetching
		VstsApi vstsApi = new VstsApi(Settings.UserName, Settings.Token, Settings.Account);

		DateTime now = DateTime.Now.Date;
		//Get daily summary data
		foreach (var iteration in Iterations.Iterations)
		{
			DateTime currentDate = iteration.StartDate;
			while (currentDate.Date <= iteration.FinishDate && currentDate.Date <= now.Date)
			{
				if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
				{
					currentDate = currentDate.AddDays(1);
					continue;
				}
				if (SavedDaySummaries.ContainsKey(currentDate.ToString("yyyyMMdd")) && currentDate.ToString("yyyyMMdd") != now.ToString("yyyyMMdd") && currentDate.ToString("yyyyMMdd") != now.AddDays(-1).ToString("yyyyMMdd"))
				{
					iteration.DailySummary.Add(SavedDaySummaries[currentDate.ToString("yyyyMMdd")]);
					currentDate = currentDate.AddDays(1);
					continue;
				}

				//Get work item ids in the current iteration for the specified date
				var request = vstsApi.BuildGetWorkItemIdsAsPerDateRequest(BurndownChart.Project, BurndownChart.Team,
					iteration, currentDate, "[Id],[Effort],[State],[Work Item Type],[Assigned To]");
				WorkItemIdList workItemIdList = null;
				yield return request;
				if (request.error != null)
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
				if (request.error != null)
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
		LoadedData = true;
		ChartState = ChartStateEnum.FinishedFetchingWorkItemData;
		StopCoroutine("FetchWorkItemData");
	}


	private void SaveDailySummaries()
	{
		var recordCount = 0;
		foreach (var iteration in Iterations.Iterations)
		{
			recordCount += iteration.DailySummary.Count();
		}

		var filename = string.Format("{0}-summary.dat", BurndownChart.Team.Name);
		if (File.Exists(filename))
			File.Delete(filename);
		using (BinaryWriter writer = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate)))
		{
			writer.Write(recordCount);

			foreach (var iteration in Iterations.Iterations)
			{
				recordCount += iteration.DailySummary.Count;
				foreach (var summary in iteration.DailySummary)
				{
					writer.Write(summary.Date.Ticks);
					writer.Write(summary.WorkItems.Count);
					foreach (var workItem in summary.WorkItems)
					{
						writer.Write(workItem.Id);
						writer.Write((int)workItem.ItemType);
						writer.Write((int)workItem.State);
						writer.Write(workItem.Effort);
					}
				}
			}
		}
	}

	private void LoadSavedDailySummaries()
	{

		SavedDaySummaries = new Dictionary<string, DaySummary>();
		var filename = string.Format("{0}-summary.dat", BurndownChart.Team.Name);
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
				if (!SavedDaySummaries.ContainsKey(summary.Date.ToString("yyyyMMdd")))
					SavedDaySummaries.Add(summary.Date.ToString("yyyyMMdd"), summary);
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
				GUILayout.Label(BurndownChart.BarSummaryText[BurndownChart.SelectedDayKey]);
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