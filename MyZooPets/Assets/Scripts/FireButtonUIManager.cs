using UnityEngine;
using System;
using System.Collections;

public class FireButtonUIManager : Singleton<FireButtonUIManager> {
	public static EventHandler<EventArgs> FireButtonActive;

	public GameObject fireOrbDropTarget;
	public Animation buttonPluseAnimation;
	public GameObject sunBeam;
	public UIImageButton imageButton;

	private string activeButtonSpriteName = "buttonFireFull";
	private string inactiveButtonSpriteName = "buttonFireEmpty";

	// Use this for initialization
	void Start () {
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;

		bool canBreatheFire = DataManager.Instance.GameData.PetInfo.CanBreathFire();
		if(!canBreatheFire)
			TurnFireButtonEffectOff();
		else
			TurnFireButtonEffectOn();
	}

	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	public GameObject GetFireButtonReference(){
		return imageButton.gameObject;
	}

	public void TurnFireButtonEffectOff(){
		buttonPluseAnimation.Stop();
		sunBeam.SetActive(false);
		
		imageButton.hoverSprite = inactiveButtonSpriteName;
		imageButton.normalSprite = inactiveButtonSpriteName;
		imageButton.disabledSprite = inactiveButtonSpriteName;
		imageButton.pressedSprite = inactiveButtonSpriteName;

		imageButton.gameObject.SetActive(false);
		imageButton.gameObject.SetActive(true);
	}

	public void TurnFireButtonEffectOn(){
		if(FireButtonActive != null)
			FireButtonActive(this, EventArgs.Empty);

		buttonPluseAnimation.Play();
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
			
			// check to make sure the item can be used
			if(ItemLogic.Instance.CanUseItem(invItemID)){
				args.IsValidTarget = true;

				//notify inventory logic that this item is being used
				InventoryLogic.Instance.UsePetItem(invItemID);

				TurnFireButtonEffectOn();
			}
		}
	}
}
