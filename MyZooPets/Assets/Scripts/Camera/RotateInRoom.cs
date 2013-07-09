using UnityEngine;
using System.Collections;

public class RotateInRoom : UserNavigation {

    int currentYRotation;
    int currentPartition = 1;
    public int rotationIncrement = 72;
    Hashtable optional = new Hashtable();
    bool lockRotation;

    float minLeft;
    float maxRight;

	void Start () {
        currentYRotation = (int)transform.eulerAngles.y;
        optional.Add("onComplete", "FinishedRotation");

        // Init swipe listener.
        SwipeDetection.OnSwipeDetected += OnSwipeDetected;

        // Init limits to room navigation
        minLeft = 0;
        maxRight = rotationIncrement;
	}

    public override void ToTheRight(){
        if (!lockRotation && IsRightArrowEnabled()){
            if (ClickManager.CanRespondToTap()){
                lockRotation = true;
                currentYRotation += rotationIncrement;
                currentPartition += 1;
                UpdateClickableItemsInPartition();
                LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
            }
        }
    }

    public override void ToTheLeft(){
        if (!lockRotation && IsLeftArrowEnabled()){
            if (ClickManager.CanRespondToTap()){
                lockRotation = true;
                currentYRotation -= rotationIncrement;
                currentPartition -= 1;
                UpdateClickableItemsInPartition();
                LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
            }
        }
    }

    protected override bool IsRightArrowEnabled(){
        return (!lockRotation && currentYRotation < maxRight);
    }

    protected override bool IsLeftArrowEnabled(){
        return (!lockRotation && currentYRotation > minLeft);
    }

    void FinishedRotation(){
        lockRotation = false;
        currentYRotation = (int)transform.eulerAngles.y; // normalize angle
    }

    void UpdateClickableItemsInPartition(){
        switch (currentPartition){
            case 1:
            GetComponent<RememberClickableItems>().ActivatePartition1();
            break;

            case 2:
            GetComponent<RememberClickableItems>().ActivatePartition2();
            break;

            case 3:
            GetComponent<RememberClickableItems>().ActivatePartition3();
            break;
        }
    }
}
