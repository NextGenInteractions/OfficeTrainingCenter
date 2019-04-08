using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;

public class GameManager : MonoBehaviour {

    private UduinoManager boardManager;
    private bool greenButtonInput;
    private bool redButtonInput;
    private int flexValue;

    public bool calibrateSlot;

    public Transform slot1;
    public Transform slot2;
    //public Transform slot3;
    public Transform fuelCell1;
    public Transform fuelCell2;
    //public Transform fuelCell3;
    public Transform airPump;

    public Material red;
    public Material green;
    public Material redEmmisive;
    public Material greenEmmisive;
    public Material purpleEmissive;
    public Material yellowEmissive;
    public Material redLightEmissive;
    public Material whiteEmissive;
    public Material white;
    public Material purple;
    public Material redLight;
    public Material yellow;
    public Transform dial;
    public Transform plungerTracker;
    public GameObject plungerHandle;
    private Vector3 plungerHandleStart;
    private Vector3 plungerTrackerStartPosition;
    private float encoderAngle;
    private int frameNumber = 0;

    private float oldBladderAngle = 0;
    const float VCC = 4.98f; // Measured voltage of Ardunio 5V line
    const float R_DIV = 47500.0f; // Measured resistance of 47k resistor
    const float STRAIGHT_RESISTANCE = 37300.0f; // resistance when straight
    const float BEND_RESISTANCE = 90000.0f; // resistance at 90 deg

    public Text textInstructions;
    private bool calibrationCompleted = false;
    public bool flashLightCheck = false;
    private bool startBlink = false;

    private bool synthSlot1 = false;
    private bool synthSlot2 = false;
    private bool synthSlot3 = false;
    private bool synthSlot1Com = false;
    private bool synthSlot2Com = false;
    private bool synthSlot3Com = false;

    public static int currentGameState = -1;

    const int CALIBRATE_ENVIRONMENT = -2;
    const int PRESS_GREEN_BUTTON = -1;
    const int WELCOME = 0;
    const int TUTORIAL = 1;
    const int GAME_START = 2;
    const int EMPTY_FUEL_CELLS = 3;
    const int PLACE_FUEL_CELLS = 4;
    const int SYNTHESIZER = 5;
    const int PUMP_AIR_PRESSURE = 6;
    const int RELEASE_COOLANT = 7;
    const int GAME_COMPLETE = 8;

    public GameObject redButton;
    public GameObject greenButton;
    public GameObject orangeButton;

    // New stages for the January 31st Video
    const int THROTTLE_UP = 1;
    const int PRESS_PRIMER = 2;
    const int DISCHARGE_WATER = 3;
    const int THROTTLE_MORE = 4;
    const int PLUGIN_INDUCTOR_TUBE = 5;
    const int CHOOSE_FOAM_TYPE = 6;
    const int RELEASE_FOAM = 7;
    const int END_OF_EXPERIENCE = 8;



    private bool buttonPressed = false;
    private bool stateDone = false;
    private bool slot1Success = false, slot2Success = false, slot3Success = false;
    private float totalEncoderAngle;
    private float totalAirPressure = 0;
    private bool valveRetracted = true;
    private float differenceZ = 0.0f;

    public AudioSource engineSound;
    public AveragedMotion panelMovement;
    public GameObject water;
    public GameObject foam;
    public GameObject fire;
    public GameObject fireLight;

    //Version 2
    //public TextMesh engineRPM;
    private float currentEngineRPM = 600.0f;
    private bool savedEncoderVal = false;
    private float initialEncoderVal = 0.0f;
    private float enginePitch = 0.2f;
    public Vector3 slotAdjustment;
    private bool engineRPMCheck = false;
    private float oldBladderScale = 0.0f;
    [Range(0, 1)] public float needleSpeed = 0.1f;
    private float intakeRotationScale = 0.0f;
    private float dischargeRotationScale = 0.0f;
    private int currentFoamMode = 0; // 0 - class A | 1 - class B
    private enum foamType {CLASS_A, CLASS_B};
    private bool actionDone = false;

    public Transform rpm;
    public Transform masterIntake;
    public Transform masterDischarge;
    public Transform discharge;
    public TextMesh foamText;


    public GameObject greenButtonArrow;
    public GameObject redButtonArrow;
    public GameObject knobArrow;
    public GameObject primerArrow;
    public GameObject dischargeArrow;
    public GameObject inductorArrow;
    public GameObject pipeArrow;

