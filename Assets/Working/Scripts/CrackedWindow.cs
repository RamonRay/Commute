using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedWindow : MonoBehaviour {
    [SerializeField] float progressionTime = 10f;
    [SerializeField] int punchesPerCrack = 10;
    [SerializeField] float handDissolvingTime = 1f;
    [SerializeField] Sprite[] sprites;
    [SerializeField] AudioClip[] glassBreakSFXs;
    [SerializeField] AudioClip[] glassPunchSFXs;


    private int index = 0;
    private float percentage = 0f;
    private float stepPercentage;
    private bool isHaunted = false;
    private bool lastIsHaunted = false;


    private SpriteRenderer crackSpriteRenderer;
    private AudioSource audioSource;
    private AudioSource hitAudioSource;


    private Monster monster;

    private MonsterWindow[] monsters;

    private int crackCount = 0;
    private Material handMat;
    private HandOnWindow handOnWindow;

	void Start () {


        crackSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        monster = transform.parent.GetComponentInChildren<Monster>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.maxDistance = 30f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.spatialBlend = 1f;
        audioSource.loop = false;


        hitAudioSource = gameObject.AddComponent<AudioSource>();
        hitAudioSource.maxDistance = 30f;
        hitAudioSource.rolloffMode = AudioRolloffMode.Linear;
        hitAudioSource.spatialBlend = 1f;
        hitAudioSource.loop = false;

        //Register Monsters
        monsters = transform.parent.GetComponentsInChildren<MonsterWindow>();
        if (monsters.Length > 0)
        {
            foreach(var monster in monsters)
            {
                monster.RegisterOnAliveEvents(IsHaunted);
                monster.RegisterOnDeadEvents(NotHaunted);
                monster.RegisterOnSlapEvent(Slapped);
                //monster.RegisterHandDissoving(SetHandDissolveRate);
            }
        }

        /*
        if(progressionTime<=0f)
        {
            stepPercentage = 1f;
        }
        else
        {
            stepPercentage = 1f / progressionTime;
        }
        */

        try { handMat = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>().material; }
        catch { }
        //handOnWindow = transform.parent.GetComponentInChildren<HandOnWindow>();
        //StartCoroutine(HandDissolving());
    }
	
	// Update is called once per frame
	void Update () {
        //Cracking();
	}

    public void Slapped()
    {
        crackCount += 1;
        if (crackCount == punchesPerCrack)
        {
            NextTexture();
            PlayGlassBreakSFX();
            crackCount = 0;
        }
        else
        {
            PlayPunchSFX();
        }
    }
    /*
    private void Cracking()
    {
       if(isHaunted)
       {
            percentage += stepPercentage*Time.deltaTime;
            if(percentage>1f)
            {
                percentage = 0f;

                crackCount += 1;
                try { handOnWindow.Slap(); }
                catch { } 
                if (crackCount==punchesPerCrack)
                {
                    NextTexture();
                    PlayGlassBreakSFX();
                    crackCount = 0;

                }
                else
                {
                    PlayPunchSFX();
                }

            }
            if(!lastIsHaunted)
            {
                StopCoroutine(HandDissolving());
                //handMat.SetFloat("_DissolveThreshold",0f);
            }
       }
       else
       {
            percentage = 0f;
            if(lastIsHaunted)
            {
                //StartCoroutine(HandDissolving());
            }
       }
        lastIsHaunted = isHaunted;
    }
    */

    private void NextTexture()
    {
        Debug.Log("NextSprite");
        if(index>=sprites.Length)
        {
            index = sprites.Length-1;
            return;
        }
        try
        {
            crackSpriteRenderer.sprite = sprites[index];
        }
        catch
        {
            Debug.LogError("No crack sprites");
        }
        index += 1;
        
    }

    private void PlayGlassBreakSFX()
    {
        audioSource.Stop();
        audioSource.clip = glassBreakSFXs[Random.Range(0, glassBreakSFXs.Length)];
        audioSource.Play();
    }

    public void IsHaunted()
    {
        isHaunted = true;
    }

    public void NotHaunted()
    {
        isHaunted = false;
    }

    private void PlayPunchSFX()
    {
        hitAudioSource.Stop();
        hitAudioSource.clip = glassPunchSFXs[Random.Range(0, glassPunchSFXs.Length)];
        hitAudioSource.Play();
    }

    IEnumerator HandDissolving()
    {
        float startTime = Time.time;
        while (Time.time < startTime + handDissolvingTime)
        {
            handMat.SetFloat("_DissolveThreshold", (Time.time - startTime) / handDissolvingTime);
            yield return new WaitForSeconds(0.05f);
        }
        yield break;
    }

    public void SetHandDissolveRate(float rate)
    {
        handMat.SetFloat("_DissolveThreshold", rate);
    }
}
