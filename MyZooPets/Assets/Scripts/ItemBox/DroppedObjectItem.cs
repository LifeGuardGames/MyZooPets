using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Dropped object item.
/// This is an item that is on the ground, in the
/// 3D world (although it may be 2D) that the player
/// can pick up to obtain.
/// </summary>
public class DroppedObjectItem : DroppedObject{
	// the item that this object represents
	private Item dataItem;

	/// <summary>
	/// Init the specified dropped item.
	/// </summary>
	/// <param name="item">Item.</param>
	public void Init(Item item){
		// set the state of this item to dropped
		SetState(DroppedItemStates.Dropped);
		
		// set the texture of this dropped item
		if(sprite != null){
			string strSprite = item.TextureName;
			sprite.spriteName = strSprite;
			
			dataItem = item;
		}
		else
			Debug.LogError("No sprite", gameObject);
		
		// also listen for when the inventory logic is being destroyed
		// InventoryLogic.Instance.OnBeingDestroyed += OnManagerDestroyed;
	}

	/// <summary>
	/// Changes the auto collect time.
	/// Use this to increase/decrease the time waiting before the item auto
	/// collects itself
	/// </summary>
	/// <param name="autoCollectTime">Auto collect time.</param>
	public void ChangeAutoCollectTime(float autoCollectTime){
		this.timeBeforeAutoCollect = autoCollectTime;
	}

	/// <summary>
	/// Obtains the object.
	/// Puts the item into the player's inventory.
	/// </summary>
	protected override void ObtainObject(){
		DroppedItemStates eState = GetState();
		
		// if this item isn't in the right state, return
		if(eState != DroppedItemStates.Dropped && eState != DroppedItemStates.PickedUp)
			return;

		// set the state to being awarded
		SetState(DroppedItemStates.Awarded);
		
		// Analytics
		Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_RECEIVED, dataItem.Type, dataItem.ID);

		// add it to the player's inventory
		InventoryLogic.Instance.AddItem(dataItem.ID, 1);
		
		// destroy the object
		Destroy(gameObject);		
	}	

	/// <summary>
	/// Collects and destroy automatically.
	/// Callback sent from the inventory logic because it
	/// is being destroyed (likely because the scene is
	/// changing).
	/// </summary>
	protected override void CollectAndDestroyAutomatically(){
		// if the inventory is being destroyed, but this dropped item has not yet been awarded, award it
		DroppedItemStates eState = GetState();
		if(eState != DroppedItemStates.Awarded)
			ObtainObject();
	}	
}
