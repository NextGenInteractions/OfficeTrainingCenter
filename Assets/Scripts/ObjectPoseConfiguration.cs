using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoseConfiguration : MonoBehaviour {

    [Header("Hand Pose Meshes")]
    public GameObject leftHandPose;
    public GameObject rightHandPose;

    [Header("Pose Freedom Settings")]
    public bool poseFreedomRequired = false;

    [Header("Pose Constraint Settings")]
    public float positionalAdjustmentLimit = 0.4f;
    public bool xDimension;
    public bool yDimension;
    public bool zDimension;
    public float handRotationalOffsetAngle = 100.0f;

    private GameObject currentTrackedHand;

    // Private variables for saving object position reference
    private Vector3 savedHookPosition;

    /// <summary>
    /// Save the start position of the transform to move the hook later with tracked hand data
    /// </summary>
    private void Start()
    {
        if(poseFreedomRequired)
        {
            savedHookPosition = transform.position;
        }
        
    }

    /// <summary>
    /// Reading grasp approach with LookAt function for calculating hand approach. And further moving the hand transform
    /// based on tracked hand transform in the constrained dimension
    /// </summary>
    private void Update()
    {
        if(currentTrackedHand!=null && poseFreedomRequired)
        {
            // Rotating the hand pose based on tracked hand approach
            var targetPosition = currentTrackedHand.transform.position; // Getting current tracked hand from the trigger enter
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition); // Current hook is configured to look the hand model and get the rotation to know the hand approach
            transform.localRotation = transform.localRotation * Quaternion.Euler(0, handRotationalOffsetAngle, 0); // Adding a rotational offset to match the static pose to user apporach

            // Positional dispalcement freedom with respect to the current tracked hand
            if (yDimension)
            {
                // If the currentTrackedHand is in a range of area, then the pose is constrained in the y axis positionally
                if (currentTrackedHand.transform.position.y <= savedHookPosition.y + positionalAdjustmentLimit && currentTrackedHand.transform.position.y > savedHookPosition.y - positionalAdjustmentLimit)
                {
                    transform.position = new Vector3(transform.position.x, currentTrackedHand.transform.position.y, transform.position.z);
                }
            }

            if (xDimension)
            {// If the currentTrackedHand is in a range of area, then the pose is constrained in the x axis positionally
                if (currentTrackedHand.transform.position.x <= savedHookPosition.x + positionalAdjustmentLimit && currentTrackedHand.transform.position.x > savedHookPosition.x - positionalAdjustmentLimit)
                {
                    transform.position = new Vector3(currentTrackedHand.transform.position.x, transform.position.y, transform.position.z);
                }
            }

            if (zDimension)
            {// If the currentTrackedHand is in a range of area, then the pose is constrained in the z axis positionally
                if (currentTrackedHand.transform.position.z <= savedHookPosition.z + positionalAdjustmentLimit && currentTrackedHand.transform.position.z > savedHookPosition.z - positionalAdjustmentLimit)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, currentTrackedHand.transform.position.z);
                }
            }
        }
    }

    /// <summary>
    /// We enable the hand posed variable on LeapPoseManager script when the live leapmotion hands enter object collider.
    /// </summary>
    /// <param name="handCollider"></param>
    private void OnTriggerStay(Collider handCollider)
    {

        if (handCollider.gameObject.tag == "left_hand")
        {
            LeapPoseManager.leftHandPosed = true;
            LeapPoseManager.objectTouched = transform.gameObject;
            currentTrackedHand = handCollider.gameObject;
        }
        else if (handCollider.gameObject.tag == "right_hand")
        {
            LeapPoseManager.rightHandPosed = true;
            LeapPoseManager.objectTouched = transform.gameObject;
            currentTrackedHand = handCollider.gameObject;
        }
    }

    /// <summary>
    /// We disable the handPosed variable on LeapPoseManager script when the live leapmotion hands exit object collider.
    /// </summary>
    /// <param name="handCollider"></param>
    private void OnTriggerExit(Collider handCollider)
    {

        if (handCollider.gameObject.tag == "left_hand")
        {
            LeapPoseManager.leftHandPosed = false;
            LeapPoseManager.objectTouched = transform.gameObject;
            currentTrackedHand = null;
        }
        else if (handCollider.gameObject.tag == "right_hand")
        {
            LeapPoseManager.rightHandPosed = false;
            LeapPoseManager.objectTouched = transform.gameObject;
            currentTrackedHand = null;
        }
    }
}
