using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Monster : MonoBehaviour {
    //[SerializeField] SlideDoorHaunted[] doors;
    [SerializeField] Vector2 reviveTimeRange=new Vector2(5f,8f);
    [SerializeField] Vector2 randomScreechInterval = new Vector2(4f, 8f);
    [SerializeField] AudioClip[] screechSFXs;
    [Range(0f,1f)]
    [SerializeField] float maxVolumn=0.2f;
    [SerializeField] KeyCode aliveKey;
    [SerializeField] float litTimeBeforeDead=2f;
    [SerializeField] float fullShowUpTime = 0.5f;

    //Materials on hands that belongs to the monster.
    [Tooltip("Length of doors and hands has to be the same or thing will go wrong")]
    [SerializeField] GameObject[] hands;
    [SerializeField] GameObject[] doors;
    [SerializeField] SkinnedMeshRenderer[] skins;

    //[SerializeField] bool isSlapping =false;
    
    private Material[] mats;
    protected MonsterController monsterController;

    private float screechVanishingTime;
    private float dissolveRate = 1f;

    private float aliveTimeStamp=0f;


    private bool _isAlive=false;
    public bool isAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            bool oldIsAlive = _isAlive;
            _isAlive = value;
            if(_isAlive!=oldIsAlive)
            {
                if(_isAlive)
                {
                    Alive();
                }
                else
                {
                    StopAllCoroutines();
                    //StartCoroutine(ScreechSFXVanishing());
                    //audioSource.Stop();
                    //audioSource.Play();
                    Dead();
                }
            }
        }
    }
    private bool _isLit = false;
    public bool isLit
    {
        get
        {
            return _isLit;
        }
        set
        {
            bool oldIsLit = _isLit;
            _isLit = value;
            //When the time the light starts to shine on the monster or the monster is dead
            if((!oldIsLit&&_isLit)||!_isAlive)
            {
                lastLitTime = Time.time;
                //Play Sound
                //StopCoroutine(RandomScreechSFX());
                try
                {
                    StopCoroutine(HandStartShowingUp());
                }
                catch
                {
                    Debug.LogError("No hand is showing up now");
                }
            }
            if(_isLit)
            {
                if (Time.time > lastLitTime + litTimeBeforeDead&&isAlive)
                {
                    isAlive = false;
                }
                else
                {
                    //audioSource.volume = maxVolumn-(Time.time - lastLitTime) / litTimeBeforeDead * maxVolumn;
                    dissolveRate = Mathf.Clamp(dissolveRate + Time.deltaTime / litTimeBeforeDead, 0f, 1f);
                    SetDissolveRate(dissolveRate);
                }
            }
            else
            {
                
                if(isAlive)
                {
                    audioSource.volume = maxVolumn;
                    StartCoroutine(HandStartShowingUp());
                    StartCoroutine(RandomScreechSFX());
                }
            }
        }
    }

    private float lastLitTime=0f;



    //Components on gameobject
    private BlackSmokeController blackSmoke;
    private AudioSource audioSource;

    //Events called on alive or dead.
    Action onAliveEnvets;
    Action onDeadEvents;
    //Action<float> setDissolveRate;
	// Use this for initialization
	protected virtual void Start ()
    {
        blackSmoke = GetComponent<BlackSmokeController>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.maxDistance = 30f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.spatialBlend = 1f;
        audioSource.volume = maxVolumn;

        //Get the materials from the hands components.

        if (skins.Length > 0)
        {
            mats = new Material[skins.Length];
            for (int index = 0; index < skins.Length; index++)
            {
                mats[index] = skins[index].material;
            }
        }
        SetDissolveRate(dissolveRate);
        //Attach the hands to the door.
        if(doors.Length>0&&doors.Length==hands.Length)
        {
            for (int index = 0; index < doors.Length; index++)
            {
                hands[index].transform.parent = doors[index].transform;
            }
        }
        else
        {
            Debug.LogError("Length of doors and hands has to be the same");
        }
        monsterController = transform.parent.GetComponentInChildren<MonsterController>();

    }
	
	// Update is called once per frame
	void Update () {
        KeyboradAlive();
	}

    //Functions called when bool value isAlive is changed.
    protected virtual void Dead()
    {
        monsterController.ChildMonsterDead();
        //MonsterDifficultyControl.instance.UpdateDifficulty(Time.time - aliveTimeStamp);
        if(onDeadEvents!=null)
        {
            onDeadEvents.Invoke();
        }
        StartCoroutine(ScreechSFXVanishing());
        if(dissolveRate<1f)
        {
            StartCoroutine(HandDissolving());
        }
    }
    protected virtual void Alive()
    {
        //aliveTimeStamp = Time.time;
        audioSource.volume = maxVolumn;
        StartCoroutine(RandomScreechSFX());

        if (onAliveEnvets != null)
        {
            onAliveEnvets.Invoke();
        }
        StartCoroutine(HandStartShowingUp());
    }


    public void RegisterOnAliveEvents(Action _action)
    {
        onAliveEnvets += _action;
    }

    public void RegisterOnDeadEvents(Action _action)
    {
        onDeadEvents += _action;
    }
    //Not Reviving
    IEnumerator ReviveAfterRandomTime()
    {
        float reviveTime = UnityEngine.Random.Range(reviveTimeRange.x, reviveTimeRange.y);
        yield return new WaitForSeconds(reviveTime);
        Alive();
        yield break;
    }
    
    IEnumerator RandomScreechSFX()
    {
        //Currently only play sfx when it dies
        yield break;
        audioSource.volume = maxVolumn;
        while (true)
        {
            audioSource.clip = screechSFXs[UnityEngine.Random.Range(0, screechSFXs.Length)];
            audioSource.Play();
            yield return new WaitForSeconds(UnityEngine.Random.Range(audioSource.clip.length + randomScreechInterval.x, audioSource.clip.length + randomScreechInterval.y));
        }
    }
    IEnumerator ScreechSFXVanishing()
    {
        AudioClip clip = screechSFXs[UnityEngine.Random.Range(0, screechSFXs.Length)];
        float clipLength = clip.length;
        audioSource.clip = clip;
        audioSource.Play();
        float startTime = Time.time;
        while (Time.time < startTime + clipLength)
        {
            audioSource.volume = Mathf.Clamp(maxVolumn-maxVolumn*(Time.time - startTime - 1f) / (clipLength - 1f), 0f, maxVolumn);
            yield return new WaitForSeconds(0.02f);
        }
        yield break;
    }

    //Press corresponding key to wake the monster up.
    public void KeyboradAlive()
    {
        if(Input.GetKeyDown(aliveKey))
        {
            if(Input.GetKey(KeyCode.LeftShift)&&isAlive)
            {
                isAlive = false;
            }
            else if(!isAlive)
            {
                isAlive = true;
            }

        }
    }


    /*
    public void RegisterHandDissoving(Action<float> action)
    {
        setDissolveRate += action;
    }
    */


    IEnumerator HandStartShowingUp()
    {
        float startTime = Time.time;
        while (Time.time<startTime+fullShowUpTime)
        {
            dissolveRate = Mathf.Clamp(1f-(Time.time-startTime)/fullShowUpTime,0,1);
            SetDissolveRate(dissolveRate);
            yield return 0;
        }
        SetDissolveRate(0f);
        yield break;
    }


    IEnumerator HandDissolving()
    {
        float startTime = Time.time;
        while (Time.time < startTime + litTimeBeforeDead)
        {
            dissolveRate = Mathf.Clamp((Time.time - startTime) / litTimeBeforeDead, 0, 1);
            SetDissolveRate(dissolveRate);
            yield return 0;
        }
        SetDissolveRate(1f);
        yield break;
    }

    public void SetDissolveRate(float _dissolveRate)
    {
        if (mats.Length < 1)
        {
            return;
        }

        foreach(Material mat in mats)
        {
            mat.SetFloat("_DissolveThreshold", _dissolveRate);
        }
    }


}
