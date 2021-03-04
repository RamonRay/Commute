using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TunnelGenerator : MonoBehaviour {

    [SerializeField] Transform TrainTransform;
    [SerializeField] TrainController TrainController;
    [SerializeField] Vector3 TunnelMovingDirection = Vector3.forward;
    [SerializeField] float TunnelPrefabLength;
    [SerializeField] Object prefab;
    [SerializeField] PlayableDirector playableDirector;
    public static TunnelGenerator instance { get; private set; }
    private bool isGenerating = true;
    private float movingSpeed;
    private float distanceTraveled;
    private Light[] tunnelLights;
    private float initialLightingIntensity;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There are more than one Tunnel Generator in the scene");
        }
    }

    // Use this for initialization
    void Start () {
        this.transform.position = TrainTransform.position;
        distanceTraveled = 0;
        UpdateLightComponents();
        initialLightingIntensity = tunnelLights[0].intensity;
        // For test ONLY
        //movingSpeed = 60f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate() {

        //if(Input.GetKeyDown(KeyCode.L)) {
        //    EndGame();
        //}

        UpdateMovingSpeed();
        float distanceCurFrame = movingSpeed * Time.deltaTime;
        distanceTraveled += distanceCurFrame;
        Vector3 newPos = transform.position + TunnelMovingDirection.normalized * distanceCurFrame;
        transform.position = newPos;

        if(distanceTraveled >= TunnelPrefabLength && isGenerating) {
            distanceTraveled = 0;
            Instantiate(prefab, TrainTransform.position - 3 * TunnelMovingDirection * TunnelPrefabLength, Quaternion.identity, transform);
            UpdateLightComponents();
        }
    }

    private void UpdateMovingSpeed() {
        movingSpeed = TrainController.velocity;
    }

    public void SetTunnelLightIntensity(float intensity)
    {
        float finalIntensity = intensity * initialLightingIntensity;
        try
        {
            foreach (var light in tunnelLights)
            {
                light.intensity = finalIntensity;
            }
        }
        catch
        {
            UpdateLightComponents();
            foreach (var light in tunnelLights)
            {
                light.intensity = finalIntensity;
            }
        }
    }

    public void UpdateLightComponents()
    {
        this.tunnelLights = GetComponentsInChildren<Light>();
    }

    public void EndGame() {
        isGenerating = false;
        playableDirector.Play();
    }
}
