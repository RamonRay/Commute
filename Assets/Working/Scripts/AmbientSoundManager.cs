using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour {
    [SerializeField] AudioClip whiteNoise, spookyIntro, spookyAmbient;
    public static AmbientSoundManager instance { get; private set; }
    private AudioSource source;
	// Use this for initialization
	void Start () {
		if(instance==null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There are 2 AmbientSoundManager in the scene");
        }
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
    public void PlayNormalWhiteNoise()
    {
        if(source.clip==whiteNoise)
        {
            return;
        }
        source.volume = 0.117f;
        source.Stop();
        source.clip = whiteNoise;
        source.Play();
    }
    public void PlaySpookyAmbient()
    {
        source.Stop();
        source.volume = 1f;
        source.clip = spookyIntro;
        source.Play();
        StartCoroutine(WaitForIntro());
    }
    IEnumerator WaitForIntro()
    {
        yield return new WaitForSeconds(spookyIntro.length);
        source.clip = spookyAmbient;
        source.Play();
        yield break;
    }


}
