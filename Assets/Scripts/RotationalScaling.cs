using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationalScaling : MonoBehaviour {

    [Range(0f, 3.0f)]
    public float amplificationFactor = 1.0f;

    public Transform physicalObj;
    private int frameCount;
    private float offset;
    private float currentYRotation;
    public bool resetRotation = false;
    public static float scaleFactor;


    // Use this for initialization
    void Start () {

    }



    // Update is called once per frame
    void Update () {

        if (frameCount < 7)
        {
            WaitForMe();
        }

        if (frameCount == 6)
        {
            offset = physicalObj.transform.eulerAngles.y;

        }

        if (frameCount > 6)
        {


            currentYRotation = TrackRotation.totalRotation * amplificationFactor;
            transform.rotation = Quaternion.Euler(0, currentYRotation, 0);


        }

        if (resetRotation)
        {
            offset = physicalObj.transform.eulerAngles.y;
            resetRotation = false;
        }
        
		
	}



    void WaitForMe()
    {
        frameCount++;
    }

}
