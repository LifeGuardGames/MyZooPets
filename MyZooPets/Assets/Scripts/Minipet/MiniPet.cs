﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Script to control Minipet and contains the basic properties of a minipet.
/// Should attach this script to the highest parent in the minipet prefab
/// </summary>
public class MiniPet : MonoBehaviour {

	/*
	 * Attributes:
	 * Immutable
	 * ID
	 * Type
	 * Dict of amount of food required to lv up
	 * Dict of lv up reward (gems, coins, special decos)
	 * 
	 * prefab name
	 * startiing position
	 * walking pattern?
	 * 
	 * ------------------
	 * Mutable
	 * current lv
	 * current food xp
	 * isUnlocked
	 * 
	 * cannot use DM directly from here so connect to MinipetManager for any mutable data
	 */

	//On item drop handler. if the correct food modify current food xp

	//On tap handler. do a funny dance or sth

	private string id;

	void Start(){
		InventoryUIManager.ItemDroppedOnTargetEvent += ItemDroppedOnTargetEventHandler;
	}
	
	void OnDestroy(){
		InventoryUIManager.ItemDroppedOnTargetEvent -= ItemDroppedOnTargetEventHandler;
	}

	void OnTap(TapGesture gesture){
		string colliderName = gesture.Selection.collider.name;
		Debug.Log("drop on minipet");
		if(colliderName == this.gameObject.name){
			//need to check clickmanager if can respond to tap
			if(ClickManager.Instance.CanRespondToTap()){
				//do some
				Debug.Log("Minipet does some funny animation here");
			}
		}
	}

	/// <summary>
	/// Pass in the immutable data so this specific MiniPet instantiate can be instantiated
	/// with the proper information.
	/// </summary>
	/// <param name="data">ImmutableDataMiniPet.</param>
	public void Init(ImmutableDataMiniPet data){
		this.id = data.ID;
	}

	private void ItemDroppedOnTargetEventHandler(object sender, InventoryDragDrop.InvDragDropArgs args){
		if(args.TargetCollider.name == this.gameObject.name){
			string invItemID = args.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);

			//check if minipet needs food
			if(MiniPetManager.Instance.CanModifyFoodXP(id)){
				//use item if so
				args.IsValidTarget = true;
				Debug.Log("item dropped on mini pet");
				
				//notify inventory logic that this item is being used
				InventoryLogic.Instance.UseMiniPetItem(invItemID);
				MiniPetManager.Instance.IncreaseFoodXP(id);
			}

		}
		else{
			//say sth if minipet doesn't want food anymore
		}
	}

}
