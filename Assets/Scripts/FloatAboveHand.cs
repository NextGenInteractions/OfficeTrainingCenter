using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAboveHand : MonoBehaviour {

    // Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;

    public Transform leftHand;
    public Transform rightHand;

    // Movement speed in units/sec.
    public float speed = 0.01F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    // User Inputs
    public float degreesPerSecond = 30.0f;
    public float amplitude = 0.1f;
    public float frequency = 0.5f;


    // Use this for initialization
    void Start () {

        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
    }
	
	// Update is called once per frame
	void Update () {

        if (Vector3.Distance(transform.position, leftHand.position) < Vector3.Distance(transform.position, rightHand.position) ? endMarker = leftHand : endMarker = rightHand) ;


        // Distance moved = time * speed.
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed = current distance divided by total distance.
        float fracJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        Vector3 movement = Vector3.Lerp(startMarker.position, endMarker.position + new Vector3 (0,0.2f,0), fracJourney);

        // Spin object around Y-Axis
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        
        float floatMovement =+ Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        Vector3 posOffset = new Vector3(0, floatMovement, 0);

        transform.position = posOffset + movement;


    }
}
