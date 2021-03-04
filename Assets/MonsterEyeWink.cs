using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MonsterEyeWink : MonoBehaviour {

    [SerializeField] Transform upEyelid;
    [SerializeField] Transform downEyelid;

 	// Use this for initialization
	void Start () {
        StartCoroutine(KeepWink());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator KeepWink() {
        while(true) {
            float interval = Random.Range(4, 9);
            StartCoroutine(DoWink());
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator DoWink() {
        upEyelid.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        downEyelid.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        
        yield return new WaitForSeconds(0.6f);

        upEyelid.DOLocalRotate(new Vector3(0, 0, -90), 0.5f);
        downEyelid.DOLocalRotate(new Vector3(0, 0, 90), 0.5f);

    }
}
