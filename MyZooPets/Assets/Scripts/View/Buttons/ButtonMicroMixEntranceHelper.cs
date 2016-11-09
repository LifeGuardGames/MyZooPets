using UnityEngine;
using UnityEngine.UI;

public class ButtonMicroMixEntranceHelper : MonoBehaviour, IDropInventoryTarget {
	public ParticleSystem buttonChargeParticle;
	public ParticleSystem buttonBurstParticle;
	public Animation enableFireButtonAnimation;
	public Image imageButton;
	public Sprite activeFireButtonSprite;
	public Sprite inactiveFireButtonSprite;
	public ButtonMicroMixEntrance entranceScript;

	public void OnItemDropped(InventoryItem itemData) {
		if(itemData.ItemID == "Usable1" && !DataManager.Instance.GameData.MicroMix.EntranceHasCrystal) {
			DataManager.Instance.GameData.MicroMix.EntranceHasCrystal = true;
			InventoryManager.Instance.UsePetItem(itemData.ItemID);
			enableFireButtonAnimation.Play();
		}
		else {
			Debug.Log("MicroMix Entrance already has crystal on it");
		}
	}

	#region Animation Events
	public void StartChargeParticle() {
		buttonChargeParticle.Play();
	}

	public void StartBurstParticle() {
		buttonBurstParticle.Play();
	}

	public void FireEffectOn() {
		imageButton.sprite = activeFireButtonSprite;    //change button image 
	}

	public void FireEffectOff() {
		imageButton.sprite = inactiveFireButtonSprite;
	}

	public void PlayAudioCharge() {
		AudioManager.Instance.PlayClip("fireCharge");
	}

	public void PlayAudioPop() {
		AudioManager.Instance.PlayClip("fireButtonPop");
	}

	public void AnimationFinished() {
		entranceScript.Pass();
    }
	#endregion
}
