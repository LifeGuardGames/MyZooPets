﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Accessory user interface manager.
/// 
/// ARCHITECTURE:
/// 
/// Controlling "store" buttons and entries:
/// 	AccessoryUIManager -> AccessoryEntryUIController
/// 
/// Controlling equipping accessory on pet:
/// 	AccessoryUIManager -> AccessoryNodeController -> AccessoryNode
/// 
/// </summary>
public class AccessoryUIManager : SingletonUI<AccessoryUIManager> {
	public UIGrid grid;
	public GameObject accessoryTitlePrefab;
	public GameObject accessoryEntryPrefab;
	public GameObject backButton;
	public GameObject zoomItem;
	private bool isActive = false;
	
	// related to zooming into the badge board
	public float ZoomTime;
	public Vector3 zoomOffset;
	public Vector3 zoomRotation;

	public string soundBuy;
	public string soundUnequip;
	public string soundEquip;

	private List<AccessoryEntryUIController> accessoryEntryList = new List<AccessoryEntryUIController>();

	void Awake(){
		eModeType = UIModeTypes.Accessory;
	}

	void Start(){
		HUDAnimator.OnLevelUp += RefreshAccessoryItems; //listen to level up so we can unlock items

		// Populate the entries with loaded data
		List<Item> accessoryList = ItemLogic.Instance.AccessoryList;
		AccessoryTypes lastCategory = AccessoryTypes.Hat;
		bool isFirstTitle = true;
		foreach(AccessoryItem accessory in accessoryList){
			// Create a new accessory type label if lastCategory has changed
			if(lastCategory != accessory.AccessoryType || isFirstTitle){
				isFirstTitle = false;
				GameObject itemUIObject = LgNGUITools.AddChildWithPositionAndScale(grid.gameObject, accessoryTitlePrefab);
				UILocalize localize = itemUIObject.GetComponent<UILocalize>();

				switch((AccessoryTypes)accessory.AccessoryType){
				case AccessoryTypes.Hat:
					localize.key = "ACCESSORIES_TYPE_HAT";
					break;
				case AccessoryTypes.Glasses:
					localize.key = "ACCESSORIES_TYPE_GLASSES";
					break;
				case AccessoryTypes.Color:
					localize.key = "ACCESSORIES_TYPE_COLOR";
					break;
				default:
					Debug.LogError("Invalid accessory type");
					break;
				}
				localize.Localize();	// Force relocalize
				lastCategory = accessory.AccessoryType;
			}

			GameObject entry = AccessoryEntryUIController.CreateEntry(grid.gameObject, accessoryEntryPrefab, accessory);
			accessoryEntryList.Add(entry.GetComponent<AccessoryEntryUIController>());
		}
	}

	void OnDestroy(){
		HUDAnimator.OnLevelUp -= RefreshAccessoryItems;
	}

	/// <summary>
	/// Gets the type of the accessory node.
	/// Given a accessory(ID), check the xml to see which type it is.
	/// </summary>
	/// <returns>The accessory node type.</returns>
	/// <param name="accessoryID">Accessory ID.</param>
	public static string GetAccessoryNodeType(string accessoryID){
		AccessoryItem itemDeco = (AccessoryItem)ItemLogic.Instance.GetItem(accessoryID);
		return itemDeco.AccessoryType.ToString();
	}

	// When the zoomItem is clicked and zoomed into
	protected override void _OpenUI(){
		if(!isActive){
			// Zoom into the item
			Vector3 targetPosition = zoomItem.transform.position + zoomOffset;
			CameraManager.Instance.ZoomToTarget(targetPosition, zoomRotation, ZoomTime, this.gameObject);
			
			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
			RoomArrowsUIManager.Instance.HidePanel();

			//need to disable more things here
			PetAnimationManager.Instance.DisableIdleAnimation();
			
			isActive = true;
			zoomItem.collider.enabled = false;
			
			backButton.SetActive(true);
		}
	}

	/// <summary>
	/// show the ui once camera is done zooming in.
	/// </summary>
	private void CameraMoveDone(){

		TweenToggleDemux toggleDemux = this.GetComponent<TweenToggleDemux>();
		toggleDemux.Show();
		toggleDemux.ShowTarget = this.gameObject;
		toggleDemux.ShowFunctionName = "MovePet";
	}

	/// <summary>
	/// Move pet into accessory view after camera is done zooming in
	/// </summary>
	private void MovePet(){
		//teleport first then walk into view
		PetMovement.Instance.petSprite.transform.localPosition = new Vector3(-12f, 0, 33f);
		PetMovement.Instance.MovePet(new Vector3(-17f, 0, 33f));
	}
	
