using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BatteryScript : MonoBehaviour {


    private float startTime;
    private float elapsedTime;
    public Image batteryImage;
    public GameObject cameraObject;
    public Text positionText;


    // Use this for initialization
    void Start () {
        startTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        elapsedTime = Time.time - startTime;

        if(elapsedTime > 2400)
        {
            //display battery2_two
            batteryImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("battery2_two");

        }
        else if(elapsedTime > 4800)
        {
            //display batter2_three
            batteryImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("battery2_three");
        }
        else
        {
            batteryImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("battery2_one");
        }

        positionText.text = "x: " + (float)Math.Round((double)cameraObject.transform.position.x, 2) + "y: " + (float)Math.Round((double)cameraObject.transform.position.z, 2) + "z: " + (float)Math.Round((double)cameraObject.transform.position.z, 2);
    }
}
