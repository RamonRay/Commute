using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class TrainController : MonoBehaviour {
    [SerializeField] CaveManager caveManager;
    [SerializeField] CameraMovement cameraMovement;
    [SerializeField] float acceleration=0.01f;
    [SerializeField] float deceleration=0.2f;
    [SerializeField] float topSpeed;
    [SerializeField] AudioClip movingSFX, shakySFX, decelerateSFX;
    [SerializeField] AudioClip hissingSFX;
    [SerializeField] AudioSource hissingSource;
    [SerializeField] Vector2 randomHissingSFXRange=new Vector2(10f,20f);
    [SerializeField] float shakeTime,shakeInterval,tiltedTimeBeforeDec;
    [SerializeField] LightingManager lightManager;
    [SerializeField] LockController lockedDoor;
    public float velocity { get; private set; }
    private Announcement announcement;
    private bool isAccelerating = false;
    private bool isBraking = false;
    private bool _pedal = false;
    public bool pedal
    {
        get
        {
            return _pedal;
        }
        set
        {
            bool oldPedal = _pedal;
            _pedal = value;
            if(_pedal&&!oldPedal)
            {
                StopAllCoroutines();
                AmbientSoundManager.instance.PlayNormalWhiteNoise();
                AudioSource _as = GetComponent<AudioSource>();
                _as.clip = movingSFX;
                _as.Play();
            }

        }
    }
    private bool crash = false;
    // Use this for initialization
    void Start () {
        velocity = 0f;
        announcement = transform.parent.GetComponentInChildren<Announcement>();
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.P))
        {
            pedal = !pedal;
        }

        if (pedal)
            Accelerate();
        else
            Decelerate();
        //MoveTrain();
	}
    private void MoveTrain()
    {
        
    }
    private void Accelerate()
    {
        if (velocity < topSpeed)
        {
            if(!isAccelerating)
            {
                try
                { 
                    caveManager.FloorSubwayStart((topSpeed-velocity) / acceleration);
                }
                catch
                {
                    Debug.LogError("NoCave!");
                }
            }
            velocity += acceleration*Time.deltaTime;
            isAccelerating = true;
            
        }
        else
        {
            if(isAccelerating)
            {
                //CaveManger Reset 
            }
            velocity = topSpeed;
            isAccelerating = false;
        }
    }
    private  void Decelerate()
    {
        if(velocity>0)
        {
            if(isBraking)
            {
                if (!crash)
                {
                    try
                    {
                        caveManager.FloorSubwayStop(velocity / deceleration);
                    }
                    catch
                    {
                        Debug.LogError("NoCave!");
                    }
                }
                else
                {
                    //Do it in coroutine;
                }
            }
            velocity -= deceleration*Time.deltaTime;
            isBraking = true;
        }
        else
        {
            if (isBraking)
            {
                //CaveManger Reset; 
            }
            velocity = 0f;
            isBraking = false;
        }
    }



    public void Go()
    {
        pedal = true;
    }

    public void Brake()
    {
        pedal = false;
    }

    public void TriggerCrash()
    {
        crash = true;
        StartCoroutine(Crash());
    }

    IEnumerator Crash()
    {
        float startTime = Time.time;
        int i = 0;
        //Shake for some time.
        //Platform Shaky, camera shaky
        //Stop moving Sound
        lightManager.LightStrobe(shakeTime+velocity/deceleration,shakeInterval/2);
        AudioSource _as = GetComponent<AudioSource>();
        _as.Stop();
        _as.clip = shakySFX;
        _as.Play();
        while (Time.time<startTime+shakeTime)
        {
            
            //shake left

            cameraMovement.CameraShake(-2f*Random.Range(0.3f,3f), shakeInterval / 2f);
            try
            {
                if(i%10==1)
                    caveManager.TiltLeftSlightly();
            }
            catch
            {
                //Debug.LogError("NoCave!");
            }
            yield return new WaitForSeconds(shakeInterval / 2f);
            //shake right
            cameraMovement.CameraShake(2f * Random.Range(0.3f, 3f), shakeInterval / 2f);
            try
            {
                if(i%10==6)
                    caveManager.TiltRightSlightly();
            }
            
            catch
            {
                //Debug.LogError("NoCave!");
            }
            i++;
            yield return new WaitForSeconds(shakeInterval / 2f);
        }
        //tilted, camera tileted. Finished Shaking
        cameraMovement.CameraShake(5f, shakeInterval / 2f);
        try
        {
            caveManager.TiltRightMid();
        }
        catch
        {
            Debug.LogError("NoCave!");
        }

        yield return new WaitForSeconds(tiltedTimeBeforeDec);
        //Starts deceleration. The platform is tilted and play screech sound;
        _as.Stop();
        _as.clip = decelerateSFX;
        _as.Play();
        pedal = false;
        try
        {
            //caveManager.TiltRightAndBrake();
        }
        catch
        {
            Debug.LogError("NoCave!");
        }
        yield return new WaitForSeconds(velocity / deceleration);
        lockedDoor.isLocked = true;
        StartCoroutine(RandomHissing());
        AmbientSoundManager.instance.PlaySpookyAmbient();
        
        //stay tilted after the train stops
        try
        {
            caveManager.TiltRightMid();
        }
        catch
        {
            Debug.LogError("NoCave!");
        }

        yield return new WaitForSeconds(1f);
        announcement.TrainCrash();
        yield break;
    }

    IEnumerator RandomHissing()
    {
        AudioSource _as = GetComponent<AudioSource>();
        _as.Stop();
        
        while(!pedal&&crash)
        {
            hissingSource.Play();
            float currentTime = Time.time;
            while (Time.time < currentTime + hissingSource.clip.length)
            {
                hissingSource.volume = (Time.time - currentTime) / hissingSource.clip.length*(-1f)+1f;
                yield return 0;
            }

            yield return new WaitForSeconds(Random.Range(randomHissingSFXRange.x,randomHissingSFXRange.y));
        }
        hissingSource.Stop();
        yield break;
    }

}
