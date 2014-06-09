using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class InventoryUIManager : Singleton<InventoryUIManager>{
	public GameObject inventoryPanel;
	public bool isDebug;
	public UIPanel gridPanel;
	public GameObject uiGridObject;
	public GameObject uiButtonSpriteObject;
	public GameObject spritePet;
	public GameObject inventoryItemPrefab;
	public PetAnimator petAnimator;
	private bool isGuiShowing = true;   // Aux to keep track, not synced!!
	private float collapsedPos;
	private GameObject fingerHintGO;

	void Start(){
		collapsedPos = inventoryPanel.GetComponent<TweenPosition>().to.x;
		InventoryLogic.OnItemAddedToInventory += OnItemAdded;

		//Spawn items in the inventory for the first time
		List<InventoryItem> allInvItems = InventoryLogic.Instance.AllInventoryItems;
		foreach(InventoryItem invItem in allInvItems){
			// ideally, we might abstract out the inventory to be an inventory of certain things (food, usables, decos, etc)
			// but for now, I guess just don't show decorations in the inventory
			//if ( invItem.ItemType != ItemType.Decorations )
			SpawnInventoryItemInPanel(invItem);
		}
	}

	void OnDestroy(){
		InventoryLogic.OnItemAddedToInventory -= OnItemAdded;
	}

	//Event listener. listening to when item is dragged out of the inventory on drop
	//on something in the game
	private void OnItemDrop(object sender, InventoryDragDrop.InvDragDropArgs e){
		bool dropOnTarget = false;

		//delete tutorial GO if still alive
		if(fingerHintGO != null)
			Destroy(fingerHintGO);

		//some debug check
		if(isDebug){
			if(e.TargetCollider && e.TargetCollider.name == "Cube")
				dropOnTarget = true;
		}
		else{
			if(e.TargetCollider && e.TargetCollider.name == "Pet_LWF")
				dropOnTarget = true;
		}

		//logic for when item is dropped on target
		if(dropOnTarget){
			string invItemID = e.ItemTransform.name; //get id from listener args
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);

			// check to make sure the item can be used
			if(ItemLogic.Instance.CanUseItem(invItemID)){
				e.IsValidTarget = true;
				

				if(invItem != null && invItem.ItemType == ItemType.Foods)
					ShowPetReceivedFoodAnimation();		
				
				//notify inventory logic that this item is being used
				InventoryLogic.Instance.UseItem(invItemID);
				
				if(invItem != null && invItem.Amount > 0){ //Redraw count label if item not 0
					e.ParentTransform.Find("Label_Amount").GetComponent<UILabel>().text = invItem.Amount.ToString();
				}
				else{ //destroy object if it has been used up
					Destroy(e.ParentTransform.gameObject);
					UpdateBarPosition();
				}
			}
			else{
				// else the drop was valid or the item could not be used...show a message
				Hashtable hashSpeech = new Hashtable();

				if(invItem.ItemType == ItemType.Foods)
					hashSpeech.Add(PetSpeechController.Keys.MessageText, Localization.Localize("ITEM_NOT_HUNGRY"));
				else
					hashSpeech.Add(PetSpeechController.Keys.MessageText, Localization.Localize("ITEM_NO_THANKS"));

				PetSpeechController.Instance.Talk(hashSpeech);				
			}
		}
	}

	private void OnItemPress(object sender, InventoryDragDrop.InvDragDropArgs e){
		bool isTutDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_FEED_PET);

		//remove drag hint on the next time user press on any item 
		if(fingerHintGO != null)
			Destroy(fingerHintGO);

		//if user is pressing the item for the first time show hint
		if(!isTutDone){
			Vector3 hintPos = e.ParentTransform.position;
			GameObject fingerHintResource = Resources.Load("inventorySwipeTut") as GameObject;
			fingerHintGO = (GameObject)Instantiate(fingerHintResource, hintPos, Quaternion.identity);
			fingerHintGO.transform.parent = GameObject.Find("Anchor-BottomRight").transform;
			fingerHintGO.transform.localScale = new Vector3(1, 1, 1);

			// fingerHintGO.transform.position = hintPos; 
			DataManager.Instance.GameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_FEED_PET);
		}
	}

	//play chew animation from pet animator
	private void ShowPetReceivedFoodAnimation(){
		if(!petAnimator.IsBusy()){
			petAnimator.PlayUnrestrictedAnim("Eat", true);
			PetMovement.Instance.StopMoving(false);
		}
	}

	//Event listener. listening to when new item is added to the inventory
	private void OnItemAdded(object sender, InventoryLogic.InventoryEventArgs e){
		
		// inventory doesn't currently care about decorations
		if(e.InvItem.ItemType == ItemType.Decorations)
			return;
		
		if(e.IsItemNew){
			SpawnInventoryItemInPanel(e.InvItem);
		}
		else{
			Transform invItem = uiGridObject.transform.Find(e.InvItem.ItemID);
			invItem.Find("Label_Amount").GetComponent<UILabel>().text = e.InvItem.Amount.ToString();
		}
	}

	//Create the NGUI object and populate the fields with InventoryItem data
	private void SpawnInventoryItemInPanel(InventoryItem invItem){
		//Create inventory item
		GameObject inventoryItemObject = NGUITools.AddChild(uiGridObject, inventoryItemPrefab);

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
		invDragDrop.OnItemDrop += OnItemDrop;
		invDragDrop.OnItemPress += OnItemPress;

		UpdateBarPosition();
	}

	public void UpdateBarPosition(){
        

		int allInventoryItemsCount = InventoryLogic.Instance.AllInventoryItems.Count;
		
		// Adjust the bar length based on how many items we want showing at all times
		if(allInventoryItemsCount <= Constants.GetConstant<int>("HudSettings_MaxInventoryDisplay")){
			inventoryPanel.GetComponent<TweenPosition>().from.x = collapsedPos - allInventoryItemsCount * 90;
			
			// Update position of the bar if inventory is open
			if(isGuiShowing){
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeOutBounce);
				LeanTween.moveLocalX(inventoryPanel, collapsedPos - allInventoryItemsCount * 90, 0.4f, optional);
			}
		}
		
		uiGridObject.GetComponent<UIGrid>().Reposition();
		
		// Reset the gridPanel again, dont want trailing white spaces in the end of scrolled down there already
		Vector3 oldPanelPos = gridPanel.transform.localPosition;
		gridPanel.transform.localPosition = new Vector3(363f, oldPanelPos.y, oldPanelPos.z);
		Vector4 oldClipRange = gridPanel.clipRange;
		gridPanel.clipRange = new Vector4(206f, oldClipRange.y, oldClipRange.z, oldClipRange.w);
	}

	//Find the position of Inventory Item game object with invItemID
	//Used for animation position in StoreUIManager
	public Vector3 GetPositionOfInvItem(string invItemID){
		// position to use
		Vector3 invItemPosition;
			
		if(!isGuiShowing){
			// if the inventory is minimized, use the position of the inventory sprite object
			invItemPosition = uiButtonSpriteObject.transform.position;
		}
		else{
			// otherwise use the position of the item in the inventory panel
			Transform invItemTrans = uiGridObject.transform.Find(invItemID);
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);
			invItemPosition = invItemTrans.position;
	
			//Offset position if the item is just added to the inventory
			if(invItem.Amount == 1)
				invItemPosition += new Vector3(-0.22f, 0, 0);
		}
		
		return invItemPosition;
	}

	// Image button clicked receiver
	public void ExpandToggled(){
		// Local aux to keep track of toggles
		// NOTE: Not synced with UIButtonTween->PlayDirection:Toggle!
		isGuiShowing = !isGuiShowing;
	}

	public void ShowPanel(){
		inventoryPanel.GetComponent<TweenToggle>().Show();
	}

	public void HidePanel(){
		inventoryPanel.GetComponent<TweenToggle>().HideWithUpdatedPosition();
	}
}
