using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Consumable Inventory UI manager
/// InventoryManager 	- DecoInventoryUIManager	(two separate inventory managers!)
/// 					\ InventoryUIManager
/// </summary>
public class InventoryUIManager : Singleton<InventoryUIManager>{
	public TweenToggle inventoryTween;
	public GameObject inventoryTokenPrefab;
	public List<Transform> inventorySlotList;
	public Transform itemFlyToTransform;
	public Animation addItemPulseAnim;

	private int inventoryPage = 0;
	private int inventoryPageSize = 5;

	public GameObject leftButton;
	public GameObject rightButton;

	private Transform currentDragDropItem;

	public List<InventoryItem> AllInvItems {
		get { return InventoryManager.Instance.AllConsumableInventoryItems; }
	}

	void Start(){
		//Spawn items in the inventory for the first time
		ShowPage(0);
		RefreshButtonShowStatus();
    }
	
	public void ShowPanel(){
		inventoryTween.Show();
	}
	
	public void HidePanel(){
		inventoryTween.Hide();
	}

	public void PulseInventory() {
		addItemPulseAnim.Play();
	}

	#region Page sorting functions
	// Checks to see if the buttons need to appear
	public void RefreshButtonShowStatus() {
		// Check left most limit
		if(inventoryPage == 0) {
			leftButton.SetActive(false);
		}
		else {
			leftButton.SetActive(true);
		}
		// Check right most limit
		if((inventoryPage * inventoryPageSize) + inventoryPageSize >= AllInvItems.Count) {
			rightButton.SetActive(false);
		}
		else {
			rightButton.SetActive(true);
		}
	}

	public void OnPageButtonClicked(bool isRightButton) {
		if(isRightButton) {
			inventoryPage++;
		}
		else {
			inventoryPage--;
		}
		ShowPage(inventoryPage);
		RefreshButtonShowStatus();
	}

	public void RefreshPage() {
		ShowPage(inventoryPage);
		RefreshButtonShowStatus();
	}

	// Either refreshes current page, or shows a new page given page number
	private void ShowPage(int inventoryPage) {
		// Destroy children beforehand
		foreach(Transform slot in inventorySlotList) {
			foreach(Transform child in slot) {  // Auto detect all/none children
				Destroy(child.gameObject);
			}
		}

		int startingIndex = inventoryPage * inventoryPageSize;
		int endingIndex = startingIndex + inventoryPageSize;

		for(int i = startingIndex; i < endingIndex; i++) {
			if(AllInvItems.Count == i) {    // Reached the end of list
				break;
			}
			else {
				GameObject inventoryToken = GameObjectUtils.AddChildGUI(inventorySlotList[i % inventoryPageSize].gameObject, inventoryTokenPrefab);
				inventoryToken.GetComponent<InventoryTokenController>().Init(AllInvItems[i]);
			}
		}
	}
	#endregion
}
