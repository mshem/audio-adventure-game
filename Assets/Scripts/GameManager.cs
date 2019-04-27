using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.IO;

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
            Stories = JsonUtility.FromJson<Dictionary<int, CurrentEvent>>(jsonString);
        }
        //var 
        currentPassage = Stories[firstPassage];

        myState.Subscribe(state =>
        {
            switch (state)
            {
                case MyState.DoNothing:
                    //Do nothing
                    break;
                case MyState.TriggerAlienDialog:
                    //fetch current state
                    //trigger soudQueueCoroutinen w/ current state (within here it will add to sound queue or trigger playerawaitactionstate
                    //set MyState.DoNothing 
                    StartCoroutine(TriggerStoryEvent()); 
                    break;
                case MyState.TriggerPlayerAwaitAction:
                    Debug.Log("waiting for player");
                    break;
            }
        });
        myState.Publish(MyState.TriggerAlienDialog);
    }

    IEnumerator TriggerStoryEvent()
    {
        // play audio file
        Queue<SoundQueue> soundQ = this.currentPassage.SoundQueue;
        while (soundQ.Count > 0)
        {
            var currentSound = soundQ.Dequeue();
            var soundLoc = currentSound.FileLoc;
            var obj = GameObject.Find(currentSound.TransformObject);
            var obj1 = obj.GetComponent<AudioSource>();
            obj1.clip = Resources.Load<AudioClip>(soundLoc);
            obj1.Play();
            yield return new WaitForSeconds(obj1.clip.length);
        }
        if(currentPassage.Goto != null) 
        {
            this.currentPassage = this.Stories[currentPassage.Goto.Value];
        }
        else 
        {
            this.myState.Publish(MyState.TriggerPlayerAwaitAction);
        }
    }
}
