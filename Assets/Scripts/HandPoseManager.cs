using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandPoseManager : MonoBehaviour {

    public Transform trackedRightHand;
    public Transform trackedLeftHand;
    public GameObject rightLeapHand;
    public GameObject leftLeapHand;

    public Transform[] interactableObjects;

    public TextMesh debugDisplay;
    public TextMesh inPoseDisplay;

    private int closestObject = -1;
    private enum Hand { left, right, both};
    private enum Visiblity { left, right, both, none};
    private Hand closestHand;
    private int oldClosestHand = -1;
    private Visiblity currentVisibility;
    private float handObjectDistance = 0.0f;
    private float currentOjectHandThreshold = 0.0f;
    private float mininumObjectDistance = 100000.0f; //higer value

    public Material testMaterial;
    public Material defaultMaterial;

    public GameObject rightHand;
    public GameObject leftHand;
    private GameObject trackedHand;
    private GameObject poseHand;
    private Transform nearestObject;
    private Transform oldNearestObject;
    private Visiblity oldVisibility;
    private bool currentInPose;
    // Use this for initialization
    void Start () {

        
		
	}

    // Update is called once per frame
    void Update()
    {

        CheckVisibility();
        

        
        if(currentVisibility==Visiblity.left)
        {
            trackedHand = leftLeapHand;
            nearestObject = CalculateObjectDistance(Hand.left);
            poseHand = nearestObject.gameObject.GetComponent<GraspConfig>().leftHandPose;
        }
        else if(currentVisibility == Visiblity.right)
        {
            trackedHand = rightLeapHand;
            nearestObject = CalculateObjectDistance(Hand.right);
            poseHand = nearestObject.gameObject.GetComponent<GraspConfig>().rightHandPose;
        }
        else if(currentVisibility ==Visiblity.both)
        {
            nearestObject = CalculateObjectDistance(Hand.both);
            
            if (closestHand == Hand.left)
            {
                trackedHand = leftLeapHand;
                poseHand = nearestObject.gameObject.GetComponent<GraspConfig>().leftHandPose;
            }
            else if(closestHand == Hand.right)
            {
                trackedHand = rightLeapHand;
                poseHand = nearestObject.gameObject.GetComponent<GraspConfig>().rightHandPose;
            }
        }


        if(currentVisibility != Visiblity.none)
        {
            float selectedHandDistance = 0.0f;
            if(closestHand == Hand.right)
            {
                selectedHandDistance = Vector3.Distance(nearestObject.position, trackedRightHand.position);
            }
            else if(closestHand == Hand.left)
            {
                selectedHandDistance = Vector3.Distance(nearestObject.position, trackedLeftHand.position);
            }

            currentOjectHandThreshold = nearestObject.gameObject.GetComponent<GraspConfig>().objectHandThreshold;
            currentInPose = nearestObject.gameObject.GetComponent<GraspConfig>().inPose;

            if (selectedHandDistance <= currentOjectHandThreshold)
            {
                trackedHand.GetComponent<SkinnedMeshRenderer>().enabled = false;
                poseHand.GetComponent<SkinnedMeshRenderer>().enabled = true;
                nearestObject.gameObject.GetComponent<GraspConfig>().inPose = true;
                
            }
            else
            {
                trackedHand.GetComponent<SkinnedMeshRenderer>().enabled = true;
                poseHand.GetComponent<SkinnedMeshRenderer>().enabled = false;
                nearestObject.gameObject.GetComponent<GraspConfig>().inPose = false;
            }
        }


        DisablePoses();

    }


    void CheckVisibility()
    {
        oldVisibility = currentVisibility;

        if (rightHand.activeSelf && leftHand.activeSelf)
        {
            currentVisibility = Visiblity.both;
            debugDisplay.text = Visiblity.both.ToString();
        }
        else if (rightHand.activeSelf)
        {
            currentVisibility = Visiblity.right;
            debugDisplay.text = Visiblity.right.ToString();
        }
        else if (leftHand.activeSelf)
        {
            currentVisibility = Visiblity.left;
            debugDisplay.text = Visiblity.left.ToString();
        }
        else
        {
            currentVisibility = Visiblity.none;
            debugDisplay.text = Visiblity.none.ToString();
        }

        if(oldVisibility!=currentVisibility)
        {
            inPoseDisplay.text = oldVisibility.ToString();
        }
    }

    void DisablePoses()
    {

        if (oldNearestObject != null)
        {

            if (currentVisibility == Visiblity.left)
            {
                GameObject posedHand = oldNearestObject.gameObject.GetComponent<GraspConfig>().rightHandPose;
                posedHand.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
            else if (currentVisibility == Visiblity.right)
            {
                GameObject posedHand = oldNearestObject.gameObject.GetComponent<GraspConfig>().leftHandPose;
                posedHand.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
            else if (currentVisibility == Visiblity.none)
            {
                
                GameObject rightPosedHand = oldNearestObject.gameObject.GetComponent<GraspConfig>().rightHandPose;
                GameObject leftPosedHand = oldNearestObject.gameObject.GetComponent<GraspConfig>().leftHandPose;

                rightPosedHand.GetComponent<SkinnedMeshRenderer>().enabled = false;
                leftPosedHand.GetComponent<SkinnedMeshRenderer>().enabled = false;

            }
        }
    }

    // Calculating hand object distance with all interactable objects
    Transform CalculateObjectDistance(Hand visibleHand)
    {
        for (int i=0; i < interactableObjects.Length; i++)
        {
            if(visibleHand == Hand.left)
            {
                handObjectDistance = Vector3.Distance(interactableObjects[i].position, trackedLeftHand.position);
            }
            else if(visibleHand == Hand.right)
            {
                handObjectDistance = Vector3.Distance(interactableObjects[i].position, trackedRightHand.position);
            }
            else if(visibleHand == Hand.both)
            {
                float rightHandDist = Vector3.Distance(interactableObjects[i].position, trackedRightHand.position);
                float leftHandDist = Vector3.Distance(interactableObjects[i].position, trackedLeftHand.position);

                handObjectDistance = rightHandDist < leftHandDist ? rightHandDist : leftHandDist;
                closestHand = rightHandDist < leftHandDist ? Hand.right : Hand.left;
            }
 

            if (handObjectDistance < mininumObjectDistance)
            {
                mininumObjectDistance = handObjectDistance;
                closestObject = i;
                /*if(interactableObjects[i].tag != "interactable")
                    interactableObjects[i].GetComponent<Renderer>().material = testMaterial;*/
            }
            
        }

        /*for (int i = 0; i < interactableObjects.Length; i++)
        {
            if (i!= closestObject && interactableObjects[i].tag != "interactable")
            {
                interactableObjects[i].GetComponent<Renderer>().material = defaultMaterial;
            }
        }*/

        mininumObjectDistance = 100000.0f;
        handObjectDistance = 0.0f;

        if(oldNearestObject!=nearestObject)
        {
            oldNearestObject = nearestObject;
        }

        return interactableObjects[closestObject];
    }
}
