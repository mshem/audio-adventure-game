using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class InputController : MonoBehaviour
{
    EventHandler BoseWearableEventHandler; 

    // Use this for initialization
    void Start()
    {
        Bose.Wearable.WearableControl.Instance.HeadNodGesture.Enable();
        Bose.Wearable.WearableControl.Instance.HeadShakeGesture.Enable();
        //BoseWearableEventHandler += Bose.Wearable.WearableControl.Instance.DoubleTapDetected;

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

    }

    public void No()
    {

    }
}
