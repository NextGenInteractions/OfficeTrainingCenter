using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityEmiter : MonoBehaviour
{

    public MutiRaeManager.Gas type;

    public float maxValue = 5.0f;
    public float maxDistance = 5.0f;

    public float noiseScale;

    public AnimationCurve curve;

    //public float output;
    //public Transform testObj;

 
    public float getValue(Transform t)
    {
        float distance = Vector3.Distance(t.position, transform.position);

        float noise = 2 * (Random.value - .5f) * noiseScale;

        return  Mathf.Clamp( noise + maxValue * (curve.Evaluate(distance/maxDistance)), 0, maxValue);
    }
/*
    public float getValue()
    {
        return getValue(testObj);
    }
*/
}
