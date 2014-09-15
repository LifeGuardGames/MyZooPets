using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Decoration user interface manager.
/// This class includes the Decoration Inventory as well as UI for inventory.
/// 
/// 
/// InventoryLogic 	- DecoInventoryUIManager	(two separate inventory managers!)
/// 				\ InventoryUIManager
/// 
/// </summary>
public class DecoInventoryUIManager : SingletonUI<DecoInventoryUIManager> {

	public EventHandler<EventArgs> OnDecoPickedUp;   // when a decoration is picked up
	public EventHandler<EventArgs> OnDecoDropped;   // when a decoration is picked up

	private bool isActive = false;

	public GameObject backButton;
	public DecorationItem currentDeco;

	public GameObject decorationGridPanel;
	public UIPanel gridPanel;
	public GameObject uiGridObject;
	public GameObject decorationItemPrefab;

	private float collapsedPos = -164f;

	private Transform currentDragDropItem;

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
		Debug.Log("ADDDED DECOO");
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
//		invDragDrop.OnItemDrop += OnItemDrop;
//		invDragDrop.OnItemPress += OnItemPress;
		
		UpdateBarPosition(isOnLoad);
	}

	/// <summary>
	/// Updates the bar position.
	/// </summary>
	/// <param name="isOnLoad">If set to <c>true</c> does tweening instantly, used for loading into scene check only</param>
	public void UpdateBarPosition(bool isOnLoad = false){
		Debug.Log("updating");
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
		Debug.Log("LOOKING FOR POS");
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

			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
			RoomArrowsUIManager.Instance.HidePanel();

			backButton.SetActive(true);
		}
	}
	
	//The back button on the left top corner is clicked to zoom out of the highscore board
	protected override void _CloseUI(){
		if(isActive){
			isActive = false;

			this.GetComponent<TweenToggleDemux>().Hide();
			
			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			EditDecosUIManager.Instance.ShowNavButton();
			RoomArrowsUIManager.Instance.ShowPanel();
			
			backButton.SetActive(false);
		}
	}
}
