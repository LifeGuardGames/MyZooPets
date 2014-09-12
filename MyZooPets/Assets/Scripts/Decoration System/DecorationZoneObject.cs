using UnityEngine;
using System.Collections;

public class DecorationZoneObject : DecorationZone {

	public Transform placementNode;
	private GameObject decoGameObject;	// Instantiated game object decoration
	
	protected override void _SetDecoration(string strID){
		// Build the prefab from the id of the decoration
		string strResource = ItemLogic.Instance.GetDecoItemPrefabName(strID);
		GameObject goPrefab = Resources.Load(strResource) as GameObject;
		
		if(goPrefab){
			decoGameObject = Instantiate(goPrefab) as GameObject;
			decoGameObject.transform.parent = placementNode;	// Put it in the hierachy of decorations in this room
			GameObjectUtils.ZeroLocalTransform(decoGameObject);	// Reset all transforms
		}
		else
			Debug.LogError("No such prefab for " + strResource);
	}

	protected override void _RemoveDecoration(){
		// Destroy the game object
		Destroy(decoGameObject);				
	}
}
