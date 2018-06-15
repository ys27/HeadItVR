using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DatabaseScript : MonoBehaviour
{

    public float startTime;
    public float elapsedTime;

    private int maxRecord = 5;
    public List<Records> savedRecords;
    private int goalScored, goalAttempts;

    private int playerID;

    public Text playerNameText;
    public Text attemptsText;
    public Text scoredText;
    public Text playtimeText;

    // Use this for initialization
    void Start()
    {
        for(int i = 0; i < maxRecord; i++)
        {
            string jsonData = PlayerPrefs.GetString("record" + i);
            if(jsonData != null)
            {
                Records loadedData = JsonUtility.FromJson<Records>(jsonData);
                savedRecords.Add(loadedData);
            }
        }

        playerID = PlayerPrefs.GetInt("playerID");
       // Debug.Log("player = " + playerID);
        for (int i = playerID; i < maxRecord + playerID; i++)
        {
           int j = (i + 1) % maxRecord ;
           // Debug.Log("j = " + j);
                if(savedRecords[j] != null)
                {
                playerNameText.text = playerNameText.text + "\n\n" + savedRecords[j].playerName;
                attemptsText.text = attemptsText.text + "\n\n" + savedRecords[j].goalsAttempted;
                scoredText.text = scoredText.text + "\n\n" + savedRecords[j].goalScored;
                playtimeText.text = playtimeText.text + "\n\n" + savedRecords[j].playTime + "s";
                //        //Debug.Log(savedRecords[i].playerName);
                //       // Debug.Log("Goal attemps = " + savedRecords[i].goalsAttempted);
                //        //Debug.Log("Goal Scored  = " +savedRecords[i].goalScored);
                //        //Debug.Log(savedRecords[i].playTime);
            }

        }




           
        goalScored = 0;
        goalAttempts = 0;
    }

    void OnApplicationPause()
    {
        elapsedTime = Time.time - startTime;
        
        Debug.Log("Application ending after " + elapsedTime + " seconds");
        Records newRecord = new Records();
        newRecord.playTime = (float)Math.Round((double)elapsedTime, 2);
        newRecord.goalScored = goalScored;
        newRecord.goalsAttempted = goalAttempts;
        playerID++;
        newRecord.playerName = "Player" + playerID;

        //Convert to Json
        string jsonData = JsonUtility.ToJson(newRecord);
        //Save Json string
        PlayerPrefs.SetString("record" + playerID%maxRecord, jsonData);
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("playerID", playerID);
        PlayerPrefs.Save();
    }

    public void startTimer()
    {
        startTime = Time.time;
    }

    public void incrementGoalAttempt()
    {
        goalAttempts++;
    }

    public void incrementGoalScored()
    {
        goalScored++;
    }
}
