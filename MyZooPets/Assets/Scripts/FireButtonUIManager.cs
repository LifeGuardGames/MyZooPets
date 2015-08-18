using UnityEngine;
using System;
using System.Collections;

public class FireButtonUIManager : Singleton<FireButtonUIManager> {
	public static EventHandler<EventArgs> FireButtonActive;

	public GameObject fireOrbDropTarget;
	public Animation buttonAnimation;
	public Animation EnableFireButtonAnimation;
	public ParticleSystem buttonChargeParticle;
	public ParticleSystem buttonBurstParticle;
	public GameObject sunBeam;
	public UIImageButton imageButton;
	public Collider fireButtonCollider;

	private string activeButtonSpriteName = "fireButtonOn";
	private string inactiveButtonSpriteName = "fireButtonOff";

	// Use this for initialization
	void Start () {
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;

		bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();
		if(!canBreatheFire)
			TurnFireButtonEffectOff();
		else
			TurnFireButtonOn(0);
	}

	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	public GameObject GetFireButtonReference(){
		return imageButton.gameObject;
	}

	public void TurnFireButtonEffectOff(){
		buttonAnimation.Stop();
		sunBeam.SetActive(false);
		
		imageButton.hoverSprite = inactiveButtonSpriteName;
		imageButton.normalSprite = inactiveButtonSpriteName;
		imageButton.disabledSprite = inactiveButtonSpriteName;
		imageButton.pressedSprite = inactiveButtonSpriteName;

		imageButton.gameObject.SetActive(false);
		imageButton.gameObject.SetActive(true);
	}

	// Start the animation for the fire button enabling process, this will call the below 4 functions
	public void StartFireButtonAnimation(){
		EnableFireButtonAnimation.Play();
	}

	// This is called from the animation event
	public void FireButtonAnimationActivate(){
		EnableFireButtonAnimation.Stop();
		buttonAnimation.Play();
	}

	// This is called from the animation event
	public void StartChargeParticle(){
		buttonChargeParticle.Play();
	}

	// This is called from the animation event
	public void StartBurstParticle(){
		if(buttonBurstParticle){
			buttonBurstParticle.Play();
		}
	}

	// This is called from the animation event / or from start
	public void TurnFireButtonOn(int isCalledFromAnimation){

		// Turn on the pulsing manually because its not done from animation
		if(isCalledFromAnimation == 0){
			FireButtonAnimationActivate();
		}

		if(FireButtonActive != null)
			FireButtonActive(this, EventArgs.Empty);

		sunBeam.SetActive(true);

		//change button image 
		imageButton.hoverSprite = activeButtonSpriteName;
		imageButton.normalSprite = activeButtonSpriteName;
		imageButton.disabledSprite = activeButtonSpriteName;
		imageButton.pressedSprite = activeButtonSpriteName;

		imageButton.gameObject.SetActive(false);
		imageButton.gameObject.SetActive(true);
	}
	
	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		if(args.TargetCollider.name == fireOrbDropTarget.name){

			string invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);
			int numOfFireBreaths = DataManager.Instance.GameData.PetInfo.FireBreaths;

			//only works if item is flame crystal and pet can't breathe fire yet
			if(invItem.ItemID == "Usable1" && numOfFireBreaths == 0){
				// check to make sure the item can be used
				if(ItemLogic.Instance.CanUseItem(invItemID)){
					args.IsValidTarget = true;
					
					//notify inventory logic that this item is being used
					InventoryLogic.Instance.UsePetItem(invItemID);
					
					StartFireButtonAnimation();
				}
			}
		}
	}

	// Audio handlers for animation event
	public void PlayAudioCharge(){
		AudioManager.Instance.PlayClip("fireCharge");
	}

	public void PlayAudioPop(){
		AudioManager.Instance.PlayClip("fireButtonPop");
	}
}
