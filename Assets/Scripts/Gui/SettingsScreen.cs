using UnityEngine;

namespace Assets.Scripts.Gui
{
	public class SettingsScreen
	{
		//public 
		//Optional Parameters
		public string name = "SettingsScreen";

		//GUI Options
		public GUISkin guiSkin; //The GUISkin to use

		public bool isVisible
		{
			get { return visible; }
		}

		//GUI
		protected Color defaultColor;

		protected int layout;
		protected Rect guiSize;
		protected GUISkin oldSkin;
		protected bool visible = false;

		//Constructors
		public SettingsScreen()
		{
			guiSize = new Rect(Screen.width * 0.125f, Screen.height * 0.125f, Screen.width * 0.75f, Screen.height * 0.75f);
		}

		public SettingsScreen(Rect guiRect) : this()
		{
			guiSize = guiRect;
		}


		public void setGUIRect(Rect r)
		{
			guiSize = r;
		}


		public void draw()
		{

			if (guiSkin)
			{
				oldSkin = GUI.skin;
				GUI.skin = guiSkin;
			}
			GUILayout.BeginArea(guiSize);
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("SprintView 3D");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("A frivolous and overly overengineered sprint burndown tool");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("written by Serge Meunier");
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Chart Type");
			Settings.ChartType = (ChartType) GUILayout.SelectionGrid((int) Settings.ChartType, Settings.ChartTypeText, 1);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Instructions");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Rotate:\t\tLeft-click while moving mouse mouse");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Pan:\t\tRight-click while moving mouse mouse");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Forward/Back:\tUp/Down arrow keys");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Left/Right:\t\tLeft/Right arrow keys");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Esc:\t\tSettings menu");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("R:\t\tReset camera to start position");
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			bool exitClicked = GUILayout.Button("Exit");
			bool cancelClicked = GUILayout.Button("Cancel");
			bool saveClicked = GUILayout.Button("Save");
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();

			visible = true;
			if (guiSkin)
			{
				GUI.skin = oldSkin;
			}

			if (exitClicked)
			{
				Application.Quit();
			}
			if (saveClicked)
			{
				Settings.SaveSettings();
				BurndownChart.ChartState = ChartStateEnum.FinishedFetchingWorkItemData;
				BurndownChart.LoadedObjects = false;
			}
			if (cancelClicked)
			{
				BurndownChart.ChartState = ChartStateEnum.Main;
			}
		}

		public override string ToString()
		{
			return "Name: " + name + "\nVisible: " + isVisible.ToString() + "\nGUI Size: " + guiSize.ToString();
		}
	}
}
