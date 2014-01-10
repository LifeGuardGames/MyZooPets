using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DroppedObject_Item
// This is an item that is on the ground, in the
// 3D world (although it may be 2D) that the player
// can pick up to obtain.
//---------------------------------------------------

public class DroppedObject_Item : DroppedObject {
	// the item that this object represents
	private Item dataItem;
	
	//---------------------------------------------------
	// Init()
	// Inits this dropped object with an item.
	//---------------------------------------------------	
	public void Init( Item item ) {
		// set the state of this item to dropped
		SetState( DroppedItemStates.Dropped );
		
		// set the texture of this dropped item
		if ( sprite != null ) {
			string strSprite = item.TextureName;
			sprite.spriteName = strSprite;
			
			dataItem = item;
		}
		else
			Debug.LogError("No sprite", gameObject);
		
		// also listen for when the inventory logic is being destroyed
		InventoryLogic.Instance.OnBeingDestroyed += OnManagerDestroyed;
	}
	
	//---------------------------------------------------
	// _ObtainObject()
	// Puts the item into the player's inventory.
	//---------------------------------------------------	
	protected override void _ObtainObject() {
		DroppedItemStates eState = GetState();
		
		// if this item isn't in the right state, return
		if ( eState != DroppedItemStates.Dropped && eState != DroppedItemStates.PickedUp )
			return;

		// set the state to being awarded
		SetState( DroppedItemStates.Awarded );
		
		// Analytics
		Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_RECEIVED, dataItem.Type, dataItem.ID);

		// add it to the player's inventory
		InventoryLogic.Instance.AddItem( dataItem.ID, 1);
		
		// destroy the object
		Destroy( gameObject );		
	}	
	
	//---------------------------------------------------
	// OnObjectDestroyed()
	//---------------------------------------------------		
	protected override void OnObjectDestroyed() {
		// if this dropped item is being destroyed, has not yet been awarded, AND the inventory still exists, obtain it
		DroppedItemStates eState = GetState();
		if ( eState != DroppedItemStates.Awarded && InventoryLogic.Instance != null )
			ObtainObject();
	}	
	
	//---------------------------------------------------
	// _OnManagerDestroyed()
	// Callback sent from the inventory logic because it
	// is being destroyed (likely because the scene is
	// changing).
	//---------------------------------------------------	
	protected override void _OnManagerDestroyed() {
		// if the inventory is being destroyed, but this dropped item has not yet been awarded, award it
		DroppedItemStates eState = GetState();
		if ( eState != DroppedItemStates.Awarded )
			ObtainObject();
	}	
}
