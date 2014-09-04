﻿using UnityEngine;
using System.Collections;

public enum AccessoryButtonType{
	UnboughtLocked,		// Not yet bought, use BuyButton but locked
	Unbought,			// Not yet bought, use BuyButton
	BoughtEquipped,		// Bought and equipped, use UnequipButton
	BoughtUnequipped	// Bought and not equipped, use EquipButton
}

/// <summary>
/// Store item entry.
/// </summary>
public class AccessoryEntryUIController : MonoBehaviour{

	private string itemID;
	public string ItemID{
		get{
			return itemID;
		}
		set{
			itemID = value;
		}
	}
	private AccessoryButtonType buttonState;

	// various elements on the entry
	public UILabel labelName;
	public UILabel labelCost;
	public UISprite spriteIcon;

	public UISprite buyButtonIcon;
	public UIButtonMessage buyButtonMessage;
	public UIButtonMessage equipButtonMessage;
	public UIButtonMessage unequipButtonMessage;
	
	/// <summary>
	/// Creates the entry.
	/// </summary>
	/// <param name="goGrid">grid to add game object to.</param>
	/// <param name="goPrefab">prefab to instantiate.</param>
	/// <param name="item">Item.</param>
	public static GameObject CreateEntry(GameObject goGrid, GameObject goPrefab, 
	                                     Item item, GameObject buyButtonMessageTarget = null,
	                                     string buyButtonMessageFunctionName = ""){
		
		GameObject itemUIObject = NGUITools.AddChild(goGrid, goPrefab);
		
		//set default buy button message target/function name if they are null
		if(buyButtonMessageTarget == null || string.IsNullOrEmpty(buyButtonMessageFunctionName)){
			buyButtonMessageTarget = StoreUIManager.Instance.gameObject;
			buyButtonMessageFunctionName = "OnBuyButton";
		}
		
		itemUIObject.GetComponent<StoreItemEntryUIController>().Init(item, 
		                                                             buyButtonMessageTarget,
		                                                             buyButtonMessageFunctionName);
		
		return itemUIObject;
	}
	
	/// <summary>
	/// Init the specified itemData.
	/// This function does the work and actually sets the
	/// UI labels, sprites, etc for this entry based on
	/// the incoming item data.
	/// </summary>
	/// <param name="itemData">Item data.</param>
	public void Init(Item itemData,
	                 GameObject buyButtonMessageTarget, string buyButtonMessageFunctionName,
	                 GameObject equipButtonMessageTarget, string equipButtonMessageFunctionName,
	                 GameObject unequipButtonMessageTarget, string unqeuipButtonMessageFunctionName){
		// set the proper values on the entry
		gameObject.name = itemData.ID;
		
		string costText = itemData.Cost.ToString();
		if(itemData.Type == ItemType.Premiums)
			costText = "$" + costText;
		
		if(itemData.CurrencyType == CurrencyTypes.Gem)
			buyButtonIcon.spriteName = "iconGem";
		
		labelCost.text = costText;
		
		labelName.text = itemData.Name;
		spriteIcon.spriteName = itemData.TextureName;
		buyButtonMessage.target = buyButtonMessageTarget;
		buyButtonMessage.functionName = buyButtonMessageFunctionName;
		
		//Check if accessory has already been bought. Change the button if so
		if(itemData.Type == ItemType.Accessories){
			AccessoryItem accessoryItem = (AccessoryItem)itemData;
			bool isAccessoryBought = InventoryLogic.Instance.CheckForAccessory(accessoryItem.ID);
			if(isAccessoryBought){

				//TODO check to see if it is equipped here
				if(true){
					// Bought and equipped
					SetState(AccessoryButtonType.BoughtEquipped);
				}
				else{
					// Bought but not equipped
					SetState(AccessoryButtonType.BoughtUnequipped);
				}
			}
		}
		
		// if this item is currently locked...
		if(itemData.IsLocked()){
			// show the UI
			LevelLockObject.CreateLock(spriteIcon.gameObject.transform.parent.gameObject, itemData.UnlockAtLevel);
			
			// Hide all the buttons
			SetState(AccessoryButtonType.UnboughtLocked);
		}
	}

	//TODO need event listener for level up and unlock items



	//

	public void SetState(AccessoryButtonType buttonType){
		switch(buttonType){
		case AccessoryButtonType.UnboughtLocked:
			buyButtonMessage.gameObject.SetActive(false);
			unequipButtonMessage.gameObject.SetActive(false);
			equipButtonMessage.gameObject.SetActive(false);
			break;
		case AccessoryButtonType.Unbought:
			buyButtonMessage.gameObject.SetActive(true);
			unequipButtonMessage.gameObject.SetActive(false);
			equipButtonMessage.gameObject.SetActive(false);
			break;
		case AccessoryButtonType.BoughtEquipped:
			buyButtonMessage.gameObject.SetActive(false);
			unequipButtonMessage.gameObject.SetActive(true);
			equipButtonMessage.gameObject.SetActive(false);
			break;
		case AccessoryButtonType.BoughtUnequipped:
			buyButtonMessage.gameObject.SetActive(false);
			unequipButtonMessage.gameObject.SetActive(false);
			equipButtonMessage.gameObject.SetActive(true);
			break;
		default:
			Debug.LogError("Invalid state for button type");
			break;
		}

		buttonState = buttonType;
	}
}
