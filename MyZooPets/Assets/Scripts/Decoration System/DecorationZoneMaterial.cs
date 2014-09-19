﻿using UnityEngine;
using System.Collections;

public class DecorationZoneMaterial : DecorationZone{

	public GameObject[] arrayObjects;	// The game objects for which the material should be changed

	/// <summary>
	/// Child override to set decoration
	/// </summary>
	/// <param name="strID">decoID</param>
	protected override void _SetDecoration(string decoID){
		// build the prefab from the id of the decoration
		string strResource = ItemLogic.Instance.GetDecoItemMaterialName(decoID);
		Material matPrefab = Resources.Load(strResource) as Material;
		
		for(int i = 0; i < arrayObjects.Length; ++i){
			GameObject go = arrayObjects[i];
			if(go.renderer)
				go.renderer.material = matPrefab;
		}
	}
	
	/// <summary>
	/// Child override to remove the decoration
	/// </summary>
	protected override void _RemoveDecoration(){
		// We don't need to do anything, because the material is being swapped out			
	}
}
