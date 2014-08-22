using UnityEngine;
using System;
using System.Collections;

//change this to PetInteraction Manager to handle item drop as well
public class PetInteractionManager : MonoBehaviour{
//	public PetAnimator petAnimator;

	void Start(){
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;
	}

	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	void OnTap(TapGesture gesture){
		try{
			string colliderName = gesture.Selection.collider.name;
			
			if(colliderName == this.gameObject.name){
				if(colliderName == "HeadCollider"){
					PetMoods moodState = DataManager.Instance.GameData.Stats.GetMoodState();
					PetHealthStates healthState = DataManager.Instance.GameData.Stats.GetHealthState();
					if(moodState == PetMoods.Sad || healthState == PetHealthStates.Sick ||
					   healthState == PetHealthStates.VerySick){
						PetAnimationManager.Instance.Swat();
					}
				}else if(colliderName == "HighFiveCollider"){
					PetAnimationManager.Instance.FinishHighFive();
				}else{}
			}
		}
		catch(NullReferenceException e){}
	}
	
	/// <summary>
	/// Raises the drag event.
	/// Note: This needs to be here so it catches the OnDrag event sent out by UICamera
	/// which belongs to NGUI. Finger Gesture also send out the same event so they
	/// need to be specified otherwise error will be thrown
	/// </summary>
	/// <param name="delta">Delta.</param>
	void OnDrag(Vector2 delta){}

	void OnDrag(DragGesture gesture){
		try{
			string colliderName = gesture.Selection.collider.name;
			
			if(colliderName == this.gameObject.name){
				switch(gesture.Phase){
				case ContinuousGesturePhase.Started:
					
					if(colliderName == "HeadCollider")
						PetAnimationManager.Instance.StartRubbing();
					else
						PetAnimationManager.Instance.StartTickling();
					
					break;
				case ContinuousGesturePhase.Ended:
					
					if(colliderName == "HeadCollider")
						PetAnimationManager.Instance.StopRubbing();
					else
						PetAnimationManager.Instance.StopTickling();
					
					break;
				}
			}
			else{
				PetAnimationManager.Instance.StopRubbing();
				PetAnimationManager.Instance.StopTickling();
			}
		}
		catch(NullReferenceException e){
			PetAnimationManager.Instance.StopRubbing();
			PetAnimationManager.Instance.StopTickling();
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
				
				if(invItem.ItemType == ItemType.Foods){
					hashSpeech.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("ITEM_NOT_HUNGRY"));
				}
				else{
					hashSpeech.Add(PetSpeechManager.Keys.MessageText, Localization.Localize("ITEM_NO_THANKS"));
				}
				hashSpeech.Add(PetSpeechManager.Keys.Follow3DTarget, gameObject);
				PetSpeechManager.Instance.Talk(hashSpeech);				
			}
		}
		//item is dropped on target, but not on the pet's collider
		else{
			PetAnimationManager.Instance.AbortFeeding();
		}
	}

	private void ShowPetReceivedFoodAnimation(){
		PetAnimationManager.Instance.FinishFeeding();
	}
}
