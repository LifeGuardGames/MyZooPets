using UnityEngine;
using System.Collections;

public class CutsceneControllerMinipetHatchPebble : CutsceneController {
	public Animation eggAnim;
	public Animation pebbleAnim;
	
	public override void Play(){
		pebbleAnim.gameObject.SetActive(false);
		eggAnim.gameObject.SetActive(true);
		eggAnim.Play();
	}
	
	public void popPet(){
		pebbleAnim.gameObject.SetActive(true);
		pebbleAnim.Play();
	}
}