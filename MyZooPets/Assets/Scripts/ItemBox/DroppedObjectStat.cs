using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is an object that is on the ground, in the
/// 3D world (although it may be 2D) that the player
/// can pick up to obtain.  It is a stat (like xp or
/// gold) that the play can get by picking it up.
/// </summary>
public class DroppedObjectStat : DroppedObject{

	public UILabel labelPoints; // label for displaying the amount of points on this object
	private HUDElementType hudElementType; // the stat that this object represents
	private int amount; // amount that this object gives for the stat
	

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

	/// <summary>
	/// Obtains the object. Puts the item into the player's inventory.
	/// </summary>
	protected override void ObtainObject(){
		DroppedItemStates eState = GetState();
		
		// if this item isn't in the right state, return
		if(eState != DroppedItemStates.Dropped && eState != DroppedItemStates.PickedUp)
			return;

		// set the state to being awarded
		SetState(DroppedItemStates.Awarded);
		
		// add the stat...I really need to refactor StatsController...
		int xp = hudElementType == HUDElementType.Xp ? amount : 0;
		int coins = hudElementType == HUDElementType.Coin ? amount : 0;
		int health = hudElementType == HUDElementType.Health ? amount : 0;
		int mood = hudElementType == HUDElementType.Hunger ? amount : 0;

		Debug.Log("DROPPED STAT");
		// Pass in the global object and flag is3DObject
		StatsController.Instance.ChangeStats(xpDelta: xp, xpPos: transform.position, coinsDelta: coins, coinsPos: transform.position,
		                                     healthDelta: health, healthPos: transform.position,
		                                     hungerDelta: mood, hungerPos: transform.position, is3DObject: true);

		// destroy the object
		GameObject go = GetGameObject();
		Destroy(go);		
	}

	/// <summary>
	/// Callback sent from the stats logic because it
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
