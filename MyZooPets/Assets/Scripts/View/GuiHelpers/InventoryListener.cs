using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Inventory listener.
/// Listens to the amout of items and kills itself if necessary
/// </summary>
public class InventoryListener : MonoBehaviour {
	
	private Inventory inventoryScript;
	private InventoryGUI inventoryGuiScript;
	string name;
	int id;
	int count;
	
	void Awake(){
		Inventory.OnUpdateItem += UpdateItem;
		inventoryScript = GameObject.Find("GameManager/InventoryLogic").GetComponent<Inventory>();
		inventoryGuiScript = GameObject.Find("Panel").GetComponent<InventoryGUI>();
	}
	
	private void UpdateItem(object sender, EventArgs e){
		if(inventoryScript.InventoryArray[id] != count){
			if(count < 1){
				Destroy(this);
			}
		}
    }
	
	
}
