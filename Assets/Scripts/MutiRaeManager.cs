using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MutiRaeManager : MonoBehaviour
{
    public enum Gas
    {
        O2=0, LEL=1, CO=2, H2S=3, VOC=4, GAMMA=5 
    }

    [System.Serializable]
    public class GasIndicator
    {
        enum color { GREY, GREEN, YELLOW, RED}

        float value;
        public Text valueText;//, nameText;
        public float greyTresh, greenThresh, yellowThresh, redThresh;
        public GameObject grey, green, yellow, red;

        public void setValue(float val)
        {
            value = val;
            if(valueText != null)
            {
                valueText.text = string.Format("{0:0.0}", value);
            }

            if (value < greenThresh)
            {
                setColor(color.GREY);
            } else if (value < yellowThresh)
            {
                setColor(color.GREEN);
            } else if (value < redThresh)
            {
                setColor(color.YELLOW);
            } else 
            {
                setColor(color.RED);
            } 


        }

        void setColor(color c)
        {

            grey.SetActive(false);
            green.SetActive(false);
            yellow.SetActive(false);
            red.SetActive(false);

            switch (c)
            {
                case color.GREY:
                    grey.SetActive(true);
                    break;
                case color.GREEN:
                    green.SetActive(true);
                    break;
                case color.YELLOW:
                    yellow.SetActive(true);
                    break;
                case color.RED:
                    red.SetActive(true);
                    break;
                default:
                    grey.SetActive(true);
                    break;
            }
        }

    }

    public float updateInterval= .1f;
    float timer = 0;

    public GasIndicator O2, LEL, CO, H2S, VOC, GAMMA;

    GasIndicator[] gasses;
    ProximityEmiter[] emmiters;

    // Start is called before the first frame update
    void Start()
    {
        gasses = new GasIndicator[] { O2, LEL, CO, H2S, VOC, GAMMA };
        emmiters = GameObject.FindObjectsOfType<ProximityEmiter>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > updateInterval)
        {
            timer = 0;
            for (int i = 0; i < gasses.Length; i++)
            {
                setValue((Gas)i, getValue((Gas)i));
            }
        }
    }

    void setValue(Gas gas, float value)
    {
        gasses[(int)gas].setValue(value);
    }

    float getValue(Gas type)
    {
        float total = 0;

        for (int i = 0; i < emmiters.Length; i++)
        {
            if (emmiters[i].type == type)
            {
                total += emmiters[i].getValue(transform);
            }
        }

        return total;
    }
}
