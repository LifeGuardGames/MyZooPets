using UnityEngine;
using System.Collections;

//---------------------------------------------------
// DecorationNode
// Script that resides on node objects that define
// the type of decoration node.
//---------------------------------------------------

public class DecorationNode : MonoBehaviour {
	
	// what type of decorations can go on this node?
	public DecorationTypes eType;
	public DecorationTypes GetDecoType() {
		return eType;
	}
	
	// the decoration currently being displayed on this node
	private GameObject goDeco;

	void Start () {	
		// use event handler to listen for when the player goes into edit deco mode
		EditDecosUIManager.OnManagerOpen += OnDecoMode;
		
		// listen for when this node is tapped/clicked -- if we change nodes to 2D, use OnPress instead
        TapItem tapItem = GetComponent<TapItem>();
        if (tapItem != null){
            tapItem.OnTap += NodeClicked;
        }		
		
		// by default, decoration nodes are not visible/interactable
		ToggleNode( false );
	}
	
	void OnDestroy() {
		// remove event handler when this node is destroyed
		EditDecosUIManager.OnManagerOpen -= OnDecoMode;	
	}
	
    //Event listener. listening to when decoration mode is enabled/disabled
    private void OnDecoMode(object sender, SingletonUI<EditDecosUIManager>.UIManagerEventArgs e){
       if(e.Opening){
            ToggleNode( true );		// edit mode is opening, so turn this node on
        }else{
            ToggleNode( false );	// edit mode is closing so turn this node off
        }
    }
	
	//---------------------------------------------------
	// ToggleNode()
	// Makes the node visible or invisible.
	//---------------------------------------------------	
	private void ToggleNode( bool bOn ) {
		GetComponent<MeshRenderer>().enabled = bOn;
		GetComponent<BoxCollider>().enabled = bOn;
	}
	
	//---------------------------------------------------
	// NodeClicked()
	// Called when this node is clicked.
	//---------------------------------------------------	
	private void NodeClicked() {
		
		Debug.Log("Testing save game system: " + DataManager.Instance.Decorations.DecoTest);
		DataManager.Instance.Decorations.DecoTest += 100;
		
		// inform the ui manager
		DecorationTypes eType = GetDecoType();
		
		Debug.Log("deco node of type " + eType + " clicked");
		
		// have the deco UI manager update itself based on this node being selected
		EditDecosUIManager.Instance.UpdateChooseMenu( this );
	}
	
	//---------------------------------------------------
	// HasDecoration()
	// Does this node currently have a decoration on it?
	//---------------------------------------------------	
	public bool HasDecoration() {
		return goDeco != null;
	}
	
	//---------------------------------------------------
	// SetDecoration()
	// Sets this node's decoration to the incoming
	// decoration.
	//---------------------------------------------------	
	public void SetDecoration( GameObject goDecoNew ) {
		// if there was already a decoration here, destroy it
		if ( goDeco )
			Destroy( goDeco );
		
		goDeco = goDecoNew;
	}
	
	//---------------------------------------------------
	// RemoveDecoration()
	// Removes the decoration from this node.
	//---------------------------------------------------	
	public void RemoveDecoration() {
		if ( goDeco )
			Destroy( goDeco );
	}
}
