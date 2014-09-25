using UnityEngine;
using System.Collections;

public class CutsceneControllerMinipetHatchFluffy : CutsceneController {
	public Animation eggAnim;
	public Animation fluffyAnim;
	
	public override void Play(){
		fluffyAnim.gameObject.SetActive(false);
		eggAnim.gameObject.SetActive(true);
		eggAnim.Play();
	}
	
	public void popPet(){
		fluffyAnim.gameObject.SetActive(true);
		fluffyAnim.Play();
	}
}