using UnityEngine;

public class EggController : Singleton<EggController>{
	public Animation eggAnimation;
	public SpriteRenderer eggSprite;
	public ParticleSystem shellParticle;
	public ParticleColorChange colorChange;
	public GameObject fingerHint;
	public Collider2D eggCollider;

	public GameObject crack1;
	public GameObject crack2;

	public void Init(){
		eggAnimation["eggIdle"].wrapMode = WrapMode.Loop;
		eggAnimation.Play("eggIdle");

		crack1.SetActive(false);
		crack2.SetActive(false);
		fingerHint.SetActive(false);
	}

	void OnMouseUpAsButton() {
		if(!CustomizationUIManager.Instance.isHatchingPet) {
			CustomizationUIManager.Instance.ShowColorChooseUI();
		}
		else {
			CustomizationUIManager.Instance.HatchEggTap();
        }
	}

	public void ToggleEggCollider(bool isColliderOn) {
		eggCollider.enabled = isColliderOn;
    }

	public void EggCrack(int crackNumber){
		shellParticle.Play();
		AudioManager.Instance.PlayClip("eggCrack", variations:3);
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
	
	// Turn egg wiggle animation on/off
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

	// User tapping the egg
	public void EggHatchingTapped(){
		eggAnimation.Play("eggClickCrack");
		shellParticle.Play();
		AudioManager.Instance.PlayClip("eggCrack", variations:3);
	}
}
