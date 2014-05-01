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

public class DroppedObjectStat : DroppedObject{
	// label for displaying the amount of points on this object
	public UILabel labelPoints;
	
	// the stat that this object represents
	private HUDElementType hudElementType;
	
	// amount that this object gives for the stat
	private int amount;
	
	//---------------------------------------------------
	// Init()
	// Inits this dropped object with a stat.
	//---------------------------------------------------
	/// <summary>
	/// Init the DroppedObject
	/// </summary>
	/// <param name="eStat">Hud element type</param>
	/// <param name="amount">amount this object gives for the stat</param>
	public void Init(HUDElementType hudElementType, int amount){
		// set the state of this object to dropped
		SetState(DroppedItemStates.Dropped);
		
		this.hudElementType = hudElementType;
		this.amount = amount;
		
		// set the texture of this dropped item
		if(sprite != null){
			string strSprite = StatsController.Instance.GetStatIconName(hudElementType);
			sprite.spriteName = strSprite;
		}
		else
			Debug.LogError("No sprite", gameObject);

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
		int nXP = hudElementType == HUDElementType.Points ? amount : 0;
		int nCoins = hudElementType == HUDElementType.Stars ? amount : 0;
		int nHealth = hudElementType == HUDElementType.Health ? amount : 0;
		int nMood = hudElementType == HUDElementType.Mood ? amount : 0;

		StatsController.Instance.ChangeStats(deltaPoints: nXP, deltaStars: nCoins, 
		                                     deltaHealth: nHealth, deltaMood: nMood, bFloaty: true);

		// destroy the object
		GameObject go = GetGameObject();
		Destroy(go);		
	}	

	/// <summary>
	/// Callback sent from the stats logic because it
	/// is being destroyed (likely because the scene is
	/// changing).
	/// </summary>
	protected override void AutoCollectAndDestroy(){
		// if the inventory is being destroyed, but this dropped item has not yet been awarded, award it
		DroppedItemStates eState = GetState();
		if(eState != DroppedItemStates.Awarded)
			ObtainObject();
	}	
}
