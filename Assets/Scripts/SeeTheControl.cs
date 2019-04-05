using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeTheControl : MonoBehaviour {

    public Transform source;
    private int frameNumber = 0;
    private Vector3 oldPosition;
    public float distanceThreshold = 0.1f;
    public float horizontalSpeed = 5.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        if (frameNumber < 11)
        {
            frameNumber++;
        }
        if (frameNumber == 10)
        {
            oldPosition = source.position;
        }
        if(frameNumber > 10)
        {
            ControlBot();
            
        }

        
    }

    private void ControlBot()
    {
        if((oldPosition.x + distanceThreshold) < source.position.x)
        {
            //Debug.Log("Move Right");
            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0.1f,0,0), Time.deltaTime * horizontalSpeed);
        }

        if ((oldPosition.x - distanceThreshold) > source.position.x)
        {
            //Debug.Log("Move Left");
            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(-0.1f, 0, 0), Time.deltaTime * horizontalSpeed);
        }

        if ((oldPosition.z + distanceThreshold) < source.position.z)
        {
            //Debug.Log("Move Up");
            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0, 0, 0.1f), Time.deltaTime * horizontalSpeed);
        }

        if ((oldPosition.z - distanceThreshold) > source.position.z)
        {
            //Debug.Log("Move Down");
            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + new Vector3(0, 0, -0.1f), Time.deltaTime * horizontalSpeed);
        }


    }

}
