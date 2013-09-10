using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DecorationNodeObject
// This kind of decoration node instantiates objects.
//---------------------------------------------------

public class DecorationNodeObject : DecorationNode {
	
	// instantiated game object decoration
	private GameObject goDeco;
	
	//---------------------------------------------------
	// _SetDecoration()
	//---------------------------------------------------	
	protected override void _SetDecoration( string strID ) {
		// build the prefab from the id of the decoration
		string strResource = "GO_" + strID;
		GameObject goPrefab = Resources.Load(strResource) as GameObject;
		Vector3 vPos = transform.position;
		goDeco = Instantiate(goPrefab, vPos, goPrefab.transform.rotation) as GameObject;			
	}
	
	//---------------------------------------------------
	// _RemoveDecoration()
	//---------------------------------------------------	
	protected override void _RemoveDecoration() {
		// destroy the game object
		Destroy( goDeco );				
	}
	
	//---------------------------------------------------
	// HasRemoveOption()
	// Does this node currently have a decoration on it?
	//---------------------------------------------------	
	public override bool HasRemoveOption() {
		return goDeco != null;
	}		
}
