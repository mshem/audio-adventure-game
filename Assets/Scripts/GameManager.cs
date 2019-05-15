using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine.UI;
using Bose.Wearable;

public enum MyState
{
    DoNothing,
    TriggerAlienDialog,
    TriggerPlayerAwaitAction
}

public class CurrentEvent
{
    public List<SoundQueue> SoundQueue;
    public int? Goto;
    public Dictionary<string, int> Decision;
}

public class SoundQueue
{
    public bool IsRepeat;
    public string FileLoc;
    public bool IsBlocking;
    public string TransformObject;
    public float? Volume;
}

public class GameManager : MonoBehaviour
{
    public int firstPassage = 0;
    public InputController inputController;
    private bool IsRepeating = false;
    public bool IsGameRunning = false;

    Dictionary<int, CurrentEvent> Stories;
    CurrentEvent currentPassage;
    List<IDisposable> runningSoundQueue = new List<IDisposable>();
    Subject<bool> IsGameLoopRunning = new Subject<bool>();
    Subject<bool> IsBoseHandSetConnected = new Subject<bool>();
    IDisposable stateSub;

    RotationMatcher Matcher;
    WearableControl Control;

    BehaviorSubject<MyState> myState;

    Dictionary<string, GameObject> OnLoopSound = new Dictionary<string, GameObject>();

    public Button ResetButton;
    public Button StartButton;
    public Button PauseButton;


    public bool debug;

    // Start is called before the first frame update
    void Start()
    {
        var boseWearableGameObject = FindObjectOfType<WearableConnectUIPanel>();
        var boseWearable = boseWearableGameObject.GetComponent<WearableConnectUIPanel>();
        boseWearable.DeviceConnectSuccess += this.OnDeviceConnected;
        boseWearable.DeviceDisconnected += this.OnDeviceDisconnected;

        inputController = FindObjectOfType<InputController>();
        string jsonString = (Resources.Load("zorblok") as TextAsset).text;
        Stories = JsonConvert.DeserializeObject<Dictionary<int, CurrentEvent>>(jsonString);
        currentPassage = Stories[firstPassage];

        //button setup
        ResetButton.OnClickAsObservable().Subscribe(_ => { this.Reset(); });
        PauseButton.OnClickAsObservable().Subscribe(_ => { this.Pause(); });
        StartButton.OnClickAsObservable().Subscribe(_ => { this.Resume(); });

        //set initial state
        this.myState = new BehaviorSubject<MyState>(MyState.TriggerAlienDialog);

        var isGameRunnable = Observable.CombineLatest(IsBoseHandSetConnected, IsGameLoopRunning);
        isGameRunnable.Subscribe(isTrue => {
            IsGameRunning = isTrue.All(t => t == true);
        });

        myState.CombineLatest(isGameRunnable, (state, isRun) => 
        {
            var isRunnable = isRun.All(r => r);
            return new Tuple<bool, MyState>(isRunnable, state); 
        })
        .Where(t => t.Item1)
        .Subscribe(t => 
        {
            GameLoop(t.Item2); 
        });


        //kick off the game
        myState.OnNext(MyState.TriggerAlienDialog);
    }

    void GameLoop(MyState state)
    {
        switch (state)
        {
            case MyState.DoNothing:
                //Do nothing
                break;
            case MyState.TriggerAlienDialog:
                Matcher.SetRelativeReference(Control.LastSensorFrame.rotation);
                //need to subscribe to observable from here and perform them one by one
                runningSoundQueue.Add(
                    TriggerStory()
                    .Where(_ => IsGameRunning)
                    .Subscribe(
                        (_)=> { //on subscribe
                            Debug.Log("On going sound queue: " + _);    
                        }, 
                        () => { //on complete
                        if (currentPassage.Goto != null)
                        {
                            currentPassage = Stories[currentPassage.Goto.Value];
                            myState.OnNext(MyState.TriggerAlienDialog);
                        }
                        else if (currentPassage.Decision != null)
                        {
                            myState.OnNext(MyState.TriggerPlayerAwaitAction);
                        }
                        else
                        {
                            //game is done, go back to main menu  
                        }
                }));
                myState.OnNext(MyState.DoNothing);
                break;
            case MyState.TriggerPlayerAwaitAction:
                IsRepeating = false;
                Debug.Log("waiting for player");
                Matcher.SetRelativeReference(Control.LastSensorFrame.rotation);
                inputController.playerResponseState.Where(e =>
                {
                    if (currentPassage.Decision.Keys.Any(k => k == "yes"))
                    {
                        return e == InputController.InputState.Yes || e == InputController.InputState.No;
                    }
                    else
                    {
                        return e == InputController.InputState.Left || e == InputController.InputState.Right;
                    }
                })
                .First()
                .Subscribe(e =>
                {
                    var d = currentPassage.Decision.Keys.Any(k => k == "yes")
                        ? e == InputController.InputState.Yes ? "yes" : "no"
                        : e == InputController.InputState.Left ? "left" : "right";
                    var id = currentPassage.Decision[d];
                    currentPassage = Stories[id];
                    myState.OnNext(MyState.TriggerAlienDialog);
                });
                inputController.playerResponseState
                   .Where(e => e == InputController.InputState.DoubleTap)
                   .First()
                   .Subscribe(_ =>
                   {
                       IsRepeating = true;
                       myState.OnNext(MyState.TriggerAlienDialog);
                   });

                break;
        }
    }

