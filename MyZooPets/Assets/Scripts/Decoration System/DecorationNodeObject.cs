using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DecorationNodeObject
// This kind of decoration node instantiates objects.
//---------------------------------------------------

public class DecorationNodeObject : DecorationNode {
	// default object that may exist on this node
	public GameObject goDefaultDeco;
	
	// instantiated game object decoration
	private GameObject goDeco;
	
	// override for where the instantiated objects should go
	public Vector3 vOverridePos;
	
	// this is different than override pos; instead of setting the pos it adds this offset to the current pos.
	// this was probably a better idea from the start, but I didn't think of it...
	public Vector3 vOffsetPos;
	
	//---------------------------------------------------
	// _SetDecoration()
	//---------------------------------------------------	
	protected override void _SetDecoration( string strID ) {
		// first, if our defeat deco exists, destroy it
		if ( goDefaultDeco )
			Destroy( goDefaultDeco );
		
		// build the prefab from the id of the decoration
		string strResource = "GO_" + strID;
		GameObject goPrefab = Resources.Load(strResource) as GameObject;
		
		// find the right position -- most likely every object will have this override
		Vector3 vPos = transform.position;
		if ( vOverridePos != Vector3.zero )
			vPos = vOverridePos;
		else if ( vOffsetPos != Vector3.zero )
			vPos += vOffsetPos;
		
		if ( goPrefab ) {
			goDeco = Instantiate(goPrefab, vPos, goPrefab.transform.rotation) as GameObject;
			goDeco.transform.parent = transform.parent;	// put it in the hierachy of decorations in this room	
		}
		else
			Debug.LogError("No such prefab for " + strResource);
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
	
	//---------------------------------------------------
	// _SetDefaultDeco()
	//---------------------------------------------------	
	protected override void _SetDefaultDeco( string strDecoID ) {
		if ( goDefaultDeco == null ) {
			Debug.LogError("Default deco ID set but no default deco object!?!!?");
			return;
		}
		
		goDeco = goDefaultDeco;
	}	
}
