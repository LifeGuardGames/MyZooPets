using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Accessory node on the pet to be controlled by AccessoryNodeController for population
/// In scenes where you only need to show accessories, this by itself is enough!
/// NOTE: A node can contain references to multiple body sprites, ie. finger accessories, not yet implemented
/// </summary>
public class AccessoryNode : MonoBehaviour{
	public string accessoryNodeID;				// ID of this node
	private string placedAccessoryID = null;	// ID of the acc placed
	public GameObject placedAccessoryObject;	// Actual gameobect of the acc placed
//	public List<GameObject> spriteNodes = new List<GameObject>();	// UNDONE List because there might be different versions of the same body part

	void Start(){
		if(accessoryNodeID == null){
			Debug.LogError("Missing accessory node ID");
		}

		// Might not always have controller depending on scene
		if(AccessoryNodeController.Instance != null){
			// Add self to the hash table in the controller
			if(AccessoryNodeController.Instance.accessoryNodeHash.ContainsKey(accessoryNodeID)){
				Debug.LogError("Key already present: " + accessoryNodeID);
			}
			else{
				AccessoryNodeController.Instance.accessoryNodeHash.Add(accessoryNodeID, this);
			}
		}

		CheckSaveData();
	}
	
	public void CheckSaveData(){
//		// if the saved data contains this node's id, it means there was a accessory placed here
//		if(DataManager.Instance.GameData.Accessories.PlacedAccessories.ContainsKey(accessoryNodeID)){
//			string savedAccessoryKey = DataManager.Instance.GameData.Accessories.PlacedAccessories[accessoryNodeID];
//			SetAccessoryNode(savedAccessoryKey);
//		}

		string savedAccessoryKey = DataManager.Instance.GameData.Accessories.GetPlacedAccessory(accessoryNodeID);
		if(!string.IsNullOrEmpty(savedAccessoryKey))
			SetAccessoryNode(savedAccessoryKey);
	}
	
	/// <summary>
	/// Sets the accessory node.
	/// IMPORTANT: Only to be changed from AccessoryNodeController!
	/// </summary>
	/// <param name="savedAccessoryKey">Saved accessory key.</param>
	public void SetAccessoryNode(string newAccessoryID){

		// UNDONE Supporting one node as of now, adding multiple nodes would need to reflect with XML attribues(?) as well
//		foreach(GameObject spriteNode in spriteNodes){
//		}

		if(HasAccessory()){
			RemoveAccessoryNode();
		}

		if(newAccessoryID != null){
			// Cache the new itemID
			placedAccessoryID = newAccessoryID;

			// Populate it in gamedata in placedAccessories
//			DataManager.Instance.GameData.Accessories.PlacedAccessories[accessoryNodeID] = placedAccessoryID;
//			DataManager.Instance.GameData.Accessories.SetAccessoryAtNode(accessoryNodeID, placedAccessoryID);

			// Load and place the actual accessory
			// build the prefab from the id of the decoration
			string strResource = DataLoaderItems.GetAccessoryItemPrefabName(placedAccessoryID);
			GameObject goPrefab = Resources.Load(strResource) as GameObject;

			if(goPrefab){
				placedAccessoryObject = Instantiate(goPrefab, Vector3.zero, goPrefab.transform.rotation) as GameObject;
				placedAccessoryObject.transform.parent = transform;
				GameObjectUtils.ResetLocalTransform(placedAccessoryObject);	// Zero out all local transforms
			}
			else{
				Debug.LogError("No such prefab for " + strResource);
			}
		}
	}

	public void RemoveAccessoryNode(){
//		// give the user the decoration back in their inventory
//		if(placedAccessoryID != null)
//			InventoryLogic.Instance.AddItem(placedAccessoryID, 1);	// TODO doublecheck this
//		else
//			Debug.LogError("Just removed an illegal accessory?");		
//		
//		// update the save data since this node is now empty
//		DataManager.Instance.GameData.Accessories.PlacedAccessories.Remove(placedAccessoryID);
//		
		// reset the deco id on this node
		placedAccessoryID = null;

		//TODO refresh the button here

		// Destroy the gameobject
		Destroy(placedAccessoryObject);

	}

	public string GetAccessoryID(){
		return placedAccessoryID;
	}

	public bool HasAccessory(){
		return placedAccessoryID != null;
	}
}
