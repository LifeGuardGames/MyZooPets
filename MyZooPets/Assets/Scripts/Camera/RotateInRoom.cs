using UnityEngine;
using System.Collections;

public class RotateInRoom : UserNavigation {

    int currentYRotation;
    int currentPartition = 0;
    int rotationIncrement;
    Hashtable optional = new Hashtable();
    bool lockRotation;

    /*
        Set this manually. enabledPartitions[0] means the default partition that appears when the game is started.
    */
    bool[] enabledPartitions = {true, true, true, false, false};
    int numPartitions; // determined in Start()

	private PetMovement petmovement;

	protected override void Start () {
        base.Start();

        numPartitions = enabledPartitions.Length;
        rotationIncrement = 360 / numPartitions;

        currentYRotation = (int)transform.eulerAngles.y;
        optional.Add("onComplete", "FinishedRotation");

		petmovement = GameObject.Find("PetMovement").GetComponent<PetMovement>();
	}

    public override void ToTheRight(){
        if (IsRightArrowEnabled()){
            if (ClickManager.CanRespondToTap()){
                lockRotation = true;
                currentYRotation += rotationIncrement;
                currentPartition = GetRight();
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
                currentPartition = GetLeft();
                UpdateClickableItemsInPartition();
                LeanTween.rotateY(gameObject, currentYRotation, 1.0f, optional);
            }
        }
    }

    protected override bool IsRightArrowEnabled(){
        return (!lockRotation && enabledPartitions[GetRight()]);
    }

    protected override bool IsLeftArrowEnabled(){
        return (!lockRotation && enabledPartitions[GetLeft()]);
    }

    int GetRight(){
        if (currentPartition == numPartitions - 1){
            return 0;
        }
        return currentPartition + 1;
    }

    int GetLeft(){
        if (currentPartition == 0){
            return numPartitions - 1;
        }
        return currentPartition - 1;
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
