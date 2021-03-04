using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvertiseController : MonoBehaviour {

    [SerializeField] List<MeshRenderer> meshList;
    [Range(0f, 1f)]
    [SerializeField] float testColorVIntensity = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // for test ONLY
        //SetMatBright(testColorVIntensity);
	}

    // intensity[0,1] will be mapped to V value [0.125, 1.25] (from the most dark to the most bright)
    public void SetMatBright(float intensity) {
        for(int i = 0; i < meshList.Count; i++) {
            Material mat = meshList[i].material;
            //Color newColor;
            //float H, S, V;
            //Color.RGBToHSV(mat.GetColor("_Color"), out H, out S, out V);
            //V = 0.125f + 1.125f * intensity;
            //newColor = Color.HSVToRGB(H, S, V);
            //mat.SetColor("_Color", newColor);
            mat.SetFloat("_Mixer", 1 - intensity);
        }
    }
}
