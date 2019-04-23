// SetTrackerDeviceIDs.cs
// 
// Set SteamVR tracker IDs in a single location so we don't have to search through the scene hierarchy to find and change tracker IDs.
// Functionality is the same as setting device IDs on each individual SteamVR_TrackedObject game object, but having values in a single location enables fast configuration
//
// Created by Jason Jerald as suggested by Jason Haskins on April 23, 2019


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SetTrackerDeviceIDs : MonoBehaviour
{
    [System.Serializable]

    // Tracker class that contains a tracker and ID information
    // This would better be a struct as it only contains data but foreach code in SetTrackerDeviceIDs methods won't compile if a struct
    public class Tracker
    {
        public SteamVR_TrackedObject trackedObject;
        public SteamVR_TrackedObject.EIndex deviceID;
        internal SteamVR_TrackedObject.EIndex deviceIDPrevious;
    }

    // Array of Trackers
    public Tracker [] trackers;

    void Start()
    {
        // Initialize previous IDs to current IDs
        foreach (Tracker tracker in trackers)
        {
            if (tracker.trackedObject != null)
            {
                tracker.trackedObject.SetDeviceIndex((int)tracker.deviceID);
                tracker.deviceIDPrevious = tracker.deviceID;
            }
        }
    }


    void Update()
    {
        // If a device ID changed then update the referenced tracker with the new ID
        foreach (Tracker tracker in trackers)
        {
            if(tracker.trackedObject != null)
            {
                tracker.trackedObject.SetDeviceIndex((int)tracker.deviceID);
                tracker.deviceIDPrevious = tracker.deviceID;
            }
        }
    }
}
