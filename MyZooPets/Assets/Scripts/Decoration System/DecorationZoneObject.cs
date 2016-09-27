using UnityEngine;

public class DecorationZoneObject : DecorationZone, IDropInventoryTarget {
	public Transform placementNode;
	private GameObject decoGameObject;  // Instantiated game object decoration
	private string oldDecoId;
	private string currDecoId;

	// Child override to set decoration
	protected override void _SetDecoration(string decoID, bool isPlacedFromDecoMode){
		currDecoId = decoID;
		// Build the prefab from the id of the decoration
		string strResource = DataLoaderItems.GetDecoItemPrefabName(decoID);
		GameObject goPrefab = Resources.Load(strResource) as GameObject;
		if(decoGameObject != null) {
			Destroy(decoGameObject.gameObject);
			InventoryManager.Instance.AddItemToInventory(oldDecoId);
			DataManager.Instance.GameData.Decorations.PlacedDecorations.Remove(oldDecoId);
		}
		if(goPrefab){
			decoGameObject = Instantiate(goPrefab) as GameObject;
			decoGameObject.transform.parent = placementNode;	// Put it in the hierachy of decorations in this room
			GameObjectUtils.ResetLocalPosition(decoGameObject); // Reset all transforms
			DataManager.Instance.GameData.Decorations.PlacedDecorations.Remove(currDecoId);
			InventoryManager.Instance.UseDecoItem(currDecoId);
			oldDecoId = currDecoId;
			// If the object is a farm deco, init some variables
			FarmGenerator farmScript = decoGameObject.GetComponent<FarmGenerator>();
			if(farmScript != null){
				farmScript.Initialize(decoID, isPlacedFromDecoMode);
			}
		}
		else
			Debug.LogError("No such prefab for " + strResource);
	}

	// Child override to remove the decoration
	protected override void _RemoveDecoration(){
		// Destroy the game object
		Destroy(decoGameObject);
	}
}
