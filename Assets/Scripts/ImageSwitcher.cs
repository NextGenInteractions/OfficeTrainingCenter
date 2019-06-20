//Image switcher randomly enables one of a set of objects.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSwitcher : MonoBehaviour
{

    public GameObject[] objects;

    public float interval = .5f;

    float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > interval)
        {
            timer = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(false);
            }

            int randomindex =  Random.Range(0, objects.Length);


            objects[randomindex].SetActive(true);

        }
        
    }
}
