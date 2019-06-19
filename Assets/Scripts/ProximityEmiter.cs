using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityEmiter : MonoBehaviour
{

    public float maxValue = 5.0f;
    public float maxDistance = 5.0f;

    public float noiseScale;

    public AnimationCurve curve;

    //public float output;
    public Transform testObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //output = getValue(testObj);
    }


    public float getValue(Transform t)
    {
        float distance = Vector3.Distance(t.position, transform.position)/maxDistance;

        float noise = 2 * (Random.value - .5f) * noiseScale;

        return  Mathf.Clamp( noiseScale + maxValue * (curve.Evaluate(distance)/maxDistance), 0, 1);
    }

    public float getValue()
    {
        return getValue(testObj);
    }

}
