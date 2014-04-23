using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---------------------------------------------------
// DroppedObject_Stat
// This is an object that is on the ground, in the
// 3D world (although it may be 2D) that the player
// can pick up to obtain.  It is a stat (like xp or
// gold) that the play can get by picking it up.
//---------------------------------------------------

public class DroppedObject_Stat : DroppedObject{
	// label for displaying the amount of points on this object
	public UILabel labelPoints;
	
	// the stat that this object represents
	private HUDElementType eStat;
	
	// amount that this object gives for the stat
	private int nAmount;
	
	//---------------------------------------------------
	// Init()
	// Inits this dropped object with a stat.
	//---------------------------------------------------	
	public void Init(HUDElementType eStat, int nAmount){
		// set the state of this object to dropped
		SetState(DroppedItemStates.Dropped);
		
		this.eStat = eStat;
		this.nAmount = nAmount;
		
		// set the label
		//labelPoints.text = "" + nAmount;
		
		// set the texture of this dropped item
		if(sprite != null){
			string strSprite = StatsController.Instance.GetStatIconName(eStat);
			sprite.spriteName = strSprite;
		}
		else
			Debug.LogError("No sprite", gameObject);
		
		// also listen for when the stats controller is being destroyed
		// StatsController.Instance.OnBeingDestroyed += OnManagerDestroyed;
	}
	
	//---------------------------------------------------
	// ObtainObject()
	// Puts the item into the player's inventory.
	//---------------------------------------------------	
	protected override void ObtainObject(){
		DroppedItemStates eState = GetState();
		
		// if this item isn't in the right state, return
		if(eState != DroppedItemStates.Dropped && eState != DroppedItemStates.PickedUp)
			return;

		// set the state to being awarded
		SetState(DroppedItemStates.Awarded);
		
		// add the stat...I really need to refactor StatsController...
		int nXP = eStat == HUDElementType.Points ? nAmount : 0;
		int nCoins = eStat == HUDElementType.Stars ? nAmount : 0;
		int nHealth = eStat == HUDElementType.Health ? nAmount : 0;
		int nMood = eStat == HUDElementType.Mood ? nAmount : 0;
		StatsController.Instance.ChangeStats(nXP, Vector3.zero, nCoins, Vector3.zero, nHealth, Vector3.zero, nMood, Vector3.zero, false, false, true);
		
		// destroy the object
		GameObject go = GetGameObject();
		Destroy(go);		
	}	
	
	//---------------------------------------------------
	// AutoCollectAndDestroy()
	// Callback sent from the stats logic because it
	// is being destroyed (likely because the scene is
	// changing).
	//---------------------------------------------------	
	protected override void AutoCollectAndDestroy(){
		// if the inventory is being destroyed, but this dropped item has not yet been awarded, award it
		DroppedItemStates eState = GetState();
		if(eState != DroppedItemStates.Awarded)
			ObtainObject();
	}	

	//---------------------------------------------------
	// OnObjectDestroyed()
	//---------------------------------------------------		
	// protected override void OnObjectDestroyed(){
		// if this dropped object is being destroyed, has not yet been awarded, AND the inventory still exists, obtain it
		// DroppedItemStates eState = GetState();
		// if ( eState != DroppedItemStates.Awarded && StatsController.Instance != null )
		// 	ObtainObject();
	// }	
}
