using UnityEngine;
using System.Collections;

public class ShooterCharacterController : MonoBehaviour {

	public enum ShooterCharacterStates{
		none,
		neutral,
		happy,
		distressed
	}

	public Animator anim;

	private ShooterCharacterStates stateAux;	// Keep track of state internally for shooting use

	public void SetState(ShooterCharacterStates state){
		switch(state){
		case ShooterCharacterStates.neutral:
			anim.SetBool("Happy", false);
			anim.SetBool("Distressed", false);
			stateAux = ShooterCharacterStates.neutral;
			break;
		case ShooterCharacterStates.happy:
			anim.SetBool("Happy", true);
			anim.SetBool("Distressed", false);
			stateAux = ShooterCharacterStates.happy;
			break;
		case ShooterCharacterStates.distressed:
			anim.SetBool("Happy", false);
			anim.SetBool("Distressed", true);
			stateAux = ShooterCharacterStates.distressed;
			break;
		default:
			Debug.LogError("Invalid state for character");
			break;
		}
	}

	public void Shoot(){
		switch(stateAux){
		case ShooterCharacterStates.neutral:
			anim.SetTrigger("Shoot");
			break;
		case ShooterCharacterStates.happy:
			anim.SetTrigger("Shoot");
			break;
		case ShooterCharacterStates.distressed:
			anim.SetTrigger("Shoot");
			break;
		default:
			Debug.LogError("Invalid state for character");
			break;
		}
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Happy")){
//			SetState(ShooterCharacterStates.happy);
//		}
//		else if(GUI.Button(new Rect(200, 100, 100, 100), "SAD")){
//			SetState(ShooterCharacterStates.distressed);
//		}
//		else if(GUI.Button(new Rect(300, 100, 100, 100), "Neugtral")){
//			SetState(ShooterCharacterStates.neutral);
//		}
//		else if(GUI.Button(new Rect(400, 100, 100, 100), "Shoot")){
//			Shoot();
//		}
//	}
}
