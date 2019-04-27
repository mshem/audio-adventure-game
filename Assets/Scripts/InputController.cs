using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class InputController : MonoBehaviour
{
    EventHandler BoseWearableEventHandler;

    public enum InputState
    {
        Yes,
        No
    }

    public Subject<InputState> playerResponseState = new Subject<InputState>();

    // Use this for initialization
    void Start()
    {
        Bose.Wearable.WearableControl.Instance.HeadNodGesture.Enable();
        Bose.Wearable.WearableControl.Instance.HeadShakeGesture.Enable();
        //BoseWearableEventHandler. Bose.Wearable.WearableControl.Instance.DoubleTapDetected;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Left()
    {
     
    }

    public void Right()
    {

    }

    public void Yes()
    {
        Debug.Log("Yes");
        playerResponseState.OnNext(InputState.Yes);
    }

    public void No()
    {
        Debug.Log("No");
        playerResponseState.OnNext(InputState.No);
    }
}
