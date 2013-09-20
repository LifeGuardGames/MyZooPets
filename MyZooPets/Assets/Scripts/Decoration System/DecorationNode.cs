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
	
	// the ID of this decoration node -- changing this will corrupt the save data
	// should be unique
	public string strNodeID;
	
	// the decoration currently being displayed on this node
	private string strDecoID = string.Empty;

	void Start () {	
		// check save data to see if something was on this node
		CheckSaveData();
		
		// use event handler to listen for when the player goes into edit deco mode
		EditDecosUIManager.OnManagerOpen += OnDecoMode;
		
		// by default, decoration nodes are not visible/interactable
		ToggleNode( false );
	}
	
	void OnDestroy() {
		// remove event handler when this node is destroyed
		EditDecosUIManager.OnManagerOpen -= OnDecoMode;	
	}

	// listen for when this node is tapped/clicked -- if we change nodes to 2D, use OnPress instead
	void OnTap(TapGesture gesture) { 
		NodeClicked();	
	}

	//---------------------------------------------------
	// CheckSaveData()
	// Checks save data to see if this node has any
	// decoration saved there.  If it does, place that
	// decoration.
	//---------------------------------------------------	
	private void CheckSaveData() {
		// if the saved data contains this node's id, it means there was a decoration placed here
		if ( DataManager.Instance.Decorations.PlacedDecorations.ContainsKey( strNodeID ) ) {
			string strSavedDeco = DataManager.Instance.Decorations.PlacedDecorations[ strNodeID ];
			SetDecoration( strSavedDeco );
		}
	}
	
    //Event listener. listening to when decoration mode is enabled/disabled
    private void OnDecoMode(object sender, UIManagerEventArgs e){
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
		// inform the ui manager
		DecorationTypes eType = GetDecoType();
		
		// have the deco UI manager update itself based on this node being selected
		EditDecosUIManager.Instance.UpdateChooseMenu( this );
	}
	
	//---------------------------------------------------
	// GetDecorationID()
	// Returns this nodes decoration id.  May be null if
	// no decoration is set.
	//---------------------------------------------------	
	public string GetDecorationID() {
		return strDecoID;
	}
	
	//---------------------------------------------------
	// HasDecoration()
	// Does this node currently have a decoration on it?
	//---------------------------------------------------	
	public virtual bool HasDecoration() {
		return strDecoID != string.Empty;
	}	
	
	//---------------------------------------------------
	// HasRemoveOption()
	// Can this decoration be removed by the user?
	//---------------------------------------------------	
	public virtual bool HasRemoveOption() {
		// children implement this
		return false;
	}		
	
	//---------------------------------------------------
	// SetDecoration()
	// Sets this node's decoration to the incoming
	// decoration.
	//---------------------------------------------------	
	public void SetDecoration( string strID ) {
		// do one last check
		if ( !CanPlaceDecoration( strID ) ) {
			Debug.Log("Illegal deco placement for " + strID + " on node " + gameObject);
			return;
		}
		
		// if there was already a decoration here, remove it
		if ( HasDecoration() )
			RemoveDecoration();		
		
		// cache the id
		strDecoID = strID;
		
		// update the save data with the new decoration id
		DataManager.Instance.Decorations.PlacedDecorations[strNodeID] = strID;		
		
		// actually create/set the decoration
		_SetDecoration( strDecoID );
	}
	
	protected virtual void _SetDecoration( string strID ) {
		// children implement this
	}
	
	//---------------------------------------------------
	// CanPlaceDecoration()
	// Checks to see if the decoration with strID may
	// be placed on this node.
	//---------------------------------------------------	
	private bool CanPlaceDecoration( string strID ) {
		bool bOK = true;	// start optimistic
		
		// compare the node type to the decoration type
		DecorationItem itemDeco = (DecorationItem) DataItems.GetItem( strID );
		DecorationTypes eNodeType = GetDecoType();
		DecorationTypes eDecoType = itemDeco.DecorationType;
		bOK = eNodeType == eDecoType;
		
		return bOK;
	}
	
	//---------------------------------------------------
	// RemoveDecoration()
	// Removes the decoration from this node.
	//---------------------------------------------------	
	public void RemoveDecoration() {		
		// call child function to actually remove the decoration
		_RemoveDecoration();
		
		// give the user the decoration back in their inventory
		if ( strDecoID != null )
			InventoryLogic.Instance.AddItem(strDecoID, 1);
		else
			Debug.Log("Just removed an illegal decoration?");		
		
		// update the save data since this node is now empty
		DataManager.Instance.Decorations.PlacedDecorations.Remove( strNodeID );
		
		// reset the deco id on this node
		strDecoID = string.Empty;
	}
	
	protected virtual void _RemoveDecoration() {
		// children implement this	
	}
}
