using UnityEngine;
using System;

public class PetInteractionManager : Singleton<PetInteractionManager>, IDropInventoryTarget {
	public bool isInteractable = true;

	void OnTap(TapGesture gesture) {
		if(isInteractable) {
			try {
				string colliderName = gesture.Selection.GetComponent<Collider>().name;

				if(colliderName == this.gameObject.name) {
					if(colliderName == "HeadCollider") {
						PetMoods moodState = DataManager.Instance.GameData.Stats.GetMoodState();
						PetHealthStates healthState = DataManager.Instance.GameData.Stats.GetHealthState();
						if(moodState == PetMoods.Sad || healthState == PetHealthStates.Sick ||
						   healthState == PetHealthStates.VerySick) {
							PetAnimationManager.Instance.Swat();
						}
					}
					else if(colliderName == "HighFiveCollider") {
						PetAnimationManager.Instance.FinishHighFive();
					}
				}
			}
			catch(NullReferenceException e) {
				Debug.LogException(e);
			}
		}
	}

	/// <summary>
	/// Raises the drag event.
	/// Note: This needs to be here so it catches the OnDrag event sent out by UICamera
	/// which belongs to NGUI. Finger Gesture also send out the same event so they
	/// need to be specified otherwise error will be thrown
	/// </summary>
	/// <param name="delta">Delta.</param>
	void OnDrag(Vector2 delta) { }

	void OnDrag(DragGesture gesture) {
		if(isInteractable) {
			try {
				if(gesture.Selection == null) {
					PetAnimationManager.Instance.StopRubbing();
					PetAnimationManager.Instance.StopTickling();
					return;
				}

				string colliderName = gesture.Selection.GetComponent<Collider>().name;
				if(colliderName == this.gameObject.name) {
					switch(gesture.Phase) {
						case ContinuousGesturePhase.Started:
							if(colliderName == "HeadCollider") {
								PetAnimationManager.Instance.StartRubbing();
							}
							else {
								PetAnimationManager.Instance.StartTickling();
							}
							break;
						case ContinuousGesturePhase.Ended:
							if(colliderName == "HeadCollider") {
								PetAnimationManager.Instance.StopRubbing();
							}
							else {
								PetAnimationManager.Instance.StopTickling();
							}
							break;
					}
				}
				else {
					PetAnimationManager.Instance.StopRubbing();
					PetAnimationManager.Instance.StopTickling();
				}
			}
			catch(NullReferenceException e) {
				Debug.LogException(e);
				PetAnimationManager.Instance.StopRubbing();
				PetAnimationManager.Instance.StopTickling();
			}
		}
	}

	// Implementation for IDropInventoryTarget
	public void OnItemDropped(InventoryItem itemData) {
		// don't allow fire orb drop on pet
		if(itemData.ItemID == "Usable1") {
			return;
		}

		// check to make sure the item can be used
		if(ItemManager.Instance.CanUseItem(itemData.ItemID)) {
			// notify inventory logic that this item is being used
			InventoryManager.Instance.UsePetItem(itemData.ItemID);

			if(itemData.ItemType == ItemType.Foods) {
				PetAnimationManager.Instance.FinishFeeding();
			}
		}
		else {
			// else the drop was valid or the item could not be used...show a message
			if(itemData.ItemType == ItemType.Foods) {
				PetSpeechAI.Instance.ShowItemNotHungryMsg();
			}
			else {
				PetSpeechAI.Instance.ShowItemNoThanksMsg();
			}
		}
	}
}
