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
public class AccessoryUIManager : SingletonUI<AccessoryUIManager>{
	public UIGrid grid;
	public GameObject accessoryTitlePrefab;
	public GameObject accessoryEntryPrefab;
	public GameObject backButton;
	public GameObject zoomItem;
	public EntranceHelperController entranceHelper;
	
	// related to zooming into the badge board
	public float ZoomTime;
	public Vector3 zoomOffset;
	public Vector3 zoomRotation;
	public string soundBuy;
	public string soundUnequip;
	public string soundEquip;
	private List<AccessoryEntryUIController> accessoryEntryList = new List<AccessoryEntryUIController>();
	private bool isActive = false;

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Accessory;
	}

	protected override void Start(){
		base.Start();
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

	protected override void OnDestroy(){
		base.OnDestroy();
		HUDAnimator.OnLevelUp -= RefreshAccessoryItems;
	}



	// When the zoomItem is clicked and zoomed into
	protected override void _OpenUI(){
		if(!isActive){
			// Zoom into the item
			Vector3 targetPosition = zoomItem.transform.position + zoomOffset;

			CameraManager.Callback cameraDoneFunction = delegate(){
				CameraMoveDone();
			};
			CameraManager.Instance.ZoomToTarget(targetPosition, zoomRotation, ZoomTime, cameraDoneFunction);
			
			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();

			//need to disable more things here
			PetAnimationManager.Instance.DisableIdleAnimation();
			
			isActive = true;
			zoomItem.collider.enabled = false;
			
			backButton.SetActive(true);
		}
	}

	/// <summary>
	/// Show the ui once camera is done zooming in.
	/// </summary>
	private void CameraMoveDone(){
		entranceHelper.EntranceUsed();	// Disable entrance highlight after zoomed in for the first time
		
		TweenToggleDemux toggleDemux = this.GetComponent<TweenToggleDemux>();
		toggleDemux.Show();
		toggleDemux.ShowTarget = this.gameObject;
		toggleDemux.ShowFunctionName = "MovePet";
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
			RoomArrowsUIManager.Instance.ShowPanel();
			
			backButton.SetActive(false);
		}
	}

	/// <summary>
	/// Move pet into accessory view after camera is done zooming in
	/// </summary>
	private void MovePet(){
		//teleport first then walk into view
		PetMovement.Instance.petSprite.transform.position = new Vector3(-13f, 0, 33f);
		PetMovement.Instance.MovePet(new Vector3(-17f, 0, 33f));
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
			if(StatsController.Instance.GetStat(HUDElementType.Stars) >= itemData.Cost){

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
		AccessoryManager.Instance.SetAccessoryAtNode(itemID);

		// Equip the node
		AccessoryNodeController.Instance.SetAccessory(itemID);

		RefreshAccessoryItems();
	}

	public void Unequip(string itemID){
		// Set the mutable data
		AccessoryManager.Instance.RemoveAccessoryAtNode(itemID);

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
