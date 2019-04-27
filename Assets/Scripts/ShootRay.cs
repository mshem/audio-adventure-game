using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        var ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            //hit.transform.GetComponent<Renderer>().material.color = Color.red;

            AudioSource snd = hit.transform.GetComponent<AudioSource>();
            if (snd.isPlaying == false)
                snd.Play();
        }
    }
}
