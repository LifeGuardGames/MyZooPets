using UnityEngine;
using System.Collections;

public class RotateInRoom : MonoBehaviour {

    float currentYRotation;
    public float rotationIncrement = 72;
    bool inverse = true;
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

    public void RotateRight(){
        if (!lockRotation){
            if (ClickManager.CanRespondToTap()){
                lockRotation = true;
                currentYRotation += rotationIncrement;
                LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
            }
        }
    }

    public void RotateLeft(){
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

    void OnSwipeDetected(Swipe s){
        switch (s){
            // case Swipe.Up:
            // print("Swipe.Up");
            // break;

            // case Swipe.Down:
            // print("Swipe.Down");
            // break;

            case Swipe.Left:
            print("Swipe.Left");
            RotateLeft();
            break;

            case Swipe.Right:
            print("Swipe.Right");
            RotateRight();
            break;
        }
    }
}
