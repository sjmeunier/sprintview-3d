using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SettingsScreen
{
	//public 
	//Optional Parameters
	public string name = "SettingsScreen";
	//GUI Options
	public GUISkin guiSkin; //The GUISkin to use

	public bool isVisible { get { return visible; } }

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
	public SettingsScreen(Rect guiRect) : this() { guiSize = guiRect; }


	public void setGUIRect(Rect r) { guiSize = r; }


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
		GUILayout.Label("Chart Type");
		Settings.ChartType = (ChartType)int.Parse(GUILayout.TextField(((int)Settings.ChartType).ToString()));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		bool exitClicked = GUILayout.Button("Exit");
		bool saveClicked = GUILayout.Button("Save");
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndArea();

		visible = true;
		if (guiSkin) { GUI.skin = oldSkin; }

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

	}
	public override string ToString()
	{
		return "Name: " + name + "\nVisible: " + isVisible.ToString() + "\nGUI Size: " + guiSize.ToString();
	}
}

