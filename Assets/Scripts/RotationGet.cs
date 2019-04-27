using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationGet : MonoBehaviour {

    public UnityEngine.UI.Text debugText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        debugText.text = transform.eulerAngles.ToString();
		
	}
}
