using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour {
    private Material mat;
    private Vector2 currentOffset = new Vector2(0f, 0f);
	// Use this for initialization
	void Start () {
        mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        //Translate(0.01f);
	}
    public void Translate(float _distance)
    {
        currentOffset = mat.GetTextureOffset("_MainTex");
        currentOffset += new Vector2(0f, _distance);
        mat.SetTextureOffset("_MainTex", currentOffset);
    }
}
