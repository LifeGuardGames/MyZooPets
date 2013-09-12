using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DecorationNodeMaterial
// This kind of decoration node instantiates materials.
//---------------------------------------------------

public class DecorationNodeMaterial : DecorationNode {
	
	// the game object for which the material should be changed
	public GameObject goObject;
	
	//---------------------------------------------------
	// _SetDecoration()
	//---------------------------------------------------	
	protected override void _SetDecoration( string strID ) {
		// build the prefab from the id of the decoration
		string strResource = "MAT_" + strID;
		Material matPrefab = Resources.Load(strResource) as Material;
		
		if ( goObject && goObject.renderer )
			goObject.renderer.material = matPrefab;
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
}
