using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MonsterBodyController : MonoBehaviour {

    [SerializeField] List<Transform> handsList;
    [SerializeField] float planeOffset;
    [SerializeField] float frontOffset;
    [SerializeField] float moveForwardDistance = 5.0f;
    [SerializeField] float moveForwardDuration = 8.0f;

	// Use this for initialization
	void Start () {
        for(int i = 0; i < handsList.Count; i++) {
            StartCoroutine(MovingHand(handsList[i]));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator MovingHand(Transform hand) {
        while(true) {
            float timeSpan = Random.Range(3, 8);
            float xOffset = Random.Range(-planeOffset, planeOffset);
            float yOffset = Random.Range(-planeOffset, planeOffset);
            float zOffset = frontOffset;
            Vector3 targetPos = this.transform.position + zOffset * transform.forward + yOffset * transform.up + xOffset * transform.right;
            hand.DOLookAt((targetPos - hand.transform.position), timeSpan);
            yield return new WaitForSeconds(timeSpan);
        }
    }

    public void MonsterMoveForward() {
        transform.DOMove(transform.position + moveForwardDistance * transform.forward, moveForwardDuration);
    }
}
