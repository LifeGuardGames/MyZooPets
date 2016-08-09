using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Accessory user interface manager.
/// 
/// ARCHITECTURE:
/// Controlling "store" buttons and entries:
/// 	AccessoryUIManager -> AccessoryEntryUIController
/// 
/// Controlling equipping accessory on pet:
/// 	AccessoryUIManager -> AccessoryNodeController -> AccessoryNode
/// </summary>
public class AccessoryUIManager : SingletonUI<AccessoryUIManager>{
	public GridLayoutGroup grid;
	public GameObject accessoryTitlePrefab;
	public GameObject accessoryEntryPrefab;
	public GameObject backButton;
	public GameObject zoomItem;
	public TweenToggle baseTween;
	public TweenToggle gridTween;

	// related to zooming into the badge board
	private float zoomTime  = 0.5f;
	public Vector3 zoomOffset;
	public Vector3 zoomRotation;
	public string soundBuy;
	public string soundUnequip;
	public string soundEquip;
	private List<AccessoryEntryUIController> accessoryEntryList = new List<AccessoryEntryUIController>();
	private bool isActive = false;
	//temp variable for pet scale
	private Vector3 petScale;

	protected override void Awake(){
		base.Awake();
		eModeType = UIModeTypes.Accessory;
	}

	protected override void Start(){
		base.Start();
		HUDAnimator.OnLevelUp += RefreshAccessoryItems; //listen to level up so we can unlock items
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		HUDAnimator.OnLevelUp -= RefreshAccessoryItems;
	}

	// When the zoomItem is clicked and zoomed into
	protected override void _OpenUI(){
		if(!isActive){
			AudioManager.Instance.PlayClip("subMenu");

			// Zoom into the item
			Vector3 targetPosition = zoomItem.transform.position + zoomOffset;

			CameraManager.Callback cameraDoneFunction = delegate(){
				CameraMoveDone();
			};
			CameraManager.Instance.ZoomToTarget(targetPosition, zoomRotation, zoomTime, cameraDoneFunction);

			// Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			RoomArrowsUIManager.Instance.HidePanel();

			//need to disable more things here
			PetAnimationManager.Instance.DisableIdleAnimation();
			PetMovement.Instance.canMove = false;
			isActive = true;
			zoomItem.GetComponent<Collider>().enabled = false;
			
			backButton.SetActive(true);
		}
	}

	public void OnHatsButton(){
		ShowCategory(AccessoryTypes.Hat);
	}

	public void OnGlassesButton(){
		ShowCategory(AccessoryTypes.Glasses);
	}

	public void ShowCategory(AccessoryTypes type){
		baseTween.Hide();
		gridTween.Show();
		int itemCount = 0;
		// Populate the entries with loaded data
		List<Item> accessoryList = ItemManager.Instance.AccessoryList;
		foreach(AccessoryItem accessory in accessoryList) {
			if(accessory.AccessoryType == type) {
				GameObject itemUIObject = GameObjectUtils.AddChildWithPositionAndScale(grid.gameObject, accessoryTitlePrefab);
				UILocalize localize = itemUIObject.GetComponent<UILocalize>();

				switch((AccessoryTypes)accessory.AccessoryType) {
					case AccessoryTypes.Hat:
						localize.key = "ACCESSORIES_TYPE_HAT";
						break;
					case AccessoryTypes.Glasses:
						localize.key = "ACCESSORIES_TYPE_GLASSES";
						break;
					default:
						Debug.LogError("Invalid accessory type");
						break;
				}
				localize.Localize();    // Force relocalize

				GameObject entry = AccessoryEntryUIController.CreateEntry(grid.gameObject, accessoryEntryPrefab, accessory);
				accessoryEntryList.Add(entry.GetComponent<AccessoryEntryUIController>());
				itemCount++;
			}
		}
		// Adjust the grid height based on the height of the cell and spacing
		float gridHeight = itemCount * (grid.cellSize.y + grid.spacing.y);
		grid.GetComponent<RectTransform>().sizeDelta = new Vector2(grid.cellSize.x, gridHeight);
	}

