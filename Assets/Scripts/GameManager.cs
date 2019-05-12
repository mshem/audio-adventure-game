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
    Dictionary<int, CurrentEvent> Stories;
    CurrentEvent currentPassage;
    List<Coroutine> runningCoroutines = new List<Coroutine>();
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
        ResetButton.OnClickAsObservable().Subscribe(_ => { this.Reset();});
        PauseButton.OnClickAsObservable().Subscribe(_ => { this.Pause();});
        StartButton.OnClickAsObservable().Subscribe(_ => { this.Resume();});

        //set initial state
        this.myState = new BehaviorSubject<MyState>(MyState.TriggerAlienDialog);

        var isGameRunnable = Observable.CombineLatest(IsBoseHandSetConnected, IsGameLoopRunning);

        //game loop setup
        isGameRunnable
            .Where(e => e.All(c => c))
            .Do(_ => Debug.Log("debug :D"))
            .SelectMany(myState)
            .Subscribe(GameLoop);

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
                runningCoroutines.Add(StartCoroutine(TriggerStoryEvent()));
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
                .Subscribe(e => {
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
                   .Subscribe(_ => {
                       IsRepeating = true;
                       myState.OnNext(MyState.TriggerAlienDialog);
                   });

                break;
        }
    }

    void RunLoop()
    {
        Debug.Log("Starting loop");
        Bose.Wearable.RotationMatcher matcher = GameObject.FindObjectOfType<Bose.Wearable.RotationMatcher>();
        Bose.Wearable.WearableControl control = GameObject.FindObjectOfType<Bose.Wearable.WearableControl>();
        //myState.First().Subscribe(state => {
        //    matcher.SetRelativeReference(control.LastSensorFrame.rotation);
        //});

        stateSub = myState.Subscribe(state =>
        {
            switch (state)
            {
                case MyState.DoNothing:
                    //Do nothing
                    break;
                case MyState.TriggerAlienDialog:
                    matcher.SetRelativeReference(control.LastSensorFrame.rotation);
                    runningCoroutines.Add(StartCoroutine(TriggerStoryEvent()));
                    myState.OnNext(MyState.DoNothing);
                    break;
                case MyState.TriggerPlayerAwaitAction:
                    IsRepeating = false;
                    Debug.Log("waiting for player");
                    matcher.SetRelativeReference(control.LastSensorFrame.rotation);
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
                    .Subscribe(e => {
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
                       .Subscribe(_ => {
                           IsRepeating = true;
                           myState.OnNext(MyState.TriggerAlienDialog);
                       });

                    break;
            }
        });
        myState.OnNext(MyState.TriggerAlienDialog);
    }

    IEnumerator TriggerStoryEvent()
    {
        // play audio file
        var soundQ = this.currentPassage.SoundQueue;
        for (var i = 0; i < soundQ.Count; i++)
        {
            var currentSound = soundQ[i];
            var soundLoc = "Audio/" + currentSound.FileLoc;
            var audioClip = Resources.Load<AudioClip>(soundLoc);
            var obj = GameObject.Find(currentSound.TransformObject);
            var obj1 = obj.GetComponent<AudioSource>();
            obj1.volume = currentSound.Volume ?? obj1.volume;
            obj1.loop = currentSound.IsRepeat;

            //if (currentSound.IsRepeat)
            //{
            //    this.OnLoopSound.Add(currentSound.TransformObject, obj);
            //}
            Debug.Log(soundLoc);
            if (audioClip == null)
                throw new Exception(soundLoc + " not found!");
            obj1.clip = audioClip;
            if (debug)
            {
                //obj1.Play();
                yield return new WaitForSeconds(1);
            }
            else if (!currentSound.IsBlocking)
            {
                obj1.Play();
            }
            else
            {
                obj1.Play();
                yield return new WaitForSeconds(obj1.clip.length);
            }

        }
        // add to queue
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
            //game is done  
        }
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
        myState.OnNext(MyState.TriggerAlienDialog);
        //stop all coroutines... reset back to 
    }

    private void Resume()
    {
        this.IsGameLoopRunning.OnNext(true);
    }

    private void Pause()
    {
        this.IsGameLoopRunning.OnNext(false); //Stop game loop from running
        //TODO: convert audio play into observable;
        StopRunningAudio();
    }

    private void StopRunningAudio()
    {
        foreach (var co in this.runningCoroutines)
        {
            StopCoroutine(co);
        }
    }
}
