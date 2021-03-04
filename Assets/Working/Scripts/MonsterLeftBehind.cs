using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MonsterLeftBehind : MonoBehaviour {

    [SerializeField] float Offset = 100;
    [SerializeField] AudioClip MonsterLeftBehindSound;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private AudioSource audioSource;
    
	// Use this for initialization
	void Start () {
        audioSource = this.GetComponent<AudioSource>();
        startPoint = this.transform.position;
        endPoint = transform.position + Offset * Vector3.forward;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MovingMonsterSound() {
        audioSource.PlayOneShot(MonsterLeftBehindSound);
        //audioSource.DOFade(0, 3);
        transform.DOMove(endPoint, 3, false);
    }
}
