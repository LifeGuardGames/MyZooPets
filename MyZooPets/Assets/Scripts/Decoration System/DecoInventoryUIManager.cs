using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Decoration user interface manager.
/// This class includes the Decoration Inventory as well as UI for decoration.
/// 
/// 
/// InventoryManager 	- DecoInventoryUIManager	(two separate inventory managers!)
/// 					\ InventoryUIManager
/// 
/// </summary>
public class DecoInventoryUIManager : SingletonUI<DecoInventoryUIManager> {

	public static EventHandler<EventArgs> OnDecoOpened;
	public static EventHandler<InventoryDragDrop.InvDragDropArgs> OnDecoDroppedOnTarget;

	public static EventHandler<EventArgs> OnDecoPickedUp;   // when a decoration is picked up
	public static EventHandler<EventArgs> OnDecoDropped;   // when a decoration is dropped

	private bool isActive = false;

	public GameObject shopButtonParent;
	private GameObject sunbeamObject = null;
	public GameObject backButton;
	public GameObject shopButton;
	public GameObject decorationGridPanel;
	public UIPanel gridPanel;
	public GameObject uiGridObject;
	public GameObject decorationItemPrefab;

	private int maxInventoryDisplay = 6;
    private float collapsedPos = -164f;

	public Transform currentDragDropItem;

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.EditDecos;
	}

	protected override void Start(){
		base.Start();
		InventoryManager.OnItemAddedToDecoInventory += OnItemAddedHandler;
		InventoryManager.OnItemUsed += OnItemUsedHandler;

		// Spawn items in the decoration inventory for the first time
		List<InventoryItem> listDecos = InventoryManager.Instance.AllDecoInventoryItems;
		foreach(InventoryItem invItem in listDecos){
			// Setting isOnLoad option to true for first time loading
			SpawnInventoryItemInPanel(invItem, isOnLoad:true);
		}

		// Workaround for start hidden, the position setting conflicts with tween
		StartCoroutine(NextFrameHelper());
	}

	IEnumerator NextFrameHelper(){
		yield return 0;
		HideDecoInventory();
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		InventoryManager.OnItemAddedToDecoInventory -= OnItemAddedHandler;
		InventoryManager.OnItemUsed -= OnItemUsedHandler;
	}

	/// <summary>
	/// Check if the deco inventory is scrollable
	/// If the item types in invetory is greater than the max display item type count, make it scrollable
	/// </summary>
	/// <returns><c>true</c> if this instance is inventory scrollable; otherwise, <c>false</c>.</returns>
	public bool IsDecoInventoryScrollable(){
		return InventoryManager.Instance.AllDecoInventoryItems.Count > maxInventoryDisplay;
	}

	public GameObject GetTutorialItem(){
		GameObject tutorialObject = null;

		if(uiGridObject.transform.childCount != 0){
			Transform parent = uiGridObject.transform.GetChild(0);
			tutorialObject = parent.FindChild(parent.name).gameObject;
		}

		return tutorialObject;
	}

	public GameObject GetShopButton(){
		return shopButton;
	}

	// Listening to when new item is added to the deco inventory
	private void OnItemAddedHandler(object sender, InventoryManager.InventoryEventArgs e){
		if(e.IsItemNew){
			SpawnInventoryItemInPanel(e.InvItem);
		}
		else{
			Transform invItem = uiGridObject.transform.Find(e.InvItem.ItemID);
			invItem.Find("Label_Amount").GetComponent<UILabel>().text = e.InvItem.Amount.ToString();
		}
	}

	/// <summary>
	/// Items the used event handler.
	/// Called to update the bar from deco inventory
	/// </summary>
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
	
		int allDecoInventoryItemsCount = InventoryManager.Instance.AllDecoInventoryItems.Count;
		// Normal case where you add item during game
		if(!isOnLoad){
		
			// Adjust the bar length based on how many items we want showing at all times
			if(allDecoInventoryItemsCount <= maxInventoryDisplay) {
				
				// Update position of the bar if inventory is open
				LeanTween.moveLocalX(decorationGridPanel, collapsedPos - allDecoInventoryItemsCount * 90, 0.4f)
					.setEase(LeanTweenType.easeInOutQuad);
			}
		}
		// Scene loading case, dont want to tween here so set them explicitly
		else{
			// Adjust the bar length based on how many items we want showing at all times
			if(allDecoInventoryItemsCount > maxInventoryDisplay) {
				allDecoInventoryItemsCount = maxInventoryDisplay;
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

		// Pulse shop icon where appropriate
		if(allDecoInventoryItemsCount == 0){
			TogglePulseShopButton(true);
		}
		else{
			TogglePulseShopButton(false);
		}
	}

	//Find the position of Decoration Item game object with invItemID
	//Used for animation position in StoreUIManager
	public Vector3 GetPositionOfDecoInvItem(string itemID){
		// position to use
		Vector3 decoInvItemPosition;
		
		// Use the position of the item in the inventory panel
		Transform deocInvItemTrans = uiGridObject.transform.Find(itemID);
		if(deocInvItemTrans == null){
			Debug.Log("ksd");
		}
		InventoryItem decoInvItem = InventoryManager.Instance.GetDecoInInventory(itemID);
		decoInvItemPosition = deocInvItemTrans.position;
		
		//Offset position if the item is just added to the inventory
		if(decoInvItem.Amount == 1)
			decoInvItemPosition += new Vector3(-0.22f, 0, 0);
		
		return decoInvItemPosition;
	}

	//Event listener. listening to when item is dragged out of the deco inventory on drop
	//on something in the game
	private void OnItemDrop(object sender, InventoryDragDrop.InvDragDropArgs e){
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

			if(OnDecoOpened != null){
				OnDecoOpened(this, EventArgs.Empty);
			}

			this.GetComponent<TweenToggleDemux>().Show();
			ShowDecoInventory();
			RoomArrowsUIManager.Instance.ShowPanel();

			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();

			// Hide the pet/minipet so it doesn't get in the way
			PetAnimationManager.Instance.DisableVisibility();
			if(MiniPetManager.Instance){
				MiniPetManager.Instance.ToggleAllMinipetVisilibity(false);
			}

			if(InventoryManager.Instance.AllDecoInventoryItems.Count == 0){
				TogglePulseShopButton(true);
			}
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
			if(MiniPetManager.Instance){
				MiniPetManager.Instance.ToggleAllMinipetVisilibity(true);
			}

			TogglePulseShopButton(false);
		}
	}

	/// <summary>
	/// Opens the shop directly to decoration category and jumps to specific
	/// decoration type
	/// </summary>
	/// <param name="decorationType">Decoration type.</param>
	private void OpenShopForTutorial(){
		// hide swipe arrow because not needed in shop mode
		RoomArrowsUIManager.Instance.HidePanel();
		
		// push the shop mode type onto the click manager stack
		ClickManager.Instance.Lock(UIModeTypes.Store);
		
		// open the shop
		StoreUIManager.OnShortcutModeEnd += ReopenChooseMenu;
		StoreUIManager.Instance.OpenToSubCategory("Decorations", true, StoreShortcutType.DecorationUIStoreButtonTutorial);

		string tabName = DecorationTypes.Carpet.ToString();
		StoreUIManager.Instance.CreateSubCategoryItemsTab(tabName, Color.white);
	}

	/// <summary>
	/// Open store directly to decoration category
	/// </summary>
	private void OpenShop(){
		// hide swipe arrow because not needed in shop mode
		RoomArrowsUIManager.Instance.HidePanel();

		// push the shop mode type onto the click manager stack
		ClickManager.Instance.Lock(UIModeTypes.Store);
		
		// open the shop
		StoreUIManager.OnShortcutModeEnd += ReopenChooseMenu;
		StoreUIManager.Instance.OpenToSubCategory("Decorations", true, StoreShortcutType.DecorationUIStoreButton);
	}

	/// <summary>
	/// This function is called from the store UI when the store closes and the user had opened the store from the deco system.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void ReopenChooseMenu(object sender, EventArgs args){
		// show swipe arrows
		RoomArrowsUIManager.Instance.ShowPanel();
		
		// pop the mode we pushed earlier from the click manager
		ClickManager.Instance.ReleaseLock();
		
		StoreUIManager.OnShortcutModeEnd -= ReopenChooseMenu;
	}

	private void TogglePulseShopButton(bool isPulseShopButton){
		// Cache the sunbeam
		if(sunbeamObject == null){
			sunbeamObject = shopButtonParent.transform.FindChild("SunBeamRotating").gameObject;
			if(sunbeamObject == null){
				Debug.LogWarning("Cannot find sunbeam object from deco mode shop");
			}
		}

		if(isPulseShopButton){
			shopButtonParent.GetComponent<Animation>().Play();
			sunbeamObject.SetActive(true);
		}
		else{
			shopButtonParent.GetComponent<Animation>().Stop();
			GameObjectUtils.ResetLocalScale(shopButtonParent);
			sunbeamObject.SetActive(false);
		}
	}
}
