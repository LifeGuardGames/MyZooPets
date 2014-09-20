﻿using UnityEngine;
using System.Collections;

public class DecorationZoneObject : DecorationZone{

	public Transform placementNode;
	private GameObject decoGameObject;	// Instantiated game object decoration

	/// <summary>
	/// Child override to set decoration
	/// </summary>
	/// <param name="strID">decoID</param>
	protected override void _SetDecoration(string decoID){
		// Build the prefab from the id of the decoration
		string strResource = ItemLogic.Instance.GetDecoItemPrefabName(decoID);
		GameObject goPrefab = Resources.Load(strResource) as GameObject;
		
		if(goPrefab){
			decoGameObject = Instantiate(goPrefab) as GameObject;
			decoGameObject.transform.parent = placementNode;	// Put it in the hierachy of decorations in this room
			GameObjectUtils.ResetLocalPosition(decoGameObject);	// Reset all transforms
		}
		else
			Debug.LogError("No such prefab for " + strResource);
	}

	/// <summary>
	/// Child override to remove the decoration
	/// </summary>
	protected override void _RemoveDecoration(){
		// Destroy the game object
		Destroy(decoGameObject);
	}
}
