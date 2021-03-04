using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLightPlaneController : MonoBehaviour {
    [SerializeField] List<MeshRenderer> meshList;
    //[Range(0f, 1f)]
    //[SerializeField] float testIntensity = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // for test ONLY
        //SetTopLightBright(testIntensity);
	}

    // intensity[0, 1] will influence the HDR color and albedo color or the top light material (intensity will be mapped to [-10, 0.5])
    public void SetTopLightBright(float intensity) {
        for(int i = 0; i < meshList.Count; i++) {
            Material mat = meshList[i].material;
            float mappedIntensity = 1.9f * intensity - 0.4f;
            //float mappedIntensity = intensity;
            mat.SetColor("_EmissionColor", Color.white * mappedIntensity);
        }
    }
}
