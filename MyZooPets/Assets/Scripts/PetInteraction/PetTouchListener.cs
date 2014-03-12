using UnityEngine;
using System.Collections;

public class PetTouchListener : MonoBehaviour {
    public PetAnimator petAnimator;

    void OnTap(TapGesture gesture){
        string colliderName = gesture.Selection.collider.name;
        if(colliderName == "Pet_LWF"){
            if(ClickManager.Instance.CanRespondToTap() && !petAnimator.IsBusy()){
                petAnimator.PlayRestrictedAnim("Poke", true);
                PetMovement.Instance.StopMoving(false);
            }
        }
    } 
}
