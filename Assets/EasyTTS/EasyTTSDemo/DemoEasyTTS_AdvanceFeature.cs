using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
/// <summary>
/// This is the demo for EasyTTS on using SpeechFlush, SpeechAdd and StopSpeech.
/// Also how to initialize what kind of language you want to use and how to totally quit it.
/// </summary>
public class DemoEasyTTS_AdvanceFeature: MonoBehaviour {

	private string stringToEdit = "Thank you for purchasing our Easy TTS plugin. Please enjoy!!";
	float volume = 1f;
	float pitch = 1f;
	float rate = 0.5f;

	void Start(){
		float screenRate = (float)600 / Screen.width;
		if( screenRate > 1 ) screenRate = 1;
		int width = (int)(Screen.width * screenRate);
		int height = (int)(Screen.height * screenRate);
		Screen.SetResolution( width , height, true, 15);
	}

	void OnGUI ()
	{
		GUI.BeginGroup (new Rect (Screen.width / 2 - 250, Screen.height / 2 - 250, 1100, 1000));
		GUI.Box (new Rect (0, 0, 500, 450), "EasyTTS Demo");
		stringToEdit = GUI.TextField (new Rect (30, 20, 440, 160), stringToEdit, 600); 

		volume = GUI.HorizontalSlider (new Rect (90,200,300,20), volume, 0.01f,1);
		GUI.Label (new Rect (40, 195, 300, 40), "Volume");
		GUI.Label (new Rect (400, 195, 300, 40), volume+"%");

		pitch = GUI.HorizontalSlider (new Rect (90,225,300,20), pitch, 0.5f,2);
		GUI.Label (new Rect (40,222,300,40), "Pitch");
		GUI.Label (new Rect (400,222,300,40), pitch+"%");


		rate = GUI.HorizontalSlider (new Rect (90,250,300,20), rate, 0.01f,1);
		GUI.Label (new Rect (40,245,300,40), "Rate");
		GUI.Label (new Rect (400, 245, 300, 40),rate+"%");

		if (GUI.Button (new Rect (30, 275, 440, 40), "Speak")) {
			EasyTTSUtil.SpeechAdd (stringToEdit,volume,rate,pitch); 

		} else if (GUI.Button (new Rect (30, 320, 440, 40), "Repeat")) {
			EasyTTSUtil.SpeechFlush (stringToEdit,volume,rate,pitch); 
		} else if (GUI.Button (new Rect (30, 365, 440, 40), "Stop")) {
			EasyTTSUtil.StopSpeech ();
		} else if (GUI.Button (new Rect (30, 410, 440, 40), "Clear")) {
			stringToEdit = "";
		}

		GUI.Label (new Rect (30, 460, 440, 100), "Stop and Repeat button only works once build on mobile iOS or Android ");

		GUI.EndGroup ();

	}


	void OnApplicationQuit() 
	{
		EasyTTSUtil.Stop ();
	}
}
