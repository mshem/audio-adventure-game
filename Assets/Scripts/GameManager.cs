using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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
    Subject<MyState> myState = new Subject<MyState>();

    // Start is called before the first frame update
    void Start()
    {
        //jsonutil load file
        //var file = loadFile
        //var 

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
                    break;
                case MyState.TriggerPlayerAwaitAction:
                    break;
            }
        });
        myState.Publish(MyState.TriggerAlienDialog);
    }
}
