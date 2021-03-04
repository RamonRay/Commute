using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DoorController : MonoBehaviour {
    [SerializeField] SteamVR_Action_Boolean teleportButton;
    [SerializeField] SteamVR_Input_Sources controller;
    [SerializeField] GameObject train;
    [SerializeField] LockController doorLock;
    private SlidingDoor[] doors;
    
    // Use this for initialization
    void Start () {
        doors = train.GetComponentsInChildren<SlidingDoor>();
	}
	
	// Update is called once per frame
	void Update () {
        /*
		if(teleportButton.GetStateDown(controller))
        {
            foreach(var door in doors)
            {
                door.SwtichState();
            }
            doorLock.isLocked = !doorLock.isLocked;
        }
        */

	}
}
