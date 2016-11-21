using UnityEngine;
using System;

public class PetMovementListener : MonoBehaviour{
    void OnTap(TapGesture gesture){
        PetMovement.Instance.ProcessTap(gesture);
    }
}