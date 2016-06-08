using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Consumable Inventory UI manager
/// InventoryManager 	- DecoInventoryUIManager	(two separate inventory managers!)
/// 					\ InventoryUIManager
/// </summary>
public class InventoryUIManager : Singleton<InventoryUIManager>{
	public static EventHandler<InventoryDragDrop.InvDragDropArgs> ItemDroppedOnTargetEvent;

	public GameObject inventoryPanel;
	public bool isDebug;
//	public UIPanel gridPanel;
	public RectTransform gridTransform;
	public GameObject spritePet;
	public GameObject inventoryItemPrefab;
	public Transform itemFlyToTransform;

	private int maxInventoryDisplay = 6;
	private float collapsedPos = -164f;
	private Transform currentDragDropItem;

	void Start(){
		InventoryManager.OnItemAddedToInventory += OnItemAddedHandler;
		InventoryManager.OnItemUsed += OnItemUsedHandler;

		//Spawn items in the inventory for the first time
		List<InventoryItem> allInvItems = InventoryManager.Instance.AllUsableInventoryItems;
		foreach(InventoryItem invItem in allInvItems){
			// ideally, we might abstract out the inventory to be an inventory of certain things (food, usables, decos, etc)
			// but for now, I guess just don't show decorations in the inventory
			//if ( invItem.ItemType != ItemType.Decorations )
			SpawnInventoryItemInPanel(invItem, isOnLoad : true);
		}
	}

	void OnDestroy(){
		InventoryManager.OnItemAddedToInventory -= OnItemAddedHandler;
		InventoryManager.OnItemUsed -= OnItemUsedHandler;
	}

	// If items in inventory greater than max count, it scrollable
	public bool IsInventoryScrollable(){
		return InventoryManager.Instance.AllUsableInventoryItems.Count > maxInventoryDisplay;
	}

	public Vector3 GetItemFlyToPosition(){
		return itemFlyToTransform.position;
	}

	/// <summary>
	/// Gets the fire orb reference.
	/// Use for the tutorial to get the fire orb gameobject.
	/// </summary>
	/// <returns>The fire orb reference.</returns>
	public GameObject GetFireOrbReference(){
		GameObject retVal = null;
		foreach(Transform item in gridTransform) {
			if(item.name == "Usable1"){
				Transform trans = item.Find("Usable1");
				if(trans != null){
					retVal = trans.gameObject;
				}
			}
		}

		// Check if user is dragging it as well...
		if(retVal == null){
			GameObject dragPanel = GameObject.Find("ItemDragPanel");
			if(dragPanel != null){
				Transform trans = dragPanel.transform.Find("Usable1");
				if(trans != null){
					retVal = trans.gameObject;
				}
			}
		}
		return retVal;
	}

	/// <param name="isOnLoad">If set to <c>true</c> does tweening instantly, used for loading into scene check only</param>
	public void UpdateBarPosition(bool isOnLoad = false){
		int allInventoryItemsCount = InventoryManager.Instance.AllUsableInventoryItems.Count;
		// Normal case where you add item during game
		if(!isOnLoad){
			// Adjust the bar length based on how many items we want showing at all times
			if(allInventoryItemsCount <= maxInventoryDisplay){

				// Update position of the bar if inventory is open
				LeanTween.moveLocalX(inventoryPanel, collapsedPos - allInventoryItemsCount * 90, 0.4f)
					.setEase(LeanTweenType.easeOutBounce);
			}
		}
		// Scene loading case, dont want to tween here so set them explicitly
		else{
			// Adjust the bar length based on how many items we want showing at all times
			if(allInventoryItemsCount > maxInventoryDisplay) {
				allInventoryItemsCount = maxInventoryDisplay;
			}
			
			if(inventoryPanel.transform.localPosition.x != collapsedPos - allInventoryItemsCount * 90){
				inventoryPanel.transform.localPosition = new Vector3(collapsedPos - allInventoryItemsCount * 90,
				                                                     inventoryPanel.transform.localPosition.y,
				                                                     inventoryPanel.transform.localPosition.z);
			}
		}
				
		// Reset the gridPanel again, dont want trailing white spaces in the end of scrolled down there already
		//Vector3 oldPanelPos = gridPanel.transform.localPosition;
		//gridPanel.transform.localPosition = new Vector3(361f, oldPanelPos.y, oldPanelPos.z);	// TODO CHANGE THIS WHEN CHANGING CLIPPING
	}
	
	//Find the position of Inventory Item game object with invItemID
	//Used for animation position in StoreUIManager
	public Vector3 GetPositionOfInvItem(string invItemID){
		// position to use
		Vector3 invItemPosition;

		// Use the position of the item in the inventory panel
		Transform invItemTrans = gridTransform.Find(invItemID);
		InventoryItem invItem = InventoryManager.Instance.GetItemInInventory(invItemID);
		invItemPosition = invItemTrans.position;
		
		//Offset position if the item is just added to the inventory
		if(invItem.Amount == 1){
			invItemPosition += new Vector3(-0.22f, 0, 0);
		}
		
		return invItemPosition;
	}
	
