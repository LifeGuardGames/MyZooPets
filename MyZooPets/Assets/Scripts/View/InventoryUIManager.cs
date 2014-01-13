using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class InventoryUIManager : Singleton<InventoryUIManager> {
	public GameObject inventoryPanel;
    public bool isDebug;
    public GameObject uiGridObject;
    public GameObject uiButtonToggleObject;
    public GameObject uiButtonSpriteObject;
    public GameObject spritePet;
    public GameObject inventoryItemPrefab;
    public PetAnimator petAnimator;
	
    private bool isGuiShowing = true;   // Aux to keep track, not synced!!
    private float collapsedPos;
    private UIButtonToggle uiButtonToggle;
    
    void Awake(){
        uiButtonToggle = uiButtonToggleObject.GetComponent<UIButtonToggle>();
    }

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
        if(isDebug){
            if(e.TargetCollider && e.TargetCollider.name == "Cube") dropOnTarget = true;
        }
        else{
            if(e.TargetCollider && e.TargetCollider.name == "Pet_LWF")
                dropOnTarget = true;
        }

        if(dropOnTarget){
            e.IsValidTarget = true;

            string invItemID = e.ItemTransform.name; //get id from listener args
			
			InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);
			if ( invItem != null && invItem.ItemType == ItemType.Foods )
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
		if ( e.InvItem.ItemType == ItemType.Decorations )
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
        GameObject inventoryItemObject = NGUITools.AddChild(uiGridObject, inventoryItemPrefab);
        Transform itemWrapper = inventoryItemObject.transform.Find("Icon");
        UISprite itemSprite = inventoryItemObject.transform.Find("Icon/Sprite_Image").GetComponent<UISprite>();
        UILabel itemAmountLabel = inventoryItemObject.transform.Find("Label_Amount").GetComponent<UILabel>();

        itemWrapper.name = invItem.ItemID;
        inventoryItemObject.name = invItem.ItemID;
        // print(invItem.ItemTextureName);
        itemSprite.spriteName = invItem.ItemTextureName;
        itemAmountLabel.text = invItem.Amount.ToString();
        itemWrapper.GetComponent<InventoryDragDrop>().OnItemDrop += OnItemDrop;

        UpdateBarPosition();
    }

    public void UpdateBarPosition(){
        uiGridObject.GetComponent<UIGrid>().Reposition();

        if(inventoryPanel.GetComponent<TweenPosition>().from.x > -1064){  // Limit Move after x items     // TODO make const
            int allInventoryItemsCount = InventoryLogic.Instance.AllInventoryItems.Count;
            inventoryPanel.GetComponent<TweenPosition>().from.x = collapsedPos - allInventoryItemsCount * 90;

            if(uiButtonToggle.isActive){    // Animate the move if inventory is open
                Hashtable optional = new Hashtable();

                optional.Add("ease", LeanTweenType.easeOutBounce);
                LeanTween.moveLocalX(inventoryPanel, collapsedPos - allInventoryItemsCount * 90, 0.4f, optional);
            }
        }
    }

    //Find the position of Inventory Item game object with invItemID
    //Used for animation position in StoreUIManager
    public Vector3 GetPositionOfInvItem(string invItemID){
		// position to use
		Vector3 invItemPosition;
			
		if ( !isGuiShowing ) {
			// if the inventory is minimized, use the position of the inventory sprite object
			invItemPosition = uiButtonSpriteObject.transform.position;
		}
		else {
			// otherwise use the position of the item in the inventory panel
	        Transform invItemTrans = uiGridObject.transform.Find(invItemID);
	        InventoryItem invItem = InventoryLogic.Instance.GetInvItem(invItemID);
	        invItemPosition = invItemTrans.position;
	
	        //Offset position if the item is just added to the inventory
	        if(invItem.Amount == 1) invItemPosition += new Vector3(-0.22f, 0, 0);
		}
		
        return invItemPosition;
    }

    // Image button clicked receiver
    public void ExpandToggled(){
        // Local aux to keep track of toggles
        if(InventoryLogic.Instance.AllInventoryItems.Count > 0)
            isGuiShowing = !isGuiShowing;
            
		// Switch images based on item box status implement here
		//uiSprite.spriteName = isGuiShowing ? inventoryOpenSpriteName : inventoryCloseSpriteName;
    }

	public void ShowPanel(){
		inventoryPanel.GetComponent<TweenToggle>().Show();
	}

	public void HidePanel(){
		inventoryPanel.GetComponent<TweenToggle>().HideWithUpdatedPosition();
	}
}
