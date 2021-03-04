using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveManager : MonoBehaviour {
    [SerializeField] LightingRequests lightingCtrl;
    [SerializeField] FloorController floorCtrl;
    [SerializeField] float subwayRunningFloorHeight = 5f;
    [SerializeField] float subwayCrushedFloorHeight = 5f;
    [SerializeField] float slightlyTiltAmount = 1f;
    [SerializeField] float midTiltAmount = 1.8f;

    private float accelTime = 10f;
    // Use this for initialization
	void Start () {
        // Turn on Guest light
        floorCtrl.enable();
        Invoke("TurnOnWhiteGuestLight", 1.2f);

        // Reset floor to zero
        //floorCtrl.resetFloor();
        TurnOffLights();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TurnOffLights() {
        lightingCtrl.Blackout();
    }

    public void TurnOnWhiteGuestLight() {
        lightingCtrl.GuestLights(0);
    }

    public void TurnOnRedGuestLight() {
        lightingCtrl.GuestLights(1);
    }

    public void FloorSubwayStart(float accelTime) {
        this.accelTime = accelTime;
        StartCoroutine(SubwayStart()); 
    }

    // Should be invoked after the floor is lifted up.
    public void FloorSubwayStop(float brakeTime) {
        floorCtrl.enable();
        floorCtrl.tiltVertical(subwayCrushedFloorHeight, 10.0f);
        //floorCtrl.raiseBack(10f);
        Invoke("FloorSetToCrushedStatus", brakeTime);
    }

    public void FloorHorizontalShake(float lastTime) {

    }

    // When the subway is running normally, we raise floor to a certain height
    public void FloorUp() {
        floorCtrl.moveAll(subwayRunningFloorHeight);
        TurnOnWhiteGuestLight();
    }

    public void FloorSetToCrushedStatus() {
        floorCtrl.moveAll(subwayCrushedFloorHeight);
        TurnOnRedGuestLight();
    }

    public void TiltLeftSlightly() {
        floorCtrl.tiltHorizontal(subwayRunningFloorHeight - slightlyTiltAmount, subwayRunningFloorHeight + slightlyTiltAmount);
    }

    public void TiltRightSlightly() {
        floorCtrl.tiltHorizontal(subwayRunningFloorHeight + slightlyTiltAmount, subwayRunningFloorHeight - slightlyTiltAmount);
    }

    public void TiltRightMid() {
        floorCtrl.tiltHorizontal(subwayRunningFloorHeight + midTiltAmount, subwayRunningFloorHeight - midTiltAmount);
    }

    public void TiltRightAndBrake() {
        floorCtrl.moveOne(0, 10);
        floorCtrl.moveOne(1, 5);
        floorCtrl.moveOne(2, 5);
        floorCtrl.moveOne(3, 0);
    }

    public void FLoorReset()
    {
        floorCtrl.resetFloor();
    }

    IEnumerator SubwayStart()
    {
        floorCtrl.enable();
        float frontVol = 5f;
        float backVol = 0f;
        while(frontVol<10f)
        {
            floorCtrl.tiltVertical(frontVol, backVol);
            frontVol += 1f;
            backVol += 1f;
            yield return new WaitForSeconds(accelTime/10f);
        }
        while(backVol<10f)
        {
            floorCtrl.tiltVertical(frontVol, backVol);
            backVol += 1f;
            yield return new WaitForSeconds(accelTime / 10f);
        }
        floorCtrl.tiltVertical(frontVol, backVol);
        yield break;
    }
}
