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

	private PetMovement petmovement;

	protected override void Start () {
        base.Start();

        currentYRotation = (int)transform.eulerAngles.y;
        optional.Add("onComplete", "FinishedRotation");

        // Init limits to room navigation
        minLeft = 360 - (rotationIncrement);
        maxRight = rotationIncrement * 2;

		petmovement = GameObject.Find("PetMovement").GetComponent<PetMovement>();
	}

    public override void ToTheRight(){
        if (IsRightArrowEnabled()){
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
        if (IsLeftArrowEnabled()){
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
		// petmovement.movePetWithCamera(); //call this so pet follows the camera
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