    public AudioClip successSound;
    public float dingVolume = 0.1f;

    public AudioSource voiceOverPlayer;
    public AudioClip pressTheGreenButtonToBegin;
    public AudioClip WelcomeToOurFireTruckPumpPanelSimulator;
    public AudioClip CalibrationCompleted;
    public AudioClip ThrottleUpTheTruckTo1000RPMs;
    public AudioClip PressThePrimerToEstablishADraft;
    public AudioClip PullTheDischargeToFullyChargeTheLine;
    public AudioClip ThrottleUpMoreToMediate;
    public AudioClip InsertTheFoamInductorIntoTheClassATank;
    public AudioClip ChooseClassBFoam;
    public AudioClip PressTheSelectButtonToBeginUsingTheFoam;
    public AudioClip GreatWorkCaptain;


    // Use this for initialization
    void Start() {

        boardManager = UduinoManager.Instance;
        boardManager.pinMode(2, PinMode.Input_pullup); //Green Button
        boardManager.pinMode(3, PinMode.Input_pullup);  //Red Button
        boardManager.pinMode(AnalogPin.A3, PinMode.Input); // Flex Sensor
        boardManager.OnDataReceived += GetRotationData; // Rotatary Encoder Information Read (Use Pin number 5 & 6 on the arduino. Refer ArduinoAdv.ino script.)

        oldBladderScale = airPump.localScale.y;

        greenButtonInput = false;

        Time.timeScale = 1.0f;

        greenButtonArrow.SetActive(true);

        //Testing
        currentGameState = CALIBRATE_ENVIRONMENT;

        StartCoroutine(lockMovement());
    }

