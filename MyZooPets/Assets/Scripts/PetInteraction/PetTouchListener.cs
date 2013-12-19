using UnityEngine;
using System.Collections;

public class PetTouchListener : MonoBehaviour {
    public PetAnimator petAnimator;

    void OnTap(TapGesture gesture){
        string colliderName = gesture.Hit.collider.name;
        if(colliderName == "Pet_LWF"){
            petAnimator.PlayRestrictedAnim("Poke", true);
            PetMovement.Instance.StopMoving(false);
        }
    } 
}
