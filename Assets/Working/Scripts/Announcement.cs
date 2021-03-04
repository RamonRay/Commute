using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Announcement : MonoBehaviour {
    public static Announcement instance;
    [SerializeField] float annoucementInterval=5f;
    [SerializeField] AudioClip[] loopedAnnouncements;
    [SerializeField] AudioClip trainStart, trainCrash,controlRoomIntercom;
    private AudioSource audioSource;
	// Use this for initialization
	void Start () {
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(RandomAnnouncement());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator RandomAnnouncement()
    {
        while (true)
        {
            audioSource.clip = loopedAnnouncements[Random.Range(0, loopedAnnouncements.Length)];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length + annoucementInterval);
        }
    }
    public void TrainStart()
    {
        audioSource.Stop();
        audioSource.clip = trainStart;
        audioSource.Play();
    }
    public void TrainCrash()
    {
        audioSource.Stop();
        audioSource.clip = trainCrash;
        audioSource.Play();
    }

    public void ControlRoonIntercom()
    {
        audioSource.Stop();
        audioSource.clip = controlRoomIntercom;
        audioSource.Play();
    }
    
}