    // Update is called once per frame
    void Update() {

        greenButtonInput = false;
        // Initial inputs are not giving consistent values, so start reading values in early frames before they are used
        int input2 = boardManager.digitalRead(2);
        int input3 = boardManager.digitalRead(3);


        if (frameNumber < 31)
        {
            frameNumber++;
        }
        if (frameNumber == 30)
        {
            //plungerTrackerStartPosition = plungerTracker.position;
            //plungerHandleStart = plungerHandle.transform.position;

            // Set plunger in local coordinates
            plungerTrackerStartPosition = plungerTracker.position;
            plungerHandleStart = plungerHandle.transform.localPosition;

            voiceOverPlayer.PlayOneShot(pressTheGreenButtonToBegin);

        }
        if (frameNumber > 30)
        {
            //Get all pin inputs

            //int input2 = boardManager.digitalRead(3);
            // Buttons are reversed
            if (input2 == 1)
            {
                //Debug.Log("Green button pushed");
                greenButtonInput = false;
            }
            else
            {
                //Debug.Log("Green button pushed");
                greenButtonInput = true;
            }

            if (input3 == 1)
            {
                redButtonInput = false;
            }
            else
            {
                redButtonInput = true;
            }

            flexValue = boardManager.analogRead(AnalogPin.A3);

            if(redButtonInput)
            {
                actionDone = false;
            }

            GlowButtons();

            //Updating the engine rpm with data received
            UpdateEngineRPM(encoderAngle);

            if (currentGameState == CHOOSE_FOAM_TYPE)
            { 
                //Swap foam mode
                SwapFoamMode();
            }

            //if (currentGameState == PRESS_PRIMER || currentGameState == DISCHARGE_WATER)
            {
                //Squishy Object
                SquishObject(flexValue);
            }


            //if (currentGameState == DISCHARGE_WATER || currentGameState == CALIBRATE_ENVIRONMENT)
            {
                //Move Plunger
                MovePlunger();
            }


            if (currentGameState == CALIBRATE_ENVIRONMENT)
            {
                if (!stateDone)
                {
                    textInstructions.text = "Press green button to begin.";
                    //redButton.GetComponent<MeshRenderer>().enabled = true; //Green light on
                    stateDone = true;
                }

                //Calibrate the slots.
                calibrateSlots();

                if (calibrationCompleted)
                {
                    if(!voiceOverPlayer.isPlaying)
                    {
                        voiceOverPlayer.PlayOneShot(CalibrationCompleted);
                    }

                    //textInstructions.text = "Calibration complete.";
                    Debug.Log("Calibration completed.");
                    currentGameState++;
                    //redButton.GetComponent<MeshRenderer>().enabled = false; //Green light on
                    stateDone = false;
                    voiceOverPlayer.clip = pressTheGreenButtonToBegin;
                    voiceOverPlayer.PlayDelayed(2.0f);
                }
            }

            if(currentGameState == CALIBRATE_ENVIRONMENT || currentGameState == PRESS_GREEN_BUTTON)
            {

                if (greenButtonInput == true)
                {
                    voiceOverPlayer.PlayOneShot(successSound, dingVolume);

                    currentGameState = WELCOME;

                    // Needs to be updated here
                    stateDone = false;
                }

            }

            if (currentGameState == WELCOME)
            {
                textInstructions.text = "Welcome.";

                frameNumber++;

                if (!stateDone)
                {
                    voiceOverPlayer.clip = WelcomeToOurFireTruckPumpPanelSimulator;
                    voiceOverPlayer.PlayDelayed(1.0f);
                    greenButtonArrow.SetActive(false);
                    stateDone = true;
                }

                if (frameNumber == 400)
                {
                    currentGameState++;
                    stateDone = false;
                }
            }




            if (currentGameState == THROTTLE_UP)
            {
                if (!stateDone)
                {
                    knobArrow.SetActive(true);
                    voiceOverPlayer.clip = ThrottleUpTheTruckTo1000RPMs;
                    voiceOverPlayer.PlayDelayed(1.0f);

                    textInstructions.text = "Throttle up the truck to 1000 RPM.";

                    stateDone = true;
                }

                //Debug.Log("currentEngineRPM = " + currentEngineRPM);


                if (currentEngineRPM >= 1000)
                {
                    knobArrow.SetActive(false);
                    engineRPMCheck = true;
                    Debug.Log("RPM Reached");
                }


                if (engineRPMCheck)
                {
                    voiceOverPlayer.PlayOneShot(successSound, dingVolume);
                    currentGameState++;
                    stateDone = false;
                }
            }


            if (currentGameState == PRESS_PRIMER)
            {
                if (!stateDone)
                {
                    knobArrow.SetActive(false);
                    primerArrow.SetActive(true);

                    voiceOverPlayer.clip = PressThePrimerToEstablishADraft;
                    voiceOverPlayer.PlayDelayed(1.0f);

                    textInstructions.text = "Press the primer to establish a draft.";
                    //greenButton.GetComponent<MeshRenderer>().enabled = true; //Green light on
                    //StartCoroutine(Blink(1.0f));

                    stateDone = true;
                }

                Debug.Log("masterIntake = " + GetGaugeRotation(masterIntake));

                if (GetGaugeRotation(masterIntake) <= -0.075f) //reached the end
                {
                    primerArrow.SetActive(false);
                    voiceOverPlayer.PlayOneShot(successSound, dingVolume);
                    SetGaugeRotation(0.07f, masterDischarge); //setting the value around 45-50
                    currentGameState++;
                    stateDone = false;
                }

            }


            if (currentGameState == DISCHARGE_WATER)
            {
                if (!stateDone)
                {
                    dischargeArrow.SetActive(true);

                    voiceOverPlayer.clip = PullTheDischargeToFullyChargeTheLine;
                    voiceOverPlayer.PlayDelayed(1.0f);

                    textInstructions.text = "Pull the discharge handle to fully charge the line.";
                    stateDone = true;
                }

                //Debug.Log("differenceZ = " + differenceZ);

                if (Mathf.Abs(differenceZ) > 0.055f) //Positional difference in Z axis of the plunger
                {
                    voiceOverPlayer.PlayOneShot(successSound, dingVolume);
                    dischargeArrow.SetActive(false);

                    ParticleSystem waterParticles = water.GetComponent<ParticleSystem>();
                    var emission = waterParticles.emission;
                    emission.rateOverTime = 0.7f;
                    waterParticles.Play();


                    Debug.Log("Water Flowing");
                    // Needs to be updated here
                    SetGaugeRotation(GetGaugeRotation(masterDischarge), discharge); //Match the master discharge gauge
                    currentGameState++;
                    stateDone = false;

                    knobArrow.SetActive(true);
                }
            }


            if (currentGameState == THROTTLE_MORE)
            {
                if (!stateDone)
                {
                    knobArrow.SetActive(true);

                    voiceOverPlayer.clip = ThrottleUpMoreToMediate;
                    voiceOverPlayer.PlayDelayed(1.0f);

                    textInstructions.text = "Throttle up more to mediate the output pump pressure.";
                    stateDone = true;
                }

                if(currentEngineRPM >= 1500)
                {
                    knobArrow.SetActive(false);
                    voiceOverPlayer.PlayOneShot(successSound, dingVolume);
                    SetGaugeRotation(0.25f, masterDischarge);
                    SetGaugeRotation(GetGaugeRotation(masterDischarge), discharge); //Match the master intake gauge
                    currentGameState++;

                    ParticleSystem waterParticles = water.GetComponent<ParticleSystem>();
                    var emission = waterParticles.emission;
                    emission.rateOverTime = 36.5f;
                    waterParticles.Play();

                    Debug.Log("More water flows out");
                    // Needs to be updated here
                    stateDone = false;
                }

            }


            if (currentGameState == PLUGIN_INDUCTOR_TUBE)
            {
                if (!stateDone)
                {
                    inductorArrow.SetActive(true);
                    pipeArrow.SetActive(true);

                    voiceOverPlayer.clip = InsertTheFoamInductorIntoTheClassATank;
                    voiceOverPlayer.PlayDelayed(1.0f);

                    textInstructions.text = "Insert the foam inductor into the class A tank.";
                    stateDone = true;
                }

                if (slot1.GetComponent<CheckFuelCell>().succesfulPlacement)
                {
                    inductorArrow.SetActive(false);
                    pipeArrow.SetActive(false);
                    voiceOverPlayer.PlayOneShot(successSound, dingVolume);
                    slot1Success = true;
                }

//                if (slot2.GetComponent<CheckFuelCell>().succesfulPlacement)
//                {
//                    voiceOverPlayer.PlayOneShot(successSound, dingVolume);
//                    slot2Success = true;
//                }


                if (slot1Success && !slot2Success)
                {
                    Debug.Log("Slot1 inserted");
                    currentGameState++;
                    stateDone = false;
                }

            }

            if (currentGameState == CHOOSE_FOAM_TYPE)
            {
                if (!stateDone)
                {
                    voiceOverPlayer.clip = ChooseClassBFoam;
                    voiceOverPlayer.PlayDelayed(1.0f);

                    redButtonArrow.SetActive(true);
                    textInstructions.text = "Choose Class B foam by pressing the red mode button.";
                    stateDone = true;
                }

                if (currentFoamMode == (int)foamType.CLASS_B)
                {
                    voiceOverPlayer.PlayOneShot(successSound, dingVolume);
                    redButtonArrow.SetActive(false);

                    currentGameState++;
                    stateDone = false;
                }

            }

            if (currentGameState == RELEASE_FOAM)
            {
                if (!stateDone)
                {
                    voiceOverPlayer.clip = PressTheSelectButtonToBeginUsingTheFoam;
                    voiceOverPlayer.PlayDelayed(1.0f);
                    greenButtonArrow.SetActive(true);

                    textInstructions.text = "Press the green select button to begin using the foam.";
                    stateDone = true;
                }

                if (greenButtonInput)
                {
                    voiceOverPlayer.PlayOneShot(successSound, dingVolume);

                    currentGameState++;
                    ParticleSystem foamParticles = foam.GetComponent<ParticleSystem>();
                    foamParticles.Play();
                    // Needs to be updated here
                    stateDone = false;
                    greenButtonArrow.SetActive(false);
                }
            }


            if (currentGameState == GAME_COMPLETE)
            {
                if (!stateDone)
                {
                    voiceOverPlayer.clip = GreatWorkCaptain;
                    voiceOverPlayer.PlayDelayed(1.0f);

                    textInstructions.text = "Great Work, Captain!";
                    Debug.Log("Fire dies down with smoke.");

                    fireLight.GetComponent<Light>().enabled = false;

                    ParticleSystem fireParticles = fire.GetComponent<ParticleSystem>();
                    var fireEmission = fireParticles.emission;
                    fireEmission.rateOverTime = 0.1f;
                    fireParticles.Stop();

                    // Needs to be updated here
                    stateDone = true;
                }
            }
        }
    }