	// The back button on the left top corner is clicked to zoom out of the zoom item
	protected override void _CloseUI(){
		if(isActive){
			this.GetComponent<TweenToggleDemux>().Hide();

			isActive = false;
			zoomItem.collider.enabled = true;
			
			CameraManager.Instance.ZoomOutMove();
			PetAnimationManager.Instance.EnableIdleAnimation();
			
			// Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			EditDecosUIManager.Instance.ShowNavButton();
			RoomArrowsUIManager.Instance.ShowPanel();
			
			backButton.SetActive(false);
		}
	}

	/// <summary>
	/// Raises the buy button event.
	/// </summary>
	/// <param name="button">Button.</param>
	public void OnBuyButton(GameObject button){
		Transform buttonParent = button.transform.parent;
		string itemID = buttonParent.name;
		Item itemData = ItemLogic.Instance.GetItem(itemID);
		
		switch(itemData.CurrencyType){
		case CurrencyTypes.WellaCoin:
			if(DataManager.Instance.GameData.Stats.Stars >= itemData.Cost){

				//Disable the buy button so user can't buy the same wallpaper anymore 
				UIImageButton buyButton = button.GetComponent<UIImageButton>();
				buyButton.isEnabled = false;
				
				InventoryLogic.Instance.AddItem(itemID, 1);
				StatsController.Instance.ChangeStats(deltaStars: (int)itemData.Cost * -1);

				// Change the state of the button
				button.transform.parent.gameObject.GetComponent<AccessoryEntryUIController>().SetState(AccessoryButtonType.BoughtEquipped);

				// Equip item
				Equip(itemID);

				//Analytics
				Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_BOUGHT, itemData.Type, itemData.ID);
				
				// play a sound since an item was bought
				AudioManager.Instance.PlayClip(soundBuy);
			}
			else{
				AudioManager.Instance.PlayClip("buttonDontClick");
			}
			break;
		case CurrencyTypes.Gem:
			if(DataManager.Instance.GameData.Stats.Gems >= itemData.Cost){
				
				//Disable the buy button so user can't buy the same wallpaper anymore 
				UIImageButton buyButton = button.GetComponent<UIImageButton>();
				buyButton.isEnabled = false;
				
				InventoryLogic.Instance.AddItem(itemID, 1);
				StatsController.Instance.ChangeStats(deltaStars: (int)itemData.Cost * -1);
				
				// Change the state of the button
				button.GetComponent<AccessoryEntryUIController>().SetState(AccessoryButtonType.BoughtEquipped);

				// Equip item
				Equip(itemID);

				//Analytics
				Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_BOUGHT, itemData.Type, itemData.ID);
				
				// play a sound since an item was bought
				AudioManager.Instance.PlayClip(soundBuy);
			}
			else{
				AudioManager.Instance.PlayClip("buttonDontClick");
			}
			break;
		}
	}

	public void OnEquipButton(GameObject button){
		Transform buttonParent = button.transform.parent;
		string itemID = buttonParent.name;

		Equip(itemID);

		AudioManager.Instance.PlayClip(soundEquip);
	}

	public void OnUnequipButton(GameObject button){
		Transform buttonParent = button.transform.parent;
		string itemID = buttonParent.name;

		Unequip(itemID);

		AudioManager.Instance.PlayClip(soundUnequip);
	}

	public void Equip(string itemID){

		// Unequip anything first
		Unequip(itemID);

		// Set the mutable data
		DataManager.Instance.GameData.Accessories.PlacedAccessories.Add(GetAccessoryNodeType(itemID), itemID);

		// Equip the node
		AccessoryNodeController.Instance.SetAccessory(itemID);

		RefreshAccessoryItems();
	}

	public void Unequip(string itemID){
		// Set the mutable data
		DataManager.Instance.GameData.Accessories.PlacedAccessories.Remove(GetAccessoryNodeType(itemID));

		// Unequip the node
		AccessoryNodeController.Instance.RemoveAccessory(itemID);	// Still need item ID to know which node to remove

		RefreshAccessoryItems();
	}

	//Check for any UI updateds
	private void RefreshAccessoryItems(){
		foreach(AccessoryEntryUIController entryController in accessoryEntryList){
			entryController.CheckState();
		}
	}

	private void RefreshAccessoryItems(object sender, EventArgs args){
		RefreshAccessoryItems();
	}
}
