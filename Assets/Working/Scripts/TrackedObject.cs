using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;
using Valve.VR;
using SteamVRPlayer = Valve.VR.InteractionSystem.Player;
using System;

// Add this script as a component to the GameObject you want to track
public class TrackedObject : MonoBehaviour
{

    public enum TrackedDevice
    {
        Tracker1 = 0,
        Tracker2,
        Tracker3,
        Tracker4,
    }

    [Header("Don't change when playing")]
    [SerializeField]
    public TrackedDevice trackedDevice;

    private TrackerObject tracker;

    private static List<TrackerObject> trackers = new List<TrackerObject>();
    private static Transform cameraRig;
    private static UnityEvent OnNewTracker = new UnityEvent();

    private static bool initialized = false;

    public bool IsTracked
    {
        get {
            return initialized &&
                (tracker != null && tracker.trackedObject != null && tracker.trackedObject.isValid);
        }
    }

    private void Awake()
    {
        CheckIfInitialized();

        OnNewTracker.AddListener(UpdateTrackerReference);
    }

    private void CheckIfInitialized()
    {
        // Check if the scene has been destroyed
        if (initialized) {
            bool isDestroyed = false;

            foreach (var trackerObj in trackers) {
                if (trackerObj.gameObject == null || trackerObj.trackedObject == null) {
                    isDestroyed = true;
                    break;
                }
            }

            // Destroy the old tracker lists
            if (isDestroyed) {
                trackers.Clear();
                initialized = false;
            }
        }

        if (!initialized) {
            Initialize();
        }
    }

    private void OnDestroy()
    {
        OnNewTracker.RemoveListener(UpdateTrackerReference);
    }

    private void OnEnable()
    {
        SteamVR_Events.NewPoses.AddListener(OnNewPoses);
    }

    private void OnDisable()
    {
        SteamVR_Events.NewPoses.RemoveListener(OnNewPoses);
    }

    private void Update()
    {
        if (IsTracked) {
            var trackerTransform = tracker.trackedObject.transform;
            transform.rotation = trackerTransform.rotation;
            transform.position = trackerTransform.position;

            if (!tracker.lastIsTracked) {
                tracker.lastIsTracked = true;
                tracker.trackedObject.name = tracker.trackedObject.name.Replace(" [Untracked]", "");
            }
        } else if (tracker != null && tracker.trackedObject != null) {
            if (tracker.lastIsTracked) {
                tracker.lastIsTracked = false;
                tracker.trackedObject.name += " [Untracked]";
                Debug.LogWarningFormat("{0} is being referenced but not tracked properly. "
                    + "Check if all trackers are turned on and visible to the base stations", trackedDevice.ToString());
            }
        }
    }

    private void UpdateTrackerReference()
    {
        if (tracker != null) {
            return;
        }

        if (trackers.Count <= (int)trackedDevice) {
            Debug.LogErrorFormat("You are trying to use {0} but there is only {1} tracker(s) connected. "
                    + "Make sure the trackers are turned on and show stable green light. If the problem persists, find a TA.",
                    trackedDevice.ToString(), trackers.Count);
            return;
        }

        tracker = trackers[(int)trackedDevice];
    }

    private static void Initialize()
    {
        // Initialize SteamVR in case it is not initialized by other scripts
        SteamVR.Initialize();
        
        var playArea = (SteamVR_PlayArea)GameObject.FindObjectOfType<SteamVR_PlayArea>();
        if (playArea != null) {
            cameraRig = playArea.transform;
            initialized = true;
            return;
        }

        var player = (SteamVRPlayer)GameObject.FindObjectOfType<SteamVRPlayer>();
        if (player != null) {
            cameraRig = player.transform;
            initialized = true;
            return;
        }

        var camera = Array.Find(Camera.allCameras, cam => cam.isActiveAndEnabled && cam.stereoTargetEye == StereoTargetEyeMask.Both);
        if (camera != null) {
            var cameraRigGo = new GameObject("CameraRig");
            cameraRigGo.transform.position = camera.transform.position;
            cameraRigGo.transform.rotation = camera.transform.rotation;

            cameraRig = cameraRigGo.transform;
            initialized = true;
            return;
        }

        // If no camera rig found
        Debug.LogError("No [CameraRig] or [SteamVR Player Prefab] or active [Camera] is found! [TrackedObject] script will not work!");
        return;
    }

    private static void OnNewPoses(TrackedDevicePose_t[] poses)
    {
        if (poses == null)
            return;

        bool newTrackerDetected = false;

        for (uint deviceIndex = 0; deviceIndex < poses.Length; deviceIndex++) {
            if (!trackers.Exists((tracker) => tracker.deviceIndex == deviceIndex)) {
                ETrackedDeviceClass deviceClass = OpenVR.System.GetTrackedDeviceClass(deviceIndex);

                if (deviceClass == ETrackedDeviceClass.GenericTracker) {
                    var tracker = new TrackerObject();
                    tracker.deviceIndex = (int)deviceIndex;
                    tracker.gameObject = new GameObject("Tracker " + (trackers.Count + 1).ToString());
                    tracker.trackedObject = tracker.gameObject.AddComponent<SteamVR_TrackedObject>();
                    tracker.trackedObject.origin = cameraRig;
                    tracker.trackedObject.SetDeviceIndex((int)deviceIndex);
                    tracker.lastIsTracked = true;

                    trackers.Add(tracker);

                    newTrackerDetected = true;
                }
            }
        }

        if (newTrackerDetected) {
            OnNewTracker.Invoke();
        }
    }

    private class TrackerObject
    {
        public int deviceIndex;
        public GameObject gameObject;
        public SteamVR_TrackedObject trackedObject;
        public bool lastIsTracked;
    }
}