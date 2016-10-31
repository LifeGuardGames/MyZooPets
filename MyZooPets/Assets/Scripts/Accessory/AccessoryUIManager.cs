using UnityEngine;
using UnityEngine.UI;
using System;
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
	public GameObject accessoryEntryPrefab;
	public GridLayoutGroup gridParent;
	public GameObject zoomItemEntrance;
	public TweenToggle baseTween;
	public TweenToggle gridTween;
	public UILocalize categoryBanner;

	// related to zooming into the badge board
	private float zoomTime = 0.5f;
	public Vector3 zoomOffset;
	public Vector3 zoomRotation;

	private bool isActive = false;
	private List<AccessoryStoreItemController> accessoryEntryList = new List<AccessoryStoreItemController>();
	private Vector3 petScale;       //temp variable for pet scale

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
			Vector3 targetPosition = zoomItemEntrance.transform.position + zoomOffset;

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
			zoomItemEntrance.GetComponent<Collider>().enabled = false;
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
		foreach(AccessoryItem accessoryData in accessoryList) {
			if(accessoryData.AccessoryType == type) {
				// Change the title of the category
				switch(accessoryData.AccessoryType) {
					case AccessoryTypes.Hat:
						categoryBanner.key = "ACCESSORIES_TYPE_HAT";
						break;
					case AccessoryTypes.Glasses:
						categoryBanner.key = "ACCESSORIES_TYPE_GLASSES";
						break;
					default:
						Debug.LogError("Invalid accessory type");
						break;
				}
				categoryBanner.Localize();    // Force relocalize

				GameObject accessoryEntry = GameObjectUtils.AddChild(gridParent.gameObject, accessoryEntryPrefab);
				AccessoryStoreItemController entryController = accessoryEntry.GetComponent<AccessoryStoreItemController>();
				entryController.Init(accessoryData);
				accessoryEntryList.Add(entryController);
				itemCount++;
			}
		}

		// Adjust the grid height based on the height of the cell and spacing
		float gridHeight = itemCount * (gridParent.cellSize.y + gridParent.spacing.y);
		gridParent.GetComponent<RectTransform>().sizeDelta = new Vector2(gridParent.cellSize.x, gridHeight);
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
		baseTween.Show();
    }

	// The back button on the left top corner is clicked to zoom out of the zoom item
	protected override void _CloseUI(){
		if(isActive){
			this.GetComponent<TweenToggleDemux>().Hide();
			PetMovement.Instance.canMove = true;
			isActive = false;
			zoomItemEntrance.GetComponent<Collider>().enabled = true;

			CameraManager.Instance.ZoomOutMove();
			PetAnimationManager.Instance.EnableIdleAnimation();
			
			// Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			InventoryUIManager.Instance.ShowPanel();
			RoomArrowsUIManager.Instance.ShowPanel();
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

				AudioManager.Instance.PlayClip("shopBuy");

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

		AudioManager.Instance.PlayClip("buttonGeneric2");
	}

	// Called from AccessoryEntryUIController
	public void UnequipAccessory(string itemID){
		AccessoryManager.Instance.RemoveAccessoryAtNode(itemID);	// Set the mutable data
		AccessoryNodeController.Instance.RemoveAccessory(itemID);	// Still need item ID to know which node to remove
		RefreshAccessoryItems();

		AudioManager.Instance.PlayClip("buttonGeneric2");
	}

	//Check for any UI updateds
	private void RefreshAccessoryItems(){
		foreach(AccessoryStoreItemController entryController in accessoryEntryList){
			entryController.CheckState();
		}
	}

	// Used for unlocking new decos
	private void RefreshAccessoryItems(object sender, EventArgs args){
		RefreshAccessoryItems();
	}
}
