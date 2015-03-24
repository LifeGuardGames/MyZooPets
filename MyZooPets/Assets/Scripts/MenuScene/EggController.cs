using UnityEngine;
using System.Collections;

public class EggController : Singleton<EggController>{
	public Animation eggAnimation;
	public SpriteRenderer eggSprite;
	public ParticleSystem shellParticle;
	public ParticleColorChange colorChange;
	public GameObject fingerHint;

	public GameObject crack1;
	public GameObject crack2;

	public void Init(){
		eggAnimation["eggIdle"].wrapMode = WrapMode.Loop;
		eggAnimation.Play("eggIdle");

		crack1.SetActive(false);
		crack2.SetActive(false);
		fingerHint.SetActive(false);
	}
	
	public void EggCrack(int crackNumber){
		shellParticle.Play();
		switch(crackNumber){
		case 1:
			crack1.SetActive(true);
			crack2.SetActive(false);
			eggAnimation.Play("eggShakeOnce");
			break;
		case 2:
			crack1.SetActive(false);
			crack2.SetActive(true);
			eggAnimation.Play("eggShakeOnce");
			break;
		default:
			Debug.LogError("Invalid crack number " + crackNumber);
			break;
		}
	}

	/// <summary>
	/// Turn egg wiggle animation on/off
	/// </summary>
	public void ToggleEggIdleAnimation(bool isOn){
		if(isOn){
			eggAnimation["eggIdle"].wrapMode = WrapMode.Loop;
			eggAnimation.Play("eggIdle");
		}
		else{
			eggAnimation["eggIdle"].wrapMode = WrapMode.Once;	// Just wait for this to stop
		}
	}

	public void ChangeColor(string spriteName){
		colorChange.ChangeColor(spriteName);
		Sprite sprite = Resources.Load<Sprite>(spriteName);
		eggSprite.sprite = sprite;
	}

	public void ToggleFingerHint(bool isOn){
		fingerHint.SetActive(isOn ? true : false);
	}

	public void EggHatchingTapped(){
		eggAnimation.Play("eggClickCrack");
		shellParticle.Play();
	}
}
