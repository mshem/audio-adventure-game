using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
/// <summary>
/// This is the demo for EasyTTS on using SpeechFlush, SpeechAdd and StopSpeech.
/// Also how to initialize what kind of language you want to use and how to totally quit it.
/// </summary>
public class DemoEasyTTS : MonoBehaviour {

	private string stringToEdit = "Thank you for purchasing our Easy TTS plugin. Please enjoy!!";

	void OnGUI ()
	{
		 

		GUI.BeginGroup (new Rect (Screen.width / 2 - 250, Screen.height / 2 - 250, 1000, 1000));
		GUI.Box (new Rect (0, 0, 500, 450), "EasyTTS Demo");
		stringToEdit = GUI.TextField (new Rect (30, 30, 440, 160), stringToEdit, 600); 

		if (GUI.Button (new Rect (30, 230, 440, 40), "Speak")) {
			EasyTTSUtil.SpeechAdd (stringToEdit); 

		} else if (GUI.Button (new Rect (30, 275, 440, 40), "Repeat")) {
			EasyTTSUtil.SpeechFlush (stringToEdit);
		} else if (GUI.Button (new Rect (30, 320, 440, 40), "Stop")) {
			EasyTTSUtil.StopSpeech ();
		} else if (GUI.Button (new Rect (30, 365, 440, 40), "Clear")) {
			stringToEdit = "";
		}

		GUI.Label (new Rect (30, 420, 440, 100), "Stop and Repeat button only works once build on mobile iOS or Android ");

		GUI.EndGroup ();

	}

	void Start(){
		EasyTTSUtil.Initialize (EasyTTSUtil.UnitedStates);
	}
	
	void OnApplicationQuit() 
	{
		EasyTTSUtil.Stop ();
	}
}
