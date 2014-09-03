using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;

public class AccessoryUIManager : SingletonUI<AccessoryUIManager> {
	public UIGrid grid;
	public GameObject accessoryTitlePrefab;
	public GameObject accessoryEntryPrefab;
	public GameObject backButton;
	public GameObject zoomItem;
	private bool isActive = false;
	
	// related to zooming into the badge board
	public float fZoomTime;
	public Vector3 vOffset;
	public Vector3 vRotation;

	public string soundBuy;
	public string soundUnequip;
	public string soundEquip;

	void Start(){
		// Populate the entries with loaded data
		List<Item> accessoryList = ItemLogic.Instance.AccessoryList;
		string lastCategory = null;
		foreach(Item accessory in accessoryList){

			// Create a new accessory type label if lastCategory has changed
			if(lastCategory != (AccessoryItem)accessory.Type.ToString()){
				GameObject itemUIObject = NGUITools.AddChild(grid, accessoryTitlePrefab);
				UILocalize localize = itemUIObject.GetComponent<UILocalize>();
				switch(accessory.Type){
				case AccessoryTypes.Hat:
					localize.key = "ACCESSORIES_TYPE_HAT";
					break;
				case AccessoryTypes.Hat:
					localize.key = "ACCESSORIES_TYPE_GLASSES";
					break;
				case AccessoryTypes.Hat:
					localize.key = "ACCESSORIES_TYPE_COLOR";
					break;
				default:
					Debug.LogError("Invalid accessory type");
					break;
				}
				localize.Localize();	// Force relocalize
				lastCategory = (AccessoryItem)accessory.Type.ToString();
			}
			AccessoryEntryUIController.CreateEntry(grid.gameObject, accessoryEntryPrefab, accessory);
		}
	}

	// When the zoomItem is clicked and zoomed into
	protected override void _OpenUI(){
		if(!isActive){
			// zoom into the item
			Vector3 vPos = zoomItem.transform.position + vOffset;
			CameraManager.Instance.ZoomToTarget( vPos, vRotation, fZoomTime, null );
			
			//Hide other UI objects
			NavigationUIManager.Instance.HidePanel();
			HUDUIManager.Instance.HidePanel();
			InventoryUIManager.Instance.HidePanel();
			EditDecosUIManager.Instance.HideNavButton();
			RoomArrowsUIManager.Instance.HidePanel();
			
			isActive = true;
			zoomItem.collider.enabled = false;
			
			backButton.SetActive(true);
		}
	}
	
	//The back button on the left top corner is clicked to zoom out of the badge board
	protected override void _CloseUI(){
		if(isActive){
			isActive = false;
			zoomItem.collider.enabled = true;
			
			CameraManager.Instance.ZoomOutMove();
			
			//Show other UI Objects
			NavigationUIManager.Instance.ShowPanel();
			HUDUIManager.Instance.ShowPanel();
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
		Transform buttonParent = button.transform.parent.parent;
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
				button.GetComponent<AccessoryEntryUIController>().SetState(AccessoryButtonType.BoughtEquipped);

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
}
