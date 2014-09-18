using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Decoration user interface manager.
/// This class includes the Decoration Inventory as well as UI for decoration.
/// 
/// 
/// InventoryLogic 	- DecoInventoryUIManager	(two separate inventory managers!)
/// 				\ InventoryUIManager
/// 
/// </summary>
public class DecoInventoryUIManager : SingletonUI<DecoInventoryUIManager> {
	public static EventHandler<InventoryDragDrop.InvDragDropArgs> OnDecoDroppedOnTarget;

	public static EventHandler<EventArgs> OnDecoPickedUp;   // when a decoration is picked up
	public static EventHandler<EventArgs> OnDecoDropped;   // when a decoration is dropped

	private bool isActive = false;

	public GameObject backButton;

	public GameObject decorationGridPanel;
	public UIPanel gridPanel;
	public GameObject uiGridObject;
	public GameObject decorationItemPrefab;

	private float collapsedPos = -164f;

	public Transform currentDragDropItem;

	void Awake(){
		eModeType = UIModeTypes.EditDecos;
	}

	void Start(){
		InventoryLogic.onItemAddedToDecoInventory += OnItemAddedHandler;
//		InventoryLogic.OnItemUsed += OnItemUsedHandler;

		// Spawn items in the decoration inventory for the first time
		List<InventoryItem> listDecos = InventoryLogic.Instance.AllDecoInventoryItems;
		foreach(InventoryItem invItem in listDecos){
			// Setting isOnLoad option to true for first time loading
			SpawnInventoryItemInPanel(invItem, isOnLoad:true);
		}
	}

	void OnDestroy(){
		InventoryLogic.onItemAddedToDecoInventory -= OnItemAddedHandler;
	}

	//Event listener. listening to when new item is added to the deco inventory
	private void OnItemAddedHandler(object sender, InventoryLogic.InventoryEventArgs e){
		if(e.IsItemNew){
			SpawnInventoryItemInPanel(e.InvItem);
		}
		else{
			Transform invItem = uiGridObject.transform.Find(e.InvItem.ItemID);
			invItem.Find("Label_Amount").GetComponent<UILabel>().text = e.InvItem.Amount.ToString();
		}
	}

	//Create the NGUI object and populate the fields with InventoryItem data
	private void SpawnInventoryItemInPanel(InventoryItem invItem, bool isOnLoad = false){
		//Create inventory item
		GameObject decoInventoryItemObject = NGUITools.AddChild(uiGridObject, decorationItemPrefab);
		
		//get reference to all the GO and scripts
		Transform itemWrapper = decoInventoryItemObject.transform.Find("Icon");
		UISprite itemSprite = decoInventoryItemObject.transform.Find("Icon/Sprite_Image").GetComponent<UISprite>();
		UILabel itemAmountLabel = decoInventoryItemObject.transform.Find("Label_Amount").GetComponent<UILabel>();
		InventoryDragDrop invDragDrop = itemWrapper.GetComponent<InventoryDragDrop>();
		
		//Set value to UI element
		itemWrapper.name = invItem.ItemID;
		decoInventoryItemObject.name = invItem.ItemID;
		itemSprite.spriteName = invItem.ItemTextureName;
		itemAmountLabel.text = invItem.Amount.ToString();
		
//		//Listen to on press and on drop
//		invDragDrop.OnItemDrag += statsHint.OnItemDrag;
//		invDragDrop.OnItemDrop += statsHint.OnItemDrop;
//		
//		//listen to on drop event
		invDragDrop.OnItemDrop += OnItemDrop;
		invDragDrop.OnItemDrag += OnItemDrag;
//		invDragDrop.OnItemPress += OnItemPress;
		
		UpdateBarPosition(isOnLoad);
	}

	/// <summary>
	/// Updates the bar position.
	/// </summary>
	/// <param name="isOnLoad">If set to <c>true</c> does tweening instantly, used for loading into scene check only</param>
	public void UpdateBarPosition(bool isOnLoad = false){
		int allDecoInventoryItemsCount = InventoryLogic.Instance.AllDecoInventoryItems.Count;
		
		// Normal case where you add item during game
		if(!isOnLoad){
			// Adjust the bar length based on how many items we want showing at all times
			if(allDecoInventoryItemsCount <= Constants.GetConstant<int>("HudSettings_MaxInventoryDisplay")){
				
				// Update position of the bar if inventory is open
				Hashtable optional = new Hashtable();
				optional.Add("ease", LeanTweenType.easeOutBounce);
				LeanTween.moveLocalX(decorationGridPanel, collapsedPos - allDecoInventoryItemsCount * 90, 0.4f, optional);
			}
		}
		// Scene loading case, dont want to tween here so set them explicitly
		else{
			// Adjust the bar length based on how many items we want showing at all times
			if(allDecoInventoryItemsCount > Constants.GetConstant<int>("HudSettings_MaxInventoryDisplay")){
				allDecoInventoryItemsCount = Constants.GetConstant<int>("HudSettings_MaxInventoryDisplay");
			}
			
			if(decorationGridPanel.transform.localPosition.x != collapsedPos - allDecoInventoryItemsCount * 90){
				decorationGridPanel.transform.localPosition = new Vector3(collapsedPos - allDecoInventoryItemsCount * 90,
				                                                          decorationGridPanel.transform.localPosition.y,
				                                                          decorationGridPanel.transform.localPosition.z);
			}
		}
		
		uiGridObject.GetComponent<UIGrid>().Reposition();
		
		// Reset the gridPanel again, dont want trailing white spaces in the end of scrolled down there already
		Vector3 oldPanelPos = gridPanel.transform.localPosition;
		gridPanel.transform.localPosition = new Vector3(361f, oldPanelPos.y, oldPanelPos.z);	// TODO CHANGE THIS WHEN CHANGING CLIPPING
		Vector4 oldClipRange = gridPanel.clipRange;
		gridPanel.clipRange = new Vector4(116f, oldClipRange.y, oldClipRange.z, oldClipRange.w);	//TODO CHANGE THIS WHEN CHANGING CLIPPING
	}

