using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using SubjectNerd.Utilities;

public class GameManager : MonoBehaviour {
    [SerializeField] CameraMovement cameraMovement;
    [SerializeField] CameraMovement cameraRig;
    [SerializeField] LightingManager lightingManager;
    [SerializeField] CaveManager caveManager;
    [Reorderable]
    [SerializeField] UnityEvent[] gameSequence;

    private int index = 0;

    private Coroutine shakeCoroutine;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            NextEvent();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void NextEvent()
    {
        gameSequence[index].Invoke();
        index++;
        if(index==gameSequence.Length)
        {
            index = 0;
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }


    public void SubwayFixed()
    {
        cameraRig.Untilted();
        //lightingManager.LightNormal();
        //lightingManager.LightsInOtherTrainOn();
        try
        {
            caveManager.FLoorReset();
        }
        catch { }
    }

    public void SubwayLightOn() {
        lightingManager.LightNormal();
        lightingManager.LightsInOtherTrainOn();
        // Train Front light should be turned on here , not in SubwayFixed
    }

    public void RestartGame()
    {
        try
        {
            DMXController.Lighting.Blackout();
            caveManager.FLoorReset();
        }
        catch
        { 
            //
        }
        SceneManager.LoadSceneAsync(0);
    }

    public void FinalShake()
    {
        shakeCoroutine = StartCoroutine(Shake(0.2f, 15f));
    }

    IEnumerator Shake(float shakeInterval, float shakeTime)
    {
        float startTime = Time.time;
        int i = 0;
        float shakeIntensity = 0.5f;
        while (Time.time < startTime + shakeTime)
        {

            //shake left

            cameraMovement.CameraShake(-shakeIntensity * Random.Range(0.6f, 1.6f), shakeInterval / 2f);
            try
            {
                if (i % 10 == 1)
                    caveManager.TiltLeftSlightly();
            }
            catch
            {
                //Debug.LogError("NoCave!");
            }
            yield return new WaitForSeconds(shakeInterval / 2f);
            //shake right
            cameraMovement.CameraShake(shakeIntensity * Random.Range(0.6f, 1.6f), shakeInterval / 2f);
            try
            {
                if (i % 10 == 6)
                    caveManager.TiltRightSlightly();
            }
                
            catch
            {
                //Debug.LogError("NoCave!");
            }
            i++;
            shakeIntensity += shakeInterval/shakeTime * 10f;
            yield return new WaitForSeconds(shakeInterval / 2f);
        }
    }

    public void StopShake()
    {
        StopCoroutine(shakeCoroutine);
        StartCoroutine(StopShakeCoroutine());
    }

    IEnumerator StopShakeCoroutine()
    {
        //yield return new WaitForSeconds(1f);
        SubwayFixed();
        yield break;
    }

}
