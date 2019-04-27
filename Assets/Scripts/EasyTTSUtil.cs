using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// EasyTTSUTil is the class where you can see all the codes that you will used in order to make text to speech work
/// on your device. This is proven to run with the platform such as iOS, Android and Amazon. 
/// Please visit our site on how to use it.
/// http://frecre.com/our-products-in-assetstore/easygoogleanalytics/
/// </summary>
public class EasyTTSUtil
{
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void  EasyTTSUtilSpeech(string text,string local);
	[DllImport("__Internal")]
	private static extern void EasyTTSUtilStop();
	[DllImport("__Internal")]
	private static extern void  EasyTTSUtilSpeechAd(string text,string local,float volume,float rate,float pitch);

	public static string localSetting = UnitedStates;

	#endif

	/// <summary>
	/// StopSpeech is a function where you can stop the text to speech at anytime without exiting your game.
	/// <example> Example Code
	/// <code>if (GUI.Button (new Rect (30, 310, 440, 40), "Stop")) {
	/// EasyTTSUtil.StopSpeech ();
	/// }</code>
	/// </example>
	/// </summary>
	public static void StopSpeech(){
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		cls_jni.CallStatic("stop",obj_Activity);
		#elif UNITY_IPHONE && !UNITY_EDITOR
		EasyTTSUtilStop();
		#endif
	}

	/// <summary>
	/// SpeechAdd is a function where you will add the text you want to make as text to speech.
	/// This is a kind of reading that needs to be executed all the words that you input before repeating it, in case you double tap
	/// the button for it.
	/// <example>Example code
	/// <code>if (GUI.Button (new Rect (30, 230, 440, 40), "Speak")) {
	/// EasyTTSUtil.SpeechAdd (stringToEdit);
	/// }</code>
	/// </example>
	/// </summary>
	/// <param name="text">The text or message that you want to add in order to read on text to speech.</param>
	public static void SpeechAdd(string text)
	{
		if (string.IsNullOrEmpty (text)) {
			return;
		}
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		cls_jni.CallStatic("speechAdd",obj_Activity,text);
		#elif UNITY_IPHONE && !UNITY_EDITOR
		EasyTTSUtilSpeech(text,localSetting);
		#endif
	}
	/// <summary>
	/// SpeechAdd is a function where you will add the text you want to make as text to speech.
	/// This is a kind of reading that needs to be executed all the words that you input before repeating it, in case you double tap
	/// the button for it.
	/// <example>Example code
	/// <code>if (GUI.Button (new Rect (30, 230, 440, 40), "Speak")) {
	/// EasyTTSUtil.SpeechAdd (stringToEdit);
	/// }</code>
	/// </example>
	/// </summary>
	/// <param name="text">The text or message that you want to add in order to read on text to speech.</param>
	/// <param name="volume">speech volume 0~1(1 is normal) if 0,it will be used before setting</param>
	/// <param name="rate">speech rate 0~1(0.5 is normal) if 0,it will be used before setting</param>
	/// <param name="pitch">speech pitch 0.5~2(1 is normal) if 0,it be used before setting</param>
	public static void SpeechAdd(string text,float volume,float rate,float pitch)
	{
		if (string.IsNullOrEmpty (text)) {
			return;
		}
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		cls_jni.CallStatic("speechAdd",obj_Activity,text,volume,rate*2,pitch);
		#elif UNITY_IPHONE && !UNITY_EDITOR
		EasyTTSUtilSpeechAd(text,localSetting,volume,rate,pitch);
		#endif
	}
	/// <summary>
	/// SpeechFlush is a function that is SpeechAdd but somewhat different. As we've written above, SpeechAdd needs
	/// to perform all the message or words that's been inputed before repeating it. Unlike SpeechFlush it fo back to
	/// first word of your message when you tap the button for it.
	/// <example>Example Code
	/// <code>
	/// if (GUI.Button (new Rect (30, 270, 440, 40), "Repeat")) {
	/// EasyTTSUtil.SpeechFlush (stringToEdit);
	///  }
	/// </code>
	/// </example>
	/// </summary>
	/// <param name="text">The text or message that needs to be read.</param>
	/// <param name="volume">speech volume 0~1(1 is normal) if 0,it will be used before setting</param>
	/// <param name="rate">speech rate 0~1(0.5 is normal) if 0,it will be used before setting</param>
	/// <param name="pitch">speech pitch 0.5~2(1 is normal) if 0,it be used before setting</param>
	public static void SpeechFlush(string text,float volume,float rate,float pitch)
	{
		if (string.IsNullOrEmpty (text)) {
			return;
		}
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		cls_jni.CallStatic("speechFrush",obj_Activity,text,volume,rate*2,pitch);
		#elif UNITY_IPHONE && !UNITY_EDITOR
		EasyTTSUtilStop();
		EasyTTSUtilSpeechAd(text,localSetting,volume,rate,pitch);
		#endif
	}

