using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class goalLineTech : MonoBehaviour
{

    public Text scoreText;
    private int score;

    private GameObject GoalSoundManager;
    private AudioSource source;

    public bool goalEnable;

    private GameObject SystemObject;
    private DatabaseScript dbscript;

    // Use this for initialization
    void Start()
    {
        score = 0;
        goalEnable = false;
    }

    private void Awake()
    {
        GoalSoundManager = GameObject.Find("GoalSoundManager");
        source = GoalSoundManager.GetComponent<AudioSource>();

        SystemObject = GameObject.Find("System");
        dbscript = SystemObject.GetComponent<DatabaseScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (goalEnable)
        {
            score++;
            scoreText.text = "Score: " + score;
            print("GOAL");
            source.Play();
            goalEnable = false;

            dbscript.incrementGoalScored();
        }
    }

    public void fadeGoalSound()
    {
        StartCoroutine(FadeOut(source, 1.5f));
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
