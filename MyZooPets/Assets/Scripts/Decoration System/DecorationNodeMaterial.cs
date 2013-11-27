using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DecorationNodeMaterial
// This kind of decoration node instantiates materials.
//---------------------------------------------------

public class DecorationNodeMaterial : DecorationNode {
	
	// the game objects for which the material should be changed
	public GameObject[] arrayObjects;

	//---------------------------------------------------
	// _SetDecoration()
	//---------------------------------------------------	
	protected override void _SetDecoration( string strID ) {
		// build the prefab from the id of the decoration
		string strResource = "Mat_" + strID;
		Material matPrefab = Resources.Load(strResource) as Material;
	
		for ( int i = 0; i < arrayObjects.Length; ++i ) {
			GameObject go = arrayObjects[i];
			if ( go.renderer )
				go.renderer.material = matPrefab;
		}
	}
	
	//---------------------------------------------------
	// _RemoveDecoration()
	//---------------------------------------------------	
	protected override void _RemoveDecoration() {
		// we don't need to do anything, because the material is being swapped out			
	}
	
	//---------------------------------------------------
	// HasRemoveOption()
	//---------------------------------------------------	
	public override bool HasRemoveOption() {
		return false;
	}		
	
	//---------------------------------------------------
	// _SetDefaultDeco()
	//---------------------------------------------------	
	protected override void _SetDefaultDeco( string strDecoID ) {
		// material nodes don't need to do anything special for setting a default deco
	}	
}