	/// <summary>
	/// SpeechFlush is a function that is SpeechAdd but somewhat different. As we've written above, SpeechAdd needs
	/// to perform all the message or words that's been inputed before repeating it. Unlike SpeechFlush it fo back to
	/// first word of your message when you tap the button for it.
	/// <example>Example Code
	/// <code>
	/// if (GUI.Button (new Rect (30, 270, 440, 40), "Repeat")) {
	/// EasyTTSUtil.SpeechFlush (stringToEdit);
	///  }
	/// </code>
	/// </example>
	/// </summary>
	/// <param name="text">The text or message that needs to be read.</param>
	public static void SpeechFlush(string text)
	{
		if (string.IsNullOrEmpty (text)) {
			return;
		}
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		cls_jni.CallStatic("speechFrush",obj_Activity,text);
		#elif UNITY_IPHONE && !UNITY_EDITOR
		EasyTTSUtilStop();
		EasyTTSUtilSpeech(text,localSetting);
		#endif
	}
	
	// -------Beyond this area is for Android Platform use only -------
	
	/// <summary>
	/// Initialize the specified local and enginePkg.
	/// </summary>
	/// <param name="local">Local is the initialize engine package</param>
	/// <param name="enginePkg">EnginePkg is a the different kind of languages</param>
	public static void Initialize(string local = UnitedStates,string enginePkg = null)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		if(enginePkg==null){
		cls_jni.CallStatic("initialize",obj_Activity,local);
		}else{
		cls_jni.CallStatic("initialize",obj_Activity,local,enginePkg);
		}
		#elif UNITY_IPHONE && !UNITY_EDITOR
		localSetting = local;
		#endif
	}
	
	/// <summary>
	/// OpenTTSSetting is a function in where you can select the text to speech engine that is installed on your phone.
	/// And this is for Android use only.
	/// <example>Example Code
	/// <code>
	/// if (GUI.Button (new Rect (30,360, 440, 40), "OpenTTSSetting")) {
	/// EasyTTSUtil.OpenTTSSetting ();
	/// }
	/// </code>
	/// </example>
	/// </summary>
	public static void OpenTTSSetting()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		cls_jni.CallStatic("openTTSSetting",obj_Activity);
		#endif
	}

	/// <summary>
	/// Stop is a function where your text to speech will totally stop once user quit the application.
	/// <example>
	/// <code>
	/// void OnApplicationQuit ()
	/// {
	///	EasyTTSUtil.Stop ();
	/// }
	/// </code>
	/// </example>
	/// </summary>
	public static void Stop() 
	{
		#if UNITY_ANDROID && !UNITY_EDITOR  
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		cls_jni.CallStatic("shotDown");
		#endif
	}
	

	/// <summary>
	/// GetEnginePkgArray is a function where you can get all the list of text to speech that is installed on your phone.
	/// For Android over 14 only, if it isn't then it will return to null.
	/// Also please intialize it first before using.
	/// <example>Example Code
	/// <code>
	/// private string engineName = "";
	/// private string enginePkg ="";
	/// if (selected != -1) {
	///	string[] pkgArray = EasyTTSUtil.GetEnginePkgArray ();
	///	enginePkg = pkgArray [selected];
	///	engineName = nameArray [selected];
	///	EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates,enginePkg);
	///	selecting = false;
	/// }
	/// if (GUI.Button (new Rect (30,320,440, 40), "SelectList")) {
	/// selecting = true;
	/// }
	/// </code>
	/// </example>
	/// </summary>
	/// <returns>The engine package array.</returns>
	public static string[] GetEnginePkgArray() 
	{
		#if UNITY_ANDROID && !UNITY_EDITOR  
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		return cls_jni.CallStatic<string[]>("getEnginePkgArray");
		#endif
		Debug.LogError ("GetEnginePkgArray is only for android");
		return null;
	}
	

	/// <summary>
	/// GetEngineNameArray is the same with GetEnginePkgArray, yet the GetEngineNameArray well have the complete form of Engine name of
	/// text to speech function.
	/// For Android over 14 only, if it isn't then it will return to null.
	/// Also please intialize it first before using.
	/// <example> Example Code
	/// <code>
	/// private string engineName = "";
	/// string[] nameArray = EasyTTSUtil.GetEngineNameArray ();
	/// if (selected != -1) {
	/// string[] pkgArray = EasyTTSUtil.GetEnginePkgArray ();
	/// enginePkg = pkgArray [selected];
	/// engineName = nameArray [selected];
	/// EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates,enginePkg);
	/// selecting = false;
	/// }
	/// </code>
	/// </example>
	/// </summary>
	/// <returns>The engine name array.</returns>
	public static string[] GetEngineNameArray() 
	{
		#if UNITY_ANDROID && !UNITY_EDITOR  
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		return cls_jni.CallStatic<string[]>("getEngineNameArray");
		#endif
		Debug.LogError ("GetEngineNameArray is only for android");
			               
		return null;
	}


	/// <summary>
	/// SetEngineByPackageName is a function where you can set the engine name by it's package.
	/// For Android over 14 only, if it isn't then it will return to null.
	/// Also please intialize it first before using.
	/// </summary>
	/// <param name="pkg">Engine package name</param>
	public static void SetEngineByPackageName(string pkg) 
	{
		#if UNITY_ANDROID && !UNITY_EDITOR  
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		cls_jni.CallStatic("setEngineByPackageName",pkg);
		#endif
		Debug.LogError ("SetEngineByPackageName is only for android");
	}

	/// <summary>
	/// Gets the default name of the engine.
	/// </summary>
	/// <returns>The default engine name.</returns>
	public static string GetDefaultEngineName() 
	{
		#if UNITY_ANDROID && !UNITY_EDITOR  
		AndroidJavaClass cls_jni = new AndroidJavaClass("com.frecre.plugin.EasyTTSUtil");
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		return cls_jni.CallStatic<string>("getDefaultEngineName");
		#endif
		Debug.LogError ("GetEngineNameArray is only for android");
		
		return null;
	}


	
	//http://www.mathguide.de/info/tools/languagecode.html
	//ISO laungagecode 639-1
	
	
	public const string SaudiArabia = "ar-SA";
	public const string SouthAfrica = "en-ZA";
	public const string Thailand = "th-TH";
	public const string Belgium = "nl-BE";
	public const string Australia = "en-AU";
	public const string Germany = "de-DE";
	public const string UnitedStates = "en-US";
	public const string Brazil = "pt-BR";
	public const string Poland = "pl-PL";
	public const string Ireland = "en-IE";
	public const string Greece = "el-GR";
	public const string Indonesia = "id-ID";
	public const string Sweden = "sv-SE";
	public const string Turkey = "tr-TR";
	public const string Portugal = "pt-PT";
	public const string Japan = "ja-JP";
	public const string Korea = "ko-KR";
	public const string Hungary = "hu-HU";
	public const string CzechRepublic = "cs-CZ";
	public const string Denmark = "da-DK";
	public const string Mexico = "es-MX";
	public const string Canada = "fr-CA";
	public const string Netherlands = "nl-NL";
	public const string Finland = "fi-FI";
	public const string Spain = "es-ES";
	public const string Italy = "it-IT";
	public const string Romania = "ro-RO";
	public const string Norway = "no-NO";
	public const string HongKong = "zh-HK";
	public const string Taiwan = "zh-TW";
	public const string Slovakia = "sk-SK";
	public const string China = "zh-CN";
	public const string Russia = "ru-RU";
	public const string UnitedKingdom = "en-GB";
	public const string France = "fr-FR";
	public const string India = "hi-IN";


}
