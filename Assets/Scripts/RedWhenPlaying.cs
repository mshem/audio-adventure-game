using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWhenPlaying : MonoBehaviour {

    AudioSource snd;
	// Use this for initialization
	void Start () {
        snd = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        if (snd.isPlaying)
            transform.GetComponent<Renderer>().material.color = Color.red;
        else
            transform.GetComponent<Renderer>().material.color = Color.white;
		
	}
}
