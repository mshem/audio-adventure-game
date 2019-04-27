using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.IO;
using Newtonsoft.Json;

public enum MyState 
{
    DoNothing,
    TriggerAlienDialog,
    TriggerPlayerAwaitAction  
}

public class CurrentEvent
{
    public Queue<SoundQueue> SoundQueue;
    public int? Goto;
    public KeyValuePair<bool, int> Decision;
}

public class SoundQueue
{
    public bool IsRepeat;
    public string FileLoc;
    public bool IsBlocking;
    public string TransformObject;
}

public class GameManager : MonoBehaviour
{
    int firstPassage = 0;
    Dictionary<int, CurrentEvent> Stories;
    CurrentEvent currentPassage;
    Subject<MyState> myState = new Subject<MyState>();

    // Start is called before the first frame update
    void Start()
    {
        //jsonutil load file
        using (StreamReader r = new StreamReader("Assets/zorblok.json"))
        {
            string jsonString = r.ReadToEnd();
            Stories = JsonConvert.DeserializeObject<Dictionary<int, CurrentEvent>>(jsonString);
            //Stories = JsonUtility.FromJson<>(jsonString);
        }
        currentPassage = Stories[firstPassage];
        myState.Subscribe(state =>
        {
            switch (state)
            {
                case MyState.DoNothing:
                    //Do nothing
                    break;
                case MyState.TriggerAlienDialog:
                    StartCoroutine(TriggerStoryEvent());
                    myState.OnNext(MyState.DoNothing); 
                    break;
                case MyState.TriggerPlayerAwaitAction:
                    Debug.Log("waiting for player");
                    break;
            }
        });
        myState.OnNext(MyState.TriggerAlienDialog);
    }

    IEnumerator TriggerStoryEvent()
    {
        // play audio file
        Queue<SoundQueue> soundQ = this.currentPassage.SoundQueue;
        while (soundQ.Count > 0)
        {
            var currentSound = soundQ.Dequeue();
            var soundLoc = "Audio/" + currentSound.FileLoc;
            var audioClip = Resources.Load<AudioClip>(soundLoc);
            var obj = GameObject.Find(currentSound.TransformObject);
            var obj1 = obj.GetComponent<AudioSource>();
            obj1.clip = audioClip;
            obj1.Play();
            yield return new WaitForSeconds(obj1.clip.length);
        }
        if(currentPassage.Goto != null) 
        {
            currentPassage = Stories[currentPassage.Goto.Value];
        }
        else 
        {
            myState.OnNext(MyState.TriggerPlayerAwaitAction);
        }
    }
}
