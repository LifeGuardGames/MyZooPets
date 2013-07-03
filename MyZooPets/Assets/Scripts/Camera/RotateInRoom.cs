using UnityEngine;
using System.Collections;

public class RotateInRoom : UserNavigation {

    float currentYRotation;
    public float rotationIncrement = 72;
    public bool inverse = false;
    Hashtable optional = new Hashtable();
    bool lockRotation;

	void Start () {
        currentYRotation = transform.eulerAngles.y;
        optional.Add("onComplete", "FinishedRotation");
        if (inverse){
            rotationIncrement = - rotationIncrement;
        }

        // Init swipe listener.
        SwipeDetection.OnSwipeDetected += OnSwipeDetected;
	}

    public override void ToTheRight(){
        if (!lockRotation){
            if (ClickManager.CanRespondToTap()){
                lockRotation = true;
                currentYRotation += rotationIncrement;
                LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
            }
        }
    }

    public override void ToTheLeft(){
        if (!lockRotation){
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
