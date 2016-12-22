using UnityEngine;

public class DecorationZoneMaterial : DecorationZone{
	public GameObject[] arrayObjects;   // The game objects for which the material should be changed
	public DecorationZoneMaterial[] otherZoneMaterials;

	void Awake() {
		otherZoneMaterials = Object.FindObjectsOfType<DecorationZoneMaterial>();
	}

	/// <summary>
	/// Child override to set decoration
	/// </summary>
	/// <param name="strID">decoID</param>
	protected override void _SetDecoration(string decoID, bool isPlacedFromDecoMode){
		// build the prefab from the id of the decoration
		string strResource = DataLoaderItems.GetDecoItemMaterialName(decoID);
		Material matPrefab = Resources.Load(strResource) as Material;
		
		for(int i = 0; i < arrayObjects.Length; ++i){
			GameObject go = arrayObjects[i];
			if(go.GetComponent<Renderer>()){
				go.GetComponent<Renderer>().material = matPrefab;
			}
		}
	}
	
	/// <summary>
	/// Child override to remove the decoration
	/// </summary>
	protected override void _RemoveDecoration(){
		// We need to remove all other wallpapers
		foreach(DecorationZoneMaterial material in otherZoneMaterials) {
			// give the user all other wallpapers back in their inventory
			if(!string.IsNullOrEmpty(material.placedDecoID)) {
				InventoryManager.Instance.AddItemToInventory(material.placedDecoID);
				
				// update the save data since this node is now empty
				DataManager.Instance.GameData.Decorations.PlacedDecorations.Remove(material.placedDecoID);
			}
		}
	}
}