    private void SetGaugeRotation(float value, Transform gauge)
    {
        if(gauge)
        {
            if (gauge.GetComponent<RotatePointer>())
            {
                gauge.GetComponent<RotatePointer>().rotationScale = value;
            }
        }
    }
    
    private float GetGaugeRotation(Transform gauge)
    {
        return gauge.GetComponent<RotatePointer>().rotationScale;
    }
    
    IEnumerator MoveToPosition(GameObject fuelCell, Vector3 newPosition, float time)
    {
        float elapsedTime = 0;
        Vector3 startPosition = fuelCell.transform.position;
        while (elapsedTime < time)
        {
            fuelCell.transform.localPosition = Vector3.Lerp(startPosition, newPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame(); ;
        }

        fuelCell.GetComponent<Rigidbody>().isKinematic = false;
        fuelCell.GetComponent<Rigidbody>().useGravity = true;

    }

    IEnumerator Blink(float waitTime)
    {
        while (true)
        {
            greenButton.GetComponent<MeshRenderer>().material = greenEmmisive;
            yield return new WaitForSeconds(waitTime);
            greenButton.GetComponent<MeshRenderer>().material = green;
            yield return new WaitForSeconds(waitTime);

        }

    }

    private void RotateAnObjectOnPivot(float value, Transform needle)
    {
        Quaternion fromRotation = needle.transform.localRotation;
        Quaternion toRotation = Quaternion.Euler(value, value, 0);
        needle.transform.localRotation = Quaternion.Euler(value, 0, 0); // Quaternion.Lerp(fromRotation, toRotation, Time.deltaTime * needleSpeed);
    }

    private void GlowButtons()
    {
        if (redButtonInput)
        {
            //Debug.Log("RedButton on");
            redButton.GetComponent<Light>().enabled = true;
        }
        else
        {
            //Debug.Log("RedButton off");
            redButton.GetComponent<Light>().enabled = false;
        }


        if (greenButtonInput)
        {
            //Debug.Log("GreenButton on");
            greenButton.GetComponent<Light>().enabled = true;
        }
        else
        {
            //Debug.Log("GreenButton off");
            greenButton.GetComponent<Light>().enabled = false;
        }



    }
    
    private void calibrateSlots()
    {
        if (redButtonInput && calibrateSlot)
        {
            Debug.Log("Calibrating slot 1");
            slot1.transform.position = fuelCell1.transform.position + slotAdjustment;
        }
        if (redButtonInput && calibrateSlot)
        {
            Debug.Log("Calibrating slot 2");
            slot2.transform.position = fuelCell2.transform.position + slotAdjustment;
            //slot2.transform.position = new Vector3(fuelCell2.transform.position.x + slotAdjustment, slot2.transform.position.y, fuelCell2.transform.position.z);
        }
        if (redButtonInput && calibrateSlot)
        {
            Debug.Log("Calibration completed");
            //slot3.transform.position = new Vector3(fuelCell3.transform.position.x, slot3.transform.position.y, fuelCell3.transform.position.z);
            calibrationCompleted = true;
        }

    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    private void MovePlunger()
    {

        //        differenceZ = plungerTracker.position.z - plungerTrackerStartPosition.z;

        // Move the object forward along its z axis 1 unit/second.
        //        plungerHandle.transform.position = (plungerHandle.transform.up * /*Mathf.Abs*/(differenceZ)) + plungerHandleStart;


        // plunger translation should be in local coordinates.

        // project tracker movement along X axis of plunger
        Vector3 plungerTrackerDifference = plungerTracker.position - plungerTrackerStartPosition;
        Vector3 plungerHandleForwardDirection = plungerHandle.transform.right;
        differenceZ = Vector3.Dot(plungerTrackerDifference, plungerHandleForwardDirection);

        //differenceZ = plungerTracker.localPosition.x - plungerTrackerStartPosition.z;
        float PlungerDepth = differenceZ + plungerHandleStart.x;
        //plungerHandle.transform.localPosition.x = differenceZ + plungerHandleStart.x;
        plungerHandle.transform.localPosition = new Vector3(PlungerDepth, 0.0f, 0.0f);
    }

    private void SwapFoamMode()
    {
        if(redButtonInput && !actionDone)
        {
            if(currentFoamMode==1)
            {
                currentFoamMode = 0;
            }
            else
            {
                currentFoamMode = 1;
            }

            UpdateFoamModeText();
            actionDone = true;
        }
    }
    
    private void UpdateFoamModeText()
    {
        if(currentFoamMode == (int)foamType.CLASS_A)
        {
            foamText.text = "CLASS A";
        }
        else if(currentFoamMode == (int)foamType.CLASS_B)
        {
            foamText.text = "CLASS B";
        }
    }

    private void SquishObject(int flexValue)
    {
        //Debug.Log("flexValue = " + flexValue);
        // Read the ADC, and calculate voltage and resistance from it

        float flexV = flexValue * VCC / 1023.0f;
        float flexR = R_DIV * (VCC / flexV - 1.0f);

        // Use the calculated resistance to estimate the sensor's
        // bend angle:
        float flexAngle = Remap(flexR, STRAIGHT_RESISTANCE, BEND_RESISTANCE, 0f, 90.0f);

        if (flexAngle != Mathf.Infinity && oldBladderAngle == 0)
        {
            oldBladderAngle = flexAngle; // First time value update
        }
        else if (flexAngle != Mathf.Infinity)
        {

            float differenceAngle = oldBladderAngle - flexAngle;
            

            if(differenceAngle < 0)
            {
                differenceAngle = -differenceAngle;
            }
            else
            {
                differenceAngle = 2 * differenceAngle;
            }

            float squishyness = Remap(Mathf.Abs(differenceAngle), 0, 15, 1.5f, 0.1f);

            orangeButton.GetComponent<Light>().intensity = Remap(Mathf.Abs(differenceAngle), 0 , 15 , 1, 4.5f);

            if (squishyness < 1.5f && squishyness > 0.0f) //Removing jitter from the sensor.
            {
                if (squishyness < 0.3f)
                {
                    totalAirPressure = totalAirPressure - 0.005f; //reducing pressure slowly
                    valveRetracted = false;
                    //Debug.Log(totalAirPressure);
                    if(totalAirPressure < 0 && totalAirPressure >= -0.1f)
                    {
                        SetGaugeRotation(totalAirPressure, masterIntake);
                    }
                    
                }
                airPump.transform.localScale = new Vector3(airPump.transform.localScale.x, (squishyness + 0.1f), airPump.transform.localScale.z);
            }

            if (squishyness > 0.7f)
            {
                valveRetracted = true;
            }

        }
    }

    void GetRotationData(string value, UduinoDevice board)
    {
        
        if (value.StartsWith("e"))
        {
            encoderAngle = float.Parse(value.Remove(0, value.IndexOf(' ') + 1));
            //encoderAngle = encoderAngle % 360;
            encoderAngle = -encoderAngle; // direction is reversed

            //Debug.Log("encoderAngle = " + encoderAngle);

            if(!savedEncoderVal)
            {
                initialEncoderVal = encoderAngle;
                //initialEncoderVal = -initialEncoderVal; // values are reversed
                savedEncoderVal = true;
            }

            //Version 2.0 to control engine RPM
            currentEngineRPM = 600.0f - (initialEncoderVal- encoderAngle) * 13.0f;

            if (currentEngineRPM <= 1)
            {
                currentEngineRPM = 1;
            }

            if (currentEngineRPM >= 1000)
            {
                engineRPMCheck = true;
            }



            totalEncoderAngle = encoderAngle;
            //dial.transform.localRotation = Quaternion.Euler(totalEncoderAngle/* / 32.0f*/, dial.transform.localEulerAngles.y, dial.transform.localEulerAngles.z); // divided by 8 for rotation scaling
            dial.transform.localRotation = Quaternion.Euler(-totalEncoderAngle/* / 32.0f*/, 0.0f, 0.0f); // divided by 8 for rotation scaling

            float dialRotation = 0.0f - (initialEncoderVal - encoderAngle);
            dialRotation = -dialRotation; // values are reversed
            float AngleOfZeroRPMs = 52.0f; 
            if (dialRotation > AngleOfZeroRPMs)
            {
                // 0 RPM angle on gauge
                dialRotation = AngleOfZeroRPMs;
            }
            RotateAnObjectOnPivot(dialRotation, rpm);
            
        }
    }

    private void UpdateEngineRPM(float encoderAngle)
    {
        engineSound.pitch = Remap(currentEngineRPM, 600.0f, 1000, 0.625f, 1.0f); ;//1f speed

        // Negative value results in increased pitch so clamp to 0.
        if(engineSound.pitch < 0.01)
        {
            engineSound.pitch = 0.01f;
        }
        //engineRPM.text = currentEngineRPM.ToString();
        //Debug.Log(currentEngineRPM);
    }

    IEnumerator lockMovement()
    {
        yield return new WaitForSeconds(5);
        panelMovement.enabled = false;

    }

}
