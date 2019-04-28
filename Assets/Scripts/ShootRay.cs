using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRay : MonoBehaviour {
    public InputController inputController;
	// Use this for initialization
	void Start () {
        inputController = FindObjectOfType<InputController>();
    }

    // Update is called once per frame
    void Update () {
        RaycastHit hit;
        var ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            //hit.transform.GetComponent<Renderer>().material.color = Color.red;
            //
            //AudioSource snd = hit.transform.GetComponent<AudioSource>();
            //if (snd.isPlaying == false)
                //snd.Play();


            // left right code
            if (hit.transform.name == "Right")
            {
                inputController.Right();
            }
            else if (hit.transform.name == "Left")
            {
                inputController.Left();
            }

        }
    }
}
