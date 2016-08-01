using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Consumable Inventory UI manager
/// InventoryManager 	- DecoInventoryUIManager	(two separate inventory managers!)
/// 					\ InventoryUIManager
/// </summary>
public class InventoryUIManager : Singleton<InventoryUIManager>{
	public static EventHandler<InventoryDragDrop.InvDragDropArgs> ItemDroppedOnTargetEvent;

	public TweenToggle inventoryTween;
	public RectTransform slotParent;
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

	/// <summary>
	/// Gets the fire orb reference.
	/// Use for the tutorial to get the fire orb gameobject.
	/// </summary>
	/// <returns>The fire orb reference.</returns>
	public GameObject GetFireOrbReference(){
		GameObject retVal = null;
		foreach(Transform item in slotParent) {
			if(item.name == "Usable1"){
				Transform trans = item.Find("Usable1");
				if(trans != null){
					retVal = trans.gameObject;
				}
			}
		}

		// Check if user is dragging it as well...
		if(retVal == null){
			GameObject dragPanel = GameObject.Find("ItemDragPanel");
			if(dragPanel != null){
				Transform trans = dragPanel.transform.Find("Usable1");
				if(trans != null){
					retVal = trans.gameObject;
				}
			}
		}
		return retVal;
	}
	
	//Find the position of Inventory Item game object with invItemID
	//Used for animation position in StoreUIManager
	public Vector3 GetPositionOfInvItem(string invItemID){
		// position to use
		Vector3 invItemPosition;

		// Use the position of the item in the inventory panel
		Transform invItemTrans = slotParent.Find(invItemID);
		InventoryItem invItem = InventoryManager.Instance.GetItemInInventory(invItemID);
		invItemPosition = invItemTrans.position;
		
		//Offset position if the item is just added to the inventory
		if(invItem.Amount == 1){
			invItemPosition += new Vector3(-0.22f, 0, 0);
		}
		
		return invItemPosition;
	}
	
	public void ShowPanel(){
		inventoryTween.Show();
	}
	
	public void HidePanel(){
		inventoryTween.Hide();
	}

	private void OnItemDropHandler(object sender, InventoryDragDrop.InvDragDropArgs e){
		if(e.TargetCollider && e.TargetCollider.tag == "ItemTarget"){
			currentDragDropItem = e.ParentTransform;

			if(ItemDroppedOnTargetEvent != null){
				ItemDroppedOnTargetEvent(this, e);
			}
		}
	}

	// Called from InventoryManager
	public void OnItemUsedUI(InventoryItem invItem){
		if(currentDragDropItem != null){
			if(invItem != null && invItem.Amount > 0){ //Redraw count label if item not 0
				Transform gridObj = slotParent.Find(invItem.ItemID);
				gridObj.GetComponent<InventoryTokenController>().SetAmount(invItem.Amount);
			}
			else{ //destroy object if it has been used up
				Destroy(currentDragDropItem.gameObject);
			}
		}
	}

	/// <summary>
	/// Handles the item press event.
	/// </summary>
	private void OnItemPress(object sender, InventoryDragDrop.InvDragDropArgs e){
//		bool isTutDone = DataManager.Instance.GameData.Tutorial.ListPlayed.Contains(TutorialManagerBedroom.TUT_FEED_PET);
//
//		//remove drag hint on the next time user press on any item 
//		if(fingerHintGO != null)
//			Destroy(fingerHintGO);
//
//		//if user is pressing the item for the first time show hint
//		if(!isTutDone){
//			Vector3 hintPos = e.ParentTransform.position;
//			GameObject fingerHintResource = Resources.Load("inventorySwipeTut") as GameObject;
//			fingerHintGO = (GameObject)Instantiate(fingerHintResource, hintPos, Quaternion.identity);
//			fingerHintGO.transform.parent = GameObject.Find("Anchor-BottomRight").transform;
//			fingerHintGO.transform.localScale = new Vector3(1, 1, 1);
//
//			// fingerHintGO.transform.position = hintPos; 
//			DataManager.Instance.GameData.Tutorial.ListPlayed.Add(TutorialManagerBedroom.TUT_FEED_PET);
//		}
	}

	public void PulseItem() {
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
