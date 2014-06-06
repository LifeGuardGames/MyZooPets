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
		if(colliderName == "Pet_LWF"){
			if(ClickManager.Instance.CanRespondToTap() && !petAnimator.IsBusy()){
				petAnimator.PlayRestrictedAnim("Poke", true);
				PetMovement.Instance.StopMoving(false);
			}
		}
	}

	/// <summary>
	/// Items the dropped on target event handler.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		if(args.TargetCollider.name == gameObject.name){
			string invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);
			
			// check to make sure the item can be used
			if(ItemLogic.Instance.CanUseItem(invItemID)){
				args.IsValidTarget = true;
				
				if(invItem != null && invItem.ItemType == ItemType.Foods)
					ShowPetReceivedFoodAnimation();		
				
				//notify inventory logic that this item is being used
				InventoryLogic.Instance.UseItem(invItemID);
				
				
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
		if(!petAnimator.IsBusy()){
			petAnimator.PlayUnrestrictedAnim("Eat", true);
			PetMovement.Instance.StopMoving(false);
		}
	}
}
