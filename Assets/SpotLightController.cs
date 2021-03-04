using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightController : MonoBehaviour {

    [SerializeField] float MaxIntensity = 1.75f;
    [SerializeField] List<float> LightStrobingStep;
    private Light spotLight;
	// Use this for initialization
	void Start () {
        spotLight = this.GetComponent<Light>();
        spotLight.intensity = MaxIntensity;
	}
	
	// Update is called once per frame
	void Update () {
        //if(Input.GetKeyDown(KeyCode.B)) {
        //    LightPowerOff();
        //}
	}

    // flashlight will strobe several times and then turn off. 
    public void LightPowerOff() {
        StartCoroutine(StrobeAndOff());
    }

    IEnumerator StrobeAndOff() {
        // down
        while(spotLight.intensity > LightStrobingStep[0]) {
            spotLight.intensity = spotLight.intensity - LightStrobingStep[0];
            yield return null;
        }

        // up
        while(spotLight.intensity < MaxIntensity - LightStrobingStep[1]) {
            spotLight.intensity = spotLight.intensity + LightStrobingStep[1];
            yield return null;
        }
        spotLight.intensity = MaxIntensity;

        // down
        while(spotLight.intensity > LightStrobingStep[2]) {
            spotLight.intensity = spotLight.intensity - LightStrobingStep[2];
            yield return null;
        }

        // wait
        yield return new WaitForSeconds(0.5f);


        // up
        while(spotLight.intensity < 0.75f * MaxIntensity) {
            spotLight.intensity = spotLight.intensity + LightStrobingStep[3];
            yield return null;
        }

        spotLight.intensity = 0.75f * MaxIntensity;

        // down
        while(spotLight.intensity > LightStrobingStep[4]) {
            spotLight.intensity = spotLight.intensity - LightStrobingStep[4];
            yield return null;
        }

        // up
        while(spotLight.intensity < 0.5f * MaxIntensity) {
            spotLight.intensity = spotLight.intensity + LightStrobingStep[5];
            yield return null;
        }

        spotLight.intensity = 0.5f * MaxIntensity;

        spotLight.intensity = 0;
    }
}