using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCam : MonoBehaviour {

    public GameObject target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = target.transform.rotation;
    }
}
