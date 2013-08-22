using UnityEngine;
using System.Collections;

/*
    Rescue Inhaler Cap (Rescue Step 1).

    This listens for the user's touch on the cap.

    If the cap is tapped, it will be removed (it will destroy itself), and the game will move on to the next step.

*/
public class RescueCap : MonoBehaviour {
    private int gameStepID = 1;

    void Update()
    {
        if (InhalerLogic.Instance.CurrentStep != gameStepID) return;

        if (Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                if (InhalerUtility.IsTouchingObject(touch, gameObject)){
                    if (InhalerLogic.Instance.IsCurrentStepCorrect(gameStepID)){
                        Destroy(gameObject);
                        InhalerLogic.Instance.NextStep();
                    }
                }
            }
        }
    }
}