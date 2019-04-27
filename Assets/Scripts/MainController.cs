using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        // Start speaking first node
        EasyTTSUtil.SpeechAdd(GetStartNode());
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    static string GetStartNode() {
        return "Hello World!";
    }
}
