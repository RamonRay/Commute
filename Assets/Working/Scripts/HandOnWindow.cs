using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandOnWindow : MonoBehaviour {

    [SerializeField] float movingOffset = 10;


    private Vector3 startPoint;
    private Vector3 endPoint;
	// Use this for initialization
	void Start () {
        endPoint = transform.position;
        startPoint = (transform.position.x > 0) ? (transform.position + movingOffset * Vector3.right) : (transform.position - movingOffset * Vector3.right);
        transform.position = startPoint;
	}
	
	// Update is called once per frame
	void Update () {
        //if(Input.GetKeyDown(KeyCode.L)) {
        //    Slap();
        //}
	}

    public void Slap() {
        transform.DOMove(endPoint, 0.1f, false);
        Invoke("Retrieve", 1f);
    }

    private void Retrieve() {
        transform.DOMove(startPoint, 0.8f, false);
    }
}
