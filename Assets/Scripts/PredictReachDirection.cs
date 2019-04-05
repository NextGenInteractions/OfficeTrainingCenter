using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictReachDirection : MonoBehaviour {

    public Transform trackedHand;
    public float handOffsetAngle = -45.0f;
    public float positionalAdjustment;
    public bool x, y, z;
    private Vector3 savedHookPosition;

    // Use this for initialization
    void Start () {
        savedHookPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        var targetPosition = trackedHand.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        transform.localRotation = transform.localRotation * Quaternion.Euler(0, handOffsetAngle, 0);

        if (y)
        {
            if (trackedHand.position.y <= savedHookPosition.y + positionalAdjustment && trackedHand.position.y > savedHookPosition.y - positionalAdjustment)
            {
                transform.position = new Vector3(transform.position.x, trackedHand.position.y, transform.position.z);
            }
        }
    }
}
