using UnityEngine;
using System.Collections;

/*
    Rescue Body Shaker (Rescue Step 2).

    This listens for the user's touch input, and shakes in response to the user shaking the device 

     When the counter reaches its target (shakeTarget), the step will be
    complete.
*/
public class RescueShaker : MonoBehaviour {
    //Variables for tracking accelerometer 
    private float accelerometerUpdateInterval = 1.0f/60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa).
    private float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation, or at least according to Brady! ;)
    private float shakeDetectionThreshold = 2.0f;
    private float lowPassFilterFactor;
    private Vector3 lowPassValue = Vector3.zero;
    private Vector3 acceleration;
    private Vector3 deltaAcceleration;

    //Inhaler game 
    private int gameStepID = 2;
    private int shakeCounter = 0; //decides how long user needs to shake before it's recognized

    void Start(){
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds; 
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    void Update(){
        if(gameStepID != InhalerLogic.Instance.CurrentStep) return;

        acceleration = Input.acceleration;
        LowPassFilterAccelerometer();
        deltaAcceleration = acceleration - lowPassValue;
        if(deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold){
            // Perform your "shaking actions" here, with suitable guards in the if check above, if necessary to not, to not fire again if they're already being performed.
            Debug.Log("Shake event detected at time "+Time.time);
            shakeCounter++;
        }

        if(shakeCounter == 30){ //shake about 2 seconds
            Handheld.Vibrate();
            InhalerLogic.Instance.NextStep();
        }
    }

    /*
        Source: unity3d documentation
        Accelerometer readings can be jerky and noisy. Applying low-pass filtering on the signal allows
        you to smooth it and get rid of high frequency noise
    */
    private void LowPassFilterAccelerometer(){
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
    }
}