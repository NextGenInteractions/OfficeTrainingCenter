using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlObject : MonoBehaviour {

    private int frameNumber=0;
    private Quaternion offset;
    public float angularThreshold = 0.2f;
    public Transform simpleObject;
    public Transform plane;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        ProjectTransformOnPlane(simpleObject, plane.transform.position, plane.up);
    }


    private void ProjectTransformOnPlane(Transform objectToProject, Vector3 planeOrigin, Vector3 planeNormal)
    {
        Plane projectionPlane = new Plane(planeNormal, planeOrigin);
        float distanceToIntersection;
        Ray intersectionRay = new Ray(transform.position, transform.forward);
        if (projectionPlane.Raycast(intersectionRay, out distanceToIntersection))
        {
            objectToProject.position = intersectionRay.GetPoint(distanceToIntersection);
        }
    }
}
