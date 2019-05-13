using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Linq;

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
    Dictionary<int, CurrentEvent> Stories;
    CurrentEvent currentPassage;
    bool IsRepeating = false;
    Subject<MyState> myState = new Subject<MyState>();
    Dictionary<string, GameObject> OnLoopSound = new Dictionary<string, GameObject>();

    public bool debug;

    // Start is called before the first frame update
    void Start()
    {
        inputController = FindObjectOfType<InputController>();
        //jsonutil load file

        //using (StreamReader r = new StreamReader("Assets/Resources/zorblok.json"))
        //{
        //string jsonString = r.ReadToEnd();
        string jsonString = (Resources.Load("zorblok") as TextAsset).text;
        Stories = JsonConvert.DeserializeObject<Dictionary<int, CurrentEvent>>(jsonString);
        //Stories = JsonUtility.FromJson<>(jsonString);
        //}
        currentPassage = Stories[firstPassage];



        Observable.EveryUpdate()
        .Where((e) => Bose.Wearable.WearableConnectUIPanel.isDeviceConnected)
        .First()
        .Subscribe(_ => ShowMainMenu());

    }


    void ShowMainMenu()
    {

        RunLoop();
    }

    void RunLoop()
    {
        Debug.Log("Starting loop");
        Bose.Wearable.RotationMatcher matcher = GameObject.FindObjectOfType<Bose.Wearable.RotationMatcher>();
        Bose.Wearable.WearableControl control = GameObject.FindObjectOfType<Bose.Wearable.WearableControl>();
        //myState.First().Subscribe(state => {
        //    matcher.SetRelativeReference(control.LastSensorFrame.rotation);
        //});

        myState.Subscribe(state =>
        {
            switch (state)
            {
                case MyState.DoNothing:
                    //Do nothing
                    break;
                case MyState.TriggerAlienDialog:
                    matcher.SetRelativeReference(control.LastSensorFrame.rotation);
                    StartCoroutine(TriggerStoryEvent());
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

    private void Update()
    {

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
}
