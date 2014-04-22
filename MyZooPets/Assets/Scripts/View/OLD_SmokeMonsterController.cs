using UnityEngine;
using System.Collections;

public class SmokeMonsterController : MonoBehaviour {
	
	public tk2dSprite eyesSprite;
	public tk2dSprite mouthSprite;
	public GameObject face;
	
	private Animation anim;
	private string currentEyesSpritesName;
	private string currentMouthSpritesName;
	private const string HURT_EYES_SPRITE_NAME = "eyesHurt";
	
	// Joe -- I changed this from Start() to Awake() because the gating manager's Start() was being called before this Start() which was leading
	// to problems because anim had not yet been initialized.
	void Awake () {
		anim = face.animation;
		SetNormalAnimation("eyes1", "mouth1");
	}
	
	public void SetNormalAnimation(string eyesSpriteName, string mouthSpriteName){
		currentEyesSpritesName = eyesSpriteName;
		currentMouthSpritesName = mouthSpriteName;
	}
	
	public void PlayNormalAnimation(){
		anim.Play("hoverUpDownSmokeMonster");
		PlayAnimation(currentEyesSpritesName, currentMouthSpritesName);
	}
	
	public void PlayHurtAnimation(){
		anim.Play("shakeSmokeMonster");
		PlayAnimation(HURT_EYES_SPRITE_NAME, currentMouthSpritesName);
	}
	
	private void PlayAnimation(string eyesSpriteName, string mouthSpriteName){
		Debug.Log("SFD");
		eyesSprite.SetSprite(eyesSpriteName);
		mouthSprite.SetSprite(mouthSpriteName);
	}
	
	// void OnGUI(){
	// 	if(GUI.Button(new Rect(10f, 10f, 20f, 20f), "1")){
	// 		PlayHurtAnimation();
	// 	}
	// 	if(GUI.Button(new Rect(30f, 10f, 20f, 20f), "2")){
	// 		playNormalAnimation();
	// 	}
	// }
}
