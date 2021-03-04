using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Flashlight : MonoBehaviour {
    [SerializeField] LayerMask monsterLayer;
    [SerializeField] SteamVR_Action_Boolean trigger;
    [SerializeField] SteamVR_Action_Single sd;
    [SerializeField] SteamVR_Input_Sources controller;
    public SteamVR_Action_Vibration hapticAction;
    //[SerializeField] Hand hand;
    [SerializeField] GameObject spotLight;
    [SerializeField] Transform laserEmitter;
    [SerializeField] GameObject windZone;
    private HighlightObject lastHighlighted;
    private Monster lastMonster=null;
    private bool lightOn;
    private bool isBroken=false;
	// Use this for initialization
	void Start () {
        lightOn = spotLight.activeSelf;
        windZone.transform.position = laserEmitter.position;
        isBroken = false;
	}
	
	// Update is called once per frame
	void Update () {
        DisableModel();
        if (!isBroken&&lightOn)
        {
            DissolveGhost();
            CheckHighlight();
        }

        if (!isBroken&&trigger.GetStateDown(controller))
        {
            if(spotLight.activeSelf)
            {
                FlashLightOff();
            }
            else
            {
                FlashLightOn();
            }
        }

        //if(Input.GetKeyDown(KeyCode.D))
        //{
            //hapticAction.Execute(0f,3f,120f,1f,controller);
        //}

	}

    public void FlashLightOn()
    {
        spotLight.SetActive(true);
        lightOn = true;
    }

    public void FlashLightOff()
    {
        spotLight.SetActive(false);
        lightOn = false;
    }

    private void DisableModel()
    {
        Renderer[] renderers = transform.parent.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
    }
    private void DissolveGhost()
    {
        RaycastHit hit;
        if (Physics.Raycast(laserEmitter.position, laserEmitter.forward, out hit, 100f, monsterLayer))
        {
            //call monster's function
            Monster _monster = hit.collider.gameObject.GetComponentInParent<Monster>();
            if (_monster != null)
            {
                _monster.isLit = true;
                windZone.transform.position = hit.point;
            }
            if(_monster!=lastMonster&&lastMonster!=null)
            {
                lastMonster.isLit = false;
                windZone.transform.position = laserEmitter.position;
            }
            lastMonster = _monster;

        }
        else
        {
            if (lastMonster != null)
            {
                lastMonster.isLit = false;
                windZone.transform.position = laserEmitter.position;
                lastMonster = null;
            }
        }

    }
    //Highlight the object that the flashlight is on.
    private void CheckHighlight()
    {
        RaycastHit hit;
        if (Physics.Raycast(laserEmitter.position, laserEmitter.forward, out hit, 10f))
        {
            HighlightObject _hlo = hit.collider.gameObject.GetComponent<HighlightObject>();
            if (_hlo == lastHighlighted)
            {
                return;
            }
            if (_hlo != null)
            {
                _hlo.IsLit = true;
            }
            else
            {
                lastHighlighted.IsLit = false;
            }
            lastHighlighted = _hlo;

        }
        else
        {
            if (lastHighlighted != null)
            {
                lastHighlighted.IsLit = false;
                lastHighlighted = null;
            }
        }

    }
    //The flashlight is broken down
    public void FlashlightDown()
    {
        StartCoroutine(FlashLightFlicker());
    }
    public void FlashlightRepaired()
    {
        isBroken = false;
    }

    IEnumerator FlashLightFlicker()
    {
        float flickerTime = 3f;
        try { hapticAction.Execute(0f, 3f, 120f, 1f, controller); }
        catch {
            //
        }
        
        float angleStep = 10f * 360f / flickerTime;
        float startTime = Time.time;
        float startIntensity;
        float angle = 90f;
        try
        {
            startIntensity = GetComponentInChildren<Light>().intensity;
        }
        catch
        {
            startIntensity = 1.75f;
        }
        while(Time.time<startTime+flickerTime)
        {
            
            float intensity = startIntensity * (1f - (Time.time - startTime) / flickerTime);
            float finalIntensity = intensity * Mathf.Abs(Mathf.Sin(angle));
            try
            {
                GetComponentInChildren<Light>().intensity = finalIntensity;
            }
            catch
            {
                //Do nothing.
            }
            if(finalIntensity>0.1f)
            {
                isBroken = false;
            }
            else
            {
                isBroken = true;
            }
            angle += angleStep * Random.Range(0.5f, 1.5f)*0.04f;
            
            yield return new WaitForSeconds(0.04f);
        }
        spotLight.SetActive(true);
        try { GetComponentInChildren<Light>().intensity = startIntensity; }
        catch { }
        spotLight.SetActive(false);
        lightOn = false;
        isBroken = true;
        yield break;
    }

}
