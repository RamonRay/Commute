using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkController : MonoBehaviour {
    [SerializeField] AudioClip[] sparkSFX;
    [SerializeField] bool WithSFX = true;
    private float DistanceThreshod = 20f;
    private AudioSource audioSource;
    private ParticleSystem particleSys;
    private Light particleBaseLight;
    private Transform playerTrans;

	// Use this for initialization
	void Start () {
        playerTrans = GameObject.FindWithTag("Player").GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
        particleSys = transform.GetChild(0).GetComponent<ParticleSystem>();
        particleBaseLight = transform.GetChild(1).GetComponent<Light>();
        StartCoroutine(ParticleOn());
    }

    void OnAwake() {
        StartCoroutine(ParticleOn());
    }
	
	// Update is called once per frame
	void Update () {
        if(Vector3.Distance(playerTrans.position, this.transform.position) < DistanceThreshod) {
            audioSource.enabled = true;
        }
        else {
            audioSource.enabled = false;
        }
	}

    IEnumerator ParticleOn() {
        float waitTime = Random.Range(1.2f, 8f);
        yield return new WaitForSeconds(waitTime);
        particleSys.Play();
        //Debug.Log("Sparks On");
        StartCoroutine(ParticleOff());
        if(WithSFX) {
            audioSource.Stop();
            audioSource.clip = sparkSFX[Random.Range(0, sparkSFX.Length)];
            audioSource.Play();
        }
    }

    IEnumerator ParticleOff() {
        float waitTime = Random.Range(0.4f, 1.25f);
        yield return new WaitForSeconds(waitTime);
        particleSys.Stop();
        if(WithSFX) {
            audioSource.Stop();
        }
        //Debug.Log("Sparks Off");
        StartCoroutine(ParticleOn());
    }


}
