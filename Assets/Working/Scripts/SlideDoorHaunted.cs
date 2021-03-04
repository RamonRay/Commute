using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDoorHaunted : SlidingDoor {
    [SerializeField]
    [Range(0f, 1f)]
    float maxPercentage=1f;
    [SerializeField] float closingTime=1f;
    [SerializeField] AudioClip doorSlam, doorYank;
    [SerializeField] float maxVolumn = 0.5f;
    private float openPercentage=0f;
    private float percentagePerSecond;
    private float closingPercentage;
    private float randomPercentage;
    private bool isHaunted = false;
    private int hauntedHands = 0;
    //private float percentageDissolve = 0f;
    private Monster[] monsters;
    //private Material handMat;
    private Material[] handMats;
    private AudioSource audioSource;
    // Use this for initialization
    void Start () {
        Init();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = maxVolumn;
        audioSource.maxDistance = 30f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        if (doorOpenTime > 0f)
            percentagePerSecond = 1f / doorOpenTime;
        else
            percentagePerSecond = 1f;

        if (closingTime > 0f)
        {
            closingPercentage = 1f / closingTime;
        }
        else
        {
            closingPercentage = 1f;
        }
        //Register Monsters
        monsters = transform.parent.GetComponentsInChildren<Monster>();
        if (monsters.Length>0)
        {
            foreach (Monster monster in monsters)
            {
                monster.RegisterOnAliveEvents(IsHaunted);
                monster.RegisterOnDeadEvents(NotHaunted);
                //monster.RegisterHandDissoving(SetHandDissolveRate);
            }
        }
        
        try
        {
            //handMat = GetComponentInChildren<SkinnedMeshRenderer>().material;
            SkinnedMeshRenderer[] skins = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skins.Length > 0)
            {
                handMats = new Material[skins.Length];
                for (int index = 0; index < skins.Length; index++)
                {
                    handMats[index] = skins[index].material;
                }
            }
        }
        catch
        {
            Debug.LogError("can't find it");
        }
        //SetHandDissolveRate(1f);

    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePercentage();
        UpdateDoorPosition(PercentageShaking(openPercentage));
	}
    private void UpdateDoorPosition(float percentage)
    {
        Vector3 translateDistance = percentage * movingDirection*doorWidth;
        transform.position = initialPosition + translateDistance;
    }
    private void UpdatePercentage()
    {
        if(isHaunted)
        {
            openPercentage += Time.deltaTime * percentagePerSecond;
            
            if (openPercentage>maxPercentage)
            {
                openPercentage = maxPercentage;
            }
            try
            {
                //handMat.SetFloat("_DissolveThreshold", Mathf.Clamp((1.2f-4*openPercentage/maxPercentage),0,1));
            }
            catch
            {
                
            }
        }
        else
        {
            openPercentage -= Time.deltaTime *closingPercentage;
            if(openPercentage<0f)
            {
                openPercentage = 0f;
            }
            try
            { 
                //handMat.SetFloat("_DissolveThreshold", Mathf.Clamp((1.2f - 4 * openPercentage / maxPercentage), 0, 1)); 
            }
            catch
            {

            }
        }
    }

    private float PercentageShaking(float percentage)
    {
        if(!isHaunted)
        {
            return percentage;
        }
        if (percentage < 0.1f * maxPercentage)
        {
            return percentage;
        }
        randomPercentage += Random.Range(-0.01f*maxPercentage-randomPercentage, 0.01f*maxPercentage-randomPercentage);
        return percentage + randomPercentage;
    }

    public void IsHaunted()
    {
        hauntedHands += 1;
        if (hauntedHands==1)
        {
            isHaunted = true;
            PlaySFX(doorYank, 0f, maxVolumn);
        }
    }
    public void NotHaunted()
    {
        hauntedHands -= 1;
        if (hauntedHands == 0)
        {
            isHaunted = false;
            PlaySFX(doorSlam, 0f, openPercentage / maxPercentage * maxVolumn);
        }
    }
    private void PlaySFX(AudioClip clip,float percentage,float volunmn)
    { if (percentage > 0.95f)
            return;
        audioSource.Stop();
        audioSource.volume = volunmn;
        audioSource.clip = clip;
        audioSource.time = clip.length * percentage;
        audioSource.Play();
    }

    public void SetHandDissolveRate(float rate)
    {
        foreach(var mat in handMats)
        {
            mat.SetFloat("_DissolveThreshold", rate);
        }
    }
}
