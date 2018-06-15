using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class ServerScript : MonoBehaviour
{

    private float prevX, currentX;
    private float prevY, currentY;
    private float prevZ, currentZ;
    private string[] coordData;
    public bool gameStarted;
    public string ipaddress;

    private float moveScaleX = 10.5f, moveScaleY = 8, moveScaleZ = 25;

    void Start()
    {
        gameStarted = false;
        if (ipaddress != null)
        {
            StartCoroutine(GetData(ipaddress));
        }

    }

    IEnumerator GetData(string ip)
    {
        while (true)
        {
            //"http://192.168.0.101:80"
            UnityWebRequest www = UnityWebRequest.Get(ip);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                coordData = www.downloadHandler.text.Split('/');
                currentX = float.Parse(coordData[0]);
                currentY = float.Parse(coordData[1]);
                currentZ = float.Parse(coordData[2]);
                //Debug.Log(currentX);
            }
        }



    }

    void Update()
    {
        if (prevX != 0)
        {
            Vector3 relativeVector = new Vector3(currentX - prevX, currentY - prevY, currentZ - prevZ);
            //Debug.Log(relativeVector.x);

            if (gameStarted)
            {
                //after the rotation
                transform.Translate(-relativeVector.z * Time.deltaTime * moveScaleZ, relativeVector.y * Time.deltaTime * moveScaleY, -relativeVector.x * Time.deltaTime * moveScaleX);
            }
            else
            {
                transform.Translate(relativeVector.x * Time.deltaTime * moveScaleX, relativeVector.y * Time.deltaTime * moveScaleY, -relativeVector.z * Time.deltaTime * moveScaleZ);
            }

        }


        prevX = currentX;
        prevY = currentY;
        prevZ = currentZ;

    }


}