    //void RunLoop()
    //{
    //    Debug.Log("Starting loop");
    //    Bose.Wearable.RotationMatcher matcher = GameObject.FindObjectOfType<Bose.Wearable.RotationMatcher>();
    //    Bose.Wearable.WearableControl control = GameObject.FindObjectOfType<Bose.Wearable.WearableControl>();
    //    //myState.First().Subscribe(state => {
    //    //    matcher.SetRelativeReference(control.LastSensorFrame.rotation);
    //    //});

    //    stateSub = myState.Subscribe(state =>
    //    {
    //        switch (state)
    //        {
    //            case MyState.DoNothing:
    //                //Do nothing
    //                break;
    //            case MyState.TriggerAlienDialog:
    //                matcher.SetRelativeReference(control.LastSensorFrame.rotation);
    //                 TriggerStory().Subscribe();
    //                myState.OnNext(MyState.DoNothing);
    //                break;
    //            case MyState.TriggerPlayerAwaitAction:
    //                IsRepeating = false;
    //                Debug.Log("waiting for player");
    //                matcher.SetRelativeReference(control.LastSensorFrame.rotation);
    //                inputController.playerResponseState.Where(e =>
    //                {
    //                    if (currentPassage.Decision.Keys.Any(k => k == "yes"))
    //                    {
    //                        return e == InputController.InputState.Yes || e == InputController.InputState.No;
    //                    }
    //                    else
    //                    {
    //                        return e == InputController.InputState.Left || e == InputController.InputState.Right;
    //                    }
    //                })
    //                .First()
    //                .Subscribe(e =>
    //                {
    //                    var d = currentPassage.Decision.Keys.Any(k => k == "yes")
    //                        ? e == InputController.InputState.Yes ? "yes" : "no"
    //                        : e == InputController.InputState.Left ? "left" : "right";
    //                    var id = currentPassage.Decision[d];
    //                    currentPassage = Stories[id];
    //                    myState.OnNext(MyState.TriggerAlienDialog);
    //                });
    //                inputController.playerResponseState
    //                   .Where(e => e == InputController.InputState.DoubleTap)
    //                   .First()
    //                   .Subscribe(_ =>
    //                   {
    //                       IsRepeating = true;
    //                       myState.OnNext(MyState.TriggerAlienDialog);
    //                   });

    //                break;
    //        }
    //    });
    //    myState.OnNext(MyState.TriggerAlienDialog);
    //}

    IObservable<float> TriggerStory()
    {
        var soundQ = this.currentPassage.SoundQueue;
        var soundObservables = soundQ.Select(s =>
        {
            var obj = GameObject.Find(s.TransformObject);
            var audioObject = obj.GetComponent<AudioObject>();
            audioObject.Load(s);
            return audioObject.AudioObservable;
        });

        IObservable<float> audioQueueObservable = Observable.Concat(soundObservables);

        audioQueueObservable.DoOnCompleted(() =>
        {

        });

        return audioQueueObservable;
    }

    void OnDeviceConnected()
    {
        Debug.Log("Device Connected!!!");
        Matcher = FindObjectOfType<RotationMatcher>();
        Control = FindObjectOfType<WearableControl>();
        this.IsBoseHandSetConnected.OnNext(true);
    }

    private void OnDeviceDisconnected(Device boseDevice)
    {
        Debug.Log($"Device {boseDevice.name} Disconnected");
    }

    //Restart the game
    private void Reset()
    {
        this.currentPassage = Stories[firstPassage];
        StopRunningAudio();
        var audioObjects = FindObjectsOfType<AudioObject>();
        foreach (var audioObject in audioObjects)
        {
            audioObject.audioSource.clip = null;
        }
        myState.OnNext(MyState.TriggerAlienDialog);
        //stop all coroutines... reset back to 
    }

    private void Resume()
    {
        this.IsGameLoopRunning.OnNext(true);
        var audioObjects = FindObjectsOfType<AudioObject>();
        foreach (var audioObject in audioObjects)
        {
            audioObject.Play();
        }
    }

    private void Pause()
    {
        this.IsGameLoopRunning.OnNext(false); //Stop game loop from running
        //TODO: convert audio play into observable;
        var audioObjects = FindObjectsOfType<AudioObject>();
        foreach (var audioObject in audioObjects)
        {
            audioObject.Pause();
        }
    }

    private void StopRunningAudio()
    {
        foreach (var co in this.runningSoundQueue)
        {
            co.Dispose();
        }
    }
}
