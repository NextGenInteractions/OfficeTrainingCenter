using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentroidPlacement : MonoBehaviour {

    public GameObject headset;
    public GameObject controller1;
    public GameObject controller2;

    private Transform groundPlane;
    


    private Vector3 _point1;
    private Vector3 _point2;
    private Vector3 _point3;
    private Vector3 _point4;

    private Vector3 _position_adjustment = new Vector3(0, -0.1f, 0);
    private float _rotation_adjustment = 90.0f;
    
    private int _frameCounter = 0;

    // Use this for initialization
    void Start()
    {
       
    }

    void PlaceCentroid()
    {
        //transform.position = ((_point1 + _point2 + _point3) / 3) + _adjustment_offset;

        transform.position = headset.transform.position + _position_adjustment;
        transform.rotation = Quaternion.Euler(0,headset.transform.rotation.eulerAngles.y + _rotation_adjustment,0);
        //Debug.Log(transform.position);
        float distance = Vector3.Distance(controller1.transform.position, controller2.transform.position);
        Debug.Log(distance);
    }

    // Update is called once per frame
    void Update ()
    {

        if(_frameCounter < 10)
        {
            _frameCounter++;
        }

        if(_frameCounter == 9)

        {
            //Debug.Log(_frameCounter);
            /*
            // Project lighthouses to ground
            _point1 = transform.TransformPoint(calibrationObject.transform.position);
            _point1 = new Vector3(_point1.x, 0, _point1.z);
            //Instantiate(prefab, _point1, Quaternion.identity);

            _point2 = transform.TransformPoint(controller2.transform.position);
            _point2 = new Vector3(_point2.x, 0, _point2.z);
            //Instantiate(prefab, _point2, Quaternion.identity);

            _point3 = transform.TransformPoint(headset.transform.position);
            _point3 = new Vector3(_point3.x, 0, _point3.z);
            //Instantiate(prefab, _point3, Quaternion.identity);

            //_point4 = new Vector3(lightHouse4.transform.position.x, 0, lightHouse4.transform.position.z);
            //Instantiate(prefab, _point4, Quaternion.identity);*/

            // Place the orgin
            PlaceCentroid();
        }



    }
}
