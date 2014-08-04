using UnityEngine;
using System.Collections;

//change this to PetInteraction Manager to handle item drop as well
public class PetInteractionManager : MonoBehaviour{
	public PetAnimator petAnimator;

	void Start(){
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;
	}

	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	void OnTap(TapGesture gesture){
		string colliderName = gesture.Selection.collider.name;
		if(colliderName == this.gameObject.name){
			if(ClickManager.Instance.CanRespondToTap() && !petAnimator.IsBusy()){
				petAnimator.PlayRestrictedAnim("Poke", true);
				PetMovement.Instance.StopMoving(false);
			}
		}
	}

	/// <summary>
	/// Items dropped on target event handler.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		if(args.TargetCollider.name == this.gameObject.name){
			string invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);

			// don't allow fire orb drop on pet
			if(invItemID == "Usable1") return;

			// check to make sure the item can be used
			if(ItemLogic.Instance.CanUseItem(invItemID)){
				args.IsValidTarget = true;
				
				if(invItem != null && invItem.ItemType == ItemType.Foods)
					ShowPetReceivedFoodAnimation();		
				
				//notify inventory logic that this item is being used
				InventoryLogic.Instance.UsePetItem(invItemID);
			}
			else{
				// else the drop was valid or the item could not be used...show a message
				Hashtable hashSpeech = new Hashtable();
				
				if(invItem.ItemType == ItemType.Foods)
					hashSpeech.Add(PetSpeechController.Keys.MessageText, Localization.Localize("ITEM_NOT_HUNGRY"));
				else
					hashSpeech.Add(PetSpeechController.Keys.MessageText, Localization.Localize("ITEM_NO_THANKS"));
				
				PetSpeechController.Instance.Talk(hashSpeech);				
			}
		}
	}

	private void ShowPetReceivedFoodAnimation(){
		PetAnimationManager.Instance.Feed();
	}
}