	/// <summary>
	/// Show the ui once camera is done zooming in.
	/// </summary>
	private void CameraMoveDone(){
		TweenToggleDemux toggleDemux = this.GetComponent<TweenToggleDemux>();
		toggleDemux.Show();
		toggleDemux.ShowTarget = this.gameObject;
		toggleDemux.ShowFunctionName = "MovePet";
	}

	public void OnBaseBackButton() {
		CloseUI();
	}

	public void OnGridBackButton(){
		gridTween.Hide();
	}

	// The back button on the left top corner is clicked to zoom out of the zoom item
	protected override void _CloseUI(){
		if(isActive){
			this.GetComponent<TweenToggleDemux>().Hide();
			PetMovement.Instance.canMove = true;
			isActive = false;
			zoomItem.GetComponent<Collider>().enabled = true;

			CameraManager.Instance.ZoomOutMove();
			PetAnimationManager.Instance.EnableIdleAnimation();
			
			// Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			RoomArrowsUIManager.Instance.ShowPanel();
			ClickManager.Instance.ReleaseLock();
			backButton.SetActive(false);
		}
	}

	/// <summary>
	/// Move pet into accessory view after camera is done zooming in
	/// </summary>
	private void MovePet(){
		GameObject pet = GameObject.Find("Pet");
		//teleport first then walk into view
		if(!pet.GetComponent<Renderer>().isVisible)
		PetMovement.Instance.petSprite.transform.position = new Vector3(-4f, 0, 26.65529f);
		PetMovement.Instance.MovePetFromAccessory(new Vector3(-8f, 0, 26.65529f));

	}

	// Called from AccessoryEntryUIController, returns success state for button toggling
	public void BuyAccessory(Item itemData){
		bool isSuccess = false;
		switch(itemData.CurrencyType){
		case CurrencyTypes.WellaCoin:
			if(StatsManager.Instance.GetStat(StatType.Coin) >= itemData.Cost){
				
				InventoryManager.Instance.AddItemToInventory(itemData.ID);
				StatsManager.Instance.ChangeStats(coinsDelta: itemData.Cost * -1);
				EquipAccessory(itemData.ID);

				Analytics.Instance.ItemEvent(Analytics.ITEM_STATUS_BOUGHT, itemData.Type, itemData.ID);

				//Check for badge unlock
				int totalNumOfAccessories = DataManager.Instance.GameData.Inventory.AccessoryItems.Count;
				BadgeManager.Instance.CheckSeriesUnlockProgress(BadgeType.Accessory, totalNumOfAccessories, true);

				AudioManager.Instance.PlayClip(soundBuy);

				RefreshAccessoryItems();
			}
			else{
				HUDUIManager.Instance.PlayNeedCoinAnimation();
				AudioManager.Instance.PlayClip("buttonDontClick");
			}
			break;
		}
	}

	// Called from AccessoryEntryUIController
	public void EquipAccessory(string itemID){
		UnequipAccessory(itemID);									// Unequip anything first
		AccessoryManager.Instance.SetAccessoryAtNode(itemID);		// Set the mutable data
		AccessoryNodeController.Instance.SetAccessory(itemID);		// Equip the node
		RefreshAccessoryItems();

		AudioManager.Instance.PlayClip(soundEquip);
	}

	// Called from AccessoryEntryUIController
	public void UnequipAccessory(string itemID){
		AccessoryManager.Instance.RemoveAccessoryAtNode(itemID);	// Set the mutable data
		AccessoryNodeController.Instance.RemoveAccessory(itemID);	// Still need item ID to know which node to remove
		RefreshAccessoryItems();

		AudioManager.Instance.PlayClip(soundUnequip);
	}

	//Check for any UI updateds
	private void RefreshAccessoryItems(){
		foreach(AccessoryEntryUIController entryController in accessoryEntryList){
			entryController.CheckState();
		}
	}

	// Used for unlocking new decos
	private void RefreshAccessoryItems(object sender, EventArgs args){
		RefreshAccessoryItems();
	}
}
