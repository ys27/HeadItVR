using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSound : MonoBehaviour {

    private AudioSource source;
   

    // Use this for initialization
    void Start () {
        
        source.Play();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void stopBGM()
    {
        StartCoroutine(FadeOut(source, 1.5f));
        //source.Stop();
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
