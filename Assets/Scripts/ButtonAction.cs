using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;


public class ButtonAction : MonoBehaviour {

    [SerializeField]
    private int redButton;
    private int greenButton;
    private int sensorValue;
    private int flexValue;
    private int lightValue;
    private int potenValue;
    private int magneticValue;
    private int encoderPinA;
    private int encoderPinB;

    private bool encoderA;
    private bool encoderB;
    private bool encoderAPrev;
    private float encoderAngle;
    public GameObject airBladder;

    private int aLastState;
    private int encoderCounter = 0;
    
    
    private int c = 0;
    private int aState;

    private Light spotLight;
    public Light worldLight;
    public Material redMaterial;
    public GameObject cube;
    public Material defaultMaterial;
    private UduinoManager boardManager;

    const float VCC = 4.98f; // Measured voltage of Ardunio 5V line
    const float R_DIV = 47500.0f; // Measured resistance of 47k resistor

    const float spotLightIntensity = 15.0f;
    const float worldLightIntensity = 1.0f;

    // Upload the code, then try to adjust these values to more
    // accurately calculate bend degree.
    const float STRAIGHT_RESISTANCE = 37300.0f; // resistance when straight
    const float BEND_RESISTANCE = 90000.0f; // resistance at 90 deg

    private float nextActionTime = 0.0f;
    public float period = 0.005f;

    private float oldBladderAngle = 0f;
    


    private bool materialSwapped = false;
    public AudioClip switchOn;
    public AudioClip pumpAir;
    private bool lightSwitch = true;
    

    // Use this for initialization
    void Start () {

        boardManager = UduinoManager.Instance;

        boardManager.pinMode(2, PinMode.Input_pullup);
        boardManager.pinMode(3, PinMode.Input_pullup);
        boardManager.pinMode(AnalogPin.A3, PinMode.Input);
        /*boardManager.pinMode(9, PinMode.Input_pullup);
        boardManager.pinMode(4, PinMode.Input_pullup);
        boardManager.pinMode(AnalogPin.A0, PinMode.Input);
        boardManager.pinMode(AnalogPin.A1, PinMode.Input);
        boardManager.pinMode(AnalogPin.A2, PinMode.Input);
        boardManager.pinMode(10, PinMode.Input);
        boardManager.pinMode(11, PinMode.Input);*/

        spotLight = gameObject.GetComponent<Light>();
        boardManager.OnDataReceived += DataReceived;

        // Runs after 1 second and every 5ms
        //InvokeRepeating("RotatoryEncoder", 1.0f, 0.01f);

    }

    // Update is called once per frame
    void Update()
    {
        greenButton = boardManager.digitalRead(2);
        redButton = boardManager.digitalRead(3);
        
        //magneticValue = boardManager.digitalRead(4);



        //sensorValue = boardManager.analogRead(AnalogPin.A0);
        flexValue = boardManager.analogRead(AnalogPin.A3);
        //lightValue = boardManager.analogRead(AnalogPin.A2);


        //Toggle Light
        ControlLight(redButton,greenButton);

        //Display Pressure
        //AlterLightIntensity(sensorValue);

        //Rotate Light
        SquishObject(flexValue);

        //Dim the world light
        //DimWorld(lightValue);

        //Swap material
        //SwapMaterial(magneticValue);

        //RotatoryEncoder();


        /* if (Time.time > nextActionTime)
         {
             nextActionTime += period;
             //Debug.Log(nextActionTime);
             if (port.IsOpen)
             {
                 try
                 {
                     transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, float.Parse(port.ReadLine()), transform.rotation.eulerAngles.z);
                 }
                 catch (System.Exception)
                 {
                     throw;
                 }
             }
         }*/




    }

    private void ControlLight(int red, int green)
    {
        if (red == 1)
        {
            lightSwitch = false;
        }
        else if (green == 1)
        {
            lightSwitch = true;
        }
        else if (red == 1 && green == 1)
        {
            lightSwitch = true;
        }


        if(lightSwitch)
        {
            spotLight.enabled = true;
        }
        else
        {
            spotLight.enabled = false;
        }


    }

    private void AlterLightIntensity(int sensorValue)
    {
        if (sensorValue != 0) // If the analog reading is non-zero
        {
            spotLight.intensity = Mathf.Lerp(spotLightIntensity, spotLightIntensity * (sensorValue),Time.deltaTime * 0.3f);
        }

    }



    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    void DataReceived(string value, UduinoDevice board)
    {
        if (value.StartsWith("e"))
        {
            encoderAngle = float.Parse(value.Remove(0, value.IndexOf(' ') + 1));
            Debug.Log(encoderAngle);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, encoderAngle, transform.rotation.eulerAngles.z);
        }
    }


   

    private void SquishObject(int flexValue)
    {
        
        // Read the ADC, and calculate voltage and resistance from it
        
        float flexV = flexValue * VCC / 1023.0f;
        float flexR = R_DIV * (VCC / flexV - 1.0f);
        //Debug.Log("Resistance: " + flexR + " ohms");

        // Use the calculated resistance to estimate the sensor's
        // bend angle:
        float flexAngle = Remap(flexR, STRAIGHT_RESISTANCE, BEND_RESISTANCE,0f, 90.0f);
        Debug.Log(flexAngle);
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, flexAngle, transform.rotation.eulerAngles.z);

        if (flexAngle != Mathf.Infinity && oldBladderAngle == 0)
        {
            oldBladderAngle = flexAngle; // First time value update
        }
        else if(flexAngle != Mathf.Infinity)
        {
           
            float differenceAngle = oldBladderAngle - flexAngle;
            float squishyness = Remap(Mathf.Abs(differenceAngle),0, 15, 1, 0);
            airBladder.transform.localScale = new Vector3(airBladder.transform.localScale.x, squishyness, airBladder.transform.localScale.z);

            

            if (squishyness < 0.5f)
            {
                if (!gameObject.GetComponent<AudioSource>().isPlaying)
                {
                    
                    gameObject.GetComponent<AudioSource>().Play();
                }

            }
        }


        


        //Debug.Log("Bend: " + flexAngle + " degrees");    

    }


    private void DimWorld(int lightValue)
    {
        worldLight.intensity = Mathf.Lerp(worldLightIntensity, (lightValue), Time.deltaTime * 0.3f) - 4.0f;
    }


    private void SwapMaterial(int magneticValue)
    {
        
        if(magneticValue==0)
        {
            if (!materialSwapped)
            {
                cube.GetComponent<MeshRenderer>().material = redMaterial;
                materialSwapped = true;
            }
            gameObject.GetComponent<AudioSource>().Stop();
        }
        else
        {
            if (materialSwapped)
            {
                cube.GetComponent<MeshRenderer>().material = defaultMaterial;
                materialSwapped = false;
            }
            gameObject.GetComponent<AudioSource>().Play();
        }

        
    }
}
