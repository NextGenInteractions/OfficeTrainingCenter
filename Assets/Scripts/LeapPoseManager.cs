using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapPoseManager : MonoBehaviour {
    [Header("Leap Motion - Hand Meshes")]
    public GameObject leftHandLeapMesh;
    public GameObject rightHandLeapMesh;

    [Header("Static Variables")]
    public static bool leftHandPosed = false;
    public static bool rightHandPosed = false;
    public static GameObject objectTouched;

    [Header("General Settings")]
    public float userObjectDistanceThreshold = 0.8f;

    // Private variables for holding information
    private enum Visiblity { left, right, both, none };
    private Camera mainCamera; //Carries a instance of the main camera to know distance of the user wrt the object touched
    private Visiblity currentVisibility;

    // Use this for initialization
    void Start () {
        mainCamera = Camera.main; //Getting a instance of the main camera for object-user distance calculation
    }
	
	// Update is called once per frame
	void Update () {

        // Update user hand visiblity wrt the leapmotion
        UpdateHandVisibility();

        // Constantly track user-object distance for disabling hand poses when the user is too far from the object
        TrackUserDistance();

        // Show poses based on boolean variables
        PoseHands();

        //Hide poses and enable real-time tracking meshes
        UnPoseHands();
	}

    /// <summary>
    /// This method controls when the leap hand meshes appear and vanish. They are primarly triggered
    /// by the leftHandPosed/rightHandPosed variables controlled by ObjectPoseConfiguration scripts on the objects.
    /// </summary>
    private void PoseHands()
    {
        if(leftHandPosed) // When left hand enters an object mesh, left hand pose is enabled
        { 
            objectTouched.GetComponent<ObjectPoseConfiguration>().leftHandPose.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
            leftHandLeapMesh.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        if(rightHandPosed) // When right hand hand enters an object mesh, right hand pose is enabled
        {
            objectTouched.GetComponent<ObjectPoseConfiguration>().rightHandPose.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
            rightHandLeapMesh.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// Similar to the PoseHands method, this method enables live tracking meshes based on leftHandPosed/rightHandPosed
    /// values controlled by ObjectPoseConfiguration script on the interactable object.
    /// </summary>
    private void UnPoseHands()
    {
        if(objectTouched != null)
        {
            if (!leftHandPosed)// When left hand exits an object mesh, live leap motion left hand is enabled
            {
                objectTouched.GetComponent<ObjectPoseConfiguration>().leftHandPose.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                leftHandLeapMesh.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }
            if (!rightHandPosed)// When right hand exits an object mesh, live leap motion right hand is enabled
            {
                objectTouched.GetComponent<ObjectPoseConfiguration>().rightHandPose.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                rightHandLeapMesh.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }
        }
        
    }

    /// <summary>
    /// As an additional hueristic of removing hand poses, we disable hand poses when the user is 0.8f units away from the interactable object.
    /// </summary>
    private void TrackUserDistance()
    {
        if(objectTouched != null)
        {
            // Calculate object distance from the user, using main camera transform
            float objectDistance = Vector3.Distance(objectTouched.transform.position, mainCamera.transform.position);
            if (objectDistance > userObjectDistanceThreshold) //If the distance is bigger than user-object distance threshold, disable the poses
            {
                rightHandPosed = false;
                leftHandPosed = false;
            }
        }        
    }

    /// <summary>
    /// Checking live leap motion hand visibility by using the object visibility in the hirarchy.
    /// </summary>
    void UpdateHandVisibility()
    {
        if (rightHandLeapMesh.activeSelf && leftHandLeapMesh.activeSelf)
        {
            currentVisibility = Visiblity.both; // When both hands are visible and tracking
        }

        if (rightHandLeapMesh.activeSelf && !leftHandLeapMesh.activeSelf)
        {
            currentVisibility = Visiblity.right; // When only right hand is visible
            leftHandPosed = false; // Disable the left hand pose, as only right hand is visible (backup heuristic)
        }

        if (leftHandLeapMesh.activeSelf && !rightHandLeapMesh.activeSelf)
        {
            currentVisibility = Visiblity.left; // When only left hand is visible
            rightHandPosed = false;// Disable the right hand pose, as only left hand is visible (backup heuristic)
        }
        
        if(!rightHandLeapMesh.activeSelf && !leftHandLeapMesh.activeSelf)
        {
            currentVisibility = Visiblity.none; // When no hands are visible
            leftHandPosed = false;
            rightHandPosed = false;
        }

    }
}
