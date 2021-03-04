using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class LightingManager : MonoBehaviour {

    [SerializeField] List<Light> lightList;
    [SerializeField] List<Light> lightInOtherTrain;
    //[SerializeField] GameObject alertLight;
    [SerializeField] List<GameObject> alermLightList;
    [SerializeField] GameObject sparksObj;
    [SerializeField] List<GameObject> sparkObjList;
    [SerializeField] TopLightPlaneController topLightCtrl;
    [SerializeField] AdvertiseController adCtrl;
    [SerializeField] TopLightPlaneController otherTrainTopLightCtrl;
    [SerializeField] AdvertiseController otherTrainAdCtrl;


	// Use this for initialization
	void Start () {
        Invoke("LightNormal", 0.1f);
        SetLightIntensity(1f);
        //LightsInOtherTrainOn();
	}
	
	// Update is called once per frame
	void Update () {
        //if(Input.GetKeyDown(KeyCode.L)) {
        //    EmergencyLight();
        //}
        //if(Input.GetKeyDown(KeyCode.K)) {
        //    LightNormal();
        //}
	}

    public void EmergencyLight() {

        //alertLight.SetActive(true);
        StartCoroutine(EmergencyLightGraduallyOn());
        sparksObj.SetActive(true);
        foreach(GameObject tmp in sparkObjList) {
            tmp.SetActive(true);
        }
        LightsInOtherTrainOff();
    }

    IEnumerator EmergencyLightGraduallyOn()
    {
        List<Light> lights = new List<Light>();
        foreach (GameObject tmp in alermLightList)
        {
            tmp.SetActive(true);
            Light[] currentLights;
            currentLights = tmp.GetComponentsInChildren<Light>();
            foreach(Light light in currentLights)
            {
                lights.Add(light);
                light.intensity = 0f;
            }
        }
        float startTime = Time.time;
        while (Time.time < startTime + 1f)
        {
            foreach(Light light in lights)
            {
                light.intensity = Time.time - startTime;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }
    public void LightStrobe(float strobeTime, float strobeInterval)
    {
        StopAllCoroutines();
        StartCoroutine("LightStrobing", new object[2] { strobeTime, strobeInterval });
    }
    IEnumerator LightStrobing(object[] parameters)
    {
        //White Light Strobing
        float maxIntensity = 1f;
        float strobeTime = (float)parameters[0];
        float strobeInterval = (float)parameters[1];
        float currentTime = Time.time;
        float angle = Mathf.PI/2f;
        float _strobeInterval = strobeInterval;
        float timeStep = 0.1f;
        while(Time.time<currentTime+strobeTime)//||angle>0.1f)
        {
            maxIntensity = Mathf.Clamp(1-(Time.time - currentTime) / strobeTime,0f,1f);
            float _intensity = Mathf.Sin(angle);
            SetLightIntensity(maxIntensity*_intensity);
            TunnelGenerator.instance.SetTunnelLightIntensity(maxIntensity*_intensity*Random.Range(0.6f,1f));
            try { GuestLight((int)(255f * _intensity)); }
            catch { Debug.LogError("NoLiginting"); }
            angle += timeStep/_strobeInterval*Mathf.PI;
            if(angle>Mathf.PI)
            {
                angle -= Mathf.PI;
                _strobeInterval = Random.Range(0.3f, 3f) * strobeInterval;
            }
            yield return new WaitForSeconds(timeStep);
        }
        SetLightIntensity(0);
        EmergencyLight();
        //Red Guest Light Lit
        currentTime = Time.time;
        while(Time.time<currentTime+1f)
        {
            try { GuetLightRed((int)(100f * (Time.time - currentTime) / 1f)); }
            catch { Debug.LogError("No Lighting Manager"); }
            yield return new WaitForSeconds(0.05f);
        }


        try { GuetLightRed((int)(100f)); }
        catch { Debug.LogError("No Lighting Manager"); }
        yield return new WaitForSeconds(0.1f);

        yield break;

    }

    private void SetLightIntensity(float intensity)
    {
        RenderSettings.ambientIntensity = intensity;
        RenderSettings.reflectionIntensity = 0.2f+intensity*0.8f;
        DynamicGI.indirectScale = intensity;
        for (int i = 0; i < lightList.Count; i++)
        {
            lightList[i].intensity = intensity * 2;
        }
        adCtrl.SetMatBright(intensity);
        topLightCtrl.SetTopLightBright(intensity);
    }


    public void LightCrashing() {

    }

    public void LightNormal() {
        RenderSettings.ambientIntensity = 1;
        RenderSettings.reflectionIntensity = 1f;
        DynamicGI.indirectScale = 1;
        for(int i = 0; i < lightList.Count; i++) {
            lightList[i].intensity = 2;
        }
        //alertLight.SetActive(false);
        foreach(GameObject tmp in alermLightList) {
            tmp.SetActive(false);
        }
        sparksObj.SetActive(false);
        foreach(GameObject tmp in sparkObjList) {
            tmp.SetActive(false);
        }
        LightsInOtherTrainOn();
        GuestAndBackLight(255);
    }
    

    public void GuestLight(int dimmer=255)
    {
        DMXController.Lighting.TurnOn("Guest", Color.white, 0, dimmer);

    }
    public void GuestAndBackLight(int dimmer=255)
    {
        DMXController.Lighting.TurnOn("GuestAndBack", Color.white, 0, dimmer);
    }
    
    public void BackLightLeft(int dimmer=30)
    {
        DMXController.Lighting.TurnOn("setLeft", Color.white, 0, dimmer);
    }

    public void BackLightRight(int dimmer = 30)
    {
        DMXController.Lighting.TurnOn("setRight", Color.white, 0, dimmer);
    }

    public void GuetLightRed(int dimmer=255)
    {
        DMXController.Lighting.TurnOn("GuestAndBack", Color.red, 0, dimmer);
    }

    public void BackLightLeftRed(int dimmer = 30)
    {
        DMXController.Lighting.TurnOn("setLeft", Color.red, 0, dimmer);
    }

    public void BackLightRightRed(int dimmer = 30)
    {
        DMXController.Lighting.TurnOn("setLeft", Color.red, 0, dimmer);
    }

    public void LightsInOtherTrainOn() {
        foreach(Light tmpLight in lightInOtherTrain) {
            tmpLight.intensity = 2;
        }
        otherTrainTopLightCtrl.SetTopLightBright(1);
        otherTrainAdCtrl.SetMatBright(1);
    }

    public void LightsInOtherTrainOff() {
        foreach(Light tmpLight in lightInOtherTrain) {
            tmpLight.intensity = 0;
        }
        otherTrainTopLightCtrl.SetTopLightBright(0);
        otherTrainAdCtrl.SetMatBright(0);
    }

    private void OnDisable()
    {
        DMXController.Lighting.Blackout();
    }
}