	public void ShowPanel(){
		inventoryPanel.GetComponent<TweenToggle>().Show();
	}
	
	public void HidePanel(){
		inventoryPanel.GetComponent<TweenToggle>().HideWithUpdatedPosition();
	}

	private void OnItemDropHandler(object sender, InventoryDragDrop.InvDragDropArgs e){
		if(e.TargetCollider && e.TargetCollider.tag == "ItemTarget"){
			currentDragDropItem = e.ParentTransform;

			if(ItemDroppedOnTargetEvent != null){
				ItemDroppedOnTargetEvent(this, e);
			}
		}
	}

	private void OnItemUsedHandler(object sender, InventoryManager.InventoryEventArgs args){
		if(currentDragDropItem != null){
			InventoryItem invItem = args.InvItem;
			if(invItem != null && invItem.Amount > 0){ //Redraw count label if item not 0
				currentDragDropItem.Find("Label_Amount").GetComponent<UILabel>().text = invItem.Amount.ToString();
			}
			else{ //destroy object if it has been used up
				Destroy(currentDragDropItem.gameObject);
				UpdateBarPosition();
			}
		}
	}

	/// <summary>
	/// Handles the item press event.
	/// </summary>
	private void OnItemPress(object sender, InventoryDragDrop.InvDragDropArgs e){
//		bool isTutDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_FEED_PET);
//
//		//remove drag hint on the next time user press on any item 
//		if(fingerHintGO != null)
//			Destroy(fingerHintGO);
//
//		//if user is pressing the item for the first time show hint
//		if(!isTutDone){
//			Vector3 hintPos = e.ParentTransform.position;
//			GameObject fingerHintResource = Resources.Load("inventorySwipeTut") as GameObject;
//			fingerHintGO = (GameObject)Instantiate(fingerHintResource, hintPos, Quaternion.identity);
//			fingerHintGO.transform.parent = GameObject.Find("Anchor-BottomRight").transform;
//			fingerHintGO.transform.localScale = new Vector3(1, 1, 1);
//
//			// fingerHintGO.transform.position = hintPos; 
//			DataManager.Instance.GameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_FEED_PET);
//		}
	}

	// When new item is added to the inventory
	private void OnItemAddedHandler(object sender, InventoryManager.InventoryEventArgs args){
		// inventory doesn't currently care about decorations/accessories
		if(args.InvItem.ItemType == ItemType.Decorations || args.InvItem.ItemType == ItemType.Accessories) {
			return;
		}
		if(args.IsItemNew){
			SpawnInventoryItemInPanel(args.InvItem);
		}
		else{
			Transform invItem = gridTransform.Find(args.InvItem.ItemID);
			invItem.Find("Label_Amount").GetComponent<UILabel>().text = args.InvItem.Amount.ToString();
		}
	}

	//Create the NGUI object and populate the fields with InventoryItem data
	private void SpawnInventoryItemInPanel(InventoryItem invItem, bool isOnLoad = false){
		//Create inventory item
		GameObject inventoryItemObject = NGUITools.AddChild(gridTransform.gameObject, inventoryItemPrefab);

		//get reference to all the GO and scripts
		Transform itemWrapper = inventoryItemObject.transform.Find("Icon");
		UISprite itemSprite = inventoryItemObject.transform.Find("Icon/Sprite_Image").GetComponent<UISprite>();
		UILabel itemAmountLabel = inventoryItemObject.transform.Find("Label_Amount").GetComponent<UILabel>();
		InventoryItemStatsHintController statsHint = itemWrapper.GetComponent<InventoryItemStatsHintController>();
		InventoryDragDrop invDragDrop = itemWrapper.GetComponent<InventoryDragDrop>();

		//Set value to UI element
		itemWrapper.name = invItem.ItemID;
		inventoryItemObject.name = invItem.ItemID;
		itemSprite.spriteName = invItem.ItemTextureName;
		itemAmountLabel.text = invItem.Amount.ToString();

		//Create stats hint
		statsHint.PopulateStatsHints((StatsItem)invItem.ItemData);

		//Listen to on press and on drop
		invDragDrop.OnItemDrag += statsHint.OnItemDrag;
		invDragDrop.OnItemDrop += statsHint.OnItemDrop;

		//listen to on drop event
		invDragDrop.OnItemDrop += OnItemDropHandler;
		invDragDrop.OnItemPress += OnItemPress;

		UpdateBarPosition(isOnLoad);
	}
}
