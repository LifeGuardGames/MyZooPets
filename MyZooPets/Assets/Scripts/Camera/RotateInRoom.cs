using UnityEngine;
using System.Collections;

public class RotateInRoom : UserNavigation {

    float currentYRotation;
    public float rotationIncrement = 72;
    public bool inverse = false;
    Hashtable optional = new Hashtable();
    bool lockRotation;

    float minLeft;
    float maxRight;

	void Start () {
        currentYRotation = transform.eulerAngles.y;
        optional.Add("onComplete", "FinishedRotation");
        if (inverse){
            rotationIncrement = - rotationIncrement;
        }

        // Init swipe listener.
        SwipeDetection.OnSwipeDetected += OnSwipeDetected;

        // Init limits to room navigation
        minLeft = 0;
        maxRight = rotationIncrement;
	}

    public override void ToTheRight(){
        if (!lockRotation && currentYRotation < maxRight){
            if (ClickManager.CanRespondToTap()){
                lockRotation = true;
                currentYRotation += rotationIncrement;
                LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
            }
        }
    }

    public override void ToTheLeft(){
        if (!lockRotation && currentYRotation > minLeft){
            if (ClickManager.CanRespondToTap()){
                lockRotation = true;
                currentYRotation -= rotationIncrement;
                LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
            }
        }
    }

    void FinishedRotation(){
        lockRotation = false;
        currentYRotation = transform.eulerAngles.y; // normalize angle
    }

}
