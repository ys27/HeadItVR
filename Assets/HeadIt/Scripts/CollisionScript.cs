using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TechTweaking.Bluetooth;

public class CollisionScript : MonoBehaviour {

    private AudioSource source;
    private float hitX;

    private GameObject btConnectionManager;
    private BasicDemo btScript;
    private GameObject camera;

    public GameObject ball;
    public float xOffset, yOffset, zOffset;
    // Use this for initialization
    void Start () {
        btConnectionManager = GameObject.Find("ArduinoConnectionManager");
        btScript = btConnectionManager.GetComponent<BasicDemo>();
        source = GetComponent<AudioSource>();
        camera = transform.GetChild(0).gameObject;

    }
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetMouseButtonDown(0))
        //{
          //  ball.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z + zOffset);
            //Instantiate(ball, new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z + zOffset), Quaternion.identity);
       // }
            
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        //print("ball hit at " + pos + "gameobj" + gameObject.transform.position);
        print("A" + camera.transform.InverseTransformPoint(pos));

        hitX = camera.transform.InverseTransformPoint(pos).x;
        source.Play();

        if(hitX < 0)
        {
            btScript.sendLeft();
        }else if(hitX > 0)
        {
            btScript.sendRight();
        }else
        {
            btScript.sendMiddle();
        }
    }
}
