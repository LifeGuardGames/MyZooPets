using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Inventory listener.
/// Listens to the amout of items and kills itself if necessary
/// </summary>
public class InventoryListener : MonoBehaviour {
	
	// private Inventory inventoryScript;
	// private InventoryUIManager inventoryGuiScript;
	// private UILabel itemLabel;
	// public string name;
	// public int id;
	
	// private int count;
	// public int Count{
	// 	get{
	// 		return count;
	// 	}
	// 	set{
	// 		if(value < 1)
	// 			Debug.LogError("Spawning bad item count");
	// 		else
	// 			count = value;
	// 	}
	// }
	
	// void Awake(){
	// 	// Inventory.OnUpdateItem += UpdateItem;
	// 	inventoryScript = GameObject.Find("GameManager/InventoryLogic").GetComponent<Inventory>();
	// 	inventoryGuiScript = GameObject.Find("Inventory").GetComponent<InventoryUIManager>();
	// 	itemLabel = gameObject.GetComponentInChildren<UILabel>();
	// }
	
	// private void UpdateItem(object sender, EventArgs e){
	// 	if(inventoryScript.InventoryArray[id] != count){
	// 		if(count < 1){
	// 			inventoryGuiScript.DecreaseItemTypeCount();
	// 			Destroy(this);					// TODO asynchronous issues?
	// 		}
	// 		else{
	// 			itemLabel.text = inventoryScript.InventoryArray[id].ToString();
	// 		}
	// 	}
 //    }
	
	// // Call when the item has been disabled
	// void OnDisable(){
	// 	inventoryGuiScript.UpdateBarPosition();	// TODO asynchronous issues?
	// }
}
