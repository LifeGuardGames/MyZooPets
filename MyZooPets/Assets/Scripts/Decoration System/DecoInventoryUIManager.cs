using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Decoration UI manager
/// This class includes the Decoration Inventory as well as UI for decoration.
/// InventoryManager 	- DecoInventoryUIManager	(two separate inventory managers!)
/// 					\ InventoryUIManager
/// </summary>
public class DecoInventoryUIManager : Singleton<DecoInventoryUIManager> {
	public TweenToggle decoInventoryTween;
	public GameObject decoInventoryTokenPrefab;
	public List<Transform> decoInventorySlotList;
	public Transform itemFlyToTransform;
	public Animation addItemPulseAnim;

	private int inventoryPage = 0;
	private int inventoryPageSize = 5;

	public GameObject leftButton;
	public GameObject rightButton;

	private Transform currentDragDropItem;

	public List<InventoryItem> AllDecoInvItems {
		get { return InventoryManager.Instance.AllDecoInventoryItems; }
	}

	void Start() {
		//Spawn items in the inventory for the first time
		ShowPage(0);
		RefreshButtonShowStatus();
	}

	public void ShowPanel() {
		decoInventoryTween.Show();
	}

	public void HidePanel() {
		decoInventoryTween.Hide();
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
		if((inventoryPage * inventoryPageSize) + inventoryPageSize >= AllDecoInvItems.Count) {
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
		foreach(Transform slot in decoInventorySlotList) {
			foreach(Transform child in slot) {  // Auto detect all/none children
				Destroy(child.gameObject);
			}
		}

		int startingIndex = inventoryPage * inventoryPageSize;
		int endingIndex = startingIndex + inventoryPageSize;

		for(int i = startingIndex; i < endingIndex; i++) {
			if(AllDecoInvItems.Count == i) {    // Reached the end of list
				break;
			}
			else {
				GameObject inventoryToken = GameObjectUtils.AddChildGUI(decoInventorySlotList[i % inventoryPageSize].gameObject, decoInventoryTokenPrefab);
				inventoryToken.GetComponent<InventoryTokenController>().Init(AllDecoInvItems[i]);
			}
		}
	}
	#endregion
}
