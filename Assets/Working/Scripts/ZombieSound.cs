using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSound : MonoBehaviour {

    [SerializeField] AudioClip[] zombieSFXs;
    [Range(0f, 1f)]
    [SerializeField] float[] volumns;
    private AudioSource audioSource;
    int index = 0;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.B))
        {
            PlayNextZombieSound();
        }
	}

    public void PlayNextZombieSound()
    {
        if(index>=zombieSFXs.Length)
        {
            //Debug.LogError("OutOfRange");
            index = -1;
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = zombieSFXs[index];
            audioSource.volume = volumns[index];
            audioSource.Play();
        }
        index += 1;
    }
}