	//Find the position of Decoration Item game object with invItemID
	//Used for animation position in StoreUIManager
	public Vector3 GetPositionOfDecoInvItem(string itemID){
		// position to use
		Vector3 decoInvItemPosition;
		
		// Use the position of the item in the inventory panel
		Transform deocInvItemTrans = uiGridObject.transform.Find(itemID);
		InventoryItem decoInvItem = InventoryLogic.Instance.GetDecoInvItem(itemID);
		decoInvItemPosition = deocInvItemTrans.position;
		
		//Offset position if the item is just added to the inventory
		if(decoInvItem.Amount == 1)
			decoInvItemPosition += new Vector3(-0.22f, 0, 0);
		
		return decoInvItemPosition;
	}

	//Event listener. listening to when item is dragged out of the deco inventory on drop
	//on something in the game
	private void OnItemDrop(object sender, InventoryDragDrop.InvDragDropArgs e){
//		Debug.Log("ON ITEM DROP");

//		bool dropOnTarget = false;
		//delete tutorial GO if still alive
//		if(fingerHintGO != null)
//			Destroy(fingerHintGO);
//		Debug.Log(e.ItemTransform.gameObject); //TODO
		if(e.TargetCollider && e.TargetCollider.tag == "DecoItemTarget"){
			currentDragDropItem = e.ParentTransform;
			
			if(OnDecoDroppedOnTarget != null)
				OnDecoDroppedOnTarget(this, e);
		}

		// Regardless of drop, reset the node state
		if(OnDecoDropped != null){
			OnDecoDropped(this, e);
		}

		currentDragDropItem = null;
	}

	private void OnItemDrag(object sender, EventArgs e){
		GameObject go = ((InventoryDragDrop)sender).gameObject;

		if(currentDragDropItem == null || go != currentDragDropItem.gameObject){
			currentDragDropItem = go.transform;

			if(OnDecoPickedUp != null){
				OnDecoPickedUp(go, e);
			}
		}
	}

	/// <summary>
	/// Show only the decoration inventory bar
	/// </summary>
	public void ShowDecoInventory(){
		decorationGridPanel.GetComponent<TweenToggle>().Show();
	}

	/// <summary>
	/// Hide only the decoration inventory bar
	/// </summary>
	public void HideDecoInventory(){
		decorationGridPanel.GetComponent<TweenToggle>().HideWithUpdatedPosition();
	}

	//When the highscore board is clicked and zoomed into
	protected override void _OpenUI(){
		if(!isActive){
			isActive = true;

			this.GetComponent<TweenToggleDemux>().Show();
			ShowDecoInventory();
			RoomArrowsUIManager.Instance.ShowPanel();

			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();

			// Hide the pet/minipet so it doesn't get in the way
			PetAnimationManager.Instance.DisableVisibility();
			MiniPetManager.Instance.DisableAllMinipetVisibility();
		}
	}
	
	//The back button on the left top corner is clicked to zoom out of the highscore board
	protected override void _CloseUI(){
		if(isActive){
			isActive = false;

			this.GetComponent<TweenToggleDemux>().Hide();
			HideDecoInventory();
			
			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			RoomArrowsUIManager.Instance.ShowPanel();

			// Show the pet/minipet again
			PetAnimationManager.Instance.EnableVisibility();
			MiniPetManager.Instance.EnableAllMinipetVisilibity();
		}
	}

	/// <summary>
	/// Opens the store leading to decorations for the current category the playing is trying to place.
	/// This is a little messy/complicated, because we are basically faking the deco UI closing and the
	/// shop UI opening. It's not legit because all the tweening and demux make it diffcult to do legitly.
	/// </summary>
	private void OpenShop(){
		// hide swipe arrow because not needed in shop mode
		RoomArrowsUIManager.Instance.HidePanel();

		// push the shop mode type onto the click manager stack
		ClickManager.Instance.Lock(UIModeTypes.Store);
		
		// open the shop
//		StoreUIManager.OnShortcutModeEnd += ReopenChooseMenu;	
		StoreUIManager.Instance.OpenToSubCategory("Decorations", true);

//		// open the specific sub category in the shop
//		string category = nodeSaved.GetDecoType().ToString();
//		StoreUIManager.Instance.CreateSubCategoryItemsTab(category, Color.white);
	}
}
