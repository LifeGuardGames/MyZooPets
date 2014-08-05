using UnityEngine;
using System.Collections;

/// <summary>
/// Pet animation event listener. All the animation events fired by pet related
/// animations are handled here
/// </summary>
public class PetAnimationEventHandler : MonoBehaviour {
	public void PetAnimationEvent(string eventName){
		switch(eventName){
		case "FinishBlowingFire":
			Debug.Log("done with blowing fire aniamtion");
			PetAnimationManager.Instance.DoneWithFireBlowAnimation();
			break;
		}
	}
}
