using UnityEngine;
using System.Collections;

public class CutsceneControllerMinipetHatchFireBird : CutsceneController{

	public Animation eggAnim;
	public Animation fireBirdAnim;

	public override void Play(){
		fireBirdAnim.gameObject.SetActive(false);
		eggAnim.gameObject.SetActive(true);
		eggAnim.Play();
	}

	public void popBird(){
		fireBirdAnim.gameObject.SetActive(true);
		fireBirdAnim.Play();
	}

	public void playRollAudio(){
//		AudioManager.Instance.PlayClip("drumRoll");
	}

	public void playHatchAudio(){
		AudioManager.Instance.PlayClip("fanfare3");
	}
}
