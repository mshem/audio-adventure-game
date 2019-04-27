using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
/// <summary>
/// This is a demo for selecting different kinds of text to speech engine that is installed on your phone.
/// </summary>
public class DemoEasyTTS_SelectEngine : MonoBehaviour
{

	private string stringToEdit = "In this demo you can know your different text to speech engine" +
		" that's been \ninstalled on your phone. You can hear the differences on how each \n" +
			"text to speech engine pronunciation and such.\nKindly choose either of SelectList or OpenTTSSettings" +
			" to find out.";
	private bool selecting = false;
	private string engineName = "";
	private string enginePkg ="";



	void OnGUI ()
	{
		// Make a group on the center of the screen
		GUI.BeginGroup (new Rect (Screen.width / 2 - 250, Screen.height / 2 - 250, 1000, 1000));
		
		GUI.Box (new Rect (0,0,500,500), "\tSelect Engine Demo");
		
		
		stringToEdit = GUI.TextField (new Rect (30,110, 440, 100), stringToEdit, 400);
		if (GUI.Button (new Rect (30,210, 440, 40), "Speak")) {
			EasyTTSUtil.SpeechAdd (stringToEdit);
		}
		if (GUI.Button (new Rect (30,320,440, 40), "SelectList")) {
			selecting = true;
		}
		
		if (GUI.Button (new Rect (30,360, 440, 40), "OpenTTSSetting")) {
			EasyTTSUtil.OpenTTSSetting ();
		}

		
		if (selecting) {

			string[] nameArray = EasyTTSUtil.GetEngineNameArray ();

			int selected = -1;
			selected = GUILayout.SelectionGrid (selected, nameArray, 1);
			if (selected != -1) {

				string[] pkgArray = EasyTTSUtil.GetEnginePkgArray ();
				enginePkg = pkgArray [selected];
				engineName = nameArray [selected];
				EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates,enginePkg);
				selecting = false;
			}
		}
		if (engineName == null) {
			engineName = "Your Text to Speech Engine will appear here after you press the button\n OpenTTSSetting";
		}
		GUI.TextField (new Rect (30,400, 440, 40), engineName);
		
		GUI.EndGroup ();

	}

	void Start(){
		EasyTTSUtil.Initialize (EasyTTSUtil.UnitedStates);
		engineName = EasyTTSUtil.GetDefaultEngineName ();
	}
	
	void OnApplicationQuit ()
	{
		EasyTTSUtil.Stop ();
	}
}
