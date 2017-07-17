using UnityEngine;

public class Loader {
//public 
	//Optional Parameters
	public string Name = "Loader"; 
	//GUI Options
	public GUISkin GuiSkin; //The GUISkin to use

	public bool IsVisible{	get{	return _visible;	}	} 

	//GUI
	private Color _defaultColor;
	private int _layout;
	private Rect _guiSize;
	private GUISkin _oldSkin;
	private bool _visible = false;
	
	//Constructors
	public Loader(){
        _guiSize = new Rect(Screen.width * 0.125f, Screen.height * 0.125f, Screen.width * 0.75f, Screen.height * 0.75f);
    }
	public Loader(Rect guiRect):this(){	_guiSize = guiRect;	}

	
	public void SetGuiRect(Rect r){	_guiSize=r;	}
	
	
	public void Draw(string text){
		if(GuiSkin){
			_oldSkin = GUI.skin;
			GUI.skin = GuiSkin;
		}
		GUILayout.BeginArea(_guiSize);
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label(text);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndArea();
        _visible = true;
		if(GuiSkin){GUI.skin = _oldSkin;}
	}
	


	//to string
	public override string ToString(){
		return "Name: " + Name + "\nVisible: " + IsVisible.ToString() + "\nGUI Size: " + _guiSize.ToString();
	}
}

