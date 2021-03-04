using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackSmokeController : MonoBehaviour {


    private ParticleSystem smokeParticleSystem;
    private Transform windZone;
    private Vector3 flashlightRaycastPoint;
	// Use this for initialization
	void Start () {
        smokeParticleSystem = transform.GetComponent<ParticleSystem>();
        foreach(Transform child in transform) {
            if(child.tag == "WindZone") {
                windZone = child;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.P)) {
            StartGenerateSmoke();
        }
        if(Input.GetKeyDown(KeyCode.Q)) {
            StopGenerateSmoke();
        }

        //windZone.gameObject.SetActive(false);
    }

    public void StartGenerateSmoke() {
        smokeParticleSystem.Play();
    }

    public void StopGenerateSmoke() {
        smokeParticleSystem.Stop();
    }
}
