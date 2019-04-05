using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackRotation : MonoBehaviour {

    public static float totalRotation = 0;
    private Vector3 lastPoint;
    public Transform physicalObj;
    private int frameCount;
    private float offset;
    private float centeredRotation;
    public bool resetRotation = false;
    public int nrOfRotations
    {
        get
        {
            return ((int)totalRotation) / 360;
        }
    }

    // Use this for initialization
    void Start () {
        lastPoint = transform.TransformDirection(Vector3.forward);
        lastPoint.y = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (frameCount < 7)
        {
            WaitForMe();
        }

        if (frameCount == 6)
        {
            offset = physicalObj.transform.eulerAngles.y ;

            //Debug.Log(offset);
        }

        if (frameCount > 6)
        {
            if (resetRotation)
            {
                totalRotation = 0;
                resetRotation = false;
            }


            centeredRotation = physicalObj.transform.eulerAngles.y + (360 - offset);
            Vector3 facing = transform.TransformDirection(Vector3.forward);
            facing.y = 0;

            float angle = Vector3.Angle(lastPoint, facing);
            if (Vector3.Cross(lastPoint, facing).y < 0)
                angle *= -1;

            totalRotation += angle;
            lastPoint = facing;
            //Debug.Log("Total Rotation :" + totalRotation);
            transform.rotation = Quaternion.Euler(0, centeredRotation, 0);
        }
    }

    

    void WaitForMe()
    {
        frameCount++;
    }

}
