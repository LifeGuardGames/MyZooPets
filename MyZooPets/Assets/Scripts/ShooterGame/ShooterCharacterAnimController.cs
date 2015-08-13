using UnityEngine;
using System.Collections;

public class ShooterCharacterAnimController : MonoBehaviour {
	public enum ShooterCharacterStates{
		None,
		Neutral,
		Happy,
		Distressed,
		Dead
	}

	public Animator anim;

	public void SetState(ShooterCharacterStates state){
		switch(state){
		case ShooterCharacterStates.Neutral:
			anim.SetBool("Happy", false);
			anim.SetBool("Distressed", false);
			anim.SetBool("Dead", false);
			break;
		case ShooterCharacterStates.Happy:
			anim.SetBool("Happy", true);
			anim.SetBool("Distressed", false);
			anim.SetBool("Dead", false);
			break;
		case ShooterCharacterStates.Distressed:
			anim.SetBool("Happy", false);
			anim.SetBool("Distressed", true);
			anim.SetBool("Dead", false);
			break;
		case ShooterCharacterStates.Dead:
			anim.SetTrigger("Die");
			anim.SetBool("Dead", true);
			break;
		default:
			Debug.LogError("Invalid state for character");
			break;
		}
	}

	public void Shoot(){
		anim.SetTrigger("Shoot");
	}

//	void OnGUI(){
//		if(GUI.Button(new Rect(100, 100, 100, 100), "Happy")){
//			SetState(ShooterCharacterStates.dead);
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
